using System;
using DG.Tweening;
using UnityEngine;

public class BarTransition : MonoBehaviour
{
	public RectTransform[] bars;

	public Ease ease;

	public float duration;

	public float delayPerBar;

	public void Show(Action OnComplete = null)
	{
		DoTransition(0f, 352f, OnComplete);
	}

	public void Hide(Action OnComplete = null)
	{
		DoTransition(352f, 0f, OnComplete);
	}

	private void DoTransition(float origin, float target, Action OnComplete = null)
	{
		float num = 0f;
		Vector2 endValue = new Vector2(target, 1f);
		RectTransform[] array = bars;
		foreach (RectTransform rectTransform in array)
		{
			rectTransform.sizeDelta = rectTransform.sizeDelta.WithX(origin);
			rectTransform.DOKill();
			if (rectTransform == bars[0])
			{
				rectTransform.DOSizeDelta(endValue, duration).SetEase(ease).SetUpdate(isIndependentUpdate: true);
			}
			else if (rectTransform == bars[^1] && OnComplete != null)
			{
				rectTransform.DOSizeDelta(endValue, duration).SetEase(ease).SetUpdate(isIndependentUpdate: true)
					.SetDelay(num)
					.OnComplete(() =>
					{
						OnComplete();
					});
			}
			else
			{
				rectTransform.DOSizeDelta(endValue, duration).SetEase(ease).SetUpdate(isIndependentUpdate: true)
					.SetDelay(num);
			}
			num += delayPerBar;
		}
	}
}
