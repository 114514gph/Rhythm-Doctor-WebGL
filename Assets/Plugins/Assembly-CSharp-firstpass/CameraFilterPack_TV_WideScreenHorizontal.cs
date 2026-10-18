using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Camera Filter Pack/TV/WideScreenHorizontal")]
public class CameraFilterPack_TV_WideScreenHorizontal : MonoBehaviour
{
	public Shader SCShader;

	private float TimeX = 1f;

	private Material SCMaterial;

	[Range(0f, 0.8f)]
	public float Size = 0.55f;

	[Range(0.001f, 0.4f)]
	public float Smooth = 0.01f;

	[Range(0f, 10f)]
	private float StretchX = 1f;

	[Range(0f, 10f)]
	private float StretchY = 1f;

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
		ChangeValue = Size;
		ChangeValue2 = Smooth;
		ChangeValue3 = StretchX;
		ChangeValue4 = StretchY;
		SCShader = Shader.Find("CameraFilterPack/TV_WideScreenHorizontal");
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
			material.SetFloat("_Value2", Smooth);
			material.SetFloat("_Value3", StretchX);
			material.SetFloat("_Value4", StretchY);
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
		ChangeValue2 = Smooth;
		ChangeValue3 = StretchX;
		ChangeValue4 = StretchY;
	}

	private void Update()
	{
		if (Application.isPlaying)
		{
			Size = ChangeValue;
			Smooth = ChangeValue2;
			StretchX = ChangeValue3;
			StretchY = ChangeValue4;
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
