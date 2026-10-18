using UnityEngine.EventSystems;

namespace RDLevelEditor
{
	public class SpritesListScroll : RDEditorBase, IScrollHandler, IEventSystemHandler
	{
		public void OnScroll(PointerEventData data)
		{
			base.timeline.verticalScrollRect.BaseScroll(data);
		}
	}
}
