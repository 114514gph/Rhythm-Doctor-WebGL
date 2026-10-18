using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using RDLevelEditor;
using UnityEngine;

public class Level_FreezeshotH : LevelBase
{
	private CustomSprite burstSprite;

	private float particlesPerBeat = 10f;

	private int burstLengthMin = 3;

	private int burstLengthMax = 8;

	private float burstDistanceMin = 20f;

	private float burstDistanceMax = 50f;

	private float burstAngleVariation = 45f;

	private float particleFadeBeats = 3f;

	private int currentBurst;

	public HashSet<string> alreadyAnimatedTargets = new HashSet<string>();

	public Level_FreezeshotH(RDLevelData data)
		: base(data)
	{
	}

	public void DoBurst()
	{
		DoBurstSeeded(currentBurst);
		currentBurst++;
	}

	public void DoBurstSeeded(int seed)
	{
		System.Random random = new System.Random(seed);
		int num = random.Next(burstLengthMin, burstLengthMax + 1);
		_ = new CustomSprite[num];
		float num2 = base.conductor.DurationBeatsToTime(base.conductor.currentBeat, 1f / particlesPerBeat, calledCloseToNextBarMaybe: true);
		float duration = base.conductor.DurationBeatsToTime(base.conductor.currentBeat, particleFadeBeats, calledCloseToNextBarMaybe: true);
		Vector2 vector = new Vector2(random.Next(0, 353), random.Next(0, 199));
		Vector2 vector2 = new Vector2(176f, 99f);
		float num3 = Mathf.Atan2(vector2.y - vector.y, vector2.x - vector.x) * 57.29578f + Mathf.Lerp(-90f, 90f, (float)random.NextDouble());
		for (int i = 0; i < num; i++)
		{
			CustomSprite particle = UnityEngine.Object.Instantiate(burstSprite.gameObject, burstSprite.transform.parent).GetComponent<CustomSprite>();
			particle.transform.localPosition = vector;
			float f = num3 * (MathF.PI / 180f);
			float num4 = Mathf.Lerp(burstDistanceMin, burstDistanceMax, (float)random.NextDouble());
			vector += new Vector2(Mathf.Cos(f), Mathf.Sin(f)) * num4;
			num3 += Mathf.Lerp(0f - burstAngleVariation, burstAngleVariation, (float)random.NextDouble());
			Material material = particle.renderer.material;
			material.SetFloat(RDShaderProperties.OpacityID, 0f);
			particle.opacityTween = material.DOFloat(0f, RDShaderProperties.OpacityID, duration).SetEase(Ease.Linear);
			DOTween.Sequence().AppendInterval((float)i * num2).AppendCallback(() =>
			{
				material.SetFloat(RDShaderProperties.OpacityID, 1f);
			})
				.Append(particle.opacityTween)
				.AppendCallback(() =>
				{
					UnityEngine.Object.Destroy(particle.gameObject);
				});
		}
	}

	public void SetBurstAngleVariation(float burstAngleVariation)
	{
		this.burstAngleVariation = burstAngleVariation;
	}

	public void SetBurstDistance(float burstDistanceMin, float burstDistanceMax)
	{
		this.burstDistanceMin = burstDistanceMin;
		this.burstDistanceMax = burstDistanceMax;
	}

	public void SetBurstFadeTime(float particleFadeBeats)
	{
		this.particleFadeBeats = particleFadeBeats;
	}

	public void SetBurstSpeed(float particlesPerBeat)
	{
		this.particlesPerBeat = particlesPerBeat;
	}

	public void SetBurstLength(int burstLengthMin, int burstLengthMax)
	{
		this.burstLengthMin = burstLengthMin;
		this.burstLengthMax = burstLengthMax;
	}

	public void SetBurstSprite(string spriteName)
	{
		sprites.TryGetValueAs<string, CustomSprite, CustomSprite>(spriteName, out burstSprite);
	}

	public void SetBurstSpriteByIndex(int id)
	{
		burstSprite = sprites.ElementAtOrDefault(id).Value;
	}

	public override void LevelEventWasRun(LevelEvent_Base levelEvent)
	{
		if (levelEvent is LevelEvent_SetVisible { visible: not false, target: var target } levelEvent_SetVisible)
		{
			CustomSprite customSprite = sprites[target];
			string name = customSprite.customAnimation.data.name;
			if (name.Contains("handwrite"))
			{
				alreadyAnimatedTargets.Add(target);
				printe($"{name} -> SetVisible {levelEvent_SetVisible.visible}");
				Vector4 fromValue = new Vector4(-1.35f, 1f, 0.35f, 0f);
				Vector4 endValue = new Vector4(0f, 1f, 0.35f, 0f);
				Vector4 endValue2 = new Vector4(1.35f, 1f, 0.35f, 0f);
				Material material = customSprite.renderer.material;
				DOTween.Sequence().Append(material.DOVector(endValue, "_SpecialFade", 0.05f).SetEase(Ease.Linear).From(fromValue)).AppendInterval(0.7f)
					.Append(material.DOVector(endValue2, "_SpecialFade", 0.3f).SetEase(Ease.Linear));
				material.SetColor(RDShaderProperties.OverlayColorID, new Color(0.9f, 0.9f, 0.9f, 1f));
			}
		}
	}
}
