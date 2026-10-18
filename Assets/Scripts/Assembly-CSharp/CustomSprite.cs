using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CustomSprite : RDBase
{
	public CustomAnimation customAnimation;

	public Tween positionTweenX;

	public Tween positionTweenY;

	public Tween scaleTweenX;

	public Tween scaleTweenY;

	public Tween pivotTweenX;

	public Tween pivotTweenY;

	public Tween borderTween;

	public Tween tintTween;

	public Tween opacityTween;

	public Tween offsetTweenX;

	public Tween offsetTweenY;

	public Tween tilingTweenX;

	public Tween tilingTweenY;

	public Tween speedTweenX;

	public Tween speedTweenY;

	private scrExecuteOnPeriodicBeat pulseEventX;

	private scrExecuteOnPeriodicBeat pulseEventY;

	private Tween pulseOrScrollTweenX;

	private Tween pulseOrScrollTweenY;

	[NonSerialized]
	public Renderer renderer;

	[NonSerialized]
	public MonoBehaviour monoBehaviour;

	[NonSerialized]
	public scrVolumeTracker borderGlowTracker;

	private Coroutine rotateCoroutine;

	private float angle;

	private int beatsOnThisExpression;

	[NonSerialized]
	public Vector2 tileOffset;

	[NonSerialized]
	public Vector2 tileAmount = Vector2.one;

	private Vector2 speed;

	private Vector2 pulseOffset;

	private Vector2 scrollOffset;

	private TilingType? tilingTypeX;

	private TilingType? tilingTypeY;

	public void Awake()
	{
		renderer = GetComponent<MeshRenderer>();
		monoBehaviour = scnGame.instance.GetComponent<MonoBehaviour>();
	}

	public void Rotate(float finalAngle, float duration, Ease ease)
	{
		if (rotateCoroutine != null)
		{
			monoBehaviour.StopCoroutine(rotateCoroutine);
		}
		if (duration == 0f)
		{
			angle = finalAngle;
			base.transform.localEulerAngles = Vector3.forward * finalAngle;
		}
		else
		{
			rotateCoroutine = monoBehaviour.StartCoroutine(RotateCoroutine(finalAngle, duration, ease));
		}
	}

	private IEnumerator RotateCoroutine(float finalAngle, float duration, Ease ease)
	{
		float t0 = Time.timeSinceLevelLoad;
		float initialAngle = angle;
		float t1;
		do
		{
			t1 = Mathf.Clamp01((Time.timeSinceLevelLoad - t0) / duration);
			float num = (angle = DOVirtual.EasedValue(initialAngle, finalAngle, t1, ease));
			base.transform.localEulerAngles = Vector3.forward * num;
			yield return null;
		}
		while (t1 < 1f);
	}

	private void Update()
	{
		UpdateTile();
	}

	public void LateUpdate()
	{
		if (base.conductor.onBeat && customAnimation.currentClip != null)
		{
			beatsOnThisExpression++;
			if (customAnimation.isLoopOnBeat)
			{
				customAnimation.AttemptLoopOnBeat();
			}
			else if (customAnimation.currentClip.fps == 0f && beatsOnThisExpression == 3)
			{
				PlayExpression("neutral");
			}
		}
	}

	public void PlayExpression(string expression, float speedMultiplier = 1f)
	{
		string text = expression;
		text = expression;
		if (!(text == ""))
		{
			Dictionary<string, CustomAnimationClip> clips = customAnimation.data.clips;
			if (clips.ContainsKey(text))
			{
				CustomAnimationClip clip = clips[text];
				customAnimation.PlayFromClip(clip, speedMultiplier);
				beatsOnThisExpression = 0;
			}
			else
			{
				printe("animation for custom character " + text + " was not found");
			}
		}
	}

	private void UpdateTile()
	{
		scrollOffset = new Vector2(scrollOffset.x + ((tilingTypeX == TilingType.Scroll) ? (speed.x * RDTime.speed * Time.deltaTime) : 0f), scrollOffset.y + ((tilingTypeY == TilingType.Scroll) ? (speed.y * RDTime.speed * Time.deltaTime) : 0f));
		renderer.material.mainTextureOffset = new Vector2(0f - ((!tilingTypeX.HasValue) ? 0f : ((tilingTypeX == TilingType.Scroll) ? scrollOffset.x : pulseOffset.x)) + tileOffset.x, 0f - ((!tilingTypeY.HasValue) ? 0f : ((tilingTypeY == TilingType.Scroll) ? scrollOffset.y : pulseOffset.y)) + tileOffset.y);
		renderer.material.mainTextureScale = tileAmount;
	}

	public void SetTileSpeedOff()
	{
		pulseOrScrollTweenX?.Kill();
		pulseEventX?.Remove();
		pulseOrScrollTweenY?.Kill();
		pulseEventY?.Remove();
	}

	public void SetTileSpeedX(float speedX, TilingType tilingType, float duration, Ease ease, float pulseIntervalBeats, float pulseStartBeat)
	{
		TilingType? tilingType2 = tilingTypeY;
		tilingTypeX = tilingType;
		pulseOrScrollTweenX?.Kill();
		pulseEventX?.Remove();
		Texture texture = renderer.material.mainTexture;
		if (tilingType == TilingType.Scroll)
		{
			if (tilingType2 != tilingTypeY)
			{
				speed.x = 0f;
			}
			pulseOrScrollTweenX = DOTween.To(() => speed.x, (float s) =>
			{
				speed.x = s;
			}, speedX / (float)texture.width, duration).SetEase(ease);
			return;
		}
		if (tilingType2 != tilingTypeY)
		{
			pulseOffset.x = 0f;
		}
		speed.x = speedX;
		pulseEventX = scrExecuteOnPeriodicBeat.Add(() =>
		{
			pulseOrScrollTweenX?.Kill(complete: true);
			Vector2 currentOffset = new Vector2(pulseOffset.x, pulseOffset.y);
			pulseOrScrollTweenX = DOTween.To(() => pulseOffset.x, (float v) =>
			{
				pulseOffset.x = v;
			}, currentOffset.x + speed.x / (float)texture.width, duration).SetEase(ease).OnComplete(() =>
			{
				pulseOffset.x = currentOffset.x + speed.x / (float)texture.width;
			});
		}, pulseIntervalBeats, pulseStartBeat);
	}

	public void SetTileSpeedY(float speedY, TilingType tilingType, float duration, Ease ease, float pulseIntervalBeats, float pulseStartBeat)
	{
		TilingType? tilingType2 = tilingTypeY;
		tilingTypeY = tilingType;
		pulseOrScrollTweenY?.Kill();
		pulseEventY?.Remove();
		Texture texture = renderer.material.mainTexture;
		if (tilingType == TilingType.Scroll)
		{
			if (tilingType2 != tilingTypeY)
			{
				speed.y = 0f;
			}
			pulseOrScrollTweenY = DOTween.To(() => speed.y, (float s) =>
			{
				speed.y = s;
			}, speedY / (float)texture.width, duration).SetEase(ease);
			return;
		}
		if (tilingType2 != tilingTypeY)
		{
			pulseOffset.y = 0f;
		}
		speed.y = speedY;
		pulseEventY = scrExecuteOnPeriodicBeat.Add(() =>
		{
			pulseOrScrollTweenY?.Kill(complete: true);
			Vector2 currentOffset = new Vector2(pulseOffset.y, pulseOffset.y);
			pulseOrScrollTweenY = DOTween.To(() => pulseOffset.y, (float v) =>
			{
				pulseOffset.y = v;
			}, currentOffset.y + speed.y / (float)texture.height, duration).SetEase(ease).OnComplete(() =>
			{
				pulseOffset.y = currentOffset.y + speed.y / (float)texture.height;
			});
		}, pulseIntervalBeats, pulseStartBeat);
	}
}
