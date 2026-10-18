using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace RDLevelEditor
{
	public record ArrayPropertyInfo<T> : BasePropertyInfo
	{
		[NotNull]
		private readonly BasePropertyInfo arrayTypeProperty;

		public ArrayPropertyInfo(PropertyInfo property)
			: base(property)
		{
			arrayTypeProperty = BasePropertyInfo.FromProperty(property, typeof(T));
		}

		public override object Decode(object value)
		{
			return ((List<object>)value).Select((object elem) => (T)arrayTypeProperty.Decode(elem)).ToArray();
		}

		public override string Encode(string key, object value, bool isLastKey)
		{
			IEnumerable<string> values = ((T[])value).Select((T e) => arrayTypeProperty.EncodeWithoutKey(e));
			return RDEditorUtils.EncodeKeyValue(key, "[" + string.Join(", ", values) + "]", isLastKey);
		}

		public override object MakeCopy(object rawSource)
		{
			T[] obj = (T[])rawSource;
			T[] array = new T[obj.Length];
			obj.CopyTo(array, 0);
			return array;
		}
	}
}
