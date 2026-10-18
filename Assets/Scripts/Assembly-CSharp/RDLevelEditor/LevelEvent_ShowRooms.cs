using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnBar, 0, RoomsUsage.ManyRooms, true, true, true, false, -10, false)]
	public class LevelEvent_ShowRooms : LevelEvent_Base, IDurationHaver
	{
		private string parameters;

		private int[] indexesArray;

		private List<float> newHeights;

		[JsonProperty("", "", null, "", false, true, null, null)]
		[ShowRooms]
		public int[] heights { get; set; } = new int[4];

		[JsonProperty("transitionTime", "", null, "", false, true, null, null)]
		[InputField("beats", InputField.LineType.SingleLine, 40, 14, false, false)]
		public float duration { get; set; } = 1f;

		[JsonProperty("", "", null, "", false, true, null, null)]
		public Ease ease { get; set; } = Ease.InOutSine;

		public override IEnumerator Prepare()
		{
			parameters = "";
			newHeights = new List<float>();
			List<int> list = new List<int>();
			for (int i = 0; i < 4; i++)
			{
				bool flag = Array.IndexOf(rooms, i) == -1;
				parameters += i;
				parameters += (flag ? "d" : "");
				parameters += ((i + 1 == 4) ? "" : ",");
				if (!flag)
				{
					list.Add(i);
				}
				newHeights.Add(heights[i]);
			}
			if (rooms.Length == 0)
			{
				Debug.LogWarning("Show Rooms event has no rooms to be shown.");
			}
			indexesArray = list.ToArray();
			prepared = true;
			yield return null;
		}

		public override void Run()
		{
			RunOnBeat(() =>
			{
				if (indexesArray.Length != 0)
				{
					base.vfx.roomsAnimationDuration = base.conductor.DurationBeatsToTime(base.beat - 1f, duration);
					base.vfx.ShowRooms(parameters, ProcessHeights(indexesArray, newHeights), ease);
					base.game.RepositionRowsAndStrips();
				}
			});
		}

		public float[] ProcessHeights(int[] rooms, List<float> values)
		{
			int num = 0;
			for (num = 0; num < values.Count; num++)
			{
				if (Array.IndexOf(rooms, num) == -1)
				{
					values[num] = 0f;
				}
			}
			float[] array = new float[values.Count];
			float num2 = 0f;
			_ = rooms.Length;
			int num3 = 0;
			int[] array2 = rooms;
			foreach (int index in array2)
			{
				num2 += values[index];
				if (values[index] == 0f)
				{
					num3++;
				}
			}
			if (num3 != 0)
			{
				if (num2 <= 100f)
				{
					array2 = rooms;
					foreach (int index2 in array2)
					{
						if (values[index2] == 0f)
						{
							values[index2] = (100f - num2) / (float)num3;
						}
					}
				}
				else
				{
					array2 = rooms;
					foreach (int index3 in array2)
					{
						if (values[index3] != 0f)
						{
							values[index3] = 100f / num2 * values[index3];
						}
					}
				}
			}
			else
			{
				array2 = rooms;
				foreach (int index4 in array2)
				{
					if (values[index4] != 0f)
					{
						values[index4] = 100f / num2 * values[index4];
					}
				}
			}
			float num4 = 0f;
			foreach (float value in values)
			{
				num4 += value;
			}
			if (num4 < 100f)
			{
				float num5 = 100f - num4;
				values[rooms[0]] += num5;
			}
			num = 0;
			foreach (float value2 in values)
			{
				array[num] = value2 * 0.01f;
				num++;
			}
			return array;
		}

		public override string CustomControlName()
		{
			return "ShowRooms";
		}
	}
}
