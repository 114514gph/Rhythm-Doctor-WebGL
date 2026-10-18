using System;

[AttributeUsage(AttributeTargets.Method)]
public class ListedMethodAttribute : Attribute
{
	public bool showDescription;

	public ListedMethodAttribute(bool hasDesc = true)
	{
		showDescription = hasDesc;
	}
}
