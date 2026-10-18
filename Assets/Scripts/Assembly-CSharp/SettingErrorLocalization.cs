using System;
using System.Collections.Generic;

[Serializable]
public class SettingErrorLocalization
{
	public string settingNameToken;

	public LevelSettingName settingName;

	public bool required;

	public List<SettingCondition> errors;

	public SettingErrorLocalization(string settingNameToken)
	{
		this.settingNameToken = settingNameToken;
		errors = new List<SettingCondition>();
	}
}
