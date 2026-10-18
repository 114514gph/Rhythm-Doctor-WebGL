using System;

namespace RDLevelEditor
{
	[AttributeUsage(AttributeTargets.Property)]
	public class PositionPickerAttribute : ControlAttribute
	{
		public readonly bool crosshair;

		public readonly bool switchUnits;

		public readonly string units;

		public PositionPickerAttribute(bool crosshair = false, bool switchUnits = true, string units = null, bool multiline = false)
			: base(multiline)
		{
			this.crosshair = crosshair;
			this.switchUnits = switchUnits;
			this.units = units;
		}
	}
}
