using System;
using RDLevelEditor;
using UnityEngine;

public readonly struct ColorOrPalette
{
	private readonly int? paletteIndex;

	private readonly Color? color;

	public float? alpha => color?.a;

	public bool isPalette => paletteIndex.HasValue;

	private ColorOrPalette(int paletteIndex)
	{
		this.paletteIndex = paletteIndex;
		color = null;
	}

	private ColorOrPalette(Color color)
	{
		paletteIndex = null;
		this.color = color;
	}

	public static implicit operator ColorOrPalette(Color color)
	{
		return new ColorOrPalette(color);
	}

	public static ColorOrPalette FromString(string str)
	{
		if (!str.StartsWith("pal"))
		{
			return new ColorOrPalette(str.HexToColor());
		}
		if (!int.TryParse(str.Replace("pal", ""), out var result))
		{
			throw new Exception("Failed to parse Color in '" + str + "'.");
		}
		if (result < 0 || result > 21)
		{
			throw new Exception($"Palette index {result} should be between 0 and {21}.");
		}
		return new ColorOrPalette(result);
	}

	public static ColorOrPalette FromColor(Color c)
	{
		return new ColorOrPalette(c);
	}

	public Color ToColor(bool useEditorPalette = false)
	{
		if (color.HasValue)
		{
			return color.Value;
		}
		int value = paletteIndex.Value;
		if (useEditorPalette && scnEditor.instance == null)
		{
			throw new Exception("ColorOrPalette.ToColor: Using editor palette but editor == null.");
		}
		if (!useEditorPalette && RDLevelData.current == null)
		{
			throw new Exception("ColorOrPalette.ToColor: Using level palette but RDLevelData.current == null.");
		}
		return (useEditorPalette ? scnEditor.instance.colorPalette : RDLevelData.current.colorPalette)[value].HexToColor();
	}

	public int GetIndex()
	{
		if (color.HasValue)
		{
			return -1;
		}
		return paletteIndex.Value;
	}

	public string Encode(bool hasAlpha)
	{
		if (color.HasValue)
		{
			if (!hasAlpha)
			{
				return ColorUtility.ToHtmlStringRGB(color.Value);
			}
			return ColorUtility.ToHtmlStringRGBA(color.Value);
		}
		return $"pal{paletteIndex.Value}";
	}

	public ColorOrPalette WithAlpha(float newAlpha)
	{
		return ((ColorOrPalette?)color?.WithAlpha(newAlpha)) ?? this;
	}
}
