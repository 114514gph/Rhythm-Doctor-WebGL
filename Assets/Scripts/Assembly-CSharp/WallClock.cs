using System;
using UnityEngine;

public class WallClock : MonoBehaviour
{
	public SpriteRenderer spriteRenderer;

	public SpriteRenderer minuteRenderer;

	public SpriteRenderer hourRenderer;

	public SpriteRenderer frameRenderer;

	private Color thisColor;

	private Color minuteColor;

	private Color hourColor;

	public Sprite[] minuteSprites;

	public Sprite[] hourSprites;

	public bool settingManually;

	private void Awake()
	{
		thisColor = spriteRenderer.color;
		minuteColor = minuteRenderer.color;
		hourColor = hourRenderer.color;
	}

	private void Update()
	{
		if (!settingManually)
		{
			SetTime(DateTime.Now.TimeOfDay.TotalSeconds);
		}
	}

	public void SetTime(double seconds)
	{
		double num = seconds / 60.0 % 60.0;
		double num2 = seconds / 3600.0 % 12.0;
		int num3 = (int)(num * (double)minuteSprites.Length / 60.0);
		int num4 = (int)(num2 * (double)hourSprites.Length / 12.0);
		minuteRenderer.sprite = minuteSprites[num3];
		hourRenderer.sprite = hourSprites[num4];
	}

	public void Tint(Color color)
	{
		spriteRenderer.color = thisColor * color;
		minuteRenderer.color = minuteColor * color;
		hourRenderer.color = hourColor * color;
		if (frameRenderer != null)
		{
			frameRenderer.color = spriteRenderer.color;
		}
	}
}
