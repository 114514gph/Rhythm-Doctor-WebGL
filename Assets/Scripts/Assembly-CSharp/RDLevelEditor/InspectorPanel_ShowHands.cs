namespace RDLevelEditor
{
	public class InspectorPanel_ShowHands : InspectorPanel
	{
		private readonly HandAction[] defaultHandActions = new HandAction[2]
		{
			HandAction.Show,
			HandAction.Hide
		};

		private readonly HandAction[] bothHandActions = new HandAction[4]
		{
			HandAction.Show,
			HandAction.Hide,
			HandAction.Raise,
			HandAction.Lower
		};

		public override void UpdateUIProperties(LevelEvent_Base levelEvent)
		{
			LevelEvent_ShowHands levelEvent_ShowHands = (LevelEvent_ShowHands)levelEvent;
			foreach (Property property in properties)
			{
				if (property.name == "action")
				{
					PropertyControl_Dropdown propertyControl_Dropdown = (PropertyControl_Dropdown)property.control;
					HandAction[] array = ((levelEvent_ShowHands.hand == Hand.Both) ? bothHandActions : defaultHandActions);
					if (propertyControl_Dropdown.dropdownOptions.Length != array.Length)
					{
						propertyControl_Dropdown.SetDropdownOptions(array);
					}
				}
				property.UpdateUI(levelEvent);
			}
		}

		public override void SaveProperties(LevelEvent_Base levelEvent)
		{
			LevelEvent_ShowHands levelEvent_ShowHands = (LevelEvent_ShowHands)levelEvent;
			foreach (Property property in properties)
			{
				if (property.name == "action")
				{
					PropertyControl_Dropdown propertyControl_Dropdown = (PropertyControl_Dropdown)property.control;
					HandAction[] array = ((levelEvent_ShowHands.hand == Hand.Both) ? bothHandActions : defaultHandActions);
					if (propertyControl_Dropdown.dropdownOptions.Length != array.Length)
					{
						int value = propertyControl_Dropdown.dropdown.value;
						propertyControl_Dropdown.SetDropdownOptions(array);
						if (value < array.Length)
						{
							propertyControl_Dropdown.dropdown.value = value;
						}
					}
				}
				property.Save(levelEvent);
			}
		}
	}
}
