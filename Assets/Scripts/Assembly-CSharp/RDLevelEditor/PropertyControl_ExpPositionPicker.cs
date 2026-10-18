using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class PropertyControl_ExpPositionPicker : PropertyControl
	{
		public ExpPositionPicker expPositionPicker;

		private bool percent;

		public override GameObject controlObject => expPositionPicker.gameObject;

		private ExpPositionPickerAttribute controlAttribute => (ExpPositionPickerAttribute)base.propertyInfo.controlAttribute;

		public override void AddListeners(InspectorPanel.ChangeAction action)
		{
			expPositionPicker.crosshair.SetActive(controlAttribute.crosshair);
			percent = controlAttribute.percent;
			expPositionPicker.units.GetComponent<Button>().enabled = !percent;
			expPositionPicker.x.onEndEdit.AddListener((string _) =>
			{
				action(null, expPositionPicker.PlayOnEndEdit ? "sndEditorValueChange" : null);
			});
			expPositionPicker.y.onEndEdit.AddListener((string _) =>
			{
				action(null, expPositionPicker.PlayOnEndEdit ? "sndEditorValueChange" : null);
			});
		}

		public override void Setup()
		{
		}

		public override void UpdateUI(LevelEvent_Base levelEvent)
		{
			FloatExpression2 floatExpression = (FloatExpression2)GetEventValue(levelEvent) * ((!percent) ? 1 : 100);
			expPositionPicker.percent = floatExpression;
		}

		public override void Save(LevelEvent_Base levelEvent)
		{
			FloatExpression2 floatExpression = ((FloatExpression2PropertyInfo)base.propertyInfo.NullableUnderlying).Clamp(expPositionPicker.percent / ((!percent) ? 1 : 100));
			expPositionPicker.percent = floatExpression * ((!percent) ? 1 : 100);
			SetEventValue(levelEvent, floatExpression);
		}
	}
}
