using System;
using DG.Tweening;
using UnityEngine;

public class InvisibleEntity : RDBase
{
	[Header("InvisibleEntity")]
	protected int lastFrameUpdatedGraphics = int.MinValue;

	public InvisibleEntity shaderDataSource;

	public RDShaderProperties shaderDataToUse;

	public RDShaderProperties shaderData = new RDShaderProperties();

	public Renderer shaderRenderer;

	private int _customBeatMultiplier = 1;

	public int beatCounter;

	[NonSerialized]
	public Tween borderColorTween;

	[NonSerialized]
	public Tween tintColorTween;

	[NonSerialized]
	public Tween opacityTween;

	[NonSerialized]
	public scrVolumeTracker borderGlowTracker;

	public bool usesOwnShaderData
	{
		set
		{
			shaderDataSource = (value ? this : null);
		}
	}

	public int customBeatMultiplier
	{
		get
		{
			return _customBeatMultiplier;
		}
		set
		{
			beatCounter = -1;
			_customBeatMultiplier = value;
		}
	}

	public void UpdateShaderDataSource()
	{
		if (shaderDataSource == null)
		{
			shaderDataToUse = RDBase.Vfx.globalShaderData;
		}
		else if (shaderDataSource == this)
		{
			shaderDataToUse = shaderData;
		}
		else if (shaderDataSource.shaderDataSource == null)
		{
			shaderDataToUse = RDBase.Vfx.globalShaderData;
		}
		else
		{
			shaderDataToUse = shaderDataSource.shaderData;
		}
	}

	public virtual void OnNewBar()
	{
	}

	public virtual void OnBeat()
	{
	}

	public virtual void Awake()
	{
		if (base.gm != null)
		{
			base.gm.invisibleEntityPool.Add(this);
		}
	}

	public virtual void OnDestroy()
	{
		if (base.gm != null)
		{
			base.gm.invisibleEntityPool.Remove(this);
		}
	}

	public void SetAdditiveColor(Color additiveColor)
	{
		usesOwnShaderData = true;
		shaderData.additiveColor = additiveColor;
	}

	public void SetOverlayColor(Color overlayColor)
	{
		usesOwnShaderData = true;
		shaderData.overlayColor = overlayColor;
	}

	public void SetBorderColor(Color color, bool outline = false, float glowPower = 1f)
	{
		usesOwnShaderData = true;
		shaderData.outline = (outline ? 1 : 0);
		shaderData.borderColor = color;
		shaderData.glowPower = glowPower;
	}

	public void SetBorderType(bool outline)
	{
		usesOwnShaderData = true;
		shaderData.outline = (outline ? 1 : 0);
	}

	public void SetGreyscale(bool greyscale)
	{
		usesOwnShaderData = true;
		shaderData.greyscale = (greyscale ? 1 : 0);
	}

	public void TweenBorder(Color borderColor, float duration, Ease ease = Ease.Linear)
	{
		borderColorTween.Kill();
		usesOwnShaderData = true;
		borderColorTween = DOTween.To(() => shaderData.borderColor, (Color c) =>
		{
			shaderData.borderColor = c;
		}, borderColor, duration).SetEase(ease).Done();
	}

	public void TweenTint(Color tintColor, float duration, Ease ease = Ease.Linear)
	{
		tintColorTween.Kill();
		usesOwnShaderData = true;
		tintColorTween = DOTween.To(() => shaderData.overlayColor, (Color c) =>
		{
			shaderData.overlayColor = c;
		}, tintColor, duration).SetEase(ease).Done();
	}

	public void TweenColor(Color borderColor, Color tintColor, float duration, Ease ease = Ease.Linear)
	{
		TweenBorder(borderColor, duration, ease);
		TweenTint(tintColor, duration, ease);
	}

	public void TweenOpacity(float opacity, float duration, Ease ease = Ease.Linear)
	{
		opacityTween.Kill();
		usesOwnShaderData = true;
		opacityTween = DOTween.To(() => shaderData.opacity, (float c) =>
		{
			shaderData.opacity = c;
		}, opacity, duration).SetEase(ease).Done();
	}
}
