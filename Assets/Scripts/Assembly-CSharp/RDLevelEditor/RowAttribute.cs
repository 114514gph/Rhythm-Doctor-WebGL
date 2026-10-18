using System;

namespace RDLevelEditor
{
	[AttributeUsage(AttributeTargets.Property)]
	public class RowAttribute : ControlAttribute
	{
		public readonly bool includeAll;

		public RowAttribute(bool includeAll = false, bool multiline = false)
			: base(multiline)
		{
			this.includeAll = includeAll;
		}
	}
}
