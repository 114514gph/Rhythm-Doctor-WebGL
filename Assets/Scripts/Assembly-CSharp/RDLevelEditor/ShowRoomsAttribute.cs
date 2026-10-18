using System;

namespace RDLevelEditor
{
	[AttributeUsage(AttributeTargets.Property)]
	public class ShowRoomsAttribute : ControlAttribute
	{
		public ShowRoomsAttribute()
			: base(multiline: true, 0)
		{
		}
	}
}
