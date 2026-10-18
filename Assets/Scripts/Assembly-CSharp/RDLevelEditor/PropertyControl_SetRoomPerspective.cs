using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class PropertyControl_SetRoomPerspective : PropertyControl
	{
		public PositionPicker[] positionPickers;

		public Button[] buttons;

		public bool[] positionPickersUsed = new bool[4] { true, true, true, true };

		private string offString;

		public override GameObject controlObject => base.gameObject;

		private SetRoomPerspectiveAttribute controlAttribute => (SetRoomPerspectiveAttribute)base.propertyInfo.controlAttribute;

		private static Float2[] defaultCorners { get; set; } = new Float2[4]
		{
			new Float2(0f, 0f),
			new Float2(100f, 0f),
			new Float2(0f, 100f),
			new Float2(100f, 100f)
		};

		public override void AddListeners(InspectorPanel.ChangeAction action)
		{
			for (int i = 0; i < positionPickers.Length; i++)
			{
				PositionPicker positionPicker = positionPickers[i];
				positionPicker.x.onEndEdit.AddListener((string _) =>
				{
					action(null, "sndEditorValueChange");
				});
				positionPicker.y.onEndEdit.AddListener((string _) =>
				{
					action(null, "sndEditorValueChange");
				});
				int targetIndex = i;
				Button button = buttons[i];
				button.onClick.AddListener(() =>
				{
					bool num = !positionPickersUsed[targetIndex];
					positionPickersUsed[targetIndex] = !positionPickersUsed[targetIndex];
					if (num)
					{
						positionPicker.percent = defaultCorners[targetIndex];
					}
					UpdatePositionPickers();
					action(null);
				});
				Text component = button.GetComponent<Text>();
				component.text = RDString.Get("editor.SetRoomPerspective." + button.gameObject.name);
				RDStringToUIText.Apply(component);
				positionPicker.perspectiveCorner = i;
			}
			offString = " " + RDString.Get("editor.off");
		}

		public override void Setup()
		{
		}

		public override void UpdateUI(LevelEvent_Base levelEvent)
		{
			Float2[] array = (GetEventValue(levelEvent) as IEnumerable<Float2>).ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				Float2 @float = array[i];
				positionPickersUsed[i] = @float.xUsed || @float.yUsed;
			}
			UpdatePositionPickers();
			for (int j = 0; j < positionPickers.Length; j++)
			{
				positionPickers[j].percent = array[j];
			}
		}

		public override void Save(LevelEvent_Base levelEvent)
		{
			SetRoomPerspectiveAttribute setRoomPerspectiveAttribute = (SetRoomPerspectiveAttribute)base.propertyInfo.controlAttribute;
			Float2[] array = new Float2[positionPickers.Length];
			for (int i = 0; i < positionPickers.Length; i++)
			{
				Float2 percent = positionPickers[i].percent;
				percent.x = Mathf.Clamp(percent.x, setRoomPerspectiveAttribute.min, setRoomPerspectiveAttribute.max);
				percent.y = Mathf.Clamp(percent.y, setRoomPerspectiveAttribute.min, setRoomPerspectiveAttribute.max);
				array[i] = percent;
				positionPickers[i].percent = array[i];
				if (!positionPickersUsed[i])
				{
					ref Float2 reference = ref array[i];
					ref Float2 reference2 = ref array[i];
					bool num = positionPickersUsed[i];
					bool xUsed = num;
					reference2.yUsed = num;
					reference.xUsed = xUsed;
				}
			}
			SetEventValue(levelEvent, array);
		}

		private void UpdatePositionPickers()
		{
			for (int i = 0; i < buttons.Length; i++)
			{
				bool flag = positionPickersUsed[i];
				Text component = buttons[i].GetComponent<Text>();
				component.text = component.text.Replace(offString, "") + (flag ? "" : offString);
				component.color = (flag ? Color.white : Color.gray);
				component.GetComponent<RDColorHoverEventTrigger>().exitColor = component.color;
				positionPickers[i].gameObject.SetActive(flag);
			}
		}
	}
}
