using System;
using System.Collections.Generic;
using UnityEngine;

public class RDWave : RDBase
{
	private RDWaveAnimation _waveAnimation;

	public RowEntity ent;

	public RDWaveManager manager;

	public RDWaveRenderer waveRenderer;

	private WaveType lastWaveType;

	[NonSerialized]
	public BeatOneshot assignedBeat;

	public List<BeatOneshot> currentFriendsList;

	[NonSerialized]
	public int friendsAdded;

	[NonSerialized]
	public OneshotPulseType pulseType;

	public int multiPulseIndex = -1;

	public RDWaveRenderer_Pulse multiPulse_PulseRenderer;

	public RDWaveRenderer_Polygon multiPulse_PolygonRenderer;

	private float timeElapsed;

	private float _playbackSpeed = 1f;

	private float _lastValidTimeScale = 1f;

	private float timeOffset;

	private float positionOffset;

	[NonSerialized]
	public float polygonRotOffset;

	private bool freezeshotAndNotFrozenYet;

	[NonSerialized]
	public float timeSpentFrozen;

	private bool _frozen;

	private bool shadowsOnForNewestBeat;

	[NonSerialized]
	public float positionOverride;

	[NonSerialized]
	public float positionOverrideLerp;

	public RDWaveAnimation waveAnimation
	{
		get
		{
			return _waveAnimation;
		}
		set
		{
			_waveAnimation = value;
			int num = (usesMultiPulse ? multiPulseIndex : 0);
			waveRenderer.waveAnimations[num] = value;
		}
	}

	public bool frozen
	{
		get
		{
			return _frozen;
		}
		set
		{
			if (!value || (value && freezeshotAndNotFrozenYet))
			{
				freezeshotAndNotFrozenYet = false;
			}
			_frozen = value;
		}
	}

	private WaveType waveType => manager.waveType;

	private RDWaveAnimation flatAnimation => manager.animationFlat;

	public bool assignedToActiveBeat
	{
		get
		{
			if (assignedBeat != null)
			{
				return base.game.beats.Contains(assignedBeat);
			}
			return false;
		}
	}

	private float playbackSpeed
	{
		get
		{
			return _playbackSpeed;
		}
		set
		{
			_playbackSpeed = value;
			waveRenderer.playbackSpeed = value;
		}
	}

	private float lastValidTimeScale
	{
		get
		{
			return _lastValidTimeScale;
		}
		set
		{
			_lastValidTimeScale = value;
			waveRenderer.lastValidTimeScale = value;
		}
	}

	private bool isPulse => waveRenderer is RDWaveRenderer_Pulse;

	public bool usesMultiPulse => multiPulseIndex >= 0;

	public bool visible
	{
		get
		{
			return waveRenderer.visible;
		}
		set
		{
			if (value != visible)
			{
				if (usesMultiPulse)
				{
					multiPulse_PulseRenderer.visible = value;
					multiPulse_PolygonRenderer.visible = !isPulse & value;
				}
				else
				{
					waveRenderer.visible = value;
				}
				manager.HideShadows();
			}
		}
	}

	public void Awake()
	{
		if (waveRenderer != null)
		{
			waveRenderer.wave = this;
		}
		if (usesMultiPulse)
		{
			multiPulse_PulseRenderer.wave = this;
			multiPulse_PolygonRenderer.wave = this;
		}
	}

	public void AdvanceTime()
	{
		if (!frozen)
		{
			timeElapsed += Time.deltaTime;
		}
		else
		{
			timeSpentFrozen += Time.deltaTime;
		}
		if (visible && base.gc.useShadows && !base.game.currentLevel.noOneshotShadows && assignedToActiveBeat && waveAnimation != null && waveAnimation.name.Contains("Rush") && timeElapsed * waveAnimation.timeScale * playbackSpeed / lastValidTimeScale > base.gc.rushAnimMinPercentage && shadowsOnForNewestBeat)
		{
			PlaceStaticShadows(timeElapsed, playbackSpeed);
		}
	}

	public void UpdateWaveToCurrentTime()
	{
		UpdateWave(timeElapsed);
	}

	public void PlayAnimation(RDWaveAnimation animation, bool rewind = true, float playbackSpeed = 1f)
	{
		waveAnimation = animation;
		this.playbackSpeed = playbackSpeed;
		if (rewind)
		{
			timeElapsed = 0f;
		}
		if (!(animation == flatAnimation) || !base.gc.useShadows || base.game.currentLevel.noOneshotShadows)
		{
			return;
		}
		foreach (Beat beat in base.game.beats)
		{
			if (beat is BeatOneshot beatOneshot && beatOneshot.rowID == ent.rowID && !beatOneshot.dead && beatOneshot.ShouldHaveShadows() && beatOneshot.phase == 2 && beatOneshot.skipToFlatAnim)
			{
				manager.ResetShadows(this.playbackSpeed);
				PlaceStaticShadows(1f, this.playbackSpeed);
			}
		}
	}

	public void Flatten()
	{
		PlayAnimation(flatAnimation);
		UpdateWave(0f);
	}

	private void UpdateWave(float time)
	{
		if (waveAnimation == null)
		{
			return;
		}
		if (!base.game.audioPaused && Time.timeScale != 0f)
		{
			lastValidTimeScale = Time.timeScale;
		}
		time *= waveAnimation.timeScale * playbackSpeed / lastValidTimeScale;
		int num = (usesMultiPulse ? multiPulseIndex : 0);
		if (isPulse)
		{
			float num2 = 1f;
			float num3 = 1f;
			if (currentFriendsList != null && currentFriendsList.Count > 0)
			{
				num2 = (float)friendsAdded / (float)currentFriendsList.Count;
				if (friendsAdded < currentFriendsList.Count)
				{
					num3 = 0.75f;
				}
			}
			RDWaveRenderer_Pulse obj = waveRenderer as RDWaveRenderer_Pulse;
			obj.friendsWidthMultipliers[num] = num2;
			obj.friendsHeightMultipliers[num] = num3;
		}
		else
		{
			RDWaveRenderer_Polygon obj2 = waveRenderer as RDWaveRenderer_Polygon;
			obj2.sides = ((pulseType == OneshotPulseType.Triangle) ? 3 : 4);
			int num4;
			if (assignedToActiveBeat)
			{
				RowVisibilityMode appearance = ent.appearance;
				num4 = ((appearance == RowVisibilityMode.Visible || appearance == RowVisibilityMode.OnlyRow) ? 1 : 0);
			}
			else
			{
				num4 = 0;
			}
			obj2.visible = (byte)num4 != 0;
		}
		if (!base.game.audioPaused && Time.timeScale != 0f)
		{
			lastValidTimeScale = Time.timeScale;
		}
		float wavePos = Mathf.Lerp(GetWaveTimeWithOffsets(time), positionOverride, positionOverrideLerp);
		waveRenderer.UpdateVisuals(time, wavePos, num);
	}

	private void PlaceStaticShadows(float time, float playbackSpeed)
	{
		RDWaveRenderer[] shadows = manager.shadows;
		float num = (base.gc.rushAnimMaxPercentage - base.gc.rushAnimMinPercentage) / (float)shadows.Length;
		if (!base.game.audioPaused && Time.timeScale != 0f)
		{
			lastValidTimeScale = Time.timeScale;
		}
		float num2 = time * waveAnimation.timeScale * playbackSpeed / lastValidTimeScale;
		float num3 = (base.gc.shadowStartMaxAlpha - base.gc.shadowStartMinAlpha) / (float)shadows.Length;
		float num4 = base.gc.shadowStartMinAlpha + num3;
		float num5 = base.gc.rushAnimMinPercentage;
		int num6 = shadows.Length - 1;
		RDWaveRenderer[] array = shadows;
		foreach (RDWaveRenderer rDWaveRenderer in array)
		{
			if (rDWaveRenderer.readyForShadowUpdate)
			{
				rDWaveRenderer.readyForShadowUpdate = false;
				float waveTimeWithOffsets = GetWaveTimeWithOffsets(num5);
				manager.FlashShadow(rDWaveRenderer, waveRenderer, num5, waveTimeWithOffsets);
				float duration = base.gc.shadowFadeDuration - base.gc.shadowDurationDifference * (float)num6;
				rDWaveRenderer.shadowAlpha = num4;
				manager.FadeShadow(rDWaveRenderer, duration);
			}
			num5 += num;
			num4 += num3;
			if (!(num5 > num2))
			{
				num6--;
				continue;
			}
			break;
		}
	}

	public void UpdateMechanicsUsingBeat(BeatOneshot e)
	{
		if (e.phase == 0)
		{
			UpdateBoom(e);
		}
		else if (e.phase == 1)
		{
			UpdateRush(e);
		}
	}

	private void UpdateBoom(BeatOneshot e)
	{
		positionOffset = e.waveOffset;
		polygonRotOffset = ((e.pulseType == OneshotPulseType.Triangle) ? e.polygonRotation : 0f);
		OneshotPulseType oneshotPulseType = pulseType;
		bool num = oneshotPulseType == OneshotPulseType.Wave || oneshotPulseType == OneshotPulseType.Heart;
		oneshotPulseType = e.pulseType;
		bool flag = oneshotPulseType == OneshotPulseType.Wave || oneshotPulseType == OneshotPulseType.Heart;
		if (num != flag)
		{
			Flatten();
			if (usesMultiPulse)
			{
				oneshotPulseType = e.pulseType;
				RDWaveRenderer rDWaveRenderer = ((oneshotPulseType == OneshotPulseType.Triangle || oneshotPulseType == OneshotPulseType.Square) ? ((RDWaveRenderer)multiPulse_PolygonRenderer) : ((RDWaveRenderer)multiPulse_PulseRenderer));
				waveRenderer = rDWaveRenderer;
				multiPulse_PolygonRenderer.visible = !isPulse;
			}
		}
		pulseType = e.pulseType;
		frozen = false;
		timeSpentFrozen = 0f;
		e.phase++;
		if (isPulse && waveRenderer is RDWaveRenderer_Pulse rDWaveRenderer_Pulse)
		{
			rDWaveRenderer_Pulse.isHeart = pulseType == OneshotPulseType.Heart;
		}
		if (base.game.currentLevel.oldSubdivFriendBehavior && pulseType == OneshotPulseType.Triangle && e.subdivIndex == e.subdivCount - 1)
		{
			bool flag2 = false;
			foreach (BeatOneshot subdivBuddy in e.subdivBuddies)
			{
				if (subdivBuddy.friends != null)
				{
					foreach (BeatOneshot friend in subdivBuddy.friends)
					{
						if (friend.pulseType == OneshotPulseType.Wave)
						{
							friend.assignedToWave.Flatten();
							flag2 = true;
							break;
						}
					}
				}
				if (flag2)
				{
					break;
				}
			}
		}
		waveRenderer.hold = assignedBeat.isHeldClap;
		if (waveType == WaveType.Single)
		{
			float wavePeriod = e.wavePeriod;
			float num2 = base.game.animationRush.duration / base.game.animationRush.timeScale / wavePeriod;
			num2 *= 1.2f;
			PlayAnimation(base.game.animationSingle, rewind: true, num2);
			ent.heart.Beat();
			base.game.hitStripManager.PingRow(ent.row);
			e.phase = 3;
			return;
		}
		if (e.friendGroup.hasFreezeshot)
		{
			freezeshotAndNotFrozenYet = true;
		}
		float boomPeriod = e.boomPeriod;
		float num3 = base.game.animationRush.duration / base.game.animationRush.timeScale;
		float num4 = Mathf.Max(1f, num3 / boomPeriod);
		PlayAnimation(waveType switch
		{
			WaveType.Ball => base.game.animationBallBoom, 
			WaveType.Spring => base.game.animationSpringBoom, 
			WaveType.Spike => base.game.animationSpikeBoom, 
			WaveType.SpikeHuge => base.game.animationSpikeHugeBoom, 
			_ => base.game.animationBoom, 
		}, rewind: true, num4);
		lastWaveType = waveType;
		ent.heart.Beat();
		base.game.hitStripManager.PingRow(ent.row);
		if (e.friends != currentFriendsList)
		{
			currentFriendsList = e.friends;
			friendsAdded = 0;
		}
		waveRenderer.hold = assignedBeat.isHeldClap;
		if (currentFriendsList == null)
		{
			friendsAdded = 0;
			return;
		}
		friendsAdded++;
		bool flag3 = friendsAdded == currentFriendsList.Count;
		waveRenderer.Flash(flag3 ? 0.75f : 0.25f, boomPeriod);
		foreach (BeatOneshot currentFriends in currentFriendsList)
		{
			if (currentFriends.isHeldClap)
			{
				waveRenderer.hold = true;
				break;
			}
		}
	}

	private void UpdateRush(BeatOneshot e)
	{
		if (base.game.currentLevel.oldSubdivFriendBehavior && pulseType == OneshotPulseType.Wave && e.friends != null && e.friends.Exists((BeatOneshot f) => f.pulseType == OneshotPulseType.Triangle))
		{
			e.phase++;
			return;
		}
		if (lastWaveType != waveType)
		{
			float boomPeriod = e.boomPeriod;
			float num = base.game.animationRush.duration / base.game.animationRush.timeScale;
			float num2 = Mathf.Max(1f, num / boomPeriod);
			PlayAnimation(waveType switch
			{
				WaveType.Ball => base.game.animationBallBoom, 
				WaveType.Spring => base.game.animationSpringBoom, 
				WaveType.Spike => base.game.animationSpikeBoom, 
				WaveType.SpikeHuge => base.game.animationSpikeHugeBoom, 
				_ => base.game.animationBoom, 
			}, rewind: false, num2);
			lastWaveType = waveType;
		}
		if (waveType == WaveType.Single || !base.conductor.HasReachedVisual(e.rushTimeAbs + (double)timeSpentFrozen))
		{
			return;
		}
		shadowsOnForNewestBeat = e.ShouldHaveShadows();
		e.phase++;
		if (e.friends == null || friendsAdded == e.friends.Count || e.pulseType == OneshotPulseType.Triangle)
		{
			float num3 = base.game.animationRush.duration / base.game.animationRush.timeScale / e.rushAnimDuration;
			if (e.rushAnimDuration == 0f)
			{
				Debug.Log("Rush animation duration is 0?");
			}
			RDWaveAnimation animation = base.game.animationRush;
			if (waveType == WaveType.Ball)
			{
				animation = base.game.animationBallRush;
			}
			else if (waveType == WaveType.Spring)
			{
				animation = base.game.animationSpringRush;
			}
			PlayAnimation(animation, rewind: true, num3);
			if (base.gc.useShadows && !base.game.currentLevel.noOneshotShadows && e.ShouldHaveShadows())
			{
				manager.ResetShadows(playbackSpeed);
			}
		}
	}

	public float GetWaveTimeWithOffsets(float time)
	{
		float num = EvaluateOffsetWave(waveAnimation, time, positionOffset) + timeOffset;
		if (frozen || freezeshotAndNotFrozenYet)
		{
			num = Mathf.Min(num, isPulse ? 0.745f : 0.99f);
		}
		return num;
	}

	private float EvaluateOffsetWave(RDWaveAnimation baseAnim, float time, float posOffset)
	{
		float num = baseAnim.waveTime.Evaluate(time);
		if (posOffset == 0f)
		{
			return num;
		}
		if (baseAnim.name.Contains("Boom"))
		{
			return num + posOffset;
		}
		if (baseAnim.name.Contains("Rush"))
		{
			float num2 = baseAnim.waveTime[0].value - 1f;
			float num3 = (posOffset + num2) / num2;
			return (num - 1f) * num3 + 1f;
		}
		return num;
	}

	public bool AttemptToAssignBeat(BeatOneshot e)
	{
		bool num = !assignedToActiveBeat;
		if (num)
		{
			e.assignedToWave = this;
			assignedBeat = e;
		}
		return num;
	}
}
