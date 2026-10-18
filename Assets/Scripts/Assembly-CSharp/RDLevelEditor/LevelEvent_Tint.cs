using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnBar, 0, RoomsUsage.NotUsed, true, true, true, true, -10, false)]
	public class LevelEvent_Tint : LevelEvent_Base, IDurationHaver, ICompatibleDecorationTypeChecker
	{
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

		public bool isCompatibleDecorationType => base.ownerDecoration is LevelEvent_Sprite;

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

		public override string Encode()
		{
			string text = base.Encode();
			float? alpha = borderColor.alpha;
			bool flag;
			if (alpha.HasValue)
			{
				float valueOrDefault = alpha.GetValueOrDefault();
				if (valueOrDefault < 0f || valueOrDefault > 1f)
				{
					flag = true;
					goto IL_003e;
				}
			}
			flag = false;
			goto IL_003e;
			IL_003e:
			if (flag)
			{
				text = text + ", " + RDEditorUtils.EncodeFloat("borderOpacity", borderColor.alpha.Value * 100f, lastValue: true);
			}
			alpha = tintColor.alpha;
			if (alpha.HasValue)
			{
				float valueOrDefault = alpha.GetValueOrDefault();
				if (valueOrDefault < 0f || valueOrDefault > 1f)
				{
					flag = true;
					goto IL_00ac;
				}
			}
			flag = false;
			goto IL_00ac;
			IL_00ac:
			if (flag)
			{
				text = text + ", " + RDEditorUtils.EncodeFloat("tintOpacity", tintColor.alpha.Value * 100f, lastValue: true);
			}
			return text;
		}

		public override void Decode(Dictionary<string, object> dict)
		{
			base.Decode(dict);
			if (dict.ContainsKey("borderOpacity"))
			{
				borderColor = borderColor.WithAlpha((float)Convert.ToInt32(dict["borderOpacity"]) * 0.01f);
			}
			if (dict.ContainsKey("tintOpacity"))
			{
				tintColor = tintColor.WithAlpha((float)Convert.ToInt32(dict["tintOpacity"]) * 0.01f);
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
			}
		}

		public void Validate()
		{
			opacity = (opacity.HasValue ? new int?(Mathf.Clamp(opacity.Value, 0, 100)) : ((int?)null));
		}

		public override void Run()
		{
			RunOnBeat(() =>
			{
				if (isCompatibleDecorationType && LevelEvent_Base.level.sprites.ContainsKey(target))
				{
					CustomSprite customSprite = LevelEvent_Base.level.sprites[target];
					if (customSprite != null)
					{
						float num = base.conductor.DurationBeatsToTime(base.beat - 1f, duration);
						Material material = customSprite.renderer.material;
						if (border.HasValue)
						{
							Color endValue = ((border != BorderType.None) ? borderColor.ToColor() : Color.clear);
							material.SetInt(RDShaderProperties.OutlineID, (border == BorderType.Outline) ? 1 : 0);
							customSprite.borderTween?.Kill();
							customSprite.borderTween = material.DOColor(endValue, RDShaderProperties.GlowColorID, num).SetEase(ease);
							if (border.Value != BorderType.None)
							{
								if (borderPulse.HasValue)
								{
									if ((bool)customSprite.borderGlowTracker)
									{
										customSprite.borderGlowTracker.Delete();
									}
									if (borderPulse.Value)
									{
										customSprite.borderGlowTracker = scrVolumeTracker.CreateAlphaTracker(() => material.GetColor(RDShaderProperties.GlowColorID), (Color col) =>
										{
											material.SetColor(RDShaderProperties.GlowColorID, col);
										}, borderPulseMin, borderPulseMax, endValue.a);
									}
								}
							}
							else if ((bool)customSprite.borderGlowTracker)
							{
								customSprite.borderGlowTracker.Delete();
							}
						}
						if (tint.HasValue)
						{
							Color endValue2 = ((tint == true) ? tintColor.ToColor() : Color.clear);
							customSprite.tintTween?.Kill();
							customSprite.tintTween = material.DOColor(endValue2, RDShaderProperties.OverlayColorID, num).SetEase(ease);
						}
						if (opacity.HasValue)
						{
							customSprite.opacityTween?.Kill();
							customSprite.opacityTween = material.DOFloat((float)opacity.Value / 100f, RDShaderProperties.OpacityID, num).SetEase(ease);
						}
					}
				}
			});
		}
	}
}
