using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using DG.Tweening;
using RDLevelEditor;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class RDStartup
{
	public static bool hasInitialized;

	public static string firstScene = "";

	public static ErrorCanvas errorCanvas;

	public static Dictionary<Type, Dictionary<int, int>> levelDeathsAtBars = new Dictionary<Type, Dictionary<int, int>>();

	public static Dictionary<Type, int> levelTries = new Dictionary<Type, int>();

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	public static void Setup()
	{
		if (hasInitialized)
		{
			return;
		}
		hasInitialized = true;
		StartupLog("SetDebugMode");
		SetDebugMode();
		StartupLog("DeterminePlatform");
		DeterminePlatform();
		StartupLog("DetermineAppLocation");
		DetermineAppLocation();
		StartupLog("SetBuildDateAndCommit");
		SetBuildDateAndCommit();
		StartupLog("Loading Steam SDK");
#if !UNITY_WEBGL
		if (!DebugSettings.instance.ForceNoSteamworks)
		{
			SteamIntegration.Setup();
			SteamIntegration.OpenConnection();
			if (SteamIntegration.initialized)
			{
				SteamWorkshop.Initialize();
			}
		}
#endif
		StartupLog("Instantiating Rewired");
		UnityEngine.Object.Instantiate(RDC.data.rewiredManager.gameObject);
		StartupLog("Instantiating Narration");
		UnityEngine.Object.Instantiate(RDC.data.narration.gameObject);
		Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
		StartupLog("Persistence.Load");
		Persistence.Load();
		GC.showAbsoluteOffsets = Persistence.GetFrameOffsets();
		GC.d_showMarginsNumerically = Persistence.GetErrorMargins();
		DebugSettings.instance.InstantDialogue = Persistence.GetInstantDialogue();
		DebugSettings.instance.PaigeStays = Persistence.GetPaigeEnding();
		DebugSettings.instance.RecordingMode = false;
		WindowMovement value = Persistence.GetWindowMovement();
		if (!Persistence.GetOBSFirstDetected() && RDUtils.IsOBSActive() && Persistence.GetWindowMovement() == WindowMovement.Full)
		{
			StartupLog("OBS detected for the first time, setting window dance movement to one screen");
			Persistence.SetOBSFirstDetected(OBSFirstDetected: true);
			Persistence.SetWindowMovement(WindowMovement.OneScreen);
			value = WindowMovement.OneScreen;
		}
		int value2 = Array.IndexOf(PauseMenuContentData.WindowMovements, value);
		value2 = Mathf.Clamp(value2, 0, PauseMenuContentData.WindowMovements.Length);
		RDC.windowMovement = PauseMenuContentData.WindowMovements[value2];
		StartupLog("LoadNarration");
		Narration.LoadNarration();
		StartupLog("LoadLevelMetadata");
		LoadLevelMetadata();
		StartupLog("LoadPlatformHelpers");
		LoadPlatformHelpers();
		StartupLog("SetupApplicationSettings");
		SetupApplicationSettings();
		StartupLog("SetupSystems");
		SetupSystems();
		StartupLog("LoadSavedVolumes");
		scrGameManager.LoadSavedVolumes();
		if (!scnBase.isMobile)
		{
#if !UNITY_WEBGL
			StartupLog("GetSteamBranch");
			GetSteamBranch();
#endif
			StartupLog("DeleteTempUnzippingFolder");
			DeleteTempUnzippingFolder();
#if !UNITY_WEBGL
			StartupLog("LoadRichPresence");
			LoadRichPresence();
			StartupLog("SetupFileURLHandler");
			SetupFileURLHandler();
#endif
			StartupLog("InitResolution");
			InitResolution();
		}
		StartupLog("LoadRowMarkers");
		LoadRowMarkers();
		StartupLog("PopulateExpressionGroupDictionary");
		PopulateExpressionGroupDictionary();
		StartupLog("LoadLevelEditorProperties");
		LoadLevelEditorProperties();
		StartupLog("LoadCharacters");
		LoadBaseCharacters();
		StartupLog("<color=green>RDStartup done!</color>");
	}

	private static void DeterminePlatform()
	{
		Platform platform;
		switch (Application.platform)
		{
		case RuntimePlatform.WindowsPlayer:
		case RuntimePlatform.WindowsEditor:
			platform = Platform.Windows;
			break;
		case RuntimePlatform.OSXEditor:
		case RuntimePlatform.OSXPlayer:
			platform = Platform.Mac;
			break;
		case RuntimePlatform.LinuxPlayer:
		case RuntimePlatform.LinuxEditor:
			platform = Platform.Linux;
			break;
		case RuntimePlatform.Android:
			platform = Platform.Android;
			break;
		case RuntimePlatform.IPhonePlayer:
			platform = Platform.iOS;
			break;
		case RuntimePlatform.Switch:
			platform = Platform.Switch;
			break;
		case RuntimePlatform.WebGLPlayer:
			platform = Platform.WebGL;
			break;
		case RuntimePlatform.XboxOne:
		case RuntimePlatform.GameCoreXboxSeries:
		case RuntimePlatform.GameCoreXboxOne:
			platform = Platform.Xbox;
			break;
		default:
			platform = Platform.Windows;
			break;
		}
		StartupLog($"Setting platform to {RDBase.platform = platform}.");
	}

	private static void LoadLevelEditorProperties()
	{
		Dictionary<string, object> dictionary = Json.Deserialize(Resources.Load<TextAsset>("LevelEditorProperties").text) as Dictionary<string, object>;
		Dictionary<string, object> obj = dictionary["tabs"] as Dictionary<string, object>;
		Dictionary<Tab, List<LevelEventType>> levelEventTabs = RDEditorConstants.levelEventTabs;
		Dictionary<Tab, List<RDEditorKeycode>> eventKeyCodes = RDEditorConstants.eventKeyCodes;
		Dictionary<GameSoundType, GameSoundType[]> gameSoundGroups = RDEditorConstants.gameSoundGroups;
		Dictionary<Tab, List<LevelEventType>> hiddenEvents = RDEditorConstants.hiddenEvents;
		Dictionary<Tab, List<LevelEventType>> advancedEvents = RDEditorConstants.advancedEvents;
		foreach (KeyValuePair<string, object> item2 in obj)
		{
			Tab key = Enum.Parse<Tab>(item2.Key);
			Dictionary<string, object> obj2 = item2.Value as Dictionary<string, object>;
			List<LevelEventType> list = new List<LevelEventType>();
			List<LevelEventType> list2 = new List<LevelEventType>();
			List<LevelEventType> list3 = new List<LevelEventType>();
			List<RDEditorKeycode> list4 = new List<RDEditorKeycode>();
			foreach (KeyValuePair<string, object> item3 in obj2)
			{
				LevelEventType item = Enum.Parse<LevelEventType>(item3.Key);
				Dictionary<string, object> dictionary2 = item3.Value as Dictionary<string, object>;
				if (!(dictionary2.TryGetValueAs("pro", out var valueAs, _default: false) & valueAs) || RDBase.isDev)
				{
					KeyCode key2 = KeyCode.None;
					bool shift = false;
					if (dictionary2.TryGetValue("key", out var value))
					{
						key2 = Enum.Parse<KeyCode>(value as string);
					}
					if (dictionary2.TryGetValue("shift", out var value2))
					{
						shift = Convert.ToBoolean(value2);
					}
					if (dictionary2.TryGetValue("unlisted", out var value3) && Convert.ToBoolean(value3))
					{
						list2.Add(item);
					}
					if (dictionary2.TryGetValue("advanced", out var value4) && Convert.ToBoolean(value4))
					{
						list3.Add(item);
					}
					list4.Add(new RDEditorKeycode
					{
						key = key2,
						shift = shift
					});
					list.Add(item);
				}
			}
			levelEventTabs.Add(key, list);
			hiddenEvents.Add(key, list2);
			advancedEvents.Add(key, list3);
			eventKeyCodes.Add(key, list4);
		}
		foreach (KeyValuePair<string, object> item4 in dictionary["gameSoundGroups"] as Dictionary<string, object>)
		{
			GameSoundType key3 = Enum.Parse<GameSoundType>(item4.Key);
			List<object> list5 = item4.Value as List<object>;
			GameSoundType[] array = new GameSoundType[list5.Count];
			for (int i = 0; i < list5.Count; i++)
			{
				array[i] = Enum.Parse<GameSoundType>(list5[i] as string);
			}
			gameSoundGroups.Add(key3, array);
		}
		Type[] types = Assembly.GetExecutingAssembly().GetTypes();
		GC.conditionalsInfo = (from t in types
			where t.IsDefined(typeof(ConditionalInfoAttribute))
			select new ConditionalInfo(t)).ToImmutableDictionary((ConditionalInfo conditionalInfo) => conditionalInfo.name, (ConditionalInfo result) => result);
		GC.levelEventsInfo = (from t in types
			where t.IsDefined(typeof(LevelEventInfoAttribute))
			select new LevelEventInfo(t)).ToImmutableDictionary((LevelEventInfo levelEventInfo) => levelEventInfo.eventTypeEnum, (LevelEventInfo result) => result);
		LevelEventType[] values = RDUtils.GetValues<LevelEventType>();
		foreach (LevelEventType levelEventType in values)
		{
			if (levelEventType != LevelEventType.None && levelEventType != LevelEventType.CommentShow && !GC.levelEventsInfo.ContainsKey(levelEventType))
			{
				StartupLogError($"LevelEventType.{levelEventType} is missing in GC.levelEventsInfo. " + "This means that the associated level event is missing a LevelEventInfoAttribute.");
			}
		}
	}

	private static void LoadBaseCharacters()
	{
		Dictionary<Character, CustomAnimationData> dictionary = (scrChar.baseCharacterAnimations = new Dictionary<Character, CustomAnimationData>());
		foreach (Character value2 in Enum.GetValues(typeof(Character)))
		{
			CustomAnimationData value = default;
			string text = value2.ToString();
			if (value2 == Character.Controller)
			{
				text = (scnBase.isXbox ? "ControllerXbox" : (scnBase.isSwitch ? "ControllerSwitch" : text));
			}
			string jsonText = Resources.Load<TextAsset>(Path.Combine("Characters", text))?.text;
			Texture2D mainTexture = Resources.Load<Texture2D>(Path.Combine("Characters", text));
			Texture2D outlineTexture = Resources.Load<Texture2D>(Path.Combine("Characters", text + "_outline"));
			Texture2D glowTexture = Resources.Load<Texture2D>(Path.Combine("Characters", text + "_glow"));
			Texture2D freezeTexture = Resources.Load<Texture2D>(Path.Combine("Characters", text + "_freeze"));
			Resources.Load<AudioClip>(Path.Combine("Sfx/SpeechTickers", "sndSpeech" + text));
			value.Setup(jsonText, mainTexture, outlineTexture, glowTexture, freezeTexture);
			value.name = text;
			dictionary.Add(value2, value);
		}
	}

	private static void PopulateExpressionGroupDictionary()
	{
		foreach (Character value in Enum.GetValues(typeof(Character)))
		{
			if (!GC.characterExpressions.ContainsKey(value))
			{
				GC.characterExpressions.Add(value, new CharacterExpressionGroup());
			}
		}
	}

	public static void LoadPlatformHelpers()
	{
		PlatformHelper.instance = new PlatformHelperWindows();
	}

	private static void SetupApplicationSettings()
	{
		firstScene = SceneManager.GetActiveScene().name;
		Application.runInBackground = true;
		Application.targetFrameRate = 120;
		StartupLog($"Framerate set to {Application.targetFrameRate}fps");
		QualitySettings.vSyncCount = ((scnBase.isXbox || Persistence.GetVSync()) ? 1 : 0);
		Screen.sleepTimeout = -1;
		Time.timeScale = 1f;
		LEDSign.showMarginError = false;
		scnGame.pauseBlocked = false;
	}

	private static void SetupSystems()
	{
		RDString.Setup();
		RDInput.Setup();
		RDCalibration.Setup();
	}

	private static void LoadRichPresence()
	{
		if (scnBase.isDesktop || scnBase.isXbox)
		{
			new GameObject("RichPresence").AddComponent<RDRichPresence>();
		}
	}

#if !UNITY_WEBGL
	private static void GetSteamBranch()
	{
		if (SteamIntegration.initialized)
		{
			string pchName = null;
			SteamApps.GetCurrentBetaName(out pchName, 20);
			GC.steamBranchName = pchName;
		}
	}
#endif

	private static void PrintArguments()
	{
		string[] commandLineArgs = Environment.GetCommandLineArgs();
		for (int i = 0; i < commandLineArgs.Length; i++)
		{
			Debug.Log($"ARG {i}: {commandLineArgs[i]}");
			if (commandLineArgs[i] == "-folderInput")
			{
				_ = commandLineArgs[i + 1];
			}
		}
	}

	private static void DeleteTempUnzippingFolder()
	{
		string tempLevelsFolder = LevelValidation.TempLevelsFolder;
		if (!Directory.Exists(tempLevelsFolder))
		{
			return;
		}
		RDUtils.SetDirectoryAttributes(tempLevelsFolder);
		Task.Factory.StartNew((Func<Task>)(async () =>
		{
			if (!(await RDUtils.TryDeleteDirectory(tempLevelsFolder)).IsNullOrEmpty())
			{
				Debug.LogWarning("Error deleting temps folder");
			}
		}));
	}

	private static void ClearErrorDictionary()
	{
		string errorDirectoryPath = ErrorCanvas.ErrorDirectoryPath;
		if (Directory.Exists(errorDirectoryPath))
		{
			try
			{
				RDUtils.SetDirectoryAttributes(errorDirectoryPath);
				Directory.Delete(errorDirectoryPath, recursive: true);
			}
			catch (Exception ex)
			{
				Debug.LogWarning("Error deleting errors folder: " + ex.Message);
			}
		}
	}

	public static void DetermineAppLocation()
	{
		bool appIsInSteamLibrary = false;
		StartupLog("Platform: " + Application.platform);
		DirectoryInfo directoryInfo = new DirectoryInfo(Application.dataPath)?.Parent;
		if (Application.platform == RuntimePlatform.OSXPlayer)
		{
			directoryInfo = directoryInfo?.Parent;
		}
		if (directoryInfo != null && directoryInfo.Parent?.Name == "common" && directoryInfo.Parent?.Parent?.Name == "steamapps")
		{
			appIsInSteamLibrary = true;
		}
		StartupLog("isInSteamLibrary: " + appIsInSteamLibrary);
		RDBase.appIsInSteamLibrary = appIsInSteamLibrary;
	}

	private static void InitResolution()
	{
		if (scnBase.isXbox)
		{
			Screen.SetResolution(1920, 1080, fullscreen: true);
		}
		else
		{
			if (!scnBase.isDesktop)
			{
				return;
			}
			Vector2Int vector2Int = new Vector2Int(Screen.width, Screen.height);
			StartupLog($"InitResolution: Current resolution ({vector2Int}).");
			Vector2Int vector2Int2 = vector2Int;
			if (vector2Int2.x < 704 || vector2Int2.y < 396)
			{
				vector2Int2 = new Vector2Int(704, 396);
				StartupLog($"InitResolution: Window size too small; defaulting to smallest size ({vector2Int2}).");
			}
			if (Mathf.Abs((float)vector2Int2.x / (float)vector2Int2.y - 1.7777778f) > 0.01f)
			{
				vector2Int2.y = Mathf.RoundToInt((float)vector2Int2.x / 1.7777778f);
				StartupLog($"InitResolution: Window aspect ratio is not 16:9, chanding to that ({vector2Int2}).");
			}
			bool flag = vector2Int != vector2Int2;
			if (flag)
			{
				if (Application.isEditor && PlatformHelper.instance?.playerWindow != null)
				{
					PlatformHelper.instance.playerWindow.SetViewSize(vector2Int2);
				}
				else
				{
					scrVfxControl.SetResolution(vector2Int2.x, vector2Int2.y, Screen.fullScreenMode);
				}
				StartupLog($"InitResolution: Size changed to {vector2Int2.x}");
			}
			if (Screen.fullScreenMode != FullScreenMode.Windowed)
			{
				return;
			}
			PlatformHelper instance = PlatformHelper.instance;
			UnityPlayerWindow playerWindow = instance?.playerWindow;
			if (playerWindow == null)
			{
				return;
			}
			Vector2Int position = playerWindow.GetPosition();
			Vector2Int windowDanceResolution = instance.WindowDanceResolution;
			StartupLog($"InitResolution: {position}, stageSize {windowDanceResolution}");
			if (position.x + vector2Int2.x <= 100 || position.y + vector2Int2.y <= 100 || windowDanceResolution.x - position.x <= 100 || windowDanceResolution.y - position.y <= 100)
			{
				StartupLog("InitResolution: Resetting position because it's out of bounds.");
				DOVirtual.DelayedCall(flag ? 0.5f : 0.1f, () =>
				{
					playerWindow.ResetPosition(null);
				}).SetUpdate(isIndependentUpdate: true).Done()
					.SetId("noKillOnReloadGeneral");
			}
		}
	}

	private static void CheckSteamDemoExpiry()
	{
	}

	private static void LoadRowMarkers()
	{
		RowEntity.markersDict = new Dictionary<string, Sprite>();
		AddMarker(Character.Samurai);
		AddMarker(RDPlayer.P1);
		static void AddMarker(Enum e)
		{
			foreach (object value in Enum.GetValues(e.GetType()))
			{
				string text = value.ToString();
				Sprite sprite = Resources.Load<Sprite>("CPUIcons/" + text);
				if (sprite != null)
				{
					RowEntity.markersDict.Add(text, sprite);
				}
				else
				{
					int num = 1;
					do
					{
						sprite = Resources.Load<Sprite>($"CPUIcons/Animations/{text}_marker_{num}");
						if (sprite != null)
						{
							RowEntity.markersDict.Add(text + num, sprite);
							num++;
						}
					}
					while (sprite != null);
				}
			}
		}
	}

	private static void LoadLevelMetadata()
	{
		GC.levelMetadata = new Dictionary<Level, LevelMetadata>();
		GC.levelCount = 0;
		foreach (KeyValuePair<string, object> item in Json.Deserialize(Resources.Load<TextAsset>("levelMetadata").text) as Dictionary<string, object>)
		{
			LevelMetadata levelMetadata = new LevelMetadata();
			string key = item.Key;
			Dictionary<string, object> levelProperties = item.Value as Dictionary<string, object>;
			levelMetadata.id = key;
			levelProperties.TryGetValueAs<string, object, string>("Unity Name", out var valueAs);
			Enum.TryParse<Level>(valueAs, out var result);
			if (result == Level.None)
			{
				continue;
			}
			if (levelProperties.TryGetValueAs<string, object, object>("BPM", out var valueAs2))
			{
				levelMetadata.bpm = Convert.ToSingle(valueAs2);
			}
			if (levelProperties.TryGetValueAs<string, object, string>("LevelType", out var valueAs3))
			{
				Enum.TryParse<LevelType>(valueAs3, out levelMetadata.type);
			}
			if (levelMetadata.type != LevelType.Tutorial && result != Level.GongXi && result != Level.RhythmWeightlifter && result != Level.Halloween)
			{
				GC.levelCount++;
			}
			if (levelProperties.TryGetValueAs<string, object, List<object>>("Ward Character", out var valueAs4))
			{
				levelMetadata.characters = new Character[valueAs4.Count];
				for (int i = 0; i < valueAs4.Count; i++)
				{
					levelMetadata.characters[i] = Character.None;
					Enum.TryParse<Character>((string)valueAs4[i], out levelMetadata.characters[i]);
				}
			}
			if (levelProperties.TryGetValueAs<string, object, List<object>>("Portrait", out var valueAs5))
			{
				levelMetadata.portraits = new Character[valueAs5.Count];
				for (int j = 0; j < valueAs5.Count; j++)
				{
					levelMetadata.portraits[j] = Character.None;
					Enum.TryParse<Character>((string)valueAs5[j], out levelMetadata.portraits[j]);
				}
			}
			levelMetadata.guest1 = GetGuest(1);
			levelMetadata.guest2 = GetGuest(2);
			levelMetadata.guest3 = GetGuest(3);
			levelMetadata.guest4 = GetGuest(4);
			GC.levelMetadata.Add(result, levelMetadata);
			GuestData GetGuest(int nr)
			{
				GuestData guestData = new GuestData();
				_ = "Guest " + nr;
				if (!levelProperties.TryGetValueAs<string, object, string>($"Guest {nr}", out guestData.name))
				{
					return null;
				}
				if (levelProperties.TryGetValueAs<string, object, string>($"Guest {nr} Type", out var valueAs6))
				{
					guestData.type = GetGuestTypeLocKey(valueAs6);
				}
				levelProperties.TryGetValueAs<string, object, string>($"Guest {nr} Link", out guestData.link);
				levelProperties.TryGetValueAs<string, object, string>($"Guest {nr} Link Type", out guestData.linkType);
				return guestData;
			}
		}
		static string GetGuestTypeLocKey(string str)
		{
			return str.Replace(" by", "").Replace(" ", "");
		}
	}

	private static void LogMessageReceived(string logString, string stackTrace, LogType type)
	{
		if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert)
		{
			if (errorCanvas == null)
			{
				errorCanvas = UnityEngine.Object.Instantiate(RDConstants.data.errorCanvas).GetComponent<ErrorCanvas>();
			}
			string text = $"{type}:\n{logString}\n{stackTrace}";
			errorCanvas.StartErrorProcess(text);
			RDBaseDllDummy.printes(text);
		}
	}

	private static void SetupFileURLHandler()
	{
		StartupLog("UrlProtocolHandler.SetupProtocol");
		UrlProtocolHandler.SetupProtocol();
	}

	private static void SetBuildDateAndCommit()
	{
		TextAsset textAsset = Resources.Load<TextAsset>("buildDate");
		string text = "Version ";
		if (textAsset != null)
		{
			Releases.buildDate = textAsset.text;
			text += $"r{Releases.releaseNumber} ({Releases.buildDate}";
		}
		TextAsset textAsset2 = Resources.Load<TextAsset>("buildCommit");
		if (textAsset2 != null)
		{
			Releases.buildCommit = textAsset2.text;
			text = text + ", " + Releases.buildCommit;
		}
		text += ")";
		StartupLog(text);
	}

	private static void SetDebugMode()
	{
		DebugSettings.instance.Debug = Debug.isDebugBuild;
	}

	private static void StartupLog(object log)
	{
		Debug.Log($"Startup: {log}");
	}

	private static void StartupLogError(object log)
	{
		Debug.LogError($"Startup Error: {log}");
	}
}
