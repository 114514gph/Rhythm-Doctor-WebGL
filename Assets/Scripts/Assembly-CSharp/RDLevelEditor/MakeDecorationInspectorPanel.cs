using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public abstract class MakeDecorationInspectorPanel : InspectorPanel
	{
		public Dropdown room;

		public RectTransform container;

		public InputField depth;

		public Button cloneButton;

		private bool updateRoomDropdown = true;

		protected new virtual void Awake()
		{
			base.Awake();
			AddOnEditListeners(null, depth);
			AddOnEditListeners(RoomDropdownWasUpdated, room);
			cloneButton.onClick.AddListener(() =>
			{
				Clone();
			});
		}

		public void RoomDropdownWasUpdated()
		{
			int num = currentLevelEvent.room;
			int value = room.value;
			if (num == value)
			{
				return;
			}
			using (new SaveStateScope())
			{
				LevelEvent_MakeDecorationBase item = currentLevelEvent as LevelEvent_MakeDecorationBase;
				int index = base.editor.decorationsData.IndexOf(item);
				List<LevelEventControl_Base> list = base.editor.eventControls_decorations[index];
				currentLevelEvent.room = value;
				foreach (LevelEventControl_Base item2 in list)
				{
					item2.levelEvent.room = value;
					item2.transform.SetParent(base.tabSection_decorations.container[value].transform, worldPositionStays: false);
				}
				base.editor.decorationsData.RemoveAt(index);
				base.editor.eventControls_decorations.RemoveAt(index);
				int i;
				for (i = 0; i < base.editor.decorationsData.Count && base.editor.decorationsData[i].room < value; i++)
				{
				}
				base.editor.decorationsData.Insert(i, item);
				base.editor.eventControls_decorations.Insert(i, list);
				while (base.editor.MoveDecorationVertically(currentLevelEvent as LevelEvent_MakeDecorationBase, VerticalDirection.Down))
				{
				}
				foreach (List<LevelEventControl_Base> eventControls_decoration in base.editor.eventControls_decorations)
				{
					foreach (LevelEventControl_Base item3 in eventControls_decoration)
					{
						item3.UpdateUI();
					}
				}
				base.editor.tabSection_decorations.ChangePage(value, calledByUnity: false);
				base.tabSection_decorations.UpdateUI();
				base.tabSection_decorations.decorationHeaders[base.editor.currentPageDecorationsData.Count - 1].GetComponent<DecorationHeader>().Select();
				UpdateRoomDropdown();
			}
		}

		public void UpdateRoomDropdown()
		{
			room.ClearOptions();
			int value = currentLevelEvent.room;
			int[] array = new int[4];
			foreach (LevelEvent_MakeDecorationBase decorationsDatum in base.editor.decorationsData)
			{
				array[decorationsDatum.room]++;
			}
			List<string> list = new List<string>();
			for (int i = 0; i < 4; i++)
			{
				string item = RDString.Get("editor.roomIndex").Replace("[index]", (i + 1).ToString());
				list.Add(item);
			}
			room.AddOptions(list);
			room.value = value;
		}

		private void Clone()
		{
			base.editor.CloneDecoration();
		}

		public override void UpdateUIInternal(LevelEvent_Base levelEvent)
		{
			LevelEvent_MakeDecorationBase levelEvent_MakeDecorationBase = (LevelEvent_MakeDecorationBase)currentLevelEvent;
			if (updateRoomDropdown)
			{
				UpdateRoomDropdown();
			}
			depth.text = levelEvent_MakeDecorationBase.depth.ToString();
			updateRoomDropdown = true;
		}

		protected override void SaveInternal(LevelEvent_Base levelEvent)
		{
			if (base.gameObject.activeInHierarchy)
			{
				LevelEvent_MakeDecorationBase obj = (LevelEvent_MakeDecorationBase)levelEvent;
				int.TryParse(depth.text, out var result);
				obj.depth = result;
				updateRoomDropdown = false;
				UpdateUIInternal(levelEvent);
			}
		}
	}
}
