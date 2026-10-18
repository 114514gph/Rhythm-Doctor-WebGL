using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RDLevelEditor;
using UnityEngine;
using UnityEngine.Networking;

public static class DesktopLevelLoader
{
	public const string LocalLevelIdentifier = "Local";

	public const string SteamLevelIdentifier = "Steam";

	private static string _cacheLevelsFilePath;

	public static UnityWebRequest uwrAudio;

	private static Texture2D tempTexture;

	private static Dictionary<string, RDLevelSettings> cacheLevelsDict;

	private static List<string> usedLevelsKeyList;

	private static string CacheLevelsFilePath
	{
		get
		{
			if (string.IsNullOrEmpty(_cacheLevelsFilePath))
			{
				_cacheLevelsFilePath = Path.Combine(Persistence.DataPath, "LevelsCache.rd");
			}
			if (!RDFile.Exists(_cacheLevelsFilePath))
			{
				File.Create(_cacheLevelsFilePath);
			}
			File.SetAttributes(_cacheLevelsFilePath, FileAttributes.Normal);
			return _cacheLevelsFilePath;
		}
	}

	public static void InitializeCacheLevelsDict()
	{
		usedLevelsKeyList = new List<string>();
		cacheLevelsDict = new Dictionary<string, RDLevelSettings>();
		try
		{
			string text = RDFile.ReadAllText(CacheLevelsFilePath);
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			Dictionary<string, object> dictionary = Json.Deserialize(text) as Dictionary<string, object>;
			foreach (KeyValuePair<string, object> item in dictionary)
			{
				Dictionary<string, object> dictionary2 = dictionary[item.Key] as Dictionary<string, object>;
				if (dictionary2.ContainsKey("firstSongOffset"))
				{
					RDLevelSettings value = default;
					value.Decode(dictionary2);
					cacheLevelsDict.Add(item.Key, value);
				}
			}
		}
		catch (Exception ex)
		{
			Debug.Log("3 - Error loading cache levels " + CacheLevelsFilePath + ": " + ex.Message);
		}
	}

	public static void LoadLevelsPreviewFromPath(string levelsDirectoryPath)
	{
		if (Directory.Exists(levelsDirectoryPath))
		{
			string[] directories = Directory.GetDirectories(levelsDirectoryPath);
			for (int i = 0; i < directories.Length; i++)
			{
				LoadLevelPreview(directories[i]);
			}
		}
	}

	public static void LoadLevelPreview(string levelDirectory, bool fromSteam = false)
	{
		CustomLevelData customLevelData = new CustomLevelData(fromSteam);
		customLevelData.path = levelDirectory;
		string obj = (fromSteam ? "Steam" : "Local");
		string name = new DirectoryInfo(levelDirectory).Name;
		string text = obj + "/" + name;
		bool flag = !cacheLevelsDict.ContainsKey(text);
		string text2 = string.Empty;
		string text3 = string.Empty;
		_ = string.Empty;
		if (!flag)
		{
			customLevelData.settings = cacheLevelsDict[text];
			text3 = Path.Combine(levelDirectory, customLevelData.settings.mainRDLevelRelativePath);
			text2 = new FileInfo(text3).LastWriteTimeUtc.ToString();
			flag = text2 != customLevelData.settings.lastModifiedTime;
		}
		if (flag)
		{
			if (string.IsNullOrEmpty(text3))
			{
				text3 = RDPackageInstaller.FindRDLevel(levelDirectory);
				if (string.IsNullOrEmpty(text3) || !RDFile.Exists(text3))
				{
					Debug.Log("No rdlevel was found inside: " + levelDirectory);
					return;
				}
			}
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			string text4 = string.Empty;
			try
			{
				text4 = RDFile.ReadAllText(text3);
				dictionary = Json.Deserialize(text4) as Dictionary<string, object>;
			}
			catch (Exception exception)
			{
				Debug.Log("4 - Error loading " + text3 + " - Error desearializing: '" + text4 + "'. See exception below.");
				Debug.LogException(exception);
			}
			if (dictionary == null || dictionary.Count <= 1)
			{
				Debug.Log($"{text3} - rootDict is null {dictionary == null} or count <= 1: {dictionary != null && dictionary.Count <= 1}, text: \n{text4}");
				return;
			}
			RDLevelData rDLevelData = new RDLevelData(dictionary, onlyActiveEvents: true, onlySettings: true);
			customLevelData.settings = rDLevelData.settings;
			customLevelData.settings.firstSongoffset = rDLevelData.GetFirstSongOffset();
			customLevelData.settings.firstSongFileName = rDLevelData.GetFirstSongFileName();
			string previewSongName = customLevelData.settings.previewSongName;
			customLevelData.settings.bpm = rDLevelData.GetSongBPM(previewSongName);
			if (string.IsNullOrEmpty(text2))
			{
				text2 = new FileInfo(text3).LastWriteTimeUtc.ToString();
			}
			customLevelData.settings.lastModifiedTime = text2;
		}
		customLevelData.mainRDLevelFilePath = text3;
		customLevelData.name = customLevelData.settings.song;
		string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(text3);
		string fileNameWithoutExtension2 = Path.GetFileNameWithoutExtension("main.rdlevel");
		customLevelData.isLegacyLevel = fileNameWithoutExtension != fileNameWithoutExtension2;
		customLevelData.settings.mainRDLevelRelativePath = RDUtils.GetRelativePath(text3, levelDirectory);
		if (customLevelData.isLegacyLevel)
		{
			if (string.IsNullOrEmpty(customLevelData.settings.song))
			{
				customLevelData.settings.song = Path.GetFileNameWithoutExtension(customLevelData.path);
			}
			if (string.IsNullOrEmpty(customLevelData.settings.artist))
			{
				customLevelData.settings.artist = new DirectoryInfo(Path.GetDirectoryName(customLevelData.path)).Name;
			}
			customLevelData.name = Path.GetFileNameWithoutExtension(text3);
			customLevelData.settings.canBePlayedOn = LevelPlayMode.BothModes;
			scnCLS.instance.levelsData.Add(customLevelData);
		}
		else
		{
			List<LevelErrorName> list = null;
			if (flag)
			{
				list = LevelValidation.ForceSettingsValidation(levelDirectory, ref customLevelData.settings);
				if (!LevelValidation.CheckLevelForErrors(new List<LevelErrorName>
				{
					LevelErrorName.LevelDataNull,
					LevelErrorName.LevelSettingsNull
				}, list))
				{
					RDBaseDllDummy.printem("- PUBLISHED LEVEL HAS ERRORS: " + levelDirectory);
					return;
				}
			}
			scnCLS.instance.levelsData.Add(customLevelData);
		}
		AddOrUpdateLevelCache(text, customLevelData.settings);
	}

	public static void LoadImageAsyncFromPath(string path, CustomLevelData level, Action<Texture> onLoadedSuccessfully)
	{
		string absoluteUri = new Uri(path).AbsoluteUri;
		absoluteUri = absoluteUri.Replace("+", "%2B");
		try
		{
			UnityWebRequest uwrImage = UnityWebRequestTexture.GetTexture(absoluteUri);
			uwrImage.SendWebRequest().completed += (AsyncOperation _) =>
			{
				if (uwrImage.AnyErrors())
				{
					Debug.Log("Error loading image async at: '" + path + "'. Error = '" + uwrImage.error + "'.");
				}
				else
				{
					Texture content = DownloadHandlerTexture.GetContent(uwrImage);
					onLoadedSuccessfully(content);
				}
			};
		}
		catch (Exception exception)
		{
			Debug.Log("1 - Error loading image " + path + ". See following exception.");
			Debug.LogException(exception);
		}
	}

	public static void AddOrUpdateLevelCache(string levelKey, RDLevelSettings levelSettings)
	{
		usedLevelsKeyList.Add(levelKey);
		if (cacheLevelsDict.ContainsKey(levelKey))
		{
			cacheLevelsDict[levelKey] = levelSettings;
		}
		else
		{
			cacheLevelsDict.Add(levelKey, levelSettings);
		}
	}

	public static void RemoveUnusedLevelsCacheFromDict()
	{
		usedLevelsKeyList = usedLevelsKeyList.Distinct().ToList();
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, RDLevelSettings> item in cacheLevelsDict)
		{
			if (!usedLevelsKeyList.Contains(item.Key))
			{
				list.Add(item.Key);
			}
		}
		foreach (string item2 in list)
		{
			RDBaseDllDummy.printem("Removing " + item2);
			cacheLevelsDict.Remove(item2);
		}
		usedLevelsKeyList = new List<string>();
	}

	public static void SaveCacheLevelsDict()
	{
		RDEditorUtils.IndentationLevel = 0;
		StringBuilder stringBuilder = new StringBuilder(100000);
		stringBuilder.Append(RDEditorUtils.OpenDictionary());
		RDEditorUtils.IndentationLevel = 1;
		List<KeyValuePair<string, RDLevelSettings>> list = cacheLevelsDict.ToList();
		foreach (KeyValuePair<string, RDLevelSettings> item in list)
		{
			stringBuilder.Append(RDEditorUtils.OpenDictionary(item.Key));
			stringBuilder.Append(item.Value.Encode(includeCacheData: true));
			bool isLastMember = item.Key == list.Last().Key;
			stringBuilder.Append(RDEditorUtils.CloseDictionary(isLastMember));
		}
		RDEditorUtils.IndentationLevel = 0;
		stringBuilder.Append(RDEditorUtils.CloseDictionary(isLastMember: true));
		RDFile.WriteAllText(CacheLevelsFilePath, stringBuilder.ToString());
	}

	public static IEnumerator LoadPendingLevelFiles(CustomLevelData level, bool onlyPreviewSong)
	{
		string levelPath = level.path;
		RDLevelSettings levelSettings = level.settings;
		if (!onlyPreviewSong)
		{
			tempTexture = null;
			_ = string.Empty;
			string imagePath = GetValidImageInPath(levelPath, levelSettings.previewImageName);
			if (!string.IsNullOrEmpty(imagePath))
			{
				yield return LoadImageFromPath(imagePath);
				if (tempTexture != null)
				{
					tempTexture.filterMode = FilterMode.Trilinear;
					scnCLS.instance.AddLevelFileDataToCache(tempTexture, levelPath);
				}
				else
				{
					RDBaseDllDummy.printem("Couldn't load the texture of " + levelSettings.levelName + ": " + imagePath);
				}
			}
		}
		string text = Path.Combine(levelPath, levelSettings.previewSongName);
		if (!RDFile.Exists(text))
		{
			RDBaseDllDummy.printem(levelSettings.levelName + ": Couldn't load the preview song, trying to load the first playsong instead");
			char directorySeparatorChar = Path.DirectorySeparatorChar;
			text = levelPath + directorySeparatorChar + levelSettings.firstSongFileName;
			if (!RDFile.Exists(text))
			{
				RDBaseDllDummy.printem(levelSettings.levelName + ": couldn't load the first playsong");
				text = null;
			}
		}
		if (!string.IsNullOrEmpty(text))
		{
			LoadAudioClipToLevel(text, levelPath);
		}
	}

	public static string GetValidImageInPath(string imagePath, string imageName)
	{
		string result = string.Empty;
		string imageExtension = Path.GetExtension(imageName).ToLowerInvariant();
		if (!string.IsNullOrEmpty(Array.Find(GC.SupportedImageFiles, (string extension) => "." + extension == imageExtension)))
		{
			result = Path.Combine(imagePath, imageName);
		}
		return result;
	}

	public static IEnumerator LoadImageFromPath(string path)
	{
		string absoluteUri = new Uri(path).AbsoluteUri;
		absoluteUri = absoluteUri.Replace("+", "%2B");
		using UnityWebRequest uwrImage = UnityWebRequestTexture.GetTexture(absoluteUri);
		yield return uwrImage.SendWebRequest();
		try
		{
			if (uwrImage.AnyErrors())
			{
				Debug.Log("error loading image: " + path + ", " + uwrImage.error);
				yield break;
			}
			tempTexture = DownloadHandlerTexture.GetContent(uwrImage);
			if (tempTexture.width == 120 && tempTexture.height == 85)
			{
				tempTexture.filterMode = FilterMode.Point;
			}
		}
		catch (Exception ex)
		{
			Debug.Log("2 - Error loading image " + path + ": " + ex.Message);
		}
	}

	public static void LoadAudioClipToLevel(string fullPath, string levelPath)
	{
		AudioType audioType = RDUtils.GetAudioType(Path.GetExtension(fullPath));
		if (audioType == AudioType.UNKNOWN)
		{
			Debug.Log("Extension from preview song file is not supported: " + fullPath);
			scnCLS.instance.previewSongPlayer.Stop();
			return;
		}
		string converted = new Uri(fullPath).AbsoluteUri;
		converted = converted.Replace("+", "%2B");
		uwrAudio = UnityWebRequestMultimedia.GetAudioClip(converted, audioType);
		uwrAudio.SendWebRequest().completed += (AsyncOperation asyncOperation) =>
		{
			if (uwrAudio.AnyErrors())
			{
				Debug.Log("error loading audio file at: " + fullPath + ", " + uwrAudio.error);
			}
			else
			{
				(uwrAudio.downloadHandler as DownloadHandlerAudioClip).streamAudio = true;
				if (scnCLS.instance.lastLevelsPendingData.Contains(levelPath))
				{
					scnCLS.LevelPendingData obj = (scnCLS.LevelPendingData)scnCLS.instance.lastLevelsPendingData[levelPath];
					obj.clip = DownloadHandlerAudioClip.GetContent(uwrAudio);
					obj.clip.name = converted;
				}
				else
				{
					RDBaseDllDummy.printem("Didn't find pending data for level path: \"" + levelPath + "\"");
				}
			}
			uwrAudio.Dispose();
			uwrAudio = null;
		};
	}
}
