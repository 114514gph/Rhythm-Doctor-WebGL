using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BpmColorAnimation : CLSAnimation
{
	[Serializable]
	public class ColorAnimation
	{
		public Ease ease;

		public Color color;

		[Range(0f, 1f)]
		public float durationPercentage;
	}

	public ColorAnimation defaultColor;

	public ColorAnimation[] colors;

	private Image image;

	private Sequence movementSequence;

	private float lastBeatDuration;

	private void Start()
	{
		image = GetComponent<Image>();
	}

	public override void Play()
	{
		BpmAnimationManager instance = BpmAnimationManager.instance;
		instance.onBeat = (BpmAnimationManager.OnBeat)Delegate.Remove(instance.onBeat, new BpmAnimationManager.OnBeat(StartAnimation));
		BpmAnimationManager instance2 = BpmAnimationManager.instance;
		instance2.onBeat = (BpmAnimationManager.OnBeat)Delegate.Combine(instance2.onBeat, new BpmAnimationManager.OnBeat(StartAnimation));
	}

	private void StartAnimation(float timeToNextBeat)
	{
		lastBeatDuration = timeToNextBeat;
		movementSequence.Kill();
		movementSequence = DOTween.Sequence();
		ColorAnimation[] array = colors;
		foreach (ColorAnimation colorAnimation in array)
		{
			movementSequence.Append(image.DOColor(colorAnimation.color, timeToNextBeat * colorAnimation.durationPercentage).SetEase(colorAnimation.ease));
		}
		movementSequence.Play();
	}

	public override void Stop()
	{
		BpmAnimationManager instance = BpmAnimationManager.instance;
		instance.onBeat = (BpmAnimationManager.OnBeat)Delegate.Remove(instance.onBeat, new BpmAnimationManager.OnBeat(StartAnimation));
		movementSequence.Kill();
		image.DOColor(defaultColor.color, lastBeatDuration * defaultColor.durationPercentage).SetEase(defaultColor.ease);
	}
}
