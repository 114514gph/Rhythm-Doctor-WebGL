using System;
using UnityEngine;
using UnityEngine.UI;

public class ApprovalLevelBadge : RDBase
{
	public Text text;

	public Image background;

	public RectTransform rectTransform;

	public Button button;

	[NonSerialized]
	public ApprovalLevel approvalLevel;

	public Sprite defaultSprite;

	public Sprite onlyColorSprite;

	public float defaultSize;

	public float onlyColorSize;

	private ArtistData currentArtistData;

	public void UpdateUI(ApprovalLevel approvalLevel, bool onlyColor = false)
	{
		this.approvalLevel = approvalLevel;
		if (approvalLevel == ApprovalLevel.Pending)
		{
			base.gameObject.SetActive(value: false);
			return;
		}
		base.gameObject.SetActive(value: true);
		switch (approvalLevel)
		{
		case ApprovalLevel.Allowed:
			background.color = base.gc.allowedColor;
			break;
		case ApprovalLevel.PartiallyAllowed:
			background.color = base.gc.mostlyAllowedColor;
			break;
		case ApprovalLevel.Declined:
			background.color = base.gc.declinedColor;
			break;
		case ApprovalLevel.PartiallyDeclined:
			background.color = base.gc.particallyDeclinedColor;
			break;
		}
		if (onlyColor)
		{
			text.text = string.Empty;
			rectTransform.sizeDelta = new Vector2(defaultSize, defaultSize);
			background.sprite = onlyColorSprite;
			rectTransform.sizeDelta = new Vector2(onlyColorSize, onlyColorSize);
		}
		else
		{
			background.sprite = defaultSprite;
			text.text = RDString.Get("enum.ApprovalLevel." + approvalLevel);
			text.font = RDString.GetAppropiateFontForString(text.text);
			rectTransform.sizeDelta = new Vector2(text.preferredWidth + 8f, defaultSize);
		}
	}

	public void ShowDisclaimer()
	{
		base.editor.levelSettingsPanel.currentArtistDisclaimerAction();
	}
}
