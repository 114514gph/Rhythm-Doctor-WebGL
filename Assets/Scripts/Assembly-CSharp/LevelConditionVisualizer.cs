using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LevelConditionVisualizer : MonoBehaviour
{
	private const float VisibilityChangeAnimDuration = 0.1f;

	private const float VerticalBorderSize = 10f;

	public LevelErrorContainer levelErrorContainer;

	public Text conditionsText;

	private RectTransform rectTransform;

	private CanvasGroup canvasGroup;

	[NonSerialized]
	public bool visible;

	private Transform originalParent;

	private bool animating;

	private void Awake()
	{
		canvasGroup = GetComponent<CanvasGroup>();
		rectTransform = GetComponent<RectTransform>();
		originalParent = rectTransform.parent;
	}

	private void LateUpdate()
	{
		if (visible && !animating && (Input.GetKeyUp(KeyCode.Mouse0) || Input.GetKeyUp(KeyCode.Mouse1) || Input.GetKeyUp(KeyCode.Mouse2) || Input.mouseScrollDelta.y != 0f))
		{
			Hide();
		}
	}

	public void Show(RectTransform settingRect, List<LevelSettingName> settings)
	{
		if (animating)
		{
			return;
		}
		animating = true;
		if (visible)
		{
			Hide();
			return;
		}
		rectTransform.SetParent(settingRect);
		rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, 2f);
		rectTransform.SetParent(originalParent);
		scrConductor.PlayImmediately("sndEditorPopupSmall", 1f, RDUtils.GetMixerGroup("LevelEditorInspectorPanel"));
		bool flag = true;
		conditionsText.text = string.Empty;
		SettingErrorLocalization settingErrorLocalization = null;
		foreach (LevelSettingName setting in settings)
		{
			if (flag)
			{
				flag = false;
			}
			else
			{
				conditionsText.text += "\n\n";
			}
			SettingErrorLocalization[] showableErrors = levelErrorContainer.showableErrors;
			foreach (SettingErrorLocalization settingErrorLocalization2 in showableErrors)
			{
				if (settingErrorLocalization2.settingName == setting)
				{
					settingErrorLocalization = settingErrorLocalization2;
					break;
				}
			}
			if (settingErrorLocalization == null)
			{
				continue;
			}
			Text text = conditionsText;
			text.text = text.text + RDString.Get(settingErrorLocalization.settingNameToken) + ":";
			foreach (SettingCondition error in settingErrorLocalization.errors)
			{
				if (error.recommended)
				{
					Text text2 = conditionsText;
					text2.text = text2.text + "\n- " + error.GetLocalized();
				}
			}
		}
		int lineHeight = conditionsText.font.lineHeight;
		float num = conditionsText.preferredHeight + (float)lineHeight * conditionsText.lineSpacing;
		rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, num + 10f);
		ToggleVisibility(show: true);
	}

	private void Hide(Action onCompleteAction = null)
	{
		animating = true;
		ToggleVisibility(show: false, onCompleteAction);
		scrConductor.PlayImmediately("sndEditorPopdownSmall", 1f, RDUtils.GetMixerGroup("LevelEditorInspectorPanel"));
	}

	private void ToggleVisibility(bool show, Action onCompleteAction = null)
	{
		float endValue;
		Vector3 endValue2;
		Ease ease;
		if (show)
		{
			canvasGroup.alpha = 0f;
			rectTransform.localScale = Vector3.zero;
			endValue = 1f;
			endValue2 = Vector3.one;
			ease = Ease.OutExpo;
		}
		else
		{
			endValue = 0f;
			endValue2 = Vector3.zero;
			ease = Ease.InExpo;
		}
		rectTransform.DOKill();
		rectTransform.DOScale(endValue2, 0.1f).SetEase(ease).SetUpdate(isIndependentUpdate: true);
		canvasGroup.DOKill();
		canvasGroup.DOFade(endValue, 0.1f).SetEase(ease).SetUpdate(isIndependentUpdate: true)
			.OnComplete(() =>
			{
				visible = show;
				animating = false;
				if (onCompleteAction != null)
				{
					onCompleteAction();
				}
			});
	}
}
