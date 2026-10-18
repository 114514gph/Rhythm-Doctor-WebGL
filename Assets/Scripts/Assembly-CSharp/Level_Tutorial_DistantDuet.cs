using System.Collections;
using DG.Tweening;
using UnityEngine;

public class Level_Tutorial_DistantDuet : LevelBase
{
	private const int PHASE1HITS = 3;

	private const int PHASE2HITS = 3;

	private const bool helpgiven = false;

	private int phase1hitstogo;

	private bool helpgiven2;

	private int phase2hitstogo;

	public override void Init()
	{
		songsUsed = new string[1] { "sndTutorial" };
		levelType = LevelType.Tutorial;
		levelToSkipTo = Level.DistantDuet;
		phase1hitstogo = 3;
		phase2hitstogo = 3;
	}

	public RDInk DiaTutorial(string node, RDInk.RDInkCallback onComplete = null)
	{
		return base.ink.Run("diaTutorialDistantDuet", node, onComplete);
	}

	private void callText2()
	{
		DiaTutorial("OnMusic", () =>
		{
			base.game.MakeRow(RowType.Classic, 1, 1f, Character.Farmer);
			RDClass.Vfx.SetSpriteMode(SpriteMode.Glow, Color.black.WithAlpha(0.2f));
			base.game.handController0.SlideIn();
		}).Prolong().WaitForNextBarOnEnd();
	}

	public override IEnumerator LoadBigAssets()
	{
		RDUtils.ShowTutorialTheme("3CC349".HexToColor(), "2DAD18".HexToColor());
		yield return null;
	}

	private void callTextMiss()
	{
	}

	private void callTextMisscom()
	{
		if (!helpgiven2)
		{
			DiaTutorial("FirstMissCompletely", () =>
			{
				helpgiven2 = true;
			});
		}
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
		if (phase1hitstogo == 2)
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
		if (phase1hitstogo == 1)
		{
			DiaTutorial("Phase1_2Hit");
		}
		if (phase1hitstogo == 0)
		{
			scrExecuteOnHit.Clear();
			DiaTutorial("Interlude1").Prolong().WaitForNextBarOnEnd();
		}
	}

	private void secondphasehit()
	{
		phase2hitstogo--;
		if (phase2hitstogo != 0)
		{
			return;
		}
		scrExecuteOnHit.Clear();
		string node = (missedAny ? "Interlude2" : "Interlude2NoMisses");
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
			phase1hitstogo = 3;
			phase2hitstogo = 3;
			base.game.nurseCount = true;
			base.game.handController0.HideInstant();
			LEDSign.status = RDString.Get("status.skipTutorial");
			base.conductor.SetBPM(88f);
			DiaTutorial("NoMusic").Prolong().JumpOnEnd();
			base.room0.showTextAboveBeatboxes = true;
			break;
		case 2:
			PlayMusic(songsUsed[0]);
			Timer.Add(callText2, 3f);
			base.game.nurseCount = true;
			break;
		case 3:
			base.conductor.Freeze(PlayStyleChange.BeatLoopOnly);
			LEDSign.status = RDString.Get("status.hitOnSeventhBeat");
			LEDSign.showMarginError = true;
			SetUpFirstPhaseExecutes();
			base.game.OnBeatClassic(1, 1, 0f, 1f, 1, SwingBeatType.SwingCustom, offOtherBeats: false, 0.5f);
			scrExecuteOnHit.Add(firstphasehit, HitType.Hit, persistent: true);
			break;
		case 4:
			scrExecuteOnHit.Clear();
			base.game.OffBeat(1);
			scrExecuteOnHit.Add(secondphasehit, HitType.Hit, persistent: true);
			base.conductor.Freeze(PlayStyleChange.BeatLoopOnly);
			base.game.OnBeatClassic(2, 1, 1f, 1f, 1, SwingBeatType.SwingCustom, offOtherBeats: false, 0.5f);
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
