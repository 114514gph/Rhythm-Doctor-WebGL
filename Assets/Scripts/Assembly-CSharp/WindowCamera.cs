using UnityEngine;

public class WindowCamera : RDCamera
{
	public void RenderManually(Window window)
	{
		camera.targetTexture = window.renderTexture;
		UpdateCameraSize(window.index);
		RDBase.Vfx.oneshotSpotlights.Layout(window.index);
		camera.Render();
	}

	protected override void UpdateCameraSize(int windowIndex = -1)
	{
		base.UpdateCameraSize(windowIndex);
		if (windowIndex != -1)
		{
			Vector2 vector = (base.game.windowDancing ? base.game.windowChoreographer.dancers[windowIndex].windowScale : Vector2.one);
			camera.orthographicSize *= vector.y;
			camera.aspect = 1.7777778f * vector.x / vector.y;
		}
	}
}
