using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;

public class scrConductor : RDBase
{
	private static scrConductor _instance;

	public const float GlobalExternalSoundVolume = 0.4f;

	public const float EARLY_MISTAKE_PITCH = 1f;

	public const float EARLY_MISTAKE_VOLUME = 1f;

	public const float LATE_MISTAKE_PITCH = 0.8f;

	public const float LATE_MISTAKE_VOLUME = 1.25f;

	private const int AudioSamples = 1024;

	private Tween volumeTween;

	private float _volume;

	private float _pitch;

	private float _pan;

	[NonSerialized]
	public int barNumberUpdateOnBar;

	[NonSerialized]
	public int lastBarScrubbed = -1;

	[NonSerialized]
	public int barNumber;

	[NonSerialized]
	public int barNumberMonotonic;

	[NonSerialized]
	public int barNumberToGo;

	[NonSerialized]
	public bool hassongstarted;

	[NonSerialized]
	public double startOfNextBar;

	[NonSerialized]
	public double startOfNextBarRefreshOnlyOnBar;

	[NonSerialized]
	public float crotchetRefreshOnlyOnBar;

	[NonSerialized]
	public double startOfLastPrebar = double.PositiveInfinity;

	[NonSerialized]
	public double startOfSong;

	[NonSerialized]
	public double songHeaderLengthDSpeedAdjusted;

	[NonSerialized]
	public float songHeaderLengthPrebar;

	[NonSerialized]
	public int startOfSongBarNumber;

	[NonSerialized]
	public double startOfLastBeatVisual;

	[NonSerialized]
	public double startOfLastBeatInput;

	[NonSerialized]
	public string currentSongString;

	[NonSerialized]
	public float actualPreBeatNumber;

	[NonSerialized]
	public double startOfLastPreActualBeat = double.PositiveInfinity;

	[NonSerialized]
	public bool shouldLoop;

	[NonSerialized]
	public bool hasLooped;

	[NonSerialized]
	public bool betweenPrebarAndBar;

	[NonSerialized]
	public bool dontPrebar;

	[NonSerialized]
	public float visualBeatLengthMultiplier = 1f;

	[NonSerialized]
	public int lastOnBeatFrame;

	public bool debug;

	public bool scrubTweens;

	private bool shouldSongBeLooping;

	public double lastScheduledTime;

	public int scrubDestination;

	private string currentLoopDestinationString;

	private AudioMixerGroup currentLoopDestinationGroup;

	public double currentSongStartTime;

	public AudioSource currentSong;

	public AudioSource previousSong;

	public bool oneSongAtATime;

	private float[] _spectrum = new float[1024];

	[NonSerialized]
	public bool getSpectrum;

	[NonSerialized]
	public List<AudioSource> audiosourcesFromPlayBeat;

	[NonSerialized]
	public List<AudioSource> audiosourcesFromPlayBeatPrevBar;

	[NonSerialized]
	public Dictionary<AudioSource, double> audioStartTimesFromPlayBeat;

	[NonSerialized]
	public Dictionary<AudioSource, double> audioEndTimesFromPlayBeat;

	private bool songStarted;

	private float[] mSamples = new float[1024];

	[NonSerialized]
	public float mainVolume;

	public List<BPMChange> bpmChangesInBar;

	public List<BPMChange> bpmChangesInBarRefreshOnBar;

	[NonSerialized]
	public float barLength;

	[NonSerialized]
	public float barLengthRefreshOnBar;

	public Dictionary<string, SongOffset> externalSoundData = new Dictionary<string, SongOffset>();

	private int _crotchetsperbar;

	private int crotchetsPerBarOnBar;

	[NonSerialized]
	public List<RDExecuteOn> executes = new List<RDExecuteOn>();

	private double audioPosCache;

	private int audioPosCachedFrame;

	public static float GlobalPanningStrength;

	public static float GlobalPanningInvert;

	public static float BeatsoundPitchVarianceMultiplier;

	public static bool BeatsoundPitchMode;

	private float savedSongVolume;

	private bool muted;

	[NonSerialized]
	public PlayStyle playStyle;

	[NonSerialized]
	public PlayStyle playStyleCache;

	[NonSerialized]
	public PlayStyleChange unfreezeBehaviour;

	[NonSerialized]
	public int playStyleTempCounter;

	private int loopBetweenStart;

	private int loopBetweenEnd;

	public bool stayOnBar;

	private bool FakeSongFunctionPlayed;

	private bool forceNoSync;

	public bool runningScrubToNextBar;

	private bool justPlayedSongInThisBar;

	private bool _isScrubbing;

	public bool isScrubbingRefreshOnBar;

	private static float smallMistakeTimerP1;

	private static float bigMistakeTimerP1;

	private static float smallMistakeTimerP2;

	private static float bigMistakeTimerP2;

	public float __TimeInBar;

	public float __BeatInBar;

	public double __visualPos;

	public float __barLengthInSeconds;

	public float __progressPct;

	private bool hasCalculatedBPMChangeTimesOnBar;

	[NonSerialized]
	public float _beatofLastBPMChange;

	[NonSerialized]
	public float _timeofLastBPMChange;

	public Dictionary<AudioSource, double> explosionAudioSources;

	public static scrConductor instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = UnityEngine.Object.FindObjectOfType<scrConductor>();
			}
			return _instance;
		}
	}

	public float volume
	{
		get
		{
			return _volume;
		}
		set
		{
			_volume = value;
			if (IsSongPlaying())
			{
				currentSong.volume = value * GlobalSongVolume;
			}
		}
	}

	public float pitch
	{
		get
		{
			return _pitch;
		}
		set
		{
			_pitch = value;
			if (IsSongPlaying())
			{
				currentSong.pitch = value;
			}
		}
	}

	public float pan
	{
		get
		{
			return _pan;
		}
		set
		{
			_pan = value;
			if (IsSongPlaying())
			{
				currentSong.panStereo = value;
			}
		}
	}

	public static RDSongOffsets songOffsets => RDSongOffsets.instance;

	public float bpm { get; private set; } = 100f;

	public static float latency
	{
		get
		{
			return RDCalibration.latency;
		}
		set
		{
			RDCalibration.latency = value;
		}
	}

	public static float calibration_v
	{
		get
		{
			return RDCalibration.calibration_v;
		}
		set
		{
			RDCalibration.calibration_v = value;
		}
	}

	public static float calibration_i
	{
		get
		{
			return RDCalibration.calibration_i;
		}
		set
		{
			RDCalibration.calibration_i = value;
		}
	}

	public static float calibration_i_P2_internal
	{
		get
		{
			return RDCalibration.calibration_i_P2_internal;
		}
		set
		{
			RDCalibration.calibration_i_P2_internal = value;
		}
	}

	public float crotchet => 60f / bpm;

	public float unscaledCrotchet => crotchet * RDTime.speed;

	public float visualCrotchet => 60f / bpm * ((visualBeatLengthMultiplier > 0f) ? visualBeatLengthMultiplier : 1f);

	public double audioPos
	{
		get
		{
			if (Time.frameCount != audioPosCachedFrame)
			{
				audioPosCache = AudioSettings.dspTime;
				audioPosCachedFrame = Time.frameCount;
			}
			return audioPosCache;
		}
	}

	public double visualPos => AudioSettings.dspTime - (double)calibration_v;

	public double inputPos_p1 => AudioSettings.dspTime - (double)calibration_i;

	public static float GlobalSongVolume => songOffsets.GlobalSongVolumeDebug * GC.VolSongs;

	public static float GlobalBeatsoundVolume => songOffsets.GlobalBeatsoundVolumeDebug * GC.VolBeatsounds;

	public static float GlobalJyiSoundVolume => songOffsets.GlobalJyiSoundVolumeDebug * GC.VolBeatsounds;

	public static float GlobalHandPopSoundVolume => songOffsets.GlobalHandPopSoundVolumeDebug * GC.VolBeatsounds;

	public static float GlobalOtherSoundVolume => songOffsets.GlobalNonBeatSoundVolumeDebug * GC.VolMenuSounds;

	public bool onBeat => lastOnBeatFrame == Time.frameCount;

	private string info => $"audioPos: {audioPos}, solpab: {startOfLastPreActualBeat}, solp: {startOfLastPrebar}, crotchet: {crotchet}, barLength: {barLength}";

	public int crotchetsPerBar
	{
		get
		{
			return _crotchetsperbar;
		}
		set
		{
			if (latency > (float)value * crotchet)
			{
				latency = (float)value * crotchet - 0.01f;
				_crotchetsperbar = value;
				Debug.LogWarning("Latency altered to be below default, may be unstable.");
			}
			else
			{
				_crotchetsperbar = value;
				refreshBarLength();
			}
		}
	}

	public float songFileTimeVisual => (float)(AudioSettings.dspTime - (double)calibration_v - startOfSong - songHeaderLengthDSpeedAdjusted - (double)songHeaderLengthPrebar);

	public float[] spectrum
	{
		get
		{
			getSpectrum = true;
			return _spectrum;
		}
	}

	private bool shouldPlayStyleAdvanceBar
	{
		get
		{
			PlayStyle playStyle = this.playStyle;
			if (playStyle == PlayStyle.Normal || playStyle == PlayStyle.LoopBetween)
			{
				return !stayOnBar;
			}
			return false;
		}
	}

	private bool shouldPlayStylePlayPrebar
	{
		get
		{
			if (playStyle != PlayStyle.Prolong)
			{
				return !stayOnBar;
			}
			return false;
		}
	}

	public bool isScrubbing
	{
		get
		{
			return _isScrubbing;
		}
		set
		{
			_isScrubbing = value;
			if (value)
			{
				isScrubbingRefreshOnBar = value;
			}
		}
	}

	public float progressOnCurrentBar
	{
		get
		{
			float num = TimeToBeatRefreshOnBar(visualPos - startOfNextBarRefreshOnlyOnBar) / (float)crotchetsPerBarOnBar;
			__TimeInBar = (float)(visualPos - startOfNextBarRefreshOnlyOnBar);
			__BeatInBar = TimeToBeatRefreshOnBar(visualPos - startOfNextBarRefreshOnlyOnBar);
			__visualPos = visualPos;
			__barLengthInSeconds = barLengthRefreshOnBar;
			__progressPct = num;
			return num;
		}
	}

	public float currentBeat => TimeToBeatRefreshOnBar(visualPos - startOfNextBarRefreshOnlyOnBar);

	private void Awake()
	{
		bpmChangesInBar = new List<BPMChange>();
		bpmChangesInBarRefreshOnBar = new List<BPMChange>();
		RDCalibration.Setup();
		crotchetsPerBar = 8;
		crotchetsPerBarOnBar = 8;
		barLength = 99999f;
		smallMistakeTimerP1 = 0f;
		bigMistakeTimerP1 = 0f;
		smallMistakeTimerP2 = 0f;
		bigMistakeTimerP2 = 0f;
		LoadSavedAudioSettings();
		if (base.editorMode)
		{
			GetComponent<AudioListener>().enabled = false;
		}
	}

	public static void LoadSavedAudioSettings()
	{
		GlobalPanningStrength = (float)Persistence.GetPanningStrength() * 0.01f;
		GlobalPanningInvert = (Persistence.GetPanningInvert() ? (-1f) : 1f);
		BeatsoundPitchVarianceMultiplier = 1f - (float)Persistence.GetBeatsoundPitchVariance() * 0.01f;
		BeatsoundPitchMode = Persistence.GetBeatsoundPitchMode();
		RDInput.SetPlayerSoundPanning();
	}

	private void Start()
	{
		playStyle = PlayStyle.Normal;
		songHeaderLengthDSpeedAdjusted = 0.0;
		audiosourcesFromPlayBeat = new List<AudioSource>();
		audiosourcesFromPlayBeatPrevBar = new List<AudioSource>();
		audioStartTimesFromPlayBeat = new Dictionary<AudioSource, double>();
		audioEndTimesFromPlayBeat = new Dictionary<AudioSource, double>();
		explosionAudioSources = new Dictionary<AudioSource, double>();
	}

	public void ToggleMuteCurrentSong()
	{
		if (!muted)
		{
			MuteCurrentSong();
		}
		else
		{
			UnmuteCurrentSong();
		}
	}

	public void MuteCurrentSong(bool justSoft = false)
	{
		if (!muted)
		{
			savedSongVolume = volume;
			volume = (justSoft ? 0.05f : 0f);
			muted = true;
		}
	}

	public void UnmuteCurrentSong()
	{
		volume = savedSongVolume;
		muted = false;
	}

	public void Pause(bool isPause)
	{
		AudioListener.pause = isPause;
	}

	public void Freeze(PlayStyleChange unfreeze)
	{
		SetPlayStyle(unfreeze);
	}

	public void FreezeLoopBetween(int loopStart, int loopEnd)
	{
		if (!base.conductor.isScrubbing)
		{
			playStyleCache = PlayStyle.LoopBetween;
			unfreezeBehaviour = PlayStyleChange.OnNextBar;
			loopBetweenStart = loopStart;
			loopBetweenEnd = loopEnd;
		}
	}

	public PSMovement PSChangeToPSMovement(PlayStyleChange unfreezeBehaviour)
	{
		if ((uint)(unfreezeBehaviour - 1) <= 1u)
		{
			return PSMovement.Immediately;
		}
		return PSMovement.OnNextBar;
	}

	public PlayStyle PSChangeToPlayStyle(PlayStyleChange unfreezeBehaviour)
	{
		return unfreezeBehaviour switch
		{
			PlayStyleChange.BeatLoopOnly => PlayStyle.BeatLoopOnly, 
			PlayStyleChange.Loop => PlayStyle.Loop, 
			PlayStyleChange.Prolong => PlayStyle.Prolong, 
			PlayStyleChange.ProlongOneBar => PlayStyle.ProlongOneBar, 
			_ => PlayStyle.Normal, 
		};
	}

	public void SetPlayStyle(PlayStyleChange changeStyle, bool forceFix = false)
	{
		if (changeStyle == PlayStyleChange.Normal)
		{
			changeStyle = PlayStyleChange.OnNextBar;
		}
		if (base.conductor.isScrubbingRefreshOnBar)
		{
			return;
		}
		switch (changeStyle)
		{
		case PlayStyleChange.None:
			return;
		default:
			unfreezeBehaviour = changeStyle;
			break;
		case PlayStyleChange.Default:
			break;
		}
		PlayStyle playStyle = PSChangeToPlayStyle(unfreezeBehaviour);
		PSMovement pSMovement = PSChangeToPSMovement(unfreezeBehaviour);
		bool flag = false;
		if (((playStyle != this.playStyle) | forceFix) && betweenPrebarAndBar)
		{
			flag = true;
		}
		if (flag)
		{
			if (base.game != null)
			{
				base.game.StopBeats(killHalfwayPlayingSounds: false, barNumberMonotonic);
			}
			barNumber = barNumberUpdateOnBar;
		}
		switch (unfreezeBehaviour)
		{
		case PlayStyleChange.Immediately:
			if (betweenPrebarAndBar)
			{
				playStyleCache = PlayStyle.Normal;
				pSMovement = PSMovement.OnNextBar;
				break;
			}
			playStyleCache = PlayStyle.Normal;
			this.playStyle = PlayStyle.Normal;
			startOfLastPrebar = audioPos;
			startOfNextBar = audioPos + (double)latency;
			startOfLastBeatVisual = startOfNextBar + (double)calibration_v;
			forceNoSync = true;
			if (!base.conductor.isScrubbing)
			{
				OnPreBar();
			}
			break;
		case PlayStyleChange.ExtraImmediately:
			if (betweenPrebarAndBar)
			{
				playStyleCache = PlayStyle.Normal;
				pSMovement = PSMovement.OnNextBar;
				break;
			}
			playStyleCache = PlayStyle.Normal;
			this.playStyle = PlayStyle.Normal;
			startOfLastPrebar = audioPos - (double)latency;
			startOfNextBar = audioPos;
			startOfLastBeatVisual = startOfNextBar + (double)calibration_v;
			forceNoSync = true;
			if (!base.conductor.isScrubbing)
			{
				OnPreBar();
			}
			OnNewBar();
			betweenPrebarAndBar = false;
			startOfLastBeatVisual = startOfLastPrebar + (double)latency + (double)calibration_v;
			startOfNextBarRefreshOnlyOnBar = startOfNextBar;
			crotchetRefreshOnlyOnBar = crotchet;
			if (bpmChangesInBar.Count != 0)
			{
				PrepareOnBeatEvents();
			}
			if (bpmChangesInBar.Count == 0)
			{
				OnBeatCond();
			}
			break;
		case PlayStyleChange.Prolong:
			playStyleCache = PlayStyle.Prolong;
			break;
		case PlayStyleChange.OnNextBar:
			playStyleCache = PlayStyle.Normal;
			break;
		case PlayStyleChange.Loop:
			playStyleCache = PlayStyle.Loop;
			break;
		case PlayStyleChange.BeatLoopOnly:
			playStyleCache = PlayStyle.BeatLoopOnly;
			if (base.conductor.isScrubbing)
			{
				playStyleCache = PlayStyle.Normal;
			}
			break;
		case PlayStyleChange.ScrubToNext:
			playStyleCache = PlayStyle.Normal;
			this.playStyle = PlayStyle.Normal;
			if (!base.conductor.isScrubbing)
			{
				ScrubToNextBar(finishHere: true, calledByScrub: false, scrubSong: false);
			}
			break;
		case PlayStyleChange.ProlongOneBar:
			this.playStyle = PlayStyle.Prolong;
			playStyleCache = PlayStyle.Normal;
			playStyleTempCounter = 1;
			break;
		default:
			throw new Exception("undefined PlayStyleChange");
		}
		if (flag && pSMovement != PSMovement.Immediately)
		{
			this.playStyle = playStyle;
			if (playStyle != PlayStyle.Prolong && playStyle != PlayStyle.ProlongOneBar)
			{
				OnPreBar();
				return;
			}
			crotchetsPerBar = crotchetsPerBarOnBar;
			barLength = barLengthRefreshOnBar;
			startOfLastPrebar -= barLength;
			startOfNextBar -= barLength;
		}
	}

	private void Update()
	{
		if (base.game != null)
		{
			Beat[] array = base.game.beats.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Update();
			}
		}
		for (int j = 0; j < executes.Count; j++)
		{
			executes[j].Update();
		}
		if (!songStarted)
		{
			return;
		}
		while (audioPos - (double)crotchet > startOfLastPreActualBeat)
		{
			startOfLastPreActualBeat += crotchet;
			actualPreBeatNumber++;
		}
		if (audioPos - (double)barLength > startOfLastPrebar)
		{
			if (playStyleTempCounter > 0)
			{
				playStyleTempCounter--;
				if (playStyleTempCounter == 0)
				{
					playStyle = PlayStyle.Normal;
				}
			}
			else
			{
				playStyle = playStyleCache;
			}
			startOfLastPrebar += barLength;
			startOfNextBar += barLength;
			OnPreBar();
			string text = currentSongString;
			if (text != null && !text.Contains("*external"))
			{
				SongOffset songOffset = songOffsets.Get(currentSongString);
				if (actualPreBeatNumber > (float)songOffset.numberOfBeatsFromSongStart && songOffset.numberOfBeatsFromSongStart != 0 && shouldSongBeLooping)
				{
					ReplaySong();
				}
			}
		}
		if (visualPos > startOfLastPrebar + (double)latency && betweenPrebarAndBar)
		{
			OnNewBar();
			betweenPrebarAndBar = false;
			startOfLastBeatVisual = startOfLastPrebar + (double)latency + (double)calibration_v;
			startOfNextBarRefreshOnlyOnBar = startOfNextBar;
			isScrubbingRefreshOnBar = isScrubbing;
			crotchetRefreshOnlyOnBar = crotchet;
			if (bpmChangesInBar.Count != 0)
			{
				PrepareOnBeatEvents();
			}
			if (bpmChangesInBar.Count == 0)
			{
				OnBeatCond();
			}
		}
		if (bpmChangesInBar.Count == 0 && audioPos - startOfLastBeatVisual > (double)(crotchet * visualBeatLengthMultiplier))
		{
			startOfLastBeatVisual += crotchet * visualBeatLengthMultiplier;
			OnBeatCond();
		}
		CalculateVolume();
		bigMistakeTimerP1 = Mathf.Max(bigMistakeTimerP1 - Time.deltaTime, 0f);
		smallMistakeTimerP1 = Mathf.Max(smallMistakeTimerP1 - Time.deltaTime, 0f);
		bigMistakeTimerP2 = Mathf.Max(bigMistakeTimerP2 - Time.deltaTime, 0f);
		smallMistakeTimerP2 = Mathf.Max(smallMistakeTimerP2 - Time.deltaTime, 0f);
	}

	private void LateUpdate()
	{
		if (base.game != null)
		{
			for (int i = 0; i < base.game.beats.Count; i++)
			{
				base.game.beats[i].LateUpdate();
			}
		}
		foreach (RDExecuteOn execute in executes)
		{
			execute.LateUpdate();
		}
		_ = executes.Count;
		executes.RemoveAll((RDExecuteOn e) => e.removed);
	}

	private void PrepareOnBeatEvents()
	{
		for (float num = 0f; num < (float)crotchetsPerBar; num += visualBeatLengthMultiplier)
		{
			scrExecuteOnCertainBeat.Add(OnBeatCond, num);
		}
	}

	private void CalculateVolume()
	{
		AudioListener.GetOutputData(mSamples, 0);
		float num = 0f;
		for (int i = 0; i < 1024; i++)
		{
			num += mSamples[i] * mSamples[i];
		}
		mainVolume = ((AudioListener.volume == 0f) ? 0f : (Mathf.Sqrt(num / 1024f) / AudioListener.volume));
	}

	private void OnBeatCond()
	{
		lastOnBeatFrame = Time.frameCount;
		if (base.gm != null && base.gm.invisibleEntityPool != null)
		{
			foreach (InvisibleEntity item in base.gm.invisibleEntityPool)
			{
				if (!(item == null))
				{
					item.beatCounter++;
					if (item.beatCounter % item.customBeatMultiplier == 0)
					{
						item.OnBeat();
					}
				}
			}
		}
		foreach (RDExecuteOn execute in executes)
		{
			execute.OnBeat();
		}
		base.scnCurrent?.OnBeat();
	}

	public void OnPreBar()
	{
		barNumberMonotonic++;
		if (audiosourcesFromPlayBeat != null)
		{
			audiosourcesFromPlayBeatPrevBar = new List<AudioSource>(audiosourcesFromPlayBeat);
		}
		if (barNumber == 1)
		{
			startOfLastPrebar = audioPos;
		}
		justPlayedSongInThisBar = false;
		if (bpmChangesInBar.Count != 0)
		{
			SetBPM(GetLastBPMChange(), adjustForSpeed: false);
		}
		bpmChangesInBar = new List<BPMChange>();
		refreshBarLength();
		if (playStyle == PlayStyle.LoopBetween && barNumber == loopBetweenEnd)
		{
			barNumberToGo = loopBetweenStart;
		}
		if (shouldPlayStyleAdvanceBar)
		{
			OnPreBarIfAdvancing();
		}
		if (shouldPlayStylePlayPrebar)
		{
			base.scnCurrent?.OnPreBar();
			betweenPrebarAndBar = true;
		}
	}

	public void OnPreBarIfAdvancing()
	{
		if (barNumberToGo != 0)
		{
			barNumber = barNumberToGo;
		}
		else
		{
			barNumber++;
		}
		barNumberToGo = 0;
	}

	public void OnNewBar()
	{
		hasCalculatedBPMChangeTimesOnBar = false;
		barNumberUpdateOnBar = barNumber;
		crotchetsPerBarOnBar = crotchetsPerBar;
		RefreshBPMChangesOnBar();
		hasCalculatedBPMChangeTimesOnBar = true;
		barLengthRefreshOnBar = barLength;
		base.scnCurrent?.OnNewBar();
	}

	public void StopSong(float fadeDuration = 0f)
	{
		if (currentSong != null)
		{
			FadeSong(0f, fadeDuration).OnComplete(() =>
			{
				shouldLoop = false;
				currentSong.Stop();
			}).Done();
		}
	}

	public Tween FadeSong(float to, float fadeDuration)
	{
		volumeTween?.Kill();
		return volumeTween = DOTween.To(() => volume, (float x) =>
		{
			volume = x;
		}, to, fadeDuration);
	}

	public bool IsSongPlaying()
	{
		if ((bool)currentSong)
		{
			return currentSong.isPlaying;
		}
		return false;
	}

	public bool MustWeKeepSync()
	{
		if (!IsSongPlaying())
		{
			return FakeSongFunctionPlayed;
		}
		return true;
	}

	public void ScrubToNextBar(bool finishHere = true, bool calledByScrub = false, bool scrubSong = true)
	{
		runningScrubToNextBar = true;
		lastBarScrubbed = barNumber;
		if (betweenPrebarAndBar)
		{
			crotchetRefreshOnlyOnBar = crotchet;
		}
		if (currentSong != null)
		{
			double num = startOfSong - songHeaderLengthDSpeedAdjusted - (double)songHeaderLengthPrebar;
			float num2 = BeatToTime(crotchetsPerBar);
			double num3 = startOfLastPrebar + (double)num2;
			double num4 = (startOfLastPrebar - num + (double)num2) * (double)RDTime.speed;
			if (scrubSong)
			{
				AudioClip clip = currentSong.clip;
				if (clip != null && (float)num4 > clip.length)
				{
					num4 = clip.length - 0.0001f;
				}
				currentSong.SetScheduledStartTime(AudioSettings.dspTime);
				currentSong.time = (float)num4;
			}
			if (betweenPrebarAndBar)
			{
				OnNewBar();
			}
			if (base.ink.type == RDInkType.Active)
			{
				base.ink.Stop();
			}
			scrExecuteOnCertainBeat.MoveBackBy(num3 - audioPos, finishHere);
			if (base.game != null)
			{
				base.game.MoveBackTimedActions(num2);
				base.game.MoveBackBeats(num2);
			}
			startOfLastPrebar = audioPos;
			double num5 = audioPos - num4 / (double)RDTime.speed + songHeaderLengthDSpeedAdjusted + (double)songHeaderLengthPrebar;
			startOfSong = num5;
			startOfNextBar = audioPos + (double)latency;
			startOfLastBeatVisual = startOfNextBar + (double)calibration_v;
		}
		else
		{
			float num6 = BeatToTime(crotchetsPerBar);
			double num7 = startOfLastPrebar + (double)num6 - audioPos;
			if (betweenPrebarAndBar)
			{
				OnNewBar();
			}
			scrExecuteOnCertainBeat.MoveBackBy(num7, finishHere);
			if (base.game != null)
			{
				base.game.MoveBackTimedActions(num7);
				base.game.MoveBackBeats(num7);
			}
			startOfLastPrebar = audioPos;
			startOfNextBar = audioPos + (double)latency;
			startOfLastBeatVisual = startOfNextBar + (double)calibration_v;
		}
		RDRoom[] rooms = RDBase.Vfx.rooms;
		foreach (RDRoom rDRoom in rooms)
		{
			if (rDRoom.roomMovedByLevelEvent)
			{
				rDRoom.FinishScrubbing();
			}
		}
		if (finishHere)
		{
			base.game.RefreshRowPlayersScrub();
			base.game.RefreshBeatSkipAesthetics();
			base.game.RefreshSyncopationAesthetics();
			base.game.RepositionRowsAndStrips(instant: true);
			isScrubbing = false;
		}
		playStyle = PlayStyle.Normal;
		playStyleCache = PlayStyle.Normal;
		if (!(finishHere & calledByScrub))
		{
			OnPreBar();
		}
		runningScrubToNextBar = false;
	}

	public IEnumerator ScrubToBarNum(int barNum)
	{
		AudioListener.pause = true;
		scrubDestination = barNum;
		isScrubbing = true;
		int iteration = 0;
		while (barNumber < barNum)
		{
			if (barNumber > barNum)
			{
				throw new Exception("Cant scrub. We've already passed the checkpoint");
			}
			bool lastTime = barNumber == barNum - 1;
			ScrubToNextBar(lastTime, calledByScrub: true);
			if (playStyle != PlayStyle.Normal)
			{
				throw new Exception("Cant exit loop. Play style is stuck");
			}
			iteration++;
			if (iteration > 100000)
			{
				throw new Exception("More than 100k iterations. Something is wrong.");
			}
			if (justPlayedSongInThisBar)
			{
				yield return 0;
			}
			if (lastTime)
			{
				yield return 0;
				AudioListener.pause = false;
				OnPreBar();
			}
			base.game.RepositionRowsAndStrips(instant: true);
		}
	}

	public void ScrubToBarNumNoCoroutine(int barNum)
	{
		scrubDestination = barNum;
		isScrubbing = true;
		int num = 0;
		int num2 = barNumber;
		while (barNumber < barNum)
		{
			if (barNumber > barNum)
			{
				throw new Exception("Cant scrub. We've already passed the checkpoint");
			}
			if (playStyle != PlayStyle.Normal)
			{
				throw new Exception("Cant exit loop. Play style is stuck");
			}
			num++;
			if (num > 100000)
			{
				throw new Exception("More than 100k iterations. Something is wrong.");
			}
			ScrubToNextBar(num2 == barNum - 1);
			num2++;
			_ = justPlayedSongInThisBar;
			base.game.RepositionRowsAndStrips(instant: true);
		}
	}

	private AudioSource Play(string sound, double timeFSAB, bool isSong, float gain, AudioMixerGroup group = null, float pitch = 1f, float pan = 0f, float startTruncation = 0f)
	{
		if (sound == null)
		{
			return null;
		}
		if (pitch == 0f)
		{
			if (!isSong)
			{
				RDBaseDllDummy.printem("pitch for " + sound + " is zero, it will never end!");
				return null;
			}
			pitch = 0.005f;
		}
		int num = 0;
		bool flag = false;
		string text = "";
		float offset;
		if (externalSoundData.ContainsKey(sound))
		{
			SongOffset songOffset = externalSoundData[sound];
			offset = songOffset.offset;
			gain *= songOffset.volume;
			flag = !songOffset.isInternal;
		}
		else
		{
			SongOffset songOffset2 = songOffsets.Get(sound);
			offset = songOffset2.offset;
			gain *= songOffset2.volume;
			text = songOffset2.folder;
			if (songOffset2.prebarBeats != 0)
			{
				num = songOffset2.prebarBeats;
			}
		}
		AudioSource result = null;
		if (isSong)
		{
			if (timeFSAB != 0.0)
			{
				Debug.LogError("Time should be 0f when playing song! Use prebarbeats and prebartime if needed.");
			}
			if (crotchet > 99f)
			{
				Debug.LogError("NO BPM SET. NEED TO SET BPM BEFORE PLAYING SONG.");
			}
			offset /= pitch;
			songHeaderLengthDSpeedAdjusted = offset / RDTime.speed;
			songHeaderLengthPrebar = (float)num * crotchet;
			double num2;
			if (!MustWeKeepSync() || forceNoSync)
			{
				num2 = audioPos + (double)latency;
				forceNoSync = false;
			}
			else
			{
				num2 = startOfNextBar - songHeaderLengthDSpeedAdjusted;
				if (num != 0)
				{
					Debug.LogWarning("We shouldnt have prebarbeats when stitching songs together / looping.");
				}
			}
			startOfNextBar = (startOfSong = num2 + songHeaderLengthDSpeedAdjusted + (double)songHeaderLengthPrebar);
			startOfSongBarNumber = barNumber;
			startOfLastPrebar = startOfNextBar - (double)latency;
			startOfLastPreActualBeat = startOfNextBar - (double)latency;
			actualPreBeatNumber = 1f;
			startOfLastBeatVisual = startOfSong + (double)calibration_v;
			if (oneSongAtATime && currentSong != null)
			{
				currentSong.SetScheduledEndTime(num2);
			}
			pan = pan * GlobalPanningStrength * GlobalPanningInvert;
			if (flag)
			{
				result = AudioManager.Play(sound, num2, gain, group, pitch, pan, startTruncation);
				lastScheduledTime = num2;
			}
			else
			{
				result = AudioManager.Play(sound.HasAudioFileExtension() ? Path.GetFileNameWithoutExtension(sound) : (text + "/" + sound), num2, gain, group, pitch, pan);
				lastScheduledTime = num2;
			}
			currentSong = result;
			currentSong.priority = 0;
			volumeTween?.Kill();
			volume = gain;
			this.pitch = RDTime.speed;
			this.pan = pan;
			justPlayedSongInThisBar = true;
			songStarted = true;
			if (barNumber == 0)
			{
				OnPreBar();
			}
		}
		else if (RDBase.debugSettings.BeatSounds)
		{
			bool flag2 = sound.HasAudioFileExtension();
			string obj = ((flag | flag2) ? "" : (text + "/"));
			if (flag2)
			{
				sound = Path.GetFileNameWithoutExtension(sound);
			}
			offset /= pitch;
			pan = pan * GlobalPanningStrength * GlobalPanningInvert;
			double time = startOfNextBar + timeFSAB - (double)offset;
			result = AudioManager.Play(obj + sound, time, gain, group, pitch, pan, startTruncation);
			lastScheduledTime = time;
		}
		return result;
	}

	public AudioSource PlaySong(string song, AudioMixerGroup group = null, float pitch = 1f, float pan = 0f, bool useBeatsNotBar = false, float volume = 1f)
	{
		currentSongString = song;
		shouldSongBeLooping = true;
		if (!SoundIsExternal(song))
		{
			currentLoopDestinationString = songOffsets.Get(song).loopDestination;
			if (currentLoopDestinationString == "")
			{
				currentLoopDestinationString = songOffsets.Get(song).name;
			}
			currentLoopDestinationGroup = group;
		}
		return Play(song, 0.0, isSong: true, volume, group, pitch, pan);
	}

	public void ReplaySong()
	{
		previousSong = currentSong;
		currentSong = PlaySong(currentLoopDestinationString, currentLoopDestinationGroup, pitch, pan, useBeatsNotBar: true, volume);
	}

	public void PlayNoSong()
	{
		double num = audioPos + 0.0 + (double)latency;
		startOfNextBar = (startOfSong = num + (double)(0f * crotchet) + 0.0);
		startOfSongBarNumber = barNumber;
		startOfLastPrebar = audioPos + 0.0;
		startOfLastBeatVisual = startOfSong + (double)calibration_v;
		FakeSongFunctionPlayed = true;
		songStarted = true;
	}

	public AudioSource PlayBeat(string sound, float time, float gain_multiplier = 1f, AudioMixerGroup group = null, float min_pitch = 1f, float max_pitch = 1f, float pan = 0f, float start_truncation = 0f, bool useGlobalBeatSound = true, bool isGameplayBeat = false)
	{
		if (isScrubbing && !isGameplayBeat)
		{
			return null;
		}
		if (string.IsNullOrEmpty(sound))
		{
			return null;
		}
		float gain = (useGlobalBeatSound ? (GlobalBeatsoundVolume * gain_multiplier) : gain_multiplier);
		AudioSource audioSource = Play(sound, time, isSong: false, gain, group, max_pitch, pan, start_truncation);
		audiosourcesFromPlayBeat.RDAdd(audioSource);
		audioStartTimesFromPlayBeat.RDAdd(audioSource, lastScheduledTime);
		return audioSource;
	}

	public AudioSource PlayBeat(GameSoundType gst, float time, AudioMixerGroup group, float min_pitch_multiplier = 1f, float max_pitch_multiplier = 1f, float pan = 0f, float additional_gain_multiplier = 1f, float startTruncation = 0f, bool isGameplayBeat = false)
	{
		RDGameSoundData rDGameSoundData = RDGameSounds.Get(gst);
		float num = rDGameSoundData.volume;
		float min_pitch = rDGameSoundData.minPitch * min_pitch_multiplier;
		float max_pitch = rDGameSoundData.maxPitch * max_pitch_multiplier;
		string filename = rDGameSoundData.filename;
		if (pan == 0f)
		{
			pan = rDGameSoundData.pan;
		}
		return PlayBeat(filename, time, num * additional_gain_multiplier, group, min_pitch, max_pitch, pan, startTruncation, useGlobalBeatSound: true, isGameplayBeat);
	}

	public AudioSource PlayBeatInCrotchets(string sound, float crotchets, float gain_multiplier = 1f, AudioMixerGroup group = null, float pitch = 1f, float pan = 0f)
	{
		return PlayBeat(sound, BeatToTime(crotchets), gain_multiplier, group, pitch, pitch, pan);
	}

	public AudioSource PlayBeatFromSongStart(string sound, double time, float gain_multiplier = 1f, AudioMixerGroup group = null, float pitch = 1f, float pan = 0f)
	{
		if (sound.IsNullOrEmpty())
		{
			return null;
		}
		float gain = GlobalBeatsoundVolume * gain_multiplier;
		return Play(sound, time - startOfNextBar + startOfSong, isSong: false, gain, group, pitch, pan);
	}

	public void SetBPM(float _bpm, bool adjustForSpeed = true)
	{
		if (adjustForSpeed)
		{
			bpm = _bpm * RDTime.speed;
		}
		else
		{
			bpm = _bpm;
		}
		if (crotchetRefreshOnlyOnBar == 0f)
		{
			crotchetRefreshOnlyOnBar = crotchet;
		}
		refreshBarLength();
		latency = Mathf.Min(crotchet * (float)crotchetsPerBar - 0.01f, latency);
		if (base.game != null)
		{
			int num = ((!scnGame.drumMode) ? 1 : 2);
			base.game.HoldDurationToSkipCutscene = NearestBPMMultiple(0.59f * (float)num);
		}
	}

	public void AddBPMChange(float beat, float bpm, bool adjustForSpeed = true)
	{
		float num = (adjustForSpeed ? (bpm * RDTime.speed) : bpm);
		BPMChange item = new BPMChange(beat, num);
		bpmChangesInBar.Add(item);
		refreshBarLength();
	}

	public static AudioSource PlayImmediatelyIgnoringTimeScale(string sound, float gainMultiplier = 1f, AudioMixerGroup group = null, float pitch = 1f, float pan = 0f)
	{
		return PlayImmediately(sound, gainMultiplier, group, pitch, pan, isBeatSound: false, ignoreListenerPause: true);
	}

	public static AudioSource PlayImmediately(string sound, float gain_multiplier = 1f, AudioMixerGroup group = null, float pitch = 1f, float pan = 0f, bool isBeatSound = false, bool ignoreListenerPause = false, bool dontActuallyPlayIt = false)
	{
		bool flag = SoundIsExternal(sound);
		float num = gain_multiplier;
		num = (flag ? (num * 0.4f) : (isBeatSound ? (num * GlobalBeatsoundVolume) : (num * GlobalOtherSoundVolume)));
		if (isBeatSound && !RDBase.debugSettings.BeatSounds)
		{
			return null;
		}
		pan = pan * GlobalPanningStrength * GlobalPanningInvert;
		if (flag)
		{
			if (Singleton<AudioManager>.Instance.audioLib.ContainsKey(sound))
			{
				return AudioManager.PlayExternal(Singleton<AudioManager>.Instance.audioLib[sound], 0.0, num, ignoreListenerPause, group, pitch, pan);
			}
			return null;
		}
		SongOffset songOffset = songOffsets.Get(sound);
		num *= songOffset.volume;
		return AudioManager.PlayImmediately(sound.HasAudioFileExtension() ? Path.GetFileNameWithoutExtension(sound) : string.Concat(songOffset.fullPath), num, group, pitch, pan, ignoreListenerPause, levelEditor: false, dontActuallyPlayIt);
	}

	public static AudioSource PlayImmediatelyLevelEditor(string sound, AudioMixerGroup group, float gain = 1f, float pitch = 1f)
	{
		SongOffset songOffset = songOffsets.Get(sound);
		gain *= songOffset.volume;
		return AudioManager.PlayImmediately(sound.HasAudioFileExtension() ? Path.GetFileNameWithoutExtension(sound) : string.Concat(songOffset.fullPath), gain, group, pitch, 0f, ignoreListenerPause: true);
	}

	public static AudioSource PlayFeedback(GameSoundType soundType, float multiplier = 1f, AudioMixerGroup group = null, float pitch = 1f, RDPlayer playerPanOverride = RDPlayer.NoChange, bool isBeatSound = false, bool ignoreListenerPause = false)
	{
		float num = 0f;
		if (playerPanOverride != RDPlayer.NoChange)
		{
			num = RDUtils.OverridePanFor2P(playerPanOverride, num);
		}
		if (soundType == GameSoundType.SmallMistake)
		{
			if (playerPanOverride == RDPlayer.P2)
			{
				if (!(smallMistakeTimerP2 <= 0f))
				{
					return null;
				}
				smallMistakeTimerP2 = 0.05f;
			}
			else
			{
				if (!(smallMistakeTimerP1 <= 0f))
				{
					return null;
				}
				smallMistakeTimerP1 = 0.05f;
			}
		}
		if (soundType == GameSoundType.BigMistake)
		{
			if (playerPanOverride == RDPlayer.P2)
			{
				if (!(bigMistakeTimerP2 <= 0f))
				{
					return null;
				}
				bigMistakeTimerP2 = 0.05f;
			}
			else
			{
				if (!(bigMistakeTimerP1 <= 0f))
				{
					return null;
				}
				bigMistakeTimerP1 = 0.05f;
			}
		}
		RDGameSoundData rDGameSoundData = RDGameSounds.Get(soundType);
		float gain_multiplier = rDGameSoundData.volume * multiplier;
		pitch *= rDGameSoundData.maxPitch;
		if (num == 0f)
		{
			num = rDGameSoundData.pan;
		}
		return PlayImmediately(rDGameSoundData.filename, gain_multiplier, group, pitch, num, isBeatSound, ignoreListenerPause);
	}

	public static bool SoundIsExternal(string filename)
	{
		return filename.Contains("*external");
	}

	public AudioSource PlayAbs(string sound, double time, float gain = 1f, bool isBeatSound = false, AudioMixerGroup group = null, float pitch = 1f, float pan = 0f, bool ignoreListenerPause = false)
	{
		if (isBeatSound && !RDBase.debugSettings.BeatSounds)
		{
			return null;
		}
		string text = "";
		float num = 0f;
		if (SoundIsExternal(sound))
		{
			num = externalSoundData[sound].offset;
			num /= pitch;
			text = sound;
		}
		else
		{
			SongOffset songOffset = songOffsets.Get(sound);
			num = songOffset.offset / pitch;
			gain *= songOffset.volume * (isBeatSound ? GlobalBeatsoundVolume : GlobalOtherSoundVolume);
			text = songOffset.fullPath;
		}
		pan = pan * GlobalPanningStrength * GlobalPanningInvert;
		return AudioManager.Play(text, time - (double)num, gain, group, pitch, pan, 0f, ignoreListenerPause);
	}

	public bool HasReachedVisual(double time)
	{
		return visualPos - time > 0.0;
	}

	public bool HasReachedVisualFromSongStart(double time)
	{
		return visualPos - startOfSong - time > 0.0;
	}

	public float BeatToTime(float beat, bool calledCloseToNextBarMaybe = false)
	{
		List<BPMChange> list = (calledCloseToNextBarMaybe ? bpmChangesInBarRefreshOnBar : bpmChangesInBar);
		if (list.Count == 0)
		{
			return crotchet * beat;
		}
		float num = 0f;
		float num2 = 0f;
		float num3 = bpm;
		foreach (BPMChange item in list.OrderBy((BPMChange a) => a.beat))
		{
			if (item.beat >= beat)
			{
				break;
			}
			float num4 = item.beat - num2;
			num += 60f / num3 * num4;
			num3 = item.bpm;
			num2 = item.beat;
		}
		float num5 = beat - num2;
		return num + 60f / num3 * num5;
	}

	public float TimeToBeatRefreshOnBar(double _time)
	{
		if (!hasCalculatedBPMChangeTimesOnBar)
		{
			if (!hasCalculatedBPMChangeTimesOnBar)
			{
				RefreshBPMChangesOnBar();
			}
			hasCalculatedBPMChangeTimesOnBar = true;
		}
		BPMChange bPMChange = null;
		foreach (BPMChange item in bpmChangesInBarRefreshOnBar.OrderBy((BPMChange a) => a.time))
		{
			if (_time < (double)item.time)
			{
				break;
			}
			bPMChange = item;
		}
		float num = 0f;
		float num2 = 0f;
		float num3 = 60f / crotchetRefreshOnlyOnBar;
		if (bPMChange != null)
		{
			num = bPMChange.beat;
			num2 = bPMChange.time;
			num3 = bPMChange.bpm;
		}
		_beatofLastBPMChange = num;
		_timeofLastBPMChange = num2;
		return num + (float)(_time - (double)num2) * num3 / 60f;
	}

	public float DurationBeatsToTime(float durationInCrotchets, bool calledCloseToNextBarMaybe = false)
	{
		return DurationBeatsToTime(currentBeat, durationInCrotchets, calledCloseToNextBarMaybe);
	}

	public float DurationBeatsToTime(float startBeat, float durationInCrotchets, bool calledCloseToNextBarMaybe = false)
	{
		float num = BeatToTime(startBeat + durationInCrotchets, calledCloseToNextBarMaybe);
		float num2 = BeatToTime(startBeat, calledCloseToNextBarMaybe);
		return Mathf.Max(0f, num - num2) * RDTime.speed;
	}

	public float NearestBPMMultiple(float goal)
	{
		float num = crotchet;
		float num2 = goal / num;
		float f = (((double)num2 < 0.5) ? Mathf.Log(num2, 2f) : num2);
		f = Mathf.Ceil(f);
		if ((double)num2 < 0.5)
		{
			f = Mathf.Pow(2f, f);
		}
		return num * f;
	}

	private void refreshBarLength()
	{
		float a = BeatToTime(crotchetsPerBar);
		barLength = Mathf.Min(a, 9999f);
	}

	private void RefreshBPMChangesOnBar()
	{
		bpmChangesInBarRefreshOnBar = bpmChangesInBar.ConvertAll((BPMChange bpmc) => new BPMChange(bpmc.beat, bpmc.bpm));
		foreach (BPMChange item in bpmChangesInBarRefreshOnBar.OrderBy((BPMChange a) => a.time))
		{
			item.time = BeatToTime(item.beat);
		}
	}

	private float GetLastBPMChange()
	{
		return bpmChangesInBar.OrderBy((BPMChange a) => a.time).Last().bpm;
	}

	private void OnGUI()
	{
		if (!debug)
		{
			return;
		}
		if (Application.isEditor)
		{
			GUI.skin.label.fontSize = 10;
			GUI.color = Color.red;
			string text = "";
			foreach (RDExecuteOn execute in executes)
			{
				text = text + execute.ToString() + " -> " + execute.callingMethod + "\n";
			}
			GUI.Label(base.guiRect, text);
		}
		string text2 = "tElapsed: " + (visualPos - startOfSong);
		GUI.Label(new Rect(0f, 0f, 200f, 200f), text2);
	}
}
