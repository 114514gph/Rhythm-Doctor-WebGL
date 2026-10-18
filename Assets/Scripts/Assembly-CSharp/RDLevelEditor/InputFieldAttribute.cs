using System;
using UnityEngine.UI;

namespace RDLevelEditor
{
	[AttributeUsage(AttributeTargets.Property)]
	public class InputFieldAttribute : ControlAttribute
	{
		public readonly string unit;

		public readonly InputField.LineType lineType;

		public readonly int characterLimit;

		public readonly int inputFieldHeight;

		public readonly bool autocomplete;

		public InputFieldAttribute(string unit = "", InputField.LineType lineType = InputField.LineType.SingleLine, int characterLimit = 40, int inputFieldHeight = 14, bool autocomplete = false, bool multiline = false)
			: base(multiline)
		{
			this.unit = unit;
			this.lineType = lineType;
			this.characterLimit = characterLimit;
			this.inputFieldHeight = inputFieldHeight;
			this.autocomplete = autocomplete;
		}
	}
}
