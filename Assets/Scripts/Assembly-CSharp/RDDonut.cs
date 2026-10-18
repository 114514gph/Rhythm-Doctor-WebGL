using System;
using UnityEngine;

[ExecuteInEditMode]
public class RDDonut : MonoBehaviour
{
	public bool shouldRecreateMesh;

	public SubdivisionRowMode mode;

	public float miniInnerRadius = 10f;

	public float miniOuterRadius = 20f;

	public float normalInnerRadius = 10f;

	public float normalOuterRadius = 20f;

	public int sides = 50;

	private void Start()
	{
		mode = SubdivisionRowMode.Mini;
		shouldRecreateMesh = true;
	}

	private void Update()
	{
		if (shouldRecreateMesh)
		{
			GenerateMesh();
			shouldRecreateMesh = false;
		}
	}

	private void GenerateMesh()
	{
		Mesh mesh = new Mesh();
		float num = ((mode == SubdivisionRowMode.Mini) ? miniInnerRadius : normalInnerRadius);
		float num2 = ((mode == SubdivisionRowMode.Mini) ? miniOuterRadius : normalOuterRadius);
		Vector3[] array = new Vector3[2 * sides];
		int[] array2 = new int[2 * sides * 3];
		for (int i = 0; i < sides; i++)
		{
			float f = (float)i * (MathF.PI * 2f) / (float)sides;
			array[i * 2] = new Vector3(Mathf.Cos(f) * num, Mathf.Sin(f) * num, 0f);
			array[i * 2 + 1] = new Vector3(Mathf.Cos(f) * num2, Mathf.Sin(f) * num2, 0f);
			array2[i * 6] = i * 2 % array.Length;
			array2[i * 6 + 1] = (i * 2 + 3) % array.Length;
			array2[i * 6 + 2] = (i * 2 + 1) % array.Length;
			array2[i * 6 + 3] = i * 2 % array.Length;
			array2[i * 6 + 4] = (i * 2 + 2) % array.Length;
			array2[i * 6 + 5] = (i * 2 + 3) % array.Length;
		}
		mesh.name = "Donut";
		mesh.vertices = array;
		mesh.SetIndices(array2, MeshTopology.Triangles, 0);
		mesh.RecalculateBounds();
		GetComponent<MeshFilter>().mesh = mesh;
	}
}
