using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Camera Filter Pack/Noise/TV_3")]
public class CameraFilterPack_Noise_TV_3 : MonoBehaviour
{
	public Shader SCShader;

	private float TimeX = 1f;

	private Material SCMaterial;

	[Range(0f, 1f)]
	public float Fade = 1f;

	[Range(0f, 1f)]
	public float Fade_Additive;

	[Range(0f, 1f)]
	public float Fade_Distortion;

	[Range(0f, 10f)]
	private float Value4 = 1f;

	public static float ChangeValue;

	public static float ChangeValue2;

	public static float ChangeValue3;

	public static float ChangeValue4;

	private Texture2D Texture2;

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
		Texture2 = Resources.Load("CameraFilterPack_TV_Noise3") as Texture2D;
		ChangeValue = Fade;
		ChangeValue2 = Fade_Additive;
		ChangeValue3 = Fade_Distortion;
		ChangeValue4 = Value4;
		SCShader = Shader.Find("CameraFilterPack/Noise_TV_3");
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
			material.SetFloat("_Value", Fade);
			material.SetFloat("_Value2", Fade_Additive);
			material.SetFloat("_Value3", Fade_Distortion);
			material.SetFloat("_Value4", Value4);
			material.SetVector("_ScreenResolution", new Vector4(sourceTexture.width, sourceTexture.height, 0f, 0f));
			material.SetTexture("Texture2", Texture2);
			Graphics.Blit(sourceTexture, destTexture, material);
		}
		else
		{
			Graphics.Blit(sourceTexture, destTexture);
		}
	}

	private void OnValidate()
	{
		ChangeValue = Fade;
		ChangeValue2 = Fade_Additive;
		ChangeValue3 = Fade_Distortion;
		ChangeValue4 = Value4;
	}

	private void Update()
	{
		if (Application.isPlaying)
		{
			Fade = ChangeValue;
			Fade_Additive = ChangeValue2;
			Fade_Distortion = ChangeValue3;
			Value4 = ChangeValue4;
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
