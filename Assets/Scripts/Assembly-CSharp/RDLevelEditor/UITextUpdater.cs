using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class UITextUpdater : MonoBehaviour
	{
		private Text currentText;

		private InputField inputField;

		private string oldText = string.Empty;

		private string currentTextString
		{
			get
			{
				if (!(inputField == null))
				{
					return inputField.text;
				}
				return currentText.text;
			}
		}

		private void Awake()
		{
			currentText = GetComponent<Text>();
			if (currentText == null)
			{
				inputField = GetComponent<InputField>();
				currentText = inputField.textComponent;
			}
		}

		private void Update()
		{
			if (oldText != currentTextString)
			{
				RDEditorUtils.UpdateUIText(currentText, currentTextString);
				oldText = currentTextString;
			}
		}
	}
}
