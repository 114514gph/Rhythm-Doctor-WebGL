#if !UNITY_WEBGL
using System;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Win32;
using SpeechLib;
using UnityEngine;

public class WindowsTTS : MonoBehaviour
{
	public enum SPEAKFLAGS
	{
		Default = 0,
		lagsAsync = 1,
		PurgeBeforeSpeak = 2,
		IsFilename = 4,
		IsXML = 8,
		IsNotXML = 16,
		PersistXML = 32,
		NLPSpeakPunc = 64,
		ParseSapi = 128,
		ParseSsml = 256,
		ParseAutodetect = Default,
		NLPMask = NLPSpeakPunc,
		ParseMask = 384,
		VoiceMask = 511,
		UnusedFlags = -512
	}

	public enum RUNSTATE
	{
		Done = 1,
		IsSpeaking = 2
	}

	public static WindowsTTS instance;

	private static SpVoice spVoice;

	private static bool _m_UseNVDA;

	private static float m_NVDAIsSpeakingTimer = -1f;

	public static bool UseNVDA
	{
		get
		{
			return _m_UseNVDA;
		}
		set
		{
			_m_UseNVDA = value;
		}
	}

	[DllImport("nvdaControllerClient")]
	internal static extern int nvdaController_testIfRunning();

	[DllImport("nvdaControllerClient", CharSet = CharSet.Auto)]
	internal static extern int nvdaController_speakText(string text);

	[DllImport("nvdaControllerClient")]
	internal static extern int nvdaController_cancelSpeech();

	private void Awake()
	{
		InitializeSAPI();
	}

	private static void InitializeSAPI()
	{
		spVoice = new SpVoiceClass();
		try
		{
			string text = string.Empty;
			using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Narrator\\NoRoam"))
			{
				if (registryKey != null)
				{
					object value = registryKey.GetValue("SpeechVoice");
					if (value != null)
					{
						text = value.ToString();
					}
				}
			}
			SpObjectTokenCategoryClass spObjectTokenCategoryClass = new SpObjectTokenCategoryClass();
			((ISpObjectTokenCategory)spObjectTokenCategoryClass).SetId("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Speech_OneCore\\Voices", 0);
			((ISpObjectTokenCategory)spObjectTokenCategoryClass).EnumTokens(string.Empty, string.Empty, out SpMMAudioEnum ppEnum);
			ppEnum.GetCount(out var pCount);
			for (uint num = 0u; num < pCount; num++)
			{
				ppEnum.Item(num, out var ppToken);
				ppToken.GetId(out var ppszCoMemTokenId);
				string text2 = ppszCoMemTokenId.Substring(ppszCoMemTokenId.IndexOf('\\') + 1);
				using RegistryKey registryKey2 = Registry.LocalMachine.OpenSubKey(text2);
				if (registryKey2 != null)
				{
					object value2 = registryKey2.GetValue(string.Empty);
					if (value2 != null && text == value2.ToString())
					{
						spVoice.SetVoice(ppToken);
					}
				}
			}
		}
		catch (Exception ex)
		{
			Debug.Log("Error getting user chosen voice: " + ex.Message);
		}
	}

	private void Start()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			UnityEngine.Object.DestroyImmediate(base.gameObject);
		}
	}

	private void Update()
	{
		if (m_NVDAIsSpeakingTimer > 0f)
		{
			m_NVDAIsSpeakingTimer -= Time.unscaledDeltaTime;
		}
	}

	private void OnDestroy()
	{
		if (instance == this)
		{
			instance = null;
		}
	}

	public static void Speak(string msg)
	{
		if (UseNVDA)
		{
			nvdaController_speakText(msg);
			m_NVDAIsSpeakingTimer += (float)msg.Length * 0.015f;
		}
		else
		{
			new Thread(() =>
			{
				spVoice.Speak(msg, 17u, out var _);
			}).Start();
		}
	}

	public static void Stop()
	{
		if (UseNVDA)
		{
			nvdaController_cancelSpeech();
			m_NVDAIsSpeakingTimer = 0f;
		}
		else
		{
			new Thread(() =>
			{
				spVoice.Speak(null, 3u, out var _);
			}).Start();
		}
	}

	public static void ToggleSAPI(bool on)
	{
		if (on)
		{
			UseNVDA = false;
		}
		else
		{
			UseNVDA = IsNVDADetected();
		}
	}

	public static bool IsSpeaking()
	{
		if (!Application.isPlaying)
		{
			return false;
		}
		if (UseNVDA)
		{
			return m_NVDAIsSpeakingTimer > 0f;
		}
		spVoice.GetStatus(out var pStatus, out var _);
		return pStatus.dwRunningState == 2;
	}

	public static bool IsNVDADetected()
	{
		return nvdaController_testIfRunning() == 0;
	}
}
#endif
