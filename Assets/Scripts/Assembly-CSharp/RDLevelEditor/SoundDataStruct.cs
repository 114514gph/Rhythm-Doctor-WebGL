using System;
using System.Collections.Generic;
using UnityEngine;

namespace RDLevelEditor
{
	public readonly struct SoundDataStruct
	{
		private const string filenameKey = "filename";

		private const string volumeKey = "volume";

		private const string panKey = "pan";

		private const string pitchKey = "pitch";

		private const string offsetKey = "offset";

		private const string usedKey = "used";

		public readonly string filename;

		public readonly int volume;

		public readonly int pitch;

		public readonly int pan;

		public readonly int offset;

		public readonly bool used;

		private const int defaultVolume = 100;

		private const int defaultOffset = 0;

		private const int defaultPitch = 100;

		private const int defaultPan = 0;

		private const bool defaultUsed = true;

		public float volumePercentage => (float)volume * 0.01f;

		public float pitchPercentage => (float)pitch * 0.01f;

		public float panPercentage => (float)pan * 0.01f;

		public float offsetInSeconds => (float)offset * 0.001f;

		public SoundDataStruct(string filename, int volume = 100, int pitch = 100, int pan = 0, int offset = 0, bool used = true)
		{
			this.filename = filename;
			this.volume = volume;
			this.pitch = pitch;
			this.pan = pan;
			this.offset = offset;
			this.used = used;
		}

		public static SoundDataStruct Decode(IReadOnlyDictionary<string, object> dict)
		{
			string text = (string)dict["filename"];
			int num = (dict.TryGetValue("offset", out var value) ? Convert.ToInt32(value) : 0);
			int num2 = (dict.TryGetValue("volume", out var value2) ? Convert.ToInt32(value2) : 100);
			int num3 = (dict.TryGetValue("pitch", out var value3) ? Convert.ToInt32(value3) : 100);
			int num4 = (dict.TryGetValue("pan", out var value4) ? Convert.ToInt32(value4) : 0);
			bool flag = !dict.TryGetValue("used", out var value5) || Convert.ToBoolean(value5);
			if (RDLevelData.current.settings.version <= 9 && text.HasAudioFileExtension())
			{
				num2 = Mathf.FloorToInt((float)num2 / 0.4f);
			}
			if (RDLevelData.current.settings.version <= 42)
			{
				if (text == "Stick")
				{
					text = "StickOld";
					Debug.Log("Pre v43 level, adjusted Stick beatsound to legacy version!");
				}
				if (text == "ClosedHat")
				{
					text = "ClosedHatOld";
					Debug.Log("Pre v43 level, adjusted ClosedHat beatsound to legacy version!");
				}
			}
			return new SoundDataStruct(text, num2, num3, num4, num, flag);
		}

		public static void MigrateLegacySound(Dictionary<string, object> dict, string newKey, string oldKeyPrefix = "")
		{
			string key = "filename";
			string key2 = "offset";
			string key3 = "volume";
			string key4 = "pitch";
			string key5 = "pan";
			string text = "sound";
			if (oldKeyPrefix != "")
			{
				key = oldKeyPrefix + "Filename";
				text = oldKeyPrefix + "Sound";
				key2 = oldKeyPrefix + "Offset";
				key3 = oldKeyPrefix + "Volume";
				key4 = oldKeyPrefix + "Pitch";
				key5 = oldKeyPrefix + "Pan";
			}
			if (!dict.ContainsKey(key) && dict.ContainsKey(text) && dict[text] is string)
			{
				key = text;
			}
			if (dict.ContainsKey(key))
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				dictionary["filename"] = (string)dict[key];
				if (dict.TryGetValue(key2, out var value))
				{
					dictionary["offset"] = Convert.ToInt32(value);
				}
				if (dict.TryGetValue(key3, out var value2))
				{
					dictionary["volume"] = Convert.ToInt32(value2);
				}
				if (dict.TryGetValue(key4, out var value3))
				{
					dictionary["pitch"] = Convert.ToInt32(value3);
				}
				if (dict.TryGetValue(key5, out var value4))
				{
					dictionary["pan"] = Convert.ToInt32(value4);
				}
				dict[newKey] = dictionary;
			}
		}

		public string Encode(string key, bool isLastValue = false)
		{
			string text = "{";
			text += RDEditorUtils.EncodeString("filename", filename);
			if (volume != 100)
			{
				text += RDEditorUtils.EncodeInt("volume", volume);
			}
			if (pitch != 100)
			{
				text += RDEditorUtils.EncodeInt("pitch", pitch);
			}
			if (pan != 0)
			{
				text += RDEditorUtils.EncodeInt("pan", pan);
			}
			if (offset != 0)
			{
				text += RDEditorUtils.EncodeInt("offset", offset);
			}
			if (!used)
			{
				text += RDEditorUtils.EncodeBool("used", used);
			}
			string text2 = text;
			text = text2.Substring(0, text2.Length - 2);
			text += "}";
			return RDEditorUtils.EncodeKeyValue(key, text, isLastValue);
		}

		public SoundData ToSoundData()
		{
			return new SoundData(filename)
			{
				volume = volume,
				pitch = pitch,
				pan = pan,
				offset = offset,
				minPitch = pitch,
				maxPitch = pitch,
				used = used
			};
		}

		public static SoundDataStruct FromSoundData(SoundData soundData)
		{
			return new SoundDataStruct(soundData.filename, soundData.volume, soundData.pitch, soundData.pan, soundData.offset, soundData.used);
		}

		public SoundDataStruct Validated()
		{
			return new SoundDataStruct(filename, Mathf.Clamp(volume, 0, 300), Mathf.Clamp(pitch, 0, 300), Mathf.Clamp(pan, -100, 100), offset, used);
		}

		public SoundDataStruct WithNewVolume(int newVolume)
		{
			return new SoundDataStruct(filename, newVolume, pitch, pan, offset, used);
		}

		public SoundDataStruct WithNewOffset(int newOffset)
		{
			return new SoundDataStruct(filename, volume, pitch, pan, newOffset, used);
		}

		public SoundDataStruct WithNewFilename(string newFilename)
		{
			return new SoundDataStruct(newFilename, volume, pitch, pan, offset, used);
		}

		public SoundDataStruct WithNewUsed(bool newUsed)
		{
			return new SoundDataStruct(filename, volume, pitch, pan, offset, newUsed);
		}

		public bool IsExternalClip()
		{
			return RDEditorUtils.FindClip(filename) == RDAudioLoadType.SuccessExternalClipLoaded;
		}

		public string LocalizedName()
		{
			if (Enum.TryParse<SoundEffect>(filename, out var result))
			{
				return RDString.GetEnumValue(result);
			}
			return filename;
		}

		public string GetTooltip()
		{
			if (pitch == 100 && pan == 0)
			{
				int num = volume;
				return num + "%";
			}
			string text = string.Format("{0} {1}%", RDString.Get("editor.volume"), volume);
			if (pitch != 100)
			{
				text += string.Format("\n{0} {1}%", RDString.Get("editor.pitch"), pitch);
			}
			if (pan != 0)
			{
				text += string.Format("\n{0} {1}%", RDString.Get("editor.pan"), pan);
			}
			return text;
		}
	}
}
