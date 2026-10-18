using UnityEngine.EventSystems;

namespace RDLevelEditor
{
	public class InspectorPanel_AddClassicBeat : InspectorPanel
	{
		private bool swingPointing;

		private bool swingDragging;

		public override void UpdateUIProperties(LevelEvent_Base levelEvent)
		{
			LevelEvent_AddClassicBeat levelEvent_AddClassicBeat = (LevelEvent_AddClassicBeat)levelEvent;
			foreach (Property property in properties)
			{
				if (property.name == "swing")
				{
					PropertyControl_Slider obj = (PropertyControl_Slider)property.control;
					obj.slider.maxValue = levelEvent_AddClassicBeat.tick * 2f;
					obj.snapInterval = 1f / (float)base.editor.denominator;
					RDEventTrigger component = obj.slider.GetComponent<RDEventTrigger>();
					component.onPointerEnter = (PointerEventData e) =>
					{
						swingPointing = true;
						UpdateSwingTickBorders();
					};
					component.onPointerExit = (PointerEventData e) =>
					{
						swingPointing = false;
						UpdateSwingTickBorders();
					};
					component.onBeginDrag = (PointerEventData e) =>
					{
						swingDragging = true;
						UpdateSwingTickBorders();
					};
					component.onEndDrag = (PointerEventData e) =>
					{
						swingDragging = false;
						UpdateSwingTickBorders();
					};
				}
				property.UpdateUI(levelEvent);
			}
		}

		public override void SaveProperties(LevelEvent_Base levelEvent)
		{
			LevelEvent_AddClassicBeat levelEvent_AddClassicBeat = (LevelEvent_AddClassicBeat)levelEvent;
			foreach (Property property in properties)
			{
				if (property.name == "swing")
				{
					PropertyControl_Slider obj = (PropertyControl_Slider)property.control;
					obj.slider.maxValue = levelEvent_AddClassicBeat.tick * 2f;
					obj.snapInterval = 1f / (float)base.editor.denominator;
				}
				property.Save(levelEvent);
			}
		}

		private void UpdateSwingTickBorders()
		{
			currentEventControl.ShowBorders(swingPointing || swingDragging);
		}
	}
}
