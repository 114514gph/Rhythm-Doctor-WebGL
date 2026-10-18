using System;
using System.Collections.Generic;
using UnityEngine;

public class PolygonWave : MonoBehaviour
{
	public enum Part
	{
		Base = 0,
		Outline = 1,
		Glow = 2
	}

	private const float LineWidth = 1f;

	private const float OutlineWidth = 1f;

	private const float GlowWidth = 3f;

	private const int MaxSides = 4;

	public Part part;

	public int sides = 4;

	public float radius1 = 13.43f;

	public MeshRenderer meshRenderer;

	public MeshFilter meshFilter;

	private float currentRadius1;

	private int currentSides;

	private Part currentPart;

	private float radius0;

	private Color _color = Color.white;

	private List<Vector3> vertices = new List<Vector3>(16);

	private List<int> indices = new List<int>(48);

	private List<Color> colors = new List<Color>(16);

	private List<Vector3> polygon = new List<Vector3>(4);

	public Color color
	{
		set
		{
			if (value != _color)
			{
				meshRenderer.material.color = value;
				_color = value;
			}
		}
	}

	private void LateUpdate()
	{
		if (currentSides != sides || currentRadius1 != radius1 || currentPart != part)
		{
			Regen();
		}
	}

	private void Regen()
	{
		if (sides < 3)
		{
			return;
		}
		Mesh mesh = meshFilter.mesh;
		mesh.Clear(keepVertexLayout: false);
		vertices.Clear();
		indices.Clear();
		colors.Clear();
		radius0 = ExtendApothem(radius1, -1f);
		Color color = new Color(1f, 1f, 1f, 0f);
		if (this.part == Part.Base)
		{
			vertices.AddRange(MakeRegularPolygon(sides, radius0));
			vertices.AddRange(MakeRegularPolygon(sides, radius1));
			vertices.AddRange(MakeRegularPolygon(sides, radius0));
			vertices.Add(Vector3.zero);
			int item = vertices.Count - 1;
			int num = sides * 2;
			for (int i = 0; i < vertices.Count; i++)
			{
				colors.Add((i < num) ? Color.white : color);
			}
			for (int j = 0; j < sides; j++)
			{
				indices.Add(item);
				indices.Add(num + (j + 1) % sides);
				indices.Add(num + j % sides);
			}
			for (int k = 0; k < sides; k++)
			{
				indices.Add(k % sides);
				indices.Add((k + 1) % sides);
				indices.Add(sides + k % sides);
				indices.Add((k + 1) % sides);
				indices.Add(sides + (k + 1) % sides);
				indices.Add(sides + k % sides);
			}
		}
		else
		{
			Part part = this.part;
			if (part == Part.Outline || part == Part.Glow)
			{
				float num2 = ((this.part == Part.Outline) ? 1f : 3f);
				float radius = ExtendApothem(radius1, num2);
				float radius2 = ExtendApothem(radius0, 0f - num2);
				vertices.AddRange(MakeRegularPolygon(sides, radius2));
				vertices.AddRange(MakeRegularPolygon(sides, radius0));
				vertices.AddRange(MakeRegularPolygon(sides, radius1));
				vertices.AddRange(MakeRegularPolygon(sides, radius));
				for (int l = 0; l < sides; l++)
				{
					indices.Add(l % sides);
					indices.Add((l + 1) % sides);
					indices.Add(sides + l % sides);
					indices.Add((l + 1) % sides);
					indices.Add(sides + (l + 1) % sides);
					indices.Add(sides + l % sides);
				}
				int num3 = sides * 2;
				for (int m = 0; m < sides; m++)
				{
					indices.Add(num3 + m % sides);
					indices.Add(num3 + (m + 1) % sides);
					indices.Add(num3 + sides + m % sides);
					indices.Add(num3 + (m + 1) % sides);
					indices.Add(num3 + sides + (m + 1) % sides);
					indices.Add(num3 + sides + m % sides);
				}
				if (this.part == Part.Outline)
				{
					for (int n = 0; n < vertices.Count; n++)
					{
						colors.Add(Color.white);
					}
				}
				else
				{
					for (int num4 = 0; num4 < sides; num4++)
					{
						colors.Add(color);
					}
					for (int num5 = 0; num5 < sides; num5++)
					{
						colors.Add(Color.white);
					}
					for (int num6 = 0; num6 < sides; num6++)
					{
						colors.Add(Color.white);
					}
					for (int num7 = 0; num7 < sides; num7++)
					{
						colors.Add(color);
					}
				}
			}
		}
		mesh.SetVertices(vertices);
		mesh.SetTriangles(indices, 0);
		mesh.SetColors(colors);
		currentSides = sides;
		currentRadius1 = radius1;
		currentPart = this.part;
		float ExtendApothem(float num9, float offset)
		{
			float num8 = Mathf.Cos(MathF.PI / (float)sides);
			return (num9 * num8 + offset) / num8;
		}
		List<Vector3> MakeRegularPolygon(int sides, float num9)
		{
			polygon.Clear();
			for (int num8 = 0; num8 < sides; num8++)
			{
				float f = (float)num8 * 1f / (float)sides * (MathF.PI * 2f);
				polygon.Add(new Vector3(Mathf.Cos(f) * num9, Mathf.Sin(f) * num9, 0f));
			}
			return polygon;
		}
	}
}
