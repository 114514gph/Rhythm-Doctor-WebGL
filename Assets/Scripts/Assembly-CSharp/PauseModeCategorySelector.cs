using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PauseModeCategorySelector : RDBase
{
	public RectTransform selector;

	public HorizontalLayoutGroup layout;

	[NonSerialized]
	public RectTransform rectTransform;

	[NonSerialized]
	public RectTransform layoutRect;

	private PauseMenuMode mode;

	private float contentWidth;

	private float selectorSpaceWidth;

	public void Initialize(float width, float height, PauseMenuMode mode)
	{
		rectTransform = GetComponent<RectTransform>();
		layoutRect = layout.GetComponent<RectTransform>();
		this.mode = mode;
		contentWidth = width;
		selectorSpaceWidth = width / (float)mode.categories.Count;
		selector.sizeDelta = new Vector2(selectorSpaceWidth, height);
		rectTransform.sizeDelta = new Vector2(width, height);
	}

	public void UpdateSelector()
	{
		float endValue = selectorSpaceWidth * (float)mode.currentCategoryIndex;
		selector.DOKill();
		selector.DOAnchorPosX(endValue, 0.2f).SetEase(Ease.OutExpo).SetUpdate(isIndependentUpdate: true);
	}
}
