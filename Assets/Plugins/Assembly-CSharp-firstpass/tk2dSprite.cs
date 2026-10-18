using System;
using System.Collections.Generic;
// using DG.Tweening; // WebGL: removed
using UnityEngine;

[AddComponentMenu("2D Toolkit/Sprite/tk2dSprite")]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[ExecuteInEditMode]
public class tk2dSprite : tk2dBaseSprite
{
	public Mesh mesh;

	private Vector3[] meshVertices;

	private Vector3[] meshNormals;

	private Vector4[] meshTangents;

	private Color32[] meshColors;

	private const string FlashProperty = "_Flash";

	private const string OverlayColorProperty = "_ColorOverlay";

	public List<object> tweens = new List<object>();

	public float x
	{
		get
		{
			return base.transform.position.x;
		}
		set
		{
			base.transform.position = base.transform.position.WithX(x);
		}
	}

	public float y
	{
		get
		{
			return base.transform.position.y;
		}
		set
		{
			base.transform.position = base.transform.position.WithY(y);
		}
	}

	public float width => GetUntrimmedBounds().extents.x * 2f;

	public float height => GetUntrimmedBounds().extents.y * 2f;

	private Renderer renderer => GetComponent<Renderer>();

	public float flash
	{
		get
		{
			if (renderer != null)
			{
				return renderer.material.GetFloat("_Flash");
			}
			return 0f;
		}
		set
		{
			if (renderer != null)
			{
				renderer.material.SetFloat("_Flash", value);
			}
		}
	}

	public Color overlayColor
	{
		get
		{
			return renderer.material.GetColor("_ColorOverlay");
		}
		set
		{
			renderer.material.SetColor("_ColorOverlay", value);
		}
	}

	private new void Awake()
	{
		base.Awake();
		mesh = new Mesh();
		mesh.MarkDynamic();
		mesh.hideFlags = HideFlags.DontSave;
		GetComponent<MeshFilter>().mesh = mesh;
		if ((bool)base.Collection)
		{
			if (_spriteId < 0 || _spriteId >= base.Collection.Count)
			{
				_spriteId = 0;
			}
			Build();
		}
	}

	protected void OnDestroy()
	{
		if ((bool)mesh)
		{
			UnityEngine.Object.Destroy(mesh);
		}
		if ((bool)meshColliderMesh)
		{
			UnityEngine.Object.Destroy(meshColliderMesh);
		}
	}

	public override void Build()
	{
		tk2dSpriteDefinition tk2dSpriteDefinition2 = collectionInst.spriteDefinitions[base.spriteId];
		meshVertices = new Vector3[tk2dSpriteDefinition2.positions.Length];
		meshColors = new Color32[tk2dSpriteDefinition2.positions.Length];
		meshNormals = new Vector3[0];
		meshTangents = new Vector4[0];
		if (tk2dSpriteDefinition2.normals != null && tk2dSpriteDefinition2.normals.Length != 0)
		{
			meshNormals = new Vector3[tk2dSpriteDefinition2.normals.Length];
		}
		if (tk2dSpriteDefinition2.tangents != null && tk2dSpriteDefinition2.tangents.Length != 0)
		{
			meshTangents = new Vector4[tk2dSpriteDefinition2.tangents.Length];
		}
		SetPositions(ref meshVertices, ref meshNormals, ref meshTangents);
		meshColors = GetColors();
		if (mesh == null)
		{
			mesh = new Mesh();
			mesh.MarkDynamic();
			mesh.hideFlags = HideFlags.DontSave;
			GetComponent<MeshFilter>().mesh = mesh;
		}
		mesh.Clear();
		mesh.vertices = meshVertices;
		mesh.normals = meshNormals;
		mesh.tangents = meshTangents;
		mesh.colors32 = meshColors;
		if (useBend)
		{
			BuildBendMesh();
		}
		else
		{
			mesh.uv = tk2dSpriteDefinition2.uvs;
			mesh.triangles = tk2dSpriteDefinition2.indices;
		}
		mesh.bounds = tk2dBaseSprite.AdjustedMeshBounds(GetBounds(), renderLayer);
		UpdateMaterial();
		CreateCollider();
		if (useBend)
		{
			Bend(curve);
		}
	}

	public static tk2dSprite AddComponent(GameObject go, tk2dSpriteCollectionData spriteCollection, int spriteId)
	{
		return tk2dBaseSprite.AddComponent<tk2dSprite>(go, spriteCollection, spriteId);
	}

	public static tk2dSprite AddComponent(GameObject go, tk2dSpriteCollectionData spriteCollection, string spriteName)
	{
		return tk2dBaseSprite.AddComponent<tk2dSprite>(go, spriteCollection, spriteName);
	}

	public static GameObject CreateFromTexture(Texture texture, tk2dSpriteCollectionSize size, Rect region, Vector2 anchor)
	{
		return tk2dBaseSprite.CreateFromTexture<tk2dSprite>(texture, size, region, anchor);
	}

	protected override void UpdateGeometry()
	{
		UpdateGeometryImpl();
	}

	protected override void UpdateColors()
	{
		UpdateColorsImpl();
	}

	protected override void UpdateVertices()
	{
		UpdateVerticesImpl();
	}

	public Color32[] GetColors()
	{
		Color32 meshColor = GetMeshColor();
		Color32[] array = new Color32[meshVertices.Length];
		for (int i = 0; i < meshVertices.Length; i++)
		{
			array[i] = meshColor;
		}
		return array;
	}

	protected void UpdateColorsImpl()
	{
		if (!(mesh == null) && meshColors != null && meshColors.Length != 0)
		{
			meshColors = GetColors();
			mesh.colors32 = meshColors;
		}
	}

	protected void UpdateVerticesImpl()
	{
		tk2dSpriteDefinition tk2dSpriteDefinition2 = collectionInst.spriteDefinitions[base.spriteId];
		if (!(mesh == null) && meshVertices != null && meshVertices.Length != 0)
		{
			if (tk2dSpriteDefinition2.normals.Length != meshNormals.Length)
			{
				meshNormals = ((tk2dSpriteDefinition2.normals != null && tk2dSpriteDefinition2.normals.Length != 0) ? new Vector3[tk2dSpriteDefinition2.normals.Length] : new Vector3[0]);
			}
			if (tk2dSpriteDefinition2.tangents.Length != meshTangents.Length)
			{
				meshTangents = ((tk2dSpriteDefinition2.tangents != null && tk2dSpriteDefinition2.tangents.Length != 0) ? new Vector4[tk2dSpriteDefinition2.tangents.Length] : new Vector4[0]);
			}
			SetPositions(ref meshVertices, ref meshNormals, ref meshTangents);
			mesh.vertices = meshVertices;
			mesh.normals = meshNormals;
			mesh.tangents = meshTangents;
			if (useBend)
			{
				BuildBendMesh();
			}
			else
			{
				mesh.uv = tk2dSpriteDefinition2.uvs;
			}
			mesh.bounds = tk2dBaseSprite.AdjustedMeshBounds(GetBounds(), renderLayer);
			if (useBend)
			{
				Bend(curve);
			}
		}
	}

	protected void UpdateGeometryImpl()
	{
		if (!(mesh == null))
		{
			tk2dSpriteDefinition tk2dSpriteDefinition2 = collectionInst.spriteDefinitions[base.spriteId];
			if (meshVertices == null || meshVertices.Length != tk2dSpriteDefinition2.positions.Length)
			{
				meshVertices = new Vector3[tk2dSpriteDefinition2.positions.Length];
				meshColors = new Color32[tk2dSpriteDefinition2.positions.Length];
			}
			if (meshNormals == null || (tk2dSpriteDefinition2.normals != null && meshNormals.Length != tk2dSpriteDefinition2.normals.Length))
			{
				meshNormals = new Vector3[tk2dSpriteDefinition2.normals.Length];
			}
			else if (tk2dSpriteDefinition2.normals == null)
			{
				meshNormals = new Vector3[0];
			}
			if (meshTangents == null || (tk2dSpriteDefinition2.tangents != null && meshTangents.Length != tk2dSpriteDefinition2.tangents.Length))
			{
				meshTangents = new Vector4[tk2dSpriteDefinition2.tangents.Length];
			}
			else if (tk2dSpriteDefinition2.tangents == null)
			{
				meshTangents = new Vector4[0];
			}
			SetPositions(ref meshVertices, ref meshNormals, ref meshTangents);
			meshColors = GetColors();
			mesh.Clear();
			mesh.vertices = meshVertices;
			mesh.normals = meshNormals;
			mesh.tangents = meshTangents;
			if (useBend)
			{
				BuildBendMesh();
			}
			else
			{
				mesh.uv = tk2dSpriteDefinition2.uvs;
				mesh.triangles = tk2dSpriteDefinition2.indices;
			}
			mesh.colors32 = meshColors;
			mesh.bounds = tk2dBaseSprite.AdjustedMeshBounds(GetBounds(), renderLayer);
			originalVertices = new List<Vector3>(meshVertices);
			if (useBend)
			{
				Bend(curve);
			}
		}
	}

	protected override void UpdateMaterial()
	{
		Renderer component = GetComponent<Renderer>();
		if (!component.sharedMaterial || component.sharedMaterial.mainTexture != collectionInst.spriteDefinitions[base.spriteId].materialInst.mainTexture)
		{
			component.material = collectionInst.spriteDefinitions[base.spriteId].materialInst;
		}
	}

	protected override int GetCurrentVertexCount()
	{
		if (meshVertices == null)
		{
			return 0;
		}
		return meshVertices.Length;
	}

	public override void ForceBuild()
	{
		base.ForceBuild();
		GetComponent<MeshFilter>().mesh = mesh;
	}

	public override void ReshapeBounds(Vector3 dMin, Vector3 dMax)
	{
		float num = 0.1f;
		tk2dSpriteDefinition currentSprite = base.CurrentSprite;
		Vector3 b = new Vector3(Mathf.Abs(_scale.x), Mathf.Abs(_scale.y), Mathf.Abs(_scale.z));
		Vector3 vector = Vector3.Scale(currentSprite.untrimmedBoundsData[0], _scale) - 0.5f * Vector3.Scale(currentSprite.untrimmedBoundsData[1], b);
		Vector3 vector2 = Vector3.Scale(currentSprite.untrimmedBoundsData[1], b) + dMax - dMin;
		vector2.x /= currentSprite.untrimmedBoundsData[1].x;
		vector2.y /= currentSprite.untrimmedBoundsData[1].y;
		if (currentSprite.untrimmedBoundsData[1].x * vector2.x < currentSprite.texelSize.x * num && vector2.x < b.x)
		{
			dMin.x = 0f;
			vector2.x = b.x;
		}
		if (currentSprite.untrimmedBoundsData[1].y * vector2.y < currentSprite.texelSize.y * num && vector2.y < b.y)
		{
			dMin.y = 0f;
			vector2.y = b.y;
		}
		Vector2 vector3 = new Vector3(Mathf.Approximately(b.x, 0f) ? 0f : (vector2.x / b.x), Mathf.Approximately(b.y, 0f) ? 0f : (vector2.y / b.y));
		Vector3 vector4 = new Vector3(vector.x * vector3.x, vector.y * vector3.y);
		Vector3 position = dMin + vector - vector4;
		position.z = 0f;
		base.transform.position = base.transform.TransformPoint(position);
		base.scale = new Vector3(_scale.x * vector3.x, _scale.y * vector3.y, _scale.z);
	}

	public void UpdateMesh()
	{
		UpdateGeometryImpl();
	}

	public static int[] GetTriangles(Vector3[] vertices)
	{
		List<int> list = new List<int>();
		int num = 1;
		int num2 = 2;
		for (int i = 0; i < vertices.Length - 4; i++)
		{
			if (i % 2 == 0)
			{
				if (i == num2)
				{
					list.Add(i + 4 - 1);
					list.Add(i + 4);
					list.Add(i);
					num2 += 4;
				}
				list.Add(i);
				list.Add(i + 4);
				list.Add(i + 1);
			}
			else
			{
				list.Add(i + 4 - 1);
				list.Add(i + 4);
				list.Add(i);
				if (i == num)
				{
					list.Add(i);
					list.Add(i + 4);
					list.Add(i + 1);
					num += 4;
				}
			}
		}
		return list.ToArray();
	}

	public static Vector2[] GetUVs(Vector2[] spriteUVs, int rowsNumber, float insideWidthPercentage, float insideHeightPercentage, ref Vector2[] uv2)
	{
		float num = float.MaxValue;
		float num2 = float.MinValue;
		float num3 = float.MaxValue;
		float num4 = float.MinValue;
		for (int i = 0; i < spriteUVs.Length; i++)
		{
			Vector2 vector = spriteUVs[i];
			if (vector.x < num)
			{
				num = vector.x;
			}
			else if (vector.x > num2)
			{
				num2 = vector.x;
			}
			if (vector.y < num3)
			{
				num3 = vector.y;
			}
			else if (vector.y > num4)
			{
				num4 = vector.y;
			}
		}
		float num5 = num4 - num3;
		float num6 = num2 - num;
		if (rowsNumber != 0)
		{
			_ = num5 / (float)rowsNumber;
		}
		float num7 = num + num6 / 2f;
		float num8 = num6 * (insideWidthPercentage / 2f);
		float num9 = num5 * insideHeightPercentage;
		float num10 = ((rowsNumber == 0) ? 0f : (num9 / (float)(rowsNumber - 2)));
		float num11 = ((rowsNumber == 0) ? 0f : (num3 + (num5 - num9) / 2f));
		List<Vector2> list = new List<Vector2>();
		List<Vector2> list2 = new List<Vector2>();
		for (int j = 0; j <= rowsNumber; j++)
		{
			float num12 = 0f;
			num12 = ((j != 0) ? ((j != rowsNumber) ? (num11 + num10 * (float)(j - 1)) : (num3 + (num4 - num3))) : num3);
			Vector2 item = new Vector2(num, num12);
			Vector2 item2 = new Vector2(num7 - num8, num12);
			Vector2 item3 = new Vector2(num7 + num8, num12);
			Vector2 item4 = new Vector2(num2, num12);
			list.Add(item);
			list.Add(item2);
			list.Add(item3);
			list.Add(item4);
			list2.Add(new Vector2(Mathf.InverseLerp(num, num2, item.x), Mathf.InverseLerp(num3, num4, item.y)));
			list2.Add(new Vector2(Mathf.InverseLerp(num, num2, item2.x), Mathf.InverseLerp(num3, num4, item2.y)));
			list2.Add(new Vector2(Mathf.InverseLerp(num, num2, item3.x), Mathf.InverseLerp(num3, num4, item3.y)));
			list2.Add(new Vector2(Mathf.InverseLerp(num, num2, item4.x), Mathf.InverseLerp(num3, num4, item4.y)));
		}
		uv2 = list2.ToArray();
		return list.ToArray();
	}

	public void BuildBendMesh()
	{
		mesh.triangles = GetTriangles(meshVertices);
		Vector2[] uv = new Vector2[0];
		mesh.uv = GetUVs(GetCurrentSpriteDef().uvs, rowsNumber, insideWidthPercentage, insideHeightPercentage, ref uv);
		mesh.uv2 = uv;
	}

	public void Bend(float curve)
	{
		if (originalVertices == null)
		{
			originalVertices = new List<Vector3>(meshVertices);
		}
		if (mesh == null)
		{
			return;
		}
		base.curve = curve;
		Vector3[] vertices = mesh.vertices;
		int num = 5;
		int num2 = 6;
		for (int i = 4; i < vertices.Length - 4; i++)
		{
			if (i == num)
			{
				vertices[i] = new Vector3(GetVertexXBendPos(i, base.FirstInsideRowY, base.InsideHeight), vertices[i].y, vertices[i].z);
				num += 4;
			}
			else if (i == num2)
			{
				num2 += 4;
				vertices[i] = new Vector3(GetVertexXBendPos(i, base.FirstInsideRowY, base.InsideHeight), vertices[i].y, vertices[i].z);
			}
		}
		mesh.vertices = vertices;
	}

	private float GetVertexXBendPos(int vertexIndex, float minY, float height)
	{
		float num = (meshVertices[vertexIndex].y - minY) / height;
		float f = MathF.PI * num;
		return originalVertices[vertexIndex].x + Mathf.Sin(f) * curve;
	}

	public void ScaleX(float scale)
	{
		base.scale = base.scale.WithX(scale);
	}

	public void ScaleY(float scale)
	{
		base.scale = base.scale.WithY(scale);
	}

	public void ScaleZ(float scale)
	{
		base.scale = base.scale.WithZ(scale);
	}

	public void ScaleXY(float scale)
	{
		base.scale = new Vector3(scale, scale, 1f);
	}

	public void ScaleXY(Vector2 scale)
	{
		base.scale = new Vector3(scale.x, scale.y, 1f);
	}

	public void Rotate2D(float deg, bool relative = false)
	{
		if (!relative)
		{
			base.transform.localEulerAngles = new Vector3(base.transform.localEulerAngles.x, base.transform.localEulerAngles.y, deg);
		}
		else
		{
			base.transform.localEulerAngles = new Vector3(base.transform.localEulerAngles.x, base.transform.localEulerAngles.y, base.transform.localEulerAngles.z + deg);
		}
	}

	public void KillAllTweens()
	{
		// tweens removed for WebGL
	}
}

