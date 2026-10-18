using UnityEngine;

public class Smoke : Entity
{
	private new void Awake()
	{
		base.Awake();
		base.animator.deltaTimeStyle = tk2dSpriteAnimator.DeltaTimeStyle.AlwaysOneAlways;
		isTintable = false;
		RDUtils.Rotate2D(base.transform, Random.value * 360f);
		base.animator.AnimationCompleted = OnAnimEnd;
	}

	private void OnAnimEnd(tk2dSpriteAnimator sprite, tk2dSpriteAnimationClip clip)
	{
		Object.Destroy(base.transform.parent.gameObject);
	}

	public static void Add(float x, float y, Vector2 scale, int sortingOrder = -8)
	{
		scrConductor instance = scrConductor.instance;
		if (!(instance != null) || !instance.isScrubbing)
		{
			scrGameManager instance2 = scrGameManager.instance;
			if (!(instance2 == null))
			{
				GameObject obj = instance2.smoke.Instantiate("Smoke", makeContainer: true);
				Smoke componentInChildren = obj.GetComponentInChildren<Smoke>();
				Transform obj2 = obj.transform;
				obj2.localScale = scale;
				obj2.position = new Vector2(x, y);
				componentInChildren.sprite.Rotate2D(Random.value * 360f);
				componentInChildren.sprite.SortingOrder = sortingOrder;
			}
		}
	}

	public static void AddMany(float x, float y, int count, Vector2 scale, int sortingOrder = -8)
	{
		for (int i = 0; i < count; i++)
		{
			Add(x, y, scale, sortingOrder);
		}
	}
}
