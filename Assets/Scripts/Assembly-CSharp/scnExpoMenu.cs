using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using RDLevelEditor;
using SA.GoogleDoc;
using UnityEngine;
using UnityEngine.UI;
using UnityFileDialog;

public class scnExpoMenu : scnBase
{
	private enum TextSwitchSelection
	{
		MainSelection = 0,
		Language = 1,
		VoiceLanguage = 2
	}

	public const string PlayerSwapKey = "PlayerSwap";

	public const string JoyconXFlipKey = "JoyconXFlip";

	private const float MaxTimeBetweenButtonsToSelectLevel = 0.04f;

	private const float MinHoldTime = 1.3f;

	private const float MinHoldFakeTransition = 0.4f;

	private const int secretLevelIndex1p = 6;

	private const int artExerciseLevel1P = 7;

	private const int secretLevelIndex2p = 5;

	private const string sleevePaintId = "sleevePaint";

	private const string rhythmWeightlifterId = "rhythmWeightlifter";

	public const string ArtExerciseLevel = "ArtExercise";

	public const string Op10No4Level = "Op10No4";

	public static readonly string[] levels1p = new string[8] { "OrientalInsomniac", "Boss2", "HaileyDuet", "Lesmis", "InsomniacHard", "sleevePaint", "Op10No4", "ArtExercise" };

	private string[] levels2p = new string[7] { "OrientalInsomniac", "Boss2", "HaileyDuet", "Lesmis", "rhythmWeightlifter", "fGirlfriend", "sleevePaint" };

	private string[] description1p = new string[8] { "medium", "medium", "hard", "hard", "harder", "sleevePaint", "easiest", "easy" };

	private string[] description2p = new string[7] { "medium", "medium", "hard", "hard", "bonus", "bonus", "sleevePaint" };

	public Logo logo;

	public tk2dSprite background;

	public GameObject currentScene;

	public GameObject expoSetttings;

	public bool isTransitioning;

	public TextMesh demoComplete;

	public GameObject noteParticles;

	public GameObject newInstructions;

	public GameObject oldInstructions;

	public AnimationData sceneTransition;

	public float ratioControlsBlocked;

	public RDIntroMenu introMenu;

	public AnimationData secretLevelTransition;

	public CreditsScroller[] creditsPages;

	private int currentCreditPageIndex;

	[Header("Debug")]
	public Text debugText;

	public Text setDialogueFolderLabel;

	public Text switchFlag;

	public Text customButtonFlag;

	[Header("Divekick-specific")]
	public GameObject playerSelect_divekick;

	public GameObject onePlayer_divekick;

	public GameObject twoPlayer_divekick;

	public GameObject[] alternateActiveEachBeat;

	[NonSerialized]
	public bool startAfreshSelected;

	private string[] textSwitchOptions_original;

	private string[] textSwitchOptions_language;

	private string[] textSwitchOptions_voiceLanguage;

	public List<WalkingSelectableCharacter> walkingCharacters = new List<WalkingSelectableCharacter>();

	private GameObject scenes;

	private GameObject introScene;

	private GameObject creditsScene;

	private GameObject playersScene;

	private GameObject onePlayerScene;

	private GameObject twoPlayerScene;

	private TextSwitch textSwitch;

	private int levelSelected1p;

	private List<GameObject> levelObjects1p = new List<GameObject>();

	private int levelSelected2p;

	private List<GameObject> levelObjects2p = new List<GameObject>();

	private Color NotSelectedColor;

	private Color SelectedColor;

	private GameObject mOnePlayerButton;

	private GameObject mTwoPlayersButton;

	private TextMesh mOnePlayerLevelDescription;

	private TextMesh mOnePlayerLevelDescription_secret;

	private TextMesh mTwoPlayerLevelDescription;

	private int mPlayersModeSelected;

	private Vector2 initialLogoPosition;

	private TextSwitchSelection textSwitchSelection;

	[NonSerialized]
	public string defaultInstructions = "";

	[NonSerialized]
	public float spotlightAlpha;

	[NonSerialized]
	public bool brightMode;

	private float p1DownTimestamp = -50f;

	private float p2DownTimestamp = -100f;

	private Tween volumeFade;

	private float anyKeyTimestamp;

	private const float LowVolume = 0.3f;

	private const float FullVolume = 1f;

	private Transform spotlightShouldFocus;

	private int prevOnePlayerLevel = -1;

	private int prevTwoPlayerLevel = -1;

	private Tween semiBackTween;

	public new static scnExpoMenu instance => scnBase._instance as scnExpoMenu;

	private new void Awake()
	{
		Persistence.currentSlotIndex = 0;
		base.Awake();
		scnBase._instance = this;
		scnBase.currentLevelSelect = "scnExpoMenu";
		muteWhenGameUnfocused = true;
	}

	private new void Start()
	{
		base.Start();
		base.gc.booth = true;
		if (!Application.isEditor)
		{
			BoothTimer.Run();
		}
		RDRichPresence.Instance?.UpdatePresence();
		scenes = GameObject.Find("Scenes");
		introScene = GameObject.Find("Intro");
		playersScene = GameObject.Find("Players");
		onePlayerScene = GameObject.Find("OnePlayer");
		twoPlayerScene = GameObject.Find("TwoPlayer");
		creditsScene = GameObject.Find("Credits");
		walkingCharacters.AddRange(UnityEngine.Object.FindObjectsOfType<WalkingSelectableCharacter>());
		foreach (WalkingSelectableCharacter walkingCharacter in walkingCharacters)
		{
			foreach (WalkingSelectableCharacter.Destination dayDestination in walkingCharacter.dayDestinations)
			{
				if (dayDestination.preferred)
				{
					SelectableCharacter obj = dayDestination.destEnt as SelectableCharacter;
					obj.gamePosition = obj.gameObject.transform.position;
					walkingCharacter.MoveCharacter(dayDestination, instant: true);
				}
			}
		}
		Vector2 vector = wrldCamera.RDFullNativeResolution();
		float num = vector.x;
		float num2 = vector.y;
		playersScene.transform.localPosition = new Vector3(0f, 0f - num2, 0f);
		onePlayerScene.transform.localPosition = new Vector3(0f - num, 0f - num2, 0f);
		twoPlayerScene.transform.localPosition = new Vector3(num, 0f - num2, 0f);
		creditsScene.transform.localPosition = new Vector3(0f - num, 0f, 0f);
		currentScene = introScene;
		textSwitch = introMenu.textSwitch;
		textSwitchOptions_original = introMenu.textSwitch.options;
		textSwitch.gameObject.SetActive(value: true);
		List<string> list = new List<string>();
		foreach (SystemLanguage availableLanguage in RDString.AvailableLanguages)
		{
			LangCode lang = RDUtils.ParseEnum(availableLanguage.ToString(), LangCode.English);
			string localizedString = Localization.GetLocalizedString("expo.options.currentLanguageName", Localization.DefaultSection, lang);
			list.Add(localizedString);
		}
		textSwitchOptions_language = list.ToArray();
		List<string> list2 = new List<string>();
		foreach (SystemLanguage availableVoiceLanguage in RDString.AvailableVoiceLanguages)
		{
			string item = RDString.Get("enum.SystemLanguage." + availableVoiceLanguage);
			list2.Add(item);
		}
		textSwitchOptions_voiceLanguage = list2.ToArray();
		mOnePlayerButton = GameObject.Find("OnePlayerButton");
		mTwoPlayersButton = GameObject.Find("TwoPlayersButton");
		defaultInstructions = introMenu.tip.text;
		NotSelectedColor = Color.black.WithAlpha(0.7f);
		SelectedColor = Color.clear;
		int num3 = 0;
		while (true)
		{
			GameObject gameObject = GameObject.Find("Level1P_" + num3);
			if (gameObject == null)
			{
				break;
			}
			levelObjects1p.Add(gameObject);
			num3++;
		}
		num3 = 0;
		while (true)
		{
			GameObject gameObject2 = GameObject.Find("Level2P_" + num3);
			if (gameObject2 == null)
			{
				break;
			}
			levelObjects2p.Add(gameObject2);
			num3++;
		}
		foreach (GameObject item2 in levelObjects1p.Concat(levelObjects2p))
		{
			scrChar component = item2.GetComponent<scrChar>();
			component.usesOwnShaderData = true;
			component.shaderData.outline = 1;
			component.shaderData.borderColor = Color.black;
		}
		int num4 = PassedLevelsFromList(levels1p);
		int num5 = PerfectedLevelsFromList(levels1p);
		int num6 = PerfectedLevelsFromList(levels2p);
		if (base.gc.booth)
		{
			levelObjects2p[5].SetActive(value: false);
			levelObjects1p[6].SetActive(value: false);
			levelObjects1p[7].SetActive(value: false);
			demoComplete.gameObject.SetActive(value: false);
		}
		else
		{
			bool flag = num4 < 4;
			if (flag)
			{
				levelObjects1p[7].SetActive(value: false);
			}
			demoComplete.SetRDString("expo.demoComplete", -1f);
			demoComplete.gameObject.SetActive(!flag);
			noteParticles.gameObject.SetActive(!flag);
			printe($"perfectedLevels1p {num5} levels1p.Length {levels1p.Length} perfectedLevels2p {num6} levels2p.Length {levels2p.Length}");
			if (num5 == levels1p.Length - 1)
			{
				demoComplete.text = RDString.Get("expo.demo100percent");
				demoComplete.color = "FB629DFF".HexToColor();
			}
			if (num5 < 2)
			{
				levelObjects2p[5].SetActive(value: false);
			}
			if (num5 < 6)
			{
				levelObjects1p[6].SetActive(value: false);
			}
		}
		RDShaderProperties shaderData = GameObject.Find("Level1P_4").GetComponent<Entity>().shaderData;
		shaderData.outline = 0;
		shaderData.additiveColor = new Color(27f / 255f, 19f / 255f, 1f, 116f / 255f);
		shaderData.borderColor = new Color(146f / 255f, 206f / 255f, 1f, 202f / 255f);
		mOnePlayerLevelDescription = GameObject.Find("LevelDescription1P").GetComponent<TextMesh>();
		mOnePlayerLevelDescription_secret = GameObject.Find("LevelDescription1P_secret").GetComponent<TextMesh>();
		mTwoPlayerLevelDescription = GameObject.Find("LevelDescription2P").GetComponent<TextMesh>();
		float num7 = (scnBase.isDivekick ? 60f : (-80f));
		mOnePlayerLevelDescription.transform.LocalMoveY(num7);
		mOnePlayerLevelDescription_secret.transform.LocalMoveY(num7);
		mTwoPlayerLevelDescription.transform.LocalMoveY(num7);
		RDBase.Vfx.FadeIn();
		string text = scnBase.lastSceneName;
		if (text == "scnSleevePaint" || text == "scnIanDesktop")
		{
			GameObject scene = (GC.twoPlayerMode ? twoPlayerScene : onePlayerScene);
			TransitionInto(scene, instant: true);
			logo.playSounds = false;
		}
		background.transform.localPosition = new Vector3(RDBase.Vfx.RDWidth / 2f, RDBase.Vfx.RDHeight / 2f, 300f);
		background.transform.localScale = new Vector3(0f - RDBase.Vfx.RDWidth, RDBase.Vfx.RDHeight, 1f);
		wrldCamera.backgroundColor = Color.black;
		base.conductor.SetBPM(82f);
		base.conductor.crotchetsPerBar = 4;
		string song = "sndIntro2";
		base.conductor.PlaySong(song, RDUtils.GetMixerGroup("StoryLevelSelect"));
		if (scnBase.isDivekick)
		{
			oldInstructions.gameObject.SetActive(value: false);
			playerSelect_divekick.SetActive(value: true);
			onePlayer_divekick.SetActive(value: true);
			twoPlayer_divekick.SetActive(value: true);
		}
		UpdatePlayerScheme();
		GetRankDescriptionForLevel(6, twoPlayer: false);
		string key = "expo." + description1p[6];
		mOnePlayerLevelDescription_secret.text = RDString.Get(key);
		textSwitch.optionSelected = (int option) =>
		{
			if (!(currentScene != introScene) && !isTransitioning)
			{
				GC.twoPlayerMode = false;
				if (textSwitchSelection == TextSwitchSelection.MainSelection)
				{
					switch (option)
					{
					case 0:
						TransitionInto(playersScene);
						break;
					case 1:
						scnBase.GoToLevelEditor();
						break;
					case 2:
						scnBase.GoToCustomLevelSelect();
						break;
					case 3:
						SelectedFullscreenOption();
						break;
					case 4:
					{
						TextSwitch obj3 = introMenu.textSwitch;
						obj3.currentOption = RDString.AvailableLanguages.IndexOf(RDString.language);
						obj3.options = textSwitchOptions_language;
						obj3.UpdateText();
						textSwitchSelection = TextSwitchSelection.Language;
						break;
					}
					case 5:
					{
						TextSwitch obj2 = introMenu.textSwitch;
						obj2.currentOption = RDString.AvailableVoiceLanguages.IndexOf(RDString.voiceLanguage);
						obj2.options = textSwitchOptions_voiceLanguage;
						obj2.UpdateText();
						textSwitchSelection = TextSwitchSelection.VoiceLanguage;
						break;
					}
					case 6:
					{
						TransitionInto(creditsScene);
						CreditsScroller[] array = creditsPages;
						for (int i = 0; i < array.Length; i++)
						{
							array[i].gameObject.SetActive(value: false);
						}
						currentCreditPageIndex = 0;
						creditsPages[currentCreditPageIndex].gameObject.SetActive(value: true);
						break;
					}
					case 7:
						SelectedStartAfreshOption();
						break;
					case 8:
						GoToFullGame();
						break;
					default:
						printe("quit");
						Application.Quit();
						break;
					}
				}
				else if (textSwitchSelection == TextSwitchSelection.Language)
				{
					printe("SwitchLanguageAndRestart: " + option);
					SwitchLanguageAndRestart(RDString.AvailableLanguages[option]);
				}
				else if (textSwitchSelection == TextSwitchSelection.VoiceLanguage)
				{
					printe("SwitchVOICELanguageAndRestart: " + option + " language: " + RDString.AvailableVoiceLanguages[option]);
					SwitchVoiceLanguageAndRestart(RDString.AvailableVoiceLanguages[option]);
				}
			}
		};
		initialLogoPosition = logo.GetComponent<RectTransform>().anchoredPosition;
	}

	private void PlayEdegaDialogue()
	{
		isTransitioning = true;
		Timer.Add(() =>
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object> { ["username"] = GetUsername() };
			base.ink.Run("diaExpoLevelSelect", "Secret", () =>
			{
				isTransitioning = false;
			}, dictionary);
		}, 1f);
		static string GetUsername()
		{
			return RDRichPresence.UserName ?? Environment.UserName;
		}
	}

	private void SelectedFullscreenOption()
	{
		bool flag = !Screen.fullScreen;
		scrVfxControl.SetFullscreen(flag);
		introMenu.tip.text = RDString.Get("pauseMenu.Fullscreen") + RDString.colon + RDString.Get(flag ? "editor.on" : "editor.off");
		string withCheck = RDString.GetWithCheck("expo.switchWindowSize", out var exists);
		printe($"windows {withCheck} exists {exists}");
		if (exists && !flag)
		{
			TextMesh tip = introMenu.tip;
			tip.text = tip.text + "\n" + withCheck;
		}
	}

	private void SelectedStartAfreshOption()
	{
		if (!startAfreshSelected)
		{
			introMenu.tip.text = RDString.Get("expo.options.startAfreshWarning");
			startAfreshSelected = true;
		}
		else
		{
			Persistence.DeleteSavedData();
			scnBase.GoToLevelSelect();
		}
	}

	private void GoToFullGame()
	{
		RDC.booth = false;
		int currentSlotIndex = Persistence.currentSlotIndex;
		Persistence.currentSlotIndex = 2;
		Persistence.ToggleLevelsState(unlock: true);
		Persistence.SetShowStartupWarnings(enabled: false);
		Persistence.SaveAll();
		Persistence.currentSlotIndex = currentSlotIndex;
		scnBase.GoToScene("scnMenu");
	}

	public static int PassedLevelsFromList(string[] list)
	{
		bool twoPlayer = false;
		int num = 0;
		for (int i = 0; i < list.Length; i++)
		{
			Rank levelRank = Persistence.GetLevelRank(list[i], twoPlayer);
			if (scnBase.isBooth || levelRank.passed)
			{
				num++;
			}
		}
		return num;
	}

	private int PerfectedLevelsFromList(string[] list)
	{
		bool twoPlayer = false;
		int num = 0;
		for (int i = 0; i < list.Length; i++)
		{
			Rank levelRank = Persistence.GetLevelRank(list[i], twoPlayer);
			if (scnBase.isBooth || levelRank.perfected)
			{
				num++;
			}
		}
		return num;
	}

	private void UpdateSpotlight()
	{
		if (currentScene == onePlayerScene)
		{
			brightMode = false;
		}
		else if (currentScene == twoPlayerScene)
		{
			brightMode = true;
		}
		if (spotlightShouldFocus != null)
		{
			float num = spotlightShouldFocus.position.x;
			if ((currentScene == onePlayerScene && levelSelected1p == 1) || (currentScene == twoPlayerScene && levelSelected2p == 1))
			{
				num += 15f;
			}
			RDBase.Vfx.SpotlightFocusIn(num, animated: false, brightMode, playSound: false, 1f, spotlightAlpha, RDUtils.GetMixerGroup("StoryLevelSelect"));
			RDBase.Vfx.SetSpotlightY(-63f + onePlayerScene.transform.position.y);
		}
	}

	private void UpdatePlayerScheme()
	{
		PlayerPrefs.GetInt("PlayerSwap", 0);
		List<RDInputType> p = RDInput.p2;
		List<RDInputType> p2 = RDInput.p1;
		RDInput.p1 = p;
		RDInput.p2 = p2;
	}

	private new void Update()
	{
		base.Update();
		if (Application.isEditor || RDBase.debugSettings.Debug)
		{
			if (Input.GetKeyDown(KeyCode.Alpha1))
			{
				printe("incomplete levels!");
				Persistence.DeleteRankLevelData();
				scnBase.GoToScene("scnExpoMenu");
			}
			else if (Input.GetKeyDown(KeyCode.Alpha2))
			{
				printe("completed levels!");
				for (int i = 0; i < 4; i++)
				{
					if (levels1p[i] != "sleevePaint")
					{
						Persistence.SetLevelRank(levels1p[i], 4, force: true);
					}
				}
				scnBase.GoToScene("scnExpoMenu");
			}
			else if (Input.GetKeyDown(KeyCode.Alpha3))
			{
				printe("perfected levels!");
				for (int j = 0; j < levels1p.Length; j++)
				{
					if (levels1p[j] != "sleevePaint" && levels1p[j] != "rhythmWeightlifter")
					{
						Persistence.SetLevelRank(levels1p[j], 5, force: true);
					}
				}
				scnBase.GoToScene("scnExpoMenu");
			}
			else if (Input.GetKeyDown(KeyCode.Alpha4))
			{
				printe("play edega dialogue!");
				PlayEdegaDialogue();
			}
			else if (Input.GetKeyDown(KeyCode.Alpha5))
			{
				scnGame.loadDogMode = true;
				GoToLevelTrigger("Lesmis");
			}
		}
		introMenu.textSwitch.enabled = currentScene == introScene;
		string customDialogueFolder = Persistence.GetCustomDialogueFolder();
		string text = (string.IsNullOrEmpty(customDialogueFolder) ? "using internal dialogues" : customDialogueFolder);
		setDialogueFolderLabel.text = "Set Dialogues Folder: \n" + text;
		if (RDBase.debugSettings.Debug)
		{
			if (Input.GetKey(KeyCode.Tab))
			{
				expoSetttings.gameObject.SetActive(value: true);
				string format = "Keys:\n\n- FlipXJoyCon (ctrl-j): {0}\n- PlayerSwap (ctrl-k) = {1}\n- p1.index = {2}\n- p2.index = {3}";
				debugText.text = string.Format(format, PlayerPrefs.GetInt("JoyconXFlip", 1).ToString(), PlayerPrefs.GetInt("PlayerSwap", 0) == 1, RDInput.p1[0].schemeIndex, RDInput.p2[0].schemeIndex);
				switchFlag.text = "Switch: " + (Persistence.GetSwitchFlag() ? "ON" : "OFF");
				customButtonFlag.text = "CustomButton: " + (Persistence.GetCustomButtonFlag() ? "ON" : "OFF");
				return;
			}
			expoSetttings.gameObject.SetActive(value: false);
			if (RDEditorUtils.CheckForKeyCombo(control: true, shift: true, KeyCode.M))
			{
				base.gm.soft = true;
				AudioListener.volume = 0.05f;
			}
			else if (RDEditorUtils.CheckForKeyCombo(control: true, shift: true, KeyCode.E))
			{
				printe("going to editor");
				scnBase.GoToLevelEditor();
			}
			else if (RDEditorUtils.CheckForKeyCombo(control: true, shift: false, KeyCode.J))
			{
				PlayerPrefs.SetInt("JoyconXFlip", PlayerPrefs.GetInt("JoyconXFlip", 1) * -1);
				printe("flipped to: " + PlayerPrefs.GetInt("JoyconXFlip", 1));
			}
			else if (RDEditorUtils.CheckForKeyCombo(control: true, shift: false, KeyCode.K))
			{
				PlayerPrefs.SetInt("PlayerSwap", 1 - PlayerPrefs.GetInt("PlayerSwap", 0));
				UpdatePlayerScheme();
			}
		}
		UpdateSpotlight();
		if (!scnBase.isDivekick)
		{
			tk2dSprite component = mOnePlayerButton.GetComponent<tk2dSprite>();
			tk2dSprite component2 = mTwoPlayersButton.GetComponent<tk2dSprite>();
			if (mPlayersModeSelected == 0)
			{
				component.overlayColor = SelectedColor;
				component2.overlayColor = NotSelectedColor;
			}
			else
			{
				component.overlayColor = NotSelectedColor;
				component2.overlayColor = SelectedColor;
			}
		}
		if (isTransitioning)
		{
			return;
		}
		if (!scnBase.isDivekick)
		{
			if (RDInput.anyPlayerPress)
			{
				PlayHammerSound();
				if (currentScene == playersScene)
				{
					GameObject scene = ((mPlayersModeSelected == 0) ? onePlayerScene : twoPlayerScene);
					TransitionInto(scene);
				}
			}
		}
		else if (currentScene == introScene)
		{
			if (RDInput.anyPlayerPress)
			{
				PlayHammerSound();
				TransitionInto(playersScene);
			}
		}
		else if (currentScene == playersScene)
		{
			if (RDInput.p1Press)
			{
				PlayHammerSound();
				TransitionInto(onePlayerScene);
			}
			else if (RDInput.p2Press)
			{
				PlayHammerSound();
				TransitionInto(twoPlayerScene);
			}
		}
		if (RDInput.cancelPress)
		{
			if (currentScene == introScene)
			{
				if (textSwitchSelection != TextSwitchSelection.MainSelection)
				{
					TextSwitch obj = introMenu.textSwitch;
					obj.options = textSwitchOptions_original;
					obj.currentOption = ((textSwitchSelection == TextSwitchSelection.Language) ? 4 : 5);
					obj.UpdateText();
					textSwitchSelection = TextSwitchSelection.MainSelection;
					PlayHammerSound();
				}
			}
			else if (currentScene == playersScene || currentScene == creditsScene)
			{
				TransitionInto(introScene);
			}
			else
			{
				TransitionInto(playersScene);
			}
		}
		scrVfxControl.CheckKeyComboForResolutionChange();
		if (isTransitioning)
		{
			return;
		}
		if (currentScene == creditsScene)
		{
			if (creditsPages[currentCreditPageIndex].finished)
			{
				if (currentCreditPageIndex < creditsPages.Length - 1)
				{
					currentCreditPageIndex++;
					creditsPages[currentCreditPageIndex].gameObject.SetActive(value: true);
				}
				else
				{
					currentCreditPageIndex = 0;
					TransitionInto(introScene);
				}
			}
		}
		else if (currentScene == playersScene)
		{
			if (!scnBase.isDivekick && (RDInput.leftPress || RDInput.rightPress))
			{
				mPlayersModeSelected = (mPlayersModeSelected + 1) % 2;
				scrConductor.PlayImmediately("sndPagerCursorMove", 1f, RDUtils.GetMixerGroup("StoryLevelSelect"));
			}
		}
		else
		{
			if (!(currentScene == onePlayerScene) && !(currentScene == twoPlayerScene))
			{
				return;
			}
			if (scnBase.isDivekick)
			{
				if (RDInput.p1Press)
				{
					p1DownTimestamp = Time.timeSinceLevelLoad;
				}
				if (RDInput.p2Press)
				{
					p2DownTimestamp = Time.timeSinceLevelLoad;
				}
				if ((RDInput.p1IsPressed && Time.timeSinceLevelLoad - p1DownTimestamp > 0.4f) || (RDInput.p2IsPressed && Time.timeSinceLevelLoad - p2DownTimestamp > 0.4f))
				{
					FakeTransitionInto(playersScene);
				}
				if ((RDInput.p1IsPressed && Time.timeSinceLevelLoad - p1DownTimestamp > 1.3f) || (RDInput.p2IsPressed && Time.timeSinceLevelLoad - p2DownTimestamp > 1.3f))
				{
					TransitionInto(playersScene);
					ResetDivekickTimestamps();
				}
				if (!isTransitioning && (RDInput.p1Release || RDInput.p2Release) && semiBackTween != null)
				{
					TransitionInto(currentScene);
					ResetDivekickTimestamps();
				}
			}
			float num = Mathf.Abs(p1DownTimestamp - p2DownTimestamp);
			if ((RDInput.anyPlayerPress && !scnBase.isDivekick) || (scnBase.isDivekick && num < 0.04f))
			{
				string text2 = "";
				if (currentScene == onePlayerScene)
				{
					text2 = levels1p[levelSelected1p];
				}
				else if (currentScene == twoPlayerScene)
				{
					text2 = levels2p[levelSelected2p];
				}
				if (!isTransitioning && !string.IsNullOrEmpty(text2))
				{
					GC.twoPlayerMode = currentScene == twoPlayerScene;
					GoToLevelTrigger(text2);
				}
				ResetDivekickTimestamps();
			}
			float delay = (scnBase.isDivekick ? 0.04f : 0f);
			if (RDInput.leftPress || (scnBase.isDivekick && RDInput.p1Press))
			{
				DOVirtual.DelayedCall(delay, () =>
				{
					scrConductor.PlayImmediately("sndMenuSquelch", 1f, RDUtils.GetMixerGroup("StoryLevelSelect"));
					LeftOrRightWasPressed(-1);
				});
			}
			else if (RDInput.rightPress || (scnBase.isDivekick && RDInput.p2Press))
			{
				DOVirtual.DelayedCall(delay, () =>
				{
					scrConductor.PlayImmediately("sndMenuSquelch", 1f, RDUtils.GetMixerGroup("StoryLevelSelect"));
					LeftOrRightWasPressed(1);
				});
			}
		}
	}

	private void LateUpdate()
	{
		if ((currentScene == onePlayerScene || currentScene == twoPlayerScene) && (prevOnePlayerLevel != levelSelected1p || prevTwoPlayerLevel != levelSelected2p))
		{
			prevOnePlayerLevel = levelSelected1p;
			prevTwoPlayerLevel = levelSelected2p;
			GetRankDescriptionForLevel(levelSelected1p, twoPlayer: false);
			GetRankDescriptionForLevel(levelSelected2p, twoPlayer: true);
			if (levelSelected1p != 6)
			{
				string key = "expo." + description1p[levelSelected1p];
				mOnePlayerLevelDescription.text = RDString.Get(key);
			}
			string key2 = "expo." + description2p[levelSelected2p];
			mTwoPlayerLevelDescription.text = RDString.Get(key2);
			for (int i = 0; i < levelObjects1p.Count; i++)
			{
				levelObjects1p[i].GetComponent<scrChar>().shaderData.overlayColor = ((i == levelSelected1p) ? SelectedColor : NotSelectedColor);
			}
			for (int j = 0; j < levelObjects2p.Count; j++)
			{
				levelObjects2p[j].GetComponent<scrChar>().shaderData.overlayColor = ((j == levelSelected2p) ? SelectedColor : NotSelectedColor);
			}
			string text = levelObjects1p[levelSelected1p].name;
			string text2 = levelObjects2p[levelSelected2p].name;
			foreach (WalkingSelectableCharacter walkingCharacter in walkingCharacters)
			{
				bool flag = walkingCharacter.currentDestination.id == text || walkingCharacter.currentDestination.id == text2;
				walkingCharacter.GetComponent<scrChar>().shaderData.overlayColor = (flag ? SelectedColor : NotSelectedColor);
			}
			if (currentScene == onePlayerScene && !isTransitioning)
			{
				float num = ((levelSelected1p == 6) ? 820f : (0f - onePlayerScene.transform.localPosition.x));
				Vector3 endValue = new Vector3(num, 0f - onePlayerScene.transform.localPosition.y, 300f);
				scenes.transform.DOKill();
				scenes.transform.DOLocalMove(endValue, secretLevelTransition.duration).SetEase(secretLevelTransition.ease);
			}
		}
		logo.GetComponent<RectTransform>().anchoredPosition = new Vector2(scenes.transform.localPosition.x + initialLogoPosition.x, scenes.transform.localPosition.y + initialLogoPosition.y);
		float timeSinceLevelLoad = Time.timeSinceLevelLoad;
		if (RDInput.anyKeyDown)
		{
			if (currentScene != introScene)
			{
				FadeAudioListenerVolume(1f, 0.3f);
			}
			anyKeyTimestamp = timeSinceLevelLoad;
		}
		if (timeSinceLevelLoad - anyKeyTimestamp > 5f)
		{
			FadeAudioListenerVolume(0.3f);
		}
	}

	private void FadeAudioListenerVolume(float value, float duration = 1f)
	{
		if (volumeFade != null)
		{
			volumeFade.Kill();
			volumeFade = null;
		}
		volumeFade = DOTween.To(() => AudioListener.volume, (float v) =>
		{
			AudioListener.volume = v;
		}, value, duration);
	}

	private void ResetDivekickTimestamps()
	{
		p1DownTimestamp = -50f;
		p2DownTimestamp = -100f;
	}

	private void LeftOrRightWasPressed(int increment)
	{
		if (currentScene == onePlayerScene)
		{
			do
			{
				levelSelected1p = (levelSelected1p + levelObjects1p.Count + increment) % levelObjects1p.Count;
			}
			while (!levelObjects1p[levelSelected1p].activeSelf);
			spotlightShouldFocus = levelObjects1p[levelSelected1p].transform;
		}
		else if (currentScene == twoPlayerScene)
		{
			do
			{
				levelSelected2p = (levelSelected2p + levelObjects2p.Count + increment) % levelObjects2p.Count;
			}
			while (!levelObjects2p[levelSelected2p].activeSelf);
			spotlightShouldFocus = levelObjects2p[levelSelected2p].transform;
		}
		string text = ((currentScene == onePlayerScene) ? levelObjects1p[levelSelected1p].name : levelObjects1p[levelSelected2p].name);
		foreach (WalkingSelectableCharacter walkingCharacter in walkingCharacters)
		{
			WalkingSelectableCharacter.Destination destination = null;
			bool flag = false;
			foreach (WalkingSelectableCharacter.Destination dayDestination in walkingCharacter.dayDestinations)
			{
				SelectableCharacter selectableCharacter = dayDestination.destEnt as SelectableCharacter;
				selectableCharacter.animation.visible = true;
				if (dayDestination.preferred)
				{
					destination = dayDestination;
				}
				if (dayDestination.id == text)
				{
					selectableCharacter.gamePosition = selectableCharacter.gameObject.transform.position;
					walkingCharacter.MoveCharacter(dayDestination, instant: false);
					flag = true;
				}
			}
			if (!flag && destination != null)
			{
				SelectableCharacter obj = destination.destEnt as SelectableCharacter;
				obj.gamePosition = obj.gameObject.transform.position;
				walkingCharacter.MoveCharacter(destination, instant: false);
			}
		}
	}

	private string GetRankDescriptionForLevel(int level, bool twoPlayer)
	{
		string[] array = (twoPlayer ? levels2p : levels1p);
		if (array[level] == "sleevePaint" || array[level] == "rhythmWeightlifter")
		{
			return "";
		}
		Rank rank = Persistence.GetLevelRank(array[level]);
		if (scnBase.isBooth)
		{
			rank = 5;
		}
		string text = (((int)rank == 5) ? "perfect" : (rank.passed ? "completed" : "incomplete"));
		return RDString.Get("levelSelect." + text);
	}

	public void GoToLevelTrigger(string level)
	{
		StartCoroutine(GoToLevelRoutine(level));
	}

	private IEnumerator GoToLevelRoutine(string level)
	{
		if (level == "OrientalInsomniac")
		{
			if (!GC.twoPlayerMode)
			{
				level = "Intro";
			}
		}
		else if (level == "Boss2")
		{
			level = "Tutorial_Boss2";
		}
		if (level != "InsomniacHard")
		{
			Level_InsomniacHard.hasGlitched = false;
		}
		isTransitioning = true;
		scrConductor.PlayImmediately("sndMenuSelect", 1f, RDUtils.GetMixerGroup("StoryLevelSelect"));
		RDBase.Vfx.Flash();
		yield return new WaitForSeconds(0.5f);
		RDBase.Vfx.FadeOut(1f);
		yield return new WaitForSeconds(2f);
		if (level == "sleevePaint")
		{
			scnBase.GoToSleevePaint();
		}
		else if (level == "rhythmWeightlifter")
		{
			scnBase.GoToRhythmWeightlifter();
		}
		else
		{
			scnBase.GoToLevel(level, loadGameScene: true, attemptToLoadTutorial: true);
		}
	}

	private void FakeTransitionInto(GameObject scene)
	{
		if (semiBackTween == null)
		{
			Vector3 endValue = Vector3.Lerp(b: new Vector3(0f - scene.transform.localPosition.x, 0f - scene.transform.localPosition.y, 300f), a: new Vector3(0f - currentScene.transform.localPosition.x, 0f - currentScene.transform.localPosition.y, 300f), t: 0.2f);
			semiBackTween = scenes.transform.DOLocalMove(endValue, 1f, snapping: true).SetEase(Ease.OutQuad);
		}
	}

	private void TransitionInto(GameObject scene, bool instant = false)
	{
		if (isTransitioning)
		{
			return;
		}
		float num = (instant ? 0f : 1f);
		semiBackTween.Kill();
		semiBackTween = null;
		Vector3 endValue = new Vector3(0f - scene.transform.localPosition.x, 0f - scene.transform.localPosition.y, 300f);
		GameObject gameObject = currentScene;
		currentScene = scene;
		scenes.transform.DOKill();
		scenes.transform.DOLocalMove(endValue, sceneTransition.duration * num).SetEase(sceneTransition.ease);
		float delay = sceneTransition.duration * num * ratioControlsBlocked;
		Timer.Add(() =>
		{
			isTransitioning = false;
		}, delay);
		isTransitioning = true;
		if (scene != introScene)
		{
			background.DOKill();
			background.DOColor(Color.black, 0.5f * num);
			FadeAudioListenerVolume(1f, 0.3f);
		}
		if (scene == introScene)
		{
			background.DOKill();
			background.DOColor(Color.clear, 0.5f * num);
			FadeAudioListenerVolume(0.3f, 0.3f);
		}
		else if (scene == onePlayerScene)
		{
			levelSelected1p = 0;
			spotlightShouldFocus = levelObjects1p[levelSelected1p].transform;
			DOTween.To(() => spotlightAlpha, (float x) =>
			{
				spotlightAlpha = x;
			}, 0.5f, 0.5f * num);
		}
		else if (scene == twoPlayerScene)
		{
			levelSelected2p = 0;
			spotlightShouldFocus = levelObjects2p[levelSelected2p].transform;
			DOTween.To(() => spotlightAlpha, (float x) =>
			{
				spotlightAlpha = x;
			}, 1f, 0.5f * num);
		}
		if (!(scene == onePlayerScene) && !(scene == twoPlayerScene))
		{
			DOTween.To(() => spotlightAlpha, (float x) =>
			{
				spotlightAlpha = x;
			}, 0f, 0.5f * num);
		}
		if (!(scene == introScene) || !(gameObject == creditsScene))
		{
			return;
		}
		DOVirtual.DelayedCall(0.5f * num, () =>
		{
			CreditsScroller[] array = creditsPages;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].gameObject.SetActive(value: false);
			}
		});
	}

	private new void OnGUI()
	{
		base.OnGUI();
		if (RDBase.debugSettings.Debug)
		{
			GUI.Label(new Rect(0f, 0f, 200f, 200f), "screen.fullscreen: " + Screen.fullScreen + "\nscreen.fullscreenMode: " + Screen.fullScreenMode.ToString() + "\n");
		}
	}

	public override void OnPreBar()
	{
		base.OnPreBar();
		if ((base.conductor.barNumber - 1) % 17 == 0 && base.conductor.barNumber > 3)
		{
			base.conductor.PlaySong("sndIntro2", RDUtils.GetMixerGroup("StoryLevelSelect"));
		}
	}

	public override void OnBeat()
	{
		base.OnBeat();
		if (scnBase.isDivekick)
		{
			GameObject[] array = alternateActiveEachBeat;
			foreach (GameObject obj in array)
			{
				obj.SetActive(!obj.activeSelf);
			}
		}
	}

	public void TraditionalChineseButton()
	{
		SwitchLanguageAndRestart(SystemLanguage.ChineseTraditional);
	}

	public void SimplifiedChineseButton()
	{
		SwitchLanguageAndRestart(SystemLanguage.ChineseSimplified);
	}

	public void EnglishButton()
	{
		SwitchLanguageAndRestart(SystemLanguage.English);
	}

	public void SpanishButton()
	{
		SwitchLanguageAndRestart(SystemLanguage.Spanish);
	}

	public void KoreanButton()
	{
		SwitchLanguageAndRestart(SystemLanguage.Korean);
	}

	public void JapaneseButton()
	{
		SwitchLanguageAndRestart(SystemLanguage.Japanese);
	}

	private void SwitchLanguageAndRestart(SystemLanguage language)
	{
		scnBase.GoToExpoLevelSelect();
		RDString.ChangeLanguage(language);
	}

	private void SwitchVoiceLanguageAndRestart(SystemLanguage language)
	{
		scnBase.GoToExpoLevelSelect();
		RDString.ChangeVoiceLanguage(language);
	}

	public void UpdateStrings()
	{
		RDString.FetchLocalization(RDString.LocalizationSheet.Strings);
		scnBase.GoToExpoLevelSelect();
	}

	public void SetCustomDialogueFolder()
	{
		string text = null;
		if (!string.IsNullOrEmpty(text))
		{
			string text2 = Uri.UnescapeDataString(text.Replace("file:", ""));
			printe("folderPath: " + text2);
			Persistence.SetCustomDialogueFolder(text2);
		}
	}

	public void CancelCustomDialogueFolder()
	{
		Persistence.SetCustomDialogueFolder("");
	}

	public void SwitchModeSwitch()
	{
		Persistence.SetSwitchFlag(!Persistence.GetSwitchFlag());
		scnBase.GoToExpoLevelSelect();
	}

	public void SwitchModeCustomFlag()
	{
		Persistence.SetCustomButtonFlag(!Persistence.GetCustomButtonFlag());
		RDInput.Reinitialize();
		scnBase.GoToExpoLevelSelect();
	}

	public void DeleteSavedData()
	{
		Persistence.DeleteSavedData();
		scnBase.GoToExpoLevelSelect();
	}
}
