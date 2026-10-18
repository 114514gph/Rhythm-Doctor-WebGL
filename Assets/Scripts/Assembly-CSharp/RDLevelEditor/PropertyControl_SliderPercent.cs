using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class PropertyControl_SliderPercent : PropertyControl
	{
		public Slider slider;

		public InputField inputField;

		public override GameObject controlObject => slider.gameObject;

		private ButtonAttribute controlAttribute => (ButtonAttribute)base.propertyInfo.controlAttribute;

		public void UpdateSlider()
		{
			float value = float.Parse(inputField.text);
			slider.value = value;
			inputField.text = value.ToString();
		}

		public override void AddListeners(InspectorPanel.ChangeAction action)
		{
			FloatPropertyInfo floatPropertyInfo = (FloatPropertyInfo)base.propertyInfo.NullableUnderlying;
			slider.minValue = ((!float.IsNegativeInfinity(floatPropertyInfo.min)) ? floatPropertyInfo.min : 0f);
			slider.maxValue = ((!float.IsPositiveInfinity(floatPropertyInfo.max)) ? floatPropertyInfo.max : 100f);
			slider.onValueChanged.AddListener((float value) =>
			{
				inputField.text = value.ToString();
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
			float num = (float)GetEventValue(levelEvent);
			slider.value = num * 100f;
		}

		public override void Save(LevelEvent_Base levelEvent)
		{
			object value = slider.value / 100f;
			SetEventValue(levelEvent, value);
		}
	}
}
