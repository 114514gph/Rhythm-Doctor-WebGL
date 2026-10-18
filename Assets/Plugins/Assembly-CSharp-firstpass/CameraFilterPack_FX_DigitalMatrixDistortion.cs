using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Camera Filter Pack/FX/DigitalMatrixDistortion")]
public class CameraFilterPack_FX_DigitalMatrixDistortion : MonoBehaviour
{
	public Shader SCShader;

	private float TimeX = 1f;

	private Material SCMaterial;

	[Range(0.4f, 5f)]
	public float Size = 1.4f;

	[Range(-2f, 2f)]
	public float Speed = 0.5f;

	[Range(-5f, 5f)]
	public float Distortion = 2.3f;

	public static float ChangeValue;

	public static float ChangeValue2;

	public static float ChangeValue3;

	public static float ChangeValue4;

	public static float ChangeValue5;

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
		ChangeValue = Size;
		ChangeValue2 = Distortion;
		ChangeValue5 = Speed;
		SCShader = Shader.Find("CameraFilterPack/FX_DigitalMatrixDistortion");
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
			material.SetFloat("_Value", Size);
			material.SetFloat("_Value2", Distortion);
			material.SetFloat("_Value5", Speed);
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
		ChangeValue = Size;
		ChangeValue2 = Distortion;
		ChangeValue5 = Speed;
	}

	private void Update()
	{
		if (Application.isPlaying)
		{
			Size = ChangeValue;
			Distortion = ChangeValue2;
			Speed = ChangeValue5;
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
