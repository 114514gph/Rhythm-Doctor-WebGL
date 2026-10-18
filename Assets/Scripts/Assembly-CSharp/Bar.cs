using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Bar : MonoBehaviour
{
	public int index;

	public TempresManager manager;

	[NonSerialized]
	public float tapTime;

	[NonSerialized]
	public RectTransform rectTransform;

	[NonSerialized]
	public Image image;

	[NonSerialized]
	public Vector3 completedPosition;

	[NonSerialized]
	public float anchorY = 0.5f;

	[NonSerialized]
	private Tween anchorYTween;

	public int barCount => manager.bars.Length;

	public float barSpacing => manager.barSpacing;

	public Vector2 startSize => manager.startSize;

	public float startScaleTime => manager.startScaleTime;

	public AnimationCurve startScaleEase => manager.startScaleEase;

	public Color defaultColor => manager.defaultColor;

	public AnimationCurve changeColorEase => manager.changeColorEase;

	private void Awake()
	{
		image = GetComponent<Image>();
		rectTransform = GetComponent<RectTransform>();
	}

	public void CancelAnimations()
	{
		rectTransform.DOKill();
		image.DOKill();
		anchorYTween?.Kill();
	}

	public void MoveUsingAnchoredPosition(Vector2 anchoredPosition, float duration, AnimationCurve ease)
	{
		rectTransform.DOAnchorPos(anchoredPosition, duration).SetUpdate(isIndependentUpdate: true).SetEase(ease);
	}

	public void PreInitialize()
	{
		rectTransform.sizeDelta = Vector2.one;
		ScaleInstant(new Vector2(startSize.x, 0f));
		SetColor(defaultColor, 0f, changeColorEase);
		SetSpacedPosition(0f, barSpacing / 4f, index, barCount);
	}

	public void Initialize()
	{
		SetSpacedPosition(1.25f, barSpacing, index, barCount, Ease.OutCubic);
		Scale(startSize, startScaleTime, startScaleEase, 0.1f * (float)index);
	}

	public void Scale(Vector2 size, float duration, AnimationCurve ease, float delay = 0f)
	{
		rectTransform.DOScale(size, duration).SetDelay(delay).SetUpdate(isIndependentUpdate: true)
			.SetEase(ease)
			.Done();
	}

	public void ScaleInstant(Vector2 size)
	{
		Scale(size, 0f, AnimationCurve.Linear(0f, 0f, 1f, 1f));
	}

	public void SetColor(Color color, float duration, AnimationCurve ease)
	{
		image.DOColor(color, duration).SetUpdate(isIndependentUpdate: true).SetEase(ease);
	}

	public void SetSpacedPosition(float duration, float spacing, int index, int count, Ease ease = Ease.Linear)
	{
		float endValue = ((float)index - (float)(count - 1) / 2f) * spacing;
		rectTransform.DOAnchorPosX(endValue, duration).SetEase(ease).Done();
	}

	public void AnimateAnchorY(bool bottom, float duration, AnimationCurve ease)
	{
		anchorYTween?.Kill();
		float endValue = (bottom ? 0f : 0.5f);
		anchorYTween = DOTween.To(() => anchorY, (float y) =>
		{
			anchorY = y;
		}, endValue, duration).SetUpdate(isIndependentUpdate: true).SetEase(ease);
	}

	private void LateUpdate()
	{
		float y = Mathf.Lerp(-25f, 0f, anchorY * 2f) + rectTransform.localScale.y * (0.5f - anchorY);
		rectTransform.AnchorPosY(y);
	}
}
