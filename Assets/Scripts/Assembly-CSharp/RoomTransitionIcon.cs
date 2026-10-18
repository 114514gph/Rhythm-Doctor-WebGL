using UnityEngine;

public class RoomTransitionIcon : MonoBehaviour
{
	private float[] sizes;

	public RectTransform[] bars;

	private bool setup;

	private float minSize => sizes[0];

	private float maxSize => sizes[^1];

	private void Setup()
	{
		sizes = new float[bars.Length];
		int num = 0;
		RectTransform[] array = bars;
		foreach (RectTransform rectTransform in array)
		{
			sizes[num] = rectTransform.sizeDelta.y;
			num++;
		}
		setup = true;
	}

	public void Show(RoomTransitionType transitionType, float scale = 1f)
	{
		if (!setup)
		{
			Setup();
		}
		base.gameObject.SetActive(value: true);
		float[] array = new float[sizes.Length];
		for (int i = 0; i < array.Length; i++)
		{
			float num = 0f;
			switch (transitionType)
			{
			case RoomTransitionType.Show:
				num = sizes[i];
				break;
			case RoomTransitionType.Hide:
				num = sizes[array.Length - i - 1];
				break;
			case RoomTransitionType.KeepVisible:
				num = maxSize;
				break;
			case RoomTransitionType.KeepHidden:
				num = minSize;
				break;
			}
			_ = minSize;
			num = minSize + num * scale;
			array[i] = num;
		}
		int num2 = 0;
		RectTransform[] array2 = bars;
		foreach (RectTransform obj in array2)
		{
			obj.sizeDelta = obj.sizeDelta.WithY(array[num2]);
			num2++;
		}
	}
}
