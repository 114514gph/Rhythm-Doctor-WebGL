using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class RhythmStackerManager : scnBase
{
	[Header("Global")]
	public bool gameIsActive = true;

	public AudioClip music;

	[HideInInspector]
	public scnIanDesktop ianDesktop;

	[Header("Visible info")]
	public Text scoreText;

	public GameObject ianIcon;

	public Text inputPrecisionText;

	private GameObject inputPrecisionGO;

	public int minPrecisionValue;

	public int maxPrecisionValue;

	public RectTransform offsetContainer;

	public Text offsetText;

	public float offsetVisibilityTime;

	private float offsetPreviousTime;

	private const float offsetLeftPadding = 5f;

	private int score;

	private int highestScore;

	private const int iansHighscore = 5;

	private int inputPrecision;

	[Header("Tap")]
	public float bpm = 75f;

	public float songOffset;

	public float timingLeniency;

	private int currentBeat;

	private int currentBeat_v;

	public float wrongBlockShowDurationTime;

	public float timeToMoveBlocksContainer;

	public float secondBackgroundDistanceToMove;

	public float backgroundDistanceModifier;

	private float secondBackgroundDefaultDistanceToMove;

	public int scoreToMoveBackground;

	public int wrongBlockShowToggleNumber;

	public int minScoreToMoveBlocksContainer;

	public RectTransform mainBlockContainer;

	public RectTransform mainBlock;

	public RectTransform mainBlockRight;

	public RectTransform mainBlockLeft;

	public RectTransform wrongBlock;

	public RectTransform rightWall;

	public RectTransform blockMask;

	public List<RectTransform> blocks;

	public RectTransform blocksContainer;

	public RectTransform backgroundFirst;

	public RectTransform backgroundSecond;

	public RectTransform starsContainerFirst;

	public RectTransform starsContainerSecond;

	public RectTransform blackBackground;

	public RectTransform starsRespawn;

	private float starHeight;

	public GameObject starWhitePrefab;

	public GameObject starGrayPrefab;

	private IEnumerator moveMainBlock;

	private IEnumerator addBlockCoroutine;

	private IEnumerator updateHeightsCoroutine;

	private AudioSource audioSrc;

	private int lastBlockXPos;

	private int lastBlockWidth;

	[Header("Start screen")]
	public RectTransform startScreen;

	public RectTransform titleImage;

	public float maxDistanceToLastBlockXPos;

	public float distanceToMoveTitleImage;

	private bool hasMadeFirstTap;

	private float defaultTitleImageHeight;

	[Header("Game over")]
	public GameObject gameoverContainer;

	public Text hiScoreText;

	private Vector2 defaultMainBlockPos;

	private Vector2 defaultMainBlockContainerPos;

	private Vector2 defaultBlockSize;

	private Vector2 defaultMainBlockSize;

	private Vector2 defaultBlocksContainerPos;

	private Vector2 defaultStarsContainerFirstPos;

	private Vector2 defaultStarsContainerSecondPos;

	private Vector2 defaultBackgroundFirstPos;

	private Vector2 defaultBackgroundSecondPos;

	private int defaultBlockXPos;

	private bool hasLost;

	private bool forceRestart;

	[Header("Visuals")]
	public float screensToStars;

	public int numberOfStars;

	private double dspTimeAtStart;

	private float currentSongTime;

	private float currentSongTime_v;

	private Sequence titleBeatSeq;

	private bool started;

	private bool canReceiveInput;

	private int mainBlockXPos;

	private int positionsNumber;

	private float unitLength;

	private float crotchetTime => 60f / bpm;

	private float nextBeatTime => (float)currentBeat * crotchetTime;

	private float nextBeatTime_v => (float)currentBeat_v * crotchetTime;

	private int GlobalMainBlockXPos => (int)(mainBlock.anchoredPosition.x + mainBlockContainer.anchoredPosition.x);

	private float currentBeatFraction => 1f - (nextBeatTime - currentSongTime) / crotchetTime;

	private float currentBeatFraction_v => 1f - (nextBeatTime_v - currentSongTime_v) / crotchetTime;

	public float calibration_i => RDCalibration.calibration_i;

	public float calibration_v => RDCalibration.calibration_v;

	public void StartGame()
	{
		started = true;
		canReceiveInput = true;
	}

	private void UpdateLastBlockInfo()
	{
		RectTransform rectTransform = blocks[0];
		blocks.RemoveAt(0);
		blocks.Add(rectTransform);
		lastBlockXPos = (int)rectTransform.anchoredPosition.x;
		lastBlockWidth = (int)rectTransform.sizeDelta.x;
	}

	private new void Start()
	{
		base.Start();
		highestScore = Mathf.Max(Persistence.GetRhythmStackerScore(), 5);
		offsetContainer.gameObject.SetActive(value: false);
		inputPrecisionGO = inputPrecisionText.gameObject;
		offsetPreviousTime = 0f - offsetVisibilityTime;
		for (int i = 0; i < numberOfStars; i++)
		{
			GameObject obj = UnityEngine.Object.Instantiate((UnityEngine.Random.Range(0, 2) == 0) ? starGrayPrefab : starWhitePrefab, starsContainerFirst);
			float num = UnityEngine.Random.Range(0f, starsContainerFirst.sizeDelta.x);
			float num2 = UnityEngine.Random.Range(0f, starsContainerFirst.sizeDelta.y);
			obj.GetComponent<RectTransform>().anchoredPosition = new Vector2(num, num2);
		}
		for (int j = 0; j < numberOfStars; j++)
		{
			GameObject obj2 = UnityEngine.Object.Instantiate((UnityEngine.Random.Range(0, 2) == 0) ? starGrayPrefab : starWhitePrefab, starsContainerSecond);
			float num3 = UnityEngine.Random.Range(0f, starsContainerSecond.sizeDelta.x);
			float num4 = UnityEngine.Random.Range(0f, starsContainerSecond.sizeDelta.y);
			obj2.GetComponent<RectTransform>().anchoredPosition = new Vector2(num3, num4);
		}
		defaultBlocksContainerPos = blocksContainer.anchoredPosition;
		defaultMainBlockSize = mainBlockLeft.sizeDelta;
		defaultBlockXPos = (int)blocks[0].anchoredPosition.x;
		defaultBlockSize = blocks[0].sizeDelta;
		defaultTitleImageHeight = titleImage.anchoredPosition.y;
		defaultMainBlockPos = mainBlock.anchoredPosition;
		defaultMainBlockContainerPos = mainBlockContainer.anchoredPosition;
		defaultBackgroundFirstPos = backgroundFirst.anchoredPosition;
		defaultBackgroundSecondPos = backgroundSecond.anchoredPosition;
		secondBackgroundDefaultDistanceToMove = secondBackgroundDistanceToMove;
		starsContainerFirst.sizeDelta = new Vector2(blackBackground.rect.width / blocksContainer.localScale.x, blackBackground.rect.height / blocksContainer.localScale.y);
		starsContainerFirst.localPosition = new Vector3(blackBackground.rect.width / blocksContainer.localScale.x * -0.5f, blackBackground.rect.height / blocksContainer.localScale.y * screensToStars, 0f);
		starsContainerSecond.sizeDelta = new Vector2(blackBackground.rect.width / blocksContainer.localScale.x, blackBackground.rect.height / blocksContainer.localScale.y);
		starsContainerSecond.localPosition = new Vector3(blackBackground.rect.width / blocksContainer.localScale.x * -0.5f, blackBackground.rect.height / blocksContainer.localScale.y * (screensToStars + 1f), 0f);
		defaultStarsContainerFirstPos = starsContainerFirst.anchoredPosition;
		defaultStarsContainerSecondPos = starsContainerSecond.anchoredPosition;
		starHeight = starsContainerSecond.position.y - starsContainerFirst.position.y;
		starsRespawn.sizeDelta = new Vector2(starsRespawn.sizeDelta.x, blackBackground.rect.height);
		UpdateLastBlockInfo();
		moveMainBlock = MoveMainBlock();
		addBlockCoroutine = AddBlock();
		StartCoroutine(moveMainBlock);
		dspTimeAtStart = AudioSettings.dspTime + (double)(crotchetTime / 2f);
		currentSongTime = 0f - crotchetTime / 2f - songOffset;
		audioSrc = GetComponent<AudioSource>();
		audioSrc.outputAudioMixerGroup = RDUtils.GetMixerGroup("IanDesktop");
		audioSrc.clip = music;
		audioSrc.PlayScheduled(dspTimeAtStart);
		Narration.Say(RDString.Get("rhythmStacker.title") + Narration.ShortPause + RDString.Get("rhythmStacker.description"), NarrationCategory.Navigation, false);
	}

	private new void Update()
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		if (!started)
		{
			StartGame();
		}
		if (!ianDesktop)
		{
			RDInput.Update();
		}
		currentSongTime = (float)(AudioSettings.dspTime - dspTimeAtStart) - songOffset - calibration_i;
		currentSongTime_v = (float)(AudioSettings.dspTime - dspTimeAtStart) - songOffset - calibration_v;
		if (currentSongTime >= nextBeatTime)
		{
			currentBeat++;
		}
		if (currentSongTime_v >= nextBeatTime_v)
		{
			currentBeat_v++;
			if (!hasMadeFirstTap)
			{
				TitleBeat();
			}
		}
		if (canReceiveInput && !hasMadeFirstTap && RDInput.downPress)
		{
			PlaySound("sndDesktopBeepD4_6");
			inputPrecisionGO.SetActive(!inputPrecisionGO.activeSelf);
		}
		if (canReceiveInput && !RDInput.cancelPress && ((RDInput.anyPlayerPress && !Input.GetKey(KeyCode.LeftShift)) || ((bool)ianDesktop && ianDesktop.TestMouseDown())))
		{
			if (!hasMadeFirstTap)
			{
				hasMadeFirstTap = true;
				startScreen.gameObject.SetActive(value: false);
				scoreText.gameObject.SetActive(value: true);
			}
			if (hasLost)
			{
				Restart();
				return;
			}
			StopCoroutine(addBlockCoroutine);
			StartCoroutine(AddBlock());
		}
	}

	private IEnumerator MoveMainBlock()
	{
		yield return null;
		mainBlockXPos = 0;
		positionsNumber = Mathf.Abs(minPrecisionValue) + Mathf.Abs(maxPrecisionValue);
		unitLength = blockMask.sizeDelta.x / (float)positionsNumber;
		while (true)
		{
			SetMainBlockPos(currentBeatFraction_v, out mainBlockXPos);
			yield return null;
		}
	}

	private void SetMainBlockPos(float t, out int pos)
	{
		float num = (float)lastBlockXPos - mainBlockContainer.anchoredPosition.x + (float)positionsNumber * t;
		pos = (int)Math.Round(num / unitLength);
		mainBlock.anchoredPosition = new Vector2(pos, mainBlock.anchoredPosition.y);
	}

	private bool VerticalIntersectsWithLastBlock(Vector2 firstPos, Vector2 firstSize)
	{
		RectTransform rectTransform = blocks[blocks.Count - 1];
		int num = (int)Math.Round(firstPos.x);
		int num2 = num + (int)Math.Round(firstSize.x);
		int num3 = (int)Math.Round(rectTransform.anchoredPosition.x);
		int num4 = num3 + (int)Math.Round(rectTransform.sizeDelta.x);
		if ((num >= num3 && num < num4) || (num2 > num3 && num2 <= num4))
		{
			return true;
		}
		return false;
	}

	private IEnumerator AddBlock()
	{
		SetMainBlockPos(currentBeatFraction, out mainBlockXPos);
		RectTransform rectTransform = blocks[blocks.Count - 1];
		Vector2 firstPos = new Vector2((float)GlobalMainBlockXPos - mainBlockContainer.sizeDelta.x, mainBlock.anchoredPosition.y);
		Vector2 firstPos2 = new Vector2(GlobalMainBlockXPos, mainBlock.anchoredPosition.y);
		if (!forceRestart && (VerticalIntersectsWithLastBlock(firstPos2, mainBlockRight.sizeDelta) || VerticalIntersectsWithLastBlock(firstPos, mainBlockLeft.sizeDelta)))
		{
			score++;
			scoreText.text = score.ToString();
			RectTransform rectTransform2 = blocks[0];
			Vector2 anchoredPosition = new Vector2(GlobalMainBlockXPos, mainBlockContainer.anchoredPosition.y);
			int num;
			int num2;
			bool flag;
			if (VerticalIntersectsWithLastBlock(firstPos2, mainBlockLeft.sizeDelta))
			{
				rectTransform2.anchoredPosition = new Vector2(GlobalMainBlockXPos, mainBlockContainer.anchoredPosition.y);
				if (GlobalMainBlockXPos >= lastBlockXPos)
				{
					num = lastBlockXPos + lastBlockWidth - GlobalMainBlockXPos;
					num2 = GlobalMainBlockXPos - lastBlockXPos;
					anchoredPosition.x += num;
					flag = false;
				}
				else
				{
					num2 = lastBlockXPos - GlobalMainBlockXPos;
					num = lastBlockWidth - num2;
					rectTransform2.anchoredPosition = new Vector2(rectTransform2.anchoredPosition.x + (float)num2, rectTransform2.anchoredPosition.y);
					flag = true;
				}
			}
			else
			{
				anchoredPosition.x -= (int)mainBlockContainer.sizeDelta.x;
				rectTransform2.anchoredPosition = new Vector2(lastBlockXPos, mainBlockContainer.anchoredPosition.y);
				num2 = lastBlockXPos - (int)anchoredPosition.x;
				num = (int)(anchoredPosition.x + mainBlockLeft.sizeDelta.x) - lastBlockXPos;
				flag = true;
			}
			inputPrecision = num2;
			if ((float)num2 <= timingLeniency)
			{
				rectTransform2.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, mainBlockContainer.anchoredPosition.y);
				num2 = 0;
				num = lastBlockWidth;
				if (score % 10 == 0)
				{
					Narration.Say(RDString.Get("rhythmStacker.score").Replace("[score]", score.ToString()), NarrationCategory.Notification, false);
				}
			}
			else
			{
				string text = RDString.Get("rhythmStacker.mistake");
				string newValue = (flag ? RDString.Get("rhythmStacker.early") : RDString.Get("rhythmStacker.late"));
				Narration.Say(text.Replace("[timing]", newValue).Replace("[length]", Mathf.Abs(num2).ToString()).Replace("[remaining]", num.ToString())
					.Replace("[score]", score.ToString()), NarrationCategory.Navigation, false);
			}
			wrongBlock.sizeDelta = new Vector2(num2, wrongBlock.sizeDelta.y);
			wrongBlock.anchoredPosition = anchoredPosition;
			Vector2 sizeDelta = new Vector2(num, mainBlock.sizeDelta.y);
			mainBlockLeft.sizeDelta = sizeDelta;
			mainBlockRight.sizeDelta = sizeDelta;
			mainBlockLeft.sizeDelta = sizeDelta;
			rectTransform2.sizeDelta = sizeDelta;
			Vector2 anchoredPosition2 = mainBlockContainer.anchoredPosition;
			anchoredPosition2.y += mainBlock.sizeDelta.y;
			anchoredPosition2.x = (float)Math.Round(rectTransform2.anchoredPosition.x + rectTransform2.sizeDelta.x / 2f - mainBlockContainer.sizeDelta.x / 2f);
			mainBlockContainer.anchoredPosition = anchoredPosition2;
			if (flag)
			{
				inputPrecision *= -1;
			}
			inputPrecisionText.text = inputPrecision.ToString();
			UpdateLastBlockInfo();
			if (score > minScoreToMoveBlocksContainer)
			{
				StartUpdateHeightsCo();
			}
			bool wrongBlockVisibility = false;
			for (int i = 0; i < wrongBlockShowToggleNumber; i++)
			{
				wrongBlockVisibility = !wrongBlockVisibility;
				wrongBlock.gameObject.SetActive(wrongBlockVisibility);
				if (i != wrongBlockShowToggleNumber - 1)
				{
					yield return new WaitForSecondsRealtime(wrongBlockShowDurationTime);
				}
			}
			yield break;
		}
		yield return null;
		gameoverContainer.SetActive(value: true);
		bool flag2 = false;
		if (score > Persistence.GetRhythmStackerScore())
		{
			Persistence.SetRhythmStackerScore(score);
			flag2 = true;
		}
		bool flag3 = false;
		if (score > highestScore)
		{
			if (highestScore == 5)
			{
				flag3 = true;
			}
			highestScore = score;
		}
		hiScoreText.text = RDString.Get("rhythmStacker.hiScore").Replace("[score]", highestScore.ToString());
		RDStringToUIText.Apply(hiScoreText);
		ianIcon.SetActive(flag3);
		hasLost = true;
		if (!forceRestart)
		{
			PlaySound(flag3 ? "sndDesktopJingleJackpot" : (flag2 ? "sndDesktopJinglePositive" : "sndDesktopJingleNegative"));
		}
		Narration.Say(RDString.Get("rhythmStacker.gameOverScore").Replace("[score]", score.ToString()).Replace("[high score]", highestScore.ToString()), NarrationCategory.Navigation, false);
		StartUpdateHeightsCo();
		if (flag3)
		{
			_ = base.ink != null;
		}
	}

	private void StartUpdateHeightsCo()
	{
		updateHeightsCoroutine = UpdateHeights();
		StartCoroutine(updateHeightsCoroutine);
	}

	private IEnumerator UpdateHeights()
	{
		float distanceToMove = blocksContainer.localScale.y * mainBlock.sizeDelta.y;
		float targetYPos = blocksContainer.anchoredPosition.y - distanceToMove;
		float currentYPos = blocksContainer.anchoredPosition.y;
		float secondBackgroundTargetYPos = backgroundSecond.anchoredPosition.y + secondBackgroundDistanceToMove;
		float secondBackgroundCurrentYPos = backgroundSecond.anchoredPosition.y;
		float startTime = Time.realtimeSinceStartup;
		do
		{
			currentYPos = Mathf.MoveTowards(currentYPos, targetYPos, distanceToMove / timeToMoveBlocksContainer * Time.deltaTime);
			blocksContainer.anchoredPosition = new Vector2(blocksContainer.anchoredPosition.x, Mathf.Round(currentYPos));
			if (score >= scoreToMoveBackground)
			{
				secondBackgroundCurrentYPos = Mathf.MoveTowards(secondBackgroundCurrentYPos, secondBackgroundTargetYPos, secondBackgroundDistanceToMove / timeToMoveBlocksContainer * Time.deltaTime);
				backgroundSecond.anchoredPosition = new Vector2(backgroundSecond.anchoredPosition.x, Mathf.Round(secondBackgroundCurrentYPos));
			}
			yield return null;
		}
		while (Time.realtimeSinceStartup - startTime < timeToMoveBlocksContainer);
		if (score >= scoreToMoveBackground && secondBackgroundDistanceToMove >= 0f)
		{
			secondBackgroundDistanceToMove -= backgroundDistanceModifier;
		}
		blocksContainer.anchoredPosition = new Vector2(blocksContainer.anchoredPosition.x, targetYPos);
		if (score >= scoreToMoveBackground)
		{
			backgroundSecond.anchoredPosition = new Vector2(backgroundSecond.anchoredPosition.x, secondBackgroundTargetYPos);
		}
		float num = starsContainerFirst.position.y + starHeight;
		float num2 = starsContainerSecond.position.y + starHeight;
		if (num < blackBackground.position.y)
		{
			starsContainerFirst.position = starsRespawn.position;
		}
		if (num2 < blackBackground.position.y)
		{
			starsContainerSecond.position = starsRespawn.position;
		}
	}

	private void TitleBeat()
	{
		if (titleBeatSeq != null)
		{
			titleBeatSeq.Kill(complete: true);
		}
		titleImage.anchoredPosition = new Vector2(titleImage.anchoredPosition.x, defaultTitleImageHeight - distanceToMoveTitleImage);
		titleBeatSeq = DOTween.Sequence().AppendInterval(crotchetTime / 6f).AppendCallback(() =>
		{
			titleImage.anchoredPosition = new Vector2(titleImage.anchoredPosition.x, defaultTitleImageHeight);
		});
	}

	private void Restart()
	{
		StopCoroutine(moveMainBlock);
		if (updateHeightsCoroutine != null)
		{
			StopCoroutine(updateHeightsCoroutine);
		}
		gameoverContainer.SetActive(value: false);
		startScreen.gameObject.SetActive(value: true);
		scoreText.gameObject.SetActive(value: false);
		hasLost = false;
		hasMadeFirstTap = false;
		float num = (float)score * mainBlock.sizeDelta.y;
		for (int i = 0; i < blocks.Count; i++)
		{
			blocks[i].anchoredPosition = new Vector2(defaultBlockXPos, blocks[i].anchoredPosition.y - num);
			blocks[i].sizeDelta = defaultBlockSize;
		}
		blocksContainer.anchoredPosition = defaultBlocksContainerPos;
		mainBlockLeft.sizeDelta = defaultMainBlockSize;
		mainBlockRight.sizeDelta = defaultMainBlockSize;
		mainBlock.anchoredPosition = defaultMainBlockPos;
		mainBlockContainer.anchoredPosition = defaultMainBlockContainerPos;
		starsContainerFirst.anchoredPosition = defaultStarsContainerFirstPos;
		starsContainerSecond.anchoredPosition = defaultStarsContainerSecondPos;
		backgroundFirst.anchoredPosition = defaultBackgroundFirstPos;
		backgroundSecond.anchoredPosition = defaultBackgroundSecondPos;
		secondBackgroundDistanceToMove = secondBackgroundDefaultDistanceToMove;
		score = 0;
		UpdateLastBlockInfo();
		StartCoroutine(moveMainBlock);
	}

	public void RestartExternal()
	{
		if (started)
		{
			StartCoroutine(RestartCourotine());
		}
	}

	private IEnumerator RestartCourotine()
	{
		canReceiveInput = false;
		forceRestart = true;
		StopCoroutine(addBlockCoroutine);
		yield return StartCoroutine(AddBlock());
		forceRestart = false;
		Restart();
		canReceiveInput = true;
	}

	private AudioSource PlaySound(string sound, float volume = 1f, float pitch = 1f)
	{
		return scrConductor.PlayImmediately(sound, volume, RDUtils.GetMixerGroup("IanDesktop"), pitch);
	}
}
