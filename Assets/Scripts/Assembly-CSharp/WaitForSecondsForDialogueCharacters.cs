using UnityEngine;

public class WaitForSecondsForDialogueCharacters : CustomYieldInstruction
{
	private float t;

	private float delay;

	private float initialTime;

	public override bool keepWaiting
	{
		get
		{
			t += RDInk.deltaTimeForDialogues;
			if (t >= delay)
			{
				_ = Time.unscaledTime;
				_ = initialTime;
				return false;
			}
			return true;
		}
	}

	public WaitForSecondsForDialogueCharacters(float delay)
	{
		this.delay = delay;
	}
}
