namespace RDLevelEditor
{
	public class InspectorPanel_SayReadyGetSetGo : InspectorPanel
	{
		public override void UpdateUIProperties(LevelEvent_Base levelEvent)
		{
			LevelEvent_SayReadyGetSetGo levelEvent_SayReadyGetSetGo = (LevelEvent_SayReadyGetSetGo)levelEvent;
			foreach (Property property in properties)
			{
				if (property.name == "tick")
				{
					((PropertyControl_InputField)property.control).inputField.interactable = levelEvent_SayReadyGetSetGo.EnableIfPhraseHasTick();
				}
				property.UpdateUI(levelEvent);
			}
		}

		public override void SaveProperties(LevelEvent_Base levelEvent)
		{
			LevelEvent_SayReadyGetSetGo levelEvent_SayReadyGetSetGo = (LevelEvent_SayReadyGetSetGo)levelEvent;
			foreach (Property property in properties)
			{
				if (property.name == "tick")
				{
					((PropertyControl_InputField)property.control).inputField.interactable = levelEvent_SayReadyGetSetGo.EnableIfPhraseHasTick();
				}
				property.Save(levelEvent);
			}
		}
	}
}
