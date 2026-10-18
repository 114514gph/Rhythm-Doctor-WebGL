using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityFileDialog;

namespace RDLevelEditor
{
	public class scnEditor : RDEditorBase
	{
		private class LevelState
		{
			public string data;

			public List<int> selectedIds = new List<int>();

			public Tab tab;

			public int rowsTabIndex;

			public int rowSelected;

			public int decorationsTabIndex;

			public string decorationSelected;

			public float timelinePosX;

			public float timelinePosY;

			public LevelState(string data, List<int> selectedIds, Tab tab, int rowsTabIndex, int rowSelected, int decorationsTabIndex, string decorationSelected, float timelinePosX, float timelinePosY)
			{
				this.data = data;
				this.selectedIds = selectedIds;
				this.tab = tab;
				this.rowsTabIndex = rowsTabIndex;
				this.rowSelected = rowSelected;
				this.decorationsTabIndex = decorationsTabIndex;
				this.decorationSelected = decorationSelected;
				this.timelinePosX = timelinePosX;
				this.timelinePosY = timelinePosY;
			}
		}

		private const int NoneSelected = -1;

		private static scnEditor _instance;

		public static scnGame gameInstance;

		[Header("Editor")]
		public bool debug;

		[NonSerialized]
		public bool proMode;

		[Header("Canvas")]
		public Canvas canvas;

		public RectTransform canvasRectTransform;

		public CanvasScaler canvasScaler;

		public EventSystem eventSystem;

		public Material PreviewMaterial;

		public RawImage gameView;

		public RectTransform panel;

		public RectTransform movementPanel;

		public RDDialog dialog;

		public GameObject snowParticles;

		public RDInk inkEditor;

		public TutorialMask mask;

		public GameObject switchLanguageSection;

		public Dropdown languages;

		public AudioListener audioListener;

		public Transform audioSourcesContainer;

		public Button popupBlocker;

		public GameObject copyrightPopup;

		public RectTransform copyrightContent;

		public Text copyrightAgreeText;

		public Text copyrightReturnText;

		public TMP_Text copyrightText;

		public GameObject hamburgerAsterisk;

		[Header("Crosshair")]
		public RectTransform crosshair;

		public Text crosshairLabel;

		public Text crosshairStatus;

		public EightSidedOutline crosshairOutline;

		public GameObject roomPerspectivePreview;

		public Camera roomPerspCam;

		public LineRenderer roomPerspLine;

		[NonSerialized]
		public RDEditorBase pickerUsingCrosshair;

		[Header("Playback")]
		public Button rewindButton;

		public Button playPauseButton;

		public Button backwardButton;

		public Button forwardButton;

		public Sprite playSprite;

		public Sprite pauseSprite;

		public RectTransform gameArea;

		public Text gameStatus;

		public Button gamePreviewScaleButton;

		public Text gamePreviewScaleText;

		public int startBar;

		public bool scrubbing;

		public Image autoplay;

		public Image metronome;

		public Image mute;

		public Image metronomeXmas;

		public Image twoPlayer;

		[Header("Otto")]
		public Sprite autoplayOffSprite;

		public Sprite autoplayOnSprite;

		public Sprite autoplayP1PressedSprite;

		public Sprite autoplayP2PressedSprite;

		public Sprite autoplayP1P2PressedSprite;

		public Sprite autoplayHeartSprite;

		public Sprite twoPlayerOffSprite;

		public Sprite twoPlayerOnSprite;

		public Image toolsSleeping;

		[Header("Timeline")]
		public Timeline timelineScript;

		[Header("Menu")]
		public GameObject mainMenu;

		public Text mostRecentFile;

		public Text[] keyShortcuts;

		public PauseMenu settingsMenu;

		[Header("Open Recent")]
		public GameObject recentFilesMenu;

		public RectTransform recentFilesArea;

		public GameObject openRecentOption;

		public GameObject olderFileBase;

		private GameObject[] olderFileGOs = new GameObject[8];

		private Text[] olderFiles = new Text[8];

		public GameObject languageSwitcher;

		[Header("URL")]
		public Button urlMenuButton;

		public GameObject insertUrlContainer;

		public InputField urlInput;

		public Text downloadButtonLabel;

		public Button downloadButton;

		public Button cancelButton;

		private RectTransform insertUrlRect;

		private bool _openingLevel;

		private const int DefaultScaleFactor = -1;

		public int scaleFactor;

		private IEnumerator openUrlCoroutine;

		[Header("Inspector")]
		public RectTransform leftPanel;

		public RectTransform bottomPanel;

		public RectTransform rightPanel;

		public RDInspectorPanelManager inspectorPanelManager;

		public Image inspectorTopPanel;

		public Button menuButton;

		public Text saveButtonText;

		public bool unsavedIndicatorCheck;

		public Text inspectorTitle;

		public Button deleteEventButton;

		public RDRoomsSelectionPopup roomsSelectionPopup;

		public RDColorPickerPopup colorPickerPopup;

		public RDCharacterPickerPopup characterPickerPopup;

		public RDLetterMovementPickerPopup letterMovementPickerPopup;

		public RDPublishPopup publishPopup;

		public Sprite emojiSprite;

		public Sprite doctahSprite;

		[Header("Game View")]
		public Button fullscreenButton;

		public GameObject cameraButton;

		public GameObject cycleWindowDanceButton;

		public GameObject muteButton;

		public GameObject windowDanceRatioButton;

		public Image windowDanceButtonImage;

		public Text currentRatioText;

		public Sprite currentScreenWindowDance;

		public Sprite simulateWindowDance;

		public Sprite noneWindowDance;

		public Image muteButtonImage;

		public Sprite muteOnSprite;

		public Sprite muteOffSprite;

		public Sprite rowViewSprite;

		public Sprite gridViewSprite;

		public InspectorPanel_LevelSettings levelSettingsPanel;

		public Text tagPreview;

		public Text eventTooltip;

		public EventRoomsTooltip eventRoomsTooltip;

		public bool tagFieldMode;

		public SoundSettingsPopup soundSettingsPopup;

		public Conditionals conditionalsPanel;

		public ConditionalsPreview conditionalsPreview;

		public List<Conditional> conditionals = new List<Conditional>();

		public List<Conditional> globalConditionals = new List<Conditional>();

		[Header("Tabs")]
		public List<TabSection> tabSections;

		[NonSerialized]
		public Tab currentTab;

		public GameObject windowTabButton;

		[NonSerialized]
		public List<LevelEventControl_Base> eventControls = new List<LevelEventControl_Base>();

		[NonSerialized]
		public List<LevelEventControl_Base> eventControls_sounds = new List<LevelEventControl_Base>();

		[NonSerialized]
		public List<LevelEventControl_Base> eventControls_rooms = new List<LevelEventControl_Base>();

		[NonSerialized]
		public List<LevelEventControl_Base> eventControls_windows = new List<LevelEventControl_Base>();

		[NonSerialized]
		public List<List<LevelEventControl_Base>> eventControls_rows = new List<List<LevelEventControl_Base>>();

		[NonSerialized]
		public List<LevelEventControl_Base> eventControls_actions = new List<LevelEventControl_Base>();

		[NonSerialized]
		public List<List<LevelEventControl_Base>> eventControls_decorations = new List<List<LevelEventControl_Base>>();

		[NonSerialized]
		public List<LevelEvent_MakeRow> rowsData = new List<LevelEvent_MakeRow>();

		[NonSerialized]
		public List<LevelEvent_MakeDecorationBase> decorationsData = new List<LevelEvent_MakeDecorationBase>();

		[NonSerialized]
		public string[] colorPalette = RDEditorConstants.defaultColorPalette;

		[NonSerialized]
		public Dictionary<string, AudioClip> audioClipCache = new Dictionary<string, AudioClip>();

		[NonSerialized]
		public int selectedRowIndex = -1;

		[NonSerialized]
		public string selectedDecoration;

		[NonSerialized]
		public string rdlevelText = "";

		[NonSerialized]
		public List<LevelEventControl_Base> selectedControls;

		[NonSerialized]
		public List<LevelEvent_Base> clipboard;

		[NonSerialized]
		public string openedFilePath;

		[NonSerialized]
		public Dictionary<string, CustomAnimationData> customCharacterData = new Dictionary<string, CustomAnimationData>();

		[NonSerialized]
		public bool muteActive;

		[NonSerialized]
		public bool metronomeActive;

		[NonSerialized]
		public bool xmasSeason;

		[NonSerialized]
		public float ottoHeartTime;

		[NonSerialized]
		public float ottoPetTime;

		private bool mouseIsOverOtto;

		private bool ottoBeingPet;

		private int maxScaleFactor;

		[NonSerialized]
		public bool reloadingGame;

		public RectTransform tabPanels;

		public Image tabPanelsBackground;

		public CanvasGroup tabPanelsCanvasGroup;

		private bool tabPanelsHidden;

		private float timeSinceLastBackup;

		private const float timeBetweenBackups = 300f;

		[NonSerialized]
		public RDLevelSettings levelSettings;

		public Transform rankSettingsPanel;

		public Transform calibrationPanel;

		private const int MaxUndoSteps = 100;

		[NonSerialized]
		public bool savingState;

		[NonSerialized]
		public int changingState;

		[NonSerialized]
		public bool changingFile;

		public int navigationalStateIndex;

		private List<LevelState> undoStates = new List<LevelState>();

		private List<LevelState> redoStates = new List<LevelState>();

		private bool populatingLanguagesDropdown;

		[NonSerialized]
		public LevelEventControl_Base hoveredLevelEventControl;

		[NonSerialized]
		public LevelEventControl_Base hoveredLevelEventControlDisplayingTooltip;

		private Vector3 lastMousePosition = Vector3.zero;

		private bool tutorialMode;

		private int tutorialStep;

		private Color metronomeOnColor;

		private Color metronomeOffColor;

		private const float FadeTransitionDuration = 1f;

		public static bool cameFromCustomLevelSelect;

		public static string customLevelPath;

		public Image fadeTransitionImage;

		private bool didNotSaveCustomLevelOnce;

		private bool canReceiveInput;

		private bool initialized;

		public GameObject languageDropdown;

		[Header("Help Menu")]
		public Toggle undoEnabledToggle;

		public bool undoEnabled;

		public Toggle sfxEnabledToggle;

		public bool sfxEnabled;

		public Toggle alwaysShowTooltipsToggle;

		public bool alwaysShowTooltips;

		public Toggle commentPlaybackEnabledToggle;

		[NonSerialized]
		public bool commentPlaybackEnabled;

		public Transform localizationSection;

		public Button fetchLocButton;

		public Button fetchInkButton;

		public InputField localizationPasswordInput;

		public Text ianSoft;

		public Image advancedOttoIcon;

		private bool skipPreviewReloadUpdate;

		[NonSerialized]
		public float gamePreviewScale;

		private float maxGamePreviewScale = 1f;

		private float preferredPreviewScale = 1f;

		[Header("Other")]
		public EditorWebServices webServices;

		private WindowMovement currentWindowMovement;

		private bool openRecentShortcutOn;

		private bool openRecentSaysShow;

		[NonSerialized]
		public bool coloredWindows;

		public GameObject[] spoilers;

		private LevelPlayMode levelPlayMode;

		private bool editorZoomButtonWasClickedThisFrame;

		private bool levelFinishedLoadingCache;

		[NonSerialized]
		public bool playAsSoonItReloads;

		private Tween menuButtonFlash;

		private float lastClickTime;

		private bool areToolsSleeping;

		[NonSerialized]
		public Texture2D textureForCrosshair;

		private const string LevelsFolder = "EditorLevels";

		private static List<string> _mostRecentFilePaths;

		private string _lastSavedData;

		private const int InvalidRowIndex = -10;

		private AsyncOperation scnGameLoad;

		private AsyncOperation scnGameUnload;

		public float fullscreenAnimationDuration = 0.3f;

		public Ease fullscreenAnimationEaseMove = Ease.OutQuad;

		public Ease fullscreenAnimationEaseScale = Ease.OutQuad;

		public bool fullscreen;

		private bool loadGameSceneAfterUnloading;

		private float speedToPlayAfterLoading = 1f;

		[NonSerialized]
		public bool leftPanelIsExpanded;

		private bool openedMainMenuThisFrame;

		private bool hoveringOverRecentFiles;

		public Text snapValueUI;

		public InputField snapValueInput;

		private int denominatorIndex = 3;

		private int[] denominatorValues = new int[9] { 1, 2, 3, 4, 6, 8, 12, 16, 100 };

		public bool filterMode;

		[NonSerialized]
		public SelectLevelEventPanel actionEventPanel;

		private bool forceQuit;

		private readonly float[] ratios = new float[4] { 1.7777778f, 2.4444444f, 1.3333334f, 1.6f };

		private readonly string[] ratioText = new string[4] { "16:9", "22:9", "4:3", "16:10" };

		private int currentRatioIndex;

		public GameObject popupBackground;

		private bool theresChangesToSave;

		private Tween metronomeFlashTween;

		private int ottoClicks;

		private bool eyedropperActive
		{
			get
			{
				if (pickerUsingCrosshair != null)
				{
					return pickerUsingCrosshair is ColorPicker;
				}
				return false;
			}
		}

		public List<Conditional> allConditionals => globalConditionals.Concat(conditionals).ToList();

		public TabSection currentTabSection => tabSections[(int)currentTab];

		public Vector2 uiOffset
		{
			get
			{
				float num = bottomPanel.sizeDelta.y - 72f;
				return new Vector2(172f, 95f + num);
			}
		}

		public static scnEditor instance => _instance;

		public bool isPlaying
		{
			get
			{
				if (base.game != null && base.game.started && !base.game.paused)
				{
					return base.game.levelFinishedLoading;
				}
				return false;
			}
		}

		public LevelEventControl_Base selectedControl
		{
			get
			{
				if (selectedControls.Count != 1)
				{
					return null;
				}
				return selectedControls[0];
			}
		}

		public int selectedRowsTabPageIndex => base.tabSection_rows.pageIndex;

		public int selectedDecorationsTabPageIndex => base.tabSection_decorations.pageIndex;

		public int lastUsedRow
		{
			get
			{
				return selectedRowIndex;
			}
			set
			{
				if (selectedRowIndex != value)
				{
					selectedRowIndex = value;
					base.editor.tabSection_rows.UpdateUIInternal(justUpdateBackgroundColor: true);
				}
			}
		}

		public string lastUsedDecoration
		{
			get
			{
				return selectedDecoration;
			}
			set
			{
				if (selectedDecoration != value)
				{
					selectedDecoration = value;
					base.editor.tabSection_decorations.UpdateUIInternal(justUpdateBackgroundColor: true);
				}
			}
		}

		public Vector3 mousePosition => Input.mousePosition / scaleFactor;

		public bool userIsEditingAnInputField
		{
			get
			{
				if (eventSystem.currentSelectedGameObject != null)
				{
					return eventSystem.currentSelectedGameObject.GetComponent<InputField>() != null;
				}
				return false;
			}
		}

		public List<LevelEvent_MakeRow> currentPageRowsData
		{
			get
			{
				List<LevelEvent_MakeRow> list = new List<LevelEvent_MakeRow>();
				foreach (LevelEvent_MakeRow rowsDatum in rowsData)
				{
					if (rowsDatum.room == selectedRowsTabPageIndex)
					{
						list.Add(rowsDatum);
					}
				}
				return list;
			}
		}

		public List<LevelEvent_MakeDecorationBase> currentPageDecorationsData
		{
			get
			{
				List<LevelEvent_MakeDecorationBase> list = new List<LevelEvent_MakeDecorationBase>();
				foreach (LevelEvent_MakeDecorationBase decorationsDatum in decorationsData)
				{
					if (decorationsDatum.room == selectedDecorationsTabPageIndex)
					{
						list.Add(decorationsDatum);
					}
				}
				return list;
			}
		}

		private bool openingLevel
		{
			get
			{
				return _openingLevel;
			}
			set
			{
				_openingLevel = value;
				cancelButton.onClick.RemoveAllListeners();
				if (_openingLevel)
				{
					cancelButton.onClick.AddListener(() =>
					{
						CancelOpeningUrl();
					});
				}
				else
				{
					cancelButton.onClick.AddListener(() =>
					{
						ToggleInsertFromUrlPopup(show: false);
					});
				}
			}
		}

		private float LevelSpeed
		{
			get
			{
				float result = 1f;
				if (Input.GetKey(KeyCode.LeftShift))
				{
					result = 1.5f;
				}
				if (Input.GetKey(KeyCode.LeftControl))
				{
					result = 0.75f;
				}
				return result;
			}
		}

		private bool LevelUsesWindowDance
		{
			get
			{
				foreach (LevelEventControl_Base eventControl in eventControls)
				{
					if (eventControl.levelEvent.usesWindowDance)
					{
						return true;
					}
				}
				return false;
			}
		}

		private static List<string> mostRecentFilePaths
		{
			get
			{
				if (_mostRecentFilePaths == null)
				{
					_mostRecentFilePaths = new List<string>();
					for (int i = 0; i < 9; i++)
					{
						string lastUsedFilePath = GetLastUsedFilePath(i);
						if (!string.IsNullOrEmpty(lastUsedFilePath) && RDFile.Exists(lastUsedFilePath))
						{
							_mostRecentFilePaths.Add(lastUsedFilePath);
						}
					}
				}
				return _mostRecentFilePaths;
			}
		}

		private string lastSavedData
		{
			get
			{
				return _lastSavedData;
			}
			set
			{
				_lastSavedData = value;
				UpdateUnsavedIndicator(showUnsaved: false);
			}
		}

		private static string DefaultLevelPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Rhythm Doctor", "template.rdlevel");

		public int denominator => denominatorValues[denominatorIndex];

		public float currentWindowDanceRatio => ratios[currentRatioIndex];

		private bool previewNeedsReload
		{
			set
			{
				Color color = (value ? RDEditorConstants.NeedsReloadColor : RDEditorConstants.NormalColor);
				rewindButton.GetComponent<Button>().targetGraphic.color = color;
				backwardButton.GetComponent<Button>().targetGraphic.color = color;
				forwardButton.GetComponent<Button>().targetGraphic.color = color;
			}
		}

		private void Awake()
		{
			_instance = this;
			scnGame.levelToLoadSource = LevelSource.ExternalPath;
			fullscreenButton.interactable = true;
			fadeTransitionImage.color = new Color(0f, 0f, 0f, 1f);
			if (Application.productName == "Rhythm Doctor Editor" || Application.isEditor)
			{
				List<SystemLanguage> availableLanguages = RDString.AvailableLanguages;
				languages.ClearOptions();
				List<string> list = new List<string>();
				foreach (SystemLanguage item in availableLanguages)
				{
					list.Add(RDString.GetNativeLanguageName(item));
				}
				populatingLanguagesDropdown = true;
				languages.AddOptions(list);
				languages.value = availableLanguages.IndexOf(RDString.language);
				populatingLanguagesDropdown = false;
			}
			else
			{
				languageSwitcher.SetActive(value: false);
			}
			int num = 0;
			foreach (TabSection tabSection in tabSections)
			{
				tabSection.Setup();
				Color color = base.gc.colorPalette[num];
				tabSection.tabRT.GetComponent<Image>().color = color;
				tabSection.tabPanel.GetComponent<Image>().color = color;
				num++;
			}
			scaleFactor = Persistence.GetLevelEditorScaleFactor();
			SetCanvasScaleFactor();
			preferredPreviewScale = (gamePreviewScale = Persistence.GetLevelEditorGamePreviewScale());
			base.timeline.rowCellCount = Persistence.GetLevelEditorTimelineHeight();
			base.timeline.UpdateUI();
			fetchInkButton.onClick.AddListener(FetchInkLocalization);
			fetchLocButton.onClick.AddListener(FetchLocalization);
			settingsMenu.Initialize();
			PauseMenu pauseMenu = settingsMenu;
			pauseMenu.toggleVisibility = (PauseMenu.ToggleVisibility)Delegate.Combine(pauseMenu.toggleVisibility, (PauseMenu.ToggleVisibility)((bool show) =>
			{
				if (show)
				{
					dialog.ShowOverlay();
				}
				else
				{
					dialog.HideOverlay();
				}
			}));
			TryHideWindowTab();
		}

		private void SetCanvasScaleFactor()
		{
			int width = Screen.width;
			int height = Screen.height;
			int a = Mathf.FloorToInt((float)width / 352f);
			int b = Mathf.FloorToInt((float)height / 198f);
			maxScaleFactor = Mathf.Max(1, Mathf.Min(a, b));
			if (scaleFactor == -1)
			{
				scaleFactor = maxScaleFactor;
			}
			ValidateScaleFactor();
			canvasScaler.scaleFactor = scaleFactor;
		}

		public void ValidateScaleFactor()
		{
			scaleFactor = Mathf.Clamp(scaleFactor, 2, maxScaleFactor);
		}

		public void EditorSizePlus()
		{
			StartCoroutine(UpdateEditorSizeCo(plus: true));
		}

		public void EditorSizeMinus()
		{
			StartCoroutine(UpdateEditorSizeCo(plus: false));
		}

		private IEnumerator UpdateEditorSizeCo(bool plus)
		{
			string sound = (plus ? "sndButtonClickOn" : "sndButtonClick");
			LevelEditorPlaySound(sound, "LevelEditorInspectorPanel");
			editorZoomButtonWasClickedThisFrame = true;
			scaleFactor += (plus ? 1 : (-1));
			ValidateScaleFactor();
			Persistence.SetLevelEditorScaleFactor(scaleFactor);
			yield return null;
			base.timeline.UpdateUI(updateControls: false);
		}

		private void Start()
		{
			Application.wantsToQuit += ShowQuitDialog;
			RDRichPresence rDRichPresence = RDRichPresence.Instance;
			if (rDRichPresence != null)
			{
				rDRichPresence.cachedSong = "";
				rDRichPresence.UpdatePresence();
			}
			UpdateSnapLabel();
			selectedControls = new List<LevelEventControl_Base>();
			clipboard = new List<LevelEvent_Base>();
			selectedRowIndex = -10;
			base.timeline.Setup();
			InitiateRowHeaders();
			ShowTabSection(Tab.Rows, animateTab: false);
			currentTabSection.ShowNothingSelectedPanel();
			levelSettings = new RDLevelSettings(68);
			roomsSelectionPopup.gameObject.SetActive(value: false);
			crosshair.gameObject.SetActive(value: false);
			crosshairStatus.gameObject.SetActive(value: false);
			roomPerspectivePreview.SetActive(value: false);
			cameraButton.SetActive(value: false);
			muteButton.SetActive(value: false);
			cycleWindowDanceButton.SetActive(value: false);
			currentWindowMovement = Persistence.GetLevelEditorWindowMovement();
			if (currentWindowMovement == WindowMovement.OneScreen || currentWindowMovement == WindowMovement.Full)
			{
				currentWindowMovement = Persistence.GetWindowMovement();
			}
			windowDanceButtonImage.sprite = ((currentWindowMovement == WindowMovement.Simulate) ? simulateWindowDance : ((currentWindowMovement == WindowMovement.Off) ? noneWindowDance : currentScreenWindowDance));
			int month = DateTime.Now.Month;
			int day = DateTime.Now.Day;
			xmasSeason = month == 12 && day >= 21;
			if (xmasSeason)
			{
				metronome.transform.parent.gameObject.SetActive(value: false);
				metronomeXmas.transform.parent.gameObject.SetActive(value: true);
				metronome = metronomeXmas;
				metronomeOnColor = base.gc.metronomeXmas_onColor;
				metronomeOffColor = base.gc.metronomeXmas_offColor;
			}
			else
			{
				metronomeOnColor = base.gc.metronome_onColor;
				metronomeOffColor = base.gc.metronome_offColor;
			}
			RDUtils.SetMixerVolume("MetronomeClicksVolume", 0f);
			keyShortcuts[0].text = RDEditorUtils.KeyComboToString(control: true, shift: false, KeyCode.N);
			keyShortcuts[1].text = RDEditorUtils.KeyComboToString(control: true, shift: false, KeyCode.O);
			keyShortcuts[2].text = RDEditorUtils.KeyComboToString(control: true, shift: true, KeyCode.O);
			keyShortcuts[3].text = RDEditorUtils.KeyComboToString(control: true, shift: false, KeyCode.S);
			keyShortcuts[4].text = RDEditorUtils.KeyComboToString(control: true, shift: true, KeyCode.S);
			keyShortcuts[5].text = RDEditorUtils.KeyComboToString(control: true, shift: false, KeyCode.Minus);
			keyShortcuts[6].text = RDEditorUtils.KeyComboToString(control: true, shift: false, KeyCode.Plus);
			keyShortcuts[7].text = RDEditorUtils.KeyComboToString(control: true, shift: false, KeyCode.U);
			keyShortcuts[8].text = RDEditorUtils.KeyComboToString(control: true, shift: true, KeyCode.P);
			keyShortcuts[9].text = RDEditorUtils.KeyComboToString(control: false, alt: true, shift: false, KeyCode.S);
			keyShortcuts[10].text = RDEditorUtils.KeyComboToString(control: false, alt: true, shift: false, KeyCode.Q);
			keyShortcuts[11].text = RDEditorUtils.KeyComboToString(control: true, alt: false, shift: true, KeyCode.R);
			lastClickTime = Time.unscaledTime;
			RDStringToUIText.Apply(mostRecentFile);
			for (int i = 0; i < 8; i++)
			{
				GameObject gameObject;
				if (i == 0)
				{
					gameObject = olderFileBase;
				}
				else
				{
					gameObject = UnityEngine.Object.Instantiate(olderFileBase, olderFileBase.transform.parent);
					gameObject.name += i;
				}
				olderFileGOs[i] = gameObject;
				Text componentInChildren = gameObject.GetComponentInChildren<Text>();
				olderFiles[i] = componentInChildren;
				RDStringToUIText.Apply(componentInChildren);
				int fileIndex = i + 1;
				gameObject.GetComponent<Button>().onClick.AddListener(() =>
				{
					OpenMostRecentFile(fileIndex);
				});
			}
			if (tutorialMode)
			{
				inkEditor.overlay.SetActive(value: true);
				DOVirtual.DelayedCall(0.5f, () =>
				{
					inkEditor.Run("Scene0", () =>
					{
						mask.set(4, 248, 670, 112);
					});
				});
			}
			fadeTransitionImage.DOKill();
			fadeTransitionImage.DOFade(0f, 1f).SetEase(Ease.OutQuint).SetUpdate(isIndependentUpdate: true)
				.SetId("noKillOnReload")
				.OnComplete(() =>
				{
					if (!string.IsNullOrEmpty(customLevelPath))
					{
						OpenFileWithPath(customLevelPath);
						customLevelPath = string.Empty;
					}
					else
					{
						ExtensionAssociation.CheckExtensionAssociation();
						string text = ExtensionAssociation.CheckForFileExtension(".rdlevel");
						if (!string.IsNullOrEmpty(text))
						{
							OpenFileWithPath(text);
						}
						if (!string.IsNullOrEmpty(ExtensionAssociation.CheckForFileExtension(".rdzip")))
						{
							Debug.Log("Do something with the rdzipfile");
							Debug.Log(SceneManager.GetActiveScene().name);
						}
					}
				});
			if (string.IsNullOrEmpty(openedFilePath) && !cameFromCustomLevelSelect)
			{
				LoadDefaultLevel();
			}
			openingLevel = false;
			canReceiveInput = true;
			initialized = true;
			CheckAdvancedFeatures();
			undoEnabled = Persistence.GetUndoEnabled();
			undoEnabledToggle.isOn = undoEnabled;
			undoEnabledToggle.onValueChanged.AddListener((bool enabled) =>
			{
				undoEnabled = enabled;
				Persistence.SetUndoEnabled(undoEnabled);
				LevelEditorPlaySound("sndButtonRadio");
			});
			sfxEnabled = Persistence.GetLevelEditorVolume() > 0;
			sfxEnabledToggle.isOn = sfxEnabled;
			sfxEnabledToggle.onValueChanged.AddListener((bool enabled) =>
			{
				sfxEnabled = enabled;
				Persistence.SetLevelEditorVolume(sfxEnabled ? 10 : 0);
				scrGameManager.LoadSavedVolumes();
				LevelEditorPlaySound("sndButtonRadio");
			});
			commentPlaybackEnabled = Persistence.GetCommentPlaybackEnabled();
			commentPlaybackEnabledToggle.isOn = commentPlaybackEnabled;
			commentPlaybackEnabledToggle.onValueChanged.AddListener((bool enabled) =>
			{
				commentPlaybackEnabled = enabled;
				Persistence.SetCommentPlaybackEnabled(enabled);
				LevelEditorPlaySound("sndButtonRadio");
			});
			alwaysShowTooltips = Persistence.GetLevelEditorAlwaysShowTooltips();
			alwaysShowTooltipsToggle.isOn = alwaysShowTooltips;
			alwaysShowTooltipsToggle.onValueChanged.AddListener((bool enabled) =>
			{
				alwaysShowTooltips = enabled;
				Persistence.SetLevelEditorAlwaysShowTooltips(enabled);
				LevelEditorPlaySound("sndButtonRadio");
			});
			localizationPasswordInput.text = Persistence.GetLocalizationPassword();
			localizationPasswordInput.onEndEdit.AddListener(Persistence.SetLocalizationPassword);
			snapValueInput.onEndEdit.AddListener(UpdateFreetimeSnapValue);
			lastSavedData = EncodeData();
			webServices.LoadAllArtists();
			copyrightPopup.SetActive(!Persistence.GetAcceptedAgreement());
			copyrightAgreeText.text = RDString.Get("editor.agreementAgree");
			copyrightAgreeText.font = RDString.GetAppropiateFontForString(copyrightAgreeText.text);
			copyrightReturnText.text = RDString.Get("editor.agreementReturn");
			copyrightReturnText.font = RDString.GetAppropiateFontForString(copyrightReturnText.text);
		}

		public void AcceptCopyrightAgreement()
		{
			Persistence.SetAcceptedAgreement(accepted: true);
			copyrightPopup.SetActive(value: false);
		}

		public void ShowShortcut(int index)
		{
			keyShortcuts[index]?.gameObject.SetActive(value: true);
			if (index != 2)
			{
				return;
			}
			openRecentShortcutOn = true;
			if (mostRecentFilePaths.Any())
			{
				for (int i = 0; i < mostRecentFilePaths.Count; i++)
				{
					SetOpenRecentText(mostRecentFilePaths[i], i);
				}
			}
		}

		public void HideShortcut(int index)
		{
			keyShortcuts[index]?.gameObject.SetActive(value: false);
			if (index != 2)
			{
				return;
			}
			openRecentShortcutOn = false;
			if (mostRecentFilePaths.Any())
			{
				for (int i = 0; i < mostRecentFilePaths.Count; i++)
				{
					SetOpenRecentText(mostRecentFilePaths[i], i);
				}
			}
		}

		public void TutorialAdvance()
		{
			inkEditor.overlay.SetActive(value: true);
			tutorialStep++;
			DOVirtual.DelayedCall(0.5f, () =>
			{
				switch (tutorialStep)
				{
				case 1:
					inkEditor.Run("Scene1", () =>
					{
						mask.set(185, 280, 4, 4, round: false);
					});
					break;
				case 2:
					inkEditor.Run("Scene2", () =>
					{
						Persistence.SetPlayTutorial(state: false);
					});
					break;
				}
			});
		}

		public void ScrubToBar(int bar, bool playAfterScrubbing = true)
		{
			speedToPlayAfterLoading = LevelSpeed;
			RDBaseDllDummy.printem($"<color=green>To bar {bar}</color> with speed " + speedToPlayAfterLoading);
			base.timeline.playhead.anchoredPosition = base.timeline.playhead.anchoredPosition.WithX(base.timeline.GetPosXFromBarAndBeat(new BarAndBeat(bar, 1f)));
			scrubbing = true;
			startBar = bar;
			if (base.timeline.followPlayhead)
			{
				base.timeline.UpdateWhereToGo();
				base.timeline.UpdateContentX();
			}
			ReloadGameScene(playAfterScrubbing);
			base.editor.tabSection_rows.UpdateUI();
		}

		private void FlashMenuButton()
		{
			string normalColor = "545454FF";
			string hex = "34B4BAFF";
			if (menuButtonFlash != null)
			{
				menuButtonFlash.Kill(complete: true);
			}
			ColorBlock colors = menuButton.colors;
			colors.normalColor = hex.HexToColor();
			menuButton.colors = colors;
			menuButtonFlash = DOVirtual.DelayedCall(0.05f, null).OnComplete(() =>
			{
				ColorBlock colors2 = menuButton.colors;
				colors2.normalColor = normalColor.HexToColor();
				menuButton.colors = colors2;
			});
		}

		private void UpdateUnsavedIndicator(bool showUnsaved)
		{
			string text = RDString.Get("editor.save");
			saveButtonText.text = text + (showUnsaved ? "<color=red>*</color>" : "");
			hamburgerAsterisk.SetActive(showUnsaved);
		}

		private void ShowToolsSleeping(bool sleeping)
		{
			areToolsSleeping = sleeping;
			if (sleeping)
			{
				autoplay.gameObject.SetActive(value: false);
				twoPlayer.gameObject.SetActive(value: false);
				toolsSleeping.gameObject.SetActive(value: true);
			}
			else
			{
				autoplay.gameObject.SetActive(value: true);
				twoPlayer.gameObject.SetActive(value: true);
				toolsSleeping.gameObject.SetActive(value: false);
			}
		}

		private void Update()
		{
			if (copyrightPopup.activeInHierarchy)
			{
				RectTransform component = copyrightContent.GetChild(0).GetComponent<RectTransform>();
				copyrightContent.SizeDeltaY(component.sizeDelta.y + 10f);
			}
			if (settingsMenu.showing || !canReceiveInput)
			{
				return;
			}
			if (areToolsSleeping && (RDInput.anyKeyDown || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2)))
			{
				lastClickTime = Time.unscaledTime;
				toolsSleeping.sprite = base.gc.toolsSleepingFrames[2];
				areToolsSleeping = false;
			}
			float num = Time.unscaledTime - lastClickTime;
			if (num > 1f)
			{
				ShowToolsSleeping(sleeping: false);
				if (isPlaying)
				{
					lastClickTime = Time.unscaledTime;
				}
			}
			if (num > base.gc.timeForToolsToSleep && !isPlaying)
			{
				ShowToolsSleeping(sleeping: true);
			}
			if (areToolsSleeping)
			{
				int num2 = ((Time.unscaledTime % 3.5f > 1.75f) ? 1 : 0);
				toolsSleeping.sprite = base.gc.toolsSleepingFrames[num2];
			}
			gameInstance = scnGame.instance;
			bool flag = mousePosition != lastMousePosition;
			lastMousePosition = mousePosition;
			if (base.game != null)
			{
				if (base.game.gameState == GameState.PreStart && !base.game.paused && !base.game.levelFinishedLoading)
				{
					playPauseButton.interactable = false;
				}
				if (!levelFinishedLoadingCache && base.game.levelFinishedLoading)
				{
					if (playAsSoonItReloads)
					{
						StartPlaying(speedToPlayAfterLoading);
					}
					playPauseButton.interactable = true;
				}
				levelFinishedLoadingCache = base.game.levelFinishedLoading;
			}
			UpdateUI_PlayPauseButton();
			if (Input.GetKeyDown(KeyCode.Tab))
			{
				ottoClicks = 0;
				GameObject currentSelectedGameObject = eventSystem.currentSelectedGameObject;
				if (currentSelectedGameObject != null)
				{
					bool num3 = !Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift);
					Selectable component2 = currentSelectedGameObject.GetComponent<Selectable>();
					Selectable selectable = (num3 ? component2.FindSelectableOnDown() : component2.FindSelectableOnUp());
					if (selectable != null)
					{
						InputField component3 = selectable.GetComponent<InputField>();
						if (component3 != null)
						{
							component3.OnPointerClick(new PointerEventData(eventSystem));
						}
						eventSystem.SetSelectedGameObject(selectable.gameObject, new BaseEventData(eventSystem));
					}
				}
			}
			if (RDEditorUtils.CheckForKeyCombo(control: true, shift: false, KeyCode.N))
			{
				FlashMenuButton();
				NewFile();
			}
			else if (RDEditorUtils.CheckForKeyCombo(control: true, shift: true, KeyCode.O))
			{
				FlashMenuButton();
				OpenMostRecentFile(forceOpenInEditor: true);
			}
			else if (RDEditorUtils.CheckForKeyCombo(control: true, shift: false, KeyCode.O))
			{
				FlashMenuButton();
				OpenFile();
			}
			else if (RDEditorUtils.CheckForKeyCombo(control: true, shift: false, KeyCode.U))
			{
				FlashMenuButton();
				OpenWithURL();
			}
			else if (RDEditorUtils.CheckForKeyCombo(control: true, shift: true, KeyCode.R))
			{
				RunAsStandaloneCustomLevel();
			}
			else if (RDEditorUtils.CheckForKeyCombo(control: true, shift: false, KeyCode.S))
			{
				FlashMenuButton();
				SaveFile();
			}
			else if (RDEditorUtils.CheckForKeyCombo(control: true, shift: true, KeyCode.S))
			{
				FlashMenuButton();
				SaveFileAs();
			}
			else if (RDEditorUtils.CheckForKeyCombo(control: true, shift: false, KeyCode.Z, xorMode: true))
			{
				Undo();
			}
			else if (RDEditorUtils.CheckForKeyCombo(control: true, shift: true, KeyCode.Z))
			{
				Redo();
			}
			else if (RDEditorUtils.CheckForKeyCombo(control: true, shift: false, KeyCode.Y))
			{
				Redo();
			}
			else if (RDEditorUtils.CheckForKeyCombo(control: true, shift: false, KeyCode.J))
			{
				NavigationalUndo();
			}
			else if (Input.GetMouseButtonDown(3))
			{
				NavigationalUndo();
			}
			else if (RDEditorUtils.CheckForKeyCombo(control: true, shift: false, KeyCode.K))
			{
				NavigationalRedo();
			}
			else if (Input.GetMouseButtonDown(4))
			{
				NavigationalRedo();
			}
			else if (RDEditorUtils.CheckForKeyCombo(control: true, shift: true, KeyCode.P))
			{
				FlashMenuButton();
				ShowPublishUI();
			}
			else if (RDEditorUtils.CheckForKeyCombo(control: false, alt: true, shift: false, KeyCode.S))
			{
				ShowSettings();
			}
			else if (RDEditorUtils.CheckForKeyCombo(control: false, alt: true, shift: false, KeyCode.Q))
			{
				FlashMenuButton();
				Quit(quittingFromWantsToQuit: false);
			}
			else if (RDEditorUtils.CheckForKeyCombo(control: true, shift: true, KeyCode.End))
			{
				SelectAllSubsequentEvents(onlyInTab: false);
			}
			else if (RDEditorUtils.CheckForKeyCombo(control: false, shift: true, KeyCode.End))
			{
				SelectAllSubsequentEvents(onlyInTab: true);
			}
			else if (RDEditorUtils.CheckForKeyCombo(control: true, shift: true, KeyCode.Home))
			{
				SelectAllPrecedingEvents(onlyInTab: false);
			}
			else if (RDEditorUtils.CheckForKeyCombo(control: false, shift: true, KeyCode.Home))
			{
				SelectAllPrecedingEvents(onlyInTab: true);
			}
			else if (RDEditorUtils.CheckForKeyCombo(control: true, shift: false, KeyCode.Equals) || RDEditorUtils.CheckForKeyCombo(control: true, shift: false, KeyCode.KeypadPlus))
			{
				EditorSizePlus();
			}
			else if (RDEditorUtils.CheckForKeyCombo(control: true, shift: false, KeyCode.Minus) || RDEditorUtils.CheckForKeyCombo(control: true, shift: false, KeyCode.KeypadMinus))
			{
				EditorSizeMinus();
			}
			if (base.game == null)
			{
				scnBase.CheckShowPersistentDataPath();
			}
			if (Application.isEditor && RDEditorUtils.CheckForKeyCombo(control: true, shift: true, KeyCode.E))
			{
				List<string> optionalFilesName = new List<string>();
				List<string> rdlevelFiles = new List<string>();
				RDPublishPopup.GetFilesForLevel(base.editor.openedFilePath, ref optionalFilesName, ref rdlevelFiles);
			}
			if (Application.isEditor && RDEditorUtils.CheckForKeyCombo(control: true, shift: true, KeyCode.A))
			{
				int index = 0;
				int index2 = 1;
				float num4 = 8f;
				foreach (LevelEventControl_Base eventControls_sound in eventControls_sounds)
				{
					if (eventControls_sound.levelEvent is LevelEvent_SetCrotchetsPerBar)
					{
						num4 = (eventControls_sound.levelEvent as LevelEvent_SetCrotchetsPerBar).crotchetsPerBar;
					}
				}
				foreach (LevelEventControl_Base item in eventControls_rows[index])
				{
					int num5 = -1;
					float num6 = float.MaxValue;
					foreach (LevelEventControl_Base item2 in eventControls_rows[index2])
					{
						float num7 = item2.rt.anchoredPosition.x + item2.rt.sizeDelta.x;
						if (num7 > item.rt.anchoredPosition.x && num7 < num6)
						{
							num6 = num7;
							num5 = eventControls_rows[index2].IndexOf(item2);
						}
					}
					if (num5 >= 0)
					{
						LevelEvent_AddOneshotBeat levelEvent_AddOneshotBeat = item.levelEvent as LevelEvent_AddOneshotBeat;
						LevelEvent_AddOneshotBeat levelEvent_AddOneshotBeat2 = eventControls_rows[index2][num5].levelEvent as LevelEvent_AddOneshotBeat;
						levelEvent_AddOneshotBeat.tick = (float)levelEvent_AddOneshotBeat2.bar * num4 + levelEvent_AddOneshotBeat2.beat + levelEvent_AddOneshotBeat2.tick - ((float)levelEvent_AddOneshotBeat.bar * num4 + levelEvent_AddOneshotBeat.beat);
						levelEvent_AddOneshotBeat.Validate();
						(levelEvent_AddOneshotBeat.inspectorPanel as InspectorPanel_AddOneshotBeat).UpdateUI(levelEvent_AddOneshotBeat);
						(item as LevelEventControl_AddClassicBeat).UpdateUI();
					}
				}
			}
			bool flag2 = selectedControl != null && selectedControl.isBase;
			bool flag3 = false;
			GameObject[] eventListPanels = inspectorPanelManager.eventListPanels;
			foreach (GameObject gameObject in eventListPanels)
			{
				if (gameObject != null && gameObject.activeSelf)
				{
					flag3 = true;
					break;
				}
			}
			if (!userIsEditingAnInputField)
			{
				if (!flag2 && !flag3)
				{
					if (RDEditorUtils.CheckForKeyCombo(control: true, shift: false, KeyCode.X))
					{
						Cut();
					}
					if (RDEditorUtils.CheckForKeyCombo(control: true, shift: false, KeyCode.C))
					{
						Copy(copyEffect: true, isCut: false, sound: true);
					}
					if (RDEditorUtils.CheckForKeyCombo(control: true, shift: false, KeyCode.D))
					{
						Clone();
					}
					if (RDEditorUtils.CheckForKeyCombo(control: true, shift: false, KeyCode.V, xorMode: true))
					{
						Paste(onNextBar: false);
					}
					if (RDEditorUtils.CheckForKeyCombo(control: true, shift: false, KeyCode.B) || RDEditorUtils.CheckForKeyCombo(control: true, shift: true, KeyCode.V))
					{
						Paste(onNextBar: true);
					}
					if (RDEditorUtils.CheckForKeyCombo(control: true, shift: true, KeyCode.T))
					{
						SyncLyricsToBeats();
					}
					if (RDEditorUtils.CheckForKeyCombo(control: false, shift: true, KeyCode.Alpha0) && selectedControls.Count >= 1)
					{
						tagFieldMode = !tagFieldMode;
						if (selectedControls.Count == 1)
						{
							selectedControl.levelEvent.inspectorPanel.position.UpdateUI();
						}
						else
						{
							base.ipm.ShowBulkPanel();
						}
					}
					if (RDEditorUtils.CheckForKeyCombo(control: false, shift: true, KeyCode.F1) && !fullscreen)
					{
						copyrightPopup.SetActive(value: true);
					}
					if (!RDEditorUtils.ModifierKeyIsPressed())
					{
						if (RDInput.CheckForStateInKey(KeyCode.A, RDButtonState.WentDown))
						{
							AutoplayButtonClick();
						}
						else if (RDInput.CheckForStateInKey(KeyCode.M, RDButtonState.WentDown))
						{
							MetronomeButtonClick();
						}
						else if (RDInput.CheckForStateInKey(KeyCode.F, RDButtonState.WentDown))
						{
							FullscreenButtonClick();
						}
						else if (Input.GetKeyDown(KeyCode.J))
						{
							base.timeline.PreviousPage();
						}
						else if (Input.GetKeyDown(KeyCode.K))
						{
							base.timeline.NextPage();
						}
						else if (Input.GetKeyDown(KeyCode.G))
						{
							base.timeline.PreviousBookmark();
						}
						else if (Input.GetKeyDown(KeyCode.H))
						{
							base.timeline.NextBookmark();
						}
						else if (RDInput.CheckForStateInKey(KeyCode.P, RDButtonState.WentDown))
						{
							PlayPauseButtonClick();
						}
						else if (RDInput.CheckForStateInKey(KeyCode.Home, RDButtonState.WentDown))
						{
							RewindButtonClick();
						}
						else if (RDInput.CheckForStateInKey(KeyCode.PageUp, RDButtonState.WentDown))
						{
							PreviousButtonClick();
						}
						else if (RDInput.CheckForStateInKey(KeyCode.PageDown, RDButtonState.WentDown))
						{
							NextButtonClick();
						}
					}
					languageSwitcher.gameObject.SetActive(Input.GetKey(KeyCode.Tab));
					if (languageSwitcher.activeSelf)
					{
						advancedOttoIcon.color = ianSoft.color;
					}
				}
				int resolutionScale = Persistence.GetResolutionScale();
				scrVfxControl.CheckKeyComboForResolutionChange();
				int resolutionScale2 = Persistence.GetResolutionScale();
				if (resolutionScale != resolutionScale2)
				{
					base.timeline.UpdateUI();
				}
				if (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Delete))
				{
					DeleteAllSelectedEventControls(sound: true);
				}
				if (Input.GetKeyDown(KeyCode.Escape))
				{
					if (fullscreen)
					{
						FullscreenButtonClick();
					}
					else
					{
						DeselectEverything();
					}
				}
				if (RDInput.leftPress)
				{
					LevelEventControl_Base controlToTheLeft = GetControlToTheLeft(selectedControl);
					if (controlToTheLeft != null)
					{
						SelectEventControl(controlToTheLeft, sound: true);
						base.timeline.UnfollowPlayhead();
						base.timeline.CenterOnPosition((controlToTheLeft.rt.anchoredPosition.x + controlToTheLeft.rightPosition) / 2f, 0.3f);
						base.timeline.CenterOnVertPosition((0f - (controlToTheLeft.rt.anchoredPosition.y + controlToTheLeft.bottomPosition)) / 2f, 0.3f);
					}
				}
				else if (RDInput.rightPress)
				{
					LevelEventControl_Base controlToTheRight = GetControlToTheRight(selectedControl);
					if (controlToTheRight != null)
					{
						SelectEventControl(controlToTheRight, sound: true);
						base.timeline.UnfollowPlayhead();
						base.timeline.CenterOnPosition((controlToTheRight.rt.anchoredPosition.x + controlToTheRight.rightPosition) / 2f, 0.3f);
						base.timeline.CenterOnVertPosition((0f - (controlToTheRight.rt.anchoredPosition.y + controlToTheRight.bottomPosition)) / 2f, 0.3f);
					}
				}
				if (mainMenu.activeSelf && mostRecentFilePaths.Any())
				{
					if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
					{
						openRecentSaysShow = true;
						for (int j = 0; j < mostRecentFilePaths.Count; j++)
						{
							SetOpenRecentText(mostRecentFilePaths[j], j);
						}
					}
					else if (Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.RightControl))
					{
						openRecentSaysShow = false;
						for (int k = 0; k < mostRecentFilePaths.Count; k++)
						{
							SetOpenRecentText(mostRecentFilePaths[k], k);
						}
					}
				}
				GameObject[] eventListPanels2 = inspectorPanelManager.eventListPanels;
				bool flag4 = false;
				eventListPanels = eventListPanels2;
				foreach (GameObject gameObject2 in eventListPanels)
				{
					if (gameObject2 != null && gameObject2.activeSelf)
					{
						flag4 = true;
						break;
					}
				}
				inspectorPanelManager.gridViewLabel.gameObject.SetActive(flag4);
				if (!flag4)
				{
					if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
					{
						TabSection tabSection = ((currentTab == Tab.Decorations) ? ((TabSection)base.tabSection_decorations) : ((TabSection)base.tabSection_rows));
						if (Input.GetKeyDown(KeyCode.Alpha1))
						{
							tabSection.ChangePage(0, calledByUnity: false);
						}
						else if (Input.GetKeyDown(KeyCode.Alpha2))
						{
							tabSection.ChangePage(1, calledByUnity: false);
						}
						else if (Input.GetKeyDown(KeyCode.Alpha3))
						{
							tabSection.ChangePage(2, calledByUnity: false);
						}
						else if (Input.GetKeyDown(KeyCode.Alpha4))
						{
							tabSection.ChangePage(3, calledByUnity: false);
						}
					}
					else if (Input.GetKeyDown(KeyCode.Alpha1))
					{
						ShowTabSection_Song();
					}
					else if (Input.GetKeyDown(KeyCode.Alpha2))
					{
						ShowTabSection_Rows();
					}
					else if (Input.GetKeyDown(KeyCode.Alpha3))
					{
						ShowTabSection_Actions();
					}
					else if (Input.GetKeyDown(KeyCode.Alpha4))
					{
						ShowTabSection_Rooms();
					}
					else if (Input.GetKeyDown(KeyCode.Alpha5))
					{
						ShowTabSection_Decorations();
					}
					else if (Input.GetKeyDown(KeyCode.Alpha6) && windowTabButton.activeSelf)
					{
						ShowTabSection_Windows();
					}
				}
				if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
				{
					base.timeline.scrollRect.horizontal = false;
					base.timeline.verticalScrollRect.vertical = false;
					if (RDEditorUtils.GetScrollWheel() != 0f)
					{
						ScrollMouseActions();
					}
				}
				else
				{
					base.timeline.scrollRect.horizontal = true;
					base.timeline.verticalScrollRect.vertical = true;
				}
			}
			if (((scnGameUnload != null && scnGameUnload.isDone) || (scnGameUnload == null && loadGameSceneAfterUnloading)) && SceneManager.sceneCount == 1)
			{
				scnGameUnload = null;
				if (loadGameSceneAfterUnloading)
				{
					loadGameSceneAfterUnloading = false;
					LoadGameScene();
				}
			}
			if (scnGameLoad != null && scnGameLoad.isDone)
			{
				UpdateUI_PlayPauseButton();
				scnGameLoad = null;
			}
			bool active = selectedControls.Count > 0;
			deleteEventButton.gameObject.SetActive(active);
			autoplay.color = (RDEditorBase.debugSettings.Auto ? base.gc.autoplay_onColor : base.gc.autoplay_offColor);
			if (mouseIsOverOtto && RDEditorBase.debugSettings.Auto && !isPlaying)
			{
				if (flag && !ottoBeingPet)
				{
					ottoPetTime += Time.deltaTime;
				}
				if (ottoPetTime >= 1.5f)
				{
					ottoBeingPet = true;
					ottoPetTime = 0f;
					LevelEditorPlaySound("sndOttoHappy", "LevelEditorOtto");
				}
			}
			else
			{
				ottoPetTime = 0f;
				ottoBeingPet = false;
			}
			if ((!isPlaying && ottoHeartTime > 0f) || ottoBeingPet)
			{
				ottoHeartTime -= Time.unscaledDeltaTime;
				autoplay.sprite = autoplayHeartSprite;
			}
			else if (!RDEditorBase.debugSettings.Auto)
			{
				autoplay.sprite = autoplayOffSprite;
			}
			else if (!userIsEditingAnInputField)
			{
				bool flag5 = RDInput.p1IsPressed || (RDInput.p2IsPressed && !GC.twoPlayerMode);
				bool flag6 = RDInput.emuStates[1].GetKey(RDInput.PlayerEmuKey.IsDown) || (GC.twoPlayerMode && RDInput.p2IsPressed);
				if ((flag5 & flag6) || (!GC.twoPlayerMode & flag5))
				{
					autoplay.sprite = autoplayP1P2PressedSprite;
				}
				else if (flag5)
				{
					autoplay.sprite = autoplayP1PressedSprite;
				}
				else if (flag6)
				{
					autoplay.sprite = autoplayP2PressedSprite;
				}
				else
				{
					autoplay.sprite = autoplayOnSprite;
				}
			}
			twoPlayer.color = (GC.twoPlayerMode ? base.gc.twoPlayer_onColor : base.gc.twoPlayer_offColor);
			twoPlayer.sprite = (GC.twoPlayerMode ? twoPlayerOnSprite : twoPlayerOffSprite);
			float y = bottomPanel.sizeDelta.y + 4f;
			rightPanel.offsetMin = rightPanel.offsetMin.WithY(y);
			if (!leftPanelIsExpanded)
			{
				leftPanel.offsetMin = leftPanel.offsetMin.WithY(y);
			}
			UpdateGamePreviewScale();
			if (base.game != null)
			{
				RDUtils.SetMixerVolume("LevelEditorMuteWhilePlayingVolume", isPlaying ? 0f : 1f);
			}
			if (timeSinceLastBackup != 0f && Time.realtimeSinceStartup - timeSinceLastBackup > 300f)
			{
				timeSinceLastBackup = Time.realtimeSinceStartup;
				BackupSave();
			}
		}

		private void LateUpdate()
		{
			float value = (muteActive ? 0f : ((float)scrGameManager.volume * 0.1f));
			RDUtils.SetMixerVolume("MasterVolume", value);
			muteButtonImage.sprite = (muteActive ? muteOnSprite : muteOffSprite);
			if (mainMenu.activeSelf && !openedMainMenuThisFrame && !hoveringOverRecentFiles && !editorZoomButtonWasClickedThisFrame && (Input.GetMouseButtonUp(0) || Input.GetKeyDown(KeyCode.Escape)))
			{
				HideMenuDropdown();
			}
			openedMainMenuThisFrame = false;
			if (pickerUsingCrosshair != null)
			{
				UpdateCrosshair();
			}
			eventSystem.enabled = !fullscreen;
			bool flag = false;
			if (base.game != null && !fullscreen)
			{
				Vector3 point = gameArea.InverseTransformPoint(Input.mousePosition);
				flag = gameArea.rect.Contains(point);
			}
			if (recentFilesMenu.activeSelf)
			{
				Vector3 point2 = recentFilesArea.InverseTransformPoint(Input.mousePosition);
				if (!hoveringOverRecentFiles && !recentFilesArea.rect.Contains(point2))
				{
					recentFilesMenu.SetActive(value: false);
				}
			}
			cameraButton.SetActive(flag);
			bool active = flag && LevelUsesWindowDance;
			cycleWindowDanceButton.SetActive(active);
			windowDanceRatioButton.SetActive(active);
			muteButton.SetActive(flag);
			SetCanvasScaleFactor();
			editorZoomButtonWasClickedThisFrame = false;
			if (unsavedIndicatorCheck)
			{
				UpdateUnsavedIndicator(!EncodeData().Equals(lastSavedData));
				unsavedIndicatorCheck = false;
			}
			bool isDev = RDBase.isDev;
			localizationSection.gameObject.SetActive(isDev);
			UpdateEventControlTooltip();
			UpdateEventControlTooltipPosition();
		}

		private void UpdateCrosshair()
		{
			RectTransform rectTransform = gameView.rectTransform;
			Vector2 vector = new Vector2(rectTransform.rect.width, rectTransform.rect.height);
			Vector2 vector2 = Input.mousePosition.xy();
			Vector2 sizeDelta = canvasRectTransform.sizeDelta;
			new Int2(Screen.width, Screen.height);
			float num = (float)Screen.height / canvasRectTransform.sizeDelta.y;
			Vector2 vector3 = vector2 / num;
			_ = rectTransform.anchoredPosition;
			Vector2 vector4 = uiOffset + (sizeDelta - uiOffset - vector) / 2f;
			Vector2 vector5 = (vector3 - vector4) / vector;
			bool flag = vector5.x >= 0f && vector5.x <= 1f && vector5.y >= 0f && vector5.y <= 1f;
			Vector2 vector6 = vector5 * 100f;
			crosshair.gameObject.SetActive(flag);
			crosshairStatus.gameObject.SetActive(value: true);
			Vector2 vector7 = vector5 * new Vector2(352f, 198f);
			string text = "000000";
			PositionPicker positionPicker = pickerUsingCrosshair as PositionPicker;
			ColorPicker colorPicker = pickerUsingCrosshair as ColorPicker;
			bool flag2 = positionPicker != null;
			bool flag3 = colorPicker != null;
			if (flag)
			{
				crosshair.anchoredPosition = vector5 * vector;
				Color effectColor = Color.black;
				if (flag2)
				{
					crosshairLabel.text = string.Format("{2:F1}{0},{1}{3:F1}{0}%{1}\n {4}{0},{1}{5}{0}px{1}", "<color=#ffffff99>", "</color>", vector6.x, vector6.y, Mathf.RoundToInt(vector7.x), Mathf.RoundToInt(vector7.y));
				}
				else if (flag3)
				{
					if (textureForCrosshair != null)
					{
						Vector2 vector8 = vector5 * new Vector2(textureForCrosshair.width, textureForCrosshair.height);
						text = textureForCrosshair.GetPixel(Mathf.RoundToInt(vector8.x), Mathf.RoundToInt(vector8.y)).ToHex().Remove(0, 1);
					}
					bool flag4 = RDEditorUtils.ColorIsDark(text.HexToColor());
					if (flag4)
					{
						effectColor = Color.white;
					}
					string text2 = $"<color=#{text}>";
					crosshairLabel.text = string.Format("{0}#{2}{1}{3}{2}", flag4 ? "<color=#00000099>" : "<color=#ffffff99>", text2, "</color>", text);
				}
				crosshairOutline.effectColor = effectColor;
			}
			if (flag2 && positionPicker.perspectiveCorner != -1 && selectedControl != null)
			{
				LevelEvent_SetRoomPerspective levelEvent_SetRoomPerspective = selectedControl.levelEvent as LevelEvent_SetRoomPerspective;
				bool flag5 = true;
				bool[] cornerPositionsUsed = levelEvent_SetRoomPerspective.cornerPositionsUsed;
				for (int i = 0; i < cornerPositionsUsed.Length; i++)
				{
					if (!cornerPositionsUsed[i])
					{
						flag5 = false;
						break;
					}
				}
				if (flag5)
				{
					roomPerspectivePreview.SetActive(value: true);
					Float2[] array = (Float2[])levelEvent_SetRoomPerspective.cornerPositions.Clone();
					if (flag)
					{
						Vector2 vector9 = vector5 * 100f;
						array[positionPicker.perspectiveCorner] = new Float2(vector9.x, vector9.y);
					}
					UpdateRoomPerspectivePreview(array);
				}
				else
				{
					roomPerspectivePreview.SetActive(value: false);
				}
			}
			if (Input.GetMouseButtonDown(0))
			{
				if (flag)
				{
					if (flag2)
					{
						if (positionPicker.usingPercentUnits)
						{
							positionPicker.percent = new Float2(vector6.x, vector6.y);
						}
						else
						{
							positionPicker.pixels = new Float2(vector7.x, vector7.y);
						}
					}
					else if (flag3)
					{
						if (colorPicker.storesAlpha)
						{
							text += "FF";
						}
						if (colorPickerPopup != null)
						{
							colorPickerPopup.color = text;
						}
					}
				}
				CancelCrosshairSelection();
			}
			else if (Input.GetMouseButtonDown(1))
			{
				CancelCrosshairSelection();
			}
			if (fullscreen)
			{
				CancelCrosshairSelection();
			}
		}

		public void CancelCrosshairSelection()
		{
			pickerUsingCrosshair = null;
			crosshair.gameObject.SetActive(value: false);
			crosshairStatus.gameObject.SetActive(value: false);
			roomPerspectivePreview.SetActive(value: false);
		}

		public void UpdateRoomPerspectivePreview(Float2[] corners)
		{
			float num = roomPerspCam.orthographicSize * 2f;
			float num2 = num * roomPerspCam.aspect;
			Vector3[] array = new Vector3[corners.Length];
			for (int i = 0; i < corners.Length; i++)
			{
				Float2 @float = corners[i];
				float x = @float.x / 100f * num2 - num2 / 2f;
				float y = @float.y / 100f * num - num / 2f;
				array[i] = new Vector3(x, y, 0f);
			}
			Vector3[] array2 = new Vector3[array.Length];
			array2[0] = array[0];
			array2[1] = array[2];
			array2[2] = array[3];
			array2[3] = array[1];
			roomPerspLine.SetPositions(array2);
		}

		private void OnGUI()
		{
			if (!debug)
			{
				return;
			}
			Rect position = new Rect(0f, 0f, 400f, 600f);
			int num = 0;
			string text = "";
			string text2 = "\n";
			foreach (List<LevelEventControl_Base> eventControls_row in eventControls_rows)
			{
				text = text + "eventControls_rows[" + num + "].Count = " + eventControls_row.Count + text2;
				foreach (LevelEventControl_Base item in eventControls_row)
				{
					text = text + "> " + item.levelEvent.row + " | " + item.levelEvent.ToString() + text2;
				}
				num++;
			}
			text = "";
			num = 0;
			foreach (LevelEvent_MakeRow rowsDatum in rowsData)
			{
				string text3 = ((selectedRowIndex == num) ? "> " : "");
				text = string.Concat(text, text3 + num.ToString() + " room: " + rowsDatum.room + " | " + rowsDatum.character.ToString() + " rowOrderInItsRoom: " + RowOrderInItsRoom(rowsDatum) + text2);
				num++;
			}
			text = scnGame.levelToLoadSource.ToString();
			GUI.Label(position, text);
		}

		public void ShowTabSection_Song()
		{
			ShowTabSection(Tab.Song, animateTab: true, playSound: true);
		}

		public void ShowTabSection_Rows()
		{
			ShowTabSection(Tab.Rows, animateTab: true, playSound: true);
		}

		public void ShowTabSection_Actions()
		{
			ShowTabSection(Tab.Actions, animateTab: true, playSound: true);
		}

		public void ShowTabSection_Rooms()
		{
			ShowTabSection(Tab.Rooms, animateTab: true, playSound: true);
		}

		public void ShowTabSection_Decorations()
		{
			ShowTabSection(Tab.Decorations, animateTab: true, playSound: true);
		}

		public void ShowTabSection_Windows()
		{
			ShowTabSection(Tab.Windows, animateTab: true, playSound: true);
		}

		public void ShowTabSection(Tab tab, bool animateTab = true, bool playSound = false)
		{
			bool flag = false;
			if ((currentTab == tab) & playSound)
			{
				tabPanelsHidden = !tabPanelsHidden;
				base.timeline.rectTransform.offsetMin = base.timeline.rectTransform.offsetMin.WithX(tabPanelsHidden ? 24f : 92f);
				tabPanels.SizeDeltaX(tabPanelsHidden ? 4f : 72f);
				tabPanelsCanvasGroup.alpha = (tabPanelsHidden ? 0f : 1f);
				tabPanelsCanvasGroup.interactable = !tabPanelsHidden;
				tabPanelsCanvasGroup.blocksRaycasts = !tabPanelsHidden;
				flag = true;
			}
			else
			{
				ClearHoveredEventControl();
				using (new SaveStateScope())
				{
					TabSection tabSection = tabSections[(int)currentTab];
					TabSection tabSection2 = tabSections[(int)tab];
					tabSection.Hide(animateTab);
					tabSection2.Show(animateTab);
					tabPanelsBackground.color = tabSection2.color;
					currentTab = tab;
					base.timeline.UpdateMaxUsedY();
				}
			}
			if (playSound)
			{
				string text = "";
				float pitch = 1f;
				if (flag)
				{
					text = "sndEditorTogglePanelExpand";
					pitch = (tabPanelsHidden ? RDUtils.PitchSemitonesToPercent(-3f) : RDUtils.PitchSemitonesToPercent(2f));
				}
				else
				{
					text = "sndEditorTabClick" + tab;
				}
				LevelEditorPlaySound(text, "LevelEditorTabClicks", 1f, pitch);
			}
			TryHideWindowTab();
		}

		public void TryHideWindowTab()
		{
			windowTabButton.SetActive(currentTab == Tab.Windows || eventControls_windows.Count > 0 || Persistence.GetWindowTabEnabled() || Application.isEditor);
		}

		public void MoveToWindowTab(bool createWindowDance = false)
		{
			if (!Persistence.GetSawWindowTab() && !Persistence.GetWindowTabEnabled())
			{
				ShowFirstTimeWindowWarning();
				return;
			}
			windowTabButton.SetActive(value: true);
			ShowTabSection_Windows();
			if ((bool)selectedControl)
			{
				BarAndBeat barAndBeat = selectedControl.levelEvent.barAndBeat;
				DeleteEventControl(selectedControl, selectControlToTheLeft: false);
				if (createWindowDance)
				{
					LevelEventControl_Base levelEventControl_Base = CreateEventControl(new LevelEvent_NewWindowDance(), Tab.Windows);
					levelEventControl_Base.levelEvent.barAndBeat = barAndBeat;
					levelEventControl_Base.UpdateUI();
					SelectEventControl(levelEventControl_Base);
				}
			}
		}

		public void ShowFirstTimeWindowWarning(bool createWindowDance = false)
		{
			dialog.Show(RDString.Get("editor.WindowTabWarning"), new string[2]
			{
				RDString.Get("editor.ok"),
				RDString.Get("editor.dialog.cancel")
			}, (int x) =>
			{
				base.editor.dialog.Hide();
				if (x != 1)
				{
					Persistence.SetSawWindowTab(accepted: true);
					MoveToWindowTab(createWindowDance);
				}
			});
		}

		public LevelEventControl_Base CreateEventControl(LevelEvent_Base levelEvent, Tab tab, bool skipSaveState = false)
		{
			using (new SaveStateScope(clearRedo: true, skipSaveState, skipTimelinePos: true))
			{
				string text = levelEvent.CustomControlName();
				if (text == null)
				{
					text = tabSections[(int)tab].DefaultControlName();
				}
				LevelEventControl_Base component = (UnityEngine.Object.Instantiate(Resources.Load("LevelEventControl_" + text), Vector3.zero, Quaternion.identity) as GameObject).GetComponent<LevelEventControl_Base>();
				component.levelEvent = levelEvent;
				levelEvent.tab = tab;
				AddNewEventControl(component, tab);
				component.ShowAsDeselected();
				return component;
			}
		}

		public void AddNewEventControl(LevelEventControl_Base eventControl, Tab tab)
		{
			TabSection tabSection = (eventControl.tabSection = tabSections[(int)tab]);
			eventControl.container.Add(eventControl);
			eventControl.UpdateUI();
			eventControls.Add(eventControl);
			Transform containerTransform;
			switch (tab)
			{
			case Tab.Rows:
			{
				int room2 = rowsData[eventControl.levelEvent.row].room;
				containerTransform = tabSection.container[room2].transform;
				break;
			}
			case Tab.Decorations:
			{
				LevelEvent_Base levelEvent = eventControl.levelEvent;
				if (levelEvent.type == LevelEventType.None)
				{
					int room = decorationsData[levelEvent.row].room;
					containerTransform = tabSection.container[room].transform;
					break;
				}
				int num = -1;
				foreach (LevelEvent_MakeDecorationBase decorationsDatum in decorationsData)
				{
					if (decorationsDatum.decorationId == levelEvent.target)
					{
						num = decorationsDatum.room;
						break;
					}
				}
				if (num == -1)
				{
					Debug.LogWarning("Room was not found for level event targeting decoration id: " + levelEvent.target);
					return;
				}
				containerTransform = tabSection.container[num].transform;
				break;
			}
			default:
				containerTransform = tabSection.containerTransform;
				break;
			}
			eventControl.GetComponent<RectTransform>().SetParent(containerTransform, worldPositionStays: false);
		}

		public void SetLevelEventControlType(LevelEventType levelEventType, bool copyRow = false)
		{
			if (filterMode)
			{
				if (RDEditorUtils.ControlIsPressed())
				{
					actionEventPanel.events.ForEach((SelectLevelEventButton selectLevelEventButton2) =>
					{
						selectLevelEventButton2.selected = false;
					});
				}
				SelectLevelEventButton selectLevelEventButton = base.editor.inspectorPanelManager.eventListPanels[(int)currentTab].GetComponent<SelectLevelEventPanel>().events.Find((SelectLevelEventButton selectLevelEventButton2) => selectLevelEventButton2.type == levelEventType);
				selectLevelEventButton.selected = !selectLevelEventButton.selected;
				return;
			}
			LevelEvent_Base levelEvent = selectedControl.levelEvent;
			LevelEvent_Base levelEvent_Base = (LevelEvent_Base)Activator.CreateInstance(Type.GetType("RDLevelEditor.LevelEvent_" + levelEventType));
			levelEvent_Base.CopyBasePropertiesFrom(levelEvent, copyBarAndBeat: true, copyRow);
			levelEvent_Base.OnCreate();
			Tab tab = selectedControl.tab;
			if (tab == Tab.Decorations && levelEvent_Base.type != LevelEventType.None)
			{
				LevelEvent_MakeDecorationBase levelEvent_MakeDecorationBase = base.editor.currentPageDecorationsData[levelEvent_Base.y];
				levelEvent_Base.target = levelEvent_MakeDecorationBase.decorationId;
			}
			DeleteEventControl(selectedControl, selectControlToTheLeft: false);
			LevelEventControl_Base levelEventControl_Base = CreateEventControl(levelEvent_Base, tab);
			levelEventControl_Base.UpdateUI();
			UpdateTimelineAccordingToLevelEventType(levelEventType);
			SelectEventControl(levelEventControl_Base);
		}

		public void SetDecorationType(LevelEventType levelEventType)
		{
			LevelEvent_MakeDecorationBase levelEvent_MakeDecorationBase = (LevelEvent_MakeDecorationBase)Activator.CreateInstance(Type.GetType("RDLevelEditor.LevelEvent_" + levelEventType));
			levelEvent_MakeDecorationBase.rooms = new int[1] { DecorationHeader.decorationTabToCreateEventIn };
			base.editor.DeselectAllEventControls();
			base.editor.AddNewDecoration(levelEvent_MakeDecorationBase);
			ShowTabSection(Tab.Decorations);
			base.tabSection_decorations.ChangePage(DecorationHeader.decorationTabToCreateEventIn, calledByUnity: false);
			base.tabSection_decorations.UpdateUI();
			base.tabSection_decorations.decorationHeaders[base.editor.currentPageDecorationsData.Count - 1].GetComponent<DecorationHeader>().Select();
		}

		public void StartPlaying(float speed = 1f)
		{
			base.game.barToScrubTo = startBar;
			base.conductor.isScrubbing = startBar > 1;
			StartCoroutine(base.game.StartTheGame(speed, RDString.Get("status.starting")));
			scrubbing = false;
		}

		public void UpdateUI_PlayPauseButton()
		{
			if (base.game != null)
			{
				(playPauseButton.GetComponent<Button>().targetGraphic as Image).sprite = (isPlaying ? pauseSprite : playSprite);
				gameStatus.gameObject.SetActive(!base.game.levelFinishedLoading);
			}
		}

		public static string GetLastUsedFilePath(int index = 0)
		{
			string text = ((index == 0) ? "" : index.ToString());
			return PlayerPrefs.GetString("LastFilePathKey" + text, "");
		}

		public static string GetLastUsedFolder()
		{
			string lastUsedFilePath = GetLastUsedFilePath();
			if (!string.IsNullOrEmpty(lastUsedFilePath))
			{
				string directoryName = Path.GetDirectoryName(lastUsedFilePath);
				if (Directory.Exists(directoryName))
				{
					return directoryName;
				}
			}
			return string.Empty;
		}

		public static void SaveLastUsedFile(string path)
		{
			if (mostRecentFilePaths.Contains(path))
			{
				mostRecentFilePaths.Remove(path);
			}
			mostRecentFilePaths.Insert(0, path);
			PlayerPrefs.SetString("LastFilePathKey", path);
			int num = 9;
			if (mostRecentFilePaths.Count > num)
			{
				mostRecentFilePaths.RemoveRange(num, mostRecentFilePaths.Count - num);
			}
			for (int i = 0; i < num; i++)
			{
				string text = ((i == 0) ? "" : i.ToString());
				string value = ((i < mostRecentFilePaths.Count) ? mostRecentFilePaths[i] : "");
				PlayerPrefs.SetString("LastFilePathKey" + text, value);
			}
		}

		public void NewFile()
		{
			PauseIfUnpaused();
			HideMenuDropdown();
			CheckIfNeedsToSaveFile(() =>
			{
				ShowSavePanel(RDString.Get("editor.dialog.newFile"), saveEmptyFile: true);
			});
		}

		public void OpenFile()
		{
			PauseIfUnpaused();
			CheckIfNeedsToSaveFile(() =>
			{
				StartCoroutine(OpenFileCoroutine());
			});
		}

		public IEnumerator OpenFileCoroutine()
		{
			HideMenuDropdown();
			string lastUsedFolder = GetLastUsedFolder();
			List<string> list = new List<string> { "rdlevel", "rdzip", "zip" };
			if (RDBase.isDev)
			{
				list.Add("json");
			}
			string text = FileBrowser.PickFile(lastUsedFolder, RDString.Get("editor.dialog.levelFileFormat"), list.ToArray(), RDString.Get("editor.dialog.openFile"));
			if (!string.IsNullOrEmpty(text))
			{
				string text2 = Path.GetExtension(text).Replace(".", string.Empty).ToLowerInvariant();
				if (text2 == "rdzip" || text2 == "zip")
				{
					StartCoroutine(OpenLevelPackage(Uri.UnescapeDataString(text.Replace("file:", ""))));
				}
				else
				{
					OpenFileWithPath(text);
				}
			}
			yield break;
		}

		public static void PauseIfUnpaused()
		{
			if (gameInstance != null && !gameInstance.paused)
			{
				gameInstance.TogglePauseGame();
			}
		}

		public static void UnpauseIfPaused()
		{
			if (gameInstance != null && gameInstance.paused)
			{
				gameInstance.TogglePauseGame();
			}
		}

		public bool FileIsSaved()
		{
			return EncodeData().Equals(lastSavedData);
		}

		public void CheckIfNeedsToSaveFile(Action succeed, string message = null)
		{
			if (FileIsSaved())
			{
				succeed();
				return;
			}
			string text = RDString.Get("editor.dialog.cancel");
			string text2 = RDString.Get("editor.dialog.discard");
			string text3 = RDString.Get("editor.save");
			dialog.Show(message ?? RDString.Get("editor.dialog.unsavedChanges"), new string[3] { text3, text2, text }, (int option) =>
			{
				dialog.Hide(() =>
				{
					if (option == 0)
					{
						if (didNotSaveCustomLevelOnce)
						{
							CustomLevelSavingPopup(succeed);
						}
						else
						{
							SaveFile();
							succeed();
						}
					}
					else if (option == 1)
					{
						succeed();
					}
				});
			});
		}

		public void OpenFileWithPath(string filePath, bool updateLastFile = true)
		{
			skipPreviewReloadUpdate = true;
			changingState = 0;
			changingFile = true;
			openedFilePath = Uri.UnescapeDataString(filePath.Replace("file:", ""));
			string directoryName = Path.GetDirectoryName(Path.GetDirectoryName(base.editor.openedFilePath));
			didNotSaveCustomLevelOnce = LevelValidation.CustomLevelsPath == directoryName;
			try
			{
				string directoryName2 = Path.GetDirectoryName(base.editor.openedFilePath);
				string text = (lastSavedData = RDFile.ReadAllText(openedFilePath, RDEditorConstants.DefaultLevelEncoding));
				DecodeData(text);
				LevelValidation.ForceSettingsValidation(directoryName2, ref levelSettings);
			}
			catch (Exception ex)
			{
				Debug.LogError("Error opening file " + filePath + ":\n" + ex.Message + "\n" + ex.StackTrace);
				dialog.Show(RDString.Get("editor.openFailed"), new string[1] { RDString.Get("editor.PublishLevel.dialogOK") }, (int option) =>
				{
					base.editor.dialog.Hide();
				});
			}
			if (updateLastFile)
			{
				SaveLastUsedFile(openedFilePath);
			}
			ReloadGameScene(playAsSoonItReloads: false, isNewlyOpenedFile: true);
			timeSinceLastBackup = Time.realtimeSinceStartup;
			changingFile = false;
			CheckIfEditorIsOlderVersion();
			ShowUIAsNewFile();
		}

		private void LoadDefaultLevel()
		{
			skipPreviewReloadUpdate = true;
			changingState = 0;
			changingFile = true;
			if (File.Exists(DefaultLevelPath))
			{
				try
				{
					string directoryName = Path.GetDirectoryName(DefaultLevelPath);
					string text = RDFile.ReadAllText(DefaultLevelPath, RDEditorConstants.DefaultLevelEncoding);
					DecodeData(text, forceLatestVersion: true);
					LevelValidation.ForceSettingsValidation(directoryName, ref levelSettings);
				}
				catch (Exception ex)
				{
					Debug.LogError("Error opening default template file " + DefaultLevelPath + ":\n" + ex.Message + "\n" + ex.StackTrace);
					dialog.Show(RDString.Get("editor.openFailed"), new string[1] { RDString.Get("editor.PublishLevel.dialogOK") }, (int _) =>
					{
						base.editor.dialog.Hide();
					});
				}
				CheckIfEditorIsOlderVersion();
			}
			else
			{
				AddNewRow(new LevelEvent_MakeRow());
				LevelEvent_Base[] array = new LevelEvent_Base[3]
				{
					new LevelEvent_PlaySong
					{
						bar = 1,
						beat = 1f
					},
					new LevelEvent_AddClassicBeat
					{
						bar = 1,
						beat = 1f,
						row = 0
					},
					new LevelEvent_SetTheme
					{
						bar = 1,
						beat = 1f,
						preset = RDTheme.OrientalTechno
					}
				};
				foreach (LevelEvent_Base levelEvent_Base in array)
				{
					levelEvent_Base.OnCreate();
					CreateEventControl(levelEvent_Base, levelEvent_Base.defaultTab).UpdateUI();
				}
			}
			ReloadGameScene(playAsSoonItReloads: false, isNewlyOpenedFile: true);
			ShowUIAsNewFile();
			changingFile = false;
		}

		private void ShowUIAsNewFile()
		{
			ClearStateCache();
			ShowTabSection(Tab.Song);
			startBar = 0;
			tabSections[0].ShowNothingSelectedPanel();
			base.timeline.whereToGo = 0f;
			base.timeline.scrollviewContent.LocalMoveX(0f);
			base.timeline.followPlayhead = true;
			lastSavedData = EncodeData();
			if (actionEventPanel != null)
			{
				actionEventPanel.lastSelectedEventsNumber = -1;
				actionEventPanel.events.ForEach((SelectLevelEventButton levelEvent) =>
				{
					levelEvent.selected = true;
				});
				actionEventPanel.ShowAllEvents();
			}
		}

		public void OpenMostRecentFile(bool forceOpenInEditor = false)
		{
			OpenMostRecentFile(0, forceOpenInEditor);
		}

		public void OpenMostRecentFile(int index, bool forceOpenInEditor = false)
		{
			if (index >= mostRecentFilePaths.Count)
			{
				return;
			}
			string fileToOpen = mostRecentFilePaths[index];
			if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && !forceOpenInEditor)
			{
				RDUtils.RevealInExplorer(fileToOpen);
				return;
			}
			CheckIfNeedsToSaveFile(() =>
			{
				OpenFileWithPath(fileToOpen);
			});
		}

		public void BackupSave()
		{
			if (!string.IsNullOrEmpty(openedFilePath))
			{
				string data = EncodeData();
				RDFile.WriteAllText(Path.Combine(Path.GetDirectoryName(openedFilePath), "backup.rdlevel"), data, RDEditorConstants.DefaultLevelEncoding);
			}
		}

		public void SaveFile_Public()
		{
			SaveFile();
		}

		public bool SaveFile()
		{
			HideMenuDropdown();
			if (didNotSaveCustomLevelOnce)
			{
				CustomLevelSavingPopup();
				return false;
			}
			if (string.IsNullOrEmpty(openedFilePath))
			{
				ShowSavePanel(RDString.Get("editor.dialog.saveFile"), saveEmptyFile: false);
				return true;
			}
			PlaySaveSfx();
			RDFile.WriteAllText(data: lastSavedData = EncodeData(), path: openedFilePath, encoding: RDEditorConstants.DefaultLevelEncoding);
			theresChangesToSave = false;
			return false;
		}

		public void ForceSave(string rdlevelPath, RDLevelData rdLevelData = null)
		{
			RDBaseDllDummy.printes("ForceSave " + rdlevelPath);
			List<LevelEvent_MakeRow> list = base.editor.rowsData;
			List<LevelEvent_MakeDecorationBase> list2 = base.editor.decorationsData;
			if (rdLevelData != null)
			{
				base.editor.rowsData = rdLevelData.rows;
				base.editor.decorationsData = rdLevelData.decorations;
			}
			string data = ((rdLevelData != null) ? rdLevelData.Encode() : (lastSavedData = EncodeData()));
			RDFile.WriteAllText(rdlevelPath, data, RDEditorConstants.DefaultLevelEncoding);
			base.editor.rowsData = list;
			base.editor.decorationsData = list2;
		}

		private void CustomLevelSavingPopup(Action succeed = null)
		{
			dialog.Show(RDString.Get("editor.dialog.saveCustomLevel"), new string[3]
			{
				RDString.Get("editor.dialog.customLevelOverwrite"),
				RDString.Get("editor.dialog.customLevelClone"),
				RDString.Get("editor.dialog.cancel")
			}, (int option) =>
			{
				dialog.Hide(() =>
				{
					if (option != 2)
					{
						didNotSaveCustomLevelOnce = false;
						if (option == 0)
						{
							string data = EncodeData();
							lastSavedData = data;
							RDFile.WriteAllText(openedFilePath, data, RDEditorConstants.DefaultLevelEncoding);
							theresChangesToSave = false;
							if (succeed != null)
							{
								succeed();
							}
						}
						else
						{
							SaveFileAs(succeed, cloning: true);
						}
					}
				});
			});
		}

		public void SaveFileAs_Public()
		{
			SaveFileAs();
		}

		private void PlaySaveSfx()
		{
			if (hamburgerAsterisk.activeSelf)
			{
				LevelEditorPlaySound("sndEditorSave");
			}
			else
			{
				LevelEditorPlaySound("sndEditorSave", "LevelEditorParent", 1f, 2.2f);
			}
		}

		public void SaveFileAs(Action succeed = null, bool cloning = false)
		{
			PauseIfUnpaused();
			HideMenuDropdown();
			string filePath = FileBrowser.SaveFile(GetLastUsedFolder(), "level", RDString.Get("editor.dialog.levelFileFormat"), new string[1] { "rdlevel" }, RDString.Get("editor.dialog.saveFileAs"));
			if (string.IsNullOrEmpty(filePath))
			{
				return;
			}
			string currentDirectoryPath = Path.GetDirectoryName(base.editor.openedFilePath);
			string newDirectoryPath = Path.GetDirectoryName(filePath);
			printe("Saving from " + newDirectoryPath + "  to  " + currentDirectoryPath);
			List<string> optionalFilesName = new List<string>();
			List<string> rdlevelFiles = new List<string>();
			string[] filesForLevel = RDPublishPopup.GetFilesForLevel(base.editor.openedFilePath, ref optionalFilesName, ref rdlevelFiles);
			if (newDirectoryPath != currentDirectoryPath && filesForLevel.Length > 1)
			{
				if (cloning)
				{
					CopyRequiredFilesToNewDirectory(filePath, currentDirectoryPath, newDirectoryPath, succeed);
					return;
				}
				dialog.Show(RDString.Get("editor.dialog.saveAsDifferentDirectory"), new string[3]
				{
					RDString.Get("editor.dialog.saveAsDiffDirectoryYes"),
					RDString.Get("editor.dialog.saveAsDiffDirectoryNo"),
					RDString.Get("editor.dialog.cancel")
				}, (int option) =>
				{
					dialog.Hide(() =>
					{
						if (option == 1)
						{
							string data = EncodeData();
							lastSavedData = data;
							RDFile.WriteAllText(filePath, data, RDEditorConstants.DefaultLevelEncoding);
							openedFilePath = filePath;
							SaveLastUsedFile(openedFilePath);
							if (succeed != null)
							{
								succeed();
							}
						}
						else if (option == 0)
						{
							CopyRequiredFilesToNewDirectory(filePath, currentDirectoryPath, newDirectoryPath, succeed);
						}
					});
				});
			}
			else
			{
				RDFile.WriteAllText(data: lastSavedData = EncodeData(), path: filePath, encoding: RDEditorConstants.DefaultLevelEncoding);
				openedFilePath = filePath;
				SaveLastUsedFile(openedFilePath);
				theresChangesToSave = false;
			}
		}

		private void CopyRequiredFilesToNewDirectory(string mainRDLevelPath, string currentDirectoryPath, string newDirectoryPath, Action succeed)
		{
			string text = currentDirectoryPath;
			char directorySeparatorChar = Path.DirectorySeparatorChar;
			currentDirectoryPath = text + directorySeparatorChar;
			string text2 = newDirectoryPath;
			directorySeparatorChar = Path.DirectorySeparatorChar;
			newDirectoryPath = text2 + directorySeparatorChar;
			List<string> optionalFilesName = new List<string>();
			List<string> rdlevelFiles = new List<string>();
			string[] filesForLevel = RDPublishPopup.GetFilesForLevel(openedFilePath, ref optionalFilesName, ref rdlevelFiles);
			for (int i = 1; i < filesForLevel.Length; i++)
			{
				string sourceFileName = currentDirectoryPath + filesForLevel[i];
				string text3 = newDirectoryPath + filesForLevel[i];
				if (!File.Exists(text3))
				{
					File.Copy(sourceFileName, text3);
				}
			}
			RDBaseDllDummy.printem(mainRDLevelPath);
			string data = EncodeData();
			RDFile.WriteAllText(mainRDLevelPath, data, RDEditorConstants.DefaultLevelEncoding);
			openedFilePath = mainRDLevelPath;
			SaveLastUsedFile(openedFilePath);
			succeed?.Invoke();
		}

		public bool ShowSavePanel(string title, bool saveEmptyFile)
		{
			PauseIfUnpaused();
			string text = FileBrowser.SaveFile(GetLastUsedFolder(), "level", RDString.Get("editor.dialog.levelFileFormat"), new string[1] { "rdlevel" }, title);
			if (string.IsNullOrEmpty(text))
			{
				printe("Didn't save because user canceled when choosing panel");
				return false;
			}
			if (saveEmptyFile)
			{
				changingState = 0;
				skipPreviewReloadUpdate = true;
				changingFile = true;
				DeleteAllData();
				LoadDefaultLevel();
				changingFile = false;
			}
			PlaySaveSfx();
			string data = (lastSavedData = EncodeData());
			RDFile.WriteAllText(text, data, RDEditorConstants.DefaultLevelEncoding);
			openedFilePath = text;
			timeSinceLastBackup = Time.realtimeSinceStartup;
			SaveLastUsedFile(openedFilePath);
			return true;
		}

		private int GetFinishLevelEventNumber()
		{
			int num = 0;
			foreach (LevelEventControl_Base eventControl in eventControls)
			{
				if (eventControl.levelEvent is LevelEvent_FinishLevel)
				{
					num++;
				}
			}
			return num;
		}

		private string EncodeData()
		{
			List<LevelEvent_Base> list = new List<LevelEvent_Base>();
			foreach (LevelEventControl_Base eventControl in eventControls)
			{
				list.Add(eventControl.levelEvent);
			}
			list = list.OrderBy((LevelEvent_Base o) => o.sortOrder).ToList();
			List<BookmarkData> list2 = new List<BookmarkData>();
			foreach (tlBookmark bookmark in base.timeline.bookmarks)
			{
				list2.Add(new BookmarkData(base.timeline.GetBarAndBeatWithPosX(bookmark.rt.anchoredPosition.x), bookmark.currentColorIndex));
			}
			return new RDLevelData(levelSettings, rowsData, list, conditionals, decorationsData, list2, colorPalette).Encode();
		}

		private void DeleteAllData()
		{
			selectedControls.Clear();
			eventControls_sounds.Clear();
			eventControls_rows.Clear();
			rowsData.Clear();
			eventControls_actions.Clear();
			eventControls_rooms.Clear();
			eventControls_windows.Clear();
			eventControls_decorations.Clear();
			decorationsData.Clear();
			eventControls.Clear();
			foreach (TabSection tabSection in tabSections)
			{
				GameObject[] container = tabSection.container;
				for (int i = 0; i < container.Length; i++)
				{
					foreach (Transform item in container[i].transform)
					{
						UnityEngine.Object.Destroy(item.gameObject);
					}
				}
			}
			levelSettings = new RDLevelSettings(68);
			conditionals = new List<Conditional>();
			UpdateUI_AllTabSections();
			UpdateUI_PlayPauseButton();
			base.timeline.ClearBookmarks();
			base.timeline.UpdateUI();
			UnloadGameScene(loadGameSceneAfterwards: false);
			scrVfxControl.FlushCustomLevelData();
			UpdateCustomCharactersCache();
			ClearStateCache();
		}

		private void UpdateCustomCharactersCache()
		{
			HashSet<string> hashSet = new HashSet<string>();
			foreach (LevelEvent_MakeRow rowsDatum in rowsData)
			{
				if (rowsDatum.character == Character.Custom)
				{
					hashSet.Add(rowsDatum.customCharacterName);
				}
			}
			HashSet<string> hashSet2 = new HashSet<string>();
			foreach (KeyValuePair<string, CustomAnimationData> customCharacterDatum in customCharacterData)
			{
				if (!hashSet.Contains(customCharacterDatum.Key))
				{
					hashSet2.Add(customCharacterDatum.Key);
				}
			}
			foreach (string item in hashSet2)
			{
				customCharacterData.Remove(item);
			}
		}

		public void DecodeData(string text, bool forceLatestVersion = false)
		{
			if (changingState == 0)
			{
				DeleteAllData();
			}
			text = LevelBase.FlipScreenFix(text);
			new Dictionary<string, object>();
			RDLevelData rDLevelData;
			try
			{
				rDLevelData = new RDLevelData(Json.Deserialize(text) as Dictionary<string, object>, onlyActiveEvents: false, onlySettings: false, forceLatestVersion);
			}
			catch (Exception ex)
			{
				Debug.Log("Error loading level:\n" + ex.Message + "\n\n" + ex.StackTrace + "\n\n" + text);
				ShowLevelLoadingError(LevelErrorName.LevelDataNull);
				return;
			}
			if (rDLevelData.errorName != LevelErrorName.None)
			{
				RDBaseDllDummy.printem(rDLevelData.errorName);
				ShowLevelLoadingError(rDLevelData.errorName);
				return;
			}
			levelSettings = rDLevelData.settings;
			conditionals = rDLevelData.conditionals;
			colorPalette = rDLevelData.colorPalette;
			bool flag = false;
			if (changingState != 0)
			{
				if (rowsData.Count != rDLevelData.rows.Count)
				{
					flag = true;
				}
				else
				{
					int num = 0;
					foreach (LevelEvent_MakeRow row3 in rDLevelData.rows)
					{
						LevelEvent_MakeRow levelEvent_MakeRow = rowsData[num];
						if (row3.Encode() != levelEvent_MakeRow.Encode())
						{
							flag = true;
							printe("rows data changed, compare rowData:\n" + row3.Encode() + "vs: \n" + levelEvent_MakeRow.Encode());
						}
						num++;
					}
				}
				if (flag)
				{
					rowsData.Clear();
					eventControls_rows.Clear();
				}
			}
			if ((changingState == 0) | flag)
			{
				foreach (LevelEvent_MakeRow row4 in rDLevelData.rows)
				{
					AddNewRow(row4);
				}
				Dictionary<int, int> dictionary = new Dictionary<int, int>();
				int num2 = 0;
				foreach (LevelEvent_MakeRow rowsDatum in rowsData)
				{
					dictionary.Add(rowsDatum.row, num2);
					num2++;
				}
				foreach (LevelEvent_Base levelEvent3 in rDLevelData.levelEvents)
				{
					if (!levelEvent3.isDecorationTabEvent && dictionary.ContainsKey(levelEvent3.row))
					{
						levelEvent3.row = dictionary[levelEvent3.row];
					}
				}
				base.tabSection_rows.UpdateUI();
			}
			bool flag2 = false;
			if (changingState != 0)
			{
				if (decorationsData.Count != rDLevelData.decorations.Count)
				{
					flag2 = true;
				}
				else
				{
					int num3 = 0;
					foreach (LevelEvent_MakeDecorationBase decoration in rDLevelData.decorations)
					{
						LevelEvent_MakeDecorationBase levelEvent_MakeDecorationBase = decorationsData[num3];
						if (decoration.Encode() != levelEvent_MakeDecorationBase.Encode())
						{
							flag2 = true;
							printe("decorations data changed, compare decorationData:\n" + decoration.Encode() + "vs: \n" + levelEvent_MakeDecorationBase.Encode());
						}
						num3++;
					}
				}
				if (flag2)
				{
					decorationsData.Clear();
					eventControls_decorations.Clear();
				}
			}
			if ((changingState == 0) | flag2)
			{
				foreach (LevelEvent_MakeDecorationBase decoration2 in rDLevelData.decorations)
				{
					AddNewDecoration(decoration2);
				}
				base.tabSection_decorations.UpdateUI();
			}
			List<LevelEventControl_Base> list = new List<LevelEventControl_Base>();
			foreach (LevelEventControl_Base eventControl in eventControls)
			{
				list.Add(eventControl);
			}
			foreach (LevelEvent_Base levelEvent4 in rDLevelData.levelEvents)
			{
				bool flag3 = false;
				if (changingState != 0)
				{
					LevelEventControl_Base levelEventControl_Base = null;
					foreach (LevelEventControl_Base item in list)
					{
						if (item.levelEvent.uid == levelEvent4.uid)
						{
							levelEventControl_Base = item;
							if (levelEvent4.Encode() != item.levelEvent.Encode() || (levelEvent4.isDecorationTabEvent && levelEvent4.row != item.levelEvent.row))
							{
								item.levelEvent.CopyFrom(levelEvent4, copyBarAndBeat: true);
								item.UpdateUI();
								LevelEventType type = item.levelEvent.type;
								base.editor.UpdateTimelineAccordingToLevelEventType(type);
							}
							flag3 = true;
							break;
						}
					}
					if (levelEventControl_Base != null)
					{
						list.Remove(levelEventControl_Base);
					}
				}
				if (flag3)
				{
					continue;
				}
				Tab defaultTab = levelEvent4.defaultTab;
				bool flag4 = levelEvent4 is LevelEvent_MakeDecorationBase;
				if ((defaultTab == Tab.None) | flag4)
				{
					if (!(levelEvent4 is LevelEvent_MakeRow) && !flag4)
					{
						printw($"LevelEvent {levelEvent4.type} is not being handled by any TabSection.");
					}
				}
				else
				{
					CreateEventControl(levelEvent4, defaultTab).UpdateUI();
				}
			}
			foreach (LevelEventControl_Base eventControls_action in eventControls_actions)
			{
				LevelEvent_Base levelEvent = eventControls_action.levelEvent;
				if (levelEvent is LevelEvent_AdvanceText)
				{
					levelEvent.barAndBeat = base.timeline.FixBarAndBeat(levelEvent.barAndBeat);
				}
			}
			foreach (List<LevelEventControl_Base> eventControls_row in eventControls_rows)
			{
				foreach (LevelEventControl_Base item2 in eventControls_row)
				{
					LevelEvent_Base levelEvent2 = item2.levelEvent;
					if (levelEvent2 is LevelEvent_AddOneshotBeat)
					{
						levelEvent2.barAndBeat = base.timeline.FixBarAndBeat(levelEvent2.barAndBeat);
					}
				}
			}
			if ((changingState != 0) & flag)
			{
				eventControls_rows.Clear();
				foreach (LevelEvent_MakeRow rowsDatum2 in rowsData)
				{
					_ = rowsDatum2;
					eventControls_rows.Add(new List<LevelEventControl_Base>());
				}
				foreach (LevelEventControl_Base eventControl2 in eventControls)
				{
					if (!list.Contains(eventControl2) && eventControl2.tab == Tab.Rows)
					{
						int row = eventControl2.levelEvent.row;
						eventControls_rows[row].Add(eventControl2);
						eventControl2.UpdateUI();
					}
				}
			}
			if ((changingState != 0) & flag2)
			{
				eventControls_decorations.Clear();
				foreach (LevelEvent_MakeDecorationBase decorationsDatum in decorationsData)
				{
					_ = decorationsDatum;
					eventControls_decorations.Add(new List<LevelEventControl_Base>());
				}
				foreach (LevelEventControl_Base eventControl3 in eventControls)
				{
					if (!list.Contains(eventControl3) && eventControl3.tabSection.tabIndex == 4)
					{
						int row2 = eventControl3.levelEvent.row;
						eventControls_decorations[row2].Add(eventControl3);
						eventControl3.UpdateUI();
					}
				}
			}
			if (changingState != 0)
			{
				DeleteEventControlsFromList(list);
			}
			base.timeline.ClearBookmarks();
			List<BookmarkData> bookmarks = rDLevelData.bookmarks;
			for (int i = 0; i < bookmarks.Count; i++)
			{
				BookmarkData bookmarkData = bookmarks[i];
				base.timeline.AddBookmark(base.timeline.GetPosXFromBarAndBeat(bookmarkData.barAndBeat));
				base.timeline.bookmarks[i].SetColor(bookmarkData.colorIndex);
			}
			if (changingState == 0)
			{
				base.timeline.UpdateUI();
			}
			UpdateUI_AllTabSections();
		}

		public void UpdateUI_AllTabSections()
		{
			foreach (TabSection tabSection in tabSections)
			{
				tabSection.UpdateUI();
			}
		}

		public void ShowLevelLoadingError(LevelErrorName errorName)
		{
			dialog.Show(RDString.Get("settingRequirement." + errorName), new string[1] { RDString.Get("editor.PublishLevel.dialogOK") }, (int option) =>
			{
				base.editor.dialog.Hide();
			});
		}

		public void ShowPublishUI()
		{
			publishPopup.gameObject.SetActive(value: true);
			publishPopup.Show();
		}

		public void CancelOpeningUrl()
		{
			if (openUrlCoroutine != null)
			{
				StopCoroutine(openUrlCoroutine);
			}
			canReceiveInput = true;
			urlInput.interactable = true;
			downloadButton.interactable = true;
			downloadButtonLabel.text = RDString.Get("editor.openURL.download");
			openingLevel = false;
		}

		public void OpenWithURL()
		{
			ToggleInsertFromUrlPopup(show: true);
		}

		private void ToggleInsertFromUrlPopup(bool show)
		{
			if (insertUrlRect == null)
			{
				insertUrlRect = insertUrlContainer.GetComponent<RectTransform>();
			}
			if (show)
			{
				urlInput.text = string.Empty;
				insertUrlContainer.SetActive(value: true);
				popupBackground.SetActive(value: true);
				LevelEditorPlaySound("sndEditorPopupSmall", "LevelEditorInspectorPanel");
			}
			Vector3 localScale = (show ? Vector3.zero : Vector3.one);
			Vector3 endValue = (show ? Vector3.one : Vector3.zero);
			float duration = (show ? base.gc.roomsSelectionPopup_showAnimDuration : base.gc.roomsSelectionPopup_hideAnimDuration);
			Ease ease = (show ? base.gc.roomsSelectionPopup_showAnimEase : base.gc.roomsSelectionPopup_hideAnimEase);
			if (!show && insertUrlContainer.activeSelf)
			{
				LevelEditorPlaySound("sndEditorPopdownSmall", "LevelEditorInspectorPanel");
			}
			insertUrlRect.DOKill(complete: true);
			insertUrlRect.localScale = localScale;
			insertUrlRect.DOScale(endValue, duration).SetEase(ease).SetUpdate(UpdateType.Normal, isIndependentUpdate: true)
				.OnComplete(() =>
				{
					if (!show)
					{
						insertUrlContainer.SetActive(value: false);
						popupBackground.SetActive(value: false);
					}
				});
		}

		public void OpenUrl()
		{
			if (!string.IsNullOrEmpty(urlInput.text))
			{
				canReceiveInput = false;
				downloadButtonLabel.text = RDString.Get("editor.openURL.downloading");
				downloadButton.interactable = false;
				urlInput.interactable = false;
				openingLevel = true;
				openUrlCoroutine = OpenLevelPackage(string.Empty);
				StartCoroutine(openUrlCoroutine);
			}
		}

		public IEnumerator OpenLevelPackage(string packagePath)
		{
			RDBaseDllDummy.printem(packagePath);
			bool downloadedFile = false;
			bool errorsOpeningLevel = false;
			bool downloadedFromGetFileName = false;
			string browserSelectedPath = string.Empty;
			string errorMessage = string.Empty;
			bool openingFromUrl = string.IsNullOrEmpty(packagePath);
			if (openingFromUrl)
			{
				string url = urlInput.text;
				string filename = string.Empty;
				yield return RDPackageInstaller.GetFileNameFromUrl(url, (string _filename, string _downloadedFilePath) =>
				{
					filename = _filename;
					downloadedFromGetFileName = (downloadedFile = !string.IsNullOrEmpty(_downloadedFilePath));
					packagePath = _downloadedFilePath;
				});
				if (!errorsOpeningLevel)
				{
					browserSelectedPath = FileBrowser.SaveFile(GetLastUsedFolder(), filename, RDString.Get("customLevelSelect.import.rdzipFiles"), new string[2] { "rdzip", "zip" }, RDString.Get("editor.openURL.saveFolder"));
					if (string.IsNullOrEmpty(browserSelectedPath))
					{
						yield break;
					}
				}
				if (!errorsOpeningLevel && !downloadedFile)
				{
					yield return RDPackageInstaller.DownloadPackage(url, browserSelectedPath);
					errorsOpeningLevel = RDPackageInstaller.anyErrors;
					downloadedFile = !errorsOpeningLevel;
					if (errorsOpeningLevel)
					{
						errorMessage = RDPackageInstaller.errorMessage;
					}
					else
					{
						packagePath = browserSelectedPath;
					}
				}
			}
			if (!errorsOpeningLevel)
			{
				string newDirectoryName = (string.IsNullOrEmpty(browserSelectedPath) ? string.Empty : Path.GetFileNameWithoutExtension(browserSelectedPath));
				string text = RDPackageInstaller.UnzipAndFindLevel(packagePath, newDirectoryName);
				if (openingFromUrl & downloadedFromGetFileName)
				{
					string directoryName = Path.GetDirectoryName(browserSelectedPath);
					text = Path.Combine(RDPackageInstaller.MoveRDLevelFolder(Path.GetDirectoryName(text), directoryName), Path.GetFileName(text));
				}
				if (!RDPackageInstaller.anyErrors)
				{
					OpenFileWithPath(text);
					ToggleInsertFromUrlPopup(show: false);
				}
				else
				{
					errorsOpeningLevel = true;
					errorMessage = RDPackageInstaller.errorMessage;
					printe("error: " + errorMessage);
				}
			}
			if (errorsOpeningLevel)
			{
				dialog.Show(errorMessage, new string[1] { RDString.Get("editor.PublishLevel.dialogOK") }, (int option) =>
				{
					base.editor.dialog.Hide();
				});
			}
			if (downloadedFile)
			{
				File.SetAttributes(packagePath, FileAttributes.Normal);
				File.Delete(packagePath);
			}
			canReceiveInput = true;
			urlInput.interactable = true;
			downloadButton.interactable = true;
			downloadButtonLabel.text = RDString.Get("editor.openURL.download");
			openingLevel = false;
		}

		public void InitiateRowHeaders()
		{
			UpdateUI_AllTabSections();
		}

		public int LastRowOfRoom(int roomIndex)
		{
			int num = 0;
			int num2 = -10;
			int num3 = -10;
			foreach (LevelEvent_MakeRow rowsDatum in rowsData)
			{
				if (rowsDatum.room == roomIndex)
				{
					num2 = num;
				}
				if (num3 == -10 && rowsDatum.room > roomIndex)
				{
					num3 = num;
				}
				num++;
			}
			if (num2 != -10)
			{
				return num2;
			}
			if (num3 != -10)
			{
				return num3 - 1;
			}
			return -10;
		}

		public int LastDecorationOfRoom(int roomIndex)
		{
			int num = 0;
			int num2 = -10;
			int num3 = -10;
			foreach (LevelEvent_MakeDecorationBase decorationsDatum in decorationsData)
			{
				if (decorationsDatum.room == roomIndex)
				{
					num2 = num;
				}
				if (num3 == -10 && decorationsDatum.room > roomIndex)
				{
					num3 = num;
				}
				num++;
			}
			if (num2 != -10)
			{
				return num2;
			}
			if (num3 != -10)
			{
				return num3 - 1;
			}
			return -10;
		}

		public void AddNewRow(LevelEvent_MakeRow data)
		{
			using (new SaveStateScope())
			{
				int room = data.room;
				int num = LastRowOfRoom(room);
				if (num == -10 || num == rowsData.Count)
				{
					rowsData.Add(data);
					eventControls_rows.Add(new List<LevelEventControl_Base>());
					return;
				}
				int num2 = num + 1;
				rowsData.Insert(num2, data);
				eventControls_rows.Insert(num2, new List<LevelEventControl_Base>());
				foreach (LevelEventControl_Base eventControl in eventControls)
				{
					LevelEvent_Base levelEvent = eventControl.levelEvent;
					if (levelEvent.isDecorationTabEvent)
					{
						continue;
					}
					if (levelEvent.row >= num2)
					{
						levelEvent.row++;
					}
					if (levelEvent is LevelEvent_ChangePlayersRows)
					{
						LevelEvent_ChangePlayersRows obj = levelEvent as LevelEvent_ChangePlayersRows;
						List<RDPlayer> list = new List<RDPlayer>(obj.players);
						list.Insert(num2, RDPlayer.NoChange);
						list.RemoveAt(15);
						if (list.Count != 16)
						{
							printe("something's not right");
						}
						obj.players = list.ToArray();
					}
				}
			}
		}

		public void AddNewDecoration(LevelEvent_MakeDecorationBase data)
		{
			using (new SaveStateScope())
			{
				int room = data.room;
				int num = LastDecorationOfRoom(room);
				int num2 = num + 1;
				if (num == -10 || num == decorationsData.Count)
				{
					decorationsData.Add(data);
					eventControls_decorations.Add(new List<LevelEventControl_Base>());
					num2 = LastDecorationOfRoom(room);
				}
				else
				{
					decorationsData.Insert(num2, data);
					eventControls_decorations.Insert(num2, new List<LevelEventControl_Base>());
				}
				foreach (LevelEventControl_Base eventControl in eventControls)
				{
					LevelEvent_Base levelEvent = eventControl.levelEvent;
					if (levelEvent.isDecorationTabEvent)
					{
						if (levelEvent.row >= num2)
						{
							levelEvent.row++;
						}
						eventControl.UpdateUI();
					}
				}
			}
		}

		public void DeleteAllSelectedEventControls(bool sound = false)
		{
			using (new SaveStateScope())
			{
				DeleteEventControlsFromList(selectedControls, sound);
			}
		}

		public void DeleteEventControlsFromList(List<LevelEventControl_Base> controls, bool sound = false)
		{
			if (sound)
			{
				string text = "";
				LevelEditorPlaySound(controls.Count switch
				{
					0 => "sndEditorErrorSmall", 
					1 => "sndEditorEventDelete", 
					_ => "sndEditorEventDeleteMultiple", 
				}, "LevelEditorActive");
			}
			while (controls.Count > 0)
			{
				LevelEventControl_Base levelEventControl_Base = controls.First();
				if (levelEventControl_Base != null)
				{
					DeleteEventControl(levelEventControl_Base, selectControlToTheLeft: false);
				}
				controls.Remove(levelEventControl_Base);
			}
		}

		public void UpdateTimelineAccordingToLevelEventType(LevelEventType type)
		{
			if (RDEditorUtils.LevelEventTypeShouldUpdateTimeline(type))
			{
				if (type == LevelEventType.SetRowXs)
				{
					base.timeline.UpdateSetBeatModifiersCache();
					foreach (List<LevelEventControl_Base> eventControls_row in eventControls_rows)
					{
						foreach (LevelEventControl_Base item in eventControls_row)
						{
							if (item is LevelEventControl_AddClassicBeat)
							{
								item.UpdateUI();
							}
						}
					}
				}
				else
				{
					bool updateControls = type == LevelEventType.SetCrotchetsPerBar;
					base.timeline.UpdateUI(updateControls);
				}
			}
			if (type != LevelEventType.None)
			{
				base.timeline.UpdateMaxUsedY();
			}
		}

		public void DeleteEventControl(LevelEventControl_Base eventControl, bool selectControlToTheLeft, bool sound = false)
		{
			ClearHoveredEventControl();
			using (new SaveStateScope())
			{
				eventControl.isShownAsSelected = false;
				eventControl.levelEvent.OnDelete();
				try
				{
					eventControl.container.Remove(eventControl);
				}
				catch (Exception ex)
				{
					RDBaseDllDummy.printem("control deleted already: " + ex.Message);
				}
				eventControls.Remove(eventControl);
				selectedControls.Remove(eventControl);
				UpdateTimelineAccordingToLevelEventType(eventControl.levelEvent.type);
				if (selectControlToTheLeft)
				{
					LevelEventControl_Base controlToTheLeft = GetControlToTheLeft(eventControl);
					if (controlToTheLeft != null)
					{
						SelectEventControl(controlToTheLeft);
					}
				}
				if (sound)
				{
					LevelEditorPlaySound("sndEditorEventDelete", "LevelEditorActive");
				}
				if (eventControl != null && eventControl.gameObject != null)
				{
					UnityEngine.Object.Destroy(eventControl.gameObject);
				}
				if (selectedControls.Count == 0)
				{
					currentTabSection.ShowNothingSelectedPanel();
				}
				else if (selectedControls.Count > 1)
				{
					currentTabSection.ShowMultipleSelectedPanel();
				}
				else
				{
					InspectorPanel_UpdateUI();
				}
			}
		}

		public void SetHoveredEventControl(LevelEventControl_Base eventControl)
		{
			if (!(eventControl == hoveredLevelEventControl))
			{
				hoveredLevelEventControl = eventControl;
				UpdateEventControlTooltip();
			}
		}

		private void SetEventControlTooltipData(LevelEventControl_Base eventControl)
		{
			hoveredLevelEventControlDisplayingTooltip = eventControl;
			conditionalsPreview.Show(eventControl.levelEvent, eventControl.transform.position + new Vector3(4f, 2f), topToBottom: false, 0.5f);
			LevelEvent_Base levelEvent = eventControl.levelEvent;
			string text = levelEvent.tag;
			if (!text.IsNullOrEmpty())
			{
				Vector3[] fourCornersArray = new Vector3[4];
				conditionalsPreview.box.GetWorldCorners(fourCornersArray);
			}
			tagPreview.text = text;
			string tooltipText = levelEvent.GetTooltipText();
			if (!tooltipText.IsNullOrEmpty())
			{
				eventTooltip.font = RDString.GetAppropiateFontForString(tooltipText);
				eventTooltip.color = eventControl.initialColor;
				eventTooltip.GetComponent<EightSidedOutline>().effectColor = ((levelEvent is LevelEvent_Comment && RDEditorUtils.ColorIsDark(eventTooltip.color)) ? Color.white : Color.black);
			}
			eventTooltip.text = tooltipText;
			bool flag = false;
			foreach (LevelEventControl_Base eventControls_window in eventControls_windows)
			{
				if (eventControls_window.levelEvent is LevelEvent_SetWindowContent && ((LevelEvent_SetWindowContent)eventControls_window.levelEvent).contentMode == WindowContentMode.Room)
				{
					flag = true;
					break;
				}
			}
			bool flag2 = ((eventControls_rooms.Count != 0) | flag) && levelEvent.roomsUsage != RoomsUsage.NotUsed && !(levelEvent is LevelEvent_ShowRooms);
			eventRoomsTooltip.SetRooms(flag2 ? levelEvent.rooms : null);
		}

		private void UpdateEventControlTooltip()
		{
			bool flag = alwaysShowTooltips || Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
			bool flag2 = hoveredLevelEventControlDisplayingTooltip != hoveredLevelEventControl;
			LevelEventControl_Base levelEventControl_Base = hoveredLevelEventControlDisplayingTooltip;
			if (hoveredLevelEventControl == null)
			{
				hoveredLevelEventControlDisplayingTooltip = hoveredLevelEventControl;
				flag = false;
			}
			if (((levelEventControl_Base != null) & flag2) && !levelEventControl_Base.selected)
			{
				levelEventControl_Base.UpdateDurationBar(show: false);
			}
			if (flag & flag2)
			{
				SetEventControlTooltipData(hoveredLevelEventControl);
			}
			if ((hoveredLevelEventControlDisplayingTooltip != null) | flag2)
			{
				conditionalsPreview.gameObject.SetActive(flag && conditionalsPreview.conditionalsCounter > 0);
				tagPreview.gameObject.SetActive(flag && !tagPreview.text.IsNullOrEmpty());
				eventTooltip.gameObject.SetActive(flag && !eventTooltip.text.IsNullOrEmpty());
				eventRoomsTooltip.gameObject.SetActive(flag && eventRoomsTooltip.enabled);
				LevelEventControl_Base levelEventControl_Base2 = hoveredLevelEventControl;
				if ((object)levelEventControl_Base2 != null && !levelEventControl_Base2.selected)
				{
					hoveredLevelEventControl.UpdateDurationBar(flag);
				}
			}
		}

		private void UpdateEventControlTooltipPosition()
		{
			if (hoveredLevelEventControl == null)
			{
				return;
			}
			LevelEventControl_Base levelEventControl_Base = hoveredLevelEventControl;
			if (conditionalsPreview.gameObject.activeSelf)
			{
				conditionalsPreview.Move(levelEventControl_Base.transform.position + new Vector3(4f, 2f), topToBottom: false, 0.5f);
			}
			if (tagPreview.gameObject.activeSelf)
			{
				RectTransform component = tagPreview.GetComponent<RectTransform>();
				Vector3[] array = new Vector3[4];
				conditionalsPreview.box.GetWorldCorners(array);
				if (conditionalsPreview.gameObject.activeSelf)
				{
					component.position = levelEventControl_Base.transform.position.WithY(array[1].y + 2f);
				}
				else
				{
					component.position = levelEventControl_Base.transform.position + Vector3.right * levelEventControl_Base.rt.sizeDelta.x / 2f * scaleFactor;
				}
			}
			if (eventTooltip.gameObject.activeSelf)
			{
				RectTransform component2 = eventTooltip.GetComponent<RectTransform>();
				component2.position = levelEventControl_Base.transform.position + new Vector3(levelEventControl_Base.rt.sizeDelta.x + 3f, (float)(-levelEventControl_Base.cellHeight) / 2f) * scaleFactor;
				Vector3[] array2 = new Vector3[4];
				base.timeline.rectTransform.GetWorldCorners(array2);
				Vector3 vector = array2[2];
				Vector3 vector2 = array2[3];
				component2.SizeDeltaX((vector.x - component2.position.x) / (float)scaleFactor);
				float num = component2.position.y - eventTooltip.preferredHeight * (float)scaleFactor;
				float num2 = Mathf.Max(0f, vector2.y - num);
				float y = component2.position.y + num2;
				component2.position = component2.position.WithY(y);
			}
			if (eventRoomsTooltip.gameObject.activeSelf)
			{
				eventRoomsTooltip.GetComponent<RectTransform>().position = levelEventControl_Base.transform.position + new Vector3(levelEventControl_Base.rt.sizeDelta.x - 2f, -2f) * scaleFactor;
			}
		}

		public void ClearHoveredEventControl()
		{
			conditionalsPreview.Hide();
			tagPreview.gameObject.SetActive(value: false);
			eventTooltip.gameObject.SetActive(value: false);
			eventRoomsTooltip.gameObject.SetActive(value: false);
			hoveredLevelEventControl = null;
		}

		public LevelEventControl_Base GetControlToTheLeft(LevelEventControl_Base control)
		{
			if (selectedControl == null)
			{
				return null;
			}
			List<LevelEventControl_Base> list = (from c in control.container
				orderby c.levelEvent.sortOrder, c.levelEvent.y
				select c).ToList();
			int num = list.IndexOf(control);
			do
			{
				num--;
				if (num < 0)
				{
					return null;
				}
			}
			while (!EventIsVisible(list[num].levelEvent));
			return list[num];
		}

		public LevelEventControl_Base GetControlToTheRight(LevelEventControl_Base control)
		{
			if (selectedControl == null)
			{
				return null;
			}
			List<LevelEventControl_Base> list = (from c in control.container
				orderby c.levelEvent.sortOrder, c.levelEvent.y
				select c).ToList();
			int num = list.IndexOf(control);
			do
			{
				num++;
				if (num > list.Count - 1)
				{
					return null;
				}
			}
			while (!EventIsVisible(list[num].levelEvent));
			return list[num];
		}

		public void DeleteEventClick()
		{
			if (selectedControls.Count == 1)
			{
				DeleteEventControl(selectedControl, selectControlToTheLeft: true, sound: true);
			}
			else if (selectedControls.Count > 1)
			{
				DeleteAllSelectedEventControls(sound: true);
			}
			else
			{
				RDBaseDllDummy.printem("No event to delete, ignoring");
			}
		}

		public void DeleteRowClick()
		{
			if (selectedRowIndex == -10)
			{
				RDBaseDllDummy.printem("no row selected, ignoring");
				return;
			}
			using (new SaveStateScope())
			{
				int num = selectedRowIndex;
				LevelEditorPlaySound("sndEditorPanelDelete", "LevelEditorActive");
				if (num >= rowsData.Count)
				{
					return;
				}
				List<LevelEventControl_Base> list = eventControls_rows[num];
				LevelEvent_MakeRow item = rowsData[num];
				while (list.Count > 0)
				{
					DeleteEventControl(list[0], selectControlToTheLeft: false);
				}
				eventControls_rows.Remove(list);
				rowsData.Remove(item);
				List<LevelEventControl_Base> list2 = new List<LevelEventControl_Base>();
				foreach (LevelEventControl_Base eventControl in eventControls)
				{
					LevelEvent_Base levelEvent = eventControl.levelEvent;
					if (levelEvent.isDecorationTabEvent)
					{
						continue;
					}
					if (levelEvent.row > num)
					{
						levelEvent.row--;
						eventControl.UpdateUI();
					}
					else if (levelEvent.row == num)
					{
						list2.Add(eventControl);
					}
					if (levelEvent is LevelEvent_ChangePlayersRows)
					{
						LevelEvent_ChangePlayersRows levelEvent_ChangePlayersRows = (LevelEvent_ChangePlayersRows)levelEvent;
						for (int i = num; i < levelEvent_ChangePlayersRows.players.Length - 1; i++)
						{
							levelEvent_ChangePlayersRows.players[i] = levelEvent_ChangePlayersRows.players[i + 1];
						}
					}
					if (levelEvent is LevelEvent_SpinningRows levelEvent_SpinningRows)
					{
						if (levelEvent_SpinningRows.toRow == num && levelEvent.row == num)
						{
							continue;
						}
						if (levelEvent_SpinningRows.toRow >= num)
						{
							levelEvent_SpinningRows.toRow--;
							eventControl.UpdateUI();
						}
					}
					if (levelEvent is LevelEvent_LinkRows levelEvent_LinkRows && (levelEvent_LinkRows.sourceRow != num || levelEvent.row != num) && levelEvent_LinkRows.sourceRow >= num)
					{
						levelEvent_LinkRows.sourceRow--;
						eventControl.UpdateUI();
					}
				}
				while (list2.Count > 0)
				{
					LevelEventControl_Base levelEventControl_Base = list2[0];
					printe("deleting: " + levelEventControl_Base.levelEvent);
					list2.Remove(levelEventControl_Base);
					DeleteEventControl(levelEventControl_Base, selectControlToTheLeft: false);
				}
				selectedRowIndex = -10;
				UpdateUI_AllTabSections();
				currentTabSection.ShowNothingSelectedPanel();
			}
		}

		public void DeleteDecorationClick()
		{
			LevelEvent_MakeDecorationBase decorationData = DecorationHeader.GetDecorationData(selectedDecoration);
			if (decorationData == null)
			{
				RDBaseDllDummy.printem("no decoration selected, ignoring");
				return;
			}
			using (new SaveStateScope())
			{
				LevelEditorPlaySound("sndEditorPanelDelete", "LevelEditorActive");
				int decorationDataIndex = DecorationHeader.GetDecorationDataIndex(decorationData.decorationId);
				List<LevelEventControl_Base> list = eventControls_decorations[decorationDataIndex];
				LevelEventControl_Base[] array = list.ToArray();
				foreach (LevelEventControl_Base levelEventControl_Base in array)
				{
					if (levelEventControl_Base.levelEvent.target == decorationData.decorationId)
					{
						DeleteEventControl(levelEventControl_Base, selectControlToTheLeft: false);
					}
				}
				eventControls_decorations.Remove(list);
				decorationsData.Remove(decorationData);
				foreach (LevelEventControl_Base eventControl in eventControls)
				{
					LevelEvent_Base levelEvent = eventControl.levelEvent;
					if (levelEvent.isDecorationTabEvent)
					{
						if (levelEvent.row > decorationDataIndex)
						{
							levelEvent.row--;
						}
						eventControl.UpdateUI();
					}
				}
				selectedDecoration = null;
				UpdateUI_AllTabSections();
				currentTabSection.ShowNothingSelectedPanel();
			}
		}

		public void RewindButtonClick()
		{
			if (eyedropperActive)
			{
				CancelCrosshairSelection();
			}
			LevelEditorPlaySound("sndEditorTransportScrub", "LevelEditorActive");
			startBar = 0;
			base.timeline.whereToGo = 0f;
			if (base.timeline.bookmarks.Count == 0)
			{
				base.timeline.FirstPage();
			}
			if (base.game == null || base.game.started)
			{
				ReloadGameScene();
			}
			else
			{
				UnloadGameScene(loadGameSceneAfterwards: false);
			}
		}

		public void NextButtonClick()
		{
			if (isPlaying)
			{
				LevelEditorPlaySound("sndEditorTransportScrub", "LevelEditorActive");
				base.conductor.ScrubToNextBar();
			}
		}

		public void PreviousButtonClick()
		{
			if (eyedropperActive)
			{
				CancelCrosshairSelection();
			}
			BarAndBeat barAndBeatWithPosX = base.timeline.GetBarAndBeatWithPosX(base.timeline.playhead.anchoredPosition.x);
			if (barAndBeatWithPosX.bar > 1 && barAndBeatWithPosX.beat == 1f && !isPlaying)
			{
				barAndBeatWithPosX.bar--;
			}
			LevelEditorPlaySound("sndEditorTransportScrub", "LevelEditorActive");
			ScrubToBar(barAndBeatWithPosX.bar, isPlaying);
		}

		public void AutoplayButtonClick()
		{
			RDEditorBase.debugSettings.Auto = !RDEditorBase.debugSettings.Auto;
			string sound = (RDEditorBase.debugSettings.Auto ? "sndOttoActivate" : "sndOttoDeactivate");
			LevelEditorPlaySound(sound, "LevelEditorOtto");
		}

		public void TwoPlayerButtonClick()
		{
			GC.twoPlayerMode = !GC.twoPlayerMode;
			base.tabSection_rows.UpdateUI();
			if (base.game != null)
			{
				base.game.EnsureSwitchPlayersState();
			}
			if (GC.twoPlayerMode)
			{
				LevelEditorPlaySound("sndTwoPlayersActivateR", "LevelEditorActive", 1f, 1f, GC.PanP1);
				LevelEditorPlaySound("sndTwoPlayersActivateL", "LevelEditorActive", 1f, 1f, GC.PanP2);
			}
			else
			{
				LevelEditorPlaySound("sndTwoPlayersDeactivate", "LevelEditorActive");
			}
			if (base.game != null)
			{
				base.game.OnChangedPlayerMode();
			}
		}

		public void MetronomeButtonClick()
		{
			metronomeActive = !metronomeActive;
			if (!metronomeActive)
			{
				RDUtils.SetMixerVolume("MetronomeClicksVolume", 0f);
				LevelEditorPlaySound("sndMetronomeClickLow", "LevelEditorActive");
				if (metronomeFlashTween != null)
				{
					metronomeFlashTween.Kill(complete: true);
					metronomeFlashTween = null;
				}
				if (base.game != null)
				{
					foreach (AudioSource metronomeAudioSource in base.game.currentLevel.metronomeAudioSources)
					{
						if (metronomeAudioSource != null)
						{
							metronomeAudioSource.volume = 0f;
						}
					}
				}
			}
			else
			{
				RDUtils.SetMixerVolume("MetronomeClicksVolume", 1f);
				LevelEditorPlaySound("sndMetronomeClickHigh", "LevelEditorActive");
				if (base.game != null)
				{
					foreach (AudioSource metronomeAudioSource2 in base.game.currentLevel.metronomeAudioSources)
					{
						if (metronomeAudioSource2 != null)
						{
							metronomeAudioSource2.volume = scrConductor.songOffsets.Get(metronomeAudioSource2.clip.name).volume;
						}
					}
				}
			}
			metronome.color = (metronomeActive ? metronomeOnColor : metronomeOffColor);
		}

		public void PlayPauseButtonClick()
		{
			if (eyedropperActive)
			{
				CancelCrosshairSelection();
			}
			if (!isPlaying && levelSettingsPanel.previewSongPlayer.Playing)
			{
				levelSettingsPanel.previewSongPlayerImage.sprite = playSprite;
				levelSettingsPanel.previewSongPlayer.Stop();
			}
			if (base.game == null)
			{
				speedToPlayAfterLoading = LevelSpeed;
				ReloadGameScene(playAsSoonItReloads: true);
				LevelEditorPlaySound("sndEditorTransportPlay", "LevelEditorActive");
				return;
			}
			if (base.game.gameState == GameState.PreStart && !base.game.paused && base.game.levelFinishedLoading)
			{
				StartPlaying(LevelSpeed);
			}
			if (base.game.started)
			{
				base.game.TogglePauseGame();
				LevelEditorPlaySound("sndEditorTransportPause", "LevelEditorActive");
				UpdateUI_PlayPauseButton();
			}
			else
			{
				if (base.timeline.bookmarks.Count <= 0)
				{
					return;
				}
				tlBookmark tlBookmark2 = base.timeline.bookmarks[0];
				foreach (tlBookmark bookmark in base.timeline.bookmarks)
				{
					if (bookmark.rt.localPosition.x < tlBookmark2.rt.localPosition.x)
					{
						tlBookmark2 = bookmark;
					}
				}
				BarAndBeat barAndBeat = new BarAndBeat(base.timeline.GetBarAndBeatWithPosX(tlBookmark2.rt.localPosition.x).bar, 1f);
				base.editor.ScrubToBar(barAndBeat.bar);
			}
		}

		public void PlaySound(string sound)
		{
			LevelEditorPlaySound(sound);
		}

		public void FullscreenButtonClick()
		{
			if (copyrightPopup.activeInHierarchy)
			{
				return;
			}
			fullscreen = !fullscreen;
			float pitch = RDUtils.PitchSemitonesToPercent(fullscreen ? 3f : (-2f));
			LevelEditorPlaySound("sndEditorToggleFullscreen", "LevelEditorPassive", 1f, pitch);
			if (fullscreen)
			{
				ClosePopup();
			}
			else
			{
				base.timeline.scrollRect.enabled = true;
				base.timeline.verticalScrollRect.enabled = true;
				tabPanelsCanvasGroup.alpha = ((!tabPanelsHidden) ? 1 : 0);
				Cursor.visible = true;
			}
			RectTransform rectTransform = gameView.rectTransform;
			Vector2 sizeDelta = canvasRectTransform.sizeDelta;
			float num = Mathf.Min(sizeDelta.y / rectTransform.sizeDelta.y, sizeDelta.x / rectTransform.sizeDelta.x);
			Vector3 endValue = (fullscreen ? new Vector3(num, num, 1f) : Vector3.one);
			Vector2 vector = (uiOffset + (sizeDelta - uiOffset) / 2f) / sizeDelta;
			panel.pivot = new Vector2(0.5f + (vector.x - 0.5f) * (1f + 1f / (num - 1f)), 0.5f + (vector.y - 0.5f) * (1f + 1f / (num - 1f)));
			panel.DOScale(endValue, fullscreenAnimationDuration).SetEase(fullscreenAnimationEaseScale).SetUpdate(UpdateType.Normal, isIndependentUpdate: true)
				.SetId("noKillOnReload")
				.OnComplete(() =>
				{
					if (fullscreen)
					{
						base.timeline.scrollRect.enabled = false;
						base.timeline.verticalScrollRect.enabled = false;
						tabPanelsCanvasGroup.alpha = 0f;
						Cursor.visible = false;
					}
				});
		}

		public void ReloadGameScene(bool playAsSoonItReloads = false, bool isNewlyOpenedFile = false)
		{
			if (base.game != null && !base.game.levelFinishedLoading)
			{
				Debug.LogWarning("reloading game already!");
				return;
			}
			reloadingGame = true;
			if (base.game != null && !base.game.paused)
			{
				_ = base.game.gameState;
			}
			previewNeedsReload = false;
			SaveWindowDance();
			this.playAsSoonItReloads = playAsSoonItReloads;
			rdlevelText = EncodeData();
			levelPlayMode = levelSettings.canBePlayedOn;
			if (isNewlyOpenedFile)
			{
				switch (levelPlayMode)
				{
				case LevelPlayMode.OnePlayerOnly:
					GC.twoPlayerMode = false;
					break;
				case LevelPlayMode.TwoPlayerOnly:
					GC.twoPlayerMode = true;
					break;
				}
				if (GetLastUsedFilePath().ToLower().Contains("2p"))
				{
					GC.twoPlayerMode = true;
				}
			}
			UnloadGameScene(loadGameSceneAfterwards: true);
		}

		private void UnloadGameScene(bool loadGameSceneAfterwards)
		{
			loadGameSceneAfterUnloading = loadGameSceneAfterwards;
			if (SceneManager.sceneCount != 1)
			{
				if (base.game != null && base.game.windowChoreographer != null)
				{
					base.game.windowChoreographer.enabled = false;
				}
				if (scnGameUnload == null)
				{
					scnGameUnload = SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("scnGame"));
				}
				Singleton<AudioManager>.Instance.DestroyAllAudioSources();
			}
		}

		private void LoadGameScene()
		{
			scnGame.levelToLoadSource = LevelSource.ExternalPath;
			scnGameLoad = SceneManager.LoadSceneAsync("scnGame", LoadSceneMode.Additive);
			gameStatus.gameObject.SetActive(value: true);
			gameStatus.text = RDString.Get("editor.loading");
			base.ipm.UpdateBlankPanel();
		}

		public void SelectEventControl(LevelEventControl_Base newControl, bool sound = false)
		{
			if (!(selectedControl == newControl))
			{
				if (currentTab == Tab.Decorations && newControl.levelEvent.type != LevelEventType.None)
				{
					lastUsedDecoration = newControl.levelEvent.target;
				}
				else
				{
					lastUsedRow = newControl.levelEvent.row;
				}
				DeselectAllEventControls(updateInspectorUI: false);
				AddEventControlToSelection(newControl, sound);
			}
		}

		public void AddEventControlToSelection(LevelEventControl_Base newControl, bool sound = false)
		{
			if (selectedControls.Contains(newControl))
			{
				DeselectEventControl(newControl);
			}
			else
			{
				newControl.ShowAsSelected();
				selectedControls.Add(newControl);
			}
			if (sound)
			{
				string sound2 = ((selectedControls.Count > 1) ? "sndEditorEventSelectAdditional" : "sndEditorEventSelect");
				LevelEditorPlaySound(sound2, "LevelEditorActive");
			}
			InspectorPanel_UpdateUI();
		}

		public void SelectEventControls(List<LevelEventControl_Base> controls)
		{
			DeselectAllEventControls(updateInspectorUI: false);
			foreach (LevelEventControl_Base control in controls)
			{
				AddEventControlToSelection(control);
			}
		}

		private void InspectorPanel_UpdateUI()
		{
			switch (selectedControls.Count)
			{
			case 0:
				currentTabSection.ShowNothingSelectedPanel();
				break;
			case 1:
				selectedControl.ShowDataOnInspector();
				break;
			default:
				currentTabSection.ShowMultipleSelectedPanel();
				break;
			}
		}

		public void ToggleExpandLeftPanel()
		{
			leftPanelIsExpanded = !leftPanelIsExpanded;
			float pitch = (leftPanelIsExpanded ? RDUtils.PitchSemitonesToPercent(3f) : RDUtils.PitchSemitonesToPercent(-2f));
			LevelEditorPlaySound("sndEditorTogglePanelExpand", "LevelEditorActive", 1f, pitch);
			float y = (leftPanelIsExpanded ? 2f : (bottomPanel.sizeDelta.y + 4f));
			leftPanel.offsetMin = leftPanel.offsetMin.WithY(y);
			bottomPanel.offsetMin = bottomPanel.offsetMin.WithX(leftPanelIsExpanded ? 172f : 0f);
		}

		public void DeselectEventControl(LevelEventControl_Base eventControl)
		{
			if (eventControl.isBase)
			{
				DeleteEventControl(eventControl, selectControlToTheLeft: false);
				return;
			}
			eventControl.SaveData();
			eventControl.ShowAsDeselected();
			selectedControls.Remove(eventControl);
		}

		public void DeselectAllEventControls(bool updateInspectorUI = true, bool sound = false)
		{
			List<LevelEventControl_Base> list = new List<LevelEventControl_Base>();
			if (sound)
			{
				LevelEditorPlaySound("sndEditorEventDeselect", "LevelEditorActive");
			}
			foreach (LevelEventControl_Base selectedControl in selectedControls)
			{
				if (selectedControl.isBase)
				{
					list.Add(selectedControl);
					continue;
				}
				selectedControl.SaveData();
				selectedControl.ShowAsDeselected();
			}
			while (list.Count > 0)
			{
				LevelEventControl_Base levelEventControl_Base = list[0];
				DeleteEventControl(levelEventControl_Base, selectControlToTheLeft: false);
				list.Remove(levelEventControl_Base);
			}
			if (DecorationHeader.decorationTabWhereAddButtonWasPressed != -1)
			{
				DecorationHeader.decorationTabWhereAddButtonWasPressed = -1;
				base.editor.tabSection_decorations.UpdateUI();
			}
			selectedControls.Clear();
			if (updateInspectorUI)
			{
				InspectorPanel_UpdateUI();
			}
		}

		public void DeselectEverything(bool sound = true)
		{
			colorPickerPopup.Hide();
			characterPickerPopup.Hide();
			letterMovementPickerPopup.Hide();
			soundSettingsPopup.Show(show: false);
			CloseCurrentActiveDropdown();
			CancelCrosshairSelection();
			DeselectAllEventControls(updateInspectorUI: true, sound);
		}

		public void BackButtonClick()
		{
			Debug.LogWarning("Back not implemented");
		}

		private void SetOpenRecentText(string filePath, int index)
		{
			string text = RDString.Get(openRecentSaysShow ? "editor.showRecent" : "editor.openRecent");
			int num = ((index == 0 && openRecentShortcutOn) ? 15 : 25);
			string text2 = Path.GetFileNameWithoutExtension(filePath);
			switch (text2)
			{
			case "main":
			case "main.night":
			case "level":
			case "tutorial":
			{
				string empty = string.Empty;
				try
				{
					empty = new DirectoryInfo(Path.GetDirectoryName(filePath)).Name;
				}
				catch (Exception arg)
				{
					Debug.Log($"Most recent name error: {arg}");
				}
				if (!empty.IsNullOrEmpty())
				{
					text2 = Path.Combine(empty, text2);
				}
				break;
			}
			}
			if (text2.Length > num)
			{
				text2 = text2.Substring(0, num - 1) + "...";
			}
			string newValue = "<color=#00ddee>" + text2 + "</color>";
			if (index == 0)
			{
				mostRecentFile.text = text.Replace("[file]", newValue);
			}
			else
			{
				olderFiles[index - 1].text = text.Replace("[file]", newValue);
			}
		}

		public void MenuButtonClick()
		{
			if (!mainMenu.activeSelf)
			{
				ShowMenuDropdown();
			}
			else
			{
				HideMenuDropdown();
			}
		}

		public void RecentFilesHover()
		{
			hoveringOverRecentFiles = true;
			if (!recentFilesMenu.activeSelf)
			{
				recentFilesMenu.SetActive(value: true);
			}
		}

		public void RecentFilesUnhover()
		{
			hoveringOverRecentFiles = false;
		}

		public void CycleSnapValues(int direction)
		{
			denominatorIndex = RXMath.Wrap(denominatorIndex + direction, denominatorValues.Length);
			UpdateSnapLabel();
			string sound = ((denominatorIndex != denominatorValues.Length - 1) ? "sndEditorToggleGrid" : "sndEditorToggleGridFreetime");
			float pitch = ((denominatorIndex != denominatorValues.Length - 1) ? RDUtils.PitchSemitonesToPercent((float)denominatorIndex - 4f) : 1f);
			LevelEditorPlaySound(sound, "LevelEditorTimeline", 1f, pitch);
		}

		public void UpdateSnapLabel()
		{
			bool flag = denominatorIndex == denominatorValues.Length - 1;
			snapValueUI.text = "1/" + denominator;
			snapValueUI.gameObject.SetActive(!flag);
			snapValueInput.gameObject.SetActive(flag);
		}

		public void UpdateFreetimeSnapValue(string s)
		{
			if (int.TryParse(s, out var result))
			{
				if (result > 0)
				{
					denominatorValues[^1] = result;
				}
				else
				{
					snapValueInput.text = denominatorValues[^1].ToString();
				}
			}
		}

		public void LevelSettingsClick()
		{
			InspectorPanel.ShowPanel(rankSettingsPanel, RDString.Get("editor.LevelSettings"), Tab.Song);
			rankSettingsPanel.GetComponent<InspectorPanel_LevelSettings>().UpdateUI();
		}

		public void AllConditionalsClick()
		{
			conditionalsPanel.currentPanel = null;
			InspectorPanel.ShowPanel(conditionalsPanel.transform, RDString.Get("editor.Conditionals"), Tab.Actions);
		}

		public void SetLastConditionalToSelectedEvents()
		{
			int editingId = conditionalsPanel.editingId;
			foreach (LevelEventControl_Base selectedControl in selectedControls)
			{
				if ((selectedControl is LevelEventControl_Action || selectedControl is LevelEventControl_Room) && !selectedControl.levelEvent.conditionals.Contains(editingId))
				{
					selectedControl.levelEvent.conditionals.Add(editingId);
					selectedControl.UpdateUIInternal();
				}
			}
		}

		public void ShowPanelConditionals(InspectorPanel panel)
		{
			conditionalsPanel.currentPanel = panel;
			conditionalsPanel.transform.SetAsLastSibling();
			conditionalsPanel.gameObject.SetActive(value: true);
		}

		public void CalibrationClick()
		{
			InspectorPanel.ShowPanel(calibrationPanel, RDString.Get("editor.Calibration"), Tab.Song);
			calibrationPanel.GetComponent<InspectorPanel_Calibration>().UpdateUI();
		}

		public void ActionsShowClick()
		{
			bool activeSelf = inspectorPanelManager.eventListPanels[currentTabSection.tabIndex].activeSelf;
			DeselectAllEventControls();
			if (!activeSelf)
			{
				ShowLevelEventSelectorPanel(filterMode: true);
			}
		}

		public void ShowLevelEventSelectorPanel(bool filterMode = false, EventCategory categoryToShow = EventCategory.None)
		{
			this.filterMode = filterMode;
			inspectorPanelManager.HideAll();
			inspectorTitle.text = RDString.Get("editor.events");
			inspectorTitle.color = currentTabSection.color;
			GameObject gameObject = inspectorPanelManager.eventListPanels[currentTabSection.tabIndex];
			gameObject.SetActive(value: true);
			gameObject.transform.SetAsLastSibling();
			SelectLevelEventPanel component = gameObject.GetComponent<SelectLevelEventPanel>();
			if (categoryToShow != EventCategory.None)
			{
				if (categoryToShow == EventCategory.MakeDecoration)
				{
					inspectorTitle.text = RDString.Get("editor.MakeDecoration.inspectorTitle");
				}
				component.ShowCategory(categoryToShow);
			}
			if (filterMode)
			{
				actionEventPanel = component;
			}
			inspectorPanelManager.background.color = currentTabSection.color;
			inspectorPanelManager.UpdateContentHeight(gameObject.GetComponent<RectTransform>());
		}

		public bool EventIsVisible(LevelEvent_Base levelEvent)
		{
			if (currentTab != Tab.Actions)
			{
				return true;
			}
			if (actionEventPanel == null)
			{
				GameObject gameObject = inspectorPanelManager.eventListPanels[2];
				actionEventPanel = gameObject.GetComponent<SelectLevelEventPanel>();
			}
			if (actionEventPanel.events.Count == 0)
			{
				return true;
			}
			if (ActionTypeIsVisible(levelEvent.type))
			{
				if (levelEvent.roomsUsage == RoomsUsage.NotUsed)
				{
					return true;
				}
				if (eventControls_rooms.Count == 0)
				{
					return true;
				}
				if (Array.Exists(levelEvent.rooms, (int r) => actionEventPanel.rooms.Contains(r)))
				{
					return true;
				}
			}
			return false;
		}

		public bool ActionTypeIsVisible(LevelEventType type)
		{
			SelectLevelEventButton selectLevelEventButton = actionEventPanel.events.Find((SelectLevelEventButton levelEvent) => levelEvent.type == type);
			if (selectLevelEventButton != null)
			{
				return selectLevelEventButton.selected;
			}
			if (type == LevelEventType.AdvanceText)
			{
				return actionEventPanel.events.Find((SelectLevelEventButton levelEvent) => levelEvent.type == LevelEventType.FloatingText).selected;
			}
			return true;
		}

		private bool ShowQuitDialog()
		{
			if (!theresChangesToSave || forceQuit)
			{
				return true;
			}
			Quit(quittingFromWantsToQuit: true);
			return false;
		}

		public void Cut()
		{
			Copy(copyEffect: false, isCut: true, sound: true);
			DeleteAllSelectedEventControls();
			HideMenuDropdown();
		}

		public void Copy(bool copyEffect = true, bool isCut = false, bool sound = false)
		{
			if (selectedControls.Count == 0)
			{
				if (sound)
				{
					LevelEditorPlaySound("sndEditorErrorSmall", "LevelEditorActive");
				}
				return;
			}
			clipboard.Clear();
			if (sound)
			{
				string sound2 = (isCut ? "sndEditorEventCut" : "sndEditorEventCopy");
				LevelEditorPlaySound(sound2, "LevelEditorActive");
			}
			int num = 0;
			foreach (LevelEventControl_Base selectedControl in selectedControls)
			{
				if (!selectedControl.isBase)
				{
					clipboard.Add(selectedControl.levelEvent.Clone());
				}
				num++;
			}
			if (clipboard.Count > 0)
			{
				StartCoroutine(CopyEffect());
			}
			HideMenuDropdown();
		}

		private IEnumerator CopyEffect()
		{
			yield return null;
			float num = float.MaxValue;
			float num2 = float.MinValue;
			float num3 = float.MinValue;
			foreach (LevelEventControl_Base selectedControl in selectedControls)
			{
				if (!selectedControl.isBase)
				{
					Image image = selectedControl.image;
					image.DOKill(complete: true);
					Color color = image.color;
					image.color = Color.white.WithAlpha(0.75f);
					image.DOColor(color.WithAlpha(selectedControl.selectedAlpha), 0.3f).SetUpdate(isIndependentUpdate: true);
					RectTransform component = selectedControl.GetComponent<RectTransform>();
					float b = component.anchoredPosition.x + component.sizeDelta.x * 0.5f;
					float b2 = component.anchoredPosition.y + component.sizeDelta.y;
					num = Mathf.Min(num, b);
					num2 = Mathf.Max(num2, b);
					num3 = Mathf.Max(num3, b2);
				}
			}
			float copiedLabel_animDuration = base.constants.copiedLabel_animDuration;
			float copiedLabel_animDistance = base.constants.copiedLabel_animDistance;
			Text copiedLabel = base.timeline.copiedLabel;
			copiedLabel.gameObject.SetActive(value: true);
			Transform obj = copiedLabel.transform;
			obj.SetParent(currentTabSection.containerTransform);
			obj.SetAsLastSibling();
			Vector2 anchoredPosition = new Vector2((num + num2) / 2f, num3);
			obj.DOKill();
			RectTransform component2 = copiedLabel.GetComponent<RectTransform>();
			component2.anchoredPosition = anchoredPosition;
			component2.SetParent(base.timeline.transform, worldPositionStays: true);
			float width = base.timeline.GetComponent<RectTransform>().rect.width;
			float num4 = component2.rect.width / 2f;
			component2.anchoredPosition = new Vector2(Mathf.Clamp(component2.anchoredPosition.x, num4, width - num4), (float)(base.timeline.usedRowCount * base.timeline.cellHeight) + component2.anchoredPosition.y);
			component2.DOAnchorPosY(component2.anchoredPosition.y + copiedLabel_animDistance, copiedLabel_animDuration).SetEase(Ease.Linear).SetUpdate(isIndependentUpdate: true);
			copiedLabel.DOKill();
			copiedLabel.color = copiedLabel.color.WithAlpha(1f);
			copiedLabel.DOColor(copiedLabel.color.WithAlpha(0f), copiedLabel_animDuration).SetEase(base.constants.copiedLabel_animEase).SetUpdate(isIndependentUpdate: true);
		}

		public void Paste()
		{
			Paste(onNextBar: false);
		}

		private void Paste(bool onNextBar)
		{
			HideMenuDropdown();
			if (clipboard.Count == 0)
			{
				printe("nothing to paste.");
				LevelEditorPlaySound("sndEditorErrorSmall", "LevelEditorActive");
				return;
			}
			string sound = ((clipboard.Count > 1) ? "sndEditorEventCreateMultiple" : "sndEditorEventCreate");
			LevelEditorPlaySound(sound, "LevelEditorActive");
			LevelEvent_Base levelEvent_Base = clipboard[0];
			LevelEvent_Base levelEvent_Base2 = clipboard[0];
			foreach (LevelEvent_Base item2 in clipboard)
			{
				if (item2.sortOrder < levelEvent_Base.sortOrder)
				{
					levelEvent_Base = item2;
				}
				else if (item2.sortOrder < levelEvent_Base.sortOrder)
				{
					levelEvent_Base2 = item2;
				}
			}
			BarAndBeat barAndBeat;
			if (onNextBar)
			{
				LevelEvent_Base levelEvent = selectedControls[0].levelEvent;
				foreach (LevelEventControl_Base selectedControl in selectedControls)
				{
					if (selectedControl.levelEvent.sortOrder < levelEvent.sortOrder)
					{
						levelEvent = selectedControl.levelEvent;
					}
				}
				int num = levelEvent_Base2.bar - levelEvent_Base.bar + 1;
				barAndBeat = new BarAndBeat(levelEvent.bar + num, 1f);
				base.timeline.ScrollTo(base.timeline.GetPosXFromBarAndBeat(levelEvent.barAndBeat), 0.3f);
			}
			else
			{
				float width = base.timeline.scrollview.rect.width;
				float posX = 0f - base.timeline.scrollviewContent.anchoredPosition.x + width / 2f;
				barAndBeat = base.timeline.GetBarAndBeatWithPosX(posX);
			}
			bool flag = false;
			bool flag2 = false;
			foreach (LevelEvent_Base item3 in clipboard)
			{
				if (item3.row == lastUsedRow)
				{
					flag = true;
				}
				if (item3.target == lastUsedDecoration)
				{
					flag2 = true;
				}
			}
			lastUsedRow = Math.Min(lastUsedRow, rowsData.Count - 1);
			using (new SaveStateScope())
			{
				List<LevelEventControl_Base> list = PasteFloatingAndAdvanceTextEvents(clipboard, out var clipboardEventsCloned, cloneEvents: true);
				int num2 = 0;
				foreach (LevelEvent_Base item4 in clipboard)
				{
					if (clipboardEventsCloned.Contains(item4))
					{
						continue;
					}
					if (item4.isClassicRowEvent || item4.isOneshotRowEvent)
					{
						if (lastUsedRow != -10 && !flag)
						{
							item4.row = lastUsedRow;
						}
						if (item4.row >= rowsData.Count)
						{
							continue;
						}
						LevelEvent_MakeRow levelEvent_MakeRow = rowsData[item4.row];
						if (levelEvent_MakeRow.rowType == RowType.Oneshot && item4.isClassicRowEvent)
						{
							num2++;
							continue;
						}
						if (levelEvent_MakeRow.rowType == RowType.Classic && item4.isOneshotRowEvent)
						{
							num2++;
							continue;
						}
					}
					if (item4.isDecorationTabEvent)
					{
						LevelEvent_MakeDecorationBase decorationData = DecorationHeader.GetDecorationData(lastUsedDecoration);
						if (decorationData != null && !flag2)
						{
							item4.target = lastUsedDecoration;
							item4.row = decorationData.row;
						}
					}
					LevelEvent_Base levelEvent2 = item4.Clone();
					item4.GenerateNewUID();
					LevelEventControl_Base item = CreateEventControl(levelEvent2, item4.defaultTab);
					list.Add(item);
				}
				foreach (LevelEventControl_Base item5 in list)
				{
					item5.bar = item5.bar - levelEvent_Base.bar + barAndBeat.bar;
					item5.UpdateUI();
				}
				SelectEventControls(list);
			}
		}

		public List<LevelEventControl_Base> PasteFloatingAndAdvanceTextEvents(List<LevelEvent_Base> currentClipboard, out List<LevelEvent_Base> clipboardEventsCloned, bool cloneEvents)
		{
			List<LevelEventControl_Base> list = new List<LevelEventControl_Base>();
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			clipboardEventsCloned = new List<LevelEvent_Base>();
			foreach (LevelEvent_Base item3 in currentClipboard)
			{
				if (item3 is LevelEvent_FloatingText)
				{
					int id = (item3 as LevelEvent_FloatingText).id;
					LevelEvent_FloatingText levelEvent_FloatingText = (cloneEvents ? item3.Clone() : item3) as LevelEvent_FloatingText;
					levelEvent_FloatingText.GenerateNewUID();
					levelEvent_FloatingText.GenerateNewID();
					LevelEventControl_Base item = CreateEventControl(levelEvent_FloatingText, item3.defaultTab);
					int id2 = levelEvent_FloatingText.id;
					printe($"FloatingText oldId {id} newId {id2}");
					dictionary.Add(id, id2);
					clipboardEventsCloned.Add(item3);
					list.Add(item);
				}
			}
			foreach (LevelEvent_Base item4 in currentClipboard)
			{
				if (item4 is LevelEvent_AdvanceText)
				{
					LevelEvent_Base levelEvent_Base = (cloneEvents ? item4.Clone() : item4);
					LevelEventControl_Base item2 = CreateEventControl(levelEvent_Base, item4.defaultTab);
					LevelEvent_AdvanceText levelEvent_AdvanceText = levelEvent_Base as LevelEvent_AdvanceText;
					int id3 = levelEvent_AdvanceText.id;
					if (dictionary.ContainsKey(id3))
					{
						levelEvent_AdvanceText.id = dictionary[id3];
					}
					clipboardEventsCloned.Add(item4);
					list.Add(item2);
				}
			}
			return list;
		}

		public void Clone(LevelEventControlEventTrigger eventTrigger = null, bool skipSaveState = false)
		{
			using (new SaveStateScope())
			{
				List<LevelEvent_Base> list = new List<LevelEvent_Base>();
				LevelEvent_Base levelEvent_Base = null;
				foreach (LevelEventControl_Base selectedControl in selectedControls)
				{
					LevelEvent_Base levelEvent_Base2 = selectedControl.levelEvent.Clone();
					if (eventTrigger != null && selectedControl == eventTrigger.control)
					{
						levelEvent_Base = levelEvent_Base2;
					}
					list.Add(levelEvent_Base2);
				}
				List<LevelEventControl_Base> list2 = PasteFloatingAndAdvanceTextEvents(list, out var clipboardEventsCloned, cloneEvents: false);
				foreach (LevelEvent_Base item2 in list)
				{
					if (!clipboardEventsCloned.Contains(item2) && !item2.isBaseEvent)
					{
						LevelEventControl_Base item = CreateEventControl(item2, item2.defaultTab, skipSaveState);
						list2.Add(item);
					}
				}
				if (list2.Count == 0)
				{
					return;
				}
				DeselectAllEventControls(updateInspectorUI: false);
				string sound = ((list2.Count > 1) ? "sndEditorEventCreateMultiple" : "sndEditorEventCreate");
				LevelEditorPlaySound(sound, "LevelEditorActive");
				foreach (LevelEventControl_Base item3 in list2)
				{
					item3.UpdateUI();
					item3.UpdateUIInternal();
					if (eventTrigger != null && levelEvent_Base != null && item3.levelEvent == levelEvent_Base)
					{
						eventTrigger.currentTransform = item3.transform;
						eventTrigger.RecalculateCurrentCell();
					}
					AddEventControlToSelection(item3);
				}
			}
		}

		public void SyncLyricsToBeats()
		{
			HideMenuDropdown();
			List<LevelEvent_AddClassicBeat> list = new List<LevelEvent_AddClassicBeat>();
			List<LevelEvent_FloatingText> list2 = new List<LevelEvent_FloatingText>();
			foreach (List<LevelEventControl_Base> eventControls_row in eventControls_rows)
			{
				foreach (LevelEventControl_Base item in eventControls_row.Where((LevelEventControl_Base c) => c.levelEvent is LevelEvent_AddClassicBeat))
				{
					list.Add(item.levelEvent as LevelEvent_AddClassicBeat);
				}
			}
			foreach (LevelEventControl_Base item2 in eventControls_actions.Where((LevelEventControl_Base c) => c.levelEvent is LevelEvent_FloatingText))
			{
				list2.Add(item2.levelEvent as LevelEvent_FloatingText);
			}
			if (list.Count == 0)
			{
				LevelEditorPlaySound("sndEditorErrorSmall", "LevelEditorActive");
				return;
			}
			using (new SaveStateScope())
			{
				int num = list.Count - list2.Count;
				if (num > 0)
				{
					for (int num2 = 0; num2 < num; num2++)
					{
						LevelEvent_FloatingText levelEvent_FloatingText = new LevelEvent_FloatingText();
						levelEvent_FloatingText.GenerateNewUID();
						levelEvent_FloatingText.GenerateNewID();
						base.editor.CreateEventControl(levelEvent_FloatingText, Tab.Actions);
						list2.Add(levelEvent_FloatingText);
					}
				}
				for (int num3 = 0; num3 < list.Count; num3++)
				{
					LevelEvent_AddClassicBeat levelEvent_AddClassicBeat = list[num3];
					LevelEvent_FloatingText levelEvent_FloatingText2 = list2[num3];
					levelEvent_FloatingText2.barAndBeat = base.timeline.FixBarAndBeat(levelEvent_AddClassicBeat.barAndBeat);
					levelEvent_FloatingText2.y = levelEvent_AddClassicBeat.row;
					List<LevelEvent_AdvanceText> list3 = new List<LevelEvent_AdvanceText>();
					foreach (LevelEventControl_Base eventControls_action in eventControls_actions)
					{
						if (eventControls_action.levelEvent is LevelEvent_AdvanceText)
						{
							LevelEvent_AdvanceText levelEvent_AdvanceText = (LevelEvent_AdvanceText)eventControls_action.levelEvent;
							if (levelEvent_AdvanceText.id == levelEvent_FloatingText2.id)
							{
								list3.Add(levelEvent_AdvanceText);
							}
						}
					}
					for (int num4 = 1; num4 < 7; num4++)
					{
						bool flag = false;
						LevelEvent_AdvanceText levelEvent_AdvanceText2;
						if (num4 - 1 < list3.Count)
						{
							levelEvent_AdvanceText2 = list3[num4 - 1];
						}
						else
						{
							levelEvent_AdvanceText2 = new LevelEvent_AdvanceText();
							flag = true;
						}
						bool num5 = levelEvent_AddClassicBeat.swing != 0f;
						float num6 = (float)num4 * levelEvent_AddClassicBeat.tick;
						if (num5 && (num4 - 1) % 2 == 0)
						{
							num6 += levelEvent_AddClassicBeat.tick - levelEvent_AddClassicBeat.swing;
						}
						levelEvent_AdvanceText2.y = levelEvent_FloatingText2.y;
						levelEvent_AdvanceText2.id = levelEvent_FloatingText2.id;
						BarAndBeat barAndBeat = levelEvent_FloatingText2.barAndBeat;
						barAndBeat.beat += num6;
						levelEvent_AdvanceText2.barAndBeat = base.timeline.FixBarAndBeat(barAndBeat);
						if (flag)
						{
							base.editor.CreateEventControl(levelEvent_AdvanceText2, Tab.Actions);
						}
					}
				}
			}
			base.timeline.UpdateUI();
		}

		public void ScrollMouseActions()
		{
			if (selectedControls.Count == 0)
			{
				return;
			}
			float scrollWheel = RDEditorUtils.GetScrollWheel();
			if (scrollWheel == 0f)
			{
				return;
			}
			float num = 1f / (float)base.editor.denominator;
			float num2 = Mathf.Sign(scrollWheel) * num;
			bool flag = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
			bool flag2 = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
			for (int i = 0; i < selectedControls.Count; i++)
			{
				LevelEventControl_Base levelEventControl_Base = selectedControls[i];
				LevelEvent_Base levelEvent = levelEventControl_Base.levelEvent;
				LevelEvent_AddClassicBeat levelEvent_AddClassicBeat = levelEvent as LevelEvent_AddClassicBeat;
				LevelEvent_AddOneshotBeat levelEvent_AddOneshotBeat = levelEvent as LevelEvent_AddOneshotBeat;
				bool flag3 = false;
				if (flag)
				{
					BarAndBeat barAndBeatWithPosX = base.timeline.GetBarAndBeatWithPosX(levelEventControl_Base.transform.localPosition.x + num2 * (float)base.cellWidth);
					levelEvent.barAndBeat = barAndBeatWithPosX;
					levelEventControl_Base.SnapBeatToGrid();
					flag3 = true;
				}
				if (levelEvent_AddClassicBeat != null)
				{
					if (flag2)
					{
						levelEvent_AddClassicBeat.tick = Mathf.Round((levelEvent_AddClassicBeat.tick + num2) / num) * num;
						levelEvent_AddClassicBeat.tick = Mathf.Max(levelEvent_AddClassicBeat.tick, 0f);
						flag3 = true;
					}
				}
				else if (levelEvent_AddOneshotBeat != null && flag2)
				{
					levelEvent_AddOneshotBeat.tick = Mathf.Round((levelEvent_AddOneshotBeat.tick + num2) / num) * num;
					levelEvent_AddOneshotBeat.tick = Mathf.Max(levelEvent_AddOneshotBeat.tick, 0f);
					flag3 = true;
				}
				if (flag3)
				{
					levelEventControl_Base.UpdateUI();
					levelEvent.inspectorPanel.UpdateUI(levelEvent);
				}
			}
		}

		public int RowOrderInItsRoom(LevelEvent_MakeRow row)
		{
			int num = 0;
			for (int i = 0; i < rowsData.Count; i++)
			{
				LevelEvent_MakeRow levelEvent_MakeRow = rowsData[i];
				if (levelEvent_MakeRow.room == row.room)
				{
					if (levelEvent_MakeRow == row)
					{
						return num;
					}
					num++;
				}
			}
			RDBaseDllDummy.printem("Not found!");
			return -1;
		}

		public void MoveRowDown_Unity()
		{
			MoveRowVertically(rowsData[selectedRowIndex], VerticalDirection.Down);
			LevelEditorPlaySound("sndButtonClick", "LevelEditorActive", 1f, 1.4f);
		}

		public void MoveRowUp_Unity()
		{
			MoveRowVertically(rowsData[selectedRowIndex], VerticalDirection.Up);
			LevelEditorPlaySound("sndButtonClickOn", "LevelEditorActive", 1f, 1.4f);
		}

		public void MoveDecorationBottom_Unity()
		{
			MoveDecoration("sndButtonClick", VerticalDirection.Down, goAllTheWay: true);
		}

		public void MoveDecorationDown_Unity()
		{
			MoveDecoration("sndButtonClick", VerticalDirection.Down);
		}

		public void MoveDecorationUp_Unity()
		{
			MoveDecoration("sndButtonClickOn", VerticalDirection.Up);
		}

		public void MoveDecorationTop_Unity()
		{
			MoveDecoration("sndButtonClickOn", VerticalDirection.Up, goAllTheWay: true);
		}

		private void MoveDecoration(string sound, VerticalDirection direction, bool goAllTheWay = false)
		{
			if (goAllTheWay)
			{
				while (MoveDecorationVertically(DecorationHeader.GetDecorationData(selectedDecoration), direction))
				{
				}
			}
			else
			{
				MoveDecorationVertically(DecorationHeader.GetDecorationData(selectedDecoration), direction);
			}
			LevelEditorPlaySound(sound, "LevelEditorActive", 1f, 1.4f);
		}

		private void UpdateGamePreviewScale()
		{
			Vector2 vector = new Vector2(352f, 198f);
			if (base.game != null && base.game.windowChoreographer is VirtualWindowChoreographer { virtualDesktopRenderTexture: { } virtualDesktopRenderTexture })
			{
				float num = (float)virtualDesktopRenderTexture.width * 1f / (float)virtualDesktopRenderTexture.height;
				vector = ((num > 1.7777778f) ? new Vector2(vector.y * num, vector.y) : new Vector2(vector.x, vector.x / num));
			}
			Vector2 vector2 = new Vector2(gameArea.rect.width, gameArea.rect.height);
			Vector2 vector3 = vector / 2f;
			Vector2 vector4 = vector2 / vector3;
			maxGamePreviewScale = Mathf.Min(vector4.x, vector4.y);
			gamePreviewScale = Mathf.Clamp(preferredPreviewScale, 0f, maxGamePreviewScale);
			bool active = maxGamePreviewScale > 1.1f;
			gamePreviewScaleButton.gameObject.SetActive(active);
			gameView.rectTransform.sizeDelta = vector / 2f * gamePreviewScale;
			gamePreviewScaleText.text = $"{gamePreviewScale:#.#}x";
			if (gameView.texture != null)
			{
				bool flag = (gamePreviewScale % 1f == 0f && gamePreviewScale >= 2f) || fullscreen;
				gameView.texture.filterMode = ((!flag) ? FilterMode.Bilinear : FilterMode.Point);
			}
		}

		public void CycleGamePreviewScale()
		{
			float num = gamePreviewScale;
			if (Mathf.Approximately(gamePreviewScale, maxGamePreviewScale))
			{
				gamePreviewScale = 1f;
			}
			else if (gamePreviewScale + 1f > maxGamePreviewScale)
			{
				gamePreviewScale = maxGamePreviewScale - Mathf.Epsilon;
			}
			else
			{
				gamePreviewScale++;
			}
			bool flag = Mathf.Approximately(gamePreviewScale, maxGamePreviewScale);
			preferredPreviewScale = (flag ? 100f : gamePreviewScale);
			Persistence.SetLevelEditorGamePreviewScale(preferredPreviewScale);
			float pitch = ((gamePreviewScale >= num) ? 1.4f : 1f);
			LevelEditorPlaySound("sndButtonClick", "LevelEditorActive", 1f, pitch);
		}

		public void TakeScreenshot()
		{
			if (string.IsNullOrEmpty(openedFilePath))
			{
				RDEditorUtils.AskUserToSaveLevel(RDString.Get("editor.takeScreenshot.levelIsNotSaved"));
				LevelEditorPlaySound("sndEditorErrorLarge", "LevelEditorPassive");
			}
			else if (gameView.texture != null)
			{
				RenderTexture active = RenderTexture.active;
				RenderTexture renderTexture = (RenderTexture.active = gameView.texture as RenderTexture);
				Texture2D texture2D = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, mipChain: false);
				texture2D.ReadPixels(new Rect(0f, 0f, renderTexture.width, renderTexture.height), 0, 0);
				RenderTexture.active = active;
				byte[] bytes = texture2D.EncodeToPNG();
				RDFile.WriteAllBytes(RDUtils.GetAvailableFilename(Path.GetDirectoryName(openedFilePath) + "/screenshot", ".png"), bytes);
				LevelEditorPlaySound("sndEditorScreenshot", "LevelEditorActive");
			}
			else
			{
				printe("Texture is null");
			}
		}

		public void ToggleMute()
		{
			muteActive = !muteActive;
		}

		public void CycleWindowDance()
		{
			if (currentWindowMovement == WindowMovement.Simulate)
			{
				currentWindowMovement = Persistence.GetWindowMovement();
				if (currentWindowMovement != WindowMovement.Full && currentWindowMovement != WindowMovement.OneScreen)
				{
					currentWindowMovement = WindowMovement.Full;
				}
				windowDanceButtonImage.sprite = currentScreenWindowDance;
			}
			else
			{
				currentWindowMovement = WindowMovement.Simulate;
				windowDanceButtonImage.sprite = simulateWindowDance;
			}
			previewNeedsReload = true;
		}

		public void CycleWindowDanceRatio()
		{
			currentRatioIndex = (currentRatioIndex + 1) % ratios.Length;
			currentRatioText.text = ratioText[currentRatioIndex];
		}

		private void SaveWindowDance()
		{
			if (LevelUsesWindowDance)
			{
				Persistence.SetLevelEditorWindowMovement(currentWindowMovement);
			}
		}

		public bool MoveRowVertically(LevelEvent_MakeRow row, VerticalDirection direction)
		{
			using (new SaveStateScope())
			{
				if (selectedRowIndex == -10)
				{
					printe("no row was selected");
					return false;
				}
				int num = rowsData.IndexOf(row);
				LevelEvent_MakeRow levelEvent_MakeRow = null;
				if (direction == VerticalDirection.Down)
				{
					for (int i = num + 1; i < rowsData.Count; i++)
					{
						LevelEvent_MakeRow levelEvent_MakeRow2 = rowsData[i];
						if (levelEvent_MakeRow2.room == row.room)
						{
							levelEvent_MakeRow = levelEvent_MakeRow2;
							selectedRowIndex = i;
							break;
						}
					}
				}
				else
				{
					for (int num2 = num - 1; num2 >= 0; num2--)
					{
						LevelEvent_MakeRow levelEvent_MakeRow3 = rowsData[num2];
						if (levelEvent_MakeRow3.room == row.room)
						{
							levelEvent_MakeRow = levelEvent_MakeRow3;
							selectedRowIndex = num2;
							break;
						}
					}
				}
				if (levelEvent_MakeRow == null)
				{
					printe("Can't move further in the " + direction.ToString() + " direction.");
					return false;
				}
				printe("RowToSwap: " + levelEvent_MakeRow.character);
				SwapRowPositions(row, levelEvent_MakeRow);
				return true;
			}
		}

		public void SwapRowPositions(LevelEvent_MakeRow a, LevelEvent_MakeRow b)
		{
			int num = rowsData.IndexOf(a);
			int num2 = rowsData.IndexOf(b);
			rowsData[num] = b;
			rowsData[num2] = a;
			List<List<LevelEventControl_Base>> list = eventControls_rows;
			int index = num;
			List<List<LevelEventControl_Base>> list2 = eventControls_rows;
			int index2 = num2;
			List<LevelEventControl_Base> list3 = eventControls_rows[num2];
			List<LevelEventControl_Base> list4 = eventControls_rows[num];
			List<LevelEventControl_Base> list5 = (list[index] = list3);
			list5 = (list2[index2] = list4);
			foreach (LevelEventControl_Base item in eventControls_rows[num])
			{
				item.levelEvent.row = num;
				item.UpdateUI();
			}
			foreach (LevelEventControl_Base item2 in eventControls_rows[num2])
			{
				item2.levelEvent.row = num2;
				item2.UpdateUI();
			}
			List<LevelEvent_Base> list8 = new List<LevelEvent_Base>();
			List<LevelEvent_Base> list9 = new List<LevelEvent_Base>();
			foreach (LevelEventControl_Base eventControls_action in eventControls_actions)
			{
				if (eventControls_action.levelEvent.row == num)
				{
					list8.Add(eventControls_action.levelEvent);
				}
				if (eventControls_action.levelEvent.row == num2)
				{
					list9.Add(eventControls_action.levelEvent);
				}
				if (eventControls_action.levelEvent is LevelEvent_ChangePlayersRows levelEvent_ChangePlayersRows)
				{
					ref RDPlayer reference = ref levelEvent_ChangePlayersRows.players[num];
					ref RDPlayer reference2 = ref levelEvent_ChangePlayersRows.players[num2];
					RDPlayer rDPlayer = levelEvent_ChangePlayersRows.players[num2];
					RDPlayer rDPlayer2 = levelEvent_ChangePlayersRows.players[num];
					reference = rDPlayer;
					reference2 = rDPlayer2;
					ref Character reference3 = ref levelEvent_ChangePlayersRows.cpuMarkers[num];
					ref Character reference4 = ref levelEvent_ChangePlayersRows.cpuMarkers[num2];
					Character character = levelEvent_ChangePlayersRows.cpuMarkers[num2];
					Character character2 = levelEvent_ChangePlayersRows.cpuMarkers[num];
					reference3 = character;
					reference4 = character2;
				}
				if (eventControls_action.levelEvent is LevelEvent_SpinningRows levelEvent_SpinningRows)
				{
					if (levelEvent_SpinningRows.toRow == num)
					{
						levelEvent_SpinningRows.toRow = num2;
					}
					else if (levelEvent_SpinningRows.toRow == num2)
					{
						levelEvent_SpinningRows.toRow = num;
					}
				}
				if (eventControls_action.levelEvent is LevelEvent_LinkRows levelEvent_LinkRows)
				{
					if (levelEvent_LinkRows.sourceRow == num)
					{
						levelEvent_LinkRows.sourceRow = num2;
					}
					else if (levelEvent_LinkRows.sourceRow == num2)
					{
						levelEvent_LinkRows.sourceRow = num;
					}
				}
			}
			foreach (LevelEventControl_Base eventControls_sound in eventControls_sounds)
			{
				if (eventControls_sound.levelEvent.row == num)
				{
					list8.Add(eventControls_sound.levelEvent);
				}
				if (eventControls_sound.levelEvent.row == num2)
				{
					list9.Add(eventControls_sound.levelEvent);
				}
			}
			foreach (LevelEvent_Base item3 in list8)
			{
				item3.row = num2;
			}
			foreach (LevelEvent_Base item4 in list9)
			{
				item4.row = num;
			}
			base.tabSection_rows.UpdateUI();
		}

		public bool MoveDecorationVertically(LevelEvent_MakeDecorationBase decoration, VerticalDirection direction)
		{
			using (new SaveStateScope())
			{
				if (string.IsNullOrEmpty(selectedDecoration))
				{
					printe("no row was selected");
					return false;
				}
				int num = decorationsData.IndexOf(decoration);
				LevelEvent_MakeDecorationBase levelEvent_MakeDecorationBase = null;
				if (direction == VerticalDirection.Down)
				{
					for (int i = num + 1; i < decorationsData.Count; i++)
					{
						LevelEvent_MakeDecorationBase levelEvent_MakeDecorationBase2 = decorationsData[i];
						if (levelEvent_MakeDecorationBase2.room == decoration.room)
						{
							levelEvent_MakeDecorationBase = levelEvent_MakeDecorationBase2;
							break;
						}
					}
				}
				else
				{
					for (int num2 = num - 1; num2 >= 0; num2--)
					{
						LevelEvent_MakeDecorationBase levelEvent_MakeDecorationBase3 = decorationsData[num2];
						if (levelEvent_MakeDecorationBase3.room == decoration.room)
						{
							levelEvent_MakeDecorationBase = levelEvent_MakeDecorationBase3;
							break;
						}
					}
				}
				if (levelEvent_MakeDecorationBase == null)
				{
					printe("Can't move further in the " + direction.ToString() + " direction.");
					return false;
				}
				if (levelEvent_MakeDecorationBase is LevelEvent_Sprite levelEvent_Sprite)
				{
					printe("SpriteToSwap: " + levelEvent_Sprite.filename);
				}
				SwapDecorationPositions(decoration, levelEvent_MakeDecorationBase);
				return true;
			}
		}

		public void SwapDecorationPositions(LevelEvent_MakeDecorationBase a, LevelEvent_MakeDecorationBase b)
		{
			int num = decorationsData.IndexOf(a);
			int num2 = decorationsData.IndexOf(b);
			decorationsData[num] = b;
			decorationsData[num2] = a;
			List<LevelEventControl_Base> value = eventControls_decorations[num];
			eventControls_decorations[num] = eventControls_decorations[num2];
			eventControls_decorations[num2] = value;
			foreach (LevelEventControl_Base item in eventControls_decorations[num])
			{
				item.levelEvent.row = num;
				item.UpdateUI();
			}
			foreach (LevelEventControl_Base item2 in eventControls_decorations[num2])
			{
				item2.levelEvent.row = num2;
				item2.UpdateUI();
			}
			base.tabSection_decorations.UpdateUI();
		}

		public void CloneDecoration()
		{
			using (new SaveStateScope())
			{
				LevelEvent_MakeDecorationBase decorationData = DecorationHeader.GetDecorationData(selectedDecoration);
				LevelEvent_MakeDecorationBase levelEvent_MakeDecorationBase = (LevelEvent_MakeDecorationBase)Activator.CreateInstance(decorationData.GetType());
				string decorationId = levelEvent_MakeDecorationBase.decorationId;
				levelEvent_MakeDecorationBase.CopyFrom(decorationData, copyBarAndBeat: false);
				levelEvent_MakeDecorationBase.decorationId = decorationId;
				levelEvent_MakeDecorationBase.rooms = decorationData.rooms;
				levelEvent_MakeDecorationBase.row = LastDecorationOfRoom(levelEvent_MakeDecorationBase.room) + 1;
				base.editor.DeselectAllEventControls();
				base.editor.AddNewDecoration(levelEvent_MakeDecorationBase);
				ShowTabSection(Tab.Decorations);
				base.tabSection_decorations.ChangePage(decorationData.rooms[0], calledByUnity: false);
				base.tabSection_decorations.UpdateUI();
				base.tabSection_decorations.decorationHeaders[base.editor.currentPageDecorationsData.Count - 1].GetComponent<DecorationHeader>().Select();
				foreach (LevelEventControl_Base item in new List<LevelEventControl_Base>(eventControls_decorations[decorationsData.IndexOf(decorationData)]))
				{
					LevelEvent_Base levelEvent_Base = item.levelEvent.Clone();
					levelEvent_Base.target = levelEvent_MakeDecorationBase.decorationId;
					levelEvent_Base.row = levelEvent_MakeDecorationBase.row;
					levelEvent_Base.GenerateNewUID();
					CreateEventControl(levelEvent_Base, Tab.Decorations).UpdateUI();
				}
				while (decorationsData.IndexOf(levelEvent_MakeDecorationBase) != decorationsData.IndexOf(decorationData) + 1)
				{
					if (!MoveDecorationVertically(DecorationHeader.GetDecorationData(selectedDecoration), VerticalDirection.Up))
					{
						Debug.LogWarning("Somehow passed original decoration? Bailing out of while loop.");
						break;
					}
				}
			}
			LevelEditorPlaySound("sndEditorEventSetType");
		}

		public void ClosePopup()
		{
			roomsSelectionPopup.Hide();
			colorPickerPopup.Hide();
			publishPopup.Hide();
			characterPickerPopup.Hide();
			letterMovementPickerPopup.Hide();
			conditionalsPanel.TogglePopup(show: false);
			soundSettingsPopup.Show(show: false);
			CloseCurrentActiveDropdown();
			ToggleInsertFromUrlPopup(show: false);
		}

		private void CloseCurrentActiveDropdown()
		{
			if (eventSystem.currentSelectedGameObject != null)
			{
				Dropdown componentInParent = eventSystem.currentSelectedGameObject.GetComponentInParent<Dropdown>();
				if (componentInParent != null)
				{
					componentInParent.Hide();
				}
			}
		}

		public void SaveState(bool clearRedo = true, bool skipTimelinePos = false)
		{
			if (!initialized || changingState != 0 || changingFile || savingState || !undoEnabled)
			{
				return;
			}
			RDUtils.StartMeasuringTime();
			savingState = true;
			_ = DateTime.UtcNow;
			string text = EncodeData();
			bool flag = false;
			if (undoStates.Count > 0)
			{
				LevelState levelState = undoStates.Last();
				flag = text.Equals(levelState.data) && levelState.tab == currentTab && levelState.rowsTabIndex == selectedRowsTabPageIndex && levelState.decorationsTabIndex == selectedDecorationsTabPageIndex;
			}
			List<int> list = new List<int>();
			foreach (LevelEventControl_Base selectedControl in selectedControls)
			{
				list.Add(selectedControl.levelEvent.uid);
			}
			if (flag)
			{
				undoStates.Last().selectedIds = list;
			}
			else
			{
				theresChangesToSave = true;
				if (skipPreviewReloadUpdate)
				{
					skipPreviewReloadUpdate = false;
				}
				else
				{
					previewNeedsReload = true;
				}
				undoStates.Add(new LevelState(text, list, currentTab, selectedRowsTabPageIndex, selectedRowIndex, selectedDecorationsTabPageIndex, selectedDecoration, skipTimelinePos ? float.NaN : (0f - base.timeline.scrollviewContent.anchoredPosition.x), skipTimelinePos ? float.NaN : base.timeline.scrollViewVertContent.anchoredPosition.y));
				if (clearRedo)
				{
					redoStates.Clear();
					navigationalStateIndex = 0;
				}
				if (undoStates.Count > 100)
				{
					undoStates.RemoveAt(0);
				}
			}
			savingState = false;
			unsavedIndicatorCheck = true;
		}

		public void ClearStateCache()
		{
			theresChangesToSave = false;
			undoStates.Clear();
			redoStates.Clear();
		}

		private void Undo()
		{
			UndoOrRedo(redo: false);
		}

		private void Redo()
		{
			UndoOrRedo(redo: true);
		}

		private void NavigationalUndo()
		{
			UndoOrRedo(redo: false, navigationalOnly: true);
		}

		private void NavigationalRedo()
		{
			UndoOrRedo(redo: true, navigationalOnly: true);
		}

		private void UndoOrRedo(bool redo, bool navigationalOnly = false)
		{
			if (changingState != 0 || changingFile || !undoEnabled)
			{
				return;
			}
			List<LevelState> list = (redo ? redoStates : undoStates);
			bool flag = false;
			if (!navigationalOnly)
			{
				if (list.Count > 0)
				{
					flag = true;
				}
			}
			else if ((redo && navigationalStateIndex > 0) || (!redo && navigationalStateIndex < redoStates.Count + undoStates.Count - 1))
			{
				flag = true;
			}
			if (flag)
			{
				using (new SaveStateScope(clearRedo: false, navigationalOnly))
				{
					if (navigationalOnly)
					{
						if (redo)
						{
							navigationalStateIndex--;
						}
						else
						{
							navigationalStateIndex++;
						}
					}
					else
					{
						if (!redo)
						{
							redoStates.Add(list.Pop());
						}
						navigationalStateIndex = redoStates.Count();
					}
					string[] array = new string[21];
					colorPalette.CopyTo(array, 0);
					LevelState levelState;
					if (navigationalOnly)
					{
						if (navigationalStateIndex >= redoStates.Count)
						{
							int index = undoStates.Count - (navigationalStateIndex - redoStates.Count + 1);
							levelState = undoStates[index];
						}
						else
						{
							levelState = redoStates[navigationalStateIndex];
						}
					}
					else
					{
						levelState = list.Last();
						DecodeData(levelState.data);
					}
					SelectLevelEventControlsWithIds(levelState.selectedIds);
					ShowTabSection(levelState.tab);
					if (currentTab == Tab.Rows)
					{
						base.tabSection_rows.ChangePage(levelState.rowsTabIndex, calledByUnity: false);
					}
					else if (currentTab == Tab.Decorations)
					{
						base.tabSection_decorations.ChangePage(levelState.decorationsTabIndex, calledByUnity: false);
					}
					int num = 0;
					foreach (List<LevelEventControl_Base> eventControls_row in base.editor.eventControls_rows)
					{
						int room = rowsData[num].room;
						foreach (LevelEventControl_Base item in eventControls_row)
						{
							item.transform.SetParent(base.tabSection_rows.container[room].transform, worldPositionStays: false);
						}
						num++;
					}
					int num2 = 0;
					foreach (List<LevelEventControl_Base> eventControls_decoration in base.editor.eventControls_decorations)
					{
						int room2 = decorationsData[num2].room;
						foreach (LevelEventControl_Base item2 in eventControls_decoration)
						{
							item2.transform.SetParent(base.tabSection_decorations.container[room2].transform, worldPositionStays: false);
						}
						num2++;
					}
					if (levelState.rowSelected >= 0)
					{
						InspectorPanel_MakeRow inspectorPanel_MakeRow = inspectorPanelManager.Get<InspectorPanel_MakeRow>();
						if (inspectorPanel_MakeRow.transform.GetSiblingIndex() + 1 == inspectorPanel_MakeRow.transform.parent.childCount)
						{
							printe("updated make row");
							RowHeader.ShowPanel(levelState.rowSelected);
						}
					}
					else if (levelState.decorationSelected != null)
					{
						InspectorPanel inspectorPanel = null;
						foreach (LevelEvent_MakeDecorationBase decorationsDatum in decorationsData)
						{
							if (decorationsDatum.decorationId == levelState.decorationSelected)
							{
								inspectorPanel = decorationsDatum.inspectorPanel;
							}
						}
						if (inspectorPanel.transform.GetSiblingIndex() + 1 == inspectorPanel.transform.parent.childCount)
						{
							printe("updated make decoration");
							DecorationHeader.ShowPanel(levelState.decorationSelected);
						}
					}
					base.timeline.UnfollowPlayhead();
					if (levelState.timelinePosX != float.NaN)
					{
						base.timeline.ScrollTo(levelState.timelinePosX, 0.3f);
					}
					if (levelState.timelinePosY != float.NaN)
					{
						base.timeline.ScrollToVert(levelState.timelinePosY, 0.3f);
					}
					base.timeline.UpdateSetBeatModifiersCache();
					foreach (List<LevelEventControl_Base> eventControls_row2 in eventControls_rows)
					{
						foreach (LevelEventControl_Base item3 in eventControls_row2)
						{
							item3.UpdateUI();
						}
					}
					if (!colorPalette.SequenceEqual(array))
					{
						foreach (LevelEventControl_Base eventControl in base.editor.eventControls)
						{
							eventControl.UpdateUI();
						}
					}
					if (!navigationalOnly)
					{
						list.Remove(levelState);
					}
					if (rankSettingsPanel.gameObject.activeSelf)
					{
						rankSettingsPanel.GetComponent<InspectorPanel_LevelSettings>().UpdateUI(changeActivePanel: false);
					}
					return;
				}
			}
			printe("nothing to " + (redo ? "redo" : "undo"));
		}

		private void SelectAllSubsequentEvents(bool onlyInTab)
		{
			if (selectedControls.Count == 0)
			{
				return;
			}
			LevelEventControl_Base levelEventControl_Base = selectedControls[0];
			TabSection tabSection = levelEventControl_Base.tabSection;
			bool num = (levelEventControl_Base.tabSection is TabSection_Rows) & onlyInTab;
			bool flag = (levelEventControl_Base.tabSection is TabSection_Decorations) & onlyInTab;
			List<LevelEventControl_Base> list = new List<LevelEventControl_Base>();
			List<LevelEventControl_Base> list2 = (num ? eventControls_rows[levelEventControl_Base.levelEvent.row] : ((!flag) ? eventControls : eventControls_decorations[levelEventControl_Base.levelEvent.row]));
			foreach (LevelEventControl_Base item in list2)
			{
				if (item.bar >= levelEventControl_Base.bar && (item.bar != levelEventControl_Base.bar || !(item.beat < levelEventControl_Base.beat)) && !((item.tabSection != tabSection) & onlyInTab))
				{
					list.Add(item);
				}
			}
			SelectEventControls(list);
		}

		private void SelectAllPrecedingEvents(bool onlyInTab)
		{
			if (selectedControls.Count == 0)
			{
				return;
			}
			LevelEventControl_Base levelEventControl_Base = selectedControls[0];
			TabSection tabSection = levelEventControl_Base.tabSection;
			bool num = (levelEventControl_Base.tabSection is TabSection_Rows) & onlyInTab;
			bool flag = (levelEventControl_Base.tabSection is TabSection_Decorations) & onlyInTab;
			List<LevelEventControl_Base> list = new List<LevelEventControl_Base>();
			List<LevelEventControl_Base> list2 = (num ? eventControls_rows[levelEventControl_Base.levelEvent.row] : ((!flag) ? eventControls : eventControls_decorations[levelEventControl_Base.levelEvent.row]));
			foreach (LevelEventControl_Base item in list2)
			{
				if (item.bar <= levelEventControl_Base.bar && (item.bar != levelEventControl_Base.bar || !(item.beat > levelEventControl_Base.beat)) && !((item.tabSection != tabSection) & onlyInTab))
				{
					list.Add(item);
				}
			}
			SelectEventControls(list);
		}

		private void SelectLevelEventControlsWithIds(List<int> ids)
		{
			List<LevelEventControl_Base> list = new List<LevelEventControl_Base>();
			foreach (int id in ids)
			{
				bool flag = false;
				foreach (LevelEventControl_Base eventControl in eventControls)
				{
					if (eventControl.levelEvent.uid == id)
					{
						list.Add(eventControl);
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					printe("level event with id: " + id + " not found");
				}
			}
			SelectEventControls(list);
		}

		public void OnBeat()
		{
			if (metronomeActive && !float.IsInfinity(base.conductor.crotchet))
			{
				RectTransform component = metronome.GetComponent<RectTransform>();
				component.ScaleX(component.localScale.x * -1f);
				if (metronomeFlashTween != null)
				{
					metronomeFlashTween.Kill();
					metronomeFlashTween = null;
				}
				metronome.color = Color.Lerp(metronomeOnColor, Color.white, 0.7f);
				metronomeFlashTween = metronome.DOColor(metronomeOnColor, base.conductor.crotchet).SetUpdate(isIndependentUpdate: true);
			}
		}

		public void SwitchLanguageButton()
		{
			if (!populatingLanguagesDropdown)
			{
				SystemLanguage language = RDString.AvailableLanguages[languages.value];
				SwitchLanguageAndRestart(language);
			}
		}

		private void SwitchLanguageAndRestart(SystemLanguage language)
		{
			RDString.ChangeLanguage(language);
			scnBase.GoToLevelEditor();
		}

		public void Quit(bool quittingFromWantsToQuit)
		{
			canReceiveInput = false;
			CheckIfNeedsToSaveFile(() =>
			{
				SaveWindowDance();
				scnGame obj = base.game;
				if ((object)obj == null || obj.currentLevel?.data?.usesWindowDance != true)
				{
					Persistence.SetLevelEditorScaleFactor(scaleFactor);
					Persistence.SetLevelEditorTimelineHeight(base.timeline.rowCellCount);
				}
				fadeTransitionImage.raycastTarget = true;
				DoSceneChange(() =>
				{
					RDUtils.SetMixerVolume("LevelMasterVolume", 1f);
					Application.wantsToQuit -= ShowQuitDialog;
					if (GC.editorBuild | quittingFromWantsToQuit)
					{
						forceQuit = true;
						Application.Quit();
					}
					else if (cameFromCustomLevelSelect)
					{
						scnBase.GoToCustomLevelSelect();
					}
					else if (scrVfxControl.CheckForWindowDanceChanges())
					{
						DOVirtual.DelayedCall(RealWindowChoreographer.WindowedSwitchTime + 0.1f, () =>
						{
							scnBase.GoToMainMenu();
						});
					}
					else
					{
						scnBase.GoToMainMenu();
					}
				});
			});
		}

		public void ShowSettings()
		{
			PauseIfUnpaused();
			settingsMenu.Show();
		}

		private void ShowMenuDropdown()
		{
			Text[] array = keyShortcuts;
			foreach (Text text in array)
			{
				if (text != null)
				{
					text.gameObject.SetActive(value: false);
				}
			}
			if (mostRecentFilePaths.Any())
			{
				openRecentSaysShow = false;
				for (int j = 0; j < mostRecentFilePaths.Count; j++)
				{
					SetOpenRecentText(mostRecentFilePaths[j], j);
				}
				openRecentOption.SetActive(mostRecentFilePaths.Count > 1);
			}
			else
			{
				mostRecentFile.text = RDString.Get("editor.noRecentFile");
				openRecentOption.SetActive(value: false);
			}
			for (int k = 0; k < 8; k++)
			{
				olderFileGOs[k].SetActive(k + 1 < mostRecentFilePaths.Count);
			}
			openedMainMenuThisFrame = true;
			mainMenu.SetActive(value: true);
			LevelEditorPlaySound("sndEditorPopupLarge");
		}

		private void HideMenuDropdown()
		{
			if (mainMenu.activeSelf)
			{
				LevelEditorPlaySound("sndEditorPopdownLarge");
			}
			mainMenu.SetActive(value: false);
			recentFilesMenu.SetActive(value: false);
		}

		public void OffsetSelectedEventsByBar(int offset)
		{
			foreach (LevelEventControl_Base selectedControl in selectedControls)
			{
				selectedControl.bar += offset;
				base.timeline.UpdateUI();
			}
		}

		public void ShowOttoHeart()
		{
			if (!isPlaying)
			{
				ottoHeartTime = 0.5f;
				if (RDEditorBase.debugSettings.Auto)
				{
					LevelEditorPlaySound("sndOttoHappy", "LevelEditorOtto");
				}
			}
		}

		public void SetMouseIsOverOtto(bool overOtto)
		{
			mouseIsOverOtto = overOtto;
		}

		public void ShowPopupBlocker(Action onClickAction)
		{
			popupBlocker.gameObject.SetActive(value: true);
			Button component = popupBlocker.GetComponent<Button>();
			component.onClick.RemoveAllListeners();
			component.onClick.AddListener(() =>
			{
				popupBlocker.gameObject.SetActive(value: false);
				onClickAction();
			});
		}

		public ApprovalLevel ApprovalLevelForArtist(string artistName)
		{
			if (EditorWebServices.artists == null)
			{
				return ApprovalLevel.Pending;
			}
			artistName = artistName.ToLower().Trim();
			ArtistData[] artists = EditorWebServices.artists;
			foreach (ArtistData artistData in artists)
			{
				if (artistName == artistData.nameLowercase)
				{
					return artistData.approvalLevel;
				}
			}
			return ApprovalLevel.Pending;
		}

		private void CheckIfEditorIsOlderVersion()
		{
			int version = levelSettings.version;
			int num = 68;
			if (version > num)
			{
				printe($"levelVersion {version}, editorVersion {num}");
				string message = RDString.Get("editor.dialog.levelWasSavedOnNewerVersion");
				dialog.Show(message, new string[1] { RDString.Get("editor.PublishLevel.dialogOK") }, (int option) =>
				{
					base.editor.dialog.Hide();
				});
			}
		}

		public void OpenKlyzxTutorialWebsite()
		{
			Application.OpenURL(RDString.isChinese ? "https://rd.rdlevel.cn/" : "https://rd-editor-docs.github.io/intro/");
		}

		public void OnAdvancedOttoClick()
		{
			if (!Persistence.GetAdvancedEditorFeatures())
			{
				ottoClicks++;
				if (ottoClicks < 7)
				{
					LevelEditorPlaySound("sndButtonClick");
					return;
				}
				dialog.Show(RDString.Get("editor.ConfirmAdvancedFeatures"), new string[2]
				{
					RDString.Get("editor.ok"),
					RDString.Get("editor.dialog.cancel")
				}, (int x) =>
				{
					base.editor.dialog.Hide();
					if (x != 1)
					{
						Persistence.SetAdvancedEditorFeatures(enabled: true);
						LevelEditorPlaySound("sndOttoHappy");
						DeselectEverything(sound: false);
						CheckAdvancedFeatures();
						inspectorPanelManager.Get<InspectorPanel_MakeRow>().ToggleLengthPicker(active: true);
					}
				});
			}
			else
			{
				Persistence.SetAdvancedEditorFeatures(enabled: false);
				LevelEditorPlaySound("sndOttoDeactivate");
				DeselectEverything(sound: false);
				CheckAdvancedFeatures();
				ottoClicks = 0;
			}
		}

		public void CheckAdvancedFeatures()
		{
			bool advancedEditorFeatures = Persistence.GetAdvancedEditorFeatures();
			advancedOttoIcon.sprite = (advancedEditorFeatures ? autoplayHeartSprite : autoplayOnSprite);
			inspectorPanelManager.Get<InspectorPanel_MakeRow>().ToggleLengthPicker(advancedEditorFeatures);
		}

		public void FetchLocalization()
		{
			RDString.FetchLocalization(RDString.LocalizationSheet.Strings);
			scnBase.GoToLevelEditor();
		}

		public void FetchInkLocalization()
		{
			RDString.FetchLocalization(RDString.LocalizationSheet.Dialogues);
			scnBase.GoToLevelEditor();
		}

		public void RunAsStandaloneCustomLevel()
		{
			if (!SaveFile())
			{
				scnGame.ClearLevelPersistence();
				scnBase.currentLevelSelect = "scnEditor";
				customLevelPath = openedFilePath;
				scnBase.GoToLevel(openedFilePath);
			}
		}

		public void DoSceneChange(Action action)
		{
			canReceiveInput = false;
			fadeTransitionImage.DOKill();
			fadeTransitionImage.DOFade(1f, 1f).SetEase(Ease.OutQuint).SetUpdate(isIndependentUpdate: true)
				.OnComplete(() =>
				{
					action();
				});
		}
	}
}
