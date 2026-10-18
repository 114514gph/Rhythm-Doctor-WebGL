using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class tlHorizontalScrollRect : ScrollRect
	{
		public ScrollRect verticalScrollRect;

		public override void OnScroll(PointerEventData data)
		{
			if (Input.GetKey(KeyCode.LeftShift))
			{
				verticalScrollRect.OnScroll(data);
			}
			else
			{
				base.OnScroll(data);
			}
		}
	}
}
