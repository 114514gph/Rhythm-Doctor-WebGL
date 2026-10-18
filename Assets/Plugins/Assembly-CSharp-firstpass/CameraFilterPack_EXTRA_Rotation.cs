using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Camera Filter Pack/EXTRA/Rotation")]
public class CameraFilterPack_EXTRA_Rotation : MonoBehaviour
{
	public Shader SCShader;

	private float TimeX = 1f;

	private Material SCMaterial;

	[Range(-360f, 360f)]
	public float Rotation;

	[Range(-1f, 2f)]
	public float PositionX = 0.5f;

	[Range(-1f, 2f)]
	public float PositionY = 0.5f;

	[Range(0f, 10f)]
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
		ChangeValue = Rotation;
		ChangeValue2 = PositionX;
		ChangeValue3 = PositionY;
		ChangeValue4 = Value4;
		SCShader = Shader.Find("CameraFilterPack/EXTRA_Rotation");
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
			material.SetFloat("_Value", 0f - Rotation);
			material.SetFloat("_Value2", PositionX);
			material.SetFloat("_Value3", PositionY);
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
		ChangeValue = Rotation;
		ChangeValue2 = PositionX;
		ChangeValue3 = PositionY;
		ChangeValue4 = Value4;
	}

	private void Update()
	{
		if (Application.isPlaying)
		{
			Rotation = ChangeValue;
			PositionX = ChangeValue2;
			PositionY = ChangeValue3;
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
