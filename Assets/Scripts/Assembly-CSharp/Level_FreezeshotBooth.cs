using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using RDLevelEditor;
using UnityEngine;

public class Level_FreezeshotBooth : LevelBase
{
	private const int lightsNumber = 16;

	private GameObject[] lightBackgrounds = new GameObject[16];

	private List<Renderer> afterimageRenderers;

	private List<RenderTexture> afterimageTextures;

	private scrPasteToRenderTex renderTexPaster;

	private Sequence afterimageSequence;

	private readonly float[] lightTimings = new float[16]
	{
		0f, 0.25f, 0.5f, 0.625f, 0.75f, 1f, 1.25f, 1.75f, 2f, 2.25f,
		2.5f, 2.75f, 3f, 3.125f, 3.25f, 3.5f
	};

	private readonly string[] lightPatterns = new string[19]
	{
		"0000000000000000", "0000000000000000", "1111111111111000", "1111111111111111", "1111111111111000", "1111111111111111", "0000000000000000", "1111111111111111", "0000000000000000", "1111111110101111",
		"0000000000000000", "0000000000000000", "0000000000000000", "0000000000000000", "0000000000000000", "0000000000000000", "1111111111111111", "0000000000000000", "1111111111111111"
	};

	private int currentTextureIndex;

	private float pasteIntervalSeconds;

	private float fadeTimeSeconds;

	public Level_FreezeshotBooth(RDLevelData data)
		: base(data)
	{
	}

	public override IEnumerator LoadBigAssets()
	{
		afterimageRenderers = new List<Renderer>();
		afterimageTextures = new List<RenderTexture>();
		for (int i = 0; i < 6; i++)
		{
			int scale = ((!base.game.currentLevel.fullResolutionRenderTextures) ? GC.RoomRenderTextureScale : 0);
			RenderTexture renderTexture = RDClass.Vfx.CreateRenderTexture(scale);
			afterimageTextures.Add(renderTexture);
			Vector2 vector = RDClass.Vfx.hudCamera.RDFullNativeResolution();
			GameObject gameObject = Object.Instantiate(base.room0.renderTextureQuadPrefab, base.gm.parentContainer);
			gameObject.transform.MoveX(9000f + vector.x / 2f);
			gameObject.transform.MoveY(vector.y / 2f);
			gameObject.transform.localScale = new Vector3(vector.x, vector.y, 1f);
			gameObject.name = "Afterimage Quad " + i;
			Renderer component = gameObject.GetComponent<Renderer>();
			component.material.mainTexture = renderTexture;
			component.enabled = false;
			afterimageRenderers.Add(component);
		}
		renderTexPaster = base.room2.renderTexPaster;
		renderTexPaster.enabled = true;
		yield return null;
	}

	public override void actions()
	{
		int num = base.conductor.barNumber - 1;
		if (num == 1)
		{
			RDHoodieBoyWard component = base.room2.coleWard.GetComponent<RDHoodieBoyWard>();
			for (int i = 0; i < 16; i++)
			{
				foreach (Transform item in component.lightBackgroundsContainer.transform)
				{
					if (item.name.Contains(i.ToString()))
					{
						lightBackgrounds[i] = item.gameObject;
						break;
					}
				}
			}
		}
		if (num >= lightPatterns.Length)
		{
			return;
		}
		string text = lightPatterns[num];
		for (int j = 0; j < text.Length; j++)
		{
			bool lightShouldBeOn = text[j] == '1';
			int x = j;
			scrExecuteOnCertainBeat.Add(() =>
			{
				if (lightShouldBeOn)
				{
					TurnOnLight(x);
				}
				else
				{
					TurnOffLights();
				}
			}, lightTimings[j]);
		}
	}

	private void TurnOnLight(int index)
	{
		if (index < lightBackgrounds.Length)
		{
			TurnOffLights();
			lightBackgrounds[index].SetActive(value: true);
		}
	}

	private void TurnOffLights()
	{
		GameObject[] array = lightBackgrounds;
		for (int i = 0; i < array.Length; i++)
		{
			array[i]?.SetActive(value: false);
		}
	}

	public void SetAfterimageParams(float pasteSeconds, float fadeSeconds)
	{
		pasteIntervalSeconds = pasteSeconds;
		fadeTimeSeconds = fadeSeconds;
	}

	public void ToggleAfterimages(bool on)
	{
		afterimageSequence?.Kill();
		if (on)
		{
			afterimageSequence = DOTween.Sequence();
			afterimageSequence.AppendCallback(() =>
			{
				PasteCurrentFrame();
			}).AppendInterval(pasteIntervalSeconds);
			((Tween)afterimageSequence).SetLoops(-1);
		}
	}

	public void PasteCurrentFrame()
	{
		Renderer renderer = afterimageRenderers[currentTextureIndex];
		renderer.enabled = true;
		renderTexPaster.SetPasteTarget(afterimageTextures[currentTextureIndex]);
		Material mat = renderer.material;
		mat.SetFloat("_Opacity", 1f);
		mat.SetColor("_ColorOverlay", Color.cyan);
		DOTween.To(() => mat.GetFloat("_Opacity"), (float o) =>
		{
			mat.SetFloat("_Opacity", o);
		}, 0f, fadeTimeSeconds);
		currentTextureIndex++;
		if (currentTextureIndex >= afterimageRenderers.Count)
		{
			currentTextureIndex = 0;
		}
	}

	public void EnterIanHand(float timeMult)
	{
		SetHandToIan(1, rightHand: true);
		base.game.handController1.UpdateHandsPositions(HandUpdateType.RightHand, timeMult);
		base.game.handController1.transform.localPosition = base.game.handController1.transform.localPosition.WithY(9f);
	}
}
