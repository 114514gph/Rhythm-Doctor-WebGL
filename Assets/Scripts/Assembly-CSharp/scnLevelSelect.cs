using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

public class scnLevelSelect : scnBase, IRequiredControllerPrompt
{
	[Serializable]
	public class EasterEggAnimation
	{
		public string entityId;

		public tk2dSpriteAnimator animator;

		public string clipName;

		public string wardName;

		public float showUpChance;

		public int minTimesBetweenShowUps;

		[NonSerialized]
		public int currentBetweenShowUps;

		public float minTimeToPlayAnimation;

		public float maxTimeToPlayAnimation;
	}

	private enum Direction
	{
		Previous = 0,
		Next = 1,
		Up = 2,
		Down = 3
	}

	private class LevelSelectDestination
	{
		public string targetObjectId;

		public string title;

		public string subtitle;

		public LevelSelectDestination(string targetObjectId, string title, string subtitle)
		{
			this.targetObjectId = targetObjectId;
			this.title = title;
			this.subtitle = subtitle;
		}
	}

	private enum Gift
	{
		Star = 0,
		Scroll = 1
	}

	private List<LevelSelectDestination> selectedVerticalDestinations;

	private const float FlashTransition = 1f;

	private const Level FirstLevel = Level.OrientalTechno;

	private const Level DistantDuet = Level.DistantDuet;

	private const Level OneShiftMore = Level.Lesmis;

	public const string LevelSelectDialogueFile = "diaLevelSelect";

	private const string talkToJanitorId = "talkToJanitor";

	private const string artCanvasId = "artCanvas";

	private const string talkToJanitorName = "TalkToJanitor";

	private const string paigeChairName = "PaigeChair";

	private const string paigeChairActionId = "paigeChair";

	private const string janitorActionId = "Janitor_";

	private const string janitorDefaultActionId = "Janitor_Default";

	private const string vendingMachinePaigeActionId = "vendingMachinePaige";

	private const string vendingMachinePaigeName = "VendingMachinePaige";

	private const string BasementComputer = "BasementComputer";

	private const string SleevePaint = "sleevePaint";

	private const string IanDesktop = "IanDesktop";

	private const string artCanvasName = "ArtCanvas";

	private const string IntroductionNode = "Intro1";

	private const string PassedLevelNodeSuffix = "_Passed";

	private const string NewLevelNodeSuffix = "_New";

	private const string GoToSVTWard = "GoToSVTWard";

	private const string ExitSVTWard = "ExitSVTWard";

	private const string GoToRooftop = "GoToRooftop";

	private const string GoToTrain = "GoToTrain";

	private const string GoToBasement = "GoToBasement";

	private const string GoToArtRoom = "GoToArtRoom";

	private const string GoToMuseDashRoom = "GoToMuseDashRoom";

	private const string ExitMuseDashRoom = "ExitMuseDashRoom";

	private const string GoToAthleteWard = "GoToAthleteWard";

	private const string ExitAthleteWard = "ExitAthleteWard";

	private const string ExitRecordsRoom = "ExitRecordsRoom";

	private const string MainElevator = "MainElevator";

	private const string ExitVoid = "ExitVoid";

	private const string BeansHopper = "2-B1";

	private const string RhythmWeightlifter = "5-B1";

	private const string HelpingHands = "X-0";

	private const string Montage2 = "7-X2";

	private const string TransitionKey = "transition:";

	private const string Bonus1CharacterName = "GongXi";

	private const string Bonus3CharacterName = "GAndTonic";

	private const string CharacterType = "character";

	private const string ObjectType = "object";

	private const string TypeKey = "type";

	private const string Action_Transition = "transition";

	private const string TransitionTexID = "_TransitionTex";

	private const string CutoffID = "_Cutoff";

	private const string highlightColor = "#F652A0";

	private const string idColor = "#CCFF22";

	private const string levelNameColor = "#6AF2F0";

	private const string perfectColor = "#FFD300";

	private const string failedColor = "#CCCCCC";

	private string[] buttonInstructionsDefault = new string[0];

	private string[] buttonInstructionsNightShift = new string[1] { "levelSelect.changeShift" };

	private string[] buttonInstructionsVerticalDestination = new string[1] { "levelSelect.changeDestination" };

	public static bool playerQuitLevel;

	public static bool steamOverlayDialoguePlayed;

	public static bool bitternessWarningPlayed;

	public static bool robotJanitorDialoguePlayed;

	[Header("GameObjects")]
	public RDInk rdInk;

	public GameObject cameraObject;

	public GameObject listenerObject;

	public GameObject verticalMovement;

	public GameObject trainMovement;

	public GameObject scene;

	public EasterEggAnimation[] easterEggArray;

	[Header("UI")]
	public Canvas canvas;

	public Text description;

	public Text patientQuote;

	public Canvas topUIcanvas;

	public Transform levelsContainer;

	public Transform arrows;

	public MovingArrow upArrow;

	public MovingArrow downArrow;

	public HeartMonitor heartMonitor;

	public RectTransform escapeArrowContainer;

	public Image escapeArrowFill;

	[Header("Unbeatable")]
	public Button unbeatableButton;

	public Image unbeatableLogo;

	public Image unbeatableKickstarter;

	[Header("Muse Dash")]
	public Button museDashButton;

	[Header("Bits and Bops")]
	public Button bitsAndBopsButton;

	public Image bitsAndBopsLogo;

	public Image bitsAndBopsKickstarter;

	[Header("VividStasis")]
	public Button vividStasisButton;

	[Header("SparkLine")]
	public Button sparklineButton;

	[Header("Beatblock")]
	public Button beatblockButton;

	[Header("Pause Menu")]
	public PauseMenu pauseMenu;

	public RawImage pauseMenuOverlay;

	[Header("Transition")]
	public SimpleBlit simpleBlit;

	public Texture transitionTexture;

	public RDGameTransition transitionAnimation;

	[Header("Mobile")]
	public GameObject MobileUI;

	public GameObject MobileNightShiftButton;

	public GameObject MobileLeftButton;

	public GameObject MobileRightButton;

	public GameObject MobileUnlockInstructions;

	public Text MobileClockTime;

	[Header("Bottom bar")]
	public Transform bottomBar;

	public Transform StarsCounter;

	public Text StarsCounterText;

	public Transform FreeStar;

	public Transform CounterStarIcon;

	public Transform ScrollsCounter;

	public Text ScrollsCounterText;

	[Header("Wards/Areas")]
	public RDEnvironment[] environments;

	public RDMainWard mainWard;

	public SVTWard svtWard;

	public RDTrain train;

	public RDAthleteWard athleteWard;

	public scrChar sleepingPaige;

	public scrChar vendingMachinePaige;

	public scrChar museDashEntranceRin;

	public scrChar nicoleAct5Blockage;

	public GameObject montage2Cole;

	public SelectableCharacter museDashLevelRin;

	public GameObject[] hospitalLights;

	public Transform[] mainWardForegrounds;

	public Transform svtWardForeground;

	public LevelSelectDreamBubble athleteDreamBubbleBig;

	public LevelSelectDreamBubble athleteDreamBubbleSmall;

	public RDAthleteWard athleteDreamBubbleSmallBackground;

	public BubbleMask athleteDreamBubbleBigParticles;

	public GameObject sleveePaintSprite;

	public RDAbandonedWard abandonedWard;

	public GameObject robotsContainer;

	[Header("Variables")]
	public float cutoffAnimDuration = 2f;

	public float parallaxFactor;

	[Header("Art Canvas")]
	public Image artCanvasFade;

	public GameObject artCanvasObject;

	public GameObject uiArtCanvasContainer;

	public RawImage uiArtCanvasImage;

	public RawImage sceneArtCanvasImage;

	[Header("Act 5 Transition")]
	public CanvasGroup act5TransitionCanvasGroup;

	public Text act5TransitionText;

	[Header("Rhythm Weightlifter")]
	public tk2dSpriteAnimator rwCabinet;

	public GameObject rwCabinetTarp;

	public AudioLowPassFilter rwLowpassFilter;

	public bool controlsAreLocked;

	[NonSerialized]
	public bool enterIsLocked;

	[NonSerialized]
	public bool dialogueIsRunning;

	[NonSerialized]
	public int currentWardIndex = 1;

	public List<SelectableEntity> selectableEntities = new List<SelectableEntity>();

	[NonSerialized]
	public Tween camMoveXTween;

	[NonSerialized]
	public AudioListener listener;

	private bool showingArtCanvas;

	private SelectableEntity artCanvasEntity;

	private int artCanvasEntityIndex;

	private SelectableEntity mainElevatorEntranceEntity;

	private int mainElevatorEntranceEntityIndex;

	private RectTransform pauseMenuOverlayRect;

	private Material blitTransitionMaterial;

	public Difficulty currentDifficulty = Difficulty.Normal;

	private string previousWard;

	private Sequence sequence;

	private Sequence quoteSeq;

	private AudioSource speechSource;

	private bool isPatientQuoteAnimating;

	private bool arrowsShouldBeVisible;

	private float soundTriggerTimer;

	private float soundTriggerCooldown = 0.05f;

	private float keyTime;

	private float keyAnnoyedTime;

	private int selectedEntityIndex = -1;

	private int selectedVerticalIndex;

	private int osmIndex;

	private int lockedLevelPresses;

	private const float escTimeForQuit = 2f;

	private bool firstFrame = true;

	private bool annoyedDialogueIsRunning;

	private bool leftArrowButtonDown;

	private bool rightArrowButtonDown;

	private bool dontMovePaigeYet;

	private int voidItemProgress;

	public List<WalkingSelectableCharacter> walkingCharacters = new List<WalkingSelectableCharacter>();

	private float rinStartX;

	private float rinStartY;

	private float rinDoorX;

	private float rinLevelX;

	private float rinLevelY;

	private bool rinInit;

	private const float rinDist = 14f;

	private bool cameFromBasementComputer;

	private bool halloweenMode;

	private Level justPlayedLevel;

	private float camStartX;

	private float touchStartX;

	private int newSelectedIndex = -1;

	private const float MinDistanceToConsiderAsDragging = 2f;

	private bool validTouchDown;

	private const int MaxTouches = 30;

	private int touchesStored;

	private float[] touchPositions = new float[30];

	private float[] touchTimes = new float[30];

	private const float DoubleTapTime = 0.2f;

	public float timeSinceLastTap;

	private Tween rinMoveTween;

	private bool rinStartingFromExit;

	private Sequence windowManSequence;

	public Tween playSongCall;

	private Tween startFadeIn;

	private bool modeXX = true;

	private bool saidFirstSpeech;

	private bool changingWard;

	private List<SelectableEntity> allWardEntities;

	private string firstSpeech = string.Empty;

	private string lastEntityNarration = string.Empty;

	private string lastLocationName = string.Empty;

	private const string caret = "|";

	private const string invisibleCaret = "<color=#0000>|</color>";

	public new static scnLevelSelect instance => scnBase._instance as scnLevelSelect;

	public static bool paigeIsAwayFromHospital
	{
		get
		{
			if (Level.PaigesReckoning.Passed() && !Persistence.GetPaigeEnding())
			{
				return !Persistence.GetPlayedRooftopCutscene();
			}
			return false;
		}
	}

	public static bool doingRobots
	{
		get
		{
			if (!Persistence.GetPaigeEnding() && Level.PaigesReckoning.Passed())
			{
				return !Level.Montage.Passed();
			}
			return false;
		}
	}

	public SelectableEntity selectedEntity => selectableEntities[selectedEntityIndex];

	private bool isDayOnGame
	{
		get
		{
			DateTime now = DateTime.Now;
			if (now.Hour >= 6)
			{
				return now.Hour < 18;
			}
			return false;
		}
	}

	private new void Awake()
	{
		base.Awake();
		scnBase._instance = this;
		muteWhenGameUnfocused = true;
		Persistence.SaveAll();
		scnBase.currentLevelSelect = "scnLevelSelect";
		if (Persistence.GetFirstTimePlaying())
		{
			Persistence.SetFirstTimePlaying(state: false);
			Persistence.SetLastPlayedLevel(Level.Intro);
		}
		if (Level.OrientalTechno.Locked())
		{
			Persistence.SetLevelRank(Level.OrientalTechno, -1);
		}
		if (Level.Montage2.Passed())
		{
			if (Level.HelpingHands.Locked())
			{
				Persistence.SetLevelRank(Level.HelpingHands, -1);
			}
			if (Level.ArtExercise.Locked())
			{
				Persistence.SetLevelRank(Level.ArtExercise, -1);
			}
		}
		if (RDBase.Vfx == null)
		{
			RDBase.Vfx = scrVfxControl.instance;
		}
		mainWard.dogMode = false;
		scrVfxControl.CheckForWindowDanceChanges();
		base.ink.Load("diaLevelSelect");
		pauseMenu.Initialize();
		pauseMenuOverlayRect = pauseMenuOverlay.GetComponent<RectTransform>();
		if (!(DateTime.Now < new DateTime(2021, 5, 8)))
		{
			unbeatableKickstarter.gameObject.SetActive(value: false);
		}
		if (!(DateTime.Now < new DateTime(2023, 1, 4)))
		{
			bitsAndBopsKickstarter.gameObject.SetActive(value: false);
		}
		else
		{
			bitsAndBopsLogo.gameObject.SetActive(value: false);
		}
		bool num = (int)Persistence.GetLevelRank(Level.HelpingHands) != -2;
		bool passed = Persistence.GetLevelRank(Level.AthleteFinale).passed;
		bool flag = (int)Persistence.GetLevelRank(Level.HaileyDuet) != -2;
		if ((num & passed) && !flag && (LevelSelector.LevelFromString(Persistence.GetLastPlayedLevel()) != Level.AthleteFinale || Persistence.GetPlayedPassedLevelCutscene()))
		{
			Persistence.SetLevelRank(Level.HaileyDuet, -1);
			Persistence.SetLastPlayedLevel(Level.AthleteFinale);
			Persistence.SetGameResult(GameResult.LevelAlreadyWasPassed);
		}
	}

	private new void Start()
	{
		base.Start();
		nicoleAct5Blockage.gameObject.SetActive(value: false);
		LoadLevelData();
		halloweenMode = RDBase.IsHalloweenWeek();
		blitTransitionMaterial = simpleBlit.material;
		float num = Mathf.Round((float)Screen.height / RDBase.Vfx.RDHeight);
		blitTransitionMaterial.SetTextureScale("_TransitionTex", new Vector2(1f, (float)Screen.height / (198f * num)));
		blitTransitionMaterial.SetFloat("_Cutoff", 0f);
		RDBase.Vfx.hudCameraOverlay.gameObject.SetActive(value: false);
		wrldCamera.backgroundColor = Color.black;
		for (int i = 0; i < selectableEntities.Count; i++)
		{
			if (selectableEntities[i] is SelectableCharacter)
			{
				SelectableCharacter selectableCharacter = selectableEntities[i] as SelectableCharacter;
				scrChar orAddComponent = selectableCharacter.gameObject.GetOrAddComponent<scrChar>();
				orAddComponent.customAnimation = selectableCharacter.gameObject.AddComponent<CustomAnimation>();
				orAddComponent.Setup(Character.Samurai);
				selectableCharacter.animation = orAddComponent;
				if (selectableCharacter.gameObject.name == "MD-1")
				{
					museDashLevelRin = selectableCharacter;
				}
			}
		}
		athleteWard.ToggleEdega(visible: false);
		train.rowContainer = trainMovement.transform;
		scene.transform.position = new Vector3(0f, wrldCamera.RDFullNativeResolution().y * 0.5f, 0f);
		RDBase.Vfx.MatchCanvasToCamera(canvas, wrldCamera);
		RDBase.Vfx.MatchCanvasToCamera(topUIcanvas, wrldCamera);
		UnityEngine.Object.Destroy(base.conductor.gameObject.GetComponent<AudioListener>());
		listener = listenerObject.AddComponent<AudioListener>();
		RDRichPresence.Instance?.UpdatePresence();
		escapeArrowContainer.localScale = Vector3.zero;
		if (Persistence.GetLevelRank(Level.KnowYou).passed && !Persistence.GetLevelRank(Level.InsomniacHard).passed)
		{
			GameObject[] array = hospitalLights;
			for (int j = 0; j < array.Length; j++)
			{
				array[j].SetActive(value: false);
			}
		}
		Persistence.GiveLevelAchievements();
		SteamIntegration.StoreStats();
		float orthographicSize = heartMonitor.GetComponent<Canvas>().worldCamera.orthographicSize;
		heartMonitor.GetComponent<RectTransform>().AnchorPosY(0f - orthographicSize);
		speechSource = base.ink.speechSource;
		saidFirstSpeech = false;
		firstSpeech = string.Empty;
		robotsContainer.SetActive(doingRobots);
	}

	private new void Update()
	{
		base.Update();
		string text = DateTime.Now.ToString("hh:mm");
		MobileClockTime.text = text + " " + GetAMorPM(currentDifficulty);
		if (firstFrame)
		{
			firstFrame = false;
			StartFadeIn();
			int num = 0;
			justPlayedLevel = LevelSelector.LevelFromString(Persistence.GetLastPlayedLevel());
			string text2 = scnBase.lastSceneName;
			cameFromBasementComputer = text2 == "scnSleevePaint" || text2 == "scnIanDesktop";
			if (cameFromBasementComputer)
			{
				num = GetBasementComputerId();
			}
			else
			{
				Level level = justPlayedLevel;
				if (level == Level.Intro || level == Level.None || (justPlayedLevel == Level.GongXi && !CheckCNY()) || (justPlayedLevel == Level.Halloween && !RDBase.IsHalloweenWeek()) || (int)Persistence.GetLevelRank(justPlayedLevel) == -2)
				{
					RDBaseDllDummy.printem($"Last level was {justPlayedLevel} which is invalid, defaulting to Samurai Techno");
					justPlayedLevel = Level.OrientalTechno;
				}
				else
				{
					RDBaseDllDummy.printem($"Last level was {justPlayedLevel}");
				}
				num = GetCharacterIndexFromLevel(justPlayedLevel);
			}
			changingWard = false;
			currentWardIndex = selectableEntities[num].group;
			if (currentWardIndex == 11)
			{
				playSongCall?.Kill();
				RDBaseDllDummy.printem("music is banned in the void");
			}
			InstantiateCamera(num);
			text2 = scnBase.lastSceneName;
			Difficulty difficulty = ((text2 == "scnSleevePaint" || text2 == "scnIanDesktop") ? Difficulty.Normal : GetDifficultyFromIndex(num));
			SetDifficulty(difficulty);
			UpdateMusic(selectedEntity, changedArea: true);
			MoveArrowsToEntity(selectedEntity);
			PrepareAthleteWardTransition();
			CheckForCutscene();
			PlaceJanitor();
			MovePaigeToVendingMachine(instant: true);
			UnlockNightShiftLevel(selectedEntity as SelectableCharacter);
			if (halloweenMode)
			{
				mainWard.halloweenMode = true;
				svtWard.halloweenMode = true;
				if (currentWardIndex == 2)
				{
					TriggerSVTWindowMan();
				}
			}
		}
		if (!firstFrame && !firstSpeech.IsNullOrEmpty() && !controlsAreLocked && !rdInk.visible && !saidFirstSpeech)
		{
			SayCurrentEntity(firstSpeech);
			saidFirstSpeech = true;
		}
		if (RDInput.anyPlayerPress)
		{
			EnterAction();
		}
		if (!controlsAreLocked)
		{
			if (RDInput.leftPress)
			{
				LeftArrowAction();
			}
			if (RDInput.rightPress)
			{
				RightArrowAction();
			}
			if (RDInput.upPress)
			{
				UpArrowAction();
			}
			if (RDInput.downPress)
			{
				DownArrowAction();
			}
			if (RDInput.cancelPress && !pauseMenu.showing)
			{
				ShowPauseMenu();
			}
			if (pauseMenu.showing)
			{
				return;
			}
			if (RDInput.rightIsPressed || RDInput.leftIsPressed || IsAnyTouchButtonDown())
			{
				keyTime += Time.deltaTime;
				if (!annoyedDialogueIsRunning)
				{
					keyAnnoyedTime += Time.deltaTime;
				}
				if (keyTime >= 0.45000002f)
				{
					if (RDInput.rightIsPressed || rightArrowButtonDown)
					{
						ChangeSelectedEntity(Direction.Next);
					}
					else if (RDInput.leftIsPressed || leftArrowButtonDown)
					{
						ChangeSelectedEntity(Direction.Previous);
					}
					keyTime -= 0.05f;
				}
				if (keyAnnoyedTime >= 5f && currentWardIndex != 11)
				{
					if (!annoyedDialogueIsRunning)
					{
						Narration.StopSpeaking();
						Narration.ToggleNavigationMessages(toggle: false);
						annoyedDialogueIsRunning = true;
						rightArrowButtonDown = false;
						leftArrowButtonDown = false;
						base.ink.Run("Headache", () =>
						{
							annoyedDialogueIsRunning = false;
						});
						Persistence.UnlockAchievement(Achievement.LevelSelectHeadache, storeStats: true);
					}
					keyAnnoyedTime = 0f;
					RDBaseDllDummy.printem("Annoyed Time!");
				}
			}
			else
			{
				if (annoyedDialogueIsRunning)
				{
					Narration.ToggleNavigationMessages(toggle: true);
					annoyedDialogueIsRunning = false;
					base.ink.Stop();
				}
				keyAnnoyedTime = 0f;
				keyTime = 0f;
			}
			if (RDInput.cancelRelease)
			{
				escapeArrowContainer.localScale = Vector3.zero;
				escapeArrowFill.DOKill();
			}
		}
		if (RDInput.cancelPress && showingArtCanvas)
		{
			showingArtCanvas = false;
			artCanvasFade.DOColor(Color.black, 0.5f).OnComplete(() =>
			{
				uiArtCanvasContainer.SetActive(value: false);
				artCanvasFade.DOColor(Color.clear, 0.5f).OnComplete(() =>
				{
					controlsAreLocked = false;
				});
			});
		}
		if (RDInput.cancelPress && heartMonitor.visible && !heartMonitor.goingToLevel)
		{
			RDBase.Vfx.SpotlightOpenAllTheWay(1f, playSwitchSound: false, RDUtils.GetMixerGroup("StoryLevelSelect"));
			LevelSelectPlaySound("sndPagerClose");
			KeyInfoPanel.instance.UpdateShortcutsRow(null);
			heartMonitor.Hide(() =>
			{
				controlsAreLocked = false;
				SetDifficultyArrowsVisible(visible: true);
				ShowPatientQuote(show: false);
				description.enabled = true;
			});
			arrowsShouldBeVisible = true;
			SayCurrentEntity(lastEntityNarration);
		}
		TouchUpdate();
		if (soundTriggerTimer > 0f)
		{
			soundTriggerTimer -= Time.unscaledDeltaTime;
		}
		else if (isPatientQuoteAnimating)
		{
			speechSource.pitch = RXRandom.Range(0.97087383f, 1.03f);
			speechSource.volume = RXRandom.Range(0.91f, 1.1f) * scrConductor.GlobalOtherSoundVolume;
			speechSource.Play();
			soundTriggerTimer = soundTriggerCooldown + RXRandom.Range(-0.005f, 0.005f);
		}
		static string GetAMorPM(Difficulty difficulty2)
		{
			int num2 = DateTime.Now.Hour % 12;
			if (difficulty2 == Difficulty.Normal)
			{
				if (num2 >= 6)
				{
					return "AM";
				}
				return "PM";
			}
			if (num2 >= 6)
			{
				return "PM";
			}
			return "AM";
		}
	}

	public void ShowPauseMenu()
	{
		if (!pauseMenu.showing)
		{
			controlsAreLocked = true;
			pauseMenu.Show();
			float num = 2f * wrldCamera.orthographicSize + 1f;
			float num2 = num * wrldCamera.aspect + 1f;
			pauseMenuOverlayRect.sizeDelta = new Vector2(num2, num);
			pauseMenuOverlay.gameObject.SetActive(value: true);
			pauseMenuOverlay.color = Color.clear;
			pauseMenuOverlay.DOKill();
			pauseMenuOverlay.DOFade(0.73f, 0.15f).SetUpdate(isIndependentUpdate: true);
		}
	}

	public void HidePauseMenu()
	{
		SayCurrentEntity(lastEntityNarration);
		controlsAreLocked = false;
		pauseMenu.Hide();
		pauseMenuOverlay.DOKill();
		pauseMenuOverlay.DOFade(0f, 0.15f).SetUpdate(isIndependentUpdate: true).OnComplete(() =>
		{
			pauseMenuOverlay.gameObject.SetActive(value: false);
		});
	}

	public void ShowRequiredController()
	{
	}

	public void HideRequiredController()
	{
	}

	private void TouchUpdate()
	{
		if ((!scnBase.isMobile && !scnBase.isSwitch) || controlsAreLocked)
		{
			return;
		}
		bool flag = Input.touchCount == 1;
		bool flag2 = Input.GetMouseButtonDown(0) || Input.GetMouseButton(0) || Input.GetMouseButtonUp(0);
		if (!flag && !flag2)
		{
			return;
		}
		float num = 0f;
		float num2 = 0f;
		int num3 = -1;
		if (flag)
		{
			Touch touch = Input.GetTouch(0);
			num = touch.position.x;
			num2 = touch.position.y;
			if (touch.phase == TouchPhase.Began)
			{
				num3 = 0;
			}
			else if (touch.phase == TouchPhase.Moved)
			{
				num3 = 1;
			}
			else if (touch.phase == TouchPhase.Ended)
			{
				num3 = 2;
			}
		}
		else if (flag2)
		{
			num = Input.mousePosition.x;
			num2 = Input.mousePosition.y;
			if (Input.GetMouseButtonDown(0))
			{
				num3 = 0;
			}
			else if (Input.GetMouseButton(0))
			{
				num3 = 1;
			}
			else if (Input.GetMouseButtonUp(0))
			{
				num3 = 2;
			}
		}
		float num4 = num / (float)Screen.width * RDBase.Vfx.RDWidth;
		float num5 = num2 / (float)Screen.height * RDBase.Vfx.RDHeight;
		float num6 = (0f - (num4 - touchStartX)) * 0.7f;
		switch (num3)
		{
		case 0:
			if (num5 > RDBase.Vfx.RDHeight - 40f)
			{
				validTouchDown = false;
				break;
			}
			ResetTouchesForSwipeCalc();
			AddTouchForSwipeCalc(num4);
			touchStartX = num4;
			camStartX = cameraObject.transform.localPosition.x;
			validTouchDown = true;
			break;
		case 1:
		{
			if (!validTouchDown || Mathf.Abs(num6) < 2f)
			{
				break;
			}
			AddTouchForSwipeCalc(num4);
			float num8 = 100000f;
			foreach (SelectableEntity selectableEntity in selectableEntities)
			{
				if (IsEntitySelectable(selectableEntity))
				{
					num8 = Mathf.Min(num8, selectableEntity.gamePosition.x + selectableEntity.cameraOffset.x);
				}
			}
			float num9 = -100000f;
			foreach (SelectableEntity selectableEntity2 in selectableEntities)
			{
				if (IsEntitySelectable(selectableEntity2))
				{
					num9 = Mathf.Max(num9, selectableEntity2.gamePosition.x + selectableEntity2.cameraOffset.x);
				}
			}
			float num10 = 0.55f;
			float rDWidth = RDBase.Vfx.RDWidth;
			float num11 = camStartX + num6;
			if (camStartX + num6 < num8)
			{
				num6 = (0f - (1f - 1f / ((Mathf.Abs(num6) - (camStartX - num8)) * num10 / rDWidth + 1f))) * rDWidth;
				num11 = num8 + num6;
			}
			else if (camStartX + num6 > num9)
			{
				num6 = (1f - 1f / ((Mathf.Abs(num6) + (camStartX - num9)) * num10 / rDWidth + 1f)) * rDWidth;
				num11 = num9 + num6;
			}
			cameraObject.transform.DOKill();
			cameraObject.transform.LocalMoveX(num11);
			int num12 = -1;
			float num13 = 100000f;
			int num14 = 0;
			foreach (SelectableEntity selectableEntity3 in selectableEntities)
			{
				if (IsEntitySelectable(selectableEntity3))
				{
					float num15 = selectableEntity3.gamePosition.x;
					float num16 = selectableEntity3.cameraOffset.x;
					float num17 = Mathf.Abs(cameraObject.transform.localPosition.x - (num15 + num16));
					if (num17 < num13)
					{
						num13 = num17;
						num12 = num14;
					}
				}
				num14++;
			}
			if (newSelectedIndex != num12)
			{
				newSelectedIndex = num12;
				MoveCamera(num12, animateCamera: true, force: true, moveCamera: false, changeIndex: false);
				ShowRanksText(num12);
			}
			break;
		}
		case 2:
			if (!validTouchDown || Mathf.Abs(num6) < 2f)
			{
				break;
			}
			if (newSelectedIndex == selectedEntityIndex || newSelectedIndex != -1)
			{
				float num7 = LinearRegressionForSwipeCalc();
				if (Mathf.Abs(num7) < 0.01f)
				{
					Direction direction = ((!(num7 > 0f)) ? Direction.Next : Direction.Previous);
					ChangeSelectedEntity(direction);
				}
				else
				{
					MoveCamera(newSelectedIndex, animateCamera: true, force: true);
					UpdateUI();
				}
			}
			else if (newSelectedIndex != -1)
			{
				MoveCamera(newSelectedIndex, animateCamera: true, force: true);
				UpdateUI();
			}
			break;
		}
	}

	private bool IsEntitySelectable(SelectableEntity entity)
	{
		if (currentWardIndex != entity.group)
		{
			return false;
		}
		if ((currentDifficulty == Difficulty.Normal && entity.normalEnabled) || (currentDifficulty == Difficulty.Hard && entity.hardEnabled))
		{
			return true;
		}
		return false;
	}

	private void AddTouchForSwipeCalc(float x)
	{
		if (touchesStored != 30)
		{
			touchPositions[touchesStored] = x;
			touchTimes[touchesStored] = Time.timeSinceLevelLoad;
			touchesStored++;
		}
	}

	private void ResetTouchesForSwipeCalc()
	{
		touchesStored = 0;
	}

	private float LinearRegressionForSwipeCalc()
	{
		int num = touchesStored;
		if (num == 30 || num < 4)
		{
			return -1f;
		}
		float num2 = 0f;
		float num3 = 0f;
		float num4 = 0f;
		float num5 = 0f;
		float num6 = 0f;
		for (int i = 0; i < num; i++)
		{
			float num7 = touchPositions[i];
			float num8 = touchTimes[i];
			num2 += num7;
			num3 += num8;
			num4 += num7 * num7;
			num5 += num7 * num8;
			num6 += num8 * num8;
		}
		return ((float)num * num5 - num2 * num3) / ((float)num * num4 - num2 * num2);
	}

	public void UnlockAllLevels()
	{
		if (Time.unscaledTime - timeSinceLastTap < 0.2f)
		{
			Persistence.UnlockAllLevels();
			scnBase.GoToScene("scnLevelSelect");
		}
		else
		{
			MobileUnlockInstructions.gameObject.SetActive(value: true);
			DOVirtual.DelayedCall(1f, () =>
			{
				MobileUnlockInstructions.gameObject.SetActive(value: false);
			});
		}
		timeSinceLastTap = Time.unscaledTime;
	}

	private void LateUpdate()
	{
		float num = cameraObject.transform.localPosition.x;
		Transform[] array = mainWardForegrounds;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].LocalMoveX(num * parallaxFactor);
		}
		svtWardForeground.LocalMoveX((num - 1380f) * parallaxFactor - 40f);
		abandonedWard.foreground.transform.LocalMoveX((num - 13409f) * parallaxFactor - 40f);
	}

	public void LeftArrowAction()
	{
		if (!controlsAreLocked)
		{
			ChangeSelectedEntity(Direction.Previous);
			RDInput.touchBlocked = true;
		}
	}

	public void RightArrowAction()
	{
		if (!controlsAreLocked)
		{
			ChangeSelectedEntity(Direction.Next);
			RDInput.touchBlocked = true;
		}
	}

	public void DownArrowAction()
	{
		if (!controlsAreLocked)
		{
			ChangeDifficulty(Direction.Down);
			ChangeVerticalDestination(Direction.Down);
			RDInput.touchBlocked = true;
		}
	}

	public void UpArrowAction()
	{
		if (!controlsAreLocked)
		{
			ChangeDifficulty(Direction.Up);
			ChangeVerticalDestination(Direction.Up);
			RDInput.touchBlocked = true;
		}
	}

	private void ChangeVerticalDestination(Direction direction)
	{
		if (selectedEntity is SelectableObject { id: var id } && (id == "MainElevator" || id == "BasementComputer"))
		{
			if (direction == Direction.Up)
			{
				upArrow.Pulse();
			}
			else
			{
				downArrow.Pulse();
			}
			int num = selectedVerticalIndex;
			selectedVerticalIndex = (int)Mathf.Repeat(selectedVerticalIndex + ((direction == Direction.Up) ? 1 : (-1)), selectedVerticalDestinations.Count);
			if (selectedVerticalIndex != num)
			{
				LevelSelectPlaySound("sndSelectObject", 1f, RDUtils.PitchSemitonesToPercent((direction == Direction.Up) ? 0.5f : (-0.5f)));
			}
			UpdateUI();
		}
	}

	public void EnterAction()
	{
		if (!enterIsLocked)
		{
			PerformEntityAction();
		}
	}

	private SelectableEntity GetSelectableEntity(string id)
	{
		foreach (SelectableEntity selectableEntity in selectableEntities)
		{
			if (selectableEntity.id.Equals(id))
			{
				return selectableEntity;
			}
		}
		return null;
	}

	private void DoTransition(SelectableEntity fromEntity, SelectableEntity toEntity, Action onArrive = null, Action onFinish = null, bool forceMenuTransition = false)
	{
		if ((Persistence.GetFastMenu() || RDBase.debugSettings.SkipMenuTransitions) && !forceMenuTransition)
		{
			LevelSelectPlaySound("sndTransitionShort");
			onArrive();
			onFinish();
			UpdateMusic(toEntity, changedArea: true);
			return;
		}
		transitionAnimation.isReversed = true;
		transitionAnimation.CalculateTransitionPieces();
		transitionAnimation.PlayTransition(fromEntity);
		float transitionDuration = transitionAnimation.transitionDuration;
		cameraObject.transform.DOKill();
		LevelSelectPlaySound("sndTransitionOut");
		StopAllAmbiences(transitionDuration);
		DOVirtual.DelayedCall(transitionDuration, () =>
		{
			LevelSelectPlaySound("sndTransitionIn");
			StartAllAmbiences();
			UpdateMusic(toEntity, changedArea: true);
			if (onArrive != null)
			{
				onArrive();
			}
			transitionAnimation.isReversed = false;
			transitionAnimation.PlayTransition(toEntity, isReverting: true);
		});
		DOVirtual.DelayedCall(transitionDuration * 1.85f, () =>
		{
			if (onFinish != null)
			{
				onFinish();
			}
		});
	}

	private void PerformEntityAction()
	{
		if (controlsAreLocked || dialogueIsRunning)
		{
			return;
		}
		if (selectedEntity is SelectableCharacter)
		{
			SelectableCharacter character = selectedEntity as SelectableCharacter;
			Level level = character.levels[currentDifficulty];
			if (Persistence.GetStateFromLevel(level) != LevelState.NotAvailable)
			{
				if (level == Level.AthleteFinale && !steamOverlayDialoguePlayed && scnBase.isMac && SteamWorkshop.OverlayEnabled())
				{
					WindowMovement windowMovement = RDC.windowMovement;
					if (windowMovement == WindowMovement.Full || windowMovement == WindowMovement.OneScreen)
					{
						controlsAreLocked = true;
						steamOverlayDialoguePlayed = true;
						base.ink.Run("Level_AthleteFinale_New_SteamOverlay", () =>
						{
							SelectCharacter(character, currentDifficulty);
						});
						return;
					}
				}
				SelectCharacter(character, currentDifficulty);
				return;
			}
			wrldCamera.transform.DOKill(complete: true);
			wrldCamera.transform.DOShakePosition(0.5f, 3f, 40, 90f, snapping: true);
			GameObject obj = GameObject.Find("Scene");
			obj.transform.DOKill(complete: true);
			obj.transform.DOShakePosition(0.5f, 3f, 40, 90f, snapping: true);
			LevelSelectPlaySound("sndError");
			lockedLevelPresses++;
			if (lockedLevelPresses == 20)
			{
				controlsAreLocked = true;
				base.ink.Run("MashingLockedLevel", () =>
				{
					controlsAreLocked = false;
				});
				Persistence.UnlockAchievement(Achievement.LevelSelectSpam, storeStats: true);
			}
		}
		else
		{
			if (!(selectedEntity is SelectableObject))
			{
				return;
			}
			controlsAreLocked = true;
			SelectableObject currentSelectableObject = selectedEntity as SelectableObject;
			KeyInfoPanel.instance.UpdateShortcutsRowWithStrings(buttonInstructionsDefault);
			if (currentSelectableObject.action.StartsWith("transition:") || currentSelectableObject.action == "vendingMachinePaige")
			{
				string transitionTo = currentSelectableObject.action.Replace("transition:", "");
				if (currentSelectableObject.id == "MainElevator")
				{
					transitionTo = selectedVerticalDestinations[selectedVerticalIndex].targetObjectId;
					if (transitionTo == "ExitVoid")
					{
						MusicFadeOut(transitionAnimation.transitionDuration);
						RDBaseDllDummy.printem("the void fades out all music");
					}
				}
				bool shouldPlayPostAct2Cutscene = currentSelectableObject.id == "ExitSVTWard" && !Persistence.GetPlayedPostAct2Cutscene() && Persistence.GetLevelRank(Level.Boss2).passed;
				bool shouldPlayPostAct3Cutscene = currentSelectableObject.id == "VendingMachinePaige" && Persistence.GetLevelRank(Level.Lesmis).passed;
				bool shouldPlayPostAct4Cutscene = currentSelectableObject.id == "ExitTrain" && !Persistence.GetPlayedPostAct4Cutscene() && Persistence.GetLevelRank(Level.KnowYou).passed;
				bool num = currentSelectableObject.id == "GoToAthleteWard" && !Persistence.GetLevelRank(Level.CareLess).passed;
				bool shouldPlayPreAct6Cutscene = currentSelectableObject.id == "ExitAthleteWard" && !Persistence.GetPlayedPreAct6Cutscene() && Persistence.GetLevelRank(Level.AthleteFinale).passed;
				bool shouldPlayPreBitternessCutscene = currentSelectableObject.id == "ExitRecordsRoom" && !Persistence.GetPlayedPreBitternessCutscene() && Persistence.GetLevelRank(Level.Blurred).passed;
				bool shouldPlayRooftopCutscene = currentSelectableObject.id == "ExitSVTWard" && !Persistence.GetPlayedRooftopCutscene() && Persistence.GetLevelRank(Level.Bitterness).passed;
				bool flag = shouldPlayPostAct2Cutscene | shouldPlayPostAct3Cutscene | shouldPlayPostAct4Cutscene | shouldPlayPreAct6Cutscene | shouldPlayPreBitternessCutscene | shouldPlayRooftopCutscene;
				if (num)
				{
					int careless = GetCharacterIndexFromLevel(Level.CareLess);
					SelectableCharacter carelessChar = selectableEntities[careless] as SelectableCharacter;
					SelectableEntity svtExit = GetSelectableEntity("ExitSVTWard");
					controlsAreLocked = true;
					dialogueIsRunning = true;
					base.ink.Run("Act5_Blockade", () =>
					{
						dialogueIsRunning = false;
						Action onArrive2 = () =>
						{
							if (currentDifficulty == Difficulty.Normal)
							{
								ChangeDifficulty(Direction.Down, force: true);
							}
							MoveCamera(careless, animateCamera: false, force: true);
							UpdateUI(refreshCharacters: true);
							RDBase.Vfx.SpotlightFocusFromCenter(carelessChar.animation.transform.position.x, 0.5f);
						};
						Action onFinish2 = () =>
						{
							DOVirtual.DelayedCall(0.5f, () =>
							{
								controlsAreLocked = false;
								RDBase.Vfx.SpotlightOpenAllTheWay(1f, playSwitchSound: true, RDUtils.GetMixerGroup("StoryLevelSelect"));
							});
						};
						DoTransition(currentSelectableObject, svtExit, onArrive2, onFinish2);
					});
					return;
				}
				previousWard = transitionTo;
				SelectableEntity entityToSelect = null;
				int indexToSelect = 0;
				foreach (SelectableEntity selectableEntity in selectableEntities)
				{
					if (selectableEntity.id.Equals(transitionTo))
					{
						entityToSelect = selectableEntity;
						break;
					}
					indexToSelect++;
				}
				if (currentSelectableObject.id == "ExitRooftop")
				{
					selectedVerticalIndex = 1;
				}
				else
				{
					selectedVerticalIndex = 0;
				}
				if (entityToSelect == null && !flag)
				{
					Debug.LogWarning("Object not found");
					return;
				}
				switch (currentSelectableObject.id)
				{
				case "GoToTrain":
					LevelSelectPlaySound("sndTrainWhistle");
					break;
				case "MainElevator":
				case "ExitRecordsRoom":
					LevelSelectPlaySound("sndElevatorDing");
					break;
				}
				bool shouldPlayHaileyDuetIntroduction = entityToSelect?.id == "ExitRecordsRoom" && !Persistence.GetPlayedHaileyDuetIntroduction();
				bool shouldPlayBitternessWarning = entityToSelect?.id == "ExitRecordsRoom" && !bitternessWarningPlayed && !Level.Bitterness.Locked() && !Level.Bitterness.Passed();
				Action onArrive = () =>
				{
					if (entityToSelect != null)
					{
						changingWard = true;
						MoveCamera(indexToSelect, animateCamera: false, force: true);
						UpdateUI();
					}
					switch (currentSelectableObject.id)
					{
					default:
						if (!(currentSelectableObject.id == "GoToAthleteWard") || Level.LuckyBreak.Passed())
						{
							string text = entityToSelect?.id;
							if (!(text == "ExitVoid") && !(text == "ExitRecordsRoom"))
							{
								break;
							}
						}
						goto case "GoToArtRoom";
					case "GoToArtRoom":
					case "GoToMuseDashRoom":
					case "GoToBasement":
						if (currentDifficulty == Difficulty.Hard)
						{
							ChangeDifficulty(Direction.Down, force: true);
						}
						break;
					}
					if (shouldPlayPostAct2Cutscene)
					{
						scnBase.GoToCutscene("act3_cutsceneA");
						Persistence.SetLastPlayedLevel(Level.Garden);
						Persistence.SetPlayedPostAct2Cutscene(played: true);
						Persistence.SaveCurrentSlot();
					}
					else if (shouldPlayPostAct3Cutscene)
					{
						scnBase.GoToCutscene("act4_cutsceneA");
						Persistence.SetLastPlayedLevel(Level.Heldbeats);
						Persistence.SetPlayedPostAct3Cutscene(played: true);
						Persistence.SaveCurrentSlot();
					}
					else
					{
						if (shouldPlayPostAct4Cutscene)
						{
							controlsAreLocked = true;
							dialogueIsRunning = true;
							Persistence.SetLastPlayedLevel(Level.InsomniacHard);
							int characterIndexFromLevel = GetCharacterIndexFromLevel(Level.OrientalInsomniac);
							MoveCamera(characterIndexFromLevel);
							SelectableCharacter soiCharacter = selectableEntities[characterIndexFromLevel] as SelectableCharacter;
							DOVirtual.DelayedCall(1f, () =>
							{
								RDBase.Vfx.SpotlightFocusFromCenter(soiCharacter.animation.transform.position.x, 0.5f, playSwitchSound: true, RDUtils.GetMixerGroup("StoryLevelSelect"));
							});
							if (currentDifficulty == Difficulty.Normal)
							{
								ChangeDifficulty(Direction.Up);
							}
							Persistence.SetPlayedPostAct4Cutscene(played: true);
							Persistence.SaveCurrentSlot();
							base.ink.Run("Level_InsomniacHard_New", () =>
							{
								controlsAreLocked = false;
								dialogueIsRunning = false;
								RDBase.Vfx.SpotlightOpenAllTheWay(1f, playSwitchSound: true, RDUtils.GetMixerGroup("StoryLevelSelect"));
							});
						}
						if (shouldPlayPreBitternessCutscene)
						{
							controlsAreLocked = true;
							dialogueIsRunning = true;
							Persistence.SetLastPlayedLevel(Level.Bitterness);
							int characterIndexFromLevel2 = GetCharacterIndexFromLevel(Level.Bitterness);
							MoveCamera(characterIndexFromLevel2);
							SelectableCharacter bitterness = selectableEntities[characterIndexFromLevel2] as SelectableCharacter;
							DOVirtual.DelayedCall(1f, () =>
							{
								RDBase.Vfx.SpotlightFocusFromCenter(bitterness.animation.transform.position.x, 0.5f, playSwitchSound: true, RDUtils.GetMixerGroup("StoryLevelSelect"));
							});
							if (currentDifficulty == Difficulty.Normal)
							{
								ChangeDifficulty(Direction.Up);
							}
							Persistence.SetPlayedPreBitternessCutscene(played: true);
							Persistence.SaveCurrentSlot();
							DOVirtual.DelayedCall(1f, () =>
							{
								base.ink.Run("Level_Bitterness_New", () =>
								{
									controlsAreLocked = false;
									dialogueIsRunning = false;
									RDBase.Vfx.SpotlightOpenAllTheWay(1f, playSwitchSound: true, RDUtils.GetMixerGroup("StoryLevelSelect"));
								});
							});
							entityToSelect = GetSelectableEntity("ExitSVTWard");
						}
						if (shouldPlayPreAct6Cutscene)
						{
							scnBase.GoToCutscene("EdegaPerformance/EdegaPerformance");
							Persistence.SetLastPlayedLevel(Level.AthleteFinale);
							Persistence.SetLevelRank(Level.HaileyDuet, -1);
							Persistence.SetPlayedPreAct6Cutscene(played: true);
							Persistence.SaveCurrentSlot();
						}
						if (shouldPlayRooftopCutscene)
						{
							scnBase.GoToCutscene(Persistence.GetPaigeEnding() ? "cutscene_rooftop_paige" : "cutscene_rooftop");
							Persistence.SetLastPlayedLevel(Level.Bitterness);
							Persistence.SetPlayedRooftopCutscene(played: true);
							Persistence.SetPlayedPassedLevelCutscene(played: false);
							Persistence.SaveCurrentSlot();
						}
						if (transitionTo == "ExitMuseDashRoom")
						{
							MoveRin(1, transitionAnimation.transitionDuration);
						}
						else if (transitionTo == "GoToMuseDashRoom")
						{
							MoveRin(3, transitionAnimation.transitionDuration);
						}
						else if (transitionTo == "ExitSVTWard" && halloweenMode)
						{
							TriggerSVTWindowMan();
						}
						DOVirtual.DelayedCall(0.5f, () =>
						{
							for (int i = 0; i < easterEggArray.Length; i++)
							{
								EasterEggAnimation easterEggAnimation = easterEggArray[i];
								if (!(easterEggAnimation.wardName != previousWard))
								{
									if (easterEggAnimation.currentBetweenShowUps == 0)
									{
										easterEggAnimation.currentBetweenShowUps = easterEggAnimation.minTimesBetweenShowUps;
										if (UnityEngine.Random.Range(0f, 1f) <= easterEggAnimation.showUpChance)
										{
											StartCoroutine(ShowEasterEggCo(i));
										}
									}
									else
									{
										easterEggAnimation.currentBetweenShowUps--;
									}
								}
							}
						});
					}
				};
				Action onFinish = () =>
				{
					if (!(shouldPlayPostAct4Cutscene | shouldPlayHaileyDuetIntroduction | shouldPlayPreBitternessCutscene | shouldPlayBitternessWarning))
					{
						controlsAreLocked = false;
					}
					if (shouldPlayHaileyDuetIntroduction)
					{
						SetUIVisible(visible: false);
						base.ink.Run("Act6_HaileyDuet_Appears", () =>
						{
							int characterIndexFromLevel = GetCharacterIndexFromLevel(Level.HaileyDuet);
							MoveCamera(characterIndexFromLevel);
							UpdateUI();
							Persistence.SetPlayedHaileyDuetIntroduction(played: true);
							controlsAreLocked = false;
							SetUIVisible(visible: true);
						});
					}
					if (shouldPlayBitternessWarning)
					{
						base.ink.Run("BitternessRecordsRoomWarning", () =>
						{
							bitternessWarningPlayed = true;
							controlsAreLocked = false;
						});
					}
				};
				bool forceMenuTransition = shouldPlayPostAct2Cutscene | shouldPlayPostAct3Cutscene | shouldPlayPreAct6Cutscene | shouldPlayPreBitternessCutscene | shouldPlayRooftopCutscene;
				DoTransition(selectedEntity, entityToSelect, onArrive, onFinish, forceMenuTransition);
				if (flag)
				{
					MusicFadeOut(transitionAnimation.transitionDuration);
				}
				if (!Persistence.GetFastMenu() && !RDBase.debugSettings.SkipMenuTransitions)
				{
					if (transitionTo == "ExitMuseDashRoom")
					{
						MoveRin(0, transitionAnimation.transitionDuration);
					}
					else if (transitionTo == "GoToMuseDashRoom")
					{
						MoveRin(2, transitionAnimation.transitionDuration);
					}
				}
			}
			else if (currentSelectableObject.action == "BasementComputer")
			{
				RDBase.Vfx.FadeOut();
				StopAllAmbiences(0.5f);
				DOVirtual.DelayedCall(0.5f, () =>
				{
					string targetObjectId = selectedVerticalDestinations[selectedVerticalIndex].targetObjectId;
					if (targetObjectId == "sleevePaint")
					{
						scnBase.GoToSleevePaint();
					}
					else if (targetObjectId == "IanDesktop")
					{
						scnBase.GoToIanDesktop();
					}
				});
			}
			else if (currentSelectableObject.action == "artCanvas")
			{
				showingArtCanvas = true;
				artCanvasFade.color = Color.clear;
				artCanvasFade.DOFade(1f, 0.5f).SetEase(Ease.OutExpo).OnComplete(() =>
				{
					uiArtCanvasContainer.SetActive(value: true);
					uiArtCanvasImage.texture = FanArtLoader.art;
					artCanvasFade.DOFade(0f, 0.5f).SetEase(Ease.InExpo).OnComplete(() =>
					{
						string text = RDString.Get("levelSelect.ArtCanvas.comment");
						text = text.Replace("[artist]", FanArtLoader.artist);
						int num2 = 0;
						string[] tags = FanArtLoader.tags;
						foreach (string text2 in tags)
						{
							text = text.Replace($"[emotion{num2}]", RDString.Get("levelSelect.ArtCanvas." + text2));
							num2++;
						}
						base.ink.RunFromText(text);
					});
				});
			}
			else if (currentSelectableObject.action.StartsWith("ink:"))
			{
				string knot = currentSelectableObject.action.Replace("ink:", "");
				RDInk.RDInkCallback onDialogEnd = () =>
				{
					controlsAreLocked = false;
					dialogueIsRunning = false;
				};
				if (currentSelectableObject.id.StartsWith("VoidItem") && Level.Montage.Locked())
				{
					string id = currentSelectableObject.id;
					int itemIndex = int.Parse(id[id.Length - 1].ToString());
					if (voidItemProgress < itemIndex)
					{
						onDialogEnd = (RDInk.RDInkCallback)Delegate.Combine(onDialogEnd, (RDInk.RDInkCallback)(() =>
						{
							voidItemProgress = itemIndex;
							if (voidItemProgress == 3)
							{
								selectableEntities.Find((SelectableEntity s) => s.id == "7-X").normalEnabled = true;
								Persistence.SetLevelRank(Level.Montage, -1);
								Persistence.SaveCurrentSlot();
							}
							else
							{
								int nextItemIndex = itemIndex + 1;
								selectableEntities.Find((SelectableEntity s) => s.id == $"VoidItem{nextItemIndex}").normalEnabled = true;
								abandonedWard.voidItems[nextItemIndex - 1].DOFade(1f, 1f);
							}
						}));
						base.ink.Run("diaLevelSelect", knot, () =>
						{
							base.ink.Run("diaLevelSelect", knot + "_Dialogue", onDialogEnd);
						});
						base.ink.stickyPanel = true;
					}
					else
					{
						base.ink.Run("diaLevelSelect", knot, onDialogEnd);
					}
				}
				else
				{
					base.ink.Run("diaLevelSelect", knot, onDialogEnd);
				}
			}
			else if (currentSelectableObject.action == "talkToJanitor")
			{
				dialogueIsRunning = true;
				base.ink.startingFromPlayerAction = true;
				RDInk.RDInkCallback callback = () =>
				{
					controlsAreLocked = false;
					dialogueIsRunning = false;
				};
				if (doingRobots && !robotJanitorDialoguePlayed)
				{
					robotJanitorDialoguePlayed = true;
					base.ink.Run("diaLevelSelect", "Janitor_Robot", () =>
					{
						base.ink.Run("diaLevelSelect", GetCurrentJanitorDialogue(), callback);
					});
				}
				else
				{
					base.ink.Run("diaLevelSelect", GetCurrentJanitorDialogue(), callback);
				}
			}
			else
			{
				dialogueIsRunning = true;
				currentSelectableObject.ActionToPerform(currentDifficulty, currentSelectableObject.action);
			}
		}
	}

	private string GetCurrentJanitorDialogue()
	{
		Level level = LevelSelector.LevelFromString(Persistence.GetLastPlayedLevel());
		if (level != Level.None && !level.Passed())
		{
			level = ((level == Level.Montage2) ? Level.Montage : Level.None);
		}
		string text = "Janitor_" + level;
		if (base.ink.story.HasFunction(text))
		{
			return text;
		}
		return "Janitor_Default";
	}

	public void ActivateEntranceRin()
	{
		museDashEntranceRin.gameObject.SetActive(value: true);
		museDashEntranceRin.shaderDataSource = museDashEntranceRin;
		museDashEntranceRin.shaderData.borderColor = Color.black;
		museDashEntranceRin.shaderData.outline = 1;
	}

	public void MovePaigeToVendingMachine(bool instant = false)
	{
		if (Persistence.GetStateFromLevel(Level.Lesmis) != LevelState.Passed || (dontMovePaigeYet & instant) || Persistence.GetPlayedPostAct3Cutscene())
		{
			vendingMachinePaige.gameObject.SetActive(value: false);
			return;
		}
		SelectableObject selectableObject = FindSelectableEntity("PaigeChair") as SelectableObject;
		if (instant)
		{
			SelectableObject obj = FindSelectableEntity("VendingMachinePaige") as SelectableObject;
			obj.normalEnabled = true;
			obj.hardEnabled = true;
			SelectableCharacter obj2 = FindSelectableEntity("3-X") as SelectableCharacter;
			obj2.normalEnabled = false;
			obj2.hardEnabled = false;
			vendingMachinePaige.shaderDataSource = vendingMachinePaige;
			vendingMachinePaige.shaderData.borderColor = Color.black;
			vendingMachinePaige.shaderData.outline = 1;
			selectableObject.normalEnabled = true;
			selectableObject.hardEnabled = true;
			if (selectedEntity.id == "3-X")
			{
				selectedEntityIndex = selectableEntities.IndexOf(selectableObject);
				UpdateUI();
			}
			vendingMachinePaige.gameObject.SetActive(value: true);
			sleepingPaige.gameObject.SetActive(value: false);
		}
		else
		{
			dontMovePaigeYet = false;
			float num = 1f;
			StartFadeOut(num);
			DOVirtual.DelayedCall(num, () =>
			{
				MovePaigeToVendingMachine(instant: true);
				SetUIVisible(visible: true);
				StartFadeIn();
				controlsAreLocked = false;
			});
		}
	}

	private IEnumerator ShowEasterEggCo(int easterEggPos)
	{
		EasterEggAnimation easterEgg = easterEggArray[easterEggPos];
		float time = UnityEngine.Random.Range(easterEgg.minTimesBetweenShowUps, easterEgg.maxTimeToPlayAnimation);
		yield return new WaitForSecondsRealtime(time);
		int num = -1;
		for (int i = 0; i < selectableEntities.Count; i++)
		{
			if (easterEgg.entityId == selectableEntities[i].id)
			{
				num = i;
			}
		}
		if (num >= 0)
		{
			easterEgg.animator.Play();
			SelectableEntity entity = selectableEntities[num];
			entity.hardEnabled = true;
			entity.normalEnabled = true;
			while (easterEgg.animator.IsPlaying(easterEgg.clipName))
			{
				yield return new WaitForEndOfFrame();
			}
			entity.hardEnabled = false;
			entity.normalEnabled = false;
		}
		else
		{
			printe("Easter egg entity " + easterEgg.entityId + " was not found");
		}
	}

	private bool CharacterHasManyLevels()
	{
		if (selectedEntity is SelectableObject)
		{
			return false;
		}
		SelectableCharacter selectableCharacter = selectedEntity as SelectableCharacter;
		Dictionary<Difficulty, Level> levels = selectableCharacter.levels;
		int num = 0;
		if (levels[Difficulty.Normal] != Level.None && selectableCharacter.normalEnabled)
		{
			num++;
		}
		if (levels[Difficulty.Hard] != Level.None && selectableCharacter.hardEnabled)
		{
			num++;
		}
		return num > 1;
	}

	private void SetDifficulty(Difficulty newDifficulty)
	{
		currentDifficulty = newDifficulty;
		SetBackgroundAppearance(newDifficulty);
		UpdateUI(refreshCharacters: true);
	}

	private void ChangeDifficulty(Direction direction, bool force = false)
	{
		if (!force)
		{
			if (selectedEntity is SelectableCharacter && !CharacterHasManyLevels())
			{
				string newValue = ((currentDifficulty == Difficulty.Hard) ? RDString.Get("narration.day") : RDString.Get("narration.night"));
				Narration.Say(RDString.Get("narration.availableShift").Replace("[shift]", newValue), NarrationCategory.Navigation, false);
				return;
			}
			if (selectedEntity is SelectableObject)
			{
				Narration.Say(RDString.Get("narration.availableShiftNonLevel"), NarrationCategory.Navigation, false);
				return;
			}
		}
		sequence.Kill(complete: true);
		verticalMovement.transform.localPosition = Vector3.zero;
		train.rowTween?.Kill();
		trainMovement.transform.localPosition = Vector3.zero;
		Difficulty difficulty = ((currentDifficulty != Difficulty.Normal) ? Difficulty.Normal : Difficulty.Hard);
		if (difficulty != currentDifficulty)
		{
			SetDifficulty(difficulty);
			LevelSelectPlaySound((difficulty == Difficulty.Normal) ? "sndShiftChangeToDay" : "sndShiftChangeToNight");
		}
		sequence = DOTween.Sequence();
		sequence.Append(verticalMovement.transform.DOLocalMoveY(5f, 0.12f, snapping: true).SetEase(Ease.OutQuad).SetLoops(2, LoopType.Yoyo)).Append(verticalMovement.transform.DOLocalMoveY(1f, 0.08f, snapping: true).SetEase(Ease.OutQuad).SetLoops(2, LoopType.Yoyo));
		train.CheckForBumpyRows();
		if (!force)
		{
			if (direction == Direction.Up)
			{
				upArrow.Pulse();
			}
			else
			{
				downArrow.Pulse();
			}
		}
	}

	private void ChangeSelectedEntity(Direction direction, int recursiveValue = 0)
	{
		int count = selectableEntities.Count;
		int num = recursiveValue;
		float semitones = 0f;
		switch (direction)
		{
		case Direction.Previous:
			num = (selectedEntityIndex - 1 + count + recursiveValue) % count;
			semitones = -0.5f;
			switch (currentDifficulty)
			{
			case Difficulty.Normal:
				if (!selectableEntities[num].normalEnabled || selectableEntities[num].group != currentWardIndex)
				{
					ChangeSelectedEntity(direction, recursiveValue - 1);
					return;
				}
				break;
			case Difficulty.Hard:
				if (!selectableEntities[num].hardEnabled || selectableEntities[num].group != currentWardIndex)
				{
					ChangeSelectedEntity(direction, recursiveValue - 1);
					return;
				}
				break;
			}
			break;
		case Direction.Next:
			num = (selectedEntityIndex + 1 + count + recursiveValue) % count;
			semitones = 0.5f;
			switch (currentDifficulty)
			{
			case Difficulty.Normal:
				if (!selectableEntities[num].normalEnabled || selectableEntities[num].group != currentWardIndex)
				{
					ChangeSelectedEntity(direction, recursiveValue + 1);
					return;
				}
				break;
			case Difficulty.Hard:
				if (!selectableEntities[num].hardEnabled || selectableEntities[num].group != currentWardIndex)
				{
					ChangeSelectedEntity(direction, recursiveValue + 1);
					return;
				}
				break;
			}
			break;
		}
		SelectableEntity selectableEntity = selectableEntities[num];
		string sound = "sndSelectPatient";
		if (selectableEntity is SelectableObject)
		{
			SelectableObject selectableObject = selectableEntity as SelectableObject;
			if (selectableObject.action.StartsWith("transition:"))
			{
				sound = "sndSelectWard";
			}
			else
			{
				semitones = 0f;
				sound = selectableObject.action switch
				{
					"BasementComputer" => "sndSelectSleevePaint", 
					"talkToJanitor" => "sndSelectJanitor", 
					"vendingMachinePaige" => "sndSelectEvent", 
					_ => "sndSelectObject", 
				};
			}
		}
		else
		{
			_ = selectableEntity is SelectableCharacter;
		}
		lockedLevelPresses = 0;
		LevelSelectPlaySound(sound, 1f, RDUtils.PitchSemitonesToPercent(semitones));
		MoveCamera(num);
		UpdateUI();
	}

	private void UpdateUI(bool refreshCharacters = false)
	{
		ShowRanksText(selectedEntityIndex);
		UpdateCharacters(isLevelUnlock: false, refreshCharacters);
		float num = (refreshCharacters ? 999f : 1f);
		rwCabinet.Play((selectedEntity.id == "5-B1") ? "AthleteWard_RW_on" : "AthleteWard_RW_off");
		RDMainWard rDMainWard = mainWard;
		string id = selectedEntity.id;
		rDMainWard.SetOfficeDoorOpened(id == "PaigeChair" || id == "3-X", num * 0.5f);
		mainWard.SetBasementDoorOpened(selectedEntity.id == "GoToBasement");
		UpdateMusic(selectedEntity);
	}

	public void UpdateMusic(SelectableEntity entity, bool changedArea = false)
	{
		if (entity == null)
		{
			return;
		}
		if (entity.group == 10)
		{
			switch (entity.id)
			{
			case "6-2":
			case "6-X":
			case "7-1":
				PlaySong("sndMarksTheme", 55f, 3f);
				break;
			default:
				if (!Level.HaileyDuet.Passed())
				{
					PlaySong("sndTutorial", 88f);
				}
				else
				{
					base.conductor.FadeSong(0f, 2f);
				}
				break;
			}
		}
		else if (entity.group == 11)
		{
			base.conductor.FadeSong(0f, 0f);
		}
		else if (changedArea)
		{
			string id = entity.id;
			if (!(id == "ExitSVTWard") && !(id == "GoToSVTWard"))
			{
				PlaySong("sndTutorial", 88f);
			}
		}
		if (changedArea && !base.conductor.IsSongPlaying())
		{
			PlaySong("sndNone", 88f);
		}
	}

	private void UpdateCharacters(bool isLevelUnlock, bool refreshCharacters = false)
	{
		foreach (WalkingSelectableCharacter walkingCharacter in walkingCharacters)
		{
			walkingCharacter.UpdateCharacter(selectedEntity.id, refreshCharacters, currentDifficulty == Difficulty.Normal);
		}
		for (int i = 0; i < selectableEntities.Count; i++)
		{
			SelectableEntity selectableEntity = selectableEntities[i];
			if (!(selectableEntity is SelectableCharacter))
			{
				continue;
			}
			SelectableCharacter selectableCharacter = selectableEntity as SelectableCharacter;
			if (selectableCharacter.levels.ContainsKey(currentDifficulty))
			{
				LevelState stateFromLevel = Persistence.GetStateFromLevel(selectableCharacter.levels[currentDifficulty]);
				ChangeCharacterAppearance(i, stateFromLevel, isLevelUnlock, refreshCharacters);
				if (selectableCharacter.id == "X-WOT" && stateFromLevel != LevelState.NotAvailable)
				{
					selectableCharacter.animation.shaderData.borderColor = Color.clear;
				}
			}
			if (selectableCharacter.levels[Difficulty.Normal] == Level.RhythmWeightlifter)
			{
				bool flag = Level.AthleteFinale.Passed();
				bool flag2 = currentDifficulty == Difficulty.Hard;
				bool flag3 = flag && !flag2;
				rwCabinetTarp.SetActive(!flag3);
				rwCabinet.gameObject.SetActive(flag3);
				rwCabinetTarp.GetComponent<tk2dSpriteAnimator>().Play((flag & flag2) ? "AthleteWard_RW_covered" : "AthleteWard_RW_coveredNoSign");
				rwLowpassFilter.cutoffFrequency = (flag3 ? 2800f : 700f);
			}
		}
	}

	private void ChangeCharacterAppearance(int levelIndex, LevelState levelState, bool isLevelUnlock = false, bool refreshCharacters = false)
	{
		if (!(selectableEntities[levelIndex] is SelectableCharacter))
		{
			return;
		}
		SelectableCharacter selectableCharacter = selectableEntities[levelIndex] as SelectableCharacter;
		Character[] array = ((currentDifficulty == Difficulty.Hard) ? selectableCharacter.charactersHard : selectableCharacter.charactersNormal);
		if (selectableCharacter.levels[currentDifficulty] == Level.StevensonsTango && levelState == LevelState.Passed)
		{
			array = new Character[1] { Character.MrStevensonTango };
		}
		if (selectableCharacter.levels[currentDifficulty] == Level.HaileyDuet && levelState == LevelState.Passed)
		{
			array = new Character[2]
			{
				Character.Boy,
				Character.Girl
			};
		}
		RDShaderProperties shader = new RDShaderProperties();
		shader.outline = 1;
		Rank levelRank = Persistence.GetLevelRank(selectableCharacter.levels[currentDifficulty]);
		switch (levelState)
		{
		case LevelState.NotPassed:
			shader.borderColor = Color.black;
			shader.overlayColor = Color.clear;
			break;
		case LevelState.Passed:
			shader.borderColor = Color.black;
			shader.overlayColor = Color.clear;
			break;
		default:
			shader.borderColor = base.gc.levelSelect_lockedLevelTextOutline;
			shader.overlayColor = Color.black;
			break;
		}
		if (currentDifficulty == Difficulty.Normal)
		{
			if (!selectableCharacter.normalEnabled)
			{
				shader.opacity = 0f;
				shader.borderColor = shader.borderColor.WithAlpha(0f);
			}
		}
		else if (!selectableCharacter.hardEnabled)
		{
			shader.opacity = 0f;
			shader.borderColor = shader.borderColor.WithAlpha(0f);
		}
		if (array[0] == Character.Cranky && levelState != LevelState.NotAvailable)
		{
			shader.borderColor = Color.clear;
		}
		bool flag = levelIndex == selectedEntityIndex;
		bool flag2 = array.Length > 1;
		Transform parent = ((selectableCharacter.group == 5) ? trainMovement.transform : verticalMovement.transform);
		if (refreshCharacters)
		{
			if (selectableCharacter.childAnimations.Length != 0)
			{
				scrChar[] childAnimations = selectableCharacter.childAnimations;
				for (int i = 0; i < childAnimations.Length; i++)
				{
					UnityEngine.Object.Destroy(childAnimations[i].gameObject);
				}
			}
			int num = (flag2 ? (array.Length - 1) : 0);
			selectableCharacter.childAnimations = new scrChar[num];
		}
		for (int j = 0; j < array.Length; j++)
		{
			Character character = array[j];
			if (halloweenMode && character == Character.Samurai && selectableCharacter.levels.Values.Contains(Level.OrientalTechno))
			{
				character = Character.SamuraiPirate;
			}
			scrChar scrChar2;
			if (j > 0)
			{
				if (refreshCharacters)
				{
					scrChar2 = scrChar.Create(character);
					scrChar2.transform.SetParent(parent, worldPositionStays: true);
					scrChar2.transform.position = selectableCharacter.gameObject.transform.position;
					selectableCharacter.childAnimations[j - 1] = scrChar2;
				}
				else
				{
					scrChar2 = selectableCharacter.childAnimations[j - 1];
				}
			}
			else
			{
				scrChar2 = selectableCharacter.animation;
			}
			scrChar2.shaderDataSource = scrChar2;
			if (refreshCharacters)
			{
				Transform transform = scrChar2.transform;
				if (flag2)
				{
					bool flag3 = levelIndex == GetCharacterIndexFromLevel(Level.HelpingHands);
					if ((array.Length < 4) | flag3)
					{
						float num2 = 16f;
						if (flag3)
						{
							num2 = 34f;
						}
						float f = MathF.PI * 2f / (float)array.Length * (float)j;
						float num3 = Mathf.Cos(f) * num2;
						float f2 = Mathf.Sin(f) * num2 * 0.6f;
						Vector2 vector = new Vector3(num3, f2);
						transform.position = selectableCharacter.gamePosition + vector;
						scrChar2.shaderRenderer.sortingOrder = -Mathf.RoundToInt(f2);
					}
					else
					{
						Vector2 vector2 = new Vector2(11f, 4f);
						Vector2 vector3 = vector2 * j - vector2 * array.Length / 2f;
						transform.position = selectableCharacter.gamePosition + vector3;
						float num4 = (float)j * (0.5f / (float)array.Length);
						scrChar2.sprite.color = Color.white - new Color(num4, num4, num4, 0f);
						scrChar2.sprite.SortingOrder = -j;
					}
				}
				else
				{
					transform.position = selectableCharacter.gamePosition;
				}
			}
			CharacterExpressionGroup characterExpressionGroup = GC.characterExpressions[character];
			scrChar2.neutralAnimName = "neutral";
			Level level = selectableCharacter.levels[currentDifficulty];
			switch (level)
			{
			case Level.Unreachable:
				characterExpressionGroup = new CharacterExpressionGroup("barely", "barely", "jammin", "jammin");
				break;
			case Level.Murmurs:
				characterExpressionGroup = new CharacterExpressionGroup("guitar", "guitar", "guitar", "guitar");
				break;
			case Level.FreezeshotH:
				characterExpressionGroup = new CharacterExpressionGroup("cast_neutral", "cast_neutral", "cast_neutral", "cast_neutral");
				break;
			case Level.AthleteTherapy:
				foreach (WalkingSelectableCharacter walkingCharacter in walkingCharacters)
				{
					if (walkingCharacter.character == Character.Miner && !Level.Freezeshot.Passed())
					{
						walkingCharacter.transform.position = selectableCharacter.gamePosition;
					}
				}
				break;
			}
			if (level == Level.StevensonsTango && Level.StevensonsTango.GetRank().perfected)
			{
				scrChar2.neutralAnimName = characterExpressionGroup.expressionPassed;
			}
			string expression = characterExpressionGroup.GetExpression(levelState, levelRank);
			if (refreshCharacters)
			{
				scrChar2.Setup(character, expression);
			}
			else if (flag && !scrChar2.IsCurrentExpression(expression))
			{
				scrChar2.PlayExpression(expression);
			}
		}
		ApplyShader(selectableCharacter.animation);
		if (flag2)
		{
			scrChar[] childAnimations = selectableCharacter.childAnimations;
			for (int i = 0; i < childAnimations.Length; i++)
			{
				ApplyShader(childAnimations[i]);
			}
		}
		void ApplyShader(scrChar lvlChar)
		{
			RDShaderProperties sd = lvlChar.shaderData;
			if (isLevelUnlock && (bool)lvlChar)
			{
				DOTween.To(() => sd.overlayColor, (Color x) =>
				{
					sd.overlayColor = x;
				}, shader.overlayColor, 1f);
				DOTween.To(() => sd.borderColor, (Color x) =>
				{
					sd.borderColor = x;
				}, shader.borderColor, 1f);
				DOTween.To(() => sd.opacity, (float x) =>
				{
					sd.opacity = x;
				}, shader.opacity, 1f);
			}
			else
			{
				lvlChar.shaderData = shader;
			}
		}
	}

	private void MoveRin(int mode, float duration)
	{
		if (!rinInit)
		{
			RinInit();
		}
		float num = 0f;
		float destX = 0f;
		float num2 = 0f;
		switch (mode)
		{
		case 0:
			num = rinStartX;
			destX = rinStartX + 59f;
			num2 = rinStartY;
			break;
		case 1:
			num = rinDoorX;
			destX = rinLevelX;
			num2 = rinLevelY;
			break;
		case 2:
			num = rinLevelX;
			destX = rinDoorX;
			num2 = rinLevelY;
			break;
		case 3:
			num = rinStartX + 59f;
			destX = rinStartX;
			num2 = rinStartY;
			break;
		}
		if (rinMoveTween != null)
		{
			rinMoveTween.Kill();
		}
		int num3;
		if (mode != 1)
		{
			num3 = ((mode == 3) ? 1 : 0);
			if (num3 == 0 && !rinStartingFromExit)
			{
				goto IL_0106;
			}
		}
		else
		{
			num3 = 1;
		}
		museDashEntranceRin.transform.MoveX(num);
		rinStartingFromExit = false;
		goto IL_0106;
		IL_0106:
		museDashEntranceRin.visible = true;
		museDashEntranceRin.transform.ScaleX((destX > num) ? 1f : (-1f));
		museDashEntranceRin.transform.MoveY(num2);
		rinMoveTween = museDashEntranceRin.transform.DOMoveX(destX, duration).SetEase(Ease.OutSine);
		if (num3 == 0)
		{
			museDashEntranceRin.PlayExpression("runNoShadow");
			museDashLevelRin.gameObject.SetActive(value: false);
			return;
		}
		rinMoveTween.OnUpdate(() =>
		{
			if (Mathf.Abs(museDashEntranceRin.transform.position.x - destX) <= 14f)
			{
				museDashEntranceRin.transform.ScaleX(1f);
				museDashEntranceRin.PlayExpression("neutral");
			}
		});
	}

	private void RinInit()
	{
		museDashEntranceRin.Setup(Character.Rin);
		rinStartX = museDashEntranceRin.transform.position.x;
		rinStartY = museDashEntranceRin.transform.position.y;
		rinDoorX = FindSelectableEntity("ExitMuseDashRoom").gamePosition.x;
		Vector2 gamePosition = FindSelectableEntity("MD-1").gamePosition;
		rinLevelX = gamePosition.x;
		rinLevelY = gamePosition.y;
		rinStartingFromExit = justPlayedLevel == Level.BlackestLuxuryCar || justPlayedLevel == Level.TapeStopNight || justPlayedLevel == Level.The90sDecision;
		rinInit = true;
	}

	private void ShowRanksText(int index)
	{
		string empty = string.Empty;
		string text = "";
		string text2 = ((currentDifficulty == Difficulty.Normal) ? "day" : "night");
		if (selectableEntities[index] is SelectableCharacter)
		{
			SelectableCharacter selectableCharacter = selectableEntities[index] as SelectableCharacter;
			Level level = selectableCharacter.levels[currentDifficulty];
			Difficulty difficulty = currentDifficulty;
			Rank levelRank = Persistence.GetLevelRank(GetLevelWithIndexAndDifficulty(index, difficulty));
			SetDifficultyArrowsVisible(visible: true);
			if ((int)levelRank == -2)
			{
				string text3 = RDString.Get("levelSelect.locked");
				empty = RDString.Get("narration.lockedLevel").Replace("[locked]", text3);
				if (selectableCharacter.isBonus)
				{
					text = text + "<color=#F652A0>" + RDString.Get("levelSelect.bonus") + "</color>";
					empty += RDString.Get("narration.bonus");
				}
				else if (selectableCharacter.isCollab)
				{
					text = text + "<color=#F652A0>" + RDString.Get("levelSelect.collab.long") + "</color>";
					empty += RDString.Get("narration.collab");
				}
				text += "\n\n";
				text += text3;
				string withCheck = RDString.GetWithCheck($"levelSelect.condition.{level}", out var exists);
				if (exists)
				{
					text = text + "\n<color=#6AF2F0>" + withCheck + "</color>";
				}
				else if (currentDifficulty == Difficulty.Hard)
				{
					text = text + "\n<color=#6AF2F0>" + RDString.Get("levelSelect.nightShiftCondition") + "</color>";
				}
				empty = empty.Replace("[levelId]", string.Empty).Replace("[levelName]", string.Empty);
			}
			else
			{
				string text4 = "<color=#CCFF22>" + GC.levelMetadata[level].id + "</color>";
				string text5 = RDString.Get("levelSelect." + level);
				string text6 = text4 + " " + text5;
				if (selectableCharacter.isBonus)
				{
					text = text + "<color=#F652A0>" + RDString.Get("levelSelect.bonus") + "</color>";
					empty += RDString.Get("narration.bonus");
				}
				else if (selectableCharacter.isIntermission && level != Level.StevensonsTango)
				{
					text = text + "<color=#F652A0>" + RDString.Get("levelSelect.intermission") + "</color>";
					empty += RDString.Get("narration.intermission");
				}
				else if (selectableCharacter.isCollab)
				{
					text = text + "<color=#F652A0>" + RDString.Get("levelSelect.collab.long") + "</color>";
					empty += RDString.Get("narration.collab");
				}
				else if (selectableCharacter.isBossLevel)
				{
					text = text + "<color=#F652A0>" + RDString.Get("levelSelect.boss").Replace("[tier]", "").Trim() + "</color>";
					empty += RDString.Get("narration.boss");
				}
				else
				{
					empty += RDString.Get("narration.level");
				}
				string text7 = string.Join(" ", (IEnumerable<char>)GC.levelMetadata[level].id);
				text7 = text7.Replace("-", RDString.Get("narration.level.dash"));
				empty = empty.Replace("[levelId]", text7);
				empty = empty.Replace("[levelName]", text5.Replace('\n', ' '));
				text += "\n\n";
				text = text + "<color=#6AF2F0>" + text6 + "</color>";
				switch (Persistence.GetStateFromLevel(selectableCharacter.levels[difficulty]))
				{
				case LevelState.NotPassed:
					ChangeTextOutline(base.gc.levelSelect_notPassedLevelTextOutline);
					ChangeArrowsOutline(base.gc.levelSelect_notPassedLevelTextOutline);
					break;
				case LevelState.NotAvailable:
					ChangeTextOutline(base.gc.levelSelect_lockedLevelTextOutline);
					ChangeArrowsOutline(base.gc.levelSelect_lockedLevelTextOutline);
					break;
				case LevelState.Passed:
					if (levelRank.perfected || selectableCharacter.isBossLevel)
					{
						ChangeTextOutline(base.gc.levelSelect_perfectLevelTextOutline);
						ChangeArrowsOutline(base.gc.levelSelect_perfectLevelTextOutline);
					}
					else
					{
						ChangeTextOutline(base.gc.levelSelect_passedLevelTextOutline);
						ChangeArrowsOutline(base.gc.levelSelect_passedLevelTextOutline);
					}
					break;
				}
				text += "\n";
				if ((int)levelRank == -1)
				{
					string text8 = ((selectableCharacter.isBossLevel || selectableCharacter.isBonus) ? "incomplete" : "unplayed");
					text += RDString.Get("levelSelect." + text8);
				}
				else if ((int)levelRank != -2)
				{
					if (selectableCharacter.isBossLevel)
					{
						text = (levelRank.perfected ? (text + "<color=#FFD300>" + RDString.Get("levelSelect.perfect") + "</color>") : ((!levelRank.noCheckpoints) ? (text + RDString.Get("levelSelect.completed")) : (text + RDString.Get("levelSelect.completedNoCheckpoints"))));
					}
					else
					{
						string text9 = (levelRank.perfected ? "#FFD300" : (levelRank.passed ? "white" : "#CCCCCC"));
						text += RDString.Get("levelSelect.rank").Replace("[rank]", "<color=" + text9 + ">" + levelRank.ToString() + "</color>");
						if (!levelRank.passed && !selectableCharacter.isBonus)
						{
							text = text + "\n<color=#F652A0>" + RDString.Get("levelSelect.passCondition") + "</color>";
						}
					}
				}
			}
			if (level == Level.Lesmis && halloweenMode)
			{
				if (UnityEngine.Random.Range(0, 4) == 3)
				{
					mainWard.halloweenShadow.Play("Shadow2");
				}
				else
				{
					mainWard.halloweenShadow.Play("Shadow1");
				}
			}
		}
		else
		{
			SelectableObject selectableObject = selectableEntities[index] as SelectableObject;
			bool difficultyArrowsVisible = false;
			if (selectableObject.id.StartsWith("TalkToJanitor"))
			{
				string text10 = RDString.Get("levelSelect.TalkToJanitor");
				text = text10;
				empty += RDString.Get("narration.special").Replace("[name]", text10);
			}
			else if (selectableObject.id == "PaigeChair" || selectableObject.id == "VendingMachinePaige" || selectableObject.id == "ArtCanvas" || selectableObject.id.StartsWith("VoidItem"))
			{
				string text11 = RDString.Get("levelSelect." + selectableObject.id);
				text = text11;
				empty = ((!(selectableObject.id == "VendingMachinePaige")) ? RDString.Get("narration.special").Replace("[name]", text11) : (empty + RDString.Get("narration.storyEvent").Replace("[name]", text11)));
			}
			else if (selectableObject.id == "BasementComputer")
			{
				int num = ((!Persistence.GetIanDesktopLogin()) ? 1 : 2);
				selectedVerticalDestinations = new List<LevelSelectDestination>
				{
					new LevelSelectDestination("sleevePaint", RDString.Get("levelSelect.SleevePaint"), RDString.Get("levelSelect.SleevePaint.details")),
					new LevelSelectDestination("IanDesktop", RDString.Get("levelSelect.IanDesktop"), RDString.Get($"levelSelect.IanDesktop.details.{num}"))
				};
				if (cameFromBasementComputer)
				{
					cameFromBasementComputer = false;
					selectedVerticalIndex = ((scnBase.lastSceneName == "scnIanDesktop") ? 1 : 0);
				}
				LevelSelectDestination levelSelectDestination = selectedVerticalDestinations[selectedVerticalIndex];
				text = levelSelectDestination.title;
				text = text + "\n<color=#6AF2F0>" + levelSelectDestination.subtitle + "</color>";
				difficultyArrowsVisible = selectedVerticalDestinations.Count > 1;
				sleveePaintSprite.SetActive(selectedVerticalIndex == 0);
				empty = RDString.Get("narration.special").Replace("[name]", levelSelectDestination.title) + RDString.Get("narration.shortPause") + levelSelectDestination.subtitle + RDString.Get("narration.shortPause") + RDString.Get("narration.unavailable");
			}
			else if (selectableObject.id == "MainElevator")
			{
				selectedVerticalDestinations = new List<LevelSelectDestination>
				{
					new LevelSelectDestination("ExitRecordsRoom", RDString.Get("levelSelect.act6"), RDString.Get("levelSelect.GoToRecordsRoom.day"))
				};
				if (Level.Bitterness.Passed())
				{
					selectedVerticalDestinations.Add(new LevelSelectDestination("ExitVoid", RDString.Get("levelSelect.finale"), RDString.Get("levelSelect.GoToVoid.day")));
				}
				LevelSelectDestination levelSelectDestination2 = selectedVerticalDestinations[selectedVerticalIndex];
				string text12 = RDString.Get("levelSelect.floorSeparatorString");
				text = string.Empty;
				for (int i = 0; i < selectedVerticalDestinations.Count; i++)
				{
					string title = selectedVerticalDestinations[i].title;
					text = text + "<color=" + ((selectedVerticalIndex == i) ? "#CCFF22" : "#6c802a") + ">" + title + "</color>";
					text = ((i == selectedVerticalDestinations.Count - 1) ? (text + "\n\n") : (text + "<color=#CCFF22>" + text12 + "</color>"));
				}
				text += levelSelectDestination2.subtitle;
				difficultyArrowsVisible = selectedVerticalDestinations.Count > 1;
				empty = string.Concat(RDString.Get("narration.location").Replace("[name]", levelSelectDestination2.subtitle).Replace("[actNumber]", levelSelectDestination2.title));
			}
			else
			{
				string text13 = selectableObject.id switch
				{
					"GoToSVTWard" => RDString.Get("levelSelect.act2"), 
					"GoToTrain" => RDString.Get("levelSelect.act4"), 
					"GoToAthleteWard" => RDString.Get("levelSelect.act5"), 
					"GoToBasement" => RDString.Get("levelSelect.extras"), 
					_ => null, 
				};
				string text14 = RDString.Get("levelSelect." + selectableObject.id + "." + text2);
				text = "\n" + text14;
				string text15 = RDString.Get("narration.location").Replace("[name]", text14);
				if (text13 != null)
				{
					text = "<color=#CCFF22>" + text13 + "</color>\n" + text;
					text15 = text15.Replace("[actNumber]", text13);
				}
				else
				{
					text15 = text15.Replace("[actNumber] ", string.Empty);
				}
				empty += text15;
			}
			ChangeTextOutline(base.gc.levelSelect_notPassedLevelTextOutline);
			SetDifficultyArrowsVisible(difficultyArrowsVisible);
		}
		description.text = text;
		if (firstSpeech.IsNullOrEmpty())
		{
			firstSpeech = empty;
			return;
		}
		SayCurrentEntity(empty);
		saidFirstSpeech = true;
	}

	private void CheckForCutscene()
	{
		SelectableCharacter selectableCharacter = null;
		if (selectedEntity is SelectableCharacter)
		{
			selectableCharacter = selectedEntity as SelectableCharacter;
		}
		bool flag = Persistence.GetGameResult() == GameResult.LevelJustPassed && selectableCharacter.levels[Difficulty.Hard] == Level.InsomniacHard;
		if (Persistence.GetGameResult() == GameResult.LevelJustPassed)
		{
			_ = selectableCharacter.levels[Difficulty.Normal] == Level.AthleteFinale;
		}
		else
			_ = 0;
		bool num = !Persistence.GetPlayedPreAct5Cutscene() && Persistence.GetLevelRank(Level.InsomniacHard).passed && Persistence.GetStateFromLevel(Level.LuckyBreak) == LevelState.NotAvailable && !flag;
		bool flag2 = !Persistence.GetPlayedPassedLevelCutscene() && !Persistence.GetPaigeEnding() && Persistence.GetLevelRank(Level.PaigesReckoning).passed && Persistence.GetStateFromLevel(Level.Blurred) == LevelState.NotAvailable;
		bool flag3 = Persistence.GetPlayedPassedLevelCutscene() && Persistence.GetLevelRank(Level.PaigesReckoning).passed && Persistence.GetStateFromLevel(Level.Blurred) == LevelState.NotAvailable;
		bool flag4 = Persistence.GetPlayedRooftopCutscene() && !Persistence.GetPlayedPassedLevelCutscene() && Persistence.GetLevelRank(Level.Bitterness).passed && Persistence.GetStateFromLevel(Level.Montage) == LevelState.NotAvailable;
		bool flag5 = Persistence.GetPlayedPreAct6Cutscene() && !Persistence.GetPlayedAct6Intro();
		if (num)
		{
			PlayAct5Intro(instant: true);
			return;
		}
		if (flag5)
		{
			PlayAct6Intro();
			return;
		}
		if (flag2)
		{
			Persistence.SetPlayedPassedLevelCutscene(played: true);
			Persistence.SetGameResult(GameResult.LevelAlreadyWasPassed);
			scnBase.GoToCutscene("cutscene_airport");
			return;
		}
		if (flag3)
		{
			PlayAct7Intro();
			return;
		}
		if (flag4)
		{
			Persistence.SetPlayedPassedLevelCutscene(played: true);
			PlayFinaleIntro();
			return;
		}
		controlsAreLocked = true;
		if (justPlayedLevel == Level.None || selectableCharacter == null)
		{
			controlsAreLocked = false;
			return;
		}
		if (justPlayedLevel.GetRank().passed)
		{
			bool flag6 = false;
			if (Persistence.GetGameResult() == GameResult.LevelJustPassed)
			{
				printe("Should go through sequence because level was just passed");
				flag6 = true;
			}
			else
			{
				foreach (Level unlock in selectableCharacter.unlocks)
				{
					if (unlock != selectableCharacter.levels[Difficulty.Hard] && unlock.Locked())
					{
						printe($"Should go through sequence because {unlock} is locked");
						flag6 = true;
						break;
					}
				}
			}
			if (flag6)
			{
				if (justPlayedLevel == Level.Lesmis)
				{
					dontMovePaigeYet = true;
				}
				printe("shouldGoThroughSequence == true");
				StartCoroutine(LevelJustPassedSequenceCo(selectableCharacter));
			}
			else
			{
				controlsAreLocked = false;
			}
		}
		else if (!playerQuitLevel)
		{
			string level = justPlayedLevel.ToString();
			int num2 = Persistence.GetTriesToClearLevel(level) + 1;
			char c = selectableCharacter.gameObject.name[0];
			bool flag7 = c == '1' && c == '2';
			if ((!selectableCharacter.isBonus & flag7) && num2 > 1 && num2 < 6)
			{
				SetUIVisible(visible: false);
				Persistence.SetTriesToClearLevel(level, num2);
				RDBase.Vfx.SpotlightFocusIn(selectableCharacter.gamePosition.x, animated: false, brightMode: false, playSound: false, 1f, 0.5f, RDUtils.GetMixerGroup("StoryLevelSelect"));
				dialogueIsRunning = true;
				base.ink.Run("Intro1", () =>
				{
					RDBase.Vfx.SpotlightOpenAllTheWay(1f, playSwitchSound: true, RDUtils.GetMixerGroup("StoryLevelSelect"));
					SetUIVisible(visible: true);
					dialogueIsRunning = false;
					controlsAreLocked = false;
				}, GetStoryVariables());
			}
			else
			{
				dialogueIsRunning = false;
				controlsAreLocked = false;
			}
		}
		else
		{
			dialogueIsRunning = false;
			controlsAreLocked = false;
		}
		Persistence.SetGameResult(GameResult.LevelAlreadyWasPassed);
	}

	private void GoToCredits()
	{
		scnBase.GoToMainMenu();
	}

	private void UnlockNextNormalLevels(SelectableCharacter character)
	{
		foreach (Level unlock in character.unlocks)
		{
			if (unlock != character.levels[Difficulty.Hard] && unlock.Locked())
			{
				Persistence.SetLevelRank(unlock, -1, force: true);
			}
		}
	}

	private void UnlockNightShiftLevel(SelectableCharacter character)
	{
		if (character == null)
		{
			RDBaseDllDummy.printem("weird stuff going on, character is null");
		}
		else if (character.levels == null)
		{
			RDBaseDllDummy.printem("weirder stuff going on, character.levels is null");
		}
		else if (character.levels.ContainsKey(Difficulty.Hard) && !character.isBossLevel && Persistence.GetLevelRank(justPlayedLevel).unlocksNightShift)
		{
			Level level = character.levels[Difficulty.Hard];
			if (level.Locked())
			{
				printe("unlocked night shift: " + level);
				Persistence.SetLevelRank(level, -1);
			}
		}
	}

	private IEnumerator LevelJustPassedSequenceCo(SelectableCharacter character)
	{
		Level level = justPlayedLevel;
		RDBaseDllDummy.printem(level);
		switch (level)
		{
		case Level.Intro:
			yield return IntroTransitionCo();
			break;
		case Level.OrientalInsomniac:
			UnlockEntrance(FindSelectableEntity("GoToSVTWard"));
			yield return SinglePlaceTransitionCo(level, isThereAnyDialogue: true, spotlightFocus: true);
			UnlockNextNormalLevels(character);
			break;
		case Level.DistantDuet:
			selectableEntities[osmIndex].normalEnabled = true;
			yield return PlaceToPlaceTransitionCo(level);
			break;
		case Level.Boss2:
			yield return SinglePlaceTransitionCo(level, isThereAnyDialogue: true, spotlightFocus: true);
			UnlockNextNormalLevels(character);
			break;
		case Level.Lesmis:
			yield return SinglePlaceTransitionCo(level, isThereAnyDialogue: true);
			UnlockNextNormalLevels(character);
			break;
		case Level.InsomniacHard:
			UnlockEntrance(FindSelectableEntity("GoToAthleteWard"));
			yield return SinglePlaceTransitionCo(level, isThereAnyDialogue: true);
			Persistence.SetLevelRank(Level.LuckyBreak, -1);
			break;
		case Level.AthleteFinale:
			UnlockEntrance(FindSelectableEntity("MainElevator"));
			yield return SinglePlaceTransitionCo(level, isThereAnyDialogue: true);
			UnlockNextNormalLevels(character);
			break;
		case Level.Blurred:
			yield return SinglePlaceTransitionCo(level, isThereAnyDialogue: true);
			UnlockNextNormalLevels(character);
			break;
		case Level.Montage2:
			FindSelectableEntity("7-X2").gameObject.SetActive(value: false);
			montage2Cole.SetActive(value: false);
			UnlockEntrance(FindSelectableEntity("GoToArtRoom"));
			Persistence.SetLastPlayedLevel(Level.HelpingHands);
			yield return SinglePlaceTransitionCo(level, isThereAnyDialogue: true);
			controlsAreLocked = true;
			yield return new WaitForSecondsRealtime(0.5f);
			RDBase.Vfx.FadeOut();
			yield return new WaitForSecondsRealtime(0.5f);
			scnBase.GoToCutscene("cutscene_edega");
			break;
		default:
			if (currentDifficulty == Difficulty.Normal)
			{
				yield return PlaceToPlaceTransitionCo(level);
			}
			else
			{
				yield return SinglePlaceTransitionCo(level, isThereAnyDialogue: true);
			}
			break;
		}
	}

	private SelectableEntity FindSelectableEntity(string name)
	{
		foreach (SelectableEntity selectableEntity in selectableEntities)
		{
			if (selectableEntity.id == name)
			{
				return selectableEntity;
			}
		}
		Debug.LogWarning("SelectableEntity not found: " + name);
		return null;
	}

	private IEnumerator IntroTransitionCo()
	{
		SetUIVisible(visible: false);
		SelectableCharacter character = selectedEntity as SelectableCharacter;
		RDBase.Vfx.SpotlightFocusIn(character.gamePosition.x, animated: false, brightMode: false, playSound: false, 1f, 0.5f, RDUtils.GetMixerGroup("StoryLevelSelect"));
		dialogueIsRunning = true;
		yield return base.ink.RunAndWait("diaLevelSelect", "OpeningCredsIntro", null, GetStoryVariables());
		Persistence.SetLevelRank(Level.OrientalTechno, -1);
		RDBase.Vfx.SpotlightFocusFromCenter(character.gamePosition.x, 0.5f, playSwitchSound: true, RDUtils.GetMixerGroup("StoryLevelSelect"));
		UpdateCharacters(isLevelUnlock: true);
		RDBase.Vfx.SpotlightOpenAllTheWay(1f, playSwitchSound: true, RDUtils.GetMixerGroup("StoryLevelSelect"));
		SetUIVisible(visible: true);
		dialogueIsRunning = false;
		controlsAreLocked = false;
		UpdateUI();
	}

	private IEnumerator PlaceToPlaceTransitionCo(Level justPassedLevel)
	{
		RDBaseDllDummy.printem(justPassedLevel.ToString());
		SelectableCharacter selectableCharacter = selectedEntity as SelectableCharacter;
		string text = "Level_" + justPassedLevel.ToString() + "_Passed";
		RDBase.Vfx.SpotlightInstantOff();
		SetUIVisible(visible: false);
		RDBase.Vfx.SpotlightFocusFromCenter(selectableCharacter.animation.transform.position.x, 0.5f, playSwitchSound: false, RDUtils.GetMixerGroup("StoryLevelSelect"));
		dialogueIsRunning = true;
		RDBaseDllDummy.printem("Transition: Place to place");
		string cutsceneName = justPassedLevel.ToString() + "_passed";
		bool cutsceneExists = Resources.Load<TextAsset>(scnBase.PathForCutscene(cutsceneName)) != null;
		bool cutscenePlayed = Persistence.GetPlayedPassedLevelCutscene();
		if (!cutscenePlayed)
		{
			printe("trying to play: " + text);
			yield return base.ink.RunAndWait("diaLevelSelect", text, null, GetStoryVariables());
		}
		printe("do we get here: " + cutsceneName);
		if (cutsceneExists && !cutscenePlayed)
		{
			printe("here");
			controlsAreLocked = true;
			StartFadeOut().OnComplete(() =>
			{
				Persistence.SetPlayedPassedLevelCutscene(played: true);
				scnBase.GoToCutscene(cutsceneName);
			});
		}
		else
		{
			yield return new WaitForSeconds(0.5f);
			yield return PlaceToPlaceTransitionPart2Co(justPassedLevel);
		}
	}

	private IEnumerator PlaceToPlaceTransitionPart2Co(Level justPassedLevel)
	{
		RDBaseDllDummy.printem(justPassedLevel.ToString());
		SelectableCharacter selectableCharacter = selectedEntity as SelectableCharacter;
		List<Level> unlocks = selectableCharacter.unlocks;
		if (unlocks.Count > 0)
		{
			string newNode = "Level_" + unlocks[0].ToString() + "_New";
			int characterIndexFromLevel = GetCharacterIndexFromLevel(selectableCharacter.unlocks[0]);
			SelectableCharacter newCharacter = selectableEntities[characterIndexFromLevel] as SelectableCharacter;
			Persistence.SetPlayedPassedLevelCutscene(played: false);
			bool skipFocus = false;
			foreach (Level item in unlocks)
			{
				if (item != selectableCharacter.levels[Difficulty.Hard] && item.Locked())
				{
					Persistence.SetLevelRank(item, -1);
					if (item == Level.InsomniacHard)
					{
						skipFocus = true;
					}
				}
			}
			if (!skipFocus)
			{
				RDBase.Vfx.SpotlightInstantOff(darkenScene: true, playSwitchSound: true);
				MoveCamera(characterIndexFromLevel);
			}
			UpdateCharacters(isLevelUnlock: true);
			yield return new WaitForSeconds(1f);
			if (!skipFocus)
			{
				RDBase.Vfx.SpotlightFocusFromCenter(newCharacter.animation.transform.position.x, 0.5f, playSwitchSound: true, RDUtils.GetMixerGroup("StoryLevelSelect"));
			}
			UpdateCharacters(isLevelUnlock: true);
			if (!skipFocus)
			{
				yield return new WaitForSeconds(1f);
				yield return base.ink.RunAndWait("diaLevelSelect", newNode, null, GetStoryVariables());
			}
		}
		RDBase.Vfx.SpotlightOpenAllTheWay(1f, playSwitchSound: true, RDUtils.GetMixerGroup("StoryLevelSelect"));
		SetUIVisible(visible: true);
		UpdateUI();
		yield return new WaitForSeconds(0.5f);
		controlsAreLocked = false;
		dialogueIsRunning = false;
	}

	private IEnumerator SinglePlaceTransitionCo(Level levelPassed, bool isThereAnyDialogue, bool spotlightFocus = false)
	{
		RDBaseDllDummy.printem($"{levelPassed}, {isThereAnyDialogue}, {spotlightFocus}");
		if (isThereAnyDialogue)
		{
			RDBase.Vfx.SpotlightInstantOff();
			if (spotlightFocus)
			{
				SelectableCharacter selectableCharacter = selectedEntity as SelectableCharacter;
				RDBase.Vfx.SpotlightFocusFromCenter(selectableCharacter.animation.transform.position.x, 0.5f, playSwitchSound: false, RDUtils.GetMixerGroup("StoryLevelSelect"));
			}
			string knotName = "Level_" + levelPassed.ToString() + "_Passed";
			SetUIVisible(visible: false);
			yield return base.ink.RunAndWait("diaLevelSelect", knotName, null, GetStoryVariables());
			RDBase.Vfx.matrixMode = true;
			RDBase.Vfx.matrixModeColor = Color.clear;
			if (spotlightFocus)
			{
				RDBase.Vfx.SpotlightOpenAllTheWay(1f, playSwitchSound: true, RDUtils.GetMixerGroup("StoryLevelSelect"));
			}
			if (levelPassed == Level.Lesmis || levelPassed == Level.InsomniacHard || levelPassed == Level.Montage)
			{
				controlsAreLocked = true;
				yield break;
			}
			yield return new WaitForSeconds(0.5f);
			SetUIVisible(visible: true);
			controlsAreLocked = false;
			UpdateUI();
			if (levelPassed == Level.InsomniacHard || levelPassed == Level.Montage2)
			{
				SetUIVisible(visible: false);
			}
		}
		else
		{
			controlsAreLocked = false;
		}
	}

	private void SelectCharacter(SelectableCharacter selectable, Difficulty difficulty)
	{
		controlsAreLocked = true;
		LevelSelectPlaySound("sndPagerOpen");
		bool skipCamAnimOnSelect = selectable.skipCamAnimOnSelect;
		bool leftPager = selectable.leftPager;
		float num = (skipCamAnimOnSelect ? selectable.cameraOffset.x : (90f * (float)((!leftPager) ? 1 : (-1))));
		float num2 = wrldCamera.transform.position.x - num;
		if (!skipCamAnimOnSelect)
		{
			float num3 = selectable.gamePosition.x;
			float num4 = wrldCamera.transform.localPosition.x;
			float endValue = num3 - num4 + num;
			cameraObject.transform.DOKill();
			cameraObject.transform.DOLocalMoveX(endValue, 0.5f).SetEase(Ease.OutQuad);
		}
		RDBase.Vfx.SpotlightFocusIn(num2, animated: true, brightMode: false, playSound: false, 0.4f, 0.5f, RDUtils.GetMixerGroup("StoryLevelSelect"));
		arrows.gameObject.SetActive(value: false);
		description.enabled = false;
		ShowPatientQuote(show: true, selectable, difficulty);
		Level level = selectable.levels[currentDifficulty];
		LevelPlayMode levelPlayMode = ((level == Level.AthleteTherapy) ? LevelPlayMode.OnePlayerOnly : ((level == Level.RhythmWeightlifter && scnBase.isDesktop) ? LevelPlayMode.TwoPlayerOnly : LevelPlayMode.BothModes));
		heartMonitor.Show(selectable, HeartMonitor.LevelSpeed.Normal, currentDifficulty, levelPlayMode);
		KeyInfoPanel.instance.UpdateShortcutsPanel(forcehide: true);
	}

	private void ShowPatientQuote(bool show, SelectableCharacter selChar = null, Difficulty difficulty = Difficulty.Normal)
	{
		isPatientQuoteAnimating = false;
		show = (selChar != null) & show;
		Level level = Level.None;
		if (selChar != null)
		{
			level = selChar.levels[Difficulty.Normal];
		}
		bool flag = level == Level.Unbeatable;
		bool flag2 = level == Level.BlackestLuxuryCar || level == Level.TapeStopNight || level == Level.The90sDecision;
		bool flag3 = level == Level.MeetAndTweet;
		bool flag4 = level == Level.VividStasis;
		bool flag5 = level == Level.SparkLine;
		bool flag6 = level == Level.Beatblock;
		if (level == Level.Lesmis && paigeIsAwayFromHospital)
		{
			return;
		}
		if (selChar != null)
		{
			printe("ShowPatientQuote " + selChar.levels[Difficulty.Normal]);
		}
		if (!flag)
		{
			patientQuote.enabled = show;
		}
		if (quoteSeq != null)
		{
			quoteSeq.Kill();
		}
		unbeatableButton.gameObject.SetActive(show & flag);
		museDashButton.gameObject.SetActive(show & flag2);
		bitsAndBopsButton.gameObject.SetActive(show & flag3);
		vividStasisButton.gameObject.SetActive(show & flag4);
		sparklineButton.gameObject.SetActive(show & flag5);
		beatblockButton.gameObject.SetActive(show & flag6);
		patientQuote.rectTransform.AnchorPosY(flag3 ? (-50f) : (-30f));
		if (flag)
		{
			printe("we are here");
			unbeatableButton.gameObject.SetActive(show);
		}
		else
		{
			if (!show)
			{
				return;
			}
			patientQuote.text = "";
			Level level2 = selChar.levels[difficulty];
			string arg = (Persistence.GetLevelRank(level2).passed ? "passed" : "notPassed");
			string text = RDString.Get($"levelSelect.{level2}.patient.{arg}");
			TextGenerator cachedTextGenerator = patientQuote.cachedTextGenerator;
			cachedTextGenerator.Populate(text, patientQuote.GetGenerationSettings(patientQuote.rectTransform.rect.size));
			string text2 = "";
			for (int i = 0; i < cachedTextGenerator.lineCount; i++)
			{
				bool num = i == cachedTextGenerator.lineCount - 1;
				int startCharIdx = cachedTextGenerator.lines[i].startCharIdx;
				int num2 = (num ? text.Length : cachedTextGenerator.lines[i + 1].startCharIdx);
				text2 += text.Substring(startCharIdx, num2 - startCharIdx);
				if (!num)
				{
					text2 += "\n";
				}
			}
			if (GC.levelMetadata.TryGetValue(level2, out var value))
			{
				Character value2 = ((value.characters.Length > 1 || value.characters[0] == Character.None) ? value.portraits[0] : value.characters[0]);
				speechSource.clip = RDUtils.GetSpeechTickerSoundForCharacter(value2);
				float duration = (float)text2.Length * 0.05f;
				quoteSeq = DOTween.Sequence().AppendInterval(2f).Append(patientQuote.DOText(text2, duration).SetEase(Ease.Linear).OnStart(() =>
				{
					isPatientQuoteAnimating = true;
				})
					.OnComplete(() =>
					{
						isPatientQuoteAnimating = false;
					})
					.OnKill(() =>
					{
						isPatientQuoteAnimating = false;
					}));
			}
		}
	}

	public void GoToLevelSequence(string levelToGo)
	{
		printe("levelToGo: " + levelToGo);
		controlsAreLocked = true;
		float num = (Persistence.GetFastMenu() ? 0.25f : 1f);
		float transitionDuration = (Persistence.GetFastMenu() ? (cutoffAnimDuration / 4f) : cutoffAnimDuration);
		Persistence.SetPlayedPassedLevelCutscene(played: false);
		string sound = ((GC.levelMetadata[LevelSelector.LevelFromString(levelToGo)].type == LevelType.Boss) ? "sndMenuSelectBoss" : "sndMenuSelect");
		AudioSource enterLevelSound = LevelSelectPlaySound(sound);
		Narration.Say(RDString.Get("common.startLevelButton"), NarrationCategory.Navigation, false);
		quoteSeq.Kill();
		patientQuote.DOFade(0f, num).SetUpdate(isIndependentUpdate: true).OnComplete(() =>
		{
			patientQuote.gameObject.SetActive(value: false);
		});
		base.conductor.StopSong(num);
		StopAllAmbiences(num);
		if (!Persistence.GetReducedFlash())
		{
			RDBase.Vfx.Flash();
		}
		Level levelVar = RDUtils.ParseEnum(levelToGo, Level.None);
		bool isBoss = false;
		if (levelVar != Level.None)
		{
			LevelMetadata levelMetadata = GC.levelMetadata[levelVar];
			isBoss = levelMetadata.type == LevelType.Boss;
		}
		if (levelVar == Level.Montage)
		{
			abandonedWard.SetDoorOpened(open: true);
			heartMonitor.Hide();
			RDBase.Vfx.SpotlightOpenAllTheWay(1f, playSwitchSound: true, RDUtils.GetMixerGroup("StoryLevelSelect"));
		}
		if (selectedEntity is SelectableCharacter selectableCharacter)
		{
			scrChar component = selectableCharacter.gameObject.GetComponent<scrChar>();
			if (component.character.ToString().StartsWith("Book"))
			{
				component.PlayExpression("open");
			}
		}
		DOVirtual.DelayedCall(num, () =>
		{
			if (enterLevelSound != null)
			{
				enterLevelSound.DOFade(0f, transitionDuration);
			}
			if (levelVar != Level.None)
			{
				if (levelVar == Level.Montage)
				{
					cameraObject.transform.DOLocalMoveY(10f, transitionDuration).SetRelative(isRelative: true).SetEase(Ease.InSine);
					cameraObject.GetComponentInChildren<RDCamera>().Zoom(5f, transitionDuration, Ease.InSine).SetUpdate(isIndependentUpdate: true);
					RDBase.Vfx.GetRoomOverlay(-1).sprite.DOColor(Color.black.WithAlpha(1f), transitionDuration).From().SetEase(Ease.InQuad)
						.SetUpdate(isIndependentUpdate: true);
				}
				else
				{
					blitTransitionMaterial.SetTexture("_TransitionTex", transitionTexture);
					blitTransitionMaterial.DOFloat(1f, "_Cutoff", transitionDuration);
					blitTransitionMaterial.SetInt("_Distort", isBoss ? 1 : 0);
				}
				if (!scnGame.loadDogMode)
				{
					Persistence.SetLastPlayedLevel(levelToGo);
				}
			}
			if (DebugSettings.instance.RecordingMode)
			{
				GC.levelRecordedIndex = Array.IndexOf(GC.levelsToRecord, levelToGo);
			}
			DOVirtual.DelayedCall(transitionDuration + 0.5f, () =>
			{
				if (levelVar == Level.RhythmWeightlifter)
				{
					scnBase.GoToRhythmWeightlifter();
				}
				else
				{
					bool flag = scnGame.levelSpeed == 1f;
					if (flag && GC.twoPlayerMode && levelVar == Level.VividStasis)
					{
						flag = false;
					}
					scnBase.GoToLevel(levelToGo, loadGameScene: true, flag);
				}
			});
		});
	}

	private bool CheckCNY()
	{
		(int year, int month, int day) dateOfChineseNewYear = RDUtils.GetDateOfChineseNewYear();
		int item = dateOfChineseNewYear.year;
		int item2 = dateOfChineseNewYear.month;
		int item3 = dateOfChineseNewYear.day;
		DateTime dateTime = new DateTime(item, item2, item3);
		DateTime dateTime2 = dateTime.AddDays(15.0);
		_ = DateTime.Now;
		long ticks = DateTime.Now.Ticks;
		long ticks2 = dateTime.Ticks;
		long ticks3 = dateTime2.Ticks;
		if (ticks > ticks2)
		{
			return ticks < ticks3;
		}
		return false;
	}

	private void LoadLevelData()
	{
		Dictionary<string, object> obj = Json.Deserialize(Resources.Load<TextAsset>("levelSequence").text) as Dictionary<string, object>;
		bool flag = Persistence.GetLevelRank(Level.Boss2).passed || Persistence.GetStateFromLevel(Level.Garden) != LevelState.NotAvailable;
		foreach (KeyValuePair<string, object> item in obj)
		{
			Dictionary<string, object> dictionary = item.Value as Dictionary<string, object>;
			SelectableEntity selectableEntity = null;
			if (dictionary.TryGetValueAs("load", out var valueAs, _default: false) && !valueAs)
			{
				continue;
			}
			string text = dictionary["type"] as string;
			if (!(text == "character"))
			{
				if (text == "object")
				{
					SelectableObject selectableObject = new SelectableObject();
					selectableEntity = selectableObject;
					selectableObject.id = item.Key;
					selectableObject.action = dictionary["action"] as string;
					selectableObject.group = Convert.ToInt32(dictionary["group"]);
					selectableObject.normalEnabled = (bool)dictionary["normalEnabled"];
					selectableObject.hardEnabled = (bool)dictionary["hardEnabled"];
					selectableObject.gameObject = GameObject.Find(selectableObject.id);
					if (Enum.TryParse<Level>(selectableObject.id, out var result))
					{
						LevelMetadata levelMetadata = GC.levelMetadata[result];
						selectableObject.levelType = levelMetadata.type;
					}
					if (selectableObject.gameObject == null)
					{
						Debug.LogWarning("Couldn't find " + selectableObject.id);
					}
					Renderer component = selectableObject.gameObject.GetComponent<Renderer>();
					Vector3 vector = ((component == null) ? selectableObject.gameObject.transform.position : component.bounds.center);
					selectableObject.gamePosition = new Vector2(vector.x, vector.y);
					if (selectableObject.id == "GoToSVTWard" && Persistence.GetLevelRank(Level.OrientalInsomniac).passed)
					{
						UnlockEntrance(selectableObject);
					}
					else if (selectableObject.id == "GoToTrain" && Persistence.GetLevelRank(Level.Lesmis).passed && Persistence.GetPlayedPostAct3Cutscene())
					{
						UnlockEntrance(selectableObject);
					}
					else if (selectableObject.id == "MainElevator")
					{
						mainElevatorEntranceEntity = selectableObject;
						mainElevatorEntranceEntityIndex = selectableEntities.Count;
						if (Level.AthleteFinale.Passed())
						{
							UnlockEntrance(selectableObject);
						}
					}
					else if (selectableObject.id == "GoToArtRoom" && Level.Montage2.Passed())
					{
						UnlockEntrance(selectableObject);
					}
					else if (selectableObject.id == "GoToMuseDashRoom" && Persistence.GetLevelRank(Level.Lesmis).passed && Persistence.GetPlayedPostAct3Cutscene())
					{
						UnlockEntrance(selectableObject);
						ActivateEntranceRin();
					}
					else if (selectableObject.id == "ArtCanvas")
					{
						artCanvasEntity = selectableObject;
						artCanvasEntityIndex = selectableEntities.Count;
					}
					else if (selectableObject.id == "GoToAthleteWard")
					{
						if (Persistence.GetLevelRank(Level.InsomniacHard).passed)
						{
							UnlockEntrance(selectableObject);
						}
					}
					else if (selectableObject.id.StartsWith("VoidItem"))
					{
						if (!Level.Montage.Locked())
						{
							selectableObject.normalEnabled = true;
						}
						if (!selectableObject.normalEnabled)
						{
							string id = selectableObject.id;
							int num = int.Parse(id[id.Length - 1].ToString()) - 1;
							abandonedWard.voidItems[num].color = Color.white.WithAlpha(0f);
						}
					}
					selectableEntities.Add(selectableObject);
				}
			}
			else
			{
				SelectableCharacter selectableCharacter = new SelectableCharacter();
				selectableEntity = selectableCharacter;
				selectableCharacter.id = item.Key;
				Dictionary<string, object> dictionary2 = dictionary["levels"] as Dictionary<string, object>;
				Dictionary<Difficulty, Level> dictionary3 = new Dictionary<Difficulty, Level>();
				Difficulty[] array = new Difficulty[3]
				{
					Difficulty.Tutorial,
					Difficulty.Normal,
					Difficulty.Hard
				};
				for (int i = 0; i < array.Length; i++)
				{
					Difficulty key = array[i];
					string key2 = key.ToString();
					dictionary3[key] = (dictionary2.ContainsKey(key2) ? LevelSelector.LevelFromString(dictionary2[key2] as string) : Level.None);
				}
				selectableCharacter.levels = dictionary3;
				Level key3 = selectableCharacter.levels[Difficulty.Normal];
				Level key4 = selectableCharacter.levels[Difficulty.Hard];
				if (GC.levelMetadata.ContainsKey(key3))
				{
					LevelMetadata levelMetadata2 = GC.levelMetadata[key3];
					selectableCharacter.charactersNormal = levelMetadata2.characters;
					selectableCharacter.portraitsNormal = levelMetadata2.portraits;
					selectableCharacter.bpmNormal = (int)levelMetadata2.bpm;
					selectableCharacter.levelType = levelMetadata2.type;
				}
				if (GC.levelMetadata.ContainsKey(key4))
				{
					LevelMetadata levelMetadata3 = GC.levelMetadata[key4];
					selectableCharacter.charactersHard = GC.levelMetadata[key4].characters;
					selectableCharacter.portraitsHard = levelMetadata3.portraits;
					selectableCharacter.bpmHard = (int)GC.levelMetadata[key4].bpm;
				}
				selectableCharacter.hardEnabled = dictionary.ContainsKey("hardEnabled") && (bool)dictionary["hardEnabled"];
				selectableCharacter.skipCamAnimOnSelect = dictionary.ContainsKey("skipCamAnimOnSelect") && (bool)dictionary["skipCamAnimOnSelect"];
				selectableCharacter.leftPager = dictionary.ContainsKey("leftPager") && (bool)dictionary["leftPager"];
				selectableCharacter.group = Convert.ToInt32(dictionary["group"]);
				selectableCharacter.gameObject = GameObject.Find(selectableCharacter.id);
				if (selectableCharacter.gameObject == null)
				{
					Debug.LogWarning("character with id: " + selectableCharacter.id + " not found.");
					continue;
				}
				selectableCharacter.gamePosition = selectableCharacter.gameObject.transform.position;
				selectableCharacter.normalEnabled = (bool)dictionary["normalEnabled"];
				if (item.Key == "1-X")
				{
					selectableCharacter.hardEnabled = Level.KnowYou.Passed();
				}
				else if (item.Key == "2-X")
				{
					selectableCharacter.normalEnabled = Level.SongOfTheSea.Passed();
					selectableCharacter.hardEnabled = Level.Blurred.Passed();
				}
				else if (item.Key == "3-X")
				{
					selectableCharacter.normalEnabled = Level.DistantDuet.Passed();
					osmIndex = selectableEntities.Count;
					if (Persistence.GetLevelRank(Level.Lesmis).passed)
					{
						selectableCharacter.gameObject.SetActive(value: false);
						sleepingPaige.gameObject.SetActive(!paigeIsAwayFromHospital);
						if (paigeIsAwayFromHospital)
						{
							mainWard.ToggleFireplace(enabled: false);
						}
					}
				}
				else if (item.Key == "7-X")
				{
					if (Level.Montage.Passed())
					{
						if (Level.Montage2.Locked())
						{
							Persistence.SetLevelRank(Level.Montage2, -1);
						}
					}
					else if (!Level.Montage2.Locked())
					{
						Persistence.SetLevelRank(Level.Montage2, -2, force: true);
					}
					selectableCharacter.normalEnabled = !Level.Montage.Locked();
				}
				else if (item.Key == "7-X2")
				{
					selectableCharacter.normalEnabled = Level.Montage.Passed();
				}
				else if (item.Key == "1-CNY")
				{
					bool flag2 = (selectableCharacter.normalEnabled = CheckCNY() && Level.OrientalInsomniac.Passed());
					selectableCharacter.gameObject.SetActive(flag2);
					if (flag2)
					{
						Persistence.SetLevelRank(Level.GongXi, -1);
					}
				}
				else if (item.Key == "1-BOO")
				{
					bool flag3 = (selectableCharacter.normalEnabled = RDBase.IsHalloweenWeek() && Level.OrientalInsomniac.Passed());
					selectableCharacter.gameObject.SetActive(flag3);
					if (flag3)
					{
						Persistence.SetLevelRank(Level.Halloween, -1);
					}
				}
				else if (item.Key == "3-B1")
				{
					bool active = (selectableCharacter.normalEnabled = false);
					selectableCharacter.gameObject.SetActive(active);
				}
				else if (item.Key == "X-1")
				{
					bool active2 = (selectableCharacter.normalEnabled = Level.Montage2.Passed());
					selectableCharacter.gameObject.SetActive(active2);
				}
				else if (selectableEntity.group == 3)
				{
					if (item.Key == "X-MAT")
					{
						if (Level.Garden.Passed())
						{
							Persistence.SetLevelRank(Level.MeetAndTweet.ToString(), -1);
						}
					}
					else if (item.Key == "X-KOB")
					{
						if (Level.KnowYou.Passed())
						{
							Persistence.SetLevelRank(Level.SparkLine.ToString(), -1);
						}
					}
					else if (item.Key == "X-WOT")
					{
						if (Level.Heldbeats.Passed())
						{
							Persistence.SetLevelRank(Level.Unbeatable.ToString(), -1);
						}
					}
					else if (item.Key == "X-FTS")
					{
						if (Level.Freezeshot.Passed())
						{
							Persistence.SetLevelRank(Level.VividStasis.ToString(), -1);
						}
					}
					else if (item.Key == "X-PBC" && Level.HaileyDuet.Passed())
					{
						Persistence.SetLevelRank(Level.Beatblock.ToString(), -1);
					}
					selectableCharacter.normalEnabled = Persistence.GetLevelRank(Level.OrientalInsomniac).passed;
				}
				else if (item.Key == "MD-1")
				{
					bool flag4 = (selectableCharacter.normalEnabled = Level.Lesmis.Passed());
					selectableCharacter.gameObject.SetActive(flag4);
					if (Level.BlackestLuxuryCar.Locked() & flag4)
					{
						Persistence.SetLevelRank(Level.BlackestLuxuryCar.ToString(), -1);
					}
				}
				else if (item.Key == "2-4")
				{
					bool flag5 = (int)Level.KnowYou.GetRank().ToNormal() >= 4;
					if (Level.Murmurs.Locked() & flag5)
					{
						Persistence.SetLevelRank(Level.Murmurs.ToString(), -1);
					}
				}
				else if (selectableCharacter.id == "X-0")
				{
					bool active3 = (selectableCharacter.normalEnabled = Persistence.GetLevelRank(Level.AthleteFinale).passed);
					selectableCharacter.gameObject.SetActive(active3);
				}
				else if (selectableCharacter.id == "5-B1")
				{
					bool normalEnabled = Level.AthleteFinale.Passed();
					selectableCharacter.normalEnabled = normalEnabled;
					if (Level.RhythmWeightlifter.Locked())
					{
						Persistence.SetLevelRank(Level.RhythmWeightlifter.ToString(), -1);
					}
				}
				else if (selectableCharacter.group == 8)
				{
					if (!Level.LuckyBreak.Passed())
					{
						selectableCharacter.hardEnabled = false;
						if (selectableCharacter.id == "5-1")
						{
							if (Persistence.GetLevelRank(Level.InsomniacHard).passed && Level.LuckyBreak.Locked())
							{
								selectableCharacter.normalEnabled = true;
							}
						}
						else
						{
							selectableCharacter.normalEnabled = false;
							selectableCharacter.gameObject.SetActive(value: false);
						}
					}
					if (item.Key == "5-3")
					{
						bool flag6 = (int)Level.AthleteTherapy.GetRank().ToNormal() >= 4;
						if (Level.StevensonsTango.Locked() & flag6)
						{
							Persistence.SetLevelRank(Level.StevensonsTango, -1);
						}
					}
				}
				else if (selectableCharacter.group == 10)
				{
					bool normalEnabled2 = Level.HaileyDuet.Passed();
					if (selectableCharacter.id != "6-1")
					{
						selectableCharacter.normalEnabled = normalEnabled2;
					}
					if (selectableCharacter.id == "6-2")
					{
						selectableCharacter.gameObject.SetActive(Level.EdegaRave.Passed());
					}
				}
				else
				{
					selectableCharacter.normalEnabled = (bool)dictionary["normalEnabled"];
				}
				List<Level> list = new List<Level>();
				if (dictionary.ContainsKey("unlocks"))
				{
					foreach (object item2 in dictionary["unlocks"] as List<object>)
					{
						string levelName = item2 as string;
						list.Add(LevelSelector.LevelFromString(levelName));
					}
				}
				selectableCharacter.unlocks = list;
				if (scnBase.isMobile)
				{
					selectableCharacter.cameraOffset = new Vector2(0f, selectableCharacter.cameraOffset.y);
				}
				if (!flag && selectableCharacter.id.StartsWith("3-"))
				{
					selectableCharacter.normalEnabled = false;
					selectableCharacter.hardEnabled = false;
				}
				if ((selectableCharacter.id == "2-B1") & flag)
				{
					selectableCharacter.normalEnabled = true;
				}
				selectableEntities.Add(selectableCharacter);
			}
			if (selectableEntity != null)
			{
				Transform transform = selectableEntity.gameObject.transform.Find("CameraOffset");
				selectableEntity.cameraOffset = (transform ? transform.localPosition : Vector3.zero);
				Transform transform2 = selectableEntity.gameObject.transform.Find("DescriptionOffset");
				selectableEntity.descriptionOffset = (transform2 ? transform2.localPosition : Vector3.zero);
				Transform transform3 = selectableEntity.gameObject.transform.Find("DownArrowOffset");
				selectableEntity.downArrowOffset = (transform3 ? transform3.localPosition : Vector3.zero);
				Transform transform4 = selectableEntity.gameObject.transform.Find("UpArrowOffset");
				selectableEntity.upArrowOffset = (transform4 ? transform4.localPosition : Vector3.zero);
			}
		}
	}

	public void UnlockEntrance(SelectableEntity selectableObject)
	{
		selectableObject.normalEnabled = true;
		selectableObject.hardEnabled = true;
		if (selectableObject.id == "GoToSVTWard")
		{
			mainWard.svtBlockage.SetActive(value: false);
		}
		else if (selectableObject.id == "GoToAthleteWard")
		{
			mainWard.rooftopBlockade.SetActive(value: false);
			if (!Level.CareLess.Passed())
			{
				nicoleAct5Blockage.gameObject.SetActive(value: true);
				nicoleAct5Blockage.shaderDataSource = nicoleAct5Blockage;
				nicoleAct5Blockage.shaderData.borderColor = Color.black;
				nicoleAct5Blockage.shaderData.outline = 1;
			}
		}
		else if (selectableObject.id == "MainElevator")
		{
			mainWard.SetElevatorOpened(open: true, 999f);
		}
	}

	private void PlaceJanitor()
	{
		List<SelectableEntity> list = selectableEntities.FindAll((SelectableEntity s) => s.id.StartsWith("TalkToJanitor"));
		bool flag = justPlayedLevel == Level.Intro;
		bool flag2 = justPlayedLevel == Level.LuckyBreak && !Level.LuckyBreak.Passed();
		if (flag | flag2)
		{
			printe(flag ? "is intro" : "is lucky break but hasn't beaten it yet.");
			{
				foreach (SelectableEntity item in list)
				{
					HideJanitor(item);
				}
				return;
			}
		}
		SelectableEntity selectableEntity = selectableEntities.Find((SelectableEntity s) => s is SelectableCharacter && ((SelectableCharacter)s).levels.Values.Contains(justPlayedLevel));
		if (selectableEntity == null)
		{
			Debug.LogWarning("No janitors available for level " + justPlayedLevel);
			return;
		}
		int num = selectableEntity.group;
		int num2 = 0;
		if (num == 1)
		{
			num2 = ((justPlayedLevel <= Level.InsomniacHard) ? 1 : ((justPlayedLevel > Level.BeansHopper) ? 3 : 2));
		}
		if (num == 3)
		{
			num = 1;
			num2 = 1;
		}
		foreach (SelectableEntity item2 in list)
		{
			int num3 = (int)char.GetNumericValue(item2.id.Last());
			if (doingRobots)
			{
				item2.gameObject.GetComponentInChildren<scrChar>().ChangeCharacter(Character.RhythmJanitorReal);
			}
			if (item2.group != num)
			{
				if (num3 != 4 || num != 5)
				{
					HideJanitor(item2);
				}
			}
			else if (num == 1 && num3 != num2)
			{
				HideJanitor(item2);
			}
		}
	}

	private void HideJanitor(SelectableEntity janitor)
	{
		janitor.normalEnabled = false;
		janitor.hardEnabled = false;
		janitor.gameObject.SetActive(value: false);
	}

	private void TriggerSVTWindowMan()
	{
		if (windowManSequence != null)
		{
			windowManSequence.Kill();
		}
		svtWard.windowMan.Stop();
		if (UnityEngine.Random.Range(0, 5) == 4)
		{
			svtWard.windowMan.Sprite.SetSprite("SVT2_DoctorWithBlinds/0");
			float interval = UnityEngine.Random.Range(5f, 10f);
			windowManSequence = DOTween.Sequence().AppendInterval(interval).AppendCallback(() =>
			{
				svtWard.windowMan.Play("SVT_Doctor");
			});
		}
		else
		{
			svtWard.windowMan.Sprite.SetSprite("SVT2_DoctorWithBlinds/21");
		}
	}

	private void SetDifficultyArrowsVisible(bool visible)
	{
		bool flag = CharacterHasManyLevels();
		bool flag2 = selectedEntity is SelectableObject { id: var id } && (id == "MainElevator" || id == "BasementComputer") && selectedVerticalDestinations.Count > 1;
		arrowsShouldBeVisible = visible && (flag | flag2);
		if (scnBase.isMobile)
		{
			arrows.gameObject.SetActive(value: false);
			MobileNightShiftButton.SetActive(flag);
		}
		else
		{
			arrows.gameObject.SetActive(arrowsShouldBeVisible);
			string[] localizationKeys = (flag ? buttonInstructionsNightShift : (flag2 ? buttonInstructionsVerticalDestination : buttonInstructionsDefault));
			KeyInfoPanel.instance.UpdateShortcutsRowWithStrings(localizationKeys);
		}
	}

	public void SetUIVisible(bool visible)
	{
		SetDifficultyArrowsVisible(visible);
		description.enabled = visible;
	}

	public void StartConductor()
	{
		UpdateMusic(selectedEntity, changedArea: true);
	}

	private void PlaySong(string songName, float bpm, float fadeInDuration = 0f)
	{
		base.conductor.oneSongAtATime = true;
		if (!base.conductor.IsSongPlaying() || !(base.conductor.currentSongString == songName))
		{
			base.conductor.SetBPM(bpm);
			base.conductor.PlaySong(songName, RDUtils.GetMixerGroup("MenuMusic"), 1f, 0f, useBeatsNotBar: false, base.conductor.volume);
		}
		base.conductor.FadeSong(1f, fadeInDuration).Done();
	}

	private void StartFadeIn()
	{
		scrQuad room = RDBase.Vfx.GetRoomOverlay(-1);
		room.SetColor(Color.black);
		startFadeIn = DOVirtual.DelayedCall(0.5f, () =>
		{
			room.sprite.DOKill();
			room.Flash(2.5f);
		});
	}

	private Tween StartFadeOut(float duration = 2.5f)
	{
		if (startFadeIn != null)
		{
			startFadeIn.Kill();
		}
		tk2dSprite sprite = RDBase.Vfx.GetRoomOverlay(-1).sprite;
		sprite.color = Color.clear;
		sprite.DOKill();
		MusicFadeOut(duration);
		return sprite.DOFade(1f, duration).SetEase(Ease.InQuad).SetUpdate(isIndependentUpdate: true);
	}

	public void MusicFadeOut(float duration = 2.5f)
	{
		base.conductor.StopSong(duration);
	}

	public void StopAllAmbiences(float duration = 1f)
	{
		RDEnvironment[] array = environments;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].StopAmbiences(duration);
		}
	}

	public void StartAllAmbiences(float duration = 0.2f)
	{
		RDEnvironment[] array = environments;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].StartAmbiences(duration);
		}
	}

	private void MoveCamera(int newIndex, bool animateCamera = true, bool force = false, bool moveCamera = true, bool changeIndex = true)
	{
		if (!((newIndex != selectedEntityIndex) | force))
		{
			return;
		}
		SelectableEntity selectableEntity = selectableEntities[newIndex];
		if (moveCamera)
		{
			float num = wrldCamera.RDFullNativeResolution().x / 2f;
			float endValue = selectableEntity.gamePosition.x + selectableEntity.cameraOffset.x - num;
			if (currentWardIndex == 4)
			{
				endValue = 4169f;
			}
			cameraObject.transform.DOKill();
			if (animateCamera)
			{
				camMoveXTween = cameraObject.transform.DOLocalMoveX(endValue, 1f).SetEase(Ease.OutQuint);
			}
			else
			{
				cameraObject.transform.LocalMoveX(endValue);
			}
		}
		MoveArrowsToEntity(selectableEntity);
		description.GetComponent<RectTransform>().anchoredPosition = new Vector2(selectableEntity.gamePosition.x + selectableEntity.descriptionOffset.x - RDBase.Vfx.RDWidth * 0.5f, selectableEntity.descriptionOffset.y + 40f);
		if (changeIndex)
		{
			selectedEntityIndex = newIndex;
			currentWardIndex = selectableEntity.group;
		}
	}

	private void MoveArrowsToEntity(SelectableEntity entity)
	{
		float num = levelsContainer.localPosition.x;
		arrows.localPosition = arrows.localPosition.WithXY(entity.gamePosition.x - num, entity.gameObject.transform.localPosition.y);
		upArrow.transform.parent.localPosition = entity.upArrowOffset;
		downArrow.transform.parent.localPosition = entity.downArrowOffset;
	}

	private void InstantiateCamera(int newIndex)
	{
		RDBase.Vfx.SetSpotlightParent(wrldCamera.transform);
		_ = arrows.localPosition;
		Vector3 localPosition = description.transform.localPosition;
		if (newIndex != selectedEntityIndex)
		{
			SelectableEntity selectableEntity = selectableEntities[newIndex];
			float num = wrldCamera.RDFullNativeResolution().x * 0.5f;
			float num2 = 0f;
			num2 = ((currentWardIndex != 4) ? (selectableEntity.gamePosition.x + selectableEntity.cameraOffset.x - num) : 4169f);
			cameraObject.transform.position = cameraObject.transform.position.WithX(num2);
			localPosition.x = selectableEntities[newIndex].gamePosition.x - num;
			description.transform.localPosition = localPosition;
			selectedEntityIndex = newIndex;
		}
	}

	private void SetBackgroundAppearance(Difficulty difficulty)
	{
		Color color = new Color(0.392f, 0.392f, 0.51f, 1f);
		Color white = Color.white;
		bool flag = difficulty == Difficulty.Hard;
		Color color2 = (flag ? color : white);
		mainWard.SetNightMode(flag);
		svtWard.SetNightMode(flag);
		train.trainActive = false;
		train.SetNightMode(flag);
		train.trainActive = true;
		athleteWard.SetNightMode(flag);
		rwCabinetTarp.GetComponent<tk2dSprite>().color = color2;
		if (robotsContainer.activeSelf)
		{
			foreach (Transform item in robotsContainer.transform)
			{
				scrChar component = item.GetComponent<scrChar>();
				component.usesOwnShaderData = true;
				component.shaderData.overlayColor = (flag ? "31317C65".HexToColor() : Color.clear);
			}
		}
		string[] obj = new string[6] { "Seamless_TVScreen", "RightEnd_TVScreen", "Seamless_ExitSign", "SVT_BlueLights", "VendingMachineLights", "SVT_VendingMachineLights" };
		int num = 0;
		string[] array = obj;
		for (int i = 0; i < array.Length; i++)
		{
			GameObject gameObject = GameObject.Find(array[i]);
			if ((bool)gameObject)
			{
				bool flag2 = difficulty == Difficulty.Normal;
				gameObject.GetComponent<tk2dSprite>().SortingOrder = (flag2 ? 2 : 6);
				num++;
			}
		}
		athleteDreamBubbleSmall.gameObject.SetActive(flag && !Level.LuckyBreak.Locked());
	}

	private void ChangeTextOutline(Color color)
	{
	}

	private void ChangeArrowsOutline(Color color)
	{
		RDShaderProperties rDShaderProperties = new RDShaderProperties();
		rDShaderProperties.outline = 1;
		rDShaderProperties.borderColor = color;
		Entity[] componentsInChildren = arrows.GetComponentsInChildren<Entity>();
		foreach (Entity obj in componentsInChildren)
		{
			obj.shaderDataSource = obj;
			obj.shaderData = rDShaderProperties;
		}
	}

	public static AudioSource LevelSelectPlaySound(string sound = "sndMenuPop", float volume = 1f, float pitch = 1f, float pan = 0f)
	{
		return scrConductor.PlayImmediately(sound, volume, RDUtils.GetMixerGroup("StoryLevelSelect"), pitch, pan);
	}

	private bool IsAnyArrowKeyDown()
	{
		if (!RDInput.rightIsPressed && !RDInput.leftIsPressed && !RDInput.upIsPressed)
		{
			return RDInput.downIsPressed;
		}
		return true;
	}

	public void LeftArrowTouchButtonDown()
	{
		leftArrowButtonDown = true;
		LeftArrowAction();
	}

	public void RightArrowTouchButtonDown()
	{
		rightArrowButtonDown = true;
		RightArrowAction();
	}

	public void LeftArrowTouchButtonUp()
	{
		leftArrowButtonDown = false;
	}

	public void RightArrowTouchButtonUp()
	{
		rightArrowButtonDown = false;
	}

	private bool IsAnyTouchButtonDown()
	{
		if (leftArrowButtonDown || rightArrowButtonDown)
		{
			return Input.touches.Length != 0;
		}
		return false;
	}

	private static string GetMistakeOffsetTypeString()
	{
		return MistakesManager.mistakeOffsetType.ToString();
	}

	private Level GetLevelWithIndexAndDifficulty(int index, Difficulty difficulty)
	{
		return (selectableEntities[index] as SelectableCharacter).levels[difficulty];
	}

	private Difficulty GetDifficultyFromIndex(int index)
	{
		SelectableCharacter selectableCharacter = selectableEntities[index] as SelectableCharacter;
		Level level = justPlayedLevel;
		if (level == selectableCharacter.levels[Difficulty.Tutorial] || level == selectableCharacter.levels[Difficulty.Normal] || level == Level.None || level == Level.OpeningCreds || level == Level.Intro)
		{
			return Difficulty.Normal;
		}
		return Difficulty.Hard;
	}

	private int GetBasementComputerId()
	{
		for (int i = 0; i < selectableEntities.Count; i++)
		{
			if (selectableEntities[i] is SelectableObject && (selectableEntities[i] as SelectableObject).id == "BasementComputer")
			{
				return i;
			}
		}
		Debug.LogWarning("Came from BasementComputer but no selectable object with the BasementComputer id was found");
		return 0;
	}

	private int GetCharacterIndexFromLevel(Level level)
	{
		if (level == Level.None)
		{
			return 0;
		}
		for (int i = 0; i < selectableEntities.Count; i++)
		{
			if (!(selectableEntities[i] is SelectableCharacter))
			{
				continue;
			}
			foreach (KeyValuePair<Difficulty, Level> level2 in (selectableEntities[i] as SelectableCharacter).levels)
			{
				if (level2.Value == level)
				{
					return i;
				}
			}
		}
		return 0;
	}

	private int GetNextAvailableLevelIndex(int currentIndex)
	{
		if (currentIndex == selectedEntityIndex)
		{
			return GetNextAvailableLevelIndex(currentIndex + 1);
		}
		SelectableEntity selectableEntity = selectableEntities[currentIndex];
		if (selectableEntity is SelectableObject || !selectableEntity.normalEnabled || selectableEntity.group != currentWardIndex)
		{
			return GetNextAvailableLevelIndex(currentIndex + 1);
		}
		return currentIndex;
	}

	private int GetNextAvailableIndex(int currentIndex)
	{
		if (currentIndex == selectedEntityIndex)
		{
			return GetNextAvailableIndex(currentIndex + 1);
		}
		SelectableEntity selectableEntity = selectableEntities[currentIndex];
		if (!selectableEntity.normalEnabled || selectableEntity.group != currentWardIndex)
		{
			return GetNextAvailableIndex(currentIndex + 1);
		}
		return currentIndex;
	}

	private int GetPreviousAvailableIndex(int currentIndex)
	{
		if (currentIndex == selectedEntityIndex)
		{
			return GetPreviousAvailableIndex(currentIndex - 1);
		}
		SelectableEntity selectableEntity = selectableEntities[currentIndex];
		if (!selectableEntity.normalEnabled || selectableEntity.group != currentWardIndex)
		{
			return GetPreviousAvailableIndex(currentIndex - 1);
		}
		return currentIndex;
	}

	private Dictionary<string, object> CreateDictionary(params object[] array)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		if (array.Length >= 2)
		{
			for (int i = 0; i < array.Length; i += 2)
			{
				dictionary.Add((string)array[i], array[i + 1]);
				MonoBehaviour.print(i);
			}
		}
		else
		{
			dictionary = null;
		}
		return dictionary;
	}

	private void MakeCharacterGiveGift(SelectableEntity character, Gift counter)
	{
		modeXX = !modeXX;
		if (modeXX)
		{
			Vector3 position = character.gameObject.transform.position;
			float num = position.x - cameraObject.transform.position.x;
			float num2 = position.y;
			MonoBehaviour.print(character.id + "| x: " + num + " y: " + num2);
			FreeStar.MoveXY(num, num2);
		}
		else
		{
			FreeStar.MoveXY(CounterStarIcon.position);
		}
	}

	public void OpenUnbeatableURL()
	{
		printe("openurl");
		Application.OpenURL(unbeatableKickstarter.gameObject.activeSelf ? "https://www.kickstarter.com/projects/dcellgames/unbeatable" : "https://www.unbeatablegame.com/");
	}

	public void OpenMuseDashURL()
	{
		printe("openurl");
		Application.OpenURL("https://store.steampowered.com/app/774171/Muse_Dash/");
	}

	public void OpenBitsAndBopsURL()
	{
		printe("openurl");
		Application.OpenURL(bitsAndBopsKickstarter.gameObject.activeSelf ? "https://www.kickstarter.com/projects/tempolabgames/bits-and-bops" : "https://www.tempolabgames.com/");
	}

	public void OpenVividStasisURL()
	{
		printe("openurl");
		Application.OpenURL("https://store.steampowered.com/app/2093940/vividstasis/");
	}

	public void OpenSparkLineURL()
	{
		printe("openurl");
		Application.OpenURL("https://circleofsparks.com/");
	}

	public void OpenBeatblockURL()
	{
		printe("openurl");
		Application.OpenURL("https://store.steampowered.com/app/3045200/Beatblock/");
	}

	public void SayCurrentEntity(string speech)
	{
		string text = currentWardIndex switch
		{
			1 => "ExitSVTWard.day", 
			2 => "GoToSVTWard.day", 
			3 => "GoToBasement.day", 
			5 => "GoToTrain.day", 
			6 => "GoToArtRoom.day", 
			7 => "GoToMuseDashRoom.day", 
			8 => "GoToAthleteWard.day", 
			9 => "GoToRooftop.day", 
			10 => "GoToRecordsRoom.day", 
			11 => "GoToVoid.day", 
			_ => ((Func<int, string>)((int v) =>
			{
				Debug.LogError($"new ward index not added here: {v}");
				return (string)null;
			}))(currentWardIndex), 
		};
		bool flag = false;
		if (text != lastLocationName)
		{
			flag = true;
			lastLocationName = text;
			List<SelectableEntity> list = new List<SelectableEntity>();
			foreach (SelectableEntity selectableEntity in selectableEntities)
			{
				if (currentWardIndex == selectableEntity.group && (selectableEntity.normalEnabled || selectableEntity.hardEnabled))
				{
					list.Add(selectableEntity);
				}
			}
			allWardEntities = new List<SelectableEntity>(list.OrderBy((SelectableEntity p) => p.gamePosition.x).ToList());
		}
		List<SelectableEntity> list2 = new List<SelectableEntity>();
		foreach (SelectableEntity allWardEntity in allWardEntities)
		{
			if (((currentDifficulty == Difficulty.Normal && allWardEntity.normalEnabled) || (currentDifficulty == Difficulty.Hard && allWardEntity.hardEnabled)) && allWardEntity.group == currentWardIndex)
			{
				list2.Add(allWardEntity);
			}
		}
		int num = list2.FindIndex((SelectableEntity entity) => entity.id == selectedEntity.id);
		if (flag)
		{
			string newValue = RDString.Get("levelSelect." + text);
			string newValue2 = (changingWard ? RDString.Get("narration.locationMoving") : string.Empty);
			string newValue3 = ((currentDifficulty == Difficulty.Hard) ? RDString.Get("narration.night") : RDString.Get("narration.day"));
			Narration.Set(RDString.Get("narration.locationFull").Replace("[locationName]", newValue).Replace("[movingTo]", newValue2)
				.Replace("[shift]", newValue3), ElementType.HorizontalList, allWardEntities.Count);
		}
		string text2 = string.Empty;
		if (selectedEntity is SelectableCharacter)
		{
			string[] array = description.text.Split('\n');
			if (array.Length >= 4)
			{
				string text3 = array[^1];
				if (text3.Contains("<color=") || text3.Contains("</color>"))
				{
					while (text3.IndexOf('<') != -1)
					{
						int num2 = text3.IndexOf('<');
						int num3 = text3.IndexOf('>');
						text3 = text3.Remove(num2, num3 + 1 - num2);
					}
				}
				text2 += text3.Replace("-", " ");
			}
		}
		string text4 = string.Empty;
		NarrationActionName[] actions = new NarrationActionName[3]
		{
			NarrationActionName.ToSelect,
			NarrationActionName.ToPause,
			NarrationActionName.ToNavigateHorizontal
		};
		if (arrowsShouldBeVisible)
		{
			if (selectedEntity is SelectableCharacter)
			{
				actions = new NarrationActionName[4]
				{
					NarrationActionName.ToSelect,
					NarrationActionName.ToChangeShift,
					NarrationActionName.ToPause,
					NarrationActionName.ToNavigateHorizontal
				};
				if (currentDifficulty == Difficulty.Normal && selectedEntity.hardEnabled)
				{
					text4 = RDString.Get("narration.nightShift");
				}
				else if (currentDifficulty == Difficulty.Hard && selectedEntity.normalEnabled)
				{
					text4 = RDString.Get("narration.dayShift");
				}
			}
			else
			{
				actions = new NarrationActionName[4]
				{
					NarrationActionName.ToSelect,
					NarrationActionName.ToChangeDestination,
					NarrationActionName.ToPause,
					NarrationActionName.ToNavigateHorizontal
				};
				text4 = RDString.Get("narration.destinationAvailable");
			}
		}
		if (text4 != string.Empty && !text2.IsNullOrEmpty())
		{
			text2 = text2 + RDString.Get("narration.shortPause") + text4;
		}
		Narration.Say(speech, NarrationCategory.Navigation, num + 1, list2.Count, ElementType.SelectableEntity, null, text2, flipCategoryQueueBehaviour: false, actions);
		lastEntityNarration = speech;
	}

	private void PrepareAthleteWardTransition()
	{
		bool flag = !Level.LuckyBreak.Passed();
		bool flag2 = justPlayedLevel == Level.LuckyBreak && Persistence.GetGameResult() == GameResult.LevelJustPassed;
		if (flag | flag2)
		{
			athleteWard.gameObject.SetActive(value: false);
			athleteDreamBubbleBig.gameObject.SetActive(value: true);
			athleteDreamBubbleBigParticles.gameObject.SetActive(value: true);
			athleteWard.StopAmbiences();
			ParticleSystem[] bubbles = athleteDreamBubbleBigParticles.bubbles;
			for (int i = 0; i < bubbles.Length; i++)
			{
				bubbles[i].Simulate(5f);
			}
			athleteDreamBubbleBigParticles.playing = !flag;
			athleteWard.physioFg.GetComponent<MeshRenderer>().sortingLayerName = "Background";
		}
	}

	public Sequence DoAthleteWardTransition()
	{
		athleteWard.gameObject.SetActive(value: true);
		GameObject gameObject = new GameObject("Circle");
		gameObject.transform.SetParent(athleteDreamBubbleBigParticles.transform);
		gameObject.transform.position = selectedEntity.gamePosition + Vector2.down * 600f;
		CircleCollider2D circle = gameObject.gameObject.AddComponent<CircleCollider2D>();
		athleteDreamBubbleBigParticles.shapes.Add(circle);
		athleteDreamBubbleBigParticles.invert = true;
		circle.radius = 0f;
		TweenerCore<float, float, FloatOptions> t = DOTween.To(() => circle.radius, (float x) =>
		{
			circle.radius = x;
		}, 88f, 0.5f).SetEase(Ease.OutCubic).OnPlay(() =>
		{
			StopAllAmbiences();
			scrConductor.PlayImmediately("sndTransitionNightmare");
			Narration.Say(RDString.Get("LuckyBreak.desc10"), NarrationCategory.Description, false);
		})
			.Pause();
		TweenerCore<float, float, FloatOptions> t2 = DOTween.To(() => circle.radius, (float x) =>
		{
			circle.radius = x;
		}, 66f, 0.75f).SetEase(Ease.InOutSine).Pause();
		TweenerCore<float, float, FloatOptions> t3 = DOTween.To(() => circle.radius, (float x) =>
		{
			circle.radius = x;
		}, 352f, 1.25f).SetEase(Ease.InQuart).Pause();
		Sequence obj = DOTween.Sequence();
		obj.Append(t).Append(t2).Append(t3)
			.SetDelay(1f)
			.AppendInterval(1f)
			.Play();
		obj.onComplete = (TweenCallback)Delegate.Combine(obj.onComplete, (TweenCallback)(() =>
		{
			athleteDreamBubbleBig.gameObject.SetActive(value: false);
			athleteDreamBubbleBigParticles.playing = false;
			athleteWard.physioFg.GetComponent<MeshRenderer>().sortingLayerName = "Foreground";
			StartAllAmbiences(2f);
		}));
		return obj;
	}

	public Sequence DoEdegaJumpscare()
	{
		athleteWard.ToggleEdega(visible: true, 1f);
		athleteWard.ToggleEdega(visible: false, 2f);
		return DOTween.Sequence();
	}

	public void PlayAct5Intro(bool instant = false)
	{
		StartCoroutine(Act5IntroCo(instant));
	}

	private IEnumerator Act5IntroCo(bool instant)
	{
		controlsAreLocked = true;
		dialogueIsRunning = true;
		Persistence.SetLastPlayedLevel(Level.OrientalInsomniac);
		Persistence.SetLevelRank(Level.LuckyBreak.ToString(), -1);
		Persistence.SetPlayedPreAct5Cutscene(played: true);
		Persistence.SaveAll();
		SetUIVisible(visible: false);
		int characterIndexFromLevel = GetCharacterIndexFromLevel(Level.OrientalInsomniac);
		_ = selectableEntities[characterIndexFromLevel];
		MoveCamera(characterIndexFromLevel, animateCamera: false);
		act5TransitionText.text = "";
		act5TransitionText.font = RDString.currentLanguageFont;
		string text = (instant ? RDString.Get("levelSelect.act4CompleteTransitionAlternate") : RDString.Get("levelSelect.act4CompleteTransition"));
		string[] splitText = text.Split('\n');
		bool isManuallyWritten = splitText.Length > 2;
		_ = string.Empty;
		int newlineIndex = -1;
		string fullText;
		if (isManuallyWritten)
		{
			for (int i = 0; i < splitText.Length; i++)
			{
				if (splitText[i].Contains('/'))
				{
					if (newlineIndex == -1)
					{
						newlineIndex = i;
					}
					splitText[i] = splitText[i].Replace('/', '\n');
				}
			}
			fullText = splitText[^1];
		}
		else
		{
			fullText = string.Join('\n', splitText);
			newlineIndex = fullText.IndexOf('\n');
			if (newlineIndex != -1)
			{
				newlineIndex++;
			}
		}
		if (newlineIndex != -1)
		{
			act5TransitionText.rectTransform.localPosition += Vector3.up * 8f;
		}
		if (instant)
		{
			SetDifficulty(Difficulty.Hard);
			playSongCall?.Kill();
			StopAllAmbiences(0f);
			act5TransitionCanvasGroup.alpha = 1f;
			yield return new WaitForSeconds(1f);
		}
		else
		{
			yield return new WaitForSeconds(0.5f);
			base.conductor.StopSong(2.5f);
			StopAllAmbiences(2.5f);
			TweenerCore<float, float, FloatOptions> t = act5TransitionCanvasGroup.DOFade(1f, 2f);
			yield return t.WaitForCompletion();
		}
		SetDifficulty(Difficulty.Normal);
		SetUIVisible(visible: false);
		yield return StartCoroutine(BlinkCaret(act5TransitionText, string.Empty, 2f));
		int length = (isManuallyWritten ? (splitText.Length - 1) : fullText.Length);
		float time = 0f;
		int prevProgress = -1;
		while (prevProgress < length)
		{
			int num = (int)(time / 0.08f);
			string text2 = (isManuallyWritten ? splitText[num] : fullText.Substring(0, num));
			act5TransitionText.text = text2 + "|";
			if (prevProgress != num)
			{
				PlayTypingSound("StoryLevelSelect");
				prevProgress = num;
				if (num == newlineIndex - 1)
				{
					yield return StartCoroutine(BlinkCaret(act5TransitionText, text2, 1f));
				}
				else if (num == newlineIndex)
				{
					yield return StartCoroutine(BlinkCaret(act5TransitionText, text2, 1f));
				}
			}
			time += Time.deltaTime;
			yield return null;
		}
		Narration.StopSpeaking();
		Narration.Say(fullText, NarrationCategory.Notification, false);
		string text3 = (isManuallyWritten ? splitText[length] : fullText.Substring(0, length));
		StartCoroutine(BlinkCaret(act5TransitionText, text3, 6f));
		yield return new WaitForSeconds(4f);
		StartConductor();
		StartAllAmbiences(3f);
		yield return act5TransitionCanvasGroup.DOFade(0f, 2f).WaitForCompletion();
		yield return base.ink.RunAndWait("diaLevelSelect", "Act5_Start");
		SetUIVisible(visible: true);
		dialogueIsRunning = false;
		controlsAreLocked = false;
	}

	private IEnumerator BlinkCaret(Text textComp, string text, float duration)
	{
		float time = 0f;
		while (time < duration)
		{
			time += Time.deltaTime;
			string text2 = ((time % 0.5f * 2f < 0.5f) ? "|" : "<color=#0000>|</color>");
			textComp.text = text + text2;
			yield return null;
		}
	}

	public void PlayAct6Intro()
	{
		StartCoroutine(Act6IntroCo());
	}

	private IEnumerator Act6IntroCo()
	{
		RDBaseDllDummy.printem("Playing Act 6 intro...");
		controlsAreLocked = true;
		dialogueIsRunning = true;
		Persistence.SetLastPlayedLevel(Level.AthleteFinale);
		Persistence.SetLevelRank(Level.HaileyDuet, -1);
		Persistence.SetPlayedAct6Intro(played: true);
		Persistence.SaveCurrentSlot();
		MoveCamera(mainElevatorEntranceEntityIndex, animateCamera: false);
		SetDifficulty(Difficulty.Normal);
		SetUIVisible(visible: false);
		yield return base.ink.RunAndWait("diaLevelSelect", "Act6_Start");
		SetUIVisible(visible: true);
		dialogueIsRunning = false;
		controlsAreLocked = false;
	}

	public void PlayAct7Intro()
	{
		StartCoroutine(PlayAct7IntroCo());
	}

	private IEnumerator PlayAct7IntroCo()
	{
		controlsAreLocked = true;
		dialogueIsRunning = true;
		Persistence.SetLevelRank(Level.Blurred, -1);
		Persistence.SaveCurrentSlot();
		MoveCamera(mainElevatorEntranceEntityIndex, animateCamera: false);
		UpdateMusic(selectedEntity, changedArea: true);
		SetDifficulty(Difficulty.Normal);
		SetUIVisible(visible: false);
		yield return base.ink.RunAndWait("diaLevelSelect", "Level_Blurred_New");
		SetUIVisible(visible: true);
		dialogueIsRunning = false;
		controlsAreLocked = false;
		Persistence.SetPlayedPassedLevelCutscene(played: true);
		Persistence.SaveCurrentSlot();
	}

	public void PlayFinaleIntro()
	{
		StartCoroutine(PlayFinaleIntroCo());
	}

	private IEnumerator PlayFinaleIntroCo()
	{
		controlsAreLocked = true;
		dialogueIsRunning = true;
		MoveCamera(mainElevatorEntranceEntityIndex, animateCamera: false);
		selectedVerticalIndex = 1;
		SetDifficulty(Difficulty.Normal);
		SetUIVisible(visible: false);
		yield return base.ink.RunAndWait("diaLevelSelect", "Level_Montage_New");
		SetUIVisible(visible: true);
		dialogueIsRunning = false;
		controlsAreLocked = false;
		Persistence.SetPlayedPassedLevelCutscene(played: true);
		Persistence.SaveCurrentSlot();
	}

	public static Dictionary<string, object> GetStoryVariables()
	{
		return new Dictionary<string, object>
		{
			["feedback"] = GetMistakeOffsetTypeString(),
			["mistakeCount"] = Mathf.RoundToInt(Persistence.GetMistakeCount())
		};
	}
}
