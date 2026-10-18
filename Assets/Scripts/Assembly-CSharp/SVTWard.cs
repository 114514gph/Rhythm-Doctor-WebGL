using DG.Tweening;
using UnityEngine;

public class SVTWard : RDEnvironment
{
	public tk2dSprite[] tk2dSprites;

	public tk2dSprite blueLights;

	public tk2dSprite vendingMachineLights;

	public AnimationCurve blueLightsCurve;

	public AnimationCurve vendingMachineLightsCurve;

	public float blueLightsDuration;

	public float vendingMachineLightsDuration;

	public WallClock clock;

	public tk2dSprite[] lights;

	public Color baseLightColor;

	public Color halloweenLightColor;

	public tk2dSpriteAnimator windowMan;

	public GameObject[] halloweenObjects;

	private bool nightMode;

	private bool _halloweenMode;

	public bool halloweenMode
	{
		get
		{
			return _halloweenMode;
		}
		set
		{
			bool flag = scnLevelSelect.instance != null;
			GameObject[] array = halloweenObjects;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value & flag);
			}
			Color color = ((nightMode & value) ? halloweenLightColor : baseLightColor);
			tk2dSprite[] array2 = lights;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].color = color;
			}
			_halloweenMode = value;
		}
	}

	public void SetNightMode(bool night)
	{
		Color white = Color.white;
		Color color = new Color(0.392f, 0.392f, 0.51f, 1f);
		Color color2 = (night ? color : white);
		tk2dSprite[] array = tk2dSprites;
		foreach (tk2dSprite tk2dSprite2 in array)
		{
			if (tk2dSprite2 != null)
			{
				tk2dSprite2.color = color2;
			}
		}
		clock.Tint(color2);
		Color color3 = ((night && halloweenMode) ? halloweenLightColor : baseLightColor);
		array = lights;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].color = color3;
		}
		nightMode = night;
	}

	protected override void Start()
	{
		base.Start();
		AnimateSpriteAlpha(blueLights, blueLightsDuration, blueLightsCurve);
		AnimateSpriteAlpha(vendingMachineLights, vendingMachineLightsDuration, vendingMachineLightsCurve);
	}

	public void AnimateSpriteAlpha(tk2dSprite sprite, float duration, AnimationCurve curve)
	{
		Color endValue = new Color(1f, 1f, 1f, 0f);
		sprite.DOColor(endValue, duration).SetEase(curve).SetLoops(-1, LoopType.Restart);
	}
}
