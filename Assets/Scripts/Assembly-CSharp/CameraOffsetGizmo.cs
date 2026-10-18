using UnityEngine;

[ExecuteInEditMode]
public class CameraOffsetGizmo : MonoBehaviour
{
	public bool retainWorldPosition;

	private Vector3 lastPosition;

	private Vector3 lastLocalPosition;

	private bool wasSelected;

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(1f, 0f, 0f, 0.75f);
		Gizmos.DrawWireCube(base.transform.position.WithY(99f), new Vector3(352f, 198f, 1f));
	}
}
