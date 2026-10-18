using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class RDManyEyes : RDBase
{
	public GameObject eyePrefab;

	public GameObject eyeSmallPrefab;

	public Transform eyesContainer;

	[NonSerialized]
	public Transform eyesTarget;

	[NonSerialized]
	public Vector2 offset;

	public float scrollSpeed;

	public float scrollAngle;

	private const int maxEyes = 400;

	private bool isSmall;

	private float spacingX;

	private float spacingY;

	private float wrapX;

	private float wrapY;

	private float verticalShiftSpeed;

	private Vector3 verticalShiftDirection = new Vector3(30f, 36f);

	private Tween eyesTargetTween;

	private Tween stareTween;

	private float stareLerp;

	private Vector2 stareFrom;

	private Vector3 stareOffset;

	private Transform stareTarget;

	private Row stareRow;

	private Tween angleTween;

	private Tween scrollTween;

	[NonSerialized]
	public EdegaEye[] eyes;

	[NonSerialized]
	private List<EdegaEye> eyesClosed = new List<EdegaEye>();

	[NonSerialized]
	private List<EdegaEye> eyesOpen = new List<EdegaEye>();

	private System.Random RNG = new System.Random(0);

	public void Setup(bool smallEyes)
	{
		isSmall = smallEyes;
		eyesTarget = eyePrefab.GetComponent<EdegaEye>().targetTransform;
		eyes = new EdegaEye[400];
		if (!smallEyes)
		{
			float num = 60f;
			float num2 = 36f;
			spacingX = num;
			spacingY = num2;
		}
		else
		{
			float num2 = 30f;
			float num = 18f;
			spacingX = num2;
			spacingY = num;
		}
		int num3 = 20;
		wrapX = spacingX * (float)num3;
		wrapY = spacingY * (float)num3;
		for (int i = 0; i < 400; i++)
		{
			GameObject obj = UnityEngine.Object.Instantiate(smallEyes ? eyeSmallPrefab : eyePrefab, eyesContainer, worldPositionStays: false);
			obj.SetActive(value: true);
			EdegaEye component = obj.GetComponent<EdegaEye>();
			eyes[i] = component;
			int num4 = i % num3;
			int num5 = i / num3;
			float num6 = (float)num4 * spacingX;
			float num7 = (float)num5 * spacingY;
			if (num5 % 2 == 0)
			{
				num6 += spacingX / 2f;
			}
			component.transform.localPosition = (component.position = new Vector2(num6, num7));
			if (component.closed)
			{
				eyesClosed.Add(component);
			}
			else
			{
				eyesOpen.Add(component);
			}
		}
	}

	private void Start()
	{
		if (eyes == null)
		{
			Setup(smallEyes: false);
		}
	}

	private void Update()
	{
		float f = MathF.PI / 180f * scrollAngle;
		Vector2 vector = new Vector2(Mathf.Cos(f), Mathf.Sin(f)) * (scrollSpeed * Time.deltaTime);
		offset += vector;
		if (eyesTarget != null && stareTarget != null)
		{
			Vector3 b = base.game.rooms[stareRow?.room ?? 0].transform.InverseTransformPoint(stareTarget.position) - new Vector3(RDBase.Vfx.RDWidth / 2f, RDBase.Vfx.RDHeight / 2f) + stareOffset;
			Vector3 v = Vector3.Lerp(stareFrom, b, stareLerp);
			eyesTarget.localPosition = v.WithZ(eyesTarget.localPosition.z);
		}
		if (verticalShiftSpeed != 0f)
		{
			float num = (isSmall ? 0.5f : 1f);
			Vector3 vector2 = verticalShiftDirection * Time.deltaTime * verticalShiftSpeed * num;
			Vector3 vector3 = -verticalShiftDirection * Time.deltaTime * verticalShiftSpeed * num;
			for (int i = 0; i < eyes.Length; i++)
			{
				int num2 = i % 20;
				int num3 = i / 20;
				Vector3 vector4 = (((num2 * 2 + num3 % 2 + num3) % 4 == 0) ? vector2 : vector3);
				eyes[i].position += vector4;
			}
		}
	}

	private void LateUpdate()
	{
		EdegaEye[] array = eyes;
		foreach (EdegaEye edegaEye in array)
		{
			edegaEye.transform.localPosition = new Vector2(Mathf.Repeat(edegaEye.position.x + offset.x, wrapX) - wrapX / 2f, Mathf.Repeat(edegaEye.position.y + offset.y, wrapY) - wrapY / 2f);
		}
	}

	public void SetScrollAngle(float angle, float duration, Ease ease)
	{
		angleTween?.Kill();
		angleTween = DOTween.To(() => scrollAngle, (float x) =>
		{
			scrollAngle = x;
		}, angle, duration).SetEase(ease).Done();
	}

	public void SetScrollSpeed(float amount, float duration, Ease ease)
	{
		scrollTween?.Kill();
		scrollTween = DOTween.To(() => scrollSpeed, (float x) =>
		{
			scrollSpeed = x;
		}, amount, duration).SetEase(ease).Done();
	}

	public void SetEyeDirection(Vector2 dir, float duration, Ease ease)
	{
		SetScrollSpeed(dir.magnitude, duration, ease);
		SetScrollAngle(Mathf.Atan2(dir.y, dir.x) * 57.29578f, duration, ease);
	}

	public void SetEyeColor(Color color, float duration, Ease ease)
	{
		SetEyePartColor(isPupil: true, color, duration, ease);
		SetEyePartColor(isPupil: false, color, duration, ease);
	}

	public void SetEyePartColor(bool isPupil, Color color, float duration, Ease ease)
	{
		EdegaEye[] array = eyes;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].TweenColor(isPupil, color, duration, ease);
		}
	}

	public void SetEyeBackgroundColor(Color color, float duration, Ease ease)
	{
		EdegaEye[] array = eyes;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].TweenBackgroundColor(color, duration, ease);
		}
	}

	public void EyesClose(float percent, bool instant = false)
	{
		float num = percent * (float)eyes.Length;
		float num2 = (float)eyes.Length - num;
		while ((float)eyesClosed.Count < num)
		{
			int index = RNG.Next(eyesOpen.Count);
			EdegaEye edegaEye = eyesOpen[index];
			eyesOpen.RemoveAt(index);
			eyesClosed.Add(edegaEye);
			edegaEye.DoBlink(closed: true, instant);
		}
		while ((float)eyesOpen.Count < num2)
		{
			int index = RNG.Next(eyesClosed.Count);
			EdegaEye edegaEye = eyesClosed[index];
			eyesClosed.RemoveAt(index);
			eyesOpen.Add(edegaEye);
			edegaEye.DoBlink(closed: false, instant);
		}
	}

	public void EyesToggle(float percent)
	{
		float num = percent * (float)eyes.Length;
		List<EdegaEye> list = new List<EdegaEye>(eyesOpen);
		list.AddRange(eyesClosed);
		List<EdegaEye> list2 = new List<EdegaEye>();
		List<EdegaEye> list3 = new List<EdegaEye>();
		for (int i = 0; (float)i < num; i++)
		{
			int index = RNG.Next(list.Count);
			EdegaEye edegaEye = list[index];
			list.RemoveAt(index);
			edegaEye.ToggleBlink();
			if (edegaEye.closed)
			{
				eyesOpen.Remove(edegaEye);
				list3.Add(edegaEye);
			}
			else
			{
				eyesClosed.Remove(edegaEye);
				list2.Add(edegaEye);
			}
		}
		eyesOpen.AddRange(list2);
		eyesClosed.AddRange(list3);
	}

	public void EyesToggleEqually(float percent)
	{
		float num = percent * (float)eyes.Length / 2f;
		num = Mathf.Min(eyesOpen.Count, Mathf.Min(eyesClosed.Count), Mathf.Min(num));
		List<EdegaEye> list = new List<EdegaEye>();
		List<EdegaEye> list2 = new List<EdegaEye>();
		for (int i = 0; (float)i < num; i++)
		{
			int index = RNG.Next(eyesOpen.Count);
			EdegaEye edegaEye = eyesOpen[index];
			eyesOpen.RemoveAt(index);
			list2.Add(edegaEye);
			edegaEye.DoBlink(closed: true);
			index = RNG.Next(eyesClosed.Count);
			edegaEye = eyesClosed[index];
			eyesClosed.RemoveAt(index);
			list.Add(edegaEye);
			edegaEye.DoBlink(closed: false);
		}
		eyesOpen.AddRange(list);
		eyesClosed.AddRange(list2);
	}

	public void EyesBlink(float percent)
	{
		float num = percent * (float)eyesOpen.Count;
		List<EdegaEye> list = new List<EdegaEye>();
		for (int i = 0; (float)i < num; i++)
		{
			int index = RNG.Next(eyesOpen.Count);
			EdegaEye edegaEye = eyesOpen[index];
			edegaEye.DoBlink(closed: true);
			eyesOpen.RemoveAt(index);
			list.Add(edegaEye);
			EdegaEye eyeRef = edegaEye;
			DOVirtual.DelayedCall(0.25f, () =>
			{
				eyeRef.DoBlink(closed: false);
			});
		}
		eyesOpen.AddRange(list);
	}

	public void StareAtTransform(Transform target, Vector3 offset = default(Vector3))
	{
		stareRow = null;
		stareFrom = eyesTarget.localPosition;
		stareTarget = target;
		stareOffset = offset;
		stareTween?.Kill();
		stareTween = DOTween.To(() => stareLerp, (float x) =>
		{
			stareLerp = x;
		}, 1f, 0.25f).From(0f).SetEase(Ease.InOutSine)
			.Done();
	}

	public void SetEyesTarget(float x, float y, float duration, Ease ease)
	{
		stareTarget = null;
		eyesTargetTween?.Kill();
		eyesTargetTween = eyesTarget.DOLocalMove(new Vector3(x, y, eyesTarget.localPosition.z), duration).SetEase(ease).Done();
	}

	public void StareAtRow(int rowID)
	{
		if (base.game.rows[rowID].ent != null)
		{
			StareAtTransform(base.game.rows[rowID].ent.transform);
			stareRow = base.game.rows[rowID];
		}
	}

	public void ResetStare(bool resetPosition = true)
	{
		StareAtTransform(eyesTarget);
		stareTarget = null;
		if (resetPosition)
		{
			eyesTarget.localPosition = new Vector3(0f, 0f, eyesTarget.localPosition.z);
		}
	}

	public void SetVerticalShiftSpeed(float speed)
	{
		verticalShiftSpeed = speed;
	}
}
