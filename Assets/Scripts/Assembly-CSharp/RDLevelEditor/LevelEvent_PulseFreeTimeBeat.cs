using UnityEngine;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnPrebar, 0, RoomsUsage.NotUsed, true, true, true, false, 0, false)]
	public class LevelEvent_PulseFreeTimeBeat : LevelEvent_Base
	{
		[JsonProperty("", "", null, "", false, true, null, null)]
		[ToggleGroup(true, null, null, null, true, 14)]
		public PulseAction action { get; set; }

		[JsonProperty("", "", null, "", false, true, "EnableCustomPulseIf", null)]
		[PulsePicker]
		public int customPulse { get; set; }

		[JsonProperty("", "", null, "", false, true, "EnableIfNotBomb", null)]
		public float hold { get; set; }

		[JsonProperty("", "", null, "", false, true, "EnableIfBomb", null)]
		[Description("editor.AddFreeTimeBeat.bombDescription", 32, 16, TextAnchor.MiddleCenter)]
		public bool description => false;

		public bool EnableCustomPulseIf()
		{
			return action == PulseAction.Custom;
		}

		public bool EnableIfBomb()
		{
			if (EnableCustomPulseIf())
			{
				return customPulse == -1;
			}
			return false;
		}

		public bool EnableIfNotBomb()
		{
			return !EnableIfBomb();
		}

		public float GetHoldLength()
		{
			if (!EnableIfBomb())
			{
				return hold;
			}
			return 0f;
		}

		public override void Run()
		{
		}

		public override string CustomControlName()
		{
			return "FreeTimeBeat";
		}
	}
}
