using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Camera Filter Pack/FX/Earth Quake")]
public class CameraFilterPack_FX_EarthQuake : MonoBehaviour
{
	public Shader SCShader;

	private float TimeX = 1f;

	private Material SCMaterial;

	[Range(0f, 100f)]
	public float Speed = 15f;

	[Range(0f, 0.2f)]
	public float X = 0.008f;

	[Range(0f, 0.2f)]
	public float Y = 0.008f;

	[Range(0f, 0.2f)]
	private float Value4 = 1f;

	public static float ChangeValue;

	public static float ChangeValue2;

	public static float ChangeValue3;

	public static float ChangeValue4;

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
		ChangeValue = Speed;
		ChangeValue2 = X;
		ChangeValue3 = Y;
		ChangeValue4 = Value4;
		SCShader = Shader.Find("CameraFilterPack/FX_EarthQuake");
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
			material.SetFloat("_Value", Speed);
			material.SetFloat("_Value2", X);
			material.SetFloat("_Value3", Y);
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
		ChangeValue = Speed;
		ChangeValue2 = X;
		ChangeValue3 = Y;
		ChangeValue4 = Value4;
	}

	private void Update()
	{
		if (Application.isPlaying)
		{
			Speed = ChangeValue;
			X = ChangeValue2;
			Y = ChangeValue3;
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
