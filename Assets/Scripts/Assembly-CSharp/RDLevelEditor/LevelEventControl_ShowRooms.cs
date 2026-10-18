using System;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class LevelEventControl_ShowRooms : LevelEventControl_Base
	{
		public RoomTransitionIcon[] roomIcons;

		public Image[] orderIcons;

		public Sprite[] orderSprites;

		public override void UpdateUIInternal()
		{
			base.UpdateUIInternal();
			Vector2 anchoredPosition = rt.anchoredPosition;
			anchoredPosition.x = base.timeline.GetPosXFromBarAndBeat(new BarAndBeat(base.bar, base.beat));
			anchoredPosition.y = 0f;
			rt.anchoredPosition = anchoredPosition;
			if (levelEvent is LevelEvent_ShowRooms levelEvent_ShowRooms)
			{
				Image[] array = orderIcons;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].gameObject.SetActive(value: false);
				}
				int num = 0;
				RoomTransitionIcon[] array2 = roomIcons;
				foreach (RoomTransitionIcon obj in array2)
				{
					RoomTransitionType transitionType = ((Array.IndexOf(levelEvent_ShowRooms.rooms, num) == -1) ? RoomTransitionType.KeepHidden : RoomTransitionType.Show);
					obj.Show(transitionType, base.timeline.zoomVertFactor);
					num++;
				}
			}
			else if (levelEvent is LevelEvent_ReorderRooms)
			{
				RoomTransitionIcon[] array2 = roomIcons;
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i].gameObject.SetActive(value: false);
				}
				int num2 = 0;
				Image[] array = orderIcons;
				foreach (Image obj2 in array)
				{
					int num3 = Array.IndexOf((levelEvent as LevelEvent_ReorderRooms).order, num2);
					obj2.GetComponent<Image>().sprite = orderSprites[num3];
					obj2.gameObject.SetActive(value: true);
					num2++;
				}
			}
			else
			{
				Image[] array = orderIcons;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].gameObject.SetActive(value: false);
				}
				RoomTransitionIcon[] array2 = roomIcons;
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i].gameObject.SetActive(value: false);
				}
			}
			rt.sizeDelta = new Vector2(base.cellWidth, base.cellHeight * 4);
		}
	}
}
