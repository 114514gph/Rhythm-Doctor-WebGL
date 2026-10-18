using System;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class ExpPositionPicker : PositionPicker
	{
		public new FloatExpression2 percent
		{
			get
			{
				FloatExpression2 floatExpression = new FloatExpression2(FloatExpression.FromString(x.text), FloatExpression.FromString(y.text));
				if (!usingPercentUnits)
				{
					return PixelsToPercent(floatExpression);
				}
				return floatExpression;
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

		public new FloatExpression2 pixels
		{
			get
			{
				FloatExpression2 floatExpression = new FloatExpression2(FloatExpression.FromString(x.text), FloatExpression.FromString(y.text));
				if (usingPercentUnits)
				{
					return PercentToPixels(floatExpression);
				}
				return floatExpression;
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

		public void Awake()
		{
			InputField inputField = x;
			inputField.onValidateInput = (InputField.OnValidateInput)Delegate.Combine(inputField.onValidateInput, new InputField.OnValidateInput(ExpValidate));
			InputField inputField2 = y;
			inputField2.onValidateInput = (InputField.OnValidateInput)Delegate.Combine(inputField2.onValidateInput, new InputField.OnValidateInput(ExpValidate));
		}

		private static char ExpValidate(string input, int charIndex, char addedChar)
		{
			if (charIndex == 0)
			{
				if (input.StartsWith("{"))
				{
					return '\0';
				}
				if ("0123456789.{-".Contains(addedChar.ToString()))
				{
					return addedChar;
				}
			}
			else
			{
				if (charIndex == input.Length && input.EndsWith("}"))
				{
					return '\0';
				}
				if (input.StartsWith("{"))
				{
					return addedChar;
				}
				string text = "0123456789";
				if (!input.Contains("."))
				{
					text += ".";
				}
				if (text.Contains(addedChar.ToString()))
				{
					return addedChar;
				}
			}
			return '\0';
		}

		public new void SwitchUnits()
		{
			FloatExpression2 value = (usingPercentUnits ? PercentToPixels(percent) : PixelsToPercent(pixels));
			SetValue(value, invokeChange: false);
			usingPercentUnits = !usingPercentUnits;
			units.text = (usingPercentUnits ? "%" : "px");
		}

		private void SetValue(FloatExpression2 value, bool invokeChange)
		{
			playOnEndEdit = false;
			x.text = value.x.ToString();
			y.text = value.y.ToString();
			if (invokeChange)
			{
				x.onEndEdit.Invoke(x.text);
			}
			playOnEndEdit = true;
		}

		private FloatExpression2 PercentToPixels(FloatExpression2 p)
		{
			return new FloatExpression2(p.x * 352f / 100f, p.y * 198f / 100f);
		}

		private FloatExpression2 PixelsToPercent(FloatExpression2 p)
		{
			return new FloatExpression2(p.x / 352f * 100f, p.y / 198f * 100f);
		}
	}
}
