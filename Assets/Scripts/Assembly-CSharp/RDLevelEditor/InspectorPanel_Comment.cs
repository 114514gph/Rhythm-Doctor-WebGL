using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class InspectorPanel_Comment : InspectorPanel
	{
		private PropertyControl_InputField _textControl;

		private bool focusedPrevious;

		private PropertyControl_InputField textControl => _textControl ?? (_textControl = properties.Find((Property x) => x.name == "text").control as PropertyControl_InputField);

		private void LateUpdate()
		{
			bool isFocused = textControl.inputField.isFocused;
			if (focusedPrevious != isFocused)
			{
				focusedPrevious = isFocused;
				UpdateTextColor(currentLevelEvent as LevelEvent_Comment, isFocused);
			}
		}

		public override void UpdateUIProperties(LevelEvent_Base levelEvent)
		{
			base.UpdateUIProperties(levelEvent);
			UpdateTextColor(levelEvent as LevelEvent_Comment);
		}

		public override void SaveProperties(LevelEvent_Base levelEvent)
		{
			LevelEvent_Comment levelEvent_Comment = (LevelEvent_Comment)levelEvent;
			foreach (Property property in properties)
			{
				property.Save(levelEvent);
			}
			foreach (Property property2 in properties)
			{
				if (property2.name == "text")
				{
					PropertyControl_InputField obj = (PropertyControl_InputField)property2.control;
					ColorBlock colors = obj.inputField.colors;
					Color color = (colors.normalColor = levelEvent_Comment.color.ToColor(useEditorPalette: true));
					colors.highlightedColor = color + Color.white * 0.1f;
					obj.inputField.colors = colors;
					obj.label.color = color;
				}
			}
			base.SaveProperties(levelEvent);
			UpdateTextColor(levelEvent as LevelEvent_Comment);
		}

		private void UpdateTextColor(LevelEvent_Comment data, bool editing = false)
		{
			LevelEvent_Comment.CommentColors commentColors = data.GetCommentColors(editing);
			ColorBlock colors = textControl.inputField.colors;
			colors.normalColor = commentColors.backgroundColor;
			colors.highlightedColor = colors.normalColor + Color.white * (RDEditorUtils.ColorIsDark(colors.normalColor) ? 0.05f : (-0.05f));
			textControl.inputField.colors = colors;
			textControl.inputField.textComponent.color = commentColors.textColor;
			textControl.label.color = commentColors.backgroundColor;
			textControl.label.GetComponent<EightSidedOutline>().effectColor = commentColors.outlineColor;
		}
	}
}
