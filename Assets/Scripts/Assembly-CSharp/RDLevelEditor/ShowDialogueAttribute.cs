using System;

namespace RDLevelEditor
{
	[AttributeUsage(AttributeTargets.Property)]
	public class ShowDialogueAttribute : ControlAttribute
	{
		public ShowDialogueAttribute()
			: base(multiline: true, 0)
		{
		}
	}
}
