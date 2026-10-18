using UnityEngine;

public class RDRooftopClouds : MonoBehaviour
{
	public float speed;

	private float width;

	private Vector3 startPosition;

	private Vector3 targetPosition;

	private void Start()
	{
		width = GetComponent<tk2dTiledSprite>().dimensions.x;
		startPosition = base.transform.localPosition;
		targetPosition = startPosition + Vector3.left * width / 2f;
	}

	private void Update()
	{
		base.transform.localPosition = Vector3.MoveTowards(base.transform.localPosition, targetPosition, speed * Time.deltaTime);
		if (base.transform.localPosition == targetPosition)
		{
			base.transform.localPosition = startPosition;
		}
	}
}
