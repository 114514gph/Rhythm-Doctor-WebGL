using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnBar, 0, RoomsUsage.NotUsed, true, true, true, true, -10, false)]
	public class LevelEvent_Move : LevelEvent_Base, IDurationHaver, ICompatibleDecorationTypeChecker
	{
		public LevelEvent_Sprite sprite;

		[JsonProperty("position", "", null, "", false, true, null, null)]
		[ExpPositionPicker(true, false, false)]
		public FloatExpression2? decorationPosition { get; set; } = new FloatExpression2(50f, 50f);

		[JsonProperty("", "", null, "", false, true, null, null)]
		[Off]
		public FloatExpression2? scale { get; set; } = new FloatExpression2(1f, 1f);

		[JsonProperty("", "", null, "", false, true, null, null)]
		[InputField("°", InputField.LineType.SingleLine, 40, 14, false, false)]
		[Off]
		public FloatExpression? angle { get; set; } = new FloatExpression(0f);

		[JsonProperty("", "", null, "", false, true, "SupportsPivot", null)]
		[Off]
		public Float2? pivot { get; set; } = new Float2(50f, 50f);

		[JsonProperty("", "", null, "", false, true, null, null)]
		[InputField("beats", InputField.LineType.SingleLine, 40, 14, false, false)]
		public float duration { get; set; } = 1f;

		[JsonProperty("", "", null, "", false, true, null, null)]
		public Ease ease { get; set; } = Ease.Linear;

		public bool isCompatibleDecorationType
		{
			get
			{
				LevelEvent_MakeDecorationBase levelEvent_MakeDecorationBase = base.ownerDecoration;
				return levelEvent_MakeDecorationBase is LevelEvent_Sprite || levelEvent_MakeDecorationBase is LevelEvent_Text;
			}
		}

		public bool SupportsPivot()
		{
			return !(base.ownerDecoration is LevelEvent_Text);
		}

		public override void Decode(Dictionary<string, object> dict)
		{
			base.Decode(dict);
			Float2? @float = pivot;
			if (!@float.HasValue)
			{
				return;
			}
			Float2 valueOrDefault = @float.GetValueOrDefault();
			if (sprite != null && RDLevelData.current.settings.version < 65 && sprite.character == Character.Beans)
			{
				if (valueOrDefault.xUsed)
				{
					valueOrDefault.x = (valueOrDefault.x - 50f) * 0.84210527f + 50f;
				}
				if (valueOrDefault.yUsed)
				{
					valueOrDefault.y = (valueOrDefault.y - 50f) * 0.8378378f + 50f;
				}
				pivot = valueOrDefault;
			}
		}

		public override void Run()
		{
			RunOnBeat(() =>
			{
				if (isCompatibleDecorationType)
				{
					if (base.ownerDecoration is LevelEvent_Sprite)
					{
						if (!LevelEvent_Base.level.sprites.ContainsKey(target))
						{
							return;
						}
					}
					else if (base.ownerDecoration is LevelEvent_Text && !base.vfx.allDecorationLyrics.ContainsKey(target))
					{
						return;
					}
					float seconds = base.conductor.DurationBeatsToTime(base.beat - 1f, duration);
					FloatExpression2? floatExpression = decorationPosition;
					if (floatExpression.HasValue)
					{
						(float? x, float? y) tuple = floatExpression.GetValueOrDefault().Unbox(LevelEvent_Base.level);
						float? num;
						(num, _) = tuple;
						if (num.HasValue)
						{
							float valueOrDefault = num.GetValueOrDefault();
							SetPositionTweenX(valueOrDefault, seconds);
						}
						num = tuple.y;
						if (num.HasValue)
						{
							float valueOrDefault2 = num.GetValueOrDefault();
							SetPositionTweenY(valueOrDefault2, seconds);
						}
					}
					floatExpression = scale;
					if (floatExpression.HasValue)
					{
						(float? x, float? y) tuple3 = floatExpression.GetValueOrDefault().Unbox(LevelEvent_Base.level);
						float? num;
						(num, _) = tuple3;
						if (num.HasValue)
						{
							float valueOrDefault3 = num.GetValueOrDefault();
							SetScaleTweenX(valueOrDefault3, seconds);
						}
						num = tuple3.y;
						if (num.HasValue)
						{
							float valueOrDefault4 = num.GetValueOrDefault();
							SetScaleTweenY(valueOrDefault4, seconds);
						}
					}
					FloatExpression? floatExpression2 = angle;
					if (floatExpression2.HasValue)
					{
						float? num = floatExpression2.GetValueOrDefault().Unbox(LevelEvent_Base.level);
						if (num.HasValue)
						{
							float valueOrDefault5 = num.GetValueOrDefault();
							SetAngleTween(valueOrDefault5, seconds);
						}
					}
					Float2? @float = pivot;
					if (@float.HasValue)
					{
						Float2 valueOrDefault6 = @float.GetValueOrDefault();
						if (valueOrDefault6.xUsed)
						{
							SetPivotTweenX(valueOrDefault6.x, seconds);
						}
						if (valueOrDefault6.yUsed)
						{
							SetPivotTweenY(valueOrDefault6.y, seconds);
						}
					}
				}
			});
		}

		private void SetPositionTweenX(float value, float seconds)
		{
			if (base.ownerDecoration is LevelEvent_Sprite)
			{
				CustomSprite customSprite = LevelEvent_Base.level.sprites[target];
				Transform transform = customSprite.transform;
				customSprite.positionTweenX?.Kill();
				float endValue = base.vfx.RDWidth * value / 100f;
				customSprite.positionTweenX = transform.DOLocalMoveX(endValue, seconds).SetEase(ease).Done();
			}
			else if (base.ownerDecoration is LevelEvent_Text)
			{
				LyricsGame targetLyricsGame = base.vfx.allDecorationLyrics[target];
				targetLyricsGame.positionTweenX?.Kill();
				float endValue2 = Mathf.Floor(base.vfx.RDWidth * (value / 100f - 0.5f)) - 0.5f;
				targetLyricsGame.positionTweenX = DOTween.To(() => targetLyricsGame.anchoredPosition.x, (float x) =>
				{
					targetLyricsGame.anchoredPosition = new Vector2(x, targetLyricsGame.anchoredPosition.y);
				}, endValue2, seconds).SetEase(ease).Done();
			}
		}

		private void SetPositionTweenY(float value, float seconds)
		{
			if (base.ownerDecoration is LevelEvent_Sprite)
			{
				CustomSprite customSprite = LevelEvent_Base.level.sprites[target];
				Transform transform = customSprite.transform;
				customSprite.positionTweenY?.Kill();
				float endValue = base.vfx.RDHeight * value / 100f;
				customSprite.positionTweenY = transform.DOLocalMoveY(endValue, seconds).SetEase(ease).Done();
			}
			else if (base.ownerDecoration is LevelEvent_Text)
			{
				LyricsGame targetLyricsGame = base.vfx.allDecorationLyrics[target];
				targetLyricsGame.positionTweenY?.Kill();
				float endValue2 = Mathf.Floor(base.vfx.RDHeight * (value / 100f - 0.5f)) - 0.5f;
				targetLyricsGame.positionTweenY = DOTween.To(() => targetLyricsGame.anchoredPosition.y, (float y) =>
				{
					targetLyricsGame.anchoredPosition = new Vector2(targetLyricsGame.anchoredPosition.x, y);
				}, endValue2, seconds).SetEase(ease).Done();
			}
		}

		private void SetScaleTweenX(float value, float seconds)
		{
			if (base.ownerDecoration is LevelEvent_Sprite)
			{
				CustomSprite customSprite = LevelEvent_Base.level.sprites[target];
				Transform transform = customSprite.transform;
				customSprite.scaleTweenX?.Kill();
				customSprite.scaleTweenX = transform.DOScaleX(value, seconds).SetEase(ease).Done();
			}
			else if (base.ownerDecoration is LevelEvent_Text)
			{
				LyricsGame targetLyricsGame = base.vfx.allDecorationLyrics[target];
				targetLyricsGame.scaleTweenX?.Kill();
				targetLyricsGame.scaleTweenX = DOTween.To(() => targetLyricsGame.scale.x, (float x) =>
				{
					targetLyricsGame.scale = new Vector2(x, targetLyricsGame.scale.y);
				}, value, seconds).SetEase(ease).Done();
			}
		}

		private void SetScaleTweenY(float value, float seconds)
		{
			if (base.ownerDecoration is LevelEvent_Sprite)
			{
				CustomSprite customSprite = LevelEvent_Base.level.sprites[target];
				Transform transform = customSprite.transform;
				customSprite.scaleTweenY?.Kill();
				customSprite.scaleTweenY = transform.DOScaleY(value, seconds).SetEase(ease).Done();
			}
			else if (base.ownerDecoration is LevelEvent_Text)
			{
				LyricsGame targetLyricsGame = base.vfx.allDecorationLyrics[target];
				targetLyricsGame.scaleTweenY?.Kill();
				targetLyricsGame.scaleTweenY = DOTween.To(() => targetLyricsGame.scale.y, (float y) =>
				{
					targetLyricsGame.scale = new Vector2(targetLyricsGame.scale.x, y);
				}, value, seconds).SetEase(ease).Done();
			}
		}

		private void SetAngleTween(float value, float seconds)
		{
			if (base.ownerDecoration is LevelEvent_Sprite)
			{
				LevelEvent_Base.level.sprites[target].Rotate(value, seconds, ease);
			}
			else if (base.ownerDecoration is LevelEvent_Text)
			{
				LyricsGame targetLyricsGame = base.vfx.allDecorationLyrics[target];
				targetLyricsGame.angleTween?.Kill();
				targetLyricsGame.angleTween = DOTween.To(() => targetLyricsGame.angle, (float angle) =>
				{
					targetLyricsGame.angle = angle;
				}, value, seconds).SetEase(ease).Done();
			}
		}

		private void SetPivotTweenX(float value, float seconds)
		{
			if (base.ownerDecoration is LevelEvent_Sprite)
			{
				CustomSprite ent = LevelEvent_Base.level.sprites[target];
				CustomAnimation customAnimation = ent.customAnimation;
				float endValue = value / 100f;
				ent.pivotTweenX?.Kill();
				ent.pivotTweenX = DOTween.To(() => customAnimation.pivotX, (float s) =>
				{
					customAnimation.pivotX = s;
				}, endValue, seconds).OnUpdate(() =>
				{
					ent.customAnimation.UpdateMesh();
				}).SetEase(ease)
					.Done();
			}
		}

		private void SetPivotTweenY(float value, float seconds)
		{
			if (base.ownerDecoration is LevelEvent_Sprite)
			{
				CustomSprite ent = LevelEvent_Base.level.sprites[target];
				CustomAnimation customAnimation = ent.customAnimation;
				float endValue = value / 100f;
				ent.pivotTweenY?.Kill();
				ent.pivotTweenY = DOTween.To(() => customAnimation.pivotY, (float s) =>
				{
					customAnimation.pivotY = s;
				}, endValue, seconds).OnUpdate(() =>
				{
					ent.customAnimation.UpdateMesh();
				}).SetEase(ease)
					.Done();
			}
		}

		public override string GetTooltipText()
		{
			List<string> list = new List<string>();
			if (((bool?)decorationPosition) ?? false)
			{
				list.Add(string.Format("{0} {1}", RDString.Get("editor.Move.decorationPosition"), decorationPosition));
			}
			if (((bool?)scale) ?? false)
			{
				list.Add(string.Format("{0} {1}", RDString.Get("editor.Move.scale"), scale));
			}
			if (((bool?)angle) ?? false)
			{
				list.Add(string.Format("{0} {1}", RDString.Get("editor.Move.angle"), angle));
			}
			if (((bool?)pivot) ?? false)
			{
				list.Add(string.Format("{0} {1}", RDString.Get("editor.Move.pivot"), pivot));
			}
			return string.Join('\n', list);
		}
	}
}
