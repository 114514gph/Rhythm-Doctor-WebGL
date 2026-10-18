using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class scnSleevePaint : scnBase
{
	private enum ToolPreset
	{
		Brush = 0,
		Eraser = 1,
		Hand = 2,
		Sleeve = 3,
		Nails = 4
	}

	private const int zoomFactor = 3;

	private const int viewAreaMinX = 0;

	[Header("Variables")]
	public AnimationData eightBitsAnimation;

	[Header("Objects")]
	public Canvas canvas;

	public RectTransform rtCanvas;

	public SleevePaintCrosshair crosshair;

	public Image mainFader;

	public CameraFilterPack_FX_8bits eightBitsEffect;

	public Tween eightBitsEffectTransition;

	public Color unselectedColor;

	public Color selectedColor;

	[NonSerialized]
	public RDPlayer currentPlayer;

	private ToolPreset toolPreset;

	private int brushSize;

	private int brushColor;

	private int skinColor;

	private int sleeveColor;

	private int nailsColor;

	private int handDetail;

	private float palmLightness;

	[Header("Splash Screen")]
	public GameObject splash;

	public SleevePreview loadPlayer1Preview;

	public SleevePreview loadPlayer2Preview;

	public Button loadPlayer1Button;

	public Button loadPlayer2Button;

	public RectTransform splashContainer;

	public Image fader;

	[Header("Localizables")]
	public Text clearLabel;

	public Text toolLabel;

	public Text sizeButtonsLabel;

	public Text colorButtonsLabel;

	public Text handDetailButtonsLabel;

	public Text lightnessLabel;

	public Text undoLabel;

	public Text title;

	[Header("Preview")]
	public RectTransform preview;

	public RectTransform previewSheetRect;

	public SleevePreview sleevePreview;

	public RectTransform viewArea;

	public Image highlight;

	public Image buttonLeft;

	public Image buttonRight;

	[Header("Buttons")]
	public RectTransform[] allButtonContainers;

	public GameObject sizeButtonsContainer;

	public GameObject colorButtonsContainer;

	public GameObject handDetailButtonsContainer;

	public GameObject lightnessSliderContainer;

	public RectTransform colorButtonsPivot;

	public Button[] toolButtons;

	public Button[] sizeButtons;

	public Button[] colorButtons;

	public Button[] handDetailButtons;

	public Slider lightnessSlider;

	public Button undoRedo;

	public Button clearAll;

	public SpriteAnimation clearAllAnimation;

	public Button discard;

	public Button save;

	[Header("Main Drawing")]
	public RectTransform drawingArea;

	public RawImage sheet_cursor;

	public RawImage hand;

	public RawImage nails;

	public RawImage palm;

	public RectTransform viewableArea;

	public RectTransform sleeve;

	public Image sleeveImage;

	public AudioSource song;

	[Header("Debug")]
	public Text debugText;

	public InputField brushColorInputField;

	public InputField sleeveColorInputField;

	public float speed = 60f;

	private ArmSkin armSkinP1;

	private ArmSkin armSkinP2;

	public Texture2D texture;

	private Texture2D texture_cursor;

	private Color[] undo;

	private Color[] tempUndo;

	private Color[] clearColors;

	private AudioSource desktopAmbience;

	private float colorButtonStartHeight = -15f;

	private float colorButtonRaisedHeight = -12f;

	private RectTransform lightnessSliderDraggableArea;

	private float widthRatio;

	private float heightRatio;

	private int[] brushOffsetX;

	private int[] brushOffsetY;

	private Vector3 lastMousePosition = Vector3.zero;

	private float horizontalDirection;

	private bool inputBlocked = true;

	private bool drawingMode;

	private bool draggingPreview;

	private bool draggingLightnessSlider;

	private string buttonSound = "sndMenuPop";

	private bool fileLoaded;

	private RDCheatCode.CheatCode transCheat;

	private static readonly KeyCode[] TransCheatCode = new KeyCode[11]
	{
		KeyCode.T,
		KeyCode.R,
		KeyCode.A,
		KeyCode.N,
		KeyCode.S,
		KeyCode.R,
		KeyCode.I,
		KeyCode.G,
		KeyCode.H,
		KeyCode.T,
		KeyCode.S
	};

	private RDCheatCode.CheatCode nbCheat;

	private static readonly KeyCode[] NbCheatCode = new KeyCode[8]
	{
		KeyCode.N,
		KeyCode.B,
		KeyCode.R,
		KeyCode.I,
		KeyCode.G,
		KeyCode.H,
		KeyCode.T,
		KeyCode.S
	};

	private Color[] brushColors => base.gc.brushColors;

	private Color[] sleeveColors => base.gc.sleeveColors;

	private Color[] skinColors => base.gc.skinColors;

	private Color[] nailColors => base.gc.nailColors;

	private float direction
	{
		get
		{
			if (currentPlayer != RDPlayer.P2)
			{
				return 1f;
			}
			return -1f;
		}
	}

	private new void Awake()
	{
		base.Awake();
		muteWhenGameUnfocused = true;
		int num = 524;
		int num2 = 40;
		clearColors = new Color[num * num2];
		for (int i = 0; i < clearColors.Length; i++)
		{
			clearColors[i] = Color.clear;
		}
		lightnessSliderDraggableArea = lightnessSlider.handleRect.parent.GetComponent<RectTransform>();
	}

	private new void Start()
	{
		base.Start();
		base.conductor.SetBPM(120f);
		desktopAmbience = base.conductor.PlaySong("sndDesktopAmbienceStartup", RDUtils.GetMixerGroup("AmbienceParent"), 1f, 0f, useBeatsNotBar: true);
		armSkinP1 = Persistence.skins[Persistence.currentSlotIndex, 0];
		armSkinP2 = Persistence.skins[Persistence.currentSlotIndex, 1];
		Load(armSkinP1);
		Load(armSkinP2);
		loadPlayer1Preview.SetArmSkin(armSkinP1);
		loadPlayer2Preview.SetArmSkin(armSkinP2);
		Time.timeScale = 1f;
		printe("adding listeners");
		loadPlayer1Button.onClick.AddListener(() =>
		{
			Show(RDPlayer.P1);
		});
		loadPlayer2Button.onClick.AddListener(() =>
		{
			Show(RDPlayer.P2);
		});
		mainFader.enabled = true;
		mainFader.color = Color.black;
		mainFader.DOFade(0f, 1f).SetEase(Ease.InQuad).OnComplete(() =>
		{
			inputBlocked = false;
			mainFader.enabled = false;
		});
		transCheat = new RDCheatCode.CheatCode(TransCheatCode);
		nbCheat = new RDCheatCode.CheatCode(NbCheatCode);
		Narration.Say(RDString.GetJoined("narration.shortPause", "sleevePaint.titleSplash", "sleevePaint.splashInstructions"), NarrationCategory.Notification, false, NarrationActionName.ToExit);
	}

	private void Show(RDPlayer player)
	{
		printe("showshow " + player);
		StartCoroutine(ShowCo(player));
	}

	private IEnumerator ShowCo(RDPlayer player)
	{
		splashContainer.DOScale(Vector3.one * 1.4f, 0.8f).SetEase(Ease.OutQuad);
		fader.enabled = true;
		fader.color = Color.clear;
		fader.DOFade(1f, 0.8f).SetEase(Ease.OutQuad);
		yield return new WaitForSeconds(0.6f);
		EightBitsEffect(show: true);
		yield return new WaitForSeconds(0.5f);
		song.Play();
		crosshair.gameObject.SetActive(value: true);
		currentPlayer = player;
		splash.SetActive(value: false);
		sleeve.localScale = sleeve.localScale.WithX(direction);
		preview.localScale = preview.localScale.WithX(direction);
		string text = ((player == RDPlayer.P2) ? "P2" : "P1");
		title.text = RDString.Get("sleevePaint.title" + text);
		Color color = Color.white.WithAlpha(0.9f);
		sheet_cursor.color = color;
		int num = 262;
		int num2 = 524;
		int num3 = 40;
		previewSheetRect.sizeDelta = previewSheetRect.sizeDelta.WithX(num);
		highlight.color = new Color(1f, 1f, 1f, 0.12f);
		texture_cursor = new Texture2D(num2, num3);
		texture_cursor.filterMode = FilterMode.Point;
		undo = new Color[num2 * num3];
		for (int i = 0; i < clearColors.Length; i++)
		{
			undo[i] = Color.clear;
		}
		ClearCursorTexture();
		ClearAll();
		Vector2 sizeDelta = sleeve.sizeDelta;
		widthRatio = sizeDelta.x / (float)num2;
		heightRatio = sizeDelta.y / (float)num3;
		printef("widthRatio {0} heightRatio {1}", widthRatio, heightRatio);
		viewArea.sizeDelta = viewArea.sizeDelta.WithX(drawingArea.rect.width / 3f);
		int num4 = 0;
		Button[] array = colorButtons;
		foreach (Button button in array)
		{
			int iCaptured = num4;
			button.onClick.AddListener(() =>
			{
				_ = button.GetComponent<Image>().color;
				bool flag = true;
				if (toolPreset == ToolPreset.Brush || toolPreset == ToolPreset.Eraser)
				{
					flag = brushColor != iCaptured;
					SetToolPreset(ToolPreset.Brush);
					brushColor = iCaptured;
				}
				else if (toolPreset == ToolPreset.Hand)
				{
					flag = skinColor != iCaptured;
					skinColor = iCaptured;
					UpdateSkinColor();
				}
				else if (toolPreset == ToolPreset.Sleeve)
				{
					flag = sleeveColor != iCaptured;
					sleeveColor = iCaptured;
					UpdateSleeveColor();
				}
				else if (toolPreset == ToolPreset.Nails)
				{
					flag = nailsColor != iCaptured;
					nailsColor = iCaptured;
					UpdateNailsColor();
				}
				SetSelectedColorBorder(iCaptured);
				if (flag)
				{
					PlaySound(buttonSound);
				}
			});
			num4++;
		}
		ArmSkin armSkin = ((player == RDPlayer.P1) ? armSkinP1 : armSkinP2);
		Load(armSkin);
		UpdateSleeveAndSheet();
		UpdateSkinColor();
		UpdateSleeveColor();
		UpdateNailsColor();
		SetSelectedColorBorder(brushColor);
		Material material = UnityEngine.Object.Instantiate(sleeveImage.material);
		printe("texture filtering: " + texture.filterMode);
		material.SetTexture("_Drawing", texture);
		material.SetColor("_TintColor", Color.white);
		sleeveImage.material = material;
		sheet_cursor.texture = texture_cursor;
		sleevePreview.SetTexture(texture);
		undoRedo.onClick.AddListener(() =>
		{
			Undo();
		});
		clearAll.onClick.AddListener(() =>
		{
			PlaySound(buttonSound);
			ClearAll();
		});
		toolButtons[0].onClick.AddListener(() =>
		{
			SetToolPreset(ToolPreset.Brush);
		});
		toolButtons[1].onClick.AddListener(() =>
		{
			SetToolPreset(ToolPreset.Eraser);
		});
		toolButtons[2].onClick.AddListener(() =>
		{
			SetToolPreset(ToolPreset.Hand);
			SetHandDetail(0, setToolPreset: false);
		});
		toolButtons[3].onClick.AddListener(() =>
		{
			SetToolPreset(ToolPreset.Sleeve);
		});
		sizeButtons[0].onClick.AddListener(() =>
		{
			SetBrushSize(0);
		});
		sizeButtons[1].onClick.AddListener(() =>
		{
			SetBrushSize(1);
		});
		sizeButtons[2].onClick.AddListener(() =>
		{
			SetBrushSize(2);
		});
		handDetailButtons[0].onClick.AddListener(() =>
		{
			SetHandDetail(0, setToolPreset: true);
		});
		handDetailButtons[1].onClick.AddListener(() =>
		{
			SetHandDetail(1, setToolPreset: true);
		});
		discard.onClick.AddListener(() =>
		{
			DiscardChanges();
		});
		save.onClick.AddListener(() =>
		{
			PlaySound(buttonSound);
			if (!inputBlocked)
			{
				Done();
			}
		});
		SetToolPreset(ToolPreset.Brush);
		SetBrushSize(0);
		SetHandDetail(0, setToolPreset: false);
		if (RDString.isCJK)
		{
			Text[] array2 = new Text[6] { clearLabel, toolLabel, sizeButtonsLabel, handDetailButtonsLabel, colorButtonsLabel, undoLabel };
			for (int j = 0; j < array2.Length; j++)
			{
				RectTransform component = array2[j].GetComponent<RectTransform>();
				component.anchoredPosition = component.anchoredPosition.WithY(8f);
			}
			RectTransform[] array3 = allButtonContainers;
			foreach (RectTransform obj in array3)
			{
				obj.anchoredPosition = obj.anchoredPosition.WithY(-29f);
			}
			colorButtonRaisedHeight = -10f;
			lightnessSlider.GetComponent<RectTransform>().AnchorPosY(-1f);
		}
		fileLoaded = true;
	}

	private void DiscardChanges()
	{
		PlaySound(buttonSound);
		if (!inputBlocked)
		{
			EightBitsEffect(show: false);
		}
	}

	private void EightBitsEffect(bool show, bool quit = true)
	{
		int num = (show ? (-1) : 0);
		int num2 = ((!show) ? (-1) : 0);
		float delay = (show ? 0.5f : 0f);
		eightBitsEffect.Brightness = num;
		eightBitsEffect.enabled = true;
		if (eightBitsEffectTransition != null)
		{
			eightBitsEffectTransition.Kill();
		}
		if (!show)
		{
			inputBlocked = true;
			song.DOFade(0f, eightBitsAnimation.duration);
			desktopAmbience.DOFade(0f, eightBitsAnimation.duration);
		}
		CameraFilterPack_FX_8bits.ChangeBrightness = num;
		eightBitsEffectTransition = DOTween.To(() => CameraFilterPack_FX_8bits.ChangeBrightness, (float x) =>
		{
			CameraFilterPack_FX_8bits.ChangeBrightness = x;
		}, num2, eightBitsAnimation.duration).SetEase(eightBitsAnimation.ease).SetDelay(delay)
			.OnComplete(() =>
			{
				if (show)
				{
					eightBitsEffect.enabled = false;
					inputBlocked = false;
				}
				else if (quit)
				{
					SwitchScene();
				}
			});
	}

	private void SwitchScene()
	{
		Cursor.visible = true;
		scnBase.GoToLevelSelect();
	}

	private void Load(ArmSkin armSkin)
	{
		armSkin.Load();
		int num = 524;
		int num2 = 40;
		if (armSkin.drawing != null)
		{
			Texture2D drawing = armSkin.drawing;
			if (drawing.width != num || drawing.height != num2)
			{
				RDBaseDllDummy.printem($"t.w {drawing.width}/{num}, h {drawing.height}/{num2}");
				int blockWidth = Math.Min(drawing.width, num);
				int blockHeight = Math.Min(drawing.height, num2);
				Color[] pixels = drawing.GetPixels(0, 0, blockWidth, blockHeight, 0);
				Int2 @int = ((drawing.width == 428 && drawing.height == 30) ? new Int2(6, 4) : new Int2(0, 0));
				Texture2D texture2D = new Texture2D(num, num2);
				texture2D.SetPixels(clearColors);
				texture2D.SetPixels(@int.x, @int.y, blockWidth, blockHeight, pixels);
				texture2D.Apply();
				texture = texture2D;
			}
			else
			{
				texture = new Texture2D(drawing.width, drawing.height, drawing.format, 0, linear: false);
				Graphics.CopyTexture(drawing, texture);
			}
			printe("filterMode is Point");
			texture.filterMode = FilterMode.Point;
		}
		else
		{
			printe("here");
			texture = new Texture2D(num, num2);
			texture.SetPixels(clearColors);
		}
		sleeveColor = GetClosestColor(armSkin.sleeveColor, sleeveColors);
		skinColor = GetClosestColor(armSkin.skinColor, skinColors);
		nailsColor = GetClosestColor(armSkin.nailsColor, nailColors);
		palmLightness = armSkin.palmLightness;
	}

	private float ColorDistance(Color ca, Color cb)
	{
		float num = Mathf.Abs(ca.r - cb.r);
		float num2 = Mathf.Abs(ca.g - cb.g);
		float num3 = Mathf.Abs(ca.b - cb.b);
		return num * num + num2 * num2 + num3 * num3;
	}

	public int GetClosestColor(Color temp, Color[] colorList)
	{
		float num = float.PositiveInfinity;
		int result = 0;
		int num2 = 0;
		foreach (Color cb in colorList)
		{
			float num3 = ColorDistance(temp, cb);
			if (num3 < num)
			{
				result = num2;
				num = num3;
			}
			num2++;
		}
		return result;
	}

	private void SetSelectedColorBorder(int index)
	{
		int num = 0;
		Button[] array = colorButtons;
		foreach (Button button in array)
		{
			Transform child = button.transform.GetChild(0);
			bool flag = num == index;
			child.gameObject.SetActive(flag);
			if (flag)
			{
				Color color = button.GetComponent<Image>().color;
				child.GetComponent<Image>().color = new Color(1f - color.r, 1f - color.g, 1f - color.b, 1f);
			}
			num++;
		}
	}

	private void SetIndicatorAroundRectTransform(RectTransform indicator, RectTransform target)
	{
		indicator.anchoredPosition = new Vector2(-4f + target.anchoredPosition.x, 4f + target.anchoredPosition.y);
		indicator.sizeDelta = new Vector2(target.sizeDelta.x + 8f, target.sizeDelta.y + 8f);
	}

	private void ClearCursorTexture()
	{
		texture_cursor.SetPixels(clearColors);
		texture_cursor.Apply();
	}

	private void SetToolPreset(ToolPreset preset)
	{
		ToolPreset toolPreset = ((this.toolPreset == ToolPreset.Nails) ? ToolPreset.Hand : this.toolPreset);
		ToolPreset toolPreset2 = ((preset == ToolPreset.Nails) ? ToolPreset.Hand : preset);
		toolButtons[(int)toolPreset].GetComponent<Image>().color = unselectedColor;
		toolButtons[(int)toolPreset2].GetComponent<Image>().color = selectedColor;
		if (toolPreset != toolPreset2)
		{
			PlaySound(buttonSound);
		}
		this.toolPreset = preset;
		Color[] colorList = null;
		bool flag = true;
		bool active = true;
		bool active2 = false;
		bool flag2 = false;
		int selectedColorBorder = 0;
		switch (preset)
		{
		case ToolPreset.Brush:
		case ToolPreset.Eraser:
			selectedColorBorder = brushColor;
			colorList = brushColors;
			colorButtonsLabel.text = RDString.Get("sleevePaint.brushColor");
			break;
		case ToolPreset.Hand:
			ClearCursorTexture();
			selectedColorBorder = skinColor;
			colorList = skinColors;
			active = false;
			active2 = true;
			flag2 = true;
			colorButtonsLabel.text = RDString.Get("sleevePaint.skinColor");
			break;
		case ToolPreset.Nails:
			ClearCursorTexture();
			selectedColorBorder = nailsColor;
			colorList = nailColors;
			active = false;
			active2 = true;
			colorButtonsLabel.text = RDString.Get("sleevePaint.nailsColor");
			break;
		case ToolPreset.Sleeve:
			ClearCursorTexture();
			selectedColorBorder = sleeveColor;
			colorList = sleeveColors;
			active = false;
			colorButtonsLabel.text = RDString.Get("sleevePaint.sleeveColor");
			break;
		}
		colorButtonsContainer.SetActive(flag);
		sizeButtonsContainer.SetActive(active);
		handDetailButtonsContainer.SetActive(active2);
		lightnessSliderContainer.SetActive(flag2);
		if (flag)
		{
			UpdateColorPalette(colorList);
			SetSelectedColorBorder(selectedColorBorder);
			lightnessSlider.value = palmLightness;
			colorButtonsPivot.AnchorPosY(flag2 ? colorButtonRaisedHeight : colorButtonStartHeight);
		}
	}

	private void UpdateColorPalette(Color[] colorList)
	{
		int num = 0;
		Button[] array = colorButtons;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].GetComponent<Image>().color = colorList[num].WithAlpha(1f);
			num++;
		}
	}

	private new void Update()
	{
		base.Update();
		if (RDInput.cancelPress && !inputBlocked)
		{
			if (fileLoaded)
			{
				DiscardChanges();
			}
			else
			{
				ExitFromSplash();
			}
			return;
		}
		EventSystem current = EventSystem.current;
		if (!current || !fileLoaded)
		{
			return;
		}
		_ = current.currentSelectedGameObject;
		PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
		{
			pointerId = -1
		};
		pointerEventData.position = crosshair.screenPosition;
		List<RaycastResult> list = new List<RaycastResult>();
		EventSystem.current.RaycastAll(pointerEventData, list);
		bool flag = false;
		foreach (RaycastResult item in list)
		{
			if (item.gameObject.name == "clear")
			{
				clearAllAnimation.stopOnLoopEnding = false;
				clearAllAnimation.playing = true;
				flag = true;
			}
		}
		if (!flag)
		{
			clearAllAnimation.stopOnLoopEnding = true;
		}
		if (brushColorInputField.isFocused)
		{
			brushColors[brushColor] = brushColorInputField.text.HexToColor();
			UpdateColorPalette(brushColors);
			printe("update color palette to color: " + brushColors[brushColor]);
		}
		if (sleeveColorInputField.isFocused)
		{
			sleeveColors[sleeveColor] = sleeveColorInputField.text.HexToColor();
			UpdateColorPalette(sleeveColors);
			printe("update sleeve palette to color: " + sleeveColors[sleeveColor]);
		}
		if (inputBlocked)
		{
			return;
		}
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			SetToolPreset(ToolPreset.Brush);
		}
		if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			SetToolPreset(ToolPreset.Eraser);
		}
		if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			SetToolPreset(ToolPreset.Hand);
			SetHandDetail(0, setToolPreset: false);
		}
		if (Input.GetKeyDown(KeyCode.Alpha4))
		{
			SetToolPreset(ToolPreset.Sleeve);
		}
		if (horizontalDirection != 0f)
		{
			float viewAreaX = viewArea.anchoredPosition.x + horizontalDirection * speed * Time.deltaTime;
			SetViewAreaX(viewAreaX);
		}
		CheckDragPreviewArea();
		CheckDragLightnessArea();
		float num = (float)Screen.width * 1f / (float)Screen.height;
		if (toolPreset == ToolPreset.Brush || toolPreset == ToolPreset.Eraser)
		{
			bool flag2 = (!Input.GetKeyDown(KeyCode.Z) && RDInput.anyPlayerPress) || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1);
			bool mouseButton = Input.GetMouseButton(1);
			bool flag3 = false;
			bool flag4 = mouseButton || toolPreset == ToolPreset.Eraser;
			Color color = (flag4 ? Color.clear : brushColors[brushColor]);
			Vector2 screenPosition = crosshair.screenPosition;
			screenPosition.x /= (float)Screen.width / (rtCanvas.sizeDelta.y * num);
			screenPosition.y /= (float)Screen.height / rtCanvas.sizeDelta.y;
			float num2 = (float)Mathf.FloorToInt(sleeve.anchoredPosition.x + 10f) + -1f;
			float num3 = (drawingArea.parent.GetComponent<RectTransform>().sizeDelta.y - drawingArea.sizeDelta.y) / 2f + drawingArea.anchoredPosition.y;
			float num4 = (float)Mathf.FloorToInt(sleeve.anchoredPosition.y + num3) + -1f;
			int num5 = Mathf.FloorToInt(direction * (lastMousePosition.x - num2) / widthRatio);
			int num6 = Mathf.FloorToInt((lastMousePosition.y - num4) / heightRatio);
			int num7 = Mathf.FloorToInt(direction * (screenPosition.x - num2) / widthRatio);
			int num8 = Mathf.FloorToInt((screenPosition.y - num4) / heightRatio);
			if (RDInput.anyPlayerRelease || Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
			{
				drawingMode = false;
			}
			if (MousePositionIsInsideBounds(num7, num8, texture))
			{
				if (flag2)
				{
					drawingMode = true;
				}
				texture_cursor.SetPixels(clearColors);
				for (int i = 0; i < brushOffsetX.Length; i++)
				{
					int num9 = Mathf.RoundToInt(num7 + brushOffsetX[i]);
					int num10 = Mathf.RoundToInt(num8 + brushOffsetY[i]);
					if (MousePositionIsInsideBounds(num9, num10, texture))
					{
						Color color2 = (flag4 ? Color.white.WithAlpha(0.2f) : color);
						texture_cursor.SetPixel(num9, num10, color2);
					}
				}
				texture_cursor.Apply();
			}
			else
			{
				texture_cursor.SetPixels(clearColors);
				texture_cursor.Apply();
			}
			if (drawingMode)
			{
				if (flag2)
				{
					tempUndo = texture.GetPixels();
				}
				debugText.text = $"mousePosition {screenPosition.x}, {screenPosition.y}\n" + $"sheet {num2}, {num4}\n" + $"last {num5}, {num6}\n" + $"current {num7}, {num8}\n" + $"sleeve.anchoredPosition {sleeve.anchoredPosition.x}, {sleeve.anchoredPosition.y}\n" + $"drawingAreaOffsetX {10f}\n";
				int num11 = Math.Max(Math.Abs(num7 - num5), Math.Abs(num8 - num6));
				for (int j = 0; j <= num11; j++)
				{
					int num12 = 0;
					int num13 = 0;
					if (num11 == 0)
					{
						num12 = num7;
						num13 = num8;
					}
					else
					{
						float t = (float)j * 1f / (float)num11;
						num12 = Mathf.RoundToInt(Mathf.Lerp(num5, num7, t));
						num13 = Mathf.RoundToInt(Mathf.Lerp(num6, num8, t));
					}
					for (int k = 0; k < brushOffsetX.Length; k++)
					{
						int num14 = num12 + brushOffsetX[k];
						int num15 = num13 + brushOffsetY[k];
						if (MousePositionIsInsideBounds(num14, num15, texture))
						{
							flag3 = true;
							texture.SetPixel(num14, num15, color);
						}
					}
				}
				if (flag3)
				{
					undo = tempUndo;
				}
				texture.Apply();
			}
		}
		if (transCheat.CheckCheatCode())
		{
			SetToTexture(base.gc.transDrawing);
		}
		if (nbCheat.CheckCheatCode())
		{
			SetToTexture(base.gc.nbDrawing);
		}
		if (Input.GetKeyDown(KeyCode.Z))
		{
			Undo();
		}
		lastMousePosition = crosshair.screenPosition;
		lastMousePosition.x /= (float)Screen.width / (rtCanvas.sizeDelta.y * num);
		lastMousePosition.y /= (float)Screen.height / rtCanvas.sizeDelta.y;
	}

	private bool MousePositionIsInsideBounds(float x, float y, Texture2D texture)
	{
		if (x >= 0f && x < (float)texture.width && y >= 0f)
		{
			return y < (float)texture.height;
		}
		return false;
	}

	private void PlaySound(string sound)
	{
		if (fileLoaded)
		{
			scrConductor.PlayImmediatelyIgnoringTimeScale(sound, 0.1f, RDUtils.GetMixerGroup("InterfaceSoundsParent"));
		}
	}

	private void Undo()
	{
		PlaySound(buttonSound);
		Color[] pixels = texture.GetPixels();
		texture.SetPixels(undo);
		texture.Apply();
		undo = pixels;
	}

	private void ClearAll()
	{
		Color32[] pixels = texture.GetPixels32();
		for (int i = 0; i < pixels.Length; i++)
		{
			if (pixels[i].a != 0)
			{
				printe("clearing");
				tempUndo = texture.GetPixels();
				texture.SetPixels(clearColors);
				texture.Apply();
				return;
			}
		}
		printe("all pixels are clear");
	}

	private void SetBrushSize(int brushSize)
	{
		PlaySound(buttonSound);
		sizeButtons[this.brushSize].GetComponent<Image>().color = unselectedColor;
		sizeButtons[brushSize].GetComponent<Image>().color = selectedColor;
		this.brushSize = brushSize;
		int[] array = new int[4] { -1, -1, 0, 0 };
		int[] array2 = new int[4] { -1, 0, -1, 0 };
		int[] array3 = new int[24]
		{
			-3, -3, -2, -2, -2, -2, -1, -1, -1, -1,
			-1, -1, 0, 0, 0, 0, 0, 0, 1, 1,
			1, 1, 2, 2
		};
		int[] array4 = new int[24]
		{
			-1, 0, -2, -1, 0, 1, -3, -2, -1, 0,
			1, 2, -3, -2, -1, 0, 1, 2, -2, -1,
			0, 1, -1, 0
		};
		int[] array5 = new int[76]
		{
			-5, -5, -5, -5, -4, -4, -4, -4, -4, -4,
			-3, -3, -3, -3, -3, -3, -3, -3, -2, -2,
			-2, -2, -2, -2, -2, -2, -2, -2, -1, -1,
			-1, -1, -1, -1, -1, -1, -1, -1, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 1, 1,
			1, 1, 1, 1, 1, 1, 1, 1, 2, 2,
			2, 2, 2, 2, 2, 2, 3, 3, 3, 3,
			3, 3, 4, 4, 4, 4
		};
		int[] array6 = new int[76]
		{
			-2, -1, 0, 1, -3, -2, -1, 0, 1, 2,
			-4, -3, -2, -1, 0, 1, 2, 3, -5, -4,
			-3, -2, -1, 0, 1, 2, 3, 4, -5, -4,
			-3, -2, -1, 0, 1, 2, 3, 4, -5, -4,
			-3, -2, -1, 0, 1, 2, 3, 4, -5, -4,
			-3, -2, -1, 0, 1, 2, 3, 4, -4, -3,
			-2, -1, 0, 1, 2, 3, -3, -2, -1, 0,
			1, 2, -2, -1, 0, 1
		};
		switch (brushSize)
		{
		case 0:
			brushOffsetX = array;
			brushOffsetY = array2;
			break;
		case 1:
			brushOffsetX = array3;
			brushOffsetY = array4;
			break;
		case 2:
			brushOffsetX = array5;
			brushOffsetY = array6;
			break;
		}
	}

	private void SetHandDetail(int detail, bool setToolPreset)
	{
		handDetailButtons[handDetail].GetComponent<Image>().color = unselectedColor;
		handDetailButtons[detail].GetComponent<Image>().color = selectedColor;
		handDetail = detail;
		if (setToolPreset)
		{
			PlaySound(buttonSound);
			switch (detail)
			{
			case 0:
				SetToolPreset(ToolPreset.Hand);
				break;
			case 1:
				SetToolPreset(ToolPreset.Nails);
				break;
			}
		}
	}

	public void CheckDragLightnessArea()
	{
		bool flag = RectTransformUtility.ScreenPointToLocalPointInRectangle(lightnessSliderDraggableArea, crosshair.screenPosition, crosshair.currentCamera, out var localPoint);
		bool flag2 = lightnessSliderDraggableArea.rect.Contains(localPoint) && lightnessSliderContainer.activeSelf;
		if ((draggingLightnessSlider | flag2) && (RDInput.anyPlayerIsPressed || Input.GetMouseButton(0) || Input.GetMouseButton(1)))
		{
			draggingLightnessSlider = true;
			Vector2 obj = (flag ? localPoint : Vector2.negativeInfinity);
			float lightnessSliderValue = obj.x;
			SetLightnessSliderValue(lightnessSliderValue);
		}
		else if (RDInput.anyPlayerRelease || Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
		{
			draggingLightnessSlider = false;
		}
	}

	private void SetLightnessSliderValue(float x)
	{
		float num = lightnessSliderDraggableArea.rect.size.x;
		float num2 = num / 2f;
		float value = Mathf.Clamp((x + num2) / num, 0f, 1f);
		lightnessSlider.value = value;
		palmLightness = value;
		UpdateSkinColor();
	}

	public void CheckDragPreviewArea()
	{
		bool flag = RectTransformUtility.ScreenPointToLocalPointInRectangle(viewableArea, crosshair.screenPosition, crosshair.currentCamera, out var localPoint);
		bool flag2 = viewableArea.rect.Contains(localPoint);
		highlight.gameObject.SetActive(flag2);
		if ((draggingPreview | flag2) && (RDInput.anyPlayerIsPressed || Input.GetMouseButton(0) || Input.GetMouseButton(1)))
		{
			draggingPreview = true;
			Vector2 obj = (flag ? localPoint : Vector2.negativeInfinity);
			float viewAreaX = obj.x - viewArea.sizeDelta.x / 2f;
			SetViewAreaX(viewAreaX);
		}
		else if (RDInput.anyPlayerRelease || Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
		{
			draggingPreview = false;
		}
	}

	private void SetViewAreaX(float x)
	{
		viewArea.anchoredPosition = new Vector2(Mathf.Clamp(x, 2f, viewableArea.sizeDelta.x - viewArea.sizeDelta.x), viewArea.anchoredPosition.y);
		UpdateSleeveAndSheet();
	}

	private void UpdateSleeveAndSheet()
	{
		float num = viewArea.anchoredPosition.x;
		sleeve.anchoredPosition = new Vector2(153f - sleeve.localScale.x * (num * 3f), sleeve.anchoredPosition.y);
	}

	private void UpdateSleeveColor()
	{
		Color color = sleeveColors[sleeveColor];
		sleeveImage.color = color;
		sleevePreview.SetSleeveColor(color);
	}

	private void UpdateSkinColor()
	{
		Color color = skinColors[skinColor];
		hand.color = color;
		sleevePreview.SetHandColor(color);
		Color color2 = color.WithLightnessBoost(palmLightness);
		palm.color = color2;
		sleevePreview.SetPalmColor(color2);
	}

	private void UpdateNailsColor()
	{
		Color color = nailColors[nailsColor];
		nails.color = color;
		sleevePreview.SetNailsColor(color);
	}

	public void PanLeftDown()
	{
		horizontalDirection = -1f * direction;
		buttonLeft.color = selectedColor;
	}

	public void PanLeftUp()
	{
		horizontalDirection = 0f;
		buttonLeft.color = unselectedColor;
	}

	public void PanRightDown()
	{
		horizontalDirection = 1f * direction;
		buttonRight.color = selectedColor;
	}

	public void PanRightUp()
	{
		horizontalDirection = 0f;
		buttonRight.color = unselectedColor;
	}

	public void Done()
	{
		((currentPlayer == RDPlayer.P1) ? armSkinP1 : armSkinP2).Save(texture, sleeveColors[sleeveColor], skinColors[skinColor], nailColors[nailsColor], palmLightness);
		EightBitsEffect(show: false);
	}

	public void ExitFromSplash()
	{
		inputBlocked = true;
		EventSystem.current.enabled = false;
		if (!mainFader.enabled)
		{
			mainFader.enabled = true;
			mainFader.color = Color.clear;
		}
		else
		{
			mainFader.DOKill();
		}
		desktopAmbience.DOFade(0f, 1f);
		mainFader.DOFade(1f, 1f).SetEase(Ease.OutExpo).OnComplete(() =>
		{
			SwitchScene();
		});
	}

	private void SetToTexture(Texture2D newTex)
	{
		Debug.Log(1);
		tempUndo = texture.GetPixels();
		texture.SetPixels(newTex.GetPixels());
		undo = tempUndo;
		texture.Apply();
	}
}
