using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnBar, 0, RoomsUsage.ManyRoomsAndOnTop, true, true, true, false, -1, false)]
	public class LevelEvent_TintRows : LevelEvent_Base, IDurationHaver
	{
		[JsonProperty("", "", null, "", false, true, null, null)]
		[ToggleGroup(true, null, null, null, false, 14)]
		public BorderType? border { get; set; } = BorderType.None;

		[JsonProperty("", "", null, "", false, true, "EnableBorderColorIf", null)]
		[ColorInfo(true)]
		public ColorOrPalette borderColor { get; set; } = Color.white;

		[JsonProperty("", "", null, "", false, true, "EnableBorderOpacityIf", null)]
		[InputField("%", InputField.LineType.SingleLine, 40, 14, false, false)]
		public int borderOpacity { get; set; } = 100;

		[JsonProperty("", "", null, "", false, true, "EnableBorderColorIf", null)]
		[Off]
		public bool? borderPulse { get; set; } = true;

		[JsonProperty("", "", null, "", false, true, "EnableBorderPulseIf", null)]
		public float borderPulseMin { get; set; } = 0.1f;

		[JsonProperty("", "", null, "", false, true, "EnableBorderPulseIf", null)]
		public float borderPulseMax { get; set; } = 1f;

		[JsonProperty("", "", null, "", false, true, null, null)]
		public bool? tint { get; set; } = false;

		[JsonProperty("", "", null, "", false, true, "EnableTintColorIf", null)]
		[ColorInfo(true)]
		public ColorOrPalette tintColor { get; set; } = Color.white;

		[JsonProperty("", "", null, "", false, true, "EnableTintOpacityIf", null)]
		[InputField("%", InputField.LineType.SingleLine, 40, 14, false, false)]
		public int tintOpacity { get; set; } = 100;

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

		[JsonProperty("", "", null, "", false, true, null, null)]
		[Off]
		[Dropdown(new string[] { "None", "Electric" }, false, false)]
		public RowEffect? effect { get; set; } = RowEffect.None;

		[JsonProperty("", "", "editor.PlaySound.sound", "", false, true, "EnableEffectSoundIf", null)]
		[Checkbox]
		public bool effectSound { get; set; } = true;

		[JsonProperty("", "", "editor.MoveRow.Heart", "", false, true, null, null)]
		[Dropdown(null, false, false)]
		[Off]
		public RDHeartType? heart { get; set; } = RDHeartType.Default;

		[JsonProperty("", "", null, "", false, true, "EnableHeartTransitionIf", null)]
		[Checkbox]
		public bool heartTransition { get; set; } = true;

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

		public bool EnableBorderOpacityIf()
		{
			if (EnableBorderColorIf())
			{
				return borderOpacity != 100;
			}
			return false;
		}

		public bool EnableTintOpacityIf()
		{
			if (EnableTintColorIf())
			{
				return tintOpacity != 100;
			}
			return false;
		}

		public bool EnableEffectSoundIf()
		{
			if (effect.HasValue)
			{
				return effect.Value == RowEffect.Electric;
			}
			return false;
		}

		public bool EnableHeartTransitionIf()
		{
			return heart.HasValue;
		}

		public override void Decode(Dictionary<string, object> dict)
		{
			base.Decode(dict);
			if (dict.ContainsKey("borderOpacity"))
			{
				int num = Convert.ToInt32(dict["borderOpacity"]);
				if (num >= 0 && num <= 100 && borderColor.ToColor().a == 1f)
				{
					borderColor = borderColor.WithAlpha((float)num / 100f);
					borderOpacity = 100;
				}
			}
			if (dict.ContainsKey("tintOpacity"))
			{
				int num2 = Convert.ToInt32(dict["tintOpacity"]);
				if (num2 >= 0 && num2 <= 100 && tintColor.ToColor().a == 1f)
				{
					tintColor = tintColor.WithAlpha((float)num2 / 100f);
					tintOpacity = 100;
				}
			}
			if (RDLevelData.current.settings.version < 54)
			{
				base.room = 4;
			}
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
				if (!dict.ContainsKey("effect"))
				{
					effect = RowEffect.None;
				}
			}
			UpdateRoomsUsage();
		}

		public void UpdateRoomsUsage()
		{
			RoomsUsage roomsUsage = ((base.row == -1) ? RoomsUsage.ManyRoomsAndOnTop : RoomsUsage.NotUsed);
			base.roomsUsage = roomsUsage;
			SanitizeRoomsData();
		}

		public override void RunPrebar()
		{
			if (effect.HasValue && effect.Value == RowEffect.Electric && effectSound)
			{
				base.conductor.PlayBeatInCrotchets("sndInsomniacVirusCue", base.beat - 1f, 1f, RDUtils.GetMixerGroup("ElectricCues"));
			}
		}

		public static Color GetTintColor(Color col, int opacity, bool shouldColor)
		{
			if (!shouldColor)
			{
				return Color.clear;
			}
			return col.WithAlpha(col.a * (float)opacity / 100f);
		}

		public override void Run()
		{
			RunOnBeat(() =>
			{
				Color? color = (border.HasValue ? new Color?(GetTintColor(borderColor.ToColor(), borderOpacity, border.Value != BorderType.None)) : ((Color?)null));
				Color? color2 = (tint.HasValue ? new Color?(GetTintColor(tintColor.ToColor(), tintOpacity, tint.Value)) : ((Color?)null));
				if (base.row == -1)
				{
					Row[] rows = base.game.rows;
					foreach (Row row in rows)
					{
						if (base.room == -1 || Array.IndexOf(rooms, row.room) != -1)
						{
							TintRow(row, tintShadowOrHost: false, color, color2);
						}
					}
				}
				else
				{
					TintRow(base.game.rows[base.row], tintShadowOrHost: true, color, color2);
				}
			});
		}

		private void TintRow(Row prop, bool tintShadowOrHost, Color? borderColor, Color? tintColor)
		{
			RowEntity entity = prop.ent;
			if (entity != null)
			{
				float num = base.conductor.DurationBeatsToTime(base.beat - 1f, duration);
				if (effect.HasValue)
				{
					entity.SetEffect(effect.Value);
				}
				if (heart.HasValue)
				{
					entity.SetHeart(heart.Value, heartTransition);
				}
				if (borderColor.HasValue)
				{
					entity.SetBorderType(border == BorderType.Outline);
					entity.playerBox.SetBorderType(border == BorderType.Outline);
					entity.TweenBorder(borderColor.Value, num, ease);
					entity.playerBox.TweenBorder(borderColor.Value, num, ease);
					if (border.Value != BorderType.None)
					{
						if (num > 0f)
						{
							entity.borderColorTween.OnUpdate(() =>
							{
								entity.playerBox.lastGlowColor = entity.shaderData.borderColor;
							});
						}
						else
						{
							entity.playerBox.lastGlowColor = borderColor.Value;
						}
						if (borderPulse.HasValue)
						{
							if ((bool)entity.borderGlowTracker)
							{
								entity.borderGlowTracker.Delete();
							}
							if (borderPulse.Value)
							{
								entity.borderGlowTracker = scrVolumeTracker.CreateAlphaTracker(() => entity.shaderData.borderColor, (Color col) =>
								{
									entity.shaderData.borderColor = col;
									entity.playerBox.shaderData.borderColor = col;
								}, borderPulseMin, borderPulseMax, borderColor.Value.a);
							}
						}
					}
					else
					{
						entity.playerBox.lastGlowColor = Color.clear;
						if ((bool)entity.borderGlowTracker)
						{
							entity.borderGlowTracker.Delete();
						}
					}
				}
				if (tintColor.HasValue)
				{
					entity.TweenTint(tintColor.Value, num, ease);
					entity.playerBox.TweenTint(tintColor.Value, num, ease);
				}
				if (opacity.HasValue)
				{
					entity.TweenOpacity((float)opacity.Value / 100f, num, ease);
					entity.playerBox.TweenOpacity((float)opacity.Value / 100f, num, ease);
				}
			}
			if (!tintShadowOrHost)
			{
				return;
			}
			if (base.game.shadowRowStyle == ShadowRowStyle.HostToShadowIf2P && prop.shadowOrHostID != -1)
			{
				TintRow(base.game.rows[prop.shadowOrHostID], tintShadowOrHost: false, borderColor, tintColor);
			}
			else
			{
				if (base.game.shadowRowStyle != ShadowRowStyle.ShadowToHost)
				{
					return;
				}
				foreach (Row item in base.game.rows.Where((Row r) => r.isShadowRealtime && r.shadowOrHostIDRealtime == base.row && r.copyPaintEffectsWhenShadowOrHost))
				{
					TintRow(item, tintShadowOrHost: false, borderColor, tintColor);
				}
			}
		}

		public override string GetTooltipText()
		{
			List<string> list = new List<string>();
			if (base.row < 0 || base.row >= base.editor.rowsData.Count)
			{
				list.Add(RDString.Get("editor.TintRows.rows.allRows"));
			}
			else
			{
				LevelEvent_MakeRow levelEvent_MakeRow = base.editor.rowsData[base.row];
				list.Add(levelEvent_MakeRow.GetRowString());
			}
			if (effect.HasValue && effect != RowEffect.None)
			{
				list.Add(RDString.GetEnumValue(effect.Value));
			}
			if (heart.HasValue)
			{
				list.Add(RDString.Get("editor.MoveRow.Heart") + ": " + RDString.GetEnumValue(heart.Value));
			}
			return string.Join('\n', list);
		}
	}
}
