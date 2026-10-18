using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class scrWrldMonthlyLevel : scnBase
{
	private const int CanvasWidth = 352;

	private const int CanvasHeight = 198;

	private const int StartOption = 0;

	private const int CreditsOption = 1;

	private const int ChangeResolutionOption = 2;

	private const int QuitOption = 3;

	private GameObject mScenes;

	private GameObject mIntroScene;

	private GameObject mPlayersScene;

	private GameObject mOnePlayerScene;

	private GameObject mTwoPlayerScene;

	private GameObject mEnterLevelScene;

	private GameObject mCreditsScene;

	private GameObject mQuitScene;

	private GameObject mCurrentScene;

	private string mDefaultInstructions = "";

	private bool mIsTransitioning;

	private bool mIsEnteringLevel;

	private TextSwitch mTextSwitch;

	private GameObject mPressSpaceToStart;

	private GameObject mLeftRightTip;

	private int mOnePlayerLevelSelected;

	private List<GameObject> mOnePlayerLevels = new List<GameObject>();

	private int mTwoPlayerLevelSelected;

	private List<GameObject> mTwoPlayerLevels = new List<GameObject>();

	private Canvas mCanvas;

	private GameObject mQuitPanel;

	private Color NotSelectedColor;

	private Color SelectedColor;

	private GameObject mOnePlayerButton;

	private GameObject mTwoPlayersButton;

	private int mPlayersModeSelected;

	private bool mFirstFrame = true;

	private bool mPlayerHasPressedButton;

	public tk2dSpriteAnimator animCredits;

	public tk2dSpriteAnimator characterOfTheMonth;

	public Text nextReleaseDate;

	public TextMesh subtitleIntroText;

	private new void Start()
	{
		base.Start();
		DOTween.logBehaviour = LogBehaviour.Default;
		wrldCamera.backgroundColor = Color.black;
		base.conductor.SetBPM(82f);
		base.conductor.crotchetsPerBar = 4;
		base.conductor.OnPreBar();
		scrConductor.PlayImmediately("sndScribble");
		mScenes = GameObject.Find("Scenes");
		mIntroScene = GameObject.Find("Intro");
		mPlayersScene = GameObject.Find("Players");
		mOnePlayerScene = GameObject.Find("OnePlayer");
		mTwoPlayerScene = GameObject.Find("TwoPlayer");
		mEnterLevelScene = GameObject.Find("EnterLevel");
		mCreditsScene = GameObject.Find("Credits");
		mQuitScene = GameObject.Find("Quit");
		mQuitPanel = GameObject.Find("QuitPanel");
		mCanvas = GameObject.Find("Canvas").GetComponent<Canvas>();
		mPressSpaceToStart = GameObject.Find("Press space to start");
		mLeftRightTip = GameObject.Find("Left right tip");
		mTextSwitch = GameObject.Find("TextSwitch").GetComponent<TextSwitch>();
		mDefaultInstructions = mLeftRightTip.GetComponent<TextMesh>().text;
		mTextSwitch.gameObject.SetActive(value: false);
		mLeftRightTip.gameObject.SetActive(value: false);
		Vector2 referenceResolution = mCanvas.GetComponent<CanvasScaler>().referenceResolution;
		Vector2 vector = wrldCamera.RDFullNativeResolution();
		float num = vector.x;
		float num2 = vector.y;
		mPlayersScene.transform.localPosition = new Vector3(0f, 0f - num2, 0f);
		mOnePlayerScene.transform.localPosition = new Vector3(0f - num, 0f - num2, 0f);
		mTwoPlayerScene.transform.localPosition = new Vector3(num, 0f - num2, 0f);
		mEnterLevelScene.transform.localPosition = new Vector3(0f, num2, 0f);
		mCreditsScene.transform.localPosition = new Vector3(0f - num, 0f, 0f);
		mQuitScene.transform.localPosition = new Vector3(num, 0f, 0f);
		mQuitPanel.GetComponent<RectTransform>().localPosition = new Vector3(referenceResolution.x, 0f, 0f);
		mCurrentScene = mIntroScene;
		mOnePlayerButton = GameObject.Find("OnePlayerButton");
		mTwoPlayersButton = GameObject.Find("TwoPlayersButton");
		NotSelectedColor = new Color(1f, 1f, 1f, 0.5f);
		SelectedColor = Color.white;
		for (int i = 0; i < 3; i++)
		{
			mOnePlayerLevels.Add(GameObject.Find("Level1P_" + i));
			mTwoPlayerLevels.Add(GameObject.Find("Level2P_" + i));
		}
		RDBase.Vfx.hudCameraOverlay.gameObject.SetActive(value: false);
		nextReleaseDate.text = nextReleaseDate.text.Replace("[date_is_autoinserted_here]", GC.NextReleaseDate);
		subtitleIntroText.text = subtitleIntroText.text.Replace("[date_is_autoinserted_here]", GC.CurrentReleaseMonth);
	}

	private new void Update()
	{
		base.Update();
		if (mFirstFrame)
		{
			RDBase.Vfx.matrixMode = true;
			RDBase.Vfx.matrixModeColor = Color.black;
			DOTween.ToAlpha(() => RDBase.Vfx.matrixModeColor, (Color x) =>
			{
				RDBase.Vfx.matrixModeColor = x;
			}, 0f, 0.8f);
			mFirstFrame = false;
		}
		if (RDInput.anyPlayerPress && mCurrentScene != mQuitScene)
		{
			scrConductor.PlayImmediately("sndHammer", 0.2f);
			if (!mPlayerHasPressedButton)
			{
				mPressSpaceToStart.SetActive(value: false);
				mLeftRightTip.SetActive(value: true);
				mTextSwitch.gameObject.SetActive(value: true);
				mPlayerHasPressedButton = true;
				return;
			}
			MonoBehaviour.print("p1press " + mCurrentScene.name);
			if (mCurrentScene == mIntroScene)
			{
				MonoBehaviour.print("mintro");
				if (mTextSwitch.currentOption == 0)
				{
					TransitionInto(mEnterLevelScene);
				}
				else if (mTextSwitch.currentOption == 1)
				{
					animCredits.SetFrame(0);
					TransitionInto(mCreditsScene);
				}
				else if (mTextSwitch.currentOption == 2)
				{
					int num = scrVfxControl.CycleWindowResolutions();
					mLeftRightTip.GetComponent<TextMesh>().text = num + "x scale";
				}
				else if (mTextSwitch.currentOption == 3)
				{
					TransitionInto(mQuitScene);
				}
			}
			else if (mCurrentScene == mEnterLevelScene)
			{
				if (!mIsTransitioning && !mIsEnteringLevel)
				{
					mIsEnteringLevel = true;
					EnterToLevel(GC.MonthlyLevelToGo);
				}
			}
			else if (mCurrentScene == mPlayersScene)
			{
				if (mPlayersModeSelected == 0)
				{
					TransitionInto(mOnePlayerScene);
				}
				else
				{
					TransitionInto(mTwoPlayerScene);
				}
			}
			else if (mCurrentScene == mCreditsScene)
			{
				if (animCredits.currentStaticFrame <= 0)
				{
					animCredits.SetFrame(animCredits.currentStaticFrame + 1);
				}
				else
				{
					TransitionInto(mIntroScene);
				}
			}
			else if (mCurrentScene == mQuitScene && !mIsTransitioning)
			{
				TransitionInto(mIntroScene);
			}
		}
		else if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (mIsEnteringLevel)
			{
				return;
			}
			if (mCurrentScene == mEnterLevelScene || mCurrentScene == mCreditsScene)
			{
				TransitionInto(mIntroScene);
			}
			else if (mCurrentScene == mQuitScene)
			{
				TransitionInto(mIntroScene);
			}
		}
		if (mCurrentScene == mPlayersScene)
		{
			if (RDInput.leftPress || RDInput.rightPress)
			{
				mPlayersModeSelected = (mPlayersModeSelected + 1) % 2;
			}
			tk2dSprite component = mOnePlayerButton.GetComponent<tk2dSprite>();
			tk2dSprite component2 = mTwoPlayersButton.GetComponent<tk2dSprite>();
			if (mPlayersModeSelected == 0)
			{
				component.color = SelectedColor;
				component2.color = NotSelectedColor;
			}
			else
			{
				component.color = NotSelectedColor;
				component2.color = SelectedColor;
			}
		}
		if (mCurrentScene == mIntroScene)
		{
			if (RDInput.leftPress || RDInput.rightPress)
			{
				mLeftRightTip.GetComponent<TextMesh>().text = mDefaultInstructions;
			}
			if (Input.GetKeyDown(KeyCode.K))
			{
				scrConductor.PlayImmediately("sndOah");
			}
			if (Input.GetKeyDown(KeyCode.L))
			{
				EnterToLevel(Level.Lean);
			}
		}
		if (!(mCurrentScene == mOnePlayerScene) && !(mCurrentScene == mTwoPlayerScene))
		{
			return;
		}
		bool flag = false;
		int num2 = 0;
		if (RDInput.leftPress)
		{
			num2 = -1;
			flag = true;
		}
		else if (RDInput.rightPress)
		{
			num2 = 1;
			flag = true;
		}
		else if (RDInput.anyPlayerPress)
		{
			Level level = Level.None;
			if (!mIsTransitioning && level != Level.None)
			{
				scnBase.GoToLevelWithEnum(level);
			}
		}
		if (flag)
		{
			if (mCurrentScene == mOnePlayerScene)
			{
				mOnePlayerLevelSelected = (mOnePlayerLevelSelected + mOnePlayerLevels.Count + num2) % mOnePlayerLevels.Count;
			}
			if (mCurrentScene == mTwoPlayerScene)
			{
				mTwoPlayerLevelSelected = (mTwoPlayerLevelSelected + mTwoPlayerLevels.Count + num2) % mTwoPlayerLevels.Count;
			}
		}
		for (int num3 = 0; num3 < mOnePlayerLevels.Count; num3++)
		{
			mOnePlayerLevels[num3].GetComponent<tk2dSprite>().color = ((num3 == mOnePlayerLevelSelected) ? SelectedColor : NotSelectedColor);
		}
		for (int num4 = 0; num4 < mTwoPlayerLevels.Count; num4++)
		{
			mTwoPlayerLevels[num4].GetComponent<tk2dSprite>().color = ((num4 == mTwoPlayerLevelSelected) ? SelectedColor : NotSelectedColor);
		}
	}

	private void EnterToLevel(Level level, bool playSound = true)
	{
		mIsTransitioning = true;
		if (playSound)
		{
			scrConductor.PlayImmediately("sndMenuSelect");
		}
		RDBase.Vfx.matrixMode = true;
		RDBase.Vfx.matrixModeColor = new Color(1f, 1f, 1f, 1f);
		GameObject.Find("Canvas").SetActive(value: false);
		DOTween.ToAlpha(() => RDBase.Vfx.matrixModeColor, (Color x) =>
		{
			RDBase.Vfx.matrixModeColor = x;
		}, 0f, 1f).OnComplete(() =>
		{
			RDBase.Vfx.matrixModeColor = Color.clear;
			DOTween.ToAlpha(() => RDBase.Vfx.matrixModeColor, (Color x) =>
			{
				RDBase.Vfx.matrixModeColor = x;
			}, 1f, 2f).OnComplete(() =>
			{
				scnBase.GoToLevelWithEnum(level);
			});
		});
	}

	private void TransitionInto(GameObject scene)
	{
		Vector3 endValue = new Vector3(0f - scene.transform.localPosition.x, 0f - scene.transform.localPosition.y, 300f);
		mCurrentScene = scene;
		mScenes.transform.DOLocalMove(endValue, 1f, snapping: true).OnComplete(() =>
		{
			mIsTransitioning = false;
		});
		RectTransform component = mQuitPanel.GetComponent<RectTransform>();
		component.DOKill();
		float num = Mathf.Sign(endValue.x);
		if (endValue.x == 0f)
		{
			num = 0f;
		}
		float num2 = Mathf.Sign(endValue.y);
		if (endValue.y == 0f)
		{
			num2 = 0f;
		}
		Vector2 referenceResolution = mCanvas.GetComponent<CanvasScaler>().referenceResolution;
		Vector3 endValue2 = new Vector3((num + 1f) * referenceResolution.x, num2 * referenceResolution.y, 0f);
		component.DOLocalMove(endValue2, 1f, snapping: true);
		mIsTransitioning = true;
	}

	public void TransitionFromQuitToIntroScene()
	{
		TransitionInto(mIntroScene);
	}

	public override void OnPreBar()
	{
		base.OnPreBar();
		if ((base.conductor.barNumber - 1) % 17 == 0)
		{
			MonoBehaviour.print("playing intro2");
			base.conductor.PlaySong("sndIntro2");
		}
	}

	public void GoToGreenlight()
	{
		MonoBehaviour.print("going to greenlight!");
		Application.OpenURL("http://rhythmdr.com/greenlight");
	}
}
