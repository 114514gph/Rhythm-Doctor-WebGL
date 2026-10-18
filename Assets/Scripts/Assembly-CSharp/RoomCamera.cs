using System;
using UnityEngine;

public class RoomCamera : RDCamera
{
	[NonSerialized]
	public RDRoom room;

	protected override void UpdateCameraSize(int windowIndex = -1)
	{
		base.UpdateCameraSize(windowIndex);
		if (camera.targetTexture != null)
		{
			Vector2 vector = customScale;
			camera.orthographicSize *= vector.y;
			camera.aspect = 1.7777778f * vector.x / vector.y;
		}
	}

	public void OnBlittingPass()
	{
		if (!base.game.windowDancing)
		{
			return;
		}
		WindowChoreographer windowChoreographer = base.game.windowChoreographer;
		Vector2Int resolutionForScale = scrVfxControl.GetResolutionForScale(windowChoreographer.GetWindowScaleForDancing());
		WindowDancer[] dancers = windowChoreographer.dancers;
		foreach (WindowDancer windowDancer in dancers)
		{
			if (windowDancer.roomIndex == room.index)
			{
				Vector2Int viewSize = windowDancer.window.GetViewSize();
				Vector2 vector = ((viewSize == Vector2.zero) ? Vector2.one : ((Vector2)viewSize / (Vector2)resolutionForScale));
				Window window = windowDancer.window;
				Material material = window.material;
				material.SetFloat("_WindowScaleX", vector.x);
				material.SetFloat("_WindowScaleY", vector.y);
				material.SetColor("_BackgroundColor", Color.black);
				Color value = (window.isFrameless ? Color.clear : window.borderColor);
				material.SetColor("_BorderColor", value);
				window.ClearRenderTexture();
				Graphics.Blit(room.renderTexture, window.renderTexture, material);
				if (window.shouldRenderUI)
				{
					base.game.uiCamera.RenderManually(window);
				}
			}
		}
	}
}
