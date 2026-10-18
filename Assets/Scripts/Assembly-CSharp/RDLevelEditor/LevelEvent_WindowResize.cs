using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnBar, 0, RoomsUsage.NotUsed, true, true, true, false, -10, false)]
	public class LevelEvent_WindowResize : LevelEvent_Base, IDurationHaver
	{
		public int window
		{
			get
			{
				if (tab != Tab.Windows)
				{
					return 0;
				}
				return y;
			}
		}

		[JsonProperty("", "", null, "", false, true, null, null)]
		[Vector2Info(0f, 0f, 2f, 2f)]
		public FloatExpression2? scale { get; set; } = new FloatExpression2(1f, 1f);

		[JsonProperty("", "", null, "", false, true, "EnableIfScale", null)]
		[ToggleGroup(false, null, null, null, true, 14)]
		public ZoomMode zoomMode { get; set; }

		[JsonProperty("", "", null, "", false, true, null, null)]
		[ToggleGroup(false, null, null, null, true, 14)]
		public PivotMode pivotMode { get; set; }

		[JsonProperty("", "", null, "", false, true, "EnablePivotIf", null)]
		[Off]
		public Float2? pivot { get; set; } = new Float2(50f, 50f);

		[JsonProperty("", "", null, "", false, true, "EnableAnchorTypeIf", null)]
		[Dropdown(new string[] { "LeftEdge", "RightEdge", "BottomEdge", "TopEdge" }, false, false)]
		public PivotAnchorType anchorType { get; set; } = PivotAnchorType.LeftEdge;

		[JsonProperty("", "", null, "", false, true, null, null)]
		[InputField("beats", InputField.LineType.SingleLine, 40, 14, false, false)]
		public float duration { get; set; } = 1f;

		[JsonProperty("", "", null, "", false, true, null, null)]
		public Ease ease { get; set; } = Ease.Linear;

		public bool EnableIfScale()
		{
			return scale.HasValue;
		}

		public bool EnablePivotIf()
		{
			return pivotMode == PivotMode.Default;
		}

		public bool EnableAnchorTypeIf()
		{
			return pivotMode == PivotMode.AnchorEdge;
		}

		public override void Decode(Dictionary<string, object> dict)
		{
			base.Decode(dict);
			if (!dict.ContainsKey("zoomMode") && dict.ContainsKey("scale"))
			{
				zoomMode = ZoomMode.None;
			}
		}

		public override void Run()
		{
			if (base.game.windowChoreographer == null)
			{
				return;
			}
			RunOnBeat(() =>
			{
				WindowDancer windowDancer = base.game.windowChoreographer.dancers[window];
				float easeDur = base.conductor.DurationBeatsToTime(base.beat - 1f, duration);
				FloatExpression2? floatExpression = scale;
				if (floatExpression.HasValue)
				{
					Float2 @float = floatExpression.GetValueOrDefault().UnboxToFloat2(LevelEvent_Base.level);
					PivotAnchorType pivotAnchorType = ((pivotMode == PivotMode.AnchorEdge) ? anchorType : PivotAnchorType.None);
					@float.x = Mathf.Min(@float.x, 2f);
					@float.y = Mathf.Min(@float.y, 2f);
					windowDancer.SetScale(@float, easeDur, ease, pivotAnchorType);
					if (zoomMode != ZoomMode.None)
					{
						float a = (@float.xUsed ? @float.x : windowDancer.windowScale.x);
						float b = (@float.yUsed ? @float.y : windowDancer.windowScale.y);
						float num = ((zoomMode == ZoomMode.Fill) ? Mathf.Max(a, b) : Mathf.Min(a, b));
						windowDancer.SetCamZoom(num * 100f, easeDur, ease);
					}
				}
				if (pivotMode == PivotMode.Default)
				{
					Float2? float2 = pivot;
					if (float2.HasValue)
					{
						Float2 valueOrDefault = float2.GetValueOrDefault();
						windowDancer.SetPivot(valueOrDefault / 100f, ReferenceType.Center, ReferenceType.Center, easeDur, ease);
					}
				}
				if (scale.HasValue || pivot.HasValue)
				{
					windowDancer.UpdatePreset();
				}
			});
		}

		public override string GetTooltipText()
		{
			List<string> list = new List<string>();
			if (((bool?)scale) ?? false)
			{
				list.Add(string.Format("{0} {1}", RDString.Get("editor.WindowResize.scale"), scale));
				if (zoomMode != ZoomMode.None)
				{
					list.Add(RDString.GetEnumValue(zoomMode));
				}
			}
			if (pivot.HasValue)
			{
				list.Add(string.Format("{0} {1}", RDString.Get("editor.WindowResize.pivot"), pivot));
			}
			return string.Join('\n', list);
		}
	}
}
