using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnBar, 0, RoomsUsage.NotUsed, true, true, true, false, -10, false)]
	public class LevelEvent_SetWindowContent : LevelEvent_Base
	{
		public int window => y;

		[JsonProperty("", "", null, "", false, true, null, null)]
		public WindowContentMode? contentMode { get; set; } = WindowContentMode.OnTop;

		[JsonProperty("", "", null, "", false, true, "EnableIfRoom", null)]
		[Dropdown(null, false, false)]
		public int roomIndex { get; set; }

		[JsonProperty("", "", "editor.MoveCamera.cameraPosition", "", false, true, null, null)]
		[PositionPicker(true, true, null, false)]
		[Float2Info(-1000f, -1000f, 1000f, 1000f)]
		[Off]
		public Float2? position { get; set; } = new Float2(50f, 50f);

		[JsonProperty("", "", "editor.MoveCamera.zoom", "", false, true, null, null)]
		[InputField("%", InputField.LineType.SingleLine, 40, 14, false, false)]
		[IntInfo(1, 9999)]
		[Off]
		public int? zoom { get; set; } = 100;

		[JsonProperty("", "", "editor.MoveCamera.angle", "", false, true, null, null)]
		[InputField("°", InputField.LineType.SingleLine, 40, 14, false, false)]
		[FloatInfo(-9999f, 9999f)]
		[Off]
		public float? angle { get; set; } = 0f;

		[JsonProperty("", "", null, "", false, true, "EnableDurationIf", null)]
		[InputField("beats", InputField.LineType.SingleLine, 40, 14, false, false)]
		public float duration { get; set; } = 1f;

		[JsonProperty("", "", null, "", false, true, "EnableDurationIf", null)]
		public Ease ease { get; set; }

		public bool EnableIfRoom()
		{
			WindowContentMode? windowContentMode = contentMode;
			if (windowContentMode.HasValue)
			{
				return windowContentMode == WindowContentMode.Room;
			}
			return false;
		}

		public bool EnableDurationIf()
		{
			if (!position.HasValue && !zoom.HasValue)
			{
				return angle.HasValue;
			}
			return true;
		}

		public override void Run()
		{
			if (base.game.windowChoreographer == null)
			{
				return;
			}
			RunOnBeat(() =>
			{
				if (contentMode.HasValue)
				{
					int num = ((contentMode == WindowContentMode.OnTop) ? (-1) : roomIndex);
					base.game.windowChoreographer.SetWindowContentToRoom(window, num);
				}
				WindowDancer windowDancer = base.game.windowChoreographer.dancers[window];
				_ = windowDancer.window.material;
				float easeDur = base.conductor.DurationBeatsToTime(base.beat - 1f, duration);
				Float2? @float = position;
				if (@float.HasValue)
				{
					Float2 valueOrDefault = @float.GetValueOrDefault();
					windowDancer.SetCamPosition(valueOrDefault, easeDur, ease);
				}
				float? num2 = angle;
				if (num2.HasValue)
				{
					float valueOrDefault2 = num2.GetValueOrDefault();
					windowDancer.SetCamAngle(valueOrDefault2, easeDur, ease);
				}
				int? num3 = zoom;
				if (num3.HasValue)
				{
					int valueOrDefault3 = num3.GetValueOrDefault();
					windowDancer.SetCamZoom(valueOrDefault3, easeDur, ease);
				}
			});
		}

		public override string GetTooltipText()
		{
			List<string> list = new List<string>();
			if (contentMode.HasValue)
			{
				if (contentMode.Value == WindowContentMode.Room)
				{
					list.Add(RDString.Get("editor.roomIndex").Replace("[index]", (roomIndex + 1).ToString()));
				}
				else if (contentMode.Value == WindowContentMode.OnTop)
				{
					list.Add(RDString.GetEnumValue(contentMode.Value));
				}
			}
			if (((bool?)position) ?? false)
			{
				list.Add(string.Format("{0} {1}", RDString.Get("editor.MoveCamera.cameraPosition"), position));
			}
			if (zoom.HasValue)
			{
				list.Add(string.Format("{0} {1}", RDString.Get("editor.MoveCamera.zoom"), zoom));
			}
			if (angle.HasValue)
			{
				list.Add(string.Format("{0} {1}", RDString.Get("editor.MoveCamera.angle"), angle));
			}
			return string.Join('\n', list);
		}
	}
}
