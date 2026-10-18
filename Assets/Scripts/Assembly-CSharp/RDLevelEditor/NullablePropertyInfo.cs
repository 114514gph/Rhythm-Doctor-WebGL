using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace RDLevelEditor
{
	public record NullablePropertyInfo : BasePropertyInfo
	{
		public object toggleOnValue
		{
			[return: NotNull]
			get
			{
				if (_toggleOnValue == null)
				{
					_toggleOnValue = propertyInfo.GetValue(Activator.CreateInstance(propertyInfo.DeclaringType));
				}
				if (_toggleOnValue == null)
				{
					throw new Exception($"Event Automation: Field '{propertyInfo.Name}' of event '{propertyInfo.DeclaringType}' is of type " + "Nullable<>, but is initialized to null. If you would like to have it to be disabled by default in the UI, please add the [Off] attribute to the field and initialize it to whatever value you would like it to be set to when it is toggled on.");
				}
				return _toggleOnValue;
			}
		}

		[NotNull]
		public readonly BasePropertyInfo underlyingPropertyInfo;

		public readonly bool offByDefault;

		public object customToggleOnValue;

		private object _toggleOnValue;

		public void UpdateToggleOnValue(object val)
		{
			_toggleOnValue = val;
		}

		public NullablePropertyInfo(PropertyInfo property, Type underlyingType)
			: base(property)
		{
			underlyingPropertyInfo = BasePropertyInfo.FromProperty(property, underlyingType);
			offByDefault = propertyInfo.GetCustomAttribute<OffAttribute>() != null;
		}

		public override object Decode(object value)
		{
			if (value != null)
			{
				return underlyingPropertyInfo.Decode(value);
			}
			return null;
		}

		public override string Encode(string key, object rawValue, bool isLastKey)
		{
			if (rawValue == null)
			{
				return RDEditorUtils.EncodeKeyValue(key, "null", isLastKey);
			}
			return underlyingPropertyInfo.Encode(key, rawValue, isLastKey);
		}

		public override object MakeCopy(object rawSource)
		{
			if (rawSource == null)
			{
				return null;
			}
			return underlyingPropertyInfo.MakeCopy(rawSource);
		}

		public bool IsUsed(LevelEvent_Base levelEvent)
		{
			return propertyInfo.GetValue(levelEvent) != null;
		}

		[CompilerGenerated]
		protected override bool PrintMembers(StringBuilder builder)
		{
			if (base.PrintMembers(builder))
			{
				builder.Append(", ");
			}
			builder.Append("underlyingPropertyInfo = ");
			builder.Append(underlyingPropertyInfo);
			builder.Append(", offByDefault = ");
			builder.Append(offByDefault.ToString());
			builder.Append(", customToggleOnValue = ");
			builder.Append(customToggleOnValue);
			builder.Append(", toggleOnValue = ");
			builder.Append(toggleOnValue);
			return true;
		}
	}
}
