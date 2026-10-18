using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class tlVerticalScrollRect : ScrollRect
	{
		public ScrollRect horizontalScrollRect;

		public TimelineEventTrigger timelineEventTrigger;

		public override void OnScroll(PointerEventData data)
		{
			if (Input.GetKey(KeyCode.LeftShift))
			{
				base.OnScroll(data);
				return;
			}
			horizontalScrollRect.OnScroll(data);
			timelineEventTrigger.OnScroll(data);
		}

		public void BaseScroll(PointerEventData data)
		{
			base.OnScroll(data);
		}
	}
}
