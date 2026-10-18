using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class RDCoffeeShop : RDBase
{
	public tk2dSpriteAnimator door;

	public tk2dSpriteAnimator doorbell;

	public tk2dSpriteAnimator lights;

	public tk2dSpriteAnimator lights_night;

	public tk2dSpriteAnimator coffeeMan;

	public tk2dSprite[] fgSprites;

	public Transform backgroundContainer;

	public Transform foregroundContainer;

	public tk2dSprite[] cups_4;

	public tk2dSprite[] cups_8;

	public tk2dSprite[] cups_12;

	public tk2dSprite[] cups_20;

	public tk2dSprite[] customers;

	private const float customerAlpha = 0.8f;

	public tk2dSprite[] spritesToTint;

	public tk2dSprite[] daySprites;

	public tk2dSprite[] nightSprites;

	public GameObject countertop;

	public GameObject countertopSpecial;

	public GameObject countertopBackground;

	private bool night;

	private Tween colTween;

	private Tween alphaTween;

	private const float nightTint = 0.35f;

	public void OpenDoor(bool open = true, bool slow = false)
	{
		if (open)
		{
			doorbell.Play();
			door.Play(slow ? "CoffeeShop_Door_openSlow" : "CoffeeShop_Door_open");
		}
		else
		{
			doorbell.StopAndResetFrame();
			door.Play(slow ? "CoffeeShop_Door_closeSlow" : "CoffeeShop_Door_close");
		}
	}

	public void SlideCoffeeMan(bool inside = true)
	{
		TweenerCore<Vector3, Vector3, VectorOptions> t = coffeeMan.transform.DOLocalMoveX(inside ? (-2f) : (-50f), 0.5f).SetEase(inside ? Ease.OutCubic : Ease.InCubic);
		if (inside)
		{
			OpenDoor();
			coffeeMan.Play("Coffeeman_enter");
		}
		else
		{
			t.OnComplete(() =>
			{
				OpenDoor(open: false);
			});
		}
	}

	public void TransitionToNight()
	{
		night = true;
		tk2dSprite[] array = spritesToTint;
		foreach (tk2dSprite target in array)
		{
			float num = 0.65f;
			target.DOColor(new Color(num, num, num, 1f), 4.5f).SetEase(Ease.Linear);
		}
		array = daySprites;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].DOFade(0f, 5.5f).SetEase(Ease.Linear);
		}
		array = nightSprites;
		foreach (tk2dSprite obj in array)
		{
			obj.gameObject.SetActive(value: true);
			obj.color = Color.white.WithAlpha(0f);
			obj.DOFade(1f, 4.5f).SetEase(Ease.Linear);
		}
	}

	public void SetNight(bool night)
	{
		this.night = night;
		float num = 0.65f;
		Color color = (night ? new Color(num, num, num, 1f) : Color.white);
		tk2dSprite[] array = spritesToTint;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].color = color;
		}
		array = daySprites;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(!night);
		}
		array = nightSprites;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(night);
		}
		lights_night.gameObject.SetActive(night);
	}

	public void SetLights(int lights)
	{
		if (night)
		{
			int frame = ((lights != 0) ? (lights / 2) : 0);
			lights_night.gameObject.SetActive(value: true);
			lights_night.SetFrame(frame);
		}
		else
		{
			int frame2 = ((lights == 0) ? 7 : (lights - 1));
			this.lights.enabled = false;
			this.lights.SetFrame(frame2);
		}
	}

	public void SetLightColor(Color color, float alpha, float dur = 0f)
	{
		tk2dBaseSprite sprite = (night ? lights_night : lights).Sprite;
		colTween?.Kill();
		colTween = sprite.DOColor(color.WithAlpha(alpha), dur).Done();
	}

	public void OnlyBackground()
	{
		foregroundContainer.gameObject.SetActive(value: false);
	}

	public void OnlyForeground()
	{
		backgroundContainer.gameObject.SetActive(value: false);
	}

	public void ShowOrHideCustomers(int from, int to, bool hide, float dur)
	{
		float alpha = (hide ? 0f : 0.8f);
		for (int i = from - 1; i <= to - 1; i++)
		{
			SetCustomerAlpha(i, alpha, dur);
		}
	}

	public void RandomizeCustomers(int from, int to, float dur)
	{
		for (int i = from - 1; i <= to - 1; i++)
		{
			float alpha = 0.8f * (float)Random.Range(0, 2);
			SetCustomerAlpha(i, alpha, dur);
		}
	}

	private void SetCustomerAlpha(int index, float alpha, float dur)
	{
		bool flag = alpha == 0f;
		tk2dSprite cst = customers[index];
		cst.DOComplete();
		if (!cst.gameObject.activeSelf && !flag)
		{
			cst.gameObject.SetActive(value: true);
			cst.color = cst.color.WithAlpha(0f);
		}
		TweenerCore<Color, Color, ColorOptions> t = (TweenerCore<Color, Color, ColorOptions>)cst.DOFade(alpha, dur).SetEase(Ease.Linear);
		if (flag)
		{
			t.OnComplete(() =>
			{
				cst.gameObject.SetActive(value: false);
			});
		}
		t.Done();
	}
}
