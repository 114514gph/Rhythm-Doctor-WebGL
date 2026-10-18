using UnityEngine;

public class TwoColorTintFilter : MonoBehaviour
{
	public Shader twoColorTint;

	private Material mMaterial;

	public Color colorA;

	public Color colorB;

	private Material material
	{
		get
		{
			if (mMaterial == null)
			{
				mMaterial = new Material(twoColorTint);
				mMaterial.hideFlags = HideFlags.HideAndDontSave;
			}
			return mMaterial;
		}
	}

	private void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
	{
		if (twoColorTint != null)
		{
			material.SetColor("_ColorA", colorA);
			material.SetColor("_ColorB", colorB);
			Graphics.Blit(sourceTexture, destTexture, material);
		}
		else
		{
			Graphics.Blit(sourceTexture, destTexture);
		}
	}
}
