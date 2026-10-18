using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class tk2dSpriteCollectionBuilderUtil
{
	private const int maxGlowColors = 4;

	private static Color[] glowColors = new Color[4];

	private const int ParamNotFound = -99;

	private const int ValueNotFound = -1;

	public static int NiceRescaleK(float scale)
	{
		if (scale > 0.499f && scale < 0.501f)
		{
			return 2;
		}
		if (scale > 0.249f && scale < 0.251f)
		{
			return 4;
		}
		return 0;
	}

	public static Texture2D RescaleTexture(Texture2D texture, float scale)
	{
		int num = NiceRescaleK(scale);
		bool flag = num != 0;
		if (texture != null)
		{
			int num2 = num;
			int width = texture.width;
			int height = texture.height;
			int num3 = (flag ? ((width + num2 - 1) / num2) : ((int)((float)width * scale)));
			int num4 = (flag ? ((height + num2 - 1) / num2) : ((int)((float)height * scale)));
			Texture2D texture2D = new Texture2D(num3, num4);
			for (int i = 0; i < num4; i++)
			{
				for (int j = 0; j < num3; j++)
				{
					if (flag)
					{
						Color color = new Color(0f, 0f, 0f, 0f);
						float num5 = 0f;
						for (int k = 0; k < num2; k++)
						{
							int num6 = i * num2 + k;
							if (num6 >= height)
							{
								continue;
							}
							for (int l = 0; l < num2; l++)
							{
								int num7 = j * num2 + l;
								if (num7 < width)
								{
									num5++;
									Color pixel = texture.GetPixel(num7, num6);
									color += pixel;
								}
							}
						}
						texture2D.SetPixel(j, i, (num5 > 0f) ? (color * (1f / num5)) : Color.black);
					}
					else
					{
						texture2D.SetPixel(j, i, texture.GetPixelBilinear((float)j / (float)num3, (float)i / (float)num4));
					}
				}
			}
			texture2D.Apply();
			return texture2D;
		}
		return null;
	}

	public static Texture2D GaussianBlur(Texture2D source, float radius = -1f)
	{
		if (radius == -1f)
		{
			radius = 2f;
		}
		int width = source.width;
		int height = source.height;
		int num = Mathf.CeilToInt(radius * 2.57f);
		Texture2D texture2D = new Texture2D(source.width, source.height, TextureFormat.RGBA32, mipChain: false);
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				float num2 = 0f;
				float num3 = 0f;
				for (int k = i - num; k <= i + num; k++)
				{
					for (int l = j - num; l <= j + num; l++)
					{
						int num4 = Mathf.Clamp(k, 0, width - 1);
						int num5 = Mathf.Clamp(l, 0, height - 1);
						float num6 = Mathf.Exp((float)(-((k - num4) * (k - num4) + (l - num5) * (l - num5))) / (2f * radius * radius)) / (MathF.PI * 2f * radius * radius);
						num2 += source.GetPixel(k, l).a * num6;
						num3 += num6;
					}
				}
				num2 /= num3;
				Color color = new Color(1f, 1f, 1f, num2 * 2f);
				texture2D.SetPixel(i, j, color);
			}
		}
		return texture2D;
	}

	public static Texture2D RDBlur(Texture2D source, int radius = -1)
	{
		if (radius == -1)
		{
			radius = 5;
		}
		int width = source.width;
		int height = source.height;
		Texture2D texture2D = new Texture2D(source.width, source.height, TextureFormat.RGBA32, mipChain: false);
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				Color pixel = source.GetPixel(i, j);
				Color color = new Color(1f, 1f, 1f, 1f);
				if (pixel.a < 0.01f)
				{
					float num = (float)radius + 0.001f;
					for (int k = -radius; k <= radius; k++)
					{
						for (int l = -radius; l <= radius; l++)
						{
							int num2 = i + l;
							int num3 = j + k;
							float num4 = Mathf.Sqrt(l * l + k * k);
							if ((l != 0 || k != 0) && num2 >= 0 && num2 < source.width && num3 >= 0 && num3 < source.height && num4 < num && source.GetPixel(num2, num3).a > 0.5f)
							{
								num = num4;
							}
						}
					}
					float num5 = Mathf.InverseLerp(radius, 0f, num);
					num5 = Mathf.Pow(Mathf.Cos(MathF.PI * (num5 - 1f) * 0.5f), 3.5f);
					color = new Color(1f, 1f, 1f, num5);
				}
				texture2D.SetPixel(i, j, color);
			}
		}
		return texture2D;
	}

	public static Texture2D OutlineTexture(Texture2D source, int strokeWidth = -1)
	{
		if (strokeWidth == -1)
		{
			strokeWidth = 1;
		}
		int width = source.width;
		int height = source.height;
		Texture2D texture2D = new Texture2D(source.width, source.height, TextureFormat.RGBA32, mipChain: false);
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				Color pixel = source.GetPixel(i, j);
				Color color = Color.clear;
				if (pixel.a < 0.01f)
				{
					int num = 404;
					for (int k = -strokeWidth; k <= strokeWidth; k++)
					{
						for (int l = -strokeWidth; l <= strokeWidth; l++)
						{
							int num2 = i + l;
							int num3 = j + k;
							int num4 = Mathf.Abs(l) + Mathf.Abs(k);
							if ((l != 0 || k != 0) && num2 >= 0 && num2 < source.width && num3 >= 0 && num3 < source.height)
							{
								Color pixel2 = source.GetPixel(num2, num3);
								if (num4 < num && pixel2.a > 0.5f)
								{
									num = num4;
								}
							}
						}
					}
					if (num != 404)
					{
						color = ((num <= strokeWidth) ? Color.white : Color.clear);
					}
				}
				texture2D.SetPixel(i, j, color);
			}
		}
		return texture2D;
	}

	public static Texture2D FreezeTexture(Texture2D source, Vector2Int spriteSize)
	{
		int x = spriteSize.x;
		int y = spriteSize.y;
		new Vector2Int(Mathf.RoundToInt((float)x / 2f), Mathf.RoundToInt((float)y / 2f));
		int num = x;
		int num2 = 0;
		int num3 = y;
		int num4 = 0;
		int num5 = 0;
		int num6 = 0;
		while (true)
		{
			int num7 = spriteSize.x * num5;
			int num8 = spriteSize.x * (num5 + 1);
			if (num7 >= source.width || num8 > source.width)
			{
				break;
			}
			for (int i = num7; i < num8; i++)
			{
				int num9 = spriteSize.y * num6;
				int num10 = spriteSize.y * (num6 + 1);
				for (int j = num9; j < num10; j++)
				{
					if (source.GetPixel(i, j).a > 0.01f)
					{
						int num11 = i - num7;
						if (num11 < num)
						{
							num = num11;
						}
						if (num11 > num2)
						{
							num2 = num11;
						}
						int num12 = j - num9;
						if (num12 < num3)
						{
							num3 = num12;
						}
						if (num12 > num4)
						{
							num4 = num12;
						}
					}
				}
			}
			num6++;
			if (spriteSize.y * num6 >= source.height)
			{
				num5++;
				num6 = 0;
			}
		}
		num -= 2;
		num3 -= 2;
		num2 += 2;
		num4 += 2;
		int num13 = num2 - num;
		int num14 = num4 - num3;
		if (num13 % 2 != 0)
		{
			num13++;
		}
		if (num14 % 2 != 0)
		{
			num14++;
		}
		Vector2 vector = new Vector2((float)num13 / 2f, (float)num14 / -2f);
		Vector2 vector2 = new Vector2((float)num13 / 2f, (float)num14 / 2f);
		Vector2 vector3 = new Vector2((float)num13 / -2f, (float)num14 / 2f);
		Vector2 vector4 = new Vector2((float)num13 / -2f, (float)num14 / -2f);
		List<Vector2> list = new List<Vector2>();
		int num15 = 0;
		List<Vector2> list2 = GetVertices(vector, vector2).ToList();
		do
		{
			if (list2[num15].y >= (float)num14 / 2f)
			{
				list.Add(list2[num15]);
				list2.RemoveAt(num15);
			}
			else
			{
				num15++;
			}
		}
		while (num15 < list2.Count);
		list.Add(vector2);
		list.AddRange(GetVertices(vector2, vector3));
		list.Add(vector3);
		list.AddRange(GetVertices(vector3, vector4));
		list.Add(vector4);
		list.AddRange(GetVertices(vector4, vector));
		list.Add(vector);
		list.AddRange(list2);
		Vector2[] array = list.ToArray();
		Vector2 zero = Vector2.zero;
		int num16 = x;
		int num17 = y;
		zero.x = (float)num + (float)num13 / 2f;
		zero.y = (float)num3 + (float)num14 / 2f;
		float num18 = x;
		float num19 = 0f;
		float num20 = y;
		float num21 = 0f;
		float num22 = zero.x;
		float num23 = zero.y;
		for (int k = 0; k < array.Length; k++)
		{
			array[k] += zero;
			float x2 = array[k].x;
			float y2 = array[k].y;
			if (x2 < num18)
			{
				num18 = x2;
			}
			if (y2 < num20)
			{
				num20 = y2;
			}
			if (x2 > num19)
			{
				num19 = x2;
			}
			if (y2 > num21)
			{
				num21 = y2;
			}
			if (x2 < num22)
			{
				num22 = x2;
			}
			if (y2 < num23)
			{
				num23 = y2;
			}
		}
		num22 = ((num22 < 0f) ? Mathf.Abs(num22) : 0f);
		num23 = ((num23 < 0f) ? Mathf.Abs(num23) : 0f);
		int num24 = Mathf.CeilToInt(num19 - num18);
		int num25 = Mathf.CeilToInt(num21 - num20);
		if (num24 > num16)
		{
			num16 += num24 - num16;
		}
		if (num25 > num17)
		{
			num17 += num25 - num17;
		}
		if (num16 < num17)
		{
			num22 += (float)((num17 - num16) / 2);
			num16 = num17;
		}
		else if (num17 < num16)
		{
			num23 += (float)((num16 - num17) / 2);
			num17 = num16;
		}
		for (int l = 0; l < array.Length; l++)
		{
			array[l].x += num22;
			array[l].y += num23;
		}
		int num26 = Mathf.Max(spriteSize.x, spriteSize.y);
		bool flag = num26 < 14 || num26 > 128;
		Texture2D texture2D = new Texture2D(num17 * (flag ? 1 : 15), num17, TextureFormat.RGBA32, mipChain: false);
		Vector2[] array2 = null;
		if (!flag)
		{
			array2 = new Vector2[14];
			System.Random random = new System.Random(x * y * (int)DateTime.Now.Ticks);
			for (int m = 0; m < 14; m++)
			{
				Vector2 vector5;
				do
				{
					vector5 = new Vector2(random.Next(num16), random.Next(num17));
				}
				while (!IsPointInPolygon4(array, vector5));
				array2[m] = vector5;
			}
			Vector2 center = Vector2.one * num17 / 2f;
			array2 = array2.OrderBy((Vector2 p) => Mathf.Repeat(0f - Mathf.Atan2(p.y - center.y, p.x - center.x) + MathF.PI / 2f, MathF.PI * 2f)).ToArray();
		}
		Color32[] array3 = new Color32[texture2D.width * texture2D.height];
		for (uint num27 = 0u; num27 < array3.Length; num27++)
		{
			array3[num27] = Color.clear;
		}
		texture2D.SetPixels32(array3);
		for (int num28 = 0; num28 < num16; num28++)
		{
			for (int num29 = 0; num29 < num17; num29++)
			{
				Vector2 vector6 = new Vector2(num28, num29);
				if (!IsPointInPolygon4(array, vector6))
				{
					continue;
				}
				texture2D.SetPixel(num28, num29, Color.white);
				if (flag)
				{
					continue;
				}
				float num30 = float.MaxValue;
				int num31 = 0;
				for (int num32 = 0; num32 < 14; num32++)
				{
					float num33 = Vector2.Distance(array2[num32], vector6);
					if (num33 < num30)
					{
						num30 = num33;
						num31 = num32;
					}
				}
				int num34 = num16 * (num31 + 1);
				texture2D.SetPixel(num28 + num34, num29, Color.white);
			}
		}
		texture2D.filterMode = FilterMode.Point;
		return texture2D;
		static Vector2[] GetVertices(Vector2 from, Vector2 to)
		{
			int num35 = UnityEngine.Random.Range(3, 8);
			Vector2[] array4 = new Vector2[num35];
			float num36 = 0f;
			int num37 = (((float)num35 % 2f != 0f) ? Mathf.CeilToInt((float)num35 / 2f) : (Mathf.RoundToInt((float)num35 / 2f) + 1));
			for (int n = 1; n <= num35; n++)
			{
				float num38 = UnityEngine.Random.Range(-0.05f, 0.05f);
				float t = 1f / (float)(num35 + 1) * (float)n + num38;
				Vector2 vector7 = Vector2.Lerp(from, to, t);
				if (n <= num37)
				{
					num36 = UnityEngine.Random.Range(num36 + 0.06f, num36 + 0.16f);
				}
				else
				{
					float num39 = num36 / (float)(num35 - (n - 1)) * 0.5f;
					float num40 = num39 - num39 * 0.2f;
					float maxInclusive = num40 + num39 / 2f;
					num36 -= UnityEngine.Random.Range(num40, maxInclusive);
				}
				Vector2 vector8 = vector7 * num36;
				vector7 += vector8;
				array4[n - 1] = vector7;
			}
			return array4;
		}
	}

	public static bool IsPointInPolygon4(Vector2[] polygon, Vector2 testPoint)
	{
		bool flag = false;
		int num = polygon.Length - 1;
		for (int i = 0; i < polygon.Length; i++)
		{
			if (((polygon[i].y < testPoint.y && polygon[num].y >= testPoint.y) || (polygon[num].y < testPoint.y && polygon[i].y >= testPoint.y)) && polygon[i].x + (testPoint.y - polygon[i].y) / (polygon[num].y - polygon[i].y) * (polygon[num].x - polygon[i].x) < testPoint.x)
			{
				flag = !flag;
			}
			num = i;
		}
		return flag;
	}

	private static void print(object text)
	{
	}

	public static RDTextureEffects MakeEffectsToTexture(string parameters, bool hasCustomOutline, Texture2D source)
	{
		parameters = parameters.Replace(" ", "");
		string parameter = "gaussianBlur";
		string parameter2 = "rdBlur";
		string parameter3 = "outline";
		int valueFromParameter = GetValueFromParameter(parameters, parameter);
		int valueFromParameter2 = GetValueFromParameter(parameters, parameter2);
		int valueFromParameter3 = GetValueFromParameter(parameters, parameter3);
		Texture2D texture2D = null;
		Texture2D texture2D2 = ((valueFromParameter != -99) ? GaussianBlur(source, valueFromParameter) : ((valueFromParameter2 == -99) ? GaussianBlur(source) : RDBlur(source, valueFromParameter2)));
		if (!hasCustomOutline)
		{
			texture2D = new Texture2D(0, 0);
			texture2D = ((valueFromParameter3 == -99) ? OutlineTexture(source) : OutlineTexture(source, valueFromParameter3));
			texture2D.Apply();
		}
		RDTextureEffects result = default;
		texture2D2.Apply();
		result.blur = texture2D2;
		result.outline = texture2D;
		return result;
	}

	private static int GetValueFromParameter(string parameters, string parameter)
	{
		int num = parameters.IndexOf(parameter);
		parameters += "  ";
		if (num != -1)
		{
			string text = NumberFromString(parameters.Substring(num + parameter.Length, 3));
			if (text.Length == 0)
			{
				return -1;
			}
			return int.Parse(text);
		}
		return -99;
	}

	private static string NumberFromString(string original)
	{
		return new string(original.Where((char c) => char.IsDigit(c)).ToArray());
	}

	public static void RDGetGlowColors(Texture2D[] textures)
	{
		bool[] array = new bool[256];
		bool[] array2 = new bool[256];
		bool[] array3 = new bool[256];
		bool[] array4 = new bool[256];
		foreach (Texture2D texture2D in textures)
		{
			for (int j = 0; j < texture2D.height; j++)
			{
				for (int k = 0; k < texture2D.width; k++)
				{
					Color pixel = texture2D.GetPixel(k, j);
					int num = Mathf.RoundToInt(pixel.r * 255f);
					int num2 = Mathf.RoundToInt(pixel.g * 255f);
					int num3 = Mathf.RoundToInt(pixel.b * 255f);
					array4[num] = true;
					array4[num2] = true;
					array4[num3] = true;
					array[num] = true;
					array2[num] = true;
					array3[num] = true;
				}
			}
		}
		int num4 = 0;
		print("active R");
		int num5 = 0;
		bool[] array5 = array;
		for (int i = 0; i < array5.Length; i++)
		{
			if (!array5[i] && num4 < 4)
			{
				print("not used: " + num5);
				glowColors[num4] = new Color((float)num5 / 255f, 1f, 1f, 1f);
				num4++;
			}
			num5++;
		}
		if (num4 != 4)
		{
			print("we couldn't find enough available red colors :(");
		}
	}

	public static void SaveTextureToPNG(Texture2D texture, string path)
	{
		byte[] array = texture.EncodeToPNG();
		FileStream fileStream = File.Create(path);
		fileStream.Write(array, 0, array.Length);
		fileStream.Close();
		UnityEngine.Object.DestroyImmediate(texture);
	}
}
