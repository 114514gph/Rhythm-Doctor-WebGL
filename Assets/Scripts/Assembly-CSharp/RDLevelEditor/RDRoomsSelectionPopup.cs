using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class RDRoomsSelectionPopup : RDEditorBase
	{
		public RectTransform background;

		public bool openedThisFrame;

		public Toggle[] toggles;

		public LevelEvent_Base levelEvent;

		private SelectLevelEventPanel _actionEventPanel;

		public bool settingActionEventRooms;

		private bool hidden = true;

		public SelectLevelEventPanel actionEventPanel
		{
			get
			{
				if (_actionEventPanel == null)
				{
					GameObject gameObject = base.editor.inspectorPanelManager.eventListPanels[2];
					_actionEventPanel = gameObject.GetComponent<SelectLevelEventPanel>();
				}
				return _actionEventPanel;
			}
		}

		private void Start()
		{
			int num = 0;
			Toggle[] array = toggles;
			foreach (Toggle toggle in array)
			{
				EventTrigger orAddComponent = toggle.gameObject.GetOrAddComponent<EventTrigger>();
				if (orAddComponent.triggers.Count == 0)
				{
					EventTrigger.Entry entry = new EventTrigger.Entry();
					entry.eventID = EventTriggerType.PointerClick;
					int iCaptured = num;
					entry.callback.AddListener((BaseEventData eventData) =>
					{
						ToggleClicked(iCaptured);
					});
					orAddComponent.triggers.Add(entry);
					if (num != 4)
					{
						Text component = toggle.transform.GetChild(1).GetComponent<Text>();
						component.text = RDString.Get("editor.roomIndex").Replace("[index]", (num + 1).ToString());
						RDStringToUIText.Apply(component);
					}
				}
				toggle.graphic.color = base.gc.colorPalette[3];
				num++;
			}
		}

		private void ToggleClicked(int index)
		{
			List<int> list = new List<int>(settingActionEventRooms ? actionEventPanel.rooms : levelEvent.rooms);
			if (!settingActionEventRooms)
			{
				RoomsUsage roomsUsage = levelEvent.roomsUsage;
				if (roomsUsage == RoomsUsage.OneRoom || roomsUsage == RoomsUsage.OneRoomOrOnTop)
				{
					list.Clear();
					list.Add(index);
					goto IL_00b1;
				}
			}
			if (index == 4 && !settingActionEventRooms)
			{
				list.Clear();
				list.Add(4);
			}
			else if (list.IndexOf(4) != -1 && !settingActionEventRooms)
			{
				list.Clear();
				list.Add(index);
			}
			else if (list.IndexOf(index) == -1)
			{
				list.Add(index);
			}
			else if (list.Count > 1)
			{
				list.Remove(index);
			}
			goto IL_00b1;
			IL_00b1:
			if (settingActionEventRooms)
			{
				actionEventPanel.rooms = list.ToArray();
				actionEventPanel.OnEnable();
			}
			else
			{
				levelEvent.rooms = list.ToArray();
				levelEvent.inspectorPanel.UpdateUI(levelEvent);
			}
		}

		public void UpdateUI()
		{
			Toggle[] array = toggles;
			foreach (Toggle obj in array)
			{
				obj.isOn = false;
				obj.transform.GetChild(1).GetComponent<Text>().color = Color.white;
			}
			int[] array2 = (settingActionEventRooms ? actionEventPanel.rooms : levelEvent.rooms);
			foreach (int num in array2)
			{
				Toggle obj2 = toggles[num];
				obj2.isOn = true;
				obj2.transform.GetChild(1).GetComponent<Text>().color = Color.white;
			}
			int num2;
			if (!settingActionEventRooms)
			{
				RoomsUsage roomsUsage = levelEvent.roomsUsage;
				num2 = ((roomsUsage == RoomsUsage.ManyRoomsAndOnTop || roomsUsage == RoomsUsage.OneRoomOrOnTop) ? 1 : 0);
			}
			else
			{
				num2 = 1;
			}
			bool flag = (byte)num2 != 0;
			toggles[4].gameObject.SetActive(flag);
			background.sizeDelta = background.sizeDelta.WithY(flag ? 78f : 61f);
		}

		public void Show()
		{
			hidden = false;
			base.gameObject.SetActive(value: true);
			openedThisFrame = true;
			RectTransform component = GetComponent<RectTransform>();
			component.DOKill(complete: true);
			component.localScale = Vector3.zero;
			component.DOScale(Vector3.one, base.gc.roomsSelectionPopup_showAnimDuration).SetEase(base.gc.roomsSelectionPopup_showAnimEase).SetUpdate(UpdateType.Normal, isIndependentUpdate: true);
			LevelEditorPlaySound("sndEditorPopupSmall", "LevelEditorInspectorPanel");
			base.editor.popupBackground.SetActive(value: true);
		}

		public void Hide()
		{
			if (!hidden)
			{
				hidden = true;
				base.gameObject.SetActive(value: true);
				RectTransform component = GetComponent<RectTransform>();
				component.DOKill(complete: true);
				component.localScale = Vector3.one;
				component.DOScale(Vector3.zero, base.gc.roomsSelectionPopup_hideAnimDuration).SetEase(base.gc.roomsSelectionPopup_hideAnimEase).SetUpdate(UpdateType.Normal, isIndependentUpdate: true)
					.OnComplete(() =>
					{
						base.gameObject.SetActive(value: false);
					});
				LevelEditorPlaySound("sndEditorPopdownSmall", "LevelEditorInspectorPanel");
				base.editor.popupBackground.SetActive(value: false);
			}
		}

		private void LateUpdate()
		{
			if (base.gameObject.activeSelf && !openedThisFrame && Input.GetKeyDown(KeyCode.Escape))
			{
				Hide();
			}
			openedThisFrame = false;
		}
	}
}
