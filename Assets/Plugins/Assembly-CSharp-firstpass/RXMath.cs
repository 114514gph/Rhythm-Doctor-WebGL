using System;
using UnityEngine;

public class RXMath
{
	public const float RTOD = 180f / MathF.PI;

	public const float DTOR = MathF.PI / 180f;

	public const float DOUBLE_PI = MathF.PI * 2f;

	public const float HALF_PI = MathF.PI / 2f;

	public const float PI = MathF.PI;

	public const float INVERSE_PI = 1f / MathF.PI;

	public const float INVERSE_DOUBLE_PI = 1f / (2f * MathF.PI);

	public static int Wrap(int input, int range)
	{
		return (input + range * 1000000) % range;
	}

	public static float Wrap(float input, float range)
	{
		return (input + range * 1000000f) % range;
	}

	public static int Clamp(int n, int min, int max)
	{
		return Math.Max(Math.Min(n, max), min);
	}

	public static float InverseLerp(double a, double b, double t)
	{
		return (float)((t - a) / (b - a));
	}

	public static float GetDegreeDelta(float startAngle, float endAngle)
	{
		float num = (endAngle - startAngle) % 360f;
		if (num != num % 180f)
		{
			num = ((num < 0f) ? (num + 360f) : (num - 360f));
		}
		return num;
	}

	public static float GetRadianDelta(float startAngle, float endAngle)
	{
		float num = (endAngle - startAngle) % (MathF.PI * 2f);
		if (num != num % MathF.PI)
		{
			num = ((num < 0f) ? (num + MathF.PI * 2f) : (num - MathF.PI * 2f));
		}
		return num;
	}

	public static float PingPong(float input, float range)
	{
		float num = (input + range * 1000000f) % range / range;
		if (num < 0.5f)
		{
			return num * 2f;
		}
		return 1f - (num - 0.5f) * 2f;
	}

	public static float FlooredDivision(float a, float b)
	{
		return a - b * Mathf.Floor(a / b);
	}
}
