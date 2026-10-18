using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class scnLogo : scnBase
{
	public Image canvasFader;

	public Text gamerTagText;

	public Text titleText;

	public RDStringToUIText titleStringToUI;

	public Text descriptionText;

	public RDStringToUIText descriptionStringToUI;

	public Text continueText;

	public RDStringToUIText continueStringToUI;

	public Image warningIcon;

	public PauseMenu firstTimeSetupMenu;

	public Image firstTimeSetupMenuBackground;

	private const float fadeDuration = 0.5f;

	private bool lockInput;

	private bool showingFirstTimeSetup;

	private new void Awake()
	{
		base.Awake();
		scnBase._instance = this;
	}

	private void OnToggleSettings(bool show)
	{
		showingFirstTimeSetup = show;
		firstTimeSetupMenuBackground.DOKill();
		if (showingFirstTimeSetup)
		{
			firstTimeSetupMenuBackground.DOFade(0.5f, 0.15f);
			FadeMonitorContent(0f, 0f);
			return;
		}
		PlayInitialNarration();
		Persistence.SetForceFirstTimeSetup(enabled: false);
		firstTimeSetupMenuBackground.DOFade(0f, 0.15f);
		FadeMonitorContent(1f, 1f);
	}

	private void PlayInitialNarration()
	{
		Narration.Set(RDString.Get("narration.titleAndVersion").Replace("[versionNumber]", Application.version));
		Narration.Set(titleText.text);
		Narration.Set(descriptionText.text);
		Narration.Say(continueText.text, NarrationCategory.Notification, false);
		Narration.SetInstructions(continueText.text);
	}

	private void FadeMonitorContent(float target, float duration)
	{
		titleText.DOFade(target, duration);
		descriptionText.DOFade(target, duration);
		continueText.DOFade(target, duration);
		warningIcon.DOFade(target, duration);
	}

	private new void Start()
	{
		base.Start();
		bool flag = !Persistence.GetDidCalibration() || Persistence.GetForceFirstTimeSetup();
		if (flag)
		{
			Narration.ForceNarrationOn();
			firstTimeSetupMenu.Initialize();
			PauseMenu pauseMenu = firstTimeSetupMenu;
			pauseMenu.toggleVisibility = (PauseMenu.ToggleVisibility)Delegate.Combine(pauseMenu.toggleVisibility, new PauseMenu.ToggleVisibility(OnToggleSettings));
			firstTimeSetupMenu.Show();
		}
		else
		{
			PlayInitialNarration();
		}
		if (!Persistence.GetDefaultResolution())
		{
			RDBaseDllDummy.printem($"Persistence.GetFirstTimePlaying(): {Persistence.GetFirstTimePlaying()}");
			scrVfxControl.SetDefaultFullscreenResolution(RDBase.debugSettings.RunningOnSteamDeck && SteamIntegration.IsSteamInBigPictureMode);
			Persistence.SetDefaultResolution(state: true);
		}
		if (!Persistence.GetShowStartupWarnings() && !flag)
		{
			Exit();
		}
		else
		{
			canvasFader.color = Color.clear;
		}
	}

	private void Exit()
	{
		string argument = UrlProtocolHandler.GetArgument();
		if (!string.IsNullOrEmpty(argument))
		{
			scnCLS.urlProtocolArg = argument;
			RDBaseDllDummy.printem("ScnLogo: has protocol args, going to custom level select");
			scnBase.GoToCustomLevelSelect();
		}
		else
		{
			scnBase.GoToMainMenu();
		}
	}

	private new void Update()
	{
		base.Update();
		if (!Application.isFocused)
		{
			return;
		}
		bool flag = Input.anyKeyDown || Input.GetMouseButtonDown(0) || RDInput.anyPlayerPress;
		if ((!showingFirstTimeSetup && !lockInput) & flag)
		{
			lockInput = true;
			RDBase.PlaySound("sndPagerButton", 1f, RDUtils.GetMixerGroup("MainMenu"));
			canvasFader.DOFade(1f, 0.5f).OnComplete(() =>
			{
				Exit();
			});
		}
	}

	private void UpdateFont(Text text)
	{
		RDConstants data = RDConstants.data;
		if (RDString.isCJK || Persistence.GetReducePixelFontUsage())
		{
			text.font = RDString.currentLanguageFont;
			text.lineSpacing = 1f;
			EightSidedOutline component = text.gameObject.GetComponent<EightSidedOutline>();
			if ((bool)component)
			{
				float effectDistance = (float)text.fontSize / 12f;
				component.effectDistance = effectDistance;
			}
		}
		else
		{
			text.font = data.latinFontPack.font;
			text.resizeTextForBestFit = false;
		}
	}

	public override void Localize()
	{
		titleText.text = RDString.Get(titleStringToUI.key);
		descriptionText.text = RDString.Get(descriptionStringToUI.key);
		continueText.text = RDString.Get(continueStringToUI.key);
		UpdateFont(titleText);
		UpdateFont(descriptionText);
		UpdateFont(continueText);
	}
}
