using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class RDAmbienceAudioSource : RDBase
{
	public enum AmbienceMode
	{
		SimpleLoop = 0,
		DynamicShots = 1,
		OnAnimation = 2
	}

	[Header("Design Space")]
	public List<AudioClip> sounds = new List<AudioClip>();

	[Range(0f, 1f)]
	public float volume;

	[Tooltip("176 is enough to span a whole screen. 264 lets you hear it from half a screen width offscreen.")]
	[Range(22f, 1416f)]
	public float minDistance;

	[Tooltip("176 is enough to span a whole screen. 264 lets you hear it from half a screen width offscreen.")]
	[Range(22f, 1416f)]
	public float maxDistance;

	[Tooltip("Stereo width depending on distance. 180 is full stereo, 0 is mono.")]
	[Range(0f, 180f)]
	public float spread;

	[Tooltip("Don't play sounds when in cutscenes or post-level dialogue.")]
	public bool disableInCutscenes;

	[Tooltip("SimpleLoop: Loop the same sound without end.\n\nDynamicShots: Randomly play a sound from the pool once in a while.\n\nOnAnimation: Drag an animated object into the slot below and fill in the correct frame number for syncing to picture.")]
	public AmbienceMode mode;

	[Tooltip("Only used in DynamicShots mode.")]
	[Range(1f, 60f)]
	public float minInterval;

	[Tooltip("Only used in DynamicShots mode.")]
	[Range(1f, 60f)]
	public float maxInterval;

	[Tooltip("Only used in OnAnimation mode.")]
	public tk2dSpriteAnimator animator;

	[Tooltip("Only used in OnAnimation mode.")]
	public int onFrame;

	[Header("Prefab References")]
	public AudioSource amb;

	public TextMesh debugText;

	public SpriteRenderer debugIcon;

	public SpriteRenderer debugMinCircle;

	public SpriteRenderer debugMaxCircle;

	public Sprite activeSpeaker;

	public Sprite inactiveSpeaker;

	[NonSerialized]
	public float fadeInDuration = 0.2f;

	private const int debugCircleDiameter = 352;

	private const float defaultFadeInDuration = 0.2f;

	private bool inRange;

	private bool isStopped;

	private float listenerPositionZ;

	private int previousClipIndex;

	private AudioListener listener;

	private bool inScnGame;

	private float planeDistanceToListener => (listener.transform.position.WithZ(0f) - base.transform.position).magnitude;

	private void Awake()
	{
		amb.volume = 0f;
		amb.spread = spread;
		if (amb.clip == null)
		{
			amb.clip = sounds[0];
		}
		AudioSource audioSource = amb;
		bool loop = mode == AmbienceMode.SimpleLoop;
		audioSource.loop = loop;
		if (mode == AmbienceMode.OnAnimation && animator == null)
		{
			RDBaseDllDummy.printesw("ambience source " + base.name + " is controlled by animation, but missing reference to an animator!!!");
			base.gameObject.SetActive(value: false);
		}
	}

	private void Start()
	{
		if ((bool)scnLevelSelect.instance)
		{
			listener = scnLevelSelect.instance.listener;
			inScnGame = false;
		}
		else
		{
			if (!scnGame.instance)
			{
				return;
			}
			listener = base.game.listener;
			inScnGame = true;
		}
		listenerPositionZ = listener.transform.position.z;
		amb.minDistance = Mathf.Sqrt(minDistance * minDistance + listenerPositionZ * listenerPositionZ);
		amb.maxDistance = Mathf.Sqrt(maxDistance * maxDistance + listenerPositionZ * listenerPositionZ);
	}

	public void Restart()
	{
		inRange = false;
	}

	private void Update()
	{
		if (disableInCutscenes && (inScnGame || scnLevelSelect.instance.ink.isPlaying))
		{
			return;
		}
		if (!inRange)
		{
			if (!(planeDistanceToListener <= maxDistance))
			{
				return;
			}
			inRange = true;
			Play();
		}
		if (inRange && planeDistanceToListener > maxDistance)
		{
			inRange = false;
			Pause();
		}
		amb.spread = spread;
		amb.minDistance = Mathf.Sqrt(minDistance * minDistance + listenerPositionZ * listenerPositionZ);
		amb.maxDistance = Mathf.Sqrt(maxDistance * maxDistance + listenerPositionZ * listenerPositionZ);
	}

	private void LateUpdate()
	{
		if (RDBase.debugSettings.DebugAmbience)
		{
			debugIcon.sprite = (amb.isPlaying ? activeSpeaker : inactiveSpeaker);
			debugMaxCircle.transform.ScaleX(2f * maxDistance / 352f);
			debugText.text = $"{minDistance:0.0} - {maxDistance:0.0}\n{planeDistanceToListener:0.0}";
			if (inRange)
			{
				debugMinCircle.gameObject.SetActive(value: true);
				debugMinCircle.transform.ScaleX(2f * minDistance / 352f);
			}
			else
			{
				debugMinCircle.gameObject.SetActive(value: false);
			}
		}
	}

	private void Play()
	{
		amb.DOKill();
		if (isStopped)
		{
			amb.DOFade(volume, fadeInDuration);
			isStopped = false;
			fadeInDuration = 0.2f;
		}
		else
		{
			amb.volume = volume;
		}
		switch (mode)
		{
		case AmbienceMode.SimpleLoop:
			PrepareSimpleLoop();
			amb.UnPause();
			break;
		case AmbienceMode.DynamicShots:
			PrepareDynamicShot();
			break;
		case AmbienceMode.OnAnimation:
			PrepareOnAnimationSound();
			break;
		}
	}

	private void Pause()
	{
		amb.DOKill();
		amb.Pause();
		StopAllCoroutines();
	}

	public void FadeOut(float duration = 1f)
	{
		if (isStopped)
		{
			return;
		}
		amb.DOKill();
		if (inRange)
		{
			StopAllCoroutines();
			amb.DOFade(0f, duration).OnComplete(() =>
			{
				amb.Stop();
			});
		}
		else
		{
			amb.Stop();
		}
		isStopped = true;
	}

	private void PrepareSimpleLoop()
	{
		amb.timeSamples = RXRandom.Range(0, amb.clip.samples);
		amb.Play();
		amb.Pause();
	}

	private void PrepareDynamicShot()
	{
		LoadRandomClip();
		float num = RXRandom.Range(minInterval, maxInterval);
		StartCoroutine("WaitToPlayShotCo", num);
	}

	private IEnumerator WaitToPlayShotCo(float interval)
	{
		yield return new WaitForSeconds(interval);
		amb.Play();
		yield return new WaitUntil(() => !amb.isPlaying);
		PrepareDynamicShot();
	}

	private void PrepareOnAnimationSound()
	{
		LoadRandomClip();
		StartCoroutine("WaitForAnimationCo");
	}

	private IEnumerator WaitForAnimationCo()
	{
		yield return new WaitUntil(() => animator.CurrentFrame == onFrame);
		amb.Play();
		yield return new WaitUntil(() => !amb.isPlaying);
		PrepareOnAnimationSound();
	}

	private void LoadRandomClip()
	{
		if (sounds.Count > 1)
		{
			int num;
			for (num = RXRandom.Int(sounds.Count); num == previousClipIndex; num = RXRandom.Int(sounds.Count))
			{
			}
			amb.clip = sounds[num];
			previousClipIndex = num;
		}
	}
}
