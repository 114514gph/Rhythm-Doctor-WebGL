using System;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnBar, 0, RoomsUsage.NotUsed, true, true, true, false, 0, false)]
	public class LevelEvent_MoveRow : LevelEvent_Base, IDurationHaver
	{
		[JsonProperty("target", "", null, "", false, true, null, null)]
		[ToggleGroup(false, null, null, null, true, 14)]
		public MoveRowTarget rowTarget { get; set; }

		[JsonProperty("", "", null, "", false, true, "EnableCustomPositionIf", null)]
		[ToggleGroup(false, null, null, null, true, 14)]
		public bool customPosition { get; set; } = true;

		[JsonProperty("", "", null, "", false, true, "EnableOffDescriptionIf", null)]
		[Description("editor.MoveRow.offDescription", 28, 16, TextAnchor.MiddleCenter)]
		public bool offDescription => false;

		[JsonProperty("", "", null, "", false, true, "EnableIfCustomPosition", null)]
		[ExpPositionPicker(true, false, false)]
		public FloatExpression2? rowPosition { get; set; } = new FloatExpression2(50f, 50f);

		[JsonProperty("", "", null, "", false, true, "EnableIfCustomPosition", null)]
		[Off]
		public FloatExpression2? scale { get; set; } = new FloatExpression2(1f, 1f);

		[JsonProperty("", "", null, "", false, true, "EnableIfCustomPosition", null)]
		[Off]
		[InputField("°", InputField.LineType.SingleLine, 40, 14, false, false)]
		public FloatExpression? angle { get; set; } = new FloatExpression(0f);

		[JsonProperty("", "", null, "", false, true, "EnablePivotIf", null)]
		[Off]
		[SliderPercent(false)]
		public float? pivot { get; set; } = 0f;

		[JsonProperty("", "", null, "", false, true, "EnableIfCustomPosition", null)]
		[InputField("beats", InputField.LineType.SingleLine, 40, 14, false, false)]
		public float duration { get; set; } = 1f;

		[JsonProperty("", "", null, "", false, true, "EnableEaseIf", null)]
		public Ease ease { get; set; } = Ease.Linear;

		[JsonProperty("", "", null, "", false, true, "EnableAccelerationIf", null)]
		[Off]
		[InputField("beats", InputField.LineType.SingleLine, 40, 14, false, false)]
		[FloatInfo(0f, float.PositiveInfinity)]
		public float? accelerationDuration { get; set; } = 0f;

		[JsonProperty("", "", null, "", false, true, "EnableDecelerationIf", null)]
		[Off]
		[InputField("beats", InputField.LineType.SingleLine, 40, 14, false, false)]
		[FloatInfo(0f, float.PositiveInfinity)]
		public float? decelerationDuration { get; set; } = 0f;

		public bool EnableCustomPositionIf()
		{
			return rowTarget == MoveRowTarget.WholeRow;
		}

		public bool EnablePivotIf()
		{
			if (EnableIfCustomPosition())
			{
				return rowTarget == MoveRowTarget.WholeRow;
			}
			return false;
		}

		public bool EnableIfCustomPosition()
		{
			if (!customPosition)
			{
				return !EnableCustomPositionIf();
			}
			return true;
		}

		public bool EnableEaseIf()
		{
			if (EnableIfCustomPosition())
			{
				return !accelerationDuration.HasValue;
			}
			return false;
		}

		public bool EnableOffDescriptionIf()
		{
			return !EnableIfCustomPosition();
		}

		public bool EnableAccelerationIf()
		{
			return EnableIfCustomPosition();
		}

		public bool EnableDecelerationIf()
		{
			if (EnableIfCustomPosition())
			{
				return accelerationDuration.HasValue;
			}
			return false;
		}

		public override void Decode(Dictionary<string, object> dict)
		{
			base.Decode(dict);
			if (RDLevelData.current.settings.version < 25)
			{
				customPosition = true;
			}
		}

		public override void Run()
		{
			RunOnBeat(() =>
			{
				RowEntity ent = LevelEvent_Base.level.rows[base.row].ent;
				if (!(ent == null))
				{
					if (rowTarget == MoveRowTarget.WholeRow)
					{
						if (!ent.customPosition && customPosition)
						{
							ent.positionTweenX.Kill();
							ent.positionTweenY.Kill();
						}
						ent.customPosition = customPosition;
						if (customPosition)
						{
							SetMovingTweens(ent);
						}
						base.vfx.rooms[base.room].RepositionRowsAndStrips(instant: false);
					}
					else
					{
						SetMovingTweens(ent);
					}
					MoveRowTarget moveRowTarget = rowTarget;
					if ((moveRowTarget == MoveRowTarget.WholeRow || moveRowTarget == MoveRowTarget.Character) && scnGame.instance.currentLevel.rowReflectionsJumping && ent.row.reflection != null && duration <= 0.001f)
					{
						ent.row.reflection.updateOriginY = true;
					}
				}
			});
		}

		private void SetMovingTweens(RowEntity ent)
		{
			float num = base.conductor.DurationBeatsToTime(base.beat - 1f, duration);
			EaseFunction easeFunction = null;
			float? num2 = accelerationDuration;
			if (num2.HasValue)
			{
				float valueOrDefault = num2.GetValueOrDefault();
				float num3 = base.conductor.DurationBeatsToTime(base.beat - 1f, valueOrDefault);
				float num4 = ((!decelerationDuration.HasValue) ? num3 : base.conductor.DurationBeatsToTime(base.beat - 1f, decelerationDuration.Value));
				if (num3 == 0f && num4 == 0f)
				{
					ease = Ease.Linear;
				}
				else
				{
					easeFunction = VelocityBasedEase(num3, (num4 == 0f) ? 0.001f : num4);
				}
			}
			Transform transform = rowTarget switch
			{
				MoveRowTarget.WholeRow => ent.transform, 
				MoveRowTarget.Character => ent.characterContainer, 
				MoveRowTarget.Heart => ent.heartContainer, 
				_ => throw new ArgumentOutOfRangeException(), 
			};
			(float?, float?)? tuple = rowPosition?.Unbox(LevelEvent_Base.level);
			if (tuple.HasValue)
			{
				(float?, float?) valueOrDefault2 = tuple.GetValueOrDefault();
				(num2, _) = valueOrDefault2;
				if (num2.HasValue)
				{
					num2.GetValueOrDefault();
					KillXTween(ent);
					float endValue = base.vfx.RDWidth * valueOrDefault2.Item1.Value / 100f;
					TweenerCore<Vector3, Vector3, VectorOptions> tweenerCore = transform.DOLocalMoveX(endValue, num);
					if (easeFunction != null)
					{
						tweenerCore.SetEase(easeFunction);
					}
					else
					{
						tweenerCore.SetEase(ease);
					}
					tweenerCore.Done();
					SetXTween(ent, tweenerCore);
				}
				num2 = valueOrDefault2.Item2;
				if (num2.HasValue)
				{
					float valueOrDefault3 = num2.GetValueOrDefault();
					KillYTween(ent);
					float endValue2 = base.vfx.RDHeight * valueOrDefault3 / 100f;
					TweenerCore<Vector3, Vector3, VectorOptions> tweenerCore2 = transform.DOLocalMoveY(endValue2, num);
					if (easeFunction != null)
					{
						tweenerCore2.SetEase(easeFunction);
					}
					else
					{
						tweenerCore2.SetEase(ease);
					}
					tweenerCore2.Done();
					SetYTween(ent, tweenerCore2);
				}
			}
			num2 = angle?.Unbox(LevelEvent_Base.level);
			if (num2.HasValue)
			{
				float valueOrDefault4 = num2.GetValueOrDefault();
				ent.Rotate(valueOrDefault4, num, ease, rowTarget);
			}
			tuple = scale?.Unbox(LevelEvent_Base.level);
			if (tuple.HasValue)
			{
				(float?, float?) valueOrDefault5 = tuple.GetValueOrDefault();
				if (rowTarget == MoveRowTarget.Character)
				{
					transform = ent.character.transform;
				}
				(num2, _) = valueOrDefault5;
				if (num2.HasValue)
				{
					float valueOrDefault6 = num2.GetValueOrDefault();
					KillScaleTweenX(ent);
					TweenerCore<Vector3, Vector3, VectorOptions> tweenerCore3 = transform.DOScaleX(valueOrDefault6, num);
					if (easeFunction != null)
					{
						tweenerCore3.SetEase(easeFunction);
					}
					else
					{
						tweenerCore3.SetEase(ease);
					}
					tweenerCore3.Done();
					SetScaleTweenX(ent, tweenerCore3);
				}
				num2 = valueOrDefault5.Item2;
				if (num2.HasValue)
				{
					float valueOrDefault7 = num2.GetValueOrDefault();
					KillScaleTweenY(ent);
					TweenerCore<Vector3, Vector3, VectorOptions> tweenerCore4 = transform.DOScaleY(valueOrDefault7, num);
					if (easeFunction != null)
					{
						tweenerCore4.SetEase(easeFunction);
					}
					else
					{
						tweenerCore4.SetEase(ease);
					}
					tweenerCore4.Done();
					SetScaleTweenY(ent, tweenerCore4);
				}
			}
			num2 = pivot;
			if (!num2.HasValue)
			{
				return;
			}
			float valueOrDefault8 = num2.GetValueOrDefault();
			if (rowTarget == MoveRowTarget.WholeRow)
			{
				ent.pivotTweenX?.Kill();
				ent.pivotTweenX = DOTween.To(() => ent.pivot, (float x) =>
				{
					ent.pivot = x;
				}, valueOrDefault8, num).SetEase(ease).Done();
			}
		}

		private void KillXTween(RowEntity ent)
		{
			((Tween)(rowTarget switch
			{
				MoveRowTarget.WholeRow => ent.positionTweenX, 
				MoveRowTarget.Character => ent.charPositionTweenX, 
				MoveRowTarget.Heart => ent.heartPositionTweenX, 
				_ => throw new ArgumentOutOfRangeException(), 
			}))?.Kill();
		}

		private void SetXTween(RowEntity ent, Tween tween)
		{
			switch (rowTarget)
			{
			case MoveRowTarget.WholeRow:
				ent.positionTweenX = tween;
				break;
			case MoveRowTarget.Character:
				ent.charPositionTweenX = tween;
				break;
			case MoveRowTarget.Heart:
				ent.heartPositionTweenX = tween;
				break;
			default:
				throw new ArgumentOutOfRangeException("rowTarget", rowTarget, null);
			}
		}

		private void KillYTween(RowEntity ent)
		{
			((Tween)(rowTarget switch
			{
				MoveRowTarget.WholeRow => ent.positionTweenY, 
				MoveRowTarget.Character => ent.charPositionTweenY, 
				MoveRowTarget.Heart => ent.heartPositionTweenY, 
				_ => throw new ArgumentOutOfRangeException(), 
			}))?.Kill();
		}

		private void SetYTween(RowEntity ent, Tween tween)
		{
			switch (rowTarget)
			{
			case MoveRowTarget.WholeRow:
				ent.positionTweenY = tween;
				break;
			case MoveRowTarget.Character:
				ent.charPositionTweenY = tween;
				break;
			case MoveRowTarget.Heart:
				ent.heartPositionTweenY = tween;
				break;
			default:
				throw new ArgumentOutOfRangeException("rowTarget", rowTarget, null);
			}
		}

		private void KillScaleTweenX(RowEntity ent)
		{
			((Tween)(rowTarget switch
			{
				MoveRowTarget.WholeRow => ent.scaleTweenX, 
				MoveRowTarget.Character => ent.charScaleTweenX, 
				MoveRowTarget.Heart => ent.heartScaleTweenX, 
				_ => throw new ArgumentOutOfRangeException(), 
			}))?.Kill();
		}

		private void SetScaleTweenX(RowEntity ent, Tween tween)
		{
			switch (rowTarget)
			{
			case MoveRowTarget.WholeRow:
			{
				Tween tween2 = (ent.scaleTweenX = tween);
				break;
			}
			case MoveRowTarget.Character:
			{
				Tween tween2 = (ent.charScaleTweenX = tween);
				break;
			}
			case MoveRowTarget.Heart:
			{
				Tween tween2 = (ent.heartScaleTweenX = tween);
				break;
			}
			default:
				throw new ArgumentOutOfRangeException("rowTarget", rowTarget, null);
			}
		}

		private void KillScaleTweenY(RowEntity ent)
		{
			((Tween)(rowTarget switch
			{
				MoveRowTarget.WholeRow => ent.scaleTweenY, 
				MoveRowTarget.Character => ent.charScaleTweenY, 
				MoveRowTarget.Heart => ent.heartScaleTweenY, 
				_ => throw new ArgumentOutOfRangeException(), 
			}))?.Kill();
		}

		private void SetScaleTweenY(RowEntity ent, Tween tween)
		{
			switch (rowTarget)
			{
			case MoveRowTarget.WholeRow:
			{
				Tween tween2 = (ent.scaleTweenY = tween);
				break;
			}
			case MoveRowTarget.Character:
			{
				Tween tween2 = (ent.charScaleTweenY = tween);
				break;
			}
			case MoveRowTarget.Heart:
			{
				Tween tween2 = (ent.heartScaleTweenY = tween);
				break;
			}
			default:
				throw new ArgumentOutOfRangeException("rowTarget", rowTarget, null);
			}
		}

		public static EaseFunction VelocityBasedEase(float accelerationDuration, float decelerationDuration)
		{
			return (float time, float duration, float overshoot, float period) =>
			{
				float num = Mathf.Clamp01(time / duration);
				float num2 = Mathf.Clamp01(accelerationDuration / duration);
				float num3 = Mathf.Clamp01(decelerationDuration / duration);
				float num4 = 1f - num2 - num3;
				float num5 = 0f;
				if (num4 >= 0f)
				{
					float num6 = 1f / (num4 + 0.5f * (num2 + num3));
					if (num2 > 0f && num < num2)
					{
						num5 = 0.5f * (num6 / num2) * num * num;
					}
					else if (num < num2 + num4)
					{
						num5 = 0.5f * num6 * num2 + num6 * (num - num2);
					}
					else if (num3 > 0f)
					{
						float num7 = num - (num2 + num4);
						num5 = 0.5f * num6 * num2 + num6 * num4 + num6 * num7 - 0.5f * (num6 / num3) * num7 * num7;
					}
					else
					{
						num5 = 0f;
					}
				}
				else
				{
					float num8 = num2 + num3;
					if (num8 <= 0f)
					{
						return num;
					}
					float num9 = num2 / num8;
					float num10 = num3 / num8;
					float num11 = 2f;
					if (num < num9)
					{
						num5 = 0.5f * (num11 / num9) * num * num;
					}
					else
					{
						float num12 = num - num9;
						num5 = 0.5f * num11 * num9 + num11 * num12 - 0.5f * (num11 / num10) * num12 * num12;
					}
				}
				return Mathf.Clamp01(num5);
			};
		}

		public override string GetTooltipText()
		{
			if (base.editor.rowsData == null || base.editor.rowsData.Count == 0 || base.row >= base.editor.rowsData.Count)
			{
				return string.Empty;
			}
			string text = base.editor.rowsData[base.row].GetRowString();
			if (rowTarget != MoveRowTarget.WholeRow)
			{
				text = text + " - " + RDString.GetEnumValue(rowTarget);
			}
			List<string> list = new List<string> { text };
			if (rowTarget != MoveRowTarget.WholeRow || customPosition)
			{
				if (((bool?)rowPosition) ?? false)
				{
					list.Add(string.Format("{0} {1}", RDString.Get("editor.MoveRow.rowPosition"), rowPosition));
				}
				if (((bool?)scale) ?? false)
				{
					list.Add(string.Format("{0} {1}", RDString.Get("editor.MoveRow.scale"), scale));
				}
				if (((bool?)angle) ?? false)
				{
					list.Add(string.Format("{0} {1}", RDString.Get("editor.MoveRow.angle"), angle));
				}
				if (pivot.HasValue)
				{
					list.Add(string.Format("{0} {1}", RDString.Get("editor.MoveRow.pivot"), pivot));
				}
			}
			return string.Join('\n', list);
		}
	}
}
