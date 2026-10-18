using System;
using System.Linq;
using RDLevelEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class RDBase : RDBaseDllDummy
{
	public static scrVfxControl Vfx;

	public static bool appIsInSteamLibrary;

	public static Platform platform;

	private static long[] discordDevIDs = new long[37]
	{
		235383080884371456L, 213313185544011786L, 129108330026237952L, 214416049230184448L, 164425061128863745L, 133207411203768321L, 191321415914356738L, 260047393934934016L, 301985049140789258L, 207567174351454208L,
		159404252651978753L, 273645812184121344L, 137002336542392320L, 166302763200937994L, 158318575017394177L, 309104296362901505L, 192083644062367745L, 331846965530853378L, 201091631795929089L, 368905841354473472L,
		511370206204002314L, 197664924363653121L, 157972666790182912L, 300050030923087872L, 125330806557114369L, 280093600543408128L, 69464262786953216L, 292673382816677908L, 298722923626364928L, 305547529192472577L,
		628595345798201355L, 348009971037765633L, 498533095541309470L, 221410634481401856L, 1079659511758983248L, 448560475987509268L, 503447721839951884L
	};

	private static ulong[] steamDevIDs = new ulong[2] { 76561197998239313uL, 76561198117167135uL };

	public scrConductor conductor => scrConductor.instance;

	public scnBase scnCurrent => scnBase.instance;

	public scnGame game => scnBase.instance as scnGame;

	public scnMenu menu => scnBase.instance as scnMenu;

	public scnCLS cls => scnCLS.instance;

	public scrGameManager gm => scrGameManager.instance;

	public RDInk ink => scrGameManager.instance.inkDialogue;

	public RDConstants gc => RDConstants.data;

	public bool editorMode => scnEditor.instance != null;

	public scnEditor editor => scnEditor.instance;

	public Rect guiRect => GC.GUIRect;

	public string currentSceneName => SceneManager.GetActiveScene().name;

	protected static DebugSettings debugSettings => DebugSettings.instance;

	public PlatformHelper platformHelper => PlatformHelper.instance;

	public float xGlobal
	{
		get
		{
			return base.transform.position.x;
		}
		set
		{
			base.transform.position = base.transform.position.WithX(value);
		}
	}

	public float yGlobal
	{
		get
		{
			return base.transform.position.y;
		}
		set
		{
			base.transform.position = base.transform.position.WithY(value);
		}
	}

	public float x
	{
		get
		{
			return base.transform.localPosition.x;
		}
		set
		{
			base.transform.localPosition = base.transform.localPosition.WithX(value);
		}
	}

	public float y
	{
		get
		{
			return base.transform.localPosition.y;
		}
		set
		{
			base.transform.localPosition = base.transform.localPosition.WithY(value);
		}
	}

	public static bool isDev
	{
		get
		{
			if (!debugSettings.NoPro)
			{
				if (!discordDevIDs.Contains(RDRichPresence.UserId) && !steamDevIDs.Contains(SteamIntegration.userSteamID) && !Application.isEditor && !Debug.isDebugBuild && !RDC.booth)
				{
					string steamBranchName = GC.steamBranchName;
					if (steamBranchName == null || !steamBranchName.Contains("internal"))
					{
						string steamBranchName2 = GC.steamBranchName;
						int num;
						switch (steamBranchName2)
						{
						default:
							num = ((!(steamBranchName2 == "review")) ? 1 : 0);
							break;
						case "deephospital":
						case "five":
						case "alpha":
						case "hexa":
							num = 0;
							break;
						}
						return num == 0;
					}
				}
				return true;
			}
			return false;
		}
	}

	public static bool isAdvanced
	{
		get
		{
			if (!isDev)
			{
				return Persistence.GetAdvancedEditorFeatures();
			}
			return true;
		}
	}

	public static GameObject CreateIfNotFound(string name, GameObject original = null)
	{
		GameObject gameObject = GameObject.Find(name);
		if (gameObject != null)
		{
			return gameObject;
		}
		GameObject obj = ((original == null) ? new GameObject(name) : UnityEngine.Object.Instantiate(original));
		obj.transform.SetParent(scrGameManager.instance.parentContainer);
		return obj;
	}

	public static void ShowGUIText(string text)
	{
		GUI.Label(new Rect(0f, 0f, 200f, 200f), text);
	}

	public static void PlaySound(string sound, float volume = 1f, AudioMixerGroup group = null, float pitch = 1f, float pan = 0f)
	{
		scrConductor.PlayImmediately(sound, volume, group, pitch, pan);
	}

	public static bool IsHalloweenWeek()
	{
		DateTime now = DateTime.Now;
		DateTime dateTime = new DateTime(now.Year, 10, 24);
		DateTime dateTime2 = dateTime.AddDays(10.0);
		if (DateTime.Now.Year == 2024)
		{
			dateTime2 = dateTime2.AddDays(4.0);
		}
		if (now >= dateTime)
		{
			return now < dateTime2;
		}
		return false;
	}

	public static bool IsAprilFoolsDay()
	{
		DateTime now = DateTime.Now;
		if (now.Month == 4)
		{
			return now.Day == 1;
		}
		return false;
	}
}
