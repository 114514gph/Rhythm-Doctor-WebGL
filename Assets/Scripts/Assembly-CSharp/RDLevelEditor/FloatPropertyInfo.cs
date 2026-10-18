using System.Reflection;
using UnityEngine;

namespace RDLevelEditor
{
	public record FloatPropertyInfo : BasePropertyInfo
	{
		public readonly float min;

		public readonly float max;

		public FloatPropertyInfo(PropertyInfo property)
			: base(property)
		{
			FloatInfoAttribute floatInfoAttribute = propertyInfo.GetCustomAttribute<FloatInfoAttribute>() ?? new FloatInfoAttribute();
			min = floatInfoAttribute.min;
			max = floatInfoAttribute.max;
		}

		public float Clamp(float value)
		{
			return Mathf.Clamp(value, min, max);
		}

		public override object Decode(object value)
		{
			return Clamp(RDEditorUtils.DecodeFloat(value));
		}

		public override string Encode(string key, object value, bool isLastKey)
		{
			return RDEditorUtils.EncodeFloat(key, (float)value, isLastKey);
		}
	}
}
