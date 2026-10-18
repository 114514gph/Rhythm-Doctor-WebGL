using System.Collections;
using DG.Tweening;
using UnityEngine;

public class Level_Tutorial_Oneshot : LevelBase
{
	private const int PHASE1HITS = 6;

	private const int PHASE2HITS = 14;

	private int phase1hitstogo;

	private bool firstTimeHitPossible = true;

	private bool anymisses;

	private bool helpgiven;

	private bool helpgiven2;

	private int phase2hitstogo;

	private bool nursesaid;

	public override void Init()
	{
		songsUsed = new string[1] { "sndTutorial" };
		levelType = LevelType.Tutorial;
		levelToSkipTo = Level.SVT;
		phase1hitstogo = 6;
		phase2hitstogo = 14;
	}

	public RDInk DiaTutorial(string node, RDInk.RDInkCallback onComplete = null)
	{
		return base.ink.Run("diaTutorialOneshot", node, onComplete);
	}

	private void callText2()
	{
		DiaTutorial("OnMusic", () =>
		{
			base.game.MakeRow(RowType.Oneshot, 1, 2f, Character.Farmer);
			RDClass.Vfx.SetSpriteMode(SpriteMode.Glow, Color.black.WithAlpha(0.2f));
			base.game.handController0.SlideIn();
		}).Prolong().WaitForNextBarOnEnd();
	}

	private void callTextMiss()
	{
		firstTimeHitPossible = false;
		if (!helpgiven && !helpgiven2)
		{
			helpgiven = true;
			DiaTutorial("FirstMiss", () =>
			{
				base.conductor.barNumberToGo = 3;
			}).Prolong().WaitForNextBarOnEnd();
		}
		if (!helpgiven && helpgiven2)
		{
			helpgiven = true;
			DiaTutorial("FirstMissAfterMissCompletely", () =>
			{
				base.conductor.barNumberToGo = 3;
			}).Prolong().WaitForNextBarOnEnd();
		}
	}

	private void callTextMisscom()
	{
		firstTimeHitPossible = false;
		if (!helpgiven && !helpgiven2)
		{
			DiaTutorial("FirstMissCompletely", () =>
			{
				helpgiven2 = true;
				base.conductor.barNumberToGo = 3;
			}).Prolong().WaitForNextBarOnEnd();
		}
	}

	public override IEnumerator LoadBigAssets()
	{
		yield return null;
	}

	private void missedAnything()
	{
		anymisses = true;
	}

	private void SetUpFirstPhaseExecutes()
	{
		scrExecuteOnHit.Add(callTextMiss, HitType.JustMiss, persistent: true);
		scrExecuteOnHit.Add(callTextMiss, HitType.BigMiss, persistent: true);
		scrExecuteOnHit.Add(callTextMisscom, HitType.MissCompletely, persistent: true);
	}

	private void firstphasehit()
	{
		phase1hitstogo--;
		if (phase1hitstogo == 5)
		{
			if (firstTimeHitPossible)
			{
				DiaTutorial("FirstHitPerfect");
			}
			else
			{
				DiaTutorial("Phase1_1Hit");
			}
		}
		if (phase1hitstogo == 4)
		{
			DiaTutorial("Phase1_2Hit");
		}
		if (phase1hitstogo == 3)
		{
			DiaTutorial("Phase1_3Hit");
		}
		if (phase1hitstogo == 2)
		{
			DiaTutorial("Phase1_4Hit");
		}
		if (phase1hitstogo == 1)
		{
			DiaTutorial("Phase1_5Hit");
		}
		if (phase1hitstogo == 0)
		{
			scrExecuteOnHit.Clear();
			DiaTutorial("Interlude1").Prolong().WaitForNextBarOnEnd();
		}
		firstTimeHitPossible = false;
	}

	private void secondphasehit()
	{
		phase2hitstogo--;
		if (phase2hitstogo != 0)
		{
			return;
		}
		scrExecuteOnHit.Clear();
		string node = (anymisses ? "Interlude2" : "Interlude2NoMisses");
		DiaTutorial(node, () =>
		{
			base.game.DeleteRow(1);
			RDClass.Vfx.FadeOut();
			FadeOutMusic(1f);
			DOVirtual.DelayedCall(1f, () =>
			{
				scnBase.GoToLevelWithEnum(levelToSkipTo);
			});
		}).Prolong().WaitForNextBarOnEnd();
	}

	private void secondphasemiss()
	{
	}

	public override void preactions()
	{
		switch (base.conductor.barNumber)
		{
		case 1:
			base.game.SetPlayerHitSounds(RowType.Oneshot, "sndClapHitMassivePreEcho", "sndClapHitMassivePreEchoP2");
			phase1hitstogo = 6;
			phase2hitstogo = 14;
			base.game.nurseCount = true;
			base.game.handController0.HideInstant();
			LEDSign.status = RDString.Get("status.skipTutorial");
			RDUtils.ShowTutorialTheme("B163E0".HexToColor(), "9F55CF".HexToColor());
			base.conductor.SetBPM(88f);
			DiaTutorial("NoMusic").Prolong().JumpOnEnd();
			break;
		case 2:
			PlayMusic(songsUsed[0]);
			Timer.Add(callText2, 3f);
			base.game.nurseCount = true;
			break;
		case 3:
			shouldJyiSayOneshot = true;
			base.conductor.crotchetsPerBar = 4;
			LEDSign.status = RDString.Get("status.OneshotHitOnTheClap");
			LEDSign.showMarginError = true;
			SetUpFirstPhaseExecutes();
			base.game.OneshotSayGetSet(0f, 2f);
			break;
		case 4:
			base.conductor.Freeze(PlayStyleChange.Loop);
			base.game.AddBeatOneshot(1, 0f, 2f);
			scrExecuteOnHit.Add(firstphasehit, HitType.Hit, persistent: true);
			scrExecuteOnHit.Add(missedAnything, HitType.AnyMiss, persistent: true);
			break;
		case 5:
			scrExecuteOnHit.Add(secondphasehit, HitType.Hit, persistent: true);
			scrExecuteOnHit.Add(missedAnything, HitType.AnyMiss, persistent: true);
			base.game.OneshotSayGetSet(0f, 2f);
			base.game.OnBeatOneshotConstInterval(1, 1, 8f, 2f);
			break;
		case 6:
			base.conductor.crotchetsPerBar = 32;
			base.conductor.Freeze(PlayStyleChange.Loop);
			base.game.ChangeLoopOneshot(1, 0f, 2f);
			base.game.ChangeLoopOneshot(1, 16f, 1f);
			base.game.OneshotSayReady(12f, 1f);
			base.game.OneshotSayGetSet(28f, 2f);
			break;
		}
	}

	public override void actions()
	{
		if (base.conductor.barNumber == 3 && !nursesaid)
		{
			DiaTutorial("Nurse");
			nursesaid = true;
		}
	}

	public override void BeepGet(float beepPeriod, bool fromTop = false, int[] rooms = null)
	{
		base.BeepGet(beepPeriod);
		RDClass.Vfx.oneshotSpotlights.Show(0);
	}

	public override void BeepSet(float beepPeriod, bool fromTop = false, int[] rooms = null)
	{
		base.BeepSet(beepPeriod);
		RDClass.Vfx.oneshotSpotlights.Show(1);
	}

	public override void BeepGo(float beepPeriod, bool fromTop = false, int[] rooms = null)
	{
		RDClass.Vfx.oneshotSpotlights.Hide();
	}
}
