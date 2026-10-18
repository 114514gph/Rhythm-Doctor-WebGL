#if !UNITY_WEBGL
using System;
using System.Collections.Generic;
using System.Linq;
using SA.GoogleDoc;
using UnityEngine;
using UnityEngine.UI;

public class StoryDebugScreen : MonoBehaviour
{
	private class StoryStep
	{
		public readonly string name;

		public readonly Action<bool> action;

		public readonly Color color;

		public StoryStep(string name, Action<bool> action, Color? color = null)
		{
			this.action = action;
			this.name = name;
			this.color = color ?? actColors[0];
		}

		public static StoryStep Passed(int act, Level level)
		{
			return new StoryStep("Passed " + GC.levelMetadata[level].id, (bool endHere) =>
			{
				int num = ((GC.levelMetadata[level].type == LevelType.Intermission) ? 15 : 3);
				Persistence.SetLevelRank(level, num, force: true);
				Persistence.SetLastPlayedLevel(level);
				Persistence.SetPlayedPassedLevelCutscene(played: false);
			}, actColors[act]);
		}
	}

	public static bool expanded;

	public Button expandButton;

	public Dropdown storyDropdown;

	public Toggle paigeStays;

	public Toggle instantDialog;

	public Toggle recordingMode;

	public GameObject screenContainer;

	public GameObject buttonsContainer;

	public Button fetchLocButton;

	public Button fetchInkButton;

	public Dropdown locDropdown;

	public Dropdown inkDropdown;

	public InputField passwordInput;

	private static StoryStep[] storySteps;

	private static readonly Color DefaultColor = "CACACAFF".HexToColor();

	private static readonly Color[] actColors = new Color[8]
	{
		DefaultColor,
		"90C7E7FF".HexToColor(),
		"E08F8FFF".HexToColor(),
		"DDC582FF".HexToColor(),
		"AC7454FF".HexToColor(),
		"42DA6FFF".HexToColor(),
		"D98FE0FF".HexToColor(),
		"b80f0fff".HexToColor()
	};

	private void Start()
	{
		storySteps = new StoryStep[48]
		{
			new StoryStep("[Intro]", (bool endHere) =>
			{
			}),
			new StoryStep("Passed Intro", (bool endHere) =>
			{
				Persistence.SetFirstTimePlaying(state: false);
				Persistence.SetDidCalibration(didCalibration: true);
				Persistence.SetLevelRankInternal(Level.OrientalTechno, -1);
			}),
			StoryStep.Passed(1, Level.OrientalTechno),
			StoryStep.Passed(1, Level.Intimate),
			new StoryStep("[Watched Intimate passed cutscene]", (bool endHere) =>
			{
				Persistence.SetPlayedPassedLevelCutscene(played: true);
				Persistence.SetLastPlayedLevel(Level.OrientalInsomniac);
				Persistence.SetLevelRankInternal(Level.OrientalInsomniac, -1);
			}),
			StoryStep.Passed(1, Level.OrientalInsomniac),
			StoryStep.Passed(2, Level.Lofi),
			StoryStep.Passed(2, Level.SVT),
			StoryStep.Passed(2, Level.Smokin),
			StoryStep.Passed(2, Level.SongOfTheSea),
			StoryStep.Passed(2, Level.Boss2),
			new StoryStep("[Watched Post-Act 2 cutscene]", (bool endHere) =>
			{
				Persistence.SetPlayedPostAct2Cutscene(played: true);
				Persistence.SetLevelRank(Level.BeansHopper, -1);
			}),
			StoryStep.Passed(3, Level.Garden),
			StoryStep.Passed(3, Level.Classy),
			StoryStep.Passed(3, Level.DistantDuet),
			StoryStep.Passed(3, Level.Lesmis),
			new StoryStep("[Watched Post-Act 3 cutscene]", (bool endHere) =>
			{
				Persistence.SetPlayedPostAct3Cutscene(played: true);
			}),
			StoryStep.Passed(4, Level.Heldbeats),
			StoryStep.Passed(4, Level.Invisible),
			StoryStep.Passed(4, Level.Steinway),
			StoryStep.Passed(4, Level.KnowYou),
			StoryStep.Passed(4, Level.InsomniacHard),
			new StoryStep("[Watched Post-Act 4 cutscene]", (bool endHere) =>
			{
				Persistence.SetPlayedPostAct4Cutscene(played: true);
			}),
			new StoryStep("[Watched Pre-Act 5 cutscene]", (bool endHere) =>
			{
				Persistence.SetPlayedPreAct5Cutscene(played: true);
				Persistence.SetLevelRankInternal(Level.LuckyBreak, -1);
			}),
			new StoryStep("A Rank on 2-1", (bool endHere) =>
			{
				Persistence.SetLastPlayedLevel(Level.Lofi);
				Persistence.SetLevelRankInternal(Level.Lofi, 14);
				Persistence.SetLevelRankInternal(Level.CareLess, -1);
				Persistence.SetGameResult(GameResult.LevelAlreadyWasPassed);
			}, actColors[2]),
			StoryStep.Passed(2, Level.CareLess),
			StoryStep.Passed(5, Level.LuckyBreak),
			StoryStep.Passed(5, Level.Freezeshot),
			StoryStep.Passed(5, Level.AthleteTherapy),
			StoryStep.Passed(5, Level.AthleteFinale),
			new StoryStep("[Watched Pre-Act 6 cutscene]", (bool endHere) =>
			{
				Persistence.SetPlayedPreAct6Cutscene(played: true);
			}),
			new StoryStep("[Watched Act 6 intro]", (bool endHere) =>
			{
				Persistence.SetPlayedAct6Intro(played: true);
			}),
			new StoryStep("[Watched HaileyDuet introduction]", (bool endHere) =>
			{
				Persistence.SetPlayedHaileyDuetIntroduction(played: true);
			}),
			StoryStep.Passed(6, Level.HaileyDuet),
			StoryStep.Passed(6, Level.EdegaRave),
			StoryStep.Passed(6, Level.PaigesReckoning),
			new StoryStep("[Watched Airport cutscene]", (bool endHere) =>
			{
				Persistence.SetPlayedPassedLevelCutscene(played: true);
			}),
			StoryStep.Passed(7, Level.Blurred),
			new StoryStep("[Watched pre-Biterness cutscene]", (bool endHere) =>
			{
				Persistence.SetPlayedPreBitternessCutscene(played: true);
			}),
			StoryStep.Passed(7, Level.Bitterness),
			new StoryStep("[Watched Rooftop cutscene]", (bool endHere) =>
			{
				Persistence.SetPlayedRooftopCutscene(played: true);
			}),
			StoryStep.Passed(7, Level.Montage),
			StoryStep.Passed(7, Level.Montage2),
			new StoryStep("[Watched Edega cutscene]", (bool endHere) =>
			{
				Persistence.SetPlayedPassedLevelCutscene(played: true);
			}),
			StoryStep.Passed(0, Level.HelpingHands),
			StoryStep.Passed(0, Level.ArtExercise),
			new StoryStep("Passed all Night shifts", (bool endHere) =>
			{
				Persistence.SetLastPlayedLevel(Level.OrientalTechno);
				Persistence.SetPlayedPassedLevelCutscene(played: true);
				Persistence.SetGameResult(GameResult.LevelAlreadyWasPassed);
				Level[] values = RDUtils.GetValues<Level>();
				for (int i = 0; i < values.Length; i++)
				{
					Persistence.SetLevelRankInternal(values[i], 4);
				}
			}, Color.white),
			new StoryStep("100%", (bool endHere) =>
			{
				Level[] values = RDUtils.GetValues<Level>();
				for (int i = 0; i < values.Length; i++)
				{
					Persistence.SetLevelRankInternal(values[i], 15);
				}
			}, Color.yellow)
		};
		storyDropdown.ClearOptions();
		List<string> list = new List<string>();
		StoryStep[] array = storySteps;
		foreach (StoryStep storyStep in array)
		{
			list.Add(storyStep.name);
		}
		storyDropdown.AddOptions(list);
		storyDropdown.onValueChanged.AddListener((int x) =>
		{
			FastForwardToStoryStep(x);
		});
		paigeStays.isOn = Persistence.GetPaigeEnding();
		paigeStays.onValueChanged.AddListener((bool x) =>
		{
			Persistence.SetPaigeEnding(x);
			Persistence.SaveCurrentSlot();
		});
		instantDialog.isOn = DebugSettings.instance.InstantDialogue;
		instantDialog.onValueChanged.AddListener((bool x) =>
		{
			DebugSettings.instance.InstantDialogue = x;
			Persistence.SetInstantDialogue(x);
		});
		recordingMode.isOn = DebugSettings.instance.RecordingMode;
		recordingMode.onValueChanged.AddListener((bool x) =>
		{
			DebugSettings.instance.RecordingMode = x;
		});
		expandButton.onClick.AddListener(() =>
		{
			SetExpanded(!expanded);
		});
		SetExpanded(expanded);
		List<string> list2 = new List<string> { "All" };
		list2.AddRange(Settings.Instance.GetDocByName("RDLocalization").GetWorksheetNames().ToList());
		List<string> list3 = new List<string> { "All" };
		list3.AddRange(Settings.Instance.GetDocByName("InkLocalization").GetWorksheetNames().ToList());
		locDropdown.ClearOptions();
		locDropdown.AddOptions(list2);
		inkDropdown.ClearOptions();
		inkDropdown.AddOptions(list3);
		fetchLocButton.onClick.AddListener(() =>
		{
			RDString.FetchLocalization(RDString.LocalizationSheet.Strings, locDropdown.value - 1);
			scnBase.GoToLevelSelect();
		});
		fetchInkButton.onClick.AddListener(() =>
		{
			RDString.FetchLocalization(RDString.LocalizationSheet.Dialogues, inkDropdown.value - 1);
			scnBase.GoToLevelSelect();
		});
		passwordInput.text = Persistence.GetLocalizationPassword();
		passwordInput.onEndEdit.AddListener(Persistence.SetLocalizationPassword);
	}

	public void FastForwardToStoryStep(int target)
	{
		Persistence.ToggleLevelsState(unlock: false);
		Persistence.SetFirstTimePlaying(state: true);
		Persistence.SetDidCalibration(didCalibration: false);
		for (int i = 0; i <= target; i++)
		{
			storySteps[i].action(i == target);
		}
		Persistence.SetPaigeEnding(paigeStays.isOn);
		Persistence.SaveCurrentSlot();
		scnBase.GoToLevelSelect();
	}

	private void Update()
	{
		bool debug = DebugSettings.instance.Debug;
		if (debug || scnBase.isBooth)
		{
			if (Input.GetKeyDown(KeyCode.Alpha1))
			{
				SetCurrentLevelRank(-2);
			}
			if (Input.GetKeyDown(KeyCode.Alpha2))
			{
				SetCurrentLevelRank(-1);
			}
			if (Input.GetKeyDown(KeyCode.Alpha3))
			{
				SetCurrentLevelRank(4);
			}
			if (Input.GetKeyDown(KeyCode.Alpha4))
			{
				SetCurrentLevelRank(15);
			}
		}
		if (screenContainer.activeSelf != debug)
		{
			screenContainer.SetActive(debug);
		}
	}

	private void SetExpanded(bool on)
	{
		expanded = on;
		expandButton.GetComponentInChildren<Text>().text = (on ? "Hide" : "Show") + " Story Debug Menu";
		buttonsContainer.gameObject.SetActive(on);
	}

	public void SetCurrentLevelRank(string rank)
	{
		SetCurrentLevelRank(Rank.FromString(rank));
	}

	private void SetCurrentLevelRank(Rank rank)
	{
		scnLevelSelect instance = scnLevelSelect.instance;
		if (instance.selectedEntity is SelectableCharacter selectableCharacter)
		{
			Level num = selectableCharacter.levels[instance.currentDifficulty];
			Rank levelRank = Persistence.GetLevelRank(num);
			if (levelRank.passed)
			{
				Persistence.SetGameResult(GameResult.LevelAlreadyWasPassed);
			}
			Persistence.SetPlayedPassedLevelCutscene(levelRank.passed);
			Persistence.SetLevelRank(num, rank, force: true);
			Persistence.SetLastPlayedLevel(num);
			scnBase.GoToLevelSelect();
		}
	}

	public void OnDropdownOpened()
	{
		Image[] array = (from go in storyDropdown.GetComponentsInChildren<Image>()
			where go.name == "Item Background"
			select go).ToArray();
		for (int num = 0; num < array.Length; num++)
		{
			StoryStep storyStep = storySteps[num];
			Image image = array[num];
			image.color = storyStep.color;
			if (storyStep.name.StartsWith('['))
			{
				image.GetComponentInParent<Toggle>().interactable = false;
			}
		}
	}

	public void OnPasswordSelect()
	{
		scnLevelSelect.instance.controlsAreLocked = true;
	}

	public void OnPasswordDeselect()
	{
		scnLevelSelect.instance.controlsAreLocked = false;
	}
}

#endif
