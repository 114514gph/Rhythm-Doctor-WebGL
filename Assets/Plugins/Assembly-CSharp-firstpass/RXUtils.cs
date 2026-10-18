using System;
using System.Globalization;
using UnityEngine;

public static class RXUtils
{
	public static Color HexToColor(this string hex)
	{
		if (string.IsNullOrEmpty(hex) || hex.Length < 6)
		{
			return Color.black;
		}
		byte r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
		byte g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
		byte b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
		byte a = ((hex.Length >= 8) ? byte.Parse(hex.Substring(6, 2), NumberStyles.HexNumber) : byte.MaxValue);
		return new Color32(r, g, b, a);
	}

	public static string ToHex(this Color c)
	{
		return $"#{ToByte(c.r):X2}{ToByte(c.g):X2}{ToByte(c.b):X2}";
	}

	public static string OpacityToHex(int opacity)
	{
		int value = Mathf.RoundToInt((float)opacity / 100f * 255f);
		return Mathf.Clamp(value, 0, 255).ToString("X2");
	}

	private static byte ToByte(float f)
	{
		f = Mathf.Clamp01(f);
		return (byte)(f * 255f);
	}

	public static float GetAngle(this Vector2 vector)
	{
		return Mathf.Atan2(0f - vector.y, vector.x) * (180f / MathF.PI);
	}

	public static float GetRadians(this Vector2 vector)
	{
		return Mathf.Atan2(0f - vector.y, vector.x);
	}

	public static Rect ExpandRect(Rect rect, float paddingX, float paddingY)
	{
		return new Rect(rect.x - paddingX, rect.y - paddingY, rect.width + paddingX * 2f, rect.height + paddingY * 2f);
	}

	public static void LogRect(string name, Rect rect)
	{
		Debug.Log(name + ": (" + rect.x + "," + rect.y + "," + rect.width + "," + rect.height + ")");
	}

	public static void LogVectors(string name, params Vector2[] args)
	{
		string text = name + ": " + args.Length + " Vector2 " + args[0];
		for (int i = 1; i < args.Length; i++)
		{
			Vector2 vector = args[i];
			text = text + ", " + vector;
		}
		Debug.Log(text);
	}

	public static void LogVectors(string name, params Vector3[] args)
	{
		string text = name + ": " + args.Length + " Vector3 " + args[0];
		for (int i = 1; i < args.Length; i++)
		{
			Vector3 vector = args[i];
			text = text + ", " + vector;
		}
		Debug.Log(text);
	}

	public static void LogVectorsDetailed(string name, params Vector2[] args)
	{
		string text = name + ": " + args.Length + " Vector2 " + VectorDetailedToString(args[0]);
		for (int i = 1; i < args.Length; i++)
		{
			Vector2 vector = args[i];
			text = text + ", " + VectorDetailedToString(vector);
		}
		Debug.Log(text);
	}

	public static string VectorDetailedToString(Vector2 vector)
	{
		return "(" + vector.x + "," + vector.y + ")";
	}

	public static Color GetColorFromHex(uint hex)
	{
		uint num = hex >> 16;
		uint num2 = hex - (num << 16);
		uint num3 = num2 >> 8;
		uint num4 = num2 - (num3 << 8);
		return new Color((float)num / 255f, (float)num3 / 255f, (float)num4 / 255f);
	}

	public static Color GetColorFromHex(string hexString)
	{
		return GetColorFromHex(Convert.ToUInt32(hexString, 16));
	}

	public static Vector2 GetVector2FromString(string input)
	{
		string[] array = input.Split(new char[1] { ',' });
		return new Vector2(float.Parse(array[0]), float.Parse(array[1]));
	}
}
