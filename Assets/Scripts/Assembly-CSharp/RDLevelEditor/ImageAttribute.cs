using System;

namespace RDLevelEditor
{
	[AttributeUsage(AttributeTargets.Property)]
	public class ImageAttribute : ControlAttribute
	{
		public ImageAttribute(bool multiline = true)
			: base(multiline)
		{
		}
	}
}
