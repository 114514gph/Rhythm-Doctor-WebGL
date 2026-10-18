using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RDLevelEditor;
using UnityEngine;

public static class LevelValidation
{
	private static string _tempLevelsFolder = null;

	private static string _customLevelPath = null;

	private static string _customCharacterPath = null;

	public const int MaxArtistLength = 256;

	public const int MaxSongLength = 256;

	public const int MaxDescriptionLength = 1000;

	public const int MaxTagLength = 254;

	public const int MaxTagTotalLength = 1024;

	public const int PreviewImageMaxFileSize = 2000000;

	public const int SyringeIconMaxFileSize = 20000;

	public const int MaxAuthorLength = 256;

	public const float Saturation = 60f;

	public const float Brightness = 95f;

	public const float MinSongLabelHue = 0f;

	public const float MaxSongLabelHue = 100f;

	public static readonly int[] DefaultRankMaxMistakes = new int[4] { 20, 15, 10, 5 };

	public const float MinPreviewSongDuration = 1f;

	public const float MaxPreviewSongDuration = 20f;

	public const float DefaultPreviewSongStartTime = 0f;

	public const float DefaultPreviewSongDuration = 10f;

	public const string ErrorTokenPrefix = "settingRequirement.";

	public const string MainFilename = "main.rdlevel";

	public const string BackupFilename = "backup.rdlevel";

	public const int MinFinishLevelEventNumber = 3;

	public static readonly string[] InvalidFileNameChars = new string[9] { "<", ">", ":", "\"", "/", "\\", "|", "?", "*" };

	public static readonly string[] InvalidFileNames = new string[24]
	{
		"CON", "PRN", "AUX", "NUL", "COM0", "COM1", "COM2", "COM3", "COM4", "COM5",
		"COM6", "COM7", "COM8", "COM9", "LPT0", "LPT1", "LPT2", "LPT3", "LPT4", "LPT5",
		"LPT6", "LPT7", "LPT8", "LPT9"
	};

	public static readonly Vector2Int PreviewImageRecommendedDimensions = new Vector2Int(120, 68);

	public static readonly Vector2Int SyringeIconRecommendedDimensions = new Vector2Int(19, 15);

	public static string TempLevelsFolder
	{
		get
		{
			if (string.IsNullOrEmpty(_tempLevelsFolder))
			{
				_tempLevelsFolder = Path.Combine(Persistence.DataPath, "Temp");
			}
			return _tempLevelsFolder;
		}
	}

	public static string CustomLevelsPath
	{
		get
		{
			if (string.IsNullOrEmpty(_customLevelPath))
			{
				_customLevelPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Rhythm Doctor", "Levels");
			}
			if (!Directory.Exists(_customLevelPath))
			{
				Directory.CreateDirectory(_customLevelPath);
			}
			return _customLevelPath.ToString();
		}
	}

	public static string CustomCharactersPath
	{
		get
		{
			if (string.IsNullOrEmpty(_customCharacterPath))
			{
				_customCharacterPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + Path.DirectorySeparatorChar + "Rhythm Doctor" + Path.DirectorySeparatorChar + "Characters";
			}
			if (!Directory.Exists(_customCharacterPath))
			{
				Directory.CreateDirectory(_customCharacterPath);
			}
			return _customCharacterPath.ToString();
		}
	}

	public static List<LevelErrorName> ForceSettingsValidation(string levelDirectory, ref RDLevelSettings settings)
	{
		List<LevelErrorName> levelSettingsErrors = GetLevelSettingsErrors(levelDirectory, settings);
		List<LevelErrorName> list = new List<LevelErrorName>(levelSettingsErrors);
		foreach (LevelErrorName item in levelSettingsErrors)
		{
			switch (item)
			{
			case LevelErrorName.ArtistLength:
				settings.artist = settings.artist.Substring(0, 256);
				list.Remove(item);
				break;
			case LevelErrorName.SongLength:
				settings.song = settings.song.Substring(0, 256);
				list.Remove(item);
				break;
			case LevelErrorName.AuthorLength:
				settings.author = settings.author.Substring(0, 256);
				list.Remove(item);
				break;
			case LevelErrorName.DescriptionLength:
				settings.description = settings.description.Substring(0, 1000);
				list.Remove(item);
				break;
			case LevelErrorName.PreviewImageWrongExtension:
			case LevelErrorName.PreviewImageFileNotFound:
			case LevelErrorName.PreviewImageFileSize:
				settings.previewImageName = string.Empty;
				list.Remove(item);
				break;
			case LevelErrorName.SyringeIconFileNotFound:
			case LevelErrorName.SyringeIconFileSize:
				settings.syringeIconName = string.Empty;
				list.Remove(item);
				break;
			case LevelErrorName.PreviewSongStartTimeNegative:
				settings.previewSongStartTime = 0f;
				list.Remove(item);
				break;
			case LevelErrorName.PreviewSongDurationOutRange:
				settings.previewSongDuration = Mathf.Clamp(settings.previewSongDuration, 1f, 20f);
				list.Remove(item);
				break;
			case LevelErrorName.TagsLength:
				if (settings.tags.Length > 1024)
				{
					settings.tags = settings.tags.Substring(0, 1024);
				}
				settings.tags = string.Join(", ", (from tag in settings.tags.Split(",", StringSplitOptions.RemoveEmptyEntries)
					select tag.Trim()).Select(TruncateTag));
				list.Remove(item);
				break;
			case LevelErrorName.CanBePlayedOnNull:
				settings.canBePlayedOn = LevelPlayMode.OnePlayerOnly;
				list.Remove(item);
				break;
			case LevelErrorName.SongLabelHueOutRange:
				settings.songLabelHue = Mathf.Clamp(settings.songLabelHue, 0f, 100f);
				list.Remove(item);
				break;
			case LevelErrorName.RankMaxMistakesNull:
				settings.rankMaxMistakes = DefaultRankMaxMistakes;
				list.Remove(item);
				break;
			}
		}
		return list;
		static string TruncateTag(string tag)
		{
			if (tag.Length <= 254)
			{
				return tag;
			}
			return tag.Substring(0, 254);
		}
	}

	public static bool CheckLevelForErrors(List<LevelErrorName> errorsToCheck, List<LevelErrorName> levelErrors)
	{
		foreach (LevelErrorName item in errorsToCheck)
		{
			foreach (LevelErrorName levelError in levelErrors)
			{
				if (item == levelError)
				{
					return false;
				}
			}
		}
		return true;
	}

	public static List<LevelErrorName> GetSharedErrors(List<LevelErrorName> errorsToCheck, List<LevelErrorName> levelErrors)
	{
		new List<LevelErrorName>();
		return levelErrors.Intersect(errorsToCheck).ToList();
	}

	public static List<LevelErrorName> GetLevelErrors(string levelDirectory, RDLevelData data, bool ignoreLevelSettings = false)
	{
		List<LevelErrorName> list = new List<LevelErrorName>();
		if (data == null)
		{
			list.Add(LevelErrorName.LevelDataNull);
			return list;
		}
		if (!ignoreLevelSettings)
		{
			list.AddRange(GetLevelSettingsErrors(levelDirectory, data.settings));
		}
		list.AddRange(GetLevelEventsErrors(data.levelEvents));
		list.RemoveAll((LevelErrorName error) => error == LevelErrorName.None);
		return list;
	}

	public static List<LevelErrorName> GetLevelEventsErrors(List<LevelEvent_Base> levelEvents)
	{
		new List<LevelErrorName>();
		return new List<LevelErrorName>
		{
			GetFinishLevelEventError(levelEvents),
			GetPlaySongExtensionError(levelEvents)
		};
	}

	public static List<LevelErrorName> GetLevelSettingsErrors(string levelDirectory, RDLevelSettings settings)
	{
		List<LevelErrorName> list = new List<LevelErrorName>();
		if (settings.Equals(default(RDLevelSettings)))
		{
			list.Add(LevelErrorName.LevelSettingsNull);
			return list;
		}
		list.Add(CheckString(settings.artist, 256, LevelErrorName.ArtistNull, LevelErrorName.ArtistLength));
		list.Add(CheckString(settings.song, 256, LevelErrorName.SongNull, LevelErrorName.SongLength));
		list.Add(CheckString(settings.author, 256, LevelErrorName.AuthorNull, LevelErrorName.AuthorLength));
		if (settings.tags.Length > 1024)
		{
			list.Add(LevelErrorName.TagsLength);
		}
		else
		{
			string[] array = settings.tags.Split(",");
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i].Trim();
				if (text.IsNullOrEmpty())
				{
					list.Add(LevelErrorName.TagsNull);
					break;
				}
				if (text.Length > 254)
				{
					list.Add(LevelErrorName.TagsLength);
					break;
				}
			}
		}
		if (string.IsNullOrEmpty(settings.description) && settings.description.Length > 1000)
		{
			list.Add(LevelErrorName.DescriptionLength);
		}
		list.Add(CheckFile(levelDirectory, settings.previewImageName, GC.SupportedImageFiles, LevelErrorName.PreviewImageNull, LevelErrorName.PreviewImageFileNotFound, LevelErrorName.PreviewImageWrongExtension, LevelErrorName.PreviewImageFileSize, 2000000));
		list.Add(GetSignatureImageError(levelDirectory, settings));
		list.Add(CheckFile(levelDirectory, settings.previewSongName, GC.SupportedAudioFiles, LevelErrorName.None, LevelErrorName.PreviewSongFileNotFound, LevelErrorName.PreviewSongWrongExtension));
		if (settings.canBePlayedOn == LevelPlayMode.None)
		{
			list.Add(LevelErrorName.CanBePlayedOnNull);
		}
		if (settings.canBePlayedOn == LevelPlayMode.BothModes && !string.IsNullOrEmpty(settings.separate2PLevelFilename))
		{
			list.Add(GetSeparate2PLevelErrors(levelDirectory, settings.separate2PLevelFilename));
		}
		if (settings.songLabelHue < 0f || settings.songLabelHue > 100f)
		{
			list.Add(LevelErrorName.SongLabelHueOutRange);
			RDBaseDllDummy.printem($"{LevelErrorName.SongLabelHueOutRange} - {settings.songLabelHue} is out of range. hue value: " + $"{settings.songLabelHue}, minimum acceptable: {0f}, maximum acceptable: {100f}");
		}
		if (settings.rankMaxMistakes == null)
		{
			list.Add(LevelErrorName.RankMaxMistakesNull);
		}
		if (settings.previewSongStartTime < 0f)
		{
			list.Add(LevelErrorName.PreviewSongStartTimeNegative);
		}
		else if (settings.previewSongDuration < 1f || settings.previewSongDuration > 20f)
		{
			list.Add(LevelErrorName.PreviewSongDurationOutRange);
		}
		list.RemoveAll((LevelErrorName error) => error == LevelErrorName.None);
		return list;
	}

	public static LevelErrorName GetSignatureImageError(string levelDirectory, RDLevelSettings settings)
	{
		return CheckFile(levelDirectory, settings.syringeIconName, GC.SupportedImageFiles, LevelErrorName.SyringeIconNull, LevelErrorName.SyringeIconFileNotFound, LevelErrorName.SyringeIconWrongExtension, LevelErrorName.SyringeIconFileSize, 20000);
	}

	public static LevelErrorName GetSeparate2PLevelErrors(string levelDirectory, string separate2PLevelFilename)
	{
		char directorySeparatorChar = Path.DirectorySeparatorChar;
		string text = levelDirectory + directorySeparatorChar + separate2PLevelFilename;
		bool flag = false;
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		try
		{
			dictionary = Json.Deserialize(RDFile.ReadAllText(text)) as Dictionary<string, object>;
		}
		catch (Exception ex)
		{
			RDBaseDllDummy.printem("Error desearializing json: " + text + "\nException: " + ex.ToString());
			flag = true;
		}
		if (flag || dictionary == null || dictionary.Count <= 1)
		{
			return LevelErrorName.Separate2PLevelDeserialization;
		}
		string[] extensions = new string[1] { "rdlevel" };
		return CheckFile(levelDirectory, separate2PLevelFilename, extensions, LevelErrorName.Separate2PLevelNull, LevelErrorName.Separate2PLevelNotFound, LevelErrorName.Separate2PLevelWrongExtension);
	}

	public static LevelErrorName GetFinishLevelEventError(List<LevelEvent_Base> levelEvents)
	{
		int num = 0;
		foreach (LevelEvent_Base levelEvent in levelEvents)
		{
			if (levelEvent is LevelEvent_FinishLevel)
			{
				num++;
			}
			else if (levelEvent is LevelEvent_GoToLevel levelEvent_GoToLevel && !levelEvent_GoToLevel.rdlevel.IsNullOrEmpty() && (levelEvent_GoToLevel.action != GoToLevelAction.SetNext || levelEvent_GoToLevel.skippable))
			{
				return LevelErrorName.None;
			}
		}
		if (num < 3)
		{
			return LevelErrorName.FinishLevelEventNumberNotEnough;
		}
		return LevelErrorName.None;
	}

	public static LevelErrorName GetPlaySongExtensionError(List<LevelEvent_Base> levelEvents)
	{
		foreach (LevelEvent_Base levelEvent in levelEvents)
		{
			if (levelEvent is LevelEvent_PlaySong levelEvent_PlaySong && levelEvent_PlaySong.RequiresConversion())
			{
				return LevelErrorName.PlaySongNotOGG;
			}
		}
		return LevelErrorName.None;
	}

	private static LevelErrorName CheckString(string value, int length, LevelErrorName valueNull, LevelErrorName valueLength)
	{
		LevelErrorName result = LevelErrorName.None;
		if (value.IsNullOrEmpty() || value.Trim().IsNullOrEmpty())
		{
			result = valueNull;
		}
		else if (value.Length > length)
		{
			result = valueLength;
		}
		return result;
	}

	private static LevelErrorName CheckFile(string levelDirectory, string filename, string[] extensions, LevelErrorName filenameNull, LevelErrorName fileNotFound, LevelErrorName fileWrongExtension, LevelErrorName fileSizeLimitExceeded = LevelErrorName.None, int maxFileSize = -1, LevelErrorName fileDimensions = LevelErrorName.None, Vector2Int dimensions = default(Vector2Int))
	{
		if (string.IsNullOrEmpty(filename))
		{
			return filenameNull;
		}
		string fileExtension = Path.GetExtension(filename);
		fileExtension = fileExtension.ToLowerInvariant();
		if (fileWrongExtension != LevelErrorName.None && string.IsNullOrEmpty(Array.Find(extensions, (string extension) => "." + extension == fileExtension)))
		{
			return fileWrongExtension;
		}
		string text = string.Empty;
		bool flag = false;
		if (fileWrongExtension != LevelErrorName.None)
		{
			char directorySeparatorChar = Path.DirectorySeparatorChar;
			text = levelDirectory + directorySeparatorChar + filename;
			flag = RDFile.Exists(text);
		}
		else
		{
			foreach (string text2 in extensions)
			{
				text = Path.Combine(levelDirectory, filename) + "." + text2;
				RDBaseDllDummy.printem(text);
				if (RDFile.Exists(text))
				{
					flag = true;
					break;
				}
			}
		}
		if (!flag)
		{
			return fileNotFound;
		}
		if (fileDimensions != LevelErrorName.None)
		{
			_ = dimensions != Vector2Int.zero;
		}
		if (fileSizeLimitExceeded != LevelErrorName.None && maxFileSize > 0 && new FileInfo(text).Length > maxFileSize)
		{
			return fileSizeLimitExceeded;
		}
		return LevelErrorName.None;
	}

	public static Vector2 GetImageFixedSize(Texture texture, Vector2Int recommendedDimensions)
	{
		float num = (float)recommendedDimensions.x / (float)texture.width;
		float num2 = (float)recommendedDimensions.y / (float)texture.height;
		float num3 = 0f;
		float num4 = 0f;
		if (num < num2)
		{
			num3 = recommendedDimensions.y;
			num4 = (float)texture.width * num2;
		}
		else
		{
			num4 = recommendedDimensions.x;
			num3 = (float)texture.height * num;
		}
		return new Vector2(num4, num3);
	}
}
