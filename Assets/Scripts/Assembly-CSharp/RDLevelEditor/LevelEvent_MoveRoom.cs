using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnBar, 0, RoomsUsage.NotUsed, true, true, true, false, -10, false)]
	public class LevelEvent_MoveRoom : LevelEvent_Base, IDurationHaver
	{
		[JsonProperty("", "", null, "", false, true, null, null)]
		[PositionPicker(true, true, null, false)]
		[Float2Info(-10000f, -10000f, 10000f, 10000f)]
		public Float2? roomPosition { get; set; } = new Float2(50f, 50f);

		[JsonProperty("", "", null, "", false, true, null, null)]
		[Off]
		public Float2? scale { get; set; } = new Float2(100f, 100f);

		[JsonProperty("", "", null, "", false, true, null, null)]
		[InputField("°", InputField.LineType.SingleLine, 40, 14, false, false)]
		[FloatInfo(-9999f, 9999f)]
		[Off]
		public float? angle { get; set; } = 0f;

		[JsonProperty("", "", null, "", false, true, null, null)]
		[Off]
		public Float2? pivot { get; set; } = new Float2(50f, 50f);

		[JsonProperty("", "", null, "", false, true, null, null)]
		[InputField("beats", InputField.LineType.SingleLine, 40, 14, false, false)]
		public float duration { get; set; } = 1f;

		[JsonProperty("", "", null, "", false, true, null, null)]
		public Ease ease { get; set; } = Ease.Linear;

		public void Validate()
		{
			Float2? @float = roomPosition;
			if (@float.HasValue)
			{
				Float2 valueOrDefault = @float.GetValueOrDefault();
				valueOrDefault.x = Mathf.Clamp(valueOrDefault.x, -10000f, 10000f);
				valueOrDefault.y = Mathf.Clamp(valueOrDefault.y, -10000f, 10000f);
				roomPosition = valueOrDefault;
			}
			float? num = angle;
			if (num.HasValue)
			{
				float valueOrDefault2 = num.GetValueOrDefault();
				angle = Mathf.Clamp(valueOrDefault2, -9999f, 9999f);
			}
		}

		public override void Run()
		{
			RunOnBeat(() =>
			{
				Validate();
				float num = base.conductor.DurationBeatsToTime(base.beat - 1f, duration);
				RDRoom rDRoom = base.game.rooms[y];
				Transform transform = rDRoom.renderQuad.transform;
				Transform renderQuadPivot = rDRoom.renderQuadPivot;
				Float2? @float = roomPosition;
				if (@float.HasValue)
				{
					Float2 valueOrDefault = @float.GetValueOrDefault();
					if (valueOrDefault.xUsed)
					{
						if (rDRoom.positionTweenX != null)
						{
							rDRoom.positionTweenX.Kill();
						}
						float num2 = base.vfx.RDWidth * valueOrDefault.x / 100f - base.vfx.RDWidth / 2f;
						rDRoom.moveRoomPositionXUsed = true;
						rDRoom.moveRoomPosition.x = num2;
						rDRoom.positionTweenX = renderQuadPivot.DOLocalMoveX(num2, num).SetEase(ease).Done();
					}
					if (valueOrDefault.yUsed)
					{
						if (rDRoom.positionTweenY != null)
						{
							rDRoom.positionTweenY.Kill();
						}
						float endValue = base.vfx.RDHeight * valueOrDefault.y / 100f - base.vfx.RDHeight / 2f;
						rDRoom.moveRoomPositionYUsed = true;
						rDRoom.moveRoomPosition.y = endValue;
						rDRoom.positionTweenY = renderQuadPivot.DOLocalMoveY(endValue, num).SetEase(ease).Done();
					}
				}
				@float = scale;
				if (@float.HasValue)
				{
					Float2 valueOrDefault2 = @float.GetValueOrDefault();
					if (valueOrDefault2.xUsed)
					{
						rDRoom.scaleTweenX?.Kill();
						float num3 = valueOrDefault2.x / 100f;
						rDRoom.moveRoomScaleXUsed = true;
						rDRoom.moveRoomScale.x = num3;
						rDRoom.scaleTweenX = renderQuadPivot.DOScaleX(num3, num).SetEase(ease).Done();
					}
					if (valueOrDefault2.yUsed)
					{
						rDRoom.scaleTweenY?.Kill();
						float endValue2 = valueOrDefault2.y / 100f;
						rDRoom.moveRoomScaleYUsed = true;
						rDRoom.moveRoomScale.y = endValue2;
						rDRoom.scaleTweenY = renderQuadPivot.DOScaleY(endValue2, num).SetEase(ease).Done();
					}
				}
				float? num4 = angle;
				if (num4.HasValue)
				{
					float valueOrDefault3 = num4.GetValueOrDefault();
					rDRoom.moveRoomAngleUsed = true;
					rDRoom.moveRoomAngle = valueOrDefault3;
					rDRoom.Rotate(valueOrDefault3, num, ease);
				}
				@float = pivot;
				if (@float.HasValue)
				{
					Float2 valueOrDefault4 = @float.GetValueOrDefault();
					if (RDLevelData.current.settings.version >= 64)
					{
						if (valueOrDefault4.xUsed)
						{
							rDRoom.pivotTweenX?.Kill();
							float num5 = (0.5f - valueOrDefault4.x / 100f) * RDClass.Vfx.RDWidth;
							rDRoom.moveRoomPivotUsed = true;
							rDRoom.moveRoomPivot.x = num5;
							rDRoom.pivotTweenX = transform.DOLocalMoveX(num5, num).SetEase(ease).Done();
						}
						if (valueOrDefault4.yUsed)
						{
							rDRoom.pivotTweenY?.Kill();
							float endValue3 = (0.5f - valueOrDefault4.y / 100f) * RDClass.Vfx.RDHeight;
							rDRoom.moveRoomPivotUsed = true;
							rDRoom.moveRoomPivot.y = endValue3;
							rDRoom.pivotTweenY = transform.DOLocalMoveY(endValue3, num).SetEase(ease).Done();
						}
					}
					else
					{
						Vector3 vector = new Vector3((0.5f - valueOrDefault4.x / 100f) * RDClass.Vfx.RDWidth, (0.5f - valueOrDefault4.y / 100f) * RDClass.Vfx.RDHeight, 0f);
						transform.DOKill();
						rDRoom.moveRoomPivotUsed = true;
						rDRoom.moveRoomPivot = vector;
						transform.DOLocalMove(vector, num).SetEase(ease).Done();
					}
				}
				int customTransform;
				if (!rDRoom.customTransform)
				{
					@float = roomPosition;
					if (!@float.HasValue || !@float.GetValueOrDefault().xUsed)
					{
						@float = roomPosition;
						if (!@float.HasValue || !@float.GetValueOrDefault().yUsed)
						{
							@float = scale;
							if (!@float.HasValue || !@float.GetValueOrDefault().xUsed)
							{
								@float = scale;
								if ((!@float.HasValue || !@float.GetValueOrDefault().yUsed) && !angle.HasValue)
								{
									customTransform = (pivot.HasValue ? 1 : 0);
									goto IL_04c2;
								}
							}
						}
					}
				}
				customTransform = 1;
				goto IL_04c2;
				IL_04c2:
				rDRoom.customTransform = (byte)customTransform != 0;
				if (rDRoom.customTransform)
				{
					rDRoom.roomMovedByLevelEvent = true;
				}
			});
		}

		public override string CustomControlName()
		{
			return "Room";
		}

		public override string GetTooltipText()
		{
			List<string> list = new List<string>();
			if (((bool?)roomPosition) ?? false)
			{
				list.Add(string.Format("{0} {1}", RDString.Get("editor.MoveRoom.roomPosition"), roomPosition));
			}
			if (((bool?)scale) ?? false)
			{
				list.Add(string.Format("{0} {1}", RDString.Get("editor.MoveRoom.scale"), scale));
			}
			if (angle.HasValue)
			{
				list.Add(string.Format("{0} {1}", RDString.Get("editor.MoveRoom.angle"), angle));
			}
			if (((bool?)pivot) ?? false)
			{
				list.Add(string.Format("{0} {1}", RDString.Get("editor.MoveRoom.pivot"), pivot));
			}
			return string.Join('\n', list);
		}
	}
}
