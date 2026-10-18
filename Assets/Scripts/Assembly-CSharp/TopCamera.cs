using UnityEngine;

public class TopCamera : RDCamera
{
	private Material material;

	public override void Awake()
	{
		base.Awake();
		material = new Material(base.gc.renderTextureShader);
	}

	protected override void UpdateCameraSize(int windowIndex = -1)
	{
		base.UpdateCameraSize(windowIndex);
		Vector2 vector = (base.game.windowDancing ? (Vector2.one * 2f) : Vector2.one);
		camera.orthographicSize *= vector.y;
		camera.aspect = 1.7777778f * vector.x / vector.y;
	}

	private void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
		if (base.game.windowChoreographer == null)
		{
			material.SetColor("_BorderColor", base.game.flashBorderColor);
			Graphics.Blit(src, dest, material);
		}
		else
		{
			Graphics.Blit(src, dest);
		}
	}

	public void OnBlittingPass()
	{
		if (!base.game.windowDancing)
		{
			return;
		}
		WindowDancer[] dancers = base.game.windowChoreographer.dancers;
		foreach (WindowDancer windowDancer in dancers)
		{
			if (windowDancer.roomIndex == -1)
			{
				RenderTexture targetTexture = camera.targetTexture;
				Window window = windowDancer.window;
				Material material = window.material;
				material.SetFloat("_WindowScaleX", windowDancer.windowScale.x / 2f);
				material.SetFloat("_WindowScaleY", windowDancer.windowScale.y / 2f);
				Color value = (window.isFrameless ? Color.clear : window.borderColor);
				material.SetColor("_BorderColor", value);
				window.ClearRenderTexture();
				Graphics.Blit(targetTexture, window.renderTexture, material);
				base.game.windowCamera.RenderManually(window);
				if (window.shouldRenderUI)
				{
					base.game.uiCamera.RenderManually(window);
				}
			}
		}
	}
}
