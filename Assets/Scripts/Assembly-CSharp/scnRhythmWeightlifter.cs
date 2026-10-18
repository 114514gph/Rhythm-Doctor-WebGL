using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace RhythmWeightlifter
{
	public class scnRhythmWeightlifter : scnBase
	{
		private enum Screen
		{
			Intro = 0,
			LevelSelect = 1,
			Gameplay = 2,
			Ending = 3,
			DeleteData = 4,
			LanguageSelection = 5,
			ExtraScreen = 6
		}

		private enum HeadHeight
		{
			Low = 33,
			Mid = 66,
			High = 67
		}

		private enum GameState
		{
			Playing = 0,
			Paused = 1,
			NoLift = 2,
			GoodLift = 3
		}

		private const float FadeDuration = 0.5f;

		private const float TextsFadeAnimDuration = 0.3f;

		public const float originalMaxDistance = 255f;

		public const float originalPushStrength = 50f;

		public const float originalGravityMultiplier = 0.8f;

		public const float inconsistentDropRateMultiplier = 12f;

		private const int consistencyScoreMin = 0;

		public const int consistencyScoreMax = 50;

		private const int speedScoreMin = 0;

		public const int speedScoreMax = 100;

		public static readonly ScoreEntry[] highScores = new ScoreEntry[6]
		{
			new ScoreEntry
			{
				initialsKey = "rhythmWeightlifter.initialsIan",
				score = 2350,
				character = Character.Ian
			},
			new ScoreEntry
			{
				initialsKey = "rhythmWeightlifter.initialsSmokinBarista",
				score = 2050,
				character = Character.SmokinBarista
			},
			new ScoreEntry
			{
				initialsKey = "rhythmWeightlifter.initialsMiner",
				score = 1850,
				character = Character.Miner
			},
			new ScoreEntry
			{
				initialsKey = "rhythmWeightlifter.initialsSamurai",
				score = 1100,
				character = Character.Samurai
			},
			new ScoreEntry
			{
				initialsKey = "rhythmWeightlifter.initialsPaige",
				score = 650,
				character = Character.Paige
			},
			new ScoreEntry
			{
				initialsKey = "rhythmWeightlifter.initialsPolitician",
				score = 20,
				character = Character.Politician
			}
		};

		private const int MetronomeBeatsPerBar = 4;

		private double nextBar;

		private const double MetronomeBPM = 220.0;

		[NonSerialized]
		public Failure failure;

		[Header("General")]
		public GameObject weightlifter;

		public SpriteRenderer maxLimit;

		public Image foregroundFade;

		public RDImageEffectPixelate pixelateOverlay;

		public Image fadeOverlay;

		[Header("Scene Objects")]
		public GameObject intro;

		public GameObject levelSelect;

		public GameObject gameplay;

		public GameObject ending;

		public GameObject deleteRWLDataScreen;

		public GameObject languageSelectionScreen;

		[Header("Canvases and UI")]
		public RectTransform foregroundImageDefault;

		public RectTransform canvasContainer;

		public RectTransform foregroundCanvas;

		public RectTransform backgroundCanvas;

		public SpriteAnimation crowdSpriteAnim;

		public Image lightLeft;

		public Sprite lightLeftOff;

		public Sprite lightLeftOn;

		public Image metronomeLightL;

		public Image metronomeLightR;

		[Header("Crowd Animations")]
		public SpriteAnimation.SpriteAnimationData neutralAnimData;

		public SpriteAnimation.SpriteAnimationData winAnimData;

		public SpriteAnimation.SpriteAnimationData loseAnimData;

		public SpriteAnimation.SpriteAnimationData almostWinAnimData;

		public SpriteAnimation.SpriteAnimationData almostLoseAnimData;

		[Header("Intro")]
		public Text resetInstructions;

		public GameObject tempoCredits;

		[Header("Level Select")]
		public Level[] levels;

		public RectTransform levelsContainer;

		public Text speedText;

		public Text consistencyText;

		public Text rankText;

		public Text scoreText;

		public Text rankLetterText;

		public Text levelNumberText;

		public Text weightP1Text;

		public Text weightP2Text;

		public Vector2[] weightTextPositions;

		public Image medalImage;

		public Sprite bronzeMedalSprite;

		public Sprite silverMedalSprite;

		public Sprite goldMedalSprite;

		public Sprite perfectMedalSprite;

		public Text finalScoreText;

		public SpriteRenderer screenP1Barbel10kgWeight;

		public SpriteRenderer screenP2Barbel10kgWeight;

		[Header("Level Select Extra Containers")]
		public ExtraContainer[] leftExtraContainers;

		public ExtraContainer[] rightExtraContainers;

		public GameObject levelSelectLineDetail;

		[Header("Infinite Mode")]
		public Sprite heartFullSprite;

		[Header("Infinite Mode")]
		public Sprite heartEmptySprite;

		public Transform infiniteModeHeartsContainer;

		public Text extraLifeText;

		public RectTransform infiniteModeLevelContainerLeftWeight;

		public RectTransform infiniteModeLevelContainerRightWeight;

		[Header("Gameplay Screen")]
		public Barbell barbell;

		public Camera camera;

		public Camera backgroundCanvasCamera;

		public Text goalText;

		public Text screenLevelNumberText;

		public Text screenWeightP1Text;

		public Text screenWeightP2Text;

		public Text gameplayWeightP1Text;

		public Text gameplayWeightP2Text;

		public GameObject[] p1Weights;

		public GameObject[] p2Weights;

		public GameObject pausedStatus;

		public GameObject noLiftStatus;

		public GameObject tooSlowStatus;

		public GameObject tooInconsistentStatus;

		public GameObject goodLiftStatus;

		public GameObject statsContainer;

		public GameObject pausedOptions;

		public GameObject rankContainer;

		public GameObject announcerCaption;

		public Text announcerCaptionText;

		public RDInk outGameInk;

		public RDInk inGameInk;

		public RectTransform topDisplay;

		public Sprite metronomeLightOff;

		public Sprite metronomeLightOn;

		[Header("Pause Screen + Rank Screen")]
		public RectTransform pausedSelector;

		public Image consistencyBarP1;

		public Image consistencyBarP2;

		public Image speedBar;

		public Text statsRankText;

		public Text statsScoreText;

		public Image pauseLed;

		public Color pauseLedColorPlaying;

		public Color pauseLedColorLost;

		public Color pauseLedColorWon;

		public Image[] pausedIcons;

		public Color pausedIconSelectedColor;

		public Color pausedIconUnselectedColor;

		public Color goodLiftColor;

		public Color noLiftColor;

		public Text[] pauseOptionTexts;

		public LevelCharacter[] characters;

		public RectTransform playersIndicators;

		public Text livesText;

		public RectTransform livesIndicators;

		public Image leftCharacterIcon;

		public Image rightCharacterIcon;

		public Text leftCharacterText;

		public Text rightCharacterText;

		public Text languageSelectableText;

		[Header("Gameplay Body parts")]
		public Transform torso;

		public Transform leftFoot;

		public Transform rightFoot;

		public float leftFootCloseX;

		public float leftFootOpenX;

		public float rightFootCloseX;

		public float rightFootOpenX;

		public float footDuration;

		public float torsoLowY;

		public float torsoMidY;

		public float torsoHighY;

		public float torsoDuration;

		public FixedJoint2D leftHandJoint;

		public FixedJoint2D rightHandJoint;

		public Rigidbody2D leftHandRB;

		public Rigidbody2D rightHandRB;

		public SpriteRenderer[] spritesAffectedByBuckling;

		public Color bucklingTintColor;

		[Header("Gameplay Sounds")]
		public RWSound flexHigh;

		[Header("Gameplay Sounds")]
		public RWSound flexHighAlt;

		public RWSound flexLow;

		public RWSound flexLowAlt;

		public RWSound wobbleHigh;

		public RWSound wobbleLow;

		public RWSound dropP1;

		public RWSound dropP2;

		public RWSound metronomeLSound;

		public RWSound metronomeRSound;

		private RWSoundController soundController;

		[Header("Ending")]
		public GameObject endingPrefab;

		private Ending endingComp;

		[Header("Mobile")]
		public VirtualTouchControls virtualTouchControls;

		public RectTransform mobileForegroundOverlay;

		public RectTransform mobileDecorationCablesLR;

		public RectTransform mobileDecorationSliderLeft;

		public RectTransform mobileDecorationKnobLeft;

		public RectTransform mobileDecorationHeartMonitor;

		public RectTransform mobileDecorationRhythmBoyLogo;

		public RectTransform foregroundLeftOverlay;

		public RectTransform foregroundRightOverlay;

		public RectTransform foregroundCenterBorders;

		public RectTransform foregroundTopBorder;

		public RectTransform foregroundBottomBorder;

		[Header("Dog Easter Egg")]
		public Sprite dogSprite;

		public RWSound dogHigh;

		public RWSound dogHighAlt;

		public RWSound dogLow;

		public RWSound dogLowAlt;

		private Vector2 lastScreenSize = Vector2.zero;

		private HeadHeight headCurrentHeight = HeadHeight.High;

		private Dictionary<Screen, GameObject> screens;

		private int lastUnlockedLevelIndex;

		private int pausedOptionIndex;

		private bool canReceiveInput;

		private AudioMixerGroup mixerGroup;

		private AudioSource ambienceLoopAudioSrc;

		private List<string> announcerPool;

		private bool showingGameplayTexts;

		private float lastTimeDialogueWasPlayed;

		private Screen screen;

		private Screen lastScreenCache;

		[NonSerialized]
		public bool unbalancedWarning;

		private bool shouldShowEnding;

		private bool shouldAdvanceLevel;

		private Tween sceneTransitionTween;

		private Tween infiniteModeWeightAnimTween;

		private Sprite torsoSprite;

		private int currentExtraSlot = -1;

		private bool currentExtraSideIsRight;

		private int pendingExtraSlot = -1;

		private bool pendingExtraSideIsRight;

		private bool isOnInfiniteExtra;

		[NonSerialized]
		public bool isInfiniteMode;

		private bool hasObtainedBestRound;

		private bool _metronome;

		private int _levelIndex = -1;

		private bool paused;

		private bool _showingScore;

		private GameState _gameState;

		private Level infiniteModeCurrentLevel;

		private const int infiniteModeMaxLives = 3;

		private int infiniteModeCurrentLives;

		private int currentInfiniteModeRound = 1;

		private bool hasEarnedExtraLife;

		private Level previousInfiniteModeLevel;

		public float infiniteModeEffectiveBpm;

		private List<Level> easyLevels;

		private List<Level> mediumLevels;

		private List<Level> hardLevels;

		private List<Level> extremeLevels;

		private int easyIndex;

		private int mediumIndex;

		private int hardIndex;

		private int extremeIndex;

		private List<double> metronomeLTimes = new List<double>();

		private List<double> metronomeRTimes = new List<double>();

		private int _languageIndex = -1;

		private int lastLevelPlayed = -1;

		private static readonly Color[] InfiniteWeightColors = new Color[3]
		{
			new Color(0.788f, 0.235f, 0.098f),
			new Color(0.192f, 0.651f, 0.494f),
			new Color(1f, 1f, 1f)
		};

		private const float ContainersSpacing = 50f;

		private int _debugLeftPressCount;

		private bool _debugComboArmed;

		private bool _debugComboFiredThisFrame;

		private int _infiniteDebugPhase;

		private int _infiniteDebugFirstCount;

		private int _infiniteDebugSecondCount;

		private float _infiniteDebugPhaseTimer;

		private Level _debugForcedInfiniteLevel;

		private bool _debugInfiniteWaitTimeSoundPlayed;

		private bool _dogMode;

		public static scnRhythmWeightlifter gameInstance => scnBase._instance as scnRhythmWeightlifter;

		public Level CurrentLevel
		{
			get
			{
				if (isInfiniteMode && infiniteModeCurrentLevel != null)
				{
					return infiniteModeCurrentLevel;
				}
				return levels[LevelIndex];
			}
		}

		private bool DialoguePlaying
		{
			get
			{
				if (!inGameInk.isPlaying)
				{
					return outGameInk.isPlaying;
				}
				return true;
			}
		}

		private bool Responsive
		{
			get
			{
				if (Time.time > lastTimeDialogueWasPlayed + 1f)
				{
					if (sceneTransitionTween != null && sceneTransitionTween.IsActive())
					{
						return !sceneTransitionTween.IsPlaying();
					}
					return true;
				}
				return false;
			}
		}

		private bool finishedCampaign => lastUnlockedLevelIndex >= levels.Length;

		private bool Metronome
		{
			get
			{
				return _metronome;
			}
			set
			{
				_metronome = value;
				string key = (_metronome ? "rhythmWeightlifter.metronomeOn" : "rhythmWeightlifter.metronomeOff");
				pauseOptionTexts[1].text = RDString.Get(key);
				pauseOptionTexts[1].font = RDString.GetAppropiateFontForString(pauseOptionTexts[1].text);
				if (value)
				{
					nextBar = soundController.Clock;
				}
				else
				{
					ClearMetronomeSounds();
				}
				float num = (value ? 130f : 200f);
				if (screen == Screen.Gameplay)
				{
					float duration = 0.25f;
					metronomeLightL.rectTransform.DOLocalMoveX(0f - num, duration).SetEase(Ease.OutQuad);
					metronomeLightR.rectTransform.DOLocalMoveX(num, duration).SetEase(Ease.OutQuad);
				}
				else
				{
					metronomeLightL.rectTransform.localPosition = Vector2.right * (0f - num);
					metronomeLightR.rectTransform.localPosition = Vector2.right * num;
				}
			}
		}

		private int LevelIndex
		{
			get
			{
				return _levelIndex;
			}
			set
			{
				if (_levelIndex != value)
				{
					_levelIndex = Mathf.Clamp(value, 0, levels.Length - 1);
					levelsContainer.DOKill();
					levelsContainer.DOAnchorPosX(-50f * (float)_levelIndex, 0.2f).SetEase(Ease.OutExpo);
					RefreshStatsDisplay();
					barbell.Reset();
					barbell.p1Weight.CurrentWeight = CurrentLevel.player1Weight;
					barbell.p2Weight.CurrentWeight = CurrentLevel.player2Weight;
					weightP1Text.text = RDString.Get("rhythmWeightlifter.kgNumber").Replace("[weightNumber]", (CurrentLevel.player1Weight * 10).ToString());
					weightP2Text.text = RDString.Get("rhythmWeightlifter.kgNumber").Replace("[weightNumber]", (CurrentLevel.player2Weight * 10).ToString());
					weightP1Text.rectTransform.anchoredPosition = weightTextPositions[CurrentLevel.player1Weight - 1] * new Vector2(-1f, 1f);
					weightP2Text.rectTransform.anchoredPosition = weightTextPositions[CurrentLevel.player2Weight - 1];
					screenP1Barbel10kgWeight.color = Color.white;
					screenP2Barbel10kgWeight.color = Color.white;
					screenLevelNumberText.text = RDString.Get("rhythmWeightlifter.level").Replace("[levelNumber]", (_levelIndex + 1).ToString());
					screenWeightP1Text.text = (CurrentLevel.player1Weight * 10).ToString();
					screenWeightP2Text.text = (CurrentLevel.player2Weight * 10).ToString();
					gameplayWeightP1Text.text = weightP1Text.text;
					gameplayWeightP2Text.text = weightP2Text.text;
					float num = gameplayWeightP1Text.rectTransform.anchoredPosition.y;
					gameplayWeightP1Text.rectTransform.anchoredPosition = new Vector2(weightTextPositions[CurrentLevel.player1Weight - 1].x * -1f, num);
					gameplayWeightP2Text.rectTransform.anchoredPosition = new Vector2(weightTextPositions[CurrentLevel.player2Weight - 1].x, num);
					UpdateWeights();
					UpdateChars();
				}
			}
		}

		private bool ShowingScore
		{
			get
			{
				return _showingScore;
			}
			set
			{
				_showingScore = value;
				ShowGameplayScreen(_showingScore, _showingScore);
				Rigidbody2D rigidbody2D = leftHandRB;
				RigidbodyType2D bodyType = (rightHandRB.bodyType = (_showingScore ? RigidbodyType2D.Static : RigidbodyType2D.Dynamic));
				rigidbody2D.bodyType = bodyType;
				leftHandRB.DOKill();
				rightHandRB.DOKill();
				if (!_showingScore)
				{
					return;
				}
				Metronome = false;
				leftHandRB.transform.DOLocalMoveY(-32f, 0.5f);
				rightHandRB.transform.DOLocalMoveY(-32f, 0.5f);
				bool num = isInfiniteMode && infiniteModeCurrentLives <= 0;
				gameState = GameState.GoodLift;
				if (num)
				{
					goodLiftStatus.SetActive(value: false);
					noLiftStatus.SetActive(value: true);
				}
				ClearMetronomeSounds();
				CalculateConsistencyScore();
				if (num)
				{
					Level.lastSpeed = 0;
				}
				else
				{
					CalculateSpeedScore();
				}
				if (!isInfiniteMode)
				{
					CurrentLevel.Save();
				}
				if (isInfiniteMode)
				{
					currentInfiniteModeRound++;
					printe($"Incrementing infinite mode round to {currentInfiniteModeRound}");
					if (currentInfiniteModeRound > Persistence.GetBestInfiniteRound())
					{
						hasObtainedBestRound = true;
					}
					if (Level.GetRank(Level.lastScore) == "S" && infiniteModeCurrentLives > 0 && infiniteModeCurrentLives < 3)
					{
						printe("Gained a life (S rank)");
						infiniteModeCurrentLives++;
						hasEarnedExtraLife = true;
					}
				}
				if (!isInfiniteMode)
				{
					int num2 = LevelIndex + 1;
					if (num2 <= levels.Length && num2 > lastUnlockedLevelIndex)
					{
						printe($"level no. {num2 + 1} unlocked!");
						lastUnlockedLevelIndex = num2;
						Persistence.SetRhythmWeightlifterLastLevelUnlocked(num2);
						string text = $"level{LevelIndex}complete";
						if (!scnBase.isMobile && outGameInk.story.HasFunction(text))
						{
							outGameInk.Run(text);
						}
						else if (!scnBase.isMobile)
						{
							printe("possible knot " + text + " not found.");
						}
						shouldAdvanceLevel = true;
					}
					if (finishedCampaign)
					{
						Rank levelRank = Persistence.GetLevelRank(global::Level.RhythmWeightlifter);
						int score = CalculateFinalScore();
						Rank finalRank = GetFinalRank(score);
						Persistence.SetLevelScore(global::Level.RhythmWeightlifter.ToString(), score);
						Persistence.SetLevelRank(global::Level.RhythmWeightlifter, finalRank);
						if ((int)finalRank > (int)levelRank)
						{
							shouldShowEnding = true;
						}
					}
				}
				consistencyBarP1.fillAmount = (float)Level.lastConsistencyP1 / 50f;
				consistencyBarP2.fillAmount = (float)Level.lastConsistencyP2 / 50f;
				speedBar.fillAmount = (float)Level.lastSpeed / 100f;
				string text2 = (num ? "F" : Level.GetRank(Level.lastScore).ToString());
				statsRankText.text = text2;
				statsScoreText.color = (isInfiniteMode ? Color.yellow : Color.white);
				int bestInfiniteRound = Persistence.GetBestInfiniteRound();
				if (isInfiniteMode && bestInfiniteRound == 0)
				{
					statsScoreText.text = "";
				}
				else
				{
					statsScoreText.text = (isInfiniteMode ? RDString.Get("rhythmWeightlifter.bestInfiniteRound").Replace("[roundNumber]", bestInfiniteRound.ToString()) : RDString.Get("rhythmWeightlifter.score").Replace("[score]", Level.lastScore.ToString()));
				}
				RWPlaySound("sndCrowdCheer");
				RWPlaySound("sndSportsBell");
				Narration.Say(RDString.Get("rhythmWeightlifter.goodLift"), NarrationCategory.Notification, false);
				if (text2 == "S")
				{
					PlayRandomAnnouncerLine();
				}
				else
				{
					SayCurrentRank();
				}
				ChangeCrowdAnimation(winAnimData);
			}
		}

		private GameState gameState
		{
			get
			{
				return _gameState;
			}
			set
			{
				if (_gameState == value)
				{
					return;
				}
				bool flag = _gameState == GameState.NoLift;
				_gameState = value;
				if (isInfiniteMode)
				{
					ShowLivesIndicators(_gameState == GameState.Playing);
				}
				if (!isInfiniteMode)
				{
					ShowPlayerIndicators(_gameState == GameState.Playing);
				}
				bool flag2 = _gameState == GameState.GoodLift;
				bool flag3 = _gameState == GameState.NoLift;
				pausedStatus.SetActive(_gameState == GameState.Paused);
				pausedOptions.SetActive(_gameState == GameState.Paused || _gameState == GameState.Playing);
				noLiftStatus.SetActive(flag3 && failure == Failure.None);
				tooInconsistentStatus.SetActive(flag3 && failure == Failure.Consistency);
				tooSlowStatus.SetActive(flag3 && failure == Failure.Speed);
				goodLiftStatus.SetActive(flag2);
				statsContainer.SetActive(flag2);
				rankContainer.SetActive(flag2);
				GameObject[] array = new GameObject[3] { pausedStatus, noLiftStatus, goodLiftStatus };
				foreach (GameObject obj in array)
				{
					Canvas.ForceUpdateCanvases();
					HorizontalLayoutGroup component = obj.GetComponent<HorizontalLayoutGroup>();
					component.enabled = false;
					component.enabled = true;
				}
				if (flag2)
				{
					ChangeCrowdAnimation(winAnimData);
					Image component2 = goodLiftStatus.GetComponent<Image>();
					goodLiftStatus.GetComponentInChildren<Text>().text = RDString.Get("rhythmWeightlifter.goodLift");
					component2.DOKill();
					component2.color = Color.white;
					component2.DOColor(goodLiftColor, 0.3f).SetLoops(3, LoopType.Restart);
				}
				else if (flag3)
				{
					ChangeCrowdAnimation(loseAnimData);
				}
				else if (_gameState == GameState.Playing)
				{
					if (flag)
					{
						nextBar = soundController.Clock;
					}
					ChangeCrowdAnimation(neutralAnimData);
				}
				pauseLed.color = ((_gameState == GameState.GoodLift) ? pauseLedColorWon : ((_gameState == GameState.NoLift) ? pauseLedColorLost : pauseLedColorPlaying));
			}
		}

		private List<SystemLanguage> availableLanguages => RDString.AvailableLanguages;

		private SystemLanguage language => RDString.language;

		private int languageIndex
		{
			get
			{
				if (_languageIndex == -1)
				{
					_languageIndex = availableLanguages.IndexOf(language);
				}
				return _languageIndex;
			}
			set
			{
				if (value < 0)
				{
					_languageIndex = availableLanguages.Count - 1;
				}
				else if (value >= availableLanguages.Count)
				{
					_languageIndex = 0;
				}
				else
				{
					_languageIndex = value;
				}
			}
		}

		private IEnumerable<ExtraContainer> AllExtraContainers
		{
			get
			{
				if (leftExtraContainers != null)
				{
					ExtraContainer[] array = leftExtraContainers;
					for (int i = 0; i < array.Length; i++)
					{
						yield return array[i];
					}
				}
				if (rightExtraContainers != null)
				{
					ExtraContainer[] array = rightExtraContainers;
					for (int i = 0; i < array.Length; i++)
					{
						yield return array[i];
					}
				}
			}
		}

		private ExtraContainer CurrentExtraContainer
		{
			get
			{
				if (currentExtraSlot < 0)
				{
					return null;
				}
				List<ExtraContainer> list = VisibleSideContainers(currentExtraSideIsRight);
				if (currentExtraSlot >= list.Count)
				{
					return null;
				}
				return list[currentExtraSlot];
			}
		}

		public bool DogMode
		{
			get
			{
				return _dogMode;
			}
			set
			{
				if (screen == Screen.LevelSelect)
				{
					_dogMode = value;
					torso.GetComponent<SpriteRenderer>().sprite = (value ? dogSprite : torsoSprite);
					RWPlaySound(value ? "sndDogHowl" : "sndDogCry");
				}
			}
		}

		private void UpdateWeights()
		{
			if (screen != Screen.Intro)
			{
				barbell.p1Weight.UpdateOuterCollarPosition();
				barbell.p2Weight.UpdateOuterCollarPosition();
			}
			Level level = (isInfiniteMode ? infiniteModeCurrentLevel : CurrentLevel);
			if (level != null)
			{
				for (int i = 0; i < p1Weights.Length; i++)
				{
					p1Weights[i].SetActive(i == level.player1Weight - 1);
				}
				for (int j = 0; j < p2Weights.Length; j++)
				{
					p2Weights[j].SetActive(j == level.player2Weight - 1);
				}
			}
		}

		private void UpdateChars()
		{
			Level currentLvl = (isInfiniteMode ? infiniteModeCurrentLevel : CurrentLevel);
			if (currentLvl != null)
			{
				LevelCharacter levelCharacter = Array.Find(characters, (LevelCharacter s) => s.name == currentLvl.leftCharacter);
				leftCharacterIcon.sprite = levelCharacter.sprite;
				leftCharacterText.text = RDString.Get($"rhythmWeightlifter.{levelCharacter.name}");
				leftCharacterText.font = RDString.GetAppropiateFontForString(leftCharacterText.text);
				LevelCharacter levelCharacter2 = Array.Find(characters, (LevelCharacter s) => s.name == currentLvl.rightCharacter);
				rightCharacterIcon.sprite = levelCharacter2.sprite;
				rightCharacterText.text = RDString.Get($"rhythmWeightlifter.{levelCharacter2.name}");
				rightCharacterText.font = RDString.GetAppropiateFontForString(rightCharacterText.text);
			}
		}

		private new void Awake()
		{
			base.Awake();
			scnBase._instance = this;
			inGameInk.Load("diaRhythmWeightlifter");
			inGameInk.weightlifterInGameDialogue = true;
			outGameInk.Load("diaRhythmWeightlifter");
			soundController = new RWSoundController_Tempo();
			soundController.Setup();
			flexHigh.Setup();
			flexHighAlt.Setup();
			flexLow.Setup();
			flexLowAlt.Setup();
			dogHigh.Setup();
			dogHighAlt.Setup();
			dogLow.Setup();
			dogLowAlt.Setup();
			wobbleHigh.Setup();
			wobbleLow.Setup();
			dropP1.Setup();
			dropP2.Setup();
			metronomeLSound.Setup();
			metronomeRSound.Setup();
			Metronome = false;
			torsoSprite = torso.GetComponent<SpriteRenderer>().sprite;
			soundController.Volume = (float)Persistence.GetGlobalVolume() * 0.1f;
			barbell.Setup();
			foregroundImageDefault.gameObject.SetActive(!scnBase.isMobile);
			mobileForegroundOverlay.gameObject.SetActive(scnBase.isMobile);
			pausedIcons[pausedIcons.Length - 1].transform.parent.gameObject.SetActive(scnBase.isMobile);
			languageSelectionScreen.SetActive(value: false);
			deleteRWLDataScreen.SetActive(value: false);
			SetupResolutionForMobile();
		}

		private new void Start()
		{
			base.Start();
			RDInputType_Touch.ResetAllVirtualButtons();
			if (RDC.booth)
			{
				ResetProgress();
			}
			RDRichPresence.Instance?.UpdatePresence();
			canReceiveInput = false;
			DoSceneTransition(fadeIn: true, () =>
			{
				scrConductor.PlayImmediately("sndChampionshipIntro", 1f, RDUtils.GetMixerGroup("MenuMusic"));
				Narration.Say(RDString.Get("rhythmWeightlifter.title") + Narration.ShortPause + RDString.Get("rhythmWeightlifter.holdToBegin"), NarrationCategory.Navigation, false);
				canReceiveInput = true;
			});
			Level[] array = levels;
			for (int num = 0; num < array.Length; num++)
			{
				array[num].Load();
			}
			lastUnlockedLevelIndex = Mathf.Clamp(Persistence.GetRhythmWeightlifterLastLevel(), 0, levels.Length);
			screens = new Dictionary<Screen, GameObject>
			{
				{
					Screen.Intro,
					intro
				},
				{
					Screen.LevelSelect,
					levelSelect
				},
				{
					Screen.Gameplay,
					gameplay
				},
				{
					Screen.Ending,
					ending
				},
				{
					Screen.DeleteData,
					deleteRWLDataScreen
				},
				{
					Screen.LanguageSelection,
					languageSelectionScreen
				}
			};
			LevelIndex = lastUnlockedLevelIndex;
			barbell.Reset();
			barbell.p1Weight.CurrentWeight = CurrentLevel.player1Weight;
			barbell.p2Weight.CurrentWeight = CurrentLevel.player2Weight;
			SetScreen(Screen.Intro);
			ChangeCrowdAnimation(neutralAnimData);
			scrConductor.LoadSavedAudioSettings();
			float globalPanningStrength = scrConductor.GlobalPanningStrength;
			float globalPanningInvert = scrConductor.GlobalPanningInvert;
			float num2 = globalPanningStrength * globalPanningInvert;
			flexHigh.Pan *= num2;
			flexHighAlt.Pan *= num2;
			flexLow.Pan *= num2;
			flexLowAlt.Pan *= num2;
			dogHigh.Pan *= num2;
			dogHighAlt.Pan *= num2;
			dogLow.Pan *= num2;
			dogLowAlt.Pan *= num2;
			wobbleHigh.Pan *= num2;
			wobbleLow.Pan *= num2;
			dropP1.Pan *= num2;
			dropP2.Pan *= num2;
			metronomeLSound.Pan *= num2;
			metronomeRSound.Pan *= num2;
			mixerGroup = RDUtils.GetMixerGroup("IanDesktop");
			Narration.SetInstructions(RDString.Get("rhythmWeightlifter.holdToBegin") + Narration.ShortPause + RDString.Get("narration.rhythmWeightlifter.resetProgress"), NarrationActionName.ToExit);
			if (RDInput.actuallySwapped)
			{
				RDInput.SwapP1AndP2Controls(changePersistence: false);
			}
			extraLifeText.gameObject.SetActive(value: false);
		}

		protected override void OnApplicationPause(bool paused)
		{
			if (!paused && Metronome)
			{
				ClearMetronomeSounds();
				nextBar = soundController.Clock;
			}
			if (paused && isInfiniteMode && currentInfiniteModeRound > Persistence.GetBestInfiniteRound())
			{
				Persistence.SetRhythmWeightlifterBestInfiniteRound(currentInfiniteModeRound);
			}
			base.OnApplicationPause(paused);
		}

		private new void Update()
		{
			base.Update();
			SetupResolutionForMobile();
			double dspTime;
			if (Metronome && screen == Screen.Gameplay && (gameState == GameState.Playing || gameState == GameState.Paused))
			{
				dspTime = soundController.Clock;
				if (dspTime > nextBar)
				{
					float num = 60f / CurrentLevel.bpm * 4f;
					int player1Weight = CurrentLevel.player1Weight;
					int player2Weight = CurrentLevel.player2Weight;
					float num2 = 0.5f;
					for (int i = 0; i < player1Weight; i++)
					{
						metronomeRSound.Volume = (paused ? 0.25f : 0.8f);
						double num3 = dspTime + (double)(num * (float)i / (float)player1Weight) + (double)num2;
						metronomeRSound.Schedule(num3);
						metronomeRTimes.Add(num3);
					}
					for (int j = 0; j < player2Weight; j++)
					{
						metronomeLSound.Volume = (paused ? 0.25f : 0.8f);
						double num4 = dspTime + (double)(num * (float)j / (float)player2Weight) + (double)num2;
						metronomeLSound.Schedule(num4);
						metronomeLTimes.Add(num4);
					}
					if (nextBar == 0.0)
					{
						nextBar = dspTime;
					}
					nextBar += num;
					printe($"{dspTime} nextbar {nextBar}, barDuration {num}");
				}
				LightMetronomeLight(metronomeLightL, metronomeLTimes);
				LightMetronomeLight(metronomeLightR, metronomeRTimes);
			}
			if (!canReceiveInput)
			{
				barbell.Update(ShowingScore);
				CheckBodyHeight();
				return;
			}
			_ = mobileForegroundOverlay.gameObject.activeSelf;
			bool leftPress = RDInput.leftPress;
			_ = RDInput.leftRelease;
			bool rightPress = RDInput.rightPress;
			_ = RDInput.rightRelease;
			bool cancelPress = RDInput.cancelPress;
			bool cancelIsPressed = RDInput.cancelIsPressed;
			bool cancelRelease = RDInput.cancelRelease;
			bool flag = RDInput.p1Press || (scnBase.isMobile && RDInput.rightPress);
			bool flag2 = RDInput.p1IsPressed || (scnBase.isMobile && RDInput.rightIsPressed);
			bool flag3 = RDInput.p1Release || (scnBase.isMobile && RDInput.rightRelease);
			bool flag4 = RDInput.p2Press || (scnBase.isMobile && RDInput.leftPress);
			bool flag5 = RDInput.p2IsPressed || (scnBase.isMobile && RDInput.leftIsPressed);
			bool flag6 = RDInput.p2Release || (scnBase.isMobile && RDInput.leftRelease);
			bool flag7 = flag2 & flag5;
			bool flag8 = flag | flag4;
			bool selectPress = RDInput.selectPress;
			bool selectIsPressed = RDInput.selectIsPressed;
			bool selectRelease = RDInput.selectRelease;
			switch (screen)
			{
			case Screen.Intro:
				if (cancelPress && !scnBase.isMobile)
				{
					canReceiveInput = false;
					DoSceneTransition(fadeIn: false, () =>
					{
						scnBase.GoToLevelSelect();
					});
					break;
				}
				if (flag7 && barbell.p1Weight.showing && barbell.p2Weight.showing)
				{
					barbell.p1Weight.AnimateOuterCollar();
					barbell.p2Weight.AnimateOuterCollar();
					SetScreen(finishedCampaign ? Screen.LevelSelect : Screen.Gameplay);
				}
				if (flag)
				{
					barbell.p1Weight.Toggle(show: true);
					flexHigh.Play();
				}
				else if (flag3)
				{
					barbell.p1Weight.Toggle(show: false);
					wobbleHigh.Play();
				}
				if (flag4)
				{
					barbell.p2Weight.Toggle(show: true);
					flexLow.Play();
				}
				else if (flag6)
				{
					barbell.p2Weight.Toggle(show: false);
					wobbleLow.Play();
				}
				if (KeyCode.LeftControl.IsPressed() && KeyCode.LeftShift.IsPressed() && KeyCode.R.WentDown())
				{
					ResetProgress();
					scnBase.GoToRhythmWeightlifter();
				}
				break;
			case Screen.LevelSelect:
				if (cancelRelease)
				{
					if (scnBase.isMobile)
					{
						SetScreen(Screen.Intro);
					}
					else
					{
						canReceiveInput = false;
						DoSceneTransition(fadeIn: false, () =>
						{
							scnBase.GoToLevelSelect();
						});
					}
				}
				if (currentExtraSlot >= 0)
				{
					ExtraContainer currentExtraContainer = CurrentExtraContainer;
					if (selectRelease && !_debugComboFiredThisFrame)
					{
						currentExtraContainer?.onSelect?.Invoke();
						RWPlaySound(DogMode ? "sndDogToyOut" : "sndDesktopBeepD5_6");
					}
					else
					{
						if (!(leftPress | rightPress) || selectIsPressed)
						{
							break;
						}
						bool num5 = (currentExtraSideIsRight ? leftPress : rightPress);
						bool flag9 = (currentExtraSideIsRight ? rightPress : leftPress);
						if (num5)
						{
							if (currentExtraSlot == 0)
							{
								ExitExtraContainersView();
								RWPlaySound(DogMode ? "sndDogToy" : "sndDesktopBeepD4_6");
							}
							else
							{
								ShowExtraContainer(currentExtraSideIsRight, currentExtraSlot - 1);
								RWPlaySound(DogMode ? "sndDogToy" : "sndDesktopBeepD4_6");
							}
						}
						else if (flag9)
						{
							List<ExtraContainer> list = VisibleSideContainers(currentExtraSideIsRight);
							if (currentExtraSlot < list.Count - 1)
							{
								ShowExtraContainer(currentExtraSideIsRight, currentExtraSlot + 1);
								RWPlaySound(DogMode ? "sndDogToy" : "sndDesktopBeepD4_6");
							}
						}
					}
				}
				else if ((flag8 && !scnBase.isMobile) || (selectRelease && !_debugComboFiredThisFrame))
				{
					SetScreen(Screen.Gameplay);
					RWPlaySound(DogMode ? "sndDogToyOut" : "sndDesktopBeepD5_6");
				}
				else
				{
					if (!(leftPress | rightPress) || selectIsPressed)
					{
						break;
					}
					if (leftPress && LevelIndex == 0 && VisibleSideContainers(right: false).Count > 0)
					{
						ShowExtraContainer(right: false, 0);
						RWPlaySound(DogMode ? "sndDogToy" : "sndDesktopBeepD4_6");
						break;
					}
					if (rightPress && LevelIndex == levels.Length - 1 && VisibleSideContainers(right: true).Count > 0)
					{
						ShowExtraContainer(right: true, 0);
						RWPlaySound(DogMode ? "sndDogToy" : "sndDesktopBeepD4_6");
						break;
					}
					int direction2 = ((!leftPress) ? 1 : (-1));
					if (ChangeLevel(direction2))
					{
						RWPlaySound(DogMode ? "sndDogToy" : "sndDesktopBeepD4_6");
					}
				}
				break;
			case Screen.Gameplay:
				if (paused)
				{
					if (cancelRelease)
					{
						SetPaused(value: false);
						RWPlaySound(DogMode ? "sndDogToyOut" : "sndDesktopBeepD4_6");
					}
					else if (leftPress | rightPress)
					{
						int direction = ((!leftPress) ? 1 : (-1));
						if (ChangePausedOption(direction))
						{
							RWPlaySound(DogMode ? "sndDogToy" : "sndDesktopBeepD4_6");
							SayCurrentPauseSelection();
						}
					}
					else if ((flag8 && !scnBase.isMobile) | selectRelease)
					{
						SelectPauseOption();
						RWPlaySound(DogMode ? "sndDogToy" : "sndDesktopBeepD5_6");
					}
				}
				else if (failure != Failure.None)
				{
					SetLevelFailed();
				}
				else if (ShowingScore)
				{
					if ((cancelRelease | selectRelease) && DialoguePlaying)
					{
						outGameInk.Stop();
					}
					else if (isInfiniteMode && (flag8 | selectRelease) && Responsive)
					{
						if (infiniteModeCurrentLives <= 0)
						{
							canReceiveInput = false;
							SetScreen(Screen.LevelSelect);
						}
						else
						{
							canReceiveInput = false;
							printe($"Starting round {currentInfiniteModeRound} with {infiniteModeCurrentLives} lives left.");
							SetScreen(Screen.Gameplay);
						}
					}
					else
					{
						if (!(flag8 | selectRelease) || !Responsive)
						{
							break;
						}
						canReceiveInput = false;
						ShowingScore = false;
						if (shouldShowEnding)
						{
							SetScreen(Screen.Ending);
							break;
						}
						if (!shouldAdvanceLevel || LevelIndex >= levels.Length - 1)
						{
							SetScreen(Screen.LevelSelect);
							break;
						}
						Flash(() =>
						{
							canReceiveInput = true;
							ResetGameplay(playWhistle: false);
							ChangeLevel(1);
						});
					}
				}
				else if (cancelRelease && DialoguePlaying)
				{
					if (!scnBase.isMobile)
					{
						outGameInk.Stop();
						inGameInk.Stop();
					}
				}
				else if (cancelRelease && Responsive)
				{
					SetPaused(value: true);
					RWPlaySound(DogMode ? "sndDogToyIn" : "sndDesktopBeepD4_6");
				}
				else
				{
					if (!Responsive)
					{
						break;
					}
					if (RDInput.faceLeftPress || (selectRelease && !_debugComboFiredThisFrame))
					{
						ToggleMetronome();
					}
					else
					{
						if (flag && !selectIsPressed)
						{
							barbell.Lift(p1: true, CurrentLevel.player1Weight);
						}
						if (flag4 && !selectIsPressed)
						{
							barbell.Lift(p1: false, CurrentLevel.player2Weight);
						}
					}
					if (barbell.completed)
					{
						ShowingScore = true;
					}
				}
				break;
			case Screen.Ending:
				if (!(flag8 | selectPress) || !endingComp.endingDownArrow.activeSelf)
				{
					break;
				}
				if (endingComp.resultDescription.gameObject.activeSelf)
				{
					endingComp.endingDownArrow.gameObject.SetActive(value: false);
					if (scnBase.isMobile)
					{
						canReceiveInput = false;
						string knotName = ((endingComp.playerHighScoreRank == 1) ? "ending_result_first" : "ending_result_other");
						Flash(() =>
						{
							endingComp.gameObject.SetActive(value: false);
							gameplay.SetActive(value: true);
							weightlifter.SetActive(value: true);
							DOVirtual.DelayedCall(1.5f, () =>
							{
								inGameInk.Run(knotName, () =>
								{
									gameplay.SetActive(value: false);
									SetScreen(Screen.Intro);
								});
							});
						});
					}
					else
					{
						SetScreen(Screen.Intro);
					}
				}
				else
				{
					endingComp.ShowReward();
				}
				break;
			case Screen.ExtraScreen:
				if (cancelRelease)
				{
					SetScreen(Screen.LevelSelect);
					RWPlaySound(DogMode ? "sndDogToyIn" : "sndDesktopBeepD4_6");
				}
				break;
			case Screen.LanguageSelection:
				if (flag2 & flag5 & selectIsPressed & cancelIsPressed)
				{
					SetScreen(Screen.DeleteData);
				}
				if (flag3 && !selectIsPressed)
				{
					ChangeLanguage(-1);
				}
				else if (flag6 && !selectIsPressed)
				{
					ChangeLanguage(1);
				}
				else if (selectRelease)
				{
					ConfirmLanguageSelection();
				}
				if (cancelRelease)
				{
					SetScreen(lastScreenCache);
					RDInputType_Touch.ResetAllVirtualButtons();
				}
				break;
			case Screen.DeleteData:
				if (selectPress)
				{
					ResetProgress();
					scnBase.GoToRhythmWeightlifter();
				}
				else if (cancelPress)
				{
					SetScreen(lastScreenCache);
					RDInputType_Touch.ResetAllVirtualButtons();
				}
				break;
			}
			barbell.Update(ShowingScore);
			CheckBodyHeight();
			void LightMetronomeLight(Image light, List<double> times)
			{
				light.sprite = metronomeLightOff;
				foreach (double time in times)
				{
					if (dspTime > time && dspTime < time + 0.07000000029802322)
					{
						light.sprite = metronomeLightOn;
						break;
					}
				}
			}
		}

		private void ChangeLanguage(int direction)
		{
			languageIndex += direction;
			string nativeLanguageName = RDString.GetNativeLanguageName(availableLanguages[languageIndex]);
			languageSelectableText.font = RDString.GetAppropiateFontForString(nativeLanguageName);
			languageSelectableText.text = "< " + nativeLanguageName + " >";
			RWPlaySound(DogMode ? "sndDogToy" : "sndDesktopBeepD4_6");
		}

		private void ConfirmLanguageSelection()
		{
			RWPlaySound(DogMode ? "sndDogToyOut" : "sndDesktopBeepD5_6");
			RDString.ChangeLanguage(availableLanguages[languageIndex]);
			scnBase.GoToRhythmWeightlifter();
		}

		private void LateUpdate()
		{
			int num = ((screen == Screen.Gameplay && !inGameInk.isPlaying) ? 1 : 0);
			maxLimit.color = maxLimit.color.WithAlpha(num);
			goalText.color = goalText.color.WithAlpha(num);
			backgroundCanvasCamera.transform.position = camera.transform.position * backgroundCanvasCamera.orthographicSize / camera.orthographicSize;
			resetInstructions.enabled = screen == Screen.Intro && Application.isEditor;
			Color endValue = (unbalancedWarning ? bucklingTintColor : Color.white);
			SpriteRenderer[] array = spritesAffectedByBuckling;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].DOColor(endValue, 0.2f);
			}
			if (DialoguePlaying)
			{
				lastTimeDialogueWasPlayed = Time.time;
			}
		}

		private void CalculateConsistencyScore()
		{
			int lastConsistencyP = Mathf.Clamp(barbell.p1Weight.CalculateConsistency(), 0, 50);
			int lastConsistencyP2 = Mathf.Clamp(barbell.p2Weight.CalculateConsistency(), 0, 50);
			Level.lastConsistencyP1 = lastConsistencyP;
			Level.lastConsistencyP2 = lastConsistencyP2;
		}

		private void CalculateSpeedScore()
		{
			float num = 0f;
			if (barbell.p1Weight.hitTimes.Count > 0 && barbell.p2Weight.hitTimes.Count > 0)
			{
				num = Mathf.Min(barbell.p1Weight.hitTimes.Last(), barbell.p2Weight.hitTimes.Last());
			}
			else
			{
				printe($"cheat code used: {barbell.p1Weight.hitTimes.Count}, {barbell.p2Weight.hitTimes.Count}");
			}
			float num2 = Time.time - num;
			int num3 = Mathf.Max(barbell.p1Weight.CurrentWeight, barbell.p2Weight.CurrentWeight);
			float num4 = (isInfiniteMode ? infiniteModeEffectiveBpm : CurrentLevel.bpm);
			float num5 = num4 / 220f;
			float num6 = (isInfiniteMode ? (50f * (220f / num4)) : 50f);
			float num7 = Mathf.Min(4.3f, (float)num3 * num5 * 1.333f);
			float num8 = (float)num3 * num5 * 0.666f;
			int num9 = 0;
			float num10;
			float num11;
			for (num10 = 0f; num10 <= 255f; num10 = num6 / (float)num3 * (num7 * (float)num9 / 60f) - num11)
			{
				num9++;
				num11 = (float)barbell.additiveWeight * 255f / 5000f * 0.8f * (float)num9;
			}
			float num12 = (float)num9 / 60f;
			num9 = 0;
			num10 = 0f;
			num11 = 0f;
			while (num10 <= 255f)
			{
				num9++;
				num11 = (float)barbell.additiveWeight * 255f / 5000f * 0.8f * (float)num9;
				num10 = num6 / (float)num3 * (num8 * (float)num9 / 60f) - num11;
			}
			float num13 = (float)num9 / 60f;
			Level.lastSpeed = Mathf.Clamp(Mathf.RoundToInt((num2 - num13) / (num12 - num13) * 100f), 0, 100);
		}

		private void ResetGameplay(bool playWhistle = true)
		{
			gameState = GameState.Playing;
			barbell.Reset();
			barbell.p1Weight.CurrentWeight = CurrentLevel.player1Weight;
			barbell.p2Weight.CurrentWeight = CurrentLevel.player2Weight;
			if (ambienceLoopAudioSrc == null)
			{
				printe("ambience created");
				ambienceLoopAudioSrc = RWPlaySound("sndCrowdAmbienceLoop");
				ambienceLoopAudioSrc.loop = true;
			}
			if (LevelIndex != lastLevelPlayed)
			{
				printe($"LevelIndex {LevelIndex} lastLevelPlayed {lastLevelPlayed}");
				if (LevelIndex == 0)
				{
					Narration.Say(RDString.Get("RhythmWeightlifter.desc1"), NarrationCategory.Description, false);
				}
				SayCurrentLevel();
				PlayLevelDialogue();
				Metronome = false;
			}
			else if (playWhistle)
			{
				PlayWhistleSound();
			}
			lastLevelPlayed = LevelIndex;
			announcerCaption.SetActive(value: false);
		}

		private bool ChangeLevel(int direction)
		{
			if (LevelIndex + direction < 0 || (LevelIndex + direction > lastUnlockedLevelIndex && lastUnlockedLevelIndex < levels.Length) || (LevelIndex == levels.Length - 1 && direction > 0))
			{
				return false;
			}
			LevelIndex += direction;
			lastLevelPlayed = LevelIndex;
			if (!isInfiniteMode)
			{
				screenP1Barbel10kgWeight.color = Color.white;
				screenP2Barbel10kgWeight.color = Color.white;
			}
			SayCurrentLevel(screen == Screen.LevelSelect);
			PlayLevelDialogue();
			return true;
		}

		private void PlayLevelDialogue(bool force = false)
		{
			if (LevelIndex < lastUnlockedLevelIndex && !force)
			{
				return;
			}
			string possibleKnot = "level" + LevelIndex;
			if (screen != Screen.Gameplay)
			{
				return;
			}
			if (inGameInk.story.HasFunction(possibleKnot))
			{
				inGameInk.SetPanelSide(RDInk.PanelSide.Top);
				inGameInk.Run(possibleKnot, () =>
				{
					string text = possibleKnot + "_0";
					if (!scnBase.isMobile && outGameInk.story.HasFunction(text))
					{
						outGameInk.Run(text, () =>
						{
							PlayWhistleSound();
						});
					}
					else
					{
						PlayWhistleSound();
						if (!scnBase.isMobile)
						{
							printe("possible knot " + text + " not found.");
						}
					}
				});
			}
			else
			{
				printe("knot " + possibleKnot + " doesn't exist!");
			}
		}

		private void PlayWhistleSound()
		{
			DOVirtual.DelayedCall(0.5f, () =>
			{
				RWPlaySound("sndSportsWhistle");
			});
		}

		private void SetLevelFailed()
		{
			ClearMetronomeSounds();
			RWPlaySound(DogMode ? "sndDogCry" : "sndCrowdAww");
			canReceiveInput = false;
			gameState = GameState.NoLift;
			if (isInfiniteMode)
			{
				infiniteModeCurrentLives--;
				UpdateInfiniteModeLivesUI();
				printe($"Lost a life, {infiniteModeCurrentLives} lives remaining.");
			}
			DOVirtual.DelayedCall(1f, () =>
			{
				Flash(() =>
				{
					failure = Failure.None;
					canReceiveInput = true;
					if (!isInfiniteMode || infiniteModeCurrentLives > 0)
					{
						ResetGameplay();
					}
					else
					{
						if (isInfiniteMode)
						{
							Persistence.SetRhythmWeightlifterBestInfiniteRound(currentInfiniteModeRound);
						}
						ShowingScore = true;
					}
				});
			});
		}

		private void Flash(Action midFlashAction, float animDuration = 0.5f, bool black = false)
		{
			foregroundFade.color = (black ? Color.black : Color.white).WithAlpha(0f);
			foregroundFade.DOKill();
			RWPlaySound("sndDesktopFlash", 0.5f);
			foregroundFade.DOFade(1f, animDuration).SetEase(Ease.Linear).OnComplete(() =>
			{
				if (midFlashAction != null)
				{
					midFlashAction();
				}
				foregroundFade.DOFade(0f, animDuration).SetEase(Ease.Linear);
			});
		}

		private void CheckBodyHeight()
		{
			if (ShowingScore)
			{
				return;
			}
			float num = barbell.maxY - barbell.minY;
			float num2 = (Mathf.Min(barbell.p1Y, barbell.p2Y) - barbell.minY) / num * 100f;
			HeadHeight headHeight = ((num2 <= 33f) ? HeadHeight.Low : ((num2 <= 66f) ? HeadHeight.Mid : HeadHeight.High));
			if (headCurrentHeight != headHeight)
			{
				headCurrentHeight = headHeight;
				float endValue = ((headCurrentHeight == HeadHeight.Low) ? torsoLowY : ((headCurrentHeight == HeadHeight.Mid) ? torsoMidY : torsoHighY));
				torso.DOKill();
				torso.DOLocalMoveY(endValue, torsoDuration).SetEase(Ease.Linear);
				if (headCurrentHeight == HeadHeight.Low || headCurrentHeight == HeadHeight.Mid)
				{
					float endValue2 = ((headCurrentHeight == HeadHeight.Low) ? leftFootOpenX : leftFootCloseX);
					float endValue3 = ((headCurrentHeight == HeadHeight.Low) ? rightFootOpenX : rightFootCloseX);
					leftFoot.DOKill();
					leftFoot.DOLocalMoveX(endValue2, footDuration).SetEase(Ease.Linear);
					rightFoot.DOKill();
					rightFoot.DOLocalMoveX(endValue3, footDuration).SetEase(Ease.Linear);
				}
			}
		}

		private bool ChangePausedOption(int direction)
		{
			pausedIcons[pausedOptionIndex].color = pausedIconUnselectedColor;
			pausedOptionIndex += direction;
			int num = pausedOptionIndex;
			int max = (scnBase.isMobile ? 3 : 2);
			pausedOptionIndex = Mathf.Clamp(pausedOptionIndex, 0, max);
			RectTransform rectTransform = pausedIcons[pausedOptionIndex].rectTransform.parent as RectTransform;
			pausedSelector.anchoredPosition = rectTransform.anchoredPosition;
			pausedIcons[pausedOptionIndex].color = pausedIconSelectedColor;
			for (int i = 0; i < pauseOptionTexts.Length; i++)
			{
				pauseOptionTexts[i].gameObject.SetActive(i == pausedOptionIndex);
			}
			return pausedOptionIndex == num;
		}

		private void SelectPauseOption()
		{
			if (pausedOptionIndex == 0)
			{
				SetPaused(value: false);
				gameState = GameState.Playing;
			}
			else if (pausedOptionIndex == 1)
			{
				ToggleMetronome();
				SayCurrentPauseSelection();
			}
			else if (pausedOptionIndex == 2)
			{
				if (isInfiniteMode && currentInfiniteModeRound > Persistence.GetBestInfiniteRound())
				{
					Persistence.SetRhythmWeightlifterBestInfiniteRound(currentInfiniteModeRound);
				}
				ShowGameplayScreen(show: false, showingScore: false);
				SetScreen(Screen.LevelSelect);
			}
			else if (pausedOptionIndex == 3)
			{
				SetScreen(Screen.LanguageSelection);
			}
		}

		public void CheckGameplayTextsFade()
		{
			if (!showingGameplayTexts && Mathf.Abs(barbell.p1Weight.y - barbell.minY) < 0.5f && Mathf.Abs(barbell.p2Weight.y - barbell.minY) < 0.5f)
			{
				showingGameplayTexts = true;
				barbell.p1Weight.UpdateText();
				barbell.p2Weight.UpdateText();
			}
			else if (showingGameplayTexts && (Mathf.Abs(barbell.p1Weight.y - barbell.minY) > 0.5f || Mathf.Abs(barbell.p2Weight.y - barbell.minY) > 0.5f))
			{
				showingGameplayTexts = false;
				barbell.p1Weight.text.DOKill();
				barbell.p1Weight.text.DOFade(0f, 0.3f);
				barbell.p2Weight.text.DOKill();
				barbell.p2Weight.text.DOFade(0f, 0.3f);
			}
		}

		private void ChangeCrowdAnimation(SpriteAnimation.SpriteAnimationData animData)
		{
			if (animData != crowdSpriteAnim.currentAnimationData)
			{
				crowdSpriteAnim.Stop();
				crowdSpriteAnim.currentAnimationData = animData;
				crowdSpriteAnim.Play();
			}
		}

		private void ShowGameplayScreen(bool show, bool showingScore)
		{
			canReceiveInput = false;
			camera.transform.DOKill();
			canvasContainer.DOKill();
			float num = (show ? 15.838f : 0f);
			float num2 = (showingScore ? 2f : 0.5f);
			float delay = (showingScore ? 0.45f : 0.1f);
			camera.transform.DOMoveY(num, num2).SetEase(Ease.OutExpo).SetDelay(delay);
			canvasContainer.DOAnchorPosY(num * -99f / 32f, num2).SetEase(Ease.OutExpo).SetDelay(delay);
			DOVirtual.DelayedCall(num2 * 0.75f, () =>
			{
				canReceiveInput = true;
			});
		}

		private void ResetProgress()
		{
			printe("progress reset!");
			lastUnlockedLevelIndex = 0;
			LevelIndex = 0;
			shouldShowEnding = false;
			Persistence.SetRhythmWeightlifterLastLevelUnlocked(0);
			for (int i = 0; i < levels.Length - 1; i++)
			{
				Persistence.SetRhythmWeightlifterSpeed(i, 0);
				Persistence.SetRhythmWeightlifterConsistency(i, 0);
				Persistence.SetRhythmWeightlifterScore(i, 0);
			}
			Persistence.SetRhythmWeightlifterBestInfiniteRound(0);
			Persistence.SaveCurrentSlot();
		}

		private void PlayRandomAnnouncerLine()
		{
			if (announcerPool == null || announcerPool.Count() == 0)
			{
				InitAnnouncerPool();
			}
			string voicelineID = announcerPool.Pop();
			DOVirtual.DelayedCall(0.8f, () =>
			{
				AudioSource audioSource = scrConductor.PlayImmediately("sndSportsAnnouncer-" + voicelineID, 1.4f, mixerGroup);
				announcerCaptionText.text = RDString.Get("rhythmWeightlifter.announcer" + voicelineID);
				announcerCaptionText.font = RDString.GetAppropiateFontForString(announcerCaptionText.text);
				announcerCaption.SetActive(value: true);
				LayoutRebuilder.ForceRebuildLayoutImmediate(announcerCaptionText.rectTransform);
				DOVirtual.DelayedCall(audioSource.clip.length + 1f, () =>
				{
					announcerCaption.SetActive(value: false);
					SayCurrentRank();
				});
			});
			void InitAnnouncerPool()
			{
				announcerPool = new List<string>();
				for (int i = 1; i < 13; i++)
				{
					announcerPool.Add(i.ToString("d3"));
				}
				announcerPool.Shuffle();
				RDBaseDllDummy.printem(string.Join(", ", announcerPool));
			}
		}

		public void SetPaused(bool value)
		{
			paused = value;
			Narration.Stop();
			ShowGameplayScreen(paused, showingScore: false);
			gameState = (paused ? GameState.Paused : GameState.Playing);
			if (paused)
			{
				if (ambienceLoopAudioSrc != null)
				{
					ambienceLoopAudioSrc.volume = 0.3f;
				}
			}
			else if (ambienceLoopAudioSrc == null)
			{
				ambienceLoopAudioSrc = RWPlaySound("sndCrowdAmbienceLoop");
				ambienceLoopAudioSrc.loop = true;
			}
			else
			{
				ambienceLoopAudioSrc.volume = 1f;
			}
			if (paused)
			{
				Narration.Set(RDString.Get("rhythmWeightlifter.paused"), 1, 3, ElementType.HorizontalList);
				SayCurrentPauseSelection();
			}
		}

		private void SetScreen(Screen v)
		{
			if (v == Screen.Gameplay)
			{
				RDBaseDllDummy.printesw("going to gameplay screen");
			}
			bool fromIntro = screen == Screen.Intro;
			bool fromLevelSelect = screen == Screen.LevelSelect;
			bool flag = screen == Screen.Ending;
			if ((v == screen) & fromIntro)
			{
				return;
			}
			if (fromLevelSelect)
			{
				infiniteModeWeightAnimTween?.Kill();
				infiniteModeWeightAnimTween = null;
				screenP1Barbel10kgWeight?.DOKill();
				screenP2Barbel10kgWeight?.DOKill();
				screenP1Barbel10kgWeight.color = Color.white;
				screenP2Barbel10kgWeight.color = Color.white;
				if (currentExtraSlot >= 0 && v != Screen.ExtraScreen)
				{
					pendingExtraSlot = currentExtraSlot;
					pendingExtraSideIsRight = currentExtraSideIsRight;
				}
			}
			Narration.StopSpeaking();
			if (screen != Screen.DeleteData && screen != Screen.LanguageSelection)
			{
				lastScreenCache = screen;
			}
			Metronome = false;
			if (v == Screen.Intro)
			{
				if (flag)
				{
					scrConductor.PlayImmediately("sndChampionshipIntro", 1f, RDUtils.GetMixerGroup("MenuMusic"));
				}
				if (ambienceLoopAudioSrc != null)
				{
					ambienceLoopAudioSrc.Stop();
				}
			}
			canReceiveInput = false;
			float delay = (fromIntro ? 1.25f : 0f);
			if (fromIntro)
			{
				foregroundFade.color = Color.white;
				foregroundFade.DOFade(0f, 0.3f).SetEase(Ease.Linear);
				RWPlaySound("sndCrowdCheer");
				if (ambienceLoopAudioSrc == null)
				{
					printe("ambience created");
					ambienceLoopAudioSrc = RWPlaySound("sndCrowdAmbienceLoop");
					ambienceLoopAudioSrc.loop = true;
				}
			}
			ChangeCrowdAnimation(neutralAnimData);
			if (v == Screen.Ending)
			{
				SpawnEnding();
			}
			RDRichPresence.Instance?.UpdatePresence();
			DOVirtual.DelayedCall(delay, () =>
			{
				Flash(() =>
				{
					canReceiveInput = true;
					screens[screen]?.SetActive(value: false);
					screen = v;
					if (screens.TryGetValue(screen, out var value) && value != null)
					{
						value.SetActive(value: true);
					}
					bool flag2 = false;
					if (screen == Screen.LevelSelect && lastScreenCache != Screen.ExtraScreen)
					{
						flag2 = pendingExtraSlot >= 0;
						currentExtraSlot = -1;
						isOnInfiniteExtra = false;
						StopInfiniteWeightAnimation();
						screenP1Barbel10kgWeight.DOKill();
						screenP2Barbel10kgWeight.DOKill();
						screenP1Barbel10kgWeight.color = Color.white;
						screenP2Barbel10kgWeight.color = Color.white;
						levelsContainer.DOKill();
						levelsContainer.anchoredPosition = new Vector2(-50f * (float)LevelIndex, levelsContainer.anchoredPosition.y);
					}
					if (screen != Screen.Gameplay && screen != Screen.ExtraScreen)
					{
						isInfiniteMode = false;
					}
					topDisplay.gameObject.SetActive(v == Screen.Gameplay);
					if (screen == Screen.Gameplay)
					{
						OnEnterGameplayScreen(fromLevelSelect, fromIntro);
					}
					else
					{
						OnEnterNonGameplayScreen();
					}
					if (flag2)
					{
						ShowExtraContainer(pendingExtraSideIsRight, pendingExtraSlot, snap: true);
						pendingExtraSlot = -1;
					}
				});
			});
		}

		private void OnEnterGameplayScreen(bool fromLevelSelect, bool fromIntro)
		{
			livesIndicators.gameObject.SetActive(isInfiniteMode);
			playersIndicators.gameObject.SetActive(!isInfiniteMode);
			if (isInfiniteMode)
			{
				OnEnterInfiniteModeGameplay(fromLevelSelect, fromIntro);
			}
			weightlifter.SetActive(value: true);
			barbell.p1Weight.text.gameObject.SetActive(value: true);
			barbell.p2Weight.text.gameObject.SetActive(value: true);
			ResetGameplay();
			SetPaused(value: false);
			gameState = GameState.Playing;
			ShowingScore = false;
			camera.transform.DOKill();
			canvasContainer.DOKill();
			camera.transform.DOMoveY(0f, 0f);
			canvasContainer.DOAnchorPosY(0f, 0f);
			shouldAdvanceLevel = false;
		}

		private void OnEnterInfiniteModeGameplay(bool fromLevelSelect, bool fromIntro)
		{
			if (fromLevelSelect | fromIntro)
			{
				infiniteModeCurrentLives = 3;
				currentInfiniteModeRound = 1;
				printe("setting lives to " + 3);
				hasObtainedBestRound = false;
				InitializeInfiniteModeLevels();
			}
			if (hasEarnedExtraLife)
			{
				extraLifeText.gameObject.SetActive(value: true);
				extraLifeText.text = extraLifeText.text.ToUpper();
				RectTransform rt = extraLifeText.rectTransform;
				Vector2 baseAnch = rt.anchoredPosition;
				Color baseColor = extraLifeText.color;
				baseColor.a = 1f;
				extraLifeText.color = baseColor;
				float duration = 2f;
				float fadeDuration = 0.3f;
				float sineAmplitude = 8f;
				int sineCycles = 3;
				float riseAmount = 30f;
				Tweener rainbowTween = DOVirtual.Float(0f, 1f, 1f, (float h) =>
				{
					float a = extraLifeText.color.a;
					Color color = Color.HSVToRGB(h % 1f, 1f, 1f);
					color.a = a;
					extraLifeText.color = color;
				}).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
				DOVirtual.Float(0f, 1f, duration, (float t) =>
				{
					float num2 = Mathf.Sin(t * MathF.PI * 2f * (float)sineCycles);
					rt.anchoredPosition = baseAnch + new Vector2(num2 * sineAmplitude, t * riseAmount);
				}).SetEase(Ease.Linear).OnComplete(() =>
				{
					DOVirtual.Float(extraLifeText.color.a, 0f, fadeDuration, (float a) =>
					{
						Color color = extraLifeText.color;
						color.a = a;
						extraLifeText.color = color;
					}).SetEase(Ease.Linear).OnComplete(() =>
					{
						if (rainbowTween.IsActive())
						{
							rainbowTween.Kill();
						}
						Color color = baseColor;
						color.a = 0f;
						extraLifeText.color = color;
						extraLifeText.gameObject.SetActive(value: false);
						extraLifeText.rectTransform.anchoredPosition = baseAnch;
					});
				});
				hasEarnedExtraLife = false;
			}
			UpdateInfiniteModeLivesUI();
			infiniteModeCurrentLevel = GetRandomInfiniteModeLevel();
			if (_debugForcedInfiniteLevel != null)
			{
				infiniteModeCurrentLevel = _debugForcedInfiniteLevel;
				_debugForcedInfiniteLevel = null;
				printe($"[INFINITE DEBUG] Forced level applied: p1={infiniteModeCurrentLevel.player1Weight} p2={infiniteModeCurrentLevel.player2Weight}");
			}
			float num = Mathf.Max(0f, (float)(currentInfiniteModeRound - 16) * 5f);
			infiniteModeEffectiveBpm = Mathf.Min(260f, infiniteModeCurrentLevel.bpm + num);
			ResetGameplay();
			printe("Restarting or continuing to next round in infinite mode.");
			string text = RDString.Get("rhythmWeightlifter.round").Replace("[roundNumber]", currentInfiniteModeRound.ToString());
			screenLevelNumberText.text = (hasObtainedBestRound ? ("<color=#FFFF00>" + text + "</color>") : (text ?? ""));
			weightP1Text.text = RDString.Get("rhythmWeightlifter.kgNumber").Replace("[weightNumber]", (infiniteModeCurrentLevel.player1Weight * 10).ToString());
			weightP2Text.text = RDString.Get("rhythmWeightlifter.kgNumber").Replace("[weightNumber]", (infiniteModeCurrentLevel.player2Weight * 10).ToString());
			screenWeightP1Text.text = (infiniteModeCurrentLevel.player1Weight * 10).ToString();
			screenWeightP2Text.text = (infiniteModeCurrentLevel.player2Weight * 10).ToString();
			gameplayWeightP1Text.text = weightP1Text.text;
			gameplayWeightP2Text.text = weightP2Text.text;
			livesIndicators.gameObject.SetActive(value: true);
			ShowingScore = false;
			ShowLivesIndicators(maximized: true);
			UpdateWeights();
			UpdateChars();
		}

		private void OnEnterNonGameplayScreen()
		{
			weightlifter.SetActive(value: false);
			barbell.p1Weight.text.gameObject.SetActive(value: false);
			barbell.p2Weight.text.gameObject.SetActive(value: false);
			switch (screen)
			{
			case Screen.Intro:
				barbell.p1Weight.Toggle(show: false);
				barbell.p2Weight.Toggle(show: false);
				break;
			case Screen.LevelSelect:
				OnEnterLevelSelectScreen();
				break;
			case Screen.Ending:
				endingComp.ShowScoreboard(CalculateFinalScore());
				shouldShowEnding = false;
				break;
			case Screen.LanguageSelection:
				OnEnterLanguageSelectionScreen();
				break;
			case Screen.Gameplay:
			case Screen.DeleteData:
				break;
			}
		}

		private void OnEnterLevelSelectScreen()
		{
			barbell.Reset();
			RefreshExtraContainersVisibility();
			LayoutExtraContainers();
			if (finishedCampaign)
			{
				StartInfiniteWeightAnimation();
			}
			RefreshStatsDisplay();
			int num = 0;
			Level[] array = levels;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].container.SetActive(num <= lastUnlockedLevelIndex);
				num++;
			}
			printe($"lastUnlockedLevelIndex {lastUnlockedLevelIndex} levels.Length {levels.Length}");
			finalScoreText.enabled = finishedCampaign;
			finalScoreText.text = RDString.Get("rhythmWeightlifter.totalScore").Replace("[score]", CalculateFinalScore().ToString());
			string text = RDString.Get("rhythmWeightlifter.selectLevel");
			if (finalScoreText.enabled)
			{
				text = text + Narration.ShortPause + finalScoreText.text;
			}
			Narration.Set(text, LevelIndex + 1, lastUnlockedLevelIndex + 1, ElementType.HorizontalList);
			SayCurrentLevel(levelSelect: true);
			ShowGameplayScreen(show: false, showingScore: false);
		}

		private void StartInfiniteWeightAnimation()
		{
			Image leftImage;
			Image rightImage;
			float colorSnapStep;
			if (!(infiniteModeLevelContainerLeftWeight == null) && !(infiniteModeLevelContainerRightWeight == null))
			{
				leftImage = infiniteModeLevelContainerLeftWeight.GetComponent<Image>();
				rightImage = infiniteModeLevelContainerRightWeight.GetComponent<Image>();
				colorSnapStep = 0.25f;
				infiniteModeLevelContainerLeftWeight.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Snap(19f));
				infiniteModeLevelContainerRightWeight.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Snap(3f));
				leftImage?.DOKill();
				rightImage?.DOKill();
				screenP1Barbel10kgWeight?.DOKill();
				screenP2Barbel10kgWeight?.DOKill();
				if (leftImage != null)
				{
					leftImage.color = PickColor();
				}
				if (rightImage != null)
				{
					rightImage.color = PickColor();
				}
				if (isOnInfiniteExtra)
				{
					screenP1Barbel10kgWeight.color = PickColor();
					screenP2Barbel10kgWeight.color = PickColor();
				}
				infiniteModeWeightAnimTween?.Kill();
				float leftHeightVal = Snap(19f);
				float rightHeightVal = Snap(3f);
				infiniteModeWeightAnimTween = DOTween.Sequence().Append(DOTween.To(() => leftHeightVal, (float v) =>
				{
					leftHeightVal = Snap(v);
					infiniteModeLevelContainerLeftWeight.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, leftHeightVal);
				}, 3f, 0.75f).SetEase(Ease.InOutSine)).Join(DOTween.To(() => rightHeightVal, (float v) =>
				{
					rightHeightVal = Snap(v);
					infiniteModeLevelContainerRightWeight.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rightHeightVal);
				}, 19f, 0.75f).SetEase(Ease.InOutSine))
					.AppendCallback(() =>
					{
						ApplyLeft(PickColor());
						ApplyRight(PickColor());
					})
					.Append(DOTween.To(() => leftHeightVal, (float v) =>
					{
						leftHeightVal = Snap(v);
						infiniteModeLevelContainerLeftWeight.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, leftHeightVal);
					}, 19f, 0.75f).SetEase(Ease.InOutSine))
					.Join(DOTween.To(() => rightHeightVal, (float v) =>
					{
						rightHeightVal = Snap(v);
						infiniteModeLevelContainerRightWeight.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rightHeightVal);
					}, 3f, 0.75f).SetEase(Ease.InOutSine))
					.AppendCallback(() =>
					{
						ApplyLeft(PickColor());
						ApplyRight(PickColor());
					})
					.SetLoops(-1);
			}
			void ApplyLeft(Color c)
			{
				ChunkyColorImage(leftImage, c);
				if (isOnInfiniteExtra)
				{
					ChunkyColorSprite(screenP2Barbel10kgWeight, c);
				}
			}
			void ApplyRight(Color c)
			{
				ChunkyColorImage(rightImage, c);
				if (isOnInfiniteExtra)
				{
					ChunkyColorSprite(screenP1Barbel10kgWeight, c);
				}
			}
			void ChunkyColorImage(Image img, Color to)
			{
				if (!(img == null))
				{
					img.DOKill();
					Color from = img.color;
					DOTween.To(() => 0f, (float t) =>
					{
						img.color = Color.Lerp(from, to, Mathf.Round(t / colorSnapStep) * colorSnapStep);
					}, 1f, 0.75f).SetTarget(img);
				}
			}
			void ChunkyColorSprite(SpriteRenderer sr, Color to)
			{
				if (!(sr == null))
				{
					sr.DOKill();
					Color from = sr.color;
					DOTween.To(() => 0f, (float t) =>
					{
						sr.color = Color.Lerp(from, to, Mathf.Round(t / colorSnapStep) * colorSnapStep);
					}, 1f, 0.75f).SetTarget(sr);
				}
			}
			static Color PickColor()
			{
				return InfiniteWeightColors[UnityEngine.Random.Range(0, InfiniteWeightColors.Length)];
			}
			static float Snap(float v)
			{
				return Mathf.Round(v / 4f) * 4f;
			}
		}

		private void OnEnterLanguageSelectionScreen()
		{
			string nativeLanguageName = RDString.GetNativeLanguageName(availableLanguages[languageIndex]);
			languageSelectableText.font = RDString.GetAppropiateFontForString(nativeLanguageName);
			languageSelectableText.text = "< " + nativeLanguageName + " >";
		}

		public void ShowPlayerIndicators(bool maximized)
		{
			float num = (maximized ? 152f : 54f);
			Vector2 endValue = playersIndicators.sizeDelta.WithX(num);
			playersIndicators.DOSizeDelta(endValue, 0.3f).SetEase(Ease.OutExpo);
			leftCharacterText.enabled = maximized;
			rightCharacterText.enabled = maximized;
			playersIndicators.GetComponent<CanvasGroup>().DOFade(ShowingScore ? 0f : 1f, 0.3f).SetEase(Ease.OutExpo);
		}

		public void ShowLivesIndicators(bool maximized)
		{
			livesText.gameObject.SetActive(maximized);
			CanvasGroup component = livesIndicators.GetComponent<CanvasGroup>();
			printe($"Showing Score {ShowingScore}");
			component.DOFade(ShowingScore ? 0f : 1f, 0.3f).SetEase(Ease.OutExpo);
		}

		private void ClearMetronomeSounds()
		{
			metronomeLSound.Unschedule();
			metronomeRSound.Unschedule();
			metronomeLTimes.Clear();
			metronomeRTimes.Clear();
			metronomeLightL.sprite = metronomeLightOff;
			metronomeLightR.sprite = metronomeLightOff;
		}

		private void ToggleMetronome(bool playSound = true)
		{
			Metronome = !Metronome;
			if (scnBase.isMobile)
			{
				lightLeft.sprite = (Metronome ? lightLeftOn : lightLeftOff);
				lightLeft.DOKill();
				if (Metronome)
				{
					lightLeft.color = Color.white;
					lightLeft.DOColor(new Color(0.9f, 0.9f, 0.9f), 0.25f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.OutBack);
				}
			}
			if (playSound)
			{
				RWPlaySound(Metronome ? "sndLightsEnter" : "sndLightsExit");
			}
		}

		private void SpawnEnding()
		{
			if (endingComp != null)
			{
				UnityEngine.Object.Destroy(endingComp.gameObject);
			}
			endingComp = UnityEngine.Object.Instantiate(endingPrefab, endingPrefab.transform.parent, worldPositionStays: true).GetComponent<Ending>();
			endingComp.transform.SetSiblingIndex(endingPrefab.transform.GetSiblingIndex());
			screens[Screen.Ending] = endingComp.gameObject;
		}

		public static Rank GetFinalRank(int score)
		{
			int i;
			for (i = 0; i < highScores.Length && score < highScores[i].score; i++)
			{
			}
			return i switch
			{
				0 => 15, 
				1 => 14, 
				2 => 13, 
				3 => 12, 
				4 => 11, 
				_ => 11, 
			};
		}

		private int CalculateFinalScore()
		{
			int num = 0;
			for (int i = 0; i < levels.Length; i++)
			{
				num += Persistence.GetRhythmWeightlifterScore(i);
			}
			return num;
		}

		private void RefreshStatsDisplay()
		{
			levelNumberText.text = RDString.Get("rhythmWeightlifter.level").Replace("[levelNumber]", (_levelIndex + 1).ToString());
			levelNumberText.font = RDString.GetAppropiateFontForString(levelNumberText.text);
			consistencyText.enabled = true;
			speedText.enabled = true;
			weightP1Text.enabled = true;
			weightP2Text.enabled = true;
			consistencyText.text = RDString.Get("rhythmWeightlifter.rhythmPercent").Replace("[percent]", CurrentLevel.consistency.ToString());
			speedText.text = RDString.Get("rhythmWeightlifter.speedPercent").Replace("[percent]", CurrentLevel.speed.ToString());
			scoreText.text = RDString.Get("rhythmWeightlifter.score").Replace("[score]", CurrentLevel.score + "/200");
			SetScoreTextStatsLayout();
			rankText.gameObject.SetActive(value: true);
			rankLetterText.gameObject.SetActive(value: true);
			rankLetterText.text = Level.GetRank(CurrentLevel.score).ToString();
			medalImage.gameObject.SetActive(value: true);
			Image image = medalImage;
			image.sprite = rankLetterText.text switch
			{
				"S" => perfectMedalSprite, 
				"A" => goldMedalSprite, 
				"B" => silverMedalSprite, 
				_ => bronzeMedalSprite, 
			};
		}

		private void SetScoreTextStatsLayout()
		{
			RectTransform rectTransform = scoreText.rectTransform;
			Vector2 anchoredPosition = rectTransform.anchoredPosition;
			anchoredPosition.y = -19f;
			rectTransform.anchoredPosition = anchoredPosition;
			Vector2 sizeDelta = rectTransform.sizeDelta;
			sizeDelta.y = 16f;
			rectTransform.sizeDelta = sizeDelta;
		}

		private void SetScoreTextSingleLineLayout()
		{
			RectTransform rectTransform = scoreText.rectTransform;
			Vector2 anchoredPosition = rectTransform.anchoredPosition;
			anchoredPosition.y = 0f;
			rectTransform.anchoredPosition = anchoredPosition;
			Vector2 sizeDelta = rectTransform.sizeDelta;
			sizeDelta.y = 40f;
			rectTransform.sizeDelta = sizeDelta;
		}

		private bool IsExtraEligible(ExtraContainer e)
		{
			if (e == null || e.container == null)
			{
				return false;
			}
			if (e.isOnlyMobile && !scnBase.isMobile)
			{
				return false;
			}
			return e.visible;
		}

		private List<ExtraContainer> VisibleSideContainers(bool right)
		{
			List<ExtraContainer> list = new List<ExtraContainer>();
			ExtraContainer[] array = (right ? rightExtraContainers : leftExtraContainers);
			if (array == null)
			{
				return list;
			}
			ExtraContainer[] array2 = array;
			foreach (ExtraContainer extraContainer in array2)
			{
				if (IsExtraEligible(extraContainer))
				{
					list.Add(extraContainer);
				}
			}
			return list;
		}

		private void RefreshExtraContainersVisibility()
		{
			foreach (ExtraContainer allExtraContainer in AllExtraContainers)
			{
				if (allExtraContainer != null)
				{
					if (allExtraContainer.requiresFinishedCampaign && !finishedCampaign)
					{
						allExtraContainer.visible = false;
					}
					else
					{
						allExtraContainer.visible = true;
					}
				}
			}
		}

		private void LayoutExtraContainers()
		{
			foreach (ExtraContainer allExtraContainer in AllExtraContainers)
			{
				if (allExtraContainer?.container != null)
				{
					allExtraContainer.container.gameObject.SetActive(IsExtraEligible(allExtraContainer));
				}
			}
			if (levels == null || levels.Length == 0)
			{
				return;
			}
			int num = Mathf.Clamp(lastUnlockedLevelIndex, 0, levels.Length - 1);
			RectTransform rectTransform = levels[0]?.container?.GetComponent<RectTransform>();
			RectTransform rectTransform2 = levels[num]?.container?.GetComponent<RectTransform>();
			if (!(rectTransform == null) && !(rectTransform2 == null))
			{
				float num2 = rectTransform.anchoredPosition.x;
				float num3 = rectTransform2.anchoredPosition.x;
				List<ExtraContainer> list = VisibleSideContainers(right: false);
				for (int i = 0; i < list.Count; i++)
				{
					RectTransform container = list[i].container;
					container.anchoredPosition = new Vector2(num2 - (float)(i + 1) * 50f, container.anchoredPosition.y);
				}
				List<ExtraContainer> list2 = VisibleSideContainers(right: true);
				for (int j = 0; j < list2.Count; j++)
				{
					RectTransform container2 = list2[j].container;
					container2.anchoredPosition = new Vector2(num3 + (float)(j + 1) * 50f, container2.anchoredPosition.y);
				}
			}
		}

		private void ShowExtraContainer(bool right, int slot, bool snap = false)
		{
			List<ExtraContainer> list = VisibleSideContainers(right);
			if (list.Count != 0)
			{
				slot = Mathf.Clamp(slot, 0, list.Count - 1);
				currentExtraSideIsRight = right;
				currentExtraSlot = slot;
				ExtraContainer extraContainer = list[slot];
				int num = Mathf.Clamp(lastUnlockedLevelIndex, 0, levels.Length - 1);
				float endValue = (right ? (-50f * (float)(num + 1 + slot)) : (50f * (float)(slot + 1)));
				levelsContainer.DOKill();
				if (snap)
				{
					levelsContainer.anchoredPosition = new Vector2(endValue, levelsContainer.anchoredPosition.y);
				}
				else
				{
					levelsContainer.DOAnchorPosX(endValue, 0.2f).SetEase(Ease.OutExpo);
				}
				ApplyExtraContainerDisplay(extraContainer);
				extraContainer.onShow?.Invoke();
			}
		}

		private void SetBarbellPlateWeights(int p1, int p2)
		{
			barbell.p1Weight.CurrentWeight = 0;
			barbell.p1Weight.CurrentWeight = p1;
			barbell.p2Weight.CurrentWeight = 0;
			barbell.p2Weight.CurrentWeight = p2;
		}

		private void RestoreWeightTextsFromCurrentLevel()
		{
			weightP1Text.text = RDString.Get("rhythmWeightlifter.kgNumber").Replace("[weightNumber]", (CurrentLevel.player1Weight * 10).ToString());
			weightP2Text.text = RDString.Get("rhythmWeightlifter.kgNumber").Replace("[weightNumber]", (CurrentLevel.player2Weight * 10).ToString());
			weightP1Text.rectTransform.anchoredPosition = weightTextPositions[CurrentLevel.player1Weight - 1] * new Vector2(-1f, 1f);
			weightP2Text.rectTransform.anchoredPosition = weightTextPositions[CurrentLevel.player2Weight - 1];
		}

		private void ExitExtraContainersView()
		{
			currentExtraSlot = -1;
			isOnInfiniteExtra = false;
			scoreText.transform.parent.gameObject.SetActive(value: true);
			if (levelSelectLineDetail != null)
			{
				levelSelectLineDetail.SetActive(value: true);
			}
			levelsContainer.DOKill();
			levelsContainer.DOAnchorPosX(-50f * (float)LevelIndex, 0.2f).SetEase(Ease.OutExpo);
			screenP1Barbel10kgWeight.DOKill();
			screenP2Barbel10kgWeight.DOKill();
			screenP1Barbel10kgWeight.color = Color.white;
			screenP2Barbel10kgWeight.color = Color.white;
			RestoreWeightTextsFromCurrentLevel();
			SetBarbellPlateWeights(CurrentLevel.player1Weight, CurrentLevel.player2Weight);
			SetBarbellWeightsVisible(visible: true);
			RefreshStatsDisplay();
			SayCurrentLevel(levelSelect: true);
		}

		private void ApplyExtraContainerDisplay(ExtraContainer ct)
		{
			isOnInfiniteExtra = false;
			RestoreWeightTextsFromCurrentLevel();
			levelNumberText.text = RDString.Get(ct.titleKey);
			levelNumberText.font = RDString.GetAppropiateFontForString(levelNumberText.text);
			bool flag = !string.IsNullOrEmpty(ct.bodyKey);
			scoreText.transform.parent.gameObject.SetActive(flag);
			levelSelectLineDetail.SetActive(flag);
			if (flag)
			{
				scoreText.text = RDString.Get(ct.bodyKey);
			}
			SetScoreTextSingleLineLayout();
			rankText.gameObject.SetActive(value: false);
			rankLetterText.gameObject.SetActive(value: false);
			medalImage.gameObject.SetActive(value: false);
			consistencyText.enabled = false;
			speedText.enabled = false;
			SetBarbellWeightsVisible(visible: false);
		}

		private void SetBarbellWeightsVisible(bool visible)
		{
			weightP1Text.enabled = visible;
			weightP2Text.enabled = visible;
			screenP1Barbel10kgWeight.enabled = visible;
			screenP2Barbel10kgWeight.enabled = visible;
			if (visible)
			{
				UpdateWeights();
				return;
			}
			GameObject[] array = p1Weights;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: false);
			}
			array = p2Weights;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: false);
			}
			array = barbell.p1Weight.weights;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: false);
			}
			array = barbell.p2Weight.weights;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: false);
			}
		}

		public void ShowExtraScreen(GameObject screenGO)
		{
			if (!(screenGO == null))
			{
				screens[Screen.ExtraScreen] = screenGO;
				SetScreen(Screen.ExtraScreen);
			}
		}

		public void StartInfiniteMode()
		{
			isInfiniteMode = true;
			SetScreen(Screen.Gameplay);
		}

		public void RefreshInfiniteExtraBody()
		{
			isOnInfiniteExtra = true;
			scoreText.text = RDString.Get("rhythmWeightlifter.bestInfiniteRound").Replace("[roundNumber]", Persistence.GetBestInfiniteRound().ToString());
			scoreText.transform.parent.gameObject.SetActive(value: true);
			levelSelectLineDetail.SetActive(value: true);
			SetScoreTextSingleLineLayout();
		}

		public void PlayInfiniteEntryAnimation()
		{
			isOnInfiniteExtra = true;
			weightP1Text.enabled = true;
			weightP2Text.enabled = true;
			weightP1Text.text = "?";
			weightP2Text.text = "?";
			Vector2 vector = new Vector2(82f, 67f);
			weightP1Text.rectTransform.anchoredPosition = vector;
			weightP2Text.rectTransform.anchoredPosition = vector * new Vector2(-1f, 1f);
			screenP1Barbel10kgWeight.enabled = true;
			screenP2Barbel10kgWeight.enabled = true;
			for (int i = 0; i < p1Weights.Length; i++)
			{
				p1Weights[i].SetActive(i == 0);
			}
			for (int j = 0; j < p2Weights.Length; j++)
			{
				p2Weights[j].SetActive(j == 0);
			}
			SetBarbellPlateWeights(1, 1);
			StartInfiniteWeightAnimation();
		}

		private void StopInfiniteWeightAnimation()
		{
			infiniteModeWeightAnimTween?.Kill();
			infiniteModeWeightAnimTween = null;
		}

		private void DoSceneTransition(bool fadeIn, TweenCallback callback = null)
		{
			float duration = (fadeIn ? 1f : 1f);
			pixelateOverlay.time = (fadeIn ? 8f : 0f);
			sceneTransitionTween = DOTween.To(() => pixelateOverlay.time, (float t) =>
			{
				pixelateOverlay.time = t;
			}, fadeIn ? 0f : 8f, duration).OnComplete(callback);
			if (fadeIn)
			{
				Tween tween = sceneTransitionTween;
				tween.OnComplete(() =>
				{
					pixelateOverlay.enabled = false;
				});
			}
			else
			{
				pixelateOverlay.enabled = true;
			}
			fadeOverlay.color = fadeOverlay.color.WithAlpha(fadeIn ? 1f : 0f);
			fadeOverlay.DOFade(fadeIn ? 0f : 1f, duration).SetEase(fadeIn ? Ease.InQuad : Ease.OutQuad);
			scrConductor.PlayImmediatelyIgnoringTimeScale("sndTransition" + (fadeIn ? "In" : "Out"), 1f, RDUtils.GetMixerGroup("LevelInterface"));
		}

		private AudioSource RWPlaySound(string sound, float volume = 1f, float pitch = 1f)
		{
			return scrConductor.PlayImmediately(sound, volume, mixerGroup, pitch);
		}

		private void SetupResolutionForMobile()
		{
			if (!scnBase.isMobile)
			{
				return;
			}
			float num = UnityEngine.Screen.height;
			float num2 = UnityEngine.Screen.width;
			Vector2 vector = new Vector2(num2, num);
			if (lastScreenSize == vector)
			{
				return;
			}
			lastScreenSize = vector;
			float num3 = UnityEngine.Screen.safeArea.width / num;
			Camera[] allCameras = Camera.allCameras;
			foreach (Camera camera in allCameras)
			{
				if (camera.orthographic)
				{
					float num4 = ((camera == this.camera) ? 32f : 99f);
					if (num3 < 1.7777778f)
					{
						float num5 = 1.7777778f / num3;
						printe($"{camera.name} -> Resolution scale adjusted for narrow screen, resolution {num2}x{num}, aspect {num3}");
						camera.orthographicSize = num4 * num5;
						foregroundCanvas.sizeDelta = new Vector2(352f, 198f);
						backgroundCanvas.sizeDelta = new Vector2(266f, backgroundCanvas.sizeDelta.y);
						foregroundLeftOverlay.sizeDelta = new Vector2(37f, foregroundLeftOverlay.sizeDelta.y);
						foregroundRightOverlay.sizeDelta = new Vector2(37f, foregroundRightOverlay.sizeDelta.y);
						foregroundCenterBorders.sizeDelta = new Vector2(278f, foregroundCenterBorders.sizeDelta.y);
						foregroundTopBorder.sizeDelta = new Vector2(278f, foregroundTopBorder.sizeDelta.y);
						foregroundBottomBorder.sizeDelta = new Vector2(278f, foregroundBottomBorder.sizeDelta.y);
					}
					else
					{
						camera.orthographicSize = num4;
						printe($"{camera.name} -> Resolution scale adjusted for wide screens, resolution {num2}x{num}, aspect {num3}");
						float num6 = num3 / 1.7777778f;
						foregroundCanvas.sizeDelta = new Vector2(352f * num6, 198f);
						float num7 = 266f * num6;
						backgroundCanvas.sizeDelta = new Vector2(num7, backgroundCanvas.sizeDelta.y);
						float value = (352f * num6 - 278f) / 2f;
						value = Mathf.Clamp(value, 0f, 70f);
						foregroundLeftOverlay.sizeDelta = new Vector2(value, foregroundLeftOverlay.sizeDelta.y);
						foregroundRightOverlay.sizeDelta = new Vector2(value, foregroundRightOverlay.sizeDelta.y);
						float num8 = 352f * num6 - value * 2f;
						foregroundCenterBorders.sizeDelta = new Vector2(num8, foregroundCenterBorders.sizeDelta.y);
						foregroundTopBorder.sizeDelta = new Vector2(num8, foregroundTopBorder.sizeDelta.y);
						foregroundBottomBorder.sizeDelta = new Vector2(num8, foregroundBottomBorder.sizeDelta.y);
					}
				}
			}
			mobileDecorationCablesLR.gameObject.SetActive(num3 > 1.77f);
			mobileDecorationSliderLeft.gameObject.SetActive(num3 > 1.96f);
			mobileDecorationKnobLeft.gameObject.SetActive(num3 > 1.96f);
			mobileDecorationHeartMonitor.gameObject.SetActive(num3 > 2.06f);
			mobileDecorationRhythmBoyLogo.gameObject.SetActive(num3 > 1.96f);
		}

		private void InitializeInfiniteModeLevels()
		{
			easyLevels = new List<Level>
			{
				new Level(1, 1, LevelCharacterName.ian, LevelCharacterName.ian),
				new Level(1, 2, LevelCharacterName.ian, LevelCharacterName.ian),
				new Level(2, 1, LevelCharacterName.ian, LevelCharacterName.ian),
				new Level(2, 2, LevelCharacterName.ian, LevelCharacterName.ian)
			};
			mediumLevels = new List<Level>
			{
				new Level(3, 3, LevelCharacterName.ian, LevelCharacterName.ian),
				new Level(2, 3, LevelCharacterName.ian, LevelCharacterName.ian),
				new Level(3, 2, LevelCharacterName.ian, LevelCharacterName.ian),
				new Level(4, 4, LevelCharacterName.ian, LevelCharacterName.ian),
				new Level(1, 3, LevelCharacterName.ian, LevelCharacterName.ian),
				new Level(3, 1, LevelCharacterName.ian, LevelCharacterName.ian)
			};
			hardLevels = new List<Level>
			{
				new Level(4, 4, LevelCharacterName.ian, LevelCharacterName.ian),
				new Level(4, 2, LevelCharacterName.ian, LevelCharacterName.ian),
				new Level(2, 4, LevelCharacterName.ian, LevelCharacterName.ian),
				new Level(4, 3, LevelCharacterName.ian, LevelCharacterName.ian),
				new Level(3, 4, LevelCharacterName.ian, LevelCharacterName.ian),
				new Level(5, 5, LevelCharacterName.ian, LevelCharacterName.ian, 185f),
				new Level(5, 1, LevelCharacterName.ian, LevelCharacterName.ian, 185f),
				new Level(1, 5, LevelCharacterName.ian, LevelCharacterName.ian, 185f)
			};
			extremeLevels = new List<Level>
			{
				new Level(4, 3, LevelCharacterName.ian, LevelCharacterName.ian),
				new Level(3, 4, LevelCharacterName.ian, LevelCharacterName.ian),
				new Level(5, 5, LevelCharacterName.ian, LevelCharacterName.ian, 185f),
				new Level(5, 2, LevelCharacterName.ian, LevelCharacterName.ian, 185f),
				new Level(2, 5, LevelCharacterName.ian, LevelCharacterName.ian, 185f),
				new Level(5, 3, LevelCharacterName.ian, LevelCharacterName.ian, 185f),
				new Level(3, 5, LevelCharacterName.ian, LevelCharacterName.ian, 185f),
				new Level(6, 6, LevelCharacterName.ian, LevelCharacterName.ian, 165f),
				new Level(6, 1, LevelCharacterName.ian, LevelCharacterName.ian, 165f),
				new Level(1, 6, LevelCharacterName.ian, LevelCharacterName.ian, 165f),
				new Level(6, 2, LevelCharacterName.ian, LevelCharacterName.ian, 165f),
				new Level(2, 6, LevelCharacterName.ian, LevelCharacterName.ian, 165f)
			};
			Shuffle(easyLevels);
			Shuffle(mediumLevels);
			Shuffle(hardLevels);
			Shuffle(extremeLevels);
			easyIndex = (mediumIndex = (hardIndex = (extremeIndex = 0)));
			previousInfiniteModeLevel = null;
		}

		private void Shuffle(List<Level> list)
		{
			for (int num = list.Count - 1; num > 0; num--)
			{
				int num2 = UnityEngine.Random.Range(0, num + 1);
				int index = num;
				int index2 = num2;
				Level level = list[num2];
				Level level2 = list[num];
				Level level3 = (list[index] = level);
				level3 = (list[index2] = level2);
			}
		}

		private Level NextFromPool(List<Level> pool, ref int index)
		{
			if (index >= pool.Count)
			{
				Shuffle(pool);
				index = 0;
				if (pool.Count > 1 && pool[0].player1Weight == previousInfiniteModeLevel?.player1Weight && pool[0].player2Weight == previousInfiniteModeLevel?.player2Weight)
				{
					int index2 = pool.Count - 1;
					Level level = pool[pool.Count - 1];
					Level level2 = pool[0];
					Level level3 = (pool[0] = level);
					level3 = (pool[index2] = level2);
				}
			}
			return pool[index++];
		}

		private void RemoveAndAdjustIndex(List<Level> pool, Level level, ref int index)
		{
			int num = pool.IndexOf(level);
			if (num >= 0)
			{
				pool.RemoveAt(num);
				if (num < index)
				{
					index--;
				}
			}
		}

		private Level GetRandomInfiniteModeLevel()
		{
			Level level = ((currentInfiniteModeRound <= 5) ? NextFromPool(easyLevels, ref easyIndex) : ((currentInfiniteModeRound <= 11) ? NextFromPool(mediumLevels, ref mediumIndex) : ((currentInfiniteModeRound > 17) ? NextFromPool(extremeLevels, ref extremeIndex) : NextFromPool(hardLevels, ref hardIndex))));
			if (level.player1Weight == level.player2Weight)
			{
				RemoveAndAdjustIndex(easyLevels, level, ref easyIndex);
				RemoveAndAdjustIndex(mediumLevels, level, ref mediumIndex);
				RemoveAndAdjustIndex(hardLevels, level, ref hardIndex);
				RemoveAndAdjustIndex(extremeLevels, level, ref extremeIndex);
			}
			previousInfiniteModeLevel = level;
			return level;
		}

		private void UpdateInfiniteModeLivesUI()
		{
			int num = infiniteModeCurrentLives;
			foreach (Transform item in infiniteModeHeartsContainer)
			{
				Image component = item.GetComponent<Image>();
				if (component != null)
				{
					component.sprite = ((num > 0) ? heartFullSprite : heartEmptySprite);
					num--;
				}
			}
		}

		private void SayCurrentLevel(bool levelSelect = false)
		{
			bool flag = !levelSelect && LevelIndex == 0;
			string text = RDString.Get("narration.rhythmWeightlifter." + (flag ? "weightsLong" : "weights")).Replace("[leftWeight]", (CurrentLevel.player2Weight * 10).ToString()).Replace("[rightWeight]", (CurrentLevel.player1Weight * 10).ToString());
			List<string> values = new List<string>
			{
				RDString.Get("rhythmWeightlifter.level").Replace("[levelNumber]", (LevelIndex + 1).ToString()),
				text
			};
			Narration.Say(string.Join(Narration.ShortPause, values), levelSelect ? NarrationCategory.Navigation : NarrationCategory.Description, _levelIndex + 1, lastUnlockedLevelIndex + 1, ElementType.SelectableEntity, null, "", false);
			if (levelSelect)
			{
				SayCurrentRank(levelSelect: true);
			}
			if (levelSelect)
			{
				Narration.SetInstructions("", NarrationActionName.ToSelect, NarrationActionName.ToNavigateHorizontal, NarrationActionName.ToExit);
			}
			else
			{
				Narration.SetInstructions(text + Narration.ShortPause + RDString.Get("narration.rhythmWeightlifter.gameInstruction"), NarrationActionName.ToExit);
			}
		}

		private void SayCurrentRank(bool levelSelect = false)
		{
			int num = (levelSelect ? CurrentLevel.speed : Level.lastSpeed);
			int num2 = (levelSelect ? CurrentLevel.consistency : Level.lastConsistency);
			int score = (levelSelect ? CurrentLevel.score : Level.lastScore);
			string text = Level.GetRank(score).Replace("-", "NotFinished");
			string newValue = RDString.Get("narration.rank." + text);
			string[] array = null;
			string speech = string.Join(value: (!levelSelect) ? new string[3]
			{
				RDString.Get("narration.rhythmWeightlifter.speedPercent").Replace("[percent]", num.ToString()),
				RDString.Get("narration.rhythmWeightlifter.rhythmPercent").Replace("[percent]", num2.ToString()),
				RDString.Get("narration.rank").Replace("[rank]", newValue)
			} : new string[2]
			{
				RDString.Get("narration.rank").Replace("[rank]", newValue),
				RDString.Get("rhythmWeightlifter.score").Replace("[score]", score.ToString())
			}, separator: Narration.ShortPause);
			if (levelSelect)
			{
				Narration.Say(speech, NarrationCategory.Navigation, _levelIndex + 1, lastUnlockedLevelIndex + 1, ElementType.SelectableEntity, null, "", true, NarrationActionName.ToSelect, NarrationActionName.ToNavigateHorizontal, NarrationActionName.ToExit);
			}
			else
			{
				Narration.Say(speech, NarrationCategory.Notification, false);
			}
		}

		private void SayCurrentPauseSelection()
		{
			string key = ((pausedOptionIndex == 0) ? "rhythmWeightlifter.continue" : ((pausedOptionIndex != 1) ? ((pausedOptionIndex == 2) ? "rhythmWeightlifter.quit" : "rhythmWeightlifter.language") : (Metronome ? "rhythmWeightlifter.metronomeOn" : "rhythmWeightlifter.metronomeOff")));
			Narration.Say(itemsLength: scnBase.isMobile ? 4 : 3, speech: RDString.Get(key), category: NarrationCategory.Navigation, itemIndex: pausedOptionIndex + 1, elementType: (pausedOptionIndex != 1) ? ElementType.Button : ElementType.None, typeData: null, description: "", flipCategoryQueueBehaviour: false, actions: new NarrationActionName[3]
			{
				NarrationActionName.ToSelect,
				NarrationActionName.ToNavigateHorizontal,
				NarrationActionName.ToGoBack
			});
		}

		public void TempoEngineURL()
		{
			Application.OpenURL("https://www.tempolabgames.com/tempo-engine");
		}

		private void UpdateDebugCombo()
		{
			if (!scnBase.isMobile)
			{
				return;
			}
			_debugComboFiredThisFrame = false;
			bool selectIsPressed = RDInput.selectIsPressed;
			bool leftPress = RDInput.leftPress;
			if (!selectIsPressed)
			{
				if (_debugComboArmed)
				{
					_debugComboArmed = false;
					_debugLeftPressCount = 0;
					_debugComboFiredThisFrame = true;
					ExecuteDebugCombo();
				}
				else
				{
					_debugLeftPressCount = 0;
				}
			}
			else if (leftPress)
			{
				_debugLeftPressCount++;
				if (_debugLeftPressCount == 5)
				{
					_debugComboArmed = true;
					printe("[DEBUG COMBO] Armed! Release select to activate.");
				}
			}
		}

		private void ExecuteDebugCombo()
		{
			printe("[DEBUG COMBO] Activated on screen: " + screen);
			switch (screen)
			{
			case Screen.Intro:
			{
				lastUnlockedLevelIndex = levels.Length - 1;
				Persistence.SetRhythmWeightlifterLastLevelUnlocked(levels.Length - 1);
				for (int i = 0; i < levels.Length; i++)
				{
					Persistence.SetRhythmWeightlifterSpeed(i, 100);
					Persistence.SetRhythmWeightlifterConsistency(i, 50);
					Persistence.SetRhythmWeightlifterScore(i, 200);
				}
				Persistence.SetLevelRank(global::Level.RhythmWeightlifter, 15);
				Persistence.SaveCurrentSlot();
				Level[] array = levels;
				for (int j = 0; j < array.Length; j++)
				{
					array[j].Load();
				}
				LevelIndex = levels.Length - 1;
				Flash(null);
				printe("[DEBUG COMBO] All levels maxed out.");
				break;
			}
			case Screen.LevelSelect:
				DebugSetProgressToLevel(LevelIndex);
				break;
			case Screen.Gameplay:
				if (!ShowingScore && !isInfiniteMode)
				{
					PlayLevelDialogue(force: true);
					printe("[DEBUG COMBO] Replayed level dialogue.");
				}
				else
				{
					_ = isInfiniteMode;
				}
				break;
			case Screen.LanguageSelection:
				DebugSetProgressToLevel(11);
				printe("[DEBUG COMBO] Skipping to level 11.");
				break;
			case Screen.Ending:
			case Screen.DeleteData:
				break;
			}
		}

		private void DebugSetProgressToLevel(int levelIndex)
		{
			int num = (lastUnlockedLevelIndex = levelIndex);
			Persistence.SetRhythmWeightlifterLastLevelUnlocked(num);
			for (int i = num; i < levels.Length; i++)
			{
				Persistence.SetRhythmWeightlifterSpeed(i, 0);
				Persistence.SetRhythmWeightlifterConsistency(i, 0);
				Persistence.SetRhythmWeightlifterScore(i, 0);
			}
			Persistence.SetLevelRank(global::Level.RhythmWeightlifter, -2, force: true);
			Persistence.SaveCurrentSlot();
			Level[] array = levels;
			for (int j = 0; j < array.Length; j++)
			{
				array[j].Load();
			}
			if (screen == Screen.LevelSelect)
			{
				OnEnterLevelSelectScreen();
				Flash(null);
				lastLevelPlayed = -1;
			}
			printe($"[DEBUG COMBO] Progress set to level {num + 1}.");
			if (screen != Screen.LevelSelect)
			{
				SetScreen(Screen.LevelSelect);
			}
		}

		private void UpdateInfiniteDebugCombo()
		{
			if (!scnBase.isMobile || screen != Screen.Gameplay || !isInfiniteMode)
			{
				if (_infiniteDebugPhase != 0 || _infiniteDebugFirstCount != 0)
				{
					ResetInfiniteDebugCombo();
				}
				return;
			}
			bool selectIsPressed = RDInput.selectIsPressed;
			bool selectRelease = RDInput.selectRelease;
			bool leftPress = RDInput.leftPress;
			if (selectRelease)
			{
				if (_infiniteDebugFirstCount <= 0)
				{
					return;
				}
				if (_infiniteDebugPhase == 0 && _infiniteDebugFirstCount == 5)
				{
					printe("[INFINITE DEBUG] Normal skip.");
					ResetInfiniteDebugCombo();
					SkipInfiniteRound();
				}
				else if (_infiniteDebugPhase == 1)
				{
					if (_infiniteDebugSecondCount > 0)
					{
						printe($"[INFINITE DEBUG] Custom skip: left={_infiniteDebugFirstCount} right={_infiniteDebugSecondCount}.");
						Level customLevel = new Level(_infiniteDebugSecondCount, _infiniteDebugFirstCount, LevelCharacterName.ian, LevelCharacterName.ian);
						ResetInfiniteDebugCombo();
						SkipInfiniteRoundWithLevel(customLevel);
					}
					else if (_infiniteDebugFirstCount == 5)
					{
						printe("[INFINITE DEBUG] Phase 2 with no second count, normal skip.");
						ResetInfiniteDebugCombo();
						SkipInfiniteRound();
					}
				}
				else
				{
					ResetInfiniteDebugCombo();
				}
			}
			else
			{
				if (!selectIsPressed)
				{
					return;
				}
				if (_infiniteDebugPhase == 0)
				{
					if (leftPress)
					{
						_infiniteDebugFirstCount = Mathf.Clamp(_infiniteDebugFirstCount + 1, 0, 6);
						_infiniteDebugPhaseTimer = 0f;
						printe($"[INFINITE DEBUG] First count: {_infiniteDebugFirstCount}");
					}
					if (_infiniteDebugFirstCount <= 0)
					{
						return;
					}
					_infiniteDebugPhaseTimer += Time.deltaTime;
					if (_infiniteDebugPhaseTimer >= 1f)
					{
						if (!_debugInfiniteWaitTimeSoundPlayed)
						{
							RWPlaySound("sndCrowdCheer");
							_debugInfiniteWaitTimeSoundPlayed = true;
						}
						printe($"[INFINITE DEBUG] Entering phase 2. Left weight = {_infiniteDebugFirstCount}.");
						_infiniteDebugPhase = 1;
						_infiniteDebugPhaseTimer = 0f;
					}
				}
				else if (_infiniteDebugPhase == 1)
				{
					if (leftPress)
					{
						_infiniteDebugSecondCount = Mathf.Clamp(_infiniteDebugSecondCount + 1, 0, 6);
						_infiniteDebugPhaseTimer = 0f;
						printe($"[INFINITE DEBUG] Second count: {_infiniteDebugSecondCount}");
					}
					_infiniteDebugPhaseTimer += Time.deltaTime;
					if (_infiniteDebugPhaseTimer >= 5f)
					{
						printe("[INFINITE DEBUG] Phase 2 timed out, cancelling.");
						ResetInfiniteDebugCombo();
					}
				}
			}
		}

		private void ResetInfiniteDebugCombo()
		{
			_infiniteDebugPhase = 0;
			_infiniteDebugFirstCount = 0;
			_infiniteDebugSecondCount = 0;
			_infiniteDebugPhaseTimer = 0f;
			_debugInfiniteWaitTimeSoundPlayed = false;
		}

		private void SkipInfiniteRound()
		{
			ShowingScore = false;
			canReceiveInput = false;
			currentInfiniteModeRound++;
			SetScreen(Screen.Gameplay);
			printe("[INFINITE DEBUG] Round skipped normally.");
		}

		private void SkipInfiniteRoundWithLevel(Level customLevel)
		{
			_debugForcedInfiniteLevel = customLevel;
			ShowingScore = false;
			canReceiveInput = false;
			currentInfiniteModeRound++;
			SetScreen(Screen.Gameplay);
			printe($"[INFINITE DEBUG] Skipping to custom level p1={customLevel.player1Weight} p2={customLevel.player2Weight}.");
		}
	}
}
