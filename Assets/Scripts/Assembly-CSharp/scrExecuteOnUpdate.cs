using System;

public class scrExecuteOnUpdate : RDExecuteOn
{
	public bool lateUpdate;

	public static bool printedThisFrame;

	public override void Update()
	{
		if (!removed && !lateUpdate && enabled)
		{
			action();
		}
	}

	public override void LateUpdate()
	{
		if (!removed && lateUpdate && enabled)
		{
			action();
		}
	}

	public static scrExecuteOnUpdate Add(Action action, bool lateUpdate = false)
	{
		return new scrExecuteOnUpdate
		{
			action = action,
			lateUpdate = lateUpdate
		};
	}

	public static void Clear()
	{
		RDExecuteOn.ClearAll<scrExecuteOnUpdate>();
	}
}
