namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnBar, 0, RoomsUsage.NotUsed, true, true, true, false, -10, false)]
	public class LevelEvent_AdvanceText : LevelEvent_SyllableBase
	{
		[JsonProperty("", "", null, "", false, true, null, null)]
		public int id { get; set; }

		protected override float fallbackFadeOutDuration => floatingText.fadeOutDuration;

		public LevelEvent_FloatingText floatingText
		{
			get
			{
				foreach (LevelEventControl_Base eventControl in base.editor.eventControls)
				{
					if (eventControl.levelEvent is LevelEvent_FloatingText levelEvent_FloatingText && levelEvent_FloatingText.id == id)
					{
						return levelEvent_FloatingText;
					}
				}
				return null;
			}
		}

		public override void Run()
		{
			RunOnBeat(() =>
			{
				float? customFadeTime = (base.fadeOutDuration.HasValue ? new float?(base.conductor.DurationBeatsToTime(base.beat - 1f, base.fadeOutDuration.Value)) : ((float?)null));
				foreach (LevelEvent_Base levelEvent in RDLevelData.current.levelEvents)
				{
					if (levelEvent is LevelEvent_FloatingText levelEvent_FloatingText && levelEvent_FloatingText.id == id)
					{
						if (base.vfx.allVfxLyrics.ContainsKey(id))
						{
							int[] currentIds = levelEvent_FloatingText.currentIds;
							foreach (int key in currentIds)
							{
								if (base.vfx.allVfxLyrics.TryGetValue(key, out var value))
								{
									value.AdvanceText(customFadeTime);
								}
							}
						}
						break;
					}
				}
			});
		}

		public override string GetTooltipText()
		{
			int num = 1;
			foreach (LevelEventControl_Base eventControl in base.editor.eventControls)
			{
				if (eventControl.levelEvent is LevelEvent_AdvanceText levelEvent_AdvanceText && eventControl.levelEvent.active && id == levelEvent_AdvanceText.id && levelEvent_AdvanceText.sortOrder < base.sortOrder && levelEvent_AdvanceText.sortOrder > floatingText.sortOrder)
				{
					num++;
				}
			}
			string[] array = LyricsGame.SplitLyricsLines(floatingText.GetLocalizedText(), useLinebreak: true);
			if (num < array.Length)
			{
				return array[num].Trim();
			}
			return null;
		}
	}
}
