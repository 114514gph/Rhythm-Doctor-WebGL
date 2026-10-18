using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Camera Filter Pack/Blur/Radial")]
public class CameraFilterPack_Blur_Radial : MonoBehaviour
{
	public Shader SCShader;

	private float TimeX = 1f;

	private Material SCMaterial;

	[Range(-0.5f, 0.5f)]
	public float Intensity = 0.125f;

	[Range(-2f, 2f)]
	public float MovX = 0.5f;

	[Range(-2f, 2f)]
	public float MovY = 0.5f;

	[Range(0f, 10f)]
	private float blurWidth = 1f;

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
		ChangeValue = Intensity;
		ChangeValue2 = MovX;
		ChangeValue3 = MovY;
		ChangeValue4 = blurWidth;
		SCShader = Shader.Find("CameraFilterPack/Blur_Radial");
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
			material.SetFloat("_Value2", MovX);
			material.SetFloat("_Value3", MovY);
			material.SetFloat("_Value4", blurWidth);
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
		ChangeValue2 = MovX;
		ChangeValue3 = MovY;
		ChangeValue4 = blurWidth;
	}

	private void Update()
	{
		if (Application.isPlaying)
		{
			Intensity = ChangeValue;
			MovX = ChangeValue2;
			MovY = ChangeValue3;
			blurWidth = ChangeValue4;
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
