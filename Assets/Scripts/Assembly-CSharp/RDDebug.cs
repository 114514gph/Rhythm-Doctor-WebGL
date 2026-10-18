using UnityEngine;

public class RDDebug : MonoBehaviour
{
	public static bool active;

	public static bool GetKeyDown(KeyCode keyCode)
	{
		if (active && Input.GetKeyDown(keyCode))
		{
			MonoBehaviour.print("GetKeyDown - " + keyCode);
			return true;
		}
		return false;
	}
}
