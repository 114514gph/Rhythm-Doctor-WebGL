using System;

[AttributeUsage(AttributeTargets.Field)]
public class ListedModAttribute : Attribute
{
	public bool showDescription;

	public ListedModAttribute(bool hasDesc = true)
	{
		showDescription = hasDesc;
	}
}
