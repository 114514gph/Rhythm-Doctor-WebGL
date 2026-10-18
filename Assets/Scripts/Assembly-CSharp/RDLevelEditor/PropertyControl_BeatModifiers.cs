using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class PropertyControl_BeatModifiers : PropertyControl
	{
		public RDEventTrigger[] buttons;

		public RDEventTrigger[] syncoButtons;

		public Sprite normalPulse;

		public Sprite beatX;

		public Sprite upArrow;

		public Sprite downArrow;

		public Sprite pull;

		public Sprite release;

		public Sprite syncoOn;

		public Sprite syncoOff;

		public Sprite straight;

		public int syncoBeat = -1;

		[NonSerialized]
		public int[] buttonState;

		private Sprite[] sprites;

		private char[] allIdentifiers = new char[7] { '-', 'x', 'u', 'd', 'b', 'r', 's' };

		private char[] commonIdentifiers = new char[2] { '-', 'x' };

		private char[] uncommonIdentifiers = new char[3] { 'u', 'd', '-' };

		private char[] specialIdentifiers = new char[4] { 'b', 'r', 's', '-' };

		private string value;

		public override GameObject controlObject => base.gameObject;

		private BeatModifiersAttribute controlAttribute => (BeatModifiersAttribute)base.propertyInfo.controlAttribute;

		public override void AddListeners(InspectorPanel.ChangeAction action)
		{
			sprites = new Sprite[7] { normalPulse, beatX, upArrow, downArrow, pull, release, straight };
			buttonState = new int[6];
			int num = 0;
			RDEventTrigger[] array = buttons;
			foreach (RDEventTrigger obj in array)
			{
				int x = num;
				obj.onClick = (PointerEventData e) =>
				{
					bool flag = e.button == PointerEventData.InputButton.Left;
					bool flag2 = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
					bool flag3 = controlAttribute.leftClick & flag;
					bool num3 = controlAttribute.middleClick && (e.button == PointerEventData.InputButton.Middle || (flag & flag2));
					bool flag4 = controlAttribute.rightClick && e.button == PointerEventData.InputButton.Right;
					char[] array2 = (num3 ? specialIdentifiers : (flag3 ? commonIdentifiers : (flag4 ? uncommonIdentifiers : null)));
					if (array2 != null)
					{
						int num4 = Array.IndexOf(array2, value[x]);
						int num5 = ((num4 != -1) ? ((num4 + 1) % array2.Length) : 0);
						char[] array3 = value.ToCharArray();
						array3[x] = array2[num5];
						value = new string(array3);
						action(null, "sndButtonClick");
					}
				};
				num++;
			}
			int num2 = 0;
			array = syncoButtons;
			foreach (RDEventTrigger obj2 in array)
			{
				int x2 = num2;
				obj2.onClick = (PointerEventData e) =>
				{
					if (controlAttribute.syncoBeat && e.button == PointerEventData.InputButton.Left)
					{
						syncoBeat = ((syncoBeat == x2) ? (-1) : x2);
						action(null, "sndButtonClick");
					}
				};
				num2++;
			}
		}

		public override void Setup()
		{
		}

		public override void UpdateUI(LevelEvent_Base levelEvent)
		{
			value = GetEventValue(levelEvent).ToString();
			int num = 0;
			RDEventTrigger[] array = buttons;
			foreach (RDEventTrigger obj in array)
			{
				int num2 = Array.IndexOf(allIdentifiers, value[num]);
				obj.GetComponent<Image>().sprite = sprites[num2];
				num++;
			}
			int num3 = 0;
			array = syncoButtons;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].GetComponent<Image>().sprite = ((num3 == syncoBeat) ? syncoOn : syncoOff);
				num3++;
			}
		}

		public override void Save(LevelEvent_Base levelEvent)
		{
			int num = 0;
			RDEventTrigger[] array = buttons;
			foreach (RDEventTrigger obj in array)
			{
				int num2 = Array.IndexOf(allIdentifiers, value[num]);
				obj.GetComponent<Image>().sprite = sprites[num2];
				num++;
			}
			int num3 = 0;
			array = syncoButtons;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].GetComponent<Image>().sprite = ((num3 == syncoBeat) ? syncoOn : syncoOff);
				num3++;
			}
			SetEventValue(levelEvent, value);
		}
	}
}
