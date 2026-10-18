using System;
using UnityEngine;
using UnityEngine.UI;

public class SpriteAnimation : CLSAnimation
{
	[Serializable]
	public class SpriteAnimationData
	{
		public const int MinAnimSpeedMultiplier = 1;

		public string name;

		public Sprite[] sprites;

		public int loops;

		public int defaultSpriteIndex;

		public float speedMultiplier = 1f;
	}

	public SpriteAnimationData currentAnimationData;

	public float singleSpriteDuration;

	public bool stopOnLoopEnding;

	public bool deactivateOnStop;

	public bool playing;

	private Image image;

	private SpriteRenderer spriteRenderer;

	[NonSerialized]
	public float currentAnimationTime;

	private int currentSpriteIndex;

	private int currentLoopNumber;

	private void Awake()
	{
		SetTargetComponent();
	}

	private void SetTargetComponent()
	{
		image = GetComponent<Image>();
		if (image == null)
		{
			spriteRenderer = GetComponent<SpriteRenderer>();
		}
	}

	private void Update()
	{
		if (!playing)
		{
			return;
		}
		currentAnimationTime += RDTime.unscaledDeltaTime;
		if (currentAnimationTime < singleSpriteDuration)
		{
			return;
		}
		currentSpriteIndex++;
		currentAnimationTime = 0f;
		if (currentSpriteIndex >= currentAnimationData.sprites.Length)
		{
			currentSpriteIndex = 0;
			currentLoopNumber++;
			if (stopOnLoopEnding)
			{
				Stop();
			}
		}
		SetSprite(currentAnimationData.sprites[currentSpriteIndex]);
		if (currentAnimationData.loops > 0 && currentLoopNumber == currentAnimationData.loops)
		{
			playing = false;
			Stop();
		}
	}

	public override void Play()
	{
		PlayFromFrame(0);
	}

	public void PlayFromFrame(int frameIndex)
	{
		SetTargetComponent();
		currentLoopNumber = 0;
		currentAnimationTime = 0f;
		currentSpriteIndex = frameIndex;
		if (currentAnimationData.sprites.Length != 0 && frameIndex >= 0 && frameIndex < currentAnimationData.sprites.Length)
		{
			SetSprite(currentAnimationData.sprites[currentSpriteIndex]);
		}
		else if (currentAnimationData.sprites.Length == 0)
		{
			Debug.LogError("trying to play animation with no sprites / non-existent animation");
		}
		else
		{
			Debug.LogError("trying to play animation from an out-of-bounds index");
		}
		playing = true;
	}

	public void Rewind()
	{
		currentAnimationTime = 0f;
		SetSprite(currentAnimationData.sprites[0]);
		playing = false;
	}

	public override void Stop()
	{
		playing = false;
		int num = Mathf.Clamp(currentAnimationData.defaultSpriteIndex, 0, currentAnimationData.sprites.Length);
		if (currentAnimationData.sprites.Length > num)
		{
			SetSprite(currentAnimationData.sprites[num]);
		}
		if (deactivateOnStop)
		{
			base.gameObject.SetActive(value: false);
		}
	}

	public void SetSprite(Sprite sprite)
	{
		if (image != null)
		{
			image.sprite = sprite;
		}
		else if (spriteRenderer != null)
		{
			spriteRenderer.sprite = sprite;
		}
	}
}
