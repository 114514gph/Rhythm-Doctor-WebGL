using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Camera Filter Pack/Old Film/Old_Movie_2")]
public class CameraFilterPack_TV_Old_Movie_2 : MonoBehaviour
{
	public Shader SCShader;

	private float TimeX = 1f;

	private Material SCMaterial;

	[Range(1f, 60f)]
	public float FramePerSecond = 15f;

	[Range(0f, 5f)]
	public float Contrast = 1f;

	[Range(0f, 4f)]
	public float Burn;

	[Range(0f, 16f)]
	public float SceneCut = 1f;

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
		ChangeValue = FramePerSecond;
		ChangeValue2 = Contrast;
		ChangeValue3 = Burn;
		ChangeValue4 = SceneCut;
		SCShader = Shader.Find("CameraFilterPack/TV_Old_Movie_2");
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
			material.SetFloat("_Value", FramePerSecond);
			material.SetFloat("_Value2", Contrast);
			material.SetFloat("_Value3", Burn);
			material.SetFloat("_Value4", SceneCut);
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
		ChangeValue = FramePerSecond;
		ChangeValue2 = Contrast;
		ChangeValue3 = Burn;
		ChangeValue4 = SceneCut;
	}

	private void Update()
	{
		if (Application.isPlaying)
		{
			FramePerSecond = ChangeValue;
			Contrast = ChangeValue2;
			Burn = ChangeValue3;
			SceneCut = ChangeValue4;
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
