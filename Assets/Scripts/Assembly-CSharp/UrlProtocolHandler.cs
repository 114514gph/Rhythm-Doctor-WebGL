#if !UNITY_WEBGL
using System;
using System.IO;
using Microsoft.Win32;
using UnityEngine;

public static class UrlProtocolHandler
{
	private const string AppProtocolName = "rhythmdr";

	public static void SetupProtocol()
	{
		try
		{
			RegistryKey registryKey = Registry.CurrentUser.CreateSubKey("Software\\Classes\\rhythmdr", RegistryKeyPermissionCheck.ReadWriteSubTree);
			registryKey.SetValue(string.Empty, "URL:rhythmdr Protocol", RegistryValueKind.String);
			registryKey.SetValue("URL Protocol", string.Empty, RegistryValueKind.String);
			RegistryKey registryKey2 = registryKey.CreateSubKey("DefaultIcon", RegistryKeyPermissionCheck.ReadWriteSubTree);
			string text = Path.Combine(Path.GetDirectoryName(Application.dataPath), Application.productName + ".exe");
			registryKey2.SetValue(string.Empty, text + ",1", RegistryValueKind.String);
			registryKey.CreateSubKey("Shell", RegistryKeyPermissionCheck.ReadWriteSubTree).CreateSubKey("Open", RegistryKeyPermissionCheck.ReadWriteSubTree).CreateSubKey("Command", RegistryKeyPermissionCheck.ReadWriteSubTree)
				.SetValue(value: "\"" + text + "\" \"%1\"", name: string.Empty, valueKind: RegistryValueKind.String);
		}
		catch (Exception ex)
		{
			Debug.Log("Error setting up rhythmdr protocol: " + ex.Message);
		}
	}

	public static string GetArgument()
	{
		string[] commandLineArgs = Environment.GetCommandLineArgs();
		string result = string.Empty;
		string[] array = commandLineArgs;
		foreach (string text in array)
		{
			if (text.StartsWith("rhythmdr"))
			{
				result = text.Replace("rhythmdr://", string.Empty);
				break;
			}
		}
		return result;
	}
}

#endif
