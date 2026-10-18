namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnBar, 0, RoomsUsage.NotUsed, true, true, true, false, -10, false)]
	public class LevelEvent_ReorderWindows : LevelEvent_Base
	{
		[JsonProperty("", "", null, "", false, true, null, null)]
		[ReorderRooms(true)]
		public int[] order { get; set; } = new int[4] { 0, 1, 2, 3 };

		public override void Run()
		{
			if (base.game.windowChoreographer != null)
			{
				RunOnBeat(() =>
				{
					base.game.windowChoreographer.zOrder = order;
				});
			}
		}
	}
}
