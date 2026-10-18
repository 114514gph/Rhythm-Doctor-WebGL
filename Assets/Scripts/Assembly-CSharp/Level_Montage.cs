using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using RDLevelEditor;
using UnityEngine;

public class Level_Montage : LevelBase
{
	private RDSamuraiWard samuraiWard;

	private bool missedOnce;

	private bool levelWasAlreadyPassed;

	private RDCoffeeShop coffeeShop;

	private RowEntity coffeeShopOneshotRow;

	private tk2dSprite[] currentStack;

	private Vector2[] currentStackPos;

	private bool startedStack;

	private bool missedStack;

	private int currentCup;

	private Vector2[] cupPos_4;

	private Vector2[] cupPos_20;

	private int _tiredness;

	private bool playedHitActionThisFrame;

	private RDBossStageText bossText;

	public int tiredness
	{
		get
		{
			return _tiredness;
		}
		set
		{
			_tiredness = value;
			SetExpressions(startedStack);
		}
	}

	public Level_Montage(RDLevelData data)
		: base(data)
	{
	}

	private static Vector2[] GetCupPositions(tk2dSprite[] cups)
	{
		Vector2[] array = new Vector2[cups.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = cups[i].transform.localPosition;
		}
		return array;
	}

	public override void Init()
	{
		checkpointBarsDesc = new int[2] { 168, 71 };
		base.game.barToSkipToOnSlash = 168;
		scnGame.pauseBlocked = false;
	}

	public override void OnMistake(RowEntity ent = null)
	{
		if (!b9)
		{
			missedOnce = true;
		}
		if (base.numMistakes > missesToCrackHeart)
		{
			FailBoss(survivedSomehow: false);
		}
		else
		{
			UpdateAllHearts();
		}
	}

	public override void Update()
	{
		if (base.conductor.barNumber >= 28 && base.conductor.barNumber <= 38)
		{
			playedHitActionThisFrame = false;
		}
	}

	public override IEnumerator LoadBigAssets()
	{
		levelType = LevelType.Boss;
		noBossFail = true;
		manualHeartCrack = true;
		levelWasAlreadyPassed = Persistence.GetLevelRank(Level.Montage).passed;
		samuraiWard = LoadLevelAsset<RDSamuraiWard>();
		samuraiWard.transform.localPosition = new Vector3(176f, 99f, 0f);
		bossText = LoadLevelAsset<RDBossStageText>();
		bossText.transform.localPosition = new Vector3(RDClass.Vfx.RDWidth * 0.5f, RDClass.Vfx.RDHeight * 0.5f, 20f);
		yield return null;
	}

	public override void actions()
	{
		if (base.conductor.barNumber == 28)
		{
			coffeeShop = base.game.rooms[2].coffeeShop.GetComponent<RDCoffeeShop>();
			coffeeShopOneshotRow = base.game.rows[7].ent;
			cupPos_4 = GetCupPositions(coffeeShop.cups_4);
			cupPos_20 = GetCupPositions(coffeeShop.cups_20);
		}
		else if (base.conductor.barNumber == 65)
		{
			base.game.rooms[0].peekWindowMode = true;
			base.game.rooms[0].windowPeekIndex = 0;
			base.game.rooms[1].peekWindowMode = true;
			base.game.rooms[1].windowPeekIndex = 1;
		}
		else if (base.conductor.barNumber == 71)
		{
			base.game.rooms[0].peekWindowMode = false;
			base.game.rooms[1].peekWindowMode = false;
		}
	}

	public override void preactions()
	{
		if (base.conductor.barNumber == 1)
		{
			scnGame.pauseBlocked = false;
			missesToCrackHeart = rankLowerBounds[1];
			if (base.game.barToScrubTo >= 168 && !base.editorMode)
			{
				missedOnce = true;
			}
		}
	}

	public override void OnHit(HitType hitType, Beat beat)
	{
		if (base.conductor.barNumber >= 28 && base.conductor.barNumber <= 38 && !playedHitActionThisFrame)
		{
			playedHitActionThisFrame = true;
			bool miss = hitType != HitType.Hit;
			ThrowCoffee(miss);
		}
	}

	public void OpenSamuraiDoors()
	{
		base.game.StartCoroutine(samuraiWard.OpenDoors());
	}

	public void ShowSamuraiLanterns()
	{
		samuraiWard.ShowLanterns();
	}

	public void ShowSamuraiWard(bool active)
	{
		samuraiWard.gameObject.SetActive(active);
	}

	public void StartStack(int newStack = -1)
	{
		missedStack = false;
		FastBeatTrigger(isFast: true);
		if (currentStack != null)
		{
			tk2dSprite[] array = currentStack;
			foreach (tk2dSprite obj in array)
			{
				obj.DOComplete(withCallbacks: true);
				obj.gameObject.SetActive(value: false);
			}
		}
		switch (newStack)
		{
		case 4:
			currentStack = coffeeShop.cups_4;
			currentStackPos = cupPos_4;
			break;
		case 20:
			currentStack = coffeeShop.cups_20;
			currentStackPos = cupPos_20;
			break;
		}
		coffeeShop.SlideCoffeeMan();
		startedStack = true;
		SetExpressions(stacking: true);
	}

	public void EndStack()
	{
		DOVirtual.DelayedCall(0.15f, () =>
		{
			coffeeShop.SlideCoffeeMan(inside: false);
		});
		startedStack = false;
		currentCup = 0;
		FastBeatTrigger(isFast: false);
		SetExpressions(stacking: false);
	}

	private void CollapseStack()
	{
		missedStack = true;
		for (int i = 0; i < currentStack.Length; i++)
		{
			tk2dSprite cup = currentStack[i];
			cup.DOComplete(withCallbacks: true);
			Vector2 origPosition = currentStackPos[i];
			float duration = Random.Range(0.1f, 0.5f);
			int num = Random.Range(-60, -30);
			int num2 = Random.Range(90, 420);
			cup.transform.DOLocalJump(origPosition.WithY(num), 10f, 1, duration).SetEase(Ease.Linear);
			cup.transform.DORotate(new Vector3(0f, 0f, num2), duration).SetEase(Ease.Linear);
			cup.DOFade(0f, duration).SetEase(Ease.Linear).OnComplete(() =>
			{
				cup.transform.LocalMoveY(origPosition.y);
				cup.color = cup.color.WithAlpha(1f);
				cup.Rotate2D(0f);
				cup.gameObject.SetActive(value: false);
			});
			coffeeShop.coffeeMan.Play("Coffeeman_miss");
		}
	}

	private void ThrowCoffee(bool miss)
	{
		if (!startedStack)
		{
			return;
		}
		if (currentStack == null)
		{
			currentStack = coffeeShop.cups_4;
		}
		if (currentCup >= currentStack.Length)
		{
			return;
		}
		tk2dSprite cup = currentStack[currentCup];
		Vector2 origPosition = currentStackPos[currentCup];
		Vector2 baristaPosition = new Vector2(15f, 16f);
		float height = (miss ? 30f : 60f);
		if (!missedStack)
		{
			RunEventsWithTag("ThrowCoffee");
		}
		DOVirtual.DelayedCall(1f / 6f, () =>
		{
			cup.gameObject.SetActive(value: true);
			cup.transform.localPosition = baristaPosition;
			cup.transform.DOLocalJump(origPosition, height, 1, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
			{
				cup.transform.DOShakePosition(0.2f, 1f, 10, 90f, snapping: true);
			});
			cup.Rotate2D(0f);
			cup.transform.DORotate(new Vector3(0f, 0f, 720f), 0.5f, RotateMode.FastBeyond360).SetEase(Ease.InOutCubic);
			if (miss)
			{
				startedStack = false;
				DOVirtual.DelayedCall(0.5f, () =>
				{
					CollapseStack();
				});
				SetExpressions(stacking: false);
			}
		});
		currentCup++;
	}

	private void SetExpressions(bool stacking)
	{
		string text = ((tiredness == 0) ? "" : ((tiredness == 1) ? "Tired" : "VeryTired"));
		string text2 = "neutral" + text;
		string happyAnimName = (stacking ? ("throw" + text) : text2);
		coffeeShopOneshotRow.character.noHappy = !stacking;
		coffeeShopOneshotRow.character.noMiss = tiredness != 0;
		coffeeShopOneshotRow.character.neutralAnimName = text2;
		coffeeShopOneshotRow.character.happyAnimName = happyAnimName;
	}

	public void SetWaveSize(float width, float height)
	{
		coffeeShopOneshotRow.oneshotRowController.waveHeightMultiplier = height;
		coffeeShopOneshotRow.oneshotRowController.waveWidthMultiplier = width;
	}

	public void FastBeatTrigger(bool isFast)
	{
		if (isFast)
		{
			SetWaveSize(0.75f, 0.75f);
			SetOneshotType(7, "Spring");
		}
		else
		{
			SetWaveSize(1f, 1f);
			SetOneshotType(7, "BoomAndRush");
		}
	}

	public void ShowBossText()
	{
		if (!base.conductor.isScrubbing)
		{
			bossText.gameObject.SetActive(value: true);
			bossText.SetColor(Color.white);
			bossText.Play();
		}
		else
		{
			bossText.gameObject.SetActive(value: false);
		}
	}

	public void CalibratePeek(int roomID, int windowID)
	{
		base.game.rooms[roomID].ManualCalibratePeek(base.game.windowChoreographer.dancers[windowID].window.GetCenter());
	}

	public void SetLevelAsPassed()
	{
		base.game.rankscreen.SaveCurrentBossLevelAsPassed(!missedOnce);
		if (levelWasAlreadyPassed && !missedOnce && !base.game.GetPassedLevelWithoutCheckpoints())
		{
			base.game.statusText.SetStatusText(RDString.Get("rankscreen.bossPerfectCaps"), null, 4f, narrate: true);
		}
	}

	public void SetSkippableIfPassed()
	{
		if (levelWasAlreadyPassed)
		{
			skippable = true;
		}
	}

	public void PrepNextLevel()
	{
		Persistence.SetLastPlayedLevel(Level.Montage2);
	}

	public void AlmostBreakHeart(int row)
	{
		SetHeartMistakes(row, missesToCrackHeart - 1f);
	}

	public void FailBoss(bool survivedSomehow)
	{
		base.game.FailLevelLite();
		if (b9)
		{
			b2 = survivedSomehow;
			RunEventsWithTag("FakeFailLevel");
			if (!base.game.editorMode && !levelWasAlreadyPassed)
			{
				scnGame.pauseBlocked = true;
			}
		}
		else
		{
			base.game.AddLevelDeath();
			RunEventsWithTag("FailLevel");
		}
	}

	public void RunGameOverDialogue()
	{
		GameOverBarInfo genericGameOverInfo = new GameOverBarInfo(-1, new int[5] { 1, 1, 1, 1, 1 });
		List<GameOverBarInfo> gameOverBars = new List<GameOverBarInfo>();
		PlayGameOverDialogue(gameOverBars, genericGameOverInfo, "GameOver_Montage");
	}
}
