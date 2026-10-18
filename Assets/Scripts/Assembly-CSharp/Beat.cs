using System;
using System.Collections.Generic;
using RDLevelEditor;
using UnityEngine;
using UnityEngine.Audio;

public class Beat : RDClass
{
	[Flags]
	internal enum HeldbeatSoundFlag
	{
		ShortEnd = 1,
		Alt = 2,
		P2 = 4,
		Oneshot = 8
	}

	private const float downTime = 0.2f;

	public int playerHitSoundsIndex;

	public AudioMixerGroup heartExplosionsGroup;

	public AudioMixerGroup heartExplosion00Group;

	public AudioMixerGroup heartExplosion01Group;

	public AudioMixerGroup heartExplosion02Group;

	public bool hasHeldPulses;

	public bool isHeldClap;

	public double inputTimeSansCalibration;

	public double releaseTimeSansCalibration;

	public static double[] lastHoldInputTime = new double[2];

	public static double[] lastHoldReleaseTime = new double[2];

	public float weight;

	public int bar;

	public int specialRowToTransferHitTo = -1;

	public Row row;

	public bool isLenientMargins;

	public Action OnDestroyBeat;

	public bool isLastBeat;

	public bool dontPlayMistakeSound;

	public bool dead;

	public bool deadByMistake;

	public bool unhittable;

	public bool hasPulsed7thBeat;

	public bool bomb;

	public List<AudioSource> audioSources = new List<AudioSource>();

	public Dictionary<AudioSource, double> audioTimes;

	public Dictionary<AudioSource, double> audioEndTimes;

	public AudioSource audsrcDrumRoll;

	public AudioSource audsrcReleaseClap;

	public List<AudioSource> audsrcHoldSounds = new List<AudioSource>();

	public double explodeTimeAbs;

	private int autoPressedCount;

	public bool playerDrives7thBeat;

	public double playerDriven7thBeatHitTime;

	public RDPlayer player => row.GetCurrentPlayer();

	public AudioMixerGroup BeatSoundMixerGroup => RDUtils.GetMixerGroup(MixerPath.BeatsoundsRows[rowID]);

	public AudioMixerGroup SpecialOneshotCuesMixerGroup => RDUtils.GetMixerGroup("SpecialOneshotCues");

	public AudioMixerGroup CountingVoiceMixerGroup => RDUtils.GetMixerGroup("CountingVoice");

	public AudioMixerGroup HitSoundMixerGroup => RDUtils.GetMixerGroup(((player == RDPlayer.P2) ? MixerPath.PlayerTwoHitSounds : MixerPath.PlayerOneHitSounds)[playerHitSoundsIndex]);

	public double inputTime
	{
		get
		{
			RDPlayer rDPlayer = player;
			float num = ((rDPlayer == RDPlayer.P1 || rDPlayer == RDPlayer.CPU) ? RDCalibration.calibration_i : RDCalibration.calibration_i_P2);
			return ((playerDrives7thBeat && playerDriven7thBeatHitTime != 0.0) ? playerDriven7thBeatHitTime : inputTimeSansCalibration) + (double)num;
		}
	}

	public double releaseTime
	{
		get
		{
			if (playerDrives7thBeat && playerDriven7thBeatHitTime != 0.0)
			{
				double num = releaseTimeSansCalibration - inputTimeSansCalibration;
				return playerDriven7thBeatHitTime + num;
			}
			RDPlayer rDPlayer = player;
			float num2 = ((rDPlayer == RDPlayer.P1 || rDPlayer == RDPlayer.CPU) ? RDCalibration.calibration_i : RDCalibration.calibration_i_P2);
			return releaseTimeSansCalibration + (double)num2;
		}
	}

	public int rowID
	{
		get
		{
			return row.id;
		}
		set
		{
			row = base.game.rows[value];
		}
	}

	protected virtual RDGameSoundData hitSoundData
	{
		get
		{
			GameSoundType type = GameSoundType.ClapSoundCPUClassic;
			switch (row.GetPlayerWeAreGoingToChangeInto())
			{
			case RDPlayer.P1:
				type = GameSoundType.ClapSoundP1Classic;
				break;
			case RDPlayer.P2:
				type = GameSoundType.ClapSoundP2Classic;
				break;
			}
			return RDGameSounds.Get(type);
		}
	}

	private string beatHitSound
	{
		get
		{
			if (!(row.rowSoundHitOverride != ""))
			{
				return hitSoundData.filename;
			}
			return row.rowSoundHitOverride;
		}
	}

	private float beatHitSoundMultiplier
	{
		get
		{
			if (!(row.rowSoundHitOverride != ""))
			{
				return hitSoundData.volume;
			}
			return row.rowSoundHitOverrideMultiplier;
		}
	}

	public static implicit operator bool(Beat beat)
	{
		return beat != null;
	}

	public Beat()
	{
		bar = base.conductor.barNumberMonotonic;
		audioTimes = new Dictionary<AudioSource, double>();
		audioEndTimes = new Dictionary<AudioSource, double>();
		isHeldClap = false;
		dead = false;
		deadByMistake = false;
		hasPulsed7thBeat = false;
		heartExplosionsGroup = RDUtils.GetMixerGroup("HeartExplosionsParent");
		heartExplosion00Group = RDUtils.GetMixerGroup("HeartExplosion00");
		heartExplosion01Group = RDUtils.GetMixerGroup("HeartExplosion01");
		heartExplosion02Group = RDUtils.GetMixerGroup("HeartExplosion02");
	}

	public void RefreshMistakeWeight()
	{
		float mistakeWeight = base.game.currentLevel.mistakeWeight;
		weight = mistakeWeight * row.mistakeWeight;
	}

	public virtual void Update()
	{
		if (dead)
		{
			return;
		}
		if (base.conductor.audioPos > inputTime && this.row.ent.hitstripCenter != null)
		{
			RDClass.Vfx.rooms[this.row.room].BeatboxPulsed(this.row.ent.hitstripCenter.position, 7);
		}
		bool flag = base.conductor.audioPos > inputTime + 0.4000000059604645;
		if (this.row.playerBox == null || this.row.ent == null || this.row.dead)
		{
			if (flag)
			{
				DestroyBeat();
				printe("Beat deleted cause no corresponding rowProperty");
			}
			return;
		}
		if (bomb & flag)
		{
			base.game.AddHitOffset(rowID, OffsetType.Perfect);
			DestroyBeat();
			return;
		}
		if (flag && !playerDrives7thBeat && !hasPulsed7thBeat && !bomb)
		{
			DestroyBeat();
			if (unhittable)
			{
				return;
			}
			if (LEDSign.showMarginError)
			{
				base.game?.statusText.SetStatusText(RDString.Get("status.wayTooLateFeedback"), null, 4f, narrate: true);
			}
			if (Time.frameCount > this.row.lastMissedFrame && ((!RDClass.debugSettings.Auto && !this.row.cpuControlled) || (this.row.cpuControlled && this.row.playerBox.clumsyCPUTiming != 0f)))
			{
				this.row.ent.ExpressionPlusFX("missed");
				base.game.AddHitOffset(rowID, OffsetType.Missed);
				base.game.OnMistakeOrHeal(0.4f, weight, this.row);
				this.row.ent.CrackAdvance(weight);
				base.game.mistakesManager.UpdateConsecutiveMistakes(player, OffsetType.Missed);
				if (this is BeatOneshot)
				{
					((BeatOneshot)this).KillSubdivBuddyWeights();
				}
				bool flag2 = false;
				Row row = this.row;
				if (row.isShadowRealtime && row.shadowOrHostIDRealtime != -1)
				{
					flag2 = true;
				}
				if (!flag2 || row.isShadowRealtime || base.game.rows[row.shadowOrHostIDRealtime].GetCurrentPlayer() != player)
				{
					string groupPath = "MistakesParent";
					if (player == RDPlayer.P1)
					{
						groupPath = "PlayerOneMistakes";
					}
					else if (player == RDPlayer.P2)
					{
						groupPath = "PlayerTwoMistakes";
					}
					if (!dontPlayMistakeSound)
					{
						scrConductor.PlayFeedback(GameSoundType.BigMistake, 1.25f, RDUtils.GetMixerGroup(groupPath), 0.8f, player);
					}
				}
				base.game.FlashBorderFeedback(correct: false, this.row);
				this.row.lastMissedFrame = Time.frameCount;
			}
			foreach (scrExecuteOnHit item in RDExecuteOn.FindAll<scrExecuteOnHit>())
			{
				bool flag3 = false;
				if ((item.hitType == HitType.MissCompletely || item.hitType == HitType.AnyMiss) && item.isActive && (item.rowID == -1 || item.rowID == rowID))
				{
					item.Execute(this);
					flag3 = true;
				}
				if ((!item.persistent & flag3) || item.autodelete)
				{
					item.Remove();
				}
			}
			RunEventsTaggedOnMiss(rowID);
			base.game.currentLevel.OnHit(HitType.BigMiss, this);
		}
		if (this.row.cpuControlled && base.conductor.audioPos > inputTime + (double)this.row.playerBox.clumsyCPUTiming && !bomb)
		{
			Character cpuCharacter = this.row.cpuCharacter;
			base.game.handController0.cpuHandPress(cpuCharacter);
			base.game.handController1.cpuHandPress(cpuCharacter);
			base.game.handController2.cpuHandPress(cpuCharacter);
			base.game.handController3.cpuHandPress(cpuCharacter);
			base.game.handControllerOnTop.cpuHandPress(cpuCharacter);
			this.row.playerBox.SpaceBarEvent(CPUTriggered: true);
		}
		if (base.game.BeatIsBeingHeldByPlayer(this.row.GetCurrentPlayer()) && base.conductor.audioPos >= inputTime && this.row.ent.playerBox.currentHoldBeat != null && this.row.ent.playerBox.currentHoldBeat.inputTime <= inputTime && !hasPulsed7thBeat)
		{
			this.row.ent.playerBox.ReleasePop();
		}
	}

	public static void RunEventsTaggedOnMiss(int rowID)
	{
		LevelBase currentLevel = scnGame.instance.currentLevel;
		if (currentLevel.data != null)
		{
			Dictionary<string, List<LevelEvent_Base>> dict = currentLevel.taggedEvents.Filter((KeyValuePair<string, List<LevelEvent_Base>> kvp) => !kvp.Key.Contains("[row"));
			currentLevel.RunTagContainingGeneric(dict, true, "[onMiss]");
			currentLevel.RunTagContaining(true, "[onMiss]", "[row" + rowID + "]");
		}
	}

	private void ReleaseIfClear(RDPlayer player, bool heldbeat)
	{
		if (heldbeat)
		{
			base.game.autoHeldBeatCounter--;
		}
		if (base.game.autoHeldBeatCounter <= 0)
		{
			RDInput.emuStates[(int)player].SetKey(RDInput.PlayerEmuKey.Up);
		}
	}

	public virtual void LateUpdate()
	{
		if (!RDClass.debugSettings.Auto && player != RDPlayer.CPU)
		{
			RDInput.PlayerEmuState playerEmuState = RDInput.emuStates[(int)player];
			bool flag = false;
			Row[] rows = base.game.rows;
			foreach (Row row in rows)
			{
				if (player == row.GetCurrentPlayer() && !(row.playerBox == null) && (bool)row.playerBox.currentHoldBeat)
				{
					flag = true;
					if ((bool)this.row.playerBox.currentHoldBeat && this.row.playerBox.currentHoldBeat.releaseTime > lastHoldReleaseTime[(int)player])
					{
						lastHoldReleaseTime[(int)player] = this.row.playerBox.currentHoldBeat.releaseTime;
						lastHoldInputTime[(int)player] = this.row.playerBox.currentHoldBeat.inputTime;
					}
					break;
				}
			}
			if (flag)
			{
				if (!this.row.playerBox.currentHoldBeat && !isHeldClap && base.conductor.audioPos >= inputTime && lastHoldReleaseTime[(int)player] >= inputTime && lastHoldInputTime[(int)player] <= inputTime)
				{
					this.row.playerBox.Pulse((float)(base.conductor.audioPos - inputTime), this, CPUTriggered: true);
					base.game.FlashBorderFeedback(correct: true, this.row);
					Create8thBeat();
				}
				if (isHeldClap && base.conductor.audioPos >= releaseTime + 0.4000000059604645)
				{
					playerEmuState.SetKey(RDInput.PlayerEmuKey.Up);
				}
			}
		}
		if (RDClass.debugSettings.Auto && !bomb && player != RDPlayer.CPU)
		{
			RDInput.PlayerEmuState playerEmuState2 = RDInput.emuStates[(int)player];
			if (base.conductor.audioPos > inputTime && autoPressedCount < 1)
			{
				autoPressedCount++;
				if (!this.row.playerBox.currentHoldBeat)
				{
					playerEmuState2.SetKey(RDInput.PlayerEmuKey.Down);
				}
				if (!isHeldClap)
				{
					Timer.Add(() =>
					{
						ReleaseIfClear(player, heldbeat: false);
					}, 0.2f);
				}
				if (isHeldClap)
				{
					base.game.autoHeldBeatCounter++;
				}
			}
			if (base.conductor.audioPos > releaseTime && isHeldClap)
			{
				base.game.autoHeldBeatCounter--;
				if (base.game.autoHeldBeatCounter == 0)
				{
					ReleaseIfClear(player, heldbeat: true);
				}
			}
		}
		if (dead)
		{
			DestroyBeat(killSounds: false, evenIfHalfwayPlaying: false);
		}
	}

	public void DestroyBeatIfNotHeldBeat(bool killSounds = false, bool evenIfHalfwayPlaying = true)
	{
		if (hasHeldPulses)
		{
			hasPulsed7thBeat = true;
		}
		else
		{
			DestroyBeat(killSounds, evenIfHalfwayPlaying);
		}
	}

	public void CutOffHeldSounds()
	{
		if (audsrcDrumRoll != null)
		{
			audsrcDrumRoll.Stop();
		}
		if (audsrcReleaseClap != null)
		{
			audsrcReleaseClap.Stop();
		}
		foreach (AudioSource audsrcHoldSound in audsrcHoldSounds)
		{
			if (audsrcHoldSound != null)
			{
				audsrcHoldSound.Stop();
			}
		}
		audsrcHoldSounds.Clear();
	}

	public void DestroyBeat(bool killSounds = false, bool evenIfHalfwayPlaying = true, bool cutHeldSounds = true, int _bar = -1)
	{
		if (_bar != -1 && bar != _bar)
		{
			return;
		}
		if (this is BeatClassic)
		{
			for (int i = 0; i < (this as BeatClassic).beatboxStatuses.Length; i++)
			{
				(this as BeatClassic).beatboxStatuses[i] = BeatboxStatus.Flat;
			}
		}
		dead = true;
		if (row.ent != null && this is BeatClassic)
		{
			scrBeatbox[] beatboxes = row.ent.classicRowController.beatboxes;
			for (int j = 0; j < beatboxes.Length; j++)
			{
				beatboxes[j].RefreshPulseStatus();
			}
		}
		if (killSounds)
		{
			foreach (AudioSource audioSource in audioSources)
			{
				if (audioSource != null && (evenIfHalfwayPlaying || !audioSource.IsActuallyPlayingNotJustScheduled()) && audioSource != audsrcDrumRoll && audioSource != audsrcReleaseClap)
				{
					audioSource.Stop();
				}
			}
		}
		if (cutHeldSounds)
		{
			CutOffHeldSounds();
		}
		if (playerDrives7thBeat)
		{
			scrConductor.PlayImmediately(beatHitSound, beatHitSoundMultiplier, BeatSoundMixerGroup);
		}
		if (OnDestroyBeat != null)
		{
			OnDestroyBeat();
		}
		hasPulsed7thBeat = true;
		base.game.beats.Remove(this);
	}

	public void KillSounds()
	{
		foreach (AudioSource audioSource in audioSources)
		{
			if (audioSource != null && audioSource != audsrcDrumRoll && audioSource != audsrcReleaseClap)
			{
				audioSource.Stop();
			}
		}
	}

	public Beat SetOnDestroy(Action _OnDestroy)
	{
		OnDestroyBeat = _OnDestroy;
		return this;
	}

	public Beat SetLastBeat()
	{
		isLastBeat = true;
		return this;
	}

	public virtual void Create8thBeat()
	{
		if (base.game.currentLevel.heartExplodeType == HeartExplodeType.Disabled)
		{
			return;
		}
		HeartExplodeType heartExplodeType = base.game.currentLevel.heartExplodeType;
		if (playerDrives7thBeat)
		{
			heartExplodeType = HeartExplodeType.Instant;
		}
		if (heartExplodeType == HeartExplodeType.Instant)
		{
			return;
		}
		foreach (Beat beat in base.game.beats)
		{
			if (beat != null && (heartExplodeType == HeartExplodeType.GatherAndCeil || heartExplodeType == HeartExplodeType.GatherNoCeil) && beat.explodeTimeAbs > explodeTimeAbs && beat.explodeTimeAbs <= explodeTimeAbs + (double)base.conductor.crotchet + 0.20000000298023224)
			{
				explodeTimeAbs = beat.explodeTimeAbs;
			}
		}
		scrExecuteOnCertainTime.Add(base.game.rows[rowID].ent.heart.Explode, explodeTimeAbs);
		int num = 1;
		foreach (KeyValuePair<AudioSource, double> explosionAudioSource in base.conductor.explosionAudioSources)
		{
			if (!(explosionAudioSource.Key == null) && RDUtils.WithinSmallMargin(explosionAudioSource.Value, explodeTimeAbs))
			{
				explosionAudioSource.Key.Stop();
				num++;
			}
		}
		RDGameSoundData rDGameSoundData;
		AudioMixerGroup audioMixerGroup;
		if (base.gc.playExplosionFileVariations)
		{
			switch (num)
			{
			case 1:
				rDGameSoundData = RDGameSounds.Get(GameSoundType.HeartExplosion);
				audioMixerGroup = heartExplosion00Group;
				break;
			case 2:
				rDGameSoundData = RDGameSounds.Get(GameSoundType.HeartExplosion2);
				audioMixerGroup = heartExplosion01Group;
				break;
			default:
				rDGameSoundData = RDGameSounds.Get(GameSoundType.HeartExplosion3);
				audioMixerGroup = heartExplosion02Group;
				break;
			}
		}
		else
		{
			rDGameSoundData = RDGameSounds.Get(GameSoundType.HeartExplosion);
			audioMixerGroup = heartExplosionsGroup;
		}
		float volume = rDGameSoundData.volume;
		AudioSource audioSource = base.conductor.PlayAbs(rDGameSoundData.filename, explodeTimeAbs, volume * base.game.currentLevel.heartExplodeMultiplier, isBeatSound: true, audioMixerGroup, rDGameSoundData.maxPitch, rDGameSoundData.pan);
		audioSources.RDAdd(audioSource);
		audioTimes.RDAdd(audioSources.GetLastObject(), base.conductor.lastScheduledTime);
		if (base.gc.playExplosionPitchVariations)
		{
			audioSource.pitch = row.rowExplosionPitch;
		}
		base.conductor.explosionAudioSources.Add(audioSource, explodeTimeAbs);
	}

	public void SetWeight(float weight)
	{
		this.weight = weight;
	}

	public Beat SetTransferRow(int _row)
	{
		specialRowToTransferHitTo = _row;
		return this;
	}

	public void PlayHitSoundIfNoOthersPlaying(float time, bool fromSongStart = false, bool isReleaseSound = false)
	{
		bool flag = true;
		foreach (Beat beat in base.game.beats)
		{
			if (Math.Abs(beat.inputTime - inputTime) < 0.009999999776482582 && this != beat && beat.beatHitSound == beatHitSound && beat.player == player && base.gc.dontplaysametime)
			{
				flag = false;
				break;
			}
		}
		int num = 0;
		foreach (Beat beat2 in base.game.beats)
		{
			if (Math.Abs(beat2.inputTime - inputTime) < 0.009999999776482582 && this != beat2 && row.GetPlayerWeAreGoingToChangeInto() == beat2.row.GetPlayerWeAreGoingToChangeInto())
			{
				dontPlayMistakeSound = true;
				break;
			}
			num++;
		}
		AudioSource audioSource = null;
		if (!flag || beatHitSound == null)
		{
			return;
		}
		if (fromSongStart)
		{
			audioSources.RDAdd(base.conductor.PlayBeatFromSongStart(beatHitSound, time, beatHitSoundMultiplier, HitSoundMixerGroup));
			if (audioSources.GetLastObject() != null)
			{
				audioTimes.RDAdd(audioSources.GetLastObject(), base.conductor.lastScheduledTime);
			}
		}
		else
		{
			audioSource = base.conductor.PlayBeat(beatHitSound, time, beatHitSoundMultiplier, HitSoundMixerGroup, hitSoundData.minPitch, hitSoundData.maxPitch, RDUtils.OverridePanFor2P(row, hitSoundData.pan), 0f, useGlobalBeatSound: true, isGameplayBeat: true);
			audioSources.RDAdd(audioSource);
			if (audioSource != null)
			{
				audioTimes.RDAdd(audioSource, base.conductor.lastScheduledTime);
			}
		}
		if (isReleaseSound && audioSource != null)
		{
			audsrcReleaseClap = audioSource;
		}
	}

	internal void PlayHeldClapSound(float holdStart, float holdEnd)
	{
		float num = holdEnd - holdStart;
		bool flag = this is BeatOneshot;
		bool flag2 = true;
		bool flag3 = true;
		foreach (Beat beat in base.game.beats)
		{
			bool flag4 = beat.isHeldClap && this != beat && beat.player == player && beat is BeatOneshot == flag && base.gc.dontplaysametime;
			if ((flag2 & flag4) && Math.Abs(beat.inputTime - inputTime) < 0.009999999776482582)
			{
				if (beat.releaseTime >= releaseTime)
				{
					flag2 = false;
				}
				else if (beat.audsrcDrumRoll != null && beat.audsrcDrumRoll.isPlaying)
				{
					beat.audsrcDrumRoll.Stop();
				}
			}
			if ((flag3 & flag4) && Math.Abs(beat.releaseTime - releaseTime) < 0.009999999776482582)
			{
				flag3 = false;
			}
		}
		HeldbeatSoundFlag heldbeatSoundFlag = (HeldbeatSoundFlag)0;
		if (player == RDPlayer.P2)
		{
			heldbeatSoundFlag |= HeldbeatSoundFlag.P2;
		}
		if (flag)
		{
			heldbeatSoundFlag |= HeldbeatSoundFlag.Oneshot;
		}
		GameSoundType gst = GameSoundType.ClapSoundHoldLongEnd;
		switch (heldbeatSoundFlag)
		{
		case HeldbeatSoundFlag.P2:
			gst = GameSoundType.ClapSoundHoldLongEndP2;
			break;
		case HeldbeatSoundFlag.Oneshot:
		case HeldbeatSoundFlag.P2 | HeldbeatSoundFlag.Oneshot:
			gst = GameSoundType.HoldshotSoundClapLongEnd;
			break;
		}
		if (num < RDUtils.GetOffsetForSoundType(gst))
		{
			heldbeatSoundFlag |= HeldbeatSoundFlag.ShortEnd;
		}
		GameSoundType gst2 = GameSoundType.ClapSoundHoldLongStart;
		GameSoundType gst3 = GameSoundType.ClapSoundHoldLongEnd;
		switch (heldbeatSoundFlag)
		{
		case HeldbeatSoundFlag.ShortEnd:
			gst2 = GameSoundType.ClapSoundHoldShortStart;
			gst3 = GameSoundType.ClapSoundHoldShortEnd;
			break;
		case HeldbeatSoundFlag.P2:
			gst2 = GameSoundType.ClapSoundHoldLongStartP2;
			gst3 = GameSoundType.ClapSoundHoldLongEndP2;
			break;
		case HeldbeatSoundFlag.ShortEnd | HeldbeatSoundFlag.P2:
			gst2 = GameSoundType.ClapSoundHoldShortStartP2;
			gst3 = GameSoundType.ClapSoundHoldShortEndP2;
			break;
		case HeldbeatSoundFlag.Oneshot:
		case HeldbeatSoundFlag.P2 | HeldbeatSoundFlag.Oneshot:
			gst2 = GameSoundType.HoldshotSoundClapStart;
			gst3 = GameSoundType.HoldshotSoundClapLongEnd;
			break;
		case HeldbeatSoundFlag.ShortEnd | HeldbeatSoundFlag.Oneshot:
		case HeldbeatSoundFlag.ShortEnd | HeldbeatSoundFlag.P2 | HeldbeatSoundFlag.Oneshot:
			gst2 = GameSoundType.HoldshotSoundClapStart;
			gst3 = GameSoundType.HoldshotSoundClapShortEnd;
			break;
		}
		if (flag2)
		{
			playerHitSoundsIndex = 2;
			AudioSource audioSource = audioSources.RDAdd(base.conductor.PlayBeat(gst2, holdStart, HitSoundMixerGroup, hitSoundData.minPitch, hitSoundData.maxPitch, RDUtils.OverridePanFor2P(row, hitSoundData.pan), hitSoundData.volume, 0f, isGameplayBeat: true));
			audioTimes.RDAdd(audioSource, base.conductor.lastScheduledTime);
			audioSource.SetScheduledEndTime((double)holdEnd + base.conductor.startOfNextBar);
			audioEndTimes.Add(audioSource, (double)holdEnd + base.conductor.startOfNextBar);
			audsrcDrumRoll = audioSource;
		}
		if (flag3)
		{
			playerHitSoundsIndex = 3;
			AudioSource item = audioSources.RDAdd(base.conductor.PlayBeat(gst3, holdEnd, HitSoundMixerGroup, hitSoundData.minPitch, hitSoundData.maxPitch, RDUtils.OverridePanFor2P(row, hitSoundData.pan), hitSoundData.volume, 0f, isGameplayBeat: true));
			audioTimes.RDAdd(item, base.conductor.lastScheduledTime);
			audsrcReleaseClap = item;
		}
	}

	public virtual void MoveBackBy(double time)
	{
		inputTimeSansCalibration -= time;
		releaseTimeSansCalibration -= time;
		explodeTimeAbs -= time;
		if (inputTimeSansCalibration < base.conductor.audioPos)
		{
			dead = true;
		}
	}
}
