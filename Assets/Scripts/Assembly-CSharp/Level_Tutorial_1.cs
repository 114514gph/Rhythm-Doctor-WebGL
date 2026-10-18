using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level_Tutorial_1 : LevelBase
{
	private bool helpGiven;

	private Background notes;

	private bool spacePressed;

	private string dialogueFile;

	private Dictionary<string, object> inkParams = new Dictionary<string, object>();

	private int pressCount;

	private int shoutCount;

	private Character characterToShow
	{
		get
		{
			if (!scnBase.isDivekick)
			{
				return Character.Farmer;
			}
			return Character.Tentacle;
		}
	}

	public override void Init()
	{
		levelType = LevelType.Tutorial;
		helpGiven = false;
		bool isBooth = scnBase.isBooth;
		bool isCustomButton = scnBase.isCustomButton;
		songsUsed = new string[1] { "sndTutorial" };
		if (isBooth)
		{
			levelToSkipTo = Level.OrientalInsomniac;
			if (isCustomButton)
			{
				dialogueFile = (GC.twoPlayerMode ? "diaExpoTwoPlayer" : "diaTutorialExpo");
			}
			else
			{
				dialogueFile = (GC.twoPlayerMode ? "diaExpoTwoPlayer" : "diaTutorialExpoButton");
			}
		}
		else
		{
			dialogueFile = (scnBase.isMobile ? "diaTutorial.mobile" : "diaExpoTwoPlayer");
			levelToSkipTo = Level.OrientalTechno;
		}
		base.ink.Load(dialogueFile);
		if (isCustomButton)
		{
			inkParams["P1ControlName"] = RDString.Get("dialogue.P1ControlName.customButton");
			inkParams["P2ControlName"] = RDString.Get("dialogue.P2ControlName.customButton");
			inkParams["AnyControlName"] = RDString.Get("dialogue.AnyControlName.customButton");
		}
		Color c = ColorWithCeilingSaturation(base.gc.player1StripColor);
		Color c2 = ColorWithCeilingSaturation(base.gc.player2StripColor);
		inkParams["TopStripColor"] = c.ToHex();
		inkParams["BottomStripColor"] = c2.ToHex();
	}

	private Color ColorWithCeilingSaturation(Color color)
	{
		Color.RGBToHSV(color, out var H, out var S, out var V);
		S = Mathf.Min(S, 0.7f);
		return Color.HSVToRGB(H, S, V);
	}

	public override IEnumerator LoadBigAssets()
	{
		RDUtils.ShowTutorialTheme("e2a21c".HexToColor(), "e2ba1c".HexToColor().WithAlpha(1f));
		yield return null;
	}

	public RDInk DiaTutorialHit(string node)
	{
		base.conductor.SetPlayStyle(PlayStyleChange.OnNextBar);
		return base.ink.Run(node, null, inkParams);
	}

	private void callText2()
	{
		string knotName = (scnBase.isDivekick ? "expoTwoPlayer1_divekick" : "expoTwoPlayer1");
		base.ink.Run(knotName, () =>
		{
			base.game.MakeRow(RowType.Classic, 1, 1f, characterToShow);
			base.game.handController0.SlideIn();
		}, inkParams).Freeze();
	}

	private void callTextMiss()
	{
		pressCount = 0;
		if (!helpGiven)
		{
			helpGiven = true;
			base.game.StopBeats(killHalfwayPlayingSounds: false);
			base.ink.Run("expoTwoPlayer1Miss", () =>
			{
				base.game.OnBeat(1);
			}, inkParams).Prolong().BeatLoopOnlyOnEnd();
			base.game.OffBeat(1);
		}
	}

	private void callTextMisscom()
	{
		pressCount = 0;
		base.ink.Run("expoTwoPlayer1NoInput", null, inkParams);
	}

	private void callTextHit()
	{
		pressCount = 0;
		if (!missedAny)
		{
			DiaTutorialHit("expoTwoPlayer1Perfect");
		}
		else
		{
			DiaTutorialHit("expoTwoPlayer3");
		}
		base.conductor.SetPlayStyle(PlayStyleChange.OnNextBar);
	}

	private void callTextHit2()
	{
		DiaTutorialHit("expoTwoPlayer2");
		base.conductor.SetPlayStyle(PlayStyleChange.OnNextBar);
	}

	private void callTextHit3()
	{
		DiaTutorialHit("expoTwoPlayer3");
		base.conductor.SetPlayStyle(PlayStyleChange.OnNextBar);
	}

	private void callTextHit4()
	{
		DiaTutorialHit("expoTwoPlayer4");
		base.conductor.SetPlayStyle(PlayStyleChange.OnNextBar);
	}

	private void callTextHit5()
	{
		base.ink.Run("expoTwoPlayer5", null, inkParams).Prolong().JumpOnEnd();
		base.game.OffBeat(1);
	}

	private void spaceprompt()
	{
		if (!spacePressed)
		{
			base.ink.Run("expoTwoPlayer0NoInput", null, inkParams).Prolong().JumpOnEnd();
		}
	}

	public override void preactions()
	{
		pressCount = 0;
		switch (base.conductor.barNumber)
		{
		case 1:
			base.room0.showTextAboveBeatboxes = true;
			scrExecuteOnUpdate.Add(() =>
			{
				if (RDInput.anyPlayerPress)
				{
					spacePressed = true;
					if (base.game.beats.Count > 0)
					{
						pressCount++;
					}
				}
				if (pressCount > 7 && base.conductor.barNumber == 3)
				{
					shoutCount++;
					string knotName = ((shoutCount > 1) ? "expoTwoPlayerEveryBeat2" : "expoTwoPlayerEveryBeat");
					pressCount = 0;
					base.game.StopBeats(killHalfwayPlayingSounds: false);
					RDClass.Vfx.ShoutEffect();
					base.ink.Run(knotName, () =>
					{
						base.game.OnBeat(1);
					}, inkParams).Prolong().BeatLoopOnlyOnEnd();
				}
			});
			base.game.nurseCount = true;
			base.game.handController0.HideInstant();
			LEDSign.status = RDString.Get("status.skipTutorial");
			Timer.Add(spaceprompt, 8f);
			base.ink.Run("expoTwoPlayer0", null, inkParams).Prolong().JumpOnEnd();
			base.conductor.SetBPM(88f);
			RDClass.Vfx.SetSpriteMode(SpriteMode.Glow, Color.black.WithAlpha(0.2f));
			break;
		case 2:
			base.conductor.Freeze(PlayStyleChange.Loop);
			PlayMusic(songsUsed[0]);
			Timer.Add(callText2, 3f);
			base.game.nurseCount = true;
			break;
		case 3:
			LEDSign.status = RDString.Get("status.hitOnSeventhBeatP1");
			LEDSign.showMarginError = true;
			base.game.OnBeatClassic(1, 1, 0f);
			scrExecuteOnHit.Add(callTextHit, HitType.Hit, persistent: false);
			scrExecuteOnHit.Add(callTextMiss, HitType.JustMiss, persistent: true);
			scrExecuteOnHit.Add(callTextMiss, HitType.BigMiss, persistent: true);
			scrExecuteOnHit.Add(callTextMisscom, HitType.MissCompletely, persistent: true);
			break;
		case 4:
			base.game.OnBeat(1);
			base.conductor.Freeze(PlayStyleChange.BeatLoopOnly);
			scrExecuteOnHit.Add(callTextHit2, HitType.Hit, persistent: false);
			break;
		case 5:
			base.game.ChangeRowPlayers(singleplayer: false, new RDPlayer[1] { RDPlayer.P2 }, warningFlash: false);
			base.game.OnBeat(1);
			base.conductor.Freeze(PlayStyleChange.BeatLoopOnly);
			scrExecuteOnHit.Add(callTextHit3, HitType.Hit, persistent: false);
			break;
		case 6:
			base.game.OnBeat(1);
			base.conductor.Freeze(PlayStyleChange.BeatLoopOnly);
			scrExecuteOnHit.Add(callTextHit4, HitType.Hit, persistent: false);
			break;
		case 7:
			base.game.OnBeat(1);
			base.game.nurseCount = false;
			base.conductor.Freeze(PlayStyleChange.BeatLoopOnly);
			scrExecuteOnHit.Clear(clearUnoverwritables: false);
			scrExecuteOnHit.Add(callTextHit5, HitType.Hit, persistent: false);
			break;
		case 8:
			base.game.OffBeat(1);
			base.game.DeleteRow(1);
			scrExecuteOnCertainBeat.Add(() =>
			{
				RDClass.Vfx.FadeOut();
				FadeOutMusic();
			}, 0f);
			scrExecuteOnCertainBeat.Add(() =>
			{
				scnBase.GoToLevelWithEnum(levelToSkipTo);
			}, 1f);
			break;
		}
	}

	public override void actions()
	{
		pressCount = 0;
		if (base.conductor.barNumber == 3 && shoutCount == 0)
		{
			base.ink.Run("expoTwoPlayer01Nurse", null, inkParams);
		}
	}
}
