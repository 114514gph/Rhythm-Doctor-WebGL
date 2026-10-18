using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class scnCalibrationOld : scnBase
{
	private const int MS = 1000;

	private const int BEEPMS = 1500;

	private const int SPACEMS = 2500;

	private const int SECONDS = 4;

	private const float NATURALOFFSET = 1.02f;

	private const float VISUALOFFSET = 0.03f;

	private AudioSource microphoneSource;

	private double startTickFeedback;

	private double spaceTickFeedback;

	private float spacePressedAt;

	private float finalInputCalibration;

	private float finalVisualCalibration;

	private float spaceAudioFeedback;

	private float beepAudioFeedback;

	private bool isCalibrationFlawed;

	private bool firstTimePressingSpace;

	private bool isTimeForText;

	private bool isStartRecordingTime;

	public AudioSource beepSource;

	public Button btnRestart;

	public Button btnLeave;

	public Text txtInputCal;

	public Text txtVisualCal;

	public Text txtDialogue;

	private new void Start()
	{
		base.Start();
		RDBase.Vfx.FadeIn();
		StartCalibration();
	}

	private new void Update()
	{
		base.Update();
		if (isTimeForText && !beepSource.isPlaying)
		{
			txtDialogue.text = "<color=red>SLAM</color> they keyboard now!";
			firstTimePressingSpace = true;
		}
		if (!isStartRecordingTime)
		{
			return;
		}
		txtDialogue.enabled = true;
		txtDialogue.text = "Press the space bar to start the calibration and wait for the beep.";
		if (Input.GetKeyDown("space"))
		{
			txtDialogue.text = "Wait...";
			isStartRecordingTime = false;
			StartRecording();
			DOVirtual.DelayedCall(4.5f, () =>
			{
				CalculateInputCalibration();
				btnRestart.gameObject.SetActive(value: true);
				btnLeave.gameObject.SetActive(value: true);
			});
		}
	}

	private new void OnGUI()
	{
		base.OnGUI();
		if (firstTimePressingSpace && Event.current.isKey && Event.current.keyCode == KeyCode.Space)
		{
			firstTimePressingSpace = false;
			spaceTickFeedback = AudioSettings.dspTime;
			spacePressedAt = (float)(spaceTickFeedback - startTickFeedback);
		}
	}

	private void StartRecording()
	{
	}

	private void CalculateInputCalibration()
	{
		float[] array = new float[microphoneSource.clip.samples * microphoneSource.clip.channels];
		microphoneSource.clip.GetData(array, 0);
		finalInputCalibration = Math.Max(ProcessSpectrum(array) - 1.02f, 0f);
		finalVisualCalibration = Math.Max(finalInputCalibration - 0.03f, 0f);
		if (!isCalibrationFlawed)
		{
			if (finalInputCalibration <= 0.03f)
			{
				txtInputCal.text = "Calibration Failed!";
				txtVisualCal.text = "Calibration Failed!";
			}
			else
			{
				txtInputCal.text = "Input Calibration: " + finalInputCalibration.ToString("F2");
				txtVisualCal.text = "Visual Calibration: " + finalVisualCalibration.ToString("F2");
			}
			Debug.Log("MISSING TO SET THE VALUES TO THE scrConductor.calibration_v and crConductor.calibration_i");
		}
		else
		{
			txtInputCal.text = "Calibration Failed!";
			txtVisualCal.text = "Calibration Failed!";
		}
	}

	private float ProcessSpectrum(float[] samples)
	{
		for (int i = 0; i < samples.Length; i++)
		{
			if (samples[i] < 0f)
			{
				samples[i] *= -1f;
			}
		}
		float[] array = new float[1500];
		float[] array2 = new float[2500];
		for (int j = 0; j < 1500; j++)
		{
			array[j] = samples[j];
		}
		for (int k = 1500; k < samples.Length; k++)
		{
			array2[k - 1500] = samples[k];
		}
		spaceAudioFeedback = FindPeak(array2) + 1.5f;
		beepAudioFeedback = FindPeak(array);
		if (beepAudioFeedback > 1.5f)
		{
			isCalibrationFlawed = true;
		}
		return beepAudioFeedback - Math.Abs(spaceAudioFeedback - spacePressedAt);
	}

	private float FindPeak(float[] samples)
	{
		float num = 0f;
		int num2 = 0;
		for (int i = 0; i < samples.Length; i++)
		{
			if (samples[i] > num)
			{
				num = samples[i];
				num2 = i;
			}
		}
		return (float)num2 / 1000f;
	}

	private void PrintOutput(float[] samps)
	{
		string text = "";
		foreach (float num in samps)
		{
			text = text + num + ",\n";
		}
		RDFile.WriteAllText("data.csv", text);
	}

	private void ResetVariables()
	{
		btnRestart.gameObject.SetActive(value: false);
		btnLeave.gameObject.SetActive(value: false);
		txtInputCal.text = "Input Calibration: 0.00";
		txtVisualCal.text = "Input Visual: 0.00";
		txtDialogue.enabled = false;
		microphoneSource = null;
		microphoneSource = GetComponent<AudioSource>();
		isCalibrationFlawed = false;
		firstTimePressingSpace = false;
		isTimeForText = false;
		isStartRecordingTime = false;
		startTickFeedback = 0.0;
		spaceTickFeedback = 0.0;
		spacePressedAt = 0f;
		finalInputCalibration = 0f;
		finalVisualCalibration = 0f;
		spaceAudioFeedback = 0f;
		beepAudioFeedback = 0f;
	}

	public void StartCalibration()
	{
		ResetVariables();
		txtDialogue.enabled = false;
		base.ink.Run("diaCalibration", "Start", () =>
		{
			isStartRecordingTime = true;
		});
	}

	public void LeaveScene()
	{
		scnBase.GoToMainMenu();
	}
}
