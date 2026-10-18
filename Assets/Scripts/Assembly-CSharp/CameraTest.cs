using System;
using System.IO;
using UnityEngine;

public class CameraTest : RDBase
{
	private void OnPostRender()
	{
		printe($"[{Time.frameCount}] SimulateCamera.OnPostRender()" + Environment.StackTrace);
	}

	private void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
		Graphics.Blit(src, dest);
		printe($"[{Time.frameCount}] SimulateCamera.OnRenderImage() " + Environment.StackTrace);
		if (RDC.pauseEditorAtTheEndOfThisFrame)
		{
			string text = RDUtils.GetHomePath() + $"/Desktop/rt{Time.frameCount}.png";
			if (File.Exists(text))
			{
				File.Delete(text);
			}
			RDUtils.SaveRenderTextureToFile(src, text);
		}
	}
}
