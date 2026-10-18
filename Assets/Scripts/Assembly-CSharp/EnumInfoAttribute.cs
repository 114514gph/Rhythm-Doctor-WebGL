using System;

[AttributeUsage(AttributeTargets.Property)]
public class EnumInfoAttribute : Attribute
{
	public readonly bool asInt;

	public EnumInfoAttribute(bool asInt = false)
	{
		this.asInt = asInt;
	}
}
