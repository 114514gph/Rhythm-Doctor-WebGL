using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Camera Filter Pack/Light/Water")]
public class CameraFilterPack_Light_Water : MonoBehaviour
{
	public Shader SCShader;

	private float TimeX = 1f;

	private Material SCMaterial;

	[Range(0f, 1f)]
	public float Size = 4f;

	[Range(0f, 2f)]
	public float Alpha = 0.07f;

	[Range(0f, 32f)]
	public float Distance = 10f;

	[Range(-2f, 2f)]
	public float Speed = 0.4f;

	public static float ChangeAlpha;

	public static float ChangeDistance;

	public static float ChangeSize;

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
		ChangeAlpha = Alpha;
		ChangeDistance = Distance;
		ChangeSize = Size;
		SCShader = Shader.Find("CameraFilterPack/Light_Water");
	}

	private void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
	{
		if (SCShader != null)
		{
			TimeX += Time.deltaTime * Speed;
			if (TimeX > 100f)
			{
				TimeX = 0f;
			}
			material.SetFloat("_TimeX", TimeX);
			material.SetFloat("_Alpha", Alpha);
			material.SetFloat("_Distance", Distance);
			material.SetFloat("_Size", Size);
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
		ChangeAlpha = Alpha;
		ChangeDistance = Distance;
		ChangeSize = Size;
	}

	private void Update()
	{
		if (Application.isPlaying)
		{
			Alpha = ChangeAlpha;
			Distance = ChangeDistance;
			Size = ChangeSize;
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
