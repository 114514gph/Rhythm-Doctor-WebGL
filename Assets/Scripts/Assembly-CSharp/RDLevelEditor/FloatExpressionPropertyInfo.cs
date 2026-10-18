using System.Reflection;

namespace RDLevelEditor
{
	public record FloatExpressionPropertyInfo : BasePropertyInfo
	{
		public readonly float min;

		public readonly float max;

		public FloatExpressionPropertyInfo(PropertyInfo property)
			: base(property)
		{
			FloatInfoAttribute floatInfoAttribute = propertyInfo.GetCustomAttribute<FloatInfoAttribute>() ?? new FloatInfoAttribute();
			min = floatInfoAttribute.min;
			max = floatInfoAttribute.max;
		}

		public FloatExpression Clamp(FloatExpression value)
		{
			return value.Clamp(min, max);
		}

		public override object Decode(object value)
		{
			return Clamp(RDEditorUtils.DecodeFloatExpression(value));
		}

		public override string Encode(string key, object value, bool isLastKey)
		{
			return RDEditorUtils.EncodeFloatExpression(key, (FloatExpression)value, isLastKey);
		}
	}
}
