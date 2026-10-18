using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class PropertyControl_PositionPicker : PropertyControl
	{
		public PositionPicker positionPicker;

		public override GameObject controlObject => positionPicker.gameObject;

		private PositionPickerAttribute controlAttribute => (PositionPickerAttribute)base.propertyInfo.controlAttribute;

		public override void AddListeners(InspectorPanel.ChangeAction action)
		{
			PositionPickerAttribute positionPickerAttribute = (PositionPickerAttribute)base.propertyInfo.controlAttribute;
			positionPicker.crosshair.SetActive(positionPickerAttribute.crosshair);
			positionPicker.units.text = positionPickerAttribute.units ?? positionPicker.units.text;
			positionPicker.units.gameObject.SetActive(positionPickerAttribute.switchUnits || positionPickerAttribute.units != null);
			positionPicker.units.GetComponent<Button>().interactable = positionPickerAttribute.switchUnits && positionPickerAttribute.units == null;
			positionPicker.x.onEndEdit.AddListener((string _) =>
			{
				action(null, positionPicker.PlayOnEndEdit ? "sndEditorValueChange" : null);
			});
			positionPicker.y.onEndEdit.AddListener((string _) =>
			{
				action(null, positionPicker.PlayOnEndEdit ? "sndEditorValueChange" : null);
			});
		}

		public override void Setup()
		{
		}

		public override void UpdateUI(LevelEvent_Base levelEvent)
		{
			object eventValue = GetEventValue(levelEvent);
			positionPicker.percent = (Float2)eventValue;
		}

		public override void Save(LevelEvent_Base levelEvent)
		{
			SetEventValue(levelEvent, positionPicker.percent);
		}
	}
}
