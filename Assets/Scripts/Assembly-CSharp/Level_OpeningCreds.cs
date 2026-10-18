using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using RDLevelEditor;
using UnityEngine;

public class Level_OpeningCreds : LevelBase
{
	private Background noise;

	private RDCreditsNames credits;

	private const int BLNormal = 1;

	private const int BLHalftime = 2;

	private const int BarVerse1 = 5;

	private const int BarHalftime = 13;

	private const int BarVerse2 = 25;

	private const int BarVerse2Infinite = 33;

	private const int BarGameOver = 41;

	private const int BLVerse1 = 1;

	private const int BLVerse2 = 2;

	private const int BLBridge = 8;

	private const int BLChorus1 = 5;

	private const int BarChorus = 21;

	private const int BarPrechorus1 = 13;

	private const int BarChorusSnare = 25;

	private const int BarBridge = 29;

	private const int BarPrechorus2 = 37;

	private const int BarChorus2 = 45;

	private const int BarChorus2Snare = 49;

	private scrVolumeTracker volTracker1;

	public Level_OpeningCreds(RDLevelData data)
		: base(data)
	{
	}

	public override void Init()
	{
		songsUsed = new string[1] { "sndOpeningCredits" };
		rankLowerBounds = new float[6] { 100f, 99f, 98f, 97f, 96f, 95f };
		multiroom = true;
		fullResolutionRenderTextures = true;
		heartExplodeType = HeartExplodeType.GatherAndCeil;
		base.isOneRowButNotBoss = true;
		levelType = LevelType.Intro;
	}

	public override IEnumerator LoadBigAssets()
	{
		base.LoadBigAssets();
		base.room0.PreloadFX();
		noise = Background.Create("Noise", new int[4] { 0, 1, 2, 3 }, 60f, 0f, 0f, isForeground: true);
		noise.visible = false;
		RDClass.Vfx.spriteGlowColor = Color.white.WithAlpha(1f);
		RDClass.Vfx.spriteGlowPower = 0.75f;
		base.room0.LoadLightStrip();
		PreloadTheme(RDTheme.Rooftop);
		PreloadTheme(RDTheme.InsomniacNight);
		PreloadTheme(RDTheme.Intimate);
		PreloadTheme(RDTheme.NeonMuseum);
		PreloadTheme(RDTheme.Matrix);
		PreloadTheme(RDTheme.OrientalTechno);
		PreloadTheme(RDTheme.InsomniacDay);
		PreloadTheme(RDTheme.PoliticiansRally);
		List<RDThemeFX> preloadedVFX = new List<RDThemeFX> { RDThemeFX.ShakeOnHeartBeat };
		base.room0.preloadedVFX = preloadedVFX;
		base.room0.PreloadFX();
		credits = LoadLevelAsset<RDCreditsNames>();
		yield return null;
	}

	public override void preactions()
	{
		switch (base.conductor.barNumber)
		{
		case 5:
			RDGameSounds.Set(GameSoundType.ClapSoundP1Classic, "sndReverbClap");
			base.game.OnBeatClassic(1, 1, 0f, 0.5f);
			break;
		case 13:
			base.game.OffBeat(1);
			base.game.AddBeat(1, 0f, 1f);
			break;
		case 15:
		case 17:
		case 19:
			base.game.AddBeat(1, 0f, 1f);
			break;
		case 21:
		case 22:
		case 23:
			base.game.AddBeat(1, 0f, 0.5f);
			break;
		case 24:
			base.game.AddBeat(1, 0f, 0.25f);
			base.game.AddBeat(1, 2f, 0.25f);
			break;
		case 25:
			base.game.OnBeat(1);
			break;
		case 41:
			base.game.OffBeat(1);
			break;
		}
		if (base.conductor.barNumber == 1)
		{
			RDClass.Vfx.GetRoomOverlay(0).SetColor(Color.black);
			volTracker1 = scrVolumeTracker.Add((float s) =>
			{
				RDClass.Vfx.GetRoomOverlay(0).alpha = 1f - s;
			}, 0.5f, 1f);
			scrVolumeTracker.Add((float s) =>
			{
				RDClass.Vfx.SetVignetteAlpha(1f - s);
			}, 0f, 1f);
			base.room0.showTextAboveBeatboxes = true;
			noise.visible = true;
			base.room0.SetVignette(enabled: true);
			base.game.textMaskTexture.texture = base.room0.camera.targetTexture;
			base.game.textMaskTexture.texture = base.tkCam.targetTexture;
			base.room0.kaleidoscopeModeEmbers = true;
			base.conductor.SetBPM(94f);
			PlayMusic(songsUsed[0]);
			base.conductor.crotchetsPerBar = 4;
			base.game.MakeRow(RowType.Classic, 1, 0.5f);
			base.conductor.visualBeatLengthMultiplier = 1f;
			base.rows[1].rowSound = "sndShaker";
			scrVolumeTracker.Add((float s) =>
			{
				RDClass.Vfx.scrollYMultiplier = s;
			}, 0.1f, 6f);
			scrVolumeTracker.Add((float s) =>
			{
				noise.SetAlpha(s * base.gc.noiseMultiplier);
			}, 0f, 5f);
		}
	}

	public override void actions()
	{
		switch (base.conductor.barNumber)
		{
		case 1:
			scrExecuteOnCertainBeat.Add(() =>
			{
				RDClass.Vfx.ShowMaskedText("7TH       ");
			}, 0f);
			scrExecuteOnCertainBeat.Add(() =>
			{
				RDClass.Vfx.ShowMaskedText("7TH       \n");
			}, 0.25f);
			scrExecuteOnCertainBeat.Add(() =>
			{
				RDClass.Vfx.ShowMaskedText("7TH BEAT\n");
			}, 1f);
			scrExecuteOnCertainBeat.Add(() =>
			{
				RDClass.Vfx.ShowMaskedText("7TH BEAT\nGAMES");
			}, 1.75f);
			scrExecuteOnCertainBeat.Add(() =>
			{
				RDClass.Vfx.ShowMaskedText("PRESENT", 15);
			}, 2.5f);
			break;
		case 2:
			scrExecuteOnCertainBeat.Add(() =>
			{
				RDClass.Vfx.ShowMaskedText("RHYTHM DOCTOR", 70, 1000f, RDoc: true);
			}, 0f);
			break;
		case 3:
			scrExecuteOnCertainBeat.Add(() =>
			{
				RDClass.Vfx.ShowMaskedText("");
			}, 0f);
			break;
		case 4:
		{
			scrExecuteOnCertainBeat.Add(() =>
			{
				RDClass.Vfx.ShowMaskedText("GET\n", 32);
			}, 0f);
			scrExecuteOnCertainBeat.Add(() =>
			{
				RDClass.Vfx.ShowMaskedText("GET\n", 32);
			}, 0.5f);
			scrExecuteOnCertainBeat.Add(() =>
			{
				RDClass.Vfx.ShowMaskedText("GET\nREADY", 32);
			}, 1.5f);
			scrExecuteOnCertainBeat.Add(() =>
			{
				RDClass.Vfx.ShowMaskedText("GET\nREADY", 32);
			}, 2f);
			scrExecuteOnCertainBeat.Add(() =>
			{
				RDClass.Vfx.ShowMaskedText("");
			}, 4f);
			int inc = 0;
			int basesize = 32;
			scrExecuteOnCertainBeat.Add(() =>
			{
				RDClass.Vfx.ShowMaskedText("GET\nREADY", basesize);
			}, 3f);
			scrExecuteOnCertainBeat.Add(() =>
			{
				RDClass.Vfx.ShowMaskedText("[blank]", basesize);
			}, 3.05f);
			scrExecuteOnCertainBeat.Add(() =>
			{
				RDClass.Vfx.ShowMaskedText("GET\nREADY", basesize + inc);
			}, 3.25f);
			scrExecuteOnCertainBeat.Add(() =>
			{
				RDClass.Vfx.ShowMaskedText("[blank]", basesize + inc);
			}, 3.3f);
			scrExecuteOnCertainBeat.Add(() =>
			{
				RDClass.Vfx.ShowMaskedText("GET\nREADY", basesize + 2 * inc);
			}, 3.5f);
			scrExecuteOnCertainBeat.Add(() =>
			{
				RDClass.Vfx.ShowMaskedText("[blank]", basesize + 2 * inc);
			}, 3.55f);
			scrExecuteOnCertainBeat.Add(() =>
			{
				RDClass.Vfx.ShowMaskedText("GET\nREADY", basesize + 3 * inc);
			}, 3.75f);
			scrExecuteOnCertainBeat.Add(() =>
			{
				RDClass.Vfx.ShowMaskedText("[blank]", basesize + 3 * inc);
			}, 3.8f);
			break;
		}
		case 5:
			RDClass.Vfx.Flash();
			ShowThemeBackgrounds(RDTheme.CrossesStraight);
			noise.visible = false;
			volTracker1.Delete(1f);
			SetUpFlashMarginFeedback();
			break;
		case 9:
			ShowThemeBackgrounds(RDTheme.CubesFallingNiceBlue);
			RDClass.Vfx.SetSpriteMode(SpriteMode.Silhouette, Color.black);
			break;
		case 13:
			AddThemeFX(RDThemeFX.BassDropOnHit);
			AddThemeFX(RDThemeFX.ShakeOnHeartBeat);
			RDClass.Vfx.Flash();
			MarginFlashFeedback(active: false, keepCompleteMiss: true);
			ShowThemeBackgrounds(RDTheme.Matrix);
			RDClass.Vfx.SetSpriteMode(SpriteMode.Silhouette, Color.white);
			RDClass.Vfx.spriteGlowColor = Color.white.WithAlpha(1f);
			RDClass.Vfx.spriteGlowPower = 0.75f;
			break;
		case 15:
			base.rowEnt1.ChangeCharacter(Character.Barista);
			break;
		case 17:
		{
			RDClass.Vfx.Flash();
			RDClass.Vfx.SetSpriteMode(SpriteMode.None, Color.white);
			base.rowEnt1.ChangeCharacter(Character.Paige);
			ShowThemeBackgrounds(RDTheme.Rooftop);
			Vector3 endValue = new Vector3(0f, base.gc.opening_rooftopYDistance, 0f);
			Transform rowContainer = base.room0.rowContainer;
			Vector3 endValue2 = new Vector3(rowContainer.localPosition.x, rowContainer.localPosition.y + base.gc.opening_rooftopYDistance, 0f);
			base.room0.backgroundContainerCamera.DOLocalMove(endValue, base.gc.opening_rooftopYDuration).SetEase(base.gc.opening_rooftopYEase);
			rowContainer.DOLocalMove(endValue2, base.gc.opening_rooftopYDuration).SetEase(base.gc.opening_rooftopYEase);
			credits.ShowThis(credits.winston);
			base.room0.emberBloom.Tint = 3660054783u.RGBAToColor();
			break;
		}
		case 19:
			base.room0.backgroundContainerCamera.localPosition = Vector3.zero;
			ShowThemeBackgrounds(RDTheme.InsomniacNight);
			credits.ShowThis(credits.hafiz);
			base.rowEnt1.ChangeCharacter(Character.SamuraiBoss);
			break;
		case 21:
			ShowThemeBackgrounds(RDTheme.OrientalTechno);
			credits.ShowThis(credits.giacomo);
			base.rowEnt1.ChangeCharacter(Character.Samurai);
			break;
		case 22:
			ShowThemeBackgrounds(RDTheme.PoliticiansRally);
			credits.ShowThis(credits.ashley);
			base.rowEnt1.ChangeCharacter(Character.Politician);
			break;
		case 23:
			ShowThemeBackgrounds(RDTheme.Intimate);
			credits.HideAll();
			base.rowEnt1.ChangeCharacter(Character.Boy);
			break;
		case 24:
			ShowThemeBackgrounds(RDTheme.NeonMuseum);
			credits.ShowThis(credits.alberto);
			base.rowEnt1.ChangeCharacter(Character.HoodieBoy);
			scrExecuteOnCertainBeat.Add(() =>
			{
				ShowThemeBackgrounds(RDTheme.InsomniacDay);
				credits.HideAll();
				base.rowEnt1.ChangeCharacter(Character.SamuraiBoss);
				base.room0.beatboxVerticalStrip = true;
			}, 2f);
			scrExecuteOnCertainBeat.Add(() =>
			{
				base.room0.beatboxVerticalStrip = false;
			}, 3.5f);
			break;
		case 25:
			RDClass.Vfx.Flash();
			ShowThemeBackgrounds(RDTheme.CrossesFalling);
			base.rowEnt1.ChangeCharacter(Character.Samurai);
			break;
		case 33:
			RDClass.Vfx.Flash();
			RDClass.Vfx.infiniteZoom = true;
			RDClass.Vfx.sevenBeatArmy.beatSynced = true;
			RDClass.Vfx.sevenBeatArmy.borderOffset = -0.33f;
			RDClass.Vfx.sevenBeatArmy.factor = 5f;
			RDClass.Vfx.sevenBeatArmy.curve = base.gc.curveInfiniteZoomPulse;
			RDClass.Vfx.sevenBeatArmy.speed = 1f;
			break;
		case 37:
			RDClass.Vfx.Flash();
			RDClass.Vfx.sevenBeatArmy.beatSynced = false;
			RDClass.Vfx.sevenBeatArmy.borderOffset = -0.33f;
			RDClass.Vfx.sevenBeatArmy.factor = 5f;
			RDClass.Vfx.sevenBeatArmy.curve = base.gc.curveLinear;
			RDClass.Vfx.sevenBeatArmy.speed = 2f;
			base.game.SetVisualSpeed(2f);
			break;
		case 39:
			base.game.SetVisualSpeed(2f);
			RDClass.Vfx.sevenBeatArmy.speed = 3f;
			break;
		case 40:
			base.game.SetVisualSpeed(3f);
			RDClass.Vfx.sevenBeatArmy.speed = 3f;
			break;
		case 41:
			RDClass.Vfx.Flash();
			RDClass.Vfx.infiniteZoom = false;
			base.game.DeleteRow(1);
			base.game.SetVisualSpeed(0.5f);
			break;
		case 42:
			FinishOpeningCreds();
			break;
		case 6:
		case 7:
		case 8:
		case 10:
		case 11:
		case 12:
		case 14:
		case 16:
		case 18:
		case 20:
		case 26:
		case 27:
		case 28:
		case 29:
		case 30:
		case 31:
		case 32:
		case 34:
		case 35:
		case 36:
		case 38:
			break;
		}
	}

	public void FinishOpeningCreds()
	{
		RDClass.Vfx.FadeOut();
		DOVirtual.DelayedCall(1f, () =>
		{
			base.game.mistakesManager.SaveMistakesData();
			scnBase.GoToLevelSelect();
		});
	}
}
