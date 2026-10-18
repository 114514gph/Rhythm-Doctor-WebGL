using System;
using System.IO;
using System.Linq;
using DG.Tweening;
using Rewired;
using UnityEngine;
using UnityEngine.SceneManagement;

public class scnBase : RDBase
{
	private static readonly KeyCode[] DebugModeCheatCode = new KeyCode[9]
	{
		KeyCode.D,
		KeyCode.E,
		KeyCode.S,
		KeyCode.P,
		KeyCode.A,
		KeyCode.C,
		KeyCode.I,
		KeyCode.T,
		KeyCode.Alpha0
	};

	private static readonly KeyCode[] SamuraiModeCheatCode = new KeyCode[9]
	{
		KeyCode.LeftShift,
		KeyCode.S,
		KeyCode.A,
		KeyCode.M,
		KeyCode.U,
		KeyCode.R,
		KeyCode.A,
		KeyCode.I,
		KeyCode.Period
	};

	private static readonly KeyCode[] SongOfTheSeaCheatCode = new KeyCode[4]
	{
		KeyCode.J,
		KeyCode.J,
		KeyCode.D,
		KeyCode.F
	};

	public static string lastSceneName = null;

	public static string currentLevelSelect = null;

	public Camera wrldCamera;

	public bool muteWhenGameUnfocused;

	protected static scnBase _instance;

	private RDCheatCode.CheatCode debugModeCheat;

	private RDCheatCode.CheatCode samuraiModeCheat;

	private RDCheatCode.CheatCode songOfTheSeaCheat;

	private float timeSinceLastDebugMessage;

	private string currentDebugMessage;

	private int previousTypingSound;

	private float debugMessageDuration = 4f;

	private int lastTypingFrame;

	public static scnBase instance => _instance;

	public bool audioPaused
	{
		get
		{
			return AudioListener.pause;
		}
		set
		{
			AudioListener.pause = value;
		}
	}

	public static bool isDesktop
	{
		get
		{
			if (!isMac && !isWindows)
			{
				return isLinux;
			}
			return true;
		}
	}

	public static bool isMac => RDBase.platform == Platform.Mac;

	public static bool isWindows => RDBase.platform == Platform.Windows;

	public static bool isLinux => RDBase.platform == Platform.Linux;

	public static bool isMobile
	{
		get
		{
			if (Application.platform != RuntimePlatform.Android)
			{
				return Application.platform == RuntimePlatform.IPhonePlayer;
			}
			return true;
		}
	}

	public static bool isSwitch
	{
		get
		{
			if (!Persistence.GetSwitchFlag())
			{
				return Application.platform == RuntimePlatform.Switch;
			}
			return true;
		}
	}

	public static bool isXbox => false;

	public static bool isMicrosoft => false;

	public static bool isSteamDeck => Persistence.GetFeatureSet() == FeatureSet.SteamDeck;

	public static bool isConsole
	{
		get
		{
			if (!isXbox && !isSwitch)
			{
				return isSteamDeck;
			}
			return true;
		}
	}

	public static bool isBooth => RDC.booth;

	public static bool isCustomButton => Persistence.GetCustomButtonFlag();

	public static bool isDivekick => false;

	public static bool isGamepad
	{
		get
		{
			Controller lastActiveController = ReInput.controllers.GetLastActiveController();
			if (!isXbox && !isSwitch && !RDBase.debugSettings.RunningOnSteamDeck)
			{
				if (lastActiveController != null && lastActiveController.type != Rewired.ControllerType.Keyboard)
				{
					return lastActiveController.type != Rewired.ControllerType.Mouse;
				}
				return false;
			}
			return true;
		}
	}

	protected void Awake()
	{
		if (!(this is scnGame))
		{
			scrVfxControl.CheckForWindowDanceChanges();
		}
	}

	protected void Start()
	{
		if (scrGameManager.instance != null && base.ink != null)
		{
			base.ink.activeDialoguePresent = false;
			base.ink.isPlaying = false;
		}
		if (base.editor != null)
		{
			string text = "noKillOnReload";
			DOTween.KillAll(false, text);
		}
		else
		{
			DOTween.KillAll(false, "noKillOnReloadGeneral");
		}
		FocusVolumeUpdate();
		AudioListener.pause = false;
		RDTime.speed = 1f;
		Time.timeScale = 1f;
		DOTween.timeScale = 1f;
		debugModeCheat = new RDCheatCode.CheatCode(DebugModeCheatCode);
		samuraiModeCheat = new RDCheatCode.CheatCode(SamuraiModeCheatCode);
		songOfTheSeaCheat = new RDCheatCode.CheatCode(SongOfTheSeaCheatCode);
		Cursor.visible = !(this is scnGame) || base.editorMode;
	}

	protected void Update()
	{
		RDInput.Update();
		if ((debugModeCheat.CheckCheatCode() || (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Home))) && RDBase.isDev)
		{
			RDBase.debugSettings.Debug = !RDBase.debugSettings.Debug;
			ShowDebugMessage("Debug mode is now " + RDBase.debugSettings.Debug);
		}
		if (samuraiModeCheat.CheckCheatCode() && base.editor == null && !(this is scnGame) && !(this is scnCLS))
		{
			RDString.samuraiMode = !RDString.samuraiMode;
			GoToScene(base.currentSceneName);
		}
		if (songOfTheSeaCheat.CheckCheatCode() && this is scnMenu)
		{
			GoToLevelWithEnum(Level.SongOfTheSea);
		}
		if (isBooth && Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.X))
		{
			if (instance is scnExpoMenu)
			{
				GoToScene("scnMenu");
			}
			else if (instance is scnMenu)
			{
				GoToExpoLevelSelect();
			}
		}
		CheckShowPersistentDataPath();
	}

	public static void CheckShowPersistentDataPath()
	{
		if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.L))
		{
			RDUtils.RevealInExplorer(Persistence.DataPath);
		}
	}

	public virtual void OnPreBar()
	{
	}

	public virtual void OnBeat()
	{
	}

	public virtual void OnNewBar()
	{
		foreach (Entity item in base.gm.entityPool)
		{
			item.OnNewBar();
		}
	}

	public virtual void Localize()
	{
	}

	private void OnApplicationQuit()
	{
		Persistence.SaveAll();
	}

	protected virtual void OnApplicationPause(bool paused)
	{
		if (paused)
		{
			Persistence.SaveAll();
		}
	}

	public void ShowDebugMessage(string message, float duration = 4f)
	{
		currentDebugMessage = message;
		timeSinceLastDebugMessage = Time.unscaledTime;
		debugMessageDuration = duration;
	}

	protected void OnGUI()
	{
		if (Time.unscaledTime - timeSinceLastDebugMessage < debugMessageDuration)
		{
			RDBase.ShowGUIText(currentDebugMessage);
		}
	}

	protected void OnApplicationFocus(bool hasFocus)
	{
		GC.gameHasFocus = hasFocus;
		FocusVolumeUpdate();
	}

	private void FocusVolumeUpdate()
	{
		scrGameManager.ToggleFullMute(muteWhenGameUnfocused && !GC.gameHasFocus && scrGameManager.muteInBackground);
	}

	public static void GoToSignupScreen()
	{
		GoToScene("scnSignup");
	}

	public static void GoToWarning()
	{
		GoToScene("scnWarning");
	}

	public static void GoToCalibration()
	{
		GoToScene("scnCalibration");
	}

	public static void GoToAlphaLevelSelect()
	{
		GoToScene("scnAlphaLevelSelect");
	}

	public static void GoToExpoLevelSelect()
	{
		GoToScene("scnExpoMenu");
	}

	public static void GoToCustomLevelSelect()
	{
		GoToScene("scnCLS");
	}

	public static void GoToSleevePaint()
	{
		GoToScene("scnSleevePaint");
	}

	public static void GoToIanDesktop()
	{
		GoToScene("scnIanDesktop");
	}

	public static void GoToRhythmWeightlifter()
	{
		GoToScene("scnRhythmWeightlifter");
	}

	public static void GoToMainMenu()
	{
		if (isBooth)
		{
			GoToExpoLevelSelect();
		}
		else
		{
			GoToScene("scnMenu");
		}
	}

	public static void GoToLevelEditor()
	{
		GC.twoPlayerMode = false;
		GoToScene("scnEditor");
	}

	public static void GoToLevelSelect()
	{
		scnGame.ClearLevelPersistence();
		scrVfxControl.FlushCustomLevelData();
		if (currentLevelSelect == null)
		{
			currentLevelSelect = (isBooth ? "scnExpoMenu" : "scnLevelSelect");
		}
		if (DebugSettings.instance.RecordingMode)
		{
			OutputFPSRecordingData();
			GC.levelRecordedIndex++;
			MonoBehaviour.print("levelRecordedIndex: " + GC.levelRecordedIndex);
			if (GC.levelRecordedIndex < GC.levelsToRecord.Length)
			{
				string text = GC.levelsToRecord[GC.levelRecordedIndex];
				if (text.Contains("Cutscenes/"))
				{
					string text2 = text.Substring("Cutscenes/".Length);
					Debug.Log("cutscene: " + text2);
					GoToCutscene(text2);
				}
				else
				{
					GoToLevel(text, loadGameScene: true, attemptToLoadTutorial: true);
				}
			}
		}
		else
		{
			GoToScene(currentLevelSelect);
		}
	}

	public static void OutputFPSRecordingData()
	{
		scnGame scnGame2 = scnGame.instance;
		if (scnGame2 == null || !DebugSettings.instance.RecordingMode)
		{
			return;
		}
		scnGame2.recordingFPSTimer = 0f;
		if (scnGame2.recordedFPSList.Count <= 0)
		{
			return;
		}
		string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), scnGame2.recordingFileName);
		float num = 10000f;
		if (File.Exists(path) && !((float)File.ReadLines(path).Count() <= num))
		{
			return;
		}
		using StreamWriter streamWriter = File.AppendText(path);
		string text = "- fps_snapshots (5s): ";
		float num2 = 0f;
		for (int i = 0; i < scnGame2.recordedFPSList.Count; i++)
		{
			float num3 = Mathf.Floor(scnGame2.recordedFPSList[i]);
			text += num3;
			if (i + 1 != scnGame2.recordedFPSList.Count)
			{
				text += ", ";
			}
			num2 += num3;
		}
		streamWriter.WriteLine(scnGame2.levelIdentifier + " - " + DateTime.Now);
		streamWriter.WriteLine(text);
		streamWriter.WriteLine("- fps_max: " + scnGame2.maxFPS);
		streamWriter.WriteLine("- fps_min: " + scnGame2.minFPS);
		streamWriter.WriteLine("- fps_average: " + num2 / (float)scnGame2.recordedFPSList.Count);
		streamWriter.WriteLine("");
	}

	public static void GoToLevel(string path, bool loadGameScene = true, bool attemptToLoadTutorial = false)
	{
		if (path.Contains(".rdlevel") || path.Contains(".rdzip"))
		{
			scnGame.levelToLoadSource = LevelSource.ExternalPath;
			scnGame.currentLevelPath = path;
		}
		else
		{
			scnGame.levelToLoadSource = LevelSource.InternalPath;
			scnGame.internalIdentifier = (SpeedrunValues.currentLevel = path);
			scnGame.attemptToLoadTutorial = attemptToLoadTutorial;
		}
		if (path != Level.Boss2.ToString())
		{
			Level_Boss2.passedCheckpoint = false;
		}
		if (path != Level.InsomniacHard.ToString())
		{
			Level_InsomniacHard.hasGlitched = false;
		}
		if (loadGameScene)
		{
			GoToScene("scnGame");
		}
	}

	public static void GoToCutscene(string cutsceneName)
	{
		RDBaseDllDummy.printes("playing cutscene " + cutsceneName);
		scnGame.levelToLoadSource = LevelSource.CutscenesPath;
		scnGame.internalIdentifier = cutsceneName;
		scnGame.levelSpeed = 1f;
		GoToScene("scnGame");
	}

	public static string PathForCutscene(string cutsceneName)
	{
		return "InternalLevels/Cutscenes/" + cutsceneName;
	}

	public static void GoToLevelWithEnum(Level level)
	{
		OutputFPSRecordingData();
		GoToLevel(level.ToString());
	}

	public static void GoToLevelWithSteamCustomID(string id)
	{
		RDBaseDllDummy.printes("GoToLevelWithSteamCustomID: " + id);
	}

	public static void GoToLevelWithExternalPath(string path)
	{
		RDBaseDllDummy.printes("GoToLevelWithPath: " + path);
		scnGame.levelToLoadSource = LevelSource.ExternalPath;
		scnGame.currentLevelPath = path;
		GoToScene("scnGame");
	}

	public static void GoToScene(string name)
	{
		if (!RDBase.debugSettings.Debug)
		{
			RDBase.debugSettings.Auto = false;
		}
		lastSceneName = SceneManager.GetActiveScene().name;
		Singleton<AudioManager>.Instance.DestroyAllAudioSources();
		SceneManager.sceneLoaded += OnSceneLoaded;
		SceneManager.LoadScene(name);
	}

	public static void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	public void PlayHammerSound(string group = "InterfaceSoundsParent", float pitch = 1f)
	{
		scrConductor.PlayImmediately("sndHammer", 0.8f, RDUtils.GetMixerGroup(group), pitch);
	}

	public void PlayCursorSound(string group = "InterfaceSoundsParent", float pitch = 1f)
	{
		scrConductor.PlayImmediately("sndPagerCursorMove", 1f, RDUtils.GetMixerGroup(group), pitch);
	}

	public void PlayButtonSound(string group = "InterfaceSoundsParent", float pitch = 1f)
	{
		scrConductor.PlayImmediately("sndPagerButton", 1f, RDUtils.GetMixerGroup(group), pitch);
	}

	public void PlayValueSound(string group = "InterfaceSoundsParent", float pitch = 1f)
	{
		scrConductor.PlayImmediately("sndPagerValue", 1f, RDUtils.GetMixerGroup(group), pitch);
	}

	public void PlayBorderSound(string group = "InterfaceSoundsParent", float pitch = 1f)
	{
		scrConductor.PlayImmediately("sndPagerCursorBorder", 1f, RDUtils.GetMixerGroup(group), pitch);
	}

	public void PlayTypingSound(string group = "InterfaceSoundsParent")
	{
		if (Time.frameCount != lastTypingFrame)
		{
			lastTypingFrame = Time.frameCount;
			string[] array = new string[6] { "sndTypingInput00", "sndTypingInput01", "sndTypingInput02", "sndTypingInput03", "sndTypingInput04", "sndTypingInput05" };
			int num = UnityEngine.Random.Range(0, 6);
			if (num == previousTypingSound)
			{
				num = ((num != array.Length - 1) ? (++num) : 0);
			}
			string sound = array[num];
			float num2 = UnityEngine.Random.Range(RDUtils.PitchSemitonesToPercent(-0.5f), RDUtils.PitchSemitonesToPercent(0.5f));
			float pitch = num2;
			scrConductor.PlayImmediately(sound, 1f, RDUtils.GetMixerGroup(group), pitch, 0f, isBeatSound: false, ignoreListenerPause: true);
			previousTypingSound = num;
		}
	}
}
