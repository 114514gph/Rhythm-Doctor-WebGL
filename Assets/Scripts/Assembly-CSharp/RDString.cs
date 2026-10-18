using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Unicode;
using SA.GoogleDoc;
using TMPro;
using UnityEngine;

public static class RDString
{
	public enum LocalizationSheet
	{
		Strings = 0,
		Dialogues = 1
	}

	public static readonly List<SystemLanguage> AvailableVoiceLanguages = new List<SystemLanguage>
	{
		SystemLanguage.English,
		SystemLanguage.Chinese
	};

	public static readonly SystemLanguage fallbackLanguage = AvailableLanguages[0];

	public static readonly SystemLanguage fallbackVoiceLanguage = AvailableLanguages[0];

	public const string SamuraiModeText = "Samurai.";

	private const string MobileKeySuffix = ".mobile";

	private const string WindowsKeySuffix = ".mswin";

	private const string MacKeySuffix = ".macos";

	private const string LinuxKeySuffix = ".linux";

	private const string BoothButtonKeySuffix = ".boothButton";

	private const string DivekickKeySuffix = ".divekick";

	private const string NintendoSwitchKeySuffix = ".nx";

	private const string XboxKeySuffix = ".xbox";

	private const string GamepadKeySuffix = ".gamepad";

	private const string SteamDeckKeySuffix = ".steamdeck";

	private const string paletteTagToken = "[pal=";

	public static bool samuraiMode = false;

	public static SystemLanguage voiceLanguage = fallbackVoiceLanguage;

	public static SystemLanguage language = fallbackLanguage;

	public static FontPack currentFontPack;

	private static bool initialized = false;

	public const char ZeroWidthSpace = '\u00ad';

	public static List<SystemLanguage> AvailableLanguages => Localization.AvailableLanguages;

	public static List<SystemLanguage> AlphaOnlyLanguages => Localization.AlphaOnlyLanguages;

	public static Font currentLanguageFont => currentFontPack.font;

	public static TMP_FontAsset currentLanguageFontAsset => currentFontPack.fontAsset;

	public static string languageSuffix
	{
		get
		{
			if (fallbackLanguage != language)
			{
				return "_" + language;
			}
			return "";
		}
	}

	public static bool isChinese
	{
		get
		{
			SystemLanguage systemLanguage = language;
			return systemLanguage == SystemLanguage.ChineseSimplified || systemLanguage == SystemLanguage.Chinese || systemLanguage == SystemLanguage.ChineseTraditional;
		}
	}

	public static bool isKorean => language == SystemLanguage.Korean;

	public static bool isJapanese => language == SystemLanguage.Japanese;

	public static bool isCJK
	{
		get
		{
			if (!isChinese && !isKorean)
			{
				return isJapanese;
			}
			return true;
		}
	}

	public static string colon
	{
		get
		{
			if (!isCJK)
			{
				return ": ";
			}
			return "：";
		}
	}

	public static void Setup()
	{
		if (!initialized)
		{
			Reader.externalDialoguesDataPath = Path.Combine(Persistence.DataPath, "Dialogues");
			Reader.externalLocalizationDataPath = Path.Combine(Persistence.DataPath, "Localization");
			Reader.localizationPassword = Persistence.GetLocalizationPassword();
			if (GC.steamBranchName == "alpha")
			{
				AvailableLanguages.AddRange(AlphaOnlyLanguages);
			}
			SystemLanguage systemLanguage = ((RDConstants.data.forceLanguage && Application.isEditor) ? RDConstants.data.forcedLanguage : Persistence.GetLanguage());
			if (systemLanguage == SystemLanguage.Chinese)
			{
				systemLanguage = SystemLanguage.ChineseSimplified;
			}
			SystemLanguage num = GetFallbackVoiceLanguage(Persistence.GetVoiceLanguage());
			language = (AvailableLanguages.Contains(systemLanguage) ? systemLanguage : fallbackLanguage);
			voiceLanguage = num;
			LangCode defaultValue = RDUtils.ParseEnum(fallbackLanguage.ToString(), LangCode.English);
			Localization.SetLanguage(RDUtils.ParseEnum(language.ToString(), defaultValue));
			UpdateFontPack();
			initialized = true;
		}
	}

	public static void UpdateFontPack()
	{
		RDConstants data = RDConstants.data;
		switch (language)
		{
		case SystemLanguage.ChineseSimplified:
			currentFontPack = data.chineseSimpFontPack;
			break;
		case SystemLanguage.ChineseTraditional:
			currentFontPack = data.chineseTradFontPack;
			break;
		case SystemLanguage.Korean:
			currentFontPack = data.koreanFontPack;
			break;
		case SystemLanguage.Japanese:
			currentFontPack = data.japaneseFontPack;
			break;
		default:
			currentFontPack = data.latinFontPack;
			currentFontPack.usePixelFont = !Persistence.GetReducePixelFontUsage();
			break;
		}
	}

	public static SystemLanguage GetFallbackVoiceLanguage(SystemLanguage language)
	{
		if (language != SystemLanguage.ChineseSimplified && language != SystemLanguage.ChineseTraditional && language != SystemLanguage.Chinese)
		{
			return SystemLanguage.English;
		}
		return SystemLanguage.Chinese;
	}

	public static string Get(string key)
	{
		if (samuraiMode)
		{
			return "Samurai.";
		}
		if (!initialized)
		{
			Setup();
		}
		bool exists = false;
		string withCheck = GetWithCheck(key, out exists);
		if (!exists)
		{
			string text = "no key: " + key;
			Debug.LogWarning(text);
			return text;
		}
		return withCheck;
	}

	public static string GetJoined(string separatorKey, params string[] keys)
	{
		List<string> list = new List<string>();
		foreach (string key in keys)
		{
			list.RDAdd(Get(key));
		}
		return string.Join(Get(separatorKey), list.ToArray());
	}

	public static string GetVoice(string key)
	{
		if (voiceLanguage == SystemLanguage.Chinese)
		{
			RDBaseDllDummy.printes("getting chinese");
			return Localization.GetLocalizedString(key, LangSection.Levels, LangCode.ChineseSimplified);
		}
		return Localization.GetLocalizedString(key);
	}

	public static string GetEnumValue<T>(T value)
	{
		return Get($"enum.{typeof(T).Name}.{value}");
	}

	public static string GetWithCheck(string key, out bool exists)
	{
		if (!initialized)
		{
			Setup();
		}
		exists = false;
		string value = "";
		string localizationKey = key + ".mobile";
		string localizationKey2 = key + ".nx";
		string localizationKey3 = key + ".xbox";
		string localizationKey4 = key + ".mswin";
		string localizationKey5 = key + ".macos";
		string localizationKey6 = key + ".linux";
		string localizationKey7 = key + ".boothButton";
		string localizationKey8 = key + ".divekick";
		string localizationKey9 = key + ".gamepad";
		string localizationKey10 = key + ".steamdeck";
		exists = GetString(scnBase.isMobile, localizationKey);
		if (!exists)
		{
			exists = GetString(scnBase.isSwitch, localizationKey2);
		}
		if (!exists)
		{
			exists = GetString(scnBase.isXbox, localizationKey3);
		}
		if (!exists)
		{
			exists = GetString(scnBase.isWindows, localizationKey4);
		}
		if (!exists)
		{
			exists = GetString(scnBase.isMac, localizationKey5);
		}
		if (!exists)
		{
			exists = GetString(scnBase.isLinux, localizationKey6);
		}
		if (!exists)
		{
			exists = GetString(DebugSettings.instance.RunningOnSteamDeck, localizationKey10);
		}
		if (!exists)
		{
			exists = GetString(scnBase.isGamepad, localizationKey9);
		}
		if (!exists)
		{
			exists = GetString(scnBase.isCustomButton && scnBase.isDivekick, localizationKey8);
		}
		if (!exists)
		{
			exists = GetString(scnBase.isCustomButton, localizationKey7);
		}
		if (!exists)
		{
			exists = GetString(condition: true, key);
		}
		if (value == "RDNull")
		{
			exists = false;
		}
		int index;
		while (value.Has("[pal=", out index))
		{
			int num = index + "[pal=".Length;
			int num2 = value.IndexOf(']', index);
			int num3 = int.Parse(value.Substring(num, num2 - num));
			string text = RDConstants.data.hexPalette[num3];
			string value2 = "<color=" + text + ">";
			int num4 = num2 + 1;
			StringBuilder stringBuilder = new StringBuilder(value);
			stringBuilder.Remove(index, num4 - index);
			stringBuilder.Insert(index, value2);
			stringBuilder.Replace("[/pal]", "</color>");
			value = stringBuilder.ToString();
		}
		return value;
		bool GetString(bool condition, string token)
		{
			if (condition && Localization.ExistsLocalizedString(token))
			{
				value = Localization.GetLocalizedString(token);
				return true;
			}
			return false;
		}
	}

	public static string Reverse(this string s)
	{
		char[] array = s.ToCharArray();
		Array.Reverse(array);
		return new string(array);
	}

	public static void ChangeLanguage(SystemLanguage language, bool setLanguageForVoice = true)
	{
		RDConstants.data.forceLanguage = false;
		Persistence.SetLanguage(language);
		RDBaseDllDummy.printes("SetLanguage: " + language);
		if (setLanguageForVoice)
		{
			Persistence.SetVoiceLanguage(GetFallbackVoiceLanguage(language));
		}
		initialized = false;
		Setup();
	}

	public static void ChangeVoiceLanguage(SystemLanguage language)
	{
		voiceLanguage = ((!AvailableVoiceLanguages.Contains(language)) ? GetFallbackVoiceLanguage(language) : language);
		Persistence.SetVoiceLanguage(voiceLanguage);
	}

	public static string AddSpacesToChineseString(string s)
	{
		List<char> list = new List<char>(s.ToCharArray());
		int num;
		for (num = 0; num < list.Count - 1; num++)
		{
			char c = list[num];
			char c2 = list[num + 1];
			bool flag = !IsCharacterChinese(c) && !IsCharacterChinese(c2);
			if (!CharIsCJKPunctuation(c2) && !flag)
			{
				list.Insert(num + 1, ' ');
			}
			num++;
		}
		return new string(list.ToArray());
	}

	public static string AddSpacesToJapaneseString(string s)
	{
		List<char> list = new List<char>(s.ToCharArray());
		int num;
		for (num = 0; num < list.Count - 1; num++)
		{
			char c = list[num];
			char c2 = list[num + 1];
			bool flag = !IsCharacterJapanese(c) && !IsCharacterJapanese(c2);
			if (!CharIsCJKPunctuation(c2) && !flag)
			{
				list.Insert(num + 1, '\u00ad');
			}
			num++;
		}
		return new string(list.ToArray());
	}

	private static bool CharIsCJKPunctuation(char input)
	{
		return new uint[12]
		{
			65292u, 65281u, 65311u, 65307u, 65306u, 65288u, 65289u, 65339u, 65341u, 12304u,
			12305u, 12290u
		}.Contains(input);
	}

	public static FontPack GetAppropiateFontPackForString(string s)
	{
		RDConstants data = RDConstants.data;
		if (StringHasKoreanCharacters(s))
		{
			return data.koreanFontPack;
		}
		if (StringHasJapaneseCharacters(s))
		{
			return data.japaneseFontPack;
		}
		if (StringHasChineseCharacters(s))
		{
			return language switch
			{
				SystemLanguage.Japanese => data.japaneseFontPack, 
				SystemLanguage.ChineseSimplified => data.chineseSimpFontPack, 
				SystemLanguage.ChineseTraditional => data.chineseTradFontPack, 
				_ => data.chineseSimpFontPack, 
			};
		}
		FontPack latinFontPack = data.latinFontPack;
		latinFontPack.usePixelFont = !Persistence.GetReducePixelFontUsage() && LatinPixelFontSupports(s);
		return latinFontPack;
	}

	public static TMP_Asset GetAppropiateFontAssetForString(string s)
	{
		return GetAppropiateFontPackForString(s).fontAsset;
	}

	public static Font GetAppropiateFontForString(string s)
	{
		return GetAppropiateFontPackForString(s).font;
	}

	public static Font GetAppropiateFlashFontForString(string s)
	{
		return GetAppropiateFontPackForString(s).flashFont.font;
	}

	private static bool LatinPixelFontSupports(string s)
	{
		return s.All(LatinPixelFontSupports);
	}

	private static bool LatinPixelFontSupports(char c)
	{
		if (!RDConstants.data.latinFontPack.pixelFont.font.HasCharacter(c))
		{
			return char.IsWhiteSpace(c);
		}
		return true;
	}

	private static bool IsCharacterCJK(char c)
	{
		if (!IsCharacterChinese(c) && !IsCharacterJapanese(c))
		{
			return IsCharacterKorean(c);
		}
		return true;
	}

	private static bool IsCharacterChinese(char c)
	{
		if (!UnicodeRanges.CjkUnifiedIdeographs.Contains(c) && !UnicodeRanges.CjkUnifiedIdeographsExtensionA.Contains(c))
		{
			return UnicodeRanges.CjkCompatibilityIdeographs.Contains(c);
		}
		return true;
	}

	private static bool IsCharacterKorean(char c)
	{
		if (!UnicodeRanges.HangulCompatibilityJamo.Contains(c) && !UnicodeRanges.HangulSyllables.Contains(c) && !UnicodeRanges.HangulJamo.Contains(c) && !UnicodeRanges.HangulJamoExtendedA.Contains(c))
		{
			return UnicodeRanges.HangulJamoExtendedB.Contains(c);
		}
		return true;
	}

	private static bool IsCharacterJapanese(char c)
	{
		if (!UnicodeRanges.Katakana.Contains(c) && !UnicodeRanges.KatakanaPhoneticExtensions.Contains(c))
		{
			return UnicodeRanges.Hiragana.Contains(c);
		}
		return true;
	}

	public static bool StringHasCJKCharacters(string s)
	{
		return s.Any(IsCharacterCJK);
	}

	public static bool StringHasChineseCharacters(string s)
	{
		return s.Any(IsCharacterChinese);
	}

	public static bool StringHasKoreanCharacters(string s)
	{
		return s.Any(IsCharacterKorean);
	}

	public static bool StringHasJapaneseCharacters(string s)
	{
		return s.Any(IsCharacterJapanese);
	}

	public static void FetchLocalization(LocalizationSheet sheet, int? page = null)
	{
		bool flag = sheet == LocalizationSheet.Dialogues;
		string name = (flag ? "InkLocalization" : "RDLocalization");
		API.RetrievePublicSheetData(Settings.Instance.GetDocByName(name).ToString(), drawProgressBar: true, flag, page ?? (-1));
		if (!flag)
		{
			Localization.SetLanguage(Localization.CurrentLanguage);
		}
	}

	public static string GetNativeLanguageName(SystemLanguage language)
	{
		LangCode lang = RDUtils.ParseEnum(language.ToString(), LangCode.English);
		string text = string.Empty;
		foreach (LangSection value in Enum.GetValues(typeof(LangSection)))
		{
			text = Localization.GetLocalizedString("pauseMenu.myLanguage", value, lang);
			if (text != "pauseMenu.myLanguage")
			{
				break;
			}
		}
		return text;
	}
}
