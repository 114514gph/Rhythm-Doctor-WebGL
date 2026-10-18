using System;
using System.Collections.Generic;
using UnityEngine;

public class scrGameManager : RDBase
{
	public static scrGameManager instance;

	[Header("Settings")]
	public bool soft;

	public static int volume;

	public static int musicVolume;

	public static int gameplayVolume;

	public static int beatsoundsVolume;

	public static int hitsoundsVolume;

	public static int countingVoiceVolume;

	public static int cuesoundsVolume;

	public static int feedbackVolume;

	public static int interfaceVolume;

	public static int levelEditorVolume;

	public static int dialogueVolume;

	public static int ambienceVolume;

	public static bool muteInBackground;

	[Header("Prefab")]
	public GameObject Conductor;

	public GameObject scrVfxControl;

	public GameObject smoke;

	[Header("Objects")]
	public Transform parentContainer;

	[NonSerialized]
	public RDCamera hudRDCamera;

	public Camera hudCamera;

	public RDInk inkDialogue;

	public List<Entity> entityPool = new List<Entity>();

	public List<InvisibleEntity> invisibleEntityPool = new List<InvisibleEntity>();

	private static bool firstTimeSceneStarted = true;

	private void Awake()
	{
		instance = this;
		hudRDCamera = hudCamera.GetComponent<RDCamera>();
		LoadSavedVolumes();
		muteInBackground = Persistence.GetMuteInBackground();
		if (!Application.isEditor)
		{
			base.gm.soft = false;
		}
		RDBase.CreateIfNotFound("Conductor", Conductor);
		RDBase.CreateIfNotFound("VfxController", scrVfxControl);
	}

	public static void LoadSavedVolumes()
	{
		volume = Persistence.GetGlobalVolume();
		musicVolume = Persistence.GetMusicVolume();
		gameplayVolume = Persistence.GetGameplayVolume();
		beatsoundsVolume = Persistence.GetBeatsoundsVolume();
		hitsoundsVolume = Persistence.GetHitsoundsVolume();
		countingVoiceVolume = Persistence.GetCountingVoiceVolume();
		cuesoundsVolume = Persistence.GetCuesoundsVolume();
		feedbackVolume = Persistence.GetFeedbackVolume();
		interfaceVolume = Persistence.GetInterfaceVolume();
		dialogueVolume = Persistence.GetDialogueVolume();
		ambienceVolume = Persistence.GetAmbienceVolume();
		levelEditorVolume = Persistence.GetLevelEditorVolume();
		ApplyLoadedVolumes();
	}

	public static void ApplyLoadedVolumes()
	{
		RDUtils.SetMixerVolume("MasterVolume", (float)volume * 0.1f);
		RDUtils.SetMixerVolume("MusicVolume", (float)musicVolume * 0.1f);
		RDUtils.SetMixerVolume("GameplayVolume", (float)gameplayVolume * 0.1f);
		RDUtils.SetMixerVolume("BeatsoundsVolume", (float)beatsoundsVolume * 0.1f);
		RDUtils.SetMixerVolume("HitsoundsVolume", (float)hitsoundsVolume * 0.1f);
		RDUtils.SetMixerVolume("CountingVoiceVolume", (float)countingVoiceVolume * 0.1f);
		RDUtils.SetMixerVolume("CuesoundsVolume", (float)cuesoundsVolume * 0.1f);
		RDUtils.SetMixerVolume("FeedbackVolume", (float)feedbackVolume * 0.1f);
		RDUtils.SetMixerVolume("InterfaceVolume", (float)interfaceVolume * 0.1f);
		RDUtils.SetMixerVolume("DialogueVolume", (float)dialogueVolume * 0.1f);
		RDUtils.SetMixerVolume("AmbienceVolume", (float)ambienceVolume * 0.1f);
		RDUtils.SetMixerVolume("LevelEditorVolume", (float)levelEditorVolume * 0.1f);
	}

	public static void ToggleFullMute(bool mute)
	{
		volume = ((!mute) ? Persistence.GetGlobalVolume() : 0);
		ApplyLoadedVolumes();
	}

	private void Start()
	{
		RDBase.Vfx.SetSpriteMode(SpriteMode.None, Color.white);
		Time.timeScale = 1f;
		RDTime.fakePauseEverything = false;
	}

	private void Update()
	{
		RDTime.currentFrame = Time.frameCount;
	}

	private void LateUpdate()
	{
	}
}
