using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using RDLevelEditor;
using UnityEngine;

public class Level_PaigesReckoning : LevelBase
{
	private RDManyEyes eyesBig;

	private RDManyEyes eyesSmall;

	private RDBossStageText bossText;

	public Level_PaigesReckoning(RDLevelData data)
		: base(data)
	{
	}

	public override IEnumerator LoadBigAssets()
	{
		eyesBig = LoadLevelAsset<RDManyEyes>();
		eyesSmall = LoadLevelAsset<RDManyEyes>();
		eyesBig.Setup(smallEyes: false);
		eyesSmall.Setup(smallEyes: true);
		eyesBig.transform.LocalMoveXY(6000f + RDClass.Vfx.RDWidth / 2f, RDClass.Vfx.RDHeight / 2f);
		eyesSmall.transform.LocalMoveXY(9000f + RDClass.Vfx.RDWidth / 2f, RDClass.Vfx.RDHeight / 2f);
		bossText = LoadLevelAsset<RDBossStageText>();
		noBossFail = true;
		base.game.hpController.noP2BossBar = true;
		base.game.hpController.player1HP.useMistakes = true;
		levelType = LevelType.Boss;
		bossActNum = 6;
		cutsceneBarsDesc = new (int, int)[2]
		{
			(33, 60),
			(2, 13)
		};
		yield return null;
	}

	public override void OnMistake(RowEntity ent = null)
	{
		if (base.numMistakes > missesToCrackHeart)
		{
			base.game.FailLevel(base.game.rows[0].ent);
		}
	}

	public void EyeAngle(float angle, float duration, string easeStr)
	{
		Ease ease = Enum.Parse<Ease>(easeStr);
		eyesBig.SetScrollAngle(angle, duration, ease);
		eyesSmall.SetScrollAngle(angle, duration, ease);
	}

	public void EyeScroll(float amount, float duration, string easeStr)
	{
		Ease ease = Enum.Parse<Ease>(easeStr);
		eyesBig.SetScrollSpeed(amount, duration, ease);
		eyesSmall.SetScrollSpeed(amount / 2f, duration, ease);
	}

	public void EyesClose(float percent)
	{
		eyesBig.EyesClose(percent);
	}

	public void EyesToggleSmall(bool open)
	{
		EdegaEye[] eyes = eyesSmall.eyes;
		for (int i = 0; i < eyes.Length; i++)
		{
			eyes[i].DoBlink(!open).timeScale = 4f;
		}
	}

	public void EyesToggle(float percent)
	{
		eyesBig.EyesToggle(percent);
	}

	public void EyesToggleEqually(float percent)
	{
		eyesBig.EyesToggleEqually(percent);
	}

	public void EyesBlink(float percent)
	{
		eyesBig.EyesBlink(percent);
	}

	public void StareAtMark()
	{
		eyesBig.StareAtRow(0);
	}

	public void StareAtPlayer(bool p2)
	{
		scrHandController handController = base.game.handController0;
		RDArm rDArm = (handController.DoesEnumTargetLeftHand(p2 ? Hand.p2 : Hand.p1) ? handController.leftArm : handController.rightArm);
		eyesBig.StareAtTransform(rDArm.button.transform, new Vector3(37 * ((!p2) ? 1 : (-1)), 16f));
	}

	public void ShowBossText(int room, float xPerc, float yPerc)
	{
		if (!base.conductor.isScrubbing)
		{
			bossText.gameObject.SetActive(value: true);
			float x = 3000f * (float)room + RDClass.Vfx.RDWidth * (xPerc / 100f);
			float y = RDClass.Vfx.RDHeight * (yPerc / 100f);
			bossText.transform.localPosition = new Vector3(x, y, 20f);
			bossText.Play();
		}
	}

	public void SetVerticalShiftSpeed(float speed)
	{
		eyesBig.SetVerticalShiftSpeed(speed);
		eyesSmall.SetVerticalShiftSpeed(speed);
	}

	public void SetEnding(bool paigeStays)
	{
		if (scnGame.levelToLoadSource == LevelSource.InternalPath)
		{
			skippableRankScreen = false;
			customGameoverDescription = RDString.Get(paigeStays ? "PaigesReckoning.rankscreen.paigeStays" : "PaigesReckoning.rankscreen.paigeLeaves");
			Persistence.SetPaigeEnding(paigeStays);
		}
	}

	public void SetLevelFailed()
	{
		customGameoverHeader = RDString.Get("rankscreen.levelFailed");
		customGameoverDescription = RDString.Get("PaigesReckoning.rankscreen.tryAgain");
	}

	public override bool FailLevel(RowEntity ent)
	{
		Persistence.SetLevelRank(base.game.levelIdentifier, -1);
		scrChar mainCharacter = base.rowEnt0.character;
		failedLevel = true;
		RDClass.Vfx.KillFlashTextUIAnimations();
		scrExecuteOnPeriodicBeat.Clear();
		DOTween.KillAll();
		mainCharacter.animator.deltaTimeStyle = tk2dSpriteAnimator.DeltaTimeStyle.AlwaysOneAlways;
		DOVirtual.DelayedCall(2f, () =>
		{
			base.game.statusText.Show("status.bossFailRestart", narrate: true);
			RDClass.Vfx.SpotlightFocusFromCenter(mainCharacter.transform.position.x, 0.5f, playSwitchSound: true, RDUtils.GetMixerGroup("FeedbackSoundsParent"));
			RunEventsWithTag("FailLevelSpotlight");
		});
		DOVirtual.DelayedCall(3.5f, () =>
		{
			base.game.statusText.Show("status.bossFailRestart", narrate: true);
			GameOverBarInfo genericGameOverInfo = new GameOverBarInfo(-1, new int[7] { 1, 1, 1, 1, 1, 1, 1 });
			PlayGameOverDialogue(new List<GameOverBarInfo>(), genericGameOverInfo);
		});
		RunEventsWithTag("FailLevel");
		return true;
	}

	public void SetRankScreenSkippable()
	{
		skippableRankScreen = true;
	}

	public override void Init()
	{
		base.game.canvas.gameObject.layer = 5;
	}

	public override void preactions()
	{
		if (base.conductor.barNumber == 1)
		{
			missesToCrackHeart = rankLowerBounds[1];
		}
	}

	public override void actions()
	{
		if (base.conductor.barNumber == 128)
		{
			missesToCrackHeart = 999f;
		}
	}

	public override void OnEnterLowHealth()
	{
	}

	public RDArm GetArm(int room, bool leftArm)
	{
		scrHandController handControllerFromInt = base.game.GetHandControllerFromInt(room);
		if (!leftArm)
		{
			return handControllerFromInt.rightArm;
		}
		return handControllerFromInt.leftArm;
	}

	public void MoveArmX(int room, bool leftArm, float x, float duration, string easeString)
	{
		Ease ease = Enum.Parse<Ease>(easeString);
		scrHandController handController = base.game.GetHandControllerFromInt(room);
		RDArm arm = GetArm(room, leftArm);
		arm.SetVisible(visible: true, force: true);
		arm.transform.DOKill(complete: true);
		arm.transform.DOLocalMoveX(x, duration).SetEase(ease).OnStart(() =>
		{
			if (!leftArm)
			{
				handController.slidingIn = true;
			}
		})
			.OnComplete(() =>
			{
				if (!leftArm)
				{
					handController.slidingIn = false;
				}
			})
			.Done();
	}

	public void ChangeArmCharacter(int room, bool leftArm, bool paige, bool isP1)
	{
		RDArm arm = GetArm(room, leftArm);
		if (!paige)
		{
			arm.player = ((!isP1) ? RDPlayer.P2 : RDPlayer.P1);
			if (isP1)
			{
				scnGame.p1DefibMode = DefibMode.Normal;
			}
			else
			{
				scnGame.p2DefibMode = DefibMode.Normal;
			}
		}
		arm.SetToCharacter(paige ? Character.Paige : Character.Player);
		arm.SetDefibMode(DefibMode.Normal);
	}

	public void MoveArmY(int room, bool leftArm, float y, float duration, string easeString)
	{
		Ease ease = Enum.Parse<Ease>(easeString);
		RDArm arm = GetArm(room, leftArm);
		arm.SetVisible(visible: true, force: true);
		arm.transform.DOKill();
		arm.transform.DOLocalMoveY(y, duration).SetEase(ease).Done();
	}

	public void SetArmZ(int room, bool leftArm, float z)
	{
		RDArm arm = GetArm(room, leftArm);
		arm.transform.DOKill();
		arm.transform.localPosition = arm.transform.localPosition.WithZ(z);
	}

	public void MoveButtonX(int room, bool leftArm, int x, float duration, string easeString)
	{
		Ease ease = Enum.Parse<Ease>(easeString);
		RDArm arm = GetArm(room, leftArm);
		arm.SetVisible(visible: true, force: true);
		arm.button.transform.DOKill();
		arm.button.transform.DOLocalMoveX(x, duration).SetEase(ease).Done();
	}

	public void MoveButtonY(int room, bool leftArm, int y, float duration, string easeString)
	{
		Ease ease = Enum.Parse<Ease>(easeString);
		RDArm arm = GetArm(room, leftArm);
		arm.SetVisible(visible: true, force: true);
		arm.button.transform.DOKill();
		arm.button.transform.DOLocalMoveY(y, duration).SetEase(ease).Done();
	}

	public void ToggleNervous(int room, bool leftArm, bool on)
	{
		GetArm(room, leftArm).ToggleNervous(on);
	}

	public void PlayArmAnim(int room, bool leftArm, string anim)
	{
		GetArm(room, leftArm).hand.PlayAnim(anim);
	}
}
