using System;
using UnityEngine;

public class RDWaveRenderer : InvisibleEntity
{
	private const int maxPulses = 4;

	public bool isShadow;

	public bool visible;

	public float x_offset;

	public RowEntity ent;

	public RDWaveManager manager;

	[NonSerialized]
	public RDWave wave;

	[NonSerialized]
	public RDWaveAnimation[] waveAnimations = new RDWaveAnimation[4];

	[NonSerialized]
	public float playbackSpeed = 1f;

	[NonSerialized]
	public float lastValidTimeScale = 1f;

	[NonSerialized]
	public RDWaveRenderer source;

	[NonSerialized]
	public bool readyForShadowUpdate = true;

	[NonSerialized]
	public bool hold;

	private Material _material;

	protected float flashTime;

	protected float flash;

	protected float flashDuration;

	public static int waveTimeID;

	public static int sineWidthID;

	public static int sineHeightID;

	public static int triangleOffsetID;

	public static int triangleFrequencyID;

	public static int triangleOffsetLockID;

	public static int canvasWidthID;

	public static int canvasHeightID;

	public static int waveColorID;

	public static int colorOverlayID;

	public static int waveHighlightColorID;

	public static bool shaderPropertyIDsSetup;

	public Material material
	{
		get
		{
			if (_material == null)
			{
				_material = shaderRenderer.material;
			}
			return _material;
		}
	}

	public float shadowAlpha
	{
		get
		{
			return material.GetColor(waveColorID).a;
		}
		set
		{
			if (!(source == null))
			{
				Color color = source.material.GetColor(waveColorID);
				Color color2 = source.material.GetColor(colorOverlayID);
				material.SetColor(waveColorID, color.WithAlpha(value));
				material.SetColor(colorOverlayID, color2);
				material.SetColor(RDShaderProperties.GlowColorID, Color.clear);
			}
		}
	}

	protected virtual void UpdateColor()
	{
	}

	public virtual void UpdateVisuals(float t, float wavePos, int pulseIndex = 0)
	{
	}

	public virtual void ShadowHide()
	{
	}

	public virtual void ShadowFade(float duration)
	{
	}

	public virtual void ResizeCanvas()
	{
	}

	protected Color GetColor()
	{
		RDPlayer currentPlayer = ent.row.GetCurrentPlayer();
		if (currentPlayer != RDPlayer.CPU)
		{
			if (GC.twoPlayerMode)
			{
				if (currentPlayer != RDPlayer.P1)
				{
					return base.gc.player2StripColor;
				}
				return base.gc.player1StripColor;
			}
			return base.gc.oneshotWaveNormalColor;
		}
		return base.gc.playerCPUStripColor;
	}

	protected virtual void LateUpdate()
	{
		if (!isShadow)
		{
			UpdateShaderDataSource();
			if (shaderRenderer != null)
			{
				if (lastFrameUpdatedGraphics < shaderDataToUse.frameChanged)
				{
					shaderDataToUse.SetPropertiesInRenderer(shaderRenderer);
					bool num = shaderDataToUse.borderColor.a > 0f;
					bool value = num && shaderDataToUse.outline == 1;
					bool value2 = num && shaderDataToUse.outline == 0;
					Material material = shaderRenderer.material;
					if (this is RDWaveRenderer_Pulse)
					{
						material.SetKeyword(in RDWaveRenderer_Pulse.outlineKeyword, value);
						material.SetKeyword(in RDWaveRenderer_Pulse.glowKeyword, value2);
					}
					lastFrameUpdatedGraphics = RDTime.currentFrame;
				}
				if (this is RDWaveRenderer_Pulse { materialWaveHeightNeedsUpdating: not false, waveHeightCache: var waveHeightCache } rDWaveRenderer_Pulse)
				{
					int num2 = ((waveHeightCache[3] > 0f) ? 4 : ((waveHeightCache[2] > 0f) ? 3 : ((!(waveHeightCache[1] > 0f)) ? 1 : 2)));
					if (num2 != rDWaveRenderer_Pulse.lastWaveCount)
					{
						this.material.SetKeyword(in RDWaveRenderer_Pulse.waves4Keyword, num2 == 4);
						this.material.SetKeyword(in RDWaveRenderer_Pulse.waves3Keyword, num2 == 3);
						this.material.SetKeyword(in RDWaveRenderer_Pulse.waves2Keyword, num2 == 2);
						rDWaveRenderer_Pulse.lastWaveCount = num2;
					}
					rDWaveRenderer_Pulse.materialWaveHeightNeedsUpdating = false;
				}
				shaderRenderer.enabled = visible && !(this is RDWaveRenderer_Polygon);
			}
			UpdateColor();
		}
		else
		{
			Transform transform = manager.firstWave.waveRenderer.transform;
			base.transform.localPosition = transform.localPosition.AddX(x_offset);
			base.transform.localScale = transform.transform.localScale;
		}
	}

	public float GetWaveHeight(float t, int pulseIndex = 0)
	{
		RDWaveAnimation rDWaveAnimation = waveAnimations[pulseIndex];
		float num = rDWaveAnimation.sineHeight.Evaluate(t);
		if (rDWaveAnimation.heightDecays)
		{
			num *= Mathf.Exp((0f - rDWaveAnimation.decayConstant) * t) * Mathf.Cos(rDWaveAnimation.angularFrequency * t);
			if (Mathf.Abs(num) < 1f)
			{
				num = 0f;
			}
		}
		return num;
	}

	public void Flash(float alpha, float duration)
	{
		flash = alpha;
		flashDuration = duration;
		flashTime = 0f;
	}

	public static void GetShaderPropertyIDs()
	{
		if (!shaderPropertyIDsSetup)
		{
			shaderPropertyIDsSetup = true;
			waveTimeID = Shader.PropertyToID("_WaveTime");
			sineWidthID = Shader.PropertyToID("_SineWidth");
			sineHeightID = Shader.PropertyToID("_SineHeight");
			triangleOffsetID = Shader.PropertyToID("_TriangleOffset");
			triangleFrequencyID = Shader.PropertyToID("_TriangleFrequency");
			triangleOffsetLockID = Shader.PropertyToID("_TriangleOffsetLock");
			canvasWidthID = Shader.PropertyToID("_CanvasWidth");
			canvasHeightID = Shader.PropertyToID("_CanvasHeight");
			waveColorID = Shader.PropertyToID("_WaveColor");
			colorOverlayID = Shader.PropertyToID("_ColorOverlay");
			waveHighlightColorID = Shader.PropertyToID("_WaveHighlightColor");
		}
	}
}
