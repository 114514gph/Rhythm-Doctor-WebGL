using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RDAlphaLevelSelect : MonoBehaviour
{
	public Text levelName;

	public Toggle autoplayToggle;

	private Level[] mLevels;

	private int mLevelIndex;

	private void Start()
	{
		scnBase.currentLevelSelect = "scnAlphaLevelSelect";
		Array values = Enum.GetValues(typeof(Level));
		mLevels = values as Level[];
		List<Level> list = new List<Level>();
		Level[] array = mLevels;
		for (int i = 0; i < array.Length; i++)
		{
			Level item = array[i];
			if (item.ToString().StartsWith("Level_"))
			{
				list.Add(item);
			}
		}
		mLevels = list.ToArray();
		autoplayToggle.isOn = DebugSettings.instance.Auto;
		ShowLabel();
	}

	private void ShowLabel()
	{
		levelName.text = mLevels[mLevelIndex].ToString().Substring(6);
	}

	public void GoToPreviousLevel()
	{
		mLevelIndex--;
		if (mLevelIndex == -1)
		{
			mLevelIndex = mLevels.Length - 1;
		}
		ShowLabel();
	}

	public void GoToNextLevel()
	{
		mLevelIndex = (mLevelIndex + 1) % mLevels.Length;
		ShowLabel();
	}

	public void PlayLevel()
	{
		MonoBehaviour.print(mLevels[mLevelIndex].ToString());
		DebugSettings.instance.Auto = autoplayToggle.isOn;
		scnBase.GoToLevelWithEnum(mLevels[mLevelIndex]);
	}
}
