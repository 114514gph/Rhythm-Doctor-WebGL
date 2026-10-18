using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class LevelErrorContainer : ScriptableObject
{
	public SettingErrorLocalization[] showableErrors;

	public List<LevelErrorName> GetErrorsToCheck()
	{
		List<LevelErrorName> list = new List<LevelErrorName>();
		SettingErrorLocalization[] array = showableErrors;
		foreach (SettingErrorLocalization settingErrorLocalization in array)
		{
			if (!settingErrorLocalization.required)
			{
				continue;
			}
			foreach (SettingCondition error in settingErrorLocalization.errors)
			{
				list.Add(error.name);
			}
		}
		return list;
	}
}
