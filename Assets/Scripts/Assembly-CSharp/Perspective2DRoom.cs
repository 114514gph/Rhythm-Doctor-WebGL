using System;
using UnityEngine;

public class Perspective2DRoom : MonoBehaviour
{
	[Header("Camera Transform")]
	public Vector3 cameraPosition = new Vector3(0f, 40f, 0f);

	public Vector3 cameraRotation;

	public float cameraFOV = 90f;

	private readonly Vector2 screenRes = new Vector2(352f, 198f);

	private Vector3 cameraOffset;

	private Vector2 perspectiveOrigin;

	private float unit => screenRes.x / Mathf.Tan(cameraFOV * (MathF.PI / 180f) / 2f);

	private void Awake()
	{
		cameraOffset = screenRes.x * Vector3.back;
		perspectiveOrigin = screenRes / 2f;
	}

	public bool ApplyPerspective(Transform tr, Vector3 position, Vector3 scale)
	{
		Vector3 vector = position - cameraPosition - cameraOffset;
		if (vector.z <= 0f || position.y < 0f || scale.x <= 0f || scale.y <= 0f)
		{
			return false;
		}
		float x = vector.x;
		float z = vector.z;
		Vector3 localScale = scale * (unit / z);
		if (localScale.x > 800f)
		{
			return false;
		}
		tr.localPosition = new Vector3(x * (unit / z), vector.y * (unit / z), (0f - z) / 35.2f) + (Vector3)perspectiveOrigin;
		tr.localScale = localScale;
		return true;
	}

	public Vector3 GetProjectedPosition(Perspective2DSprite obj, Vector2 screenPosition, Vector2 screenScale)
	{
		screenPosition -= perspectiveOrigin;
		screenPosition += obj.spritePivot * screenScale;
		Vector3 vector = default;
		vector.z = obj.scale.x * unit / screenScale.x;
		vector.x = screenPosition.x * vector.z / unit;
		vector.y = screenPosition.y * vector.z / unit;
		return vector + cameraOffset + cameraPosition;
	}
}
