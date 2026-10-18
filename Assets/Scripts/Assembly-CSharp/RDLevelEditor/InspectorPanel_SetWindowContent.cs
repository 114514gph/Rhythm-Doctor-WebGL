using System.Collections.Generic;

namespace RDLevelEditor
{
	public class InspectorPanel_SetWindowContent : InspectorPanel
	{
		private string[] roomList;

		private new void Awake()
		{
			base.Awake();
			List<string> list = new List<string>();
			for (int i = 0; i < 4; i++)
			{
				string item = RDString.Get("editor.roomIndex").Replace("[index]", (i + 1).ToString());
				list.Add(item);
			}
			roomList = list.ToArray();
		}

		public override void UpdateUIProperties(LevelEvent_Base levelEvent)
		{
			foreach (Property property in properties)
			{
				if (property.name == "roomIndex")
				{
					((PropertyControl_Dropdown)property.control).SetDropdownOptions(roomList, localize: false);
				}
				property.UpdateUI(levelEvent);
			}
		}
	}
}
