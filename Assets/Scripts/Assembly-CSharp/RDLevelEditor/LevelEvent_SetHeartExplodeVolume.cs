using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnPrebar, -1, RoomsUsage.NotUsed, true, false, true, false, -10, true)]
	public class LevelEvent_SetHeartExplodeVolume : LevelEvent_Base
	{
		[JsonProperty("", "", null, "", false, true, null, null)]
		[InputField("%", InputField.LineType.SingleLine, 40, 14, false, false)]
		[IntInfo(0, int.MaxValue)]
		public int volume { get; set; } = 60;

		[JsonProperty("", "", null, "", false, true, null, null)]
		[Description("editor.SetHeartExplodeVolume.description", 56, 8, TextAnchor.MiddleCenter)]
		public bool description => false;

		public override void Run()
		{
			LevelEvent_Base.level.heartExplodeMultiplier = (float)volume * 0.01f;
		}

		public override string GetTooltipText()
		{
			return volume + "%";
		}
	}
}
