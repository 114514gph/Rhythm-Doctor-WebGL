using System.Collections.Generic;
using UnityEngine;

public class RDParticleSystem : RDBase
{
	public RDParticle particlePrefab;

	public string spriteName;

	public string animationName;

	public float durationBetweenSpawns;

	public bool isStreaming;

	public float streamSpawnCount;

	public float particleLifetime;

	public bool diesWhenPassingScreen;

	public float baseSpeed;

	public float speedMultiplier;

	public bool parallax;

	public float parallaxPower;

	public Range yPosition;

	public Range scale;

	public Range angularSpeed;

	public Range startAngle;

	public string sortingLayer;

	public int sortingOrder;

	public bool startFromRight;

	public float alpha;

	public bool randomColors;

	public bool tintVertexes;

	public bool sortingOrderFollowsScale;

	public bool negativeSortingOrder;

	private Camera roomCamera;

	private int mParticlesSpawned;

	private float mTimeElapsed;

	private string mSpriteName;

	private string mAnimationName;

	private string mAnimationSheetName;

	public List<RDParticle> mParticles;

	private void Awake()
	{
		mParticles = new List<RDParticle>();
		durationBetweenSpawns = 0.5f;
		sortingLayer = RDSortingLayer.BackgroundParticles.ToString();
		sortingOrder = 11;
		sortingOrderFollowsScale = true;
		diesWhenPassingScreen = true;
		particleLifetime = 5f;
		speedMultiplier = 1f;
		startAngle = new Range(0f, 360f);
		alpha = 1f;
	}

	private void Update()
	{
		if (streamSpawnCount > 0f)
		{
			isStreaming = true;
		}
		if (!isStreaming)
		{
			return;
		}
		mTimeElapsed += Time.deltaTime * speedMultiplier;
		float num = mTimeElapsed;
		float num2 = Mathf.Max(0.001f, durationBetweenSpawns);
		if (!(num / num2 > (float)(mParticlesSpawned + 1)))
		{
			return;
		}
		SpawnParticle();
		if (streamSpawnCount != 0f)
		{
			streamSpawnCount--;
			if (streamSpawnCount == 0f)
			{
				isStreaming = false;
			}
		}
	}

	private void OnGUI()
	{
	}

	public void SpawnParticles(int numberOfParticles)
	{
		for (int i = 0; i < numberOfParticles; i++)
		{
			SpawnParticle();
		}
	}

	private void SpawnParticle()
	{
		RDParticle component = Object.Instantiate(particlePrefab).GetComponent<RDParticle>();
		component.Setup(mAnimationSheetName, this);
		if (mSpriteName != null)
		{
			component.spriteName = mSpriteName;
		}
		component.gameObject.name = mSpriteName;
		component.transform.parent = base.transform;
		float num = scale.RandomValue();
		component.sprite.scale = new Vector3(num, num, num);
		component.transform.localPosition = new Vector3((0f - component.sprite.width) * 0.5f, yPosition.RandomValue(), 0f - num);
		if (startFromRight)
		{
			component.transform.localPosition = new Vector3(RDBase.Vfx.RDWidth + component.sprite.width * 0.5f, yPosition.RandomValue(), 0f - num);
		}
		component.angularSpeed = angularSpeed.RandomValue();
		component.transform.Rotate(Vector3.forward * startAngle.RandomValue());
		component.speed = (parallax ? (baseSpeed * Mathf.Pow(num, parallaxPower)) : baseSpeed);
		if (startFromRight)
		{
			component.speed *= -1f;
		}
		component.GetComponent<MeshRenderer>().sortingLayerName = sortingLayer;
		if (negativeSortingOrder)
		{
			component.sprite.SortingOrder = -Mathf.FloorToInt(num * 1000f);
		}
		else if (sortingOrderFollowsScale)
		{
			component.sprite.SortingOrder = Mathf.FloorToInt(num * 1000f);
		}
		else
		{
			component.sprite.SortingOrder = sortingOrder;
		}
		component.diesWhenPassingScreen = diesWhenPassingScreen;
		component.lifetime = particleLifetime;
		if (mAnimationName != null)
		{
			component.animator.Play(mAnimationName);
		}
		if (alpha != 1f)
		{
			component.sprite.color = component.sprite.color.WithAlpha(alpha);
		}
		if (randomColors)
		{
			Color randomColor = RDBase.Vfx.GetRandomColor();
			component.SetColor(randomColor);
			component.sprite.color = randomColor.WithAlpha(alpha);
		}
		if (!tintVertexes)
		{
			component.LateUpdate();
		}
		component.isTintable = false;
		mParticles.Add(component);
		mParticlesSpawned++;
	}

	public RDParticleSystem Setup(string spriteName, int room, string animationSheet = null, string animationName = null, float baseSpeed = 50f)
	{
		mSpriteName = spriteName;
		mAnimationName = animationName;
		mAnimationSheetName = animationSheet;
		this.baseSpeed = baseSpeed;
		parallax = true;
		parallaxPower = 2.5f;
		roomCamera = RDBase.Vfx.rooms[room].camera;
		base.transform.parent = roomCamera.transform;
		Vector2 vector = roomCamera.RDFullNativeResolution();
		yPosition = new Range((0f - vector.y) * 0.5f, vector.y * 0.5f);
		scale = new Range(1f, 2f);
		base.transform.localPosition = new Vector3((0f - vector.x) * 0.5f, 0f, 100f);
		return this;
	}

	public void StartStream(float durationBetweenSpawns = -1f, int count = 0)
	{
		isStreaming = true;
		if (durationBetweenSpawns != -1f)
		{
			this.durationBetweenSpawns = durationBetweenSpawns;
		}
		streamSpawnCount = count;
	}

	public void StopStream()
	{
		streamSpawnCount = 0f;
		isStreaming = false;
	}

	public void KillAllParticles()
	{
		foreach (RDParticle mParticle in mParticles)
		{
			mParticle.Kill();
		}
	}
}
