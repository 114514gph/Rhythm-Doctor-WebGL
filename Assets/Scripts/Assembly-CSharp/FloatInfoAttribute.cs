using System;

[AttributeUsage(AttributeTargets.Property)]
public class FloatInfoAttribute : Attribute
{
	public readonly float min;

	public readonly float max;

	public FloatInfoAttribute(float min = float.NegativeInfinity, float max = float.PositiveInfinity)
	{
		this.min = min;
		this.max = max;
	}
}
