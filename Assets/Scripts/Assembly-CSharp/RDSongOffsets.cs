using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[CreateAssetMenu]
public class RDSongOffsets : ScriptableObject
{
	private static RDSongOffsets internalData;

	private const string AudioPath = "/TrueAssets/Audio/Resources/";

	private const string InternalLevelsPath = "/Resources/InternalLevels/";

	[Header("Master Volumes")]
	public float GlobalSongVolumeDebug;

	public float GlobalBeatsoundVolumeDebug;

	public float GlobalNonBeatSoundVolumeDebug;

	[Header("Specific Volumes")]
	public float GlobalJyiSoundVolumeDebug;

	public float GlobalHandPopSoundVolumeDebug;

	public List<SongOffset> misc;

	public HashSet<string> alreadyWarnedUser = new HashSet<string>();

	[NonSerialized]
	private bool hasSetup;

	[NonSerialized]
	public Dictionary<string, SongOffset> data;

	[NonSerialized]
	public Dictionary<string, SongOffset> externalData;

	public static RDSongOffsets instance
	{
		get
		{
			if (internalData == null)
			{
				internalData = Resources.Load<RDSongOffsets>("RDSongOffsets");
				internalData.Setup();
			}
			return internalData;
		}
	}

	public void Setup()
	{
		if (data != null)
		{
			return;
		}
		hasSetup = true;
		data = new Dictionary<string, SongOffset>();
		externalData = new Dictionary<string, SongOffset>();
		foreach (SongOffset item in misc)
		{
			if (!data.ContainsKey(item.name))
			{
				data.Add(item.name, item);
			}
		}
	}

	public SongOffset Get(string name)
	{
		if (!hasSetup)
		{
			Setup();
		}
		string text = ".ogg";
		if (name.EndsWith(text))
		{
			name = name.Replace(text, "");
		}
		if (data.ContainsKey(name))
		{
			return data[name];
		}
		if (data.ContainsKey("snd" + name))
		{
			return data["snd" + name];
		}
		if (!alreadyWarnedUser.Contains(name))
		{
			alreadyWarnedUser.Add(name);
		}
		return new SongOffset
		{
			name = name,
			offset = 0f,
			volume = 1f,
			numberOfBeatsFromSongStart = 0,
			prebarBeats = 0,
			dummy = true
		};
	}

	public void UpdateData()
	{
		Setup();
		SongOffset temp = new SongOffset();
		int lastSlash;
		int lastDot;
		string filename;
		AddSongOffsets(Application.dataPath + "/TrueAssets/Audio/Resources/", keepExtension: false);
		void AddSongOffsets(string folder, bool keepExtension)
		{
			foreach (string item in Directory.GetFiles(folder, "*", SearchOption.AllDirectories).ToList())
			{
				if (item.HasAudioFileExtension())
				{
					int num = item.LastIndexOf("/");
					int num2 = item.LastIndexOf("\\");
					lastSlash = ((num > num2) ? num : num2);
					lastDot = item.LastIndexOf(".");
					string text = item.Substring(0, lastSlash).Replace(folder, string.Empty);
					text = text.Replace('\\', '/');
					filename = item.Substring(lastSlash + 1, lastDot - lastSlash - 1);
					if (data.ContainsKey(filename))
					{
						temp = data[filename];
						temp.folder = text;
						data[filename] = temp;
					}
					else
					{
						Debug.Log("Added: " + filename);
						temp = new SongOffset();
						temp.name = filename;
						temp.volume = 1f;
						temp.folder = text;
						misc.Add(temp);
						data.Add(temp.name, temp);
					}
				}
			}
		}
	}

	public void ImportFromCSV(string csvText)
	{
		if (!hasSetup || data == null)
		{
			Setup();
		}
		string[] array = csvText.Split('\n');
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(',');
			string text = array2[0];
			int num = text.LastIndexOf(".");
			if (num >= 0)
			{
				text = text.Substring(0, num);
			}
			if (text == "Filename")
			{
				Debug.Log("skipping header");
				continue;
			}
			float offset = (string.IsNullOrEmpty(array2[1].Trim()) ? 0f : float.Parse(array2[1]));
			float volume = (string.IsNullOrEmpty(array2[2].Trim()) ? 1f : float.Parse(array2[2]));
			if (!data.ContainsKey(text))
			{
				Debug.LogWarning("Not Found: " + text);
				continue;
			}
			data[text].offset = offset;
			data[text].volume = volume;
		}
	}

	public void ExportToCSV()
	{
		if (!hasSetup || data == null)
		{
			Setup();
		}
		string text = "Filename, Offset, Volume\n";
		foreach (SongOffset item in misc)
		{
			text += $"{item.name}, {item.offset}, {item.volume}\n";
		}
		StreamWriter streamWriter = File.CreateText(Application.dataPath + "/ExportedSongOffsets.csv");
		streamWriter.Write(text);
		streamWriter.Close();
	}
}
