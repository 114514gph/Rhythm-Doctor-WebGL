using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Text.Unicode;
using System.Threading.Tasks;
using DG.Tweening;
using Hsluv;
using MultiWindow;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public static class RDUtils
{
	public static DateTime[] startingTimes = new DateTime[10];

	public static void printe(this object msg)
	{
	}

	public static AudioMixerGroup GetMixerGroup(string groupPath)
	{
		return Singleton<AudioManager>.Instance.GetMixerGroup(groupPath);
	}

	public static void SetMixerParameter(string exposedParameter, float value)
	{
		if (!Singleton<AudioManager>.Instance.Mixer.SetFloat(exposedParameter, value))
		{
			UnityEngine.Debug.LogWarning("Audio Parameter not found: $" + exposedParameter + "!");
		}
	}

	public static void SetMixerVolume(string exposedParameterVolume, float value)
	{
		float value2 = ((value > 1f) ? ((value - 1f) * 10f) : ((!(value > 0f)) ? (-80f) : ((value - 1f) * 20f)));
		SetMixerParameter(exposedParameterVolume, value2);
	}

	public static float GetMixerParameter(string exposedParameterName)
	{
		Singleton<AudioManager>.Instance.Mixer.GetFloat(exposedParameterName, out var value);
		return value;
	}

	public static float PitchPercentToSemitones(float percent)
	{
		return (float)Math.Log(percent, 2.0) * 12f;
	}

	public static float PitchSemitonesToPercent(float semitones)
	{
		return (float)Math.Pow(2.0, (double)semitones / 12.0);
	}

	public static string GetDefaultPulseSoundForCharacter(Character c, bool removePrefix = false)
	{
		string text = "sndShaker";
		if (GC.defaultCharacterPulseSounds.ContainsKey(c))
		{
			text = GC.defaultCharacterPulseSounds[c];
		}
		if (removePrefix)
		{
			text = text.Substring(3);
		}
		return text;
	}

	public static AudioClip GetSpeechTickerSoundForCharacter(Character? maybeCharacter, string expression = null)
	{
		string text = "Generic";
		if (maybeCharacter.HasValue)
		{
			Character valueOrDefault = maybeCharacter.GetValueOrDefault();
			if (GC.defaultCharacterSpeechSounds.ContainsKey(valueOrDefault))
			{
				text = GC.defaultCharacterSpeechSounds[valueOrDefault];
				if (expression != null)
				{
					string text2 = text + "_" + expression;
					AudioClip audioClip = Singleton<AudioManager>.Instance.FindOrLoadAudioClip("Sfx/SpeechTickers/sndSpeech" + text2);
					if (audioClip != null)
					{
						return audioClip;
					}
				}
			}
		}
		return Singleton<AudioManager>.Instance.FindOrLoadAudioClip("Sfx/SpeechTickers/sndSpeech" + text);
	}

	public static float GetOffsetForSoundType(GameSoundType gst, float pitch = 1f)
	{
		string filename = RDGameSounds.Get(gst).filename;
		Dictionary<string, SongOffset> externalSoundData = scrConductor.instance.externalSoundData;
		if (externalSoundData.ContainsKey(filename))
		{
			return externalSoundData[filename].offset / pitch;
		}
		return scrConductor.songOffsets.Get(filename).offset / pitch;
	}

	public static float OverridePanFor2P(RDPlayer player, float pan)
	{
		if (!GC.twoPlayerMode)
		{
			return pan;
		}
		float num = player switch
		{
			RDPlayer.P1 => GC.PanP1, 
			RDPlayer.P2 => GC.PanP2, 
			_ => 0f, 
		};
		if (pan != 0f)
		{
			num = Mathf.Abs(pan) * Mathf.Sign(num);
		}
		return num;
	}

	public static float OverridePanFor2P(Row row, float pan)
	{
		return OverridePanFor2P(row.GetPlayerWeAreGoingToChangeInto(), pan);
	}

	public static bool IsActuallyPlayingNotJustScheduled(this AudioSource sound)
	{
		if (sound.time > 0f)
		{
			return sound.isPlaying;
		}
		return false;
	}

	public static AudioType GetAudioType(string extension)
	{
		extension = extension.ToLower().Replace(".", "");
		bool num = extension == "ogg";
		bool flag = extension == "wav";
		bool flag2 = extension == "aif";
		bool flag3 = extension == "aiff";
		bool flag4 = extension == "mp3";
		AudioType result = AudioType.UNKNOWN;
		if (num | flag | flag2 | flag3)
		{
			result = AudioType.OGGVORBIS;
			if (flag)
			{
				result = AudioType.WAV;
			}
			if (flag3 | flag2)
			{
				result = AudioType.AIFF;
			}
		}
		else if (flag4)
		{
			result = AudioType.MPEG;
		}
		return result;
	}

	public static async Task<string> TryDeleteDirectory(string directoryPath, int maxRetries = 10, int millisecondsDelay = 30)
	{
		string errorMessage = string.Empty;
		int i = 0;
		while (i < maxRetries)
		{
			try
			{
				StartMeasuringTime();
				Directory.Delete(directoryPath, recursive: true);
				UnityEngine.Debug.Log($"SUCCESS Finished deleting: {directoryPath} ({StopMeasuringTime()}s)");
				return string.Empty;
			}
			catch (Exception ex)
			{
				errorMessage = ex.Message;
				UnityEngine.Debug.Log("Error try deleting: " + directoryPath + ", " + ex.Message);
				await Task.Delay(millisecondsDelay);
			}
			int num = i + 1;
			i = num;
		}
		UnityEngine.Debug.Log("FAILED Finished deleting: " + directoryPath + ", " + errorMessage);
		return errorMessage;
	}

	public static string ToMilliseconds(this float s)
	{
		return Mathf.RoundToInt(s * 1000f) + RDString.Get("editor.unit.ms");
	}

	public static string ToMilliseconds(this double s)
	{
		return Mathf.RoundToInt((float)s * 1000f) + RDString.Get("editor.unit.ms");
	}

	public static void RevealInExplorer(string path)
	{
		RDBaseDllDummy.printem(path);
		if (!string.IsNullOrEmpty(path))
		{
			RuntimePlatform platform = Application.platform;
			switch (platform)
			{
			case RuntimePlatform.WindowsPlayer:
			case RuntimePlatform.WindowsEditor:
				OpenInWinFileBrowser(path);
				break;
			case RuntimePlatform.OSXEditor:
			case RuntimePlatform.OSXPlayer:
				OpenInMacFileBrowser(path);
				break;
			case RuntimePlatform.LinuxPlayer:
			case RuntimePlatform.LinuxEditor:
				OpenInLinuxFileBrowser(path);
				break;
			default:
				UnityEngine.Debug.LogWarning("RevealInExplorer not implemented for " + platform.ToString() + " platform, path: " + path);
				break;
			}
		}
	}

	public static void OpenInLinuxFileBrowser(string path)
	{
		string text = path.Replace("\\", "/");
		ProcessStartInfo startInfo;
		if (Directory.Exists(text))
		{
			startInfo = new ProcessStartInfo
			{
				FileName = text,
				UseShellExecute = true
			};
		}
		else
		{
			text.TrimEnd('/');
			ProcessStartInfo processStartInfo = new ProcessStartInfo();
			processStartInfo.FileName = "dbus-send";
			processStartInfo.Arguments = string.Join(" ", "--session", "--print-reply", "--dest=org.freedesktop.FileManager1", "--type=method_call /org/freedesktop/FileManager1 org.freedesktop.FileManager1.ShowItems", "array:string:\"file://" + text + "\" string:\"\"");
			startInfo = processStartInfo;
		}
		Process process = Process.Start(startInfo);
		if (process == null)
		{
			UnityEngine.Debug.LogError("Revealing file '$" + path + "' returned null process");
			return;
		}
		process.WaitForExit();
		if (process.ExitCode != 0)
		{
			UnityEngine.Debug.LogError($"Revealing file '${path}' returned exit code {process.ExitCode}");
		}
	}

	public static void OpenInMacFileBrowser(string path)
	{
		bool flag = false;
		string text = path.Replace("\\", "/");
		if (Directory.Exists(text))
		{
			flag = true;
		}
		if (!text.StartsWith("\""))
		{
			text = "\"" + text;
		}
		if (!text.EndsWith("\""))
		{
			text += "\"";
		}
		string arguments = (flag ? "" : "-R ") + text;
		try
		{
			Process.Start("open", arguments);
		}
		catch (Win32Exception ex)
		{
			ex.HelpLink = "";
		}
	}

	public static void OpenInWinFileBrowser(string path)
	{
		bool flag = false;
		string text = path.Replace("/", "\\");
		if (Directory.Exists(text))
		{
			flag = true;
		}
		try
		{
			Process.Start("explorer.exe", (flag ? "/root," : "/select,") + text);
		}
		catch (Win32Exception ex)
		{
			ex.HelpLink = "";
		}
	}

	public static string SanitizeFileName(string filename, char defaultChar = '_')
	{
		string text = string.Empty;
		string text2;
		try
		{
			text = Path.GetExtension(filename);
			text2 = filename.Replace(text, string.Empty);
		}
		catch
		{
			RDBaseDllDummy.printem("Sanitizing file without extension: " + filename);
			text2 = filename;
		}
		byte[] bytes = Encoding.ASCII.GetBytes(text2);
		string text3 = new string(Encoding.ASCII.GetChars(bytes));
		text3 = text3.Replace("?", string.Empty);
		if (text2 == text3)
		{
			return filename;
		}
		if (!string.IsNullOrEmpty(text3))
		{
			RDBaseDllDummy.printem("Force sanitization from " + text2 + "  to  " + text3.ToString());
			text2 = text3;
		}
		StringBuilder stringBuilder = new StringBuilder(text2.Length);
		foreach (char c in text2)
		{
			int num = c;
			if ((num >= 65 && num <= 90) || (num >= 97 && num <= 122) || (num >= 48 && num <= 57) || char.IsWhiteSpace(c) || c == '-' || c == '(' || c == ')')
			{
				stringBuilder.Append(c);
			}
			else
			{
				stringBuilder.Append(defaultChar);
			}
		}
		RDBaseDllDummy.printem("Sanitized manually from " + text2 + "  to  " + stringBuilder.ToString());
		return stringBuilder.ToString() + text;
	}

	public static string GetRelativePath(string filePath, string directoryPath)
	{
		return filePath.Replace(directoryPath, string.Empty).Substring(1);
	}

	public static bool GetFileBuffer(string filePath, int offset, int length, out byte[] buffer)
	{
		buffer = null;
		try
		{
			using BinaryReader binaryReader = new BinaryReader(new FileStream(filePath, FileMode.Open));
			binaryReader.BaseStream.Seek(offset, SeekOrigin.Begin);
			buffer = new byte[length];
			binaryReader.Read(buffer, 0, length);
		}
		catch (Exception ex)
		{
			ex.Message.printe();
		}
		return buffer != null;
	}

	public static void CopyDirectory(string sourceDirectory, string targetDirectory)
	{
		DirectoryInfo source = new DirectoryInfo(sourceDirectory);
		DirectoryInfo target = new DirectoryInfo(targetDirectory);
		CopyAll(source, target);
	}

	private static void CopyAll(DirectoryInfo source, DirectoryInfo target)
	{
		Directory.CreateDirectory(target.FullName);
		FileInfo[] files = source.GetFiles();
		foreach (FileInfo fileInfo in files)
		{
			Console.WriteLine("Copying {0}\\{1}", target.FullName, fileInfo.Name);
			fileInfo.CopyTo(Path.Combine(target.FullName, fileInfo.Name), overwrite: true);
		}
		DirectoryInfo[] directories = source.GetDirectories();
		foreach (DirectoryInfo directoryInfo in directories)
		{
			DirectoryInfo target2 = target.CreateSubdirectory(directoryInfo.Name);
			CopyAll(directoryInfo, target2);
		}
	}

	public static string GetAvailableFilename(string pathSansExtension, string extensionWithPeriod)
	{
		int num = -1;
		string text2;
		do
		{
			string text = ((num >= 0) ? num.ToString() : "");
			text2 = pathSansExtension + text + extensionWithPeriod;
			num++;
		}
		while (RDFile.Exists(text2));
		return text2;
	}

	public static string GetAvailableDirectoryName(string directoryPath)
	{
		int num = -1;
		string text = "";
		do
		{
			string text2 = ((num >= 0) ? num.ToString() : "");
			text = directoryPath + text2;
			num++;
		}
		while (Directory.Exists(text) || RDFile.Exists(text));
		return text;
	}

	public static bool HasAudioFileExtension(this string filename)
	{
		int num = filename.LastIndexOf('.');
		if (num == -1)
		{
			return false;
		}
		string item = filename.Substring(num + 1).ToLower();
		return GC.SupportedAudioFilesSet.Contains(item);
	}

	public static bool HasImageFileExtension(this string filename)
	{
		int num = filename.LastIndexOf('.');
		if (num == -1)
		{
			return false;
		}
		string item = filename.Substring(num + 1).ToLower();
		return GC.SupportedImageFilesSet.Contains(item);
	}

	public static bool HasFontFileExtension(this string filename)
	{
		int num = filename.LastIndexOf('.');
		if (num == -1)
		{
			return false;
		}
		string item = filename.Substring(num + 1).ToLower();
		return GC.SupportedFontFilesSet.Contains(item);
	}

	public static string[] GetFilesWithExtension(this string directoryPath, string extension, SearchOption searchOption)
	{
		return Array.FindAll(Directory.GetFiles(directoryPath, "*", searchOption), (string filePath) => Path.GetExtension(filePath).ToLowerInvariant() == "." + extension);
	}

	public static string GetRDLevelFilename(string level)
	{
		if (level.IsNullOrEmpty())
		{
			return null;
		}
		string text = level.Trim();
		if (!text.EndsWith(".rdlevel", StringComparison.OrdinalIgnoreCase))
		{
			text += ".rdlevel";
		}
		return text;
	}

	public static void SetDirectoryAttributes(string directoryPath)
	{
		DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
		if (directoryInfo.Exists)
		{
			SetAttributesNormal(directoryInfo);
		}
	}

	private static void SetAttributesNormal(DirectoryInfo dir)
	{
		DirectoryInfo[] directories = dir.GetDirectories();
		foreach (DirectoryInfo obj in directories)
		{
			SetAttributesNormal(obj);
			obj.Attributes = FileAttributes.Normal;
		}
		FileInfo[] files = dir.GetFiles();
		for (int i = 0; i < files.Length; i++)
		{
			files[i].Attributes = FileAttributes.Normal;
		}
	}

	public static Vector2Int GetImageDimensions(string path)
	{
		Vector2Int vector2Int = default;
		using Bitmap bitmap = new Bitmap(path);
		return new Vector2Int(bitmap.Width, bitmap.Height);
	}

	public static bool NegateIf(this bool toNegate, bool negated)
	{
		return toNegate ^ negated;
	}

	public static T ParseEnum<T>(string str, T defaultValue = default(T))
	{
		if (Enum.TryParse(typeof(T), str, ignoreCase: true, out var result))
		{
			return (T)result;
		}
		return defaultValue;
	}

	public static bool Contains(this UnicodeRange range, char input)
	{
		if (range.FirstCodePoint <= input)
		{
			return input < range.FirstCodePoint + range.Length;
		}
		return false;
	}

	public static T ParseEnum<T>(string str, out bool valueFound)
	{
		valueFound = Enum.TryParse(typeof(T), str, ignoreCase: true, out var result);
		if (!valueFound)
		{
			return default;
		}
		return (T)result;
	}

	public static T[] GetValues<T>() where T : Enum
	{
		return (T[])Enum.GetValues(typeof(T));
	}

	public static double StandardDeviation(IEnumerable<float> enumerable)
	{
		List<float> list = enumerable.ToList();
		float average = list.Average();
		return Mathf.Sqrt(list.Sum((float val) => Mathf.Pow(val - average, 2f)) / (float)list.Count);
	}

	public static GameObject SpawnIfNotFound(string GOname, GameObject prefab = null)
	{
		GameObject gameObject = null;
		GameObject gameObject2 = GameObject.Find(GOname);
		if (gameObject2 == null)
		{
			if (prefab != null)
			{
				gameObject = UnityEngine.Object.Instantiate(prefab);
				gameObject.name = GOname;
			}
			else
			{
				gameObject = new GameObject(GOname);
			}
			gameObject.transform.SetParent(GameObject.Find("ParentContainer").transform);
			return gameObject;
		}
		return gameObject2;
	}

	public static float OutQuad(float t, float d)
	{
		return 1f - Mathf.Pow(1f - t / d, 2f);
	}

	public static void ShowDebugLabel(string s)
	{
		GUI.Label(new Rect(0f, 0f, 400f, 400f), s);
	}

	public static int Clamp(int value, int min, int max)
	{
		return Math.Min(Math.Max(value, min), max);
	}

	public static float Clamp(float value, float min, float max)
	{
		return Math.Min(Math.Max(value, min), max);
	}

	public static int Clamp01(int value)
	{
		return Clamp(value, 0, 1);
	}

	public static float Clamp01(float value)
	{
		return Clamp(value, 0f, 1f);
	}

	public static Vector2 AngleToVec(float angle)
	{
		return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
	}

	public static Vector2 LerpUnclampedVector(Vector2 a, Vector2 b, float t)
	{
		return new Vector2(Mathf.LerpUnclamped(a.x, b.x, t), Mathf.LerpUnclamped(a.y, b.y, t));
	}

	public static Vector2 RepeatVector(Vector2 vector, Vector2 bounds, bool forceNoXWrap = false, bool forceNoYWrap = false)
	{
		float x = (forceNoXWrap ? vector.x : Mathf.Repeat(vector.x, bounds.x));
		float y = (forceNoYWrap ? vector.y : Mathf.Repeat(vector.y, bounds.y));
		return new Vector2(x, y);
	}

	public static UnityEngine.Color CycleColor(UnityEngine.Color currentcolour)
	{
		uint[] array = new uint[11]
		{
			4291189578u, 4291198794u, 4283090599u, 4283081414u, 4283070918u, 4283058886u, 4285745862u, 4288039622u, 4290136774u, 4291185316u,
			4291185279u
		};
		UnityEngine.Color color = currentcolour;
		while (currentcolour == color)
		{
			int num = Mathf.FloorToInt(UnityEngine.Random.value * (float)array.Length);
			color = array[num].ARGBToColor();
		}
		return color;
	}

	public static void Rotate2D(Transform t, float rotation, bool relative = false)
	{
		Vector3 localEulerAngles = t.localEulerAngles;
		localEulerAngles.z = (relative ? (localEulerAngles.z + rotation) : rotation);
		t.localEulerAngles = localEulerAngles;
	}

	public static double CeilToNearestMultiple(double n, double multiple)
	{
		double num = Math.Round(n / multiple) * multiple;
		if (WithinSmallMargin(num, n))
		{
			return num;
		}
		return Math.Ceiling(n / multiple) * multiple;
	}

	public static bool WithinSmallMargin(float a, float b)
	{
		return (double)Mathf.Abs(a - b) < 0.001;
	}

	public static bool WithinSmallMargin(double a, double b)
	{
		return Math.Abs(a - b) < 0.001;
	}

	public static T Create<T>(string name, bool makeContainer = false, int suffixNumber = -1, string customContainer = null) where T : UnityEngine.Component
	{
		GameObject gameObject = new GameObject();
		GameObject gameObject2 = gameObject.Instantiate(name, makeContainer, customContainer, suffixNumber);
		UnityEngine.Object.Destroy(gameObject);
		return gameObject2.AddComponent<T>();
	}

	public static Vector2 RDFullNativeResolution(this Camera camera)
	{
		return new Vector2(scrVfxControl.instance.RDWidth, scrVfxControl.instance.RDHeight);
	}

	public static bool IsOBSActive()
	{
		switch (Application.platform)
		{
		case RuntimePlatform.WindowsPlayer:
		case RuntimePlatform.WindowsEditor:
			return CheckForProcess("obs64") || CheckForProcess("obs32") || CheckForProcess("Streamlabs OBS");
		case RuntimePlatform.OSXEditor:
		case RuntimePlatform.OSXPlayer:
			return CheckForProcess("OBS");
		case RuntimePlatform.LinuxPlayer:
		case RuntimePlatform.LinuxEditor:
			return false;
		default:
			return false;
		}
		static bool CheckForProcess(string processName)
		{
			return Process.GetProcessesByName(processName).Length != 0;
		}
	}

	public static T GetOrAddComponent<T>(this GameObject gameObject) where T : UnityEngine.Component
	{
		T component = gameObject.GetComponent<T>();
		if (!(component == null))
		{
			return component;
		}
		return gameObject.AddComponent<T>();
	}

	public static T GetOrAddComponent<T>(this Behaviour gameObject) where T : UnityEngine.Component
	{
		T component = gameObject.GetComponent<T>();
		if (!(component == null))
		{
			return component;
		}
		return gameObject.gameObject.AddComponent<T>();
	}

	public static void DisableComponent<T>(this GameObject gameObject) where T : Behaviour
	{
		if (gameObject.TryGetComponent<T>(out var component))
		{
			component.enabled = false;
		}
	}

	public static void DisableComponent<T>(this Behaviour behaviour) where T : Behaviour
	{
		T component = behaviour.GetComponent<T>();
		if (component != null)
		{
			component.enabled = false;
		}
	}

	public static GameObject Instantiate(this GameObject gameObject, string name, bool makeContainer, string customContainer = null, int room = -1)
	{
		GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject);
		gameObject2.name = name;
		if (makeContainer)
		{
			string text = ((customContainer != null) ? customContainer : (name + " Container"));
			if (room != -1)
			{
				text = text + " " + room;
			}
			GameObject gameObject3 = SpawnIfNotFound(text);
			gameObject2.transform.SetParent(gameObject3.transform, worldPositionStays: false);
		}
		return gameObject2;
	}

	public static GameObject Instantiate(this GameObject gameObject, Transform parent, string name)
	{
		GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject);
		gameObject2.name = name;
		gameObject2.transform.parent = parent;
		return gameObject2;
	}

	public static void MoveX(this Transform t, float x)
	{
		t.position = new Vector3(x, t.position.y, t.position.z);
	}

	public static void MoveY(this Transform t, float y)
	{
		t.position = new Vector3(t.position.x, y, t.position.z);
	}

	public static void MoveZ(this Transform t, float z)
	{
		t.position = new Vector3(t.position.x, t.position.y, z);
	}

	public static void MoveXY(this Transform t, float x, float y)
	{
		t.position = new Vector3(x, y, t.position.z);
	}

	public static void MoveXY(this Transform t, Vector2 v)
	{
		t.position = new Vector3(v.x, v.y, t.position.z);
	}

	public static void MoveXY(this Transform t, Vector3 v)
	{
		t.position = new Vector3(v.x, v.y, t.position.z);
	}

	public static void LocalMoveX(this Transform t, float x)
	{
		t.localPosition = new Vector3(x, t.localPosition.y, t.localPosition.z);
	}

	public static void LocalMoveY(this Transform t, float y)
	{
		t.localPosition = new Vector3(t.localPosition.x, y, t.localPosition.z);
	}

	public static void LocalMoveZ(this Transform t, float z)
	{
		t.localPosition = new Vector3(t.localPosition.x, t.localPosition.y, z);
	}

	public static void LocalMoveXY(this Transform t, float x, float y)
	{
		t.localPosition = new Vector3(x, y, t.localPosition.z);
	}

	public static void TranslateX(this Transform t, float x)
	{
		t.localPosition = new Vector3(t.localPosition.x + x, t.localPosition.y, t.localPosition.z);
	}

	public static void TranslateY(this Transform t, float y)
	{
		t.localPosition = new Vector3(t.localPosition.x, t.localPosition.y + y, t.localPosition.z);
	}

	public static void TranslateZ(this Transform t, float z)
	{
		t.localPosition = new Vector3(t.localPosition.x, t.localPosition.y, t.localPosition.z + z);
	}

	public static void ScaleX(this Transform t, float x)
	{
		t.localScale = new Vector3(x, t.localScale.y, t.localScale.z);
	}

	public static void ScaleY(this Transform t, float y)
	{
		t.localScale = new Vector3(t.localScale.x, y, t.localScale.z);
	}

	public static void ScaleXY(this Transform t, float x, float y)
	{
		t.localScale = new Vector3(x, y, t.localScale.z);
	}

	public static void ScaleZ(this Transform t, float z)
	{
		t.localScale = new Vector3(t.localScale.x, t.localScale.y, z);
	}

	public static void AnchorPosX(this RectTransform t, float x)
	{
		t.anchoredPosition = new Vector3(x, t.anchoredPosition.y);
	}

	public static void AnchorPosY(this RectTransform t, float y)
	{
		t.anchoredPosition = new Vector3(t.anchoredPosition.x, y);
	}

	public static void SizeDeltaX(this RectTransform t, float x)
	{
		t.sizeDelta = new Vector3(x, t.sizeDelta.y);
	}

	public static void SizeDeltaY(this RectTransform t, float y)
	{
		t.sizeDelta = new Vector3(t.sizeDelta.x, y);
	}

	public static void SetRDString(this Text uiText, string key, float scale = 0f)
	{
		uiText.text = RDString.Get(key);
		RDStringToUIText.Apply(uiText, changeFontForLanguage: true, isStatusText: false, scale);
	}

	public static void SetRDString(this TextMesh textMesh, string key, float scale = 0f)
	{
		textMesh.text = RDString.Get(key);
		RDStringToUIText.Apply(textMesh, changeFontForLanguage: true, scale);
	}

	public static UnityEngine.Color ARGBToColor(this uint color)
	{
		byte b = (byte)(color >> 24);
		byte num = (byte)(color >> 16);
		byte b2 = (byte)(color >> 8);
		byte b3 = (byte)color;
		return new UnityEngine.Color((float)(int)num / 255f, (float)(int)b2 / 255f, (float)(int)b3 / 255f, (float)(int)b / 255f);
	}

	public static UnityEngine.Color RGBAToColor(this uint color)
	{
		byte num = (byte)(color >> 24);
		byte b = (byte)(color >> 16);
		byte b2 = (byte)(color >> 8);
		byte b3 = (byte)color;
		return new UnityEngine.Color((float)(int)num / 255f, (float)(int)b / 255f, (float)(int)b2 / 255f, (float)(int)b3 / 255f);
	}

	public static UnityEngine.Color GrayColor(this float f)
	{
		return new UnityEngine.Color(f, f, f, 1f);
	}

	public static bool IsInAccessibilityScope()
	{
		bool result = false;
		if (scnGame.instance == null)
		{
			return true;
		}
		switch (Persistence.GetAccessibilityScope())
		{
		case AccessibilityScope.StoryLevelsOnly:
			if (scnGame.levelToLoadSource != LevelSource.ExternalPath)
			{
				result = true;
			}
			break;
		case AccessibilityScope.AllLevels:
			result = true;
			break;
		}
		return result;
	}

	public static UnityEngine.Color WithBrightness(this UnityEngine.Color thisColor, float brightness, bool fadeAlpha = true)
	{
		if (!IsInAccessibilityScope())
		{
			return thisColor;
		}
		brightness = Clamp01(brightness);
		float num = Mathf.Lerp(0.1f, 1f, brightness);
		UnityEngine.Color result = new UnityEngine.Color(thisColor.r * num, thisColor.g * num, thisColor.b * num, thisColor.a);
		if (fadeAlpha)
		{
			float num2 = thisColor.r * thisColor.r * 0.299f + thisColor.g * thisColor.g * 0.587f + thisColor.b * thisColor.b * 0.114f;
			if (num2 > 0f)
			{
				float t = DOVirtual.EasedValue(0f, 1f, num2, Ease.InOutQuad);
				float value = Mathf.Lerp(1f, brightness, t);
				value = Clamp(value, 0.25f, 1f);
				result.a = Clamp01(result.a * value);
			}
		}
		return result;
	}

	public static void SetNormalColor(this Button button, UnityEngine.Color color)
	{
		ColorBlock colors = button.colors;
		colors.normalColor = color;
		button.colors = colors;
	}

	public static void Shake(this Behaviour camera, int jitterCount, int jitterAmount)
	{
		camera.gameObject.GetComponent<scrThingShake>().activateThingJitter(jitterCount, jitterAmount);
	}

	public static void ShakeWithDuration(this Behaviour camera, float duration, int jitterAmount)
	{
		camera.gameObject.GetComponent<scrThingShake>().activateThingJitter(duration, jitterAmount, jitterAmount);
	}

	public static void StopShake(this Behaviour camera)
	{
		camera.gameObject.GetComponent<scrThingShake>().StopShake();
	}

	public static Vector3 CameraCenterWithZ(this Camera camera, float z)
	{
		Vector2 vector = camera.RDFullNativeResolution();
		return new Vector3(vector.x / 2f, vector.y / 2f, z);
	}

	public static int IncrementWrap(this int index, int count)
	{
		return (index + 1) % count;
	}

	public static int DecrementWrap(this int index, int count)
	{
		if (index != 0)
		{
			return index - 1;
		}
		return count - 1;
	}

	public static T GetRandomItem<T>(this T[] array)
	{
		int num = UnityEngine.Random.Range(0, array.Length);
		return array[num];
	}

	public static string ToArrayString<T>(this T[] array)
	{
		if (array == null)
		{
			return string.Empty;
		}
		if (array.Length == 0)
		{
			return "{}";
		}
		string text = "{ ";
		for (int i = 0; i < array.Length; i++)
		{
			T value = array[i];
			text += value.ToString();
			text += ((Array.IndexOf(array, value) < array.Length - 1) ? ", " : " }");
		}
		return text;
	}

	public static Tweener DOScaleXY(this tk2dBaseSprite target, float endValue, float duration)
	{
		return target.DOScale(endValue, duration);
	}

	public static bool IsPressed(this KeyCode keyCode)
	{
		return Input.GetKey(keyCode);
	}

	public static bool WentDown(this KeyCode keyCode)
	{
		return Input.GetKeyDown(keyCode);
	}

	public static bool WentUp(this KeyCode keyCode)
	{
		return Input.GetKeyUp(keyCode);
	}

	public static string GetHash(params string[] inputs)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (string value in inputs)
		{
			stringBuilder.Append(value);
		}
		byte[] array = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(stringBuilder.ToString()));
		stringBuilder = new StringBuilder();
		for (int j = 0; j < array.Length; j++)
		{
			stringBuilder.Append(array[j].ToString("x2"));
		}
		return stringBuilder.ToString();
	}

	public static string RemoveNewLines(this string value)
	{
		return value.Replace("\n", string.Empty);
	}

	public static string Truncate(this string value, int maxLength)
	{
		if (string.IsNullOrEmpty(value))
		{
			return value;
		}
		if (value.Length > maxLength)
		{
			return value.Substring(0, maxLength);
		}
		return value;
	}

	public static string TruncateAnyEncoding(this string input, int maxBytes, Action onTruncate = null)
	{
		if (Encoding.UTF8.GetByteCount(input) <= maxBytes)
		{
			return input;
		}
		onTruncate?.Invoke();
		int byteCount = Encoding.UTF8.GetByteCount("...");
		int i;
		for (i = 0; Encoding.UTF8.GetByteCount(input.Substring(0, i + 1)) <= maxBytes - byteCount; i++)
		{
		}
		return input.Substring(0, i) + "...";
	}

	public static string TrimAllSpaces(this string value)
	{
		return value.Replace(" ", string.Empty);
	}

	public static string ExcludeSquareBracketed(this string str)
	{
		return Regex.Replace(str, "\\[(?>[^]]+)\\]", string.Empty);
	}

	public static string RemoveRichTextColorTags(this string input)
	{
		return Regex.Replace(input, "<color.*?>|</color>", string.Empty, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(16.0));
	}

	public static bool Has(this string s, string search, out int index)
	{
		index = s.IndexOf(search);
		return index != -1;
	}

	public static bool IsNullOrEmpty(this string s)
	{
		return string.IsNullOrEmpty(s);
	}

	public static IEnumerable<int> AllIndexesOf(this string str, string searchstring)
	{
		for (int minIndex = str.IndexOf(searchstring); minIndex != -1; minIndex = str.IndexOf(searchstring, minIndex + searchstring.Length))
		{
			yield return minIndex;
		}
	}

	public static IEnumerable<int> AllIndexesOf(this string str, char searchchar)
	{
		for (int minIndex = str.IndexOf(searchchar); minIndex != -1; minIndex = str.IndexOf(searchchar, minIndex))
		{
			yield return minIndex;
		}
	}

	public static bool IsNumeric(this string text)
	{
		double result;
		return double.TryParse(text, out result);
	}

	public static void StartMeasuringTime(int id = 0)
	{
		startingTimes[id] = DateTime.UtcNow;
	}

	public static double StopMeasuringTime(int id = 0)
	{
		return (DateTime.UtcNow - startingTimes[id]).TotalSeconds;
	}

	public static void AppendMany(this StringBuilder stringBuilder, params string[] strings)
	{
		for (int i = 0; i < strings.Length; i++)
		{
			stringBuilder.Append(strings[i]);
		}
	}

	public static void ShowTutorialTheme(UnityEngine.Color notesColor, UnityEngine.Color backgroundColor)
	{
		Background background = Background.Create("many_notes_tile", -0.1f, -0.1f);
		background.shaderDataSource = background;
		background.SetColor(notesColor);
		RDBase.Vfx.BgColor(backgroundColor);
		scrVfxControl.instance.SetVignette(enabled: true);
		scrVfxControl.instance.SetVignetteAlpha(0.5f);
	}

	public static Tweener FlashPartial(this SpriteRenderer sprite, float alpha)
	{
		sprite.DOKill();
		sprite.color = sprite.color.WithAlpha(alpha);
		return sprite.DOFade(0f, 0.5f);
	}

	public static float GetTextHeight(Text text, UnityEngine.Font defaultPixelatedFont)
	{
		float num = 0f;
		if (text.font == defaultPixelatedFont)
		{
			int num2 = Mathf.RoundToInt((text.lineSpacing - 1f) * 10f);
			num = text.font.lineHeight + num2;
		}
		else
		{
			num = 11.5f;
		}
		int lineCount = text.cachedTextGenerator.lineCount;
		return num * (float)lineCount;
	}

	public static bool Passed(this Level level)
	{
		return Persistence.GetLevelRank(level).passed;
	}

	public static bool Locked(this Level level)
	{
		return (int)Persistence.GetLevelRank(level) == -2;
	}

	public static Rank GetRank(this Level level)
	{
		return Persistence.GetLevelRank(level);
	}

	public static Dictionary<K, V> Filter<K, V>(this Dictionary<K, V> dict, Predicate<KeyValuePair<K, V>> pred)
	{
		return dict.Where((KeyValuePair<K, V> it) => pred(it)).ToDictionary((KeyValuePair<K, V> it) => it.Key, (KeyValuePair<K, V> it) => it.Value);
	}

	public static UnityEngine.Color WithLightnessBoost(this UnityEngine.Color c, float lightnessBoost)
	{
		IList<double> list = HsluvConverter.RgbToHsluv(new List<double> { c.r, c.g, c.b });
		double num = 100.0 - list[2];
		list[2] += num * (double)lightnessBoost;
		IList<double> list2 = HsluvConverter.HsluvToRgb(list);
		return new UnityEngine.Color((float)list2[0], (float)list2[1], (float)list2[2], c.a);
	}

	public static (int year, int month, int day) GetDateOfChineseNewYear()
	{
		ChineseLunisolarCalendar chineseLunisolarCalendar = new ChineseLunisolarCalendar();
		GregorianCalendar gregorianCalendar = new GregorianCalendar();
		DateTime time = chineseLunisolarCalendar.ToDateTime(DateTime.UtcNow.Year, 1, 1, 0, 0, 0, 0);
		int year = gregorianCalendar.GetYear(time);
		int month = gregorianCalendar.GetMonth(time);
		int dayOfMonth = gregorianCalendar.GetDayOfMonth(time);
		return (year: year, month: month, day: dayOfMonth);
	}

	public static Vector2Int RoundToInt(this Vector2 v)
	{
		return new Vector2Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));
	}

	public static Vector2Int RoundToInt(this MultiWindow.Size v)
	{
		return new Vector2Int(Mathf.RoundToInt(v.width), Mathf.RoundToInt(v.height));
	}

	public static Vector2Int ToVector2Int(this MultiWindow.Size v)
	{
		return new Vector2Int(v.width, v.height);
	}

	public static void SaveRenderTextureToFile(RenderTexture renderTexture, string filePath)
	{
		Texture2D texture2D = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBAFloat, mipChain: false, linear: true);
		RenderTexture active = RenderTexture.active;
		RenderTexture.active = renderTexture;
		texture2D.ReadPixels(new Rect(0f, 0f, renderTexture.width, renderTexture.height), 0, 0);
		texture2D.Apply();
		RenderTexture.active = active;
		File.WriteAllBytes(filePath, texture2D.EncodeToPNG());
		("saving " + filePath + " to render texture").printe();
		if (Application.isPlaying)
		{
			UnityEngine.Object.Destroy(texture2D);
		}
		else
		{
			UnityEngine.Object.DestroyImmediate(texture2D);
		}
	}

	public static string GetHomePath()
	{
		return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
	}
}
