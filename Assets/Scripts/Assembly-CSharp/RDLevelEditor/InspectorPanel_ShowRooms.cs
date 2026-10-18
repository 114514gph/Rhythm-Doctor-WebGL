namespace RDLevelEditor
{
	public class InspectorPanel_ShowRooms : InspectorPanel
	{
		private new void Awake()
		{
			base.Awake();
			position.forceRoomsHidden = true;
		}
	}
}
