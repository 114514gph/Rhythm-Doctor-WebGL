using System;
using System.Collections.Generic;
using UnityEngine;

public class SmokeTexturizer : RDBase
{
	public Material material;

	public MeshFilter filter;

	public tk2dSprite reference;

	private RenderTexture rt;

	private Material internalMaterial;

	private float time;

	private Renderer renderer;

	private Renderer referenceRenderer;

	private MeshFilter meshFilter;

	private List<Vector3> originalVertices;

	private float curve;

	private Rect meshRect;

	private void Awake()
	{
		rt = new RenderTexture(96, 216, 16, RenderTextureFormat.ARGB32);
		rt.Create();
		RenderTexture active = RenderTexture.active;
		RenderTexture.active = rt;
		GL.Clear(clearDepth: true, clearColor: true, Color.black);
		RenderTexture.active = active;
		internalMaterial = GetComponent<Renderer>().material;
		internalMaterial.mainTexture = rt;
		renderer = GetComponent<Renderer>();
		referenceRenderer = reference.GetComponent<Renderer>();
		meshFilter = GetComponent<MeshFilter>();
	}

	private void OnDestroy()
	{
		rt.Release();
	}

	private void LateUpdate()
	{
		time += Time.deltaTime;
		renderer.enabled = referenceRenderer.enabled;
		if (!renderer.enabled)
		{
			return;
		}
		Mesh mesh = meshFilter.mesh;
		_ = reference.mesh;
		_ = meshRect;
		if (meshRect.width == 0f || meshRect.height == 0f)
		{
			meshRect = tk2dBaseSprite.GetMeshRect(mesh.vertices, Vector3.one);
		}
		Vector3[] positions = mesh.vertices;
		Vector3[] normals = mesh.normals;
		Vector4[] tangents = mesh.tangents;
		float num = meshRect.height * reference.insideHeightPercentage;
		float insideRowHeight = ((reference.rowsNumber == 0) ? 0f : (num / (float)(reference.rowsNumber - 2)));
		float num2 = ((reference.rowsNumber == 0) ? 0f : (meshRect.y + (meshRect.height - num) / 2f));
		tk2dBaseSprite.SetPositions(ref positions, ref normals, ref tangents, meshRect, reference.rowsNumber, reference.insideWidthPercentage, num2, insideRowHeight);
		int[] triangles = tk2dSprite.GetTriangles(positions);
		Vector2[] spriteUVs = new Vector2[4]
		{
			new Vector2(0f, 0f),
			new Vector2(1f, 0f),
			new Vector2(0f, 1f),
			new Vector2(1f, 1f)
		};
		Vector2[] uv = new Vector2[0];
		Vector2[] uVs = tk2dSprite.GetUVs(spriteUVs, reference.rowsNumber, reference.insideWidthPercentage, reference.insideHeightPercentage, ref uv);
		mesh.vertices = positions;
		originalVertices = new List<Vector3>(mesh.vertices);
		mesh.normals = normals;
		mesh.tangents = tangents;
		mesh.triangles = triangles;
		mesh.uv = uVs;
		if (originalVertices == null)
		{
			originalVertices = new List<Vector3>(positions);
		}
		curve = reference.curve / 24f;
		int num3 = 5;
		int num4 = 6;
		for (int i = 4; i < positions.Length - 4; i++)
		{
			if (i == num3)
			{
				positions[i] = new Vector3(GetVertexXBendPos(i, num2, num), positions[i].y, positions[i].z);
				num3 += 4;
			}
			else if (i == num4)
			{
				num4 += 4;
				positions[i] = new Vector3(GetVertexXBendPos(i, num2, num), positions[i].y, positions[i].z);
			}
		}
		mesh.vertices = positions;
		this.material.SetFloat("_TimeU", time);
		Material material = reference.GetComponent<MeshRenderer>().material;
		this.material.SetTexture("_Reference", material.mainTexture);
		tk2dSpriteDefinition currentSpriteDef = reference.GetCurrentSpriteDef();
		this.material.SetTextureOffset("_Reference", currentSpriteDef.uvs[0]);
		this.material.SetTextureScale("_Reference", new Vector2(currentSpriteDef.uvs[3].x - currentSpriteDef.uvs[0].x, currentSpriteDef.uvs[3].y - currentSpriteDef.uvs[0].y));
		internalMaterial.color = base.gc.beatboxColorSmoke;
		Graphics.Blit(null, rt, this.material);
	}

	private float GetVertexXBendPos(int vertexIndex, float minY, float height)
	{
		float num = (meshFilter.mesh.vertices[vertexIndex].y - minY) / height;
		float f = MathF.PI * num;
		return originalVertices[vertexIndex].x + Mathf.Sin(f) * curve;
	}
}
