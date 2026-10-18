using System.Collections;
using RDLevelEditor;
using RDTools;
using UnityEngine;

public class VirtualWindowChoreographer : WindowChoreographer
{
	public SimulateWDController controller;

	public RenderTexture virtualDesktopRenderTexture;

	public override Vector2Int WindowDanceResolution
	{
		get
		{
			if (!(scnEditor.instance == null))
			{
				if (!setup)
				{
					return scrVfxControl.fullResolutionRenderTextureSize;
				}
				return new Vector2Int(virtualDesktopRenderTexture.width, virtualDesktopRenderTexture.height);
			}
			return new Vector2Int(Screen.width, Screen.height);
		}
	}

	public override IEnumerator Setup(int dancerCount)
	{
		RDClassDll.printem($"dancerCount: {dancerCount}");
		controller = base.game.simulateWD;
		controller.Setup(dancerCount);
		UICamera pauseMenuCamera = base.game.pauseMenuCamera;
		pauseMenuCamera.enabled = true;
		pauseMenuCamera.camera.enabled = true;
		pauseMenuCamera.camera.depth = 11f;
		base.game.simulateWD.graphicRaycaster.enabled = base.editor == null;
		if (base.editorMode)
		{
			float currentWindowDanceRatio = base.editor.currentWindowDanceRatio;
			Vector2Int fullResolutionRenderTextureSize = scrVfxControl.fullResolutionRenderTextureSize;
			Vector2Int size = ((currentWindowDanceRatio > 1.7777778f) ? new Vector2Int(Mathf.RoundToInt((float)fullResolutionRenderTextureSize.y * currentWindowDanceRatio), fullResolutionRenderTextureSize.y) : new Vector2Int(fullResolutionRenderTextureSize.x, Mathf.RoundToInt((float)fullResolutionRenderTextureSize.x / currentWindowDanceRatio)));
			printe("Virtual Desktop size: " + size);
			virtualDesktopRenderTexture = RDClass.Vfx.CreateRenderTexture(size, "VirtualDesktop");
			controller.camera.targetTexture = virtualDesktopRenderTexture;
			base.editor.gameView.texture = virtualDesktopRenderTexture;
			controller.camera.enabled = false;
		}
		dancers = new WindowDancer[dancerCount];
		for (int i = 0; i < dancerCount; i++)
		{
			VirtualWindow window = new VirtualWindow(controller, i, this);
			WindowDancer windowDancer = (dancers[i] = new WindowDancer(window, this));
			if (i > 0)
			{
				windowDancer.SetVisible(visible: false);
			}
		}
		for (int j = 0; j < dancerCount; j++)
		{
			controller.views[j].rawImage.texture = dancers[j].window.renderTexture;
		}
		controller.gameObject.SetActive(value: true);
		WindowDancer[] array = dancers;
		for (int k = 0; k < array.Length; k++)
		{
			array[k].window.ResetSize(this);
		}
		setup = true;
		yield break;
	}

	public override void SetWindowContentToRoom(int windowIndex, int roomIndex)
	{
		base.SetWindowContentToRoom(windowIndex, roomIndex);
		controller.views[windowIndex].rawImage.texture = dancers[windowIndex].window.renderTexture;
	}

	protected override IEnumerator PrepareForDancing()
	{
		changingWindowSize = true;
		CheckDancers();
		preparedFirstTime = true;
		changingWindowSize = false;
		yield break;
	}
}
