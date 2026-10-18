namespace RDLevelEditor
{
	public class InspectorPanel_SetClapSounds : InspectorPanel
	{
		public override void UpdateUIProperties(LevelEvent_Base levelEvent)
		{
			LevelEvent_SetClapSounds levelEvent_SetClapSounds = (LevelEvent_SetClapSounds)levelEvent;
			foreach (Property property in properties)
			{
				if (property.name == "sound")
				{
					((PropertyControl_Sound)property.control).soundInput.SetMixerPath((levelEvent_SetClapSounds.rowType == RowType.Classic) ? "PlayerOneClapClassic" : "PlayerOneClapOneshot");
				}
				property.UpdateUI(levelEvent);
			}
		}

		public override void SaveProperties(LevelEvent_Base levelEvent)
		{
			LevelEvent_SetClapSounds levelEvent_SetClapSounds = (LevelEvent_SetClapSounds)levelEvent;
			foreach (Property property in properties)
			{
				if (property.name == "sound")
				{
					((PropertyControl_Sound)property.control).soundInput.SetMixerPath((levelEvent_SetClapSounds.rowType == RowType.Classic) ? "PlayerOneClapClassic" : "PlayerOneClapOneshot");
				}
				property.Save(levelEvent);
			}
		}
	}
}
