using System;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Perspective2DSprite))]
public class FallingBaseball : MonoBehaviour
{
	public float gravity;

	public Perspective2DSprite perspective2DSprite;

	[Range(0f, 1f)]
	public float bounciness;

	private Vector3 velocity;

	private float angularVelocity;

	private bool firstBounce;

	private float origShadowAlpha;

	public Action onDestroy;

	private void Awake()
	{
		angularVelocity = UnityEngine.Random.Range(0f, 360f);
		origShadowAlpha = perspective2DSprite.shadowColor.a;
	}

	private void Update()
	{
		velocity += Vector3.down * gravity * Time.deltaTime;
		perspective2DSprite.position += velocity * Time.deltaTime * 60f;
		if (perspective2DSprite.position.y <= 0f && velocity.y < 0f)
		{
			if (!firstBounce)
			{
				float num = 0.25f;
				float f = UnityEngine.Random.Range(0f, 360f) * (MathF.PI / 180f);
				Vector3 vector = new Vector3(Mathf.Sin(f), 0f, Mathf.Cos(f)) * num;
				velocity += vector;
				firstBounce = true;
				perspective2DSprite.sprite.DOFade(0f, 0.5f).SetEase(Ease.InSine).SetDelay(1f)
					.OnComplete(() =>
					{
						if (base.gameObject != null)
						{
							UnityEngine.Object.Destroy(base.gameObject);
						}
					});
			}
			float num2 = 1f - bounciness;
			float y = 0f - (velocity.y - velocity.y * num2);
			velocity = velocity.WithY(y);
			perspective2DSprite.position.y = y;
			angularVelocity *= num2;
		}
		base.transform.eulerAngles += Vector3.back * Mathf.Sign(velocity.x) * angularVelocity * Time.deltaTime;
		perspective2DSprite.shadowColor = perspective2DSprite.shadowColor.WithAlpha(Mathf.InverseLerp(100f, 0f, perspective2DSprite.position.y) * origShadowAlpha);
	}

	private void OnDestroy()
	{
		onDestroy();
	}
}
