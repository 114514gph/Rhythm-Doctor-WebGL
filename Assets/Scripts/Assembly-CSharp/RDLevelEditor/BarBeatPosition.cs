using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class BarBeatPosition : RDEditorBase
	{
		public Text barLabel;

		public InputField bar;

		public Text beatLabel;

		public InputField beat;

		public Text evTagLabel;

		public InputField evTag;

		public Toggle evTagRunToggle;

		public GameObject roomsContainer;

		public Button[] roomButtons;

		public List<int> roomsData = new List<int>();

		public Button conditionsButton;

		public Image conditionsButtonIcon;

		public Color noConditionsColor;

		public Color anyConditionsColor;

		public Color negatedConditionsColor;

		public Color mixedConditionsColor;

		public Button[] labelButtons;

		[NonSerialized]
		public InspectorPanel panel;

		[NonSerialized]
		public LevelEvent_Base levelEvent;

		[NonSerialized]
		public bool forceRoomsHidden;

		[NonSerialized]
		public RoomsUsage roomsUsage;

		private bool didSetup;

		public BarAndBeat barAndBeat
		{
			get
			{
				return new BarAndBeat(Convert.ToInt32(bar.text), float.Parse(beat.text));
			}
			set
			{
				bar.text = value.bar.ToString();
				beat.text = value.beat.ToString();
			}
		}

		public int[] rooms
		{
			get
			{
				return roomsData.ToArray();
			}
			set
			{
				roomsData = new List<int>(value);
				UpdateUI();
			}
		}

		public string eventTag
		{
			get
			{
				return evTag.text;
			}
			set
			{
				evTag.text = value;
			}
		}

		public bool tagRunNormally
		{
			get
			{
				return evTagRunToggle.isOn;
			}
			set
			{
				panel.isUpdatingUI = true;
				evTagRunToggle.isOn = value;
				panel.isUpdatingUI = false;
			}
		}

		private void Update()
		{
			bool flag = base.editor.eventControls_rooms.Count != 0;
			bool flag2 = roomsUsage != RoomsUsage.NotUsed;
			bool flag3 = false;
			foreach (LevelEventControl_Base eventControls_window in base.editor.eventControls_windows)
			{
				if (eventControls_window.levelEvent is LevelEvent_SetWindowContent && ((LevelEvent_SetWindowContent)eventControls_window.levelEvent).contentMode == WindowContentMode.Room)
				{
					flag3 = true;
					break;
				}
			}
			roomsContainer.SetActive((!forceRoomsHidden & flag2) && (flag | flag3));
		}

		public void UpdateUI()
		{
			Update();
			UpdateRoomButtons(roomButtons, roomsData);
			if (base.editor.roomsSelectionPopup.gameObject.activeSelf)
			{
				base.editor.roomsSelectionPopup.UpdateUI();
			}
			Button[] array = labelButtons;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].gameObject.GetComponent<RDColorHoverEventTrigger>().enterColor = base.editor.inspectorTitle.color;
			}
			barLabel.gameObject.SetActive(!base.editor.tagFieldMode);
			beatLabel.gameObject.SetActive(levelEvent.usesBeat && !base.editor.tagFieldMode);
			evTagLabel.gameObject.SetActive(base.editor.tagFieldMode);
			if (!levelEvent.usesBeat)
			{
				barLabel.transform.localPosition = barLabel.transform.localPosition.WithX(0f);
			}
		}

		public void Setup(InspectorPanel panel)
		{
			if (didSetup)
			{
				return;
			}
			didSetup = true;
			this.panel = panel;
			int num = 0;
			Button[] array = roomButtons;
			foreach (Button button in array)
			{
				int iCaptured = num;
				button.onClick.AddListener(() =>
				{
					MonoBehaviour.print("setup2: " + panel.name);
					if (roomsData.IndexOf(iCaptured) == -1)
					{
						roomsData.Add(iCaptured);
					}
					else
					{
						roomsData.Remove(iCaptured);
					}
					MonoBehaviour.print("roomsdata:");
					foreach (int roomsDatum in roomsData)
					{
						MonoBehaviour.print(roomsDatum + "//");
					}
					UpdateUI();
				});
				num++;
				panel.AddOnEditListeners(null, button);
			}
			array = labelButtons;
			foreach (Button button2 in array)
			{
				button2.onClick.AddListener(() =>
				{
					base.editor.tagFieldMode = !base.editor.tagFieldMode;
					UpdateUI();
				});
				panel.AddOnEditListeners(null, button2);
			}
			panel.AddOnEditListeners(null, bar, beat, evTag, evTagRunToggle);
			if (conditionsButton != null)
			{
				conditionsButton.onClick.AddListener(() =>
				{
					base.editor.ShowPanelConditionals(panel);
					base.editor.conditionalsPreview.Hide();
					panel.currentEventControl.UpdateUI();
					LevelEditorPlaySound("sndButtonClickOn", "LevelEditorInspectorPanel");
				});
			}
		}

		public void ShowRoomsSelectionPopup()
		{
			RDRoomsSelectionPopup roomsSelectionPopup = base.editor.roomsSelectionPopup;
			roomsSelectionPopup.Show();
			roomsSelectionPopup.levelEvent = levelEvent;
			roomsSelectionPopup.settingActionEventRooms = false;
			roomsSelectionPopup.UpdateUI();
		}

		public void UpdateConditionalsButton()
		{
			LevelEvent_Base levelEvent_Base = levelEvent;
			bool flag = !(levelEvent_Base is LevelEvent_SetCrotchetsPerBar) && !(levelEvent_Base is LevelEvent_PulseFreeTimeBeat);
			conditionsButton.gameObject.SetActive(flag);
			if (!flag)
			{
				return;
			}
			RDEventTrigger component = conditionsButton.GetComponent<RDEventTrigger>();
			if (!levelEvent.HasAnyConditionals())
			{
				conditionsButtonIcon.color = noConditionsColor;
				component.enabled = false;
				return;
			}
			component.enabled = true;
			component.onPointerEnter = (PointerEventData data) =>
			{
				base.editor.conditionalsPreview.Show(levelEvent, conditionsButton.transform.position + Vector3.down * 8f, topToBottom: true);
			};
			component.onPointerExit = (PointerEventData data) =>
			{
				base.editor.conditionalsPreview.Hide();
			};
			bool flag2 = levelEvent.HasPositiveConditionals();
			bool flag3 = levelEvent.HasNegativeConditionals();
			Color color = noConditionsColor;
			color = ((flag2 && !flag3) ? anyConditionsColor : ((!(!flag2 & flag3)) ? mixedConditionsColor : negatedConditionsColor));
			conditionsButtonIcon.color = color;
		}

		public void toggleTagRun(bool val)
		{
			levelEvent.tagRunNormally = val;
		}

		public static void UpdateRoomButtons(Button[] buttons, List<int> data)
		{
			for (int i = 0; i < buttons.Length; i++)
			{
				buttons[i].GetComponent<Image>().color = "5B5B5BFF".HexToColor();
			}
			buttons[4].gameObject.SetActive(data.IndexOf(4) != -1);
			int num = 0;
			foreach (int datum in data)
			{
				buttons[datum].GetComponent<Image>().color = RDConstants.data.colorPalette[3];
				num++;
			}
		}
	}
}
