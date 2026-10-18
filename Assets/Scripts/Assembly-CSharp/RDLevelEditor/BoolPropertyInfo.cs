using System.Reflection;

namespace RDLevelEditor
{
	public record BoolPropertyInfo : BasePropertyInfo
	{
		public BoolPropertyInfo(PropertyInfo property)
			: base(property)
		{
		}

		public override object Decode(object value)
		{
			return RDEditorUtils.DecodeBool(value);
		}

		public override string Encode(string key, object value, bool isLastKey)
		{
			return RDEditorUtils.EncodeBool(key, (bool)value, isLastKey);
		}
	}
}
