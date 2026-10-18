using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace RDLevelEditor
{
	public abstract record BasePropertyInfo
	{
		public BasePropertyInfo NullableUnderlying
		{
			get
			{
				if (!(this is NullablePropertyInfo nullablePropertyInfo))
				{
					return this;
				}
				return nullablePropertyInfo.underlyingPropertyInfo;
			}
		}

		[NotNull]
		public readonly string name;

		[NotNull]
		public readonly string unit;

		[NotNull]
		public readonly string customLocalizationKey;

		[NotNull]
		public readonly string placeholder;

		public readonly bool required;

		public readonly bool enabled;

		public readonly bool onlyUI;

		[MaybeNull]
		public readonly Func<LevelEvent_Base, bool> enableIf;

		[MaybeNull]
		public readonly Func<LevelEvent_Base, bool> saveIf;

		public readonly ControlAttribute controlAttribute;

		public readonly PropertyInfo propertyInfo;

		public abstract object Decode(object value);

		public abstract string Encode(string key, object value, bool isLastKey);

		public string EncodeWithoutKey(object value)
		{
			string text = Encode("", value, isLastKey: true);
			return text.Substring(4, text.Length - 4);
		}

		public virtual object MakeCopy(object source)
		{
			return source;
		}

		protected BasePropertyInfo(PropertyInfo property)
		{
			propertyInfo = property;
			JsonPropertyAttribute customAttribute = propertyInfo.GetCustomAttribute<JsonPropertyAttribute>();
			name = ((customAttribute.name == "") ? propertyInfo.Name : customAttribute.name);
			unit = customAttribute.unit;
			customLocalizationKey = customAttribute.customLocalizationKey;
			placeholder = customAttribute.placeholder;
			required = customAttribute.required;
			enabled = customAttribute.enabled;
			string enableIfMethod = customAttribute.enableIfMethod;
			if (enableIfMethod != null)
			{
				enableIf = GetEnableIfMethod(enableIfMethod, propertyInfo.DeclaringType);
			}
			string saveIfMethod = customAttribute.saveIfMethod;
			if (saveIfMethod != null)
			{
				saveIf = GetEnableIfMethod(saveIfMethod, propertyInfo.DeclaringType);
			}
			this.controlAttribute = propertyInfo.GetCustomAttributes<ControlAttribute>().SingleOrDefault();
			if (this.controlAttribute == null)
			{
				this.controlAttribute = GetDefaultControlAttribute(property);
			}
			ControlAttribute controlAttribute = this.controlAttribute;
			onlyUI = controlAttribute is DescriptionAttribute || controlAttribute is ButtonAttribute;
		}

		[return: NotNull]
		private static Func<LevelEvent_Base, bool> GetEnableIfMethod(string methodName, Type containingType)
		{
			if (methodName == null)
			{
				throw new ArgumentNullException("methodName");
			}
			if (containingType == null)
			{
				throw new ArgumentNullException("containingType");
			}
			MethodInfo methodInfo = containingType.GetMethod(methodName);
			if (methodInfo == null)
			{
				throw new Exception($"Cannot find method with name '{methodName}' in type '{containingType}'.");
			}
			if (methodInfo.GetParameters().Length != 0)
			{
				throw new Exception("Method '" + methodName + "' should have no parameters.");
			}
			if (methodInfo.ReturnType != typeof(bool))
			{
				throw new Exception("Method '" + methodName + "' should return bool.");
			}
			return (LevelEvent_Base e) => (bool)methodInfo.Invoke(e, new object[0]);
		}

		[return: NotNull]
		public static Action GetAction(string methodName, Type containingType, LevelEvent_Base currentLevelEvent)
		{
			if (methodName == null)
			{
				throw new ArgumentNullException("methodName");
			}
			if (containingType == null)
			{
				throw new ArgumentNullException("containingType");
			}
			MethodInfo method = containingType.GetMethod(methodName);
			if (method == null)
			{
				throw new Exception($"Cannot find method with name '{methodName}' in type '{containingType}'.");
			}
			if (method.ReturnType != typeof(void))
			{
				throw new Exception("Method '" + methodName + "' should return void.");
			}
			return (Action)Delegate.CreateDelegate(typeof(Action), currentLevelEvent, method);
		}

		[return: NotNull]
		public static BasePropertyInfo FromProperty(PropertyInfo property, Type typeOverride = null)
		{
			Type type = typeOverride ?? property.PropertyType;
			if (type == typeof(bool))
			{
				return new BoolPropertyInfo(property);
			}
			if (type == typeof(int))
			{
				return new IntPropertyInfo(property);
			}
			if (type == typeof(float))
			{
				return new FloatPropertyInfo(property);
			}
			if (type == typeof(string))
			{
				return new StringPropertyInfo(property);
			}
			if (type == typeof(Vector2))
			{
				return new Vector2PropertyInfo(property);
			}
			if (type == typeof(Float2))
			{
				return new Float2PropertyInfo(property);
			}
			if (type == typeof(FloatExpression))
			{
				return new FloatExpressionPropertyInfo(property);
			}
			if (type == typeof(FloatExpression2))
			{
				return new FloatExpression2PropertyInfo(property);
			}
			if (type == typeof(ColorOrPalette))
			{
				return new ColorPropertyInfo(property);
			}
			if (type == typeof(SoundDataStruct))
			{
				return new SoundDataPropertyInfo(property);
			}
			if (type.IsEnum)
			{
				return new EnumPropertyInfo(property, type);
			}
			if (type.IsArray)
			{
				Type elementType = type.GetElementType();
				return (BasePropertyInfo)Activator.CreateInstance(typeof(ArrayPropertyInfo<>).MakeGenericType(elementType), property);
			}
			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				Type underlyingType = Nullable.GetUnderlyingType(type);
				return new NullablePropertyInfo(property, underlyingType);
			}
			throw new Exception($"I have no idea how to serialize '{type}'!");
		}

		[return: NotNull]
		public static ControlAttribute GetDefaultControlAttribute(PropertyInfo field)
		{
			Type type = field.PropertyType;
			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				type = Nullable.GetUnderlyingType(type);
			}
			if (type == typeof(bool))
			{
				return new ToggleGroupAttribute();
			}
			if (type == typeof(int))
			{
				return new InputFieldAttribute();
			}
			if (type == typeof(float))
			{
				return new InputFieldAttribute();
			}
			if (type == typeof(string))
			{
				return new InputFieldAttribute();
			}
			if (type == typeof(Vector2))
			{
				return new InputFieldAttribute();
			}
			if (type == typeof(Float2))
			{
				return new PositionPickerAttribute();
			}
			if (type == typeof(FloatExpression))
			{
				return new InputFieldAttribute();
			}
			if (type == typeof(FloatExpression2))
			{
				return new ExpPositionPickerAttribute();
			}
			if (type == typeof(ColorOrPalette))
			{
				return new ColorAttribute();
			}
			if (type == typeof(SoundDataStruct))
			{
				return new SoundAttribute();
			}
			if (type.IsEnum)
			{
				return new DropdownAttribute();
			}
			if (type.IsArray)
			{
				return new DropdownAttribute();
			}
			throw new Exception($"I have no idea how to implement '{type}'!");
		}

		[CompilerGenerated]
		protected virtual bool PrintMembers(StringBuilder builder)
		{
			RuntimeHelpers.EnsureSufficientExecutionStack();
			builder.Append("name = ");
			builder.Append((object)name);
			builder.Append(", unit = ");
			builder.Append((object)unit);
			builder.Append(", customLocalizationKey = ");
			builder.Append((object)customLocalizationKey);
			builder.Append(", placeholder = ");
			builder.Append((object)placeholder);
			builder.Append(", required = ");
			builder.Append(required.ToString());
			builder.Append(", enabled = ");
			builder.Append(enabled.ToString());
			builder.Append(", onlyUI = ");
			builder.Append(onlyUI.ToString());
			builder.Append(", enableIf = ");
			builder.Append(enableIf);
			builder.Append(", saveIf = ");
			builder.Append(saveIf);
			builder.Append(", controlAttribute = ");
			builder.Append(controlAttribute);
			builder.Append(", propertyInfo = ");
			builder.Append(propertyInfo);
			builder.Append(", NullableUnderlying = ");
			builder.Append(NullableUnderlying);
			return true;
		}
	}
}
