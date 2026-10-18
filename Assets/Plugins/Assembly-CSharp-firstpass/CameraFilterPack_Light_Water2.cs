using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Camera Filter Pack/Light/Water2")]
public class CameraFilterPack_Light_Water2 : MonoBehaviour
{
	public Shader SCShader;

	private float TimeX = 1f;

	private Material SCMaterial;

	[Range(0f, 10f)]
	public float Speed = 0.2f;

	[Range(0f, 10f)]
	public float Speed_X = 0.2f;

	[Range(0f, 1f)]
	public float Speed_Y = 0.3f;

	[Range(0f, 10f)]
	public float Intensity = 2.4f;

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
		ChangeValue2 = Speed_X;
		ChangeValue3 = Speed_Y;
		ChangeValue4 = Intensity;
		SCShader = Shader.Find("CameraFilterPack/Light_Water2");
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
			material.SetFloat("_Value2", Speed_X);
			material.SetFloat("_Value3", Speed_Y);
			material.SetFloat("_Value4", Intensity);
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
		ChangeValue2 = Speed_X;
		ChangeValue3 = Speed_Y;
		ChangeValue4 = Intensity;
	}

	private void Update()
	{
		if (Application.isPlaying)
		{
			Speed = ChangeValue;
			Speed_X = ChangeValue2;
			Speed_Y = ChangeValue3;
			Intensity = ChangeValue4;
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
