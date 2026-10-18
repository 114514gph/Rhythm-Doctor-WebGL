using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmWeightlifter
{
	public class RWSoundHandler_Unity : RWSoundHandler
	{
		private AudioSource audioSource;

		private List<AudioSource> activeAudioSources = new List<AudioSource>();

		public override float Volume
		{
			get
			{
				return audioSource.volume;
			}
			set
			{
				audioSource.volume = value;
			}
		}

		public override float Pan
		{
			get
			{
				return audioSource.panStereo;
			}
			set
			{
				audioSource.panStereo = value;
			}
		}

		public override void Setup(AudioClip audioClip, float volume, float pan, Transform soundsParent = null)
		{
			GameObject gameObject = new GameObject("RWSoundUnity_" + audioClip.name);
			gameObject.SetActive(value: false);
			if (soundsParent != null)
			{
				gameObject.transform.parent = soundsParent;
			}
			audioSource = gameObject.AddComponent<AudioSource>();
			audioSource.playOnAwake = false;
			audioSource.clip = audioClip;
			audioSource.outputAudioMixerGroup = RDUtils.GetMixerGroup("MasterPrivate");
			gameObject.SetActive(value: true);
			audioSource.volume = volume;
			audioSource.panStereo = pan;
		}

		public override void Play()
		{
			audioSource.Play();
		}

		public override void Schedule(double time)
		{
			AudioSource audioSource = this.audioSource.gameObject.AddComponent<AudioSource>();
			audioSource.playOnAwake = false;
			audioSource.clip = this.audioSource.clip;
			audioSource.outputAudioMixerGroup = RDUtils.GetMixerGroup("MasterPrivate");
			audioSource.volume = Volume;
			audioSource.panStereo = Pan;
			audioSource.PlayScheduled(time);
			activeAudioSources.Add(audioSource);
			double dspTime = AudioSettings.dspTime;
			float delay = (float)(time - dspTime) + audioSource.clip.length + 0.1f;
			context.StartCoroutine(DestroyCopyAudioSource(audioSource, delay));
		}

		private IEnumerator DestroyCopyAudioSource(AudioSource copyAudioSource, float delay)
		{
			yield return new WaitForSeconds(delay);
			if (activeAudioSources.Contains(copyAudioSource))
			{
				activeAudioSources.Remove(copyAudioSource);
			}
			if (copyAudioSource != null)
			{
				Object.Destroy(copyAudioSource);
			}
		}

		public override void Unschedule()
		{
			context.StopAllCoroutines();
			foreach (AudioSource activeAudioSource in activeAudioSources)
			{
				if (activeAudioSource != null)
				{
					activeAudioSource.Stop();
					Object.Destroy(activeAudioSource);
				}
			}
			activeAudioSources.Clear();
			audioSource.Stop();
		}
	}
}
