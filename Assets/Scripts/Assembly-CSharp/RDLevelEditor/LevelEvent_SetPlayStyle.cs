using System.Collections.Generic;
using UnityEngine;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnBar, 0, RoomsUsage.NotUsed, true, true, true, false, -10, false)]
	public class LevelEvent_SetPlayStyle : LevelEvent_Base
	{
		private int nextBar = 1;

		public bool relative = true;

		[JsonProperty("PlayStyle", "", null, "", false, true, null, null)]
		[Dropdown(new string[] { "Normal", "Loop", "Prolong", "Immediately", "ExtraImmediately", "ProlongOneBar" }, false, true)]
		public PlayStyleChange playStyle { get; set; } = PlayStyleChange.Normal;

		[JsonProperty("NextBar", "", null, "", false, true, "EnableIfNotLoop", null)]
		public int NextBar
		{
			get
			{
				return nextBar;
			}
			set
			{
				if (relative)
				{
					nextBar = value;
				}
				else
				{
					nextBar = Mathf.Max(1, value);
				}
				if (value != nextBar)
				{
					base.inspectorPanel.UpdateUI(this);
				}
			}
		}

		[JsonProperty("Relative", "", null, "", false, true, "EnableIfNotLoop", null)]
		[Checkbox]
		public bool Relative
		{
			get
			{
				return relative;
			}
			set
			{
				relative = value;
				NextBar = nextBar;
			}
		}

		public bool EnableIfNotLoop()
		{
			return playStyle != PlayStyleChange.Loop;
		}

		public override void Run()
		{
			RunOnBeat(() =>
			{
				if (relative)
				{
					base.game.currentLevel.SetNextBarRelative(nextBar);
				}
				else
				{
					base.game.currentLevel.SetNextBar(nextBar);
				}
				base.conductor.SetPlayStyle(playStyle);
			});
		}

		public override string GetTooltipText()
		{
			List<string> list = new List<string> { RDString.GetEnumValue(playStyle) };
			if (playStyle != PlayStyleChange.Loop)
			{
				string text = (relative ? nextBar.ToString("+#;-#;+0") : nextBar.ToString());
				string item = RDString.Get("editor.SetPlayStyle.nextBar") + " " + text;
				list.Add(item);
			}
			return string.Join('\n', list);
		}
	}
}
