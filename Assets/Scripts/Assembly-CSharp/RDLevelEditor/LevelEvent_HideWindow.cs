namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnBar, 0, RoomsUsage.NotUsed, true, true, true, false, -10, false)]
	public class LevelEvent_HideWindow : LevelEvent_Base
	{
		public int window => y;

		[JsonProperty("", "", null, "", false, true, null, null)]
		[ToggleGroup(false, null, "editor.SetVisible.visible", null, false, 14)]
		public bool show { get; set; }

		[Off]
		[JsonProperty("", "", null, "", false, true, "EnableIfDev", null)]
		public bool? transparent { get; set; } = false;

		[Off]
		[JsonProperty("", "", null, "", false, true, "EnableIfDev", null)]
		public bool? frameless { get; set; } = false;

		public static bool EnableIfDev()
		{
			return RDBase.isAdvanced;
		}

		public override void Run()
		{
			RunOnBeat(() =>
			{
				WindowChoreographer windowChoreographer = base.game.windowChoreographer;
				if (windowChoreographer != null)
				{
					WindowDancer windowDancer = windowChoreographer.dancers[window];
					windowDancer.SetVisible(show);
					bool? flag = transparent;
					if (flag.HasValue)
					{
						bool valueOrDefault = flag == true;
						windowDancer.window.SetTransparent(valueOrDefault);
					}
					flag = frameless;
					if (flag.HasValue)
					{
						bool valueOrDefault2 = flag == true;
						windowDancer.window.SetFrameless(valueOrDefault2);
					}
				}
			});
		}

		public override string GetTooltipText()
		{
			if (!show)
			{
				return RDString.Get("editor.SetVisible.hide");
			}
			return RDString.Get("editor.SetVisible.show");
		}
	}
}
