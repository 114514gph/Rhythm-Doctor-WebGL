using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProceduralTree
{
	public class RDProceduralTree : RDBase
	{
		public class Branch
		{
			public Transform transform;

			public Transform segMiddle;

			public Transform segEnd;

			public Vector2 size;

			public float deviation;

			public Renderer rendMiddle;

			public Renderer rendEnd;

			public Vector3 endPosition => segEnd.position;

			public float extrusionIntensity
			{
				set
				{
					rendMiddle.material.SetFloat("_ExtrusionIntensity", value);
					rendEnd.material.SetFloat("_ExtrusionIntensity", value);
				}
			}

			public float pulseWavelength
			{
				set
				{
					rendMiddle.material.SetFloat("_PulseWavelength", value);
					rendEnd.material.SetFloat("_PulseWavelength", value);
				}
			}

			public float pulsePhaseShift
			{
				set
				{
					rendMiddle.material.SetFloat("_PulsePhaseShift", value);
					rendEnd.material.SetFloat("_PulsePhaseShift", value);
				}
			}

			public float pulseRate
			{
				set
				{
					rendMiddle.material.SetFloat("_PulseRate", value);
					rendEnd.material.SetFloat("_PulseRate", value);
				}
			}

			public Color restColor
			{
				set
				{
					rendMiddle.material.SetColor("_RestColor", value);
					rendEnd.material.SetColor("_RestColor", value);
				}
			}

			public Color pulseColor
			{
				set
				{
					rendMiddle.material.SetColor("_PulseColor", value);
					rendEnd.material.SetColor("_PulseColor", value);
				}
			}

			public void Resize(Vector2 size)
			{
				this.size = size;
				Resize();
			}

			public void Resize()
			{
				Vector3 localScale = Vector3.one * size.x;
				segEnd.localScale = localScale;
				segMiddle.localScale = new Vector3(size.x, size.y / 2f, size.x);
				segMiddle.localPosition = Vector3.up * (size.y / 2f);
				segEnd.localPosition = Vector3.up * size.y;
			}

			public Branch(Transform parent, GameObject middlePrefab, GameObject endPrefab)
			{
				transform = new GameObject("Branch").transform;
				transform.SetParent(parent, worldPositionStays: false);
				segEnd = UnityEngine.Object.Instantiate(endPrefab, transform).transform;
				segMiddle = UnityEngine.Object.Instantiate(middlePrefab, transform).transform;
				rendEnd = segEnd.GetComponentInChildren<Renderer>();
				rendMiddle = segMiddle.GetComponentInChildren<Renderer>();
			}
		}

		[Header("Components")]
		public Transform treeContainer;

		public GameObject branchMiddlePrefab;

		public GameObject branchTipPrefab;

		public Camera camera;

		public MeshRenderer renderTextureQuad;

		[Header("Config")]
		public int branchesPerIteration = 10;

		public int branchesPerDivision = 2;

		[Range(1f, 60f)]
		public float iterationsPerSecond = 6f;

		[Range(0.01f, 5f)]
		public float thickness = 1f;

		[Range(0.01f, 20f)]
		public float targetLength = 15f;

		[Range(0f, 90f)]
		public float maxDeviation = 5f;

		[Range(0f, 90f)]
		public float angle = 45f;

		[Range(-90f, 90f)]
		public float camAngle = 20f;

		[Range(0f, 500f)]
		public float camDistance = 300f;

		[Range(0f, 720f)]
		public float camDegreesPerSecond = 180f;

		[Range(0.01f, 10f)]
		public float camSpeed = 4f;

		[Range(0f, 5f)]
		public float pulseIntensity;

		[Range(0f, 10f)]
		public float pulseRate = 2f;

		[Range(0f, 5f)]
		public float pulseWavelength = 1f;

		[Range(0f, 1f)]
		public float pulsePhase;

		public Color restColor = Color.white;

		public Color pulseColor = Color.white;

		[Header("Private Variables")]
		private List<Branch> currentBranches;

		private Branch[] branchPool;

		private int branchPoolIndex;

		private float time;

		private System.Random RNG;

		private RenderTexture renderTexture;

		private void Awake()
		{
			branchPool = GenerateBranchPool(500);
			currentBranches = new List<Branch>();
			Branch branch = InstantiateBranch();
			branch.Resize(new Vector2(1f, 0f));
			currentBranches.Add(branch);
			LerpCamera(branch.endPosition + Vector3.up * 80f, camDistance, 1f);
		}

		public void InitRNG(int seed)
		{
			RNG = new System.Random(seed);
		}

		private void OnDestroy()
		{
			if ((bool)renderTexture)
			{
				renderTexture.Release();
				UnityEngine.Object.Destroy(renderTexture);
			}
		}

		public void InitRenderTexture()
		{
			if (!renderTexture)
			{
				renderTexture = RDBase.Vfx.CreateRenderTexture(1);
				camera.targetTexture = renderTexture;
				renderTextureQuad.material.mainTexture = renderTexture;
			}
		}

		private Branch[] GenerateBranchPool(int amount)
		{
			Branch[] array = new Branch[amount];
			for (int i = 0; i < amount; i++)
			{
				array[i] = new Branch(treeContainer, branchMiddlePrefab, branchTipPrefab);
			}
			return array;
		}

		private Branch InstantiateBranch()
		{
			Branch result = branchPool[branchPoolIndex];
			branchPoolIndex = (branchPoolIndex + 1) % branchPool.Length;
			return result;
		}

		public void Spread(int amount)
		{
			List<Branch> list = new List<Branch>();
			foreach (Branch item in currentBranches.OrderBy((Branch x) => RNG.Next()).Take(amount))
			{
				for (int num = 0; num < branchesPerDivision; num++)
				{
					float num2 = (float)RNG.NextDouble() * 720f;
					Branch branch = InstantiateBranch();
					branch.transform.position = item.endPosition;
					branch.transform.eulerAngles = new Vector3(angle, num2, 0f);
					branch.Resize(new Vector2(thickness, 0f));
					list.Add(branch);
					float deviation = (float)RNG.NextDouble() * (maxDeviation - item.deviation);
					branch.deviation = deviation;
				}
			}
			currentBranches = list;
		}

		private void Update()
		{
			if (iterationsPerSecond <= 0f)
			{
				return;
			}
			time += Time.deltaTime;
			float num = 1f / iterationsPerSecond;
			Vector3 vector = Vector3.one * float.MaxValue;
			Vector3 vector2 = Vector3.one * float.MinValue;
			foreach (Branch currentBranch in currentBranches)
			{
				float num2 = (targetLength + (currentBranch.deviation - maxDeviation / 2f)) / num * Time.deltaTime;
				currentBranch.size += Vector2.up * num2;
				currentBranch.Resize();
				Vector3 endPosition = currentBranch.endPosition;
				if (endPosition.x > vector2.x)
				{
					vector2.x = endPosition.x;
				}
				if (endPosition.y > vector2.y)
				{
					vector2.y = endPosition.y;
				}
				if (endPosition.z > vector2.z)
				{
					vector2.z = endPosition.z;
				}
				if (endPosition.x < vector.x)
				{
					vector.x = endPosition.x;
				}
				if (endPosition.y < vector.y)
				{
					vector.y = endPosition.y;
				}
				if (endPosition.z < vector.z)
				{
					vector.z = endPosition.z;
				}
				currentBranch.extrusionIntensity = pulseIntensity;
				currentBranch.pulseRate = pulseRate;
				currentBranch.pulseWavelength = pulseWavelength;
				currentBranch.pulsePhaseShift = pulsePhase;
				currentBranch.restColor = restColor;
				currentBranch.pulseColor = pulseColor;
			}
			if (time >= num)
			{
				time %= num;
				Spread(branchesPerIteration);
			}
			Vector3 target = (vector + vector2) / 2f;
			float lerpAmount = 1f - Mathf.Pow(0.5f, Time.deltaTime * camSpeed);
			Vector3 vector3 = vector2 - vector;
			float distance = (camDistance = Mathf.Max(vector3.x, Mathf.Max(vector3.y, vector3.z)));
			Transform parent = camera.transform.parent;
			parent.eulerAngles = new Vector3(camAngle, parent.eulerAngles.y + camDegreesPerSecond * Time.deltaTime, 0f);
			LerpCamera(target, distance, lerpAmount);
		}

		private void LerpCamera(Vector3 target, float distance, float lerpAmount)
		{
			Transform obj = camera.transform;
			Transform parent = obj.parent;
			parent.position = Vector3.Lerp(parent.position, target, lerpAmount);
			obj.localPosition = Vector3.Lerp(obj.localPosition, Vector3.back * distance, lerpAmount);
		}
	}
}
