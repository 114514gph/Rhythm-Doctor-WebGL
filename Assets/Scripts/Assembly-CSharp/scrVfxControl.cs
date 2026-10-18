using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

public class scrVfxControl : RDBase
{
	public struct RoomState
	{
		public int index;

		public bool willAppear;

		public GameObject quad;

		public RoomState(int index, bool toAppear)
		{
			this.index = index;
			willAppear = toAppear;
			quad = null;
		}
	}

	private const float SpotlightScaleXFactor = 40f;

	public const int TopCameraIndex = -1;

	public const int MinimumResolutionScale = 2;

	public const int CanvasWidth = 352;

	public const int CanvasHeight = 198;

	public static readonly Vector2Int CanvasSize = new Vector2Int(352, 198);

	public const float DefaultAspectRatio = 1.7777778f;

	public const int MinWindowWidth = 704;

	public const int MinWindowHeight = 396;

	public readonly float RDWidth = 352f;

	private static scrVfxControl _instance;

	public tk2dSpriteCollectionData scBackground;

	public tk2dSpriteCollectionData scLevelElements;

	[Header("Prefabs")]
	public GameObject quad;

	public GameObject hitStripPrefab;

	public GameObject spotlightPrefab;

	public GameObject letterboxPrefab;

	public GameObject matrixModePrefab;

	public GameObject shadowModePrefab;

	public GameObject shockwavePrefab;

	public GameObject lyricsPrefab;

	public GameObject lyricsSmallPrefab;

	public GameObject particleSpawnerPrefab;

	public GameObject invertedSpotlightsPrefab;

	public GameObject sevenBeatArmyPrefab;

	public GameObject scrollyTextPrefab;

	public GameObject lightStripPrefab;

	public GameObject snowPrefab;

	public GameObject diamondsParticleSystemPrefab;

	public GameObject sparklineBalloonsPrefab;

	[NonSerialized]
	public Background fxNoise;

	[NonSerialized]
	public Background fxGlitch;

	[NonSerialized]
	public scrQuad hudCameraOverlay;

	[NonSerialized]
	public Transform topObjects;

	[NonSerialized]
	public List<Background> backgrounds = new List<Background>();

	[NonSerialized]
	public RDTiledBackground customForeground;

	public GameObject uiFxGlitch;

	public Dictionary<int, LyricsGame> allVfxLyrics;

	public Dictionary<string, LyricsGame> allDecorationLyrics;

	private GameObject letterboxTop;

	private GameObject letterboxBottom;

	private GameObject spotlight;

	public TextMesh[] flashText;

	private Text flashTextUI;

	private float defaultSpotlightWidth;

	private Sequence[] bassDropSequences = new Sequence[5];

	public RDShaderProperties globalShaderData = new RDShaderProperties();

	[NonSerialized]
	public float roomsAnimationDuration = 1f;

	[NonSerialized]
	public float scrollXMultiplier = 1f;

	[NonSerialized]
	public float scrollYMultiplier = 1f;

	[NonSerialized]
	public bool hiResVideoBloom = true;

	[NonSerialized]
	public bool hiResShockwaves = true;

	[NonSerialized]
	public bool firstFrame = true;

	private GameObject matrixModeQuad;

	public Color _matrixModeColor;

	[NonSerialized]
	public float flashIntensity = 1f;

	[NonSerialized]
	public float backgroundDimming;

	private bool _matrixMode;

	private bool mInvertScreen;

	public OneshotSpotlights oneshotSpotlights;

	public RDSevenBeatArmy sevenBeatArmy;

	[NonSerialized]
	public string debugText = "";

	[NonSerialized]
	public int lastBackgroundOrder;

	[NonSerialized]
	public Background currentObstruction;

	[NonSerialized]
	public bool scaleBackgroundsAndRows;

	private int currentCycleColorID;

	private uint[] ColorPalette = new uint[11]
	{
		4291189578u, 4291198794u, 4283090599u, 4283081414u, 4283070918u, 4283058886u, 4285745862u, 4288039622u, 4290136774u, 4291185316u,
		4291185279u
	};

	public const int AllRooms = -1;

	public const int FullResolutionRenderTexture = 0;

	private Tween maskedTextTween;

	private Tween[] flashTextTween;

	private scrExecuteOnCertainBeat flashTextUIStartExecute;

	private scrExecuteOnCertainBeat flashTextUIEndExecute;

	public static Dictionary<string, CachedTexture> textureCache = new Dictionary<string, CachedTexture>();

	public static Dictionary<string, CachedFont> fontCache = new Dictionary<string, CachedFont>();

	public static scrVfxControl instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = UnityEngine.Object.FindObjectOfType<scrVfxControl>();
			}
			return _instance;
		}
	}

	public float RDHeight => 198f;

	public int RDWidthInt => Mathf.RoundToInt(RDWidth);

	public int RDHeightInt => Mathf.RoundToInt(RDHeight);

	public Camera hudCamera => base.gm.hudCamera;

	public RDCamera hudRDCamera => base.gm.hudRDCamera;

	public RDRoom[] rooms => base.game.rooms;

	public Color spriteGlowColor
	{
		set
		{
			globalShaderData.borderColor = value;
		}
	}

	public float spriteGlowPower
	{
		set
		{
			globalShaderData.glowPower = value;
		}
	}

	public Color matrixModeColor
	{
		get
		{
			return _matrixModeColor;
		}
		set
		{
			_matrixModeColor = value;
			matrixModeQuad.GetComponent<Renderer>().material.SetColor("_Tint", _matrixModeColor);
		}
	}

	public bool matrixMode
	{
		get
		{
			return _matrixMode;
		}
		set
		{
			_matrixMode = value;
			matrixModeQuad.SetActive(value);
		}
	}

	public bool invertScreen
	{
		get
		{
			return mInvertScreen;
		}
		set
		{
			if (RDUtils.IsInAccessibilityScope() && flashIntensity < 0.3f)
			{
				value = false;
			}
			mInvertScreen = value;
			hudCamera.GetComponent<CameraFilterPack_Color_Invert>().enabled = value;
		}
	}

	public bool infiniteZoom
	{
		get
		{
			if (sevenBeatArmy != null)
			{
				return sevenBeatArmy.enabled;
			}
			return false;
		}
		set
		{
			if (sevenBeatArmy == null)
			{
				sevenBeatArmy = UnityEngine.Object.Instantiate(sevenBeatArmyPrefab, base.gm.parentContainer).GetComponent<RDSevenBeatArmy>();
			}
			sevenBeatArmy.enabled = value;
			sevenBeatArmy.EnableEffect(value);
		}
	}

	public static Vector2Int fullResolutionRenderTextureSize
	{
		get
		{
			if (!((float)Screen.width / (float)Screen.height > 1.7777778f))
			{
				return new Vector2Int(Screen.width, Mathf.RoundToInt((float)Screen.width / 1.7777778f));
			}
			return new Vector2Int(Mathf.RoundToInt((float)Screen.height * 1.7777778f), Screen.height);
		}
	}

	private void Awake()
	{
		scLevelElements.inst.InitDictionary();
		RDBase.Vfx = this;
		topObjects = RDBase.CreateIfNotFound("TopObjects").transform;
		hudCameraOverlay = quad.Instantiate("FullscreenOverlay", makeContainer: true).GetComponent<scrQuad>();
		hudCameraOverlay.transform.parent = topObjects;
		hudCameraOverlay.GetComponent<Renderer>().material.renderQueue = 3000;
		RDShaderProperties.Setup();
		allVfxLyrics = new Dictionary<int, LyricsGame>();
		allDecorationLyrics = new Dictionary<string, LyricsGame>();
	}

	private void Start()
	{
		hudCameraOverlay.sprite.color = Color.black;
		hudCameraOverlay.gameObject.layer = GC.GameHUDLayer;
		if (1 == 0)
		{
			RDBase.Vfx.hiResShockwaves = false;
			RDBase.Vfx.hiResVideoBloom = false;
		}
		flashIntensity = (float)Persistence.GetFlashIntensity() * 0.01f;
		backgroundDimming = 1f - (float)Persistence.GetBackgroundDimming() * 0.01f;
	}

	private void Update()
	{
		if (firstFrame)
		{
			firstFrame = false;
			InitializeLetterbox();
			InitializeSpotlight();
			InitializeMatrixMode();
			if (scnBase.instance is scnLevelSelect { wrldCamera: var wrldCamera })
			{
				float num = 2f * wrldCamera.orthographicSize + 1f;
				float num2 = num * wrldCamera.aspect + 1f;
				hudCameraOverlay.GetComponent<tk2dSprite>().scale = new Vector3(num2, num, 1f);
			}
			else
			{
				hudCameraOverlay.SetSquare(Mathf.CeilToInt((0f - RDWidth) * 0.5f * 2f) - 1, Mathf.CeilToInt((0f - RDHeight) * 0.5f * 2f) - 1, Mathf.CeilToInt(RDWidth * 0.5f * 2f) + 1, Mathf.CeilToInt(RDHeight * 0.5f * 2f) + 1, 201);
			}
			hudCameraOverlay.transform.localPosition = hudCameraOverlay.transform.localPosition.WithZ(5f);
			hudCamera.transform.localPosition = new Vector3(RDWidth * 0.5f, RDHeight * 0.5f, -200f);
			topObjects.transform.localPosition = hudCamera.transform.localPosition;
			if (scnBase.instance is scnLevelSelect)
			{
				Vector3 scale = hudCameraOverlay.GetComponent<tk2dSprite>().scale;
				hudCameraOverlay.transform.localPosition = new Vector3(scale.x / -2f, scale.y / 2f, 1f);
			}
		}
	}

	public void LoadGameEffects()
	{
		flashText = new TextMesh[4];
		flashTextTween = new Tween[4];
		for (int i = 0; i < 4; i++)
		{
			flashText[i] = GameObject.Find("FlashText" + i).GetComponent<TextMesh>();
			flashText[i].gameObject.SetActive(value: false);
			float num = 3000f * (float)i + RDWidth / 2f;
			flashText[i].transform.LocalMoveX(num);
			flashText[i].text = "";
		}
		flashTextUI = base.game.flashTextUI;
		fxGlitch = Background.CreateSingle("Glitch", new int[2] { 0, 1 }, base.game.reducedFlash ? 8f : 60f, -1, isForeground: true);
		fxGlitch.visible = false;
		fxGlitch.gameObject.layer = GC.GameHUDLayer;
		fxNoise = Background.CreateSingle("Noise", new int[4] { 0, 1, 2, 3 }, base.game.reducedFlash ? 8f : 60f, -1, isForeground: true);
		fxNoise.visible = false;
		fxNoise.gameObject.layer = GC.GameHUDLayer;
	}

	public void TurnOnPixelPerfect()
	{
		new Vector2Int(-1, -1);
		int width = Screen.width;
		int height = Screen.height;
		if (width == 1136 && height == 640)
		{
			new Vector2Int(379, 213);
		}
		else if (width == 1024 && height == 768)
		{
			new Vector2Int(341, 256);
		}
		else if (width == 1334 && height == 750)
		{
			new Vector2Int(334, 188);
		}
		else if (width == 1920 && height == 1080)
		{
			new Vector2Int(384, 216);
		}
		else if (width == 1280 && height == 800)
		{
			new Vector2Int(320, 200);
		}
		else if (width == 1280 && height == 720)
		{
			new Vector2Int(320, 180);
		}
		else
		{
			new Vector2Int(352, 198);
		}
	}

	public void ResetEffects()
	{
		globalShaderData = new RDShaderProperties();
	}

	private void OnGUI()
	{
		if (RDBase.debugSettings.Debug && debugText != "")
		{
			GUI.Label(new Rect(0f, 0f, 200f, 200f), "DebugVar | " + debugText);
		}
	}

	private string StateForBool(bool value)
	{
		if (!value)
		{
			return "Off";
		}
		return "On";
	}

	public void FadeOut(float time, int roomIndex, float timeInCrotchets)
	{
		scrExecuteOnCertainBeat.Add(() =>
		{
			FadeOut(time, roomIndex);
		}, timeInCrotchets);
	}

	public void FadeOut(float time = 0.5f, int roomIndex = -1)
	{
		scrQuad roomOverlay = GetRoomOverlay(roomIndex);
		roomOverlay.sprite.DOKill();
		roomOverlay.SetColor(Color.black.WithAlpha(0f));
		roomOverlay.ReverseFlash(time);
	}

	public void FadeIn(float duration, int roomIndex, float timeInCrotchets)
	{
		scrExecuteOnCertainBeat.Add(() =>
		{
			FadeIn(duration, roomIndex);
		}, timeInCrotchets);
	}

	public void FadeIn(float duration = 0.5f, int roomIndex = -1)
	{
		scrQuad roomOverlay = GetRoomOverlay(roomIndex);
		roomOverlay.sprite.DOKill();
		roomOverlay.SetColor(Color.black);
		roomOverlay.Flash(duration);
	}

	public void SetOverlayToAlpha(float alpha, int roomIndex = -1)
	{
		scrQuad roomOverlay = GetRoomOverlay(roomIndex);
		roomOverlay.sprite.DOKill();
		roomOverlay.SetColor(Color.black.WithAlpha(alpha));
	}

	public void Strobe(int roomIndex, float startTimeInCrotchets, float PeriodInCrotchets, int repeats, float lightPercent = 10f)
	{
		for (int i = 0; i < repeats; i++)
		{
			BlackoutSingleStrobe(roomIndex, startTimeInCrotchets + PeriodInCrotchets * (float)i + PeriodInCrotchets * 0.01f * lightPercent, startTimeInCrotchets + PeriodInCrotchets * (float)i + PeriodInCrotchets);
		}
	}

	public void BlackoutSingleStrobe(int roomIndex, float startTimeInCrotchets, float endTimeInCrotchets)
	{
		scrQuad roomOverlay = GetRoomOverlay(roomIndex);
		scrExecuteOnCertainBeat.Add(() =>
		{
			roomOverlay.SetColor(Color.black);
		}, startTimeInCrotchets);
		scrExecuteOnCertainBeat.Add(() =>
		{
			roomOverlay.SetColor(Color.black.WithAlpha(0f));
		}, endTimeInCrotchets);
	}

	public void Flash(int roomIndex, float timeInCrotchets)
	{
		scrExecuteOnCertainBeat.Add(() =>
		{
			Flash(roomIndex);
		}, timeInCrotchets);
	}

	public void Flash(int roomIndex = -1, float time = 0.5f, float alpha = 1f)
	{
		scrQuad roomOverlay = GetRoomOverlay(roomIndex);
		Color color = Color.white.WithAlpha(alpha).WithBrightness(flashIntensity) * 0.95f;
		roomOverlay.SetColor(color);
		roomOverlay.Flash(time, color.a);
	}

	public void ReverseFlash(float time = 0.5f, int roomIndex = -1)
	{
		scrQuad roomOverlay = GetRoomOverlay(roomIndex);
		Color color = Color.white.WithBrightness(flashIntensity) * 0.95f;
		roomOverlay.SetColor(color);
		roomOverlay.ReverseFlash(time, color.a);
	}

	public void ReverseFlashBg(float time = 1f, int roomIndex = -1)
	{
		scrQuad roomBackgroundOverlay = GetRoomBackgroundOverlay(roomIndex);
		Color color = Color.white.WithBrightness(flashIntensity) * 0.95f;
		roomBackgroundOverlay.SetColor(color);
		roomBackgroundOverlay.ReverseFlash(1f, color.a);
	}

	public void BlackScreenOn(int roomIndex = -1)
	{
		GetRoomOverlay(roomIndex).SetColor(Color.black);
	}

	public void BlackScreenOff(int roomIndex = -1)
	{
		GetRoomOverlay(roomIndex).SetColor(Color.black.WithAlpha(0f));
	}

	public void GreenScreenOn(int roomIndex = -1)
	{
		GetRoomOverlay(roomIndex).SetColor(matrixModeColor);
	}

	public void GreenScreenOff(int roomIndex = -1)
	{
		GetRoomOverlay(roomIndex).SetColor(Color.black.WithAlpha(0f));
	}

	public scrQuad GetRoomOverlay(int roomIndex)
	{
		scrQuad obj = ((roomIndex == -1) ? hudCameraOverlay : rooms[roomIndex].overlay);
		if ((object)obj != null)
		{
			obj.gameObject.SetActive(value: true);
			return obj;
		}
		return obj;
	}

	public void DarkenBackgrounds(int roomIndex = 0, float time = 0.5f, float alpha = 0.7f)
	{
		scrQuad roomBackgroundOverlay = GetRoomBackgroundOverlay(roomIndex);
		roomBackgroundOverlay.sprite.DOKill();
		roomBackgroundOverlay.sprite.DOFade(alpha, time);
	}

	public void FlashBg(int roomIndex = 0, float time = 0.5f, float alpha = 1f)
	{
		scrQuad roomBackgroundOverlay = GetRoomBackgroundOverlay(roomIndex);
		Color color = Color.white.WithAlpha(alpha).WithBrightness(flashIntensity) * 0.95f;
		roomBackgroundOverlay.SetColor(color);
		roomBackgroundOverlay.Flash(0.5f, color.a);
	}

	public void LightenBackgrounds(int roomIndex = 0, float time = 0.5f)
	{
		scrQuad roomBackgroundOverlay = GetRoomBackgroundOverlay(roomIndex);
		roomBackgroundOverlay.sprite.DOKill();
		roomBackgroundOverlay.sprite.DOFade(0f, time);
	}

	public scrQuad GetRoomBackgroundOverlay(int roomIndex)
	{
		return rooms[roomIndex].backgroundOverlay;
	}

	public void ShoutEffect()
	{
		if (!base.game)
		{
			return;
		}
		if (rooms != null && rooms.Length != 0)
		{
			rooms[0].camera.Shake(10, 5);
			Color temp = rooms[0].camera.backgroundColor;
			Color color = Color.white.WithAlpha(0.5f).WithBrightness(flashIntensity) * 0.95f;
			BgColor(color);
			GetRoomOverlay(0);
			Flash(0, 0.3f, color.a);
			DOVirtual.DelayedCall(0.1f, () =>
			{
				BgColor(temp);
			});
		}
		else
		{
			RDBaseDllDummy.printesw("there's no rooms!!");
		}
	}

	public RDCamera GetCamera(int room)
	{
		if (room != -1)
		{
			return rooms[room].rdCamera;
		}
		return hudRDCamera;
	}

	public void ShakeCam(int jitterCount, int jitterAmount, int room = 0)
	{
		if (!base.conductor.isScrubbing)
		{
			GetCamera(room).Shake(jitterCount, jitterAmount);
		}
	}

	public void ShakeCamSmooth(float duration, float strength, int room = 0)
	{
		RDCamera camera = GetCamera(room);
		camera.ClearSmoothShake();
		camera.smoothShake = camera.transform.DOShakePosition(duration, strength);
	}

	public void StopShakeCam(int room = 0)
	{
		GetCamera(room).StopShake();
	}

	public void ShakeCamRotate(float duration, float strength, int room = 0)
	{
		RDCamera camera = GetCamera(room);
		camera.ClearRotateShake();
		camera.rotateShake = camera.transform.DOShakeRotation(duration, strength);
	}

	public void BgColor(Color color, int cameraIndex = 0)
	{
		GetCamera(cameraIndex).camera.backgroundColor = color.WithBrightness(backgroundDimming, fadeAlpha: false);
	}

	public void BgColor(uint color, int cameraIndex = 0)
	{
		GetCamera(cameraIndex).camera.backgroundColor = color.ARGBToColor().WithBrightness(backgroundDimming, fadeAlpha: false);
	}

	public void SetSpotlightParent(Transform transform)
	{
		spotlight.transform.parent = transform;
	}

	private void InitializeSpotlight()
	{
		Camera camera = hudCamera;
		GameObject gameObject = UnityEngine.Object.Instantiate(spotlightPrefab);
		MeshRenderer component = gameObject.GetComponent<MeshRenderer>();
		Texture mainTexture = component.material.mainTexture;
		int num = mainTexture.width / mainTexture.height;
		float num2 = camera.RDFullNativeResolution().y;
		defaultSpotlightWidth = num2 * (float)num * 8f;
		gameObject.transform.localScale = new Vector3(defaultSpotlightWidth, num2, 1f);
		gameObject.transform.parent = topObjects.transform;
		gameObject.transform.localPosition = new Vector3(0f, 0f, 1f);
		gameObject.name = "Spotlight";
		gameObject.SetActive(value: false);
		component.sortingOrder = 100;
		spotlight = gameObject;
	}

	private void InitializeLetterbox()
	{
		for (int i = 0; i < 2; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(letterboxPrefab);
			MakeQuadFullscreen(gameObject, 0f);
			gameObject.GetComponent<MeshRenderer>().sortingLayerName = RDSortingLayer.Foreground.ToString();
			gameObject.transform.parent = topObjects.transform;
			gameObject.layer = GC.GameHUDLayer;
			if (i == 0)
			{
				letterboxTop = gameObject;
				letterboxTop.name = "Letterbox Top";
			}
			else
			{
				letterboxBottom = gameObject;
				letterboxBottom.name = "Letterbox Bottom";
			}
			gameObject.SetActive(value: false);
		}
	}

	private void InitializeMatrixMode()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(matrixModePrefab);
		gameObject.transform.parent = topObjects.transform;
		gameObject.name = "Matrix Mode";
		gameObject.GetComponent<MeshRenderer>().sortingLayerName = RDSortingLayer.Foreground.ToString();
		gameObject.SetActive(value: false);
		gameObject.transform.localScale = new Vector3(RDHeight * (float)Screen.width * 1f / (float)Screen.height, RDHeight, 1f);
		gameObject.transform.localPosition = new Vector3(0f, 0f, 1f);
		matrixModeQuad = gameObject;
		matrixModeColor = new Color(0.1f, 0.75f, 0.16f, 0.3f);
	}

	public tk2dSprite InitializeVignette(Camera camera)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(shadowModePrefab);
		gameObject.transform.parent = topObjects.transform;
		gameObject.name = "Vignette";
		gameObject.GetComponent<MeshRenderer>().sortingLayerName = RDSortingLayer.Foreground.ToString();
		gameObject.SetActive(value: false);
		tk2dSprite component = gameObject.GetComponent<tk2dSprite>();
		Bounds bounds = component.CurrentSprite.GetBounds();
		Vector2 vector = new Vector2(bounds.extents.x * 2f, bounds.extents.y * 2f);
		float num = 1.2f;
		component.scale = new Vector3(RDWidth * num / vector.x, RDHeight * num / vector.y);
		gameObject.transform.localPosition = new Vector3(0f, 0f, 1.5f);
		return component;
	}

	private void MakeQuadFullscreen(GameObject quad, float zPosition)
	{
		quad.transform.localScale = new Vector3(RDWidth, RDHeight, 1f);
		quad.transform.localPosition = new Vector3(0f, 0f, zPosition);
	}

	public void ShowLetterbox(bool show, float percentCovered = 0.2f)
	{
		if ((!show && !letterboxTop.activeSelf && !letterboxBottom.activeSelf) || (show && letterboxTop.activeSelf && letterboxBottom.activeSelf))
		{
			return;
		}
		Camera camera = hudCamera;
		float num = percentCovered * camera.RDFullNativeResolution().y * 0.5f;
		float num2 = (0f - RDHeight) / 2f;
		float num3 = camera.RDFullNativeResolution().y * 1.5f + num2;
		float num4 = camera.RDFullNativeResolution().y * -0.5f + num2;
		float num5 = num3 - num;
		float num6 = num4 + num;
		float num7 = (show ? num3 : num5);
		float endValue = (show ? num5 : num3);
		float num8 = (show ? num4 : num6);
		float endValue2 = (show ? num6 : num4);
		letterboxTop.transform.localPosition = new Vector3(0f, num7, 10f);
		letterboxBottom.transform.localPosition = new Vector3(0f, num8, 10f);
		letterboxTop.SetActive(value: true);
		letterboxBottom.SetActive(value: true);
		letterboxTop.transform.DOLocalMoveY(endValue, 1f);
		letterboxBottom.transform.DOLocalMoveY(endValue2, 1f).OnComplete(() =>
		{
			if (!show)
			{
				letterboxTop.SetActive(value: false);
				letterboxBottom.SetActive(value: false);
			}
		});
	}

	public void SpotlightFocusIn(float x = -1f, bool animated = true, bool brightMode = false, bool playSound = false, float animDuration = 1f, float alpha = 1f, AudioMixerGroup group = null)
	{
		if (x == -1f)
		{
			x = RDWidth * 0.5f;
		}
		spotlight.SetActive(value: true);
		Vector3 position = spotlight.transform.position;
		position.x = x;
		position.z = 10f;
		spotlight.transform.position = position;
		spotlight.transform.localScale = spotlight.transform.localScale.WithX(animated ? (40f * defaultSpotlightWidth) : defaultSpotlightWidth);
		if (animated)
		{
			spotlight.transform.DOKill();
			spotlight.transform.DOScaleX(defaultSpotlightWidth, animDuration).SetEase(Ease.OutQuart);
		}
		if (playSound)
		{
			scrConductor.PlayImmediately("sndSwitch", 1f, group);
			RDBaseDllDummy.printem("Satellite: Switch sound played by this function.");
		}
		if (brightMode)
		{
			SetSpotLightAlpha(1f);
			Color value = new Color(244f / 255f, 1f, 31f / 255f, (50f * alpha + 0.02f) / 255f);
			spotlight.GetComponent<Renderer>().material.SetColor("_LightColor", value);
		}
		else
		{
			SetSpotLightAlpha(alpha);
			Color value2 = new Color(244f / 255f, 1f, 31f / 255f, 0f);
			spotlight.GetComponent<Renderer>().material.SetColor("_LightColor", value2);
		}
	}

	public void SetSpotLightAlpha(float alpha)
	{
		spotlight.GetComponent<MeshRenderer>().material.SetFloat("_Alpha", alpha);
	}

	public void SetSpotlightY(float y)
	{
		spotlight.transform.localPosition = spotlight.transform.localPosition.WithY(y);
	}

	public void SpotlightFocusFromCenter(float spotlightXPos = -1f, float alpha = 1f, bool playSwitchSound = false, AudioMixerGroup group = null, float vol = 1f)
	{
		MeshRenderer component = spotlight.GetComponent<MeshRenderer>();
		Material material = component.material;
		SetSpotLightAlpha(alpha);
		matrixMode = false;
		spotlight.SetActive(value: true);
		spotlight.transform.localScale = spotlight.transform.localScale.WithX(defaultSpotlightWidth);
		material.mainTextureScale = new Vector2(200f, 1f);
		material.mainTextureOffset = new Vector2(-99.5f, 0f);
		if (spotlightXPos != -1f)
		{
			Vector3 position = spotlight.transform.position;
			position.x = spotlightXPos;
			position.z = 10f;
			spotlight.transform.position = position;
		}
		DOTween.To(() => material.mainTextureScale, (Vector2 x) =>
		{
			material.mainTextureScale = x;
		}, new Vector2(8f, 1f), 0.35f).SetEase(Ease.OutCirc);
		DOTween.To(() => material.mainTextureOffset, (Vector2 x) =>
		{
			material.mainTextureOffset = x;
		}, new Vector2(-3.5f, 0f), 0.35f).SetEase(Ease.OutCirc);
		if (playSwitchSound)
		{
			scrConductor.PlayImmediately("sndSwitch", vol, group, 1f, 0f, isBeatSound: true);
		}
	}

	public void SpotlightOpenAllTheWay(float duration = 1f, bool playSwitchSound = true, AudioMixerGroup group = null)
	{
		if (spotlight.activeSelf)
		{
			spotlight.transform.localScale = spotlight.transform.localScale.WithX(defaultSpotlightWidth);
			spotlight.transform.DOScaleX(defaultSpotlightWidth * 40f, duration).OnComplete(() =>
			{
				spotlight.SetActive(value: false);
			});
		}
		if (playSwitchSound)
		{
			scrConductor.PlayImmediately("sndSpotlightSweep", 1f, group);
		}
	}

	public void SpotlightInstantOff(bool darkenScene = true, bool playSwitchSound = false, bool noSound = false)
	{
		spotlight.SetActive(value: false);
		if (darkenScene)
		{
			matrixMode = true;
			matrixModeColor = new Color(0f, 0f, 0f, 0.5f);
		}
		if (!noSound)
		{
			string sound = "sndTransitionShort";
			if (playSwitchSound)
			{
				sound = "sndSpotlightOff";
			}
			scrConductor.PlayImmediately(sound, 1f, RDUtils.GetMixerGroup("StoryLevelSelect"));
		}
	}

	public void SpotlightInstantOn()
	{
		spotlight.SetActive(value: true);
		scrConductor.PlayImmediately("sndTransitionShort", 1f, RDUtils.GetMixerGroup("StoryLevelSelect"));
	}

	public void SetVignette(bool enabled, int roomIndex = 0)
	{
		rooms[roomIndex].SetVignette(enabled);
	}

	public void SetVignetteAlpha(float alpha, int roomIndex = 0)
	{
		rooms[roomIndex].SetVignetteAlpha(alpha);
	}

	public void CycleOverlayColor(int room = -1)
	{
		int num = currentCycleColorID;
		while (currentCycleColorID == num)
		{
			currentCycleColorID = Mathf.FloorToInt(UnityEngine.Random.value * (float)ColorPalette.Length);
		}
		SetSpriteMode(SpriteMode.Silhouette, ColorPalette[currentCycleColorID].ARGBToColor(), room);
	}

	public Color GetRandomColor()
	{
		int num = Mathf.FloorToInt(UnityEngine.Random.value * (float)ColorPalette.Length);
		return ColorPalette[num].ARGBToColor();
	}

	public void SetSpriteMode(SpriteMode mode, Color color, int room = -1, float glowPower = 1f, bool resetOverrides = false)
	{
		Color overlayColor = Color.clear;
		Color borderColor = Color.clear;
		bool flag = false;
		switch (mode)
		{
		case SpriteMode.Glow:
			borderColor = color;
			flag = false;
			break;
		case SpriteMode.Outline:
			borderColor = color;
			flag = true;
			break;
		case SpriteMode.Silhouette:
			overlayColor = color;
			break;
		default:
			overlayColor = Color.clear;
			borderColor = Color.clear;
			break;
		}
		RDShaderProperties rDShaderProperties = new RDShaderProperties();
		rDShaderProperties.overlayColor = overlayColor;
		rDShaderProperties.borderColor = borderColor;
		rDShaderProperties.outline = (flag ? 1 : 0);
		rDShaderProperties.glowPower = glowPower;
		Row[] rows;
		if (room == -1)
		{
			if (resetOverrides && base.game != null)
			{
				rows = base.game.rows;
				foreach (Row row in rows)
				{
					if (row.ent != null)
					{
						row.ent.shaderDataSource = row.ent;
						row.ent.playerBox.shaderDataSource = row.ent.playerBox;
					}
				}
			}
			globalShaderData.CopyFrom(rDShaderProperties);
			return;
		}
		rows = base.game.rows;
		foreach (Row row2 in rows)
		{
			RowEntity ent = row2.ent;
			if ((bool)ent && row2.room == room)
			{
				ent.SetMaterial(usesOwnShaderData: true, rDShaderProperties);
			}
		}
	}

	public void ShowRooms(string newOrder, float timeInCrotchets, float[] heights = null)
	{
		scrExecuteOnCertainBeat.Add(() =>
		{
			ShowRooms(newOrder, heights);
		}, timeInCrotchets);
	}

	public void ShowRooms(string newOrder, float[] heights = null, Ease ease = Ease.Unset)
	{
		string[] array = newOrder.Split(',');
		List<RoomState> list = new List<RoomState>();
		int num = 0;
		string[] array2 = array;
		foreach (string text in array2)
		{
			string s = text;
			bool num2 = text.Contains("d");
			if (num2)
			{
				s = text.Replace("d", "");
			}
			else
			{
				num++;
			}
			int index = int.Parse(s);
			bool toAppear = !num2;
			list.Add(new RoomState(index, toAppear));
		}
		List<GameObject> list2 = new List<GameObject>();
		List<GameObject> renderTextureQuads = base.game.renderTextureQuads;
		foreach (RoomState item in list)
		{
			list2.Add(renderTextureQuads[item.index]);
		}
		int num3 = 0;
		float num4 = 0f;
		float num5 = 0f;
		foreach (GameObject item2 in list2)
		{
			_ = item2;
			RoomState roomState = list[num3];
			int index2 = roomState.index;
			bool willAppear = roomState.willAppear;
			RDRoom rDRoom = rooms[index2];
			if (roomState.willAppear || !rDRoom.customTransform)
			{
				float num6 = rDRoom.renderQuadPivot.localScale.y;
				if (!rDRoom.quadRenderer.enabled)
				{
					Rect normalizedRectToQuad = new Rect(0f, 1f - num4 - num6, 1f, num6);
					rDRoom.SetNormalizedRectToQuad(normalizedRectToQuad);
				}
				num4 += num6;
				float num7 = ((heights != null) ? heights[num3] : (willAppear ? (1f / (float)num) : 0f));
				Rect to = new Rect(0f, 1f - num5 - num7, 1f, num7);
				if (willAppear)
				{
					num5 += num7;
				}
				rDRoom.Rotate(0f, roomsAnimationDuration, ease);
				rDRoom.renderQuad.transform.DOKill();
				rDRoom.renderQuad.transform.DOLocalMove(Vector3.zero, roomsAnimationDuration).SetEase(ease);
				rDRoom.renderQuadPivot.transform.DOKill();
				rDRoom.AnimateQuad(rDRoom.NormalizedRect(), to, roomsAnimationDuration, !willAppear, ease);
				rDRoom.roomMovedByLevelEvent = true;
			}
			num3++;
		}
	}

	public void AdvanceLyricsText(int id)
	{
		if (allVfxLyrics.TryGetValue(id, out var value))
		{
			value.AdvanceText();
		}
		else
		{
			printe($"id {id.ToString()} doesn't exist");
		}
	}

	public RDParticleSystem SpawnParticles(string spriteName, int room, string animationSheet = null, string animationName = null, float baseSpeed = 150f)
	{
		return UnityEngine.Object.Instantiate(particleSpawnerPrefab).GetComponent<RDParticleSystem>().Setup(spriteName, room, animationSheet, animationName, baseSpeed);
	}

	public void SetFlipScreenX(bool x, int roomIndex = -1)
	{
		CameraFilterPack_FlipScreen orAddComponent = GetCamera(roomIndex).GetOrAddComponent<CameraFilterPack_FlipScreen>();
		orAddComponent.flipX = x;
		orAddComponent.UpdateState();
	}

	public void SetFlipScreenY(bool y, int roomIndex = -1)
	{
		CameraFilterPack_FlipScreen orAddComponent = GetCamera(roomIndex).GetOrAddComponent<CameraFilterPack_FlipScreen>();
		orAddComponent.flipY = y;
		orAddComponent.UpdateState();
	}

	public void SetTileScreen(Vector2 tiles, int roomIndex = -1)
	{
		RDImageEffectTile orAddComponent = GetCamera(roomIndex).GetOrAddComponent<RDImageEffectTile>();
		orAddComponent.tileX = tiles.x;
		orAddComponent.tileY = tiles.y;
		orAddComponent.UpdateState();
	}

	public void ZoomScreen(float zoom, float offsetX = 0f, float offsetY = 0f, int roomIndex = -1)
	{
		RDImageEffectTile orAddComponent = GetCamera(roomIndex).GetOrAddComponent<RDImageEffectTile>();
		orAddComponent.offsetX = offsetX;
		orAddComponent.offsetY = offsetY;
		orAddComponent.zoom = zoom;
		orAddComponent.UpdateState();
	}

	public void ZoomBackToNormal()
	{
		ZoomScreen(1f);
	}

	public void SetHorizontalStripsVisible(bool visible)
	{
		RDRoom[] array = rooms;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].SetHorizontalStripsVisible(visible);
		}
	}

	public Background[] AddMatrixBackgrounds(int room = 0, float speed = 1f, float alpha = 1f)
	{
		Background background = Background.Create("Matrix/0", 0f, speed * 1f, isForeground: false, room).SetBackgroundOrder(5).SetScale(0.75f)
			.SetAlpha(alpha);
		Background background2 = Background.Create("Matrix/1", 0f, speed * 2f, isForeground: false, room).SetBackgroundOrder(6).SetScale(1.5f)
			.SetAlpha(alpha);
		Background background3 = Background.Create("Matrix/2", 0f, speed * 3f, isForeground: false, room).SetBackgroundOrder(7).SetScale(2.25f)
			.SetAlpha(alpha);
		return new Background[3] { background, background2, background3 };
	}

	public void MatchCanvasToCamera(Canvas canvas, Camera camera)
	{
		RectTransform component = canvas.GetComponent<RectTransform>();
		Vector2 vector = camera.RDFullNativeResolution();
		component.anchoredPosition = new Vector2(vector.x / 2f, vector.y / 2f);
		component.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, vector.x);
		component.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, vector.y);
		camera.transform.LocalMoveY(RDBase.Vfx.RDHeight * 0.5f);
	}

	public RenderTexture CreateRenderTexture(int scale = 0, string name = "Debug", int depth = 16)
	{
		Vector2Int size = ((scale == 0) ? fullResolutionRenderTextureSize : new Vector2Int(RDBase.Vfx.RDWidthInt * scale, RDBase.Vfx.RDHeightInt * scale));
		return CreateRenderTexture(size, name, depth);
	}

	public RenderTexture CreateRenderTexture(Vector2Int size, string name = "Debug", int depth = 16)
	{
		GraphicsFormat format = (PauseMenuContentData.usesConsoleWindowMovement ? GraphicsFormat.R8G8B8A8_UNorm : GraphicsFormat.B8G8R8A8_UNorm);
		return new RenderTexture(size.x, size.y, depth, format)
		{
			filterMode = FilterMode.Point,
			name = name
		};
	}

	public void ShowMaskedText(string text, int size = 28, float duration = 1000f, bool RDoc = false)
	{
		if (base.game == null)
		{
			return;
		}
		if (maskedTextTween != null)
		{
			maskedTextTween.Kill(complete: true);
		}
		if (text == "")
		{
			base.game.maskFlash.SetActive(value: false);
		}
		else
		{
			base.game.maskFlash.SetActive(value: true);
			if (text == "[blank]")
			{
				text = "";
			}
			base.game.textMask.text = text;
			base.game.textMask.fontSize = size;
			maskedTextTween = DOVirtual.DelayedCall(duration, () =>
			{
				base.game.maskFlash.SetActive(value: false);
			}, ignoreTimeScale: false);
		}
		if (RDoc)
		{
			base.game.textMask.lineSpacing = 0.77f;
			base.game.textMask.color = RDPresetColors.Teal.ARGBToColor();
		}
		else
		{
			base.game.textMask.lineSpacing = 1f;
			base.game.textMask.color = Color.white;
		}
	}

	public void HideMaskedText()
	{
		if (maskedTextTween != null)
		{
			maskedTextTween.Kill(complete: true);
		}
		base.game.maskFlash.SetActive(value: false);
	}

	public void FlashText(string text, float durationInCrotchets = 1f, float size = 1f, bool flash = true, int row = 0)
	{
		if (base.game.currentLevel.flashTextEnabled)
		{
			int room = base.game.rows[row].room;
			TextMesh textMesh = flashText[room];
			textMesh.gameObject.SetActive(value: true);
			scrQuad roomOverlay = GetRoomOverlay(room);
			flashTextTween[room]?.Kill(complete: true);
			if (flash)
			{
				roomOverlay.SetColor(Color.white.WithAlpha(0.5f));
				roomOverlay.Flash(0.25f, 0.5f);
			}
			textMesh.text = text;
			textMesh.font = RDString.currentFontPack.flashFont.font;
			MeshRenderer mr = textMesh.GetComponent<MeshRenderer>();
			mr.enabled = true;
			mr.material = textMesh.font.material;
			flashTextTween[room] = DOVirtual.DelayedCall(durationInCrotchets * base.conductor.crotchet, () =>
			{
				mr.enabled = false;
			});
		}
	}

	public void KillFlashTextUIAnimations()
	{
		if (flashTextUIStartExecute != null)
		{
			flashTextUIStartExecute.Remove();
		}
		if (flashTextUIEndExecute != null)
		{
			flashTextUIEndExecute.Remove();
		}
		flashTextUI.gameObject.SetActive(value: false);
	}

	public void FlashTextUI(string text, float startBeat, float endBeat, int size = 40, bool flash = true)
	{
		if (base.conductor.isScrubbing)
		{
			return;
		}
		flashTextUIStartExecute = scrExecuteOnCertainBeat.Add(() =>
		{
			flashTextUI.gameObject.SetActive(value: true);
			flashTextUI.text = text;
			flashTextUI.font = RDString.currentFontPack.flashFont.font;
			flashTextUI.fontSize = size;
			if (flashTextUIEndExecute != null)
			{
				flashTextUIEndExecute.Remove();
			}
			if (endBeat != 0f)
			{
				flashTextUIEndExecute = scrExecuteOnCertainBeat.Add(() =>
				{
					flashTextUI.text = "";
					flashTextUI.gameObject.SetActive(value: false);
				}, endBeat);
			}
			scrQuad roomOverlay = GetRoomOverlay(-1);
			if (flash)
			{
				roomOverlay.Flash(0.25f, 0.5f);
			}
		}, startBeat);
	}

	public void FlashTextUIInstant(string text, int size = 40, bool flash = true)
	{
		flashTextUI.text = text;
		flashTextUI.fontSize = size;
		if (flashTextUIEndExecute != null)
		{
			flashTextUIEndExecute.Remove();
		}
		scrQuad roomOverlay = GetRoomOverlay(-1);
		if (flash)
		{
			roomOverlay.Flash(0.25f, 0.5f);
		}
	}

	public void SetBloomParams(float threshold, float intensity, float kernelSize, uint tint, VideoBloom.BlendingMode bmode, int room = -1)
	{
		VideoBloom component = GetCamera(room).GetComponent<VideoBloom>();
		component.enabled = true;
		component.Threshold = threshold;
		component.masterAmount = intensity;
		component.KernelSize = kernelSize;
		component.Tint = tint.RGBAToColor();
		component.BlendMode = bmode;
	}

	public ParticleSystem ShowParticlePreset(string presetName, float seconds = -1f, int room = 0)
	{
		GameObject original = Resources.Load<GameObject>("Presets/ParticleSystem Presets/" + presetName);
		GameObject go = UnityEngine.Object.Instantiate(original, base.gm.parentContainer);
		go.transform.LocalMoveX(3000f * (float)room + go.transform.localPosition.x);
		if (seconds != -1f)
		{
			DOVirtual.DelayedCall(seconds, () =>
			{
				UnityEngine.Object.DestroyImmediate(go);
			});
		}
		return go.GetComponent<ParticleSystem>();
	}

	public void PlayBassDropEffect(int cameraIndex = -1)
	{
		if (!base.conductor.isScrubbing)
		{
			string stateName = (base.game.currentLevel.gentleBassDrop ? "Bass Drop Distortion Gentle" : "Bass Drop Distortion");
			Animator component = GetCamera(cameraIndex).GetComponent<Animator>();
			component.enabled = true;
			component.Play(stateName, 0, 0f);
		}
	}

	public void BassDropNew(int cameraIndex = -1, float intensity = 1f, float offsetDur = 0.35f, float blurDur = 0.5f, int vibrato = 14)
	{
		if (base.conductor.isScrubbing)
		{
			return;
		}
		if (base.game.currentLevel.oldBassDrop)
		{
			PlayBassDropEffect(cameraIndex);
			return;
		}
		GetCamera(cameraIndex).GetComponent<Animator>().enabled = false;
		CameraFilterPack_Color_Chromatic_Aberration ca = GetCamera(cameraIndex).GetComponent<CameraFilterPack_Color_Chromatic_Aberration>();
		CameraFilterPack_Blur_Radial_Fast blur = GetCamera(cameraIndex).GetComponent<CameraFilterPack_Blur_Radial_Fast>();
		if (!(ca != null) || !(blur != null))
		{
			return;
		}
		ca.enabled = true;
		blur.enabled = true;
		ca.Offset = 0.0016f;
		blur.Intensity = 0f;
		Sequence sequence = bassDropSequences[(cameraIndex == -1) ? 4 : cameraIndex];
		if (sequence != null)
		{
			sequence.Complete();
			sequence.Play();
			return;
		}
		sequence = DOTween.Sequence().Insert(0f, DOTween.Punch(() => Vector3.zero, (Vector3 v) =>
		{
			ca.Offset = v.x;
		}, Vector3.right * 0.02f * intensity, offsetDur, vibrato)).Insert(0f, DOTween.Punch(() => Vector3.zero, (Vector3 v) =>
		{
			blur.Intensity = v.x;
		}, Vector3.right * 0.08f * intensity, blurDur, vibrato));
	}

	public void CenterThis(Transform t)
	{
		t.transform.position = new Vector3(RDWidth * 0.5f, RDHeight * 0.5f, t.transform.position.z);
	}

	public RDTiledBackground GetForegroundTextureLayer(int index = -1)
	{
		if (index == -1)
		{
			if (customForeground == null)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(scnGame.instance.customBackground);
				gameObject.transform.SetParent(scrGameManager.instance.parentContainer);
				gameObject.layer = 11;
				gameObject.transform.localPosition = new Vector3(RDWidth / 2f, RDHeight / 2f, 10f);
				customForeground = gameObject.GetComponent<RDTiledBackground>();
			}
			return customForeground;
		}
		return rooms[index].GetTextureLayer(RDSortingLayer.Foreground);
	}

	public void InvertScreenColors(bool invert, int roomIndex = -1)
	{
		Camera obj = ((roomIndex == -1) ? base.game.topCamera.camera : rooms[roomIndex].camera);
		if (RDUtils.IsInAccessibilityScope() && flashIntensity < 0.3f)
		{
			invert = false;
		}
		obj.GetOrAddComponent<CameraFilterPack_Color_Invert>().enabled = invert;
	}

	public void BeatboxPulsed(Vector3 position, int number, string customText = "", float duration = 0.5f, int room = 0)
	{
		base.game.rooms[room].BeatboxPulsed(position, number, customText, duration);
	}

	public static void CheckKeyComboForResolutionChange()
	{
		if (Input.GetKeyDown(KeyCode.W) && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
		{
			CycleWindowResolutions((!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift)) ? 1 : (-1));
		}
	}

	public static int CycleWindowResolutions(int inc = 1)
	{
		int resolutionScale = Persistence.GetResolutionScale();
		int num = resolutionScale + inc;
		if (num < 2)
		{
			num = ((inc >= 0) ? 2 : (PauseMenuContentData.AvailableResolutions.Length - 1 + 2));
		}
		Debug.Log($"CycleWindowResolutions from {resolutionScale} -> {num}");
		if (!SetResolutionWithScale(num, Screen.fullScreenMode))
		{
			num = 2;
			SetResolutionWithScale(num, Screen.fullScreenMode);
		}
		Persistence.SetResolutionScale(num);
		return num;
	}

	public static bool ScaleIsFullResolution(int scale)
	{
		return scale - 2 == PauseMenuContentData.AvailableResolutions.Length - 1;
	}

	public static Vector2Int GetResolutionForScale(int scale)
	{
		if (ScaleIsFullResolution(scale))
		{
			Resolution resolution = PauseMenuContentData.AvailableResolutions[^1];
			RDBaseDllDummy.printem($"scale {scale} is full resolution ({resolution})");
			return new Vector2Int(resolution.width, resolution.height);
		}
		int num = 352 * scale;
		int num2 = 198 * scale;
		return new Vector2Int(num, num2);
	}

	public static Resolution GetHighestResolution()
	{
		int num = -1;
		int num2 = 0;
		Resolution[] resolutions = Screen.resolutions;
		if (resolutions.Length == 0)
		{
			Resolution currentResolution = Screen.currentResolution;
			Debug.Log($"currentResolution width: {currentResolution.width}, height {currentResolution.height}, refreshRate {currentResolution.refreshRateRatio}");
			if (currentResolution.width > 0 && currentResolution.height > 0)
			{
				Debug.Log($"returning normal {currentResolution.width} {currentResolution.height} {currentResolution.refreshRateRatio}");
				return currentResolution;
			}
			currentResolution.width = Screen.width;
			currentResolution.height = Screen.height;
			currentResolution.refreshRateRatio = Screen.mainWindowDisplayInfo.refreshRate;
			Debug.Log($"returning fallback {currentResolution.width} {currentResolution.height} {currentResolution.refreshRateRatio}");
			return currentResolution;
		}
		for (int i = 0; i < resolutions.Length; i++)
		{
			Resolution resolution = resolutions[i];
			int num3 = resolution.width * resolution.height;
			if (num3 > num2)
			{
				num = i;
				num2 = num3;
			}
		}
		return resolutions[num];
	}

	public static bool SetResolutionWithScale(int scale, FullScreenMode fullScreenMode)
	{
		Vector2Int resolutionForScale = GetResolutionForScale(scale);
		int num = resolutionForScale.x;
		int num2 = resolutionForScale.y;
		Resolution resolution = default;
		Vector2Int res = (PlatformHelper.instance as PlatformHelperWindows).GetCurrentMonitorResolution(IntPtr.Zero);
		resolution = new Resolution { width = res.x, height = res.y };
		if (num <= resolution.width && num2 <= resolution.height)
		{
			num = Math.Max(num, 704);
			num2 = Math.Max(num2, 396);
			Debug.Log($"Resolution with scale {scale}: {num}, {num2}, {fullScreenMode}");
			SetResolution(num, num2, fullScreenMode);
			return true;
		}
		Debug.Log($"resolution was not set because it was too big: {num} x {num2}, {resolution.ToString()}");
		return false;
	}

	public static void SetFullscreen(bool on)
	{
		if (on)
		{
			if (!Screen.fullScreen)
			{
				SetDefaultFullscreenResolution();
			}
			else
			{
				RDBaseDllDummy.printem("SetFullscreenOn(): screen was already fullscreen");
			}
		}
		else if (Screen.fullScreen)
		{
			Screen.fullScreenMode = FullScreenMode.Windowed;
			Debug.Log(Screen.fullScreenMode);
		}
		else
		{
			RDBaseDllDummy.printem("SetFullscreenOn(): screen was already windowed");
		}
	}

	public static void SetDefaultFullscreenResolution(bool isSteamDeck = false)
	{
		Resolution currentResolution = Screen.currentResolution;
		int width = currentResolution.width;
		int num = Math.Min(Mathf.RoundToInt((float)currentResolution.width / 1.7777778f), currentResolution.height);
		FullScreenMode fullscreenMode = ((!isSteamDeck) ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed);
		Debug.Log($"Default Resolution: {width}, {num}, {FullScreenMode.FullScreenWindow}");
		SetResolution(width, num, fullscreenMode);
	}

	public static bool CheckForWindowDanceChanges()
	{
		if (scnBase.isMac || scnBase.isWindows)
		{
			WindowChoreographer.playerWindow.Show(true);
		}
		if (!Persistence.GetHasWindowDanceChangedResolution())
		{
			return false;
		}
		FullScreenMode fullScreenMode = (Persistence.GetIsFullscreen() ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed);
		int resolutionWidth = Persistence.GetResolutionWidth();
		int resolutionHeight = Persistence.GetResolutionHeight();
		Debug.Log($"Check WindowDanceResolution: {resolutionWidth}, {resolutionHeight}, {fullScreenMode}");
		SetResolution(resolutionWidth, resolutionHeight, fullScreenMode);
		Persistence.SetHasWindowDanceChangedResolution(state: false);
		Sequence s = (Sequence)DOTween.Sequence().SetUpdate(isIndependentUpdate: true);
		s.AppendInterval(RealWindowChoreographer.WindowedSwitchTime);
		s.AppendCallback(() =>
		{
			if (fullScreenMode == FullScreenMode.Windowed)
			{
				Vector2Int position = new Vector2Int(Persistence.GetWindowXPosition(), Persistence.GetWindowYPosition());
				Debug.Log("Restoring position to: " + position);
				UnityPlayerWindow playerWindow = WindowChoreographer.playerWindow;
				Vector2Int windowDanceResolution = PlatformHelper.instance.WindowDanceResolution;
				if (position.x > 0 && position.y > 0 && position.x + resolutionWidth < windowDanceResolution.x && position.y + resolutionHeight < windowDanceResolution.y)
				{
					playerWindow.SetPosition(position);
				}
				else
				{
					playerWindow.ResetPosition(null);
				}
			}
			WindowChoreographer.playerWindow?.SetTitle("Rhythm Doctor");
		});
		return true;
	}

	public static void SetResolution(int width, int height, FullScreenMode fullscreenMode)
	{
		Debug.Log($"Changing resolution from: {Screen.currentResolution}, {Screen.fullScreenMode} to: {width} x {height}, {fullscreenMode}\n{StackTraceUtility.ExtractStackTrace()}");
		Screen.SetResolution(width, height, fullscreenMode);
	}

	public static void FlushCustomLevelData()
	{
		Singleton<AudioManager>.Instance.FlushData();
		try
		{
			foreach (KeyValuePair<string, CachedTexture> item in textureCache)
			{
				UnityEngine.Object.Destroy(textureCache[item.Key].texture);
			}
		}
		catch (Exception arg)
		{
			Debug.Log($"Error releasing textures cache: {arg}");
		}
		try
		{
			foreach (KeyValuePair<string, CachedFont> item2 in fontCache)
			{
				UnityEngine.Object.Destroy(fontCache[item2.Key].fontAsset);
			}
		}
		catch (Exception arg2)
		{
			Debug.Log($"Error releasing font cache: {arg2}");
		}
		textureCache.Clear();
		fontCache.Clear();
		Resources.UnloadUnusedAssets();
	}
}
