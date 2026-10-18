using System;
using System.Reflection;

namespace RDLevelEditor
{
	public record EnumPropertyInfo : BasePropertyInfo
	{
		public readonly Type enumType;

		public readonly bool asInt;

		public EnumPropertyInfo(PropertyInfo property, Type type)
			: base(property)
		{
			EnumInfoAttribute enumInfoAttribute = propertyInfo.GetCustomAttribute<EnumInfoAttribute>() ?? new EnumInfoAttribute();
			enumType = type;
			asInt = enumInfoAttribute.asInt;
		}

		public override object Decode(object value)
		{
			if (!asInt)
			{
				if (!Enum.TryParse(enumType, (string)value, ignoreCase: true, out var result))
				{
					return null;
				}
				return result;
			}
			return Enum.ToObject(enumType, RDEditorUtils.DecodeInt(value));
		}

		public override string Encode(string key, object value, bool isLastKey)
		{
			if (!asInt)
			{
				return RDEditorUtils.EncodeString(key, value.ToString(), isLastKey);
			}
			return RDEditorUtils.EncodeInt(key, (int)value, isLastKey);
		}
	}
}
