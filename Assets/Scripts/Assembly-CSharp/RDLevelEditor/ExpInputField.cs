using System;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class ExpInputField : RDInputField
	{
		protected override void Awake()
		{
			base.Awake();
			base.onValidateInput = (OnValidateInput)Delegate.Combine(base.onValidateInput, new OnValidateInput(Validator));
		}

		private char Validator(string text, int charIndex, char addedChar)
		{
			if (charIndex == 0)
			{
				if (text.StartsWith("{"))
				{
					return '\0';
				}
				if (addedChar == '{')
				{
					return '{';
				}
			}
			else if (charIndex == text.Length)
			{
				if (text.EndsWith("}"))
				{
					return '\0';
				}
				if (addedChar == '}')
				{
					return '}';
				}
			}
			if (text.StartsWith("{"))
			{
				return addedChar;
			}
			return Validate(text, charIndex, addedChar);
		}

		protected override bool IsInvalid(string s)
		{
			if (base.text.StartsWith("{"))
			{
				return false;
			}
			return base.IsInvalid(s);
		}
	}
}
