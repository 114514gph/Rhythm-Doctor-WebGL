using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class RDTrain : RDEnvironment
{
	private const float FrameDuration = 0.15f;

	public bool isDay;

	public bool trainActive;

	private bool _xReversed;

	private bool prevReversed;

	[Header("Day Night")]
	public Color nightColor;

	public GameObject dayContainer;

	public GameObject nightContainer;

	public tk2dSprite[] nightTintSprites;

	public tk2dTiledSprite[] nightTintTiledSprites;

	[Header("Train")]
	public Transform trainDay;

	public Transform trainNight;

	public float trainYDistance;

	[Header("Wheels Link")]
	public Transform[] wheelsLink;

	public float wheelsLinkAnimDuration;

	public float wheelsLinkXDistance;

	public float wheelsLinkYDistance;

	public AnimationCurve wheelsLinkXCurve;

	public AnimationCurve wheelsLinkYCurve;

	public Transform wheels;

	[Header("Tracks")]
	public Transform tracks;

	public float tracksXDistance;

	[Header("Sand")]
	public Transform sand;

	public float sandXDistance;

	[Header("Forefround container")]
	public Transform foregroundContainer;

	public float foregroundContainerXDistance;

	[Header("Tumbleweed")]
	public Transform tumbleweed;

	public float tumbleweedXDistance;

	public float tumbleweedMinFrameAnimDuration;

	public float tumbleweedMaxFrameAnimDuration;

	public float tumbleweedMinY;

	public float tumbleweedMaxY;

	public float tumbleweedMinScale;

	public float tumbleweedMaxScale;

	public float tumbleweedMinDelay;

	public float tumbleweedMaxDelay;

	private float tumbleweedStartX;

	private float tumbleweedTargetX;

	private float tumbleweedtimeToPlay;

	private bool tumbleweedAnimated;

	[Header("Sandhills")]
	public Transform sandhills;

	public float sandhillsXDistance;

	public AnimationCurve sandhillsAnimCurve;

	[Header("Constellations")]
	public tk2dSprite[] constellations;

	public AnimationCurve constellationsAnimCurve;

	[Header("Sky")]
	public tk2dSprite daySky;

	public tk2dSprite nightSky;

	[Header("HeadlightGlows")]
	public tk2dSprite headlightGlows;

	public AnimationCurve headlightGlowsAnimCurve;

	[NonSerialized]
	public Transform rowContainer;

	private Tween trainTween;

	[NonSerialized]
	public Tween rowTween;

	private List<Tween> xTweens = new List<Tween>();

	private List<Tween> linkTweens = new List<Tween>();

	private Tween tumbleweedTween;

	private int startFrame;

	public bool xReversed
	{
		get
		{
			return _xReversed;
		}
		set
		{
			_xReversed = value;
			if (_xReversed != prevReversed)
			{
				UpdateReversedStatus();
			}
			prevReversed = _xReversed;
		}
	}

	protected override void Start()
	{
		base.Start();
		tumbleweedStartX = tumbleweed.localPosition.x;
		tumbleweedTargetX = tumbleweed.localPosition.x + tumbleweedXDistance;
		StartCoroutine(PlayAnimation());
	}

	private IEnumerator PlayAnimation()
	{
		yield return null;
		float num = 0f;
		tk2dSprite[] array = constellations;
		foreach (tk2dSprite constellation in array)
		{
			DOTween.To(() => constellation.color, (Color x) =>
			{
				constellation.color = x;
			}, Color.clear, 3f).SetEase(constellationsAnimCurve).SetLoops(-1)
				.SetDelay(num);
			num += 0.15f;
		}
		DOTween.To(() => headlightGlows.color, (Color x) =>
		{
			headlightGlows.color = x;
		}, Color.clear, 3f).SetEase(headlightGlowsAnimCurve).SetLoops(-1);
		float endValue = sandhills.localPosition.x + sandhillsXDistance;
		xTweens.Add(sandhills.DOLocalMoveX(endValue, 1.5f, snapping: true).SetLoops(-1));
		float endValue2 = tracks.localPosition.x + tracksXDistance;
		xTweens.Add(tracks.DOLocalMoveX(endValue2, 0.3f, snapping: true).SetLoops(-1));
		float endValue3 = sand.localPosition.x + sandXDistance;
		xTweens.Add(sand.DOLocalMoveX(endValue3, 1.5f, snapping: true).SetEase(Ease.Linear).SetLoops(-1));
		float endValue4 = foregroundContainer.localPosition.x + foregroundContainerXDistance;
		xTweens.Add(foregroundContainer.DOLocalMoveX(endValue4, 1.5f, snapping: true).SetEase(Ease.Linear).SetLoops(-1));
		Vector3 endValue5 = new Vector3(0f, 0f, -360f);
		xTweens.Add(tumbleweed.DORotate(endValue5, 0.75f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear).SetLoops(-1));
		float endValue6 = trainDay.localPosition.y - trainYDistance;
		trainTween = trainDay.DOLocalMoveY(endValue6, 0.3f, snapping: true).SetLoops(-1);
		if (rowContainer != null && isDay && trainActive)
		{
			rowTween = rowContainer.DOLocalMoveY(rowContainer.localPosition.y - trainYDistance, 0.3f, snapping: true).SetLoops(-1);
		}
		float num2 = wheelsLinkAnimDuration / 3f;
		float num3 = 0f;
		int num4 = 0;
		Transform[] array2 = wheelsLink;
		foreach (Transform transform in array2)
		{
			float endValue7 = transform.localPosition.x - wheelsLinkXDistance;
			linkTweens.Add(transform.DOLocalMoveX(endValue7, wheelsLinkAnimDuration, snapping: true).SetEase(wheelsLinkXCurve).SetLoops(-1)
				.SetDelay(num3));
			float endValue8 = transform.localPosition.y + wheelsLinkYDistance;
			linkTweens.Add(transform.DOLocalMoveY(endValue8, wheelsLinkAnimDuration, snapping: true).SetEase(wheelsLinkYCurve).SetLoops(-1)
				.SetDelay(num3));
			num3 += num2;
			num4++;
			if (num4 == 3)
			{
				num3 = 0f;
				num4 = 0;
			}
		}
	}

	private void Update()
	{
		if (Time.time >= tumbleweedtimeToPlay && !tumbleweedAnimated)
		{
			AnimateTumbleweed();
		}
	}

	private void AnimateTumbleweed()
	{
		tumbleweedAnimated = true;
		tumbleweed.localPosition = tumbleweed.localPosition.WithX(tumbleweedStartX);
		float num = UnityEngine.Random.Range(tumbleweedMinY, tumbleweedMaxY);
		tumbleweed.localPosition = tumbleweed.localPosition.WithY(num);
		float num2 = tumbleweedMaxY - tumbleweedMinY;
		float num3 = tumbleweedMinScale + (tumbleweedMaxScale - tumbleweedMinScale * ((num2 - (num - tumbleweedMinY)) / num2));
		tumbleweed.localScale = Vector3.one * num3;
		float num4 = 0.15f * UnityEngine.Random.Range(tumbleweedMinFrameAnimDuration, tumbleweedMaxFrameAnimDuration);
		tumbleweedTween = tumbleweed.DOLocalMoveX(tumbleweedTargetX, num4).SetEase(Ease.Linear).OnComplete(() =>
		{
			tumbleweedtimeToPlay = Time.time + UnityEngine.Random.Range(tumbleweedMinDelay, tumbleweedMaxDelay);
			tumbleweedAnimated = false;
		})
			.OnRewind(() =>
			{
				tumbleweedTween.Kill(complete: true);
			});
		if (xReversed)
		{
			tumbleweedTween.fullPosition = num4 * 0.99f;
			tumbleweedTween.Flip();
		}
	}

	public void SetNightMode(bool night)
	{
		isDay = !night;
		Color color = (night ? nightColor : Color.white);
		tk2dSprite[] array = nightTintSprites;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].color = color;
		}
		tk2dTiledSprite[] array2 = nightTintTiledSprites;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].color = color;
		}
		nightContainer.gameObject.SetActive(night);
		dayContainer.gameObject.SetActive(!night);
		CheckForBumpyRows();
	}

	public void ToggleTrain(bool activeTrain)
	{
		trainActive = activeTrain;
		tracks.gameObject.SetActive(activeTrain);
		foreach (Transform wheel in wheels)
		{
			wheel.gameObject.SetActive(activeTrain);
		}
		foreach (Transform item in dayContainer.transform)
		{
			if (item.GetComponent<tk2dSprite>() == null || item.GetComponent<tk2dSprite>() != daySky)
			{
				item.gameObject.SetActive(activeTrain);
			}
		}
		foreach (Transform item2 in nightContainer.transform)
		{
			if (item2.GetComponent<tk2dSprite>() == null || item2.GetComponent<tk2dSprite>() != nightSky)
			{
				item2.gameObject.SetActive(activeTrain);
			}
		}
		CheckForBumpyRows();
	}

	public void CheckForBumpyRows()
	{
		if (trainTween != null && trainActive && isDay && rowContainer != null)
		{
			rowTween = rowContainer.DOLocalMoveY(rowContainer.localPosition.y - trainYDistance, 0.3f, snapping: true).SetLoops(-1);
			rowTween.Goto(trainTween.position.x, andPlay: true);
		}
		else if (rowTween != null)
		{
			rowTween.Kill();
		}
	}

	private void UpdateReversedStatus()
	{
		if (xReversed)
		{
			foreach (Tween xTween in xTweens)
			{
				float fullPosition = xTween.fullPosition;
				xTween.PlayBackwards();
				xTween.fullPosition = fullPosition + xTween.Duration(includeLoops: false) * 100000f;
			}
			foreach (Tween linkTween in linkTweens)
			{
				float fullPosition2 = linkTween.fullPosition;
				linkTween.PlayBackwards();
				linkTween.fullPosition = fullPosition2 + linkTween.Duration(includeLoops: false) * 100000f;
			}
			if (tumbleweedTween != null)
			{
				_ = tumbleweedTween.fullPosition;
				tumbleweedTween.PlayBackwards();
			}
			return;
		}
		foreach (Tween xTween2 in xTweens)
		{
			float fullPosition3 = xTween2.fullPosition;
			xTween2.PlayForward();
			xTween2.fullPosition = fullPosition3 % xTween2.Duration(includeLoops: false);
		}
		foreach (Tween linkTween2 in linkTweens)
		{
			float fullPosition4 = linkTween2.fullPosition;
			linkTween2.PlayForward();
			linkTween2.fullPosition = fullPosition4 % linkTween2.Duration(includeLoops: false);
		}
		if (tumbleweedTween != null)
		{
			_ = tumbleweedTween.fullPosition;
			tumbleweedTween.PlayForward();
		}
	}
}
