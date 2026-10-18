using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Camera Filter Pack/Vision/Aura")]
public class CameraFilterPack_Vision_Aura : MonoBehaviour
{
	public Shader SCShader;

	private float TimeX = 1f;

	private Material SCMaterial;

	[Range(0f, 2f)]
	public float Twist = 1f;

	[Range(-4f, 4f)]
	public float Speed = 1f;

	public Color Color = new Color(0.16f, 0.57f, 0.19f);

	[Range(-1f, 2f)]
	public float PosX = 0.5f;

	[Range(-1f, 2f)]
	public float PosY = 0.5f;

	public static float ChangeValue;

	public static Color ChangeValue2;

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
		ChangeValue = Twist;
		ChangeValue2 = Color;
		ChangeValue3 = PosX;
		ChangeValue4 = PosY;
		ChangeValue5 = Speed;
		SCShader = Shader.Find("CameraFilterPack/Vision_Aura");
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
			material.SetFloat("_Value", Twist);
			material.SetColor("_Value2", Color);
			material.SetFloat("_Value3", PosX);
			material.SetFloat("_Value4", PosY);
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
		ChangeValue = Twist;
		ChangeValue2 = Color;
		ChangeValue3 = PosX;
		ChangeValue4 = PosY;
		ChangeValue5 = Speed;
	}

	private void Update()
	{
		if (Application.isPlaying)
		{
			Twist = ChangeValue;
			Color = ChangeValue2;
			PosX = ChangeValue3;
			PosY = ChangeValue4;
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
