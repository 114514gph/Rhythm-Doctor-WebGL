using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Camera Filter Pack/ColorsAdjust/ColorRGB")]
public class CameraFilterPack_Colors_Adjust_ColorRGB : MonoBehaviour
{
	public Shader SCShader;

	private float TimeX = 1f;

	private Material SCMaterial;

	[Range(-2f, 2f)]
	public float Red;

	[Range(-2f, 2f)]
	public float Green;

	[Range(-2f, 2f)]
	public float Blue;

	[Range(-1f, 1f)]
	public float Brightness;

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
		ChangeValue = Red;
		ChangeValue2 = Green;
		ChangeValue3 = Blue;
		ChangeValue4 = Brightness;
		SCShader = Shader.Find("CameraFilterPack/Colors_Adjust_ColorRGB");
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
			material.SetFloat("_Value", Red);
			material.SetFloat("_Value2", Green);
			material.SetFloat("_Value3", Blue);
			material.SetFloat("_Value4", Brightness);
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
		ChangeValue = Red;
		ChangeValue2 = Green;
		ChangeValue3 = Blue;
		ChangeValue4 = Brightness;
	}

	private void Update()
	{
		if (Application.isPlaying)
		{
			Red = ChangeValue;
			Green = ChangeValue2;
			Blue = ChangeValue3;
			Brightness = ChangeValue4;
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
