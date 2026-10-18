using System.Reflection;

namespace RDLevelEditor
{
	public record ColorPropertyInfo : BasePropertyInfo
	{
		public readonly bool hasAlpha = true;

		public ColorPropertyInfo(PropertyInfo property)
			: base(property)
		{
			ColorInfoAttribute customAttribute = propertyInfo.GetCustomAttribute<ColorInfoAttribute>();
			if (customAttribute != null)
			{
				hasAlpha = customAttribute.hasAlpha;
			}
		}

		public override object Decode(object value)
		{
			return ColorOrPalette.FromString((string)value);
		}

		public override string Encode(string key, object value, bool isLastKey)
		{
			return RDEditorUtils.EncodeString(key, ((ColorOrPalette)value).Encode(hasAlpha), isLastKey);
		}
	}
}
