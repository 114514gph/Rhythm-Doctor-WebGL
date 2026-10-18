using System;

public class scrExecuteOnEveryBeat : RDExecuteOn
{
	public override void OnBeat()
	{
		if (!removed)
		{
			action();
		}
	}

	public static scrExecuteOnEveryBeat Add(Action _onBeatAction)
	{
		return new scrExecuteOnEveryBeat
		{
			action = _onBeatAction
		};
	}

	public static void Clear()
	{
		RDExecuteOn.ClearAll<scrExecuteOnEveryBeat>();
	}
}
