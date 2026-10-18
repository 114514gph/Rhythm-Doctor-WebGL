using System;

namespace RDLevelEditor
{
	[AttributeUsage(AttributeTargets.Property)]
	public class DropdownAttribute : ControlAttribute
	{
		public readonly string[] options;

		public readonly bool firstOptionIsNull;

		public DropdownAttribute(string[] options = null, bool firstOptionIsNull = false, bool multiline = false)
			: base(multiline)
		{
			this.options = options;
			this.firstOptionIsNull = firstOptionIsNull;
		}
	}
}
