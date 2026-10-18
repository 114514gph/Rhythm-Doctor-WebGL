using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Camera Filter Pack/Distortion/Heat")]
public class CameraFilterPack_Distortion_Heat : MonoBehaviour
{
	public Shader SCShader;

	public Texture bumpMap;

	public Vector2 bumpMapScale;

	public Vector2 speed;

	private float time = 1f;

	private Material SCMaterial;

	[Range(0f, 1f)]
	public float Distortion = 0.2f;

	public Vector2 Position;

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
		SCShader = Shader.Find("CameraFilterPack/Distortion_Heat");
	}

	private void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
	{
		if (SCShader != null)
		{
			time += Time.deltaTime;
			material.SetFloat("_TimeX", time);
			material.SetFloat("_Distortion", Distortion);
			material.SetVector("_Speed", speed);
			material.SetVector("_CenterPosition", Position);
			material.SetTexture("_BumpMap", bumpMap);
			material.SetTextureScale("_BumpMap", bumpMapScale);
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
	}

	private void Update()
	{
	}

	private void OnDisable()
	{
		if ((bool)SCMaterial)
		{
			Object.DestroyImmediate(SCMaterial);
		}
	}
}
