using UnityEngine;

public class BlitterCamera : RDBase
{
	public Camera simulatedWDCamera;

	private Camera camera;

	private new scnGame game;

	private void Start()
	{
		camera = GetComponent<Camera>();
		game = scnGame.instance;
		camera.enabled = true;
	}

	private void LateUpdate()
	{
		camera.enabled = game.windowDancing;
	}

	private void OnRenderImage(RenderTexture src, RenderTexture dest)
	{
		game.topCamera.OnBlittingPass();
		if (game.rooms != null)
		{
			RDRoom[] rooms = game.rooms;
			for (int i = 0; i < rooms.Length; i++)
			{
				rooms[i].rdCamera.OnBlittingPass();
			}
		}
		if (game.windowChoreographer is RealWindowChoreographer realWindowChoreographer)
		{
			Window window = realWindowChoreographer.dancers[0].window;
			game.pauseMenuCamera.RenderManually(window);
			Graphics.Blit(window.renderTexture, dest);
		}
		else
		{
			Graphics.Blit(game.topCamera.camera.targetTexture, dest);
		}
		if (!simulatedWDCamera.enabled)
		{
			simulatedWDCamera.Render();
		}
	}
}
