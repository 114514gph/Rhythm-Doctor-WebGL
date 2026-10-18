namespace RDLevelEditor
{
	public class InspectorPanel_PlaySong : InspectorPanel
	{
		public override void UpdateUIProperties(LevelEvent_Base levelEvent)
		{
			foreach (Property property in properties)
			{
				if (property.name == "song")
				{
					LevelEvent_PlaySong levelEvent_PlaySong = (LevelEvent_PlaySong)levelEvent;
					if (levelEvent_PlaySong.forceOGGConvert)
					{
						levelEvent_PlaySong.forceOGGConvert = false;
						((PropertyControl_Sound)property.control).soundInput.ForceConversionPopup(levelEvent, levelEvent_PlaySong.song);
					}
				}
				property.UpdateUI(levelEvent);
			}
		}
	}
}
