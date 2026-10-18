using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DG.Tweening;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityFileDialog;

namespace RDLevelEditor
{
	public static class RDEditorUtils
	{
		private const string Tab = "\t";

		public static string Indentation = "\t";

		private const int BaseFontSize = 8;

		public static readonly KeyCode[] ModifierKeys = new KeyCode[8]
		{
			KeyCode.LeftControl,
			KeyCode.RightControl,
			KeyCode.LeftShift,
			KeyCode.RightShift,
			KeyCode.LeftMeta,
			KeyCode.RightMeta,
			KeyCode.LeftAlt,
			KeyCode.RightAlt
		};

		public static int IndentationLevel
		{
			set
			{
				Indentation = "";
				for (int i = 0; i < value; i++)
				{
					Indentation += "\t";
				}
			}
		}

		public static void NotImplemented()
		{
		}

		public static string OpenArray(string header, bool isFirstMember = false)
		{
			string text = "";
			text += (isFirstMember ? ("{" + Environment.NewLine) : "");
			return text + Indentation + "\"" + header + "\":" + Environment.NewLine + Indentation + "[" + Environment.NewLine;
		}

		public static string OpenDictionary(string header, bool isFirstMember = false)
		{
			string text = "";
			text += (isFirstMember ? ("{" + Environment.NewLine) : "");
			return text + Indentation + "\"" + header + "\":" + Environment.NewLine + Indentation + "{" + Environment.NewLine;
		}

		public static string OpenDictionary()
		{
			return "{" + Environment.NewLine;
		}

		public static string CloseDictionary(bool isLastMember = false)
		{
			return Indentation + (isLastMember ? "}" : "},") + Environment.NewLine;
		}

		public static string CloseArray(bool isLastMember = false)
		{
			return Indentation + (isLastMember ? "]" : "],") + Environment.NewLine;
		}

		public static string EscapeTextForJSON(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				return "";
			}
			return text.Replace("\\", "\\\\").Replace("\r\n", "\\n").Replace("\n", "\\n")
				.Replace("\t", "\t")
				.Replace("\"", "\\\"");
		}

		public static T[] GetAllValuesFromEnum<T>()
		{
			return (T[])Enum.GetValues(typeof(T));
		}

		public static void AddOptionsWithEnumValues<T>(this Dropdown dropdown, bool localize = true)
		{
			T[] allValuesFromEnum = GetAllValuesFromEnum<T>();
			dropdown.AddOptionsWithEnumArray(allValuesFromEnum, localize);
		}

		public static void AddOptionsWithEnumValues(this Dropdown dropdown, Type type, bool localize = true)
		{
			List<string> options = (from object value in Enum.GetValues(type)
				select (!localize) ? value.ToString() : RDString.Get($"enum.{type.Name}.{value}")).ToList();
			dropdown.AddOptions(options);
		}

		public static Dictionary<int, T> AddOptionsWithEnumArray<T>(this Dropdown dropdown, IEnumerable<T> values, bool localize = true)
		{
			Dictionary<int, T> dictionary = new Dictionary<int, T>();
			int num = 0;
			List<string> list = new List<string>();
			foreach (T value in values)
			{
				if (localize)
				{
					list.Add(RDString.GetEnumValue(value));
				}
				else
				{
					list.Add(value.ToString());
				}
				dictionary.Add(num, value);
				num++;
			}
			dropdown.AddOptions(list);
			return dictionary;
		}

		public static void AddOptionsWithRowsList(this Dropdown dropdown)
		{
			List<string> list = new List<string>();
			foreach (LevelEvent_MakeRow rowsDatum in scnEditor.instance.rowsData)
			{
				string text = RDString.Get("editor.roomIndex").Replace("[index]", (rowsDatum.room + 1).ToString());
				list.Add(rowsDatum.GetRowString() + " (" + text + ")");
			}
			dropdown.AddOptions(list);
		}

		public static void AddOptionsWithRoomsList(this Dropdown dropdown)
		{
			List<string> list = new List<string>();
			for (int i = 0; i < 4; i++)
			{
				string item = RDString.Get("editor.roomIndex").Replace("[index]", (i + 1).ToString());
				list.Add(item);
			}
			dropdown.AddOptions(list);
		}

		public static string GetCurrentSound(this Dropdown dropdown, SoundEffect[] soundsArray)
		{
			if (dropdown.value < soundsArray.Length)
			{
				return soundsArray[dropdown.value].ToString();
			}
			return dropdown.options[dropdown.options.Count - 1].text;
		}

		public static bool SoundTypeIsGroup(GameSoundType t)
		{
			return RDEditorConstants.gameSoundGroups.ContainsKey(t);
		}

		public static int IndexOfStringInEnumArray<T>(T[] array, string s)
		{
			T value = RDUtils.ParseEnum<T>(s);
			if (value.Equals(default(T)))
			{
				return -1;
			}
			return Array.IndexOf(array, value);
		}

		public static void SetValue(this ToggleGroup toggleGroup, string value)
		{
			IEnumerable<Toggle> source = toggleGroup.ActiveToggles();
			if (source.Count() == 1 && source.First().name.Equals(value, StringComparison.InvariantCultureIgnoreCase))
			{
				return;
			}
			toggleGroup.SetAllTogglesOff();
			foreach (Transform item in toggleGroup.transform)
			{
				if (item.gameObject.name.Equals(value, StringComparison.InvariantCultureIgnoreCase))
				{
					item.GetComponent<Toggle>().isOn = true;
					return;
				}
			}
			Debug.LogWarning("RDEditorUtils.SetValue(toggleGroup, value) didn't find value " + value + ", setting it to first value.");
			toggleGroup.transform.GetChild(0).GetComponent<Toggle>().isOn = true;
		}

		public static void SetIndex(this ToggleGroup toggleGroup, int index)
		{
			toggleGroup.ActiveToggles();
			Toggle toggle = null;
			int num = 0;
			foreach (Transform item in toggleGroup.transform)
			{
				if (num == index)
				{
					toggle = item.GetComponent<Toggle>();
					if (toggle.isOn)
					{
						return;
					}
					break;
				}
				num++;
			}
			toggleGroup.SetAllTogglesOff();
			if (toggle != null)
			{
				toggle.isOn = true;
			}
			else
			{
				toggleGroup.transform.GetChild(0).GetComponent<Toggle>().isOn = true;
			}
		}

		public static bool LevelEventTypeShouldUpdateTimeline(LevelEventType levelEventType)
		{
			return Array.IndexOf(RDEditorConstants.levelEventsThatUpdateTimeline, levelEventType) != -1;
		}

		public static string GetValueFromActiveToggle(this ToggleGroup toggleGroup)
		{
			IEnumerable<Toggle> source = toggleGroup.ActiveToggles();
			if (source.Count() > 0)
			{
				return source.FirstOrDefault().name;
			}
			source = toggleGroup.GetComponentsInChildren<Toggle>();
			return source.FirstOrDefault((Toggle toggle) => toggle.isOn).gameObject.name;
		}

		public static int GetIndexFromActiveToggle(this ToggleGroup toggleGroup)
		{
			toggleGroup.ActiveToggles();
			int num = 0;
			foreach (Transform item in toggleGroup.transform)
			{
				if (item.GetComponent<Toggle>().isOn)
				{
					return num;
				}
				num++;
			}
			return -1;
		}

		public static T GetEnumValueFromActiveToggle<T>(this ToggleGroup toggleGroup, T defaultValue = default(T))
		{
			return RDUtils.ParseEnum(toggleGroup.GetValueFromActiveToggle(), defaultValue);
		}

		public static bool GetBoolValueFromActiveToggle(this ToggleGroup toggleGroup)
		{
			return toggleGroup.GetValueFromActiveToggle() == "on";
		}

		public static List<Character> GetAvailableDialogueCharacters()
		{
			Character[] availableCharacters = RDEditorConstants.AvailableCharacters;
			Dictionary<Character, Character[]> availableDialogueCharacters = RDEditorConstants.AvailableDialogueCharacters;
			List<Character> list = new List<Character>();
			foreach (Character character in availableCharacters)
			{
				list.Add(character);
				if (availableDialogueCharacters.ContainsKey(character))
				{
					list.AddRange(availableDialogueCharacters[character]);
				}
			}
			return list;
		}

		public static Character? GetCharacterFromAlias(string charName)
		{
			if (charName.IsNullOrEmpty())
			{
				return null;
			}
			if (Enum.TryParse<Character>(charName, out var result))
			{
				return result;
			}
			if (RDEditorConstants.DialogueCharacterAliases.TryGetValue(charName, out var value))
			{
				return value;
			}
			return null;
		}

		public static string EncodeInt(string key, int intValue, bool lastValue = false)
		{
			return EncodeKeyValue(key, intValue.ToString(), lastValue);
		}

		public static string EncodeBool(string key, bool boolValue, bool lastValue = false)
		{
			return EncodeKeyValue(key, boolValue ? "true" : "false", lastValue);
		}

		public static string EncodeFloat(string key, float floatValue, bool lastValue = false)
		{
			return EncodeKeyValue(key, floatValue.ToString(), lastValue);
		}

		public static string EncodeUnicodeString(string key, string stringValue, bool lastValue = false)
		{
			stringValue = EscapeTextForJSON(stringValue);
			return EncodeString(key, stringValue, lastValue);
		}

		public static string EncodeString(string key, string stringValue, bool lastValue = false)
		{
			return EncodeKeyValue(key, "\"" + stringValue + "\"", lastValue);
		}

		public static string EncodeVector2(string key, Vector2 vecValue, bool lastValue = false)
		{
			return EncodeFloatArray(key, new float[2] { vecValue.x, vecValue.y }, lastValue);
		}

		public static string EncodeIntArray(string key, int[] array, bool lastValue = false)
		{
			string text = "";
			for (int i = 0; i < array.Length; i++)
			{
				text += array[i];
				if (i != array.Length - 1)
				{
					text += ", ";
				}
			}
			return EncodeKeyValue(key, "[" + text + "]", lastValue);
		}

		public static string EncodeFloatArray(string key, float[] array, bool lastValue = false)
		{
			string text = "";
			for (int i = 0; i < array.Length; i++)
			{
				text += array[i];
				if (i != array.Length - 1)
				{
					text += ", ";
				}
			}
			return EncodeKeyValue(key, "[" + text + "]", lastValue);
		}

		public static string EncodeFloat2(string key, Float2 float2, bool lastValue = false)
		{
			string text = (float2.xUsed ? float2.x.ToString() : "null");
			string text2 = (float2.yUsed ? float2.y.ToString() : "null");
			return EncodeKeyValue(key, "[" + text + ", " + text2 + "]", lastValue);
		}

		public static string EncodeFloat2Array(string key, Float2[] array, bool lastValue = false)
		{
			string text = "";
			for (int i = 0; i < array.Length; i++)
			{
				Float2 @float = array[i];
				string text2 = (@float.xUsed ? @float.x.ToString() : "null");
				string text3 = (@float.yUsed ? @float.y.ToString() : "null");
				string text4 = "[" + text2 + ", " + text3 + "]";
				text += text4;
				if (i != array.Length - 1)
				{
					text += ", ";
				}
			}
			return EncodeKeyValue(key, "[" + text + "]", lastValue);
		}

		public static string EncodeFloatExpression2(string key, FloatExpression2 float2, bool lastValue = false)
		{
			string text = float2.x.ToJsonValue();
			string text2 = float2.y.ToJsonValue();
			return EncodeKeyValue(key, "[" + text + ", " + text2 + "]", lastValue);
		}

		public static string EncodeFloatExpression(string key, FloatExpression floatExp, bool lastValue = false)
		{
			return EncodeKeyValue(key, floatExp.ToJsonValue(), lastValue);
		}

		public static string EncodeStringArray(string key, string[] array, bool lastValue = false)
		{
			string text = "";
			for (int i = 0; i < array.Length; i++)
			{
				text = text + "\"" + array[i] + "\"";
				if (i != array.Length - 1)
				{
					text += ", ";
				}
			}
			return EncodeKeyValue(key, "[" + text + "]", lastValue);
		}

		public static string EncodeBoolArray(string key, bool[] array, bool lastValue = false)
		{
			string text = "";
			for (int i = 0; i < array.Length; i++)
			{
				text += $"{array[i]}";
				if (i != array.Length - 1)
				{
					text += ", ";
				}
			}
			return EncodeKeyValue(key, "[" + text + "]", lastValue);
		}

		public static string EncodeStringArrayForMods(string key, string[] array, bool lastValue = false)
		{
			IEnumerable<string> values = array.Select((string mod) => "\"" + mod.Trim() + "\"");
			string output = "[" + string.Join(", ", values) + "]";
			return EncodeKeyValue(key, output, lastValue);
		}

		public static string EncodeEnumArray<T>(string key, T[] array, bool lastValue = false)
		{
			string text = "";
			for (int i = 0; i < array.Length; i++)
			{
				text = text + "\"" + array[i].ToString() + "\"";
				if (i != array.Length - 1)
				{
					text += ", ";
				}
			}
			return EncodeKeyValue(key, "[" + text + "]", lastValue);
		}

		public static string EncodeSoundArray(string key, SoundData[] array, bool lastValue = false)
		{
			string text = "";
			for (int i = 0; i < array.Length; i++)
			{
				SoundData soundData = array[i];
				text = text + "{" + soundData.Encode(isLastValue: true) + "}";
				if (i != array.Length - 1)
				{
					text += ", ";
				}
			}
			return EncodeKeyValue(key, "[" + text + "]", lastValue);
		}

		public static string EncodeKeyValue(string key, string output, bool lastValue = false)
		{
			string text = (lastValue ? "" : ", ");
			return Indentation + "\"" + key + "\": " + output + text;
		}

		public static bool DecodeBool(object dictValue)
		{
			return (bool)dictValue;
		}

		public static float DecodeFloat(object dictValue)
		{
			return Convert.ToSingle(dictValue);
		}

		public static int DecodeInt(object dictValue)
		{
			return Convert.ToInt32(dictValue);
		}

		public static string DecodeString(object dictValue)
		{
			return dictValue as string;
		}

		public static string DecodeColor(object dictValue, bool hasAlpha)
		{
			string colorTemp = dictValue as string;
			if (colorTemp.StartsWith("pal"))
			{
				string s = colorTemp.Replace("pal", "");
				int result = 0;
				if (int.TryParse(s, out result))
				{
					if (result < 0 || result >= 21)
					{
						return ParseError();
					}
					return colorTemp;
				}
				return ParseError();
			}
			if (StringIsValidHexColor(colorTemp))
			{
				if (hasAlpha && colorTemp.Length == 6)
				{
					colorTemp += "ff";
				}
				return colorTemp;
			}
			return ParseError();
			string ParseError()
			{
				RDBaseDllDummy.printesw("error parsing color: " + colorTemp);
				if (!hasAlpha)
				{
					return "ffffff";
				}
				return "ffffffff";
			}
		}

		public static void DecodeTintData(Dictionary<string, object> dict, out BorderType border, out string borderColor, out bool tint, out string tintColor)
		{
			border = RDUtils.ParseEnum(dict["border"] as string, BorderType.None);
			if (dict.ContainsKey("borderOpacity"))
			{
				borderColor = DecodeColor(dict["borderColor"], hasAlpha: false);
				if (borderColor.Length == 6)
				{
					borderColor += GetAlphaHex(Convert.ToInt32(dict["borderOpacity"]));
				}
			}
			else
			{
				borderColor = DecodeColor(dict["borderColor"], hasAlpha: true);
			}
			tint = DecodeBool(dict["tint"]);
			if (dict.ContainsKey("tintOpacity"))
			{
				tintColor = DecodeColor(dict["tintColor"], hasAlpha: false);
				if (tintColor.Length == 6)
				{
					tintColor += GetAlphaHex(Convert.ToInt32(dict["tintOpacity"]));
				}
			}
			else
			{
				tintColor = DecodeColor(dict["tintColor"], hasAlpha: true);
			}
			static string GetAlphaHex(int alpha)
			{
				string text = ColorUtility.ToHtmlStringRGBA(Color.white.WithAlpha((float)alpha * 0.01f));
				return text.Substring(text.Length - 2);
			}
		}

		public static T DecodeEnum<T>(object value)
		{
			return RDUtils.ParseEnum<T>(value as string);
		}

		public static Ease DecodeEase(object value)
		{
			Ease ease = RDUtils.ParseEnum(value as string, Ease.Unset);
			return ease switch
			{
				Ease.InOutFlash => Ease.InOutQuad, 
				Ease.InFlash => Ease.InQuad, 
				Ease.OutFlash => Ease.OutQuad, 
				_ => ease, 
			};
		}

		public static T[] DecodeEnumArray<T>(object dictValue)
		{
			List<object> obj = dictValue as List<object>;
			T[] array = new T[obj.Count];
			int num = 0;
			foreach (object item in obj)
			{
				array[num] = RDUtils.ParseEnum<T>(item as string);
				num++;
			}
			return array;
		}

		public static int[] DecodeIntArray(object dictValue)
		{
			List<object> obj = dictValue as List<object>;
			int[] array = new int[obj.Count];
			int num = 0;
			foreach (object item in obj)
			{
				array[num] = Convert.ToInt32(item);
				num++;
			}
			return array;
		}

		public static string[] DecodeStringArray(object listValue)
		{
			List<object> obj = listValue as List<object>;
			string[] array = new string[obj.Count];
			int num = 0;
			foreach (object item in obj)
			{
				array[num] = (string)item;
				num++;
			}
			return array;
		}

		public static bool[] DecodeBoolArray(object listValue)
		{
			List<object> obj = listValue as List<object>;
			bool[] array = new bool[obj.Count];
			int num = 0;
			foreach (object item in obj)
			{
				array[num] = bool.Parse(item.ToString());
				num++;
			}
			return array;
		}

		public static float[] DecodeFloatArray(object dictValue)
		{
			List<object> obj = dictValue as List<object>;
			float[] array = new float[obj.Count];
			int num = 0;
			foreach (object item in obj)
			{
				array[num] = Convert.ToSingle(item);
				num++;
			}
			return array;
		}

		public static Float2 DecodeFloat2(object dictValue)
		{
			List<object> list = dictValue as List<object>;
			_ = new float[list.Count];
			Float2 result = new Float2
			{
				xUsed = true
			};
			if (list[0] == null)
			{
				result.xUsed = false;
			}
			else
			{
				result.x = Convert.ToSingle(list[0]);
			}
			result.yUsed = true;
			if (list[1] == null)
			{
				result.yUsed = false;
			}
			else
			{
				result.y = Convert.ToSingle(list[1]);
			}
			return result;
		}

		public static Float2[] DecodeFloat2Array(object dictValue)
		{
			List<object> list = dictValue as List<object>;
			Float2[] array = new Float2[list.Count];
			for (int i = 0; i < list.Count; i++)
			{
				array[i] = DecodeFloat2(list[i]);
			}
			return array;
		}

		public static FloatExpression2 DecodeFloatExpression2(object dictValue)
		{
			List<object> list = dictValue as List<object>;
			return new FloatExpression2(FloatExpression.FromJsonObject(list[0]), FloatExpression.FromJsonObject(list[1]));
		}

		public static FloatExpression DecodeFloatExpression(object dictValue)
		{
			return FloatExpression.FromJsonObject(dictValue);
		}

		public static SoundData[] DecodeSoundArray(object dictValue)
		{
			List<object> list = dictValue as List<object>;
			SoundData[] array = new SoundData[list.Count];
			for (int i = 0; i < list.Count; i++)
			{
				Dictionary<string, object> dict = list[i] as Dictionary<string, object>;
				SoundData soundData = new SoundData("");
				soundData.Decode(dict);
				array[i] = soundData;
			}
			return array;
		}

		public static string CamelCaseToReadableName(string camelCase)
		{
			return Regex.Replace(camelCase, "(\\B[A-Z]+?(?=[A-Z][^A-Z])|\\B[A-Z]+?(?=[^A-Z]))", " $1");
		}

		public static Texture2D LoadLevelTexture(LevelSource levelSource, string image, string subFolder = "", bool fromCustomCharactersFolder = false, bool isAlpha8 = false)
		{
			if (levelSource == LevelSource.InternalPath || levelSource == LevelSource.CutscenesPath)
			{
				string text = Path.GetDirectoryName(scnGame.currentLevelPath);
				if (!string.IsNullOrEmpty(subFolder))
				{
					text = text + "/" + subFolder;
				}
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(image);
				return Resources.Load<Texture2D>(text + "/" + fileNameWithoutExtension);
			}
			return GetCustomTextureWithName(image, subFolder, fromCustomCharactersFolder, isAlpha8);
		}

		public static string LoadLevelText(LevelSource levelSource, string textFilename, string subFolder = "", bool fromCustomCharactersFolder = false)
		{
			if (levelSource == LevelSource.InternalPath || levelSource == LevelSource.CutscenesPath)
			{
				string text = Path.GetDirectoryName(scnGame.currentLevelPath);
				if (!string.IsNullOrEmpty(subFolder))
				{
					text = text + "/" + subFolder;
				}
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(textFilename);
				TextAsset textAsset = Resources.Load<TextAsset>(text + "/" + fileNameWithoutExtension);
				if (!(textAsset == null))
				{
					return textAsset.text;
				}
				return string.Empty;
			}
			string text2 = (fromCustomCharactersFolder ? LevelValidation.CustomCharactersPath : GetCurrentLevelFolderPath());
			string path;
			if (!string.IsNullOrEmpty(subFolder))
			{
				string[] obj = new string[5] { text2, null, null, null, null };
				char directorySeparatorChar = Path.DirectorySeparatorChar;
				obj[1] = directorySeparatorChar.ToString();
				obj[2] = subFolder;
				directorySeparatorChar = Path.DirectorySeparatorChar;
				obj[3] = directorySeparatorChar.ToString();
				obj[4] = textFilename;
				path = string.Concat(obj);
			}
			else
			{
				char directorySeparatorChar = Path.DirectorySeparatorChar;
				path = text2 + directorySeparatorChar + textFilename;
			}
			string result = string.Empty;
			if (File.Exists(path))
			{
				result = File.ReadAllText(path);
			}
			return result;
		}

		public static string GetCurrentLevelFolderPath()
		{
			scnEditor instance = scnEditor.instance;
			if (instance == null)
			{
				return Path.GetDirectoryName(scnGame.currentLevelPath);
			}
			if (!string.IsNullOrEmpty(instance.openedFilePath))
			{
				return Path.GetDirectoryName(instance.openedFilePath);
			}
			return "";
		}

		public static IEnumerator AudioClipFromFilename(string filename, bool suppressWarning = false, string subFolder = "")
		{
			bool num = filename.HasAudioFileExtension();
			bool flag = false;
			if (!num)
			{
				string clipName = RDConstants.data.soundData.Get(filename).folder + "/" + filename;
				AudioClip audioClip = Singleton<AudioManager>.Instance.FindOrLoadAudioClip(clipName);
				if (audioClip != null)
				{
					yield return new RDAudioLoadResult(RDAudioLoadType.SuccessInternalClipLoaded, audioClip);
					flag = true;
				}
			}
			else
			{
				if (scnGame.instance != null)
				{
					LevelSource levelToLoadSource = scnGame.levelToLoadSource;
					if (levelToLoadSource == LevelSource.InternalPath || levelToLoadSource == LevelSource.CutscenesPath)
					{
						filename = Path.GetFileNameWithoutExtension(filename);
						string clipName2;
						if (!string.IsNullOrEmpty(subFolder))
						{
							string[] obj = new string[5]
							{
								GetCurrentLevelFolderPath(),
								null,
								null,
								null,
								null
							};
							char directorySeparatorChar = Path.DirectorySeparatorChar;
							obj[1] = directorySeparatorChar.ToString();
							obj[2] = subFolder;
							directorySeparatorChar = Path.DirectorySeparatorChar;
							obj[3] = directorySeparatorChar.ToString();
							obj[4] = filename;
							clipName2 = string.Concat(obj);
						}
						else
						{
							string currentLevelFolderPath = GetCurrentLevelFolderPath();
							char directorySeparatorChar = Path.DirectorySeparatorChar;
							clipName2 = currentLevelFolderPath + directorySeparatorChar + filename;
						}
						AudioClip audioClip2 = Singleton<AudioManager>.Instance.FindOrLoadAudioClip(clipName2);
						if (audioClip2 != null)
						{
							yield return new RDAudioLoadResult(RDAudioLoadType.SuccessInternalClipLoaded, audioClip2);
							flag = true;
						}
						goto IL_02a8;
					}
				}
				string path;
				if (!string.IsNullOrEmpty(subFolder))
				{
					string[] obj2 = new string[5]
					{
						GetCurrentLevelFolderPath(),
						null,
						null,
						null,
						null
					};
					char directorySeparatorChar = Path.DirectorySeparatorChar;
					obj2[1] = directorySeparatorChar.ToString();
					obj2[2] = subFolder;
					directorySeparatorChar = Path.DirectorySeparatorChar;
					obj2[3] = directorySeparatorChar.ToString();
					obj2[4] = filename;
					path = string.Concat(obj2);
				}
				else
				{
					string currentLevelFolderPath2 = GetCurrentLevelFolderPath();
					char directorySeparatorChar = Path.DirectorySeparatorChar;
					path = currentLevelFolderPath2 + directorySeparatorChar + filename;
				}
				if (RDFile.Exists(path))
				{
					MonoBehaviour owner = ((scnEditor.instance != null) ? ((RDBaseDllDummy)scnEditor.instance) : ((RDBaseDllDummy)scnGame.instance));
					CoroutineWithData loadAudio = new CoroutineWithData(owner, Singleton<AudioManager>.Instance.FindOrLoadAudioClipExternal(path));
					yield return loadAudio.coroutine;
					yield return (RDAudioLoadResult)loadAudio.result;
					flag = true;
				}
			}
			goto IL_02a8;
			IL_02a8:
			if (!flag && !suppressWarning)
			{
				Debug.LogWarning("AudioClip doesn't exist: " + filename);
				yield return new RDAudioLoadResult(RDAudioLoadType.ErrorFileNotFound, null);
			}
		}

		public static RDAudioLoadType FindClip(string filename, bool suppressWarning = false, string subFolder = "")
		{
			if (!filename.HasAudioFileExtension())
			{
				string text = "snd";
				if (filename.StartsWith(text))
				{
					string clipName = RDConstants.data.soundData.Get(filename).folder + "/" + filename;
					if (Singleton<AudioManager>.Instance.FindOrLoadAudioClip(clipName) != null)
					{
						return RDAudioLoadType.SuccessInternalClipLoaded;
					}
				}
				else
				{
					string text2 = text + filename;
					string clipName2 = RDConstants.data.soundData.Get(text2).folder + "/" + text2;
					if (Singleton<AudioManager>.Instance.FindOrLoadAudioClip(clipName2) != null)
					{
						return RDAudioLoadType.SuccessInternalClipLoaded;
					}
					clipName2 = RDConstants.data.soundData.Get(filename).folder + "/" + filename;
					if (Singleton<AudioManager>.Instance.FindOrLoadAudioClip(clipName2) != null)
					{
						return RDAudioLoadType.SuccessInternalClipLoaded;
					}
				}
			}
			else
			{
				if (scnGame.instance != null)
				{
					LevelSource levelToLoadSource = scnGame.levelToLoadSource;
					if (levelToLoadSource == LevelSource.InternalPath || levelToLoadSource == LevelSource.CutscenesPath)
					{
						filename = Path.GetFileNameWithoutExtension(filename);
						string clipName3;
						if (!string.IsNullOrEmpty(subFolder))
						{
							string[] obj = new string[5]
							{
								GetCurrentLevelFolderPath(),
								null,
								null,
								null,
								null
							};
							char directorySeparatorChar = Path.DirectorySeparatorChar;
							obj[1] = directorySeparatorChar.ToString();
							obj[2] = subFolder;
							directorySeparatorChar = Path.DirectorySeparatorChar;
							obj[3] = directorySeparatorChar.ToString();
							obj[4] = filename;
							clipName3 = string.Concat(obj);
						}
						else
						{
							string currentLevelFolderPath = GetCurrentLevelFolderPath();
							char directorySeparatorChar = Path.DirectorySeparatorChar;
							clipName3 = currentLevelFolderPath + directorySeparatorChar + filename;
						}
						if (Singleton<AudioManager>.Instance.FindOrLoadAudioClip(clipName3) != null)
						{
							return RDAudioLoadType.SuccessInternalClipLoaded;
						}
						goto IL_021c;
					}
				}
				scnEditor instance = scnEditor.instance;
				string text3 = ((instance == null) ? Path.GetDirectoryName(scnGame.currentLevelPath) : (string.IsNullOrEmpty(instance.openedFilePath) ? "" : Path.GetDirectoryName(instance.openedFilePath)));
				string path;
				if (!string.IsNullOrEmpty(subFolder))
				{
					string[] obj2 = new string[5] { text3, null, null, null, null };
					char directorySeparatorChar = Path.DirectorySeparatorChar;
					obj2[1] = directorySeparatorChar.ToString();
					obj2[2] = subFolder;
					directorySeparatorChar = Path.DirectorySeparatorChar;
					obj2[3] = directorySeparatorChar.ToString();
					obj2[4] = filename;
					path = string.Concat(obj2);
				}
				else
				{
					char directorySeparatorChar = Path.DirectorySeparatorChar;
					path = text3 + directorySeparatorChar + filename;
				}
				if (RDFile.Exists(path))
				{
					return RDAudioLoadType.SuccessExternalClipLoaded;
				}
			}
			goto IL_021c;
			IL_021c:
			if (!suppressWarning)
			{
				Debug.LogWarning("RDEditorUtils - Audio not found: " + filename);
			}
			return RDAudioLoadType.ErrorFileNotFound;
		}

		public static SoundEffect GetClapSoundForP2(SoundEffect clap)
		{
			return clap switch
			{
				SoundEffect.ClapHit => SoundEffect.ClapHitP2, 
				SoundEffect.ReverbClap => SoundEffect.ReverbClapP2, 
				SoundEffect.ClapHitEcho => SoundEffect.ClapHitEchoP2, 
				SoundEffect.ClapHitMassivePreEcho => SoundEffect.ClapHitMassivePreEchoP2, 
				SoundEffect.SnareAcoustic2 => SoundEffect.SnareAcoustic4, 
				SoundEffect.SnareHouse => SoundEffect.SnareHouseP2, 
				SoundEffect.SnareVapor => SoundEffect.SnareVaporP2, 
				_ => SoundEffect.NotFound, 
			};
		}

		public static string GetClapSoundForP2(string clap)
		{
			if (Enum.TryParse<SoundEffect>(clap, out var result) && Enum.IsDefined(typeof(SoundEffect), result))
			{
				return GetClapSoundForP2(result).ToString();
			}
			return SoundEffect.NotFound.ToString();
		}

		public static List<string> AnimationClipNamesFromCustomCharacterName(string customCharacterName, string subFolder = "")
		{
			string text = LoadLevelText(scnGame.levelToLoadSource, customCharacterName + ".json", subFolder);
			if (string.IsNullOrEmpty(text))
			{
				return new List<string>();
			}
			CustomAnimationData customAnimationData = default;
			customAnimationData.LoadFromJson(text);
			return new List<string>(customAnimationData.clips.Keys);
		}

		public static bool StringIsValidHexColor(string s)
		{
			bool num = s.Length == 6 || s.Length == 8;
			int result = 0;
			bool flag = int.TryParse(s, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out result);
			return num & flag;
		}

		public static bool StringIsValidHexColor(string s, bool hasAlpha)
		{
			int num = (hasAlpha ? 8 : 6);
			if (s.Length == num)
			{
				int result = 0;
				return int.TryParse(s, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out result);
			}
			return false;
		}

		public static Color HexOrPaletteIndexToColor(this string s, bool usePaletteAlpha = false, bool useEditorPalette = false)
		{
			if (s.StartsWith("pal"))
			{
				string s2 = s.Replace("pal", "");
				int result = 0;
				if (int.TryParse(s2, out result))
				{
					string[] colorPalette;
					if (useEditorPalette)
					{
						colorPalette = scnEditor.instance.colorPalette;
					}
					else
					{
						RDLevelData current = RDLevelData.current;
						if (current == null || result < 0 || result >= 21)
						{
							return Color.black;
						}
						colorPalette = current.colorPalette;
					}
					string text = colorPalette[result];
					if (!usePaletteAlpha)
					{
						text = text.Remove(text.Length - 2);
					}
					return text.HexToColor();
				}
				return Color.black;
			}
			return s.HexToColor();
		}

		public static bool ColorIsDark(Color color)
		{
			return Mathf.Sqrt(0.299f * Mathf.Pow(color.r * 255f, 2f) + 0.587f * Mathf.Pow(color.g * 255f, 2f) + 0.114f * Mathf.Pow(color.b * 255f, 2f)) < 127.5f;
		}

		public static string ShowFileSelectorForAudio(string title, long maximumSize = -1L)
		{
			scnEditor.PauseIfUnpaused();
			string[] array = ShowFileSelector(title, RDString.Get("editor.dialog.audioFileFormat"), GC.SupportedAudioFiles, RDString.Get("editor.dialog.saveBeforeImportingSounds"), maximumSize);
			if (array != null && array.Length != 0)
			{
				return array[0];
			}
			return null;
		}

		public static string ShowFileSelectorForImage(string title, long maximumSize = -1L)
		{
			string[] array = ShowFileSelector(title, RDString.Get("editor.dialog.imageFileFormat"), GC.SupportedImageFiles, RDString.Get("editor.dialog.saveBeforeImportingImages"), maximumSize);
			if (array != null && array.Length != 0)
			{
				return array[0];
			}
			return null;
		}

		public static string ShowFileSelectorForText(string title, long maximumSize = -1L, bool multiselect = false)
		{
			string[] array = ShowFileSelector(title, RDString.Get("editor.dialog.jsonFileFormat"), GC.SupportedJSONFiles, RDString.Get("editor.dialog.saveBeforeImporting"), maximumSize);
			if (array != null && array.Length != 0)
			{
				return array[0];
			}
			return null;
		}

		public static string[] ShowFileSelectorForImages(string title, long maximumSize = -1L)
		{
			return ShowFileSelector(title, RDString.Get("editor.dialog.imageFileFormat"), GC.SupportedImageFiles, RDString.Get("editor.dialog.saveBeforeImportingImages"), maximumSize, multiselect: true);
		}

		public static string ShowFileSelectorForFont(string title, long maximumSize = -1L)
		{
			scnEditor.PauseIfUnpaused();
			string[] array = ShowFileSelector(title, RDString.Get("editor.dialog.fontFileFormat"), GC.SupportedFontFiles, RDString.Get("editor.dialog.saveBeforeImportingFonts"), maximumSize);
			if (array != null && array.Length != 0)
			{
				return array[0];
			}
			return null;
		}

		public static void AskUserToSaveLevel(string levelNotSavedMessage)
		{
			scnEditor editor = scnEditor.instance;
			editor.dialog.Show(levelNotSavedMessage, new string[2]
			{
				RDString.Get("editor.dialog.saveLevel"),
				RDString.Get("editor.dialog.cancel")
			}, (int x) =>
			{
				editor.dialog.Hide();
				if (x == 0)
				{
					editor.ShowSavePanel(levelNotSavedMessage, saveEmptyFile: false);
				}
			});
		}

		public static string[] ShowFileSelector(string title, string extensionDescription, string[] extensions, string levelNotSavedMessage, long maximumSize = -1L, bool multiselect = false)
		{
			scnEditor editor = scnEditor.instance;
			if (string.IsNullOrEmpty(editor.openedFilePath))
			{
				AskUserToSaveLevel(levelNotSavedMessage);
				return null;
			}
			using FileDialog fileDialog = new FileDialog();
			fileDialog.SetTitle(title);
			fileDialog.SetDirectory(scnEditor.GetLastUsedFolder());
			fileDialog.AddFilter(extensionDescription, extensions);
			string[] array = (multiselect ? fileDialog.PickFiles() : new string[1] { fileDialog.PickFile() });
			if (array == null || array.Length == 0)
			{
				return null;
			}
			List<string> list = new List<string>();
			string[] array2 = array;
			foreach (string text in array2)
			{
				if (string.IsNullOrEmpty(text))
				{
					return null;
				}
				string filePath = Uri.UnescapeDataString(text.Replace("file:", ""));
				string directoryName = Path.GetDirectoryName(filePath);
				string fileName = Path.GetFileName(filePath);
				string[] invalidFileNameChars = LevelValidation.InvalidFileNameChars;
				foreach (string text2 in invalidFileNameChars)
				{
					if (fileName.Contains(text2))
					{
						string text3 = RDString.Get("editor.dialog.openedFileWithInvalidChar").Replace("[filename]", fileName);
						text3 = text3.Replace("[invalidChar]", text2);
						string[] options = new string[1] { RDString.Get("editor.PublishLevel.dialogOK") };
						editor.dialog.Show(text3, options, (int option) =>
						{
							editor.dialog.Hide();
						});
						return null;
					}
				}
				string directoryName2 = Path.GetDirectoryName(editor.openedFilePath);
				string newPath = Path.Combine(directoryName2, fileName);
				long length = new FileInfo(filePath).Length;
				if (maximumSize != -1 && length >= maximumSize)
				{
					string newValue = ((maximumSize >= 1000000) ? (maximumSize / 1000000 + RDString.Get("editor.LevelSettings.mbLabel")) : (maximumSize / 1000 + RDString.Get("editor.LevelSettings.kbLabel")));
					string message = RDString.Get("editor.LevelSettings.tooBigImage").Replace("[size]", newValue);
					editor.dialog.Show(message, new string[1] { RDString.Get("editor.PublishLevel.dialogOK") }, (int option) =>
					{
						editor.dialog.Hide();
					});
					continue;
				}
				if (directoryName != directoryName2)
				{
					RDBaseDllDummy.printes("we are not on the same folder, we should copy this to the folder");
					if (RDFile.Exists(newPath))
					{
						editor.dialog.Show(RDString.Get("editor.dialog.overwriteFiles").Replace("[files]", Path.GetFileName(newPath)), new string[2]
						{
							RDString.Get("editor.dialog.overwrite"),
							RDString.Get("editor.PublishLevel.dialogCancel")
						}, (int option) =>
						{
							editor.dialog.Hide(() =>
							{
								if (option == 0)
								{
									File.Copy(filePath, newPath, overwrite: true);
								}
							});
						});
					}
					else
					{
						File.Copy(filePath, newPath);
					}
				}
				list.Add(fileName);
			}
			return list.ToArray();
		}

		public static List<string> GetAllAudioFilesFromPath(string path)
		{
			IEnumerable<string> enumerable = from s in Directory.GetFiles(path, "*.*", SearchOption.TopDirectoryOnly)
				where s.HasAudioFileExtension()
				select s;
			List<string> list = new List<string>();
			char directorySeparatorChar = Path.DirectorySeparatorChar;
			string oldValue = path + directorySeparatorChar;
			foreach (string item in enumerable)
			{
				list.Add(item.Replace(oldValue, ""));
			}
			return list;
		}

		private static Texture2D LoadTextureFileToCache(Dictionary<string, CachedTexture> customTextures, string fullPath, string name, bool isAlpha8 = false)
		{
			byte[] data = RDFile.ReadAllBytes(fullPath);
			Texture2D texture2D = new Texture2D(2, 2, TextureFormat.ARGB32, mipChain: false);
			texture2D.LoadImage(data);
			if (isAlpha8)
			{
				Texture2D texture2D2 = new Texture2D(texture2D.width, texture2D.height, TextureFormat.Alpha8, mipChain: false);
				NativeArray<byte> rawTextureData = texture2D.GetRawTextureData<byte>();
				NativeArray<byte> rawTextureData2 = texture2D2.GetRawTextureData<byte>();
				int length = rawTextureData2.Length;
				for (int i = 0; i < length; i++)
				{
					rawTextureData2[i] = rawTextureData[i * 4];
				}
				UnityEngine.Object.Destroy(texture2D);
				texture2D = texture2D2;
			}
			texture2D.Apply(updateMipmaps: false, makeNoLongerReadable: true);
			texture2D.name = name;
			DateTime lastWriteTimeUtc = new FileInfo(fullPath).LastWriteTimeUtc;
			CachedTexture value = new CachedTexture
			{
				fileLastAccessed = lastWriteTimeUtc,
				texture = texture2D,
				isAlpha8 = isAlpha8
			};
			customTextures[name] = value;
			return texture2D;
		}

		private static TMP_FontAsset LoadFontFileToCache(Dictionary<string, CachedFont> customFonts, string fullPath, string name)
		{
			RDFile.ReadAllBytes(fullPath);
			TMP_FontAsset tMP_FontAsset = TMP_FontAsset.CreateFontAsset(new Font(fullPath));
			DateTime lastWriteTimeUtc = new FileInfo(fullPath).LastWriteTimeUtc;
			CachedFont value = new CachedFont
			{
				fileLastAccessed = lastWriteTimeUtc,
				fontAsset = tMP_FontAsset
			};
			customFonts[name] = value;
			return tMP_FontAsset;
		}

		public static Texture2D GetCustomTextureWithName(string name, string subFolderName = "", bool fromCustomCharactersFolder = false, bool isAlpha8 = false)
		{
			if (string.IsNullOrEmpty(name))
			{
				return null;
			}
			string text = (fromCustomCharactersFolder ? LevelValidation.CustomCharactersPath : GetCurrentLevelFolderPath());
			Dictionary<string, CachedTexture> textureCache = scrVfxControl.textureCache;
			string text2;
			if (!string.IsNullOrEmpty(subFolderName))
			{
				string[] obj = new string[5] { text, null, null, null, null };
				char directorySeparatorChar = Path.DirectorySeparatorChar;
				obj[1] = directorySeparatorChar.ToString();
				obj[2] = subFolderName;
				directorySeparatorChar = Path.DirectorySeparatorChar;
				obj[3] = directorySeparatorChar.ToString();
				obj[4] = name;
				text2 = string.Concat(obj);
			}
			else
			{
				char directorySeparatorChar = Path.DirectorySeparatorChar;
				text2 = text + directorySeparatorChar + name;
			}
			if (!RDFile.Exists(text2))
			{
				return null;
			}
			if (textureCache.ContainsKey(name))
			{
				DateTime lastWriteTimeUtc = new FileInfo(text2).LastWriteTimeUtc;
				if (DateTime.Compare(textureCache[name].fileLastAccessed, lastWriteTimeUtc) != 0)
				{
					RDBaseDllDummy.printem("Reloading texture because is has been modified");
					textureCache.Remove(name);
					return LoadTextureFileToCache(textureCache, text2, name, isAlpha8);
				}
				if (textureCache[name].isAlpha8 && !isAlpha8)
				{
					RDBaseDllDummy.printem("Reloading texture as a non-Alpha8 texture");
					textureCache.Remove(name);
					return LoadTextureFileToCache(textureCache, text2, name);
				}
				return textureCache[name].texture;
			}
			return LoadTextureFileToCache(textureCache, text2, name, isAlpha8);
		}

		public static TMP_FontAsset GetCustomFontWithName(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return null;
			}
			Dictionary<string, CachedFont> fontCache = scrVfxControl.fontCache;
			string currentLevelFolderPath = GetCurrentLevelFolderPath();
			char directorySeparatorChar = Path.DirectorySeparatorChar;
			string text = currentLevelFolderPath + directorySeparatorChar + name;
			if (!RDFile.Exists(text))
			{
				return null;
			}
			if (fontCache.ContainsKey(name))
			{
				DateTime lastWriteTimeUtc = new FileInfo(text).LastWriteTimeUtc;
				if (DateTime.Compare(fontCache[name].fileLastAccessed, lastWriteTimeUtc) != 0)
				{
					RDBaseDllDummy.printem("Reloading font because is has been modified");
					fontCache.Remove(name);
					return LoadFontFileToCache(fontCache, text, name);
				}
				Debug.Log("Using cached font: " + name);
				return fontCache[name].fontAsset;
			}
			return LoadFontFileToCache(fontCache, text, name);
		}

		public static void SetValueForEnumArray<T>(this Dropdown dropdown, T[] array, string filename)
		{
			bool valueFound = false;
			T value = RDUtils.ParseEnum<T>(filename, out valueFound);
			int num = (valueFound ? Array.IndexOf(array, value) : (-1));
			if (num == -1)
			{
				if (array.Length < dropdown.options.Count)
				{
					dropdown.options.GetLastObject().text = filename;
				}
				else
				{
					dropdown.AddOptions(new List<string> { filename });
				}
				num = array.Length;
			}
			else if (array.Length < dropdown.options.Count)
			{
				while (array.Length < dropdown.options.Count)
				{
					dropdown.options.RemoveAt(dropdown.options.Count - 1);
				}
			}
			dropdown.value = num;
			dropdown.RefreshShownValue();
		}

		public static bool CheckForKeyCombo(bool control, bool shift, KeyCode key, bool xorMode = false)
		{
			return CheckForKeyCombo(control, alt: false, shift, key, xorMode);
		}

		public static bool CheckForKeyCombo(bool control, bool alt, bool shift, KeyCode key, bool strictMatch = false)
		{
			if (!Input.GetKeyDown(key))
			{
				return false;
			}
			bool control2 = ControlIsPressed();
			bool alt2 = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
			bool shift2 = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
			byte b = ComboToByte(control, alt, shift);
			byte b2 = ComboToByte(control2, alt2, shift2);
			if (b == 0)
			{
				strictMatch = true;
			}
			if (strictMatch)
			{
				return b2 == b;
			}
			return (b2 & b) == b;
			static byte ComboToByte(bool _control, bool _alt, bool _shift)
			{
				return (byte)((_control ? 4 : 0) + (_alt ? 2 : 0) + (_shift ? 1 : 0));
			}
		}

		public static bool ControlIsPressed()
		{
			bool num = ((Application.platform != RuntimePlatform.OSXPlayer || Application.isEditor) ? (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) : (Input.GetKey(KeyCode.LeftMeta) || Input.GetKey(KeyCode.RightMeta)));
			bool flag = Application.platform == RuntimePlatform.WindowsEditor && (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt));
			return num | flag;
		}

		public static string KeyComboToString(bool control, bool shift, KeyCode key, bool noJoinerColor = false, bool lowercase = true)
		{
			return KeyComboToString(control, alt: false, shift, key, noJoinerColor, lowercase);
		}

		public static string KeyComboToString(bool control, bool alt, bool shift, KeyCode key, bool noJoinerColor = false, bool lowercase = true)
		{
			bool flag = (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer) && !Application.isEditor;
			string text = (noJoinerColor ? "+" : "<color=#A6A6A6>+</color>");
			string text2 = "";
			if (control)
			{
				text2 = text2 + (flag ? "Cmd" : "Ctrl") + text;
			}
			if (alt)
			{
				text2 = text2 + "Alt" + text;
			}
			if (shift)
			{
				text2 = text2 + "Shift" + text;
			}
			string text3 = text2;
			string text4;
			switch (key)
			{
			case KeyCode.Plus:
			case KeyCode.KeypadPlus:
				text4 = "+";
				break;
			case KeyCode.Minus:
			case KeyCode.KeypadMinus:
				text4 = "-";
				break;
			case KeyCode.Equals:
			case KeyCode.KeypadEquals:
				text4 = "=";
				break;
			case KeyCode.Alpha1:
				text4 = "1";
				break;
			case KeyCode.Alpha2:
				text4 = "2";
				break;
			case KeyCode.Alpha3:
				text4 = "3";
				break;
			case KeyCode.Alpha4:
				text4 = "4";
				break;
			case KeyCode.Alpha5:
				text4 = "5";
				break;
			case KeyCode.Alpha6:
				text4 = "6";
				break;
			case KeyCode.Alpha7:
				text4 = "7";
				break;
			case KeyCode.Alpha8:
				text4 = "8";
				break;
			case KeyCode.Alpha9:
				text4 = "9";
				break;
			case KeyCode.Alpha0:
				text4 = "0";
				break;
			default:
				text4 = key.ToString();
				break;
			}
			text2 = text3 + text4;
			if (lowercase)
			{
				text2 = text2.ToLower();
			}
			return text2;
		}

		public static float GetScrollWheel()
		{
			return Input.GetAxis("Mouse ScrollWheel");
		}

		public static void UpdateUIText(Text uiText, string s, bool applyOutlines = false)
		{
			FontPack appropiateFontPackForString = RDString.GetAppropiateFontPackForString(s);
			if (uiText.font != appropiateFontPackForString.font)
			{
				uiText.font = appropiateFontPackForString.font;
			}
			uiText.fontSize = Mathf.RoundToInt(8f);
			uiText.lineSpacing = 1f;
			if (applyOutlines)
			{
				Outline[] components = uiText.gameObject.GetComponents<Outline>();
				int num = 0;
				float num2 = (appropiateFontPackForString.usePixelFont ? 8f : 12f);
				float num3 = (float)uiText.fontSize / num2;
				Outline[] array = components;
				foreach (Outline obj in array)
				{
					bool flag = num % 2 == 0;
					obj.effectDistance = new Vector2(flag ? num3 : 0f, flag ? 0f : num3);
					num++;
				}
			}
		}

		public static string RemoveTextIfStartsWith(this string fullText, string token)
		{
			if (!fullText.StartsWith(token))
			{
				return null;
			}
			return fullText.Remove(0, token.Length);
		}

		public static string[] GetParameters(this string text)
		{
			text = text.Trim();
			if (!text.StartsWith("(") || !text.EndsWith(")"))
			{
				return null;
			}
			if (text.Length == 2)
			{
				return null;
			}
			text = text.Trim('(', ')');
			return text.Split(',');
		}

		public static bool ModifierKeyIsPressed()
		{
			for (int i = 0; i < ModifierKeys.Length; i++)
			{
				if (Input.GetKey(ModifierKeys[i]))
				{
					return true;
				}
			}
			return false;
		}

		public static bool InitializeCustomCharacterFromPath(string sourceJsonPath, bool placeFilesInSubFolder = false, bool fromCustomCharactersFolder = false, bool askForSecondaryTextures = true)
		{
			if (string.IsNullOrEmpty(sourceJsonPath))
			{
				return false;
			}
			scnEditor editor = scnEditor.instance;
			if (editor == null)
			{
				return false;
			}
			bool result = false;
			string text = File.ReadAllText(sourceJsonPath);
			CustomAnimationData customAnimationData = default;
			customAnimationData.LoadFromJson(text);
			string customCharacterName = Path.GetFileNameWithoutExtension(sourceJsonPath);
			string sourceDirectory = Path.GetDirectoryName(sourceJsonPath);
			string sourcePngPath = Path.Combine(sourceDirectory, customCharacterName + ".png");
			if (!File.Exists(sourcePngPath))
			{
				editor.dialog.Show(RDString.Get("editor.MakeRow.customCharacter.textureNotFound").Replace("[image]", customCharacterName + ".png"), new string[1] { RDString.Get("editor.PublishLevel.dialogOK") }, (int option) =>
				{
					editor.dialog.Hide();
				});
				return false;
			}
			Vector2Int imageDimensions = RDUtils.GetImageDimensions(sourcePngPath);
			customAnimationData.CheckSize(imageDimensions.x, imageDimensions.y);
			if (customAnimationData.jsonErrors.Count > 0)
			{
				string jsonErrorsString = customAnimationData.GetJsonErrorsString(imageDimensions.x, imageDimensions.y);
				editor.dialog.Show(jsonErrorsString, new string[1] { RDString.Get("editor.PublishLevel.dialogOK") }, (int option) =>
				{
					editor.dialog.Hide();
				});
				return false;
			}
			string subfolderPath = Path.Combine(GetCurrentLevelFolderPath(), customCharacterName);
			bool sourcePathIsLevelSubFolder = sourceDirectory == subfolderPath;
			string text2 = (placeFilesInSubFolder ? Path.Combine(subfolderPath, customCharacterName) : Path.Combine(GetCurrentLevelFolderPath(), customCharacterName));
			string jsonPath = text2 + ".json";
			string pngPath = text2 + ".png";
			string outlinePath = text2 + "_outline.png";
			string glowPath = text2 + "_glow.png";
			string freezePath = text2 + "_freeze.png";
			string sourceOutlinePath = Path.Combine(sourceDirectory, customCharacterName + "_outline.png");
			string sourceGlowPath = Path.Combine(sourceDirectory, customCharacterName + "_glow.png");
			string sourceFreezePath = Path.Combine(sourceDirectory, customCharacterName + "_freeze.png");
			int num = customAnimationData.clips.Count();
			string[] voicePaths = new string[num * 2];
			string[] ccVoicePaths = new string[num * 2];
			int num2 = 0;
			foreach (string key in customAnimationData.clips.Keys)
			{
				if (num2 + 1 >= voicePaths.Length)
				{
					Debug.Log("More expressions than expected?!");
					break;
				}
				string text3 = ((key != "neutral") ? ("_" + key) : string.Empty);
				voicePaths[num2] = text2 + text3 + ".wav";
				voicePaths[num2 + 1] = text2 + text3 + ".ogg";
				try
				{
					ccVoicePaths[num2] = Path.Combine(sourceDirectory, customCharacterName + text3 + ".wav");
					ccVoicePaths[num2 + 1] = Path.Combine(sourceDirectory, customCharacterName + text3 + ".ogg");
				}
				catch (Exception ex)
				{
					string message = RDString.Get("editor.dialog.openedCCWithInvalidChar").Replace("[clipName]", key);
					string[] options = new string[1] { RDString.Get("editor.PublishLevel.dialogOK") };
					editor.dialog.Show(message, options, (int option) =>
					{
						editor.dialog.Hide();
					});
					Debug.LogError("Initialize CC error: " + ex.Message + ". " + sourceDirectory + ", " + customCharacterName + " + " + text3);
					return false;
				}
				num2 += 2;
			}
			bool flag = sourceJsonPath != jsonPath && File.Exists(jsonPath);
			bool flag2 = sourcePngPath != pngPath && File.Exists(pngPath);
			bool flag3 = File.Exists(sourceOutlinePath) && sourceOutlinePath != outlinePath && File.Exists(outlinePath);
			bool flag4 = File.Exists(sourceGlowPath) && sourceGlowPath != glowPath && File.Exists(glowPath);
			bool flag5 = File.Exists(sourceFreezePath) && sourceFreezePath != freezePath && File.Exists(freezePath);
			if (!sourcePathIsLevelSubFolder && !placeFilesInSubFolder && (flag | flag2 | flag3 | flag4 | flag5))
			{
				string empty = string.Empty;
				empty += (flag ? AddFileToString(Path.GetFileName(jsonPath), empty) : string.Empty);
				empty += (flag2 ? AddFileToString(Path.GetFileName(pngPath), empty) : string.Empty);
				empty += (flag3 ? AddFileToString(Path.GetFileName(outlinePath), empty) : string.Empty);
				empty += (flag4 ? AddFileToString(Path.GetFileName(glowPath), empty) : string.Empty);
				empty += (flag5 ? AddFileToString(Path.GetFileName(freezePath), empty) : string.Empty);
				for (int num3 = 0; num3 < voicePaths.Length; num3++)
				{
					string text4 = voicePaths[num2];
					string text5 = ccVoicePaths[num2];
					bool flag6 = File.Exists(text5) && text5 != text4 && File.Exists(text4);
					empty += (flag6 ? AddFileToString(Path.GetFileName(text4), empty) : string.Empty);
				}
				editor.dialog.Show(RDString.Get("editor.dialog.overwriteFiles").Replace("[files]", empty), new string[2]
				{
					RDString.Get("editor.dialog.overwrite"),
					RDString.Get("editor.PublishLevel.dialogCancel")
				}, (int option) =>
				{
					editor.dialog.Hide(() =>
					{
						if (option == 0)
						{
							CopyCustomCharacterFiles();
						}
					});
				});
			}
			else
			{
				CopyCustomCharacterFiles();
			}
			return result;
			static string AddFileToString(string toAdd, string text6)
			{
				if (!string.IsNullOrEmpty(text6))
				{
					return text6 + ", " + toAdd;
				}
				return toAdd;
			}
			bool CopyCustomCharacterFiles()
			{
				bool flag7 = File.Exists(sourceOutlinePath);
				bool flag8 = File.Exists(sourceGlowPath);
				bool flag9 = File.Exists(sourceFreezePath);
				if (!sourcePathIsLevelSubFolder)
				{
					try
					{
						if (placeFilesInSubFolder && !RDDirectory.Exists(subfolderPath))
						{
							RDDirectory.CreateDirectory(subfolderPath);
						}
						if (sourceJsonPath != jsonPath)
						{
							File.Copy(sourceJsonPath, jsonPath, overwrite: true);
						}
						if (sourcePngPath != pngPath)
						{
							File.Copy(sourcePngPath, pngPath, overwrite: true);
						}
						if (flag7 && sourceOutlinePath != outlinePath)
						{
							File.Copy(sourceOutlinePath, outlinePath, overwrite: true);
						}
						if (flag8 && sourceGlowPath != glowPath)
						{
							File.Copy(sourceGlowPath, glowPath, overwrite: true);
						}
						if (flag9 && sourceFreezePath != freezePath)
						{
							File.Copy(sourceFreezePath, freezePath, overwrite: true);
						}
						for (int i = 0; i < voicePaths.Length; i++)
						{
							string text6 = ccVoicePaths[i];
							string text7 = voicePaths[i];
							if (File.Exists(text6) && text6 != text7)
							{
								File.Copy(text6, text7, overwrite: true);
							}
						}
					}
					catch
					{
						editor.dialog.Show(RDString.Get("editor.MakeRow.errorCopying"), new string[1] { RDString.Get("editor.ok") }, (int a) =>
						{
							editor.dialog.Hide();
						});
						return false;
					}
				}
				if (askForSecondaryTextures && (!flag7 || !flag8 || !flag9))
				{
					editor.dialog.Show(RDString.Get("editor.MakeRow.customCharacter.secondaryTexturesNotFound"), new string[2]
					{
						RDString.Get("editor.MakeRow.customCharacter.autogenerate"),
						RDString.Get("editor.MakeRow.customCharacter.dontGenerate")
					}, (int option) =>
					{
						if (option == 0)
						{
							MakeMissingCustomCharacterTextures(sourceDirectory, customCharacterName, fromCustomCharactersFolder);
						}
						editor.dialog.Hide();
					});
				}
				result = true;
				return true;
			}
		}

		public static void MakeMissingCustomCharacterTextures(string customCharacterDirectory, string customCharacterName, bool alsoSaveToCharactersFolder = false, bool overwrite = true)
		{
			string text = Path.Combine(GetCurrentLevelFolderPath(), customCharacterName);
			bool flag = customCharacterDirectory == text;
			string obj = (flag ? Path.Combine(text, customCharacterName) : Path.Combine(GetCurrentLevelFolderPath(), customCharacterName));
			string text2 = obj + "_outline.png";
			string text3 = obj + "_glow.png";
			string text4 = obj + "_freeze.png";
			string path = Path.Combine(customCharacterDirectory, customCharacterName + ".png");
			string text5 = Path.Combine(customCharacterDirectory, customCharacterName + "_outline.png");
			string text6 = Path.Combine(customCharacterDirectory, customCharacterName + "_glow.png");
			string text7 = Path.Combine(customCharacterDirectory, customCharacterName + "_freeze.png");
			bool num = File.Exists(text5);
			bool flag2 = File.Exists(text6);
			bool flag3 = File.Exists(text7);
			byte[] data = RDFile.ReadAllBytes(path);
			Texture2D texture2D = new Texture2D(2, 2);
			texture2D.LoadImage(data);
			if (!num)
			{
				Texture2D texture2D2 = tk2dSpriteCollectionBuilderUtil.OutlineTexture(texture2D);
				texture2D2.Apply();
				tk2dSpriteCollectionBuilderUtil.SaveTextureToPNG(texture2D2, text2);
				if (alsoSaveToCharactersFolder)
				{
					RDFile.Copy(text2, text5);
				}
			}
			if (!flag2)
			{
				Texture2D texture2D3 = tk2dSpriteCollectionBuilderUtil.GaussianBlur(texture2D, 1f);
				texture2D3.Apply();
				tk2dSpriteCollectionBuilderUtil.SaveTextureToPNG(texture2D3, text3);
				if (alsoSaveToCharactersFolder)
				{
					RDFile.Copy(text3, text6);
				}
			}
			if (!flag3)
			{
				object obj2;
				if (!(flag | alsoSaveToCharactersFolder))
				{
					obj2 = "";
				}
				else
				{
					char directorySeparatorChar = Path.DirectorySeparatorChar;
					obj2 = customCharacterName + directorySeparatorChar;
				}
				string text8 = (string)obj2;
				Vector2Int spriteSizeFromJson = CustomAnimationData.GetSpriteSizeFromJson(LoadLevelText(LevelSource.ExternalPath, text8 + customCharacterName + ".json", "", alsoSaveToCharactersFolder));
				Texture2D texture2D4 = tk2dSpriteCollectionBuilderUtil.FreezeTexture(texture2D, spriteSizeFromJson);
				texture2D4.Apply();
				tk2dSpriteCollectionBuilderUtil.SaveTextureToPNG(texture2D4, text4);
				if (alsoSaveToCharactersFolder)
				{
					RDFile.Copy(text4, text7);
				}
			}
		}
	}
}
