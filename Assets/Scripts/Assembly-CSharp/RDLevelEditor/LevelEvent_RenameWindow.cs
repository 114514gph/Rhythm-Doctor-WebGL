using UnityEngine.UI;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnBar, 0, RoomsUsage.NotUsed, true, true, true, false, -10, false)]
	public class LevelEvent_RenameWindow : LevelEvent_Base
	{
		public int window => y;

		[JsonProperty("", "", null, "", false, true, null, null)]
		[ToggleGroup(false, null, null, null, true, 14)]
		public WindowNameAction action { get; set; }

		[JsonProperty("", "", null, "", false, true, "EnableTextIf", null)]
		[InputField("", InputField.LineType.SingleLine, 200, 14, false, true)]
		public string text { get; set; } = "";

		public bool EnableTextIf()
		{
			return action != WindowNameAction.Reset;
		}

		public override void Run()
		{
			if (base.game.windowChoreographer == null)
			{
				return;
			}
			RunOnBeat(() =>
			{
				Window window = base.game.windowChoreographer.dancers[this.window].window;
				printe($"setting name to window {this.window}, y {y}, tab {tab}");
				if (action == WindowNameAction.Reset)
				{
					window.ResetTitle();
				}
				else if (action == WindowNameAction.Set)
				{
					window.SetTitle(text);
				}
				else if (action == WindowNameAction.Append)
				{
					window.SetTitle(text, append: true);
				}
			});
		}

		public override string GetTooltipText()
		{
			if (action != WindowNameAction.Reset)
			{
				return ((action == WindowNameAction.Append) ? "+" : "") + text;
			}
			return "";
		}
	}
}
