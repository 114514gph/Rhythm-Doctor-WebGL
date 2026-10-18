using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class PropertyControl_BPMCalculator : PropertyControl
	{
		public Text beatsPerMinuteText;

		public InputField inputField;

		public BPMCalculator bpmCalculator;

		public override GameObject controlObject => base.gameObject;

		private BPMCalculatorAttribute controlAttribute => (BPMCalculatorAttribute)base.propertyInfo.controlAttribute;

		private FloatPropertyInfo floatPropertyInfo => (FloatPropertyInfo)base.propertyInfo;

		public override void AddListeners(InspectorPanel.ChangeAction action)
		{
			beatsPerMinuteText.text = RDString.Get("editor.beatsPerMinute");
			bpmCalculator.onBeatsPerMinuteUpdated = (double s) =>
			{
				inputField.text = s.ToString();
			};
			inputField.onEndEdit.AddListener((string _) =>
			{
				action(() =>
				{
					bpmCalculator.currentInspectorPanel.timeline.UpdateUI(updateControls: false);
				}, "sndEditorValueChange");
			});
		}

		public override void Setup()
		{
		}

		public override void UpdateUI(LevelEvent_Base levelEvent)
		{
			object eventValue = GetEventValue(levelEvent);
			inputField.text = eventValue.ToString();
		}

		public override void Save(LevelEvent_Base levelEvent)
		{
			object value = Mathf.Clamp(float.Parse(inputField.text), floatPropertyInfo.min, floatPropertyInfo.max);
			SetEventValue(levelEvent, value);
		}
	}
}
