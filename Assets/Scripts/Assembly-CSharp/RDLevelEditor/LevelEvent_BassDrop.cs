using System;
using UnityEngine;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnBar, 0, RoomsUsage.ManyRoomsAndOnTop, true, true, true, false, -10, false)]
	public class LevelEvent_BassDrop : LevelEvent_Base
	{
		[JsonProperty("", "", null, "", false, true, null, null)]
		[Slider(null, "BassDrop", 0f, true, false)]
		public StrengthLevel strength { get; set; } = StrengthLevel.High;

		[JsonProperty("", "", null, "", false, true, null, null)]
		[Description("editor.BassDrop.description", 42, 16, TextAnchor.MiddleCenter)]
		public bool description => false;

		public override void Run()
		{
			float strengthValue = strength switch
			{
				StrengthLevel.Low => 0.15f, 
				StrengthLevel.Medium => 0.4f, 
				StrengthLevel.High => 1f, 
				_ => throw new ArgumentOutOfRangeException(), 
			};
			int[] array = rooms;
			foreach (int room in array)
			{
				RunOnBeat(() =>
				{
					base.vfx.BassDropNew(room, strengthValue);
				});
			}
		}
	}
}
