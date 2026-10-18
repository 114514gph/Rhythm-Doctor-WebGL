using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class LevelEventControl_SetRowXs : LevelEventControl_Base
	{
		public Image[] Xs;

		public Image[] syncoArrows;

		public Image[] LineSegments;

		public GameObject inspectorPanel => GameObject.Find(levelEvent.GetType().Name);

		private new void Awake()
		{
			base.Awake();
			levelEvent = new LevelEvent_SetRowXs();
		}

		public override void UpdateUIInternal()
		{
			base.UpdateUIInternal();
			LevelEvent_SetRowXs levelEvent_SetRowXs = (LevelEvent_SetRowXs)levelEvent;
			RectTransform component = GetComponent<RectTransform>();
			Vector2 anchoredPosition = component.anchoredPosition;
			anchoredPosition.x = base.timeline.GetPosXFromBarAndBeat(new BarAndBeat(base.bar, base.beat));
			anchoredPosition.y = -base.visualRowIndex * base.cellHeight;
			component.anchoredPosition = anchoredPosition;
			component.sizeDelta = new Vector2(28f, base.cellHeight);
			int num = 0;
			string pattern = levelEvent_SetRowXs.pattern;
			foreach (char c in pattern)
			{
				Xs[num].enabled = c == 'x';
				syncoArrows[num].enabled = levelEvent_SetRowXs.syncoBeat == num;
				LineSegments[num].enabled = levelEvent_SetRowXs.syncoBeat != num;
				num++;
			}
		}
	}
}
