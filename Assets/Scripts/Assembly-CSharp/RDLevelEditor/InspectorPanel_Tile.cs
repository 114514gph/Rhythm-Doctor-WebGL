namespace RDLevelEditor
{
	public class InspectorPanel_Tile : InspectorPanel
	{
		public override void SaveProperties(LevelEvent_Base levelEvent)
		{
			foreach (Property property in properties)
			{
				property.Save(levelEvent);
			}
			foreach (Property property2 in properties)
			{
				property2.Save(levelEvent);
			}
		}
	}
}
