using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class PropertyControl_Room : PropertyControl
	{
		public Dropdown dropdown;

		public override GameObject controlObject => dropdown.gameObject;

		private RoomAttribute controlAttribute => (RoomAttribute)base.propertyInfo.controlAttribute;

		public override void AddListeners(InspectorPanel.ChangeAction action)
		{
			SetDropdownOptionsUsingRooms();
			dropdown.onValueChanged.AddListener((int _) =>
			{
				action(null, "sndListClick");
			});
		}

		public override void Setup()
		{
		}

		public override void UpdateUI(LevelEvent_Base levelEvent)
		{
			object eventValue = GetEventValue(levelEvent);
			SetDropdownOptionsUsingRooms(controlAttribute.includeOnTop);
			int value = Convert.ToInt32(eventValue);
			dropdown.value = value;
		}

		public override void Save(LevelEvent_Base levelEvent)
		{
			int value = dropdown.value;
			SetEventValue(levelEvent, value);
		}

		public void SetDropdownOptionsUsingRooms(bool includeOnTop = false)
		{
			dropdown.ClearOptions();
			dropdown.AddOptionsWithRoomsList();
			if (includeOnTop)
			{
				dropdown.AddOptions(new List<string> { RDString.Get("editor.roomsPopup.onTop") });
			}
		}
	}
}
