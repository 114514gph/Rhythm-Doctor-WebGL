using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Camera Filter Pack/Vision/Crystal")]
public class CameraFilterPack_Vision_Crystal : MonoBehaviour
{
	public Shader SCShader;

	private float TimeX = 1f;

	private Material SCMaterial;

	[Range(-10f, 10f)]
	public float Value = 1f;

	[Range(-1f, 1f)]
	public float X = 1f;

	[Range(-1f, 1f)]
	public float Y = 1f;

	[Range(-1f, 1f)]
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
		ChangeValue = Value;
		ChangeValue2 = X;
		ChangeValue3 = Y;
		ChangeValue4 = Value4;
		SCShader = Shader.Find("CameraFilterPack/Vision_Crystal");
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
			material.SetFloat("_Value", Value);
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
		ChangeValue = Value;
		ChangeValue2 = X;
		ChangeValue3 = Y;
		ChangeValue4 = Value4;
	}

	private void Update()
	{
		if (Application.isPlaying)
		{
			Value = ChangeValue;
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
