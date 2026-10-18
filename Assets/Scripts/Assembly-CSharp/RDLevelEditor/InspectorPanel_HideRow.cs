namespace RDLevelEditor
{
	public class InspectorPanel_HideRow : InspectorPanel
	{
		private TransitionType[] visibleOptions = new TransitionType[3]
		{
			TransitionType.Smooth,
			TransitionType.Instant,
			TransitionType.Full
		};

		private TransitionType[] defaultOptions = new TransitionType[2]
		{
			TransitionType.Smooth,
			TransitionType.Instant
		};

		public override void UpdateUIProperties(LevelEvent_Base levelEvent)
		{
			LevelEvent_HideRow levelEvent_HideRow = (LevelEvent_HideRow)levelEvent;
			foreach (Property property in properties)
			{
				if (property.name == "transition")
				{
					PropertyControl_ToggleGroup propertyControl_ToggleGroup = (PropertyControl_ToggleGroup)property.control;
					TransitionType[] array = ((levelEvent_HideRow.show == RowVisibilityMode.Visible) ? visibleOptions : defaultOptions);
					if (propertyControl_ToggleGroup.toggleOptions.Length != array.Length)
					{
						propertyControl_ToggleGroup.SetToggleOptions(array);
					}
				}
				property.UpdateUI(levelEvent);
			}
		}

		public override void SaveProperties(LevelEvent_Base levelEvent)
		{
			LevelEvent_HideRow levelEvent_HideRow = (LevelEvent_HideRow)levelEvent;
			foreach (Property property in properties)
			{
				if (property.name == "transition")
				{
					PropertyControl_ToggleGroup propertyControl_ToggleGroup = (PropertyControl_ToggleGroup)property.control;
					TransitionType[] array = ((levelEvent_HideRow.show == RowVisibilityMode.Visible) ? visibleOptions : defaultOptions);
					if (propertyControl_ToggleGroup.toggleOptions.Length != array.Length)
					{
						int indexFromActiveToggle = propertyControl_ToggleGroup.toggleGroup.GetIndexFromActiveToggle();
						propertyControl_ToggleGroup.SetToggleOptions(array);
						if (indexFromActiveToggle >= propertyControl_ToggleGroup.toggleOptions.Length)
						{
							propertyControl_ToggleGroup.toggleGroup.SetValue(array[0].ToString());
						}
					}
				}
				property.Save(levelEvent);
			}
		}
	}
}
