using System;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class RDWaveManager : RDBase
{
	public RowEntity ent;

	public RDWaveAnimation animationFlat;

	public RDWaveAnimation animationWobble;

	[NonSerialized]
	public RDWave[] allWaves;

	public RDWave[] primaryWaves;

	public RDWave[] subdivWaves;

	public RDWaveRenderer[] shadows;

	public WaveType waveType;

	private Tween positionOverrideTween;

	private Tween positionOverrideLerpTween;

	public static float heldwave_speed = 47.5f;

	public static float heldwave_vertOffset = 1.2f;

	public static float heldwave_vertMult = 0.429f;

	public static float heldwave_t = 0f;

	public static float heldwave_offsetSpeed = 12.34f;

	public static float heldwave_offsetFPS = 25f;

	public RDWave firstWave => primaryWaves[0];

	private void Awake()
	{
		RDWaveRenderer.GetShaderPropertyIDs();
		allWaves = primaryWaves.Concat(subdivWaves).ToArray();
	}

	private void Update()
	{
		UpdateWaveMechanics();
		if (!Application.isPlaying)
		{
			return;
		}
		RDWave[] array = allWaves;
		foreach (RDWave rDWave in array)
		{
			if (!base.game.audioPaused)
			{
				rDWave.AdvanceTime();
			}
			rDWave.UpdateWaveToCurrentTime();
		}
	}

	private void UpdateWaveMechanics()
	{
		bool flag = true;
		foreach (Beat beat in base.game.beats)
		{
			if (!(beat is BeatOneshot beatOneshot) || beatOneshot.rowID != ent.rowID || beatOneshot.dead || !base.conductor.HasReachedVisual(beatOneshot.boomTimeAbs))
			{
				continue;
			}
			flag = false;
			if (beatOneshot.phase == 2)
			{
				continue;
			}
			if (beatOneshot.assignedToWave == null)
			{
				bool flag2 = false;
				if (beatOneshot.friends != null && (!base.game.currentLevel.oldSubdivFriendBehavior || beatOneshot.pulseType != OneshotPulseType.Triangle))
				{
					foreach (BeatOneshot friend in beatOneshot.friends)
					{
						if (friend.assignedToWave != null)
						{
							beatOneshot.assignedToWave = friend.assignedToWave;
							flag2 = true;
							break;
						}
					}
				}
				if (!flag2)
				{
					bool flag3 = beatOneshot.friends != null && beatOneshot.friends.Exists((BeatOneshot f) => !f.isPolygon);
					if (beatOneshot.isPolygon && !flag3)
					{
						RDWave[] array = subdivWaves;
						for (int num = 0; num < array.Length; num++)
						{
							if (array[num].AttemptToAssignBeat(beatOneshot))
							{
								flag2 = true;
								break;
							}
						}
					}
					else
					{
						RDWave[] array = primaryWaves;
						for (int num = 0; num < array.Length; num++)
						{
							if (array[num].AttemptToAssignBeat(beatOneshot))
							{
								flag2 = true;
								break;
							}
						}
					}
					if (!flag2)
					{
						Debug.LogWarning("Couldn't find a wave for this beat!");
						break;
					}
				}
			}
			beatOneshot.assignedToWave.UpdateMechanicsUsingBeat(beatOneshot);
		}
	}

	private void PlayAnimation(RDWaveAnimation animation, RDWave targetWave)
	{
		targetWave.PlayAnimation(animation);
	}

	public void PlayWobble()
	{
		PlayAnimation(animationWobble, firstWave);
	}

	public void SetPositionOverride(float posOverride, float dur, Ease ease)
	{
		positionOverrideTween?.Kill(complete: true);
		positionOverrideTween = DOTween.To(() => firstWave.positionOverride, (float x) =>
		{
			firstWave.positionOverride = x;
		}, posOverride, dur).SetEase(ease).Done();
	}

	public void SetPositionOverrideLerp(float posOverrideLerp, float dur, Ease ease)
	{
		positionOverrideLerpTween?.Kill(complete: true);
		positionOverrideLerpTween = DOTween.To(() => firstWave.positionOverrideLerp, (float x) =>
		{
			firstWave.positionOverrideLerp = x;
		}, posOverrideLerp, dur).SetEase(ease).Done();
	}

	public void FlashShadow(RDWaveRenderer shadow, RDWaveRenderer source, float time, float offsetTime)
	{
		shadow.shaderRenderer.enabled = true;
		shadow.enabled = true;
		shadow.source = source;
		if (source is RDWaveRenderer_Pulse rDWaveRenderer_Pulse)
		{
			(shadow as RDWaveRenderer_Pulse).isHeart = rDWaveRenderer_Pulse.isHeart;
		}
		shadow.UpdateVisuals(time, offsetTime);
	}

	public void FadeShadow(RDWaveRenderer shadow, float duration)
	{
		shadow.ShadowFade(duration);
	}

	public void ResetShadows(float playbackSpeed)
	{
		float a = ent.playerBox.HitlineWidth / 2f - 3f;
		for (int i = 0; i < shadows.Length; i++)
		{
			RDWaveRenderer obj = shadows[i];
			obj.source = null;
			obj.readyForShadowUpdate = true;
			obj.waveAnimations[0] = base.game.animationRush;
			obj.playbackSpeed = playbackSpeed;
			obj.x_offset = Mathf.Min(a, (float)i * RDConstants.data.shadowOffsetPer);
		}
	}

	public void HideShadows()
	{
		if (shadows == null)
		{
			return;
		}
		RDWaveRenderer[] array = shadows;
		foreach (RDWaveRenderer rDWaveRenderer in array)
		{
			if (rDWaveRenderer != null)
			{
				rDWaveRenderer.ShadowHide();
			}
		}
	}

	public void FlattenAll()
	{
		RDWave[] array = allWaves;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Flatten();
		}
	}
}
