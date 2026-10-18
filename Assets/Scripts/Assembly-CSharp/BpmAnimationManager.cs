using System;
using UnityEngine;

[RequireComponent(typeof(PreviewSongPlayer))]
public class BpmAnimationManager : RDBase
{
	public delegate void OnBeat(float timeToNextBeat);

	private const float MinBpm = 70f;

	private const float MaxBpm = 200f;

	private const float DefaultBpm = 100f;

	public static BpmAnimationManager instance;

	public static float deltaDspTime;

	public OnBeat onBeat;

	private PreviewSongPlayer previewSongPlayer;

	private double lastDspTime;

	private float currentTime;

	private float timeToNextBeat;

	private float bpm;

	private float offset;

	private bool hasWaitedOffsetTime;

	private void Awake()
	{
		instance = this;
		onBeat = (float timeToNextBeat) =>
		{
		};
		previewSongPlayer = GetComponent<PreviewSongPlayer>();
		PreviewSongPlayer obj = previewSongPlayer;
		obj.onStartPreviewSong = (PreviewSongPlayer.OnStartPreviewSong)Delegate.Combine(obj.onStartPreviewSong, new PreviewSongPlayer.OnStartPreviewSong(OnStartPreviewSong));
		bpm = 100f;
	}

	private void Start()
	{
		currentTime = 0f;
		lastDspTime = AudioSettings.dspTime;
	}

	private void Update()
	{
		if (bpm < 0f)
		{
			return;
		}
		deltaDspTime = (float)(AudioSettings.dspTime - lastDspTime);
		currentTime += deltaDspTime;
		lastDspTime = AudioSettings.dspTime;
		if (hasWaitedOffsetTime)
		{
			if (currentTime >= timeToNextBeat)
			{
				currentTime -= timeToNextBeat;
				timeToNextBeat = 60f / bpm;
				onBeat(timeToNextBeat);
			}
		}
		else if (currentTime >= offset)
		{
			hasWaitedOffsetTime = true;
			currentTime -= offset;
			timeToNextBeat = 60f / bpm;
			onBeat(timeToNextBeat);
		}
	}

	private void OnStartPreviewSong()
	{
		hasWaitedOffsetTime = false;
		currentTime = 0f;
		onBeat(0f);
	}

	public void SetBpm(float newBpm, float newOffset = 0f)
	{
		bpm = ((newBpm <= 0f) ? 100f : newBpm);
		do
		{
			if (bpm > 200f)
			{
				bpm /= 2f;
			}
			else if (bpm < 70f)
			{
				bpm *= 2f;
			}
		}
		while (bpm < 70f || bpm > 200f);
		newOffset /= 1000f;
		float num = newOffset + RDCalibration.calibration_v;
		for (float num2 = 60f / bpm; num < previewSongPlayer.startTime; num += num2)
		{
		}
		offset = num - previewSongPlayer.startTime;
		currentTime = 0f;
	}
}
