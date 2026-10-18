using System;

namespace RDLevelEditor
{
	[AttributeUsage(AttributeTargets.Property)]
	public class SliderPercentAttribute : ControlAttribute
	{
		public SliderPercentAttribute(bool multiline = false)
			: base(multiline)
		{
		}
	}
}
