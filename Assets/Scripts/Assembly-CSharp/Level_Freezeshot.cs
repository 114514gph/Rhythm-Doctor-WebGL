using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using RDLevelEditor;
using UnityEngine;

public class Level_Freezeshot : LevelBase
{
	private RDAthleteWard _stadium;

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

	private Color afterimageColor = Color.white;

	private float scaleTarget = 1f;

	private RDAthleteWard stadium
	{
		get
		{
			if (_stadium == null)
			{
				_stadium = base.room2.athleteWard.GetComponent<RDAthleteWard>();
			}
			return _stadium;
		}
	}

	public Level_Freezeshot(RDLevelData data)
		: base(data)
	{
	}

	public override void OnHit(HitType hitType, Beat beat = null)
	{
		stadium.HitBaseball(hitType, beat);
	}

	public override void preactions()
	{
		int bar = base.conductor.barNumber;
		stadium.PrepareBaseballs(bar);
	}

	public override IEnumerator LoadBigAssets()
	{
		afterimageRenderers = new List<Renderer>();
		afterimageTextures = new List<RenderTexture>();
		for (int i = 0; i < 16; i++)
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
		if (num < lightPatterns.Length)
		{
			string text = lightPatterns[num];
			for (int i = 0; i < text.Length; i++)
			{
				bool lightShouldBeOn = text[i] == '1';
				int x = i;
				scrExecuteOnCertainBeat.Add(() =>
				{
					stadium.usingLights = true;
					if (lightShouldBeOn)
					{
						TurnOnLight(x);
					}
					else
					{
						stadium.ClearAllLights();
					}
				}, lightTimings[i]);
			}
		}
		else
		{
			stadium.usingLights = false;
		}
	}

	private void TurnOnLight(int index)
	{
		bool flag = index < 8;
		if (!flag)
		{
			index = 8 - (index - 8);
		}
		char value = ((index % 2 == 0) ? '°' : 'o');
		stadium.ClearAllLights();
		stadium.SetScoreboardLightWithChar(flag, index, value);
	}

	[ListedMethod(false)]
	public void SetAfterimageParams(float pasteSeconds, float fadeSeconds, string colorHex, float size)
	{
		pasteIntervalSeconds = pasteSeconds;
		fadeTimeSeconds = fadeSeconds;
		afterimageColor = colorHex.HexToColor();
		scaleTarget = size;
	}

	[ListedMethod(false)]
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
		mat.SetFloat("_Scale", 1f);
		mat.SetColor("_ColorOverlay", afterimageColor);
		DOTween.To(() => mat.GetFloat("_Opacity"), (float o) =>
		{
			mat.SetFloat("_Opacity", o);
		}, 0f, fadeTimeSeconds);
		if (scaleTarget != 1f)
		{
			DOTween.To(() => mat.GetFloat("_Scale"), (float o) =>
			{
				mat.SetFloat("_Scale", o);
			}, scaleTarget, fadeTimeSeconds);
		}
		currentTextureIndex++;
		if (currentTextureIndex >= afterimageRenderers.Count)
		{
			currentTextureIndex = 0;
		}
	}
}
