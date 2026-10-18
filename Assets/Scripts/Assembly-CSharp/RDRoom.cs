using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Core;
using ProceduralTree;
using RDLevelEditor;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class RDRoom : RDBase
{
	public enum shakeIntensityPreset
	{
		Dubstep = 0,
		OneshotLarge = 1
	}

	public const float RoomsXOffset = 3000f;

	private const string RoomPrefix = "Room ";

	public Camera camera;

	public RoomCamera rdCamera;

	public GameObject quadPrefab;

	public GameObject renderTextureQuadPrefab;

	public Transform cameraContainer;

	public Transform rowContainer;

	public Transform decorationContainer;

	public scrQuad overlay;

	public scrQuad backgroundOverlay;

	public Transform backgroundContainerWorldspace;

	public Transform backgroundContainerCamera;

	public OneshotSpotlights oneshotSpotlights;

	[NonSerialized]
	public scrStutter stutter;

	[NonSerialized]
	public scrPasteToRenderTex renderTexPaster;

	[NonSerialized]
	public int index;

	[NonSerialized]
	public GameObject renderQuad;

	[NonSerialized]
	public Transform renderQuadPivot;

	[NonSerialized]
	public Renderer quadRenderer;

	[NonSerialized]
	public Material quadMaterial;

	[NonSerialized]
	public Mesh quadMesh;

	[NonSerialized]
	public bool customTransform;

	[NonSerialized]
	public bool roomMovedByLevelEvent;

	[NonSerialized]
	public bool followedByWindow;

	public RenderTexture renderTexture;

	[NonSerialized]
	public Texture2D[] maskTextures;

	private double initialTime;

	private float maskFps = 1f;

	[NonSerialized]
	public RenderTexture roomMask;

	[NonSerialized]
	public RDEnvironment currentEnvironment;

	[NonSerialized]
	public ContentMode contentMode;

	[NonSerialized]
	public float yOffset;

	[NonSerialized]
	public int verticalRowSpacing = 42;

	[NonSerialized]
	public bool rowSeparationTiedToRoomHeight;

	private float yOffsetVisible;

	private float yOffsetTarget;

	private float yOffsetPrevious;

	[NonSerialized]
	public Range[] playerBounds = new Range[3];

	[NonSerialized]
	public int yPlayerStripBottom;

	[NonSerialized]
	public int yPlayerStripMiddle;

	[NonSerialized]
	public int yPlayerStripTop;

	[NonSerialized]
	public int xPlayerStrip;

	[NonSerialized]
	public bool donutFlash;

	[NonSerialized]
	public Tween positionTweenX;

	[NonSerialized]
	public Tween positionTweenY;

	[NonSerialized]
	public Tween scaleTweenX;

	[NonSerialized]
	public Tween scaleTweenY;

	[NonSerialized]
	public Tween pivotTweenX;

	[NonSerialized]
	public Tween pivotTweenY;

	[NonSerialized]
	public Tween opacityTween;

	[NonSerialized]
	public Tween scrollTween;

	[NonSerialized]
	public Tween[] vertexTweensX = new Tween[4];

	[NonSerialized]
	public Tween[] vertexTweensY = new Tween[4];

	[NonSerialized]
	public Dictionary<RDThemeFX, Dictionary<int, Tween>> vfxTweens = new Dictionary<RDThemeFX, Dictionary<int, Tween>>();

	[NonSerialized]
	public List<RDThemeFX> usedThemeFX = new List<RDThemeFX>();

	public Vector3[] currentVertexPositions = new Vector3[4]
	{
		new Vector3(-0.5f, -0.5f, 0f),
		new Vector3(0.5f, -0.5f, 0f),
		new Vector3(-0.5f, 0.5f, 0f),
		new Vector3(0.5f, 0.5f, 0f)
	};

	public Vector3[] previousVertexPositions;

	[NonSerialized]
	public float angle;

	private Coroutine rotateCoroutine;

	[NonSerialized]
	public LyricsGame lyrics;

	private float mLyricsPositionYTo;

	private Coroutine mQuadAnimation;

	private const int ShockwaveCount = 10;

	private List<GameObject> mShockwaves = new List<GameObject>();

	[NonSerialized]
	public tk2dSprite vignette;

	private Rect mInitialRect;

	private Rect mFinalRect;

	private Ease ease;

	private bool mInactivatesAfterAnimation;

	private float mAnimationDuration;

	private Vector2Int renderTextureBaseSize;

	private scrQuad playerStripP1;

	private scrQuad playerStripP2;

	[NonSerialized]
	public GameObject coffeeShop;

	[NonSerialized]
	public GameObject samuraiWard;

	[NonSerialized]
	public GameObject garden;

	[NonSerialized]
	public GameObject rooftop;

	[NonSerialized]
	public GameObject train;

	[NonSerialized]
	public GameObject hospitalWard;

	[NonSerialized]
	public GameObject coleWard;

	[NonSerialized]
	public GameObject tutorialBg;

	[NonSerialized]
	public GameObject boyWard;

	[NonSerialized]
	public GameObject girlWard;

	[NonSerialized]
	public GameObject basement;

	[NonSerialized]
	public GameObject rollerDisco;

	[NonSerialized]
	public GameObject athleteWard;

	[NonSerialized]
	public GameObject proceduralTree;

	[NonSerialized]
	public GameObject recordsRoom;

	[NonSerialized]
	public GameObject airport;

	[NonSerialized]
	public GameObject abandonedWard;

	[NonSerialized]
	public RDTheme currentTheme;

	[NonSerialized]
	public Dictionary<RDTheme, float> defaultPositions = new Dictionary<RDTheme, float>();

	private GameObject activeBackground;

	private RDTrain trainComp;

	private RDGarden gardenComp;

	private int hospitalRoomOffset = -250;

	private bool usingBrightness;

	private bool usingContrast;

	private bool usingSaturation;

	private float scrolledPosition;

	private int customBackgroundDepth;

	[NonSerialized]
	public int windowPeekIndex;

	[NonSerialized]
	public bool moveRoomPositionXUsed;

	[NonSerialized]
	public bool moveRoomPositionYUsed;

	[NonSerialized]
	public bool moveRoomScaleXUsed;

	[NonSerialized]
	public bool moveRoomScaleYUsed;

	[NonSerialized]
	public bool moveRoomAngleUsed;

	[NonSerialized]
	public bool moveRoomPivotUsed;

	[NonSerialized]
	public bool[] perspectiveRoomCornersUsed = new bool[4];

	[NonSerialized]
	public Vector2 moveRoomPosition;

	[NonSerialized]
	public Vector2 moveRoomScale;

	[NonSerialized]
	public float moveRoomAngle;

	[NonSerialized]
	public Vector3 moveRoomPivot;

	[NonSerialized]
	public Vector3[] perspectiveRoomCorners = new Vector3[4];

	[NonSerialized]
	public float finalAngle;

	private Vector3 blPrev = Vector3.negativeInfinity;

	private Vector3 brPrev = Vector3.negativeInfinity;

	private Vector3 tlPrev = Vector3.negativeInfinity;

	private Vector3 trPrev = Vector3.negativeInfinity;

	public bool _peekWindowMode;

	public float customPeekScale = 1f;

	private bool peekWindow_setup;

	private bool manually_setup;

	private Vector2Int peekWindow_initialWindowCenter;

	private Vector2 peekWindow_initialCameraPosition;

	private Tween topHorizontalStripTween;

	private Tween middleHorizontalStripTween;

	private Tween bottomHorizontalStripTween;

	private List<List<Background>> themeBackgroundsList = new List<List<Background>>();

	private List<Background> tblSamuraiWard = new List<Background>();

	private List<Background> tblInsomniacN = new List<Background>();

	private List<Background> tblInsomniac = new List<Background>();

	private List<Background> tblIntimateSimple = new List<Background>();

	private List<Background> tblRooftop = new List<Background>();

	private List<Background> tblNeonMuseum = new List<Background>();

	private List<Background> tblMatrix = new List<Background>();

	private List<Background> tblMatrixAlt = new List<Background>();

	private List<Background> tblPoliticiansRally = new List<Background>();

	private List<Background> tblBackAlley = new List<Background>();

	private List<Background> tblSky = new List<Background>();

	private List<Background> tblSkyline = new List<Background>();

	private List<Background> tblSkylineBlue = new List<Background>();

	private List<Background> tblVaporwave = new List<Background>();

	private Background ptIntimateSimpleRain;

	private Background ptMatrix1;

	private Background ptMatrix2;

	private Background ptMatrix3;

	private Background ptMatrixAlt1;

	private Background ptMatrixAlt2;

	private Background ptMatrixAlt3;

	private scrVolumeTracker volTrackerMatrix;

	private Background ptInsomniacNightSky;

	private Background ptInsomniacNPagoda;

	private Background ptInsomniacNPagodaLights;

	private Background ptInsomniacNStars;

	private Background ptInsomniacNPetals;

	private Background ptInsomniacNCat;

	private Background ptInsomniacNBat;

	private Background ptInsomniacSky;

	private Background ptInsomniacTrees;

	private Background ptInsomniacPagoda;

	private Background ptInsomniacSakuraTree;

	private Background ptRooftopBackClouds;

	private Background ptRooftopFrontClouds;

	private Background ptRooftopMidClouds;

	private Background ptRooftopRooftop;

	private Background ptRooftopSignLights;

	private Background ptRooftopSky;

	private Background ptRooftopSkyscrapers;

	private Background ptPoliticiansRally;

	private Background ptNeonMuseumBg;

	private Background ptBackAlleyBackground;

	private Background ptBackAlleyWindows;

	private Background ptBackAlleyCloud1;

	private Background ptBackAlleyCloud2;

	private Background ptBackAlleyCloud3;

	private Background ptBackAlleyCloud4;

	private Background ptBackAlleyForeground;

	private Background ptBackAlleyClubSign;

	private Background ptBackAlleyClubSignOn;

	private Background ptBackAlleyCafeSign;

	private Background ptBackAlleyCafeSignOn;

	private Background ptBackAlleyLamppostGlow;

	private Background ptSkylineSky;

	private Background ptSkylineFront;

	private Background ptSkylineBack;

	private Background ptSkylineSkyBlue;

	private Background ptSkylineFrontBlue;

	private Background ptSkylineBackBlue;

	private Background ptVaporwave;

	private Background ptTutorialNotes;

	private scrExecuteOnHit fxShakeOnHit;

	private scrExecuteOnHit fxBassDrop;

	private scrExecuteOnEveryHB fxShakeOnHB;

	private scrExecuteOnEveryHB fxSilhouetteChangeOnHB;

	private scrVolumeTracker fxVignetteFlicker;

	private Background fxNoise;

	private Background fxGlitch;

	private ParticleSystem confetti;

	private ParticleSystem confettiBurstL;

	private ParticleSystem confettiBurstR;

	private ParticleSystem petals1;

	private ParticleSystem petals2;

	private ParticleSystem petals3;

	private ParticleSystem petalsSnow1;

	private ParticleSystem petalsSnow2;

	private ParticleSystem petalsSnow3;

	private ParticleSystem leaves1;

	private ParticleSystem leaves2;

	private ParticleSystem leaves3;

	private ParticleSystem snowParticles;

	private ParticleSystem diamondParticles;

	private ParticleSystem emberVFXParticles;

	private ShatteredGlass glass;

	private RDManyEyes eyesBig;

	private RDManyEyes eyesSmall;

	public SparklineBalloons sparklineBalloons;

	[NonSerialized]
	public List<RDTheme> preloadedThemes = new List<RDTheme>();

	[NonSerialized]
	public List<RDThemeFX> preloadedVFX = new List<RDThemeFX>();

	[NonSerialized]
	public float bloomThreshold;

	[NonSerialized]
	public float bloomIntensity;

	[NonSerialized]
	public Color bloomTint;

	public GameObject kaleidoscopePrefab;

	[NonSerialized]
	public RDKaleidoscope kaleidoscope;

	private bool mKaleidoscopeMode;

	public GameObject kaleidoscopeCrossPrefab;

	[NonSerialized]
	public RDKaleidoscope mKaleidoscopeCross;

	[NonSerialized]
	public bool mKaleidoscopeModeCross;

	public GameObject kaleidoscopeEmbersPrefab;

	[NonSerialized]
	public RDKaleidoscope embers;

	[NonSerialized]
	public bool mKaleidoscopeModeEmbers;

	public GameObject voxelObjPrefab;

	[NonSerialized]
	public RDRotatingVoxel voxelObj;

	public GameObject spacePrefab;

	[NonSerialized]
	public RDSpaceBackground spaceBackground;

	[NonSerialized]
	public bool mSpaceMode;

	private bool mWavyRows;

	private float wavyRowsTimeElapsed;

	[NonSerialized]
	public float wavyRowsFrequency = 2f;

	[NonSerialized]
	public float wavyRowsAmplitude = 15f;

	[NonSerialized]
	public float customWavyRowsFrequencyMult = 1f;

	[NonSerialized]
	public float customWavyRowsAmplitudeMult = 1f;

	private bool mShowTextAboveBeatboxes;

	private int lastNumberPulsed;

	public Tween textCountTween;

	[NonSerialized]
	public string[] CustomTextAboveBeatboxes = new string[8] { "", "1", "2", "3", "4", "5", "6", "7" };

	private TextMesh mCountText;

	[NonSerialized]
	public RDLightStrip lightStrip;

	private bool mBeatboxWithLightStrip;

	public Tween syncTextTween;

	private scrExecuteOnHit syncTextEOH;

	private bool syncTextEnabled;

	private bool _inverseShockwave;

	private GameObject inverseShockwavePrefab;

	private GameObject inverseShockwaveGO;

	private GameObject customBackground;

	private GameObject customForeground;

	private const int perspectiveSegmentCount = 16;

	private VideoBloom camBloom => camera.GetComponent<VideoBloom>();

	public RowEntity firstRow
	{
		get
		{
			Row[] rows = base.game.rows;
			foreach (Row obj in rows)
			{
				RowEntity ent = obj.ent;
				if (obj.room == index && ent != null && ent.gameObject.activeSelf)
				{
					return ent;
				}
			}
			return null;
		}
	}

	public bool peekWindowMode
	{
		get
		{
			return _peekWindowMode;
		}
		set
		{
			if (value && !manually_setup)
			{
				peekWindow_setup = false;
			}
			else if (peekWindow_setup)
			{
				camera.transform.MoveXY(peekWindow_initialCameraPosition);
			}
			_peekWindowMode = value;
		}
	}

	public bool kaleidoscopeMode
	{
		get
		{
			return mKaleidoscopeMode;
		}
		set
		{
			mKaleidoscopeMode = value;
			if (mKaleidoscopeMode && kaleidoscope == null)
			{
				kaleidoscope = UnityEngine.Object.Instantiate(kaleidoscopePrefab, base.gm.parentContainer).GetComponent<RDKaleidoscope>();
				kaleidoscope.transform.LocalMoveX(3000f * (float)index);
				kaleidoscope.room = this;
			}
			if (kaleidoscope != null)
			{
				kaleidoscope.gameObject.SetActive(mKaleidoscopeMode);
			}
			kaleidoscope?.ToggleVolumeTracker(mKaleidoscopeMode);
			if (scnGame.instance.currentLevel.multiroom)
			{
				if (mKaleidoscopeMode)
				{
					kaleidoscope.kaleidoCam.targetTexture = camera.targetTexture;
					camera.clearFlags = CameraClearFlags.Depth;
				}
				else
				{
					camera.clearFlags = CameraClearFlags.Color;
				}
			}
			else
			{
				camera.clearFlags = (mKaleidoscopeMode ? CameraClearFlags.Nothing : CameraClearFlags.Color);
			}
		}
	}

	public Color kaleidoscopeColor0
	{
		set
		{
			kaleidoscope.SetColor(0, value, 1.5f);
		}
	}

	public Color kaleidoscopeColor1
	{
		set
		{
			kaleidoscope.SetColor(1, value, 1.5f);
		}
	}

	public float kaleidoscopeSpeed
	{
		get
		{
			return kaleidoscope.speed;
		}
		set
		{
			kaleidoscope.SetSpeed(value);
		}
	}

	public float kaleidoscopeEmissionRate
	{
		set
		{
			kaleidoscope.SetEmissionRate(value);
		}
	}

	public bool kaleidoscopeModeCross
	{
		get
		{
			return mKaleidoscopeModeCross;
		}
		set
		{
			mKaleidoscopeModeCross = value;
			if (mKaleidoscopeModeCross && mKaleidoscopeCross == null)
			{
				mKaleidoscopeCross = UnityEngine.Object.Instantiate(kaleidoscopeCrossPrefab, base.gm.parentContainer).GetComponent<RDKaleidoscope>();
				mKaleidoscopeCross.transform.LocalMoveX(3000f * (float)index);
				mKaleidoscopeCross.room = this;
			}
			if (mKaleidoscopeCross != null)
			{
				mKaleidoscopeCross.gameObject.SetActive(mKaleidoscopeModeCross);
			}
			if (scnGame.instance.currentLevel.multiroom)
			{
				if (mKaleidoscopeModeCross)
				{
					mKaleidoscopeCross.kaleidoCam.targetTexture = camera.targetTexture;
					camera.clearFlags = CameraClearFlags.Depth;
				}
				else
				{
					camera.clearFlags = CameraClearFlags.Color;
				}
			}
			else
			{
				camera.clearFlags = (mKaleidoscopeModeCross ? CameraClearFlags.Nothing : CameraClearFlags.Color);
			}
		}
	}

	public Color kaleidoscopeCrossColor0
	{
		set
		{
			mKaleidoscopeCross.SetColor(0, value, 1.5f);
		}
	}

	public Color kaleidoscopeCrossColor1
	{
		set
		{
			mKaleidoscopeCross.SetColor(1, value, 1.5f);
		}
	}

	public float kaleidoscopeCrossSpeed
	{
		get
		{
			return mKaleidoscopeCross.speed;
		}
		set
		{
			mKaleidoscopeCross.SetSpeed(value);
		}
	}

	public float kaleidoscopeCrossEmissionRate
	{
		set
		{
			mKaleidoscopeCross.SetEmissionRate(value);
		}
	}

	public bool kaleidoscopeModeEmbers
	{
		get
		{
			return mKaleidoscopeModeEmbers;
		}
		set
		{
			mKaleidoscopeModeEmbers = value;
			if (mKaleidoscopeModeEmbers)
			{
				PreloadEmbers();
			}
			if (embers != null)
			{
				embers.gameObject.SetActive(mKaleidoscopeModeEmbers);
			}
			if (camera == null)
			{
				printe("camera is null");
			}
			if (scnGame.instance.currentLevel.multiroom)
			{
				if (mKaleidoscopeModeEmbers)
				{
					embers.kaleidoCam.targetTexture = camera.targetTexture;
					camera.clearFlags = CameraClearFlags.Depth;
				}
				else
				{
					camera.clearFlags = CameraClearFlags.Color;
				}
			}
			else
			{
				camera.clearFlags = (mKaleidoscopeModeEmbers ? CameraClearFlags.Nothing : CameraClearFlags.Color);
			}
		}
	}

	public Color kaleidoscopeEmbersColor0
	{
		set
		{
			embers.SetColor(0, value, 1.5f);
		}
	}

	public Color kaleidoscopeEmbersColor1
	{
		set
		{
			embers.SetColor(1, value, 1.5f);
		}
	}

	public float kaleidoscopeEmbersSpeed
	{
		get
		{
			return embers.speed;
		}
		set
		{
			embers.SetSpeed(value);
		}
	}

	public float kaleidoscopeEmbersEmissionRate
	{
		set
		{
			embers.SetEmissionRate(value);
		}
	}

	public VideoBloom emberBloom
	{
		get
		{
			if ((bool)embers)
			{
				return embers.kaleidoCam.GetComponent<VideoBloom>();
			}
			return null;
		}
	}

	public VideoBloom crossBloom => mKaleidoscopeCross?.kaleidoCam.GetComponent<VideoBloom>();

	public VideoBloom bloom => camera.GetComponent<VideoBloom>();

	public bool rotatingVoxelMode
	{
		set
		{
			bool flag = value;
			if (flag && voxelObj == null)
			{
				voxelObj = UnityEngine.Object.Instantiate(voxelObjPrefab, base.gm.parentContainer).GetComponent<RDRotatingVoxel>();
				voxelObj.transform.LocalMoveX(3000f * (float)index);
				voxelObj.room = this;
			}
			if (voxelObj != null)
			{
				voxelObj.gameObject.SetActive(flag);
				if (!flag)
				{
					voxelObj.ResetRotation();
				}
			}
			if (scnGame.instance.currentLevel.multiroom)
			{
				if (flag)
				{
					voxelObj.kaleidoCam.targetTexture = camera.targetTexture;
					camera.clearFlags = CameraClearFlags.Depth;
				}
				else
				{
					camera.clearFlags = CameraClearFlags.Color;
				}
			}
			else
			{
				camera.clearFlags = (flag ? CameraClearFlags.Nothing : CameraClearFlags.Color);
			}
		}
	}

	public float rotatingVoxelSpeed
	{
		get
		{
			return voxelObj.speed;
		}
		set
		{
			voxelObj.SetSpeed(value);
		}
	}

	public RotatingVoxelMode voxelMode
	{
		get
		{
			return voxelObj.voxelMode;
		}
		set
		{
			voxelObj.SetVoxelMode(value);
		}
	}

	public bool voxelFallingCubes
	{
		set
		{
			voxelObj.SetCubeMode(value);
		}
	}

	public bool spaceMode
	{
		get
		{
			return mSpaceMode;
		}
		set
		{
			mSpaceMode = value;
			if (spaceBackground != null)
			{
				spaceBackground.gameObject.SetActive(mSpaceMode);
			}
			if (scnGame.instance.currentLevel.multiroom)
			{
				if (mSpaceMode)
				{
					spaceBackground.mainCam.targetTexture = camera.targetTexture;
					spaceBackground.starsCam.targetTexture = camera.targetTexture;
					camera.clearFlags = CameraClearFlags.Depth;
				}
				else
				{
					camera.clearFlags = CameraClearFlags.Color;
				}
			}
			else
			{
				camera.clearFlags = (mSpaceMode ? CameraClearFlags.Nothing : CameraClearFlags.Color);
			}
		}
	}

	public bool wavyRows
	{
		get
		{
			return mWavyRows;
		}
		set
		{
			if (value)
			{
				if (!mWavyRows)
				{
					wavyRowsTimeElapsed = 0f;
				}
			}
			else
			{
				SetRowContainerY(0f);
			}
			mWavyRows = value;
		}
	}

	public bool showTextAboveBeatboxes
	{
		set
		{
			mShowTextAboveBeatboxes = value;
		}
	}

	public TextMesh countText
	{
		get
		{
			if (mCountText == null)
			{
				mCountText = GameObject.Find("CountText").GetComponent<TextMesh>();
			}
			return mCountText;
		}
	}

	public bool beatboxVerticalStrip
	{
		get
		{
			return mBeatboxWithLightStrip;
		}
		set
		{
			mBeatboxWithLightStrip = value;
			if (!value)
			{
				lightStrip.Hide();
			}
		}
	}

	public bool enableSyncText
	{
		set
		{
			if (!GC.twoPlayerMode)
			{
				return;
			}
			TextMesh syncText = base.game.syncText;
			if (syncTextEOH == null)
			{
				syncTextEOH = scrExecuteOnHit.Add(() =>
				{
					if (base.game.syncHitRemainingTime > 0f)
					{
						float num = 0f;
						float num2 = 0f;
						int num3 = 0;
						Row[] rows = base.game.rows;
						foreach (Row row in rows)
						{
							if (!(row.ent == null))
							{
								RowEntity ent = row.ent;
								num3++;
								num += ent.hitstripCenter.position.x;
								num2 = Mathf.Max(ent.hitstripCenter.position.y, num2);
							}
						}
						num /= (float)num3;
						Vector3 vector = new Vector3(num, num2 + 35f, 0f);
						syncText.gameObject.SetActive(value: true);
						syncText.transform.MoveXY(vector.x, vector.y);
						syncText.transform.DOMoveY(vector.y + 20f, 0.5f).SetDelay(0f);
						syncText.color = "31DD10".HexToColor();
						syncTextTween.Kill();
						syncTextTween = DOTween.ToAlpha(() => syncText.color, (Color c) =>
						{
							syncText.color = c;
						}, 0f, 0.5f).SetDelay(0f);
						base.game.OnMistakeOrHeal(0f, -0.3f, null, removeMistakes: false, RDPlayer.BOTH);
					}
					base.game.syncHitRemainingTime = 0.05f;
				}, HitType.Hit, persistent: true, autoDelete: false, -1, overwrite: false);
			}
			else
			{
				syncTextEOH.enabled = base.enabled;
				if (!base.enabled)
				{
					syncText.gameObject.SetActive(value: false);
				}
			}
			syncTextEnabled = value;
		}
	}

	public bool colorfulShockwaves
	{
		get
		{
			return _inverseShockwave;
		}
		set
		{
			if (!value)
			{
				if (inverseShockwaveGO != null)
				{
					camera.GetComponent<RDHueShift>().enabled = false;
					inverseShockwaveGO.transform.DOKill();
					inverseShockwaveGO.SetActive(value: false);
				}
			}
			else if (inverseShockwavePrefab == null)
			{
				inverseShockwavePrefab = Resources.Load<GameObject>("Levels/RDInverseShockwave");
				inverseShockwaveGO = UnityEngine.Object.Instantiate(inverseShockwavePrefab, base.gm.parentContainer);
			}
			_inverseShockwave = value;
		}
	}

	private RDAthleteWard stadium => athleteWard?.GetComponent<RDAthleteWard>();

	private void OnDestroy()
	{
		if (renderTexture != null)
		{
			renderTexture.Release();
			UnityEngine.Object.Destroy(renderTexture);
		}
	}

	public void Setup(int index, bool useRenderTextures)
	{
		this.index = index;
		int num = index;
		base.gameObject.name = "Room " + num;
		foreach (Transform item in base.transform)
		{
			item.name = item.name + " " + num;
			foreach (Transform item2 in item.transform)
			{
				item2.name = item2.name + " " + num;
				foreach (Transform item3 in item2.transform)
				{
					item3.name = item3.name + " " + num;
				}
			}
		}
		base.transform.localPosition = new Vector3(3000f * (float)num, 0f, 0f);
		camera.transform.localPosition = RDBase.Vfx.hudCamera.CameraCenterWithZ(-200f);
		camera.backgroundColor = Color.black;
		rdCamera.room = this;
		base.game.rooms[index] = this;
		if (useRenderTextures)
		{
			renderTexture = RDBase.Vfx.CreateRenderTexture(base.game.roomScale, "Debug", 24);
			renderTexture.name = $"Room {num} -> HUDCamera";
			renderTextureBaseSize = new Vector2Int(renderTexture.width, renderTexture.height);
			camera.targetTexture = renderTexture;
			Vector2 vector = RDBase.Vfx.hudCamera.RDFullNativeResolution();
			renderQuadPivot = new GameObject("Pivot " + num).transform;
			renderQuadPivot.parent = RDBase.Vfx.topObjects;
			renderQuadPivot.localPosition = new Vector3(0f, 0f, 20f);
			GameObject gameObject = UnityEngine.Object.Instantiate(renderTextureQuadPrefab);
			gameObject.transform.parent = renderQuadPivot;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localScale = new Vector3(vector.x, vector.y, 1f);
			gameObject.name = "RenderTextureQuad " + num;
			gameObject.layer = GC.GameHUDLayer;
			quadRenderer = gameObject.GetComponent<Renderer>();
			quadMaterial = quadRenderer.material;
			quadMaterial.mainTexture = renderTexture;
			quadRenderer.sortingOrder = 4 - num;
			base.game.renderTextureQuads.Add(gameObject);
			quadMesh = gameObject.GetComponent<MeshFilter>().mesh;
			renderQuad = gameObject;
			SetRenderingEnabled(index == 0);
			moveRoomPositionXUsed = true;
			moveRoomPosition.x = 0f;
			moveRoomScaleXUsed = true;
			moveRoomScale.x = 1f;
			if (index != 0)
			{
				SetNormalizedRectToQuad(new Rect(0f, 0f, 1f, 0f));
			}
			else
			{
				moveRoomPositionYUsed = true;
				moveRoomPosition.y = 0f;
				moveRoomScaleYUsed = true;
				moveRoomScale.y = 1f;
			}
		}
		previousVertexPositions = currentVertexPositions;
		InitializePerspectiveMesh();
		playerStripP1 = quadPrefab.Instantiate(base.transform, "PlayerStrip P1").GetComponent<scrQuad>();
		playerStripP2 = quadPrefab.Instantiate(base.transform, "PlayerStrip P2").GetComponent<scrQuad>();
		playerStripP1.SetSquareCenter(0, 0, 0, 0);
		playerStripP2.SetSquareCenter(0, 0, 0, 0);
		int num2 = RDBase.Vfx.RDHeightInt / 2;
		yPlayerStripBottom = (yPlayerStripMiddle = (yPlayerStripTop = num2));
		playerStripP1.SetColor(base.gc.player1StripColor);
		playerStripP2.SetColor(base.gc.player2StripColor);
		playerStripP1.SetColor(Color.clear);
		playerStripP2.SetColor(Color.clear);
		playerStripP1.SetSortingOrder(-10);
		playerStripP2.SetSortingOrder(-10);
		int num3 = (int)RDBase.Vfx.RDWidth * 2 + 2;
		int num4 = (int)RDBase.Vfx.RDHeight * 2 + 2;
		int num5 = (int)((float)(-num3) * 0.5f);
		int num6 = (int)((float)(-num4) * 0.5f);
		overlay.SetSquare(num5, num6, Mathf.CeilToInt(num3 + num5), Mathf.CeilToInt(num4 + num6), 201);
		overlay.SetSortingLayerName(RDSortingLayer.Foreground.ToString());
		overlay.GetComponent<Renderer>().material.renderQueue = 3000;
		overlay.sprite.color = Color.clear;
		backgroundOverlay.SetSquare(num5, num6, num3 + num5, num4 + num6, 50);
		backgroundOverlay.SetSortingLayerName(RDSortingLayer.Background.ToString());
		backgroundOverlay.sprite.color = Color.clear;
		vignette = RDBase.Vfx.InitializeVignette(camera);
		stutter = camera.GetComponent<scrStutter>();
		renderTexPaster = camera.GetComponent<scrPasteToRenderTex>();
		GameObject gameObject2 = RDBase.CreateIfNotFound("Shockwaves");
		for (num = 0; num < 10; num++)
		{
			GameObject gameObject3 = UnityEngine.Object.Instantiate(RDBase.Vfx.shockwavePrefab);
			gameObject3.gameObject.SetActive(value: false);
			gameObject3.transform.parent = gameObject2.transform;
			mShockwaves.Add(gameObject3);
		}
	}

	public void SetMoveRoomEnd()
	{
		if (moveRoomPositionXUsed)
		{
			renderQuadPivot.LocalMoveX(moveRoomPosition.x);
		}
		if (moveRoomPositionYUsed)
		{
			renderQuadPivot.LocalMoveY(moveRoomPosition.y);
		}
		if (moveRoomScaleXUsed)
		{
			renderQuadPivot.ScaleX(moveRoomScale.x);
		}
		if (moveRoomScaleYUsed)
		{
			renderQuadPivot.ScaleY(moveRoomScale.y);
		}
		if (moveRoomAngleUsed)
		{
			if (rotateCoroutine != null)
			{
				StopCoroutine(rotateCoroutine);
			}
			angle = moveRoomAngle;
			renderQuadPivot.transform.localEulerAngles = Vector3.forward * moveRoomAngle;
		}
		if (moveRoomPivotUsed)
		{
			renderQuad.transform.localPosition = moveRoomPivot;
		}
		UpdateVertices();
	}

	public void ResetMoveRoomProperties()
	{
		moveRoomPositionXUsed = false;
		moveRoomPositionYUsed = false;
		moveRoomScaleXUsed = false;
		moveRoomScaleYUsed = false;
		moveRoomAngleUsed = false;
		moveRoomPivotUsed = false;
	}

	public void FinishScrubbing()
	{
		KillAnimation(finish: false);
		if (rotateCoroutine != null)
		{
			StopCoroutine(rotateCoroutine);
		}
		angle = 0f;
		renderQuadPivot.transform.localEulerAngles = Vector3.zero;
		renderQuad.transform.localPosition = Vector3.zero;
		FinishAnimation();
		if (customTransform)
		{
			SetMoveRoomEnd();
		}
	}

	public void SetNormalizedRectToQuad(Rect rect)
	{
		float width = rect.width;
		float height = rect.height;
		float num = (-0.5f + rect.width * 0.5f + rect.x) * RDBase.Vfx.RDWidth;
		float num2 = (-0.5f + rect.height * 0.5f + rect.y) * RDBase.Vfx.RDHeight;
		renderQuadPivot.transform.localScale = new Vector3(width, height + 0f, 1f);
		renderQuadPivot.transform.localPosition = new Vector3(num, num2, renderQuadPivot.transform.localPosition.z);
	}

	public Rect NormalizedRect()
	{
		float num = renderQuadPivot.localScale.x;
		float num2 = renderQuadPivot.localScale.y;
		float num3 = renderQuadPivot.localPosition.x / RDBase.Vfx.RDWidth - num * 0.5f + 0.5f;
		float num4 = renderQuadPivot.localPosition.y / RDBase.Vfx.RDHeight - num2 * 0.5f + 0.5f;
		return new Rect(num3, num4, num, num2);
	}

	public void AnimateQuad(Rect from, Rect to, float duration, bool shouldDisappear, Ease ease = Ease.Unset)
	{
		this.ease = ease;
		mInitialRect = from;
		mFinalRect = to;
		mInactivatesAfterAnimation = shouldDisappear;
		mAnimationDuration = duration;
		ResetMoveRoomProperties();
		customTransform = false;
		KillAnimation();
		base.gameObject.SetActive(value: true);
		if (mAnimationDuration == 0f)
		{
			FinishAnimation();
		}
		else
		{
			mQuadAnimation = StartCoroutine(QuadAnimation());
		}
	}

	public bool EnableRenderingDependingOnRoomSize()
	{
		float num = Mathf.Abs(renderQuadPivot.localScale.x);
		float num2 = Mathf.Abs(renderQuadPivot.localScale.y);
		bool flag = num * num2 < 1E-06f;
		SetRenderingEnabled(!flag, followedByWindow);
		return flag;
	}

	private void SetRenderingEnabled(bool enabled, bool leaveCameraOn = false)
	{
		camera.enabled = enabled | leaveCameraOn;
		quadRenderer.enabled = enabled;
	}

	private IEnumerator QuadAnimation()
	{
		Rect from = mInitialRect;
		Rect to = mFinalRect;
		_ = renderQuad;
		float lyricsPositionYFrom = 0f;
		float lyricsPositionYTo = 0f;
		yOffsetPrevious = yOffsetTarget;
		yOffsetTarget = ((yOffset != 0f && to.height < 0.4f) ? yOffset : 0f);
		if (lyrics != null)
		{
			RectTransform component = lyrics.GetComponent<RectTransform>();
			lyricsPositionYFrom = component.anchoredPosition.y;
			lyricsPositionYTo = ((mFinalRect.height < 0.4f) ? (-10f) : 0f) + RDBase.Vfx.RDHeight * yOffsetTarget;
			mLyricsPositionYTo = Mathf.Floor(lyricsPositionYTo);
		}
		for (float t = 0f; t < mAnimationDuration; t += Time.deltaTime)
		{
			float t2 = ((ease == Ease.Unset) ? Mathf.SmoothStep(0f, 1f, t / mAnimationDuration) : DOVirtual.EasedValue(0f, 1f, t / mAnimationDuration, ease));
			float num = Mathf.Lerp(from.x, to.x, t2);
			float num2 = Mathf.Lerp(from.y, to.y, t2);
			float width = Mathf.Lerp(from.width, to.width, t2);
			float height = Mathf.Lerp(from.height, to.height, t2);
			yOffsetVisible = Mathf.Lerp(yOffsetPrevious, yOffsetTarget, t2);
			SetNormalizedRectToQuad(new Rect(num, num2, width, height));
			if (rowSeparationTiedToRoomHeight)
			{
				RepositionRowsAndStrips(instant: true);
			}
			if (lyrics != null)
			{
				RectTransform component = lyrics.GetComponent<RectTransform>();
				component.anchoredPosition = new Vector2(component.anchoredPosition.x, Mathf.Lerp(lyricsPositionYFrom, lyricsPositionYTo, t2));
			}
			yield return null;
		}
		FinishAnimation();
	}

	public void Rotate(float finalAngle, float duration, Ease ease)
	{
		if (rotateCoroutine != null)
		{
			StopCoroutine(rotateCoroutine);
		}
		this.finalAngle = finalAngle;
		if (duration == 0f)
		{
			angle = finalAngle;
			renderQuadPivot.transform.localEulerAngles = Vector3.forward * finalAngle;
		}
		else
		{
			base.gameObject.SetActive(value: true);
			rotateCoroutine = StartCoroutine(RotateCoroutine(duration, ease, Time.timeSinceLevelLoad, angle));
		}
	}

	private IEnumerator RotateCoroutine(float duration, Ease ease, float t0, float initialAngle)
	{
		float t1;
		do
		{
			t1 = Mathf.Clamp01((Time.timeSinceLevelLoad - t0) / duration);
			float num = (angle = DOVirtual.EasedValue(initialAngle, finalAngle, t1, ease));
			renderQuadPivot.transform.localEulerAngles = Vector3.forward * num;
			yield return null;
		}
		while (t1 < 1f);
	}

	public void FinishAnimation()
	{
		if (lyrics != null)
		{
			RectTransform component = lyrics.GetComponent<RectTransform>();
			component.anchoredPosition = new Vector2(component.anchoredPosition.x, mLyricsPositionYTo);
		}
		SetNormalizedRectToQuad(mFinalRect);
		EnableRenderingDependingOnRoomSize();
	}

	public void KillAnimation(bool finish = true)
	{
		if (mQuadAnimation != null)
		{
			StopCoroutine(mQuadAnimation);
			if (finish)
			{
				FinishAnimation();
			}
		}
	}

	private void Update()
	{
		if (mWavyRows)
		{
			wavyRowsTimeElapsed += Time.deltaTime;
			SetRowContainerY(Mathf.Sin(wavyRowsTimeElapsed * wavyRowsFrequency * customWavyRowsFrequencyMult) * wavyRowsAmplitude * customWavyRowsAmplitudeMult);
		}
		if (renderQuad != null)
		{
			quadRenderer.material.SetColor("_BackgroundColor", camera.backgroundColor);
			UpdateVertices();
		}
	}

	public void UpdateVertices()
	{
		if (renderQuad != null)
		{
			Vector3 vector = currentVertexPositions[0];
			Vector3 vector2 = currentVertexPositions[1];
			Vector3 vector3 = currentVertexPositions[2];
			Vector3 vector4 = currentVertexPositions[3];
			if (vector != blPrev || vector2 != brPrev || vector3 != tlPrev || vector4 != trPrev)
			{
				blPrev = vector;
				brPrev = vector2;
				tlPrev = vector3;
				trPrev = vector4;
				quadMaterial.SetVector("_BottomLeft", vector);
				quadMaterial.SetVector("_BottomRight", vector2);
				quadMaterial.SetVector("_TopLeft", vector3);
				quadMaterial.SetVector("_TopRight", vector4);
			}
		}
	}

	private void InitializePerspectiveMesh()
	{
		if (!(renderQuad == null))
		{
			MakePerspectiveMesh(quadMesh);
			quadMaterial.SetInt("_UsingPerspective", 1);
			quadMaterial.SetInt("_SegmentCount", 16);
			quadMesh.bounds = new Bounds(Vector3.zero, new Vector3(float.MaxValue, float.MaxValue, float.MaxValue));
		}
	}

	private void LateUpdate()
	{
		if (RDBase.debugSettings.Debug && KeyCode.Alpha1.WentDown())
		{
			peekWindowMode = !peekWindowMode;
		}
		UpdateHitStripPosition();
		UpdateRenderTextureQuadUVs();
		if (vignette.gameObject.activeSelf)
		{
			vignette.transform.localScale = (Vector3.one * (1f / rdCamera.zoom)).WithZ(1f);
		}
	}

	public void PeekWindowModeUpdate()
	{
		if (!peekWindowMode)
		{
			return;
		}
		WindowChoreographer windowChoreographer = base.game.windowChoreographer;
		if (windowChoreographer != null)
		{
			Vector2Int center = windowChoreographer.dancers[windowPeekIndex].window.GetCenter();
			if (!peekWindow_setup)
			{
				peekWindow_setup = true;
				peekWindow_initialWindowCenter = center;
				peekWindow_initialCameraPosition = camera.transform.position;
			}
			float num = windowChoreographer.GetWindowScaleForDancing();
			if (windowChoreographer is RealWindowChoreographer && base.platformHelper != null)
			{
				num *= base.platformHelper.scalingFactor;
			}
			Vector2 vector = center - peekWindow_initialWindowCenter;
			Vector2 v = peekWindow_initialCameraPosition + vector / num * customPeekScale;
			camera.transform.MoveXY(v);
		}
	}

	public void ManualCalibratePeek(Vector2Int targetWindowPos)
	{
		if (base.game.windowChoreographer != null)
		{
			peekWindow_setup = true;
			manually_setup = true;
			peekWindow_initialWindowCenter = targetWindowPos;
			peekWindow_initialCameraPosition = camera.transform.position;
		}
	}

	private void UpdateRenderTextureQuadUVs()
	{
		if (!(renderQuad == null))
		{
			float num = Mathf.Abs(renderQuadPivot.localScale.x);
			float num2 = Mathf.Abs(renderQuadPivot.localScale.y);
			if (contentMode == ContentMode.Real)
			{
				quadMaterial.mainTextureScale = Vector2.one;
				quadMaterial.mainTextureOffset = Vector2.zero;
				rdCamera.customScale = new Vector2(num, num2);
			}
			else if (contentMode == ContentMode.Center)
			{
				quadMaterial.mainTextureScale = new Vector2(num, num2);
				quadMaterial.mainTextureOffset = new Vector2((1f - num) / 2f, (1f - num2) / 2f + yOffsetVisible);
				rdCamera.customScale = Vector2.one;
			}
			else if (contentMode == ContentMode.AspectFill)
			{
				quadMaterial.mainTextureScale = Vector2.one;
				quadMaterial.mainTextureOffset = Vector2.zero;
				rdCamera.customScale = Vector2.one;
			}
			else
			{
				RDBaseDllDummy.printesw($"RDRoom: {contentMode} content mode not supported.");
			}
			if (roomMask != null)
			{
				SetMask(roomMask);
			}
			else if (maskTextures != null && maskTextures.Length != 0)
			{
				int num3 = (int)Math.Floor((AudioSettings.dspTime - initialTime) * (double)RDTime.speed * (double)maskFps % (double)maskTextures.Length);
				SetMask(maskTextures[num3]);
			}
			EnableRenderingDependingOnRoomSize();
		}
	}

	public void SetMask(Texture mask)
	{
		quadMaterial.SetTexture("_MaskTex", mask);
	}

	public void SetRoomMask(int room)
	{
		if (room < 0 || room > 3)
		{
			printe("Invalid room index!");
			return;
		}
		roomMask = base.game.rooms[room].renderTexture;
		SetMask(roomMask);
	}

	public void SetColorMask(Color color, float cutoff, float feathering)
	{
		quadMaterial.SetColor("_KeyColor", color);
		quadMaterial.SetFloat("_ColorCutoff", cutoff);
		quadMaterial.SetFloat("_ColorFeathering", cutoff + feathering);
	}

	public void ShowMasks(Texture2D[] textures, FilterMode filterMode = FilterMode.Point, float fps = 0f)
	{
		if (textures == null || textures[0] == null)
		{
			Debug.LogWarning("SetRoomMask(): texture is null");
			return;
		}
		maskTextures = textures;
		initialTime = AudioSettings.dspTime;
		base.gameObject.SetActive(value: true);
		base.enabled = true;
		maskFps = fps;
		for (int i = 0; i < textures.Length; i++)
		{
			textures[i].filterMode = filterMode;
		}
	}

	public void SetMaskAlphaMode(bool inverted)
	{
		quadMaterial.SetInt("_InvertMaskAlpha", inverted ? 1 : 0);
	}

	public void SetPremultiplyAlpha(bool enabled)
	{
		quadMaterial.SetInt("_Premultiply", enabled ? 1 : 0);
	}

	public void SetUsingMask(bool usingMask)
	{
		quadMaterial.SetInt("_UsingMask", usingMask ? 1 : 0);
	}

	public void SetUsingChromakey(bool usingChromakey)
	{
		quadMaterial.SetInt("_UsingChromaKey", usingChromakey ? 1 : 0);
	}

	public void RepositionRowsAndStrips(bool instant)
	{
		List<Row> list = new List<Row>(16);
		Row[] rows = base.game.rows;
		foreach (Row row in rows)
		{
			if (row.ent != null && row.ent.appearance != RowVisibilityMode.Hidden && !row.ent.customPosition)
			{
				list.Add(row);
			}
		}
		list = list.OrderBy((Row row2) => row2.ent.sortDepth).ToList();
		LevelBase currentLevel = base.game.currentLevel;
		int[] array = new int[3];
		List<Row> list2 = new List<Row>();
		if (GC.twoPlayerMode)
		{
			RDPlayer[] multiplayerRowOrder = GC.MultiplayerRowOrder;
			foreach (RDPlayer rDPlayer in multiplayerRowOrder)
			{
				foreach (Row item in list)
				{
					if (item.GetCurrentPlayer() == rDPlayer && item.room == index)
					{
						array[(int)rDPlayer]++;
						list2.Add(item);
					}
				}
				if (!currentLevel.multiroom && !currentLevel.dontAddEmptyStripsForPlayers && rDPlayer != RDPlayer.CPU && array[(int)rDPlayer] == 0)
				{
					array[(int)rDPlayer]++;
					list2.Add(null);
				}
			}
		}
		else
		{
			foreach (Row item2 in list)
			{
				if (item2.room == index)
				{
					list2.Add(item2);
				}
			}
		}
		if (currentLevel.multiroom && rowSeparationTiedToRoomHeight)
		{
			float num = 20f;
			float num2 = (float)((list2.Count - 1) * 42) + num;
			float num3 = renderQuadPivot.transform.localScale.y * RDBase.Vfx.RDHeight;
			verticalRowSpacing = ((num2 < num3) ? 42 : Mathf.RoundToInt((num3 - num) / (float)(list2.Count - 1)));
		}
		int num4 = 0;
		foreach (Row item3 in list2)
		{
			double num5 = (double)(RDBase.Vfx.RDHeight / 2f) + (double)verticalRowSpacing * 0.5 * (double)(list2.Count - 1) - (double)(verticalRowSpacing * num4);
			item3?.ent.Reposition((int)num5, instant);
			num4++;
		}
		int x = RDBase.Vfx.RDWidthInt / 2;
		if (GC.twoPlayerMode)
		{
			for (int num6 = 0; num6 < playerBounds.Length; num6++)
			{
				playerBounds[num6] = new Range(-10000f, 10000f);
			}
			int count = list2.Count;
			int num7 = verticalRowSpacing / 2;
			int num8 = (int)((double)(RDBase.Vfx.RDHeight / 2f) + (double)verticalRowSpacing * 0.5 * (double)(count - 1));
			int num9 = 0;
			for (int num10 = 0; num10 < playerBounds.Length; num10++)
			{
				playerBounds[num10] = new Range(-10000f, 10000f);
				if (num10 > 0)
				{
					playerBounds[num10].max = playerBounds[num10 - 1].min;
				}
				int num11 = array[num10];
				if (num11 == 0)
				{
					playerBounds[num10].min = playerBounds[num10].max;
				}
				else if (num9 + num11 == count)
				{
					playerBounds[num10].min = -10000f;
				}
				else if (num9 > 0)
				{
					playerBounds[num10].min = playerBounds[num10].max - (float)(num11 * verticalRowSpacing);
				}
				else
				{
					playerBounds[num10].min = num8 - (num11 - 1) * verticalRowSpacing - num7;
				}
				num9 += num11;
			}
			if (count > 0)
			{
				if (list2[0] == null)
				{
					return;
				}
				x = (int)list2[0].ent.transform.position.x;
			}
			int y = num7 + num8;
			int y2 = num7 + num8 - array[0] * verticalRowSpacing;
			int y3 = num7 + num8 - (array[0] + array[1]) * verticalRowSpacing;
			RepositionHorizontalStrips(x, y3, y2, y, instant);
		}
		else
		{
			int num12 = (int)(RDBase.Vfx.RDHeight / 2f);
			RepositionHorizontalStrips(x, num12, num12, num12, instant);
		}
	}

	private void UpdateHitStripPosition()
	{
		int num = xPlayerStrip;
		int num2 = 400;
		if (base.game.currentLevel is Level_Lesmis && rowContainer.childCount == 1)
		{
			int num3 = (int)firstRow.transform.position.y;
			int num4 = verticalRowSpacing / 2;
			num = (int)firstRow.transform.position.x;
			if (firstRow.row.multiplayer == RDPlayer.P1)
			{
				playerStripP1.SetSquareGlobal(num - num2, num3 - num4, num + num2, num3 + num4);
				playerStripP2.SetSquareGlobal(0, 0, 0, 0);
			}
			else
			{
				playerStripP1.SetSquareGlobal(0, 0, 0, 0);
				playerStripP2.SetSquareGlobal(num - num2, num3 - num4, num + num2, num3 + num4);
			}
		}
		else
		{
			playerStripP1.SetSquareGlobal(num - num2, yPlayerStripMiddle, num + num2, yPlayerStripTop);
			playerStripP2.SetSquareGlobal(num - num2, yPlayerStripBottom, num + num2, yPlayerStripMiddle);
		}
	}

	public void RepositionHorizontalStrips(int x0, int y1, int y2, int y3, bool instant = false)
	{
		xPlayerStrip = x0;
		if (topHorizontalStripTween != null)
		{
			topHorizontalStripTween.Kill();
		}
		if (middleHorizontalStripTween != null)
		{
			middleHorizontalStripTween.Kill();
		}
		if (bottomHorizontalStripTween != null)
		{
			bottomHorizontalStripTween.Kill();
		}
		topHorizontalStripTween = DOTween.To(() => yPlayerStripTop, (int num) =>
		{
			yPlayerStripTop = num;
		}, y3, RDCalibration.latency);
		middleHorizontalStripTween = DOTween.To(() => yPlayerStripMiddle, (int num) =>
		{
			yPlayerStripMiddle = num;
		}, y2, RDCalibration.latency);
		bottomHorizontalStripTween = DOTween.To(() => yPlayerStripBottom, (int num) =>
		{
			yPlayerStripBottom = num;
		}, y1, RDCalibration.latency);
	}

	public void SetHorizontalStripsVisible(bool visible)
	{
		playerStripP1.gameObject.SetActive(visible);
		playerStripP2.gameObject.SetActive(visible);
	}

	public bool HasAnyBeatWithinMargin(RDPlayer player)
	{
		Row[] rows = base.game.rows;
		foreach (Row row in rows)
		{
			if (row.room == index && row.GetCurrentPlayer() == player && row.hasBeatsWithinMargin)
			{
				return true;
			}
		}
		return false;
	}

	public void HideAllThemeBackgrounds()
	{
		foreach (List<Background> themeBackgrounds in themeBackgroundsList)
		{
			foreach (Background item in themeBackgrounds)
			{
				item.visible = false;
			}
		}
		coffeeShop?.SetActive(value: false);
		samuraiWard?.SetActive(value: false);
		train?.SetActive(value: false);
		garden?.SetActive(value: false);
		rooftop?.SetActive(value: false);
		tutorialBg?.SetActive(value: false);
		hospitalWard?.SetActive(value: false);
		basement?.SetActive(value: false);
		coleWard?.SetActive(value: false);
		boyWard?.SetActive(value: false);
		girlWard?.SetActive(value: false);
		rollerDisco?.SetActive(value: false);
		athleteWard?.SetActive(value: false);
		proceduralTree?.SetActive(value: false);
		recordsRoom?.SetActive(value: false);
		airport?.SetActive(value: false);
		abandonedWard?.SetActive(value: false);
		activeBackground = null;
	}

	public void DisableThemeGeneralEffects(bool paintRows = true)
	{
		if (paintRows)
		{
			RDBase.Vfx.SetSpriteMode(SpriteMode.None, Color.white, index);
		}
		camera.DisableComponent<CameraFilterPack_Atmosphere_Rain_Pro>();
		SetVignette(enabled: false);
		camBloom.enabled = false;
		if (emberBloom != null)
		{
			emberBloom.enabled = false;
		}
		if (crossBloom != null)
		{
			crossBloom.enabled = false;
		}
		kaleidoscopeMode = false;
		kaleidoscopeModeCross = false;
		kaleidoscopeModeEmbers = false;
		rotatingVoxelMode = false;
		spaceMode = false;
		camera.clearFlags = CameraClearFlags.Color;
		camera.GetComponent<ColorCorrectionCurves>().enabled = false;
		base.game.currentLevel.DisableRowReflections(index);
	}

	public void ShowThemeBackgrounds(RDTheme theme, bool disableEffects = true, bool hideOtherBackgrounds = true, bool affectRowContainers = true, bool firstRowOnFloorHack = false, bool paintRows = true, int variant = 0)
	{
		scrollTween.Kill();
		bool flag = samuraiWard != null && samuraiWard.activeSelf;
		if (scrolledPosition != 0f && (bool)activeBackground && currentTheme != theme)
		{
			ResetScroll(defaultPositions[currentTheme]);
			scrolledPosition = 0f;
		}
		if (hideOtherBackgrounds)
		{
			HideAllThemeBackgrounds();
		}
		if (disableEffects)
		{
			DisableThemeGeneralEffects(paintRows);
		}
		rowContainer.DOKill();
		{
			rowContainer.localPosition = Vector2.zero;
		}
		float num = 0f;
		float num2 = 0f;
		List<Background> backgroundList = null;
		currentEnvironment = null;
		currentTheme = theme;
		SetCustomBackgroundDepth(0);
		int num3;
		bool flag2;
		switch (theme)
		{
		case RDTheme.Intimate:
		case RDTheme.IntimateSimple:
			camera.backgroundColor = RDPresetColors.Peach.ARGBToColor();
			SetSpriteMode(SpriteMode.Glow, Color.black.WithAlpha(0.2f));
			SetVignette(enabled: true);
			if (variant == 1 || theme == RDTheme.IntimateSimple)
			{
				backgroundList = tblIntimateSimple;
			}
			else
			{
				camera.GetOrAddComponent<CameraFilterPack_Atmosphere_Rain_Pro>().enabled = true;
			}
			break;
		case RDTheme.InsomniacDay:
			if (variant == 0)
			{
				backgroundList = tblInsomniac;
				VideoBloom videoBloom3 = camBloom;
				videoBloom3.enabled = true;
				videoBloom3.Tint = 4284618239u.RGBAToColor();
				videoBloom3.Threshold = 0.4f;
				videoBloom3.KernelSize = 59f;
				videoBloom3.MasterAmount = 2.18f;
				SetSpriteMode(SpriteMode.Glow, Color.black.WithAlpha(0.6f));
				SetVignette(enabled: true);
				break;
			}
			goto case RDTheme.InsomniacNight;
		case RDTheme.InsomniacNight:
		{
			backgroundList = tblInsomniacN;
			VideoBloom videoBloom4 = camBloom;
			videoBloom4.enabled = true;
			videoBloom4.Tint = 641858559u.RGBAToColor();
			videoBloom4.Threshold = 0.14f;
			videoBloom4.KernelSize = 74.9f;
			videoBloom4.MasterAmount = 3.36f;
			SetVignette(enabled: true);
			SetSpriteMode(SpriteMode.Glow, Color.white.WithAlpha(0.6f));
			break;
		}
		case RDTheme.Matrix:
			backgroundList = tblMatrix;
			kaleidoscopeMode = false;
			kaleidoscopeModeEmbers = false;
			kaleidoscopeModeCross = false;
			foreach (Background item in backgroundList)
			{
				item.SetAlphaWithoutCoroutine(0f);
			}
			if (volTrackerMatrix == null)
			{
				volTrackerMatrix = scrVolumeTracker.Add((float s) =>
				{
					foreach (Background item2 in backgroundList)
					{
						item2.SetAlphaWithoutCoroutine(s);
					}
				}, 0f, 5f);
			}
			SetSpriteMode(SpriteMode.Glow, Color.white, 0.75f);
			break;
		case RDTheme.NeonMuseum:
			AddThemeFX(RDThemeFX.RowGlowWhite);
			backgroundList = tblNeonMuseum;
			break;
		case RDTheme.CrossesStraight:
		case RDTheme.CrossesFalling:
		{
			AddThemeFX(RDThemeFX.RowGlowWhite);
			kaleidoscopeModeCross = true;
			kaleidoscopeModeEmbers = true;
			emberBloom.enabled = true;
			embers.kaleidoCam.GetComponent<VideoBloom>().Tint = 3277018111u.RGBAToColor();
			camera.clearFlags = (RDBase.Vfx.infiniteZoom ? CameraClearFlags.Color : CameraClearFlags.Depth);
			mKaleidoscopeCross.kaleidoCam.depth = camera.depth - 1f;
			mKaleidoscopeCross.kaleidoCam.targetTexture = renderTexture;
			mKaleidoscopeCross.particleSystems[2].gameObject.SetActive(value: true);
			embers.particleSystems[0].gameObject.SetActive(value: true);
			bool flag4 = theme == RDTheme.CrossesFalling || variant == 1;
			mKaleidoscopeCross.particleSystems[0].gameObject.SetActive(!flag4);
			mKaleidoscopeCross.particleSystems[1].gameObject.SetActive(flag4);
			break;
		}
		case RDTheme.OrientalTechno:
		case RDTheme.OrientalTechnoClosed:
		{
			bool flag5 = base.conductor.isScrubbing || !flag;
			bool flag6 = variant == 1 || theme == RDTheme.OrientalTechnoClosed;
			RDSamuraiWard component4 = samuraiWard.GetComponent<RDSamuraiWard>();
			samuraiWard.SetActive(value: true);
			base.game.StartCoroutine(component4.OpenDoors(!flag6, flag5));
			if (flag5)
			{
				component4.MoveLanternsInstant(!flag6);
			}
			else
			{
				component4.ToggleLanternPositions(!flag6, fast: true);
			}
			RDLevelData data = base.game.currentLevel.data;
			if (data != null && data.settings.version <= 41)
			{
				component4.SetOldVersion(old: true);
			}
			break;
		}
		case RDTheme.TrainDay:
		case RDTheme.TrainNight:
		case RDTheme.DesertDay:
		case RDTheme.DesertNight:
		{
			train.SetActive(value: true);
			bool nightMode = variant == 1 || theme == RDTheme.TrainNight || theme == RDTheme.DesertNight;
			trainComp.SetNightMode(nightMode);
			bool activeTrain = theme == RDTheme.TrainDay || theme == RDTheme.TrainNight;
			trainComp.ToggleTrain(activeTrain);
			currentEnvironment = train.GetComponent<RDTrain>();
			activeBackground = train;
			break;
		}
		case RDTheme.Tutorial:
			camera.backgroundColor = 3803847935u.RGBAToColor();
			tutorialBg.SetActive(value: true);
			tutorialBg.GetComponent<RDTutorial>().SetNotesColor(3802275071u.RGBAToColor());
			SetSpriteMode(SpriteMode.Glow, Color.black.WithAlpha(0.2f));
			break;
		case RDTheme.Kaleidoscope:
			camera.backgroundColor = Color.black;
			kaleidoscopeMode = true;
			break;
		case RDTheme.PoliticiansRally:
			camera.backgroundColor = Color.black;
			backgroundList = tblPoliticiansRally;
			num2 = -20f;
			SetSpriteMode(SpriteMode.Outline, Color.black, 0.75f);
			break;
		case RDTheme.Rooftop:
		case RDTheme.RooftopSummer:
		case RDTheme.RooftopAutumn:
		{
			rooftop.SetActive(value: true);
			RDTheme theme2 = theme;
			if (theme == RDTheme.Rooftop)
			{
				switch (variant)
				{
				case 1:
					theme2 = RDTheme.RooftopSummer;
					break;
				case 2:
					theme2 = RDTheme.RooftopAutumn;
					break;
				}
			}
			rooftop.GetComponent<RDRooftop>().SetMode(RDRooftop.ModeFromTheme(theme2));
			currentEnvironment = rooftop.GetComponent<RDRooftop>();
			break;
		}
		case RDTheme.CubesFalling:
		case RDTheme.CubesFallingNiceBlue:
			AddThemeFX(RDThemeFX.RowGlowWhite);
			kaleidoscopeModeCross = true;
			camera.clearFlags = (RDBase.Vfx.infiniteZoom ? CameraClearFlags.Color : CameraClearFlags.Depth);
			mKaleidoscopeCross.kaleidoCam.depth = camera.depth - 1f;
			mKaleidoscopeCross.kaleidoCam.targetTexture = renderTexture;
			mKaleidoscopeCross.particleSystems[3].gameObject.SetActive(value: true);
			camera.backgroundColor = 3824741631u.RGBAToColor();
			if (variant == 1 || theme == RDTheme.CubesFallingNiceBlue)
			{
				mKaleidoscopeCross.particleSystems[0].gameObject.SetActive(value: true);
				embers.kaleidoCam.GetComponent<VideoBloom>().Tint = 160676095u.RGBAToColor();
				mKaleidoscopeCross.kaleidoCam.GetComponent<VideoBloom>().Tint = 10274303u.RGBAToColor();
				SetVignette(enabled: true);
			}
			break;
		case RDTheme.Sky:
			if (variant == 0)
			{
				backgroundList = tblSky;
				VideoBloom videoBloom2 = camBloom;
				videoBloom2.enabled = true;
				videoBloom2.Tint = 4284618239u.RGBAToColor();
				videoBloom2.Threshold = 0.4f;
				videoBloom2.KernelSize = 59f;
				videoBloom2.MasterAmount = 2.18f;
				SetSpriteMode(SpriteMode.Glow, Color.black.WithAlpha(0.6f));
				SetVignette(enabled: true);
				break;
			}
			goto case RDTheme.NightSky;
		case RDTheme.NightSky:
		{
			backgroundList = tblSky;
			VideoBloom videoBloom = camBloom;
			videoBloom.enabled = true;
			videoBloom.Tint = 641858559u.RGBAToColor();
			videoBloom.Threshold = 0.14f;
			videoBloom.KernelSize = 74.9f;
			videoBloom.MasterAmount = 3.36f;
			SetVignette(enabled: true);
			SetSpriteMode(SpriteMode.Glow, Color.white.WithAlpha(0.6f));
			break;
		}
		case RDTheme.BackAlley:
		{
			backgroundList = tblBackAlley;
			camera.GetOrAddComponent<ColorCorrectionCurves>().enabled = true;
			SetSpriteMode(SpriteMode.Glow, Color.white, 0.75f);
			VideoBloom videoBloom5 = camBloom;
			videoBloom5.enabled = true;
			videoBloom5.Tint = 4002800127u.RGBAToColor();
			videoBloom5.Threshold = 0.24f;
			videoBloom5.KernelSize = 42.9f;
			videoBloom5.MasterAmount = 0.94f;
			videoBloom5.MediumKernelScale = 1.9f;
			videoBloom5.LargeKernelScale = 3.8f;
			SetVignette(enabled: true);
			break;
		}
		case RDTheme.HallOfMirrors:
			camera.clearFlags = CameraClearFlags.Depth;
			break;
		case RDTheme.CoffeeShop:
		case RDTheme.CoffeeShopNight:
			coffeeShop.SetActive(value: true);
			coffeeShop.GetComponent<RDCoffeeShop>().SetNight(variant == 1 || theme == RDTheme.CoffeeShopNight);
			break;
		case RDTheme.Garden:
		case RDTheme.GardenNight:
			garden.SetActive(value: true);
			gardenComp.SetNightMode(variant == 1 || theme == RDTheme.GardenNight);
			base.game.currentLevel.EnableRowReflections(index);
			currentEnvironment = gardenComp;
			break;
		case RDTheme.HospitalWard:
		case RDTheme.HospitalWardNight:
		{
			hospitalWard.SetActive(value: true);
			RDMainWard component5 = hospitalWard.GetComponent<RDMainWard>();
			component5.SetNightMode(variant == 1 || theme == RDTheme.HospitalWardNight);
			component5.scrollRoom = index;
			int num6 = (index - 2) * hospitalRoomOffset;
			float num7 = 1024f + 3000f * (float)index;
			hospitalWard.transform.localPosition = new Vector3(num7 + (float)num6, 99f, -50f);
			defaultPositions[theme] = num7 + (float)(hospitalRoomOffset * -2);
			num2 = -40f;
			currentEnvironment = hospitalWard.GetComponent<RDMainWard>();
			activeBackground = hospitalWard;
			break;
		}
		case RDTheme.PaigeOffice:
		{
			hospitalWard.SetActive(value: true);
			RDMainWard component2 = hospitalWard.GetComponent<RDMainWard>();
			component2.GetComponent<RDMainWard>().SetNightMode(night: false);
			component2.scrollRoom = index;
			int num4 = 1024;
			int num5 = -630;
			hospitalWard.transform.localPosition = new Vector3((float)num4 + 3000f * (float)index + (float)num5, 99f, -50f);
			currentEnvironment = hospitalWard.GetComponent<RDMainWard>();
			activeBackground = hospitalWard;
			break;
		}
		case RDTheme.Basement:
			basement.SetActive(value: true);
			basement.GetComponent<RDBasement>().typingIan.SetActive(value: false);
			basement.GetComponent<RDBasement>().paige.SetActive(value: false);
			SetCustomBackgroundDepth(-100);
			currentEnvironment = basement.GetComponent<RDBasement>();
			activeBackground = basement;
			break;
		case RDTheme.ColeWardNight:
		case RDTheme.ColeWardSunrise:
			coleWard.SetActive(value: true);
			coleWard.GetComponent<RDHoodieBoyWard>().SetMode(variant == 0 && theme == RDTheme.ColeWardNight);
			break;
		case RDTheme.BoyWard:
			boyWard.SetActive(value: true);
			break;
		case RDTheme.GirlWard:
			girlWard.SetActive(value: true);
			break;
		case RDTheme.Skyline:
			num3 = ((variant == 0) ? 1 : 0);
			goto IL_0ce3;
		case RDTheme.SkylineBlue:
			num3 = 0;
			goto IL_0ce3;
		case RDTheme.RollerDisco:
		{
			rollerDisco.SetActive(value: true);
			RDRollerDisco component = rollerDisco.GetComponent<RDRollerDisco>();
			component.scrollSpeed = 0f;
			component.UpdatePosition();
			activeBackground = rollerDisco;
			break;
		}
		case RDTheme.Stadium:
		case RDTheme.StadiumStormy:
		case RDTheme.AthleteWard:
		case RDTheme.AthleteWardNight:
		{
			athleteWard.SetActive(value: true);
			RDAthleteWard component3 = athleteWard.GetComponent<RDAthleteWard>();
			bool flag3 = theme == RDTheme.Stadium || theme == RDTheme.StadiumStormy;
			component3.SetStadium(flag3);
			component3.SetNightMode((theme == RDTheme.AthleteWard && variant == 1) || theme == RDTheme.AthleteWardNight);
			component3.SetStorm((theme == RDTheme.Stadium && variant == 1) || theme == RDTheme.StadiumStormy, animate: false);
			num2 = -6f;
			currentEnvironment = (flag3 ? component3.stadiumEnvironment : component3);
			activeBackground = athleteWard;
			break;
		}
		case RDTheme.ProceduralTree:
			proceduralTree.SetActive(value: true);
			break;
		case RDTheme.RecordsRoom:
			recordsRoom.SetActive(value: true);
			activeBackground = recordsRoom;
			break;
		case RDTheme.Airport:
			airport.SetActive(value: true);
			activeBackground = airport;
			break;
		case RDTheme.AbandonedWard:
			abandonedWard.SetActive(value: true);
			activeBackground = abandonedWard;
			break;
		case RDTheme.FloatingHeart:
		case RDTheme.FloatingHeartWithCubes:
		case RDTheme.FloatingHeartBroken:
		case RDTheme.FloatingHeartBrokenWithCubes:
		{
			AddThemeFX(RDThemeFX.RowGlowWhite);
			this.rotatingVoxelMode = true;
			camera.clearFlags = (RDBase.Vfx.infiniteZoom ? CameraClearFlags.Color : CameraClearFlags.Depth);
			voxelObj.kaleidoCam.depth = camera.depth - 1f;
			voxelObj.kaleidoCam.targetTexture = renderTexture;
			RotatingVoxelMode rotatingVoxelMode = ((theme != RDTheme.FloatingHeart && theme != RDTheme.FloatingHeartWithCubes) ? RotatingVoxelMode.BrokenHeart : RotatingVoxelMode.Heart);
			voxelObj.SetVoxelMode(rotatingVoxelMode);
			bool cubeMode = variant == 1 || theme == RDTheme.FloatingHeartWithCubes || theme == RDTheme.FloatingHeartBrokenWithCubes;
			voxelObj.SetCubeMode(cubeMode);
			camera.backgroundColor = 570425599u.RGBAToColor();
			break;
		}
		case RDTheme.ZenGarden:
			this.rotatingVoxelMode = true;
			camera.clearFlags = (RDBase.Vfx.infiniteZoom ? CameraClearFlags.Color : CameraClearFlags.Depth);
			voxelObj.kaleidoCam.depth = camera.depth - 1f;
			voxelObj.kaleidoCam.targetTexture = renderTexture;
			voxelObj.SetVoxelMode(RotatingVoxelMode.ZenGarden);
			voxelObj.SetCubeMode(enableCubes: false);
			break;
		case RDTheme.Space:
			spaceMode = true;
			if (!spaceBackground.isCloseup)
			{
				spaceBackground.mainCam.depth = camera.depth - 2f;
				spaceBackground.mainCam.targetTexture = renderTexture;
				spaceBackground.starsCam.depth = camera.depth - 1f;
				spaceBackground.starsCam.targetTexture = renderTexture;
			}
			break;
		case RDTheme.Vaporwave:
			{
				backgroundList = tblVaporwave;
				break;
			}
			IL_0ce3:
			flag2 = (byte)num3 != 0;
			backgroundList = (flag2 ? tblSkyline : tblSkylineBlue);
			break;
		}
		if (affectRowContainers)
		{
			SetRowContainerX(num);
			SetRowContainerY(num2);
		}
		if (firstRowOnFloorHack)
		{
			RowEntity rowEntity = firstRow;
			if (rowEntity != null)
			{
				rowEntity.positionTweenX.Kill();
				rowEntity.positionTweenY.Kill();
				rowEntity.customPosition = true;
				rowEntity.transform.LocalMoveXY(35f + num, RDBase.Vfx.RDHeight / 2f + num2);
				RepositionRowsAndStrips(instant: false);
			}
		}
		if (backgroundList == null)
		{
			return;
		}
		foreach (Background item3 in backgroundList)
		{
			item3.visible = true;
		}
		void SetSpriteMode(SpriteMode mode, Color color, float glowPower = 1f)
		{
			if (paintRows)
			{
				RDBase.Vfx.SetSpriteMode(mode, color, index, glowPower);
			}
		}
	}

	public void PreloadTheme(RDTheme theme, int variant = 0)
	{
		if (theme == RDTheme.InsomniacDay && variant == 1)
		{
			theme = RDTheme.InsomniacNight;
		}
		if (theme == RDTheme.Skyline && variant == 1)
		{
			theme = RDTheme.SkylineBlue;
		}
		if (preloadedThemes.Contains(theme))
		{
			return;
		}
		List<Background> list = new List<Background>();
		switch (theme)
		{
		case RDTheme.Intimate:
			if (variant == 1)
			{
				PreloadTheme(RDTheme.IntimateSimple);
				return;
			}
			break;
		case RDTheme.IntimateSimple:
			if (tblIntimateSimple.Count == 0)
			{
				list = tblIntimateSimple;
				ptIntimateSimpleRain = Background.Create("IntimateRain", 1000f, 1000f, isForeground: false, index).SetAlpha(0.2f);
				tblIntimateSimple.Add(ptIntimateSimpleRain);
			}
			break;
		case RDTheme.InsomniacDay:
			if (tblInsomniac.Count == 0)
			{
				printe("Preloading insom day...");
				list = tblInsomniac;
				ptInsomniacSky = Background.CreateSingle("SkySS", index);
				ptInsomniacTrees = Background.CreateSingle("TreesSS", index).SetFPS(1.5f);
				ptInsomniacPagoda = Background.CreateSingle("PagodaSS", index);
				ptInsomniacPagoda.updates = false;
				ptInsomniacSakuraTree = Background.CreateSingle("SakuraTreeSS", new int[6] { 0, 1, 2, 3, 0, 1 }, 9f, index).SetFollowBeat(FollowBeatType.HeadbobStyle);
				tblInsomniac.Add(ptInsomniacSky);
				tblInsomniac.Add(ptInsomniacTrees);
				tblInsomniac.Add(ptInsomniacPagoda);
				tblInsomniac.Add(ptInsomniacSakuraTree);
			}
			break;
		case RDTheme.InsomniacNight:
			if (tblInsomniacN.Count == 0)
			{
				float fps = 1.5f;
				list = tblInsomniacN;
				ptInsomniacNightSky = Background.CreateSingle("InsomniacHard_Background", index);
				ptInsomniacNStars = Background.CreateSingle("InsomniacHard_Stars", new int[5] { 0, 1, 2, 3, 4 }, fps, index);
				ptInsomniacNPagoda = Background.CreateSingle("InsomniacHard_PagodaUnlit", index);
				ptInsomniacNPagodaLights = Background.CreateSingle("InsomniacHard_PagodaGlowAndLights", index);
				ptInsomniacNBat = Background.CreateSingle("InsomniacHard_Bat", new int[5] { 0, 1, 2, 3, 4 }, 8f, index);
				ptInsomniacNCat = Background.CreateSingle("InsomniacHard_Cat", new int[8] { 0, 1, 2, 3, 4, 3, 2, 1 }, 6f, index);
				ptInsomniacNPetals = Background.CreateSingle("InsomniacHard_FrontPetals", index);
				tblInsomniacN.Add(ptInsomniacNightSky);
				tblInsomniacN.Add(ptInsomniacNStars);
				tblInsomniacN.Add(ptInsomniacNPagoda);
				tblInsomniacN.Add(ptInsomniacNPagodaLights);
				tblInsomniacN.Add(ptInsomniacNPetals);
				tblInsomniacN.Add(ptInsomniacNBat);
				tblInsomniacN.Add(ptInsomniacNCat);
			}
			break;
		case RDTheme.Matrix:
			if (tblMatrix.Count == 0)
			{
				ptMatrix1 = Background.Create("Matrix/0", 0f, 1f, isForeground: false, index).SetBackgroundOrder(5).SetScale(0.75f);
				ptMatrix2 = Background.Create("Matrix/1", 0f, 2f, isForeground: false, index).SetBackgroundOrder(6).SetScale(1.5f);
				ptMatrix3 = Background.Create("Matrix/2", 0f, 3f, isForeground: false, index).SetBackgroundOrder(7).SetScale(2.25f);
				tblMatrix.Add(ptMatrix1);
				tblMatrix.Add(ptMatrix2);
				tblMatrix.Add(ptMatrix3);
				ptMatrixAlt1 = Background.Create("MatrixAlt/0", 0f, 1f, isForeground: false, index).SetBackgroundOrder(5).SetScale(0.75f);
				ptMatrixAlt2 = Background.Create("MatrixAlt/1", 0f, 2f, isForeground: false, index).SetBackgroundOrder(6).SetScale(1.5f);
				ptMatrixAlt3 = Background.Create("MatrixAlt/2", 0f, 3f, isForeground: false, index).SetBackgroundOrder(7).SetScale(2.25f);
				tblMatrixAlt.Add(ptMatrixAlt1);
				tblMatrixAlt.Add(ptMatrixAlt2);
				tblMatrixAlt.Add(ptMatrixAlt3);
				list = tblMatrix.Concat(tblMatrixAlt).ToList();
			}
			break;
		case RDTheme.NeonMuseum:
			if (tblNeonMuseum.Count == 0)
			{
				list = tblNeonMuseum;
				ptNeonMuseumBg = Background.CreateSingle("NeonMuseum", index);
				tblNeonMuseum.Add(ptNeonMuseumBg);
			}
			break;
		case RDTheme.CubesFalling:
			if (variant == 1)
			{
				PreloadTheme(RDTheme.CubesFallingNiceBlue);
			}
			return;
		case RDTheme.CrossesStraight:
		case RDTheme.CrossesFalling:
		case RDTheme.CubesFallingNiceBlue:
			PreloadEmbers();
			break;
		case RDTheme.OrientalTechno:
		case RDTheme.OrientalTechnoClosed:
		{
			bool flag = variant == 1 || theme == RDTheme.OrientalTechnoClosed;
			RDSamuraiWard rDSamuraiWard = base.game.currentLevel.LoadLevelAsset<RDSamuraiWard>();
			rDSamuraiWard.legacyLanternBehavior = false;
			rDSamuraiWard.MoveLanternsInstant(!flag);
			samuraiWard = rDSamuraiWard.gameObject;
			Vector3 localPosition = new Vector3(176f + 3000f * (float)index, 99f);
			rDSamuraiWard.transform.localPosition = localPosition;
			if (!flag)
			{
				base.game.StartCoroutine(rDSamuraiWard.OpenDoors(opened: true, instant: true));
			}
			break;
		}
		case RDTheme.TrainDay:
		case RDTheme.TrainNight:
		case RDTheme.DesertDay:
		case RDTheme.DesertNight:
		{
			RDTrain rDTrain = base.game.currentLevel.LoadLevelAsset<RDTrain>();
			train = rDTrain.gameObject;
			trainComp = train.GetComponent<RDTrain>();
			train.transform.localPosition = new Vector3(3000f * (float)index, 130f);
			rDTrain.rowContainer = rowContainer;
			break;
		}
		case RDTheme.Tutorial:
		{
			RDTutorial rDTutorial = base.game.currentLevel.LoadLevelAsset<RDTutorial>();
			tutorialBg = rDTutorial.gameObject;
			tutorialBg.transform.localPosition = new Vector3(176f + 3000f * (float)index, 99f);
			break;
		}
		case RDTheme.Kaleidoscope:
			kaleidoscopeMode = false;
			break;
		case RDTheme.PoliticiansRally:
			if (tblPoliticiansRally.Count == 0)
			{
				list = tblPoliticiansRally;
				ptPoliticiansRally = Background.Create("PoliticianBackground", new int[2] { 0, 1 }, 0f, 0f, 0f, isForeground: false, index).SetBackgroundOrder(10).SetAlpha(1f)
					.SetFixedPosition()
					.SetFollowCamera()
					.SetFollowBeat(FollowBeatType.HeadbobStyle);
				tblPoliticiansRally.Add(ptPoliticiansRally);
			}
			break;
		case RDTheme.Rooftop:
		case RDTheme.RooftopSummer:
		case RDTheme.RooftopAutumn:
			rooftop = UnityEngine.Object.Instantiate(base.gc.rooftop, base.gm.parentContainer);
			rooftop.transform.localPosition = new Vector3(rooftop.transform.localPosition.x + 3000f * (float)index, 99f, -50f);
			break;
		case RDTheme.BackAlley:
			if (tblBackAlley.Count == 0)
			{
				list = tblBackAlley;
				ptBackAlleyBackground = Background.CreateSingle("Boss2_Background", index);
				ptBackAlleyWindows = Background.CreateSingle("Boss2_Windows_and_Reflection", new int[3] { 0, 1, 2 }, 0.25f, index);
				ptBackAlleyCloud1 = Background.Create("Boss2_Cloud_1", 0.1f, 0f, isForeground: false, index).SetTileType(TileType.Horizontal);
				ptBackAlleyCloud2 = Background.Create("Boss2_Cloud_2", 0.5f, 0f, isForeground: false, index).SetTileType(TileType.Horizontal);
				ptBackAlleyCloud3 = Background.Create("Boss2_Cloud_3", 0.2f, 0f, isForeground: false, index).SetTileType(TileType.Horizontal);
				ptBackAlleyCloud4 = Background.Create("Boss2_Cloud_4", 0.3f, 0f, isForeground: false, index).SetTileType(TileType.Horizontal);
				ptBackAlleyForeground = Background.CreateSingle("Boss2_Foreground", index);
				ptBackAlleyClubSign = Background.CreateSingle("Boss2_Club_Sign/1", index);
				ptBackAlleyCafeSign = Background.CreateSingle("Boss2_Cafe_Sign/1", index);
				ptBackAlleyLamppostGlow = Background.CreateSingle("Boss2_Lamppost_Glow", new int[26]
				{
					0, 0, 0, 0, 0, 0, 0, 0, 1, 0,
					0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
					0, 1, 0, 0, 0, 0
				}, 9f, index);
				ptBackAlleyClubSignOn = Background.CreateSingle("Boss2_Club_Sign/0", index);
				ptBackAlleyCafeSignOn = Background.CreateSingle("Boss2_Cafe_Sign/0", index);
				tblBackAlley.Add(ptBackAlleyBackground);
				tblBackAlley.Add(ptBackAlleyWindows);
				tblBackAlley.Add(ptBackAlleyCloud1);
				tblBackAlley.Add(ptBackAlleyCloud2);
				tblBackAlley.Add(ptBackAlleyCloud3);
				tblBackAlley.Add(ptBackAlleyCloud4);
				tblBackAlley.Add(ptBackAlleyForeground);
				tblBackAlley.Add(ptBackAlleyCafeSign);
				tblBackAlley.Add(ptBackAlleyClubSign);
				tblBackAlley.Add(ptBackAlleyLamppostGlow);
				tblBackAlley.Add(ptBackAlleyCafeSignOn);
				tblBackAlley.Add(ptBackAlleyClubSignOn);
				ptBackAlleyCafeSignOn.SetAlpha(0f);
				ptBackAlleyCafeSignOn.SetAlpha(0f);
			}
			break;
		case RDTheme.Sky:
			if (variant == 1)
			{
				PreloadTheme(RDTheme.NightSky);
				return;
			}
			if (tblSky.Count == 0)
			{
				list = tblSky;
				ptInsomniacSky = Background.CreateSingle("SkySS", index);
				tblSky.Add(ptInsomniacSky);
			}
			break;
		case RDTheme.NightSky:
			if (tblSky.Count == 0)
			{
				list = tblSky;
				ptInsomniacSky = Background.CreateSingle("SkySS", index);
				tblSky.Add(ptInsomniacSky);
			}
			break;
		case RDTheme.CoffeeShop:
		case RDTheme.CoffeeShopNight:
			coffeeShop = UnityEngine.Object.Instantiate(base.gc.coffeeShop, base.gm.parentContainer);
			coffeeShop.transform.LocalMoveX(coffeeShop.transform.localPosition.x + 3000f * (float)index);
			break;
		case RDTheme.Garden:
		case RDTheme.GardenNight:
			garden = UnityEngine.Object.Instantiate(base.gc.garden, base.gm.parentContainer);
			garden.transform.LocalMoveX(3000f * (float)index);
			gardenComp = garden.GetComponent<RDGarden>();
			break;
		case RDTheme.HospitalWard:
		case RDTheme.HospitalWardNight:
		case RDTheme.PaigeOffice:
		{
			RDMainWard rDMainWard = base.game.currentLevel.LoadLevelAsset<RDMainWard>();
			hospitalWard = rDMainWard.gameObject;
			rDMainWard.SetNightMode(night: false);
			if (scnGame.levelToLoadSource != LevelSource.CutscenesPath)
			{
				rDMainWard.ShowVendingMachine(show: false);
			}
			rDMainWard.sleepingPaige.SetActive(value: false);
			break;
		}
		case RDTheme.Basement:
			basement = base.game.currentLevel.LoadLevelAsset<RDBasement>().gameObject;
			basement.transform.LocalMoveXY(3000f * (float)index + 320f, RDBase.Vfx.RDHeight / 2f);
			break;
		case RDTheme.ColeWardNight:
		case RDTheme.ColeWardSunrise:
		{
			RDHoodieBoyWard rDHoodieBoyWard = base.game.currentLevel.LoadLevelAsset<RDHoodieBoyWard>();
			rDHoodieBoyWard.room = this;
			rDHoodieBoyWard.transform.LocalMoveX(3000f * (float)index);
			coleWard = rDHoodieBoyWard.gameObject;
			break;
		}
		case RDTheme.BoyWard:
		{
			RDBoyWard rDBoyWard = base.game.currentLevel.LoadLevelAsset<RDBoyWard>();
			rDBoyWard.windowGirl.SetActive(value: false);
			rDBoyWard.transform.LocalMoveXY(RDBase.Vfx.RDWidth / 2f + 3000f * (float)index, RDBase.Vfx.RDHeight / 2f);
			boyWard = rDBoyWard.gameObject;
			break;
		}
		case RDTheme.GirlWard:
		{
			RDGirlWard rDGirlWard = base.game.currentLevel.LoadLevelAsset<RDGirlWard>();
			rDGirlWard.windowBoy.SetActive(value: false);
			rDGirlWard.transform.LocalMoveXY(RDBase.Vfx.RDWidth / 2f + 3000f * (float)index, RDBase.Vfx.RDHeight / 2f);
			girlWard = rDGirlWard.gameObject;
			break;
		}
		case RDTheme.Skyline:
			if (tblSkyline.Count == 0)
			{
				list = tblSkyline;
				ptSkylineSky = Background.Create("Intimate_SkySS", base.gc.intimate_skyScrollX, 0f, isForeground: false, index);
				ptSkylineBack = Background.Create("Intimate_CityBackSS", base.gc.intimate_cityBackScrollX, 0f, isForeground: false, index).SetTileType(TileType.Horizontal);
				ptSkylineFront = Background.Create("Intimate_CityFrontSS", base.gc.intimate_cityFrontScrollX, 0f, isForeground: false, index).SetTileType(TileType.Horizontal);
				tblSkyline.Add(ptSkylineSky);
				tblSkyline.Add(ptSkylineBack);
				tblSkyline.Add(ptSkylineFront);
			}
			break;
		case RDTheme.SkylineBlue:
			if (tblSkylineBlue.Count == 0)
			{
				list = tblSkylineBlue;
				ptSkylineSkyBlue = Background.Create("Intimate_SkySS_Blue/0", base.gc.intimate_skyScrollX, 0f, isForeground: false, index);
				ptSkylineBackBlue = Background.Create("Intimate_CityBackSS_Blue/0", base.gc.intimate_cityBackScrollX, 0f, isForeground: false, index).SetTileType(TileType.Horizontal);
				ptSkylineFrontBlue = Background.Create("Intimate_CityFrontSS_Blue/0", base.gc.intimate_cityFrontScrollX, 0f, isForeground: false, index).SetTileType(TileType.Horizontal);
				tblSkylineBlue.Add(ptSkylineSkyBlue);
				tblSkylineBlue.Add(ptSkylineBackBlue);
				tblSkylineBlue.Add(ptSkylineFrontBlue);
			}
			break;
		case RDTheme.RollerDisco:
		{
			RDRollerDisco rDRollerDisco = base.game.currentLevel.LoadLevelAsset<RDRollerDisco>();
			rDRollerDisco.transform.LocalMoveX(3000f * (float)index);
			rollerDisco = rDRollerDisco.gameObject;
			break;
		}
		case RDTheme.Stadium:
		case RDTheme.StadiumStormy:
		case RDTheme.AthleteWard:
		case RDTheme.AthleteWardNight:
			athleteWard = base.game.currentLevel.LoadLevelAsset<RDAthleteWard>().gameObject;
			athleteWard.transform.LocalMoveX(athleteWard.transform.localPosition.x + 3000f * (float)index);
			break;
		case RDTheme.ProceduralTree:
		{
			RDProceduralTree rDProceduralTree = base.game.currentLevel.LoadLevelAsset<RDProceduralTree>();
			rDProceduralTree.transform.LocalMoveX(rDProceduralTree.transform.localPosition.x + 3000f * (float)index);
			rDProceduralTree.InitRNG(index);
			rDProceduralTree.InitRenderTexture();
			proceduralTree = rDProceduralTree.gameObject;
			break;
		}
		case RDTheme.Space:
			spaceBackground = base.game.currentLevel.LoadLevelAsset<RDSpaceBackground>();
			spaceBackground.transform.LocalMoveX(3000f * (float)index);
			break;
		case RDTheme.RecordsRoom:
		{
			RDRecordsRoom rDRecordsRoom = base.game.currentLevel.LoadLevelAsset<RDRecordsRoom>();
			rDRecordsRoom.transform.LocalMoveX(3000f * (float)index);
			recordsRoom = rDRecordsRoom.gameObject;
			break;
		}
		case RDTheme.Airport:
		{
			RDAirport rDAirport = base.game.currentLevel.LoadLevelAsset<RDAirport>();
			int num = 300;
			rDAirport.transform.LocalMoveX((float)num + 3000f * (float)index);
			airport = rDAirport.gameObject;
			break;
		}
		case RDTheme.AbandonedWard:
		{
			RDAbandonedWard rDAbandonedWard = base.game.currentLevel.LoadLevelAsset<RDAbandonedWard>();
			int num = 500;
			rDAbandonedWard.transform.LocalMoveX((float)num + 3000f * (float)index);
			abandonedWard = rDAbandonedWard.gameObject;
			break;
		}
		case RDTheme.Vaporwave:
			if (tblVaporwave.Count == 0)
			{
				list = tblVaporwave;
				ptVaporwave = Background.Create("Vaporwave_BG", new int[6] { 0, 1, 2, 3, 4, 5 }, 10f, 0f, 0f, isForeground: false, index).SetBackgroundOrder(10).SetAlpha(1f)
					.SetFixedPosition()
					.SetFollowCamera();
				tblVaporwave.Add(ptVaporwave);
			}
			break;
		}
		if (list.Count != 0)
		{
			themeBackgroundsList.Add(list);
		}
		preloadedThemes.Add(theme);
		HideAllThemeBackgrounds();
	}

	public void ScrollTheme(float xOffset, float duration, Ease ease)
	{
		if (!activeBackground || !RDEditorConstants.EditorThemeBounds.TryGetValue(currentTheme, out var value))
		{
			return;
		}
		if (!defaultPositions.TryGetValue(currentTheme, out var originalPos))
		{
			originalPos = activeBackground.transform.localPosition.x;
		}
		defaultPositions[currentTheme] = originalPos;
		float num = Mathf.Clamp(xOffset, value.x, value.y);
		float endValue = originalPos - num;
		if (scrolledPosition != 0f && duration > 0f)
		{
			activeBackground.transform.LocalMoveX(originalPos - scrolledPosition);
		}
		if (duration > 0f)
		{
			scrollTween = activeBackground.transform.DOLocalMoveX(endValue, duration).SetEase(ease).OnUpdate(() =>
			{
				scrolledPosition = originalPos - activeBackground.transform.localPosition.x;
			});
		}
		else
		{
			scrolledPosition = num;
			activeBackground.transform.LocalMoveX(endValue);
		}
	}

	public void TryResetScroll()
	{
		if ((bool)activeBackground && defaultPositions.TryGetValue(currentTheme, out var value))
		{
			ResetScroll(value);
		}
	}

	public void ResetScroll(float pos)
	{
		RDTheme rDTheme = currentTheme;
		if (rDTheme == RDTheme.HospitalWard || rDTheme == RDTheme.HospitalWardNight)
		{
			pos += (float)(index * hospitalRoomOffset);
		}
		activeBackground.transform.LocalMoveX(pos);
	}

	public void PreloadFX()
	{
		if (preloadedVFX.Contains(RDThemeFX.FallingPetals) || preloadedVFX.Contains(RDThemeFX.FallingPetalsInstant))
		{
			petals1 = RDBase.Vfx.ShowParticlePreset("petals1", -1f, index);
			petals2 = RDBase.Vfx.ShowParticlePreset("petals2", -1f, index);
			petals3 = RDBase.Vfx.ShowParticlePreset("petals3", -1f, index);
		}
		foreach (RDThemeFX item in preloadedVFX)
		{
			switch (item)
			{
			case RDThemeFX.BassDropOnHit:
				fxBassDrop = scrExecuteOnHit.Add(() =>
				{
					RDBase.Vfx.BassDropNew(index);
				}, HitType.Hit, persistent: true, autoDelete: false, -1, overwrite: false);
				break;
			case RDThemeFX.VignetteFlicker:
				fxVignetteFlicker = scrVolumeTracker.Add((float s) =>
				{
					SetVignetteAlpha(1f - s);
				}, 0f, 1f);
				break;
			case RDThemeFX.ShakeOnHit:
				fxShakeOnHit = scrExecuteOnHit.Add(null, HitType.Hit, persistent: true);
				fxShakeOnHit.action = () =>
				{
					camera.Shake(3, 2);
				};
				break;
			case RDThemeFX.ShakeOnHeartBeat:
				fxShakeOnHB = scrExecuteOnEveryHB.Add(null);
				fxShakeOnHB.action = () =>
				{
					camera.Shake(3, 2);
				};
				if (base.game.currentLevel.smoothShake)
				{
					fxShakeOnHB.action = () =>
					{
						camera.transform.DOShakePosition(0.3f, 2f);
					};
				}
				break;
			case RDThemeFX.SilhouettesOnHBeat:
				fxSilhouetteChangeOnHB = scrExecuteOnEveryHB.Add(() =>
				{
					RDBase.Vfx.CycleOverlayColor(index);
				});
				break;
			case RDThemeFX.GlitchObstruction:
				fxGlitch = Background.CreateSingle("Glitch", new int[2] { 0, 1 }, base.game.reducedFlash ? 8f : 60f, index, isForeground: true);
				fxGlitch.visible = false;
				break;
			case RDThemeFX.Noise:
				fxNoise = Background.CreateSingle("Noise", new int[4] { 0, 1, 2, 3 }, base.game.reducedFlash ? 8f : 60f, index, isForeground: true);
				fxNoise.visible = false;
				break;
			case RDThemeFX.Confetti:
				confetti = RDBase.Vfx.ShowParticlePreset("confetti", -1f, index);
				break;
			case RDThemeFX.ConfettiBurst:
				confettiBurstL = RDBase.Vfx.ShowParticlePreset("confettiBurstL", -1f, index);
				confettiBurstR = RDBase.Vfx.ShowParticlePreset("confettiBurstR", -1f, index);
				break;
			case RDThemeFX.FallingPetalsSnow:
				petalsSnow1 = RDBase.Vfx.ShowParticlePreset("petalsSnow1", -1f, index);
				petalsSnow2 = RDBase.Vfx.ShowParticlePreset("petalsSnow2", -1f, index);
				petalsSnow3 = RDBase.Vfx.ShowParticlePreset("petalsSnow3", -1f, index);
				break;
			case RDThemeFX.FallingLeaves:
				leaves1 = RDBase.Vfx.ShowParticlePreset("leaves1", -1f, index);
				leaves2 = RDBase.Vfx.ShowParticlePreset("leaves2", -1f, index);
				leaves3 = RDBase.Vfx.ShowParticlePreset("leaves3", -1f, index);
				break;
			case RDThemeFX.Snow:
				snowParticles = UnityEngine.Object.Instantiate(RDBase.Vfx.snowPrefab, base.gm.parentContainer).GetComponent<ParticleSystem>();
				snowParticles.transform.TranslateX(3000f * (float)index);
				break;
			case RDThemeFX.Matrix:
				PreloadTheme(RDTheme.Matrix);
				break;
			case RDThemeFX.LightStripVert:
				LoadLightStrip();
				break;
			case RDThemeFX.Diamonds:
				diamondParticles = UnityEngine.Object.Instantiate(RDBase.Vfx.diamondsParticleSystemPrefab, base.gm.parentContainer).GetComponent<ParticleSystem>();
				diamondParticles.transform.TranslateX(3000f * (float)index);
				break;
			case RDThemeFX.Tutorial:
				ptTutorialNotes = Background.Create("many_notes_tile", 0f, 0f, isForeground: false, index).SetBackgroundOrder(3);
				ptTutorialNotes.visible = false;
				break;
			case RDThemeFX.Balloons:
				sparklineBalloons = UnityEngine.Object.Instantiate(RDBase.Vfx.sparklineBalloonsPrefab, base.gm.parentContainer).GetComponent<SparklineBalloons>();
				sparklineBalloons.transform.TranslateX(3000f * (float)index);
				break;
			case RDThemeFX.Embers:
				emberVFXParticles = RDBase.Vfx.ShowParticlePreset("embersVFX", -1f, index);
				break;
			case RDThemeFX.GlassShatter:
			{
				GameObject gameObject = base.game.currentLevel.LoadPublicPrefab("ShatteredGlass/ShatteredGlass");
				glass = gameObject.GetComponent<ShatteredGlass>();
				glass.gameObject.SetActive(value: false);
				break;
			}
			case RDThemeFX.EyesBig:
			case RDThemeFX.EyesSmall:
			{
				bool flag = item == RDThemeFX.EyesSmall;
				RDManyEyes rDManyEyes = base.game.currentLevel.LoadLevelAsset<RDManyEyes>();
				rDManyEyes.Setup(flag);
				rDManyEyes.transform.LocalMoveXY(3000f * (float)index + RDBase.Vfx.RDWidth / 2f, RDBase.Vfx.RDHeight / 2f);
				rDManyEyes.gameObject.SetActive(value: false);
				if (flag)
				{
					eyesSmall = rDManyEyes;
				}
				else
				{
					eyesBig = rDManyEyes;
				}
				break;
			}
			}
		}
		DisableAllThemeFX();
	}

	public void DisableAllThemeFX()
	{
		foreach (RDThemeFX item in preloadedVFX)
		{
			DisableThemeFX(item);
		}
	}

	public void DisableThemeFX(RDThemeFX fx)
	{
		usedThemeFX.Remove(fx);
		switch (fx)
		{
		case RDThemeFX.WavyRows:
			wavyRows = false;
			break;
		case RDThemeFX.Vignette:
			SetVignette(enabled: false);
			break;
		case RDThemeFX.Tile2:
		case RDThemeFX.Tile3:
		case RDThemeFX.Tile4:
		case RDThemeFX.TileN:
			RDBase.Vfx.SetTileScreen(new Vector2(1f, 1f), index);
			break;
		case RDThemeFX.LightStripVert:
			beatboxVerticalStrip = false;
			break;
		case RDThemeFX.ColourfulShockwaves:
			colorfulShockwaves = false;
			break;
		case RDThemeFX.VignetteFlicker:
			SetVignette(enabled: false);
			fxVignetteFlicker.isActive = false;
			break;
		case RDThemeFX.ScreenScroll:
		case RDThemeFX.ScreenScrollX:
			camera.GetOrAddComponent<CameraFilterPack_TV_VHS>().enabled = false;
			break;
		case RDThemeFX.ScreenScrollSansVHS:
		case RDThemeFX.ScreenScrollXSansVHS:
		case RDThemeFX.CustomScreenScroll:
		{
			CameraFilterPack_TV_VHS orAddComponent2 = camera.GetOrAddComponent<CameraFilterPack_TV_VHS>();
			orAddComponent2.Calibrage = 0f;
			orAddComponent2.CalibrageX = 0f;
			if (orAddComponent2.VHSEffectDisabled == 1f)
			{
				orAddComponent2.enabled = false;
			}
			break;
		}
		case RDThemeFX.VHS:
		{
			CameraFilterPack_TV_VHS orAddComponent = camera.GetOrAddComponent<CameraFilterPack_TV_VHS>();
			orAddComponent.VHSEffectDisabled = 1f;
			if (orAddComponent.Calibrage == 0f && orAddComponent.CalibrageX == 0f)
			{
				orAddComponent.enabled = false;
			}
			break;
		}
		case RDThemeFX.VHSRewind:
			camera.GetOrAddComponent<CameraFilterPack_TV_VHS_Rewind>().enabled = false;
			break;
		case RDThemeFX.BassDropOnHit:
			if (fxBassDrop != null)
			{
				fxBassDrop.isActive = false;
			}
			break;
		case RDThemeFX.ShakeOnHeartBeat:
			fxShakeOnHB.isActive = false;
			break;
		case RDThemeFX.ShakeOnHit:
			fxShakeOnHit.isActive = false;
			break;
		case RDThemeFX.SilhouettesOnHBeat:
			fxSilhouetteChangeOnHB.isActive = false;
			break;
		case RDThemeFX.CutsceneMode:
			if (base.game.started)
			{
				base.game.SetGameState(GameState.Handmode);
			}
			break;
		case RDThemeFX.Blackout:
			RDBase.Vfx.BlackScreenOff(index);
			break;
		case RDThemeFX.Noise:
			fxNoise.visible = false;
			break;
		case RDThemeFX.GlitchObstruction:
			fxGlitch.visible = false;
			break;
		case RDThemeFX.Rain:
			camera.GetOrAddComponent<CameraFilterPack_Atmosphere_Rain_Pro>().enabled = false;
			break;
		case RDThemeFX.Matrix:
			foreach (Background item in tblMatrix)
			{
				item.visible = false;
			}
			{
				foreach (Background item2 in tblMatrixAlt)
				{
					item2.visible = false;
				}
				break;
			}
		case RDThemeFX.MiawMiaw:
			base.game.nurseCount = false;
			break;
		case RDThemeFX.Confetti:
			confetti.gameObject.SetActive(value: false);
			break;
		case RDThemeFX.ConfettiBurst:
			confettiBurstL.gameObject.SetActive(value: false);
			confettiBurstR.gameObject.SetActive(value: false);
			break;
		case RDThemeFX.FallingPetals:
		case RDThemeFX.FallingPetalsInstant:
			petals1.gameObject.SetActive(value: false);
			petals2.gameObject.SetActive(value: false);
			petals3.gameObject.SetActive(value: false);
			break;
		case RDThemeFX.FallingPetalsSnow:
			petalsSnow1.gameObject.SetActive(value: false);
			petalsSnow2.gameObject.SetActive(value: false);
			petalsSnow3.gameObject.SetActive(value: false);
			break;
		case RDThemeFX.FallingLeaves:
			leaves1.gameObject.SetActive(value: false);
			leaves2.gameObject.SetActive(value: false);
			leaves3.gameObject.SetActive(value: false);
			break;
		case RDThemeFX.Snow:
			snowParticles.gameObject.SetActive(value: false);
			break;
		case RDThemeFX.Bloom:
			camera.GetComponent<VideoBloom>().enabled = false;
			break;
		case RDThemeFX.OrangeBloom:
			camera.GetComponent<VideoBloom>().enabled = false;
			break;
		case RDThemeFX.BlueBloom:
			camera.GetComponent<VideoBloom>().enabled = false;
			break;
		case RDThemeFX.HallOfMirrors:
			if (currentTheme != RDTheme.Kaleidoscope && currentTheme != RDTheme.Matrix && currentTheme != RDTheme.CrossesStraight && currentTheme != RDTheme.CrossesFalling && currentTheme != RDTheme.HallOfMirrors && currentTheme != RDTheme.FloatingHeart && currentTheme != RDTheme.FloatingHeartWithCubes && currentTheme != RDTheme.FloatingHeartBroken && currentTheme != RDTheme.FloatingHeartBrokenWithCubes && currentTheme != RDTheme.ZenGarden && currentTheme != RDTheme.Space && currentTheme != RDTheme.CubesFalling && currentTheme != RDTheme.CubesFallingNiceBlue)
			{
				camera.clearFlags = CameraClearFlags.Color;
			}
			break;
		case RDThemeFX.BlackAndWhite:
			camera.GetComponent<Grayscale>().enabled = false;
			break;
		case RDThemeFX.Sepia:
			camera.GetComponent<CameraFilterPack_Color_Sepia>().enabled = false;
			break;
		case RDThemeFX.JPEG:
			camera.GetComponent<CameraFilterPack_TV_CompressionFX>().enabled = false;
			break;
		case RDThemeFX.NumbersAbovePulses:
			showTextAboveBeatboxes = false;
			break;
		case RDThemeFX.Mosaic:
			camera.GetOrAddComponent<CameraFilterPack_Glitch_Mozaic>().enabled = false;
			break;
		case RDThemeFX.ScreenWaves:
			camera.GetOrAddComponent<CameraFilterPack_Distortion_Wave_Horizontal>().enabled = false;
			break;
		case RDThemeFX.HeatDistortion:
			camera.GetOrAddComponent<CameraFilterPack_Distortion_Heat>().enabled = false;
			break;
		case RDThemeFX.Pixelate:
			camera.GetOrAddComponent<CameraFilterPack_Pixel_Pixelisation>().enabled = false;
			break;
		case RDThemeFX.Scanlines:
			camera.GetOrAddComponent<CameraFilterPack_TV_80>().enabled = false;
			break;
		case RDThemeFX.Funk:
			camera.GetOrAddComponent<CameraFilterPack_FX_Funk>().enabled = false;
			break;
		case RDThemeFX.Grain:
			camera.GetOrAddComponent<CameraFilterPack_Film_Grain>().enabled = false;
			break;
		case RDThemeFX.Blizzard:
			camera.GetOrAddComponent<CameraFilterPack_Blizzard>().enabled = false;
			break;
		case RDThemeFX.Drawing:
			camera.GetOrAddComponent<CameraFilterPack_Drawing_Paper>().enabled = false;
			break;
		case RDThemeFX.Aberration:
			camera.GetOrAddComponent<ChromaAberrationDupe>().enabled = false;
			break;
		case RDThemeFX.HueShift:
			camera.GetOrAddComponent<RDHueShiftDupe>().enabled = false;
			break;
		case RDThemeFX.Blur:
			camera.GetOrAddComponent<BlurOptimized>().enabled = false;
			break;
		case RDThemeFX.RadialBlur:
			camera.GetOrAddComponent<RadialBlurDupe>().enabled = false;
			break;
		case RDThemeFX.Dots:
			camera.GetOrAddComponent<CameraFilterPack_FX_Dot_Circle>().enabled = false;
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
		case RDThemeFX.Diamonds:
			diamondParticles.gameObject.SetActive(value: false);
			break;
		case RDThemeFX.Tutorial:
			ptTutorialNotes.visible = false;
			break;
		case RDThemeFX.Balloons:
			sparklineBalloons.gameObject.SetActive(value: false);
			sparklineBalloons.Clear();
			break;
		case RDThemeFX.Embers:
			emberVFXParticles.gameObject.SetActive(value: false);
			break;
		case RDThemeFX.Fisheye:
			camera.GetOrAddComponent<CameraFilterPack_Distortion_FishEye>().enabled = false;
			break;
		case RDThemeFX.GlassShatter:
			glass.gameObject.SetActive(value: false);
			break;
		case RDThemeFX.EyesBig:
			eyesBig.gameObject.SetActive(value: false);
			break;
		case RDThemeFX.EyesSmall:
			eyesSmall.gameObject.SetActive(value: false);
			break;
		case RDThemeFX.RowGlowWhite:
		case RDThemeFX.RowOutline:
		case RDThemeFX.RowShadow:
		case RDThemeFX.RowAllWhite:
		case RDThemeFX.RowSilhouetteGlow:
		case RDThemeFX.RowPlain:
		case RDThemeFX.DisableAll:
			break;
		}
	}

	private void TryDisableBrightnessContrastSaturation()
	{
		if (!usingBrightness && !usingContrast && !usingSaturation)
		{
			camera.GetOrAddComponent<CameraFilterPack_Color_BrightContrastSaturation>().enabled = false;
		}
	}

	public void AddThemeFX(RDThemeFX fx, float? floatX = null, float? floatY = null, float? effectIntensity = null, float? extra = null, float? extra2 = null, float duration = 0f, Ease ease = Ease.Linear, Color? color = null)
	{
		bool useTweens = duration != 0f && duration != -999f;
		float num = Mathf.Abs(effectIntensity.GetValueOrDefault());
		float disabledIntensity = 0f;
		bool firstSet = !base.game.currentLevel.oldVFXEasing && !usedThemeFX.Contains(fx);
		if (firstSet)
		{
			usedThemeFX.Add(fx);
		}
		if (!vfxTweens.ContainsKey(fx))
		{
			vfxTweens.Add(fx, new Dictionary<int, Tween>());
		}
		CameraFilterPack_Pixel_Pixelisation pixel;
		switch (fx)
		{
		case RDThemeFX.WavyRows:
			if (firstSet)
			{
				customWavyRowsAmplitudeMult = 1f;
				customWavyRowsFrequencyMult = 1f;
			}
			if (effectIntensity.HasValue)
			{
				if (!wavyRows & useTweens)
				{
					customWavyRowsAmplitudeMult = 0f;
				}
				TweenVFX(0, () => customWavyRowsAmplitudeMult, (float a) =>
				{
					customWavyRowsAmplitudeMult = a;
				}, effectIntensity.Value);
			}
			if (extra.HasValue)
			{
				TweenVFX(1, () => customWavyRowsFrequencyMult, (float f) =>
				{
					customWavyRowsFrequencyMult = f;
				}, extra.Value);
			}
			wavyRows = true;
			break;
		case RDThemeFX.Tile2:
			RDBase.Vfx.SetTileScreen(new Vector2(2f, 2f), index);
			break;
		case RDThemeFX.Tile3:
			RDBase.Vfx.SetTileScreen(new Vector2(3f, 3f), index);
			break;
		case RDThemeFX.Tile4:
			RDBase.Vfx.SetTileScreen(new Vector2(4f, 4f), index);
			break;
		case RDThemeFX.LightStripVert:
			beatboxVerticalStrip = true;
			break;
		case RDThemeFX.BassDropOnHit:
			if (fxBassDrop == null)
			{
				fxBassDrop = scrExecuteOnHit.Add(() =>
				{
					RDBase.Vfx.BassDropNew(index);
				}, HitType.Hit, persistent: true, autoDelete: false, -1, overwrite: false);
			}
			else
			{
				fxBassDrop.isActive = true;
			}
			break;
		case RDThemeFX.ColourfulShockwaves:
			colorfulShockwaves = true;
			break;
		case RDThemeFX.ShakeOnHeartBeat:
			fxShakeOnHB.isActive = true;
			break;
		case RDThemeFX.SilhouettesOnHBeat:
			fxSilhouetteChangeOnHB.isActive = true;
			break;
		case RDThemeFX.Vignette:
			SetVignette(enabled: true);
			break;
		case RDThemeFX.VignetteFlicker:
			SetVignette(enabled: true);
			fxVignetteFlicker.isActive = true;
			break;
		case RDThemeFX.ShakeOnHit:
			fxShakeOnHit.isActive = true;
			break;
		case RDThemeFX.ScreenScroll:
		{
			CameraFilterPack_TV_VHS orAddComponent3 = camera.GetOrAddComponent<CameraFilterPack_TV_VHS>();
			orAddComponent3.enabled = true;
			orAddComponent3.Calibrage = 0.35f;
			orAddComponent3.CalibrageX = 0f;
			orAddComponent3.VHSEffectDisabled = 0f;
			break;
		}
		case RDThemeFX.ScreenScrollX:
		{
			CameraFilterPack_TV_VHS orAddComponent2 = camera.GetOrAddComponent<CameraFilterPack_TV_VHS>();
			orAddComponent2.enabled = true;
			orAddComponent2.Calibrage = 0f;
			orAddComponent2.CalibrageX = 0.35f;
			orAddComponent2.VHSEffectDisabled = 0f;
			break;
		}
		case RDThemeFX.ScreenScrollSansVHS:
		{
			CameraFilterPack_TV_VHS orAddComponent = camera.GetOrAddComponent<CameraFilterPack_TV_VHS>();
			if (!orAddComponent.enabled)
			{
				orAddComponent.VHSEffectDisabled = 1f;
			}
			orAddComponent.enabled = true;
			orAddComponent.Calibrage = 0.35f;
			orAddComponent.CalibrageX = 0f;
			break;
		}
		case RDThemeFX.ScreenScrollXSansVHS:
		{
			CameraFilterPack_TV_VHS orAddComponent4 = camera.GetOrAddComponent<CameraFilterPack_TV_VHS>();
			if (!orAddComponent4.enabled)
			{
				orAddComponent4.VHSEffectDisabled = 1f;
			}
			orAddComponent4.enabled = true;
			orAddComponent4.VHSEffectDisabled = 1f;
			orAddComponent4.Calibrage = 0f;
			orAddComponent4.CalibrageX = 0.35f;
			break;
		}
		case RDThemeFX.VHS:
		{
			CameraFilterPack_TV_VHS orAddComponent5 = camera.GetOrAddComponent<CameraFilterPack_TV_VHS>();
			orAddComponent5.enabled = true;
			orAddComponent5.VHSEffectDisabled = 0f;
			orAddComponent5.Parasite = 100f;
			orAddComponent5.Calibrage = 0f;
			orAddComponent5.CalibrageX = 0f;
			break;
		}
		case RDThemeFX.VHSRewind:
			TweenSimpleCameraFilter<CameraFilterPack_TV_VHS_Rewind>(1f, (CameraFilterPack_TV_VHS_Rewind f) => f.Cryptage, (CameraFilterPack_TV_VHS_Rewind f, float x) =>
			{
				f.Cryptage = x;
			});
			break;
		case RDThemeFX.CustomScreenScroll:
		{
			CameraFilterPack_TV_VHS ccs = camera.GetOrAddComponent<CameraFilterPack_TV_VHS>();
			if (!ccs.enabled)
			{
				ccs.VHSEffectDisabled = 1f;
			}
			ccs.enabled = true;
			if (firstSet)
			{
				ccs.Calibrage = 0f;
				ccs.CalibrageX = 0f;
			}
			if (floatY.HasValue)
			{
				TweenVFX(0, () => ccs.Calibrage, (float y) =>
				{
					ccs.Calibrage = y;
				}, floatY.Value / 10f);
			}
			if (floatX.HasValue)
			{
				TweenVFX(1, () => ccs.CalibrageX, (float x) =>
				{
					ccs.CalibrageX = x;
				}, floatX.Value / 10f);
			}
			break;
		}
		case RDThemeFX.RowGlowWhite:
			RDBase.Vfx.SetSpriteMode(SpriteMode.Glow, Color.white, index, 0.75f);
			break;
		case RDThemeFX.RowOutline:
			RDBase.Vfx.SetSpriteMode(SpriteMode.Outline, Color.black, index, 0.75f);
			break;
		case RDThemeFX.RowShadow:
			RDBase.Vfx.SetSpriteMode(SpriteMode.Glow, Color.black.WithAlpha(0.2f));
			break;
		case RDThemeFX.RowAllWhite:
			UseMaterial(new RDShaderProperties(Color.white, Color.white, 0.75f, borderType: false));
			break;
		case RDThemeFX.RowSilhouetteGlow:
			UseMaterial(new RDShaderProperties(Color.black, Color.white, 0.75f, borderType: false));
			break;
		case RDThemeFX.RowPlain:
			RDBase.Vfx.SetSpriteMode(SpriteMode.None, Color.white, index);
			break;
		case RDThemeFX.CutsceneMode:
			base.game.gameState = GameState.Cutscene;
			base.game.ShowCutsceneBars(show: true);
			break;
		case RDThemeFX.Blackout:
			RDBase.Vfx.BlackScreenOn(index);
			break;
		case RDThemeFX.Noise:
			fxNoise.visible = true;
			break;
		case RDThemeFX.GlitchObstruction:
			fxGlitch.visible = true;
			break;
		case RDThemeFX.Rain:
		{
			CameraFilterPack_Atmosphere_Rain_Pro rain = camera.GetOrAddComponent<CameraFilterPack_Atmosphere_Rain_Pro>();
			if (firstSet)
			{
				rain.Intensity = 0f;
				rain.Size = 0f;
				rain.Distortion = 0f;
			}
			if (RDUtils.IsInAccessibilityScope())
			{
				rain.StormFlashOnOff = (float)Persistence.GetFlashIntensity() / 100f;
			}
			if (effectIntensity.HasValue)
			{
				float targetVal6 = Mathf.Clamp(0.5f * effectIntensity.Value * num, -5f, 5f);
				float targetVal7 = Mathf.Clamp(effectIntensity.Value * 1.5f, -2f, 2f);
				float targetVal8 = Mathf.Max(0.025f * num, 0.025f);
				TweenVFX(0, () => rain.Intensity, (float x) =>
				{
					rain.Intensity = x;
				}, targetVal6);
				TweenVFX(1, () => rain.Size, (float x) =>
				{
					rain.Size = x;
				}, targetVal7);
				TweenVFX(2, () => rain.Distortion, (float x) =>
				{
					rain.Distortion = x;
				}, targetVal8);
			}
			rain.enabled = true;
			break;
		}
		case RDThemeFX.Matrix:
		{
			bool alternativeMatrix = base.game.currentLevel.alternativeMatrix;
			List<Background> list = (alternativeMatrix ? tblMatrixAlt : tblMatrix);
			List<Background> list2 = (alternativeMatrix ? tblMatrix : tblMatrixAlt);
			foreach (Background item in list)
			{
				item.visible = true;
			}
			{
				foreach (Background item2 in list2)
				{
					item2.visible = false;
				}
				break;
			}
		}
		case RDThemeFX.MiawMiaw:
			base.game.nurseCount = true;
			break;
		case RDThemeFX.Confetti:
			confetti.gameObject.SetActive(value: true);
			confetti.Simulate(8f);
			confetti.Play();
			break;
		case RDThemeFX.ConfettiBurst:
			confettiBurstL.gameObject.SetActive(value: true);
			confettiBurstR.gameObject.SetActive(value: true);
			if (!base.conductor.isScrubbing)
			{
				confettiBurstL.Play();
				confettiBurstR.Play();
			}
			else
			{
				confettiBurstL.Stop();
				confettiBurstR.Stop();
			}
			break;
		case RDThemeFX.FallingPetals:
			petals1.gameObject.SetActive(value: true);
			petals2.gameObject.SetActive(value: true);
			petals3.gameObject.SetActive(value: true);
			petals1.Simulate(8f);
			petals2.Simulate(8f);
			petals3.Simulate(8f);
			petals1.Play();
			petals2.Play();
			petals3.Play();
			break;
		case RDThemeFX.FallingPetalsInstant:
			petals1.gameObject.SetActive(value: true);
			petals2.gameObject.SetActive(value: true);
			petals3.gameObject.SetActive(value: true);
			petals1.Simulate(20f);
			petals2.Simulate(20f);
			petals3.Simulate(20f);
			petals1.Play();
			petals2.Play();
			petals3.Play();
			break;
		case RDThemeFX.FallingPetalsSnow:
			petalsSnow1.gameObject.SetActive(value: true);
			petalsSnow2.gameObject.SetActive(value: true);
			petalsSnow3.gameObject.SetActive(value: true);
			petalsSnow1.Simulate(8f);
			petalsSnow2.Simulate(8f);
			petalsSnow3.Simulate(8f);
			petalsSnow1.Play();
			petalsSnow2.Play();
			petalsSnow3.Play();
			break;
		case RDThemeFX.FallingLeaves:
			leaves1.gameObject.SetActive(value: true);
			leaves2.gameObject.SetActive(value: true);
			leaves3.gameObject.SetActive(value: true);
			leaves1.Simulate(10f);
			leaves2.Simulate(10f);
			leaves3.Simulate(10f);
			leaves1.Play();
			leaves2.Play();
			leaves3.Play();
			break;
		case RDThemeFX.Snow:
			snowParticles.gameObject.SetActive(value: true);
			break;
		case RDThemeFX.Bloom:
		{
			VideoBloom videoBloomN = camera.GetComponent<VideoBloom>();
			if (firstSet)
			{
				bloomTint = (videoBloomN.Tint = Color.white);
				bloomThreshold = (videoBloomN.Threshold = 4f);
				bloomIntensity = (videoBloomN.MasterAmount = 0f);
			}
			videoBloomN.enabled = true;
			if (color.HasValue)
			{
				bloomTint = color.Value;
				TweenVFXColor(0, () => videoBloomN.Tint, (Color x) =>
				{
					videoBloomN.Tint = x;
				}, bloomTint);
			}
			if (extra.HasValue)
			{
				bloomThreshold = extra.Value;
				TweenVFX(1, () => videoBloomN.Threshold, (float x) =>
				{
					videoBloomN.Threshold = x;
				}, bloomThreshold);
			}
			if (effectIntensity.HasValue)
			{
				bloomIntensity = effectIntensity.Value * 100f;
				TweenVFX(2, () => videoBloomN.MasterAmount, (float x) =>
				{
					videoBloomN.MasterAmount = x;
				}, bloomIntensity);
			}
			videoBloomN.KernelSize = 50f;
			break;
		}
		case RDThemeFX.OrangeBloom:
		{
			VideoBloom component = camera.GetComponent<VideoBloom>();
			component.enabled = true;
			component.Tint = 4284618239u.RGBAToColor();
			component.Threshold = 0.4f;
			component.KernelSize = 59f;
			component.MasterAmount = 2.18f;
			component.MediumKernelScale = 2.8f;
			component.HighQuality = false;
			break;
		}
		case RDThemeFX.BlueBloom:
		{
			VideoBloom component2 = camera.GetComponent<VideoBloom>();
			component2.enabled = true;
			component2.Tint = 641858559u.RGBAToColor();
			component2.Threshold = 0.14f;
			component2.KernelSize = 74.9f;
			component2.MasterAmount = 3.36f;
			break;
		}
		case RDThemeFX.HallOfMirrors:
			camera.clearFlags = CameraClearFlags.Depth;
			break;
		case RDThemeFX.TileN:
		{
			RDImageEffectTile tile = camera.GetOrAddComponent<RDImageEffectTile>();
			if (firstSet)
			{
				tile.tileX = 1f;
				tile.tileY = 1f;
				tile.anchorX = extra.GetValueOrDefault() / 100f;
				tile.anchorY = extra2.GetValueOrDefault() / 100f;
			}
			if (floatX.HasValue)
			{
				TweenVFX(0, () => tile.tileX, (float x) =>
				{
					tile.tileX = x;
				}, floatX.Value).OnUpdate(() =>
				{
					tile.UpdateState();
				});
			}
			if (floatY.HasValue)
			{
				TweenVFX(1, () => tile.tileY, (float x) =>
				{
					tile.tileY = x;
				}, floatY.Value).OnUpdate(() =>
				{
					tile.UpdateState();
				});
			}
			if (extra.HasValue)
			{
				TweenVFX(2, () => tile.anchorX, (float x) =>
				{
					tile.anchorX = x;
				}, extra.Value / 100f);
			}
			if (extra2.HasValue)
			{
				TweenVFX(3, () => tile.anchorY, (float x) =>
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
		case RDThemeFX.JPEG:
			TweenSimpleCameraFilter<CameraFilterPack_TV_CompressionFX>(1f, (CameraFilterPack_TV_CompressionFX f) => f.Parasite, (CameraFilterPack_TV_CompressionFX f, float x) =>
			{
				f.Parasite = x;
			});
			break;
		case RDThemeFX.NumbersAbovePulses:
			showTextAboveBeatboxes = true;
			break;
		case RDThemeFX.Mosaic:
			TweenSimpleCameraFilter<CameraFilterPack_Glitch_Mozaic>(1f, (CameraFilterPack_Glitch_Mozaic f) => f.ChangeValue, (CameraFilterPack_Glitch_Mozaic f, float x) =>
			{
				f.ChangeValue = x;
			}, (CameraFilterPack_Glitch_Mozaic f, float x) =>
			{
				f.Intensity = x;
			});
			break;
		case RDThemeFX.ScreenWaves:
			TweenSimpleCameraFilter<CameraFilterPack_Distortion_Wave_Horizontal>(32f, (CameraFilterPack_Distortion_Wave_Horizontal f) => f.ChangeWaveIntensity, (CameraFilterPack_Distortion_Wave_Horizontal f, float x) =>
			{
				f.ChangeWaveIntensity = x;
			}, (CameraFilterPack_Distortion_Wave_Horizontal f, float x) =>
			{
				f.WaveIntensity = x;
			});
			break;
		case RDThemeFX.HeatDistortion:
		{
			CameraFilterPack_Distortion_Heat heatAbnormal = camera.GetOrAddComponent<CameraFilterPack_Distortion_Heat>();
			if (firstSet)
			{
				heatAbnormal.Distortion = 0f;
				heatAbnormal.Position = new Vector2(1f - (floatX ?? 50f) / 100f, 1f - (floatY ?? 50f) / 100f);
			}
			if (effectIntensity.HasValue)
			{
				TweenVFX(0, () => heatAbnormal.Distortion, (float x) =>
				{
					heatAbnormal.Distortion = x;
				}, effectIntensity.Value * 0.1f);
			}
			if (floatX.HasValue && !firstSet)
			{
				TweenVFX(1, () => heatAbnormal.Position.x, (float x) =>
				{
					heatAbnormal.Position.x = x;
				}, 1f - floatX.Value / 100f);
			}
			if (floatY.HasValue && !firstSet)
			{
				TweenVFX(2, () => heatAbnormal.Position.y, (float y) =>
				{
					heatAbnormal.Position.y = y;
				}, 1f - floatY.Value / 100f);
			}
			heatAbnormal.enabled = true;
			break;
		}
		case RDThemeFX.Pixelate:
		{
			pixel = camera.GetOrAddComponent<CameraFilterPack_Pixel_Pixelisation>();
			if (firstSet)
			{
				pixel._SizeX = 0f;
				pixel._SizeY = 0f;
			}
			float? num4 = floatX * 9f / 4f;
			float? num5 = floatY * 16f / 4f;
			if (floatX.HasValue)
			{
				TweenVFX(0, () => pixel._SizeX, (float x) =>
				{
					pixel._SizeX = x;
				}, num4.Value).OnComplete(() =>
				{
					FinishPixelate();
				});
			}
			if (floatY.HasValue)
			{
				TweenVFX(1, () => pixel._SizeY, (float x) =>
				{
					pixel._SizeY = x;
				}, num5.Value).OnComplete(() =>
				{
					FinishPixelate();
				});
			}
			if (!useTweens)
			{
				FinishPixelate();
			}
			pixel._Pixelisation = 1f;
			pixel.enabled = true;
			break;
		}
		case RDThemeFX.Scanlines:
			camera.GetOrAddComponent<CameraFilterPack_TV_80>().enabled = true;
			break;
		case RDThemeFX.Funk:
			camera.GetOrAddComponent<CameraFilterPack_FX_Funk>().enabled = true;
			break;
		case RDThemeFX.Grain:
			TweenSimpleCameraFilter<CameraFilterPack_Film_Grain>(32f, (CameraFilterPack_Film_Grain f) => f.ChangeValue, (CameraFilterPack_Film_Grain f, float x) =>
			{
				f.ChangeValue = x;
			}, (CameraFilterPack_Film_Grain f, float x) =>
			{
				f.Value = x;
			});
			break;
		case RDThemeFX.Blizzard:
		{
			CameraFilterPack_Blizzard blizzard = camera.GetOrAddComponent<CameraFilterPack_Blizzard>();
			if (firstSet)
			{
				blizzard._Speed = 0f;
				blizzard._Fade = 0f;
			}
			if (effectIntensity.HasValue)
			{
				float value2 = effectIntensity.Value;
				float num2 = 1f;
				float num3 = 1f;
				if (value2 >= 0f)
				{
					num2 = Mathf.Sqrt(value2);
					num3 = Mathf.Sqrt(value2);
				}
				else
				{
					num2 = Mathf.Sqrt(num) * -1f;
					num3 = Mathf.Sqrt(num) * -1f;
				}
				TweenVFX(0, () => blizzard._Speed, (float x) =>
				{
					blizzard._Speed = x;
				}, num2);
				TweenVFX(1, () => blizzard._Fade, (float x) =>
				{
					blizzard._Fade = x;
				}, num3);
			}
			blizzard.enabled = true;
			break;
		}
		case RDThemeFX.Drawing:
		{
			CameraFilterPack_Drawing_Paper drawing = camera.GetOrAddComponent<CameraFilterPack_Drawing_Paper>();
			if (firstSet)
			{
				drawing.Pencil_Size = 0.0001f;
				drawing.Pencil_Correction = 0.55f;
				drawing.Intensity = 0.5f;
				drawing.Fade_With_Original = 0f;
				drawing.Speed_Animation = 1f;
				drawing.Pencil_Color = Color.black;
			}
			if (effectIntensity.HasValue)
			{
				float value = effectIntensity.Value;
				float targetVal2 = Mathf.LerpUnclamped(0.0001f, 0.0008f, value);
				float targetVal3 = Mathf.LerpUnclamped(0.55f, 0.76f, value);
				float targetVal4 = Mathf.LerpUnclamped(0.5f, 1f, value);
				float targetVal5 = 1f * value;
				TweenVFX(0, () => drawing.Pencil_Size, (float x) =>
				{
					drawing.Pencil_Size = x;
				}, targetVal2);
				TweenVFX(1, () => drawing.Pencil_Correction, (float x) =>
				{
					drawing.Pencil_Correction = x;
				}, targetVal3);
				TweenVFX(2, () => drawing.Intensity, (float x) =>
				{
					drawing.Intensity = x;
				}, targetVal4);
				TweenVFX(3, () => drawing.Fade_With_Original, (float x) =>
				{
					drawing.Fade_With_Original = x;
				}, targetVal5);
			}
			if (extra.HasValue)
			{
				TweenVFX(4, () => drawing.Speed_Animation, (float x) =>
				{
					drawing.Speed_Animation = x;
				}, extra.Value);
			}
			if (color.HasValue)
			{
				TweenVFXColor(5, () => drawing.Pencil_Color, (Color x) =>
				{
					drawing.Pencil_Color = x;
				}, color.Value);
			}
			drawing.enabled = true;
			break;
		}
		case RDThemeFX.Aberration:
			TweenSimpleCameraFilter<ChromaAberrationDupe>(0.02f, (ChromaAberrationDupe f) => f.ChangeOffset, (ChromaAberrationDupe f, float x) =>
			{
				f.ChangeOffset = x;
			}, (ChromaAberrationDupe f, float x) =>
			{
				f.Offset = x;
			});
			break;
		case RDThemeFX.Blur:
			TweenSimpleCameraFilter<BlurOptimized>(1.45f, (BlurOptimized f) => f.blurSize, (BlurOptimized f, float x) =>
			{
				f.blurSize = x;
			});
			break;
		case RDThemeFX.RadialBlur:
		{
			RadialBlurDupe radial = camera.GetOrAddComponent<RadialBlurDupe>();
			if (firstSet)
			{
				radial.Intensity = (radial.ChangeValue = 0f);
				radial.MovX = (radial.ChangeValue2 = 0.5f);
				radial.MovY = (radial.ChangeValue3 = 0.5f);
			}
			if (effectIntensity.HasValue)
			{
				TweenVFX(0, () => radial.Intensity, (float x) =>
				{
					radial.ChangeValue = (radial.Intensity = x);
				}, effectIntensity.Value * 0.125f);
			}
			if (floatX.HasValue)
			{
				TweenVFX(1, () => radial.MovX, (float x) =>
				{
					radial.ChangeValue2 = (radial.MovX = x);
				}, floatX.Value / 100f);
			}
			if (floatY.HasValue)
			{
				TweenVFX(2, () => radial.MovY, (float x) =>
				{
					radial.ChangeValue3 = (radial.MovY = x);
				}, floatY.Value / 100f);
			}
			radial.enabled = true;
			break;
		}
		case RDThemeFX.Dots:
			disabledIntensity = 1f;
			if (effectIntensity.HasValue && (!useTweens || !base.game.currentLevel.oldVFXEasing))
			{
				effectIntensity = Mathf.Max(1f, 7f * effectIntensity.Value);
			}
			TweenSimpleCameraFilter<CameraFilterPack_FX_Dot_Circle>(1f, (CameraFilterPack_FX_Dot_Circle f) => f.ChangeValue, (CameraFilterPack_FX_Dot_Circle f, float x) =>
			{
				f.ChangeValue = x;
			}, (CameraFilterPack_FX_Dot_Circle f, float x) =>
			{
				f.Value = x;
			});
			break;
		case RDThemeFX.HueShift:
			disabledIntensity = 1f;
			TweenSimpleCameraFilter<RDHueShiftDupe>(1f, (RDHueShiftDupe f) => f.Hue, (RDHueShiftDupe f, float x) =>
			{
				f.Hue = x;
			});
			break;
		case RDThemeFX.Brightness:
			usingBrightness = true;
			disabledIntensity = 1f;
			TweenSimpleCameraFilter<CameraFilterPack_Color_BrightContrastSaturation>(1f, (CameraFilterPack_Color_BrightContrastSaturation f) => f.Brightness, (CameraFilterPack_Color_BrightContrastSaturation f, float x) =>
			{
				f.Brightness = x;
			});
			break;
		case RDThemeFX.Contrast:
			usingContrast = true;
			disabledIntensity = 1f;
			TweenSimpleCameraFilter<CameraFilterPack_Color_BrightContrastSaturation>(1f, (CameraFilterPack_Color_BrightContrastSaturation f) => f.Contrast, (CameraFilterPack_Color_BrightContrastSaturation f, float x) =>
			{
				f.Contrast = x;
			});
			break;
		case RDThemeFX.Saturation:
			usingSaturation = true;
			disabledIntensity = 1f;
			TweenSimpleCameraFilter<CameraFilterPack_Color_BrightContrastSaturation>(1f, (CameraFilterPack_Color_BrightContrastSaturation f) => f.Saturation, (CameraFilterPack_Color_BrightContrastSaturation f, float x) =>
			{
				f.Saturation = x;
			});
			break;
		case RDThemeFX.Diamonds:
		{
			diamondParticles.gameObject.SetActive(value: true);
			ParticleSystem particleSystem = diamondParticles;
			ParticleSystem subEmitterSystem = particleSystem.subEmitters.GetSubEmitterSystem(0);
			ParticleSystem.EmissionModule burstEmission = particleSystem.emission;
			ParticleSystem.MainModule psMain = subEmitterSystem.main;
			if (firstSet)
			{
				burstEmission.rateOverTime = 0f;
			}
			if (effectIntensity.HasValue)
			{
				TweenVFX(0, () => burstEmission.rateOverTime.constant, (float x) =>
				{
					burstEmission.rateOverTime = x;
				}, effectIntensity.Value * 10f);
			}
			if (color.HasValue)
			{
				TweenVFXColor(1, () => psMain.startColor.color, (Color x) =>
				{
					psMain.startColor = x;
				}, color.Value);
			}
			if (!diamondParticles.isPlaying)
			{
				diamondParticles.Play();
			}
			break;
		}
		case RDThemeFX.Tutorial:
			ptTutorialNotes.visible = true;
			if (firstSet)
			{
				ptTutorialNotes.velocity = Vector2.zero;
			}
			if (effectIntensity.HasValue)
			{
				Vector2 vector = Vector2.one * -6f / 60f * effectIntensity.Value;
				TweenVFX(0, () => ptTutorialNotes.velocity.x, (float x) =>
				{
					ptTutorialNotes.velocity = ptTutorialNotes.velocity.WithX(x);
				}, vector.x);
				TweenVFX(1, () => ptTutorialNotes.velocity.y, (float y) =>
				{
					ptTutorialNotes.velocity = ptTutorialNotes.velocity.WithY(y);
				}, vector.y);
			}
			if (color.HasValue)
			{
				TweenVFXColor(2, () => ptTutorialNotes.tiledSprite.color, (Color x) =>
				{
					ptTutorialNotes.tiledSprite.color = x;
				}, color.Value);
			}
			break;
		case RDThemeFX.Balloons:
			sparklineBalloons.gameObject.SetActive(value: true);
			break;
		case RDThemeFX.Embers:
		{
			ParticleSystem.MainModule main = emberVFXParticles.main;
			ParticleSystem.EmissionModule em = emberVFXParticles.emission;
			if (firstSet)
			{
				em.rateOverTime = 0f;
			}
			if (effectIntensity.HasValue)
			{
				float targetVal = Mathf.Clamp(50f * effectIntensity.Value, 0f, 500f);
				TweenVFX(0, () => em.rateOverTime.constant, (float x) =>
				{
					em.rateOverTime = x;
				}, targetVal);
			}
			if (color.HasValue)
			{
				TweenVFXColor(1, () => main.startColor.color, (Color x) =>
				{
					main.startColor = x;
				}, color.Value);
			}
			if (!emberVFXParticles.gameObject.activeSelf)
			{
				emberVFXParticles.gameObject.SetActive(value: true);
				emberVFXParticles.Simulate(8f);
				emberVFXParticles.Play();
			}
			break;
		}
		case RDThemeFX.Fisheye:
			if (effectIntensity.HasValue)
			{
				effectIntensity = effectIntensity * 0.5f + 0.5f;
			}
			disabledIntensity = 0.5f;
			TweenSimpleCameraFilter<CameraFilterPack_Distortion_FishEye>(1f, (CameraFilterPack_Distortion_FishEye f) => f.ChangeDistortion, (CameraFilterPack_Distortion_FishEye f, float x) =>
			{
				f.ChangeDistortion = x;
				f.Distortion = x;
			}, (CameraFilterPack_Distortion_FishEye f, float x) =>
			{
				f.Distortion = x;
			});
			break;
		case RDThemeFX.GlassShatter:
			glass.transform.position = base.transform.position;
			glass.Shatter(this);
			break;
		case RDThemeFX.EyesBig:
		case RDThemeFX.EyesSmall:
		{
			RDManyEyes rDManyEyes = ((fx == RDThemeFX.EyesSmall) ? eyesSmall : eyesBig);
			if (effectIntensity.HasValue)
			{
				rDManyEyes.EyesClose(1f - Mathf.Clamp(effectIntensity.Value, 0f, 1f), !rDManyEyes.gameObject.activeSelf || base.conductor.isScrubbing);
			}
			if (floatX.HasValue || floatY.HasValue)
			{
				rDManyEyes.SetEyeDirection(new Vector2(floatX.GetValueOrDefault(), floatY.GetValueOrDefault()), duration, ease);
			}
			if (extra.HasValue || extra2.HasValue)
			{
				rDManyEyes.SetEyesTarget((extra.GetValueOrDefault() - 50f) * RDBase.Vfx.RDWidth / 100f, (extra2.GetValueOrDefault() - 50f) * RDBase.Vfx.RDHeight / 100f, duration, ease);
			}
			if (color.HasValue)
			{
				rDManyEyes.SetEyeColor(color.Value, duration, ease);
			}
			rDManyEyes.gameObject.SetActive(value: true);
			break;
		}
		case RDThemeFX.DisableAll:
			DisableAllThemeFX();
			break;
		}
		void FinishPixelate()
		{
			if (pixel._SizeX == 0f && pixel._SizeY == 0f)
			{
				pixel.enabled = false;
			}
		}
		void TweenSimpleCameraFilter<Filter>(float intensityMultiplier, Func<Filter, float> get, Action<Filter, float> set, Action<Filter, float> finalSet = null) where Filter : Behaviour
		{
			Filter filter = camera.GetOrAddComponent<Filter>();
			if (firstSet)
			{
				set(filter, disabledIntensity);
				finalSet?.Invoke(filter, disabledIntensity);
			}
			if (effectIntensity.HasValue)
			{
				float num6 = effectIntensity.Value * intensityMultiplier;
				TweenVFX(0, () => get(filter), (float x) =>
				{
					set(filter, x);
				}, num6);
				if (!useTweens)
				{
					finalSet?.Invoke(filter, num6);
				}
			}
			filter.enabled = true;
		}
		Tween TweenVFX(int id, DOGetter<float> get, DOSetter<float> set, float targetVal9)
		{
			return vfxTweens[fx][id] = base.game.currentLevel.TweenThemeFX(vfxTweens[fx].GetValueOrDefault(id), get, set, targetVal9, duration, ease, useTweens);
		}
		Tween TweenVFXColor(int id, DOGetter<Color> get, DOSetter<Color> set, Color targetVal9)
		{
			return vfxTweens[fx][id] = base.game.currentLevel.TweenThemeFXColor(vfxTweens[fx].GetValueOrDefault(id), get, set, targetVal9, duration, ease, useTweens);
		}
	}

	public void UseMaterial(RDShaderProperties material)
	{
		Row[] rows = base.game.rows;
		foreach (Row row in rows)
		{
			if (row.ent != null && row.room == index)
			{
				row.ent.SetMaterial(usesOwnShaderData: true, material);
			}
		}
	}

	public void SetShakeIntensityOnHit(int count, int amount)
	{
		if (fxShakeOnHit == null)
		{
			fxShakeOnHit = scrExecuteOnHit.Add(null, HitType.Hit, persistent: true, autoDelete: false, -1, overwrite: false);
		}
		fxShakeOnHit.action = () =>
		{
			camera.Shake(count, amount);
		};
	}

	public void SetShakeIntensityDefault(shakeIntensityPreset preset)
	{
		if (fxShakeOnHit == null)
		{
			fxShakeOnHit = scrExecuteOnHit.Add(null, HitType.Hit, persistent: true, autoDelete: false, -1, overwrite: false);
		}
		int shakeCount = 3;
		int shakeAmount = 2;
		switch (preset)
		{
		case shakeIntensityPreset.Dubstep:
			shakeCount = 3;
			shakeAmount = 2;
			break;
		case shakeIntensityPreset.OneshotLarge:
			shakeCount = 10;
			shakeAmount = 5;
			break;
		}
		fxShakeOnHit.action = () =>
		{
			camera.Shake(shakeCount, shakeAmount);
		};
	}

	public void SetVignette(bool enabled)
	{
		vignette.transform.parent = camera.transform;
		vignette.transform.localPosition = new Vector3(0f, 0f, 1.5f);
		vignette.gameObject.SetActive(enabled);
	}

	[ListedMethod(true)]
	public void SetVignetteAlpha(float alpha)
	{
		vignette.transform.parent = camera.transform;
		vignette.transform.localPosition = new Vector3(0f, 0f, 1.5f);
		vignette.color = vignette.color.WithAlpha(alpha);
	}

	public void PreloadEmbers()
	{
		if (!(embers != null))
		{
			embers = UnityEngine.Object.Instantiate(kaleidoscopeEmbersPrefab, base.gm.parentContainer).GetComponent<RDKaleidoscope>();
			embers.transform.LocalMoveX(3000f * (float)index);
			embers.room = this;
		}
	}

	public void SetRowContainerY(float amount, float reference = -1f)
	{
		float num = reference / 2f + amount - RDBase.Vfx.RDHeight / 2f;
		if (reference == -1f)
		{
			num = amount;
		}
		rowContainer.localPosition = new Vector2(rowContainer.localPosition.x, num);
	}

	public void SetRowContainerX(float amount)
	{
		rowContainer.localPosition = new Vector2(amount, rowContainer.localPosition.y);
	}

	public void BeatboxPulsed(Vector3 position, int number, string customText = "", float duration = 0.5f)
	{
		if (mShowTextAboveBeatboxes && lastNumberPulsed != number && (number != 7 || !syncTextEnabled))
		{
			if (textCountTween != null)
			{
				textCountTween.Kill();
			}
			countText.transform.DOKill();
			float num = position.y + 35f;
			countText.transform.MoveXY(position.x, num);
			countText.text = (string.IsNullOrEmpty(customText) ? CustomTextAboveBeatboxes[number] : customText);
			countText.font = (string.IsNullOrEmpty(customText) ? RDString.GetAppropiateFlashFontForString(countText.text) : base.gc.latinFontPack.flashFont.font);
			countText.color = Color.white;
			textCountTween = DOTween.ToAlpha(() => countText.color, (Color c) =>
			{
				countText.color = c;
			}, 0f, duration).SetDelay(0f);
			countText.transform.DOMoveY(num + 20f, 0.5f).SetDelay(0f);
			lastNumberPulsed = number;
		}
		if (mBeatboxWithLightStrip)
		{
			lightStrip.Show(position.x, (number == 7) ? 106f : 24f);
		}
	}

	public void LoadLightStrip()
	{
		lightStrip = UnityEngine.Object.Instantiate(RDBase.Vfx.lightStripPrefab, base.gm.parentContainer).GetComponent<RDLightStrip>();
	}

	public void MakeShockwave(Vector3 position, RowEntity rowEnt = null)
	{
		if (!RDBase.Vfx.hiResShockwaves)
		{
			return;
		}
		Vector3 position2 = new Vector3(position.x, position.y, rowEnt.heart.transform.position.z + -10f);
		foreach (GameObject mShockwave in mShockwaves)
		{
			DetonatorHeatwave component = mShockwave.GetComponent<DetonatorHeatwave>();
			if (component.dead)
			{
				mShockwave.transform.localScale = new Vector3(3f, 3f, 3f);
				mShockwave.transform.position = position2;
				component.durationMultiplier = base.game.currentLevel.shockwaveDurationMultiplier;
				component.distortionMultiplier = base.game.currentLevel.shockwaveDistortionMultiplier;
				component.sizeMultiplier = base.game.currentLevel.shockwaveSizeMultiplier;
				mShockwave.GetComponent<DetonatorHeatwave>().Explode();
				break;
			}
		}
		if (colorfulShockwaves)
		{
			inverseShockwaveGO.transform.DOKill();
			inverseShockwaveGO.SetActive(value: true);
			Material material = inverseShockwaveGO.GetComponent<Renderer>().material;
			float num = material.GetFloat("_Hue");
			num = 0.28f;
			material.SetFloat("_Hue", num);
			Transform obj = inverseShockwaveGO.transform;
			obj.position = position2;
			obj.localScale = Vector3.one * 0.001f;
			obj.DOScale(Vector3.one * 800f, 1f).SetEase(Ease.Linear).OnComplete(() =>
			{
				inverseShockwaveGO.SetActive(value: false);
				RDHueShift component2 = camera.GetComponent<RDHueShift>();
				component2.enabled = true;
				component2.Hue += 0.28f;
			});
		}
	}

	public RDTiledBackground GetTextureLayer(RDSortingLayer sortingLayer)
	{
		GameObject gameObject = null;
		switch (sortingLayer)
		{
		case RDSortingLayer.Background:
			if (customBackground == null)
			{
				customBackground = UnityEngine.Object.Instantiate(base.game.customBackground);
				customBackground.transform.SetParent(backgroundContainerCamera);
				customBackground.transform.localPosition = Vector3.zero.WithZ(10f);
				customBackground.gameObject.SetActive(value: false);
				customBackground.transform.SetParent(backgroundContainerCamera);
				SetCustomBackgroundDepth(customBackgroundDepth);
			}
			gameObject = customBackground;
			break;
		case RDSortingLayer.Foreground:
			if (customForeground == null)
			{
				customForeground = UnityEngine.Object.Instantiate(base.game.customBackground);
				customForeground.transform.SetParent(backgroundContainerCamera);
				customForeground.transform.localPosition = Vector3.zero.WithZ(10f);
				customForeground.gameObject.SetActive(value: false);
				customForeground.transform.SetParent(backgroundContainerCamera);
			}
			gameObject = customForeground;
			break;
		}
		return gameObject.GetComponent<RDTiledBackground>();
	}

	public void HideTextureLayer(RDSortingLayer sortingLayer)
	{
		switch (sortingLayer)
		{
		case RDSortingLayer.Background:
			customBackground?.SetActive(value: false);
			break;
		case RDSortingLayer.Foreground:
			customForeground?.SetActive(value: false);
			break;
		}
	}

	[ListedMethod(true)]
	public void EditKaleidoscopeColor(bool primaryColor, string colHex, float durationInBeats, string easeString)
	{
		Enum.TryParse<Ease>(easeString, out var result);
		Color color = colHex.HexToColor();
		if (primaryColor)
		{
			kaleidoscope.SetColorA(color, durationInBeats, result);
		}
		else
		{
			kaleidoscope.SetColorB(color, durationInBeats, result);
		}
	}

	[ListedMethod(true)]
	public void EditKaleidoscopeSpeed(float speed, float durationInBeats, string easeString)
	{
		Enum.TryParse<Ease>(easeString, out var result);
		float duration = base.conductor.DurationBeatsToTime(base.conductor.currentBeat, durationInBeats, calledCloseToNextBarMaybe: true);
		DOTween.To(() => kaleidoscopeSpeed, (float x) =>
		{
			kaleidoscopeSpeed = x;
		}, speed, duration).SetEase(result);
	}

	[ListedMethod(true)]
	public void EditKaleidoscopeRate(float rate)
	{
		kaleidoscopeEmissionRate = rate;
	}

	[ListedMethod(true)]
	public void EditKaleidoscopeColors(float r1, float g1, float b1, float r2, float g2, float b2)
	{
		base.game.currentLevel.SetKaleidoscopeColor(index, r1, g1, b1, r2, g2, b2);
	}

	[ListedMethod(true)]
	public void SyncKaleidoscopes(int otherRoom)
	{
		base.game.currentLevel.SyncKaleidoscopes(index, otherRoom);
	}

	[ListedMethod(true)]
	public void EditKaleidoscopeRepeat(int repeat)
	{
		kaleidoscope.SetRepeat(repeat);
	}

	[ListedMethod(true)]
	public void EditKaleidoscopeOffset(float offset, float durationBeats, string easeStr)
	{
		base.game.currentLevel.SetKaleidoscopeOffset(index, offset, durationBeats, easeStr);
	}

	[ListedMethod(true)]
	public void EditKaleidoscopeRoll(float roll, float durationBeats, string easeStr)
	{
		base.game.currentLevel.SetKaleidoscopeRoll(index, roll, durationBeats, easeStr);
	}

	[ListedMethod(true)]
	public void ToggleKaleidoscopeSymmetry(bool enabled)
	{
		kaleidoscope.ToggleSymmetry(enabled);
	}

	public void EditTree(string propertyName, float toValue, float durationInBeats, string easeString)
	{
		RDProceduralTree tree = proceduralTree.GetComponent<RDProceduralTree>();
		Enum.TryParse<Ease>(easeString, out var result);
		float duration = base.conductor.DurationBeatsToTime(base.conductor.currentBeat, durationInBeats, calledCloseToNextBarMaybe: true);
		switch (propertyName)
		{
		case "branchesPerIteration":
			DOTween.To(() => tree.branchesPerIteration, (float x) =>
			{
				tree.branchesPerIteration = Mathf.RoundToInt(x);
			}, toValue, duration).SetEase(result);
			break;
		case "branchesPerDivision":
			DOTween.To(() => tree.branchesPerDivision, (float x) =>
			{
				tree.branchesPerDivision = Mathf.RoundToInt(x);
			}, toValue, duration).SetEase(result);
			break;
		case "iterationsPerSecond":
			DOTween.To(() => tree.iterationsPerSecond, (float x) =>
			{
				tree.iterationsPerSecond = x;
			}, toValue, duration).SetEase(result);
			break;
		case "thickness":
			DOTween.To(() => tree.thickness, (float x) =>
			{
				tree.thickness = x;
			}, toValue, duration).SetEase(result);
			break;
		case "targetLength":
			DOTween.To(() => tree.targetLength, (float x) =>
			{
				tree.targetLength = x;
			}, toValue, duration).SetEase(result);
			break;
		case "maxDeviation":
			DOTween.To(() => tree.maxDeviation, (float x) =>
			{
				tree.maxDeviation = x;
			}, toValue, duration).SetEase(result);
			break;
		case "angle":
			DOTween.To(() => tree.angle, (float x) =>
			{
				tree.angle = x;
			}, toValue, duration).SetEase(result);
			break;
		case "camAngle":
			DOTween.To(() => tree.camAngle, (float x) =>
			{
				tree.camAngle = x;
			}, toValue, duration).SetEase(result);
			break;
		case "camDistance":
			DOTween.To(() => tree.camDistance, (float x) =>
			{
				tree.camDistance = x;
			}, toValue, duration).SetEase(result);
			break;
		case "camDegreesPerSecond":
			DOTween.To(() => tree.camDegreesPerSecond, (float x) =>
			{
				tree.camDegreesPerSecond = x;
			}, toValue, duration).SetEase(result);
			break;
		case "camSpeed":
			DOTween.To(() => tree.camSpeed, (float x) =>
			{
				tree.camSpeed = x;
			}, toValue, duration).SetEase(result);
			break;
		case "pulseIntensity":
			DOTween.To(() => tree.pulseIntensity, (float x) =>
			{
				tree.pulseIntensity = x;
			}, toValue, duration).SetEase(result);
			break;
		case "pulseRate":
			DOTween.To(() => tree.pulseRate, (float x) =>
			{
				tree.pulseRate = x;
			}, toValue, duration).SetEase(result);
			break;
		case "pulseWavelength":
			DOTween.To(() => tree.pulseWavelength, (float x) =>
			{
				tree.pulseWavelength = x;
			}, toValue, duration).SetEase(result);
			break;
		case "pulsePhase":
			DOTween.To(() => tree.pulsePhase, (float x) =>
			{
				tree.pulsePhase = x;
			}, toValue, duration).SetEase(result);
			break;
		}
	}

	public void EditTreeColor(bool pulseColor, string colHex, float durationInBeats, string easeString)
	{
		RDProceduralTree tree = proceduralTree.GetComponent<RDProceduralTree>();
		Enum.TryParse<Ease>(easeString, out var result);
		float duration = base.conductor.DurationBeatsToTime(base.conductor.currentBeat, durationInBeats, calledCloseToNextBarMaybe: true);
		Color endValue = colHex.HexToColor();
		if (pulseColor)
		{
			DOTween.To(() => tree.pulseColor, (Color x) =>
			{
				tree.pulseColor = x;
			}, endValue, duration).SetEase(result);
		}
		else
		{
			DOTween.To(() => tree.restColor, (Color x) =>
			{
				tree.restColor = x;
			}, endValue, duration).SetEase(result);
		}
	}

	public void FadeCafeFG(float opPerc, float secs)
	{
		tk2dSprite[] fgSprites = coffeeShop.GetComponent<RDCoffeeShop>().fgSprites;
		for (int i = 0; i < fgSprites.Length; i++)
		{
			fgSprites[i].DOFade(opPerc / 100f, secs);
		}
	}

	[ListedMethod(true)]
	public void ColeLight(int index)
	{
		RDHoodieBoyWard rDHoodieBoyWard = coleWard?.GetComponent<RDHoodieBoyWard>();
		if ((bool)rDHoodieBoyWard)
		{
			if (index < 0)
			{
				rDHoodieBoyWard.ToggleAllLights(on: false);
			}
			else
			{
				rDHoodieBoyWard.TurnOnLight(index);
			}
		}
	}

	[ListedMethod(true)]
	public void CafeLight(int patternID)
	{
		RDCoffeeShop rDCoffeeShop = coffeeShop?.GetComponent<RDCoffeeShop>();
		if ((bool)rDCoffeeShop)
		{
			if (patternID < 0)
			{
				rDCoffeeShop.lights.enabled = true;
			}
			else
			{
				rDCoffeeShop.SetLights(patternID);
			}
		}
	}

	[ListedMethod(true)]
	public void CafeLightColor(string colHex, float opacity, float durationInBeats)
	{
		RDCoffeeShop rDCoffeeShop = coffeeShop?.GetComponent<RDCoffeeShop>();
		if ((bool)rDCoffeeShop)
		{
			float dur = base.conductor.DurationBeatsToTime(base.conductor.currentBeat, durationInBeats, calledCloseToNextBarMaybe: true);
			rDCoffeeShop.SetLightColor(colHex.HexToColor(), opacity, dur);
		}
	}

	[ListedMethod(true)]
	public void CafeDoor(bool open)
	{
		coffeeShop?.GetComponent<RDCoffeeShop>()?.OpenDoor(open);
	}

	[ListedMethod(true)]
	public void PaigeDoor(bool open)
	{
		hospitalWard?.GetComponent<RDMainWard>()?.SetOfficeDoorOpened(open);
	}

	[ListedMethod(true)]
	public void AbandonedDoor(bool open)
	{
		abandonedWard?.GetComponent<RDAbandonedWard>()?.SetDoorOpened(open);
	}

	[ListedMethod(true)]
	public void AbandonedElevator(bool open)
	{
		abandonedWard?.GetComponent<RDAbandonedWard>()?.SetElevatorOpened(open);
	}

	[ListedMethod(true)]
	public void SpaceCloseup(bool enabled)
	{
		if (!(spaceBackground == null))
		{
			if (enabled)
			{
				spaceBackground.SwitchToCloseupMode();
			}
			else if (!spaceBackground.ship.activeSelf)
			{
				bool flag = spaceMode;
				UnityEngine.Object.Destroy(spaceBackground.gameObject);
				spaceBackground = base.game.currentLevel.LoadLevelAsset<RDSpaceBackground>();
				spaceBackground.transform.LocalMoveX(3000f * (float)index);
				spaceMode = flag;
			}
		}
	}

	[ListedMethod(true)]
	public void SpaceStars(bool enabled)
	{
		if (!(spaceBackground == null))
		{
			spaceBackground.stars.gameObject.SetActive(enabled);
		}
	}

	[ListedMethod(true)]
	public void SamuraiDoor(bool open, bool instant, bool updateLanterns)
	{
		RDSamuraiWard component = samuraiWard.GetComponent<RDSamuraiWard>();
		base.game.StartCoroutine(component.OpenDoors(open, instant, updateLanterns));
	}

	[ListedMethod(true)]
	public void SamuraiLanterns(bool show, float duration, string easeStr)
	{
		float duration2 = base.conductor.DurationBeatsToTime(base.conductor.currentBeat, duration);
		Enum.TryParse<Ease>(easeStr, out var result);
		samuraiWard.GetComponent<RDSamuraiWard>()?.MoveLanterns(show, duration2, result);
	}

	[ListedMethod(true)]
	public void SamuraiTransparentBG(bool enabled)
	{
		samuraiWard.GetComponent<RDSamuraiWard>()?.SetToEmptyVersion(enabled);
	}

	[ListedMethod(true)]
	public void ScaleInsomniacPagoda(float startScale, float endScale, float duration)
	{
		Background[] obj = new Background[3] { ptInsomniacPagoda, ptInsomniacNPagoda, ptInsomniacNPagodaLights };
		float num = (startScale - 1f) / 2f;
		float num2 = (endScale - 1f) / 2f;
		Background[] array = obj;
		foreach (Background background in array)
		{
			if (!(background == null))
			{
				background.transform.localScale = new Vector3(startScale, startScale, 1f);
				background.transform.localPosition = new Vector3((0f - RDBase.Vfx.RDWidth) * num, RDBase.Vfx.RDHeight * num, background.transform.localPosition.z);
				background.transform.DOKill();
				background.transform.DOScale(new Vector3(endScale, endScale, 1f), duration);
				background.transform.DOLocalMove(new Vector3((0f - RDBase.Vfx.RDWidth) * num2, RDBase.Vfx.RDHeight * num2, background.transform.localPosition.z), duration);
			}
		}
	}

	[ListedMethod(true)]
	public void PhysioEdega(bool show)
	{
		stadium.physioEdega.gameObject.SetActive(show);
	}

	[ListedMethod(true)]
	public void PhysioLights(bool on)
	{
		stadium.TweenPhysioLightingAlpha(on ? 1f : 0f, 0f);
	}

	[ListedMethod(true)]
	public void StadiumBaseballRain(float baseballsPerSecond)
	{
		if (baseballsPerSecond < 0f)
		{
			stadium.KillRain();
		}
		else
		{
			stadium.SetRain(baseballsPerSecond);
		}
	}

	[ListedMethod(true)]
	public void StadiumLightning(bool showTeam)
	{
		stadium.AnimateLightning(showTeam);
	}

	[ListedMethod(true)]
	public void StadiumGlitchyLights(bool enabled)
	{
		stadium.glitchyLights = enabled;
	}

	[ListedMethod(true)]
	public void SetWardScroll(float speed, float durationInBeats, string easeString)
	{
		float duration = base.conductor.DurationBeatsToTime(base.conductor.currentBeat, durationInBeats);
		Enum.TryParse<Ease>(easeString, out var result);
		RDMainWard ward = hospitalWard.GetComponent<RDMainWard>();
		DOTween.To(() => ward.scrollSpeed, (float x) =>
		{
			ward.scrollSpeed = x;
		}, speed, duration).SetEase(result).Done();
	}

	[ListedMethod(true)]
	public void IncrementWardScrollOffset(float xDistance, float durationInBeats, string easeString)
	{
		float duration = base.conductor.DurationBeatsToTime(base.conductor.currentBeat, durationInBeats);
		Enum.TryParse<Ease>(easeString, out var result);
		RDMainWard ward = hospitalWard.GetComponent<RDMainWard>();
		float endValue = ward.scrollOffsetX + xDistance;
		DOTween.To(() => ward.scrollOffsetX, (float x) =>
		{
			ward.scrollOffsetX = x;
		}, endValue, duration).SetEase(result);
	}

	[ListedMethod(true)]
	public void SetWardScrollOffsetSmooth(float targetX, float durationInBeats, float accelerationBeats, float decelerationBeats)
	{
		float num = durationInBeats - accelerationBeats - decelerationBeats;
		float num2 = base.conductor.DurationBeatsToTime(base.conductor.currentBeat, accelerationBeats);
		float num3 = base.conductor.DurationBeatsToTime(base.conductor.currentBeat + accelerationBeats, num);
		float num4 = base.conductor.DurationBeatsToTime(base.conductor.currentBeat + accelerationBeats + num, decelerationBeats);
		float duration = num2 + num3 + num4;
		RDMainWard ward = hospitalWard.GetComponent<RDMainWard>();
		DOTween.To(() => ward.scrollOffsetX, (float x) =>
		{
			ward.scrollOffsetX = x;
		}, targetX, duration).SetEase(LevelEvent_MoveRow.VelocityBasedEase(num2, num4));
	}

	public void ParentRowToWard(int rowID, bool parentToWard)
	{
		RowEntity ent = base.game.rows[rowID].ent;
		if (!(ent == null))
		{
			RDMainWard component = hospitalWard.GetComponent<RDMainWard>();
			Transform parent = (parentToWard ? component.objectsContainer : rowContainer);
			ent.transform.SetParent(parent, worldPositionStays: false);
		}
	}

	public void ParentSpriteToWard(int spriteIndex, bool parentToWard)
	{
		CustomSprite value = base.game.currentLevel.sprites.ElementAtOrDefault(spriteIndex).Value;
		RDMainWard component = hospitalWard.GetComponent<RDMainWard>();
		Transform parent = (parentToWard ? component.objectsContainer : decorationContainer);
		value.transform.SetParent(parent, worldPositionStays: false);
	}

	[ListedMethod(true)]
	public void SetWardSeamless(bool seamless)
	{
		hospitalWard.GetComponent<RDMainWard>().ToggleSeamless(seamless);
	}

	[ListedMethod(true)]
	public void ToggleWardTVs(bool enabled)
	{
		hospitalWard.GetComponent<RDMainWard>().ToggleTVs(enabled);
	}

	[ListedMethod(true)]
	public void PopBalloons(float percentage)
	{
		sparklineBalloons.PopBalloons(percentage);
	}

	[ListedMethod(true)]
	public void SetBalloonsSpawnDelay(float delay)
	{
		sparklineBalloons.SetSpawnDelay(delay);
	}

	[ListedMethod(true)]
	public void SetBalloonsSortingOrder(int layer, int order)
	{
		sparklineBalloons.SetSortingOrder(layer, order);
	}

	[ListedMethod(true)]
	public void FlowingDiamonds(float arc, float direction)
	{
		ParticleSystem.ShapeModule shape = diamondParticles.shape;
		shape.randomDirectionAmount = 0f;
		ParticleSystem.ShapeModule shape2 = diamondParticles.subEmitters.GetSubEmitterSystem(0).shape;
		shape2.randomDirectionAmount = 0f;
		shape2.arc = arc;
		shape2.rotation = new Vector3(0f, 0f, direction);
	}

	[ListedMethod(true)]
	public void SpawnParticle(string particleName, float x, float y, float scale)
	{
		if (!base.conductor.isScrubbing)
		{
			UnityEngine.Object obj = Resources.Load("PublicPrefabs/CustomParticles/" + particleName);
			if (!(obj == null))
			{
				GameObject obj2 = UnityEngine.Object.Instantiate(obj, scrGameManager.instance.parentContainer) as GameObject;
				obj2.transform.LocalMoveXY(3000f * (float)index + x / 100f * RDBase.Vfx.RDWidth, y / 100f * RDBase.Vfx.RDHeight);
				obj2.transform.localScale *= scale;
			}
		}
	}

	[ListedMethod(true)]
	public void DarkenedRollerdisco(bool night)
	{
		if (!(rollerDisco == null))
		{
			rollerDisco.GetComponent<RDRollerDisco>().SetNightMode(night);
		}
	}

	[ListedMethod(true)]
	public void TintRecordsRoom(bool tintForeground, string colorHex, float opacity, float beats, string easeStr)
	{
		if (!(recordsRoom == null))
		{
			Ease ease = Enum.Parse<Ease>(easeStr);
			Color endValue = colorHex.HexToColor();
			RDRecordsRoom component = recordsRoom.GetComponent<RDRecordsRoom>();
			tk2dSprite tk2dSprite2 = (tintForeground ? component.foreground : component.background);
			MeshRenderer component2 = tk2dSprite2.GetComponent<MeshRenderer>();
			float duration = base.conductor.DurationBeatsToTime(base.conductor.currentBeat, beats);
			component2.material.DOColor(endValue, "_ColorOverlay", duration).SetEase(ease);
			tk2dSprite2.DOColor(tk2dSprite2.color.WithAlpha(opacity), duration).SetEase(ease);
		}
	}

	[ListedMethod(true)]
	public void EnableTypingIan(bool enable)
	{
		basement.GetComponent<RDBasement>().typingIan.SetActive(enable);
	}

	public void SetCustomBackgroundDepth(int order)
	{
		customBackgroundDepth = order;
		if (customBackground != null)
		{
			RDTiledBackground component = customBackground.GetComponent<RDTiledBackground>();
			component.setSortingLayer.sortingOrder = customBackgroundDepth;
			component.setSortingLayer.Update();
		}
	}

	public void SetScrollOffset(float xDistance, float durationInBeats, string ease)
	{
		if (!(rollerDisco == null))
		{
			RDRollerDisco comp = rollerDisco.GetComponent<RDRollerDisco>();
			float endValue = comp.xOffset + xDistance;
			float duration = base.conductor.DurationBeatsToTime(base.conductor.currentBeat, durationInBeats, calledCloseToNextBarMaybe: true);
			Ease ease2 = RDUtils.ParseEnum(ease, Ease.Linear);
			DOTween.To(() => comp.xOffset, (float x) =>
			{
				comp.xOffset = x;
			}, endValue, duration).SetEase(ease2);
		}
	}

	public void SetScrollSpeed(float speed, float durationInBeats, string ease)
	{
		if (!(rollerDisco == null))
		{
			RDRollerDisco comp = rollerDisco.GetComponent<RDRollerDisco>();
			float duration = base.conductor.DurationBeatsToTime(base.conductor.currentBeat, durationInBeats, calledCloseToNextBarMaybe: true);
			Ease ease2 = RDUtils.ParseEnum(ease, Ease.Linear);
			DOTween.To(() => comp.scrollSpeed, (float x) =>
			{
				comp.scrollSpeed = x;
			}, speed, duration).SetEase(ease2);
		}
	}

	[ListedMethod(true)]
	public void ShowBossStageText(float xOffset, float yOffset, string colorHex)
	{
		if (!base.conductor.isScrubbing)
		{
			RDBossStageText rDBossStageText = base.game.currentLevel.LoadLevelAsset<RDBossStageText>();
			float num = 3000f * (float)index + RDBase.Vfx.RDWidth * 0.5f;
			float num2 = RDBase.Vfx.RDHeight * 0.5f;
			rDBossStageText.transform.localPosition = new Vector3(num + xOffset - 25f, num2 + yOffset, 20f);
			rDBossStageText.SetColor(colorHex.HexToColor());
			rDBossStageText.Play();
		}
	}

	[ListedMethod(true)]
	public void SetRollerdiscoScrollSpeed(float speed, float durationInBeats, string ease)
	{
		SetScrollSpeed(speed, durationInBeats, ease);
	}

	[ListedMethod(true)]
	public void SetRollerdiscoScrollOffset(float xDistance, float durationInBeats, string ease)
	{
		SetScrollOffset(xDistance, durationInBeats, ease);
	}

	public void SetRollerdiscoScrollPosition(float xPos)
	{
		if (!(rollerDisco == null))
		{
			rollerDisco.GetComponent<RDRollerDisco>().ScrollTo(xPos);
		}
	}

	[ListedMethod(true)]
	public void SetEyesVerticalShift(bool isSmall, float speed)
	{
		(isSmall ? eyesSmall : eyesBig)?.SetVerticalShiftSpeed(speed);
	}

	[ListedMethod(true)]
	public void SetEyesColor(bool isSmall, bool isOutline, string colorHex, float opacity, float durationInBeats, string easeStr)
	{
		RDManyEyes rDManyEyes = (isSmall ? eyesSmall : eyesBig);
		if (rDManyEyes != null)
		{
			Color color = colorHex.HexToColor().WithAlpha(opacity);
			float duration = base.conductor.DurationBeatsToTime(base.conductor.currentBeat, durationInBeats);
			Ease ease = Enum.Parse<Ease>(easeStr);
			rDManyEyes.SetEyePartColor(!isOutline, color, duration, ease);
		}
	}

	[ListedMethod(true)]
	public void SetEyesBackgroundColor(bool isSmall, string colorHex, float opacity, float durationInBeats, string easeStr)
	{
		RDManyEyes rDManyEyes = (isSmall ? eyesSmall : eyesBig);
		if (rDManyEyes != null)
		{
			Color color = colorHex.HexToColor().WithAlpha(opacity);
			float duration = base.conductor.DurationBeatsToTime(base.conductor.currentBeat, durationInBeats);
			Ease ease = Enum.Parse<Ease>(easeStr);
			rDManyEyes.SetEyeBackgroundColor(color, duration, ease);
		}
	}

	[ListedMethod(true)]
	public void SetEyesFocus(bool isSmall, float screenX, float screenY, float durationInBeats, string easeStr)
	{
		RDManyEyes rDManyEyes = (isSmall ? eyesSmall : eyesBig);
		if (rDManyEyes != null)
		{
			float duration = base.conductor.DurationBeatsToTime(base.conductor.currentBeat, durationInBeats);
			Ease ease = Enum.Parse<Ease>(easeStr);
			rDManyEyes.SetEyesTarget((screenX - 50f) * RDBase.Vfx.RDWidth / 100f, (screenY - 50f) * RDBase.Vfx.RDHeight / 100f, duration, ease);
		}
	}

	[ListedMethod(true)]
	public void SetEyesFocusRow(bool isSmall, int rowID)
	{
		RDManyEyes rDManyEyes = (isSmall ? eyesSmall : eyesBig);
		if (rDManyEyes != null)
		{
			if (rowID < 0)
			{
				rDManyEyes.ResetStare();
			}
			else
			{
				rDManyEyes.StareAtRow(rowID);
			}
		}
	}

	[ListedMethod(true)]
	public void EyesBlink(bool isSmall, float percent)
	{
		RDManyEyes rDManyEyes = (isSmall ? eyesSmall : eyesBig);
		if (rDManyEyes != null)
		{
			rDManyEyes.EyesBlink(Mathf.Clamp(percent / 100f, 0f, 100f));
		}
	}

	[ListedMethod(true)]
	public void EyesToggle(bool isSmall, float percent, bool equally)
	{
		RDManyEyes rDManyEyes = (isSmall ? eyesSmall : eyesBig);
		if (rDManyEyes != null)
		{
			float percent2 = Mathf.Clamp(percent / 100f, 0f, 100f);
			if (equally)
			{
				rDManyEyes.EyesToggleEqually(percent2);
			}
			else
			{
				rDManyEyes.EyesToggle(percent2);
			}
		}
	}

	private static void MakePerspectiveMesh(Mesh mesh)
	{
		mesh.Clear();
		int num = 16;
		int num2 = num + 1;
		float num3 = 1f / (float)num;
		Vector3[] array = new Vector3[num2 * num2];
		Vector2[] array2 = new Vector2[num2 * num2];
		int[] array3 = new int[num * num * 6];
		int num4 = 0;
		for (float num5 = 0f; num5 < (float)num2; num5++)
		{
			for (float num6 = 0f; num6 < (float)num2; num6++)
			{
				array[num4] = new Vector3(num6, num5, 0f);
				array2[num4++] = new Vector2(num6 * num3, num5 * num3);
			}
		}
		num4 = 0;
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < num; j++)
			{
				array3[num4] = i * num2 + j;
				array3[num4 + 1] = (i + 1) * num2 + j;
				array3[num4 + 2] = i * num2 + j + 1;
				array3[num4 + 3] = (i + 1) * num2 + j;
				array3[num4 + 4] = (i + 1) * num2 + j + 1;
				array3[num4 + 5] = i * num2 + j + 1;
				num4 += 6;
			}
		}
		mesh.vertices = array;
		mesh.uv = array2;
		mesh.triangles = array3;
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
	}
}
