using UnityEngine.EventSystems;

namespace RDLevelEditor
{
	public class SnapButtonEventTrigger : RDEventTrigger
	{
		public override void OnPointerClick(PointerEventData data)
		{
			if (data.button == PointerEventData.InputButton.Right)
			{
				base.editor.CycleSnapValues(-1);
			}
			else
			{
				base.editor.CycleSnapValues(1);
			}
		}
	}
}
