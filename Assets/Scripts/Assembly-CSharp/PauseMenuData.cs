using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PauseMenuData", menuName = "RhythmDoctor/PauseMenuData", order = 6)]
public class PauseMenuData : ScriptableObject
{
	public PauseMenuContentData[] contents;

	public PauseMenuModeData[] modes;

	private Dictionary<PauseContentName, PauseMenuContentData> pauseContentsDict;

	private Dictionary<PauseModeName, PauseMenuModeData> pauseModesDict;

	[NonSerialized]
	private bool isInitialized;

	public void Initialize()
	{
		if (isInitialized)
		{
			return;
		}
		pauseContentsDict = new Dictionary<PauseContentName, PauseMenuContentData>();
		PauseMenuContentData[] array = contents;
		foreach (PauseMenuContentData pauseMenuContentData in array)
		{
			if (!scnBase.isLinux || pauseMenuContentData.name != PauseContentName.WindowMovement)
			{
				pauseContentsDict.Add(pauseMenuContentData.name, pauseMenuContentData);
			}
		}
		pauseModesDict = new Dictionary<PauseModeName, PauseMenuModeData>();
		PauseMenuModeData[] array2 = modes;
		foreach (PauseMenuModeData pauseMenuModeData in array2)
		{
			pauseModesDict.Add(pauseMenuModeData.name, pauseMenuModeData);
		}
		isInitialized = true;
	}

	public List<PauseMenuContentData> GetModeContents(PauseModeName modeName)
	{
		PauseMenuModeData value = new PauseMenuModeData();
		if (pauseModesDict.TryGetValue(modeName, out value))
		{
			List<PauseMenuContentData> list = new List<PauseMenuContentData>();
			PauseContentName[] contentNames = value.contentNames;
			foreach (PauseContentName pauseContentName in contentNames)
			{
				foreach (PauseMenuContentData value2 in pauseContentsDict.Values)
				{
					if (pauseContentName == value2.name)
					{
						bool flag = true;
						if (pauseContentName == PauseContentName.BackgroundDimming)
						{
							flag = false;
						}
						if ((pauseContentName == PauseContentName.NarrationSettings || pauseContentName == PauseContentName.NarrationDelayed || pauseContentName == PauseContentName.RowInfoCuesGlossary) && !Narration.IsAvailable)
						{
							flag = false;
						}
						if (Application.productName == "Rhythm Doctor Editor" && (pauseContentName == PauseContentName.ReenterFirstTimeSetup || pauseContentName == PauseContentName.ResetCalibrationDialogue || pauseContentName == PauseContentName.Voices))
						{
							flag = false;
						}
						if (pauseContentName == PauseContentName.ForceSAPINarration && !scnBase.isWindows)
						{
							flag = false;
							RDBaseDllDummy.printem("Hiding SAPI option for non-Windows system");
						}
						if ((pauseContentName == PauseContentName.SpeechRate || pauseContentName == PauseContentName.SpeechVolume) && !scnBase.isMac && !scnBase.isLinux)
						{
							flag = false;
							RDBaseDllDummy.printem("Hiding Speech Rate and Volume settings for non-Mac, non-Linux system.");
						}
						if (flag)
						{
							list.Add(value2);
						}
						break;
					}
				}
			}
			return list;
		}
		Debug.LogError("No Pause Mode exists with the name: " + modeName);
		return null;
	}

	public PauseMenuModeData GetModeByName(PauseModeName modeName)
	{
		PauseMenuModeData value = new PauseMenuModeData();
		if (pauseModesDict.TryGetValue(modeName, out value))
		{
			return value;
		}
		Debug.LogError("No Pause Mode exists with the name: " + modeName);
		return null;
	}
}
