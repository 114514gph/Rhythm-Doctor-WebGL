using System;

namespace RDLevelEditor
{
	[AttributeUsage(AttributeTargets.Property)]
	public class SetGameSoundAttribute : SoundAttribute
	{
		public SetGameSoundAttribute()
			: base(customFile: true, playSoundOnEdit: true, updateTimeline: false, isSong: false, null, multiline: true, 0)
		{
		}
	}
}
