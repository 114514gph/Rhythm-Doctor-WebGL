using System;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class SpotlightMesh : MonoBehaviour
{
	public float radio = 10f;

	public float angle = 20f;

	private void Start()
	{
		Vector2 zero = Vector2.zero;
		float f = angle * (MathF.PI / 180f);
		Vector2 vector = new Vector2(radio * Mathf.Cos(f), radio * Mathf.Sin(f));
		Vector2 vector2 = new Vector2(vector.x, 0f - vector.y);
		Mesh mesh = new Mesh();
		mesh.Clear();
		mesh.vertices = new Vector3[3] { zero, vector, vector2 };
		mesh.triangles = new int[3] { 0, 1, 2 };
		GetComponent<MeshFilter>().mesh = mesh;
	}
}
