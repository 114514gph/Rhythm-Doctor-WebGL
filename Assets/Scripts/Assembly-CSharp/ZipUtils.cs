using System.IO;
using System.IO.Compression;
using System.Linq;
using UnityEngine;

public class ZipUtils
{
	public static void Unzip(string sourceArchiveFileName, string destinationDirectoryName)
	{
		if (Directory.Exists(destinationDirectoryName))
		{
			if (Directory.EnumerateFileSystemEntries(destinationDirectoryName).Any())
			{
				Debug.LogWarning("Unzip: Extracting to non-empty directory at '" + destinationDirectoryName + "'.");
			}
			else
			{
				Debug.Log("Unzip: Extracting to existing empty directory at '" + destinationDirectoryName + "'.");
			}
		}
		else
		{
			Debug.Log("Unzip: Creating directory to extract to at '" + destinationDirectoryName + "'.");
			Directory.CreateDirectory(destinationDirectoryName);
		}
		ZipFile.ExtractToDirectory(sourceArchiveFileName, destinationDirectoryName);
	}

	public static void Zip(string zipFileName, params string[] files)
	{
		string directoryName = Path.GetDirectoryName(zipFileName);
		if (File.Exists(zipFileName))
		{
			Debug.Log("Zip: Overwriting existing file at '" + zipFileName + "'.");
		}
		else if (!Directory.Exists(directoryName))
		{
			Debug.Log("Zip: Creating directory at '" + directoryName + "'.");
			Directory.CreateDirectory(directoryName);
		}
		using FileStream stream = new FileStream(zipFileName, FileMode.Create);
		using ZipArchive destination = new ZipArchive(stream, ZipArchiveMode.Create);
		foreach (string text in files)
		{
			destination.CreateEntryFromFile(text, Path.GetFileName(text));
		}
	}
}
