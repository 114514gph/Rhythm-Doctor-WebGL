using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Camera Filter Pack/FX/Screens")]
public class CameraFilterPack_FX_Screens : MonoBehaviour
{
	public Shader SCShader;

	private float TimeX = 1f;

	private Material SCMaterial;

	[Range(0f, 256f)]
	public float Tiles = 8f;

	[Range(0f, 5f)]
	public float Speed = 0.25f;

	[Range(-1f, 1f)]
	public float PosX;

	[Range(-1f, 1f)]
	public float PosY;

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
		ChangeValue = Tiles;
		ChangeValue2 = Speed;
		ChangeValue3 = PosX;
		ChangeValue4 = PosY;
		SCShader = Shader.Find("CameraFilterPack/FX_Screens");
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
			material.SetFloat("_Value", Tiles);
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
		ChangeValue = Tiles;
		ChangeValue2 = Speed;
		ChangeValue3 = PosX;
		ChangeValue4 = PosY;
	}

	private void Update()
	{
		if (Application.isPlaying)
		{
			Tiles = ChangeValue;
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
