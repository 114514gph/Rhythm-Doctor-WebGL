using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(tk2dTiledSprite))]
public class Background : InvisibleEntity
{
	public delegate void BackgroundCallback();

	private const float BackgroundZPosition = 10f;

	private const float SpeedMod = 60f;

	private Vector2 spriteSize;

	private Vector3 initialPosition;

	private Vector3 lastScale;

	private BackgroundData data;

	private Renderer rendererComponent;

	public bool loopAnimation = true;

	private bool mAnimated;

	private int currentFrame;

	private int[] mFrameIds = new int[0];

	private float mElapsedTime;

	public Vector2 velocity;

	public float fps = 4f;

	public bool updates = true;

	[Header("Debugging")]
	public Vector3 debug_newOffset;

	public Vector3 debug_translation;

	private BackgroundCallback onAnimationEnd;

	public tk2dTiledSprite tiledSprite => base.gameObject.GetComponent<tk2dTiledSprite>();

	public bool visible
	{
		get
		{
			return rendererComponent.enabled;
		}
		set
		{
			rendererComponent.enabled = value;
		}
	}

	public override void Awake()
	{
		base.Awake();
		rendererComponent = GetComponent<Renderer>();
	}

	private void Start()
	{
		updates = true;
		tiledSprite.SetSprite(data.spriteCollectionData, data.spriteName);
		tiledSprite.scale = data.scale;
		tiledSprite.anchor = tk2dBaseSprite.Anchor.MiddleCenter;
		UpdateFollowCamera();
		CalculateDimensions();
		rendererComponent.sortingLayerName = (data.isForeground ? RDSortingLayer.Foreground.ToString() : RDSortingLayer.Background.ToString());
		if (data.followCamera)
		{
			base.transform.localPosition = initialPosition;
		}
		if (data.renderQueue != 0)
		{
			rendererComponent.material.renderQueue = data.renderQueue;
		}
		velocity = data.velocity;
	}

	private void UpdateFollowCamera()
	{
		RDRoom rDRoom = base.game.rooms[data.room];
		Transform parent = (data.followCamera ? rDRoom.backgroundContainerCamera : rDRoom.backgroundContainerWorldspace);
		base.transform.SetParent(parent, worldPositionStays: true);
	}

	private void Update()
	{
		if (!updates)
		{
			return;
		}
		if (lastScale != tiledSprite.scale)
		{
			CalculateDimensions();
		}
		if (data.tileType != TileType.None)
		{
			Vector3 translation = new Vector3(velocity.x * Time.deltaTime * RDBase.Vfx.scrollXMultiplier * 60f, velocity.y * Time.deltaTime * RDBase.Vfx.scrollYMultiplier * 60f, 0f);
			base.transform.Translate(translation);
			debug_translation = translation;
			Vector3 vector = base.transform.localPosition - initialPosition;
			Vector3 vector2 = Vector3.zero;
			switch (data.tileType)
			{
			case TileType.Both:
				vector2 = new Vector3(RXMath.FlooredDivision(vector.x, spriteSize.x * lastScale.x), RXMath.FlooredDivision(vector.y, spriteSize.y * lastScale.y), 0f);
				break;
			case TileType.Horizontal:
				vector2 = new Vector3(RXMath.FlooredDivision(vector.x, spriteSize.x * lastScale.x), vector.y, 0f);
				break;
			case TileType.Vertical:
				vector2 = new Vector3(vector.x, RXMath.FlooredDivision(vector.y, spriteSize.y * lastScale.y), 0f);
				break;
			}
			debug_newOffset = vector2;
			if (data.fixedPosition)
			{
				tiledSprite.anchor = (data.followCamera ? tk2dBaseSprite.Anchor.MiddleCenter : tk2dBaseSprite.Anchor.LowerLeft);
				base.transform.localPosition = new Vector3(0f, 0f, 10f);
			}
			else
			{
				base.transform.localPosition = initialPosition + vector2;
			}
		}
		if (mAnimated)
		{
			mElapsedTime += Time.deltaTime;
			UpdateAnimation();
		}
	}

	private void LateUpdate()
	{
		if (shaderDataSource != null && lastFrameUpdatedGraphics < shaderDataSource.shaderData.frameChanged)
		{
			shaderDataSource.shaderData.SetPropertiesInRenderer(rendererComponent);
			lastFrameUpdatedGraphics = RDTime.currentFrame;
		}
	}

	private void CalculateDimensions()
	{
		lastScale = tiledSprite.scale;
		tk2dSpriteDefinition currentSpriteDef = tiledSprite.GetCurrentSpriteDef();
		spriteSize = currentSpriteDef.untrimmedBoundsData[1];
		if (!data.fixedPosition || data.forceTiling)
		{
			float num = RDBase.Vfx.RDWidth / Mathf.Abs(lastScale.x) + spriteSize.x;
			float num2 = RDBase.Vfx.RDHeight / Mathf.Abs(lastScale.y) + spriteSize.y;
			if (data.tileType == TileType.Both)
			{
				tiledSprite.dimensions = new Vector2(num, num2);
			}
			if (data.tileType == TileType.Vertical)
			{
				tiledSprite.dimensions = new Vector2(spriteSize.x, num2);
			}
			if (data.tileType == TileType.Horizontal)
			{
				tiledSprite.dimensions = new Vector2(num, spriteSize.y);
			}
			if (data.tileType == TileType.None)
			{
				tiledSprite.dimensions = spriteSize;
			}
		}
		else
		{
			tiledSprite.dimensions = spriteSize;
		}
		if (data.tileType == TileType.None)
		{
			if (data.followCamera)
			{
				initialPosition = new Vector3(0f, 0f, 10f);
			}
			else
			{
				initialPosition = new Vector3(RDBase.Vfx.RDWidth * 0.5f, RDBase.Vfx.RDHeight * 0.5f, 10f);
			}
		}
		else
		{
			TileType tileType = data.tileType;
			float num3 = ((tileType == TileType.Horizontal || tileType == TileType.Both) ? ((0f - spriteSize.x) * data.scale.x * 0.5f) : 0f);
			tileType = data.tileType;
			initialPosition = new Vector3(num3, (tileType == TileType.Vertical || tileType == TileType.Both) ? ((0f - spriteSize.y) * data.scale.y * 0.5f) : 0f, 10f);
		}
		base.transform.localPosition = RDBase.Vfx.hudCamera.CameraCenterWithZ(0f);
	}

	private void UpdateAnimation()
	{
		int num = currentFrame;
		bool flag = false;
		if (data.followBeatType == FollowBeatType.None)
		{
			num = Mathf.FloorToInt(mElapsedTime * fps % (float)mFrameIds.Length);
			if (currentFrame != num && num == mFrameIds.Length - 1 && !loopAnimation)
			{
				flag = true;
			}
		}
		if (data.followBeatType == FollowBeatType.HeadbobStyle)
		{
			num = Mathf.Clamp(Mathf.FloorToInt(mElapsedTime * fps), 0, mFrameIds.Length - 1);
		}
		if (mAnimated && num != currentFrame)
		{
			currentFrame = num;
			if (visible)
			{
				int num2 = mFrameIds[currentFrame];
				if (tiledSprite.spriteId != num2)
				{
					tiledSprite.spriteId = num2;
					if (data.renderQueue != 0)
					{
						rendererComponent.material.renderQueue = data.renderQueue;
					}
				}
			}
		}
		if (flag)
		{
			PauseAnimation();
			onAnimationEnd?.Invoke();
		}
	}

	public Background SetForeground(bool foreground)
	{
		data.isForeground = foreground;
		return this;
	}

	public Background SetRenderQueue(int renderQueue)
	{
		data.renderQueue = renderQueue;
		return this;
	}

	public Background SetColor(Color color)
	{
		StartCoroutine(SetColorCoroutine(color));
		return this;
	}

	public Background SetColor(uint color)
	{
		StartCoroutine(SetColorCoroutine(color.ARGBToColor()));
		return this;
	}

	public Background SetAlpha(float value)
	{
		StartCoroutine(SetAlphaCoroutine(value));
		return this;
	}

	public Background SetAlphaWithoutCoroutine(float value)
	{
		tiledSprite.color = tiledSprite.color.WithAlpha(value);
		return this;
	}

	private IEnumerator SetColorCoroutine(Color color)
	{
		yield return 0;
		tiledSprite.color = color;
	}

	private IEnumerator SetAlphaCoroutine(float value)
	{
		yield return 0;
		tiledSprite.color = tiledSprite.color.WithAlpha(value);
	}

	public void SetData(BackgroundData data)
	{
		this.data = data;
	}

	public Background SetFPS(float _fps)
	{
		fps = _fps;
		return this;
	}

	public Background FadeIn(float time = 1f)
	{
		visible = true;
		tiledSprite.color = tiledSprite.color.WithAlpha(0f);
		tiledSprite.DOFade(1f, time);
		return this;
	}

	public Background ScaleTween(float scale, float duration)
	{
		tiledSprite.DOScaleXY(scale, duration);
		return this;
	}

	public Background FadeOut(float time = 1f)
	{
		tiledSprite.DOFade(0f, time);
		return this;
	}

	public Background SetBackgroundOrder(int value)
	{
		tiledSprite.SortingOrder = value;
		return this;
	}

	public Background SetSortingLayer(string sortingLayer)
	{
		GetComponent<MeshRenderer>().sortingLayerName = sortingLayer;
		return this;
	}

	public Background SetLayer(int layer)
	{
		tiledSprite.gameObject.layer = layer;
		return this;
	}

	public Background SetFixedPosition(bool fixedPosition = true)
	{
		data.fixedPosition = fixedPosition;
		return this;
	}

	public Background SetForceTiling(bool forceTiling = true)
	{
		data.forceTiling = forceTiling;
		return this;
	}

	public Background SetFollowCamera(bool followCamera = true)
	{
		data.followCamera = followCamera;
		UpdateFollowCamera();
		return this;
	}

	public Background SetRoom(int roomIndex)
	{
		data.room = roomIndex;
		UpdateFollowCamera();
		return this;
	}

	public Background SetScale(float _scale)
	{
		data.scale = new Vector2(_scale, _scale);
		return this;
	}

	public Background SetTileType(TileType tt)
	{
		data.tileType = tt;
		return this;
	}

	public Background SetFollowBeat(FollowBeatType followType)
	{
		data.followBeatType = followType;
		return this;
	}

	public void SetAnimation(string rootString, int[] spriteIds)
	{
		List<string> list = new List<string>();
		foreach (int num in spriteIds)
		{
			list.Add(rootString + "/" + num);
		}
		SetAnimation(list.ToArray());
	}

	public void SetAnimation(string[] frameNames)
	{
		List<int> list = new List<int>();
		foreach (string text in frameNames)
		{
			int spriteIdByName = data.spriteCollectionData.GetSpriteIdByName(text);
			list.Add(spriteIdByName);
		}
		mFrameIds = list.ToArray();
	}

	public void RewindAnimation()
	{
		mElapsedTime = 0f;
		UpdateAnimation();
	}

	public Background PlayAnimation()
	{
		mAnimated = true;
		return this;
	}

	public Background PauseAnimation()
	{
		mAnimated = false;
		return this;
	}

	public Background OnAnimationEnd(BackgroundCallback callback)
	{
		onAnimationEnd = callback;
		return this;
	}

	public void SetSprite(string sprite)
	{
		PauseAnimation();
		tiledSprite.SetSprite(data.spriteCollectionData, sprite);
		SetAnimation(new string[1] { sprite });
	}

	public override void OnBeat()
	{
		base.OnBeat();
		if (data.followBeatType == FollowBeatType.HeadbobStyle)
		{
			SetFPS(1f / (base.conductor.crotchet / 4f));
			RewindAnimation();
		}
		if (data.followBeatType == FollowBeatType.OneFramePerBeat)
		{
			currentFrame = (currentFrame + 1) % mFrameIds.Length;
		}
	}

	public Background SetSpriteCollectionData(tk2dSpriteCollectionData scdata)
	{
		data.spriteCollectionData = scdata;
		return this;
	}

	public static Background CreateSingle(string name, int room = 0, bool isForegound = false)
	{
		return Create(new string[1] { name }, 0f, 0f, 0f, isForegound, room).SetTileType(TileType.None);
	}

	public static Background CreateSingle(string rootString, int[] frameIds, float fps = 0f, int room = 0, bool isForeground = false)
	{
		return Create(rootString, frameIds, fps, 0f, 0f, isForeground, room).SetTileType(TileType.None);
	}

	public static Background Create(string name, float scrollx = 0f, float scrolly = 0f, bool isForeground = false, int room = 0)
	{
		return Create(new string[1] { name }, 4f, scrollx, scrolly, isForeground, room);
	}

	public static Background Create(string rootString, int[] frameIds, float fps, float scrollx = 0f, float scrolly = 0f, bool isForeground = false, int room = 0)
	{
		string[] array = new string[frameIds.Length];
		for (int i = 0; i < frameIds.Length; i++)
		{
			array[i] = $"{rootString}/{frameIds[i]}";
		}
		return Create(array, fps, scrollx, scrolly, isForeground, room);
	}

	public static Background Create(string[] bgs, float fps = 0f, float scrollx = 0f, float scrolly = 0f, bool isForeground = false, int room = 0)
	{
		BackgroundData backgroundData = new BackgroundData
		{
			spriteCollectionData = RDBase.Vfx.scBackground,
			spriteName = bgs[0],
			isForeground = isForeground,
			followCamera = true,
			scale = Vector3.one
		};
		if (room == -1)
		{
			room = 0;
		}
		backgroundData.room = room;
		backgroundData.velocity = new Vector2(scrollx, scrolly);
		if (RDBase.Vfx.scaleBackgroundsAndRows)
		{
			float num = RDBase.Vfx.RDHeight / RDBase.Vfx.hudCamera.RDFullNativeResolution().y;
			backgroundData.scale = new Vector3(num, num, 1f);
		}
		Background background = RDUtils.Create<Background>("Background", makeContainer: true, room, "Background Container Camera");
		background.gameObject.layer = 9;
		background.gameObject.name = backgroundData.spriteName;
		background.SetBackgroundOrder(++RDBase.Vfx.lastBackgroundOrder);
		background.SetData(backgroundData);
		RDBase.Vfx.backgrounds.Add(background);
		background.SetAnimation(bgs);
		background.fps = fps;
		background.PlayAnimation();
		return background;
	}
}
