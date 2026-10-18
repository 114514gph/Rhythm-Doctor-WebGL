using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RDLevelEditor
{
	public class RDEventTrigger : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IMoveHandler, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler
	{
		protected bool isDragging;

		public int lastEventFrame = -1;

		[NonSerialized]
		public RDPointerEventData onClick;

		[NonSerialized]
		public RDPointerEventData onPointerEnter;

		[NonSerialized]
		public RDPointerEventData onPointerDown;

		[NonSerialized]
		public RDPointerEventData onPointerUp;

		[NonSerialized]
		public RDPointerEventData onPointerExit;

		[NonSerialized]
		public RDPointerEventData onBeginDrag;

		[NonSerialized]
		public RDPointerEventData onEndDrag;

		public TabSection_Rows rowsTab => (TabSection_Rows)editor.tabSections[1];

		public TabSection_Decorations decorationsTab => (TabSection_Decorations)editor.tabSections[4];

		protected scnEditor editor => scnEditor.instance;

		protected Timeline timeline => editor.timeline;

		protected float cellWidthSnap => (float)timeline.cellWidth * 1f / (float)editor.denominator;

		protected int cellWidth => timeline.cellWidth;

		protected int cellHeight => timeline.cellHeight;

		public virtual void OnBeginDrag(PointerEventData eventData)
		{
			isDragging = true;
			if (onBeginDrag != null)
			{
				onBeginDrag(eventData);
			}
			lastEventFrame = Time.frameCount;
		}

		public virtual void OnEndDrag(PointerEventData eventData)
		{
			isDragging = false;
			if (onEndDrag != null)
			{
				onEndDrag(eventData);
			}
			lastEventFrame = Time.frameCount;
		}

		public virtual void OnPointerClick(PointerEventData eventData)
		{
			if (onClick != null)
			{
				onClick(eventData);
			}
			lastEventFrame = Time.frameCount;
		}

		public virtual void OnPointerEnter(PointerEventData eventData)
		{
			if (onPointerEnter != null)
			{
				onPointerEnter(eventData);
			}
			lastEventFrame = Time.frameCount;
		}

		public virtual void OnPointerExit(PointerEventData eventData)
		{
			if (onPointerExit != null)
			{
				onPointerExit(eventData);
			}
			lastEventFrame = Time.frameCount;
		}

		public virtual void OnPointerDown(PointerEventData eventData)
		{
			if (onPointerDown != null)
			{
				onPointerDown(eventData);
			}
			lastEventFrame = Time.frameCount;
		}

		public virtual void OnPointerUp(PointerEventData eventData)
		{
			if (onPointerUp != null)
			{
				onPointerUp(eventData);
			}
			lastEventFrame = Time.frameCount;
		}

		public virtual void OnMove(AxisEventData eventData)
		{
		}

		public virtual void OnDrag(PointerEventData eventData)
		{
		}
	}
}
