#if !UNITY_WEBGL
using DG.Tweening.Core;
using DG.Tweening.Core.Enums;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace DG.Tweening
{
	public static class Perspective2DSpriteExtensions
	{
		public static TweenerCore<Vector3, Vector3, VectorOptions> DOScale(this Perspective2DSprite target, Vector3 endValue, float duration)
		{
			TweenerCore<Vector3, Vector3, VectorOptions> tweenerCore = DOTween.To(() => target.scale, (Vector3 x) =>
			{
				target.scale = x;
			}, endValue, duration);
			tweenerCore.SetTarget(target);
			return tweenerCore;
		}

		public static TweenerCore<Vector3, Vector3, VectorOptions> DOScale(this Perspective2DSprite target, float endValue, float duration)
		{
			TweenerCore<Vector3, Vector3, VectorOptions> obj = DOTween.To(endValue: new Vector3(endValue, endValue, endValue), getter: () => target.scale, setter: (Vector3 x) =>
			{
				target.scale = x;
			}, duration: duration);
			obj.SetTarget(target);
			return obj;
		}

		public static TweenerCore<Vector3, Vector3, VectorOptions> DOMove(this Perspective2DSprite target, Vector3 endValue, float duration, bool snapping = false)
		{
			TweenerCore<Vector3, Vector3, VectorOptions> tweenerCore = DOTween.To(() => target.position, (Vector3 x) =>
			{
				target.position = x;
			}, endValue, duration);
			tweenerCore.SetOptions(snapping).SetTarget(target);
			return tweenerCore;
		}

		public static Sequence DOJump(this Perspective2DSprite target, Vector3 endValue, float jumpPower, int numJumps, float duration, bool snapping = false)
		{
			if (numJumps < 1)
			{
				numJumps = 1;
			}
			float startPosY = target.position.y;
			float offsetY = -1f;
			bool offsetYSet = false;
			Sequence s = DOTween.Sequence();
			Tween yTween = DOTween.To(() => target.position, (Vector3 x) =>
			{
				target.position = x;
			}, new Vector3(0f, jumpPower, 0f), duration / (float)(numJumps * 2)).SetOptions(AxisConstraint.Y, snapping).SetEase(Ease.OutQuad)
				.SetRelative()
				.SetLoops(numJumps * 2, LoopType.Yoyo)
				.OnStart(() =>
				{
					startPosY = target.position.y;
				});
			s.Append(DOTween.To(() => target.position, (Vector3 x) =>
			{
				target.position = x;
			}, new Vector3(endValue.x, 0f, 0f), duration).SetOptions(AxisConstraint.X, snapping).SetEase(Ease.Linear)).Join(DOTween.To(() => target.position, (Vector3 x) =>
			{
				target.position = x;
			}, new Vector3(0f, 0f, endValue.z), duration).SetOptions(AxisConstraint.Z, snapping).SetEase(Ease.Linear)).Join(yTween)
				.SetTarget(target)
				.SetEase(DOTween.defaultEaseType);
			yTween.OnUpdate(() =>
			{
				if (!offsetYSet)
				{
					offsetYSet = true;
					offsetY = (s.isRelative ? endValue.y : (endValue.y - startPosY));
				}
				Vector3 position = target.position;
				position.y += DOVirtual.EasedValue(0f, offsetY, yTween.ElapsedPercentage(), Ease.OutQuad);
				target.position = position;
			});
			return s;
		}

		public static Tweener DOShakePosition(this Perspective2DSprite target, float duration, float strength = 1f, int vibrato = 10, float randomness = 90f, bool snapping = false, bool fadeOut = true)
		{
			if (duration <= 0f)
			{
				if (Debugger.logPriority > 0)
				{
					Debug.LogWarning("DOShakePosition: duration can't be 0, returning NULL without creating a tween");
				}
				return null;
			}
			return DOTween.Shake(() => target.position, (Vector3 x) =>
			{
				target.position = x;
			}, duration, strength, vibrato, randomness, ignoreZAxis: false, fadeOut).SetTarget(target).SetSpecialStartupMode(SpecialStartupMode.SetShake)
				.SetOptions(snapping);
		}
	}
}

#endif
