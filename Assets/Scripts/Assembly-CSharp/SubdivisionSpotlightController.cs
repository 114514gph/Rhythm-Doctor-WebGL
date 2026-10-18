using System;
using DG.Tweening;
using UnityEngine;

public class SubdivisionSpotlightController : MonoBehaviour
{
	private class Ray
	{
		public Transform tr;

		public Renderer renderer;

		public Material mat;

		public float alpha;

		public Tween alphaTween;

		public Tween Fade(float toAlpha, float duration)
		{
			return alphaTween = DOTween.To(() => alpha, (float a) =>
			{
				alpha = a;
			}, toAlpha, duration);
		}
	}

	[Header("Components")]
	[SerializeField]
	private GameObject rayPrefab;

	[SerializeField]
	private GameObject spotlightPrefab;

	[SerializeField]
	private Transform lightsContainer;

	[SerializeField]
	private Renderer quad;

	[SerializeField]
	private Camera cam;

	[Header("Properties")]
	[SerializeField]
	private Color[] spotlightColors;

	[SerializeField]
	private Gradient rayGradient;

	[SerializeField]
	private AnimationCurve sizeCurve;

	[SerializeField]
	private AnimationCurve widthCurve;

	[SerializeField]
	private AnimationCurve alphaCurve;

	[NonSerialized]
	[Header("Public")]
	public float raysAlpha = 1f;

	[NonSerialized]
	public float speed = 1f;

	[Header("Const")]
	private const float rayDuration = 1f;

	private const int rayCount = 20;

	private const int maxSubdivisions = 8;

	[Header("Private")]
	private float time;

	private uint currentSubdivID;

	private Entity parentEntity;

	private int id;

	private int currentSubdivCount;

	private float timeOfLastSubdivision;

	private bool subdivIsOngoing;

	private Ray[] rays;

	private Ray[] spotlights;

	public void Init(Entity parentEntity = null, int id = 0)
	{
		this.parentEntity = parentEntity;
		this.id = id;
		InstantiateLights(20, rayPrefab, ref rays);
		InstantiateLights(8, spotlightPrefab, ref spotlights);
		SetupQuad();
	}

	private void Update()
	{
		UpdateQuad();
		if (quad.enabled)
		{
			time += Time.deltaTime * speed;
			UpdateRays(time);
			UpdateSpotlights(time);
			lightsContainer.position = new Vector2(3000f * (float)id, 3000f);
		}
	}

	public void DoSubdiv(int bar, float beat, int count)
	{
		if (scrConductor.instance.isScrubbing)
		{
			return;
		}
		currentSubdivID = BarAndBeatIndex(bar, beat);
		currentSubdivCount = count;
		Ray[] array = spotlights;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].tr.gameObject.SetActive(value: false);
		}
		for (int j = 0; j < 20; j++)
		{
			Ray obj = rays[j];
			obj.alphaTween?.Kill();
			obj.Fade(1f, 0.5f).SetEase(Ease.OutBack);
		}
		subdivIsOngoing = true;
		if (count > 1 && currentSubdivCount <= 8)
		{
			for (int k = 0; k < count; k++)
			{
				Transform tr = spotlights[k].tr;
				tr.gameObject.SetActive(value: true);
				tr.DOKill();
				tr.localScale = Vector3.zero;
				tr.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
			}
		}
	}

	public void AdvanceSubdiv(int bar, float beat, int index)
	{
		if (!scrConductor.instance.isScrubbing && BarAndBeatIndex(bar, beat) == currentSubdivID)
		{
			AdvanceSubdiv(index);
		}
	}

	private void AdvanceSubdiv(int index)
	{
		float num = 20f / (float)currentSubdivCount;
		float num2 = num * (float)index;
		float num3 = Mathf.Max(num2 + num);
		for (float num4 = num2; num4 < num3 && num4 < 20f; num4++)
		{
			int num5 = Mathf.FloorToInt(num4);
			Ray obj = rays[num5];
			obj.alphaTween?.Kill();
			obj.Fade(0f, 0.2f).SetEase(Ease.Linear);
		}
		if (currentSubdivCount <= 8)
		{
			Transform tr = spotlights[index].tr;
			tr.DOKill();
			tr.DOScale(new Vector3(0f, 1f, 1f), 0.2f).SetEase(Ease.OutSine);
			if (index == currentSubdivCount - 1)
			{
				timeOfLastSubdivision = Time.timeSinceLevelLoad;
				subdivIsOngoing = false;
			}
		}
	}

	public void Kill()
	{
		for (int i = 0; i < currentSubdivCount; i++)
		{
			AdvanceSubdiv(i);
		}
	}

	private void InstantiateLights(int count, GameObject prefab, ref Ray[] rayArr)
	{
		prefab.SetActive(value: false);
		rayArr = new Ray[count];
		for (int i = 0; i < count; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(prefab, lightsContainer);
			MeshRenderer componentInChildren = gameObject.GetComponentInChildren<MeshRenderer>();
			rayArr[i] = new Ray
			{
				tr = gameObject.transform,
				renderer = componentInChildren,
				mat = componentInChildren.material
			};
		}
	}

	private void UpdateRays(float time)
	{
		System.Random random = new System.Random(id);
		for (int i = 0; i < 20; i++)
		{
			Ray ray = rays[i];
			Transform tr = ray.tr;
			Material mat = ray.mat;
			float num = NextFloat(random, 0f, 1f);
			float num2 = 0.75f;
			float num3 = (time * num2 + num) / 1f;
			float num4 = num3 % 1f;
			System.Random random2 = new System.Random((Mathf.FloorToInt(num3) << 16) + i);
			float num5 = NextFloat(random2, 0f, 1f);
			Color color = rayGradient.Evaluate(num5 % 1f);
			mat.color = RXColorExtensions.WithAlpha(alpha: alphaCurve.Evaluate(num4) * raysAlpha * color.a, color: color);
			float num6 = NextFloat(random2, 0f, 360f);
			float num7 = num4 / 360f + num6;
			tr.eulerAngles = Vector3.forward * num7;
			float num8 = NextFloat(random2, 0.75f, 1.1f);
			float y = sizeCurve.Evaluate(num4) * num8 * ray.alpha;
			float num9 = NextFloat(random2, 0.75f, 1f);
			float x = widthCurve.Evaluate(num4) * num9;
			tr.localScale = new Vector3(x, y, 1f);
			tr.gameObject.SetActive(value: true);
		}
	}

	private void UpdateSpotlights(float time)
	{
		if (currentSubdivCount <= 8)
		{
			System.Random random = new System.Random(Mathf.FloorToInt(currentSubdivID + id * 1000));
			float num = NextFloat(random, 60f, 140f);
			int num2 = ((random.Next() % 2 != 0) ? 1 : (-1));
			float num3 = num * (float)num2 * time;
			Color color = spotlightColors[(int)Mathf.Repeat(currentSubdivCount - 1, spotlightColors.Length)];
			float num4 = 360f / (float)currentSubdivCount;
			for (int i = 0; i < currentSubdivCount; i++)
			{
				Ray obj = spotlights[i];
				obj.tr.eulerAngles = Vector3.forward * (num4 * (float)i + num3);
				obj.mat.color = color;
			}
		}
	}

	private void SetupQuad()
	{
		Vector2Int vector2Int = Vector2Int.RoundToInt(quad.transform.localScale);
		RenderTexture renderTexture = new RenderTexture(vector2Int.x, vector2Int.y, 16, RenderTextureFormat.ARGB32)
		{
			antiAliasing = 1,
			filterMode = FilterMode.Point,
			name = cam.gameObject.name + "RT"
		};
		cam.targetTexture = renderTexture;
		cam.orthographicSize = (float)vector2Int.y / 2f;
		quad.material = UnityEngine.Object.Instantiate(quad.material);
		quad.material.mainTexture = renderTexture;
	}

	private void UpdateQuad()
	{
		float timeSinceLevelLoad = Time.timeSinceLevelLoad;
		bool flag = subdivIsOngoing || timeSinceLevelLoad - timeOfLastSubdivision <= 1f;
		cam.enabled = flag;
		quad.enabled = flag;
		if (flag)
		{
			quad.material.renderQueue = 2000;
			if (parentEntity != null)
			{
				quad.sortingLayerName = parentEntity.GetComponent<MeshRenderer>().sortingLayerName;
				quad.sortingOrder = parentEntity.GetComponent<MeshRenderer>().sortingLayerID;
			}
			cam.orthographicSize = quad.transform.lossyScale.y / 2f;
		}
	}

	private void OnDestroy()
	{
		Texture texture = quad?.material?.mainTexture;
		if (texture != null)
		{
			UnityEngine.Object.Destroy(texture);
		}
	}

	private uint BarAndBeatIndex(int bar, float beat)
	{
		return (uint)(((float)bar * 1000f + beat) * 100f);
	}

	private static float NextFloat(System.Random random, float min, float max)
	{
		return (float)(random.NextDouble() * (double)(max - min) + (double)min);
	}
}
