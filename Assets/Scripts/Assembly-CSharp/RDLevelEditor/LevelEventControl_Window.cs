using System;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class LevelEventControl_Window : LevelEventControl_Base
	{
		[Header("Window Control")]
		public RoomTransitionIcon[] roomIcons;

		public Image[] orderIcons;

		public Sprite[] orderSprites;

		public Image icon;

		public override void UpdateUIInternal()
		{
			base.UpdateUIInternal();
			rt.anchoredPosition = new Vector2(base.timeline.GetPosXFromBarAndBeat(new BarAndBeat(base.bar, base.beat)), base.timeline.GetPosYFromRowIndex(levelEvent.y));
			if (levelEvent is LevelEvent_ReorderWindows)
			{
				rt.sizeDelta = new Vector2(base.cellWidth, base.cellHeight * 4);
				rt.anchoredPosition = rt.anchoredPosition.WithY(0f);
				RoomTransitionIcon[] array = roomIcons;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].gameObject.SetActive(value: false);
				}
				int num = 0;
				Image[] array2 = orderIcons;
				foreach (Image obj in array2)
				{
					int num2 = Array.IndexOf((levelEvent as LevelEvent_ReorderWindows).order, num);
					obj.GetComponent<Image>().sprite = orderSprites[num2];
					obj.gameObject.SetActive(value: true);
					num++;
				}
			}
			else if (levelEvent is LevelEvent_DesktopColor)
			{
				rt.sizeDelta = new Vector2(base.cellWidth, base.cellHeight * 4);
				rt.anchoredPosition = rt.anchoredPosition.WithY(0f);
				LevelEvent_DesktopColor levelEvent_DesktopColor = levelEvent as LevelEvent_DesktopColor;
				Image component = orderIcons[1].GetComponent<Image>();
				component.gameObject.SetActive(value: true);
				component.sprite = base.iconSprite;
				component.color = (levelEvent_DesktopColor.custom ? levelEvent_DesktopColor.endColor.GetValueOrDefault(Color.white).ToColor(useEditorPalette: true) : levelEvent_DesktopColor.GetDefaultColor());
			}
			else
			{
				rt.sizeDelta = new Vector2(Mathf.Min(14, base.cellWidth), base.cellHeight);
				Image[] array2 = orderIcons;
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i].gameObject.SetActive(value: false);
				}
				RoomTransitionIcon[] array = roomIcons;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].gameObject.SetActive(value: false);
				}
				icon.enabled = true;
				icon.sprite = base.iconSprite;
			}
			UpdateDurationBar(selected);
		}
	}
}
