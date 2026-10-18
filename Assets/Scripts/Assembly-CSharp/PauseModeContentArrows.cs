using System;
using System.Linq;
using DG.Tweening;
using RDLevelEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseModeContentArrows : RDBase, IPointerDownHandler, IEventSystemHandler
{
	[Serializable]
	public struct Arrow
	{
		public RectTransform rect;

		public int direction;

		public Vector2 startPos;
	}

	public string valueKey;

	public Text valueText;

	public Arrow leftArrow;

	public Arrow rightArrow;

	private RectTransform rectTransform;

	private bool skipNextInitialize;

	[NonSerialized]
	public bool canChangeContentValue;

	public PauseMenuContentData contentData;

	[NonSerialized]
	public PauseMenu pauseMenu;

	[Header("Glitch")]
	public GameObject glitchesPrefab;

	[NonSerialized]
	public SpriteAnimation[] glitches;

	[NonSerialized]
	public GameObject disabledImage;

	[NonSerialized]
	public bool showedGlitchOnce;

	[NonSerialized]
	public AudioSource glitchAudioSource;

	private const float arrowWidth = 10f;

	private int currentVolume
	{
		get
		{
			return scrGameManager.volume;
		}
		set
		{
			scrGameManager.volume = value;
		}
	}

	private int currentMusicVolume
	{
		get
		{
			return scrGameManager.musicVolume;
		}
		set
		{
			scrGameManager.musicVolume = value;
		}
	}

	private int currentGameplayVolume
	{
		get
		{
			return scrGameManager.gameplayVolume;
		}
		set
		{
			scrGameManager.gameplayVolume = value;
		}
	}

	private int currentBeatsoundsVolume
	{
		get
		{
			return scrGameManager.beatsoundsVolume;
		}
		set
		{
			scrGameManager.beatsoundsVolume = value;
		}
	}

	private int currentHitsoundsVolume
	{
		get
		{
			return scrGameManager.hitsoundsVolume;
		}
		set
		{
			scrGameManager.hitsoundsVolume = value;
		}
	}

	private int currentCountingVoiceVolume
	{
		get
		{
			return scrGameManager.countingVoiceVolume;
		}
		set
		{
			scrGameManager.countingVoiceVolume = value;
		}
	}

	private int currentCuesoundsVolume
	{
		get
		{
			return scrGameManager.cuesoundsVolume;
		}
		set
		{
			scrGameManager.cuesoundsVolume = value;
		}
	}

	private int currentFeedbackVolume
	{
		get
		{
			return scrGameManager.feedbackVolume;
		}
		set
		{
			scrGameManager.feedbackVolume = value;
		}
	}

	private int currentInterfaceVolume
	{
		get
		{
			return scrGameManager.interfaceVolume;
		}
		set
		{
			scrGameManager.interfaceVolume = value;
		}
	}

	private int currentDialogueVolume
	{
		get
		{
			return scrGameManager.dialogueVolume;
		}
		set
		{
			scrGameManager.dialogueVolume = value;
		}
	}

	private int currentAmbienceVolume
	{
		get
		{
			return scrGameManager.ambienceVolume;
		}
		set
		{
			scrGameManager.ambienceVolume = value;
		}
	}

	private int currentEditorVolume
	{
		get
		{
			return scrGameManager.levelEditorVolume;
		}
		set
		{
			scrGameManager.levelEditorVolume = value;
		}
	}

	public void PlayGlitchAnimation()
	{
		if (glitchAudioSource != null)
		{
			glitchAudioSource.Stop();
		}
		glitchAudioSource = scrConductor.PlayImmediately("sndError", 1f, RDUtils.GetMixerGroup("PauseMenu"), 1f, 0f, isBeatSound: false, ignoreListenerPause: true);
		if (!glitches.Any((SpriteAnimation glitch) => glitch.playing))
		{
			SpriteAnimation[] array = glitches;
			foreach (SpriteAnimation obj in array)
			{
				obj.Stop();
				obj.gameObject.SetActive(value: true);
				obj.Play();
			}
		}
	}

	public void ShowDisabledIcon()
	{
		disabledImage.SetActive(value: true);
		SetValueTextCustom("", "");
		RectTransform component = valueText.GetComponent<RectTransform>();
		component.sizeDelta = new Vector2(0f, component.sizeDelta.y);
		Button[] componentsInChildren = GetComponentsInChildren<Button>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].gameObject.SetActive(value: false);
		}
	}

	public void Initialize(PauseMenuMode newParentMode, PauseMenuContentData newContentData)
	{
		if (skipNextInitialize)
		{
			return;
		}
		rectTransform = GetComponent<RectTransform>();
		pauseMenu = newParentMode.parentPauseMenu;
		contentData = newContentData;
		if (!canChangeContentValue)
		{
			if (glitches == null)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(glitchesPrefab, base.transform.parent);
				glitches = gameObject.GetComponentsInChildren<SpriteAnimation>();
			}
		}
		else
		{
			UpdateValue();
		}
	}

	public void SetValueText(string str, bool isKey)
	{
		valueKey = str;
		valueText.text = (isKey ? RDString.Get(str) : str);
	}

	public void SetValueTextPercent(int amt)
	{
		valueKey = amt.ToString();
		valueText.text = amt + "%";
	}

	public void SetValueTextCustom(string key, string str = null)
	{
		valueKey = key;
		valueText.text = str ?? key;
	}

	public string GetStartValueText(object startValue)
	{
		bool isKey;
		return contentData.SetStartValue(startValue, out isKey);
	}

	public void SetStartValueText(object startValue)
	{
		SetValueText(contentData.SetStartValue(startValue, out var isKey), isKey);
	}

	public void UpdateValue()
	{
		switch (contentData.name)
		{
		case PauseContentName.Fullscreen:
		{
			bool flag2 = Screen.fullScreenMode != FullScreenMode.Windowed;
			SetStartValueText(flag2);
			break;
		}
		case PauseContentName.VSync:
		{
			bool flag = QualitySettings.vSyncCount > 0;
			Persistence.SetVSync(flag);
			SetStartValueText(flag);
			break;
		}
		case PauseContentName.Resolution:
		case PauseContentName.Language:
		case PauseContentName.Voices:
			SetStartValueText(null);
			break;
		case PauseContentName.DefibrillatorP1:
			SetStartValueText(Persistence.GetDefibrillatorP1());
			break;
		case PauseContentName.DefibrillatorP2:
			SetStartValueText(Persistence.GetDefibrillatorP2());
			break;
		case PauseContentName.ForceDesktopFeatureSet:
			SetStartValueText(Persistence.GetForceDesktopFeatureSet());
			break;
		case PauseContentName.Volume:
			SetStartValueText(currentVolume);
			break;
		case PauseContentName.MusicVolume:
			SetStartValueText(currentMusicVolume);
			break;
		case PauseContentName.GameplayVolume:
			SetStartValueText(currentGameplayVolume);
			break;
		case PauseContentName.BeatsoundsVolume:
			SetStartValueText(currentBeatsoundsVolume);
			break;
		case PauseContentName.HitsoundsVolume:
			SetStartValueText(currentHitsoundsVolume);
			break;
		case PauseContentName.CountingVoiceVolume:
			SetStartValueText(currentCountingVoiceVolume);
			break;
		case PauseContentName.CuesoundsVolume:
			SetStartValueText(currentCuesoundsVolume);
			break;
		case PauseContentName.FeedbackVolume:
			SetStartValueText(currentFeedbackVolume);
			break;
		case PauseContentName.InterfaceVolume:
			SetStartValueText(currentInterfaceVolume);
			break;
		case PauseContentName.DialogueVolume:
			SetStartValueText(currentDialogueVolume);
			break;
		case PauseContentName.AmbienceVolume:
			SetStartValueText(currentAmbienceVolume);
			break;
		case PauseContentName.MuteInBackground:
			SetStartValueText(Persistence.GetMuteInBackground());
			break;
		case PauseContentName.PanningStrength:
			SetValueTextPercent(int.Parse(GetStartValueText(Persistence.GetPanningStrength())));
			break;
		case PauseContentName.PanningStrength2P:
			SetValueTextPercent(int.Parse(GetStartValueText(Persistence.GetPanningStrength2P())));
			break;
		case PauseContentName.PanningInvert:
			SetStartValueText(Persistence.GetPanningInvert());
			break;
		case PauseContentName.BeatsoundPitchVariance:
			SetValueTextPercent(int.Parse(GetStartValueText(Persistence.GetBeatsoundPitchVariance())));
			break;
		case PauseContentName.BeatsoundPitchMode:
			SetStartValueText(Persistence.GetBeatsoundPitchMode());
			break;
		case PauseContentName.HeartExplosionsGlossary:
		case PauseContentName.RowInfoCuesGlossary:
		case PauseContentName.BeatsoundsGlossary:
		case PauseContentName.HitsoundsGlossary:
		case PauseContentName.HeartCracksGlossary:
		case PauseContentName.ClassicCuesGlossary:
		case PauseContentName.OneshotCuesGlossary:
			SetStartValueText(0);
			break;
		case PauseContentName.EnableExtraDifficultyOptions:
			SetStartValueText(Persistence.GetExtraDifficultyOptions());
			break;
		case PauseContentName.EnableMoreVolumeSettings:
			SetStartValueText(Persistence.GetMoreVolumeSettings());
			break;
		case PauseContentName.AccessibilityScope:
			SetStartValueText(Persistence.GetAccessibilityScope());
			break;
		case PauseContentName.WindowMovement:
			SetStartValueText(Persistence.GetWindowMovement());
			break;
		case PauseContentName.StreamerMode:
			SetStartValueText(Persistence.GetWindowMovement());
			break;
		case PauseContentName.ReducePixelFontUsage:
			SetStartValueText(Persistence.GetReducePixelFontUsage());
			break;
		case PauseContentName.InstantReducePixelFontUsage:
			SetStartValueText(Persistence.GetReducePixelFontUsage());
			break;
		case PauseContentName.AudibleSkipBeats:
			SetStartValueText(Persistence.GetAudibleSkipBeats());
			break;
		case PauseContentName.TapsOnly:
			SetStartValueText(Persistence.GetTapsOnly());
			break;
		case PauseContentName.DrumMode:
			SetStartValueText(Persistence.GetDrumMode());
			break;
		case PauseContentName.FlashIntensity:
		{
			int num = int.Parse(GetStartValueText(Persistence.GetFlashIntensity()));
			SetValueTextPercent((num == 0) ? 10 : num);
			break;
		}
		case PauseContentName.BackgroundDimming:
			SetValueTextPercent(int.Parse(GetStartValueText(Persistence.GetBackgroundDimming())));
			break;
		case PauseContentName.ReducedFlash:
			SetStartValueText(Persistence.GetReducedFlash());
			break;
		case PauseContentName.Narration:
			SetStartValueText(Persistence.GetNarration());
			break;
		case PauseContentName.NarrationDelayed:
			SetStartValueText(Persistence.GetNarration());
			break;
		case PauseContentName.SpeechRate:
		{
			int speechRate = Persistence.GetSpeechRate();
			SetStartValueText(speechRate);
			if (speechRate == 0)
			{
				SetValueTextCustom(speechRate.ToString(), RDString.Get("pauseMenu.levelDetail.narrationSystemValue"));
			}
			break;
		}
		case PauseContentName.SpeechVolume:
		{
			int speechVolume = Persistence.GetSpeechVolume();
			SetStartValueText(speechVolume);
			if (speechVolume == 11)
			{
				SetValueTextCustom(speechVolume.ToString(), RDString.Get("pauseMenu.levelDetail.narrationSystemValue"));
			}
			break;
		}
		case PauseContentName.NarrateDescriptions:
			SetStartValueText(Persistence.GetNarrateDescriptions());
			break;
		case PauseContentName.NarrateSubtitles:
			SetStartValueText(Persistence.GetNarrateSubtitles());
			break;
		case PauseContentName.LevelSelectReportPosition:
			SetStartValueText(Persistence.GetLevelSelectReportPosition());
			break;
		case PauseContentName.OtherMenusReportPosition:
			SetStartValueText(Persistence.GetOtherMenusReportPosition());
			break;
		case PauseContentName.ForceNarrateCustomLevels:
			SetStartValueText(Persistence.GetForceNarrateCustomLevels());
			break;
		case PauseContentName.ForceSAPINarration:
			SetStartValueText(Persistence.GetForceSAPINarration());
			break;
		case PauseContentName.InputOffsetP1:
			GetStartValueText(Mathf.RoundToInt(RDCalibration.calibration_i * 1000f));
			SetValueTextCustom(RDCalibration.calibration_i.ToString(), RDCalibration.calibration_i.ToMilliseconds());
			break;
		case PauseContentName.InputOffsetP2:
			GetStartValueText(Mathf.RoundToInt(RDCalibration.calibration_i_P2_internal * 1000f));
			SetValueTextCustom(RDCalibration.calibration_i_P2_internal.ToString(), RDCalibration.calibration_i_P2_internal.ToMilliseconds());
			break;
		case PauseContentName.VisualOffset:
			GetStartValueText(Mathf.RoundToInt(RDCalibration.calibration_v * 1000f));
			SetValueTextCustom(RDCalibration.calibration_v.ToString(), RDCalibration.calibration_v.ToMilliseconds());
			break;
		case PauseContentName.FrameOffsets:
			SetStartValueText(Persistence.GetFrameOffsets());
			break;
		case PauseContentName.ErrorMargins:
			SetStartValueText(Persistence.GetErrorMargins());
			break;
		case PauseContentName.ShowStartupWarnings:
			SetStartValueText(Persistence.GetShowStartupWarnings());
			break;
		case PauseContentName.InstantDialogue:
			SetStartValueText(Persistence.GetInstantDialogue());
			break;
		case PauseContentName.FastMenu:
			SetStartValueText(Persistence.GetFastMenu());
			break;
		case PauseContentName.DiscordPresence:
			SetStartValueText(Persistence.GetDiscordPresence());
			break;
		case PauseContentName.EditorAlwaysShowTooltips:
			SetStartValueText(Persistence.GetLevelEditorAlwaysShowTooltips());
			break;
		case PauseContentName.EditorUIVolume:
			SetStartValueText(Persistence.GetLevelEditorVolume());
			break;
		case PauseContentName.EditorEnableUndo:
			SetStartValueText(Persistence.GetUndoEnabled());
			break;
		case PauseContentName.EditorEnableComments:
			SetStartValueText(Persistence.GetCommentPlaybackEnabled());
			break;
		case PauseContentName.EditorWindowTab:
			SetStartValueText(Persistence.GetWindowTabEnabled());
			break;
		}
		PauseMenuMode.CheckCJKText(valueText);
		if (contentData.name != PauseContentName.Fullscreen)
		{
			rightArrow.rect.gameObject.SetActive(contentData.CanChangeRight());
			leftArrow.rect.gameObject.SetActive(contentData.CanChangeLeft());
		}
	}

	public void ChangeContentValue(int direction, bool shiftPressed)
	{
		if ((direction == 1 && !contentData.CanChangeRight()) || (direction == -1 && !contentData.CanChangeLeft()))
		{
			scrConductor.PlayImmediatelyIgnoringTimeScale("sndPagerCursorBorder", 1f, RDUtils.GetMixerGroup("PauseMenu"));
			return;
		}
		if (!canChangeContentValue)
		{
			PlayGlitchAnimation();
			return;
		}
		switch (contentData.name)
		{
		case PauseContentName.Fullscreen:
		case PauseContentName.Resolution:
		case PauseContentName.ReducePixelFontUsage:
			ChangeValue();
			break;
		case PauseContentName.InstantReducePixelFontUsage:
			ChangeValue();
			break;
		case PauseContentName.VSync:
			ChangeValue();
			if (contentData.changedValue)
			{
				bool currentBoolValue9 = contentData.CurrentBoolValue;
				QualitySettings.vSyncCount = (currentBoolValue9 ? 1 : 0);
				Persistence.SetVSync(currentBoolValue9);
			}
			break;
		case PauseContentName.Language:
			ChangeValue(set: false);
			SetValueTextCustom(contentData.CurrentLanguage.ToString(), RDString.GetNativeLanguageName(contentData.CurrentLanguage));
			break;
		case PauseContentName.ForceDesktopFeatureSet:
			ChangeValue();
			break;
		case PauseContentName.Voices:
			ChangeValue();
			if (contentData.changedValue)
			{
				RDString.ChangeVoiceLanguage(contentData.CurrentLanguage);
			}
			break;
		case PauseContentName.DefibrillatorP1:
		case PauseContentName.DefibrillatorP2:
		{
			ChangeValue();
			if (!contentData.changedValue)
			{
				break;
			}
			bool flag = contentData.name == PauseContentName.DefibrillatorP1;
			if (flag)
			{
				scnGame.p1DefibMode = contentData.currentDefibValue;
				Persistence.SetDefibrillatorP1(contentData.currentDefibValue);
			}
			else
			{
				scnGame.p2DefibMode = contentData.currentDefibValue;
				Persistence.SetDefibrillatorP2(contentData.currentDefibValue);
			}
			if (!(base.game != null))
			{
				break;
			}
			for (int i = 0; i < 5; i++)
			{
				scrHandController handControllerFromInt = base.game.GetHandControllerFromInt(i);
				RDArm rDArm = handControllerFromInt.rightArm;
				if ((flag && RDInput.p1[0].schemeIndex != 0) || (!flag && RDInput.p1[0].schemeIndex == 0))
				{
					rDArm = handControllerFromInt.leftArm;
				}
				rDArm.SetDefibMode(contentData.currentDefibValue);
			}
			break;
		}
		case PauseContentName.Volume:
			ChangeValue();
			if (contentData.changedValue)
			{
				currentVolume = int.Parse(valueText.text);
			}
			Persistence.SetGlobalVolume(currentVolume);
			break;
		case PauseContentName.MusicVolume:
			ChangeValue();
			if (contentData.changedValue)
			{
				currentMusicVolume = int.Parse(valueText.text);
			}
			Persistence.SetMusicVolume(currentMusicVolume);
			break;
		case PauseContentName.GameplayVolume:
			ChangeValue();
			if (contentData.changedValue)
			{
				currentGameplayVolume = int.Parse(valueText.text);
			}
			Persistence.SetGameplayVolume(currentGameplayVolume);
			break;
		case PauseContentName.InterfaceVolume:
			ChangeValue();
			if (contentData.changedValue)
			{
				currentInterfaceVolume = int.Parse(valueText.text);
			}
			Persistence.SetInterfaceVolume(currentInterfaceVolume);
			break;
		case PauseContentName.BeatsoundsVolume:
			ChangeValue();
			if (contentData.changedValue)
			{
				currentBeatsoundsVolume = int.Parse(valueText.text);
			}
			Persistence.SetBeatsoundsVolume(currentBeatsoundsVolume);
			break;
		case PauseContentName.HitsoundsVolume:
			ChangeValue();
			if (contentData.changedValue)
			{
				currentHitsoundsVolume = int.Parse(valueText.text);
			}
			Persistence.SetHitsoundsVolume(currentHitsoundsVolume);
			break;
		case PauseContentName.CountingVoiceVolume:
			ChangeValue();
			if (contentData.changedValue)
			{
				currentCountingVoiceVolume = int.Parse(valueText.text);
			}
			Persistence.SetCountingVoiceVolume(currentCountingVoiceVolume);
			break;
		case PauseContentName.CuesoundsVolume:
			ChangeValue();
			if (contentData.changedValue)
			{
				currentCuesoundsVolume = int.Parse(valueText.text);
			}
			Persistence.SetCuesoundsVolume(currentCuesoundsVolume);
			break;
		case PauseContentName.FeedbackVolume:
			ChangeValue();
			if (contentData.changedValue)
			{
				currentFeedbackVolume = int.Parse(valueText.text);
			}
			Persistence.SetFeedbackVolume(currentFeedbackVolume);
			break;
		case PauseContentName.DialogueVolume:
			ChangeValue();
			if (contentData.changedValue)
			{
				currentDialogueVolume = int.Parse(valueText.text);
			}
			Persistence.SetDialogueVolume(currentDialogueVolume);
			break;
		case PauseContentName.AmbienceVolume:
			ChangeValue();
			if (contentData.changedValue)
			{
				currentAmbienceVolume = int.Parse(valueText.text);
			}
			Persistence.SetAmbienceVolume(currentAmbienceVolume);
			break;
		case PauseContentName.MuteInBackground:
		{
			ChangeValue();
			bool currentBoolValue11 = contentData.CurrentBoolValue;
			if (contentData.changedValue)
			{
				scrGameManager.muteInBackground = currentBoolValue11;
			}
			Persistence.SetMuteInBackground(currentBoolValue11);
			break;
		}
		case PauseContentName.PanningStrength:
		{
			ChangeValue(set: false);
			int currentIndex5 = contentData.currentIndex;
			SetValueTextPercent(currentIndex5);
			if (contentData.changedValue)
			{
				scrConductor.GlobalPanningStrength = (float)currentIndex5 * 0.01f;
			}
			Persistence.SetPanningStrength(currentIndex5);
			break;
		}
		case PauseContentName.PanningStrength2P:
		{
			ChangeValue(set: false);
			int currentIndex4 = contentData.currentIndex;
			SetValueTextPercent(currentIndex4);
			Persistence.SetPanningStrength2P(currentIndex4);
			if (contentData.changedValue)
			{
				RDInput.SetPlayerSoundPanning();
			}
			break;
		}
		case PauseContentName.PanningInvert:
		{
			ChangeValue();
			bool currentBoolValue6 = contentData.CurrentBoolValue;
			if (contentData.changedValue)
			{
				scrConductor.GlobalPanningInvert = (currentBoolValue6 ? (-1f) : 1f);
			}
			Persistence.SetPanningInvert(currentBoolValue6);
			break;
		}
		case PauseContentName.BeatsoundPitchVariance:
		{
			ChangeValue(set: false);
			int currentIndex3 = contentData.currentIndex;
			SetValueTextPercent(currentIndex3);
			if (contentData.changedValue)
			{
				scrConductor.BeatsoundPitchVarianceMultiplier = 1f - (float)currentIndex3 * 0.01f;
			}
			Persistence.SetBeatsoundPitchVariance(currentIndex3);
			break;
		}
		case PauseContentName.BeatsoundPitchMode:
		{
			ChangeValue();
			bool currentBoolValue = contentData.CurrentBoolValue;
			if (contentData.changedValue)
			{
				scrConductor.BeatsoundPitchMode = currentBoolValue;
			}
			Persistence.SetBeatsoundPitchMode(currentBoolValue);
			break;
		}
		case PauseContentName.HeartExplosionsGlossary:
		case PauseContentName.RowInfoCuesGlossary:
		case PauseContentName.BeatsoundsGlossary:
		case PauseContentName.HitsoundsGlossary:
		case PauseContentName.HeartCracksGlossary:
		case PauseContentName.ClassicCuesGlossary:
		case PauseContentName.OneshotCuesGlossary:
			ChangeValue();
			break;
		case PauseContentName.EnableExtraDifficultyOptions:
			ChangeValue();
			Persistence.SetExtraDifficultyOptions(contentData.CurrentBoolValue);
			if (!contentData.CurrentBoolValue)
			{
				if (Persistence.GetDefibrillatorP1() == DefibMode.VeryEasy || Persistence.GetDefibrillatorP1() == DefibMode.Unmissable)
				{
					Persistence.SetDefibrillatorP1(DefibMode.Easy);
				}
				if (Persistence.GetDefibrillatorP2() == DefibMode.VeryEasy || Persistence.GetDefibrillatorP2() == DefibMode.Unmissable)
				{
					Persistence.SetDefibrillatorP2(DefibMode.Easy);
				}
			}
			break;
		case PauseContentName.EnableMoreVolumeSettings:
		{
			ChangeValue();
			bool currentBoolValue10 = contentData.CurrentBoolValue;
			if (contentData.changedValue && !currentBoolValue10)
			{
				Persistence.SetBeatsoundsVolume(10);
				Persistence.SetHitsoundsVolume(10);
				Persistence.SetCountingVoiceVolume(10);
				Persistence.SetCuesoundsVolume(10);
				Persistence.SetFeedbackVolume(10);
				scrGameManager.LoadSavedVolumes();
			}
			Persistence.SetMoreVolumeSettings(currentBoolValue10);
			break;
		}
		case PauseContentName.WindowMovement:
			ChangeValue();
			RDC.UpdateWindowMovement(contentData.currentWindowMovementValue);
			break;
		case PauseContentName.StreamerMode:
			ChangeValue();
			RDC.UpdateWindowMovement(contentData.currentWindowMovementValue);
			break;
		case PauseContentName.AudibleSkipBeats:
			ChangeValue();
			Persistence.SetAudibleSkipBeats(contentData.CurrentBoolValue);
			break;
		case PauseContentName.TapsOnly:
		{
			ChangeValue();
			bool currentBoolValue8 = contentData.CurrentBoolValue;
			if (contentData.changedValue)
			{
				scnGame.tapsOnly = currentBoolValue8;
			}
			Persistence.SetTapsOnly(currentBoolValue8);
			break;
		}
		case PauseContentName.DrumMode:
		{
			ChangeValue();
			bool currentBoolValue3 = contentData.CurrentBoolValue;
			if (contentData.changedValue)
			{
				scnGame.drumMode = currentBoolValue3;
				scnGame.tapsOnly = currentBoolValue3 || Persistence.GetTapsOnly();
				if (base.game != null)
				{
					base.game.HoldDurationToSkipCutscene = base.conductor.NearestBPMMultiple(0.59f * (float)((!currentBoolValue3) ? 1 : 2));
				}
			}
			Persistence.SetDrumMode(currentBoolValue3);
			break;
		}
		case PauseContentName.AccessibilityScope:
			ChangeValue();
			Persistence.SetAccessibilityScope(contentData.currentAccessibilityScopeValue);
			break;
		case PauseContentName.FlashIntensity:
		{
			ChangeValue(set: false);
			int currentIndex2 = contentData.currentIndex;
			int valueTextPercent = ((currentIndex2 == 0) ? 10 : currentIndex2);
			SetValueTextPercent(valueTextPercent);
			if (contentData.changedValue)
			{
				RDBase.Vfx.flashIntensity = (float)currentIndex2 * 0.01f;
			}
			Persistence.SetFlashIntensity(currentIndex2);
			break;
		}
		case PauseContentName.BackgroundDimming:
		{
			ChangeValue(set: false);
			int currentIndex = contentData.currentIndex;
			SetValueTextPercent(currentIndex);
			if (contentData.changedValue)
			{
				RDBase.Vfx.backgroundDimming = (float)currentIndex * 0.01f;
			}
			Persistence.SetBackgroundDimming(currentIndex);
			break;
		}
		case PauseContentName.ReducedFlash:
			ChangeValue();
			Persistence.SetReducedFlash(contentData.CurrentBoolValue);
			break;
		case PauseContentName.Narration:
			ChangeValue();
			break;
		case PauseContentName.NarrationDelayed:
			ChangeValue();
			break;
		case PauseContentName.SpeechRate:
		{
			ChangeValue();
			int currentIndex7 = contentData.currentIndex;
			Narration.SetSpeed(currentIndex7);
			Persistence.SetSpeechRate(currentIndex7);
			if (currentIndex7 == 0)
			{
				SetValueTextCustom(currentIndex7.ToString(), RDString.Get("pauseMenu.levelDetail.narrationSystemValue"));
			}
			break;
		}
		case PauseContentName.SpeechVolume:
		{
			ChangeValue();
			int currentIndex6 = contentData.currentIndex;
			Narration.SetVolume(currentIndex6);
			Persistence.SetSpeechVolume(currentIndex6);
			if (currentIndex6 == 11)
			{
				SetValueTextCustom(currentIndex6.ToString(), RDString.Get("pauseMenu.levelDetail.narrationSystemValue"));
			}
			break;
		}
		case PauseContentName.NarrateDescriptions:
			ChangeValue();
			Persistence.SetNarrateDescriptions(contentData.CurrentBoolValue);
			scrConductor.PlayImmediately(contentData.CurrentBoolValue ? "sndNarrationDescriptionEnabled" : "sndNarrationDescriptionDisabled", 1f, RDUtils.GetMixerGroup("InterfaceSoundsParent"), 1f, 0f, isBeatSound: false, ignoreListenerPause: true);
			break;
		case PauseContentName.NarrateSubtitles:
			ChangeValue();
			Persistence.SetNarrateSubtitles(contentData.CurrentBoolValue);
			scrConductor.PlayImmediately(contentData.CurrentBoolValue ? "sndNarrationSubtitlesEnabled" : "sndNarrationSubtitlesDisabled", 1f, RDUtils.GetMixerGroup("InterfaceSoundsParent"), 1f, 0f, isBeatSound: false, ignoreListenerPause: true);
			break;
		case PauseContentName.LevelSelectReportPosition:
			ChangeValue();
			Persistence.SetLevelSelectReportPosition(contentData.currentReportPositionValue);
			break;
		case PauseContentName.OtherMenusReportPosition:
			ChangeValue();
			Persistence.SetOtherMenusReportPosition(contentData.currentReportPositionValue);
			break;
		case PauseContentName.ForceNarrateCustomLevels:
			ChangeValue();
			Persistence.SetForceNarrateCustomLevels(contentData.CurrentBoolValue);
			break;
		case PauseContentName.ForceSAPINarration:
			ChangeValue();
			break;
		case PauseContentName.InputOffsetP1:
		case PauseContentName.InputOffsetP2:
		case PauseContentName.VisualOffset:
		{
			float num = (float)int.Parse(ChangeValue(set: false)) / 1000f;
			SetValueTextCustom(num.ToString(), num.ToMilliseconds());
			if (contentData.changedValue)
			{
				if (contentData.name == PauseContentName.InputOffsetP1)
				{
					RDCalibration.calibration_i = num;
				}
				else if (contentData.name == PauseContentName.InputOffsetP2)
				{
					RDCalibration.calibration_i_P2_internal = num;
				}
				else if (contentData.name == PauseContentName.VisualOffset)
				{
					RDCalibration.calibration_v = num;
				}
				Persistence.SetCalibrationValues(RDCalibration.calibration_v, RDCalibration.calibration_i, RDCalibration.calibration_i_P2_internal, RDCalibration.latency);
				if (base.currentSceneName == "scnEditor" && base.editor.calibrationPanel.gameObject.activeInHierarchy)
				{
					base.editor.calibrationPanel.GetComponent<InspectorPanel_Calibration>().UpdateUI();
				}
			}
			break;
		}
		case PauseContentName.FrameOffsets:
		{
			ChangeValue();
			bool currentBoolValue7 = contentData.CurrentBoolValue;
			if (contentData.changedValue)
			{
				GC.showAbsoluteOffsets = !GC.showAbsoluteOffsets;
			}
			Persistence.SetFrameOffsets(currentBoolValue7);
			break;
		}
		case PauseContentName.ErrorMargins:
		{
			ChangeValue();
			bool currentBoolValue5 = contentData.CurrentBoolValue;
			if (contentData.changedValue)
			{
				GC.d_showMarginsNumerically = !GC.d_showMarginsNumerically;
			}
			Persistence.SetErrorMargins(currentBoolValue5);
			break;
		}
		case PauseContentName.ShowStartupWarnings:
			ChangeValue();
			Persistence.SetShowStartupWarnings(contentData.CurrentBoolValue);
			break;
		case PauseContentName.InstantDialogue:
		{
			ChangeValue();
			bool currentBoolValue4 = contentData.CurrentBoolValue;
			if (contentData.changedValue)
			{
				RDBase.debugSettings.InstantDialogue = currentBoolValue4;
			}
			Persistence.SetInstantDialogue(currentBoolValue4);
			break;
		}
		case PauseContentName.FastMenu:
		{
			ChangeValue();
			bool currentBoolValue2 = contentData.CurrentBoolValue;
			if (contentData.changedValue)
			{
				RDBase.debugSettings.SkipMenuTransitions = currentBoolValue2;
			}
			Persistence.SetFastMenu(currentBoolValue2);
			break;
		}
		case PauseContentName.DiscordPresence:
			ChangeValue();
			Persistence.SetDiscordPresence(contentData.currentDiscordPresenceValue);
			RDRichPresence.Instance?.UpdatePresence();
			break;
		case PauseContentName.EditorAlwaysShowTooltips:
			ChangeValue();
			Persistence.SetLevelEditorAlwaysShowTooltips(contentData.CurrentBoolValue);
			if ((bool)base.editor && contentData.changedValue)
			{
				base.editor.alwaysShowTooltips = contentData.CurrentBoolValue;
			}
			break;
		case PauseContentName.EditorUIVolume:
			ChangeValue();
			if (contentData.changedValue)
			{
				currentEditorVolume = int.Parse(valueText.text);
			}
			Persistence.SetLevelEditorVolume(currentEditorVolume);
			break;
		case PauseContentName.EditorEnableUndo:
			ChangeValue();
			Persistence.SetUndoEnabled(contentData.CurrentBoolValue);
			if ((bool)base.editor && contentData.changedValue)
			{
				base.editor.undoEnabled = contentData.CurrentBoolValue;
			}
			break;
		case PauseContentName.EditorEnableComments:
			ChangeValue();
			Persistence.SetCommentPlaybackEnabled(contentData.CurrentBoolValue);
			if ((bool)base.editor && contentData.changedValue)
			{
				base.editor.commentPlaybackEnabled = contentData.CurrentBoolValue;
			}
			break;
		case PauseContentName.EditorWindowTab:
			ChangeValue();
			Persistence.SetWindowTabEnabled(contentData.CurrentBoolValue);
			if ((bool)base.editor && contentData.changedValue)
			{
				base.editor.TryHideWindowTab();
			}
			break;
		default:
			Debug.Log("contentData.name not found or set, check data: " + base.gameObject.name);
			break;
		}
		PauseModeName modeName = pauseMenu.currentMode.CurrentCategory.modeName;
		if ((modeName == PauseModeName.AudioSettings || modeName == PauseModeName.AdvancedAudioSettings) && contentData.changedValue)
		{
			scrGameManager.ApplyLoadedVolumes();
			PauseMenuAudioPreview.PlayPreviewAudio(contentData.name);
		}
		PauseMenuMode.CheckCJKText(valueText);
		float pitch = 1f;
		float indexPercentage = contentData.IndexPercentage;
		if (0f <= indexPercentage && indexPercentage <= 1f)
		{
			pitch = Mathf.Lerp(0.5f, 1f, indexPercentage);
		}
		else if (contentData.valueType == PauseContentValueType.Bool && !contentData.CurrentBoolValue)
		{
			pitch = 0.5f;
		}
		scrConductor.PlayImmediately("sndPagerValue", 1f, RDUtils.GetMixerGroup("PauseMenu"), pitch, 0f, isBeatSound: false, ignoreListenerPause: true);
		string text = "pauseMenu.levelDetail." + contentData.name;
		if (contentData.name == PauseContentName.WindowMovement && PauseMenuContentData.usesConsoleWindowMovement)
		{
			text += "Console";
		}
		string description = RDString.Get(text);
		string settingDetailValue = pauseMenu.GetSettingDetailValue(contentData, this);
		pauseMenu.UpdateLevelDetail(description, settingDetailValue, contentData.needsConfirmation);
		string description2 = null;
		NarrationActionName[] actions;
		if (contentData.needsConfirmation)
		{
			actions = new NarrationActionName[3]
			{
				NarrationActionName.ToConfirm,
				NarrationActionName.ToChangeHorizontal,
				NarrationActionName.ToGoBack
			};
			description2 = RDString.Get("pauseMenu.levelDetail.pressToConfirm");
		}
		else
		{
			actions = new NarrationActionName[2]
			{
				NarrationActionName.ToChangeHorizontal,
				NarrationActionName.ToGoBack
			};
		}
		Narration.Say(pauseMenu.usingOptionDetail ? settingDetailValue.RemoveRichTextColorTags() : valueText.text, NarrationCategory.Navigation, -1, -1, ElementType.None, null, description2, flipCategoryQueueBehaviour: false, actions);
		if (contentData.changedValue)
		{
			Arrow arrow = ((direction == 1) ? rightArrow : leftArrow);
			arrow.rect.DOKill(complete: true);
			arrow.rect.DOAnchorPosX(arrow.startPos.x + 2f * (float)arrow.direction, 0.05f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.Linear)
				.SetUpdate(isIndependentUpdate: true);
			if (contentData.name != PauseContentName.Fullscreen)
			{
				rightArrow.rect.gameObject.SetActive(contentData.CanChangeRight());
				leftArrow.rect.gameObject.SetActive(contentData.CanChangeLeft());
			}
		}
		string ChangeValue(bool set = true)
		{
			valueKey = contentData.ChangeValue(direction, shiftPressed, out var isKey);
			if (set)
			{
				SetValueText(valueKey, isKey);
			}
			return valueKey;
		}
	}

	void IPointerDownHandler.OnPointerDown(PointerEventData data)
	{
		RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, data.position, pauseMenu.pauseMenuCamera, out var localPoint);
		int num = ((localPoint.x >= rectTransform.sizeDelta.x - 10f) ? 1 : ((localPoint.x <= 10f) ? (-1) : 0));
		if (num != 0)
		{
			skipNextInitialize = true;
			ChangeContentValue(num, Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
		}
	}
}
