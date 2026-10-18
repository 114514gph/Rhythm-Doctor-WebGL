using System;

namespace RDLevelEditor
{
	[AttributeUsage(AttributeTargets.Property)]
	public class RoomAttribute : ControlAttribute
	{
		public readonly bool includeOnTop;

		public RoomAttribute(bool includeOnTop = false, bool multiline = false)
			: base(multiline)
		{
			this.includeOnTop = includeOnTop;
		}
	}
}
