using UnityEngine.EventSystems;

namespace RDLevelEditor
{
	public class FollowPlayheadEventTrigger : RDEventTrigger
	{
		public override void OnPointerClick(PointerEventData data)
		{
			if (data.button == PointerEventData.InputButton.Right)
			{
				base.timeline.CenterOnPlayhead();
			}
		}
	}
}
