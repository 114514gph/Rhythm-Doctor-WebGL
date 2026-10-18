using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnBar, 0, RoomsUsage.ManyRoomsAndOnTop, true, true, true, false, -10, false)]
	public class LevelEvent_MoveCamera : LevelEvent_Base, IDurationHaver
	{
		public bool legacyOnTopMovement;

		[JsonProperty("", "", null, "", false, true, null, null)]
		[PositionPicker(true, true, null, false)]
		[Float2Info(-100f, -100f, 200f, 200f)]
		public Float2? cameraPosition { get; set; } = new Float2(50f, 50f);

		[JsonProperty("", "", null, "", false, true, null, null)]
		[InputField("%", InputField.LineType.SingleLine, 40, 14, false, false)]
		[IntInfo(1, 9999)]
		[Off]
		public int? zoom { get; set; } = 100;

		[JsonProperty("", "", null, "", false, true, null, null)]
		[InputField("°", InputField.LineType.SingleLine, 40, 14, false, false)]
		[FloatInfo(-9999f, 9999f)]
		[Off]
		public float? angle { get; set; } = 0f;

		[JsonProperty("", "", null, "", false, true, null, null)]
		[InputField("beats", InputField.LineType.SingleLine, 40, 14, false, false)]
		public float duration { get; set; } = 1f;

		[JsonProperty("", "", null, "", false, true, null, null)]
		public Ease ease { get; set; }

		[JsonProperty("real", "", "editor.MoveCamera.realMovement", "", false, true, "EnableRealMovementIf", "SaveRealMovementIf")]
		public bool realMovement { get; set; }

		[JsonProperty("", "", null, "", false, true, "EnableRealMovementWindowIf", null)]
		public int window { get; set; } = -1;

		public bool EnableRealMovementIf()
		{
			return RDBase.isAdvanced;
		}

		public bool EnableRealMovementWindowIf()
		{
			if (realMovement)
			{
				return window != -1;
			}
			return false;
		}

		public bool SaveRealMovementIf()
		{
			return realMovement;
		}

		public void Validate()
		{
			int? num = zoom;
			if (num.HasValue)
			{
				int valueOrDefault = num.GetValueOrDefault();
				zoom = RDUtils.Clamp(valueOrDefault, 1, 9999);
			}
			Float2? @float = cameraPosition;
			if (@float.HasValue)
			{
				Float2 valueOrDefault2 = @float.GetValueOrDefault();
				if (!realMovement)
				{
					valueOrDefault2.x = Mathf.Clamp(valueOrDefault2.x, -100f, 200f);
					valueOrDefault2.y = Mathf.Clamp(valueOrDefault2.y, -100f, 200f);
					cameraPosition = valueOrDefault2;
				}
			}
			float? num2 = angle;
			if (num2.HasValue)
			{
				float valueOrDefault3 = num2.GetValueOrDefault();
				angle = Mathf.Clamp(valueOrDefault3, -9999f, 9999f);
			}
		}

		public override void Decode(Dictionary<string, object> dict)
		{
			base.Decode(dict);
			if (RDLevelData.current.settings.version < 25)
			{
				base.room = 4;
			}
			legacyOnTopMovement = RDLevelData.current.settings.version < 53;
		}

		public override void Run()
		{
			RunOnBeat(() =>
			{
				Validate();
				int[] array = rooms;
				foreach (int num in array)
				{
					RDCamera camera = base.vfx.GetCamera(num);
					float num2 = base.conductor.DurationBeatsToTime(base.beat - 1f, duration);
					_ = base.vfx.RDHeight / 2f;
					if (num == -1 || (realMovement && window == -1))
					{
						Float2? @float = cameraPosition;
						if (@float.HasValue)
						{
							Float2 valueOrDefault = @float.GetValueOrDefault();
							Vector3 vector = new Vector3(base.vfx.RDWidth * valueOrDefault.x / 100f, base.vfx.RDHeight * valueOrDefault.y / 100f, camera.transform.localPosition.z);
							if (legacyOnTopMovement)
							{
								camera.transform.DOKill();
							}
							if (valueOrDefault.xUsed)
							{
								camera.quad_MoveXTween?.Kill();
								camera.quad_MoveXTween = camera.transform.DOLocalMoveX(vector.x, num2).SetEase(ease).Done();
							}
							if (valueOrDefault.yUsed)
							{
								camera.quad_MoveYTween?.Kill();
								camera.quad_MoveYTween = camera.transform.DOLocalMoveY(vector.y, num2).SetEase(ease).Done();
							}
						}
						float? num3 = angle;
						if (num3.HasValue)
						{
							float valueOrDefault2 = num3.GetValueOrDefault();
							camera.Rotate(valueOrDefault2, num2, ease);
						}
						int? num4 = zoom;
						if (num4.HasValue)
						{
							int valueOrDefault3 = num4.GetValueOrDefault();
							float finalZoom = (float)valueOrDefault3 / 100f;
							camera.Zoom(finalZoom, num2, ease);
						}
					}
					else
					{
						Material material = ((window == -1) ? base.vfx.rooms[num].renderQuad.GetComponent<Renderer>().material : base.game.windowChoreographer.dancers[window].window.material);
						Float2? @float = cameraPosition;
						if (@float.HasValue)
						{
							Float2 valueOrDefault4 = @float.GetValueOrDefault();
							if (valueOrDefault4.xUsed)
							{
								camera.quad_MoveXTween?.Kill();
								float endValue = valueOrDefault4.x / 100f - 0.5f;
								camera.quad_MoveXTween = material.DOFloat(endValue, "_PosX", num2).SetEase(ease).Done();
							}
							if (valueOrDefault4.yUsed)
							{
								camera.quad_MoveYTween?.Kill();
								float endValue2 = valueOrDefault4.y / 100f - 0.5f;
								camera.quad_MoveYTween = material.DOFloat(endValue2, "_PosY", num2).SetEase(ease).Done();
							}
						}
						float? num3 = angle;
						if (num3.HasValue)
						{
							float valueOrDefault5 = num3.GetValueOrDefault();
							camera.quad_AngleTween?.Kill();
							float endValue3 = valueOrDefault5 * MathF.PI / 180f;
							camera.quad_AngleTween = material.DOFloat(endValue3, "_Angle", num2).SetEase(ease).Done();
						}
						int? num4 = zoom;
						if (num4.HasValue)
						{
							int valueOrDefault6 = num4.GetValueOrDefault();
							float endValue4 = (float)valueOrDefault6 / 100f;
							camera.quad_ZoomTween?.Kill();
							camera.quad_ZoomTween = material.DOFloat(endValue4, "_Scale", num2).SetEase(ease).Done();
						}
					}
				}
			});
		}

		public override string GetTooltipText()
		{
			List<string> list = new List<string>();
			if (realMovement)
			{
				list.Add(RDString.Get("editor.MoveCamera.realMovement"));
			}
			if (((bool?)cameraPosition) ?? false)
			{
				list.Add(string.Format("{0} {1}", RDString.Get("editor.MoveCamera.cameraPosition"), cameraPosition));
			}
			if (zoom.HasValue)
			{
				list.Add(string.Format("{0} {1}", RDString.Get("editor.MoveCamera.zoom"), zoom));
			}
			if (angle.HasValue)
			{
				list.Add(string.Format("{0} {1}", RDString.Get("editor.MoveCamera.angle"), angle));
			}
			return string.Join('\n', list);
		}
	}
}
