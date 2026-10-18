using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class LevelEventControl_Decoration : LevelEventControl_Base
	{
		public Image icon;

		public Image iconSecondary;

		public Text label;

		private Color incompatibleDecorationColor = new Color(0.1f, 0.25f, 0.15f, 1f);

		public override void UpdateUIInternal()
		{
			base.UpdateUIInternal();
			SetPosWithDecorationTarget();
			icon.sprite = base.iconSprite;
			if (levelEvent is LevelEvent_Tint)
			{
				LevelEvent_Tint levelEvent_Tint = levelEvent as LevelEvent_Tint;
				Color color = ((levelEvent_Tint.tint == true) ? levelEvent_Tint.tintColor.ToColor(useEditorPalette: true) : Color.white);
				icon.color = new Color(color.r + 1f - color.a, color.g + 1f - color.a, color.b + 1f - color.a, 1f);
				if (levelEvent_Tint.border.HasValue)
				{
					iconSecondary.color = levelEvent_Tint.borderColor.ToColor(useEditorPalette: true);
					iconSecondary.sprite = LevelEventControl_Base.GetIconFromName(levelEvent.name + ((levelEvent_Tint.border == BorderType.Glow) ? "_Glow" : "_Outline"));
				}
				iconSecondary.gameObject.SetActive(levelEvent_Tint.border.HasValue && levelEvent_Tint.border.Value != BorderType.None);
			}
			else if (levelEvent is LevelEvent_TintText)
			{
				LevelEvent_TintText levelEvent_TintText = levelEvent as LevelEvent_TintText;
				if (levelEvent_TintText.tintColor.HasValue)
				{
					icon.color = levelEvent_TintText.tintColor.Value.ToColor(useEditorPalette: true);
				}
				else
				{
					icon.color = Color.white;
				}
				if (levelEvent_TintText.borderColor.HasValue)
				{
					iconSecondary.color = levelEvent_TintText.borderColor.Value.ToColor(useEditorPalette: true);
					iconSecondary.sprite = base.secondaryIconSprite;
					iconSecondary.gameObject.SetActive(value: true);
				}
				else
				{
					iconSecondary.gameObject.SetActive(value: false);
				}
			}
			else if (levelEvent is LevelEvent_SetText)
			{
				LevelEvent_SetText levelEvent_SetText = levelEvent as LevelEvent_SetText;
				icon.color = levelEvent_SetText.color.ToColor(useEditorPalette: true);
				iconSecondary.color = levelEvent_SetText.outlineColor.ToColor(useEditorPalette: true);
				iconSecondary.sprite = base.secondaryIconSprite;
				iconSecondary.gameObject.SetActive(value: true);
			}
			else if (levelEvent is LevelEvent_AdvanceTextDecoration)
			{
				RectTransform component = GetComponent<RectTransform>();
				component.sizeDelta = new Vector2(Mathf.Min(14f, (float)base.cellWidth * 0.5f), base.cellHeight);
				RectTransform rectTransform = icon.rectTransform;
				rectTransform.anchorMin = new Vector2(0.5f, 0f);
				rectTransform.anchorMax = new Vector2(0.5f, 1f);
				rectTransform.sizeDelta = VectorExtensions.WithX(x: 10f * Mathf.Min(component.sizeDelta.x / 14f * 2f, 1f), v: rectTransform.sizeDelta);
			}
			else
			{
				icon.color = Color.white;
			}
		}

		private void Update()
		{
		}

		public override void ShowAsSelected(bool notBeingActuallySelected = false)
		{
			base.ShowAsSelected(notBeingActuallySelected);
			if (levelEvent is ICompatibleDecorationTypeChecker { isCompatibleDecorationType: false })
			{
				Color color = incompatibleDecorationColor.WithAlpha(selectedAlpha);
				image.color = color;
			}
		}

		public override void ShowAsDeselected()
		{
			base.ShowAsDeselected();
			if (levelEvent is ICompatibleDecorationTypeChecker { isCompatibleDecorationType: false })
			{
				Color color = incompatibleDecorationColor.WithAlpha(deselectedAlpha);
				image.color = color;
			}
		}
	}
}
