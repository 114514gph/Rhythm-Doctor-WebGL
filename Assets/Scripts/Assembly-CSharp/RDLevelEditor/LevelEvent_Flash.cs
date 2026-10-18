using System;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnBar, 0, RoomsUsage.ManyRoomsAndOnTop, true, true, true, false, -10, false)]
	public class LevelEvent_Flash : LevelEvent_Base, IDurationHaver
	{
		[JsonProperty("duration", "", null, "", false, true, null, null)]
		[Slider(null, null, 0f, true, false)]
		public SimpleDuration simpleDuration { get; set; } = SimpleDuration.Short;

		public float duration
		{
			get
			{
				return GetSimpleDurationCrotchets(simpleDuration);
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public float GetSimpleDurationCrotchets(SimpleDuration simpleDuration)
		{
			return simpleDuration switch
			{
				SimpleDuration.Long => 4f, 
				SimpleDuration.Medium => 2f, 
				SimpleDuration.Short => 1f, 
				_ => throw new ArgumentOutOfRangeException(), 
			};
		}

		public override void Run()
		{
			float seconds = base.conductor.DurationBeatsToTime(base.beat - 1f, GetSimpleDurationCrotchets(simpleDuration));
			int[] array = rooms;
			foreach (int room in array)
			{
				RunOnBeat(() =>
				{
					base.vfx.Flash(room, seconds, 1f);
				});
			}
		}
	}
}
