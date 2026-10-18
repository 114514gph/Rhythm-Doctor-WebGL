using UnityEngine;

public static class RDCalibration
{
	public static float latency;

	public static float calibration_v;

	public static float calibration_i;

	public static float calibration_i_P2_internal;

	public static int NotSet = 999;

	public static float calibration_i_P2
	{
		get
		{
			if (GC.d_separateP2Calibration)
			{
				return calibration_i_P2_internal;
			}
			return calibration_i;
		}
	}

	public static void Setup()
	{
		Persistence.GetCalibrationValues(out calibration_v, out calibration_i, out calibration_i_P2_internal, out latency);
		if (calibration_v == (float)NotSet)
		{
			SetToPresets(slam: true);
		}
		SetLatencyToPlatform();
		Persistence.SetLatencyValue(latency);
	}

	public static void SetLatencyToPlatform()
	{
		float num = 0.6f;
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
		{
			num = 1f;
		}
		latency = num;
	}

	public static void SetToPresets(bool slam, bool saveOnlyVisualOffset = false)
	{
		float num = calibration_i;
		if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor)
		{
			calibration_v = 0.02f;
			latency = 0.6f;
		}
		else if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
		{
			calibration_v = 0.09f;
			latency = 0.6f;
		}
		else if (Application.platform == RuntimePlatform.Android)
		{
			calibration_v = 0.23f;
			calibration_i = 0.23f;
			latency = 1f;
		}
		else if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			calibration_v = 0.05f;
			calibration_i = 0.07f;
			latency = 1f;
		}
		else if (Application.platform == RuntimePlatform.Switch)
		{
			Debug.Log("Calibration set to Nintendo Switch (non-docked) defaults.");
			calibration_v = 0.04f;
			calibration_i = 0.06f;
			latency = 1f;
		}
		else if (Application.platform == RuntimePlatform.WebGLPlayer)
		{
			if (SystemInfo.operatingSystemFamily == OperatingSystemFamily.MacOSX)
			{
				calibration_v = 0.02f;
				latency = 0.6f;
			}
			else
			{
				calibration_v = 0.09f;
				latency = 0.6f;
			}
		}
		else
		{
			calibration_v = 0.09f;
			latency = 0.6f;
		}
		if (!scnBase.isMobile && Application.platform != RuntimePlatform.Switch)
		{
			calibration_i = calibration_v + (slam ? 0.03f : 0f);
		}
		if (calibration_i == (float)NotSet)
		{
			Debug.Log("CHECK YOUR isMOBILE FLAG ISNT RETURN TRUE CUZ THIS SHOULD NOT BE HAPPENING ON DESKTOPS");
			calibration_i = calibration_v + (slam ? 0.03f : 0f);
		}
		if (saveOnlyVisualOffset)
		{
			calibration_i = num;
		}
		calibration_i_P2_internal = calibration_i;
		Save();
	}

	public static void IncreaseCalibration(bool finer = false)
	{
		calibration_i += (finer ? 0.001f : 0.01f);
		Save();
	}

	public static void DecreaseCalibration(bool finer = false)
	{
		calibration_i -= (finer ? 0.001f : 0.01f);
		Save();
	}

	public static void IncreaseCalibrationP2()
	{
		calibration_i_P2_internal += 0.01f;
		Save();
	}

	public static void DecreaseCalibrationP2()
	{
		calibration_i_P2_internal -= 0.01f;
		Save();
	}

	public static void IncreaseLatency()
	{
		latency += 0.1f;
		Save();
	}

	public static void DecreaseLatency()
	{
		latency -= 0.1f;
		Save();
	}

	public static void IncreaseCalibrationV()
	{
		calibration_v += 0.01f;
		Save();
	}

	public static void DecreaseCalibrationV()
	{
		calibration_v -= 0.01f;
		Save();
	}

	public static void Save()
	{
		Persistence.SetCalibrationValues(calibration_v, calibration_i, calibration_i_P2_internal, latency);
	}

	public static void SetToDefaultCalibrationValues(int defaultValue = 0)
	{
		if (defaultValue == 1)
		{
			SetToPresets(slam: true);
		}
		if (defaultValue == 2)
		{
			SetToPresets(slam: false);
		}
	}
}
