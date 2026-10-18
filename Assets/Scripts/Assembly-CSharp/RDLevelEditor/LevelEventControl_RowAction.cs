using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class LevelEventControl_RowAction : LevelEventControl_Base
	{
		public Image icon;

		public override void UpdateUIInternal()
		{
			base.UpdateUIInternal();
			RectTransform component = GetComponent<RectTransform>();
			Vector2 anchoredPosition = component.anchoredPosition;
			icon.sprite = base.iconSprite;
			component.sizeDelta = new Vector2(Mathf.Min(14, base.cellWidth), base.cellHeight);
			anchoredPosition.x = base.timeline.GetPosXFromBarAndBeat(new BarAndBeat(base.bar, base.beat));
			anchoredPosition.y = -base.visualRowIndex * base.cellHeight;
			component.anchoredPosition = anchoredPosition;
		}
	}
}
