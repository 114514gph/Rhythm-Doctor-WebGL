using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace DG.Tweening
{
	public static class ShortcutExtensionsTk2d
	{
		public static TweenerCore<Vector3, Vector3, VectorOptions> DOScale(this tk2dBaseSprite target, Vector3 endValue, float duration)
		{
			TweenerCore<Vector3, Vector3, VectorOptions> tweenerCore = DOTween.To(() => target.scale, (Vector3 x) =>
			{
				target.scale = x;
			}, endValue, duration);
			tweenerCore.SetTarget(target);
			return tweenerCore;
		}

		public static TweenerCore<Vector3, Vector3, VectorOptions> DOScaleX(this tk2dBaseSprite target, float endValue, float duration)
		{
			TweenerCore<Vector3, Vector3, VectorOptions> tweenerCore = DOTween.To(() => target.scale, (Vector3 x) =>
			{
				target.scale = x;
			}, new Vector3(endValue, 0f, 0f), duration);
			tweenerCore.SetOptions(AxisConstraint.X).SetTarget(target);
			return tweenerCore;
		}

		public static TweenerCore<Vector3, Vector3, VectorOptions> DOScaleY(this tk2dBaseSprite target, float endValue, float duration)
		{
			TweenerCore<Vector3, Vector3, VectorOptions> tweenerCore = DOTween.To(() => target.scale, (Vector3 x) =>
			{
				target.scale = x;
			}, new Vector3(0f, endValue, 0f), duration);
			tweenerCore.SetOptions(AxisConstraint.Y).SetTarget(target);
			return tweenerCore;
		}

		public static TweenerCore<Vector3, Vector3, VectorOptions> DOScaleZ(this tk2dBaseSprite target, float endValue, float duration)
		{
			TweenerCore<Vector3, Vector3, VectorOptions> tweenerCore = DOTween.To(() => target.scale, (Vector3 x) =>
			{
				target.scale = x;
			}, new Vector3(0f, 0f, endValue), duration);
			tweenerCore.SetOptions(AxisConstraint.Z).SetTarget(target);
			return tweenerCore;
		}

		public static TweenerCore<Color, Color, ColorOptions> DOColor(this tk2dBaseSprite target, Color endValue, float duration)
		{
			TweenerCore<Color, Color, ColorOptions> tweenerCore = DOTween.To(() => target.color, (Color x) =>
			{
				target.color = x;
			}, endValue, duration);
			tweenerCore.SetTarget(target);
			return tweenerCore;
		}

		public static TweenerCore<Color, Color, ColorOptions> DOFade(this tk2dBaseSprite target, float endValue, float duration)
		{
			TweenerCore<Color, Color, ColorOptions> tweenerCore = DOTween.ToAlpha(() => target.color, (Color x) =>
			{
				target.color = x;
			}, endValue, duration);
			tweenerCore.SetTarget(target);
			return tweenerCore;
		}

		public static Sequence DOGradientColor(this tk2dBaseSprite target, Gradient gradient, float duration)
		{
			Sequence sequence = DOTween.Sequence();
			GradientColorKey[] colorKeys = gradient.colorKeys;
			int num = colorKeys.Length;
			for (int i = 0; i < num; i++)
			{
				GradientColorKey gradientColorKey = colorKeys[i];
				if (i == 0 && gradientColorKey.time <= 0f)
				{
					target.color = gradientColorKey.color;
					continue;
				}
				float duration2 = ((i == num - 1) ? (duration - sequence.Duration(includeLoops: false)) : (duration * ((i == 0) ? gradientColorKey.time : (gradientColorKey.time - colorKeys[i - 1].time))));
				sequence.Append(target.DOColor(gradientColorKey.color, duration2).SetEase(Ease.Linear));
			}
			sequence.SetTarget(target);
			return sequence;
		}

		public static TweenerCore<Vector2, Vector2, VectorOptions> DOScaleDimensions(this tk2dSlicedSprite target, Vector2 endValue, float duration)
		{
			TweenerCore<Vector2, Vector2, VectorOptions> tweenerCore = DOTween.To(() => target.dimensions, (Vector2 x) =>
			{
				target.dimensions = x;
			}, endValue, duration);
			tweenerCore.SetTarget(target);
			return tweenerCore;
		}

		public static TweenerCore<Vector2, Vector2, VectorOptions> DOScaleDimensionsX(this tk2dSlicedSprite target, float endValue, float duration)
		{
			TweenerCore<Vector2, Vector2, VectorOptions> tweenerCore = DOTween.To(() => target.dimensions, (Vector2 x) =>
			{
				target.dimensions = x;
			}, new Vector2(endValue, 0f), duration);
			tweenerCore.SetOptions(AxisConstraint.X).SetTarget(target);
			return tweenerCore;
		}

		public static TweenerCore<Vector2, Vector2, VectorOptions> DOScaleDimensionsY(this tk2dSlicedSprite target, float endValue, float duration)
		{
			TweenerCore<Vector2, Vector2, VectorOptions> tweenerCore = DOTween.To(() => target.dimensions, (Vector2 x) =>
			{
				target.dimensions = x;
			}, new Vector2(0f, endValue), duration);
			tweenerCore.SetOptions(AxisConstraint.Y).SetTarget(target);
			return tweenerCore;
		}

		public static TweenerCore<Vector3, Vector3, VectorOptions> DOScale(this tk2dTextMesh target, Vector3 endValue, float duration)
		{
			TweenerCore<Vector3, Vector3, VectorOptions> tweenerCore = DOTween.To(() => target.scale, (Vector3 x) =>
			{
				target.scale = x;
			}, endValue, duration);
			tweenerCore.SetTarget(target);
			return tweenerCore;
		}

		public static TweenerCore<Vector3, Vector3, VectorOptions> DOScaleX(this tk2dTextMesh target, float endValue, float duration)
		{
			TweenerCore<Vector3, Vector3, VectorOptions> tweenerCore = DOTween.To(() => target.scale, (Vector3 x) =>
			{
				target.scale = x;
			}, new Vector3(endValue, 0f, 0f), duration);
			tweenerCore.SetOptions(AxisConstraint.X).SetTarget(target);
			return tweenerCore;
		}

		public static TweenerCore<Vector3, Vector3, VectorOptions> DOScaleY(this tk2dTextMesh target, float endValue, float duration)
		{
			TweenerCore<Vector3, Vector3, VectorOptions> tweenerCore = DOTween.To(() => target.scale, (Vector3 x) =>
			{
				target.scale = x;
			}, new Vector3(0f, endValue, 0f), duration);
			tweenerCore.SetOptions(AxisConstraint.Y).SetTarget(target);
			return tweenerCore;
		}

		public static TweenerCore<Vector3, Vector3, VectorOptions> DOScaleZ(this tk2dTextMesh target, float endValue, float duration)
		{
			TweenerCore<Vector3, Vector3, VectorOptions> tweenerCore = DOTween.To(() => target.scale, (Vector3 x) =>
			{
				target.scale = x;
			}, new Vector3(0f, 0f, endValue), duration);
			tweenerCore.SetOptions(AxisConstraint.Z).SetTarget(target);
			return tweenerCore;
		}

		public static TweenerCore<Color, Color, ColorOptions> DOColor(this tk2dTextMesh target, Color endValue, float duration)
		{
			TweenerCore<Color, Color, ColorOptions> tweenerCore = DOTween.To(() => target.color, (Color x) =>
			{
				target.color = x;
			}, endValue, duration);
			tweenerCore.SetTarget(target);
			return tweenerCore;
		}

		public static TweenerCore<Color, Color, ColorOptions> DOFade(this tk2dTextMesh target, float endValue, float duration)
		{
			TweenerCore<Color, Color, ColorOptions> tweenerCore = DOTween.ToAlpha(() => target.color, (Color x) =>
			{
				target.color = x;
			}, endValue, duration);
			tweenerCore.SetTarget(target);
			return tweenerCore;
		}

		public static Sequence DOGradientColor(this tk2dTextMesh target, Gradient gradient, float duration)
		{
			Sequence sequence = DOTween.Sequence();
			GradientColorKey[] colorKeys = gradient.colorKeys;
			int num = colorKeys.Length;
			for (int i = 0; i < num; i++)
			{
				GradientColorKey gradientColorKey = colorKeys[i];
				if (i == 0 && gradientColorKey.time <= 0f)
				{
					target.color = gradientColorKey.color;
					continue;
				}
				float duration2 = ((i == num - 1) ? (duration - sequence.Duration(includeLoops: false)) : (duration * ((i == 0) ? gradientColorKey.time : (gradientColorKey.time - colorKeys[i - 1].time))));
				sequence.Append(target.DOColor(gradientColorKey.color, duration2).SetEase(Ease.Linear));
			}
			sequence.SetTarget(target);
			return sequence;
		}

		public static TweenerCore<string, string, StringOptions> DOText(this tk2dTextMesh target, string endValue, float duration, bool richTextEnabled = true, ScrambleMode scrambleMode = ScrambleMode.None, string scrambleChars = null)
		{
			TweenerCore<string, string, StringOptions> tweenerCore = DOTween.To(() => target.text, (string x) =>
			{
				target.text = x;
			}, endValue, duration);
			tweenerCore.SetOptions(richTextEnabled, scrambleMode, scrambleChars).SetTarget(target);
			return tweenerCore;
		}
	}
}
