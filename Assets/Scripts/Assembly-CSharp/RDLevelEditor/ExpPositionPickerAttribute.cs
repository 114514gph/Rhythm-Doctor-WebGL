using System;

namespace RDLevelEditor
{
	[AttributeUsage(AttributeTargets.Property)]
	public class ExpPositionPickerAttribute : ControlAttribute
	{
		public readonly bool crosshair;

		public readonly bool percent;

		public ExpPositionPickerAttribute(bool crosshair = false, bool percent = true, bool multiline = false)
			: base(multiline)
		{
			this.crosshair = crosshair;
			this.percent = percent;
		}
	}
}
