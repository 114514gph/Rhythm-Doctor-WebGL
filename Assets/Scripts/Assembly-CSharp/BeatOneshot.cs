using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class BeatOneshot : Beat
{
	public int phase;

	public float boomTime;

	public float chakTime;

	public float rushTime;

	public float rushAnimDuration;

	public double boomTimeAbs;

	public double rushTimeAbs;

	public double chakTimeAbs;

	public OneshotPulseType pulseType;

	public OneshotFriendGroup friendGroup;

	public List<BeatOneshot> subdivBuddies;

	[NonSerialized]
	public bool freezeshot;

	[NonSerialized]
	public bool secondSubdivisionOrLater;

	[NonSerialized]
	public bool addedBeatEvent;

	[NonSerialized]
	public bool skipToFlatAnim;

	[NonSerialized]
	public bool weightWasReduced;

	[NonSerialized]
	public float tick;

	[NonSerialized]
	public RDWave assignedToWave;

	[NonSerialized]
	public int subdivCount;

	[NonSerialized]
	public int subdivIndex;

	[NonSerialized]
	public float waveOffset;

	[NonSerialized]
	public float polygonRotation;

	public float boomPeriod => (float)(rushTimeAbs - boomTimeAbs) / 2f;

	public float wavePeriod => (float)(chakTimeAbs - boomTimeAbs);

	public List<BeatOneshot> friends => friendGroup.friends;

	public bool isPolygon
	{
		get
		{
			OneshotPulseType oneshotPulseType = pulseType;
			return oneshotPulseType == OneshotPulseType.Square || oneshotPulseType == OneshotPulseType.Triangle;
		}
	}

	protected override RDGameSoundData hitSoundData
	{
		get
		{
			GameSoundType type = GameSoundType.ClapSoundCPUOneshot;
			switch (row.GetPlayerWeAreGoingToChangeInto())
			{
			case RDPlayer.P1:
				type = GameSoundType.ClapSoundP1Oneshot;
				break;
			case RDPlayer.P2:
				type = GameSoundType.ClapSoundP2Oneshot;
				break;
			}
			return RDGameSounds.Get(type);
		}
	}

	protected RDGameSoundData[] freezeshotSoundData => new RDGameSoundData[4]
	{
		RDGameSounds.Get(GameSoundType.FreezeshotSoundCueLow),
		RDGameSounds.Get(GameSoundType.FreezeshotSoundCueHigh),
		RDGameSounds.Get(GameSoundType.FreezeshotSoundRiser),
		RDGameSounds.Get(GameSoundType.FreezeshotSoundCymbal)
	};

	protected RDGameSoundData[] burnshotSoundData => new RDGameSoundData[4]
	{
		RDGameSounds.Get(GameSoundType.BurnshotSoundCueLow),
		RDGameSounds.Get(GameSoundType.BurnshotSoundCueHigh),
		RDGameSounds.Get(GameSoundType.BurnshotSoundRiser),
		RDGameSounds.Get(GameSoundType.BurnshotSoundCymbal)
	};

	public BeatOneshot(int _rowID)
	{
		playerHitSoundsIndex = 1;
		base.rowID = _rowID;
		RefreshMistakeWeight();
	}

	public BeatOneshot AddBeatOneShot(float boomStartBeat, float tick, OneshotPulseType pulseType = OneshotPulseType.Wave, string customHitSound = "", float delay = 0f, float delayBeatOffset = 0f, float visualFreezeBeatOverride = 0f, int subdivCount = 1, int subdivIndex = 0, bool isBurnshot = false, float holdDuration = 0f, float holdCueOffset = 0f)
	{
		float crotchet = base.conductor.crotchet;
		float start = boomStartBeat;
		boomStartBeat += delayBeatOffset;
		this.tick = tick;
		this.pulseType = pulseType;
		this.subdivCount = subdivCount;
		this.subdivIndex = subdivIndex;
		waveOffset = (float)(subdivCount - (subdivIndex + 1)) / (float)subdivCount / 1.33f;
		if (subdivCount > 0)
		{
			polygonRotation = (float)subdivIndex / (float)(subdivCount - 1) * 60f;
		}
		freezeshot = delay > 0f;
		secondSubdivisionOrLater = subdivIndex > 0;
		bool flag = false;
		float b = base.conductor.DurationBeatsToTime(boomStartBeat, tick) / 2f;
		float a = base.game.animationRush.duration / base.game.animationRush.timeScale;
		rushAnimDuration = Mathf.Min(a, b);
		boomTime = base.conductor.BeatToTime(boomStartBeat);
		chakTime = base.conductor.BeatToTime(boomStartBeat + tick + delay);
		float num = base.conductor.BeatToTime(boomStartBeat + tick);
		rushTime = ((!isBurnshot) ? num : chakTime) - rushAnimDuration;
		float num2 = base.conductor.BeatToTime(boomStartBeat + tick + delay + holdDuration);
		isHeldClap = (hasHeldPulses = holdDuration > 0f);
		float num3 = base.conductor.BeatToTime(start);
		float num4 = base.conductor.BeatToTime(start + (freezeshot ? delay : tick));
		if (base.game.currentLevel.pauseOn7thBeat)
		{
			float num5 = crotchet * 6f;
			if (boomTime < num5)
			{
				if (chakTime >= num5)
				{
					chakTime += crotchet;
					flag = true;
				}
				if (rushTime >= num5)
				{
					rushTime += crotchet;
				}
			}
		}
		boomTimeAbs = (double)boomTime + base.conductor.startOfNextBar;
		chakTimeAbs = (double)chakTime + base.conductor.startOfNextBar;
		rushTimeAbs = (double)rushTime + base.conductor.startOfNextBar;
		double num6 = (double)num2 + base.conductor.startOfNextBar;
		explodeTimeAbs = num6 + (double)(crotchet * base.game.currentLevel.crotchetsToExplode);
		string sound = ((customHitSound != "") ? customHitSound : row.rowSound);
		AudioMixerGroup beatSoundMixerGroup = base.BeatSoundMixerGroup;
		AudioMixerGroup specialOneshotCuesMixerGroup = base.SpecialOneshotCuesMixerGroup;
		AudioMixerGroup specialOneshotCuesMixerGroup2 = base.SpecialOneshotCuesMixerGroup;
		AudioMixerGroup hitSoundMixerGroup = base.HitSoundMixerGroup;
		AudioSource item = base.conductor.PlayBeat(sound, boomTime, row.rowSoundVolMultiplier, beatSoundMixerGroup, row.rowSoundMinPitch, row.rowSoundMaxPitch, row.rowSoundPanAdjustedFor2P);
		audioSources.RDAdd(item);
		inputTimeSansCalibration = base.conductor.startOfNextBar + (double)chakTime;
		releaseTimeSansCalibration = base.conductor.startOfNextBar + (double)num2;
		BeatOneshot beatOneshot = null;
		foreach (Beat beat in base.game.beats)
		{
			if (!(beat is BeatOneshot beatOneshot2) || beat.rowID != base.rowID || Math.Abs(beat.inputTimeSansCalibration - inputTimeSansCalibration) >= 0.001)
			{
				continue;
			}
			if (!isHeldClap && beatOneshot2.isHeldClap)
			{
				beatOneshot = beatOneshot2;
			}
			else if (isHeldClap)
			{
				beatOneshot = this;
			}
			if (beatOneshot == null)
			{
				beatOneshot = beatOneshot2;
			}
			if (beat != this)
			{
				beatOneshot2.subdivCount = subdivCount;
				beatOneshot2.subdivIndex = subdivIndex;
				ref List<BeatOneshot> reference = ref beatOneshot2.friendGroup.friends;
				if (reference == null)
				{
					reference = new List<BeatOneshot> { beatOneshot2 };
				}
				friendGroup = beatOneshot2.friendGroup;
			}
		}
		if (freezeshot)
		{
			friendGroup.hasFreezeshot = true;
			friendGroup.delay = delay;
		}
		if (friends != null)
		{
			if (!freezeshot && friendGroup.hasFreezeshot)
			{
				float delay2 = friendGroup.delay;
				float num7 = base.conductor.DurationBeatsToTime(boomStartBeat, delay2);
				float b2 = base.conductor.DurationBeatsToTime(boomStartBeat, tick - delay2) / 2f;
				rushAnimDuration = Mathf.Min(a, b2);
				rushTime = num - num7 - rushAnimDuration;
				rushTimeAbs = (double)rushTime + base.conductor.startOfNextBar;
			}
			foreach (BeatOneshot friend in friends)
			{
				if (waveOffset != friend.waveOffset && !base.game.currentLevel.oldSubdivFriendBehavior)
				{
					if (waveOffset == 0f)
					{
						waveOffset = friend.waveOffset;
					}
					else if (friend.waveOffset == 0f)
					{
						friend.waveOffset = waveOffset;
					}
				}
				friend.unhittable = friend != beatOneshot;
			}
			unhittable = this != beatOneshot;
			friends.Add(this);
		}
		RowEntity ent = row.ent;
		if (!unhittable && (friends == null || !friends.Exists((BeatOneshot f) => f.addedBeatEvent)))
		{
			scrExecuteOnCertainBeat.Add(() =>
			{
				ent.beatsLeftToHit--;
			}, boomStartBeat + tick + delay);
			scrExecuteOnCertainBeat.Add(() =>
			{
				ent.beatsLeftToHit++;
			}, start);
			addedBeatEvent = true;
		}
		scrExecuteOnCertainBeat.Add(() =>
		{
			row.ent.SwitchRowTypeAppearance(RowType.Oneshot);
		}, start);
		if (!base.game.currentLevel.pauseOn7thBeat || !flag)
		{
			PlayHitSoundIfNoOthersPlaying(chakTime);
			RDGameSoundData[] array = (freezeshot ? freezeshotSoundData : burnshotSoundData);
			RDGameSoundData[] data = new RDGameSoundData[6]
			{
				array[0],
				array[1],
				array[1],
				array[2],
				array[0],
				array[3]
			};
			if (freezeshot)
			{
				if (!secondSubdivisionOrLater)
				{
					float[] soundTimes = new float[6]
					{
						num3,
						num4,
						num,
						num - 0.74f,
						chakTime,
						chakTime
					};
					PlayFreezeshotBurnshotSounds(data, soundTimes, specialOneshotCuesMixerGroup, row);
					ent.SetOverlayTextureAsFreezeshot(1);
					scrExecuteOnCertainBeat.Add(() =>
					{
						ent.targetCharacterRowEnt.character.shaderDataSource = ent;
						ent.targetCharacterRowEnt.iceBlock.shaderDataSource = ent;
						ent.SetOverlayTexturePower(0.8f);
						ent.TweenOverlayTexturePower(0.2f, base.conductor.DurationBeatsToTime(delay));
						ent.targetCharacterRowEnt.subdivisionSpotlightController.speed = 0.5f;
					}, start);
					scrExecuteOnCertainBeat.Add(() =>
					{
						ent.targetCharacterRowEnt.iceBlock.shaderDataSource = ent;
						ent.PlayFreezeAnimation(1f);
						ent.targetCharacterRowEnt.subdivisionSpotlightController.speed = 0f;
						ent.targetCharacterRowEnt.ShowIceBlock(show: true);
						ent.targetCharacterRowEnt.character.customAnimation.Pause();
					}, boomStartBeat + tick);
					scrExecuteOnCertainBeat.Add(() =>
					{
						ent.targetCharacterRowEnt.iceBlock.shaderDataSource = ent;
						ent.PlayFreezeAnimation(1f);
						ent.TweenOverlayTexturePower(0.5f, base.conductor.DurationBeatsToTime(tick / 2f));
						ent.targetCharacterRowEnt.subdivisionSpotlightController.speed = 0.1f;
					}, start + delay);
					scrExecuteOnCertainBeat.Add(() =>
					{
						ent.targetCharacterRowEnt.iceBlock.shaderDataSource = ent;
						ent.SetOverlayTexturePower(0f);
						ent.targetCharacterRowEnt.subdivisionSpotlightController.speed = 1f;
						ent.targetCharacterRowEnt.ShowIceBlock(show: false);
						ent.targetCharacterRowEnt.character.customAnimation.Resume();
					}, boomStartBeat + tick + delay);
				}
				float num8 = ((visualFreezeBeatOverride > 0f) ? (visualFreezeBeatOverride + delayBeatOffset) : (boomStartBeat + tick));
				scrExecuteOnCertainBeat.Add(() =>
				{
					FreezeWaves(frozen: true);
				}, num8);
				scrExecuteOnCertainBeat.Add(() =>
				{
					FreezeWaves(frozen: false);
				}, num8 + delay);
			}
			else if (isBurnshot && !secondSubdivisionOrLater)
			{
				float[] soundTimes2 = new float[6] { num3, num4, boomTime, boomTime, chakTime, chakTime };
				PlayFreezeshotBurnshotSounds(data, soundTimes2, specialOneshotCuesMixerGroup2, row);
				scrExecuteOnCertainBeat.Add(() =>
				{
					ent.targetCharacterRowEnt.character.shaderDataSource = ent;
					ent.SetNoiseTexturePower(0.4f);
					ent.TweenNoiseTexturePower(0.2f, base.conductor.DurationBeatsToTime(tick));
					ent.targetCharacterRowEnt.subdivisionSpotlightController.speed = 1.25f;
				}, start);
				scrExecuteOnCertainBeat.Add(() =>
				{
					ent.SetNoiseTexturePower(0.8f);
					ent.TweenNoiseTexturePower(0.5f, base.conductor.DurationBeatsToTime((boomStartBeat - start) / 2f));
					ent.oneshotRowController.EmitSparks(5);
					ent.oneshotRowController.PlaySparks(play: true);
					ent.targetCharacterRowEnt.subdivisionSpotlightController.speed = 1.5f;
				}, start + tick);
				scrExecuteOnCertainBeat.Add(() =>
				{
					ent.SetNoiseTexturePower(2f);
					ent.oneshotRowController.EmitSparks(15);
					ent.targetCharacterRowEnt.subdivisionSpotlightController.speed = 2f;
				}, boomStartBeat);
				scrExecuteOnCertainBeat.Add(() =>
				{
					ent.SetNoiseTexturePower(0f);
					ent.oneshotRowController.PlaySparks(play: false);
					ent.targetCharacterRowEnt.subdivisionSpotlightController.speed = 1f;
				}, boomStartBeat + tick);
			}
		}
		else
		{
			audioSources.RDAdd(base.conductor.PlayBeat("sndClapHitMassivePreEchoCut1", base.conductor.BeatToTime(boomStartBeat + tick + delay - 1f), row.rowSoundVolMultiplier, hitSoundMixerGroup, 1f, 1f, row.rowSoundPanAdjustedFor2P));
			audioSources.RDAdd(base.conductor.PlayBeat("sndClapHitMassivePreEchoCut2", chakTime, row.rowSoundVolMultiplier, hitSoundMixerGroup, 1f, 1f, row.rowSoundPanAdjustedFor2P));
		}
		if (isHeldClap)
		{
			PlayHeldClapSound(chakTime, num2);
			RDGameSoundData rDGameSoundData = RDGameSounds.Get(GameSoundType.HoldshotSoundCue);
			audioSources.RDAdd(base.conductor.PlayBeat(rDGameSoundData.filename, base.conductor.BeatToTime(boomStartBeat - holdCueOffset), rDGameSoundData.volume * row.rowSoundVolMultiplier, base.SpecialOneshotCuesMixerGroup, rDGameSoundData.minPitch, rDGameSoundData.maxPitch, (row.rowSoundPan == 0f && rDGameSoundData.pan != 0f) ? RDUtils.OverridePanFor2P(row, rDGameSoundData.pan) : row.rowSoundPanAdjustedFor2P));
		}
		skipToFlatAnim = tick <= 0.1f;
		if (skipToFlatAnim)
		{
			phase = 2;
		}
		return this;
		void FreezeWaves(bool frozen)
		{
			RDWave[] allWaves = ent.oneshotRowController.waveManager.allWaves;
			for (int i = 0; i < allWaves.Length; i++)
			{
				allWaves[i].frozen = frozen;
			}
		}
	}

	private void PlayFreezeshotBurnshotSounds(RDGameSoundData[] data, float[] soundTimes, AudioMixerGroup mixerGroup, Row row)
	{
		for (int i = 0; i < data.Length; i++)
		{
			RDGameSoundData rDGameSoundData = data[i];
			float time = soundTimes[i];
			audioSources.RDAdd(base.conductor.PlayBeat(rDGameSoundData.filename, time, rDGameSoundData.volume * row.rowSoundVolMultiplier, mixerGroup, rDGameSoundData.minPitch, rDGameSoundData.maxPitch, (row.rowSoundPan == 0f && rDGameSoundData.pan != 0f) ? RDUtils.OverridePanFor2P(row, rDGameSoundData.pan) : row.rowSoundPanAdjustedFor2P));
		}
	}

	public bool ShouldHaveShadows()
	{
		if (tick > 0f && !friendGroup.hasFreezeshot && wavePeriod < base.gc.minOneshotLengthForShadows && pulseType != OneshotPulseType.Square)
		{
			return pulseType != OneshotPulseType.Triangle;
		}
		return false;
	}

	public void KillSubdivBuddyWeights()
	{
		if (subdivBuddies == null)
		{
			return;
		}
		foreach (BeatOneshot subdivBuddy in subdivBuddies)
		{
			if (!subdivBuddy.weightWasReduced)
			{
				subdivBuddy.weight *= base.game.currentLevel.subdivisionMissWeight;
				subdivBuddy.weightWasReduced = true;
			}
		}
	}

	public override void MoveBackBy(double timeDiff)
	{
		base.MoveBackBy(timeDiff);
		boomTimeAbs -= timeDiff;
		chakTimeAbs -= timeDiff;
		rushTimeAbs -= timeDiff;
	}
}
