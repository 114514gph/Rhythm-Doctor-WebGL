using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public static class PauseMenuAudioPreview
{
	private static List<AudioSource> audioSources = new List<AudioSource>();

	private static int lastCount = 0;

	private static int oneshotPhraseProgress = 0;

	private static bool subdivThree = false;

	private static float firstOffset = 0f;

	private static readonly string[] oneshotWords = new string[7] { "RDGS_Rea", "RDGS_Dy", "RDGS_Get", "RDGS_Set", "RDGS_Go", "AndStop_And", "AndStop_Stop" };

	private static List<Tween> lastPreviewTweens = new List<Tween>();

	private static void Start()
	{
		PrepareHoldSound("sndHoldStartTamb");
		PrepareHoldSound("sndHoldStartTambAlt");
		PrepareHoldSound("sndHoldStartShaker");
		PrepareHoldSound("sndHoldStartShakerAlt");
		static void PrepareHoldSound(string sound)
		{
			string clipName = "Sfx/Beatsounds/" + sound;
			Singleton<AudioManager>.Instance.FindOrLoadAudioClip(clipName);
		}
	}

	public static void PlayPreviewAudio(PauseContentName name, string soundToPlay = "")
	{
		StopPreviewAudio();
		if (name <= PauseContentName.BeatsoundPitchMode)
		{
			switch (name)
			{
			case PauseContentName.GameplayVolume:
			case PauseContentName.BeatsoundsVolume:
			case PauseContentName.BeatsoundPitchVariance:
			case PauseContentName.BeatsoundPitchMode:
				PlaySevenBeats();
				break;
			case PauseContentName.HitsoundsVolume:
				soundToPlay = new string[4] { "sndClapHit", "sndClapHitP2", "sndReverbClap", "sndClapHitEcho" }.GetRandomItem();
				PreviewAudioPlaySound(soundToPlay, "HitsoundsParent");
				break;
			case PauseContentName.CuesoundsVolume:
				soundToPlay = new string[2] { "sndJyi - ", "sndIan - " }.GetRandomItem() + oneshotWords[oneshotPhraseProgress];
				PreviewAudioPlaySound(soundToPlay, "CuesoundsParent");
				if (oneshotPhraseProgress >= oneshotWords.Length - 1)
				{
					oneshotPhraseProgress = 0;
				}
				else
				{
					oneshotPhraseProgress++;
				}
				break;
			case PauseContentName.FeedbackVolume:
				soundToPlay = new string[6] { "sndExplodeNewv1_1", "sndExplodeNewv1_2", "sndExplodeNewv1_3", "sndMistakeSmall3", "sndMistakeBig", "sndMenuPop" }.GetRandomItem();
				PreviewAudioPlaySound(soundToPlay, "FeedbackSoundsParent");
				break;
			case PauseContentName.DialogueVolume:
			{
				soundToPlay = new string[5] { "sndSpeechIan", "sndSpeechPaige", "sndSpeechLogan", "sndSpeechHailey", "sndSpeechSamurai" }.GetRandomItem();
				AudioSource src = PreviewAudioPlaySound(soundToPlay, "DialogueParent");
				for (int i = 0; i < 9; i++)
				{
					lastPreviewTweens.RDAdd(DOVirtual.DelayedCall((float)i * 0.065f, () =>
					{
						src.volume = Mathf.Clamp(RXRandom.Range(0.91f, 1.1f) * scrConductor.GlobalOtherSoundVolume, 0f, 1f);
						src.pitch = Mathf.Clamp(RXRandom.Range(0.97087383f, 1.03f), 0f, 3f);
						src.Play();
					}));
				}
				break;
			}
			case PauseContentName.InterfaceVolume:
				PreviewAudioPlaySound("sndTestTone1");
				break;
			case PauseContentName.PanningStrength:
			case PauseContentName.PanningInvert:
				PreviewAudioPlaySound("sndTestTone1", "InterfaceSoundsParent", 1f, 1f, -1f);
				PreviewAudioPlaySound("sndTestTone2", "InterfaceSoundsParent", 1f, 1f, -0.5f);
				PreviewAudioPlaySound("sndTestTone3");
				PreviewAudioPlaySound("sndTestTone4", "InterfaceSoundsParent", 1f, 1f, 0.5f);
				PreviewAudioPlaySound("sndTestTone5", "InterfaceSoundsParent", 1f, 1f, 1f);
				RDBaseDllDummy.printem($"audiosources in list: {audioSources.Count()}");
				break;
			}
			return;
		}
		if (name <= PauseContentName.EditorUIVolume)
		{
			switch (name)
			{
			case PauseContentName.CountingVoiceVolume:
				soundToPlay = new string[6] { "sndJyi - ChineseCount", "sndJyi - ChineseCountFast", "sndJyi - ChineseCountCalm", "sndIan - ChineseCount", "sndIan - ChineseCountFast", "sndIan - ChineseCountCalm" }.GetRandomItem() + (lastCount + 1);
				PreviewAudioPlaySound(soundToPlay, "CountingVoice");
				lastCount++;
				if (lastCount == 7)
				{
					lastCount = 0;
				}
				break;
			case PauseContentName.EditorUIVolume:
				soundToPlay = new string[7] { "sndEditorEventCreate", "sndEditorEventCut", "sndEditorEventDelete", "sndEditorEventSelect", "sndEditorEventDeselect", "sndEditorEventDisable", "sndEditorEventEnable" }.GetRandomItem();
				PreviewAudioPlaySound(soundToPlay, "LevelEditorParent");
				break;
			}
			return;
		}
		switch (name)
		{
		default:
			_ = 3254;
			break;
		case PauseContentName.PanningStrength2P:
			PlaySevenBeats("sndShaker", is2P: true, GC.PanP1);
			PlaySevenBeats("sndKick", is2P: true, GC.PanP2);
			break;
		case PauseContentName.RowInfoCuesGlossary:
			soundToPlay = soundToPlay switch
			{
				"enum.NarrateInfoType.Connect" => "sndPatientConnect", 
				"enum.NarrateInfoType.Update" => "sndPatientUpdate", 
				"enum.NarrateInfoType.Disconnect" => "sndPatientDisconnect", 
				"enum.NarrateInfoType.Online" => "sndOttoActivate", 
				"enum.NarrateInfoType.Offline" => "sndOttoDeactivate", 
				_ => "", 
			};
			PreviewAudioPlaySound(soundToPlay, "CuesoundsParent");
			break;
		case PauseContentName.BeatsoundsGlossary:
			soundToPlay = soundToPlay.Replace("enum.SoundEffect.", "snd");
			PlaySevenBeats(soundToPlay);
			break;
		case PauseContentName.HitsoundsGlossary:
			soundToPlay = soundToPlay.Replace("enum.SoundEffect.", "snd");
			PreviewAudioPlaySound(soundToPlay, "HitsoundsParent");
			break;
		case PauseContentName.HeartExplosionsGlossary:
			soundToPlay = soundToPlay.Replace("enum.SoundGlossary.HeartExplosion", "sndExplodeNewv1_");
			PreviewAudioPlaySound(soundToPlay, "HeartExplosionsParent");
			break;
		case PauseContentName.HeartCracksGlossary:
		{
			float volume = 1f;
			float pitch = 1f;
			switch (soundToPlay)
			{
			case "enum.SoundGlossary.MistakeSmallEarly":
				soundToPlay = "sndMistakeSmall3";
				break;
			case "enum.SoundGlossary.MistakeSmallLate":
				soundToPlay = "sndMistakeSmall3";
				volume = 1.25f;
				pitch = 0.8f;
				break;
			case "enum.SoundGlossary.MistakeLargeEarly":
				soundToPlay = "sndMistakeBig";
				break;
			case "enum.SoundGlossary.MistakeLargeLate":
				soundToPlay = "sndMistakeBig";
				volume = 1.25f;
				pitch = 0.8f;
				break;
			default:
				Debug.LogError("Sound not found: " + soundToPlay);
				break;
			}
			PreviewAudioPlaySound(soundToPlay, "MistakesParent", volume, pitch);
			break;
		}
		case PauseContentName.ClassicCuesGlossary:
			switch (soundToPlay)
			{
			case "enum.CountingVoiceSource.BirdCount":
				PlaySoundSequence(new string[4] { "sndBird1", "sndBird3", "sndBird5", "sndBird7" }, "CountingVoice");
				break;
			case "enum.CountingVoiceSource.WhistleCount":
				PlaySoundSequence(new string[4] { "sndWhistle1", "sndWhistle3", "sndWhistle5", "sndWhistle7" }, "CountingVoice");
				break;
			case "enum.CountingVoiceSource.OwlCount":
				PlaySoundSequence(new string[3] { "sndOwl1", "sndOwl4", "sndOwl7" }, "CountingVoice");
				break;
			case "enum.SoundGlossary.HeldbeatsP1":
				PlaySoundSequence(new string[8] { "sndHoldStartTamb", "sndHoldEndTambShort", "sndHoldStartTambAlt", "sndHoldEndTambShortAlt", "sndHoldStartTamb", "sndHoldEndTambShort", "sndHoldStartTambAlt", "sndHoldEndTambShortAlt" }, "BeatsoundsParent", isBeats: false, isHeldbeat: true);
				break;
			case "enum.SoundGlossary.HeldbeatsP2":
				PlaySoundSequence(new string[8] { "sndHoldStartShaker", "sndHoldEndShakerShort", "sndHoldStartShakerAlt", "sndHoldEndShakerShortAlt", "sndHoldStartShaker", "sndHoldEndShakerShort", "sndHoldStartShakerAlt", "sndHoldEndShakerShortAlt" }, "BeatsoundsParent", isBeats: false, isHeldbeat: true);
				break;
			default:
				Debug.LogError("Sound not found: " + soundToPlay);
				break;
			}
			break;
		case PauseContentName.OneshotCuesGlossary:
			switch (soundToPlay)
			{
			case "enum.SoundGlossary.Vibraslap":
				PreviewAudioPlaySound("sndVibraslap", "SpecialOneshotCues");
				break;
			case "enum.SoundGlossary.Squareshot":
				PreviewAudioPlaySound("sndSquareshot", "BeatsoundsParent");
				break;
			case "enum.SoundGlossary.Triangleshots":
				if (subdivThree)
				{
					PlaySoundSequence(new string[3] { "sndTriangleshot1", "sndTriangleshot2", "sndTriangleshot3" }, "BeatsoundsParent");
				}
				else
				{
					PlaySoundSequence(new string[2] { "sndTriangleshot1", "sndTriangleshot2" }, "BeatsoundsParent");
				}
				subdivThree = !subdivThree;
				break;
			case "enum.SoundGlossary.FreezeshotCue":
				PlaySoundSequence(new string[2] { "sndFreezeshotCueLow", "sndFreezeshotCueHigh" }, "SpecialOneshotCues");
				break;
			case "enum.SoundGlossary.FreezeshotHit":
				PlaySoundSequence(new string[3] { "sndFreezeshotCueHigh", "sndFreezeshotCueLow", "sndFreezeshotCymbal" }, "SpecialOneshotCues", isBeats: false, isHeldbeat: false, isFreezeOrBurnHit: true);
				break;
			case "enum.SoundGlossary.BurnshotCue":
				PlaySoundSequence(new string[2] { "sndBurnshotCueLow", "sndBurnshotCueHigh" }, "SpecialOneshotCues");
				break;
			case "enum.SoundGlossary.BurnshotHit":
				PlaySoundSequence(new string[3] { "sndBurnshotCueHigh", "sndBurnshotCueLow", "sndBurnshotCymbal" }, "SpecialOneshotCues", isBeats: false, isHeldbeat: false, isFreezeOrBurnHit: true);
				break;
			case "enum.SoundGlossary.BlastbeatStart":
				PlaySoundSequence(new string[4] { "sndRefereeShort", "sndRefereeLow", "sndRefereeShort", "sndRefereeLow" }, "SpecialOneshotCues");
				break;
			case "enum.SoundGlossary.BlastbeatEnd":
				PlaySoundSequence(new string[3] { "sndRefereeHigh", "sndRefereeHigh", "sndRefereeLow" }, "SpecialOneshotCues");
				break;
			default:
				Debug.LogError("Sound not found: " + soundToPlay);
				break;
			}
			break;
		}
	}

	public static AudioSource PreviewAudioPlaySound(string soundToPlay, string mixerPath = "InterfaceSoundsParent", float volume = 1f, float pitch = 1f, float pan = 0f)
	{
		if (soundToPlay == string.Empty)
		{
			return null;
		}
		return audioSources.RDAdd(scrConductor.PlayImmediately(soundToPlay, volume, RDUtils.GetMixerGroup(mixerPath), pitch, pan, isBeatSound: false, ignoreListenerPause: true));
	}

	private static void PlaySevenBeats(string soundToPlay = "", bool is2P = false, float pan = 0f)
	{
		if (soundToPlay == "")
		{
			soundToPlay = new string[6] { "sndShaker", "sndKick", "sndKickChroma", "sndSizzle", "sndStick", "sndHatTight" }.GetRandomItem();
		}
		PlaySoundSequence(new string[7] { soundToPlay, soundToPlay, soundToPlay, soundToPlay, soundToPlay, soundToPlay, soundToPlay }, "BeatsoundsParent", isBeats: true, isHeldbeat: false, isFreezeOrBurnHit: false, !is2P, pan);
	}

	private static void PlaySoundSequence(string[] soundsToPlay, string mixerPath, bool isBeats = false, bool isHeldbeat = false, bool isFreezeOrBurnHit = false, bool randomizeSpeed = true, float pan = 0f)
	{
		float num = (randomizeSpeed ? RXRandom.Range(0.25f, 0.35f) : 0.3f);
		float num2 = 1f;
		for (int i = 0; i < soundsToPlay.Length; i++)
		{
			if (scrConductor.BeatsoundPitchVarianceMultiplier != 1f)
			{
				if (isBeats)
				{
					num2 = BeatClassic.GetAlternatingBeatPitchMultiplier(i);
				}
				if (isHeldbeat)
				{
					num2 = BeatClassic.GetAlternatingBeatPitchMultiplier(i / 2);
				}
			}
			string text = soundsToPlay[i];
			float pitch = num2;
			AudioSource src = scrConductor.PlayImmediately(text, 1f, RDUtils.GetMixerGroup(mixerPath), pitch, pan, isBeatSound: false, ignoreListenerPause: true, dontActuallyPlayIt: true);
			audioSources.RDAdd(src);
			float num3 = RDSongOffsets.instance.Get(text).offset / num2;
			float num4 = num * (float)i;
			if (isFreezeOrBurnHit && i == soundsToPlay.Length - 1)
			{
				num4 = num * (float)(i - 1);
			}
			float delay = 0f;
			if (i == 0)
			{
				firstOffset = num3;
			}
			else
			{
				delay = firstOffset + num4 - num3;
			}
			lastPreviewTweens.RDAdd(DOVirtual.DelayedCall(delay, () =>
			{
				src.Play();
			}));
			if (isHeldbeat && i % 2 == 0)
			{
				lastPreviewTweens.RDAdd(DOVirtual.DelayedCall(firstOffset + num * (float)(i + 1), () =>
				{
					src.Stop();
				}));
			}
		}
	}

	public static void StopPreviewAudio()
	{
		if (audioSources.Count > 0)
		{
			foreach (AudioSource audioSource in audioSources)
			{
				if (!(audioSource == null))
				{
					audioSource.Stop();
					Object.Destroy(audioSource);
				}
			}
			audioSources.Clear();
		}
		if (lastPreviewTweens.Count <= 0)
		{
			return;
		}
		foreach (Tween lastPreviewTween in lastPreviewTweens)
		{
			lastPreviewTween.Kill();
		}
		lastPreviewTweens.Clear();
	}
}
