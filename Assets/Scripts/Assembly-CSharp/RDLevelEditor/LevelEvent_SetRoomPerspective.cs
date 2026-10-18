using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnBar, 0, RoomsUsage.NotUsed, true, true, true, false, -10, false)]
	public class LevelEvent_SetRoomPerspective : LevelEvent_Base, IDurationHaver
	{
		public bool[] cornerPositionsUsed = new bool[4] { true, true, true, true };

		[JsonProperty("", "", null, "", false, true, null, null)]
		[SetRoomPerspective(-10000f, 10000f)]
		public Float2[] cornerPositions { get; set; } = new Float2[4]
		{
			new Float2(0f, 0f),
			new Float2(100f, 0f),
			new Float2(0f, 100f),
			new Float2(100f, 100f)
		};

		[JsonProperty("", "", null, "", false, true, null, null)]
		[InputField("beats", InputField.LineType.SingleLine, 40, 14, false, false)]
		public float duration { get; set; } = 1f;

		[JsonProperty("", "", null, "", false, true, null, null)]
		public Ease ease { get; set; } = Ease.Linear;

		public override void Run()
		{
			RunOnBeat(() =>
			{
				float num = base.conductor.DurationBeatsToTime(base.beat - 1f, duration);
				RDRoom room = base.game.rooms[y];
				bool flag = false;
				for (int i = 0; i < 4; i++)
				{
					if (cornerPositionsUsed[i])
					{
						Float2 @float = cornerPositions[i];
						if (@float.xUsed)
						{
							RDRoom rDRoom = room;
							if ((object)rDRoom != null)
							{
								rDRoom.vertexTweensX[i].Kill();
							}
							float num2 = @float.x / 100f - 0.5f;
							room.perspectiveRoomCornersUsed[i] = true;
							room.perspectiveRoomCorners[i].x = num2;
							int targetIndex = i;
							room.vertexTweensX[i] = DOTween.To(() => room.currentVertexPositions[targetIndex].x, (float x) =>
							{
								room.currentVertexPositions[targetIndex].x = x;
							}, num2, num).SetEase(ease).Done();
							flag = true;
						}
						if (@float.yUsed)
						{
							room.vertexTweensY[i]?.Kill();
							float endValue = @float.y / 100f - 0.5f;
							room.perspectiveRoomCornersUsed[i] = true;
							room.perspectiveRoomCorners[i].y = endValue;
							int targetIndex2 = i;
							room.vertexTweensY[i] = DOTween.To(() => room.currentVertexPositions[targetIndex2].y, (float y) =>
							{
								room.currentVertexPositions[targetIndex2].y = y;
							}, endValue, num).SetEase(ease).Done();
							flag = true;
						}
					}
				}
				if (flag)
				{
					room.UpdateVertices();
					room.customTransform = true;
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
			string[] array = new string[4] { "bottomLeft", "bottomRight", "topLeft", "topRight" };
			for (int i = 0; i < cornerPositions.Length; i++)
			{
				Float2 @float = cornerPositions[i];
				if (cornerPositionsUsed[i] && (bool)@float)
				{
					list.Add(string.Format("{0} {1}", RDString.Get("editor.SetRoomPerspective." + array[i]), @float));
				}
			}
			return string.Join('\n', list);
		}
	}
}
