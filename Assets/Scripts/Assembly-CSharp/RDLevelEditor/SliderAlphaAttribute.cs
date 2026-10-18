using System;

namespace RDLevelEditor
{
	[AttributeUsage(AttributeTargets.Property)]
	public class SliderAlphaAttribute : ControlAttribute
	{
		public SliderAlphaAttribute(bool multiline = false)
			: base(multiline)
		{
		}
	}
}
