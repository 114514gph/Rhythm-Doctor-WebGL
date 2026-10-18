using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarSlider : MonoBehaviour
{
	public List<Image> stars;

	public Sprite emptyStar;

	public Sprite halfStar;

	public Sprite fullStar;

	public Slider slider;

	private void Update()
	{
		float num = slider.value * (float)stars.Count;
		for (int i = 0; i < stars.Count; i++)
		{
			if (num > (float)i)
			{
				stars[i].sprite = fullStar;
				if (Mathf.FloorToInt(num) == i && (double)(num % 1f) <= 0.5)
				{
					stars[i].sprite = halfStar;
				}
			}
			else
			{
				stars[i].sprite = emptyStar;
			}
		}
	}

	public string NumberOfStars()
	{
		float num = slider.value * (float)stars.Count;
		float num2 = 0f;
		for (int i = 0; i < stars.Count; i++)
		{
			if (num > (float)i)
			{
				num2 = ((Mathf.FloorToInt(num) != i || !((double)(num % 1f) <= 0.5)) ? (num2 + 1f) : (num2 + 0.5f));
			}
		}
		if (slider.value == 0.45f)
		{
			return "didn't vote";
		}
		return num2.ToString() ?? "";
	}
}
