using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace RDLevelEditor
{
	public record ConditionalInfo
	{
		public readonly string name;

		public readonly ConditionalInfoAttribute attribute;

		public readonly ImmutableDictionary<string, BasePropertyInfo> propertiesInfo;

		public ConditionalInfo(Type eventType)
		{
			attribute = eventType.GetCustomAttribute<ConditionalInfoAttribute>();
			if (attribute == null)
			{
				throw new Exception(string.Format("Conditional '{0}' does not have an '{1}' attribute.", eventType, "ConditionalInfoAttribute"));
			}
			string text = eventType.ToString();
			int length = "RDLevelEditor.Conditional_".Length;
			name = text.Substring(length, text.Length - length);
			propertiesInfo = (from propertyInfo in eventType.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public)
				where propertyInfo.IsDefined(typeof(JsonPropertyAttribute))
				select BasePropertyInfo.FromProperty(propertyInfo)).ToImmutableDictionary((BasePropertyInfo i) => i.name, (BasePropertyInfo i) => i);
		}
	}
}
