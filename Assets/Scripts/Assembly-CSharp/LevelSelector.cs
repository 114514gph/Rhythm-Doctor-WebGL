using System;

public class LevelSelector
{
	public static Type GetLevelTypeFromEnum(Level ln)
	{
		return Type.GetType("Level_" + ln);
	}

	public static Level LevelFromString(string levelName)
	{
		if (string.IsNullOrEmpty(levelName))
		{
			return Level.None;
		}
		if (levelName.Contains("*internal*/"))
		{
			return Level.Custom;
		}
		if (!Enum.TryParse<Level>(levelName, ignoreCase: false, out var result))
		{
			result = Level.None;
			RDBaseDllDummy.printem("we had problems parsing " + levelName + ", returning " + result);
		}
		return result;
	}
}
