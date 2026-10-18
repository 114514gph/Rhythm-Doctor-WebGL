using System.Collections;
using DG.Tweening;
using RDLevelEditor;
using UnityEngine;

public class Level_AthleteTherapy : LevelBase
{
	private AudioSource hitSound;

	private double hitTime;

	private BungeeRope rope;

	private float timerDestination;

	private string tagToRunAfterTimer;

	private AudioSource rummageSound;

	private int eavesdroppingProgress;

	public Level_AthleteTherapy(RDLevelData data)
		: base(data)
	{
	}

	public override IEnumerator LoadBigAssets()
	{
		rope = LoadPublicPrefab("BungeeRope/BungeeRope").GetComponent<BungeeRope>();
		rope.gameObject.SetActive(value: false);
		yield return null;
	}

	public override void preactions()
	{
		if (base.conductor.barNumber == 1)
		{
			base.game.rows[0].ent.playerParticle.transform.localScale = Vector3.zero;
		}
	}

	public override void OnHeldPress(HitType hitType, Beat beat = null)
	{
		hitTime = beat.releaseTime;
		hitSound = AudioManager.Play("Sfx\\ClapsAndExplosions\\sndExplodeNewv1_Highpass", hitTime, 0.3f);
	}

	public override void OnHit(HitType hitType, Beat beat = null)
	{
		if (hitType == HitType.JustMiss && base.conductor.audioPos < hitTime)
		{
			hitSound?.Stop();
		}
	}

	public void MoveRope(float x, float y)
	{
		rope.transform.position = new Vector2(x, y);
	}

	public void ShowRope(bool show)
	{
		rope.gameObject.SetActive(show);
	}

	public void ScheduleRummageSound()
	{
		DOVirtual.DelayedCall(2.3f, () =>
		{
			rummageSound = scrConductor.PlayImmediately("sndStretchRummage");
		});
	}

	public void FadeRummageSound()
	{
		rummageSound?.DOFadeStop(0.15f);
	}

	public void KillBeats()
	{
		Beat[] array = base.game.beats.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].DestroyBeat();
		}
	}

	public override void Update()
	{
		if (!tagToRunAfterTimer.IsNullOrEmpty() && Time.timeSinceLevelLoad >= timerDestination)
		{
			base.game.currentLevel.RunEventsWithTag(tagToRunAfterTimer);
			tagToRunAfterTimer = null;
		}
		if (DebugSettings.instance.RecordingMode && base.autoplay)
		{
			b9 = true;
			if (i1 >= i2)
			{
				b1 = false;
			}
		}
	}

	public void StartTimer(float beats, string tagToRun)
	{
		timerDestination = Time.timeSinceLevelLoad + base.conductor.DurationBeatsToTime(beats);
		tagToRunAfterTimer = tagToRun;
	}

	public void IncrementEavesdroppingProgress()
	{
		eavesdroppingProgress++;
	}

	public void TryGrantEavesdroppingAchievement()
	{
		if (eavesdroppingProgress == 3)
		{
			Persistence.UnlockAchievement(Achievement.Eavesdropping, storeStats: true);
		}
	}
}
