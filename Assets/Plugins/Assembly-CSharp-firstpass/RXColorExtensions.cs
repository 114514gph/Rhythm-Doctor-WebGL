using UnityEngine;

public static class RXColorExtensions
{
	public static Color WithAlpha(this Color color, float alpha)
	{
		return new Color(color.r, color.g, color.b, alpha);
	}

	public static Color WithMultipliedAlpha(this Color color, float alpha)
	{
		return new Color(color.r, color.g, color.b, color.a * alpha);
	}

	public static void ApplyMultipliedAlpha(this Color color, ref Color targetColor, float alpha)
	{
		targetColor.r = color.r;
		targetColor.g = color.g;
		targetColor.b = color.b;
		targetColor.a = color.a * alpha;
	}
}
