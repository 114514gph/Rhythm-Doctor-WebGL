using UnityEngine;

public class scrPasteToRenderTex : RDBase
{
	public RenderTexture targetTexture;

	public Material material;

	public bool readyToPaste;

	private void OnRenderImage(RenderTexture source, RenderTexture dest)
	{
		source.filterMode = FilterMode.Point;
		if (targetTexture != null)
		{
			targetTexture.filterMode = FilterMode.Point;
		}
		if (readyToPaste && targetTexture != null)
		{
			Graphics.Blit(source, targetTexture, material);
			readyToPaste = false;
		}
		Graphics.Blit(source, dest);
	}

	public void SetPasteTarget(RenderTexture target)
	{
		targetTexture = target;
		readyToPaste = true;
	}

	public void Reset()
	{
		readyToPaste = false;
	}
}
