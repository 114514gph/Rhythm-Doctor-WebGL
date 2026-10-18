using System.Collections;
using DG.Tweening;
using UnityEngine;

public class Level_Tutorial_OneshotIntro : LevelBase
{
	private const int Phase1Hits = 6;

	private const int Phase2Hits = 6;

	private int phase1HitsLeft;

	private int phase2HitsLeft;

	private bool firstTimeHitPossible = true;

	private bool anyMisses;

	private bool helpGiven;

	private bool helpgiven2;

	private bool nursesaid;

	public override void Init()
	{
		levelType = LevelType.Tutorial;
		levelToSkipTo = Level.Lofi;
		phase1HitsLeft = 6;
		phase2HitsLeft = 6;
		songsUsed = new string[1] { "sndTutorial" };
	}

	public RDInk DiaTutorial(string node, RDInk.RDInkCallback onComplete = null)
	{
		return base.ink.Run("diaTutorialOneshotIntro", node, onComplete);
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
		if (!helpGiven && !helpgiven2)
		{
			helpGiven = true;
			DiaTutorial("FirstMiss", () =>
			{
				base.conductor.barNumberToGo = 3;
			}).Prolong().WaitForNextBarOnEnd();
		}
		if (!helpGiven && helpgiven2)
		{
			helpGiven = true;
			DiaTutorial("FirstMissAfterMissCompletely", () =>
			{
				base.conductor.barNumberToGo = 3;
			}).Prolong().WaitForNextBarOnEnd();
		}
	}

	private void callTextMisscom()
	{
		firstTimeHitPossible = false;
		if (!helpGiven && !helpgiven2)
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
		RDUtils.ShowTutorialTheme("B163E0".HexToColor(), "9F55CF".HexToColor());
		yield return null;
	}

	private void missedAnything()
	{
		anyMisses = true;
	}

	private void SetUpFirstPhaseExecutes()
	{
		scrExecuteOnHit.Add(callTextMiss, HitType.JustMiss, persistent: true);
		scrExecuteOnHit.Add(callTextMiss, HitType.BigMiss, persistent: true);
		scrExecuteOnHit.Add(callTextMisscom, HitType.MissCompletely, persistent: true);
	}

	private void firstphasehit()
	{
		phase1HitsLeft--;
		if (phase1HitsLeft == 5)
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
		if (phase1HitsLeft == 4)
		{
			DiaTutorial("Phase1_2Hit");
		}
		if (phase1HitsLeft == 3)
		{
			DiaTutorial("Phase1_3Hit");
		}
		if (phase1HitsLeft == 2)
		{
			DiaTutorial("Phase1_4Hit");
		}
		if (phase1HitsLeft == 1)
		{
			DiaTutorial("Phase1_5Hit");
		}
		if (phase1HitsLeft == 0)
		{
			scrExecuteOnHit.Clear();
			DiaTutorial("Interlude1").Prolong().WaitForNextBarOnEnd();
		}
		firstTimeHitPossible = false;
	}

	private void secondphasehit()
	{
		phase2HitsLeft--;
		if (phase2HitsLeft != 0)
		{
			return;
		}
		scrExecuteOnHit.Clear();
		string node = (anyMisses ? "Interlude2" : "Interlude2NoMisses");
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
			phase1HitsLeft = 6;
			phase2HitsLeft = 6;
			base.game.nurseCount = true;
			base.game.handController0.HideInstant();
			LEDSign.status = RDString.Get("status.skipTutorial");
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
			break;
		case 6:
			base.conductor.crotchetsPerBar = 16;
			base.conductor.Freeze(PlayStyleChange.Loop);
			base.game.AddBeatOneshot(1, 0f, 2f);
			base.game.AddBeatOneshot(1, 3.5f, 2.5f);
			base.game.AddBeatOneshot(1, 8f, 2f);
			base.game.AddBeatOneshot(1, 11.5f, 2.5f);
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
