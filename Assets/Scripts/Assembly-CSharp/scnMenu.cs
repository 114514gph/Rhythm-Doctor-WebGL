using System;
using System.Collections.Generic;
using System.IO;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class scnMenu : scnBase
{
	private enum OpenSlotOption
	{
		SelectSlot = 0,
		EraseSlot = 1,
		CopySlot = 2
	}

	private enum MenuSection
	{
		TitleScreen = 0,
		MainMenu = 1,
		SlotMenu = 2,
		Credits = 3
	}

	public static bool doBarTransitionOnStart;

	private const float songBPM = 82f;

	private const string songName = "sndIntro2";

	private const float slotOptionsPosY_visible = 42f;

	private const float slotOptionsPosY_hidden = -10f;

	private const float minWardX = 750f;

	private const float maxWardX = 1860f;

	private const float steamDeckOptionsTextOffsetY = -15f;

	private const float windowsXPAOptionsTextOffsetY = -20f;

	private const float consoleOptionsTextOffsetY = -25f;

	private float optionsTextOffsetY;

	private readonly float[] copySlotPositionsX = new float[2] { -70f, 70f };

	private readonly float[] logoPositionsX = new float[3] { 0f, -56f, -80f };

	private const float MainCameraStartXPos = 176f;

	private readonly KeyCode[] titleScreenKeys = new KeyCode[0];

	private readonly KeyCode[] mainMenuKeys = new KeyCode[2]
	{
		KeyCode.Space,
		KeyCode.Escape
	};

	[Header("UI")]
	public Text gamerTagText;

	public RectTransform mainMenu;

	public RectTransform logoRectTransform;

	public Logo logo;

	public Text logoInstructions;

	public RectTransform options;

	public RectTransform optionsContainer;

	public RectTransform mainLeftArrow;

	public RectTransform mainRightArrow;

	public AudioSource song;

	public RectTransform slotMenu;

	public Text title;

	public Image leftNote;

	public Image rightNote;

	public Text slotInstructions;

	public RectTransform slotOptions;

	public Text erase;

	public Text copy;

	public RectTransform slotLeftArrow;

	public RectTransform slotRightArrow;

	public Text eraseMessage;

	public Text eraseCancel;

	public Text eraseConfirm;

	public SlotUI[] slots;

	public Image overlay;

	public PauseMenu settingsMenu;

	public BarTransition barTransition;

	[Header("Other")]
	public Transform mainWard;

	public Transform basement;

	public GameObject prefabTestPixel;

	public Camera mainCamera;

	public GameObject fanArtLoaderPrefab;

	public RedeemCode redeemCode;

	[Header("Sprites")]
	public Sprite[] titleNoteFrames;

	public CreditsScroller[] creditsPages;

	public RectTransform creditsTransform;

	[Header("Variables")]
	public float optionsOffset;

	public Color optionHighlightColor;

	public float wardScrollingSpeed = 1f;

	private Text[] optionsText;

	private MenuSection section;

	private int currentOption = -1;

	private int currentSlotIndex;

	private bool slotOpen;

	private float lastBeatTime = -100f;

	private bool onBeatRecentlyCalled;

	private bool transitioningToAnotherScene;

	private bool changingSection;

	private OpenSlotOption openSlotSelectedOption;

	private bool selectingSlotToCopy;

	private bool confirmingSlotToErase;

	private int confirmUIOption;

	private int copySlotIndex;

	private bool showingPauseMenu;

	private int currentCreditPageIndex;

	private float ratioDistance;

	private bool showingSettings;

	private int lastScreenWidth;

	private int lastScreenHeight;

	private bool saidFirstText;

	private bool justCopiedSlot;

	private bool justErasedSlot;

	private int resolutionChangedFrame;

	private bool firstTimeUpdatingRatioDistances = true;

	private bool debug_showTestPixels;

	private List<RectTransform> testPixels = new List<RectTransform>();

	public new static scnMenu instance => scnBase._instance as scnMenu;

	private float canvasHeight => GetComponent<RectTransform>().sizeDelta.y;

	private new void Awake()
	{
		base.Awake();
		scnBase._instance = this;
		muteWhenGameUnfocused = true;
		SpeedrunValues.inGame = false;
		mainMenu.gameObject.SetActive(value: true);
		slotMenu.gameObject.SetActive(value: true);
		ShowSlotOptions(on: false, animated: false);
		optionsTextOffsetY = (scnBase.isSteamDeck ? (-15f) : (scnBase.isConsole ? (-25f) : (scnBase.isMicrosoft ? (-20f) : 0f)));
		optionsContainer.offsetMax = new Vector2(optionsContainer.offsetMax.x, optionsTextOffsetY);
		new List<string>();
		List<Text> list = new List<Text>();
		bool flag = false;
		for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
		{
			if (!(Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i)) != "scnExpoMenu"))
			{
				flag = true;
				break;
			}
		}
		foreach (RectTransform item in optionsContainer)
		{
			GameObject gameObject = item.gameObject;
			string text = gameObject.name;
			if ((text == "levelEditor" && (scnBase.isConsole || scnBase.isMicrosoft || scnBase.isSwitch)) || (text == "customLevels" && (scnBase.isXbox || scnBase.isMicrosoft || scnBase.isSwitch)) || (text == "music" && (scnBase.isConsole || scnBase.isMicrosoft || scnBase.isSwitch)) || (text == "rhythmWeightlifter" && !flag) || (text == "expoMode" && !flag) || (text == "quit" && (scnBase.isXbox || scnBase.isSwitch)))
			{
				gameObject.SetActive(value: false);
				continue;
			}
			Text component = item.GetComponent<Text>();
			list.Add(component);
		}
		optionsText = list.ToArray();
		UpdateRatioDistances();
		logoRectTransform.AnchorPosX(logoPositionsX[0]);
		float num = (float)Screen.width * 1f / (float)Screen.height * canvasHeight;
		options.AnchorPosX(num);
		Localize();
		HighlightSlot(0, sound: false);
		settingsMenu.Initialize();
	}

	private new void Start()
	{
		base.Start();
		RDBase.Vfx.hudCameraOverlay.gameObject.SetActive(value: false);
		base.conductor.SetBPM(82f);
		base.conductor.crotchetsPerBar = 4;
		song = base.conductor.PlaySong("sndIntro2", RDUtils.GetMixerGroup("MenuMusic"));
		PauseMenu pauseMenu = settingsMenu;
		pauseMenu.toggleVisibility = (PauseMenu.ToggleVisibility)Delegate.Combine(pauseMenu.toggleVisibility, new PauseMenu.ToggleVisibility(OnToggleSettings));
		if (doBarTransitionOnStart)
		{
			doBarTransitionOnStart = false;
			barTransition.Hide();
		}
		RDRichPresence.Instance?.UpdatePresence();
		KeyInfoPanel.instance.UpdateShortcutsRow(titleScreenKeys);
		mainWard.GetComponent<RDMainWard>().EnableBackgroundCharacters();
	}

	public override void Localize()
	{
		Text[] array = new Text[8] { logoInstructions, title, copy, erase, eraseMessage, eraseCancel, eraseConfirm, slotInstructions };
		for (int i = 0; i < array.Length; i++)
		{
			array[i].font = RDString.currentLanguageFont;
		}
		SlotUI[] array2 = slots;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].Localize();
		}
		foreach (RectTransform item in optionsContainer)
		{
			item.GetComponent<Text>().font = RDString.currentLanguageFont;
		}
		KeyInfoPanel keyInfoPanel = KeyInfoPanel.instance;
		if (keyInfoPanel != null)
		{
			KeyHelper[] shortcutKeyHelpers = keyInfoPanel.shortcutKeyHelpers;
			foreach (KeyHelper obj in shortcutKeyHelpers)
			{
				obj.gameObject.SetActive(value: false);
				obj.enabledOnce = false;
			}
			keyInfoPanel.UpdateShortcutsRow(mainMenuKeys);
		}
		List<Text> list = new List<Text>();
		array = optionsText;
		foreach (Text text in array)
		{
			if (text.gameObject.activeSelf)
			{
				list.Add(text);
			}
		}
		optionsText = list.ToArray();
		int num = 0;
		array = optionsText;
		foreach (Text text2 in array)
		{
			text2.rectTransform.anchoredPosition = text2.rectTransform.anchoredPosition.WithY(-15 * num);
			num++;
			if (text2.name == "rhythmWeightlifter")
			{
				text2.text = RDString.Get("levelSelect.RhythmWeightlifter");
				continue;
			}
			string key = "mainMenu." + text2.name;
			text2.text = RDString.Get(key);
		}
		logoInstructions.text = RDString.Get("mainMenu.pressAnyButton");
		title.text = RDString.Get("mainMenu.storyMode");
		copy.text = RDString.Get("slotMenu.copy");
		erase.text = RDString.Get("slotMenu.erase");
		eraseMessage.text = RDString.Get("slotMenu.eraseMessage");
		eraseCancel.text = RDString.Get("slotMenu.eraseCancel");
		eraseConfirm.text = RDString.Get("slotMenu.eraseConfirm");
		slotInstructions.text = RDString.Get("slotMenu.instructions.selectSlot");
		if (currentOption >= 0)
		{
			Text text3 = optionsText[currentOption];
			FocusArrowsOnText(mainLeftArrow, mainRightArrow, text3, animated: false);
		}
		PopulateText(eraseCancel);
		PopulateText(title);
		FocusArrowsOnText(leftNote.rectTransform, rightNote.rectTransform, title, animated: false, 12f);
	}

	private void PopulateText(Text text)
	{
		TextGenerationSettings generationSettings = text.GetGenerationSettings(text.rectTransform.sizeDelta);
		text.cachedTextGenerator.Populate(text.text, generationSettings);
	}

	private void ShowSlotOptions(bool on, bool animated)
	{
		float endValue = (on ? 42f : (-10f));
		float duration = ((!animated) ? 0f : (on ? 0.4f : 0.2f));
		slotOptions.DOKill();
		slotOptions.DOAnchorPosY(endValue, duration).SetEase(on ? Ease.OutExpo : Ease.InQuint);
	}

	public override void OnPreBar()
	{
		base.OnPreBar();
		if (!transitioningToAnotherScene && (base.conductor.barNumber - 1) % 17 == 0 && base.conductor.barNumber > 3)
		{
			song = base.conductor.PlaySong("sndIntro2", RDUtils.GetMixerGroup("MenuMusic"));
		}
	}

	private void OnValidate()
	{
		UpdateOptionsPositions();
	}

	private void UpdateOptionsPositions()
	{
		int num = 0;
		foreach (RectTransform item in optionsContainer)
		{
			item.anchoredPosition = item.anchoredPosition.WithY(optionsOffset * (float)num);
			num++;
		}
	}

	private void UpdateRatioDistances()
	{
		int width = Screen.width;
		int height = Screen.height;
		if (!firstTimeUpdatingRatioDistances && lastScreenWidth == width && lastScreenHeight == height)
		{
			return;
		}
		firstTimeUpdatingRatioDistances = false;
		if (resolutionChangedFrame < Time.frameCount)
		{
			resolutionChangedFrame = Time.frameCount + 1;
			return;
		}
		lastScreenWidth = width;
		lastScreenHeight = height;
		ratioDistance = (float)width * 1f / (float)height * canvasHeight;
		settingsMenu.phoneHidePos.x = ratioDistance;
		settingsMenu.levelDetailHidePos.x = ratioDistance + 172f;
		if (showingSettings)
		{
			mainMenu.DOKill();
			mainMenu.AnchorPosX(0f - ratioDistance);
		}
		else
		{
			settingsMenu.phoneRect.DOKill();
			settingsMenu.phoneRect.AnchorPosX(ratioDistance);
			settingsMenu.levelDetailContainer.DOKill();
			settingsMenu.levelDetailContainer.AnchorPosX(settingsMenu.levelDetailHidePos.x);
			if (section == MenuSection.TitleScreen || section == MenuSection.MainMenu)
			{
				creditsTransform.DOKill();
				creditsTransform.AnchorPosX(0f - ratioDistance);
				CreditsScroller[] array = creditsPages;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].gameObject.SetActive(value: false);
				}
				creditsTransform.gameObject.SetActive(value: false);
			}
			else if (section == MenuSection.Credits)
			{
				changingSection = false;
				mainMenu.DOKill();
				mainMenu.AnchorPosX(ratioDistance);
			}
		}
		mainWard.LocalMoveY(canvasHeight / 2f);
		basement.LocalMoveY((0f - canvasHeight) / 2f);
		float num = ((section == MenuSection.SlotMenu) ? ((0f - canvasHeight) / 2f) : (canvasHeight / 2f));
		mainCamera.transform.LocalMoveY(num);
		mainMenu.anchoredPosition = mainMenu.anchoredPosition.WithY(0f);
		slotMenu.anchoredPosition = slotMenu.anchoredPosition.WithY(0f - canvasHeight);
	}

	private new void Update()
	{
		UpdateRatioDistances();
		base.Update();
		bool flag = currentOption == -1;
		if (flag)
		{
			HighlightOption(0, animated: false, sound: false);
		}
		else if (!saidFirstText)
		{
			Narration.Say(RDString.Get("narration.titleScreen").Replace("[pressAnyKey]", logoInstructions.text), NarrationCategory.Navigation, false);
			saidFirstText = true;
			Narration.SetInstructions(logoInstructions.text, NarrationActionName.ForContextInstructions, NarrationActionName.ToRepeatLastMessage, NarrationActionName.ToToggleNarration);
		}
		float num = base.conductor.crotchet * 0.25f;
		if (onBeatRecentlyCalled && Time.unscaledTime - lastBeatTime > num)
		{
			OnBeatInternal(on: false);
		}
		mainWard.TranslateX(wardScrollingSpeed * Time.unscaledDeltaTime);
		float num2 = mainWard.localPosition.x;
		if (num2 < 750f || num2 > 1860f)
		{
			wardScrollingSpeed *= -1f;
			mainWard.MoveX(Mathf.Clamp(num2, 750f, 1860f));
			mainWard.TranslateX(wardScrollingSpeed * Time.unscaledDeltaTime);
		}
		if (redeemCode.isEnabled || ((transitioningToAnotherScene || showingPauseMenu || changingSection) | flag))
		{
			return;
		}
		bool leftPress = RDInput.leftPress;
		bool rightPress = RDInput.rightPress;
		bool upPress = RDInput.upPress;
		bool downPress = RDInput.downPress;
		bool anyPlayerPress = RDInput.anyPlayerPress;
		bool cancelPress = RDInput.cancelPress;
		if (section == MenuSection.TitleScreen)
		{
			if (anyPlayerPress || RDInput.anyKeyDown)
			{
				GoToSection(MenuSection.MainMenu);
				PlayButtonSound("MainMenu", RDUtils.PitchSemitonesToPercent(-3f));
			}
		}
		else if (section == MenuSection.MainMenu)
		{
			if (downPress)
			{
				HighlightOption(currentOption + 1);
			}
			else if (upPress)
			{
				HighlightOption(currentOption - 1);
			}
			else if (anyPlayerPress)
			{
				SelectOption();
			}
			else if (cancelPress)
			{
				FocusArrowsOnText(mainLeftArrow, mainRightArrow, optionsText[currentOption], animated: false);
				GoToSection(MenuSection.TitleScreen);
				PlayHammerSound("MainMenu", RDUtils.PitchSemitonesToPercent(-4f));
			}
		}
		else if (section == MenuSection.Credits)
		{
			if (cancelPress)
			{
				GoToSection(MenuSection.MainMenu);
			}
			else if (creditsPages[currentCreditPageIndex].finished)
			{
				if (currentCreditPageIndex < creditsPages.Length - 1)
				{
					currentCreditPageIndex++;
					creditsPages[currentCreditPageIndex].gameObject.SetActive(value: true);
				}
				else
				{
					currentCreditPageIndex = 0;
					GoToSection(MenuSection.MainMenu);
				}
			}
		}
		else
		{
			if (section != MenuSection.SlotMenu)
			{
				return;
			}
			if (!slotOpen)
			{
				if (rightPress)
				{
					HighlightSlot(currentSlotIndex + 1);
				}
				else if (leftPress)
				{
					HighlightSlot(currentSlotIndex - 1);
				}
				else if (cancelPress)
				{
					PlayHammerSound("MainMenu");
					GoToSection(MenuSection.MainMenu);
				}
				else if (anyPlayerPress)
				{
					PlayButtonSound("MainMenu", RDUtils.PitchSemitonesToPercent(3f));
					slotOpen = true;
					ShowCurrentSlotAsSelected();
				}
			}
			else if (cancelPress)
			{
				if (openSlotSelectedOption == OpenSlotOption.CopySlot && selectingSlotToCopy)
				{
					PlayHammerSound("MainMenu", RDUtils.PitchSemitonesToPercent(5f));
					ShowCurrentSlotAsSelected();
					slotInstructions.text = RDString.Get("slotMenu.instructions.copy");
					selectingSlotToCopy = false;
				}
				else if (openSlotSelectedOption == OpenSlotOption.EraseSlot && confirmingSlotToErase)
				{
					PlayHammerSound("MainMenu", RDUtils.PitchSemitonesToPercent(5f));
					ShowEraseConfirmUI(on: false);
				}
				else
				{
					PlayHammerSound("MainMenu", RDUtils.PitchSemitonesToPercent(3f));
					slotOpen = false;
					slots[currentSlotIndex].SetOpen(on: false);
					ShowSlotOptions(on: false, animated: true);
					slotInstructions.text = RDString.Get("slotMenu.instructions.selectSlot");
					MoveSlotsToOriginalPositions();
				}
			}
			else if (anyPlayerPress)
			{
				if (openSlotSelectedOption == OpenSlotOption.SelectSlot)
				{
					Persistence.currentSlotIndex = currentSlotIndex;
					SpeedrunValues.inGame = true;
					slots[currentSlotIndex].PressButton();
					scnBase.currentLevelSelect = "scnLevelSelect";
					bool didCalibration = Persistence.GetDidCalibration();
					bool firstTimePlaying = Persistence.GetFirstTimePlaying();
					if (!didCalibration)
					{
						scnCalibration.nextSceneName = (firstTimePlaying ? string.Empty : "scnLevelSelect");
						TransitionToScene("scnCalibration");
					}
					else
					{
						string scene = (firstTimePlaying ? "Intro" : "scnLevelSelect");
						TransitionToScene(scene);
					}
				}
				else if (openSlotSelectedOption == OpenSlotOption.CopySlot)
				{
					if (!selectingSlotToCopy)
					{
						PlayButtonSound("MainMenu", RDUtils.PitchSemitonesToPercent(5f));
						selectingSlotToCopy = true;
						copySlotIndex = GetCopySlotIndex(-1, 1);
						_ = slots[currentSlotIndex];
						MoveSlotsToCopyPositions();
						slots[currentSlotIndex].SetOpen(on: false);
						MoveCurrentSlotOnTopOfCopySlot();
						ShowSlotOptions(on: false, animated: true);
					}
					else
					{
						PlayButtonSound("MainMenu", RDUtils.PitchSemitonesToPercent(7f));
						Persistence.Copy(currentSlotIndex, copySlotIndex, copySleeveSprites: true);
						justCopiedSlot = true;
						MoveSlotsToOriginalPositions();
						slotOpen = false;
						selectingSlotToCopy = false;
						SlotUI obj = slots[copySlotIndex];
						obj.Flash();
						obj.LoadSlotData();
						slotInstructions.text = RDString.Get("slotMenu.instructions.selectSlot");
					}
				}
				else if (openSlotSelectedOption == OpenSlotOption.EraseSlot)
				{
					if (!confirmingSlotToErase)
					{
						PlayButtonSound("MainMenu", RDUtils.PitchSemitonesToPercent(5f));
						RDBase.PlaySound("sndLevelDetailsDeleteWarning", 1f, RDUtils.GetMixerGroup("MainMenu"));
						ShowEraseConfirmUI(on: true);
						return;
					}
					if (confirmUIOption == 0)
					{
						PlayButtonSound("MainMenu", RDUtils.PitchSemitonesToPercent(7f));
						ShowEraseConfirmUI(on: false);
						return;
					}
					Persistence.DeleteSlotData(currentSlotIndex);
					SlotUI obj2 = slots[currentSlotIndex];
					obj2.Flash();
					obj2.LoadSlotData();
					ShowEraseConfirmUI(on: false);
					justErasedSlot = true;
					SelectOpenSlotOption(OpenSlotOption.SelectSlot);
					RDBase.PlaySound("sndMistakeBig", 1f, RDUtils.GetMixerGroup("MainMenu"));
					PlayHammerSound("MainMenu", RDUtils.PitchSemitonesToPercent(-4f));
				}
			}
			else if ((!selectingSlotToCopy && !confirmingSlotToErase) & downPress)
			{
				if (openSlotSelectedOption == OpenSlotOption.SelectSlot)
				{
					PlayValueSound("MainMenu");
					SelectOpenSlotOption(OpenSlotOption.CopySlot);
				}
			}
			else if ((!selectingSlotToCopy && !confirmingSlotToErase) & upPress)
			{
				if (openSlotSelectedOption != OpenSlotOption.SelectSlot)
				{
					PlayCursorSound("MainMenu");
					SelectOpenSlotOption(OpenSlotOption.SelectSlot);
				}
			}
			else if (leftPress)
			{
				if (openSlotSelectedOption == OpenSlotOption.EraseSlot)
				{
					if (!confirmingSlotToErase)
					{
						PlayValueSound("MainMenu");
						SelectOpenSlotOption(OpenSlotOption.CopySlot);
						return;
					}
					if (confirmUIOption != 0)
					{
						PlayValueSound("MainMenu");
						confirmUIOption = 0;
						Narration.Say(eraseCancel.text, NarrationCategory.Navigation, false, NarrationActionName.ToConfirm, NarrationActionName.ToEraseSlot, NarrationActionName.ToGoBack);
					}
					else
					{
						PlayBorderSound("MainMenu");
					}
					FocusArrowsOnText(slotLeftArrow, slotRightArrow, eraseCancel, animated: true);
				}
				else if (openSlotSelectedOption == OpenSlotOption.CopySlot)
				{
					if (selectingSlotToCopy)
					{
						copySlotIndex = GetCopySlotIndex(copySlotIndex, -1);
						PlayCursorSound("MainMenu", RDUtils.PitchSemitonesToPercent(4f - 10f * (float)copySlotIndex / (float)(slots.Length - 1)));
						MoveCurrentSlotOnTopOfCopySlot();
					}
					else
					{
						PlayBorderSound("MainMenu");
					}
				}
			}
			else
			{
				if (!rightPress)
				{
					return;
				}
				if (openSlotSelectedOption == OpenSlotOption.CopySlot)
				{
					if (selectingSlotToCopy)
					{
						copySlotIndex = GetCopySlotIndex(copySlotIndex, 1);
						PlayCursorSound("MainMenu", RDUtils.PitchSemitonesToPercent(4f - 10f * (float)copySlotIndex / (float)(slots.Length - 1)));
						_ = slots[currentSlotIndex];
						_ = slots[copySlotIndex];
						MoveCurrentSlotOnTopOfCopySlot();
					}
					else
					{
						RDBase.PlaySound("sndLevelDetailsDeleteHover", 1f, RDUtils.GetMixerGroup("MainMenu"));
						SelectOpenSlotOption(OpenSlotOption.EraseSlot);
					}
				}
				else
				{
					if (openSlotSelectedOption != OpenSlotOption.EraseSlot)
					{
						return;
					}
					if (confirmingSlotToErase)
					{
						if (confirmUIOption != 1)
						{
							RDBase.PlaySound("sndLevelDetailsDeleteHover", 1f, RDUtils.GetMixerGroup("MainMenu"));
							confirmUIOption = 1;
							Narration.Say(eraseConfirm.text, NarrationCategory.Navigation, false, NarrationActionName.ToConfirm, NarrationActionName.ToGoBack);
						}
						else
						{
							PlayBorderSound("MainMenu");
						}
						FocusArrowsOnText(slotLeftArrow, slotRightArrow, eraseConfirm, animated: true);
					}
					else
					{
						PlayBorderSound("MainMenu");
					}
				}
			}
		}
		int GetCopySlotIndex(int index, int direction)
		{
			int num3 = CycleInt(index + direction, slots.Length);
			if (currentSlotIndex != num3)
			{
				return num3;
			}
			return CycleInt(num3 + direction, slots.Length);
		}
	}

	private void ShowEraseConfirmUI(bool on)
	{
		confirmingSlotToErase = on;
		copy.enabled = !on;
		erase.enabled = !on;
		eraseMessage.enabled = on;
		eraseCancel.enabled = on;
		eraseConfirm.enabled = on;
		confirmUIOption = 0;
		Text text = (on ? eraseCancel : erase);
		FocusArrowsOnText(slotLeftArrow, slotRightArrow, text, animated: false);
		if (on)
		{
			Narration.Set(eraseMessage.text, -1, 2, ElementType.HorizontalList);
			Narration.Say(RDString.Get("slotMenu.eraseCancel"), NarrationCategory.Navigation, false, NarrationActionName.ToCancelSpace, NarrationActionName.ToEraseSlot, NarrationActionName.ToGoBack);
		}
		else
		{
			string text2 = RDString.Get("narration.slotAction");
			string newValue = RDString.Get("slotMenu.erase");
			string speech = text2.Replace("[slotAction]", newValue).Replace("[actionDescription]", slotInstructions.text);
			Narration.Set(RDString.Get("narration.copyOrErase"), ElementType.HorizontalList, 2);
			Narration.Say(speech, NarrationCategory.Navigation, false, NarrationActionName.ToConfirm, NarrationActionName.ToNavigateHorizontal, NarrationActionName.ToCancelErase, NarrationActionName.ToGoBack);
		}
	}

	private void ShowCurrentSlotAsSelected()
	{
		for (int i = 0; i < slots.Length; i++)
		{
			SlotUI slotUI = slots[i];
			float duration = 0.28f;
			float num = 0f;
			if (i < currentSlotIndex)
			{
				num = slotUI.originalPosX - 400f;
			}
			else if (i > currentSlotIndex)
			{
				num = slotUI.originalPosX + 400f;
			}
			Vector2 endValue = new Vector2(num, 0f);
			slotUI.rectTransform.DOKill();
			slotUI.rectTransform.DOAnchorPos(endValue, duration).SetEase(Ease.InQuad);
		}
		slots[currentSlotIndex].SetOpen(on: true);
		SelectOpenSlotOption(OpenSlotOption.SelectSlot);
		slotInstructions.text = RDString.Get("slotMenu.instructions.selectConfirm");
		ShowSlotOptions(on: true, animated: true);
	}

	private void MoveSlotsToOriginalPositions()
	{
		for (int i = 0; i < slots.Length; i++)
		{
			SlotUI slotUI = slots[i];
			float duration = 0.4f;
			slotUI.moveTween = slotUI.rectTransform.DOAnchorPos(new Vector2(slotUI.originalPosX, 0f), duration).SetEase(Ease.OutQuad);
		}
		string actionSuccess = string.Empty;
		if (justCopiedSlot)
		{
			justCopiedSlot = false;
			actionSuccess = RDString.Get("narration.copySuccess");
		}
		NarrateSlot(includeSlotSelection: true, actionSuccess);
	}

	private void MoveSlotsToCopyPositions()
	{
		int num = 0;
		for (int i = 0; i < slots.Length; i++)
		{
			if (i != currentSlotIndex)
			{
				SlotUI obj = slots[i];
				float duration = 0.4f;
				float endValue = copySlotPositionsX[num];
				RDBaseDllDummy.printem(Ease.OutQuad);
				obj.moveTween = obj.rectTransform.DOAnchorPosX(endValue, duration).SetEase(Ease.OutQuad);
				num++;
			}
		}
	}

	private void MoveCurrentSlotOnTopOfCopySlot()
	{
		Vector2 vector = new Vector2(15f, -15f);
		SlotUI obj = slots[copySlotIndex];
		int num = ((copySlotIndex < currentSlotIndex) ? copySlotIndex : (copySlotIndex - 1));
		Vector2 endValue = VectorExtensions.WithX(x: copySlotPositionsX[num], v: obj.rectTransform.anchoredPosition) + vector;
		SlotUI obj2 = slots[currentSlotIndex];
		RectTransform rectTransform = obj2.rectTransform;
		obj2.moveTween.Kill();
		obj2.moveTween = rectTransform.DOAnchorPos(endValue, 0.1f).SetEase(Ease.OutCubic);
		rectTransform.SetAsLastSibling();
		int num2 = currentSlotIndex + 1;
		int num3 = copySlotIndex + 1;
		slotInstructions.text = RDString.Get("slotMenu.instructions.copyConfirm").Replace("[number1]", num2.ToString()).Replace("[number2]", num3.ToString());
		Narration.Say(slotInstructions.text + RDString.Get("narration.shortPause") + RDString.Get("slotMenu.instructions.selectConfirm"), NarrationCategory.Navigation, false, NarrationActionName.ToNavigateHorizontal, NarrationActionName.ToGoBack);
	}

	private void SelectOpenSlotOption(OpenSlotOption newOption)
	{
		OpenSlotOption openSlotOption = openSlotSelectedOption;
		openSlotSelectedOption = newOption;
		Text text = null;
		switch (newOption)
		{
		case OpenSlotOption.CopySlot:
			text = copy;
			break;
		case OpenSlotOption.EraseSlot:
			text = erase;
			break;
		}
		if ((bool)text)
		{
			FocusArrowsOnText(slotLeftArrow, slotRightArrow, text, animated: true);
		}
		else
		{
			slotLeftArrow.anchoredPosition = new Vector2(-20f, 40f);
			slotRightArrow.anchoredPosition = new Vector2(20f, 40f);
		}
		if (openSlotOption != newOption)
		{
			bool flag = newOption == OpenSlotOption.SelectSlot;
			slotLeftArrow.gameObject.SetActive(!flag);
			slotRightArrow.gameObject.SetActive(!flag);
			float alpha = ((newOption == OpenSlotOption.SelectSlot) ? 1f : 0.3f);
			Image outline = slots[currentSlotIndex].outline;
			outline.color = outline.color.WithAlpha(alpha);
		}
		string key = "";
		switch (newOption)
		{
		case OpenSlotOption.CopySlot:
			key = "slotMenu.instructions.copy";
			break;
		case OpenSlotOption.SelectSlot:
			key = "slotMenu.instructions.selectConfirm";
			break;
		case OpenSlotOption.EraseSlot:
			key = "slotMenu.instructions.erase";
			break;
		}
		slotInstructions.text = RDString.Get(key);
		if (newOption == OpenSlotOption.SelectSlot)
		{
			string newValue = (justErasedSlot ? RDString.Get("narration.eraseSuccess") : string.Empty);
			justErasedSlot = false;
			Narration.Say(RDString.Get("narration.slotSelected").Replace("[actionSuccess]", newValue).Replace("[index]", (currentSlotIndex + 1).ToString())
				.Replace("[pressToConfirm]", slotInstructions.text), NarrationCategory.Navigation, false, NarrationActionName.ToCopyOrEraseSaveFile, NarrationActionName.ToGoBack);
			return;
		}
		if (openSlotOption == OpenSlotOption.SelectSlot)
		{
			Narration.Set(RDString.Get("narration.copyOrErase"), ElementType.HorizontalList, 2);
		}
		string text2 = RDString.Get("narration.slotAction");
		string newValue2 = ((newOption == OpenSlotOption.CopySlot) ? RDString.Get("slotMenu.copy") : RDString.Get("slotMenu.erase"));
		string speech = text2.Replace("[slotAction]", newValue2).Replace("[actionDescription]", slotInstructions.text);
		NarrationActionName narrationActionName = ((newOption == OpenSlotOption.CopySlot) ? NarrationActionName.ToCancelCopy : NarrationActionName.ToCancelErase);
		NarrationActionName[] obj = new NarrationActionName[4]
		{
			NarrationActionName.ToConfirm,
			NarrationActionName.ToNavigateHorizontal,
			(NarrationActionName)0,
			NarrationActionName.ToGoBack
		};
		obj[2] = narrationActionName;
		Narration.Say(speech, NarrationCategory.Navigation, flipCategoryQueueBehaviour: false, obj);
	}

	private void OnToggleSettings(bool show)
	{
		showingSettings = show;
		logo.Rewind();
		logo.playSounds = false;
		float endValue = (showingSettings ? (0f - ratioDistance) : 0f);
		mainMenu.DOKill();
		mainMenu.DOAnchorPosX(endValue, settingsMenu.toggleAnimDuration).SetUpdate(isIndependentUpdate: true).SetEase(Ease.OutExpo);
		float endValue2 = (showingSettings ? (176f + ratioDistance) : 176f);
		mainCamera.transform.DOKill();
		mainCamera.transform.DOMoveX(endValue2, settingsMenu.toggleAnimDuration).SetUpdate(isIndependentUpdate: true).SetEase(Ease.OutExpo)
			.OnComplete(() =>
			{
				if (!showingSettings)
				{
					mainLeftArrow.gameObject.SetActive(value: true);
					mainRightArrow.gameObject.SetActive(value: true);
					Text text = optionsText[currentOption];
					FocusArrowsOnText(mainLeftArrow, mainRightArrow, text, animated: false);
					showingPauseMenu = false;
					Narration.Set(RDString.Get("narration.mainMenu"), -1, optionsText.Length, ElementType.VerticalList, null, RDString.Get("narration.pressF1"));
					Narration.Say(optionsText[currentOption].text, NarrationCategory.Navigation, false, NarrationActionName.ToNavigateVertical, NarrationActionName.ToSelect, NarrationActionName.ToGoBack, NarrationActionName.ForContextInstructions, NarrationActionName.ToRepeatLastMessage, NarrationActionName.ToToggleNarration);
				}
			});
		if (showingSettings)
		{
			mainLeftArrow.gameObject.SetActive(value: false);
			mainRightArrow.gameObject.SetActive(value: false);
			showingPauseMenu = true;
		}
	}

	private void HighlightOption(int index, bool animated = true, bool sound = true)
	{
		int num = CycleInt(index, optionsText.Length);
		Text text = optionsText[num];
		if (text.cachedTextGenerator.characters.Count == 0)
		{
			return;
		}
		currentOption = num;
		if (sound)
		{
			PlayCursorSound("MainMenu", RDUtils.PitchSemitonesToPercent(4f - 10f * (float)currentOption / (float)(optionsText.Length - 1)));
		}
		FocusArrowsOnText(mainLeftArrow, mainRightArrow, text, animated);
		if (saidFirstText)
		{
			string text2 = optionsText[currentOption].gameObject.name;
			string text3 = text.text;
			if (text2 == "levelEditor" || text2 == "credits")
			{
				text3 = text3 + RDString.Get("narration.shortPause") + RDString.Get("narration.unavailable");
			}
			Narration.Say(text3, NarrationCategory.Navigation, currentOption + 1, optionsText.Length, ElementType.None, null, null, false, NarrationActionName.ToNavigateVertical, NarrationActionName.ToSelect, NarrationActionName.ToGoBack);
		}
	}

	private void FocusArrowsOnText(RectTransform leftArrow, RectTransform rightArrow, Text text, bool animated, float offset = 7f)
	{
		TextGenerator cachedTextGenerator = text.cachedTextGenerator;
		if (cachedTextGenerator.characters.Count == 0)
		{
			RDBaseDllDummy.printesw(text.gameObject.name + " \"" + text.text + "\": characters.count is 0");
			return;
		}
		IList<UIVertex> verts = cachedTextGenerator.verts;
		float num = (text.font.dynamic ? text.canvas.scaleFactor : 1f);
		float num2 = LeftmostVertex(verts) / num;
		float num3 = RightmostVertex(verts) / num;
		if (debug_showTestPixels && Application.isEditor)
		{
			foreach (RectTransform testPixel in testPixels)
			{
				testPixel.gameObject.SetActive(value: false);
			}
			for (int i = 0; i < verts.Count; i++)
			{
				RectTransform rectTransform;
				if (i == testPixels.Count)
				{
					Transform obj = UnityEngine.Object.Instantiate(prefabTestPixel).transform;
					obj.SetParent(options, worldPositionStays: true);
					obj.gameObject.name = $"Vertex {i}";
					rectTransform = obj.GetComponent<RectTransform>();
					testPixels.Add(rectTransform);
					obj.GetComponent<Image>().color = ((i < 4) ? Color.red : ((i < 8) ? Color.yellow : ((i < 12) ? Color.green : ((i < 16) ? Color.cyan : ((i < 20) ? Color.blue : ((i < 24) ? Color.magenta : Color.white))))));
				}
				else
				{
					rectTransform = testPixels[i];
				}
				rectTransform.gameObject.SetActive(value: true);
				rectTransform.AnchorPosX(verts[i].position.x / num);
				rectTransform.AnchorPosY(verts[i].position.y / num + text.rectTransform.anchoredPosition.y);
			}
		}
		float duration = (animated ? 0.15f : 0f);
		Vector2 anchoredPosition = text.rectTransform.anchoredPosition;
		if (leftArrow == mainLeftArrow)
		{
			anchoredPosition.y += optionsTextOffsetY;
		}
		leftArrow.DOKill();
		rightArrow.DOKill();
		Vector2 endValue = new Vector2(anchoredPosition.x + num2 - offset, anchoredPosition.y);
		Vector2 endValue2 = new Vector2(anchoredPosition.x + num3 + offset, anchoredPosition.y);
		leftArrow.DOAnchorPos(endValue, duration).SetEase(Ease.OutExpo).SetUpdate(isIndependentUpdate: true);
		rightArrow.DOAnchorPos(endValue2, duration).SetEase(Ease.OutExpo).SetUpdate(isIndependentUpdate: true);
		static float LeftmostVertex(IList<UIVertex> vertices)
		{
			float num4 = float.MaxValue;
			foreach (UIVertex vertex in vertices)
			{
				num4 = Mathf.Min(num4, vertex.position.x);
			}
			return num4;
		}
		static float RightmostVertex(IList<UIVertex> vertices)
		{
			float num4 = float.MinValue;
			foreach (UIVertex vertex2 in vertices)
			{
				num4 = Mathf.Max(num4, vertex2.position.x);
			}
			return num4;
		}
	}

	private void SelectOption()
	{
		FocusArrowsOnText(mainLeftArrow, mainRightArrow, optionsText[currentOption], animated: false);
		switch (optionsText[currentOption].gameObject.name)
		{
		case "storyMode":
			PlayButtonSound("MainMenu");
			GoToSection(MenuSection.SlotMenu);
			break;
		case "expoMode":
			Persistence.currentSlotIndex = 0;
			RDC.booth = true;
			TransitionToScene("scnExpoMenu");
			break;
		case "customLevels":
			PlayConfirmSound();
			TransitionToScene("scnCLS");
			break;
		case "levelEditor":
			PlayConfirmSound();
			TransitionToScene("scnEditor");
			break;
		case "calibration":
			PlayConfirmSound();
			scnCalibration.nextSceneName = "scnMenu";
			TransitionToScene("scnCalibration");
			break;
		case "settings":
			PlayButtonSound("MainMenu");
			settingsMenu.Show();
			break;
		case "credits":
			PlayButtonSound("MainMenu");
			GoToSection(MenuSection.Credits);
			break;
		case "music":
			PlayButtonSound("MainMenu");
			Application.OpenURL(RDString.Get("mainMenu.musicLink"));
			break;
		case "rhythmWeightlifter":
			Persistence.currentSlotIndex = 2;
			TransitionToScene("scnRhythmWeightlifter");
			break;
		case "quit":
			PlayHammerSound("MainMenu");
			Application.Quit();
			break;
		}
	}

	private void TransitionToScene(string scene)
	{
		if (transitioningToAnotherScene)
		{
			return;
		}
		PlayConfirmSound();
		transitioningToAnotherScene = true;
		float duration = (Persistence.GetFastMenu() ? 0.51f : 1.3f);
		song.DOFade(0f, duration);
		Text obj = optionsText[currentOption];
		obj.color = optionHighlightColor;
		obj.DOColor(Color.white, 0.5f);
		Text component = mainLeftArrow.GetComponent<Text>();
		Color color = component.color;
		component.color = optionHighlightColor;
		component.DOColor(color, 0.5f);
		Text component2 = mainRightArrow.GetComponent<Text>();
		Color color2 = component.color;
		component2.color = optionHighlightColor;
		component2.DOColor(color2, 0.5f);
		overlay.enabled = true;
		overlay.color = Color.clear;
		overlay.DOKill();
		overlay.DOColor(Color.black, duration).SetEase(Ease.InExpo).OnComplete(() =>
		{
			scnGame.levelSpeed = 1f;
			if (scene == "Intro")
			{
				scnBase.GoToLevelWithEnum(Level.Intro);
			}
			else
			{
				scnBase.GoToScene(scene);
			}
		});
	}

	private void HighlightSlot(int slotIndex, bool sound = true)
	{
		currentSlotIndex = CycleInt(slotIndex, slots.Length);
		SlotUI[] array = slots;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetFocused(on: false);
		}
		if (sound)
		{
			PlayCursorSound("MainMenu", RDUtils.PitchSemitonesToPercent(4f - 10f * (float)currentSlotIndex / (float)(slots.Length - 1)));
		}
		slots[currentSlotIndex].SetFocused(on: true);
		if (saidFirstText)
		{
			NarrateSlot(includeSlotSelection: false, string.Empty);
		}
	}

	private int CycleInt(int number, int total)
	{
		if (number >= 0)
		{
			return number % total;
		}
		return (number + 1) % total + (total - 1);
	}

	private void GoToSection(MenuSection section)
	{
		switch (section)
		{
		case MenuSection.TitleScreen:
			Narration.Set(RDString.Get("narration.titleAndVersion").Replace("[versionNumber]", Application.version));
			Narration.Say(logoInstructions.text, NarrationCategory.Navigation, false, NarrationActionName.ForContextInstructions, NarrationActionName.ToRepeatLastMessage, NarrationActionName.ToToggleNarration);
			break;
		case MenuSection.MainMenu:
			Narration.Set(RDString.Get("narration.mainMenu"), -1, optionsText.Length, ElementType.VerticalList, null, RDString.Get("narration.pressF1"));
			Narration.Say(optionsText[currentOption].text, NarrationCategory.Navigation, false, NarrationActionName.ToNavigateVertical, NarrationActionName.ToSelect, NarrationActionName.ToGoBack, NarrationActionName.ForContextInstructions, NarrationActionName.ToRepeatLastMessage, NarrationActionName.ToToggleNarration);
			break;
		case MenuSection.SlotMenu:
			NarrateSlot(includeSlotSelection: true, string.Empty);
			break;
		}
		changingSection = true;
		if (section == MenuSection.MainMenu && this.section == MenuSection.Credits)
		{
			this.section = section;
			creditsTransform.DOKill();
			creditsTransform.DOAnchorPosX(0f - ratioDistance, 0.5f).SetEase(Ease.OutExpo).OnComplete(() =>
			{
				CreditsScroller[] array2 = creditsPages;
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i].gameObject.SetActive(value: false);
				}
				creditsTransform.gameObject.SetActive(value: false);
			});
			mainMenu.DOKill();
			mainMenu.DOAnchorPosX(0f, 0.5f).SetEase(Ease.OutExpo).OnComplete(() =>
			{
				changingSection = false;
			});
			PlayHammerSound();
		}
		else if (section == MenuSection.TitleScreen || this.section == MenuSection.TitleScreen)
		{
			this.section = section;
			bool flag = section == MenuSection.MainMenu;
			int num = (flag ? 1 : 0);
			Ease ease = (flag ? Ease.OutExpo : Ease.OutExpo);
			logoRectTransform.DOKill();
			logoRectTransform.DOAnchorPosX(logoPositionsX[num], 0.3f).SetEase(ease);
			options.DOKill();
			options.DOAnchorPosX(flag ? 88f : ratioDistance, 0.3f).SetEase(ease).OnComplete(() =>
			{
				changingSection = false;
			});
			gamerTagText.rectTransform.DOAnchorPosX(flag ? 170f : (-9f), 0.3f).SetEase(ease);
			logoInstructions.gameObject.SetActive(!flag);
			KeyCode[] shortcuts = (flag ? mainMenuKeys : titleScreenKeys);
			KeyInfoPanel.instance.UpdateShortcutsRow(shortcuts);
		}
		else if (section == MenuSection.SlotMenu || this.section == MenuSection.SlotMenu)
		{
			this.section = section;
			bool num2 = section == MenuSection.MainMenu;
			float endValue = (num2 ? 0f : canvasHeight);
			float endValue2 = (num2 ? (0f - canvasHeight) : 0f);
			float endValue3 = (num2 ? (canvasHeight / 2f) : ((0f - canvasHeight) / 2f));
			Ease ease2 = Ease.OutExpo;
			mainMenu.DOKill();
			mainMenu.DOAnchorPosY(endValue, 1f).SetEase(ease2);
			slotMenu.DOKill();
			slotMenu.DOAnchorPosY(endValue2, 1f).SetEase(ease2);
			wrldCamera.transform.DOKill();
			wrldCamera.transform.DOMoveY(endValue3, 1f).SetEase(ease2);
			DOVirtual.DelayedCall(0.5f, () =>
			{
				changingSection = false;
			});
			RectTransform parent = (num2 ? mainMenu : slotMenu);
			RectTransform rect = KeyInfoPanel.instance.rect;
			rect.SetParent(parent);
			rect.anchoredPosition = rect.anchoredPosition.WithY(0f);
			if (section == MenuSection.SlotMenu)
			{
				slotLeftArrow.gameObject.SetActive(value: false);
				slotRightArrow.gameObject.SetActive(value: false);
			}
		}
		else if (section == MenuSection.Credits)
		{
			this.section = section;
			creditsTransform.DOKill();
			creditsTransform.gameObject.SetActive(value: true);
			CreditsScroller[] array = creditsPages;
			for (int num3 = 0; num3 < array.Length; num3++)
			{
				array[num3].gameObject.SetActive(value: false);
			}
			currentCreditPageIndex = 0;
			creditsPages[currentCreditPageIndex].gameObject.SetActive(value: true);
			creditsTransform.DOAnchorPosX(0f, 0.5f).SetEase(Ease.OutExpo);
			mainMenu.DOKill();
			mainMenu.DOAnchorPosX(ratioDistance, 0.5f).SetEase(Ease.OutExpo).OnComplete(() =>
			{
				changingSection = false;
			});
		}
	}

	public override void OnBeat()
	{
		base.OnBeat();
		OnBeatInternal(on: true);
	}

	private void OnBeatInternal(bool on)
	{
		if (on)
		{
			lastBeatTime = Time.unscaledTime;
		}
		onBeatRecentlyCalled = on;
		string text = (on ? " >" : ">");
		mainLeftArrow.GetComponent<Text>().text = text;
		mainRightArrow.GetComponent<Text>().text = text;
		slotLeftArrow.GetComponent<Text>().text = text;
		slotRightArrow.GetComponent<Text>().text = text;
		SlotUI[] array = slots;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].OnBeat(on);
		}
		Sprite sprite = titleNoteFrames[on ? 1 : 0];
		leftNote.sprite = sprite;
		rightNote.sprite = sprite;
		logoInstructions.enabled = !on;
	}

	private void PlayConfirmSound()
	{
		scrConductor.PlayImmediately("sndMenuSelect", 1f, RDUtils.GetMixerGroup("MainMenu"));
	}

	private void NarrateSlot(bool includeSlotSelection, string actionSuccess)
	{
		_ = string.Empty;
		if (includeSlotSelection)
		{
			Narration.Set(RDString.Get("narration.slotSelection").Replace("[actionSuccess]", actionSuccess).Replace("[selectSlot]", RDString.Get("slotMenu.instructions.selectSlot")), ElementType.HorizontalList, slots.Length);
		}
		Narration.Say(RDString.Get("narration.slotDescription").Replace("[index]", (currentSlotIndex + 1).ToString()).Replace("[slotsLength]", slots.Length.ToString())
			.Replace("[starsNumber]", slots[currentSlotIndex].stars.text)
			.Replace("[scrollsNumber]", slots[currentSlotIndex].scrolls.text)
			.Replace("[percentage]", slots[currentSlotIndex].percent.text.Replace("%", string.Empty)), NarrationCategory.Navigation, false, NarrationActionName.ToSelect, NarrationActionName.ToNavigateHorizontal, NarrationActionName.ToGoBack);
	}

	private void LateUpdate()
	{
		if (Application.isEditor)
		{
			UnityPlayerWindow playerWindow = PlatformHelper.instance.playerWindow;
			if (playerWindow != null && Input.GetKeyDown(KeyCode.F3))
			{
				playerWindow.ResetPosition(null);
			}
		}
	}
}
