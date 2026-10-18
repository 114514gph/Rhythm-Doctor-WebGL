using System;
using UnityEngine;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnBar, 0, RoomsUsage.NotUsed, true, true, true, true, -10, false)]
	public class LevelEvent_SetFont : LevelEvent_Base, ICompatibleDecorationTypeChecker
	{
		public bool isCompatibleDecorationType => base.ownerDecoration is LevelEvent_Text;

		[JsonProperty("", "", null, "", false, true, null, null)]
		public string font { get; set; } = TextFont.Default.ToString();

		[JsonProperty("", "", null, "", false, true, "SupportsFontStyles", null)]
		public bool bold { get; set; }

		[JsonProperty("", "", null, "", false, true, "SupportsFontStyles", null)]
		public bool italic { get; set; }

		[JsonProperty("", "", null, "", false, true, "SupportsFontStyles", null)]
		public bool underline { get; set; }

		[JsonProperty("", "", null, "", false, true, null, null)]
		public float wrappingWidth { get; set; } = 100f;

		[JsonProperty("", "", null, "", false, true, "ShowExtraSettings", null)]
		public float characterSpacing { get; set; }

		[JsonProperty("", "", null, "", false, true, "ShowExtraSettings", null)]
		public float outlineWidth { get; set; } = 0.25f;

		public bool IsDefault()
		{
			if (Enum.TryParse<TextFont>(font, out var result))
			{
				return result == TextFont.Default;
			}
			return false;
		}

		public bool SupportsFontStyles()
		{
			TextFont result;
			return !Enum.TryParse<TextFont>(font, out result);
		}

		public bool ShowExtraSettings()
		{
			if (Enum.TryParse<TextFont>(font, out var result))
			{
				if (result != TextFont.Default)
				{
					return result != TextFont.Pixel;
				}
				return false;
			}
			return true;
		}

		public override void Run()
		{
			RunOnBeat(() =>
			{
				if (isCompatibleDecorationType && base.vfx.allDecorationLyrics.ContainsKey(target))
				{
					LyricsGame lyricsGame = base.vfx.allDecorationLyrics[target];
					if (lyricsGame != null)
					{
						lyricsGame.bold = bold;
						lyricsGame.SetLyricToFont(lyricsGame.fullText, font);
						lyricsGame.italic = italic;
						lyricsGame.underline = underline;
						lyricsGame.sizeDelta = lyricsGame.sizeDelta.WithX(scrVfxControl.instance.RDWidth * Mathf.Max(0f, wrappingWidth) / 100f);
						lyricsGame.characterSpacing = characterSpacing;
						lyricsGame.outlineWidth = Mathf.Clamp(outlineWidth, 0f, 1f);
						lyricsGame.SetRenderQueueForTextDecoration();
					}
				}
			});
		}

		public override string GetTooltipText()
		{
			return font;
		}
	}
}
