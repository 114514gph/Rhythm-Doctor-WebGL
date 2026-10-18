using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class scnIanDesktop : scnBase
{
	private enum ComputerState
	{
		Login = 0,
		Desktop = 1,
		InAnimation = 2,
		InGame = 3,
		Quit = 4
	}

	[Header("General")]
	public RectTransform lockScreen;

	public RectTransform desktopScreen;

	public RectTransform tempresScreen;

	public RectTransform stackerScreen;

	public Image mainFader;

	[Header("Login")]
	public CanvasGroup loginCanvas;

	public RectTransform avatarLayer;

	public RectTransform welcomeLayer;

	public RectTransform loadingIcon;

	[Header("Desktop")]
	public RectTransform currentProgramSelected;

	public Button[] programButtons;

	public GameObject desktopBar;

	public GameObject programBar;

	public Button xboxProgram;

	public float loginInitialPosition;

	public float welcomeMessageDelay = 2f;

	[Header("Tempres")]
	public TempresManager tempresManager;

	public RectTransform tempresBG;

	public Image tempresBGImage;

	public GameObject tempresBar;

	[Header("Stacker")]
	public RhythmStackerManager stackerManager;

	public GameObject stackerBar;

	private AudioSource desktopAmbience;

	private ComputerState state;

	private int currentProgramIndex;

	private bool firstPress;

	private bool mouseDown;

	private float loginEndYPos;

	private new void OnApplicationFocus(bool focus)
	{
		base.OnApplicationFocus(focus);
		if (!focus && currentProgramIndex == 2 && state == ComputerState.InAnimation)
		{
			state = ComputerState.Desktop;
		}
	}

	private new void Awake()
	{
		base.Awake();
		scnBase._instance = this;
	}

	private new void Start()
	{
		base.Start();
		loginEndYPos = avatarLayer.anchoredPosition.y;
		avatarLayer.anchoredPosition = new Vector2(avatarLayer.anchoredPosition.x, loginInitialPosition);
		lockScreen.gameObject.SetActive(value: false);
		welcomeLayer.gameObject.SetActive(value: false);
		desktopScreen.gameObject.SetActive(value: false);
		tempresScreen.gameObject.SetActive(value: false);
		stackerScreen.gameObject.SetActive(value: false);
		scrConductor.instance.SetBPM(120f);
		desktopAmbience = scrConductor.instance.PlaySong("sndDesktopAmbienceStartup", RDUtils.GetMixerGroup("AmbienceParent"));
		bool ianDesktopLogin = Persistence.GetIanDesktopLogin();
		state = (ianDesktopLogin ? ComputerState.Desktop : ComputerState.Login);
		if (state == ComputerState.Login)
		{
			lockScreen.gameObject.SetActive(value: true);
		}
		else if (state == ComputerState.Desktop)
		{
			desktopScreen.gameObject.SetActive(value: true);
		}
		tempresManager.ianDesktop = this;
		stackerManager.ianDesktop = this;
	}

	private new void Update()
	{
		base.Update();
		if (state == ComputerState.Quit)
		{
			return;
		}
		if (state == ComputerState.Login)
		{
			if (RDInput.cancelPress)
			{
				ExitToHospital();
			}
			else if (RDInput.anyPlayerPress || Input.GetMouseButtonDown(0))
			{
				InputOnLogin();
			}
		}
		else if (state == ComputerState.Desktop)
		{
			if (RDInput.leftPress)
			{
				MoveBetweenPrograms(-1);
			}
			else if (RDInput.rightPress)
			{
				MoveBetweenPrograms(1);
			}
			else if (RDInput.cancelPress)
			{
				ExitToHospital();
			}
			else if (RDInput.anyPlayerPress)
			{
				SelectProgram();
			}
		}
		else if (state == ComputerState.InGame)
		{
			if (RDInput.cancelPress)
			{
				CloseProgram();
			}
			else if (RDInput.anyPlayerPress || Input.GetMouseButtonDown(0))
			{
				InputOnLogin();
			}
		}
	}

	public void InputOnLogin()
	{
		if (!firstPress && state == ComputerState.Login)
		{
			firstPress = true;
			ShowTempresLogin();
		}
		else if (tempresManager.gameObject.activeInHierarchy && tempresManager.enableInput)
		{
			tempresManager.Tap();
		}
	}

	public void OnMousePress(bool released)
	{
		mouseDown = !released;
	}

	public bool TestMouseDown()
	{
		bool result = mouseDown;
		mouseDown = false;
		return result;
	}

	private void ShowTempresLogin()
	{
		tempresScreen.gameObject.SetActive(value: true);
		PlaySound("sndDesktopBeepD5_6");
		Vector2 vector = tempresBG.localScale;
		tempresBG.localScale = Vector2.zero;
		tempresBG.gameObject.SetActive(value: true);
		DOTween.Sequence().Insert(0f, avatarLayer.DOAnchorPosY(loginEndYPos, 0.75f).SetEase(Ease.OutCubic)).Insert(0.15f, tempresBG.DOScale(vector, 1.5f).SetEase(Ease.OutExpo))
			.InsertCallback(0.4f, () =>
			{
				tempresManager.InitializeBars();
			})
			.InsertCallback(1f, () =>
			{
				tempresManager.enableInput = true;
			});
	}

	public void StartLoginAnimation()
	{
		Persistence.SetIanDesktopLogin(ianDesktopLogin: true);
		Vector2 vector = welcomeLayer.localPosition;
		CanvasGroup component = welcomeLayer.GetComponent<CanvasGroup>();
		welcomeLayer.localPosition = new Vector2(vector.x, vector.y - 10f);
		component.alpha = 0f;
		welcomeLayer.gameObject.SetActive(value: true);
		desktopScreen.gameObject.SetActive(value: true);
		loadingIcon.DOLocalRotate(new Vector3(0f, 0f, 10000f), 100f, RotateMode.FastBeyond360);
		DOTween.Sequence().Insert(0.33f, welcomeLayer.DOLocalMoveY(vector.y, 1f).SetEase(Ease.OutCubic)).Insert(0.33f, component.DOFade(1f, 1f).SetEase(Ease.OutCubic))
			.Insert(welcomeMessageDelay, loginCanvas.transform.DOLocalMoveY(-160f, 0.5f).SetEase(Ease.OutCubic))
			.Insert(welcomeMessageDelay, tempresBG.DOLocalMoveY(-180f, 0.5f).SetEase(Ease.OutCubic))
			.Insert(welcomeMessageDelay, loginCanvas.DOFade(0f, 0.5f).SetEase(Ease.OutCubic))
			.InsertCallback(welcomeMessageDelay + 0.5f, () =>
			{
				OpenDesktop();
			});
	}

	public void OpenDesktop()
	{
		if (state == ComputerState.Login)
		{
			state = ComputerState.Desktop;
			tempresScreen.gameObject.SetActive(value: false);
			lockScreen.gameObject.SetActive(value: false);
		}
	}

	public void OpenTempres()
	{
		currentProgramIndex = 1;
		MoveBetweenPrograms(0);
		if (state != ComputerState.Desktop)
		{
			return;
		}
		state = ComputerState.InAnimation;
		PlaySound("sndDesktopBeepD5_6");
		tempresBar.SetActive(value: true);
		stackerBar.SetActive(value: false);
		tempresBG.sizeDelta = new Vector2(267f, 159f);
		tempresBG.localPosition = new Vector2(0f, -4.5f);
		tempresBGImage.color = new Color(0.6f, 0.6f, 0.6f);
		tempresBG.gameObject.SetActive(value: true);
		tempresManager.enableInput = false;
		tempresManager.freeplay = true;
		if (tempresManager.initalized)
		{
			tempresManager.Restart();
			tempresManager.PreInitializeBars();
		}
		OpenProgram(tempresScreen, () =>
		{
			if (tempresManager.initalized)
			{
				tempresManager.InitializeBars();
			}
			DOTween.Sequence().InsertCallback(0.6f, () =>
			{
				tempresManager.enableInput = true;
			});
		});
	}

	public void OpenStacker()
	{
		currentProgramIndex = 0;
		MoveBetweenPrograms(0);
		if (state == ComputerState.Desktop)
		{
			state = ComputerState.InAnimation;
			PlaySound("sndDesktopBeepD5_6");
			stackerBar.SetActive(value: true);
			tempresBar.SetActive(value: false);
			OpenProgram(stackerScreen, () =>
			{
				stackerManager.gameObject.SetActive(value: true);
				stackerManager.RestartExternal();
			});
		}
	}

	public void OpenAdofai()
	{
		currentProgramIndex = 2;
		MoveBetweenPrograms(0);
		if (state == ComputerState.Desktop)
		{
			state = ComputerState.InAnimation;
			PlaySound("sndDesktopBeepD5_6");
			Application.OpenURL("https://store.steampowered.com/app/977950/A_Dance_of_Fire_and_Ice/");
		}
	}

	public void OpenProgram(RectTransform currentProgram, Action onOpenProgram = null)
	{
		desktopBar.SetActive(value: false);
		programBar.SetActive(value: true);
		currentProgram.gameObject.SetActive(value: true);
		Vector3 currentPosition = currentProgram.localPosition;
		Vector3 currentScale = currentProgram.localScale;
		currentProgram.localPosition = programButtons[currentProgramIndex].transform.localPosition;
		currentProgram.localScale = Vector3.zero;
		DOTween.Sequence().Insert(0f, currentProgram.DOLocalMove(currentPosition, 0.5f).SetEase(Ease.OutExpo)).Insert(0f, currentProgram.DOScale(currentScale, 0.5f).SetEase(Ease.OutExpo))
			.InsertCallback(0.5f, () =>
			{
				currentProgram.localScale = currentScale;
				currentProgram.localPosition = currentPosition;
				onOpenProgram?.Invoke();
				state = ComputerState.InGame;
			});
	}

	public void CloseProgram()
	{
		if (state == ComputerState.InGame)
		{
			state = ComputerState.InAnimation;
			PlaySound("sndDesktopBeepD4_6");
			desktopBar.SetActive(value: true);
			programBar.SetActive(value: false);
			RectTransform currentProgram;
			if (currentProgramIndex == 1)
			{
				currentProgram = tempresScreen;
			}
			else
			{
				currentProgram = stackerScreen;
			}
			Vector3 currentPosition = currentProgram.localPosition;
			Vector3 localPosition = programButtons[currentProgramIndex].transform.localPosition;
			DOTween.Sequence().Insert(0f, currentProgram.DOLocalMove(localPosition, 0.5f).SetEase(Ease.OutExpo)).Insert(0f, currentProgram.DOScale(Vector3.zero, 0.5f).SetEase(Ease.OutExpo))
				.InsertCallback(0.5f, () =>
				{
					currentProgram.localScale = Vector3.one;
					currentProgram.localPosition = currentPosition;
					currentProgram.gameObject.SetActive(value: false);
					state = ComputerState.Desktop;
				});
		}
	}

	public void ExitToHospital()
	{
		if (state != ComputerState.Quit)
		{
			state = ComputerState.Quit;
			mainFader.enabled = true;
			mainFader.color = Color.clear;
			desktopAmbience.DOFade(0f, 1f);
			mainFader.DOFade(1f, 1f).SetEase(Ease.OutExpo).OnComplete(() =>
			{
				scnBase.GoToLevelSelect();
			});
		}
	}

	private void MoveBetweenPrograms(int direction)
	{
		if (direction != 0)
		{
			PlaySound("sndDesktopBeepD4_6");
		}
		currentProgramIndex = Mathf.Clamp(currentProgramIndex + direction, 0, programButtons.Length - 1);
		currentProgramSelected.localPosition = programButtons[currentProgramIndex].transform.localPosition;
	}

	private void SelectProgram()
	{
		programButtons[currentProgramIndex].onClick.Invoke();
	}

	private AudioSource PlaySound(string sound, float volume = 1f, float pitch = 1f)
	{
		return scrConductor.PlayImmediately(sound, volume, RDUtils.GetMixerGroup("IanDesktop"), pitch);
	}
}
