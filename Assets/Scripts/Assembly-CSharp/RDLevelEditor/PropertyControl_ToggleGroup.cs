using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class PropertyControl_ToggleGroup : PropertyControl
	{
		public ToggleGroup toggleGroup;

		private GridLayoutGroup grid;

		private string[] values;

		public override GameObject controlObject => toggleGroup.gameObject;

		private ToggleGroupAttribute controlAttribute => (ToggleGroupAttribute)base.propertyInfo.controlAttribute;

		public string[] toggleOptions => values;

		private Type enumType => ((EnumPropertyInfo)base.propertyInfo.NullableUnderlying)?.enumType;

		private bool isEnum => base.propertyInfo.NullableUnderlying is EnumPropertyInfo;

		private void Start()
		{
			UpdateGrid();
		}

		private void UpdateGrid()
		{
			float num = (float)((controlAttribute.multiline ? Property.propertySize : Property.controlSize) - 2) / 2f;
			int num2 = (int)Math.Ceiling((float)values.Length / 2f);
			int num3 = 14 * num2 + 2 * (num2 - 1);
			grid.cellSize = new Vector2(num, 14f);
			grid.constraintCount = num2;
			SetPropertyControlHeight(num3, updateControlObject: true, updateProperty: true);
			grid.enabled = true;
			Canvas.ForceUpdateCanvases();
			grid.enabled = false;
			if (values.Length % 2 != 0)
			{
				RectTransform component = toggleGroup.transform.GetChild(values.Length - 1).GetComponent<RectTransform>();
				component.localPosition = new Vector2(component.localPosition.x + component.sizeDelta.x / 2f, component.localPosition.y);
			}
		}

		public override void AddListeners(InspectorPanel.ChangeAction action)
		{
			GameObject toggleControlPrefab = RDConstants.data.toggleControlPrefab;
			Transform parent = toggleGroup.transform;
			grid = toggleGroup.GetComponent<GridLayoutGroup>();
			bool icons = controlAttribute.icons;
			string text = controlAttribute.customIcon ?? (isEnum ? enumType.Name : base.propertyInfo.name);
			string[] array = controlAttribute.options;
			if (array == null)
			{
				BasePropertyInfo nullableUnderlying = base.propertyInfo.NullableUnderlying;
				string[] array2;
				if (!(nullableUnderlying is EnumPropertyInfo))
				{
					if (!(nullableUnderlying is BoolPropertyInfo))
					{
						throw new NotImplementedException();
					}
					array2 = new string[2] { "off", "on" };
				}
				else
				{
					array2 = Enum.GetNames(enumType);
				}
				array = array2;
			}
			values = array;
			array = values;
			foreach (string text2 in array)
			{
				GameObject obj = UnityEngine.Object.Instantiate(toggleControlPrefab, parent, worldPositionStays: false);
				obj.name = text2;
				PropertyControl_Toggle component = obj.GetComponent<PropertyControl_Toggle>();
				component.toggle.group = toggleGroup;
				Image icon = component.icon;
				Text text3 = component.text;
				RDStringToUIText.Apply(text3);
				icon.gameObject.SetActive(icons);
				text3.gameObject.SetActive(!icons);
				if (icons)
				{
					Sprite sprite = Resources.Load<Sprite>("Editor/" + text + "/" + text + "_" + text2);
					if (sprite == null)
					{
						text3.gameObject.SetActive(value: true);
						icon.gameObject.SetActive(value: false);
						text3.text = RDString.Get("enum." + enumType.Name + "." + text2);
					}
					else
					{
						icon.sprite = sprite;
						icon.GetComponent<RectTransform>().sizeDelta = new Vector2(sprite.rect.width, sprite.rect.height);
					}
				}
				else if (!(base.propertyInfo.NullableUnderlying is IntPropertyInfo))
				{
					string text4 = controlAttribute.customLocalizationKey ?? ("enum." + (isEnum ? enumType.Name : "ToggleBool"));
					text3.text = RDString.Get(text4 + "." + text2);
				}
				InspectorPanel.RegisterToggle(component.toggle, action, null, "sndButtonClick");
			}
		}

		public override void Setup()
		{
		}

		public override void UpdateUI(LevelEvent_Base levelEvent)
		{
			object eventValue = GetEventValue(levelEvent);
			toggleGroup.SetValue((!(base.propertyInfo.NullableUnderlying is BoolPropertyInfo)) ? eventValue.ToString() : (((bool)eventValue) ? "on" : "off"));
		}

		public override void Save(LevelEvent_Base levelEvent)
		{
			BasePropertyInfo nullableUnderlying = base.propertyInfo.NullableUnderlying;
			object obj;
			if (!(nullableUnderlying is EnumPropertyInfo))
			{
				if (!(nullableUnderlying is IntPropertyInfo))
				{
					if (!(nullableUnderlying is BoolPropertyInfo))
					{
						throw new NotImplementedException();
					}
					obj = toggleGroup.GetValueFromActiveToggle() == "on";
				}
				else
				{
					obj = Convert.ToInt32(toggleGroup.GetValueFromActiveToggle());
				}
			}
			else
			{
				obj = Enum.Parse(enumType, toggleGroup.GetValueFromActiveToggle(), ignoreCase: true);
			}
			object value = obj;
			SetEventValue(levelEvent, value);
		}

		public void SetToggleOptions<T>(T[] options)
		{
			values = (from x in options.OfType<object>()
				select x.ToString()).ToArray();
			Transform transform = toggleGroup.transform;
			for (int num = 0; num < transform.childCount; num++)
			{
				bool flag = num < options.Length;
				PropertyControl_Toggle component = transform.GetChild(num).GetComponent<PropertyControl_Toggle>();
				component.gameObject.SetActive(flag);
				if (flag)
				{
					string text = controlAttribute.customLocalizationKey ?? ("enum." + (isEnum ? enumType.Name : "ToggleBool"));
					component.text.text = RDString.Get(text + "." + values[num]);
				}
			}
			UpdateGrid();
		}

		public void SetThemeOptions(int options, string customLocalizationKey)
		{
			values = new string[options];
			Transform transform = toggleGroup.transform;
			for (int i = 0; i < transform.childCount; i++)
			{
				bool flag = i < options;
				PropertyControl_Toggle component = transform.GetChild(i).GetComponent<PropertyControl_Toggle>();
				component.gameObject.SetActive(flag);
				if (flag)
				{
					string text = RDString.GetWithCheck($"{customLocalizationKey}.variant{i}", out var exists);
					if (!exists)
					{
						text = RDString.Get($"editor.SetTheme.defaultVariant{i}");
					}
					component.text.text = text;
					values[i] = text;
				}
			}
			UpdateGrid();
		}
	}
}
