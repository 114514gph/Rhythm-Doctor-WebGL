using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using DG.Tweening;
using RDLevelEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

public class scnCLS : scnBase
{
	[Serializable]
	public static class CachedData
	{
		public static List<CustomLevelData> levelsData;

		public static CustomLevelData currentLevelData;

		public static string currentLevelPath;

		public static LevelPendingData levelFileData;

		public static string searchInput;

		public static bool hasSavedData;

		public static LevelDetail.LevelSpeed levelSpeed;

		public static bool selectedSearchedLevel;

		public static void SaveData()
		{
			scnCLS instance = scnCLS.instance;
			levelsData = instance.levelsData;
			currentLevelPath = instance.levelDetail.CurrentLevelData.path;
			levelFileData = instance.CurrentLevelPendingData;
			currentLevelData = instance.levelDetail.CurrentLevelData;
			searchInput = (instance.showingSearchContent ? instance.lastSearchInput : string.Empty);
			levelSpeed = instance.levelDetail.CurrentLevelSpeedName;
			selectedSearchedLevel = instance.showingSearchContent;
			hasSavedData = true;
		}

		public static void ClearData()
		{
			_ = instance;
			levelsData = new List<CustomLevelData>();
			currentLevelPath = string.Empty;
			levelFileData = null;
			currentLevelData = new CustomLevelData();
			searchInput = string.Empty;
			levelSpeed = LevelDetail.LevelSpeed.Normal;
			hasSavedData = false;
		}
	}

	[Serializable]
	public class LevelPendingData
	{
		public Texture previewImage;

		public AudioClip clip;

		public string hash;
	}

	[Serializable]
	public class WardOption
	{
		private const float WardOptionUnselectedX = -13f;

		private const float WardOptionAnimDuration = 0.3f;

		public WardOptionName name;

		public RectTransform rect;

		public GameObject signSilhouette;

		public GameObject detailContainer;

		public string detailTitleToken;

		public string detailDescriptionToken;

		public Image signImage;

		public Image signBaseImage;

		public Image signFlashImage;

		public SpriteAnimation spriteAnimation;

		public Sprite DefaultSprite;

		public SpriteAnimation.SpriteAnimationData introAnimData;

		public SpriteAnimation.SpriteAnimationData idleAnimData;

		private Tween idleTween;

		public void Hide()
		{
			if (idleTween != null)
			{
				idleTween.Kill();
			}
			rect.DOKill();
			rect.DOAnchorPosX(-13f, 0.3f).SetEase(Ease.OutBack).SetUpdate(isIndependentUpdate: true);
			detailContainer.SetActive(value: false);
			signSilhouette.SetActive(value: true);
			signBaseImage.color = new Color(0.7f, 0.7f, 0.7f, 1f);
			spriteAnimation.Stop();
			signImage.sprite = DefaultSprite;
		}

		public void Show()
		{
			rect.DOKill();
			rect.DOAnchorPosX(0f, 0.3f).SetEase(Ease.OutBack).SetUpdate(isIndependentUpdate: true)
				.OnComplete(() =>
				{
					if (instance.SetFirstCurrentWardOptionIndex)
					{
						instance.ShowDetailStaticNoise = false;
					}
				});
			bool active = true;
			if (name == WardOptionName.OpenSteamWorkshop && !SteamWorkshop.OverlayEnabled())
			{
				active = false;
			}
			detailContainer.SetActive(active);
			signSilhouette.SetActive(value: false);
			signBaseImage.color = new Color(1f, 1f, 1f, 1f);
			spriteAnimation.currentAnimationData = introAnimData;
			spriteAnimation.Play();
			idleTween = DOVirtual.DelayedCall(0.4f, () =>
			{
				spriteAnimation.Stop();
				spriteAnimation.currentAnimationData = idleAnimData;
				spriteAnimation.Play();
			});
		}
	}

	public enum WardOptionName
	{
		Library = 0,
		OpenSteamWorkshop = 1,
		ImportLevels = 2,
		Exit = 3
	}

	private enum ChangeManyLevelsOption
	{
		First = 0,
		Last = 1,
		NextPage = 2,
		PreviousPage = 3
	}

	public const float TimeToMoveLevel = 0.1f;

	public const int MaxVisibleLevels = 11;

	public const float SearchModeLevelXPos = -31f;

	public const int FewLevelsLimit = 8;

	public const float ToggleLevelSelectionDuration = 0.6f;

	private const float FadeTransitionDuration = 0.6f;

	private const float DetailFallAnimDelay = 0.3f;

	private const float WardSelectionAnimDuration = 0.5f;

	public const float HiddenXPosOffset = 200f;

	public const float SyringeTextMaxWidth = 109f;

	public const float LevelDetailAuthorMaxWidth = 83f;

	private const int ChangeLevelPageQuantity = 6;

	public static readonly KeyCode[] LibraryDefaultShortcuts = new KeyCode[4]
	{
		KeyCode.Tab,
		KeyCode.Escape,
		KeyCode.S,
		KeyCode.Space
	};

	public static readonly KeyCode[] LibraryEmptyShortcuts = new KeyCode[3]
	{
		KeyCode.Tab,
		KeyCode.Escape,
		KeyCode.Space
	};

	public static readonly KeyCode[] LibraryDetailShortcuts = new KeyCode[3]
	{
		KeyCode.Tab,
		KeyCode.Escape,
		KeyCode.Space
	};

	public static readonly KeyCode[] SearchShortcuts = new KeyCode[3]
	{
		KeyCode.Tab,
		KeyCode.Escape,
		KeyCode.Return
	};

	public static readonly KeyCode[] WardShortcuts = new KeyCode[4]
	{
		KeyCode.Tab,
		KeyCode.Escape,
		KeyCode.I,
		KeyCode.Space
	};

	public static bool showedAnyLevel;

	public static string urlProtocolArg;

	public static bool errorLoadingLevel;

	private bool _searching;

	private bool _selectedLevel;

	private bool _canReceiveInput;

	private int _currentLevelIndex;

	private int _currentWardOptionIndex;

	private bool _showDetailStaticNoise;

	[NonSerialized]
	public bool scrollingFaster;

	[NonSerialized]
	public bool showingSearchContent;

	[NonSerialized]
	public List<int> searchLevelsDataIndex;

	private bool _showingWard;

	[NonSerialized]
	[Header("General")]
	public bool hasLoadedAnyLevel;

	[NonSerialized]
	public List<CustomLevelSyringe> showableLevels;

	[NonSerialized]
	public List<CustomLevelSyringe> visibleLevels;

	[NonSerialized]
	public List<CustomLevelData> levelsData;

	public GameObject customLevelPrefab;

	public RectTransform levelListContainer;

	public Image raycastBlocker;

	public LevelImporter levelImporter;

	public PreviewSongPlayer previewSongPlayer;

	public Image fadeTransitionImage;

	public IEnumerator sendLevelDataToLevelDetailCoroutine;

	public IEnumerator loadLevelPendingFilesCoroutine;

	public IEnumerator playLevelPreviewAudioClipCoroutine;

	public IEnumerator selectLevelCoroutine;

	public IEnumerator deselectLevelCoroutine;

	public GameObject detailStaticNoiseAnimation;

	public RDDialog dialog;

	public KeyInfoPanel infoPanel;

	public List<LevelErrorName> currentLevelErrors;

	[Header("Level Detail")]
	public LevelDetail levelDetail;

	public GameObject levelNeverSelectedDetail;

	public Text levelNeverSelectedDetailText;

	[Header("Ward")]
	public LEDSign wardLEDSign;

	public GameObject wardContainer;

	public GameObject wardDetailContainer;

	public List<WardOption> wardOptions;

	public RectTransform wardOptionsContainer;

	public RectTransform wardArrowsContainer;

	public RectTransform wardDetailTitleDescriptionRect;

	public Text wardDetailTitleText;

	public Text wardDetailTitleDescription;

	[Header("Change Level")]
	public Scrollbar levelsScrollBar;

	public CanvasGroup levelsScrollBarCanvasGroup;

	public OrderedDictionary lastLevelsPendingData;

	[Header("Level Selection")]
	public AnimationCurve SelectedLevelPushCurve;

	public AnimationCurve SelectedLevelReleaseCurve;

	public AnimationCurve SyringeDetailPushCurve;

	public AnimationCurve SyringeDetailReleaseCurve;

	[Header("Search Bar")]
	public RectTransform searchBarContainer;

	public InputField searchBarInputField;

	public Button searchBarCloseButton;

	public Button searchBarBackgroundButton;

	public Image searchBarBorder;

	public Image searchBarExteriorBorder;

	public RectTransform searchBarBackground;

	[NonSerialized]
	public int beforeSearchLevelIndex;

	[Header("Search Detail Screen")]
	public GameObject searchDetailContainer;

	public SpriteAnimation searchingIconAnim;

	public Text levelsFoundText;

	[Header("Colors")]
	public Color SearchBorderUnselectedColor = new Color(0f, 0.7960785f, 145f / 255f, 1f);

	public Color SearchExteriorBorderBrightColor = new Color(180f / 255f, 1f, 0.937255f, 1f);

	public Color SearchExteriorBorderDarkColor = new Color(0f, 202f / 255f, 144f / 255f, 1f);

	public Color SearchExteriorBorderDefaultColor = new Color(0f, 0.5333334f, 0.4313726f, 1f);

	public Color DetailOptionSquareColorSelected = new Color(0.3843138f, 0.4588236f, 189f / 255f, 1f);

	public Color DetailOptionSquareColorUnselected = new Color(189f / 255f, 0.7607844f, 0.8705883f, 1f);

	[Header("Audio")]
	public AudioSource backgroundMusicAudioSrc;

	public AudioSource staticNoiseAudioSrc;

	private int levelsScrolled;

	private bool isChangingLevel;

	private bool fadedLevels;

	private bool finishedSyringeDetailFirstPartAnim;

	private bool searchedOnce;

	private GameObject lastFocusedGameObject;

	private bool pressingChangeLevelKey;

	private float fastScrollingTime;

	private bool lastChangeLevelKeyWasDown;

	private bool canSelectLevel;

	private bool SetFirstCurrentWardOptionIndex;

	private Tweener scrollbarTween;

	private string lastSearchInput;

	private bool hidingLevels;

	private const float TimeToUpdateDetail = 0.2f;

	public const string ArtistSongHorizontalSeparator = "          ";

	public new static scnCLS instance
	{
		get
		{
			if (!(scnBase._instance is scnCLS))
			{
				return null;
			}
			return (scnCLS)scnBase._instance;
		}
	}

	public LevelPendingData CurrentLevelPendingData
	{
		get
		{
			string path = levelDetail.CurrentLevelData.path;
			if (lastLevelsPendingData.Contains(path))
			{
				return (LevelPendingData)lastLevelsPendingData[path];
			}
			return null;
		}
		set
		{
			string path = levelDetail.CurrentLevelData.path;
			if (lastLevelsPendingData.Contains(path))
			{
				lastLevelsPendingData[path] = value;
			}
		}
	}

	public int MaxHalfLength => Mathf.FloorToInt((float)visibleLevels.Count / 2f);

	public bool Searching
	{
		get
		{
			return _searching;
		}
		set
		{
			bool num = _searching == value;
			_searching = value;
			if (!_searching)
			{
				levelsFoundText.text = string.Empty;
			}
			else
			{
				canSelectLevel = false;
			}
			searchDetailContainer.SetActive(_searching);
			GameObject selectedGameObject = ((_searching && showedAnyLevel) ? searchBarInputField.gameObject : null);
			EventSystem.current.SetSelectedGameObject(selectedGameObject);
			if (num)
			{
				return;
			}
			float endValue = (_searching ? 0f : (-14f));
			searchBarBackground.DOKill();
			searchBarBackground.DOAnchorPosY(endValue, 0.3f).SetEase(Ease.OutQuint).SetUpdate(isIndependentUpdate: true);
			Color endValue2 = (_searching ? Color.white : SearchBorderUnselectedColor);
			searchBarBorder.DOKill();
			searchBarBorder.DOColor(endValue2, 0.3f).SetEase(Ease.InCirc).SetUpdate(isIndependentUpdate: true);
			searchBarExteriorBorder.DOKill();
			if (_searching)
			{
				searchBarExteriorBorder.DOColor(SearchExteriorBorderDarkColor, 0.15f).SetEase(Ease.InCirc).SetUpdate(isIndependentUpdate: true)
					.OnComplete(() =>
					{
						searchBarExteriorBorder.DOColor(SearchExteriorBorderBrightColor, 0.15f).SetEase(Ease.InCirc).SetUpdate(isIndependentUpdate: true)
							.SetLoops(-1, LoopType.Yoyo);
					});
			}
			else
			{
				searchBarExteriorBorder.DOColor(SearchExteriorBorderDefaultColor, 0.3f).SetEase(Ease.InCirc).SetUpdate(isIndependentUpdate: true);
			}
		}
	}

	public bool SelectedLevel
	{
		get
		{
			return _selectedLevel;
		}
		set
		{
			_selectedLevel = value;
			if (!_selectedLevel)
			{
				levelDetail.currentChickenSleepTime = 0f;
			}
		}
	}

	public bool CanReceiveInput
	{
		get
		{
			return _canReceiveInput;
		}
		set
		{
			_canReceiveInput = value;
			raycastBlocker.raycastTarget = !value;
		}
	}

	public int CurrentLevelIndex
	{
		get
		{
			return _currentLevelIndex;
		}
		set
		{
			ToggleCurrentSyringeScrolling(scroll: false);
			_currentLevelIndex = value;
			if (!Searching)
			{
				ToggleCurrentSyringeScrolling(scroll: true);
			}
			if (levelDetail.CurrentLevelsData.Count > 1)
			{
				levelsScrollBar.value = 1f / (float)(levelDetail.CurrentLevelsData.Count - 1) * (float)_currentLevelIndex;
			}
			else
			{
				levelsScrollBar.value = 1f;
			}
		}
	}

	public CustomLevelSyringe CurrentLevel
	{
		get
		{
			List<int> list = new List<int>();
			if (visibleLevels != null)
			{
				foreach (CustomLevelSyringe visibleLevel in visibleLevels)
				{
					list.Add(visibleLevel.currentDataIndex);
				}
			}
			if (visibleLevels == null)
			{
				return null;
			}
			return visibleLevels.Find((CustomLevelSyringe level) => level.currentDataIndex == CurrentLevelIndex);
		}
	}

	private WardOption CurrentWardOption => wardOptions[CurrentWardOptionIndex];

	private int CurrentWardOptionIndex
	{
		get
		{
			return _currentWardOptionIndex;
		}
		set
		{
			if (SetFirstCurrentWardOptionIndex)
			{
				if (_currentWardOptionIndex == value)
				{
					return;
				}
				ShowDetailStaticNoise = true;
			}
			CurrentWardOption.Hide();
			_currentWardOptionIndex = value;
			CurrentWardOption.Show();
			wardArrowsContainer.SetParent(CurrentWardOption.rect);
			wardArrowsContainer.anchoredPosition = Vector2.zero;
			wardDetailTitleText.text = RDString.Get(CurrentWardOption.detailTitleToken);
			wardDetailTitleText.font = RDString.GetAppropiateFontForString(wardDetailTitleText.text);
			string key = CurrentWardOption.detailDescriptionToken;
			float num = -59f;
			if (CurrentWardOption.name == WardOptionName.OpenSteamWorkshop && !SteamWorkshop.OverlayEnabled())
			{
				key = "customLevelSelect.ward.workshopDisabledDescription";
				num = -22f;
			}
			wardDetailTitleDescriptionRect.offsetMax = wardDetailTitleDescriptionRect.offsetMax.WithY(num);
			wardDetailTitleDescription.text = RDString.Get(key);
			wardDetailTitleDescription.font = RDString.GetAppropiateFontForString(wardDetailTitleDescription.text);
		}
	}

	public bool ShowDetailStaticNoise
	{
		get
		{
			return _showDetailStaticNoise;
		}
		set
		{
			if (_showDetailStaticNoise != value)
			{
				_showDetailStaticNoise = value;
				staticNoiseAudioSrc.DOKill();
				if (_showDetailStaticNoise)
				{
					detailStaticNoiseAnimation.SetActive(value: true);
					staticNoiseAudioSrc.DOFade(0.03f, 0.15f).SetEase(Ease.InCirc).SetUpdate(isIndependentUpdate: true);
				}
				else
				{
					staticNoiseAudioSrc.volume = 0f;
					detailStaticNoiseAnimation.SetActive(value: false);
				}
			}
		}
	}

	public bool ShowingWard
	{
		get
		{
			return _showingWard;
		}
		set
		{
			_showingWard = value;
			if (SteamIntegration.initialized)
			{
				SteamWorkshop.itemsUnsubscribed = 0;
			}
		}
	}

	private new void Awake()
	{
		base.Awake();
		scnBase._instance = this;
		muteWhenGameUnfocused = true;
		if (RDBase.Vfx == null)
		{
			RDBase.Vfx = scrVfxControl.instance;
		}
		RDRichPresence.Instance?.UpdatePresence();
		scnBase.currentLevelSelect = "scnCLS";
		DesktopLevelLoader.InitializeCacheLevelsDict();
		showableLevels = new List<CustomLevelSyringe>();
		for (int i = 0; i < 11; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(customLevelPrefab, levelListContainer);
			showableLevels.Add(gameObject.GetComponent<CustomLevelSyringe>());
			showableLevels[i].name = "Level" + i;
		}
		sendLevelDataToLevelDetailCoroutine = SendLevelDataToLevelDetail();
		loadLevelPendingFilesCoroutine = DesktopLevelLoader.LoadPendingLevelFiles(null, onlyPreviewSong: false);
		playLevelPreviewAudioClipCoroutine = PlayLevelPreviewAudioClip(0f, 0f);
		selectLevelCoroutine = SelectLevel();
		deselectLevelCoroutine = DeselectLevel();
		searchLevelsDataIndex = new List<int>();
		searchBarCloseButton.onClick.AddListener(() =>
		{
			ToggleSearchMode(showing: false);
		});
		searchBarBackgroundButton.onClick.AddListener(() =>
		{
			if (showingSearchContent && !SelectedLevel)
			{
				ToggleSearchMode(showing: false);
			}
		});
		searchBarInputField.onValueChanged.AddListener(delegate
		{
			RDEditorUtils.UpdateUIText(searchBarInputField.textComponent, searchBarInputField.text);
			if (!RDInput.cancelPress)
			{
				Search(skippingBlankSpacesFilter: false);
			}
			PlayTypingSound("CustomLevelSelect");
		});
		levelsScrollBar.transform.SetAsLastSibling();
		pressingChangeLevelKey = false;
		fadeTransitionImage.color = Color.black;
		lastLevelsPendingData = new OrderedDictionary();
		levelImporter.Initialize();
		if (!SteamIntegration.initialized)
		{
			WardOption wardOption = wardOptions.Find((WardOption wardOption3) => wardOption3.name == WardOptionName.OpenSteamWorkshop);
			wardOption.rect.transform.parent.gameObject.SetActive(value: false);
			wardOptions.Remove(wardOption);
		}
		if (scnBase.isSteamDeck)
		{
			WardOption wardOption2 = wardOptions.Find((WardOption wardOption3) => wardOption3.name == WardOptionName.ImportLevels);
			wardOption2.rect.transform.parent.gameObject.SetActive(value: false);
			wardOptions.Remove(wardOption2);
		}
		GC.twoPlayerMode = false;
		dialog.hideOnEnterPressed = true;
	}

	protected new void Start()
	{
		base.Start();
		RDBase.Vfx.hudCameraOverlay.gameObject.SetActive(value: false);
		UnityEngine.Object.FindObjectOfType<RDMainWard>(includeInactive: true)?.EnableBackgroundCharacters();
		levelsData = new List<CustomLevelData>();
		wardContainer.SetActive(value: true);
		wardDetailContainer.SetActive(value: true);
		levelDetail.loadingDetailContainer.SetActive(value: false);
		ShowingWard = true;
		CurrentWardOptionIndex = 0;
		SetFirstCurrentWardOptionIndex = true;
		wardLEDSign.Show("customLevelSelect.ward");
		infoPanel.UpdateShortcutsRow(WardShortcuts);
		bool comingback = CachedData.hasSavedData;
		fadeTransitionImage.DOFade(0f, 0.6f).SetDelay(0.3f).SetEase(Ease.InQuint)
			.SetUpdate(isIndependentUpdate: true)
			.OnComplete(() =>
			{
				if (!comingback)
				{
					CanReceiveInput = true;
				}
			});
		if (comingback)
		{
			StartCoroutine(LoadLevelsData());
		}
		else
		{
			try
			{
				DesktopLevelLoader.LoadLevelsPreviewFromPath(LevelValidation.CustomLevelsPath);
			}
			catch (Exception exception)
			{
				Debug.LogError("Error in DesktopLevelLoader.LoadLevelsPreviewFromPath. See exception below");
				Debug.LogException(exception);
			}
			if (string.IsNullOrEmpty(urlProtocolArg))
			{
				PlayPauseWardBackgroundMusic(play: true);
				levelDetail.rect.anchoredPosition = LevelDetail.IdleWardPos;
				levelDetail.canReceiveInputs = false;
				searchBarBackground.transform.SetAsFirstSibling();
				levelDetail.AnimateDetailIdle(animate: true);
			}
			else
			{
				levelImporter.Showing = true;
				levelImporter.ToggleInsertUrlContainer(show: true);
				levelImporter.urlInput.text = urlProtocolArg;
				levelImporter.ValidateUrl();
			}
			NarrateWardName(fullContext: true);
		}
		if (errorLoadingLevel)
		{
			errorLoadingLevel = false;
			string[] options = new string[1] { RDString.Get("editor.PublishLevel.dialogOK") };
			base.cls.dialog.onHideComplete = null;
			base.cls.dialog.Show(RDString.Get("customLevelSelect.errorLoadingLevel"), options, (int option) =>
			{
				base.cls.dialog.Hide();
			});
		}
	}

	private new void Update()
	{
		base.Update();
		if (SteamIntegration.initialized)
		{
			SteamIntegration.CheckCallbacks();
			if (SteamWorkshop.isDownloadingItem)
			{
				SteamWorkshop.CheckDownloadInfo();
			}
		}
		if (!SelectedLevel && levelsData.Count > 1 && levelsScrolled > 0 && !RDInput.upIsPressed && !RDInput.downIsPressed)
		{
			levelsScrolled = 0;
			ToggleScrollbar(show: false);
			if (!Searching && !hidingLevels)
			{
				StopCoroutine(sendLevelDataToLevelDetailCoroutine);
				sendLevelDataToLevelDetailCoroutine = SendLevelDataToLevelDetail();
				StartCoroutine(sendLevelDataToLevelDetailCoroutine);
				foreach (CustomLevelSyringe visibleLevel in visibleLevels)
				{
					visibleLevel.PlayPlungerIdle(visibleLevel.currentDataIndex == CurrentLevelIndex);
					if (fadedLevels)
					{
						visibleLevel.FadeOverlay(visibleLevel.currentDataIndex == CurrentLevelIndex);
					}
				}
				fadedLevels = false;
			}
		}
		if (pressingChangeLevelKey)
		{
			if ((RDInput.downIsPressed || RDInput.upIsPressed) && !isChangingLevel && !scrollingFaster)
			{
				fastScrollingTime += RDTime.unscaledDeltaTime;
				if (fastScrollingTime >= 0.5f)
				{
					scrollingFaster = true;
					fastScrollingTime = 0f;
				}
			}
			else if (!RDInput.downIsPressed && !RDInput.upIsPressed)
			{
				scrollingFaster = false;
			}
		}
		if (EventSystem.current.currentSelectedGameObject?.name == searchBarInputField?.name && (!CanReceiveInput || !showingSearchContent || !Searching))
		{
			EventSystem.current.SetSelectedGameObject(null);
		}
		if (!CanReceiveInput || levelDetail.showingErrorsContainer || levelImporter.Showing || dialog.gameObject.activeInHierarchy)
		{
			return;
		}
		if (ShowingWard)
		{
			if ((RDInput.downPress && !RDInput.upIsPressed) || (RDInput.upPress && !RDInput.downIsPressed))
			{
				int direction = (RDInput.downPress ? 1 : (-1));
				ChangeWardOption(direction);
			}
			else if (Input.GetKeyDown(KeyCode.I) && !showingSearchContent)
			{
				ChangeToImportOption();
				WardOptionSelectedFeedback();
				levelImporter.Showing = true;
			}
			else if (RDInput.anyPlayerPress)
			{
				SelectWardOption();
			}
			else if (RDInput.cancelPress)
			{
				if (CurrentWardOption.name == WardOptionName.Exit)
				{
					WardOptionSelectedFeedback();
					Exit();
				}
				else
				{
					ChangeToExitOption();
					WardOptionSelectedFeedback(sound: false);
				}
			}
			return;
		}
		if (!hasLoadedAnyLevel)
		{
			if (RDInput.cancelPress)
			{
				ToggleLevels(showLevels: false);
			}
			return;
		}
		if (showingSearchContent)
		{
			if (RDInput.cancelPress)
			{
				if (SelectedLevel)
				{
					levelDetail.CheckCancelOptions();
				}
				else
				{
					ToggleSearchMode(showing: false);
				}
				return;
			}
			if (Input.GetKeyDown(KeyCode.Return) && Searching)
			{
				if (searchLevelsDataIndex.Count > 0 && !string.IsNullOrEmpty(searchBarInputField.text))
				{
					ShowDetailStaticNoise = true;
					ToggleLevelsSearchMode(oneColumnMode: false);
					StopCoroutine(sendLevelDataToLevelDetailCoroutine);
					sendLevelDataToLevelDetailCoroutine = SendLevelDataToLevelDetail();
					StartCoroutine(sendLevelDataToLevelDetailCoroutine);
					searchedOnce = true;
				}
				else if (!levelDetail.isShaking)
				{
					levelDetail.ShakeContainer();
					CLSPlaySound("sndError");
				}
				return;
			}
			if (!Searching && (Input.GetKeyDown(KeyCode.S) || (EventSystem.current.currentSelectedGameObject?.name == searchBarInputField?.name && lastFocusedGameObject != searchBarInputField)))
			{
				if (SelectedLevel)
				{
					SelectedLevel = false;
					infoPanel.UpdateShortcutsRow(LibraryDefaultShortcuts);
					levelDetail.ChangeContentHeight(0f);
					levelDetail.AnimateDetailIdle(animate: true);
					levelDetail.ExpandDescription(expand: false);
				}
				ToggleLevelsSearchMode(oneColumnMode: true);
			}
			lastFocusedGameObject = EventSystem.current.currentSelectedGameObject;
			if (Searching)
			{
				return;
			}
		}
		if (!SelectedLevel)
		{
			if (levelDetail.CurrentLevelsData.Count > 1 && !isChangingLevel)
			{
				if (!RDInput.upIsPressed && (RDInput.downPress || (scrollingFaster && RDInput.downIsPressed)))
				{
					ChangeLevel(-1);
					return;
				}
				if (!RDInput.downIsPressed && (RDInput.upPress || (scrollingFaster && RDInput.upIsPressed)))
				{
					ChangeLevel(1);
					return;
				}
			}
			if (canSelectLevel)
			{
				if (levelDetail.CurrentLevelsData.Count > 6 && (RDInput.leftPress || RDInput.rightPress))
				{
					ChangeManyLevels(RDInput.leftPress ? ChangeManyLevelsOption.PreviousPage : ChangeManyLevelsOption.NextPage);
					return;
				}
				if (levelDetail.CurrentLevelsData.Count > 1 && ((CurrentLevelIndex != 0 && Input.GetKeyDown(KeyCode.Home)) || (CurrentLevelIndex != levelDetail.CurrentLevelsData.Count - 1 && Input.GetKeyDown(KeyCode.End))))
				{
					ChangeManyLevelsOption option = ((!Input.GetKeyDown(KeyCode.Home)) ? ChangeManyLevelsOption.Last : ChangeManyLevelsOption.First);
					ChangeManyLevels(option);
					return;
				}
			}
			if (canSelectLevel)
			{
				if (!showingSearchContent && Input.GetKeyDown(KeyCode.S))
				{
					ToggleSearchMode(showing: true);
					return;
				}
				if (Input.GetKeyDown(KeyCode.R) && levelDetail.CurrentLevelsData.Count > 1)
				{
					int customDataIndex = UnityEngine.Random.Range(0, levelDetail.CurrentLevelsData.Count);
					ShowSyringesWithIndex(levelDetail.CurrentLevelsData, customDataIndex);
					sendLevelDataToLevelDetailCoroutine = SendLevelDataToLevelDetail(loadingLevelsData: false, 0f);
					StartCoroutine(sendLevelDataToLevelDetailCoroutine);
					return;
				}
			}
			if (RDInput.anyPlayerPress && canSelectLevel)
			{
				StopCoroutine(deselectLevelCoroutine);
				selectLevelCoroutine = SelectLevel();
				StartCoroutine(selectLevelCoroutine);
			}
			else if (RDInput.cancelPress)
			{
				ToggleLevels(showLevels: false);
			}
		}
		else if (levelDetail.currentOptionScreenName != LevelDetail.ScreenName.NoLevelsFound && RDInput.cancelPress)
		{
			levelDetail.CheckCancelOptions();
		}
	}

	private void LateUpdate()
	{
		if (showingSearchContent && Input.GetKeyDown(KeyCode.Return) && Searching && (searchLevelsDataIndex.Count == 0 || string.IsNullOrEmpty(searchBarInputField.text)))
		{
			searchBarInputField.ActivateInputField();
		}
	}

	private void ChangeLevel(int direction)
	{
		if (isChangingLevel)
		{
			return;
		}
		isChangingLevel = true;
		UnityWebRequest uwrAudio = DesktopLevelLoader.uwrAudio;
		if (uwrAudio != null && !uwrAudio.isDone)
		{
			StopCoroutine(playLevelPreviewAudioClipCoroutine);
			uwrAudio.Abort();
		}
		canSelectLevel = false;
		previewSongPlayer.Stop();
		levelDetail.nonPlayableDetailContainer.SetActive(value: false);
		ShowDetailStaticNoise = true;
		StopCoroutine(playLevelPreviewAudioClipCoroutine);
		CurrentLevel.ToggleSyringeUsb(showing: false);
		if (levelsScrolled == 0)
		{
			StopCoroutine(sendLevelDataToLevelDetailCoroutine);
			StopCoroutine(loadLevelPendingFilesCoroutine);
			ToggleScrollbar(show: true);
		}
		CurrentLevelIndex -= direction;
		if (CurrentLevelIndex > levelDetail.CurrentLevelsData.Count - 1)
		{
			CurrentLevelIndex = 0;
		}
		else if (CurrentLevelIndex < 0)
		{
			CurrentLevelIndex = levelDetail.CurrentLevelsData.Count - 1;
		}
		levelsScrolled++;
		float num = 1f;
		float num2 = (((float)levelsScrolled > num) ? 0.040000003f : 0.1f);
		int num3 = 0;
		int num4 = 0;
		if (levelDetail.CurrentLevelsData.Count < 11 && levelDetail.CurrentLevelsData.Count > 8)
		{
			int num5 = 11 - levelDetail.CurrentLevelsData.Count;
			int num6 = Mathf.CeilToInt((float)num5 / 2f);
			int num7 = num5 - num6;
			num4 = 10 - num6;
			num3 += num7;
		}
		else
		{
			num4 = 10;
		}
		foreach (CustomLevelSyringe visibleLevel in visibleLevels)
		{
			bool flag = false;
			int currentPositionIndex = visibleLevel.currentPositionIndex;
			int direction2 = direction;
			if (levelDetail.CurrentLevelsData.Count < 11)
			{
				if (levelDetail.CurrentLevelsData.Count <= 8)
				{
					if ((CurrentLevelIndex == 0 && direction == -1) || (CurrentLevelIndex == levelDetail.CurrentLevelsData.Count - 1 && direction == 1))
					{
						direction2 = direction * -1;
						currentPositionIndex += (levelDetail.CurrentLevelsData.Count - 1) * (direction * -1);
						if ((direction == 1 && currentPositionIndex < 0) || (direction == -1 && currentPositionIndex >= 11))
						{
							flag = true;
							currentPositionIndex += 11 * direction;
						}
					}
					else
					{
						currentPositionIndex += direction;
						if (currentPositionIndex < 0 && direction == -1)
						{
							flag = true;
							currentPositionIndex = 11 + currentPositionIndex;
						}
						else if (currentPositionIndex >= 11 && direction == 1)
						{
							flag = true;
							currentPositionIndex = 11 - currentPositionIndex;
						}
					}
				}
				else
				{
					currentPositionIndex += direction;
					if (currentPositionIndex < num3 || currentPositionIndex > num4)
					{
						flag = true;
						currentPositionIndex = ((currentPositionIndex < num3) ? num4 : num3);
					}
				}
			}
			else if (currentPositionIndex + direction >= num3 && currentPositionIndex + direction <= num4)
			{
				currentPositionIndex += direction;
			}
			else
			{
				int currentDataIndex = visibleLevel.currentDataIndex;
				if (currentPositionIndex + direction < num3)
				{
					currentPositionIndex = num4;
					currentDataIndex = CurrentLevelIndex + MaxHalfLength;
					if (currentDataIndex >= levelDetail.CurrentLevelsData.Count)
					{
						currentDataIndex -= levelDetail.CurrentLevelsData.Count;
					}
				}
				else
				{
					currentPositionIndex = num3;
					int num8 = ((MaxHalfLength % 2 == 0) ? (MaxHalfLength - 1) : MaxHalfLength);
					currentDataIndex = CurrentLevelIndex - num8;
					if (currentDataIndex <= -1)
					{
						currentDataIndex = levelDetail.CurrentLevelsData.Count + currentDataIndex;
					}
				}
				flag = true;
				visibleLevel.currentDataIndex = currentDataIndex;
				visibleLevel.UpdateInfo(levelDetail.CurrentLevelsData[currentDataIndex]);
			}
			visibleLevel.currentPositionIndex = currentPositionIndex;
			if (scrollingFaster)
			{
				if (!fadedLevels)
				{
					visibleLevel.FadeOverlay(fadeOut: true);
				}
			}
			else
			{
				visibleLevel.FadeOverlay(visibleLevel.currentDataIndex == CurrentLevelIndex);
			}
			float timeToMove = (flag ? (-1f) : num2);
			visibleLevel.Move(direction2, timeToMove);
		}
		lastChangeLevelKeyWasDown = direction == -1;
		pressingChangeLevelKey = true;
		fadedLevels = scrollingFaster;
		string sound = (((int)CurrentLevel.CurrentRank == -3) ? "sndLibrarySelectWrapper" : "sndLibrarySelectSyringe");
		float pitch = RDUtils.PitchSemitonesToPercent(direction);
		CLSPlaySound(sound, 1f, "CustomLevelSelect", pitch);
		NarrateCurrentLevel();
		DOVirtual.DelayedCall(num2, () =>
		{
			isChangingLevel = false;
		});
	}

	private void ToggleScrollbar(bool show, bool instant = false)
	{
		float duration = (show ? 0.3f : 0.3f);
		float num = (show ? 1f : 0f);
		Ease ease = (show ? Ease.OutExpo : Ease.InExpo);
		if (show)
		{
			if (scrollbarTween != null)
			{
				scrollbarTween.Kill();
			}
			levelsScrollBarCanvasGroup.DOKill();
		}
		if (instant)
		{
			levelsScrollBarCanvasGroup.alpha = num;
		}
		else
		{
			scrollbarTween = levelsScrollBarCanvasGroup.DOFade(num, duration).SetEase(ease).SetUpdate(isIndependentUpdate: true);
		}
		if (!show && !instant)
		{
			scrollbarTween.SetDelay(1f);
		}
	}

	private List<LevelErrorName> FindCurrentLevelErrors()
	{
		List<LevelErrorName> levelSettingsErrors = LevelValidation.GetLevelSettingsErrors(levelDetail.CurrentLevelData.path, levelDetail.CurrentLevelData.settings);
		List<LevelErrorName> errorsToCheck = levelDetail.levelErrorPresentation.errorContainer.GetErrorsToCheck();
		return levelSettingsErrors.Intersect(errorsToCheck).ToList();
	}

	public IEnumerator SendLevelDataToLevelDetail(bool loadingLevelsData = false, float timeToUpdate = 0.2f)
	{
		if (timeToUpdate > 0f)
		{
			ShowDetailStaticNoise = true;
			yield return new WaitForSecondsRealtime(timeToUpdate);
		}
		CurrentLevel.ToggleSyringeUsb(showing: true);
		levelDetail.ChangeChickenAnimation(LevelDetail.ChickenAnimName.Idle);
		previewSongPlayer.Stop();
		StopCoroutine(playLevelPreviewAudioClipCoroutine);
		if ((int)CurrentLevel.CurrentRank == -3)
		{
			levelNeverSelectedDetail.SetActive(value: true);
			ShowDetailStaticNoise = false;
			canSelectLevel = true;
			levelDetail.ToggleRankTicket(-2);
			yield break;
		}
		if (!levelDetail.CurrentLevelData.isLegacyLevel)
		{
			bool levelHasErrors = false;
			if (CurrentLevelPendingData == null)
			{
				currentLevelErrors = FindCurrentLevelErrors();
				levelHasErrors = currentLevelErrors.Count > 0;
			}
			if (!levelHasErrors && (CurrentLevelPendingData == null || (CurrentLevelPendingData != null && CurrentLevelPendingData.clip == null)))
			{
				StopCoroutine(loadLevelPendingFilesCoroutine);
				bool onlyPreviewSong = CurrentLevelPendingData != null;
				loadLevelPendingFilesCoroutine = DesktopLevelLoader.LoadPendingLevelFiles(levelDetail.CurrentLevelData, onlyPreviewSong);
				yield return loadLevelPendingFilesCoroutine;
			}
			if ((CurrentLevelPendingData == null) | levelHasErrors)
			{
				levelDetail.ShowErrorLoadingData();
				canSelectLevel = true;
				ShowDetailStaticNoise = false;
				yield break;
			}
			RDLevelSettings settings = levelDetail.CurrentLevelData.settings;
			playLevelPreviewAudioClipCoroutine = PlayLevelPreviewAudioClip(settings.previewSongStartTime, settings.previewSongDuration);
			StartCoroutine(playLevelPreviewAudioClipCoroutine);
		}
		levelDetail.ShowLevelData(CurrentLevelPendingData);
		levelNeverSelectedDetail.SetActive(value: false);
		if (!loadingLevelsData)
		{
			ShowDetailStaticNoise = false;
		}
		canSelectLevel = true;
	}

	private IEnumerator PlayLevelPreviewAudioClip(float startTime, float duration)
	{
		while (CurrentLevelPendingData.clip == null)
		{
			yield return null;
		}
		previewSongPlayer.Stop();
		previewSongPlayer.Play(CurrentLevelPendingData.clip, startTime, duration, isCLS: true);
		BpmAnimationManager.instance.SetBpm(CurrentLevel.bpm, CurrentLevel.offset);
	}

	public void AddLevelFileDataToCache(Texture image, string levelPath)
	{
		int index = levelDetail.CurrentLevelsData.FindIndex((CustomLevelData levelData) => levelData.path == levelPath);
		LevelPendingData levelPendingData = new LevelPendingData();
		levelPendingData.clip = null;
		levelPendingData.previewImage = image;
		levelPendingData.hash = levelDetail.CurrentLevelsData[index].Hash;
		lastLevelsPendingData.Add(levelDetail.CurrentLevelsData[index].path, levelPendingData);
		if (lastLevelsPendingData.Count >= 10)
		{
			lastLevelsPendingData.RemoveAt(0);
		}
	}

	private void ChangeManyLevels(ChangeManyLevelsOption option)
	{
		StopCoroutine(sendLevelDataToLevelDetailCoroutine);
		StopCoroutine(playLevelPreviewAudioClipCoroutine);
		previewSongPlayer.Stop();
		int num = levelDetail.CurrentLevelsData.Count - 1;
		if (option == ChangeManyLevelsOption.First && CurrentLevelIndex == num)
		{
			ChangeLevel(-1);
			return;
		}
		if (option == ChangeManyLevelsOption.Last && CurrentLevelIndex == 0)
		{
			ChangeLevel(1);
			return;
		}
		int num2 = 0;
		int num3;
		int num4;
		switch (option)
		{
		case ChangeManyLevelsOption.First:
			num2 = 0;
			break;
		case ChangeManyLevelsOption.Last:
			num2 = num;
			break;
		case ChangeManyLevelsOption.PreviousPage:
			num3 = 1;
			goto IL_008d;
		case ChangeManyLevelsOption.NextPage:
			{
				num3 = -1;
				goto IL_008d;
			}
			IL_008d:
			num4 = num3;
			num4 *= 6;
			num2 = CurrentLevelIndex;
			num2 -= num4;
			if (num2 > num)
			{
				num2 -= num;
			}
			else if (num2 < 0)
			{
				num2 = num - num2 * -1;
			}
			break;
		}
		ShowSyringesWithIndex(levelDetail.CurrentLevelsData, num2);
		sendLevelDataToLevelDetailCoroutine = SendLevelDataToLevelDetail(loadingLevelsData: false, 0f);
		StartCoroutine(sendLevelDataToLevelDetailCoroutine);
		ToggleScrollbar(show: true, instant: true);
		ToggleScrollbar(show: false);
	}

	private IEnumerator SelectLevel()
	{
		CanReceiveInput = false;
		if ((int)CurrentLevel.CurrentRank == -3)
		{
			Persistence.SetCustomLevelRank(levelDetail.CurrentLevelData.Hash, -1);
			CurrentLevel.CurrentRank = -1;
			CurrentLevel.PlayUnwrapAnimation();
			RDLevelSettings settings = levelDetail.CurrentLevelData.settings;
			CurrentLevel.SetTextAnchoredPosition(CurrentLevel.artistText, CurrentLevel.artistRect);
			CheckTextScrolling(CurrentLevel.artistRect, CurrentLevel.artistText, settings.artist, 109f);
			CurrentLevel.SetTextAnchoredPosition(CurrentLevel.songText, CurrentLevel.songRect);
			CheckTextScrolling(CurrentLevel.songRect, CurrentLevel.songText, settings.song, 109f);
			Narration.Set(RDString.Get("customLevelSelect.unwrapping"), ElementType.VerticalList, levelsData.Count);
			NarrateCurrentLevel();
		}
		else
		{
			SelectedLevel = true;
			infoPanel.UpdateShortcutsRow(LibraryDetailShortcuts);
			MoveLevelsSyringe(selectingLevel: true);
			levelDetail.SelectLevel();
			yield return new WaitForSecondsRealtime(0.6f);
			CanReceiveInput = true;
			levelDetail.MoveDescription();
		}
	}

	public IEnumerator DeselectLevel(bool deletedLevel = false)
	{
		CanReceiveInput = false;
		SelectedLevel = false;
		infoPanel.UpdateShortcutsRow(LibraryDefaultShortcuts);
		levelDetail.CurrentOptionIndex = 0;
		MoveLevelsSyringe(selectingLevel: false, !deletedLevel);
		levelDetail.DeselectLevel();
		if (!deletedLevel)
		{
			Narration.Set(RDString.Get("customLevelSelect.ward.library"), ElementType.VerticalList, levelsData.Count);
			NarrateCurrentLevel();
		}
		yield return new WaitForSecondsRealtime(0.6f);
		CanReceiveInput = true;
	}

	public void MoveLevelsSyringe(bool selectingLevel, bool moveCurrentSyringe = true)
	{
		foreach (CustomLevelSyringe level in visibleLevels)
		{
			level.StopMove();
			Vector2 targetPos = level.GetPosition();
			if (level.currentDataIndex != CurrentLevelIndex)
			{
				if (selectingLevel)
				{
					targetPos.x -= 60f;
				}
				float delay = (float)Mathf.Abs(CurrentLevel.currentPositionIndex - level.currentPositionIndex) * 0.05f;
				level.rect.DOKill();
				level.rect.DOAnchorPos(targetPos, 0.6f).SetDelay(delay).SetEase(Ease.OutBack)
					.SetUpdate(isIndependentUpdate: true);
			}
			else
			{
				if (!moveCurrentSyringe)
				{
					continue;
				}
				if (selectingLevel)
				{
					finishedSyringeDetailFirstPartAnim = false;
					float num = (float)Screen.width * 1f / (float)Screen.height;
					float num2 = 1.7777778f;
					float num3 = (num - num2) * 198f / 2f;
					float num4 = 26f + num3;
					targetPos.x += num4;
				}
				else if (finishedSyringeDetailFirstPartAnim)
				{
					level.ToggleLiquidPressing(pressing: false);
					SyringeDetailMovement(levelDetail.rect, LevelDetail.IdleLevelsPos.x, 0.3f, goingRight: false);
				}
				level.rect.DOKill();
				Ease ease = (selectingLevel ? Ease.InBack : Ease.OutBack);
				level.rect.DOAnchorPos(targetPos, 0.3f).SetEase(ease, 6f).SetUpdate(isIndependentUpdate: true)
					.OnComplete(() =>
					{
						if (selectingLevel)
						{
							SyringeDetailMovement(level.rect, targetPos.x, 0.3f, goingRight: true);
							SyringeDetailMovement(levelDetail.rect, LevelDetail.IdleLevelsPos.x, 0.3f, goingRight: true);
							level.ToggleLiquidPressing(pressing: true);
						}
					});
			}
		}
	}

	private void SyringeDetailMovement(RectTransform rect, float startXPos, float duration, bool goingRight)
	{
		int num = (goingRight ? 1 : (-1));
		rect.DOAnchorPosX(startXPos + 15f * (float)num, duration * 0.3f).SetEase(SyringeDetailPushCurve).SetUpdate(isIndependentUpdate: true)
			.OnStart(() =>
			{
				finishedSyringeDetailFirstPartAnim = true;
			})
			.OnComplete(() =>
			{
				rect.DOAnchorPosX(startXPos, duration * 0.7f).SetUpdate(isIndependentUpdate: true).SetEase(SyringeDetailReleaseCurve);
			});
	}

	private void ToggleSearchMode(bool showing, bool skipAnimations = false)
	{
		showingSearchContent = showing;
		Searching = showing;
		StopCoroutine(sendLevelDataToLevelDetailCoroutine);
		ToggleSearchBar(showing, skipAnimations);
		if (showing)
		{
			ShowDetailStaticNoise = false;
			searchLevelsDataIndex = new List<int>();
			for (int i = 0; i < levelsData.Count; i++)
			{
				searchLevelsDataIndex.Add(i);
			}
			infoPanel.UpdateShortcutsRow(SearchShortcuts);
		}
		if (showing)
		{
			beforeSearchLevelIndex = CurrentLevelIndex;
			StopCoroutine(playLevelPreviewAudioClipCoroutine);
			CurrentLevelIndex = 0;
		}
		else
		{
			levelsFoundText.text = string.Empty;
			SelectedLevel = false;
			infoPanel.UpdateShortcutsRow(LibraryDefaultShortcuts);
			int customDataIndex = beforeSearchLevelIndex;
			if ((searchedOnce && searchLevelsDataIndex.Count > CurrentLevelIndex) || CachedData.selectedSearchedLevel)
			{
				customDataIndex = searchLevelsDataIndex[CurrentLevelIndex];
				CachedData.selectedSearchedLevel = false;
			}
			searchedOnce = false;
			if (CurrentLevel != null)
			{
				CurrentLevel.ToggleSyringeUsb(showing: false, moveInstantly: true);
			}
			ShowSyringes(levelDetail.CurrentLevelsData, customDataIndex);
			CurrentLevel.FadeOverlay(fadeOut: true);
			CurrentLevel.ToggleSyringeUsb(showing: true);
			CurrentLevel.ToggleLiquidPressing(pressing: false);
			CurrentLevel.PlayPlungerIdle(play: true);
			StopCoroutine(sendLevelDataToLevelDetailCoroutine);
			sendLevelDataToLevelDetailCoroutine = SendLevelDataToLevelDetail();
			StartCoroutine(sendLevelDataToLevelDetailCoroutine);
			infoPanel.UpdateShortcutsRow(LibraryDefaultShortcuts);
		}
		if (!skipAnimations)
		{
			ToggleLevelsSearchMode(showing);
		}
	}

	private void ToggleSearchBar(bool showing, bool skipAnimation)
	{
		float endValue = (showing ? (-4f) : 17f);
		searchBarContainer.DOKill();
		if (showing)
		{
			searchBarInputField.text = string.Empty;
		}
		if (skipAnimation)
		{
			searchBarContainer.anchoredPosition = new Vector2(searchBarContainer.anchoredPosition.x, endValue);
		}
		else
		{
			searchBarContainer.DOAnchorPosY(endValue, 0.7f).SetUpdate(isIndependentUpdate: true).SetEase(Ease.OutExpo);
		}
	}

	public void ToggleLevelsSearchMode(bool oneColumnMode, bool skipAnimations = false)
	{
		if (oneColumnMode)
		{
			levelDetail.ToggleRankTicket(-2);
		}
		ToggleCurrentSyringeScrolling(!oneColumnMode);
		if (!skipAnimations)
		{
			string sound = (oneColumnMode ? "sndLibrarySearchOpen" : "sndLibrarySearchClose");
			CLSPlaySound(sound);
		}
		Searching = oneColumnMode;
		foreach (CustomLevelSyringe visibleLevel in visibleLevels)
		{
			if (showedAnyLevel)
			{
				Vector2 position = visibleLevel.GetPosition();
				if (oneColumnMode)
				{
					position.x = -31f;
				}
				visibleLevel.rect.DOKill();
				if (skipAnimations)
				{
					visibleLevel.rect.anchoredPosition = position;
				}
				else
				{
					visibleLevel.rect.DOAnchorPos(position, 0.2f).SetEase(Ease.OutBack).SetUpdate(isIndependentUpdate: true);
				}
			}
			if (oneColumnMode)
			{
				visibleLevel.FadeOverlay(fadeOut: true, !skipAnimations);
			}
			else
			{
				bool fadeOut = (showingSearchContent && visibleLevel.currentDataIndex == CurrentLevelIndex) || (!showingSearchContent && levelsData.Count > 0 && visibleLevel.currentDataIndex == CurrentLevelIndex);
				visibleLevel.FadeOverlay(fadeOut, !skipAnimations);
			}
			if (!oneColumnMode && visibleLevel.currentDataIndex == CurrentLevelIndex && showingSearchContent)
			{
				visibleLevel.ToggleLiquidPressing(pressing: false);
				visibleLevel.PlayPlungerIdle(play: true);
			}
			else
			{
				visibleLevel.ToggleSyringeUsb(showing: false);
				visibleLevel.PlayPlungerIdle(play: false);
			}
		}
		if (oneColumnMode)
		{
			searchingIconAnim.Play();
		}
		else
		{
			searchingIconAnim.Stop();
		}
		searchBarBackgroundButton.enabled = oneColumnMode;
		if (oneColumnMode)
		{
			Narration.Say(RDString.Get("customLevelSelect.searchMode"), NarrationCategory.Navigation, -1, -1, ElementType.Inputfield, null, null, false);
			Narration.SetInstructions(RDString.Get("customLevelSelect.searchInstructions"), NarrationActionName.ToViewSearchResults, NarrationActionName.ToGoBack);
		}
		else
		{
			NarrateCurrentLevel();
		}
	}

	public void Search(bool skippingBlankSpacesFilter, CustomLevelData customLevelToSelect = null)
	{
		string text = searchBarInputField.text.ToLower();
		Dictionary<string, List<string>> filters = new Dictionary<string, List<string>>();
		if (!skippingBlankSpacesFilter && (text.EndsWith(" ") || !showingSearchContent))
		{
			return;
		}
		if (text.Contains(":"))
		{
			printe("Searching for filters!");
			text = string.Join(" ", text.Split(" ").Where((string f) =>
			{
				if (!f.Contains(":"))
				{
					return true;
				}
				string[] array = f.Split(":");
				if (array.Length == 2)
				{
					switch (array[0])
					{
					case "rank":
					case "diff":
					case "difficulty":
					case "2p":
					case "played":
					{
						string text2 = array[0];
						string[] array2 = array[1].ToLowerInvariant().Split(",");
						if (text2 == "difficulty")
						{
							text2 = "diff";
						}
						string[] array3 = array2;
						foreach (string item in array3)
						{
							if (!filters.ContainsKey(text2))
							{
								filters.Add(text2, new List<string> { item });
							}
							else if (!filters[text2].Contains(item))
							{
								filters[text2].Add(item);
							}
						}
						return array2.Length < 1;
					}
					}
				}
				return true;
			}));
		}
		SetSearchData(text, filters);
		levelsFoundText.text = RDString.Get("customLevelSelect.searchLevelsFound");
		levelsFoundText.text = levelsFoundText.text.Replace("[levels]", searchLevelsDataIndex.Count.ToString());
		levelsFoundText.font = RDString.GetAppropiateFontForString(levelsFoundText.text);
		Narration.Say(text + " " + levelsFoundText.text, NarrationCategory.Navigation, false);
		Narration.SetInstructions(RDString.Get("customLevelSelect.searchInstructions"), NarrationActionName.ToViewSearchResults, NarrationActionName.ToGoBack);
		List<CustomLevelData> list = new List<CustomLevelData>();
		int customDataIndex = -1;
		foreach (int item2 in searchLevelsDataIndex)
		{
			list.Add(levelsData[item2]);
			if (customLevelToSelect != null && levelsData[item2] == customLevelToSelect)
			{
				customDataIndex = list.Count - 1;
			}
		}
		ShowSyringes(list, customDataIndex);
	}

	public void SetSearchData(string textToSearch, Dictionary<string, List<string>> filters)
	{
		lastSearchInput = textToSearch;
		searchLevelsDataIndex = new List<int>();
		int num = 0;
		foreach (CustomLevelData levelsDatum in levelsData)
		{
			if (levelsDatum.settings.song.ToLower().RemoveRichTextColorTags().Contains(textToSearch) || levelsDatum.settings.artist.ToLower().RemoveRichTextColorTags().Contains(textToSearch) || levelsDatum.settings.author.ToLower().RemoveRichTextColorTags().Contains(textToSearch) || levelsDatum.settings.tags.ToLower().Contains(textToSearch))
			{
				searchLevelsDataIndex.Add(num);
			}
			num++;
		}
		foreach (var (text2, vals) in filters)
		{
			switch (text2)
			{
			case "rank":
			{
				List<int> ranks = GetSearchRanks(vals);
				if (ranks.Count() > 0)
				{
					searchLevelsDataIndex = searchLevelsDataIndex.Where((int i) => ranks.Contains(Persistence.GetCustomLevelRank(levelsData[i].Hash))).ToList();
				}
				break;
			}
			case "diff":
			case "difficulty":
			{
				List<LevelDifficulty> diffs = GetSearchDifficulties(vals);
				if (diffs.Count() > 0)
				{
					searchLevelsDataIndex = searchLevelsDataIndex.Where((int i) => diffs.Contains(levelsData[i].settings.difficulty)).ToList();
				}
				break;
			}
			case "2p":
			{
				bool? is2P = GetSearchBool(vals);
				if (is2P.HasValue)
				{
					searchLevelsDataIndex = searchLevelsDataIndex.Where((int i) =>
					{
						bool value = is2P.Value;
						LevelPlayMode canBePlayedOn = levelsData[i].settings.canBePlayedOn;
						return value == (canBePlayedOn == LevelPlayMode.TwoPlayerOnly || canBePlayedOn == LevelPlayMode.BothModes);
					}).ToList();
				}
				break;
			}
			case "played":
			{
				bool? isPlayed = GetSearchBool(vals);
				if (isPlayed.HasValue)
				{
					searchLevelsDataIndex = searchLevelsDataIndex.Where((int i) =>
					{
						bool value = isPlayed.Value;
						int customLevelRank = Persistence.GetCustomLevelRank(levelsData[i].Hash);
						return value == (customLevelRank != -1 && customLevelRank != -3 && customLevelRank != -2);
					}).ToList();
				}
				break;
			}
			}
		}
	}

	private bool? GetSearchBool(List<string> vals)
	{
		bool? result = null;
		foreach (string val in vals)
		{
			bool? flag;
			switch (val)
			{
			default:
				flag = null;
				break;
			case "0":
			case "f":
			case "n":
			case "no":
			case "false":
				flag = false;
				break;
			case "1":
			case "t":
			case "y":
			case "yes":
			case "true":
				flag = true;
				break;
			}
			bool? flag2 = flag;
			if (flag2.HasValue)
			{
				if (!result.HasValue)
				{
					result = flag2.Value;
				}
				else if (result.HasValue && result.Value != flag2.Value)
				{
					return null;
				}
			}
		}
		return result;
	}

	private List<int> GetSearchRanks(List<string> vals)
	{
		List<int> list = new List<int>();
		foreach (string val in vals)
		{
			int num = Rank.FromString(val.ToUpperInvariant(), -1);
			if (num != -2 && num != -1 && num != -3)
			{
				Rank rank = num;
				list.Add(rank);
				if ((int)rank.ToNormal() == (int)rank)
				{
					list.Add(rank.ToPlus());
					list.Add(rank.ToMinus());
				}
			}
		}
		return list;
	}

	private List<LevelDifficulty> GetSearchDifficulties(List<string> vals)
	{
		List<LevelDifficulty> list = new List<LevelDifficulty>();
		foreach (string val in vals)
		{
			if (val == "e" || val == "easy" || val == GetLoc("Easy"))
			{
				list.Add(LevelDifficulty.Easy);
				continue;
			}
			switch (val)
			{
			default:
				if (!(val == GetLoc("Medium")))
				{
					switch (val)
					{
					default:
						if (!(val == GetLoc("Tough")))
						{
							switch (val)
							{
							default:
								if (!(val == GetLoc("VeryTough")))
								{
									goto end_IL_00a2;
								}
								break;
							case "v":
							case "very":
							case "vt":
							case "vtough":
							case "verytough":
							case "vh":
							case "vhard":
							case "veryhard":
								break;
							}
							list.Add(LevelDifficulty.VeryTough);
							break;
						}
						goto case "t";
					case "t":
					case "tough":
					case "h":
					case "hard":
						{
							list.Add(LevelDifficulty.Tough);
							break;
						}
						end_IL_00a2:
						break;
					}
					break;
				}
				goto case "m";
			case "m":
			case "med":
			case "medium":
				list.Add(LevelDifficulty.Medium);
				break;
			}
		}
		return list;
		static string GetLoc(string v)
		{
			return RDString.Get("enum.levelDifficulty." + v).Replace(" ", "").ToLowerInvariant();
		}
	}

	public void ChangeWardOption(int direction)
	{
		int num = CurrentWardOptionIndex + direction;
		if (num > wardOptions.Count - 1)
		{
			CurrentWardOptionIndex = 0;
		}
		else if (num < 0)
		{
			CurrentWardOptionIndex = wardOptions.Count - 1;
		}
		else
		{
			CurrentWardOptionIndex = num;
		}
		string sound = "";
		switch (CurrentWardOption.name)
		{
		case WardOptionName.Library:
			sound = "sndWardSelectLibrary";
			break;
		case WardOptionName.OpenSteamWorkshop:
			sound = "sndWardSelectWorkshop";
			break;
		case WardOptionName.ImportLevels:
			sound = "sndWardSelectImport";
			break;
		case WardOptionName.Exit:
			sound = "sndWardSelectExit";
			break;
		}
		CLSPlaySound("sndWardSelect");
		CLSPlaySound(sound);
		NarrateWardName();
	}

	private void WardOptionSelectedFeedback(bool sound = true)
	{
		CurrentWardOption.signFlashImage.color = Color.white;
		CurrentWardOption.signFlashImage.DOKill();
		CurrentWardOption.signFlashImage.DOFade(0f, 1f).SetUpdate(isIndependentUpdate: true).SetEase(Ease.OutQuint);
		if (sound)
		{
			string text = "";
			switch (CurrentWardOption.name)
			{
			case WardOptionName.OpenSteamWorkshop:
				text = "sndWardButtonWorkshop";
				break;
			case WardOptionName.Exit:
				text = "sndWardButtonExit";
				break;
			}
			CLSPlaySound("sndWardButton");
			if (!text.IsNullOrEmpty())
			{
				CLSPlaySound(text);
			}
		}
	}

	private void SelectWardOption()
	{
		WardOptionSelectedFeedback();
		CanReceiveInput = false;
		switch (CurrentWardOption.name)
		{
		case WardOptionName.Library:
			if (SteamIntegration.initialized)
			{
				SteamWorkshop.ClearItemsInfoCache();
			}
			StartCoroutine(LoadLevelsData());
			break;
		case WardOptionName.OpenSteamWorkshop:
			if (SteamIntegration.initialized)
			{
				if (SteamWorkshop.OverlayEnabled())
				{
					SteamWorkshop.OpenWorkshop();
					break;
				}
				Application.OpenURL("https://steamcommunity.com//workshop/browse?appid=774181");
				CanReceiveInput = true;
			}
			break;
		case WardOptionName.ImportLevels:
			levelImporter.Showing = true;
			break;
		case WardOptionName.Exit:
			Exit();
			break;
		}
	}

	private void ChangeToImportOption()
	{
		int currentWardOptionIndex = wardOptions.FindIndex((WardOption wardOption) => wardOption.name == WardOptionName.ImportLevels);
		CurrentWardOptionIndex = currentWardOptionIndex;
	}

	private void ChangeToExitOption()
	{
		CLSPlaySound("sndWardSelectExit");
		int currentWardOptionIndex = wardOptions.FindIndex((WardOption wardOption) => wardOption.name == WardOptionName.Exit);
		CurrentWardOptionIndex = currentWardOptionIndex;
	}

	public void Exit()
	{
		CanReceiveInput = false;
		PlayPauseWardBackgroundMusic(play: false);
		wardLEDSign.Hide();
		wardOptionsContainer.DOKill();
		wardOptionsContainer.DOAnchorPosX(-129f, 0.6f).SetEase(Ease.OutCubic).SetUpdate(isIndependentUpdate: true);
		levelDetail.rect.DOKill();
		levelDetail.rect.DOAnchorPosX(352f, 0.6f).SetEase(Ease.OutCubic).SetUpdate(isIndependentUpdate: true);
		fadeTransitionImage.DOKill();
		fadeTransitionImage.DOFade(1f, 0.6f).SetDelay(0.3f).SetEase(Ease.InCubic)
			.SetUpdate(isIndependentUpdate: true)
			.OnComplete(() =>
			{
				scnBase.GoToMainMenu();
			});
	}

	public IEnumerator LoadLevelsData(float timeToLoadLevels = -1f)
	{
		ShowingWard = false;
		wardDetailContainer.SetActive(value: false);
		levelDetail.loadingDetailContainer.SetActive(value: true);
		Narration.Say(RDString.Get("customLevelSelect.loading"), NarrationCategory.Navigation, false);
		bool endedWardSelectionAnimation = false;
		wardLEDSign.Hide();
		wardOptionsContainer.DOKill();
		wardOptionsContainer.DOAnchorPosX(-129f, 0.3f).SetEase(Ease.InExpo).SetUpdate(isIndependentUpdate: true)
			.OnComplete(() =>
			{
				endedWardSelectionAnimation = true;
			});
		yield return new WaitUntil(() => endedWardSelectionAnimation);
		CanReceiveInput = false;
		if (timeToLoadLevels > 0f)
		{
			levelDetail.noLevelsFound.SetActive(value: false);
			yield return new WaitForSecondsRealtime(timeToLoadLevels);
		}
		DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
		levelsData = new List<CustomLevelData>();
		if (CachedData.hasSavedData)
		{
			levelsData = CachedData.levelsData;
		}
		else if (SteamIntegration.initialized)
		{
			yield return SteamWorkshop.GetLocalItems();
		}
		else
		{
			try
			{
				DesktopLevelLoader.LoadLevelsPreviewFromPath(LevelValidation.CustomLevelsPath);
			}
			catch (Exception exception)
			{
				Debug.LogError("Error in DesktopLevelLoader.LoadLevelsPreviewFromPath. See exception below.");
				Debug.LogException(exception);
				levelsData = new List<CustomLevelData>();
			}
		}
		if (!CachedData.hasSavedData)
		{
			DesktopLevelLoader.RemoveUnusedLevelsCacheFromDict();
			DesktopLevelLoader.SaveCacheLevelsDict();
		}
		List<CustomLevelData> list = new List<CustomLevelData>();
		for (int num = 0; num < levelsData.Count; num++)
		{
			if (!levelsData[num].isLegacyLevel)
			{
				Rank rank = Persistence.GetCustomLevelRank(levelsData[num].Hash);
				if ((int)rank == -2 || (int)rank == -3)
				{
					list.Add(levelsData[num]);
					levelsData.RemoveAt(num);
					num--;
				}
			}
		}
		list = list.OrderBy((CustomLevelData l) => l.settings.song.RemoveRichTextColorTags()).ToList();
		levelsData = levelsData.OrderBy((CustomLevelData l) => l.settings.song.RemoveRichTextColorTags()).ToList();
		levelsData.InsertRange(0, list);
		scnEditor.cameFromCustomLevelSelect = false;
		hasLoadedAnyLevel = levelsData.Count > 0;
		int customDataIndex = -1;
		if (CachedData.levelFileData != null)
		{
			lastLevelsPendingData.Add(CachedData.currentLevelPath, CachedData.levelFileData);
			customDataIndex = levelsData.IndexOf(CachedData.currentLevelData);
		}
		if (!string.IsNullOrEmpty(CachedData.searchInput))
		{
			searchedOnce = false;
			ToggleSearchMode(showing: true, skipAnimations: true);
			searchBarInputField.text = CachedData.searchInput;
			Search(skippingBlankSpacesFilter: true, CachedData.currentLevelData);
			ToggleLevelsSearchMode(oneColumnMode: false, skipAnimations: true);
		}
		else
		{
			ShowSyringes(levelDetail.CurrentLevelsData, customDataIndex, loadingLevelsData: true);
		}
		CachedData.ClearData();
		levelDetail.noLevelsFound.SetActive(!hasLoadedAnyLevel);
		if (!hasLoadedAnyLevel)
		{
			levelDetail.currentOptionScreenName = LevelDetail.ScreenName.NoLevelsFound;
			levelDetail.CurrentOptionIndex = 0;
			BpmMovementAnimation[] selectorFrameCorners = levelDetail.selectorFrameCorners;
			for (int num2 = 0; num2 < selectorFrameCorners.Length; num2++)
			{
				selectorFrameCorners[num2].Play();
			}
			infoPanel.UpdateShortcutsRow(LibraryEmptyShortcuts);
			ShowingWard = false;
			wardDetailContainer.SetActive(value: false);
			levelDetail.rect.DOKill();
			levelDetail.rect.DOAnchorPos(new Vector2(0f, LevelDetail.IdleWardPos.y), 0.5f).SetUpdate(isIndependentUpdate: true).SetEase(Ease.OutBack)
				.OnComplete(() =>
				{
					levelDetail.loadingDetailContainer.SetActive(value: false);
					CanReceiveInput = true;
					levelDetail.canReceiveInputs = true;
					levelDetail.AnimateDetailIdle(animate: true);
				});
			levelDetail.NarrateIntro();
		}
		else
		{
			BpmMovementAnimation[] selectorFrameCorners = levelDetail.selectorFrameCorners;
			for (int num2 = 0; num2 < selectorFrameCorners.Length; num2++)
			{
				selectorFrameCorners[num2].Stop();
			}
			EventSystem.current.SetSelectedGameObject(null);
			levelDetail.chickenAnimation.Stop();
			BpmAnimationManager.instance.SetBpm(CurrentLevel.bpm);
			yield return SendLevelDataToLevelDetail(loadingLevelsData: true, -1f);
			yield return new WaitForSecondsRealtime(0.1f);
			CurrentLevel.PlayPlungerIdle(play: true);
			ToggleLevels(showLevels: true);
		}
	}

	private void ToggleLevels(bool showLevels)
	{
		hidingLevels = !showLevels;
		if (hidingLevels)
		{
			if (sendLevelDataToLevelDetailCoroutine != null)
			{
				StopCoroutine(sendLevelDataToLevelDetailCoroutine);
			}
			if (playLevelPreviewAudioClipCoroutine != null)
			{
				StopCoroutine(playLevelPreviewAudioClipCoroutine);
			}
		}
		ToggleCurrentSyringeScrolling(showLevels);
		if (!showLevels)
		{
			if (scrollbarTween != null)
			{
				scrollbarTween.Kill();
			}
			levelsScrollBarCanvasGroup.DOKill();
			levelsScrollBarCanvasGroup.DOFade(0f, 0.2f).SetEase(Ease.InExpo).SetUpdate(isIndependentUpdate: true);
			ShowDetailStaticNoise = true;
		}
		else
		{
			levelDetail.loadingDetailContainer.SetActive(value: true);
			CLSPlaySound("sndLibraryOpen");
		}
		CanReceiveInput = false;
		PlayPauseWardBackgroundMusic(!showLevels);
		float num = (showLevels ? 0.8f : 0.4f);
		if (visibleLevels != null && visibleLevels.Count > 0)
		{
			foreach (CustomLevelSyringe level in visibleLevels)
			{
				int num2 = 0;
				int num3 = Mathf.Abs(CurrentLevelIndex - level.currentDataIndex);
				if (levelsData.Count <= 8)
				{
					num2 = num3;
				}
				else
				{
					int num4 = levelsData.Count - num3;
					num2 = ((num3 < num4) ? num3 : num4);
				}
				float num5 = (float)num2 * (num * 0.2f);
				float currentSyringeAnimDuration = num - num5;
				Vector2 targetPos = level.GetPosition();
				if (!showLevels)
				{
					targetPos.x -= 200f;
				}
				level.rect.DOKill();
				if (!showLevels || level.currentDataIndex != CurrentLevelIndex)
				{
					level.rect.DOAnchorPosX(targetPos.x, currentSyringeAnimDuration).SetEase(Ease.OutBack, 0.95f).SetUpdate(isIndependentUpdate: true);
					continue;
				}
				level.rect.DOAnchorPosX(5.5f, currentSyringeAnimDuration * 0.6f).SetDelay(0.1f).SetEase(Ease.OutQuad)
					.SetUpdate(isIndependentUpdate: true)
					.OnComplete(() =>
					{
						level.rect.DOAnchorPosX(targetPos.x, currentSyringeAnimDuration * 0.4f).SetEase(Ease.InOutQuad).SetUpdate(isIndependentUpdate: true);
					});
			}
		}
		if (!showLevels)
		{
			CLSPlaySound("sndLibraryExit");
			previewSongPlayer.Stop(0.2f);
			wardLEDSign.Show("customLevelSelect.ward");
			wardOptionsContainer.DOKill();
			if (CurrentLevel != null)
			{
				wardOptionsContainer.DOAnchorPosX(0f, 0.5f).SetEase(Ease.OutExpo).SetUpdate(isIndependentUpdate: true)
					.SetDelay(0.4f)
					.OnStart(() =>
					{
						CurrentLevel.ToggleSyringeUsb(showing: false);
					});
			}
			else
			{
				wardOptionsContainer.DOAnchorPosX(0f, 0.5f).SetEase(Ease.OutExpo).SetUpdate(isIndependentUpdate: true);
			}
			levelDetail.chickenBpmAnimation.OnStopAction = null;
			levelDetail.chickenBpmAnimation.Stop();
			levelDetail.chickenAnimation.Play();
		}
		levelDetail.rect.DOKill();
		float endValue = (showLevels ? LevelDetail.IdleLevelsPos.x : LevelDetail.IdleWardPos.x);
		levelDetail.rect.DOAnchorPosX(endValue, 0.48000002f).SetEase(Ease.OutBack).SetUpdate(isIndependentUpdate: true)
			.OnComplete(() =>
			{
				levelDetail.canReceiveInputs = showLevels;
				CanReceiveInput = true;
				if (showLevels)
				{
					levelDetail.loadingDetailContainer.SetActive(value: false);
				}
				else
				{
					levelDetail.ToggleRankTicket(-2);
					ShowDetailStaticNoise = false;
					if (CurrentLevel != null)
					{
						CurrentLevel.PlayPlungerIdle(play: false);
					}
				}
				StopCoroutine(playLevelPreviewAudioClipCoroutine);
				showedAnyLevel = true;
				ShowingWard = !showLevels;
				levelDetail.AnimateDetailIdle(animate: true);
				wardDetailContainer.SetActive(!showLevels);
				KeyCode[] shortcuts = (showLevels ? LibraryDefaultShortcuts : WardShortcuts);
				infoPanel.UpdateShortcutsRow(shortcuts);
				if (SteamIntegration.initialized)
				{
					infoPanel.canRestart = true;
				}
			});
		if (showLevels)
		{
			Narration.Set(RDString.Get("customLevelSelect.ward.library"), ElementType.VerticalList, levelsData.Count);
			NarrateCurrentLevel();
		}
		else
		{
			Narration.Set(RDString.Get("customLevelSelect.ward"), ElementType.VerticalMenu, wardOptions.Count);
			NarrateWardName();
		}
	}

	private void PlayPauseWardBackgroundMusic(bool play)
	{
		float endValue = (play ? 0.45f : 0f);
		backgroundMusicAudioSrc.DOKill();
		if (play)
		{
			backgroundMusicAudioSrc.Play();
		}
		backgroundMusicAudioSrc.DOFade(endValue, 0.5f).SetEase(Ease.OutExpo).SetUpdate(isIndependentUpdate: true)
			.OnComplete(() =>
			{
				if (!play)
				{
					backgroundMusicAudioSrc.Stop();
				}
			});
	}

	private void ShowSyringesWithIndex(List<CustomLevelData> levelsData, int customDataIndex)
	{
		ShowSyringes(levelsData, customDataIndex);
		foreach (CustomLevelSyringe visibleLevel in visibleLevels)
		{
			bool flag = visibleLevel == CurrentLevel;
			visibleLevel.PlayPlungerIdle(flag);
			visibleLevel.ToggleSyringeUsb(flag);
			visibleLevel.FadeOverlay(flag);
		}
	}

	private void ShowSyringes(List<CustomLevelData> levelsData, int customDataIndex, bool loadingLevelsData = false)
	{
		CurrentLevelIndex = ((customDataIndex >= 0 && customDataIndex <= levelsData.Count - 1) ? customDataIndex : 0);
		ToggleCurrentSyringeScrolling(scroll: false);
		if (levelsData.Count == 0)
		{
			foreach (CustomLevelSyringe showableLevel in showableLevels)
			{
				showableLevel.gameObject.SetActive(value: false);
			}
			return;
		}
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = ((showableLevels.Count <= levelsData.Count) ? showableLevels.Count : levelsData.Count);
		int num5 = Mathf.FloorToInt((float)showableLevels.Count / 2f);
		visibleLevels = new List<CustomLevelSyringe>();
		num2 = ((customDataIndex != -1) ? customDataIndex : 0);
		if (num4 <= 8)
		{
			num3 = num5 - num2;
			num2 = 0;
		}
		else
		{
			num2 = ((customDataIndex < 0) ? (levelsData.Count - Mathf.FloorToInt((float)num4 / 2f)) : (customDataIndex - Mathf.FloorToInt((float)num4 / 2f)));
			if (num2 < 0)
			{
				num2 = levelsData.Count + num2;
			}
			num3 = num5 - Mathf.FloorToInt((float)num4 / 2f);
		}
		int num6 = num5;
		int num7 = num6;
		foreach (CustomLevelSyringe showableLevel2 in showableLevels)
		{
			if (num2 >= levelsData.Count)
			{
				num2 = 0;
			}
			if (num < num4)
			{
				if (num3 > num7)
				{
					num7 = num3;
				}
				if (num3 < num6)
				{
					num6 = num3;
				}
				showableLevel2.gameObject.SetActive(value: true);
				visibleLevels.Add(showableLevel2);
				showableLevel2.currentPositionIndex = num3;
				showableLevel2.currentDataIndex = num2;
				showableLevel2.UpdateInfo(levelsData[num2]);
				Vector2 position = showableLevel2.GetPosition();
				if (!loadingLevelsData)
				{
					showableLevel2.rect.DOKill();
					showableLevel2.rect.DOAnchorPos(position, 0.1f).SetEase(Ease.OutBack, 0.95f).SetUpdate(isIndependentUpdate: true);
				}
				else
				{
					position.x -= 200f;
					showableLevel2.rect.anchoredPosition = position;
				}
				if (Searching && showedAnyLevel)
				{
					Vector2 position2 = showableLevel2.GetPosition();
					position2.x = -31f;
					showableLevel2.rect.DOKill();
					showableLevel2.rect.DOAnchorPos(position2, 0.1f).SetUpdate(isIndependentUpdate: true).SetEase(Ease.OutExpo);
				}
			}
			else
			{
				showableLevel2.gameObject.SetActive(value: false);
			}
			num++;
			num2++;
			num3++;
			if (showableLevels.Count > 8 && num3 >= showableLevels.Count)
			{
				num3 = 0;
			}
		}
		ToggleCurrentSyringeScrolling(scroll: true);
		levelsScrollBar.size = Mathf.Max(0.04f, 1f / (float)levelsData.Count);
	}

	public void ToggleCurrentSyringeScrolling(bool scroll)
	{
		if (CurrentLevel == null || levelDetail.CurrentLevelData == null)
		{
			return;
		}
		if (levelDetail.CurrentLevelData == null)
		{
			RDBaseDllDummy.printem("levelDetail.CurrentLevelData null");
		}
		else if (scroll)
		{
			RDLevelSettings settings = levelDetail.CurrentLevelData.settings;
			if ((int)CurrentLevel.CurrentRank == -3)
			{
				CurrentLevel.SetTextAnchoredPosition(CurrentLevel.artistText, CurrentLevel.artistRect);
				CheckTextScrolling(CurrentLevel.boxArtistRect, CurrentLevel.boxArtistText, settings.artist, 109f);
				CurrentLevel.SetTextAnchoredPosition(CurrentLevel.songText, CurrentLevel.songRect);
				CheckTextScrolling(CurrentLevel.boxSongRect, CurrentLevel.boxSongText, settings.song, 109f);
			}
			else
			{
				CurrentLevel.SetTextAnchoredPosition(CurrentLevel.artistText, CurrentLevel.artistRect);
				CheckTextScrolling(CurrentLevel.artistRect, CurrentLevel.artistText, settings.artist, 109f);
				CurrentLevel.SetTextAnchoredPosition(CurrentLevel.songText, CurrentLevel.songRect);
				CheckTextScrolling(CurrentLevel.songRect, CurrentLevel.songText, settings.song, 109f);
			}
		}
		else if ((int)CurrentLevel.CurrentRank == -3)
		{
			CheckStopTextScrolling(CurrentLevel.boxArtistRect, CurrentLevel.boxArtistText, 109f, 0.1f, -109f);
			CheckStopTextScrolling(CurrentLevel.boxSongRect, CurrentLevel.boxSongText, 109f, 0.1f, -109f);
		}
		else
		{
			CheckStopTextScrolling(CurrentLevel.artistRect, CurrentLevel.artistText, 109f, 0.1f, -109f);
			CheckStopTextScrolling(CurrentLevel.songRect, CurrentLevel.songText, 109f, 0.1f, -109f);
		}
	}

	public void CheckTextScrolling(RectTransform textRect, Text text, string textData, float maxWidth)
	{
		if (!(text.preferredWidth <= maxWidth))
		{
			text.text = "          ";
			textData = textData.RemoveNewLines();
			text.text = textData + "          ";
			float preferredWidth = text.preferredWidth;
			text.text += textData;
			float endValue = textRect.anchoredPosition.x - preferredWidth;
			float duration = (float)text.text.Length * 0.1f;
			textRect.DOAnchorPosX(endValue, duration).SetUpdate(isIndependentUpdate: true).SetEase(Ease.Linear)
				.SetDelay(1f)
				.SetLoops(-1, LoopType.Restart);
		}
	}

	public void CheckStopTextScrolling(RectTransform textRect, Text text, float maxWidth, float duration, float defaultPos)
	{
		if (text.preferredWidth > maxWidth)
		{
			textRect.DOKill();
			textRect.DOAnchorPosX(defaultPos, duration).SetEase(Ease.InExpo).SetUpdate(isIndependentUpdate: true);
		}
	}

	public AudioSource CLSPlaySound(string sound = "sndMenuPop", float volume = 1f, string groupPath = "CustomLevelSelect", float pitch = 1f, float pan = 0f)
	{
		return scrConductor.PlayImmediately(sound, volume, RDUtils.GetMixerGroup(groupPath), pitch, pan, isBeatSound: false, ignoreListenerPause: true);
	}

	public void NarrateWardName(bool fullContext = false)
	{
		if (fullContext)
		{
			Narration.Set(RDString.Get("customLevelSelect.ward"), ElementType.VerticalMenu, wardOptions.Count);
		}
		string speech = "";
		switch (CurrentWardOption.name)
		{
		case WardOptionName.Library:
			speech = RDString.Get("customLevelSelect.ward.library");
			break;
		case WardOptionName.OpenSteamWorkshop:
			speech = RDString.Get("customLevelSelect.ward.workshop");
			break;
		case WardOptionName.ImportLevels:
			speech = RDString.Get("customLevelSelect.ward.import");
			break;
		case WardOptionName.Exit:
			speech = RDString.Get("customLevelSelect.ward.exit");
			break;
		}
		Narration.Say(speech, NarrationCategory.Navigation, CurrentWardOptionIndex + 1, wardOptions.Count, ElementType.None, null, RDString.Get(wardOptions[CurrentWardOptionIndex].detailDescriptionToken), false, NarrationActionName.ToSelect, NarrationActionName.ToNavigateVertical, NarrationActionName.ToImport, NarrationActionName.ToGoBack);
	}

	public void NarrateCurrentLevel()
	{
		RDLevelSettings settings = levelDetail.CurrentLevelData.settings;
		string newValue = settings.song.RemoveRichTextColorTags();
		string newValue2 = settings.artist.RemoveRichTextColorTags();
		string newValue3 = settings.author.RemoveRichTextColorTags();
		string text;
		if ((int)CurrentLevel.CurrentRank == -3)
		{
			text = RDString.Get("customLevelSelect.unopenedLevel");
			text = text.Replace("[song]", newValue);
			text = text.Replace("[artist]", newValue2);
			text = text.Replace("[pressToOpen]", levelNeverSelectedDetailText.text);
			Narration.Say(text, NarrationCategory.Navigation, CurrentLevelIndex + 1, levelsData.Count, ElementType.None, null, "", false, NarrationActionName.ToUnwrap, NarrationActionName.ToNavigateVertical, NarrationActionName.ToGoBack);
			return;
		}
		if (!levelDetail.CurrentLevelData.isLegacyLevel && CurrentLevelPendingData == null && FindCurrentLevelErrors().Count > 0)
		{
			text = RDString.Get("customLevelSelect.errorLevel");
			text = text.Replace("[song]", newValue);
			text = text.Replace("[artist]", newValue2);
			text = text.Replace("[error]", RDString.Get("customLevelSelect.errorLoadingLevel") + " " + RDString.Get("customLevelSelect.checkDetails"));
			Narration.Say(text, NarrationCategory.Navigation, CurrentLevelIndex + 1, levelsData.Count, ElementType.None, null, "", false, NarrationActionName.ToSelect, NarrationActionName.ToNavigateVertical, NarrationActionName.ToSearchCustomLevels, NarrationActionName.ToGoBack);
			return;
		}
		text = RDString.Get("customLevelSelect.unwrappedLevel");
		text = text.Replace("[song]", newValue);
		text = text.Replace("[artist]", newValue2);
		text = text.Replace("[difficulty]", RDString.Get("enum.LevelDifficulty." + settings.difficulty));
		Rank rank = Persistence.GetCustomLevelRank(levelDetail.CurrentLevelData.Hash, levelDetail.GetCurrentLevelSpeed());
		if ((int)rank == -1)
		{
			text = text.Replace("[rank]", RDString.Get("customLevelSelect.unplayed"));
		}
		else
		{
			text = text.Replace("[rank]", RDString.Get("customLevelSelect.bestRank"));
			text = text.Replace("[rank]", RDString.Get("narration.rank." + rank.ToString()));
		}
		text = text.Replace("[author]", newValue3);
		text = text.Replace("[playerModes]", RDString.Get("enum.LevelPlayMode." + settings.canBePlayedOn));
		Narration.Say(text, NarrationCategory.Navigation, CurrentLevelIndex + 1, levelsData.Count, ElementType.None, null, "", false, NarrationActionName.ToSelect, NarrationActionName.ToNavigateVertical, NarrationActionName.ToSearchCustomLevels, NarrationActionName.ToGoBack);
	}
}
