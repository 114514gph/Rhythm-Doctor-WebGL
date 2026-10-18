using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SparklineBalloons : RDBase
{
	[Header("Params")]
	public Vector2 minMaxDuration;

	public float spawnDelay = 0.25f;

	public float minSpawnDistance = 20f;

	[Header("References")]
	public GameObject[] balloonPrefabs;

	public Transform spawnPointLeftmost;

	public Transform spawnPointRightmost;

	public Transform directionPoint;

	public Transform balloonsContainer;

	[Header("Private")]
	private float lastSpawnTime;

	private Vector2 lastSpawnPosition = Vector2.positiveInfinity;

	private List<tk2dSpriteAnimator> balloons = new List<tk2dSpriteAnimator>();

	private float time;

	private float stutterPoint;

	private float lastStutter;

	private void Update()
	{
		time += Time.deltaTime;
		if (time - lastSpawnTime >= spawnDelay)
		{
			SpawnBalloon();
			lastSpawnTime = time;
		}
	}

	private void SpawnBalloon()
	{
		float num = 0f;
		Vector2 vector;
		do
		{
			vector = Vector2.Lerp(spawnPointLeftmost.position, spawnPointRightmost.position, UnityEngine.Random.Range(0f, 1f));
			num = Vector2.Distance(vector, lastSpawnPosition);
		}
		while (num < minSpawnDistance);
		GameObject gameObject = balloonPrefabs[UnityEngine.Random.Range(0, balloonPrefabs.Length)];
		tk2dSpriteAnimator balloon = UnityEngine.Object.Instantiate(gameObject, balloonsContainer).GetComponent<tk2dSpriteAnimator>();
		balloon.PlayFromFrame(UnityEngine.Random.Range(0, 2));
		balloon.Pause();
		MeshRenderer component = balloon.GetComponent<MeshRenderer>();
		MeshRenderer component2 = gameObject.GetComponent<MeshRenderer>();
		component.sortingOrder = component2.sortingOrder;
		component.sortingLayerID = component2.sortingLayerID;
		float num2 = UnityEngine.Random.Range(0f, 1f);
		float duration = Mathf.Lerp(minMaxDuration.x, minMaxDuration.y, num2);
		float num3 = DOVirtual.EasedValue(1f, 0.5f, num2, Ease.InSine);
		Color color = new Color(num3, num3, num3);
		balloon.Sprite.color = color;
		float z = 0f - num2;
		balloons.Add(balloon);
		balloon.transform.position = vector.WithZ(z);
		Vector2 vector2 = Vector2.Lerp(spawnPointLeftmost.position, spawnPointRightmost.position, 0.5f);
		Vector2 vector3 = (Vector2)directionPoint.position - vector2;
		Vector2 v = vector + vector3;
		balloon.transform.DOMove(v.WithZ(z), duration).SetEase(Ease.OutCubic).OnComplete(() =>
		{
			balloons.Remove(balloon);
			UnityEngine.Object.Destroy(balloon.gameObject);
		});
	}

	public void PopBalloons(float percentage)
	{
		float num = 1f / percentage;
		List<tk2dSpriteAnimator> list = new List<tk2dSpriteAnimator>();
		int num2 = 0;
		for (int i = 0; i < balloons.Count; i++)
		{
			tk2dSpriteAnimator b = balloons[i];
			if (i == (int)((float)num2 * num))
			{
				b.Resume();
				b.Play(b.currentClip.name + "_pop");
				tk2dSpriteAnimator obj = b;
				obj.AnimationCompleted = (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>)Delegate.Combine(obj.AnimationCompleted, (Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>)((tk2dSpriteAnimator _, tk2dSpriteAnimationClip _) =>
				{
					UnityEngine.Object.Destroy(b.gameObject);
				}));
				num2++;
			}
			else
			{
				list.Add(b);
			}
		}
		balloons = list;
	}

	public void SetSpawnDelay(float delay)
	{
		spawnDelay = delay;
	}

	public void SetSortingOrder(int layer, int order)
	{
		GameObject[] array = balloonPrefabs;
		for (int i = 0; i < array.Length; i++)
		{
			MeshRenderer component = array[i].GetComponent<MeshRenderer>();
			component.sortingOrder = order;
			component.sortingLayerID = layer;
		}
	}

	public void Clear()
	{
		foreach (tk2dSpriteAnimator balloon in balloons)
		{
			GetTween(balloon.transform)?.Kill();
			UnityEngine.Object.Destroy(balloon.gameObject);
		}
		balloons.Clear();
	}

	public void SetStutterPoint()
	{
		stutterPoint = Time.timeSinceLevelLoad;
		lastStutter = stutterPoint;
	}

	public void Stutter()
	{
		float num = Time.timeSinceLevelLoad - lastStutter;
		foreach (tk2dSpriteAnimator balloon in balloons)
		{
			Tween tween = GetTween(balloon.transform);
			tween?.Goto(tween.position.x - num, andPlay: true);
		}
		lastStutter = Time.timeSinceLevelLoad;
	}

	public void Resume()
	{
		float num = Time.timeSinceLevelLoad - stutterPoint;
		foreach (tk2dSpriteAnimator balloon in balloons)
		{
			Tween tween = GetTween(balloon.transform);
			tween?.Goto(Mathf.Min(tween.position.x + num, tween.Duration() - 0.01f), andPlay: true);
		}
	}

	private Tween GetTween(object obj)
	{
		List<Tween> list = DOTween.TweensByTarget(obj);
		if (list == null || list.Count == 0)
		{
			return null;
		}
		return list[0];
	}
}
