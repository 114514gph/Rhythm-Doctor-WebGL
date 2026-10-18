namespace RDLevelEditor
{
	public class InspectorPanel_SetRoomPerspective : InspectorPanel
	{
		public override void UpdateUIProperties(LevelEvent_Base levelEvent)
		{
			LevelEvent_SetRoomPerspective levelEvent_SetRoomPerspective = (LevelEvent_SetRoomPerspective)levelEvent;
			foreach (Property property in properties)
			{
				if (property.name == "cornerPositions")
				{
					PropertyControl_SetRoomPerspective propertyControl_SetRoomPerspective = (PropertyControl_SetRoomPerspective)property.control;
					levelEvent_SetRoomPerspective.cornerPositionsUsed = propertyControl_SetRoomPerspective.positionPickersUsed;
				}
				property.UpdateUI(levelEvent);
			}
		}

		public override void SaveProperties(LevelEvent_Base levelEvent)
		{
			LevelEvent_SetRoomPerspective levelEvent_SetRoomPerspective = (LevelEvent_SetRoomPerspective)levelEvent;
			foreach (Property property in properties)
			{
				if (property.name == "cornerPositions")
				{
					PropertyControl_SetRoomPerspective propertyControl_SetRoomPerspective = (PropertyControl_SetRoomPerspective)property.control;
					levelEvent_SetRoomPerspective.cornerPositionsUsed = propertyControl_SetRoomPerspective.positionPickersUsed;
				}
				property.Save(levelEvent);
			}
		}
	}
}
