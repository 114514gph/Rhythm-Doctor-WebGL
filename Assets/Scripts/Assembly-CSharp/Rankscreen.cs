using System;
using System.Collections;
using System.IO;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Rankscreen : RDBase
{
	private const float minSecondsOnGameOverToExit = 1f;

	public GameObject rankscreen;

	public Text header;

	public Text rank;

	public Text description;

	public Text customText;

	public Text extraCredits;

	public Sprite[] easySmallHandSprites;

	public Sprite[] hardSmallHandSprites;

	public Image smallHand;

	public Text SkippableLabel;

	public int skippableTextRoom = -1;

	public Image vividStasisRank;

	public VerticalLayoutGroup descriptionLayoutGroup;

	public HorizontalLayoutGroup multiplayerResultsLayoutGroup;

	public Text resultsSingleplayer;

	public Text resultsP1;

	public Text resultsP2;

	private Rank mRank;

	private bool previousFrameWasSkippable;

	private float secondsToExit = -100000f;

	private bool exiting;

	private bool closeBlades;

	private float rankscreenShowReferenceTime;

	private string levelToJump;

	private float timeToNarrate;

	private bool narratedPress;

	private float recordingAdvanceTimer;

	private const float recordingAdvanceTimerTrigger = 1f;

	private int trueGameover;

	private int autoGameover;

	private const float SkippableLabelMargin = 0.1f;

	private bool isAnySRank
	{
		get
		{
			if ((int)mRank != 5 && (int)mRank != 15)
			{
				return (int)mRank == -15;
			}
			return true;
		}
	}

	public void Setup()
	{
		Font currentLanguageFont = RDString.currentLanguageFont;
		Text text = header;
		Text text2 = description;
		Font font = (customText.font = currentLanguageFont);
		Font font3 = (text2.font = font);
		text.font = font3;
		Text text3 = resultsSingleplayer;
		Text text4 = resultsP1;
		font = (resultsP2.font = currentLanguageFont);
		font3 = (text4.font = font);
		text3.font = font3;
		rankscreen.SetActive(value: false);
		header.gameObject.SetActive(value: false);
		rank.gameObject.SetActive(value: false);
		description.gameObject.SetActive(value: false);
		smallHand.gameObject.SetActive(value: false);
		customText.gameObject.SetActive(value: false);
		descriptionLayoutGroup.gameObject.SetActive(value: false);
		multiplayerResultsLayoutGroup.gameObject.SetActive(value: false);
		resultsSingleplayer.gameObject.SetActive(value: false);
		resultsP1.gameObject.SetActive(value: false);
		resultsP2.gameObject.SetActive(value: false);
		narratedPress = true;
	}

	public void UpdateFontSizes()
	{
		bool isCJK = RDString.isCJK;
		customText.fontSize = (isCJK ? 16 : 24);
		header.fontSize = (isCJK ? 12 : 16);
		description.fontSize = (isCJK ? 8 : 12);
		resultsSingleplayer.fontSize = (isCJK ? 6 : 8);
		resultsP1.fontSize = (isCJK ? 6 : 8);
		resultsP2.fontSize = (isCJK ? 6 : 8);
	}

	public void SetSkippableLabelAnchor(TextAnchor anchor, Vector2 offset = default(Vector2))
	{
		SkippableLabel.alignment = anchor;
		RectTransform rectTransform = SkippableLabel.rectTransform;
		Vector2 textAnchorPivot = Text.GetTextAnchorPivot(anchor);
		rectTransform.anchorMin = textAnchorPivot + offset;
		rectTransform.anchorMax = textAnchorPivot + offset;
		rectTransform.pivot = GetSkippableLabelPivot();
	}

	private Vector2 GetSkippableLabelPivot()
	{
		RectTransform rectTransform = SkippableLabel.rectTransform;
		return new Vector2((base.game.gameState == GameState.CutsceneSkippable) ? rectTransform.anchorMin.x : (1.1f - 1.2f * rectTransform.anchorMin.x), rectTransform.anchorMin.y);
	}

	private void Update()
	{
		if (base.game.pauseMenu.showing)
		{
			return;
		}
		if (1 <= trueGameover)
		{
			recordingAdvanceTimer += Time.deltaTime;
		}
		if (base.game.currentLevel.skippableRankScreen)
		{
			if (RDInput.anyPlayerPress || (DebugSettings.instance.RecordingMode && recordingAdvanceTimer >= 1f))
			{
				if (1 <= trueGameover && trueGameover <= 3)
				{
					AdvanceGameover(isPlayer: true);
					recordingAdvanceTimer = 0f;
				}
			}
			else if (!narratedPress && Time.time > timeToNarrate)
			{
				narratedPress = true;
				if (trueGameover == 1)
				{
					Narration.Say(RDString.Get("calibration.input.continue"), NarrationCategory.Notification, false);
				}
				else if (trueGameover == 2)
				{
					Narration.Say(RDString.Get("calibration.input.continue"), NarrationCategory.Notification, false);
				}
			}
		}
		bool flag = base.game.gameState == GameState.CutsceneSkippable;
		if (flag != previousFrameWasSkippable)
		{
			RectTransform rectTransform = SkippableLabel.rectTransform;
			rectTransform.DOKill();
			rectTransform.DOPivot(GetSkippableLabelPivot(), 2f).SetUpdate(isIndependentUpdate: true);
			if (flag)
			{
				LEDSign.status = "";
			}
			previousFrameWasSkippable = flag;
		}
		if (!base.editorMode && !exiting && trueGameover == 4)
		{
			secondsToExit -= Time.unscaledDeltaTime;
			if (secondsToExit <= 0f)
			{
				ShowSmallHand();
				if (Time.unscaledTime - rankscreenShowReferenceTime > 1f)
				{
					closeBlades = true;
					exiting = true;
					StartCoroutine(ExitLevelCoroutine());
				}
			}
		}
		float b = 0f;
		float a = 0f;
		if (skippableTextRoom > -1)
		{
			Transform transform = base.game.rooms[skippableTextRoom].renderQuad.transform;
			b = transform.position.y + transform.lossyScale.y / 2f - RDBase.Vfx.RDHeight;
			a = transform.position.y - transform.lossyScale.y / 2f;
		}
		SkippableLabel.rectTransform.anchoredPosition = SkippableLabel.rectTransform.anchoredPosition.WithY(Mathf.Lerp(a, b, SkippableLabel.rectTransform.anchorMin.y));
	}

	public void AdvanceGameover(bool isPlayer = false)
	{
		if (base.game.failedLevel || trueGameover >= 4)
		{
			return;
		}
		if (isPlayer)
		{
			trueGameover++;
		}
		else
		{
			autoGameover++;
			if (trueGameover >= autoGameover)
			{
				return;
			}
			trueGameover++;
		}
		if (trueGameover == 1)
		{
			if (base.game.windowChoreographer is RealWindowChoreographer realWindowChoreographer)
			{
				realWindowChoreographer.Cancel();
				WindowChoreographer.playerWindow.ResetPosition(realWindowChoreographer);
			}
			rankscreenShowReferenceTime = Time.unscaledTime;
		}
		if (scnGame.levelToLoadSource == LevelSource.CutscenesPath)
		{
			if (trueGameover == 3)
			{
				scnBase.GoToLevelSelect();
			}
			return;
		}
		bool skipRankText = base.game.currentLevel.skipRankText;
		if (trueGameover == 1 && !skipRankText)
		{
			ShowHeaderRankText();
		}
		else if (trueGameover == 2)
		{
			ShowAndSaveRank(base.game.currentLevel?.failedLevel ?? false, skipRankText);
		}
		else if (trueGameover == 3 && !skipRankText)
		{
			ShowRankDescription();
		}
	}

	public void ShowExtraCredits(string text, float lineSpacing = 2f)
	{
		extraCredits.gameObject.SetActive(value: true);
		int num = (GC.twoPlayerMode ? (-50) : (-34));
		extraCredits.rectTransform.anchoredPosition = new Vector2(extraCredits.rectTransform.anchoredPosition.x, num);
		extraCredits.text = text;
		extraCredits.font = RDString.GetAppropiateFontForString(extraCredits.text);
		extraCredits.color = extraCredits.color.WithAlpha(0f);
		extraCredits.lineSpacing = lineSpacing;
		Narration.Say(text, NarrationCategory.Notification, false);
		extraCredits.DOFade(1f, base.game.statusText.animDuration).SetEase(Ease.Linear).SetUpdate(isIndependentUpdate: true);
	}

	public void HideExtraCredits()
	{
		extraCredits.DOFade(0f, base.game.statusText.animDuration).SetEase(Ease.Linear).SetUpdate(isIndependentUpdate: true);
	}

	public void ShowSmallHand()
	{
		if (!smallHand.gameObject.activeSelf)
		{
			SpriteAnimation component = smallHand.GetComponent<SpriteAnimation>();
			Image component2 = smallHand.GetComponent<Image>();
			if (scnGame.p1DefibMode > DefibMode.Normal)
			{
				component2.sprite = hardSmallHandSprites[1];
				component.currentAnimationData.sprites = hardSmallHandSprites;
			}
			if (scnGame.p1DefibMode < DefibMode.Normal)
			{
				component2.sprite = easySmallHandSprites[1];
				component.currentAnimationData.sprites = easySmallHandSprites;
			}
			smallHand.gameObject.SetActive(value: true);
			RectTransform component3 = smallHand.GetComponent<RectTransform>();
			component3.AnchorPosX(50f);
			component3.DOAnchorPosX(3f, 0.3f).SetEase(Ease.OutExpo);
			if (Narration.IsEnabled)
			{
				RDBase.PlaySound("sndPressToContinue", 1f, RDUtils.GetMixerGroup("InterfaceSoundsParent"));
			}
		}
	}

	public void ShowHeaderRankText()
	{
		UpdateFontSizes();
		base.game.handController0.Show(show: false);
		rankscreen.SetActive(value: true);
		LevelType levelType = base.game.currentLevel.levelType;
		if (levelType != LevelType.Boss && levelType != LevelType.Challenge)
		{
			header.gameObject.SetActive(value: true);
			Narration.Say(RDString.Get(header.GetComponent<RDStringToUIText>().key), NarrationCategory.Notification, false);
			narratedPress = false;
			timeToNarrate = Time.time + 5f;
		}
		rank.gameObject.SetActive(value: false);
		description.gameObject.SetActive(value: false);
		base.game.statusText.gameObject.SetActive(value: false);
		descriptionLayoutGroup.gameObject.SetActive(value: false);
		multiplayerResultsLayoutGroup.gameObject.SetActive(value: false);
		resultsSingleplayer.gameObject.SetActive(value: false);
		resultsP1.gameObject.SetActive(value: false);
		resultsP2.gameObject.SetActive(value: false);
	}

	public void ShowAndSaveRank(bool bossLevelFailed = false, bool onlySavePersistence = false)
	{
		mRank = base.game.currentLevel.GetRankFromMistakes();
		LevelType levelType = base.game.currentLevel.levelType;
		if (!base.editorMode)
		{
			if (bossLevelFailed)
			{
				mRank = 0;
			}
			string levelIdentifier = base.game.levelIdentifier;
			scnGame.instance.IncrementLevelTries();
			if (scnGame.levelToLoadSource != LevelSource.ExternalPath)
			{
				SpeedrunValues.rank = mRank;
				if (!base.game.currentLevel.dogMode && levelType != LevelType.Tutorial)
				{
					Persistence.SetLastPlayedLevel(levelIdentifier);
				}
				switch (levelType)
				{
				case LevelType.Boss:
					if (!bossLevelFailed)
					{
						bool perfectRun = base.game.mistakesManager.mistakes == 0f;
						SaveCurrentBossLevelAsPassed(perfectRun);
					}
					break;
				default:
					if (!base.game.currentLevel.dogMode)
					{
						Persistence.SetLevelRank(levelIdentifier, mRank);
					}
					break;
				case LevelType.Tutorial:
				case LevelType.Intro:
					break;
				}
				if (base.game.currentLevel.useScore)
				{
					Persistence.SetLevelScore(levelIdentifier, base.game.currentLevel.i1);
				}
			}
			else
			{
				string empty = string.Empty;
				if (scnCLS.CachedData.levelFileData == null)
				{
					string text = new DirectoryInfo(Path.GetDirectoryName(scnGame.currentLevelPath)).Name;
					empty = RDUtils.GetHash(text);
				}
				else
				{
					empty = scnCLS.CachedData.levelFileData.hash;
				}
				Rank oldRank = Persistence.GetCustomLevelRank(empty, scnGame.levelSpeed);
				if (Persistence.IsBetterRank(mRank, oldRank))
				{
					Persistence.SetCustomLevelRank(empty, mRank, scnGame.levelSpeed);
				}
				if (base.game.currentLevel.useScore)
				{
					Persistence.SetLevelScore(empty, base.game.currentLevel.i1);
				}
			}
			SpeedrunValues.currentGameState = GameState.RankShown;
		}
		if (onlySavePersistence)
		{
			return;
		}
		Color color = ((RDTime.speed > 1f) ? "ED7070".HexToColor() : ((RDTime.speed < 1f) ? "70D8ED".HexToColor() : Color.white));
		switch (levelType)
		{
		case LevelType.Boss:
		{
			string customGameoverHeader = base.game.currentLevel.customGameoverHeader;
			if (customGameoverHeader != null)
			{
				customText.text = customGameoverHeader;
			}
			else
			{
				int bossActNum = base.game.currentLevel.bossActNum;
				string text2 = ((bossActNum >= 0) ? $"rankscreen.act{bossActNum}" : "rankscreen.boss");
				bool passedLevelWithoutCheckpoints = base.game.GetPassedLevelWithoutCheckpoints();
				bool perfected = isAnySRank;
				if ((base.game.currentLevel is Level_Montage2) & perfected)
				{
					perfected = Level.Montage.GetRank().perfected;
				}
				string text3 = (perfected ? "Perfect" : (passedLevelWithoutCheckpoints ? "CompletePlus" : "Complete"));
				customText.text = RDString.Get(text2 + text3);
			}
			customText.color = color;
			customText.gameObject.SetActive(value: true);
			Narration.Say(customText.text, NarrationCategory.Notification, false, NarrationActionName.ToContinue);
			break;
		}
		case LevelType.Challenge:
			customText.text = "challenge complete!";
			customText.gameObject.SetActive(value: true);
			break;
		default:
			rank.gameObject.SetActive(value: true);
			rank.text = mRank.ToString();
			Narration.Say(RDString.Get("narration.rank." + mRank.ToString()), NarrationCategory.Notification, false);
			narratedPress = false;
			timeToNarrate = Time.time + 5f;
			if (!base.game.currentLevel.delayRankVoice)
			{
				PlayRankVoiceline();
			}
			rank.color = color;
			vividStasisRank.color = color;
			break;
		}
		base.game.mistakesManager.SaveMistakesData();
	}

	public void SaveCurrentBossLevelAsPassed(bool perfectRun)
	{
		string levelIdentifier = base.game.levelIdentifier;
		if (!base.game.currentLevel.dogMode)
		{
			bool passedLevelWithoutCheckpoints = base.game.GetPassedLevelWithoutCheckpoints();
			Persistence.SetLastPlayedLevel(levelIdentifier);
			Persistence.SetLevelRank(levelIdentifier, perfectRun ? 5 : (passedLevelWithoutCheckpoints ? 14 : 4));
		}
	}

	public void ShowRankDescription()
	{
		string text = string.Empty;
		if (base.game.currentLevel.levelType == LevelType.Challenge)
		{
			description.text = RDString.Get("rankscreen.challengeRankDescription");
		}
		else if (base.game.currentLevel.levelType == LevelType.Boss)
		{
			bool passedLevelWithoutCheckpoints = base.game.GetPassedLevelWithoutCheckpoints();
			description.text = RDString.Get("rankscreen.congratulations");
			string customGameoverDescription = base.game.currentLevel.customGameoverDescription;
			if (customGameoverDescription != null)
			{
				description.text = customGameoverDescription;
			}
			else if (RDTime.speed > 1f)
			{
				description.text = RDString.Get("rankscreen.doublespeedcongratulations");
			}
			else if (isAnySRank)
			{
				description.text = RDString.Get("rankscreen.congratulations");
			}
			else if (passedLevelWithoutCheckpoints)
			{
				description.text = RDString.Get("rankscreen.nocheckpointscongratulations");
			}
		}
		else
		{
			bool flag = false;
			if (base.game.showAbsoluteOffsets | flag)
			{
				MistakesManager mistakesManager = base.game.mistakesManager;
				if (!GC.twoPlayerMode)
				{
					resultsSingleplayer.text = RDString.Get("rankscreen.mistakes").Replace("[mistakes]", Math.Round(mistakesManager.mistakes, 4).ToString()) + "\n" + RDString.Get("rankscreen.frameOffsetsDescription").Replace("[earlyAmount]", mistakesManager.earlyOffsetsSum.ToString()).Replace("[lateAmount]", mistakesManager.lateOffsetsSum.ToString())
						.Replace("[totalAmount]", mistakesManager.totalOffsetsSum.ToString());
					descriptionLayoutGroup.padding = new RectOffset(2, 2, 30, 0);
					descriptionLayoutGroup.spacing = 0f;
					resultsSingleplayer.gameObject.SetActive(value: true);
					multiplayerResultsLayoutGroup.gameObject.SetActive(value: false);
					resultsP1.gameObject.SetActive(value: false);
					resultsP2.gameObject.SetActive(value: false);
					text += resultsSingleplayer.text;
				}
				else
				{
					string text2 = RDString.Get("rankscreen.playerOne") + "\n" + RDString.Get("rankscreen.mistakes").Replace("[mistakes]", Math.Round(mistakesManager.mistakesP1, 4).ToString()) + "\n" + RDString.Get("rankscreen.frameOffsetsDescriptionShort").Replace("[earlyAmount]", mistakesManager.earlyOffsetsSumP1.ToString()).Replace("[lateAmount]", mistakesManager.lateOffsetsSumP1.ToString())
						.Replace("[totalAmount]", mistakesManager.totalOffsetsSumP1.ToString());
					string text3 = RDString.Get("rankscreen.playerTwo") + "\n" + RDString.Get("rankscreen.mistakes").Replace("[mistakes]", Math.Round(mistakesManager.mistakesP2, 4).ToString()) + "\n" + RDString.Get("rankscreen.frameOffsetsDescriptionShort").Replace("[earlyAmount]", mistakesManager.earlyOffsetsSumP2.ToString()).Replace("[lateAmount]", mistakesManager.lateOffsetsSumP2.ToString())
						.Replace("[totalAmount]", mistakesManager.totalOffsetsSumP2.ToString());
					if (!RDInput.persistenceSwapped)
					{
						resultsP1.text = text2;
						resultsP2.text = text3;
					}
					else
					{
						resultsP1.text = text3;
						resultsP2.text = text2;
					}
					descriptionLayoutGroup.padding = new RectOffset(2, 2, 12, 0);
					descriptionLayoutGroup.spacing = 32f;
					resultsSingleplayer.gameObject.SetActive(value: false);
					multiplayerResultsLayoutGroup.gameObject.SetActive(value: true);
					resultsP1.gameObject.SetActive(value: true);
					resultsP2.gameObject.SetActive(value: true);
					text += resultsP1.text;
					text = text + "\n" + resultsP2.text;
				}
				bool flag2 = RDString.StringHasCJKCharacters(resultsSingleplayer.text + resultsP1.text + resultsP2.text);
				Font appropiateFontForString = RDString.GetAppropiateFontForString(resultsSingleplayer.text + resultsP1.text + resultsP2.text);
				Text text4 = resultsSingleplayer;
				Text text5 = resultsP1;
				Font font = (resultsP2.font = appropiateFontForString);
				Font font3 = (text5.font = font);
				text4.font = font3;
				Text text6 = resultsSingleplayer;
				Text text7 = resultsP1;
				int num = (resultsP2.fontSize = (flag2 ? 8 : 10));
				int fontSize = (text7.fontSize = num);
				text6.fontSize = fontSize;
				Text text8 = resultsSingleplayer;
				Text text9 = resultsP1;
				Color color = (resultsP2.color = new Color(1f, 1f, 0f));
				Color color3 = (text9.color = color);
				text8.color = color3;
			}
			else
			{
				descriptionLayoutGroup.padding = new RectOffset(2, 2, 40, 0);
				descriptionLayoutGroup.spacing = 0f;
				resultsSingleplayer.gameObject.SetActive(value: false);
				multiplayerResultsLayoutGroup.gameObject.SetActive(value: false);
				resultsP1.gameObject.SetActive(value: false);
				resultsP2.gameObject.SetActive(value: false);
			}
			description.text = GetDescriptionForRank(scnGame.instance.currentLevel);
			if (base.game.currentLevel.delayRankVoice)
			{
				PlayRankVoiceline();
			}
		}
		bool flag3 = RDString.StringHasCJKCharacters(description.text);
		Font appropiateFontForString2 = RDString.GetAppropiateFontForString(description.text);
		description.font = appropiateFontForString2;
		description.fontSize = (flag3 ? 8 : 12);
		descriptionLayoutGroup.gameObject.SetActive(value: true);
		description.gameObject.SetActive(value: true);
		ShowSmallHand();
		text = text + "\n" + description.text;
		Narration.Say(text, NarrationCategory.Notification, false, NarrationActionName.ToContinue);
	}

	public string GetDescriptionForRank(LevelBase levelInstance)
	{
		if (base.game.currentLevel is Level_VividStasis && (int)mRank == 15)
		{
			return RDString.Get("VividStasis.V");
		}
		int num = mRank.ToNormal();
		return levelInstance.rankDescriptions[num];
	}

	public void PlayRankVoiceline()
	{
		scrConductor.PlayImmediately("sndJyi - Rank" + mRank.ToString(), 1f, RDUtils.GetMixerGroup("LevelResults"));
		if (isAnySRank)
		{
			scrConductor.PlayImmediately("sndApplause", 1f, RDUtils.GetMixerGroup("Applause"));
			string soundToPlay = "sndJyi - GoodJob";
			if (base.game.currentLevel is Level_VividStasis && (int)mRank == 15)
			{
				soundToPlay = "";
			}
			if (base.game.currentLevel.museDashResults)
			{
				soundToPlay = "sndMuseDash - FullCombo";
			}
			DOVirtual.DelayedCall(((int)mRank == 5) ? 1.4f : 2f, () =>
			{
				scrConductor.PlayImmediately(soundToPlay, 1f, RDUtils.GetMixerGroup("LevelResults"));
			});
		}
	}

	public void ShowLevelFailed()
	{
		rankscreen.SetActive(value: true);
		rank.gameObject.SetActive(value: false);
		description.gameObject.SetActive(value: false);
		base.game.statusText.gameObject.SetActive(value: false);
		customText.text = RDString.Get("status.levelFailed");
		customText.gameObject.SetActive(value: true);
		header.gameObject.SetActive(value: false);
		Narration.Say(customText.text, NarrationCategory.Notification, false, NarrationActionName.ToContinue);
	}

	public void ShowDontGiveUp()
	{
		descriptionLayoutGroup.gameObject.SetActive(value: true);
		description.gameObject.SetActive(value: true);
		description.text = RDString.Get("dialogue.dontGiveUp");
		Narration.Say(description.text, NarrationCategory.Notification, false, NarrationActionName.ToContinue);
		ExitLevelAfterSeconds(3f);
	}

	public void ExitLevelAfterSeconds(float seconds, bool closeBlades = false)
	{
		trueGameover = 4;
		if (!base.editorMode && !exiting)
		{
			this.closeBlades = closeBlades;
			if (secondsToExit == -100000f)
			{
				secondsToExit = seconds;
			}
			else
			{
				secondsToExit = Mathf.Min(seconds, secondsToExit);
			}
		}
	}

	private IEnumerator ExitLevelCoroutine()
	{
		if (closeBlades)
		{
			float num = base.gc.bladeRotationDuration * 1.5f;
			base.game.CloseBlades(num);
			yield return new WaitForSecondsRealtime(num);
		}
		if (levelToJump != null)
		{
			GC.twoPlayerMode = false;
			scnBase.GoToLevel(levelToJump);
		}
		else
		{
			base.game.EndLevel();
		}
	}
}
