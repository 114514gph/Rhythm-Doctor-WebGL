using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using DG.Tweening;
using DG.Tweening.Core;
using RDLevelEditor;
using RDTools;
using UnityEngine;
using UnityEngine.Audio;
using UnityStandardAssets.ImageEffects;

public class LevelBase : RDClass
{
	public const float DefaultBPM = 100f;

	public const int DefaultCrotchetsPerBar = 8;

	public RDLevelData data;

	public bool loadingSuccessful = true;

	public List<LevelEvent_Base> levelEvents;

	public List<LevelEvent_Base>[] levelEventsPerBar;

	public Dictionary<string, AudioClip> preparedAudioClips = new Dictionary<string, AudioClip>();

	public int[] crotchetsInEachBar;

	public List<BPMChange> bpmChanges = new List<BPMChange>();

	public List<LevelEvent_SetCrotchetsPerBar> cpbEvents = new List<LevelEvent_SetCrotchetsPerBar>();

	public List<LevelEvent_Base> dynamicallyAddedEvents = new List<LevelEvent_Base>();

	public Dictionary<string, List<LevelEvent_Base>> taggedEvents = new Dictionary<string, List<LevelEvent_Base>>();

	private bool songPlaysAtFirstBar;

	private float firstBPMFound;

	[NonSerialized]
	public int floatingTextsCreatedCount;

	[NonSerialized]
	public LevelEvent_GoToLevel queuedNextLevel;

	[NonSerialized]
	public LevelEvent_GoToLevel scrubbedGoToLevel;

	[NonSerialized]
	public Dictionary<string, CustomAnimationData> customCharacterData = new Dictionary<string, CustomAnimationData>();

	[NonSerialized]
	public Dictionary<string, CustomSprite> sprites = new Dictionary<string, CustomSprite>();

	public bool runningFromTag;

	public TaggedEventType runningTagEventType;

	private static AudioMixerGroup _musicGroup;

	public Level levelToSkipTo;

	public LevelType levelType;

	public int bossActNum = -1;

	public bool multiroom;

	[ListedMod(true)]
	public bool noHitFlashBorder;

	[ListedMod(true)]
	public bool noHitStrips;

	[ListedMod(true)]
	public bool activeDialogues;

	[ListedMod(true)]
	public bool activeDialoguesImmediately;

	[ListedMod(true)]
	public bool skippableRankScreen;

	[ListedMod(true)]
	public bool rowReflectionsJumping;

	[ListedMod(true)]
	public bool alternativeMatrix;

	[ListedMod(true)]
	public bool noHitParticles;

	[ListedMod(true)]
	public float subdivisionMissWeight;

	[ListedMod(true)]
	public float mistakeWeight = 1f;

	[ListedMod(true)]
	public float hitMarginMultiplier = 1f;

	public float shockwaveSizeMultiplier;

	public float shockwaveDistortionMultiplier;

	public float shockwaveDurationMultiplier;

	public float heartExplodeMultiplier;

	public float statusSignWidth = -1f;

	public float crotchetsToExplode;

	[ListedMod(true)]
	public bool disableRowChangeWarningFlashes;

	[ListedMod(true)]
	public bool wobblyLines;

	[ListedMod(true)]
	public bool noBananaBeats;

	[ListedMod(true)]
	public bool noOneshotShadows;

	[ListedMod(true)]
	public bool skipRankText;

	[ListedMod(true)]
	public bool noShadowRowWeighting;

	[ListedMod(true)]
	public bool useSyncoPitchShift;

	[ListedMod(true)]
	public bool showHitstripOnlyOnActiveBeats;

	public bool startImmediately;

	public bool oldRoomBlending;

	public bool oldSubdivFriendBehavior;

	public bool classicHitParticles;

	public bool booleansDefaultToTrue;

	public bool runTaggedEventsWhileScrubbing;

	public bool legacyTaggedEvents;

	public bool oldVFXEasing;

	public bool noSmartJudgment;

	public bool fullResolutionRenderTextures;

	public bool hideHandsOnStart;

	public bool noRowAnimsOnStart;

	public bool charsOnlyOnStart;

	public bool cpuIsP2On2P;

	public bool shadowRowsCopyExpressions;

	public bool useScore;

	public bool playerDrives7thBeat;

	public bool manualLyricAlpha;

	public bool noPopHitstrips;

	public bool cutsceneLevel;

	public bool skippable;

	public bool doublePulse;

	public bool smoothShake;

	public bool rotateShake;

	public bool invisibleChars;

	public bool noHands;

	public bool subdivisions;

	public bool bombBeats;

	public bool noGetSet;

	public bool gentleBassDrop;

	public bool countHeldPresses;

	public bool hideHandsAfterStart;

	public bool PanExplodeSound;

	public bool forceNoFrameOffsets;

	public bool noFreezeshotExplosion;

	public bool earlyFreezeshotExplosion;

	public bool dogMode;

	public bool rowReflections;

	public (int startBar, int endBar)[] cutsceneBarsDesc;

	public int[] checkpointBarsDesc;

	public bool narrateHP = true;

	public bool noBossFail;

	public bool manualHeartCrack;

	public float missesToCrackHeart;

	public string customGameoverDescription;

	public string customGameoverHeader;

	public bool shouldJyiSayOneshot;

	public bool failedLevel;

	public bool runFirstActionBarInPreactions;

	public bool keepHandsInFront;

	public bool missedAny;

	public bool delayRankVoice;

	public bool museDashResults;

	public bool flashTextEnabled;

	public bool noCutsceneLetterbox;

	public bool alwaysSkippableCutscene;

	public bool pauseOn7thBeat;

	public bool dontAddEmptyStripsForPlayers;

	public bool invisibleHeart;

	public bool useFlashFontForFloatingText;

	public bool oldBassDrop;

	public int score;

	public HeartExplodeType heartExplodeType;

	public float[] rankLowerBounds;

	public List<int> rowReflectionJumpOverrides = new List<int>();

	public string[] rankDescriptions;

	public string[] songsUsed;

	public bool loadedHeldBeatSounds;

	public float deltaTime;

	private const string LevelsFolder = "EditorLevels";

	public static string levelHash = "";

	public List<AudioSource> metronomeAudioSources = new List<AudioSource>();

	private readonly int[] OnTopRoomSelection = new int[1] { -1 };

	public float specialSpeedHack;

	private scrExecuteOnHit eohMarginPerfect;

	private scrExecuteOnHit eohMarginEarly;

	private scrExecuteOnHit eohMarginLate;

	private scrExecuteOnHit eohMarginCompleteMiss;

	private bool hasWarnedMargin;

	private const string LevelAssetPath = "Levels/";

	private const string PublicPrefabPath = "PublicPrefabs/";

	private Tween tileXTween;

	private Tween tileYTween;

	private Tween tileXAnchorTween;

	private Tween tileYAnchorTween;

	private Tween scrollXTween;

	private Tween scrollYTween;

	private Tween hueShiftTween;

	private Tween brightnessTween;

	private Tween contrastTween;

	private Tween saturationTween;

	private Tween fisheyeTween;

	private bool usingBrightness;

	private bool usingContrast;

	private bool usingSaturation;

	private List<RDThemeFX> usedThemeFX = new List<RDThemeFX>();

	public int buttonPressCount;

	public int i0;

	public int i1;

	public int i2;

	public int i3;

	public int i4;

	public int i5;

	public int i6;

	public int i7;

	public int i8;

	public int i9;

	public float f0;

	public float f1;

	public float f2;

	public float f3;

	public float f4;

	public float f5;

	public float f6;

	public float f7;

	public float f8;

	public float f9;

	public bool b0;

	public bool b1;

	public bool b2;

	public bool b3;

	public bool b4;

	public bool b5;

	public bool b6;

	public bool b7;

	public bool b8;

	public bool b9;

	private int buttonPressCountPrevious;

	public OffsetType lastHeldPress;

	public OffsetType lastHeldRelease;

	private static readonly DataTable dt = new DataTable();

	private const string randPrefix = "Rand(";

	public bool dontKillWindowDanceOnGameOverUntilExit;

	public bool glitchObstructionOnGameOverExit;

	public static AudioMixerGroup musicGroup
	{
		get
		{
			if ((object)_musicGroup == null)
			{
				_musicGroup = RDUtils.GetMixerGroup("LevelMusic");
			}
			return _musicGroup;
		}
	}

	public bool skippedLastSkippableCutscene => base.game.skippedLastSkippableCutscene;

	public bool shouldMakeHealthBar
	{
		get
		{
			if (levelType == LevelType.Boss)
			{
				return !(base.game.currentLevel is Level_Lesmis);
			}
			return false;
		}
	}

	public Rankscreen rankscreen => base.game.rankscreen;

	public Row[] rows => base.game.rows;

	public Camera tkCam => room0.camera;

	public RDRoom room0 => base.game.rooms[0];

	public RDRoom room1 => base.game.rooms[1];

	public RDRoom room2 => base.game.rooms[2];

	public RDRoom room3 => base.game.rooms[3];

	public RowEntity rowEnt0 => base.game.rows[0].ent;

	public RowEntity rowEnt1 => base.game.rows[1].ent;

	public RowEntity rowEnt2 => base.game.rows[2].ent;

	public RowEntity rowEnt3 => base.game.rows[3].ent;

	public bool isScrubbing
	{
		get
		{
			if (!base.conductor.isScrubbing)
			{
				return base.conductor.runningScrubToNextBar;
			}
			return true;
		}
	}

	public bool isOneRowButNotBoss
	{
		set
		{
			if (value)
			{
				missesToCrackHeart = rankLowerBounds[3] + 1f;
			}
		}
	}

	public bool buttonPress
	{
		get
		{
			if (buttonPressCount != buttonPressCountPrevious)
			{
				if (buttonPressCount > buttonPressCountPrevious)
				{
					buttonPressCountPrevious = buttonPressCount;
					return true;
				}
				return false;
			}
			return false;
		}
	}

	public bool upPress => RDInput.upPress;

	public bool downPress => RDInput.downPress;

	public bool leftPress => RDInput.leftPress;

	public bool rightPress => RDInput.rightPress;

	public bool upIsPressed => RDInput.upIsPressed;

	public bool downIsPressed => RDInput.downIsPressed;

	public bool leftIsPressed => RDInput.leftIsPressed;

	public bool rightIsPressed => RDInput.rightIsPressed;

	public bool upRelease => RDInput.upRelease;

	public bool downRelease => RDInput.downRelease;

	public bool leftRelease => RDInput.leftRelease;

	public bool rightRelease => RDInput.rightRelease;

	public bool upReleased => RDInput.upRelease;

	public bool downReleased => RDInput.downRelease;

	public bool leftReleased => RDInput.leftRelease;

	public bool rightReleased => RDInput.rightRelease;

	public bool anyPlayerPress => RDInput.anyPlayerPress;

	public bool p1Press => RDInput.p1Press;

	public bool p2Press => RDInput.p2Press;

	public bool p1IsPressed => RDInput.p1IsPressed;

	public bool p2IsPressed => RDInput.p2IsPressed;

	public bool p1Release => RDInput.p1Release;

	public bool p2Release => RDInput.p2Release;

	public bool anyPlayerRelease
	{
		get
		{
			if (!RDInput.p1Release)
			{
				return RDInput.p2Release;
			}
			return true;
		}
	}

	public bool autoplay => RDClass.debugSettings.Auto;

	public bool isChineseVoice => RDString.voiceLanguage == SystemLanguage.Chinese;

	public bool arePlayerInputsSwapped => RDInput.persistenceSwapped;

	public float levelSpeed => RDTime.speed;

	public float bpm => base.conductor.bpm;

	public int crotchets => base.conductor.crotchetsPerBar;

	public float audioVolume => base.conductor.mainVolume;

	public int numPerfectHits => base.game.allHitOffsets.Count((scnGame.HitInfo a) => a.offsetType == OffsetType.Perfect);

	public int numMisses => base.game.allHitOffsets.Count((scnGame.HitInfo a) => a.offsetType != OffsetType.Perfect);

	public int numEarlyHits => base.game.allHitOffsets.Count((scnGame.HitInfo a) => a.offsetType == OffsetType.VeryEarly || a.offsetType == OffsetType.SlightlyEarly);

	public int numLateHits => base.game.allHitOffsets.Count((scnGame.HitInfo a) => a.offsetType == OffsetType.VeryLate || a.offsetType == OffsetType.SlightlyLate);

	public float numMistakes => base.game.mistakesManager.mistakes;

	public float numMistakesP1 => base.game.mistakesManager.mistakesP1;

	public float numMistakesP2 => base.game.mistakesManager.mistakesP2;

	public float lateOffset => base.game.mistakesManager.lateOffsetsSum;

	public float lateOffsetP1 => base.game.mistakesManager.lateOffsetsSumP1;

	public float lateOffsetP2 => base.game.mistakesManager.lateOffsetsSumP2;

	public float earlyOffset => base.game.mistakesManager.earlyOffsetsSum;

	public float earlyOffsetP1 => base.game.mistakesManager.earlyOffsetsSumP1;

	public float earlyOffsetP2 => base.game.mistakesManager.earlyOffsetsSumP2;

	public float totalOffset => base.game.mistakesManager.totalOffsetsSum;

	public float totalOffsetP1 => base.game.mistakesManager.totalOffsetsSumP1;

	public float totalOffsetP2 => base.game.mistakesManager.totalOffsetsSumP2;

	public float lastOffsetP1 => base.game.mistakesManager.mistakeOffsetsListP1.DefaultIfEmpty(0f).Last();

	public float lastOffsetP2 => base.game.mistakesManager.mistakeOffsetsListP2.DefaultIfEmpty(0f).Last();

	public float hitMarginP1 => scnGame.GetHitMargin(RDPlayer.P1);

	public float hitMarginP2 => scnGame.GetHitMargin(RDPlayer.P2);

	public bool isZeroOffset
	{
		get
		{
			if (totalOffset == 0f && numMistakes == 0f)
			{
				return !autoplay;
			}
			return false;
		}
	}

	public bool isJCI => isZeroOffset;

	public bool detailedLevelResults => base.game.showAbsoluteOffsets;

	public int editorVersion => 68;

	public bool inEditor => base.game.editorMode;

	public int barNumber => base.conductor.barNumberUpdateOnBar;

	public int nextBarNumber => base.conductor.barNumberToGo;

	public int beatNumber => Mathf.FloorToInt(base.conductor.currentBeat);

	public float beatNumberFloat => base.conductor.currentBeat;

	public LevelBase()
	{
		rankLowerBounds = new float[6] { 100f, 20f, 15f, 10f, 5f, 0f };
		rankDescriptions = new string[6] { "Error: F", "Error: C", "Error: D", "Error: B", "Error: A", "Error: S" };
		skippableRankScreen = true;
		levelType = LevelType.Regular;
		shouldJyiSayOneshot = true;
		mistakeWeight = 1f;
		checkpointBarsDesc = new int[0];
		cutsceneBarsDesc = new (int, int)[0];
		missesToCrackHeart = 5f;
		crotchetsToExplode = 1f;
		heartExplodeMultiplier = 0.6f;
		heartExplodeType = HeartExplodeType.GatherAndCeil;
		multiroom = false;
		doublePulse = true;
		shockwaveSizeMultiplier = 1f;
		shockwaveDistortionMultiplier = 1f;
		shockwaveDurationMultiplier = 1f;
		baseInit();
		Init();
	}

	public LevelBase(RDLevelData data)
		: this()
	{
		if (data == null)
		{
			return;
		}
		this.data = data;
		levelEvents = data.levelEvents;
		LevelSource levelToLoadSource = scnGame.levelToLoadSource;
		cpbEvents = new List<LevelEvent_SetCrotchetsPerBar>
		{
			new LevelEvent_SetCrotchetsPerBar
			{
				bar = 1,
				beat = 1f,
				crotchetsPerBar = 8
			}
		};
		levelEvents.RemoveAll((LevelEvent_Base levelEvent) => !levelEvent.CheckConditionals(checkConstantOnly: true));
		foreach (LevelEvent_Base levelEvent in data.levelEvents)
		{
			if (!string.IsNullOrEmpty(levelEvent.tag))
			{
				if (!taggedEvents.TryGetValue(levelEvent.tag, out var value))
				{
					value = new List<LevelEvent_Base>();
					taggedEvents.Add(levelEvent.tag, value);
				}
				value.Add(levelEvent);
			}
		}
		multiroom = true;
		if (levelToLoadSource == LevelSource.ExternalPath)
		{
			rankDescriptions = data.settings.rankDescription;
		}
		rankLowerBounds = data.settings.rankLowerBounds;
		missesToCrackHeart = data.settings.rankLowerBounds[3];
		base.game.multiplayerAppearance = data.settings.multiplayerAppearance;
		if (GetType() == typeof(LevelBase) || levelToLoadSource == LevelSource.ExternalPath)
		{
			hideHandsAfterStart = true;
		}
		scrConductor.instance.externalSoundData = new Dictionary<string, SongOffset>();
		if (data.settings.mods != null)
		{
			if (ModExists("bombBeats"))
			{
				bombBeats = true;
			}
			if (ModExists("smoothShake"))
			{
				smoothShake = true;
			}
			if (ModExists("rotateShake"))
			{
				rotateShake = true;
			}
			if (ModExists("noSmartBeatJudgment"))
			{
				noSmartJudgment = true;
			}
			if (ModExists("noSmartJudgment"))
			{
				noSmartJudgment = true;
			}
			if (ModExists("cutsceneLevel"))
			{
				cutsceneLevel = true;
			}
			if (ModExists("skippable"))
			{
				skippable = true;
			}
			if (ModExists("wobblyLines"))
			{
				wobblyLines = true;
			}
			if (ModExists("showHitStripOnlyOnActiveBeats"))
			{
				showHitstripOnlyOnActiveBeats = true;
			}
			if (ModExists("invisibleCharacters"))
			{
				invisibleChars = true;
			}
			if (ModExists("disableRowChangeWarningFlashes"))
			{
				disableRowChangeWarningFlashes = true;
			}
			if (ModExists("noBananaBeats"))
			{
				noBananaBeats = true;
			}
			if (ModExists("noOneshotShadows"))
			{
				noOneshotShadows = true;
			}
			if (ModExists("hideHandsOnStart"))
			{
				hideHandsOnStart = true;
			}
			if (ModExists("noRowAnimsOnStart"))
			{
				noRowAnimsOnStart = true;
			}
			if (ModExists("charsOnlyOnStart"))
			{
				charsOnlyOnStart = true;
			}
			if (ModExists("subdivisions"))
			{
				subdivisions = true;
			}
			if (ModExists("noHands"))
			{
				noHands = true;
			}
			if (ModExists("cpuIsP2On2P"))
			{
				cpuIsP2On2P = true;
			}
			if (ModExists("skipRankText"))
			{
				skipRankText = true;
			}
			if (ModExists("useScore"))
			{
				useScore = true;
			}
			if (ModExists("startImmediately"))
			{
				startImmediately = true;
			}
			if (ModExists("shadowRowsCopyExpressions"))
			{
				shadowRowsCopyExpressions = true;
			}
			if (ModExists("playerDrives7thBeat"))
			{
				playerDrives7thBeat = true;
			}
			if (ModExists("manualLyricAlpha"))
			{
				manualLyricAlpha = true;
			}
			if (ModExists("noPopHitstrips"))
			{
				noPopHitstrips = true;
			}
			if (ModExists("noShadowRowWeighting"))
			{
				noShadowRowWeighting = true;
			}
			if (ModExists("fullResolutionRenderTextures"))
			{
				fullResolutionRenderTextures = true;
			}
			if (ModExists("useSyncoPitchShift"))
			{
				useSyncoPitchShift = true;
			}
			if (ModExists("oldRoomBlending"))
			{
				oldRoomBlending = true;
			}
			if (ModExists("oldSubdivFriendBehavior"))
			{
				oldSubdivFriendBehavior = true;
			}
			if (ModExists("classicHitParticles"))
			{
				classicHitParticles = true;
			}
			if (ModExists("booleansDefaultToTrue"))
			{
				booleansDefaultToTrue = true;
			}
			if (ModExists("runTaggedEventsWhileScrubbing"))
			{
				runTaggedEventsWhileScrubbing = true;
			}
			if (ModExists("legacyTaggedEvents"))
			{
				legacyTaggedEvents = true;
			}
			if (ModExists("oldVFXEasing"))
			{
				oldVFXEasing = true;
			}
			if (ModExists("bossLevel"))
			{
				levelType = LevelType.Boss;
			}
			if (ModExists("doublePulse"))
			{
				doublePulse = true;
			}
			if (ModExists("noDoublePulse"))
			{
				doublePulse = false;
			}
		}
	}

	private bool ModExists(string mod)
	{
		string[] mods = data.settings.mods;
		for (int i = 0; i < mods.Length; i++)
		{
			if (string.Equals(mods[i].Trim(), mod, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
		}
		return false;
	}

	public static LevelBase InstantiateLevelClass(Level level)
	{
		return Activator.CreateInstance(LevelSelector.GetLevelTypeFromEnum(level)) as LevelBase;
	}

	public static LevelBase InstantiateLevelClass(Level level, RDLevelData data)
	{
		return Activator.CreateInstance(LevelSelector.GetLevelTypeFromEnum(level), data) as LevelBase;
	}

	public static LevelBase InstantiateLevelClass(string jsonText)
	{
		RDLevelData rDLevelData = DecodeLevelData(jsonText);
		if (rDLevelData == null)
		{
			if (scnEditor.instance != null)
			{
				return new LevelBase
				{
					multiroom = true,
					loadingSuccessful = false
				};
			}
			scnCLS.errorLoadingLevel = true;
			scnBase.GoToLevelSelect();
			return null;
		}
		if (!string.IsNullOrEmpty(rDLevelData?.settings.customClass))
		{
			Level level = RDUtils.ParseEnum(rDLevelData.settings.customClass, Level.None);
			if (level != Level.None)
			{
				return InstantiateLevelClass(level, rDLevelData);
			}
		}
		return new LevelBase(rDLevelData);
	}

	public void DisableTag(string tag)
	{
		foreach (KeyValuePair<string, List<LevelEvent_Base>> taggedEvent in taggedEvents)
		{
			List<LevelEvent_Base> value = taggedEvent.Value;
			if (!(taggedEvent.Key == tag) && !(taggedEvent.Key.ExcludeSquareBracketed() == tag))
			{
				continue;
			}
			foreach (LevelEvent_Base item in value)
			{
				item.tagActive = false;
			}
		}
	}

	public void EnableTag(string tag)
	{
		foreach (KeyValuePair<string, List<LevelEvent_Base>> taggedEvent in taggedEvents)
		{
			List<LevelEvent_Base> value = taggedEvent.Value;
			if (!(taggedEvent.Key == tag) && !(taggedEvent.Key == tag.ExcludeSquareBracketed()))
			{
				continue;
			}
			foreach (LevelEvent_Base item in value)
			{
				item.tagActive = true;
			}
		}
	}

	public void DisableTagContaining(string tag)
	{
		DisableTagsContaining(tag);
	}

	public void DisableTagsContaining(string tag)
	{
		foreach (KeyValuePair<string, List<LevelEvent_Base>> taggedEvent in taggedEvents)
		{
			List<LevelEvent_Base> value = taggedEvent.Value;
			if (!taggedEvent.Key.Contains(tag))
			{
				continue;
			}
			foreach (LevelEvent_Base item in value)
			{
				item.tagActive = false;
			}
		}
	}

	public void EnableTagContaining(string tag)
	{
		EnableTagsContaining(tag);
	}

	public void EnableTagsContaining(string tag)
	{
		foreach (KeyValuePair<string, List<LevelEvent_Base>> taggedEvent in taggedEvents)
		{
			List<LevelEvent_Base> value = taggedEvent.Value;
			if (!taggedEvent.Key.Contains(tag))
			{
				continue;
			}
			foreach (LevelEvent_Base item in value)
			{
				item.tagActive = true;
			}
		}
	}

	public void RunTagContaining(bool legacy = false, TaggedEventType typeToRun = TaggedEventType.All, params string[] tags)
	{
		RunTagContainingGeneric(taggedEvents, legacy, typeToRun, tags);
	}

	public void RunTagContaining(bool legacy = false, params string[] tags)
	{
		RunTagContainingGeneric(taggedEvents, legacy, tags);
	}

	public void RunTagContainingGeneric(Dictionary<string, List<LevelEvent_Base>> dict, bool legacy = false, TaggedEventType typeToRun = TaggedEventType.All, params string[] tags)
	{
		foreach (KeyValuePair<string, List<LevelEvent_Base>> item in dict.Filter((KeyValuePair<string, List<LevelEvent_Base>> kvp) =>
		{
			bool result = true;
			string[] array = tags;
			foreach (string value in array)
			{
				if (!kvp.Key.Contains(value))
				{
					result = false;
				}
			}
			return result;
		}))
		{
			RunEventsWithTag(item.Key, -1f, runPreEvent: false, legacyTaggedEvents | legacy);
		}
	}

	public void RunTagContainingGeneric(Dictionary<string, List<LevelEvent_Base>> dict, bool legacy = false, params string[] tags)
	{
		RunTagContainingGeneric(dict, legacy, TaggedEventType.All, tags);
	}

	public void RunTag(string tag)
	{
		RunTagWithType(tag, TaggedEventType.All);
	}

	public void RunTagWithType(string tag, TaggedEventType typeToRun)
	{
		RunEventsWithTag(tag, -1f, runPreEvent: false, legacyTaggedEvents, typeToRun);
	}

	public void RunRandomTagWithType(string tag, TaggedEventType typeToRun)
	{
		RunEventsWithTag(tag, -1f, runPreEvent: false, legacyTaggedEvents, typeToRun, chooseOneRandomly: true);
	}

	public void RunEventsWithTag(string tag, float beat = -1f, bool runPreEvent = false, bool legacy = false, TaggedEventType typeToRun = TaggedEventType.All, bool chooseOneRandomly = false)
	{
		if (beat < 0f)
		{
			beat = base.conductor.TimeToBeatRefreshOnBar(base.conductor.visualPos - base.conductor.startOfNextBarRefreshOnlyOnBar);
		}
		runningFromTag = true;
		if (taggedEvents.ContainsKey(tag))
		{
			IEnumerable<LevelEvent_Base> enumerable2;
			if (chooseOneRandomly)
			{
				IEnumerable<LevelEvent_Base> enumerable = new LevelEvent_Base[1] { taggedEvents[tag][UnityEngine.Random.Range(0, taggedEvents[tag].Count)] };
				enumerable2 = enumerable;
			}
			else
			{
				IEnumerable<LevelEvent_Base> enumerable = taggedEvents[tag].OrderBy((LevelEvent_Base a) => a.bar1000timesbeat);
				enumerable2 = enumerable;
			}
			IEnumerable<LevelEvent_Base> enumerable3 = enumerable2;
			foreach (LevelEvent_Base item in enumerable3)
			{
				if (!item.tagActive)
				{
					continue;
				}
				bool flag = item is LevelEvent_PlaySound;
				bool flag2 = item is LevelEvent_TagAction;
				if (typeToRun != TaggedEventType.All && !flag2)
				{
					if (typeToRun == TaggedEventType.OnlySound)
					{
						if (!flag)
						{
							continue;
						}
					}
					else if (flag)
					{
						continue;
					}
				}
				float num = ((!legacy && data.settings.version >= 53) ? (Timeline.GetColumnFromBarAndBeat(item.barAndBeat, cpbEvents, debug: false) - Timeline.GetColumnFromBarAndBeat(enumerable3.First().barAndBeat, cpbEvents, debug: false)) : (item.beat - enumerable3.First().beat));
				float timeInBeats = beat + num;
				if (base.conductor.isScrubbingRefreshOnBar)
				{
					runningFromTag = true;
					runningTagEventType = typeToRun;
					item.Run();
					runningFromTag = false;
					continue;
				}
				scrExecuteOnCertainBeat.Add(() =>
				{
					runningFromTag = true;
					runningTagEventType = typeToRun;
					item.Run();
					runningFromTag = false;
				}, timeInBeats, null, minusLatency: false, calledCloseToNextBarMaybe: true);
			}
		}
		runningFromTag = false;
		runningTagEventType = TaggedEventType.All;
	}

	public void RunEventWithTag(string tag)
	{
		RunEventsWithTag(tag);
	}

	public void CalculateCrotchetsInEachBar(List<LevelEvent_Base> levelEvents)
	{
		int num = 1;
		foreach (LevelEvent_Base levelEvent in levelEvents)
		{
			if (levelEvent is LevelEvent_SetCrotchetsPerBar)
			{
				cpbEvents.Add(levelEvent as LevelEvent_SetCrotchetsPerBar);
				num = Math.Max(num, levelEvent.bar);
			}
		}
		crotchetsInEachBar = new int[num + 1];
		foreach (LevelEvent_SetCrotchetsPerBar cpbEvent in cpbEvents)
		{
			int bar = cpbEvent.bar;
			int crotchetsPerBar = cpbEvent.crotchetsPerBar;
			crotchetsInEachBar[bar] = crotchetsPerBar;
		}
		int num2 = 8;
		for (int i = 0; i < num; i++)
		{
			if (crotchetsInEachBar[i] == 0)
			{
				crotchetsInEachBar[i] = num2;
			}
			else
			{
				num2 = crotchetsInEachBar[i];
			}
		}
	}

	private static RDLevelData DecodeLevelData(string text)
	{
		string text2 = FlipScreenFix(text);
		try
		{
			return new RDLevelData(Json.Deserialize(text2) as Dictionary<string, object>, onlyActiveEvents: true);
		}
		catch (Exception ex)
		{
			Debug.Log("Error loading level from text:\n" + ex.Message + "\n\n" + ex.StackTrace + "\n\n" + text2);
			return null;
		}
	}

	public static string FlipScreenFix(string text)
	{
		text = text.Replace("\"y\": true", "\"flipY\": true");
		text = text.Replace("\"x\": true", "\"flipX\": true");
		text = text.Replace("\"y\": false", "\"flipY\": false");
		text = text.Replace("\"x\": false", "\"flipX\": false");
		return text;
	}

	public static List<CustomAnimationData.JsonError> LoadCustomCharacter(Dictionary<string, CustomAnimationData> characterData, string name, string jsonData, Texture2D mainTexture, Texture2D outlineTexture, Texture2D glowTexture, Texture2D freezeTexture)
	{
		if (characterData.ContainsKey(name))
		{
			return new List<CustomAnimationData.JsonError>();
		}
		CustomAnimationData value = default;
		value.Setup(jsonData, mainTexture, outlineTexture, glowTexture, freezeTexture);
		if (value.name.IsNullOrEmpty())
		{
			value.name = name;
		}
		if (value.jsonErrors.Count == 0)
		{
			characterData.Add(name, value);
		}
		return value.jsonErrors;
	}

	public int CountHits(List<LevelEvent_Base> events = null)
	{
		if (events == null)
		{
			events = data.levelEvents;
		}
		int num = 0;
		List<float> list = new List<float>();
		foreach (LevelEvent_Base @event in events)
		{
			if (!@event.active || (!(@event is LevelEvent_AddClassicBeat) && !(@event is LevelEvent_AddOneshotBeat) && !(@event is LevelEvent_AddFreeTimeBeat)))
			{
				continue;
			}
			if (@event is LevelEvent_AddOneshotBeat levelEvent_AddOneshotBeat)
			{
				int num2 = Mathf.Max(levelEvent_AddOneshotBeat.subdivisions, 1);
				for (int i = 0; i < levelEvent_AddOneshotBeat.loops + 1; i++)
				{
					for (int j = 0; j < num2; j++)
					{
						float num3 = levelEvent_AddOneshotBeat.BarAndBeatToAbsoluteBeat(levelEvent_AddOneshotBeat.barAndBeat);
						float num4 = levelEvent_AddOneshotBeat.tick / (float)num2;
						float num5 = levelEvent_AddOneshotBeat.delay + levelEvent_AddOneshotBeat.interval * (float)i + levelEvent_AddOneshotBeat.tick + num4 * (float)j;
						float num6 = num3 + num5;
						bool flag = false;
						foreach (float item in list)
						{
							if (Mathf.Abs(num6 - item) < 0.001f)
							{
								flag = true;
							}
						}
						if (!flag)
						{
							list.Add(num6);
							num++;
						}
					}
				}
			}
			else
			{
				num++;
			}
		}
		return num;
	}

	public void basePreactions()
	{
		if (levelType == LevelType.Boss && rows[1].ent != null && Is1HP())
		{
			OnLowHealthEveryPrebar();
		}
		if (base.conductor.barNumber == 0 || data == null)
		{
			return;
		}
		scnEditor instance = scnEditor.instance;
		if (base.conductor.barNumber == 1)
		{
			base.conductor.SetBPM(firstBPMFound);
			if (!songPlaysAtFirstBar)
			{
				base.conductor.PlayNoSong();
			}
			if ((runFirstActionBarInPreactions || data.settings.firstBeatBehavior == FirstBeatBehavior.RunEventsOnPrebar) && levelEventsPerBar.Length >= 2)
			{
				foreach (LevelEvent_Base item2 in levelEventsPerBar[1])
				{
					if (item2.beat == 1f && item2.executionTime == LevelEventExecutionTime.OnBar && item2.type != LevelEventType.ShowHands)
					{
						item2.executionTime = LevelEventExecutionTime.OnPrebar;
					}
				}
			}
		}
		if (failedLevel)
		{
			return;
		}
		if (base.conductor.barNumber < levelEventsPerBar.Length)
		{
			foreach (LevelEvent_Base item3 in levelEventsPerBar[base.conductor.barNumber].OrderByDescending((LevelEvent_Base levelEvent) => levelEvent is LevelEvent_SetBeatsPerMinute))
			{
				item3.RunPrebar();
				if (item3 is LevelEvent_AddClassicBeat { setRowXs: not null } levelEvent_AddClassicBeat)
				{
					levelEvent_AddClassicBeat.setRowXs.RunPrebar();
					levelEvent_AddClassicBeat.setRowXs.Run();
				}
				if (item3.executionTime == LevelEventExecutionTime.OnPrebar)
				{
					item3.Run();
				}
			}
		}
		if (instance != null && !base.conductor.isScrubbing)
		{
			metronomeAudioSources.Clear();
			string sound = (instance.xmasSeason ? "sndSleighHi" : "sndMetronomeClickHigh");
			string sound2 = (instance.xmasSeason ? "sndSleighLo" : "sndMetronomeClickMid");
			AudioMixerGroup mixerGroup = RDUtils.GetMixerGroup("MetronomeClicks");
			AudioSource item = base.conductor.PlayBeat(sound, 0f, 1f, mixerGroup, 1f, 1f, 0f, 0f, useGlobalBeatSound: false);
			metronomeAudioSources.Add(item);
			for (int num = 1; num < base.conductor.crotchetsPerBar; num++)
			{
				item = base.conductor.PlayBeat(sound2, base.conductor.BeatToTime(num), 1f, mixerGroup, 1f, 1f, 0f, 0f, useGlobalBeatSound: false);
				metronomeAudioSources.Add(item);
			}
		}
	}

	public void baseActions()
	{
		if (base.conductor.barNumber == 0 || data == null)
		{
			return;
		}
		RDRoom[] rooms = RDClass.Vfx.rooms;
		foreach (RDRoom obj in rooms)
		{
			obj.stutter?.ClearStutterCache();
			obj.renderTexPaster?.Reset();
		}
		if (failedLevel || base.conductor.barNumber >= levelEventsPerBar.Length)
		{
			return;
		}
		foreach (LevelEvent_Base item in levelEventsPerBar[base.conductor.barNumber])
		{
			if (item.executionTime == LevelEventExecutionTime.OnBar && (item.tag.IsNullOrEmpty() || item.tagRunNormally || runTaggedEventsWhileScrubbing))
			{
				item.Run();
			}
		}
	}

	public void baseInit()
	{
		if (!Enum.TryParse<Level>(GetType().ToString().Replace("Level_", ""), out var result))
		{
			result = Level.None;
		}
		if (GC.levelMetadata.ContainsKey(result))
		{
			LevelMetadata levelMetadata = GC.levelMetadata[result];
			levelType = levelMetadata.type;
		}
	}

	public virtual void Init()
	{
	}

	public virtual void preactions()
	{
	}

	public virtual void actions()
	{
	}

	public virtual void Update()
	{
	}

	public virtual void LevelEventWasRun(LevelEvent_Base levelEvent)
	{
	}

	public bool Is1HP()
	{
		if (base.game.rows[1].ent != null)
		{
			return base.game.currentLevel.missesToCrackHeart == base.game.rows[1].ent.rowMisses + 1f;
		}
		return false;
	}

	private OneshotSpotlights GetOneshotSpotlights(int roomIndex)
	{
		if (roomIndex != -1)
		{
			return base.game.rooms[roomIndex].oneshotSpotlights;
		}
		return RDClass.Vfx.oneshotSpotlights;
	}

	public virtual void BeepReady(float beepPeriod, bool fromTop = false, int[] rooms = null)
	{
		if (rooms == null)
		{
			rooms = OnTopRoomSelection;
		}
		int[] array = rooms;
		foreach (int roomIndex in array)
		{
			GetOneshotSpotlights(roomIndex).Flash(fromTop);
		}
		PlayBeepAnimations(0, beepPeriod);
	}

	public virtual void BeepGet(float beepPeriod, bool fromTop = false, int[] rooms = null)
	{
		if (rooms == null)
		{
			rooms = OnTopRoomSelection;
		}
		int[] array = rooms;
		foreach (int roomIndex in array)
		{
			GetOneshotSpotlights(roomIndex).Show(0, fromTop);
		}
		PlayBeepAnimations(1, beepPeriod);
	}

	public virtual void BeepSet(float beepPeriod, bool fromTop = false, int[] rooms = null)
	{
		if (rooms == null)
		{
			rooms = OnTopRoomSelection;
		}
		int[] array = rooms;
		foreach (int roomIndex in array)
		{
			GetOneshotSpotlights(roomIndex).Show(1, fromTop);
		}
		PlayBeepAnimations(2, beepPeriod);
	}

	public virtual void BeepGo(float beepPeriod, bool fromTop = false, int[] rooms = null)
	{
		if (rooms == null)
		{
			rooms = OnTopRoomSelection;
		}
		int[] array = rooms;
		foreach (int roomIndex in array)
		{
			GetOneshotSpotlights(roomIndex).Hide(fromTop);
		}
		PlayBeepAnimations(3, beepPeriod);
	}

	public virtual void PlayBeepAnimations(int beep, float beepPeriod = 0f)
	{
		Row[] array = rows;
		foreach (Row row in array)
		{
			if (row.rowType == RowType.Oneshot)
			{
				string expression = row.ent.character.beepAnimName + beep;
				if (row.ent.character.ExpressionExists(expression) && !row.ent.character.noBeep)
				{
					row.ent.character.keepAnimation = beep == 1;
					float speedMultiplier = ((beepPeriod == 0f) ? 1f : (base.conductor.crotchet / beepPeriod));
					row.ent.character.PlayExpression(expression, disableOverride: false, ignoreKeep: true, speedMultiplier);
				}
			}
		}
	}

	public virtual IEnumerator LoadBigAssets()
	{
		yield break;
	}

	public virtual IEnumerator LoadCustomAssets()
	{
		if (scnGame.levelToLoadSource != LevelSource.InternalPath)
		{
			base.game.handControllerOnTop.shouldShow = false;
		}
		string inkFile = data.settings.inkFile;
		if (!string.IsNullOrEmpty(inkFile))
		{
			base.ink.Load(inkFile);
		}
		else if (levelType == LevelType.Cutscene)
		{
			base.ink.Load("diaCutscenes");
		}
		RDRoom[] rooms;
		if (data.settings.mods != null && ModExists("adaptRowsToRoomHeight"))
		{
			rooms = base.game.rooms;
			for (int i = 0; i < rooms.Length; i++)
			{
				rooms[i].rowSeparationTiedToRoomHeight = true;
			}
		}
		rooms = RDClass.Vfx.rooms;
		foreach (RDRoom obj in rooms)
		{
			obj.DisableThemeGeneralEffects();
			obj.PreloadFX();
		}
		CalculateCrotchetsInEachBar(levelEvents);
		int num = 0;
		foreach (LevelEvent_Base levelEvent in levelEvents)
		{
			if (levelEvent.room == 4)
			{
				levelEvent.room = -1;
			}
			int bar = levelEvent.bar;
			if (levelEvent is LevelEvent_PlaySong levelEvent_PlaySong)
			{
				if (firstBPMFound == 0f)
				{
					firstBPMFound = levelEvent_PlaySong.beatsPerMinute;
				}
				if (bar == 1)
				{
					songPlaysAtFirstBar = true;
				}
				BPMChange item = new BPMChange(levelEvent.bar, levelEvent.beat, levelEvent_PlaySong.beatsPerMinute, 0f);
				bpmChanges.Add(item);
			}
			else if (levelEvent is LevelEvent_SetBeatsPerMinute levelEvent_SetBeatsPerMinute)
			{
				BPMChange item2 = new BPMChange(levelEvent.bar, levelEvent.beat, levelEvent_SetBeatsPerMinute.beatsPerMinute, 0f);
				bpmChanges.Add(item2);
			}
			if (levelEvent.usesPrepare)
			{
				base.game.StartCoroutine(levelEvent.Prepare());
			}
			else
			{
				levelEvent.prepared = true;
			}
		}
		levelEvents.AddRange(dynamicallyAddedEvents);
		LevelEvent_AddOneshotBeat levelEvent_AddOneshotBeat = null;
		BarAndBeat barAndBeat = default;
		List<LevelEvent_AddOneshotBeat> list = new List<LevelEvent_AddOneshotBeat>();
		foreach (LevelEvent_Base levelEvent2 in levelEvents)
		{
			if (levelEvent2 is LevelEvent_AddOneshotBeat item3)
			{
				list.Add(item3);
			}
			num = Math.Max(num, levelEvent2.bar);
		}
		foreach (LevelEvent_AddOneshotBeat item4 in list.OrderBy((LevelEvent_AddOneshotBeat oneshot) => oneshot.BarAndBeatToAbsoluteBeat(oneshot.barAndBeat)).ToList())
		{
			if (levelEvent_AddOneshotBeat != null && levelEvent_AddOneshotBeat.bar != item4.bar && Mathf.Abs(levelEvent_AddOneshotBeat.absoluteClapPos - item4.absoluteClapPos) < 0.001f)
			{
				BarAndBeat barAndBeat2 = new BarAndBeat(item4.bar, 1f);
				float num2 = item4.BarAndBeatToAbsoluteBeat(barAndBeat2) - item4.BarAndBeatToAbsoluteBeat(barAndBeat);
				item4.bar = levelEvent_AddOneshotBeat.bar;
				item4.beat += num2;
			}
			else
			{
				levelEvent_AddOneshotBeat = item4;
				barAndBeat = new BarAndBeat(item4.bar, 1f);
			}
		}
		levelEventsPerBar = new List<LevelEvent_Base>[num + 1];
		for (int num3 = 0; num3 < num + 1; num3++)
		{
			levelEventsPerBar[num3] = new List<LevelEvent_Base>();
		}
		foreach (LevelEvent_Base levelEvent3 in levelEvents)
		{
			int bar2 = levelEvent3.bar;
			if (levelEventsPerBar[bar2] == null)
			{
				levelEventsPerBar[bar2] = new List<LevelEvent_Base>();
			}
			levelEventsPerBar[bar2].Add(levelEvent3);
		}
		if (firstBPMFound == 0f)
		{
			firstBPMFound = 100f;
		}
		bool allPrepared = false;
		bool errorOccurred = false;
		do
		{
			try
			{
				allPrepared = true;
				foreach (LevelEvent_Base levelEvent4 in levelEvents)
				{
					if (!levelEvent4.prepared)
					{
						allPrepared = false;
						break;
					}
				}
			}
			catch
			{
				errorOccurred = true;
				printe("Error loading big assets");
			}
			if (errorOccurred)
			{
				yield break;
			}
			yield return null;
		}
		while (!allPrepared);
		rooms = RDClass.Vfx.rooms;
		foreach (RDRoom obj3 in rooms)
		{
			obj3.DisableThemeGeneralEffects();
			obj3.PreloadFX();
			obj3.SetPremultiplyAlpha(!oldRoomBlending);
		}
		if (booleansDefaultToTrue)
		{
			LevelBase levelBase = this;
			LevelBase levelBase2 = this;
			LevelBase levelBase3 = this;
			LevelBase levelBase4 = this;
			LevelBase levelBase5 = this;
			LevelBase levelBase6 = this;
			LevelBase levelBase7 = this;
			LevelBase levelBase8 = this;
			LevelBase levelBase9 = this;
			LevelBase levelBase10 = this;
			bool flag = true;
			levelBase10.b9 = true;
			levelBase.b0 = (levelBase2.b1 = (levelBase3.b2 = (levelBase4.b3 = (levelBase5.b4 = (levelBase6.b5 = (levelBase7.b6 = (levelBase8.b7 = (levelBase9.b8 = flag))))))));
		}
		yield return null;
	}

	public virtual void OnMistake(RowEntity ent = null)
	{
	}

	public virtual void OnHit(HitType hitType, Beat beat = null)
	{
	}

	public virtual void OnHeldPress(HitType hitType, Beat beat = null)
	{
	}

	public virtual void OnLowHealthEveryPrebar()
	{
	}

	public virtual void OnHeartBeat()
	{
	}

	public void OnHit(OffsetType offsetType, Beat beat = null)
	{
		HitType hitType = HitType.Hit;
		if (Mathf.Abs((int)offsetType) == 0)
		{
			hitType = HitType.Hit;
		}
		if (Mathf.Abs((int)offsetType) == 1)
		{
			hitType = HitType.JustMiss;
		}
		if (Mathf.Abs((int)offsetType) == 2)
		{
			hitType = HitType.MissCompletely;
		}
		OnHit(hitType, beat);
	}

	public void OnLowHealthRowEveryPrebar(int rowID)
	{
	}

	public virtual void OnEnterLowHealth()
	{
		RDClass.Vfx.Flash();
		base.game.frontOfAllOverlay.gameObject.SetActive(value: true);
		base.game.frontOfAllOverlay.color = Color.red.WithAlpha(0.2f);
	}

	public virtual void OnExitLowHealth()
	{
		base.game.frontOfAllOverlay.gameObject.SetActive(value: false);
	}

	public virtual bool FailLevel(RowEntity ent)
	{
		return false;
	}

	public void OnLevelStartedBase()
	{
		scrExecuteOnHit.Add(() =>
		{
			missedAny = true;
		}, HitType.AnyMiss, persistent: true, autoDelete: false, -1, overwrite: false);
		Type type = GetType();
		Dictionary<Type, int> levelTries = RDStartup.levelTries;
		if (levelTries.ContainsKey(type))
		{
			levelTries[type]++;
		}
		else
		{
			levelTries.Add(type, 1);
		}
	}

	public void ChallengeModeFakePause()
	{
		RDClass.Vfx.SetOverlayToAlpha(0.6f);
		RDTime.fakePauseEverything = true;
		base.game.SetVisualSpeed(0f);
	}

	public void ChallengeModeFakeResume()
	{
		RDClass.Vfx.SetOverlayToAlpha(0f);
		RDTime.fakePauseEverything = false;
		base.game.SetVisualSpeed(specialSpeedHack);
	}

	public void Test(string msg)
	{
		Debug.Log(msg);
	}

	public void TestSound(string msg)
	{
		Debug.Log(msg);
		scrConductor.PlayImmediatelyLevelEditor("sndKickRupture", RDUtils.GetMixerGroup("MasterPrivate"));
	}

	public void SetOneshotType(int row, string type)
	{
		if (rows[row] != null && Enum.TryParse<WaveType>(type, out var result))
		{
			rows[row].ent.oneshotRowController.waveManager.waveType = result;
		}
	}

	public void SetMistakeWeight(int row, float weight)
	{
		if (rows[row] != null)
		{
			rows[row].mistakeWeight = weight;
		}
	}

	[ListedMethod(true)]
	public void SetMistakeWeightInstant(int row, float weight)
	{
		SetMistakeWeight(row, weight);
		RefreshMistakeWeight();
	}

	public void RefreshMistakeWeight()
	{
		foreach (Beat beat in base.game.beats)
		{
			beat.RefreshMistakeWeight();
		}
	}

	public void PrepareMusic()
	{
		if (songsUsed != null)
		{
			string[] array = songsUsed;
			foreach (string text in array)
			{
				Singleton<AudioManager>.Instance.FindOrLoadAudioClip("Music/" + text);
			}
		}
	}

	public void PlayMusic(string song)
	{
		base.conductor.previousSong = base.conductor.currentSong;
		base.conductor.currentSong = base.conductor.PlaySong(song, musicGroup);
	}

	public void FadeOutMusic(float duration = 0.5f)
	{
		base.conductor.previousSong?.DOKill();
		base.conductor.previousSong?.DOFade(0f, duration).OnComplete(() =>
		{
			base.conductor.previousSong?.Stop();
		});
		if (base.conductor.currentSong == null)
		{
			RDClassDll.printesw($"Trying to fadeout music but audiosource is null: {GetType()}");
		}
		else
		{
			base.conductor.StopSong(duration);
		}
	}

	public void print(object message)
	{
		MonoBehaviour.print(message);
	}

	public bool IsPassingRank(Rank rank)
	{
		return base.game.mistakesManager.mistakes > rankLowerBounds[3];
	}

	[ListedMethod(true)]
	public void SetRankMargin(string rankStr, int margin)
	{
		Rank rank = Rank.FromString(rankStr[0].ToString().ToUpper());
		if ((int)rank != 0 && (int)rank != 5)
		{
			base.game.currentLevel.rankLowerBounds[(int)rank] = margin;
		}
	}

	public Rank GetRankFromMistakes()
	{
		float mistakes = base.game.mistakesManager.mistakes;
		float[] array = base.game.currentLevel.rankLowerBounds;
		Rank result = 0;
		if (mistakes == 0f)
		{
			result = 5;
		}
		else if (mistakes <= array[4])
		{
			result = 4;
		}
		else if (mistakes <= array[3])
		{
			result = 3;
		}
		else if (mistakes <= array[2])
		{
			result = 2;
		}
		else if (mistakes <= array[1])
		{
			result = 1;
		}
		bool flag = false;
		bool flag2 = false;
		if ((GC.twoPlayerMode && scnGame.p1DefibMode == DefibMode.Hard && scnGame.p2DefibMode == DefibMode.Hard) || (!GC.twoPlayerMode && scnGame.p1DefibMode == DefibMode.Hard))
		{
			flag = true;
		}
		else
		{
			List<float> p1HitTimes = scnGame.p1HitTimes;
			if (GC.twoPlayerMode)
			{
				p1HitTimes.AddRange(scnGame.p2HitTimes);
			}
			int num = p1HitTimes.Count((float h) => h < 0.04f);
			flag = p1HitTimes.Count == 0 || num > p1HitTimes.Count / 2;
			if (!flag)
			{
				flag2 = p1HitTimes.Count((float h) => h > 0.08f) > p1HitTimes.Count / 2;
			}
		}
		if (flag)
		{
			result = result.ToPlus();
		}
		else if (flag2)
		{
			result = result.ToMinus();
		}
		return result;
	}

	public void BeatShake()
	{
		tkCam.Shake(3, 2);
	}

	public void SetUpFlashMarginFeedback(RowType rowType = RowType.Classic, float lengthInCrotchets = 1f, bool flash = false)
	{
		base.game.currentLevel.flashTextEnabled = true;
		eohMarginEarly = scrExecuteOnHit.AddWithRow((int row) =>
		{
			RDClass.Vfx.FlashText(RDString.Get("status.early"), lengthInCrotchets, 1f, flash, row);
		}, HitType.EarlyOnly, persistent: true, autoDelete: false, -1, overwrite: false);
		eohMarginLate = scrExecuteOnHit.AddWithRow((int row) =>
		{
			RDClass.Vfx.FlashText(RDString.Get("status.late"), lengthInCrotchets, 1f, flash, row);
		}, HitType.LateOnly, persistent: true, autoDelete: false, -1, overwrite: false);
		eohMarginCompleteMiss = scrExecuteOnHit.AddWithRow((int row) =>
		{
			string text = ((rowType == RowType.Classic && hasWarnedMargin) ? "seventhBeat" : "late");
			RDClass.Vfx.FlashText(RDString.Get("status." + text), lengthInCrotchets, 1f, flash, row);
			hasWarnedMargin = true;
		}, HitType.MissCompletely, persistent: true, autoDelete: false, -1, overwrite: false);
		eohMarginPerfect = scrExecuteOnHit.AddWithRow((int row) =>
		{
			RDClass.Vfx.FlashText(RDString.Get("status.perfect"), lengthInCrotchets, 1f, flash, row);
		}, HitType.Hit, persistent: true, autoDelete: false, -1, overwrite: false);
	}

	public void MarginFlashFeedback(bool active, bool keepCompleteMiss = false)
	{
		if (eohMarginEarly != null)
		{
			eohMarginEarly.isActive = active;
		}
		if (eohMarginLate != null)
		{
			eohMarginLate.isActive = active;
		}
		if (eohMarginPerfect != null)
		{
			eohMarginPerfect.isActive = active;
		}
		if (eohMarginCompleteMiss != null && !keepCompleteMiss)
		{
			eohMarginCompleteMiss.isActive = active;
		}
	}

	public T LoadLevelAsset<T>() where T : Component
	{
		string name = typeof(T).Name;
		return UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Levels/" + name + "/" + name), base.gm.parentContainer).GetComponent<T>();
	}

	public GameObject LoadLevelAsset(string name)
	{
		return UnityEngine.Object.Instantiate(Resources.Load<GameObject>("Levels/" + name), base.gm.parentContainer);
	}

	public GameObject LoadPublicPrefab(string name)
	{
		return UnityEngine.Object.Instantiate(Resources.Load<GameObject>("PublicPrefabs/" + name), base.gm.parentContainer);
	}

	public void ShowThemeBackgrounds(RDTheme theme, int room = 0, bool disableEffects = true, bool hideOtherBackgrounds = true, bool affectRowContainers = true, bool firstRowOnFloorHack = false, bool paintRows = true, int variant = 0)
	{
		base.game.rooms[room].ShowThemeBackgrounds(theme, disableEffects, hideOtherBackgrounds, affectRowContainers, firstRowOnFloorHack, paintRows, variant);
	}

	public void PreloadTheme(RDTheme theme, int room = 0, int variant = 0)
	{
		base.game.rooms[room].PreloadTheme(theme, variant);
	}

	public Tween TweenThemeFX(Tween tween, DOGetter<float> get, DOSetter<float> set, float targetVal, float duration, Ease ease, bool useTweens)
	{
		tween?.Kill();
		if (useTweens)
		{
			tween = DOTween.To(get, set, targetVal, duration).SetEase(ease).OnComplete(() =>
			{
				set(targetVal);
			});
		}
		else
		{
			set(targetVal);
		}
		return tween;
	}

	public Tween TweenThemeFXColor(Tween tween, DOGetter<Color> get, DOSetter<Color> set, Color targetVal, float duration, Ease ease, bool useTweens)
	{
		tween?.Kill();
		if (useTweens)
		{
			tween = DOTween.To(get, set, targetVal, duration).SetEase(ease).OnComplete(() =>
			{
				set(targetVal);
			});
		}
		else
		{
			set(targetVal);
		}
		return tween;
	}

	public void AddThemeFX(RDThemeFX fx, int room = 0, float? floatX = null, float? floatY = null, float? effectIntensity = null, float? extra = null, float? extra2 = null, float duration = 0f, Ease ease = Ease.Linear, Color? color = null)
	{
		bool useTweens;
		if (room == -1)
		{
			useTweens = duration != 0f;
			RDCamera camera = RDClass.Vfx.GetCamera(-1);
			bool flag = !oldVFXEasing && !usedThemeFX.Contains(fx);
			if (flag)
			{
				usedThemeFX.Add(fx);
			}
			switch (fx)
			{
			case RDThemeFX.Noise:
				RDClass.Vfx.fxNoise.visible = true;
				break;
			case RDThemeFX.GlitchObstruction:
				RDClass.Vfx.fxGlitch.visible = true;
				break;
			case RDThemeFX.Tile2:
				RDClass.Vfx.SetTileScreen(new Vector2(2f, 2f));
				break;
			case RDThemeFX.Tile3:
				RDClass.Vfx.SetTileScreen(new Vector2(3f, 3f));
				break;
			case RDThemeFX.Tile4:
				RDClass.Vfx.SetTileScreen(new Vector2(4f, 4f));
				break;
			case RDThemeFX.TileN:
			{
				RDImageEffectTile tile = camera.GetOrAddComponent<RDImageEffectTile>();
				if (flag)
				{
					tile.tileX = 1f;
					tile.tileY = 1f;
					tile.anchorX = extra.GetValueOrDefault() / 100f;
					tile.anchorY = extra2.GetValueOrDefault() / 100f;
				}
				if (floatX.HasValue)
				{
					tileXTween = TweenVFX(tileXTween, () => tile.tileX, (float x) =>
					{
						tile.tileX = x;
					}, floatX.Value).OnUpdate(() =>
					{
						tile.UpdateState();
					});
				}
				if (floatY.HasValue)
				{
					tileYTween = TweenVFX(tileYTween, () => tile.tileY, (float x) =>
					{
						tile.tileY = x;
					}, floatY.Value).OnUpdate(() =>
					{
						tile.UpdateState();
					});
				}
				if (extra.HasValue)
				{
					tileXAnchorTween = TweenVFX(tileXAnchorTween, () => tile.anchorX, (float x) =>
					{
						tile.anchorX = x;
					}, extra.Value / 100f);
				}
				if (extra2.HasValue)
				{
					tileYAnchorTween = TweenVFX(tileYAnchorTween, () => tile.anchorY, (float x) =>
					{
						tile.anchorY = x;
					}, extra2.Value / 100f);
				}
				if (!useTweens)
				{
					tile.UpdateState();
				}
				break;
			}
			case RDThemeFX.BlackAndWhite:
				camera.GetComponent<Grayscale>().enabled = true;
				break;
			case RDThemeFX.Sepia:
				camera.GetComponent<CameraFilterPack_Color_Sepia>().enabled = true;
				break;
			case RDThemeFX.CustomScreenScroll:
			{
				CameraFilterPack_TV_VHS vhs5 = camera.GetOrAddComponent<CameraFilterPack_TV_VHS>();
				vhs5.enabled = true;
				vhs5.VHSEffectDisabled = 1f;
				if (flag)
				{
					vhs5.Calibrage = 0f;
					vhs5.CalibrageX = 0f;
				}
				if (floatY.HasValue)
				{
					scrollYTween = TweenVFX(scrollYTween, () => vhs5.Calibrage, (float y) =>
					{
						vhs5.Calibrage = y;
					}, floatY.Value / 10f);
				}
				if (floatX.HasValue)
				{
					scrollXTween = TweenVFX(scrollXTween, () => vhs5.CalibrageX, (float x) =>
					{
						vhs5.CalibrageX = x;
					}, floatX.Value / 10f);
				}
				break;
			}
			case RDThemeFX.HallOfMirrors:
				camera.GetComponent<Camera>().clearFlags = CameraClearFlags.Depth;
				break;
			case RDThemeFX.HueShift:
			{
				RDHueShiftDupe hueShift = camera.GetOrAddComponent<RDHueShiftDupe>();
				if (flag)
				{
					hueShift.Hue = 1f;
				}
				hueShift.enabled = true;
				if (effectIntensity.HasValue)
				{
					hueShiftTween = TweenVFX(hueShiftTween, () => hueShift.Hue, (float x) =>
					{
						hueShift.Hue = x;
					}, effectIntensity.Value);
				}
				break;
			}
			case RDThemeFX.Brightness:
			{
				CameraFilterPack_Color_BrightContrastSaturation bri = camera.GetOrAddComponent<CameraFilterPack_Color_BrightContrastSaturation>();
				bri.enabled = true;
				usingBrightness = true;
				if (flag)
				{
					bri.Brightness = 1f;
				}
				if (effectIntensity.HasValue)
				{
					brightnessTween = TweenVFX(brightnessTween, () => bri.Brightness, (float x) =>
					{
						bri.Brightness = x;
					}, effectIntensity.Value);
				}
				break;
			}
			case RDThemeFX.Contrast:
			{
				CameraFilterPack_Color_BrightContrastSaturation con = camera.GetOrAddComponent<CameraFilterPack_Color_BrightContrastSaturation>();
				con.enabled = true;
				usingContrast = true;
				if (flag)
				{
					con.Contrast = 1f;
				}
				if (effectIntensity.HasValue)
				{
					contrastTween = TweenVFX(contrastTween, () => con.Contrast, (float x) =>
					{
						con.Contrast = x;
					}, effectIntensity.Value);
				}
				break;
			}
			case RDThemeFX.Saturation:
			{
				CameraFilterPack_Color_BrightContrastSaturation sat = camera.GetOrAddComponent<CameraFilterPack_Color_BrightContrastSaturation>();
				sat.enabled = true;
				usingSaturation = true;
				if (flag)
				{
					sat.Saturation = 1f;
				}
				if (effectIntensity.HasValue)
				{
					saturationTween = TweenVFX(saturationTween, () => sat.Saturation, (float x) =>
					{
						sat.Saturation = x;
					}, effectIntensity.Value);
				}
				break;
			}
			case RDThemeFX.Fisheye:
			{
				CameraFilterPack_Distortion_FishEye fish = camera.GetOrAddComponent<CameraFilterPack_Distortion_FishEye>();
				if (flag)
				{
					fish.Distortion = 0.5f;
				}
				float? num = effectIntensity * 0.5f + 0.5f;
				fish.enabled = true;
				if (effectIntensity.HasValue)
				{
					fisheyeTween = TweenVFX(fisheyeTween, () => fish.Distortion, (float x) =>
					{
						fish.ChangeDistortion = x;
						fish.Distortion = x;
					}, num.Value);
				}
				break;
			}
			}
		}
		else
		{
			base.game.rooms[room].AddThemeFX(fx, floatX, floatY, effectIntensity, extra, extra2, duration, ease, color);
		}
		Tween TweenVFX(Tween tween, DOGetter<float> get, DOSetter<float> set, float targetVal)
		{
			return TweenThemeFX(tween, get, set, targetVal, duration, ease, useTweens);
		}
	}

	public void DisableThemeFX(RDThemeFX fx, int room = 0)
	{
		usedThemeFX.Remove(fx);
		if (room == -1)
		{
			RDCamera camera = RDClass.Vfx.GetCamera(-1);
			switch (fx)
			{
			case RDThemeFX.Noise:
				RDClass.Vfx.fxNoise.visible = false;
				break;
			case RDThemeFX.GlitchObstruction:
				RDClass.Vfx.fxGlitch.visible = false;
				break;
			case RDThemeFX.Tile2:
			case RDThemeFX.Tile3:
			case RDThemeFX.Tile4:
			case RDThemeFX.TileN:
				RDClass.Vfx.SetTileScreen(new Vector2(1f, 1f));
				break;
			case RDThemeFX.BlackAndWhite:
				camera.GetComponent<Grayscale>().enabled = false;
				break;
			case RDThemeFX.Sepia:
				camera.GetComponent<CameraFilterPack_Color_Sepia>().enabled = false;
				break;
			case RDThemeFX.HallOfMirrors:
				camera.GetComponent<Camera>().clearFlags = CameraClearFlags.Color;
				break;
			case RDThemeFX.HueShift:
				camera.GetOrAddComponent<RDHueShiftDupe>().enabled = false;
				break;
			case RDThemeFX.Brightness:
				camera.GetOrAddComponent<CameraFilterPack_Color_BrightContrastSaturation>().Brightness = 1f;
				usingBrightness = false;
				TryDisableBrightnessContrastSaturation();
				break;
			case RDThemeFX.Contrast:
				camera.GetOrAddComponent<CameraFilterPack_Color_BrightContrastSaturation>().Contrast = 1f;
				usingContrast = false;
				TryDisableBrightnessContrastSaturation();
				break;
			case RDThemeFX.Saturation:
				camera.GetOrAddComponent<CameraFilterPack_Color_BrightContrastSaturation>().Saturation = 1f;
				usingSaturation = false;
				TryDisableBrightnessContrastSaturation();
				break;
			case RDThemeFX.Fisheye:
				camera.GetOrAddComponent<CameraFilterPack_Distortion_FishEye>().enabled = false;
				break;
			}
		}
		else
		{
			base.game.rooms[room].DisableThemeFX(fx);
		}
	}

	private void TryDisableBrightnessContrastSaturation()
	{
		if (!usingBrightness && !usingContrast && !usingSaturation)
		{
			RDClass.Vfx.GetCamera(-1).GetOrAddComponent<CameraFilterPack_Color_BrightContrastSaturation>().enabled = false;
		}
	}

	public void SetRankDescriptionsFromLocalizationData(string levelName)
	{
		RDString.GetWithCheck(levelName + ".F", out var exists);
		if (!exists)
		{
			printe("description not found for level: " + levelName);
			return;
		}
		rankDescriptions = new string[6]
		{
			RDString.Get(levelName + ".F"),
			RDString.Get(levelName + ".D"),
			RDString.Get(levelName + ".C"),
			RDString.Get(levelName + ".B"),
			RDString.Get(levelName + ".A"),
			RDString.Get(levelName + ".S")
		};
	}

	public void OffBeat(int i)
	{
		base.game.OffBeat(i);
		if (GC.twoPlayerMode)
		{
			base.game.OffBeat(i + base.game.shadowIdBeatOffset);
		}
	}

	public void OnBeat(int id)
	{
		base.game.beatAlarm[id] = true;
		if (GC.twoPlayerMode)
		{
			base.game.beatAlarm[id + base.game.shadowIdBeatOffset] = true;
		}
	}

	public void AddBeat(int rowID, float startBeat, float tick = 999f, SwingBeatType beatType = SwingBeatType.Normal, float customSwing = 0f, float holdTime = 0f)
	{
		base.game.AddBeat(rowID, startBeat, tick, beatType, customSwing, holdTime);
		if (GC.twoPlayerMode && rows[rowID].shadowOrHostID != -1)
		{
			base.game.AddBeat(rows[rowID].shadowOrHostID, startBeat, tick, beatType, customSwing, holdTime);
		}
	}

	public (BeatOneshot mainBeat, BeatOneshot shadowBeat) AddBeatOneshot(int rowID, float startBeat, float tick = 999f, OneshotPulseType pulseType = OneshotPulseType.Wave, string customHitSound = "", float delay = 0f, float delayBeatOffset = 0f, float visualFreezeBeatOverride = 0f, int subdivCount = 1, int subdivIndex = 0, bool isBurnshot = false, float holdDuration = 0f, float holdCueOffset = 0f)
	{
		BeatOneshot item = base.game.AddBeatOneshot(rowID, startBeat, tick, pulseType, customHitSound, delay, delayBeatOffset, visualFreezeBeatOverride, subdivCount, subdivIndex, isBurnshot, holdDuration, holdCueOffset);
		BeatOneshot item2 = null;
		if (GC.twoPlayerMode && base.game.shadowRowStyle == ShadowRowStyle.HostToShadowIf2P && rows[rowID].shadowOrHostID != -1)
		{
			item2 = base.game.AddBeatOneshot(rows[rowID].shadowOrHostID, startBeat, tick, pulseType, customHitSound, delay, delayBeatOffset, visualFreezeBeatOverride, subdivCount, subdivIndex, isBurnshot, holdDuration, holdCueOffset);
		}
		return (mainBeat: item, shadowBeat: item2);
	}

	public void OnBeatClassic(int i, int rowID, float startBeat, float tick = 999f, int cycleFreq = 1, SwingBeatType beatType = SwingBeatType.Normal, bool offOtherBeats = false, float customSwing = 0f)
	{
		base.game.OnBeatClassic(i, rowID, startBeat, tick, cycleFreq, beatType, offOtherBeats, customSwing);
		if (GC.twoPlayerMode && base.game.shadowRowStyle == ShadowRowStyle.HostToShadowIf2P && rows[rowID].shadowOrHostID != -1)
		{
			base.game.OnBeatClassic(i + base.game.shadowIdBeatOffset, rows[rowID].shadowOrHostID, startBeat, tick, cycleFreq, beatType, offOtherBeats, customSwing);
		}
	}

	public void SongVol(string name, float volume = 1f, float duration = 1f)
	{
		string audioName = "Audio Source (" + name + ")";
		GameObject[] array = (from obj in Resources.FindObjectsOfTypeAll<GameObject>()
			where obj.name == audioName
			select obj).ToArray();
		if (array.Length == 0)
		{
			printe("AudioSource " + audioName + " GameObject not found");
			return;
		}
		GameObject[] array2 = array;
		for (int num = 0; num < array2.Length; num++)
		{
			if (array2[num].TryGetComponent<AudioSource>(out var component))
			{
				if (component == base.conductor.currentSong)
				{
					base.conductor.FadeSong(volume, duration);
				}
				else
				{
					component.DOFade(volume * scrConductor.GlobalSongVolume, duration);
				}
			}
		}
	}

	[ListedMethod(true)]
	public void CurrentSongVol(float volume = 1f, float duration = 1f)
	{
		if (base.conductor.currentSong != null)
		{
			RDClassDll.printem("current song audioSource clip: " + base.conductor.currentSong.clip.name);
			if (volume <= 0f)
			{
				StopSong(duration);
			}
			else
			{
				base.conductor.FadeSong(volume, duration);
			}
		}
	}

	public void PreviousSongVol(float volume = 1f, float duration = 1f)
	{
		if (base.conductor.previousSong != null)
		{
			RDClassDll.printem("previous song audioSource clip: " + base.conductor.previousSong.clip.name);
			base.conductor.previousSong.DOFade(volume * scrConductor.GlobalSongVolume, duration);
		}
		else
		{
			RDClassDll.printem("previous song not found!!");
		}
	}

	[ListedMethod(true)]
	public void StopSong(float duration)
	{
		base.conductor.StopSong(duration);
	}

	public void StopCurrentSong(float duration)
	{
		StopSong(duration);
	}

	public void StopAmbiences(float duration)
	{
		RDRoom[] rooms = base.game.rooms;
		for (int i = 0; i < rooms.Length; i++)
		{
			rooms[i].currentEnvironment?.StopAmbiences(duration);
		}
	}

	public void StartAmbiences(float duration)
	{
		RDRoom[] rooms = base.game.rooms;
		for (int i = 0; i < rooms.Length; i++)
		{
			rooms[i].currentEnvironment?.StartAmbiences(duration);
		}
	}

	[ListedMethod(true)]
	public void SetShadowRow(int target, int source)
	{
		Row row = rows[target];
		row.isShadow = true;
		row.isShadowRealtime = true;
		if (base.game.shadowRowStyle == ShadowRowStyle.HostToShadowIf2P)
		{
			rows[source].shadowOrHostID = target;
			rows[source].shadowOrHostIDRealtime = target;
		}
		else
		{
			row.shadowOrHostID = source;
			row.shadowOrHostIDRealtime = source;
		}
		if (base.editor != null)
		{
			base.editor.tabSection_rows.UpdateUI();
		}
	}

	[ListedMethod(true)]
	public void UnsetShadowRow(int target, int source)
	{
		Row row = rows[target];
		row.isShadow = false;
		row.isShadowRealtime = false;
		if (base.game.shadowRowStyle == ShadowRowStyle.HostToShadowIf2P)
		{
			rows[source].shadowOrHostID = -1;
			rows[source].shadowOrHostIDRealtime = -1;
		}
		else
		{
			row.shadowOrHostID = -1;
			row.shadowOrHostIDRealtime = -1;
		}
		if (base.editor != null)
		{
			base.editor.tabSection_rows.UpdateUI();
		}
	}

	[ListedMethod(true)]
	public void SetClumsyRow(int target, int clumsiness)
	{
		scrPlayerbox playerBox = rows[target].playerBox;
		playerBox.clumsyCPUTiming = clumsiness switch
		{
			-2 => -0.4f, 
			-1 => -0.2f, 
			0 => 0f, 
			1 => 0.08f, 
			2 => 0.2f, 
			_ => 0f, 
		};
	}

	public bool RowGetsBeat(int originalRow, int rowID, string tag)
	{
		Row row = base.game.rows[rowID];
		if (row == null)
		{
			return false;
		}
		bool flag = rowID == originalRow;
		bool flag2 = row.isShadow && row.shadowOrHostID == originalRow && row.copyBeatsWhenShadow;
		if (tag == null)
		{
			tag = "";
		}
		if (tag.Contains("[mimic]"))
		{
			bool flag3 = base.game.rows.Any((Row r) => r.isShadow && r.shadowOrHostID == originalRow && row.copyBeatsWhenShadow);
			if (!flag2)
			{
				if (flag)
				{
					return !flag3;
				}
				return false;
			}
			return true;
		}
		if (tag.Contains("[nomimic]"))
		{
			return flag;
		}
		return flag | flag2;
	}

	public void SetKaleidoscopeColor(int room, float r1, float g1, float b1, float r2, float g2, float b2)
	{
		RDKaleidoscope kaleidoscope = base.game.rooms[room].kaleidoscope;
		kaleidoscope.colorA = new Color(r1, g1, b1, 1f);
		kaleidoscope.colorB = new Color(r2, g2, b2, 1f);
	}

	public void SyncKaleidoscopes(int targetRoom, int otherRoom)
	{
		RDKaleidoscope kaleidoscope = base.game.rooms[targetRoom].kaleidoscope;
		RDKaleidoscope kaleidoscope2 = base.game.rooms[otherRoom].kaleidoscope;
		if (kaleidoscope != null && kaleidoscope.gameObject.activeSelf && kaleidoscope2 != null && kaleidoscope2.gameObject.activeSelf)
		{
			kaleidoscope.SyncSeed(kaleidoscope2);
		}
	}

	public void SetKaleidoscopeRepeat(int room, int repeat)
	{
		base.game.rooms[room].kaleidoscope.SetRepeat(repeat);
	}

	public void SetKaleidoscopeOffset(int room, float offset, float durationBeats, string easeStr)
	{
		Ease ease = Enum.Parse<Ease>(easeStr);
		base.game.rooms[room].kaleidoscope.SetOffset(offset, durationBeats, ease);
	}

	public void SetKaleidoscopeRoll(int room, float roll, float durationBeats, string easeStr)
	{
		Ease ease = Enum.Parse<Ease>(easeStr);
		base.game.rooms[room].kaleidoscope.SetRoll(roll, durationBeats, ease);
	}

	public void SetKaleidoscopeSymmetry(int room, bool on)
	{
		base.game.rooms[room].kaleidoscope.ToggleSymmetry(on);
	}

	public void SetWavyRowsSpeed(int room, float speed)
	{
		base.game.rooms[room].wavyRowsFrequency = speed;
	}

	public void SetWavyRowsAmplitude(int room, float amplitude)
	{
		base.game.rooms[room].wavyRowsAmplitude = amplitude;
	}

	public void ToggleHands(bool enableHand, bool instant)
	{
		HandAction action = ((!enableHand) ? HandAction.Hide : HandAction.Show);
		float duration = (instant ? 0f : 0.75f);
		base.game.handController0.rightArm.Toggle(action, duration, GC.twoPlayerMode);
		if (GC.twoPlayerMode)
		{
			base.game.handController0.leftArm.Toggle(action, duration, GC.twoPlayerMode);
		}
	}

	public void ShowHandsInRoom(int room, bool show, bool instant)
	{
		base.game.GetHandControllerFromInt(room).Show(show, instant);
	}

	private void SetHandToChar(int room, bool rightHand, string charName)
	{
		scrHandController scrHandController2 = room switch
		{
			1 => base.game.handController1, 
			2 => base.game.handController2, 
			3 => base.game.handController3, 
			4 => base.game.handControllerOnTop, 
			_ => base.game.handController0, 
		};
		RDArm rDArm = (rightHand ? scrHandController2.rightArm : scrHandController2.leftArm);
		Character toCharacter;
		if (Enum.TryParse<RDPlayer>(charName, out rDArm.player))
		{
			toCharacter = Character.Player;
		}
		else
		{
			rDArm.player = RDPlayer.CPU;
			toCharacter = Enum.Parse<Character>(charName);
		}
		rDArm.SetToCharacter(toCharacter);
	}

	public void SetHandToIan(int room, bool rightHand)
	{
		SetHandToChar(room, rightHand, "Ian");
	}

	public void SetHandToPaige(int room, bool rightHand)
	{
		SetHandToChar(room, rightHand, "Paige");
	}

	public void SetHandToPolitician(int room, bool rightHand)
	{
		SetHandToChar(room, rightHand, "Politician");
	}

	[ListedMethod(true)]
	public void SetHandToP1(int room, bool rightHand)
	{
		SetHandToChar(room, rightHand, "P1");
	}

	[ListedMethod(true)]
	public void SetHandToP2(int room, bool rightHand)
	{
		SetHandToChar(room, rightHand, "P2");
	}

	[ListedMethod(true)]
	public void SetHandToPlayer(int room, bool rightHand)
	{
		bool flag = GC.twoPlayerMode && base.game.currentLevel.arePlayerInputsSwapped;
		SetHandToChar(room, rightHand, (rightHand ^ flag) ? "P1" : "P2");
	}

	public void SetHandsBorderColor(int room, float r, float g, float b, float a, int outline)
	{
		scrHandController handControllerFromInt = base.game.GetHandControllerFromInt(room);
		RDArm leftArm = handControllerFromInt.leftArm;
		RDArm rightArm = handControllerFromInt.rightArm;
		IEnumerable<Entity> first = leftArm.allSprites.Except(leftArm.dontOutline);
		IEnumerable<Entity> second = rightArm.allSprites.Except(rightArm.dontOutline);
		IEnumerable<Entity> enumerable = first.Concat(second);
		Color color = new Color(r, g, b, a);
		foreach (Entity item in enumerable)
		{
			item.SetBorderColor(color, outline != 0);
		}
	}

	public void SetAllHandsBorderColor(float r, float g, float b, float a, int outline)
	{
		for (int i = 0; i < 5; i++)
		{
			SetHandsBorderColor(i, r, g, b, a, outline);
		}
	}

	public void SetHandsAdditiveColor(int room, Color color)
	{
		scrHandController handControllerFromInt = base.game.GetHandControllerFromInt(room);
		RDArm leftArm = handControllerFromInt.leftArm;
		RDArm rightArm = handControllerFromInt.rightArm;
		IEnumerable<Entity> first = leftArm.allSprites.Except(new Entity[1] { leftArm.buttonEffect });
		IEnumerable<Entity> second = rightArm.allSprites.Except(new Entity[1] { rightArm.buttonEffect });
		foreach (Entity item in first.Concat(second))
		{
			item.SetAdditiveColor(color);
		}
	}

	public void SetAllHandsAdditiveColor(Color color)
	{
		for (int i = 0; i < 4; i++)
		{
			SetHandsAdditiveColor(i, color);
		}
	}

	public void SetHandsOverlayColor(int room, Color color)
	{
		scrHandController handControllerFromInt = base.game.GetHandControllerFromInt(room);
		RDArm leftArm = handControllerFromInt.leftArm;
		RDArm rightArm = handControllerFromInt.rightArm;
		IEnumerable<Entity> first = leftArm.allSprites.Except(new Entity[1] { leftArm.buttonEffect });
		IEnumerable<Entity> second = rightArm.allSprites.Except(new Entity[1] { rightArm.buttonEffect });
		foreach (Entity item in first.Concat(second))
		{
			item.SetOverlayColor(color);
		}
	}

	public void SetAllHandsOverlayColor(Color color)
	{
		for (int i = 0; i < 4; i++)
		{
			SetHandsOverlayColor(i, color);
		}
	}

	public void TintHandsWithInts(int room, float r, float g, float b, float a)
	{
		SetHandsOverlayColor(room, new Color(r, g, b, a));
	}

	public void SetCPUHand(int roomIndex, bool rightHand)
	{
		base.game.GetHandControllerFromInt(roomIndex).rightArmIsCPU = rightHand;
	}

	public void LimitCPUHandRoomTo(int roomIndex, bool rightHand, bool leftHand)
	{
		for (int i = 0; i <= 4; i++)
		{
			bool flag = i == roomIndex;
			SetHandsCPUCanUse(i, flag & rightHand, flag & leftHand);
		}
	}

	public void SetHandsCPUCanUse(int roomIndex, bool rightHand, bool leftHand)
	{
		scrHandController handControllerFromInt = base.game.GetHandControllerFromInt(roomIndex);
		handControllerFromInt.leftArm.cpuCanUse = leftHand;
		handControllerFromInt.rightArm.cpuCanUse = rightHand;
	}

	public void SetHandsPlayersCanUse(int roomIndex, bool rightHand, bool leftHand)
	{
		scrHandController handControllerFromInt = base.game.GetHandControllerFromInt(roomIndex);
		handControllerFromInt.leftArm.playerCanUse = leftHand;
		handControllerFromInt.rightArm.playerCanUse = rightHand;
	}

	public void HideHandsInstant(int roomIndex)
	{
		scrHandController scrHandController2 = base.game.handController0;
		switch (roomIndex)
		{
		case 1:
			scrHandController2 = base.game.handController1;
			break;
		case 2:
			scrHandController2 = base.game.handController2;
			break;
		case 3:
			scrHandController2 = base.game.handController3;
			break;
		case 4:
			scrHandController2 = base.game.handControllerOnTop;
			break;
		}
		scrHandController2.HideInstant();
	}

	public void ShowPlayerHand(int room, bool p1, bool isShort, bool instant)
	{
		scrHandController handControllerFromInt = base.game.GetHandControllerFromInt(room);
		HandUpdateType handType = ((!p1) ? base.game.GetP2HandUpdateType(isShort) : base.game.GetP1HandUpdateType(isShort));
		handControllerFromInt.UpdateHandsPositions(handType, instant ? 0f : 1f, shakeOnSettle: false);
	}

	public void ShowShortHand(int room, bool rightHand)
	{
		scrHandController handControllerFromInt = base.game.GetHandControllerFromInt(room);
		if (rightHand)
		{
			handControllerFromInt.UpdateHandsPositions(HandUpdateType.RightHandShort, 1f, shakeOnSettle: false);
		}
		else
		{
			handControllerFromInt.UpdateHandsPositions(HandUpdateType.LeftHandShort, 1f, shakeOnSettle: false);
		}
	}

	public void AlignHandsWithRoom(int room, bool enable)
	{
		base.game.GetHandControllerFromInt(room).ToggleAlignWithRoomComp(enable);
	}

	public void SummonHands()
	{
		base.game.handController0.SlideIn();
	}

	private void GoToLevelWithWarning(string level, bool instantly = false)
	{
		TweenCallback callback = () =>
		{
			if (instantly)
			{
				scnGame.levelPersistence.Setup(level).startImmediately = true;
			}
			scnBase.GoToLevel(level);
		};
		if (base.editorMode)
		{
			if (!RDBase.isDev)
			{
				return;
			}
			base.editor.dialog.Show("GoToLevel() has been called.\n<color=red>Unsaved changes will be lost!</color>", new string[2]
			{
				"Go to " + level,
				"Cancel"
			}, (int selectedOption) =>
			{
				if (selectedOption == 0)
				{
					callback();
				}
				else
				{
					base.editor.dialog.Hide();
				}
			});
			if (!base.conductor.isScrubbing)
			{
				scnEditor.PauseIfUnpaused();
			}
		}
		else
		{
			callback();
		}
	}

	public void GoToLevelInstantly(string level)
	{
		GoToLevelWithWarning(level, instantly: true);
	}

	public void GoToLevel(string level)
	{
		RDClass.Vfx.FadeOut();
		FadeOutMusic();
		Timer.Add(() =>
		{
			GoToLevelWithWarning(level);
		}, 1f);
	}

	public object GetVariable(string str)
	{
		int num = str.TakeWhile((char c) => c == '!').Count();
		str = str.Remove(0, num);
		bool flag = num % 2 == 1;
		if (str == "true")
		{
			return !flag;
		}
		if (str == "false")
		{
			return flag;
		}
		Type typeFromHandle = typeof(LevelBase);
		object obj = typeFromHandle.GetField(str)?.GetValue(this) ?? typeFromHandle.GetProperty(str)?.GetValue(this);
		if ((obj is bool) & flag)
		{
			obj = !(bool)obj;
		}
		return obj;
	}

	public string EvaluateCurlyBracketsInString(string str)
	{
		return Regex.Replace(str, "\\{([^]]+?)\\}", (Match match) =>
		{
			string value = match.Value;
			return EvalStringWithVariables(value.Substring(1, value.Length - 1 - 1)).ToString();
		});
	}

	public object EvalStringWithVariables(string str)
	{
		Dictionary<string, string> variableCache = new Dictionary<string, string>();
		string str2 = ReplaceVarsWithValues(str, variableCache, out var hasRandWrapper);
		object obj = EvalString(str2);
		if (hasRandWrapper)
		{
			obj = UnityEngine.Random.Range(1, (int)obj + 1);
		}
		return obj;
	}

	public object EvalString(string str)
	{
		return dt.Compute(str, "");
	}

	private string RemoveRand(string str)
	{
		if (!str.Contains("Rand(") || str.IndexOf("Rand(") != 0)
		{
			return null;
		}
		str = str.Remove(0, 5);
		str = str.TrimEnd(')', ' ');
		return str;
	}

	public string ReplaceVarsWithValues(string str, Dictionary<string, string> variableCache, out bool hasRandWrapper)
	{
		hasRandWrapper = false;
		string text = RemoveRand(str);
		if (text != null)
		{
			hasRandWrapper = true;
			str = text;
		}
		return Regex.Replace(str, "!*[a-zA-Z][a-zA-Z0-9]+", (Match match) =>
		{
			string value = match.Value;
			switch (value)
			{
			case "And":
			case "Or":
			case "Not":
			case "IIf":
				return value;
			default:
			{
				if (!variableCache.TryGetValue(value, out var value2))
				{
					value2 = GetVariable(value).ToString();
					variableCache[value] = value2;
				}
				return value2;
			}
			}
		});
	}

	public void SetBPM(float bpm)
	{
		base.conductor.SetBPM(bpm);
	}

	[ListedMethod(true)]
	public void Mistake()
	{
		MistakeOrHealGeneric(1f, RDPlayer.BOTH, "MistakesParent");
	}

	[ListedMethod(true)]
	public void MistakeSilent()
	{
		MistakeOrHealGeneric(1f, RDPlayer.BOTH);
	}

	[ListedMethod(true)]
	public void MistakeOrHeal(float weight = 1f)
	{
		MistakeOrHealGeneric(weight, RDPlayer.BOTH, "MistakesParent");
	}

	[ListedMethod(true)]
	public void MistakeOrHealP1(float weight = 1f)
	{
		MistakeOrHealGeneric(weight, RDPlayer.P1, "PlayerOneMistakes");
	}

	[ListedMethod(true)]
	public void MistakeOrHealP2(float weight = 1f)
	{
		MistakeOrHealGeneric(weight, RDPlayer.P2, "PlayerOneMistakes");
	}

	[ListedMethod(true)]
	public void MistakeOrHealSilent(float weight = 1f)
	{
		MistakeOrHealGeneric(weight, RDPlayer.BOTH);
	}

	[ListedMethod(true)]
	public void MistakeOrHealP1Silent(float weight = 1f)
	{
		MistakeOrHealGeneric(weight, RDPlayer.P1);
	}

	[ListedMethod(true)]
	public void MistakeOrHealP2Silent(float weight = 1f)
	{
		MistakeOrHealGeneric(weight, RDPlayer.P2);
	}

	private void MistakeOrHealGeneric(float weight, RDPlayer player, string soundMixerPath = null)
	{
		if (soundMixerPath != null || weight > 0f)
		{
			base.game.FlashBorderFeedback(weight < 0f);
		}
		if (soundMixerPath != null && weight >= 0f)
		{
			scrConductor.PlayFeedback(GameSoundType.SmallMistake, 1f, RDUtils.GetMixerGroup(soundMixerPath));
		}
		base.game.OnMistakeOrHeal(0f, weight, null, removeMistakes: true, player);
	}

	public void DamageHeart(int row, float weight = 1f)
	{
		if (rows[row] != null && !(weight < 0f))
		{
			base.game.rows[row].ent.CrackAdvance(weight);
		}
	}

	public void HealHeart(int row, float weight = 1f)
	{
		if (rows[row] != null && !(weight < 0f))
		{
			base.game.rows[row].ent.Heal(weight);
		}
	}

	[ListedMethod(true)]
	public void Hit()
	{
		base.game.FlashBorderFeedback(correct: true);
	}

	[ListedMethod(true)]
	public void HitIncorrect()
	{
		base.game.FlashBorderFeedback(correct: true);
	}

	[ListedMethod(true)]
	public void ResetHitHistory()
	{
		List<scnGame.HitInfo>[] rowsHitOffsets = base.game.rowsHitOffsets;
		for (int i = 0; i < rowsHitOffsets.Length; i++)
		{
			rowsHitOffsets[i].Clear();
		}
		base.game.allHitOffsets.Clear();
		base.game.mistakesManager.ClearMistakes();
	}

	[ListedMethod(true)]
	public void ResetHitHistoryP1()
	{
		List<scnGame.HitInfo>[] rowsHitOffsets = base.game.rowsHitOffsets;
		for (int i = 0; i < rowsHitOffsets.Length; i++)
		{
			rowsHitOffsets[i].Clear();
		}
		base.game.allHitOffsets.Clear();
		base.game.mistakesManager.ClearMistakes(RDPlayer.P1);
	}

	[ListedMethod(true)]
	public void ResetHitHistoryP2()
	{
		List<scnGame.HitInfo>[] rowsHitOffsets = base.game.rowsHitOffsets;
		for (int i = 0; i < rowsHitOffsets.Length; i++)
		{
			rowsHitOffsets[i].Clear();
		}
		base.game.allHitOffsets.Clear();
		base.game.mistakesManager.ClearMistakes(RDPlayer.P2);
	}

	public void ClearMistakes()
	{
		ResetHitHistory();
	}

	public void ClearMistakesP1()
	{
		ResetHitHistoryP1();
	}

	public void ClearMistakesP2()
	{
		ResetHitHistoryP2();
	}

	public void SetCutsceneSkippable(bool cutscene)
	{
		if (cutscene)
		{
			base.game.gameState = GameState.CutsceneSkippable;
		}
		else
		{
			base.game.gameState = GameState.Handmode;
		}
	}

	public void SetSkippableLabelAnchor(string anchorStr)
	{
		rankscreen.SetSkippableLabelAnchor(Enum.Parse<TextAnchor>(anchorStr));
	}

	public void SetSkippableLabelAnchorCustom(string anchorStr, float x, float y)
	{
		rankscreen.SetSkippableLabelAnchor(Enum.Parse<TextAnchor>(anchorStr), new Vector2(x, y));
	}

	public void SetSkippableLabelRoom(int room)
	{
		rankscreen.skippableTextRoom = room;
	}

	[ListedMethod(true)]
	public void OneSongAtATime(bool enabled)
	{
		base.conductor.oneSongAtATime = enabled;
	}

	public void SetNextBarRelative(int barNumber)
	{
		if (!base.conductor.isScrubbingRefreshOnBar)
		{
			base.conductor.barNumberToGo = Mathf.Max(base.conductor.barNumberUpdateOnBar + barNumber, 1);
			if (scnEditor.instance != null)
			{
				scnEditor.instance.startBar = Mathf.Min(scnEditor.instance.startBar, base.conductor.barNumberToGo);
			}
			base.conductor.SetPlayStyle(PlayStyleChange.OnNextBar, forceFix: true);
		}
	}

	public void SetNextBar(int barNumber)
	{
		if (!base.conductor.isScrubbingRefreshOnBar)
		{
			base.conductor.barNumberToGo = Mathf.Max(barNumber, 1);
			if (scnEditor.instance != null)
			{
				scnEditor.instance.startBar = Mathf.Min(scnEditor.instance.startBar, base.conductor.barNumberToGo);
			}
			base.conductor.SetPlayStyle(PlayStyleChange.OnNextBar, forceFix: true);
		}
	}

	public void SetNextBarImmediately(int barNumber)
	{
		if (!base.conductor.isScrubbingRefreshOnBar)
		{
			base.conductor.barNumberToGo = barNumber;
			base.conductor.SetPlayStyle(PlayStyleChange.Immediately, forceFix: true);
		}
	}

	public void SetNextBarExtraImmediately(int barNumber)
	{
		if (!base.conductor.isScrubbingRefreshOnBar)
		{
			base.conductor.barNumberToGo = barNumber;
			base.conductor.SetPlayStyle(PlayStyleChange.ExtraImmediately, forceFix: true);
		}
	}

	public void SetPlayStyle(string changeStylestr)
	{
		PlayStyleChange changeStyle = RDEditorUtils.DecodeEnum<PlayStyleChange>(changeStylestr);
		base.conductor.SetPlayStyle(changeStyle);
	}

	public void EnableRowReflections(int room = -1)
	{
		if (rowReflections)
		{
			return;
		}
		int num = 0;
		Row[] array = base.game.rows;
		foreach (Row row in array)
		{
			if (row.ent != null && (row.room == room || room == -1))
			{
				row.ToggleReflections(enable: true);
			}
			num++;
		}
	}

	public void ToggleSingleRowReflections(int room, int rowIndex, bool enable)
	{
		if (rowReflections)
		{
			return;
		}
		List<Row> list = new List<Row>();
		Row[] array = base.game.rows;
		foreach (Row row in array)
		{
			if (row.ent != null && (row.room == room || room == -1))
			{
				list.Add(row);
			}
		}
		if (rowIndex < list.Count)
		{
			list[rowIndex].ToggleReflections(enable);
		}
	}

	public void DisableRowReflections(int room = -1)
	{
		if (rowReflections)
		{
			return;
		}
		Row[] array = base.game.rows;
		foreach (Row row in array)
		{
			if (row.ent != null && (row.room == room || room == -1) && row.reflection != null)
			{
				row.ToggleReflections(enable: false);
			}
		}
	}

	[ListedMethod(true)]
	public void ToggleRowReflection(int row, bool enabled)
	{
		base.game.rows[row].ToggleReflections(enabled);
	}

	[ListedMethod(true)]
	public void ToggleRowReflectionRoom(int room, bool enabled)
	{
		if (enabled)
		{
			EnableRowReflections(room);
		}
		else
		{
			DisableRowReflections(room);
		}
	}

	public void ToggleRowReflectionJumpOverride(int row, bool canJump)
	{
		List<int> list = base.game.currentLevel.rowReflectionJumpOverrides;
		if (canJump)
		{
			if (!list.Contains(row))
			{
				list.Add(row);
			}
		}
		else if (list.Contains(row))
		{
			list.Remove(row);
		}
	}

	[ListedMethod(true)]
	public void ToggleRowHitFX(int row, bool on)
	{
		base.game.rows[row].ent.hideHitFX = !on;
	}

	[ListedMethod(true)]
	public void TweenRowPulseBend(int row, int pulse, float amount, float durationBeats, string easeStr)
	{
		Ease ease = Enum.Parse<Ease>(easeStr);
		float duration = base.conductor.DurationBeatsToTime(durationBeats);
		base.game.rows[row].beatboxes[pulse].TweenBend(amount, duration, ease);
	}

	[ListedMethod(true)]
	public void TweenOneshotPositionOverride(int row, float overridePosition, float durationBeats, string easeStr)
	{
		Ease ease = Enum.Parse<Ease>(easeStr);
		float dur = base.conductor.DurationBeatsToTime(durationBeats);
		base.game.rows[row].ent.oneshotRowController.waveManager.SetPositionOverride(overridePosition, dur, ease);
	}

	[ListedMethod(true)]
	public void TweenOneshotPositionOverrideLerp(int row, float overridePercent, float durationBeats, string easeStr)
	{
		Ease ease = Enum.Parse<Ease>(easeStr);
		float dur = base.conductor.DurationBeatsToTime(durationBeats);
		base.game.rows[row].ent.oneshotRowController.waveManager.SetPositionOverrideLerp(overridePercent, dur, ease);
	}

	[ListedMethod(true)]
	public void TweenFloat(int floatID, float val, float durationBeats, string easeStr)
	{
		if (floatID < 0 || floatID > 9)
		{
			return;
		}
		float duration = base.conductor.DurationBeatsToTime(durationBeats);
		Ease ease = Enum.Parse<Ease>(easeStr);
		FieldInfo f = typeof(LevelBase).GetField($"f{floatID}");
		DOTween.Kill(f);
		DOTween.To(() => (float)f.GetValue(this), (float x) =>
		{
			f.SetValue(this, x);
		}, val, duration).OnUpdate(() =>
		{
			if (inEditor)
			{
				scnEditor.instance.ipm.UpdateBlankPanel(onlyIfVisible: true);
			}
		}).SetEase(ease)
			.SetId(f)
			.OnComplete(() =>
			{
				f.SetValue(this, val);
			});
	}

	public void TweenHitMarginMultiplier(float newMultiplier, float durationBeats, string easeStr)
	{
		float duration = base.conductor.DurationBeatsToTime(durationBeats);
		Ease ease = Enum.Parse<Ease>(easeStr);
		DOTween.To(() => hitMarginMultiplier, (float x) =>
		{
			hitMarginMultiplier = x;
		}, newMultiplier, duration).SetEase(ease).OnComplete(() =>
		{
			hitMarginMultiplier = newMultiplier;
		});
	}

	[ListedMethod(true)]
	public void StopEverything()
	{
		base.game.FailLevelLite();
	}

	[ListedMethod(true)]
	public void ShowSpotlight(int row, bool playAudio)
	{
		RDClass.Vfx.SpotlightFocusFromCenter(rows[row].ent.character.transform.position.x, 0.5f, playAudio && !base.conductor.isScrubbing, RDUtils.GetMixerGroup("FeedbackSoundsParent"));
	}

	[ListedMethod(true)]
	public void ExpandSpotlight(float durationBeats)
	{
		RDClass.Vfx.SpotlightOpenAllTheWay(base.conductor.DurationBeatsToTime(durationBeats), playSwitchSound: false);
	}

	[ListedMethod(true)]
	public void HideSpotlight()
	{
		RDClass.Vfx.SpotlightInstantOff(darkenScene: false, playSwitchSound: false, noSound: true);
	}

	public void SetForceTrueHeldOffset(int row, bool forceOn)
	{
		base.game.rows[row].playerBox.forceTrueOffset = forceOn;
	}

	public void ForceHeldBeatRelease(int row)
	{
		base.game.rows[row].playerBox.SpaceBarReleased(RDPlayer.P1, cpuTriggered: true);
		base.game.rows[row].playerBox.SpaceBarReleased(RDPlayer.P2, cpuTriggered: true);
	}

	public void ForceHeldBeatHold(int row)
	{
		base.game.rows[row].playerBox.holdAssist = true;
	}

	public void UnforceHeldBeatHold(int row)
	{
		base.game.rows[row].playerBox.holdAssist = false;
		base.game.rows[row].playerBox.SpaceBarReleased(RDPlayer.P1, cpuTriggered: true);
		base.game.rows[row].playerBox.SpaceBarReleased(RDPlayer.P2, cpuTriggered: true);
	}

	[ListedMethod(true)]
	public void StopAllBeats()
	{
		base.game.StopBeats();
	}

	public void ChangeCharacter(string character, int rowID)
	{
		base.game.rows[rowID].ent.ChangeCharacter((Character)Enum.Parse(typeof(Character), character));
	}

	public void ChangeCharacterSmooth(string character, int rowID)
	{
		base.game.rows[rowID].ent.ChangeCharacter((Character)Enum.Parse(typeof(Character), character), showParticles: true);
	}

	public void ChangeCharacterCustom(string cc, int rowID, bool smooth)
	{
		base.game.rows[rowID].ent.ChangeCharacterCustom(cc, smooth);
	}

	[ListedMethod(true)]
	public void CrackAllHearts()
	{
		Row[] array = base.game.rows;
		foreach (Row row in array)
		{
			if ((bool)row?.ent?.heart)
			{
				row.ent.heart.DoFinalCrack();
				RDUtils.Rotate2D(row.ent.heart.transform, 0f);
			}
		}
	}

	[ListedMethod(true)]
	public void UpdateAllHearts()
	{
		Row[] array = base.game.rows;
		foreach (Row row in array)
		{
			if ((bool)row?.ent?.heart)
			{
				row.ent.heart.SetMisses(numMistakes);
			}
		}
	}

	[ListedMethod(true)]
	public void SetHeartMistakes(int rowID, float mistakes)
	{
		scrHeart scrHeart2 = base.game.rows[rowID]?.ent?.heart;
		if ((bool)scrHeart2)
		{
			scrHeart2.ChangeSprite("Heart");
			scrHeart2.SetMisses(mistakes);
		}
	}

	[ListedMethod(true)]
	public void ShakeRow(int rowID, int amount, float duration)
	{
		RowEntity rowEntity = base.game.rows[rowID]?.ent;
		if (rowEntity != null)
		{
			if (Math.Abs(amount) > 25)
			{
				amount = 25;
			}
			float duration2 = base.conductor.DurationBeatsToTime(duration);
			rowEntity.heart.Shake(duration2, amount, amount);
			rowEntity.character.Shake(duration2, amount, amount);
		}
	}

	public void ShakeSprite(string target, float amp, int freq, float secs, bool fadeOut)
	{
		if (base.game.currentLevel.sprites.ContainsKey(target))
		{
			base.game.currentLevel.sprites[target].transform.DOShakePosition(secs, new Vector3(amp, amp, 0f), freq, 90f, snapping: false, fadeOut);
		}
	}

	public void InfectHeart(int rowID, bool instant)
	{
		scrHeart scrHeart2 = base.game.rows[rowID]?.ent?.heart;
		if ((bool)scrHeart2)
		{
			if (instant)
			{
				scrHeart2.QuickInfect();
			}
			else
			{
				scrHeart2.InfectAnimation();
			}
		}
	}

	private void ChangeHeartSprite(int rowID, string sprite)
	{
		base.game.rows[rowID]?.ent?.heart?.ChangeSprite(sprite);
	}

	public void SetHeartCracked(int rowID)
	{
		ChangeHeartSprite(rowID, "Heart_Joined");
	}

	public void SetHeartCrackedLeft(int rowID)
	{
		ChangeHeartSprite(rowID, "Heart_Left");
	}

	public void SetHeartCrackedRight(int rowID)
	{
		ChangeHeartSprite(rowID, "Heart_Right");
	}

	public void SetHeartUnbeatable(int rowID)
	{
		ChangeHeartSprite(rowID, "HeartUnbeatable");
	}

	public void SetHeartPumpkin(int rowID)
	{
		ChangeHeartSprite(rowID, "HeartPumpkin");
	}

	public void ResetHeart(int rowID)
	{
		base.game.rows[rowID]?.ent?.heart?.Reset();
	}

	public void FadeLyric(int id, float alpha, float duration)
	{
		if (scrVfxControl.instance.allVfxLyrics.TryGetValueAs<int, LyricsGame, LyricsGame>(id, out var valueAs))
		{
			float duration2 = base.conductor.DurationBeatsToTime(duration);
			valueAs.Fade(alpha, duration2);
		}
	}

	public void MoveLyric(int id, float x, float y)
	{
		if (scrVfxControl.instance.allVfxLyrics.TryGetValueAs<int, LyricsGame, LyricsGame>(id, out var valueAs))
		{
			valueAs.GetComponent<RectTransform>().anchoredPosition3D = new Vector2(x, y);
		}
	}

	[ListedMethod(true)]
	public void IgnoreInput(bool disable)
	{
		base.game.ignoreInput = disable;
	}

	public void ShowHealth(bool show, float beats)
	{
		HPBarController hpController = base.game.hpController;
		hpController.settingVisibilityManually = true;
		hpController.Show(show, beats);
	}

	public void ShowVirusBar(bool show)
	{
		base.game.hpController.ShowVirus(show);
	}

	public void ShowMarginError(bool show)
	{
		LEDSign.showMarginError = show;
	}

	public void SetWindowDanceTimeShift(int room, float shiftBeats, float easeBeats, string easeStr)
	{
		if (base.game.windowChoreographer != null)
		{
			float shift = base.conductor.DurationBeatsToTime(base.conductor.currentBeat, Mathf.Abs(shiftBeats)) * Mathf.Sign(shiftBeats);
			float easeDur = base.conductor.DurationBeatsToTime(base.conductor.currentBeat, easeBeats);
			Ease ease = Enum.Parse<Ease>(easeStr);
			base.game.windowChoreographer.dancers[room].SetTimeShift(shift, easeDur, ease);
		}
	}

	public void ToggleWindowDanceWrap(int room, bool vertical, bool wrap)
	{
		if (base.game.windowChoreographer != null)
		{
			WindowDancer windowDancer = base.game.windowChoreographer.dancers[room];
			if (vertical)
			{
				windowDancer.forceNoYWrap = !wrap;
			}
			else
			{
				windowDancer.forceNoXWrap = !wrap;
			}
		}
	}

	public void PlayDialogueBasedOnTries(string baseKnotName, int[] tryVariantInfo)
	{
		if (tryVariantInfo.Length == 1)
		{
			RunDialogueWithVariations(baseKnotName, tryVariantInfo[0], gameOver: false);
			return;
		}
		Type type = GetType();
		int num = RDStartup.levelTries[type];
		int variations = tryVariantInfo[0];
		int num2 = tryVariantInfo[^1];
		if (num >= tryVariantInfo.Length)
		{
			if (num2 == -1)
			{
				RunDialogueWithVariations(baseKnotName + "_OnTry_1", variations, gameOver: false);
			}
			else
			{
				RunDialogueWithVariations(baseKnotName + "_OnTry_Max", num2, gameOver: false);
			}
			return;
		}
		int num3 = tryVariantInfo[num - 1];
		switch (num3)
		{
		case -1:
			RunDialogueWithVariations(baseKnotName + "_OnTry_1", variations, gameOver: false);
			break;
		case -2:
			RunDialogueWithVariations(baseKnotName + "_OnTry_Max", num2, gameOver: false);
			break;
		default:
			RunDialogueWithVariations(baseKnotName + "_OnTry_" + num, num3, gameOver: false);
			break;
		}
	}

	[ListedMethod(true)]
	public void SetRowLength(int row, int length)
	{
		base.game.rows[row].ent.SetRowLength(Math.Clamp(length, 1, 7), 1f);
	}

	[ListedMethod(true)]
	public void SetRowLengthTimed(int row, int length, float durationBeats)
	{
		base.game.rows[row].ent.SetRowLength(Math.Clamp(length, 1, 7), base.conductor.DurationBeatsToTime(durationBeats));
	}

	public void PlayGameOverDialogue(List<GameOverBarInfo> gameOverBars, GameOverBarInfo genericGameOverInfo, string dialoguePrefix = "GameOver")
	{
		Type type = base.game.currentLevel.GetType();
		Dictionary<int, int> dictionary = RDStartup.levelDeathsAtBars[type];
		List<int> list = new List<int>();
		foreach (GameOverBarInfo gameOverBar in gameOverBars)
		{
			int bar = gameOverBar.bar;
			if (scnGame.lastBarDiedAt >= bar)
			{
				list.Add(bar);
			}
		}
		bool flag = false;
		if (!list.Any())
		{
			flag = true;
		}
		else
		{
			int highestBarTriggered = list.Max();
			GameOverBarInfo gameOverBarInfo = gameOverBars.Find((GameOverBarInfo gobi) => gobi.bar == highestBarTriggered);
			if (gameOverBarInfo.deathVariantInfo.Length == 1)
			{
				if (gameOverBarInfo.deathVariantInfo[0] == -1)
				{
					flag = true;
				}
				else
				{
					RunDialogueWithVariations(dialoguePrefix + "_AfterBar_" + highestBarTriggered, gameOverBarInfo.deathVariantInfo[0], gameOver: true);
				}
			}
			else
			{
				int num = 0;
				if (highestBarTriggered == gameOverBars.Last().bar)
				{
					foreach (int item in dictionary.Keys.Where((int k) => k >= highestBarTriggered))
					{
						num += dictionary[item];
					}
				}
				else
				{
					int nextSpecialBar = gameOverBars.Find((GameOverBarInfo gobi) => gobi.bar > highestBarTriggered).bar;
					foreach (int item2 in dictionary.Keys.Where((int k) => k >= highestBarTriggered && k < nextSpecialBar))
					{
						num += dictionary[item2];
					}
				}
				int variations = gameOverBarInfo.deathVariantInfo[0];
				int num2 = gameOverBarInfo.deathVariantInfo[^1];
				if (num >= gameOverBarInfo.deathVariantInfo.Length)
				{
					if (num2 == -1)
					{
						RunDialogueWithVariations(dialoguePrefix + "_AfterBar_" + highestBarTriggered + "_WithDeaths_1", variations, gameOver: true);
					}
					else
					{
						RunDialogueWithVariations(dialoguePrefix + "_AfterBar_" + highestBarTriggered + "_WithDeaths_Max", num2, gameOver: true);
					}
				}
				else
				{
					int num3 = gameOverBarInfo.deathVariantInfo[num - 1];
					switch (num3)
					{
					case -1:
						RunDialogueWithVariations(dialoguePrefix + "_AfterBar_" + highestBarTriggered + "_WithDeaths_1", variations, gameOver: true);
						break;
					case -2:
						RunDialogueWithVariations(dialoguePrefix + "_AfterBar_" + highestBarTriggered + "_WithDeaths_Max", num2, gameOver: true);
						break;
					default:
						RunDialogueWithVariations(dialoguePrefix + "_AfterBar_" + highestBarTriggered + "_WithDeaths_" + num, num3, gameOver: true);
						break;
					}
				}
			}
		}
		if (!flag)
		{
			return;
		}
		if (genericGameOverInfo.deathVariantInfo.Length == 1)
		{
			RunDialogueWithVariations(dialoguePrefix, genericGameOverInfo.deathVariantInfo[0], gameOver: true);
			return;
		}
		int num4 = dictionary.Values.Sum();
		int variations2 = genericGameOverInfo.deathVariantInfo[0];
		int num5 = genericGameOverInfo.deathVariantInfo[^1];
		if (num4 >= genericGameOverInfo.deathVariantInfo.Length)
		{
			if (num5 == -1)
			{
				RunDialogueWithVariations(dialoguePrefix + "_WithDeaths_1", variations2, gameOver: true);
			}
			else
			{
				RunDialogueWithVariations(dialoguePrefix + "_WithDeaths_Max", num5, gameOver: true);
			}
			return;
		}
		int num6 = genericGameOverInfo.deathVariantInfo[num4 - 1];
		switch (num6)
		{
		case -1:
			RunDialogueWithVariations(dialoguePrefix + "_WithDeaths_1", variations2, gameOver: true);
			break;
		case -2:
			RunDialogueWithVariations(dialoguePrefix + "_WithDeaths_Max", num5, gameOver: true);
			break;
		default:
			RunDialogueWithVariations(dialoguePrefix + "_WithDeaths_" + num4, num6, gameOver: true);
			break;
		}
	}

	private void RunDialogueWithVariations(string baseKnotName, int variations, bool gameOver)
	{
		string text = baseKnotName;
		if (variations > 1)
		{
			text = text + "_Random_" + RXRandom.Range(1, variations + 1);
		}
		if (gameOver)
		{
			base.ink.Run(text, GameOverExit);
		}
		else
		{
			base.ink.Run(text);
		}
	}

	private void GameOverExit()
	{
		if (glitchObstructionOnGameOverExit)
		{
			Background obstruction = RDClass.Vfx.fxGlitch;
			scrExecuteOnCertainBeat.Add(() =>
			{
				obstruction.visible = true;
			}, -1f);
			DOVirtual.DelayedCall(base.conductor.crotchet, () =>
			{
				obstruction.visible = false;
				RDClass.Vfx.BlackScreenOn();
			});
			scrConductor.PlayImmediately("sndNoiseMed", 0.5f);
			rankscreen.ExitLevelAfterSeconds(0.3f);
		}
		else
		{
			rankscreen.ExitLevelAfterSeconds(1f);
		}
		if (dontKillWindowDanceOnGameOverUntilExit)
		{
			base.game.windowChoreographer?.Cancel();
		}
	}

	[ListedMethod(true)]
	public void StopDialogue()
	{
		base.ink.Stop();
	}

	[ListedMethod(true)]
	public void StopDialogueInstant()
	{
		base.ink.Stop(instant: true);
	}

	public void NarrateDescription(string descID)
	{
		if (!base.conductor.isScrubbing && !base.conductor.runningScrubToNextBar)
		{
			Narration.Say(RDString.Get(descID), NarrationCategory.Description, false);
		}
	}

	public void SkipLevel()
	{
		base.game.SkipLevel();
	}

	public void SetDialogueAnchor(float y)
	{
		base.ink.SetPanelAnchor(y);
	}

	public void SetDialogueRoom(int room)
	{
		base.ink.SetPanelRoom(room);
	}
}
