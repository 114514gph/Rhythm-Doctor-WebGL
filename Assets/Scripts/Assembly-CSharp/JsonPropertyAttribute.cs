using System;
using System.Diagnostics.CodeAnalysis;

[AttributeUsage(AttributeTargets.Property)]
public class JsonPropertyAttribute : Attribute
{
	[NotNull]
	public readonly string name;

	[NotNull]
	public readonly string unit;

	[MaybeNull]
	public readonly string customLocalizationKey;

	[NotNull]
	public readonly string placeholder;

	public readonly bool required;

	public readonly bool enabled;

	[MaybeNull]
	public readonly string enableIfMethod;

	[MaybeNull]
	public readonly string saveIfMethod;

	[MaybeNull]
	public readonly string fieldUsed;

	public JsonPropertyAttribute(string name = "", string unit = "", string customLocalizationKey = null, string placeholder = "", bool required = false, bool enabled = true, string enableIfMethod = null, string saveIf = null)
	{
		this.name = name;
		this.unit = unit;
		this.customLocalizationKey = customLocalizationKey;
		this.placeholder = placeholder;
		this.required = required;
		this.enabled = enabled;
		this.enableIfMethod = enableIfMethod;
		saveIfMethod = saveIf;
	}
}
