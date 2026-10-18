using System;

namespace RDLevelEditor
{
	[AttributeUsage(AttributeTargets.Property)]
	public class ButtonAttribute : ControlAttribute
	{
		public readonly string methodName;

		public readonly string key;

		public ButtonAttribute(string methodName, string key = null)
			: base(multiline: true, 0)
		{
			this.methodName = methodName;
			this.key = key;
		}
	}
}
