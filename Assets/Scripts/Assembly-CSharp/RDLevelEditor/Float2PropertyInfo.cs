using System.Reflection;
using UnityEngine;

namespace RDLevelEditor
{
	public record Float2PropertyInfo : BasePropertyInfo
	{
		public readonly Float2 min;

		public readonly Float2 max;

		public Float2PropertyInfo(PropertyInfo property)
			: base(property)
		{
			Float2InfoAttribute float2InfoAttribute = propertyInfo.GetCustomAttribute<Float2InfoAttribute>() ?? new Float2InfoAttribute();
			min = float2InfoAttribute.min;
			max = float2InfoAttribute.max;
		}

		public Float2 Clamp(Float2 value)
		{
			return new Float2(Mathf.Clamp(value.x, min.x, max.x), Mathf.Clamp(value.y, min.y, max.y));
		}

		public override object Decode(object value)
		{
			return RDEditorUtils.DecodeFloat2(value);
		}

		public override string Encode(string key, object value, bool isLastKey)
		{
			return RDEditorUtils.EncodeFloat2(key, (Float2)value, isLastKey);
		}
	}
}
