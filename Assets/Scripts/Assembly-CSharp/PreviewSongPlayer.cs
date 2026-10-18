using System;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PreviewSongPlayer : RDBase
{
	public delegate void OnStartPreviewSong();

	private const float PreviewSongFadeDuration = 1f;

	private const float PreviewSongVolume = 0.2376f;

	private const float PreviewSongDiff = 0.04f;

	[NonSerialized]
	public bool UseFade;

	public bool StartingLevel;

	private AudioSource audioSource;

	private AudioClip clip;

	[NonSerialized]
	public float endTime;

	[NonSerialized]
	public float startTime;

	[NonSerialized]
	public float duration;

	[NonSerialized]
	public float startOfFadeOut;

	public OnStartPreviewSong onStartPreviewSong;

	public bool Playing => audioSource.isPlaying;

	private void Update()
	{
		if (!Playing || StartingLevel)
		{
			return;
		}
		float time = audioSource.time;
		if (time >= endTime - 0.04f)
		{
			audioSource.time = startTime;
			if (onStartPreviewSong != null)
			{
				onStartPreviewSong();
			}
		}
		if (UseFade)
		{
			if (time > startOfFadeOut)
			{
				float lifetimePercentage = Mathf.InverseLerp(startOfFadeOut, endTime, time);
				audioSource.volume = 1f - DOVirtual.EasedValue(0f, 1f, lifetimePercentage, Ease.InQuad);
			}
			else if (time < startTime + 1f)
			{
				float lifetimePercentage2 = Mathf.InverseLerp(startTime, startTime + 1f, time);
				audioSource.volume = DOVirtual.EasedValue(0f, 1f, lifetimePercentage2, Ease.OutQuad);
			}
			else
			{
				audioSource.volume = 1f;
			}
		}
		else
		{
			audioSource.volume = 1f;
		}
		audioSource.volume *= 0.2376f;
	}

	private void OnEnable()
	{
		if (audioSource == null)
		{
			audioSource = GetComponent<AudioSource>();
			audioSource.ignoreListenerPause = true;
		}
		Stop();
	}

	public void Stop(float fadeLength = 0f)
	{
		if (fadeLength == 0f)
		{
			audioSource.Stop();
			return;
		}
		audioSource.DOFade(0f, fadeLength).OnComplete(() =>
		{
			audioSource.Stop();
		});
	}

	public void Play(AudioClip newClip, float newStartTime, float newDuration, bool isCLS = false)
	{
		audioSource.Stop();
		clip = newClip;
		startTime = newStartTime;
		duration = newDuration;
		if (startTime > clip.length - 1f || startTime < 0f)
		{
			startTime = 0f;
		}
		duration = Mathf.Clamp(duration, 1f, 20f);
		if (startTime + duration > clip.length)
		{
			duration = clip.length - startTime;
		}
		endTime = startTime + duration;
		startOfFadeOut = 0f;
		UseFade = false;
		if (duration >= 4f)
		{
			UseFade = true;
			startOfFadeOut = endTime - 1f;
		}
		audioSource.volume = (UseFade ? 0f : 0.2376f);
		audioSource.clip = clip;
		audioSource.time = startTime;
		audioSource.outputAudioMixerGroup = (isCLS ? RDUtils.GetMixerGroup("CLSPreviewMusic") : RDUtils.GetMixerGroup("MenuMusic"));
		audioSource.Play();
		if (onStartPreviewSong != null)
		{
			onStartPreviewSong();
		}
	}

	public void SeekToEnd(float offset)
	{
		audioSource.time = Mathf.Max(startTime, endTime + offset);
	}
}
