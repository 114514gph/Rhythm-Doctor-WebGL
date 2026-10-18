namespace RDLevelEditor
{
	public class InspectorPanel_SetPlayStyle : InspectorPanel
	{
		public override void UpdateUIProperties(LevelEvent_Base levelEvent)
		{
			foreach (Property property in properties)
			{
				if (property.name == "NextBar")
				{
					PropertyControl_InputField obj = (PropertyControl_InputField)property.control;
					int nextBar = ((LevelEvent_SetPlayStyle)levelEvent).NextBar;
					obj.inputField.text = nextBar.ToString();
				}
			}
			base.UpdateUIProperties(levelEvent);
		}
	}
}
