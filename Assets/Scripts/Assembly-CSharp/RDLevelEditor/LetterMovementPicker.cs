using System;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class LetterMovementPicker : RDEditorBase
	{
		public delegate void EndHandler(LetterMovementType movementType);

		[NonSerialized]
		public LetterMovementType[] availableLetterMovementTypes;

		[NonSerialized]
		public LetterMovementType letterMovementType;

		public Text label;

		public EndHandler onEndEdit;

		public void SetLetterMovementType(LetterMovementType movementType)
		{
			letterMovementType = movementType;
			UpdateLabel(RDString.Get("enum.LetterMovementType." + movementType));
		}

		public void ShowLetterMovementPickerPopup()
		{
			base.editor.letterMovementPickerPopup.ShowLetterMovementTypes(this, availableLetterMovementTypes);
		}

		private void UpdateLabel(string str)
		{
			if (label != null)
			{
				label.text = str;
				label.font = RDString.GetAppropiateFontForString(str);
			}
		}
	}
}
