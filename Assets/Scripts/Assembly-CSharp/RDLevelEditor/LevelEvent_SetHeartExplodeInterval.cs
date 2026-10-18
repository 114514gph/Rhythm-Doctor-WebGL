using UnityEngine.UI;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnPrebar, -1, RoomsUsage.NotUsed, true, true, true, false, -10, true)]
	public class LevelEvent_SetHeartExplodeInterval : LevelEvent_Base
	{
		[JsonProperty("", "", null, "", false, true, null, null)]
		[Dropdown(null, false, true)]
		public HeartExplodeType intervalType { get; set; } = HeartExplodeType.GatherAndCeil;

		[JsonProperty("", "", null, "", false, true, "EnableIntervalIf", null)]
		[InputField("beats", InputField.LineType.SingleLine, 40, 14, false, false)]
		public float interval { get; set; } = 1f;

		public bool EnableIntervalIf()
		{
			return intervalType != HeartExplodeType.Disabled;
		}

		public override void Run()
		{
			LevelEvent_Base.level.heartExplodeType = intervalType;
			LevelEvent_Base.level.crotchetsToExplode = interval;
		}

		public override string GetTooltipText()
		{
			if (!EnableIntervalIf())
			{
				return RDString.GetEnumValue(intervalType);
			}
			return $"{RDString.GetEnumValue(intervalType)}: {interval}";
		}
	}
}
