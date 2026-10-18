using System;

namespace RDLevelEditor
{
	[AttributeUsage(AttributeTargets.Property)]
	public class HandAttribute : ControlAttribute
	{
		public HandAttribute()
			: base(multiline: true)
		{
		}
	}
}
