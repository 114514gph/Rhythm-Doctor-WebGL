using System.Collections.Generic;
using System.Text;
using DG.Tweening;
using UnityEngine;

namespace RDLevelEditor
{
	public static class RDEditorConstants
	{
		public struct VFXInfo
		{
			public RoomsUsage roomsUsage;

			public bool hasEase;

			public bool hasThreshold;

			public bool hasIntensity;

			public bool hasColor;

			public bool hasPosition;

			public bool hasXY;

			public bool hasXYSpeed;

			public bool hasSpeed;

			public bool legacy;

			public Color? defaultColor;

			public Float2? defaultPosition;
		}

		public const int CurrentVersion = 68;

		public static readonly string DisplayVersion = $"r{68}";

		public static readonly Encoding DefaultLevelEncoding = Encoding.UTF8;

		public const string DefaultPlaySongFilename = "sndOrientalTechno";

		public const string SettingsKey = "settings";

		public const string RowsKey = "rows";

		public const string DecorationsKey = "decorations";

		public const string EventsKey = "events";

		public const string ConditionalsKey = "conditionals";

		public const string BookmarksKey = "bookmarks";

		public const string ColorPaletteKey = "colorPalette";

		public const string EventsTypeKey = "type";

		public const string BorderKey = "border";

		public const string BorderColorKey = "borderColor";

		public const string LegacyBorderOpacityKey = "borderOpacity";

		public const string TintKey = "tint";

		public const string TintColorKey = "tintColor";

		public const string LegacyTintOpacityKey = "tintOpacity";

		public const string LevelEventClassFullName = "RDLevelEditor.LevelEvent_";

		public const string LevelEventClassName = "LevelEvent_";

		public const string LevelEventControlPrefix = "LevelEventControl_";

		public const string InspectorPanelClassPrefix = "RDLevelEditor.InspectorPanel_";

		public const string InspectorPanelPrefix = "InspectorPanel_";

		public const string EditorStringsKey = "editor.";

		public const string WhiteHexColor = "ffffff";

		public const string DefaultHexColor = "000000";

		public const string DefaultHexColorWithAlpha = "00000000";

		public const string NoKillOnReloadId = "noKillOnReload";

		public const int MaxRowsPerPage = 4;

		public const int RoomCount = 4;

		public const int WindowCount = 4;

		public const int OnTopIndex = 4;

		public const int PaletteBackground = 9;

		public const int ColorPaletteSize = 21;

		public const int MostOlderFilesToShow = 8;

		public const string InternalSoundPrefix = "snd";

		public static Color NeedsReloadColor = "FFC660FF".HexToColor();

		public static Color NormalColor = Color.white;

		public const float minPreviewSongTimeLength = 5f;

		public const float maxPreviewSongTimeLength = 15f;

		public static Dictionary<Tab, List<LevelEventType>> levelEventTabs = new Dictionary<Tab, List<LevelEventType>>();

		public static Dictionary<Tab, List<LevelEventType>> hiddenEvents = new Dictionary<Tab, List<LevelEventType>>();

		public static Dictionary<Tab, List<LevelEventType>> advancedEvents = new Dictionary<Tab, List<LevelEventType>>();

		public static Dictionary<Tab, List<RDEditorKeycode>> eventKeyCodes = new Dictionary<Tab, List<RDEditorKeycode>>();

		public static Dictionary<GameSoundType, GameSoundType[]> gameSoundGroups = new Dictionary<GameSoundType, GameSoundType[]>();

		public static readonly Dictionary<RDThemeFX, VFXInfo> VFXInfos = new Dictionary<RDThemeFX, VFXInfo>
		{
			[RDThemeFX.Vignette] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRooms
			},
			[RDThemeFX.VignetteFlicker] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRooms
			},
			[RDThemeFX.CutsceneMode] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRooms
			},
			[RDThemeFX.WavyRows] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRooms,
				hasIntensity = true,
				hasEase = true,
				hasSpeed = true
			},
			[RDThemeFX.LightStripVert] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRooms
			},
			[RDThemeFX.SilhouettesOnHBeat] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRooms
			},
			[RDThemeFX.ColourfulShockwaves] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRooms
			},
			[RDThemeFX.BassDropOnHit] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRooms
			},
			[RDThemeFX.ShakeOnHeartBeat] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRooms
			},
			[RDThemeFX.ShakeOnHit] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRooms
			},
			[RDThemeFX.NumbersAbovePulses] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRooms
			},
			[RDThemeFX.FallingPetals] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRooms
			},
			[RDThemeFX.FallingPetalsInstant] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRooms
			},
			[RDThemeFX.FallingPetalsSnow] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRooms
			},
			[RDThemeFX.FallingLeaves] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRooms
			},
			[RDThemeFX.Rain] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRooms,
				hasIntensity = true,
				hasEase = true
			},
			[RDThemeFX.Snow] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRooms
			},
			[RDThemeFX.Blizzard] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRooms,
				hasIntensity = true,
				hasEase = true
			},
			[RDThemeFX.Embers] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRooms,
				hasIntensity = true,
				hasEase = true,
				hasColor = true,
				defaultColor = new Color32(208, 73, 73, byte.MaxValue)
			},
			[RDThemeFX.Matrix] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRooms
			},
			[RDThemeFX.Diamonds] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRooms,
				hasIntensity = true,
				hasEase = true,
				hasColor = true
			},
			[RDThemeFX.Confetti] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRooms
			},
			[RDThemeFX.ConfettiBurst] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRooms
			},
			[RDThemeFX.Balloons] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRooms
			},
			[RDThemeFX.VHS] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRooms
			},
			[RDThemeFX.VHSRewind] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRooms,
				hasIntensity = true,
				hasEase = true
			},
			[RDThemeFX.Scanlines] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRooms
			},
			[RDThemeFX.Aberration] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRooms,
				hasIntensity = true,
				hasEase = true
			},
			[RDThemeFX.JPEG] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRooms,
				hasIntensity = true,
				hasEase = true
			},
			[RDThemeFX.Grain] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRooms,
				hasIntensity = true,
				hasEase = true
			},
			[RDThemeFX.Blur] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRooms,
				hasIntensity = true,
				hasEase = true
			},
			[RDThemeFX.RadialBlur] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRooms,
				hasIntensity = true,
				hasPosition = true,
				hasEase = true
			},
			[RDThemeFX.Fisheye] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRoomsAndOnTop,
				hasIntensity = true,
				hasEase = true
			},
			[RDThemeFX.HallOfMirrors] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRoomsAndOnTop
			},
			[RDThemeFX.TileN] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRoomsAndOnTop,
				hasEase = true,
				hasXY = true,
				hasPosition = true,
				defaultPosition = new Float2(0f, 0f)
			},
			[RDThemeFX.CustomScreenScroll] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRoomsAndOnTop,
				hasEase = true,
				hasXYSpeed = true
			},
			[RDThemeFX.ScreenWaves] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRooms,
				hasIntensity = true,
				hasEase = true
			},
			[RDThemeFX.HeatDistortion] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRooms,
				hasIntensity = true,
				hasPosition = true,
				hasEase = true
			},
			[RDThemeFX.Pixelate] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRooms,
				hasXY = true,
				hasEase = true
			},
			[RDThemeFX.Mosaic] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRooms,
				hasIntensity = true,
				hasEase = true
			},
			[RDThemeFX.GlassShatter] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRooms
			},
			[RDThemeFX.GlitchObstruction] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRoomsAndOnTop
			},
			[RDThemeFX.Noise] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRoomsAndOnTop
			},
			[RDThemeFX.HueShift] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRoomsAndOnTop,
				hasIntensity = true,
				hasEase = true
			},
			[RDThemeFX.Brightness] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRoomsAndOnTop,
				hasIntensity = true,
				hasEase = true
			},
			[RDThemeFX.Contrast] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRoomsAndOnTop,
				hasIntensity = true,
				hasEase = true
			},
			[RDThemeFX.Saturation] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRoomsAndOnTop,
				hasIntensity = true,
				hasEase = true
			},
			[RDThemeFX.BlackAndWhite] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRoomsAndOnTop
			},
			[RDThemeFX.Sepia] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRoomsAndOnTop
			},
			[RDThemeFX.Bloom] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRooms,
				hasIntensity = true,
				hasEase = true,
				hasColor = true,
				hasThreshold = true
			},
			[RDThemeFX.OrangeBloom] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRooms
			},
			[RDThemeFX.BlueBloom] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRooms
			},
			[RDThemeFX.Funk] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRooms
			},
			[RDThemeFX.Drawing] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRooms,
				hasIntensity = true,
				hasEase = true,
				hasSpeed = true,
				hasColor = true,
				defaultColor = Color.black
			},
			[RDThemeFX.Dots] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRooms,
				hasIntensity = true,
				hasEase = true
			},
			[RDThemeFX.EyesBig] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRooms,
				hasIntensity = true,
				hasXYSpeed = true,
				hasEase = true,
				hasColor = true,
				hasPosition = true
			},
			[RDThemeFX.EyesSmall] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRooms,
				hasIntensity = true,
				hasXYSpeed = true,
				hasEase = true,
				hasColor = true,
				hasPosition = true
			},
			[RDThemeFX.Tutorial] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRooms,
				hasIntensity = true,
				hasEase = true,
				hasColor = true
			},
			[RDThemeFX.Tile2] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRoomsAndOnTop,
				legacy = true
			},
			[RDThemeFX.Tile3] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRoomsAndOnTop,
				legacy = true
			},
			[RDThemeFX.Tile4] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRoomsAndOnTop,
				legacy = true
			},
			[RDThemeFX.ScreenScrollX] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRooms,
				legacy = true
			},
			[RDThemeFX.ScreenScroll] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRooms,
				legacy = true
			},
			[RDThemeFX.ScreenScrollSansVHS] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRooms,
				legacy = true
			},
			[RDThemeFX.ScreenScrollXSansVHS] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRooms,
				legacy = true
			},
			[RDThemeFX.RowGlowWhite] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRooms,
				legacy = true
			},
			[RDThemeFX.RowOutline] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRooms,
				legacy = true
			},
			[RDThemeFX.RowShadow] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRooms,
				legacy = true
			},
			[RDThemeFX.RowAllWhite] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRooms,
				legacy = true
			},
			[RDThemeFX.RowSilhouetteGlow] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRooms,
				legacy = true
			},
			[RDThemeFX.RowPlain] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRooms,
				legacy = true
			},
			[RDThemeFX.Blackout] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRooms,
				legacy = true
			},
			[RDThemeFX.MiawMiaw] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRooms,
				legacy = true
			},
			[RDThemeFX.DisableAll] = new VFXInfo
			{
				roomsUsage = RoomsUsage.ManyRoomsAndOnTop
			}
		};

		public static readonly RDTheme[] FunctionalThemes = new RDTheme[56]
		{
			RDTheme.None,
			RDTheme.Intimate,
			RDTheme.IntimateSimple,
			RDTheme.InsomniacDay,
			RDTheme.InsomniacNight,
			RDTheme.Matrix,
			RDTheme.NeonMuseum,
			RDTheme.CrossesStraight,
			RDTheme.CrossesFalling,
			RDTheme.CubesFalling,
			RDTheme.CubesFallingNiceBlue,
			RDTheme.OrientalTechno,
			RDTheme.OrientalTechnoClosed,
			RDTheme.Kaleidoscope,
			RDTheme.PoliticiansRally,
			RDTheme.Rooftop,
			RDTheme.RooftopSummer,
			RDTheme.RooftopAutumn,
			RDTheme.BackAlley,
			RDTheme.Sky,
			RDTheme.NightSky,
			RDTheme.HallOfMirrors,
			RDTheme.CoffeeShop,
			RDTheme.CoffeeShopNight,
			RDTheme.Garden,
			RDTheme.GardenNight,
			RDTheme.TrainDay,
			RDTheme.TrainNight,
			RDTheme.DesertDay,
			RDTheme.DesertNight,
			RDTheme.HospitalWard,
			RDTheme.HospitalWardNight,
			RDTheme.PaigeOffice,
			RDTheme.Basement,
			RDTheme.ColeWardNight,
			RDTheme.ColeWardSunrise,
			RDTheme.BoyWard,
			RDTheme.GirlWard,
			RDTheme.Skyline,
			RDTheme.SkylineBlue,
			RDTheme.FloatingHeart,
			RDTheme.FloatingHeartWithCubes,
			RDTheme.FloatingHeartBroken,
			RDTheme.FloatingHeartBrokenWithCubes,
			RDTheme.ZenGarden,
			RDTheme.Space,
			RDTheme.Vaporwave,
			RDTheme.RollerDisco,
			RDTheme.Stadium,
			RDTheme.StadiumStormy,
			RDTheme.AthleteWard,
			RDTheme.AthleteWardNight,
			RDTheme.ProceduralTree,
			RDTheme.RecordsRoom,
			RDTheme.Airport,
			RDTheme.AbandonedWard
		};

		public static readonly RDTheme[] LegacyThemes = new RDTheme[19]
		{
			RDTheme.IntimateSimple,
			RDTheme.InsomniacNight,
			RDTheme.CrossesFalling,
			RDTheme.CubesFallingNiceBlue,
			RDTheme.RooftopSummer,
			RDTheme.RooftopAutumn,
			RDTheme.NightSky,
			RDTheme.CoffeeShopNight,
			RDTheme.GardenNight,
			RDTheme.TrainNight,
			RDTheme.DesertNight,
			RDTheme.HospitalWardNight,
			RDTheme.ColeWardSunrise,
			RDTheme.SkylineBlue,
			RDTheme.FloatingHeartWithCubes,
			RDTheme.FloatingHeartBrokenWithCubes,
			RDTheme.StadiumStormy,
			RDTheme.AthleteWardNight,
			RDTheme.OrientalTechnoClosed
		};

		public static readonly Dictionary<RDTheme, int> ThemeVariants = new Dictionary<RDTheme, int>
		{
			{
				RDTheme.Intimate,
				2
			},
			{
				RDTheme.OrientalTechno,
				2
			},
			{
				RDTheme.InsomniacDay,
				2
			},
			{
				RDTheme.CrossesStraight,
				2
			},
			{
				RDTheme.CubesFalling,
				2
			},
			{
				RDTheme.Rooftop,
				3
			},
			{
				RDTheme.Sky,
				2
			},
			{
				RDTheme.CoffeeShop,
				2
			},
			{
				RDTheme.Garden,
				2
			},
			{
				RDTheme.TrainDay,
				2
			},
			{
				RDTheme.DesertDay,
				2
			},
			{
				RDTheme.HospitalWard,
				2
			},
			{
				RDTheme.ColeWardNight,
				2
			},
			{
				RDTheme.Skyline,
				2
			},
			{
				RDTheme.FloatingHeart,
				2
			},
			{
				RDTheme.FloatingHeartBroken,
				2
			},
			{
				RDTheme.Stadium,
				2
			},
			{
				RDTheme.AthleteWard,
				2
			}
		};

		public static readonly Dictionary<RDTheme, Vector2> EditorThemeBounds = new Dictionary<RDTheme, Vector2>
		{
			[RDTheme.TrainDay] = new Vector2(-528f, 176f),
			[RDTheme.TrainNight] = new Vector2(-528f, 176f),
			[RDTheme.DesertDay] = new Vector2(-528f, 176f),
			[RDTheme.DesertNight] = new Vector2(-528f, 176f),
			[RDTheme.HospitalWard] = new Vector2(-490f, 1172f),
			[RDTheme.PaigeOffice] = new Vector2(-358f, 42f),
			[RDTheme.Basement] = new Vector2(-32f, 320f),
			[RDTheme.RollerDisco] = new Vector2(-1000f, 1000f),
			[RDTheme.AthleteWard] = new Vector2(0f, 732f),
			[RDTheme.AthleteWardNight] = new Vector2(0f, 732f),
			[RDTheme.Airport] = new Vector2(0f, 248f),
			[RDTheme.RecordsRoom] = new Vector2(-475f, 0f),
			[RDTheme.AbandonedWard] = new Vector2(-42f, 706f)
		};

		public static readonly Character[] AvailableCharacters = new Character[48]
		{
			Character.Samurai,
			Character.Boy,
			Character.Girl,
			Character.SamuraiBoss,
			Character.Farmer,
			Character.Paige,
			Character.HoodieBoy,
			Character.SmokinBarista,
			Character.Politician,
			Character.MrStevenson,
			Character.MrsStevenson,
			Character.Miner,
			Character.Athlete,
			Character.Mark,
			Character.Cockatiel,
			Character.Parrot,
			Character.Owl,
			Character.Oriole,
			Character.Lucia,
			Character.Bodybuilder,
			Character.Barista,
			Character.SamuraiBlue,
			Character.SamuraiYellow,
			Character.SamuraiGreen,
			Character.SamuraiBaseball,
			Character.SamuraiPirate,
			Character.BoyTangzhuang,
			Character.BoyRaya,
			Character.GirlCNY,
			Character.SamuraiBossAlt,
			Character.HoodieBoyBlue,
			Character.MrStevensonTango,
			Character.MrsStevensonTango,
			Character.AthleteAlt,
			Character.Beat,
			Character.Quaver,
			Character.Clef,
			Character.Treble,
			Character.Rin,
			Character.Buro,
			Character.Marija,
			Character.Wren,
			Character.Canary,
			Character.Saturday,
			Character.Allison,
			Character.Sophia,
			Character.Lune,
			Character.Cranky
		};

		public static readonly Character[] AvailableRandomCharacters = new Character[19]
		{
			Character.Samurai,
			Character.Boy,
			Character.Girl,
			Character.SamuraiBoss,
			Character.HoodieBoy,
			Character.Paige,
			Character.Politician,
			Character.Miner,
			Character.Farmer,
			Character.MrStevenson,
			Character.MrsStevenson,
			Character.SmokinBarista,
			Character.Cockatiel,
			Character.Parrot,
			Character.Owl,
			Character.Oriole,
			Character.Lucia,
			Character.Athlete,
			Character.Mark
		};

		public static readonly Dictionary<Character, Character[]> AvailableDialogueCharacters = new Dictionary<Character, Character[]>
		{
			[Character.Paige] = new Character[2]
			{
				Character.Ian,
				Character.Edega
			},
			[Character.Barista] = new Character[4]
			{
				Character.Janitor,
				Character.RhythmSecurity,
				Character.RhythmNurse,
				Character.RhythmJanitor
			}
		};

		public static readonly Dictionary<string, Character> DialogueCharacterAliases = new Dictionary<string, Character>
		{
			["Logan"] = Character.Boy,
			["LoganCNY"] = Character.BoyTangzhuang,
			["LoganRaya"] = Character.BoyRaya,
			["Hailey"] = Character.Girl,
			["HaileyCNY"] = Character.GirlCNY,
			["Insomniac"] = Character.SamuraiBoss,
			["InsomniacAlt"] = Character.SamuraiBossAlt,
			["Wrestler"] = Character.Bodybuilder,
			["Richard"] = Character.Politician,
			["Cole"] = Character.HoodieBoy,
			["ColeBlue"] = Character.HoodieBoyBlue,
			["Nicole"] = Character.SmokinBarista,
			["Lucky"] = Character.Athlete,
			["LuckyAlt"] = Character.AthleteAlt
		};

		public static readonly Character[] AvailableSpriteCharacters = new Character[14]
		{
			Character.DancingCouple,
			Character.NicoleMints,
			Character.NicoleCigs,
			Character.NicoleCoffee,
			Character.ColeGuitar,
			Character.ColeSynth,
			Character.Controller,
			Character.LuckyBaseball,
			Character.LuckyJersey,
			Character.LuckyIce,
			Character.LuckyBag,
			Character.BookEdega,
			Character.BookEye,
			Character.BookDiary
		};

		public static readonly Character[] AvailableCPUCharacters = new Character[5]
		{
			Character.Otto,
			Character.Ian,
			Character.Paige,
			Character.Edega,
			Character.BlankCPU
		};

		public static readonly Character[] UnsafeEditorCharacters = new Character[6]
		{
			Character.Otto,
			Character.Custom,
			Character.Player,
			Character.BlankCPU,
			Character.New,
			Character.Weightlifter
		};

		public static readonly SoundEffect[] BeatSounds = new SoundEffect[34]
		{
			SoundEffect.Shaker,
			SoundEffect.None,
			SoundEffect.ShakerHi,
			SoundEffect.Stick,
			SoundEffect.StickOld,
			SoundEffect.Sidestick,
			SoundEffect.Punch,
			SoundEffect.Ride2,
			SoundEffect.Kick,
			SoundEffect.KickChroma,
			SoundEffect.KickClean,
			SoundEffect.KickTight,
			SoundEffect.KickHouse,
			SoundEffect.KickRupture,
			SoundEffect.KickEcho,
			SoundEffect.Hammer,
			SoundEffect.Jumpsnare,
			SoundEffect.Chuck,
			SoundEffect.ClosedHat,
			SoundEffect.HatTight,
			SoundEffect.HatHouse,
			SoundEffect.Sizzle,
			SoundEffect.ClavesLow,
			SoundEffect.ClavesHigh,
			SoundEffect.TomLowE,
			SoundEffect.TomMidG,
			SoundEffect.TomMidB,
			SoundEffect.TomHighD,
			SoundEffect.WoodblockHigh,
			SoundEffect.WoodblockLow,
			SoundEffect.TriangleMute,
			SoundEffect.Cowbell,
			SoundEffect.TimbaleHigh,
			SoundEffect.TimbaleLow
		};

		public static readonly SoundEffect[] PlaySounds = new SoundEffect[32]
		{
			SoundEffect.MistakeSmall3,
			SoundEffect.MistakeBig,
			SoundEffect.InsomniacVirusCue,
			SoundEffect.Switch,
			SoundEffect.Vibraslap,
			SoundEffect.Squareshot,
			SoundEffect.Triangleshot1,
			SoundEffect.Triangleshot2,
			SoundEffect.Triangleshot3,
			SoundEffect.Triangleshot4,
			SoundEffect.FreezeshotTick,
			SoundEffect.FreezeshotCymbal,
			SoundEffect.BurnshotCymbal,
			SoundEffect.HoldshotCue,
			SoundEffect.HoldshotClapShortEnd,
			SoundEffect.HoldshotClapLongEnd,
			SoundEffect.OttoActivate,
			SoundEffect.OttoDeactivate,
			SoundEffect.MetronomeClickLow,
			SoundEffect.MetronomeClickMid,
			SoundEffect.MetronomeClickHigh,
			SoundEffect.SyncoModifierOn,
			SoundEffect.SyncoModifierOff,
			SoundEffect.SyncoFirstPulse,
			SoundEffect.SyncoPull,
			SoundEffect.SyncoRelease,
			SoundEffect.SyncopationBeepLow,
			SoundEffect.SyncopationBeepMid,
			SoundEffect.SyncopationBeepHigh,
			SoundEffect.RefereeLow,
			SoundEffect.RefereeShort,
			SoundEffect.RefereeHigh
		};

		public static readonly SoundEffect[] ClapSoundsP1 = new SoundEffect[7]
		{
			SoundEffect.ClapHit,
			SoundEffect.ReverbClap,
			SoundEffect.ClapHitEcho,
			SoundEffect.ClapHitMassivePreEcho,
			SoundEffect.SnareAcoustic2,
			SoundEffect.SnareHouse,
			SoundEffect.SnareVapor
		};

		public static readonly SoundEffect[] ClapSoundsP2 = new SoundEffect[7]
		{
			SoundEffect.ClapHitP2,
			SoundEffect.ReverbClapP2,
			SoundEffect.ClapHitEchoP2,
			SoundEffect.ClapHitMassivePreEchoP2,
			SoundEffect.SnareAcoustic4,
			SoundEffect.SnareHouseP2,
			SoundEffect.SnareVaporP2
		};

		public static readonly SoundEffect[] ClapSoundsCPU = new SoundEffect[1] { SoundEffect.ClapHitCPU };

		public static readonly CustomSoundType[] CustomSoundTypes = new CustomSoundType[5]
		{
			CustomSoundType.CueSound,
			CustomSoundType.MusicSound,
			CustomSoundType.BeatSound,
			CustomSoundType.HitSound,
			CustomSoundType.OtherSound
		};

		public static Ease[] availableEases = new Ease[31]
		{
			Ease.Linear,
			Ease.InSine,
			Ease.OutSine,
			Ease.InOutSine,
			Ease.InQuad,
			Ease.OutQuad,
			Ease.InOutQuad,
			Ease.InCubic,
			Ease.OutCubic,
			Ease.InOutCubic,
			Ease.InQuart,
			Ease.OutQuart,
			Ease.InOutQuart,
			Ease.InQuint,
			Ease.OutQuint,
			Ease.InOutQuint,
			Ease.InExpo,
			Ease.OutExpo,
			Ease.InOutExpo,
			Ease.InCirc,
			Ease.OutCirc,
			Ease.InOutCirc,
			Ease.InElastic,
			Ease.OutElastic,
			Ease.InOutElastic,
			Ease.InBack,
			Ease.OutBack,
			Ease.InOutBack,
			Ease.InBounce,
			Ease.OutBounce,
			Ease.InOutBounce
		};

		public static Ease[] availableEasesForShowRooms = new Ease[32]
		{
			Ease.Linear,
			Ease.InSine,
			Ease.OutSine,
			Ease.InOutSine,
			Ease.InQuad,
			Ease.OutQuad,
			Ease.InOutQuad,
			Ease.InCubic,
			Ease.OutCubic,
			Ease.InOutCubic,
			Ease.InQuart,
			Ease.OutQuart,
			Ease.InOutQuart,
			Ease.InQuint,
			Ease.OutQuint,
			Ease.InOutQuint,
			Ease.InExpo,
			Ease.OutExpo,
			Ease.InOutExpo,
			Ease.InCirc,
			Ease.OutCirc,
			Ease.InOutCirc,
			Ease.InElastic,
			Ease.OutElastic,
			Ease.InOutElastic,
			Ease.InBack,
			Ease.OutBack,
			Ease.InOutBack,
			Ease.InBounce,
			Ease.OutBounce,
			Ease.InOutBounce,
			Ease.Unset
		};

		public static readonly LevelEventType[] levelEventsThatUpdateTimeline = new LevelEventType[4]
		{
			LevelEventType.PlaySong,
			LevelEventType.SetCrotchetsPerBar,
			LevelEventType.SetBeatsPerMinute,
			LevelEventType.SetRowXs
		};

		public static readonly NarrationCategory[] availableNarrationCategories = new NarrationCategory[4]
		{
			NarrationCategory.Notification,
			NarrationCategory.Description,
			NarrationCategory.Subtitles,
			NarrationCategory.Instruction
		};

		public static readonly string[] defaultColorPalette = new string[21]
		{
			"000000ff", "ffffffff", "7f7f7fff", "c3c3c3ff", "880015ff", "b97a57ff", "ff0000ff", "ffaec9ff", "ff7f27ff", "ffc90eff",
			"fff200ff", "efe4b0ff", "22b14cff", "b5e61dff", "00a2e8ff", "99d9eaff", "3f48ccff", "7092beff", "a349a4ff", "c8bfe7ff",
			"00000000"
		};

		public static readonly Character[] availableCPUCharacters = new Character[5]
		{
			Character.Otto,
			Character.Ian,
			Character.Paige,
			Character.Edega,
			Character.BlankCPU
		};

		public static readonly Color[] windowColors = new Color[4]
		{
			"FFBB00".HexToColor(),
			"FF66BB".HexToColor(),
			"33dd99".HexToColor(),
			"8455e0".HexToColor()
		};

		public static Dictionary<EventCategory, LevelEventType[]> eventCategories = new Dictionary<EventCategory, LevelEventType[]>
		{
			{
				EventCategory.Gameplay,
				new LevelEventType[11]
				{
					LevelEventType.HideRow,
					LevelEventType.ChangePlayersRows,
					LevelEventType.FinishLevel,
					LevelEventType.GoToLevel,
					LevelEventType.ShowHands,
					LevelEventType.SetHandOwner,
					LevelEventType.SetHeartExplodeVolume,
					LevelEventType.SetHeartExplodeInterval,
					LevelEventType.SayReadyGetSetGo,
					LevelEventType.SetCountingSound,
					LevelEventType.SetPlayStyle
				}
			},
			{
				EventCategory.RowFX,
				new LevelEventType[7]
				{
					LevelEventType.HideRow,
					LevelEventType.MoveRow,
					LevelEventType.ReorderRow,
					LevelEventType.PlayExpression,
					LevelEventType.ChangeCharacter,
					LevelEventType.TintRows,
					LevelEventType.SpinningRows
				}
			},
			{
				EventCategory.CamFX,
				new LevelEventType[5]
				{
					LevelEventType.MoveCamera,
					LevelEventType.ShakeScreenCustom,
					LevelEventType.ShakeScreen,
					LevelEventType.FlipScreen,
					LevelEventType.PulseCamera
				}
			},
			{
				EventCategory.VisualFX,
				new LevelEventType[10]
				{
					LevelEventType.SetVFXPreset,
					LevelEventType.SetSpeed,
					LevelEventType.Flash,
					LevelEventType.CustomFlash,
					LevelEventType.PaintHands,
					LevelEventType.BassDrop,
					LevelEventType.InvertColors,
					LevelEventType.Stutter,
					LevelEventType.NewWindowDance,
					LevelEventType.WindowResize
				}
			},
			{
				EventCategory.Environment,
				new LevelEventType[6]
				{
					LevelEventType.SetTheme,
					LevelEventType.SetBackgroundColor,
					LevelEventType.SetForeground,
					LevelEventType.SetSpeed,
					LevelEventType.Flash,
					LevelEventType.CustomFlash
				}
			},
			{
				EventCategory.Text,
				new LevelEventType[4]
				{
					LevelEventType.TextExplosion,
					LevelEventType.ShowDialogue,
					LevelEventType.ShowStatusSign,
					LevelEventType.FloatingText
				}
			},
			{
				EventCategory.Utility,
				new LevelEventType[3]
				{
					LevelEventType.Comment,
					LevelEventType.CallCustomMethod,
					LevelEventType.TagAction
				}
			},
			{
				EventCategory.Song,
				new LevelEventType[3]
				{
					LevelEventType.PlaySong,
					LevelEventType.SetCrotchetsPerBar,
					LevelEventType.SetBeatsPerMinute
				}
			},
			{
				EventCategory.Sounds,
				new LevelEventType[6]
				{
					LevelEventType.PlaySound,
					LevelEventType.SetClapSounds,
					LevelEventType.SetGameSound,
					LevelEventType.SetBeatSound,
					LevelEventType.ReadNarration,
					LevelEventType.NarrateRowInfo
				}
			},
			{
				EventCategory.MakeDecoration,
				new LevelEventType[2]
				{
					LevelEventType.Sprite,
					LevelEventType.Text
				}
			},
			{
				EventCategory.SpriteDecoration,
				new LevelEventType[8]
				{
					LevelEventType.Tile,
					LevelEventType.Tint,
					LevelEventType.SetVisible,
					LevelEventType.ReorderDecoration,
					LevelEventType.PlayAnimation,
					LevelEventType.Move,
					LevelEventType.Blend,
					LevelEventType.Comment
				}
			},
			{
				EventCategory.TextDecoration,
				new LevelEventType[8]
				{
					LevelEventType.SetText,
					LevelEventType.AdvanceTextDecoration,
					LevelEventType.SetFont,
					LevelEventType.Move,
					LevelEventType.TintText,
					LevelEventType.SetVisible,
					LevelEventType.ReorderDecoration,
					LevelEventType.Comment
				}
			},
			{
				EventCategory.None,
				new LevelEventType[13]
				{
					LevelEventType.AddClassicBeat,
					LevelEventType.AddFreeTimeBeat,
					LevelEventType.AddOneshotBeat,
					LevelEventType.PulseFreeTimeBeat,
					LevelEventType.SetRowXs,
					LevelEventType.SetOneshotWave,
					LevelEventType.FadeRoom,
					LevelEventType.ShowRooms,
					LevelEventType.SetRoomContentMode,
					LevelEventType.SetRoomPerspective,
					LevelEventType.ReorderRooms,
					LevelEventType.MoveRoom,
					LevelEventType.MaskRoom
				}
			}
		};
	}
}
