using System;
using DG.Tweening;
using UnityEngine;

public class RDTiledBackground : RDBase
{
	public Vector2 speed;

	public Vector2 pulseOffset;

	public RDSetSortingLayer setSortingLayer;

	[NonSerialized]
	public Material material;

	private Texture2D[] textures;

	private bool scrolling;

	private bool pulsing;

	private double initialTime;

	private float fps = 1f;

	private scrExecuteOnPeriodicBeat pulseEvent;

	private Tween pulseTween;

	private void Awake()
	{
		material = GetComponent<Renderer>().material;
	}

	private void Update()
	{
		if (scrolling)
		{
			float num = RDTime.speed * Time.deltaTime;
			material.mainTextureOffset = new Vector2((100f + material.mainTextureOffset.x - num * speed.x) % 1f, (100f + material.mainTextureOffset.y - num * speed.y) % 1f);
		}
		else if (pulsing)
		{
			material.mainTextureOffset = new Vector2((100f - pulseOffset.x) % 1f, (100f - pulseOffset.y) % 1f);
		}
		int num2 = (int)Math.Floor((AudioSettings.dspTime - initialTime) * (double)RDTime.speed * (double)fps % (double)textures.Length);
		if (num2 >= 0 && num2 < textures.Length)
		{
			material.mainTexture = textures[num2];
		}
	}

	public void Show(Texture2D[] textures, ContentMode contentMode = ContentMode.ScaleToFill, FilterMode filterMode = FilterMode.Point, Color color = default(Color), RDSortingLayer sortingLayer = RDSortingLayer.Background, int sortingOrder = 0, Vector2 scrollingSpeed = default(Vector2), float fps = 0f, float duration = 0f, Ease ease = Ease.Linear, bool tileMovementIsScroll = true, bool dontSetSortingOrder = false, float pulseIntervalBeats = 1f, float pulseStartBeat = 0f)
	{
		Texture2D texture = textures[0];
		this.textures = textures;
		if (texture == null)
		{
			Debug.LogWarning("ShowCustomBackground(): texture is null");
			material.color = Color.clear;
			return;
		}
		initialTime = AudioSettings.dspTime;
		base.gameObject.SetActive(value: true);
		base.enabled = true;
		this.fps = fps;
		for (int i = 0; i < textures.Length; i++)
		{
			textures[i].filterMode = filterMode;
		}
		bool flag = false;
		if (material.mainTexture != texture)
		{
			material.mainTexture = texture;
		}
		else
		{
			flag = true;
		}
		material.mainTexture.wrapMode = TextureWrapMode.Repeat;
		material.color = color;
		Transform transform = base.transform;
		float rDWidth = RDBase.Vfx.RDWidth;
		float rDHeight = RDBase.Vfx.RDHeight;
		if (!flag || contentMode != ContentMode.Tiled)
		{
			material.mainTextureOffset = Vector2.zero;
		}
		material.mainTextureScale = Vector2.one;
		bool flag2 = pulsing;
		bool flag3 = scrolling;
		scrolling = false;
		pulsing = false;
		pulseTween?.Kill();
		pulseEvent?.Remove();
		switch (contentMode)
		{
		case ContentMode.ScaleToFill:
			transform.localScale = new Vector3(rDWidth, rDHeight, 1f);
			break;
		case ContentMode.AspectFit:
			transform.localScale = ((texture.width > texture.height) ? new Vector3(rDWidth, rDWidth * (float)texture.height / (float)texture.width, 1f) : new Vector3(rDHeight * (float)texture.width / (float)texture.height, rDHeight, 1f));
			break;
		case ContentMode.AspectFill:
			transform.localScale = ((texture.width > texture.height) ? new Vector3(rDHeight * (float)texture.width / (float)texture.height, rDHeight, 1f) : new Vector3(rDWidth, rDWidth * (float)texture.height / (float)texture.width, 1f));
			break;
		case ContentMode.Center:
			transform.localScale = new Vector3(texture.width, texture.height, 1f);
			break;
		case ContentMode.Tiled:
			transform.localScale = new Vector3(rDWidth, rDHeight, 1f);
			material.mainTextureScale = new Vector2(rDWidth / (float)texture.width, rDHeight / (float)texture.height);
			scrolling = tileMovementIsScroll;
			pulsing = !tileMovementIsScroll;
			if (tileMovementIsScroll)
			{
				if (flag2)
				{
					speed = Vector2.zero;
				}
				DOTween.To(() => speed.x, (float s) =>
				{
					speed.x = s;
				}, scrollingSpeed.x / (float)texture.width, duration).SetEase(ease);
				DOTween.To(() => speed.y, (float s) =>
				{
					speed.y = s;
				}, scrollingSpeed.y / (float)texture.height, duration).SetEase(ease);
			}
			else
			{
				if (flag3)
				{
					pulseOffset = Vector2.zero;
				}
				speed = scrollingSpeed;
				pulseEvent = scrExecuteOnPeriodicBeat.Add(() =>
				{
					pulseTween?.Kill(complete: true);
					Vector2 currentOffset = new Vector2(pulseOffset.x, pulseOffset.y);
					Vector2 textureDimensions = new Vector2(texture.width, texture.height);
					pulseTween = DOTween.To(() => pulseOffset, (Vector2 v) =>
					{
						pulseOffset = v;
					}, currentOffset + speed / textureDimensions, duration).SetEase(ease).OnComplete(() =>
					{
						pulseOffset = currentOffset + speed / textureDimensions;
					});
				}, pulseIntervalBeats, pulseStartBeat);
			}
			Update();
			break;
		}
		setSortingLayer.sortingLayer = sortingLayer;
		if (!dontSetSortingOrder)
		{
			setSortingLayer.sortingOrder = sortingOrder;
		}
	}

	public void Hide()
	{
		base.gameObject.SetActive(value: false);
	}
}
