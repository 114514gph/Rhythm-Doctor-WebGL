using System.Collections;
using System.Collections.Generic;
using RDLevelEditor;
using UnityEngine;
using UnityEngine.UI;

public class LevelErrorPresentation : RDBase
{
	public LevelErrorContainer errorContainer;

	public Text errorsText;

	public RectTransform contentRect;

	public ScrollRect scrollRect;

	public bool showingInEditor;

	public string GetErrors(List<LevelErrorName> levelErrors, RDLevelSettings levelSettings, string mainRDLevelPath = null, bool showTitle = true)
	{
		string text = string.Empty;
		SettingErrorLocalization[] showableErrors = errorContainer.showableErrors;
		foreach (SettingErrorLocalization settingErrorLocalization in showableErrors)
		{
			if (!settingErrorLocalization.required)
			{
				continue;
			}
			bool flag = false;
			foreach (SettingCondition error in settingErrorLocalization.errors)
			{
				if (!levelErrors.Contains(error.name))
				{
					continue;
				}
				if (!flag)
				{
					flag = true;
					if (showTitle)
					{
						string text2 = RDString.Get(settingErrorLocalization.settingNameToken);
						text = text + text2 + ": \n";
					}
				}
				string value = string.Empty;
				if (error.type == LevelErrorType.FileNotFound)
				{
					if (error.name == LevelErrorName.PreviewImageFileNotFound)
					{
						value = levelSettings.previewImageName;
					}
					else if (error.name == LevelErrorName.SyringeIconFileNotFound)
					{
						value = levelSettings.syringeIconName;
					}
					else if (error.name == LevelErrorName.PreviewSongFileNotFound)
					{
						value = levelSettings.previewSongName;
					}
					else if (error.name == LevelErrorName.Separate2PLevelNotFound)
					{
						value = levelSettings.separate2PLevelFilename;
					}
				}
				else if (error.type == LevelErrorType.FileCorruption && (error.name == LevelErrorName.LevelDataNull || error.name == LevelErrorName.LevelSettingsNull || error.name == LevelErrorName.LevelRowsNull || error.name == LevelErrorName.LevelEventsNull))
				{
					value = mainRDLevelPath;
				}
				text = text + error.GetLocalized(value) + "\n";
			}
			if (flag)
			{
				text += "\n";
			}
		}
		return text;
	}

	public void ShowErrors(List<LevelErrorName> levelErrors, RDLevelSettings levelSettings, string mainRDLevelPath = null)
	{
		RDBaseDllDummy.printem($"level with name: {levelSettings.levelName} has {levelErrors.Count} errors");
		errorsText.text = GetErrors(levelErrors, levelSettings, mainRDLevelPath);
		errorsText.font = RDString.GetAppropiateFontForString(errorsText.text);
		float preferredHeight = LayoutUtility.GetPreferredHeight(errorsText.GetComponent<RectTransform>());
		contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, preferredHeight);
		StartCoroutine(ScrollToTop());
	}

	private IEnumerator ScrollToTop()
	{
		yield return new WaitForEndOfFrame();
		scrollRect.verticalNormalizedPosition = 1f;
	}
}
