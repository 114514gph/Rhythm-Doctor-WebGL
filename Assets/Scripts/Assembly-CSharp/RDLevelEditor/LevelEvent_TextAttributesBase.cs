using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RDLevelEditor
{
	public abstract class LevelEvent_TextAttributesBase : LevelEvent_Base, IDurationHaver
	{
		private const char LyricsSeparator = '/';

		private const char LyricsSeparatorCJK = '／';

		private const char ln = '\n';

		[JsonProperty("", "", null, "", false, true, null, null)]
		public string text { get; set; }

		[JsonProperty("", "", null, "", false, true, null, null)]
		public ColorOrPalette color { get; set; } = Color.white;

		[JsonProperty("", "", null, "", false, true, null, null)]
		public ColorOrPalette outlineColor { get; set; } = Color.black;

		[JsonProperty("", "", null, "", false, true, null, null)]
		public Float2? textPosition { get; set; } = new Float2(50f, 50f);

		[JsonProperty("", "", null, "", false, true, null, null)]
		[IntInfo(0, int.MaxValue)]
		public int size { get; set; } = 8;

		[JsonProperty("", "", null, "", false, true, null, null)]
		public float? angle { get; set; } = 0f;

		[JsonProperty("fadeOutRate", "", null, "", false, true, null, null)]
		[FloatInfo(0f, float.PositiveInfinity)]
		public float fadeOutDuration { get; set; } = 3f;

		public float duration
		{
			get
			{
				return fadeOutDuration;
			}
			set
			{
				fadeOutDuration = value;
			}
		}

		[JsonProperty("", "", null, "", false, true, null, null)]
		public FloatingTextMode mode { get; set; } = FloatingTextMode.FadeOut;

		[JsonProperty("", "", null, "", false, true, null, null)]
		public TextAnchor anchor { get; set; } = TextAnchor.MiddleCenter;

		[JsonProperty("", "", null, "", false, true, "EnableNarrateIf", null)]
		[Checkbox]
		public bool narrate { get; set; } = true;

		[JsonProperty("", "", null, "", false, true, "EnableNarrateIf", null)]
		public NarrationCategory narrationCategory { get; set; } = NarrationCategory.Subtitles;

		public string[] splitLyrics => LyricsGame.SplitLyricsLines(text, useLinebreak: true);

		public bool EnableNarrateIf()
		{
			return Narration.IsAvailable;
		}

		public override void Init()
		{
			base.Init();
			text = RDString.Get("editor.FloatingText.text.example");
		}

		public string GetLocalizedText()
		{
			string text = "[[";
			string text2 = "]]";
			int num = this.text.IndexOf(text, 0);
			int num2 = this.text.IndexOf(text2, 0);
			if (num == -1 || num2 == -1 || num >= num2)
			{
				return this.text;
			}
			string text3 = this.text.Substring(num, num2 + text2.Length - num);
			string key = text3.Replace(text, "").Replace(text2, "");
			bool exists = false;
			string withCheck = RDString.GetWithCheck(key, out exists);
			if (exists)
			{
				return this.text.Replace(text3, withCheck);
			}
			return this.text;
		}

		public override IEnumerator Prepare()
		{
			text = GetLocalizedText();
			prepared = true;
			yield return null;
		}

		public override void Decode(Dictionary<string, object> dict)
		{
			if (RDLevelData.current.settings.version <= 50)
			{
				dict["narrate"] = false;
			}
			if (dict.ContainsKey("narrationCategory") && (string)dict["narrationCategory"] == "Main")
			{
				dict["narrationCategory"] = "Fallback";
			}
			base.Decode(dict);
		}

		public override string GetTooltipText()
		{
			return text;
		}
	}
}
