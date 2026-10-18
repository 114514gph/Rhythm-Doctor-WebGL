namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnBar, 0, RoomsUsage.ManyRoomsAndOnTop, true, true, true, false, -10, false)]
	public class LevelEvent_InvertColors : LevelEvent_Base
	{
		[JsonProperty("", "", null, "", false, true, null, null)]
		[ToggleGroup(false, null, "editor.InvertColors.enable", null, true, 14)]
		public bool enable { get; set; } = true;

		public override void Run()
		{
			int[] array = rooms;
			foreach (int room in array)
			{
				RunOnBeat(() =>
				{
					base.vfx.InvertScreenColors(enable, room);
				});
			}
		}

		public override string GetTooltipText()
		{
			return RDString.Get(enable ? "editor.InvertColors.enable.on" : "editor.InvertColors.enable.off");
		}
	}
}
