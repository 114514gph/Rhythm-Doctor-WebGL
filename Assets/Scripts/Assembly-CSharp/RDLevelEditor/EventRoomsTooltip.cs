using UnityEngine.UI;

namespace RDLevelEditor
{
	public class EventRoomsTooltip : RDEditorBase
	{
		public Image[] roomIcons;

		public Image topIcon;

		private void Awake()
		{
			topIcon.color = RDConstants.data.colorPalette[3];
		}

		public void SetRooms(int[] rooms)
		{
			if (rooms == null)
			{
				base.enabled = false;
				return;
			}
			base.enabled = true;
			Image[] array = roomIcons;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].color = "5B5B5BFF".HexToColor();
			}
			bool active = false;
			foreach (int num in rooms)
			{
				if (num < 0 || num >= roomIcons.Length)
				{
					active = true;
				}
				else
				{
					roomIcons[num].color = RDConstants.data.colorPalette[3];
				}
			}
			topIcon.gameObject.SetActive(active);
		}
	}
}
