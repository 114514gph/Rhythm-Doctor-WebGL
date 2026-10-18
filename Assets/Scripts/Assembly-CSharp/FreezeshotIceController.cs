using System;
using DG.Tweening;
using UnityEngine;

public class FreezeshotIceController : RDBase
{
	[Serializable]
	public class EntityCollection
	{
		public Entity[] entities;
	}

	[Header("References")]
	public Entity iceBlock;

	public EntityCollection[] spriteCollections;

	[Header("Private")]
	private bool usesCustomSpriteCollection;

	private Vector2 size;

	private Entity[] sprites;

	private Sequence[] sequences;

	public bool earlyExplosion => scnGame.instance?.currentLevel?.earlyFreezeshotExplosion == true;

	public bool noExplosion => scnGame.instance?.currentLevel?.noFreezeshotExplosion == true;

	public void Explode()
	{
		if (noExplosion || (usesCustomSpriteCollection && sprites == null))
		{
			return;
		}
		int num = sprites.Length - 1;
		for (int i = 0; i < num; i++)
		{
			Entity seg = sprites[i + 1];
			sequences[i]?.Kill();
			float f = MathF.PI * 2f * (float)(-i) / (float)num + MathF.PI / 2f;
			Vector2 vector = new Vector2(Mathf.Cos(f) * 20f, Mathf.Sin(f) * 20f);
			seg.visible = true;
			seg.transform.localPosition = Vector3.zero;
			seg.sprite.color = iceBlock.sprite.color;
			seg.SetOverlayColor(iceBlock.shaderDataToUse.overlayColor);
			seg.GetComponent<Entity>().shaderData.overlayTexturePower = 1f;
			sequences[i] = DOTween.Sequence().Insert(0f, seg.transform.DOLocalMove(vector, 0.5f).SetEase(Ease.OutCirc)).Insert(0f, seg.sprite.DOFade(0f, 0.5f).SetEase(Ease.OutSine))
				.OnComplete(() =>
				{
					seg.visible = false;
				});
			if (earlyExplosion)
			{
				sequences[i].Goto(0.05f, andPlay: true);
			}
		}
	}

	public void SetupBasic()
	{
		usesCustomSpriteCollection = false;
		iceBlock.transform.localScale = Vector3.one;
		sprites = spriteCollections[UnityEngine.Random.Range(0, spriteCollections.Length)].entities;
		iceBlock.sprite.SetSprite(sprites[0].sprite.spriteId);
		sequences = new Sequence[sprites.Length - 1];
		Entity[] array = sprites;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].usesOwnShaderData = true;
		}
	}

	public void Setup(Texture2D iceTexture)
	{
		usesCustomSpriteCollection = true;
		int num = ((iceTexture.width % iceTexture.height != 0) ? 1 : (iceTexture.width / iceTexture.height));
		size = ((num == 1) ? new Vector2(iceTexture.width, iceTexture.height) : (Vector2.one * iceTexture.height));
		sprites = new Entity[num];
		sprites[0] = iceBlock;
		sprites[0].transform.localScale = Vector3.one;
		if (num > 1)
		{
			Transform parent = CreateFragmentsContainer();
			sequences = new Sequence[num - 1];
			for (int i = 1; i < num; i++)
			{
				Entity component = UnityEngine.Object.Instantiate(iceBlock, parent).GetComponent<Entity>();
				component.Setup();
				component.usesOwnShaderData = true;
				component.name = $"Fragment{i}";
				sprites[i] = component;
			}
		}
		Vector3 vector = new Vector3(0.5f, 0.5f, 0f);
		for (int j = 0; j < num; j++)
		{
			Entity entity = sprites[j];
			entity.usesOwnShaderData = true;
			entity.visible = false;
			Material material = entity.shaderRenderer.material;
			material.mainTexture = iceTexture;
			material.SetTexture(RDShaderProperties.GlowTexID, Texture2D.blackTexture);
			material.SetTexture(RDShaderProperties.OutlineTexID, Texture2D.blackTexture);
			MeshFilter component2 = entity.GetComponent<MeshFilter>();
			Mesh mesh = component2.mesh;
			entity.sprite.mesh = component2.mesh;
			mesh.vertices = new Vector3[4]
			{
				new Vector3(0f, 0f, 0f) - vector,
				new Vector3(1f, 0f, 0f) - vector,
				new Vector3(0f, 1f, 0f) - vector,
				new Vector3(1f, 1f, 0f) - vector
			};
			mesh.RecalculateBounds();
			int[] indices = new int[6] { 0, 3, 1, 0, 2, 3 };
			mesh.SetIndices(indices, MeshTopology.Triangles, 0);
			mesh.colors = new Color[4]
			{
				Color.white,
				Color.white,
				Color.white,
				Color.white
			};
			Vector2[] array = GetUVs(j);
			mesh.SetUVs(0, array);
			material.SetVector(RDShaderProperties.UVBoundsID, new Vector4(array[0].x, array[0].y, array[3].x, array[3].y));
			entity.transform.localScale = entity.transform.localScale * size;
		}
		Vector2[] GetUVs(int index)
		{
			float num2 = size.x * (float)index / (float)iceTexture.width;
			float num3 = size.x * (float)(index + 1) / (float)iceTexture.width;
			return new Vector2[4]
			{
				new Vector2(num2, 0f),
				new Vector2(num3, 0f),
				new Vector2(num2, 1f),
				new Vector2(num3, 1f)
			};
		}
	}

	private Transform CreateFragmentsContainer()
	{
		GameObject obj = new GameObject("FreezeshotIceFragments");
		obj.transform.SetParent(base.transform, worldPositionStays: false);
		return obj.transform;
	}
}
