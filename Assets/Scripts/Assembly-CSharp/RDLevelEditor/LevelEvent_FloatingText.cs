using System;
using System.Collections.Generic;
using RDTools;
using UnityEngine;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnBar, 0, RoomsUsage.ManyRoomsAndOnTop, true, true, true, false, -10, false)]
	public class LevelEvent_FloatingText : LevelEvent_TextAttributesBase
	{
		private static List<int> allIds;

		public int[] currentIds;

		[JsonProperty("", "", null, "", false, true, null, null)]
		public int id { get; set; }

		[JsonProperty("", "", null, "", false, true, null, null)]
		public string font { get; set; } = TextFont.Default.ToString();

		[JsonProperty("", "", null, "", false, true, null, null)]
		public bool showChildren { get; set; } = true;

		public override void OnCreate()
		{
			base.OnCreate();
			GenerateNewID();
		}

		public void GenerateNewID()
		{
			id = -1;
			bool flag = false;
			do
			{
				id++;
				flag = false;
				foreach (LevelEventControl_Base eventControl in base.editor.eventControls)
				{
					if (eventControl.levelEvent is LevelEvent_FloatingText && (eventControl.levelEvent as LevelEvent_FloatingText).id == id && this != eventControl.levelEvent)
					{
						flag = true;
					}
				}
			}
			while (flag);
		}

		protected int GenerateNewTempId()
		{
			if (allIds == null)
			{
				allIds = new List<int>();
				foreach (LevelEvent_Base levelEvent in RDLevelData.current.levelEvents)
				{
					if (levelEvent is LevelEvent_FloatingText levelEvent_FloatingText)
					{
						allIds.Add(levelEvent_FloatingText.id);
					}
				}
			}
			foreach (int key in base.vfx.allVfxLyrics.Keys)
			{
				if (allIds.IndexOf(key) == -1)
				{
					allIds.Add(key);
				}
			}
			int result = -1;
			for (int i = 0; i < int.MaxValue; i++)
			{
				if (allIds.IndexOf(i) == -1)
				{
					result = i;
					break;
				}
			}
			return result;
		}

		public override void OnDelete()
		{
			using (new SaveStateScope())
			{
				List<LevelEventControl_Base> list = new List<LevelEventControl_Base>();
				foreach (LevelEventControl_Base eventControl in base.editor.eventControls)
				{
					if (eventControl.levelEvent is LevelEvent_AdvanceText && (eventControl.levelEvent as LevelEvent_AdvanceText).id == id)
					{
						list.Add(eventControl);
					}
				}
				base.editor.DeleteEventControlsFromList(list);
			}
		}

		public override void Run()
		{
			RunOnBeat(() =>
			{
				if (base.vfx.allVfxLyrics.ContainsKey(id))
				{
					int[] array = currentIds;
					foreach (int key in array)
					{
						LyricsGame lyricsGame = base.vfx.allVfxLyrics[key];
						lyricsGame.Reset();
						lyricsGame.AdvanceText();
					}
				}
				else
				{
					float fadeOutSeconds = base.conductor.DurationBeatsToTime(base.beat - 1f, base.fadeOutDuration);
					Vector2 zero = Vector2.zero;
					Float2? @float = base.textPosition;
					if (@float.HasValue)
					{
						Float2 valueOrDefault = @float.GetValueOrDefault();
						if (valueOrDefault.xUsed)
						{
							zero.x = valueOrDefault.x;
						}
						if (valueOrDefault.yUsed)
						{
							zero.y = valueOrDefault.y;
						}
					}
					Vector2 position = new Vector2(Mathf.Floor(base.vfx.RDWidth * (zero.x / 100f - 0.5f)) - 0.5f, Mathf.Floor(base.vfx.RDHeight * (zero.y / 100f - 0.5f)) - 0.5f);
					float num = 0f;
					float? num2 = base.angle;
					if (num2.HasValue)
					{
						float valueOrDefault2 = num2.GetValueOrDefault();
						num = valueOrDefault2;
					}
					currentIds = new int[rooms.Length];
					for (int j = 0; j < rooms.Length; j++)
					{
						int num3 = rooms[j];
						int num4 = ((j == 0) ? id : GenerateNewTempId());
						currentIds[j] = num4;
						bool followCam = num3 != -1 && !base.game.rooms[num3].peekWindowMode;
						LyricsGame.Create().Setup(base.text, font, position, base.size, num, base.color.ToColor(), num3, num4, fadeOutSeconds, base.mode, base.outlineColor.ToColor(), base.anchor, followCam, !tag.IsNullOrEmpty()).AdvanceText();
					}
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

		public override void CopyFromInternal(LevelEvent_Base levelEvent)
		{
			LevelEvent_FloatingText levelEvent_FloatingText = (LevelEvent_FloatingText)levelEvent;
			base.text = levelEvent_FloatingText.text;
			base.color = levelEvent_FloatingText.color;
			font = levelEvent_FloatingText.font;
			base.textPosition = levelEvent_FloatingText.textPosition;
			base.size = levelEvent_FloatingText.size;
			base.angle = levelEvent_FloatingText.angle;
			base.fadeOutDuration = levelEvent_FloatingText.fadeOutDuration;
			base.mode = levelEvent_FloatingText.mode;
			base.outlineColor = levelEvent_FloatingText.outlineColor;
			base.anchor = levelEvent_FloatingText.anchor;
			base.narrate = levelEvent_FloatingText.narrate;
			base.narrationCategory = levelEvent_FloatingText.narrationCategory;
			id = levelEvent_FloatingText.id;
		}
	}
}
