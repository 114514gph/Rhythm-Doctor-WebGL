using System;
using System.Collections.Generic;
using UnityEngine;
using tk2dRuntime;

[AddComponentMenu("2D Toolkit/Backend/tk2dBaseSprite")]
public abstract class tk2dBaseSprite : MonoBehaviour, ISpriteCollectionForceBuild
{
	public enum Anchor
	{
		LowerLeft = 0,
		LowerCenter = 1,
		LowerRight = 2,
		MiddleLeft = 3,
		MiddleCenter = 4,
		MiddleRight = 5,
		UpperLeft = 6,
		UpperCenter = 7,
		UpperRight = 8
	}

	[SerializeField]
	private tk2dSpriteCollectionData collection;

	protected tk2dSpriteCollectionData collectionInst;

	[SerializeField]
	protected Color _color = Color.white;

	[SerializeField]
	protected Vector3 _scale = new Vector3(1f, 1f, 1f);

	[SerializeField]
	protected int _spriteId;

	public BoxCollider2D boxCollider2D;

	public List<PolygonCollider2D> polygonCollider2D = new List<PolygonCollider2D>(1);

	public List<EdgeCollider2D> edgeCollider2D = new List<EdgeCollider2D>(1);

	public BoxCollider boxCollider;

	public MeshCollider meshCollider;

	public Vector3[] meshColliderPositions;

	public Mesh meshColliderMesh;

	private Renderer _cachedRenderer;

	[SerializeField]
	protected int renderLayer;

	public const int ColumnsNumber = 4;

	public const int InsideLeftStartIndex = 1;

	public const int InsideRightStartIndex = 2;

	public bool useBend;

	public float curve;

	public int rowsNumber = 10;

	public float insideWidthPercentage = 0.5f;

	public float insideHeightPercentage = 0.75f;

	protected List<Vector3> originalVertices;

	private Rect _meshRect;

	public tk2dSpriteCollectionData Collection
	{
		get
		{
			return collection;
		}
		set
		{
			collection = value;
			collectionInst = collection.inst;
		}
	}

	public Color color
	{
		get
		{
			return _color;
		}
		set
		{
			if (value != _color)
			{
				_color = value;
				InitInstance();
				UpdateColors();
			}
		}
	}

	public Vector3 scale
	{
		get
		{
			return _scale;
		}
		set
		{
			if (value != _scale)
			{
				_scale = value;
				InitInstance();
				UpdateVertices();
				UpdateCollider();
				if (SpriteChanged != null)
				{
					SpriteChanged(this);
				}
			}
		}
	}

	private Renderer CachedRenderer
	{
		get
		{
			if (_cachedRenderer == null)
			{
				_cachedRenderer = GetComponent<Renderer>();
			}
			return _cachedRenderer;
		}
	}

	public int SortingOrder
	{
		get
		{
			return CachedRenderer.sortingOrder;
		}
		set
		{
			if (CachedRenderer.sortingOrder != value)
			{
				renderLayer = value;
				CachedRenderer.sortingOrder = value;
			}
		}
	}

	public bool FlipX
	{
		get
		{
			return _scale.x < 0f;
		}
		set
		{
			scale = new Vector3(Mathf.Abs(_scale.x) * (float)((!value) ? 1 : (-1)), _scale.y, _scale.z);
		}
	}

	public bool FlipY
	{
		get
		{
			return _scale.y < 0f;
		}
		set
		{
			scale = new Vector3(_scale.x, Mathf.Abs(_scale.y) * (float)((!value) ? 1 : (-1)), _scale.z);
		}
	}

	public int spriteId
	{
		get
		{
			return _spriteId;
		}
		set
		{
			if (value != _spriteId)
			{
				InitInstance();
				value = Mathf.Clamp(value, 0, collectionInst.spriteDefinitions.Length - 1);
				if (_spriteId < 0 || _spriteId >= collectionInst.spriteDefinitions.Length || GetCurrentVertexCount() != collectionInst.spriteDefinitions[value].positions.Length || collectionInst.spriteDefinitions[_spriteId].complexGeometry != collectionInst.spriteDefinitions[value].complexGeometry)
				{
					_spriteId = value;
					UpdateGeometry();
				}
				else
				{
					_spriteId = value;
					UpdateVertices();
				}
				UpdateMaterial();
				UpdateCollider();
				if (SpriteChanged != null)
				{
					SpriteChanged(this);
				}
			}
		}
	}

	protected Rect MeshRect
	{
		get
		{
			_ = _meshRect;
			if (_meshRect.width == 0f || _meshRect.height == 0f)
			{
				tk2dSpriteDefinition tk2dSpriteDefinition2 = collectionInst.spriteDefinitions[spriteId];
				GetNumVertices();
				_meshRect = GetMeshRect(tk2dSpriteDefinition2.positions, _scale);
			}
			return _meshRect;
		}
	}

	protected float InsideHeight => MeshRect.height * insideHeightPercentage;

	public float InsideRowHeight
	{
		get
		{
			if (rowsNumber != 0)
			{
				return InsideHeight / (float)(rowsNumber - 2);
			}
			return 0f;
		}
	}

	public float FirstInsideRowY
	{
		get
		{
			if (rowsNumber != 0)
			{
				return MeshRect.y + (MeshRect.height - InsideHeight) / 2f;
			}
			return 0f;
		}
	}

	public tk2dSpriteDefinition CurrentSprite
	{
		get
		{
			InitInstance();
			if (!(collectionInst == null))
			{
				return collectionInst.spriteDefinitions[_spriteId];
			}
			return null;
		}
	}

	public event Action<tk2dBaseSprite> SpriteChanged;

	private void InitInstance()
	{
		if (collectionInst == null && collection != null)
		{
			collectionInst = collection.inst;
		}
	}

	public void SetSprite(int newSpriteId)
	{
		spriteId = newSpriteId;
	}

	public bool SetSprite(string spriteName)
	{
		int spriteIdByName = collection.GetSpriteIdByName(spriteName, -1);
		if (spriteIdByName != -1)
		{
			SetSprite(spriteIdByName);
		}
		else
		{
			Debug.LogError("SetSprite - Sprite not found in collection: " + spriteName);
		}
		return spriteIdByName != -1;
	}

	public void SetSprite(tk2dSpriteCollectionData newCollection, int newSpriteId)
	{
		bool flag = false;
		if (Collection != newCollection)
		{
			collection = newCollection;
			collectionInst = collection.inst;
			_spriteId = -1;
			flag = true;
		}
		spriteId = newSpriteId;
		if (flag)
		{
			UpdateMaterial();
		}
	}

	public bool SetSprite(tk2dSpriteCollectionData newCollection, string spriteName)
	{
		if (newCollection == null)
		{
			return false;
		}
		int spriteIdByName = newCollection.GetSpriteIdByName(spriteName, -1);
		if (spriteIdByName != -1)
		{
			SetSprite(newCollection, spriteIdByName);
		}
		else
		{
			Debug.LogError("SetSprite - Sprite not found in collection: " + spriteName);
		}
		return spriteIdByName != -1;
	}

	public void MakePixelPerfect()
	{
		float num = 1f;
		if ((bool)Camera.main)
		{
			if (Camera.main.orthographic)
			{
				num = Camera.main.orthographicSize;
			}
			else
			{
				float zdist = base.transform.position.z - Camera.main.transform.position.z;
				num = tk2dPixelPerfectHelper.CalculateScaleForPerspectiveCamera(Camera.main.fieldOfView, zdist);
			}
			num *= Collection.invOrthoSize;
		}
		else
		{
			Debug.LogError("Main camera not found.");
		}
		scale = new Vector3(Mathf.Sign(scale.x) * num, Mathf.Sign(scale.y) * num, Mathf.Sign(scale.z) * num);
	}

	protected abstract void UpdateMaterial();

	protected abstract void UpdateColors();

	protected abstract void UpdateVertices();

	protected abstract void UpdateGeometry();

	protected abstract int GetCurrentVertexCount();

	public abstract void Build();

	public int GetSpriteIdByName(string name)
	{
		InitInstance();
		return collectionInst.GetSpriteIdByName(name);
	}

	public static T AddComponent<T>(GameObject go, tk2dSpriteCollectionData spriteCollection, int spriteId) where T : tk2dBaseSprite
	{
		T val = go.AddComponent<T>();
		val._spriteId = -1;
		val.SetSprite(spriteCollection, spriteId);
		val.Build();
		return val;
	}

	public static T AddComponent<T>(GameObject go, tk2dSpriteCollectionData spriteCollection, string spriteName) where T : tk2dBaseSprite
	{
		int spriteIdByName = spriteCollection.GetSpriteIdByName(spriteName, -1);
		if (spriteIdByName == -1)
		{
			Debug.LogError($"Unable to find sprite named {spriteName} in sprite collection {spriteCollection.spriteCollectionName}");
			return null;
		}
		return AddComponent<T>(go, spriteCollection, spriteIdByName);
	}

	protected int GetNumVertices()
	{
		InitInstance();
		return collectionInst.spriteDefinitions[spriteId].positions.Length;
	}

	protected int GetNumIndices()
	{
		InitInstance();
		return collectionInst.spriteDefinitions[spriteId].indices.Length;
	}

	public static Rect GetMeshRect(Vector3[] positions, Vector3 scale)
	{
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		float num4 = 0f;
		for (int i = 0; i < positions.Length; i++)
		{
			float num5 = positions[i].x * scale.x;
			float num6 = positions[i].y * scale.y;
			_ = ref positions[i];
			if (num5 < num)
			{
				num = num5;
			}
			else if (num5 > num2)
			{
				num2 = num5;
			}
			if (num6 < num3)
			{
				num3 = num6;
			}
			else if (num6 > num4)
			{
				num4 = num6;
			}
		}
		return new Rect(num, num3, num2 - num, num4 - num3);
	}

	public static void SetPositions(ref Vector3[] positions, ref Vector3[] normals, ref Vector4[] tangents, Rect meshRect, int rowsNumber, float insideWidthPercentage, float firstInsideRowY, float insideRowHeight)
	{
		List<Vector3> list = new List<Vector3>();
		float num = meshRect.x + meshRect.width / 2f;
		float num2 = meshRect.width * (insideWidthPercentage / 2f);
		for (int i = 0; i <= rowsNumber; i++)
		{
			float num3 = 0f;
			num3 = ((i != 0) ? ((i != rowsNumber) ? (firstInsideRowY + insideRowHeight * (float)(i - 1)) : (meshRect.y + meshRect.height)) : meshRect.y);
			list.Add(new Vector3(meshRect.x, num3, 0f));
			list.Add(new Vector3(num - num2, num3, 0f));
			list.Add(new Vector3(num + num2, num3, 0f));
			list.Add(new Vector3(meshRect.x + meshRect.width, num3, 0f));
		}
		positions = list.ToArray();
		normals = new Vector3[list.Count];
		for (int j = 0; j < normals.Length; j++)
		{
			normals[j] = Vector3.forward;
		}
		tangents = new Vector4[0];
	}

	protected void SetPositions(ref Vector3[] positions, ref Vector3[] normals, ref Vector4[] tangents)
	{
		if (useBend)
		{
			SetPositions(ref positions, ref normals, ref tangents, MeshRect, rowsNumber, insideWidthPercentage, FirstInsideRowY, InsideRowHeight);
			return;
		}
		tk2dSpriteDefinition tk2dSpriteDefinition2 = collectionInst.spriteDefinitions[spriteId];
		int numVertices = GetNumVertices();
		for (int i = 0; i < numVertices; i++)
		{
			positions[i].x = tk2dSpriteDefinition2.positions[i].x * _scale.x;
			positions[i].y = tk2dSpriteDefinition2.positions[i].y * _scale.y;
			positions[i].z = tk2dSpriteDefinition2.positions[i].z * _scale.z;
		}
		int num = tk2dSpriteDefinition2.normals.Length;
		if (normals.Length == num)
		{
			for (int j = 0; j < num; j++)
			{
				normals[j] = tk2dSpriteDefinition2.normals[j];
			}
		}
		int num2 = tk2dSpriteDefinition2.tangents.Length;
		if (tangents.Length == num2)
		{
			for (int k = 0; k < num2; k++)
			{
				tangents[k] = tk2dSpriteDefinition2.tangents[k];
			}
		}
	}

	protected Color32 GetMeshColor()
	{
		Color color = _color;
		if (collectionInst.premultipliedAlpha)
		{
			color.r *= color.a;
			color.g *= color.a;
			color.b *= color.a;
		}
		return color;
	}

	protected void SetColors(Color32[] dest)
	{
		Color color = _color;
		if (collectionInst.premultipliedAlpha)
		{
			color.r *= color.a;
			color.g *= color.a;
			color.b *= color.a;
		}
		Color32 color2 = color;
		int numVertices = GetNumVertices();
		for (int i = 0; i < numVertices; i++)
		{
			dest[i] = color2;
		}
	}

	public Bounds GetBounds()
	{
		InitInstance();
		tk2dSpriteDefinition tk2dSpriteDefinition2 = collectionInst.spriteDefinitions[_spriteId];
		return new Bounds(new Vector3(tk2dSpriteDefinition2.boundsData[0].x * _scale.x, tk2dSpriteDefinition2.boundsData[0].y * _scale.y, tk2dSpriteDefinition2.boundsData[0].z * _scale.z), new Vector3(tk2dSpriteDefinition2.boundsData[1].x * Mathf.Abs(_scale.x), tk2dSpriteDefinition2.boundsData[1].y * Mathf.Abs(_scale.y), tk2dSpriteDefinition2.boundsData[1].z * Mathf.Abs(_scale.z)));
	}

	public Bounds GetUntrimmedBounds()
	{
		InitInstance();
		tk2dSpriteDefinition tk2dSpriteDefinition2 = collectionInst.spriteDefinitions[_spriteId];
		return new Bounds(new Vector3(tk2dSpriteDefinition2.untrimmedBoundsData[0].x * _scale.x, tk2dSpriteDefinition2.untrimmedBoundsData[0].y * _scale.y, tk2dSpriteDefinition2.untrimmedBoundsData[0].z * _scale.z), new Vector3(tk2dSpriteDefinition2.untrimmedBoundsData[1].x * Mathf.Abs(_scale.x), tk2dSpriteDefinition2.untrimmedBoundsData[1].y * Mathf.Abs(_scale.y), tk2dSpriteDefinition2.untrimmedBoundsData[1].z * Mathf.Abs(_scale.z)));
	}

	public static Bounds AdjustedMeshBounds(Bounds bounds, int renderLayer)
	{
		Vector3 center = bounds.center;
		center.z = (float)(-renderLayer) * 0.01f;
		bounds.center = center;
		return bounds;
	}

	public tk2dSpriteDefinition GetCurrentSpriteDef()
	{
		InitInstance();
		if (!(collectionInst == null))
		{
			return collectionInst.spriteDefinitions[_spriteId];
		}
		return null;
	}

	public virtual void ReshapeBounds(Vector3 dMin, Vector3 dMax)
	{
	}

	protected virtual bool NeedBoxCollider()
	{
		return false;
	}

	protected virtual void UpdateCollider()
	{
		tk2dSpriteDefinition tk2dSpriteDefinition2 = collectionInst.spriteDefinitions[_spriteId];
		if (tk2dSpriteDefinition2.physicsEngine == tk2dSpriteDefinition.PhysicsEngine.Physics3D)
		{
			if (tk2dSpriteDefinition2.colliderType == tk2dSpriteDefinition.ColliderType.Box && boxCollider == null)
			{
				boxCollider = base.gameObject.GetComponent<BoxCollider>();
				if (boxCollider == null)
				{
					boxCollider = base.gameObject.AddComponent<BoxCollider>();
				}
			}
			if (boxCollider != null)
			{
				if (tk2dSpriteDefinition2.colliderType == tk2dSpriteDefinition.ColliderType.Box)
				{
					boxCollider.center = new Vector3(tk2dSpriteDefinition2.colliderVertices[0].x * _scale.x, tk2dSpriteDefinition2.colliderVertices[0].y * _scale.y, tk2dSpriteDefinition2.colliderVertices[0].z * _scale.z);
					boxCollider.size = new Vector3(Mathf.Abs(2f * tk2dSpriteDefinition2.colliderVertices[1].x * _scale.x), Mathf.Abs(2f * tk2dSpriteDefinition2.colliderVertices[1].y * _scale.y), Mathf.Abs(2f * tk2dSpriteDefinition2.colliderVertices[1].z * _scale.z));
				}
				else if (tk2dSpriteDefinition2.colliderType != tk2dSpriteDefinition.ColliderType.Unset && boxCollider != null)
				{
					boxCollider.center = new Vector3(0f, 0f, -100000f);
					boxCollider.size = Vector3.zero;
				}
			}
		}
		else
		{
			if (tk2dSpriteDefinition2.physicsEngine != tk2dSpriteDefinition.PhysicsEngine.Physics2D)
			{
				return;
			}
			if (tk2dSpriteDefinition2.colliderType == tk2dSpriteDefinition.ColliderType.Box)
			{
				if (boxCollider2D == null)
				{
					boxCollider2D = base.gameObject.GetComponent<BoxCollider2D>();
					if (boxCollider2D == null)
					{
						boxCollider2D = base.gameObject.AddComponent<BoxCollider2D>();
					}
				}
				if (polygonCollider2D.Count > 0)
				{
					foreach (PolygonCollider2D item in polygonCollider2D)
					{
						if (item != null && item.enabled)
						{
							item.enabled = false;
						}
					}
				}
				if (edgeCollider2D.Count > 0)
				{
					foreach (EdgeCollider2D item2 in edgeCollider2D)
					{
						if (item2 != null && item2.enabled)
						{
							item2.enabled = false;
						}
					}
				}
				if (!boxCollider2D.enabled)
				{
					boxCollider2D.enabled = true;
				}
				boxCollider2D.offset = new Vector2(tk2dSpriteDefinition2.colliderVertices[0].x * _scale.x, tk2dSpriteDefinition2.colliderVertices[0].y * _scale.y);
				boxCollider2D.size = new Vector2(Mathf.Abs(2f * tk2dSpriteDefinition2.colliderVertices[1].x * _scale.x), Mathf.Abs(2f * tk2dSpriteDefinition2.colliderVertices[1].y * _scale.y));
			}
			else if (tk2dSpriteDefinition2.colliderType == tk2dSpriteDefinition.ColliderType.Mesh)
			{
				if (boxCollider2D != null && boxCollider2D.enabled)
				{
					boxCollider2D.enabled = false;
				}
				int num = tk2dSpriteDefinition2.polygonCollider2D.Length;
				for (int i = 0; i < polygonCollider2D.Count; i++)
				{
					if (polygonCollider2D[i] == null)
					{
						polygonCollider2D[i] = base.gameObject.AddComponent<PolygonCollider2D>();
					}
				}
				while (polygonCollider2D.Count < num)
				{
					polygonCollider2D.Add(base.gameObject.AddComponent<PolygonCollider2D>());
				}
				for (int j = 0; j < num; j++)
				{
					if (!polygonCollider2D[j].enabled)
					{
						polygonCollider2D[j].enabled = true;
					}
					if (_scale.x != 1f || _scale.y != 1f)
					{
						Vector2[] points = tk2dSpriteDefinition2.polygonCollider2D[j].points;
						Vector2[] array = new Vector2[points.Length];
						for (int k = 0; k < points.Length; k++)
						{
							array[k] = Vector2.Scale(points[k], _scale);
						}
						polygonCollider2D[j].points = array;
					}
					else
					{
						polygonCollider2D[j].points = tk2dSpriteDefinition2.polygonCollider2D[j].points;
					}
				}
				for (int l = num; l < polygonCollider2D.Count; l++)
				{
					if (polygonCollider2D[l].enabled)
					{
						polygonCollider2D[l].enabled = false;
					}
				}
				int num2 = tk2dSpriteDefinition2.edgeCollider2D.Length;
				for (int m = 0; m < edgeCollider2D.Count; m++)
				{
					if (edgeCollider2D[m] == null)
					{
						edgeCollider2D[m] = base.gameObject.AddComponent<EdgeCollider2D>();
					}
				}
				while (edgeCollider2D.Count < num2)
				{
					edgeCollider2D.Add(base.gameObject.AddComponent<EdgeCollider2D>());
				}
				for (int n = 0; n < num2; n++)
				{
					if (!edgeCollider2D[n].enabled)
					{
						edgeCollider2D[n].enabled = true;
					}
					if (_scale.x != 1f || _scale.y != 1f)
					{
						Vector2[] points2 = tk2dSpriteDefinition2.edgeCollider2D[n].points;
						Vector2[] array2 = new Vector2[points2.Length];
						for (int num3 = 0; num3 < points2.Length; num3++)
						{
							array2[num3] = Vector2.Scale(points2[num3], _scale);
						}
						edgeCollider2D[n].points = array2;
					}
					else
					{
						edgeCollider2D[n].points = tk2dSpriteDefinition2.edgeCollider2D[n].points;
					}
				}
				for (int num4 = num2; num4 < edgeCollider2D.Count; num4++)
				{
					if (edgeCollider2D[num4].enabled)
					{
						edgeCollider2D[num4].enabled = false;
					}
				}
			}
			else
			{
				if (tk2dSpriteDefinition2.colliderType != tk2dSpriteDefinition.ColliderType.None)
				{
					return;
				}
				if (boxCollider2D != null && boxCollider2D.enabled)
				{
					boxCollider2D.enabled = false;
				}
				if (polygonCollider2D.Count > 0)
				{
					foreach (PolygonCollider2D item3 in polygonCollider2D)
					{
						if (item3 != null && item3.enabled)
						{
							item3.enabled = false;
						}
					}
				}
				if (edgeCollider2D.Count <= 0)
				{
					return;
				}
				foreach (EdgeCollider2D item4 in edgeCollider2D)
				{
					if (item4 != null && item4.enabled)
					{
						item4.enabled = false;
					}
				}
			}
		}
	}

	protected virtual void CreateCollider()
	{
		tk2dSpriteDefinition tk2dSpriteDefinition2 = collectionInst.spriteDefinitions[_spriteId];
		if (tk2dSpriteDefinition2.colliderType == tk2dSpriteDefinition.ColliderType.Unset)
		{
			return;
		}
		if (tk2dSpriteDefinition2.physicsEngine == tk2dSpriteDefinition.PhysicsEngine.Physics3D)
		{
			if (GetComponent<Collider>() != null)
			{
				boxCollider = GetComponent<BoxCollider>();
				meshCollider = GetComponent<MeshCollider>();
			}
			if ((NeedBoxCollider() || tk2dSpriteDefinition2.colliderType == tk2dSpriteDefinition.ColliderType.Box) && meshCollider == null)
			{
				if (boxCollider == null)
				{
					boxCollider = base.gameObject.AddComponent<BoxCollider>();
				}
			}
			else if (tk2dSpriteDefinition2.colliderType == tk2dSpriteDefinition.ColliderType.Mesh && boxCollider == null)
			{
				if (meshCollider == null)
				{
					meshCollider = base.gameObject.AddComponent<MeshCollider>();
				}
				if (meshColliderMesh == null)
				{
					meshColliderMesh = new Mesh();
				}
				meshColliderMesh.Clear();
				meshColliderPositions = new Vector3[tk2dSpriteDefinition2.colliderVertices.Length];
				for (int i = 0; i < meshColliderPositions.Length; i++)
				{
					meshColliderPositions[i] = new Vector3(tk2dSpriteDefinition2.colliderVertices[i].x * _scale.x, tk2dSpriteDefinition2.colliderVertices[i].y * _scale.y, tk2dSpriteDefinition2.colliderVertices[i].z * _scale.z);
				}
				meshColliderMesh.vertices = meshColliderPositions;
				float num = _scale.x * _scale.y * _scale.z;
				meshColliderMesh.triangles = ((num >= 0f) ? tk2dSpriteDefinition2.colliderIndicesFwd : tk2dSpriteDefinition2.colliderIndicesBack);
				meshCollider.sharedMesh = meshColliderMesh;
				meshCollider.convex = tk2dSpriteDefinition2.colliderConvex;
				if ((bool)GetComponent<Rigidbody>())
				{
					GetComponent<Rigidbody>().centerOfMass = Vector3.zero;
				}
			}
			else if (tk2dSpriteDefinition2.colliderType != tk2dSpriteDefinition.ColliderType.None && Application.isPlaying)
			{
				Debug.LogError("Invalid mesh collider on sprite '" + base.name + "', please remove and try again.");
			}
			UpdateCollider();
		}
		else if (tk2dSpriteDefinition2.physicsEngine == tk2dSpriteDefinition.PhysicsEngine.Physics2D)
		{
			UpdateCollider();
		}
	}

	protected void Awake()
	{
		if (collection != null)
		{
			collectionInst = collection.inst;
		}
		CachedRenderer.sortingOrder = renderLayer;
	}

	public void CreateSimpleBoxCollider()
	{
		if (CurrentSprite == null)
		{
			return;
		}
		if (CurrentSprite.physicsEngine == tk2dSpriteDefinition.PhysicsEngine.Physics3D)
		{
			boxCollider2D = GetComponent<BoxCollider2D>();
			if (boxCollider2D != null)
			{
				UnityEngine.Object.DestroyImmediate(boxCollider2D, allowDestroyingAssets: true);
			}
			boxCollider = GetComponent<BoxCollider>();
			if (boxCollider == null)
			{
				boxCollider = base.gameObject.AddComponent<BoxCollider>();
			}
		}
		else if (CurrentSprite.physicsEngine == tk2dSpriteDefinition.PhysicsEngine.Physics2D)
		{
			boxCollider = GetComponent<BoxCollider>();
			if (boxCollider != null)
			{
				UnityEngine.Object.DestroyImmediate(boxCollider, allowDestroyingAssets: true);
			}
			boxCollider2D = GetComponent<BoxCollider2D>();
			if (boxCollider2D == null)
			{
				boxCollider2D = base.gameObject.AddComponent<BoxCollider2D>();
			}
		}
	}

	public bool UsesSpriteCollection(tk2dSpriteCollectionData spriteCollection)
	{
		return Collection == spriteCollection;
	}

	public virtual void ForceBuild()
	{
		if (!(collection == null))
		{
			collectionInst = collection.inst;
			if (spriteId < 0 || spriteId >= collectionInst.spriteDefinitions.Length)
			{
				spriteId = 0;
			}
			Build();
			if (SpriteChanged != null)
			{
				SpriteChanged(this);
			}
		}
	}

	public static GameObject CreateFromTexture<T>(Texture texture, tk2dSpriteCollectionSize size, Rect region, Vector2 anchor) where T : tk2dBaseSprite
	{
		tk2dSpriteCollectionData tk2dSpriteCollectionData2 = SpriteCollectionGenerator.CreateFromTexture(texture, size, region, anchor);
		if (tk2dSpriteCollectionData2 == null)
		{
			return null;
		}
		GameObject obj = new GameObject();
		AddComponent<T>(obj, tk2dSpriteCollectionData2, 0);
		return obj;
	}
}
