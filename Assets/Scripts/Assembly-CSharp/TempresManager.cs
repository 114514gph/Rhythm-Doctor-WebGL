using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TempresManager : RDBase
{
	[HideInInspector]
	public scnIanDesktop ianDesktop;

	[HideInInspector]
	public bool initalized;

	public bool freeplay;

	public bool enableInput;

	public Bar[] bars;

	public Color defaultColor;

	public Color succeededColor;

	public Color firstSuccessColor;

	public float barSpacing;

	[Header("Start")]
	public Vector2 startSize;

	public float startScaleTime;

	public AnimationCurve startScaleEase;

	[Header("Tap")]
	public Vector2 tapSize;

	public float tapTime;

	public AnimationCurve tapEase;

	[Header("Failed")]
	public float changeColorTime;

	public AnimationCurve changeColorEase;

	[Header("Succeeded all")]
	public AnimationCurve firstSuccessEase;

	public AnimationCurve freeplaySuccessEase;

	public Vector2 succeededAllMinSize;

	public Vector2 succeededAllMaxSize;

	public float suceededAllTime;

	public Text scoreText;

	public Text bestText;

	[Header("Restart")]
	public float restartTime;

	public float automaticRestartThreshold;

	public AnimationCurve moveRestartEase;

	public AnimationCurve scaleRestartEase;

	public AnimationCurve colorRestartEase;

	private float tapButtonPressedTime;

	private bool isRestarting;

	private AudioSource restartSound;

	[Header("Restart Completed")]
	private int tapCount;

	private bool currentGameHasFinished;

	private bool exiting;

	private bool cheated;

	private void Awake()
	{
		scrConductor.LoadSavedAudioSettings();
	}

	private void Start()
	{
		scoreText.text = string.Empty;
		bestText.text = string.Empty;
		PreInitializeBars();
		initalized = true;
	}

	public void PreInitializeBars()
	{
		Bar[] array = bars;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].PreInitialize();
		}
	}

	public void InitializeBars()
	{
		Bar[] array = bars;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Initialize();
		}
	}

	public void CancelAnimations()
	{
		Bar[] array = bars;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].CancelAnimations();
		}
	}

	private void Update()
	{
		if (isRestarting)
		{
			if ((RDInput.anyPlayerIsPressed || Input.GetMouseButton(0)) && Time.time - tapButtonPressedTime > restartTime * automaticRestartThreshold)
			{
				Restart();
			}
			else if (RDInput.anyPlayerRelease || Input.GetMouseButtonUp(0))
			{
				StartCoroutine(RevertRestart());
			}
		}
	}

	public void Restart()
	{
		isRestarting = false;
		scoreText.text = string.Empty;
		scoreText.gameObject.SetActive(value: false);
		bestText.gameObject.SetActive(value: false);
		currentGameHasFinished = false;
		Bar[] array = bars;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetColor(defaultColor, restartTime - automaticRestartThreshold, colorRestartEase);
		}
		tapCount = 0;
		cheated = false;
	}

	public IEnumerator RevertRestart()
	{
		float timeToGoBack = Time.time - tapButtonPressedTime;
		float maxTimeLength = bars[tapCount].tapTime - bars[tapCount - 1].tapTime;
		CancelAnimations();
		restartSound.DOFadeStop(timeToGoBack);
		yield return null;
		for (int i = 0; i < bars.Length; i++)
		{
			Bar bar = bars[i];
			bar.AnimateAnchorY(bottom: true, timeToGoBack, scaleRestartEase);
			if (i != tapCount)
			{
				if (i == 0 || i > tapCount + 1)
				{
					bar.Scale(succeededAllMinSize, timeToGoBack, scaleRestartEase);
				}
				else
				{
					ScaleBarRelativeToOthers(bar, maxTimeLength, timeToGoBack, scaleRestartEase);
				}
			}
			else
			{
				bar.image.color = succeededColor;
				bar.Scale(succeededAllMaxSize, timeToGoBack, scaleRestartEase);
			}
		}
	}

	public void Tap()
	{
		if (currentGameHasFinished)
		{
			restartSound = PlaySound("sndTempres_Restart", panByBarPosition: false);
			tapButtonPressedTime = Time.time;
			isRestarting = true;
			for (int i = 0; i < bars.Length; i++)
			{
				Bar bar = bars[i];
				bar.AnimateAnchorY(bottom: false, restartTime, moveRestartEase);
				if (i != tapCount)
				{
					Vector2 anchoredPosition = new Vector2(bar.rectTransform.anchoredPosition.x, 0f);
					bar.MoveUsingAnchoredPosition(anchoredPosition, restartTime, moveRestartEase);
				}
				bar.Scale(startSize, restartTime, scaleRestartEase);
			}
			return;
		}
		bool flag = tapCount <= 1 || Time.time - bars[tapCount - 1].tapTime > bars[tapCount - 1].tapTime - bars[tapCount - 2].tapTime;
		if ((RDBase.isDev && RDInput.controlIsPressed) | flag)
		{
			if (!flag)
			{
				cheated = true;
			}
			PlaySound($"sndTempres_Hit{tapCount}", panByBarPosition: true);
			Bar obj = bars[tapCount];
			obj.image.color = succeededColor;
			obj.ScaleInstant(tapSize);
			obj.Scale(startSize, tapTime, tapEase);
			obj.tapTime = Time.time;
			if (tapCount == bars.Length - 1)
			{
				StartCoroutine(WinGame());
			}
			else
			{
				tapCount++;
			}
			return;
		}
		PlaySound($"sndTempres_Fail{tapCount}", panByBarPosition: true);
		if (freeplay)
		{
			currentGameHasFinished = true;
			enableInput = false;
			bars[tapCount].tapTime = Time.time;
			float maxTimeLength = bars[tapCount - 1].tapTime - bars[tapCount - 2].tapTime;
			for (int j = 0; j < bars.Length; j++)
			{
				Bar bar2 = bars[j];
				bar2.AnimateAnchorY(bottom: true, suceededAllTime, freeplaySuccessEase);
				if (j == 0 || j > tapCount)
				{
					bar2.Scale(succeededAllMinSize, suceededAllTime, freeplaySuccessEase);
				}
				else if (j == tapCount - 1)
				{
					bar2.Scale(succeededAllMaxSize, suceededAllTime, freeplaySuccessEase);
				}
				else
				{
					ScaleBarRelativeToOthers(bar2, maxTimeLength, suceededAllTime, freeplaySuccessEase);
				}
			}
			StartCoroutine(ActivateTapButton(suceededAllTime));
			tapCount--;
		}
		else
		{
			enableInput = false;
			for (int k = 0; k < tapCount; k++)
			{
				bars[k].SetColor(defaultColor, changeColorTime, changeColorEase);
			}
			StartCoroutine(ActivateTapButton(changeColorTime));
			tapCount = 0;
		}
	}

	private string FormatScore(float score)
	{
		return score.ToString("F3");
	}

	public IEnumerator WinGame()
	{
		enableInput = false;
		currentGameHasFinished = true;
		float maxTimeLength = bars[tapCount].tapTime - bars[tapCount - 1].tapTime;
		yield return new WaitForSecondsRealtime(1f);
		PlaySound("sndTempres_Win", panByBarPosition: false);
		float num = bars[tapCount].tapTime - bars[0].tapTime;
		float tempresScore = Persistence.GetTempresScore();
		if (!cheated && (tempresScore <= 0f || num < tempresScore))
		{
			Persistence.SetTempresScore(num);
			bestText.text = RDString.Get("tempres.newBest");
		}
		else
		{
			bestText.text = ((tempresScore <= 0f) ? "" : RDString.Get("tempres.hiScore").Replace("[score]", FormatScore(tempresScore)));
		}
		RDStringToUIText.Apply(bestText);
		if (!freeplay)
		{
			for (int i = 0; i < bars.Length; i++)
			{
				Bar obj = bars[i];
				obj.Scale(new Vector2(startSize.x, 0f), suceededAllTime / 2f, firstSuccessEase, (float)i / 20f);
				obj.SetColor(firstSuccessColor, 0.6f, changeColorEase);
			}
			if ((bool)ianDesktop)
			{
				ianDesktop.StartLoginAnimation();
			}
			yield break;
		}
		for (int j = 0; j < bars.Length; j++)
		{
			Bar bar = bars[j];
			bar.AnimateAnchorY(bottom: true, suceededAllTime, freeplaySuccessEase);
			if (j == 0)
			{
				bar.Scale(succeededAllMinSize, suceededAllTime, freeplaySuccessEase);
			}
			else if (j == tapCount)
			{
				bar.Scale(succeededAllMaxSize, suceededAllTime, freeplaySuccessEase);
			}
			else
			{
				ScaleBarRelativeToOthers(bar, maxTimeLength, suceededAllTime, freeplaySuccessEase);
			}
		}
		scoreText.text = FormatScore(bars[tapCount].tapTime - bars[0].tapTime);
		scoreText.gameObject.SetActive(value: true);
		bestText.gameObject.SetActive(value: true);
		yield return ActivateTapButton(suceededAllTime);
	}

	public void ScaleBarRelativeToOthers(Bar bar, float maxTimeLength, float duration, AnimationCurve ease)
	{
		float t = (bar.tapTime - bars[bar.index - 1].tapTime) / maxTimeLength;
		float num = Mathf.Lerp(succeededAllMinSize.y, succeededAllMaxSize.y, t);
		Vector2 size = new Vector2(succeededAllMaxSize.x, num);
		bar.Scale(size, duration, ease);
	}

	public IEnumerator ActivateTapButton(float waitTime)
	{
		yield return new WaitForSecondsRealtime(waitTime);
		if (!exiting)
		{
			enableInput = true;
		}
	}

	private AudioSource PlaySound(string soundName, bool panByBarPosition)
	{
		float pan = (panByBarPosition ? Mathf.Lerp(-0.3f, 0.3f, ((float)tapCount + 0.5f) / (float)bars.Length) : 0f);
		return scrConductor.PlayImmediately(soundName, 1f, RDUtils.GetMixerGroup("IanDesktop"), 1f, pan);
	}

	public void ExitGame()
	{
		if (!exiting)
		{
			exiting = true;
			enableInput = false;
			printe("exiting...");
			scnBase.GoToLevelSelect();
		}
	}
}
