using System;

[AttributeUsage(AttributeTargets.Property)]
public class Float2InfoAttribute : Attribute
{
	public readonly Float2 min;

	public readonly Float2 max;

	public Float2InfoAttribute(float minX = float.NegativeInfinity, float minY = float.NegativeInfinity, float maxX = float.PositiveInfinity, float maxY = float.PositiveInfinity)
	{
		min = new Float2(minX, minY);
		max = new Float2(maxX, maxY);
	}
}
