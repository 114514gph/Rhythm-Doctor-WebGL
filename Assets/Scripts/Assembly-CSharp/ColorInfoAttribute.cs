using System;

[AttributeUsage(AttributeTargets.Property)]
public class ColorInfoAttribute : Attribute
{
	public readonly bool hasAlpha;

	public ColorInfoAttribute(bool hasAlpha)
	{
		this.hasAlpha = hasAlpha;
	}
}
