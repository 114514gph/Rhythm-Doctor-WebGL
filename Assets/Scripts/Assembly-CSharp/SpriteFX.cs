using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public class SpriteFX : RDBase
{
	public static Tweener Flash(tk2dSprite sprite, float duration = 4f, float alpha = 1f)
	{
		sprite.flash = alpha;
		TweenerCore<float, float, FloatOptions> item = DOTween.To(() => sprite.flash, (float x) =>
		{
			sprite.flash = x;
		}, 0f, duration);
		sprite.KillAllTweens();
		sprite.tweens.Add(item);
		return null;
	}

	public static Tweener ReverseFlash(tk2dSprite sprite, float duration = 1f, float alpha = 1f)
	{
		TweenerCore<float, float, FloatOptions> tweenerCore = DOTween.To(() => sprite.flash, (float x) =>
		{
			sprite.flash = x;
		}, 1f, duration);
		sprite.KillAllTweens();
		sprite.tweens.Add(tweenerCore);
		return tweenerCore;
	}

	public static void Tint(tk2dSprite sprite, Color color)
	{
		sprite.KillAllTweens();
		sprite.overlayColor = color;
	}
}
