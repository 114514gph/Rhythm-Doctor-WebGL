using System;
using UnityEngine;

public class scrVolumeTracker : InvisibleEntity
{
	public delegate void RDSetter<in T>(T pNewValue);

	private const string TrackerTag = "VolumeTracker";

	public int frequency = 20;

	public bool isActive = true;

	private RDSetter<float> setter;

	private float minValue;

	private float maxValue;

	private void Update()
	{
		if (isActive && Time.timeScale != 0f)
		{
			int num = (10 - Persistence.GetGlobalVolume()) * 2;
			float num2 = Mathf.Pow(10f, (float)num / 20f);
			float pNewValue = Mathf.Lerp(minValue, maxValue, base.conductor.mainVolume * base.gc.volTrackerMultiplier * num2);
			setter(pNewValue);
		}
	}

	private scrVolumeTracker Setup(RDSetter<float> setter, float min, float max)
	{
		base.gameObject.tag = "VolumeTracker";
		this.setter = setter;
		minValue = min;
		maxValue = max;
		isActive = true;
		return this;
	}

	public void Delete(float? finalValue = null)
	{
		if (finalValue.HasValue)
		{
			setter(finalValue.Value);
		}
		base.enabled = false;
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public static scrVolumeTracker Add(RDSetter<float> setter, float min, float max)
	{
		return RDUtils.Create<scrVolumeTracker>("VolumeTracker", makeContainer: true).Setup(setter, min, max);
	}

	public static void ClearAll()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("VolumeTracker");
		for (int i = 0; i < array.Length; i++)
		{
			UnityEngine.Object.Destroy(array[i]);
		}
	}

	public static scrVolumeTracker CreateAlphaTracker(Func<Color> get, Action<Color> set, float min, float max, float baseAlpha)
	{
		bool reversed = min > max;
		if (reversed)
		{
			float num = max;
			float num2 = min;
			min = num;
			max = num2;
		}
		return Add((float vol) =>
		{
			Color obj = get();
			float num3 = vol * 10f;
			if (reversed)
			{
				num3 = baseAlpha - num3;
			}
			obj.a = Mathf.Clamp(num3, 0f, baseAlpha);
			set(obj);
		}, min * 0.3f, max * 0.3f);
	}
}
