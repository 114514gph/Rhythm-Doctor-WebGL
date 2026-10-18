using System;

[AttributeUsage(AttributeTargets.Class)]
public class ConditionalInfoAttribute : Attribute
{
	public readonly bool isConstant;

	public ConditionalInfoAttribute(bool isConstant = false)
	{
		this.isConstant = isConstant;
	}
}
