using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Camera Filter Pack/Flip/Flip_Screen")]
public class CameraFilterPack_FlipScreen : MonoBehaviour
{
	public Shader SCShader;

	public bool flipX;

	public bool flipY;

	private Material SCMaterial;

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
		SCShader = Shader.Find("CameraFilterPack/FlipScreen");
	}

	private void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
	{
		if (SCShader != null)
		{
			material.SetFloat("_FlipX", flipX ? 1f : 0f);
			material.SetFloat("_FlipY", flipY ? 1f : 0f);
			material.SetVector("_ScreenResolution", new Vector4(sourceTexture.width, sourceTexture.height, 0f, 0f));
			Graphics.Blit(sourceTexture, destTexture, material);
		}
		else
		{
			Graphics.Blit(sourceTexture, destTexture);
		}
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

	public void UpdateState()
	{
		base.enabled = flipX || flipY;
	}
}
