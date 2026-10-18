using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnBar, -10, RoomsUsage.ManyRooms, true, true, true, false, -10, false)]
	public class LevelEvent_SetVFXPreset : LevelEvent_Base, IDurationHaver
	{
		[JsonProperty("", "", null, "", false, true, null, null)]
		[Dropdown(new string[] { }, false, true)]
		public RDThemeFX preset { get; set; }

		[JsonProperty("", "", null, "", false, true, "EnableIf", null)]
		public bool enable { get; set; } = true;

		[JsonProperty("", "", null, "", false, true, "EnableThresholdIf", null)]
		public float? threshold { get; set; } = 0.3f;

		[JsonProperty("", "", null, "", false, true, "EnableIntensityIf", null)]
		[InputField("%", InputField.LineType.SingleLine, 40, 14, false, false)]
		[FloatInfo(-9999f, 9999f)]
		public float? intensity { get; set; } = 100f;

		[JsonProperty("", "", null, "", false, true, "EnableColorIf", null)]
		[ColorInfo(false)]
		public ColorOrPalette? color { get; set; } = Color.white;

		[JsonProperty("", "", null, "", false, true, "EnableXYIf", null)]
		[PositionPicker(false, false, null, false)]
		public Float2? amount { get; set; } = new Float2(1f, 1f);

		[JsonProperty("", "", null, "", false, true, "EnablePositionIf", null)]
		[PositionPicker(true, true, null, false)]
		public Float2? position { get; set; } = new Float2(50f, 50f);

		[JsonProperty("", "", null, "", false, true, "EnableXYSpeedIf", null)]
		[PositionPicker(false, false, null, false)]
		public Float2? xySpeed { get; set; } = new Float2(0f, 0f);

		[JsonProperty("", "", null, "", false, true, "EnableSpeedIf", null)]
		[InputField("%", InputField.LineType.SingleLine, 40, 14, false, false)]
		[FloatInfo(-9999f, 9999f)]
		public float? speedPerc { get; set; } = 100f;

		[JsonProperty("", "", null, "", false, true, "EnableEaseIf", null)]
		[InputField("beats", InputField.LineType.SingleLine, 40, 14, false, false)]
		public float duration { get; set; }

		[JsonProperty("", "", null, "", false, true, "EnableEaseIf", null)]
		public Ease ease { get; set; } = Ease.Linear;

		public bool EnableIf()
		{
			return preset != RDThemeFX.DisableAll;
		}

		public bool EnableThresholdIf()
		{
			if (enable)
			{
				return RDEditorConstants.VFXInfos[preset].hasThreshold;
			}
			return false;
		}

		public bool EnableColorIf()
		{
			if (enable)
			{
				return RDEditorConstants.VFXInfos[preset].hasColor;
			}
			return false;
		}

		public bool EnableIntensityIf()
		{
			if (enable)
			{
				return RDEditorConstants.VFXInfos[preset].hasIntensity;
			}
			return false;
		}

		public bool EnablePositionIf()
		{
			if (enable)
			{
				return RDEditorConstants.VFXInfos[preset].hasPosition;
			}
			return false;
		}

		public bool EnableXYIf()
		{
			if (enable)
			{
				return RDEditorConstants.VFXInfos[preset].hasXY;
			}
			return false;
		}

		public bool EnableXYSpeedIf()
		{
			if (enable)
			{
				return RDEditorConstants.VFXInfos[preset].hasXYSpeed;
			}
			return false;
		}

		public bool EnableSpeedIf()
		{
			if (enable)
			{
				return RDEditorConstants.VFXInfos[preset].hasSpeed;
			}
			return false;
		}

		public bool EnableEaseIf()
		{
			if (enable)
			{
				return RDEditorConstants.VFXInfos[preset].hasEase;
			}
			return false;
		}

		public void UpdateRoomsUsage()
		{
			roomsUsage = RDEditorConstants.VFXInfos[preset].roomsUsage;
			SanitizeRoomsData();
		}

		public override IEnumerator Prepare()
		{
			int[] array = rooms;
			foreach (int num in array)
			{
				if (num != -1)
				{
					List<RDThemeFX> preloadedVFX = base.game.rooms[num].preloadedVFX;
					if (!preloadedVFX.Contains(preset))
					{
						preloadedVFX.Add(preset);
					}
				}
			}
			prepared = true;
			yield return null;
		}

		public override void Decode(Dictionary<string, object> dict)
		{
			base.Decode(dict);
			bool isPreVFXRefactor = RDLevelData.current.settings.version < 67;
			if (dict.ContainsKey("floatX") && dict.ContainsKey("floatY") && (EnableXYIf() || EnableXYSpeedIf()))
			{
				amount = new Float2(float.Parse(dict["floatX"].ToString()), float.Parse(dict["floatY"].ToString()));
			}
			if (preset == RDThemeFX.HeatDistortion && RDLevelData.current.settings.version < 68)
			{
				position = new Float2(100f, 100f);
			}
			else if (preset == RDThemeFX.CustomScreenScroll && !dict.ContainsKey("xySpeed"))
			{
				xySpeed = amount;
			}
			else if (dict.ContainsKey("amount") && amount.HasValue && !EnableXYIf() && EnablePositionIf())
			{
				Float2 value = amount.Value;
				if (value.xUsed)
				{
					value.x *= 50f;
				}
				if (value.yUsed)
				{
					value.y *= 50f;
				}
				position = value;
			}
			if (!EnableThresholdIf() || LegacyNullCheck("threshold"))
			{
				threshold = 0.3f;
			}
			if (!EnableIntensityIf() || LegacyNullCheck("intensity"))
			{
				intensity = 100f;
			}
			if (!EnableColorIf() || LegacyNullCheck("color"))
			{
				color = Color.white;
			}
			if (!EnableSpeedIf() || LegacyNullCheck("speedPerc"))
			{
				speedPerc = 100f;
			}
			if (!EnableXYIf() || (LegacyNullCheck("amount") && !dict.ContainsKey("floatX") && !dict.ContainsKey("floatY")))
			{
				amount = new Float2(1f, 1f);
			}
			if (!EnableXYSpeedIf())
			{
				xySpeed = new Float2(0f, 0f);
			}
			if (isPreVFXRefactor)
			{
				if (preset == RDThemeFX.Drawing)
				{
					color = Color.black;
					speedPerc = 100f;
				}
				if (preset == RDThemeFX.RadialBlur)
				{
					amount = new Float2(1f, 1f);
				}
			}
			if (RDEditorConstants.VFXInfos[preset].legacy)
			{
				InspectorPanel_SetVFXPreset.AddLegacyPresetToLevel(preset);
			}
			UpdateRoomsUsage();
			bool LegacyNullCheck(string key)
			{
				if (isPreVFXRefactor)
				{
					return !dict.ContainsKey(key);
				}
				return false;
			}
		}

		public override void Run()
		{
			if (preset == RDThemeFX.MiawMiaw)
			{
				return;
			}
			int[] array = rooms;
			foreach (int room in array)
			{
				RunOnBeat(() =>
				{
					if (preset == RDThemeFX.DisableAll)
					{
						if (room == -1)
						{
							foreach (KeyValuePair<RDThemeFX, RDEditorConstants.VFXInfo> vFXInfo in RDEditorConstants.VFXInfos)
							{
								if (vFXInfo.Value.roomsUsage == RoomsUsage.ManyRoomsAndOnTop)
								{
									LevelEvent_Base.level.DisableThemeFX(vFXInfo.Key, room);
								}
							}
							return;
						}
						LevelEvent_Base.level.AddThemeFX(preset, room);
					}
					else if (enable)
					{
						debugStatsCustom = "enable";
						Float2? @float = (EnableXYSpeedIf() ? xySpeed : ((EnablePositionIf() && !EnableXYIf()) ? position : amount));
						float? floatX = ((@float.HasValue && @float.GetValueOrDefault().xUsed) ? new float?(@float.Value.x) : ((float?)null));
						float? floatY = ((@float.HasValue && @float.GetValueOrDefault().yUsed) ? new float?(@float.Value.y) : ((float?)null));
						RDThemeFX rDThemeFX = preset;
						bool flag = rDThemeFX == RDThemeFX.TileN || rDThemeFX == RDThemeFX.EyesBig || rDThemeFX == RDThemeFX.EyesSmall;
						float? extra = ((preset == RDThemeFX.Bloom) ? threshold : ((!flag) ? ((!EnableSpeedIf()) ? ((float?)null) : (speedPerc / 100f)) : position?.x));
						float? extra2 = ((!flag) ? ((float?)null) : position?.y);
						LevelEvent_Base.level.AddThemeFX(preset, room, floatX, floatY, intensity / 100f, extra, extra2, base.conductor.DurationBeatsToTime(base.beat - 1f, duration), ease, color?.ToColor());
					}
					else
					{
						debugStatsCustom = "disable";
						LevelEvent_Base.level.DisableThemeFX(preset, room);
					}
				});
			}
		}

		public override void RunPrebar()
		{
			if (preset == RDThemeFX.MiawMiaw)
			{
				if (enable)
				{
					LevelEvent_Base.level.AddThemeFX(preset);
				}
				else
				{
					LevelEvent_Base.level.DisableThemeFX(preset);
				}
			}
		}

		public override string GetTooltipText()
		{
			List<string> list = new List<string> { RDString.GetEnumValue(preset) };
			if (!enable)
			{
				list.Add(RDString.Get("editor.off"));
			}
			else if (EnableIntensityIf() && intensity.HasValue)
			{
				list.Add(intensity + "%");
			}
			return string.Join('\n', list);
		}
	}
}
