using System.Collections.Generic;
using UnityEngine.UI;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnBar, 0, RoomsUsage.NotUsed, true, true, true, false, -10, false)]
	public class LevelEvent_ShowStatusSign : LevelEvent_Base, IDurationHaver
	{
		[JsonProperty("", "", null, "", false, true, null, null)]
		[InputField("", InputField.LineType.MultiLineNewline, 200, 32, false, true)]
		public string text { get; set; } = "";

		[JsonProperty("", "", null, "", false, true, null, null)]
		[InputField("beats", InputField.LineType.SingleLine, 40, 14, false, false)]
		[FloatInfo(0f, float.PositiveInfinity)]
		public float duration { get; set; } = 4f;

		[JsonProperty("", "", null, "", false, true, null, null)]
		[Checkbox]
		public bool useBeats { get; set; } = true;

		[JsonProperty("", "", null, "", false, true, "EnableNarrateIf", null)]
		[Checkbox]
		public bool narrate { get; set; } = true;

		public bool EnableNarrateIf()
		{
			return Narration.IsAvailable;
		}

		public override void Decode(Dictionary<string, object> dict)
		{
			if (RDLevelData.current.settings.version <= 50)
			{
				dict["narrate"] = false;
			}
			base.Decode(dict);
		}

		public override void Run()
		{
			if (base.conductor.isScrubbing)
			{
				return;
			}
			string[] array = this.text.Split('|');
			for (int i = 0; i <= array.Length - 1; i++)
			{
				string text = array[i];
				if (text.StartsWith("[[") && text.EndsWith("]]"))
				{
					array[i] = RDString.Get(text.Substring(2, text.Length - 4));
				}
			}
			string textToShow = string.Concat(array);
			if (useBeats)
			{
				duration = base.conductor.DurationBeatsToTime(base.beat - 1f, duration);
			}
			RunOnBeat(() =>
			{
				LEDSign statusText = base.game.statusText;
				string obj = base.game.currentLevel.EvaluateCurlyBracketsInString(textToShow);
				float num = duration;
				bool flag = narrate || (Persistence.GetForceNarrateCustomLevels() && scnGame.levelToLoadSource == LevelSource.ExternalPath);
				statusText.SetStatusText(obj, null, num, flag);
			});
		}

		public override string GetTooltipText()
		{
			return text;
		}
	}
}
