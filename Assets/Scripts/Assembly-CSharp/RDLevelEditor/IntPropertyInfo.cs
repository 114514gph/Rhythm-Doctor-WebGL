using System.Reflection;

namespace RDLevelEditor
{
	public record IntPropertyInfo : BasePropertyInfo
	{
		public readonly int min;

		public readonly int max;

		public IntPropertyInfo(PropertyInfo property)
			: base(property)
		{
			IntInfoAttribute intInfoAttribute = propertyInfo.GetCustomAttribute<IntInfoAttribute>() ?? new IntInfoAttribute();
			min = intInfoAttribute.min;
			max = intInfoAttribute.max;
		}

		public override object Decode(object value)
		{
			return RDEditorUtils.DecodeInt(value);
		}

		public override string Encode(string key, object value, bool isLastKey)
		{
			return RDEditorUtils.EncodeInt(key, (int)value, isLastKey);
		}
	}
}
