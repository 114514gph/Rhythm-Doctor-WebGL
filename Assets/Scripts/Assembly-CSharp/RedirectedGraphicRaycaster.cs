using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RedirectedGraphicRaycaster : GraphicRaycaster
{
	public RectTransform sourceRect;

	public GraphicRaycaster targetRaycaster;

	public bool overrideTarget = true;

	private Camera sourceCam;

	private Camera targetCam;

	protected override void Awake()
	{
		base.Awake();
		if (sourceRect == null)
		{
			sourceRect = base.transform as RectTransform;
		}
		sourceCam = sourceRect.GetComponentInParent<Canvas>().worldCamera;
		targetCam = targetRaycaster.GetComponent<Canvas>().worldCamera;
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		if (overrideTarget)
		{
			targetRaycaster.enabled = false;
		}
	}

	public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
	{
		Rect rectangle = RectTransformToScreenSpace(sourceRect, sourceCam);
		if (rectangle.Contains(eventData.position))
		{
			Vector2 vector = Rect.PointToNormalized(rectangle, eventData.position);
			Vector2 vector2 = ((targetCam.targetTexture != null) ? new Vector2(targetCam.targetTexture.width, targetCam.targetTexture.height) : new Vector2(Screen.width, Screen.height));
			Vector2 pressPosition = (eventData.position = vector * vector2);
			eventData.pressPosition = pressPosition;
			targetRaycaster.Raycast(eventData, resultAppendList);
		}
	}

	public static Rect RectTransformToScreenSpace(RectTransform transform, Camera cam)
	{
		Vector3[] array = new Vector3[4];
		Vector3[] array2 = new Vector3[4];
		transform.GetWorldCorners(array);
		for (int i = 0; i < 4; i++)
		{
			array2[i] = cam.WorldToScreenPoint(array[i]);
		}
		return new Rect(array2[0].x, array2[0].y, array2[2].x - array2[0].x, array2[2].y - array2[0].y);
	}
}
