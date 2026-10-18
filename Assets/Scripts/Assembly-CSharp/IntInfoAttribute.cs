using System;

[AttributeUsage(AttributeTargets.Property)]
public class IntInfoAttribute : Attribute
{
	public readonly int min;

	public readonly int max;

	public IntInfoAttribute(int min = int.MinValue, int max = int.MaxValue)
	{
		this.min = min;
		this.max = max;
	}
}
