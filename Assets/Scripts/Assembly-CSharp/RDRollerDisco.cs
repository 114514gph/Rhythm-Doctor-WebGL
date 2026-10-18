using UnityEngine;

public class RDRollerDisco : RDBase
{
	private const float length = 1000f;

	private const float hardcodedOffset = 489f;

	private float realX;

	public float scrollSpeed;

	public float xOffset;

	public Transform bgContainer;

	[Header("Day Night")]
	public GameObject dayContainer;

	public GameObject nightContainer;

	private void Update()
	{
		UpdatePosition();
	}

	public void UpdatePosition()
	{
		realX -= scrollSpeed * Time.deltaTime;
		float num = (realX + 489f - xOffset) % 1000f;
		bgContainer.LocalMoveX(num);
	}

	public void ScrollTo(float x)
	{
		realX = x;
	}

	public void SetNightMode(bool night)
	{
		dayContainer.SetActive(!night);
		nightContainer.SetActive(night);
	}
}
