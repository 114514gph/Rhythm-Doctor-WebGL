using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SA.GoogleDoc
{
	public class LocalizationClient
	{
		private LangCode langCode;

		private Dictionary<LangSection, Dictionary<string, string>> SheetDictionary = new Dictionary<LangSection, Dictionary<string, string>>();

		public LangCode CurrentLanguage => langCode;

		public void SetLanguage(LangCode lang)
		{
			langCode = lang;
			FillLocalizationDictionary(langCode);
		}

		public string GetLocalizedString(string token)
		{
			return GetLocalizedString(token, Localization.DefaultSection, langCode);
		}

		public string GetLocalizedString(string token, LangSection section)
		{
			return GetLocalizedString(token, section, langCode);
		}

		public string GetLocalizedString(string token, LangSection section, LangCode language)
		{
			if (!language.Equals(CurrentLanguage))
			{
				return GetUnparsedTokenValue(token, section, language);
			}
			if (!SheetDictionary.ContainsKey(section))
			{
				Debug.LogWarning("Localization: " + section.ToString() + " Section Not Found");
				return token + "_key_not_found";
			}
			if (!SheetDictionary[section].ContainsKey(token))
			{
				Debug.LogWarning("Localization: " + token + " Token Not Found under the " + section.ToString() + " Section");
				return token + "_key_not_found";
			}
			return SheetDictionary[section][token];
		}

		public bool ExistsLocalizedString(string token, LangSection section)
		{
			return SheetDictionary[section].ContainsKey(token);
		}

		public Dictionary<string, string> GetFullSectionLocalization(LangSection section)
		{
			if (!SheetDictionary.TryGetValue(section, out var value))
			{
				return new Dictionary<string, string>();
			}
			return value;
		}

		public void FillLocalizationDictionary(LangCode lang)
		{
			SheetDictionary = new Dictionary<LangSection, Dictionary<string, string>>();
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (LangSection value in Enum.GetValues(typeof(LangSection)))
			{
				if (!(Json.Deserialize(GetLocalizationJson(value)) is Dictionary<string, object> dictionary2))
				{
					continue;
				}
				foreach (KeyValuePair<string, object> item in dictionary2)
				{
					List<object> list = item.Value as List<object>;
					dictionary.Add(item.Key, list[(int)lang].ToString());
				}
			}
			SheetDictionary.Add(LangSection.Translations, dictionary);
		}

		private string GetUnparsedTokenValue(string token, LangSection section, LangCode lang)
		{
			if (Json.Deserialize(GetLocalizationJson(section)) is Dictionary<string, object> dictionary)
			{
				foreach (KeyValuePair<string, object> item in dictionary)
				{
					List<object> list = item.Value as List<object>;
					if (item.Key.Trim().Equals(token))
					{
						return list[(int)lang].ToString();
					}
				}
			}
			return token;
		}

		private string GetLocalizationJson(LangSection section)
		{
			string text = "Localization/Auto/";
			string externalLocalizationDataPath = Reader.externalLocalizationDataPath;
			string text2 = Path.Combine(externalLocalizationDataPath, $"{section}.json");
			string path = Path.Combine(externalLocalizationDataPath, "LastLocFetch.txt");
			if (File.Exists(text2) && File.Exists(path))
			{
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension("LastLocFetch.txt");
				string text3 = Resources.Load<TextAsset>(text + fileNameWithoutExtension).text;
				if (Reader.IsTimestampMoreRecent(File.ReadAllText(path), text3))
				{
					Debug.Log("found newer external local! " + text2);
					return File.ReadAllText(text2);
				}
			}
			TextAsset textAsset = Resources.Load<TextAsset>(text + $"{section}");
			if (textAsset == null)
			{
				return null;
			}
			return textAsset.text;
		}
	}
}
