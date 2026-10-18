using System;
using System.Collections;
using System.Collections.Generic;
using System.Media;
using DG.Tweening;
using RDLevelEditor;
using RDTools;
using UnityEngine;

public class Level_AthleteFinale : LevelBase
{
	private RDBossStageText bossText;

	public bool samuraiHittingCups;

	public bool hittingBaseballs;

	private Row nicoleRow;

	private Row samuraiRow;

	private Row samuraiRowP2;

	private Row luckyRow;

	private Row luckyRowP2;

	private RDCoffeeShop coffeeShop;

	private RDAthleteWard stadium;

	private Dictionary<Beat, Transform> cupDict = new Dictionary<Beat, Transform>();

	private Dictionary<Beat, Sequence> cupSeqDict = new Dictionary<Beat, Sequence>();

	private GameObject cupPrefab;

	private const string DialogueFile = "diaAthleteFinale";

	private HPBarController hpController;

	private bool dealBossDamage;

	public int notesCount = 29;

	private bool startedRooftopTransition;

	private float specialBallBeats;

	private bool doingSpecialBall;

	private Sequence specialBallSequence;

	private const int sortingOrderOffset = 10;

	public Level_AthleteFinale(RDLevelData data)
		: base(data)
	{
	}

	public override void Init()
	{
		checkpointBarsDesc = new int[1] { 71 };
		cutsceneBarsDesc = new (int, int)[3]
		{
			(199, 222),
			(133, 160),
			(71, 99)
		};
		songsUsed = new string[1] { "sndDDS" };
		base.game.barToSkipToOnSlash = 151;
		hpController = base.game.hpController;
		base.ink.Load("diaAthleteFinale");
	}

	public override IEnumerator LoadBigAssets()
	{
		if (base.game.currentLevel == null)
		{
			RDClassDll.printem("current level is null!");
		}
		missesToCrackHeart = 25f;
		levelType = LevelType.Boss;
		bossActNum = 5;
		SetUpFlashMarginFeedback(RowType.Oneshot);
		bossText = LoadLevelAsset<RDBossStageText>();
		bossText.transform.localPosition = new Vector3(3000f + RDClass.Vfx.RDWidth * 0.5f, RDClass.Vfx.RDHeight * 0.5f, 20f);
		yield return null;
	}

	public override bool FailLevel(RowEntity ent)
	{
		Persistence.SetLevelRank(base.game.levelIdentifier, -1);
		scrChar mainCharacter = base.rowEnt0.character;
		failedLevel = true;
		RDClass.Vfx.KillFlashTextUIAnimations();
		mainCharacter.animator.deltaTimeStyle = tk2dSpriteAnimator.DeltaTimeStyle.AlwaysOneAlways;
		DOVirtual.DelayedCall(2f, () =>
		{
			ent.heart.DoFinalCrack();
			RDUtils.Rotate2D(ent.heart.transform, 0f);
		});
		DOVirtual.DelayedCall(3f, () =>
		{
			RDClass.Vfx.SpotlightFocusFromCenter(mainCharacter.transform.position.x, 0.5f, playSwitchSound: true, RDUtils.GetMixerGroup("FeedbackSoundsParent"));
		});
		DOVirtual.DelayedCall(5f, () =>
		{
			base.game.statusText.Show("status.bossFailRestart", narrate: true);
			GameOverBarInfo genericGameOverInfo = new GameOverBarInfo(-1, new int[4] { 1, 1, 1, 1 });
			GameOverBarInfo item = new GameOverBarInfo(99, new int[4] { 1, 1, 1, 1 });
			GameOverBarInfo item2 = new GameOverBarInfo(151, new int[4] { 1, 1, 1, 1 });
			GameOverBarInfo item3 = new GameOverBarInfo(185, new int[4] { 1, 1, 1, 1 });
			List<GameOverBarInfo> gameOverBars = new List<GameOverBarInfo> { item, item2, item3 };
			PlayGameOverDialogue(gameOverBars, genericGameOverInfo);
		});
		scrExecuteOnPeriodicBeat.Clear();
		HideEntities();
		RunEventsWithTag("FailLevel");
		return true;
	}

	private void HideEntities()
	{
		foreach (CustomSprite value in sprites.Values)
		{
			value.gameObject.SetActive(value: false);
		}
		Row[] array = base.game.rows;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].ent.Hide(keepCharacterVisible: false, effects: false);
		}
	}

	public override void preactions()
	{
		int num = base.conductor.barNumber;
		if (num == 1)
		{
			nicoleRow = base.game.rows[10];
			samuraiRow = base.game.rows[1];
			samuraiRowP2 = base.game.rows[3];
			luckyRow = base.game.rows[0];
			luckyRowP2 = base.game.rows[2];
			coffeeShop = base.game.rooms[0].coffeeShop.GetComponent<RDCoffeeShop>();
			stadium = base.game.rooms[0].athleteWard.GetComponent<RDAthleteWard>();
			ShowEdega(show: true);
			MeshRenderer component = nicoleRow.ent.character.GetComponent<MeshRenderer>();
			component.sortingLayerName = "Background";
			component.sortingOrder = 5;
			cupPrefab = coffeeShop.cups_4[0].gameObject;
			SpeedTrail speedTrail = cupPrefab.gameObject.AddComponent<SpeedTrail>();
			speedTrail.length = 10;
			speedTrail.enabled = false;
			hpController.bossHP.SetMaxHP(notesCount);
			scrExecuteOnHit.Add(() =>
			{
				luckyRow.ent.CrackAdvance(mistakeWeight);
				samuraiRow.ent.Heal(mistakeWeight);
				if (GC.twoPlayerMode)
				{
					luckyRowP2.ent.CrackAdvance(mistakeWeight);
					samuraiRowP2.ent.Heal(mistakeWeight);
				}
			}, HitType.AnyMiss, persistent: true, autoDelete: false, 1);
		}
		if (samuraiHittingCups)
		{
			PrepareCups(num);
		}
		if (hittingBaseballs)
		{
			stadium.PrepareBaseballs(num);
		}
	}

	public override void actions()
	{
		if (base.conductor.barNumber == 163)
		{
			dealBossDamage = true;
			mistakeWeight = 2f;
		}
		if (base.conductor.barNumber == 183)
		{
			dealBossDamage = false;
			mistakeWeight = 1f;
		}
		if (base.conductor.barNumber == 193)
		{
			dealBossDamage = true;
			mistakeWeight = 2f;
		}
		if (base.conductor.barNumber == 199)
		{
			dealBossDamage = false;
			missesToCrackHeart = 999f;
		}
	}

	public override void OnHit(HitType hitType, Beat beat = null)
	{
		HitCup(hitType, beat);
		stadium.HitBaseball(hitType, beat);
		if (dealBossDamage)
		{
			hpController.bossHP.DealDamage(hitType);
		}
	}

	public void ShowBossText(float enterSpeed, float waitSpeed, float exitSpeed)
	{
		if (!base.conductor.isScrubbing)
		{
			bossText.gameObject.SetActive(value: true);
			bossText.SetColor(Color.white);
			bossText.Play();
		}
		else
		{
			bossText.gameObject.SetActive(value: false);
		}
	}

	public void MoveSky(int room, float y, float durBeats, string easeStr)
	{
		_ = y / 100f;
		_ = RDClass.Vfx.RDHeight;
		float dur = base.conductor.DurationBeatsToTime(base.conductor.currentBeat, durBeats);
		Ease ease = Enum.Parse<Ease>(easeStr);
		base.game.rooms[room].rooftop.GetComponent<RDRooftop>().TweenSky(y, dur, ease);
	}

	public void MoveSkyscrapers(int room, float y, float durBeats, string easeStr)
	{
		_ = y / 100f;
		_ = RDClass.Vfx.RDHeight;
		float dur = base.conductor.DurationBeatsToTime(base.conductor.currentBeat, durBeats);
		Ease ease = Enum.Parse<Ease>(easeStr);
		base.game.rooms[room].rooftop.GetComponent<RDRooftop>().TweenSkyscraper(y, dur, ease);
	}

	public void MoveRooftop(int room, float y, float durBeats, string easeStr)
	{
		_ = y / 100f;
		_ = RDClass.Vfx.RDHeight;
		float dur = base.conductor.DurationBeatsToTime(base.conductor.currentBeat, durBeats);
		Ease ease = Enum.Parse<Ease>(easeStr);
		base.game.rooms[room].rooftop.GetComponent<RDRooftop>().TweenRoof(y, dur, ease);
	}

	public void RooftopTransition(int room, int step)
	{
		RDRooftop component = base.game.rooms[room].rooftop.GetComponent<RDRooftop>();
		if (!startedRooftopTransition)
		{
			component.autumnParent.SetActive(value: true);
			component.autumnSky.gameObject.SetActive(value: false);
			component.autumnSkyscrapers.gameObject.SetActive(value: false);
			component.autumnRoof.gameObject.SetActive(value: false);
			startedRooftopTransition = true;
		}
		switch (step)
		{
		case 0:
			component.summerSky.gameObject.SetActive(value: false);
			component.autumnSky.gameObject.SetActive(value: true);
			break;
		case 1:
			component.summerSkyscrapers.gameObject.SetActive(value: false);
			component.autumnSkyscrapers.gameObject.SetActive(value: true);
			break;
		case 2:
			component.summerRoof.gameObject.SetActive(value: false);
			component.autumnRoof.gameObject.SetActive(value: true);
			break;
		case 3:
			component.summerSky.gameObject.SetActive(value: true);
			component.summerSkyscrapers.gameObject.SetActive(value: true);
			component.summerRoof.gameObject.SetActive(value: true);
			component.autumnSky.gameObject.SetActive(value: true);
			component.autumnSkyscrapers.gameObject.SetActive(value: true);
			component.autumnRoof.gameObject.SetActive(value: true);
			break;
		}
	}

	private void HitCup(HitType hittype, Beat beat)
	{
		if (hittype == HitType.Hit && cupDict.TryGetValue(beat, out var value) && cupSeqDict.ContainsKey(beat))
		{
			Vector3 position = ((GC.twoPlayerMode && beat.rowID == samuraiRowP2.id) ? samuraiRowP2 : samuraiRow).ent.transform.position + new Vector3(26f, 5f);
			Vector3 vector = new Vector3(UnityEngine.Random.Range(0f, 40f), 0f, UnityEngine.Random.Range(0f, 40f));
			Vector3 endValue = new Vector3(360f, 200f) + vector;
			float num = -2880f;
			float num2 = 0.5f / RDTime.speed;
			value.position = position;
			value.GetComponent<SpeedTrail>().enabled = true;
			cupSeqDict[beat].Kill();
			Sequence value2 = DOTween.Sequence().Append(value.DOMove(endValue, num2 * base.game.visualSpeed).SetEase(Ease.Linear)).Join(value.DORotate(Vector3.forward * num, num2, RotateMode.LocalAxisAdd).SetEase(Ease.Linear))
				.OnComplete(() =>
				{
					DestroyCup(beat);
				});
			cupSeqDict[beat] = value2;
		}
	}

	private void PrepareCups(int bar)
	{
		foreach (Beat beat in base.game.beats)
		{
			if (!(beat is BeatOneshot))
			{
				continue;
			}
			BeatOneshot beatOneshot = beat as BeatOneshot;
			if (beatOneshot.bar == bar && !beatOneshot.unhittable)
			{
				if (!GC.twoPlayerMode)
				{
					SetupCup(beatOneshot, player2: false);
				}
				else if (beat.rowID == samuraiRow.id)
				{
					SetupCup(beatOneshot, player2: false);
				}
				else if (beat.rowID == samuraiRowP2.id)
				{
					SetupCup(beatOneshot, player2: true);
				}
			}
		}
	}

	private void SetupCup(BeatOneshot oneshot, bool player2)
	{
		Transform cup = UnityEngine.Object.Instantiate(cupPrefab, base.game.rooms[0].transform).transform;
		cup.gameObject.SetActive(value: false);
		float height = 40f;
		float throwDelay = 1f / 6f;
		float airTime = 0.39f;
		float landTime = 0.065f;
		double actionTime = oneshot.chakTimeAbs - (double)throwDelay - (double)(airTime / RDTime.speed);
		Vector3 initialRot = Vector3.forward * UnityEngine.Random.Range(0f, 360f);
		Vector3 randomOffset = new Vector3(UnityEngine.Random.Range(-2f, 2f), 0f, UnityEngine.Random.Range(-2f, 2f));
		Row targetRow = (player2 ? samuraiRowP2 : samuraiRow);
		if (!player2)
		{
			scrExecuteOnCertainTime.Add(() =>
			{
				nicoleRow.ent.character.PlayExpression("throw");
			}, actionTime);
		}
		scrExecuteOnCertainTime.Add(() =>
		{
			Vector3 position = nicoleRow.ent.transform.position + new Vector3(-15f, 16f);
			Vector3 endValue = targetRow.ent.transform.position + new Vector3(11f, -14f) + randomOffset;
			cup.position = position;
			cup.eulerAngles = initialRot;
			float duration = (airTime + landTime) / RDTime.speed * base.game.visualSpeed;
			Sequence value = DOTween.Sequence().AppendInterval(throwDelay).AppendCallback(() =>
			{
				cup.gameObject.SetActive(value: true);
			})
				.Append(cup.DOLocalJump(endValue, height, 1, duration).SetEase(Ease.Linear))
				.Join(cup.transform.DORotate(new Vector3(0f, 0f, 720f), duration, RotateMode.LocalAxisAdd).SetEase(Ease.InOutCubic))
				.Append(cup.transform.DOShakePosition(0.2f, 1f, 10, 90f, snapping: true))
				.Append(cup.GetComponent<tk2dSprite>().DOFade(0f, 0.5f).SetEase(Ease.Linear))
				.OnComplete(() =>
				{
					DestroyCup(oneshot);
				})
				.Play();
			cupSeqDict.Add(oneshot, value);
		}, actionTime);
		cupDict.Add(oneshot, cup);
	}

	private void DestroyCup(Beat beat)
	{
		UnityEngine.Object.Destroy(cupDict[beat].gameObject);
		cupDict.Remove(beat);
	}

	public void ThrowSpecialBall(int row, float beats, float skipTo)
	{
		Transform transform = UnityEngine.Object.Instantiate(stadium.specialBaseballPrefab, stadium.baseballHolder).transform;
		Perspective2DSprite component = transform.GetComponent<Perspective2DSprite>();
		transform.gameObject.SetActive(value: true);
		float num = base.conductor.crotchet * beats;
		specialBallSequence = stadium.ThrowBaseball(component, base.game.rows[row].ent, num);
		specialBallSequence.Goto(skipTo * num, andPlay: true);
	}

	public void DoSpecialBallSpeed(float speed, float beats, string easeStr)
	{
		if (specialBallSequence != null)
		{
			float duration = base.conductor.crotchet * beats;
			Ease ease = Enum.Parse<Ease>(easeStr);
			DOTween.To(() => specialBallSequence.timeScale, (float x) =>
			{
				specialBallSequence.timeScale = x;
			}, speed, duration).SetEase(ease);
		}
	}

	public void TryHitSpecialBall(int row, float beats)
	{
		doingSpecialBall = true;
		scrExecuteOnHit.Add(() =>
		{
			if (doingSpecialBall)
			{
				HitSpecialBall(row, beats);
			}
			doingSpecialBall = false;
		}, HitType.Hit, persistent: false, autoDelete: true, row);
		scrExecuteOnHit.Add(() =>
		{
			doingSpecialBall = false;
		}, HitType.AnyMiss, persistent: false, autoDelete: true, row);
	}

	public void HitSpecialBall(int row, float beats)
	{
		specialBallSequence?.Complete();
		Transform transform = UnityEngine.Object.Instantiate(stadium.specialBaseballPrefab, stadium.baseballHolder).transform;
		Perspective2DSprite component = transform.GetComponent<Perspective2DSprite>();
		transform.gameObject.SetActive(value: true);
		Vector2 screenPosition = (Vector2)(base.game.rows[row].ent.character.transform.position - stadium.perspective2DRoom.transform.position) + new Vector2(13f, -4f);
		Vector3 projectedPosition = stadium.perspective2DRoom.GetProjectedPosition(component, screenPosition, Vector3.one);
		float duration = beats * base.conductor.crotchet / RDTime.speed * base.game.visualSpeed;
		float f = 2.443461f;
		Vector3 endValue = new Vector3(Mathf.Sin(f) * 352f, 100f, Mathf.Cos(f) * 352f) + projectedPosition;
		float num = -2880f;
		component.position = projectedPosition;
		DOTween.Sequence().Append(component.DOMove(endValue, duration).SetEase(Ease.Linear)).Join(transform.DORotate(Vector3.forward * num, duration, RotateMode.LocalAxisAdd).SetEase(Ease.Linear));
	}

	public void SetCharacterLayer(int row, int layer)
	{
		base.rows[row].ent.character.sprite.SortingOrder = 10 * layer;
	}

	public void ShowEdega(bool show)
	{
		stadium.physioEdega.gameObject.SetActive(show);
	}

	public void TogglePhysioLights(bool on)
	{
		stadium.TweenPhysioLightingAlpha(on ? 1f : 0f, 0f);
	}

	public void SetRain(float baseballsPerSecond)
	{
		stadium.SetRain(baseballsPerSecond);
	}

	public void KillRain()
	{
		stadium.KillRain();
	}

	public void PlayLightning(bool showTeam)
	{
		stadium.AnimateLightning(showTeam);
	}

	public void PlaySystemErrorSound(int num)
	{
		if (scnBase.isWindows)
		{
			SystemSound systemSound = SystemSounds.Asterisk;
			switch (num)
			{
			case 1:
				systemSound = SystemSounds.Beep;
				break;
			case 2:
				systemSound = SystemSounds.Hand;
				break;
			case 3:
				systemSound = SystemSounds.Exclamation;
				break;
			case 4:
				systemSound = SystemSounds.Question;
				break;
			}
			systemSound.Play();
		}
	}
}
