using System.Collections;
using RDLevelEditor;
using UnityEngine;

public class Level_IntimateH : LevelBase
{
	private string dialogueName = "diaIntimate";

	private Background[] skyBackgrounds;

	private Background sky;

	private Background cityFront;

	private Background cityBack;

	private CameraFilterPack_Atmosphere_Rain_Pro rainPro;

	private RDGirlWard girlWard;

	public Level_IntimateH(RDLevelData data)
		: base(data)
	{
	}

	public override void Init()
	{
		heartExplodeType = HeartExplodeType.GatherAndCeil;
		songsUsed = new string[1] { GC.twoPlayerMode ? "sndIntimate2P" : "sndIntimate" };
		hideHandsAfterStart = true;
	}

	public override IEnumerator LoadBigAssets()
	{
		girlWard = LoadLevelAsset<RDGirlWard>();
		sky = Background.Create("Intimate_SkySS_Blue/0", base.gc.intimate_skyScrollX);
		cityBack = Background.Create("Intimate_CityBackSS_Blue/0", base.gc.intimate_cityBackScrollX);
		cityBack.SetTileType(TileType.Horizontal);
		cityFront = Background.Create("Intimate_CityFrontSS_Blue/0", base.gc.intimate_cityFrontScrollX);
		cityFront.SetTileType(TileType.Horizontal);
		skyBackgrounds = new Background[3] { sky, cityBack, cityFront };
		Background[] array = skyBackgrounds;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].visible = false;
		}
		base.ink.Load(dialogueName);
		yield return null;
	}

	public override void preactions()
	{
		switch (base.conductor.barNumber)
		{
		case 1:
		{
			base.conductor.SetBPM(90f);
			PlayMusic(songsUsed[0]);
			RDClass.Vfx.BgColor(RDPresetColors.Black);
			base.room0.SetVignette(enabled: true);
			base.room1.SetVignette(enabled: true);
			rainPro = base.tkCam.GetOrAddComponent<CameraFilterPack_Atmosphere_Rain_Pro>();
			rainPro.enabled = false;
			Row row = base.rows[1];
			row.tick = 0.5f;
			ParticleSystem.MainModule main = row.ent.playerParticle.main;
			main.startColor = "E382FFE3".HexToColor();
			base.room0.rowContainer.LocalMoveXY(25f, -37f);
			base.room1.rowContainer.LocalMoveXY(25f, -37f);
			if (GC.twoPlayerMode)
			{
				row.ent.SetRowMarkerCorner(rightCorner: false);
				base.game.rows[3].ent.SetRowMarkerCorner(rightCorner: false);
			}
			RDClass.Vfx.SetSpriteMode(SpriteMode.Glow, Color.black.WithAlpha(0.2f));
			break;
		}
		case 3:
			base.game.rows[1].ent.character.PlayExpression("neutral");
			base.game.gameState = GameState.Handmode;
			base.game.handControllerOnTop.SetHandsInFront(inFront: false);
			break;
		case 27:
			RDClass.Vfx.BgColor(Color.black);
			base.game.DeleteRow(1);
			base.game.DeleteRow(2);
			base.conductor.crotchetsPerBar = 4;
			break;
		case 28:
			base.rankscreen.AdvanceGameover();
			break;
		case 29:
			base.rankscreen.AdvanceGameover();
			break;
		case 30:
			base.rankscreen.AdvanceGameover();
			break;
		case 2:
		case 4:
		case 5:
		case 6:
		case 7:
		case 8:
		case 9:
		case 10:
		case 11:
		case 12:
		case 13:
		case 14:
		case 15:
		case 16:
		case 17:
		case 18:
		case 19:
		case 20:
		case 21:
		case 22:
		case 23:
		case 24:
		case 25:
		case 26:
			break;
		}
	}

	public override void actions()
	{
		switch (base.conductor.barNumber)
		{
		case 1:
			base.game.gameState = GameState.Cutscene;
			base.game.rows[1].ent.character.PlayExpression("barely");
			scrExecuteOnCertainBeat.Add(() =>
			{
				base.ink.Run("IntroHard", () =>
				{
					base.game.gameState = GameState.Handmode;
				});
			}, 1f);
			break;
		case 14:
			girlWard.BoyRun();
			scrExecuteOnCertainBeat.Add(() =>
			{
				base.game.gameState = GameState.Cutscene;
				RDClass.Vfx.ShoutEffect();
				base.ink.Run("InterruptionHard1");
			}, 6f);
			TransitionToSkylineBackground();
			break;
		case 16:
			base.game.gameState = GameState.Handmode;
			break;
		case 26:
			if (base.game.mistakesManager.mistakes <= rankLowerBounds[3] - 2f)
			{
				base.rows[1].SetBeatSkipsLegacy("------", 5f);
				base.rows[2].SetBeatSkipsLegacy("------", 5f);
			}
			break;
		}
	}

	public void TransitionToSkylineBackground()
	{
		scrExecuteOnCertainBeat.Add(() =>
		{
			RDClass.Vfx.ReverseFlash(base.conductor.crotchet * 2f);
		}, 4f);
		scrExecuteOnCertainBeat.Add(() =>
		{
			RDClass.Vfx.Flash(-1, 2f);
			rainPro.enabled = true;
			base.room0.rowContainer.LocalMoveXY(0f, 0f);
			girlWard.gameObject.SetActive(value: false);
			Background[] array = skyBackgrounds;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].visible = true;
			}
			if (GC.twoPlayerMode)
			{
				base.game.rows[1].ent.SetRowMarkerCorner(rightCorner: true);
			}
		}, 6f);
	}
}
