using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class StartRow : RDBase
{
	[Serializable]
	public struct Button
	{
		public RectTransform content;

		public Image background;

		public Image label;

		public SpriteAnimation handAnim;

		public Sprite bgDefault;

		public Sprite bgSelected;

		public Sprite bgPressed;
	}

	public struct Frame
	{
		public Vector2 pos;

		public Vector2 size;
	}

	public Button oneP;

	public Button twoP;

	[Tooltip("Fixed design scale for the labels while the play mode is selectable; 1 when locked.")]
	public float labelSelectableScale = 1f;

	[Tooltip("Horizontal gap between the expanded and the collapsed button.")]
	public float buttonGap = 2.3f;

	[Tooltip("Width of the collapsed button; the expanded one takes the rest of the row.")]
	public float collapsedWidth = 21.2f;

	public static readonly Vector2 FocusContentNudge = new Vector2(-1f, 1f);

	private const float StartResizeDuration = 0.25f;

	private const float CollapsedHandSpeedMultiplier = 2f;

	private const float LockedTwoPlayerHandX = 8f;

	private const float LabelEdgePad = 3f;

	private const float OnePHandArtRightInset = 3f;

	private RectTransform rowRect;

	private RectTransform oneBgRect;

	private RectTransform twoBgRect;

	private Vector2 oneContentInitPos;

	private Vector2 oneContentInitSize;

	private Vector2 twoContentInitPos;

	private Vector2 twoContentInitSize;

	private Vector2 oneBgInitPos;

	private Vector2 oneBgInitSize;

	private Vector2 twoBgInitPos;

	private Vector2 twoBgInitSize;

	private float handExpandedX;

	private float handWidth;

	private Vector2 selectorBasePos;

	private Vector2 selectorBaseSize;

	private float selectorLeftPad;

	private float selectorRightPad;

	private RectTransform oneHandRect;

	private RectTransform twoHandRect;

	private float oneHandAnimDuration;

	private float twoHandAnimDuration;

	private Vector2 oneAppliedNudge;

	private Vector2 twoAppliedNudge;

	private float RowWidth => rowRect.rect.width;

	private float ExpandedWidth => RowWidth - collapsedWidth - buttonGap;

	public void Init(Vector2 selectorBasePos, Vector2 selectorBaseSize)
	{
		this.selectorBasePos = selectorBasePos;
		this.selectorBaseSize = selectorBaseSize;
		rowRect = (RectTransform)oneP.content.parent;
		oneBgRect = oneP.background.rectTransform;
		twoBgRect = twoP.background.rectTransform;
		oneContentInitPos = oneP.content.anchoredPosition;
		oneContentInitSize = oneP.content.sizeDelta;
		twoContentInitPos = twoP.content.anchoredPosition;
		twoContentInitSize = twoP.content.sizeDelta;
		oneBgInitPos = oneBgRect.anchoredPosition;
		oneBgInitSize = oneBgRect.sizeDelta;
		twoBgInitPos = twoBgRect.anchoredPosition;
		twoBgInitSize = twoBgRect.sizeDelta;
		selectorLeftPad = 0f - selectorBasePos.x;
		selectorRightPad = selectorBaseSize.x - RowWidth - selectorLeftPad;
		oneHandRect = (RectTransform)oneP.handAnim.transform;
		oneHandAnimDuration = oneP.handAnim.singleSpriteDuration;
		twoHandRect = (RectTransform)twoP.handAnim.transform;
		twoHandAnimDuration = twoP.handAnim.singleSpriteDuration;
		handExpandedX = oneHandRect.anchoredPosition.x;
		handWidth = oneHandRect.sizeDelta.x;
	}

	public Frame SetState(bool singlePlayer, bool canChangePlayMode, bool focused, bool animate)
	{
		Vector2 vector = (focused ? FocusContentNudge : Vector2.zero);
		SnapNudge(oneP.content, ref oneAppliedNudge, singlePlayer ? vector : Vector2.zero);
		SnapNudge(twoP.content, ref twoAppliedNudge, singlePlayer ? Vector2.zero : vector);
		if (!canChangePlayMode)
		{
			SetButtonActive(isOneP: true, singlePlayer);
			SetButtonActive(isOneP: false, !singlePlayer);
			float num = (collapsedWidth + buttonGap) / 2f;
			ApplyRowInsets(singlePlayer ? oneP.content : twoP.content, singlePlayer ? oneContentInitPos : twoContentInitPos, singlePlayer ? oneContentInitSize : twoContentInitSize, num, num, singlePlayer ? oneAppliedNudge : twoAppliedNudge, animate);
			ApplyRowInsets(singlePlayer ? oneBgRect : twoBgRect, singlePlayer ? oneBgInitPos : twoBgInitPos, singlePlayer ? oneBgInitSize : twoBgInitSize, 0f, 0f, Vector2.zero, animate);
		}
		else
		{
			SetButtonActive(isOneP: true, active: true);
			SetButtonActive(isOneP: false, active: true);
			float num2 = collapsedWidth + buttonGap;
			float right = (singlePlayer ? num2 : (RowWidth - collapsedWidth));
			float left = (singlePlayer ? (RowWidth - collapsedWidth) : num2);
			ApplyRowInsets(oneP.content, oneContentInitPos, oneContentInitSize, 0f, right, oneAppliedNudge, animate);
			ApplyRowInsets(oneBgRect, oneBgInitPos, oneBgInitSize, 0f, right, Vector2.zero, animate);
			ApplyRowInsets(twoP.content, twoContentInitPos, twoContentInitSize, left, 0f, twoAppliedNudge, animate);
			ApplyRowInsets(twoBgRect, twoBgInitPos, twoBgInitSize, left, 0f, Vector2.zero, animate);
		}
		RefreshAppearance(singlePlayer, canChangePlayMode, focused);
		UpdateHands(singlePlayer, canChangePlayMode, animate);
		return ComputeFrame(singlePlayer, canChangePlayMode);
	}

	public Vector2 GetActiveContentPos(bool singlePlayer, bool canChangePlayMode)
	{
		float left;
		float right;
		if (!canChangePlayMode)
		{
			left = (right = (collapsedWidth + buttonGap) / 2f);
		}
		else if (singlePlayer)
		{
			left = 0f;
			right = collapsedWidth + buttonGap;
		}
		else
		{
			left = collapsedWidth + buttonGap;
			right = 0f;
		}
		RectTransform rt = (singlePlayer ? oneP.content : twoP.content);
		Vector2 vector = (singlePlayer ? oneContentInitPos : twoContentInitPos);
		return new Vector2(InsetsToPosX(rt, left, right), vector.y);
	}

	public Button ActiveButton(bool singlePlayer)
	{
		if (!singlePlayer)
		{
			return twoP;
		}
		return oneP;
	}

	private void UpdateHands(bool singlePlayer, bool canChangePlayMode, bool animate)
	{
		float targetX = (singlePlayer ? handExpandedX : 0f);
		MoveHand(oneHandRect, singlePlayer, targetX, animate);
		oneP.handAnim.singleSpriteDuration = (singlePlayer ? oneHandAnimDuration : (oneHandAnimDuration * 2f));
		bool flag = !singlePlayer;
		float targetX2 = ((!flag) ? 0f : (canChangePlayMode ? handExpandedX : 8f));
		MoveHand(twoHandRect, flag, targetX2, animate);
		twoP.handAnim.singleSpriteDuration = (singlePlayer ? (twoHandAnimDuration * 2f) : twoHandAnimDuration);
	}

	private void MoveHand(RectTransform hand, bool expanded, float targetX, bool animate)
	{
		hand.DOKill();
		SetHandAnchor(hand, expanded);
		if (animate)
		{
			hand.DOAnchorPosX(targetX, 0.25f).SetEase(Ease.OutExpo).SetUpdate(isIndependentUpdate: true);
		}
		else
		{
			hand.anchoredPosition = new Vector2(targetX, hand.anchoredPosition.y);
		}
	}

	private void SnapNudge(RectTransform content, ref Vector2 applied, Vector2 target)
	{
		if (!(applied == target))
		{
			content.anchoredPosition += target - applied;
			applied = target;
		}
	}

	private void SetHandAnchor(RectTransform hand, bool expanded)
	{
		float num = (expanded ? 0f : 0.5f);
		float num2 = hand.anchorMin.x;
		if (!Mathf.Approximately(num2, num))
		{
			float width = ((RectTransform)hand.parent).rect.width;
			hand.anchorMin = new Vector2(num, hand.anchorMin.y);
			hand.anchorMax = new Vector2(num, hand.anchorMax.y);
			hand.anchoredPosition += new Vector2((num2 - num) * width, 0f);
		}
	}

	private void ApplyRowInsets(RectTransform rt, Vector2 initPos, Vector2 initSize, float left, float right, Vector2 offset, bool animate)
	{
		float num = RowWidth - left - right;
		float num2 = (rt.anchorMax.x - rt.anchorMin.x) * RowWidth;
		Vector2 vector = new Vector2(InsetsToPosX(rt, left, right) + offset.x, initPos.y + offset.y);
		Vector2 vector2 = new Vector2(num - num2, initSize.y);
		rt.DOKill();
		if (animate)
		{
			rt.DOAnchorPos(vector, 0.25f).SetEase(Ease.OutExpo).SetUpdate(isIndependentUpdate: true);
			rt.DOSizeDelta(vector2, 0.25f).SetEase(Ease.OutExpo).SetUpdate(isIndependentUpdate: true);
		}
		else
		{
			rt.anchoredPosition = vector;
			rt.sizeDelta = vector2;
		}
	}

	private float InsetsToPosX(RectTransform rt, float left, float right)
	{
		float num = RowWidth - left - right;
		float num2 = (rt.anchorMax.x - rt.anchorMin.x) * RowWidth;
		float num3 = rt.anchorMin.x * RowWidth + rt.pivot.x * num2;
		return left + rt.pivot.x * num - num3;
	}

	private void SetButtonActive(bool isOneP, bool active)
	{
		if (isOneP)
		{
			oneP.content.gameObject.SetActive(active);
			oneBgRect.gameObject.SetActive(active);
		}
		else
		{
			twoP.content.gameObject.SetActive(active);
			twoBgRect.gameObject.SetActive(active);
		}
	}

	private void RefreshAppearance(bool singlePlayer, bool canChangePlayMode, bool focused)
	{
		Button button = (singlePlayer ? oneP : twoP);
		Button button2 = (singlePlayer ? twoP : oneP);
		button.background.sprite = (focused ? button.bgSelected : button.bgDefault);
		button2.background.sprite = button2.bgDefault;
		int num = Array.FindIndex(base.gc.startLocalizations, (SpriteLocalization localization) => localization.language == RDString.language);
		if (num == -1)
		{
			num = base.gc.startLocalizations.Length - 1;
		}
		oneP.label.sprite = base.gc.startLocalizations[num].sprite;
		oneP.label.SetNativeSize();
		twoP.label.sprite = base.gc.start2PLocalizations[num].sprite;
		twoP.label.SetNativeSize();
		oneP.label.gameObject.SetActive(singlePlayer);
		twoP.label.gameObject.SetActive(!singlePlayer);
		PlaceLabel(button, singlePlayer, canChangePlayMode);
		oneP.handAnim.gameObject.SetActive(value: true);
		twoP.handAnim.gameObject.SetActive(value: true);
	}

	private void PlaceLabel(Button button, bool singlePlayer, bool canChangePlayMode)
	{
		float num = ((!canChangePlayMode && !singlePlayer) ? 8f : handExpandedX);
		float num2 = (singlePlayer ? 3f : 0f);
		float num3 = num + handWidth / 2f - num2;
		float num4 = ExpandedWidth - 3f;
		float num5 = (canChangePlayMode ? labelSelectableScale : 1f);
		button.label.transform.localScale = new Vector3(num5, num5, 1f);
		RectTransform rectTransform = button.label.rectTransform;
		rectTransform.anchoredPosition = new Vector2((num3 + num4) / 2f, rectTransform.anchoredPosition.y);
	}

	private Frame ComputeFrame(bool singlePlayer, bool canChangePlayMode)
	{
		float num;
		float num2;
		if (!canChangePlayMode)
		{
			num = 0f;
			num2 = RowWidth;
		}
		else
		{
			num2 = ExpandedWidth;
			num = (singlePlayer ? 0f : (collapsedWidth + buttonGap));
		}
		Frame result = default;
		result.pos = new Vector2(num - selectorLeftPad, selectorBasePos.y);
		result.size = new Vector2(num2 + selectorLeftPad + selectorRightPad, selectorBaseSize.y);
		return result;
	}
}
