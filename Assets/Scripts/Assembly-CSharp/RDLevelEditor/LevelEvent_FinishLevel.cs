using UnityEngine;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnBar, 0, RoomsUsage.NotUsed, true, true, true, false, -10, false)]
	public class LevelEvent_FinishLevel : LevelEvent_Base
	{
		[JsonProperty("", "", null, "", false, true, null, null)]
		[Description("editor.FinishLevel.description", 84, 16, TextAnchor.MiddleCenter)]
		public bool description => false;

		public override void Run()
		{
			RunOnBeat(() =>
			{
				base.game.rankscreen.AdvanceGameover();
			});
		}
	}
}
