using System;
using DG.Tweening;
using UnityEngine;

public class EdegaEye : MonoBehaviour
{
	public Sprite[] sprites;

	public Sprite[] masks;

	public Transform border;

	public Transform pupil;

	public SpriteRenderer backgroundColor;

	public Vector3 radius;

	public Vector3 targetVector;

	public Transform targetTransform;

	[NonSerialized]
	public Vector3 position;

	private int blinkFrame;

	private Tween blinkTween;

	private Tween borderTween;

	private Tween pupilTween;

	private Tween backgroundTween;

	private SpriteMask mask;

	[NonSerialized]
	public new Transform transform;

	private Transform parent;

	private SpriteRenderer borderSpriteRenderer;

	private SpriteRenderer pupilSpriteRenderer;

	private Vector3 radiusReciprocal;

	public bool closed { get; private set; }

	private void Awake()
	{
		transform = base.transform;
		parent = transform.parent;
		mask = GetComponent<SpriteMask>();
		borderSpriteRenderer = border.GetComponent<SpriteRenderer>();
		pupilSpriteRenderer = pupil.GetComponent<SpriteRenderer>();
		radiusReciprocal = new Vector3(1f / radius.x, 1f / radius.y, 1f / radius.z);
	}

	private void LateUpdate()
	{
		if ((bool)targetTransform)
		{
			LookAt(parent.InverseTransformPoint(targetTransform.position));
		}
		else
		{
			LookAt(targetVector);
		}
	}

	private void LookAt(Vector3 target)
	{
		Vector3 vector = Vector3.Scale(target - transform.localPosition, radiusReciprocal);
		float num = Mathf.Clamp01(vector.magnitude);
		Vector3 v = Vector3.Scale(vector.normalized * num, radius);
		pupil.transform.localPosition = Vector3Int.RoundToInt(v);
	}

	public void ToggleBlink()
	{
		DoBlink(!closed);
	}

	public void SetFrame(int frame)
	{
		blinkFrame = frame;
		borderSpriteRenderer.sprite = sprites[frame];
		mask.sprite = masks[frame];
	}

	public Tween DoBlink(bool closed, bool instant = false)
	{
		float duration = (instant ? 0f : 0.25f);
		this.closed = closed;
		blinkTween?.Kill();
		return blinkTween = DOTween.To(() => blinkFrame, (int x) =>
		{
			SetFrame(x);
		}, closed ? (sprites.Length - 1) : 0, duration).SetEase(Ease.Linear).Done();
	}

	public Tween TweenColor(bool isPupil, Color color, float duration, Ease ease)
	{
		if (isPupil)
		{
			pupilTween?.Kill();
			return pupilTween = pupilSpriteRenderer.DOColor(color, duration).SetEase(ease).Done();
		}
		borderTween?.Kill();
		return borderTween = borderSpriteRenderer.DOColor(color, duration).SetEase(ease).Done();
	}

	public Tween TweenBackgroundColor(Color color, float duration, Ease ease)
	{
		backgroundTween?.Kill();
		return backgroundTween = backgroundColor.DOColor(color, duration).SetEase(ease).Done();
	}
}
