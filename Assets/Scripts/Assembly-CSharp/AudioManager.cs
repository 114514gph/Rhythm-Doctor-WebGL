using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using MP3Sharp;
using RDLevelEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Networking;

public class AudioManager : Singleton<AudioManager>
{
	public Dictionary<string, AudioClip> audioLib;

	public Dictionary<string, MP3Stream> mp3Streams;

	public Dictionary<string, int> mp3Samples;

	public List<RDMP3Stream> rdMP3Streams;

	public List<AudioSource> liveSources;

	public GameObject audioSourcePrefab;

	public List<AudioSource> musicTracks;

	public List<bool> tracksActive;

	public float maxMusicVol = 0.4f;

	public bool appPaused;

	[Header("Note: This variable is for display only")]
	public int audioClipsLoaded;

	private AudioMixer _mixer;

	private Dictionary<string, AudioMixerGroup> mixerGroups;

	private AudioMixerGroup fallbackMixerGroup;

	public Dictionary<string, string> loadingAudioClips = new Dictionary<string, string>();

	public AudioMixer Mixer
	{
		get
		{
			if (_mixer == null)
			{
				_mixer = Resources.Load<AudioMixer>("MasterMixer");
			}
			return _mixer;
		}
	}

	public override void Awake()
	{
		rdMP3Streams = new List<RDMP3Stream>();
		if (audioLib == null)
		{
			audioLib = new Dictionary<string, AudioClip>();
		}
		if (mp3Streams == null)
		{
			mp3Streams = new Dictionary<string, MP3Stream>();
		}
		if (mp3Samples == null)
		{
			mp3Samples = new Dictionary<string, int>();
		}
		if (audioSourcePrefab == null)
		{
			audioSourcePrefab = Resources.Load<GameObject>("Audio Source");
		}
		liveSources = new List<AudioSource>();
		musicTracks = new List<AudioSource>();
		tracksActive = new List<bool>();
		mixerGroups = new Dictionary<string, AudioMixerGroup>();
		AudioMixerGroup[] array = Mixer.FindMatchingGroups("");
		foreach (AudioMixerGroup audioMixerGroup in array)
		{
			mixerGroups.Add(audioMixerGroup.name, audioMixerGroup);
		}
		fallbackMixerGroup = mixerGroups["Fallback"];
	}

	public AudioMixerGroup GetMixerGroup(string group)
	{
		if (mixerGroups.ContainsKey(group))
		{
			return mixerGroups[group];
		}
		Debug.LogWarning("AudioMixerGroup not found: " + group + "!");
		return null;
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		appPaused = pauseStatus;
	}

	public void Update()
	{
		if (appPaused || AudioListener.pause)
		{
			return;
		}
		for (int i = 0; i < liveSources.Count; i++)
		{
			AudioSource audioSource = liveSources[i];
			if (audioSource == null)
			{
				RemoveLiveSource(i);
			}
			else if (!audioSource.isPlaying)
			{
				UnityEngine.Object.Destroy(audioSource.gameObject, 3.5f);
				RemoveLiveSource(i);
			}
		}
		for (int j = 0; j < musicTracks.Count; j++)
		{
			if (musicTracks[j] == null)
			{
				musicTracks.RemoveAt(j);
			}
			else if (!musicTracks[j].isPlaying)
			{
				UnityEngine.Object.Destroy(musicTracks[j].gameObject, 3.5f);
				tracksActive.RemoveAt(j);
				musicTracks.RemoveAt(j);
			}
		}
		for (int k = 0; k < musicTracks.Count; k++)
		{
			Debug.Log("thawuaewhi");
			CrossFadeAudioSource(musicTracks[k], tracksActive[k]);
		}
	}

	public void SetTrackActivity(int trackNum, bool newState)
	{
		if (tracksActive.Count > trackNum)
		{
			tracksActive[trackNum] = newState;
		}
		else
		{
			Debug.LogWarning("[AudioManager]\n\tTried to alter a track that does not exist.\n");
		}
	}

	public void AddMusicTrack(AudioSource audioSource, bool playing = false)
	{
		musicTracks.Add(audioSource);
		tracksActive.Add(playing);
	}

	private void RemoveLiveSource(int index)
	{
		liveSources.RemoveAt(index);
	}

	public void StopAllSounds()
	{
		for (int i = 0; i < liveSources.Count; i++)
		{
			if (liveSources[i] == null)
			{
				RemoveLiveSource(i);
				continue;
			}
			liveSources[i].Stop();
			UnityEngine.Object.Destroy(liveSources[i].gameObject, 0f);
			RemoveLiveSource(i);
		}
	}

	public void MuteAllSounds()
	{
		for (int i = 0; i < liveSources.Count; i++)
		{
			if (liveSources[i] == null)
			{
				RemoveLiveSource(i);
			}
			else
			{
				liveSources[i].volume = 0f;
			}
		}
	}

	public void UnmuteAllSounds()
	{
		for (int i = 0; i < liveSources.Count; i++)
		{
			if (liveSources[i] == null)
			{
				RemoveLiveSource(i);
			}
			else
			{
				liveSources[i].volume = maxMusicVol;
			}
		}
	}

	public void CrossFadeAudioSource(AudioSource audioSource, bool fadingIn = false)
	{
		if (!audioSource)
		{
			return;
		}
		if (fadingIn)
		{
			if (audioSource.volume < maxMusicVol - 0.1f)
			{
				audioSource.volume = Mathf.Lerp(audioSource.volume, maxMusicVol, 0.75f * Time.deltaTime);
			}
			else
			{
				audioSource.volume = maxMusicVol;
			}
		}
		else if ((double)audioSource.volume > 0.05)
		{
			audioSource.volume = Mathf.Lerp(audioSource.volume, 0f, 1f * Time.deltaTime);
		}
		else
		{
			audioSource.volume = 0f;
		}
	}

	public AudioSource MakeSource(string clipName = "Error", AudioClip customClip = null, Transform container = null)
	{
		GameObject obj = UnityEngine.Object.Instantiate(audioSourcePrefab, Vector3.zero, Quaternion.identity);
		AudioSource audioSource = null;
		obj.name = ((customClip != null) ? "Audio Source (Custom)" : ("Audio Source (" + clipName + ")"));
		audioSource = obj.GetComponent<AudioSource>();
		liveSources.Add(audioSource);
		obj.transform.SetParent(base.transform);
		audioSource.clip = ((customClip != null) ? customClip : FindOrLoadAudioClip(clipName));
		return audioSource;
	}

	public AudioClip FindOrLoadAudioClip(string clipName)
	{
		string text = Path.GetFileName(clipName);
		if (RDBase.IsAprilFoolsDay() && Persistence.GetLevelClearedByAnySlot(Level.OrientalInsomniac))
		{
			if (text == "sndPagerCursorMove")
			{
				text = "sndDoctahWeh";
				clipName = "Sfx/Other/sndDoctahWeh";
			}
			switch (text)
			{
			case "sndPagerButton":
			case "sndMenuSelect":
			case "sndMenuSelectBoss":
				text = "sndDoctahHehehe";
				clipName = "Sfx/Other/sndDoctahHehehe";
				break;
			}
		}
		if (audioLib.ContainsKey(text))
		{
			return audioLib[text];
		}
		if (audioLib.ContainsKey("snd" + text))
		{
			return audioLib["snd" + text];
		}
		AudioClip audioClip = Resources.Load<AudioClip>(clipName);
		if (audioClip == null && !text.IsNullOrEmpty())
		{
			audioClip = Resources.Load<AudioClip>(clipName.Replace(text, "snd" + text));
		}
		if (audioClip != null)
		{
			audioClipsLoaded++;
			audioLib.Add(text, audioClip);
			return audioLib[text];
		}
		Debug.LogWarning("FindOrLoadAudioClip(): Could not find: " + clipName + " in Audio Lib");
		return null;
	}

	public IEnumerator FindOrLoadAudioClipExternal(string path)
	{
		string filename = Path.GetFileName(path);
		string extension = Path.GetExtension(path);
		if (audioLib.ContainsKey(filename + "*external"))
		{
			yield return new RDAudioLoadResult(RDAudioLoadType.SuccessExternalClipLoaded, audioLib[filename + "*external"]);
			yield break;
		}
		bool loadSuccess = false;
		AudioClip clip = null;
		AudioType audioType = RDUtils.GetAudioType(extension);
		switch (audioType)
		{
		case AudioType.AIFF:
		case AudioType.OGGVORBIS:
		case AudioType.WAV:
		{
			string absoluteUri = new Uri(path).AbsoluteUri;
			absoluteUri = absoluteUri.Replace("+", "%2B");
			UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(absoluteUri, audioType);
			yield return uwr.SendWebRequest();
			DownloadHandlerAudioClip downloadHandlerAudioClip = uwr.downloadHandler as DownloadHandlerAudioClip;
			clip = downloadHandlerAudioClip.audioClip;
			if (clip == null)
			{
				yield return new RDAudioLoadResult(RDAudioLoadType.ErrorFileNotFound, null);
			}
			else if (clip.length == 0f)
			{
				switch (audioType)
				{
				case AudioType.OGGVORBIS:
					yield return new RDAudioLoadResult(RDAudioLoadType.ErrorLoadingOggVorbis, null);
					break;
				case AudioType.WAV:
					yield return new RDAudioLoadResult(RDAudioLoadType.ErrorLoadingWAV, null);
					break;
				case AudioType.AIFF:
					yield return new RDAudioLoadResult(RDAudioLoadType.ErrorLoadingAIFF, null);
					break;
				}
			}
			else
			{
				loadSuccess = true;
			}
			break;
		}
		case AudioType.MPEG:
		{
			bool num = scnEditor.instance != null && GC.levelEditor_streamMP3Loading;
			bool flag = scnCLS.instance != null && GC.customLevelSelect_streamMP3Loading;
			if (num | flag)
			{
				string key = filename + "*external";
				if (!audioLib.ContainsKey(key))
				{
					RDMP3Stream rDMP3Stream = new RDMP3Stream(path);
					clip = rDMP3Stream.CreateMP3Stream();
					audioLib.Add(key, clip);
					rdMP3Streams.Add(rDMP3Stream);
					loadSuccess = true;
				}
				else
				{
					yield return new RDAudioLoadResult(RDAudioLoadType.SuccessExternalClipLoaded, audioLib[key]);
					loadSuccess = false;
				}
			}
			else
			{
				try
				{
					clip = MP3SharpUnity.CreateAudioClipUsingMP3File(path);
					loadSuccess = true;
				}
				catch (Exception ex)
				{
					Debug.LogError(ex.Message);
					loadSuccess = false;
				}
				if (!loadSuccess)
				{
					yield return new RDAudioLoadResult(RDAudioLoadType.ErrorLoadingMP3, clip);
				}
			}
			break;
		}
		default:
			yield return new RDAudioLoadResult(RDAudioLoadType.ErrorFormatNotSupported, null);
			break;
		}
		if (loadSuccess)
		{
			string conductorName = (clip.name = filename + "*external");
			yield return new RDAudioLoadResult(RDAudioLoadType.SuccessExternalClipLoaded, clip);
			if (!audioLib.ContainsKey(conductorName))
			{
				audioLib.Add(conductorName, clip);
			}
		}
	}

	public void PrintAllAudioLibData()
	{
		string text = string.Empty;
		foreach (string key in audioLib.Keys)
		{
			text = string.Concat(text, key + " -> " + (audioLib[key] != null), "\n");
		}
		Debug.Log(text);
	}

	public static AudioSource Play(string snd, double time, float volume, AudioMixerGroup group = null, float pitch = 1f, float pan = 0f, float startTruncation = 0f, bool ignoreListenerPause = false)
	{
		return Singleton<AudioManager>.Instance._Play(snd, time, volume, group, pitch, pan, startTruncation, ignoreListenerPause);
	}

	public AudioSource _Play(string snd, double time, float volume, AudioMixerGroup group, float pitch = 1f, float pan = 0f, float startTruncation = 0f, bool ignoreListenerPause = false)
	{
		AudioSource audioSource = MakeSource(snd);
		audioSource.volume = volume;
		audioSource.outputAudioMixerGroup = ((group == null) ? fallbackMixerGroup : group);
		audioSource.pitch = pitch;
		audioSource.panStereo = pan;
		audioSource.ignoreListenerPause = ignoreListenerPause;
		audioSource.PlayScheduled(time);
		if (startTruncation != 0f)
		{
			audioSource.time = startTruncation;
		}
		return audioSource;
	}

	public static AudioSource PlayImmediately(string snd, float volume, AudioMixerGroup group = null, float pitch = 1f, float pan = 0f, bool ignoreListenerPause = false, bool levelEditor = false, bool dontActuallyPlayIt = false)
	{
		return Singleton<AudioManager>.Instance._PlayImmediately(snd, volume, group, pitch, pan, ignoreListenerPause, levelEditor, dontActuallyPlayIt);
	}

	public AudioSource _PlayImmediately(string snd, float volume, AudioMixerGroup group, float pitch = 1f, float pan = 0f, bool ignoreListenerPause = false, bool levelEditor = false, bool dontActuallyPlayIt = false)
	{
		AudioSource audioSource = MakeSource(snd, null, levelEditor ? scnEditor.instance.audioSourcesContainer : null);
		audioSource.volume = volume;
		audioSource.outputAudioMixerGroup = ((group == null) ? fallbackMixerGroup : group);
		audioSource.pitch = pitch;
		audioSource.panStereo = pan;
		audioSource.ignoreListenerPause = ignoreListenerPause;
		if (!dontActuallyPlayIt)
		{
			audioSource.Play();
		}
		return audioSource;
	}

	public static AudioSource PlayExternal(AudioClip customClip, double time, float volume, bool ignoreListenerPause, AudioMixerGroup group = null, float pitch = 1f, float pan = 0f)
	{
		return Singleton<AudioManager>.Instance._PlayFromExternalClip(customClip, time, volume, ignoreListenerPause, group, pitch, pan);
	}

	public AudioSource _PlayFromExternalClip(AudioClip customClip, double time, float volume, bool ignoreListenerPause, AudioMixerGroup group, float pitch = 1f, float pan = 0f)
	{
		AudioSource audioSource = MakeSource("", customClip);
		audioSource.volume = volume;
		audioSource.ignoreListenerPause = ignoreListenerPause;
		audioSource.outputAudioMixerGroup = ((group == null) ? fallbackMixerGroup : group);
		audioSource.pitch = pitch;
		audioSource.panStereo = pan;
		audioSource.PlayScheduled(time);
		return audioSource;
	}

	public void FlushData()
	{
		audioLib.Clear();
		mp3Streams.Clear();
		mp3Samples.Clear();
		rdMP3Streams.Clear();
		foreach (AudioSource liveSource in liveSources)
		{
			if (liveSource != null)
			{
				liveSource.clip = null;
			}
		}
		foreach (AudioSource musicTrack in musicTracks)
		{
			if (musicTrack != null)
			{
				musicTrack.clip = null;
			}
		}
		DestroyAllAudioSources();
	}

	public void DestroyAllAudioSources()
	{
		foreach (Transform item in base.transform)
		{
			UnityEngine.Object.Destroy(item.gameObject);
		}
	}
}
