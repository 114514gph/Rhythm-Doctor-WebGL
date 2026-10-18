using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Level_Lesmis : LevelBase
{
	public bool chineseVoice;

	private const float oldman_offset = 100f;

	private LyricsGame lyrics1;

	private LyricsGame lyrics2;

	private LyricsGame lyrics3;

	private LyricsGame lyrics4;

	private LyricsGame lyrics1convo;

	private LyricsGame lyrics2convo;

	private LyricsGame lyricsCoda;

	private LyricsGame lyricsOutroDoctor1;

	private LyricsGame lyricsOutroDoctor2;

	private List<LyricsGame> madeLyrics = new List<LyricsGame>();

	private RowEntity paigeRow;

	private scrChar paige;

	private scrChar mrStevenson;

	private scrChar patient2;

	private scrChar politician;

	private Background hospitalBackgroundBg;

	private Background hospitalBedsBg;

	private tk2dSprite hospitalFirstLight;

	private tk2dSprite hospitalSecondLight;

	private tk2dSprite hospitalThirdLight;

	private tk2dSprite hospitalShadow;

	private Background bgHelpSpotlight;

	private scrChar mrStevensonDummy;

	private scrChar ianDummy;

	private Transform cameraContainer0;

	private Transform cameraContainer1;

	private Transform cameraContainer2;

	private Transform doctor1RowContainer;

	private RDBossStageText mBossStageText;

	private BeatClassic finalBeat;

	private GameObject paigeFire;

	private int mHurryUpIndex = 1;

	private const float lightsAlpha = 0.3f;

	private RDMainWard mainWard0;

	private RDMainWard mainWard1;

	private RDMainWard mainWard2;

	private Background rhythmDogtorBg;

	private Background byMorphiousBg;

	private double[][] politicianTimings = new double[5][]
	{
		new double[9] { 0.295, 0.646, 0.914, 1.263, 1.554, 1.774, 2.4, 2.504, 2.679 },
		new double[9] { 0.279, 0.458, 0.731, 0.951, 1.239, 1.371, 1.689, 1.771, 1.97 },
		new double[8] { 0.352, 0.438, 0.611, 1.057, 1.419, 1.612, 1.906, 2.196 },
		new double[8] { 0.237, 0.431, 0.601, 0.848, 1.006, 1.154, 1.311, 1.452 },
		new double[10] { 0.293, 0.73, 0.921, 1.055, 1.398, 1.562, 1.65, 1.841, 1.969, 2.105 }
	};

	private Tween tripTween;

	private float tripRemainingTime;

	private float runningSpeed;

	private float _runningSpeedMultiplier = 1f;

	private const float NormalRunningSpeed = 180f;

	private const float FastRunningSpeed = 240f;

	private float runningSpeedMultiplier
	{
		get
		{
			return _runningSpeedMultiplier;
		}
		set
		{
			_runningSpeedMultiplier = value;
			mainWard0.scrollSpeed = runningSpeed * runningSpeedMultiplier;
		}
	}

	public override void Init()
	{
		dogMode = scnGame.loadDogMode;
		if (RDString.voiceLanguage == SystemLanguage.Chinese && !dogMode)
		{
			chineseVoice = true;
		}
		songsUsed = ((!dogMode) ? ((!chineseVoice) ? new string[2] { "sndLesMisBar1", "sndLesMisBar39" } : new string[2] { "sndLesmisPart1ChineseFull", "sndLesmisPart2Chinese" }) : new string[2] { "sndLesmisDog", "sndLesMisBar39" });
		rankLowerBounds = new float[6] { 100f, 20f, 15f, 8f, 3f, 0f };
		showHitstripOnlyOnActiveBeats = true;
		multiroom = true;
		cutsceneBarsDesc = new (int, int)[1] { (0, 22) };
		heartExplodeMultiplier = 0.5f;
		base.game.barToSkipToOnSlash = 32;
		hideHandsAfterStart = true;
		mBossStageText = LoadLevelAsset<RDBossStageText>();
		mBossStageText.transform.localPosition = new Vector3(RDClass.Vfx.hudCamera.RDFullNativeResolution().x * 0.5f, 99f, 20f);
		if (dogMode)
		{
			customGameoverHeader = RDString.Get("rankscreen.dogtor");
			customGameoverDescription = RDString.Get("rankscreen.dogtorCongratulations");
		}
	}

	public override IEnumerator LoadBigAssets()
	{
		bossActNum = 3;
		mainWard0 = LoadLevelAsset<RDMainWard>();
		mainWard1 = LoadLevelAsset<RDMainWard>();
		mainWard2 = LoadLevelAsset<RDMainWard>();
		mainWard0.ShowVendingMachine(show: false);
		mainWard1.ShowVendingMachine(show: false);
		mainWard2.ShowVendingMachine(show: false);
		mainWard0.dogMode = dogMode;
		mainWard1.dogMode = dogMode;
		mainWard2.dogMode = dogMode;
		mainWard0.name = "mainWard0";
		mainWard1.name = "mainWard1";
		mainWard2.name = "mainWard2";
		mainWard0.svtBlockage.SetActive(value: false);
		mainWard1.svtBlockage.SetActive(value: false);
		mainWard2.svtBlockage.SetActive(value: false);
		mainWard0.transform.localPosition = new Vector3(1598f, 108f);
		mainWard1.transform.localPosition = new Vector3(4696f, 99f);
		mainWard2.transform.localPosition = new Vector3(7835f, 99f);
		bgHelpSpotlight = Background.Create("LesMisSpotlight264", 0f, 0f, isForeground: true).SetFixedPosition().SetFollowCamera();
		bgHelpSpotlight.SetAlpha(0f);
		RDClass.Vfx.FadeOut(0f, 0);
		Background.Create("PoliticianBackground", new int[2] { 0, 1 }, 0f, 0f, 0f, isForeground: false, 3).SetBackgroundOrder(10).SetAlpha(1f)
			.SetFixedPosition()
			.SetFollowCamera()
			.SetFollowBeat(FollowBeatType.HeadbobStyle);
		float y = 99f;
		base.room0.camera.transform.LocalMoveY(y);
		RDClass.Vfx.rooms[1].camera.transform.LocalMoveY(y);
		RDClass.Vfx.rooms[2].camera.transform.LocalMoveY(y);
		if (dogMode)
		{
			rhythmDogtorBg = Background.Create(new string[3] { "rdog_a1", "rdog_a2", "rdog_a3" }, 6.6666665f, 0f, 0f, isForeground: true).SetTileType(TileType.None).SetBackgroundOrder(301)
				.SetRenderQueue(3000);
			byMorphiousBg = Background.Create(new string[3] { "rdog_b1", "rdog_b2", "rdog_b3" }, 6.6666665f, 0f, 0f, isForeground: true).SetTileType(TileType.None).SetBackgroundOrder(301)
				.SetRenderQueue(3000);
			rhythmDogtorBg.visible = true;
			byMorphiousBg.visible = false;
		}
		paigeFire = Object.Instantiate(base.gc.paigeFire);
		paigeFire.SetActive(value: false);
		base.game.handControllerOnTop.shouldShow = false;
		ToggleAllLamps(enable: false);
		yield return null;
	}

	public override void preactions()
	{
		switch (base.conductor.barNumber)
		{
		case 1:
		{
			base.rows[1].rowSound = "sndShaker";
			base.rows[2].rowSound = "sndHatTight";
			base.rows[3].rowSound = "sndStick";
			base.rows[4].rowSound = "sndPunch";
			base.rows[2].rowSoundVolMultiplier = 0.7f;
			base.rows[2].rowSoundPan = -0.5f;
			base.rows[3].rowSoundVolMultiplier = 0.7f;
			base.rows[3].rowSoundPan = 0.5f;
			RDGameSounds.Set(GameSoundType.ClapSoundP1Classic, "sndClapHit", 1f);
			RDGameSounds.Set(GameSoundType.ClapSoundP2Classic, "sndClapHitP2", 1f);
			base.conductor.SetBPM(100f);
			PlayMusic(songsUsed[0]);
			base.conductor.crotchetsPerBar = 8;
			Character[] array = ((!dogMode) ? new Character[4]
			{
				Character.Paige,
				Character.MrStevenson,
				Character.MrsStevenson,
				Character.Politician
			} : new Character[4]
			{
				Character.Adog,
				Character.MrStevendog,
				Character.MrsStevendog,
				Character.Purritician
			});
			base.game.MakeRow(RowType.Classic, 1, 1f, array[0], RDPlayer.P1, RDPlayer.P2);
			base.game.MakeRow(RowType.Classic, 2, 1f, array[1], RDPlayer.P1, RDPlayer.P1, -1, isShadow: false, 1);
			base.game.MakeRow(RowType.Classic, 3, 1f, array[2], RDPlayer.P1, RDPlayer.P1, -1, isShadow: false, 2);
			base.game.MakeRow(RowType.Classic, 4, 1f, array[3], RDPlayer.P1, RDPlayer.P2, -1, isShadow: false, 3);
			paigeRow = base.rows[1].ent;
			paige = base.rows[1].ent.character;
			mrStevenson = base.rows[2].ent.character;
			patient2 = base.rows[3].ent.character;
			politician = base.rows[4].ent.character;
			paigeFire.transform.SetParent(paige.transform);
			paigeFire.transform.localPosition = new Vector3(-4f, 0f, 0f);
			base.game.rooms[0].yOffset = -0.1f;
			base.game.rooms[1].yOffset = -0.09f;
			base.game.rooms[2].yOffset = -0.09f;
			base.game.rooms[2].firstRow.SetOverlayColor(Color.gray);
			lyrics1 = MakeLyrics("lesmis_lyrics_paige", multilineMode: false, 0);
			lyrics2 = MakeLyrics("lesmis_lyrics_mrStevenson", multilineMode: false, 1);
			lyrics3 = MakeLyrics("lesmis_lyrics_mrsStevenson", multilineMode: false, 2);
			lyrics4 = MakeLyrics("lesmis_lyrics_politician", multilineMode: false, 3);
			if (dogMode)
			{
				mrStevensonDummy = scrChar.Create(Character.MrStevendog);
			}
			else
			{
				mrStevensonDummy = scrChar.Create(Character.MrStevenson);
			}
			mrStevensonDummy.x = 637f;
			mrStevensonDummy.y = 67f;
			mrStevensonDummy.TweenOpacity(0f, 0.5f);
			mrStevensonDummy.shaderData.borderColor = Color.black.WithAlpha(0f);
			mrStevensonDummy.shaderData.outline = 1;
			mrStevensonDummy.PlayExpression("neutral");
			lyrics2convo = MakeLyrics("lesmis_lyrics_mrStevensonInterlude", multilineMode: false, -1, mrStevensonDummy.gameObject);
			lyrics2convo.y += 4f;
			lyrics1convo = MakeLyrics("lesmis_lyrics_paigeInterlude", multilineMode: false, -1, base.game.rooms[0].firstRow.character.gameObject);
			if (dogMode)
			{
				lyrics1convo.transform.localScale = new Vector3(-1f, 1f, 1f);
			}
			ianDummy = scrChar.Create(Character.Ian, "explaining");
			ianDummy.x = 950f;
			ianDummy.y = 74f;
			ianDummy.transform.localScale = new Vector3(-1f, 1f, 1f);
			ianDummy.gameObject.SetActive(value: false);
			if (!dogMode)
			{
				lyricsOutroDoctor1 = MakeLyrics("lesmis_lyrics_paigeOutro", multilineMode: false, -1, base.game.rooms[0].firstRow.character.gameObject, 0.8f);
				lyricsOutroDoctor2 = MakeLyrics("lesmis_lyrics_ianOutro", multilineMode: false, -1, ianDummy.gameObject, 0.8f);
				lyricsOutroDoctor1.gameObject.SetActive(value: false);
				lyricsOutroDoctor2.gameObject.SetActive(value: false);
				if (RDString.isChinese)
				{
					lyricsOutroDoctor1.offset = 182.13968f;
					lyricsOutroDoctor2.offset = 182.13968f;
				}
				lyricsCoda = MakeLyrics("lesmis_lyrics_paigeCoda", multilineMode: true);
				lyricsCoda.SetupFollowCam(0);
				lyricsCoda.name = "Coda Lyrics";
				lyricsCoda.fadeTime = 3600f;
				lyricsCoda.GetComponent<RectTransform>().localPosition = new Vector3(68.5f, 47f, 1f);
			}
			cameraContainer0 = base.room0.cameraContainer;
			cameraContainer1 = base.room1.cameraContainer;
			cameraContainer2 = base.room2.cameraContainer;
			doctor1RowContainer = base.room0.rowContainer;
			break;
		}
		case 2:
			if (!dogMode)
			{
				mBossStageText.gameObject.SetActive(value: true);
				mBossStageText.Play();
			}
			if (Narration.IsEnabled)
			{
				base.conductor.PlayBeatInCrotchets("sndPatientConnect", 7f, 1f, RDUtils.GetMixerGroup("CuesoundsParent"), 1f, base.rows[2].rowSoundPanAdjustedFor2P);
			}
			break;
		case 3:
			base.game.OnBeatClassic(2, 2, 1f, 0.5f, 1, SwingBeatType.SwingSimpleThird);
			break;
		case 6:
			if (Narration.IsEnabled)
			{
				base.conductor.PlayBeatInCrotchets("sndPatientConnect", 7f, 1f, RDUtils.GetMixerGroup("CuesoundsParent"), 1f, base.rows[1].rowSoundPanAdjustedFor2P);
			}
			break;
		case 7:
			base.game.OffBeat(2);
			base.game.OnBeatClassic(11, 1, 1f, 1f, 1, SwingBeatType.SwingSimpleCustom, offOtherBeats: false, 1.3333334f);
			break;
		case 9:
			base.game.OffBeat(11);
			base.game.OnBeatClassic(1, 1, 1f, 0.5f, 1, SwingBeatType.SwingSimpleThird);
			break;
		case 11:
			base.game.AddBeat(2, 4f, 0.5f, SwingBeatType.SwingSimpleThird);
			break;
		case 13:
			base.game.OffBeat(1);
			base.game.OnBeat(11);
			base.game.OnBeatClassic(12, 2, 1f, 1f, 1, SwingBeatType.SwingSimpleCustom, offOtherBeats: false, 1.3333334f);
			break;
		case 15:
			base.game.OffAllBeatLoops();
			break;
		case 22:
		{
			if (Narration.IsEnabled)
			{
				base.conductor.PlayBeatInCrotchets("sndPatientConnect", 0f, 1f, RDUtils.GetMixerGroup("CuesoundsParent"), 1f, base.rows[3].rowSoundPanAdjustedFor2P);
			}
			float offset2 = 0f;
			lyrics1.offset = offset2;
			lyrics2.offset = offset2;
			lyrics3.offset = offset2;
			lyrics4.offset = offset2;
			base.game.AddBeat(3, 1f, 1f, SwingBeatType.SwingSimpleCustom, 1.3333334f);
			RDGameSounds.Set(GameSoundType.ClapSoundP1Classic, "sndClapHit", 1f);
			RDGameSounds.Set(GameSoundType.ClapSoundP2Classic, "sndClapHitP2", 1f);
			base.rows[1].rowSoundVolMultiplier = 1.4f;
			base.rows[2].rowSoundVolMultiplier = 1.2f;
			base.rows[3].rowSoundVolMultiplier = 1f;
			base.rows[4].rowSoundVolMultiplier = 1.6f;
			base.game.AddBeat(1, 5f, 0.5f, SwingBeatType.SwingSimpleThird);
			break;
		}
		case 23:
			base.game.AddBeat(3, 1f, 0.5f, SwingBeatType.SwingSimpleThird);
			base.game.AddBeat(1, 5f, 0.5f, SwingBeatType.SwingSimpleThird);
			break;
		case 24:
			base.game.AddBeat(3, 1f, 0.5f, SwingBeatType.SwingSimpleThird);
			base.game.AddBeat(1, 4f, 1f / 3f);
			base.game.AddBeat(2, 6f, 0.5f, SwingBeatType.SwingCustomReverse, 1f / 3f);
			break;
		case 25:
			base.game.AddBeat(3, 1f, 0.5f, SwingBeatType.SwingSimpleThird);
			base.game.AddBeat(1, 3.75f, 0.25f);
			if (Narration.IsEnabled)
			{
				base.conductor.PlayBeatInCrotchets("sndPatientConnect", 7f, 1f, RDUtils.GetMixerGroup("CuesoundsParent"), 1f, base.rows[4].rowSoundPanAdjustedFor2P);
			}
			break;
		case 26:
			base.game.AddBeat(4, 0f, 1f / 3f);
			base.game.AddBeat(4, 4f, 1f / 3f);
			base.game.AddBeat(3, 1f, 0.5f, SwingBeatType.SwingSimpleThird);
			base.game.AddBeat(1, 7.1f, 0.288f);
			break;
		case 27:
			base.game.AddBeat(4, 3f, 1f / 3f);
			base.game.AddBeat(3, 4f, 1f, SwingBeatType.SwingSimpleCustom, 1.3333334f);
			base.game.AddBeat(2, 4f, 1f, SwingBeatType.SwingSimpleCustom, 1.3333334f);
			break;
		case 28:
			base.game.AddBeat(4, 1f, 1f / 3f);
			base.game.AddBeat(1, 5f, 0.5f, SwingBeatType.SwingSimpleThird);
			break;
		case 29:
			base.game.AddBeat(4, 1f, 1f / 3f);
			base.game.AddBeat(3, 4f, 1f, SwingBeatType.SwingSimpleCustom, 1.3333334f);
			base.game.AddBeat(2, 4f, 1f, SwingBeatType.SwingSimpleCustom, 1.3333334f);
			base.game.AddBeat(1, 5f, 0.5f, SwingBeatType.SwingSimpleThird);
			break;
		case 30:
			base.game.AddBeat(4, 1f, 1f / 3f);
			base.game.AddBeat(1, 4f, 2f / 3f);
			break;
		case 31:
			base.conductor.crotchetsPerBar = 7;
			break;
		case 32:
		{
			base.conductor.crotchetsPerBar = 56;
			if (dogMode)
			{
				SetEnding(didPlayerWin: true);
				base.conductor.StopSong();
				base.conductor.barNumber = 900;
				base.conductor.crotchetsPerBar = 4;
				break;
			}
			if (base.game.mistakesManager.mistakes > rankLowerBounds[3])
			{
				SetEnding(didPlayerWin: false);
				base.conductor.StopSong();
				lyricsCoda.gameObject.SetActive(value: false);
				base.conductor.barNumber = 900;
				base.conductor.crotchetsPerBar = 4;
				break;
			}
			mainWard0.gameObject.SetActive(value: false);
			scrExecuteOnEveryHB.Clear();
			base.rows[1].rowSound = "sndRide2";
			base.game.SetPlayerHitSounds(RowType.Classic, "sndClapHitEcho", "sndClapHitEchoP2");
			base.rows[1].rowSoundVolMultiplier = 0.7f;
			float offset = 0f;
			base.game.AddBeatFree(1, new float[7] { 149.1135f, 149.3457f, 149.6244f, 150.6693f, 150.7389f, 150.9943f, 151.2962f }, playerDrives7thBeat: false, _isLenientMargins: true, offset);
			base.game.AddBeatFree(1, new float[7] { 152.3922f, 152.619f, 152.8638f, 153.0727f, 153.6997f, 153.9551f, 154.2105f }, playerDrives7thBeat: false, _isLenientMargins: true, offset);
			base.game.AddBeatFree(1, new float[7] { 155.0288f, 155.3146f, 155.6164f, 155.8486f, 156.5059f, 156.7381f, 157.0167f }, playerDrives7thBeat: false, _isLenientMargins: true, offset);
			base.game.AddBeatFree(1, new float[7] { 157.8991f, 158.1313f, 158.4332f, 158.6653f, 159.2923f, 159.4316f, 159.7103f }, playerDrives7thBeat: false, _isLenientMargins: true, offset);
			base.game.AddBeatFree(1, new float[7] { 163.3357f, 163.6014f, 163.8582f, 164.707f, 164.9682f, 165.421f, 165.7867f }, playerDrives7thBeat: false, _isLenientMargins: true, offset);
			base.game.AddBeatFree(1, new float[7] { 166.7142f, 166.9754f, 167.3541f, 167.5282f, 168.3642f, 168.5557f, 168.7995f }, playerDrives7thBeat: false, _isLenientMargins: true, offset);
			base.game.AddBeatFree(1, new float[7] { 170.5326f, 170.8286f, 170.968f, 171.264f, 172.3218f, 172.496f, 172.7572f }, playerDrives7thBeat: false, _isLenientMargins: true, offset);
			finalBeat = base.game.AddBeatFree(1, new float[7] { 175.4311f, 176.1652f, 177.0405f, 177.5281f, 178.798f, 179.4392f, 181.4392f }, playerDrives7thBeat: true, _isLenientMargins: true, offset);
			finalBeat.SetHeartExplosion(base.conductor.audioPos + 99999.0);
			finalBeat.SetOnDestroy(() =>
			{
				base.conductor.SetPlayStyle(PlayStyleChange.Immediately);
				hospitalShadow.transform.localPosition = hospitalShadow.transform.localPosition.WithY(100f);
			});
			base.conductor.Freeze(PlayStyleChange.Prolong);
			break;
		}
		case 33:
			base.conductor.crotchetsPerBar = 8;
			PlayMusic(songsUsed[1]);
			break;
		case 34:
			base.conductor.crotchetsPerBar = 999;
			break;
		case 4:
		case 5:
		case 8:
		case 10:
		case 12:
		case 14:
		case 16:
		case 17:
		case 18:
		case 19:
		case 20:
		case 21:
			break;
		}
	}

	public override void actions()
	{
		int num = base.conductor.barNumber;
		switch (num)
		{
		case 1:
			RDClass.Vfx.SetSpriteMode(SpriteMode.Outline, Color.black);
			RDClass.Vfx.roomsAnimationDuration = 0f;
			RDClass.Vfx.ShowRooms("1,0", new float[2] { 0f, 1f });
			if (!dogMode)
			{
				RDClass.Vfx.roomsAnimationDuration = 1f;
				RDClass.Vfx.ShowRooms("1,0", new float[2] { 0.5f, 0.5f });
			}
			base.room1.rowContainer.localPosition = new Vector3(633f, -26f);
			base.room1.SetRowContainerY(-26f, 198f);
			base.room2.SetRowContainerY(-26f, 198f);
			base.room1.backgroundContainerWorldspace.localPosition = new Vector3(0f, 33f);
			base.room2.backgroundContainerWorldspace.localPosition = new Vector3(0f, 33f);
			cameraContainer1.localPosition = new Vector3(531f, 0f, -200f);
			cameraContainer1.DOLocalMoveX(631f, 10f, snapping: true);
			cameraContainer2.localPosition = new Vector3(0f, 0f, -200f);
			base.game.handControllerOnTop.Show(show: false);
			NarrateDescription("Lesmis.d1");
			break;
		case 2:
			if (dogMode)
			{
				rhythmDogtorBg.visible = false;
				byMorphiousBg.visible = true;
			}
			if (dogMode)
			{
				RDClass.Vfx.ShowRooms("1,0", base.conductor.crotchet * 4f, new float[2] { 0.5f, 0.5f });
				RDClass.Vfx.roomsAnimationDuration = 1f;
			}
			scrExecuteOnCertainBeat.Add(() =>
			{
				Narration.NarrateRowInfo(base.rows[2], NarrateInfoType.Connect, soundOnly: true, narrateSkipBeats: false, null, skipsUnstable: false, RDPlayer.AutoDetect, -1);
			}, 7f);
			break;
		case 3:
			if (dogMode)
			{
				byMorphiousBg.FadeOut(base.conductor.crotchet);
			}
			base.game.handControllerOnTop.gameObject.SetActive(value: false);
			base.game.gameState = GameState.Handmode;
			break;
		case 6:
			scrExecuteOnCertainBeat.Add(() =>
			{
				Narration.NarrateRowInfo(base.rows[1], NarrateInfoType.Connect, soundOnly: true, narrateSkipBeats: false, null, skipsUnstable: false, RDPlayer.AutoDetect, -1);
			}, 7f);
			break;
		case 7:
			base.rows[1].ent.character.happyAnimName = "";
			base.room0.backgroundContainerWorldspace.localPosition = new Vector3(0f, 33.18f);
			doctor1RowContainer.localPosition = new Vector3(1503f, -27f);
			base.room0.SetRowContainerY(-27f, 198f);
			doctor1RowContainer.localScale = new Vector3(-1f, 1f, 1f);
			RDClass.Vfx.FadeIn(2f, 0);
			RDClass.Vfx.FadeOut(2f, 1);
			cameraContainer0.localPosition = new Vector3(1247f, 0f, -200f);
			cameraContainer0.DOLocalMoveX(1163f, 10f, snapping: true);
			scrExecuteOnCertainBeat.Add(() =>
			{
				NarrateDescription("Lesmis.d2");
			}, 5f);
			break;
		case 9:
			paige.PlayExpression("singing");
			paige.neutralAnimName = "singing";
			paige.missedAnimName = "missed";
			paige.barelyAnimName = "missed";
			paige.happyAnimName = "happy";
			break;
		case 11:
			RDClass.Vfx.FadeIn(2f, 1, 4f);
			RDClass.Vfx.FadeOut(2f, 1, 6f);
			break;
		case 12:
			scrExecuteOnCertainBeat.Add(() =>
			{
				NarrateDescription("Lesmis.d3");
			}, 6.5f);
			break;
		case 13:
		{
			RDClass.Vfx.FadeIn(2f, 1, 1f);
			if (!dogMode)
			{
				paige.transform.localScale = new Vector3(-1f, 1f, 1f);
			}
			paige.PlayExpression("walking");
			paige.neutralAnimName = "walking";
			paige.missedAnimName = "";
			paige.happyAnimName = "";
			paige.barelyAnimName = "";
			float num5 = 662f;
			float duration = 12f;
			float num6 = 30f;
			doctor1RowContainer.transform.DOLocalMoveX(1503f - num5, duration);
			cameraContainer0.DOLocalMoveX(1163f - num5 + num6, duration);
			break;
		}
		case 15:
			alwaysSkippableCutscene = true;
			base.game.gameState = GameState.CutsceneSkippable;
			RDClass.Vfx.ShowRooms("1d,0");
			paige.PlayExpression("conversing");
			mrStevensonDummy.TweenOpacity(1f, 0.5f);
			mrStevensonDummy.TweenColor(Color.black, Color.clear, 0.5f);
			base.game.rooms[0].firstRow.FadeOutRowExceptChar(1f, 0.5f);
			RDClass.Vfx.DarkenBackgrounds(0, 2f);
			TweenForegroundColor("A9A9A9FF".HexToColor(), 2f);
			RDClass.Vfx.SetHorizontalStripsVisible(visible: false);
			break;
		case 22:
		{
			RDClass.Vfx.SetHorizontalStripsVisible(visible: true);
			float num4 = 0.5f;
			base.rows[1].ent.character.PlayExpression("neutral", disableOverride: true);
			mrStevensonDummy.TweenOpacity(0f, num4);
			mrStevensonDummy.TweenColor(Color.clear, Color.clear, num4);
			base.game.gameState = GameState.Handmode;
			TweenForegroundColor(Color.white, num4);
			RDClass.Vfx.LightenBackgrounds(0, num4);
			SetWardAlpha(mainWard2, 0f);
			base.game.rooms[0].firstRow.FadeInRow(num4);
			RDClass.Vfx.ShowRooms("2,0");
			if (!dogMode)
			{
				patient2.keepAnimation = true;
				patient2.PlayExpression("lyingdown");
				patient2.neutralAnimName = "gettingup";
				patient2.happyAnimName = "gettingup";
				patient2.barelyAnimName = "gettingup";
				patient2.missedAnimName = "gettingup";
				patient2.PlayExpression("lyingdown");
			}
			cameraContainer0.DOKill(complete: true);
			cameraContainer0.DOLocalMoveX(501f, 3f);
			scrExecuteOnCertainBeat.Add(() =>
			{
				Narration.NarrateRowInfo(base.rows[3], NarrateInfoType.Connect, soundOnly: true, narrateSkipBeats: false, null, skipsUnstable: false, RDPlayer.AutoDetect, -1);
			}, 0f);
			scrExecuteOnCertainBeat.Add(() =>
			{
				NarrateDescription("Lesmis.d4");
			}, 0.5f);
			break;
		}
		case 23:
			base.game.rooms[2].firstRow.shaderDataSource = null;
			base.game.rooms[2].firstRow.FlashChildren();
			RDClass.Vfx.Flash(2);
			SetWardAlpha(mainWard2, 1f);
			RDClass.Vfx.SetSpriteMode(SpriteMode.Outline, Color.black);
			RDClass.Vfx.roomsAnimationDuration = 1f;
			patient2.keepAnimation = false;
			patient2.missedAnimName = "missed";
			patient2.happyAnimName = "happy";
			patient2.barelyAnimName = "barely";
			patient2.missedAnimName = "missed";
			if (!dogMode)
			{
				patient2.PlayExpression("gettingup");
			}
			paige.neutralAnimName = "walking";
			paige.missedAnimName = "missed";
			paige.happyAnimName = "happywalking";
			paige.barelyAnimName = "missed";
			paige.PlayExpression("walking");
			scrExecuteOnHit.Add(() =>
			{
				Trip();
			}, HitType.AnyMiss, persistent: true, autoDelete: false, 1);
			scrExecuteOnEveryHB.Add(() =>
			{
				RDClass.Vfx.ShakeCam(2, 2);
			}, 1);
			scrExecuteOnEveryHB.Add(() =>
			{
				RDClass.Vfx.ShakeCam(3, 2, 3);
			}, 4);
			scrExecuteOnUpdate.Add(() =>
			{
				Update2();
			});
			mainWard0.legacyScroll = true;
			scrExecuteOnCertainBeat.Add(() =>
			{
				NarrateDescription("Lesmis.d5");
			}, 7.5f);
			runningSpeed = 180f;
			break;
		case 24:
			foreach (Transform item in base.room0.backgroundContainerWorldspace)
			{
				item.gameObject.SetActive(value: false);
			}
			RDClass.Vfx.ShowRooms("2d,1,0", 6f);
			break;
		case 25:
			RDClass.Vfx.ShowRooms("2,1d,0", 1f);
			scrExecuteOnCertainBeat.Add(() =>
			{
				Narration.NarrateRowInfo(base.rows[4], NarrateInfoType.Connect, soundOnly: true, narrateSkipBeats: false, null, skipsUnstable: false, RDPlayer.AutoDetect, -1);
			}, 7f);
			scrExecuteOnCertainBeat.Add(() =>
			{
				NarrateDescription("Lesmis.d6");
			}, 7.5f);
			break;
		case 26:
			RDClass.Vfx.ShowRooms("2,0d,3");
			RDClass.Vfx.ShowRooms("2,0,3d", 6.2f);
			break;
		case 27:
			runningSpeed = 240f;
			scrExecuteOnEveryHB.Add(() =>
			{
				RDClass.Vfx.ShakeCam(2, 3, 1);
			}, 2);
			scrExecuteOnEveryHB.Add(() =>
			{
				RDClass.Vfx.ShakeCam(2, 3, 2);
			}, 3);
			RDClass.Vfx.ShowRooms("2,1,0d,3", 2.8f, new float[4] { 0.25f, 0.25f, 0f, 0.5f });
			break;
		case 28:
			RDClass.Vfx.ShowRooms("2,1,0,3d", 3.2f, new float[4] { 0.25f, 0.25f, 0.5f, 0f });
			RDClass.Vfx.ShowRooms("2d,1d,0", 5f);
			base.room0.camera.transform.localPosition = base.room0.camera.transform.localPosition.WithY(99f);
			scrExecuteOnCertainBeat.Add(() =>
			{
				paige.PlayExpression("needsomehelp", disableOverride: true, ignoreKeep: true);
				if (!dogMode)
				{
					paigeFire.SetActive(value: true);
				}
				RDShaderProperties shaderData2 = paige.shaderData;
				paige.shaderDataSource = paige;
				shaderData2.borderColor = "EADB10FF".HexToColor().WithAlpha(0f);
				shaderData2.outline = 0;
				DOTween.ToAlpha(() => shaderData2.borderColor, (Color x) =>
				{
					shaderData2.borderColor = x;
				}, 1f, 1f);
				scrExecuteOnHit.Clear();
				DOTween.To(() => runningSpeed, (float x) =>
				{
					runningSpeed = x;
				}, 0f, 1f);
				scrExecuteOnCertainBeat.Add(() =>
				{
					bgHelpSpotlight.FadeIn();
					TweenForegroundColor("BDBDBDFF".HexToColor(), 1f);
				}, 0.1f);
			}, 5f);
			base.rows[1].ent.character.keepAnimation = true;
			break;
		case 29:
			RDClass.Vfx.ShakeCam(40, 3);
			RDClass.Vfx.Flash(0);
			RDClass.Vfx.ShowRooms("0,3", 0.5f);
			RDClass.Vfx.ShowRooms("2,1,0,3", 3.2f);
			break;
		case 30:
			paigeFire.SetActive(value: false);
			RDClass.Vfx.FadeOut(0.4f, 1, 4f);
			RDClass.Vfx.FadeOut(0.4f, 2, 4.6666665f);
			RDClass.Vfx.FadeOut(0.4f, 3, 5.3333335f);
			scrExecuteOnCertainBeat.Add(() =>
			{
				lyrics2.gameObject.SetActive(value: false);
				lyrics3.gameObject.SetActive(value: false);
				lyrics4.gameObject.SetActive(value: false);
			}, 4f);
			RDClass.Vfx.ShowRooms("2d,1d,0,3d", 6f);
			RDClass.Vfx.Flash(0, 8f);
			scrExecuteOnCertainBeat.Add(() =>
			{
				if (base.game.mistakesManager.mistakes <= rankLowerBounds[3])
				{
					RDClass.Vfx.FadeOut(0f, 0);
					NarrateDescription("Lesmis.d7");
					bgHelpSpotlight.SetAlpha(0f);
				}
				else
				{
					RDShaderProperties shaderData2 = base.rowEnt1.character.shaderData;
					DOTween.ToAlpha(() => shaderData2.borderColor, (Color x) =>
					{
						shaderData2.borderColor = x;
					}, 0f, 0.7f);
					if (!dogMode)
					{
						NarrateDescription("Lesmis.GameOver");
						base.rowEnt1.character.PlayExpression("gameover", disableOverride: false, ignoreKeep: true);
					}
				}
			}, 9f);
			break;
		case 32:
		{
			DOTween.timeScale = 1f;
			mainWard0.gameObject.SetActive(value: false);
			RDShaderProperties shaderData = base.rowEnt1.character.shaderData;
			shaderData.outline = 1;
			shaderData.glowPower = 0f;
			paige.shaderData.borderColor = Color.black;
			cameraContainer0.transform.DOLocalMoveX(540f, 0f, snapping: true);
			doctor1RowContainer.localScale = Vector3.one;
			doctor1RowContainer.localPosition = new Vector3(545f, -66f);
			base.room0.backgroundOverlay.sprite.color = Color.black;
			RDClass.Vfx.scaleBackgroundsAndRows = true;
			Transform transform = base.room0.camera.transform;
			transform.localPosition = transform.localPosition.WithY(RDClass.Vfx.RDHeight / 2f);
			hospitalBackgroundBg = Background.CreateSingle("Hospital_Background_New").SetBackgroundOrder(20);
			noBananaBeats = true;
			GameObject gameObject = new GameObject("Hospital_Shadow");
			gameObject.transform.SetParent(hospitalBackgroundBg.transform);
			hospitalShadow = gameObject.AddComponent<tk2dSprite>();
			hospitalShadow.SetSprite(RDClass.Vfx.scBackground, "WhitePixel");
			hospitalShadow.GetComponent<Renderer>().sortingLayerName = "Background";
			hospitalShadow.SortingOrder = 21;
			hospitalShadow.color = Color.clear;
			hospitalShadow.scale = new Vector3(400f, 200f, 1f);
			RDClass.Vfx.scaleBackgroundsAndRows = false;
			RDClass.Vfx.FadeIn(1f, 0);
			RDClass.Vfx.DarkenBackgrounds();
			NarrateDescription("Lesmis.d8");
			scrExecuteOnCertainBeat.Add(() =>
			{
				RDClass.Vfx.SpotlightFocusIn(base.game.rooms[0].firstRow.character.transform.position.x, animated: false, brightMode: true);
			}, 0.01f);
			base.rows[1].ent.character.PlayExpression("walking", disableOverride: false, ignoreKeep: true);
			for (int num3 = 0; num3 < 5; num3++)
			{
				scrExecuteOnCertainTime.Add(() =>
				{
					PlayHurryUp();
				}, 188f + (float)num3 * 7f, isFromSongStart: true, divideBySpeed: true);
			}
			scrExecuteOnCertainTime.Add(() =>
			{
				PoliticianButtonPress();
			}, 223.0, isFromSongStart: true, divideBySpeed: true);
			break;
		}
		case 900:
			scrExecuteOnCertainTime.Add(() =>
			{
				base.rankscreen.AdvanceGameover();
				base.rankscreen.AdvanceGameover();
			}, 38.5, isFromSongStart: true, divideBySpeed: true);
			break;
		case 901:
			base.rankscreen.AdvanceGameover();
			break;
		case 902:
			base.rankscreen.AdvanceGameover();
			break;
		case 33:
		{
			scrExecuteOnCertainTime.Clear();
			scrExecuteOnCertainBeat.Add(() =>
			{
				paigeRow.heart.Explode();
			}, 1f);
			scrExecuteOnCertainBeat.Add(() =>
			{
				NarrateDescription("Lesmis.d9");
			}, 2f);
			scrExecuteOnCertainBeat.Add(() =>
			{
				base.game.gameState = GameState.Cutscene;
				base.game.ShowCutsceneBars(show: true);
			}, 3f);
			lyricsCoda.enabled = false;
			string LongText = RDString.Get("level.Lesmis.paigeCodaFinalWord");
			Narration.Say(LongText, NarrationCategory.Subtitles, false);
			for (int num2 = 0; num2 < LongText.Length; num2++)
			{
				int m = num2;
				scrExecuteOnCertainBeat.Add(() =>
				{
					lyricsCoda.text += LongText[m];
				}, 1f + 1f / 3f * (float)num2);
			}
			scrExecuteOnCertainBeat.Add(() =>
			{
				RDClass.Vfx.FadeOut(1f);
			}, 2.666f);
			scrExecuteOnCertainBeat.Add(() =>
			{
				foreach (Transform item2 in base.room0.backgroundContainerWorldspace.transform)
				{
					item2.gameObject.SetActive(value: true);
				}
				RDClass.Vfx.SetHorizontalStripsVisible(visible: false);
				mainWard0.gameObject.SetActive(value: true);
				mainWard0.transform.LocalMoveX(1900f);
				RDClass.Vfx.FadeIn(1f);
				Transform transform2 = base.room0.camera.transform;
				transform2.localPosition = transform2.localPosition.WithY(99f);
				printe("we are on outro");
				hospitalBackgroundBg.gameObject.SetActive(value: false);
				base.game.rooms[0].firstRow.FadeOutRowExceptChar(1f, 0f);
				RDClass.Vfx.DarkenBackgrounds(0, 1f);
				TweenForegroundColor("AEAEAEFF".HexToColor(), 1f);
				RDClass.Vfx.SpotlightOpenAllTheWay(0f, playSwitchSound: false, RDUtils.GetMixerGroup("FeedbackSoundsParent"));
				ToggleAllLamps(enable: true);
				doctor1RowContainer.localPosition = new Vector3(623f, -23f, 0f);
				lyricsOutroDoctor1.transform.localScale = new Vector3(-1f, 1f, 1f);
				ianDummy.gameObject.SetActive(value: true);
				lyricsOutroDoctor1.gameObject.SetActive(value: true);
				lyricsOutroDoctor2.gameObject.SetActive(value: true);
				lyricsOutroDoctor1.gameObject.SetActive(value: true);
				lyricsOutroDoctor2.gameObject.SetActive(value: true);
				mrStevensonDummy.x = -50f;
				mrStevensonDummy.TweenOpacity(1f, 0f);
				mrStevensonDummy.TweenColor(Color.black, Color.clear, 0f);
				mrStevensonDummy.PlayExpression("sad");
				if (Persistence.GetLevelRank(Level.Lesmis).passed)
				{
					skippable = true;
				}
				LyricsGame[] array = new LyricsGame[7] { lyrics1, lyrics2, lyrics3, lyrics4, lyrics1convo, lyrics2convo, lyricsCoda };
				for (int i = 0; i < array.Length; i++)
				{
					array[i].gameObject.SetActive(value: false);
				}
				ianDummy.PlayExpression("running");
				ianDummy.transform.DOLocalMoveX(776f, 3f).SetEase(Ease.Linear).OnComplete(() =>
				{
					ianDummy.PlayExpression("explaining");
				});
				for (float num7 = 2f; num7 < 4.5f; num7 += 0.333f)
				{
					scrExecuteOnCertainBeat.Add(() =>
					{
						Smoke.Add(ianDummy.xGlobal, ianDummy.yGlobal - 13f, Vector2.one, 100);
					}, num7);
				}
				scrExecuteOnCertainBeat.Add(() =>
				{
					paige.PlayExpression("neutral", disableOverride: true, ignoreKeep: true);
				}, 9f);
				scrExecuteOnCertainBeat.Add(() =>
				{
					ianDummy.PlayExpression("sighing");
				}, 21f);
				scrExecuteOnCertainBeat.Add(() =>
				{
					cameraContainer0.transform.DOLocalMoveX(-106f, 15f).SetEase(Ease.InOutQuad);
				}, 32f);
				scrExecuteOnCertainBeat.Add(() =>
				{
					TurnOffAutoNarration();
				}, 43f);
				scrExecuteOnCertainBeat.Add(() =>
				{
					NarrateDescription("Lesmis.d10");
				}, 45f);
				scrExecuteOnCertainBeat.Add(() =>
				{
					lyricsOutroDoctor1.gameObject.SetActive(value: false);
				}, 63f);
				scrExecuteOnCertainBeat.Add(() =>
				{
					RDClass.Vfx.FadeOut(4f);
				}, 70f);
			}, 3.666f);
			scrExecuteOnCertainTime.Add(() =>
			{
				paige.PlayExpression("neutral", disableOverride: true, ignoreKeep: true);
			}, 1.5, isFromSongStart: true, divideBySpeed: true);
			scrExecuteOnCertainTime.Add(() =>
			{
				paige.PlayExpression("asking", disableOverride: true, ignoreKeep: true);
			}, 3.5, isFromSongStart: true, divideBySpeed: true);
			scrExecuteOnCertainTime.Add(() =>
			{
				ianDummy.PlayExpression("explainingstop", disableOverride: true, ignoreKeep: true);
			}, 11.5, isFromSongStart: true, divideBySpeed: true);
			scrExecuteOnCertainTime.Add(() =>
			{
				paige.PlayExpression("conversing", disableOverride: true, ignoreKeep: true);
			}, 26.899999618530273, isFromSongStart: true, divideBySpeed: true);
			SetEnding(didPlayerWin: true);
			scrExecuteOnCertainTime.Add(() =>
			{
				base.rankscreen.AdvanceGameover();
				base.rankscreen.AdvanceGameover();
			}, 38.5, isFromSongStart: true, divideBySpeed: true);
			scrExecuteOnCertainTime.Add(() =>
			{
				base.rankscreen.AdvanceGameover();
			}, 43.5, isFromSongStart: true, divideBySpeed: true);
			scrExecuteOnCertainTime.Add(() =>
			{
				base.rankscreen.AdvanceGameover();
			}, 49.5, isFromSongStart: true, divideBySpeed: true);
			break;
		}
		}
	}

	private void SetWardAlpha(RDMainWard ward, float alpha)
	{
		Color color = Color.Lerp(Color.black, Color.white, alpha);
		foreach (Transform item in ward.leftEnd.transform)
		{
			if (item.TryGetComponent<tk2dSprite>(out var component))
			{
				component.color = color;
			}
		}
		foreach (Transform item2 in ward.seamless.transform)
		{
			if (item2.TryGetComponent<tk2dSprite>(out var component2))
			{
				component2.color = color;
			}
		}
	}

	private void PlayHurryUp()
	{
		RDClass.Vfx.ShowRooms("0,3", new float[2] { 0.75f, 0.25f });
		RDClass.Vfx.FadeIn(0f, 3);
		scrConductor.PlayImmediately("sndPoliticianHurryUp" + mHurryUpIndex, 1f, null, RDTime.speed);
		politician.PlayExpression("happy");
		char separator = '\n';
		string linesString = "/" + RDString.Get("level.Lesmis.politicianHurryUp" + mHurryUpIndex);
		string timesString = "0.0" + separator + string.Join(separator, politicianTimings[mHurryUpIndex]);
		lyrics4.gameObject.SetActive(value: true);
		lyrics4.Reset();
		lyrics4.ResetSnapshots();
		lyrics4.LoadLinesAndTimes(linesString, timesString, multilineMode: false);
		lyrics4.AdvanceText();
		lyrics4.transform.position = lyrics4.transform.position.AddY(-10f);
		lyrics4.offset = (0f - base.conductor.songFileTimeVisual) * RDTime.speed;
		mHurryUpIndex = (mHurryUpIndex + 1) % 5;
		DOVirtual.DelayedCall(3f, () =>
		{
			RDClass.Vfx.ShowRooms("0,3d");
		});
	}

	private void PoliticianButtonPress()
	{
		scrHandController hc = base.game.handControllerOnTop;
		hc.gameObject.SetActive(value: true);
		hc.ToggleAlignWithRoomComp(enableAlign: true);
		hc.Show(show: true, instant: true);
		hc.UpdateHandsPositions(HandUpdateType.LeftHand);
		hc.SetHandsInFront(inFront: true);
		hc.leftArm.SetSkin(base.gc.armSkin_politician);
		hc.leftArm.SetDefibMode(Persistence.GetDefibrillatorP1());
		DOVirtual.DelayedCall(0.75f, () =>
		{
			base.game.rows[1].playerBox.SpaceBarEvent(CPUTriggered: false);
			base.game.hitStripManager.ShowHitstrip(RDPlayer.P1);
			hc.leftArm.Press();
		});
	}

	private void ToggleAllLamps(bool enable)
	{
		mainWard0.ToggleLamps(enable);
		mainWard1.ToggleLamps(enable);
		mainWard2.ToggleLamps(enable);
	}

	public void Update2()
	{
		if (tripRemainingTime > 0f)
		{
			tripRemainingTime -= Time.deltaTime;
			if (tripRemainingTime <= 0f && tripTween != null)
			{
				tripTween.Kill();
			}
		}
		if (tripRemainingTime <= 0f)
		{
			runningSpeedMultiplier = Mathf.Clamp01(runningSpeedMultiplier + Time.deltaTime * 4f);
			if (runningSpeedMultiplier >= 1f && paige.IsCurrentExpression("missed"))
			{
				paige.PlayExpression(paige.neutralAnimName, disableOverride: false, ignoreKeep: true);
			}
		}
	}

	private void Trip()
	{
		if (tripTween != null)
		{
			tripTween.Kill();
		}
		tripTween = DOTween.To(() => runningSpeedMultiplier, (float x) =>
		{
			runningSpeedMultiplier = x;
		}, 0f, 1f);
		tripRemainingTime = 2f;
	}

	private void TweenForegroundColor(Color color, float duration)
	{
		DOTween.To(() => mainWard0.foregroundTintColor, (Color x) =>
		{
			mainWard0.foregroundTintColor = x;
		}, color, duration).SetUpdate(isIndependentUpdate: true);
		DOTween.To(() => mainWard1.foregroundTintColor, (Color x) =>
		{
			mainWard1.foregroundTintColor = x;
		}, color, duration).SetUpdate(isIndependentUpdate: true);
		DOTween.To(() => mainWard2.foregroundTintColor, (Color x) =>
		{
			mainWard2.foregroundTintColor = x;
		}, color, duration).SetUpdate(isIndependentUpdate: true);
	}

	public LyricsGame MakeLyrics(string filename, bool multilineMode, int roomIndex = -1, GameObject parent = null, float offset = 0f)
	{
		if (dogMode)
		{
			filename = filename.Replace("lesmis_", "lesmis_dog_");
		}
		else if (RDString.isChinese)
		{
			filename = "Chinese/" + filename;
		}
		LyricsGame lyricsGame = LyricsGame.Create(parent).Setup(filename, multilineMode, roomIndex, parent, offset);
		madeLyrics.Add(lyricsGame);
		lyricsGame.autoNarration = true;
		foreach (LyricsGame madeLyric in madeLyrics)
		{
			madeLyric.syncedLyrics = madeLyrics;
		}
		return lyricsGame;
	}

	private void TurnOffAutoNarration()
	{
		foreach (LyricsGame madeLyric in madeLyrics)
		{
			madeLyric.autoNarration = false;
		}
	}

	public void SetEnding(bool didPlayerWin)
	{
		if (!didPlayerWin)
		{
			failedLevel = true;
			customGameoverHeader = RDString.Get("rankscreen.levelFailed");
			customGameoverDescription = RDString.Get("lesmis.tryAgain");
		}
	}

	public void SkipEnding()
	{
		SetEnding(didPlayerWin: true);
		cameraContainer0.transform.DOKill();
		cameraContainer0.transform.DOLocalMoveX(-106f, 0f);
	}
}
