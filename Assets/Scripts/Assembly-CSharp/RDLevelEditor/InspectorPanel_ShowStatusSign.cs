namespace RDLevelEditor
{
	public class InspectorPanel_ShowStatusSign : InspectorPanel
	{
		public override void UpdateUIProperties(LevelEvent_Base levelEvent)
		{
			LevelEvent_ShowStatusSign levelEvent_ShowStatusSign = (LevelEvent_ShowStatusSign)levelEvent;
			foreach (Property property in properties)
			{
				if (property.name == "duration")
				{
					((PropertyControl_InputField)property.control).unit.text = (levelEvent_ShowStatusSign.useBeats ? RDString.Get("editor.beats") : RDString.Get("editor.unit.s"));
				}
				property.UpdateUI(levelEvent);
			}
		}

		public override void SaveProperties(LevelEvent_Base levelEvent)
		{
			LevelEvent_ShowStatusSign levelEvent_ShowStatusSign = (LevelEvent_ShowStatusSign)levelEvent;
			foreach (Property property in properties)
			{
				property.Save(levelEvent);
			}
			foreach (Property property2 in properties)
			{
				if (property2.name == "duration")
				{
					((PropertyControl_InputField)property2.control).unit.text = (levelEvent_ShowStatusSign.useBeats ? RDString.Get("editor.beats") : RDString.Get("editor.unit.s"));
				}
			}
		}
	}
}
