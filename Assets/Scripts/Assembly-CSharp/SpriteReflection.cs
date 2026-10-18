using System;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class SpriteReflection : RDBase
{
	[NonSerialized]
	public bool updateOriginY;

	public CustomAnimation targetAnimation;

	public float extraOffset;

	[SerializeField]
	private float yOffset;

	private MeshRenderer targetRenderer;

	private MeshFilter targetMeshFilter;

	private MeshRenderer renderer;

	private MeshFilter meshFilter;

	private bool isCharacterReflection;

	private bool isJumping;

	private float jumpOriginY;

	private int rowID = -1;

	private Transform reflectionToFollow;

	private void Awake()
	{
		renderer = GetComponent<MeshRenderer>();
		meshFilter = GetComponent<MeshFilter>();
		if (targetAnimation != null)
		{
			SetSprite(targetAnimation, targetAnimation.GetComponent<scrChar>() != null, base.transform);
		}
	}

	public void SetSprite(CustomAnimation target, bool isCharacter, Transform reflectionToFollow = null, int rowID = -1)
	{
		targetAnimation = target;
		targetRenderer = target.GetComponent<MeshRenderer>();
		targetMeshFilter = target.GetComponent<MeshFilter>();
		renderer.material = UnityEngine.Object.Instantiate(targetRenderer.material);
		base.transform.parent = target.transform;
		base.transform.localScale = new Vector3(1f, -1f, 1f);
		isCharacterReflection = isCharacter;
		this.reflectionToFollow = reflectionToFollow;
		this.rowID = rowID;
	}

	private void LateUpdate()
	{
		if (targetAnimation == null)
		{
			return;
		}
		renderer.enabled = targetAnimation.enabled && targetRenderer.enabled && targetAnimation.gameObject.activeSelf;
		if (!renderer.enabled)
		{
			return;
		}
		renderer.material.CopyPropertiesFromMaterial(targetRenderer.material);
		float value = renderer.material.GetFloat(RDShaderProperties.OpacityID) * 0.4f;
		renderer.material.SetFloat(RDShaderProperties.OpacityID, value);
		renderer.material.SetColor(RDShaderProperties.GlowColorID, Color.clear);
		renderer.material.SetFloat(RDShaderProperties.FlashID, targetRenderer.material.GetFloat(RDShaderProperties.FlashID));
		renderer.sortingOrder = targetRenderer.sortingOrder;
		meshFilter.mesh = targetMeshFilter.mesh;
		Transform transform = targetAnimation.transform;
		Vector3 eulerAngles = transform.rotation.eulerAngles;
		eulerAngles = eulerAngles.WithZ(eulerAngles.z * -1f);
		base.transform.rotation = Quaternion.Euler(eulerAngles);
		base.transform.localScale = transform.localScale.WithY(0f - transform.localScale.y);
		yOffset = targetAnimation.currentClip.reflectionOffset + extraOffset;
		if (isCharacterReflection)
		{
			if (!(scnGame.instance != null) || (!scnGame.instance.currentLevel.rowReflectionsJumping && !scnGame.instance.currentLevel.rowReflectionJumpOverrides.Contains(rowID)))
			{
				isJumping = false;
			}
			else if (!isJumping || updateOriginY)
			{
				isJumping = true;
				updateOriginY = false;
				jumpOriginY = transform.position.y;
			}
			if (isJumping)
			{
				float num = transform.position.y - jumpOriginY;
				yOffset += num * 2f;
			}
		}
		else
		{
			yOffset = reflectionToFollow.localPosition.y;
		}
		base.transform.position = transform.position + Vector3.down * (yOffset * transform.localScale.y);
	}
}
