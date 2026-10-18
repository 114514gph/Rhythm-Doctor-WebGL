using TempoStudio;
using UnityEngine;

namespace RhythmWeightlifter
{
	public class RWSoundHandler_Tempo : RWSoundHandler
	{
		private TempoSound tempoSound;

		public override float Volume
		{
			get
			{
				return tempoSound.Volume;
			}
			set
			{
				tempoSound.Volume = value;
			}
		}

		public override float Pan
		{
			get
			{
				return tempoSound.Pan;
			}
			set
			{
				tempoSound.Pan = value;
			}
		}

		public override void Setup(AudioClip audioClip, float volume, float pan, Transform soundsParent = null)
		{
			GameObject gameObject = new GameObject("RWSoundTempo_" + audioClip.name);
			gameObject.SetActive(value: false);
			if (soundsParent != null)
			{
				gameObject.transform.parent = soundsParent;
			}
			tempoSound = gameObject.AddComponent<TempoSound>();
			tempoSound.Init(audioClip);
			gameObject.SetActive(value: true);
			tempoSound.Volume = volume;
			tempoSound.Pan = pan;
		}

		public override void Play()
		{
			tempoSound.Play();
		}

		public override void Schedule(double time)
		{
			tempoSound.Schedule((float)time);
		}

		public override void Unschedule()
		{
			tempoSound.Unschedule();
		}
	}
}
