using UnityEngine;

public static class VectorExtensions
{
	public static Vector2 xy(this Vector3 v)
	{
		return new Vector2(v.x, v.y);
	}

	public static Vector3 AddXY(this Vector3 v, float x, float y)
	{
		return new Vector3(v.x + x, v.y + y, v.z);
	}

	public static Vector3 AddX(this Vector3 v, float x)
	{
		return new Vector3(v.x + x, v.y, v.z);
	}

	public static Vector3 AddY(this Vector3 v, float y)
	{
		return new Vector3(v.x, v.y + y, v.z);
	}

	public static Vector3 AddZ(this Vector3 v, float z)
	{
		return new Vector3(v.x, v.y, v.z + z);
	}

	public static Vector3 WithX(this Vector3 v, float x)
	{
		return new Vector3(x, v.y, v.z);
	}

	public static Vector3 WithY(this Vector3 v, float y)
	{
		return new Vector3(v.x, y, v.z);
	}

	public static Vector3 WithZ(this Vector3 v, float z)
	{
		return new Vector3(v.x, v.y, z);
	}

	public static Vector3 WithXY(this Vector3 v, float x, float y)
	{
		return new Vector3(x, y, v.z);
	}

	public static Vector2 WithX(this Vector2 v, float x)
	{
		return new Vector2(x, v.y);
	}

	public static Vector2 WithY(this Vector2 v, float y)
	{
		return new Vector2(v.x, y);
	}

	public static Vector3 WithZ(this Vector2 v, float z)
	{
		return new Vector3(v.x, v.y, z);
	}
}
