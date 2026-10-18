using UnityEngine;

public class PrintOnRenderImageFrame : RDBase
{
	private void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
		printe($"[{Time.frameCount}] {base.name}.OnRenderImage()");
		Graphics.Blit(src, dest);
	}
}
