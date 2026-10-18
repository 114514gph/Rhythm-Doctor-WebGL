using System;

namespace RDLevelEditor
{
	[AttributeUsage(AttributeTargets.Property)]
	public class SoundAttribute : ControlAttribute
	{
		public readonly bool customFile;

		public readonly bool playSoundOnEdit;

		public readonly bool updateTimeline;

		public readonly bool isSong;

		public readonly string optionsMethod;

		public SoundAttribute(bool customFile = true, bool playSoundOnEdit = true, bool updateTimeline = false, bool isSong = false, string optionsMethod = null, bool multiline = true, int labelHeight = 16)
			: base(multiline, labelHeight)
		{
			this.customFile = customFile;
			this.playSoundOnEdit = playSoundOnEdit;
			this.updateTimeline = updateTimeline;
			this.isSong = isSong;
			this.optionsMethod = optionsMethod;
		}
	}
}
