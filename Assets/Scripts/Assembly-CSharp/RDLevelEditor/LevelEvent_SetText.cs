using System;
using DG.Tweening;
using RDTools;
using UnityEngine;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnBar, 0, RoomsUsage.NotUsed, true, true, true, true, -10, false)]
	public class LevelEvent_SetText : LevelEvent_TextAttributesBase, ICompatibleDecorationTypeChecker
	{
		public bool isCompatibleDecorationType => base.ownerDecoration is LevelEvent_Text;

		public override void Run()
		{
			RunOnBeat(() =>
			{
				if (isCompatibleDecorationType)
				{
					LyricsGame lyricsGame = base.vfx.allDecorationLyrics[target];
					float fadeOutSeconds = base.conductor.DurationBeatsToTime(base.beat - 1f, base.fadeOutDuration);
					Vector2 anchoredPosition = lyricsGame.anchoredPosition;
					Float2? @float = base.textPosition;
					if (@float.HasValue)
					{
						Float2 valueOrDefault = @float.GetValueOrDefault();
						if (valueOrDefault.xUsed)
						{
							lyricsGame.positionTweenX?.Kill();
							anchoredPosition.x = Mathf.Floor(base.vfx.RDWidth * (valueOrDefault.x / 100f - 0.5f)) - 0.5f;
						}
						if (valueOrDefault.yUsed)
						{
							lyricsGame.positionTweenY?.Kill();
							anchoredPosition.y = Mathf.Floor(base.vfx.RDHeight * (valueOrDefault.y / 100f - 0.5f)) - 0.5f;
						}
					}
					float num = lyricsGame.angle;
					float? num2 = base.angle;
					if (num2.HasValue)
					{
						float valueOrDefault2 = num2.GetValueOrDefault();
						lyricsGame.angleTween?.Kill();
						num = valueOrDefault2;
					}
					int num3 = base.ownerDecoration.room;
					bool followCam = !base.game.rooms[num3].peekWindowMode;
					lyricsGame.Reset();
					lyricsGame.ResetSnapshots();
					lyricsGame.SetLyrics(base.text, anchoredPosition, base.size, num, base.color.ToColor(), num3, fadeOutSeconds, base.mode, base.outlineColor.ToColor(), base.anchor, followCam);
					lyricsGame.SetRenderQueueForTextDecoration();
					lyricsGame.AdvanceText();
					if (Narration.CheckCategoryEnabled(base.narrationCategory) && !base.conductor.isScrubbing && (base.narrate || (Persistence.GetForceNarrateCustomLevels() && scnGame.levelToLoadSource == LevelSource.ExternalPath)))
					{
						string text = string.Join("", base.splitLyrics).RemoveRichTextColorTags();
						try
						{
							text = base.game.currentLevel.EvaluateCurlyBracketsInString(text);
						}
						catch (Exception ex)
						{
							RDClassDll.printem("Using curly brackets but is not a variable: " + ex.Message);
						}
						Narration.Say(text, base.narrationCategory, false);
					}
				}
			});
		}
	}
}
