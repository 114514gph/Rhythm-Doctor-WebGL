using System.Collections;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class Level_Boss2Hard : LevelBase
{
	private Background mClubSignOn;

	private Background mCafeSignOn;

	public override void Init()
	{
		rankLowerBounds = new float[6] { 100f, 50f, 35f, 25f, 8f, 0f };
		songsUsed = new string[1] { "sndBoss2" };
	}

	public override IEnumerator LoadBigAssets()
	{
		Background.Create("Boss2_Background");
		Background.Create("Boss2_Windows_and_Reflection", new int[3] { 0, 1, 2 }, 0.25f);
		Background.Create("Boss2_Cloud_1", 0.1f);
		Background.Create("Boss2_Cloud_2", 0.5f);
		Background.Create("Boss2_Cloud_3", 0.2f);
		Background.Create("Boss2_Cloud_4", 0.3f);
		Background.Create("Boss2_Foreground");
		Background.Create("Boss2_Club_Sign/1");
		Background.Create("Boss2_Cafe_Sign/1");
		Background.Create("Boss2_Lamppost_Glow", new int[26]
		{
			0, 0, 0, 0, 0, 0, 0, 0, 1, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
			0, 1, 0, 0, 0, 0
		}, 9f);
		mClubSignOn = Background.Create("Boss2_Club_Sign/0");
		mCafeSignOn = Background.Create("Boss2_Cafe_Sign/0");
		int num = 0;
		foreach (Background background in RDClass.Vfx.backgrounds)
		{
			background.SetBackgroundOrder(num);
			num++;
		}
		mClubSignOn.SetAlpha(0f);
		mCafeSignOn.SetAlpha(0f);
		yield return null;
	}

	private void SetBgStyle(int ID)
	{
		VideoBloom component = base.tkCam.GetComponent<VideoBloom>();
		ColorCorrectionCurves component2 = base.tkCam.GetComponent<ColorCorrectionCurves>();
		switch (ID)
		{
		case 1:
			component.enabled = true;
			component.Tint = 4284618239u.RGBAToColor();
			component.Threshold = 0.4f;
			component.KernelSize = 59f;
			component.MasterAmount = 2.18f;
			base.room0.SetVignette(enabled: true);
			break;
		case 2:
			component2.enabled = true;
			RDClass.Vfx.spriteGlowColor = Color.white.WithAlpha(1f);
			RDClass.Vfx.spriteGlowPower = 0.75f;
			component.enabled = true;
			component.Tint = 4002800127u.RGBAToColor();
			component.Threshold = 0.24f;
			component.KernelSize = 42.9f;
			component.MasterAmount = 0.94f;
			component.MediumKernelScale = 1.9f;
			component.LargeKernelScale = 3.8f;
			base.room0.SetVignette(enabled: true);
			break;
		case 3:
			break;
		}
	}

	public override void preactions()
	{
		switch (base.conductor.barNumber)
		{
		case 1:
			base.conductor.SetBPM(155f);
			base.conductor.crotchetsPerBar = 4;
			base.conductor.PlayNoSong();
			base.game.MakeRow(RowType.Oneshot, 1, 0f, Character.Farmer);
			base.rowEnt1.oneshotRowController.waveManager.waveType = WaveType.BoomAndRush;
			base.rowEnt1.y = 52f;
			base.game.OnBeatOneshotAndBeep(1, 1, 4f, 1f, 0f, sayReady: true);
			SetBgStyle(2);
			scrVolumeTracker.Add((float s) =>
			{
				RDClass.Vfx.SetVignetteAlpha(1f - s);
			}, 0f, 1f);
			scrExecuteOnHit.Add(() =>
			{
				RDClass.Vfx.Flash(-1, 0.25f, 0.5f);
			}, HitType.Hit, persistent: true, autoDelete: false, -1, overwrite: false);
			break;
		case 2:
			PlayMusic(songsUsed[0]);
			break;
		case 5:
			base.game.ChangeLoopOneshotSimple(1, 2f, 0.5f, sayReady: true, sayGo: true);
			break;
		case 9:
			base.game.ChangeLoopOneshotSimple(1, 0f, 2f, sayReady: false, sayGo: true);
			Stop(0.02f);
			break;
		case 18:
			Stop(0f);
			break;
		case 24:
			base.game.ChangeLoopOneshotSimple(1, 0f, 4f, sayReady: false, sayGo: true);
			break;
		case 33:
			base.game.ChangeLoopOneshotSimple(1, 0f, 2f, sayReady: false, sayGo: true);
			break;
		case 42:
			Stop(0f);
			break;
		case 48:
			base.game.ChangeLoopOneshotSimple(1, 0f, 2f, sayReady: true, sayGo: false);
			break;
		case 51:
			base.game.ChangeLoopOneshotSimple(1, 0f, 1f, sayReady: true, sayGo: true);
			break;
		case 53:
			base.game.ChangeLoopOneshotSimple(1, 0f, 2f, sayReady: false, sayGo: true);
			break;
		case 56:
			base.game.ChangeLoopOneshotSimple(1, 2f, 0.5f, sayReady: true, sayGo: false);
			break;
		case 57:
			base.game.ChangeLoopOneshotSimple(1, 0f, 2f, sayReady: false, sayGo: true);
			break;
		case 60:
			base.game.ChangeLoopOneshotSimple(1, 1.33f, 2f / 3f, sayReady: true, sayGo: false);
			break;
		case 61:
			base.game.ChangeLoopOneshotSimple(1, 0f, 2f, sayReady: false, sayGo: false);
			break;
		case 62:
			base.game.ChangeLoopOneshotSimple(1, 0f, 4f, sayReady: false, sayGo: true);
			break;
		case 65:
			base.game.ChangeLoopOneshotSimple(1, 2f, 0.5f, sayReady: true, sayGo: true);
			break;
		case 66:
			base.game.ChangeLoopOneshotSimple(1, 2f, 1f, sayReady: false, sayGo: true);
			break;
		case 67:
			base.game.ChangeLoopOneshotSimple(1, 1.33f, 2f / 3f, sayReady: true, sayGo: true);
			break;
		case 69:
			base.game.ChangeLoopOneshotSimple(1, 0f, 1f, sayReady: true, sayGo: true);
			break;
		case 71:
			base.game.ChangeLoopOneshotSimple(1, 0f, 2f, sayReady: false, sayGo: true);
			break;
		case 73:
			base.game.ChangeLoopOneshotSimple(1, 2f, 0.5f, sayReady: true, sayGo: true);
			break;
		case 74:
			base.game.ChangeLoopOneshotSimple(1, 2f, 1f, sayReady: false, sayGo: true);
			break;
		case 75:
			base.game.ChangeLoopOneshotSimple(1, 1.33f, 2f / 3f, sayReady: true, sayGo: true);
			break;
		case 77:
			base.game.ChangeLoopOneshotSimple(1, 0f, 1f, sayReady: true, sayGo: true);
			break;
		case 79:
			base.game.ChangeLoopOneshotSimple(1, 0f, 2f, sayReady: false, sayGo: true);
			break;
		case 81:
			base.game.ChangeLoopOneshotSimple(1, 2f, 0.5f, sayReady: true, sayGo: true);
			break;
		case 82:
			base.game.ChangeLoopOneshotSimple(1, 2f, 1f, sayReady: false, sayGo: true);
			break;
		case 83:
			base.game.ChangeLoopOneshotSimple(1, 2f, 0.5f, sayReady: true, sayGo: true);
			break;
		case 84:
			base.game.ChangeLoopOneshotSimple(1, 2f, 1f, sayReady: false, sayGo: true);
			break;
		case 85:
			base.game.ChangeLoopOneshotSimple(1, 2f, 0.5f, sayReady: true, sayGo: true);
			break;
		case 86:
			base.game.ChangeLoopOneshotSimple(1, 2f, 1f, sayReady: false, sayGo: true);
			break;
		case 87:
			base.game.ChangeLoopOneshotSimple(1, 2f, 0.5f, sayReady: true, sayGo: false);
			break;
		case 88:
			base.game.ChangeLoopOneshotSimple(1, 0f, 2f, sayReady: false, sayGo: true);
			break;
		case 89:
			base.game.ChangeLoopOneshotSimple(1, 2f, 0.5f, sayReady: true, sayGo: true);
			break;
		case 90:
			base.game.ChangeLoopOneshotSimple(1, 2f, 1f, sayReady: false, sayGo: true);
			break;
		case 91:
			base.game.ChangeLoopOneshotSimple(1, 2.08f, 0.5f, sayReady: true, sayGo: true);
			break;
		case 92:
			base.game.ChangeLoopOneshotSimple(1, 2.08f, 1f, sayReady: false, sayGo: true);
			break;
		case 93:
			base.game.ChangeLoopOneshotSimple(1, 2f, 0.5f, sayReady: true, sayGo: true);
			break;
		case 96:
			base.game.ChangeLoopOneshotSimple(1, 0f, 4f, sayReady: false, sayGo: true);
			break;
		case 101:
			Stop(0f);
			break;
		}
	}

	public override void BeepGet(float beepPeriod, bool fromTop = false, int[] rooms = null)
	{
		mClubSignOn.SetAlpha(1f);
		mClubSignOn.shaderDataSource = mClubSignOn;
		mClubSignOn.shaderData.additiveColor = new Color(0f, 0.2f, 0.1f, 0.5f);
	}

	public override void BeepSet(float beepPeriod, bool fromTop = false, int[] rooms = null)
	{
		mCafeSignOn.SetAlpha(1f);
		mClubSignOn.shaderData.additiveColor = new Color(0.2f, 0.1f, 0f, 0.5f);
	}

	public override void BeepGo(float beepPeriod, bool fromTop = false, int[] rooms = null)
	{
		mClubSignOn.SetAlpha(0f);
		mCafeSignOn.SetAlpha(0f);
		mClubSignOn.shaderData.additiveColor = Color.clear;
	}

	public override void actions()
	{
	}

	private void Stop(float beatnum)
	{
		base.game.KillOneshot(1, beatnum);
	}
}
