using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	public class InspectorPanel_Calibration : InspectorPanel
	{
		public InputField latency;

		public InputField p1InputOffset;

		public InputField p2InputOffset;

		public InputField visualOffset;

		public Image leftHandImg;

		public Image leftHandBg;

		public Image rightHandImg;

		public Image rightHandBg;

		public Color p1HandColor;

		public Color p1BgColor;

		public Color p2HandColor;

		public Color p2BgColor;

		private void Start()
		{
			latency.onEndEdit.AddListener(delegate
			{
				Save();
			});
			p1InputOffset.onEndEdit.AddListener(delegate
			{
				Save();
			});
			p2InputOffset.onEndEdit.AddListener(delegate
			{
				Save();
			});
			visualOffset.onEndEdit.AddListener(delegate
			{
				Save();
			});
		}

		public void UpdateUI()
		{
			latency.text = Mathf.RoundToInt(RDCalibration.latency * 1000f).ToString();
			p1InputOffset.text = Mathf.RoundToInt(RDCalibration.calibration_i * 1000f).ToString();
			p2InputOffset.text = Mathf.RoundToInt(RDCalibration.calibration_i_P2_internal * 1000f).ToString();
			visualOffset.text = Mathf.RoundToInt(RDCalibration.calibration_v * 1000f).ToString();
			bool swapP1AndP2Controls = Persistence.GetSwapP1AndP2Controls();
			rightHandImg.color = (swapP1AndP2Controls ? p2HandColor : p1HandColor);
			rightHandBg.color = (swapP1AndP2Controls ? p2BgColor : p1BgColor);
			leftHandImg.color = (swapP1AndP2Controls ? p1HandColor : p2HandColor);
			leftHandBg.color = (swapP1AndP2Controls ? p1BgColor : p2BgColor);
		}

		private void Save()
		{
			float num = 0.001f;
			RDCalibration.latency = (float)int.Parse(latency.text) * num;
			RDCalibration.calibration_i = (float)int.Parse(p1InputOffset.text) * num;
			RDCalibration.calibration_i_P2_internal = (float)int.Parse(p2InputOffset.text) * num;
			RDCalibration.calibration_v = (float)int.Parse(visualOffset.text) * num;
			RDCalibration.Save();
		}

		public void IncreaseCalibration()
		{
			RDCalibration.IncreaseCalibration();
			UpdateUI();
		}

		public void DecreaseCalibration()
		{
			RDCalibration.DecreaseCalibration();
			UpdateUI();
		}

		public void IncreaseCalibrationP2()
		{
			RDCalibration.IncreaseCalibrationP2();
			UpdateUI();
		}

		public void DecreaseCalibrationP2()
		{
			RDCalibration.DecreaseCalibrationP2();
			UpdateUI();
		}

		public void IncreaseLatency()
		{
			RDCalibration.IncreaseLatency();
			UpdateUI();
		}

		public void DecreaseLatency()
		{
			RDCalibration.DecreaseLatency();
			UpdateUI();
		}

		public void IncreaseCalibrationV()
		{
			RDCalibration.IncreaseCalibrationV();
			UpdateUI();
		}

		public void DecreaseCalibrationV()
		{
			RDCalibration.DecreaseCalibrationV();
			UpdateUI();
		}

		public void SetSlamDefaults()
		{
			RDCalibration.SetToPresets(slam: true);
			UpdateUI();
		}

		public void SetTapDefaults()
		{
			RDCalibration.SetToPresets(slam: false);
			UpdateUI();
		}

		public void SwapHands()
		{
			Persistence.SetSwapP1AndP2Controls(!Persistence.GetSwapP1AndP2Controls());
			LevelEditorPlaySound("sndPagerCursorMove", "LevelEditorInspectorPanel");
			UpdateUI();
		}
	}
}
