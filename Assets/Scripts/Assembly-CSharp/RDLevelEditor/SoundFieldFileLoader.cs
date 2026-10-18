using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class SoundFieldFileLoader : RDEditorBase
	{
		public InputField filename;

		public RectTransform rect;

		public Button settingsButton;

		public Button browseButton;

		public Text labelText;

		public Button labelButton;

		public RDColorHoverEventTrigger colorHoverEventTrigger;

		private bool on;

		public void ShowLabel(bool on)
		{
			this.on = on;
			filename.gameObject.SetActive(on);
			browseButton.gameObject.SetActive(on);
			settingsButton.gameObject.SetActive(on);
			labelText.color = (on ? Color.white : Color.gray);
			colorHoverEventTrigger.exitColor = labelText.color;
		}

		public void SetTextFromSoundType(GameSoundType soundType)
		{
			string text = (on ? "" : (" (" + RDString.Get("editor.off") + ")"));
			labelText.text = RDString.GetEnumValue(soundType) + text;
		}

		public void HideLabelButton(bool hidden)
		{
			ShowLabel(on);
			if (hidden)
			{
				labelText.text = "";
			}
			labelText.raycastTarget = !hidden;
		}
	}
}
