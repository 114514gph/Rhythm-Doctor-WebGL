using System;
using System.Collections.Generic;
using System.IO;
using RDLevelEditor;
using Steamworks;
using UnityEngine;

[Serializable]
public class CustomLevelData
{
	public const float DefaultBpm = 100f;

	public const string WorkshopItemURLPrefix = "http://steamcommunity.com/sharedfiles/filedetails/?id=";

	[NonSerialized]
	public PublishedFileId_t publishedId;

	public string name;

	public RDLevelSettings settings;

	public Texture syringeIcon;

	public string path;

	public string mainRDLevelFilePath;

	public bool isFromSteam;

	public bool isLegacyLevel;

	public List<string> tags;

	private string _hash;

	public string Hash
	{
		get
		{
			if (string.IsNullOrEmpty(_hash))
			{
				if (isLegacyLevel)
				{
					string text = new DirectoryInfo(path).Name;
					_hash = RDUtils.GetHash(text);
				}
				else
				{
					_hash = RDUtils.GetHash(settings.author, settings.artist, settings.song);
				}
			}
			return _hash;
		}
	}

	public CustomLevelData()
	{
		tags = new List<string>();
		settings = default;
	}

	public CustomLevelData(bool isFromSteam)
	{
		this.isFromSteam = isFromSteam;
		settings = default;
		tags = new List<string>();
	}
}
