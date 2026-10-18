using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnBar, 0, RoomsUsage.NotUsed, true, true, true, false, -10, false)]
	public class LevelEvent_FadeRoom : LevelEvent_Base, IDurationHaver
	{
		[JsonProperty("", "", null, "", false, true, null, null)]
		[SliderAlpha(false)]
		[IntInfo(0, 100)]
		public int opacity { get; set; } = 100;

		[JsonProperty("", "", null, "", false, true, null, null)]
		[InputField("beats", InputField.LineType.SingleLine, 40, 14, false, false)]
		[IntInfo(0, int.MaxValue)]
		public float duration { get; set; }

		[JsonProperty("", "", null, "", false, true, null, null)]
		public Ease ease { get; set; } = Ease.Linear;

		public override string CustomControlName()
		{
			return "Room";
		}

		public void Validate()
		{
			opacity = Mathf.Clamp(opacity, 0, 100);
		}

		public override void Run()
		{
			RunOnBeat(() =>
			{
				float num = base.conductor.DurationBeatsToTime(base.beat - 1f, duration);
				RDRoom obj = base.game.rooms[y];
				obj.opacityTween?.Kill();
				obj.opacityTween = obj.quadMaterial.DOFloat((float)opacity / 100f, "_Opacity", num).SetEase(ease).Done();
			});
		}

		public override string GetTooltipText()
		{
			return opacity + "%";
		}
	}
}
