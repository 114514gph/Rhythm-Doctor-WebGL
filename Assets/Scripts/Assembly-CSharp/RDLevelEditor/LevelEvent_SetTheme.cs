using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DG.Tweening;
using UnityEngine.UI;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnBar, -10, RoomsUsage.ManyRooms, true, true, true, false, -10, false)]
	public class LevelEvent_SetTheme : LevelEvent_Base
	{
		public bool moveRowContainer;

		[JsonProperty("", "", null, "", false, true, null, null)]
		[Dropdown(new string[]
		{
			"None", "OrientalTechno", "BoyWard", "GirlWard", "Skyline", "InsomniacDay", "ColeWardNight", "Kaleidoscope", "CoffeeShop", "PoliticiansRally",
			"TrainDay", "DesertDay", "Stadium", "HospitalWard", "PaigeOffice", "Basement", "Garden", "Rooftop", "AthleteWard", "RecordsRoom",
			"AbandonedWard", "Intimate", "Sky", "Space", "NeonMuseum", "BackAlley", "Airport", "RollerDisco", "Matrix", "CrossesStraight",
			"CubesFalling", "HallOfMirrors", "FloatingHeart", "FloatingHeartBroken", "ZenGarden", "Vaporwave", "ProceduralTree"
		}, false, true)]
		public RDTheme preset { get; set; }

		[JsonProperty("", "", null, "", false, true, "EnableVariantIf", null)]
		[ToggleGroup(false, null, null, new string[] { "0", "1", "2" }, true, 14)]
		public int variant { get; set; }

		[JsonProperty("", "", "editor.MoveRow.customPosition", "", false, true, "EnablePositionIf", null)]
		public bool enablePosition { get; set; }

		[JsonProperty("", "", null, "", false, true, "EnablePositionFieldIf", null)]
		[FloatInfo(float.NegativeInfinity, float.PositiveInfinity)]
		[Slider("px", null, 8f, true, true)]
		public float positionX { get; set; }

		[JsonProperty("", "", "editor.duration", "", false, true, "EnablePositionFieldIf", null)]
		[InputField("beats", InputField.LineType.SingleLine, 40, 14, false, false)]
		[FloatInfo(0f, float.PositiveInfinity)]
		public float positionDuration { get; set; }

		[JsonProperty("", "", "editor.ease", "", false, true, "EnablePositionFieldIf", null)]
		public Ease positionEase { get; set; } = Ease.Linear;

		[JsonProperty("", "", null, "", false, true, "EnableFirstRowOnFloorIf", null)]
		[Checkbox]
		public bool firstRowOnFloor { get; set; }

		[JsonProperty("", "", null, "", false, true, null, null)]
		[Checkbox]
		public bool skipPaintEffects { get; set; } = true;

		public static bool ThemeHasVariants(RDTheme theme)
		{
			return RDEditorConstants.ThemeVariants.ContainsKey(theme);
		}

		public static bool ThemeHasPositioning(RDTheme theme)
		{
			return RDEditorConstants.EditorThemeBounds.ContainsKey(theme);
		}

		public bool EnableFirstRowOnFloorIf()
		{
			if (RDBase.isDev)
			{
				return firstRowOnFloor;
			}
			return false;
		}

		public bool EnableVariantIf()
		{
			return ThemeHasVariants(preset);
		}

		public bool EnablePositionIf()
		{
			return ThemeHasPositioning(preset);
		}

		public bool EnablePositionFieldIf()
		{
			if (EnablePositionIf())
			{
				return enablePosition;
			}
			return false;
		}

		public override string Encode()
		{
			string text = base.Encode();
			if (!firstRowOnFloor)
			{
				text = Regex.Replace(text, "\"firstRowOnFloor\": false(?:, )?", "");
			}
			return text;
		}

		public override IEnumerator Prepare()
		{
			int[] array = rooms;
			foreach (int num in array)
			{
				base.game.currentLevel.PreloadTheme(preset, num, variant);
			}
			prepared = true;
			yield break;
		}

		public override void Decode(Dictionary<string, object> dict)
		{
			base.Decode(dict);
			if (RDLevelData.current.settings.version < 9 && preset == RDTheme.Kaleidoscope)
			{
				preset = RDTheme.HallOfMirrors;
			}
			moveRowContainer = RDLevelData.current.settings.version < 30;
		}

		public override void Run()
		{
			int[] array = rooms;
			foreach (int room in array)
			{
				RunOnBeat(() =>
				{
					base.game.currentLevel.ShowThemeBackgrounds(preset, room, disableEffects: true, hideOtherBackgrounds: true, moveRowContainer, firstRowOnFloor, !skipPaintEffects, variant);
					if (enablePosition)
					{
						base.game.rooms[room].ScrollTheme(positionX, base.conductor.DurationBeatsToTime(base.beat - 1f, positionDuration), positionEase);
					}
					else
					{
						base.game.rooms[room].TryResetScroll();
					}
				});
			}
		}

		private string VariantText()
		{
			string result = RDString.GetWithCheck($"enum.RDTheme.{preset}.variant{variant}", out var exists);
			if (!exists)
			{
				result = RDString.Get($"editor.SetTheme.defaultVariant{variant}");
			}
			return result;
		}

		public override string GetTooltipText()
		{
			if (!ThemeHasVariants(preset))
			{
				return RDString.GetEnumValue(preset);
			}
			return RDString.GetEnumValue(preset) + " (" + VariantText() + ")";
		}
	}
}
