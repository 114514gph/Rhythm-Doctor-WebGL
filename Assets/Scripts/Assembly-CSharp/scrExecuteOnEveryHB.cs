using System;

public class scrExecuteOnEveryHB : RDExecuteOn
{
	public int rowID;

	public bool isActive;

	public static scrExecuteOnEveryHB Add(Action _onHeartBeatAction, int _rowID = -1)
	{
		return new scrExecuteOnEveryHB
		{
			action = _onHeartBeatAction,
			rowID = _rowID,
			isActive = true,
			removed = false
		};
	}

	public static void Clear()
	{
		RDExecuteOn.ClearAll<scrExecuteOnEveryHB>();
	}
}
