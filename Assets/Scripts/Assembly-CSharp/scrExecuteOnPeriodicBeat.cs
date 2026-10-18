using System;

public class scrExecuteOnPeriodicBeat : RDExecuteOn
{
	private float frequency;

	private double timeToExecute;

	public bool active = true;

	public override void Update()
	{
		if (base.conductor.visualPos > timeToExecute)
		{
			if (active && !removed)
			{
				action();
			}
			timeToExecute += frequency * base.conductor.crotchet;
		}
	}

	public static scrExecuteOnPeriodicBeat Add(Action onBeatAction, float frequency, float beatNumber = 0f)
	{
		scrConductor instance = scrConductor.instance;
		bool flag = beatNumber + 1f >= (float)instance.crotchetsPerBar && !instance.isScrubbingRefreshOnBar;
		double num = (flag ? instance.startOfNextBarRefreshOnlyOnBar : instance.startOfNextBar);
		return new scrExecuteOnPeriodicBeat
		{
			action = onBeatAction,
			timeToExecute = (double)instance.BeatToTime(beatNumber, flag) + num,
			frequency = frequency
		};
	}

	public static void Clear()
	{
		RDExecuteOn.ClearAll<scrExecuteOnPeriodicBeat>();
	}
}
