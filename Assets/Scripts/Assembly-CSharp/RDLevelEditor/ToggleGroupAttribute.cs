using System;

namespace RDLevelEditor
{
	[AttributeUsage(AttributeTargets.Property)]
	public class ToggleGroupAttribute : ControlAttribute
	{
		public readonly bool icons;

		public readonly string customIcon;

		public readonly string customLocalizationKey;

		public readonly string[] options;

		public ToggleGroupAttribute(bool icons = false, string customIcon = null, string customLocalizationKey = null, string[] options = null, bool multiline = false, int labelHeight = 14)
			: base(multiline, labelHeight)
		{
			this.icons = icons;
			this.customIcon = customIcon;
			this.customLocalizationKey = customLocalizationKey;
			this.options = options;
		}
	}
}
