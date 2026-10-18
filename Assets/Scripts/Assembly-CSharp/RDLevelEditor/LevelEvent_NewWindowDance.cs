using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnBar, 0, RoomsUsage.NotUsed, true, true, true, false, -10, false)]
	public class LevelEvent_NewWindowDance : LevelEvent_Base, IDurationHaver
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
		public WindowDancePreset preset { get; set; }

		[JsonProperty("", "", null, "", false, true, "EnablePresetBehaviorIf", null)]
		[ToggleGroup(false, null, null, null, true, 14)]
		public SamePresetBehavior samePresetBehavior { get; set; } = SamePresetBehavior.Reset;

		[JsonProperty("", "", null, "", false, true, null, null)]
		public Float2 position { get; set; } = new Float2(50f, 50f);

		[JsonProperty("", "", null, "", false, true, "EnableReferenceIf", null)]
		[ToggleGroup(false, null, null, null, false, 14)]
		public ReferenceType reference { get; set; }

		[JsonProperty("", "", null, "", false, true, "EnableSpeedIf", null)]
		[InputField("%/beat", InputField.LineType.SingleLine, 40, 14, false, false)]
		public float speed { get; set; }

		[JsonProperty("", "", null, "", false, true, "EnableUseCircleIf", null)]
		public bool useCircle { get; set; }

		[JsonProperty("", "", null, "", false, true, "EnableAmplitudeIf", null)]
		[InputField("%", InputField.LineType.SingleLine, 40, 14, false, false)]
		[Off]
		public float? amplitude { get; set; } = 0f;

		[JsonProperty("", "", null, "", false, true, "EnableAmplitudeVectorIf", null)]
		public Float2 amplitudeVector { get; set; } = new Float2(0f, 0f);

		[JsonProperty("", "", null, "", false, true, "EnableAngleIf", null)]
		[InputField("°", InputField.LineType.SingleLine, 40, 14, false, false)]
		[Off]
		public float? angle { get; set; } = 0f;

		[JsonProperty("", "", null, "", false, true, "EnableFrequencyIf", null)]
		[InputField("/beat", InputField.LineType.SingleLine, 40, 14, false, false)]
		public float frequency { get; set; }

		[JsonProperty("", "", null, "", false, true, "EnablePeriodIf", null)]
		[InputField("beats", InputField.LineType.SingleLine, 40, 14, false, false)]
		public float period { get; set; }

		[JsonProperty("", "", null, "", false, true, "EnableSubEaseIf", null)]
		public Ease subEase { get; set; } = Ease.Linear;

		[JsonProperty("", "", null, "", false, true, "EnableEaseTypeIf", null)]
		[ToggleGroup(false, null, null, null, false, 14)]
		public EasingType easeType { get; set; }

		[JsonProperty("", "", null, "", false, true, null, null)]
		[InputField("beats", InputField.LineType.SingleLine, 40, 14, false, false)]
		public float easingDuration
		{
			get
			{
				return duration;
			}
			set
			{
				duration = value;
			}
		}

		public float duration { get; set; } = 1f;

		[JsonProperty("", "", null, "", false, true, null, null)]
		public Ease ease { get; set; } = Ease.Linear;

		private bool amplitudeIsVector
		{
			get
			{
				if (preset != WindowDancePreset.Ellipse || useCircle)
				{
					return preset == WindowDancePreset.ShakePer;
				}
				return true;
			}
		}

		private bool presetBehaviorIsKeep => samePresetBehavior == SamePresetBehavior.Keep;

		public bool EnableReferenceIf()
		{
			return preset == WindowDancePreset.Move;
		}

		public bool EnablePresetBehaviorIf()
		{
			return preset != WindowDancePreset.Move;
		}

		public bool EnableSpeedIf()
		{
			WindowDancePreset windowDancePreset = preset;
			if (windowDancePreset == WindowDancePreset.Wrap || windowDancePreset == WindowDancePreset.Ellipse)
			{
				return !presetBehaviorIsKeep;
			}
			return false;
		}

		public bool EnableUseCircleIf()
		{
			return preset == WindowDancePreset.Ellipse;
		}

		public bool EnableAmplitudeIf()
		{
			if (!amplitudeIsVector)
			{
				WindowDancePreset windowDancePreset = preset;
				return windowDancePreset == WindowDancePreset.Sway || windowDancePreset == WindowDancePreset.Wrap || windowDancePreset == WindowDancePreset.Ellipse || windowDancePreset == WindowDancePreset.ShakePer;
			}
			return false;
		}

		public bool EnableAmplitudeVectorIf()
		{
			if (amplitudeIsVector)
			{
				WindowDancePreset windowDancePreset = preset;
				return windowDancePreset == WindowDancePreset.Sway || windowDancePreset == WindowDancePreset.Wrap || windowDancePreset == WindowDancePreset.Ellipse || windowDancePreset == WindowDancePreset.ShakePer;
			}
			return false;
		}

		public bool EnableAngleIf()
		{
			WindowDancePreset windowDancePreset = preset;
			if (windowDancePreset != WindowDancePreset.Sway && windowDancePreset != WindowDancePreset.Ellipse)
			{
				if (preset == WindowDancePreset.Wrap)
				{
					return !presetBehaviorIsKeep;
				}
				return false;
			}
			return true;
		}

		public bool EnableFrequencyIf()
		{
			WindowDancePreset windowDancePreset = preset;
			if (windowDancePreset == WindowDancePreset.Sway || windowDancePreset == WindowDancePreset.Wrap || windowDancePreset == WindowDancePreset.ShakePer)
			{
				return !presetBehaviorIsKeep;
			}
			return false;
		}

		public bool EnablePeriodIf()
		{
			if (preset == WindowDancePreset.ShakePer)
			{
				return !presetBehaviorIsKeep;
			}
			return false;
		}

		public bool EnableSubEaseIf()
		{
			WindowDancePreset windowDancePreset = preset;
			if (windowDancePreset == WindowDancePreset.Sway || windowDancePreset == WindowDancePreset.ShakePer)
			{
				return !presetBehaviorIsKeep;
			}
			return false;
		}

		public bool EnableEaseTypeIf()
		{
			if (preset == WindowDancePreset.Sway)
			{
				return !presetBehaviorIsKeep;
			}
			return false;
		}

		public override void Decode(Dictionary<string, object> dict)
		{
			base.Decode(dict);
			int version = RDLevelData.current.settings.version;
			if (version < 56)
			{
				if (dict.ContainsKey("usePosition"))
				{
					samePresetBehavior = SamePresetBehavior.Reset;
					if (RDUtils.ParseEnum(dict["usePosition"] as string, PositionType.New) == PositionType.Current)
					{
						position = Float2.empty;
					}
				}
				angle = 90f - angle % 360f;
				switch (preset)
				{
				case WindowDancePreset.Wrap:
					amplitude *= 2f;
					amplitudeVector *= 2f;
					break;
				case WindowDancePreset.Sway:
					subEase = ease;
					frequency = ((easingDuration == 0f) ? easingDuration : (1f / easingDuration));
					break;
				case WindowDancePreset.ShakePer:
					subEase = ease;
					frequency = 1f / Mathf.Max(period, 0.001f);
					period = easingDuration;
					easingDuration = 0f;
					break;
				}
			}
			if (version < 63 && preset == WindowDancePreset.Ellipse)
			{
				speed *= 100f;
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
				Float2 @float = (amplitudeIsVector ? amplitudeVector : (amplitude.HasValue ? (Float2.one * amplitude.Value) : Float2.zero));
				SamePresetBehavior samePresetBehavior = ((preset != WindowDancePreset.Move) ? this.samePresetBehavior : SamePresetBehavior.Keep);
				float num = speed / 100f / base.conductor.unscaledCrotchet;
				float easeDur = base.conductor.DurationBeatsToTime(base.beat - 1f, easingDuration);
				float num2 = frequency / base.conductor.unscaledCrotchet;
				float num3 = base.conductor.DurationBeatsToTime(base.beat - 1f, period);
				WindowDancePresetInfo presetInfo = new WindowDancePresetInfo
				{
					preset = preset,
					samePresetBehavior = samePresetBehavior,
					pos = new Vector2(position.xUsed ? (position.x / 100f) : 0f, position.yUsed ? (position.y / 100f) : 0f),
					speed = num,
					amplitude = new Vector2(@float.xUsed ? (@float.x / 100f) : 0f, @float.yUsed ? (@float.y / 100f) : 0f),
					angle = (angle.HasValue ? (angle.Value * (MathF.PI / 180f)) : 0f),
					frequency = num2,
					period = num3,
					easeType = easeType,
					subEase = subEase,
					easeDur = easeDur,
					ease = ease
				};
				bool usingAmpX = (amplitudeIsVector ? @float.xUsed : amplitude.HasValue);
				bool usingAmpY = (amplitudeIsVector ? @float.yUsed : amplitude.HasValue);
				windowDancer.ChangePreset(presetInfo, position.xUsed, position.yUsed, angle.HasValue, usingAmpX, usingAmpY);
				ReferenceType referenceType = ((preset == WindowDancePreset.Move) ? reference : ReferenceType.Center);
				if (referenceType != ReferenceType.Center || !windowDancer.pivotIsInCenterMode)
				{
					windowDancer.SetPivot(Float2.one * 0.5f, referenceType, referenceType, easeDur, ease);
				}
				windowDancer.UpdatePreset();
				base.game.windowDanceEventCalled = true;
			});
		}

		public override string GetTooltipText()
		{
			List<string> list = new List<string> { RDString.GetEnumValue(preset) };
			if (position.xUsed || position.yUsed)
			{
				list.Add(string.Format("{0} {1}", RDString.Get("editor.NewWindowDance.position"), position));
			}
			if (EnableSpeedIf())
			{
				list.Add(string.Format("{0} {1}{2}", RDString.Get("editor.NewWindowDance.speed"), speed, RDString.Get("editor.%/beat")));
			}
			if (amplitude.HasValue && EnableAmplitudeIf())
			{
				list.Add(string.Format("{0} {1}%", RDString.Get("editor.NewWindowDance.amplitude"), amplitude));
			}
			if (EnableFrequencyIf())
			{
				list.Add(string.Format("{0} {1}{2}", RDString.Get("editor.NewWindowDance.frequency"), frequency, RDString.Get("editor./beat")));
			}
			return string.Join('\n', list);
		}
	}
}
