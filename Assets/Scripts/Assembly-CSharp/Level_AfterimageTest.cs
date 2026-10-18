using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Level_AfterimageTest : LevelBase
{
	private List<Renderer> afterimageRenderers;

	private List<RenderTexture> afterimageTextures;

	private scrPasteToRenderTex renderTexPaster;

	private int currentTextureIndex;

	public override IEnumerator LoadBigAssets()
	{
		afterimageRenderers = new List<Renderer>();
		afterimageTextures = new List<RenderTexture>();
		for (int i = 0; i < 4; i++)
		{
			int roomScale = base.game.roomScale;
			RenderTexture renderTexture = RDClass.Vfx.CreateRenderTexture(roomScale);
			afterimageTextures.Add(renderTexture);
			Vector2 vector = RDClass.Vfx.hudCamera.RDFullNativeResolution();
			GameObject gameObject = Object.Instantiate(base.room0.renderTextureQuadPrefab, base.gm.parentContainer);
			gameObject.transform.MoveX(3000f + vector.x / 2f);
			gameObject.transform.MoveY(vector.y / 2f);
			gameObject.transform.localScale = new Vector3(vector.x, vector.y, 1f);
			gameObject.name = "Afterimage Quad " + i;
			Renderer component = gameObject.GetComponent<Renderer>();
			component.material.mainTexture = renderTexture;
			component.enabled = false;
			afterimageRenderers.Add(component);
		}
		renderTexPaster = base.room0.renderTexPaster;
		renderTexPaster.enabled = true;
		yield return null;
	}

	public void PasteCurrentFrame()
	{
		Renderer renderer = afterimageRenderers[currentTextureIndex];
		renderer.enabled = true;
		renderTexPaster.SetPasteTarget(afterimageTextures[currentTextureIndex]);
		Material mat = renderer.material;
		mat.SetFloat("_Opacity", 1f);
		DOTween.To(() => mat.GetFloat("_Opacity"), (float o) =>
		{
			mat.SetFloat("_Opacity", o);
		}, 0f, 1f);
		currentTextureIndex++;
		if (currentTextureIndex >= afterimageRenderers.Count)
		{
			currentTextureIndex = 0;
		}
	}
}
