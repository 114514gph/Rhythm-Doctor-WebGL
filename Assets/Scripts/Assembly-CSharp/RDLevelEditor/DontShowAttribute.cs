using System;

namespace RDLevelEditor
{
	[AttributeUsage(AttributeTargets.Property)]
	public class DontShowAttribute : ControlAttribute
	{
		public DontShowAttribute()
			: base(multiline: false)
		{
		}
	}
}
