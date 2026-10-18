using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseballRainSpawner : RDBase
{
	public GameObject baseballPrefab;

	public Vector3 boundsMin;

	public Vector3 boundsMax;

	public float ballsPerSecond;

	public Transform baseballHolder;

	[NonSerialized]
	public List<GameObject> objList = new List<GameObject>();

	private float timer;

	private void Update()
	{
		ballsPerSecond = Mathf.Clamp(ballsPerSecond, 0f, 1000f);
		if (!(ballsPerSecond < 0f))
		{
			float num = 1f / ballsPerSecond;
			timer += Time.deltaTime;
			if (timer >= num)
			{
				timer = 0f;
				SpawnBall();
			}
		}
	}

	private void SpawnBall()
	{
		Perspective2DSprite ball = UnityEngine.Object.Instantiate(baseballPrefab, baseballHolder).GetComponent<Perspective2DSprite>();
		ball.gameObject.SetActive(value: true);
		Vector3 position = new Vector3(UnityEngine.Random.Range(boundsMin.x, boundsMax.x), UnityEngine.Random.Range(boundsMin.y, boundsMax.y), UnityEngine.Random.Range(boundsMin.z, boundsMax.z));
		ball.position = position;
		ball.GetComponent<FallingBaseball>().onDestroy = () =>
		{
			objList.Remove(ball.gameObject);
		};
		objList.Add(ball.gameObject);
	}

	public void Kill()
	{
		foreach (GameObject obj in objList)
		{
			UnityEngine.Object.Destroy(obj);
		}
		objList.Clear();
	}
}
