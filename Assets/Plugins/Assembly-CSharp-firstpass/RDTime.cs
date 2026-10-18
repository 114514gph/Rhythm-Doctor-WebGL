using UnityEngine;

public static class RDTime
{
	public static float speed = 1f;

	public static int currentFrame = 0;

	public static bool fakePauseEverything;

	public static float deltaTimeIndependentPauseAffected
	{
		get
		{
			if (!AudioListener.pause && !fakePauseEverything)
			{
				return Time.unscaledDeltaTime;
			}
			return 0f;
		}
	}

	public static float deltaTimeWithMaximumSpeed
	{
		get
		{
			float result = Mathf.Max(1f, Time.timeScale) * Time.deltaTime;
			if (!AudioListener.pause && !fakePauseEverything)
			{
				return result;
			}
			return 0f;
		}
	}

	public static float unscaledDeltaTime
	{
		get
		{
			if (!fakePauseEverything)
			{
				return Time.unscaledDeltaTime;
			}
			return 0f;
		}
	}
}
