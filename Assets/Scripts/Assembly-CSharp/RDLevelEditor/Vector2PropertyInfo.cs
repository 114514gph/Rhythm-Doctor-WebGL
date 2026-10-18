using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace RDLevelEditor
{
	public record Vector2PropertyInfo : BasePropertyInfo
	{
		public readonly Vector2 min;

		public readonly Vector2 max;

		public Vector2PropertyInfo(PropertyInfo property)
			: base(property)
		{
			Vector2InfoAttribute vector2InfoAttribute = propertyInfo.GetCustomAttribute<Vector2InfoAttribute>() ?? new Vector2InfoAttribute();
			min = vector2InfoAttribute.min;
			max = vector2InfoAttribute.max;
		}

		public Vector2 Clamp(Vector2 value)
		{
			return new Vector2(Mathf.Clamp(value.x, min.x, max.x), Mathf.Clamp(value.y, min.y, max.y));
		}

		public override object Decode(object value)
		{
			List<object> obj = (List<object>)value;
			float x = Convert.ToSingle(obj[0]);
			float y = Convert.ToSingle(obj[1]);
			return Clamp(new Vector2(x, y));
		}

		public override string Encode(string key, object value, bool isLastKey)
		{
			return RDEditorUtils.EncodeVector2(key, (Vector2)value, isLastKey);
		}
	}
}
