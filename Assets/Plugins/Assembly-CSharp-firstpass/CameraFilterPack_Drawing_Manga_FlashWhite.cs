using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Camera Filter Pack/Drawing/Manga_FlashWhite")]
public class CameraFilterPack_Drawing_Manga_FlashWhite : MonoBehaviour
{
	public Shader SCShader;

	private float TimeX = 1f;

	private Material SCMaterial;

	[Range(1f, 10f)]
	public float Size = 1f;

	[Range(0f, 30f)]
	public int Speed = 5;

	[Range(-1f, 1f)]
	public float PosX = 0.5f;

	[Range(-1f, 1f)]
	public float PosY = 0.5f;

	public static float ChangeValue;

	public static int ChangeValue2;

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
		ChangeValue2 = Speed;
		ChangeValue3 = PosX;
		ChangeValue4 = PosY;
		SCShader = Shader.Find("CameraFilterPack/Drawing_Manga_FlashWhite");
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
			material.SetFloat("_Value2", Speed);
			material.SetFloat("_Value3", PosX);
			material.SetFloat("_Value4", PosY);
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
		ChangeValue2 = Speed;
		ChangeValue3 = PosX;
		ChangeValue4 = PosY;
	}

	private void Update()
	{
		if (Application.isPlaying)
		{
			Size = ChangeValue;
			Speed = ChangeValue2;
			PosX = ChangeValue3;
			PosY = ChangeValue4;
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
