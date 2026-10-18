using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class PropertyControl_FlipScreen : PropertyControl
	{
		public ToggleGroup toggleGroup;

		public Toggle none;

		public Toggle horizontally;

		public Toggle vertically;

		public Toggle bothAxes;

		public override GameObject controlObject => toggleGroup.gameObject;

		private FlipScreenAttribute controlAttribute => (FlipScreenAttribute)base.propertyInfo.controlAttribute;

		public override void AddListeners(InspectorPanel.ChangeAction action)
		{
			InspectorPanel.RegisterToggle(none, action, null, "sndButtonClick");
			InspectorPanel.RegisterToggle(horizontally, action, null, "sndButtonClick");
			InspectorPanel.RegisterToggle(vertically, action, null, "sndButtonClick");
			InspectorPanel.RegisterToggle(bothAxes, action, null, "sndButtonClick");
		}

		public override void Setup()
		{
		}

		public override void UpdateUI(LevelEvent_Base levelEvent)
		{
			bool[] array = (GetEventValue(levelEvent) as IEnumerable<bool>).ToArray();
			none.isOn = !array[0] && !array[1];
			horizontally.isOn = array[0] && !array[1];
			vertically.isOn = !array[0] && array[1];
			bothAxes.isOn = array[0] && array[1];
		}

		public override void Save(LevelEvent_Base levelEvent)
		{
			bool[] value = new bool[2]
			{
				horizontally.isOn || bothAxes.isOn,
				vertically.isOn || bothAxes.isOn
			};
			SetEventValue(levelEvent, value);
		}
	}
}
