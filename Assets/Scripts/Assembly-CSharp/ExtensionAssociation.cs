#if !UNITY_WEBGL
using System;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using UnityEngine;

namespace RDLevelEditor
{
	public static class ExtensionAssociation
	{
		private const string ExeName = "Rhythm Doctor Editor.exe";

		private const string FileRegPath = "Software\\Classes\\Applications\\";

		private const string OpenAppRegPath = "\\shell\\open\\command";

		private const string AppAssocPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FileExts\\";

		[DllImport("Shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern void SHChangeNotify(uint wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);

		public static string CheckForFileExtension(string extension)
		{
			string[] commandLineArgs = Environment.GetCommandLineArgs();
			if (commandLineArgs.Length != 0)
			{
				for (int i = 0; i < commandLineArgs.Length; i++)
				{
					if (commandLineArgs[i].Contains(extension))
					{
						return commandLineArgs[i];
					}
				}
			}
			return null;
		}

		public static void CheckExtensionAssociation()
		{
			string text = "Rhythm Doctor Editor.exe";
			string dataPath = Application.dataPath;
			dataPath = dataPath.Replace(text.Replace(".exe", string.Empty) + "_Data", string.Empty);
			dataPath = dataPath + "/" + text;
			dataPath = dataPath.Replace("/", "\\");
			if (Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FileExts\\.rdlevel\\UserChoice", writable: false) == null || Registry.CurrentUser.OpenSubKey("Software\\Classes\\Applications\\" + text + "\\shell\\open\\command", writable: false) == null || Registry.CurrentUser.OpenSubKey("Software\\Classes\\Applications\\" + text + "\\shell\\open\\command", writable: false).GetValue("version") == null || Convert.ToInt32(Registry.CurrentUser.OpenSubKey("Software\\Classes\\Applications\\" + text + "\\shell\\open\\command", writable: false).GetValue("version")) < 68 || (!Convert.ToString(Registry.CurrentUser.OpenSubKey("Software\\Classes\\Applications\\" + text + "\\shell\\open\\command", writable: false).GetValue("")).Equals("\"" + dataPath + "\" \"%1\"") && Convert.ToInt32(Registry.CurrentUser.OpenSubKey("Software\\Classes\\Applications\\" + text + "\\shell\\open\\command", writable: false).GetValue("version")) < 68))
			{
				string value = "Rhythm Doctor Level";
				RegistryKey registryKey = Registry.CurrentUser.CreateSubKey("Software\\Classes\\.rdlevel");
				RegistryKey registryKey2 = Registry.CurrentUser.CreateSubKey("Software\\Classes\\Applications\\" + text);
				RegistryKey registryKey3 = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FileExts\\.rdlevel");
				registryKey.CreateSubKey("DefaultIcon").SetValue("", dataPath + ",0");
				registryKey.CreateSubKey("PerceivedType").SetValue("", "Text");
				registryKey2.SetValue("", value);
				registryKey2.CreateSubKey("shell\\open\\command").SetValue("", "\"" + dataPath + "\" \"%1\"");
				registryKey2.CreateSubKey("shell\\open\\command").SetValue("version", 68);
				registryKey2.CreateSubKey("shell\\edit\\command").SetValue("", "\"" + dataPath + "\" \"%1\"");
				registryKey2.CreateSubKey("DefaultIcon").SetValue("", dataPath + ",0");
				registryKey3.CreateSubKey("OpenWithList").SetValue("", text);
				registryKey3.CreateSubKey("OpenWithList").SetValue("a", text);
				registryKey3.CreateSubKey("OpenWithList").SetValue("b", text);
				registryKey3.CreateSubKey("OpenWithList").SetValue("a", text);
				registryKey3.CreateSubKey("OpenWithList").SetValue("b", text);
				registryKey3.CreateSubKey("OpenWithList").SetValue("a", text);
				registryKey3.CreateSubKey("OpenWithList").SetValue("b", text);
				registryKey3.CreateSubKey("OpenWithList").SetValue("c", text);
				registryKey3.CreateSubKey("OpenWithList").SetValue("d", text);
				registryKey3.CreateSubKey("OpenWithList").SetValue("MRUList", "abcde");
				SHChangeNotify(134217728u, 0u, IntPtr.Zero, IntPtr.Zero);
			}
		}
	}
}

#endif
