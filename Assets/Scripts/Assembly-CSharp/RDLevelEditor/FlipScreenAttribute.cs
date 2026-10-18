using System;

namespace RDLevelEditor
{
	[AttributeUsage(AttributeTargets.Property)]
	public class FlipScreenAttribute : ControlAttribute
	{
		public FlipScreenAttribute()
			: base(multiline: true, 0)
		{
		}
	}
}
