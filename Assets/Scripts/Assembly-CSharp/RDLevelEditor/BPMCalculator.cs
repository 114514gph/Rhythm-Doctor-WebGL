using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class BPMCalculator : RDEditorBase
	{
		public delegate void BPMCalculatorCallback(double s);

		private const int tapsToComplete = 19;

		public SoundData data;

		public Button calculateBPMButton;

		public Text percentageText;

		public float audioBPM;

		[NonSerialized]
		public InspectorPanel currentInspectorPanel;

		public BPMCalculatorCallback onBeatsPerMinuteUpdated;

		public BPMCalculatorCallback onOffsetUpdated;

		private List<float> bpmList;

		private float currentBeatsPerMinute;

		private double lastInputTime;

		private double firstInputTime;

		private bool isFirstInput;

		private bool hasStartedPlaying;

		private Text bpmButtonText;

		private double audioOffset;

		private bool isCalculatingBPM;

		private LevelEventControl_Base currentControl;

		private void Start()
		{
			InitializeValues();
			SetDefaultText();
			calculateBPMButton.onClick.AddListener(Initialize);
			bpmButtonText = calculateBPMButton.GetComponentInChildren<Text>();
			InspectorPanel componentInParent = GetComponentInParent<InspectorPanel>();
			if (currentInspectorPanel == null && componentInParent != null)
			{
				currentInspectorPanel = componentInParent;
			}
		}

		private void SetDefaultText()
		{
			percentageText.text = RDString.Get("editor.BPMCalculator.instructions");
		}

		private void OnDisable()
		{
			if (isCalculatingBPM)
			{
				StopBPMCalculation();
			}
		}

		private void Update()
		{
			if (!isCalculatingBPM)
			{
				return;
			}
			if (!base.editor.isPlaying || currentControl != base.editor.selectedControl)
			{
				if (hasStartedPlaying)
				{
					StopBPMCalculation();
				}
			}
			else
			{
				hasStartedPlaying = true;
			}
			if (RDInput.anyPlayerPress)
			{
				UpdateBPMValues();
			}
		}

		public void Initialize()
		{
			currentControl = base.editor.selectedControl;
			if (!currentInspectorPanel.editor.isPlaying)
			{
				currentInspectorPanel.editor.ScrubToBar(currentInspectorPanel.currentLevelEvent.bar);
			}
			RDUtils.SetMixerVolume("HandPopSoundsVolume", 0f);
			if (base.editor.metronomeActive)
			{
				base.editor.MetronomeButtonClick();
			}
			bpmButtonText.text = RDString.Get("editor.BPMCalculator.stop");
			SetStatusPercent(0);
			Button.ButtonClickedEvent onClick = calculateBPMButton.onClick;
			onClick.RemoveAllListeners();
			onClick.AddListener(StopBPMCalculation);
			isCalculatingBPM = true;
			hasStartedPlaying = false;
		}

		public void InitializeValues()
		{
			isFirstInput = true;
			bpmList = new List<float>();
			currentBeatsPerMinute = 0f;
		}

		private void ResetCalculateBPMButtonValues()
		{
			Button.ButtonClickedEvent onClick = calculateBPMButton.onClick;
			onClick.RemoveAllListeners();
			onClick.AddListener(Initialize);
			RDUtils.SetMixerVolume("HandPopSoundsVolume", 1f);
			bpmButtonText.text = RDString.Get("editor.BPMCalculator.calculateBPM");
			bpmButtonText.color = Color.white;
		}

		public void UpdateBPMValues()
		{
			if (!hasStartedPlaying)
			{
				return;
			}
			if (!isFirstInput)
			{
				currentBeatsPerMinute = 60f / (float)(base.conductor.audioPos - lastInputTime);
				lastInputTime = base.conductor.audioPos;
				bpmList.Add(currentBeatsPerMinute);
				if (bpmList.Count < 19)
				{
					int statusPercent = Mathf.RoundToInt((bpmList.Count + 1) * 100 / 20);
					SetStatusPercent(statusPercent);
					return;
				}
				ResetCalculateBPMButtonValues();
				isCalculatingBPM = false;
				currentInspectorPanel.editor.PlayPauseButtonClick();
				float num = 0f;
				for (int i = 0; i < bpmList.Count; i++)
				{
					num += bpmList[i];
				}
				float f = (float)Mathf.RoundToInt(num / (float)bpmList.Count * 10f) / 10f;
				audioBPM = Mathf.RoundToInt(f);
				percentageText.text = RDString.Get("editor.BPMCalculator.result").Replace("[bpm]", f.ToString());
				float num2 = (float)(lastInputTime - firstInputTime) / (float)bpmList.Count;
				audioOffset = firstInputTime;
				while (audioOffset - (double)num2 > 0.0)
				{
					audioOffset -= num2;
				}
				if (onOffsetUpdated != null)
				{
					onOffsetUpdated(audioOffset);
				}
				if (onBeatsPerMinuteUpdated != null)
				{
					onBeatsPerMinuteUpdated(audioBPM);
				}
				InitializeValues();
			}
			else
			{
				isFirstInput = false;
				SetStatusPercent(Mathf.RoundToInt(5f));
				lastInputTime = base.conductor.audioPos;
				firstInputTime = base.conductor.audioPos;
			}
		}

		private void SetStatusPercent(int percent)
		{
			percentageText.text = RDString.Get("editor.BPMCalculator.progress").Replace("[percent]", percent.ToString());
		}

		public void StopBPMCalculation()
		{
			InitializeValues();
			SetDefaultText();
			isCalculatingBPM = false;
			ResetCalculateBPMButtonValues();
			if (currentInspectorPanel.editor.isPlaying)
			{
				currentInspectorPanel.editor.PlayPauseButtonClick();
			}
		}
	}
}
