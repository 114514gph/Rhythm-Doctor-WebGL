using System;
using Steamworks;
using UnityEngine;

public static class SteamIntegration
{
	public struct SteamAchievement
	{
		public Achievement id;

		public string name;

		public string description;

		public bool achieved;

		public SteamAchievement(Achievement id)
		{
			this.id = id;
			name = string.Empty;
			description = string.Empty;
			achieved = false;
		}
	}

	public struct SteamStat
	{
		public Stat name;

		public float value;

		public bool isValueInt;

		public SteamStat(Stat name, bool valueType)
		{
			this.name = name;
			value = 0f;
			isValueInt = valueType;
		}
	}

	public enum Stat
	{
		stat_test = 0
	}

	public static bool initialized;

	private static SteamAchievement[] steamAchievementArray = new SteamAchievement[0];

	public static CGameID gameID;

	public static ulong userSteamID;

	private static SteamStat[] steamStatArray = new SteamStat[1]
	{
		new SteamStat(Stat.stat_test, valueType: false)
	};

	private static Callback<UserStatsReceived_t> userStatsReceived;

	private static Callback<UserStatsStored_t> userStatsStored;

	private static Callback<UserAchievementStored_t> userAchievementStored;

	private static bool everInitialized;

	public static bool IsSteamInBigPictureMode
	{
		get
		{
			if (!initialized)
			{
				return false;
			}
			return SteamUtils.IsSteamInBigPictureMode();
		}
	}

	public static void Setup()
	{
		if (everInitialized)
		{
			throw new Exception("Tried to Initialize the SteamAPI twice in one session");
		}
		if (!Packsize.Test())
		{
			Debug.LogError("[Steamworks.NET] Packsize Test returned false, the wrong version of Steamworks.NET is being run in this platform.");
		}
		if (!DllCheck.Test())
		{
			Debug.LogError("[Steamworks.NET] DllCheck Test returned false, One or more of the Steamworks binaries seems to be the wrong version.");
		}
		initialized = SteamAPI.Init();
		if (initialized)
		{
			SteamApps.GetCurrentBetaName(out var pchName, 100);
			Debug.Log("Steam Build: " + (pchName.IsNullOrEmpty() ? "default" : pchName));
			userSteamID = (ulong)SteamUser.GetSteamID();
			if (RDBase.isDev)
			{
				RDBaseDllDummy.printes($"<color=#00aa00>SteamID {userSteamID} is developer!</color>");
			}
			DebugSettings.instance.RunningOnSteamDeck = SteamUtils.IsSteamRunningOnSteamDeck();
			everInitialized = true;
		}
		else
		{
			Debug.LogWarning("[Steamworks.NET] SteamAPI_Init() failed.");
		}
	}

	public static void CheckCallbacks()
	{
		if (initialized)
		{
			SteamAPI.RunCallbacks();
		}
	}

	public static bool OpenConnection()
	{
		if (!initialized)
		{
			return false;
		}
		gameID = new CGameID(SteamUtils.GetAppID());
		userStatsReceived = Callback<UserStatsReceived_t>.Create(OnUserStatsReceived);
		userStatsStored = Callback<UserStatsStored_t>.Create(OnUserStatsStored);
		userAchievementStored = Callback<UserAchievementStored_t>.Create(OnAchievementStored);
		if (SteamUserStats.RequestCurrentStats())
		{
			for (int i = 0; i < steamStatArray.Length; i++)
			{
				SteamUserStats.GetStat(steamStatArray[i].name.ToString(), out steamStatArray[i].value);
			}
		}
		return true;
	}

	public static void CloseConnection()
	{
		if (initialized)
		{
			SteamAPI.Shutdown();
			everInitialized = false;
			initialized = false;
		}
	}

	public static bool GetStatValue(Stat statName, ref int value)
	{
		if (!initialized)
		{
			return false;
		}
		for (int i = 0; i < steamStatArray.Length; i++)
		{
			if (statName == steamStatArray[i].name)
			{
				value = (int)steamStatArray[i].value;
				return true;
			}
		}
		return false;
	}

	public static bool GetStatValue(Stat statName, ref float value)
	{
		if (!initialized)
		{
			return false;
		}
		for (int i = 0; i < steamStatArray.Length; i++)
		{
			if (statName == steamStatArray[i].name)
			{
				value = steamStatArray[i].value;
				return true;
			}
		}
		return false;
	}

	public static bool SetStatValue(Stat statName, int addedValue)
	{
		if (!initialized)
		{
			return false;
		}
		for (int i = 0; i < steamStatArray.Length; i++)
		{
			if (statName == steamStatArray[i].name)
			{
				steamStatArray[i].value += addedValue;
				Debug.Log((int)steamStatArray[i].value);
				SteamUserStats.SetStat(statName.ToString(), (int)steamStatArray[i].value);
				SteamUserStats.StoreStats();
				return true;
			}
		}
		return false;
	}

	public static bool SetStatValue(Stat statName, float addedValue)
	{
		if (!initialized)
		{
			return false;
		}
		for (int i = 0; i < steamStatArray.Length; i++)
		{
			if (statName == steamStatArray[i].name)
			{
				steamStatArray[i].value += addedValue;
				SteamUserStats.SetStat(statName.ToString(), steamStatArray[i].value);
				SteamUserStats.StoreStats();
				return true;
			}
		}
		return false;
	}

	public static string[] GetSteamFriends()
	{
		if (initialized)
		{
			string[] array = new string[SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagImmediate)];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = SteamFriends.GetFriendPersonaName(SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagImmediate));
			}
			return array;
		}
		return new string[0];
	}

	public static string GetPlayersName()
	{
		if (!initialized)
		{
			return string.Empty;
		}
		return SteamFriends.GetPersonaName();
	}

	public static bool GetAchievementByName(Achievement achievementID, ref SteamAchievement steamAchievement)
	{
		if (!initialized)
		{
			return false;
		}
		for (int i = 0; i < steamAchievementArray.Length; i++)
		{
			if (steamAchievementArray[i].id == achievementID)
			{
				steamAchievement = steamAchievementArray[i];
				return true;
			}
		}
		return false;
	}

	public static void UnlockAchievement(Achievement achievement, bool storeStats = false)
	{
		if (initialized)
		{
			UnlockAchievement(achievement.ToString(), storeStats);
		}
	}

	public static void UnlockAchievement(string achievement, bool storeStats = false)
	{
		if (initialized)
		{
			SteamUserStats.SetAchievement(achievement.ToString());
			if (storeStats)
			{
				StoreStats();
			}
		}
	}

	public static void StoreStats()
	{
		if (initialized)
		{
			SteamUserStats.StoreStats();
		}
	}

	private static void OnUserStatsReceived(UserStatsReceived_t pCallback)
	{
		if (!initialized || (ulong)gameID != pCallback.m_nGameID)
		{
			return;
		}
		if (EResult.k_EResultOK == pCallback.m_eResult)
		{
			for (int i = 0; i < steamAchievementArray.Length; i++)
			{
				if (SteamUserStats.GetAchievement(steamAchievementArray[i].id.ToString(), out steamAchievementArray[i].achieved))
				{
					steamAchievementArray[i].name = SteamUserStats.GetAchievementDisplayAttribute(steamAchievementArray[i].id.ToString(), "name");
					steamAchievementArray[i].description = SteamUserStats.GetAchievementDisplayAttribute(steamAchievementArray[i].id.ToString(), "desc");
				}
				else
				{
					Debug.LogWarning("SteamUserStats.GetAchievement failed for Achievement " + steamAchievementArray[i].id.ToString() + "\nIs it registered in the Steam Partner site?");
				}
			}
			for (int j = 0; j < steamStatArray.Length; j++)
			{
				SteamUserStats.GetStat(steamStatArray[j].name.ToString(), out steamStatArray[j].value);
			}
		}
		else
		{
			Debug.Log("RequestStats - failed, " + pCallback.m_eResult);
		}
	}

	private static void OnUserStatsStored(UserStatsStored_t pCallback)
	{
		if ((ulong)gameID == pCallback.m_nGameID)
		{
			if (EResult.k_EResultOK == pCallback.m_eResult)
			{
				Debug.Log("StoreStats - success");
			}
			else if (EResult.k_EResultInvalidParam == pCallback.m_eResult)
			{
				Debug.Log("StoreStats - some failed to validate");
				OnUserStatsReceived(new UserStatsReceived_t
				{
					m_eResult = EResult.k_EResultOK,
					m_nGameID = (ulong)gameID
				});
			}
			else
			{
				Debug.Log("StoreStats - failed, " + pCallback.m_eResult);
			}
		}
	}

	private static void OnAchievementStored(UserAchievementStored_t pCallback)
	{
		if ((ulong)gameID == pCallback.m_nGameID)
		{
			if (pCallback.m_nMaxProgress == 0)
			{
				Debug.Log("Achievement '" + pCallback.m_rgchAchievementName + "' unlocked!");
				return;
			}
			Debug.Log("Achievement '" + pCallback.m_rgchAchievementName + "' progress callback, (" + pCallback.m_nCurProgress + "," + pCallback.m_nMaxProgress + ")");
		}
	}

	private static void ClearAllAchievements()
	{
		if (initialized)
		{
			for (int i = 0; i < steamAchievementArray.Length; i++)
			{
				SteamUserStats.ClearAchievement(steamAchievementArray[i].id.ToString());
			}
		}
	}

	private static void ResetAllData()
	{
		if (initialized)
		{
			SteamUserStats.ResetAllStats(bAchievementsToo: true);
		}
	}
}
