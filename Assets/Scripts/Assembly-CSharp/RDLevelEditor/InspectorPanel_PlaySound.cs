namespace RDLevelEditor
{
	public class InspectorPanel_PlaySound : InspectorPanel
	{
		public override void UpdateUIProperties(LevelEvent_Base levelEvent)
		{
			LevelEvent_PlaySound levelEvent_PlaySound = (LevelEvent_PlaySound)levelEvent;
			foreach (Property property in properties)
			{
				if (property.name == "sound")
				{
					((PropertyControl_Sound)property.control).soundInput.SetMixerPath(levelEvent_PlaySound.GetMixerPath());
				}
				property.UpdateUI(levelEvent);
			}
		}

		public override void SaveProperties(LevelEvent_Base levelEvent)
		{
			LevelEvent_PlaySound levelEvent_PlaySound = (LevelEvent_PlaySound)levelEvent;
			foreach (Property property in properties)
			{
				property.Save(levelEvent);
			}
			foreach (Property property2 in properties)
			{
				if (property2.name == "sound")
				{
					((PropertyControl_Sound)property2.control).soundInput.SetMixerPath(levelEvent_PlaySound.GetMixerPath());
				}
			}
		}
	}
}
