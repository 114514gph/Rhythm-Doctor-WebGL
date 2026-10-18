namespace RhythmWeightlifter
{
	public abstract class RWSoundController
	{
		public abstract double Clock { get; }

		public abstract float Volume { set; }

		public abstract void Setup();
	}
}
