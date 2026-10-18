namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnPrebar, -9, RoomsUsage.NotUsed, true, true, true, false, -10, true)]
	public class LevelEvent_SetBeatsPerMinute : LevelEvent_Base
	{
		[JsonProperty("", "", null, "", false, true, null, null)]
		[BPMCalculator]
		[FloatInfo(1f, 1000f)]
		public float beatsPerMinute { get; set; } = 100f;

		public override void Run()
		{
			if (RDUtils.WithinSmallMargin(base.beat, 1f))
			{
				base.conductor.SetBPM(beatsPerMinute);
			}
			base.conductor.AddBPMChange(base.beat - 1f, beatsPerMinute);
		}

		public override string GetTooltipText()
		{
			return beatsPerMinute + " BPM";
		}
	}
}
