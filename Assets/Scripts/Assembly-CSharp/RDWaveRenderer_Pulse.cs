using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

public class RDWaveRenderer_Pulse : RDWaveRenderer
{
	public bool isHeart;

	[NonSerialized]
	public float[] friendsWidthMultipliers = new float[4] { 1f, 1f, 1f, 1f };

	[NonSerialized]
	public float[] friendsHeightMultipliers = new float[4] { 1f, 1f, 1f, 1f };

	public RDWaveAnimation currentAnim;

	private static bool localKeywordsSetup;

	public static LocalKeyword heartKeyword;

	public static LocalKeyword shadowKeyword;

	public static LocalKeyword waves2Keyword;

	public static LocalKeyword waves3Keyword;

	public static LocalKeyword waves4Keyword;

	public static LocalKeyword outlineKeyword;

	public static LocalKeyword glowKeyword;

	private Tween shadowFadeTween;

	public Vector4 waveHeightCache = Vector4.zero;

	public int lastWaveCount = -1;

	public bool materialWaveHeightNeedsUpdating;

	public static void SetupLocalKeywords(Material material)
	{
		if (!localKeywordsSetup)
		{
			heartKeyword = new LocalKeyword(material.shader, "HEART");
			shadowKeyword = new LocalKeyword(material.shader, "SHADOW");
			waves2Keyword = new LocalKeyword(material.shader, "_2WAVES");
			waves3Keyword = new LocalKeyword(material.shader, "_3WAVES");
			waves4Keyword = new LocalKeyword(material.shader, "_4WAVES");
			outlineKeyword = new LocalKeyword(material.shader, "OUTLINE");
			glowKeyword = new LocalKeyword(material.shader, "GLOW");
			localKeywordsSetup = true;
		}
	}

	public override void Awake()
	{
		base.Awake();
		base.material.SetVector(RDWaveRenderer.sineHeightID, Vector4.zero);
		SetupLocalKeywords(base.material);
		base.material.SetKeyword(in heartKeyword, isHeart);
		base.material.SetKeyword(in shadowKeyword, isShadow);
	}

	public override void UpdateVisuals(float t, float wavePos, int pulseIndex = 0)
	{
		if (!(waveAnimations[pulseIndex] == null))
		{
			float waveHeight = GetWaveHeight(t, pulseIndex);
			RDWaveAnimation rDWaveAnimation = (currentAnim = waveAnimations[pulseIndex]);
			if (pulseIndex == 0)
			{
				RDWaveManager.heldwave_t = wavePos;
			}
			float num = Mathf.Floor(Time.time * RDWaveManager.heldwave_offsetFPS) / RDWaveManager.heldwave_offsetFPS;
			float num2 = (hold ? ((0f - num) * RDWaveManager.heldwave_offsetSpeed) : 1f);
			float num3 = (hold ? 0.75f : 1f);
			float num4 = (hold ? (RDWaveManager.heldwave_vertOffset + (Mathf.Sin(Time.time * RDWaveManager.heldwave_speed) + 1f) / 2f * RDWaveManager.heldwave_vertMult) : 1f) * friendsHeightMultipliers[pulseIndex];
			base.material.SetKeyword(in heartKeyword, isHeart);
			SetWaveWidth(rDWaveAnimation.sineWidth.Evaluate(t) * ent.oneshotRowController.waveWidthMultiplier * friendsWidthMultipliers[pulseIndex], pulseIndex);
			SetWaveHeight(waveHeight * ent.oneshotRowController.waveHeightMultiplier * num4, pulseIndex);
			SetTriangleOffset(rDWaveAnimation.triangleOffset.Evaluate(t) + num2, pulseIndex);
			SetTriangleFrequency(rDWaveAnimation.triangleFrequency.Evaluate(t) * num3, pulseIndex);
			SetTriangleOffsetLock(rDWaveAnimation.triangleLock, pulseIndex);
			SetVectorComponent(RDWaveRenderer.waveTimeID, pulseIndex, wavePos);
		}
	}

	private void SetVectorComponent(int propertyID, int pulseIndex, float x)
	{
		Vector4 vector = base.material.GetVector(propertyID);
		vector[pulseIndex] = x;
		base.material.SetVector(propertyID, vector);
	}

	protected override void UpdateColor()
	{
		Color color = GetColor();
		if (flashDuration > 0f && base.material != null)
		{
			flashTime += Time.deltaTime;
			float t = DOVirtual.EasedValue(1f - flash, 1f, flashTime / flashDuration, Ease.InQuad);
			Color color2 = base.material.GetColor(RDWaveRenderer.colorOverlayID);
			Color b = Color.Lerp(color, color2, color2.a);
			Color value = Color.Lerp(Color.white, b, t);
			base.material.SetColor(RDWaveRenderer.waveHighlightColorID, value);
			if (flashTime > flashDuration)
			{
				flashDuration = 0f;
			}
		}
		base.material.SetColor(RDWaveRenderer.waveColorID, color);
	}

	public override void ResizeCanvas()
	{
		base.ResizeCanvas();
		Material sharedMaterial = shaderRenderer.sharedMaterial;
		Vector3 localScale = base.transform.localScale;
		sharedMaterial.SetFloat(RDWaveRenderer.canvasWidthID, localScale.x);
		sharedMaterial.SetFloat(RDWaveRenderer.canvasHeightID, localScale.y);
	}

	public override void ShadowHide()
	{
		if (isShadow && shadowFadeTween != null)
		{
			shadowFadeTween.Kill();
		}
		shaderRenderer.enabled = false;
		base.enabled = false;
		base.material.SetColor(RDWaveRenderer.waveColorID, Color.clear);
		base.material.SetColor(RDWaveRenderer.colorOverlayID, Color.clear);
	}

	public override void ShadowFade(float duration)
	{
		if (shadowFadeTween != null)
		{
			shadowFadeTween.Kill();
		}
		shadowFadeTween = DOTween.To(() => base.shadowAlpha, (float x) =>
		{
			base.shadowAlpha = x;
		}, 0f, duration).SetEase(base.gc.shadowFadeEase).OnComplete(() =>
		{
			ShadowHide();
		});
	}

	public void SetWaveWidth(float w, int pulseIndex = 0)
	{
		SetVectorComponent(RDWaveRenderer.sineWidthID, pulseIndex, w);
	}

	public void SetTriangleOffset(float x, int pulseIndex = 0)
	{
		SetVectorComponent(RDWaveRenderer.triangleOffsetID, pulseIndex, x);
	}

	public void SetTriangleFrequency(float f, int pulseIndex = 0)
	{
		SetVectorComponent(RDWaveRenderer.triangleFrequencyID, pulseIndex, f);
	}

	public void SetTriangleOffsetLock(float x, int pulseIndex = 0)
	{
		SetVectorComponent(RDWaveRenderer.triangleOffsetLockID, pulseIndex, x);
	}

	public void SetWaveHeight(float h, int pulseIndex = 0)
	{
		SetVectorComponent(RDWaveRenderer.sineHeightID, pulseIndex, h);
		materialWaveHeightNeedsUpdating = true;
		waveHeightCache[pulseIndex] = h;
	}
}
