using System;
using System.Collections.Generic;
using UnityEngine.Analytics;

public static class Analytics
{
	public static void UploadBranchToUnity()
	{
#if !UNITY_WEBGL
		int dayOfYear = DateTime.Now.DayOfYear;
		if (Persistence.GetLastAnalyticsUpdate() != dayOfYear)
		{
			Persistence.SetLastAnalyticsUpdate(dayOfYear);
			string pchName = "unknown";
			if (SteamIntegration.initialized)
			{
				SteamApps.GetCurrentBetaName(out pchName, 20);
			}
			if (!pchName.IsNullOrEmpty())
			{
				UnityEngine.Analytics.Analytics.CustomEvent("VersionInfo", new Dictionary<string, object> { { "branch", pchName } });
			}
		}
#endif
	}
}
