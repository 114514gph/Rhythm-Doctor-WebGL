using System.Collections;
using DG.Tweening;
using RDLevelEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class Level_Intimate : LevelBase
{
	private string dialogueName = "diaIntimate";

	private Background[] skyBackgrounds;

	private Background sky;

	private Background cityFront;

	private Background cityBack;

	private Background couple;

	private Background shadow;

	private CameraFilterPack_Atmosphere_Rain_Pro rainPro;

	private RDBoyWard boyWard;

	public Level_Intimate(RDLevelData data)
		: base(data)
	{
	}

	public override void Init()
	{
		heartExplodeType = HeartExplodeType.GatherAndCeil;
		songsUsed = new string[1] { "sndIntimateExtended" };
		heartExplodeMultiplier = 0.5f;
		hideHandsAfterStart = true;
	}

	public override IEnumerator LoadBigAssets()
	{
		sky = Background.Create("Intimate_SkySS", base.gc.intimate_skyScrollX);
		cityBack = Background.Create("Intimate_CityBackSS", base.gc.intimate_cityBackScrollX);
		cityBack.SetTileType(TileType.Horizontal);
		cityFront = Background.Create("Intimate_CityFrontSS", base.gc.intimate_cityFrontScrollX);
		cityFront.SetTileType(TileType.Horizontal);
		couple = Background.CreateSingle("Intimate_CoupleSS", new int[2] { 0, 1 }, 1.5f);
		shadow = Background.CreateSingle("Intimate_Lighting_ShadowSS");
		shadow.SetAlpha(0f);
		skyBackgrounds = new Background[5] { sky, cityBack, cityFront, couple, shadow };
		Background[] array = skyBackgrounds;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].visible = false;
		}
		boyWard = LoadLevelAsset<RDBoyWard>();
		boyWard.transform.LocalMoveXY(RDClass.Vfx.RDWidth / 2f, RDClass.Vfx.RDHeight / 2f);
		base.ink.Load(dialogueName);
		yield return null;
	}

	private IEnumerator PlayFinalAnimation()
	{
		cityBack.velocity = Vector2.zero;
		cityFront.velocity = Vector2.zero;
		sky.velocity = Vector2.zero;
		yield return new WaitForSeconds(0.5f);
		base.game.handControllerOnTop.Show(show: false);
		float num = base.conductor.crotchet * 16f;
		float num2 = 132f - RDClass.Vfx.RDHeight / 2f;
		cityBack.transform.DOLocalMoveY(28f, num);
		cityFront.transform.DOLocalMoveY(35f, num);
		Ease ease = Ease.OutQuart;
		couple.transform.DOLocalMoveY(num2, num).SetEase(ease);
		shadow.transform.DOLocalMoveY(num2 - 10f, num).SetEase(ease);
		DOVirtual.DelayedCall(num / base.gc.intimate_shadowDelayDenominator, () =>
		{
			shadow.shaderDataSource = shadow;
			shadow.shaderData.blendFirst = BlendMode.SrcAlpha;
			shadow.shaderData.blendSecond = BlendMode.DstAlpha;
			shadow.shaderData.opacity = 1.49f;
			shadow.tiledSprite.DOColor(Color.white, 2f);
		});
	}

	public override void preactions()
	{
		switch (base.conductor.barNumber)
		{
		case 1:
			if (RDClass.debugSettings.Debug)
			{
				scrExecuteOnUpdate.Add(() =>
				{
					if (Input.GetKeyDown(KeyCode.Alpha1))
					{
						base.game.StartCoroutine(PlayFinalAnimation());
					}
				});
			}
			base.conductor.SetBPM(90f);
			PlayMusic(songsUsed[0]);
			RDClass.Vfx.BgColor(RDPresetColors.Black);
			rainPro = base.tkCam.GetOrAddComponent<CameraFilterPack_Atmosphere_Rain_Pro>();
			rainPro.enabled = false;
			rainPro.Fade = 0.74f;
			rainPro.Intensity = 0.37f;
			rainPro.DirectionX = -0.207f;
			rainPro.Size = 1.66f;
			rainPro.Speed = 0.323f;
			rainPro.Distortion = 0.105f;
			base.room0.rowContainer.LocalMoveXY(25f, -37f);
			if (GC.twoPlayerMode)
			{
				base.game.rows[1].ent.SetRowMarkerCorner(rightCorner: false);
			}
			RDClass.Vfx.SetSpriteMode(SpriteMode.Glow, Color.black.WithAlpha(0.3f));
			if (!GC.twoPlayerMode)
			{
				SetHandToPaige(4, rightHand: false);
			}
			couple.visible = true;
			shadow.visible = true;
			couple.transform.TranslateY(-300f);
			shadow.transform.TranslateY(-300f);
			break;
		case 3:
			base.game.rows[1].ent.character.PlayExpression("neutral");
			base.game.gameState = GameState.Handmode;
			base.game.RepositionRowsAndStrips();
			break;
		case 15:
			base.game.ChangeRowPlayers(singleplayer: false, new RDPlayer[1]);
			base.conductor.crotchetsPerBar = 8;
			break;
		case 16:
			base.rows[2].cpuCharacter = Character.Paige;
			base.game.ChangeRowPlayers(singleplayer: true, new RDPlayer[2]
			{
				RDPlayer.P1,
				RDPlayer.CPU
			}, 7f);
			base.conductor.crotchetsPerBar = 8;
			break;
		case 27:
			RDClass.Vfx.BgColor(Color.black);
			base.game.DeleteRow(1);
			base.game.DeleteRow(2);
			base.conductor.crotchetsPerBar = 4;
			break;
		}
		int num = base.conductor.barNumber;
		if (num < 27)
		{
			return;
		}
		if (GetRankFromMistakes().passed)
		{
			if (num == 27)
			{
				base.game.StartCoroutine(PlayFinalAnimation());
			}
			if (num == 30)
			{
				base.rankscreen.AdvanceGameover();
			}
			if (num == 31)
			{
				base.rankscreen.AdvanceGameover();
			}
			if (num == 32)
			{
				base.rankscreen.AdvanceGameover();
			}
		}
		else
		{
			if (num == 27)
			{
				base.rankscreen.AdvanceGameover();
			}
			if (num == 38)
			{
				base.rankscreen.AdvanceGameover();
			}
			if (num == 39)
			{
				base.rankscreen.AdvanceGameover();
			}
		}
	}

	public void TransitionToSkylineBackground()
	{
		scrExecuteOnCertainBeat.Add(() =>
		{
			RDClass.Vfx.ReverseFlash(base.conductor.crotchet * 2f, 0);
		}, 4f);
		scrExecuteOnCertainBeat.Add(() =>
		{
			RDClass.Vfx.Flash(0, 2f);
			rainPro.enabled = true;
			base.room0.rowContainer.LocalMoveXY(0f, 0f);
			boyWard.gameObject.SetActive(value: false);
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

	public override void actions()
	{
		switch (base.conductor.barNumber)
		{
		case 1:
			base.game.gameState = GameState.Cutscene;
			base.game.rows[1].ent.character.PlayExpression("barely");
			base.game.handControllerOnTop.SetHandsInFront(inFront: false);
			scrExecuteOnCertainBeat.Add(() =>
			{
				base.ink.Run("Intro", () =>
				{
					base.game.gameState = GameState.Handmode;
				});
			}, 1f);
			break;
		case 14:
			boyWard.GirlRun();
			scrExecuteOnCertainBeat.Add(() =>
			{
				base.game.gameState = GameState.HandmodePreCutscene;
				base.game.rows[1].ent.character.PlayExpression("surprised");
				RDClass.Vfx.ShoutEffect();
				string knotName = (GC.twoPlayerMode ? "Interruption1_TwoPlayer" : "Interruption1");
				base.ink.Run(knotName, () =>
				{
					base.game.gameState = GameState.Handmode;
					base.game.handControllerOnTop.UpdateHandsPositions(HandUpdateType.BothHands);
				});
			}, 6f);
			break;
		case 15:
			TransitionToSkylineBackground();
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
}
