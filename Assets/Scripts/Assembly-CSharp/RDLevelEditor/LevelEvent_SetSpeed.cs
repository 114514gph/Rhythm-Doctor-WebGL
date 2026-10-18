using DG.Tweening;
using UnityEngine.UI;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnBar, 0, RoomsUsage.NotUsed, true, true, true, false, -10, false)]
	public class LevelEvent_SetSpeed : LevelEvent_Base, IDurationHaver
	{
		private static Tween setSpeedTween;

		[JsonProperty("", "", null, "", false, true, null, null)]
		[FloatInfo(0f, float.PositiveInfinity)]
		public float speed { get; set; } = 1f;

		[JsonProperty("", "", null, "", false, true, null, null)]
		[InputField("beats", InputField.LineType.SingleLine, 40, 14, false, false)]
		[FloatInfo(0f, float.PositiveInfinity)]
		public float duration { get; set; }

		[JsonProperty("", "", null, "", false, true, null, null)]
		public Ease ease { get; set; } = Ease.Linear;

		public override void Run()
		{
			RunOnBeat(() =>
			{
				setSpeedTween?.Kill();
				float num = base.conductor.DurationBeatsToTime(base.beat - 1f, duration);
				float gameSpeed = base.game.visualSpeed / RDTime.speed;
				setSpeedTween = DOTween.To(() => gameSpeed, (float x) =>
				{
					gameSpeed = x;
				}, speed, num).SetEase(ease).SetUpdate(isIndependentUpdate: true)
					.OnComplete(() =>
					{
						base.game.SetVisualSpeed(gameSpeed);
					})
					.OnUpdate(() =>
					{
						base.game.SetVisualSpeed(gameSpeed);
					})
					.Done();
			});
		}

		public override string GetTooltipText()
		{
			return speed.ToString();
		}
	}
}
