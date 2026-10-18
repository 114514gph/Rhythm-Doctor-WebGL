using UnityEngine;
using UnityEngine.EventSystems;

namespace RDLevelEditor
{
	public class TimelineHeightEventTrigger : RDEventTrigger
	{
		private float dragStartY;

		private int dragStartRows;

		public override void OnPointerDown(PointerEventData eventData)
		{
			base.OnPointerDown(eventData);
			dragStartY = base.editor.mousePosition.y;
			dragStartRows = base.timeline.rowCellCount;
			scrConductor.PlayImmediately("sndSliderGrab", 1f, RDUtils.GetMixerGroup("LevelEditorPassive"), 1f, 0f, isBeatSound: false, ignoreListenerPause: true);
		}

		public override void OnDrag(PointerEventData eventData)
		{
			base.OnDrag(eventData);
			int num = Mathf.RoundToInt((base.editor.mousePosition.y - dragStartY) / 14f);
			int rowCellCount = base.timeline.rowCellCount;
			base.timeline.rowCellCount = dragStartRows + num;
			base.timeline.UpdateUI();
			if (base.timeline.rowCellCount != rowCellCount)
			{
				scrConductor.PlayImmediately("sndSliderTick", 1f, RDUtils.GetMixerGroup("LevelEditorPassive"), 1f, 0f, isBeatSound: false, ignoreListenerPause: true);
			}
		}

		public override void OnEndDrag(PointerEventData eventData)
		{
			base.OnEndDrag(eventData);
			scrConductor.PlayImmediately("sndSliderRelease", 1f, RDUtils.GetMixerGroup("LevelEditorPassive"), 1f, 0f, isBeatSound: false, ignoreListenerPause: true);
		}

		public override void OnPointerClick(PointerEventData eventData)
		{
			if (!eventData.dragging)
			{
				base.OnPointerClick(eventData);
				base.timeline.CycleExpanded();
			}
		}
	}
}
