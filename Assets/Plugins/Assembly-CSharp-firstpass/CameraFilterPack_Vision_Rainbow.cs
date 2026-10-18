using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Camera Filter Pack/Vision/Rainbow")]
public class CameraFilterPack_Vision_Rainbow : MonoBehaviour
{
	public Shader SCShader;

	private float TimeX = 1f;

	private Material SCMaterial;

	[Range(0f, 10f)]
	public float Speed = 1f;

	[Range(0f, 1f)]
	public float PosX = 0.5f;

	[Range(0f, 1f)]
	public float PosY = 0.5f;

	[Range(0f, 5f)]
	public float Colors = 0.5f;

	[Range(0f, 1f)]
	public float Vision = 0.5f;

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
		ChangeValue = Speed;
		ChangeValue2 = PosX;
		ChangeValue3 = PosY;
		ChangeValue4 = Colors;
		ChangeValue5 = Vision;
		SCShader = Shader.Find("CameraFilterPack/Vision_Rainbow");
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
			material.SetFloat("_Value2", PosX);
			material.SetFloat("_Value3", PosY);
			material.SetFloat("_Value4", Colors);
			material.SetFloat("_Value5", Vision);
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
		ChangeValue2 = PosX;
		ChangeValue3 = PosY;
		ChangeValue4 = Colors;
		ChangeValue5 = Vision;
	}

	private void Update()
	{
		if (Application.isPlaying)
		{
			Speed = ChangeValue;
			PosX = ChangeValue2;
			PosY = ChangeValue3;
			Colors = ChangeValue4;
			Vision = ChangeValue5;
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
