using System;

namespace RDLevelEditor
{
	[AttributeUsage(AttributeTargets.Property)]
	public class PulsePickerAttribute : ControlAttribute
	{
		public PulsePickerAttribute()
			: base(multiline: true)
		{
		}
	}
}
