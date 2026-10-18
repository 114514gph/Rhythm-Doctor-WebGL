using System.Collections.Generic;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnBar, 0, RoomsUsage.NotUsed, true, true, true, true, -10, false)]
	public class LevelEvent_ReorderDecoration : LevelEvent_Base, ICompatibleDecorationTypeChecker
	{
		[JsonProperty("", "", "editor.ReorderRow.newRoom", "", false, true, null, null)]
		[EnumInfo(true)]
		public RoomSelectType? newRoom { get; set; } = RoomSelectType.Room1;

		[JsonProperty("", "", null, "", false, true, null, null)]
		public int? depth { get; set; } = 0;

		[JsonProperty("", "", null, "", false, true, null, null)]
		[Dropdown(new string[] { "Default", "Background", "Foreground" }, false, false)]
		[Off]
		public RDSortingLayer? sortingLayerName { get; set; } = RDSortingLayer.Default;

		public bool isCompatibleDecorationType
		{
			get
			{
				LevelEvent_MakeDecorationBase levelEvent_MakeDecorationBase = base.ownerDecoration;
				return levelEvent_MakeDecorationBase is LevelEvent_Sprite || levelEvent_MakeDecorationBase is LevelEvent_Text;
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
					if (newRoom.HasValue)
					{
						int value = (int)newRoom.Value;
						SetRoom(value);
					}
					if (sortingLayerName.HasValue)
					{
						string text = ((sortingLayerName.Value == RDSortingLayer.Background) ? "Background" : ((sortingLayerName.Value == RDSortingLayer.Foreground) ? "Foreground" : "Default"));
						SetSortingLayerName(text);
					}
					if (depth.HasValue)
					{
						SetDepth(depth.Value);
					}
				}
			});
		}

		private void SetRoom(int roomIndex)
		{
			if (base.ownerDecoration is LevelEvent_Sprite)
			{
				LevelEvent_Base.level.sprites[target].transform.SetParent(base.game.rooms[roomIndex].decorationContainer, worldPositionStays: false);
			}
			else if (base.ownerDecoration is LevelEvent_Text)
			{
				LyricsGame lyricsGame = base.vfx.allDecorationLyrics[target];
				bool followCam = !base.game.rooms[roomIndex].peekWindowMode;
				lyricsGame.SetRoom(followCam, roomIndex);
			}
		}

		private void SetSortingLayerName(string layerName)
		{
			if (base.ownerDecoration is LevelEvent_Sprite)
			{
				LevelEvent_Base.level.sprites[target].renderer.sortingLayerName = layerName;
			}
			else if (base.ownerDecoration is LevelEvent_Text)
			{
				base.vfx.allDecorationLyrics[target].canvas.sortingLayerName = layerName;
			}
		}

		private void SetDepth(int depth)
		{
			if (base.ownerDecoration is LevelEvent_Sprite)
			{
				CustomSprite customSprite = LevelEvent_Base.level.sprites[target];
				int sortingOrder = ((customSprite.renderer.sortingLayerName == "Default") ? (-depth) : depth);
				customSprite.renderer.sortingOrder = sortingOrder;
			}
			else if (base.ownerDecoration is LevelEvent_Text)
			{
				LyricsGame lyricsGame = base.vfx.allDecorationLyrics[target];
				int sortingOrder2 = ((lyricsGame.canvas.sortingLayerName == "Default") ? (-depth) : depth);
				lyricsGame.canvas.sortingOrder = sortingOrder2;
			}
		}

		public override string GetTooltipText()
		{
			List<string> list = new List<string>();
			if (newRoom.HasValue)
			{
				list.Add(RDString.GetEnumValue(newRoom.Value));
			}
			if (depth.HasValue)
			{
				list.Add(string.Format("{0}: {1}", RDString.Get("editor.depth"), depth));
			}
			return string.Join('\n', list);
		}
	}
}
