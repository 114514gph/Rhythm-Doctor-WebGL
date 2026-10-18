using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SA.GoogleDoc;
using UnityEngine;

public static class Persistence
{
	public const int SlotCount = 3;

	private const string CustomDialogueFolderKey = "customDialoguesFolder";

	private const string DidCalibrationKey = "did_calibration";

	private const string LanguageKey = "language";

	private const string ResolutionScaleKey = "resolution_scale";

	private const string VSyncKey = "VSync";

	private const string VoiceLanguageKey = "voiceLanguage";

	private const string HighQualityKey = "high_quality";

	public const string LastFilePathKey = "LastFilePathKey";

	private const string RhythmWeightlifterUnlockedKey = "rw_last_level_unlocked";

	private const string RhythmWeightlifterConsistencyKey = "rw_consistency_";

	private const string RhythmWeightlifterSpeedKey = "rw_speed_";

	private const string RhythmWeightlifterScoreKey = "rw_score_";

	private const string RhythmWeightlifterBestInfiniteRoundKey = "rw_best_infinite_round";

	private const string LastAnalyticsUpdateKey = "last_analytics_update";

	private const string OBSFirstDetectedKey = "obs_first_detected";

	public const string HasWindowDanceChangedResolutionKey = "windowdance_changed_resolution";

	public const string IsFullscreen = "is_fullscreen";

	public const string WindowXPosition = "window_x";

	public const string WindowYPosition = "window_y";

	public const string ResolutionWidthKey = "resolution_width";

	public const string ResolutionHeightKey = "resolution_height";

	private const string AcceptedAgreementKey = "acceptedAgreement";

	private const string SawWindowTabKey = "saw_window_tab_alert";

	private const string UndoEnabledKey = "undo_enabled";

	private const string WindowEnabledKey = "show_window_tab";

	private const string CommentPlaybackEnabledKey = "comment_playback_enabled";

	private const string AdvancedEditorKey = "level_editor_advanced_features";

	private const string LevelEditorAlwaysShowTooltipsKey = "level_editor_always_show_tooltips";

	private const string LevelEditorScaleFactorKey = "level_editor_scale_factor";

	private const string LevelEditorGamePreviewScaleKey = "level_editor_game_preview_scale";

	private const string LevelEditorTimelineHeight = "level_editor_game_timeline_height";

	private const string LevelEditorVolumeKey = "level_editor_volume";

	private const string LevelEditorWindowMovementKey = "level_editor_window_movement";

	private const string LevelEditorCollapseSoundEventListKey = "level_editor_collapse_sound_event_list";

	private const string LevelEditorCollapseActionEventListKey = "level_editor_collapse_action_event_list";

	private const string GlobalVolumeKey = "global_volume";

	private const string MusicVolumeKey = "music_volume";

	private const string GameplayVolumeKey = "gameplay_volume";

	private const string BeatsoundsVolumeKey = "beatsounds_volume";

	private const string HitsoundsVolumeKey = "hitsounds_volume";

	private const string CountingVoiceVolumeKey = "counting_voice_volume";

	private const string CuesoundsVolumeKey = "cuesounds_volume";

	private const string FeedbackVolumeKey = "feedback_volume";

	private const string InterfaceVolumeKey = "interface_volume";

	private const string DialogueVolumeKey = "dialogue_volume";

	private const string AmbienceVolumeKey = "ambience_volume";

	private const string MuteInBackgoundKey = "mute_in_background";

	private const string PanningStrengthKey = "panning_strength";

	private const string PanningStrength2PKey = "panning_strength_2P";

	private const string PanningInvertKey = "panning_invert";

	private const string BeatsoundPitchVarianceKey = "beatsound_pitch_variance";

	private const string BeatsoundPitchModeKey = "beatsound_pitch_mode";

	private const string DefibrillatorP1Key = "defibrillator_p1";

	private const string DefibrillatorP2Key = "defibrillator_p2";

	private const string SwapP1AndP2ControlsKey = "swap_p1_and_p2_controls";

	private const string ExtraDifficultyOptionsKey = "extra_difficulty_options";

	private const string MoreVolumeSettingsKey = "more_volume_settings";

	private const string AccessibilityScopeKey = "accessibility_scope";

	private const string WindowMovementKey = "window_movement";

	private const string ReducePixelFontUsageKey = "reduce_pixel_font_usage";

	private const string AudibleSkipBeatsKey = "audible_skip_beats";

	private const string FlashIntensityKey = "flash_intensity";

	private const string BackgroundDimmingKey = "background_dimming";

	private const string ReducedFlashKey = "reduced_flash";

	private const string TapsOnlyKey = "taps_only";

	private const string DrumModeKey = "drum_mode";

	private const string NarrationKey = "narration";

	private const string SpeechRateKey = "speech_rate";

	private const string SpeechVolumeKey = "speech_volume";

	private const string LevelSelectReportPositionKey = "level_select_report_position";

	private const string OtherMenusReportPositionKey = "other_menus_report_position";

	private const string NarrateDescriptionsKey = "narrate_descriptions";

	private const string NarrateSubtitlesKey = "narrate_subtitles";

	private const string ForceNarrateCustomLevelsKey = "force_narrate_custom_levels";

	private const string ForceSAPINarrationKey = "force_sapi_narration";

	private const string CalibrationVisualKey = "calibration_v";

	private const string CalibrationInputKey = "calibration_i";

	private const string CalibrationInputP2Key = "calibration_i_P2";

	private const string CalibrationLatencyKey = "latency";

	private const string FrameOffsetsKey = "frame_offsets";

	private const string ErrorMarginsKey = "error_margins";

	private const string ShowStartupWarningsKey = "show_startup_warnings";

	private const string InstantDialogueKey = "instant_dialogue";

	private const string FastMenuKey = "fast_menu";

	private const string ForceDesktopFeatureSet = "force_non_steam_deck_mode";

	private const string ForceFirstTimeSetupKey = "force_first_time_setup";

	private const string DiscordPresenceKey = "discord_presence";

	private const string SwitchFlagKey = "switch";

	private const string CustomButtonFlagKey = "customButton";

	private const string PlayTutorialKey = "playTutorial";

	public const string rdlevelToOpenKey = "rdlevelToOpen";

	public const string LocalizationPasswordKey = "localizationPassword";

	public const string SwitcherCustomLevelKey = "switcherCustomLevel";

	public const string SwitcherTwoPlayerKey = "switcherTwoPlayer";

	public const string SwitcherLoadTutorialKey = "switcherLoadTutorial";

	public const string SwitcherGameSpeedKey = "switcherGameSpeed";

	private const string RenderTextureScaleKey = "render_texture_scale";

	private const string JustPlayedLevelKey = "just_finished_level";

	private const string GameResultKey = "game_result";

	private const string PlayedPassedLevelCutsceneKey = "played_passed_level_cutscene";

	private const string DefaultResolutionKey = "default_resolution";

	private const string FirstTimePlayingKey = "first_time";

	private const string MistakeOffsetKey = "mistake_offset";

	private const string MistakeCountKey = "mistake_count";

	private const string FeedbackCounterKey = "feedback_counter";

	private const string InsultCounterKey = "insult_counter";

	private const string IanDesktopLoginKey = "ian_desktop_login";

	private const string RhythmStackerScoreKey = "rhythmstacker_score";

	private const string TempresScoreKey = "tempres_score";

	private const string IsGameDoneKey = "is_game_done";

	private const string PlayedPostAct2CutsceneKey = "played_post_act_2_cutscene";

	private const string PlayedPostAct3CutsceneKey = "played_post_act_3_cutscene";

	private const string PlayedPostAct4CutsceneKey = "played_post_act_4_cutscene";

	private const string PlayedPreAct5CutsceneKey = "played_pre_act_5_cutscene";

	private const string PlayedPreAct6CutsceneKey = "played_pre_act_6_cutscene";

	private const string PlayedPreBitternessCutsceneKey = "played_pre_bitterness_cutscene";

	private const string PlayedHaileyDuetIntroductionKey = "played_haileyduet_introduction";

	private const string PlayedRooftopCutsceneKey = "played_pre_finale_cutscene";

	private const string PlayedAct6IntroKey = "played_act_6_start";

	private const string PaigeStaysKey = "paige_stays";

	private const string DownloadedLevelsPathKey = "downloaded_levels_path";

	private const string LevelRankSuffix = "_rank";

	private const string LevelScoreSuffix = "_score";

	private const string SpeedModifiersUnlockedKey = "speed_modifiers_unlocked";

	public const string CustomLevelPrefix = "CustomLevel_";

	public static ArmSkin[,] skins = new ArmSkin[3, 2];

	public static int currentSlotIndex;

	private const int GeneralPrefsIndex = -1;

	public static ArmSkin p1Skin => skins[currentSlotIndex, 0];

	public static ArmSkin p2Skin => skins[currentSlotIndex, 1];

	public static PlayerPrefsJson generalPrefs => PlayerPrefsJson.Get(PlayerPrefsJson.FileType.Settings);

	public static PlayerPrefsJson[] slotPrefs => new PlayerPrefsJson[3]
	{
		PlayerPrefsJson.Get(PlayerPrefsJson.FileType.Slot0),
		PlayerPrefsJson.Get(PlayerPrefsJson.FileType.Slot1),
		PlayerPrefsJson.Get(PlayerPrefsJson.FileType.Slot2)
	};

	public static PlayerPrefsJson currentSlot => slotPrefs[currentSlotIndex];

	public static string DataPath => Application.persistentDataPath;

	public static void DeleteRankLevelData()
	{
		List<string> list = new List<string>();
		foreach (string key in currentSlot.dict.Keys)
		{
			RDBaseDllDummy.printes("key: " + key);
			if (key.EndsWith("_rank_" + currentSlotIndex))
			{
				("removing: " + key).printe();
				list.Add(key);
			}
		}
		foreach (string item in list)
		{
			generalPrefs.Remove(item);
		}
		SaveAll();
	}

	public static void DeleteSavedData()
	{
		generalPrefs.Delete();
		for (int i = 0; i < 3; i++)
		{
			DeleteSlotData(i);
		}
	}

	public static void DeleteSlotData(int slot)
	{
		RDFile.Delete(ArmSkin.GetDrawingPath(RDPlayer.P1, slot));
		RDFile.Delete(ArmSkin.GetDrawingPath(RDPlayer.P2, slot));
		LoadArmSkin(slot, RDPlayer.P1);
		LoadArmSkin(slot, RDPlayer.P2);
		slotPrefs[slot].Delete();
	}

	public static void DeleteSlotDataWithPrefix(int slotIndex, string prefix)
	{
		Dictionary<string, object> dict = slotPrefs[slotIndex].dict;
		string[] array = dict.Keys.ToArray();
		foreach (string text in array)
		{
			if (text.StartsWith(prefix))
			{
				dict.Remove(text);
			}
		}
		slotPrefs[slotIndex].Save();
	}

	private static string GetCustomLevelKey(string hash, float speed = 1f)
	{
		string text = ((speed > 1.01f) ? "chili" : ((speed < 0.99f) ? "ice" : "normal"));
		return "CustomLevel_" + hash + "_" + text;
	}

	public static void SetDefaultResolution(bool state)
	{
		generalPrefs.Set("default_resolution", state);
		generalPrefs.Save();
	}

	public static void SetSwitchFlag(bool itsOn)
	{
		generalPrefs.Set("switch", itsOn);
	}

	public static void SetCustomButtonFlag(bool itsOn)
	{
		generalPrefs.Set("customButton", itsOn);
	}

	public static void SetVSync(bool state)
	{
		generalPrefs.Set("VSync", state);
	}

	public static void SetDownloadedLevelsPath(string path)
	{
		generalPrefs.Set("downloaded_levels_path", path);
	}

	public static void SetHighQuality(bool highQuality)
	{
		generalPrefs.Set("high_quality", highQuality);
	}

	public static void SetCustomDialogueFolder(string folder)
	{
		generalPrefs.Set("customDialoguesFolder", folder);
	}

	public static void SetVoiceLanguage(SystemLanguage voiceLanguage)
	{
		generalPrefs.Set("voiceLanguage", voiceLanguage);
	}

	public static void SetDefibrillatorP1(DefibMode defibMode)
	{
		generalPrefs.Set("defibrillator_p1", defibMode);
	}

	public static void SetDefibrillatorP2(DefibMode defibMode)
	{
		generalPrefs.Set("defibrillator_p2", defibMode);
	}

	public static void SetSwapP1AndP2Controls(bool swap)
	{
		generalPrefs.Set("swap_p1_and_p2_controls", swap);
	}

	public static void SetResolutionScale(int scale)
	{
		generalPrefs.Set("resolution_scale", scale);
		generalPrefs.Save();
	}

	public static void SetWindowXPosition(int x)
	{
		generalPrefs.Set("window_x", x);
	}

	public static void SetWindowYPosition(int y)
	{
		generalPrefs.Set("window_y", y);
	}

	public static void SetResolutionWidth(int width)
	{
		generalPrefs.Set("resolution_width", width);
	}

	public static void SetResolutionHeight(int height)
	{
		generalPrefs.Set("resolution_height", height);
	}

	public static void SetHasWindowDanceChangedResolution(bool state)
	{
		generalPrefs.Set("windowdance_changed_resolution", state);
	}

	public static void SetIsFullscreen(bool state)
	{
		generalPrefs.Set("is_fullscreen", state);
	}

	public static void SetAcceptedAgreement(bool accepted)
	{
		generalPrefs.Set("acceptedAgreement", accepted);
		generalPrefs.Save();
	}

	public static void SetSawWindowTab(bool accepted)
	{
		generalPrefs.Set("saw_window_tab_alert", accepted);
		generalPrefs.Save();
	}

	public static void SetUndoEnabled(bool undoEnabled)
	{
		generalPrefs.Set("undo_enabled", undoEnabled);
		generalPrefs.Save();
	}

	public static void SetWindowTabEnabled(bool windowEnabled)
	{
		generalPrefs.Set("show_window_tab", windowEnabled);
		generalPrefs.Save();
	}

	public static void SetCommentPlaybackEnabled(bool enabled)
	{
		generalPrefs.Set("comment_playback_enabled", enabled);
	}

	public static void SetAdvancedEditorFeatures(bool enabled)
	{
		generalPrefs.Set("level_editor_advanced_features", enabled);
	}

	public static void SetLevelEditorScaleFactor(int scale)
	{
		generalPrefs.Set("level_editor_scale_factor", scale);
	}

	public static void SetLevelEditorGamePreviewScale(float scale)
	{
		generalPrefs.Set("level_editor_game_preview_scale", scale);
	}

	public static void SetLevelEditorTimelineHeight(int height)
	{
		generalPrefs.Set("level_editor_game_timeline_height", height);
	}

	public static void SetLevelEditorAlwaysShowTooltips(bool enabled)
	{
		generalPrefs.Set("level_editor_always_show_tooltips", enabled);
		generalPrefs.Save();
	}

	public static void SetLevelEditorCollapseSoundEventList(bool collapse)
	{
		generalPrefs.Set("level_editor_collapse_sound_event_list", collapse);
	}

	public static void SetLevelEditorCollapseActionEventList(bool collapse)
	{
		generalPrefs.Set("level_editor_collapse_action_event_list", collapse);
	}

	public static void SetLevelEditorWindowMovement(WindowMovement wm)
	{
		generalPrefs.Set("level_editor_window_movement", wm);
	}

	public static void SetGlobalVolume(int volume)
	{
		generalPrefs.Set("global_volume", volume);
	}

	public static void SetMusicVolume(int volume)
	{
		generalPrefs.Set("music_volume", volume);
	}

	public static void SetGameplayVolume(int volume)
	{
		generalPrefs.Set("gameplay_volume", volume);
	}

	public static void SetBeatsoundsVolume(int volume)
	{
		generalPrefs.Set("beatsounds_volume", volume);
	}

	public static void SetHitsoundsVolume(int volume)
	{
		generalPrefs.Set("hitsounds_volume", volume);
	}

	public static void SetCountingVoiceVolume(int volume)
	{
		generalPrefs.Set("counting_voice_volume", volume);
	}

	public static void SetCuesoundsVolume(int volume)
	{
		generalPrefs.Set("cuesounds_volume", volume);
	}

	public static void SetFeedbackVolume(int volume)
	{
		generalPrefs.Set("feedback_volume", volume);
	}

	public static void SetInterfaceVolume(int volume)
	{
		generalPrefs.Set("interface_volume", volume);
	}

	public static void SetLevelEditorVolume(int volume)
	{
		generalPrefs.Set("level_editor_volume", volume);
	}

	public static void SetDialogueVolume(int volume)
	{
		generalPrefs.Set("dialogue_volume", volume);
	}

	public static void SetAmbienceVolume(int volume)
	{
		generalPrefs.Set("ambience_volume", volume);
	}

	public static void SetMuteInBackground(bool state)
	{
		generalPrefs.Set("mute_in_background", state);
	}

	public static void SetPanningStrength(int strength)
	{
		generalPrefs.Set("panning_strength", strength);
	}

	public static void SetPanningStrength2P(int strength)
	{
		generalPrefs.Set("panning_strength_2P", strength);
	}

	public static void SetPanningInvert(bool state)
	{
		generalPrefs.Set("panning_invert", state);
	}

	public static void SetBeatsoundPitchVariance(int variance)
	{
		generalPrefs.Set("beatsound_pitch_variance", variance);
	}

	public static void SetBeatsoundPitchMode(bool mode)
	{
		generalPrefs.Set("beatsound_pitch_mode", mode);
	}

	public static void SetExtraDifficultyOptions(bool enabled)
	{
		generalPrefs.Set("extra_difficulty_options", enabled);
	}

	public static void SetWindowMovement(WindowMovement wm)
	{
		generalPrefs.Set("window_movement", wm);
	}

	public static void SetAccessibilityScope(AccessibilityScope scope)
	{
		generalPrefs.Set("accessibility_scope", scope);
	}

	public static void SetReducePixelFontUsage(bool enabled)
	{
		generalPrefs.Set("reduce_pixel_font_usage", enabled);
		generalPrefs.Save();
	}

	public static void SetAudibleSkipBeats(bool enabled)
	{
		generalPrefs.Set("audible_skip_beats", enabled);
	}

	public static void SetFlashIntensity(int intensity)
	{
		generalPrefs.Set("flash_intensity", intensity);
	}

	public static void SetBackgroundDimming(int dim)
	{
		generalPrefs.Set("background_dimming", dim);
	}

	public static void SetMoreVolumeSettings(bool enabled)
	{
		generalPrefs.Set("more_volume_settings", enabled);
	}

	public static void SetNarration(bool enabled)
	{
		generalPrefs.Set("narration", enabled);
	}

	public static void SetSpeechRate(int rate)
	{
		generalPrefs.Set("speech_rate", rate);
	}

	public static void SetSpeechVolume(int volume)
	{
		generalPrefs.Set("speech_volume", volume);
	}

	public static void SetLevelSelectReportPosition(ReportPosition position)
	{
		generalPrefs.Set("level_select_report_position", position);
	}

	public static void SetOtherMenusReportPosition(ReportPosition position)
	{
		generalPrefs.Set("other_menus_report_position", position);
	}

	public static void SetNarrateDescriptions(bool enabled)
	{
		generalPrefs.Set("narrate_descriptions", enabled);
	}

	public static void SetNarrateSubtitles(bool enabled)
	{
		generalPrefs.Set("narrate_subtitles", enabled);
	}

	public static void SetForceNarrateCustomLevels(bool enabled)
	{
		generalPrefs.Set("force_narrate_custom_levels", enabled);
	}

	public static void SetForceSAPINarration(bool enabled)
	{
		generalPrefs.Set("force_sapi_narration", enabled);
	}

	public static void SetTapsOnly(bool enabled)
	{
		generalPrefs.Set("taps_only", enabled);
	}

	public static void SetDrumMode(bool enabled)
	{
		generalPrefs.Set("drum_mode", enabled);
	}

	public static void SetReducedFlash(bool enabled)
	{
		generalPrefs.Set("reduced_flash", enabled);
	}

	public static void SetFrameOffsets(bool enabled)
	{
		generalPrefs.Set("frame_offsets", enabled);
	}

	public static void SetErrorMargins(bool enabled)
	{
		generalPrefs.Set("error_margins", enabled);
	}

	public static void SetShowStartupWarnings(bool enabled)
	{
		generalPrefs.Set("show_startup_warnings", enabled);
	}

	public static void SetInstantDialogue(bool enabled)
	{
		generalPrefs.Set("instant_dialogue", enabled);
	}

	public static void SetFastMenu(bool enabled)
	{
		generalPrefs.Set("fast_menu", enabled);
	}

	public static void SetForceDesktopFeatureSet(bool enabled)
	{
		generalPrefs.Set("force_non_steam_deck_mode", enabled);
	}

	public static void SetForceFirstTimeSetup(bool enabled)
	{
		generalPrefs.Set("force_first_time_setup", enabled);
		generalPrefs.Save();
	}

	public static void SetDiscordPresence(DiscordPresence dp)
	{
		generalPrefs.Set("discord_presence", dp);
	}

	public static void SetRenderTextureScale(int scale)
	{
		generalPrefs.Set("render_texture_scale", scale);
	}

	public static void SetLanguage(SystemLanguage language)
	{
		generalPrefs.Set("language", language);
	}

	public static void SetDidCalibration(bool didCalibration)
	{
		generalPrefs.Set("did_calibration", didCalibration);
		generalPrefs.Save();
	}

	public static void SetOBSFirstDetected(bool OBSFirstDetected)
	{
		generalPrefs.Set("obs_first_detected", OBSFirstDetected);
		generalPrefs.Save();
	}

	public static void SetCustomLevelRank(string hash, Rank rank, float speed = 1f)
	{
		Rank oldRank = GetCustomLevelRank(hash, speed);
		if (IsBetterRank(rank, oldRank))
		{
			string customLevelKey = GetCustomLevelKey(hash, speed);
			generalPrefs.Set(customLevelKey, rank.ToString());
			generalPrefs.Save();
		}
	}

	public static void SetLastAnalyticsUpdate(int day)
	{
		generalPrefs.Set("last_analytics_update", day);
	}

	public static void SetRhythmWeightlifterLastLevelUnlocked(int index)
	{
		currentSlot.Set("rw_last_level_unlocked", index);
	}

	public static void SetRhythmWeightlifterConsistency(int index, int consistency)
	{
		currentSlot.Set(string.Format("{0}{1}", "rw_consistency_", index), consistency);
	}

	public static void SetRhythmWeightlifterSpeed(int index, int speed)
	{
		currentSlot.Set(string.Format("{0}{1}", "rw_speed_", index), speed);
	}

	public static void SetRhythmWeightlifterScore(int index, int score)
	{
		currentSlot.Set(string.Format("{0}{1}", "rw_score_", index), score);
	}

	public static void SetRhythmWeightlifterBestInfiniteRound(int roundNumber)
	{
		currentSlot.Set("rw_best_infinite_round", roundNumber);
	}

	public static bool GetDefaultResolution()
	{
		return generalPrefs.Get("default_resolution", defaultValue: false);
	}

	public static bool GetSwitchFlag()
	{
		return generalPrefs.Get("switch", defaultValue: false);
	}

	public static bool GetCustomButtonFlag()
	{
		return generalPrefs.Get("customButton", defaultValue: false);
	}

	public static SystemLanguage GetLanguage()
	{
		SystemLanguage systemLanguage = Application.systemLanguage;
		return generalPrefs.Get("language", systemLanguage);
	}

	public static SystemLanguage GetVoiceLanguage()
	{
		return generalPrefs.Get("voiceLanguage", GetLanguage());
	}

	public static bool GetHighQuality()
	{
		return generalPrefs.Get("high_quality", defaultValue: true);
	}

	public static string GetCustomDialogueFolder()
	{
		return generalPrefs.Get("customDialoguesFolder", string.Empty);
	}

	public static int GetResolutionScale()
	{
		return generalPrefs.Get("resolution_scale", 2);
	}

	public static bool GetIsFullscreen()
	{
		return generalPrefs.Get("is_fullscreen", defaultValue: false);
	}

	public static int GetWindowXPosition()
	{
		return generalPrefs.Get("window_x", 0);
	}

	public static int GetWindowYPosition()
	{
		return generalPrefs.Get("window_y", 0);
	}

	public static bool GetVSync()
	{
		return generalPrefs.Get("VSync", defaultValue: false);
	}

	public static string GetDownloadedLevelsPath()
	{
		return generalPrefs.Get("downloaded_levels_path", LevelValidation.CustomLevelsPath);
	}

	public static int GetResolutionWidth()
	{
		return generalPrefs.Get("resolution_width", 704);
	}

	public static int GetResolutionHeight()
	{
		return generalPrefs.Get("resolution_height", 396);
	}

	public static bool GetHasWindowDanceChangedResolution()
	{
		return generalPrefs.GetBool("windowdance_changed_resolution");
	}

	public static bool GetAcceptedAgreement()
	{
		return generalPrefs.Get("acceptedAgreement", defaultValue: false);
	}

	public static bool GetSawWindowTab()
	{
		return generalPrefs.Get("saw_window_tab_alert", defaultValue: false);
	}

	public static bool GetUndoEnabled()
	{
		return generalPrefs.Get("undo_enabled", defaultValue: true);
	}

	public static bool GetWindowTabEnabled()
	{
		return generalPrefs.Get("show_window_tab", defaultValue: false);
	}

	public static bool GetCommentPlaybackEnabled()
	{
		return generalPrefs.Get("comment_playback_enabled", defaultValue: true);
	}

	public static bool GetAdvancedEditorFeatures()
	{
		return generalPrefs.Get("level_editor_advanced_features", defaultValue: false);
	}

	public static int GetLevelEditorScaleFactor()
	{
		return generalPrefs.Get("level_editor_scale_factor", -1);
	}

	public static float GetLevelEditorGamePreviewScale()
	{
		return generalPrefs.Get("level_editor_game_preview_scale", 1f);
	}

	public static int GetLevelEditorTimelineHeight()
	{
		return generalPrefs.Get("level_editor_game_timeline_height", 4);
	}

	public static bool GetLevelEditorAlwaysShowTooltips()
	{
		return generalPrefs.Get("level_editor_always_show_tooltips", defaultValue: true);
	}

	public static int GetLevelEditorVolume()
	{
		return GetVolume("level_editor_volume");
	}

	public static bool GetLevelEditorCollapseSoundEventList()
	{
		return generalPrefs.Get("level_editor_collapse_sound_event_list", defaultValue: false);
	}

	public static bool GetLevelEditorCollapseActionEventList()
	{
		return generalPrefs.Get("level_editor_collapse_action_event_list", defaultValue: false);
	}

	public static WindowMovement GetLevelEditorWindowMovement()
	{
		WindowMovement windowMovement = generalPrefs.Get("level_editor_window_movement", scnBase.isLinux ? WindowMovement.Simulate : WindowMovement.Full);
		if (windowMovement == WindowMovement.Off)
		{
			windowMovement = WindowMovement.Simulate;
		}
		return windowMovement;
	}

	public static int GetVolume(string key, int DefaultVolume = 10)
	{
		return Mathf.Clamp(generalPrefs.Get(key, DefaultVolume), 0, 11);
	}

	public static int GetGlobalVolume()
	{
		return GetVolume("global_volume", 7);
	}

	public static int GetMusicVolume()
	{
		return GetVolume("music_volume");
	}

	public static int GetGameplayVolume()
	{
		return GetVolume("gameplay_volume");
	}

	public static int GetBeatsoundsVolume()
	{
		return GetVolume("beatsounds_volume");
	}

	public static int GetHitsoundsVolume()
	{
		return GetVolume("hitsounds_volume");
	}

	public static int GetCountingVoiceVolume()
	{
		return GetVolume("counting_voice_volume");
	}

	public static int GetCuesoundsVolume()
	{
		return GetVolume("cuesounds_volume");
	}

	public static int GetFeedbackVolume()
	{
		return GetVolume("feedback_volume");
	}

	public static int GetInterfaceVolume()
	{
		return GetVolume("interface_volume");
	}

	public static int GetDialogueVolume()
	{
		return GetVolume("dialogue_volume");
	}

	public static int GetAmbienceVolume()
	{
		return GetVolume("ambience_volume", 7);
	}

	public static bool GetMuteInBackground()
	{
		return generalPrefs.Get("mute_in_background", defaultValue: false);
	}

	public static int GetPanningStrength()
	{
		return Mathf.Clamp(generalPrefs.Get("panning_strength", 100), 0, 200);
	}

	public static int GetPanningStrength2P()
	{
		return Mathf.Clamp(generalPrefs.Get("panning_strength_2P", 80), 40, 100);
	}

	public static bool GetPanningInvert()
	{
		return generalPrefs.Get("panning_invert", defaultValue: false);
	}

	public static int GetBeatsoundPitchVariance()
	{
		return generalPrefs.Get("beatsound_pitch_variance", 5);
	}

	public static bool GetBeatsoundPitchMode()
	{
		return generalPrefs.Get("beatsound_pitch_mode", defaultValue: false);
	}

	public static bool GetExtraDifficultyOptions()
	{
		return generalPrefs.GetBool("extra_difficulty_options");
	}

	public static bool GetMoreVolumeSettings()
	{
		return generalPrefs.GetBool("more_volume_settings");
	}

	public static AccessibilityScope GetAccessibilityScope()
	{
		return generalPrefs.Get("accessibility_scope", AccessibilityScope.StoryLevelsOnly);
	}

	public static bool GetReducePixelFontUsage()
	{
		return generalPrefs.GetBool("reduce_pixel_font_usage");
	}

	public static bool GetAudibleSkipBeats()
	{
		return generalPrefs.Get("audible_skip_beats", defaultValue: false);
	}

	public static WindowMovement GetWindowMovement()
	{
		WindowMovement windowMovement = generalPrefs.Get("window_movement", WindowMovement.Full);
		if (windowMovement != WindowMovement.Off)
		{
			return windowMovement;
		}
		return WindowMovement.Simulate;
	}

	public static bool GetNarration()
	{
		return generalPrefs.Get("narration", defaultValue: true);
	}

	public static int GetSpeechRate()
	{
		return generalPrefs.Get("speech_rate", 0);
	}

	public static int GetSpeechVolume()
	{
		return generalPrefs.Get("speech_volume", 11);
	}

	public static ReportPosition GetLevelSelectReportPosition()
	{
		return generalPrefs.Get("level_select_report_position", ReportPosition.Once);
	}

	public static ReportPosition GetOtherMenusReportPosition()
	{
		return generalPrefs.Get("other_menus_report_position", ReportPosition.Once);
	}

	public static bool GetNarrateDescriptions()
	{
		return generalPrefs.Get("narrate_descriptions", defaultValue: true);
	}

	public static bool GetNarrateSubtitles()
	{
		return generalPrefs.Get("narrate_subtitles", defaultValue: true);
	}

	public static bool GetForceNarrateCustomLevels()
	{
		return generalPrefs.Get("force_narrate_custom_levels", defaultValue: false);
	}

	public static bool GetForceSAPINarration()
	{
		return generalPrefs.Get("force_sapi_narration", defaultValue: false);
	}

	public static bool GetTapsOnly()
	{
		return generalPrefs.Get("taps_only", defaultValue: false);
	}

	public static bool GetDrumMode()
	{
		return generalPrefs.Get("drum_mode", defaultValue: false);
	}

	public static bool GetReducedFlash()
	{
		return generalPrefs.Get("reduced_flash", defaultValue: false);
	}

	public static int GetFlashIntensity()
	{
		return Mathf.Clamp(generalPrefs.Get("flash_intensity", 100), 0, 100);
	}

	public static int GetBackgroundDimming()
	{
		return Mathf.Clamp(generalPrefs.Get("background_dimming", 0), 0, 100);
	}

	public static bool GetDidCalibration()
	{
		return generalPrefs.Get("did_calibration", defaultValue: false);
	}

	public static bool GetOBSFirstDetected()
	{
		return generalPrefs.Get("obs_first_detected", defaultValue: false);
	}

	public static DefibMode GetDefibrillatorP1()
	{
		return generalPrefs.Get("defibrillator_p1", DefibMode.Normal);
	}

	public static DefibMode GetDefibrillatorP2()
	{
		return generalPrefs.Get("defibrillator_p2", DefibMode.Normal);
	}

	public static bool GetSwapP1AndP2Controls()
	{
		return generalPrefs.Get("swap_p1_and_p2_controls", defaultValue: false);
	}

	public static bool GetFrameOffsets()
	{
		return generalPrefs.Get("frame_offsets", defaultValue: false);
	}

	public static bool GetErrorMargins()
	{
		return generalPrefs.Get("error_margins", defaultValue: false);
	}

	public static bool GetShowStartupWarnings()
	{
		return generalPrefs.Get("show_startup_warnings", defaultValue: true);
	}

	public static bool GetInstantDialogue()
	{
		return generalPrefs.Get("instant_dialogue", defaultValue: false);
	}

	public static bool GetFastMenu()
	{
		return generalPrefs.Get("fast_menu", defaultValue: false);
	}

	public static bool GetForceDesktopFeatureSet()
	{
		return generalPrefs.Get("force_non_steam_deck_mode", defaultValue: false);
	}

	public static bool GetForceFirstTimeSetup()
	{
		return generalPrefs.Get("force_first_time_setup", defaultValue: false);
	}

	public static DiscordPresence GetDiscordPresence()
	{
		return generalPrefs.Get("discord_presence", DiscordPresence.Full);
	}

	public static FeatureSet GetFeatureSet()
	{
		if (!DebugSettings.instance.RunningOnSteamDeck || GetForceDesktopFeatureSet() || !SteamIntegration.IsSteamInBigPictureMode)
		{
			return FeatureSet.Desktop;
		}
		return FeatureSet.SteamDeck;
	}

	public static int GetRenderTextureScale()
	{
		return generalPrefs.Get("render_texture_scale", 4);
	}

	public static int GetCustomLevelRank(string hash, float speed = 1f)
	{
		string customLevelKey = GetCustomLevelKey(hash, speed);
		string text = generalPrefs.Get(customLevelKey, (-2).ToString());
		if (text == (-2).ToString())
		{
			return -2;
		}
		return Rank.FromString(text);
	}

	public static int GetLastAnalyticsUpdate()
	{
		return generalPrefs.GetInt("last_analytics_update");
	}

	public static int GetRhythmWeightlifterLastLevel()
	{
		return currentSlot.Get("rw_last_level_unlocked", 0);
	}

	public static int GetRhythmWeightlifterConsistency(int index)
	{
		return currentSlot.Get(string.Format("{0}{1}", "rw_consistency_", index), 0);
	}

	public static int GetRhythmWeightlifterSpeed(int index)
	{
		return currentSlot.Get(string.Format("{0}{1}", "rw_speed_", index), 0);
	}

	public static int GetRhythmWeightlifterScore(int index)
	{
		return currentSlot.Get(string.Format("{0}{1}", "rw_score_", index), 0);
	}

	public static int GetBestInfiniteRound()
	{
		return currentSlot.Get("rw_best_infinite_round", 0);
	}

	public static void SetSpeedModifiersUnlocked(bool unlocked)
	{
		currentSlot.Set("speed_modifiers_unlocked", unlocked);
	}

	public static void SetPlayedPassedLevelCutscene(bool played)
	{
		currentSlot.Set("played_passed_level_cutscene", played);
	}

	public static void SetIsGameDone(bool state)
	{
		currentSlot.Set("is_game_done", state);
	}

	public static void SetFirstTimePlaying(bool state)
	{
		currentSlot.Set("first_time", state);
	}

	public static void SetPlayTutorial(bool state)
	{
		currentSlot.Set("playTutorial", state);
	}

	public static void SetGameResult(GameResult gameResult)
	{
		currentSlot.Set("game_result", gameResult.ToString());
	}

	public static void SetMistakeOffset(float mistakeOffset)
	{
		currentSlot.Set("mistake_offset", mistakeOffset);
	}

	public static void SetMistakeCount(float mistakes)
	{
		currentSlot.Set("mistake_count", mistakes);
	}

	public static void SetFeedbackCounter(int feedbackCounter)
	{
		currentSlot.Set("feedback_counter", feedbackCounter);
	}

	public static void SetInsultCounter(int insultCounter)
	{
		currentSlot.Set("insult_counter", insultCounter);
	}

	public static void SetIanDesktopLogin(bool ianDesktopLogin)
	{
		currentSlot.Set("ian_desktop_login", ianDesktopLogin);
	}

	public static void SetRhythmStackerScore(int stackerScore)
	{
		currentSlot.Set("rhythmstacker_score", stackerScore);
	}

	public static void SetTempresScore(float tempresScore)
	{
		currentSlot.Set("tempres_score", tempresScore);
	}

	public static void SetTriesToClearLevel(string level, int tries)
	{
		currentSlot.Set(level + "_tries", tries);
	}

	public static void SetPlayedPostAct2Cutscene(bool played)
	{
		currentSlot.Set("played_post_act_2_cutscene", played);
	}

	public static void SetPlayedPostAct3Cutscene(bool played)
	{
		currentSlot.Set("played_post_act_3_cutscene", played);
	}

	public static void SetPlayedPostAct4Cutscene(bool played)
	{
		currentSlot.Set("played_post_act_4_cutscene", played);
	}

	public static void SetPlayedPreAct5Cutscene(bool played)
	{
		currentSlot.Set("played_pre_act_5_cutscene", played);
	}

	public static void SetPlayedPreAct6Cutscene(bool played)
	{
		currentSlot.Set("played_pre_act_6_cutscene", played);
	}

	public static void SetPlayedHaileyDuetIntroduction(bool played)
	{
		currentSlot.Set("played_haileyduet_introduction", played);
	}

	public static void SetPlayedPreBitternessCutscene(bool played)
	{
		currentSlot.Set("played_pre_bitterness_cutscene", played);
	}

	public static void SetPlayedRooftopCutscene(bool played)
	{
		currentSlot.Set("played_pre_finale_cutscene", played);
	}

	public static void SetPlayedAct6Intro(bool played)
	{
		currentSlot.Set("played_act_6_start", played);
	}

	public static void SetPaigeEnding(bool paigeStays)
	{
		currentSlot.Set("paige_stays", paigeStays);
	}

	public static void SetLastPlayedLevel(Level level)
	{
		SetLastPlayedLevel(level.ToString());
	}

	public static void SetLastPlayedLevel(string level)
	{
		currentSlot.Set("just_finished_level", level);
	}

	public static void SetLocalizationPassword(string password)
	{
		generalPrefs.Set("localizationPassword", password);
		Reader.localizationPassword = password;
	}

	public static bool IsBetterRank(Rank newRank, Rank oldRank)
	{
		if ((int)newRank.ToNormal() <= (int)oldRank.ToNormal())
		{
			if ((int)newRank.ToNormal() == (int)oldRank.ToNormal())
			{
				return (int)newRank >= (int)oldRank;
			}
			return false;
		}
		return true;
	}

	public static void SetLevelRankInternal(Level level, Rank rank)
	{
		if (level != Level.None)
		{
			SetLevelRankInternal(level.ToString(), rank);
		}
	}

	private static void SetLevelRankInternal(string level, Rank rank)
	{
		if (level != null)
		{
			string key = GetLevelRankPrefix(twoPlayer: false) + level + "_rank";
			RDBaseDllDummy.printem("assigned " + level + " with rank " + rank.ToString());
			currentSlot.Set(key, rank.ToString());
		}
	}

	public static bool SetLevelRank(Level level, Rank rank, bool force = false, bool setResult = true)
	{
		if (level == Level.None)
		{
			return false;
		}
		return SetLevelRank(level.ToString(), rank, force, setResult);
	}

	public static bool SetLevelRank(string level, Rank rank, bool force = false, bool setResult = true)
	{
		Rank levelRank = GetLevelRank(level);
		bool flag = false;
		bool num = IsBetterRank(rank, levelRank) | force;
		if (num)
		{
			SetLevelRankInternal(level, rank);
			flag = true;
		}
		if (setResult)
		{
			if (!levelRank.passed && rank.passed)
			{
				SetGameResult(GameResult.LevelJustPassed);
			}
			if (level == Level.Montage2.ToString() && !levelRank.isNormalRank && rank.isNormalRank)
			{
				Debug.Log("Beaten 7-X2 for the first time, taking player to menu after Helping Hands.");
				scnGame.goToScnMenuAfterHelpingHands = true;
			}
			flag = true;
		}
		if (flag)
		{
			SaveCurrentSlot();
		}
		GiveLevelAchievement(LevelSelector.LevelFromString(level), rank, GC.twoPlayerMode);
		SteamIntegration.StoreStats();
		return num;
	}

	public static void SetLevelScore(string level, int score, bool force = false)
	{
		string text = GetLevelRankPrefix(twoPlayer: false) + level + "_score";
		RDBaseDllDummy.printem($"{text}: {score}");
		SpeedrunValues.score = score;
		if ((score > currentSlot.Get(text, 0)) | force)
		{
			currentSlot.Set(text, score);
		}
		if (level == Level.BeansHopper.ToString() && score == 0)
		{
			UnlockAchievement(Achievement.BeansHopperFail, storeStats: true);
		}
	}

	public static bool GetSpeedModifiersUnlocked()
	{
		return currentSlot.Get("speed_modifiers_unlocked", defaultValue: false);
	}

	public static int GetTriesToClearLevel(string level)
	{
		return currentSlot.Get(level.ToString() + "_tries", 0);
	}

	public static bool GetIsGameDone()
	{
		return currentSlot.Get("is_game_done", defaultValue: false);
	}

	public static bool GetPlayedPassedLevelCutscene()
	{
		return currentSlot.Get("played_passed_level_cutscene", defaultValue: false);
	}

	public static bool GetFirstTimePlaying()
	{
		return currentSlot.Get("first_time", defaultValue: true);
	}

	public static bool GetPlayTutorial()
	{
		return currentSlot.Get("playTutorial", defaultValue: true);
	}

	public static bool GetPlayedPostAct2Cutscene()
	{
		return currentSlot.Get("played_post_act_2_cutscene", defaultValue: false);
	}

	public static bool GetPlayedPostAct3Cutscene()
	{
		return currentSlot.Get("played_post_act_3_cutscene", defaultValue: false);
	}

	public static bool GetPlayedPostAct4Cutscene()
	{
		return currentSlot.Get("played_post_act_4_cutscene", defaultValue: false);
	}

	public static bool GetPlayedPreAct5Cutscene()
	{
		return currentSlot.Get("played_pre_act_5_cutscene", defaultValue: false);
	}

	public static bool GetPlayedPreAct6Cutscene()
	{
		return currentSlot.Get("played_pre_act_6_cutscene", defaultValue: false);
	}

	public static bool GetPlayedAct6Intro()
	{
		return currentSlot.Get("played_act_6_start", defaultValue: false);
	}

	public static bool GetPlayedHaileyDuetIntroduction()
	{
		return currentSlot.Get("played_haileyduet_introduction", defaultValue: false);
	}

	public static bool GetPlayedPreBitternessCutscene()
	{
		return currentSlot.Get("played_pre_bitterness_cutscene", defaultValue: false);
	}

	public static bool GetPlayedRooftopCutscene()
	{
		return currentSlot.Get("played_pre_finale_cutscene", defaultValue: false);
	}

	public static bool GetPaigeEnding()
	{
		return currentSlot.Get("paige_stays", defaultValue: false);
	}

	public static LevelState GetStateFromLevel(Level level, int specificSlot = -1)
	{
		return GetStateFromLevel(level.ToString(), specificSlot);
	}

	public static string GetLocalizationPassword()
	{
		return generalPrefs.Get("localizationPassword", string.Empty);
	}

	public static LevelState GetStateFromLevel(string level, int specificSlot = -1)
	{
		Rank levelRank = GetLevelRank(level, twoPlayer: false, specificSlot);
		if ((int)levelRank != -2)
		{
			if (!levelRank.passed)
			{
				return LevelState.NotPassed;
			}
			return LevelState.Passed;
		}
		return LevelState.NotAvailable;
	}

	public static Rank GetLevelRank(Level level, bool twoPlayer = false, int specificSlot = -1)
	{
		return GetLevelRank(level.ToString(), twoPlayer: false, specificSlot);
	}

	public static Rank GetLevelRank(string level, bool twoPlayer = false, int specificSlot = -1)
	{
		PlayerPrefsJson obj = ((specificSlot != -1) ? slotPrefs[specificSlot] : currentSlot);
		string key = GetLevelRankPrefix(twoPlayer: false) + level + "_rank";
		string text = obj.Get(key, (-2).ToString());
		if (!(text == (-2).ToString()))
		{
			return Rank.FromString(text);
		}
		return -2;
	}

	public static bool GetLevelClearedByAnySlot(Level level)
	{
		for (int i = 0; i < 3; i++)
		{
			if (GetStateFromLevel(level, i) == LevelState.Passed)
			{
				return true;
			}
		}
		return false;
	}

	public static int GetLevelScore(string level)
	{
		string key = GetLevelRankPrefix(twoPlayer: false) + level + "_score";
		return currentSlot.Get(key, 0);
	}

	public static string GetLastPlayedLevel()
	{
		return currentSlot.Get("just_finished_level", null);
	}

	public static GameResult GetGameResult()
	{
		return currentSlot.Get("game_result", GameResult.LevelHasntBeenPassed);
	}

	public static float GetMistakeOffset()
	{
		return currentSlot.Get("mistake_offset", -1f);
	}

	public static float GetMistakeCount()
	{
		return currentSlot.Get("mistake_count", 0f);
	}

	public static int GetFeedbackCounter()
	{
		return currentSlot.Get("feedback_counter", 0);
	}

	public static int GetInsultCounter()
	{
		return currentSlot.Get("insult_counter", 0);
	}

	public static bool GetIanDesktopLogin()
	{
		return currentSlot.Get("ian_desktop_login", defaultValue: false);
	}

	public static int GetRhythmStackerScore()
	{
		return currentSlot.Get("rhythmstacker_score", 0);
	}

	public static float GetTempresScore()
	{
		return currentSlot.Get("tempres_score", 0f);
	}

	public static string GetLevelRankPrefix(bool twoPlayer)
	{
		string text = "";
		if (scnBase.isBooth)
		{
			text = text + "expo" + (twoPlayer ? "2p_" : "_");
		}
		return text + "Level_";
	}

	public static int NumberOfStars(string[] levels = null)
	{
		int num = 0;
		for (int i = 0; i < levels.Length; i++)
		{
			if (GetLevelRank(levels[i]).passed)
			{
				num++;
			}
		}
		return num;
	}

	public static int NumberOfScrolls(string[] levels = null)
	{
		int num = 0;
		for (int i = 0; i < levels.Length; i++)
		{
			if (GetLevelRank(levels[i]).perfected)
			{
				num++;
			}
		}
		return num;
	}

	public static void UnlockAllLevels()
	{
		foreach (Level value in Enum.GetValues(typeof(Level)))
		{
			SetLevelRank(value, -1);
		}
		SetIsGameDone(state: true);
	}

	public static void GetCalibrationValues(out float calibration_v, out float calibration_i, out float calibration_i_P2_internal, out float latency)
	{
		calibration_v = generalPrefs.GetFloat("calibration_v", RDCalibration.NotSet);
		calibration_i = generalPrefs.GetFloat("calibration_i", RDCalibration.NotSet);
		calibration_i_P2_internal = generalPrefs.GetFloat("calibration_i_P2", RDCalibration.NotSet);
		latency = generalPrefs.GetFloat("latency", RDCalibration.NotSet);
	}

	public static void SetCalibrationValues(float calibration_v, float calibration_i, float calibration_i_P2_internal, float latency)
	{
		generalPrefs.Set("calibration_i", calibration_i);
		generalPrefs.Set("calibration_i_P2", calibration_i_P2_internal);
		generalPrefs.Set("latency", latency);
		generalPrefs.Set("calibration_v", calibration_v);
		generalPrefs.Save();
	}

	public static void SetLatencyValue(float latency)
	{
		generalPrefs.Set("latency", latency);
	}

	public static void SaveWindowState()
	{
		SetHasWindowDanceChangedResolution(state: true);
		SetResolutionWidth(Screen.width);
		SetResolutionHeight(Screen.height);
		SetIsFullscreen(Screen.fullScreen);
		if (!Screen.fullScreen)
		{
			Vector2Int position = WindowChoreographer.playerWindow.GetPosition();
			SetWindowXPosition(position.x);
			SetWindowYPosition(position.y);
			Debug.Log($"SaveWindowState() -> Window Position {position}");
		}
		else
		{
			SetWindowXPosition(0);
			SetWindowYPosition(0);
		}
		Debug.Log($"SaveWindowState() ({Screen.width} x {Screen.height})");
	}

	public static void ToggleLevelsState(bool unlock)
	{
		if (!Application.isPlaying)
		{
			Load();
		}
		SetPlayedPassedLevelCutscene(unlock);
		SetPlayedPostAct2Cutscene(unlock);
		SetPlayedPostAct3Cutscene(unlock);
		SetPlayedPostAct4Cutscene(unlock);
		SetPlayedPreAct5Cutscene(unlock);
		SetPlayedPreAct6Cutscene(unlock);
		SetPlayedHaileyDuetIntroduction(unlock);
		SetPlayedPreBitternessCutscene(unlock);
		SetPlayedRooftopCutscene(unlock);
		SetPlayedAct6Intro(unlock);
		SetDidCalibration(unlock);
		Level[] values = RDUtils.GetValues<Level>();
		if (unlock)
		{
			SetFirstTimePlaying(state: false);
			Level[] array = values;
			for (int i = 0; i < array.Length; i++)
			{
				SetLevelRankInternal(array[i], 4);
			}
		}
		else
		{
			slotPrefs[0].dict.Clear();
		}
		SaveAll();
	}

	public static void SaveAll()
	{
		PlayerPrefsJson.SaveAllFiles();
	}

	public static void SaveCurrentSlot()
	{
		currentSlot.Save();
	}

	public static void Copy(int slotFrom, int slotTo, bool copySleeveSprites)
	{
		PlayerPrefsJson playerPrefsJson = slotPrefs[slotTo];
		playerPrefsJson.CopyFrom(slotPrefs[slotFrom]);
		if (copySleeveSprites)
		{
			string drawingPath = ArmSkin.GetDrawingPath(RDPlayer.P1, slotFrom);
			string drawingPath2 = ArmSkin.GetDrawingPath(RDPlayer.P2, slotFrom);
			string drawingPath3 = ArmSkin.GetDrawingPath(RDPlayer.P1, slotTo);
			string drawingPath4 = ArmSkin.GetDrawingPath(RDPlayer.P2, slotTo);
			if (!RDFile.Exists(drawingPath))
			{
				RDFile.Delete(drawingPath3);
			}
			else
			{
				RDFile.Copy(drawingPath, drawingPath3, overwrite: true);
			}
			if (!RDFile.Exists(drawingPath2))
			{
				RDFile.Delete(drawingPath4);
			}
			else
			{
				RDFile.Copy(drawingPath2, drawingPath4, overwrite: true);
			}
			for (int i = 0; i < 2; i++)
			{
				LoadArmSkin(slotTo, (RDPlayer)i);
			}
		}
		playerPrefsJson.Save();
	}

	public static void Load()
	{
		PlayerPrefsJson.LoadAllFiles();
		int releaseVersion = generalPrefs.GetReleaseVersion();
		Debug.Log("releaseVersion: " + releaseVersion);
		if (releaseVersion < 2 && GetDidCalibration())
		{
			float num = generalPrefs.GetFloat("calibration_i", RDCalibration.NotSet);
			if (num != (float)RDCalibration.NotSet)
			{
				generalPrefs.Set("calibration_i_P2", num);
				Debug.Log("Matching P2 Calibration with P1 if coming from r1");
			}
		}
		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < 2; j++)
			{
				LoadArmSkin(i, (RDPlayer)j);
			}
		}
		if (releaseVersion < 9)
		{
			Rank rank = 0;
			int levelScore = GetLevelScore("BeansHopper");
			if (levelScore > 10)
			{
				rank = 11;
			}
			if (levelScore > 20)
			{
				rank = 12;
			}
			if (levelScore > 30)
			{
				rank = 13;
			}
			if (levelScore > 40)
			{
				rank = 14;
			}
			if (levelScore > 60)
			{
				rank = 15;
			}
			SetLevelRankInternal(Level.BeansHopper, rank);
		}
		if (releaseVersion < 10)
		{
			if ((int)GetLevelRank(Level.HelpingHands) == -2)
			{
				SetLevelRankInternal(Level.HelpingHands, -1);
			}
			if ((int)GetLevelRank(Level.ArtExercise) == -2)
			{
				SetLevelRankInternal(Level.ArtExercise, -1);
			}
		}
		if (releaseVersion < 12)
		{
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, object> item in slotPrefs[0].dict)
			{
				if (item.Key.StartsWith("CustomLevel_"))
				{
					list.Add(item.Key);
				}
			}
			foreach (string item2 in list)
			{
				string text = string.Empty;
				try
				{
					int num2 = item2.IndexOf('_');
					int num3 = item2.LastIndexOf('_');
					if (num3 != num2)
					{
						num2++;
						string hash = item2.Substring(num2, num3 - num2);
						float speed = Convert.ToSingle(item2.Substring(num3 + 1));
						text = GetCustomLevelKey(hash, speed);
					}
					else
					{
						num2++;
						text = GetCustomLevelKey(item2.Substring(num2));
					}
				}
				catch (Exception ex)
				{
					Debug.Log("CLS rank error at: " + item2 + ", " + ex.ToString());
				}
				if (text != string.Empty)
				{
					Rank rank2 = slotPrefs[0].GetInt(item2);
					generalPrefs.Set(text, rank2.ToString());
				}
				slotPrefs[0].Remove(item2);
			}
			if (list.Count > 0)
			{
				Debug.Log($"moved {list.Count} ranks to general");
				SaveAll();
			}
		}
		if (releaseVersion < 27 && !generalPrefs.dict.ContainsKey("global_volume"))
		{
			SetGlobalVolume(10);
		}
		if (releaseVersion < 40 && !generalPrefs.dict.ContainsKey("narration") && GetLevelClearedByAnySlot(Level.OrientalInsomniac))
		{
			SetNarration(enabled: false);
		}
	}

	private static void LoadArmSkin(int slotIndex, RDPlayer playerIndex)
	{
		ArmSkin armSkin = new ArmSkin(slotIndex, playerIndex);
		armSkin.Load();
		skins[slotIndex, (int)playerIndex] = armSkin;
	}

	public static void ConvertOldSave()
	{
		string text = Path.Combine(Application.persistentDataPath, "RDdata.rdsave");
		if (!RDFile.Exists(text))
		{
			return;
		}
		RDBaseDllDummy.printem("old file exists");
		bool flag = false;
		bool flag2 = false;
		try
		{
			foreach (object item in (Json.Deserialize(RDFile.ReadAllText(text)) as Dictionary<string, object>)["playerPrefKeyList"] as List<object>)
			{
				Dictionary<string, object> obj = item as Dictionary<string, object>;
				string text2 = obj["key"] as string;
				string text3 = obj["value"] as string;
				string text4 = obj["type"] as string;
				bool num = text2.EndsWith("_1");
				PlayerPrefsJson playerPrefsJson = (num ? slotPrefs[0] : generalPrefs);
				if (num)
				{
					RDBaseDllDummy.printes("is slot key: " + text2);
					text2 = text2.Remove(text2.Length - "_1".Length, "_1".Length);
					flag = true;
				}
				object value = null;
				switch (text4)
				{
				case "String":
					value = text3;
					break;
				case "Float":
					value = Convert.ToSingle(text3);
					break;
				case "Int":
					value = Convert.ToInt32(text3);
					break;
				case "Bool":
					value = Convert.ToInt32(text3) == 1;
					break;
				}
				playerPrefsJson.dict[text2] = value;
			}
		}
		catch (Exception ex)
		{
			Debug.LogError(ex.Message + "\n" + ex.StackTrace);
			Load();
			flag2 = true;
		}
		string availableFilename = RDUtils.GetAvailableFilename(DataPath + "/RDdata_old", ".rdsave");
		RDFile.Move(text, availableFilename);
		if (!flag2)
		{
			generalPrefs.Save();
			if (flag)
			{
				slotPrefs[0].Save();
			}
		}
	}

	public static GameCompletion GetGameCompletion()
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		foreach (var (level2, levelMetadata2) in GC.levelMetadata)
		{
			if (levelMetadata2.type != LevelType.Tutorial && level2 != Level.GongXi && level2 != Level.RhythmWeightlifter && level2 != Level.Halloween)
			{
				Rank rank = GetLevelRank(level2).ToNormal();
				if ((int)rank >= 4)
				{
					num2++;
				}
				if (rank.passed)
				{
					num++;
				}
				if (rank.perfected)
				{
					num3++;
				}
			}
		}
		float percentCompletion = (float)(num + num3 + num2) / 3f / (float)GC.levelCount * 100f;
		return new GameCompletion
		{
			starLevels = num2,
			passedLevels = num,
			perfectLevels = num3,
			percentCompletion = percentCompletion
		};
	}

	public static void UnlockAchievement(Achievement achievement, bool storeStats = false)
	{
		SteamIntegration.UnlockAchievement(achievement, storeStats);
	}

	public static void UnlockAchievement(string name, bool storeStats = false)
	{
		SteamIntegration.UnlockAchievement(name, storeStats);
	}

	public static void GiveLevelAchievements()
	{
		if (Level.OrientalInsomniac.Passed())
		{
			CompleteAct("Act1");
		}
		if (Level.Boss2.Passed())
		{
			CompleteAct("Act2");
		}
		if (Level.Lesmis.Passed())
		{
			CompleteAct("Act3");
		}
		if (Level.InsomniacHard.Passed())
		{
			CompleteAct("Act4");
		}
		if (Level.AthleteFinale.Passed())
		{
			CompleteAct("Act5");
		}
		if (Level.PaigesReckoning.Passed())
		{
			CompleteAct("Act6");
		}
		if (Level.Montage2.Passed())
		{
			CompleteAct("Act7");
		}
		if (Level.HelpingHands.Passed())
		{
			CompleteAct("EarlyAccess");
		}
		if (Level.The90sDecision.Passed())
		{
			CompleteAct("MuseDash");
		}
		foreach (Level key in GC.levelMetadata.Keys)
		{
			GiveLevelAchievement(key, GetLevelRank(key));
		}
		static void CompleteAct(string act)
		{
			UnlockAchievement(act + "Complete");
		}
	}

	private static void GiveLevelAchievement(Level level, Rank rank, bool coop = false)
	{
		int num;
		if (GC.levelMetadata.TryGetValue(level, out var value))
		{
			LevelType type = value.type;
			num = ((type != LevelType.Intermission && type != LevelType.Cutscene && type != LevelType.Tutorial) ? 1 : 0);
		}
		else
		{
			num = 0;
		}
		bool flag = (byte)num != 0;
		if (level == Level.Montage)
		{
			rank = ((Level.Montage.GetRank().perfected && Level.Montage2.GetRank().perfected) ? ((Rank)5) : ((!Level.Montage.Passed() || !Level.Montage2.Passed()) ? ((Rank)(-1)) : ((Rank)4)));
		}
		if (rank.passed)
		{
			UnlockAchievement($"Level{level}Complete");
		}
		if (rank.perfected)
		{
			UnlockAchievement($"Level{level}Perfect");
			if (coop)
			{
				UnlockAchievement($"Level{level}CoopPerfect");
			}
			if (flag)
			{
				UnlockAchievement(Achievement.FirstPerfect);
			}
		}
		if (rank.ComparativeInt() >= 4)
		{
			UnlockAchievement($"Level{level}A");
		}
		if (rank.isNormalRank)
		{
			UnlockAchievement($"Level{level}Any");
			if (coop)
			{
				UnlockAchievement(Achievement.CoopAny);
			}
		}
	}
}
