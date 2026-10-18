using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class InspectorPanel_CommentShow : InspectorPanel
	{
		public InputField comment;

		public Text bar;

		public Text beat;

		public Text labelText;

		private Color originalTextColor;

		public new void Awake()
		{
			base.Awake();
			originalTextColor = comment.textComponent.color;
		}

		public void UpdateTextColor(LevelEvent_Comment.CommentColors commentColors)
		{
			ColorBlock colors = comment.colors;
			colors.disabledColor = commentColors.backgroundColor;
			comment.colors = colors;
			comment.textComponent.color = commentColors.textColor;
			labelText.color = commentColors.backgroundColor;
			labelText.GetComponent<EightSidedOutline>().effectColor = commentColors.outlineColor;
		}
	}
}
