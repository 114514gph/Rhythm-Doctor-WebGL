using UnityEngine;

public class NoteTileAnimation : RDBase
{
	private const string MainTexHandle = "_MainTex";

	public Material material;

	public Vector2 speed;

	public bool unscaledDeltaTime;

	private Vector2 mOffset;

	private void Awake()
	{
		material = new Material(material);
	}

	private void Update()
	{
		float num = (unscaledDeltaTime ? Time.unscaledDeltaTime : Time.deltaTime);
		mOffset = new Vector2((mOffset.x + num * speed.x) % 1f, (mOffset.y + num * speed.y) % 1f);
		material.SetTextureOffset("_MainTex", mOffset);
	}
}
