using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : RDBase
{
	public delegate void ToggleVisibility(bool show);

	public static List<PauseContentName> contentsToSelect;

	public static bool inputsLocked;

	public const string OpenSnd = "sndPagerOpen";

	public const string CloseSnd = "sndPagerClose";

	[NonSerialized]
	public bool showing;

	[NonSerialized]
	public bool goingToScene;

	[NonSerialized]
	public bool usingOptionDetail;

	[NonSerialized]
	public PauseModeName currentModeName;

	public scnLevelSelect levelSelect;

	public Camera pauseMenuCamera;

	public Canvas canvas;

	public CanvasGroup canvasGroup;

	public PauseMenuData pauseMenuData;

	public PauseModeName firstModeName;

	public GameObject modePrefab;

	public GameObject icePhone;

	public GameObject chiliPhone;

	public GameObject normalPhone;

	public Color[] screenColors;

	public Image screenBackground;

	public RectTransform phoneMask;

	public RectTransform phoneRect;

	public Image overlay;

	public ToggleVisibility toggleVisibility;

	public GameObject staticNoiseAnimation;

	public GameObject detailStaticNoiseAnimation;

	public RectTransform levelDetailContainer;

	public RequiredControllerTablet requiredControllerTablet;

	public Text detailDescription;

	public Text detailValue;

	public Text confirmationText;

	public float toggleAnimDuration;

	public Vector2 levelDetailShowPos;

	public Vector2 levelDetailHidePos;

	public Vector2 phoneShowPos;

	public Vector2 phoneHidePos;

	public Vector2 usingDetailPhonePos;

	public CanvasGroup backgroundCanvasGroup;

	public RawImage background;

	private Dictionary<PauseModeName, PauseMenuMode> instantiatedModes;

	public PauseMenuMode currentMode => instantiatedModes[currentModeName];

	public bool dogMode
	{
		get
		{
			if (base.game != null)
			{
				return base.game.currentLevel.dogMode;
			}
			return false;
		}
	}

	public void Initialize()
	{
		toggleVisibility = (bool show) =>
		{
		};
		contentsToSelect = new List<PauseContentName>();
		pauseMenuData.Initialize();
		instantiatedModes = new Dictionary<PauseModeName, PauseMenuMode>();
		currentModeName = firstModeName;
	}

	public void Update()
	{
		if ((bool)base.game && base.game.paused && !base.editorMode)
		{
			CheckTemporaryFade();
		}
		if (inputsLocked || canvasGroup.alpha < 1f)
		{
			return;
		}
		if (RDInput.downPress)
		{
			currentMode.SetContentIndex(currentMode.CurrentContentIndex + 1);
		}
		else if (RDInput.upPress)
		{
			currentMode.SetContentIndex(currentMode.CurrentContentIndex - 1);
		}
		else if (RDInput.cancelPress)
		{
			currentMode.changeIndexWithSound = false;
			if (currentModeName == firstModeName)
			{
				bool flag = currentMode.SelectFirstContentWithName(PauseContentName.Continue);
				if (!flag)
				{
					flag = currentMode.SelectFirstContentWithName(PauseContentName.Dog_Continue);
				}
				if (!flag)
				{
					flag = currentMode.SelectFirstContentWithName(PauseContentName.Back);
				}
			}
			else
			{
				currentMode.SelectFirstContentWithName(PauseContentName.Back);
			}
		}
		else if (RDInput.skipPressed && scnGame.instance != null && scnGame.instance.levelIsSkippable)
		{
			inputsLocked = true;
			currentMode.changeIndexWithSound = false;
			if (!currentMode.SelectFirstContentWithName(PauseContentName.Skip))
			{
				currentMode.SelectFirstContentWithName(PauseContentName.Dog_Skip);
			}
		}
		else if (RDInput.restartPressed && scnGame.instance != null && scnGame.instance.levelIsRestartable)
		{
			inputsLocked = true;
			currentMode.changeIndexWithSound = false;
			bool flag2 = currentMode.SelectFirstContentWithName(PauseContentName.RestartFromCheckpoint);
			if (!flag2)
			{
				flag2 = currentMode.SelectFirstContentWithName(PauseContentName.Restart);
			}
			if (!flag2)
			{
				flag2 = currentMode.SelectFirstContentWithName(PauseContentName.Dog_Restart);
			}
		}
		else if (RDInput.quitPressed)
		{
			inputsLocked = true;
			currentMode.changeIndexWithSound = false;
			if (!currentMode.SelectFirstContentWithName(PauseContentName.Quit))
			{
				currentMode.SelectFirstContentWithName(PauseContentName.Dog_Quit);
			}
		}
		else if (RDInput.anyPlayerPress)
		{
			StartCoroutine(currentMode.SelectContent());
		}
		else
		{
			int num = (RDInput.leftPress ? (-1) : (RDInput.rightPress ? 1 : 0));
			if (num != 0)
			{
				currentMode.ChangeContentValue(num, RDInput.finerControlIsPressed);
			}
		}
	}

	public IEnumerator ChangeMode(PauseModeName newModeName, bool isGoingBackToLastMode = false)
	{
		inputsLocked = true;
		if (dogMode && newModeName == PauseModeName.Game)
		{
			newModeName = PauseModeName.DogMode;
			firstModeName = PauseModeName.DogMode;
		}
		if (newModeName == PauseModeName.NarrationSettings || newModeName == PauseModeName.FirstTimeSetup)
		{
			Narration.lockToggleHotkeys = true;
		}
		else
		{
			Narration.lockToggleHotkeys = false;
		}
		if (!instantiatedModes.ContainsKey(newModeName))
		{
			GameObject obj = UnityEngine.Object.Instantiate(modePrefab, phoneMask);
			staticNoiseAnimation.transform.SetAsLastSibling();
			detailStaticNoiseAnimation.transform.SetAsLastSibling();
			PauseMenuMode component = obj.GetComponent<PauseMenuMode>();
			PauseMenuModeData modeByName = pauseMenuData.GetModeByName(newModeName);
			component.Initialize(this, modeByName, pauseMenuData);
			instantiatedModes.Add(newModeName, component);
		}
		yield return null;
		PauseModeName pauseModeName = currentModeName;
		currentModeName = newModeName;
		Narration.Clear();
		Narration.Set(instantiatedModes[newModeName].titleText.text, ElementType.VerticalMenu, currentMode.CurrentCategory.contentsDataList.Count - currentMode.hiddenContent);
		if (instantiatedModes.Count == 1 || pauseModeName == newModeName)
		{
			instantiatedModes[newModeName].Show(moveWithinOneFrame: true);
		}
		else
		{
			instantiatedModes[pauseModeName].Hide(isGoingBackToLastMode);
			instantiatedModes[newModeName].Show(moveWithinOneFrame: false, isGoingBackToLastMode);
			if (!isGoingBackToLastMode)
			{
				instantiatedModes[newModeName].lastModeName = pauseModeName;
			}
		}
		if (contentsToSelect.Count > 0)
		{
			PauseMenuMode pauseMenuMode = instantiatedModes[currentModeName];
			foreach (PauseContentName item in contentsToSelect)
			{
				int num = -1;
				for (int i = 0; i < pauseMenuMode.contentsDataList.Count; i++)
				{
					if (pauseMenuMode.contentsDataList[i].name == item)
					{
						num = i;
						break;
					}
				}
				if (num != -1)
				{
					pauseMenuMode.changeIndexWithSound = false;
					pauseMenuMode.SetContentIndex(num);
					contentsToSelect = new List<PauseContentName>();
					break;
				}
			}
		}
		pauseMenuData.GetModeByName(newModeName);
		yield return new WaitForSecondsRealtime(0.1f);
		inputsLocked = false;
	}

	public void UpdateCurrentMode()
	{
		StartCoroutine(UpdateCurrentModeCoroutine());
	}

	public IEnumerator UpdateCurrentModeCoroutine()
	{
		inputsLocked = true;
		staticNoiseAnimation.transform.SetAsLastSibling();
		staticNoiseAnimation.SetActive(value: true);
		detailStaticNoiseAnimation.transform.SetAsLastSibling();
		detailStaticNoiseAnimation.SetActive(value: true);
		int currentContentIndex = currentMode.CurrentContentIndex;
		PauseModeName lastModeName = currentMode.lastModeName;
		UnityEngine.Object.Destroy(currentMode.gameObject);
		instantiatedModes.Remove(currentModeName);
		yield return ChangeMode(currentModeName);
		confirmationText.text = RDString.Get("pauseMenu.levelDetail.pressToConfirm");
		RDStringToUIText.Apply(confirmationText);
		inputsLocked = true;
		currentMode.moveSelectorInstantly = true;
		currentMode.SetContentIndex(currentContentIndex);
		currentMode.lastModeName = lastModeName;
		yield return null;
		staticNoiseAnimation.SetActive(value: false);
		detailStaticNoiseAnimation.SetActive(value: false);
		inputsLocked = false;
	}

	public void PlayStaticAnimation(float duration)
	{
		StartCoroutine(PlayStaticAnimationCoroutine(duration));
	}

	public IEnumerator PlayStaticAnimationCoroutine(float duration)
	{
		inputsLocked = true;
		staticNoiseAnimation.transform.SetAsLastSibling();
		staticNoiseAnimation.SetActive(value: true);
		detailStaticNoiseAnimation.transform.SetAsLastSibling();
		detailStaticNoiseAnimation.SetActive(value: true);
		yield return new WaitForSecondsRealtime(duration);
		staticNoiseAnimation.SetActive(value: false);
		detailStaticNoiseAnimation.SetActive(value: false);
		inputsLocked = false;
	}

	public void Show()
	{
		base.enabled = true;
		showing = true;
		chiliPhone.SetActive(value: false);
		icePhone.SetActive(value: false);
		normalPhone.SetActive(value: false);
		staticNoiseAnimation.SetActive(value: false);
		detailStaticNoiseAnimation.SetActive(value: false);
		if (RDTime.speed > 1f)
		{
			screenBackground.color = screenColors[2];
			chiliPhone.SetActive(value: true);
		}
		else if (RDTime.speed < 1f)
		{
			screenBackground.color = screenColors[0];
			icePhone.SetActive(value: true);
		}
		else
		{
			normalPhone.SetActive(value: true);
			screenBackground.color = screenColors[1];
		}
		inputsLocked = true;
		phoneRect.DOKill();
		base.gameObject.SetActive(value: true);
		if ((bool)pauseMenuCamera)
		{
			pauseMenuCamera.enabled = true;
		}
		StartCoroutine(ChangeMode(currentModeName));
		toggleVisibility(show: true);
		scrConductor.PlayImmediatelyIgnoringTimeScale("sndPagerOpen", 1f, RDUtils.GetMixerGroup("PauseMenu"));
	}

	public void Hide()
	{
		PauseMenuModeData modeByName = pauseMenuData.GetModeByName(firstModeName);
		bool flag = false;
		showing = false;
		if (modeByName.useLevelDetail | flag)
		{
			ShowAnimated(showLevelDetail: false, useDelay: false, flag);
		}
		inputsLocked = true;
		Narration.lockToggleHotkeys = false;
		Ease ease = ((base.currentSceneName == "scnMenu") ? Ease.OutExpo : Ease.OutCubic);
		phoneRect.DOKill();
		phoneRect.DOAnchorPos(phoneHidePos, toggleAnimDuration).SetEase(ease).SetUpdate(isIndependentUpdate: true)
			.OnComplete(() =>
			{
				PauseMenuMode pauseMenuMode = instantiatedModes[firstModeName];
				pauseMenuMode.moveSelectorInstantly = true;
				pauseMenuMode.changeIndexWithSound = false;
				pauseMenuMode.SetContentIndex(0);
				currentMode.skipNarration = false;
				if ((bool)pauseMenuCamera)
				{
					pauseMenuCamera.enabled = false;
				}
				base.gameObject.SetActive(value: false);
			});
		toggleVisibility(show: false);
		scrConductor.PlayImmediatelyIgnoringTimeScale("sndPagerClose", 1f, RDUtils.GetMixerGroup("PauseMenu"));
		if (base.game != null)
		{
			for (int num = 0; num < 5; num++)
			{
				scrHandController handControllerFromInt = base.game.GetHandControllerFromInt(num);
				handControllerFromInt.leftArm.Depress();
				handControllerFromInt.rightArm.Depress();
			}
			Narration.SetInstructions("", NarrationActionName.ToPause);
		}
	}

	public string GetSettingDetailValue(PauseMenuContentData contentData, PauseModeContentArrows arrows)
	{
		if (!arrows)
		{
			usingOptionDetail = false;
			return string.Empty;
		}
		string empty = string.Empty;
		string text = arrows.valueText.text;
		PauseContentName pauseContentName = contentData.name;
		if (pauseContentName == PauseContentName.StreamerMode || pauseContentName == PauseContentName.NarrationDelayed)
		{
			empty = string.Empty;
		}
		else
		{
			if (contentData.name == PauseContentName.Resolution)
			{
				empty = contentData.CurrentLongResolution;
				usingOptionDetail = true;
				return "<color=#3594B5>" + text + ":</color> " + empty;
			}
			string arg = arrows.valueKey.Split('.').Last();
			string withCheck = RDString.GetWithCheck($"pauseMenu.optionDetail.{contentData.name}.{arg}", out var exists);
			if (exists)
			{
				usingOptionDetail = true;
				return "<color=#3594B5>" + text + ":</color> " + withCheck;
			}
		}
		usingOptionDetail = false;
		if (!string.IsNullOrEmpty(empty))
		{
			return RDString.Get("pauseMenu.levelDetail.optionChosenShort") + " <color=#3594B5>" + empty + "</color>";
		}
		return "";
	}

	public void UpdateLevelDetail(string description, string value, bool showConfirmationText)
	{
		detailDescription.text = (string.IsNullOrEmpty(value) ? description : (description + "\n\n" + value));
		detailValue.text = value;
		PauseMenuMode.CheckCJKText(detailDescription);
		confirmationText.gameObject.SetActive(showConfirmationText);
		PauseMenuMode.CheckCJKText(confirmationText);
	}

	public void ShowAnimated(bool showLevelDetail, bool useDelay, bool checkRequiredControllers = false)
	{
		float num = (useDelay ? 0.1f : 0f);
		levelDetailContainer.DOKill();
		phoneRect.DOKill();
		if (showLevelDetail)
		{
			HideRequiredController();
			phoneRect.DOAnchorPos(usingDetailPhonePos, toggleAnimDuration).SetUpdate(isIndependentUpdate: true).SetEase(Ease.OutExpo);
			levelDetailContainer.DOAnchorPos(levelDetailShowPos, toggleAnimDuration - num).SetUpdate(isIndependentUpdate: true).SetEase(Ease.OutCirc)
				.SetDelay(num)
				.OnComplete(() =>
				{
					levelDetailContainer.DOAnchorPosY(levelDetailShowPos.y - 8f, 4.9f).SetEase(Ease.InOutFlash).SetUpdate(isIndependentUpdate: true)
						.SetLoops(-1, LoopType.Yoyo);
				});
		}
		else if (checkRequiredControllers)
		{
			levelDetailContainer.DOAnchorPos(levelDetailHidePos, toggleAnimDuration).SetUpdate(isIndependentUpdate: true).SetEase(Ease.OutBack);
			phoneRect.DOAnchorPos(phoneShowPos, toggleAnimDuration - num).SetDelay(num).SetEase(Ease.OutBack, 1f)
				.SetUpdate(isIndependentUpdate: true);
			requiredControllerTablet.Show(toggleAnimDuration - num, num);
		}
		else
		{
			HideRequiredController();
			levelDetailContainer.DOAnchorPos(levelDetailHidePos, toggleAnimDuration).SetUpdate(isIndependentUpdate: true).SetEase(Ease.OutBack);
			phoneRect.DOAnchorPos(phoneShowPos, toggleAnimDuration - num).SetDelay(num).SetEase(Ease.OutBack, 1f)
				.SetUpdate(isIndependentUpdate: true);
		}
	}

	public void ShowRequiredController(PauseModeName modeName)
	{
		if (modeName == PauseModeName.LevelSelect || modeName == PauseModeName.Game)
		{
			StartCoroutine(ChangeMode(modeName));
		}
	}

	public void HideRequiredController()
	{
		requiredControllerTablet.Hide(toggleAnimDuration);
	}

	public void HighlightRequiredController()
	{
		requiredControllerTablet.HighlightRequiredController();
	}

	public void GoToScene(string scene)
	{
		goingToScene = true;
		overlay.transform.SetAsLastSibling();
		overlay.gameObject.SetActive(value: true);
		overlay.DOKill();
		overlay.DOFade(1f, 0.6f).SetEase(Ease.InCubic).SetUpdate(isIndependentUpdate: true)
			.OnComplete(() =>
			{
				scnBase.GoToScene(scene);
			});
	}

	public void CheckTemporaryFade()
	{
		PauseModeName pauseModeName = currentModeName;
		bool flag = pauseModeName == PauseModeName.Game || pauseModeName == PauseModeName.DogMode;
		if (flag && Input.GetKeyDown(KeyCode.Tab))
		{
			TemporaryFade(fade: true);
		}
		else if ((!flag && canvasGroup.alpha == 0f) || Input.GetKeyUp(KeyCode.Tab))
		{
			TemporaryFade(fade: false);
		}
	}

	public void TemporaryFade(bool fade)
	{
		canvasGroup.DOKill();
		backgroundCanvasGroup.DOKill();
		float endValue = (fade ? 0f : 1f);
		canvasGroup.DOFade(endValue, 0.15f).SetUpdate(isIndependentUpdate: true);
		backgroundCanvasGroup.DOFade(endValue, 0.2f).SetUpdate(isIndependentUpdate: true);
	}
}
