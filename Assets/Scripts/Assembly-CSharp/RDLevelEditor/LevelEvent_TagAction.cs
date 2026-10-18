using System;
using RDTools;
using UnityEngine.UI;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnBar, 0, RoomsUsage.NotUsed, true, true, true, false, -10, false)]
	public class LevelEvent_TagAction : LevelEvent_Base
	{
		[JsonProperty("Action", "", null, "", false, true, null, null)]
		[Dropdown(new string[] { "Run", "RunAll", "RunRandom", "Enable", "Disable", "EnableAll", "DisableAll" }, false, true)]
		public TagAction action { get; set; }

		[JsonProperty("Tag", "", null, "", false, true, null, null)]
		[InputField("", InputField.LineType.SingleLine, 40, 14, false, true)]
		public string tagName { get; set; }

		public override void Run()
		{
			LevelBase level = base.game.currentLevel;
			if (action != TagAction.Run && action != TagAction.RunAll && action != TagAction.RunRandom)
			{
				RunOnBeat(() =>
				{
					RunTag(tagName, soundOnly: false, level);
				});
			}
			else if (!level.runningFromTag)
			{
				RunTagOnBeat(tagName, soundOnly: false, level);
				RunTagOnBeat(tagName, soundOnly: true, level);
			}
			else
			{
				RunTagOnBeat(tagName, level.runningTagEventType == TaggedEventType.OnlySound, level);
			}
		}

		private void RunTagOnBeat(string tagName, bool soundOnly, LevelBase level)
		{
			if (!soundOnly)
			{
				RunOnBeat(() =>
				{
					RunTag(tagName, soundOnly: false, level);
				});
			}
			else
			{
				RunOnBeat(() =>
				{
					RunTag(tagName, soundOnly: true, level);
				}, forcePrebar: false, null, base.beat, forceNoIncrementTimesRun: false, forceCheckConditionsEarlier: false, forceMinusLatency: true);
			}
		}

		private void RunTag(string tagName, bool soundOnly, LevelBase level)
		{
			string text = tagName;
			try
			{
				text = level.EvaluateCurlyBracketsInString(tagName);
			}
			catch (Exception ex)
			{
				RDClassDll.printem("Using curly brackets but is not a variable: " + ex.Message);
			}
			if (!base.conductor.isScrubbingRefreshOnBar || !(tag == text))
			{
				TaggedEventType typeToRun = (soundOnly ? TaggedEventType.OnlySound : TaggedEventType.OnlyNonSound);
				switch (action)
				{
				case TagAction.Run:
					level.RunTagWithType(text, typeToRun);
					break;
				case TagAction.RunAll:
				{
					string text2 = text;
					level.RunTagContaining(false, typeToRun, text2);
					break;
				}
				case TagAction.Enable:
					level.EnableTag(text);
					break;
				case TagAction.Disable:
					level.DisableTag(text);
					break;
				case TagAction.EnableAll:
					level.EnableTagsContaining(text);
					break;
				case TagAction.DisableAll:
					level.DisableTagsContaining(text);
					break;
				case TagAction.RunRandom:
					level.RunRandomTagWithType(text, typeToRun);
					break;
				default:
					throw new ArgumentOutOfRangeException();
				}
			}
		}

		public override string GetTooltipText()
		{
			if (action == TagAction.Run)
			{
				return tagName;
			}
			return RDString.GetEnumValue(action) + "\n" + tagName;
		}
	}
}
