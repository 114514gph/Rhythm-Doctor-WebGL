using System;
using System.Collections;
using DG.Tweening;
using RDLevelEditor;
using UnityEngine;

public class Level_HaileyDuet : LevelBase
{
	private BubbleMask bubbleMask;

	private Background sunset;

	private Background city;

	private Background foreground;

	private Background glow;

	private Background couple;

	private Background[] intimateBackgrounds;

	private RDCoffeeShop coffeeShopBg;

	private RDCoffeeShop coffeeShopFg;

	private int nicoleSortingOrder;

	private int nicoleSortingLayer;

	public RowEntity nicole => base.game.rows[12].ent;

	public RowEntity logan => base.game.rows[1].ent;

	public RDCoffeeShop cafe => base.game.rooms[0].coffeeShop.GetComponent<RDCoffeeShop>();

	public Level_HaileyDuet(RDLevelData data)
		: base(data)
	{
	}

	public override IEnumerator LoadBigAssets()
	{
		bubbleMask = LoadLevelAsset("RDAthleteWard/Injury/BubbleMask").GetComponent<BubbleMask>();
		bubbleMask.shapes.Add(new GameObject("Bubble Shape Circle").AddComponent<CircleCollider2D>());
		bubbleMask.shapes.Add(new GameObject("Bubble Shape Square").AddComponent<BoxCollider2D>());
		bubbleMask.transform.SetParent(base.game.rooms[3].transform, worldPositionStays: false);
		foreach (Collider2D shape in bubbleMask.shapes)
		{
			shape.transform.parent = bubbleMask.transform;
			if (shape is CircleCollider2D circleCollider2D)
			{
				circleCollider2D.radius = 1f;
			}
		}
		int room = 1;
		intimateBackgrounds = new Background[4]
		{
			(sunset = Background.Create("IntimateNew_Sunset/0", 0f, 0f, isForeground: false, room).SetFollowCamera(followCamera: false)),
			(city = Background.Create("IntimateNew_CityScape/0", 0f, 0f, isForeground: false, room).SetFollowCamera(followCamera: false)),
			(couple = Background.Create("Intimate_CoupleSS/0", 0f, 0f, isForeground: false, room).SetFollowCamera(followCamera: false)),
			(foreground = Background.Create("IntimateNew_Foreground/0", 0f, 0f, isForeground: false, room).SetFollowCamera(followCamera: false))
		};
		SunsetVisible(visible: false);
		yield return null;
	}

	public override void preactions()
	{
		if (base.conductor.barNumber == 1)
		{
			base.game.rooms[0].coleWard.GetComponent<RDHoodieBoyWard>().showSpectrum = false;
			base.room3.renderTexture.Release();
			UnityEngine.Object.Destroy(base.room3.renderTexture);
			RenderTexture renderTexture = RDClass.Vfx.CreateRenderTexture(1);
			base.room3.renderTexture = renderTexture;
			base.room3.camera.targetTexture = renderTexture;
			base.room3.quadMaterial.mainTexture = renderTexture;
		}
	}

	public void SunsetVisible(bool visible)
	{
		Background[] array = intimateBackgrounds;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].visible = visible;
		}
	}

	public void SunsetUmbrella(float fps)
	{
		couple.SetAnimation("Intimate_CoupleSS", new int[5] { 1, 2, 3, 4, 5 });
		couple.RewindAnimation();
		couple.SetFPS(fps);
		couple.PlayAnimation();
		couple.loopAnimation = false;
	}

	public void CafeVisible(bool visible)
	{
		cafe.countertopSpecial.SetActive(visible);
		MeshRenderer component = nicole.character.sprite.GetComponent<MeshRenderer>();
		if (visible)
		{
			nicoleSortingOrder = component.sortingOrder;
			nicoleSortingLayer = component.sortingLayerID;
			MeshRenderer component2 = cafe.countertopSpecial.GetComponent<MeshRenderer>();
			component.sortingOrder = component2.sortingOrder - 1;
			component.sortingLayerID = component2.sortingLayerID;
		}
		else
		{
			component.sortingOrder = nicoleSortingOrder;
			component.sortingLayerID = nicoleSortingLayer;
		}
	}

	public void CafeMan(bool visible)
	{
		cafe.SlideCoffeeMan(visible);
	}

	public void ThrowCoffee()
	{
		tk2dSprite cup = cafe.cups_4[0];
		Vector3 baristaPosition = nicole.character.transform.position + new Vector3(-15f, 16f);
		float height = 60f;
		DOVirtual.DelayedCall(1f / 6f, () =>
		{
			cup.gameObject.SetActive(value: true);
			cup.transform.position = baristaPosition;
			cup.transform.DOJump(logan.character.transform.position + Vector3.up * 8f + Vector3.right * -4f, height, 1, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
			{
				cup.transform.DOShakePosition(0.2f, 1f, 10, 90f, snapping: true);
			});
			cup.Rotate2D(0f);
			cup.transform.DORotate(new Vector3(0f, 0f, 720f), 0.5f, RotateMode.FastBeyond360).SetEase(Ease.InOutCubic).OnComplete(() =>
			{
				float duration = UnityEngine.Random.Range(0.1f, 0.5f);
				cup.transform.DOJump(cup.transform.position + new Vector3(-10f, -25f), 10f, 1, duration).SetEase(Ease.Linear);
				cup.transform.DORotate(new Vector3(0f, 0f, 355f), duration).SetEase(Ease.Linear);
				cup.DOFade(0f, duration).SetEase(Ease.Linear);
			});
		});
	}

	public override void actions()
	{
	}

	public void RunBubbleMask()
	{
		bubbleMask.playing = true;
	}

	public void PauseBubbleMask()
	{
		bubbleMask.playing = false;
	}

	public void StopBubbleMask()
	{
		ParticleSystem[] bubbles = bubbleMask.bubbles;
		for (int i = 0; i < bubbles.Length; i++)
		{
			bubbles[i].Stop();
		}
	}

	public void DisableBubbleMask()
	{
		bubbleMask.gameObject.SetActive(value: false);
	}

	public void EnableBubbleMask()
	{
		bubbleMask.gameObject.SetActive(value: true);
	}

	public void MoveBubbleShape(int shapeID, float x, float y, float duration, string easeString)
	{
		Ease ease = Enum.Parse<Ease>(easeString);
		Transform transform = bubbleMask.shapes[shapeID].transform;
		Vector2 vector = new Vector2(x / 100f * 352f, y / 100f * 198f);
		transform.DOLocalMove(vector, duration).SetEase(ease);
	}

	public void ScaleBubbleShape(int shapeID, float x, float y, float duration, string easeString)
	{
		Ease ease = Enum.Parse<Ease>(easeString);
		bubbleMask.shapes[shapeID].transform.DOScale(new Vector2(x, y), duration).SetEase(ease);
	}

	public void SetBubbleSpeed(float s)
	{
		ParticleSystem[] bubbles = bubbleMask.bubbles;
		for (int i = 0; i < bubbles.Length; i++)
		{
			ParticleSystem.MainModule main = bubbles[i].main;
			main.simulationSpeed = s * UnityEngine.Random.Range(0.8f, 1.2f);
		}
	}
}
