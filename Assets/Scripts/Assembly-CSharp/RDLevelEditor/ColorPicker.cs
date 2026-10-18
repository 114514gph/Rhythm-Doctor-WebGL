using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class ColorPicker : RDEditorBase
	{
		public delegate void EndHandler(string data);

		public bool storesAlpha;

		public bool canUsePalette = true;

		public Image sample;

		public Image border;

		public Image star;

		private string _color;

		public EndHandler onEndEdit;

		public string color
		{
			get
			{
				return _color;
			}
			set
			{
				_color = value;
				sample.color = _color.HexOrPaletteIndexToColor(storesAlpha, useEditorPalette: true);
				border.color = (_color.StartsWith("pal") ? Color.white : Color.black);
			}
		}

		public ColorOrPalette colorOrPalette
		{
			get
			{
				return ColorOrPalette.FromString(_color);
			}
			set
			{
				_color = value.Encode(storesAlpha);
				sample.color = value.ToColor(useEditorPalette: true);
				border.color = (value.isPalette ? Color.white : Color.black);
				if (!storesAlpha)
				{
					sample.color = sample.color.WithAlpha(1f);
				}
			}
		}

		public void ShowColorPickerPopup()
		{
			base.editor.colorPickerPopup.Show(this);
		}

		private void OnEnable()
		{
			if (string.IsNullOrEmpty(color))
			{
				color = sample.color.ToHex().Replace("#", string.Empty);
			}
		}
	}
}
