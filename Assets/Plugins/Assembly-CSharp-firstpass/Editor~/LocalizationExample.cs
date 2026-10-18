using System;
using System.Collections.Generic;
using SA.GoogleDoc;
using UnityEngine;

public class LocalizationExample : MonoBehaviour
{
	private const string DOC_NAME = "Google Doc Connector Example";

	private const string LOCALIZATION_WORK_SHEET_NAME = "Localization";

	public GUIStyle GUITextStyle;

	private List<string> Tokens = new List<string>();

	private string str = "";

	private int _LocalizationDocIndex;

	private DocTemplate _LocalizationDoc;

	private List<string> _DetectedLanguages = new List<string>();

	private void Awake()
	{
		InitDocumentTemplate();
		foreach (KeyValuePair<string, string> item in Localization.GetFullSectionLocalization())
		{
			Tokens.Add(item.Key);
		}
		Localization.OnLanguageChanged += Localization_OnLanguageChanged;
	}

	private void OnGUI()
	{
		int num = 10;
		int num2 = 10;
		foreach (LangCode value in Enum.GetValues(typeof(LangCode)))
		{
			if (GUI.Button(new Rect(num, num2, 140f, 60f), value.ToString()))
			{
				Localization.SetLanguage(value);
				str = "";
				foreach (string token in Tokens)
				{
					str = str + token + " -> " + Localization.GetLocalizedString(token) + Environment.NewLine;
				}
				MonoBehaviour.print(str);
			}
			num += 170;
		}
		GUI.Label(new Rect(0f, 40f, 1000f, 1000f), str);
		if (GUI.Button(new Rect(400f, 300f, 100f, 30f), "Update from web"))
		{
			API.RetrievePublicSheetData(_LocalizationDoc);
		}
	}

	private void Localization_OnLanguageChanged()
	{
		Debug.Log("Localization Changed: " + Localization.CurrentLanguage);
	}

	private void InitDocumentTemplate()
	{
		_LocalizationDoc = Settings.Instance.GetDocByKey(LocalizationConfig.Instance.LocalizationDocKey);
		if (_LocalizationDoc != null)
		{
			_LocalizationDocIndex = Settings.Instance.GetDocIndexByKey(LocalizationConfig.Instance.LocalizationDocKey);
		}
	}

	private void OnListChanged()
	{
		InitLocalData();
		if (_LocalizationDoc == null)
		{
			return;
		}
		List<string> list = new List<string>();
		foreach (WorksheetTemplate worksheet in _LocalizationDoc.Worksheets)
		{
			list.Add(worksheet.listName);
		}
		bool flag = false;
		string[] names = Enum.GetNames(typeof(LangCode));
		foreach (string detectedLanguage in _DetectedLanguages)
		{
			MonoBehaviour.print("detected languages: " + detectedLanguage.ToString());
			flag = false;
			string[] array = names;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == detectedLanguage)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				break;
			}
		}
		if (!flag)
		{
			Debug.Log("ex db changed");
		}
		else
		{
			Debug.Log("ex enum didnt change");
		}
	}

	private void InitLocalData()
	{
		if (_LocalizationDocIndex == 0)
		{
			_LocalizationDoc = null;
			LocalizationConfig.Instance.LocalizationDocKey = string.Empty;
			_DetectedLanguages = new List<string>();
		}
		else
		{
			_LocalizationDoc = Settings.Instance.GetDocByIndex(_LocalizationDocIndex);
			LocalizationConfig.Instance.LocalizationDocKey = _LocalizationDoc.key;
			_DetectedLanguages = API.GetList<string>(_LocalizationDoc.name, new CellRange("B1"), _LocalizationDoc.Worksheets[0].listName);
		}
	}
}
