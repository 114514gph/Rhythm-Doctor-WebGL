using System;

namespace RDLevelEditor
{
	[AttributeUsage(AttributeTargets.Property)]
	public class ReorderRoomsAttribute : ControlAttribute
	{
		public readonly bool isWindow;

		public ReorderRoomsAttribute(bool isWindow = false)
			: base(multiline: true)
		{
			this.isWindow = isWindow;
		}
	}
}
