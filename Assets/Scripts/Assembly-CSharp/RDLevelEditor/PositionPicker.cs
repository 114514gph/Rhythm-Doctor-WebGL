using System;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class PositionPicker : RDEditorBase
	{
		public GameObject crosshair;

		public InputField x;

		public InputField y;

		public Text units;

		public bool usingPercentUnits = true;

		[NonSerialized]
		public int perspectiveCorner = -1;

		protected bool playOnEndEdit = true;

		public bool PlayOnEndEdit => playOnEndEdit;

		public bool interactable
		{
			set
			{
				x.interactable = value;
				y.interactable = value;
			}
		}

		public Int2 position
		{
			get
			{
				return new Int2(Convert.ToInt32(x.text), Convert.ToInt32(y.text));
			}
			set
			{
				x.text = value.x.ToString();
				y.text = value.y.ToString();
				x.onEndEdit.Invoke(x.text);
			}
		}

		public Float2 percent
		{
			get
			{
				Float2 @float = new Float2(0f, 0f);
				if (x.text.IsNullOrEmpty())
				{
					@float.xUsed = false;
				}
				else
				{
					@float.x = float.Parse(x.text);
				}
				if (y.text.IsNullOrEmpty())
				{
					@float.yUsed = false;
				}
				else
				{
					@float.y = float.Parse(y.text);
				}
				if (!usingPercentUnits)
				{
					return PixelsToPercent(@float);
				}
				return @float;
			}
			set
			{
				if (!usingPercentUnits)
				{
					value = PercentToPixels(value);
				}
				SetValue(value, invokeChange: true);
			}
		}

		public Float2 pixels
		{
			get
			{
				Float2 @float = new Float2(0f, 0f);
				if (x.text.IsNullOrEmpty())
				{
					@float.xUsed = false;
				}
				else
				{
					@float.x = float.Parse(x.text);
				}
				if (y.text.IsNullOrEmpty())
				{
					@float.yUsed = false;
				}
				else
				{
					@float.y = float.Parse(y.text);
				}
				if (usingPercentUnits)
				{
					return PercentToPixels(@float);
				}
				return @float;
			}
			set
			{
				if (usingPercentUnits)
				{
					value = PixelsToPercent(value);
				}
				SetValue(value, invokeChange: true);
			}
		}

		public void SelectUsingCrosshair()
		{
			base.editor.pickerUsingCrosshair = this;
		}

		public void SwitchUnits()
		{
			Float2 value = (usingPercentUnits ? PercentToPixels(percent) : PixelsToPercent(pixels));
			SetValue(value, invokeChange: false);
			usingPercentUnits = !usingPercentUnits;
			units.text = (usingPercentUnits ? "%" : "px");
		}

		private void SetValue(Float2 value, bool invokeChange)
		{
			playOnEndEdit = false;
			x.text = (value.xUsed ? value.x.ToString() : "");
			y.text = (value.yUsed ? value.y.ToString() : "");
			if (invokeChange)
			{
				x.onEndEdit.Invoke(x.text);
			}
			playOnEndEdit = true;
		}

		private Float2 PercentToPixels(Float2 p)
		{
			p.x = 352f * p.x / 100f;
			p.y = 198f * p.y / 100f;
			return p;
		}

		private Float2 PixelsToPercent(Float2 p)
		{
			p.x = p.x / 352f * 100f;
			p.y = p.y / 198f * 100f;
			return p;
		}
	}
}
