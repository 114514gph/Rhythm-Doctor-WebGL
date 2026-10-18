using System.Reflection;
using UnityEngine;

namespace RDLevelEditor
{
	public record FloatExpression2PropertyInfo : BasePropertyInfo
	{
		public readonly Vector2 min;

		public readonly Vector2 max;

		public FloatExpression2PropertyInfo(PropertyInfo property)
			: base(property)
		{
			Vector2InfoAttribute vector2InfoAttribute = propertyInfo.GetCustomAttribute<Vector2InfoAttribute>() ?? new Vector2InfoAttribute();
			min = vector2InfoAttribute.min;
			max = vector2InfoAttribute.max;
		}

		public FloatExpression2 Clamp(FloatExpression2 value)
		{
			return new FloatExpression2(value.x.Clamp(min.x, max.x), value.y.Clamp(min.y, max.y));
		}

		public override object Decode(object value)
		{
			return Clamp(RDEditorUtils.DecodeFloatExpression2(value));
		}

		public override string Encode(string key, object value, bool isLastKey)
		{
			return RDEditorUtils.EncodeFloatExpression2(key, (FloatExpression2)value, isLastKey);
		}
	}
}
