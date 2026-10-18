using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Level_InsomniacHard : LevelBase
{
	private const int Phase1HP = 21;

	private const int Phase1HPLeftover = 4;

	private const int Phase2HP = 25;

	private const int Phase2HPLeftover = 4;

	private const int Phase3HP = 19;

	private const string DialogueFile = "diaInsomniacHard";

	private const float MinHealthPctToBeep = 0.3f;

	private const string LowHPBeepSound1 = "sndJyi - ChineseCount1";

	private const string LowHPBeepSound2 = "sndJyi - ChineseCount2";

	public static bool hasGlitched;

	public static int numberOfTries;

	private List<scrExecuteOnPeriodicBeat> mPagodaPulses = new List<scrExecuteOnPeriodicBeat>();

	private List<Background> mBackgrounds = new List<Background>();

	private Background[] arrMatrix;

	private bool hasGlitchedBefore;

	private Background mDayBackground;

	private Background mNightBackground;

	private Background mPagoda;

	private Background mPagodaLights;

	private Background mStars;

	private Background mPetals;

	private Background mCat;

	private Background mBat;

	private scrStutter stutter;

	private RDBossStageText mBossStageText;

	private scrChar mBossCharacter;

	private scrChar mBossCharacter2;

	private HPBarController hpController;

	private CameraFilterPack_TV_VHS vhs;

	private CameraFilterPack_TV_VHS_Rewind vhsRewind;

	private FakePopup fakePopup;

	private Background mGreenTrees;

	private Background mSkyBackground;

	private scrVolumeTracker matrixVolTracker;

	private ParticleSystem petals1;

	private ParticleSystem petals2;

	private ParticleSystem petals3;

	private float heartCenter;

	public int LowHealthBeepCounter;

	private bool LowHealthBeepCustomPlayed;

	private bool narratedLowHealth;

	public override void Init()
	{
		multiroom = true;
		songsUsed = new string[1] { "sndOrientalInsomniacHardRep" };
		mBossStageText = LoadLevelAsset<RDBossStageText>();
		mBossStageText.lowerText.color = Color.black;
		mBossStageText.upperText.color = Color.black;
		mBossStageText.glitchMode = true;
		hasGlitchedBefore = hasGlitched;
		mBossStageText.transform.localPosition = new Vector3(RDClass.Vfx.RDWidth * 0.5f, RDClass.Vfx.RDHeight * 0.5f, 20f);
		heartExplodeType = HeartExplodeType.Instant;
		hpController = base.game.hpController;
		hpController.gameObject.SetActive(value: false);
		int num = 25;
		if (GC.twoPlayerMode)
		{
			num *= 2;
		}
		hpController.bossHP.SetMaxHP(num, compensateMisses: false);
		levelType = LevelType.Boss;
		missesToCrackHeart = 11f;
		skippableRankScreen = false;
		base.ink.Load("diaInsomniacHard");
		base.game.barToSkipToOnSlash = 174;
	}

	public override IEnumerator LoadBigAssets()
	{
		bossActNum = 4;
		stutter = base.room0.stutter;
		mSkyBackground = Background.CreateSingle("SkySS");
		mGreenTrees = Background.CreateSingle("TreesSS").SetFPS(1.5f);
		mGreenTrees.visible = false;
		mDayBackground = Background.CreateSingle("SkySS");
		float fps = 1.5f;
		mNightBackground = Background.CreateSingle("InsomniacHard_Background");
		mNightBackground.visible = false;
		mStars = Background.CreateSingle("InsomniacHard_Stars", new int[5] { 0, 1, 2, 3, 4 }, fps);
		mStars.visible = false;
		mPagoda = Background.CreateSingle("InsomniacHard_PagodaUnlit");
		mPagoda.visible = false;
		mPagodaLights = Background.CreateSingle("InsomniacHard_PagodaGlowAndLights");
		mPagodaLights.visible = false;
		mBat = Background.CreateSingle("InsomniacHard_Bat", new int[5] { 0, 1, 2, 3, 4 }, 8f);
		mBat.visible = false;
		mCat = Background.CreateSingle("InsomniacHard_Cat", new int[8] { 0, 1, 2, 3, 4, 3, 2, 1 }, 6f);
		mCat.visible = false;
		mPetals = Background.CreateSingle("InsomniacHard_FrontPetals");
		mPetals.visible = false;
		mBackgrounds.Add(mNightBackground);
		mBackgrounds.Add(mStars);
		mBackgrounds.Add(mPagoda);
		mBackgrounds.Add(mPagodaLights);
		mBackgrounds.Add(mPetals);
		mBackgrounds.Add(mBat);
		mBackgrounds.Add(mCat);
		fakePopup = FakePopup.Load(RuntimePlatform.IPhonePlayer);
		arrMatrix = new Background[3];
		arrMatrix[0] = Background.Create("Matrix/0", 0f, 1f).SetBackgroundOrder(15).SetScale(0.75f);
		arrMatrix[1] = Background.Create("Matrix/1", 0f, 2f).SetBackgroundOrder(16).SetScale(1.5f);
		arrMatrix[2] = Background.Create("Matrix/2", 0f, 3f).SetBackgroundOrder(17).SetScale(2.25f);
		matrixVolTracker = scrVolumeTracker.Add((float s) =>
		{
			Background[] array = arrMatrix;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetAlpha(s);
			}
		}, 0f, 5f);
		vhs = base.tkCam.GetComponent<CameraFilterPack_TV_VHS>();
		vhsRewind = base.tkCam.GetComponent<CameraFilterPack_TV_VHS_Rewind>();
		base.game.MakeRow(RowType.Classic, 1, 0.5f, Character.SamuraiBoss, RDPlayer.P1, RDPlayer.P1, 2, isShadow: false, 0, animate: false);
		base.game.rows[1].ent.Hide(keepCharacterVisible: false, effects: false);
		if (GC.twoPlayerMode)
		{
			base.game.rows[2].ent.Hide(keepCharacterVisible: false, effects: false);
		}
		yield return null;
	}

	private void SetBgStyle(int ID)
	{
		VideoBloom component = base.tkCam.GetComponent<VideoBloom>();
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
			ShowMatrixes(show: true);
			RDClass.Vfx.spriteGlowColor = Color.white.WithAlpha(1f);
			RDClass.Vfx.spriteGlowPower = 0.75f;
			printe("enabling bloom");
			component.enabled = true;
			component.Tint = 641858559u.RGBAToColor();
			component.Threshold = 0.14f;
			component.KernelSize = 74.9f;
			component.MasterAmount = 3.36f;
			base.room0.SetVignette(enabled: true);
			mGreenTrees.visible = false;
			mSkyBackground.visible = false;
			RDClass.Vfx.fxNoise.visible = false;
			mDayBackground.visible = false;
			mStars.visible = true;
			mBat.visible = true;
			mCat.visible = true;
			mPetals.visible = true;
			mPagoda.visible = true;
			mPagodaLights.visible = true;
			mNightBackground.visible = true;
			break;
		case 3:
			break;
		}
	}

	public void ShowMatrixes(bool show)
	{
		Background[] array = arrMatrix;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].visible = show;
		}
	}

	private void Pthis()
	{
	}

	public override void preactions()
	{
		if (failedLevel)
		{
			return;
		}
		switch (base.conductor.barNumber)
		{
		case 1:
			SetBgStyle(1);
			base.game.shadowRowStyle = ShadowRowStyle.HostToShadowIf2P;
			base.conductor.SetBPM(90f);
			PlayMusic(songsUsed[0]);
			RDClass.Vfx.BgColor(Color.white);
			base.conductor.crotchetsPerBar = 4;
			base.conductor.visualBeatLengthMultiplier = 1f;
			base.game.rows[1].rowSound = "sndShaker";
			if (GC.twoPlayerMode)
			{
				base.game.rows[2].rowSound = "sndShaker";
			}
			base.game.SetPlayerHitSounds(RowType.Classic, "sndReverbClap", "sndReverbClapP2");
			base.rows[1].rowSoundVolMultiplier = 0.25f;
			if (GC.twoPlayerMode)
			{
				base.rows[2].rowSoundVolMultiplier = 0.25f;
				base.rows[2].rowSoundMinPitch = 1.3f;
				base.rows[2].rowSoundMaxPitch = 1.3f;
			}
			Pthis();
			break;
		case 2:
			Pthis();
			hpController.gameObject.SetActive(value: true);
			break;
		case 3:
			Pthis();
			break;
		case 4:
			base.conductor.SetBPM(160f);
			petals1 = RDClass.Vfx.ShowParticlePreset("petals1");
			petals2 = RDClass.Vfx.ShowParticlePreset("petals2");
			petals3 = RDClass.Vfx.ShowParticlePreset("petals3");
			RDClass.Vfx.SetSpriteMode(SpriteMode.Glow, Color.white.WithAlpha(0.6f));
			base.game.rows[1].ent.Show(ShowAnimationType.InstantNoEffects, setup: false);
			mBossCharacter = base.rows[1].ent.character;
			if (GC.twoPlayerMode)
			{
				mBossCharacter2 = base.rows[2].ent.character;
				mBossCharacter.ChangeCharacter(Character.SamuraiBossAlt);
				base.game.rows[2].ent.Show(ShowAnimationType.InstantNoEffects, setup: false);
			}
			petals1.time = 50f;
			petals2.time = 50f;
			petals3.time = 50f;
			OnBeatClassic(1, 1, 0f);
			mBossCharacter.PlayExpression("neutral");
			if (GC.twoPlayerMode)
			{
				mBossCharacter2.PlayExpression("neutral");
			}
			if (Narration.IsEnabled)
			{
				base.conductor.PlayBeatInCrotchets("sndPatientConnect", 0f, 1f, RDUtils.GetMixerGroup("CuesoundsParent"), 1f, base.rows[1].rowSoundPanAdjustedFor2P);
				if (GC.twoPlayerMode)
				{
					base.conductor.PlayBeatInCrotchets("sndPatientConnect", 0.5f, 1f, RDUtils.GetMixerGroup("CuesoundsParent"), 1f, base.rows[2].rowSoundPanAdjustedFor2P);
				}
			}
			break;
		case 20:
			base.game.gameState = GameState.HandmodePreCutscene;
			OffBeat(1);
			break;
		case 24:
			base.game.gameState = GameState.Handmode;
			OnBeat(1);
			break;
		case 25:
			base.rows[1].rowSound = null;
			if (GC.twoPlayerMode)
			{
				base.rows[2].rowSound = null;
			}
			break;
		case 26:
			base.rows[1].rowSound = "sndShaker";
			if (GC.twoPlayerMode)
			{
				base.rows[2].rowSound = "sndShaker";
			}
			base.rows[1].rowSoundVolMultiplier = 0.25f;
			if (GC.twoPlayerMode)
			{
				base.rows[2].rowSoundVolMultiplier = 0.25f;
				base.rows[2].rowSoundMinPitch = 1.3f;
				base.rows[2].rowSoundMaxPitch = 1.3f;
			}
			break;
		case 29:
			OffBeat(1);
			break;
		case 33:
			base.conductor.SetBPM(360f);
			base.conductor.visualBeatLengthMultiplier = 2f;
			base.conductor.crotchetsPerBar = 10;
			break;
		case 34:
			base.conductor.crotchetsPerBar = 7;
			break;
		case 41:
			OnBeatClassic(2, 1, 0f, 1f);
			break;
		case 66:
			OffBeat(2);
			break;
		case 69:
			base.conductor.crotchetsPerBar = 9;
			base.conductor.barNumberToGo = 166;
			break;
		case 166:
			base.conductor.SetBPM(180f);
			base.conductor.crotchetsPerBar = 4;
			base.conductor.visualBeatLengthMultiplier = 1f;
			SetPossessed(2);
			base.game.gameState = GameState.Handmode;
			AddBeat(1, 0f);
			AddBeat(1, 3.5f);
			break;
		case 167:
			AddBeat(1, 3f);
			break;
		case 168:
			AddBeat(1, 2.5f);
			break;
		case 169:
			AddBeat(1, 2f);
			break;
		case 170:
			AddBeat(1, 1.5f);
			break;
		case 171:
			AddBeat(1, 1f);
			break;
		case 172:
			AddBeat(1, 0.5f);
			break;
		case 173:
			AddBeat(1, 0f);
			AddBeat(1, 3.5f);
			break;
		case 174:
			AddBeat(1, 3f);
			break;
		case 175:
			AddBeat(1, 2.5f);
			break;
		case 176:
			AddBeat(1, 2f);
			break;
		case 177:
			AddBeat(1, 1.5f);
			break;
		case 178:
			AddBeat(1, 1f);
			break;
		case 179:
			AddBeat(1, 0.5f);
			break;
		case 180:
			AddBeat(1, 0f);
			AddBeat(1, 3.5f);
			break;
		case 181:
			AddBeat(1, 3f);
			break;
		case 182:
			if (Narration.IsEnabled)
			{
				base.conductor.PlayBeatInCrotchets("sndPatientDisconnect", 2.5f, 1f, RDUtils.GetMixerGroup("CuesoundsParent"), 1f, base.rows[1].rowSoundPanAdjustedFor2P);
				if (GC.twoPlayerMode)
				{
					base.conductor.PlayBeatInCrotchets("sndPatientDisconnect", 3f, 1f, RDUtils.GetMixerGroup("CuesoundsParent"), 1f, base.rows[2].rowSoundPanAdjustedFor2P);
				}
			}
			break;
		}
	}

	public override void actions()
	{
		if (stutter != null)
		{
			stutter.ClearStutterCache();
		}
		if (failedLevel)
		{
			return;
		}
		NumberFlashingOnAction();
		switch (base.conductor.barNumber)
		{
		case 1:
			base.room0.showTextAboveBeatboxes = true;
			base.room0.stutter.enabled = true;
			base.game.gameState = GameState.Cutscene;
			mBackgrounds.Add(mDayBackground);
			if (!hasGlitchedBefore)
			{
				mBossStageText.Play();
				hasGlitched = true;
			}
			else
			{
				scrExecuteOnCertainBeat.Add(() =>
				{
					base.conductor.ScrubToBarNumNoCoroutine(4);
				}, 0.2f + Random.value * 2.5f);
			}
			numberOfTries++;
			break;
		case 2:
		{
			mGreenTrees.FadeIn(4f * base.conductor.crotchet);
			if (!hasGlitchedBefore)
			{
				stutter.AddStutter(0.25f, 0.25f, 0.25f, 13);
			}
			for (int num = 0; num < 13; num++)
			{
				scrExecuteOnCertainBeat.Add(() =>
				{
					Narration.Say(RDString.Get("status.bossStageGlitch"), NarrationCategory.Notification, true);
				}, 0.25f * (float)(num + 1));
			}
			scrExecuteOnCertainBeat.Add(() =>
			{
				RDClass.Vfx.scrollYMultiplier = 0f;
				matrixVolTracker.enabled = false;
			}, 2f);
			break;
		}
		case 3:
			if (!hasGlitchedBefore)
			{
				scrExecuteOnCertainBeat.Add(() =>
				{
					if (base.game.pauseMenu.showing)
					{
						base.game.pauseMenu.Hide();
					}
					scnGame.pauseBlocked = false;
					VideoBloom component = base.tkCam.GetComponent<VideoBloom>();
					printe("disabling bloom");
					component.enabled = false;
					base.game.activityIndicator.gameObject.SetActive(value: false);
					base.game.activityIndicatorPivot.gameObject.SetActive(value: false);
					base.game.frontOfAllOverlay.gameObject.SetActive(value: false);
					mBossStageText.gameObject.SetActive(value: false);
				}, 3.25f);
			}
			else
			{
				RDClass.Vfx.invertScreen = true;
				SetVHSRewindEffect(on: true);
			}
			AddObstruction(1, 3.5f, 0.5f);
			break;
		case 4:
		{
			RDClass.Vfx.invertScreen = false;
			RDClass.Vfx.scrollYMultiplier = 1f;
			matrixVolTracker.enabled = true;
			SetVHSRewindEffect(on: false);
			SetBgStyle(2);
			base.game.gameState = GameState.Handmode;
			base.game.handControllerOnTop.SetHandsInFront(inFront: false);
			scrConductor.instance.Pause(isPause: false);
			if (!base.game.reducedFlash)
			{
				RDClass.Vfx.Flash();
			}
			mBackgrounds.Add(mCat);
			mBackgrounds.Add(mPetals);
			scrVolumeTracker.Add((float x) =>
			{
				if (mPagodaLights.gameObject.activeSelf)
				{
					mPagodaLights.SetAlpha(x);
				}
			}, 0f, 7f);
			mPagoda.tiledSprite.anchor = tk2dBaseSprite.Anchor.LowerRight;
			mPagodaLights.tiledSprite.anchor = tk2dBaseSprite.Anchor.LowerRight;
			Vector3 localPosition = new Vector3(RDClass.Vfx.RDWidth * 0.5f, (0f - RDClass.Vfx.RDHeight) * 0.5f, 2f);
			mPagoda.transform.localPosition = localPosition;
			mPagodaLights.transform.localPosition = localPosition;
			PulseOnEveryBar(1.03f, 0.1f, 0f);
			PulseOnEveryBar(1.03f, 0.1f, 0.5f);
			PulseOnEveryBar(1.1f, 0.25f, 1f);
			PulseOnEveryBar(1.03f, 0.1f, 2f);
			PulseOnEveryBar(1.03f, 0.05f, 2.75f);
			PulseOnEveryBar(1.1f, 0.25f, 3f);
			Narration.NarrateRowInfo(base.rows[1], NarrateInfoType.Connect, soundOnly: false, narrateSkipBeats: false, null, skipsUnstable: false, RDPlayer.AutoDetect, -1);
			if (GC.twoPlayerMode)
			{
				scrExecuteOnCertainBeat.Add(() =>
				{
					Narration.NarrateRowInfo(base.rows[2], NarrateInfoType.Connect, soundOnly: false, narrateSkipBeats: false, null, skipsUnstable: false, RDPlayer.AutoDetect, -1);
				}, 0.5f);
			}
			break;
		}
		case 5:
			base.room0.wavyRows = true;
			break;
		case 7:
			SetPossessed(1);
			stutter.AddStutter(1f, 1f, 1f / 6f, 3);
			stutter.AddStutter(3f, 3f, 0.25f, 2);
			break;
		case 8:
			heartCenter = base.rows[1].ent.heart.xGlobal;
			stutter.AddStutter(2.5f, 2.5f, 1f / 6f, 3);
			stutter.AddStutter(3f, 3f, 0.125f, 8);
			NarrateDescription("InsomniacHard.desc2");
			break;
		case 9:
			stutter.AddStutter(1f, 1f, 0.25f, 4);
			stutter.AddStutter(3.75f, 3f, 0.25f, 1);
			break;
		case 10:
			stutter.AddStutter(1f, 1f, 0.25f, 4);
			stutter.AddStutter(2f, 2f, 0.25f, 4);
			stutter.AddStutter(3f, 3f, 0.25f, 4);
			break;
		case 11:
			stutter.AddStutter(2f, 2f, 0.25f, 4);
			stutter.AddStutter(3f, 3f, 1f / 6f, 6);
			break;
		case 12:
			base.room0.showTextAboveBeatboxes = false;
			stutter.AddStutter(1f, 1f, 0.25f, 4);
			NarrateDescription("InsomniacHard.desc3");
			break;
		case 13:
			stutter.AddStutter(1.75f, 1f, 0.25f, 1);
			stutter.AddStutter(2.25f, 1f, 0.25f, 1);
			break;
		case 14:
			stutter.AddStutter(1.75f, 1f, 0.25f, 1);
			stutter.AddStutter(2.25f, 1f, 0.25f, 1);
			break;
		case 15:
			stutter.AddStutter(1.75f, 1f, 0.25f, 1);
			stutter.AddStutter(2.25f, 1f, 0.25f, 1);
			break;
		case 16:
			NarrateDescription("InsomniacHard.desc4");
			stutter.AddStutter(1f, 1f, 0.125f, 3);
			stutter.AddStutter(2.5f, 2.5f, 0.125f, 4);
			break;
		case 17:
			if (!base.game.reducedFlash)
			{
				scrExecuteOnCertainBeat.Add(() =>
				{
					RDClass.Vfx.SetFlipScreenX(x: true);
				}, 1.25f);
				scrExecuteOnCertainBeat.Add(() =>
				{
					RDClass.Vfx.SetFlipScreenX(x: false);
				}, 1.5f);
			}
			stutter.AddStutter(1.5f, 1.5f, 1f, 2);
			stutter.EndStutterOnBeat(3f);
			break;
		case 18:
			stutter.AddStutter(0f, 0f, 0.75f, 2);
			stutter.AddStutter(1f, 1f, 0.75f, 2);
			stutter.AddStutter(2.5f, 2.5f, 0.125f, 4);
			NarrateDescription("InsomniacHard.desc5");
			break;
		case 19:
			stutter.AddStutter(0f, 0f, 0.75f, 2);
			stutter.AddStutter(1f, 1f, 0.75f, 2);
			stutter.AddStutter(3f, 3f, 0.25f, 4);
			break;
		case 20:
			if (!base.game.reducedFlash)
			{
				RDClass.Vfx.Flash();
			}
			SetPossessed(0);
			mBossCharacter.PlayExpression("neutral");
			if (GC.twoPlayerMode)
			{
				mBossCharacter2.PlayExpression("neutral");
			}
			if (numberOfTries != 5)
			{
				base.ink.Run("Interlude");
			}
			else
			{
				base.ink.Run("InterludeAlt");
			}
			SetAllPulses(active: false);
			break;
		case 22:
			base.game.HealRowsToMinimum(3f);
			base.game.ReviveDeadRows();
			break;
		case 24:
			SetAllPulses(active: true);
			base.game.gameState = GameState.Handmode;
			SetPossessed(1);
			stutter.AddStutter(1.5f, 1.25f, 0.125f, 1);
			stutter.AddStutter(1.625f, 1.5f, 0.125f, 1);
			stutter.AddStutter(2f, 1.75f, 0.125f, 1);
			stutter.AddStutter(2.125f, 2f, 0.125f, 1);
			break;
		case 25:
			if (base.conductor.lastBarScrubbed < 4)
			{
				fakePopup.ShowSinglePopup();
				scrExecuteOnCertainBeat.Add(() =>
				{
					fakePopup.KillOnePopup();
				}, 3f);
			}
			stutter.AddStutter(3f, 3f, 0.25f, 4, stack: true);
			break;
		case 26:
			if (base.conductor.lastBarScrubbed < 4)
			{
				fakePopup.ShowManyPopups();
			}
			break;
		case 27:
			fakePopup.KillManyPopups();
			stutter.AddStutter(1f, 1f, 0.125f, 4);
			stutter.AddStutter(2.5f, 2.5f, 0.125f, 4);
			stutter.AddStutter(3.5f, 0f, 3.5f, 1, stack: false, -7);
			scrExecuteOnCertainBeat.Add(() =>
			{
				SetVHSRewindEffect(on: true);
			}, 3.5f);
			break;
		case 28:
			SetVHSRewindEffect(on: false);
			stutter.AddStutter(1f, 1f, 0.125f, 4);
			stutter.AddStutter(2.5f, 2.5f, 0.125f, 4);
			break;
		case 29:
			StartInterlude();
			NarrateDescription("InsomniacHard.desc6");
			break;
		case 30:
			scrExecuteOnCertainBeat.Add(() =>
			{
				scrConductor.PlayImmediately("sndBossVirusDie", 0.8f, RDUtils.GetMixerGroup("LevelCutscene"), RDTime.speed);
			}, 4.5f);
			break;
		case 31:
			RDClass.Vfx.SpotlightFocusFromCenter(heartCenter, 0.7f, playSwitchSound: true, RDUtils.GetMixerGroup("LevelCutscene"));
			NarrateDescription("InsomniacHard.desc7");
			if (base.rows[1].ent != null)
			{
				base.rows[1].ent.heart.Infect();
			}
			if (GC.twoPlayerMode && base.rows[2].ent != null)
			{
				base.rows[2].ent.heart.Infect();
			}
			break;
		case 33:
			scrExecuteOnCertainBeat.Add(() =>
			{
				scrConductor.PlayImmediately("sndBossDisappear", 1f, RDUtils.GetMixerGroup("LevelCutscene"), RDTime.speed);
			}, 1f);
			if (!base.game.reducedFlash)
			{
				RDClass.Vfx.Flash(-1, 2f);
			}
			base.game.gameState = GameState.Cutscene;
			base.ink.RunOnBeat("Interlude2", () =>
			{
				base.game.gameState = GameState.Handmode;
			}, 4f);
			break;
		case 35:
			base.game.HealRowsToMinimum(3f);
			base.game.ReviveDeadRows();
			break;
		case 41:
		{
			int num4 = 29;
			if (GC.twoPlayerMode && base.game.playersAlive > 1)
			{
				num4 *= 2;
			}
			hpController.bossHP.SetMaxHP(num4, compensateMisses: false);
			base.room0.showTextAboveBeatboxes = false;
			base.game.gameState = GameState.Handmode;
			base.room0.wavyRows = true;
			RDClass.Vfx.SpotlightOpenAllTheWay(1f, playSwitchSound: false, RDUtils.GetMixerGroup("LevelCutscene"));
			break;
		}
		case 48:
			AddObstruction(2, 6f, 1f, 0.5f);
			break;
		case 49:
			base.room0.showTextAboveBeatboxes = true;
			mSkyBackground.visible = false;
			if (!base.game.reducedFlash)
			{
				RDClass.Vfx.Flash();
			}
			RDClass.Vfx.infiniteZoom = true;
			SetVHSEffect(on: true);
			vhs.Calibrage = 0.35f;
			base.game.SetVisualSpeed(2f);
			SetBgStyle(2);
			NarrateDescription("InsomniacHard.desc8");
			break;
		case 57:
			if (!base.game.reducedFlash)
			{
				RDClass.Vfx.Flash();
			}
			base.game.SetVisualSpeed(3f);
			NarrateDescription("InsomniacHard.desc9");
			break;
		case 66:
			if (!base.game.reducedFlash)
			{
				RDClass.Vfx.Flash();
			}
			base.game.ReviveDeadRows();
			RDClass.Vfx.infiniteZoom = false;
			SetVHSEffect(on: false);
			SetVHSRewindEffect(on: true);
			base.game.SetVisualSpeed(1f);
			base.game.gameState = GameState.Cutscene;
			base.ink.Run("Interlude3", () =>
			{
				base.game.gameState = GameState.Handmode;
			});
			break;
		case 69:
			RDBaseDllDummy.printem("case 69");
			scrExecuteOnCertainBeat.Add(() =>
			{
				RDClass.Vfx.SetOverlayToAlpha(1f);
			}, 7f);
			break;
		case 166:
		{
			RDBaseDllDummy.printem("all of this is happening");
			int num2 = 19;
			if (GC.twoPlayerMode && base.game.playersAlive > 1)
			{
				num2 *= 2;
			}
			hpController.bossHP.SetMaxHP(num2);
			base.room0.showTextAboveBeatboxes = false;
			petals1.gameObject.SetActive(value: true);
			petals2.gameObject.SetActive(value: true);
			petals3.gameObject.SetActive(value: true);
			petals1.time = 200f;
			petals2.time = 200f;
			petals3.time = 200f;
			SetPossessed(2);
			scrExecuteOnCertainBeat.Clear(execute: false);
			RDClass.Vfx.Flash();
			RDClass.Vfx.FadeIn(0f);
			ShowMainBackgrounds(visible: true);
			scrVolumeTracker.ClearAll();
			Background[] array = arrMatrix;
			for (int num3 = 0; num3 < array.Length; num3++)
			{
				array[num3].SetAlpha(1f);
			}
			scrVolumeTracker.Add((float s) =>
			{
				RDClass.Vfx.scrollYMultiplier = s;
			}, 0.1f, 2f);
			stutter.AddStutter(1f, 1f, 0.25f, 4);
			base.game.SetVisualSpeed(2f);
			base.room0.wavyRowsAmplitude *= 2f;
			NarrateDescription("InsomniacHard.desc10");
			break;
		}
		case 167:
			stutter.AddStutter(1f, 1f, 0.125f, 2);
			stutter.AddStutter(1.25f, 1f, 0.25f, 2);
			stutter.AddStutter(2.5f, 1f, 0.25f, 1);
			stutter.AddStutter(2.75f, 1f, 0.125f, 2);
			stutter.AddStutter(3.5f, 3f, 0.5f, 1);
			break;
		case 168:
			stutter.AddStutter(0.5f, 0.5f, 1f / 12f, 3);
			stutter.AddStutter(1f, 1f, 1f / 12f, 3);
			stutter.AddStutter(1.75f, 1.75f, 1f / 12f, 3);
			stutter.AddStutter(2.25f, 2.25f, 1f / 12f, 3);
			break;
		case 169:
			stutter.AddStutter(0f, 0f, 0.25f, 8);
			stutter.AddStutter(2f, 2f, 0.5f, 2);
			stutter.AddStutter(3f, 3f, 1f / 6f, 3);
			AddObstruction(1, 3.5f, 0.5f);
			break;
		case 170:
			stutter.AddStutter(3f, 3f, 0.25f, 4);
			break;
		case 171:
			stutter.AddStutter(1f, 1f, 0.0625f, 4);
			stutter.AddStutter(1.25f, 1f, 1f / 12f, 3);
			stutter.AddStutter(1.5f, 1f, 0.125f, 2);
			stutter.AddStutter(1.75f, 1f, 0.25f, 3);
			stutter.AddStutter(3.5f, 1f, 0.125f, 4);
			break;
		case 172:
			stutter.AddStutter(0.5f, 0f, 0.25f, 1);
			stutter.AddStutter(1.25f, 0.75f, 0.75f, 1);
			stutter.AddStutter(2f, 0f, 0.25f, 1);
			stutter.AddStutter(2.25f, 0.25f, 0.25f, 1);
			stutter.AddStutter(2.5f, 2.25f, 0.25f, 1);
			stutter.AddStutter(2.75f, 0f, 0.25f, 1);
			stutter.AddStutter(3.5f, 0f, 0.25f, 1);
			break;
		case 173:
			stutter.AddStutter(2f, 2f, 0.25f, 4);
			stutter.AddStutter(3f, 3f, 0.25f, 2);
			AddObstruction(1, 3.5f, 0.5f);
			SetVHSEffect(on: true);
			base.game.SetVisualSpeed(3f);
			NarrateDescription("InsomniacHard.desc11");
			break;
		case 174:
		case 175:
		case 178:
			stutter.AddStutter(1.25f, 0f, 0.5f, 1);
			stutter.AddStutter(1.75f, 0f, 0.25f, 1);
			stutter.AddStutter(2f, 0.75f, 0.25f, 1);
			stutter.AddStutter(2.25f, 0f, 0.25f, 1);
			stutter.AddStutter(2.75f, 0f, 0.25f, 1);
			stutter.AddStutter(3f, 3f, 0.5f, 2);
			break;
		case 176:
			AddObstruction(2, 0.55f, 0.52000004f);
			if (!base.game.reducedFlash)
			{
				AddObstruction(2, 1.32f, 0.54999995f);
			}
			AddObstruction(2, 2.07f, 0.93000007f);
			break;
		case 177:
			stutter.AddStutter(1.25f, 0f, 0.5f, 1);
			stutter.AddStutter(1.75f, 0f, 0.25f, 1);
			stutter.AddStutter(2f, 0.75f, 0.25f, 1);
			stutter.AddStutter(2.25f, 0f, 0.25f, 1);
			stutter.AddStutter(2.75f, 0f, 0.25f, 1);
			stutter.AddStutter(3f, 3f, 0.5f, 2);
			AddObstruction(1, 3.5f, 0.5f);
			break;
		case 179:
			stutter.AddStutter(0.5f, 0f, 0.32000002f, 1);
			stutter.AddStutter(0.8f, 0f, 0.2f, 1);
			stutter.AddStutter(1f, 0.75f, 0.08f, 1);
			stutter.AddStutter(1.08f, 0f, 0.08f, 7);
			stutter.AddStutter(1.64f, 0f, 0.16f, 1);
			stutter.AddStutter(1.8f, 0f, 0.2f, 1);
			stutter.AddStutter(0.5f, 0f, 0.08f, 1);
			stutter.AddStutter(2.08f, 0.8f, 0.08f, 1);
			stutter.AddStutter(2.16f, 1.4f, 0.08f, 1);
			stutter.AddStutter(2.24f, 2f, 0.08f, 1);
			stutter.AddStutter(2.48f, 2f, 0.08f, 1);
			stutter.AddStutter(2.56f, 1.8f, 0.08f, 1);
			stutter.AddStutter(2.64f, 1.6f, 0.08f, 1);
			stutter.AddStutter(2.72f, 0.8f, 0.08f, 1);
			stutter.AddStutter(2.8f, 0.3f, 0.08f, 1);
			stutter.AddStutter(2.88f, 0f, 0.08f, 1);
			stutter.AddStutter(3.47f, 0f, 0.08f, 1);
			stutter.AddStutter(3.56f, 0.5f, 0.08f, 1);
			stutter.AddStutter(3.63f, 1f, 0.08f, 1);
			stutter.AddStutter(2.72f, 1.5f, 0.08f, 1);
			stutter.AddStutter(2.8f, 2f, 0.08f, 1);
			stutter.AddStutter(2.88f, 2.5f, 0.08f, 1);
			break;
		case 180:
			vhs.Calibrage = 1f;
			scrExecuteOnCertainBeat.Add(() =>
			{
				vhs.Calibrage = 2f;
			}, 3.5f);
			if (!base.game.reducedFlash)
			{
				scrExecuteOnHit.Add(() =>
				{
					RDClass.Vfx.Flash();
				}, HitType.Hit, persistent: true);
			}
			break;
		case 181:
			scrExecuteOnCertainBeat.Add(() =>
			{
				vhs.Calibrage = 3f;
			}, 3f);
			break;
		case 182:
			SetPossessed(0);
			mBossCharacter.PlayExpression("neutral");
			if (GC.twoPlayerMode)
			{
				mBossCharacter2.PlayExpression("neutral");
			}
			if (!base.game.reducedFlash)
			{
				RDClass.Vfx.Flash(-1, 3f);
			}
			base.game.SetVisualSpeed(0.5f);
			scrExecuteOnCertainBeat.Add(() =>
			{
				Narration.NarrateRowInfo(base.rows[1], NarrateInfoType.Disconnect, soundOnly: false, narrateSkipBeats: false, null, skipsUnstable: false, RDPlayer.AutoDetect, -1);
			}, 2.5f);
			scrExecuteOnCertainBeat.Add(() =>
			{
				SetVHSEffect(on: false);
				SetVHSRewindEffect(on: false);
				if (GC.twoPlayerMode)
				{
					Narration.NarrateRowInfo(base.rows[2], NarrateInfoType.Disconnect, soundOnly: false, narrateSkipBeats: false, null, skipsUnstable: false, RDPlayer.AutoDetect, -1);
				}
			}, 3f);
			break;
		case 185:
			base.game.DeleteRow(1);
			if (GC.twoPlayerMode)
			{
				base.game.DeleteRow(2);
			}
			NarrateDescription("InsomniacHard.desc12");
			break;
		case 187:
			SetAllPulses(active: false);
			base.game.handControllerOnTop.Show(show: false);
			hpController.Show(show: true);
			RDClass.Vfx.BgColor(Color.black);
			scrVolumeTracker.ClearAll();
			mPagodaLights.FadeOut();
			mPagoda.FadeOut();
			break;
		case 188:
			mCat.FadeOut();
			mBat.FadeOut();
			break;
		case 189:
			mPetals.FadeOut();
			mNightBackground.FadeOut();
			break;
		case 190:
			scrExecuteOnEveryBeat.Clear();
			base.rankscreen.AdvanceGameover();
			base.rankscreen.AdvanceGameover();
			if (base.game.mistakesManager.mistakes == 0f)
			{
				base.ink.Run("Perfect", () =>
				{
					base.rankscreen.ExitLevelAfterSeconds(2f, closeBlades: true);
				});
			}
			break;
		case 191:
			base.rankscreen.AdvanceGameover();
			break;
		case 192:
			if (base.game.mistakesManager.mistakes != 0f)
			{
				base.rankscreen.ExitLevelAfterSeconds(3f, closeBlades: true);
			}
			break;
		}
	}

	public void NumberFlashingOnAction()
	{
		if (base.conductor.barNumber >= 41 && base.conductor.barNumber < 49)
		{
			int size = 90;
			float endBeat = 99f;
			RDClass.Vfx.FlashTextUI("1", 0f, endBeat, size, flash: false);
			RDClass.Vfx.FlashTextUI("2", 1f, endBeat, size, flash: false);
			RDClass.Vfx.FlashTextUI("3", 2f, endBeat, size, flash: false);
			RDClass.Vfx.FlashTextUI("4", 3f, endBeat, size, flash: false);
			RDClass.Vfx.FlashTextUI("5", 4f, endBeat, size, flash: false);
			RDClass.Vfx.FlashTextUI("6", 5f, endBeat, size, flash: false);
			RDClass.Vfx.FlashTextUI("<color=cyan>7</color>", 6f, endBeat, size, flash: false);
			if (!base.game.reducedFlash)
			{
				scrExecuteOnCertainBeat.Add(() =>
				{
					RDClass.Vfx.GetRoomOverlay(-1).Flash(0.25f, 0.5f);
				}, 6f);
			}
		}
		if (base.conductor.barNumber == 49)
		{
			RDClass.Vfx.FlashTextUI("", 0f, 0f);
		}
	}

	private void StartInterlude()
	{
		RDBaseDllDummy.printem("starting interlude");
		if (!base.game.reducedFlash)
		{
			RDClass.Vfx.Flash();
		}
		petals1.gameObject.SetActive(value: false);
		petals2.gameObject.SetActive(value: false);
		petals3.gameObject.SetActive(value: false);
		base.game.gameState = GameState.Cutscene;
		SetPossessed(0);
		mBossCharacter.PlayExpression("neutral");
		if (GC.twoPlayerMode)
		{
			mBossCharacter2.PlayExpression("neutral");
		}
		ShowMainBackgrounds(visible: false);
		mSkyBackground.visible = true;
		base.tkCam.GetComponent<VideoBloom>().masterAmount = 0.35f;
		RDClass.Vfx.BgColor(Color.black);
		base.room0.wavyRows = false;
		ShowMatrixes(show: true);
	}

	public override void OnMistake(RowEntity ent = null)
	{
		stutter.EndCurrentStutter();
	}

	private void ShowMainBackgrounds(bool visible)
	{
		foreach (Background mBackground in mBackgrounds)
		{
			mBackground.gameObject.SetActive(visible);
		}
	}

	public void AddObstruction(int type, float playbeat, float length, float alpha = 1f)
	{
		float num = 0f;
		Background obstruction = null;
		switch (type)
		{
		case 1:
			obstruction = RDClass.Vfx.fxGlitch;
			break;
		case 2:
			obstruction = RDClass.Vfx.fxNoise;
			break;
		case 3:
			obstruction = RDClass.Vfx.fxNoise;
			num = base.conductor.crotchet * 2f;
			break;
		default:
			print("error NOT implemented");
			break;
		}
		if (num != 0f)
		{
			obstruction.SetAlpha(0f);
			obstruction.tiledSprite.DOFade(1f, num);
		}
		else
		{
			obstruction.SetAlpha(alpha);
		}
		scrExecuteOnCertainBeat.Add(() =>
		{
			obstruction.visible = true;
		}, playbeat);
		if (playbeat >= 0f)
		{
			scrExecuteOnCertainBeat.Add(() =>
			{
				obstruction.visible = false;
			}, playbeat + length);
		}
		else
		{
			DOVirtual.DelayedCall(length * base.conductor.crotchet, () =>
			{
				obstruction.visible = false;
			});
		}
	}

	public override bool FailLevel(RowEntity ent)
	{
		Persistence.SetLevelRank(base.game.levelIdentifier, -1);
		RDClass.Vfx.fxGlitch.visible = false;
		RDClass.Vfx.fxNoise.visible = false;
		fakePopup.KillManyPopups();
		fakePopup.KillOnePopup();
		failedLevel = true;
		RDClass.Vfx.Flash();
		RDClass.Vfx.ShakeCam(30, 20);
		if (RDClass.Vfx.infiniteZoom)
		{
			print("just show as simple screen");
			RDClass.Vfx.sevenBeatArmy.JustShowASimpleScreen();
		}
		base.room0.wavyRows = false;
		base.game.frontOfAllOverlay.gameObject.SetActive(value: false);
		SetVHSRewindEffect(on: false);
		SetVHSEffect(on: false);
		mBossCharacter.animator.deltaTimeStyle = tk2dSpriteAnimator.DeltaTimeStyle.AlwaysOneAlways;
		if (GC.twoPlayerMode)
		{
			mBossCharacter2.animator.deltaTimeStyle = tk2dSpriteAnimator.DeltaTimeStyle.AlwaysOneAlways;
		}
		DOVirtual.DelayedCall(2f, () =>
		{
			ent.heart.DoFinalCrack();
			RDUtils.Rotate2D(ent.heart.transform, 0f);
		});
		DOVirtual.DelayedCall(3f, () =>
		{
			RDClass.Vfx.SpotlightFocusFromCenter(mBossCharacter.transform.position.x, 0.5f, playSwitchSound: true, RDUtils.GetMixerGroup("FeedbackSoundsParent"), 1.4f);
		});
		DOVirtual.DelayedCall(5f, () =>
		{
			mBossCharacter.PlayExpression("gameover");
			if (mBossCharacter2 != null)
			{
				mBossCharacter2.PlayExpression("gameover");
			}
		});
		DOVirtual.DelayedCall(8f, () =>
		{
			base.game.statusText.Show("status.bossFailRestart", narrate: true);
			string knotName = "GameOver0";
			if (base.conductor.barNumber > 30)
			{
				knotName = "GameOver1";
			}
			if (base.conductor.barNumber > 100)
			{
				knotName = "GameOverClose";
			}
			base.ink.Run(knotName, () =>
			{
				AddObstruction(1, -1f, 1f);
				scrConductor.PlayImmediately("sndNoiseMed", 0.5f);
				base.rankscreen.ExitLevelAfterSeconds(0.3f);
			});
		});
		scrExecuteOnPeriodicBeat.Clear();
		return true;
	}

	private void QuitLevel()
	{
		base.rankscreen.ExitLevelAfterSeconds(1f);
	}

	private void SetVHSEffect(bool on)
	{
		vhs.enabled = on;
	}

	private void SetVHSRewindEffect(bool on)
	{
		if (!base.game.reducedFlash)
		{
			vhsRewind.enabled = on;
		}
	}

	private void SetPossessed(int possessed)
	{
		string neutralAnimName = "neutral";
		switch (possessed)
		{
		case 1:
			neutralAnimName = "headspin";
			break;
		case 2:
			neutralAnimName = "headspin2";
			break;
		}
		mBossCharacter.neutralAnimName = neutralAnimName;
		if (GC.twoPlayerMode)
		{
			mBossCharacter2.neutralAnimName = neutralAnimName;
		}
	}

	private void PulseOnEveryBar(float pulseScale, float pulseLength, float beat)
	{
		scrExecuteOnPeriodicBeat item = scrExecuteOnPeriodicBeat.Add(() =>
		{
			PulsePagoda(pulseScale, pulseLength);
		}, base.conductor.crotchetsPerBar, beat);
		mPagodaPulses.Add(item);
	}

	private void PulsePagoda(float s, float pulseLength)
	{
		s = Mathf.Pow(s, 2f);
		Vector3 localScale = new Vector3(s, s, 1f);
		mPagoda.transform.localScale = localScale;
		mPagoda.transform.DOKill();
		mPagoda.transform.DOScale(Vector3.one, pulseLength);
		mPagodaLights.transform.localScale = localScale;
		mPagodaLights.transform.DOKill();
		mPagodaLights.transform.DOScale(Vector3.one, pulseLength);
	}

	private void SetAllPulses(bool active)
	{
		foreach (scrExecuteOnPeriodicBeat mPagodaPulse in mPagodaPulses)
		{
			mPagodaPulse.active = active;
		}
	}

	public override void OnEnterLowHealth()
	{
		LowHealthBeepCounter = 32;
		base.OnEnterLowHealth();
		RDClass.Vfx.FlashTextUIInstant("<color=red>LOW HEALTH</color>\n", 32, flash: false);
	}

	public override void OnExitLowHealth()
	{
		base.OnExitLowHealth();
		RDClass.Vfx.FlashTextUIInstant("", 32, flash: false);
	}

	public override void OnLowHealthEveryPrebar()
	{
		if (LowHealthBeepCounter == 0)
		{
			RDClass.Vfx.FlashTextUI("", 0f, 0f, 40, flash: false);
			return;
		}
		int num = base.conductor.barNumber;
		bool flag = false;
		int num2 = 4;
		LowHealthBeepCustomPlayed = false;
		float num3 = 1f;
		if (num >= 4 && num < 20)
		{
			base.conductor.PlayBeatInCrotchets("sndLowHealthDA", 0f, num3);
			base.conductor.PlayBeatInCrotchets("sndLowHealthDA", 2f, num3);
			flag = true;
		}
		else if (num >= 24 && num < 29)
		{
			base.conductor.PlayBeatInCrotchets("sndLowHealthDA", 0f, num3);
			base.conductor.PlayBeatInCrotchets("sndLowHealthDA", 2f, num3);
			flag = true;
		}
		else if (num >= 41 && num < 66)
		{
			base.conductor.PlayBeatInCrotchets("sndLowHealthDA", 0f, num3);
			base.conductor.PlayBeatInCrotchets("sndLowHealthDA", 4f, num3);
			if (num >= 50)
			{
				flag = true;
			}
		}
		else if (num >= 166 && num <= 182)
		{
			flag = true;
			num2 = 1;
			if (num == 166)
			{
				LowHealthBeep("sndLowHealthE", 0f, num3);
				LowHealthBeep("sndLowHealthB", 1f, num3);
				LowHealthBeep("sndLowHealthE", 2f, num3);
				LowHealthBeep("sndLowHealthB", 3f, num3);
				LowHealthBeep("sndLowHealthE", 3.5f, num3);
			}
			if (num == 167)
			{
				LowHealthBeep("sndLowHealthB", 0.5f, num3);
				LowHealthBeep("sndLowHealthE", 1.5f, num3);
				LowHealthBeep("sndLowHealthB", 2.5f, num3);
				LowHealthBeep("sndLowHealthE", 3f, num3);
			}
			if (num == 168)
			{
				LowHealthBeep("sndLowHealthB", 0f, num3);
				LowHealthBeep("sndLowHealthE", 1f, num3);
				LowHealthBeep("sndLowHealthB", 2f, num3);
				LowHealthBeep("sndLowHealthE", 2.5f, num3);
				LowHealthBeep("sndLowHealthB", 3.5f, num3);
			}
			if (num == 169)
			{
				LowHealthBeep("sndLowHealthE", 0.5f, num3);
				LowHealthBeep("sndLowHealthB", 1.5f, num3);
				LowHealthBeep("sndLowHealthE", 2f, num3);
				LowHealthBeep("sndLowHealthB", 3f, num3);
			}
			if (num == 170)
			{
				LowHealthBeep("sndLowHealthE", 0f, num3);
				LowHealthBeep("sndLowHealthB", 1f, num3);
				LowHealthBeep("sndLowHealthE", 1.5f, num3);
				LowHealthBeep("sndLowHealthB", 2.5f, num3);
				LowHealthBeep("sndLowHealthE", 3.5f, num3);
			}
			if (num == 171)
			{
				LowHealthBeep("sndLowHealthB", 0.5f, num3);
				LowHealthBeep("sndLowHealthE", 1f, num3);
				LowHealthBeep("sndLowHealthB", 2f, num3);
				LowHealthBeep("sndLowHealthE", 3f, num3);
			}
			if (num == 172)
			{
				LowHealthBeep("sndLowHealthB", 0f, num3);
				LowHealthBeep("sndLowHealthE", 0.5f, num3);
				LowHealthBeep("sndLowHealthB", 1.5f, num3);
				LowHealthBeep("sndLowHealthE", 2.5f, num3);
				LowHealthBeep("sndLowHealthB", 3.5f, num3);
			}
			if (num == 173)
			{
				LowHealthBeep("sndLowHealthE", 0f, num3);
				LowHealthBeep("sndLowHealthB", 1f, num3);
				LowHealthBeep("sndLowHealthE", 2f, num3);
				LowHealthBeep("sndLowHealthB", 3f, num3);
				LowHealthBeep("sndLowHealthE", 3.5f, num3);
			}
			if (num == 174)
			{
				LowHealthBeep("sndLowHealthB", 0.5f, num3);
				LowHealthBeep("sndLowHealthE", 1.5f, num3);
				LowHealthBeep("sndLowHealthB", 2.5f, num3);
				LowHealthBeep("sndLowHealthE", 3f, num3);
			}
			if (num == 175)
			{
				LowHealthBeep("sndLowHealthB", 0f, num3);
				LowHealthBeep("sndLowHealthE", 1f, num3);
				LowHealthBeep("sndLowHealthB", 2f, num3);
				LowHealthBeep("sndLowHealthE", 2.5f, num3);
				LowHealthBeep("sndLowHealthB", 3.5f, num3);
			}
			if (num == 176)
			{
				LowHealthBeep("sndLowHealthE", 0.5f, num3);
				LowHealthBeep("sndLowHealthB", 1.5f, num3);
				LowHealthBeep("sndLowHealthE", 2f, num3);
				LowHealthBeep("sndLowHealthB", 3f, num3);
			}
			if (num == 177)
			{
				LowHealthBeep("sndLowHealthE", 0f, num3);
				LowHealthBeep("sndLowHealthB", 1f, num3);
				LowHealthBeep("sndLowHealthE", 1.5f, num3);
				LowHealthBeep("sndLowHealthB", 2.5f, num3);
				LowHealthBeep("sndLowHealthE", 3.5f, num3);
			}
			if (num == 178)
			{
				LowHealthBeep("sndLowHealthB", 0.5f, num3);
				LowHealthBeep("sndLowHealthE", 1f, num3);
				LowHealthBeep("sndLowHealthB", 2f, num3);
				LowHealthBeep("sndLowHealthE", 3f, num3);
			}
			if (num == 179)
			{
				LowHealthBeep("sndLowHealthB", 0f, num3);
				LowHealthBeep("sndLowHealthE", 0.5f, num3);
				LowHealthBeep("sndLowHealthB", 1.5f, num3);
				LowHealthBeep("sndLowHealthE", 2.5f, num3);
				LowHealthBeep("sndLowHealthB", 3.5f, num3);
			}
			if (num == 180)
			{
				LowHealthBeep("sndLowHealthE", 0f, num3);
				LowHealthBeep("sndLowHealthB", 1f, num3);
				LowHealthBeep("sndLowHealthE", 2f, num3);
				LowHealthBeep("sndLowHealthB", 3f, num3);
				LowHealthBeep("sndLowHealthE", 3.5f, num3);
			}
			if (num == 181)
			{
				LowHealthBeep("sndLowHealthB", 0.5f, num3);
				LowHealthBeep("sndLowHealthE", 1.5f, num3);
				LowHealthBeep("sndLowHealthB", 2.5f, num3);
				LowHealthBeep("sndLowHealthE", 3f, num3);
			}
			if (num == 182)
			{
				LowHealthBeep("sndLowHealthB", 0f, num3);
				LowHealthBeep("sndLowHealthE", 1f, num3);
				LowHealthBeep("sndLowHealthB", 2f, num3);
			}
		}
		if (LowHealthBeepCustomPlayed)
		{
			return;
		}
		string text = RDString.Get("status.lowHealth");
		if (flag)
		{
			for (int i = 0; i < 4; i++)
			{
				RDClass.Vfx.FlashTextUI("<color=red>" + text + "</color>\n", 0f + (float)i, 0f, 32, flash: false);
				RDClass.Vfx.FlashTextUI("<color=#d23131ff>" + text + "</color>\n", 0.25f + (float)i, 0f, 32, flash: false);
				NarrateLowHealth(0f + (float)i);
			}
		}
		else
		{
			RDClass.Vfx.FlashTextUI("", 0f, 0f, 40, flash: false);
		}
		if (flag)
		{
			LowHealthBeepCounter -= num2;
		}
	}

	private void LowHealthBeep(string soundfile, float crotchet, float gainmultiplier)
	{
		base.conductor.PlayBeatInCrotchets(soundfile, crotchet, gainmultiplier, RDUtils.GetMixerGroup("LowHealthBeeps"));
		string text = RDString.Get("status.lowHealth");
		if (soundfile == "sndLowHealthE")
		{
			RDClass.Vfx.FlashTextUI("<color=red>" + text + "</color>\n", crotchet, 0f, 32, flash: false);
			NarrateLowHealth(crotchet);
		}
		if (soundfile == "sndLowHealthB")
		{
			RDClass.Vfx.FlashTextUI("<color=#d23131ff>" + text + "</color>\n", crotchet, 0f, 32, flash: false);
			NarrateLowHealth(crotchet);
		}
		LowHealthBeepCustomPlayed = true;
		LowHealthBeepCounter--;
	}

	private void NarrateLowHealth(float crotchet)
	{
		if (!narratedLowHealth)
		{
			scrExecuteOnCertainBeat.Add(() =>
			{
				Narration.Say(RDString.Get("status.lowHealth"), NarrationCategory.Notification, false);
			}, crotchet);
			narratedLowHealth = true;
		}
	}

	public override void OnHit(HitType hitType, Beat beat = null)
	{
		hpController.bossHP.DealDamage(hitType);
	}
}
