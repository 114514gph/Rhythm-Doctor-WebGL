using System;

public class scrExecuteOnCertainTime : RDExecuteOn
{
	public double actionTime;

	public bool isFromSongStart;

	public override void Update()
	{
		if (!removed && ((base.conductor.HasReachedVisual(actionTime) && !isFromSongStart) || (base.conductor.HasReachedVisualFromSongStart(actionTime) && isFromSongStart)))
		{
			action();
			Remove();
		}
	}

	private new void OnBeat()
	{
		action();
	}

	public static scrExecuteOnCertainTime Add(Action _onBeatAction, double _actionTime, bool isFromSongStart = false, bool divideBySpeed = false)
	{
		scrExecuteOnCertainTime scrExecuteOnCertainTime2 = new scrExecuteOnCertainTime();
		scrExecuteOnCertainTime2.action = _onBeatAction;
		scrExecuteOnCertainTime2.isFromSongStart = isFromSongStart;
		scrExecuteOnCertainTime2.actionTime = _actionTime;
		if (divideBySpeed)
		{
			scrExecuteOnCertainTime2.actionTime /= RDTime.speed;
		}
		return scrExecuteOnCertainTime2;
	}

	public static void Clear()
	{
		RDExecuteOn.ClearAll<scrExecuteOnCertainTime>();
	}
}
