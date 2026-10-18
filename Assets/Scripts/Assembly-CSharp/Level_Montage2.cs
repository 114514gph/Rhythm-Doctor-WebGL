using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using RDLevelEditor;
using UnityEngine;

public class Level_Montage2 : LevelBase
{
	private string dialogueName = "diaMontage";

	private bool missedOnce;

	private scrCareLessParticles careLessParticles;

	private ColeMidi coleMidi;

	private RDAthleteWard _stadium;

	private LyricsGame countingLyric;

	private RDAthleteWard stadium
	{
		get
		{
			if (_stadium == null)
			{
				_stadium = base.room0.athleteWard.GetComponent<RDAthleteWard>();
			}
			return _stadium;
		}
	}

	public Level_Montage2(RDLevelData data)
		: base(data)
	{
	}

	public override void Init()
	{
		checkpointBarsDesc = new int[2] { 286, 213 };
		base.game.barToSkipToOnSlash = 309;
	}

	public override void preactions()
	{
		if (base.conductor.barNumber == 1)
		{
			scnGame.pauseBlocked = false;
			missesToCrackHeart = rankLowerBounds[1];
			if (base.game.barToScrubTo > 0 && !base.editorMode)
			{
				missedOnce = true;
			}
		}
	}

	public override void OnMistake(RowEntity ent = null)
	{
		missedOnce = true;
		if (base.numMistakes > missesToCrackHeart)
		{
			FailBoss();
		}
		else
		{
			UpdateAllHearts();
		}
	}

	public override IEnumerator LoadBigAssets()
	{
		levelType = LevelType.Boss;
		bossActNum = 7;
		noBossFail = true;
		manualHeartCrack = true;
		careLessParticles = LoadLevelAsset("RDHoodieBoyWard/CareLess/CareLessParticles").GetComponent<scrCareLessParticles>();
		careLessParticles.spiralTrans.LocalMoveXY(3000f + RDClass.Vfx.RDWidth * 0.5f, RDClass.Vfx.RDHeight * 0.5f);
		ParticleSystem[] spiralSystems = careLessParticles.spiralSystems;
		foreach (ParticleSystem obj in spiralSystems)
		{
			ParticleSystem.MainModule main = obj.main;
			main.startLifetime = main.startLifetime.constant * 2f;
			main.prewarm = false;
			obj.Stop(withChildren: false, ParticleSystemStopBehavior.StopEmittingAndClear);
		}
		careLessParticles.ToggleAllSystemsActive(toggleActive: false);
		coleMidi = LoadLevelAsset("ColeMidi/ColeMidi").GetComponent<ColeMidi>();
		coleMidi.transform.LocalMoveXY(3000f + RDClass.Vfx.RDWidth * 0.5f, RDClass.Vfx.RDHeight * 0.5f - 149f - 198f);
		coleMidi.gameObject.SetActive(value: false);
		base.ink.Load(dialogueName);
		yield return null;
	}

	public void ToggleMidi(bool toggle)
	{
		ParticleSystem[] spiralSystems = careLessParticles.spiralSystems;
		foreach (ParticleSystem particleSystem in spiralSystems)
		{
			particleSystem.gameObject.SetActive(toggle);
			if (toggle)
			{
				particleSystem.Simulate(200f);
				particleSystem.Play();
			}
			else
			{
				particleSystem.Stop();
			}
		}
		foreach (ColeMidi.Pattern pattern in coleMidi.patterns)
		{
			foreach (ColeMidi.Note note in pattern.notes)
			{
				note.renderer.DOFade(0.8f, 4f).From(0f).SetEase(Ease.InSine)
					.Done();
			}
		}
		coleMidi.gameObject.SetActive(toggle);
	}

	public void SetScoreboardLights(bool home, string text, int startIndex)
	{
		stadium.SetScoreboardLightsWithString(home, text, startIndex);
	}

	public void ClearAllScoreboardLights()
	{
		stadium.ClearAllLights();
	}

	public void SetTrackingScore(bool on)
	{
		stadium.trackingScore = on;
	}

	public void LinkRowsSoIDontHaveToVFXThis(int parentID, int childID)
	{
		base.rows[childID].ent.transform.SetParent(base.rows[parentID].ent.transform);
	}

	public void UnlinkRow(int rowID, int room)
	{
		base.rows[rowID].ent.transform.SetParent(base.game.rooms[room].rowContainer);
	}

	public void InfectAll()
	{
		Row[] array = base.rows;
		foreach (Row row in array)
		{
			if (row.ent != null)
			{
				row.ent.heart.QuickInfect();
			}
		}
	}

	public override void actions()
	{
		if (base.conductor.barNumber == 191)
		{
			scrExecuteOnHit.Add(() =>
			{
				RunEventsWithTag("coleCatchEarly");
			}, HitType.EarlyOnly, persistent: true, autoDelete: false, -1, overwrite: false);
			scrExecuteOnHit.Add(() =>
			{
				RunEventsWithTag("coleCatchPerfect");
			}, HitType.Hit, persistent: true, autoDelete: false, -1, overwrite: false);
			scrExecuteOnHit.Add(() =>
			{
				RunEventsWithTag("coleCatchLate");
			}, HitType.LateOnly, persistent: true, autoDelete: false, -1, overwrite: false);
		}
		if (base.conductor.barNumber == 192)
		{
			scrExecuteOnHit.Clear();
		}
	}

	public override void OnHit(HitType hitType, Beat beat)
	{
		if (stadium.trackingScore)
		{
			stadium.UpdateScoreboard(hitType, beat, beat.player == RDPlayer.P1);
		}
		if (60 <= base.conductor.barNumber && base.conductor.barNumber < 76 && beat.hasHeldPulses && hitType != HitType.Hit)
		{
			RunEventsWithTag("showHold");
		}
	}

	public void ToggleCatching(int room, bool rightHand, bool on)
	{
		scrHandController scrHandController2 = room switch
		{
			1 => base.game.handController1, 
			2 => base.game.handController2, 
			3 => base.game.handController3, 
			4 => base.game.handControllerOnTop, 
			_ => base.game.handController0, 
		};
		(rightHand ? scrHandController2.rightArm : scrHandController2.leftArm).ToggleCatching(on);
	}

	public void EnableTextPeek()
	{
		base.game.rooms[2].peekWindowMode = true;
		base.game.rooms[2].windowPeekIndex = 0;
		base.game.rooms[2].customPeekScale = 0.5f;
	}

	public void DisableTextPeek()
	{
		base.game.rooms[2].peekWindowMode = false;
		base.game.rooms[2].customPeekScale = 1f;
	}

	public void EnableHandPeek()
	{
		base.game.rooms[0].ManualCalibratePeek(new Vector2Int(base.game.windowChoreographer.dancers[0].window.GetCenter().x, base.game.windowChoreographer.dancers[2].window.GetCenter().y));
		base.game.rooms[0].peekWindowMode = true;
		base.game.rooms[0].windowPeekIndex = 0;
	}

	public void DisableHandPeek()
	{
		base.game.rooms[0].peekWindowMode = false;
	}

	public void FindLyric()
	{
		scrVfxControl.instance.allVfxLyrics.TryGetValueAs<int, LyricsGame, LyricsGame>(27, out var valueAs);
		countingLyric = valueAs;
	}

	public void ReplaceLyric(string text)
	{
		if ((bool)countingLyric)
		{
			countingLyric.text = text;
		}
	}

	public void AddMistakeIfMissed()
	{
		if (missedOnce && base.numMistakes < 1f)
		{
			base.game.mistakesManager.AddMistake(RDPlayer.P1, 0f, 1f);
		}
	}

	public void FailBoss()
	{
		if (i9 < 3)
		{
			base.game.FailLevelLite();
			base.game.AddLevelDeath();
			RunEventsWithTag("FailLevel");
		}
	}

	public void FailSection()
	{
		RunEventsWithTag($"FailLevel_{i9}");
	}

	public void SetSkippableIfPassed()
	{
		if (Persistence.GetLevelRank(Level.Montage2).passed)
		{
			skippable = true;
		}
	}

	public void HideAllCharacters()
	{
		Row[] array = base.game.rows;
		for (int i = 0; i < array.Length; i++)
		{
			array[i]?.ent?.Hide(keepCharacterVisible: false, effects: false);
		}
	}

	public void RunGameOverDialogue()
	{
		GameOverBarInfo genericGameOverInfo = new GameOverBarInfo(-1, new int[6] { 1, 1, 1, 1, 1, 1 });
		List<GameOverBarInfo> list = new List<GameOverBarInfo>();
		list.Add(new GameOverBarInfo(213, new int[4] { 1, 1, 1, 1 }));
		list.Add(new GameOverBarInfo(289, new int[3] { 1, 1, 1 }));
		List<GameOverBarInfo> gameOverBars = list;
		PlayGameOverDialogue(gameOverBars, genericGameOverInfo, "GameOver_Montage2");
	}

	public void CancelWindowDance()
	{
		base.game.windowChoreographer?.Cancel();
	}
}
