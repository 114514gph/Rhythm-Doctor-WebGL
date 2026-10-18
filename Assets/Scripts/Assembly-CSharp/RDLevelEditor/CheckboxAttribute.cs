using System;

namespace RDLevelEditor
{
	[AttributeUsage(AttributeTargets.Property)]
	public class CheckboxAttribute : ControlAttribute
	{
		public CheckboxAttribute()
			: base(multiline: true, 0)
		{
		}
	}
}
