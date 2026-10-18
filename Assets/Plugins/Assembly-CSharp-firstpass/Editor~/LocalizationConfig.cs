using UnityEngine;

namespace SA.GoogleDoc
{
	public class LocalizationConfig : ScriptableObject
	{
		public string LocalizationDocKey = string.Empty;

		public bool isLanguagesInfoOpen;

		public bool isWorksheetInfoOpen;

		public const string VERSION_NUMBER = "1.0";

		public const string DEBUG_LOG_PREFIX = "Localization: ";

		private const string ConfigAssetName = "LocalizationSettings";

		private const string ConfigAssetExtension = ".asset";

		private static LocalizationConfig instance;

		public static LocalizationConfig Instance
		{
			get
			{
				if (instance == null)
				{
					instance = Resources.Load("LocalizationSettings") as LocalizationConfig;
					if (instance == null)
					{
						instance = ScriptableObject.CreateInstance<LocalizationConfig>();
					}
				}
				return instance;
			}
		}

		public DocTemplate LocalizationDoc => Settings.Instance.GetDocByKey(LocalizationDocKey);
	}
}
