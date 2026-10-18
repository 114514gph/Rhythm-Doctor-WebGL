using System;

namespace RDLevelEditor
{
	[AttributeUsage(AttributeTargets.Property)]
	public class SetRoomPerspectiveAttribute : ControlAttribute
	{
		public readonly float min;

		public readonly float max;

		public SetRoomPerspectiveAttribute(float min = float.NegativeInfinity, float max = float.PositiveInfinity)
			: base(multiline: true)
		{
			this.min = min;
			this.max = max;
		}
	}
}
