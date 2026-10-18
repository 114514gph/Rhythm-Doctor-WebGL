using System;

namespace RDLevelEditor
{
	[AttributeUsage(AttributeTargets.Property)]
	public class BeatModifiersAttribute : ControlAttribute
	{
		public readonly bool leftClick;

		public readonly bool middleClick;

		public readonly bool rightClick;

		public readonly bool syncoBeat;

		public BeatModifiersAttribute(bool leftClick = true, bool middleClick = false, bool rightClick = false, bool syncoBeat = false)
			: base(multiline: true)
		{
			this.leftClick = leftClick;
			this.middleClick = middleClick;
			this.rightClick = rightClick;
			this.syncoBeat = syncoBeat;
		}
	}
}
