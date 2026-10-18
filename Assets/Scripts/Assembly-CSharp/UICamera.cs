using UnityEngine;
using UnityEngine.UI;

public class UICamera : RDCamera
{
	public CanvasScaler[] canvasScalers = new CanvasScaler[0];

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

	public void RenderManually(Window window)
	{
		camera.targetTexture = window.renderTexture;
		UpdateCameraSize(window.index);
		WindowChoreographer windowChoreographer = base.game.windowChoreographer;
		if (windowChoreographer != null && window.shouldRenderUI)
		{
			Vector2 windowScale = windowChoreographer.dancers[window.index].windowScale;
			Vector2 referenceResolution = new Vector2(352f * windowScale.x, 198f * windowScale.y);
			CanvasScaler[] array = canvasScalers;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].referenceResolution = referenceResolution;
			}
		}
		camera.Render();
	}
}
