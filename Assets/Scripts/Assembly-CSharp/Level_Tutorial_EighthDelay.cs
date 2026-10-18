using DG.Tweening;
using UnityEngine;

public class Level_Tutorial_EighthDelay : LevelBase
{
	private const int PHASE1HITS = 6;

	private const int PHASE2HITS = 14;

	private int phase1hitstogo;

	private bool helpgiven;

	private bool helpgiven2;

	public override void Init()
	{
		songsUsed = new string[1] { "sndTutorial" };
		levelType = LevelType.Tutorial;
		levelToSkipTo = Level.SVT;
		phase1hitstogo = 6;
	}

	public RDInk DiaTutorial(string node, RDInk.RDInkCallback onComplete = null)
	{
		return base.ink.Run("diaTutorialEighthDelay", node, onComplete);
	}

	private void callText2()
	{
		DiaTutorial("OnMusic", () =>
		{
			base.game.MakeRow(RowType.Oneshot, 1, 2.5f, Character.Farmer);
			base.game.handController0.SlideIn();
		}).Prolong().WaitForNextBarOnEnd();
	}

	private void callTextMiss()
	{
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
		if (!helpgiven && !helpgiven2)
		{
			DiaTutorial("FirstMissCompletely", () =>
			{
				helpgiven2 = true;
				base.conductor.barNumberToGo = 3;
			}).Prolong().WaitForNextBarOnEnd();
		}
	}

	private void missedAnything()
	{
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
			if (!missedAny)
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
		if (phase1hitstogo != 0)
		{
			return;
		}
		scrExecuteOnHit.Clear();
		DiaTutorial(missedAny ? "Interlude1" : "Interlude1Perfect", () =>
		{
			base.game.DeleteRow(1);
			RDClass.Vfx.FadeOut();
			FadeOutMusic(1f);
			DOVirtual.DelayedCall(1f, () =>
			{
				scnBase.GoToLevelWithEnum(Level.EighthDelay);
			});
		}).Prolong().WaitForNextBarOnEnd();
	}

	public override void preactions()
	{
		switch (base.conductor.barNumber)
		{
		case 1:
		{
			base.game.SetPlayerHitSounds(RowType.Oneshot, "sndClapHitMassivePreEcho", "sndClapHitMassivePreEchoP2");
			phase1hitstogo = 6;
			base.game.nurseCount = true;
			base.game.handController0.HideInstant();
			LEDSign.status = RDString.Get("status.skipTutorial");
			Background background = Background.Create("TutorialNoteBlack", 1f, 1f);
			background.shaderDataSource = background;
			float num = 0.4f;
			background.SetColor(new Color(num, num, num, 1f));
			RDClass.Vfx.BgColor(3214520576u.RGBAToColor());
			base.conductor.SetBPM(88f);
			DiaTutorial("NoMusic").Prolong().JumpOnEnd();
			break;
		}
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
			base.game.OneshotSayGetSet(0f, 1.5f);
			break;
		case 4:
			base.conductor.Freeze(PlayStyleChange.Loop);
			base.game.AddBeatOneshot(1, 0f, 1.5f);
			scrExecuteOnHit.Add(firstphasehit, HitType.Hit, persistent: true);
			scrExecuteOnHit.Add(missedAnything, HitType.AnyMiss, persistent: true);
			if (helpgiven)
			{
				base.conductor.PlayBeatInCrotchets("sndTypewriter", 0.5f);
				base.conductor.PlayBeatInCrotchets("sndTypewriter", 1f);
				base.conductor.PlayBeatInCrotchets("sndTypewriter", 1.5f);
			}
			break;
		}
	}

	public override void actions()
	{
		_ = base.conductor.barNumber;
		_ = 3;
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
