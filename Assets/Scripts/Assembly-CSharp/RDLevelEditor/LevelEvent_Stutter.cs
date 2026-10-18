using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnBar, 0, RoomsUsage.ManyRooms, true, true, true, false, -10, false)]
	public class LevelEvent_Stutter : LevelEvent_Base
	{
		[JsonProperty("", "", null, "", false, true, null, null)]
		[ToggleGroup(false, null, null, null, true, 14)]
		public StutterAction action { get; set; }

		[JsonProperty("", "", null, "", false, true, "EnableIfAdd", null)]
		[FloatInfo(1f, float.PositiveInfinity)]
		public float sourceBeat { get; set; } = 1f;

		[JsonProperty("", "", null, "", false, true, "EnableIfAdd", null)]
		[FloatInfo(0f, float.PositiveInfinity)]
		public float length { get; set; } = 1f;

		[JsonProperty("", "", null, "", false, true, "EnableIfAdd", null)]
		[IntInfo(1, int.MaxValue)]
		public int loops { get; set; } = 1;

		public bool EnableIfAdd()
		{
			return action == StutterAction.Add;
		}

		public override void Decode(Dictionary<string, object> dict)
		{
			base.Decode(dict);
			Validate();
		}

		public void Validate()
		{
			sourceBeat = Mathf.Clamp(sourceBeat, 1f, base.beat);
			length = Mathf.Max(length, 0f);
			loops = Math.Max(loops, 1);
			if (length == 0f)
			{
				length = 1f;
			}
		}

		public override IEnumerator Prepare()
		{
			int[] array = rooms;
			foreach (int num in array)
			{
				base.game.rooms[num].stutter.enabled = true;
			}
			prepared = true;
			yield return null;
		}

		public override void Run()
		{
			int[] array = rooms;
			foreach (int num in array)
			{
				scrStutter stutter = base.game.rooms[num].stutter;
				if (action == StutterAction.Add)
				{
					stutter.AddStutter(base.beat - 1f, sourceBeat - 1f, length, loops);
					continue;
				}
				if (action == StutterAction.Cancel)
				{
					stutter.EndStutterOnBeat(base.beat - 1f);
					continue;
				}
				throw new ArgumentOutOfRangeException("action", action, null);
			}
		}

		public override string GetTooltipText()
		{
			if (action == StutterAction.Cancel)
			{
				return RDString.GetEnumValue(action);
			}
			return string.Format("{0} {1}\n", RDString.Get("editor.Stutter.sourceBeat"), sourceBeat) + string.Format("{0} {1}\n", RDString.Get("editor.Stutter.length"), length) + string.Format("{0} {1}", RDString.Get("editor.Stutter.loops"), loops);
		}
	}
}
