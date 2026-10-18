using UnityEngine;

namespace RhythmWeightlifter
{
	public class RWSound : MonoBehaviour
	{
		public Transform soundsParent;

		[SerializeField]
		private AudioClip audioClip;

		[SerializeField]
		[Range(0f, 1f)]
		private float volume = 1f;

		[SerializeField]
		[Range(-1f, 1f)]
		private float pan;

		private RWSoundHandler handler;

		public float Volume
		{
			get
			{
				return handler?.Volume ?? volume;
			}
			set
			{
				volume = value;
				if (handler != null)
				{
					handler.Volume = value;
				}
			}
		}

		public float Pan
		{
			get
			{
				return handler?.Pan ?? pan;
			}
			set
			{
				pan = value;
				if (handler != null)
				{
					handler.Pan = value;
				}
			}
		}

		public void Setup()
		{
			handler = new RWSoundHandler_Tempo();
			handler.context = this;
			handler.Setup(audioClip, volume, pan, soundsParent);
		}

		public void Play()
		{
			handler.Play();
		}

		public void Schedule(double time)
		{
			handler.Schedule(time);
		}

		public void Unschedule()
		{
			handler.Unschedule();
		}
	}
}
