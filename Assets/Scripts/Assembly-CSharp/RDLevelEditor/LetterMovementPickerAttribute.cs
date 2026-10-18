using System;

namespace RDLevelEditor
{
	[AttributeUsage(AttributeTargets.Property)]
	public class LetterMovementPickerAttribute : ControlAttribute
	{
		public readonly LetterMovementType[] availableLetterMovementTypes;

		public LetterMovementPickerAttribute(LetterMovementType[] availableLetterMovementTypes, bool multiline = false)
			: base(multiline)
		{
			this.availableLetterMovementTypes = availableLetterMovementTypes;
		}
	}
}
