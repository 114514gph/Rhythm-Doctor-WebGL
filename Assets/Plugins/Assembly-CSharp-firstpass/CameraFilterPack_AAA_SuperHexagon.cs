using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Camera Filter Pack/AAA/Super Hexagon")]
public class CameraFilterPack_AAA_SuperHexagon : MonoBehaviour
{
	public Shader SCShader;

	[Range(0f, 1f)]
	public float _AlphaHexa = 1f;

	private float TimeX = 1f;

	private Material SCMaterial;

	[Range(0.2f, 10f)]
	public float HexaSize = 2.5f;

	public float _BorderSize = 1f;

	public Color _BorderColor = new Color(0.75f, 0.75f, 1f, 1f);

	public Color _HexaColor = new Color(0f, 0.5f, 1f, 1f);

	public float _SpotSize = 2.5f;

	public static float ChangeBorderSize = 1f;

	public static Color ChangeBorderColor;

	public static Color ChangeHexaColor;

	public static float ChangeSpotSize = 1f;

	public static float ChangeAlphaHexa = 1f;

	public static float ChangeValue;

	public Vector2 center = new Vector2(0.5f, 0.5f);

	public float Radius = 0.25f;

	public static Vector2 Changecenter;

	public static float ChangeRadius;

	private Material material
	{
		get
		{
			if (SCMaterial == null)
			{
				SCMaterial = new Material(SCShader);
				SCMaterial.hideFlags = HideFlags.HideAndDontSave;
			}
			return SCMaterial;
		}
	}

	private void Start()
	{
		Changecenter = center;
		ChangeRadius = Radius;
		ChangeValue = HexaSize;
		ChangeAlphaHexa = _AlphaHexa;
		ChangeBorderSize = _BorderSize;
		ChangeBorderColor = _BorderColor;
		ChangeHexaColor = _HexaColor;
		ChangeSpotSize = _SpotSize;
		SCShader = Shader.Find("CameraFilterPack/AAA_Super_Hexagon");
	}

	private void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
	{
		if (SCShader != null)
		{
			TimeX += Time.deltaTime;
			if (TimeX > 100f)
			{
				TimeX = 0f;
			}
			material.SetFloat("_TimeX", TimeX);
			material.SetFloat("_Value", HexaSize);
			material.SetFloat("_PositionX", center.x);
			material.SetFloat("_PositionY", center.y);
			material.SetFloat("_Radius", Radius);
			material.SetFloat("_BorderSize", _BorderSize);
			material.SetColor("_BorderColor", _BorderColor);
			material.SetColor("_HexaColor", _HexaColor);
			material.SetFloat("_AlphaHexa", _AlphaHexa);
			material.SetFloat("_SpotSize", _SpotSize);
			material.SetVector("_ScreenResolution", new Vector4(sourceTexture.width, sourceTexture.height, 0f, 0f));
			Graphics.Blit(sourceTexture, destTexture, material);
		}
		else
		{
			Graphics.Blit(sourceTexture, destTexture);
		}
	}

	private void OnValidate()
	{
		ChangeValue = HexaSize;
		Changecenter = center;
		ChangeRadius = Radius;
		ChangeBorderSize = _BorderSize;
		ChangeBorderColor = _BorderColor;
		ChangeHexaColor = _HexaColor;
		ChangeSpotSize = _SpotSize;
		ChangeAlphaHexa = _AlphaHexa;
	}

	private void Update()
	{
		if (Application.isPlaying)
		{
			HexaSize = ChangeValue;
			center = Changecenter;
			Radius = ChangeRadius;
			_BorderSize = ChangeBorderSize;
			_BorderColor = ChangeBorderColor;
			_HexaColor = ChangeHexaColor;
			_SpotSize = ChangeSpotSize;
			_AlphaHexa = ChangeAlphaHexa;
		}
	}

	private void OnDisable()
	{
		if ((bool)SCMaterial)
		{
			Object.DestroyImmediate(SCMaterial);
		}
	}
}
