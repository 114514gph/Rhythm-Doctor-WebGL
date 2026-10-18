using UnityEngine;

namespace RhythmWeightlifter
{
	public abstract class RWSoundHandler
	{
		public MonoBehaviour context;

		public abstract float Volume { get; set; }

		public abstract float Pan { get; set; }

		public abstract void Setup(AudioClip audioClip, float volume, float pan, Transform soundsParent = null);

		public abstract void Play();

		public abstract void Schedule(double time);

		public abstract void Unschedule();
	}
}
