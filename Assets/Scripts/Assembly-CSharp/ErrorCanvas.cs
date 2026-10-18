using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ErrorCanvas : RDBase
{
	public const string ErrorsDirectoryName = "Errors";

	public const string ScreenshotName = "Screenshot.png";

	public const string LogFileName = "errorOutput.log";

	public GameObject containerImage;

	public Text errorText;

	private string originalLogPath;

	public static string ErrorDirectoryPath => Path.Combine(Application.persistentDataPath, "Errors");

	public void StartErrorProcess(string errorMessage)
	{
		string errorDirectoryPath = ErrorDirectoryPath;
		try
		{
			if (Directory.Exists(errorDirectoryPath))
			{
				RDUtils.SetDirectoryAttributes(errorDirectoryPath);
				Directory.Delete(errorDirectoryPath, recursive: true);
			}
			Directory.CreateDirectory(errorDirectoryPath);
			ScreenCapture.CaptureScreenshot(Path.Combine(errorDirectoryPath, "Screenshot.png"));
			StartCoroutine(PostErrorFrameActions(errorMessage));
		}
		catch (Exception ex)
		{
			Debug.Log("StartErrorProcess: " + ex.Message);
		}
	}

	private IEnumerator PostErrorFrameActions(string errorMessage)
	{
		yield return null;
		originalLogPath = Path.Combine(Application.persistentDataPath, "output_log.txt");
		string text = Path.Combine(ErrorDirectoryPath, "errorOutput.log");
		RDBaseDllDummy.printem("Copying Log from " + originalLogPath + " to " + text);
		try
		{
			if (File.Exists(originalLogPath))
			{
				if (File.Exists(text))
				{
					File.SetAttributes(text, FileAttributes.Normal);
					File.Delete(text);
				}
				File.Copy(originalLogPath, text, overwrite: true);
			}
		}
		catch (Exception ex)
		{
			Debug.Log("Error copying output log from " + originalLogPath + " to " + text + ":  " + ex.Message);
		}
		containerImage.SetActive(value: true);
		errorText.text = errorMessage;
	}

	public void Quit()
	{
		Application.Quit();
	}

	public void ShowInputFileLocation()
	{
		string directoryName = Path.GetDirectoryName(originalLogPath);
		RDBaseDllDummy.printem("Original log path: " + directoryName);
		RDUtils.RevealInExplorer(directoryName);
	}
}
