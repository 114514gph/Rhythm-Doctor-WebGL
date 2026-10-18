using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using RDLevelEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuMode : RDBase
{
	public class Category
	{
		public PauseModeName modeName;

		public string localizationKey;

		public RectTransform container;

		public List<PauseMenuContentData> contentsDataList;

		public Dictionary<PauseContentName, GameObject> contentsDict;

		public Dictionary<PauseContentName, PauseModeContentArrows> contentArrowsDict;

		public Image icon;

		public Shadow iconShadow;

		public Category(PauseModeName modeName, string localizationKey, List<PauseMenuContentData> contentsDataList)
		{
			this.modeName = modeName;
			this.contentsDataList = contentsDataList;
			this.localizationKey = localizationKey;
			contentsDict = new Dictionary<PauseContentName, GameObject>();
			contentArrowsDict = new Dictionary<PauseContentName, PauseModeContentArrows>();
		}
	}

	private const float ContentHeight = 24f;

	private const float SeparatorHeight = 1f;

	private const float CategoryIconSize = 16f;

	private const float ModeChangerArrowSize = 7f;

	public const float ChangeModeAnimationDuration = 0.1f;

	private const Ease ChangeModeAnimationEase = Ease.OutExpo;

	private const int MinContentIndexToScroll = 3;

	public const float SelectedContentAnimDuration = 0.2f;

	[NonSerialized]
	public bool changeIndexWithSound;

	[NonSerialized]
	public float modeWidth;

	[NonSerialized]
	public RectTransform rectTransform;

	[NonSerialized]
	public PauseMenu parentPauseMenu;

	[NonSerialized]
	public bool moveSelectorInstantly;

	[NonSerialized]
	public PauseModeName lastModeName;

	[NonSerialized]
	public Dictionary<Text, string> textsKeysDict;

	[NonSerialized]
	public List<Category> categories;

	[NonSerialized]
	public int currentCategoryIndex;

	[NonSerialized]
	public int hiddenContent;

	public RectTransform contentContainer;

	public RectTransform separatorContainer;

	public RectTransform selectedContent;

	public GameObject contentPrefab;

	public GameObject contentArrowsPrefab;

	public GameObject contentCategorySelectorPrefab;

	public GameObject separatorPrefab;

	public Image selectedContentImage;

	public Sprite modeChangerArrow;

	public Text titleText;

	public Color selectedContentDefaultColor;

	public Color selectedContentPressedColor;

	public Scrollbar verticalScrollbar;

	public ScrollRect scrollRect;

	public RectTransform scrollRectContainer;

	private int _currentContentIndex;

	private PauseModeName modeName;

	private GameObject skip;

	private GameObject restart;

	private GameObject restartFromCheckpoint;

	private GameObject quit;

	private GameObject windowMovement;

	private GameObject streamerMode;

	private bool firstTimeBeingShown = true;

	private bool usesLevelDetail;

	private TweenerCore<float, float, FloatOptions> scrollbarTween;

	private PauseModeCategorySelector categorySelector;

	private List<GameObject> separators;

	private GameObject categorySelectorContainer;

	private int lastIndex;

	private const string DogModeSound = "sndDogToy";

	private const float SelectedContentPressedDuration = 0.2f;

	[NonSerialized]
	public bool skipNarration;

	public float ScrollbarValue
	{
		get
		{
			return verticalScrollbar.value;
		}
		set
		{
			verticalScrollbar.value = value;
		}
	}

	public int CurrentContentIndex => _currentContentIndex;

	public int CurrentActiveContentsLength
	{
		get
		{
			int num = 0;
			if (categories.Count > 1)
			{
				foreach (Transform item in CurrentCategory.container)
				{
					if (item.gameObject.activeSelf)
					{
						num++;
					}
				}
				num++;
			}
			else
			{
				foreach (Transform item2 in contentContainer)
				{
					if (item2.gameObject.activeSelf)
					{
						num++;
					}
				}
			}
			return num;
		}
	}

	private PauseMenuContentData CurrentContentData => contentsDataList[CurrentContentIndex];

	private PauseModeContentArrows CurrentContentArrows
	{
		get
		{
			if (!contentArrowsDict.ContainsKey(CurrentContentData.name))
			{
				return null;
			}
			return contentArrowsDict[CurrentContentData.name];
		}
	}

	public Category CurrentCategory
	{
		get
		{
			return categories[currentCategoryIndex];
		}
		set
		{
			categories[currentCategoryIndex] = value;
		}
	}

	public List<PauseMenuContentData> contentsDataList => CurrentCategory.contentsDataList;

	public Dictionary<PauseContentName, GameObject> contentsDict => CurrentCategory.contentsDict;

	public Dictionary<PauseContentName, PauseModeContentArrows> contentArrowsDict => CurrentCategory.contentArrowsDict;

	private void Awake()
	{
		firstTimeBeingShown = true;
	}

	private void Start()
	{
		changeIndexWithSound = true;
	}

	private void OnEnable()
	{
		CheckButtonsVisibility();
	}

	private void CheckButtonsVisibility()
	{
		hiddenContent = 0;
		PauseModeName pauseModeName = modeName;
		if ((pauseModeName == PauseModeName.Game || pauseModeName == PauseModeName.DogMode) && base.game?.currentLevel != null)
		{
			skip?.SetActive(base.game.levelIsSkippable);
			if (!base.game.levelIsSkippable && skip != null)
			{
				hiddenContent++;
			}
			restart?.SetActive(base.game.levelIsRestartable);
			if (!base.game.levelIsRestartable && restart != null)
			{
				hiddenContent++;
			}
			quit?.SetActive(base.game.levelIsQuittable);
			if (!base.game.levelIsQuittable && quit != null)
			{
				hiddenContent++;
			}
			bool flag = false;
			int[] checkpointBarsDesc = base.game.currentLevel.checkpointBarsDesc;
			if (checkpointBarsDesc != null && checkpointBarsDesc.Length != 0)
			{
				flag = base.conductor.barNumber > checkpointBarsDesc[^1];
			}
			restartFromCheckpoint?.SetActive(flag);
			if (!flag && restartFromCheckpoint != null)
			{
				hiddenContent++;
			}
		}
		if (modeName == PauseModeName.FirstTimeSetup)
		{
			if (PauseMenuContentData.usesConsoleWindowMovement)
			{
				streamerMode?.SetActive(value: false);
				hiddenContent++;
				windowMovement?.SetActive(value: false);
				hiddenContent++;
			}
			else
			{
				bool flag2 = !Persistence.GetForceFirstTimeSetup();
				streamerMode?.SetActive(flag2);
				windowMovement?.SetActive(!flag2);
				hiddenContent++;
			}
		}
	}

	public void Initialize(PauseMenu mainPauseMenu, PauseMenuModeData modeData, PauseMenuData pauseMenuData)
	{
		separators = new List<GameObject>();
		textsKeysDict = new Dictionary<Text, string>();
		parentPauseMenu = mainPauseMenu;
		this.modeName = modeData.name;
		usesLevelDetail = modeData.useLevelDetail;
		base.gameObject.name = this.modeName.ToString();
		this.rectTransform = GetComponent<RectTransform>();
		modeWidth = this.rectTransform.sizeDelta.x;
		selectedContent.sizeDelta = new Vector2(modeWidth, 24f);
		VerticalLayoutGroup component = separatorContainer.GetComponent<VerticalLayoutGroup>();
		component.padding.top = Mathf.FloorToInt(23f);
		component.spacing = 23f;
		categories = new List<Category>();
		List<PauseMenuContentData> modeContents = pauseMenuData.GetModeContents(this.modeName);
		string text = modeData.localizedText;
		if (modeData.useCategories)
		{
			PauseMenuContentData pauseMenuContentData = new PauseMenuContentData();
			pauseMenuContentData.name = PauseContentName.CategorySelector;
			List<PauseMenuContentData> list = new List<PauseMenuContentData> { pauseMenuContentData };
			List<PauseMenuContentData> list2 = new List<PauseMenuContentData>();
			foreach (PauseMenuContentData item3 in modeContents)
			{
				if (item3.valueType == PauseContentValueType.ModeChanger)
				{
					list2.Add(item3);
				}
				else
				{
					list.Add(item3);
				}
			}
			Category item = new Category(PauseModeName.DefaultCategory, modeData.defaultCategoryLocalizationKey, list);
			categories.Add(item);
			foreach (PauseMenuContentData item4 in list2)
			{
				PauseModeName modeName = item4.targetMode;
				List<PauseMenuContentData> modeContents2 = pauseMenuData.GetModeContents(modeName);
				modeContents2.Insert(0, pauseMenuContentData);
				string localizedText = Array.Find(pauseMenuData.modes, (PauseMenuModeData mode) => mode.name == modeName).localizedText;
				Category item2 = new Category(modeName, localizedText, modeContents2);
				categories.Add(item2);
			}
			categorySelectorContainer = UnityEngine.Object.Instantiate(contentCategorySelectorPrefab, contentContainer);
			categorySelector = categorySelectorContainer.GetComponent<PauseModeCategorySelector>();
			categorySelector.Initialize(modeWidth, 24f, this);
			foreach (Category category in categories)
			{
				RemoveSteamDeckOptions(category.contentsDataList);
				category.contentsDict.Add(PauseContentName.CategorySelector, categorySelectorContainer);
				PauseModeName categoryModeName = ((category.modeName == PauseModeName.DefaultCategory) ? this.modeName : category.modeName);
				PauseMenuModeData pauseMenuModeData = Array.Find(pauseMenuData.modes, (PauseMenuModeData mode) => mode.name == categoryModeName);
				GameObject gameObject = new GameObject($"{categoryModeName} Icon", typeof(Image));
				gameObject.transform.SetParent(categorySelector.layoutRect, worldPositionStays: false);
				RectTransform orAddComponent = gameObject.GetOrAddComponent<RectTransform>();
				orAddComponent.sizeDelta = new Vector2(16f, 16f);
				orAddComponent.anchoredPosition3D = orAddComponent.anchoredPosition3D.WithZ(0f);
				category.icon = gameObject.GetComponent<Image>();
				category.icon.sprite = pauseMenuModeData.categoryIcon;
				category.icon.preserveAspect = true;
				category.iconShadow = gameObject.AddComponent<Shadow>();
				category.iconShadow.effectDistance = new Vector2(0f, -1f);
				category.iconShadow.effectColor = Color.black.WithAlpha(0.2f);
				gameObject.AddComponent<Button>().onClick.AddListener(() =>
				{
					changeIndexWithSound = false;
					ChangeCategory(category.modeName);
				});
			}
			RectTransform orAddComponent2 = new GameObject("Categories Container").GetOrAddComponent<RectTransform>();
			orAddComponent2.SetParent(contentContainer, worldPositionStays: false);
			orAddComponent2.anchoredPosition3D = orAddComponent2.anchoredPosition3D.WithZ(0f);
			orAddComponent2.sizeDelta = new Vector2(modeWidth, 24f);
			foreach (Category category3 in categories)
			{
				GameObject gameObject2 = new GameObject($"{category3.modeName} Container", typeof(VerticalLayoutGroup));
				RectTransform rectTransform = (category3.container = gameObject2.GetOrAddComponent<RectTransform>());
				rectTransform.SetParent(orAddComponent2, worldPositionStays: false);
				rectTransform.anchorMax = Vector2.up;
				rectTransform.anchorMin = Vector2.up;
				rectTransform.pivot = Vector2.up;
				rectTransform.anchoredPosition3D = new Vector3(modeWidth, 24f, 0f);
				rectTransform.sizeDelta = new Vector2(modeWidth, 24f * (float)category3.contentsDataList.Count);
				gameObject2.GetComponent<VerticalLayoutGroup>().padding.top = 24;
				SetupCategory(category3.modeName, gameObject2.transform, category3.contentsDataList);
				currentCategoryIndex++;
			}
			currentCategoryIndex = 0;
			CurrentCategory.container.anchoredPosition = CurrentCategory.container.anchoredPosition.WithX(0f);
			text = CurrentCategory.localizationKey;
		}
		else
		{
			Category category2 = new Category(modeData.name, modeData.localizedText, modeContents);
			RemoveSteamDeckOptions(category2.contentsDataList);
			categories.Add(category2);
			SetupCategory(category2.modeName, contentContainer, modeContents);
		}
		CheckButtonsVisibility();
		textsKeysDict.Add(titleText, text);
		titleText.text = RDString.Get(text);
		CheckCJKText(titleText);
	}

	private static void RemoveSteamDeckOptions(List<PauseMenuContentData> data)
	{
		if (RDBase.debugSettings.RunningOnSteamDeck && SteamIntegration.IsSteamInBigPictureMode && scnBase.isSteamDeck)
		{
			data.RemoveAll((PauseMenuContentData contentData) =>
			{
				PauseContentName pauseContentName = contentData.name;
				return pauseContentName == PauseContentName.Resolution || pauseContentName == PauseContentName.Fullscreen || pauseContentName == PauseContentName.VSync;
			});
		}
		if (!RDBase.debugSettings.RunningOnSteamDeck || !(SceneManager.GetActiveScene().name == "scnMenu"))
		{
			data.RemoveAll((PauseMenuContentData contentData) => contentData.name == PauseContentName.ForceDesktopFeatureSet);
		}
	}

	private void SetupCategory(PauseModeName categoryName, Transform categoryContainer, List<PauseMenuContentData> modeContentsData)
	{
		int num = 0;
		foreach (PauseMenuContentData contentData in modeContentsData)
		{
			if (contentData.name == PauseContentName.CategorySelector)
			{
				continue;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(contentPrefab, categoryContainer);
			gameObject.name = contentData.name.ToString();
			RectTransform component = gameObject.GetComponent<RectTransform>();
			component.sizeDelta = new Vector2(modeWidth, 24f);
			CurrentCategory.contentsDict.Add(contentData.name, gameObject);
			Text componentInChildren = gameObject.GetComponentInChildren<Text>();
			RectTransform component2 = componentInChildren.GetComponent<RectTransform>();
			string text = "pauseMenu." + contentData.name;
			textsKeysDict.Add(componentInChildren, text);
			componentInChildren.text = RDString.Get(text);
			CheckCJKText(componentInChildren);
			float num2 = 24f;
			Vector2 sizeDelta = new Vector2(modeWidth - 4f, num2);
			float num3 = 3f;
			if (contentData.icon != null)
			{
				GameObject gameObject2 = new GameObject("Icon Parent", typeof(RectTransform));
				gameObject2.transform.SetParent(component, worldPositionStays: false);
				gameObject2.transform.SetAsFirstSibling();
				gameObject2.layer = 16;
				RectTransform component3 = gameObject2.GetComponent<RectTransform>();
				component3.sizeDelta = new Vector2(contentData.iconSize, 24f);
				component3.anchorMin = new Vector2(0f, 0.5f);
				component3.anchorMax = new Vector2(0f, 0.5f);
				component3.pivot = new Vector2(0f, 0.5f);
				component3.AnchorPosX(3f);
				GameObject obj = new GameObject("Icon", typeof(Image), typeof(Shadow));
				obj.transform.SetParent(gameObject2.transform, worldPositionStays: false);
				obj.layer = 16;
				Image component4 = obj.GetComponent<Image>();
				Shadow component5 = obj.GetComponent<Shadow>();
				component4.preserveAspect = true;
				component4.raycastTarget = false;
				component4.sprite = contentData.icon;
				component4.color = Color.black;
				component4.rectTransform.localScale = Vector3.one;
				component4.rectTransform.sizeDelta = new Vector2(contentData.iconSize, contentData.iconSize);
				component4.rectTransform.localPosition = Vector3.one;
				component4.rectTransform.anchoredPosition = Vector2.zero;
				component5.effectColor = new Color(0f, 0f, 0f, 0.17f);
				component5.effectDistance = new Vector2(0f, -1f);
				sizeDelta.x -= contentData.iconSize + 3f;
				num3 += contentData.iconSize + 3f;
			}
			gameObject.AddComponent<Image>().color = Color.clear;
			Button button = gameObject.AddComponent<Button>();
			PauseContentName pauseContentName = contentData.name;
			if (pauseContentName == PauseContentName.Skip || pauseContentName == PauseContentName.Dog_Skip)
			{
				skip = gameObject;
			}
			else
			{
				pauseContentName = contentData.name;
				if (pauseContentName == PauseContentName.Restart || pauseContentName == PauseContentName.Dog_Restart)
				{
					restart = gameObject;
				}
				else
				{
					pauseContentName = contentData.name;
					if (pauseContentName == PauseContentName.Quit || pauseContentName == PauseContentName.Dog_Quit)
					{
						quit = gameObject;
					}
					else if (contentData.name == PauseContentName.RestartFromCheckpoint)
					{
						restartFromCheckpoint = gameObject;
					}
					else if (contentData.name == PauseContentName.WindowMovement)
					{
						windowMovement = gameObject;
					}
					else if (contentData.name == PauseContentName.StreamerMode)
					{
						streamerMode = gameObject;
					}
				}
			}
			button.onClick.AddListener(() =>
			{
				if (!PauseMenu.inputsLocked)
				{
					changeIndexWithSound = false;
					SelectFirstContentWithName(contentData.name);
				}
			});
			if (contentData.valueType == PauseContentValueType.ModeChanger)
			{
				GameObject gameObject3 = new GameObject("ModeArrowContainer", typeof(RectTransform));
				gameObject3.transform.SetParent(component, worldPositionStays: false);
				RectTransform component6 = gameObject3.GetComponent<RectTransform>();
				component6.localScale = Vector3.one;
				component6.anchoredPosition = Vector2.zero;
				component6.sizeDelta = new Vector2(modeWidth, 24f);
				GameObject obj2 = new GameObject("Arrow", typeof(Image), typeof(Shadow));
				obj2.transform.SetParent(gameObject3.transform, worldPositionStays: false);
				obj2.layer = 16;
				RectTransform obj3 = (RectTransform)obj2.transform;
				obj3.localScale = Vector3.one;
				obj3.sizeDelta = new Vector2(7f, 7f);
				obj3.anchorMin = new Vector2(0f, 0.5f);
				obj3.anchorMax = new Vector2(0f, 0.5f);
				obj3.pivot = new Vector2(0f, 0.5f);
				obj3.anchoredPosition = new Vector2(modeWidth - 8f, 0.5f);
				Image component7 = obj2.GetComponent<Image>();
				component7.preserveAspect = true;
				component7.raycastTarget = false;
				Sprite sprite = modeChangerArrow;
				component7.sprite = sprite;
				component7.color = Color.black;
				Shadow component8 = obj2.GetComponent<Shadow>();
				component8.effectColor = new Color(0f, 0f, 0f, 0.17f);
				component8.effectDistance = new Vector2(0f, -1f);
				sizeDelta.x -= 8f;
			}
			else if (contentData.valueType != PauseContentValueType.None)
			{
				RectTransform component9 = UnityEngine.Object.Instantiate(contentArrowsPrefab, component).GetComponent<RectTransform>();
				component9.sizeDelta = new Vector2(60f, 24f);
				component9.AnchorPosX(modeWidth - 61f);
				PauseModeContentArrows componentInChildren2 = component9.GetComponentInChildren<PauseModeContentArrows>();
				RectTransform component10 = componentInChildren2.valueText.GetComponent<RectTransform>();
				pauseContentName = contentData.name;
				bool flag = (pauseContentName == PauseContentName.Resolution || pauseContentName == PauseContentName.Fullscreen) && (object)base.game != null && base.game.windowChoreographer is RealWindowChoreographer;
				componentInChildren2.canChangeContentValue = !flag;
				componentInChildren2.disabledImage = component10.GetChild(0).gameObject;
				componentInChildren2.disabledImage.SetActive(value: false);
				componentInChildren2.Initialize(this, contentData);
				CurrentCategory.contentArrowsDict.Add(contentData.name, componentInChildren2);
				sizeDelta.x -= 61f;
				if (!componentInChildren2.canChangeContentValue)
				{
					componentInChildren2.UpdateValue();
				}
			}
			component2.sizeDelta = sizeDelta;
			component2.AnchorPosX(num3);
			Canvas.ForceUpdateCanvases();
			componentInChildren.lineSpacing = GetLineSpacing(componentInChildren);
			if (num >= separators.Count)
			{
				GameObject gameObject4 = UnityEngine.Object.Instantiate(separatorPrefab, separatorContainer, worldPositionStays: false);
				gameObject4.GetComponent<RectTransform>().sizeDelta = new Vector2(modeWidth, 1f);
				separators.Add(gameObject4);
			}
			num++;
		}
	}

	private float GetLineSpacing(Text text)
	{
		if (text.cachedTextGenerator.lineCount < 4)
		{
			return 1f;
		}
		return 0.8f;
	}

	public void Show(bool moveWithinOneFrame = false, bool isReversed = false)
	{
		foreach (KeyValuePair<Text, string> item in textsKeysDict)
		{
			Text key = item.Key;
			if (key == titleText)
			{
				key.text = RDString.Get(CurrentCategory.localizationKey);
			}
			else
			{
				key.text = RDString.Get(item.Value);
			}
			key.lineSpacing = GetLineSpacing(key);
			CheckCJKText(key);
		}
		foreach (KeyValuePair<PauseContentName, PauseModeContentArrows> item2 in contentArrowsDict)
		{
			if (item2.Value.canChangeContentValue)
			{
				item2.Value.UpdateValue();
				continue;
			}
			SpriteAnimation[] glitches = item2.Value.glitches;
			for (int i = 0; i < glitches.Length; i++)
			{
				glitches[i].gameObject.SetActive(value: false);
			}
		}
		if (firstTimeBeingShown && base.gameObject.activeSelf)
		{
			PauseContentName selectedContentName = contentsDataList[0].name;
			MoveSelectedContentRect(selectedContentName, moveWithinOneFrame: true);
			firstTimeBeingShown = false;
		}
		if (moveWithinOneFrame)
		{
			rectTransform.anchoredPosition = Vector2.zero;
		}
		else
		{
			float num = (isReversed ? (modeWidth * -1f) : modeWidth);
			rectTransform.anchoredPosition = new Vector2(num, 0f);
			rectTransform.DOKill();
			rectTransform.DOAnchorPosX(0f, 0.1f).SetEase(Ease.OutExpo).SetUpdate(isIndependentUpdate: true);
		}
		int num2 = CurrentActiveContentsLength;
		foreach (Transform item3 in separatorContainer)
		{
			bool active = num2 > 0;
			item3.gameObject.SetActive(active);
			num2--;
		}
		CheckScrollingFunctionality();
		PauseMenuModeData modeByName = parentPauseMenu.pauseMenuData.GetModeByName(modeName);
		bool useDelay = true;
		if (modeByName.useLevelDetail && modeName == parentPauseMenu.firstModeName)
		{
			useDelay = false;
		}
		bool checkRequiredControllers = false;
		parentPauseMenu.ShowAnimated(usesLevelDetail, useDelay, checkRequiredControllers);
		PrepareDataToUpdateLevelDetail();
		CheckButtonsVisibility();
		SayCurrentContent(CurrentContentData, CurrentContentIndex + 1, contentsDataList.Count - hiddenContent);
	}

	public void SetContentIndex(int value)
	{
		RDBaseDllDummy.printem($"from {_currentContentIndex} to {value}, {contentsDataList.Count}");
		lastIndex = CurrentContentIndex;
		if (CurrentContentArrows != null && CurrentContentArrows.canChangeContentValue && usesLevelDetail && CurrentContentArrows.contentData.needsConfirmation)
		{
			CurrentContentArrows.Initialize(this, CurrentContentArrows.contentData);
		}
		if (contentsDataList[CurrentContentIndex].name == PauseContentName.CategorySelector)
		{
			SetCategoriesIcon(selected: false);
		}
		int num = CurrentContentIndex - value;
		if (num == 1 || num == -1)
		{
			RDBaseDllDummy.printem(_currentContentIndex + " - " + value + " - " + num);
			GameObject gameObject;
			do
			{
				_currentContentIndex -= num;
				if (CurrentContentIndex > contentsDataList.Count - 1)
				{
					_currentContentIndex = 0;
				}
				else if (CurrentContentIndex < 0)
				{
					_currentContentIndex = contentsDataList.Count - 1;
				}
				PauseContentName key = CurrentContentData.name;
				gameObject = contentsDict[key];
			}
			while (!gameObject.activeSelf);
		}
		else
		{
			RDBaseDllDummy.printem($"currentContentIndex: {_currentContentIndex} -> {value}");
			_currentContentIndex = value;
		}
		RDBaseDllDummy.printem(base.gameObject.name + " - " + _currentContentIndex);
		if (contentsDataList[CurrentContentIndex].name == PauseContentName.CategorySelector)
		{
			SetCategoriesIcon(selected: true);
		}
		if (CurrentContentArrows != null && !CurrentContentArrows.canChangeContentValue)
		{
			if (!CurrentContentArrows.showedGlitchOnce)
			{
				CurrentContentArrows.showedGlitchOnce = true;
				CurrentContentArrows.ShowDisabledIcon();
			}
			CurrentContentArrows.PlayGlitchAnimation();
		}
		if (changeIndexWithSound)
		{
			if (CurrentContentData.name == PauseContentName.CategorySelector)
			{
				PlayCategorySFX(CurrentCategory.modeName);
				scrConductor.PlayImmediatelyIgnoringTimeScale("sndPagerCursorMove", 1f, RDUtils.GetMixerGroup("PauseMenu"), RDUtils.PitchSemitonesToPercent(4f));
			}
			else
			{
				scrConductor.PlayImmediatelyIgnoringTimeScale("sndPagerCursorMove", 1f, RDUtils.GetMixerGroup("PauseMenu"), RDUtils.PitchSemitonesToPercent(4f - 10f * (float)CurrentContentIndex / (float)(contentsDataList.Count - 1)));
			}
		}
		else
		{
			changeIndexWithSound = true;
		}
		int num2 = CurrentActiveContentsLength - 3;
		float num3 = 1f;
		if (CurrentContentIndex >= num2)
		{
			num3 = 0f;
		}
		else if (CurrentContentIndex > 3)
		{
			int num4 = CurrentContentIndex - 3;
			int num5 = num2 - 3;
			float num6 = (float)num4 / (float)num5;
			RDBaseDllDummy.printem($"{num6} = {num4} / {num5}, {_currentContentIndex}");
			num3 = 1f - num6;
		}
		if (scrollbarTween != null)
		{
			scrollbarTween.Kill();
		}
		if (moveSelectorInstantly)
		{
			ScrollbarValue = num3;
		}
		else
		{
			printe("scrolling to value: " + num3);
			scrollbarTween = DOTween.To(() => ScrollbarValue, (float x) =>
			{
				ScrollbarValue = x;
			}, num3, 0.2f);
			scrollbarTween.SetEase(Ease.OutExpo);
			scrollbarTween.SetUpdate(isIndependentUpdate: true);
		}
		MoveSelectedContentRect(CurrentContentData.name, moveSelectorInstantly);
		PrepareDataToUpdateLevelDetail();
		Debug.Log($"{modeName}, {parentPauseMenu.currentModeName}");
		if (lastIndex != CurrentContentIndex && parentPauseMenu.currentModeName == modeName)
		{
			CheckButtonsVisibility();
			SayCurrentContent(CurrentContentData, CurrentContentIndex + 1, contentsDataList.Count - hiddenContent);
		}
	}

	private void PrepareDataToUpdateLevelDetail()
	{
		if (usesLevelDetail && parentPauseMenu.currentModeName == modeName)
		{
			string text = "pauseMenu.levelDetail.";
			text = ((CurrentContentData.name == PauseContentName.Back) ? (text + ((parentPauseMenu.firstModeName == PauseModeName.MainMenuSettings) ? "backMainMenu" : "backPauseMenu")) : ((CurrentContentData.name != PauseContentName.WindowMovement) ? (text + CurrentContentData.name) : (text + (PauseMenuContentData.usesConsoleWindowMovement ? "WindowMovementConsole" : "WindowMovement"))));
			string settingDetailValue = parentPauseMenu.GetSettingDetailValue(CurrentContentData, CurrentContentArrows);
			parentPauseMenu.UpdateLevelDetail(RDString.Get(text), settingDetailValue, CurrentContentData.needsConfirmation);
		}
	}

	private void CheckScrollingFunctionality()
	{
		RDBaseDllDummy.printem("");
		bool flag = CurrentActiveContentsLength > 4;
		scrollRect.enabled = flag;
		verticalScrollbar.gameObject.SetActive(flag);
		verticalScrollbar.transform.SetAsLastSibling();
		if (flag)
		{
			float num = (float)CurrentActiveContentsLength * 24f;
			scrollRectContainer.sizeDelta = scrollRectContainer.sizeDelta.WithY(num);
			float num2 = 72f;
			RDBaseDllDummy.printem($"({rectTransform.sizeDelta.y} - {num2}) / {num}");
			verticalScrollbar.size = (rectTransform.sizeDelta.y - num2) / num;
		}
	}

	public void Hide(bool isReversed = false)
	{
		float endValue = (isReversed ? modeWidth : (modeWidth * -1f));
		rectTransform.DOKill();
		rectTransform.DOAnchorPosX(endValue, 0.1f).SetEase(Ease.OutExpo).SetUpdate(isIndependentUpdate: true)
			.OnComplete(() =>
			{
				if (isReversed)
				{
					PauseContentName selectedContentName = contentsDataList[0].name;
					MoveSelectedContentRect(selectedContentName, moveWithinOneFrame: true);
					changeIndexWithSound = false;
					SetContentIndex(0);
				}
			});
	}

	public void MoveSelectedContentRect(PauseContentName selectedContentName, bool moveWithinOneFrame)
	{
		float endValue = contentsDict[selectedContentName].GetComponent<RectTransform>().anchoredPosition.y;
		if (moveWithinOneFrame)
		{
			selectedContent.anchoredPosition = new Vector2(0f, endValue);
			moveSelectorInstantly = false;
		}
		else
		{
			selectedContent.DOKill();
			selectedContent.DOAnchorPosY(endValue, 0.2f).SetEase(Ease.OutExpo).SetUpdate(isIndependentUpdate: true);
		}
	}

	public IEnumerator SelectContent(int forceIndex = -1)
	{
		if (CurrentContentData.name == PauseContentName.CategorySelector || (CurrentContentData.valueType != PauseContentValueType.ModeChanger && CurrentContentData.valueType != PauseContentValueType.None && !CurrentContentData.needsConfirmation))
		{
			yield break;
		}
		if (CurrentContentArrows != null && !CurrentContentArrows.canChangeContentValue)
		{
			CurrentContentArrows.PlayGlitchAnimation();
			yield break;
		}
		PauseMenu.inputsLocked = true;
		moveSelectorInstantly = false;
		if (forceIndex >= 0)
		{
			moveSelectorInstantly = true;
			SetContentIndex(forceIndex);
		}
		selectedContentImage.color = selectedContentPressedColor;
		selectedContentImage.DOKill();
		selectedContentImage.DOColor(selectedContentDefaultColor, 0.2f).SetEase(Ease.InQuad).SetUpdate(isIndependentUpdate: true);
		if (modeName != PauseModeName.SoundGlossary || CurrentContentData.name == PauseContentName.Back)
		{
			scrConductor.PlayImmediatelyIgnoringTimeScale((parentPauseMenu.dogMode && CurrentContentData.name == PauseContentName.Dog_Settings) ? "sndDogToy" : "sndPagerButton", 1f, RDUtils.GetMixerGroup("PauseMenu"));
		}
		PauseContentName selectedContentName = CurrentContentData.name;
		yield return new WaitForSecondsRealtime(0.2f);
		switch (selectedContentName)
		{
		case PauseContentName.Continue:
		case PauseContentName.Dog_Continue:
			if (base.game != null)
			{
				base.game.TogglePauseGame();
			}
			else if (parentPauseMenu.levelSelect != null)
			{
				parentPauseMenu.levelSelect.HidePauseMenu();
			}
			else
			{
				Hide();
			}
			break;
		case PauseContentName.Restart:
		case PauseContentName.Dog_Restart:
		case PauseContentName.RestartFromCheckpoint:
		{
			parentPauseMenu.enabled = false;
			bool fromCheckpoint = selectedContentName == PauseContentName.RestartFromCheckpoint;
			base.game.Restart(fromCheckpoint);
			break;
		}
		case PauseContentName.Skip:
		case PauseContentName.Dog_Skip:
		{
			parentPauseMenu.enabled = false;
			LevelEvent_GoToLevel queuedNextLevel = base.game.currentLevel.queuedNextLevel;
			if (queuedNextLevel != null && queuedNextLevel.skippable)
			{
				queuedNextLevel.skippable = false;
				queuedNextLevel.fadeOut = false;
				queuedNextLevel.isSkipping = true;
				queuedNextLevel.PrepNextLevel();
			}
			else if (scnGame.levelToLoadSource != LevelSource.ExternalPath || RDBase.debugSettings.Debug)
			{
				base.game.WinLevel();
			}
			break;
		}
		case PauseContentName.Settings:
		case PauseContentName.Dog_Settings:
			yield return parentPauseMenu.ChangeMode(PauseModeName.GameSettings);
			break;
		case PauseContentName.Quit:
		case PauseContentName.Dog_Quit:
			parentPauseMenu.enabled = false;
			if (base.game != null)
			{
				base.game.Quit();
			}
			else if (parentPauseMenu.levelSelect != null)
			{
				parentPauseMenu.levelSelect.StopAllAmbiences(0.6f);
				parentPauseMenu.levelSelect.MusicFadeOut(0.6f);
				DoSceneChange(() =>
				{
					parentPauseMenu.GoToScene("scnMenu");
				});
			}
			break;
		case PauseContentName.Back:
			PauseMenuAudioPreview.StopPreviewAudio();
			if (lastModeName == PauseModeName.None)
			{
				parentPauseMenu.Hide();
			}
			else
			{
				yield return parentPauseMenu.ChangeMode(lastModeName, isGoingBackToLastMode: true);
			}
			break;
		case PauseContentName.StartGame:
			skipNarration = true;
			Narration.LoadNarration();
			parentPauseMenu.Hide();
			break;
		case PauseContentName.GeneralSettings:
		{
			PauseModeName newModeName = ((base.game == null) ? PauseModeName.MainMenuGeneralSettings : PauseModeName.GameGeneralSettings);
			yield return parentPauseMenu.ChangeMode(newModeName);
			break;
		}
		case PauseContentName.AccessibilitySettings:
			yield return parentPauseMenu.ChangeMode(PauseModeName.AccessibilitySettings);
			break;
		case PauseContentName.AudioSettings:
			yield return parentPauseMenu.ChangeMode(PauseModeName.AudioSettings);
			break;
		case PauseContentName.AdvancedAudioSettings:
			PauseMenuAudioPreview.StopPreviewAudio();
			yield return parentPauseMenu.ChangeMode(PauseModeName.AdvancedAudioSettings);
			break;
		case PauseContentName.AdvancedSettings:
			yield return parentPauseMenu.ChangeMode(PauseModeName.AdvancedSettings);
			break;
		case PauseContentName.NarrationSettings:
			yield return parentPauseMenu.ChangeMode(PauseModeName.NarrationSettings);
			break;
		case PauseContentName.SoundGlossary:
			yield return parentPauseMenu.ChangeMode(PauseModeName.SoundGlossary);
			break;
		case PauseContentName.Narration:
			Persistence.SetNarration(CurrentContentData.CurrentBoolValue);
			Narration.LoadNarration(feedback: true);
			break;
		case PauseContentName.NarrationDelayed:
			Persistence.SetNarration(CurrentContentData.CurrentBoolValue);
			Narration.PlayNarrationFeedback(CurrentContentData.CurrentBoolValue);
			break;
		case PauseContentName.ForceSAPINarration:
			Persistence.SetForceSAPINarration(CurrentContentData.CurrentBoolValue);
			Narration.ToggleSAPI(CurrentContentData.CurrentBoolValue);
			break;
		case PauseContentName.RestoreDefaultSettings:
			RDBaseDllDummy.printem($"menu mode name: {modeName}, category name: {CurrentCategory.modeName}");
			if (modeName == PauseModeName.NarrationSettings)
			{
				Persistence.SetNarration(enabled: true);
				Persistence.SetSpeechRate(0);
				Persistence.SetSpeechVolume(11);
				Persistence.SetNarrateSubtitles(enabled: true);
				Persistence.SetNarrateDescriptions(enabled: true);
				Persistence.SetLevelSelectReportPosition(ReportPosition.Once);
				Persistence.SetOtherMenusReportPosition(ReportPosition.Once);
				Persistence.SetForceNarrateCustomLevels(enabled: false);
				Persistence.SetForceSAPINarration(enabled: false);
				foreach (KeyValuePair<PauseContentName, PauseModeContentArrows> item in contentArrowsDict)
				{
					PauseModeContentArrows value = item.Value;
					if (value.contentData.name == PauseContentName.Narration || value.contentData.name == PauseContentName.SpeechRate || value.contentData.name == PauseContentName.SpeechVolume || value.contentData.name == PauseContentName.NarrateSubtitles || value.contentData.name == PauseContentName.NarrateDescriptions || value.contentData.name == PauseContentName.LevelSelectReportPosition || value.contentData.name == PauseContentName.OtherMenusReportPosition || value.contentData.name == PauseContentName.ForceNarrateCustomLevels || value.contentData.name == PauseContentName.ForceSAPINarration)
					{
						value.UpdateValue();
					}
				}
				Narration.LoadNarration(feedback: true);
				Narration.ToggleSAPI(enable: false);
				parentPauseMenu.PlayStaticAnimation(0.2f);
			}
			else if (CurrentCategory.modeName == PauseModeName.AudioSettings)
			{
				PauseMenuAudioPreview.StopPreviewAudio();
				Persistence.SetGlobalVolume(7);
				Persistence.SetMusicVolume(10);
				Persistence.SetGameplayVolume(10);
				Persistence.SetInterfaceVolume(10);
				Persistence.SetDialogueVolume(10);
				Persistence.SetAmbienceVolume(7);
				Persistence.SetBeatsoundsVolume(10);
				Persistence.SetHitsoundsVolume(10);
				Persistence.SetCountingVoiceVolume(10);
				Persistence.SetCuesoundsVolume(10);
				Persistence.SetFeedbackVolume(10);
				scrGameManager.LoadSavedVolumes();
				Persistence.SetMuteInBackground(state: false);
				scrConductor.LoadSavedAudioSettings();
				foreach (KeyValuePair<PauseContentName, PauseModeContentArrows> item2 in contentArrowsDict)
				{
					PauseModeContentArrows value2 = item2.Value;
					if (value2.contentData.name == PauseContentName.Volume || value2.contentData.name == PauseContentName.MusicVolume || value2.contentData.name == PauseContentName.GameplayVolume || value2.contentData.name == PauseContentName.InterfaceVolume || value2.contentData.name == PauseContentName.DialogueVolume || value2.contentData.name == PauseContentName.AmbienceVolume || value2.contentData.name == PauseContentName.BeatsoundsVolume || value2.contentData.name == PauseContentName.HitsoundsVolume || value2.contentData.name == PauseContentName.CountingVoiceVolume || value2.contentData.name == PauseContentName.CuesoundsVolume || value2.contentData.name == PauseContentName.FeedbackVolume || value2.contentData.name == PauseContentName.MuteInBackground)
					{
						value2.UpdateValue();
					}
				}
				parentPauseMenu.PlayStaticAnimation(0.2f);
			}
			else if (CurrentCategory.modeName == PauseModeName.AdvancedAudioSettings)
			{
				PauseMenuAudioPreview.StopPreviewAudio();
				Persistence.SetPanningStrength(100);
				Persistence.SetPanningStrength2P(80);
				Persistence.SetPanningInvert(state: false);
				Persistence.SetBeatsoundPitchVariance(5);
				Persistence.SetBeatsoundPitchMode(mode: false);
				scrConductor.LoadSavedAudioSettings();
				foreach (KeyValuePair<PauseContentName, PauseModeContentArrows> item3 in contentArrowsDict)
				{
					PauseModeContentArrows value3 = item3.Value;
					if (value3.contentData.name == PauseContentName.PanningStrength || value3.contentData.name == PauseContentName.PanningStrength2P || value3.contentData.name == PauseContentName.PanningInvert || value3.contentData.name == PauseContentName.BeatsoundPitchVariance || value3.contentData.name == PauseContentName.BeatsoundPitchMode)
					{
						value3.UpdateValue();
					}
				}
				parentPauseMenu.PlayStaticAnimation(0.2f);
			}
			else
			{
				if (CurrentCategory.modeName != PauseModeName.AccessibilitySettings)
				{
					break;
				}
				Persistence.SetExtraDifficultyOptions(enabled: false);
				Persistence.SetWindowMovement(WindowMovement.Full);
				Persistence.SetAudibleSkipBeats(enabled: false);
				Persistence.SetFlashIntensity(100);
				Persistence.SetBackgroundDimming(0);
				foreach (KeyValuePair<PauseContentName, PauseModeContentArrows> item4 in contentArrowsDict)
				{
					PauseModeContentArrows value4 = item4.Value;
					if (value4.contentData.name == PauseContentName.EnableExtraDifficultyOptions || value4.contentData.name == PauseContentName.WindowMovement || value4.contentData.name == PauseContentName.ReducePixelFontUsage || value4.contentData.name == PauseContentName.AudibleSkipBeats || value4.contentData.name == PauseContentName.FlashIntensity || value4.contentData.name == PauseContentName.BackgroundDimming)
					{
						value4.UpdateValue();
					}
				}
				parentPauseMenu.PlayStaticAnimation(1f);
				ReloadFonts(reducedPixelFontUsage: false);
			}
			break;
		case PauseContentName.ResetCalibrationDialogue:
			Persistence.SetDidCalibration(didCalibration: false);
			break;
		case PauseContentName.Fullscreen:
		{
			int currentResolutionScale = contentArrowsDict[PauseContentName.Resolution].contentData.CurrentResolutionScale;
			FullScreenMode fullScreenMode = (CurrentContentData.CurrentBoolValue ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed);
			scrVfxControl.SetResolutionWithScale(currentResolutionScale, fullScreenMode);
			yield return null;
			foreach (KeyValuePair<PauseContentName, PauseModeContentArrows> item5 in contentArrowsDict)
			{
				item5.Value.UpdateValue();
			}
			break;
		}
		case PauseContentName.Resolution:
			scrVfxControl.SetResolutionWithScale(CurrentContentData.CurrentResolutionScale, Screen.fullScreenMode);
			break;
		case PauseContentName.CalibrationScreen:
			if (base.scnCurrent is scnLevelSelect)
			{
				scnCalibration.nextSceneName = "scnLevelSelect";
			}
			else if (base.scnCurrent is scnGame)
			{
				scnCalibration.nextSceneName = "scnGame";
			}
			else if (base.scnCurrent is scnMenu)
			{
				scnCalibration.nextSceneName = "scnMenu";
			}
			Debug.Log("Next scene to go to after Calibration: " + scnCalibration.nextSceneName);
			DoSceneChange(() =>
			{
				scnBase.GoToCalibration();
			});
			break;
		case PauseContentName.Language:
			if (CurrentContentData.CurrentLanguage != Persistence.GetLanguage())
			{
				RDString.ChangeLanguage(CurrentContentData.CurrentLanguage, setLanguageForVoice: false);
				parentPauseMenu.UpdateCurrentMode();
				scnBase.instance.Localize();
			}
			break;
		case PauseContentName.ReducePixelFontUsage:
		{
			bool currentBoolValue2 = CurrentContentData.CurrentBoolValue;
			if (CurrentContentData.CurrentBoolValue != Persistence.GetReducePixelFontUsage())
			{
				ReloadFonts(currentBoolValue2);
			}
			break;
		}
		case PauseContentName.InstantReducePixelFontUsage:
		{
			bool currentBoolValue = CurrentContentData.CurrentBoolValue;
			if (currentBoolValue != Persistence.GetReducePixelFontUsage())
			{
				ReloadFonts(currentBoolValue);
			}
			break;
		}
		case PauseContentName.AccessibilityScope:
		{
			AccessibilityScope currentAccessibilityScopeValue = CurrentContentData.currentAccessibilityScopeValue;
			if (currentAccessibilityScopeValue != Persistence.GetAccessibilityScope())
			{
				Persistence.SetAccessibilityScope(currentAccessibilityScopeValue);
				Persistence.generalPrefs.Save();
			}
			break;
		}
		case PauseContentName.ForceDesktopFeatureSet:
			if (CurrentContentData.CurrentBoolValue != Persistence.GetForceDesktopFeatureSet())
			{
				Persistence.SetForceDesktopFeatureSet(CurrentContentData.CurrentBoolValue);
				DoSceneChange(() =>
				{
					scnBase.GoToScene("scnMenu");
				});
			}
			break;
		case PauseContentName.ReenterFirstTimeSetup:
			ReenterFirstTimeSetup();
			break;
		case PauseContentName.HeartExplosionsGlossary:
		case PauseContentName.RowInfoCuesGlossary:
		case PauseContentName.BeatsoundsGlossary:
		case PauseContentName.HitsoundsGlossary:
		case PauseContentName.HeartCracksGlossary:
		case PauseContentName.ClassicCuesGlossary:
		case PauseContentName.OneshotCuesGlossary:
			PauseMenuAudioPreview.PlayPreviewAudio(selectedContentName, CurrentContentData.CurrentStringValue);
			break;
		default:
			Debug.Log($"The content {CurrentContentData.name} was not found inside the pause mode {modeName}");
			break;
		}
		if (parentPauseMenu.showing && !parentPauseMenu.goingToScene && parentPauseMenu.enabled)
		{
			PauseMenu.inputsLocked = false;
		}
	}

	private void ReloadFonts(bool reducedPixelFontUsage)
	{
		if (parentPauseMenu.currentModeName != PauseModeName.FirstTimeSetup)
		{
			DoSceneChange(() =>
			{
				Persistence.SetReducePixelFontUsage(reducedPixelFontUsage);
				RDString.UpdateFontPack();
				scrVfxControl.CheckForWindowDanceChanges();
				parentPauseMenu.GoToScene((base.currentSceneName == "scnEditor") ? "scnEditor" : "scnMenu");
			});
		}
		else
		{
			Persistence.SetReducePixelFontUsage(reducedPixelFontUsage);
			RDString.UpdateFontPack();
			scrVfxControl.CheckForWindowDanceChanges();
			parentPauseMenu.UpdateCurrentMode();
			scnBase.instance.Localize();
		}
	}

	private void ReenterFirstTimeSetup()
	{
		DoSceneChange(() =>
		{
			Persistence.SetForceFirstTimeSetup(enabled: true);
			RDString.UpdateFontPack();
			scrVfxControl.CheckForWindowDanceChanges();
			parentPauseMenu.GoToScene("scnLogo");
		});
	}

	public void ChangeContentValue(int direction, bool finerChange)
	{
		if (CurrentContentArrows != null)
		{
			CurrentContentArrows.ChangeContentValue(direction, finerChange);
		}
		else if (CurrentContentData.name == PauseContentName.CategorySelector)
		{
			int index = ((currentCategoryIndex + direction < categories.Count) ? ((currentCategoryIndex + direction < 0) ? (categories.Count - 1) : (currentCategoryIndex + direction)) : 0);
			ChangeCategory(categories[index].modeName);
		}
	}

	private void ChangeCategory(PauseModeName categoryName)
	{
		if (CurrentCategory.modeName == categoryName)
		{
			return;
		}
		changeIndexWithSound = false;
		SetContentIndex(0);
		int num = categories.FindIndex((Category c) => c.modeName == categoryName);
		int num2 = ((num > currentCategoryIndex) ? 1 : (-1));
		CurrentCategory.container.DOKill();
		CurrentCategory.container.DOAnchorPosX(rectTransform.sizeDelta.x * (float)num2 * -1f, 0.1f).SetEase(Ease.OutExpo).SetUpdate(isIndependentUpdate: true);
		currentCategoryIndex = num;
		PlayCategorySFX(CurrentCategory.modeName);
		if (CurrentCategory.modeName == PauseModeName.AudioSettings)
		{
			foreach (Transform item in CurrentCategory.container)
			{
				item.gameObject.SetActive(value: false);
				UnityEngine.Object.Destroy(item.gameObject);
			}
			CurrentCategory.contentsDataList = parentPauseMenu.pauseMenuData.GetModeContents(CurrentCategory.modeName);
			PauseMenuContentData pauseMenuContentData = new PauseMenuContentData();
			pauseMenuContentData.name = PauseContentName.CategorySelector;
			CurrentCategory.contentsDataList.Insert(0, pauseMenuContentData);
			CurrentCategory.container.sizeDelta = new Vector2(modeWidth, 24f * (float)CurrentCategory.contentsDataList.Count);
			CurrentCategory.contentsDict = new Dictionary<PauseContentName, GameObject>();
			CurrentCategory.contentsDict.Add(PauseContentName.CategorySelector, categorySelectorContainer);
			CurrentCategory.contentArrowsDict = new Dictionary<PauseContentName, PauseModeContentArrows>();
			SetupCategory(CurrentCategory.modeName, CurrentCategory.container, CurrentCategory.contentsDataList);
		}
		CurrentCategory.container.AnchorPosX(rectTransform.sizeDelta.x * (float)num2);
		CurrentCategory.container.DOKill();
		CurrentCategory.container.DOAnchorPosX(0f, 0.1f).SetEase(Ease.OutExpo).SetUpdate(isIndependentUpdate: true);
		CheckScrollingFunctionality();
		titleText.text = RDString.Get(CurrentCategory.localizationKey);
		UpdateSeparators();
		categorySelector.UpdateSelector();
		CheckButtonsVisibility();
		SayCurrentContent(CurrentContentData, CurrentContentIndex + 1, contentsDataList.Count - hiddenContent);
	}

	private void PlayCategorySFX(PauseModeName categoryName)
	{
		float pitch = categoryName switch
		{
			PauseModeName.DefaultCategory => RDUtils.PitchSemitonesToPercent(-6f), 
			PauseModeName.AudioSettings => RDUtils.PitchSemitonesToPercent(-2f), 
			PauseModeName.AccessibilitySettings => RDUtils.PitchSemitonesToPercent(1f), 
			PauseModeName.AdvancedSettings => RDUtils.PitchSemitonesToPercent(6f), 
			_ => 1f, 
		};
		if (base.currentSceneName == "scnEditor")
		{
			pitch = categoryName switch
			{
				PauseModeName.DefaultCategory => RDUtils.PitchSemitonesToPercent(-9f), 
				PauseModeName.GameGeneralSettings => RDUtils.PitchSemitonesToPercent(-6f), 
				PauseModeName.AudioSettings => RDUtils.PitchSemitonesToPercent(-2f), 
				PauseModeName.AccessibilitySettings => RDUtils.PitchSemitonesToPercent(1f), 
				PauseModeName.AdvancedSettings => RDUtils.PitchSemitonesToPercent(6f), 
				_ => 1f, 
			};
		}
		scrConductor.PlayImmediately("sndPagerCategory", 1f, RDUtils.GetMixerGroup("PauseMenu"), pitch, 0f, isBeatSound: false, ignoreListenerPause: true);
	}

	public bool SelectFirstContentWithName(PauseContentName contentName)
	{
		int num = contentsDataList.FindIndex(0, contentsDataList.Count, (PauseMenuContentData contentData) => contentData.name == contentName);
		if (num == -1)
		{
			return false;
		}
		if (!categories[currentCategoryIndex].contentsDict.TryGetValue(contentName, out var value) || !value.activeSelf)
		{
			return false;
		}
		if (contentName == PauseContentName.Back)
		{
			skipNarration = true;
		}
		SetContentIndex(num);
		StartCoroutine(SelectContent(num));
		return true;
	}

	private void SetCategoriesIcon(bool selected)
	{
		float num = (selected ? 1f : 0.75f);
		float alpha = (selected ? 0.2f : 0.5f);
		for (int i = 0; i < categories.Count; i++)
		{
			if (selected || i != currentCategoryIndex)
			{
				categories[i].icon.color = new Color(num, num, num, 1f);
				categories[i].iconShadow.effectColor = Color.black.WithAlpha(alpha);
			}
		}
	}

	public void ToggleContents(bool enable, params PauseContentName[] contents)
	{
		foreach (PauseContentName key in contents)
		{
			if (contentsDict.ContainsKey(key))
			{
				contentsDict[key].SetActive(enable);
			}
		}
		UpdateSeparators();
	}

	private void UpdateSeparators()
	{
		int num = 0;
		int currentActiveContentsLength = CurrentActiveContentsLength;
		foreach (GameObject separator in separators)
		{
			separator.SetActive(num < currentActiveContentsLength);
			num++;
		}
	}

	public static void CheckCJKText(Text text)
	{
		text.font = RDString.GetAppropiateFontForString(text.text);
		bool flag = text.font == RDConstants.data.latinFontPack.font;
		text.fontSize = (flag ? 8 : 7);
		text.alignByGeometry = !flag;
	}

	public void SayCurrentContent(PauseMenuContentData contentData, int itemIndex = -1, int itemsLength = -1)
	{
		if (skipNarration)
		{
			skipNarration = false;
			return;
		}
		ElementType elementType = ElementType.None;
		string empty = string.Empty;
		object obj = null;
		bool flag = true;
		NarrationActionName[] actions = new NarrationActionName[0];
		if (contentData.name == PauseContentName.CategorySelector)
		{
			itemIndex = currentCategoryIndex + 1;
			itemsLength = categories.Count;
			flag = false;
			empty = titleText.text;
			elementType = ElementType.Tab;
			actions = new NarrationActionName[3]
			{
				NarrationActionName.ToChangeHorizontal,
				NarrationActionName.ToNavigateVertical,
				NarrationActionName.ToGoBack
			};
		}
		else
		{
			empty = RDString.Get("pauseMenu." + contentData.name);
			if (parentPauseMenu.pauseMenuData.GetModeByName(modeName).useCategories)
			{
				itemIndex--;
				itemsLength--;
			}
			switch (contentData.valueType)
			{
			case PauseContentValueType.None:
			case PauseContentValueType.ModeChanger:
				actions = new NarrationActionName[3]
				{
					NarrationActionName.ToSelect,
					NarrationActionName.ToGoBack,
					NarrationActionName.ToNavigateVertical
				};
				elementType = ElementType.Button;
				break;
			case PauseContentValueType.String:
			case PauseContentValueType.Int:
			case PauseContentValueType.Bool:
			case PauseContentValueType.LanguageLocalization:
			case PauseContentValueType.Resolution:
			case PauseContentValueType.Defibrillator:
			case PauseContentValueType.WindowMovement:
			case PauseContentValueType.AccessibilityScope:
			case PauseContentValueType.NarrationReportPosition:
			case PauseContentValueType.DiscordPresence:
				actions = ((!contentData.needsConfirmation) ? new NarrationActionName[3]
				{
					NarrationActionName.ToChangeHorizontal,
					NarrationActionName.ToNavigateVertical,
					NarrationActionName.ToGoBack
				} : new NarrationActionName[4]
				{
					NarrationActionName.ToConfirm,
					NarrationActionName.ToChangeHorizontal,
					NarrationActionName.ToNavigateVertical,
					NarrationActionName.ToGoBack
				});
				elementType = ElementType.Selection;
				obj = ((!parentPauseMenu.usingOptionDetail) ? CurrentContentArrows.valueText.text : parentPauseMenu.detailValue.text.RemoveRichTextColorTags());
				break;
			default:
				Debug.LogError($"New value type {contentData.valueType} was not added");
				break;
			}
		}
		string text = ((usesLevelDetail & flag) ? parentPauseMenu.detailDescription.text.RemoveRichTextColorTags() : null);
		if (parentPauseMenu.usingOptionDetail)
		{
			text = text.Replace(obj.ToString(), "");
		}
		string text2 = (contentData.needsConfirmation ? (" " + RDString.Get("pauseMenu.levelDetail.pressToConfirm")) : null);
		Narration.Say(empty, NarrationCategory.Navigation, itemIndex, itemsLength, elementType, obj, text + text2, flipCategoryQueueBehaviour: false, actions);
	}

	private void DoSceneChange(Action action)
	{
		scnEditor editor = scnEditor.instance;
		if (editor == null)
		{
			action();
			return;
		}
		parentPauseMenu.Hide();
		editor.CheckIfNeedsToSaveFile(() =>
		{
			editor.DoSceneChange(() =>
			{
				action();
			});
		});
	}
}
