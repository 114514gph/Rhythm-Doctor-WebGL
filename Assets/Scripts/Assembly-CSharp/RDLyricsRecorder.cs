using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RDLyricsRecorder : scnBase
{
	private const string TimesStartTag = "-Times-";

	public InputField times;

	public Dropdown filesList;

	public InputField allLyrics;

	public InputField offset;

	public TextAsset lyrics;

	public Text textDisplay;

	public Text timesDisplay;

	public AudioSource music;

	public Text pauseButtonText;

	public Text seconds;

	public Slider timeSlider;

	public Slider scrubbingSlider;

	public bool multiLineMode = true;

	public string tempRecordTimes = "-Times-";

	private List<string> textSnapshots = new List<string>();

	private List<double> timeSnapshots = new List<double>();

	private int caretPosition = -1;

	private string[] currentFiles = new string[23]
	{
		"Chinese/lesmis_lyrics_interlude_doctor", "Chinese/lesmis_lyrics_interlude_patient1", "Chinese/lesmis_lyrics_verse_doctor", "Chinese/lesmis_lyrics_verse_patient1", "Chinese/lesmis_lyrics_verse_patient2", "Chinese/lesmis_lyrics_verse_politician", "Chinese/lesmis_lyrics_outro_doctor1", "Chinese/lesmis_lyrics_outro_doctor2", "lesmis_lyrics_interlude_doctor", "lesmis_lyrics_interlude_patient1",
		"lesmis_lyrics_verse_doctor", "lesmis_lyrics_verse_patient1", "lesmis_lyrics_verse_patient2", "lesmis_lyrics_verse_politician", "lesmis_lyrics_outro_doctor1", "lesmis_lyrics_outro_doctor2", "lesmis_dog_lyrics_interlude_doctor", "lesmis_dog_lyrics_interlude_patient1", "lesmis_dog_lyrics_verse2_politician", "lesmis_dog_lyrics_verse_doctor",
		"lesmis_dog_lyrics_verse_patient1", "lesmis_dog_lyrics_verse_patient2", "lesmis_dog_lyrics_verse_politician"
	};

	private const string ln = "\n";

	private const string lnWindows = "\r\n";

	private new void Start()
	{
		base.Start();
		filesList.ClearOptions();
		filesList.AddOptions(new List<string>(currentFiles));
		timeSlider.onValueChanged.AddListener(delegate
		{
			SliderValueChanged();
		});
		timeSlider.minValue = 0f;
		timeSlider.maxValue = music.clip.length;
		timeSlider.value = 0f;
		LoadLyricsFile(currentFiles[0]);
		UpdateLyrics(currentFiles[0]);
		filesList.onValueChanged.AddListener((int arg0) =>
		{
			UpdateLyrics(currentFiles[filesList.value]);
		});
		allLyrics.onValueChanged.AddListener((string arg0) =>
		{
			printe("onValueChanged: " + arg0);
			LoadLyrics(allLyrics.text);
		});
	}

	private new void Update()
	{
		base.Update();
		if (RDInput.anyPlayerPress)
		{
			tempRecordTimes = tempRecordTimes + "\n" + music.time;
		}
		int num = 0;
		foreach (double timeSnapshot in timeSnapshots)
		{
			if (timeSnapshot + (double)float.Parse(offset.text) < (double)music.time)
			{
				textDisplay.text = textSnapshots[num] + "  <color=yellow>(" + timeSnapshot.ToString("0.####") + ")</color>";
			}
			num++;
		}
		if (!Input.GetMouseButton(0))
		{
			timeSlider.value = music.time;
		}
		seconds.text = (Mathf.Floor(music.time * 10000f) / 10000f).ToString("0.####");
		times.text = tempRecordTimes;
		if (caretPosition != allLyrics.caretPosition)
		{
			printe("update caret pos");
			caretPosition = allLyrics.caretPosition;
		}
	}

	private void UpdateLyrics(string filename)
	{
		allLyrics.text = LoadLyricsAsset(filename);
	}

	private void LoadLyricsFile(string filename)
	{
		string lyricsText = LoadLyricsAsset(filename);
		LoadLyrics(lyricsText);
	}

	private string LoadLyricsAsset(string filename)
	{
		return Resources.Load<TextAsset>("Lyrics/" + filename).text;
	}

	private void LoadLyrics(string lyricsText)
	{
		Debug.Log("lyricsText: " + lyricsText);
		textSnapshots = new List<string>();
		timeSnapshots = new List<double>();
		lyricsText = lyricsText.Replace("\r\n", "\n");
		lyricsText = lyricsText.Replace("\n\n", "\n");
		string[] array = lyricsText.Split(new string[1] { "-Times-\n" }, StringSplitOptions.None);
		if (array.Length != 2)
		{
			Debug.LogError("Error parsing lyrics file");
		}
		string[] array2 = array[0].Split(new string[1] { "\n" }, StringSplitOptions.None);
		for (int i = 0; i < array2.Length; i++)
		{
			string[] array3 = array2[i].Split(new char[1] { '/' }, StringSplitOptions.None);
			string text = "";
			string[] array4 = array3;
			foreach (string text2 in array4)
			{
				text += text2;
				textSnapshots.Add(text);
			}
		}
		array2 = array[1].Split(new string[1] { "\n" }, StringSplitOptions.None);
		foreach (string text3 in array2)
		{
			if (!string.IsNullOrEmpty(text3.Trim()))
			{
				timeSnapshots.Add(double.Parse(text3));
			}
		}
	}

	public void SliderValueChanged()
	{
		if (Input.GetMouseButton(0))
		{
			music.time = timeSlider.value;
		}
	}

	public void PlayPause()
	{
		if (music.isPlaying)
		{
			music.Pause();
			pauseButtonText.text = "Play";
		}
		else
		{
			music.UnPause();
			pauseButtonText.text = "Pause";
		}
	}
}
