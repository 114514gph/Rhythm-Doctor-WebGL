using System;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnBar, 0, RoomsUsage.ManyRoomsAndOnTop, true, true, true, false, -10, false)]
	public class LevelEvent_PulseCamera : LevelEvent_Base
	{
		[JsonProperty("", "", null, "", false, true, null, null)]
		[Slider(null, null, 0f, true, false)]
		[IntInfo(0, 2)]
		public int strength { get; set; } = 1;

		[JsonProperty("", "", null, "", false, true, null, null)]
		[InputField("pulses", InputField.LineType.SingleLine, 40, 14, false, false)]
		[IntInfo(0, int.MaxValue)]
		public int count { get; set; } = 1;

		[JsonProperty("", "", null, "", false, true, null, null)]
		[InputField("beats", InputField.LineType.SingleLine, 40, 14, false, false)]
		[FloatInfo(0f, float.PositiveInfinity)]
		public float frequency { get; set; } = 1f;

		public override void Run()
		{
			RunOnBeat(() =>
			{
				float pulseTime = Mathf.Min(0.3f, frequency);
				float zoom = strength switch
				{
					0 => 0.03f, 
					1 => 0.1f, 
					2 => 0.2f, 
					_ => throw new ArgumentOutOfRangeException(), 
				};
				frequency = Mathf.Max(0.001f, frequency);
				bool calledCloseToNextBarMaybe = base.beat + 1f >= (float)base.conductor.crotchetsPerBar && !base.conductor.isScrubbingRefreshOnBar;
				float num = base.beat - 1f;
				int[] array = rooms;
				for (int i = 0; i < array.Length; i++)
				{
					int num2 = array[i];
					RDCamera rdCamera = base.vfx.GetCamera(num2).GetComponent<RDCamera>();
					string debugStats = "PulseCamera" + num2;
					for (int j = 0; j < count; j++)
					{
						scrExecuteOnCertainBeat.Add(() =>
						{
							rdCamera.PulseCamera(zoom, pulseTime);
						}, num + frequency * (float)j, debugStats, minusLatency: false, calledCloseToNextBarMaybe);
					}
				}
			});
		}
	}
}
