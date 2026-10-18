using UnityEngine;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnBar, 0, RoomsUsage.NotUsed, true, true, true, false, -10, false)]
	public class LevelEvent_ReorderRooms : LevelEvent_Base
	{
		[JsonProperty("", "", null, "", false, true, null, null)]
		[ReorderRooms(false)]
		public int[] order { get; set; } = new int[4] { 0, 1, 2, 3 };

		public override void Run()
		{
			RunOnBeat(() =>
			{
				for (int i = 0; i < order.Length; i++)
				{
					int sortingOrder = 4 - i;
					int num = order[i];
					base.game.rooms[num].renderQuad.GetComponent<Renderer>().sortingOrder = sortingOrder;
				}
			});
		}

		public override string CustomControlName()
		{
			return "ShowRooms";
		}
	}
}
