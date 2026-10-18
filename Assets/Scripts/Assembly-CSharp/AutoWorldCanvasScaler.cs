using UnityEngine;
using UnityEngine.UI;

public class AutoWorldCanvasScaler : RDBase
{
	private CanvasScaler canvasScaler;

	private void Awake()
	{
		canvasScaler = GetComponent<CanvasScaler>();
	}

	private void Update()
	{
		canvasScaler.dynamicPixelsPerUnit = Screen.height / 198;
	}
}
