using System;
using System.Collections.Generic;
using UnityEngine;

namespace SA.GoogleDoc
{
	public class Localization
	{
		public static readonly List<SystemLanguage> AvailableLanguages = new List<SystemLanguage>
		{
			SystemLanguage.English,
			SystemLanguage.Spanish,
			SystemLanguage.Portuguese,
			SystemLanguage.ChineseSimplified,
			SystemLanguage.ChineseTraditional,
			SystemLanguage.Korean,
			SystemLanguage.Polish,
			SystemLanguage.Japanese,
			SystemLanguage.German,
			SystemLanguage.French
		};

		public static readonly List<SystemLanguage> AlphaOnlyLanguages = new List<SystemLanguage>();

		public static LocalizationClient Client = new LocalizationClient();

		public static LangCode CurrentLanguage => Client.CurrentLanguage;

		public static LangCode DefaultLanguage => LangCode.English;

		public static LangSection DefaultSection => LangSection.Translations;

		public static event Action OnLanguageChanged = () =>
		{
		};

		public static void SetLanguage(LangCode lang)
		{
			Client.SetLanguage(lang);
			OnLanguageChanged();
		}

		public static string GetLocalizedString(string token)
		{
			return Client.GetLocalizedString(token);
		}

		public static string GetLocalizedString(string token, LangSection section)
		{
			return Client.GetLocalizedString(token, section);
		}

		public static string GetLocalizedString(string token, LangSection section, LangCode lang)
		{
			return Client.GetLocalizedString(token, section, lang);
		}

		public static Dictionary<string, string> GetFullSectionLocalization()
		{
			return GetFullSectionLocalization(DefaultSection);
		}

		public static Dictionary<string, string> GetFullSectionLocalization(LangSection section)
		{
			return Client.GetFullSectionLocalization(section);
		}

		public static bool ExistsLocalizedString(string token)
		{
			return Client.ExistsLocalizedString(token, LangSection.Translations);
		}
	}
}
