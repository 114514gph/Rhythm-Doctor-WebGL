using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RDLevelEditor
{
	public struct RDLevelSettings
	{
		private const string versionKey = "version";

		private const string artistKey = "artist";

		private const string specialArtistTypeKey = "specialArtistType";

		private const string artistPermissionKey = "artistPermission";

		private const string artistLinksKey = "artistLinks";

		private const string songKey = "song";

		private const string authorKey = "author";

		private const string difficultyKey = "difficulty";

		private const string seizureWarningKey = "seizureWarning";

		private const string previewImageNameKey = "previewImage";

		private const string syringeIconNameKey = "syringeIcon";

		private const string previewSongNameKey = "previewSong";

		private const string previewSongStartTimeKey = "previewSongStartTime";

		private const string previewSongDurationKey = "previewSongDuration";

		private const string descriptionKey = "description";

		private const string tagsKey = "tags";

		private const string canBePlayedOnKey = "canBePlayedOn";

		private const string separate2PLevelFilenameKey = "separate2PLevelFilename";

		private const string rankMaxMistakesKey = "rankMaxMistakes";

		private const string rankDescriptionKey = "rankDescription";

		private const string customClassKey = "customClass";

		private const string multiplayerAppearanceKey = "multiplayerAppearance";

		private const string firstBeatBehaviorKey = "firstBeatBehavior";

		private const string modsKey = "mods";

		private const string songLabelHueKey = "songNameHue";

		private const string songLabelGrayscaleKey = "songLabelGrayscale";

		private const string inkFileKey = "inkFile";

		private const string levelVolumeKey = "levelVolume";

		private const string lastModifiedTimeKey = "lastModifiedTime";

		private const string firstSongKey = "firstSong";

		private const string bpmKey = "bpm";

		public const string firstSongOffsetKey = "firstSongOffset";

		private const string separate2PLevelLastModifiedTimeKey = "separate2PLevelLastModifiedTime";

		private const string mainRDLevelRelativePathKey = "mainRDLevelRelativePath";

		public int version;

		public string artist;

		public string song;

		public SpecialArtistType specialArtistType;

		public string artistPermissionFileName;

		public string artistLinks;

		public string author;

		public LevelDifficulty difficulty;

		public bool seizureWarning;

		public string previewImageName;

		public string syringeIconName;

		public string previewSongName;

		public float previewSongStartTime;

		public float previewSongDuration;

		public string description;

		public string tags;

		public float songLabelHue;

		public bool songLabelGrayscale;

		public LevelPlayMode canBePlayedOn;

		public string separate2PLevelFilename;

		public int[] rankMaxMistakes;

		public string[] rankDescription;

		public string customClass;

		public MultiplayerAppearance multiplayerAppearance;

		public FirstBeatBehavior firstBeatBehavior;

		public string[] mods;

		public string inkFile;

		public float levelVolume;

		public bool createRowsManually;

		public string lastModifiedTime;

		public string firstSongFileName;

		public float firstSongoffset;

		public float bpm;

		public string mainRDLevelRelativePath;

		public string levelName => artist + " - " + song;

		public float[] rankLowerBounds => new float[6]
		{
			float.MaxValue,
			rankMaxMistakes[0],
			rankMaxMistakes[1],
			rankMaxMistakes[2],
			rankMaxMistakes[3],
			0f
		};

		public RDLevelSettings(int version)
		{
			this.version = 68;
			author = string.Empty;
			artist = string.Empty;
			song = string.Empty;
			specialArtistType = SpecialArtistType.None;
			artistPermissionFileName = string.Empty;
			artistLinks = string.Empty;
			author = string.Empty;
			difficulty = LevelDifficulty.Medium;
			seizureWarning = false;
			inkFile = string.Empty;
			previewImageName = string.Empty;
			syringeIconName = string.Empty;
			previewSongName = string.Empty;
			previewSongStartTime = 0f;
			previewSongDuration = 10f;
			description = string.Empty;
			songLabelHue = UnityEngine.Random.Range(0f, 100f) / 100f;
			songLabelGrayscale = false;
			tags = string.Empty;
			separate2PLevelFilename = string.Empty;
			canBePlayedOn = LevelPlayMode.OnePlayerOnly;
			multiplayerAppearance = MultiplayerAppearance.HorizontalStrips;
			rankMaxMistakes = LevelValidation.DefaultRankMaxMistakes;
			string[] array = new string[6]
			{
				RDString.Get("editor.LevelSettings.defaultF"),
				RDString.Get("editor.LevelSettings.defaultD"),
				RDString.Get("editor.LevelSettings.defaultC"),
				RDString.Get("editor.LevelSettings.defaultB"),
				RDString.Get("editor.LevelSettings.defaultA"),
				RDString.Get("editor.LevelSettings.defaultS")
			};
			rankDescription = array;
			firstBeatBehavior = FirstBeatBehavior.RunEventsOnPrebar;
			levelVolume = 1f;
			customClass = null;
			mods = null;
			createRowsManually = false;
			lastModifiedTime = string.Empty;
			firstSongFileName = string.Empty;
			bpm = 0f;
			firstSongoffset = 0f;
			mainRDLevelRelativePath = string.Empty;
		}

		public void Decode(Dictionary<string, object> dict)
		{
			version = Convert.ToInt32(dict["version"]);
			artist = GetStringValue(dict, "artist", 256);
			song = GetStringValue(dict, "song", 256);
			if (dict.ContainsKey("specialArtistType"))
			{
				specialArtistType = RDEditorUtils.DecodeEnum<SpecialArtistType>(dict["specialArtistType"]);
			}
			artistPermissionFileName = GetStringValue(dict, "artistPermission");
			artistLinks = GetStringValue(dict, "artistLinks");
			description = GetStringValue(dict, "description", 1000);
			author = GetStringValue(dict, "author", 256);
			previewImageName = GetStringValue(dict, "previewImage");
			syringeIconName = GetStringValue(dict, "syringeIcon");
			previewSongName = GetStringValue(dict, "previewSong");
			tags = GetStringValue(dict, "tags");
			separate2PLevelFilename = GetStringValue(dict, "separate2PLevelFilename");
			if (dict.ContainsKey("canBePlayedOn"))
			{
				canBePlayedOn = RDEditorUtils.DecodeEnum<LevelPlayMode>(dict["canBePlayedOn"]);
			}
			previewSongStartTime = GetFloatValue(dict, "previewSongStartTime");
			previewSongDuration = GetFloatValue(dict, "previewSongDuration");
			if (dict.ContainsKey("songNameHue"))
			{
				songLabelHue = GetFloatValue(dict, "songNameHue");
			}
			if (dict.ContainsKey("songLabelGrayscale"))
			{
				songLabelGrayscale = RDEditorUtils.DecodeBool(dict["songLabelGrayscale"]);
			}
			if (dict.ContainsKey("levelVolume"))
			{
				levelVolume = GetFloatValue(dict, "levelVolume");
			}
			List<object> list = dict["rankMaxMistakes"] as List<object>;
			rankMaxMistakes = new int[4];
			for (int i = 0; i < rankMaxMistakes.Length; i++)
			{
				rankMaxMistakes[i] = Convert.ToInt32(list[i]);
			}
			List<object> list2 = dict["rankDescription"] as List<object>;
			rankDescription = new string[6];
			for (int j = 0; j < rankDescription.Length; j++)
			{
				rankDescription[j] = list2[j] as string;
			}
			if (dict.ContainsKey("customClass"))
			{
				customClass = dict["customClass"] as string;
			}
			if (dict.ContainsKey("multiplayerAppearance"))
			{
				multiplayerAppearance = RDEditorUtils.DecodeEnum<MultiplayerAppearance>(dict["multiplayerAppearance"]);
			}
			if (dict.ContainsKey("firstBeatBehavior"))
			{
				firstBeatBehavior = RDEditorUtils.DecodeEnum<FirstBeatBehavior>(dict["firstBeatBehavior"]);
			}
			if (dict.ContainsKey("difficulty"))
			{
				difficulty = RDEditorUtils.DecodeEnum<LevelDifficulty>(dict["difficulty"]);
			}
			if (dict.ContainsKey("seizureWarning"))
			{
				seizureWarning = RDEditorUtils.DecodeBool(dict["seizureWarning"]);
			}
			if (dict.ContainsKey("inkFile"))
			{
				inkFile = RDEditorUtils.DecodeString(dict["inkFile"]);
			}
			if (dict.ContainsKey("mods"))
			{
				object obj = dict["mods"];
				string[] array = ((obj is string text) ? (from mod in text.Split(',')
					select mod.Trim()).ToArray() : ((!(obj is List<object> source)) ? null : source.Cast<string>().ToArray()));
				mods = array;
			}
			else
			{
				mods = null;
			}
			if (dict.ContainsKey("lastModifiedTime"))
			{
				lastModifiedTime = GetStringValue(dict, "lastModifiedTime");
			}
			if (dict.ContainsKey("firstSong"))
			{
				firstSongFileName = GetStringValue(dict, "firstSong");
			}
			if (dict.ContainsKey("firstSongOffset"))
			{
				firstSongoffset = GetFloatValue(dict, "firstSongOffset");
			}
			if (dict.ContainsKey("bpm"))
			{
				bpm = GetFloatValue(dict, "bpm");
			}
			if (dict.ContainsKey("mainRDLevelRelativePath"))
			{
				mainRDLevelRelativePath = GetStringValue(dict, "mainRDLevelRelativePath");
			}
		}

		private string GetStringValue(Dictionary<string, object> dict, string key, int maxLength = 0)
		{
			string text = string.Empty;
			if (dict.ContainsKey(key))
			{
				text = dict[key] as string;
				if (maxLength > 0 && text.Length > maxLength)
				{
					text = text.Substring(0, maxLength);
				}
			}
			return text;
		}

		private float GetFloatValue(Dictionary<string, object> dict, string key)
		{
			if (!dict.TryGetValue(key, out var value))
			{
				return 0f;
			}
			return Convert.ToSingle(value);
		}

		private int GetIntValue(Dictionary<string, object> dict, string key)
		{
			if (!dict.TryGetValue(key, out var value))
			{
				return 0;
			}
			return Convert.ToInt32(value);
		}

		public string Encode(bool includeCacheData = false)
		{
			string newLine = Environment.NewLine;
			RDEditorUtils.IndentationLevel = 2;
			string text = RDEditorUtils.EncodeInt("version", includeCacheData ? version : 68) + newLine + RDEditorUtils.EncodeUnicodeString("artist", artist) + newLine + RDEditorUtils.EncodeUnicodeString("song", song) + newLine + RDEditorUtils.EncodeString("specialArtistType", specialArtistType.ToString()) + newLine + RDEditorUtils.EncodeUnicodeString("artistPermission", artistPermissionFileName) + newLine + RDEditorUtils.EncodeUnicodeString("artistLinks", artistLinks) + newLine + RDEditorUtils.EncodeUnicodeString("author", author) + newLine + RDEditorUtils.EncodeString("difficulty", difficulty.ToString()) + newLine + RDEditorUtils.EncodeBool("seizureWarning", seizureWarning) + newLine + RDEditorUtils.EncodeUnicodeString("previewImage", previewImageName) + newLine + RDEditorUtils.EncodeUnicodeString("syringeIcon", syringeIconName) + newLine + RDEditorUtils.EncodeUnicodeString("previewSong", previewSongName) + newLine + RDEditorUtils.EncodeFloat("previewSongStartTime", previewSongStartTime) + newLine + RDEditorUtils.EncodeFloat("previewSongDuration", previewSongDuration) + newLine + RDEditorUtils.EncodeFloat("songNameHue", songLabelHue) + newLine + RDEditorUtils.EncodeBool("songLabelGrayscale", songLabelGrayscale) + newLine + RDEditorUtils.EncodeUnicodeString("description", description) + newLine + RDEditorUtils.EncodeUnicodeString("tags", tags) + newLine + RDEditorUtils.EncodeUnicodeString("separate2PLevelFilename", separate2PLevelFilename) + newLine + RDEditorUtils.EncodeString("canBePlayedOn", canBePlayedOn.ToString()) + newLine + RDEditorUtils.EncodeString("firstBeatBehavior", firstBeatBehavior.ToString()) + newLine;
			if (includeCacheData)
			{
				text = text + RDEditorUtils.EncodeUnicodeString("mainRDLevelRelativePath", mainRDLevelRelativePath) + newLine + RDEditorUtils.EncodeString("lastModifiedTime", lastModifiedTime) + newLine + RDEditorUtils.EncodeString("firstSong", firstSongFileName) + newLine + RDEditorUtils.EncodeFloat("firstSongOffset", firstSongoffset) + newLine + RDEditorUtils.EncodeFloat("bpm", bpm) + newLine;
			}
			string text2 = "";
			if (!string.IsNullOrEmpty(customClass))
			{
				text2 = RDEditorUtils.EncodeString("customClass", customClass) + newLine;
			}
			string text3 = "";
			if (!string.IsNullOrEmpty(inkFile))
			{
				text3 = RDEditorUtils.EncodeString("inkFile", inkFile) + newLine;
			}
			string text4 = RDEditorUtils.EncodeString("multiplayerAppearance", multiplayerAppearance.ToString()) + newLine;
			string text5 = RDEditorUtils.EncodeFloat("levelVolume", levelVolume) + newLine;
			string text6 = RDEditorUtils.EncodeIntArray("rankMaxMistakes", rankMaxMistakes) + newLine;
			string text7 = ((mods != null && mods.Length != 0) ? RDEditorUtils.EncodeStringArrayForMods("mods", mods) : string.Empty) + newLine;
			string text8 = RDEditorUtils.OpenArray("rankDescription");
			RDEditorUtils.IndentationLevel = 3;
			for (int i = 0; i < rankDescription.Length; i++)
			{
				string text9 = ((i != rankDescription.Length - 1) ? "," : "");
				text8 = text8 + RDEditorUtils.Indentation + "\"" + RDEditorUtils.EscapeTextForJSON(rankDescription[i]) + "\"" + text9 + newLine;
			}
			RDEditorUtils.IndentationLevel = 2;
			text8 += RDEditorUtils.CloseArray(isLastMember: true);
			RDEditorUtils.IndentationLevel = 1;
			return text + text2 + text3 + text4 + text5 + text6 + text7 + text8;
		}
	}
}
