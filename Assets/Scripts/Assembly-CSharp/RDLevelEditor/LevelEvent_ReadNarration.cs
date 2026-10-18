using UnityEngine.UI;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnBar, 0, RoomsUsage.NotUsed, true, true, true, false, -10, false)]
	public class LevelEvent_ReadNarration : LevelEvent_Base
	{
		[JsonProperty("", "", null, "", false, true, null, null)]
		[InputField("", InputField.LineType.MultiLineNewline, 900, 84, false, true)]
		public string text { get; set; }

		[JsonProperty("", "", null, "", false, true, null, null)]
		[Dropdown(new string[] { "Notification", "Description", "Subtitles", "Instruction" }, false, false)]
		public NarrationCategory category { get; set; } = NarrationCategory.Description;

		public override void Init()
		{
			base.Init();
			text = RDString.Get("editor.ReadNarration.exampleText");
		}

		public override void Run()
		{
			if (base.conductor.isScrubbing || !Narration.CheckCategoryEnabled(category))
			{
				return;
			}
			string textToSay = this.text;
			string[] array = this.text.Split('|');
			for (int i = 0; i <= array.Length - 1; i++)
			{
				string text = array[i].Trim();
				if (text.StartsWith("[[") && text.EndsWith("]]"))
				{
					array[i] = RDString.Get(text.Substring(2, text.Length - 4));
				}
			}
			textToSay = string.Join(' ', array).RemoveRichTextColorTags();
			textToSay = base.game.currentLevel.EvaluateCurlyBracketsInString(textToSay);
			RunOnBeat(() =>
			{
				Narration.Say(textToSay, category, false);
			});
		}

		public override string GetTooltipText()
		{
			return text;
		}
	}
}
