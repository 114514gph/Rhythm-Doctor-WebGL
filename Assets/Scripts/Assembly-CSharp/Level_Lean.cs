using DG.Tweening;
using UnityEngine;

public class Level_Lean : LevelBase
{
	private Background[] arrMatrix;

	public override void Init()
	{
		songsUsed = new string[1] { "sndLeanOn" };
	}

	public override void preactions()
	{
		switch (base.conductor.barNumber)
		{
		case 1:
		{
			RDClass.Vfx.SetSpriteMode(SpriteMode.Glow, Color.white);
			LEDSign.status = "Major Lazer & DJ Snake- Lean On ft MO";
			base.conductor.SetBPM(98f);
			PlayMusic(songsUsed[0]);
			base.conductor.crotchetsPerBar = 4;
			RDClass.Vfx.BgColor(RDPresetColors.Black.ARGBToColor());
			base.room0.SetVignette(enabled: true);
			arrMatrix = new Background[3];
			Background.Create("TutorialNote", 2f, 2f);
			arrMatrix[0] = Background.Create("Matrix/0", 0f, 1f);
			arrMatrix[1] = Background.Create("Matrix/1", 0f, 2f);
			arrMatrix[2] = Background.Create("Matrix/2", 0f, 3f);
			for (int num = 0; num < 3; num++)
			{
				arrMatrix[num].SetBackgroundOrder(4 - num);
			}
			base.game.rows[1].rowSound = "sndStick";
			break;
		}
		case 2:
			base.game.MakeRow(RowType.Classic, 1, 1f);
			base.rows[1].SetBeatSkipsLegacy("-x-x-x");
			break;
		case 3:
			LEDSign.status = "Press space on the clap!";
			break;
		case 5:
			base.game.OnBeatClassic(1, 1, 0f, 0.375f);
			break;
		case 13:
			scrExecuteOnEveryHB.Add(() =>
			{
				base.tkCam.transform.DOShakePosition(0.3f, 2f);
			});
			base.game.OffBeat(1);
			base.conductor.crotchetsPerBar = 8;
			base.rows[1].SetBeatSkipsLegacy("------");
			base.game.rows[1].rowSound = "sndKick";
			base.game.OnBeatClassic(2, 1, 0f);
			break;
		case 15:
			base.rows[1].SetBeatSkipsLegacy("-xx-xx");
			base.conductor.crotchetsPerBar = 4;
			base.game.rows[1].rowSound = "sndStick";
			base.game.OffBeat(2);
			base.game.OnBeatClassic(3, 1, 0f, 0.25f);
			base.game.OnBeatClassic(3, 1, 2f, 0.25f);
			break;
		case 18:
			base.conductor.crotchetsPerBar = 4;
			base.game.OffBeat(3);
			base.rows[1].SetBeatSkipsLegacy("------");
			base.game.OnBeatClassic(4, 1, 0f, 0.5f);
			break;
		case 19:
			base.game.rows[1].rowSound = "sndKick";
			base.game.OffBeat(4);
			base.game.OnBeatClassic(5, 1, 0f, 0.5f, 1, SwingBeatType.SwingSimpleThird);
			break;
		case 23:
			base.conductor.crotchetsPerBar = 4;
			base.game.OffBeat(5);
			base.game.rows[1].rowSound = "sndStick";
			base.rows[1].SetBeatSkipsLegacy("-xx-xx");
			base.game.beatAlarm[3] = true;
			break;
		case 26:
			base.conductor.crotchetsPerBar = 4;
			base.game.OffBeat(3);
			base.rows[1].SetBeatSkipsLegacy("------");
			base.game.OnBeat(4);
			break;
		case 27:
			base.game.OffBeat(4);
			break;
		}
	}

	public override void actions()
	{
		switch (base.conductor.barNumber)
		{
		case 27:
			base.rankscreen.AdvanceGameover();
			base.tkCam.transform.DOShakePosition(0.3f, 2f);
			break;
		case 28:
			base.rankscreen.AdvanceGameover();
			break;
		case 29:
			base.rankscreen.AdvanceGameover();
			break;
		}
	}

	public void reduceshadow()
	{
	}
}
