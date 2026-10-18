using System;
using System.Collections.Generic;
using UnityEngine;

public class BubbleMask : RDBase
{
	public GameObject bubblePrefab;

	public int resolution = 10;

	public bool invert;

	private bool _playing;

	public List<Collider2D> shapes = new List<Collider2D>();

	[NonSerialized]
	public ParticleSystem[] bubbles;

	private int gridSize;

	public bool playing
	{
		get
		{
			return _playing;
		}
		set
		{
			_playing = value;
			Play(value);
		}
	}

	private void Awake()
	{
		SpawnShapes();
		playing = true;
	}

	private void SpawnShapes()
	{
		Time.timeScale = 1f;
		int num = resolution + 6;
		gridSize = 352 / resolution;
		int num2 = 198 / gridSize + 2;
		int num3 = num * num2;
		ParticleSystem.ShapeModule shape = bubblePrefab.GetComponent<ParticleSystem>().shape;
		shape.scale = Vector3.one * gridSize;
		bubbles = new ParticleSystem[num3];
		for (int i = 0; i < num3; i++)
		{
			int num4 = i % num;
			int num5 = i / num;
			Vector3 position = new Vector3(num4, num5) * gridSize + base.transform.position;
			ParticleSystem component = UnityEngine.Object.Instantiate(bubblePrefab, position, Quaternion.identity, base.transform).GetComponent<ParticleSystem>();
			ParticleSystem.MainModule main = component.main;
			component.time = UnityEngine.Random.Range(0f, 5f);
			main.simulationSpeed = UnityEngine.Random.Range(0.8f, 1.2f);
			bubbles[i] = component;
		}
	}

	private void Play(bool play)
	{
		ParticleSystem[] array = bubbles;
		foreach (ParticleSystem particleSystem in array)
		{
			if (play)
			{
				particleSystem.Play();
			}
			else
			{
				particleSystem.Pause();
			}
		}
	}

	private void Update()
	{
		if (!playing)
		{
			return;
		}
		ParticleSystem[] array = bubbles;
		foreach (ParticleSystem particleSystem in array)
		{
			Vector3 position = particleSystem.transform.position;
			bool flag = false;
			foreach (Collider2D shape in shapes)
			{
				if (shape.OverlapPoint(position))
				{
					flag = true;
					break;
				}
			}
			ParticleSystem.EmissionModule emission = particleSystem.emission;
			emission.enabled = flag != invert;
		}
	}
}
