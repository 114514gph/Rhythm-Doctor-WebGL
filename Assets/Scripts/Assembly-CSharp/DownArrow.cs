using UnityEngine;

public class DownArrow : RDBase
{
	private RectTransform rt;

	private float initialY;

	private bool isUp;

	private void Start()
	{
		rt = GetComponent<RectTransform>();
		initialY = rt.anchoredPosition.y;
	}

	private void Update()
	{
		isUp = Time.timeSinceLevelLoad % 2f < 1f;
		rt.AnchorPosY(initialY + (isUp ? 1f : 0f));
	}
}
