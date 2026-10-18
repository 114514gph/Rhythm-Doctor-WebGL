using System;

namespace RDLevelEditor
{
	[AttributeUsage(AttributeTargets.Property)]
	public class SliderAttribute : ControlAttribute
	{
		public readonly string unit;

		public readonly string customIcon;

		public readonly float expandInputBy;

		public readonly bool expandToLeft;

		public SliderAttribute(string unit = null, string customIcon = null, float expandInputBy = 0f, bool expandToLeft = true, bool multiline = false)
			: base(multiline)
		{
			this.unit = unit;
			this.customIcon = customIcon;
			this.expandInputBy = expandInputBy;
			this.expandToLeft = expandToLeft;
		}
	}
}
