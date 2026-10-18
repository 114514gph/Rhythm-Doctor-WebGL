using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public struct CustomAnimationData
{
	public enum JsonError
	{
		FailedDeserializing = 0,
		NoSizeKey = 1,
		WrongSizeLength = 2,
		WrongSizeX = 13,
		WrongSizeY = 14,
		NoClipsKey = 12,
		WrongClipsLength = 3,
		NoHappyAnim = 4,
		NoMissedAnim = 5,
		NoBarelyAnim = 6,
		NoNeutralAnim = 7,
		NoNameKey = 8,
		NoFramesKey = 9,
		NoLoopKey = 10,
		NoFpsKey = 11
	}

	public Texture2D mainTexture;

	public Texture2D outlineTexture;

	public Texture2D glowTexture;

	public Texture2D freezeTexture;

	public string name;

	public Vector2Int spriteSize;

	public Dictionary<string, CustomAnimationClip> clips;

	public List<JsonError> jsonErrors;

	public bool hasPortraitData;

	public int rowPreviewFrame;

	public float rowPreviewScale;

	public Vector2 rowPreviewOffset;

	public string voice;

	private int frameCount;

	public void Setup(string jsonText, Texture2D mainTexture, Texture2D outlineTexture, Texture2D glowTexture, Texture2D freezeTexture)
	{
		this.mainTexture = mainTexture;
		DateTime utcNow = DateTime.UtcNow;
		if ((object)outlineTexture == null)
		{
			outlineTexture = Texture2D.blackTexture;
		}
		if ((object)glowTexture == null)
		{
			glowTexture = Texture2D.blackTexture;
		}
		if ((object)freezeTexture == null)
		{
			freezeTexture = Texture2D.whiteTexture;
		}
		_ = (DateTime.UtcNow - utcNow).TotalSeconds;
		this.outlineTexture = outlineTexture;
		this.glowTexture = glowTexture;
		outlineTexture.filterMode = FilterMode.Point;
		this.freezeTexture = freezeTexture;
		this.freezeTexture.filterMode = FilterMode.Point;
		LoadFromJson(jsonText);
	}

	public void LoadFromJson(string text)
	{
		jsonErrors = new List<JsonError>();
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		try
		{
			dictionary = Json.Deserialize(text) as Dictionary<string, object>;
		}
		catch (Exception ex)
		{
			jsonErrors.Add(JsonError.FailedDeserializing);
			RDBaseDllDummy.printem("Error deserializing json: " + ex.Message);
			return;
		}
		if (dictionary == null)
		{
			jsonErrors.Add(JsonError.FailedDeserializing);
			return;
		}
		if (dictionary.ContainsKey("name"))
		{
			name = dictionary["name"] as string;
		}
		else
		{
			name = "";
		}
		if (dictionary.ContainsKey("voice"))
		{
			voice = dictionary["voice"] as string;
		}
		else
		{
			voice = "";
		}
		if (dictionary.ContainsKey("size"))
		{
			List<object> list = dictionary["size"] as List<object>;
			if (list.Count != 2)
			{
				jsonErrors.Add(JsonError.WrongSizeLength);
			}
			else
			{
				int num = (int)list[0];
				int num2 = (int)list[1];
				int x = ((num == 0 && mainTexture != null) ? mainTexture.width : num);
				int y = ((num2 == 0 && mainTexture != null) ? mainTexture.height : num2);
				spriteSize = new Vector2Int(x, y);
			}
		}
		else
		{
			jsonErrors.Add(JsonError.NoSizeKey);
		}
		clips = new Dictionary<string, CustomAnimationClip>(StringComparer.OrdinalIgnoreCase);
		if (!dictionary.ContainsKey("clips"))
		{
			jsonErrors.Add(JsonError.NoClipsKey);
			return;
		}
		bool flag = false;
		List<object> obj = dictionary["clips"] as List<object>;
		bool flag2 = false;
		if (dictionary.ContainsKey("rowPreviewOffset"))
		{
			List<object> list2 = dictionary["rowPreviewOffset"] as List<object>;
			if (list2.Count == 2)
			{
				rowPreviewOffset = new Vector2(Convert.ToSingle(list2[0]), Convert.ToSingle(list2[1]));
				flag2 = true;
			}
		}
		if (dictionary.ContainsKey("rowPreviewFrame"))
		{
			rowPreviewFrame = Convert.ToInt32(dictionary["rowPreviewFrame"]);
		}
		else
		{
			rowPreviewFrame = -1;
		}
		if (dictionary.ContainsKey("rowPreviewScale"))
		{
			rowPreviewScale = Mathf.Abs(Convert.ToSingle(dictionary["rowPreviewScale"]));
		}
		else
		{
			rowPreviewScale = (flag2 ? 1 : 0);
		}
		Vector2 pivotOffset = default;
		float reflectionOffset = (float)spriteSize.y * 0.5f;
		Vector2 portraitOffset = default;
		Vector2Int portraitSize = default;
		float portraitScale = 0f;
		if (dictionary.ContainsKey("pivotOffset"))
		{
			List<object> list3 = dictionary["pivotOffset"] as List<object>;
			if (list3.Count != 2)
			{
				jsonErrors.Add(JsonError.WrongSizeLength);
			}
			else
			{
				pivotOffset = new Vector2(Convert.ToSingle(list3[0]), Convert.ToSingle(list3[1]));
			}
		}
		if (dictionary.ContainsKey("reflectionOffset"))
		{
			reflectionOffset = Convert.ToSingle(dictionary["reflectionOffset"]);
		}
		if (dictionary.ContainsKey("portraitOffset"))
		{
			List<object> list4 = dictionary["portraitOffset"] as List<object>;
			if (list4.Count != 2)
			{
				jsonErrors.Add(JsonError.WrongSizeLength);
			}
			else
			{
				portraitOffset = new Vector2(Convert.ToSingle(list4[0]), Convert.ToSingle(list4[1]));
			}
			hasPortraitData = true;
		}
		if (dictionary.ContainsKey("portraitSize"))
		{
			List<object> list5 = dictionary["portraitSize"] as List<object>;
			if (list5.Count == 2)
			{
				portraitSize = new Vector2Int((int)Convert.ToSingle(list5[0]), (int)Convert.ToSingle(list5[1]));
			}
			hasPortraitData = true;
		}
		if (dictionary.ContainsKey("portraitScale"))
		{
			portraitScale = Convert.ToSingle(dictionary["portraitScale"]);
			hasPortraitData = true;
		}
		foreach (object item in obj)
		{
			Dictionary<string, object> dictionary2 = item as Dictionary<string, object>;
			CustomAnimationClip customAnimationClip = new CustomAnimationClip();
			if (dictionary2.ContainsKey("name"))
			{
				customAnimationClip.name = dictionary2["name"] as string;
				if (customAnimationClip.name == "neutral")
				{
					flag = true;
				}
			}
			else
			{
				jsonErrors.Add(JsonError.NoNameKey);
			}
			if (dictionary2.ContainsKey("frames"))
			{
				List<object> list6 = dictionary2["frames"] as List<object>;
				customAnimationClip.frames = new int[list6.Count];
				int num3 = 0;
				foreach (object item2 in list6)
				{
					customAnimationClip.frames[num3] = (int)item2;
					num3++;
				}
			}
			else
			{
				jsonErrors.Add(JsonError.NoFramesKey);
			}
			customAnimationClip.frameEvents = new Dictionary<int, Dictionary<AnimationEvent, string>>();
			if (dictionary2.TryGetValueAs<string, object, Dictionary<string, object>>("events", out var valueAs))
			{
				foreach (KeyValuePair<string, object> item3 in valueAs)
				{
					if (!int.TryParse(item3.Key, out var result) || result >= customAnimationClip.frames.Length)
					{
						continue;
					}
					if (result < 0)
					{
						result = -1;
					}
					if (item3.Value is string eventData)
					{
						customAnimationClip.AddLegacyFrameEvent(result, eventData);
					}
					else if (item3.Value is Dictionary<string, object> dictionary3)
					{
						foreach (KeyValuePair<string, object> item4 in dictionary3)
						{
							if (item4.Value is string value)
							{
								customAnimationClip.AddFrameEvent(result, item4.Key, value);
							}
						}
					}
					else
					{
						Debug.Log(item3.Value);
					}
				}
			}
			if (dictionary2.TryGetValueAs<string, object, string>("endEvent", out var valueAs2))
			{
				customAnimationClip.AddLegacyFrameEvent(-1, valueAs2);
			}
			if (dictionary2.ContainsKey("loopStart"))
			{
				int num4 = (int)dictionary2["loopStart"];
				if (num4 < 0)
				{
					num4 = customAnimationClip.frames.Length + num4;
				}
				customAnimationClip.loopStart = Mathf.Clamp(num4, 0, customAnimationClip.frames.Length - 1);
			}
			if (dictionary2.ContainsKey("loop"))
			{
				string text2 = dictionary2["loop"] as string;
				switch (text2)
				{
				case "yes":
					customAnimationClip.type = AnimationType.Loop;
					break;
				case "freeze":
					customAnimationClip.type = AnimationType.Loop;
					customAnimationClip.loopStart = customAnimationClip.frames.Length - 1;
					break;
				case "onBeat":
					customAnimationClip.type = AnimationType.LoopOnBeat;
					break;
				case "no":
					customAnimationClip.type = AnimationType.Once;
					break;
				default:
					Debug.LogWarning("Unknown loop type '" + text2 + "' in " + name + " clip '" + customAnimationClip.name + "', defaulting to 'no'");
					customAnimationClip.type = AnimationType.Once;
					break;
				}
			}
			else
			{
				jsonErrors.Add(JsonError.NoLoopKey);
			}
			if (dictionary2.ContainsKey("fps"))
			{
				customAnimationClip.fps = Convert.ToSingle(dictionary2["fps"]);
			}
			else
			{
				customAnimationClip.fps = 0f;
			}
			if (dictionary2.ContainsKey("pivotOffset"))
			{
				List<object> list7 = dictionary2["pivotOffset"] as List<object>;
				if (list7.Count != 2)
				{
					jsonErrors.Add(JsonError.WrongSizeLength);
				}
				else
				{
					customAnimationClip.pivotOffset = new Vector2(Convert.ToSingle(list7[0]), Convert.ToSingle(list7[1]));
				}
			}
			else
			{
				customAnimationClip.pivotOffset = pivotOffset;
			}
			if (dictionary2.ContainsKey("reflectionOffset"))
			{
				customAnimationClip.reflectionOffset = Convert.ToSingle(dictionary2["reflectionOffset"]);
			}
			else
			{
				customAnimationClip.reflectionOffset = reflectionOffset;
			}
			if (dictionary2.ContainsKey("portraitOffset"))
			{
				List<object> list8 = dictionary2["portraitOffset"] as List<object>;
				if (list8.Count != 2)
				{
					jsonErrors.Add(JsonError.WrongSizeLength);
				}
				else
				{
					customAnimationClip.portraitOffset = new Vector2(Convert.ToSingle(list8[0]), Convert.ToSingle(list8[1]));
				}
				hasPortraitData = true;
			}
			else if (hasPortraitData)
			{
				customAnimationClip.portraitOffset = portraitOffset;
			}
			if (dictionary2.ContainsKey("portraitSize"))
			{
				List<object> list9 = dictionary2["portraitSize"] as List<object>;
				if (list9.Count == 2)
				{
					customAnimationClip.portraitSize = new Vector2Int((int)Convert.ToSingle(list9[0]), (int)Convert.ToSingle(list9[1]));
				}
				hasPortraitData = true;
			}
			else if (hasPortraitData)
			{
				customAnimationClip.portraitSize = portraitSize;
			}
			if (dictionary2.ContainsKey("portraitName") && dictionary2["portraitName"] is string portraitName)
			{
				customAnimationClip.portraitName = portraitName;
			}
			if (customAnimationClip.portraitSize.x < 1 || customAnimationClip.portraitSize.y < 1)
			{
				customAnimationClip.portraitSize = new Vector2Int(25, 30);
			}
			if (dictionary2.ContainsKey("portraitScale"))
			{
				customAnimationClip.portraitScale = Convert.ToSingle(dictionary2["portraitScale"]);
				hasPortraitData = true;
			}
			else if (hasPortraitData)
			{
				customAnimationClip.portraitScale = portraitScale;
			}
			if (customAnimationClip.portraitScale <= 0f)
			{
				customAnimationClip.portraitScale = 2f;
			}
			if (!clips.ContainsKey(customAnimationClip.name))
			{
				clips.Add(customAnimationClip.name, customAnimationClip);
				if (rowPreviewFrame == -1 && customAnimationClip.name == "neutral" && customAnimationClip.frames.Length != 0)
				{
					rowPreviewFrame = customAnimationClip.frames.Last();
				}
			}
		}
		if (!flag)
		{
			jsonErrors.Add(JsonError.NoNeutralAnim);
		}
		if (rowPreviewFrame < 0)
		{
			rowPreviewFrame = 0;
		}
	}

	public void CheckSize(int width, int height)
	{
		if (spriteSize.x < 0 || spriteSize.x > width)
		{
			jsonErrors.Add(JsonError.WrongSizeX);
		}
		else if (spriteSize.y < 0 || spriteSize.y > height)
		{
			jsonErrors.Add(JsonError.WrongSizeY);
		}
	}

	public string GetJsonErrorsString(int textureWidth = 0, int textureHeight = 0)
	{
		string text = RDString.Get("editor.MakeRow.customCharacter.failedLoading") + ":";
		foreach (JsonError jsonError in jsonErrors)
		{
			text += "\n";
			switch (jsonError)
			{
			case JsonError.FailedDeserializing:
				text += RDString.Get("editor.MakeRow.customCharacter.failedDeserializing");
				break;
			case JsonError.NoSizeKey:
				text += RDString.Get("editor.MakeRow.customCharacter.missingKey").Replace("[key]", "size");
				break;
			case JsonError.NoClipsKey:
				text += RDString.Get("editor.MakeRow.customCharacter.missingKey").Replace("[key]", "clips");
				break;
			case JsonError.NoNameKey:
				text += RDString.Get("editor.MakeRow.customCharacter.missingKey").Replace("[key]", "name");
				break;
			case JsonError.NoFramesKey:
				text += RDString.Get("editor.MakeRow.customCharacter.missingKey").Replace("[key]", "frames");
				break;
			case JsonError.NoLoopKey:
				text += RDString.Get("editor.MakeRow.customCharacter.missingKey").Replace("[key]", "loop");
				break;
			case JsonError.NoFpsKey:
				text += RDString.Get("editor.MakeRow.customCharacter.missingKey").Replace("[key]", "fps");
				break;
			case JsonError.WrongSizeLength:
				text += RDString.Get("editor.MakeRow.customCharacter.wrongKeyLength").Replace("[key]", "size").Replace("[length]", "2");
				break;
			case JsonError.WrongClipsLength:
				text += RDString.Get("editor.MakeRow.customCharacter.wrongKeyLength").Replace("[key]", "clips").Replace("[length]", "4");
				break;
			case JsonError.WrongSizeX:
				text += RDString.Get("editor.MakeRow.customCharacter.wrongSizeWidth").Replace("[size]", spriteSize.x.ToString()).Replace("[textureSize]", textureWidth.ToString());
				break;
			case JsonError.WrongSizeY:
				text += RDString.Get("editor.MakeRow.customCharacter.wrongSizeHeight").Replace("[size]", spriteSize.y.ToString()).Replace("[textureSize]", textureHeight.ToString());
				break;
			case JsonError.NoHappyAnim:
				text += RDString.Get("editor.MakeRow.customCharacter.missingAnim").Replace("[name]", "happy");
				break;
			case JsonError.NoMissedAnim:
				text += RDString.Get("editor.MakeRow.customCharacter.missingAnim").Replace("[name]", "missed");
				break;
			case JsonError.NoNeutralAnim:
				text += RDString.Get("editor.MakeRow.customCharacter.missingAnim").Replace("[name]", "neutral");
				break;
			case JsonError.NoBarelyAnim:
				text += RDString.Get("editor.MakeRow.customCharacter.missingAnim").Replace("[name]", "barely");
				break;
			}
		}
		return text;
	}

	public List<Vector2> GetUVsForSheetFrame(int sheetFrame)
	{
		if (mainTexture == null || mainTexture.width == 0 || mainTexture.height == 0)
		{
			object arg = mainTexture == null;
			Texture2D texture2D = mainTexture;
			object arg2 = (object)texture2D != null && texture2D.width == 0;
			Texture2D texture2D2 = mainTexture;
			Debug.LogError($"UVs warning: {arg}, {arg2} || {(object)texture2D2 != null && texture2D2.height == 0}");
			return null;
		}
		Texture2D texture2D3 = mainTexture;
		int num = texture2D3.width / spriteSize.x;
		float num2 = (float)spriteSize.x * 1f / (float)texture2D3.width;
		float num3 = (float)spriteSize.y * 1f / (float)texture2D3.height;
		int num4 = sheetFrame % num;
		int num5 = sheetFrame / num;
		float num6 = num2 * (float)num4;
		float num7 = num3 * (float)num5;
		float num8 = num6;
		float num9 = 1f - num7 - num3;
		return new List<Vector2>(4)
		{
			new Vector2(num8, num9),
			new Vector2(num8 + num2, num9),
			new Vector2(num8, num9 + num3),
			new Vector2(num8 + num2, num9 + num3)
		};
	}

	public static Vector2Int GetSpriteSizeFromJson(string jsonText)
	{
		Vector2Int result = Vector2Int.zero;
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		try
		{
			if (Json.Deserialize(jsonText) is Dictionary<string, object> dictionary2 && dictionary2.ContainsKey("size"))
			{
				List<object> list = dictionary2["size"] as List<object>;
				if (list.Count == 2)
				{
					result = new Vector2Int((int)list[0], (int)list[1]);
				}
			}
		}
		catch (Exception ex)
		{
			RDBaseDllDummy.printem("Error getting sprite size: " + ex.Message);
		}
		return result;
	}
}
