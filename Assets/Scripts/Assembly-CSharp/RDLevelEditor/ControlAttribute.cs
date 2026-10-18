using System;

namespace RDLevelEditor
{
	[AttributeUsage(AttributeTargets.Property)]
	public class ControlAttribute : Attribute
	{
		public readonly bool multiline;

		public readonly int labelHeight;

		public ControlAttribute(bool multiline, int labelHeight = 14)
		{
			this.multiline = multiline;
			this.labelHeight = labelHeight;
		}
	}
}
