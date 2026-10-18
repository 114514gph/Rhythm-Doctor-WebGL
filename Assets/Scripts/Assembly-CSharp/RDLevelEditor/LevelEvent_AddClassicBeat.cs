using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnPrebar, 0, RoomsUsage.NotUsed, true, true, true, false, 0, false)]
	public class LevelEvent_AddClassicBeat : LevelEvent_Base
	{
		private SoundData soundData;

		public LevelEvent_SetRowXs setRowXs;

		[JsonProperty("", "", null, "", false, true, null, null)]
		[FloatInfo(0f, float.PositiveInfinity)]
		public float tick { get; set; } = 1f;

		[JsonProperty("", "", null, "", false, true, "EnableSwingIf", null)]
		[Slider("", null, 8f, false, false)]
		[FloatInfo(0f, float.PositiveInfinity)]
		public float swing { get; set; }

		[JsonProperty("", "", null, "", false, true, null, null)]
		[InputField("beats", InputField.LineType.SingleLine, 40, 14, false, false)]
		public float hold { get; set; }

		[JsonProperty("", "", null, "", false, true, "EnableXsIf", null)]
		[ToggleGroup(false, null, null, null, false, 14)]
		public SetXs setXs { get; set; }

		[JsonProperty("", "", null, "", false, true, "EnableLegacyIf", null)]
		[DontShow]
		public bool legacy { get; set; }

		[JsonProperty("", "", null, "", false, true, "EnableLengthIf", null)]
		[IntInfo(1, 7)]
		[Off]
		public int? length { get; set; } = 7;

		[JsonProperty("", "", "editor.MakeRow.pulseSound", "", false, true, null, null)]
		[Sound(true, true, false, false, "GetBeatSounds", true, 16)]
		[Off]
		public SoundDataStruct? sound { get; set; } = new SoundDataStruct("Shaker");

		[JsonProperty("", "", null, "", false, true, "EnableSwitchIf", null)]
		[Button("SwitchControlToSetRowXs", "editor.AddClassicBeat.switchToSetRowXs")]
		public bool switchToSetRowXs => false;

		[JsonProperty("", "", null, "", false, true, null, null)]
		[Button("BreakIntoFreeTimeBeats", "editor.AddClassicBeat.breakIntoFreeTime")]
		public bool breakIntoFreeTime => false;

		public static string[] GetBeatSounds()
		{
			return RDEditorConstants.BeatSounds.Select((SoundEffect s) => s.ToString()).ToArray();
		}

		public bool EnableXsIf()
		{
			if (hold != 0f)
			{
				int? num = length;
				return !num.HasValue || num == 7;
			}
			return false;
		}

		public bool EnableLegacyIf()
		{
			return legacy;
		}

		public bool EnableSwitchIf()
		{
			return !EnableXsIf();
		}

		public bool EnableSwingIf()
		{
			return hold == 0f;
		}

		public static bool EnableLengthIf()
		{
			return RDBase.isAdvanced;
		}

		public override void Decode(Dictionary<string, object> dict)
		{
			if (!dict.ContainsKey("legacy"))
			{
				legacy = RDLevelData.current.settings.version < 29 && dict.ContainsKey("swing") && Convert.ToSingle(dict["swing"]) != 0f;
			}
			base.Decode(dict);
			Validate();
		}

		public override IEnumerator Prepare()
		{
			Validate();
			if (hold != 0f && setXs != SetXs.NoChange)
			{
				setRowXs = new LevelEvent_SetRowXs();
				setRowXs.Init();
				setRowXs.bar = base.bar;
				setRowXs.beat = base.beat;
				setRowXs.sortOrderOffset = -1;
				setRowXs.row = base.row;
				setRowXs.pattern = ((setXs == SetXs.FourBeat) ? "-x-x-x" : "-xx-xx");
				setRowXs.prepared = true;
				if (!LevelEvent_Base.level.loadedHeldBeatSounds)
				{
					string folderPrefix = "Sfx/Beatsounds/";
					string folderPrefix2 = "Sfx/ClapsAndExplosions/";
					PrepareHoldSound(GameSoundType.PulseSoundHoldStart, folderPrefix);
					PrepareHoldSound(GameSoundType.PulseSoundHoldStartAlt, folderPrefix);
					PrepareHoldSound(GameSoundType.ClapSoundHoldLongStart, folderPrefix2);
					if (GC.twoPlayerMode)
					{
						PrepareHoldSound(GameSoundType.PulseSoundHoldStartP2, folderPrefix);
						PrepareHoldSound(GameSoundType.PulseSoundHoldStartAltP2, folderPrefix);
						PrepareHoldSound(GameSoundType.ClapSoundHoldLongStartP2, folderPrefix2);
					}
					LevelEvent_Base.level.loadedHeldBeatSounds = true;
				}
			}
			SoundDataStruct? soundDataStruct = sound;
			if (soundDataStruct.HasValue)
			{
				soundData = soundDataStruct.GetValueOrDefault().ToSoundData();
				yield return soundData.Prepare();
				if (soundData.externalClip || soundData.filename != "None")
				{
					soundData.filename = soundData.conductorFilename;
				}
			}
			else
			{
				soundData = null;
			}
			prepared = true;
			yield return null;
			static void PrepareHoldSound(GameSoundType soundType, string text)
			{
				string clipName = text + RDGameSounds.Get(soundType).filename;
				Singleton<AudioManager>.Instance.FindOrLoadAudioClip(clipName);
			}
		}

		public void Validate()
		{
			tick = Mathf.Max(0f, tick);
			swing = Mathf.Clamp(swing, 0f, tick * 2f);
		}

		public int GetLength()
		{
			return length ?? 7;
		}

		public override void Run()
		{
			RunOnBeat(() =>
			{
				Row[] rows = base.game.rows;
				for (int i = 0; i < rows.Length; i++)
				{
					_ = rows[i];
					if (LevelEvent_Base.level.RowGetsBeat(base.row, i, tag) && !LevelEvent_Base.level.data.rows[i].muted)
					{
						if (base.row != i)
						{
							soundData = null;
						}
						int num = GetLength();
						if (hold != 0f)
						{
							base.game.AddBeatHold(i, base.beat - 1f, tick, hold, num, soundData);
						}
						else
						{
							SwingBeatType beattype = ((swing != 0f) ? (legacy ? SwingBeatType.SwingCustom : SwingBeatType.SwingCustomNew) : SwingBeatType.Normal);
							base.game.AddBeat(i, base.beat - 1f, tick, beattype, swing, 0f, num, soundData);
						}
					}
				}
			});
		}

		public void SwitchControlToSetRowXs()
		{
			using (new SaveStateScope())
			{
				base.inspectorPanel.LevelEditorPlaySound("sndEditorEventCreate", "LevelEditorActive");
				base.editor.SetLevelEventControlType(LevelEventType.SetRowXs, copyRow: true);
			}
		}

		public void BreakIntoFreeTimeBeats()
		{
			using (new SaveStateScope())
			{
				base.inspectorPanel.LevelEditorPlaySound("sndEditorEventBreak", "LevelEditorActive");
				int num = GetLength() - 1;
				int num2 = -1;
				float num3 = 0f;
				bool flag = false;
				Timeline timeline = base.inspectorPanel.timeline;
				LevelEvent_SetRowXs closestSetBeatModifiersEventWithRow = timeline.GetClosestSetBeatModifiersEventWithRow(base.row, barAndBeat);
				if (closestSetBeatModifiersEventWithRow != null)
				{
					num2 = closestSetBeatModifiersEventWithRow.syncoBeat;
					flag = num2 != -1 && num2 < num;
					if (flag)
					{
						float syncoSwing = closestSetBeatModifiersEventWithRow.syncoSwing;
						num3 = ((!(syncoSwing > 0f)) ? (-0.5f) : (0f - syncoSwing));
					}
				}
				LevelEvent_AddFreeTimeBeat levelEvent_AddFreeTimeBeat = new LevelEvent_AddFreeTimeBeat();
				levelEvent_AddFreeTimeBeat.CopyBasePropertiesFrom(this, copyBarAndBeat: true, copyRow: true);
				levelEvent_AddFreeTimeBeat.pulse = 7 - GetLength();
				levelEvent_AddFreeTimeBeat.hold = hold;
				base.editor.CreateEventControl(levelEvent_AddFreeTimeBeat, Tab.Rows);
				for (int i = 1; i < GetLength(); i++)
				{
					if (!(hold > 0f) || setXs == SetXs.NoChange || i % ((setXs == SetXs.FourBeat) ? 2 : 3) == 0)
					{
						LevelEvent_PulseFreeTimeBeat levelEvent_PulseFreeTimeBeat = new LevelEvent_PulseFreeTimeBeat();
						levelEvent_PulseFreeTimeBeat.CopyBasePropertiesFrom(this, copyBarAndBeat: true, copyRow: true);
						LevelEventControl_Base levelEventControl_Base = base.editor.CreateEventControl(levelEvent_PulseFreeTimeBeat, Tab.Rows);
						levelEvent_PulseFreeTimeBeat.action = PulseAction.Increment;
						levelEvent_PulseFreeTimeBeat.hold = hold;
						if (hold > 0f && setXs != SetXs.NoChange)
						{
							levelEvent_PulseFreeTimeBeat.action = PulseAction.Custom;
							levelEvent_PulseFreeTimeBeat.customPulse = i;
						}
						double num4 = GetOffsetFromSwingNew(i, this);
						if (flag && i > num2)
						{
							num4 += (double)(num3 * tick);
						}
						levelEventControl_Base.beat = (float)((double)levelEventControl_Base.beat + num4);
						float posXFromBarAndBeat = timeline.GetPosXFromBarAndBeat(levelEventControl_Base.levelEvent.barAndBeat);
						levelEventControl_Base.levelEvent.barAndBeat = timeline.GetBarAndBeatWithPosX(posXFromBarAndBeat);
						levelEventControl_Base.UpdateUI();
					}
				}
				if (hold > 0f && setXs != SetXs.NoChange)
				{
					LevelEvent_SetRowXs levelEvent_SetRowXs = new LevelEvent_SetRowXs();
					levelEvent_SetRowXs.CopyBasePropertiesFrom(this, copyBarAndBeat: true, copyRow: true);
					levelEvent_SetRowXs.pattern = ((setXs == SetXs.FourBeat) ? "-x-x-x" : "-xx-xx");
					base.editor.CreateEventControl(levelEvent_SetRowXs, Tab.Rows);
				}
				base.editor.DeleteEventControl(base.inspectorPanel.currentEventControl, selectControlToTheLeft: false);
			}
		}

		public static double GetOffsetFromSwing(int pulseIndex, LevelEvent_AddClassicBeat data)
		{
			if (data.swing != 0f && Mathf.RoundToInt((data.beat - 1f) / data.tick + (float)pulseIndex) % 2 == 1)
			{
				return data.tick * (float)(pulseIndex + 1) - data.swing;
			}
			return Math.Round((double)data.tick * (double)pulseIndex * 10000.0) / 10000.0;
		}

		public static double GetOffsetFromSwingNew(int pulseIndex, LevelEvent_AddClassicBeat data)
		{
			if (data.swing != 0f && pulseIndex % 2 == 1 && data.hold == 0f)
			{
				return data.tick * (float)(pulseIndex + 1) - data.swing;
			}
			return Math.Round((double)data.tick * (double)pulseIndex * 10000.0) / 10000.0;
		}
	}
}
