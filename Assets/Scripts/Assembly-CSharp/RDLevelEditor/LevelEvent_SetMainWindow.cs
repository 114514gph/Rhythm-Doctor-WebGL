using UnityEngine;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnBar, 0, RoomsUsage.NotUsed, true, true, true, false, -10, false)]
	public class LevelEvent_SetMainWindow : LevelEvent_Base
	{
		public int window => y;

		[JsonProperty("", "", null, "", false, true, null, null)]
		[Description("editor.SetMainWindow.description", 42, 16, TextAnchor.MiddleCenter)]
		public bool description => false;

		public override void Run()
		{
			RunOnBeat(() =>
			{
				WindowChoreographer windowChoreographer = base.game.windowChoreographer;
				if (windowChoreographer != null)
				{
					WindowDancer[] dancers = windowChoreographer.dancers;
					foreach (WindowDancer obj in dancers)
					{
						int index = obj.window.index;
						obj.window.shouldRenderUI = index == window;
					}
				}
			});
		}
	}
}
