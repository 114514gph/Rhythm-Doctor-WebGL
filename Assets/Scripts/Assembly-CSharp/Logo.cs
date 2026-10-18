using System;
using UnityEngine;
using UnityEngine.UI;

public class Logo : RDBase
{
	public enum PlaybackPreset
	{
		Rhythm = 0,
		Doctor = 1,
		Monitor = 2,
		RhythmDoctorLoop = 3
	}

	[Serializable]
	public class Animatable
	{
		public Image image;

		public Sprite[] sprites;

		public bool loop;

		public float speed;

		public float time;

		public bool playing;

		public void Play(bool rewind = false)
		{
			if (rewind)
			{
				time = 0f;
			}
			playing = true;
		}

		public void PlayPause()
		{
			playing = !playing;
		}

		public void Update()
		{
			if (playing)
			{
				image.enabled = true;
				time += Time.unscaledDeltaTime;
				int num = Mathf.FloorToInt(time * speed);
				if (num >= sprites.Length)
				{
					num = (loop ? (num % sprites.Length) : (sprites.Length - 1));
				}
				image.sprite = sprites[num];
			}
		}
	}

	public bool playAtStart = true;

	public Animatable monitor;

	public Animatable rhythm;

	public Animatable rhythmChinese;

	public Animatable doctor;

	public Animatable doctorChinese;

	private Animatable currentRhythm;

	private Animatable currentDoctor;

	[NonSerialized]
	public bool playSounds = true;

	private int initialFrame = -1;

	public void Play(PlaybackPreset preset)
	{
		switch (preset)
		{
		}
	}

	private void Awake()
	{
		if (playAtStart)
		{
			Rewind();
		}
	}

	public void Rewind()
	{
		initialFrame = Time.frameCount;
		monitor.image.enabled = false;
		rhythm.image.enabled = false;
		doctor.image.enabled = false;
		rhythmChinese.image.enabled = false;
		doctorChinese.image.enabled = false;
		currentRhythm = (RDString.isChinese ? rhythmChinese : rhythm);
		currentDoctor = (RDString.isChinese ? doctorChinese : doctor);
	}

	private void Update()
	{
		if (initialFrame == -1)
		{
			return;
		}
		monitor.Update();
		currentRhythm.Update();
		currentDoctor.Update();
		int num = initialFrame + 10;
		if (Time.frameCount == num + 1)
		{
			monitor.Play();
		}
		if (Time.frameCount == num + 16)
		{
			if (playSounds)
			{
				scrConductor.PlayImmediately("sndScribble", 1f, RDUtils.GetMixerGroup("MainMenu"));
			}
			currentRhythm.Play();
			currentDoctor.Play();
		}
	}
}
