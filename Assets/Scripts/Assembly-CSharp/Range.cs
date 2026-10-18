using UnityEngine;

public struct Range
{
	public float min;

	public float max;

	public Range(float value)
	{
		min = (max = value);
	}

	public Range(float min, float max)
	{
		this.min = min;
		this.max = max;
	}

	public float RandomValue()
	{
		return Random.Range(min, max);
	}
}
