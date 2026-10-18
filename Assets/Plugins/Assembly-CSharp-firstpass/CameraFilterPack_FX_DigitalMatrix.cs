using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Camera Filter Pack/FX/DigitalMatrix")]
public class CameraFilterPack_FX_DigitalMatrix : MonoBehaviour
{
	public Shader SCShader;

	private float TimeX = 1f;

	private Material SCMaterial;

	[Range(0.4f, 5f)]
	public float Size = 1f;

	[Range(-10f, 10f)]
	public float Speed = 1f;

	[Range(-1f, 1f)]
	public float ColorR = -1f;

	[Range(-1f, 1f)]
	public float ColorG = 1f;

	[Range(-1f, 1f)]
	public float ColorB = -1f;

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
		ChangeValue2 = ColorR;
		ChangeValue3 = ColorG;
		ChangeValue4 = ColorB;
		ChangeValue5 = Speed;
		SCShader = Shader.Find("CameraFilterPack/FX_DigitalMatrix");
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
			material.SetFloat("_Value2", ColorR);
			material.SetFloat("_Value3", ColorG);
			material.SetFloat("_Value4", ColorB);
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
		ChangeValue2 = ColorR;
		ChangeValue3 = ColorG;
		ChangeValue4 = ColorB;
		ChangeValue5 = Speed;
	}

	private void Update()
	{
		if (Application.isPlaying)
		{
			Size = ChangeValue;
			ColorR = ChangeValue2;
			ColorG = ChangeValue3;
			ColorB = ChangeValue4;
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
