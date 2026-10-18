using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DG.Tweening;
using RDLevelEditor;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LyricsGame : RDBase
{
	private const string TimesStartTag = "-Times-";

	private const string ln = "\n";

	private const char LyricsSeparator = '/';

	private const char LyricsSeparatorCJK = '／';

	private const char EscapeChar = '\\';

	private const string lnWindows = "\r\n";

	private const double syncLeniency = 0.5;

	private const double skipLeniency = 0.5;

	[NonSerialized]
	public float offset;

	[NonSerialized]
	public bool manualAdvance;

	[NonSerialized]
	public string tempRecordTimes = "-Times-";

	[NonSerialized]
	public float fadeTime;

	[NonSerialized]
	public FloatingTextMode floatingTextMode = FloatingTextMode.FadeOut;

	[NonSerialized]
	public Color baseColor;

	[NonSerialized]
	public Color baseOutlineColor = Color.black;

	[NonSerialized]
	public bool dontDestroyOnComplete;

	[NonSerialized]
	public bool autoNarration;

	[NonSerialized]
	public List<LyricsGame> syncedLyrics = new List<LyricsGame>();

	[NonSerialized]
	public string fullText;

	[NonSerialized]
	public string currentFont = "";

	[NonSerialized]
	public Tween positionTweenX;

	[NonSerialized]
	public Tween positionTweenY;

	[NonSerialized]
	public Tween scaleTweenX;

	[NonSerialized]
	public Tween scaleTweenY;

	[NonSerialized]
	public Tween angleTween;

	[NonSerialized]
	public Tween tintTween;

	[NonSerialized]
	public Tween borderTween;

	public Canvas canvas;

	private float storedAngle;

	private VerticalAlignmentOptions verticalAlignment = VerticalAlignmentOptions.Middle;

	private float storedWidth = 0.25f;

	[SerializeField]
	private Text textDisplay;

	[SerializeField]
	private TextMeshProUGUI TMPTextDisplay;

	[SerializeField]
	private RectTransform TMPOffsetRect;

	[SerializeField]
	private CanvasGroup canvasGroup;

	[SerializeField]
	private FourSidedOutline outline;

	private int snapshotIndex = -1;

	private List<string> textSnapshots = new List<string>();

	private List<double> timeSnapshots = new List<double>();

	private List<string> narrationText = new List<string>();

	private List<int> narrationTimeIndex = new List<int>();

	private int lastLineIndexNarrated = -1;

	private Tween delayedInstantFade;

	private Tween outlineFadeTween;

	private Tween fillFadeTween;

	public string text
	{
		get
		{
			return textDisplay.text;
		}
		set
		{
			textDisplay.text = value;
			TMPTextDisplay.text = ColorTagConverter(value);
			ManualTMProVerticalAlignment();
		}
	}

	public Vector3 anchoredPosition3D
	{
		get
		{
			return textDisplay.rectTransform.anchoredPosition3D;
		}
		set
		{
			textDisplay.rectTransform.anchoredPosition3D = value;
			TMPOffsetRect.anchoredPosition3D = value;
		}
	}

	public Vector2 anchoredPosition
	{
		get
		{
			return textDisplay.rectTransform.anchoredPosition;
		}
		set
		{
			textDisplay.rectTransform.anchoredPosition = value;
			TMPOffsetRect.anchoredPosition = value;
		}
	}

	public Vector2 anchorMin
	{
		set
		{
			textDisplay.rectTransform.anchorMin = value;
			TMPOffsetRect.anchorMin = value;
		}
	}

	public Vector2 anchorMax
	{
		set
		{
			textDisplay.rectTransform.anchorMax = value;
			TMPOffsetRect.anchorMax = value;
		}
	}

	public Vector2 scale
	{
		get
		{
			return new Vector2(textDisplay.rectTransform.localScale.x, textDisplay.rectTransform.localScale.y);
		}
		set
		{
			textDisplay.rectTransform.localScale = new Vector3(value.x, value.y, 1f);
			TMPOffsetRect.localScale = new Vector3(value.x, value.y, 1f);
		}
	}

	public float angle
	{
		get
		{
			return storedAngle;
		}
		set
		{
			textDisplay.rectTransform.localEulerAngles = Vector3.forward * value;
			TMPOffsetRect.localEulerAngles = Vector3.forward * value;
			storedAngle = value;
		}
	}

	public Vector2 pivot
	{
		get
		{
			return textDisplay.rectTransform.pivot;
		}
		set
		{
			textDisplay.rectTransform.pivot = value;
			TMPOffsetRect.pivot = new Vector2(value.x, 0.5f);
		}
	}

	public Color color
	{
		get
		{
			return textDisplay.color.WithAlpha(textDisplay.material.color.a);
		}
		set
		{
			textDisplay.material.color = Color.white.WithAlpha(value.a);
			TMPTextDisplay.fontMaterial?.SetColor("_FaceColor", Color.white.WithAlpha(value.a));
			textDisplay.color = value.WithAlpha(1f);
			TMPTextDisplay.color = value.WithAlpha(1f);
		}
	}

	public Color outlineColor
	{
		get
		{
			return outline.effectColor;
		}
		set
		{
			outline.effectColor = value;
			TMPTextDisplay.fontMaterial?.SetColor("_OutlineColor", value);
		}
	}

	public TextAnchor alignment
	{
		set
		{
			textDisplay.alignment = value;
			TMPTextDisplay.alignment = ConvertAlignment(value);
		}
	}

	public int fontSize
	{
		get
		{
			return textDisplay.fontSize;
		}
		set
		{
			textDisplay.fontSize = value;
			TMPTextDisplay.fontSize = value;
			SetUpOutline();
		}
	}

	public int layer
	{
		set
		{
			textDisplay.gameObject.layer = value;
			TMPTextDisplay.gameObject.layer = value;
		}
	}

	public Vector2 sizeDelta
	{
		get
		{
			return TMPTextDisplay.rectTransform.sizeDelta;
		}
		set
		{
			textDisplay.rectTransform.sizeDelta = new Vector2(value.x, RDBase.Vfx.RDWidth);
			TMPTextDisplay.rectTransform.sizeDelta = value;
			TMPOffsetRect.sizeDelta = value;
		}
	}

	public float characterSpacing
	{
		get
		{
			return TMPTextDisplay.characterSpacing;
		}
		set
		{
			TMPTextDisplay.characterSpacing = value;
		}
	}

	public float outlineWidth
	{
		get
		{
			return storedWidth;
		}
		set
		{
			TMPTextDisplay.fontMaterial?.EnableKeyword("OUTLINE_ON");
			TMPTextDisplay.fontMaterial?.SetFloat(ShaderUtilities.ID_OutlineWidth, value);
			TMPTextDisplay.fontMaterial?.SetFloat(ShaderUtilities.ID_FaceDilate, value);
			TMPTextDisplay.SetAllDirty();
			storedWidth = value;
			outlineColor = outlineColor;
		}
	}

	public bool bold
	{
		set
		{
			if (value)
			{
				TMPTextDisplay.fontStyle |= FontStyles.Bold;
			}
			else
			{
				TMPTextDisplay.fontStyle &= ~FontStyles.Bold;
			}
		}
	}

	public bool italic
	{
		set
		{
			if (value)
			{
				TMPTextDisplay.fontStyle |= FontStyles.Italic;
			}
			else
			{
				TMPTextDisplay.fontStyle &= ~FontStyles.Italic;
			}
		}
	}

	public bool underline
	{
		set
		{
			if (value)
			{
				TMPTextDisplay.fontStyle |= FontStyles.Underline;
			}
			else
			{
				TMPTextDisplay.fontStyle &= ~FontStyles.Underline;
			}
		}
	}

	private bool manualLyricAlpha => scnGame.instance?.currentLevel?.manualLyricAlpha == true;

	public bool canAcceptFontAssets => TMPTextDisplay != null;

	private void Awake()
	{
		GetComponent<CanvasScaler>().dynamicPixelsPerUnit = GC.RoomRenderTextureScale;
		outline.useGraphicAlpha = false;
		text = "";
		if (textDisplay.material != null)
		{
			textDisplay.material = new Material(textDisplay.material);
		}
	}

	public LyricsGame Setup(string filename, bool multilineMode, int roomIndex = -1, GameObject parent = null, float offset = 0f)
	{
		fadeTime = (parent ? 1f : 3f);
		Setup(filename, multilineMode, offset);
		baseColor = color;
		dontDestroyOnComplete = true;
		if (roomIndex != -1)
		{
			SetupFollowCam(roomIndex);
			scnGame.instance.rooms[roomIndex].lyrics = this;
		}
		if ((bool)parent)
		{
			RectTransform component = GetComponent<RectTransform>();
			component.SetParent(parent.transform);
			component.anchoredPosition = new Vector3(0.5f, 0f, 1f);
		}
		return this;
	}

	public LyricsGame Setup(string text, string font, Vector2 position, int size, float angle, Color color, int roomIndex, int id, float fadeOutSeconds, FloatingTextMode floatingTextMode, Color outlineColor, TextAnchor alignment, bool followCam, bool hasTag)
	{
		manualAdvance = true;
		dontDestroyOnComplete = hasTag;
		SetLyricToFont(text, font);
		sizeDelta = new Vector2(RDBase.Vfx.RDWidth, RDBase.Vfx.RDWidth);
		SetLyrics(text, position, size, angle, color, roomIndex, fadeOutSeconds, floatingTextMode, outlineColor, alignment, followCam);
		RDBase.Vfx.allVfxLyrics.Add(id, this);
		return this;
	}

	public static LyricsGame Create(bool smallPrefab = false)
	{
		return (smallPrefab ? RDBase.Vfx.lyricsSmallPrefab : RDBase.Vfx.lyricsPrefab).Instantiate("Lyrics", makeContainer: false).GetComponent<LyricsGame>();
	}

	public LyricsGame Setup(string decorationId, string font, RDSortingLayer sortingLayer, int depth)
	{
		LoadLinesAndTimes("", null, multilineMode: false);
		manualAdvance = true;
		dontDestroyOnComplete = true;
		base.transform.SetParent(base.gm.parentContainer);
		sizeDelta = new Vector2(RDBase.Vfx.RDWidth, RDBase.Vfx.RDWidth);
		Vector2 vector = new Vector2(50f, 50f);
		anchoredPosition = new Vector2(Mathf.Floor(RDBase.Vfx.RDWidth * (vector.x / 100f - 0.5f)) - 0.5f, Mathf.Floor(RDBase.Vfx.RDHeight * (vector.y / 100f - 0.5f)) - 0.5f);
		SetRenderQueueForTextDecoration();
		canvas.sortingLayerName = sortingLayer.ToString();
		int sortingOrder = depth;
		if (sortingLayer == RDSortingLayer.Default)
		{
			sortingOrder = -depth;
		}
		canvas.sortingOrder = sortingOrder;
		SetLyricToFont("", font);
		RDBase.Vfx.allDecorationLyrics.Add(decorationId, this);
		return this;
	}

	public void Setup(string lyricsFileName, bool multilineMode, float offset = 0f)
	{
		this.offset = offset;
		string[] array = lyricsFileName.Split('_');
		string linesString = RDString.Get(((array[1] == "dog") ? "level.RhythmDogtor." : "level.Lesmis.") + array[^1]);
		string text = (Resources.Load("Lyrics/" + lyricsFileName) as TextAsset).text;
		text = text.Replace("\r\n", "\n");
		text = text.Replace("\n\n", "\n");
		base.gameObject.name = "Lyrics " + array[^1];
		LoadLinesAndTimes(linesString, text, multilineMode);
		if (scnGame.instance.currentLevel is Level_Lesmis)
		{
			SetFont(RDString.currentLanguageFont);
			textDisplay.rectTransform.offsetMin = new Vector2(textDisplay.rectTransform.offsetMin.x, 0f);
		}
	}

	public void SetUpOutline()
	{
		float num = (RDString.GetAppropiateFontPackForString(text).usePixelFont ? 8f : 12f);
		outline.effectDistance = new Vector2((float)fontSize / num, (float)fontSize / num);
	}

	public static string[] SplitLyricsLines(string line, bool useLinebreak = false)
	{
		char[] array = new char[2] { '/', '／' };
		char[] array2 = new char[3] { '/', '／', '\n' };
		string text = "</color>";
		string text2 = "X_A_X_A_X_A_X_A";
		string oldValue = "\\/";
		string text3 = "0+1+0+1+0+1+0+1";
		string oldValue2 = "\\／";
		string text4 = "Q=V=Q=V=Q=V=Q=V";
		line = line.Replace(text, text2);
		line = line.Replace(oldValue, text3);
		line = line.Replace(oldValue2, text4);
		char[] separator = (useLinebreak ? array2 : array);
		string[] array3 = line.Split(separator, StringSplitOptions.None);
		for (int i = 0; i < array3.Length; i++)
		{
			array3[i] = array3[i].Replace(text2, text).Replace(text3, '/'.ToString()).Replace(text4, '／'.ToString())
				.Replace("\\n", "\n");
		}
		return array3;
	}

	public void SetLyricToFont(string text, string font)
	{
		int num = fontSize;
		currentFont = font;
		FontPack appropiateFontPackForString = RDString.GetAppropiateFontPackForString(text);
		if (Enum.TryParse<TextFont>(font, out var result))
		{
			SetLyricToBaseFont(appropiateFontPackForString, result);
		}
		else
		{
			TMP_FontAsset customFontWithName = RDEditorUtils.GetCustomFontWithName(font);
			if (customFontWithName != null && canAcceptFontAssets)
			{
				SetFont(customFontWithName);
			}
			else
			{
				currentFont = TextFont.Default.ToString();
				SetLyricToBaseFont(appropiateFontPackForString, TextFont.Default);
			}
		}
		fontSize = num;
	}

	private void SetLyricToBaseFont(FontPack fontPack, TextFont fontEnum)
	{
		int num = fontEnum switch
		{
			TextFont.Default => base.game.currentLevel.useFlashFontForFloatingText ? 1 : 0, 
			TextFont.Flash => 1, 
			_ => 0, 
		};
		bool flag = fontEnum switch
		{
			TextFont.Default => !fontPack.usePixelFont, 
			TextFont.Vector => true, 
			_ => false, 
		};
		if (num != 0)
		{
			SetFont(fontPack.flashFontAsset);
			if (fontPack.language == SystemLanguage.Japanese)
			{
				bold = true;
			}
		}
		else if (flag)
		{
			SetFont(fontPack.vectorFontAsset);
			SetUnderlay(state: false);
		}
		else
		{
			SetFont(fontPack.pixelFont.font);
		}
		if (flag)
		{
			fontSize *= 4;
			GetComponent<CanvasScaler>().dynamicPixelsPerUnit = 4f;
		}
	}

	public void SetLyrics(string text, Vector2 position, int size, float angle, Color color, int roomIndex, float fadeOutSeconds, FloatingTextMode floatingTextMode, Color outlineColor, TextAnchor alignment, bool followCam)
	{
		if (RDString.samuraiMode)
		{
			string[] array = text.Split('\n');
			int num = array.Length;
			string text2 = "";
			for (int i = 0; i < num; i++)
			{
				text2 += (string.IsNullOrWhiteSpace(array[i]) ? array[i] : "Samurai.");
				if (i < num - 1)
				{
					text2 += "\n";
				}
			}
			printe($"text: {text}\nlinesCount: {num}\nfinalText: {text2}");
			text = text2;
		}
		LoadLinesAndTimes(text, null, multilineMode: false);
		baseColor = color;
		fadeTime = fadeOutSeconds;
		this.floatingTextMode = floatingTextMode;
		SetRoom(followCam, roomIndex);
		anchorMin = new Vector2(0.5f, 0.5f);
		anchorMax = new Vector2(0.5f, 0.5f);
		pivot = alignment switch
		{
			TextAnchor.UpperLeft => new Vector2(0f, 1f), 
			TextAnchor.UpperCenter => new Vector2(0.5f, 1f), 
			TextAnchor.UpperRight => new Vector2(1f, 1f), 
			TextAnchor.MiddleLeft => new Vector2(0f, 0.5f), 
			TextAnchor.MiddleCenter => new Vector2(0.5f, 0.5f), 
			TextAnchor.MiddleRight => new Vector2(1f, 0.5f), 
			TextAnchor.LowerLeft => new Vector2(0f, 0f), 
			TextAnchor.LowerCenter => new Vector2(0.5f, 0f), 
			TextAnchor.LowerRight => new Vector2(1f, 0f), 
			_ => new Vector2(0.5f, 0.5f), 
		};
		this.alignment = alignment;
		LevelBase currentLevel = base.game.currentLevel;
		currentLevel.floatingTextsCreatedCount = (currentLevel.floatingTextsCreatedCount + 1) % 1000;
		float z = 10f - (float)currentLevel.floatingTextsCreatedCount * 0.001f;
		anchoredPosition3D = new Vector3(position.x, position.y, z);
		this.angle = angle;
		this.color = color;
		SetLyricToFont(text, currentFont);
		fontSize = size;
		baseOutlineColor = outlineColor;
		if (roomIndex == -1)
		{
			base.gameObject.layer = GC.GameHUDLayer;
			layer = GC.GameHUDLayer;
		}
	}

	public void LoadLinesAndTimes(string linesString, string timesString, bool multilineMode)
	{
		string[] array = null;
		fullText = linesString;
		if (multilineMode)
		{
			linesString = linesString.Replace("\n", "\n/");
			linesString = linesString.Replace("[linebreak]\n/", "\n");
			linesString = linesString.Replace("[linebreak]", "\n");
			array = new string[1] { linesString };
		}
		else
		{
			array = linesString.Split(new string[1] { "\n" }, StringSplitOptions.None);
		}
		if (array.Length == 0)
		{
			return;
		}
		int num = 0;
		string[] array2 = array;
		foreach (string line in array2)
		{
			string text = "";
			string text2 = "";
			string[] array3 = SplitLyricsLines(line);
			narrationTimeIndex.Add(num);
			string[] array4 = array3;
			foreach (string text3 in array4)
			{
				text += text3;
				text2 += text3;
				textSnapshots.Add(text);
				num++;
				if (multilineMode && text3.EndsWith("\n"))
				{
					narrationTimeIndex.Add(num);
					narrationText.Add(text2);
					text2 = "";
				}
			}
			if (!multilineMode)
			{
				narrationText.Add(text);
			}
		}
		if (timesString == null)
		{
			return;
		}
		array2 = timesString.Split(new string[1] { "\n" }, StringSplitOptions.None);
		foreach (string text4 in array2)
		{
			if (text4.Length > 0)
			{
				timeSnapshots.Add(double.Parse(text4));
			}
		}
	}

	private string ColorTagConverter(string textToConvert)
	{
		Regex regex = new Regex("(<color=\"#)([A-Fa-f0-9]{3}|[A-Fa-f0-9]{4}|[A-Fa-f0-9]{6}|[A-Fa-f0-9]{8})(\">)");
		Match match = regex.Match(textToConvert);
		while (match.Success)
		{
			textToConvert = textToConvert.Remove(match.Index + match.Length - 2, 1);
			textToConvert = textToConvert.Remove(match.Index + 7, 1);
			match = regex.Match(textToConvert);
		}
		textToConvert = ReplaceNamedColorTag(textToConvert, "aqua", "#00ffffff");
		textToConvert = ReplaceNamedColorTag(textToConvert, "brown", "#a52a2aff");
		textToConvert = ReplaceNamedColorTag(textToConvert, "cyan", "#00ffffff");
		textToConvert = ReplaceNamedColorTag(textToConvert, "darkblue", "#0000a0ff");
		textToConvert = ReplaceNamedColorTag(textToConvert, "fuchsia", "#ff00ffff");
		textToConvert = ReplaceNamedColorTag(textToConvert, "grey", "#808080ff");
		textToConvert = ReplaceNamedColorTag(textToConvert, "lightblue", "#add8e6ff");
		textToConvert = ReplaceNamedColorTag(textToConvert, "lime", "#00ff00ff");
		textToConvert = ReplaceNamedColorTag(textToConvert, "magenta", "#ff00ffff");
		textToConvert = ReplaceNamedColorTag(textToConvert, "maroon", "#800000ff");
		textToConvert = ReplaceNamedColorTag(textToConvert, "navy", "#000080ff");
		textToConvert = ReplaceNamedColorTag(textToConvert, "olive", "#808000ff");
		textToConvert = ReplaceNamedColorTag(textToConvert, "silver", "#c0c0c0ff");
		textToConvert = ReplaceNamedColorTag(textToConvert, "teal", "#008080ff");
		return textToConvert;
	}

	private string ReplaceNamedColorTag(string textToConvert, string colorName, string hexcode)
	{
		textToConvert = textToConvert.Replace(ColorTagWithoutQuotations(colorName), ColorTagWithoutQuotations(hexcode));
		textToConvert = textToConvert.Replace(ColorTagWithQuotations(colorName), ColorTagWithoutQuotations(hexcode));
		return textToConvert;
	}

	private string ColorTagWithoutQuotations(string colorName)
	{
		return "<color=" + colorName + ">";
	}

	private string ColorTagWithQuotations(string colorName)
	{
		return "<color=\"" + colorName + "\">";
	}

	public void SetupFollowCam(int roomIndex)
	{
		base.transform.SetParent(RDBase.Vfx.GetCamera(roomIndex).transform);
		base.transform.localPosition = Vector3.zero.WithZ(1f).WithX(0.5f);
	}

	private void Update()
	{
		ManualTMProVerticalAlignment();
		if (!manualAdvance)
		{
			while (snapshotIndex < timeSnapshots.Count - 1 && timeSnapshots[snapshotIndex + 1] < (double)(base.conductor.songFileTimeVisual * RDTime.speed + offset))
			{
				AdvanceText();
			}
		}
	}

	private void ManualTMProVerticalAlignment()
	{
		if (TMPTextDisplay.text.Length > 0)
		{
			float num = TMPTextDisplay.GetPreferredValues(TMPTextDisplay.text[0].ToString(), TMPTextDisplay.rectTransform.sizeDelta.x, TMPTextDisplay.rectTransform.sizeDelta.y).y;
			float num2 = TMPTextDisplay.GetPreferredValues(TMPTextDisplay.text, TMPTextDisplay.rectTransform.sizeDelta.x, TMPTextDisplay.rectTransform.sizeDelta.y).y;
			TMPOffsetRect.sizeDelta = TMPOffsetRect.sizeDelta.WithY(num2);
			TMPTextDisplay.rectTransform.sizeDelta = TMPOffsetRect.sizeDelta.WithY(num2);
			float num3 = 0f - num / 4f;
			switch (verticalAlignment)
			{
			case VerticalAlignmentOptions.Top:
				num3 += 0f - num / 2f;
				break;
			case VerticalAlignmentOptions.Middle:
				num3 += (num2 - num) / 2f;
				break;
			case VerticalAlignmentOptions.Bottom:
				num3 += num2 - num + num / 2f;
				break;
			}
			TMPTextDisplay.rectTransform.localPosition = TMPTextDisplay.rectTransform.localPosition.WithY(num3);
		}
	}

	public void SetBlank()
	{
		Color color = this.color;
		color.a = 0f;
		this.color = color;
	}

	public void SetFont(Font font)
	{
		textDisplay.font = font;
		textDisplay.gameObject.SetActive(value: true);
		TMPTextDisplay.gameObject.SetActive(value: false);
	}

	public void SetFont(TMP_FontAsset font)
	{
		TMPTextDisplay.font = font;
		outlineWidth = storedWidth;
		textDisplay.gameObject.SetActive(value: false);
		TMPTextDisplay.gameObject.SetActive(value: true);
	}

	public void SetUnderlay(bool state)
	{
		if (state)
		{
			TMPTextDisplay.fontMaterial?.EnableKeyword("UNDERLAY_ON");
		}
		else
		{
			TMPTextDisplay.fontMaterial?.DisableKeyword("UNDERLAY_ON");
		}
	}

	public void SetRoom(bool followCam, int roomIndex)
	{
		if (followCam)
		{
			SetupFollowCam(roomIndex);
			return;
		}
		scrVfxControl instance = scrVfxControl.instance;
		int num = ((roomIndex != -1) ? roomIndex : 0);
		base.transform.SetParent(base.gm.parentContainer);
		base.transform.position = new Vector3(instance.RDWidth / 2f + 0.5f + 3000f * (float)num, instance.RDHeight / 2f, -200f);
	}

	public void AdvanceText(float? customFadeTime = null)
	{
		float num = customFadeTime ?? fadeTime;
		snapshotIndex++;
		if (snapshotIndex >= textSnapshots.Count)
		{
			return;
		}
		string text = textSnapshots[snapshotIndex];
		if (RDString.samuraiMode && !string.IsNullOrWhiteSpace(text))
		{
			text = "Samurai.";
		}
		if (scnGame.instance?.currentLevel != null)
		{
			try
			{
				text = scnGame.instance.currentLevel.EvaluateCurlyBracketsInString(text);
			}
			catch (Exception ex)
			{
				RDBaseDllDummy.printem("Using curly brackets but is not a variable: " + ex.Message);
			}
		}
		this.text = text;
		if (manualLyricAlpha)
		{
			color = baseColor.WithAlpha(0f);
			outlineColor = baseOutlineColor.WithAlpha(0f);
			return;
		}
		color = baseColor;
		outlineColor = baseOutlineColor;
		if (floatingTextMode == FloatingTextMode.HideAbruptly)
		{
			delayedInstantFade.Kill();
			delayedInstantFade = DOVirtual.DelayedCall(num, null, ignoreTimeScale: false).OnStart(() =>
			{
				canvasGroup.alpha = 1f;
			}).OnComplete(CompleteFade);
		}
		else if (floatingTextMode == FloatingTextMode.FadeOut)
		{
			Fade(0f, num);
		}
		else if (floatingTextMode == FloatingTextMode.Never)
		{
			canvasGroup.alpha = 1f;
		}
		if (!autoNarration || lastLineIndexNarrated + 1 >= narrationTimeIndex.Count || snapshotIndex != narrationTimeIndex[lastLineIndexNarrated + 1])
		{
			return;
		}
		lastLineIndexNarrated++;
		bool flag = false;
		foreach (LyricsGame syncedLyric in syncedLyrics)
		{
			if (syncedLyric != this && syncedLyric.lastLineIndexNarrated != -1)
			{
				flag = flag || (timeSnapshots[snapshotIndex] - syncedLyric.timeSnapshots[syncedLyric.snapshotIndex] < 0.5 && narrationText[lastLineIndexNarrated] == syncedLyric.narrationText[syncedLyric.lastLineIndexNarrated]);
			}
		}
		bool flag2 = false;
		if (timeSnapshots[snapshotIndex] != 0.0)
		{
			flag2 = timeSnapshots[snapshotIndex] + 0.5 < (double)(base.conductor.songFileTimeVisual * RDTime.speed + offset);
		}
		if (!flag && !flag2)
		{
			Narration.Say(narrationText[lastLineIndexNarrated], NarrationCategory.Subtitles, false);
		}
	}

	public void Fade(float alpha, float duration)
	{
		fillFadeTween.Kill();
		fillFadeTween = DOTween.To(() => color, (Color x) =>
		{
			color = x;
		}, baseColor.WithAlpha(alpha), duration).SetEase(Ease.InSine);
		outlineFadeTween.Kill();
		outlineFadeTween = DOTween.To(() => outlineColor, (Color x) =>
		{
			outlineColor = x;
		}, outlineColor.WithAlpha(alpha), duration).SetEase(Ease.OutQuad).OnStart(() =>
		{
			canvasGroup.alpha = 1f;
		})
			.OnComplete(CompleteFade)
			.Play();
	}

	public void KillFade()
	{
		fillFadeTween.Kill();
		outlineFadeTween.Kill();
	}

	private void CompleteFade()
	{
		if (!manualLyricAlpha)
		{
			if (snapshotIndex == textSnapshots.Count - 1 && !dontDestroyOnComplete)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			else
			{
				canvasGroup.alpha = 0f;
			}
		}
	}

	public void Reset()
	{
		delayedInstantFade.Kill();
		fillFadeTween.Kill();
		color = Color.clear;
		outlineFadeTween.Kill();
		outlineColor = Color.clear;
		snapshotIndex = -1;
	}

	public void ResetSnapshots()
	{
		textSnapshots = new List<string>();
		timeSnapshots = new List<double>();
	}

	private TextAlignmentOptions ConvertAlignment(TextAnchor alignment)
	{
		switch (alignment)
		{
		case TextAnchor.UpperLeft:
		case TextAnchor.UpperCenter:
		case TextAnchor.UpperRight:
			verticalAlignment = VerticalAlignmentOptions.Top;
			break;
		case TextAnchor.MiddleLeft:
		case TextAnchor.MiddleCenter:
		case TextAnchor.MiddleRight:
			verticalAlignment = VerticalAlignmentOptions.Middle;
			break;
		case TextAnchor.LowerLeft:
		case TextAnchor.LowerCenter:
		case TextAnchor.LowerRight:
			verticalAlignment = VerticalAlignmentOptions.Bottom;
			break;
		default:
			verticalAlignment = VerticalAlignmentOptions.Middle;
			break;
		}
		switch (alignment)
		{
		case TextAnchor.UpperLeft:
		case TextAnchor.MiddleLeft:
		case TextAnchor.LowerLeft:
			return TextAlignmentOptions.BaselineLeft;
		case TextAnchor.UpperCenter:
		case TextAnchor.MiddleCenter:
		case TextAnchor.LowerCenter:
			return TextAlignmentOptions.Baseline;
		case TextAnchor.UpperRight:
		case TextAnchor.MiddleRight:
		case TextAnchor.LowerRight:
			return TextAlignmentOptions.BaselineRight;
		default:
			return TextAlignmentOptions.Baseline;
		}
	}

	public void SetRenderQueueForTextDecoration()
	{
		textDisplay.material.renderQueue = 2000;
		if (TMPTextDisplay.fontMaterial != null)
		{
			TMPTextDisplay.fontMaterial.renderQueue = 2000;
		}
	}
}
