using System;
using System.Collections;
using System.Collections.Generic;
using RDTools;
using UnityEngine;

namespace RDLevelEditor
{
	public class SoundData : RDClass
	{
		public string filename;

		public int volume;

		public int pitch;

		public int minPitch;

		public int maxPitch;

		public int pan;

		public int offset;

		public bool externalClip;

		public bool loadSuccessful;

		public GameSoundType groupSubtype;

		public bool used;

		private readonly string filenameKey;

		private readonly string volumeKey;

		private readonly string pitchKey;

		private readonly string panKey;

		private readonly string offsetKey;

		private readonly string groupSubtypeKey;

		private readonly string usedKey;

		private const int defaultVolume = 100;

		private const int defaultOffset = 0;

		private const int defaultPitch = 100;

		private const int defaultPan = 0;

		public const GameSoundType notInGroup = GameSoundType.ClapSoundP1Classic;

		public float volumePercentage
		{
			get
			{
				return (float)volume * 0.01f;
			}
			set
			{
				volume = (int)(value * 100f);
			}
		}

		public float pitchPercentage
		{
			get
			{
				return (float)pitch * 0.01f;
			}
			set
			{
				pitch = (int)(value * 100f);
			}
		}

		public float panPercentage
		{
			get
			{
				return (float)pan * 0.01f;
			}
			set
			{
				pan = (int)(value * 100f);
			}
		}

		public float offsetInSeconds
		{
			get
			{
				return (float)offset * 0.001f;
			}
			set
			{
				offset = (int)(value * 1000f);
			}
		}

		public float minPitchPercentage
		{
			get
			{
				return (float)minPitch * 0.01f;
			}
			set
			{
				minPitch = (int)(value * 100f);
			}
		}

		public float maxPitchPercentage
		{
			get
			{
				return (float)maxPitch * 0.01f;
			}
			set
			{
				maxPitch = (int)(value * 100f);
			}
		}

		public string conductorFilename
		{
			get
			{
				if (!externalClip)
				{
					return filename;
				}
				return filename + "*external";
			}
		}

		public SoundData()
		{
			volume = 100;
			offset = 0;
			pitch = 100;
			minPitch = 100;
			maxPitch = 100;
			pan = 0;
			groupSubtype = GameSoundType.ClapSoundP1Classic;
			used = true;
			externalClip = false;
		}

		public SoundData(string filename, string filenameKey = "filename", string volumeKey = "volume", string pitchKey = "pitch", string panKey = "pan", string offsetKey = "offset", string groupSubtypeKey = "groupSubtype", string usedKey = "used")
			: this()
		{
			this.filename = filename;
			this.filenameKey = filenameKey;
			this.volumeKey = volumeKey;
			this.offsetKey = offsetKey;
			this.pitchKey = pitchKey;
			this.panKey = panKey;
			this.groupSubtypeKey = groupSubtypeKey;
			this.usedKey = usedKey;
		}

		public IEnumerator Prepare()
		{
			loadSuccessful = false;
			LevelBase level = scnGame.instance.currentLevel;
			bool flag = false;
			if (level == null)
			{
				Debug.LogError("level is null");
			}
			if (level.preparedAudioClips == null)
			{
				Debug.LogError("preparedaudioclips is null");
			}
			if (!used || string.IsNullOrEmpty(filename))
			{
				yield break;
			}
			if (level.preparedAudioClips.ContainsKey(filename))
			{
				flag = true;
			}
			if (level.preparedAudioClips.ContainsKey(filename + "*external"))
			{
				externalClip = true;
				flag = true;
			}
			if (flag)
			{
				yield break;
			}
			switch (RDEditorUtils.FindClip(filename))
			{
			case RDAudioLoadType.SuccessExternalClipLoaded:
			{
				yield return RDEditorUtils.AudioClipFromFilename(filename);
				externalClip = true;
				loadSuccessful = true;
				string text2 = conductorFilename;
				Dictionary<string, AudioClip> audioLib = Singleton<AudioManager>.Instance.audioLib;
				if (!audioLib.ContainsKey(text2))
				{
					Debug.LogWarning("Trying to find sound in library but was not found! " + text2);
					break;
				}
				AudioClip value = audioLib[text2];
				if (!level.preparedAudioClips.ContainsKey(text2))
				{
					level.preparedAudioClips.Add(text2, value);
				}
				SongOffset songOffset2 = new SongOffset();
				songOffset2.offset = offsetInSeconds;
				songOffset2.volume = 1f;
				scrConductor.instance.externalSoundData[text2] = songOffset2;
				break;
			}
			case RDAudioLoadType.SuccessInternalClipLoaded:
			{
				externalClip = false;
				loadSuccessful = true;
				string text = conductorFilename;
				Dictionary<string, SongOffset> externalSoundData = scrConductor.instance.externalSoundData;
				if (scrConductor.songOffsets.Get(text).dummy && !externalSoundData.ContainsKey(text))
				{
					SongOffset songOffset = new SongOffset();
					songOffset.offset = offsetInSeconds;
					songOffset.volume = 1f;
					songOffset.isInternal = true;
					externalSoundData[text] = songOffset;
				}
				break;
			}
			default:
				externalClip = false;
				Debug.LogWarning("Couldn't find file " + filename);
				Debug.Log("here D");
				break;
			}
		}

		public void Validate()
		{
			volume = Mathf.Clamp(volume, 0, 300);
			minPitch = (maxPitch = (pitch = Mathf.Clamp(pitch, 0, 300)));
			pan = Mathf.Clamp(pan, -100, 100);
		}

		public void Decode(Dictionary<string, object> dict)
		{
			offset = (dict.TryGetValue(offsetKey, out var value) ? Convert.ToInt32(value) : 0);
			volume = (dict.TryGetValue(volumeKey, out var value2) ? Convert.ToInt32(value2) : 100);
			pitch = (dict.TryGetValue(pitchKey, out var value3) ? Convert.ToInt32(value3) : 100);
			pan = (dict.TryGetValue(panKey, out var value4) ? Convert.ToInt32(value4) : 0);
			filename = (dict.TryGetValue(filenameKey, out var value5) ? (value5 as string) : "");
			groupSubtype = (dict.TryGetValue(groupSubtypeKey, out var value6) ? Enum.Parse<GameSoundType>(value6 as string) : GameSoundType.ClapSoundP1Classic);
			used = !dict.TryGetValue(usedKey, out var value7) || (bool)value7;
			minPitch = (maxPitch = pitch);
			if (RDLevelData.current.settings.version <= 9 && filename.HasAudioFileExtension())
			{
				volume = Mathf.FloorToInt((float)volume / 0.4f);
			}
			if (RDLevelData.current.settings.version <= 42)
			{
				if (filename == "Stick")
				{
					filename = "StickOld";
					RDClassDll.printem("Pre v43 level, adjusted Stick beatsound to legacy version!");
				}
				if (filename == "ClosedHat")
				{
					filename = "ClosedHatOld";
					RDClassDll.printem("Pre v43 level, adjusted ClosedHat beatsound to legacy version!");
				}
			}
		}

		public string Encode(bool isLastValue = false)
		{
			string text = RDEditorUtils.EncodeString(filenameKey, filename);
			if (volume != 100)
			{
				text += RDEditorUtils.EncodeInt(volumeKey, volume);
			}
			if (pitch != 100)
			{
				text += RDEditorUtils.EncodeInt(pitchKey, pitch);
			}
			if (pan != 0)
			{
				text += RDEditorUtils.EncodeInt(panKey, pan);
			}
			if (offset != 0)
			{
				text += RDEditorUtils.EncodeInt(offsetKey, offset);
			}
			text = text.Substring(0, text.Length - 2);
			if (groupSubtype != GameSoundType.ClapSoundP1Classic)
			{
				return RDEditorUtils.EncodeString(groupSubtypeKey, groupSubtype.ToString()) + RDEditorUtils.EncodeBool(usedKey, used, !used & isLastValue) + (used ? text : "");
			}
			return text;
		}

		public void CopyFrom(SoundData another)
		{
			filename = another.filename;
			offset = another.offset;
			volume = another.volume;
			pitch = another.pitch;
			minPitch = another.minPitch;
			maxPitch = another.maxPitch;
			pan = another.pan;
			groupSubtype = another.groupSubtype;
			used = another.used;
		}

		public void PlayImmediately(string mixerPath = "GameplaySoundsParent")
		{
			IsExternalClip();
			AudioSource item = scrConductor.PlayImmediately(conductorFilename, volumePercentage, RDUtils.GetMixerGroup(mixerPath), pitchPercentage, panPercentage, isBeatSound: false, ignoreListenerPause: true);
			scnEditor.instance.soundSettingsPopup.audioSources.Add(item);
		}

		public bool IsExternalClip()
		{
			RDClassDll.printem("filename: " + (filename ?? "is null!"));
			RDAudioLoadType rDAudioLoadType = RDEditorUtils.FindClip(filename);
			externalClip = rDAudioLoadType == RDAudioLoadType.SuccessExternalClipLoaded;
			return externalClip;
		}
	}
}
