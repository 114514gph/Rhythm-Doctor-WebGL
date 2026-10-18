using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SlotUI : RDBase
{
	private const float buttonAwayPosX = -100f;

	private const float buttonVisiblePosX = 17.5f;

	private const int handNotPressed = 0;

	private const int handPressed = 1;

	private const int buttonUnpressed = 0;

	private const int buttonPressed = 1;

	private const int buttonPressedSplash = 2;

	[Header("Scene")]
	public RectTransform rectTransform;

	public Image border;

	public Image outline;

	public Image background;

	public Image contents;

	public Image hand;

	public Image nails;

	public Image palm;

	public RawImage arm;

	public Image button;

	public Text title;

	public Image leftArrow;

	public Image rightArrow;

	public Text stars;

	public Text scrolls;

	public Text percent;

	public Image flashBorder;

	public Image flashBackground;

	[Header("Assets")]
	public Material notes;

	public Sprite[] outlineFrames;

	public Sprite[] buttonFramesNormal;

	public Sprite[] buttonFramesGold;

	public Sprite[] handFrames;

	public Sprite[] nailFrames;

	public Sprite[] palmFrames;

	[Header("Variables")]
	public int slot;

	public float mult1;

	public float mult2;

	public float mult3;

	public Color goldColor;

	public float slotClosedHandX = 29f;

	public float slotOpenHandX = 50f;

	public float slotClosedWidth = 103f;

	public float slotOpenWidth = 250f;

	public Vector2 notesSpeed;

	[NonSerialized]
	public float originalPosX;

	[NonSerialized]
	public Tweener moveTween;

	private bool focused;

	private bool isGold;

	private Tweener sizeDeltaTween;

	private Sprite[] buttonFrames
	{
		get
		{
			if (!isGold)
			{
				return buttonFramesNormal;
			}
			return buttonFramesGold;
		}
	}

	private void Awake()
	{
		contents.material = UnityEngine.Object.Instantiate(notes);
		arm.material = UnityEngine.Object.Instantiate(arm.material);
		originalPosX = rectTransform.anchoredPosition.x;
		button.rectTransform.anchoredPosition = button.rectTransform.anchoredPosition.WithX(-100f);
		LoadSlotData();
	}

	public void LoadSlotData()
	{
		ArmSkin armSkin = Persistence.skins[slot, 0];
		SetSkin(armSkin, arm, hand, nails, palm);
		Persistence.currentSlotIndex = slot;
		GameCompletion gameCompletion = Persistence.GetGameCompletion();
		stars.text = gameCompletion.passedLevels.ToString();
		scrolls.text = gameCompletion.perfectLevels.ToString();
		int num = Mathf.RoundToInt(gameCompletion.percentCompletion);
		percent.text = num + "%";
		isGold = num >= 100;
		Color color = (isGold ? goldColor : Color.white);
		percent.color = color;
		scrolls.color = color;
		stars.color = color;
		title.color = color;
		leftArrow.color = color;
		rightArrow.color = color;
		outline.color = color;
	}

	public void Localize()
	{
		title.font = RDString.currentLanguageFont;
		title.text = RDString.Get("slotMenu.slotSelect.title").Replace("[number]", (slot + 1).ToString());
	}

	public void SetSkin(ArmSkin armSkin, RawImage drawing, Image hand, Image nails, Image palm)
	{
		Texture2D texture2D = armSkin.drawing;
		if (texture2D != null)
		{
			texture2D.filterMode = FilterMode.Point;
		}
		else
		{
			texture2D = base.gc.emptyTexture;
		}
		drawing.material.SetTexture("_Drawing", texture2D);
		drawing.material.SetColor("_TintColor", armSkin.sleeveColor);
		drawing.material = UnityEngine.Object.Instantiate(drawing.material);
		hand.color = armSkin.skinColor;
		nails.color = armSkin.nailsColor;
		palm.color = armSkin.skinColor.WithLightnessBoost(armSkin.palmLightness);
	}

	private void Update()
	{
		Color sleeveColor = Persistence.skins[slot, 0].sleeveColor;
		float a = 0.7f;
		background.color = new Color(sleeveColor.r * mult1, sleeveColor.g * mult1, sleeveColor.b * mult1, a);
		contents.color = new Color(sleeveColor.r * mult2, sleeveColor.g * mult2, sleeveColor.b * mult2, a);
		border.color = new Color(sleeveColor.r * mult3, sleeveColor.g * mult3, sleeveColor.b * mult3, a);
		if (focused)
		{
			Vector2 textureOffset = contents.materialForRendering.GetTextureOffset("_MaskTex");
			Vector2 value = new Vector2((textureOffset.x + Time.deltaTime * notesSpeed.x) % 1f, (textureOffset.y + Time.deltaTime * notesSpeed.y) % 1f);
			contents.materialForRendering.SetTextureOffset("_MaskTex", value);
		}
	}

	public void SetFocused(bool on)
	{
		outline.enabled = on;
		focused = on;
		leftArrow.enabled = on;
		rightArrow.enabled = on;
	}

	public void SetOpen(bool on)
	{
		float duration = 0.4f;
		float num = (on ? slotOpenWidth : slotClosedWidth);
		if (sizeDeltaTween != null)
		{
			sizeDeltaTween.Kill();
		}
		sizeDeltaTween = rectTransform.DOSizeDelta(rectTransform.sizeDelta.WithX(num), duration).SetEase(Ease.OutBack);
		float endValue = (on ? slotOpenHandX : slotClosedHandX);
		hand.DOKill();
		hand.rectTransform.DOAnchorPosX(endValue, duration).SetEase(Ease.OutBack);
		hand.sprite = handFrames[(!on) ? 1u : 0u];
		nails.sprite = nailFrames[(!on) ? 1u : 0u];
		palm.sprite = palmFrames[(!on) ? 1u : 0u];
		float endValue2 = (on ? 17.5f : (-100f));
		button.sprite = buttonFrames[0];
		button.DOKill();
		button.rectTransform.DOAnchorPosX(endValue2, duration).SetEase(Ease.OutQuad);
		outline.color = outline.color.WithAlpha(1f);
		leftArrow.enabled = !on;
		rightArrow.enabled = !on;
	}

	public void Flash()
	{
		FlashImage(flashBorder);
		FlashImage(flashBackground);
	}

	private void FlashImage(Image image)
	{
		Color white = Color.white;
		Color endValue = Color.white.WithAlpha(0f);
		image.enabled = true;
		image.color = white;
		image.DOKill();
		image.DOColor(endValue, 0.4f).SetEase(Ease.InQuad).OnComplete(() =>
		{
			image.enabled = false;
		});
	}

	public void PressButton()
	{
		button.sprite = buttonFrames[1];
		hand.sprite = handFrames[1];
		nails.sprite = nailFrames[1];
		palm.sprite = palmFrames[1];
		Flash();
	}

	public void OnBeat(bool on)
	{
		outline.sprite = outlineFrames[on ? 1 : 0];
		float num = 22f;
		if (on)
		{
			num++;
		}
		leftArrow.rectTransform.AnchorPosX(0f - num);
		rightArrow.rectTransform.AnchorPosX(num);
	}
}
