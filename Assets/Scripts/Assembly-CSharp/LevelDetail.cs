using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DG.Tweening;
using RDLevelEditor;
using UnityEngine;
using UnityEngine.UI;

public class LevelDetail : RDBase
{
	[Serializable]
	public struct CustomLevelPlayMode
	{
		public LevelPlayMode name;

		public Sprite sprite;
	}

	[Serializable]
	public class Difficulty
	{
		public LevelDifficulty name;

		public GameObject container;

		public Text text;

		public RectTransform textRect;

		public RectTransform stripRect;

		public string token;

		private bool calculatedPosition;
	}

	public struct SpeedSetting
	{
		public LevelSpeed name;

		public float discXPos;

		public Color ticketColor;

		public Color ticketLetterColor;

		public SpeedSetting(LevelSpeed _name, float _discXPos, Color _ticketColor, Color _ticketLetterColor)
		{
			name = _name;
			discXPos = _discXPos;
			ticketColor = _ticketColor;
			ticketLetterColor = _ticketLetterColor;
		}
	}

	public enum MediaLinkType
	{
		YouTube = 0,
		Spotify = 1,
		Bandcamp = 2,
		Twitter = 3,
		SoundCloud = 4,
		Facebook = 5,
		Bilibili = 6,
		Niconico = 7,
		Other = 8
	}

	public struct MediaLinkTheme
	{
		public Color color;
	}

	public enum LevelSpeed
	{
		Ice = 0,
		Normal = 1,
		Chili = 2
	}

	public enum DetailOptionName
	{
		ExpandDescription = -1,
		ArtistLink = -2,
		LevelSpeed = 0,
		StartLevel = 1,
		EditLevel = 2,
		CheckErrors = 3,
		DeleteLevel = 4,
		ShowInFolder = 5,
		ShowInWorkshop = 9,
		Unsubscribe = 10,
		OpenLevelsFolder = 6,
		Reload = 7,
		ImportLevels = 8
	}

	public enum ScreenName
	{
		NoLevelsFound = 1,
		PlayableLevel = 2,
		NonPlayableLevel = 3,
		Description = 4
	}

	public enum ChickenAnimName
	{
		Idle = 0,
		Ok = 1,
		LevelSelectedIdle = 2,
		Sleepy = 3,
		Sleep = 4,
		Wakeup = 5
	}

	public Dictionary<MediaLinkType, MediaLinkTheme> mediaLinkThemes = new Dictionary<MediaLinkType, MediaLinkTheme>
	{
		[MediaLinkType.YouTube] = new MediaLinkTheme
		{
			color = 4292428868u.ARGBToColor()
		},
		[MediaLinkType.Spotify] = new MediaLinkTheme
		{
			color = 4281914998u.ARGBToColor()
		},
		[MediaLinkType.Bandcamp] = new MediaLinkTheme
		{
			color = 4281636528u.ARGBToColor()
		},
		[MediaLinkType.Twitter] = new MediaLinkTheme
		{
			color = 4281180904u.ARGBToColor()
		},
		[MediaLinkType.SoundCloud] = new MediaLinkTheme
		{
			color = 4294607681u.ARGBToColor()
		},
		[MediaLinkType.Facebook] = new MediaLinkTheme
		{
			color = 4282542002u.ARGBToColor()
		},
		[MediaLinkType.Bilibili] = new MediaLinkTheme
		{
			color = 4294939819u.ARGBToColor()
		},
		[MediaLinkType.Niconico] = new MediaLinkTheme
		{
			color = 4289374890u.ARGBToColor()
		},
		[MediaLinkType.Other] = new MediaLinkTheme
		{
			color = 4287945199u.ARGBToColor()
		}
	};

	public readonly SpeedSetting[] SpeedSettings = new SpeedSetting[3]
	{
		new SpeedSetting(LevelSpeed.Ice, -27.5f, new Color(0.6755073f, 0.8679245f, 0.8640358f, 1f), new Color(0.04570134f, 0.6725605f, 79f / 106f, 1f)),
		new SpeedSetting(LevelSpeed.Normal, 0f, new Color(1f, 1f, 0.7882354f, 1f), new Color(1f, 0.3960785f, 0f, 1f)),
		new SpeedSetting(LevelSpeed.Chili, 27.5f, new Color(0.9056604f, 0.7390531f, 0.7865437f, 1f), new Color(77f / 106f, 0.05844719f, 0f, 1f))
	};

	public static readonly Vector2 IdleLevelsPos = new Vector2(99f, -9.5f);

	public static readonly Vector2 IdleWardPos = new Vector2(75f, -14.5f);

	public const float RankTicketHideYPos = -9f;

	public const float RankTicketAnimDuration = 0.25f;

	private const float PreviewSelectedHeight = 0f;

	private const float OptionsDefaultHeight = 0f;

	private const float FadeTransitionDuration = 0.5f;

	private int _currentOptionIndex;

	private ChickenAnimName _currentChickenAnimName;

	[NonSerialized]
	public RectTransform rect;

	[NonSerialized]
	public ScreenName currentOptionScreenName;

	[NonSerialized]
	public float currentChickenSleepTime;

	[NonSerialized]
	public bool isShaking;

	[NonSerialized]
	public bool showingErrorsContainer;

	[NonSerialized]
	public bool canReceiveInputs;

	[NonSerialized]
	public int currentLevelSpeedIndex;

	[Header("General")]
	public SpriteAnimation scanlinesAnimation;

	public GameObject legacyLevelPreviewImage;

	public GameObject loadingDetailContainer;

	[Header("No Levels Found Screen")]
	public GameObject noLevelsFound;

	public LevelDetailOption[] noLevelsFoundOptions;

	[Header("Preview")]
	public RawImage previewImage;

	public Image PreviewBlackCoverImage;

	public Image levelSource;

	public Image levelPlayModeImage;

	public Sprite fromSteam;

	public Sprite fromOther;

	public RectTransform authorNameRect;

	public Text authorNameText;

	public Text descriptionText;

	public Font defaultPixelatedFont;

	[Header("Start Row")]
	public StartRow startRow;

	public RectTransform startRect;

	public RectTransform speedRect;

	public BpmMovementAnimation[] selectorFrameCorners;

	public RectTransform selectorContainer;

	public RectTransform difficultiesContainer;

	public Difficulty[] difficulties;

	public CanvasGroup authorContainer;

	public CanvasGroup seizureWarningContainer;

	[Header("Playable Level Screen")]
	public RectTransform playableDetailRect;

	public RectTransform playableDetailOptionsRect;

	public RectTransform speedSettingDiscRect;

	public BpmSpriteAnimation speedSettingIce;

	public BpmMovementAnimation speedSettingChilli;

	public LevelDetailOption[] playableOptions;

	public Image[] playableDetailOptionSquares;

	public GameObject playableDetailOptionSquaresContainer;

	public GameObject lastSquareSelector;

	[Header("Non Playable Level")]
	public RectTransform nonPlayableDetailRect;

	public GameObject nonPlayableDetailContainer;

	public LevelDetailOption[] nonPlayableOptions;

	[Header("Steam Level Detail")]
	public LevelDetailOption[] steamOptions;

	public RectTransform showInFolderRect;

	public RectTransform showInWorkshopRect;

	public RectTransform unsubscribeRect;

	public GameObject showInWorkshopOption;

	public GameObject unsubscribeOption;

	public GameObject editLevelOption;

	public GameObject deleteLevelOption;

	[Header("Description Screen")]
	public LevelDetailOption descriptionOption;

	public RectTransform optionsBelowDescriptionRect;

	public RectTransform descriptionContainer;

	public RectTransform expandedDescriptionContainer;

	public GameObject descriptionPagePrefab;

	public GameObject artistLinkPrefab;

	public GameObject squareSelectorPrefab;

	public Transform descriptionSquareSelectorContainer;

	public Image[] descriptionSelectorSquares;

	public GameObject descriptionSquareOnMainPage;

	[NonSerialized]
	public LevelDetailOption[] descriptionOptions;

	[Header("Level Errors Presentation")]
	public RectTransform errorsContainer;

	public RectTransform errorsEditTransform;

	public RectTransform errorsBackTransform;

	public Button errorContainerEditButton;

	public LevelErrorPresentation levelErrorPresentation;

	[Header("Change Level")]
	public List<CustomLevelPlayMode> levelPlayModes;

	public RectTransform rankTicket;

	public Sprite rankFSprite;

	public Sprite rankDSprite;

	public Sprite rankCSprite;

	public Sprite rankBSprite;

	public Sprite rankASprite;

	public Sprite rankFPlusSprite;

	public Sprite rankDPlusSprite;

	public Sprite rankCPlusSprite;

	public Sprite rankBPlusSprite;

	public Sprite rankAPlusSprite;

	public Sprite rankFMinusSprite;

	public Sprite rankDMinusSprite;

	public Sprite rankCMinusSprite;

	public Sprite rankBMinusSprite;

	public Sprite rankAMinusSprite;

	public Sprite[] rankSSprites;

	public Sprite[] rankSPlusSprites;

	public Sprite[] rankSMinusSprites;

	public SpriteAnimation rankLetterAnimation;

	public Image rankTicketImage;

	public Image rankLetterImage;

	[Header("ChickenAnimations")]
	public Transform detailBackground;

	public RectTransform detailTablet;

	public RectTransform chicken;

	public BpmSpriteAnimation chickenBpmAnimation;

	public SpriteAnimation chickenAnimation;

	public SpriteAnimation.SpriteAnimationData[] chickenAnimations;

	private LevelDetailOption startOption;

	private const float StartResizeDuration = 0.25f;

	private RectTransform descriptionRect;

	private RectTransform previewImageRect;

	private AudioSource audioSource;

	private Dictionary<LevelPlayMode, Sprite> levelPlayerModesDict;

	private bool singlePlayerChosen;

	private bool animatingChickenSleepIdle;

	private bool canChangePlayMode;

	private Dictionary<int, Sprite> ranksDict;

	private Vector2 detailDescriptionStartPos;

	private bool canUseShowInFolderOption;

	private float timeToPlayScanlinesAnimation;

	private float scanlinesAnimCurrentTime;

	private Tween AnimateTicketUsingLevelSpeedTween;

	private int descriptionPageCount;

	private string[] descriptionPagesForNarration;

	private string[] artistLinks;

	private bool canExpandDescription;

	private bool isDescriptionExpanded;

	private Vector2 errorPresentationSizeOffset = new Vector2(4f, 4f);

	public CustomLevelData CurrentLevelData => CurrentLevelsData.Find((CustomLevelData levelData) => levelData.path == base.cls.CurrentLevel.path);

	public List<CustomLevelData> CurrentLevelsData
	{
		get
		{
			if (!base.cls.showingSearchContent)
			{
				return base.cls.levelsData;
			}
			List<CustomLevelData> list = new List<CustomLevelData>();
			foreach (int item in base.cls.searchLevelsDataIndex)
			{
				list.Add(base.cls.levelsData[item]);
			}
			return list;
		}
	}

	public int CurrentOptionIndex
	{
		get
		{
			return _currentOptionIndex;
		}
		set
		{
			if (currentOptionScreenName == ScreenName.PlayableLevel)
			{
				playableDetailOptionSquares[_currentOptionIndex].color = base.cls.DetailOptionSquareColorUnselected;
			}
			else if (currentOptionScreenName == ScreenName.Description)
			{
				descriptionSelectorSquares[_currentOptionIndex].color = base.cls.DetailOptionSquareColorUnselected;
			}
			CurrentOption.Deselect();
			if (CurrentOption.optionName == DetailOptionName.DeleteLevel)
			{
				selectorContainer.gameObject.SetActive(value: true);
			}
			_currentOptionIndex = value;
			if (currentOptionScreenName == ScreenName.PlayableLevel)
			{
				playableDetailOptionSquares[_currentOptionIndex].color = base.cls.DetailOptionSquareColorSelected;
			}
			else if (currentOptionScreenName == ScreenName.Description)
			{
				descriptionSelectorSquares[_currentOptionIndex].color = base.cls.DetailOptionSquareColorSelected;
			}
			CurrentOption.Select();
		}
	}

	public LevelDetailOption[] CurrentOptions
	{
		get
		{
			switch (currentOptionScreenName)
			{
			case ScreenName.NoLevelsFound:
				return noLevelsFoundOptions;
			case ScreenName.PlayableLevel:
				if (CurrentLevelData.isFromSteam)
				{
					return steamOptions;
				}
				return playableOptions;
			case ScreenName.NonPlayableLevel:
				return nonPlayableOptions;
			case ScreenName.Description:
				return descriptionOptions;
			default:
				RDBaseDllDummy.printem($"{currentOptionScreenName} has not been added here");
				return null;
			}
		}
	}

	public LevelDetailOption CurrentOption
	{
		get
		{
			return CurrentOptions[CurrentOptionIndex];
		}
		set
		{
			CurrentOptions[CurrentOptionIndex] = value;
		}
	}

	public LevelSpeed CurrentLevelSpeedName => SpeedSettings[currentLevelSpeedIndex].name;

	public ChickenAnimName CurrentChickenAnimName
	{
		get
		{
			return _currentChickenAnimName;
		}
		set
		{
			_currentChickenAnimName = value;
			SpriteAnimation.SpriteAnimationData currentAnimData = chickenAnimations[0];
			bool flag = false;
			SpriteAnimation.SpriteAnimationData[] array = chickenAnimations;
			foreach (SpriteAnimation.SpriteAnimationData spriteAnimationData in array)
			{
				if (spriteAnimationData.name == _currentChickenAnimName.ToString())
				{
					currentAnimData = spriteAnimationData;
					flag = true;
				}
			}
			if (!flag)
			{
				RDBaseDllDummy.printem("Did not found any chicken animation with the name " + _currentChickenAnimName);
			}
			chickenBpmAnimation.currentAnimData = currentAnimData;
		}
	}

	private void Awake()
	{
		levelPlayerModesDict = new Dictionary<LevelPlayMode, Sprite>();
		foreach (CustomLevelPlayMode levelPlayMode in levelPlayModes)
		{
			levelPlayerModesDict.Add(levelPlayMode.name, levelPlayMode.sprite);
		}
		ranksDict = new Dictionary<int, Sprite>();
		ranksDict.Add(0, rankFSprite);
		ranksDict.Add(1, rankDSprite);
		ranksDict.Add(2, rankCSprite);
		ranksDict.Add(3, rankBSprite);
		ranksDict.Add(4, rankASprite);
		ranksDict.Add(10, rankFPlusSprite);
		ranksDict.Add(11, rankDPlusSprite);
		ranksDict.Add(12, rankCPlusSprite);
		ranksDict.Add(13, rankBPlusSprite);
		ranksDict.Add(14, rankAPlusSprite);
		ranksDict.Add(-10, rankFMinusSprite);
		ranksDict.Add(-11, rankDMinusSprite);
		ranksDict.Add(-12, rankCMinusSprite);
		ranksDict.Add(-13, rankBMinusSprite);
		ranksDict.Add(-14, rankAMinusSprite);
		errorContainerEditButton.onClick.AddListener(() =>
		{
			SelectSetting(DetailOptionName.EditLevel);
		});
		rect = GetComponent<RectTransform>();
		previewImageRect = previewImage.GetComponent<RectTransform>();
		descriptionRect = descriptionText.GetComponent<RectTransform>();
		InitStartLayout();
		currentOptionScreenName = ScreenName.NoLevelsFound;
		detailDescriptionStartPos = descriptionRect.anchoredPosition;
		if (scnBase.isSteamDeck)
		{
			List<LevelDetailOption> list = steamOptions.ToList();
			LevelDetailOption levelDetailOption = list.Find((LevelDetailOption option) => option.optionName == DetailOptionName.ShowInFolder);
			levelDetailOption.gameObject.SetActive(value: false);
			list.Remove(levelDetailOption);
			steamOptions = list.ToArray();
			lastSquareSelector.SetActive(value: false);
		}
	}

	private void Start()
	{
		audioSource = base.cls.previewSongPlayer.GetComponent<AudioSource>();
		if (scnCLS.CachedData.hasSavedData)
		{
			float levelSpeed = scnGame.levelSpeed;
			if (levelSpeed != 0.75f)
			{
				if (levelSpeed != 1f && levelSpeed == 1.5f)
				{
					currentLevelSpeedIndex = 2;
				}
				else
				{
					currentLevelSpeedIndex = 1;
				}
			}
			else
			{
				currentLevelSpeedIndex = 0;
			}
		}
		else
		{
			currentLevelSpeedIndex = 1;
		}
		scnGame.levelSpeed = 1f;
		speedSettingDiscRect.anchoredPosition = new Vector2(SpeedSettings[currentLevelSpeedIndex].discXPos, speedSettingDiscRect.anchoredPosition.y);
		float num = SpeedSettings[currentLevelSpeedIndex].name switch
		{
			LevelSpeed.Ice => 0.75f, 
			LevelSpeed.Chili => 1.5f, 
			_ => 1f, 
		};
		audioSource.pitch = num;
		if (CurrentLevelsData != null && base.cls.CurrentLevel != null)
		{
			BpmAnimationManager.instance.SetBpm(CurrentLevelData.settings.bpm * num);
		}
	}

	private void Update()
	{
		if (scanlinesAnimCurrentTime < timeToPlayScanlinesAnimation)
		{
			scanlinesAnimCurrentTime += RDTime.unscaledDeltaTime;
		}
		else
		{
			scanlinesAnimation.Play();
			timeToPlayScanlinesAnimation = UnityEngine.Random.Range(8f, 16f);
			scanlinesAnimCurrentTime = 0f;
		}
		if (RDInput.anyKeyDown)
		{
			currentChickenSleepTime = 0f;
			if (CurrentChickenAnimName == ChickenAnimName.Sleep || CurrentChickenAnimName == ChickenAnimName.Sleepy)
			{
				ChangeChickenAnimation(ChickenAnimName.Wakeup);
				AnimateDetailIdle(animate: true);
			}
		}
		if (!canReceiveInputs || base.cls.dialog.gameObject.activeInHierarchy || !base.cls.CanReceiveInput || base.cls.levelImporter.Showing)
		{
			return;
		}
		if (showingErrorsContainer)
		{
			if (RDInput.leftPress && !RDInput.rightIsPressed)
			{
				if (selectorContainer.parent == errorsEditTransform)
				{
					base.cls.CLSPlaySound("sndPagerCursorBorder");
				}
				else
				{
					base.cls.CLSPlaySound("sndPagerCursorMove");
					selectorContainer.SetParent(errorsEditTransform);
					selectorContainer.localPosition = new Vector3((0f - errorPresentationSizeOffset.x) / 2f, errorPresentationSizeOffset.y / 2f, 0f);
					selectorContainer.sizeDelta = errorsEditTransform.sizeDelta + errorPresentationSizeOffset;
					Narration.Say(RDString.Get("customLevelSelect.editLevel"), NarrationCategory.Navigation, 1, 2, ElementType.Button, null, RDString.Get("narration.unavailable"), false, NarrationActionName.ToGoBack, NarrationActionName.ToSelect, NarrationActionName.ToNavigateHorizontal);
				}
			}
			else if (RDInput.rightPress && !RDInput.leftIsPressed)
			{
				if (selectorContainer.parent == errorsBackTransform)
				{
					base.cls.CLSPlaySound("sndPagerCursorBorder");
				}
				else
				{
					base.cls.CLSPlaySound("sndPagerCursorMove");
					selectorContainer.SetParent(errorsBackTransform);
					selectorContainer.localPosition = new Vector3((0f - errorPresentationSizeOffset.x) / 2f, errorPresentationSizeOffset.y / 2f, 0f);
					selectorContainer.sizeDelta = errorsEditTransform.sizeDelta + errorPresentationSizeOffset;
					Narration.Say(RDString.Get("customLevelSelect.errorsBack"), NarrationCategory.Navigation, 2, 2, ElementType.Button, null, null, false, NarrationActionName.ToGoBack, NarrationActionName.ToSelect, NarrationActionName.ToNavigateHorizontal);
				}
			}
			if (RDInput.cancelPress)
			{
				CloseErrorsContainer();
				base.cls.CLSPlaySound("sndHammer");
			}
			else if (RDInput.anyPlayerPress)
			{
				if (selectorContainer.parent == errorsEditTransform)
				{
					SelectSetting(DetailOptionName.EditLevel);
				}
				else
				{
					CloseErrorsContainer();
				}
				base.cls.CLSPlaySound("sndPagerButton");
			}
		}
		else if (!base.cls.hasLoadedAnyLevel)
		{
			if (RDInput.downPress && !RDInput.upIsPressed)
			{
				if (CurrentOptionIndex != CurrentOptions.Length - 1)
				{
					MoveDetailOptionSelector(1);
				}
				else
				{
					base.cls.CLSPlaySound("sndPagerCursorBorder");
				}
			}
			else if (RDInput.upPress && !RDInput.downIsPressed)
			{
				if (CurrentOptionIndex != 0)
				{
					MoveDetailOptionSelector(-1);
				}
				else
				{
					base.cls.CLSPlaySound("sndPagerCursorBorder");
				}
			}
			if (RDInput.anyPlayerPress)
			{
				SelectSetting(CurrentOption.optionName);
			}
		}
		else if (!base.cls.SelectedLevel)
		{
			currentChickenSleepTime += RDTime.unscaledDeltaTime;
			if (currentChickenSleepTime >= 140f)
			{
				AnimateDetailDrop();
			}
			else if (currentChickenSleepTime >= 120f && CurrentChickenAnimName == ChickenAnimName.Idle)
			{
				ChangeChickenAnimation(ChickenAnimName.Sleepy);
				StartCoroutine(AnimateDetailSleepIdle());
			}
		}
		else
		{
			if (currentOptionScreenName == ScreenName.NoLevelsFound)
			{
				return;
			}
			if (RDInput.downPress && !RDInput.upIsPressed)
			{
				if (CurrentOptionIndex != CurrentOptions.Length - 1)
				{
					MoveDetailOptionSelector(1);
				}
				else
				{
					base.cls.CLSPlaySound("sndPagerCursorBorder");
					if (CurrentOption.optionName == DetailOptionName.ExpandDescription)
					{
						Narration.Say(RDString.Get("customLevelSelect.noArtistLinks"), NarrationCategory.Navigation, false);
					}
				}
			}
			else if (RDInput.upPress && !RDInput.downIsPressed)
			{
				int num = ((!canExpandDescription && currentOptionScreenName != ScreenName.NonPlayableLevel) ? 1 : 0);
				if (CurrentOptionIndex != num)
				{
					MoveDetailOptionSelector(-1);
				}
				else
				{
					base.cls.CLSPlaySound("sndPagerCursorBorder");
				}
			}
			if (RDInput.rightPress || RDInput.leftPress)
			{
				string speech = "";
				NarrationActionName[] actions = new NarrationActionName[2]
				{
					NarrationActionName.ToChangeValue,
					NarrationActionName.ToClose
				};
				switch (CurrentOption.optionName)
				{
				case DetailOptionName.DeleteLevel:
					if (CurrentOption.ShowingOptions)
					{
						CurrentOption.ConfirmOptionIsSelected = !CurrentOption.ConfirmOptionIsSelected;
						string sound = (CurrentOption.ConfirmOptionIsSelected ? "sndLevelDetailsDeleteHover" : "sndPagerCursorMove");
						base.cls.CLSPlaySound(sound);
						int itemIndex = (CurrentOption.ConfirmOptionIsSelected ? 1 : 2);
						speech = (CurrentOption.ConfirmOptionIsSelected ? RDString.Get("customLevelSelect.deleteLevelConfirm") : RDString.Get("customLevelSelect.deleteLevelCancel"));
						Narration.Set(speech, itemIndex, 2, ElementType.Button);
						speech = string.Empty;
						actions = new NarrationActionName[3]
						{
							NarrationActionName.ToSelect,
							NarrationActionName.ToNavigateHorizontal,
							NarrationActionName.ToGoBack
						};
					}
					break;
				case DetailOptionName.LevelSpeed:
				{
					int direction = ((!RDInput.leftPress) ? 1 : (-1));
					ChangeLevelSpeed(direction);
					speech = RDString.Get("narration." + CurrentLevelSpeedName);
					break;
				}
				case DetailOptionName.StartLevel:
					if (canChangePlayMode)
					{
						base.cls.CLSPlaySound("sndPagerValue");
						ChangePlayMode(RDInput.rightPress);
						speech = (singlePlayerChosen ? RDString.Get("narration.onePlayerMode") : RDString.Get("narration.twoPlayerMode"));
					}
					break;
				}
				Narration.Say(speech, NarrationCategory.Navigation, flipCategoryQueueBehaviour: false, actions);
			}
			if (RDInput.anyPlayerPress)
			{
				if (CurrentOption.optionName == DetailOptionName.LevelSpeed)
				{
					ChangeLevelSpeed(1, wrap: true);
					Narration.Say(RDString.Get("narration." + CurrentLevelSpeedName), NarrationCategory.Navigation, false, NarrationActionName.ToChangeValue, NarrationActionName.ToClose);
				}
				else
				{
					SelectSetting(CurrentOption.optionName);
				}
			}
		}
		void CloseErrorsContainer()
		{
			ToggleErrorContainer(show: false);
			selectorContainer.SetParent(nonPlayableDetailRect);
			MoveDetailOptionSelector(0, fromUserInput: false);
		}
	}

	public void CheckCancelOptions()
	{
		if (CurrentOption.optionName == DetailOptionName.DeleteLevel && CurrentOption.ShowingOptions)
		{
			base.cls.PlayHammerSound();
			CurrentOption.Select();
			selectorContainer.gameObject.SetActive(value: true);
			NarrateIntro();
		}
		else if (currentOptionScreenName == ScreenName.Description)
		{
			ExpandDescription(expand: false);
			base.cls.PlayHammerSound();
			NarrateSetting(DetailOptionName.ExpandDescription);
		}
		else
		{
			StopCoroutine(base.cls.selectLevelCoroutine);
			base.cls.deselectLevelCoroutine = base.cls.DeselectLevel();
			StartCoroutine(base.cls.deselectLevelCoroutine);
		}
	}

	public void AnimateDetailIdle(bool animate)
	{
		animatingChickenSleepIdle = false;
		rect.DOKill();
		if (animate)
		{
			Vector2 idlePos = (base.cls.ShowingWard ? IdleWardPos : IdleLevelsPos);
			rect.DOAnchorPosY(idlePos.y - 4f, 0.375f).SetEase(Ease.InOutFlash).SetUpdate(isIndependentUpdate: true)
				.OnComplete(() =>
				{
					rect.DOAnchorPosY(idlePos.y + 4f, 0.75f).SetEase(Ease.InOutFlash).SetUpdate(isIndependentUpdate: true)
						.SetLoops(-1, LoopType.Yoyo);
				});
		}
		else
		{
			rect.DOAnchorPosY(IdleLevelsPos.y, 0.3f).SetEase(Ease.InOutFlash).SetUpdate(isIndependentUpdate: true);
		}
	}

	public IEnumerator AnimateDetailSleepIdle()
	{
		animatingChickenSleepIdle = true;
		float startYPos = rect.anchoredPosition.y;
		int multiplier = 1;
		do
		{
			float num = startYPos + (float)multiplier * -5f;
			float endValue = num + UnityEngine.Random.Range(5f, 6f);
			rect.DOKill();
			rect.DOAnchorPosY(num, 2f).SetUpdate(isIndependentUpdate: true).SetEase(Ease.InOutBack);
			rect.DOAnchorPosY(endValue, 2f).SetUpdate(isIndependentUpdate: true).SetEase(Ease.InOutBounce)
				.SetDelay(2f);
			yield return new WaitForSecondsRealtime(4f);
			multiplier++;
		}
		while (animatingChickenSleepIdle);
	}

	public void AnimateDetailDrop()
	{
		base.cls.CanReceiveInput = false;
		rect.DOKill();
		ChangeChickenAnimation(ChickenAnimName.Wakeup);
		animatingChickenSleepIdle = false;
		Vector2 DetailScreenStartPos = Vector2.zero;
		Vector2 ChickenStartPos = new Vector2(-1f, -4f);
		float currentSectionDuration = 0.4f;
		detailTablet.DOAnchorPosY(DetailScreenStartPos.y - 150f, currentSectionDuration).SetUpdate(isIndependentUpdate: true).SetEase(Ease.InFlash)
			.OnComplete(() =>
			{
				detailTablet.anchoredPosition = new Vector2(DetailScreenStartPos.x + 150f, DetailScreenStartPos.y);
			});
		detailTablet.DORotate(new Vector3(0f, 0f, -15f), currentSectionDuration).SetUpdate(isIndependentUpdate: true).SetEase(Ease.InCirc)
			.OnComplete(() =>
			{
				detailTablet.eulerAngles = Vector3.zero;
			});
		chicken.DOAnchorPosY(chicken.anchoredPosition.y - 20f, currentSectionDuration).SetUpdate(isIndependentUpdate: true).SetEase(Ease.InOutFlash);
		currentSectionDuration = 1.6f;
		chicken.DOAnchorPosX(ChickenStartPos.x + 150f, currentSectionDuration * 0.6f).SetUpdate(isIndependentUpdate: true).SetEase(Ease.InOutSine)
			.SetDelay(currentSectionDuration * 0.4f);
		chicken.DOAnchorPosY(ChickenStartPos.y, currentSectionDuration * 0.6f).SetUpdate(isIndependentUpdate: true).SetEase(Ease.InOutBack)
			.SetDelay(currentSectionDuration * 0.4f)
			.OnComplete(() =>
			{
				currentSectionDuration = 2f;
				CurrentChickenAnimName = ChickenAnimName.Idle;
				chickenBpmAnimation.Play();
				Transform detailBackgroundParent = detailBackground.parent;
				int detailBackgroundChildIndex = detailBackground.GetSiblingIndex();
				detailBackground.SetParent(rect);
				detailBackground.SetSiblingIndex(0);
				rect.DOAnchorPosY(IdleLevelsPos.y, currentSectionDuration).SetUpdate(isIndependentUpdate: true).SetEase(Ease.OutBack);
				detailTablet.SetParent(chicken);
				chicken.DOAnchorPos(ChickenStartPos, currentSectionDuration * 0.5f).SetUpdate(isIndependentUpdate: true).SetEase(Ease.OutBack);
				chicken.eulerAngles = new Vector3(0f, 0f, 5f);
				chicken.DORotate(new Vector3(0f, 0f, -10f), currentSectionDuration * 0.2f).SetUpdate(isIndependentUpdate: true).SetEase(Ease.OutBack)
					.OnComplete(() =>
					{
						chicken.DORotate(Vector3.zero, currentSectionDuration * 0.8f).SetUpdate(isIndependentUpdate: true).SetEase(Ease.OutBack)
							.OnComplete(() =>
							{
								detailBackground.SetParent(detailBackgroundParent);
								detailBackground.SetSiblingIndex(detailBackgroundChildIndex);
								detailTablet.SetParent(rect);
								currentChickenSleepTime = 0f;
								AnimateDetailIdle(animate: true);
								base.cls.CanReceiveInput = true;
							});
					});
			});
	}

	public void ChangeChickenAnimation(ChickenAnimName animationName)
	{
		CurrentChickenAnimName = animationName;
		chickenBpmAnimation.Play();
		if (CurrentChickenAnimName == ChickenAnimName.Sleepy)
		{
			chickenBpmAnimation.OnStopAction = () =>
			{
				CurrentChickenAnimName = ChickenAnimName.Sleep;
				chickenBpmAnimation.Play();
			};
		}
	}

	public void ShowErrorLoadingData()
	{
		currentOptionScreenName = ScreenName.NonPlayableLevel;
		nonPlayableDetailContainer.SetActive(value: true);
		base.cls.levelNeverSelectedDetail.SetActive(value: false);
		ToggleRankTicket(-2);
	}

	public void ShowLevelData(scnCLS.LevelPendingData levelFileData)
	{
		base.cls.CheckStopTextScrolling(authorNameRect, authorNameText, 83f, 0.1f, 0f);
		nonPlayableDetailContainer.SetActive(value: false);
		RDLevelSettings settings = CurrentLevelData.settings;
		currentOptionScreenName = ScreenName.PlayableLevel;
		if (CurrentLevelData.isLegacyLevel)
		{
			authorNameText.text = (string.IsNullOrEmpty(settings.author) ? "???" : settings.author);
			Difficulty[] array = difficulties;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].container.SetActive(value: false);
			}
		}
		else
		{
			previewImage.texture = levelFileData.previewImage;
			previewImageRect.sizeDelta = LevelValidation.GetImageFixedSize(levelFileData.previewImage, LevelValidation.PreviewImageRecommendedDimensions);
			authorNameText.text = settings.author;
			Difficulty[] array = difficulties;
			foreach (Difficulty difficulty in array)
			{
				if (difficulty.name == settings.difficulty)
				{
					difficulty.container.SetActive(value: true);
					difficulty.text.text = RDString.Get(difficulty.token).ToUpper();
					difficulty.text.font = RDString.GetAppropiateFontForString(difficulty.text.text);
					float preferredWidth = difficulty.text.preferredWidth;
					difficulty.textRect.anchoredPosition = difficulty.textRect.anchoredPosition.WithX((preferredWidth + 2f) * -1f);
					difficulty.stripRect.sizeDelta = difficulty.stripRect.sizeDelta.WithX(preferredWidth + 15f);
				}
				else
				{
					difficulty.container.SetActive(value: false);
				}
			}
			Array.Find(difficulties, (Difficulty difficulty2) => difficulty2.name == settings.difficulty);
		}
		previewImage.gameObject.SetActive(!CurrentLevelData.isLegacyLevel);
		legacyLevelPreviewImage.SetActive(CurrentLevelData.isLegacyLevel);
		authorNameText.text = authorNameText.text.RemoveNewLines();
		authorNameText.font = RDString.GetAppropiateFontForString(authorNameText.text);
		base.cls.CheckTextScrolling(authorNameRect, authorNameText, settings.author, 83f);
		authorContainer.DOKill();
		seizureWarningContainer.DOKill();
		authorContainer.alpha = 1f;
		seizureWarningContainer.alpha = 0f;
		levelSource.sprite = (CurrentLevelData.isFromSteam ? fromSteam : fromOther);
		if (string.IsNullOrEmpty(settings.description))
		{
			if (string.IsNullOrEmpty(settings.artistLinks))
			{
				descriptionRect.gameObject.SetActive(value: false);
				startRect.anchoredPosition = new Vector2(startRect.anchoredPosition.x, -9f);
				speedRect.anchoredPosition = new Vector2(speedRect.anchoredPosition.x, -35f);
			}
			else
			{
				descriptionText.text = RDString.Get("customLevelSelect.more");
				descriptionText.font = RDString.GetAppropiateFontForString(descriptionText.text);
			}
		}
		else
		{
			descriptionRect.gameObject.SetActive(value: true);
			descriptionText.font = RDString.GetAppropiateFontForString(settings.description);
			descriptionText.lineSpacing = ((descriptionText.font == defaultPixelatedFont) ? 1.2f : 1f);
			descriptionText.text = settings.description;
			startRect.anchoredPosition = new Vector2(startRect.anchoredPosition.x, -23f);
			speedRect.anchoredPosition = new Vector2(speedRect.anchoredPosition.x, -43f);
		}
		editLevelOption.SetActive(!CurrentLevelData.isFromSteam);
		deleteLevelOption.SetActive(!CurrentLevelData.isFromSteam);
		showInWorkshopOption.SetActive(CurrentLevelData.isFromSteam);
		unsubscribeOption.SetActive(CurrentLevelData.isFromSteam);
		if (!scnBase.isSteamDeck)
		{
			float num = (CurrentLevelData.isFromSteam ? (-66f) : (-87f));
			showInFolderRect.anchoredPosition = showInFolderRect.anchoredPosition.WithY(num);
		}
		if (CurrentLevelData.isFromSteam)
		{
			float num2 = (scnBase.isSteamDeck ? (-66f) : (-87f));
			showInWorkshopRect.anchoredPosition = showInWorkshopRect.anchoredPosition.WithY(num2);
			float num3 = (scnBase.isSteamDeck ? (-87f) : (-108f));
			unsubscribeRect.anchoredPosition = unsubscribeRect.anchoredPosition.WithY(num3);
		}
		LevelPlayMode canBePlayedOn = settings.canBePlayedOn;
		GC.twoPlayerMode = canBePlayedOn == LevelPlayMode.TwoPlayerOnly;
		levelPlayModeImage.sprite = levelPlayerModesDict[canBePlayedOn];
		canChangePlayMode = canBePlayedOn == LevelPlayMode.BothModes;
		Rank rank = Persistence.GetCustomLevelRank(CurrentLevelData.Hash, GetCurrentLevelSpeed());
		ToggleRankTicket(rank);
		Canvas.ForceUpdateCanvases();
	}

	public void ToggleRankTicket(Rank rank)
	{
		float endValue;
		if (ranksDict.ContainsKey(rank) || (int)rank == 5 || (int)rank == 15 || (int)rank == -15)
		{
			endValue = 8f;
			if ((int)rank == 15)
			{
				rankLetterAnimation.currentAnimationData.sprites = rankSPlusSprites;
				rankLetterAnimation.Play();
			}
			else if ((int)rank == 5)
			{
				rankLetterAnimation.currentAnimationData.sprites = rankSSprites;
				rankLetterAnimation.Play();
			}
			else if ((int)rank == -15)
			{
				rankLetterAnimation.currentAnimationData.sprites = rankSMinusSprites;
				rankLetterAnimation.Play();
			}
			else
			{
				rankLetterAnimation.Stop();
				rankLetterAnimation.SetSprite(ranksDict[rank]);
			}
			rankTicketImage.color = SpeedSettings[currentLevelSpeedIndex].ticketColor;
			rankLetterImage.color = SpeedSettings[currentLevelSpeedIndex].ticketLetterColor;
		}
		else
		{
			endValue = -9f;
			if (rankLetterAnimation.playing)
			{
				rankLetterAnimation.Stop();
			}
		}
		rankTicket.DOKill();
		rankTicket.DOAnchorPosY(endValue, 0.25f).SetEase(Ease.OutBack).SetUpdate(isIndependentUpdate: true);
	}

	public void SelectLevel()
	{
		base.cls.CLSPlaySound("sndLibrarySyringeInsert");
		AnimateDetailIdle(animate: false);
		ChangeChickenAnimation(ChickenAnimName.LevelSelectedIdle);
		CurrentOptionIndex = ((currentOptionScreenName != ScreenName.NonPlayableLevel) ? 1 : 0);
		CrossfadeAuthorAndSeizureWarning(CurrentLevelData.settings.seizureWarning);
		if (currentOptionScreenName == ScreenName.PlayableLevel)
		{
			LevelPlayMode canBePlayedOn = CurrentLevelData.settings.canBePlayedOn;
			singlePlayerChosen = canBePlayedOn != LevelPlayMode.TwoPlayerOnly;
			UpdateStartLayout(animate: false);
			canExpandDescription = !CurrentLevelData.settings.description.IsNullOrEmpty() || !CurrentLevelData.settings.artistLinks.IsNullOrEmpty();
			if (canExpandDescription)
			{
				GenerateExpandedDescription();
			}
			descriptionSquareOnMainPage.SetActive(canExpandDescription);
		}
		playableDetailOptionsRect.anchoredPosition = new Vector2(playableDetailOptionsRect.anchoredPosition.x, 0f);
		BpmMovementAnimation[] array = selectorFrameCorners;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Play();
		}
		ChangeContentHeight(67f);
		NarrateIntro();
	}

	public void MoveDescription()
	{
		if (currentOptionScreenName != ScreenName.PlayableLevel)
		{
			return;
		}
		int lineCount = descriptionText.cachedTextGenerator.lineCount;
		if (lineCount > 2)
		{
			float num = 0f;
			if (descriptionText.font == defaultPixelatedFont)
			{
				int num2 = Mathf.RoundToInt((descriptionText.lineSpacing - 1f) * 10f);
				num = descriptionText.font.lineHeight + num2;
			}
			else
			{
				num = 11.5f;
			}
			int num3 = lineCount + 1;
			string description = CurrentLevelData.settings.description;
			descriptionText.text = description + "\n\n" + description;
			float num4 = num * (float)num3;
			descriptionRect.DOAnchorPosY(detailDescriptionStartPos.y + num4, (float)num3 * 1f).SetDelay(2f).SetEase(Ease.Flash)
				.SetUpdate(isIndependentUpdate: true)
				.SetLoops(-1, LoopType.Restart);
		}
	}

	public void DeselectLevel(bool deletedLevel = false)
	{
		base.cls.CLSPlaySound("sndLibrarySyringeRemove");
		if (currentOptionScreenName == ScreenName.PlayableLevel)
		{
			descriptionRect.DOKill();
			descriptionRect.anchoredPosition = detailDescriptionStartPos;
			speedSettingIce.Stop();
			speedSettingChilli.Stop();
			BpmMovementAnimation[] array = selectorFrameCorners;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Stop();
			}
		}
		CrossfadeAuthorAndSeizureWarning(showSeizureWarning: false);
		ChangeContentHeight(0f);
		ChangeChickenAnimation(ChickenAnimName.Idle);
		AnimateDetailIdle(animate: true);
	}

	public void ChangeContentHeight(float targetYPos)
	{
		RectTransform target = ((currentOptionScreenName == ScreenName.NonPlayableLevel) ? nonPlayableDetailRect : playableDetailRect);
		target.DOKill();
		target.DOAnchorPosY(targetYPos, 0.6f).SetEase(Ease.OutExpo).SetUpdate(isIndependentUpdate: true);
	}

	private void CrossfadeAuthorAndSeizureWarning(bool showSeizureWarning)
	{
		authorContainer.DOKill();
		seizureWarningContainer.DOKill();
		if (showSeizureWarning)
		{
			base.cls.CheckStopTextScrolling(authorNameRect, authorNameText, 83f, 0.1f, 0f);
		}
		else
		{
			base.cls.CheckTextScrolling(authorNameRect, authorNameText, CurrentLevelData.settings.author, 83f);
		}
		float endValue = (showSeizureWarning ? 0f : 1f);
		authorContainer.DOFade(endValue, 0.6f).SetEase(Ease.OutExpo).SetUpdate(isIndependentUpdate: true);
		float endValue2 = (showSeizureWarning ? 1f : 0f);
		seizureWarningContainer.DOFade(endValue2, 0.6f).SetEase(Ease.OutExpo).SetUpdate(isIndependentUpdate: true);
	}

	private void MoveDetailOptionSelector(int direction, bool fromUserInput = true)
	{
		if (fromUserInput)
		{
			base.cls.CLSPlaySound("sndPagerCursorMove", 1f, "CustomLevelSelect", RDUtils.PitchSemitonesToPercent((float)direction / -2f));
		}
		CurrentOptionIndex += direction;
		if (currentOptionScreenName == ScreenName.PlayableLevel)
		{
			if (CurrentOptionIndex == 2 && direction == -1)
			{
				playableDetailOptionsRect.DOKill();
				playableDetailOptionsRect.DOAnchorPosY(0f, 0.6f).SetEase(Ease.OutExpo).SetUpdate(isIndependentUpdate: true);
			}
			else if (CurrentOptionIndex == 3 && direction == 1)
			{
				playableDetailOptionsRect.DOKill();
				playableDetailOptionsRect.DOAnchorPosY(64f, 0.6f).SetEase(Ease.OutExpo).SetUpdate(isIndependentUpdate: true);
			}
		}
		else if (currentOptionScreenName == ScreenName.Description)
		{
			if (CurrentOptionIndex < descriptionPageCount)
			{
				int num = 85 * CurrentOptionIndex;
				playableDetailOptionsRect.DOKill();
				playableDetailOptionsRect.DOAnchorPosY(num, 0.6f).SetEase(Ease.OutExpo).SetUpdate(isIndependentUpdate: true);
			}
			else
			{
				int num2 = descriptionPageCount * 85 + (CurrentOptionIndex - descriptionPageCount) / 4 * 85;
				playableDetailOptionsRect.DOKill();
				playableDetailOptionsRect.DOAnchorPosY(num2, 0.6f).SetEase(Ease.OutExpo).SetUpdate(isIndependentUpdate: true);
			}
		}
		if (fromUserInput)
		{
			NarrateSetting(CurrentOption.optionName);
		}
	}

	private void SelectSetting(DetailOptionName settingName)
	{
		string sound = "sndPagerButton";
		switch (settingName)
		{
		case DetailOptionName.ExpandDescription:
			if (currentOptionScreenName != ScreenName.Description)
			{
				ExpandDescription();
				Narration.Set(RDString.Get("customLevelSelect.expandDescription"), ElementType.VerticalList, descriptionOptions.Length);
				NarrateSetting(CurrentOption.optionName);
			}
			else
			{
				ExpandDescription(expand: false);
				sound = "sndHammer";
				NarrateIntro();
			}
			break;
		case DetailOptionName.StartLevel:
			PlayLevel();
			break;
		case DetailOptionName.EditLevel:
			if (RDFile.Exists(CurrentLevelData.mainRDLevelFilePath))
			{
				scnCLS.CachedData.SaveData();
				base.cls.CanReceiveInput = false;
				base.cls.previewSongPlayer.StartingLevel = true;
				audioSource.DOFade(0f, 0.5f).SetUpdate(isIndependentUpdate: true).SetEase(Ease.InCirc);
				base.cls.fadeTransitionImage.DOKill();
				base.cls.fadeTransitionImage.DOFade(1f, 0.5f).SetEase(Ease.OutQuint).SetUpdate(isIndependentUpdate: true)
					.OnComplete(() =>
					{
						scnEditor.customLevelPath = CurrentLevelData.mainRDLevelFilePath;
						scnEditor.cameFromCustomLevelSelect = true;
						scnBase.GoToScene("scnEditor");
					});
			}
			break;
		case DetailOptionName.CheckErrors:
			if (Directory.Exists(CurrentLevelData.path))
			{
				RDBaseDllDummy.printem(CurrentLevelData.mainRDLevelFilePath + " - " + Path.GetFileName(CurrentLevelData.mainRDLevelFilePath));
				levelErrorPresentation.ShowErrors(base.cls.currentLevelErrors, CurrentLevelData.settings, Path.GetFileName(CurrentLevelData.mainRDLevelFilePath));
				ToggleErrorContainer(show: true);
			}
			break;
		case DetailOptionName.ShowInFolder:
			RDUtils.RevealInExplorer(CurrentLevelData.path);
			break;
		case DetailOptionName.DeleteLevel:
			if (CurrentOption.ShowingOptions)
			{
				CurrentOption.Press(showOptions: true);
				try
				{
					if (Directory.Exists(CurrentLevelData.path) && CurrentOption.ConfirmOptionIsSelected)
					{
						StartCoroutine(RemoveCurrentLevel());
						return;
					}
					CurrentOption.Select();
					selectorContainer.gameObject.SetActive(value: true);
					base.cls.CLSPlaySound("sndPagerButton");
					NarrateIntro();
					return;
				}
				catch (Exception ex)
				{
					canReceiveInputs = false;
					string text = RDString.Get("customLevelSelect.error.errorDeleteLevel").Replace("[error]", ex.Message);
					string[] options = new string[1] { RDString.Get("editor.PublishLevel.dialogOK") };
					base.cls.dialog.onHideComplete = () =>
					{
						canReceiveInputs = true;
					};
					base.cls.dialog.Show(text, options, (int option) =>
					{
						base.cls.dialog.Hide();
					});
					Debug.Log("Error deleting level: " + ex.Message);
					Narration.Say(text, NarrationCategory.Navigation, false);
					return;
				}
			}
			base.cls.CLSPlaySound("sndLevelDetailsDeleteWarning");
			Narration.Set(RDString.Get("customLevelSelect.deleteLevelPrompt"), ElementType.HorizontalMenu, 2);
			Narration.Say(RDString.Get("customLevelSelect.deleteLevelCancel"), NarrationCategory.Navigation, 2, 2, ElementType.Button, null, null, false, NarrationActionName.ToSelect, NarrationActionName.ToNavigateHorizontal, NarrationActionName.ToGoBack);
			break;
		case DetailOptionName.OpenLevelsFolder:
			RDUtils.RevealInExplorer(LevelValidation.CustomLevelsPath);
			break;
		case DetailOptionName.ImportLevels:
			base.cls.levelImporter.Showing = true;
			break;
		case DetailOptionName.Reload:
			StartCoroutine(base.cls.LoadLevelsData(1f));
			break;
		case DetailOptionName.ArtistLink:
		{
			int num = CurrentOptionIndex - descriptionPageCount;
			Application.OpenURL(artistLinks[num]);
			break;
		}
		case DetailOptionName.ShowInWorkshop:
			SteamWorkshop.ShowItemOnWorkshop(CurrentLevelData.publishedId);
			break;
		case DetailOptionName.Unsubscribe:
			SteamWorkshop.UnsubscribeItem(CurrentLevelData.publishedId);
			break;
		}
		bool showOptions = false;
		if (settingName == DetailOptionName.DeleteLevel)
		{
			showOptions = Directory.Exists(CurrentLevelData.path);
			selectorContainer.gameObject.SetActive(value: false);
		}
		CurrentOption.Press(showOptions);
		if (settingName != DetailOptionName.StartLevel)
		{
			base.cls.CLSPlaySound(sound);
		}
	}

	public IEnumerator RemoveCurrentLevel()
	{
		if (!RemoveLevelCheck(() =>
		{
			RDUtils.SetDirectoryAttributes(CurrentLevelData.path);
		}))
		{
			yield break;
		}
		yield return null;
		if (RemoveLevelCheck(async () =>
		{
			string text = await RDUtils.TryDeleteDirectory(CurrentLevelData.path);
			if (!text.IsNullOrEmpty())
			{
				ShowErrorLevelDetailDialog(text);
			}
		}))
		{
			yield return null;
			if (RemoveLevelCheck(() =>
			{
				RemoveCurrentLevelSyringe();
			}))
			{
				yield return null;
			}
		}
		bool RemoveLevelCheck(Action checkAction)
		{
			try
			{
				checkAction();
				return true;
			}
			catch (Exception ex)
			{
				ShowErrorLevelDetailDialog(ex.Message);
				canReceiveInputs = false;
				return false;
			}
		}
		void ShowErrorLevelDetailDialog(string errorMessage)
		{
			canReceiveInputs = false;
			string message = RDString.Get("customLevelSelect.error.errorDeleteLevel").Replace("[error]", errorMessage);
			string[] options = new string[1] { RDString.Get("editor.PublishLevel.dialogOK") };
			base.cls.dialog.onHideComplete = () =>
			{
				canReceiveInputs = true;
			};
			base.cls.dialog.Show(message, options, (int option) =>
			{
				base.cls.dialog.Hide();
			});
			Debug.Log("Error deleting level: " + errorMessage);
		}
	}

	public void RemoveCurrentLevelSyringe()
	{
		base.cls.ToggleCurrentSyringeScrolling(scroll: false);
		base.cls.CanReceiveInput = false;
		StartCoroutine(base.cls.DeselectLevel(deletedLevel: true));
		CustomLevelSyringe deletedLevel = base.cls.CurrentLevel;
		int deletedDataIndex = base.cls.CurrentLevel.currentDataIndex;
		int deletedPositionIndex = base.cls.CurrentLevel.currentPositionIndex;
		deletedLevel.ToggleLiquidPressing(pressing: false, deletedLevel: true);
		deletedLevel.ToggleSyringeUsb(showing: false);
		base.cls.previewSongPlayer.Stop(0.2f);
		base.cls.CLSPlaySound("sndLibrarySyringeDelete");
		deletedLevel.rect.DOAnchorPosX(-200f, 0.6f).SetEase(Ease.OutExpo).SetUpdate(isIndependentUpdate: true)
			.OnComplete(() =>
			{
				base.cls.lastLevelsPendingData.Remove(CurrentLevelData.path);
				base.cls.levelsData.Remove(CurrentLevelData);
				if (base.cls.showingSearchContent)
				{
					_ = base.cls.searchLevelsDataIndex[base.cls.CurrentLevelIndex];
					base.cls.searchLevelsDataIndex.RemoveAt(base.cls.CurrentLevelIndex);
					for (int i = 0; i < base.cls.searchLevelsDataIndex.Count; i++)
					{
						if (base.cls.searchLevelsDataIndex[i] >= deletedDataIndex)
						{
							base.cls.searchLevelsDataIndex[i]--;
						}
					}
				}
				if (CurrentLevelsData.Count < 11)
				{
					base.cls.visibleLevels.Remove(deletedLevel);
					deletedLevel.gameObject.SetActive(value: false);
				}
				if (CurrentLevelsData.Count == 0)
				{
					if (base.cls.showingSearchContent)
					{
						base.cls.ToggleLevelsSearchMode(oneColumnMode: true);
					}
					else
					{
						base.cls.fadeTransitionImage.DOKill();
						base.cls.fadeTransitionImage.DOFade(1f, 0.5f).SetDelay(0.3f).SetEase(Ease.InQuint)
							.SetUpdate(isIndependentUpdate: true)
							.OnComplete(() =>
							{
								scnBase.GoToCustomLevelSelect();
							});
					}
				}
				else
				{
					foreach (CustomLevelSyringe visibleLevel in base.cls.visibleLevels)
					{
						if (visibleLevel.currentDataIndex > deletedDataIndex)
						{
							visibleLevel.currentDataIndex--;
						}
						if (visibleLevel.currentPositionIndex > deletedPositionIndex)
						{
							visibleLevel.currentPositionIndex--;
						}
					}
					if (CurrentLevelsData.Count > 8)
					{
						CustomLevelSyringe customLevelSyringe = base.cls.visibleLevels[0];
						CustomLevelSyringe customLevelSyringe2 = base.cls.visibleLevels[0];
						foreach (CustomLevelSyringe visibleLevel2 in base.cls.visibleLevels)
						{
							if (visibleLevel2.currentPositionIndex >= deletedPositionIndex && visibleLevel2 != deletedLevel)
							{
								visibleLevel2.Move(-1, 0.1f);
							}
							if (visibleLevel2.currentPositionIndex < customLevelSyringe.currentPositionIndex)
							{
								customLevelSyringe = visibleLevel2;
							}
							if (visibleLevel2.currentPositionIndex > customLevelSyringe2.currentPositionIndex)
							{
								customLevelSyringe2 = visibleLevel2;
							}
						}
						if (CurrentLevelsData.Count >= 11)
						{
							deletedLevel.currentDataIndex = customLevelSyringe2.currentDataIndex + 1;
							deletedLevel.currentPositionIndex = customLevelSyringe2.currentPositionIndex + 1;
							if (deletedLevel.currentDataIndex >= CurrentLevelsData.Count)
							{
								deletedLevel.currentDataIndex -= CurrentLevelsData.Count;
							}
							deletedLevel.Move(-1);
							deletedLevel.UpdateInfo(CurrentLevelsData[deletedLevel.currentDataIndex]);
						}
						else if (CurrentLevelsData.Count % 2 != 0)
						{
							customLevelSyringe.currentPositionIndex = customLevelSyringe2.currentPositionIndex + 1;
							customLevelSyringe.Move(1);
						}
						if (base.cls.CurrentLevelIndex >= CurrentLevelsData.Count)
						{
							base.cls.CurrentLevelIndex -= CurrentLevelsData.Count;
						}
					}
					else if (CurrentLevelsData.Count == 8)
					{
						int currentLevelIndex = Mathf.CeilToInt(4f);
						base.cls.CurrentLevelIndex = currentLevelIndex;
						foreach (CustomLevelSyringe visibleLevel3 in base.cls.visibleLevels)
						{
							if (visibleLevel3.currentPositionIndex >= deletedPositionIndex)
							{
								visibleLevel3.Move(-1, 0.1f);
							}
						}
						List<CustomLevelSyringe> list = new List<CustomLevelSyringe>(base.cls.visibleLevels);
						for (int num = 0; num < 8; num++)
						{
							CustomLevelSyringe customLevelSyringe3 = list[0];
							foreach (CustomLevelSyringe item in list)
							{
								if (customLevelSyringe3.currentPositionIndex > item.currentPositionIndex)
								{
									customLevelSyringe3 = item;
								}
							}
							customLevelSyringe3.currentDataIndex = num;
							list.Remove(customLevelSyringe3);
						}
					}
					else
					{
						if (deletedDataIndex == CurrentLevelsData.Count)
						{
							base.cls.CurrentLevelIndex--;
						}
						foreach (CustomLevelSyringe visibleLevel4 in base.cls.visibleLevels)
						{
							bool flag = false;
							int direction = 1;
							if (deletedDataIndex == CurrentLevelsData.Count)
							{
								int num2 = ((visibleLevel4.currentPositionIndex < deletedPositionIndex) ? 1 : 2);
								visibleLevel4.currentPositionIndex += num2;
								if (visibleLevel4.currentPositionIndex >= 11)
								{
									visibleLevel4.currentPositionIndex -= 11;
									flag = true;
								}
							}
							else if (visibleLevel4.currentDataIndex >= deletedDataIndex)
							{
								direction = -1;
								if (visibleLevel4.currentPositionIndex < deletedPositionIndex)
								{
									visibleLevel4.currentPositionIndex--;
									if (visibleLevel4.currentPositionIndex < 0)
									{
										visibleLevel4.currentPositionIndex += 11;
										flag = true;
									}
								}
							}
							float timeToMove = (flag ? (-1f) : 0.1f);
							visibleLevel4.Move(direction, timeToMove);
						}
					}
					if (base.cls.showingSearchContent)
					{
						string text = base.cls.searchBarInputField.text.ToLower();
						if (CurrentLevelsData.Count == 0)
						{
							base.cls.searchBarInputField.text = text;
							return;
						}
						base.cls.Search(skippingBlankSpacesFilter: true);
						if (base.cls.CurrentLevelIndex >= CurrentLevelsData.Count)
						{
							RDBaseDllDummy.printem($"{base.cls.CurrentLevelIndex} - {CurrentLevelsData.Count}");
							base.cls.CurrentLevelIndex -= CurrentLevelsData.Count;
						}
					}
					base.cls.CurrentLevel.PlayPlungerIdle(play: true);
					base.cls.CurrentLevel.ToggleSyringeUsb(showing: true);
					base.cls.CurrentLevel.FadeOverlay(fadeOut: true);
					StopCoroutine(base.cls.sendLevelDataToLevelDetailCoroutine);
					base.cls.sendLevelDataToLevelDetailCoroutine = base.cls.SendLevelDataToLevelDetail(loadingLevelsData: false, -1f);
					StartCoroutine(base.cls.sendLevelDataToLevelDetailCoroutine);
					Narration.Set(RDString.Get("customLevelSelect.deleted"), ElementType.None, -1);
					Narration.Set(RDString.Get("customLevelSelect.ward.library"), ElementType.VerticalList, base.cls.levelsData.Count);
					base.cls.NarrateCurrentLevel();
				}
			});
	}

	private void PlayLevel()
	{
		if (CurrentLevelData.settings.version > 68)
		{
			string message = RDString.Get("customLevelSelect.playVersionError") ?? "";
			string[] options = new string[1] { RDString.Get("editor.PublishLevel.dialogOK") };
			base.cls.dialog.onHideComplete = () =>
			{
				canReceiveInputs = true;
			};
			base.cls.dialog.Show(message, options, (int option) =>
			{
				base.cls.dialog.Hide();
			});
			return;
		}
		string levelPath = CurrentLevelData.mainRDLevelFilePath;
		scnGame.levelPersistence.Setup(levelPath).rootLevelName = CurrentLevelData.settings.levelName;
		if (!CurrentLevelData.isLegacyLevel && !singlePlayerChosen && CurrentLevelData.settings.canBePlayedOn == LevelPlayMode.BothModes && !string.IsNullOrEmpty(CurrentLevelData.settings.separate2PLevelFilename))
		{
			levelPath = Path.Combine(CurrentLevelData.path, CurrentLevelData.settings.separate2PLevelFilename);
			RDBaseDllDummy.printem("Using separate 2p level at: " + levelPath);
		}
		if (!RDFile.Exists(levelPath))
		{
			string message2 = RDString.Get("customLevelSelect.playLevelNotFoundError") + " " + levelPath;
			string[] options2 = new string[1] { RDString.Get("editor.PublishLevel.dialogOK") };
			base.cls.dialog.onHideComplete = () =>
			{
				canReceiveInputs = true;
			};
			base.cls.dialog.Show(message2, options2, (int option) =>
			{
				base.cls.dialog.Hide();
			});
			RDBaseDllDummy.printem("RDLevel was not found using path: " + levelPath);
			return;
		}
		scanlinesAnimation.stopOnLoopEnding = true;
		timeToPlayScanlinesAnimation = 9999f;
		base.cls.CLSPlaySound("sndMenuSelect");
		base.cls.CanReceiveInput = false;
		scnCLS.CachedData.SaveData();
		ChangeChickenAnimation(ChickenAnimName.Ok);
		ChangeContentHeight(0f);
		RectTransform rectTransform = base.cls.infoPanel.rect;
		float endValue = rectTransform.anchoredPosition.y - rectTransform.sizeDelta.y;
		rectTransform.DOAnchorPosY(endValue, 0.6f).SetUpdate(isIndependentUpdate: true).SetEase(Ease.OutExpo);
		float endValue2 = difficultiesContainer.sizeDelta.x;
		difficultiesContainer.DOAnchorPosX(endValue2, 0.6f).SetUpdate(isIndependentUpdate: true).SetEase(Ease.InExpo)
			.SetDelay(0.6f);
		foreach (CustomLevelSyringe level in base.cls.visibleLevels)
		{
			float targetXPos = level.rect.anchoredPosition.x - 227f;
			level.DOKill();
			if (base.cls.CurrentLevel == level)
			{
				float endValue3 = level.rect.anchoredPosition.x - 30f - 4f;
				level.rect.DOAnchorPosX(endValue3, 0.4f).SetUpdate(isIndependentUpdate: true).SetEase(Ease.InOutCubic);
				rect.DOKill();
				float num = rect.anchoredPosition.x - 30f;
				rect.DOAnchorPos(new Vector2(num, 0f), 0.4f).SetUpdate(isIndependentUpdate: true).SetEase(Ease.InOutCubic)
					.OnComplete(() =>
					{
						level.rect.DOAnchorPosX(targetXPos, 0.8f).SetUpdate(isIndependentUpdate: true).SetEase(Ease.OutExpo);
						rect.DOAnchorPos(new Vector2(0f, -28.5f), 0.8f).SetUpdate(isIndependentUpdate: true).SetEase(Ease.OutQuad);
						float num2 = (float)Screen.width / (float)Screen.height / 1.7777778f;
						rect.DOScale(3f * num2, 1.2800001f).SetUpdate(isIndependentUpdate: true).SetEase(Ease.InOutQuad)
							.SetDelay(0.32000002f);
						PreviewBlackCoverImage.DOFade(1f, 0.32000002f).SetUpdate(isIndependentUpdate: true).SetEase(Ease.InCirc)
							.SetDelay(0.96000004f);
						base.cls.previewSongPlayer.StartingLevel = true;
						audioSource.DOFade(0f, 1.6f).SetUpdate(isIndependentUpdate: true).SetEase(Ease.InCirc)
							.OnComplete(() =>
							{
								scnGame.levelSpeed = GetCurrentLevelSpeed();
								scnBase.GoToLevelWithExternalPath(levelPath);
							});
					});
			}
			else
			{
				level.rect.DOAnchorPosX(targetXPos, 0.6f).SetUpdate(isIndependentUpdate: true).SetEase(Ease.OutQuart);
			}
		}
	}

	public float GetCurrentLevelSpeed()
	{
		return CurrentLevelSpeedName switch
		{
			LevelSpeed.Chili => 1.5f, 
			LevelSpeed.Ice => 0.75f, 
			_ => 1f, 
		};
	}

	private void GenerateExpandedDescription()
	{
		int num = ((descriptionText.font == defaultPixelatedFont) ? 8 : 7);
		string description = CurrentLevelData.settings.description;
		string text = CurrentLevelData.settings.artistLinks;
		LevelDetailOption[] array = new LevelDetailOption[0];
		LevelDetailOption[] array2 = new LevelDetailOption[0];
		foreach (Transform item in expandedDescriptionContainer)
		{
			UnityEngine.Object.Destroy(item.gameObject);
		}
		foreach (Transform item2 in descriptionSquareSelectorContainer)
		{
			UnityEngine.Object.Destroy(item2.gameObject);
		}
		descriptionPageCount = 0;
		if (!description.IsNullOrEmpty())
		{
			TextGenerator cachedTextGenerator = descriptionText.cachedTextGenerator;
			cachedTextGenerator.Populate(description, descriptionText.GetGenerationSettings(descriptionText.rectTransform.rect.size));
			int lineCount = cachedTextGenerator.lineCount;
			IList<UILineInfo> lines = cachedTextGenerator.lines;
			_ = new string[lineCount];
			int num2 = lineCount / num;
			if (lineCount % num != 0)
			{
				num2++;
			}
			string[] array3 = new string[num2];
			descriptionPagesForNarration = new string[num2];
			for (int i = 0; i < lineCount; i++)
			{
				int num3 = i / num;
				bool num4 = i == lineCount - 1;
				int startCharIdx = lines[i].startCharIdx;
				int num5 = (num4 ? description.Length : lines[i + 1].startCharIdx);
				string text2 = description.Substring(startCharIdx, num5 - startCharIdx);
				descriptionPagesForNarration[num3] += text2;
				if (!num4 && !text2.Contains('\n'))
				{
					text2 += "\n";
				}
				array3[num3] += text2;
			}
			descriptionPageCount = num2;
			array = new LevelDetailOption[num2];
			array[0] = descriptionOption;
			if (num2 > 1)
			{
				for (int j = 1; j < num2; j++)
				{
					int num6 = j * -86;
					GameObject obj = UnityEngine.Object.Instantiate(descriptionPagePrefab, expandedDescriptionContainer);
					obj.GetComponent<RectTransform>().AnchorPosY(num6);
					Text componentInChildren = obj.GetComponentInChildren<Text>();
					componentInChildren.text = array3[j];
					componentInChildren.font = RDString.GetAppropiateFontForString(componentInChildren.text);
					LevelDetailOption component = obj.GetComponent<LevelDetailOption>();
					component.optionSelector = descriptionOption.optionSelector;
					array[j] = component;
				}
			}
		}
		if (text != "")
		{
			string[] array4 = text.Replace(" ", "").Split(',');
			artistLinks = null;
			array2 = null;
			List<string> list = new List<string>();
			List<LevelDetailOption> list2 = new List<LevelDetailOption>();
			int num7 = 0;
			for (int k = 0; k < array4.Length; k++)
			{
				string text3 = array4[k];
				string[] array5 = text3.Replace("https://", "").Replace("http://", "").Replace("www.", "")
					.Split('.');
				if (array5.Length < 2)
				{
					RDBaseDllDummy.printem("linkParts is smaller than 2! first part is " + array5[0]);
					continue;
				}
				GameObject obj2 = UnityEngine.Object.Instantiate(artistLinkPrefab, expandedDescriptionContainer);
				int num8 = descriptionPageCount * 86 + 20 * num7 + 5 * (num7 / 4);
				if (descriptionPageCount == 0)
				{
					num8 += 3;
				}
				num8 = -num8;
				obj2.GetComponent<RectTransform>().AnchorPosY(num8);
				LevelDetailOption component2 = obj2.GetComponent<LevelDetailOption>();
				component2.optionSelector = descriptionOption.optionSelector;
				list2.Add(component2);
				MediaLinkType mediaLinkType = MediaLinkType.Other;
				switch (array5[0])
				{
				case "youtube":
				case "youtu":
					mediaLinkType = MediaLinkType.YouTube;
					break;
				case "spotify":
					mediaLinkType = MediaLinkType.Spotify;
					break;
				case "bandcamp":
					mediaLinkType = MediaLinkType.Bandcamp;
					break;
				case "twitter":
				case "x":
					mediaLinkType = MediaLinkType.Twitter;
					break;
				case "soundcloud":
					mediaLinkType = MediaLinkType.SoundCloud;
					break;
				case "facebook":
					mediaLinkType = MediaLinkType.Facebook;
					break;
				case "bilibili":
				case "b23":
					mediaLinkType = MediaLinkType.Bilibili;
					break;
				case "nicovideo":
					mediaLinkType = MediaLinkType.Niconico;
					break;
				}
				switch (array5[1])
				{
				case "bandcamp":
					mediaLinkType = MediaLinkType.Bandcamp;
					break;
				case "spotify":
					mediaLinkType = MediaLinkType.Spotify;
					break;
				case "bilibili":
					mediaLinkType = MediaLinkType.Bilibili;
					break;
				}
				component2.labelIcon.sprite = Resources.Load<Sprite>("CLS/MediaIcons/" + mediaLinkType);
				component2.backgroundImage.color = mediaLinkThemes[mediaLinkType].color;
				component2.mainLabel.text = ((mediaLinkType == MediaLinkType.Other) ? RDString.Get("customLevelSelect.linkOther") : mediaLinkType.ToString());
				component2.mainLabel.font = RDString.GetAppropiateFontForString(component2.mainLabel.text);
				if (!text3.StartsWith("http://") && !text3.StartsWith("https://") && !text3.StartsWith("www."))
				{
					text3 = "https://" + text3;
				}
				list.Add(text3);
				num7++;
			}
			artistLinks = list.ToArray();
			array2 = list2.ToArray();
		}
		descriptionOptions = new LevelDetailOption[array.Length + array2.Length];
		descriptionSelectorSquares = new Image[descriptionOptions.Length];
		for (int l = 0; l < array.Length; l++)
		{
			descriptionOptions[l] = array[l];
		}
		for (int m = 0; m < array2.Length; m++)
		{
			descriptionOptions[m + descriptionPageCount] = array2[m];
		}
		for (int n = 0; n < descriptionSelectorSquares.Length; n++)
		{
			Image component3 = UnityEngine.Object.Instantiate(squareSelectorPrefab, descriptionSquareSelectorContainer).GetComponent<Image>();
			descriptionSelectorSquares[n] = component3;
		}
	}

	public void ExpandDescription(bool expand = true)
	{
		CurrentOptionIndex = 0;
		float num = 0.25f;
		Ease ease = (expand ? Ease.OutBack : Ease.OutSine);
		bool flag = descriptionPageCount <= 0;
		if (expand)
		{
			ChangeContentHeight(87f);
		}
		else
		{
			descriptionOption.optionSelector.transform.SetParent(playableDetailOptionsRect);
			playableDetailOptionsRect.DOKill();
			playableDetailOptionsRect.AnchorPosY(0f);
		}
		currentOptionScreenName = (expand ? ScreenName.Description : ScreenName.PlayableLevel);
		optionsBelowDescriptionRect.DOAnchorPosY(expand ? (-80) : 0, num).SetEase(ease);
		playableDetailRect.DOKill();
		playableDetailRect.DOAnchorPosY(expand ? 87 : 67, num).SetEase(ease);
		descriptionContainer.DOKill();
		descriptionContainer.DOSizeDelta(new Vector2(102f, expand ? 79 : 21), num).SetEase(ease);
		descriptionContainer.DOAnchorPosY(expand ? (-2) : 0, num).SetEase(ease);
		descriptionOption.selectorSize = descriptionOption.selectorSize.WithY(expand ? 80 : 21);
		descriptionOption.optionSelector.DOAnchorPosY(expand ? (-2) : 0, num).SetEase(ease);
		descriptionOption.optionSelector.DOSizeDelta(new Vector2(103f, expand ? 80 : 21), num).SetEase(ease);
		descriptionRect.DOKill();
		if (expand)
		{
			if (flag)
			{
				descriptionText.text = string.Empty;
			}
			else
			{
				descriptionText.text = CurrentLevelData.settings.description;
			}
			descriptionRect.DOAnchorPosY(-1f, num).SetEase(ease);
			DOVirtual.DelayedCall(num, () =>
			{
				optionsBelowDescriptionRect.gameObject.SetActive(value: false);
			});
			expandedDescriptionContainer.gameObject.SetActive(value: true);
		}
		else
		{
			if (flag)
			{
				expandedDescriptionContainer.gameObject.SetActive(value: false);
				descriptionText.text = RDString.Get("customLevelSelect.more");
			}
			else
			{
				DOVirtual.DelayedCall(num, () =>
				{
					expandedDescriptionContainer.gameObject.SetActive(value: false);
					MoveDescription();
				});
			}
			optionsBelowDescriptionRect.gameObject.SetActive(value: true);
		}
		playableDetailOptionSquaresContainer.SetActive(!expand);
		descriptionSquareSelectorContainer.gameObject.SetActive(expand);
		canReceiveInputs = false;
		scnCLS.instance.CanReceiveInput = false;
		DOVirtual.DelayedCall(num, () =>
		{
			canReceiveInputs = true;
			base.cls.CanReceiveInput = true;
		});
		CurrentOptionIndex = 0;
		isDescriptionExpanded = expand;
	}

	private void InitStartLayout()
	{
		startOption = startRow.oneP.content.GetComponentInParent<LevelDetailOption>();
		startRow.Init(startOption.selectorPosition, startOption.selectorSize);
	}

	private void UpdateStartLayout(bool animate)
	{
		bool flag = currentOptionScreenName == ScreenName.PlayableLevel && CurrentOption == startOption;
		bool singlePlayer = singlePlayerChosen;
		StartRow.Frame frame = startRow.SetState(singlePlayer, canChangePlayMode, flag, animate);
		StartRow.Button button = startRow.ActiveButton(singlePlayer);
		startOption.backgroundImage = button.background;
		startOption.contentContainer = button.content;
		startOption.defaultBackgroundSprite = button.bgDefault;
		startOption.selectedBackgroundSprite = button.bgSelected;
		startOption.pressedBackgroundSprite = button.bgPressed;
		Vector2 activeContentPos = startRow.GetActiveContentPos(singlePlayer, canChangePlayMode);
		startOption.contentSelectedPos = activeContentPos + StartRow.FocusContentNudge;
		startOption.contentUnselectedPos = activeContentPos;
		startOption.selectorPosition = frame.pos;
		startOption.selectorSize = frame.size;
		if (flag)
		{
			float endValue = startRect.anchoredPosition.x + frame.pos.x;
			selectorContainer.DOKill();
			if (animate)
			{
				selectorContainer.DOAnchorPosX(endValue, 0.25f).SetEase(Ease.OutExpo).SetUpdate(isIndependentUpdate: true);
				selectorContainer.DOSizeDelta(frame.size, 0.25f).SetEase(Ease.OutExpo).SetUpdate(isIndependentUpdate: true);
			}
			else
			{
				selectorContainer.anchoredPosition = new Vector2(endValue, selectorContainer.anchoredPosition.y);
				selectorContainer.sizeDelta = frame.size;
			}
		}
	}

	private void ChangePlayMode(bool isRightPressed)
	{
		singlePlayerChosen = !singlePlayerChosen;
		GC.twoPlayerMode = !singlePlayerChosen;
		UpdateStartLayout(animate: true);
		if (GC.twoPlayerMode)
		{
			base.cls.CLSPlaySound("sndTwoPlayersActivateL", 1f, "CustomLevelSelect", 1f, GC.PanP1);
			base.cls.CLSPlaySound("sndTwoPlayersActivateR", 1f, "CustomLevelSelect", 1f, GC.PanP2);
		}
		else
		{
			base.cls.CLSPlaySound("sndTwoPlayersDeactivate");
		}
	}

	public void ToggleErrorContainer(bool show)
	{
		Vector3 endValue;
		if (show)
		{
			errorsContainer.localScale = Vector3.zero;
			endValue = Vector3.one;
			showingErrorsContainer = true;
		}
		else
		{
			errorsContainer.localScale = Vector3.one;
			endValue = Vector3.zero;
		}
		errorsContainer.DOKill(complete: true);
		errorsContainer.DOScale(endValue, 0.15f).SetEase(Ease.OutExpo).SetUpdate(isIndependentUpdate: true)
			.OnComplete(() =>
			{
				showingErrorsContainer = show;
				if (show)
				{
					selectorContainer.SetParent(errorsBackTransform);
					selectorContainer.localPosition = new Vector3((0f - errorPresentationSizeOffset.x) / 2f, errorPresentationSizeOffset.y / 2f, 0f);
					selectorContainer.sizeDelta = errorsEditTransform.sizeDelta + errorPresentationSizeOffset;
					Narration.Set(RDString.Get("customLevelSelect.import.errors"));
					Narration.Set(levelErrorPresentation.errorsText.text, ElementType.HorizontalMenu, 2);
					Narration.Say(RDString.Get("customLevelSelect.errorsBack"), NarrationCategory.Navigation, 2, -1, ElementType.Button, null, null, false, NarrationActionName.ToSelect, NarrationActionName.ToNavigateHorizontal);
				}
				else
				{
					NarrateIntro();
				}
			});
	}

	private void ChangeLevelSpeed(int direction, bool wrap = false)
	{
		int num = currentLevelSpeedIndex + direction;
		if (num >= SpeedSettings.Length || num < 0)
		{
			if (!wrap)
			{
				base.cls.CLSPlaySound("sndPagerCursorBorder");
				return;
			}
			num = (num + SpeedSettings.Length) % SpeedSettings.Length;
		}
		if (CurrentLevelSpeedName == LevelSpeed.Ice)
		{
			speedSettingIce.Stop();
		}
		else if (CurrentLevelSpeedName == LevelSpeed.Chili)
		{
			speedSettingChilli.Stop();
		}
		currentLevelSpeedIndex = num;
		speedSettingDiscRect.DOKill();
		speedSettingDiscRect.DOAnchorPosX(SpeedSettings[currentLevelSpeedIndex].discXPos, 0.3f).SetEase(Ease.OutExpo).SetUpdate(isIndependentUpdate: true);
		ToggleRankTicket(-2);
		if (AnimateTicketUsingLevelSpeedTween != null)
		{
			AnimateTicketUsingLevelSpeedTween.Kill();
		}
		Rank rank = Persistence.GetCustomLevelRank(CurrentLevelData.Hash, GetCurrentLevelSpeed());
		AnimateTicketUsingLevelSpeedTween = DOVirtual.DelayedCall(0.25f, () =>
		{
			ToggleRankTicket(rank);
		});
		string text = "";
		if (CurrentLevelSpeedName == LevelSpeed.Ice)
		{
			speedSettingIce.Play();
			text = "sndLevelDetailsSpeedIce";
			BpmAnimationManager.instance.SetBpm(CurrentLevelData.settings.bpm * 0.75f);
			audioSource.pitch = 0.75f;
		}
		else if (CurrentLevelSpeedName == LevelSpeed.Chili)
		{
			speedSettingChilli.Play();
			text = "sndLevelDetailsSpeedChili";
			BpmAnimationManager.instance.SetBpm(CurrentLevelData.settings.bpm * 1.5f);
			audioSource.pitch = 1.5f;
		}
		else
		{
			BpmAnimationManager.instance.SetBpm(CurrentLevelData.settings.bpm);
			audioSource.pitch = 1f;
		}
		if (!text.IsNullOrEmpty())
		{
			base.cls.CLSPlaySound(text);
		}
		base.cls.CLSPlaySound("sndPagerValue");
	}

	public void ShakeContainer()
	{
		isShaking = true;
		AnimateDetailIdle(animate: false);
		rect.DOKill();
		rect.DOShakeAnchorPos(1f, Vector2.right * 6f, 10, 0f).SetEase(Ease.OutQuint).SetUpdate(isIndependentUpdate: true)
			.OnKill(() =>
			{
				isShaking = false;
				AnimateDetailIdle(animate: true);
			})
			.OnComplete(() =>
			{
				isShaking = false;
				AnimateDetailIdle(animate: true);
			});
	}

	public void NarrateIntro()
	{
		switch (currentOptionScreenName)
		{
		case ScreenName.NoLevelsFound:
			Narration.Set(RDString.Get("customLevelSelect.noLevelsFound"), ElementType.VerticalMenu, CurrentOptions.Length);
			NarrateSetting(CurrentOption.optionName);
			break;
		case ScreenName.NonPlayableLevel:
			Narration.Set(RDString.Get("customLevelSelect.levelDetails"), ElementType.VerticalMenu, CurrentOptions.Length);
			NarrateSetting(CurrentOption.optionName);
			break;
		default:
		{
			string description = (CurrentLevelData.settings.seizureWarning ? RDString.Get("customLevelSelect.seizureWarning") : null);
			Narration.Set(RDString.Get("customLevelSelect.levelDetails"), -1, CurrentOptions.Length, ElementType.VerticalMenu, null, description);
			NarrateSetting(CurrentOption.optionName);
			break;
		}
		}
	}

	private void NarrateSetting(DetailOptionName settingName)
	{
		string speech = "";
		ElementType elementType = ElementType.Button;
		string description = null;
		switch (settingName)
		{
		case DetailOptionName.ExpandDescription:
			if (isDescriptionExpanded)
			{
				speech = descriptionPagesForNarration[CurrentOptionIndex].RemoveRichTextColorTags();
				elementType = ElementType.None;
			}
			else
			{
				speech = RDString.Get("customLevelSelect.expandDescription");
			}
			break;
		case DetailOptionName.StartLevel:
		{
			string newValue = (singlePlayerChosen ? RDString.Get("narration.onePlayerMode") : RDString.Get("narration.twoPlayerMode"));
			speech = RDString.Get("narration.characterSelectedTitle").Replace("[playerMode]", newValue);
			elementType = ElementType.None;
			break;
		}
		case DetailOptionName.LevelSpeed:
			speech = RDString.Get("narration.levelSpeedSlider").Replace("[speed]", RDString.Get("narration." + CurrentLevelSpeedName));
			elementType = ElementType.Slider;
			break;
		case DetailOptionName.EditLevel:
			speech = RDString.Get("customLevelSelect.editLevel");
			description = RDString.Get("narration.unavailable");
			break;
		case DetailOptionName.CheckErrors:
			speech = RDString.Get("customLevelSelect.checkErrors");
			break;
		case DetailOptionName.ShowInFolder:
			speech = RDString.Get("customLevelSelect.showInFolder");
			break;
		case DetailOptionName.DeleteLevel:
			speech = RDString.Get("customLevelSelect.deleteLevel");
			break;
		case DetailOptionName.OpenLevelsFolder:
			speech = RDString.Get("customLevelSelect.openLevelsFolder");
			break;
		case DetailOptionName.ImportLevels:
			speech = RDString.Get("customLevelSelect.import");
			break;
		case DetailOptionName.Reload:
			speech = RDString.Get("customLevelSelect.reload");
			break;
		case DetailOptionName.ArtistLink:
			speech = CurrentOption.mainLabel.text;
			elementType = ElementType.Link;
			break;
		case DetailOptionName.ShowInWorkshop:
			speech = RDString.Get("customLevelSelect.showInWorkshop");
			break;
		case DetailOptionName.Unsubscribe:
			speech = RDString.Get("customLevelSelect.unsubscribe");
			break;
		}
		List<NarrationActionName> list;
		if (settingName == DetailOptionName.ExpandDescription && isDescriptionExpanded)
		{
			list = new List<NarrationActionName>
			{
				NarrationActionName.ToClose,
				NarrationActionName.ToNavigateVertical
			};
		}
		else if (settingName == DetailOptionName.StartLevel)
		{
			list = ((CurrentLevelData.settings.canBePlayedOn != LevelPlayMode.BothModes) ? new List<NarrationActionName>
			{
				NarrationActionName.ToStart,
				NarrationActionName.ToNavigateVertical,
				NarrationActionName.ToGoBack
			} : new List<NarrationActionName>
			{
				NarrationActionName.ToStart,
				NarrationActionName.ToChangePlayerMode,
				NarrationActionName.ToNavigateVertical,
				NarrationActionName.ToGoBack
			});
		}
		else
		{
			List<NarrationActionName> list2 = ((settingName != DetailOptionName.LevelSpeed) ? new List<NarrationActionName>
			{
				NarrationActionName.ToSelect,
				NarrationActionName.ToNavigateVertical,
				NarrationActionName.ToGoBack
			} : new List<NarrationActionName>
			{
				NarrationActionName.ToChangeValue,
				NarrationActionName.ToNavigateVertical,
				NarrationActionName.ToGoBack
			});
			list = list2;
		}
		Narration.Say(speech, NarrationCategory.Navigation, CurrentOptionIndex + 1, CurrentOptions.Length, elementType, null, description, flipCategoryQueueBehaviour: false, list.ToArray());
	}
}
