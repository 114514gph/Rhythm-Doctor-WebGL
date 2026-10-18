using System;
using DG.Tweening;
using Rewired;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "RDConstants", menuName = "RhythmDoctor/Constants", order = 1)]
public class RDConstants : ScriptableObject
{
	private static RDConstants internalData;

	[Header("Tweaks")]
	public bool forceNewWindowEnabled;

	[Header("Build")]
	public bool booth;

	public RDBool mobile;

	public RDBool nx;

	[Header("Custom Windows")]
	public Texture2D defaultCustomWindowIcon;

	[Header("Localization")]
	[Tooltip("Only works on editor")]
	public bool forceLanguage;

	[Tooltip("Only works on editor")]
	public SystemLanguage forcedLanguage;

	public bool retrieveEnglishOnly;

	public bool retrieveKoreanOnly;

	public bool retrieveChineseOnly;

	public bool compileOnlyLastImportedInk;

	[Header("General")]
	public RDSongOffsets soundData;

	public float customSpeed;

	public float volTrackerMultiplier;

	public InputManager rewiredManager;

	public Narration narration;

	[Header("Game")]
	public RDBool instantPulses;

	public bool playExplosionFileVariations;

	public bool playExplosionPitchVariations;

	public float blurDecayDuration;

	public float bladeRotationDuration;

	public Ease blurDecayEase;

	public Ease bladeRotationEase;

	public Ease bladeRotationCloseEase;

	public float bladeRotationCloseDuration;

	public float bladeSpritesFinalAngle;

	public float transitionBars_animationDuration;

	public float transitionBars_animationDelay;

	public Ease transitionBars_animationEase;

	public Ease rowSeparationTiedWithRoomEase;

	public ArmSkin armSkin_paige;

	public ArmSkin armSkin_ian;

	public ArmSkin armSkin_edega;

	public ArmSkin armSkin_politician;

	public Texture2D transDrawing;

	public Texture2D nbDrawing;

	[Header("Freezeshot")]
	public Texture2D[] freezeshot_textures;

	public float freezeshot_animationFPS;

	[Header("Sounds")]
	public BeatSoundMode beatSoundMode;

	public float beat_audibleSkipBeatGainMultiplier;

	public float beat_firstBeatPitch;

	public float beat_firstBeatVolume;

	public float[] beat_customBeatPitches;

	public float[] beat_customBeatVolumes;

	[Header("Colors")]
	public Color simulatedDesktopColor;

	public Color bossHPBarColor;

	public Color player1HPBarColor;

	public Color player2HPBarColor;

	public Color lowHealthHPBarColor;

	public Color rdsrtEarlyColor;

	public Color rdsrtLateColor;

	public Color rdsrtPerfectColor;

	public Color[] colorPalette;

	public Color[] brushColors;

	public Color[] sleeveColors;

	public Color[] skinColors;

	public Color[] nailColors;

	public int[] defaultP1SleeveColor;

	public int[] defaultP1SkinColor;

	public int[] defaultP1NailColor;

	public float[] defaultP1PalmLightness;

	public int[] defaultP2SleeveColor;

	public int[] defaultP2SkinColor;

	public int[] defaultP2NailColor;

	public float[] defaultP2PalmLightness;

	[Header("Row Colors")]
	public Color player1StripColor;

	public Color player2StripColor;

	public Color playerCPUStripColor;

	public Color beatboxColor;

	public Color playerboxColor;

	public Color playerboxP1Color;

	public Color playerboxP2Color;

	public Color playerboxCPUColor;

	public Color oneshotSquareDefaultTint;

	public Color oneshotWaveNormalColor;

	public Color particleColor;

	public Color rowHeaderDefaultColor;

	public Color rowHeaderIncompatibleColor;

	[Header("Hit Strip")]
	public Color hitStripDefaultColor;

	public Color hitStripP1Color;

	public Color hitStripP2Color;

	public Color hitStripHoldColor;

	public Color hitStripHoldParticleColor;

	[NonSerialized]
	public string[] hexPalette;

	[Header("Banana Beats")]
	public float bananaPullMaxDuration = 0.5f;

	public float bananaReleaseMaxDuration = 0.5f;

	public float bananaReleaseNoSpringMaxDuration = 0.5f;

	public float bananaBeatboxDelay;

	[Header("Beatbox Smoke")]
	public Color beatboxColorSmoke;

	[Header("Level - Oriental Techno")]
	public Color techno_additive0;

	public Color techno_additive1;

	public Color techno_overlay0;

	public Color techno_overlay1;

	public Color techno_glow0;

	public Color techno_glow1;

	public float techno_openDoorsBeat;

	public float techno_rowFlyDuration;

	public Ease techno_rowFlyEase;

	public float techno_rowFlyBeat;

	public float techno_rowFly2Duration;

	public Ease techno_rowFly2Ease;

	public float techno_rowFly2Beat;

	[Header("Level - SVT")]
	public float svt_slowDownDuration;

	public float svt_slowDownTo;

	public Ease svt_ease;

	[Header("Intimate")]
	public float intimate_skyScrollX;

	public float intimate_cityBackScrollX;

	public float intimate_cityFrontScrollX;

	public float intimate_shadowDelayDenominator;

	[Header("Opening Creds")]
	public RDTheme theme;

	public bool allSilhouettes;

	public float noiseMultiplier;

	public bool changeRandomCharEveryBeat;

	public float opening_rooftopYDistance;

	public float opening_rooftopYDuration;

	public Ease opening_rooftopYEase;

	[Header("Playground")]
	public Ease playground_pulse_ease;

	public float playground_pulse_tick;

	public float playground_pulse_zoom;

	public int playground_pulse_count;

	[Header("AnimCurves")]
	public AnimationCurve pulseCam;

	public AnimationCurve beatPull;

	public AnimationCurve beatRelease;

	public AnimationCurve beatPullQuick;

	public AnimationCurve beatReleaseNoSpring;

	[Header("Infinite Zoom")]
	public AnimationCurve curveInfiniteZoomPulse;

	public AnimationCurve curveLinear;

	[Header("Shader")]
	public Shader renderTextureShader;

	[Header("Editor")]
	public float copiedLabel_animDistance;

	public float copiedLabel_animDuration;

	public Ease copiedLabel_animEase;

	public float roomsSelectionPopup_showAnimDuration;

	public Ease roomsSelectionPopup_showAnimEase;

	public float roomsSelectionPopup_hideAnimDuration;

	public Ease roomsSelectionPopup_hideAnimEase;

	public Color metronome_onColor;

	public Color metronome_offColor;

	public Color metronomeXmas_onColor;

	public Color metronomeXmas_offColor;

	public Color twoPlayer_onColor;

	public Color twoPlayer_offColor;

	public Color autoplay_onColor;

	public Color autoplay_offColor;

	public Color followPlayhead_onColor;

	public Color followPlayhead_offColor;

	public Color selectBookmark_onColor;

	public Color selectBookmark_offColor;

	public Color eventTagStripColor;

	public Color eventTagRunStripColor;

	public Color beatControl_yellowPulseNormal;

	public Color beatControl_yellowPulseHighlighted;

	public Color beatControl_greenPulseNormal;

	public Color beatControl_greenPulseHighlighted;

	public Color addFreetimeControlColor;

	public Color pulseFreetimeControlColor;

	public Sprite[] toolsSleepingFrames;

	public float timeForToolsToSleep;

	public Color allowedColor;

	public Color mostlyAllowedColor;

	public Color declinedColor;

	public Color particallyDeclinedColor;

	[Header("Editor - Auto Properties")]
	public GameObject propertyPrefab;

	public GameObject propertiesList;

	public GameObject rowPrefab;

	public GameObject descriptionControlPrefab;

	public GameObject buttonControlPrefab;

	public GameObject colorControlPrefab;

	public GameObject dropdownControlPrefab;

	public GameObject rowControlPrefab;

	public GameObject roomControlPrefab;

	public GameObject inputFieldControlPrefab;

	public GameObject checkboxControlPrefab;

	public GameObject toggleGroupControlPrefab;

	public GameObject toggleControlPrefab;

	public GameObject sliderControlPrefab;

	public GameObject sliderAlphaControlPrefab;

	public GameObject sliderPercentControlPrefab;

	public GameObject sliderDescriptionControlPrefab;

	public GameObject positionPickerControlPrefab;

	public GameObject expPositionPickerControlPrefab;

	public GameObject characterPickerControlPrefab;

	public GameObject handControlPrefab;

	public GameObject soundInputControlPrefab;

	public GameObject soundControlPrefab;

	public GameObject imageControlPrefab;

	public GameObject reorderRoomsControlPrefab;

	public GameObject showRoomsControlPrefab;

	public GameObject setRoomPerspectiveControlPrefab;

	public GameObject flipScreenControlPrefab;

	public GameObject pulsePickerControlPrefab;

	public GameObject bpmCalculatorControlPrefab;

	public GameObject beatModifiersControlPrefab;

	public GameObject showDialogueControlPrefab;

	public GameObject setCountingSoundControlPrefab;

	public GameObject gameSoundControlPrefab;

	public GameObject setGameSoundControlPrefab;

	[Header("Oneshot Shadows")]
	public bool useShadows;

	public int shadowsNumber;

	public float rushAnimMinPercentage;

	public float rushAnimMaxPercentage;

	public float shadowFadeDuration;

	public float shadowDurationDifference;

	public Ease shadowFadeEase;

	public float shadowStartMinAlpha;

	public float shadowStartMaxAlpha;

	public float shadowOffsetPer;

	public float minOneshotLengthForShadows;

	[Header("Language Fonts")]
	[FormerlySerializedAs("chineseFontPack")]
	public FontPack chineseTradFontPack;

	public FontPack chineseSimpFontPack;

	public FontPack japaneseFontPack;

	public FontPack koreanFontPack;

	public FontPack latinFontPack;

	[Header("Testing")]
	public bool dontplaysametime;

	public Ease windowEase;

	public float windowAnimation_duration;

	public Ease pulse_ease;

	[Header("Ink")]
	public float ink_punctuationMarkFastMultiplier;

	public float ink_baseTimePerCharacterPassive;

	public float ink_baseTimePerCharacterActive;

	public float ink_baseTime_CJK_multiplier;

	public float ink_minTimeBetweenLinesPassive;

	public float ink_maxTimeBetweenLinesPassive;

	public float ink_passiveReadingTimePerCharacter;

	public float ink_bigPause;

	public float ink_smallPause;

	public float ink_vslow_Dialogue_Speed;

	public float ink_slow_Dialogue_Speed;

	public float ink_normal_Dialogue_Speed;

	public float ink_fast_Dialogue_Speed;

	public float ink_vfast_Dialogue_Speed;

	public float ink_vvvfast_Dialogue_Speed;

	public float ink_shout_Dialogue_Speed;

	public float ink_ian_Dialogue_Speed;

	public float ink_paige_Dialogue_Speed;

	public float ink_excited_Dialogue_Speed;

	public float ink_fastforward;

	public float ink_PauseTimeBeforePortrait;

	public float ink_PauseTimeAfterPortrait;

	[Header("Level Select")]
	public Color levelSelect_notPassedLevelTextOutline;

	public Color levelSelect_passedLevelTextOutline;

	public Color levelSelect_perfectLevelTextOutline;

	public Color levelSelect_lockedLevelTextOutline;

	public SpriteLocalization[] startLocalizations;

	public SpriteLocalization[] start2PLocalizations;

	[Header("Prefabs and Other")]
	public GameObject errorCanvas;

	public Texture2D emptyTexture;

	public Texture2D beatboxSmokeTexture;

	public Vector2 beatboxSmokeSpeed;

	public GameObject coffeeShop;

	public GameObject rooftop;

	public GameObject garden;

	public GameObject hospitalWard;

	public GameObject paigeFire;

	public Material UIPaletteMaterial;

	public Material RDSpriteMaterial;

	public KeyHelperInfo[] keyHelperInfos;

	public GameObject customSprite;

	[Header("Ian's Desktop")]
	public Color desktop_green;

	public Color desktop_yellow;

	public Color desktop_purple;

	public Color desktop_violet;

	[Header("2D Toolkit")]
	public tk2dSpriteCollectionData collection_char;

	public tk2dSpriteAnimation animations_char;

	public static RDConstants data
	{
		get
		{
			if (internalData == null)
			{
				internalData = Resources.Load<RDConstants>("RDConstants");
				internalData.Setup();
			}
			return internalData;
		}
	}

	public void Setup()
	{
		hexPalette = new string[colorPalette.Length];
		for (int i = 0; i < hexPalette.Length; i++)
		{
			hexPalette[i] = colorPalette[i].ToHex();
		}
	}
}
