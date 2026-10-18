using System;
using DG.Tweening;
using UnityEngine;

public class BpmMovementAnimation : CLSAnimation
{
	[Serializable]
	public class MovementAnimation
	{
		public Ease ease;

		public Vector2 position;

		[Range(0f, 1f)]
		public float durationPercentage = 1f;

		public bool snapping;
	}

	public int repetitionsPerBeat = 1;

	public bool playOnAwake;

	public MovementAnimation defaultPositionMovement;

	public MovementAnimation[] movements;

	private RectTransform rectTransform;

	private Sequence movementSequence;

	private float lastBeatDuration;

	private void OnDestroy()
	{
		movementSequence.Kill();
	}

	private void Start()
	{
		rectTransform = GetComponent<RectTransform>();
		movementSequence = DOTween.Sequence();
		if (playOnAwake)
		{
			Play();
		}
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
		if (movementSequence != null)
		{
			movementSequence.Kill();
		}
		movementSequence = DOTween.Sequence();
		for (int i = 0; i < repetitionsPerBeat; i++)
		{
			MovementAnimation[] array = movements;
			foreach (MovementAnimation movementAnimation in array)
			{
				float duration = timeToNextBeat * movementAnimation.durationPercentage / (float)repetitionsPerBeat;
				movementSequence.Append(rectTransform.DOAnchorPos(movementAnimation.position, duration, movementAnimation.snapping).SetEase(movementAnimation.ease).SetUpdate(isIndependentUpdate: true));
			}
		}
		movementSequence.Play();
	}

	public override void Stop()
	{
		BpmAnimationManager instance = BpmAnimationManager.instance;
		instance.onBeat = (BpmAnimationManager.OnBeat)Delegate.Remove(instance.onBeat, new BpmAnimationManager.OnBeat(StartAnimation));
		movementSequence.Kill();
		rectTransform.DOAnchorPos(defaultPositionMovement.position, lastBeatDuration * defaultPositionMovement.durationPercentage).SetEase(defaultPositionMovement.ease);
	}
}
