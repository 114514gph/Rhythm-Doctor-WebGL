using System;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
public class RDShaderProperties
{
	public const string OverlayColorProperty = "_ColorOverlay";

	public const string AdditiveColorProperty = "_AdditiveColor";

	public const string GlowColorProperty = "_GlowColor";

	public const string GlowPowerProperty = "_GlowPower";

	public const string OutlineProperty = "_Outline";

	public const string OpacityProperty = "_Opacity";

	public const string FlashProperty = "_Flash";

	public const string BlendOpProperty = "_BlendOp";

	public const string BlendFirstProperty = "_BlendFirst";

	public const string BlendSecondProperty = "_BlendSecond";

	public const string OverlayTexturePower = "_OverlayTexturePower";

	public const string OverlayTexture = "_OverlayTexture";

	public const string NoiseTexturePower = "_NoiseTexturePower";

	public const string NoiseTexture = "_NoiseTexture";

	public const string PixelScaleProperty = "_PixelScale";

	public const string GreyscaleProperty = "_Greyscale";

	public const string HideOutlineProperty = "_HideOutline";

	public const string GlowTexProperty = "_GlowTex";

	public const string OutlineTexProperty = "_OutlineTex";

	public const string UVBoundsProperty = "_UVBounds";

	public static int OverlayColorID;

	public static int AdditiveColorID;

	public static int GlowColorID;

	public static int GlowPowerID;

	public static int OutlineID;

	public static int OpacityID;

	public static int FlashID;

	public static int BlendOpID;

	public static int BlendFirstID;

	public static int BlendSecondID;

	public static int OverlayTexturePowerID;

	public static int OverlayTextureID;

	public static int NoiseTexturePowerID;

	public static int NoiseTextureID;

	public static int PixelScaleID;

	public static int GreyscaleID;

	public static int HideOutlineID;

	public static int GlowTexID;

	public static int OutlineTexID;

	public static int UVBoundsID;

	[NonSerialized]
	public int frameChanged = -1;

	[SerializeField]
	private int uid;

	[SerializeField]
	private Color _overlayColor;

	[SerializeField]
	private Color _additiveColor;

	[SerializeField]
	private Color _borderColor;

	[SerializeField]
	private float _glowPower;

	[SerializeField]
	private float _opacity;

	[SerializeField]
	private int _outline;

	[SerializeField]
	private float _black;

	[SerializeField]
	private int _greyscale;

	[SerializeField]
	private float _overlayTexturePower;

	[SerializeField]
	private float _noiseTexturePower;

	[SerializeField]
	private Texture2D _overlayTexture;

	[SerializeField]
	private Texture2D _noiseTexture;

	[SerializeField]
	private BlendMode _blendFirst = BlendMode.One;

	[SerializeField]
	private BlendMode _blendSecond = BlendMode.OneMinusSrcAlpha;

	[SerializeField]
	private BlendOp _blendOp;

	public Color overlayColor
	{
		get
		{
			return _overlayColor;
		}
		set
		{
			_overlayColor = value;
			SetFrameChanged();
		}
	}

	public Color additiveColor
	{
		get
		{
			return _additiveColor;
		}
		set
		{
			_additiveColor = value;
			SetFrameChanged();
		}
	}

	public Color borderColor
	{
		get
		{
			return _borderColor;
		}
		set
		{
			_borderColor = value;
			SetFrameChanged();
		}
	}

	public float glowPower
	{
		get
		{
			return _glowPower;
		}
		set
		{
			_glowPower = value;
			SetFrameChanged();
		}
	}

	public float opacity
	{
		get
		{
			return _opacity;
		}
		set
		{
			_opacity = value;
			SetFrameChanged();
		}
	}

	public int outline
	{
		get
		{
			return _outline;
		}
		set
		{
			_outline = value;
			SetFrameChanged();
		}
	}

	public float black
	{
		get
		{
			return _black;
		}
		set
		{
			_black = value;
			SetFrameChanged();
		}
	}

	public int greyscale
	{
		get
		{
			return _greyscale;
		}
		set
		{
			_greyscale = value;
			SetFrameChanged();
		}
	}

	public BlendMode blendFirst
	{
		get
		{
			return _blendFirst;
		}
		set
		{
			_blendFirst = value;
			SetFrameChanged();
		}
	}

	public BlendMode blendSecond
	{
		get
		{
			return _blendSecond;
		}
		set
		{
			_blendSecond = value;
			SetFrameChanged();
		}
	}

	public BlendOp blendOp
	{
		get
		{
			return _blendOp;
		}
		set
		{
			_blendOp = value;
			SetFrameChanged();
		}
	}

	public float overlayTexturePower
	{
		get
		{
			return _overlayTexturePower;
		}
		set
		{
			_overlayTexturePower = value;
			SetFrameChanged();
		}
	}

	public Texture2D overlayTexture
	{
		get
		{
			return _overlayTexture;
		}
		set
		{
			_overlayTexture = value;
			SetFrameChanged();
		}
	}

	public float noiseTexturePower
	{
		get
		{
			return _noiseTexturePower;
		}
		set
		{
			_noiseTexturePower = value;
			SetFrameChanged();
		}
	}

	public Texture2D noiseTexture
	{
		get
		{
			return _noiseTexture;
		}
		set
		{
			_noiseTexture = value;
			SetFrameChanged();
		}
	}

	public static void Setup()
	{
		OverlayColorID = Shader.PropertyToID("_ColorOverlay");
		AdditiveColorID = Shader.PropertyToID("_AdditiveColor");
		GlowColorID = Shader.PropertyToID("_GlowColor");
		GlowPowerID = Shader.PropertyToID("_GlowPower");
		OutlineID = Shader.PropertyToID("_Outline");
		OpacityID = Shader.PropertyToID("_Opacity");
		FlashID = Shader.PropertyToID("_Flash");
		BlendOpID = Shader.PropertyToID("_BlendOp");
		BlendFirstID = Shader.PropertyToID("_BlendFirst");
		BlendSecondID = Shader.PropertyToID("_BlendSecond");
		OverlayTexturePowerID = Shader.PropertyToID("_OverlayTexturePower");
		OverlayTextureID = Shader.PropertyToID("_OverlayTexture");
		NoiseTexturePowerID = Shader.PropertyToID("_NoiseTexturePower");
		NoiseTextureID = Shader.PropertyToID("_NoiseTexture");
		PixelScaleID = Shader.PropertyToID("_PixelScale");
		GreyscaleID = Shader.PropertyToID("_Greyscale");
		HideOutlineID = Shader.PropertyToID("_HideOutline");
		GlowTexID = Shader.PropertyToID("_GlowTex");
		OutlineTexID = Shader.PropertyToID("_OutlineTex");
		UVBoundsID = Shader.PropertyToID("_UVBounds");
	}

	public RDShaderProperties()
	{
		_glowPower = 1f;
		_opacity = 1f;
		_black = 0f;
		_blendOp = BlendOp.Add;
		_blendFirst = BlendMode.One;
		_blendSecond = BlendMode.OneMinusSrcAlpha;
		_overlayTexturePower = 0f;
		_noiseTexturePower = 0f;
		SetFrameChanged();
	}

	public RDShaderProperties(Color overlayColor, Color borderColor, float glowPower, bool borderType)
	{
		_borderColor = borderColor;
		_glowPower = glowPower;
		_overlayColor = overlayColor;
		_outline = (borderType ? 1 : 0);
		_opacity = 1f;
		_blendOp = BlendOp.Add;
		_overlayTexturePower = 0f;
		_noiseTexturePower = 0f;
	}

	public void SetPropertiesInRenderer(Renderer renderer)
	{
		Material material = renderer.material;
		material.SetColor(OverlayColorID, Color.Lerp(_overlayColor, Color.black, _black));
		material.SetColor(GlowColorID, _borderColor);
		material.SetColor(AdditiveColorID, _additiveColor);
		material.SetFloat(GlowPowerID, _glowPower);
		material.SetFloat(OpacityID, _opacity);
		material.SetFloat(OutlineID, _outline);
		material.SetFloat(BlendOpID, (float)_blendOp);
		material.SetInt(BlendFirstID, (int)_blendFirst);
		material.SetFloat(BlendSecondID, (float)_blendSecond);
		material.SetFloat(OverlayTexturePowerID, _overlayTexturePower);
		material.SetFloat(NoiseTexturePowerID, _noiseTexturePower);
		material.SetFloat(GreyscaleID, _greyscale);
		material.SetFloat(PixelScaleID, GC.RoomRenderTextureScale);
		if (overlayTexture != null)
		{
			material.SetTexture("_OverlayTexture", _overlayTexture);
		}
		if (noiseTexture != null)
		{
			material.SetTexture("_NoiseTexture", _noiseTexture);
		}
	}

	private void SetFrameChanged()
	{
		frameChanged = RDTime.currentFrame;
	}

	public void CopyFrom(RDShaderProperties other)
	{
		_overlayColor = other.overlayColor;
		_additiveColor = other.additiveColor;
		_borderColor = other.borderColor;
		_glowPower = other.glowPower;
		_black = other.black;
		_greyscale = other.greyscale;
		_opacity = other.opacity;
		_outline = other.outline;
		_blendOp = other.blendOp;
		_overlayTexturePower = other.overlayTexturePower;
		_overlayTexture = other.overlayTexture;
		_noiseTexturePower = other.noiseTexturePower;
		_noiseTexture = other.noiseTexture;
		SetFrameChanged();
	}

	public override string ToString()
	{
		return $"[RDShaderProperties ]";
	}
}
