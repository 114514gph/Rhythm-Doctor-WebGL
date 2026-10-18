using DG.Tweening;
using UnityEngine;

public struct WindowDancePresetInfo
{
	public WindowDancePreset preset;

	public SamePresetBehavior samePresetBehavior;

	public Vector2 pos;

	public Vector2 amplitude;

	public float speed;

	public float angle;

	public float frequency;

	public float period;

	public EasingType easeType;

	public Ease subEase;

	public float easeDur;

	public Ease ease;

	public static WindowDancePresetInfo Default()
	{
		return new WindowDancePresetInfo
		{
			preset = WindowDancePreset.Move,
			samePresetBehavior = SamePresetBehavior.Keep,
			pos = new Vector2(0.5f, 0.5f),
			speed = 0f,
			amplitude = Vector2.zero,
			angle = 0f,
			frequency = 0f,
			period = 0f,
			easeType = EasingType.Repeat,
			subEase = Ease.Linear,
			easeDur = 0f,
			ease = Ease.Linear
		};
	}

	public void CopyFrom(WindowDancePresetInfo other)
	{
		preset = other.preset;
		samePresetBehavior = other.samePresetBehavior;
		pos = other.pos;
		angle = other.angle;
		speed = other.speed;
		amplitude = other.amplitude;
		frequency = other.frequency;
		period = other.period;
		easeType = other.easeType;
		subEase = other.subEase;
		easeDur = other.easeDur;
		ease = other.ease;
	}
}
