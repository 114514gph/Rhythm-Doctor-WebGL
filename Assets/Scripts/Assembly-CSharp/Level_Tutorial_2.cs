using System.Collections;
using UnityEngine;

public class Level_Tutorial_2 : LevelBase
{
	private bool helpgiven;

	private string dialogueName = "diaTutorialIntimate";

	private bool spacePressed;

	public override void Init()
	{
		levelType = LevelType.Tutorial;
		helpgiven = false;
		levelToSkipTo = Level.Intimate;
		songsUsed = new string[1] { "sndTutorial" };
		base.ink.Load(dialogueName);
		multiroom = true;
	}

	public RDInk DiaTutorialHit(string node)
	{
		base.conductor.SetPlayStyle(PlayStyleChange.OnNextBar);
		printe("running node: " + node);
		return base.ink.Run(node);
	}

	public override IEnumerator LoadBigAssets()
	{
		RDUtils.ShowTutorialTheme("422C87".HexToColor(), "210F72".HexToColor());
		yield return null;
	}

	private void callText2()
	{
		base.ink.Run("Tutorial1", () =>
		{
			base.game.handController0.SlideIn();
		}).Freeze();
	}

	private void callTextMiss()
	{
		if (!helpgiven)
		{
			helpgiven = true;
			base.game.StopBeats(killHalfwayPlayingSounds: false);
			base.ink.Run("Tutorial1Miss", () =>
			{
				base.game.OnBeat(1);
				base.room0.showTextAboveBeatboxes = true;
			}).Prolong().WaitForNextBarOnEnd();
			base.game.OffBeat(1);
		}
	}

	private void callTextMissCompletely()
	{
		base.ink.Run("Tutorial1NoInput");
	}

	private void callTextHit()
	{
		DiaTutorialHit("Tutorial1Perfect");
	}

	private void callTextHit2()
	{
		DiaTutorialHit("Tutorial2");
	}

	private void callTextHit3()
	{
		DiaTutorialHit("Tutorial3");
	}

	private void callTextHit4()
	{
		DiaTutorialHit("Tutorial4");
	}

	private void callTextHit5()
	{
		base.ink.Run("Tutorial5").Prolong().JumpOnEnd();
		base.game.OffBeat(1);
	}

	private void spaceprompt()
	{
		if (!spacePressed)
		{
			base.ink.Run("Tutorial0NoInput").Prolong().JumpOnEnd();
		}
	}

	public override void preactions()
	{
		switch (base.conductor.barNumber)
		{
		case 1:
			base.room0.showTextAboveBeatboxes = true;
			scrExecuteOnUpdate.Add(() =>
			{
				if (RDInput.anyPlayerPress)
				{
					spacePressed = true;
				}
			});
			base.game.nurseCount = true;
			base.game.handController0.HideInstant();
			base.ink.Run("Tutorial0").Prolong().JumpOnEnd();
			base.conductor.PlayNoSong();
			base.conductor.SetBPM(88f);
			break;
		case 2:
			base.conductor.Freeze(PlayStyleChange.Loop);
			PlayMusic(songsUsed[0]);
			base.game.MakeRow(RowType.Classic, 1, 1f, Character.Farmer);
			RDClass.Vfx.SetSpriteMode(SpriteMode.Glow, Color.black.WithAlpha(0.2f));
			base.rows[1].rowSound = "sndHatTight";
			base.rows[1].rowSoundVolMultiplier = 1.5f;
			if (Narration.IsEnabled)
			{
				base.conductor.PlayBeat("sndPatientConnect", 0f, 1f, RDUtils.GetMixerGroup("CuesoundsParent"));
			}
			Timer.Add(callText2, 3f);
			base.game.nurseCount = true;
			break;
		case 3:
			if (!GC.twoPlayerMode)
			{
				base.game.statusText.SetStatusText(RDString.Get("status.hitOnSeventhBeat"), null, 4f, narrate: true);
			}
			LEDSign.showMarginError = true;
			base.game.OnBeatClassic(1, 1, 0f);
			scrExecuteOnHit.Add(callTextHit, HitType.Hit, persistent: false, autoDelete: true);
			scrExecuteOnHit.Add(callTextMiss, HitType.JustMiss, persistent: true);
			scrExecuteOnHit.Add(callTextMiss, HitType.BigMiss, persistent: true);
			scrExecuteOnHit.Add(callTextMissCompletely, HitType.MissCompletely, persistent: true);
			break;
		case 4:
			base.game.OnBeat(1);
			base.conductor.Freeze(PlayStyleChange.BeatLoopOnly);
			scrExecuteOnHit.Add(callTextHit2, HitType.Hit, persistent: false);
			break;
		case 5:
			base.game.OnBeat(1);
			base.conductor.Freeze(PlayStyleChange.BeatLoopOnly);
			scrExecuteOnHit.Add(callTextHit4, HitType.Hit, persistent: false);
			break;
		case 6:
			base.game.OnBeat(1);
			base.game.nurseCount = false;
			base.conductor.Freeze(PlayStyleChange.BeatLoopOnly);
			scrExecuteOnHit.Clear(clearUnoverwritables: false);
			scrExecuteOnHit.Add(callTextHit5, HitType.Hit, persistent: false);
			break;
		case 7:
			base.game.OffBeat(1);
			base.game.DeleteRow(1);
			base.game.handController0.Show(show: false);
			scrExecuteOnCertainBeat.Add(() =>
			{
				RDClass.Vfx.FadeOut();
				FadeOutMusic();
			}, 3f);
			scrExecuteOnCertainBeat.Add(() =>
			{
				scnBase.GoToLevelWithEnum(levelToSkipTo);
			}, 5f);
			break;
		}
	}

	public override void actions()
	{
		switch (base.conductor.barNumber)
		{
		case 1:
			NarrateDescription("Intimate.tutorial.desc1");
			break;
		case 2:
			Narration.NarrateRowInfo(base.rows[1], NarrateInfoType.Connect, soundOnly: false, narrateSkipBeats: false, null, skipsUnstable: false, RDPlayer.AutoDetect, -1);
			break;
		case 3:
			scrExecuteOnCertainBeat.Add(() =>
			{
				base.ink.Run("Tutorial1Nurse");
			}, 2f);
			break;
		}
	}

	public void ShowRowX()
	{
		base.rows[1].SetBeatSkipsLegacy("----xx");
		if (Narration.IsEnabled)
		{
			scrConductor.PlayImmediately("sndPatientUpdate", 1f, RDUtils.GetMixerGroup("CuesoundsParent"));
			Narration.NarrateRowInfo(base.rows[1], NarrateInfoType.Update, soundOnly: false, narrateSkipBeats: true, null, skipsUnstable: false, RDPlayer.AutoDetect, -1);
		}
	}
}
