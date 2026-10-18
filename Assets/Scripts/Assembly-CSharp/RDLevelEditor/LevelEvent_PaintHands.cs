using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnBar, 0, RoomsUsage.ManyRoomsAndOnTop, true, true, true, false, -10, false)]
	public class LevelEvent_PaintHands : LevelEvent_Base, IDurationHaver
	{
		[JsonProperty("", "", null, "", false, true, null, null)]
		[Hand]
		public Hand hands { get; set; } = Hand.Right;

		[JsonProperty("", "", null, "", false, true, null, null)]
		[ToggleGroup(true, null, null, null, false, 14)]
		public BorderType? border { get; set; } = BorderType.None;

		[JsonProperty("", "", null, "", false, true, "EnableBorderColorIf", null)]
		public ColorOrPalette borderColor { get; set; } = Color.white;

		[JsonProperty("", "", "editor.TintRows.borderPulse", "", false, true, "EnableBorderColorIf", null)]
		[Off]
		public bool? borderPulse { get; set; } = true;

		[JsonProperty("", "", "editor.TintRows.borderPulseMin", "", false, true, "EnableBorderPulseIf", null)]
		public float borderPulseMin { get; set; } = 0.1f;

		[JsonProperty("", "", "editor.TintRows.borderPulseMax", "", false, true, "EnableBorderPulseIf", null)]
		public float borderPulseMax { get; set; } = 1f;

		[JsonProperty("", "", null, "", false, true, null, null)]
		public bool? tint { get; set; } = false;

		[JsonProperty("", "", null, "", false, true, "EnableTintColorIf", null)]
		public ColorOrPalette tintColor { get; set; } = Color.white;

		[JsonProperty("", "", null, "", false, true, null, null)]
		[SliderAlpha(false)]
		[IntInfo(0, 100)]
		public int? opacity { get; set; } = 100;

		[JsonProperty("", "", null, "", false, true, null, null)]
		[InputField("beats", InputField.LineType.SingleLine, 40, 14, false, false)]
		[FloatInfo(0f, float.PositiveInfinity)]
		public float duration { get; set; }

		[JsonProperty("", "", null, "", false, true, null, null)]
		public Ease ease { get; set; } = Ease.Linear;

		public bool EnableBorderColorIf()
		{
			if (border.HasValue)
			{
				return border.Value != BorderType.None;
			}
			return false;
		}

		public bool EnableBorderPulseIf()
		{
			if (EnableBorderColorIf())
			{
				return borderPulse ?? false;
			}
			return false;
		}

		public bool EnableTintColorIf()
		{
			return tint == true;
		}

		public void Validate()
		{
			opacity = (opacity.HasValue ? new int?(Mathf.Clamp(opacity.Value, 0, 100)) : ((int?)null));
		}

		public override void Decode(Dictionary<string, object> dict)
		{
			base.Decode(dict);
			if (RDLevelData.current.settings.version < 66)
			{
				if (!dict.ContainsKey("border"))
				{
					border = BorderType.None;
				}
				if (!dict.ContainsKey("tint"))
				{
					tint = false;
				}
				if (!dict.ContainsKey("opacity"))
				{
					opacity = 100;
				}
			}
		}

		public override void Run()
		{
			RunOnBeat(() =>
			{
				Color? color = ((!border.HasValue) ? ((Color?)null) : new Color?((border != BorderType.None) ? borderColor.ToColor() : Color.clear));
				Color? color2 = ((!tint.HasValue) ? ((Color?)null) : new Color?((tint == true) ? tintColor.ToColor() : Color.clear));
				int[] array = rooms;
				foreach (int handNum in array)
				{
					scrHandController handControllerFromInt = base.game.GetHandControllerFromInt(handNum);
					if (handControllerFromInt.DoesEnumTargetLeftHand(hands))
					{
						TintArm(handControllerFromInt.leftArm, color, color2);
					}
					if (handControllerFromInt.DoesEnumTargetRightHand(hands))
					{
						TintArm(handControllerFromInt.rightArm, color, color2);
					}
				}
			});
		}

		private void TintArm(RDArm arm, Color? borderColor, Color? tintColor)
		{
			float num = base.conductor.DurationBeatsToTime(base.beat - 1f, duration);
			Entity[] allSprites = arm.allSprites;
			foreach (Entity ent in allSprites)
			{
				if (!(ent != null))
				{
					continue;
				}
				if (border.HasValue)
				{
					Color color = (arm.dontOutline.Contains(ent) ? Color.clear : borderColor.Value);
					ent.SetBorderType(border == BorderType.Outline);
					ent.TweenBorder(color, num, ease);
					if (border.Value != BorderType.None)
					{
						if (borderPulse.HasValue)
						{
							if ((bool)ent.borderGlowTracker)
							{
								ent.borderGlowTracker.Delete();
							}
							if (borderPulse.Value)
							{
								ent.borderGlowTracker = scrVolumeTracker.CreateAlphaTracker(() => ent.shaderData.borderColor, (Color col) =>
								{
									ent.shaderData.borderColor = col;
								}, borderPulseMin, borderPulseMax, borderColor.Value.a);
							}
						}
					}
					else if ((bool)ent.borderGlowTracker)
					{
						ent.borderGlowTracker.Delete();
					}
				}
				if (tintColor.HasValue)
				{
					ent.TweenTint(tintColor.Value, num, ease);
				}
				if (opacity.HasValue)
				{
					ent.TweenOpacity((float)opacity.Value * 0.01f, num, ease);
				}
			}
		}

		public override string GetTooltipText()
		{
			return RDString.GetEnumValue(hands);
		}
	}
}
