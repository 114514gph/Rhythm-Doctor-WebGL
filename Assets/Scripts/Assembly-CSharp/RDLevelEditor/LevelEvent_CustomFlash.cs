using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnBar, 0, RoomsUsage.ManyRoomsAndOnTop, true, true, true, false, -10, false)]
	public class LevelEvent_CustomFlash : LevelEvent_Base, IDurationHaver
	{
		[JsonProperty("", "", null, "", false, true, null, null)]
		[ColorInfo(false)]
		public ColorOrPalette? startColor { get; set; } = Color.white;

		[JsonProperty("", "", null, "", false, true, null, null)]
		[SliderAlpha(false)]
		public int? startOpacity { get; set; } = 100;

		[JsonProperty("", "", null, "", false, true, null, null)]
		[ColorInfo(false)]
		public ColorOrPalette? endColor { get; set; } = Color.white;

		[JsonProperty("", "", null, "", false, true, null, null)]
		[SliderAlpha(false)]
		public int? endOpacity { get; set; } = 0;

		[JsonProperty("", "", null, "", false, true, null, null)]
		[ToggleGroup(true, null, null, null, false, 14)]
		public bool background { get; set; }

		[JsonProperty("", "", null, "", false, true, null, null)]
		[InputField("beats", InputField.LineType.SingleLine, 40, 14, false, false)]
		public float duration { get; set; } = 2f;

		[JsonProperty("", "", null, "", false, true, null, null)]
		public Ease ease { get; set; } = Ease.Linear;

		[JsonProperty("", "", null, "", false, true, "EnabledReducedStrengthIf", null)]
		[SliderAlpha(false)]
		[Off]
		public int? reducedStrength { get; set; } = 50;

		public bool EnabledReducedStrengthIf()
		{
			if (!RDBase.isAdvanced && !reducedStrength.HasValue)
			{
				return Persistence.GetReducedFlash();
			}
			return true;
		}

		public override void Decode(Dictionary<string, object> dict)
		{
			base.Decode(dict);
			if (!dict.ContainsKey("startOpacity") && dict.ContainsKey("startColor") && ((string)dict["startColor"]).Length == 8 && startColor.HasValue && startColor.Value.alpha.HasValue)
			{
				startOpacity = (int)(startColor.Value.alpha * 100f).Value;
			}
			if (!dict.ContainsKey("endOpacity") && dict.ContainsKey("endColor") && ((string)dict["endColor"]).Length == 8 && endColor.HasValue && startColor.Value.alpha.HasValue)
			{
				endOpacity = (int)(endColor.Value.alpha * 100f).Value;
			}
		}

		public override void Run()
		{
			RunOnBeat(() =>
			{
				if (base.room != -1 || !background)
				{
					int[] array = rooms;
					foreach (int roomIndex in array)
					{
						scrQuad scrQuad2 = (background ? base.vfx.GetRoomBackgroundOverlay(roomIndex) : base.vfx.GetRoomOverlay(roomIndex));
						float num = (float)Persistence.GetFlashIntensity() * 0.01f;
						float num2 = base.conductor.DurationBeatsToTime(base.beat - 1f, duration);
						float brightness = DOVirtual.EasedValue(num, 1f, RDUtils.Clamp01(num2 / 3f), Ease.InSine);
						float a = scrQuad2.sprite.color.a;
						ColorOrPalette colorOrPalette = startColor.GetValueOrDefault(scrQuad2.sprite.color).WithAlpha(1f);
						ColorOrPalette colorOrPalette2 = endColor.GetValueOrDefault(scrQuad2.sprite.color).WithAlpha(1f);
						float num3 = ((reducedStrength.HasValue && base.game.reducedFlash) ? ((float)Math.Clamp(reducedStrength.Value, 0, 100) / 100f) : 1f);
						scrQuad2.TweenColorFromTo(colorOrPalette.ToColor().WithAlpha(startOpacity.HasValue ? ((float)startOpacity.Value / 100f * num3) : a).WithBrightness(brightness), colorOrPalette2.ToColor().WithAlpha(endOpacity.HasValue ? ((float)endOpacity.Value / 100f) : a).WithBrightness(brightness), num2, ease).Done();
					}
				}
			});
		}
	}
}
