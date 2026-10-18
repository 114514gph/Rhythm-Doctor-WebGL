using System;
using System.Collections.Generic;
using RDLevelEditor;
using RDTools;
using UnityEngine;
using UnityEngine.Audio;

public class BeatClassic : Beat
{
	public class SingleBeat
	{
		public int beatboxNumber;

		public double timeAbs;

		public double timeReleaseAbs;

		public double timeToNextBeat;

		public double timeToNextBeatExcludingHold;

		public BananaBend bend;

		public AudioSource audioSource;
	}

	private const int DestroyBeatNumber = -99;

	private const int BombBeatNumber = 0;

	public float defaultTick;

	public float swing;

	public float offset;

	public float tick;

	public SwingBeatType swingType;

	public bool affected6thBeatboxFlag;

	public List<SingleBeat> beats = new List<SingleBeat>();

	public BeatboxStatus[] beatboxStatuses = new BeatboxStatus[6];

	public int currentActivatedArrayPosition;

	public int currentBeatbox;

	public bool justPulsed;

	public bool shouldHeldbeatPulseBeAlt;

	public LevelEvent_AddFreeTimeBeat freeTimeEvent;

	private bool firstFreeTimeExecuted;

	public bool shouldCallRefresh;

	public BeatClassic(int rowID)
	{
		base.rowID = rowID;
		RefreshMistakeWeight();
		SetTransferRow(base.game.rows[rowID].transferTarget);
	}

	public BeatClassic Add7BeatsClassic(float _startbeat, float _tick, SwingBeatType swingType = SwingBeatType.Normal, float customswing = 0f, float holdTimeInCrotchets = 0f, int syncoBeat = -1, float syncoSwing = 0f, int length = 7, SoundData customBeatsound = null)
	{
		List<float> list = new List<float>();
		List<float> list2 = new List<float>();
		offset = base.conductor.BeatToTime(_startbeat);
		tick = _tick;
		defaultTick = _tick;
		swing = customswing;
		this.swingType = swingType;
		bool flag = syncoBeat >= 0 && syncoBeat <= 5;
		for (int i = 1; i - 1 < length; i++)
		{
			float num = 0f;
			if (flag && i - 1 > syncoBeat)
			{
				num = ((!(syncoSwing > 0f)) ? ((0f - tick) * 0.5f) : ((0f - tick) * syncoSwing));
			}
			float num2 = tick * (float)(i - 1) + _startbeat;
			if (swingType != SwingBeatType.Normal)
			{
				if (swingType == SwingBeatType.SwingCustom)
				{
					if (Mathf.Approximately(Mathf.Round(_startbeat / tick + (float)(i - 1)) % 2f, 1f))
					{
						num2 = tick * (float)i + _startbeat - customswing;
					}
				}
				else if (swingType == SwingBeatType.SwingCustomNew)
				{
					if (i % 2 == 0)
					{
						num2 = _startbeat + tick * (float)i - customswing;
					}
				}
				else if (swingType == SwingBeatType.SwingCustomReverse)
				{
					num2 = ((i % 2 != 1) ? (tick * (float)i + _startbeat) : (tick * (float)(i + 1) + _startbeat - customswing));
				}
				else if (swingType == SwingBeatType.SwingSimpleThird && i % 2 == 0)
				{
					num2 = tick * (float)i + _startbeat - 1f / 3f;
				}
				else if (swingType == SwingBeatType.SwingSimpleQuarter && i % 2 == 0)
				{
					num2 = tick * (float)i + _startbeat - 0.25f;
				}
				else if (swingType == SwingBeatType.SwingSimpleCustom && i % 2 == 0)
				{
					num2 = tick * (float)i + _startbeat - customswing;
				}
				else if (swingType == SwingBeatType.SwingDefineSpacing && i % 2 == 0)
				{
					num2 = tick * (float)(i - 1) + _startbeat + customswing;
				}
			}
			float num3 = base.conductor.BeatToTime(num2 + num);
			if (num3 >= base.conductor.crotchet * 6f && base.game.currentLevel.pauseOn7thBeat)
			{
				num3 += base.conductor.crotchet;
			}
			list.Add(num3);
			list2.Add(base.conductor.BeatToTime(num2 + holdTimeInCrotchets + num));
		}
		int[] array = new int[length];
		for (int j = 0; j < length; j++)
		{
			array[j] = 8 - length + j;
		}
		Add7BeatsFreetimeFlexible(array, list.ToArray(), list2.ToArray(), _playerDrives7thBeat: false, _isLenientMargins: false, syncoBeat, customBeatsound);
		return this;
	}

	public BeatClassic Add7BeatsFreetime(float[] arrTimes, bool _playerDrives7thBeat = false, bool isLenientMargins = false)
	{
		float[] array = new float[7];
		for (int i = 0; i < 7; i++)
		{
			array[i] = (float)((double)(arrTimes[i] / RDTime.speed) - (base.conductor.startOfNextBar - base.conductor.startOfSong));
		}
		return Add7BeatsFreetimeFlexible(new int[7] { 1, 2, 3, 4, 5, 6, 7 }, array, null, _playerDrives7thBeat, isLenientMargins);
	}

	public BeatClassic Add7BeatsFreetimeFlexible(int[] _arrBeatboxNumbers, float[] _arrTimesFromStartOfBar, float[] _arrTimesReleaseFromStartOfBar, bool _playerDrives7thBeat = false, bool _isLenientMargins = false, int syncoBeat = -1, SoundData customBeatsound = null)
	{
		if (_arrBeatboxNumbers[0] == 0 || (_arrBeatboxNumbers[0] == 7 && base.game.currentLevel.bombBeats))
		{
			bomb = true;
			_arrBeatboxNumbers[0] = 7;
		}
		if (_arrTimesReleaseFromStartOfBar != null)
		{
			for (int i = 0; i < _arrTimesReleaseFromStartOfBar.Length; i++)
			{
				if (!RDUtils.WithinSmallMargin(_arrTimesReleaseFromStartOfBar[i], _arrTimesFromStartOfBar[i]))
				{
					if (_arrBeatboxNumbers[i] == 7)
					{
						isHeldClap = true;
					}
					else
					{
						hasHeldPulses = true;
					}
				}
			}
		}
		else
		{
			hasHeldPulses = false;
			_arrTimesReleaseFromStartOfBar = _arrTimesFromStartOfBar;
		}
		isLenientMargins = _isLenientMargins;
		bool flag = _playerDrives7thBeat || base.game.currentLevel.playerDrives7thBeat;
		if (flag)
		{
			isLenientMargins = true;
		}
		tick = Mathf.Min(tick, 1f);
		playerDrives7thBeat = flag;
		double num = 99999.0;
		inputTimeSansCalibration = 99999.0;
		for (int j = 0; j < _arrTimesFromStartOfBar.Length; j++)
		{
			bool flag2 = j >= _arrTimesFromStartOfBar.Length - 1;
			SingleBeat item = new SingleBeat
			{
				beatboxNumber = _arrBeatboxNumbers[j],
				timeAbs = base.conductor.startOfNextBar + (double)_arrTimesFromStartOfBar[j],
				timeReleaseAbs = base.conductor.startOfNextBar + (double)_arrTimesReleaseFromStartOfBar[j],
				timeToNextBeat = ((!flag2) ? (_arrTimesFromStartOfBar[j + 1] - _arrTimesReleaseFromStartOfBar[j]) : 0f),
				timeToNextBeatExcludingHold = ((!flag2) ? (_arrTimesFromStartOfBar[j + 1] - _arrTimesFromStartOfBar[j]) : 0f)
			};
			beats.Add(item);
		}
		BananaBeatsCalculation();
		for (int k = 0; k < beats.Count; k++)
		{
			int num2 = beats[k].beatboxNumber;
			if (num2 == -99)
			{
				break;
			}
			if (num2 == 0)
			{
				bomb = true;
				num2 = 7;
			}
			bool flag3 = num2 == 7;
			bool flag4 = !flag3 && row.show[num2 - 1];
			bool flag5 = flag4 || (flag3 && !playerDrives7thBeat);
			bool audibleSkipBeats = Persistence.GetAudibleSkipBeats();
			if (num2 == 0)
			{
				flag5 = false;
				if (k != beats.Count - 1)
				{
					Debug.LogWarning("0 found in NOT the last position. this shouldnt be possible");
				}
			}
			if (bomb)
			{
				flag5 = false;
			}
			AudioMixerGroup beatSoundMixerGroup = base.BeatSoundMixerGroup;
			AudioMixerGroup countingVoiceMixerGroup = base.CountingVoiceMixerGroup;
			float syncoPitch = 1f;
			if (base.game.currentLevel.useSyncoPitchShift && syncoBeat >= 0 && syncoBeat <= 5)
			{
				syncoPitch = ((k <= syncoBeat) ? 0.8333f : 1.2233f);
			}
			if (flag5)
			{
				if (!hasHeldPulses)
				{
					MakeNonHeldBeatSound(_arrTimesFromStartOfBar, k, num2, 1f, customBeatsound, syncoPitch);
				}
				if (hasHeldPulses)
				{
					if (!RDUtils.WithinSmallMargin(_arrTimesReleaseFromStartOfBar[k], _arrTimesFromStartOfBar[k]))
					{
						if (!flag3)
						{
							float num3 = _arrTimesReleaseFromStartOfBar[k] - _arrTimesFromStartOfBar[k];
							if (k == 0)
							{
								shouldHeldbeatPulseBeAlt = scrConductor.BeatsoundPitchMode;
							}
							bool flag6 = row.GetPlayerWeAreGoingToChangeInto() == RDPlayer.P2;
							GameSoundType gst = (flag6 ? GameSoundType.PulseSoundHoldEndP2 : GameSoundType.PulseSoundHoldEnd);
							HeldbeatSoundFlag heldbeatSoundFlag = (HeldbeatSoundFlag)0;
							if (num3 < RDUtils.GetOffsetForSoundType(gst))
							{
								heldbeatSoundFlag |= HeldbeatSoundFlag.ShortEnd;
							}
							if (shouldHeldbeatPulseBeAlt)
							{
								heldbeatSoundFlag |= HeldbeatSoundFlag.Alt;
							}
							if (flag6)
							{
								heldbeatSoundFlag |= HeldbeatSoundFlag.P2;
							}
							object obj = heldbeatSoundFlag switch
							{
								HeldbeatSoundFlag.ShortEnd => (GameSoundType.PulseSoundHoldShortEnd, GameSoundType.PulseSoundHoldStart), 
								HeldbeatSoundFlag.Alt => (GameSoundType.PulseSoundHoldEndAlt, GameSoundType.PulseSoundHoldStartAlt), 
								HeldbeatSoundFlag.ShortEnd | HeldbeatSoundFlag.Alt => (GameSoundType.PulseSoundHoldShortEndAlt, GameSoundType.PulseSoundHoldStartAlt), 
								HeldbeatSoundFlag.P2 => (GameSoundType.PulseSoundHoldEndP2, GameSoundType.PulseSoundHoldStartP2), 
								HeldbeatSoundFlag.ShortEnd | HeldbeatSoundFlag.P2 => (GameSoundType.PulseSoundHoldShortEndP2, GameSoundType.PulseSoundHoldStartP2), 
								HeldbeatSoundFlag.Alt | HeldbeatSoundFlag.P2 => (GameSoundType.PulseSoundHoldEndAltP2, GameSoundType.PulseSoundHoldStartAltP2), 
								HeldbeatSoundFlag.ShortEnd | HeldbeatSoundFlag.Alt | HeldbeatSoundFlag.P2 => (GameSoundType.PulseSoundHoldShortEndAltP2, GameSoundType.PulseSoundHoldStartAltP2), 
								_ => (GameSoundType.PulseSoundHoldEnd, GameSoundType.PulseSoundHoldStart), 
							};
							GameSoundType item2 = ((ValueTuple<GameSoundType, GameSoundType>)obj).Item1;
							GameSoundType item3 = ((ValueTuple<GameSoundType, GameSoundType>)obj).Item2;
							float num4 = (shouldHeldbeatPulseBeAlt ? Mathf.Sqrt(scrConductor.BeatsoundPitchVarianceMultiplier) : 1f);
							shouldHeldbeatPulseBeAlt = !shouldHeldbeatPulseBeAlt;
							RDGameSoundData rDGameSoundData = RDGameSounds.Get(item3);
							RDGameSoundData rDGameSoundData2 = RDGameSounds.Get(item2);
							AudioSource audioSource = audioSources.RDAdd(base.conductor.PlayBeat(item3, _arrTimesFromStartOfBar[k], beatSoundMixerGroup, row.rowSoundMinPitch * num4, row.rowSoundMaxPitch * num4, (row.rowSoundPan == 0f && rDGameSoundData.pan != 0f) ? rDGameSoundData.pan : row.rowSoundPanAdjustedFor2P, row.rowSoundVolMultiplier, 0f, isGameplayBeat: true));
							AudioSource item4 = audioSources.RDAdd(base.conductor.PlayBeat(item2, _arrTimesReleaseFromStartOfBar[k], beatSoundMixerGroup, row.rowSoundMinPitch * num4, row.rowSoundMaxPitch * num4, (row.rowSoundPan == 0f && rDGameSoundData2.pan != 0f) ? rDGameSoundData2.pan : row.rowSoundPanAdjustedFor2P, row.rowSoundVolMultiplier, 0f, isGameplayBeat: true));
							audioTimes.RDAdd(item4, base.conductor.lastScheduledTime);
							audsrcHoldSounds.Add(audioSource);
							audsrcHoldSounds.Add(item4);
							if (audioSource != null)
							{
								audioSource.SetScheduledEndTime(beats[k].timeReleaseAbs);
							}
						}
					}
					else
					{
						float alternatingBeatPitchMultiplier = GetAlternatingBeatPitchMultiplier(k);
						AudioSource item5 = base.conductor.PlayBeat(row.pulseSounds[k].filename, 1f, _arrTimesFromStartOfBar[k], beatSoundMixerGroup, alternatingBeatPitchMultiplier, alternatingBeatPitchMultiplier, row.pulseSounds[k].panPercentage, 0f, useGlobalBeatSound: true, isGameplayBeat: true);
						audioSources.RDAdd(item5);
					}
				}
				if (base.game.nurseCount || row.rowCountingVoiceEnabled)
				{
					if (base.game.nurseCount)
					{
						row.countingSounds = new SoundData[7];
						row.SetCountingSounds(CountingVoiceSource.JyiCount);
					}
					SoundData soundData = row.countingSounds[num2 - 1];
					audioSources.RDAdd(base.conductor.PlayBeat(soundData.filename, _arrTimesFromStartOfBar[k], soundData.volumePercentage, countingVoiceMixerGroup, soundData.minPitchPercentage, soundData.maxPitchPercentage, (soundData.panPercentage == 0f) ? row.rowSoundPanAdjustedFor2P : soundData.panPercentage, 0f, useGlobalBeatSound: true, isGameplayBeat: true));
				}
				if (syncoBeat >= 0 && syncoBeat <= 5)
				{
					string text = string.Empty;
					if (row.syncoStyle == SyncoStyle.Chirp)
					{
						if (k == syncoBeat)
						{
							text = "sndSyncoPull";
						}
						else if (k == syncoBeat + 1)
						{
							text = "sndSyncoRelease";
						}
						else if (k == 0)
						{
							text = "sndSyncoFirstPulse";
						}
					}
					if (row.syncoStyle == SyncoStyle.Beep)
					{
						if (k == 6)
						{
							text = "sndSyncopationBeepHigh";
						}
						else if (k == syncoBeat + 1)
						{
							text = "sndSyncopationBeepMid";
						}
						else if (k % 2 == 0 && k <= syncoBeat)
						{
							text = "sndSyncopationBeepLow";
						}
					}
					if (text != string.Empty)
					{
						AudioSource item6 = audioSources.RDAdd(base.conductor.PlayBeat(text, _arrTimesFromStartOfBar[k], row.rowSoundVolMultiplier * row.syncoVolume, RDUtils.GetMixerGroup("CuesoundsParent"), row.syncoPitch, row.syncoPitch, row.rowSoundPanAdjustedFor2P, 0f, useGlobalBeatSound: true, isGameplayBeat: true));
						audioTimes.RDAdd(item6, base.conductor.lastScheduledTime);
					}
				}
			}
			else if ((!flag4 & audibleSkipBeats) && !hasHeldPulses)
			{
				MakeNonHeldBeatSound(_arrTimesFromStartOfBar, k, num2, base.gc.beat_audibleSkipBeatGainMultiplier, customBeatsound, syncoPitch);
			}
			if (flag3)
			{
				inputTimeSansCalibration = beats[k].timeAbs;
				if (hasHeldPulses)
				{
					releaseTimeSansCalibration = beats[k].timeReleaseAbs;
				}
				if (GC.d_auto7th && !playerDrives7thBeat)
				{
					PlayHitSoundIfNoOthersPlaying(_arrTimesFromStartOfBar[k]);
				}
				if (hasHeldPulses)
				{
					PlayHeldClapSound(_arrTimesFromStartOfBar[k], _arrTimesReleaseFromStartOfBar[k]);
				}
				num = (hasHeldPulses ? releaseTimeSansCalibration : inputTimeSansCalibration);
			}
		}
		SetHeartExplosion(num - base.conductor.startOfNextBar);
		return this;
	}

	private void MakeNonHeldBeatSound(float[] _arrTimesFromStartOfBar, int i, int beatNumber, float gainMultiplier = 1f, SoundData customBeatsound = null, float syncoPitch = 1f)
	{
		if (row.muteRowSounds)
		{
			return;
		}
		AudioMixerGroup beatSoundMixerGroup = base.BeatSoundMixerGroup;
		float alternatingBeatPitchMultiplier = GetAlternatingBeatPitchMultiplier(i);
		SoundData soundData = customBeatsound ?? row.pulseSounds[beatNumber - 1];
		AudioSource audioSource = base.conductor.PlayBeat(soundData.filename, _arrTimesFromStartOfBar[i], soundData.volumePercentage * gainMultiplier, beatSoundMixerGroup, soundData.minPitchPercentage * alternatingBeatPitchMultiplier * syncoPitch, soundData.maxPitchPercentage * alternatingBeatPitchMultiplier * syncoPitch, (soundData.panPercentage != 0f && row.rowSoundPan == 0f) ? RDUtils.OverridePanFor2P(row, soundData.panPercentage) : row.rowSoundPanAdjustedFor2P, 0f, useGlobalBeatSound: true, isGameplayBeat: true);
		if (audioSource == null)
		{
			return;
		}
		beats[i].audioSource = audioSource;
		if (base.gc.beatSoundMode == BeatSoundMode.HighPitchOnFirstBeat)
		{
			if (i == 0)
			{
				audioSource.pitch *= base.gc.beat_firstBeatPitch;
				audioSource.volume *= base.gc.beat_firstBeatVolume;
			}
		}
		else if (base.gc.beatSoundMode == BeatSoundMode.CustomPitchForEachBeat)
		{
			audioSource.pitch *= base.gc.beat_customBeatPitches[beatNumber - 1];
			audioSource.volume *= base.gc.beat_customBeatVolumes[beatNumber - 1];
		}
		audioSource.gameObject.name = string.Concat($"{soundData.filename} -> Bar: {base.conductor.barNumber} Beat: {beatNumber} i: {i} pitch: {audioSource.pitch}");
		audioSources.RDAdd(audioSource);
	}

	public static float GetAlternatingBeatPitchMultiplier(int i)
	{
		bool flag = !scrConductor.BeatsoundPitchMode;
		bool flag2 = (i + 1) % 2 == 0;
		float result = 1f;
		if ((flag & flag2) || (!flag && !flag2))
		{
			result = scrConductor.BeatsoundPitchVarianceMultiplier;
		}
		return result;
	}

	public void SetHeartExplosion(double hittimeFSAB)
	{
		double num = hittimeFSAB + (double)(base.game.currentLevel.crotchetsToExplode * base.conductor.crotchet);
		if (base.game.currentLevel.heartExplodeType == HeartExplodeType.GatherAndCeil)
		{
			num = RDUtils.CeilToNearestMultiple(num, base.conductor.crotchet);
		}
		explodeTimeAbs = num + base.conductor.startOfNextBar;
	}

	public override void Update()
	{
		if (!dead)
		{
			if (hasHeldPulses)
			{
				UpdateBeatboxStatuses();
			}
			else
			{
				UpdateCurrentBeatbox();
			}
			base.Update();
		}
	}

	public void UpdateCurrentBeatbox()
	{
		if (base.rowID >= base.game.rows.Length)
		{
			RDClassDll.printem($"rowID is {base.rowID} and game.rows.Length is {base.game.rows.Length}");
			return;
		}
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < beats.Count && !(base.conductor.visualPos <= beats[i].timeAbs); i++)
		{
			if (!hasHeldPulses)
			{
				num++;
				continue;
			}
			int num3 = beats[i].beatboxNumber - 1;
			if (num3 >= 0 && num3 < 6)
			{
				bool flag = false;
				if (num3 < row.show.Length)
				{
					flag = row.beatboxes[num3].pulseMode != 'x';
				}
				if (!flag)
				{
					num2++;
					continue;
				}
				num++;
				num += num2;
				num2 = 0;
			}
			else
			{
				num++;
				num += num2;
				num2 = 0;
			}
		}
		int num4 = currentActivatedArrayPosition;
		currentActivatedArrayPosition = num - 1;
		justPulsed = num4 != currentActivatedArrayPosition;
		int num5 = currentBeatbox;
		int num6 = 0;
		if (currentActivatedArrayPosition >= 0)
		{
			num6 = beats[currentActivatedArrayPosition].beatboxNumber;
		}
		if (num6 == -99)
		{
			DestroyBeat();
			scrBeatbox[] beatboxes = row.ent.classicRowController.beatboxes;
			for (int j = 0; j < beatboxes.Length; j++)
			{
				beatboxes[j].RefreshPulseStatus();
			}
		}
		currentBeatbox = num6;
		if (playerDrives7thBeat)
		{
			currentBeatbox = Mathf.Min(6, num6);
		}
		if (num5 == currentBeatbox && !justPulsed)
		{
			return;
		}
		if (freeTimeEvent != null && !hasHeldPulses)
		{
			if (currentBeatbox == freeTimeEvent.pulse + 1)
			{
				firstFreeTimeExecuted = freeTimeEvent.CheckConditionals();
				if (firstFreeTimeExecuted)
				{
					shouldCallRefresh = true;
					return;
				}
				foreach (SingleBeat beat in beats)
				{
					beat.audioSource.volume = 0f;
				}
				DestroyBeat(killSounds: true);
			}
			else if (firstFreeTimeExecuted)
			{
				shouldCallRefresh = true;
			}
		}
		else
		{
			shouldCallRefresh = true;
		}
	}

	private void UpdateBeatboxStatuses()
	{
		BeatboxStatus[] array = (BeatboxStatus[])beatboxStatuses.Clone();
		bool flag = false;
		for (int i = 1; i < 7; i++)
		{
			BeatboxStatus beatboxStatus = BeatboxStatus.Flat;
			for (int j = 0; j < beats.Count; j++)
			{
				if (i == beats[j].beatboxNumber && row.beatboxes[i - 1].pulseMode != 'x' && !(base.conductor.visualPos <= beats[j].timeAbs) && !(base.conductor.visualPos >= beats[j].timeReleaseAbs))
				{
					beatboxStatus = BeatboxStatus.Rolling;
					flag = true;
					currentBeatbox = -1;
				}
			}
			beatboxStatuses[i - 1] = beatboxStatus;
		}
		if (flag)
		{
			bool flag2 = false;
			for (int k = 0; k < beatboxStatuses.Length; k++)
			{
				if (beatboxStatuses[k] == array[k])
				{
					flag2 = true;
				}
			}
			if (flag2)
			{
				shouldCallRefresh = true;
			}
		}
		else
		{
			UpdateCurrentBeatbox();
		}
	}

	public override void LateUpdate()
	{
		if (row.ent != null && shouldCallRefresh)
		{
			scrBeatbox[] beatboxes = row.ent.classicRowController.beatboxes;
			foreach (scrBeatbox scrBeatbox2 in beatboxes)
			{
				if (!scrBeatbox2.beatboxRefreshedThisFrame)
				{
					scrBeatbox2.RefreshPulseStatus();
				}
			}
			shouldCallRefresh = false;
		}
		base.LateUpdate();
	}

	public override void MoveBackBy(double timeDiff)
	{
		base.MoveBackBy(timeDiff);
		for (int i = 0; i < beats.Count; i++)
		{
			beats[i].timeAbs -= timeDiff;
		}
		if (hasHeldPulses)
		{
			for (int j = 0; j < beats.Count; j++)
			{
				beats[j].timeReleaseAbs -= timeDiff;
			}
		}
	}

	private void BananaBeatsCalculation()
	{
		if (beats.Count < 3)
		{
			return;
		}
		Dictionary<float, int> dictionary = new Dictionary<float, int>();
		int num = 0;
		foreach (SingleBeat beat in beats)
		{
			float num2 = (float)Math.Round(beat.timeToNextBeat, 3);
			if (num2 != 0f)
			{
				if (!dictionary.TryAdd(num2, 1))
				{
					dictionary[num2]++;
				}
				if (dictionary[num2] > num)
				{
					num++;
				}
			}
		}
		bool flag = beats.Count > 4;
		List<float> list = new List<float>();
		foreach (KeyValuePair<float, int> item in dictionary)
		{
			float key = item.Key;
			if (item.Value >= num || !flag)
			{
				list.Add(key);
			}
		}
		if (list.Count == 0)
		{
			return;
		}
		list.Sort((float x, float y) => y.CompareTo(x));
		float num3 = list[list.Count / 2];
		int num4 = 0;
		int index = 0;
		for (int num5 = 0; num5 < beats.Count - 1; num5++)
		{
			double num6 = beats[num5].timeToNextBeat - (double)num3;
			bool flag2 = num6 > 0.01;
			bool flag3 = num6 < -0.01;
			num4 = ((!(flag2 | flag3)) ? (++num4) : 0);
			if (flag2)
			{
				beats[num5].bend = BananaBend.Pull;
				beats[num5 + 1].bend = BananaBend.Release;
				index = num5 + 1;
			}
			if (flag3)
			{
				beats[num5].bend = BananaBend.ReleaseNoSpring;
				if (num5 > 0 && beats[num5 - 1].bend != BananaBend.ReleaseNoSpring)
				{
					beats[num5 - 1].bend = BananaBend.Pull;
				}
			}
			if (num4 > 1)
			{
				beats[index].bend = BananaBend.Straight;
			}
		}
		bool flag4 = true;
		for (int num7 = 1; num7 < beats.Count - 1; num7++)
		{
			BananaBend bend = beats[num7].bend;
			bool num8 = (num7 + 1) % 2 == 0;
			bool flag5 = !num8;
			if ((!num8 || bend != BananaBend.Pull) && (!flag5 || bend != BananaBend.Release))
			{
				flag4 = false;
				break;
			}
		}
		if (flag4)
		{
			bool flag6 = beats[0].timeAbs != beats[0].timeReleaseAbs;
			beats[0].bend = (flag6 ? BananaBend.Release : BananaBend.ReleaseNoSpring);
		}
	}
}
