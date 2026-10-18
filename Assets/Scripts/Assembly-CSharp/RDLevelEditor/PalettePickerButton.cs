using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class PalettePickerButton : RDEditorBase
	{
		public Image colorImage;

		public void SetImageColor(Color color)
		{
			colorImage.color = color;
		}
	}
}
