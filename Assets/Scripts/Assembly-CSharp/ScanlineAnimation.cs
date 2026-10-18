using DG.Tweening;
using UnityEngine;

public class ScanlineAnimation : MonoBehaviour
{
	[Header("Scanlines")]
	public RectTransform rectTransform;

	public float topY;

	public float bottomY;

	public float duration;

	public float minDelay;

	public float maxDelay;

	private float time;

	private void Awake()
	{
		time = Time.unscaledTime;
	}

	private void Update()
	{
		if (!(Time.unscaledTime <= time))
		{
			rectTransform.DOAnchorPosY(bottomY, duration, snapping: true).SetOptions(snapping: true).SetEase(Ease.Linear)
				.OnComplete(() =>
				{
					rectTransform.AnchorPosY(topY);
				})
				.SetUpdate(isIndependentUpdate: true);
			time = Time.unscaledTime + duration + Random.Range(minDelay, maxDelay);
		}
	}
}
