using UnityEngine;

namespace RhythmWeightlifter
{
	public class RWSoundController_Unity : RWSoundController
	{
		public override double Clock => AudioSettings.dspTime;

		public override float Volume
		{
			set
			{
			}
		}

		public override void Setup()
		{
		}
	}
}
