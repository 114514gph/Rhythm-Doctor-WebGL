using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;

public class Level_OrientalInsomniac : LevelBase
{
	private const int Phase1HP = 20;

	private const int Phase1HPLeftover = 4;

	private const int Phase2HP = 15;

	private const int Phase2HPDeducted = 3;

	private const int Phase3HP = 4;

	private const string DialogueFile = "diaInsomniac";

	private const float MinHealthPctToBeep = 0.3f;

	private const string LowHPBeepSound1 = "sndJyi - ChineseCount1";

	private const string LowHPBeepSound2 = "sndJyi - ChineseCount2";

	private List<scrExecuteOnPeriodicBeat> mPagodaPulses = new List<scrExecuteOnPeriodicBeat>();

	private List<Background> mBackgrounds = new List<Background>();

	private Background[] arrMatrix;

	private Background mSkyBackground;

	private Background mGreenTrees;

	private Background mSakuraTree;

	private Background mPagoda;

	private scrStutter stutter;

	private RDBossStageText mBossStageText;

	private scrChar mBossCharacter;

	private scrChar mBossCharacter2;

	private HPBarController hpController;

	public float xBossPosition = 35f;

	private ParticleSystem petals1;

	private ParticleSystem petals2;

	private ParticleSystem petals3;

	public int beepCounter = 4;

	private bool narratedLowHealth;

	private bool playedDisconnectCue;

	public override void Init()
	{
		songsUsed = new string[3] { "sndOrientalInsomniac1", "sndOrientalInsomniac2", "sndOrientalInsomniac3" };
		mBossStageText = LoadLevelAsset<RDBossStageText>();
		mBossStageText.lowerText.color = Color.black;
		mBossStageText.upperText.color = Color.black;
		mBossStageText.transform.localPosition = new Vector3(RDClass.Vfx.RDWidth * 0.5f, RDClass.Vfx.RDHeight * 0.5f, 20f);
		heartExplodeType = HeartExplodeType.Instant;
		hpController = base.game.hpController;
		hpController.gameObject.SetActive(value: false);
		int num = 24;
		if (GC.twoPlayerMode)
		{
			num *= 2;
		}
		hpController.bossHP.SetMaxHP(num, compensateMisses: false);
		dontAddEmptyStripsForPlayers = true;
		cutsceneBarsDesc = new (int, int)[1] { (0, 38) };
		base.ink.Load("diaInsomniac");
		multiroom = true;
		base.game.barToSkipToOnSlash = 29;
	}

	public override IEnumerator LoadBigAssets()
	{
		levelType = LevelType.Boss;
		missesToCrackHeart = 7f;
		bossActNum = 1;
		mSkyBackground = Background.CreateSingle("SkySS");
		mGreenTrees = Background.CreateSingle("TreesSS").SetFPS(1.5f);
		mPagoda = Background.CreateSingle("PagodaSS");
		mPagoda.updates = false;
		mSakuraTree = Background.CreateSingle("SakuraTreeSS", new int[6] { 0, 1, 2, 3, 0, 1 }, 9f).SetFollowBeat(FollowBeatType.HeadbobStyle);
		mSakuraTree.customBeatMultiplier = 2;
		stutter = base.room0.stutter;
		yield return null;
	}

	private void SetBgStyle(int ID)
	{
		if (ID == 1 && RDClass.Vfx.hiResVideoBloom)
		{
			VideoBloom component = base.tkCam.GetComponent<VideoBloom>();
			component.enabled = true;
			component.Tint = 4284618239u.RGBAToColor();
			component.Threshold = 0.4f;
			component.KernelSize = 59f;
			component.MasterAmount = 2.18f;
			component.MediumKernelScale = 2.8f;
			component.HighQuality = false;
			base.room0.SetVignette(enabled: true);
		}
	}

	public new bool Is1HP()
	{
		if (base.conductor.barNumber < 7)
		{
			return false;
		}
		return base.game.currentLevel.missesToCrackHeart == base.game.rows[1].ent.rowMisses + 1f;
	}

	public void PlayLowHealthIfLow()
	{
		if (GC.twoPlayerMode || !Is1HP())
		{
			return;
		}
		if (beepCounter == 0)
		{
			RDClass.Vfx.FlashTextUI("", 0f, 0f, 40, flash: false);
			return;
		}
		int num = base.conductor.barNumber;
		bool flag = false;
		AudioMixerGroup mixerGroup = RDUtils.GetMixerGroup("LowHealthBeeps");
		if (num == 9 || num == 13 || num == 17)
		{
			base.conductor.PlayBeatInCrotchets("sndLowHealthDA", 0f, 1f, mixerGroup);
			base.conductor.PlayBeatInCrotchets("sndLowHealthDA", 1f, 1f, mixerGroup);
			base.conductor.PlayBeatInCrotchets("sndLowHealthDA", 2f, 1f, mixerGroup);
			base.conductor.PlayBeatInCrotchets("sndLowHealthDA", 3f, 1f, mixerGroup);
			flag = true;
		}
		else if (num >= 6 && num < 21)
		{
			base.conductor.PlayBeatInCrotchets("sndLowHealthDA", 0f, 1f, mixerGroup);
			base.conductor.PlayBeatInCrotchets("sndLowHealthDA", 1f, 1f, mixerGroup);
			base.conductor.PlayBeatInCrotchets("sndLowHealthDA", 2f, 1f, mixerGroup);
			base.conductor.PlayBeatInCrotchets("sndLowHealthDA", 3f, 1f, mixerGroup);
			flag = true;
		}
		if (num == 21)
		{
			base.conductor.PlayBeatInCrotchets("sndLowHealthDA", 0f, 1f, mixerGroup);
			base.conductor.PlayBeatInCrotchets("sndLowHealthDA", 1f, 1f, mixerGroup);
			base.conductor.PlayBeatInCrotchets("sndLowHealthCSharpA", 2f, 1f, mixerGroup);
			base.conductor.PlayBeatInCrotchets("sndLowHealthCSharpA", 3f, 1f, mixerGroup);
			flag = true;
		}
		if (num == 22)
		{
			base.conductor.PlayBeatInCrotchets("sndMelonMorse", 0f);
		}
		if (num >= 26 && num <= 29)
		{
			base.conductor.PlayBeatInCrotchets("sndLowHealthDA", 0f, 1f, mixerGroup);
			base.conductor.PlayBeatInCrotchets("sndLowHealthDA", 1f, 1f, mixerGroup);
			base.conductor.PlayBeatInCrotchets("sndLowHealthDA", 2f, 1f, mixerGroup);
			base.conductor.PlayBeatInCrotchets("sndLowHealthDA", 3f, 1f, mixerGroup);
			flag = true;
		}
		if (num >= 38 && num < 53 && num != 48)
		{
			base.conductor.PlayBeatInCrotchets("sndLowHealthEB", 0f, 1f, mixerGroup);
			base.conductor.PlayBeatInCrotchets("sndLowHealthEB", 1f, 1f, mixerGroup);
			base.conductor.PlayBeatInCrotchets("sndLowHealthEB", 2f, 1f, mixerGroup);
			base.conductor.PlayBeatInCrotchets("sndLowHealthEB", 3f, 1f, mixerGroup);
			flag = true;
		}
		if (flag)
		{
			for (int i = 0; i < 4; i++)
			{
				string text = RDString.Get("status.lowHealth");
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
			beepCounter--;
		}
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

	public override void preactions()
	{
		PlayLowHealthIfLow();
		switch (base.conductor.barNumber)
		{
		case 1:
		{
			SetBgStyle(1);
			base.game.shadowRowStyle = ShadowRowStyle.HostToShadowIf2P;
			Color color = new Color(1f, 1f, 1f, 0f);
			mGreenTrees.tiledSprite.color = color;
			mPagoda.tiledSprite.color = color;
			mSakuraTree.tiledSprite.color = color;
			base.conductor.SetBPM(90f);
			PlayMusic(songsUsed[0]);
			RDClass.Vfx.BgColor(Color.white);
			base.conductor.crotchetsPerBar = 4;
			base.game.SetPlayerHitSounds(RowType.Classic, "sndReverbClap", "sndReverbClapP2");
			TurnBeatSoundsOn();
			break;
		}
		case 2:
			hpController.gameObject.SetActive(value: true);
			break;
		case 5:
			petals1 = RDClass.Vfx.ShowParticlePreset("petals1");
			petals2 = RDClass.Vfx.ShowParticlePreset("petals2");
			petals3 = RDClass.Vfx.ShowParticlePreset("petals3");
			RDClass.Vfx.SetSpriteMode(SpriteMode.Outline, Color.black.WithAlpha(1f));
			base.game.MakeRow(RowType.Classic, 1, 0.5f, Character.SamuraiBoss, RDPlayer.P1, RDPlayer.P1, 2);
			mBossCharacter = base.rows[1].ent.character;
			if (GC.twoPlayerMode)
			{
				mBossCharacter2 = base.rows[2].ent.character;
				mBossCharacter.ChangeCharacter(Character.SamuraiBossAlt);
			}
			petals1.time = 50f;
			petals2.time = 50f;
			petals3.time = 50f;
			if (Narration.IsEnabled)
			{
				base.conductor.PlayBeatInCrotchets("sndPatientConnect", 0f, 1f, RDUtils.GetMixerGroup("CuesoundsParent"), 1f, base.rows[1].rowSoundPanAdjustedFor2P);
				if (GC.twoPlayerMode)
				{
					base.conductor.PlayBeatInCrotchets("sndPatientConnect", 2f, 1f, RDUtils.GetMixerGroup("CuesoundsParent"), 1f, base.rows[2].rowSoundPanAdjustedFor2P);
				}
			}
			break;
		case 6:
			base.game.gameState = GameState.Handmode;
			OnBeatClassic(1, 1, 0f);
			CustomPlayExpression("neutral");
			break;
		case 22:
			base.game.gameState = GameState.HandmodePreCutscene;
			OffBeat(1);
			break;
		case 26:
			base.game.gameState = GameState.Handmode;
			OnBeat(1);
			break;
		case 30:
			OffBeat(1);
			break;
		case 38:
			SetPossessed(2);
			base.game.gameState = GameState.Handmode;
			OnBeat(1);
			break;
		case 48:
			TurnBeatSoundsOff();
			break;
		case 49:
			TurnBeatSoundsOn();
			break;
		case 53:
			if (hpController.bossHP.Health <= 0f)
			{
				PlayMusic(songsUsed[2]);
				OffBeat(1);
				PlayDisconnectCues();
				base.conductor.barNumber = 57;
			}
			else
			{
				PlayMusic(songsUsed[1]);
			}
			break;
		case 54:
			TurnBeatSoundsOff();
			break;
		case 57:
			OffBeat(1);
			PlayMusic(songsUsed[2]);
			PlayDisconnectCues();
			break;
		case 99:
			base.conductor.crotchetsPerBar = 4;
			scrExecuteOnEveryBeat.Clear();
			hpController.Show(show: true);
			RDClass.Vfx.BgColor(Color.black);
			base.rankscreen.AdvanceGameover();
			base.rankscreen.AdvanceGameover();
			break;
		case 100:
			base.rankscreen.AdvanceGameover();
			break;
		case 101:
			base.rankscreen.ExitLevelAfterSeconds(3f, closeBlades: true);
			break;
		}
	}

	private void PlayDisconnectCues()
	{
		if (Narration.IsEnabled && !playedDisconnectCue)
		{
			base.conductor.PlayBeatInCrotchets("sndPatientDisconnect", 0f, 1f, RDUtils.GetMixerGroup("CuesoundsParent"), 1f, base.rows[1].rowSoundPanAdjustedFor2P);
			if (GC.twoPlayerMode)
			{
				base.conductor.PlayBeatInCrotchets("sndPatientDisconnect", 2f, 1f, RDUtils.GetMixerGroup("CuesoundsParent"), 1f, base.rows[2].rowSoundPanAdjustedFor2P);
			}
			playedDisconnectCue = true;
		}
	}

	private void TurnBeatSoundsOn()
	{
		base.rows[1].rowSound = "sndShaker";
		base.rows[1].rowSoundVolMultiplier = 0.3f;
		if (GC.twoPlayerMode)
		{
			base.rows[2].rowSound = "sndShaker";
			base.rows[2].rowSoundVolMultiplier = 0.3f;
			base.rows[2].rowSoundMinPitch = 1.3f;
			base.rows[2].rowSoundMaxPitch = 1.3f;
		}
	}

	private void TurnBeatSoundsOff()
	{
		base.game.rows[1].rowSound = null;
		if (GC.twoPlayerMode)
		{
			base.game.rows[2].rowSound = null;
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
		switch (base.conductor.barNumber)
		{
		case 1:
			base.room0.stutter.enabled = true;
			base.game.gameState = GameState.Cutscene;
			mSkyBackground.FadeIn();
			mBackgrounds.Add(mSkyBackground);
			NarrateDescription("BattlewornInsomniac.d1");
			break;
		case 2:
			mBossStageText.Play();
			mGreenTrees.FadeIn();
			mBackgrounds.Add(mGreenTrees);
			base.room0.showTextAboveBeatboxes = true;
			SetUpFlashMarginFeedback();
			RDClass.Vfx.rooms[0].enableSyncText = true;
			base.game.handControllerOnTop.SetHandsInFront(inFront: false);
			break;
		case 3:
			mPagoda.tiledSprite.anchor = tk2dBaseSprite.Anchor.LowerRight;
			mPagoda.transform.localPosition = new Vector3(RDClass.Vfx.RDWidth * 0.5f, (0f - RDClass.Vfx.RDHeight) * 0.5f, 2f);
			mPagoda.FadeIn();
			mBackgrounds.Add(mPagoda);
			break;
		case 4:
			mSakuraTree.FadeIn();
			mBackgrounds.Add(mSakuraTree);
			break;
		case 5:
			Narration.NarrateRowInfo(base.rows[1], NarrateInfoType.Connect, soundOnly: false, narrateSkipBeats: false, null, skipsUnstable: false, RDPlayer.AutoDetect, -1);
			scrExecuteOnCertainBeat.Add(() =>
			{
				Narration.NarrateRowInfo(base.rows[2], NarrateInfoType.Connect, soundOnly: false, narrateSkipBeats: false, null, skipsUnstable: false, RDPlayer.AutoDetect, -1);
			}, 2f);
			break;
		case 6:
			xBossPosition = mBossCharacter.transform.position.x;
			PulseOnEveryBar(1.03f, 0.2f, 0f);
			PulseOnEveryBar(1.03f, 0.2f, 0.5f);
			PulseOnEveryBar(1.1f, 0.5f, 1f);
			PulseOnEveryBar(1.03f, 0.2f, 2f);
			PulseOnEveryBar(1.03f, 0.1f, 2.75f);
			PulseOnEveryBar(1.1f, 0.5f, 3f);
			break;
		case 9:
			SetPossessed(1);
			stutter.AddStutter(1f, 1f, 1f / 6f, 3);
			stutter.AddStutter(3f, 3f, 0.25f, 2);
			break;
		case 10:
			NarrateDescription("BattlewornInsomniac.d2");
			stutter.AddStutter(2.5f, 2.5f, 1f / 6f, 3);
			stutter.AddStutter(3f, 3f, 0.125f, 8);
			break;
		case 11:
			stutter.AddStutter(2f, 2f, 0.5f, 2);
			break;
		case 12:
			stutter.AddStutter(2f, 2f, 0.25f, 4);
			stutter.AddStutter(3f, 3f, 0.25f, 4);
			break;
		case 13:
			MarginFlashFeedback(active: false);
			stutter.AddStutter(1f, 1f, 0.5f, 2);
			stutter.AddStutter(2f, 2f, 0.25f, 4);
			stutter.AddStutter(3f, 3f, 1f / 6f, 6);
			break;
		case 14:
			NarrateDescription("BattlewornInsomniac.d3");
			break;
		case 15:
			stutter.AddStutter(0f, 0f, 0.5f, 2);
			stutter.AddStutter(3f, 3f, 0.25f, 4);
			break;
		case 16:
			stutter.AddStutter(0f, 0f, 0.5f, 2);
			stutter.AddStutter(3f, 3f, 0.25f, 4);
			break;
		case 17:
			stutter.AddStutter(0f, 0f, 0.5f, 2);
			stutter.AddStutter(3f, 3f, 0.25f, 4);
			break;
		case 18:
			stutter.AddStutter(1f, 1f, 0.125f, 3);
			stutter.AddStutter(2.5f, 2.5f, 0.125f, 4);
			break;
		case 19:
			stutter.AddStutter(1f, 0f, 1f, 1, stack: false, -4);
			stutter.AddStutter(1.5f, 1.5f, 1f, 2);
			stutter.EndStutterOnBeat(3f);
			break;
		case 20:
			stutter.AddStutter(0f, 0f, 0.75f, 2);
			stutter.AddStutter(1f, 1f, 0.75f, 2);
			stutter.AddStutter(2.5f, 2.5f, 0.125f, 4);
			break;
		case 21:
			stutter.AddStutter(0f, 0f, 0.75f, 2);
			stutter.AddStutter(1f, 1f, 0.75f, 2);
			stutter.AddStutter(3f, 3f, 0.25f, 4);
			break;
		case 22:
			if (!base.game.reducedFlash)
			{
				RDClass.Vfx.Flash();
			}
			SetAllPulses(active: false);
			SetPossessed(0);
			CustomPlayExpression("neutral");
			base.ink.Run("insomniac1");
			break;
		case 24:
			base.game.ReviveDeadRows();
			break;
		case 26:
			base.game.gameState = GameState.Handmode;
			SetPossessed(1);
			SetAllPulses(active: true);
			stutter.AddStutter(1.5f, 1.25f, 0.125f, 1);
			stutter.AddStutter(1.625f, 1.5f, 0.125f, 1);
			stutter.AddStutter(2f, 1.75f, 0.125f, 1);
			stutter.AddStutter(2.125f, 2f, 0.125f, 1);
			break;
		case 27:
			stutter.AddStutter(1.5f, 1.25f, 0.125f, 1);
			stutter.AddStutter(1.625f, 1.5f, 0.125f, 1);
			stutter.AddStutter(2f, 1.75f, 0.125f, 1);
			stutter.AddStutter(2.125f, 2f, 0.125f, 1);
			stutter.AddStutter(3f, 3f, 0.25f, 4, stack: true);
			break;
		case 28:
			stutter.AddStutter(1.5f, 1.25f, 0.125f, 1);
			break;
		case 29:
			stutter.AddStutter(1.5f, 1.25f, 0.125f, 1);
			stutter.AddStutter(1.625f, 1.5f, 0.125f, 1);
			stutter.AddStutter(2f, 1.75f, 0.125f, 1);
			stutter.AddStutter(2.125f, 2f, 0.125f, 1);
			break;
		case 30:
			base.game.handControllerOnTop.Show(show: false);
			StartInterlude();
			break;
		case 31:
			base.ink.Run("insomniac2");
			break;
		case 34:
			NarrateDescription("BattlewornInsomniac.d4");
			break;
		case 36:
			base.game.ReviveDeadRows();
			base.ink.Run("insomniac3");
			break;
		case 37:
			scrExecuteOnCertainBeat.Add(() =>
			{
				base.ink.Run("insomniac4");
				RDClass.Vfx.ShakeCam(60, 20);
			}, 2.5f);
			scrExecuteOnCertainBeat.Add(() =>
			{
				base.ink.Stop();
				base.game.gameState = GameState.Handmode;
				RDClass.Vfx.FadeOut(0.3f);
				int num2 = 12;
				if (GC.twoPlayerMode && base.game.playersAlive > 1)
				{
					num2 *= 2;
				}
				hpController.bossHP.SetMaxHP(num2, compensateMisses: false);
			}, 3f);
			break;
		case 38:
		{
			base.game.handControllerOnTop.Show(show: true);
			RDClass.Vfx.Flash();
			petals1.gameObject.SetActive(value: true);
			petals2.gameObject.SetActive(value: true);
			petals3.gameObject.SetActive(value: true);
			petals1.time = 200f;
			petals2.time = 200f;
			petals3.time = 200f;
			SetPossessed(2);
			RDClass.Vfx.Flash();
			RDClass.Vfx.FadeIn(0f);
			ShowMainBackgrounds(visible: true);
			scrVolumeTracker.ClearAll();
			Background[] array = arrMatrix;
			for (int num = 0; num < array.Length; num++)
			{
				array[num].SetAlpha(1f);
			}
			scrVolumeTracker.Add((float s) =>
			{
				RDClass.Vfx.scrollYMultiplier = s;
			}, 0.1f, 2f);
			break;
		}
		case 39:
			stutter.AddStutter(1f, 1f, 0.125f, 2);
			stutter.AddStutter(1.25f, 1f, 0.25f, 2);
			stutter.AddStutter(2.5f, 1f, 0.25f, 1);
			stutter.AddStutter(2.75f, 1f, 0.125f, 2);
			stutter.AddStutter(3.5f, 3f, 0.5f, 1);
			break;
		case 40:
			stutter.AddStutter(0.5f, 0.5f, 1f / 12f, 3);
			stutter.AddStutter(1f, 1f, 1f / 12f, 3);
			stutter.AddStutter(1.75f, 1.75f, 1f / 12f, 3);
			stutter.AddStutter(2.25f, 2.25f, 1f / 12f, 3);
			break;
		case 41:
			Narration.Stop();
			stutter.AddStutter(0f, 0f, 0.25f, 8);
			stutter.AddStutter(2f, 2f, 0.5f, 2);
			stutter.AddStutter(3f, 3f, 1f / 6f, 3);
			AddObstruction(1, 3.5f, 0.5f);
			break;
		case 42:
			NarrateDescription("BattlewornInsomniac.d5");
			stutter.AddStutter(3f, 3f, 0.25f, 4);
			break;
		case 43:
			stutter.AddStutter(1f, 1f, 0.0625f, 4);
			stutter.AddStutter(1.25f, 1f, 1f / 12f, 3);
			stutter.AddStutter(1.5f, 1f, 0.125f, 2);
			stutter.AddStutter(1.75f, 1f, 0.25f, 3);
			stutter.AddStutter(3.5f, 1f, 0.125f, 4);
			break;
		case 44:
			stutter.AddStutter(0.5f, 0f, 0.25f, 1);
			stutter.AddStutter(1.25f, 0.75f, 0.75f, 1);
			stutter.AddStutter(2f, 0f, 0.25f, 1);
			stutter.AddStutter(2.25f, 0.25f, 0.25f, 1);
			stutter.AddStutter(2.5f, 2.25f, 0.25f, 1);
			stutter.AddStutter(2.75f, 0f, 0.25f, 1);
			stutter.AddStutter(3.5f, 0f, 0.25f, 1);
			break;
		case 53:
			stutter.AddStutter(0.5f, 0f, 0.25f, 1);
			stutter.AddStutter(1.25f, 0.75f, 0.75f, 1);
			stutter.AddStutter(2f, 0f, 0.25f, 1);
			stutter.AddStutter(2.25f, 0.25f, 0.25f, 1);
			stutter.AddStutter(2.5f, 2.25f, 0.25f, 1);
			stutter.AddStutter(2.75f, 0f, 0.25f, 1);
			stutter.AddStutter(3.5f, 0f, 0.25f, 1);
			break;
		case 45:
			stutter.AddStutter(2f, 2f, 0.25f, 4);
			stutter.AddStutter(3f, 3f, 0.25f, 2);
			AddObstruction(1, 3.5f, 0.5f);
			break;
		case 46:
			NarrateDescription("BattlewornInsomniac.d6");
			stutter.AddStutter(1.25f, 0f, 0.5f, 1);
			stutter.AddStutter(1.75f, 0f, 0.25f, 1);
			stutter.AddStutter(2f, 0.75f, 0.25f, 1);
			stutter.AddStutter(2.25f, 0f, 0.25f, 1);
			stutter.AddStutter(2.75f, 0f, 0.25f, 1);
			stutter.AddStutter(3f, 3f, 0.5f, 2);
			break;
		case 47:
		case 50:
			stutter.AddStutter(1.25f, 0f, 0.5f, 1);
			stutter.AddStutter(1.75f, 0f, 0.25f, 1);
			stutter.AddStutter(2f, 0.75f, 0.25f, 1);
			stutter.AddStutter(2.25f, 0f, 0.25f, 1);
			stutter.AddStutter(2.75f, 0f, 0.25f, 1);
			stutter.AddStutter(3f, 3f, 0.5f, 2);
			break;
		case 48:
			AddObstruction(2, 0.55f, 0.52000004f);
			AddObstruction(2, 1.32f, 0.54999995f);
			AddObstruction(2, 2.07f, 0.93000007f);
			break;
		case 49:
			stutter.AddStutter(1.25f, 0f, 0.5f, 1);
			stutter.AddStutter(1.75f, 0f, 0.25f, 1);
			stutter.AddStutter(2f, 0.75f, 0.25f, 1);
			stutter.AddStutter(2.25f, 0f, 0.25f, 1);
			stutter.AddStutter(2.75f, 0f, 0.25f, 1);
			stutter.AddStutter(3f, 3f, 0.5f, 2);
			AddObstruction(1, 3.5f, 0.5f);
			break;
		case 51:
		case 52:
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
		case 54:
		case 55:
		case 56:
			AddObstruction(3, 0f, 3f);
			break;
		case 57:
			RDClass.Vfx.ShakeCam(200, 30);
			RDClass.Vfx.ReverseFlash(2f);
			Narration.NarrateRowInfo(base.rows[1], NarrateInfoType.Disconnect, soundOnly: false, narrateSkipBeats: false, null, skipsUnstable: false, RDPlayer.AutoDetect, -1);
			scrExecuteOnCertainBeat.Add(() =>
			{
				Narration.NarrateRowInfo(base.rows[2], NarrateInfoType.Disconnect, soundOnly: false, narrateSkipBeats: false, null, skipsUnstable: false, RDPlayer.AutoDetect, -1);
			}, 2f);
			break;
		case 58:
			SetPossessed(0);
			CustomPlayExpression("neutral");
			RDClass.Vfx.StopShakeCam();
			RDClass.Vfx.Flash();
			break;
		case 59:
			base.game.DeleteRow(1, 0f, deleteShadowRow: true, repositionRows: true);
			break;
		case 60:
			NarrateDescription("BattlewornInsomniac.d7");
			hpController.Show(show: true);
			base.game.handControllerOnTop.Show(show: false);
			mBackgrounds[3].FadeOut();
			break;
		case 61:
			mBackgrounds[2].FadeOut();
			break;
		case 62:
			mBackgrounds[1].FadeOut();
			break;
		case 63:
			base.conductor.crotchetsPerBar = 4;
			scrExecuteOnEveryBeat.Clear();
			hpController.Show(show: true);
			RDClass.Vfx.BgColor(Color.black);
			base.rankscreen.AdvanceGameover();
			base.rankscreen.AdvanceGameover();
			break;
		case 64:
			base.rankscreen.AdvanceGameover();
			break;
		case 65:
			base.rankscreen.ExitLevelAfterSeconds(3f, closeBlades: true);
			break;
		case 7:
		case 8:
		case 23:
		case 25:
		case 32:
		case 33:
		case 35:
			break;
		}
	}

	private void CustomPlayExpression(string expression)
	{
		mBossCharacter.PlayExpression(expression);
		if (GC.twoPlayerMode)
		{
			mBossCharacter2.PlayExpression(expression);
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
		mPagoda.transform.localScale = new Vector3(s, s, 1f);
		mPagoda.transform.DOKill();
		mPagoda.transform.DOScale(Vector3.one, pulseLength);
	}

	private void SetAllPulses(bool active)
	{
		foreach (scrExecuteOnPeriodicBeat mPagodaPulse in mPagodaPulses)
		{
			mPagodaPulse.active = active;
		}
	}

	private void StartInterlude()
	{
		if (!base.game.reducedFlash)
		{
			RDClass.Vfx.Flash();
		}
		petals1.gameObject.SetActive(value: false);
		petals2.gameObject.SetActive(value: false);
		petals3.gameObject.SetActive(value: false);
		alwaysSkippableCutscene = true;
		base.game.gameState = GameState.CutsceneSkippable;
		SetPossessed(0);
		CustomPlayExpression("neutral");
		ShowMainBackgrounds(visible: false);
		RDClass.Vfx.BgColor(Color.black);
		arrMatrix = new Background[3];
		arrMatrix[0] = Background.Create("Matrix/0", 0f, 1f).SetBackgroundOrder(5).SetScale(0.75f);
		arrMatrix[1] = Background.Create("Matrix/1", 0f, 2f).SetBackgroundOrder(6).SetScale(1.5f);
		arrMatrix[2] = Background.Create("Matrix/2", 0f, 3f).SetBackgroundOrder(7).SetScale(2.25f);
		scrVolumeTracker.Add((float s) =>
		{
			Background[] array = arrMatrix;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetAlpha(s);
			}
		}, 0f, 5f);
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

	public void AddObstruction(int type, float playbeat, float length)
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
		scrExecuteOnCertainBeat.Add(() =>
		{
			obstruction.visible = true;
		}, playbeat);
		scrExecuteOnCertainBeat.Add(() =>
		{
			obstruction.visible = false;
		}, playbeat + length);
	}

	public override bool FailLevel(RowEntity ent)
	{
		Persistence.SetLevelRank(base.game.levelIdentifier, -1);
		RDClass.Vfx.fxGlitch.visible = false;
		RDClass.Vfx.fxNoise.visible = false;
		failedLevel = true;
		RDClass.Vfx.Flash();
		RDClass.Vfx.ShakeCam(30, 20);
		RDClass.Vfx.KillFlashTextUIAnimations();
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
			RDClass.Vfx.SpotlightFocusFromCenter(xBossPosition, 0.5f, playSwitchSound: true, RDUtils.GetMixerGroup("FeedbackSoundsParent"));
			NarrateDescription("BattlewornInsomniac.GameOver");
		});
		DOVirtual.DelayedCall(5f, () =>
		{
			CustomPlayExpression("gameover");
		});
		DOVirtual.DelayedCall(8f, () =>
		{
			base.game.statusText.Show("status.bossFailRestart", narrate: true);
			GameOverBarInfo genericGameOverInfo = new GameOverBarInfo(-1, new int[13]
			{
				1, 1, 1, 1, 1, -2, -2, -2, -2, 1,
				-2, 1, 2
			});
			PlayGameOverDialogue(new List<GameOverBarInfo>(), genericGameOverInfo);
		});
		scrExecuteOnPeriodicBeat.Clear();
		return true;
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

	public override void OnHit(HitType hitType, Beat beat = null)
	{
		hpController.bossHP.DealDamage(hitType);
	}
}
