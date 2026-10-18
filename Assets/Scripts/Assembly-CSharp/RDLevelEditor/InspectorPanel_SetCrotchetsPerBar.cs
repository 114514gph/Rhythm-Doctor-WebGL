namespace RDLevelEditor
{
	public class InspectorPanel_SetCrotchetsPerBar : InspectorPanel
	{
		public override void SaveProperties(LevelEvent_Base levelEvent)
		{
			base.SaveProperties(levelEvent);
			base.timeline.UpdateUI();
		}
	}
}
