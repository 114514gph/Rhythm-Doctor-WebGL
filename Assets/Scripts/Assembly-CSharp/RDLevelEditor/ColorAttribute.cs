using System;

namespace RDLevelEditor
{
	[AttributeUsage(AttributeTargets.Property)]
	public class ColorAttribute : ControlAttribute
	{
		public readonly bool alignToLeft;

		public ColorAttribute(bool alignToLeft = true, bool multiline = false)
			: base(multiline)
		{
			this.alignToLeft = alignToLeft;
		}
	}
}
