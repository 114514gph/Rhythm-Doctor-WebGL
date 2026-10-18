using UnityEngine;
using UnityEngine.UI;

public class UIClampWidth : MonoBehaviour
{
	public CanvasScaler canvasScaler;

	public float minWidth;

	public float maxWidth;

	public float padding;

	private RectTransform rt;

	private void Awake()
	{
		rt = GetComponent<RectTransform>();
	}

	private void LateUpdate()
	{
		float value = canvasScaler.referenceResolution.x - padding;
		rt.SizeDeltaX(Mathf.Clamp(value, minWidth, maxWidth));
	}
}
