namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnPrebar, 0, RoomsUsage.NotUsed, true, false, true, false, -10, false)]
	public class LevelEvent_SetCrotchetsPerBar : LevelEvent_Base
	{
		[JsonProperty("", "", null, "", false, true, null, null)]
		[IntInfo(1, int.MaxValue)]
		public int crotchetsPerBar { get; set; } = 8;

		[JsonProperty("", "", null, "", false, true, null, null)]
		[FloatInfo(0f, float.PositiveInfinity)]
		public float visualBeatMultiplier { get; set; } = 1f;

		public override void Run()
		{
			base.conductor.crotchetsPerBar = crotchetsPerBar;
			base.conductor.visualBeatLengthMultiplier = visualBeatMultiplier;
		}

		public override string GetTooltipText()
		{
			string text = RDString.Get("editor.SetCrotchetsPerBar.visualBeatMultiplier") + ": " + visualBeatMultiplier;
			if (crotchetsPerBar >= 10)
			{
				text = string.Join('\n', $"{crotchetsPerBar}/4", text);
			}
			return text;
		}
	}
}
