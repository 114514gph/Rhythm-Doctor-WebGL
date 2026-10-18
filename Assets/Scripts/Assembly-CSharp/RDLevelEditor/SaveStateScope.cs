using System;

namespace RDLevelEditor
{
	public class SaveStateScope : IDisposable
	{
		private scnEditor editor => scnEditor.instance;

		public SaveStateScope(bool clearRedo = true, bool skipSaving = false, bool skipTimelinePos = false)
		{
			if (!skipSaving)
			{
				editor.SaveState(clearRedo, skipTimelinePos);
			}
			editor.changingState++;
		}

		public void Dispose()
		{
			editor.changingState--;
		}
	}
}
