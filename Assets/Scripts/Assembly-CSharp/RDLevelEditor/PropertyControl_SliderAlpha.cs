using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class PropertyControl_SliderAlpha : PropertyControl
	{
		public Slider slider;

		public InputField inputField;

		public override GameObject controlObject => slider.gameObject;

		private SliderAlphaAttribute controlAttribute => (SliderAlphaAttribute)base.propertyInfo.controlAttribute;

		public void UpdateSlider()
		{
			int num = int.Parse(inputField.text);
			slider.value = num;
			inputField.text = num.ToString();
		}

		public override void AddListeners(InspectorPanel.ChangeAction action)
		{
			IntPropertyInfo intPropertyInfo = (IntPropertyInfo)base.propertyInfo.NullableUnderlying;
			slider.minValue = intPropertyInfo.min;
			slider.maxValue = intPropertyInfo.max;
			if (slider.minValue <= -2.1474836E+09f)
			{
				slider.minValue = 0f;
			}
			if (slider.maxValue >= 2.1474836E+09f)
			{
				slider.maxValue = 100f;
			}
			slider.onValueChanged.AddListener((float value) =>
			{
				inputField.text = Convert.ToInt32(value).ToString();
				action(null);
			});
			RDEventTrigger orAddComponent = slider.GetOrAddComponent<RDEventTrigger>();
			orAddComponent.onClick = (RDPointerEventData)Delegate.Combine(orAddComponent.onClick, (RDPointerEventData)((PointerEventData _) =>
			{
				action(null, "sndSliderGrab");
			}));
			inputField.onEndEdit.AddListener((string _) =>
			{
				action(UpdateSlider, "sndEditorValueChange");
			});
		}

		public override void Setup()
		{
		}

		public override void UpdateUI(LevelEvent_Base levelEvent)
		{
			int num = Convert.ToInt32(GetEventValue(levelEvent));
			slider.value = num;
		}

		public override void Save(LevelEvent_Base levelEvent)
		{
			object value = Convert.ToInt32(slider.value);
			SetEventValue(levelEvent, value);
		}
	}
}
