using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class PropertyControl_Slider : PropertyControl
	{
		public Slider slider;

		public InputField inputField;

		public Text unit;

		public RectTransform sliderDescriptions;

		[NonSerialized]
		public float snapInterval = 1f;

		private string[] values;

		public override GameObject controlObject => slider.gameObject;

		private SliderAttribute controlAttribute => (SliderAttribute)base.propertyInfo.controlAttribute;

		private Type enumType => ((EnumPropertyInfo)base.propertyInfo.NullableUnderlying)?.enumType;

		public override void AddListeners(InspectorPanel.ChangeAction action)
		{
			if (controlAttribute.unit != null)
			{
				unit.text = controlAttribute.unit;
				unit.gameObject.SetActive(value: true);
				inputField.gameObject.SetActive(value: true);
				RectTransform component = slider.GetComponent<RectTransform>();
				float num = -40f - (controlAttribute.expandToLeft ? controlAttribute.expandInputBy : 0f);
				component.offsetMax = new Vector2(num, component.offsetMax.y);
				sliderDescriptions.offsetMax = new Vector2(num, sliderDescriptions.offsetMax.y);
				if (controlAttribute.expandInputBy != 0f)
				{
					component = inputField.GetComponent<RectTransform>();
					component.anchoredPosition = new Vector2(component.anchoredPosition.x + (controlAttribute.expandToLeft ? 0f : controlAttribute.expandInputBy), component.anchoredPosition.y);
					component.sizeDelta = new Vector2(component.sizeDelta.x + controlAttribute.expandInputBy, component.sizeDelta.y);
				}
			}
			BasePropertyInfo nullableUnderlying = base.propertyInfo.NullableUnderlying;
			if (!(nullableUnderlying is EnumPropertyInfo))
			{
				if (!(nullableUnderlying is IntPropertyInfo intPropertyInfo))
				{
					if (nullableUnderlying is FloatPropertyInfo floatPropertyInfo)
					{
						slider.minValue = ((!float.IsNegativeInfinity(floatPropertyInfo.min)) ? floatPropertyInfo.min : 0f);
						slider.maxValue = ((!float.IsPositiveInfinity(floatPropertyInfo.max)) ? floatPropertyInfo.max : 1f);
						slider.wholeNumbers = false;
						inputField.contentType = InputField.ContentType.DecimalNumber;
					}
				}
				else
				{
					slider.minValue = ((intPropertyInfo.min != int.MinValue) ? intPropertyInfo.min : 0);
					slider.maxValue = ((intPropertyInfo.max != int.MaxValue) ? intPropertyInfo.max : 100);
					slider.wholeNumbers = true;
					inputField.contentType = InputField.ContentType.IntegerNumber;
				}
			}
			else
			{
				string text = controlAttribute.customIcon ?? enumType.Name;
				sliderDescriptions.gameObject.SetActive(value: true);
				values = Enum.GetNames(enumType);
				slider.minValue = 1f;
				slider.maxValue = values.Length;
				slider.wholeNumbers = true;
				for (int i = 0; i < values.Length; i++)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(RDConstants.data.sliderDescriptionControlPrefab, sliderDescriptions, worldPositionStays: false);
					string text2 = values[i];
					Sprite sprite = Resources.Load<Sprite>("Editor/" + text + "/" + text + "_" + text2);
					if (sprite != null)
					{
						Image componentInChildren = gameObject.GetComponentInChildren<Image>(includeInactive: true);
						componentInChildren.sprite = sprite;
						componentInChildren.preserveAspect = true;
						componentInChildren.gameObject.SetActive(value: true);
						RectTransform component2 = componentInChildren.GetComponent<RectTransform>();
						Vector2 anchorMin = (component2.anchorMax = ((i == 0) ? new Vector2(0f, 0.5f) : ((i == values.Length - 1) ? new Vector2(1f, 0.5f) : new Vector2(0.5f, 0.5f))));
						component2.anchorMin = anchorMin;
						component2.localPosition = new Vector2((i == 0) ? 4 : ((i == values.Length - 1) ? (-4) : 0), component2.localPosition.y);
					}
					else
					{
						Text component3 = gameObject.GetComponent<Text>();
						component3.text = RDString.Get("enum." + enumType.Name + "." + text2);
						component3.alignment = ((i == 0) ? TextAnchor.MiddleLeft : ((i == values.Length - 1) ? TextAnchor.MiddleRight : TextAnchor.MiddleCenter));
					}
				}
				SetPropertyControlHeight(28f, updateControlObject: false);
			}
			slider.onValueChanged.AddListener((float value) =>
			{
				float num2 = snapInterval;
				if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
				{
					num2 /= 2f;
				}
				value = Mathf.Round(value / num2) * num2;
				inputField.text = value.ToString();
				slider.SetValueWithoutNotify(value);
				action(null);
			});
			RDEventTrigger component4 = slider.GetComponent<RDEventTrigger>();
			component4.onClick = (RDPointerEventData)Delegate.Combine(component4.onClick, (RDPointerEventData)((PointerEventData _) =>
			{
				action(null, "sndSliderGrab");
			}));
			inputField.onEndEdit.AddListener((string value) =>
			{
				Slider obj = slider;
				BasePropertyInfo basePropertyInfo = base.propertyInfo;
				float valueWithoutNotify;
				if (!(basePropertyInfo is IntPropertyInfo))
				{
					if (!(basePropertyInfo is FloatPropertyInfo))
					{
						throw new NotImplementedException();
					}
					valueWithoutNotify = float.Parse(value);
				}
				else
				{
					valueWithoutNotify = Convert.ToInt32(value);
				}
				obj.SetValueWithoutNotify(valueWithoutNotify);
				inputField.text = slider.value.ToString();
				action(null, "sndEditorValueChange");
			});
		}

		public override void Setup()
		{
		}

		public override void UpdateUI(LevelEvent_Base levelEvent)
		{
			object eventValue = GetEventValue(levelEvent);
			Slider obj = slider;
			BasePropertyInfo nullableUnderlying = base.propertyInfo.NullableUnderlying;
			float valueWithoutNotify;
			if (!(nullableUnderlying is EnumPropertyInfo))
			{
				if (!(nullableUnderlying is IntPropertyInfo))
				{
					if (!(nullableUnderlying is FloatPropertyInfo))
					{
						throw new NotImplementedException();
					}
					valueWithoutNotify = (float)eventValue;
				}
				else
				{
					valueWithoutNotify = Convert.ToInt32(eventValue);
				}
			}
			else
			{
				valueWithoutNotify = Array.IndexOf(values, eventValue.ToString()) + 1;
			}
			obj.SetValueWithoutNotify(valueWithoutNotify);
			if (controlAttribute.unit != null)
			{
				inputField.text = slider.value.ToString();
			}
		}

		public override void Save(LevelEvent_Base levelEvent)
		{
			BasePropertyInfo nullableUnderlying = base.propertyInfo.NullableUnderlying;
			object obj;
			if (!(nullableUnderlying is EnumPropertyInfo))
			{
				if (!(nullableUnderlying is IntPropertyInfo))
				{
					if (!(nullableUnderlying is FloatPropertyInfo))
					{
						throw new NotImplementedException();
					}
					obj = slider.value;
				}
				else
				{
					obj = Convert.ToInt32(slider.value);
				}
			}
			else
			{
				obj = Enum.Parse(enumType, values[Convert.ToInt32(slider.value) - 1], ignoreCase: true);
			}
			object value = obj;
			SetEventValue(levelEvent, value);
		}

		public void UpdateBounds(float min, float max)
		{
			slider.minValue = min;
			slider.maxValue = max;
		}
	}
}
