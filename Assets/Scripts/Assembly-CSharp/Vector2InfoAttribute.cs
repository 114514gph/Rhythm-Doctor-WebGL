using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Property)]
public class Vector2InfoAttribute : Attribute
{
	public readonly Vector2 min;

	public readonly Vector2 max;

	public Vector2InfoAttribute(float minX = float.NegativeInfinity, float minY = float.NegativeInfinity, float maxX = float.PositiveInfinity, float maxY = float.PositiveInfinity)
	{
		min = new Vector2(minX, minY);
		max = new Vector2(maxX, maxY);
	}
}
