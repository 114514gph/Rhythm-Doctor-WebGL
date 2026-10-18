namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnBar, 0, RoomsUsage.NotUsed, true, true, true, true, -10, false)]
	public class LevelEvent_AdvanceTextDecoration : LevelEvent_SyllableBase, ICompatibleDecorationTypeChecker
	{
		protected override float fallbackFadeOutDuration
		{
			get
			{
				if (lastSetTextEvent == null)
				{
					return 0f;
				}
				return lastSetTextEvent.fadeOutDuration;
			}
		}

		public bool isCompatibleDecorationType => base.ownerDecoration is LevelEvent_Text;

		public LevelEvent_SetText lastSetTextEvent
		{
			get
			{
				int num = -1;
				LevelEvent_SetText result = null;
				foreach (LevelEventControl_Base eventControl in base.editor.eventControls)
				{
					if (eventControl.levelEvent is LevelEvent_SetText { active: not false } levelEvent_SetText && !(levelEvent_SetText.target != target) && levelEvent_SetText.sortOrder <= base.sortOrder && levelEvent_SetText.sortOrder > num)
					{
						result = levelEvent_SetText;
						num = levelEvent_SetText.sortOrder;
					}
				}
				return result;
			}
		}

		public override void Run()
		{
			RunOnBeat(() =>
			{
				if (isCompatibleDecorationType && base.vfx.allDecorationLyrics.ContainsKey(target))
				{
					float? customFadeTime = (base.fadeOutDuration.HasValue ? new float?(base.conductor.DurationBeatsToTime(base.beat - 1f, base.fadeOutDuration.Value)) : ((float?)null));
					if (base.vfx.allDecorationLyrics.TryGetValue(target, out var value))
					{
						value.AdvanceText(customFadeTime);
					}
				}
			});
		}

		public override string GetTooltipText()
		{
			int num = -1;
			LevelEvent_SetText levelEvent_SetText = lastSetTextEvent;
			if (levelEvent_SetText != null)
			{
				num = levelEvent_SetText.sortOrder;
				int num2 = 1;
				foreach (LevelEventControl_Base eventControl in base.editor.eventControls)
				{
					if (eventControl.levelEvent is LevelEvent_AdvanceTextDecoration levelEvent_AdvanceTextDecoration && eventControl.levelEvent.active && !(levelEvent_AdvanceTextDecoration.target != target) && levelEvent_AdvanceTextDecoration.sortOrder < base.sortOrder && levelEvent_AdvanceTextDecoration.sortOrder > num)
					{
						num2++;
					}
				}
				string[] array = LyricsGame.SplitLyricsLines(levelEvent_SetText.GetLocalizedText(), useLinebreak: true);
				if (num2 < array.Length)
				{
					return array[num2].Trim();
				}
			}
			return null;
		}
	}
}
