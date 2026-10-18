using System;

namespace RDLevelEditor
{
	[AttributeUsage(AttributeTargets.Property)]
	public class BPMCalculatorAttribute : ControlAttribute
	{
		public BPMCalculatorAttribute()
			: base(multiline: true, 0)
		{
		}
	}
}
