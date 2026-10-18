using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using RDLevelEditor;
using UnityEngine;

public static class GC
{
	public static bool levelEditor_streamMP3Loading = false;

	public static bool customLevelSelect_streamMP3Loading = true;

	public static bool twoPlayerMode;

	public static bool gameHasFocus = true;

	public static string lastWrittenInkFilePath = "";

	public static string steamBranchName;

	public static readonly string[] betaBranches = new string[3] { "beta", "alpha", "accessibility" };

	public static bool showAbsoluteOffsets = false;

	public static bool d_auto7th = true;

	public static bool beatSoundsEnabled = true;

	public static bool d_showMarginsNumerically = false;

	public static bool d_separateP2Calibration = true;

	public static bool allowDoubleSpeed = true;

	public static float VolBeatsounds = 1f;

	public static float VolSongs = 1f;

	public static float VolMenuSounds = 1f;

	public const bool oneRatio = true;

	public const int DefaultVerticalRowSpacing = 42;

	public const int RowPositionX = 35;

	public const int PlayerboxCenterX = 0;

	public const int SubdivisionPlayerboxCenterX = -39;

	public const int PlayerboxWidth = 24;

	public static readonly int StripCenterX = 35;

	public const int MaxBeatloopIDCount = 200;

	public const int MaxRowCount = 16;

	public const int RoomsCount = 4;

	public const int DefaultRoomRenderTextureScale = 4;

	public const int PlayerCount = 3;

	public static readonly RDPlayer[] MultiplayerRowOrder = new RDPlayer[3]
	{
		RDPlayer.P1,
		RDPlayer.P2,
		RDPlayer.CPU
	};

	public const string NoKillOnReloadGeneral = "noKillOnReloadGeneral";

	public const float GAME_MARGIN_COUNTED = 0.4f;

	public const float GAME_MARGIN_BARELY = 0.2f;

	public const float GAME_MARGIN_UNMISSABLE = 0.4f;

	public const float GAME_MARGIN_VERYEASY = 0.2f;

	public const float GAME_MARGIN_EASY = 0.12f;

	public const float GAME_MARGIN_HIT = 0.08f;

	public const float GAME_MARGIN_HARD = 0.04f;

	public const float GAME_MARGIN_COMPO = 0.025f;

	public const float GAME_MARGIN_FREETIMEBEAT_MULTIPLIER = 3f;

	public static int GameHUDLayer = 11;

	public static int EffectsLayer = 12;

	public static int InfiniteZoomLayer = 13;

	public const string SceneGame = "scnGame";

	public const string SceneWarning = "scnWarning";

	public const string SceneDemoLevels = "scnDemoLevels";

	public const string SceneExpoLevelSelect = "scnExpoMenu";

	public const string SceneLevelSelect = "scnLevelSelect";

	public const string SceneMonthlyLevel = "scnMonthlyLevel";

	public const string SceneCustomLevelSelect = "scnCLS";

	public const string SceneAlphaLevelSelect = "scnAlphaLevelSelect";

	public const string SceneMainMenu = "scnMenu";

	public const string SceneCalibration = "scnCalibration";

	public const string SceneLevelEditor = "scnEditor";

	public const string SceneSignup = "scnSignup";

	public const string SceneSRT = "scnRDSRT";

	public const string SceneLevelURL = "scnLevelURL";

	public const string SceneSleevePaint = "scnSleevePaint";

	public const string SceneLogo = "scnLogo";

	public const string SceneSplash = "scnSplash";

	public const string SceneRhythmStacker = "scnRhythmStacker";

	public const string SceneIanDesktop = "scnIanDesktop";

	public const string SceneRhythmWeightlifter = "scnRhythmWeightlifter";

	public const string NeutralExpression = "neutral";

	public const string HappyExpression = "happy";

	public const string BarelyExpression = "barely";

	public const string MissedExpression = "missed";

	public const string BeepExpression = "beep";

	public const string PrehitExpression = "prehit";

	public const string ShakerSound = "sndShaker";

	public const string HammerSound = "sndHammer";

	public const string KickSound = "sndKick";

	public const string KickChromaSound = "sndKickChroma";

	public const string KickCleanSound = "sndKickClean";

	public const string KickTightSound = "sndKickTight";

	public const string PunchSound = "sndPunch";

	public const string ChuckSound = "sndChuck";

	public const string ClavesHighSound = "sndClavesHigh";

	public const string ClavesLowSound = "sndClavesLow";

	public const string WoodblockHighSound = "sndWoodblockHigh";

	public const string WoodblockLowSound = "sndWoodblockLow";

	public const string StickSound = "sndStick";

	public const string SizzleSound = "sndSizzle";

	public const string ClosedHatSound = "sndClosedHat";

	public const string HatTightSound = "sndHatTight";

	public const string TomMidGSound = "sndTomMidG";

	public const string TomMidBSound = "sndTomMidB";

	public const string RideSound = "sndRide2";

	public const string SidestickSound = "sndSidestick";

	public const string SwitchSound = "sndSwitch";

	public const string SpotlightOffSound = "sndSpotlightOff";

	public const string SpotlightSweepSound = "sndSpotlightSweep";

	public const string EnterLevelSound = "sndMenuSelect";

	public const string EnterBossLevelSound = "sndMenuSelectBoss";

	public const string PressToContinueSound = "sndPressToContinue";

	public const string SyncoModifierOnSound = "sndSyncoModifierOn";

	public const string SyncoModifierOffSound = "sndSyncoModifierOff";

	public const string SyncoPullSound = "sndSyncoPull";

	public const string SyncoReleaseSound = "sndSyncoRelease";

	public const string SyncoFirstPulseSound = "sndSyncoFirstPulse";

	public const string SyncopationBeepLowSound = "sndSyncopationBeepLow";

	public const string SyncopationBeepMidSound = "sndSyncopationBeepMid";

	public const string SyncopationBeepHighSound = "sndSyncopationBeepHigh";

	public const string PopSound = "sndMenuPop";

	public const string PopSound2 = "sndMenuPop2";

	public const string PopSoundLarge = "sndMenuPopLarge";

	public const string PopSoundLarge2 = "sndMenuPopLarge2";

	public const string PopSoundSpring = "sndMenuPopSpring";

	public const string PopSoundSpring2 = "sndMenuPopSpring2";

	public const string PopSoundGold = "sndMenuPopGold";

	public const string PopSoundGold2 = "sndMenuPopGold2";

	public const string CursorSound = "sndPagerCursorMove";

	public const string BorderSound = "sndPagerCursorBorder";

	public const string ButtonSound = "sndPagerButton";

	public const string ValueSound = "sndPagerValue";

	public const string RankSound = "sndJyi - Rank";

	public const string PatientConnectSound = "sndPatientConnect";

	public const string PatientUpdateSound = "sndPatientUpdate";

	public const string PatientDisconnectSound = "sndPatientDisconnect";

	public const string DoctorOnlineSound = "sndOttoActivate";

	public const string DoctorOfflineSound = "sndOttoDeactivate";

	public const string JyiCount = "sndJyi - ChineseCount";

	public const string JyiCountLegacy = "sndJyi - ChineseCountLegacy";

	public const string JyiCountFast = "sndJyi - ChineseCountFast";

	public const string JyiCountCalm = "sndJyi - ChineseCountCalm";

	public const string JyiCountTired = "sndJyi - ChineseCountTired";

	public const string JyiCountVeryTired = "sndJyi - ChineseCountVTired";

	public const string JyiCountJapanese = "sndJyi - JapaneseCount";

	public const string IanCount = "sndIan - ChineseCount";

	public const string IanCountFast = "sndIan - ChineseCountFast";

	public const string IanCountCalm = "sndIan - ChineseCountCalm";

	public const string IanCountSlow = "sndIan - ChineseCountSlow";

	public const string IanCountSlower = "sndIan - CountSlower";

	public const string WhistleCount = "sndWhistle";

	public const string BirdCount = "sndBird";

	public const string ParrotCount = "sndParrot";

	public const string OwlCount = "sndOwl";

	public const string OrioleCount = "sndOriole";

	public const string WrenCount = "sndWren";

	public const string CanaryCount = "sndCanary";

	public const string SpearCount = "sndSpear";

	public const string JyiCountEnglish = "sndJyi - CountEnglish";

	public const string IanCountEnglish = "sndIan - CountEnglish";

	public const string IanCountEnglishCalm = "sndIanCalm - CountEnglish";

	public const string IanCountEnglishSlow = "sndIanSlow - CountEnglish";

	public const float PAN_FULL_LEFT = -1f;

	public const float PAN_LEFT = -0.5f;

	public const float PAN_CENTER = 0f;

	public const float PAN_RIGHT = 0.5f;

	public const float PAN_FULL_RIGHT = 1f;

	public static float TWO_PLAYER_PANNING_STRENGTH = 0.9f;

	public static float PanP1 = TWO_PLAYER_PANNING_STRENGTH;

	public static float PanP2 = TWO_PLAYER_PANNING_STRENGTH * -1f;

	public const string LevelPrefix = "Level_";

	public const string InternalLevelPrefix = "*internal*/";

	public const string LevelFileExtension = "rdlevel";

	public const string JSONFileExtension = "json";

	public static readonly string[] SupportedJSONFiles = new string[1] { "json" };

	public const string RDZipFileExtension = "rdzip";

	public const string ZipFileExtension = "zip";

	public static readonly string[] SupportedAudioFiles = new string[5] { "mp3", "wav", "ogg", "aiff", "aif" };

	public static readonly ImmutableHashSet<string> SupportedAudioFilesSet = SupportedAudioFiles.ToImmutableHashSet();

	public const string ExternalSoundSuffix = "*external";

	public const string OggVorbisFileExtension = "ogg";

	public const string MP3FileExtension = "mp3";

	public const string WavFileExtension = "wav";

	public const string AiffFileExtension = "aiff";

	public const string AifFileExtension = "aif";

	public static readonly string[] SupportedImageFiles = new string[4] { "jpeg", "jpg", "png", "gif" };

	public static readonly ImmutableHashSet<string> SupportedImageFilesSet = SupportedImageFiles.ToImmutableHashSet();

	public const string JPEGFileExtension = "jpeg";

	public const string JPGFileExtension = "jpg";

	public const string PNGFileExtension = "png";

	public const string GIFFileExtension = "gif";

	public static readonly string[] SupportedSpriteFiles = new string[5] { "jpeg", "jpg", "png", "gif", "json" };

	public static readonly string[] SupportedFontFiles = new string[2] { "ttf", "otf" };

	public static readonly ImmutableHashSet<string> SupportedFontFilesSet = SupportedFontFiles.ToImmutableHashSet();

	public const string TTFFileExtension = "ttf";

	public const string OTFFileExtension = "otf";

	public const string InternalSongsFolder = "Music/";

	public const string InternalSoundEffectsFolder = "Sfx/";

	public const string InternalLevelsFolder = "InternalLevels";

	public const string CutscenesFolder = "Cutscenes";

	public const float IceLevelSpeed = 0.75f;

	public const float NormalLevelSpeed = 1f;

	public const float ChiliLevelSpeed = 1.5f;

	public const float HoldCueThreshold = 0.666f;

	public static string VersionText = "Preorder";

	public static string CurrentReleaseMonth = "2019";

	public static string NextReleaseDate = "None";

	public static Level MonthlyLevelToGo = Level.InsomniacHard;

	public static readonly Rect GUIRect = new Rect(0f, 0f, 400f, 800f);

	public static string[] levelsToRecord = new string[63]
	{
		"OrientalTechno", "OrientalDubstep", "Intimate", "IntimateH", "OrientalInsomniac", "InsomniacHard", "Lofi", "CareLess", "SVT", "Unreachable",
		"Smokin", "Pomeranian", "SongOfTheSea", "SongOfTheSeaH", "Boss2", "Garden", "Lounge", "Classy", "ClassyH", "DistantDuet",
		"DistantDuetH", "GAndTonic", "ArtExercise", "Lesmis", "Heldbeats", "Rollerdisco", "Invisible", "InvisibleH", "Steinway", "SteinwayH",
		"KnowYou", "Murmurs", "LuckyBreak", "Injury", "Freezeshot", "FreezeshotH", "AthleteTherapy", "StevensonsTango", "AthleteFinale", "HaileyDuet",
		"EdegaRave", "PaigesReckoning", "Blurred", "Bitterness", "Montage", "Montage2", "FourChannel", "fGirlfriend", "GongXi", "Halloween",
		"HelpingHands", "Unbeatable", "MeetAndTweet", "VividStasis", "SparkLine", "Beatblock", "BlackestLuxuryCar", "TapeStopNight", "The90sDecision", "Cutscenes/act3_cutsceneA",
		"Cutscenes/act4_cutsceneA", "Cutscenes/Intimate_passed", "Cutscenes/Cutscene_Airport"
	};

	public static int levelRecordedIndex = 0;

	public static Dictionary<Level, LevelMetadata> levelMetadata;

	public static int levelCount;

	public static ImmutableDictionary<string, ConditionalInfo> conditionalsInfo;

	public static ImmutableDictionary<LevelEventType, LevelEventInfo> levelEventsInfo;

	private const string defaultExpression = "neutral";

	private static readonly CharacterExpressionGroup neutralOnlyGroup = new CharacterExpressionGroup("neutral", "neutral", "neutral", "neutral");

	public static Dictionary<Character, CharacterExpressionGroup> characterExpressions = new Dictionary<Character, CharacterExpressionGroup>
	{
		[Character.Samurai] = new CharacterExpressionGroup("barely", "barely", "neutral", "happy"),
		[Character.Ian] = new CharacterExpressionGroup("sighing", "sighing", "explaining", "explaining"),
		[Character.SamuraiBoss] = new CharacterExpressionGroup("headspin", "headspin", "neutral", "happy"),
		[Character.SamuraiBossAlt] = new CharacterExpressionGroup("headspin", "headspin", "neutral", "happy"),
		[Character.SmokinBarista] = new CharacterExpressionGroup("smokeLoop", "smokeLoop", "neutral", "happy"),
		[Character.LuckyJersey] = new CharacterExpressionGroup("neutral", "neutral", "coach", "coach"),
		[Character.Saturday] = new CharacterExpressionGroup("neutral", "neutral", "happyLoop", "happyLoop"),
		[Character.MrStevensonTango] = new CharacterExpressionGroup("neutral", "neutral", "pair_neutral", "pair_happy"),
		[Character.MrsStevensonTango] = new CharacterExpressionGroup("neutral", "neutral", null, null),
		[Character.Paige] = neutralOnlyGroup,
		[Character.ColeGuitar] = neutralOnlyGroup,
		[Character.ColeSynth] = neutralOnlyGroup,
		[Character.NicoleCigs] = neutralOnlyGroup,
		[Character.NicoleCoffee] = neutralOnlyGroup,
		[Character.NicoleMints] = neutralOnlyGroup,
		[Character.Controller] = neutralOnlyGroup,
		[Character.IanBubble] = neutralOnlyGroup,
		[Character.LuckyBaseball] = neutralOnlyGroup,
		[Character.LuckyIce] = neutralOnlyGroup,
		[Character.LuckyBag] = neutralOnlyGroup
	};

	public static Dictionary<Character, string> defaultCharacterPulseSounds = new Dictionary<Character, string>
	{
		[Character.Farmer] = "sndPunch",
		[Character.FarmerAlternate] = "sndPunch",
		[Character.Samurai] = "sndKick",
		[Character.SamuraiGreen] = "sndKick",
		[Character.SamuraiBlue] = "sndKick",
		[Character.SamuraiYellow] = "sndKick",
		[Character.SamuraiGirl] = "sndKick",
		[Character.SamuraiBaseball] = "sndKick",
		[Character.SamuraiPirate] = "sndKick",
		[Character.Girl] = "sndSizzle",
		[Character.GirlCNY] = "sndSizzle",
		[Character.HoodieBoy] = "sndKickChroma",
		[Character.HoodieBoyAlternate] = "sndKickChroma",
		[Character.HoodieBoyBlue] = "sndKickChroma",
		[Character.SmokinBarista] = "sndKick",
		[Character.Athlete] = "sndKickClean",
		[Character.AthleteAlt] = "sndKickClean",
		[Character.AthletePhysio] = "sndKickClean",
		[Character.Mark] = "sndKickTight",
		[Character.MrStevenson] = "sndHatTight",
		[Character.MrStevensonTango] = "sndHatTight",
		[Character.MrStevendog] = "sndHatTight",
		[Character.MrsStevenson] = "sndClosedHat",
		[Character.MrsStevensonTango] = "sndClosedHat",
		[Character.MrsStevendog] = "sndClosedHat",
		[Character.Paige] = "sndStick",
		[Character.Ian] = "sndPunch",
		[Character.Edega] = "sndSidestick",
		[Character.Barista] = "sndChuck",
		[Character.Politician] = "sndHammer",
		[Character.Bodybuilder] = "sndHammer",
		[Character.Adog] = "sndStick",
		[Character.Purritician] = "sndHammer",
		[Character.RhythmSecurity] = "sndSidestick",
		[Character.RhythmNurse] = "sndStick",
		[Character.RhythmJanitor] = "sndChuck",
		[Character.RhythmJanitorReal] = "sndChuck",
		[Character.Cockatiel] = "sndClavesLow",
		[Character.Parrot] = "sndClavesHigh",
		[Character.Owl] = "sndWoodblockLow",
		[Character.Oriole] = "sndWoodblockHigh",
		[Character.Wren] = "sndWoodblockLow",
		[Character.Canary] = "sndWoodblockHigh",
		[Character.Quaver] = "sndKick",
		[Character.Beat] = "sndChuck",
		[Character.Clef] = "sndKickClean",
		[Character.Treble] = "sndPunch",
		[Character.Buro] = "sndClosedHat",
		[Character.Rin] = "sndKickChroma",
		[Character.Marija] = "sndChuck",
		[Character.Saturday] = "sndShaker",
		[Character.Allison] = "sndSizzle",
		[Character.Sophia] = "sndRide2",
		[Character.Lune] = "sndClavesHigh",
		[Character.Cranky] = "sndSidestick"
	};

	public static Dictionary<Character, string> defaultCharacterSpeechSounds = new Dictionary<Character, string>
	{
		[Character.Paige] = "Paige",
		[Character.Ian] = "Ian",
		[Character.IanBubble] = "IanMuffled",
		[Character.Edega] = "Edega",
		[Character.Janitor] = "Janitor",
		[Character.Farmer] = "Farmer",
		[Character.FarmerAlternate] = "Farmer",
		[Character.Samurai] = "Samurai",
		[Character.SamuraiGreen] = "Samurai",
		[Character.SamuraiBlue] = "Samurai",
		[Character.SamuraiYellow] = "Samurai",
		[Character.SamuraiGirl] = "Samurai",
		[Character.SamuraiPirate] = "Samurai",
		[Character.Boy] = "Logan",
		[Character.BoyTangzhuang] = "Logan",
		[Character.BoyRaya] = "Logan",
		[Character.GirlCNY] = "Hailey",
		[Character.Girl] = "Hailey",
		[Character.SamuraiBoss] = "Insomniac",
		[Character.SamuraiBossAlt] = "Insomniac",
		[Character.HoodieBoy] = "Cole",
		[Character.HoodieBoyAlternate] = "Cole",
		[Character.HoodieBoyBlue] = "Cole",
		[Character.ColeGuitar] = "Cole",
		[Character.ColeSynth] = "Cole",
		[Character.SmokinBarista] = "Nicole",
		[Character.NicoleCigs] = "Nicole",
		[Character.NicoleCoffee] = "Nicole",
		[Character.NicoleMints] = "Nicole",
		[Character.MrStevenson] = "MrStevenson",
		[Character.MrStevensonTango] = "MrStevenson",
		[Character.MrsStevenson] = "MrsStevenson",
		[Character.MrsStevensonTango] = "MrsStevenson",
		[Character.Politician] = "Richard",
		[Character.Miner] = "Miner",
		[Character.Athlete] = "Athlete",
		[Character.AthletePhysio] = "Athlete",
		[Character.AthleteAlt] = "Athlete",
		[Character.LuckyBaseball] = "Athlete",
		[Character.LuckyIce] = "Athlete",
		[Character.LuckyJersey] = "Athlete",
		[Character.LuckyBag] = "Athlete",
		[Character.Mark] = "Mark",
		[Character.BookEdega] = "Mark",
		[Character.BookEye] = "Mark",
		[Character.BookDiary] = "Mark",
		[Character.Lucia] = "Lucia",
		[Character.Quaver] = "Unbeatable",
		[Character.Beat] = "Unbeatable",
		[Character.Treble] = "Unbeatable",
		[Character.Clef] = "Unbeatable",
		[Character.Buro] = "Buro",
		[Character.Marija] = "Marija",
		[Character.Rin] = "Rin",
		[Character.Saturday] = "VividStasis",
		[Character.Allison] = "VividStasis",
		[Character.RhythmSecurity] = "RhythmSecurity",
		[Character.RhythmJanitor] = "RhythmJanitor",
		[Character.RhythmNurse] = "RhythmNurse",
		[Character.RhythmJanitorReal] = "RhythmJanitorReal",
		[Character.None] = "Generic"
	};

	public static bool editorBuild => Application.productName.Contains("Editor");

	public static bool onBetaBranch
	{
		get
		{
			if (SteamIntegration.initialized)
			{
				return betaBranches.Contains(steamBranchName);
			}
			return false;
		}
	}

	public static int RoomRenderTextureScale => Persistence.GetRenderTextureScale();

	public static string GetCountingSoundFilename(CountingVoiceSource voiceSource, bool useInternalFilename = false)
	{
		string text = typeof(GC).GetField(voiceSource.ToString(), BindingFlags.Static | BindingFlags.Public).GetValue(null) as string;
		if (useInternalFilename)
		{
			return text;
		}
		return text.Substring(3);
	}
}
