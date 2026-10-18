using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnBar, 0, RoomsUsage.NotUsed, true, true, true, true, -10, false)]
	public class LevelEvent_TintText : LevelEvent_Base, IDurationHaver, ICompatibleDecorationTypeChecker
	{
		[JsonProperty("", "", null, "", false, true, null, null)]
		public ColorOrPalette? borderColor { get; set; } = Color.black;

		[JsonProperty("", "", null, "", false, true, null, null)]
		public ColorOrPalette? tintColor { get; set; } = Color.white;

		[JsonProperty("", "", null, "", false, true, null, null)]
		[InputField("beats", InputField.LineType.SingleLine, 40, 14, false, false)]
		[FloatInfo(0f, float.PositiveInfinity)]
		public float duration { get; set; }

		[JsonProperty("", "", null, "", false, true, null, null)]
		public Ease ease { get; set; } = Ease.Linear;

		public bool isCompatibleDecorationType => base.ownerDecoration is LevelEvent_Text;

		public override void Run()
		{
			RunOnBeat(() =>
			{
				if (isCompatibleDecorationType && base.vfx.allDecorationLyrics.ContainsKey(target))
				{
					LyricsGame customText = base.vfx.allDecorationLyrics[target];
					if (customText != null)
					{
						float num = base.conductor.DurationBeatsToTime(base.beat - 1f, duration);
						if (borderColor.HasValue)
						{
							customText.KillFade();
							Color color = borderColor.Value.ToColor();
							customText.borderTween?.Kill();
							customText.borderTween = DOTween.To(() => customText.outlineColor, (Color outlineColor) =>
							{
								customText.outlineColor = outlineColor;
							}, color, num).SetEase(ease).Done();
							customText.baseOutlineColor = color;
						}
						if (tintColor.HasValue)
						{
							customText.KillFade();
							Color color2 = tintColor.Value.ToColor();
							customText.tintTween?.Kill();
							customText.tintTween = DOTween.To(() => customText.color, (Color color3) =>
							{
								customText.color = color3;
							}, color2, num).SetEase(ease).Done();
							customText.baseColor = color2;
						}
					}
				}
			});
		}
	}
}
