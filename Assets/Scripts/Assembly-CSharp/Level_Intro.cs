using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Level_Intro : LevelBase
{
	public enum LevelPhase
	{
		TitleScreen = 0,
		FirstDialogue = 1,
		Tutorial = 2,
		AfterTutorial = 3,
		OpeningCredits = 4
	}

	private scrChar farmer;

	private scrChar farmer_reflection;

	private scrChar drIan;

	private scrChar drIan_reflection;

	private Background backgroundNotes;

	private LevelPhase levelPhase;

	private bool helpgiven;

	private int firstBarOnTutorial = -1;

	private Transform sceneContainer;

	private float pressCount;

	private bool hasHitOnce;

	private int playTutorialMusic = -1;

	private Dictionary<string, object> dictIntro = new Dictionary<string, object>();

	private GameObject rdBasement;

	private float extraRatio = 1f;

	private int relativeBar;

	private const float FadeInDuration = 3f;

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
		multiroom = true;
		songsUsed = new string[2] { "sndIntro2", "sndTutorial" };
		helpgiven = false;
		hideHandsAfterStart = true;
		bool isBooth = scnBase.isBooth;
		bool isCustomButton = scnBase.isCustomButton;
		if (isBooth)
		{
			levelType = LevelType.Tutorial;
			levelToSkipTo = Level.OrientalInsomniac;
		}
		else
		{
			levelType = LevelType.Intro;
			levelToSkipTo = Level.OrientalTechno;
		}
		base.game.handControllerOnTop.shouldShow = false;
		extraRatio = (float)Screen.width * 1f / (float)Screen.height / 1.7777778f;
		string key = "P1ControlName";
		if (scnBase.isGamepad | isCustomButton)
		{
			if (!scnBase.isDivekick)
			{
				dictIntro.Add(key, RDString.Get("dialogue.P1ControlName.customButton"));
			}
			else
			{
				dictIntro.Add(key, RDString.Get("dialogue.P1ControlName.divekick"));
			}
		}
		else
		{
			dictIntro.Add(key, RDString.Get("dialogue.P1ControlName"));
		}
	}

	public override IEnumerator LoadBigAssets()
	{
		yield return base.LoadBigAssets();
		backgroundNotes = Background.Create("many_notes_tile_filled", -0.1f, -0.1f);
		backgroundNotes.visible = false;
		backgroundNotes.tiledSprite.SortingOrder = 20;
		sceneContainer = new GameObject("sceneContainer").transform;
		base.ink.Load("diaIntro");
		rdBasement = LoadLevelAsset<RDBasement>().gameObject;
		rdBasement.transform.localPosition = new Vector3(352f, 99f, 0f);
		yield return null;
	}

	public override void preactions()
	{
		int bar = base.conductor.barNumber;
		switch (levelPhase)
		{
		case LevelPhase.TitleScreen:
			ShowingLogo_Preactions(bar);
			break;
		case LevelPhase.FirstDialogue:
			FirstDialogue_Preactions(bar);
			break;
		case LevelPhase.Tutorial:
			Tutorial_Preactions(bar);
			break;
		case LevelPhase.OpeningCredits:
			OpeningCredits_Preactions(bar);
			break;
		case LevelPhase.AfterTutorial:
			break;
		}
	}

	public override void actions()
	{
		int bar = base.conductor.barNumber;
		switch (levelPhase)
		{
		case LevelPhase.TitleScreen:
			ShowingLogo_Actions(bar);
			break;
		case LevelPhase.Tutorial:
			Tutorial_Actions(bar);
			break;
		case LevelPhase.OpeningCredits:
			OpeningCredits_Actions(bar);
			break;
		case LevelPhase.FirstDialogue:
		case LevelPhase.AfterTutorial:
			break;
		}
	}

	public void ShowingLogo_Preactions(int bar)
	{
		if (bar == 1)
		{
			base.game.gameState = GameState.Cutscene;
			base.game.ShowCutsceneBars(show: true);
			base.conductor.SetBPM(82f);
			base.conductor.crotchetsPerBar = 4;
			base.conductor.PlayNoSong();
			RDClass.Vfx.CenterThis(sceneContainer);
			drIan = scrChar.Create(Character.Ian, "typing");
			drIan.transform.parent = sceneContainer;
			drIan.x = -500f;
			drIan.y = -27f;
			drIan.transform.localScale = new Vector3(-1f, 1f, 1f);
			drIan_reflection = scrChar.Create(Character.Ian, "typing");
			drIan_reflection.name = Character.Ian.ToString() + "Reflection";
			drIan_reflection.transform.parent = drIan.transform;
			drIan_reflection.x = 0f;
			drIan_reflection.y = -21f;
			drIan_reflection.transform.localScale = new Vector3(1f, -1f / 3f, 1f);
			drIan_reflection.usesOwnShaderData = true;
			drIan_reflection.shaderData.opacity = 0.4f;
			drIan_reflection.GetComponent<MeshRenderer>().sortingLayerName = RDSortingLayer.Background.ToString();
			drIan_reflection.GetComponent<MeshRenderer>().sortingOrder = 10;
			farmer = scrChar.Create(characterToShow);
			farmer.name = "Farmer";
			farmer.transform.parent = sceneContainer;
			farmer.x = 220f;
			farmer.y = -38f;
			farmer.transform.localScale = new Vector3(1f, 1f, 1f);
			farmer_reflection = scrChar.Create(characterToShow);
			farmer_reflection.name = "FarmerReflection";
			farmer_reflection.transform.parent = farmer.transform;
			farmer_reflection.x = 0f;
			farmer_reflection.y = -21f;
			farmer_reflection.transform.localScale = new Vector3(1f, -1f / 3f, 1f);
			farmer_reflection.usesOwnShaderData = true;
			farmer_reflection.shaderData.opacity = 0.4f;
			farmer_reflection.GetComponent<MeshRenderer>().sortingLayerName = RDSortingLayer.Background.ToString();
			farmer_reflection.GetComponent<MeshRenderer>().sortingOrder = 10;
			base.room0.backgroundOverlay.SetColor(Color.black.WithAlpha(0.43f));
			if (scnBase.isBooth)
			{
				base.game.StartCoroutine(BoothIntroSequence());
				return;
			}
			base.game.StartCoroutine(IntroSequence());
			levelPhase = LevelPhase.FirstDialogue;
		}
	}

	public void ShowingLogo_Actions(int bar)
	{
	}

	public void FirstDialogue_Preactions(int bar)
	{
		if (playTutorialMusic == 0)
		{
			playTutorialMusic = 1;
			base.conductor.SetBPM(88f);
			base.conductor.crotchetsPerBar = 8;
			PlayMusic(songsUsed[1]);
		}
	}

	public void RunPart2Ish()
	{
		string knotName = (scnBase.isBooth ? "part2ish_EXPO" : "part2ish");
		base.ink.Run(knotName, () =>
		{
			levelPhase = LevelPhase.Tutorial;
		}, dictIntro);
	}

	public void ShowArm()
	{
		base.game.handControllerOnTop.gameObject.SetActive(value: true);
		base.game.handControllerOnTop.SlideIn();
		base.game.handControllerOnTop.SetHandsInFront(inFront: true);
	}

	public void EveryBeat()
	{
		if (!hasHitOnce && pressCount >= 7f && relativeBar == 1 && !hasHitOnce)
		{
			pressCount = -999f;
			base.game.StopBeats(killHalfwayPlayingSounds: false);
			RDClass.Vfx.ShoutEffect();
			base.ink.Run("EveryBeat", () =>
			{
				base.game.OnBeat(1);
			}, dictIntro).Prolong(1).WaitForNextBarOnEnd();
		}
	}

	public void Tutorial_Preactions(int bar)
	{
		if (firstBarOnTutorial == -1)
		{
			firstBarOnTutorial = bar - 1;
		}
		relativeBar = bar - firstBarOnTutorial;
		pressCount = 0f;
		base.game.handControllerOnTop.shouldShow = true;
		base.game.handControllerOnTop.Show(show: true);
		switch (relativeBar)
		{
		case 1:
			base.room0.showTextAboveBeatboxes = true;
			scrExecuteOnUpdate.Add(() =>
			{
				if (RDInput.anyPlayerPress)
				{
					pressCount++;
				}
				if (pressCount >= 7f && relativeBar == 1 && !hasHitOnce)
				{
					Timer.Add(EveryBeat, 0.1f);
				}
			});
			base.game.nurseCount = true;
			base.game.statusText.SetStatusText(RDString.Get("status.hitOnSeventhBeat"), null, 4f, narrate: true);
			LEDSign.showMarginError = true;
			base.game.OnBeatClassic(1, 1, 0f);
			scrExecuteOnHit.Add(callTextHit, HitType.Hit, persistent: false, autoDelete: true);
			scrExecuteOnHit.Add(() =>
			{
				Timer.Add(() =>
				{
					hasHitOnce = true;
				}, 0.1f);
			}, HitType.Hit, persistent: false, autoDelete: true, -1, overwrite: false);
			scrExecuteOnHit.Add(callTextMiss, HitType.JustMiss, persistent: true);
			scrExecuteOnHit.Add(callTextMiss, HitType.BigMiss, persistent: true);
			scrExecuteOnHit.Add(callTextMisscom, HitType.MissCompletely, persistent: true);
			break;
		case 2:
			base.game.OnBeat(1);
			base.conductor.Freeze(PlayStyleChange.BeatLoopOnly);
			scrExecuteOnHit.Add(callTextHit2, HitType.Hit, persistent: false);
			break;
		case 3:
			base.game.OnBeat(1);
			base.conductor.Freeze(PlayStyleChange.BeatLoopOnly);
			scrExecuteOnHit.Add(callTextHit3, HitType.Hit, persistent: false);
			break;
		case 4:
			base.game.OnBeat(1);
			base.conductor.Freeze(PlayStyleChange.BeatLoopOnly);
			scrExecuteOnHit.Add(callTextHit4, HitType.Hit, persistent: false);
			break;
		case 5:
			base.game.OnBeat(1);
			base.game.nurseCount = false;
			base.conductor.Freeze(PlayStyleChange.BeatLoopOnly);
			scrExecuteOnHit.Clear(clearUnoverwritables: false);
			scrExecuteOnHit.Add(callTextHit5, HitType.Hit, persistent: false);
			break;
		case 6:
			ShowDrGreyCutscene();
			break;
		}
	}

	public void Tutorial_Actions(int bar)
	{
		pressCount = 0f;
		relativeBar = bar - firstBarOnTutorial;
		_ = relativeBar;
		_ = 1;
	}

	public void OpeningCredits_Preactions(int bar)
	{
	}

	public void OpeningCredits_Actions(int bar)
	{
	}

	public IEnumerator IntroSequence()
	{
		yield return new WaitForSeconds(0f);
		base.room0.backgroundOverlay.sprite.DOColor(Color.clear, 3f).SetEase(Ease.OutQuad);
		base.room0.SetVignette(enabled: true);
		base.room0.SetVignetteAlpha(0f);
		base.room0.vignette.DOFade(0.4f, 3f).SetEase(Ease.OutQuad);
		base.room0.vignette.ScaleX(extraRatio);
		yield return new WaitForSeconds(1f);
		yield return base.ink.RunAndWait(null, "part1", () =>
		{
			RunPart2Ish();
		}, dictIntro);
	}

	public IEnumerator BoothIntroSequence()
	{
		base.room0.backgroundOverlay.sprite.DOColor(Color.clear, 3f).SetEase(Ease.OutQuad);
		yield return new WaitForSeconds(1f);
		LEDSign.status = RDString.Get("status.skipTutorial");
		levelPhase = LevelPhase.FirstDialogue;
		yield return base.ink.RunAndWait(null, scnBase.isBooth ? "part1_EXPO" : "part1", () =>
		{
			RunPart2Ish();
		}, dictIntro);
	}

	public void PlayTutorialMusic()
	{
		playTutorialMusic = 0;
		Vector2 dimensions = backgroundNotes.tiledSprite.dimensions;
		if (extraRatio > 1f)
		{
			backgroundNotes.tiledSprite.dimensions = new Vector2(dimensions.x * extraRatio, dimensions.y);
		}
		backgroundNotes.visible = true;
		backgroundNotes.tiledSprite.color = Color.white.WithAlpha(0f);
		backgroundNotes.tiledSprite.DOColor(Color.white, 0.5f);
		base.game.handControllerOnTop.gameObject.SetActive(value: true);
		base.game.SetGameState(GameState.Handmode);
		rdBasement.GetComponent<RDEnvironment>().StopAmbiences(3f);
	}

	public void ShowTutorial()
	{
	}

	public void ShowBasement()
	{
		backgroundNotes.visible = false;
		RDClass.Vfx.BgColor(Color.black);
		base.game.gameState = GameState.Cutscene;
		base.game.ShowCutsceneBars(show: true);
		RDClass.Vfx.SetHorizontalStripsVisible(visible: false);
		rdBasement.GetComponent<RDEnvironment>().StartAmbiences(3f);
	}

	private void ShowDrGreyCutscene()
	{
		LEDSign.status = "";
		base.game.OffBeat(1);
		base.game.DeleteRow(1);
		levelPhase = LevelPhase.AfterTutorial;
		scrExecuteOnCertainBeat.Add(() =>
		{
			RDClass.Vfx.FadeOut();
		}, 0f);
		scrExecuteOnCertainBeat.Add(() =>
		{
			base.game.StartCoroutine(LoadNextLevel());
		}, 1f);
	}

	public IEnumerator LoadNextLevel()
	{
		string knotName = (scnBase.isBooth ? "part2_EXPO" : "part2");
		yield return base.ink.RunAndWait(null, knotName, null, dictIntro);
		FadeOutMusic(1f);
		rdBasement.GetComponent<RDEnvironment>().StopAmbiences();
		yield return new WaitForSeconds(1f);
		Persistence.SetFirstTimePlaying(state: false);
		if (!scnBase.isBooth)
		{
			Persistence.SetLevelRank(Level.OrientalTechno, -1);
		}
		scnBase.GoToLevelWithEnum(levelToSkipTo);
	}

	public void CreateRow()
	{
		farmer.DOKill();
		base.game.repositionRows = false;
		Row row = base.game.MakeRow(RowType.Classic, 1, 1f, characterToShow);
		RDClass.Vfx.SetSpriteMode(SpriteMode.Glow, Color.black.WithAlpha(0.2f));
		row.ent.animateCharacterEntrance = false;
		Transform transform = row.ent.character.transform;
		transform.localPosition = transform.localPosition.WithX(0f);
		if (Narration.IsEnabled)
		{
			scrConductor.PlayImmediately("sndPatientConnect", 1f, RDUtils.GetMixerGroup("CuesoundsParent"));
			Narration.NarrateRowInfo(row, NarrateInfoType.Connect, soundOnly: false, narrateSkipBeats: false, null, skipsUnstable: false, RDPlayer.AutoDetect, -1);
		}
	}

	public RDInk DiaTutorialHit(string node)
	{
		base.conductor.SetPlayStyle(PlayStyleChange.OnNextBar);
		return base.ink.Run(node, null, dictIntro);
	}

	private void callTextMiss()
	{
		hasHitOnce = true;
		if (!helpgiven)
		{
			helpgiven = true;
			base.game.StopBeats(killHalfwayPlayingSounds: false);
			base.ink.Run("tutorial1Miss", () =>
			{
				base.game.OnBeat(1);
			}, dictIntro).Prolong(2).WaitForNextBarOnEnd();
			base.game.OffBeat(1);
		}
	}

	private void callTextMisscom()
	{
		hasHitOnce = true;
		base.ink.Run("tutorial1NoInput", null, dictIntro);
	}

	private void callTextHit()
	{
		DiaTutorialHit("tutorial1Perfect");
	}

	private void callTextHit2()
	{
		DiaTutorialHit("tutorial2");
	}

	private void callTextHit3()
	{
		DiaTutorialHit("tutorial3");
	}

	private void callTextHit4()
	{
		DiaTutorialHit("tutorial4");
	}

	private void callTextHit5()
	{
		base.ink.Run("tutorial5", null, dictIntro).Prolong().JumpOnEnd();
		base.game.OffBeat(1);
	}
}
