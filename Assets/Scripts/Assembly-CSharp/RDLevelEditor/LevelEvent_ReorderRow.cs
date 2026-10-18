using System.Collections.Generic;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnBar, 0, RoomsUsage.NotUsed, true, true, true, false, 0, false)]
	public class LevelEvent_ReorderRow : LevelEvent_Base
	{
		[JsonProperty("", "", null, "", false, true, null, null)]
		[EnumInfo(true)]
		public RoomSelectType? newRoom { get; set; } = RoomSelectType.Room1;

		[JsonProperty("", "", null, "", false, true, null, null)]
		[Off]
		[IntInfo(1, 16)]
		public int? order { get; set; } = 1;

		[JsonProperty("", "", null, "", false, true, null, null)]
		[ToggleGroup(false, null, null, null, true, 14)]
		public CharacterReorderType transition { get; set; }

		[JsonProperty("", "", null, "", false, true, "EnableSortingOrderIf", null)]
		[Dropdown(new string[] { "Default", "Background", "Foreground" }, false, false)]
		[Off]
		public RDSortingLayer? sortingLayerName { get; set; } = RDSortingLayer.Default;

		[JsonProperty("", "", null, "", false, true, "EnableSortingOrderIf", null)]
		[Off]
		public int? sortingOrder { get; set; } = 0;

		public static bool EnableSortingOrderIf()
		{
			return RDBase.isDev;
		}

		public override void Run()
		{
			RunOnBeat(() =>
			{
				Row row = LevelEvent_Base.level.rows[base.row];
				RowEntity ent = row.ent;
				bool flag = false;
				int num = row.room;
				int num2 = -1;
				if (newRoom.HasValue)
				{
					num2 = (row.room = (int)newRoom.Value);
					ent.transform.SetParent(base.game.rooms[num2].rowContainer, worldPositionStays: false);
					flag = true;
				}
				if (sortingOrder.HasValue)
				{
					ent.SetSortOrder(sortingOrder.Value, exactDepth: true);
				}
				if (sortingLayerName.HasValue)
				{
					string sortLayer = ((sortingLayerName.Value == RDSortingLayer.Background) ? "Background" : ((sortingLayerName.Value == RDSortingLayer.Foreground) ? "Foreground" : "Default"));
					ent.SetSortLayer(sortLayer);
				}
				if (order.HasValue && !sortingOrder.HasValue)
				{
					int sortDepth = ent.sortDepth;
					int num3 = order.Value - 1;
					ent.transform.SetSiblingIndex(num3);
					if (sortDepth != num3)
					{
						base.game.rowSortOrder.Remove(base.row);
						base.game.rowSortOrder.Insert(num3, base.row);
						int num4 = 0;
						foreach (int item in base.game.rowSortOrder)
						{
							if ((bool)LevelEvent_Base.level.rows[item].ent)
							{
								LevelEvent_Base.level.rows[item].ent.SetSortOrder(num4);
								num4++;
							}
						}
						flag = true;
					}
				}
				if (flag && transition != CharacterReorderType.None)
				{
					base.game.rooms[num].RepositionRowsAndStrips(transition != CharacterReorderType.Smooth);
					if (num2 >= 0)
					{
						base.game.rooms[num2].RepositionRowsAndStrips(transition != CharacterReorderType.Smooth);
					}
				}
			});
		}

		public override string GetTooltipText()
		{
			if (base.row >= base.editor.rowsData.Count || base.row < 0)
			{
				return string.Empty;
			}
			string rowString = base.editor.rowsData[base.row].GetRowString();
			List<string> list = new List<string> { rowString };
			if (newRoom.HasValue)
			{
				list.Add(RDString.GetEnumValue(newRoom.Value));
			}
			if (order.HasValue)
			{
				list.Add(string.Format("{0} {1}", RDString.Get("editor.ReorderRow.order"), order));
			}
			return string.Join('\n', list);
		}
	}
}
