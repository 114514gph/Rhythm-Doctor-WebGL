using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class PropertyControl_Dropdown : PropertyControl
	{
		public Dropdown dropdown;

		private string[] values;

		public override GameObject controlObject => dropdown.gameObject;

		private DropdownAttribute controlAttribute => (DropdownAttribute)base.propertyInfo.controlAttribute;

		public string[] dropdownOptions => values;

		private Type enumType => ((EnumPropertyInfo)base.propertyInfo.NullableUnderlying)?.enumType;

		private bool isEase => enumType == typeof(Ease);

		public override void AddListeners(InspectorPanel.ChangeAction action)
		{
			if (base.propertyInfo.NullableUnderlying is EnumPropertyInfo)
			{
				if (isEase)
				{
					dropdown.AddOptionsWithEnumArray(RDEditorConstants.availableEases);
				}
				else
				{
					SetDropdownOptions(controlAttribute.options ?? Enum.GetNames(enumType));
				}
			}
			dropdown.onValueChanged.AddListener((int _) =>
			{
				action(null, "sndListClick");
			});
		}

		public override void Setup()
		{
		}

		public override void UpdateUI(LevelEvent_Base levelEvent)
		{
			object eventValue = GetEventValue(levelEvent);
			Dropdown obj = dropdown;
			BasePropertyInfo nullableUnderlying = base.propertyInfo.NullableUnderlying;
			int value;
			if (!(nullableUnderlying is EnumPropertyInfo))
			{
				if (!(nullableUnderlying is StringPropertyInfo))
				{
					if (!(nullableUnderlying is IntPropertyInfo))
					{
						throw new NotImplementedException();
					}
					value = Convert.ToInt32(eventValue);
				}
				else
				{
					value = Array.IndexOf(values, eventValue?.ToString());
				}
			}
			else
			{
				value = (isEase ? Array.IndexOf(RDEditorConstants.availableEases, (Ease)eventValue) : Array.IndexOf(values, eventValue.ToString()));
			}
			obj.value = value;
		}

		public override void Save(LevelEvent_Base levelEvent)
		{
			if (controlAttribute.firstOptionIsNull && dropdown.value == 0)
			{
				SetEventValue(levelEvent, null);
				return;
			}
			BasePropertyInfo nullableUnderlying = base.propertyInfo.NullableUnderlying;
			object obj;
			if (!(nullableUnderlying is EnumPropertyInfo))
			{
				if (!(nullableUnderlying is StringPropertyInfo))
				{
					if (!(nullableUnderlying is IntPropertyInfo))
					{
						throw new NotImplementedException();
					}
					obj = dropdown.value;
				}
				else
				{
					obj = values[dropdown.value];
				}
			}
			else
			{
				obj = (isEase ? ((object)RDEditorConstants.availableEases[dropdown.value]) : Enum.Parse(enumType, values[dropdown.value], ignoreCase: true));
			}
			object value = obj;
			SetEventValue(levelEvent, value);
		}

		public void SetDropdownOptions(string[] options, bool localize = true)
		{
			values = options;
			List<string> options2 = (localize ? (from object value in options
				select (!localize) ? value.ToString() : RDString.Get($"enum.{enumType.Name}.{value}")).ToList() : values.ToList());
			dropdown.ClearOptions();
			dropdown.AddOptions(options2);
		}

		public void SetDropdownOptions<T>(T[] options, bool localize = true)
		{
			string[] options2 = (from x in options.OfType<object>()
				select x.ToString()).ToArray();
			SetDropdownOptions(options2, localize);
		}
	}
}
