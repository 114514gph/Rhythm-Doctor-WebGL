using System.Reflection;

namespace RDLevelEditor
{
	public record StringPropertyInfo : BasePropertyInfo
	{
		public StringPropertyInfo(PropertyInfo property)
			: base(property)
		{
		}

		public override object Decode(object value)
		{
			return RDEditorUtils.DecodeString(value);
		}

		public override string Encode(string key, object value, bool isLastKey)
		{
			return RDEditorUtils.EncodeUnicodeString(key, (string)value, isLastKey);
		}
	}
}
