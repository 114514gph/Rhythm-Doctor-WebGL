namespace RDLevelEditor
{
	public class InspectorPanel_SetGameSound : InspectorPanel
	{
		public override void UpdateUIProperties(LevelEvent_Base levelEvent)
		{
			base.UpdateUIProperties(levelEvent);
			LevelEvent_SetGameSound levelEvent_SetGameSound = (LevelEvent_SetGameSound)levelEvent;
			foreach (Property property in properties)
			{
				if (property.name == "sounds")
				{
					((PropertyControl_SetGameSound)property.control).SetSoundType(levelEvent_SetGameSound.soundType, replaceValues: false);
				}
			}
		}

		public override void SaveProperties(LevelEvent_Base levelEvent)
		{
			base.SaveProperties(levelEvent);
			LevelEvent_SetGameSound levelEvent_SetGameSound = (LevelEvent_SetGameSound)levelEvent;
			foreach (Property property in properties)
			{
				if (property.name == "sounds")
				{
					((PropertyControl_SetGameSound)property.control).SetSoundType(levelEvent_SetGameSound.soundType);
				}
			}
		}
	}
}
