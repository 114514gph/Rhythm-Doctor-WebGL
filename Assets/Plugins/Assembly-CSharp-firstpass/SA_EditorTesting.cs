using UnityEngine;
using UnityEngine.EventSystems;

public static class SA_EditorTesting
{
	public const int DEFAULT_SORT_ORDER = 10000;

	public static bool IsInsideEditor
	{
		get
		{
			bool result = false;
			if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
			{
				result = true;
			}
			return result;
		}
	}

	public static bool HasFill(float fillRate)
	{
		if ((float)Random.Range(1, 100) <= fillRate)
		{
			return true;
		}
		return false;
	}

	public static void CheckForEventSystem()
	{
		_ = (EventSystem)Object.FindObjectOfType(typeof(EventSystem)) == null;
	}
}
