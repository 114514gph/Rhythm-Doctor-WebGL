using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class LevelEventControl_Action : LevelEventControl_Base
	{
		public Image icon;

		public Image iconSecondary;

		public Image iconTertiary;

		public RawImage rawImageBackground;

		public RawImage rawImageForeground;

		public Text label;

		public override void UpdateUIInternal()
		{
			base.UpdateUIInternal();
			RectTransform component = GetComponent<RectTransform>();
			Vector2 anchoredPosition = component.anchoredPosition;
			anchoredPosition.x = base.timeline.GetPosXFromBarAndBeat(new BarAndBeat(base.bar, base.beat));
			anchoredPosition.y = base.timeline.GetPosYFromRowIndex(levelEvent.y);
			component.anchoredPosition = anchoredPosition;
			component.sizeDelta = new Vector2(Mathf.Min(14, base.cellWidth), base.cellHeight);
			if (rawImageBackground != null)
			{
				rawImageBackground.gameObject.SetActive(value: false);
			}
			if (rawImageForeground != null)
			{
				rawImageForeground.gameObject.SetActive(value: false);
			}
			if (levelEvent is LevelEvent_SetBackgroundColor)
			{
				LevelEvent_SetBackgroundColor levelEvent_SetBackgroundColor = levelEvent as LevelEvent_SetBackgroundColor;
				Color color = levelEvent_SetBackgroundColor.color.ToColor(useEditorPalette: true);
				if (levelEvent_SetBackgroundColor.backgroundType == BackgroundType.Color)
				{
					icon.color = Color.black;
					rawImageBackground.gameObject.SetActive(value: true);
					rawImageBackground.color = color;
				}
				else if (levelEvent_SetBackgroundColor.images.Length == 0)
				{
					icon.color = Color.white;
				}
				else
				{
					string[] images = levelEvent_SetBackgroundColor.images;
					if (images != null && images.Length != 0)
					{
						Texture2D customTextureWithName = RDEditorUtils.GetCustomTextureWithName(images[0]);
						rawImageBackground.texture = customTextureWithName;
						icon.color = ((customTextureWithName == null) ? color : Color.white);
						rawImageBackground.gameObject.SetActive(value: true);
						rawImageBackground.color = color;
					}
				}
			}
			else if (levelEvent is LevelEvent_SetForeground)
			{
				LevelEvent_SetForeground levelEvent_SetForeground = levelEvent as LevelEvent_SetForeground;
				Color color2 = levelEvent_SetForeground.color.ToColor(useEditorPalette: true);
				string[] images2 = levelEvent_SetForeground.images;
				if (images2 != null && images2.Length != 0)
				{
					Texture2D customTextureWithName2 = RDEditorUtils.GetCustomTextureWithName(levelEvent_SetForeground.images[0]);
					rawImageForeground.texture = customTextureWithName2;
					icon.color = ((customTextureWithName2 == null) ? color2 : Color.white);
					rawImageForeground.gameObject.SetActive(value: true);
					rawImageForeground.color = color2;
				}
				else
				{
					icon.color = Color.white;
				}
			}
			else if (levelEvent is LevelEvent_CustomFlash)
			{
				LevelEvent_CustomFlash levelEvent_CustomFlash = levelEvent as LevelEvent_CustomFlash;
				icon.color = levelEvent_CustomFlash.startColor.GetValueOrDefault(Color.white).WithAlpha((float)(levelEvent_CustomFlash.startOpacity ?? 0) / 100f).ToColor(useEditorPalette: true);
				iconSecondary.sprite = base.secondaryIconSprite;
				iconSecondary.color = levelEvent_CustomFlash.endColor.GetValueOrDefault(Color.white).WithAlpha((float)(levelEvent_CustomFlash.endOpacity ?? 0) / 100f).ToColor(useEditorPalette: true);
				iconSecondary.gameObject.SetActive(value: true);
				iconTertiary.sprite = LevelEventControl_Base.GetIconFromName(levelEvent.name + "_Outline");
				iconTertiary.gameObject.SetActive(value: true);
			}
			else if (levelEvent is LevelEvent_TintRows)
			{
				LevelEvent_TintRows levelEvent_TintRows = levelEvent as LevelEvent_TintRows;
				Color color3 = ((levelEvent_TintRows.tint == true) ? levelEvent_TintRows.tintColor.ToColor(useEditorPalette: true) : Color.white);
				icon.color = new Color(color3.r + 1f - color3.a, color3.g + 1f - color3.a, color3.b + 1f - color3.a, 1f);
				if (levelEvent_TintRows.border.HasValue)
				{
					iconSecondary.color = levelEvent_TintRows.borderColor.ToColor(useEditorPalette: true);
					iconSecondary.sprite = LevelEventControl_Base.GetIconFromName(levelEvent.name + ((levelEvent_TintRows.border == BorderType.Glow) ? "_Glow" : "_Outline"));
				}
				iconSecondary.gameObject.SetActive(levelEvent_TintRows.border.HasValue && levelEvent_TintRows.border.Value != BorderType.None);
			}
			else if (levelEvent is LevelEvent_PaintHands)
			{
				LevelEvent_PaintHands levelEvent_PaintHands = levelEvent as LevelEvent_PaintHands;
				Color color4 = ((levelEvent_PaintHands.tint == true) ? levelEvent_PaintHands.tintColor.ToColor(useEditorPalette: true) : Color.white);
				icon.color = new Color(color4.r + 1f - color4.a, color4.g + 1f - color4.a, color4.b + 1f - color4.a, 1f);
				if (levelEvent_PaintHands.border.HasValue)
				{
					iconSecondary.color = levelEvent_PaintHands.borderColor.ToColor(useEditorPalette: true);
					iconSecondary.sprite = LevelEventControl_Base.GetIconFromName(levelEvent.name + ((levelEvent_PaintHands.border == BorderType.Glow) ? "_Glow" : "_Outline"));
				}
				iconSecondary.gameObject.SetActive(levelEvent_PaintHands.border.HasValue && levelEvent_PaintHands.border.Value != BorderType.None);
			}
			else if (levelEvent is LevelEvent_FloatingText)
			{
				LevelEvent_FloatingText levelEvent_FloatingText = levelEvent as LevelEvent_FloatingText;
				icon.color = levelEvent_FloatingText.color.ToColor(useEditorPalette: true);
				iconSecondary.color = levelEvent_FloatingText.outlineColor.ToColor(useEditorPalette: true);
				iconSecondary.sprite = base.secondaryIconSprite;
				iconSecondary.gameObject.SetActive(value: true);
				foreach (LevelEventControl_Base eventControls_action in base.editor.eventControls_actions)
				{
					if (eventControls_action.levelEvent is LevelEvent_AdvanceText && ((LevelEvent_AdvanceText)eventControls_action.levelEvent).id == levelEvent_FloatingText.id)
					{
						eventControls_action.UpdateUI();
					}
				}
			}
			else if (levelEvent is LevelEvent_AdvanceText)
			{
				component.sizeDelta = new Vector2(Mathf.Min(14f, (float)base.cellWidth * 0.5f), base.cellHeight);
				LevelEvent_FloatingText floatingText = (levelEvent as LevelEvent_AdvanceText).floatingText;
				RectTransform rectTransform = icon.rectTransform;
				rectTransform.anchorMin = new Vector2(0.5f, 0f);
				rectTransform.anchorMax = new Vector2(0.5f, 1f);
				rectTransform.sizeDelta = VectorExtensions.WithX(x: 10f * Mathf.Min(component.sizeDelta.x / 14f * 2f, 1f), v: rectTransform.sizeDelta);
				if (floatingText != null)
				{
					visible = floatingText.showChildren;
					icon.color = floatingText.color.ToColor(useEditorPalette: true);
					component.anchoredPosition = anchoredPosition;
				}
			}
			else if (levelEvent is LevelEvent_Comment)
			{
				if (levelEvent.tab == Tab.Decorations)
				{
					SetPosWithDecorationTarget();
				}
				initialColor = (levelEvent as LevelEvent_Comment).color.ToColor(useEditorPalette: true);
				image.color = (levelEvent.active ? initialColor : Color.gray).WithAlpha(image.color.a);
			}
			else
			{
				icon.color = Color.white;
			}
			if (levelEvent is LevelEvent_FlipScreen)
			{
				LevelEvent_FlipScreen levelEvent_FlipScreen = levelEvent as LevelEvent_FlipScreen;
				if (levelEvent_FlipScreen.flip[0] && levelEvent_FlipScreen.flip[1])
				{
					icon.sprite = LevelEventControl_Base.GetIconFromName(levelEvent.name + "Diagonal");
				}
				else if (levelEvent_FlipScreen.flip[0] && !levelEvent_FlipScreen.flip[1])
				{
					icon.sprite = LevelEventControl_Base.GetIconFromName(levelEvent.name + "Horizontal");
				}
				else if (!levelEvent_FlipScreen.flip[0] && levelEvent_FlipScreen.flip[1])
				{
					icon.sprite = LevelEventControl_Base.GetIconFromName(levelEvent.name + "Vertical");
				}
				else
				{
					icon.sprite = LevelEventControl_Base.GetIconFromName(levelEvent.name + "None");
				}
			}
			else
			{
				icon.sprite = base.iconSprite;
			}
			UpdateDurationBar(selected);
		}

		public override void OnDoubleClick()
		{
			if (levelEvent is LevelEvent_FloatingText)
			{
				LevelEvent_FloatingText levelEvent_FloatingText = (LevelEvent_FloatingText)levelEvent;
				SelectTextFamily(levelEvent_FloatingText.id);
			}
			else if (levelEvent is LevelEvent_AdvanceText)
			{
				LevelEvent_AdvanceText levelEvent_AdvanceText = (LevelEvent_AdvanceText)levelEvent;
				SelectTextFamily(levelEvent_AdvanceText.id);
			}
		}

		private void SelectTextFamily(int familyId)
		{
			scrConductor.PlayImmediately("sndEditorEventSelectMultiple", 1f, RDUtils.GetMixerGroup("LevelEditorActive"), 1f, 0f, isBeatSound: false, ignoreListenerPause: true);
			foreach (LevelEventControl_Base eventControls_action in base.editor.eventControls_actions)
			{
				if (eventControls_action == this)
				{
					continue;
				}
				if (eventControls_action.levelEvent is LevelEvent_FloatingText)
				{
					if ((eventControls_action.levelEvent as LevelEvent_FloatingText).id == familyId)
					{
						base.editor.AddEventControlToSelection(eventControls_action);
					}
				}
				else if (eventControls_action.levelEvent is LevelEvent_AdvanceText && (eventControls_action.levelEvent as LevelEvent_AdvanceText).id == familyId)
				{
					base.editor.AddEventControlToSelection(eventControls_action);
				}
			}
		}

		public override void ShowAsSelected(bool notBeingActuallySelected = false)
		{
			base.ShowAsSelected(notBeingActuallySelected);
		}

		public override void ShowAsDeselected()
		{
			base.ShowAsDeselected();
		}

		private void Update()
		{
		}
	}
}
