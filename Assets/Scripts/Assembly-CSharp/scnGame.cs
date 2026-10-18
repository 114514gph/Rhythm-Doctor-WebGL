using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DG.Tweening;
using RDLevelEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class scnGame : scnBase, IRequiredControllerPrompt
{
	public struct HitInfo
	{
		public int bar;

		public OffsetType offsetType;
	}

	public enum BorderFeedbackType
	{
		Correct = 0,
		Incorrect = 1,
		DrumHit = 2
	}

	public RenderTexture[] windowRenderTextures;

	public static LevelSource levelToLoadSource = LevelSource.InternalPath;

	public static bool attemptToLoadTutorial = false;

	public static string currentLevelPath = null;

	public static string internalIdentifier = null;

	public static bool loadDogMode = false;

	public static float levelSpeed = 1f;

	public static bool goToScnMenuAfterHelpingHands = false;

	public static bool isRestartingLevel = false;

	public static LevelPersistence levelPersistence = new LevelPersistence();

	public static int TEST_barToScrubToStatic = 0;

	public static DefibMode p1DefibMode = DefibMode.Normal;

	public static DefibMode p2DefibMode = DefibMode.Normal;

	public static List<float> p1HitTimes = new List<float>();

	public static List<float> p2HitTimes = new List<float>();

	public static string customDebugText = "";

	public static bool tapsOnly = false;

	public static bool drumMode = false;

	public static int framesElapsed;

	public static int fps;

	public static float lastTimeReference;

	public static float timeReference;

	[NonSerialized]
	[Header("Options")]
	public int barToSkipToOnSlash;

	[Header("Prefabs")]
	public GameObject RowEntity;

	public GameObject Room;

	public GameObject customBackground;

	[Header("scnGame Objects")]
	public Canvas uiCanvas;

	public Rankscreen rankscreen;

	public GameObject listenerObject;

	public TextMesh syncText;

	public scrHandController handController0;

	public scrHandController handController1;

	public scrHandController handController2;

	public scrHandController handController3;

	public scrHandController handControllerOnTop;

	public HPBarController hpController;

	public MistakesManager mistakesManager;

	public MistakesManager mistakesAtStart;

	public GameObject srt_noteParticles;

	public RDImageEffectPixelate imageEffectPixelate;

	public HitStripManager hitStripManager;

	public Text debugText;

	private StringBuilder debugString;

	public Transform roomsContainer;

	public Image frontOfAllOverlay;

	public RectTransform activityIndicatorPivot;

	public Image activityIndicator;

	public Text flashTextUI;

	public GameObject windowDanceMovement;

	public LEDSign statusText;

	public GameObject eventSystem;

	public Transform bladesContainer;

	public Transform[] bladeSprites;

	[Header("Cameras")]
	public TopCamera topCamera;

	public UICamera uiCamera;

	public UICamera pauseMenuCamera;

	public BlitterCamera blitterCamera;

	public WindowCamera windowCamera;

	[NonSerialized]
	public RDCamera[] uiCameras;

	[Header("Simulate Window Dance")]
	public SimulateWDController simulateWD;

	[Header("Text Mask")]
	public GameObject maskFlash;

	public Text textMask;

	public RawImage textMaskTexture;

	public Image textMaskBackground;

	[Header("Switch Players")]
	public GameObject switchPlayersContainer;

	public RectTransform p1Mark;

	public RectTransform p2Mark;

	public Image p1MarkImage;

	public Image p2MarkImage;

	public Vector3 leftMarkPos;

	public Vector3 rightMarkPos;

	[Header("Mobile UI")]
	public GameObject MobileButtons;

	public GameObject MobilePauseButton;

	public GameObject MobileStatusText;

	public Canvas canvas;

	[Header("Desktop UI")]
	public GameObject transitionBarsContainer;

	public Text p1Text;

	public Text p2Text;

	[Header("Pause Menu")]
	public PauseMenu pauseMenu;

	public Image pauseMenuOverlay;

	[Header("Oneshot Row Animations")]
	public RDWaveAnimation animationBoom;

	public RDWaveAnimation animationRush;

	public RDWaveAnimation animationBallBoom;

	public RDWaveAnimation animationBallRush;

	public RDWaveAnimation animationSpringBoom;

	public RDWaveAnimation animationSpringRush;

	public RDWaveAnimation animationSpikeBoom;

	public RDWaveAnimation animationSpikeHugeBoom;

	public RDWaveAnimation animationRushSpecial;

	public RDWaveAnimation animationSingle;

	[NonSerialized]
	public bool levelFinishedLoading;

	[NonSerialized]
	public Row[] rows;

	[NonSerialized]
	public List<int> rowSortOrder = new List<int>();

	[NonSerialized]
	public List<bool> beatAlarm = new List<bool>();

	[NonSerialized]
	public List<scrBeatLoopOneshot> beatLoopsOneshot = new List<scrBeatLoopOneshot>();

	[NonSerialized]
	public bool failedLevel;

	[NonSerialized]
	public bool repositionRows = true;

	[NonSerialized]
	public RDRoom[] rooms;

	[NonSerialized]
	public MultiplayerAppearance multiplayerAppearance;

	[NonSerialized]
	public bool nurseCount;

	[NonSerialized]
	public LevelBase currentLevel;

	[NonSerialized]
	public List<Beat> beats = new List<Beat>();

	[NonSerialized]
	public string levelIdentifier;

	[NonSerialized]
	public List<GameObject> renderTextureQuads = new List<GameObject>();

	[NonSerialized]
	public AudioListener listener;

	[NonSerialized]
	public bool spacebarOnNothing = true;

	[NonSerialized]
	public Beat[] playerCaughtHoldBeat = new Beat[4];

	[NonSerialized]
	public int autoHeldBeatCounter;

	[NonSerialized]
	public bool ignoreInput;

	[NonSerialized]
	public bool skippedLastSkippableCutscene;

	public WindowChoreographer windowChoreographer;

	[NonSerialized]
	public float oneshotSlowdownMultiplier;

	[NonSerialized]
	public static bool pauseBlocked;

	[NonSerialized]
	public bool paused;

	[NonSerialized]
	public float HoldDurationToSkipCutscene = 0.6f;

	[NonSerialized]
	public int barToScrubTo;

	[NonSerialized]
	public float syncHitRemainingTime;

	[NonSerialized]
	public float lastBeepTime;

	[NonSerialized]
	public static int lastBarDiedAt;

	[NonSerialized]
	public float visualSpeed;

	[NonSerialized]
	public bool reducedFlash;

	[NonSerialized]
	public List<scrBeatLoop> beatLoops = new List<scrBeatLoop>();

	[NonSerialized]
	public int getScreenshot;

	[NonSerialized]
	public const int NullFinger = -1;

	[NonSerialized]
	public int leftFinger = -1;

	[NonSerialized]
	public int rightFinger = -1;

	[NonSerialized]
	public AudioMixerGroup cuesoundsGroup;

	[NonSerialized]
	public AudioMixerGroup oneshotGroup;

	[NonSerialized]
	public AudioMixerGroup oneshotVoiceGroup;

	[NonSerialized]
	public AudioMixerGroup oneshotClicksGroup;

	[NonSerialized]
	public List<Conditional> conditionals = new List<Conditional>();

	[NonSerialized]
	public List<TimedAction> timedActions = new List<TimedAction>();

	[NonSerialized]
	public bool windowDanceEventCalled;

	private bool unpauseAudioOnResume;

	private bool loadedFromPersistence;

	private bool loadedFromRestart;

	private bool persistenceStartImmediately;

	private GameState gameStateWatcher;

	private bool hasEnteredLowHealth;

	private float skippingCutsceneElapsedTime;

	private float p1WentDownTimeStamp;

	private float p2WentDownTimeStamp;

	private int debugKey;

	private float beginNarrationTime;

	private float lastBeginNarrationTime;

	private string beginLevel;

	public float recordingFPSTimer;

	private const float recordingFPSInterval = 5f;

	public float minFPS = -1f;

	public float maxFPS = -1f;

	public List<float> recordedFPSList = new List<float>();

	public string recordingFileName = "RD_RECORDING_FPS_OUTPUT.txt";

	public List<HitInfo>[] rowsHitOffsets = new List<HitInfo>[16];

	public List<HitInfo> allHitOffsets = new List<HitInfo>();

	private GameState _gameState;

	private bool doOnce;

	private bool narratedLevelReady;

	private bool narratedSkipLabel;

	private bool narratedKeepHoldingLabel;

	private bool narratedSkippingLabel;

	private Tween disappearBlades;

	private bool dontLetGameStateToggleHands;

	public const string BeatName = "Beat";

	public int shadowIdBeatOffset = 100;

	public const int DontWantShadowRows = -1;

	[NonSerialized]
	public ShadowRowStyle shadowRowStyle;

	private Tween flashBorderTween;

	[NonSerialized]
	public Color flashBorderColor;

	[Header("Sight Reading Tournament")]
	public GameObject timerGO;

	public Text timerText;

	public Text localTime;

	private bool showLoadingScreen
	{
		get
		{
			LevelType levelType = currentLevel.levelType;
			if (levelType != LevelType.Intro && levelType != LevelType.Tutorial && levelType != LevelType.Cutscene && !currentLevel.startImmediately && !persistenceStartImmediately)
			{
				if (base.editor != null)
				{
					return !base.editor.scrubbing;
				}
				return true;
			}
			return false;
		}
	}

	public GameState gameState
	{
		get
		{
			return _gameState;
		}
		set
		{
			_gameState = value;
			if (!base.editorMode && _gameState == GameState.CutsceneSkippable)
			{
				int num;
				if (levelToLoadSource == LevelSource.InternalPath)
				{
					LevelType levelType = currentLevel.levelType;
					num = ((levelType != LevelType.Intermission && levelType != LevelType.Cutscene && levelType != LevelType.Tutorial) ? 1 : 0);
				}
				else
				{
					num = 0;
				}
				bool flag = Persistence.GetStateFromLevel(levelIdentifier) != LevelState.Passed && Persistence.GetTriesToClearLevel(levelIdentifier) <= 0;
				if (((uint)num & (flag ? 1u : 0u)) != 0 && !currentLevel.alwaysSkippableCutscene)
				{
					_gameState = GameState.Cutscene;
				}
			}
			if (dontLetGameStateToggleHands)
			{
				return;
			}
			if (_gameState == GameState.Handmode)
			{
				HandAction lastHandAction = handControllerOnTop.lastHandAction;
				if (lastHandAction == HandAction.Show || lastHandAction == HandAction.Raise)
				{
					handControllerOnTop.Show(show: true);
				}
			}
			if (_gameState == GameState.Cutscene)
			{
				handControllerOnTop.Show(show: false, instant: false, deactivateAfterHiding: false, updateHandsAfterHiding: false, fromCutscene: true);
			}
		}
	}

	public bool receptive
	{
		get
		{
			bool flag = !base.ink.activeDialoguePresent;
			bool num = gameState == GameState.Handmode || gameState == GameState.HandmodePreCutscene;
			bool flag2 = beats.Count > 0;
			bool flag3 = !ignoreInput;
			return ((num & flag) | flag2) & flag3;
		}
	}

	public int playersAlive
	{
		get
		{
			int num = 0;
			bool flag = true;
			bool flag2 = true;
			Row[] array = rows;
			for (int i = 0; i < array.Length; i++)
			{
				RowEntity ent = array[i].ent;
				if (!(ent != null))
				{
					continue;
				}
				RDPlayer currentPlayer = ent.row.GetCurrentPlayer();
				if (ent.heartHasCracked)
				{
					if ((currentPlayer == RDPlayer.P1) & flag)
					{
						flag = false;
					}
					else if ((currentPlayer == RDPlayer.P2) & flag2)
					{
						flag2 = false;
					}
				}
			}
			if (flag)
			{
				num++;
			}
			if (flag2)
			{
				num++;
			}
			return num;
		}
	}

	public new static scnGame instance => scnBase._instance as scnGame;

	public bool startTheGameCalled => gameState != GameState.PreStart;

	public bool started
	{
		get
		{
			GameState gameState = this.gameState;
			if (gameState != GameState.PreStart)
			{
				return gameState != GameState.SpacePressedPreStart;
			}
			return false;
		}
	}

	public bool levelIsSkippable
	{
		get
		{
			LevelType levelType = currentLevel.levelType;
			if (levelType != LevelType.Cutscene && levelType != LevelType.Tutorial && levelType != LevelType.Intro && !currentLevel.skippable)
			{
				return RDBase.debugSettings.Debug;
			}
			return true;
		}
	}

	public bool levelIsRestartable
	{
		get
		{
			LevelType levelType = currentLevel.levelType;
			return levelType == LevelType.Regular || levelType == LevelType.Boss || levelType == LevelType.Challenge;
		}
	}

	public bool levelIsQuittable => currentLevel.levelType != LevelType.Cutscene;

	public bool levelHasCheckpoint
	{
		get
		{
			LevelBase levelBase = currentLevel;
			if (levelBase == null)
			{
				return false;
			}
			return levelBase.checkpointBarsDesc?.Length > 0;
		}
	}

	public bool showMarginsNumerically
	{
		get
		{
			if (GC.d_showMarginsNumerically)
			{
				LevelBase levelBase = currentLevel;
				if (levelBase == null)
				{
					return true;
				}
				return !levelBase.forceNoFrameOffsets;
			}
			return false;
		}
	}

	public bool showAbsoluteOffsets
	{
		get
		{
			if (GC.showAbsoluteOffsets)
			{
				LevelBase levelBase = currentLevel;
				if (levelBase == null)
				{
					return true;
				}
				return !levelBase.forceNoFrameOffsets;
			}
			return false;
		}
	}

	public bool windowDancing => windowChoreographer?.setup ?? false;

	public int roomScale
	{
		get
		{
			if (!base.game.currentLevel.fullResolutionRenderTextures)
			{
				return GC.RoomRenderTextureScale;
			}
			return 0;
		}
	}

	private new void Awake()
	{
		base.Awake();
		scnBase._instance = this;
		uiCameras = new RDCamera[3] { uiCamera, pauseMenuCamera, windowCamera };
		if (scnBase.lastSceneName != "scnGame")
		{
			base.platformHelper?.InvalidateMonitors();
		}
		Persistence.SetMistakeOffset(999f);
		mistakesManager = new MistakesManager();
		scrChar.FlushCharacters();
		if (!base.editorMode && scnBase.isMobile)
		{
			MobileButtons.SetActive(value: true);
		}
		eventSystem.SetActive(!base.editorMode);
		RDGameSounds.LoadDefaults();
		LoadPlayerHandPopSounds();
		if (scnBase.isDivekick)
		{
			base.gc.player1StripColor = "FFF80089".HexToColor();
			base.gc.player2StripColor = "004CFFB2".HexToColor();
		}
		rankscreen.Setup();
		statusText.message.text = "";
		if (RDStartup.firstScene == base.gameObject.scene.name)
		{
			RDStartup.firstScene = null;
			GC.twoPlayerMode = PlayerPrefs.GetInt("switcherTwoPlayer", 0) == 1;
			levelSpeed = PlayerPrefs.GetFloat("switcherGameSpeed", 1f);
			string path = PlayerPrefs.GetString("switcherCustomLevel", "OrientalTechno");
			bool flag = PlayerPrefs.GetInt("switcherLoadTutorial", 0) == 1;
			scnBase.GoToLevel(path, loadGameScene: false, flag);
		}
		cuesoundsGroup = RDUtils.GetMixerGroup("CuesoundsParent");
		oneshotVoiceGroup = RDUtils.GetMixerGroup("RDGSVoice");
		oneshotClicksGroup = RDUtils.GetMixerGroup("RDGSClicks");
		p1DefibMode = Persistence.GetDefibrillatorP1();
		p2DefibMode = Persistence.GetDefibrillatorP2();
		reducedFlash = Persistence.GetReducedFlash();
		pauseMenu.Initialize();
		for (int i = 0; i < 16; i++)
		{
			rowsHitOffsets[i] = new List<HitInfo>();
		}
		scnLevelSelect.playerQuitLevel = false;
		if (isRestartingLevel)
		{
			isRestartingLevel = false;
			loadedFromRestart = true;
		}
		debugString = new StringBuilder();
	}

	private new void Start()
	{
		base.Start();
		Narration.Clear();
		Narration.InitializeNarrateRowInfo();
		Narration.Say(RDString.Get("status.loading"), NarrationCategory.Notification, false);
		RDBase.Vfx.ResetEffects();
		gameState = (SpeedrunValues.currentGameState = GameState.PreStart);
		gameStateWatcher = gameState;
		base.audioPaused = false;
		visualSpeed = 1f;
		oneshotSlowdownMultiplier = 1f;
		LEDSign.showMarginError = false;
		hpController.Setup();
		SpeedrunValues.isLoading = true;
		drumMode = Persistence.GetDrumMode();
		tapsOnly = drumMode || Persistence.GetTapsOnly();
		RDTime.speed = levelSpeed;
		string jsonText = "";
		bool flag = false;
		if (RDLevelData.current != null)
		{
			RDLevelData.current.usesWindowDance = false;
		}
		if (base.editorMode)
		{
			levelToLoadSource = LevelSource.ExternalPath;
			jsonText = base.editor.rdlevelText;
		}
		else if (levelToLoadSource == LevelSource.InternalPath)
		{
			string text = internalIdentifier;
			printe("internalIdentifier: " + internalIdentifier);
			TextAsset textAsset = null;
			string text2 = "InternalLevels/" + text;
			string path = text2 + "/" + text + ".tutorial";
			string path2 = text2 + "/" + text;
			string path3 = text2.TrimEnd('H') + "/" + text.TrimEnd('H') + ".night";
			string path4 = text2.TrimEnd('H') + "/" + text.TrimEnd('H') + ".tutorial.night";
			string path5 = text2 + "/" + text + ".2p";
			if (attemptToLoadTutorial)
			{
				attemptToLoadTutorial = false;
				textAsset = Resources.Load<TextAsset>(path);
				if (textAsset != null)
				{
					currentLevelPath = path;
					flag = true;
				}
				else
				{
					textAsset = Resources.Load<TextAsset>(path4);
					if (textAsset != null)
					{
						currentLevelPath = path4;
						flag = true;
					}
				}
			}
			if (textAsset == null && GC.twoPlayerMode)
			{
				textAsset = Resources.Load<TextAsset>(path5);
				if (textAsset != null)
				{
					currentLevelPath = path5;
				}
			}
			else if (textAsset != null && GC.twoPlayerMode && RDInput.actuallySwapped)
			{
				RDInput.SwapP1AndP2Controls(changePersistence: false);
			}
			if (textAsset == null)
			{
				textAsset = Resources.Load<TextAsset>(path2);
				if (textAsset != null)
				{
					currentLevelPath = path2;
				}
			}
			if (textAsset == null)
			{
				if (text[text.Length - 1] == 'H')
				{
					textAsset = Resources.Load<TextAsset>(path3);
					if (textAsset != null)
					{
						currentLevelPath = path3;
					}
				}
			}
			if (textAsset == null)
			{
				if (Enum.TryParse<Level>(text, out var result))
				{
					currentLevel = LevelBase.InstantiateLevelClass(result);
				}
			}
			else
			{
				jsonText = textAsset.text;
			}
			levelIdentifier = (flag ? (text + "Tutorial") : text);
		}
		else if (levelToLoadSource == LevelSource.ExternalPath)
		{
			levelIdentifier = currentLevelPath;
			jsonText = RDFile.ReadAllText(currentLevelPath);
		}
		else if (levelToLoadSource == LevelSource.CutscenesPath)
		{
			string path6 = scnBase.PathForCutscene(internalIdentifier);
			jsonText = Resources.Load<TextAsset>(path6).text;
			currentLevelPath = path6;
			levelIdentifier = internalIdentifier;
		}
		if (currentLevel == null)
		{
			currentLevel = LevelBase.InstantiateLevelClass(jsonText);
			if (currentLevel.loadingSuccessful)
			{
				currentLevel.levelType = LevelType.Regular;
			}
			else if (base.editor != null)
			{
				base.editor.ShowLevelLoadingError(LevelErrorName.LevelDataNull);
			}
		}
		RDLevelData current;
		RDRichPresence rDRichPresence;
		int num;
		string text3;
		if (currentLevel != null)
		{
			if (flag)
			{
				currentLevel.levelType = LevelType.Tutorial;
				currentLevel.levelToSkipTo = RDUtils.ParseEnum(internalIdentifier, Level.None);
			}
			else if (levelToLoadSource == LevelSource.CutscenesPath || currentLevel.cutsceneLevel)
			{
				currentLevel.levelType = LevelType.Cutscene;
			}
			current = RDLevelData.current;
			if (current != null)
			{
				conditionals = Conditionals.GetGlobalConditionals().Concat(current.conditionals).ToList();
				if (levelPersistence.ShouldLoad())
				{
					printe("Found persistent level data for " + levelPersistence.levelDir);
					levelPersistence.PrintDebug();
					loadedFromPersistence = true;
					persistenceStartImmediately = levelPersistence.startImmediately;
					if (levelPersistence.mistakes != null)
					{
						mistakesManager = levelPersistence.mistakes;
						mistakesAtStart = mistakesManager.Clone();
					}
					else if (loadedFromRestart && levelPersistence.restartLevelMistakes != null)
					{
						mistakesManager = levelPersistence.restartLevelMistakes.Clone();
						mistakesAtStart = mistakesManager.Clone();
					}
					levelPersistence.Clear();
				}
			}
			rDRichPresence = RDRichPresence.Instance;
			if (rDRichPresence != null)
			{
				if (levelToLoadSource == LevelSource.InternalPath)
				{
					num = ((!flag) ? 1 : 0);
					if (num != 0)
					{
						text3 = RDString.Get("levelSelect." + levelIdentifier);
						goto IL_04e4;
					}
				}
				else
				{
					num = 0;
				}
				text3 = current.settings.levelName;
				goto IL_04e4;
			}
		}
		goto IL_0557;
		IL_0640:
		if ((bool)base.editor || windowChoreographer != null)
		{
			int scale = (base.editor ? Persistence.GetRenderTextureScale() : (roomScale * 2));
			RenderTexture renderTexture = RDBase.Vfx.CreateRenderTexture(scale, "TopCamera");
			renderTexture.filterMode = FilterMode.Point;
			topCamera.camera.targetTexture = renderTexture;
			if (base.editorMode && windowChoreographer == null)
			{
				base.game.uiCamera.camera.targetTexture = renderTexture;
				base.game.uiCamera.camera.clearFlags = CameraClearFlags.Depth;
				base.game.windowCamera.camera.targetTexture = renderTexture;
				base.editor.gameView.texture = renderTexture;
			}
		}
		nurseCount = false;
		base.conductor.crotchetsPerBar = 8;
		GC.d_auto7th = true;
		RDBase.debugSettings.BeatSounds = true;
		loadDogMode = false;
		rows = new Row[16];
		for (int i = 0; i < 16; i++)
		{
			rows[i] = new Row(i);
			rowSortOrder.Add(i);
		}
		for (int j = 0; j < 200; j++)
		{
			beatAlarm.Add(item: true);
			beatLoopsOneshot.Add(null);
		}
		shadowRowStyle = ((currentLevel.data != null) ? ShadowRowStyle.ShadowToHost : ShadowRowStyle.HostToShadowIf2P);
		if (DebugSettings.instance.RecordingMode)
		{
			persistenceStartImmediately = true;
		}
		timerText.transform.parent.gameObject.SetActive(value: false);
		UnityEngine.Object.Destroy(base.conductor.gameObject.GetComponent<AudioListener>());
		listener = listenerObject.AddComponent<AudioListener>();
		if (base.editorMode)
		{
			listener.enabled = false;
		}
		StartCoroutine(LoadingRoutine());
		return;
		IL_0557:
		if (!loadedFromPersistence)
		{
			ClearLevelPersistence();
		}
		LevelType levelType = currentLevel.levelType;
		bool active = GC.twoPlayerMode && !persistenceStartImmediately && !base.editorMode && levelType != LevelType.Tutorial && levelType != LevelType.Cutscene && levelType != LevelType.Intermission;
		switchPlayersContainer.SetActive(active);
		EnsureSwitchPlayersState();
		if (RDC.windowDance)
		{
			RDLevelData current2 = RDLevelData.current;
			if (current2 != null && current2.usesWindowDance)
			{
				windowChoreographer = ((RDC.windowMovement == WindowMovement.Simulate) ? ((WindowChoreographer)new VirtualWindowChoreographer()) : ((WindowChoreographer)new RealWindowChoreographer(mainWindowIsDancer: false)));
				RDCamera[] array = uiCameras;
				foreach (RDCamera obj in array)
				{
					obj.camera.enabled = false;
					obj.enabled = false;
				}
				StartCoroutine(windowChoreographer.Setup(currentLevel.data.windowCount));
				goto IL_0640;
			}
		}
		scrVfxControl.CheckForWindowDanceChanges();
		goto IL_0640;
		IL_04e4:
		string cachedLevelName = text3;
		if (num == 0 && current.settings.song.IsNullOrEmpty() && current.settings.artist.IsNullOrEmpty() && levelPersistence.rootLevelName != null)
		{
			cachedLevelName = levelPersistence.rootLevelName;
		}
		rDRichPresence.cachedLevelType = currentLevel.levelType;
		rDRichPresence.cachedLevelName = cachedLevelName;
		rDRichPresence.isCustomLevel = levelToLoadSource == LevelSource.ExternalPath;
		rDRichPresence.UpdatePresence();
		goto IL_0557;
	}

	private void OnDestroy()
	{
		windowChoreographer?.OnDestroy();
	}

	private IEnumerator LoadingRoutine()
	{
		if (showLoadingScreen)
		{
			bladesContainer.gameObject.SetActive(value: true);
			handControllerOnTop.ShowHandButtons();
			handControllerOnTop.SetHandsInFront(inFront: true, beforeGameStart: true);
			handControllerOnTop.transform.localPosition = handControllerOnTop.transform.localPosition.WithZ(70f);
			handControllerOnTop.moveButtonsWithArms = false;
		}
		int num = ((!currentLevel.multiroom) ? 1 : 4);
		bool multiroom = currentLevel.multiroom;
		rooms = new RDRoom[num];
		for (int i = 0; i < num; i++)
		{
			GameObject gameObject = Room.Instantiate("Room", makeContainer: false);
			gameObject.transform.parent = roomsContainer;
			rooms[i] = gameObject.GetComponent<RDRoom>();
			rooms[i].Setup(i, multiroom);
		}
		Camera hudCamera = RDBase.Vfx.hudCamera;
		if (multiroom)
		{
			hudCamera.clearFlags = CameraClearFlags.Color;
			hudCamera.backgroundColor = Color.black;
		}
		else
		{
			hudCamera.clearFlags = CameraClearFlags.Depth;
		}
		RDBase.Vfx.LoadGameEffects();
		wrldCamera = rooms[0].camera;
		listenerObject.transform.SetParent(wrldCamera.gameObject.transform, worldPositionStays: true);
		yield return null;
		currentLevel.PrepareMusic();
		if (levelToLoadSource == LevelSource.InternalPath && currentLevel.levelType == LevelType.Regular)
		{
			currentLevel.SetRankDescriptionsFromLocalizationData(levelIdentifier);
		}
		if (windowChoreographer != null)
		{
			yield return new WaitUntil(() => windowChoreographer.setup);
		}
		if (persistenceStartImmediately)
		{
			windowChoreographer?.StartUsingCustomWindows();
		}
		yield return currentLevel.LoadBigAssets();
		if (currentLevel.data != null)
		{
			yield return currentLevel.LoadCustomAssets();
		}
		_ = Singleton<AudioManager>.Instance;
		yield return null;
		levelFinishedLoading = true;
		if (showLoadingScreen)
		{
			if (scnBase.isMobile)
			{
				LEDSign.status = "";
				MobileStatusText.SetActive(value: true);
			}
			else
			{
				string key = (currentLevel.dogMode ? "rhythmDogtor." : "") + (GC.twoPlayerMode ? "status.beginLevelInstructions2P" : "status.beginLevelInstructions");
				beginLevel = RDString.Get(key);
				statusText.SetStatusText(beginLevel);
				statusText.hideTextAfterSomeTime = false;
			}
			if (Narration.IsEnabled && !base.editorMode)
			{
				RDBase.PlaySound("sndPressToContinue", 1f, RDUtils.GetMixerGroup("InterfaceSoundsParent"));
			}
			string withCheck = RDString.GetWithCheck(levelIdentifier + ".extraCredits", out var exists);
			if (exists)
			{
				rankscreen.ShowExtraCredits(withCheck.Trim());
			}
			if (currentLevel is Level_AthleteFinale && SteamWorkshop.OverlayEnabled() && scnBase.isMac && base.game.windowChoreographer is RealWindowChoreographer)
			{
				rankscreen.ShowExtraCredits(RDString.Get("level.AthleteFinale.warning"), 1f);
			}
			RDBase.Vfx.BlackScreenOn();
		}
		if (multiplayerAppearance == MultiplayerAppearance.Nothing)
		{
			RDBase.Vfx.SetHorizontalStripsVisible(visible: false);
		}
		SpeedrunValues.isLoading = false;
		if (showLoadingScreen)
		{
			handControllerOnTop.Show(show: true, instant: true);
			handControllerOnTop.SlideIn();
		}
		else if (base.editor == null || !base.editor.playAsSoonItReloads)
		{
			yield return StartTheGame(levelSpeed);
		}
		base.editor?.ipm.UpdateBlankPanel();
	}

	private void TouchUpdate()
	{
		if (!scnBase.isMobile)
		{
			return;
		}
		Touch[] touches = Input.touches;
		for (int i = 0; i < touches.Length; i++)
		{
			Touch touch = touches[i];
			if (touch.phase != TouchPhase.Began)
			{
				continue;
			}
			if (touch.position.x < (float)Screen.width * 0.5f)
			{
				if (leftFinger == -1)
				{
					leftFinger = touch.fingerId;
					break;
				}
			}
			else if (rightFinger == -1)
			{
				rightFinger = touch.fingerId;
				break;
			}
		}
		touches = Input.touches;
		for (int i = 0; i < touches.Length; i++)
		{
			Touch touch2 = touches[i];
			if (leftFinger == touch2.fingerId && touch2.phase == TouchPhase.Ended)
			{
				leftFinger = -1;
			}
			if (rightFinger == touch2.fingerId && touch2.phase == TouchPhase.Ended)
			{
				rightFinger = -1;
			}
		}
	}

	public void TogglePauseGame()
	{
		if (pauseBlocked)
		{
			return;
		}
		paused = !paused;
		if (paused)
		{
			if (!base.audioPaused)
			{
				unpauseAudioOnResume = true;
				base.audioPaused = true;
			}
			Time.timeScale = 0f;
		}
		else
		{
			if (unpauseAudioOnResume)
			{
				base.audioPaused = false;
			}
			Time.timeScale = visualSpeed;
		}
		if (!base.editorMode)
		{
			Cursor.visible = paused;
			windowDanceMovement.SetActive(paused && levelIdentifier == "Boss2" && windowDanceEventCalled && windowChoreographer is RealWindowChoreographer && base.platformHelper.Monitors.Count > 1);
			pauseMenu.backgroundCanvasGroup.DOKill();
			if (paused)
			{
				pauseMenu.Show();
				pauseMenu.backgroundCanvasGroup.DOFade(1f, 0.15f).SetUpdate(isIndependentUpdate: true);
			}
			else
			{
				statusText.gameObject.SetActive(value: true);
				pauseMenu.Hide();
				pauseMenu.backgroundCanvasGroup.DOFade(0f, 0.15f).SetUpdate(isIndependentUpdate: true);
				if (pauseMenu.canvasGroup.alpha < 1f)
				{
					pauseMenu.canvasGroup.DOKill();
					pauseMenu.canvasGroup.DOFade(1f, 0.075f).SetUpdate(isIndependentUpdate: true);
				}
			}
		}
		if (startTheGameCalled)
		{
			windowChoreographer?.OnPauseChanged(paused);
		}
		if (!paused)
		{
			SetEnabledCameras(enabled: true);
		}
	}

	public void SetEnabledCameras(bool enabled)
	{
		RDRoom[] array = rooms;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].camera.enabled = enabled;
		}
		RDBase.Vfx.hudCamera.enabled = enabled;
		if (base.editorMode && windowChoreographer == null)
		{
			RDCamera[] array2 = uiCameras;
			foreach (RDCamera obj in array2)
			{
				obj.enabled = enabled;
				obj.camera.enabled = enabled;
			}
		}
	}

	public void Quit()
	{
		ClearLevelPersistence();
		float a = base.conductor.crotchet * 2f;
		a = Mathf.Min(a, 1f);
		StartCoroutine(PixelateAnimation(a));
		Timer.Add(QuitTimer, a);
		scrConductor.PlayImmediatelyIgnoringTimeScale("sndTransitionOut", 1f, RDUtils.GetMixerGroup("LevelInterface"));
	}

	private IEnumerator PixelateAnimation(float time)
	{
		Image overlay = pauseMenu.overlay;
		overlay.gameObject.SetActive(value: true);
		overlay.color = Color.clear;
		overlay.DOColor(Color.black, time).SetUpdate(isIndependentUpdate: true).SetEase(Ease.OutQuad);
		float delta = Time.unscaledDeltaTime;
		imageEffectPixelate.enabled = true;
		for (float t = 0f; t < time; t += delta)
		{
			imageEffectPixelate.time = Mathf.Lerp(0f, 8f, t / time);
			yield return null;
		}
	}

	private void QuitTimer()
	{
		Time.timeScale = 1f;
		RDTime.speed = 1f;
		Singleton<AudioManager>.Instance.StopAllSounds();
		RDUtils.SetMixerVolume("LevelMasterVolume", 1f);
		if (!RDBase.debugSettings.Debug && currentLevel.levelType == LevelType.Intro)
		{
			scnBase.GoToMainMenu();
			return;
		}
		scnLevelSelect.playerQuitLevel = true;
		if (!CheckForSpecialRoute())
		{
			scnBase.GoToLevelSelect();
		}
	}

	public void Restart(bool fromCheckpoint = true)
	{
		if ((currentLevel.levelType == LevelType.Intro || currentLevel.levelType == LevelType.Tutorial) && !RDBase.debugSettings.Debug)
		{
			return;
		}
		Row[] array = rows;
		foreach (Row row in array)
		{
			if ((bool)row.ent)
			{
				DeleteRow(row.ent.rowID);
			}
		}
		if (currentLevel is Level_Lesmis { dogMode: not false })
		{
			loadDogMode = true;
		}
		BlockControls();
		OpenBlades(instant: true);
		CloseBlades(base.gc.bladeRotationCloseDuration);
		Timer.Add(RestartTimer, base.gc.bladeRotationCloseDuration);
		if (windowChoreographer is VirtualWindowChoreographer)
		{
			pauseMenu.backgroundCanvasGroup.DOKill();
			pauseMenu.backgroundCanvasGroup.DOFade(0f, 0.3f).SetUpdate(isIndependentUpdate: true);
			pauseMenu.Hide();
		}
		if (!fromCheckpoint)
		{
			lastBarDiedAt = 0;
		}
		else if (lastBarDiedAt == 0)
		{
			lastBarDiedAt = base.conductor.barNumber;
		}
		printe("barToScrubTo = " + TEST_barToScrubToStatic);
	}

	private void RestartTimer()
	{
		IncrementLevelTries();
		isRestartingLevel = true;
		if (levelPersistence.ShouldLoad() && !levelPersistence.restartLevelPath.IsNullOrEmpty())
		{
			scnBase.GoToLevel(levelPersistence.restartLevelPath);
		}
		else
		{
			scnBase.GoToScene("scnGame");
		}
	}

	public void IncrementLevelTries()
	{
		if (levelToLoadSource != LevelSource.ExternalPath && currentLevel != null)
		{
			LevelType levelType = currentLevel.levelType;
			if (levelType != LevelType.Cutscene && levelType != LevelType.Tutorial && levelType != LevelType.Intro)
			{
				int tries = Persistence.GetTriesToClearLevel(levelIdentifier) + 1;
				Persistence.SetTriesToClearLevel(levelIdentifier, tries);
			}
		}
	}

	private bool CheckForSpecialRoute()
	{
		if (goToScnMenuAfterHelpingHands && levelIdentifier == "HelpingHands")
		{
			scnBase.GoToMainMenu();
			goToScnMenuAfterHelpingHands = false;
			return true;
		}
		return false;
	}

	public void EndLevel()
	{
		RDUtils.SetMixerVolume("LevelMasterVolume", 1f);
		if (!CheckForSpecialRoute())
		{
			scnBase.GoToLevelSelect();
		}
	}

	public void SetVisualSpeed(float speed)
	{
		visualSpeed = RDTime.speed * speed;
		Time.timeScale = visualSpeed * oneshotSlowdownMultiplier;
	}

	public void SetVisualSpeed(float speed, float beat)
	{
		scrExecuteOnCertainBeat.Add(() =>
		{
			SetVisualSpeed(speed);
		}, beat);
	}

	public void SetSlowdown(float speed)
	{
		oneshotSlowdownMultiplier = speed;
		SetVisualSpeed(visualSpeed);
	}

	private new void Update()
	{
		if (currentLevel != null)
		{
			currentLevel.deltaTime = Time.deltaTime;
		}
		base.Update();
		TouchUpdate();
		bool anyPlayerPress = RDInput.anyPlayerPress;
		bool p1Press = RDInput.p1Press;
		bool p2Press = RDInput.p2Press;
		bool p1IsPressed = RDInput.p1IsPressed;
		bool p2IsPressed = RDInput.p2IsPressed;
		bool p1Release = RDInput.p1Release;
		bool p2Release = RDInput.p2Release;
		bool flag = KeyCode.LeftShift.IsPressed() || KeyCode.RightShift.IsPressed();
		bool flag2 = base.editorMode && (base.editor.userIsEditingAnInputField || base.editor.inspectorPanelManager.IsSelectEventPanelVisible());
		float value = 1f;
		if (base.editorMode)
		{
			value = base.editor.levelSettings.levelVolume;
		}
		else if (currentLevel.data != null)
		{
			value = currentLevel.data.settings.levelVolume;
		}
		RDUtils.SetMixerVolume("LevelMasterVolume", value);
		RDUtils.SetMixerVolume("PlayerOneMistakesVolume", Mathf.Lerp(1f, 0.45f, 0.2f * (float)(mistakesManager.consecutiveMistakesP1 - 2)));
		RDUtils.SetMixerVolume("PlayerTwoMistakesVolume", Mathf.Lerp(1f, 0.45f, 0.2f * (float)(mistakesManager.consecutiveMistakesP2 - 2)));
		bool flag3 = false;
		if (!flag2)
		{
			if (base.editorMode)
			{
				if (flag && KeyCode.O.WentDown() && !RDInput.controlIsPressed)
				{
					GC.showAbsoluteOffsets = !GC.showAbsoluteOffsets;
					Persistence.SetFrameOffsets(GC.showAbsoluteOffsets);
					LEDSign.status = (GC.showAbsoluteOffsets ? RDString.Get("status.frameOffsets.On") : RDString.Get("status.frameOffsets.Off"));
				}
				if (flag && KeyCode.C.WentDown() && !RDInput.controlIsPressed)
				{
					GC.d_showMarginsNumerically = !GC.d_showMarginsNumerically;
					Persistence.SetErrorMargins(GC.d_showMarginsNumerically);
					LEDSign.status = (GC.d_showMarginsNumerically ? RDString.Get("status.errorMargins.On") : RDString.Get("status.errorMargins.Off"));
				}
			}
			if (!base.editorMode)
			{
				SingleLevelUpdate();
				scrVfxControl.CheckKeyComboForResolutionChange();
				if (scnBase.isDivekick)
				{
					if (p1Press)
					{
						p1WentDownTimeStamp = Time.timeSinceLevelLoad;
					}
					if (p1Release)
					{
						p1WentDownTimeStamp = -100f;
					}
					if (p2Press)
					{
						p2WentDownTimeStamp = Time.timeSinceLevelLoad;
					}
					if (p2Release)
					{
						p2WentDownTimeStamp = -100f;
					}
					if (((p1IsPressed && Time.timeSinceLevelLoad - p1WentDownTimeStamp > 2f) & p2IsPressed) && Time.timeSinceLevelLoad - p2WentDownTimeStamp > 2f)
					{
						printe("Divekick special Quit/Skip");
						if (levelIsSkippable)
						{
							return;
						}
						Quit();
					}
				}
				if (RDInput.cancelPress && !paused)
				{
					PauseMenu.contentsToSelect = new List<PauseContentName>
					{
						PauseContentName.Continue,
						PauseContentName.Dog_Continue
					};
					TogglePauseGame();
				}
			}
		}
		if ((((RDBase.debugSettings.Debug || Debug.isDebugBuild || Application.isEditor) && started) & flag) && !flag2)
		{
			if (KeyCode.A.WentDown())
			{
				ToggleAutoplay();
			}
			if (KeyCode.B.WentDown())
			{
				RDBase.debugSettings.BeatSounds = !RDBase.debugSettings.BeatSounds;
				LEDSign.status = (RDBase.debugSettings.BeatSounds ? "BEATSOUNDS ON!" : "BEATSOUNDS OFF!");
			}
			if (KeyCode.F.WentDown())
			{
				base.conductor.stayOnBar = !base.conductor.stayOnBar;
				LEDSign.status = (base.conductor.stayOnBar ? "STAYING ON BAR ON!" : "STAYING ON BAR OFF!");
			}
			if (KeyCode.H.WentDown())
			{
				HealRowsToMinimum(3f);
			}
			if (KeyCode.K.WentDown() && rows != null && rows[1] != null && rows[1].ent != null)
			{
				OnMistakeOrHeal(0f, 1f, rows[1]);
				rows[1].ent.CrackAdvance(5f);
			}
			if (KeyCode.L.WentDown())
			{
				foreach (scrExecuteOnHit item in RDExecuteOn.FindAll<scrExecuteOnHit>())
				{
					if (item.hitType == HitType.Hit)
					{
						item.action();
						if (!item.persistent || item.autodelete)
						{
							item.Remove();
						}
					}
				}
			}
			if (KeyCode.M.WentDown())
			{
				nurseCount = !nurseCount;
				LEDSign.status = (nurseCount ? "MIAW MIAW ON!" : "MIAW OFF!");
			}
			if (KeyCode.I.WentDown())
			{
				RDBase.debugSettings.InstantDialogue = !RDBase.debugSettings.InstantDialogue;
				Persistence.SetInstantDialogue(RDBase.debugSettings.InstantDialogue);
			}
			if (KeyCode.P.WentDown())
			{
				ReviveDeadRows();
			}
			if (KeyCode.S.WentDown())
			{
				base.gm.soft = !base.gm.soft;
			}
			if (KeyCode.U.WentDown())
			{
				handController0.Show(show: false);
			}
			if (KeyCode.V.WentDown())
			{
				handController0.Show(show: true);
			}
			if (KeyCode.W.WentDown())
			{
				WinLevel();
			}
			if (KeyCode.Y.WentDown())
			{
				for (int i = 0; i < 50; i++)
				{
					OnMistakeOrHeal(0f, 1f, rows[1]);
					rows[1].ent.CrackAdvance(1f, ignoreBoss2Hack: true);
				}
				rankscreen.AdvanceGameover();
			}
			if (Input.GetKeyDown(KeyCode.RightBracket) || Input.GetKeyDown(KeyCode.Alpha9))
			{
				RDBase.Vfx.PlayBassDropEffect();
				base.conductor.ScrubToNextBar();
			}
			if (KeyCode.Quote.WentDown())
			{
				StopBeats();
			}
			if (KeyCode.Slash.WentDown())
			{
				StartCoroutine(base.conductor.ScrubToBarNum(barToSkipToOnSlash));
			}
			if (KeyCode.Period.WentDown())
			{
				base.conductor.ToggleMuteCurrentSong();
			}
			if (KeyCode.LeftBracket.WentDown() && RDInput.controlIsPressed)
			{
				SetVisualSpeed(Time.timeScale / 2f);
				LEDSign.status = "SPEED IS NOW " + Time.timeScale;
			}
			if (KeyCode.RightBracket.WentDown() && RDInput.controlIsPressed)
			{
				SetVisualSpeed(Time.timeScale * 2f);
				LEDSign.status = "SPEED IS NOW " + Time.timeScale;
			}
		}
		if (!flag2)
		{
			bool flag4 = this.gameState == GameState.PreStart && !paused && levelFinishedLoading;
			if (flag4 && !handControllerOnTop.slidingIn)
			{
				if (!narratedLevelReady)
				{
					if (GC.twoPlayerMode)
					{
						Narration.Say(beginLevel, NarrationCategory.Instruction, false, NarrationActionName.ToSwitchPlayers);
					}
					else
					{
						Narration.Say(beginLevel, NarrationCategory.Instruction, false);
					}
					narratedLevelReady = true;
				}
				bool flag5 = anyPlayerPress && !GC.twoPlayerMode;
				bool flag6 = (p1IsPressed & p2IsPressed) && GC.twoPlayerMode;
				bool num = ((p1Press && !p2IsPressed) || (p2Press & p2IsPressed)) && GC.twoPlayerMode;
				bool flag7 = flag5 | flag6;
				if (num)
				{
					if (!p2IsPressed)
					{
						scrConductor.PlayFeedback(GameSoundType.Hand1PopSound, 1f, RDUtils.GetMixerGroup("PlayerOneHandPops"), 1f, RDPlayer.NoChange, isBeatSound: false, ignoreListenerPause: true);
					}
					else
					{
						scrConductor.PlayFeedback(GameSoundType.Hand2PopSound, 1f, RDUtils.GetMixerGroup("PlayerTwoHandPops"), 1f, RDPlayer.NoChange, isBeatSound: false, ignoreListenerPause: true);
					}
				}
				else if (flag7)
				{
					StartCoroutine(StartTheGame(levelSpeed));
					flag3 = true;
					return;
				}
			}
			if (!flag3 && (Input.GetKey(KeyCode.LeftShift) || !base.editorMode) && flag4)
			{
				bool flag8 = false;
				bool flag9 = !KeyCode.D.WentDown() && false;
				bool flag10 = !KeyCode.S.WentDown() && false;
				if (flag9 | flag10 | flag8)
				{
					if (GC.allowDoubleSpeed)
					{
						if (flag8)
						{
							StartCoroutine(StartTheGame(base.gc.customSpeed, "starting customspeed..."));
						}
						else if (flag9)
						{
							StartCoroutine(StartTheGame(1.5f, RDString.Get("status.startingDoublespeed")));
						}
						else if (flag10)
						{
							StartCoroutine(StartTheGame(0.75f, RDString.Get("status.startingHalfspeed")));
						}
						if (base.editorMode)
						{
							base.editor.scrubbing = false;
						}
						flag3 = true;
					}
					else
					{
						LEDSign.status = "no customspeed sorry, too glitchy!";
					}
				}
				else if (GC.twoPlayerMode && (RDInput.leftPress || RDInput.rightPress))
				{
					RDInput.SwapP1AndP2Controls(changePersistence: true);
				}
			}
		}
		if (this.gameState == GameState.CutsceneSkippable && !paused)
		{
			if (skippedLastSkippableCutscene)
			{
				skippedLastSkippableCutscene = false;
			}
			if (skippingCutsceneElapsedTime > HoldDurationToSkipCutscene)
			{
				skippingCutsceneElapsedTime = 0f;
				this.gameState = GameState.Cutscene;
				(int, int) tuple = Array.Find(currentLevel.cutsceneBarsDesc, ((int startBar, int endBar) x) => x.startBar < base.conductor.barNumber);
				if (tuple.Item2 > base.conductor.barNumber)
				{
					StartCoroutine(base.conductor.ScrubToBarNum(tuple.Item2));
				}
				else
				{
					Debug.LogWarning("Cutscene endpoint has already passed, but we're still in the cutscene??");
				}
				rankscreen.SkippableLabel.text = RDString.Get("status.skipping");
				if (!narratedSkippingLabel)
				{
					Narration.Say(rankscreen.SkippableLabel.text, NarrationCategory.Notification, true);
					narratedSkippingLabel = true;
					narratedSkipLabel = false;
					narratedKeepHoldingLabel = false;
				}
				skippedLastSkippableCutscene = true;
				flag3 = true;
			}
			else if (p1IsPressed | p2IsPressed)
			{
				string text = "";
				text = (currentLevel.dogMode ? "rhythmDogtor.status.keepHoldingInstructions" : (drumMode ? "status.keepHoldingInstructions.drumMode" : ((!tapsOnly) ? "status.keepHoldingInstructions" : "status.keepHoldingInstructions.tapsOnly")));
				rankscreen.SkippableLabel.text = RDString.Get(text);
				if (!narratedKeepHoldingLabel)
				{
					Narration.Say(rankscreen.SkippableLabel.text, NarrationCategory.Notification, true);
					narratedKeepHoldingLabel = true;
				}
				skippingCutsceneElapsedTime += Time.unscaledDeltaTime;
				flag3 = true;
			}
			else
			{
				string key = (currentLevel.dogMode ? "rhythmDogtor.status.skipCutsceneInstructions" : (drumMode ? "status.skipCutsceneInstructions.drumMode" : (tapsOnly ? "status.skipCutsceneInstructions.tapsOnly" : "status.skipCutsceneInstructions")));
				rankscreen.SkippableLabel.text = RDString.Get(key);
				if (!narratedSkipLabel)
				{
					Narration.Say(rankscreen.SkippableLabel.text, NarrationCategory.Notification, false);
					narratedSkipLabel = true;
					narratedKeepHoldingLabel = false;
					narratedSkippingLabel = false;
				}
			}
		}
		if (receptive && !flag3 && !paused && currentLevel.levelType != LevelType.Cutscene)
		{
			bool flag11 = RDInput.p1Press || (RDInput.p2Press && !GC.twoPlayerMode);
			bool keyReleased = RDInput.p1Release || (RDInput.p2Release && !GC.twoPlayerMode);
			bool key2 = RDInput.emuStates[0].GetKey(RDInput.PlayerEmuKey.Down);
			bool keyPressed = (flag2 & key2) || (!flag2 & flag11);
			UpdateGameplayInput(RDPlayer.P1, keyPressed, keyReleased);
			if (GC.twoPlayerMode)
			{
				bool flag12 = p2Press;
				bool key3 = RDInput.emuStates[1].GetKey(RDInput.PlayerEmuKey.Down);
				bool keyPressed2 = (flag2 & key3) || (!flag2 & flag12);
				UpdateGameplayInput(RDPlayer.P2, keyPressed2, p2Release);
			}
			if (currentLevel.data != null)
			{
				List<TimedAction> list = new List<TimedAction>();
				foreach (TimedAction timedAction in timedActions)
				{
					if (Time.time > timedAction.timeOfEnd)
					{
						list.Add(timedAction);
						continue;
					}
					bool flag13 = true;
					foreach (Conditional conditional in conditionals)
					{
						foreach (ConditionalID conditional2 in timedAction.conditionals)
						{
							if (conditional.Matches(conditional2) && !conditional.Check(timedAction.levelEvent, conditional2.negated))
							{
								flag13 = false;
								break;
							}
						}
						if (!flag13)
						{
							break;
						}
					}
					if (flag13)
					{
						timedAction.levelEvent.timesRun++;
						timedAction.action();
					}
				}
				foreach (TimedAction item2 in list)
				{
					timedActions.Remove(item2);
				}
			}
		}
		if (gameStateWatcher != this.gameState)
		{
			bool flag14 = currentLevel != null && currentLevel.shouldMakeHealthBar;
			GameState gameState = this.gameState;
			bool show = gameState == GameState.Cutscene || gameState == GameState.CutsceneSkippable || gameState == GameState.HandmodePreCutscene;
			ShowCutsceneBars(show);
			if (this.gameState == GameState.Handmode && gameStateWatcher != GameState.Handmode && gameStateWatcher != GameState.SpacePressedPreStart && flag14 && !hpController.settingVisibilityManually)
			{
				hpController.Show(show: true);
			}
			if (this.gameState != GameState.Handmode && gameStateWatcher == GameState.Handmode && flag14 && !hpController.settingVisibilityManually)
			{
				hpController.Show(show: false);
			}
			gameStateWatcher = this.gameState;
		}
		if (syncHitRemainingTime > 0f)
		{
			syncHitRemainingTime -= RDTime.deltaTimeIndependentPauseAffected;
		}
		if (!p1IsPressed && !p2IsPressed)
		{
			skippingCutsceneElapsedTime = 0f;
		}
		if (RDBase.debugSettings.Debug)
		{
			debugText.gameObject.SetActive(value: true);
			debugText.color = Color.white;
			framesElapsed++;
			timeReference = Time.realtimeSinceStartup;
			if (timeReference - lastTimeReference >= 1f)
			{
				lastTimeReference++;
				fps = framesElapsed;
				framesElapsed = 0;
			}
			string arg = ((fps < 30) ? "#ff0000" : ((fps < 50) ? "#ffff00" : "#00ff00"));
			debugString.Clear();
			debugString.AppendFormat("\nFPS: <color={0}>{1}</color> (frame #{2})", arg, fps, Time.frameCount);
			debugString.AppendFormat("\nLevel: {0}", levelIdentifier);
			debugString.AppendFormat("\nBar: {0}, {1:F2}", base.conductor.barNumber, base.conductor.currentBeat);
			debugString.AppendFormat("\nMistakes: {0}", mistakesManager.mistakes);
			debugString.AppendFormat("\nAuto: {0}", RDBase.debugSettings.Auto ? "<color=00ff00>on</color>" : "off");
			debugString.AppendLine(customDebugText);
			debugText.text = debugString.ToString();
		}
		else
		{
			debugText.gameObject.SetActive(value: false);
		}
		if (DebugSettings.instance.RecordingMode && levelFinishedLoading)
		{
			recordingFPSTimer += Time.deltaTime;
			if (recordingFPSTimer >= 5f)
			{
				recordingFPSTimer = 0f;
				minFPS = ((minFPS == -1f) ? ((float)fps) : Mathf.Min(minFPS, fps));
				maxFPS = ((maxFPS == -1f) ? ((float)fps) : Mathf.Max(maxFPS, fps));
				recordedFPSList.Add(fps);
			}
		}
		currentLevel?.Update();
	}

	private void LateUpdate()
	{
		windowChoreographer?.LateUpdate();
	}

	public void EnsureSwitchPlayersState()
	{
		if ((GC.twoPlayerMode && RDInput.persistenceSwapped != RDInput.actuallySwapped) || (!GC.twoPlayerMode && RDInput.actuallySwapped))
		{
			RDInput.SwapP1AndP2Controls(changePersistence: false);
		}
		else
		{
			SwitchPlayers();
		}
	}

	public void SwitchPlayers()
	{
		p1MarkImage.color = Color.white;
		p1MarkImage.DOColor(base.gc.player1StripColor, 0.3f);
		p2MarkImage.color = Color.white;
		p2MarkImage.DOColor(base.gc.player2StripColor, 0.3f);
		for (int i = 0; i < 5; i++)
		{
			scrHandController handControllerFromInt = GetHandControllerFromInt(i);
			if (RDInput.p1[0].schemeIndex == 0)
			{
				handControllerFromInt.leftArm.player = RDPlayer.P2;
				handControllerFromInt.leftArm.SetDefibMode(p2DefibMode);
				handControllerFromInt.rightArm.player = RDPlayer.P1;
				handControllerFromInt.rightArm.SetDefibMode(p1DefibMode);
				handControllerFromInt.leftArm.SetSkin(Persistence.p2Skin);
				handControllerFromInt.rightArm.SetSkin(Persistence.p1Skin);
			}
			else
			{
				handControllerFromInt.leftArm.player = RDPlayer.P1;
				handControllerFromInt.leftArm.SetDefibMode(p1DefibMode);
				handControllerFromInt.rightArm.player = RDPlayer.P2;
				handControllerFromInt.rightArm.SetDefibMode(p2DefibMode);
				handControllerFromInt.leftArm.SetSkin(Persistence.p1Skin);
				handControllerFromInt.rightArm.SetSkin(Persistence.p2Skin);
			}
		}
		if (RDInput.p1[0].schemeIndex == 0)
		{
			p1Mark.anchoredPosition = rightMarkPos;
			p2Mark.anchoredPosition = leftMarkPos;
		}
		else
		{
			p1Mark.anchoredPosition = leftMarkPos;
			p2Mark.anchoredPosition = rightMarkPos;
		}
	}

	public void UpdateGameplayInput(RDPlayer player, bool keyPressed, bool keyReleased)
	{
		if ((tapsOnly & keyPressed) && (bool)playerCaughtHoldBeat[(int)player])
		{
			keyPressed = false;
			keyReleased = true;
		}
		if (keyPressed)
		{
			spacebarOnNothing = true;
			playerCaughtHoldBeat[(int)player] = null;
			foreach (scrPlayerbox item in scrPlayerbox.ListAll())
			{
				if (rows[item.rowID].GetCurrentPlayer() == player)
				{
					item.SpaceBarEvent(CPUTriggered: false);
				}
			}
			if (drumMode && spacebarOnNothing && levelIdentifier != "SongOfTheSea")
			{
				FlashBorderFeedback(BorderFeedbackType.DrumHit);
			}
			else
			{
				Beat heldBeat = playerCaughtHoldBeat[(int)player];
				hitStripManager.ShowHitstrip(player, heldBeat);
			}
			if (spacebarOnNothing)
			{
				if (!drumMode || levelIdentifier == "SongOfTheSea")
				{
					switch (player)
					{
					case RDPlayer.P1:
						scrConductor.PlayFeedback(GameSoundType.Hand1PopSound, 1f, RDUtils.GetMixerGroup("PlayerOneHandPops"));
						break;
					case RDPlayer.P2:
						scrConductor.PlayFeedback(GameSoundType.Hand2PopSound, 1f, RDUtils.GetMixerGroup("PlayerTwoHandPops"));
						break;
					}
				}
				currentLevel.buttonPressCount++;
			}
		}
		if (!keyReleased)
		{
			return;
		}
		bool flag = false;
		foreach (scrPlayerbox item2 in scrPlayerbox.ListAll())
		{
			item2.SpaceBarReleased(player);
			flag = flag || item2.holdAssist;
		}
		if (!flag)
		{
			hitStripManager.Release(player);
		}
	}

	public void SkipLevel()
	{
		SkipLevelWithCallback(() =>
		{
			if (currentLevel.levelType == LevelType.Cutscene)
			{
				if (internalIdentifier == "cutscene_edega")
				{
					scnBase.GoToLevelWithEnum(Level.HelpingHands);
				}
				else
				{
					scnBase.GoToLevelSelect();
				}
			}
			else
			{
				scnBase.GoToLevelWithEnum(currentLevel.levelToSkipTo);
			}
		});
	}

	public void SkipLevelWithCallback(TweenCallback action)
	{
		if (!levelIsSkippable)
		{
			return;
		}
		BlockControls();
		pauseMenuOverlay.gameObject.SetActive(value: true);
		pauseMenuOverlay.color = Color.white.WithAlpha(0.8f).WithBrightness((float)Persistence.GetFlashIntensity() * 0.01f);
		pauseMenuOverlay.DOColor(Color.white.WithAlpha(0f), 0.4f).SetEase(Ease.Linear).SetUpdate(isIndependentUpdate: true);
		Timer.Add(() =>
		{
			float num = 1f;
			pauseMenuOverlay.gameObject.SetActive(value: true);
			pauseMenuOverlay.color = Color.clear;
			pauseMenuOverlay.DOColor(Color.black, num).SetEase(Ease.OutExpo).SetUpdate(isIndependentUpdate: true);
			Timer.Add(() =>
			{
				action();
			}, num);
		}, 0.4f);
	}

	private void BlockControls()
	{
		pauseMenu.enabled = false;
		base.enabled = false;
	}

	public void GoToNextLevel()
	{
		if (currentLevel.levelToSkipTo != Level.None)
		{
			scnBase.GoToLevelWithEnum(currentLevel.levelToSkipTo);
		}
	}

	public IEnumerator StartTheGame(float speed = 1f, string customMessage = "")
	{
		handControllerOnTop.moveButtonsWithArms = true;
		if (currentLevel.levelType != LevelType.Tutorial)
		{
			LevelBase levelBase = currentLevel;
			if (levelBase != null && levelBase.checkpointBarsDesc?.Length > 0 && levelIdentifier == Persistence.GetLastPlayedLevel())
			{
				int num = Array.Find(currentLevel.checkpointBarsDesc, (int x) => x <= lastBarDiedAt);
				if (num > 0 && !base.editorMode)
				{
					barToScrubTo = num;
					base.conductor.isScrubbing = true;
					windowChoreographer?.HideWindows();
				}
			}
			lastBarDiedAt = 0;
		}
		if (startTheGameCalled)
		{
			RDBaseDllDummy.printesw("scnGame.StartTheGame() has been called already!");
			yield break;
		}
		windowChoreographer?.StartUsingCustomWindows();
		p1HitTimes = new List<float>();
		p2HitTimes = new List<float>();
		if (currentLevel.levelType != LevelType.Tutorial && !base.editorMode)
		{
			levelSpeed = speed;
		}
		RDTime.speed = speed;
		SetVisualSpeed(1f);
		scnGame obj = this;
		SpeedrunValues.currentGameState = GameState.SpacePressedPreStart;
		obj.gameState = GameState.SpacePressedPreStart;
		if (showLoadingScreen)
		{
			statusText.SetStatusText(customMessage);
		}
		if (GC.twoPlayerMode)
		{
			switchPlayersContainer.SetActive(value: false);
		}
		base.gm.hudCamera.StopShake();
		MobileStatusText.SetActive(value: false);
		currentLevel.OnLevelStartedBase();
		base.conductor.OnPreBar();
		if (showLoadingScreen)
		{
			RDBase.Vfx.FadeIn(0f);
			scrConductor.PlayImmediatelyIgnoringTimeScale("sndSwitch", 1f, RDUtils.GetMixerGroup("LevelInterface"));
			OpenBlades();
			disappearBlades = DOVirtual.DelayedCall(base.gc.bladeRotationDuration, () =>
			{
				bladesContainer.gameObject.SetActive(value: false);
			});
		}
		else
		{
			RDBase.Vfx.FadeIn();
			bladesContainer.gameObject.SetActive(value: false);
			if (!currentLevel.noHands && !currentLevel.hideHandsAfterStart && !currentLevel.hideHandsOnStart)
			{
				handControllerOnTop.OnTopHandForceUpdate();
			}
		}
		rankscreen.HideExtraCredits();
		if (scnBase.isMobile)
		{
			MobilePauseButton.SetActive(value: true);
		}
		Narration.SetInstructions("", NarrationActionName.ToPause);
		yield return null;
		if (currentLevel.noHands)
		{
			handControllerOnTop.Show(show: false, instant: false, deactivateAfterHiding: true);
		}
		else if (currentLevel.hideHandsAfterStart || currentLevel.hideHandsOnStart)
		{
			handControllerOnTop.Show(show: false, instant: false, deactivateAfterHiding: false, updateHandsAfterHiding: true);
			handControllerOnTop.UnsetLastArmActions();
		}
		yield return null;
		dontLetGameStateToggleHands = true;
		gameState = GameState.Handmode;
		dontLetGameStateToggleHands = false;
		if (currentLevel.data != null)
		{
			handControllerOnTop.shouldShow = true;
		}
		if (base.conductor != null && base.conductor.barNumber < barToScrubTo)
		{
			yield return base.conductor.ScrubToBarNum(barToScrubTo);
		}
		if (!base.editorMode)
		{
			yield return new WaitForSeconds(1f);
		}
		if (showLoadingScreen)
		{
			LEDSign.status = "";
		}
		if (currentLevel.scrubbedGoToLevel != null)
		{
			currentLevel.scrubbedGoToLevel.PrepNextLevel();
		}
		base.editor?.ipm.UpdateBlankPanel();
	}

	public void SetGameState(GameState gameState, float delayInCrotchets = 0f)
	{
		if (delayInCrotchets != 0f)
		{
			scrExecuteOnCertainBeat.Add(() =>
			{
				this.gameState = gameState;
			}, delayInCrotchets);
		}
		else
		{
			this.gameState = gameState;
		}
	}

	public void ShowCutsceneBars(bool show)
	{
		if (!currentLevel.noCutsceneLetterbox)
		{
			RDBase.Vfx.ShowLetterbox(show);
		}
	}

	public override void OnPreBar()
	{
		if (base.conductor.playStyle != PlayStyle.BeatLoopOnly && !failedLevel)
		{
			currentLevel.basePreactions();
			currentLevel.preactions();
		}
		foreach (scrBeatLoop beatLoop in beatLoops)
		{
			beatLoop.Cycle();
		}
		if (currentLevel.pauseOn7thBeat)
		{
			base.conductor.PlayBeatInCrotchets("sndClickEcho", 6f, 1f, cuesoundsGroup);
			base.conductor.PlayBeatInCrotchets("sndClickEcho", 7f, 1f, cuesoundsGroup);
		}
	}

	public override void OnNewBar()
	{
		if (gameState == GameState.HandmodePreCutscene)
		{
			gameState = (SpeedrunValues.currentGameState = GameState.Cutscene);
		}
		if (base.conductor.playStyle != PlayStyle.BeatLoopOnly && !failedLevel)
		{
			currentLevel.baseActions();
			currentLevel.actions();
			if (currentLevel.levelType == LevelType.Tutorial && base.conductor.barNumber == 1)
			{
				Level level = (Level)Enum.Parse(typeof(Level), internalIdentifier);
				if (level != Level.Montage && Persistence.GetLevelRank(level).isNormalRank)
				{
					statusText.Show("status.skipTutorial", narrate: true);
					DOVirtual.DelayedCall(4f, () =>
					{
						statusText.Hide();
					});
				}
			}
		}
		foreach (Entity item in base.gm.entityPool)
		{
			if (item != null)
			{
				item.OnNewBar();
			}
		}
		Row[] array = rows;
		foreach (Row row in array)
		{
			if (row.ent != null)
			{
				row.ent.classicRowController.shine = 0f;
			}
		}
		if (currentLevel.pauseOn7thBeat)
		{
			scrExecuteOnCertainBeat.Add(currentLevel.ChallengeModeFakePause, 6f);
			scrExecuteOnCertainBeat.Add(currentLevel.ChallengeModeFakeResume, 7f);
		}
	}

	public BeatClassic AddBeat(int rowID, float startBeat, float tick = 999f, SwingBeatType beattype = SwingBeatType.Normal, float customswing = 0f, float holdTime = 0f, int length = 7, SoundData customBeatsound = null)
	{
		if (base.conductor.isScrubbing && (base.conductor.BeatToTime(tick * (float)(length - 1) + startBeat) < base.conductor.barLength || base.conductor.scrubDestination > base.conductor.barNumber + 3))
		{
			return null;
		}
		if (tick == 999f)
		{
			tick = rows[rowID].tick;
		}
		BeatClassic beatClassic = new BeatClassic(rowID).Add7BeatsClassic(startBeat, tick, beattype, customswing, 0f, rows[rowID].syncoBeat, rows[rowID].syncoSwing, length, customBeatsound);
		beats.Add(beatClassic);
		return beatClassic;
	}

	public BeatClassic AddBeatFree(int rowID, float[] arrTimes, bool playerDrives7thBeat = false, bool _isLenientMargins = false, float offset = 0f)
	{
		if (base.conductor.isScrubbing)
		{
			return null;
		}
		if (offset != 0f)
		{
			for (int i = 0; i < arrTimes.Length; i++)
			{
				arrTimes[i] += offset;
			}
		}
		BeatClassic beatClassic = new BeatClassic(rowID).Add7BeatsFreetime(arrTimes, playerDrives7thBeat, _isLenientMargins);
		beats.Add(beatClassic);
		return beatClassic;
	}

	public BeatClassic AddBeatFreeFlex(int rowID, int[] arrBeatboxes, float[] arrTimesFSAB, float[] arrReleaseTimesFSAB = null)
	{
		if (base.conductor.isScrubbing && (arrTimesFSAB.Last() < base.conductor.barLength || base.conductor.scrubDestination > base.conductor.barNumber + 3))
		{
			return null;
		}
		BeatClassic beatClassic = new BeatClassic(rowID).Add7BeatsFreetimeFlexible(arrBeatboxes, arrTimesFSAB, arrReleaseTimesFSAB);
		beats.Add(beatClassic);
		return beatClassic;
	}

	public BeatOneshot AddBeatOneshot(int rowID, float startBeat, float tick = 999f, OneshotPulseType pulseType = OneshotPulseType.Wave, string customHitSound = "", float delay = 0f, float delayBeatOffset = 0f, float visualFreezeBeatOverride = 0f, int subdivCount = 1, int subdivIndex = 0, bool isBurnshot = false, float holdDuration = 0f, float holdCueOffset = 0f)
	{
		if (base.conductor.isScrubbing)
		{
			return null;
		}
		if (tick == 999f)
		{
			tick = rows[rowID].tick;
		}
		BeatOneshot beatOneshot = new BeatOneshot(rowID).AddBeatOneShot(startBeat, tick, pulseType, customHitSound, delay, delayBeatOffset, visualFreezeBeatOverride, subdivCount, subdivIndex, isBurnshot, holdDuration, holdCueOffset);
		beats.Add(beatOneshot);
		return beatOneshot;
	}

	public BeatClassic AddBeatHold(int rowID, float startBeat, float tick = 999f, float holdTimeInCrotchets = 1f, int length = 7, SoundData customBeatsound = null)
	{
		if (base.conductor.isScrubbing && (base.conductor.BeatToTime(tick * (float)(length - 1) + startBeat) < base.conductor.barLength || base.conductor.scrubDestination > base.conductor.barNumber + 3))
		{
			return null;
		}
		if (tick == 999f)
		{
			tick = rows[rowID].tick;
		}
		BeatClassic beatClassic = new BeatClassic(rowID).Add7BeatsClassic(startBeat, tick, SwingBeatType.Normal, 0f, holdTimeInCrotchets, -1, 0f, length, customBeatsound);
		beats.Add(beatClassic);
		return beatClassic;
	}

	public void OnBeatClassic(int i, int _rowID, float _startbeat, float _tick = 999f, int _cyclefreq = 1, SwingBeatType beattype = SwingBeatType.Normal, bool offOtherBeats = false, float customswing = 0f)
	{
		foreach (scrBeatLoop beatLoop in beatLoops)
		{
			if (beatLoop.i == i && beatLoop.rowID == _rowID && beatLoop.startbeat == _startbeat && beatLoop.tick == _tick)
			{
				OnBeat(i);
				return;
			}
		}
		if (offOtherBeats)
		{
			OffAllBeatLoops();
		}
		beatAlarm[i] = true;
		new scrBeatLoop().SetupClassic(i, _rowID, _startbeat, _tick, _cyclefreq, beattype, customswing);
	}

	public void OnBeatHold(int _i, int _rowID, float _startbeat, float _tick = 999f, float holdTimeInCrotchets = 1f, int _cyclefreq = 1, bool offOtherBeats = false)
	{
		if (offOtherBeats)
		{
			OffAllBeatLoops();
		}
		beatAlarm[_i] = true;
		new scrBeatLoop().SetupHold(_i, _rowID, _startbeat, _tick, _cyclefreq, holdTimeInCrotchets);
	}

	public void OnBeatOneshotBar(int _i, int _rowID, float _startbeat, float _tick = 999f, int _cyclefreq = 1, bool offotherbeats = false)
	{
		if (offotherbeats)
		{
			OffAllBeatLoops();
		}
		beatAlarm[_i] = true;
		new scrBeatLoop().SetupOneshot(_i, _rowID, _startbeat, _tick, _cyclefreq);
	}

	public void OnBeat(int id)
	{
		beatAlarm[id] = true;
	}

	public scrBeatLoopOneshot OnBeatOneshotConstInterval(int i, int rowID, float startBeat, float tick, float customBoomInterval = 0f)
	{
		beatLoopsOneshot[i] = new scrBeatLoopOneshot();
		beatLoopsOneshot[i].Setup(i, rowID, startBeat, tick, customBoomInterval);
		return beatLoopsOneshot[i];
	}

	public void SetOneshotSpillover(int i, bool spillover)
	{
		beatLoopsOneshot[i].spilloverDelay = spillover;
	}

	public void ChangeLoopOneshot(int i, float startBeat, float tick, float customBeatInterval = 0f, bool kill = false)
	{
		beatLoopsOneshot[i].AddLoopChange(startBeat, tick, customBeatInterval, kill);
	}

	public void ChangeLoopOneshotAndBeep(int i, float startBeat, float tick, float customBoomInterval = 0f, bool kill = false, bool sayReady = false, bool sayGo = true)
	{
		float sayGetBeat = ((customBoomInterval == 0f) ? (startBeat - tick * 2f) : (startBeat - customBoomInterval));
		OneshotWarn(sayGetBeat, tick, customBoomInterval, sayReady, sayGo);
		beatLoopsOneshot[i].AddLoopChange(startBeat, tick, customBoomInterval, kill);
	}

	public void KillOneshot(int i, float startBeat)
	{
		ChangeLoopOneshot(i, startBeat, 0f, 0f, kill: true);
	}

	public void ChangeLoopOneshotSimple(int i, float startBeat, float tick, bool sayReady, bool sayGo, float _customBoomInterval = 0f)
	{
		OneshotWarnSimple(startBeat, sayReady, sayGo, tick, _customBoomInterval);
		float beat = (sayReady ? ((_customBoomInterval != 0f) ? (startBeat + _customBoomInterval * 2f) : (startBeat + tick * 4f)) : ((_customBoomInterval != 0f) ? (startBeat + _customBoomInterval) : (startBeat + tick * 2f)));
		beatLoopsOneshot[i].AddLoopChange(beat, tick, _customBoomInterval);
	}

	public void OneshotSayReady(float sayReadyBeat, float tick, float customBoomInterval = 0f, bool sayGo = true)
	{
		float num = ((customBoomInterval == 0f) ? (tick * 2f) : customBoomInterval);
		OneshotWarn(sayReadyBeat + num, tick, customBoomInterval, sayReady: true, sayGo);
	}

	public void OneshotSayGetSet(float _sayGetBeat, float _tick, float customBoomInterval = 0f, bool sayGo = true)
	{
		OneshotWarn(_sayGetBeat, _tick, customBoomInterval, sayReady: false, sayGo);
	}

	public void OneshotWarnSimple(float startBeat, bool startOnReady, bool sayGo, float _tick, float customBoomInterval = 0f)
	{
		float sayGetBeat = (startOnReady ? (startBeat + _tick * 2f) : startBeat);
		OneshotWarn(sayGetBeat, _tick, customBoomInterval, startOnReady, sayGo);
	}

	public void OneshotWarn(float _sayGetBeat, float _tick, float customBoomInterval = 0f, bool sayReady = false, bool sayGo = true)
	{
		float num = ((customBoomInterval != 0f) ? customBoomInterval : (_tick * 2f));
		float num2 = _sayGetBeat - num;
		float num3 = _sayGetBeat;
		float num4 = _sayGetBeat + _tick;
		float num5 = _sayGetBeat + num;
		if (currentLevel.pauseOn7thBeat)
		{
			if (num2 >= 6f)
			{
				num2++;
			}
			if (num3 >= 6f)
			{
				num3++;
			}
			if (num4 >= 6f)
			{
				num4++;
			}
			if (num5 >= 6f)
			{
				num5++;
			}
		}
		float time = num2 * base.conductor.crotchet;
		float num6 = num3 * base.conductor.crotchet;
		float num7 = num4 * base.conductor.crotchet;
		float time2 = num5 * base.conductor.crotchet;
		if (currentLevel.shouldJyiSayOneshot)
		{
			base.conductor.PlayBeat("sndJyi - RDGS_Get", num6, 1f, oneshotVoiceGroup);
			base.conductor.PlayBeat("sndJyi - RDGS_Set", num7, 1f, oneshotVoiceGroup);
		}
		base.conductor.PlayBeat("sndClickMid", num6, 1f, oneshotClicksGroup);
		base.conductor.PlayBeat("sndClickMid", num7, 1f, oneshotClicksGroup);
		if (sayGo)
		{
			if (currentLevel.shouldJyiSayOneshot)
			{
				base.conductor.PlayBeat("sndJyi - RDGS_Go", time2, 1f, oneshotVoiceGroup);
			}
			base.conductor.PlayBeat("sndClickHi", time2, 1f, oneshotClicksGroup);
		}
		if (sayReady)
		{
			if (currentLevel.shouldJyiSayOneshot)
			{
				base.conductor.PlayBeat("sndJyi - RDGS_Ready", time, 1f, oneshotVoiceGroup);
			}
			base.conductor.PlayBeat("sndClickLow", time, 1f, oneshotClicksGroup);
		}
		float beepPeriod = num7 - num6;
		scrExecuteOnCertainBeat.Add(() =>
		{
			currentLevel.BeepGet(beepPeriod);
		}, num3);
		scrExecuteOnCertainBeat.Add(() =>
		{
			currentLevel.BeepSet(beepPeriod);
		}, num4);
		scrExecuteOnCertainBeat.Add(() =>
		{
			currentLevel.BeepGo(beepPeriod);
		}, num5);
	}

	public scrBeatLoopOneshot OnBeatOneshotAndBeep(int _i, int _rowID, float _startbeat, float _tick, float _customBoomInterval = 0f, bool sayReady = false, bool sayGo = true)
	{
		if (_customBoomInterval == 0f)
		{
			OneshotWarn(_startbeat - _tick * 2f, _tick, _customBoomInterval, sayReady, sayGo);
		}
		else
		{
			OneshotWarn(_startbeat - _customBoomInterval, _tick, _customBoomInterval, sayReady, sayGo);
		}
		return OnBeatOneshotConstInterval(_i, _rowID, _startbeat, _tick, _customBoomInterval);
	}

	public void OffBeat(int i)
	{
		beatAlarm[i] = false;
	}

	public void OffAllBeatLoops()
	{
		for (int i = 0; i < 200; i++)
		{
			beatAlarm[i] = false;
		}
	}

	public void StopBeats(bool killHalfwayPlayingSounds = true, int _bar = -1)
	{
		Row[] array = rows;
		foreach (Row row in array)
		{
			if (!(row.ent == null))
			{
				scrBeatbox[] beatboxes = row.ent.classicRowController.beatboxes;
				for (int j = 0; j < beatboxes.Length; j++)
				{
					beatboxes[j].Flatten();
				}
				row.ent.oneshotRowController.FlattenWaves();
				row.ent.subdivisionSpotlightController.Kill();
			}
		}
		for (int num = beats.Count - 1; num >= 0; num--)
		{
			beats[num].DestroyBeat(killSounds: true, killHalfwayPlayingSounds, cutHeldSounds: true, _bar);
		}
		List<AudioSource> list = base.conductor.audiosourcesFromPlayBeat.Except(base.conductor.audiosourcesFromPlayBeatPrevBar).ToList();
		foreach (AudioSource item in (_bar == -1) ? base.conductor.audiosourcesFromPlayBeat : list)
		{
			if (item != null && (killHalfwayPlayingSounds || !item.IsActuallyPlayingNotJustScheduled()))
			{
				item.Stop();
			}
		}
	}

	public void StopBeatsFromRow(int rowID, bool killHalfwayPlayingSounds = true)
	{
		Row row = rows[rowID];
		if (row.rowType == RowType.Classic || row.rowType == RowType.Hold)
		{
			scrBeatbox[] beatboxes = row.ent.classicRowController.beatboxes;
			for (int i = 0; i < beatboxes.Length; i++)
			{
				beatboxes[i].Flatten();
			}
		}
		for (int num = beats.Count - 1; num >= 0; num--)
		{
			Beat beat = beats[num];
			if (beat.rowID == rowID)
			{
				beat.DestroyBeat(killSounds: true, killHalfwayPlayingSounds);
			}
		}
	}

	public void RefreshBeatSkipAesthetics()
	{
		Row[] array = rows;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].RefreshBeatSkipAesthetic();
		}
	}

	public void RefreshSyncopationAesthetics()
	{
		Row[] array = rows;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].RefreshSyncopationAesthetic();
		}
	}

	public Row MakeRow(RowType rowType, int rowID, float tick, Character character = Character.Samurai, RDPlayer singleplayer = RDPlayer.P1, RDPlayer multiplayer = RDPlayer.P1, int shadowOrHostRow = -1, bool isShadow = false, int room = 0, bool animate = true, RowEntity rowEntity = null, string customCharacter = null, bool present = true, bool hideAtStart = false)
	{
		Row row = rows[rowID];
		row.isShadow = isShadow;
		row.isShadowRealtime = isShadow;
		row.shadowOrHostID = shadowOrHostRow;
		row.shadowOrHostIDRealtime = shadowOrHostRow;
		row.singleplayer = singleplayer;
		row.singleplayerToChangeInto = singleplayer;
		row.multiplayer = multiplayer;
		row.multiplayerToChangeInto = multiplayer;
		row.room = room;
		if (rowEntity == null)
		{
			row.ent = UnityEngine.Object.Instantiate(RowEntity).GetComponent<RowEntity>();
			row.ent.transform.SetParent(rooms[room].rowContainer, worldPositionStays: false);
			row.ent.transform.localPosition = Vector3.right * 35f;
			row.ent.Setup(rowID, character, animate, customPosition: false, customCharacter, present, hideAtStart);
		}
		else
		{
			row.ent = rowEntity;
		}
		row.rowType = rowType;
		row.tick = tick;
		row.ent.character.charName = character.ToString();
		row.mistakeWeight = 1f;
		if (GC.twoPlayerMode && shadowOrHostRow != -1 && shadowRowStyle == ShadowRowStyle.HostToShadowIf2P)
		{
			MakeRow(rowType, shadowOrHostRow, tick, character, singleplayer, RDPlayer.P2, -1, isShadow: true, room, animate, null, customCharacter, present);
		}
		return row;
	}

	public void DeleteRow(int rowID, float beat = 0f, bool deleteShadowRow = true, bool repositionRows = false)
	{
		scrExecuteOnCertainBeat.Add(() =>
		{
			DeleteRowInternal(rowID, deleteShadowRow, repositionRows);
		}, beat);
	}

	private void DeleteRowInternal(int rowID, bool deleteShadowRow, bool repositionRows)
	{
		Row row = rows[rowID];
		row.ent.Hide();
		if (deleteShadowRow)
		{
			int shadowOrHostID = row.shadowOrHostID;
			if (shadowOrHostID > 0 && GC.twoPlayerMode)
			{
				printe("shadowID: " + shadowOrHostID);
				if (rows[shadowOrHostID].ent != null)
				{
					rows[shadowOrHostID].ent.Hide();
				}
			}
		}
		if (repositionRows)
		{
			RepositionRowsAndStrips();
		}
	}

	public void RepositionRowsAndStrips(bool instant = false)
	{
		if (repositionRows)
		{
			RDRoom[] array = rooms;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].RepositionRowsAndStrips(instant);
			}
		}
	}

	public scrHandController GetHandControllerFromInt(int handNum)
	{
		return handNum switch
		{
			0 => handController0, 
			1 => handController1, 
			2 => handController2, 
			3 => handController3, 
			_ => handControllerOnTop, 
		};
	}

	private HandUpdateType GetHandUpdateTypeForPlayer(bool player1, bool isShort = false)
	{
		bool flag = false;
		if ((!player1) ? ((byte)RDInput.p1[0].schemeIndex != 0) : (!GC.twoPlayerMode || RDInput.p1[0].schemeIndex == 0))
		{
			if (!isShort)
			{
				return HandUpdateType.RightHand;
			}
			return HandUpdateType.RightHandShort;
		}
		if (!isShort)
		{
			return HandUpdateType.LeftHand;
		}
		return HandUpdateType.LeftHandShort;
	}

	public HandUpdateType GetP1HandUpdateType(bool isShort = false)
	{
		return GetHandUpdateTypeForPlayer(player1: true, isShort);
	}

	public HandUpdateType GetP2HandUpdateType(bool isShort = false)
	{
		return GetHandUpdateTypeForPlayer(player1: false, isShort);
	}

	public void ChangeRowPlayers(bool singleplayer, RDPlayer[] arrRowPlayers, float delayInCrotchets, int indexOffset = 1, bool adjustFlashingToBeat = false, bool warningFlash = true, Character[] arrCpuMarkers = null)
	{
		if (singleplayer != GC.twoPlayerMode)
		{
			ImplementRowPropPlayerOnBarStartChange(singleplayer, arrRowPlayers, indexOffset, arrCpuMarkers);
			scrExecuteOnCertainBeat.Add(() =>
			{
				ChangeRowPlayers(singleplayer, arrRowPlayers, warningFlash, indexOffset, adjustFlashingToBeat, delayInCrotchets, calledFromTag: false, arrCpuMarkers);
			}, delayInCrotchets);
		}
	}

	public void ChangeRowPlayersImmediately(bool singleplayer, RDPlayer[] arrRowPlayers, float delayInCrotchets, int indexOffset = 1, bool adjustFlashingToBeat = false, bool warningFlash = true, Character[] arrCpuMarkers = null)
	{
		if (singleplayer != GC.twoPlayerMode)
		{
			ImplementRowPropPlayerOnBarStartChange(singleplayer, arrRowPlayers, indexOffset, arrCpuMarkers);
			ChangeRowPlayers(singleplayer, arrRowPlayers, warningFlash, indexOffset, adjustFlashingToBeat, delayInCrotchets, calledFromTag: true, arrCpuMarkers);
		}
	}

	public void ChangeRowPlayers(bool singleplayer, RDPlayer[] arrRowPlayers, bool warningFlash = true, int indexOffset = 1, bool adjustFlashingToBeat = false, float startingBeat = 0f, bool calledFromTag = false, Character[] arrCpuMarkers = null)
	{
		if (singleplayer == GC.twoPlayerMode)
		{
			return;
		}
		if (arrCpuMarkers == null)
		{
			arrCpuMarkers = new Character[arrRowPlayers.Length];
			for (int i = 0; i < arrCpuMarkers.Length; i++)
			{
				arrCpuMarkers[i] = Character.None;
			}
		}
		for (int j = 0; j < arrRowPlayers.Length; j++)
		{
			RDPlayer rDPlayer = arrRowPlayers[j];
			Character character = arrCpuMarkers[j];
			Row row = rows[j + indexOffset];
			bool num = rDPlayer != RDPlayer.NoChange && row.GetCurrentPlayer() != rDPlayer;
			bool flag = rDPlayer == RDPlayer.CPU && character != Character.None && row.cpuCharacter != character;
			if ((num | flag) && warningFlash)
			{
				rows[j + indexOffset].isFlashing = true;
			}
		}
		ImplementRowPropPlayerOnBarStartChange(singleplayer, arrRowPlayers, indexOffset, arrCpuMarkers);
		float num2 = (adjustFlashingToBeat ? base.conductor.DurationBeatsToTime(startingBeat, 1f) : RDCalibration.latency);
		if (calledFromTag)
		{
			num2 = base.conductor.crotchet;
		}
		num2 /= RDTime.speed;
		if (base.conductor.isScrubbing)
		{
			PostChangeRowPlayers(singleplayer, arrRowPlayers, arrCpuMarkers, indexOffset);
			return;
		}
		Timer.Add(() =>
		{
			PostChangeRowPlayers(singleplayer, arrRowPlayers, arrCpuMarkers, indexOffset);
		}, num2);
	}

	public void PostChangeRowPlayers(bool singleplayer, RDPlayer[] arrRowPlayers, Character[] arrCpuMarkers, int indexOffset = 1)
	{
		ImplementRowPropPlayerChange(singleplayer, arrRowPlayers, arrCpuMarkers, indexOffset);
		Row[] array = rows;
		foreach (Row row in array)
		{
			row.isFlashing = false;
			if (row.ent != null)
			{
				row.ent.TintIfCPU(isEntrance: false);
			}
		}
		OnChangedPlayerMode();
	}

	public void OnChangedPlayerMode()
	{
		RepositionRowsAndStrips();
		Row[] array = rows;
		foreach (Row row in array)
		{
			if (row.ent != null)
			{
				row.ent.UpdatePlayerSprites();
			}
		}
	}

	private void ImplementRowPropPlayerOnBarStartChange(bool singleplayer, RDPlayer[] arrRows, int indexOffset, Character[] arrCpuMarkers = null)
	{
		if (arrCpuMarkers == null)
		{
			arrCpuMarkers = new Character[arrRows.Length];
			for (int i = 0; i < arrCpuMarkers.Length; i++)
			{
				arrCpuMarkers[i] = Character.None;
			}
		}
		for (int j = 0; j < arrRows.Length; j++)
		{
			if (arrRows[j] != RDPlayer.NoChange)
			{
				if (singleplayer)
				{
					rows[j + indexOffset].singleplayerToChangeInto = arrRows[j];
				}
				else
				{
					rows[j + indexOffset].multiplayerToChangeInto = arrRows[j];
				}
			}
			if (arrRows[j] == RDPlayer.CPU)
			{
				rows[j + indexOffset].cpuCharacterToChangeInto = arrCpuMarkers[j];
			}
		}
	}

	private void ImplementRowPropPlayerChange(bool singleplayer, RDPlayer[] arrRowPlayers, Character[] arrCpuMarkers, int indexOffset = 1)
	{
		for (int i = 0; i < arrRowPlayers.Length; i++)
		{
			if (arrRowPlayers[i] != RDPlayer.NoChange)
			{
				Row row = rows[i + indexOffset];
				if (singleplayer)
				{
					row.singleplayer = arrRowPlayers[i];
				}
				else
				{
					row.multiplayer = arrRowPlayers[i];
				}
			}
			if (arrRowPlayers[i] == RDPlayer.CPU && arrCpuMarkers[i] != Character.None)
			{
				rows[i + indexOffset].cpuCharacter = arrCpuMarkers[i];
			}
		}
	}

	public void RefreshRowPlayersScrub()
	{
		Row[] array = rows;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].RefreshRowPlayersScrub();
		}
	}

	public void SetPlayerHitSounds(RowType rowType, string p1Sound, string p2Sound, string cpuSound = null, float p1Volume = 1f, float p2Volume = 1f, float cpuVolume = 1f, float p1Pitch = 1f, float p2Pitch = 1f, float cpuPitch = 1f, float p1Pan = 0f, float p2Pan = 0f, float cpuPan = 0f)
	{
		GameSoundType type = ((rowType != RowType.Classic) ? GameSoundType.ClapSoundP1Oneshot : GameSoundType.ClapSoundP1Classic);
		int type2 = ((rowType == RowType.Classic) ? 1 : 3);
		GameSoundType type3 = ((rowType == RowType.Classic) ? GameSoundType.ClapSoundCPUClassic : GameSoundType.ClapSoundCPUOneshot);
		RDGameSounds.Set(type, p1Sound, p1Volume, p1Pitch, p1Pan);
		RDGameSounds.Set((GameSoundType)type2, p2Sound, p2Volume, p2Pitch, p2Pan);
		if (cpuSound != null)
		{
			RDGameSounds.Set(type3, cpuSound, cpuVolume, cpuPitch, cpuPan);
		}
	}

	public string GetPlayerHandPopSound(bool isPlayer1)
	{
		DefibMode defibMode = (isPlayer1 ? p1DefibMode : p2DefibMode);
		string result = (isPlayer1 ? "sndMenuPop" : "sndMenuPop2");
		switch (defibMode)
		{
		case DefibMode.Unmissable:
		case DefibMode.VeryEasy:
		case DefibMode.Easy:
			result = (isPlayer1 ? "sndMenuPopLarge" : "sndMenuPopLarge2");
			break;
		case DefibMode.Normal:
			result = ((!(Persistence.GetGameCompletion().percentCompletion >= 100f)) ? (isPlayer1 ? "sndMenuPop" : "sndMenuPop2") : (isPlayer1 ? "sndMenuPopGold" : "sndMenuPopGold2"));
			break;
		case DefibMode.Hard:
			result = (isPlayer1 ? "sndMenuPopSpring" : "sndMenuPopSpring2");
			break;
		}
		return result;
	}

	public void LoadPlayerHandPopSounds()
	{
		string playerHandPopSound = GetPlayerHandPopSound(isPlayer1: true);
		string playerHandPopSound2 = GetPlayerHandPopSound(isPlayer1: false);
		RDGameSounds.Set(GameSoundType.Hand1PopSound, playerHandPopSound);
		RDGameSounds.Set(GameSoundType.Hand2PopSound, playerHandPopSound2);
		if (GC.twoPlayerMode)
		{
			RDGameSounds.SetPan(GameSoundType.Hand1PopSound, GC.PanP1);
			RDGameSounds.SetPan(GameSoundType.Hand2PopSound, GC.PanP2);
		}
	}

	public void FailLevel(RowEntity ent)
	{
		if (failedLevel)
		{
			return;
		}
		statusText.SetStatusText(RDString.Get("status.gameOver"), null, 4f, narrate: true);
		base.conductor.executes.Clear();
		scrVolumeTracker.ClearAll();
		if (!currentLevel.dontKillWindowDanceOnGameOverUntilExit)
		{
			windowChoreographer?.Cancel();
		}
		RDBase.Vfx.oneshotSpotlights.gameObject.SetActive(value: false);
		RDRoom[] array = rooms;
		foreach (RDRoom rDRoom in array)
		{
			if (rDRoom.stutter != null)
			{
				rDRoom.stutter.EndCurrentStutter();
				rDRoom.stutter.enabled = false;
			}
			if (rDRoom.renderTexPaster != null)
			{
				rDRoom.renderTexPaster.Reset();
				rDRoom.renderTexPaster.enabled = false;
			}
			rDRoom.wavyRows = false;
		}
		visualSpeed = 0f;
		StopBeats();
		base.conductor.StopSong();
		OffAllBeatLoops();
		SetVisualSpeed(1f);
		SetSlowdown(1f);
		mistakesManager.SaveMistakesData();
		AddLevelDeath();
		if (!currentLevel.FailLevel(ent))
		{
			LevelFailSequence(ent);
		}
		else
		{
			RDBase.Vfx.Flash();
		}
		failedLevel = true;
	}

	public void FailLevelLite()
	{
		base.conductor.executes.Clear();
		scrVolumeTracker.ClearAll();
		base.game.StopBeats();
		base.conductor.StopSong();
		base.game.OffAllBeatLoops();
		base.game.SetVisualSpeed(1f);
		currentLevel.StopDialogue();
	}

	public void AddLevelDeath()
	{
		lastBarDiedAt = base.conductor.barNumber;
		Type type = currentLevel.GetType();
		if (!RDStartup.levelDeathsAtBars.ContainsKey(type))
		{
			RDStartup.levelDeathsAtBars.Add(type, new Dictionary<int, int>());
		}
		if (!RDStartup.levelDeathsAtBars[type].ContainsKey(lastBarDiedAt))
		{
			RDStartup.levelDeathsAtBars[type].Add(lastBarDiedAt, 1);
		}
		else
		{
			RDStartup.levelDeathsAtBars[type][lastBarDiedAt]++;
		}
		IncrementLevelTries();
	}

	public static void ClearLevelPersistence()
	{
		levelPersistence = new LevelPersistence();
	}

	private bool CheckIfIsAHost(int rowID)
	{
		if (shadowRowStyle == ShadowRowStyle.HostToShadowIf2P)
		{
			Debug.LogError("shouldnt be calling this!");
		}
		bool result = false;
		for (int i = 0; i < rows.Length; i++)
		{
			if (i != rowID && rows[i].shadowOrHostIDRealtime == rowID)
			{
				result = true;
			}
		}
		return result;
	}

	public void OnMistakeOrHeal(float offset = 0f, float weight = 1f, Row prop = null, bool removeMistakes = false, RDPlayer currentPlayer = RDPlayer.NoChange)
	{
		if (prop != null)
		{
			if (!currentLevel.noShadowRowWeighting)
			{
				if (shadowRowStyle == ShadowRowStyle.ShadowToHost)
				{
					if ((prop.isShadowRealtime && prop.normalizeMistakeWeightWhenShadow) || CheckIfIsAHost(prop.id))
					{
						int hostID = (prop.isShadowRealtime ? prop.shadowOrHostIDRealtime : prop.id);
						RDPlayer hostPlayer = rows[hostID].GetCurrentPlayer();
						int num = 1 + rows.Count((Row r) => r.shadowOrHostIDRealtime == hostID && r.normalizeMistakeWeightWhenShadow && r.GetCurrentPlayer() == hostPlayer);
						weight *= 1f / (float)num;
					}
				}
				else if (shadowRowStyle == ShadowRowStyle.HostToShadowIf2P && GC.twoPlayerMode && (prop.shadowOrHostIDRealtime != -1 || prop.isShadowRealtime))
				{
					weight *= 0.5f;
				}
			}
			if (currentPlayer == RDPlayer.NoChange)
			{
				currentPlayer = prop.GetCurrentPlayer();
			}
			else
			{
				Debug.LogWarning("Satellite messed something up! Were you using custom methods?");
			}
		}
		else if (currentPlayer == RDPlayer.NoChange)
		{
			currentPlayer = RDPlayer.BOTH;
			RDBaseDllDummy.printem("Satellite messed something up! Check which function resulted in this message!");
			Debug.LogWarning("Satellite messed something up! Were you using custom methods?");
		}
		if (weight > 0f)
		{
			currentLevel.OnMistake();
			mistakesManager.AddMistake(currentPlayer, offset, weight);
		}
		if ((weight < 0f) & removeMistakes)
		{
			mistakesManager.RemoveMistake(currentPlayer, weight);
		}
		base.editor?.ipm.UpdateBlankPanel();
	}

	public void UpdatePlayerHealthBars()
	{
		if (!currentLevel.shouldMakeHealthBar)
		{
			return;
		}
		hpController.RefreshBars();
		if (currentLevel.Is1HP() && !hasEnteredLowHealth && !GC.twoPlayerMode)
		{
			currentLevel.OnEnterLowHealth();
			hasEnteredLowHealth = true;
		}
		if (!currentLevel.Is1HP())
		{
			if (hasEnteredLowHealth)
			{
				currentLevel.OnExitLowHealth();
			}
			hasEnteredLowHealth = false;
		}
	}

	private new void OnGUI()
	{
		base.OnGUI();
		if (!RDBase.debugSettings.Debug || base.editorMode)
		{
			return;
		}
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			debugKey = 1;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			debugKey = 2;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			debugKey = 3;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha4))
		{
			debugKey = 4;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha5))
		{
			debugKey = 5;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha6))
		{
			debugKey = 6;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha7))
		{
			debugKey = 7;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha8))
		{
			debugKey = 8;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha9))
		{
			debugKey = 9;
		}
		else if (Input.GetKeyDown(KeyCode.Alpha0))
		{
			debugKey = 0;
		}
		bool flag = false;
		string text = "\n";
		string text2 = "";
		if (debugKey == 1)
		{
			text2 = "Mistakes: " + mistakesManager.mistakes + text + "Mistake average: " + mistakesManager.GetAverage() + text + "Bar: " + base.conductor.barNumberUpdateOnBar + text + "Rank: " + currentLevel.GetRankFromMistakes().ToString();
		}
		if (debugKey == 2)
		{
			text2 = "Beats:";
			foreach (Beat beat in beats)
			{
				text2 += $"\n{beat.inputTime}";
			}
		}
		if (debugKey == 3)
		{
			string text3 = "";
			text3 = text3 + "currentResolution Width: " + Screen.width + " height: " + Screen.height + " @ " + Screen.currentResolution.refreshRateRatio.value + "hz\n";
			Resolution[] resolutions = Screen.resolutions;
			for (int i = 0; i < resolutions.Length; i++)
			{
				Resolution resolution = resolutions[i];
				text3 = string.Concat(text3, resolution.width + " x " + resolution.height + " @ " + resolution.refreshRateRatio.value + "hz\n");
			}
			text3 = text3 + "fullscreen: " + Screen.fullScreen + "\n";
			text3 = text3 + "fullscreenMode: " + Screen.fullScreenMode.ToString() + "\n";
			text2 = text3;
		}
		GUI.Label(new Rect(20f, 40f, 200f, 600f), text2);
		if (flag)
		{
			printe(Time.frameCount + " " + text2);
		}
	}

	public bool BeatIsBeingHeldByPlayer(RDPlayer player)
	{
		Row[] array = rows;
		foreach (Row row in array)
		{
			if (row.ent != null && (bool)row.ent.playerBox.currentHoldBeat && row.GetCurrentPlayer() == player)
			{
				return true;
			}
		}
		return false;
	}

	public void LevelFailSequence(RowEntity ent)
	{
		Persistence.SetLevelRank(levelIdentifier, -1);
		failedLevel = true;
		RDBase.Vfx.ShakeCam(30, 20);
		ent.character.animator.deltaTimeStyle = tk2dSpriteAnimator.DeltaTimeStyle.AlwaysOneAlways;
		DOVirtual.DelayedCall(1f, () =>
		{
			RDBase.Vfx.SpotlightFocusFromCenter(ent.character.transform.position.x, 0.5f, playSwitchSound: true, RDUtils.GetMixerGroup("GameplaySoundsParent"));
		});
		DOVirtual.DelayedCall(2f, () =>
		{
			ent.heart.DoFinalCrack();
			RDUtils.Rotate2D(ent.heart.transform, 0f);
		});
		DOVirtual.DelayedCall(3f, () =>
		{
			rankscreen.ShowLevelFailed();
		});
		DOVirtual.DelayedCall(5f, () =>
		{
			rankscreen.ShowDontGiveUp();
		});
		scrExecuteOnPeriodicBeat.Clear();
	}

	public void WinLevel()
	{
		LevelType levelType = currentLevel.levelType;
		if (levelType == LevelType.Tutorial || levelType == LevelType.Cutscene || levelType == LevelType.Intro)
		{
			SkipLevel();
			return;
		}
		if (currentLevel is Level_Montage)
		{
			base.game.SkipLevelWithCallback(() =>
			{
				currentLevel.GoToLevelInstantly("Montage2");
			});
			return;
		}
		if (paused)
		{
			TogglePauseGame();
		}
		if (currentLevel.levelType == LevelType.Boss)
		{
			if (!rankscreen.rankscreen.activeSelf)
			{
				if (currentLevel is Level_Lesmis level_Lesmis)
				{
					level_Lesmis.SkipEnding();
				}
				currentLevel.StopEverything();
				currentLevel.SetNextBarExtraImmediately(999);
				currentLevel.skippableRankScreen = true;
				rankscreen.AdvanceGameover();
				rankscreen.AdvanceGameover();
				rankscreen.AdvanceGameover();
			}
		}
		else
		{
			rankscreen.AdvanceGameover();
		}
	}

	public void ResetAllData()
	{
		Persistence.DeleteSavedData();
	}

	public void ToggleDebugMode()
	{
		RDBase.debugSettings.Debug = !RDBase.debugSettings.Debug;
		RDBase.Vfx.Flash();
		MonoBehaviour.print("Debug mode " + RDBase.debugSettings.Debug);
		if (!RDBase.debugSettings.Debug)
		{
			debugText.text = "";
		}
	}

	public void GoToLevelButtonClick()
	{
		scnBase.GoToAlphaLevelSelect();
	}

	public void HealRowsToMinimum(float lives)
	{
		Row[] array = rows;
		foreach (Row row in array)
		{
			if (!(row.ent == null))
			{
				RowEntity ent = row.ent;
				if (!ent.heartHasCracked && ent.rowMisses > currentLevel.missesToCrackHeart - lives)
				{
					printe("heal");
					ent.rowMisses = currentLevel.missesToCrackHeart - lives;
					ent.Heal(0f);
					OnMistakeOrHeal(0f, 0f, row);
					Vector3 position = new Vector3(RDBase.Vfx.RDWidth * 0.5f, row.ent.transform.position.y - 10f);
					RDBase.Vfx.BeatboxPulsed(position, -100, RDString.Get("status.healed"), 1f);
				}
			}
		}
	}

	public void ReviveDeadRows()
	{
		if (!GC.twoPlayerMode)
		{
			return;
		}
		bool flag = false;
		Row[] array = rows;
		foreach (Row row in array)
		{
			if (row.dead)
			{
				printe("revived row: " + row.id);
				flag = true;
				row.ent.Show(ShowAnimationType.FullAnimation, setup: false);
				row.dead = false;
				row.ent.rowMisses = currentLevel.missesToCrackHeart / 2f;
				hpController.RefreshBars();
				Vector3 position = new Vector3(RDBase.Vfx.RDWidth * 0.5f, row.ent.transform.position.y - 10f);
				RDBase.Vfx.BeatboxPulsed(position, -100, RDString.Get("status.revived"), 1f);
			}
		}
		if (flag)
		{
			RepositionRowsAndStrips();
		}
	}

	private IEnumerator ReviveColorAnimation(RowEntity ent, float duration)
	{
		Color overlay0 = ent.shaderData.overlayColor;
		Color overlay1 = Color.white;
		Color overlay2 = RDBase.Vfx.globalShaderData.overlayColor;
		Color glow0 = ent.shaderData.borderColor;
		Color glow1 = Color.white;
		Color glow2 = RDBase.Vfx.globalShaderData.borderColor;
		for (float time = 0f; time < 0.4f; time += Time.unscaledDeltaTime)
		{
			float t = time / 0.4f;
			ent.SetOverlayColor(Color.Lerp(overlay0, overlay1, t));
			ent.SetBorderColor(Color.Lerp(glow0, glow1, t));
			yield return null;
		}
		for (float time = 0f; time < 0.05f; time += Time.unscaledDeltaTime)
		{
			yield return null;
		}
		for (float time = 0f; time < 0.3f; time += Time.unscaledDeltaTime)
		{
			float t2 = time / 0.3f;
			ent.SetOverlayColor(Color.Lerp(overlay1, overlay2, t2));
			ent.SetBorderColor(Color.Lerp(glow1, glow2, t2));
			yield return null;
		}
		ent.shaderDataSource = null;
	}

	public void CloseBlades(float duration)
	{
		if (currentLevel.startImmediately)
		{
			OpenBlades(instant: true);
		}
		disappearBlades?.Kill();
		scrConductor.PlayImmediatelyIgnoringTimeScale("sndTransitionShort", 1f, RDUtils.GetMixerGroup("LevelInterface"));
		bladesContainer.gameObject.SetActive(value: true);
		base.game.pauseMenu.pauseMenuCamera.enabled = true;
		Transform[] array = bladeSprites;
		foreach (Transform obj in array)
		{
			obj.gameObject.layer = ((windowChoreographer is VirtualWindowChoreographer) ? 5 : 16);
			obj.DOKill();
			obj.DOLocalRotate(new Vector3(0f, 0f, 0.53f), duration).SetEase(base.gc.bladeRotationCloseEase).SetUpdate(isIndependentUpdate: true);
		}
		DOVirtual.DelayedCall(duration, () =>
		{
			if (handControllerOnTop.startingAnimation)
			{
				handControllerOnTop.OnTopHandForceUpdate();
			}
		});
		currentLevel.FadeOutMusic(duration);
	}

	public void OpenBlades(bool instant = false)
	{
		bladesContainer.gameObject.SetActive(value: true);
		if (!instant)
		{
			handControllerOnTop.SetHandsInFront(inFront: true, beforeGameStart: true);
			DOVirtual.DelayedCall(base.gc.bladeRotationDuration, () =>
			{
				handControllerOnTop.SetHandsInFront(inFront: true);
				if (handControllerOnTop.startingAnimation)
				{
					handControllerOnTop.OnTopHandForceUpdate();
				}
			});
		}
		Transform[] array = bladeSprites;
		foreach (Transform transform in array)
		{
			if (instant)
			{
				transform.Rotate(Vector3.forward, base.gc.bladeSpritesFinalAngle);
				continue;
			}
			Vector3 endValue = new Vector3(0f, 0f, base.gc.bladeSpritesFinalAngle);
			transform.DOKill();
			transform.DORotate(endValue, base.gc.bladeRotationDuration, RotateMode.LocalAxisAdd).SetEase(base.gc.bladeRotationEase).SetUpdate(isIndependentUpdate: true);
		}
	}

	public override void OnBeat()
	{
		base.OnBeat();
		if (base.editorMode)
		{
			base.editor.OnBeat();
		}
	}

	private void SingleLevelUpdate()
	{
	}

	public static float GetHitMargin(RDPlayer player)
	{
		float num = (DefibMode)(player switch
		{
			RDPlayer.P1 => (int)p1DefibMode, 
			RDPlayer.P2 => (int)p2DefibMode, 
			_ => 3, 
		}) switch
		{
			DefibMode.Unmissable => 0.4f, 
			DefibMode.VeryEasy => 0.2f, 
			DefibMode.Easy => 0.12f, 
			DefibMode.Hard => 0.04f, 
			_ => 0.08f, 
		};
		scnGame obj = instance;
		if ((object)obj != null && obj.currentLevel?.hitMarginMultiplier > 0f)
		{
			num *= instance.currentLevel.hitMarginMultiplier;
		}
		return num;
	}

	public static float GetReleaseMargin(RDPlayer player)
	{
		float num;
		switch ((player == RDPlayer.P2) ? p2DefibMode : p1DefibMode)
		{
		case DefibMode.Unmissable:
		case DefibMode.VeryEasy:
			num = 0.4f;
			break;
		case DefibMode.Easy:
			num = 0.12f;
			break;
		default:
			num = 0.08f;
			break;
		}
		float num2 = num;
		scnGame obj = instance;
		if ((object)obj != null && obj.currentLevel?.hitMarginMultiplier > 0f)
		{
			num2 *= instance.currentLevel.hitMarginMultiplier;
		}
		return num2;
	}

	public void AddHitOffset(int rowID, OffsetType offsetType)
	{
		HitInfo item = new HitInfo
		{
			bar = base.conductor.barNumber,
			offsetType = offsetType
		};
		rowsHitOffsets[rowID].Add(item);
		allHitOffsets.Add(item);
	}

	public void MoveBackTimedActions(double moveBackBy)
	{
		if (currentLevel.data != null)
		{
			List<TimedAction> list = new List<TimedAction>();
			for (int i = 0; i < timedActions.Count; i++)
			{
				timedActions[i].timeOfEnd -= (float)moveBackBy;
			}
			foreach (TimedAction timedAction in timedActions)
			{
				if (Time.time > timedAction.timeOfEnd)
				{
					list.Add(timedAction);
					continue;
				}
				bool flag = true;
				foreach (Conditional conditional in conditionals)
				{
					foreach (ConditionalID conditional2 in timedAction.conditionals)
					{
						if (conditional.Matches(conditional2) && !conditional.Check(timedAction.levelEvent, conditional2.negated))
						{
							flag = false;
							break;
						}
					}
					if (!flag)
					{
						break;
					}
				}
				if (flag)
				{
					timedAction.levelEvent.timesRun++;
					timedAction.action();
				}
			}
			foreach (TimedAction item in list)
			{
				timedActions.Remove(item);
			}
		}
		RDInk.mainDialogue.FastForward(moveBackBy);
	}

	public void MoveWorldSpaceToRoom(int room = -1)
	{
		bool flag = room == -1;
		canvas.gameObject.layer = (flag ? 11 : 0);
		hpController.rectTransform.AnchorPosX(flag ? 0f : 3000f);
		canvas.transform.SetParent(flag ? RDBase.Vfx.topObjects : null);
		canvas.transform.LocalMoveZ(8f);
	}

	public void MoveBackBeats(double time)
	{
		foreach (Beat beat in beats)
		{
			beat.MoveBackBy(time);
		}
		foreach (AudioSource src in base.conductor.audiosourcesFromPlayBeat)
		{
			if (src == null)
			{
				continue;
			}
			if (src.isPlaying && src.time > 0f)
			{
				src.Stop();
			}
			double newStartTime = base.conductor.audioStartTimesFromPlayBeat[src] - time;
			base.conductor.audioStartTimesFromPlayBeat[src] = newStartTime;
			if (base.conductor.audioPos > newStartTime - 0.10000000149011612)
			{
				src.Stop();
			}
			else
			{
				Timer.Add(() =>
				{
					src.SetScheduledStartTime(newStartTime);
				}, 0.1f);
			}
			if (!base.conductor.audioEndTimesFromPlayBeat.ContainsKey(src))
			{
				continue;
			}
			double newEndTime = base.conductor.audioEndTimesFromPlayBeat[src] - time + base.conductor.audioPos;
			if (base.conductor.audioPos > newEndTime - 0.10000000149011612)
			{
				src.Stop();
				continue;
			}
			Timer.Add(() =>
			{
				src.SetScheduledEndTime(newEndTime);
			}, 0.1f);
		}
	}

	public bool GetPassedLevelWithoutCheckpoints()
	{
		int[] checkpointBarsDesc = currentLevel.checkpointBarsDesc;
		if (checkpointBarsDesc != null && checkpointBarsDesc.Length != 0)
		{
			return barToScrubTo == 0;
		}
		return false;
	}

	public void ToggleAutoplay()
	{
		RDBase.debugSettings.Auto = !RDBase.debugSettings.Auto;
		LEDSign.status = (RDBase.debugSettings.Auto ? "autoplay on!" : "autoplay off!");
	}

	public void ShowRequiredController()
	{
	}

	public void HideRequiredController()
	{
	}

	public void FlashBorderFeedback(bool correct, Row source = null)
	{
		FlashBorderFeedback((!correct) ? BorderFeedbackType.Incorrect : BorderFeedbackType.Correct, source);
	}

	public void FlashBorderFeedback(BorderFeedbackType type, Row rowSource = null)
	{
		if (currentLevel.noHitFlashBorder)
		{
			return;
		}
		Color color = type switch
		{
			BorderFeedbackType.DrumHit => "402A00FF".HexToColor(), 
			BorderFeedbackType.Incorrect => "7F2020FF".HexToColor(), 
			BorderFeedbackType.Correct => "1C6D6DFF".HexToColor(), 
			_ => Color.magenta, 
		};
		bool flag = Time.timeScale > 1f;
		float duration = PreventDurationFasterThan((type == BorderFeedbackType.DrumHit) ? 0.125f : 0.5f);
		if (base.game.reducedFlash)
		{
			color = color.WithAlpha(color.a / 2f);
		}
		List<int> list = new List<int>();
		if (rowSource != null && windowChoreographer != null)
		{
			WindowDancer[] dancers = windowChoreographer.dancers;
			foreach (WindowDancer windowDancer in dancers)
			{
				if (windowDancer.roomIndex == -1 || windowDancer.roomIndex == rowSource.room)
				{
					windowDancer.window.FlashBorder(color, duration, flag);
					list.Add(windowDancer.window.index);
				}
			}
		}
		else
		{
			flashBorderColor = color;
			flashBorderTween?.Kill();
			flashBorderTween = DOTween.To(() => flashBorderColor, (Color c) =>
			{
				flashBorderColor = c;
			}, color.WithAlpha(0f), duration).SetUpdate(UpdateType.Normal, flag).Done();
		}
		static float PreventDurationFasterThan(float time)
		{
			if (!(Time.timeScale < 1f))
			{
				return time;
			}
			return time / Time.timeScale;
		}
	}
}
