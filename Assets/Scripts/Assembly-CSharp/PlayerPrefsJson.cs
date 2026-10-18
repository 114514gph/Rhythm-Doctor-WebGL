using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GDMiniJSON;
using UnityEngine;

public class PlayerPrefsJson
{
	public enum FileLocation
	{
		Automatic = 0,
		UserFolder = 1,
		PersistentDataFolder = 2
	}

	public enum FileType
	{
		Settings = 0,
		Slot0 = 1,
		Slot1 = 2,
		Slot2 = 3
	}

	public const string FileExtension = ".rdsave";

	private const string ReleaseVersionKey = "version";

	private const string BackupFileExtension = ".old";

	public static readonly Dictionary<FileType, PlayerPrefsJson> files = new Dictionary<FileType, PlayerPrefsJson>();

	private static readonly Dictionary<FileType, string> filesPath = new Dictionary<FileType, string>();

	public static readonly FileType[] AllFileTypes = Enum.GetValues(typeof(FileType)) as FileType[];

	public static HashSet<string> nonSyncedKeys = new HashSet<string> { "windowdance_changed_resolution", "is_fullscreen", "window_x", "window_y", "resolution_width", "resolution_height" };

	public static readonly Encoding DefaultLevelEncoding = Encoding.UTF8;

	public static readonly Dictionary<FileType, string> Filenames = new Dictionary<FileType, string>
	{
		{
			FileType.Settings,
			"settings"
		},
		{
			FileType.Slot0,
			"slot0"
		},
		{
			FileType.Slot1,
			"slot1"
		},
		{
			FileType.Slot2,
			"slot2"
		}
	};

	public Dictionary<string, object> dict = new Dictionary<string, object>();

	private readonly FileType fileType;

	public static bool LoadFile(FileType fileType, bool loadBackup, out PlayerPrefsJson prefs)
	{
		prefs = null;
		string filePath = GetFilePath(fileType, loadBackup);
		printe($"loading fileType {fileType} from path: {filePath}");
		if (RDFile.Exists(filePath))
		{
			string json = RDFile.ReadAllText(filePath, DefaultLevelEncoding);
			if (!ValidateJson(json))
			{
				return false;
			}
			Dictionary<string, object> dictionary = null;
			try
			{
				dictionary = Json.Deserialize(json) as Dictionary<string, object>;
			}
			catch (Exception arg)
			{
				Debug.Log($"There was an error deserializing save file in path {filePath}.\nException: {arg}");
			}
			if (dictionary == null)
			{
				Debug.Log("Deserializing save in path " + filePath + " is null.");
				return false;
			}
			printe("JSON deserialized successfully.");
			prefs = new PlayerPrefsJson(fileType, dictionary);
			return true;
		}
		return false;
	}

	private static void ClearAllFiles()
	{
		filesPath.Clear();
		files.Clear();
	}

	public static bool LoadAllFiles()
	{
		ClearAllFiles();
		bool result = true;
		FileType[] allFileTypes = AllFileTypes;
		foreach (FileType fileType in allFileTypes)
		{
			PlayerPrefsJson prefs2;
			if (LoadFile(fileType, loadBackup: false, out var prefs))
			{
				AddFile(prefs);
				prefs.SaveBackup();
				printe($"Regular savefile {fileType} loaded.");
			}
			else if (LoadFile(fileType, loadBackup: true, out prefs2))
			{
				MarkCorruptFile(fileType, isBackup: false);
				printe($"Main save file {fileType} seems corrupted, attempting to load backup.");
				AddFile(prefs2);
			}
			else
			{
				MarkCorruptFile(fileType, isBackup: false);
				MarkCorruptFile(fileType, isBackup: true);
				result = false;
				AddFile(CreateFile(fileType));
			}
		}
		return result;
	}

	public void CopyFrom(PlayerPrefsJson other)
	{
		dict = new Dictionary<string, object>(other.dict);
	}

	private static bool ValidateJson(string json)
	{
		if (json.Length < 10)
		{
			Debug.LogWarning("PlayerPrefsJson: encoded JSON is way too short, seems corrupted: " + json);
			return false;
		}
		if (json[0] != '{' && json[1] != '{' && json[2] != '{')
		{
			Debug.LogWarning("PlayerPrefsJson: doesn't start with {");
			return false;
		}
		return true;
	}

	public static bool CreateFromBytes(byte[] bytes, out Dictionary<string, object> fileContent)
	{
		fileContent = null;
		if (bytes == null || bytes.Length == 0)
		{
			return false;
		}
		string json = DefaultLevelEncoding.GetString(bytes);
		if (!ValidateJson(json))
		{
			return false;
		}
		try
		{
			if (GDMiniJSON.Json.Deserialize(json) is Dictionary<string, object> dictionary)
			{
				fileContent = dictionary;
				return true;
			}
			Debug.Log("Deserialized object is not a dictionary.");
		}
		catch (Exception arg)
		{
			Debug.Log($"There was an error deserializing file from bytes.\nException: {arg}");
		}
		return false;
	}

	public static string GetFilePath(FileType fileType, bool loadBackup = false, bool corruptedFile = false, FileLocation fileFolderLocation = FileLocation.Automatic)
	{
		string text = Filenames[fileType];
		string text2;
		if (!corruptedFile && filesPath.TryGetValue(fileType, out var value))
		{
			text2 = value;
		}
		else
		{
			string fileFolderPath = GetFileFolderPath();
			char directorySeparatorChar = Path.DirectorySeparatorChar;
			text2 = fileFolderPath + directorySeparatorChar + (corruptedFile ? $"corrupted_{text}_{DateTime.Now:yyyy'-'MM'-'dd'_'HH'-'mm'-'ss}" : text) + ".rdsave";
			if (!corruptedFile)
			{
				filesPath.Add(fileType, text2);
			}
		}
		if (loadBackup)
		{
			text2 += ".old";
		}
		return text2;
	}

	public static string GetFileFolderPath()
	{
		string text;
		if (RDBase.appIsInSteamLibrary)
		{
			text = Application.dataPath;
			switch (RDBase.platform)
			{
			case Platform.Linux:
			case Platform.Windows:
				text = Directory.GetParent(text).FullName;
				break;
			case Platform.Mac:
				text = Directory.GetParent(text).Parent.FullName;
				break;
			}
			string text2 = text;
			char directorySeparatorChar = Path.DirectorySeparatorChar;
			text = text2 + directorySeparatorChar + "User";
			if (!RDDirectory.Exists(text))
			{
				RDDirectory.CreateDirectory(text);
			}
		}
		else
		{
			text = Persistence.DataPath;
		}
		return text;
	}

	public static bool FileExists(FileType fileType, FileLocation location = FileLocation.Automatic)
	{
		return RDFile.Exists(GetFilePath(fileType, loadBackup: false, corruptedFile: false, location));
	}

	public static void AddFile(PlayerPrefsJson data)
	{
		if (!files.ContainsKey(data.fileType))
		{
			files.Add(data.fileType, data);
		}
	}

	public static PlayerPrefsJson CreateFile(FileType fileType)
	{
		PlayerPrefsJson playerPrefsJson = new PlayerPrefsJson(fileType);
		AddFile(playerPrefsJson);
		return playerPrefsJson;
	}

	public static void SaveAllFiles()
	{
		foreach (PlayerPrefsJson value in files.Values)
		{
			value.Save();
		}
	}

	public static void MarkCorruptFile(FileType fileType, bool isBackup)
	{
		string filePath = GetFilePath(fileType, isBackup);
		if (RDFile.Exists(filePath))
		{
			RDFile.Copy(filePath, GetFilePath(fileType, isBackup, corruptedFile: true));
			RDFile.Delete(filePath);
		}
	}

	public static PlayerPrefsJson Get(FileType fileType)
	{
		if (files.TryGetValue(fileType, out var value))
		{
			return value;
		}
		if (LoadFile(fileType, loadBackup: false, out var prefs))
		{
			return prefs;
		}
		return CreateFile(fileType);
	}

	public PlayerPrefsJson(FileType fileType, Dictionary<string, object> data = null)
	{
		this.fileType = fileType;
		dict = data ?? dict;
	}

	public void Save()
	{
		if (dict == null)
		{
			Debug.LogWarning("PlayerPrefsJson: 'dict' is null");
			return;
		}
		if (Application.isEditor && dict.Keys.Count == 0)
		{
			Debug.LogWarning("PlayerPrefsJson: 'dict' is empty");
			return;
		}
		Set("version", Releases.releaseNumber);
		string text = Json.Serialize(dict);
		if (text == null)
		{
			Debug.LogWarning("PlayerPrefsJson: encoded JSON is null");
		}
		else if (ValidateJson(text))
		{
			RDFile.WriteAllText(GetFilePath(fileType), text, DefaultLevelEncoding);
		}
	}

	public void SaveBackup()
	{
		RDFile.Copy(GetFilePath(fileType), GetFilePath(fileType, loadBackup: true), overwrite: true);
	}

	public string GetString(string key, string defaultValue = "")
	{
		return GetGeneric(key, defaultValue);
	}

	public bool GetBool(string key, bool defaultValue = false)
	{
		return GetGeneric(key, defaultValue);
	}

	public int GetInt(string key, int defaultValue = 0)
	{
		return GetGeneric(key, defaultValue);
	}

	public int GetReleaseVersion()
	{
		return GetInt("version");
	}

	public float GetFloat(string key, float defaultValue = 0f)
	{
		if (!dict.TryGetValue(key, out var value))
		{
			return defaultValue;
		}
		return Convert.ToSingle(value);
	}

	public Dictionary<string, object> GetDict(string key)
	{
		if (!dict.TryGetValue(key, out var value))
		{
			return new Dictionary<string, object>();
		}
		return value as Dictionary<string, object>;
	}

	public List<object> GetList(string key)
	{
		if (!dict.TryGetValue(key, out var value))
		{
			return new List<object>();
		}
		return value as List<object>;
	}

	public T GetEnum<T>(string key, T defaultValue = default(T)) where T : Enum
	{
		if (!dict.TryGetValue(key, out var value) || !(value is string text) || string.IsNullOrEmpty(text))
		{
			return defaultValue;
		}
		return RDUtils.ParseEnum(text, defaultValue);
	}

	public string Get(string key, string defaultValue)
	{
		return GetString(key, defaultValue);
	}

	public bool Get(string key, bool defaultValue)
	{
		return GetBool(key, defaultValue);
	}

	public int Get(string key, int defaultValue)
	{
		return GetInt(key, defaultValue);
	}

	public float Get(string key, float defaultValue)
	{
		return GetFloat(key, defaultValue);
	}

	public T Get<T>(string key, T defaultValue) where T : Enum
	{
		return GetEnum(key, defaultValue);
	}

	private T GetGeneric<T>(string key, T defaultValue)
	{
		if (nonSyncedKeys.Contains(key))
		{
			object obj = null;
			if (defaultValue is float defaultValue2)
			{
				obj = PlayerPrefs.GetFloat(key, defaultValue2);
			}
			if (defaultValue is int defaultValue3)
			{
				obj = PlayerPrefs.GetInt(key, defaultValue3);
			}
			if (defaultValue is string defaultValue4)
			{
				obj = PlayerPrefs.GetString(key, defaultValue4);
			}
			if (defaultValue is bool flag)
			{
				obj = PlayerPrefs.GetInt(key, flag ? 1 : 0) == 1;
			}
			if (obj != null)
			{
				return (T)obj;
			}
			Debug.LogError($"nonSyncedKey {key} type {typeof(T)} not supported!");
		}
		if (dict.TryGetValue(key, out var value) && value is T)
		{
			return (T)value;
		}
		return defaultValue;
	}

	public void Set(string key, List<object> values)
	{
		SetGeneric(key, values);
	}

	public void Set(string key, Dictionary<string, object> value)
	{
		SetGeneric(key, value);
	}

	public void Set(string key, int value)
	{
		SetGeneric(key, value);
	}

	public void Set(string key, float value)
	{
		SetGeneric(key, value);
	}

	public void Set(string key, string value)
	{
		SetGeneric(key, value);
	}

	public void Set(string key, bool value)
	{
		SetGeneric(key, value);
	}

	public void Set<T>(string key, T value) where T : Enum
	{
		SetGeneric(key, value.ToString());
	}

	private void SetGeneric<T>(string key, T value)
	{
		if (nonSyncedKeys.Contains(key))
		{
			if (value is float value2)
			{
				PlayerPrefs.SetFloat(key, value2);
			}
			else if (value is int value3)
			{
				PlayerPrefs.SetInt(key, value3);
			}
			else if (value is string value4)
			{
				PlayerPrefs.SetString(key, value4);
			}
			else if (value is bool flag)
			{
				PlayerPrefs.SetInt(key, flag ? 1 : 0);
			}
		}
		else
		{
			dict[key] = value;
		}
	}

	public void Remove(string key)
	{
		if (dict.ContainsKey(key))
		{
			dict.Remove(key);
		}
	}

	public bool ContainsKey(string key)
	{
		return dict.ContainsKey(key);
	}

	public void Delete()
	{
		dict.Clear();
		string filePath = GetFilePath(fileType);
		if (RDFile.Exists(filePath))
		{
			RDFile.Delete(filePath);
		}
		string path = filePath + ".old";
		if (RDFile.Exists(path))
		{
			RDFile.Delete(path);
		}
	}

	public static void printe(object obj)
	{
	}
}
