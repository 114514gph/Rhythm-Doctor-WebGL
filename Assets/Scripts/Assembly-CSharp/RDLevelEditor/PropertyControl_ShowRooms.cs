using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class PropertyControl_ShowRooms : PropertyControl
	{
		public Toggle[] visibleRooms;

		public InputField[] heights;

		public override GameObject controlObject => base.gameObject;

		private ShowRoomsAttribute controlAttribute => (ShowRoomsAttribute)base.propertyInfo.controlAttribute;

		public override void AddListeners(InspectorPanel.ChangeAction action)
		{
			Toggle[] array = visibleRooms;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].onValueChanged.AddListener((bool t) =>
				{
					action(null, "sndButtonRadio");
				});
			}
			InputField[] array2 = heights;
			foreach (InputField obj in array2)
			{
				Text component = obj.placeholder.GetComponent<Text>();
				component.text = RDString.Get("editor.auto");
				RDStringToUIText.Apply(component);
				obj.onEndEdit.AddListener((string _) =>
				{
					action(null, "sndEditorValueChange");
				});
			}
		}

		public override void Setup()
		{
		}

		public override void UpdateUI(LevelEvent_Base levelEvent)
		{
			int[] array = (GetEventValue(levelEvent) as IEnumerable<int>).ToArray();
			Toggle[] array2 = visibleRooms;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].isOn = false;
			}
			int[] rooms = levelEvent.rooms;
			foreach (int num in rooms)
			{
				visibleRooms[num].isOn = true;
			}
			for (int j = 0; j < visibleRooms.Length; j++)
			{
				InputField inputField = heights[j];
				inputField.gameObject.SetActive(visibleRooms[j].isOn);
				if (array[j] == 0)
				{
					inputField.text = "";
				}
				else
				{
					inputField.text = array[j].ToString();
				}
			}
		}

		public override void Save(LevelEvent_Base levelEvent)
		{
			List<int> list = new List<int>();
			for (int i = 0; i < visibleRooms.Length; i++)
			{
				Toggle toggle = visibleRooms[i];
				heights[i].gameObject.SetActive(toggle.isOn);
				if (toggle.isOn)
				{
					list.Add(i);
				}
			}
			levelEvent.rooms = list.ToArray();
			int[] array = new int[4];
			for (int j = 0; j < heights.Length; j++)
			{
				InputField inputField = heights[j];
				array[j] = ((!string.IsNullOrEmpty(inputField.text)) ? Convert.ToInt32(inputField.text) : 0);
			}
			SetEventValue(levelEvent, array);
		}
	}
}
