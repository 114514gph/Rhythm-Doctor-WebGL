using UnityEngine;

public class Follower : MonoBehaviour
{
	public Transform targetToFollow;

	private void LateUpdate()
	{
		if (targetToFollow != null)
		{
			base.transform.transform.position = targetToFollow.position;
			base.transform.rotation = targetToFollow.rotation;
		}
	}
}
