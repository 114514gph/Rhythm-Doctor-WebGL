using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class PauseMenuContentData
{
	private const int TrueInt = 1;

	private const int FalseInt = 0;

	public PauseContentName name;

	public Sprite icon;

	public float iconSize = 10f;

	public bool needsConfirmation;

	public PauseContentValueType valueType;

	public int currentIndex;

	private const string windowMovementPrefixKey = "enum.WindowMovement.";

	private static readonly WindowMovement[] DesktopWindowMovements = new WindowMovement[3]
	{
		WindowMovement.Full,
		WindowMovement.OneScreen,
		WindowMovement.Simulate
	};

	private static readonly WindowMovement[] ConsoleWindowMovements = new WindowMovement[1] { WindowMovement.Simulate };

	private const string defibPrefixKey = "enum.DefibMode.";

	public readonly DefibMode[] DefaultDifficultyOptions = new DefibMode[3]
	{
		DefibMode.Easy,
		DefibMode.Normal,
		DefibMode.Hard
	};

	public readonly DefibMode[] ExtraDifficultyOptions = new DefibMode[5]
	{
		DefibMode.Unmissable,
		DefibMode.VeryEasy,
		DefibMode.Easy,
		DefibMode.Normal,
		DefibMode.Hard
	};

	private const string accessibilityScopePrefixKey = "enum.AccessibilityScope.";

	public readonly AccessibilityScope[] AccessibilityScopes = new AccessibilityScope[2]
	{
		AccessibilityScope.StoryLevelsOnly,
		AccessibilityScope.AllLevels
	};

	private const string discordPresencePrefixKey = "enum.DiscordPresence.";

	public readonly DiscordPresence[] DiscordPresences = new DiscordPresence[5]
	{
		DiscordPresence.Full,
		DiscordPresence.NoEditor,
		DiscordPresence.NoCustom,
		DiscordPresence.Never,
		DiscordPresence.Disabled
	};

	public string[] stringSelectablesValue;

	public int minIntValue;

	public int maxIntValue;

	public int changeIntValue;

	public int shiftChangeIntValue;

	public string trueText;

	public string falseText;

	private const string LanguageToken = "enum.SystemLanguage.";

	private const string MaxResolutionShortToken = "pauseMenu.levelDetail.maxResolutionShort";

	private const string MaxResolutionLongToken = "pauseMenu.levelDetail.maxResolutionLong";

	private const string reportPositionToken = "enum.ReportPosition.";

	public readonly ReportPosition[] ReportPositions = new ReportPosition[3]
	{
		ReportPosition.Never,
		ReportPosition.Once,
		ReportPosition.Always
	};

	private static Resolution[] _availableResolutions;

	[NonSerialized]
	public bool changedValue;

	private bool changedValueUsingShift;

	public PauseModeName targetMode
	{
		get
		{
			switch (name)
			{
			case PauseContentName.Settings:
				return PauseModeName.GameSettings;
			case PauseContentName.GeneralSettings:
				return PauseModeName.GameGeneralSettings;
			case PauseContentName.AccessibilitySettings:
				return PauseModeName.AccessibilitySettings;
			case PauseContentName.AdvancedSettings:
				return PauseModeName.AdvancedSettings;
			case PauseContentName.AudioSettings:
				return PauseModeName.AudioSettings;
			default:
				Debug.LogError("Add target mode for mode changer " + name.ToString() + " here");
				return PauseModeName.None;
			}
		}
	}

	public WindowMovement currentWindowMovementValue => WindowMovements[currentIndex];

	public static bool usesConsoleWindowMovement
	{
		get
		{
			#if UNITY_WEBGL
			return true;
			#endif
			if (!scnBase.isLinux && !scnBase.isMobile)
			{
				return scnBase.isConsole;
			}
			return true;
		}
	}

	public static WindowMovement[] WindowMovements
	{
		get
		{
			if (!usesConsoleWindowMovement)
			{
				return DesktopWindowMovements;
			}
			return ConsoleWindowMovements;
		}
	}

	public DefibMode currentDefibValue => defibOptions[currentIndex];

	public DefibMode[] defibOptions
	{
		get
		{
			if (!Persistence.GetExtraDifficultyOptions())
			{
				return DefaultDifficultyOptions;
			}
			return ExtraDifficultyOptions;
		}
	}

	public AccessibilityScope currentAccessibilityScopeValue => AccessibilityScopes[currentIndex];

	public DiscordPresence currentDiscordPresenceValue => DiscordPresences[currentIndex];

	public string CurrentStringValue => stringSelectablesValue[currentIndex];

	private bool UsesShiftChangeValue => shiftChangeIntValue > 0;

	public bool CurrentBoolValue => currentIndex == 1;

	private List<SystemLanguage> AvailableLanguages
	{
		get
		{
			if (name != PauseContentName.Voices)
			{
				return RDString.AvailableLanguages;
			}
			return RDString.AvailableVoiceLanguages;
		}
	}

	public SystemLanguage CurrentLanguage => AvailableLanguages[currentIndex];

	public ReportPosition currentReportPositionValue => ReportPositions[currentIndex];

	public static Resolution[] AvailableResolutions
	{
		get
		{
			if (_availableResolutions == null)
			{
				UpdateAvailableResolutions();
			}
			return _availableResolutions;
		}
	}

	public bool IsFullResolution => scrVfxControl.ScaleIsFullResolution(CurrentResolutionScale);

	public int CurrentResolutionScale => currentIndex + 2;

	public string CurrentShortResolution
	{
		get
		{
			if (!IsFullResolution)
			{
				return $"{CurrentResolutionScale}x";
			}
			return RDString.Get("pauseMenu.levelDetail.maxResolutionShort");
		}
	}

	public string CurrentLongResolution
	{
		get
		{
			Resolution resolution = AvailableResolutions[currentIndex];
			return $"{resolution.width}x{resolution.height}";
		}
	}

	public float IndexPercentage
	{
		get
		{
			switch (valueType)
			{
			case PauseContentValueType.String:
				return (float)currentIndex / (float)(stringSelectablesValue.Length - 1);
			case PauseContentValueType.Int:
				return (float)(currentIndex - minIntValue) / (float)(maxIntValue - minIntValue);
			case PauseContentValueType.LanguageLocalization:
				return (float)currentIndex / (float)(AvailableLanguages.Count - 1);
			case PauseContentValueType.Resolution:
				return (float)currentIndex / (float)(AvailableResolutions.Length - 1);
			case PauseContentValueType.Defibrillator:
				return (float)currentIndex / (float)(defibOptions.Length - 1);
			case PauseContentValueType.WindowMovement:
				return (float)currentIndex / (float)(WindowMovements.Length - 1);
			case PauseContentValueType.AccessibilityScope:
				return (float)currentIndex / (float)(AccessibilityScopes.Length - 1);
			case PauseContentValueType.NarrationReportPosition:
				return (float)currentIndex / (float)(ReportPositions.Length - 1);
			case PauseContentValueType.DiscordPresence:
				return (float)currentIndex / (float)(DiscordPresences.Length - 1);
			case PauseContentValueType.Bool:
				return -1f;
			default:
				RDBaseDllDummy.printem("Did not found valueType: " + valueType);
				return -1f;
			}
		}
	}

	public static void UpdateAvailableResolutions()
	{
		Resolution resolution = default;
		Vector2Int res = (PlatformHelper.instance as PlatformHelperWindows).GetCurrentMonitorResolution(IntPtr.Zero);
		resolution = new Resolution { width = res.x, height = res.y };
		float num = (float)resolution.width * 1f / (float)resolution.height;
		if (num != 1.7777778f)
		{
			if (num > 1.7777778f)
			{
				resolution.width = Mathf.RoundToInt((float)resolution.height * 1.7777778f);
			}
			else
			{
				resolution.height = Mathf.RoundToInt((float)resolution.width / 1.7777778f);
			}
		}
		int num2 = 2;
		RefreshRate refreshRateRatio = Screen.currentResolution.refreshRateRatio;
		Resolution xResolution = GetXResolution(num2, refreshRateRatio);
		List<Resolution> list = new List<Resolution>();
		do
		{
			list.Add(xResolution);
			num2++;
			xResolution = GetXResolution(num2, refreshRateRatio);
		}
		while (xResolution.width < resolution.width && xResolution.height < resolution.height);
		list.Add(resolution);
		_availableResolutions = list.Distinct().ToArray();
	}

	public bool CanChangeRight()
	{
		switch (valueType)
		{
		case PauseContentValueType.String:
			return currentIndex + 1 <= stringSelectablesValue.Length - 1;
		case PauseContentValueType.Int:
			if (currentIndex + changeIntValue > maxIntValue)
			{
				if (UsesShiftChangeValue)
				{
					return currentIndex + shiftChangeIntValue <= maxIntValue;
				}
				return false;
			}
			return true;
		case PauseContentValueType.Bool:
			return true;
		case PauseContentValueType.LanguageLocalization:
			return currentIndex + 1 <= AvailableLanguages.Count - 1;
		case PauseContentValueType.Resolution:
			return currentIndex + 1 <= AvailableResolutions.Length - 1;
		case PauseContentValueType.Defibrillator:
			return currentIndex + 1 <= defibOptions.Length - 1;
		case PauseContentValueType.WindowMovement:
			return currentIndex + 1 <= WindowMovements.Length - 1;
		case PauseContentValueType.AccessibilityScope:
			return currentIndex + 1 <= AccessibilityScopes.Length - 1;
		case PauseContentValueType.NarrationReportPosition:
			return currentIndex + 1 <= ReportPositions.Length - 1;
		case PauseContentValueType.DiscordPresence:
			return currentIndex + 1 <= DiscordPresences.Length - 1;
		default:
			RDBaseDllDummy.printem("Did not found valueType: " + valueType);
			return false;
		}
	}

	public bool CanChangeLeft()
	{
		switch (valueType)
		{
		case PauseContentValueType.Int:
			if (currentIndex - changeIntValue < minIntValue)
			{
				if (UsesShiftChangeValue)
				{
					return currentIndex - shiftChangeIntValue >= minIntValue;
				}
				return false;
			}
			return true;
		case PauseContentValueType.Bool:
			return true;
		case PauseContentValueType.String:
		case PauseContentValueType.LanguageLocalization:
		case PauseContentValueType.Resolution:
		case PauseContentValueType.Defibrillator:
		case PauseContentValueType.WindowMovement:
		case PauseContentValueType.AccessibilityScope:
		case PauseContentValueType.NarrationReportPosition:
		case PauseContentValueType.DiscordPresence:
			return currentIndex - 1 >= 0;
		default:
			RDBaseDllDummy.printem("Did not found valueType: " + valueType);
			return false;
		}
	}

	public string ChangeValue(int direction, bool shiftPressed, out bool isKey)
	{
		isKey = true;
		changedValueUsingShift = shiftPressed;
		switch (valueType)
		{
		case PauseContentValueType.Defibrillator:
			UpdateIntValue(direction, 0, defibOptions.Length - 1);
			return "enum.DefibMode." + defibOptions[currentIndex];
		case PauseContentValueType.String:
			UpdateIntValue(direction, 0, stringSelectablesValue.Length - 1);
			return CurrentStringValue;
		case PauseContentValueType.Int:
		{
			changedValue = false;
			int num = ((shiftPressed && UsesShiftChangeValue) ? shiftChangeIntValue : changeIntValue);
			int num2 = currentIndex + num * direction;
			if (num2 >= minIntValue && num2 <= maxIntValue)
			{
				currentIndex = num2;
				changedValue = true;
			}
			isKey = false;
			return currentIndex.ToString();
		}
		case PauseContentValueType.Bool:
			changedValue = true;
			currentIndex = ((currentIndex != 1) ? 1 : 0);
			if (currentIndex != 1)
			{
				return falseText;
			}
			return trueText;
		case PauseContentValueType.LanguageLocalization:
			UpdateIntValue(direction, 0, AvailableLanguages.Count - 1);
			return "enum.SystemLanguage." + AvailableLanguages[currentIndex];
		case PauseContentValueType.Resolution:
			UpdateIntValue(direction, 0, AvailableResolutions.Length - 1);
			isKey = false;
			return CurrentShortResolution;
		case PauseContentValueType.WindowMovement:
			UpdateIntValue(direction, 0, WindowMovements.Length - 1);
			return "enum.WindowMovement." + currentWindowMovementValue;
		case PauseContentValueType.AccessibilityScope:
			UpdateIntValue(direction, 0, AccessibilityScopes.Length - 1);
			return "enum.AccessibilityScope." + currentAccessibilityScopeValue;
		case PauseContentValueType.NarrationReportPosition:
			UpdateIntValue(direction, 0, ReportPositions.Length - 1);
			return "enum.ReportPosition." + currentReportPositionValue;
		case PauseContentValueType.DiscordPresence:
			UpdateIntValue(direction, 0, DiscordPresences.Length - 1);
			return "enum.DiscordPresence." + currentDiscordPresenceValue;
		default:
			return null;
		}
	}

	public string SetStartValue(object value, out bool isKey)
	{
		isKey = true;
		switch (valueType)
		{
		case PauseContentValueType.Defibrillator:
		{
			DefibMode value6 = (DefibMode)value;
			DefibMode[] array = (Persistence.GetExtraDifficultyOptions() ? ExtraDifficultyOptions : DefaultDifficultyOptions);
			currentIndex = Array.IndexOf(array, value6);
			currentIndex = Mathf.Clamp(currentIndex, 0, array.Length - 1);
			return "enum.DefibMode." + array[currentIndex];
		}
		case PauseContentValueType.String:
			currentIndex = (int)value;
			currentIndex = Mathf.Clamp(currentIndex, 0, stringSelectablesValue.Length - 1);
			return CurrentStringValue;
		case PauseContentValueType.Int:
			int.TryParse(value.ToString(), out currentIndex);
			currentIndex = Mathf.Clamp(currentIndex, minIntValue, maxIntValue);
			isKey = false;
			return currentIndex.ToString();
		case PauseContentValueType.Bool:
			currentIndex = (((bool)value) ? 1 : 0);
			if (currentIndex != 1)
			{
				return falseText;
			}
			return trueText;
		case PauseContentValueType.LanguageLocalization:
		{
			SystemLanguage systemLanguage = ((name == PauseContentName.Voices) ? RDString.voiceLanguage : RDString.language);
			currentIndex = AvailableLanguages.IndexOf(systemLanguage);
			if (name == PauseContentName.Voices)
			{
				return "enum.SystemLanguage." + systemLanguage;
			}
			isKey = false;
			return RDString.GetNativeLanguageName(systemLanguage);
		}
		case PauseContentValueType.Resolution:
			_availableResolutions = null;
			isKey = false;
			currentIndex = Array.FindIndex(AvailableResolutions, (Resolution resolution) => resolution.width == Screen.width && resolution.height == Screen.height);
			if (currentIndex == -1)
			{
				currentIndex = AvailableResolutions.Length - 1;
			}
			return CurrentShortResolution;
		case PauseContentValueType.WindowMovement:
		{
			WindowMovement value5 = (WindowMovement)value;
			currentIndex = Array.IndexOf(WindowMovements, value5);
			currentIndex = Mathf.Clamp(currentIndex, 0, WindowMovements.Length);
			return "enum.WindowMovement." + currentWindowMovementValue;
		}
		case PauseContentValueType.AccessibilityScope:
		{
			AccessibilityScope value4 = (AccessibilityScope)value;
			currentIndex = Array.IndexOf(AccessibilityScopes, value4);
			return "enum.AccessibilityScope." + currentAccessibilityScopeValue;
		}
		case PauseContentValueType.NarrationReportPosition:
		{
			ReportPosition value3 = (ReportPosition)value;
			currentIndex = Array.IndexOf(ReportPositions, value3);
			return "enum.ReportPosition." + currentReportPositionValue;
		}
		case PauseContentValueType.DiscordPresence:
		{
			DiscordPresence value2 = (DiscordPresence)value;
			currentIndex = Array.IndexOf(DiscordPresences, value2);
			currentIndex = Mathf.Clamp(currentIndex, 0, DiscordPresences.Length);
			return "enum.DiscordPresence." + currentDiscordPresenceValue;
		}
		default:
			return null;
		}
	}

	private void UpdateIntValue(int direction, int minValue, int maxValue)
	{
		changedValue = false;
		int num = currentIndex + direction;
		if (num >= minValue || num <= maxValue)
		{
			currentIndex = num;
			changedValue = true;
		}
	}

	private static Resolution GetXResolution(int scale, RefreshRate refreshRate)
	{
		return new Resolution
		{
			width = 352 * scale,
			height = 198 * scale,
			refreshRateRatio = refreshRate
		};
	}
}
