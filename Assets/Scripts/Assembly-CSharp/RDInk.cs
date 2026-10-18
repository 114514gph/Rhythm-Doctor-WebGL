using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DG.Tweening;
using Ink.Runtime;
using RDLevelEditor;
using RhythmWeightlifter;
using SA.GoogleDoc;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Networking;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class RDInk : RDBase
{
	private class TagInfo
	{
		public int start;

		public int length;

		public string name;

		public string value;

		public int tagLength;

		public TagInfo otherEnd;

		public int end => start + length;
	}

	private struct InlineAction
	{
		public string action;

		public int position;
	}

	public enum PanelSide
	{
		Bottom = 0,
		Top = 1
	}

	public enum PortraitSide
	{
		Left = 0,
		Right = 1
	}

	public delegate void RDInkCallback();

	public const int defaultPortraitWidth = 25;

	public const int defaultPortraitHeight = 30;

	public const int defaultPortraitScale = 2;

	public const float defaultSoundVolume = 1f;

	public const float defaultSoundBasePitch = 1f;

	public const float defaultSoundPitchRange = 0.03f;

	public const float defaultSoundHighFreqCutoff = 0f;

	public const float defaultSoundLowFreqCutoff = 0f;

	public const float defaultSoundDistortion = 0f;

	private const string DialoguesBasePath = "Dialogues/Auto/";

	private const float DelayForInterruptingText = 0.15f;

	private const float DelayForInterruptingTextPassive = 0.5f;

	private const float DefaultTextSpeedup = 4f;

	private const float PortraitlessNameOffset = 45f;

	private const float PortraitTopYOffset = -14f;

	public static Dictionary<char, float> punctuationDelay;

	private static ImmutableArray<string> delayExceptions = ImmutableArray<string>.Empty;

	private static readonly int UnderlayOffsetX = Shader.PropertyToID("_UnderlayOffsetX");

	private static readonly int UnderlayOffsetY = Shader.PropertyToID("_UnderlayOffsetY");

	public Story story;

	public Canvas canvas;

	[NonSerialized]
	public Text currentDialogueText;

	[NonSerialized]
	public LetterMovementController dialogueMovement;

	public GameObject overlay;

	[Header("Notifications")]
	public GameObject notification_panelUI;

	public Image notification_downArrow;

	public Text notification_textUI;

	public Image notification_image;

	public GameObject notification_backgroundUI;

	[Header("Dialogue")]
	public GameObject dialogue_panelUI;

	public Text dialogueTextLatinPixel;

	public TMP_Text dialogueTextVector;

	public Image dialogue_downArrow;

	public Image portraitShadow;

	public CustomPortrait customPortrait;

	[Header("Character")]
	public Text characterName;

	public RectTransform characterNameBackground;

	public RectTransform characterNameContainer;

	public AudioSource speechSource;

	public AudioHighPassFilter speechHighPass;

	public AudioLowPassFilter speechLowPass;

	public AudioDistortionFilter speechDistortion;

	[Header("Choice panel")]
	public Button buttonPrefab;

	public RDChoiceControl firstChoice;

	public GameObject choicesUI;

	private List<RDChoiceControl> mChoices = new List<RDChoiceControl>();

	private PlayStyleChange exitPlayStyleCache;

	[NonSerialized]
	public string file;

	[NonSerialized]
	public bool isEditorTutorial;

	[NonSerialized]
	public bool activeDialoguePresent;

	[NonSerialized]
	public bool isPlaying;

	[NonSerialized]
	public string currentKnot;

	[NonSerialized]
	public bool visible;

	[NonSerialized]
	public bool startingFromPlayerAction;

	[NonSerialized]
	public RDInkType type;

	[NonSerialized]
	public float anchorY;

	[NonSerialized]
	public int room = -1;

	[NonSerialized]
	public bool weightlifterInGameDialogue;

	public Coroutine currentCoroutine;

	private RDInkCallback onComplete;

	private Color colorToShow;

	private PortraitSide portraitSide;

	private AudioMixerGroup levelDialogueGroup;

	private bool isNotification;

	private bool lastDialogueWasNotification;

	private float timePerChar = 0.05f;

	private float speedMultiplier = 1f;

	private int currentChoice;

	private float waitTime;

	private bool shouldWait;

	private bool isWordByWord;

	private bool isFirstPanel;

	public bool stickyPanel;

	private TextAnchor textAlignment;

	private float soundTriggerTimer;

	private float soundTriggerCooldown;

	private float soundVolume;

	private float soundBasePitch;

	private float soundPitchRange;

	private float soundHighFreqCutoff;

	private float soundLowFreqCutoff;

	private float soundDistortion;

	private float letterShakeRadius = 1f;

	private float letterWaveFrequency = 1f;

	private float letterWaveHeight = 1f;

	private float letterSwirlFrequency = 1f;

	private float letterSwirlRadius = 1f;

	private Tween textShakeTween;

	private string lastNarrationChar;

	private float playbackTime;

	private int finishedDialogueFrame;

	private const float HidePanelDuration = 0.15f;

	private const string ActionToken = "DOTHIS";

	private const string PeriodToken = ".";

	private const string EllipsisToken = "...";

	private const string SwitchToken = "[NX]";

	private const string XboxToken = "[XBOX]";

	private const string GamepadToken = "[GAMEPAD]";

	private const string DesktopToken = "[DESKTOP]";

	private const string MobileToken = "[MOBILE]";

	private const string BoothToken = "[BOOTH]";

	private const string CustomButtonToken = "[CUSTOMBUTTON]";

	private const string OnePlayerModeToken = "[1P]";

	private const string TwoPlayerModeToken = "[2P]";

	private int lastBarScrubbed = -1;

	public Image downArrow
	{
		get
		{
			if (!isNotification)
			{
				return dialogue_downArrow;
			}
			return notification_downArrow;
		}
	}

	public GameObject panelUI
	{
		get
		{
			if (!isNotification)
			{
				return dialogue_panelUI;
			}
			return notification_panelUI;
		}
	}

	public static RDInk mainDialogue => scrGameManager.instance.inkDialogue;

	private float DefaultTimePerChar => (RDString.isCJK ? base.gc.ink_baseTime_CJK_multiplier : 1f) * ((type == RDInkType.Active) ? base.gc.ink_baseTimePerCharacterActive : base.gc.ink_baseTimePerCharacterPassive);

	public static float deltaTimeForDialogues => RDTime.deltaTimeIndependentPauseAffected * RDTime.speed;

	private bool usingPixelText => dialogueTextLatinPixel.gameObject.activeSelf;

	private GameObject dialogue
	{
		get
		{
			if (!usingPixelText)
			{
				return dialogueTextVector.gameObject;
			}
			return dialogueTextLatinPixel.gameObject;
		}
	}

	public string dialogueText
	{
		get
		{
			if (!usingPixelText)
			{
				return dialogueTextVector.text;
			}
			return dialogueTextLatinPixel.text;
		}
		set
		{
			if (usingPixelText)
			{
				dialogueTextLatinPixel.text = value;
			}
			else
			{
				dialogueTextVector.text = value;
			}
		}
	}

	private void Awake()
	{
		levelDialogueGroup = RDUtils.GetMixerGroup("LevelDialogueBoxes");
		if (punctuationDelay == null)
		{
			Dictionary<string, object> obj = Json.Deserialize(Resources.Load<TextAsset>("punctuationMarks").text) as Dictionary<string, object>;
			punctuationDelay = new Dictionary<char, float>();
			foreach (KeyValuePair<string, object> item in obj)
			{
				char key = item.Key[0];
				float value = Convert.ToSingle(item.Value);
				punctuationDelay.Add(key, value);
			}
		}
		if (delayExceptions.Length == 0)
		{
			delayExceptions = RDUtils.GetValues<LangCode>().SelectMany(GetDelayExceptions).ToImmutableArray();
		}
		static IEnumerable<string> GetDelayExceptions(LangCode langCode)
		{
			return from s in Localization.GetLocalizedString("dialogue.punctuationDelayExceptions", LangSection.Translations, langCode).Split(',')
				select s.Trim() into s
				where s != ""
				select s;
		}
	}

	private void Update()
	{
		if (base.game != null && base.game.paused)
		{
			return;
		}
		if (choicesUI.activeSelf)
		{
			if (RDInput.downPress)
			{
				SetCurrentChoice((currentChoice + 1) % mChoices.Count);
			}
			else if (RDInput.upPress)
			{
				SetCurrentChoice(((currentChoice == 0) ? mChoices.Count : currentChoice) - 1);
			}
			else if (RDInput.anyPlayerPress)
			{
				scrConductor.PlayImmediately("sndMenuButtonPush", 1f, levelDialogueGroup);
				story.ChooseChoiceIndex(mChoices[currentChoice].choice.index);
				choicesUI.SetActive(value: false);
				RefreshView();
			}
		}
		if (soundTriggerTimer > 0f)
		{
			soundTriggerTimer -= Time.unscaledDeltaTime;
		}
	}

	private void LateUpdate()
	{
		if (room > -1)
		{
			UpdatePanelPosition();
		}
	}

	public RDInk Run(string filename, string knotName, RDInkCallback onComplete = null, Dictionary<string, object> dictionary = null, bool keepExitPlayStyle = false)
	{
		Stop();
		if (!keepExitPlayStyle)
		{
			exitPlayStyleCache = PlayStyleChange.None;
		}
		currentCoroutine = StartCoroutine(RunInternal(filename, knotName, onComplete, dictionary, showAsNotification: false, keepExitPlayStyle));
		return this;
	}

	public RDInk Run(string knotName, RDInkCallback onComplete = null, Dictionary<string, object> dictionary = null, bool keepExitPlayStyle = false)
	{
		Debug.Log("calling " + knotName);
		Stop();
		if (!keepExitPlayStyle)
		{
			exitPlayStyleCache = PlayStyleChange.None;
		}
		currentCoroutine = StartCoroutine(RunInternal(null, knotName, onComplete, dictionary, showAsNotification: false, keepExitPlayStyle));
		return this;
	}

	public RDInk RunOnBeat(string knotName, RDInkCallback onComplete, float beatnumber, bool keepExitPlayStyle = false)
	{
		scrExecuteOnCertainBeat.Add(() =>
		{
			Stop();
			currentCoroutine = StartCoroutine(RunInternal(null, knotName, onComplete, null, showAsNotification: false, keepExitPlayStyle));
		}, beatnumber);
		return this;
	}

	public IEnumerator RunAndWait(string filename, string knotName, RDInkCallback onComplete = null, Dictionary<string, object> dictionary = null, PanelSide panelSide = PanelSide.Bottom)
	{
		Stop();
		yield return RunInternal(filename, knotName, onComplete, dictionary, showAsNotification: false, keepExitPlayStyle: false);
	}

	public RDInk RunNotification(string filename, string knotName, RDInkCallback onComplete = null, Dictionary<string, object> dictionary = null)
	{
		Stop();
		currentCoroutine = StartCoroutine(RunInternal(filename, knotName, onComplete, dictionary, showAsNotification: false, keepExitPlayStyle: false));
		return this;
	}

	public RDInk RunFromText(string dialogue, PanelSide panelSide = PanelSide.Bottom, PortraitSide portraitSide = PortraitSide.Left, bool keepExitPlayStyle = false)
	{
		Stop();
		SetPanelSide(panelSide);
		SetPortraitSide(portraitSide);
		if (!keepExitPlayStyle)
		{
			exitPlayStyleCache = PlayStyleChange.None;
		}
		currentCoroutine = StartCoroutine(StartCustomDialogue(dialogue));
		return this;
	}

	public void RunInlineInk(string text)
	{
		TextAsset arg = Resources.Load<TextAsset>("Dialogues/Auto/RDFunctions");
		string text2 = $"{arg}\nVAR type = \"Passive\"\n=== Dialogue === \n{text}\n-> END";
		Debug.Log("dialogue:\n" + text2);
		story = RDPluginsUtils.CompileInk(text2);
		if (story == null)
		{
			Debug.Log("story wasn't compiled properly");
			return;
		}
		exitPlayStyleCache = PlayStyleChange.None;
		Stop();
		BindExternalFunctions();
		currentCoroutine = StartCoroutine(RunInternal(null, "Dialogue", () =>
		{
			Debug.Log("we finished!");
		}, null, showAsNotification: false, keepExitPlayStyle: false));
	}

	public void Stop(bool instant = false)
	{
		if (currentCoroutine != null)
		{
			StopCoroutine(currentCoroutine);
			currentCoroutine = null;
			DialogueEnded(instant);
		}
	}

	public void Load(string filename)
	{
		if (file == filename)
		{
			RDBaseDllDummy.printem(filename + " was already loaded, skipping.");
			return;
		}
		file = filename;
		RDBaseDllDummy.printem("Loading " + filename);
		string text = $"{filename}_{RDString.language}";
		string customDialogueFolder = Persistence.GetCustomDialogueFolder();
		string path = System.IO.Path.Combine(Reader.externalDialoguesDataPath, "LastInkFetch.txt");
		string text2 = System.IO.Path.Combine(Reader.externalDialoguesDataPath, text + ".ink");
		bool flag = false;
		if (!string.IsNullOrEmpty(customDialogueFolder))
		{
			printe("A");
			string path2 = System.IO.Path.Combine(customDialogueFolder, text + ".ink");
			LoadFromExternalFile(path2);
			flag = true;
		}
		else if (File.Exists(path) && File.Exists(text2))
		{
			string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension("LastInkFetch.txt");
			string text3 = Resources.Load<TextAsset>("Dialogues/Auto/" + fileNameWithoutExtension).text;
			if (false)
			{
				Debug.Log("found newer external dialogue! " + text2);
				flag = true;
				LoadFromExternalFile(text2);
			}
		}
		if (!flag)
		{
			string text4 = "Dialogues/Auto/" + text;
			TextAsset textAsset = Resources.Load<TextAsset>(text4);
			if (textAsset == null || textAsset.text == "")
			{
				Debug.LogError("Ink JSON file is empty, corrupted, or doesn't exist: " + text4);
			}
			else
			{
				LoadFromJSONText(textAsset.text);
			}
		}
	}

	private IEnumerator RunInternal(string filename, string knotName, RDInkCallback onComplete, Dictionary<string, object> dictionary, bool showAsNotification, bool keepExitPlayStyle)
	{
		isPlaying = true;
		isNotification = showAsNotification;
		if (!keepExitPlayStyle)
		{
			exitPlayStyleCache = PlayStyleChange.None;
		}
		if (!isNotification)
		{
			characterNameBackground.gameObject.SetActive(value: false);
			SetActiveText();
		}
		else
		{
			currentDialogueText = notification_textUI;
		}
		dialogueMovement = dialogue.GetComponent<LetterMovementController>();
		if (!string.IsNullOrEmpty(filename))
		{
			Load(filename);
		}
		this.onComplete = onComplete;
		timePerChar = DefaultTimePerChar;
		return StartStory(knotName, dictionary);
	}

	private void SetType(string typeString)
	{
		RDInkType rDInkType = RDUtils.ParseEnum(typeString.ToLower(), RDInkType.Passive);
		SetType(rDInkType);
	}

	public void SetType(RDInkType inkType)
	{
		type = inkType;
		activeDialoguePresent = type == RDInkType.Active;
	}

	private void SetKnot(string knot)
	{
		currentKnot = knot;
		story.ChoosePathString(knot, true);
	}

	private IEnumerator StartStory(string knotName, Dictionary<string, object> dictionary = null)
	{
		bool flag = false;
		try
		{
			string text = (GC.twoPlayerMode ? "_2p" : (scnBase.isBooth ? "_expo" : (scnBase.isDesktop ? "_desktop" : (scnBase.isSwitch ? "_nx" : null))));
			SetKnot(knotName + text);
		}
		catch
		{
			try
			{
				printe("choosing " + knotName);
				SetKnot(knotName);
			}
			catch
			{
				flag = true;
				printe("<color=yellow>Knot " + knotName + " not found, not playing it.</color>");
			}
		}
		if (flag)
		{
			yield break;
		}
		if (dictionary != null)
		{
			foreach (KeyValuePair<string, object> item in dictionary)
			{
				story.variablesState[item.Key] = item.Value;
			}
		}
		if (story.variablesState.Contains("paigeStays"))
		{
			story.variablesState["paigeStays"] = Persistence.GetPaigeEnding();
		}
		string text2 = story.variablesState["type"] as string;
		SetType(text2);
		timePerChar = DefaultTimePerChar;
		colorToShow = Color.white;
		dialogueText = string.Empty;
		RestartChoices();
		yield return RefreshView();
	}

	private IEnumerator StartCustomDialogue(string s)
	{
		isPlaying = true;
		isNotification = false;
		LevelBase levelBase = scnGame.instance?.currentLevel;
		if (levelBase != null && (levelBase.activeDialogues || levelBase.activeDialoguesImmediately))
		{
			type = RDInkType.Active;
		}
		characterNameBackground.gameObject.SetActive(value: false);
		SetActiveText(s);
		dialogueMovement = dialogue.GetComponent<LetterMovementController>();
		onComplete = null;
		colorToShow = Color.white;
		float num = (RDString.GetAppropiateFontPackForString(s).isCJK ? base.gc.ink_baseTime_CJK_multiplier : 1f);
		timePerChar = ((type == RDInkType.Active) ? base.gc.ink_baseTimePerCharacterActive : base.gc.ink_baseTimePerCharacterPassive) * num;
		string[] array = s.Split(new string[1] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
		RemoveChildren();
		playbackTime = 0f;
		string[] array2 = array;
		foreach (string text in array2)
		{
			downArrow.gameObject.SetActive(value: false);
			ClearPanel();
			if (!visible)
			{
				ShowPanel();
				isFirstPanel = true;
			}
			yield return Say(text);
		}
		bool scrubbedPastEnd = playbackTime > 0.65f;
		if (type == RDInkType.Passive && !scrubbedPastEnd)
		{
			yield return new WaitForSecondsForDialogueCharacters(0.5f);
		}
		DialogueEnded(scrubbedPastEnd);
	}

	public void LoadFromExternalFile(string path)
	{
		MonoBehaviour.print("LoadFromExternalFile: " + path);
		string jsonText = RDFile.ReadAllText(path);
		if (path.EndsWith(".ink"))
		{
			story = RDPluginsUtils.CompileInk(jsonText);
			BindExternalFunctions();
		}
		else
		{
			LoadFromJSONText(jsonText);
		}
	}

	public IEnumerator LoadFromURL(string url)
	{
		UnityWebRequest uwr = new UnityWebRequest(url);
		yield return uwr.SendWebRequest();
		string jsonText = uwr.downloadHandler.text.Trim();
		LoadFromJSONText(jsonText);
	}

	public void LoadFromJSONText(string jsonText)
	{
		story = new Story(jsonText);
		BindExternalFunctions();
	}

	private void UpdatePanelPosition()
	{
		if (weightlifterInGameDialogue)
		{
			return;
		}
		RectTransform component = panelUI.GetComponent<RectTransform>();
		Vector2 anchorMin = (component.anchorMax = component.anchorMin.WithY(anchorY));
		component.anchorMin = anchorMin;
		component.pivot = component.pivot.WithY(anchorY);
		float num = -2f;
		float num2 = 4f;
		if (customPortrait.gameObject.activeSelf)
		{
			num = -14f;
			float num3 = (customPortrait.transform as RectTransform).sizeDelta.y;
			if (num3 > 60f)
			{
				num -= num3 - 60f;
			}
		}
		else if (characterNameBackground.gameObject.activeSelf)
		{
			num = -14f;
		}
		float num4 = 0f;
		float num5 = 0f;
		if (room > -1)
		{
			Transform transform = base.game.rooms[room].renderQuad.transform;
			num4 = transform.position.y + transform.lossyScale.y / 2f - RDBase.Vfx.RDHeight;
			num5 = transform.position.y - transform.lossyScale.y / 2f;
		}
		num4 += num;
		num5 += num2;
		component.anchoredPosition = component.anchoredPosition.WithY(Mathf.Lerp(num5, num4, anchorY));
	}

	public void SetPanelAnchor(float anchorY)
	{
		this.anchorY = anchorY;
	}

	public void SetPanelRoom(int room)
	{
		this.room = room;
	}

	public void SetPanelSide(PanelSide panelPosition)
	{
		SetPanelAnchor((panelPosition == PanelSide.Top) ? 1f : 0f);
	}

	public void SetPortraitSide(PortraitSide portraitSide)
	{
		this.portraitSide = portraitSide;
		bool leftPortrait = portraitSide == PortraitSide.Left;
		float anchorValue = (leftPortrait ? 0f : 1f);
		ChangeAnchorAndPivot(customPortrait.transform as RectTransform, changePivot: false, changePosition: true);
		ChangeAnchorAndPivot(characterNameContainer, changePivot: true, changePosition: false, flip: false);
		ChangeAnchorAndPivot(characterNameBackground, changePivot: true, changePosition: true, flip: false);
		void ChangeAnchorAndPivot(RectTransform rt, bool changePivot, bool changePosition, bool flip = true)
		{
			rt.anchorMin = rt.anchorMin.WithX(anchorValue);
			rt.anchorMax = rt.anchorMax.WithX(anchorValue);
			if (changePivot)
			{
				rt.pivot = rt.pivot.WithX(anchorValue);
			}
			if (changePosition)
			{
				rt.AnchorPosX(Mathf.Abs(rt.anchoredPosition.x) * (float)(leftPortrait ? 1 : (-1)));
			}
			if (flip)
			{
				rt.ScaleX(Mathf.Abs(rt.localScale.x) * (float)(leftPortrait ? 1 : (-1)));
			}
		}
	}

	private void RestartChoices()
	{
		foreach (RDChoiceControl mChoice in mChoices)
		{
			if (mChoice != firstChoice)
			{
				UnityEngine.Object.Destroy(mChoice.gameObject);
			}
		}
		mChoices.Clear();
		mChoices.Add(firstChoice);
	}

	public void ShowPanel()
	{
		float duration = (weightlifterInGameDialogue ? 0f : 0.4f);
		visible = true;
		panelUI.transform.DOKill(complete: true);
		panelUI.GetComponent<CanvasGroup>().DOKill(complete: true);
		panelUI.SetActive(value: true);
		overlay.SetActive(value: true);
		panelUI.GetComponent<CanvasGroup>().alpha = 1f;
		RectTransform component = panelUI.GetComponent<RectTransform>();
		component.localScale = Vector3.one * 0.8f;
		component.DOScale(Vector3.one, duration).SetEase(Ease.OutBack);
		if (isNotification)
		{
			notification_backgroundUI.SetActive(value: true);
			notification_backgroundUI.GetComponent<Image>().DOFade(0.73f, 0.4f).SetEase(Ease.Linear);
		}
	}

	private void ClearPanel()
	{
		dialogueTextLatinPixel.text = "";
		dialogueTextVector.text = "";
	}

	private void HidePanel(bool instant = false)
	{
		visible = false;
		overlay.SetActive(value: false);
		float duration = (instant ? 0f : 0.15f);
		panelUI.transform.localScale = Vector3.one;
		panelUI.GetComponent<CanvasGroup>().DOFade(0f, duration).SetEase(Ease.InSine);
		panelUI.transform.DOScale(new Vector3(0.7f, 0.7f, 1f), duration).SetEase(Ease.InSine).OnComplete(() =>
		{
			panelUI.SetActive(value: false);
		});
		if (isNotification)
		{
			notification_backgroundUI.GetComponent<Image>().DOFade(0f, duration).SetEase(Ease.Linear)
				.OnComplete(() =>
				{
					notification_backgroundUI.SetActive(value: false);
				});
		}
	}

	private IEnumerator RefreshView()
	{
		RemoveChildren();
		List<string> currentPlatformTokens = new List<string>();
		if (scnBase.isSwitch)
		{
			currentPlatformTokens.Add("[NX]");
		}
		else if (scnBase.isXbox)
		{
			currentPlatformTokens.Add("[XBOX]");
		}
		else if (scnBase.isMobile)
		{
			currentPlatformTokens.Add("[MOBILE]");
		}
		else if (scnBase.isGamepad)
		{
			currentPlatformTokens.Add("[GAMEPAD]");
		}
		else if (scnBase.isCustomButton)
		{
			currentPlatformTokens.Add("[CUSTOMBUTTON]");
		}
		else
		{
			currentPlatformTokens.Add("[DESKTOP]");
		}
		if (scnBase.isBooth)
		{
			currentPlatformTokens.Add("[BOOTH]");
		}
		currentPlatformTokens.Add(GC.twoPlayerMode ? "[2P]" : "[1P]");
		playbackTime = 0f;
		while (story.canContinue)
		{
			lastDialogueWasNotification = isNotification;
			string text = story.Continue();
			if (lastDialogueWasNotification != isNotification && visible)
			{
				isNotification = !isNotification;
				HidePanel();
				yield return new WaitForSecondsRealtime(0.5f);
				isNotification = !isNotification;
			}
			bool flag = false;
			for (int i = 0; i < currentPlatformTokens.Count; i++)
			{
				if (text.Contains(currentPlatformTokens[i].Insert(1, "!")))
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				continue;
			}
			if (text.StartsWith("[!", StringComparison.CurrentCulture))
			{
				text = text.Replace(text.Substring(0, text.IndexOf(']') + 1), string.Empty);
			}
			bool num = text.Contains("[XBOX]") || text.Contains("[GAMEPAD]") || text.Contains("[NX]") || text.Contains("[DESKTOP]") || text.Contains("[MOBILE]") || text.Contains("[BOOTH]") || text.Contains("[CUSTOMBUTTON]") || text.Contains("[1P]") || text.Contains("[2P]");
			bool flag2 = true;
			if (num)
			{
				flag2 = false;
				foreach (string item in currentPlatformTokens)
				{
					if (text.Contains(item))
					{
						text = text.Replace(item, string.Empty);
						flag2 = true;
					}
				}
			}
			if (!flag2)
			{
				continue;
			}
			text = text.Trim();
			if (shouldWait)
			{
				downArrow.gameObject.SetActive(value: false);
				shouldWait = false;
				yield return new WaitForSecondsForDialogueCharacters(waitTime);
				waitTime = 0f;
			}
			if (!isNotification)
			{
				SetActiveText();
			}
			else
			{
				currentDialogueText = notification_textUI;
			}
			dialogueMovement = dialogue.GetComponent<LetterMovementController>();
			if (!text.Contains("DOTHIS"))
			{
				downArrow.gameObject.SetActive(value: false);
				if (!visible)
				{
					ShowPanel();
					isFirstPanel = true;
				}
				if (dialogue == dialogueTextVector.gameObject)
				{
					text = text.Replace('’', '\'');
				}
				yield return Say(text);
				lastDialogueWasNotification = isNotification;
			}
		}
		if (story.currentChoices.Count > 0)
		{
			RestartChoices();
			choicesUI.SetActive(value: true);
			for (int j = 0; j < story.currentChoices.Count; j++)
			{
				Choice choice = story.currentChoices[j];
				RDChoiceControl component = firstChoice;
				if (j > 0)
				{
					GameObject obj = UnityEngine.Object.Instantiate(firstChoice.gameObject);
					if (obj == null)
					{
						MonoBehaviour.print("new choice is null");
					}
					RectTransform component2 = obj.GetComponent<RectTransform>();
					component2.SetParent(choicesUI.transform);
					RectTransform component3 = firstChoice.GetComponent<RectTransform>();
					component2.localScale = component3.localScale;
					component2.anchoredPosition = component3.anchoredPosition;
					component2.localPosition = component3.localPosition;
					Vector2 anchoredPosition = component2.anchoredPosition;
					anchoredPosition.y = -12f - 13f * (float)j;
					component2.anchoredPosition = anchoredPosition;
					component = obj.GetComponent<RDChoiceControl>();
					mChoices.Add(component);
				}
				component.choice = choice;
				component.text.text = choice.text.Trim();
				SetCurrentChoice(0);
			}
		}
		else
		{
			if (type == RDInkType.Passive)
			{
				yield return new WaitForSecondsForDialogueCharacters(0.5f);
			}
			DialogueEnded();
		}
	}

	private void SetActiveText(string text = null)
	{
		FontPack fontPack = (string.IsNullOrEmpty(text) ? RDString.currentFontPack : RDString.GetAppropiateFontPackForString(text));
		dialogueTextLatinPixel.gameObject.SetActive(fontPack.usePixelFont);
		dialogueTextVector.gameObject.SetActive(!fontPack.usePixelFont);
		dialogueTextVector.font = fontPack.vectorFontAsset;
		dialogueTextVector.fontSize = ((!fontPack.isCJK || fontPack.language == SystemLanguage.Japanese) ? 7 : 10);
		dialogueTextVector.fontSharedMaterial = UnityEngine.Object.Instantiate(fontPack.vectorFontAsset.material);
		dialogueTextVector.fontSharedMaterial.SetKeyword(new LocalKeyword(dialogueTextVector.fontSharedMaterial.shader, "UNDERLAY_ON"), value: true);
		dialogueTextVector.fontSharedMaterial.SetFloat(UnderlayOffsetX, 1f);
		dialogueTextVector.fontSharedMaterial.SetFloat(UnderlayOffsetY, -1f);
		dialogueTextVector.SetMaterialDirty();
		dialogueTextVector.SetVerticesDirty();
	}

	private void DialogueEnded(bool instant = false)
	{
		isPlaying = false;
		currentCoroutine = null;
		if (exitPlayStyleCache != PlayStyleChange.None)
		{
			RDBaseDllDummy.printem("Unfreeze " + exitPlayStyleCache);
			base.conductor.SetPlayStyle(exitPlayStyleCache);
		}
		choicesUI.SetActive(value: false);
		if (!stickyPanel)
		{
			HidePanel(instant || weightlifterInGameDialogue);
		}
		stickyPanel = false;
		activeDialoguePresent = false;
		if (!isEditorTutorial)
		{
			RDInput.touchBlocked = true;
		}
		if (onComplete != null)
		{
			onComplete();
		}
	}

	public void FastForward(double seconds)
	{
		if (currentCoroutine != null)
		{
			if (lastBarScrubbed == base.conductor.lastBarScrubbed)
			{
				RDBaseDllDummy.printem($"<color=red>not adding more fast forwards because we already added on this bar {lastBarScrubbed}</color>");
				return;
			}
			lastBarScrubbed = base.conductor.lastBarScrubbed;
			playbackTime += (float)seconds;
		}
	}

	private void SetCurrentChoice(int index)
	{
		foreach (RDChoiceControl mChoice in mChoices)
		{
			mChoice.arrow.SetActive(value: false);
		}
		mChoices[index].arrow.SetActive(value: true);
		currentChoice = index;
	}

	private void OnClickChoiceButton(Choice choice)
	{
		story.ChooseChoiceIndex(choice.index);
		RefreshView();
	}

	private Button CreateChoiceView(string text)
	{
		Button button = UnityEngine.Object.Instantiate(buttonPrefab);
		button.transform.SetParent(canvas.transform, worldPositionStays: false);
		button.GetComponentInChildren<Text>().text = text;
		button.GetComponent<HorizontalLayoutGroup>().childForceExpandHeight = false;
		return button;
	}

	private void RemoveChildren()
	{
		for (int num = canvas.transform.childCount - 1; num >= 0; num--)
		{
			GameObject gameObject = canvas.transform.GetChild(num).gameObject;
			if (gameObject.GetComponent<Button>() != null)
			{
				UnityEngine.Object.Destroy(gameObject);
			}
		}
	}

	private string ProcessTextWithColor(string fullText, int index, List<TagInfo> tags)
	{
		string text = fullText.Substring(0, index);
		int num = 0;
		foreach (TagInfo tag in tags)
		{
			if (tag.start >= index)
			{
				break;
			}
			string text2 = null;
			switch (tag.name)
			{
			case "color":
				text2 = TMPColorToHexConverter(tag);
				break;
			case "whisper":
				text2 = "#ffffffa0";
				break;
			case "bold":
			case "emphasis":
			case "highlight":
				text2 = "#03EBF5";
				break;
			}
			if (text2 != null)
			{
				string text3 = "<color=" + text2 + ">";
				text = text.Insert(tag.start + num, text3);
				num += text3.Length;
				string text4 = "</color>";
				if (tag.end < index)
				{
					text = text.Insert(tag.end + num, text4);
					num += text4.Length;
				}
				else
				{
					text += text4;
				}
			}
		}
		return text + "<color=#0000>" + fullText.Substring(index) + "</color>";
	}

	private string TMPColorToHexConverter(TagInfo tag)
	{
		return tag.value switch
		{
			"aqua" => "#00ffffff", 
			"brown" => "#a52a2aff", 
			"cyan" => "#00ffffff", 
			"darkblue" => "#0000a0ff", 
			"fuchsia" => "#ff00ffff", 
			"grey" => "#808080ff", 
			"lightblue" => "#add8e6ff", 
			"lime" => "#00ff00ff", 
			"magenta" => "#ff00ffff", 
			"maroon" => "#800000ff", 
			"navy" => "#000080ff", 
			"olive" => "#808000ff", 
			"silver" => "#c0c0c0ff", 
			"teal" => "#008080ff", 
			_ => tag.value, 
		};
	}

	private Match GetFirstMatch(Match a, Match b)
	{
		if (a == null || !a.Success)
		{
			if (!b.Success)
			{
				return null;
			}
			return b;
		}
		if (b == null || !b.Success)
		{
			return a;
		}
		if (a.Index >= b.Index)
		{
			return b;
		}
		return a;
	}

	private bool IsAnyPunctuationMark(string s, int index)
	{
		char c = s[index];
		bool result = punctuationDelay.ContainsKey(c);
		if (c == '.')
		{
			foreach (string delayException in delayExceptions)
			{
				if (PrevMunch(delayException))
				{
					return false;
				}
			}
		}
		return result;
		bool PrevMunch(string substring)
		{
			int length = substring.Length;
			if (index < length)
			{
				return false;
			}
			return s.Substring(index - length, length) == substring;
		}
	}

	private IEnumerator Say(string text)
	{
		Character character = Character.None;
		int num = text.IndexOf(':');
		bool flag = false;
		if (num >= 0)
		{
			string text2 = text.Substring(0, num);
			characterNameBackground.gameObject.SetActive(value: false);
			if (ParsePortrait(text2, out var isInternal, out var charName, out var expression, out var displayName))
			{
				bool flag2 = !isInternal;
				flag = !string.IsNullOrEmpty(charName);
				if (!flag)
				{
					ShowCharacterName("", customCharacter: false, displayName);
				}
				else if (flag2)
				{
					if (!customPortrait.Setup(charName, expression))
					{
						customPortrait.PlayExpression("neutral");
					}
					string text3 = displayName ?? customPortrait.customAnimation.currentClip?.portraitName ?? customPortrait.customAnimation.data.name;
					if (!text3.IsNullOrEmpty())
					{
						ShowCharacterName(charName, customCharacter: true, text3);
					}
					Character? characterFromAlias = RDEditorUtils.GetCharacterFromAlias(scnGame.instance.currentLevel.customCharacterData[charName].voice);
					if (characterFromAlias.HasValue)
					{
						speechSource.clip = RDUtils.GetSpeechTickerSoundForCharacter(characterFromAlias.Value);
					}
					else
					{
						string text4 = text2 + ".wav*external";
						string key = text2 + ".ogg*external";
						string key2 = charName + ".wav*external";
						string key3 = charName + ".ogg*external";
						Dictionary<string, AudioClip> audioLib = Singleton<AudioManager>.Instance.audioLib;
						if (audioLib.ContainsKey(text4))
						{
							speechSource.clip = audioLib[text4];
						}
						else if (audioLib.ContainsKey(key))
						{
							speechSource.clip = audioLib[key];
						}
						else if (audioLib.ContainsKey(key2))
						{
							speechSource.clip = audioLib[key2];
						}
						else if (audioLib.ContainsKey(key3))
						{
							speechSource.clip = audioLib[key3];
						}
						else
						{
							speechSource.clip = RDUtils.GetSpeechTickerSoundForCharacter(null);
							RDBaseDllDummy.printesw("Could not find file: \"" + text4 + "\". Please make sure it is in the level folder.");
						}
					}
				}
				else
				{
					character = RDUtils.ParseEnum(charName, Character.None);
					if (RDString.samuraiMode)
					{
						character = Character.Samurai;
					}
					if (character == Character.Weightlifter)
					{
						flag = false;
					}
					if (flag && !customPortrait.Setup(character, expression))
					{
						customPortrait.PlayExpression("neutral");
					}
					speechSource.clip = RDUtils.GetSpeechTickerSoundForCharacter(character);
					ShowCharacterName(charName, customCharacter: false, displayName);
				}
				int num2 = num + 1;
				text = text.Substring(num2, text.Length - num2).Trim();
				CustomAnimationClip currentClip = customPortrait.customAnimation.currentClip;
				RectTransform rectTransform = customPortrait.transform as RectTransform;
				if (currentClip != null)
				{
					rectTransform.sizeDelta = currentClip.GetPortraitTransformSize();
				}
			}
			else
			{
				speechSource.clip = RDUtils.GetSpeechTickerSoundForCharacter(null);
			}
		}
		else
		{
			speechSource.clip = RDUtils.GetSpeechTickerSoundForCharacter(null);
			characterNameBackground.gameObject.SetActive(value: false);
		}
		if (RDString.samuraiMode)
		{
			text = "Samurai.";
		}
		bool num3 = portraitSide == PortraitSide.Left;
		RectTransform component = dialogue.GetComponent<RectTransform>();
		_ = component.offsetMin;
		float num4 = (num3 ? 6f : 18f);
		float num5 = (num3 ? 18f : 6f);
		if (flag)
		{
			float num6 = (customPortrait.transform as RectTransform).sizeDelta.x;
			num4 = 10f + num6;
		}
		if (portraitSide == PortraitSide.Left)
		{
			component.offsetMin = component.offsetMin.WithX(num4);
			component.offsetMax = component.offsetMax.WithX(0f - num5);
		}
		else
		{
			component.offsetMin = component.offsetMin.WithX(num5);
			component.offsetMax = component.offsetMax.WithX(0f - num4);
		}
		customPortrait.gameObject.SetActive(flag);
		if (characterNameBackground.gameObject.activeSelf)
		{
			bool flag3 = characterNameContainer.anchorMin.x < 0.5f;
			characterNameContainer.anchoredPosition = new Vector2(flag ? 0f : (flag3 ? (-45f) : 45f), 0f);
		}
		UpdatePanelPosition();
		if (RDString.samuraiMode)
		{
			text = "Samurai.";
		}
		scnGame instance = scnGame.instance;
		if (instance != null && instance.currentLevel != null)
		{
			text = instance.currentLevel.EvaluateCurlyBracketsInString(text);
		}
		float baseSpeedMultiplier = character switch
		{
			Character.Ian => base.gc.ink_ian_Dialogue_Speed, 
			Character.Paige => base.gc.ink_paige_Dialogue_Speed, 
			_ => 1f, 
		};
		if (usingPixelText)
		{
			dialogueTextLatinPixel.color = colorToShow;
		}
		else
		{
			dialogueTextVector.color = colorToShow;
		}
		dialogueText = string.Empty;
		string text5 = text;
		List<TagInfo> tags = new List<TagInfo>();
		List<InlineAction> inlineActions = new List<InlineAction>();
		int num7 = 0;
		Regex regex = new Regex("\\<(\\w+)(?:\\s*=\\s*([^=\\>]+))?\\>|\\<(\\/\\w+)\\>");
		Match match = regex.Match(text5);
		Regex regex2 = new Regex("\\[([^]]+)\\]");
		Match match2 = regex2.Match(text5);
		while (match.Success || match2.Success)
		{
			Match firstMatch = GetFirstMatch(match, match2);
			if (firstMatch == match)
			{
				if (firstMatch.Groups[1].Success)
				{
					tags.Add(new TagInfo
					{
						start = firstMatch.Groups[0].Index - num7,
						tagLength = firstMatch.Groups[0].Value.Length,
						name = firstMatch.Groups[1].Value,
						value = firstMatch.Groups[2].Value
					});
				}
				else if (firstMatch.Groups[3].Success)
				{
					tags.Add(new TagInfo
					{
						start = firstMatch.Groups[0].Index - num7,
						tagLength = firstMatch.Groups[0].Length,
						name = firstMatch.Groups[3].Value
					});
				}
				match = match.NextMatch();
			}
			else
			{
				if (firstMatch.Groups[1].Success)
				{
					inlineActions.Add(new InlineAction
					{
						position = firstMatch.Index - num7,
						action = firstMatch.Groups[1].Value
					});
				}
				match2 = match2.NextMatch();
			}
			num7 += firstMatch.Groups[0].Length;
		}
		List<TagInfo> list = new List<TagInfo>();
		foreach (TagInfo item in tags)
		{
			if (item.name.StartsWith('/'))
			{
				for (int num8 = list.Count - 1; num8 >= 0; num8--)
				{
					if (list[num8].name == item.name.Substring(1))
					{
						item.otherEnd = list[num8];
						list[num8].otherEnd = item;
						list.Remove(list[num8]);
						break;
					}
				}
			}
			else
			{
				list.Add(item);
			}
		}
		TagInfo[] array = tags.ToArray();
		foreach (TagInfo tagInfo in array)
		{
			if (tagInfo.otherEnd == null)
			{
				tagInfo.length = text5.Length - tagInfo.start;
			}
			else if (tagInfo.start < tagInfo.otherEnd.start)
			{
				tagInfo.length = tagInfo.otherEnd.start - tagInfo.start;
				tags.Remove(tagInfo.otherEnd);
			}
		}
		string colorlessText = regex.Replace(text5, string.Empty);
		colorlessText = regex2.Replace(colorlessText, string.Empty);
		tags.AddRange(ConvertObsoleteActions(inlineActions));
		colorToShow = Color.white;
		downArrow.gameObject.SetActive(value: false);
		dialogueTextLatinPixel.alignment = textAlignment;
		dialogueTextVector.alignment = RDUtils.ParseEnum(textAlignment.ToString().Replace("Middle", "Midline").Replace("Upper", "Top")
			.Replace("Center", ""), (TextAlignmentOptions)0);
		float[] charTimeArray = new float[colorlessText.Length + 1];
		for (int j = 0; j < charTimeArray.Length - 1; j++)
		{
			if (IsAnyPunctuationMark(colorlessText, j))
			{
				bool flag4;
				do
				{
					int num9 = j + 1;
					if (colorlessText.Length > num9 && IsAnyPunctuationMark(colorlessText, num9))
					{
						charTimeArray[num9] = timePerChar;
						j++;
						flag4 = true;
						continue;
					}
					flag4 = false;
					if (colorlessText.Length >= 3 && j - 2 >= 0)
					{
						string text6 = colorlessText.Substring(Mathf.Max(j - 2, 0), 3);
						string text7 = colorlessText.Substring(Mathf.Max(j - 1, 0), 2);
						if (colorlessText.Length > num9 && (text6 == "..." || text7 == "……") && colorlessText[num9] != ' ')
						{
							charTimeArray[num9] = timePerChar;
						}
						else
						{
							charTimeArray[num9] = timePerChar + punctuationDelay[colorlessText[j]];
						}
					}
					else
					{
						charTimeArray[num9] = timePerChar + punctuationDelay[colorlessText[j]];
					}
				}
				while (flag4);
			}
			else
			{
				charTimeArray[j + 1] = timePerChar;
			}
		}
		if (inlineActions.Count > 0)
		{
			foreach (InlineAction item2 in inlineActions)
			{
				if (item2.action == "0")
				{
					charTimeArray[item2.position] = timePerChar;
				}
			}
		}
		if (base.conductor == null || (!base.conductor.isScrubbing && !base.conductor.runningScrubToNextBar))
		{
			bool flipCategoryQueueBehaviour = false;
			if ((type == RDInkType.Active && !isFirstPanel) || (isFirstPanel && startingFromPlayerAction))
			{
				flipCategoryQueueBehaviour = true;
			}
			isFirstPanel = false;
			startingFromPlayerAction = false;
			if (characterNameBackground.gameObject.activeSelf)
			{
				if (lastNarrationChar != characterName.text)
				{
					Narration.Say(RDString.Get("narration.dialogue").Replace("[character]", characterName.text).Replace("[text]", colorlessText), NarrationCategory.Dialogue, flipCategoryQueueBehaviour);
					lastNarrationChar = characterName.text;
				}
				else
				{
					Narration.Say(colorlessText, NarrationCategory.Dialogue, flipCategoryQueueBehaviour);
				}
			}
			else
			{
				Narration.Say(colorlessText, NarrationCategory.Dialogue, flipCategoryQueueBehaviour);
				lastNarrationChar = string.Empty;
			}
		}
		int index = 0;
		int lastIndex = -1;
		int s = 0;
		bool isDialogueMadeFaster = false;
		dialogueMovement.ClearAllChars();
		for (int k = 0; k < colorlessText.Length; k++)
		{
			if (colorlessText[k] == ' ')
			{
				continue;
			}
			letterShakeRadius = 1f;
			letterWaveFrequency = 1f;
			letterWaveHeight = 1f;
			letterSwirlFrequency = 1f;
			letterSwirlRadius = 1f;
			LetterMovementType letterMovementType = LetterMovementType.None;
			stickyPanel = false;
			foreach (TagInfo item3 in tags)
			{
				if (k < item3.start || k >= item3.end)
				{
					continue;
				}
				switch (item3.name)
				{
				case "shakeRadius":
				{
					if (float.TryParse(item3.value, out var result2))
					{
						letterShakeRadius = result2;
					}
					break;
				}
				case "waveSpeed":
				{
					if (float.TryParse(item3.value, out var result4))
					{
						letterWaveFrequency = result4;
					}
					break;
				}
				case "waveHeight":
				{
					if (float.TryParse(item3.value, out var result5))
					{
						letterWaveHeight = result5;
					}
					break;
				}
				case "swirlSpeed":
				{
					if (float.TryParse(item3.value, out var result3))
					{
						letterSwirlFrequency = result3;
					}
					break;
				}
				case "swirlRadius":
				{
					if (float.TryParse(item3.value, out var result))
					{
						letterSwirlRadius = result;
					}
					break;
				}
				case "shake":
				case "wave":
				case "swirl":
					letterMovementType = Enum.Parse<LetterMovementType>(item3.name, ignoreCase: true);
					break;
				case "static":
					letterMovementType = LetterMovementType.None;
					break;
				}
			}
			float[] array2;
			float[] multipliers;
			switch (letterMovementType)
			{
			case LetterMovementType.Shake:
				array2 = new float[1] { letterShakeRadius };
				goto IL_1060;
			case LetterMovementType.Wave:
				array2 = new float[2] { letterWaveFrequency, letterWaveHeight };
				goto IL_1060;
			case LetterMovementType.Swirl:
				array2 = new float[2] { letterSwirlFrequency, letterSwirlRadius };
				goto IL_1060;
			default:
				array2 = new float[0];
				goto IL_1060;
			case LetterMovementType.None:
				break;
				IL_1060:
				multipliers = array2;
				if (usingPixelText)
				{
					dialogueMovement.AddCharIndex(s, letterMovementType, multipliers);
				}
				else
				{
					dialogueMovement.AddCharIndex(k, letterMovementType, multipliers);
				}
				break;
			}
			s++;
		}
		soundTriggerTimer = 0f;
		while (index <= colorlessText.Length)
		{
			if (lastIndex != index)
			{
				speedMultiplier = baseSpeedMultiplier;
				soundTriggerCooldown = 0.065f;
				soundVolume = 1f;
				soundBasePitch = 1f;
				soundPitchRange = 0.03f;
				soundHighFreqCutoff = 0f;
				soundLowFreqCutoff = 0f;
				soundDistortion = 0f;
				RDUtils.SetMixerParameter("DialogueEchoWet", -80f);
				RDUtils.SetMixerParameter("DialogueReverbWet", -80f);
				foreach (InlineAction item4 in inlineActions)
				{
					if (item4.position != index)
					{
						continue;
					}
					string action = item4.action;
					switch (action)
					{
					case "shout":
						scrVfxControl.instance.ShoutEffect();
						continue;
					case "bigPause":
						yield return new WaitForSecondsForDialogueCharacters(base.gc.ink_bigPause / base.gc.ink_fastforward);
						continue;
					case "smallPause":
						yield return new WaitForSecondsForDialogueCharacters(base.gc.ink_smallPause / base.gc.ink_fastforward);
						continue;
					case "flash":
						scrVfxControl.instance.Flash();
						continue;
					}
					if (float.TryParse(action, out waitTime))
					{
						yield return new WaitForSecondsForDialogueCharacters(waitTime / 3f);
					}
					else
					{
						Debug.LogWarning("Using obsolete tag! [" + action + "]");
					}
				}
				foreach (TagInfo item5 in tags)
				{
					if (index < item5.start || index >= item5.end)
					{
						continue;
					}
					switch (item5.name)
					{
					case "speed":
					{
						if (float.TryParse(item5.value, out var result9))
						{
							speedMultiplier = result9;
							soundTriggerCooldown = 0.065f - Mathf.Clamp(Mathf.Log(result9, 2f), -3f, 3f) * 0.01f;
						}
						break;
					}
					case "pitch":
					{
						if (float.TryParse(item5.value, out var result7))
						{
							soundBasePitch = result7;
						}
						break;
					}
					case "pitchRange":
					{
						if (float.TryParse(item5.value, out var result8))
						{
							soundPitchRange = result8;
						}
						break;
					}
					case "volume":
					{
						if (float.TryParse(item5.value, out var result6))
						{
							soundVolume = result6;
						}
						break;
					}
					case "whisper":
						soundVolume *= 0.45f;
						soundBasePitch *= 0.95f;
						break;
					case "bold":
					case "emphasis":
					case "highlight":
					case "loud":
						soundVolume *= 1.2f;
						soundBasePitch *= 1.04f;
						break;
					case "louder":
					case "shout":
						soundVolume *= 1.8f;
						soundBasePitch *= 1.1f;
						break;
					case "phone":
						soundHighFreqCutoff = 4200f;
						soundLowFreqCutoff = 1800f;
						soundDistortion = 0.6f;
						break;
					case "megaphone":
						soundHighFreqCutoff = 6000f;
						soundDistortion = 0.4f;
						RDUtils.SetMixerParameter("DialogueEchoWet", 0f);
						RDUtils.SetMixerParameter("DialogueReverbWet", 0f);
						break;
					case "sticky":
						stickyPanel = true;
						break;
					}
				}
			}
			lastIndex = index;
			float num10 = speedMultiplier;
			bool flag5 = InputNext() && type == RDInkType.Active && finishedDialogueFrame != Time.frameCount && index > 0;
			if (scnBase.instance is scnRhythmWeightlifter)
			{
				num10 *= 2.5f;
			}
			if (isDialogueMadeFaster | flag5)
			{
				isDialogueMadeFaster = true;
				num10 *= base.gc.ink_fastforward;
			}
			if (RDBase.debugSettings.InstantDialogue && type == RDInkType.Active)
			{
				num10 *= 1000f;
			}
			float num11 = ((index > 0 && IsAnyPunctuationMark(colorlessText, index - 1)) ? base.gc.ink_punctuationMarkFastMultiplier : num10);
			playbackTime += deltaTimeForDialogues * num11;
			for (; index <= colorlessText.Length && playbackTime > charTimeArray[index]; index++)
			{
				if (index == colorlessText.Length)
				{
					playbackTime -= charTimeArray[index];
					index++;
					break;
				}
				int index2 = Math.Min(index + 1, colorlessText.Length);
				dialogueText = ProcessTextWithColor(colorlessText, index2, tags);
				playbackTime -= charTimeArray[index];
				if (speechSource.isActiveAndEnabled && soundTriggerTimer <= 0f)
				{
					speechSource.pitch = Mathf.Clamp(soundBasePitch * RXRandom.Range(1f / (1f + soundPitchRange), 1f + soundPitchRange), 0f, 3f);
					speechSource.volume = Mathf.Clamp(soundVolume * RXRandom.Range(0.91f, 1.1f) * scrConductor.GlobalOtherSoundVolume, 0f, 1f);
					speechHighPass.enabled = soundLowFreqCutoff > 0f;
					speechHighPass.cutoffFrequency = soundLowFreqCutoff;
					speechLowPass.enabled = soundHighFreqCutoff > 0f;
					speechLowPass.cutoffFrequency = soundHighFreqCutoff;
					speechDistortion.enabled = soundDistortion > 0f;
					speechDistortion.distortionLevel = soundDistortion;
					speechSource.Play();
					soundTriggerTimer = soundTriggerCooldown + RXRandom.Range(-0.005f, 0.005f);
				}
				if (dialogueText.Length > 0 && index < colorlessText.Length)
				{
					string text8 = dialogueText;
					if (text8[text8.Length - 1] == ' ' && isWordByWord)
					{
						yield return new WaitForSecondsForDialogueCharacters(1f);
					}
				}
				if (colorlessText[index] != ' ')
				{
					s++;
				}
			}
			if (index <= colorlessText.Length)
			{
				yield return null;
			}
		}
		dialogueText = ProcessTextWithColor(colorlessText, colorlessText.Length, tags);
		isWordByWord = false;
		if (type == RDInkType.Passive)
		{
			float num12 = base.gc.ink_passiveReadingTimePerCharacter * (float)colorlessText.Length;
			float num13 = DefaultTimePerChar / speedMultiplier * (float)colorlessText.Length;
			float value = num12 - num13;
			float timeToWait = Mathf.Clamp(value, base.gc.ink_minTimeBetweenLinesPassive, base.gc.ink_maxTimeBetweenLinesPassive);
			while (playbackTime < timeToWait)
			{
				playbackTime += deltaTimeForDialogues;
				yield return null;
			}
			playbackTime -= timeToWait;
			yield break;
		}
		for (float timeToWait = playbackTime; timeToWait < 0.15f; timeToWait += deltaTimeForDialogues)
		{
			if (InputNext())
			{
				break;
			}
			yield return null;
		}
		playbackTime = 0f;
		downArrow.gameObject.SetActive(value: true);
		if (Narration.IsEnabled)
		{
			RDBase.PlaySound("sndPressToContinue", 1f, RDUtils.GetMixerGroup("InterfaceSoundsParent"));
		}
		if (!RDBase.debugSettings.Debug || !Input.GetKey(KeyCode.LeftControl))
		{
			if (DebugSettings.instance.RecordingMode)
			{
				yield return new WaitForSecondsRealtime(1f);
			}
			else
			{
				while (InputNext())
				{
					yield return null;
				}
				while (!InputNext())
				{
					yield return null;
				}
			}
		}
		finishedDialogueFrame = Time.frameCount;
		RDBase.PlaySound("sndMenuButtonPush", 1f, levelDialogueGroup);
		void ShowCharacterName(string text9, bool customCharacter, string customName)
		{
			if (customName != null)
			{
				characterName.text = customName;
			}
			else if (customCharacter)
			{
				characterName.text = text9;
			}
			else
			{
				character = RDUtils.ParseEnum(text9, Character.None);
				characterName.text = RDString.Get("enum.Character." + character.ToString() + ".short").Split('-')[0].Split('(')[0].Trim();
			}
			characterNameBackground.gameObject.SetActive(characterName.text != string.Empty);
			RDStringToUIText.Apply(characterName);
			characterName.font = RDString.GetAppropiateFontForString(characterName.text);
			float num14 = characterName.rectTransform.offsetMin.x * 2f;
			characterNameBackground.SizeDeltaX(characterName.preferredWidth + num14);
		}
	}

	private TagInfo[] ConvertObsoleteActions(List<InlineAction> actions)
	{
		List<TagInfo> list = new List<TagInfo>();
		bool flag = true;
		InlineAction[] array = actions.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			InlineAction item = array[i];
			string[] array2 = item.action.Split('=');
			string text = array2[0];
			string s = ((array2.Length > 1) ? array2[1] : null);
			switch (text)
			{
			case "vslow":
				list.Add(new TagInfo
				{
					name = "speed",
					value = base.gc.ink_vslow_Dialogue_Speed.ToString(),
					start = item.position,
					length = 9999
				});
				break;
			case "slow":
				list.Add(new TagInfo
				{
					name = "speed",
					value = base.gc.ink_slow_Dialogue_Speed.ToString(),
					start = item.position,
					length = 9999
				});
				break;
			case "normal":
				list.Add(new TagInfo
				{
					name = "speed",
					value = base.gc.ink_normal_Dialogue_Speed.ToString(),
					start = item.position,
					length = 9999
				});
				break;
			case "fast":
				list.Add(new TagInfo
				{
					name = "speed",
					value = base.gc.ink_fast_Dialogue_Speed.ToString(),
					start = item.position,
					length = 9999
				});
				break;
			case "vfast":
				list.Add(new TagInfo
				{
					name = "speed",
					value = base.gc.ink_vfast_Dialogue_Speed.ToString(),
					start = item.position,
					length = 9999
				});
				break;
			case "vvvfast":
				list.Add(new TagInfo
				{
					name = "speed",
					value = base.gc.ink_vvvfast_Dialogue_Speed.ToString(),
					start = item.position,
					length = 9999
				});
				break;
			case "shout":
				list.Add(new TagInfo
				{
					name = "speed",
					value = base.gc.ink_shout_Dialogue_Speed.ToString(),
					start = item.position,
					length = 9999
				});
				flag = false;
				break;
			case "excited":
				list.Add(new TagInfo
				{
					name = "speed",
					value = base.gc.ink_excited_Dialogue_Speed.ToString(),
					start = item.position,
					length = 9999
				});
				break;
			case "pitch":
			case "pitchRange":
			case "volume":
			case "shakeRadius":
			case "waveSpeed":
			case "waveHeight":
			case "swirlSpeed":
			case "swirlRadius":
				list.Add(new TagInfo
				{
					name = text,
					value = ((float)int.Parse(s) / 100f).ToString(),
					start = item.position,
					length = 9999
				});
				break;
			case "shake":
			case "wave":
			case "swirl":
			case "static":
				list.Add(new TagInfo
				{
					name = text,
					start = item.position,
					length = 9999
				});
				break;
			default:
				flag = false;
				break;
			}
			if (flag)
			{
				actions.Remove(item);
			}
		}
		return list.ToArray();
	}

	private void PlaySound(string sound, float volume = 1f)
	{
		if (!isEditorTutorial)
		{
			scrConductor.PlayImmediately(sound, volume, RDUtils.GetMixerGroup("LevelCutscene"));
		}
	}

	private bool InputFaster()
	{
		if (base.game != null && base.game.paused)
		{
			return false;
		}
		if (!RDInput.p1IsPressed)
		{
			return RDInput.p2IsPressed;
		}
		return true;
	}

	private bool InputNext()
	{
		if (base.game != null && base.game.paused)
		{
			return false;
		}
		bool flag = scnBase.isMobile && RDInput.touch != null && RDInput.touch.GetTouchDown(0);
		if ((RDInput.anyPlayerPress || (RDBase.debugSettings.Debug && Input.GetKey(KeyCode.LeftControl) && RDInput.anyPlayerIsPressed)) | flag)
		{
			if (!isEditorTutorial)
			{
				RDInput.touchBlocked = true;
			}
			return true;
		}
		return false;
	}

	public bool ParsePortrait(string fullName, out bool isInternal, out string charName, out string expression, out string displayName)
	{
		displayName = null;
		int num = fullName.IndexOf('/');
		if (num != -1)
		{
			int num2 = num + 1;
			string text = fullName;
			int num3 = num2;
			displayName = text.Substring(num3, text.Length - num3);
			fullName = fullName.Substring(0, num);
		}
		int num4 = fullName.IndexOf('_');
		if (num4 != -1)
		{
			int num5 = num4 + 1;
			string text = fullName;
			int num3 = num5;
			expression = text.Substring(num3, text.Length - num3);
			charName = fullName.Substring(0, num4);
		}
		else
		{
			expression = "neutral";
			charName = fullName;
		}
		if (charName == string.Empty && displayName != string.Empty)
		{
			isInternal = false;
			return true;
		}
		isInternal = Enum.TryParse<Character>(charName, ignoreCase: true, out var result);
		if (isInternal)
		{
			if (result == Character.Janitor && (bool)scnLevelSelect.instance && scnLevelSelect.doingRobots)
			{
				charName = Character.RhythmJanitorReal.ToString();
			}
			return true;
		}
		if (scnBase.instance as scnGame != null)
		{
			LevelBase currentLevel = scnGame.instance.currentLevel;
			if (currentLevel.data != null && currentLevel.customCharacterData.ContainsKey(charName))
			{
				return true;
			}
		}
		if (RDEditorConstants.DialogueCharacterAliases.TryGetValue(charName, out var value))
		{
			isInternal = true;
			charName = value.ToString();
			return true;
		}
		return false;
	}

	private void BindExternalFunctions()
	{
		if (story == null)
		{
			Debug.Log("story is null!");
		}
		story.BindExternalFunction("flash", () =>
		{
			scrVfxControl.instance.Flash();
		});
		story.BindExternalFunction("setWordByWord", () =>
		{
			isWordByWord = true;
		});
		story.BindExternalFunction("shakeCam", () =>
		{
			if ((bool)scnGame.instance)
			{
				scrVfxControl.instance.ShakeCam(20, 5, -1);
			}
			else if ((bool)scnLevelSelect.instance)
			{
				scnBase.instance.wrldCamera.transform.DOKill(complete: true);
				scnBase.instance.wrldCamera.transform.DOShakePosition(0.5f, 3f, 40, 90f, snapping: true);
			}
		});
		story.BindExternalFunction("wait", (float seconds) =>
		{
			shouldWait = true;
			waitTime = seconds;
		});
		story.BindExternalFunction("playSound", (string soundName, float volume) =>
		{
			PlaySound(soundName, volume);
		});
		story.BindExternalFunction("playMusic", (string name, int crotchetsInBar, float bpm, bool fade) =>
		{
			base.conductor.StopSong();
			base.conductor.crotchetsPerBar = crotchetsInBar;
			base.conductor.SetBPM(bpm);
			base.conductor.startOfNextBar = base.conductor.audioPos + (double)scrConductor.latency;
			base.conductor.PlaySong(name, RDUtils.GetMixerGroup("OtherMusic"));
			RDBaseDllDummy.printem("Ink playMusic: " + name);
		});
		story.BindExternalFunction("stopMusic", (float fadeDuration) =>
		{
			scnLevelSelect instance = scnLevelSelect.instance;
			if (instance != null && instance.playSongCall != null)
			{
				printe("level select song call killed!");
				instance.playSongCall.Kill();
			}
			base.conductor.StopSong(fadeDuration);
			RDBaseDllDummy.printem("Ink stopMusic");
		});
		story.BindExternalFunction("restartLevelSelectMusic", () =>
		{
			base.conductor.StopSong();
			scnLevelSelect.instance.StartConductor();
		});
		story.BindExternalFunction("startAmbiences", (float fadeDuration) =>
		{
			scnLevelSelect instance = scnLevelSelect.instance;
			if (instance != null)
			{
				instance.StartAllAmbiences(fadeDuration);
				RDBaseDllDummy.printem("Ink startAmbiences called in level select");
			}
			LevelBase levelBase = scnGame.instance?.currentLevel;
			if (levelBase != null)
			{
				levelBase.StartAmbiences(fadeDuration);
				RDBaseDllDummy.printem("Ink startAmbiences called in cutscene level");
			}
		});
		story.BindExternalFunction("stopAmbiences", (float fadeDuration) =>
		{
			scnLevelSelect instance = scnLevelSelect.instance;
			if (instance != null)
			{
				instance.StopAllAmbiences(fadeDuration);
				RDBaseDllDummy.printem("Ink stopAmbiences called in level select");
			}
			LevelBase levelBase = scnGame.instance?.currentLevel;
			if (levelBase != null)
			{
				levelBase.StopAmbiences(fadeDuration);
				RDBaseDllDummy.printem("Ink stopAmbiences called in cutscene level");
			}
		});
		story.BindExternalFunction("animCharacter", (string cname, string animation) =>
		{
			scrChar.Named(cname).PlayAnim(animation);
		});
		story.BindExternalFunction("playExpression", (string goName, string expression) =>
		{
			GameObject.Find(goName).GetComponent<scrChar>().PlayExpression(expression);
		});
		story.BindExternalFunction("playExpressionD", (string goName, string expression, float delay) =>
		{
			delay /= RDTime.speed;
			DOVirtual.DelayedCall(delay, () =>
			{
				GameObject.Find(goName).GetComponent<scrChar>().PlayExpression(expression);
			});
			WaitForAnimation(delay);
		});
		story.BindExternalFunctionGeneral("move", (object[] args) =>
		{
			MoveGameObject((string)TryCoerce<string>(args[0]), (float)TryCoerce<float>(args[1]), (float)TryCoerce<float>(args[2]), (float)TryCoerce<float>(args[3]));
			return "";
		});
		story.BindExternalFunctionGeneral("move_ease", (object[] args) =>
		{
			MoveGameObject((string)TryCoerce<string>(args[0]), (float)TryCoerce<float>(args[1]), (float)TryCoerce<float>(args[2]), (float)TryCoerce<float>(args[3]), (string)TryCoerce<string>(args[4]));
			return "";
		});
		story.BindExternalFunctionGeneral("moveX", (object[] args) =>
		{
			MoveGameObject((string)TryCoerce<string>(args[0]), (float)TryCoerce<float>(args[1]), float.NaN, (float)TryCoerce<float>(args[2]));
			return "";
		});
		story.BindExternalFunctionGeneral("moveX_ease", (object[] args) =>
		{
			MoveGameObject((string)TryCoerce<string>(args[0]), (float)TryCoerce<float>(args[1]), float.NaN, (float)TryCoerce<float>(args[2]), (string)TryCoerce<string>(args[3]));
			return "";
		});
		story.BindExternalFunctionGeneral("moveY", (object[] args) =>
		{
			MoveGameObject((string)TryCoerce<string>(args[0]), float.NaN, (float)TryCoerce<float>(args[1]), (float)TryCoerce<float>(args[2]));
			return "";
		});
		story.BindExternalFunctionGeneral("moveY_ease", (object[] args) =>
		{
			MoveGameObject((string)TryCoerce<string>(args[0]), float.NaN, (float)TryCoerce<float>(args[1]), (float)TryCoerce<float>(args[2]), (string)TryCoerce<string>(args[3]));
			return "";
		});
		story.BindExternalFunctionGeneral("translate", (object[] args) =>
		{
			MoveGameObject((string)TryCoerce<string>(args[0]), (float)TryCoerce<float>(args[1]), (float)TryCoerce<float>(args[2]), (float)TryCoerce<float>(args[3]), "", translate: true);
			return "";
		});
		story.BindExternalFunctionGeneral("translate_ease", (object[] args) =>
		{
			MoveGameObject((string)TryCoerce<string>(args[0]), (float)TryCoerce<float>(args[1]), (float)TryCoerce<float>(args[2]), (float)TryCoerce<float>(args[3]), (string)TryCoerce<string>(args[4]), translate: true);
			return "";
		});
		story.BindExternalFunctionGeneral("translateX", (object[] args) =>
		{
			MoveGameObject((string)TryCoerce<string>(args[0]), (float)TryCoerce<float>(args[1]), float.NaN, (float)TryCoerce<float>(args[2]), "", translate: true);
			return "";
		});
		story.BindExternalFunctionGeneral("translateX_ease", (object[] args) =>
		{
			MoveGameObject((string)TryCoerce<string>(args[0]), (float)TryCoerce<float>(args[1]), float.NaN, (float)TryCoerce<float>(args[2]), (string)TryCoerce<string>(args[3]), translate: true);
			return "";
		});
		story.BindExternalFunctionGeneral("translateY", (object[] args) =>
		{
			MoveGameObject((string)TryCoerce<string>(args[0]), float.NaN, (float)TryCoerce<float>(args[1]), (float)TryCoerce<float>(args[2]));
			return "";
		});
		story.BindExternalFunctionGeneral("runEvent", (object[] args) =>
		{
			string text = (string)TryCoerce<string>(args[0]);
			RunLevelEvent(text);
			printe("running event " + text);
			return "";
		});
		story.BindExternalFunctionGeneral("translateY_ease", (object[] args) =>
		{
			MoveGameObject((string)TryCoerce<string>(args[0]), float.NaN, (float)TryCoerce<float>(args[1]), (float)TryCoerce<float>(args[2]), (string)TryCoerce<string>(args[3]), translate: true);
			return "";
		});
		story.BindExternalFunction("scaleX", (string goName, float x) =>
		{
			GameObject.Find(goName).transform.ScaleX(x);
		});
		story.BindExternalFunction("scaleY", (string goName, float y) =>
		{
			GameObject.Find(goName).transform.ScaleX(y);
		});
		story.BindExternalFunction("scale", (string goName, float x, float y) =>
		{
			GameObject.Find(goName).transform.ScaleXY(x, y);
		});
		story.BindExternalFunction("darkenBackgrounds", () =>
		{
			scrVfxControl.instance.DarkenBackgrounds();
		});
		story.BindExternalFunction("lightenBackgrounds", () =>
		{
			scrVfxControl.instance.LightenBackgrounds();
		});
		story.BindExternalFunction("fadeOut", () =>
		{
			scrVfxControl.instance.FadeOut();
		});
		story.BindExternalFunction("fadeIn", () =>
		{
			scrVfxControl.instance.FadeIn();
		});
		story.BindExternalFunctionGeneral("moveCam", (object[] args) =>
		{
			MoveTransform(base.game.rooms[0].cameraContainer, (float)TryCoerce<float>(args[0]), (float)TryCoerce<float>(args[1]), (float)TryCoerce<float>(args[2]), (string)TryCoerce<string>(args[3]));
			return "";
		});
		story.BindExternalFunctionGeneral("moveCam_ease", (object[] args) =>
		{
			MoveTransform(base.game.rooms[0].cameraContainer, (float)TryCoerce<float>(args[0]), (float)TryCoerce<float>(args[1]), (float)TryCoerce<float>(args[2]), (string)TryCoerce<string>(args[3]));
			return "";
		});
		story.BindExternalFunctionGeneral("translateCam", (object[] args) =>
		{
			MoveTransform(base.game.rooms[0].cameraContainer, (float)TryCoerce<float>(args[0]), (float)TryCoerce<float>(args[1]), (float)TryCoerce<float>(args[2]), "", translate: true);
			return "";
		});
		story.BindExternalFunctionGeneral("translateCam_ease", (object[] args) =>
		{
			MoveTransform(base.game.rooms[0].cameraContainer, (float)TryCoerce<float>(args[0]), (float)TryCoerce<float>(args[1]), (float)TryCoerce<float>(args[2]), (string)TryCoerce<string>(args[3]), translate: true);
			return "";
		});
		story.BindExternalFunctionGeneral("setColor", (object[] args) =>
		{
			colorToShow = new Color((float)TryCoerce<float>(args[0]), (float)TryCoerce<float>(args[1]), (float)TryCoerce<float>(args[2]), (float)TryCoerce<float>(args[3]));
			return "";
		});
		story.BindExternalFunction("setSpeed", (float speed) =>
		{
			RDBaseDllDummy.printem("speed changed to " + speed);
			timePerChar = DefaultTimePerChar / speed;
			return "";
		});
		story.BindExternalFunction("hidePanel", (float seconds) =>
		{
			RDBaseDllDummy.printem("hidepanel: " + seconds);
			HideTemporarily(seconds);
		});
		story.BindExternalFunction("waitForAnimations", () =>
		{
			shouldWait = true;
		});
		story.BindExternalFunction("setPanelSide", (string side) =>
		{
			PanelSide panelSide = RDUtils.ParseEnum(side, PanelSide.Bottom);
			SetPanelSide(panelSide);
		});
		story.BindExternalFunction("runLevelMethod", (string methodName) =>
		{
			LevelBase currentLevel = scnGame.instance.currentLevel;
			currentLevel.GetType().GetMethod(methodName).Invoke(currentLevel, null);
		});
		story.BindExternalFunction("waitN", (string goName, float x) =>
		{
			Debug.Log("waitN");
			GameObject.Find(goName).transform.ScaleX(x);
		});
		story.BindExternalFunction("setDialogueType", (string type) =>
		{
			SetType(type);
		});
		story.BindExternalFunction("changeWindowToNotification", (string imageName) =>
		{
			isNotification = true;
			SetNotificationImage(imageName);
		});
		story.BindExternalFunction("changeWindowToDialogue", () =>
		{
			isNotification = false;
		});
		story.BindExternalFunction("readNarration", (string descID) =>
		{
			scnGame.instance.currentLevel.NarrateDescription(descID);
		});
		story.BindExternalFunction("forceNarrateSpeakerName", () =>
		{
			lastNarrationChar = string.Empty;
		});
		story.BindExternalFunction("movePaigeToVendingMachine", () =>
		{
			scnLevelSelect.instance.MovePaigeToVendingMachine();
		});
		story.BindExternalFunction("doAthleteWardTransition", () =>
		{
			scnLevelSelect.instance.DoAthleteWardTransition();
		});
		story.BindExternalFunction("doAct5Intro", () =>
		{
			scnLevelSelect.instance.PlayAct5Intro();
		});
		story.BindExternalFunction("endCutscene", () =>
		{
			if (base.editorMode)
			{
				base.editor.dialog.Show("endCutscene() has been called on ink.", new string[1] { "Ok" }, (int selectedOption) =>
				{
					base.editor.dialog.Hide();
				});
			}
			else
			{
				HidePanel();
				printe("endCutscene");
				_ = scnLevelSelect.instance;
				DOVirtual.DelayedCall(0.15f, () =>
				{
					RDBase.Vfx.FadeOut(1f);
					base.conductor.StopSong(1f);
					base.game.currentLevel.StopAmbiences(1f);
					DOVirtual.DelayedCall(1.2f, () =>
					{
						scnBase.GoToLevelSelect();
					});
				});
			}
		});
		story.BindExternalFunction("doIntimateCutsceneEvent1", () =>
		{
			StartCoroutine(DoIntimateCutsceneEvent1());
			return "";
		});
		story.BindExternalFunction("setAlignment", (string alignment) =>
		{
			textAlignment = RDUtils.ParseEnum(alignment, TextAnchor.UpperLeft);
		});
	}

	private IEnumerator DoIntimateCutsceneEvent1()
	{
		for (int i = 3; i <= 10; i++)
		{
			yield return new WaitForSeconds(0.6f);
			RunLevelEvent("act1cutevent" + i);
		}
	}

	private void SetNotificationImage(string imageName)
	{
		notification_image.sprite = Resources.Load("Notifications/" + imageName, typeof(Sprite)) as Sprite;
	}

	private void WaitForAnimation(float seconds)
	{
		waitTime = Mathf.Max(waitTime, seconds);
	}

	private void HideTemporarily(float seconds = 0f)
	{
		HidePanel();
		shouldWait = true;
		waitTime = seconds;
	}

	private void RunLevelEvent(string tag)
	{
		base.game.currentLevel.RunEventsWithTag(tag);
	}

	private void MoveGameObject(string goName, float x, float y, float duration, string easeString = null, bool translate = false)
	{
		GameObject gameObject = GameObject.Find(goName);
		if (gameObject == null)
		{
			RDBaseDllDummy.printem("Couldn't find GameObject named " + goName);
			return;
		}
		Transform t = gameObject.transform;
		MoveTransform(t, x, y, duration, easeString, translate);
	}

	private void MoveTransform(Transform t, float x, float y, float duration, string easeString = null, bool translate = false)
	{
		Ease ease = (string.IsNullOrEmpty(easeString) ? Ease.Linear : RDUtils.ParseEnum(easeString, Ease.Linear));
		if (float.IsNaN(x))
		{
			x = (translate ? 0f : t.localPosition.x);
		}
		if (float.IsNaN(y))
		{
			y = (translate ? 0f : t.localPosition.y);
		}
		Vector3 vector = (translate ? t.localPosition.AddXY(x, y) : t.localPosition.WithXY(x, y));
		if (duration > 0f)
		{
			t.DOLocalMove(vector, duration).SetEase(ease);
		}
		else
		{
			t.localPosition = vector;
		}
		WaitForAnimation(duration);
	}

	private object TryCoerce<T>(object value)
	{
		if (value == null)
		{
			return null;
		}
		if (value is T)
		{
			return (T)value;
		}
		if (value is float && typeof(T) == typeof(int))
		{
			return (int)Mathf.Round((float)value);
		}
		if (value is int && typeof(T) == typeof(float))
		{
			return (float)(int)value;
		}
		if (value is int && typeof(T) == typeof(bool))
		{
			return ((int)value != 0) ? true : false;
		}
		if (typeof(T) == typeof(string))
		{
			return value.ToString();
		}
		MonoBehaviour.print("Failed to cast " + value.GetType().Name + " to " + typeof(T).Name);
		return null;
	}

	public RDInk Freeze(PlayStyleChange changeAtStart = PlayStyleChange.Prolong, PlayStyleChange changeAfterDialogue = PlayStyleChange.OnNextBar)
	{
		RDBaseDllDummy.printem($"<color=cyan>changeAtStart {changeAtStart}, changeAfterDialogue {changeAfterDialogue}</color>");
		base.conductor.Freeze(changeAtStart);
		exitPlayStyleCache = changeAfterDialogue;
		return this;
	}

	public RDInk Prolong(int numberOfBars = 0)
	{
		Freeze();
		base.conductor.playStyleTempCounter = numberOfBars;
		return this;
	}

	public RDInk Loop()
	{
		Freeze(PlayStyleChange.Loop);
		return this;
	}

	public RDInk WaitForNextBarOnEnd()
	{
		exitPlayStyleCache = PlayStyleChange.OnNextBar;
		return this;
	}

	public RDInk JumpOnEnd()
	{
		exitPlayStyleCache = PlayStyleChange.Immediately;
		return this;
	}

	public RDInk LoopOnEnd()
	{
		exitPlayStyleCache = PlayStyleChange.Loop;
		return this;
	}

	public RDInk BeatLoopOnlyOnEnd()
	{
		exitPlayStyleCache = PlayStyleChange.BeatLoopOnly;
		return this;
	}
}
