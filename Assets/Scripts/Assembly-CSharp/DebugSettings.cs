using UnityEngine;

public class DebugSettings
{
	public static readonly DebugSettings instance = new DebugSettings();

	[SerializeField]
	private bool debug;

	[SerializeField]
	private bool noPro;

	[SerializeField]
	private bool auto;

	[SerializeField]
	private bool beatSounds = true;

	[SerializeField]
	private bool forceNoSteamworks;

	[SerializeField]
	private bool emulateMobile;

	[SerializeField]
	private bool instantDialogue;

	[SerializeField]
	private bool recordingMode;

	[SerializeField]
	private bool skipMenuTransitions;

	[SerializeField]
	private bool paigeStays;

	[SerializeField]
	private bool debugAmbience;

	[SerializeField]
	private bool runningOnSteamDeck;

	[SerializeField]
	private bool giveAchievements;

	[SerializeField]
	private WindowMovement windowMovement;

	[SerializeField]
	private SystemLanguage language = SystemLanguage.English;

	[SerializeField]
	private bool pauseOnFocusLost = true;

	[SerializeField]
	private bool unlimitedFramerate;

	public bool Debug
	{
		get
		{
			return debug;
		}
		set
		{
			debug = value;
			Save(saveAsText: true);
		}
	}

	public bool NoPro
	{
		get
		{
			return noPro;
		}
		set
		{
			noPro = value;
			Save(saveAsText: true);
		}
	}

	public bool Auto
	{
		get
		{
			return auto;
		}
		set
		{
			auto = value;
			Save(saveAsText: true);
		}
	}

	public bool BeatSounds
	{
		get
		{
			return beatSounds;
		}
		set
		{
			beatSounds = value;
			Save(saveAsText: true);
		}
	}

	public bool ForceNoSteamworks
	{
		get
		{
			return forceNoSteamworks;
		}
		set
		{
			forceNoSteamworks = value;
			Save(saveAsText: true);
		}
	}

	public bool EmulateMobile
	{
		get
		{
			return emulateMobile;
		}
		set
		{
			emulateMobile = value;
			Save(saveAsText: true);
		}
	}

	public bool InstantDialogue
	{
		get
		{
			return instantDialogue;
		}
		set
		{
			instantDialogue = value;
			Save(saveAsText: true);
		}
	}

	public bool RecordingMode
	{
		get
		{
			return recordingMode;
		}
		set
		{
			recordingMode = value;
			Save(saveAsText: true);
		}
	}

	public bool SkipMenuTransitions
	{
		get
		{
			return skipMenuTransitions;
		}
		set
		{
			skipMenuTransitions = value;
			Save(saveAsText: true);
		}
	}

	public bool PaigeStays
	{
		get
		{
			return paigeStays;
		}
		set
		{
			Persistence.SetPaigeEnding(value);
			paigeStays = value;
			Save(saveAsText: true);
		}
	}

	public bool DebugAmbience
	{
		get
		{
			return debugAmbience;
		}
		set
		{
			debugAmbience = value;
			Save(saveAsText: true);
		}
	}

	public bool RunningOnSteamDeck
	{
		get
		{
			return runningOnSteamDeck;
		}
		set
		{
			runningOnSteamDeck = value;
			Save(saveAsText: true);
		}
	}

	public bool GiveAchievements
	{
		get
		{
			return giveAchievements;
		}
		set
		{
			giveAchievements = value;
			Save(saveAsText: true);
		}
	}

	public WindowMovement WindowMovement
	{
		get
		{
			#if UNITY_WEBGL
			return WindowMovement.Simulate;
			#endif
			return windowMovement;
		}
		set
		{
			windowMovement = value;
			Save(saveAsText: true);
		}
	}

	public SystemLanguage Language
	{
		get
		{
			return language;
		}
		set
		{
			language = value;
			Save(saveAsText: true);
		}
	}

	public bool PauseOnFocusLost
	{
		get
		{
			return pauseOnFocusLost;
		}
		set
		{
			Application.runInBackground = !value;
			pauseOnFocusLost = value;
			Save(saveAsText: true);
		}
	}

	public bool UnlimitedFramerate
	{
		get
		{
			return unlimitedFramerate;
		}
		set
		{
			Application.targetFrameRate = (value ? 10000 : 120);
			unlimitedFramerate = value;
			Save(saveAsText: true);
		}
	}

	private void Save(bool saveAsText)
	{
	}
}
