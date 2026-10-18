using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnBar, 0, RoomsUsage.NotUsed, true, true, true, false, -10, false)]
	public class LevelEvent_DesktopColor : LevelEvent_Base, IDurationHaver
	{
		[JsonProperty("", "", null, "", false, true, null, null)]
		public bool custom { get; set; } = true;

		[JsonProperty("", "", "editor.CustomFlash.startColor", "", false, true, "EnableCustomColorIf", null)]
		[ColorInfo(false)]
		[Off]
		public ColorOrPalette? startColor { get; set; } = Color.white;

		[JsonProperty("", "", "editor.CustomFlash.endColor", "", false, true, "EnableCustomColorIf", null)]
		[ColorInfo(false)]
		public ColorOrPalette? endColor { get; set; } = Color.black;

		[JsonProperty("", "", null, "", false, true, "DisableCustomColorIf", null)]
		[Slider("%", null, 0f, true, false)]
		[FloatInfo(0f, 200f)]
		public float brightness { get; set; } = 100f;

		[JsonProperty("", "", null, "", false, true, null, null)]
		[InputField("beats", InputField.LineType.SingleLine, 40, 14, false, false)]
		public float duration { get; set; } = 2f;

		[JsonProperty("", "", null, "", false, true, null, null)]
		public Ease ease { get; set; } = Ease.Linear;

		public bool EnableCustomColorIf()
		{
			return custom;
		}

		public bool DisableCustomColorIf()
		{
			return !custom;
		}

		public Color GetDefaultColor()
		{
			Color simulatedDesktopColor = RDConstants.data.simulatedDesktopColor;
			if (brightness == 100f)
			{
				return simulatedDesktopColor;
			}
			float num = brightness / 100f;
			if (!(brightness < 100f))
			{
				return Color.Lerp(simulatedDesktopColor, Color.white, num - 1f);
			}
			return Color.Lerp(Color.black, simulatedDesktopColor, num);
		}

		public override void Run()
		{
			RunOnBeat(() =>
			{
				Image image = base.game.simulateWD?.background;
				Color color = ((custom && startColor.HasValue) ? startColor.Value.ToColor() : image.color);
				Color color2 = ((!custom) ? GetDefaultColor() : (endColor.HasValue ? endColor.Value.ToColor() : image.color));
				image.DOKill(complete: true);
				image.color = color;
				if (!(color == color2))
				{
					image.DOColor(color2, base.conductor.DurationBeatsToTime(base.beat - 1f, duration)).SetEase(ease).Done();
				}
			});
		}
	}
}
