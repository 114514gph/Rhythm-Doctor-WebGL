using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RDLevelEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class RDPackageInstaller : RDBase
{
	public static bool anyErrors;

	public static string errorMessage;

	public static bool cancelDownload;

	public static IEnumerator GetFileNameFromUrl(string url, Action<string, string> callback)
	{
		anyErrors = false;
		errorMessage = string.Empty;
		string contentDisposition = string.Empty;
		string defaultFilePath = string.Empty;
		string filename = string.Empty;
		if (!Uri.TryCreate(url, UriKind.Absolute, out var result))
		{
			anyErrors = true;
			errorMessage = "url failed: " + url;
			Debug.Log(errorMessage);
			filename = "DownloadedLevel";
			callback?.Invoke(filename, defaultFilePath);
			yield break;
		}
		using (UnityWebRequest uwr = UnityWebRequest.Head(result))
		{
			defaultFilePath = Path.Combine(LevelValidation.TempLevelsFolder, "DownloadedLevel");
			defaultFilePath = RDUtils.GetAvailableDirectoryName(defaultFilePath);
			DownloadHandlerFile downloadHandlerFile = new DownloadHandlerFile(defaultFilePath);
			downloadHandlerFile.removeFileOnAbort = true;
			uwr.method = "HEAD";
			uwr.downloadHandler = downloadHandlerFile;
			yield return uwr.SendWebRequest();
			if (RDFile.Exists(defaultFilePath) && new FileInfo(defaultFilePath).Length == 0L)
			{
				defaultFilePath = string.Empty;
			}
			List<string> source = url.Split('/').ToList();
			if (uwr.AnyErrors())
			{
				RDBaseDllDummy.printesw(source.Last() + ": Error: " + uwr.error);
				anyErrors = true;
			}
			else
			{
				contentDisposition = uwr.GetResponseHeader("content-disposition");
				if (string.IsNullOrEmpty(contentDisposition))
				{
					contentDisposition = uwr.GetResponseHeader("Content-Disposition");
				}
			}
		}
		contentDisposition = UnityWebRequest.UnEscapeURL(contentDisposition, Encoding.UTF8);
		try
		{
			if (!string.IsNullOrEmpty(contentDisposition))
			{
				List<string> list = contentDisposition.Split(';').ToList();
				string text = string.Empty;
				foreach (string item in list)
				{
					string text2 = item.TrimStart();
					if (text2.StartsWith("filename"))
					{
						text = text2.Substring("filename".Length);
					}
				}
				if (text.StartsWith("*="))
				{
					int num = text.IndexOf("'");
					text = text.Substring(num + 2);
				}
				else if (text.StartsWith("="))
				{
					int num2 = text.IndexOf('=');
					text = text.Substring(num2 + 1);
				}
				if (text.StartsWith("\""))
				{
					text = text.Remove(0);
				}
				if (text.EndsWith("\""))
				{
					text = text.Remove(text.Length - 1);
				}
				filename = text;
			}
			if (string.IsNullOrEmpty(filename))
			{
				RDBaseDllDummy.printesw("Did not found content disposition inside the header");
				if (Uri.TryCreate(url, UriKind.Absolute, out result))
				{
					filename = Path.GetFileName(result.LocalPath);
				}
				if (string.IsNullOrEmpty(filename))
				{
					foreach (string item2 in new List<string>(url.Split('/').Reverse()))
					{
						string text3 = new string(item2.Where((char c) => !char.IsControl(c)).ToArray());
						if (!string.IsNullOrEmpty(text3) && text3.Length > 1)
						{
							filename = text3;
							RDBaseDllDummy.printesw("Couldn't get the filename from the uri: " + url + " using the segment " + filename);
							break;
						}
					}
				}
			}
			filename = string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));
		}
		catch (Exception ex)
		{
			Debug.Log("GetFileNameFromUrl: " + ex.Message);
			anyErrors = true;
			errorMessage = ex.Message;
			filename = string.Empty;
		}
		if (filename.EndsWith(", attachment"))
		{
			string text4 = filename;
			int length = ", attachment".Length;
			filename = text4.Substring(0, text4.Length - length);
		}
		if (string.IsNullOrEmpty(filename))
		{
			filename = "DownloadedLevel";
		}
		callback?.Invoke(filename, defaultFilePath);
	}

	public static IEnumerator DownloadPackage(string url, string resultFilePath, Text progressText = null)
	{
		anyErrors = false;
		errorMessage = string.Empty;
		if (!Uri.TryCreate(url, UriKind.Absolute, out var result))
		{
			anyErrors = true;
			errorMessage = RDString.Get("rdPackageInstaller.downloadError");
			Debug.Log("DownloadPackage " + url + ", " + resultFilePath + ", " + errorMessage);
			yield break;
		}
		using UnityWebRequest uwr = new UnityWebRequest(result);
		try
		{
			DownloadHandlerFile downloadHandlerFile = new DownloadHandlerFile(resultFilePath);
			downloadHandlerFile.removeFileOnAbort = true;
			uwr.method = "GET";
			uwr.downloadHandler = downloadHandlerFile;
		}
		catch (Exception ex)
		{
			Debug.LogError(ex.Message);
			anyErrors = true;
			errorMessage = ex.Message;
		}
		uwr.SendWebRequest();
		MonoBehaviour coroutineRunner = ((scnCLS.instance != null) ? ((RDBaseDllDummy)scnCLS.instance) : ((RDBaseDllDummy)scnEditor.instance));
		if (coroutineRunner == null)
		{
			throw new Exception("RDPackageInstaller.DownloadPackage was called but editor and cls are both null!");
		}
		Coroutine downloadNarration = coroutineRunner.StartCoroutine(NarrateDownloadProgress(uwr, progressText));
		while (!uwr.isDone)
		{
			if (cancelDownload)
			{
				cancelDownload = false;
				uwr.Abort();
				coroutineRunner.StopCoroutine(downloadNarration);
				yield break;
			}
			if (progressText != null)
			{
				progressText.text = Mathf.RoundToInt(uwr.downloadProgress * 100f) + "%";
			}
			yield return null;
		}
		coroutineRunner.StopCoroutine(downloadNarration);
		if (uwr.AnyErrors())
		{
			Debug.LogError(uwr.error);
			anyErrors = true;
			errorMessage = uwr.error;
		}
		else if (!RDFile.Exists(resultFilePath))
		{
			errorMessage = RDString.Get("rdPackageInstaller.downloadError");
			anyErrors = true;
			Debug.LogError(uwr.error);
		}
	}

	public static IEnumerator NarrateDownloadProgress(UnityWebRequest uwr, Text progressText)
	{
		yield return new WaitForSecondsRealtime(2f);
		int lastNarratedProgress = 0;
		while (!uwr.isDone && Narration.IsEnabled)
		{
			int num = Mathf.RoundToInt(uwr.downloadProgress * 100f);
			if (num >= lastNarratedProgress + 5)
			{
				Narration.Say(progressText?.text, NarrationCategory.Notification, false);
				lastNarratedProgress = num;
				yield return new WaitForSecondsRealtime(0.75f);
			}
			else
			{
				yield return null;
			}
		}
	}

	public static void CheckFileIsZip(string filePath)
	{
		anyErrors = false;
		errorMessage = string.Empty;
		try
		{
			string empty = string.Empty;
			if (RDUtils.GetFileBuffer(filePath, 0, 4, out var buffer))
			{
				empty = Encoding.ASCII.GetString(buffer);
				char[] array = empty.ToCharArray();
				StringBuilder stringBuilder = new StringBuilder(empty.Length);
				char[] array2 = array;
				foreach (char c in array2)
				{
					if (!char.IsControl(c))
					{
						stringBuilder.Append(c);
					}
				}
				empty = stringBuilder.ToString();
				if (!("PK" == empty))
				{
					RDBaseDllDummy.printesw("The file at path: " + filePath + " doesn't start with the zip header: PK != " + empty);
					anyErrors = true;
					errorMessage = RDString.Get("rdPackageInstaller.fakeZipError");
				}
			}
			else
			{
				RDBaseDllDummy.printesw("Couldn't read bytes from: " + filePath);
				anyErrors = true;
				errorMessage = RDString.Get("rdPackageInstaller.fileAccessError");
			}
		}
		catch (Exception ex)
		{
			RDBaseDllDummy.printesw(ex.Message);
			anyErrors = true;
			errorMessage = ex.Message;
		}
	}

	public static string UnzipAndFindLevel(string packagePath, string newDirectoryName = null)
	{
		RDBaseDllDummy.printem("UnzipAndFindLevel: " + packagePath + "   newDirectoryName: " + newDirectoryName);
		try
		{
			CheckFileIsZip(packagePath);
			if (!anyErrors)
			{
				string path = (string.IsNullOrEmpty(newDirectoryName) ? Path.GetFileNameWithoutExtension(packagePath) : newDirectoryName);
				string directoryPath = Path.Combine(Path.GetDirectoryName(packagePath), path);
				directoryPath = RDUtils.GetAvailableDirectoryName(directoryPath);
				RDBaseDllDummy.printes("Unzipping " + packagePath + "  at path:  " + directoryPath);
				ZipUtils.Unzip(packagePath, directoryPath);
				string result = FindRDLevel(directoryPath);
				if (!anyErrors)
				{
					return result;
				}
			}
		}
		catch (Exception ex)
		{
			RDBaseDllDummy.printesw(ex.Message);
			RDBaseDllDummy.printesw(ex.StackTrace);
			anyErrors = true;
			errorMessage = RDString.Get("editor.openURL.genericError").Replace("[errorOpeningLevel]", ex.Message);
		}
		return null;
	}

	public static string FindRDLevel(string sourceFolderPath)
	{
		anyErrors = false;
		errorMessage = string.Empty;
		string text = Path.Combine(sourceFolderPath, "main.rdlevel");
		if (RDFile.Exists(text))
		{
			return text;
		}
		try
		{
			string[] filesWithExtension = sourceFolderPath.GetFilesWithExtension("rdlevel", SearchOption.AllDirectories);
			if (filesWithExtension.Length != 0)
			{
				string mainRDLevelWithoutExtension = Path.GetFileNameWithoutExtension("main.rdlevel");
				string text2 = Array.Find(filesWithExtension, (string rdlevel) => Path.GetFileNameWithoutExtension(rdlevel) == mainRDLevelWithoutExtension);
				if (string.IsNullOrEmpty(text2))
				{
					string backupRDLevelWithoutExtension = Path.GetFileNameWithoutExtension("backup.rdlevel");
					text2 = Array.Find(filesWithExtension, (string rdlevel) => Path.GetFileNameWithoutExtension(rdlevel) != backupRDLevelWithoutExtension);
					if (string.IsNullOrEmpty(text2))
					{
						text2 = filesWithExtension[0];
					}
				}
				return text2;
			}
			RDBaseDllDummy.printesw("The folder at path: " + sourceFolderPath + " doesn't contain any file with the extension *.rdlevel");
			errorMessage = RDString.Get("rdPackageInstaller.rdLevelNotFound");
			anyErrors = true;
		}
		catch (Exception ex)
		{
			RDBaseDllDummy.printesw(ex.Message);
			anyErrors = true;
			errorMessage = ex.Message;
		}
		return null;
	}

	public static string MoveRDLevelFolder(string sourceFolderPath, string targetFolderPath)
	{
		anyErrors = false;
		errorMessage = string.Empty;
		string text = string.Empty;
		try
		{
			string[] directories = Directory.GetDirectories(sourceFolderPath, "*", SearchOption.TopDirectoryOnly);
			foreach (string obj in directories)
			{
				RDUtils.SetDirectoryAttributes(obj);
				Directory.Delete(obj, recursive: true);
			}
			string path = new DirectoryInfo(sourceFolderPath).Name;
			text = Path.Combine(targetFolderPath, path);
			text = RDUtils.GetAvailableDirectoryName(text);
			try
			{
				RDUtils.SetDirectoryAttributes(sourceFolderPath);
				Directory.Move(sourceFolderPath, text);
			}
			catch (Exception ex)
			{
				Debug.LogError("Error moving directory: " + ex.Message + ". Copying folder instead from:  " + sourceFolderPath + "   to:  " + targetFolderPath);
				RDUtils.CopyDirectory(sourceFolderPath, text);
			}
		}
		catch (Exception ex2)
		{
			Debug.LogError("Error moving directory: " + ex2.Message + ". Tried moving folder from:  " + sourceFolderPath + "   to:  " + targetFolderPath);
			anyErrors = true;
			errorMessage = ex2.Message;
		}
		return text;
	}
}
