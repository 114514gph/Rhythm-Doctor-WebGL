using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BoothTimer : RDBase
{
	private const float defaultMinutesForInactivity = 5f;

	private static float minutesForInactivity = 5f;

	private static float timeToRestartAt = 9999f;

	private static bool dontRestart = true;

	private static BoothTimer _instance;

	public static BoothTimer instance
	{
		get
		{
			if (_instance == null)
			{
				GameObject obj = new GameObject("BOOTH TIMER");
				_instance = obj.AddComponent<BoothTimer>();
				Object.DontDestroyOnLoad(obj);
			}
			return _instance;
		}
	}

	public static void Run()
	{
		_ = instance;
		string path = Path.Combine(Application.streamingAssetsPath, "BoothInactivityMinutes.txt");
		if (File.Exists(path))
		{
			if (float.TryParse(File.ReadAllText(path), out var result))
			{
				minutesForInactivity = result;
			}
		}
		else
		{
			File.WriteAllText(path, 5f.ToString());
		}
	}

	private void Update()
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		if (Input.anyKeyDown)
		{
			dontRestart = false;
			timeToRestartAt = realtimeSinceStartup + minutesForInactivity * 60f;
		}
		if (realtimeSinceStartup >= timeToRestartAt)
		{
			RestartDemo();
		}
	}

	private static void RestartDemo()
	{
		if (!((double)minutesForInactivity < 0.01) && !dontRestart)
		{
			dontRestart = true;
			Persistence.currentSlotIndex = 0;
			Persistence.ToggleLevelsState(unlock: false);
			Singleton<AudioManager>.Instance?.DestroyAllAudioSources();
			SceneManager.LoadScene("scnExpoMenu");
		}
	}

	public static void SetMinutes(float minutes)
	{
		minutesForInactivity = minutes;
	}
}
