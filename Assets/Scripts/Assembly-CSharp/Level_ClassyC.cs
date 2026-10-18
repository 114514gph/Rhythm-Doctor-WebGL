using System.Collections;
using DG.Tweening;
using UnityEngine;

public class Level_ClassyC : LevelBase
{
	private scrExecuteOnEveryHB cycler;

	public const float ColorBase = 30f;

	public Color colorMultiplierA = new Color(1f / 3f, 0f, 2f / 15f, 1f);

	public Color colorMultiplierB = new Color(0f, 2f / 15f, 1f / 3f, 1f);

	public float colorPowerA = 30f;

	public float colorPowerB = 30f;

	private scrExecuteOnEveryHB shakeybeat;

	public RowEntity oneshotRowComp => base.game.rows[5].ent;

	public override void Init()
	{
		songsUsed = new string[1] { "sndBarbraC" };
		heartExplodeType = HeartExplodeType.GatherAndCeil;
		multiroom = true;
		missesToCrackHeart = 2f;
		levelType = LevelType.Challenge;
		base.game.animationRush = base.game.animationRushSpecial;
	}

	public override IEnumerator LoadBigAssets()
	{
		specialSpeedHack = 1f;
		base.room0.kaleidoscopeMode = true;
		base.room0.camera.clearFlags = CameraClearFlags.Color;
		base.room0.kaleidoscope.kaleidoCam.targetTexture = RDClass.Vfx.rooms[1].camera.targetTexture;
		base.room1.camera.clearFlags = CameraClearFlags.Depth;
		base.room0.kaleidoscope.kaleidoCam.depth = RDClass.Vfx.rooms[1].camera.depth - 1f;
		RDClass.Vfx.AddMatrixBackgrounds(0, 1f, 0.1f);
		RDClass.Vfx.AddMatrixBackgrounds(0, 1.25f, 0.3f);
		scrVolumeTracker.Add((float s) =>
		{
			RDClass.Vfx.scrollYMultiplier = s;
		}, 0.1f, 6f);
		RDClass.Vfx.AddMatrixBackgrounds(1);
		RDClass.Vfx.BgColor(Color.black, 1);
		RDClass.Vfx.AddMatrixBackgrounds(2);
		yield return null;
	}

	public override void preactions()
	{
		switch (base.conductor.barNumber)
		{
		case 1:
			pauseOn7thBeat = true;
			base.game.nurseCount = true;
			base.conductor.SetBPM(130f);
			base.game.MakeRow(RowType.Classic, 1, 1f);
			PlayMusic(songsUsed[0]);
			base.game.OnBeatClassic(1, 1, 0f);
			RDClass.Vfx.BgColor(RDPresetColors.DarkGray);
			base.game.rows[4].rowSound = "sndStick";
			base.game.MakeRow(RowType.Oneshot, 5, 1f, Character.Barista, RDPlayer.P1, RDPlayer.P1, -1, isShadow: false, 1);
			base.rows[5].ent.SetBorderColor(Color.white);
			base.rows[5].rowSound = "sndKickEcho";
			base.game.SetPlayerHitSounds(RowType.Oneshot, "sndClapHitMassivePreEcho", "sndClapHitMassivePreEchoP2");
			base.game.MakeRow(RowType.Classic, 6, 1f, Character.Samurai, RDPlayer.P1, RDPlayer.P1, -1, isShadow: false, 2);
			RDClass.Vfx.BgColor(11776u, 2);
			scrVolumeTracker.Add((float x) =>
			{
				if (!(oneshotRowComp == null))
				{
					Color color = new Color(x * colorMultiplierA.r * 30f, x * colorMultiplierA.g * 30f, x * colorMultiplierA.b * 30f, 1f);
					Color color2 = new Color(x * colorMultiplierB.r * 30f, x * colorMultiplierB.g * 30f, x * colorMultiplierB.b * 30f, 1f);
					base.room0.kaleidoscope.SetColor(0, color, x * colorPowerA);
					base.room0.kaleidoscope.SetColor(1, color2, x * colorPowerB);
					Color a = Color.Lerp(color, color2, 0.5f);
					a = Color.Lerp(a, Color.white, 0.4f);
					a.a = 6f * x;
					oneshotRowComp.SetOverrideAdditiveColor(a);
					Color color3 = new Color(1f, x * 20f, 1f, 10f * x);
					oneshotRowComp.SetBorderColor(color3);
				}
			}, 0.03f, 0.3f);
			break;
		case 3:
			base.game.ChangeRowPlayers(singleplayer: false, new RDPlayer[1] { RDPlayer.P2 });
			break;
		case 4:
			LEDSign.status = "count!";
			base.rows[1].SetBeatSkipsLegacy("---xxx");
			base.game.MakeRow(RowType.Classic, 2, 0.5f, Character.Boy, RDPlayer.P1, RDPlayer.P2);
			break;
		case 5:
			LEDSign.status = "";
			base.game.rows[2].rowSound = "sndClosedHat";
			base.game.ChangeRowPlayers(singleplayer: false, new RDPlayer[2]
			{
				RDPlayer.P1,
				RDPlayer.P2
			});
			base.game.OnBeatClassic(2, 2, 0f);
			base.rows[1].SetBeatSkipsLegacy("------");
			base.rows[2].SetBeatSkipsLegacy("------");
			break;
		case 9:
			base.game.ChangeRowPlayers(singleplayer: false, new RDPlayer[2]
			{
				RDPlayer.P2,
				RDPlayer.P1
			});
			LEDSign.status = "";
			base.game.OnBeatClassic(3, 2, 4f);
			break;
		case 12:
			base.conductor.PlayBeatInCrotchets("sndJyi - ChineseCount1", 0f);
			base.conductor.PlayBeatInCrotchets("sndJyi - ChineseCount2", 1f);
			base.conductor.PlayBeatInCrotchets("sndJyi - ChineseCount1", 0f);
			base.conductor.PlayBeatInCrotchets("sndJyi - ChineseCount2", 0.5f);
			base.conductor.PlayBeatInCrotchets("sndJyi - ChineseCount3", 1f);
			base.conductor.PlayBeatInCrotchets("sndJyi - ChineseCount4", 1.5f);
			base.game.nurseCount = false;
			base.game.OneshotWarn(3f, 2f);
			base.game.OffBeat(2);
			base.game.OffBeat(3);
			base.game.AddBeat(2, 0f, 0.5f);
			base.game.AddBeat(2, 4f, 0.5f).SetTransferRow(5);
			break;
		case 13:
		{
			scrBeatLoopOneshot obj = base.game.OnBeatOneshotConstInterval(5, 5, 0f, 2f);
			obj.visualSpeedChange = false;
			obj.autoSetWeights = false;
			base.game.ChangeRowPlayers(singleplayer: false, new RDPlayer[2]
			{
				RDPlayer.P2,
				RDPlayer.P1
			});
			base.game.OffBeat(1);
			base.game.OffBeat(2);
			base.game.OffBeat(3);
			base.game.rows[3].rowSound = "sndHammer";
			base.rows[3].SetBeatSkipsLegacy("--x--x");
			shakeybeat = scrExecuteOnEveryHB.Add(() =>
			{
				base.tkCam.Shake(10, 5);
			});
			RowEntity ent = base.rows[5].ent;
			ent.oneshotRowController.waveHeightMultiplier = 1.5f;
			ent.oneshotRowController.waveWidthMultiplier = 1.5f;
			specialSpeedHack = 0.25f;
			base.game.SetVisualSpeed(0.25f);
			break;
		}
		case 16:
			base.game.ChangeRowPlayers(singleplayer: false, new RDPlayer[3]
			{
				RDPlayer.P1,
				RDPlayer.P1,
				RDPlayer.P2
			}, 6f);
			base.game.OneshotWarn(5f, 1f, 0f, sayReady: true);
			break;
		case 17:
			base.room0.kaleidoscope.kaleidoCam.backgroundColor = "1B3B52FF".HexToColor();
			base.game.SetVisualSpeed(1f);
			specialSpeedHack = 1f;
			base.game.ChangeLoopOneshot(5, 0f, 1f);
			break;
		case 18:
			base.game.SetOneshotSpillover(5, spillover: true);
			base.game.ChangeRowPlayers(singleplayer: false, new RDPlayer[3]
			{
				RDPlayer.P2,
				RDPlayer.P2,
				RDPlayer.P1
			}, 6f);
			break;
		case 19:
			base.game.OffBeat(5);
			base.game.AddBeatOneshot(5, 0f, 1f);
			base.game.AddBeatOneshot(5, 2f, 1f);
			base.game.AddBeatOneshot(5, 4f, 1f);
			base.game.AddBeatOneshot(5, 7f, 1f).SetTransferRow(6);
			break;
		case 20:
			base.game.OffBeat(1);
			base.game.OffBeat(2);
			base.game.OffBeat(3);
			base.game.AddBeat(6, 0f, 0.5f);
			base.game.AddBeat(6, 4f, 1f / 3f).SetOnDestroy(() =>
			{
				RDClass.Vfx.ShakeCam(30, 5, 2);
			});
			break;
		case 21:
			base.game.OnBeat(1);
			base.game.OnBeat(2);
			base.game.OnBeat(3);
			break;
		case 22:
			base.game.ChangeRowPlayers(singleplayer: false, new RDPlayer[3]
			{
				RDPlayer.P1,
				RDPlayer.P1,
				RDPlayer.P2
			}, 6f);
			break;
		case 24:
			base.game.MakeRow(RowType.Classic, 3, 0.25f, Character.Bodybuilder);
			break;
		case 25:
			base.game.OnBeatClassic(4, 3, 0f);
			break;
		case 26:
			base.game.ChangeRowPlayers(singleplayer: false, new RDPlayer[3]
			{
				RDPlayer.P2,
				RDPlayer.P2,
				RDPlayer.P1
			}, 6f);
			break;
		case 27:
			base.game.OnBeatClassic(4, 3, 4f);
			break;
		case 29:
			base.game.OffAllBeatLoops();
			break;
		case 30:
			base.game.DeleteRow(1);
			base.game.DeleteRow(2);
			base.game.DeleteRow(3);
			break;
		case 2:
		case 6:
		case 7:
		case 8:
		case 10:
		case 11:
		case 14:
		case 15:
		case 23:
		case 28:
			break;
		}
	}

	public override void actions()
	{
		int num = base.conductor.barNumber;
		if (num < 13)
		{
			float beatsToTravel = 5f;
			AddScrollyText(0f, "yi", 30f, beatsToTravel);
			AddScrollyText(1f, "er", 30f, beatsToTravel);
			AddScrollyText(2f, "san", 30f, beatsToTravel);
			if (num != 4)
			{
				AddScrollyText(3f, "si", 30f, beatsToTravel);
				AddScrollyText(4f, "wu", 30f, beatsToTravel);
				AddScrollyText(5f, "liu", 30f, beatsToTravel);
			}
			AddScrollyText(6f, "qi", 30f, beatsToTravel);
		}
		if (num > 4 && num < 13)
		{
			float beatsToTravel2 = 3f;
			float y = 100f;
			AddScrollyText(0f, "yi", y, beatsToTravel2);
			AddScrollyText(0.5f, "er", y, beatsToTravel2);
			AddScrollyText(1f, "san", y, beatsToTravel2);
			AddScrollyText(1.5f, "si", y, beatsToTravel2);
			AddScrollyText(2f, "wu", y, beatsToTravel2);
			AddScrollyText(2.5f, "liu", y, beatsToTravel2);
			AddScrollyText(3f, "qi", y, beatsToTravel2);
		}
		if (num >= 9 && num < 13)
		{
			float beatsToTravel3 = 3f;
			float y2 = 100f;
			AddScrollyText(4f, "yi", y2, beatsToTravel3);
			AddScrollyText(4.5f, "er", y2, beatsToTravel3);
			AddScrollyText(5f, "san", y2, beatsToTravel3);
			AddScrollyText(5.5f, "si", y2, beatsToTravel3);
			AddScrollyText(6f, "wu", y2, beatsToTravel3);
			AddScrollyText(6.5f, "liu", y2, beatsToTravel3);
			AddScrollyText(7f, "qi", y2, beatsToTravel3);
		}
		base.actions();
		switch (base.conductor.barNumber)
		{
		case 1:
			RDClass.Vfx.BgColor(uint.MaxValue);
			if (!GC.twoPlayerMode)
			{
				RDClass.Vfx.SetSpriteMode(SpriteMode.Silhouette, RDPresetColors.Avacado.ARGBToColor());
				cycler = scrExecuteOnEveryHB.Add(() =>
				{
					RDClass.Vfx.CycleOverlayColor();
				});
			}
			break;
		case 13:
			RDClass.Vfx.roomsAnimationDuration = 0f;
			RDClass.Vfx.ShowRooms("0d,1");
			RDClass.Vfx.BgColor(uint.MaxValue);
			if (cycler != null)
			{
				cycler.Remove();
			}
			RDClass.Vfx.SetSpriteMode(SpriteMode.Glow, Color.white);
			break;
		case 20:
			RDClass.Vfx.ShowRooms("0d,1d,2", 0f);
			RDClass.Vfx.GreenScreenOn(2);
			break;
		case 21:
			shakeybeat.Remove();
			RDClass.Vfx.ShowRooms("0,2d", 0f);
			RDClass.Vfx.Flash();
			if (cycler != null)
			{
				cycler.Remove();
			}
			RDClass.Vfx.SetSpriteMode(SpriteMode.None, Color.white);
			RDClass.Vfx.BgColor(RDPresetColors.DarkGray);
			break;
		case 30:
			RDClass.Vfx.BgColor(0u);
			base.rankscreen.AdvanceGameover();
			break;
		case 31:
			base.rankscreen.AdvanceGameover();
			break;
		case 32:
			base.rankscreen.AdvanceGameover();
			break;
		}
	}

	public void AddScrollyText(float beat, string text, float y, float beatsToTravel, Color? color = null, int roomIndex = 0)
	{
		if (!base.conductor.isScrubbing)
		{
			scrExecuteOnCertainBeat.Add(() =>
			{
				RDScrollyText.Create().Setup(text, y, beatsToTravel, Ease.Linear, 20f, 0f, randomColors: false, color).Run();
			}, beat);
		}
	}
}
