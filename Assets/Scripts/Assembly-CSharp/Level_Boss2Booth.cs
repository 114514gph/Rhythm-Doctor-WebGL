using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using RDLevelEditor;
using RDTools;
using UnityEngine;

public class Level_Boss2Booth : LevelBase
{
	public int notesHitCorrectly;

	public int notesMissed;

	public int notesCount;

	private GameObject hoodieWard;

	private RDMainWard mainWard;

	private float mainWardStartX;

	private const float mainWardOffsetX = 1205f;

	private const float seamlessWidth = 510f;

	private RDBossStageText bossText;

	private RDCoffeeShop coffeeShopBg;

	private RDCoffeeShop coffeeShopFg;

	private RDCoffeeShop coffeeShopIntro;

	private VideoBloom videoBloom;

	public static bool passedCheckpoint;

	private float room3CamScrollX;

	private Vector3 originalPos = new Vector3(0f, 0f, -1000f);

	public Level_Boss2Booth(RDLevelData data)
		: base(data)
	{
	}

	public override void Init()
	{
		base.Init();
		bossText = LoadLevelAsset<RDBossStageText>();
		bossText.transform.LocalMoveX(9000f + RDClass.Vfx.hudCamera.RDFullNativeResolution().x * 0.5f);
		bossText.transform.LocalMoveY(99f);
		bossText.transform.LocalMoveZ(20f);
		coffeeShopBg = LoadLevelAsset<RDCoffeeShop>();
		coffeeShopBg.transform.MoveX(coffeeShopBg.transform.position.x + 3000f + 3000f);
		coffeeShopBg.OnlyBackground();
		coffeeShopFg = LoadLevelAsset<RDCoffeeShop>();
		coffeeShopFg.transform.MoveX(coffeeShopBg.transform.position.x + 3000f);
		coffeeShopFg.OnlyForeground();
		DisableRoom3CoffeeShop();
		coffeeShopIntro = LoadLevelAsset<RDCoffeeShop>();
		coffeeShopIntro.transform.MoveX(coffeeShopIntro.transform.position.x + 3000f);
		videoBloom = RDClass.Vfx.hudCamera.GetComponent<VideoBloom>();
		base.game.barToSkipToOnSlash = 94;
	}

	public override IEnumerator LoadBigAssets()
	{
		if (base.game.currentLevel == null)
		{
			RDClassDll.printem("current level is null!");
		}
		missesToCrackHeart = 16f;
		levelType = LevelType.Boss;
		bossActNum = 2;
		runFirstActionBarInPreactions = true;
		charsOnlyOnStart = true;
		hoodieWard = LoadLevelAsset("RDHoodieBoyWard/RDHoodieBoyWard");
		hoodieWard.GetComponent<RDHoodieBoyWard>().room = base.room0;
		hoodieWard.transform.LocalMoveX(0f);
		mainWardStartX = 10205f;
		mainWard = LoadLevelAsset("RDMainWard/RDMainWard").GetComponent<RDMainWard>();
		mainWard.transform.LocalMoveX(mainWardStartX);
		mainWard.transform.LocalMoveY(RDClass.Vfx.hudCamera.RDFullNativeResolution().y * 0.5f);
		mainWard.leftEnd.SetActive(value: false);
		mainWard.rightEnd.SetActive(value: false);
		mainWard.seamlessExtra.SetActive(value: true);
		tk2dSprite[] vendingMachineSprites = mainWard.vendingMachineSprites;
		for (int i = 0; i < vendingMachineSprites.Length; i++)
		{
			vendingMachineSprites[i].gameObject.SetActive(value: false);
		}
		mainWard.gameObject.SetActive(value: false);
		foreach (LevelEvent_Base levelEvent in data.levelEvents)
		{
			if (levelEvent is LevelEvent_AddOneshotBeat || levelEvent is LevelEvent_AddClassicBeat || levelEvent is LevelEvent_AddFreeTimeBeat)
			{
				notesCount++;
			}
		}
		SetUpFlashMarginFeedback(RowType.Oneshot);
		yield return null;
	}

	public override void preactions()
	{
		if (base.conductor.barNumber == 1)
		{
			if (GC.twoPlayerMode)
			{
				base.game.MoveWorldSpaceToRoom();
			}
			if (passedCheckpoint)
			{
				float num = missesToCrackHeart;
				float num2 = (float)Mathf.FloorToInt(num * 0.33f) / num;
				float num3 = missesToCrackHeart * num2;
				float weight = (missesToCrackHeart - num3) / 2f;
				base.rows[0].ent.CrackAdvance(weight);
				base.rows[1].ent.CrackAdvance(weight);
				_ = base.game.hpController;
			}
			SetBgStyle(1);
		}
	}

	public override void actions()
	{
		if (base.conductor.barNumber == 1 && base.conductor.lastBarScrubbed < 1)
		{
			scrExecuteOnCertainBeat.Add(() =>
			{
				bossText.gameObject.SetActive(value: true);
				bossText.Play();
			}, 10.5f);
		}
		if (base.conductor.barNumber == 3)
		{
			base.game.hpController.Show(show: true);
			scrExecuteOnHit.Add(() =>
			{
				notesHitCorrectly++;
				_ = (float)notesHitCorrectly * 1f / (float)notesCount;
			}, HitType.Hit, persistent: true);
		}
		if (base.conductor.barNumber == 4)
		{
			hoodieWard.SetActive(value: false);
		}
		if (base.conductor.barNumber == 52)
		{
			base.game.ReviveDeadRows();
		}
		if (base.conductor.barNumber == 85)
		{
			hoodieWard.SetActive(value: true);
		}
		if (base.conductor.barNumber == 91)
		{
			hoodieWard.SetActive(value: false);
			mainWard.gameObject.SetActive(value: true);
			scrExecuteOnCertainBeat.Add(() =>
			{
				scrChar character2 = base.rows[0].ent.character;
				character2.noBeat = true;
				character2.noHappy = true;
				character2.noBeep = true;
				character2.neutralAnimName = "running";
			}, 3.25f);
			scrExecuteOnUpdate.Add(() =>
			{
				LateUpdate2();
			}, lateUpdate: true);
		}
		if (base.conductor.barNumber == 101)
		{
			scrChar character = base.rows[0].ent.character;
			character.noBeat = false;
			character.noHappy = false;
			character.neutralAnimName = "neutral";
			mainWard.gameObject.SetActive(value: false);
		}
		if (base.conductor.barNumber == 105 && base.conductor.lastBarScrubbed < 105)
		{
			base.room0.stutter.AddStutter(3f, 3f, 0.1666f, 5, stack: true);
		}
	}

	private void SetBgStyle(int ID)
	{
		if (RDClass.Vfx.hiResVideoBloom)
		{
			videoBloom.enabled = true;
			videoBloom.Tint = "6C93BFFF".HexToColor();
			videoBloom.Threshold = 0.74f;
			videoBloom.KernelSize = 59f;
			videoBloom.MasterAmount = 2.18f;
			videoBloom.MediumKernelScale = 2.8f;
			videoBloom.HighQuality = false;
		}
	}

	public override bool FailLevel(RowEntity ent)
	{
		Persistence.SetLevelRank(base.game.levelIdentifier, -1);
		scrChar mainCharacter = ent.character;
		failedLevel = true;
		RDClass.Vfx.Flash();
		RDClass.Vfx.ShakeCam(30, 20);
		RDClass.Vfx.KillFlashTextUIAnimations();
		mainCharacter.animator.deltaTimeStyle = tk2dSpriteAnimator.DeltaTimeStyle.AlwaysOneAlways;
		DOVirtual.DelayedCall(2f, () =>
		{
			ent.heart.DoFinalCrack();
			RDUtils.Rotate2D(ent.heart.transform, 0f);
		});
		DOVirtual.DelayedCall(3f, () =>
		{
			RDClass.Vfx.SpotlightFocusFromCenter(mainCharacter.transform.position.x, 0.5f, playSwitchSound: true, RDUtils.GetMixerGroup("FeedbackSoundsParent"));
		});
		DOVirtual.DelayedCall(5f, () =>
		{
			mainCharacter.PlayExpression("gameover");
		});
		DOVirtual.DelayedCall(8f, () =>
		{
			base.game.statusText.Show("status.bossFailRestart", narrate: true);
			GameOverBarInfo genericGameOverInfo = new GameOverBarInfo(-1, new int[13]
			{
				1, 1, 1, 1, 1, -2, -2, -2, -2, 1,
				-2, 1, 2
			});
			GameOverBarInfo item = new GameOverBarInfo(94, new int[3] { 1, 1, 3 });
			List<GameOverBarInfo> gameOverBars = new List<GameOverBarInfo> { item };
			PlayGameOverDialogue(gameOverBars, genericGameOverInfo);
		});
		scrExecuteOnPeriodicBeat.Clear();
		return true;
	}

	private void LateUpdate2()
	{
		if (room3CamScrollX == 0f)
		{
			return;
		}
		base.room3.camera.transform.position += new Vector3(room3CamScrollX * Time.deltaTime * 120f, 0f, 0f);
		float num = mainWard.transform.position.x - 1205f - base.room3.camera.transform.position.x;
		int num2 = 0;
		if (num > 0f)
		{
			while (num > 0f)
			{
				mainWard.transform.LocalMoveX(mainWard.transform.position.x - 510f);
				num = mainWard.transform.position.x - 1205f - base.room3.camera.transform.position.x;
				num2++;
				if (num2 > 1000)
				{
					Debug.Log("Loop exploded :(");
					break;
				}
			}
		}
		else
		{
			if (!(num < -510f))
			{
				return;
			}
			while (num < -510f)
			{
				mainWard.transform.LocalMoveX(mainWard.transform.position.x + 510f);
				num = mainWard.transform.position.x - 1205f - base.room3.camera.transform.position.x;
				num2++;
				if (num2 > 1000)
				{
					Debug.Log("Loop exploded :(");
					break;
				}
			}
		}
	}

	public void EnableRoom3CoffeeShop()
	{
		coffeeShopFg.gameObject.SetActive(value: true);
		coffeeShopBg.gameObject.SetActive(value: true);
	}

	public void DisableRoom3CoffeeShop()
	{
		coffeeShopFg.gameObject.SetActive(value: false);
		coffeeShopBg.gameObject.SetActive(value: false);
	}

	public void TransitionRoom3CoffeeShopToNight()
	{
		coffeeShopFg.TransitionToNight();
		coffeeShopBg.TransitionToNight();
	}

	public void TransitionIntroCoffeeShopToNight()
	{
		coffeeShopIntro.TransitionToNight();
	}

	public void ToggleIntroCoffeeShop(bool enable)
	{
		coffeeShopIntro.gameObject.SetActive(enable);
	}

	public void OpenRoom3Door()
	{
		coffeeShopFg.OpenDoor();
	}

	public void CloseRoom3Door()
	{
		coffeeShopFg.OpenDoor(open: false);
	}

	public void ToggleBloom(bool enableBloom)
	{
		if (RDClass.Vfx.hiResVideoBloom)
		{
			videoBloom.enabled = enableBloom;
		}
	}

	public void Room2CamScrollX(float scroll)
	{
		if (originalPos.z == -1000f)
		{
			originalPos = base.room2.camera.transform.position;
		}
		room3CamScrollX = scroll;
	}

	public void ReturnRoom2Cam()
	{
		room3CamScrollX = 0f;
		if (originalPos.z != -1000f)
		{
			base.room2.camera.transform.position = originalPos;
		}
	}

	public void Room3CamScrollX(float x)
	{
		if (originalPos.z == -1000f)
		{
			originalPos = base.room3.camera.transform.position;
		}
		room3CamScrollX = x;
	}

	public void ReturnRoom3Cam()
	{
		room3CamScrollX = 0f;
		if (originalPos.z != -1000f)
		{
			base.room3.camera.transform.position = originalPos;
		}
	}
}
