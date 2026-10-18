using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Camera Filter Pack/Glitch/Mozaic")]
public class CameraFilterPack_Glitch_Mozaic : MonoBehaviour
{
	public Shader SCShader;

	private float TimeX = 1f;

	private Material SCMaterial;

	[Range(0f, 10f)]
	public float Intensity = 1f;

	[Range(0f, 10f)]
	private float Value2 = 1f;

	[Range(0f, 10f)]
	private float Value3 = 1f;

	[Range(0f, 10f)]
	private float Value4 = 1f;

	public float ChangeValue;

	public float ChangeValue2;

	public float ChangeValue3;

	public float ChangeValue4;

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

	private void Awake()
	{
		ChangeValue = Intensity;
		ChangeValue2 = Value2;
		ChangeValue3 = Value3;
		ChangeValue4 = Value4;
		SCShader = Shader.Find("CameraFilterPack/Glitch_Mozaic");
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
			material.SetFloat("_Value", Intensity);
			material.SetFloat("_Value2", Value2);
			material.SetFloat("_Value3", Value3);
			material.SetFloat("_Value4", Value4);
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
		ChangeValue = Intensity;
		ChangeValue2 = Value2;
		ChangeValue3 = Value3;
		ChangeValue4 = Value4;
	}

	private void Update()
	{
		if (Application.isPlaying)
		{
			Intensity = ChangeValue;
			Value2 = ChangeValue2;
			Value3 = ChangeValue3;
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
