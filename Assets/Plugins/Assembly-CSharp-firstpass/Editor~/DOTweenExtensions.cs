#if UNITY_EDITOR || !UNITY_WEBGL
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

public static class DOTweenExtensions
{
	public static TweenerCore<float, float, FloatOptions> DOFadeStop(this AudioSource target, float duration)
	{
		return target.DOFade(0f, duration).OnComplete(() =>
		{
			target.Stop();
		});
	}
}

#endif
