using TempoStudio;

namespace RhythmWeightlifter
{
	public class RWSoundController_Tempo : RWSoundController
	{
		public override double Clock => TempoAudio.Clock;

		public override float Volume
		{
			set
			{
				TempoAudio.Volume = value;
			}
		}

		public override void Setup()
		{
			TempoAudio.StartClock();
		}
	}
}
