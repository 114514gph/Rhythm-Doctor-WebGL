using UnityEngine;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnBar, 0, RoomsUsage.NotUsed, true, true, true, true, -10, false)]
	public class LevelEvent_SetVisible : LevelEvent_Base, ICompatibleDecorationTypeChecker
	{
		[JsonProperty("", "", null, "", false, true, null, null)]
		[ToggleGroup(false, null, "editor.SetVisible.visible", null, false, 14)]
		public bool visible { get; set; } = true;

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
							Debug.Log("SetVisible - didn't found matching id " + target);
						}
						else
						{
							LevelEvent_Base.level.sprites[target].gameObject.SetActive(visible);
						}
					}
					else if (base.ownerDecoration is LevelEvent_Text)
					{
						if (!base.vfx.allDecorationLyrics.ContainsKey(target))
						{
							Debug.Log("SetVisible - didn't found matching id " + target);
						}
						else
						{
							base.vfx.allDecorationLyrics[target].gameObject.SetActive(visible);
						}
					}
				}
			});
		}

		public override string GetTooltipText()
		{
			if (!visible)
			{
				return RDString.Get("editor.SetVisible.hide");
			}
			return RDString.Get("editor.SetVisible.show");
		}
	}
}
