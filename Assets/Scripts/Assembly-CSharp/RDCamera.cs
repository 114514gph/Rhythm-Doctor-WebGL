using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class RDCamera : RDBase
{
	public CameraSizeCalculation useDynamicRatio;

	public bool dontMove;

	[NonSerialized]
	public Camera camera;

	[NonSerialized]
	public Tween quad_MoveXTween;

	[NonSerialized]
	public Tween quad_MoveYTween;

	[NonSerialized]
	public Tween quad_ZoomTween;

	[NonSerialized]
	public Tween quad_AngleTween;

	[NonSerialized]
	public Vector2 customScale = Vector2.one;

	[NonSerialized]
	public Tween smoothShake;

	[NonSerialized]
	public Tween rotateShake;

	private Tween zoomTween;

	private Tween pulseTween;

	private float angle;

	private float pulseAmount;

	private Coroutine rotateCoroutine;

	public float zoom { get; private set; } = 1f;

	public virtual void Awake()
	{
		camera = GetComponent<Camera>();
		UpdateCameraSize();
		if (!dontMove)
		{
			camera.transform.LocalMoveY(camera.orthographicSize);
		}
	}

	private void LateUpdate()
	{
		UpdateCameraSize();
	}

	protected virtual void UpdateCameraSize(int windowIndex = -1)
	{
		RenderTexture targetTexture = camera.targetTexture;
		float num = ((useDynamicRatio == CameraSizeCalculation.Default) ? 1.7777778f : ((targetTexture != null) ? ((float)targetTexture.width / (float)targetTexture.height) : ((float)Screen.width / (float)Screen.height)));
		if (useDynamicRatio == CameraSizeCalculation.DefaultOrLess)
		{
			num = Mathf.Min(1.7777778f, num);
		}
		float num2 = 352f / num;
		num2 /= zoom + pulseAmount;
		camera.orthographicSize = num2 / 2f;
	}

	public void Rotate(float finalAngle, float duration, Ease ease)
	{
		if (rotateCoroutine != null)
		{
			StopCoroutine(rotateCoroutine);
		}
		if (duration == 0f)
		{
			angle = finalAngle;
			base.transform.localEulerAngles = Vector3.forward * finalAngle;
		}
		else
		{
			rotateCoroutine = StartCoroutine(RotateCoroutine(finalAngle, duration, ease, Time.timeSinceLevelLoad, angle));
		}
	}

	private IEnumerator RotateCoroutine(float finalAngle, float duration, Ease ease, float t0, float initialAngle)
	{
		float t1;
		do
		{
			t1 = ((duration == 0f) ? 1f : Mathf.Clamp01((Time.timeSinceLevelLoad - t0) / duration));
			float num = (angle = DOVirtual.EasedValue(initialAngle, finalAngle, t1, ease));
			base.transform.localEulerAngles = Vector3.forward * num;
			yield return null;
		}
		while (t1 < 1f);
	}

	public Tween Zoom(float finalZoom, float duration, Ease ease)
	{
		zoomTween?.Kill();
		if (duration == 0f)
		{
			zoom = finalZoom;
			UpdateCameraSize();
			return null;
		}
		return zoomTween = DOTween.To(() => zoom, (float x) =>
		{
			zoom = x;
		}, finalZoom, duration).SetEase(ease);
	}

	public void PulseCamera(float zoomValue, float pulseTime)
	{
		pulseTween?.Kill();
		pulseAmount = 0f;
		pulseTween = DOTween.To(() => pulseAmount, (float x) =>
		{
			pulseAmount = x;
		}, zoomValue, pulseTime).SetEase(base.gc.pulseCam);
	}

	public void ClearSmoothShake()
	{
		if (smoothShake != null && smoothShake.active)
		{
			smoothShake.Complete();
		}
	}

	public void ClearRotateShake()
	{
		if (rotateShake != null && rotateShake.active)
		{
			rotateShake.Complete();
		}
	}
}
