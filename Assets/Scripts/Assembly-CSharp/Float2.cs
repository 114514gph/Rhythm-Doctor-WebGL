using UnityEngine;

public struct Float2
{
	public float x;

	public float y;

	public bool xUsed;

	public bool yUsed;

	public float[] asArray
	{
		get
		{
			return new float[2] { x, y };
		}
		set
		{
			x = value[0];
			y = value[1];
		}
	}

	public static Float2 one => new Float2(1f, 1f);

	public static Float2 zero => new Float2(0f, 0f);

	public static Float2 empty => new Float2
	{
		xUsed = false,
		yUsed = false
	};

	public Float2(float x, float y)
	{
		this.x = x;
		this.y = y;
		xUsed = true;
		yUsed = true;
	}

	public Float2(int[] a)
	{
		x = a[0];
		y = a[1];
		xUsed = true;
		yUsed = true;
	}

	public Float2(Vector2Int a)
	{
		x = a.x;
		y = a.y;
		xUsed = true;
		yUsed = true;
	}

	public Float2(Vector2 a)
	{
		x = a.x;
		y = a.y;
		xUsed = true;
		yUsed = true;
	}

	public static implicit operator Vector2(Float2 d)
	{
		return new Vector2(d.x, d.y);
	}

	public static explicit operator Float2(Vector2 d)
	{
		return new Float2(d.x, d.y);
	}

	public static implicit operator bool(Float2 f)
	{
		if (!f.xUsed)
		{
			return f.yUsed;
		}
		return true;
	}

	public Vector2 ToVector2()
	{
		return new Vector2(x, y);
	}

	public Vector2Int ToVector2Int()
	{
		return Vector2Int.RoundToInt(ToVector2());
	}

	public override string ToString()
	{
		if (xUsed && yUsed)
		{
			return $"X: {x}, Y: {y}";
		}
		if (xUsed)
		{
			return $"X: {x}";
		}
		if (yUsed)
		{
			return $"Y: {y}";
		}
		return string.Empty;
	}

	public static Float2 operator +(Float2 a, Float2 d)
	{
		return new Float2(a.x + d.x, a.y + d.y);
	}

	public static Float2 operator +(Float2 a, Vector2Int d)
	{
		return new Float2(a.x + (float)d.x, a.y + (float)d.y);
	}

	public static Float2 operator +(Vector2Int d, Float2 a)
	{
		return new Float2((float)d.x + a.x, (float)d.y + a.y);
	}

	public static Float2 operator -(Float2 a, Float2 d)
	{
		return new Float2(a.x - d.x, a.y - d.y);
	}

	public static Float2 operator -(Float2 a, Vector2Int d)
	{
		return new Float2(a.x - (float)d.x, a.y - (float)d.y);
	}

	public static Float2 operator -(Vector2Int d, Float2 a)
	{
		return new Float2((float)d.x - a.x, (float)d.y - a.y);
	}

	public static Float2 operator *(Float2 a, float d)
	{
		return new Float2(a.x * d, a.y * d);
	}

	public static Float2 operator *(float d, Float2 a)
	{
		return new Float2(d * a.x, d * a.y);
	}

	public static Float2 operator *(Float2 a, Float2 d)
	{
		return new Float2(a.x * d.x, a.y * d.y);
	}

	public static Float2 operator /(Float2 a, float d)
	{
		return new Float2(a.x / d, a.y / d);
	}

	public static Float2 operator /(float d, Float2 a)
	{
		return new Float2(d / a.x, d / a.y);
	}

	public static Float2 operator /(Float2 a, Float2 d)
	{
		return new Float2(a.x / d.x, a.y / d.y);
	}

	public static Float2 operator /(Vector2Int a, Float2 d)
	{
		return new Float2((float)a.x / d.x, (float)a.y / d.y);
	}
}
