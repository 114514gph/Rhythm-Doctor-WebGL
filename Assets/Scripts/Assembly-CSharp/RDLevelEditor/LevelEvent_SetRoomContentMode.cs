namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnBar, 0, RoomsUsage.NotUsed, true, true, true, false, -10, false)]
	public class LevelEvent_SetRoomContentMode : LevelEvent_Base
	{
		[JsonProperty("", "", null, "", false, true, null, null)]
		[ToggleGroup(false, null, null, new string[] { "Center", "AspectFill", "Real" }, true, 28)]
		public ContentMode mode { get; set; }

		public override void Run()
		{
			RunOnBeat(() =>
			{
				base.game.rooms[y].contentMode = mode;
			});
		}

		public override string CustomControlName()
		{
			return "Room";
		}

		public override string GetTooltipText()
		{
			return RDString.GetEnumValue(mode);
		}
	}
}
