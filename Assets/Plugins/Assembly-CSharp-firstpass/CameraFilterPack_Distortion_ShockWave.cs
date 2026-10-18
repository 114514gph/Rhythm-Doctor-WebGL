using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Camera Filter Pack/Distortion/ShockWave")]
public class CameraFilterPack_Distortion_ShockWave : MonoBehaviour
{
	public Shader SCShader;

	private float TimeX = 1f;

	private Material SCMaterial;

	[Range(-1.5f, 1.5f)]
	public float PosX = 0.5f;

	[Range(-1.5f, 1.5f)]
	public float PosY = 0.5f;

	[Range(0f, 10f)]
	public float Speed = 1f;

	[Range(0f, 10f)]
	private float Size = 1f;

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
		ChangeValue = PosX;
		ChangeValue2 = PosY;
		ChangeValue3 = Speed;
		ChangeValue4 = Size;
		SCShader = Shader.Find("CameraFilterPack/Distortion_ShockWave");
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
			material.SetFloat("_Value", PosX);
			material.SetFloat("_Value2", PosY);
			material.SetFloat("_Value3", Speed);
			material.SetFloat("_Value4", Size);
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
		ChangeValue = PosX;
		ChangeValue2 = PosY;
		ChangeValue3 = Speed;
		ChangeValue4 = Size;
	}

	private void Update()
	{
		if (Application.isPlaying)
		{
			PosX = ChangeValue;
			PosY = ChangeValue2;
			Speed = ChangeValue3;
			Size = ChangeValue4;
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
