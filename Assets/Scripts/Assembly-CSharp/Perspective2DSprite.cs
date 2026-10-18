using System;
using UnityEngine;

[RequireComponent(typeof(tk2dSprite))]
public class Perspective2DSprite : MonoBehaviour
{
	[Header("Transform")]
	public Perspective2DRoom room;

	public Vector3 position;

	public Vector3 scale = Vector3.one;

	[Header("Visual")]
	public Vector3 spritePivot;

	public Color shadowColor;

	public bool renderShadow = true;

	[Header("References")]
	public tk2dSprite sprite;

	public tk2dSprite shadow;

	[NonSerialized]
	public Renderer renderer;

	[NonSerialized]
	public Renderer shadowRenderer;

	private void Awake()
	{
		renderer = GetComponent<Renderer>();
		shadowRenderer = shadow?.GetComponent<Renderer>();
		shadow.transform.parent = base.transform.parent;
	}

	public void Update()
	{
		bool flag = room.ApplyPerspective(base.transform, position, scale);
		base.transform.position -= spritePivot * base.transform.localScale.y;
		renderer.enabled = flag;
		if ((bool)shadow)
		{
			shadowRenderer.enabled = flag && renderShadow;
			if (flag)
			{
				room.ApplyPerspective(shadow.transform, position.WithY(0f), scale);
				shadow.color = shadowColor.WithAlpha(shadowColor.a * sprite.color.a);
				shadow.transform.localEulerAngles = Vector3.zero;
			}
		}
		sprite.SortingOrder = ((position.z > 0f) ? (-10) : 10);
	}

	private void OnEnable()
	{
		Toggle(on: true);
	}

	private void OnDisable()
	{
		Toggle(on: false);
	}

	private void Toggle(bool on)
	{
		if ((bool)shadow)
		{
			shadowRenderer.enabled = on;
		}
	}

	private void OnDestroy()
	{
		if ((bool)shadow)
		{
			UnityEngine.Object.Destroy(shadow.gameObject);
		}
	}
}
