using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class PropertyControl_ReorderRooms : PropertyControl
	{
		public RectTransform container;

		public RectTransform[] roomButtons;

		public Text front;

		public Text back;

		private const float offsetY = -12f;

		private const float animationDuration = 0.1f;

		private List<RectTransform> ordered = new List<RectTransform>();

		private float initialDragOffset;

		private RectTransform buttonBeingDragged;

		private float yPrev;

		private Action registerForChange;

		public override GameObject controlObject => container.gameObject;

		private ReorderRoomsAttribute controlAttribute => (ReorderRoomsAttribute)base.propertyInfo.controlAttribute;

		public override void AddListeners(InspectorPanel.ChangeAction action)
		{
			for (int i = 0; i < roomButtons.Length; i++)
			{
				RectTransform button = roomButtons[i];
				Text componentInChildren = button.GetComponentInChildren<Text>();
				componentInChildren.text = string.Format("{0} {1}", RDString.Get(controlAttribute.isWindow ? "editor.window" : "editor.room"), i + 1);
				RDStringToUIText.Apply(componentInChildren);
				button.gameObject.AddComponent<RDEventTrigger>().onPointerDown = (PointerEventData e) =>
				{
					buttonBeingDragged = button;
					initialDragOffset = Input.mousePosition.y - button.transform.position.y;
					button.SetAsLastSibling();
				};
			}
			ordered = new List<RectTransform>(roomButtons);
			registerForChange = () =>
			{
				action(null);
			};
		}

		public override void Setup()
		{
		}

		public override void UpdateUI(LevelEvent_Base levelEvent)
		{
			int[] array = (GetEventValue(levelEvent) as IEnumerable<int>).ToArray();
			for (int i = 0; i < 4; i++)
			{
				ordered[i] = roomButtons[array[i]];
				ordered[i].AnchorPosY((float)i * -12f);
			}
		}

		public override void Save(LevelEvent_Base levelEvent)
		{
			int[] array = new int[4];
			for (int i = 0; i < 4; i++)
			{
				array[i] = int.Parse(ordered[i].name);
			}
			SetEventValue(levelEvent, array);
		}

		private void Update()
		{
			if (!(buttonBeingDragged != null))
			{
				return;
			}
			RectTransform rectTransform = buttonBeingDragged;
			float num = Input.mousePosition.y - initialDragOffset;
			if (yPrev != num)
			{
				yPrev = num;
				rectTransform.position = rectTransform.position.WithY(num);
				float min = 0f - (container.sizeDelta.y - rectTransform.sizeDelta.y);
				float max = 0f;
				float num2 = Mathf.Clamp(rectTransform.anchoredPosition.y, min, max);
				rectTransform.AnchorPosY(num2);
				int index = Mathf.RoundToInt(rectTransform.anchoredPosition.y / -12f);
				ordered.Remove(rectTransform);
				ordered.Insert(index, rectTransform);
				int num3 = 0;
				foreach (RectTransform item in ordered)
				{
					if (item != rectTransform)
					{
						item.DOKill();
						item.DOAnchorPosY((float)num3 * -12f, 0.1f).SetEase(Ease.OutSine).SetUpdate(isIndependentUpdate: true);
					}
					num3++;
				}
			}
			if (Input.GetMouseButtonUp(0))
			{
				rectTransform.DOKill();
				float endValue = Mathf.Round(rectTransform.anchoredPosition.y / -12f) * -12f;
				rectTransform.DOAnchorPosY(endValue, 0.1f).SetEase(Ease.OutSine).SetUpdate(isIndependentUpdate: true);
				buttonBeingDragged = null;
				registerForChange();
			}
		}
	}
}
