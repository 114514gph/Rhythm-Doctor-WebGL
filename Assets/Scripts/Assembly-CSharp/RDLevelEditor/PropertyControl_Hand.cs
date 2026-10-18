using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class PropertyControl_Hand : PropertyControl
	{
		public ToggleGroup hand;

		public override GameObject controlObject => hand.gameObject;

		private HandAttribute controlAttribute => (HandAttribute)base.propertyInfo.controlAttribute;

		public override void AddListeners(InspectorPanel.ChangeAction action)
		{
			foreach (Transform item in hand.transform)
			{
				Toggle component = item.GetComponent<Toggle>();
				if (component != null)
				{
					InspectorPanel.RegisterToggle(component, action, null, "sndButtonClick");
				}
			}
		}

		public override void Setup()
		{
		}

		public override void UpdateUI(LevelEvent_Base levelEvent)
		{
			object eventValue = GetEventValue(levelEvent);
			hand.SetValue(eventValue.ToString().ToLower());
		}

		public override void Save(LevelEvent_Base levelEvent)
		{
			Hand enumValueFromActiveToggle = hand.GetEnumValueFromActiveToggle(Hand.Left);
			SetEventValue(levelEvent, enumValueFromActiveToggle);
		}
	}
}
