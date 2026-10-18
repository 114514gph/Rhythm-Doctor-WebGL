using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("CameraFilterPack/VHS/Real VHS HQ")]
public class CameraFilterPack_Real_VHS : MonoBehaviour
{
	public Shader SCShader;

	private Material SCMaterial;

	private Texture2D VHS;

	private Texture2D VHS2;

	[Range(0f, 1f)]
	public float TRACKING = 0.212f;

	[Range(0f, 1.5f)]
	public float Constrast = 1f;

	public static float ChangeDistortion;

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
		SCShader = Shader.Find("CameraFilterPack/Real_VHS");
		VHS = Resources.Load("CameraFilterPack_VHS1") as Texture2D;
		VHS2 = Resources.Load("CameraFilterPack_VHS2") as Texture2D;
	}

	public static Texture2D GetRTPixels(Texture2D t, RenderTexture rt, int sx, int sy)
	{
		RenderTexture active = RenderTexture.active;
		RenderTexture.active = rt;
		t.ReadPixels(new Rect(0f, 0f, t.width, t.height), 0, 0);
		RenderTexture.active = active;
		return t;
	}

	private void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
	{
		if (SCShader != null)
		{
			material.SetTexture("VHS", VHS);
			material.SetTexture("VHS2", VHS2);
			material.SetFloat("TRACKING", TRACKING);
			material.SetFloat("CONTRAST", 1f - Constrast);
			int height = 576;
			RenderTexture temporary = RenderTexture.GetTemporary(382, height, 0);
			temporary.filterMode = FilterMode.Trilinear;
			Graphics.Blit(sourceTexture, temporary, material);
			Graphics.Blit(temporary, destTexture);
			RenderTexture.ReleaseTemporary(temporary);
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
}
