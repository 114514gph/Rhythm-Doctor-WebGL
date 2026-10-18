using System;
using UnityEngine;

namespace RDLevelEditor
{
	[AttributeUsage(AttributeTargets.Property)]
	public class DescriptionAttribute : ControlAttribute
	{
		public readonly string key;

		public readonly int height;

		public readonly int padding;

		public readonly TextAnchor alignment;

		public DescriptionAttribute(string key, int height = 28, int padding = 16, TextAnchor alignment = TextAnchor.MiddleCenter)
			: base(multiline: true, 0)
		{
			this.key = key;
			this.height = height;
			this.padding = padding;
			this.alignment = alignment;
		}
	}
}
