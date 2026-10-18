using System.Collections.Generic;
using System.Reflection;

namespace RDLevelEditor
{
	public record SoundDataPropertyInfo : BasePropertyInfo
	{
		public SoundDataPropertyInfo(PropertyInfo property)
			: base(property)
		{
		}

		public override object Decode(object value)
		{
			return SoundDataStruct.Decode((Dictionary<string, object>)value).Validated();
		}

		public override string Encode(string key, object value, bool isLastKey)
		{
			return ((SoundDataStruct)value).Encode(key, isLastKey);
		}
	}
}
