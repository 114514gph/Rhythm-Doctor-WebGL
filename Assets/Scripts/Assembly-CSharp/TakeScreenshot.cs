using DG.Tweening;
using UnityEngine;

public class TakeScreenshot : MonoBehaviour
{
	[Range(0f, 10f)]
	public int iterations = 2;

	[Range(0f, 1f)]
	public float blurSpread = 0.6f;

	public RenderTexture blurredTexture;

	public Material mat;

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		scnGame instance = scnGame.instance;
		if (instance.getScreenshot > 0)
		{
			instance.getScreenshot--;
		}
		if (instance.getScreenshot == 1)
		{
			scrVfxControl instance2 = scrVfxControl.instance;
			if (blurredTexture == null)
			{
				blurredTexture = new RenderTexture(instance2.RDWidthInt, instance2.RDHeightInt, source.depth);
			}
			mat.mainTexture = source;
			int width = blurredTexture.width / 4;
			int height = blurredTexture.height / 4;
			RenderTexture temporary = RenderTexture.GetTemporary(width, height, 0);
			DownSample4x(source, temporary);
			RenderTexture temporary2 = RenderTexture.GetTemporary(width, height, 0);
			for (int i = 0; i < iterations; i++)
			{
				FourTapCone(temporary, temporary2, i);
				temporary.DiscardContents();
				Graphics.Blit(temporary2, temporary);
				temporary2.DiscardContents();
			}
			RenderTexture.ReleaseTemporary(temporary2);
			Graphics.Blit(temporary, blurredTexture);
			RenderTexture.ReleaseTemporary(temporary);
			PauseMenu pauseMenu = instance.pauseMenu;
			pauseMenu.canvasGroup.DOKill();
			pauseMenu.backgroundCanvasGroup.alpha = 1f;
			pauseMenu.background.texture = blurredTexture;
			instance.pauseMenu.Show();
			instance.SetEnabledCameras(enabled: false);
		}
		if (instance.getScreenshot == 0)
		{
			base.enabled = false;
		}
		Graphics.Blit(source, destination);
	}

	public void FourTapCone(RenderTexture source, RenderTexture dest, int iteration)
	{
		float num = 0.5f + (float)iteration * blurSpread;
		Graphics.BlitMultiTap(source, dest, mat, new Vector2(0f - num, 0f - num), new Vector2(0f - num, num), new Vector2(num, num), new Vector2(num, 0f - num));
	}

	private void DownSample4x(RenderTexture source, RenderTexture dest)
	{
		float num = 1f;
		Graphics.BlitMultiTap(source, dest, mat, new Vector2(0f - num, 0f - num), new Vector2(0f - num, num), new Vector2(num, num), new Vector2(num, 0f - num));
	}
}
