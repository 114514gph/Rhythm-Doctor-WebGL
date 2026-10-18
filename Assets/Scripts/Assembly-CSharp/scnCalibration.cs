using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class scnCalibration : scnBase
{
	public enum CalibrationPage
	{
		None = 0,
		Intro = 1,
		PreferenceSelection = 2,
		Visual = 3,
		Input = 4
	}

	private const int crotchetsPerBar = 8;

	public static string nextSceneName = string.Empty;

	[Header("Debug")]
	public bool renderOnGUI;

	public int lastAudioBeepRewind;

	public int lastVisualBeepRewind;

	[Header("General")]
	public CalibrationPage firstPage;

	public RectTransform windowFrame;

	public GameObject[] allPages;

	public AudioSource[] audioSources;

	public double beepDuration = 0.16766666666666666;

	public AudioSource lowBeep;

	public AudioSource highBeep;

	public Text logOffText;

	public Button logOffButton;

	[Header("Preference Selection")]
	public GameObject intro;

	public Image bracketsSlam;

	public Image bracketsTap;

	public Sprite[] bracketFrames;

	public Image buttonTap;

	public Image buttonSlam;

	[Header("Visual Offset")]
	public GameObject visualCalibration;

	public Text visualDescription;

	public Image[] visualBeats;

	public Text currentVisualOffset;

	public Image pointer;

	public Image visualLeftButton;

	public Image visualRightButton;

	public Button visualNextButton;

	[Header("Visual Congrats")]
	public GameObject visualCongrats;

	public Text amount;

	[Header("InputOffset")]
	public GameObject inputCalibration;

	public Text inputDescription;

	public Image[] inputBeats;

	public Image border7th;

	public Text averageInputOffset;

	public Text[] beatNumbers;

	public Text beatNumber7;

	public Text hit;

	public Text lastInputOffset;

	public Image[] completionSquares;

	public Sprite squareBorderSprite;

	public GameObject inputTest;

	public GameObject inputResults;

	public Text inputResultsDescription;

	public SpriteAnimation inputOttoAnimation;

	public Sprite[] superOtto;

	public Sprite[] happyOtto;

	public Sprite[] sadOtto;

	public Button inputNextButton;

	private bool nextClicked;

	private bool tapSelected;

	private bool preferenceSelected;

	private double time;

	private double visualTime;

	private double loopDuration;

	private bool playingBeeps;

	private bool[] scheduledAudioSources;

	private bool visualKnotHasFinished;

	private double initialTime;

	private double visualOffBeep;

	private int visualBeep;

	private int audioBeep;

	private bool inkGreenBeeps;

	private bool inkPinkBeeps;

	private int lastVisualBeep = -100;

	private int lastAudioBeep = -100;

	private const int InputTapsNeeded = 10;

	private int closest7thBeatIndex;

	private bool inputKnotHasFinished;

	private float inputOffset;

	private float average;

	private double lastInputTapTime;

	private List<float> hitOffset = new List<float>();

	private bool consistent;

	private Rank rank;

	private const int MaxTries = 2;

	private int tries;

	private bool exitScreenCalled;

	private bool shouldNarrate;

	[NonSerialized]
	public CalibrationPage page;

	private AudioSource desktopAmbience;

	private float timeSpaceBlocked;

	public new static scnCalibration instance => scnBase._instance as scnCalibration;

	public Image[] beats
	{
		get
		{
			if (page != CalibrationPage.Visual)
			{
				return inputBeats;
			}
			return visualBeats;
		}
	}

	public double bpm
	{
		get
		{
			if (page != CalibrationPage.Visual)
			{
				return 240.0;
			}
			return 120.0;
		}
	}

	public double beatDuration => 60.0 / bpm;

	private double dspTime => AudioSettings.dspTime;

	private new void Awake()
	{
		base.Awake();
		scnBase._instance = this;
		base.ink.Load("diaCalibration");
	}

	private new void Start()
	{
		base.Start();
		initialTime = dspTime;
		scheduledAudioSources = new bool[audioSources.Length];
		base.conductor.SetBPM(120f);
		desktopAmbience = base.conductor.PlaySong("sndDesktopAmbienceStartup", RDUtils.GetMixerGroup("AmbienceParent"), 1f, 0f, useBeatsNotBar: true);
		ChangeToPage(firstPage);
	}

	private void RewindBeeps()
	{
		StopBeeps();
		playingBeeps = true;
		lastAudioBeep = lastAudioBeepRewind;
		lastVisualBeep = lastVisualBeepRewind;
		initialTime = dspTime - (double)Math.Min(lastAudioBeepRewind, lastVisualBeepRewind) * beatDuration;
	}

	private void StopBeeps()
	{
		playingBeeps = false;
		AudioSource[] array = audioSources;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Stop();
		}
	}

	private void ChangeToPage(CalibrationPage page)
	{
		this.page = page;
		switch (page)
		{
		case CalibrationPage.Intro:
			RunInkDialogue("Intro", () =>
			{
				if (scnBase.isXbox || scnBase.isSwitch)
				{
					preferenceSelected = true;
					tapSelected = true;
					ChangeToPage(CalibrationPage.Visual);
				}
				else
				{
					ChangeToPage(CalibrationPage.PreferenceSelection);
				}
			});
			windowFrame.gameObject.SetActive(value: false);
			break;
		case CalibrationPage.PreferenceSelection:
		{
			playingBeeps = false;
			preferenceSelected = false;
			bracketsTap.enabled = false;
			bracketsSlam.enabled = false;
			buttonTap.gameObject.SetActive(value: true);
			buttonSlam.gameObject.SetActive(value: true);
			Image image = buttonTap;
			Color color = (buttonSlam.color = base.gc.desktop_green);
			image.color = color;
			BlockSpace(0.25f);
			shouldNarrate = true;
			Narration.Set(string.Concat(RDString.Get("narration.calibrationAssistant") + RDString.Get("narration.longPause") + RDString.Get("calibration.preference.description"), RDString.Get("narration.longPause"), RDString.Get("calibration.preference.confirm")));
			Narration.SetInstructions(RDString.Get("calibration.preference.description"), NarrationActionName.ToConfirm, NarrationActionName.ToExit);
			break;
		}
		case CalibrationPage.Visual:
			RewindBeeps();
			visualDescription.enabled = false;
			visualKnotHasFinished = false;
			DOVirtual.DelayedCall(0.5f, () =>
			{
				RunInkDialogue("Visual", () =>
				{
					BlockSpace(1f);
					visualDescription.enabled = true;
					visualKnotHasFinished = true;
					shouldNarrate = true;
					Narration.Set(RDString.Get("narration.visualOffset").Replace("[visualTitle]", RDString.Get("calibration.visual.title")).Replace("[visualDescription]", RDString.Get("calibration.visual.description")));
				});
			});
			break;
		case CalibrationPage.Input:
		{
			Text[] array = beatNumbers;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].color = base.gc.desktop_green;
			}
			RewindBeeps();
			VisualBeepsOff();
			hitOffset.Clear();
			consistent = false;
			inputTest.SetActive(value: true);
			inputResults.SetActive(value: false);
			lastInputOffset.text = "";
			inputDescription.enabled = true;
			if (tries == 0)
			{
				inputKnotHasFinished = false;
				DOVirtual.DelayedCall(0.5f, () =>
				{
					RunInkDialogue("Input", () =>
					{
						BlockSpace(0.5f);
						inputDescription.enabled = true;
						inputKnotHasFinished = true;
						Narration.Say(RDString.Get("narration.inputOffset").Replace("[inputTitle]", RDString.Get("calibration.input.title")).Replace("[inputDescription]", RDString.Get("calibration.input.description")), NarrationCategory.Notification, false);
						Narration.SetInstructions(RDString.Get("calibration.input.description"), NarrationActionName.ToExit);
					});
				});
			}
			else
			{
				inputKnotHasFinished = true;
			}
			tries++;
			break;
		}
		}
	}

	private void ShowPage(GameObject page)
	{
		GameObject[] array = allPages;
		foreach (GameObject gameObject in array)
		{
			if (gameObject == page)
			{
				if (!gameObject.activeSelf)
				{
					gameObject.SetActive(value: true);
				}
			}
			else
			{
				gameObject.SetActive(value: false);
			}
		}
	}

	private void LateUpdate()
	{
		scnBase.CheckShowPersistentDataPath();
		if (RDInput.cancelPress)
		{
			ExitScreen();
		}
		if (Input.GetKeyDown(KeyCode.R))
		{
			RewindBeeps();
		}
		if (timeSpaceBlocked > 0f)
		{
			timeSpaceBlocked = Mathf.Max(0f, timeSpaceBlocked - Time.unscaledDeltaTime);
		}
		if (page == CalibrationPage.PreferenceSelection)
		{
			ShowPage(intro);
		}
		else if (page == CalibrationPage.Visual)
		{
			ShowPage(visualCalibration);
		}
		else if (page == CalibrationPage.Input)
		{
			ShowPage(inputCalibration);
		}
		windowFrame.gameObject.SetActive(page != CalibrationPage.Intro);
		if (page == CalibrationPage.PreferenceSelection && !base.ink.isPlaying && !preferenceSelected)
		{
			if (RDInput.downPress || RDInput.upPress)
			{
				tapSelected = !tapSelected;
				shouldNarrate = true;
				RDBase.PlaySound("sndDesktopBeepD4_6", 1f, RDUtils.GetMixerGroup("IanDesktop"));
			}
			if (bracketsTap.enabled != tapSelected)
			{
				bracketsTap.enabled = tapSelected;
			}
			if (bracketsSlam.enabled != !tapSelected)
			{
				bracketsSlam.enabled = !tapSelected;
			}
			if (AnyPressed())
			{
				preferenceSelected = true;
				RDBase.PlaySound("sndDesktopBeepD5_6", 1f, RDUtils.GetMixerGroup("IanDesktop"));
				string knotToPlay = (tapSelected ? "TapSelected" : "SlamSelected");
				Image obj = (tapSelected ? buttonTap : buttonSlam);
				if (!tapSelected)
				{
					_ = buttonTap;
				}
				else
				{
					_ = buttonSlam;
				}
				Image bracketsSelected = (tapSelected ? bracketsTap : bracketsSlam);
				obj.color = base.gc.desktop_yellow;
				bracketsSelected.enabled = false;
				DOVirtual.DelayedCall(1f / 60f * 4f, () =>
				{
					bracketsSelected.enabled = true;
				});
				DOVirtual.DelayedCall(1f / 60f * 8f, () =>
				{
					bracketsSelected.enabled = false;
				});
				DOVirtual.DelayedCall(1f / 60f * 12f, () =>
				{
					RunInkDialogue(knotToPlay, () =>
					{
						RunInkDialogue("TapSelectedEnd", () =>
						{
							ChangeToPage(CalibrationPage.Visual);
						});
					});
				});
			}
			if (shouldNarrate)
			{
				shouldNarrate = false;
				Narration.Say(RDString.Get("calibration.preference." + (tapSelected ? "tap" : "slam")), NarrationCategory.Navigation, false);
			}
		}
		if (playingBeeps)
		{
			time = dspTime - initialTime;
			visualTime = time - (double)RDCalibration.calibration_v;
			loopDuration = 8.0 * beatDuration;
			visualBeep = (int)(visualTime / beatDuration);
			if (lastVisualBeep < visualBeep)
			{
				lastVisualBeep = visualBeep;
				int index = visualBeep % 8;
				VisualBeepOn(index);
				visualOffBeep = visualTime + beepDuration;
			}
			if (page == CalibrationPage.Visual && visualTime > visualOffBeep)
			{
				VisualBeepsOff();
				visualOffBeep = 3.4028234663852886E+38;
			}
			audioBeep = (int)Math.Floor(time / beatDuration);
			closest7thBeatIndex = (int)Math.Floor(((double)audioBeep + 5.0) / 8.0) * 8 - 2;
			double num = beatDuration * (double)closest7thBeatIndex;
			inputOffset = (float)(time - num);
			if (lastAudioBeep < audioBeep)
			{
				lastAudioBeep = audioBeep;
				int num2 = audioBeep + 2;
				double num3 = beatDuration * (double)num2;
				int num4 = num2 % 8;
				if (num4 < audioSources.Length)
				{
					AudioSource audioSource = audioSources[num4];
					if (page == CalibrationPage.Visual)
					{
						float lifetimePercentage = (float)num4 * 1f / (float)(beats.Length - 1);
						audioSource.volume = DOVirtual.EasedValue(0.2f, 0.6f, lifetimePercentage, Ease.InExpo);
					}
					else
					{
						audioSource.volume = 0.6f;
					}
					if (num4 == 6)
					{
						_ = page == CalibrationPage.Input;
					}
					else
						_ = 0;
					audioSource.PlayScheduled(initialTime + num3);
				}
			}
		}
		if (page == CalibrationPage.Visual)
		{
			if (!base.ink.isPlaying)
			{
				if (RDInput.leftPress)
				{
					shouldNarrate = true;
					if (RDInput.finerControlIsPressed)
					{
						RDCalibration.calibration_v -= 0.001f;
					}
					else
					{
						RDCalibration.calibration_v -= 0.01f;
					}
				}
				else if (RDInput.rightPress)
				{
					shouldNarrate = true;
					if (RDInput.finerControlIsPressed)
					{
						RDCalibration.calibration_v += 0.001f;
					}
					else
					{
						RDCalibration.calibration_v += 0.01f;
					}
				}
				if (RDInput.leftIsPressed)
				{
					visualLeftButton.color = base.gc.desktop_yellow;
				}
				if (RDInput.leftRelease)
				{
					visualLeftButton.color = base.gc.desktop_green;
				}
				if (RDInput.rightIsPressed)
				{
					visualRightButton.color = base.gc.desktop_yellow;
				}
				if (RDInput.rightRelease)
				{
					visualRightButton.color = base.gc.desktop_green;
				}
			}
			inkGreenBeeps = base.ink.currentKnot == "Visual";
			inkPinkBeeps = base.ink.currentKnot == "Input";
			int num5 = Mathf.Clamp(Mathf.RoundToInt(RDCalibration.calibration_v * 1000f / 3.33333f), -90, 90);
			pointer.rectTransform.AnchorPosX(num5);
			int num6 = Mathf.RoundToInt(RDCalibration.calibration_v * 1000f);
			currentVisualOffset.text = RDString.Get("calibration.visual.current").Replace("[offset]", num6.ToString());
			if (!RDInput.finerControlIsPressed && AnyPressed() && !base.ink.isPlaying && visualKnotHasFinished)
			{
				RDCalibration.Save();
				RunInkDialogue("VisualEnd", () =>
				{
					ChangeToPage(CalibrationPage.Input);
				});
			}
			if (shouldNarrate)
			{
				shouldNarrate = false;
				Narration.Say(RDString.Get("narration.offsetMilliseconds").Replace("[offset]", num6.ToString()), NarrationCategory.Navigation, false, NarrationActionName.ToMakeBlinksEarlier, NarrationActionName.ToMakeBlinksLater, NarrationActionName.ToConfirm, NarrationActionName.ToExit);
			}
		}
		else if (page == CalibrationPage.Input)
		{
			if (inputTest.activeSelf)
			{
				if (AnyPressed() && !base.ink.isPlaying && inputKnotHasFinished)
				{
					shouldNarrate = true;
					border7th.color = base.gc.desktop_violet;
					lastInputTapTime = Time.timeSinceLevelLoad;
					lastInputOffset.text = inputOffset.ToMilliseconds();
					hitOffset.Add(inputOffset);
					average = hitOffset.Average();
					if (hitOffset.Count >= 10)
					{
						BlockSpace(0.5f);
						StopBeeps();
						inputResults.SetActive(value: true);
						inputTest.SetActive(value: false);
						double num7 = RDUtils.StandardDeviation(hitOffset);
						List<float> list = hitOffset.Where((float offset) => (double)Mathf.Abs(offset - average) < 0.2).ToList();
						if (list.Count > 6)
						{
							double num8 = RDUtils.StandardDeviation(list);
							consistent = num8 < 0.1;
						}
						else
						{
							consistent = false;
						}
						if (consistent)
						{
							StopBeeps();
							rank = 1;
							if (num7 < 0.01)
							{
								rank = 5;
							}
							else if (num7 < 0.015)
							{
								rank = 4;
							}
							else if (num7 < 0.02)
							{
								rank = 3;
							}
							else if (num7 < 0.025)
							{
								rank = 2;
							}
							else
							{
								rank = 1;
							}
							inputResultsDescription.text = RDString.Get("calibration.input.results");
							Text text = inputResultsDescription;
							text.text = text.text + " " + RDString.Get("calibration.input.rank").Replace("[rank]", rank.ToString());
							Text text2 = inputResultsDescription;
							text2.text = text2.text + "\n" + RDString.Get("calibration.input.current").Replace("[offset]", Mathf.RoundToInt(average * 1000f).ToString());
							Text text3 = inputResultsDescription;
							text3.text = text3.text + "\n" + RDString.Get("calibration.input.continue");
							if ((int)rank == 5)
							{
								scrConductor.PlayImmediately("sndDesktopJingleJackpot", 1f, RDUtils.GetMixerGroup("IanDesktop"));
								DOVirtual.DelayedCall(0.25f, () =>
								{
									scrConductor.PlayImmediately("sndDesktopJinglePositive", 1f, RDUtils.GetMixerGroup("IanDesktop"));
								});
							}
							else if ((int)rank == 4)
							{
								scrConductor.PlayImmediately("sndDesktopJinglePositive", 1f, RDUtils.GetMixerGroup("IanDesktop"));
							}
							else
							{
								scrConductor.PlayImmediately("sndDesktopJingleNeutral", 1f, RDUtils.GetMixerGroup("IanDesktop"));
							}
							RDCalibration.calibration_i = average;
							RDCalibration.calibration_i_P2_internal = average;
							RDCalibration.Save();
							inputOttoAnimation.currentAnimationData.sprites = (((int)rank == 5) ? superOtto : happyOtto);
						}
						else
						{
							inputResultsDescription.text = RDString.Get("calibration.input.inconsistent");
							if (tries < 2)
							{
								Text text4 = inputResultsDescription;
								text4.text = text4.text + "\n" + RDString.Get("calibration.input.hint");
								Text text5 = inputResultsDescription;
								text5.text = text5.text + "\n" + RDString.Get("calibration.input.tryAgain");
							}
							else
							{
								Text text6 = inputResultsDescription;
								text6.text = text6.text + "\n" + RDString.Get("calibration.input.defaults");
								Text text7 = inputResultsDescription;
								text7.text = text7.text + "\n" + RDString.Get("calibration.input.continue");
							}
							inputOttoAnimation.currentAnimationData.sprites = sadOtto;
							scrConductor.PlayImmediately("sndDesktopJingleNegative", 1f, RDUtils.GetMixerGroup("IanDesktop"));
						}
					}
				}
				int num9 = 0;
				Image[] array = completionSquares;
				for (int num10 = 0; num10 < array.Length; num10++)
				{
					array[num10].sprite = ((num9 < hitOffset.Count) ? null : squareBorderSprite);
					num9++;
				}
				if ((double)Time.timeSinceLevelLoad - lastInputTapTime > 0.10000000149011612)
				{
					border7th.color = base.gc.desktop_green;
				}
				averageInputOffset.enabled = false;
				if (shouldNarrate)
				{
					shouldNarrate = false;
					if (hitOffset.Count >= 10)
					{
						Narration.Say(inputResultsDescription.text, NarrationCategory.Notification, false);
					}
					else
					{
						Narration.Say(RDString.Get("narration.offsetMilliseconds").Replace("[offset]", Mathf.RoundToInt(inputOffset * 1000f).ToString()), NarrationCategory.Notification, false);
					}
				}
			}
			else if (AnyPressed() && !base.ink.isPlaying)
			{
				if (consistent)
				{
					if ((int)rank == 5)
					{
						RunInkDialogue("ConsistentS", () =>
						{
							ExitScreen();
						});
					}
					else
					{
						RunInkDialogue("Consistent", () =>
						{
							ExitScreen();
						});
					}
				}
				else
				{
					RunInkDialogue("Inconsistent" + ((tries > 1) ? 3 : tries), () =>
					{
						if (tries == 2)
						{
							ExitScreen();
						}
						else
						{
							BlockSpace(0.5f);
							ChangeToPage(CalibrationPage.Input);
						}
					});
				}
			}
		}
		nextClicked = false;
	}

	public void LogOffButton()
	{
		ExitScreen();
	}

	private void ExitScreen()
	{
		if (!exitScreenCalled)
		{
			exitScreenCalled = true;
			desktopAmbience.DOFade(0f, 0.5f);
			if (!Persistence.GetDidCalibration())
			{
				Persistence.SetDidCalibration(didCalibration: true);
			}
			string s = nextSceneName;
			nextSceneName = string.Empty;
			if (s.IsNullOrEmpty())
			{
				scnBase.GoToLevel("Intro");
			}
			else
			{
				scnBase.GoToScene(s);
			}
		}
	}

	private bool AnyPressed()
	{
		if (!(timeSpaceBlocked <= 0f))
		{
			return false;
		}
		if (!RDInput.anyPlayerPress)
		{
			return nextClicked;
		}
		return true;
	}

	private void BlockSpace(float seconds)
	{
		timeSpaceBlocked = seconds;
	}

	private new void OnGUI()
	{
		if (!renderOnGUI)
		{
			return;
		}
		base.OnGUI();
		string text = "";
		int num = 0;
		text += $"time\t{time}\nvTime\t{visualTime}\nloopDuration {loopDuration}\naudioBeep {audioBeep}\n7thBeat {closest7thBeatIndex}\ninputOffset {inputOffset}\nInkPlaying {base.ink.isPlaying}\nplayingBeeps {playingBeeps}\n";
		AudioSource[] array = audioSources;
		foreach (AudioSource audioSource in array)
		{
			if (audioSource.isPlaying)
			{
				text = text + audioSource.name + num + " / ";
			}
			num++;
		}
		GUI.Label(new Rect(0f, 0f, 600f, 200f), text);
	}

	private void VisualBeepsOff()
	{
		Image[] array = beats;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enabled = false;
		}
		InkGreenBeep(on: false);
		InkPinkBeep(on: false);
	}

	private void VisualBeepOn(int index)
	{
		VisualBeepsOff();
		if (index >= 0 && index < beats.Length)
		{
			beats[index].enabled = true;
			beatNumbers[index].color = base.gc.desktop_purple;
		}
		if (page == CalibrationPage.Input)
		{
			if (index > 0)
			{
				beatNumbers[index - 1].color = base.gc.desktop_green;
			}
			else
			{
				beatNumbers[beats.Length - 1].color = base.gc.desktop_green;
			}
		}
		if (index != 7)
		{
			InkPinkBeep(on: true);
		}
	}

	private void Show(bool show)
	{
		windowFrame.gameObject.SetActive(show);
	}

	private void InkPinkBeep(bool on)
	{
		if (!inkPinkBeeps)
		{
			return;
		}
		string text = "color=#C786FA";
		string text2 = "color=#928DF7";
		if (base.ink.dialogueText.Contains(text2))
		{
			if (on)
			{
				base.ink.dialogueText = base.ink.dialogueText.Replace(text2, text);
			}
		}
		else if (!on)
		{
			base.ink.dialogueText = base.ink.dialogueText.Replace(text, text2);
		}
	}

	private void InkGreenBeep(bool on)
	{
		if (!inkGreenBeeps)
		{
			return;
		}
		string text = "color=#A8F9BF";
		string text2 = "color=#928DF7";
		if (base.ink.dialogueText.Contains(text2))
		{
			if (on)
			{
				base.ink.dialogueText = base.ink.dialogueText.Replace(text2, text);
			}
		}
		else if (!on)
		{
			base.ink.dialogueText = base.ink.dialogueText.Replace(text, text2);
		}
	}

	public void Next()
	{
		nextClicked = true;
	}

	public void RunInkDialogue(string knotName, RDInk.RDInkCallback onComplete)
	{
		if (Persistence.GetDidCalibration())
		{
			onComplete();
		}
		else
		{
			base.ink.Run(knotName, onComplete);
		}
	}
}
