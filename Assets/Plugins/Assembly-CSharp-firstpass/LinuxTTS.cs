using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using System.Threading;
using UnityEngine;

public class LinuxTTS : MonoBehaviour
{
	public static LinuxTTS instance;

	private BlockingCollection<string> speechQueue;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			speechQueue = new BlockingCollection<string>();
			BlockingCollection<string> threadSpeachQueue = speechQueue;
			new Thread(() =>
			{
				SpeechThread(threadSpeachQueue);
			}).Start();
		}
		else
		{
			UnityEngine.Debug.LogError("[Accessibility] Trying to create another Linux TTS instance, when there already is one.");
			UnityEngine.Object.DestroyImmediate(base.gameObject);
		}
	}

	private bool? CheckIsSteamRuntime()
	{
		try
		{
			using Process process = NewSpdSayProcess("--version", redirectStandardStreams: false, isSteamRuntime: false);
			process.Start();
			process.WaitForExit();
			return false;
		}
		catch
		{
			try
			{
				using Process process2 = NewSpdSayProcess("--version", redirectStandardStreams: false, isSteamRuntime: true);
				process2.Start();
				process2.WaitForExit();
				return true;
			}
			catch
			{
				return null;
			}
		}
	}

	private Process NewSpdSayProcess(string arguments, bool redirectStandardStreams, bool isSteamRuntime)
	{
		Process process = new Process
		{
			StartInfo = 
			{
				CreateNoWindow = true,
				UseShellExecute = false
			}
		};
		if (isSteamRuntime)
		{
			process.StartInfo.FileName = "steam-runtime-launch-client";
			process.StartInfo.Arguments = "--alongside-steam --host -- spd-say " + arguments;
		}
		else
		{
			process.StartInfo.FileName = "spd-say";
			process.StartInfo.Arguments = arguments;
		}
		if (redirectStandardStreams)
		{
			process.StartInfo.RedirectStandardInput = true;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
			process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
		}
		return process;
	}

	private void SpeechThread(BlockingCollection<string> speechQueue)
	{
		bool? flag = CheckIsSteamRuntime();
		if (!flag.HasValue)
		{
			UnityEngine.Debug.LogWarning("Failed to run `spd-say`; try running `sudo apt-get install speech-dispatcher`");
			return;
		}
		bool value = flag.Value;
		while (true)
		{
			string text;
			try
			{
				text = speechQueue.Take();
			}
			catch (InvalidOperationException)
			{
				if (speechQueue.IsCompleted)
				{
					speechQueue.Dispose();
					break;
				}
				throw;
			}
			if (text == null)
			{
				using (Process process = NewSpdSayProcess("-C", redirectStandardStreams: false, value))
				{
					process.Start();
					process.WaitForExit();
				}
				continue;
			}
			int speechRate = UAP_AccessibilityManager.GetSpeechRate();
			string text2 = string.Empty;
			if (speechRate != 0)
			{
				float t = Mathf.InverseLerp(0f, 100f, speechRate);
				int num = (int)Mathf.Lerp(-100f, 100f, t);
				text2 = $" -r {num}";
			}
			int speechVolume = UAP_AccessibilityManager.GetSpeechVolume();
			string text3 = string.Empty;
			if (speechVolume != 11)
			{
				float t2 = Mathf.InverseLerp(1f, 10f, speechVolume);
				float num2 = (int)Mathf.Lerp(-100f, 100f, t2);
				text3 = $" -i {num2}";
			}
			string arguments = "-e" + text2 + text3;
			using Process process2 = NewSpdSayProcess(arguments, redirectStandardStreams: true, value);
			process2.Start();
			process2.BeginOutputReadLine();
			process2.BeginErrorReadLine();
			process2.StandardInput.Write(text);
			process2.StandardInput.Close();
			process2.WaitForExit();
		}
	}

	public void Speak(string msg)
	{
		speechQueue.Add(msg);
	}

	public void Stop()
	{
		string item;
		while (speechQueue.TryTake(out item))
		{
		}
		speechQueue.Add(null);
	}

	public bool IsSpeaking()
	{
		return false;
	}

	private void OnDestroy()
	{
		Stop();
		speechQueue.CompleteAdding();
		speechQueue = null;
	}
}
