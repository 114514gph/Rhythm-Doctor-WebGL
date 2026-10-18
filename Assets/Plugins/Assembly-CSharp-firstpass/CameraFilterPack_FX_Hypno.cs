using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Camera Filter Pack/FX/Hypno")]
public class CameraFilterPack_FX_Hypno : MonoBehaviour
{
	public Shader SCShader;

	private float TimeX = 1f;

	private Material SCMaterial;

	[Range(0f, 1f)]
	public float Speed = 1f;

	[Range(-2f, 2f)]
	public float Red;

	[Range(-2f, 2f)]
	public float Green = 1f;

	[Range(-2f, 2f)]
	public float Blue = 1f;

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
		ChangeValue2 = Red;
		ChangeValue3 = Green;
		ChangeValue4 = Blue;
		SCShader = Shader.Find("CameraFilterPack/FX_Hypno");
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
			material.SetFloat("_Value2", Red);
			material.SetFloat("_Value3", Green);
			material.SetFloat("_Value4", Blue);
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
		ChangeValue2 = Red;
		ChangeValue3 = Green;
		ChangeValue4 = Blue;
	}

	private void Update()
	{
		if (Application.isPlaying)
		{
			Speed = ChangeValue;
			Red = ChangeValue2;
			Green = ChangeValue3;
			Blue = ChangeValue4;
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
