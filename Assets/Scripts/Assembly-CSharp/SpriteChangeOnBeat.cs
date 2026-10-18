using UnityEngine;
using UnityEngine.UI;

public class SpriteChangeOnBeat : RDBase
{
	public Image spriteRenderer;

	private Sprite originalSprite;

	public Sprite onBeatSprite;

	private void Awake()
	{
		originalSprite = spriteRenderer.sprite;
	}

	private void LateUpdate()
	{
		spriteRenderer.sprite = (((double)(Time.timeSinceLevelLoad % 1f) > 0.75) ? onBeatSprite : originalSprite);
	}
}
