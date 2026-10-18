namespace RDLevelEditor
{
	public class InspectorPanel_SetRowXs : InspectorPanel
	{
		public override void UpdateUIProperties(LevelEvent_Base levelEvent)
		{
			LevelEvent_SetRowXs levelEvent_SetRowXs = (LevelEvent_SetRowXs)levelEvent;
			foreach (Property property in properties)
			{
				if (property.name == "pattern")
				{
					((PropertyControl_BeatModifiers)property.control).syncoBeat = levelEvent_SetRowXs.syncoBeat;
				}
				property.UpdateUI(levelEvent);
			}
		}

		public override void SaveProperties(LevelEvent_Base levelEvent)
		{
			LevelEvent_SetRowXs levelEvent_SetRowXs = (LevelEvent_SetRowXs)levelEvent;
			foreach (Property property in properties)
			{
				if (property.name == "pattern")
				{
					PropertyControl_BeatModifiers propertyControl_BeatModifiers = (PropertyControl_BeatModifiers)property.control;
					levelEvent_SetRowXs.syncoBeat = propertyControl_BeatModifiers.syncoBeat;
				}
				if (property.name == "syncoSwing")
				{
					((PropertyControl_Slider)property.control).snapInterval = 1f / (float)base.editor.denominator;
				}
				property.Save(levelEvent);
			}
			base.timeline.UpdateClassicControlsForRow(levelEvent_SetRowXs.row);
		}
	}
}
