using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using RDLevelEditor;
using UnityEngine;
using UnityEngine.UI;

public class scnDemoLevels : scnBase
{
	public const string PlayerSwapKey = "PlayerSwap";

	public const string JoyconXFlipKey = "JoyconXFlip";

	private const string ColorStartTag = "<color=#33ccee>";

	private const string ColorEndTag = "</color>";

	private const string InsomniacHard = "InsomniacHard";

	private const string MyFutureGirlfiend = "fGirlfriend";

	private const string sleevePaintId = "sleevePaint";

	public const string ArtExerciseLevel = "ArtExercise";

	public const string Op10No4Level = "Op10No4";

	public static readonly string[] levels1p = new string[8] { "OrientalInsomniac", "Boss2", "Lesmis", "InsomniacHard", "fGirlfriend", "Op10No4", "ArtExercise", "sleevePaint" };

	private readonly string[] description1p = new string[8] { "medium", "hard", "hard", "harder", "hard", "easiest", "easy", "sleevePaint" };

	public static bool shouldPlayEdegaDialogue = false;

	[Header("GameObjects")]
	public tk2dSprite background;

	public GameObject currentScene;

	public AnimationData sceneTransition;

	public AnimationData secretLevelTransition;

	public HeartMonitor heartMonitor;

	public Image foreground;

	public TextMesh mfgDescription;

	public BarTransition barTransition;

	[Header("Variables")]
	public bool isTransitioning;

	public float ratioControlsBlocked;

	private GameObject scenes;

	private GameObject onePlayerScene;

	private int levelSelected1p;

	private List<GameObject> levelObjects1p = new List<GameObject>();

	private Color NotSelectedColor;

	private Color SelectedColor;

	private TextMesh mOnePlayerLevelDescription;

	private TextMesh mOnePlayerLevelDescription_secret;

	private float spotlightAlpha;

	private bool brightMode;

	public Transform spotlightShouldFocus;

	private int prevOnePlayerLevel = -1;

	private Tween semiBackTween;

	private int op10no4Index => Array.IndexOf(levels1p, "Op10No4");

	private int artExerciseLevel => Array.IndexOf(levels1p, "ArtExercise");

	private int mfgIndex => Array.IndexOf(levels1p, "fGirlfriend");

	private int insomniacHardIndex => Array.IndexOf(levels1p, "InsomniacHard");

	public bool isPlaying
	{
		get
		{
			if (scnBase.instance != null && scnBase.instance is scnGame)
			{
				return !scnGame.instance.paused;
			}
			return false;
		}
	}

	private new void Awake()
	{
		base.Awake();
		scnBase._instance = this;
		scnBase.currentLevelSelect = "scnDemoLevels";
		RDTime.speed = 1f;
		scnGame.levelSpeed = 1f;
	}

	private new void Start()
	{
		base.Start();
		RDRichPresence.Instance.UpdatePresence();
		scenes = GameObject.Find("Scenes");
		onePlayerScene = GameObject.Find("OnePlayer");
		wrldCamera.RDFullNativeResolution();
		onePlayerScene.transform.localPosition = new Vector3(0f, 0f, 0f);
		currentScene = onePlayerScene;
		NotSelectedColor = new Color(0.3f, 0.3f, 0.3f, 1f);
		SelectedColor = Color.white;
		int num = 0;
		while (true)
		{
			GameObject gameObject = GameObject.Find("Level1P_" + num);
			if (gameObject == null)
			{
				break;
			}
			levelObjects1p.Add(gameObject);
			num++;
		}
		int num2 = PassedLevelsFromList(levels1p);
		int num3 = PerfectedLevelsFromList(levels1p);
		bool num4 = num2 >= 4;
		_ = levelObjects1p[artExerciseLevel];
		if (!num4)
		{
			levelObjects1p[artExerciseLevel].SetActive(value: false);
		}
		if (num3 < 2)
		{
			levelObjects1p[mfgIndex].SetActive(value: false);
		}
		if (num3 < 5)
		{
			levelObjects1p[op10no4Index].SetActive(value: false);
		}
		Entity component = GameObject.Find("Level1P_3").GetComponent<Entity>();
		component.shaderDataSource = component;
		RDShaderProperties shaderData = component.shaderData;
		shaderData.additiveColor = new Color(27f / 255f, 19f / 255f, 1f, 116f / 255f);
		shaderData.borderColor = new Color(146f / 255f, 206f / 255f, 1f, 202f / 255f);
		mOnePlayerLevelDescription = GameObject.Find("LevelDescription1P").GetComponent<TextMesh>();
		mOnePlayerLevelDescription_secret = GameObject.Find("LevelDescription1P_secret").GetComponent<TextMesh>();
		float num5 = -80f;
		mOnePlayerLevelDescription.transform.LocalMoveY(num5);
		mOnePlayerLevelDescription_secret.transform.LocalMoveY(num5);
		RDBase.Vfx.FadeIn(2f);
		string text = scnBase.lastSceneName;
		bool flag = text == "scnSleevePaint" || text == "scnIanDesktop";
		background.transform.localPosition = new Vector3(RDBase.Vfx.RDWidth / 2f, RDBase.Vfx.RDHeight / 2f, 300f);
		background.transform.localScale = new Vector3(0f - RDBase.Vfx.RDWidth, RDBase.Vfx.RDHeight, 1f);
		wrldCamera.backgroundColor = Color.black;
		string song = (shouldPlayEdegaDialogue ? "sndEdegaMock2" : "sndTutorial");
		base.conductor.SetBPM(shouldPlayEdegaDialogue ? 82f : 88f);
		base.conductor.crotchetsPerBar = 4;
		base.conductor.PlaySong(song, RDUtils.GetMixerGroup("MenuMusic"));
		if (shouldPlayEdegaDialogue)
		{
			PlayEdegaDialogue();
			shouldPlayEdegaDialogue = false;
		}
		UpdatePlayerScheme();
		string rankDescriptionForLevel = GetRankDescriptionForLevel(op10no4Index);
		string key = "expo." + description1p[op10no4Index];
		mOnePlayerLevelDescription_secret.text = RDString.Get(key) + "\n<color=#33ccee>" + rankDescriptionForLevel + "</color>";
		key = "expo." + description1p[mfgIndex];
		rankDescriptionForLevel = GetRankDescriptionForLevel(mfgIndex);
		mfgDescription.text = RDString.Get(key) + "\n<color=#33ccee>" + rankDescriptionForLevel + "</color>";
		levelSelected1p = (flag ? Array.IndexOf(levels1p, "sleevePaint") : 0);
		spotlightShouldFocus = levelObjects1p[levelSelected1p].transform;
		DOTween.To(() => spotlightAlpha, (float x) =>
		{
			spotlightAlpha = x;
		}, 0.5f, 0.5f);
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
	}

	private string GetUsername()
	{
		return RDRichPresence.UserName ?? Environment.UserName;
	}

	public static int PassedLevelsFromList(string[] list)
	{
		bool twoPlayer = false;
		int num = 0;
		for (int i = 0; i < list.Length; i++)
		{
			if (Persistence.GetLevelRank(list[i], twoPlayer).passed)
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
			if (Persistence.GetLevelRank(list[i], twoPlayer).perfected)
			{
				num++;
			}
		}
		return num;
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
				scnBase.GoToScene("scnDemoLevels");
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
				scnBase.GoToScene("scnDemoLevels");
			}
			else if (Input.GetKeyDown(KeyCode.Alpha3))
			{
				printe("perfected levels!");
				for (int j = 0; j < levels1p.Length; j++)
				{
					if (levels1p[j] != "sleevePaint")
					{
						Persistence.SetLevelRank(levels1p[j], 5, force: true);
					}
				}
				scnBase.GoToScene("scnDemoLevels");
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
		if (RDBase.debugSettings.Debug)
		{
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
		brightMode = levelSelected1p == mfgIndex;
		if (spotlightShouldFocus != null)
		{
			RDBase.Vfx.SpotlightFocusIn(spotlightShouldFocus.position.x, animated: false, brightMode, playSound: false, 1f, spotlightAlpha, RDUtils.GetMixerGroup("StoryLevelSelect"));
			RDBase.Vfx.SetSpotlightY(-63f + onePlayerScene.transform.position.y);
		}
		if (isTransitioning)
		{
			return;
		}
		if (RDInput.anyPlayerPress)
		{
			PlayHammerSound();
		}
		if (RDInput.cancelPress && currentScene == onePlayerScene)
		{
			if (RDInput.cancelPress && heartMonitor.visible)
			{
				heartMonitor.Hide();
			}
			else
			{
				isTransitioning = true;
				scnMenu.doBarTransitionOnStart = true;
				barTransition.Show(() =>
				{
					scnBase.GoToMainMenu();
				});
			}
		}
		scrVfxControl.CheckKeyComboForResolutionChange();
		if (isTransitioning || !(currentScene == onePlayerScene) || heartMonitor.visible)
		{
			return;
		}
		if (RDInput.anyPlayerPress)
		{
			string text = "";
			GameObject gameObject = null;
			if (currentScene == onePlayerScene)
			{
				text = levels1p[levelSelected1p];
				gameObject = levelObjects1p[levelSelected1p];
			}
			if (text == "sleevePaint")
			{
				GoToLevelTrigger("sleevePaint");
			}
			else
			{
				SelectableCharacter selectableCharacter = new SelectableCharacter();
				if (text == "Lesmis")
				{
					selectableCharacter.charactersNormal = new Character[1] { Character.MrsStevenson };
				}
				else
				{
					selectableCharacter.charactersNormal = new Character[1] { RDUtils.ParseEnum(gameObject.GetComponent<scrChar>().charName, Character.Farmer) };
				}
				selectableCharacter.leftPager = false;
				selectableCharacter.gameObject = gameObject;
				selectableCharacter.skipCamAnimOnSelect = true;
				selectableCharacter.gamePosition.x = gameObject.transform.position.x;
				selectableCharacter.levelType = LevelType.Boss;
				selectableCharacter.id = text;
				selectableCharacter.bpmNormal = 100;
				selectableCharacter.bpmHard = 150;
				bool flag = true;
				if (text == "InsomniacHard")
				{
					selectableCharacter.id = "OrientalInsomniac";
					flag = false;
				}
				switch (text)
				{
				case "fGirlfriend":
				case "ArtExercise":
				case "Op10No4":
					selectableCharacter.levelType = LevelType.Regular;
					break;
				}
				LevelPlayMode levelPlayMode = LevelPlayMode.BothModes;
				if (text == "fGirlfriend")
				{
					levelPlayMode = LevelPlayMode.TwoPlayerOnly;
				}
				Difficulty difficulty = (flag ? Difficulty.Normal : Difficulty.Hard);
				selectableCharacter.levels = new Dictionary<Difficulty, Level>();
				Level level = LevelSelector.LevelFromString(text);
				selectableCharacter.levels.Add(difficulty, level);
				if (GC.levelMetadata.ContainsKey(level))
				{
					if (difficulty == Difficulty.Normal)
					{
						selectableCharacter.portraitsNormal = GC.levelMetadata[level].portraits;
					}
					else
					{
						selectableCharacter.portraitsHard = GC.levelMetadata[level].portraits;
					}
				}
				else if (difficulty == Difficulty.Normal)
				{
					selectableCharacter.portraitsNormal = new Character[1];
				}
				else
				{
					selectableCharacter.portraitsHard = new Character[1];
				}
				heartMonitor.Show(selectableCharacter, HeartMonitor.LevelSpeed.Normal, difficulty, levelPlayMode);
			}
		}
		float delay = 0f;
		if (RDInput.leftPress)
		{
			DOVirtual.DelayedCall(delay, () =>
			{
				scrConductor.PlayImmediately("sndMenuSquelch", 1f, RDUtils.GetMixerGroup("StoryLevelSelect"));
				LeftOrRightWasPressed(-1);
			});
		}
		else if (RDInput.rightPress)
		{
			DOVirtual.DelayedCall(delay, () =>
			{
				scrConductor.PlayImmediately("sndMenuSquelch", 1f, RDUtils.GetMixerGroup("StoryLevelSelect"));
				LeftOrRightWasPressed(1);
			});
		}
	}

	private void LateUpdate()
	{
		if (!(currentScene == onePlayerScene) || prevOnePlayerLevel == levelSelected1p)
		{
			return;
		}
		prevOnePlayerLevel = levelSelected1p;
		string rankDescriptionForLevel = GetRankDescriptionForLevel(levelSelected1p);
		if (levelSelected1p != op10no4Index)
		{
			string key = "expo." + description1p[levelSelected1p];
			mOnePlayerLevelDescription.text = RDString.Get(key) + "\n<color=#33ccee>" + rankDescriptionForLevel + "</color>";
		}
		for (int i = 0; i < levelObjects1p.Count; i++)
		{
			levelObjects1p[i].GetComponent<tk2dSprite>().color = ((i == levelSelected1p) ? SelectedColor : NotSelectedColor);
		}
		if (currentScene == onePlayerScene && !isTransitioning)
		{
			bool num = levelSelected1p == op10no4Index;
			bool flag = levelSelected1p == mfgIndex;
			float num2 = 0f - onePlayerScene.transform.localPosition.x;
			if (num)
			{
				num2 = 435f;
				printe("secret level selected");
			}
			else if (flag)
			{
				num2 = -377f;
			}
			Vector3 endValue = new Vector3(num2, 0f - onePlayerScene.transform.localPosition.y, 300f);
			scenes.transform.DOKill();
			scenes.transform.DOLocalMove(endValue, secretLevelTransition.duration).SetEase(secretLevelTransition.ease);
		}
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
	}

	private string GetRankDescriptionForLevel(int level)
	{
		string[] array = levels1p;
		if (array[level] == "sleevePaint")
		{
			return "";
		}
		Rank levelRank = Persistence.GetLevelRank(array[level]);
		string text = "";
		if ((int)levelRank == 5)
		{
			return RDString.Get("levelSelect.perfect");
		}
		if (levelRank.passed)
		{
			return RDString.Get("levelSelect.completed");
		}
		return RDString.Get("levelSelect.incomplete");
	}

	public void GoToLevelTrigger(string level)
	{
		StartCoroutine(GoToLevelRoutine(level));
	}

	private IEnumerator GoToLevelRoutine(string level)
	{
		if (level == "OrientalInsomniac")
		{
			level = ((!GC.twoPlayerMode) ? "Intro" : "Tutorial_1");
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
		foreground.gameObject.SetActive(value: true);
		foreground.color = Color.white;
		foreground.DOFade(0f, 0.45f);
		yield return new WaitForSeconds(0.5f);
		foreground.DOKill();
		foreground.color = Color.clear;
		foreground.DOFade(1f, 1f);
		yield return new WaitForSeconds(2f);
		if (level == "sleevePaint")
		{
			scnBase.GoToSleevePaint();
		}
		else
		{
			scnBase.GoToLevel(level);
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
		currentScene = scene;
		scenes.transform.DOKill();
		scenes.transform.DOLocalMove(endValue, sceneTransition.duration * num).SetEase(sceneTransition.ease);
		float delay = sceneTransition.duration * num * ratioControlsBlocked;
		Timer.Add(() =>
		{
			isTransitioning = false;
		}, delay);
		isTransitioning = true;
		if (scene == onePlayerScene)
		{
			levelSelected1p = 0;
			spotlightShouldFocus = levelObjects1p[levelSelected1p].transform;
			DOTween.To(() => spotlightAlpha, (float x) =>
			{
				spotlightAlpha = x;
			}, 0.5f, 0.5f * num);
		}
		if (!(scene == onePlayerScene))
		{
			DOTween.To(() => spotlightAlpha, (float x) =>
			{
				spotlightAlpha = x;
			}, 0f, 0.5f * num);
		}
	}

	private new void OnGUI()
	{
		base.OnGUI();
		if (RDBase.debugSettings.Debug)
		{
			GUI.Label(new Rect(0f, 0f, 200f, 200f), "screen.fullscreen: " + Screen.fullScreen + "\nscreen.fullscreenMode: " + Screen.fullScreenMode.ToString() + "\n");
		}
	}

	public override void OnBeat()
	{
		base.OnBeat();
	}
}
