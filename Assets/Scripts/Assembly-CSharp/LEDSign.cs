using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LEDSign : RDBase
{
	private const float StatusTextTime = 4f;

	public static bool showMarginError;

	public Text message;

	public RectTransform messageBackground;

	public Ease showEase;

	public Ease hideEase;

	public float animDuration;

	public float hiddenYPos;

	public float showYPos;

	[NonSerialized]
	public bool hideTextAfterSomeTime;

	private RectTransform rect;

	private RectTransform messageRect;

	private bool useUnscaledDeltaTime;

	private float remainingStatusTextTime;

	private int scaleFactor;

	private bool needsWidthUpdate;

	public static string status
	{
		set
		{
			if (!DebugSettings.instance.RecordingMode)
			{
				scnGame.instance.statusText.SetStatusText(value);
			}
		}
	}

	public static Color statusColor
	{
		set
		{
			scnGame.instance.statusText.message.color = value;
		}
	}

	private void Awake()
	{
		rect = GetComponent<RectTransform>();
		messageRect = message.GetComponent<RectTransform>();
	}

	private void Update()
	{
		if (needsWidthUpdate)
		{
			UpdateWidth(message.cachedTextGenerator.verts);
		}
		if (hideTextAfterSomeTime && remainingStatusTextTime > 0f)
		{
			remainingStatusTextTime -= (useUnscaledDeltaTime ? Time.unscaledDeltaTime : Time.deltaTime);
			if (remainingStatusTextTime <= 0f)
			{
				Hide();
			}
		}
	}

	private void UpdateWidth(IList<UIVertex> verts)
	{
		if (verts != null && verts.Count == 0)
		{
			needsWidthUpdate = true;
			return;
		}
		needsWidthUpdate = false;
		scaleFactor = PopulateText(message);
		float num = verts[0].position.x;
		float num2 = verts[0].position.x;
		foreach (UIVertex vert in verts)
		{
			if (num > vert.position.x)
			{
				num = vert.position.x;
			}
			if (num2 < vert.position.x)
			{
				num2 = vert.position.x;
			}
		}
		float num3 = (Mathf.Abs(num) + Mathf.Abs(num2) - 3f * (float)scaleFactor) / (float)scaleFactor;
		float num4 = -1f;
		if (base.game != null)
		{
			num4 = base.game.currentLevel.statusSignWidth;
		}
		if (num4 != -1f)
		{
			num3 = num4;
		}
		messageBackground.sizeDelta = new Vector2(num3, messageBackground.sizeDelta.y);
		rect.DOKill();
		rect.DOAnchorPosY(showYPos, animDuration).SetEase(showEase).SetUpdate(isIndependentUpdate: true);
		rect.sizeDelta = rect.sizeDelta.WithX(90f + num3);
	}

	private void SetText(string value)
	{
		message.text = value;
		message.font = RDString.GetAppropiateFontForString(value);
		scaleFactor = PopulateText(message);
		float num = ((scaleFactor == 2) ? (-3) : (-4));
		messageRect.anchoredPosition = new Vector2(messageRect.anchoredPosition.x, num);
		UpdateWidth(message.cachedTextGenerator.verts);
	}

	private void Narrate(string str)
	{
		Narration.Say(str.RemoveRichTextColorTags(), NarrationCategory.Notification, false);
	}

	private int PopulateText(Text text)
	{
		TextGenerationSettings generationSettings = text.GetGenerationSettings(text.rectTransform.sizeDelta);
		text.cachedTextGenerator.Populate(text.text, generationSettings);
		return Mathf.RoundToInt(generationSettings.scaleFactor);
	}

	public void Show(string textToken, bool narrate = false)
	{
		string text = RDString.Get(textToken);
		SetText(text);
		if (narrate)
		{
			Narrate(text);
		}
	}

	public void Hide()
	{
		rect.DOKill();
		rect.DOAnchorPosY(hiddenYPos, animDuration).SetEase(hideEase).SetUpdate(isIndependentUpdate: true);
	}

	public void SetStatusText(string text, Color? color = null, float duration = 4f, bool narrate = false, bool useUnscaledTime = false)
	{
		if (string.IsNullOrEmpty(text))
		{
			Hide();
			return;
		}
		SetText(text);
		useUnscaledDeltaTime = useUnscaledTime;
		remainingStatusTextTime = duration;
		hideTextAfterSomeTime = true;
		if (color.HasValue)
		{
			message.color = color.Value;
		}
		if (narrate)
		{
			Narrate(text);
		}
	}
}
