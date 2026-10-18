using System;
using UnityEngine;
using UnityEngine.UI;

public class BpmSpriteAnimation : CLSAnimation
{
	private bool _playing;

	public SpriteAnimation.SpriteAnimationData currentAnimData;

	private Image target;

	private float singleSpriteDuration;

	private float currentTime;

	private int currentLoopNumber;

	private int currentSpriteIndex;

	private int currentBeatCounter;

	public bool Playing
	{
		get
		{
			return _playing;
		}
		set
		{
			_playing = value;
			BpmAnimationManager instance = BpmAnimationManager.instance;
			instance.onBeat = (BpmAnimationManager.OnBeat)Delegate.Remove(instance.onBeat, new BpmAnimationManager.OnBeat(StartAnimation));
			if (_playing)
			{
				BpmAnimationManager instance2 = BpmAnimationManager.instance;
				instance2.onBeat = (BpmAnimationManager.OnBeat)Delegate.Combine(instance2.onBeat, new BpmAnimationManager.OnBeat(StartAnimation));
			}
		}
	}

	public Action OnStopAction { get; set; }

	private void Start()
	{
		target = GetComponent<Image>();
		singleSpriteDuration = -1f;
	}

	private void Update()
	{
		if (Playing && !(singleSpriteDuration <= 0f))
		{
			currentTime += BpmAnimationManager.deltaDspTime;
			if (currentTime > singleSpriteDuration)
			{
				ChangeSprite();
			}
		}
	}

	public override void Play()
	{
		currentSpriteIndex = 0;
		currentLoopNumber = 0;
		currentTime = 0f;
		currentBeatCounter = 0;
		Playing = true;
	}

	private void StartAnimation(float timeToNextBeat)
	{
		currentBeatCounter++;
		int num = ((currentAnimData.speedMultiplier < 1f) ? 1 : Mathf.RoundToInt(currentAnimData.speedMultiplier));
		if (currentBeatCounter >= num)
		{
			currentBeatCounter = 0;
			singleSpriteDuration = timeToNextBeat * currentAnimData.speedMultiplier / (float)currentAnimData.sprites.Length;
			currentSpriteIndex = 0;
			currentTime = 0f;
			ChangeSprite();
		}
	}

	private void ChangeSprite()
	{
		if (currentSpriteIndex == currentAnimData.sprites.Length)
		{
			currentSpriteIndex = 0;
			currentLoopNumber++;
		}
		target.sprite = currentAnimData.sprites[currentSpriteIndex];
		currentSpriteIndex++;
		if (currentAnimData.loops > 0 && currentLoopNumber >= currentAnimData.loops)
		{
			Stop();
		}
		if (currentTime > singleSpriteDuration)
		{
			currentTime -= singleSpriteDuration;
		}
	}

	public override void Stop()
	{
		Playing = false;
		if (currentAnimData.defaultSpriteIndex < currentAnimData.sprites.Length)
		{
			target.sprite = currentAnimData.sprites[currentAnimData.defaultSpriteIndex];
		}
		else if (OnStopAction != null)
		{
			OnStopAction();
			OnStopAction = null;
		}
	}

	public void SetSprite(Sprite spriteToShow)
	{
		target.sprite = spriteToShow;
	}
}
