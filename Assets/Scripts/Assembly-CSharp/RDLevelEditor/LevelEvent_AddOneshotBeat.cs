using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnPrebar, 0, RoomsUsage.NotUsed, true, true, true, false, 0, false)]
	public class LevelEvent_AddOneshotBeat : LevelEvent_Base
	{
		private const int NotUsingSubdivisions = 0;

		private SoundData soundData;

		public float boomOffset;

		private float _absoluteClapPos;

		public List<AudioSource> prebarAudioSrcs = new List<AudioSource>();

		private const float floatingError = 0.001f;

		[JsonProperty("", "", null, "", false, true, null, null)]
		public OneshotPulseType pulseType { get; set; }

		[JsonProperty("", "", null, "", false, true, "EnableFreezeBurnModeIf", null)]
		public FreezeBurnMode freezeBurnMode { get; set; }

		[JsonProperty("", "", null, "", false, true, "EnableIntervalIf", null)]
		public float interval { get; set; } = 2f;

		[JsonProperty("", "", null, "", false, true, null, null)]
		public float tick { get; set; } = 1f;

		[JsonProperty("", "", null, "", false, true, "EnableDelayIf", null)]
		public float delay { get; set; }

		[JsonProperty("", "", null, "", false, true, "EnableLoopIf", null)]
		public int loops { get; set; }

		[JsonProperty("", "", null, "", false, true, "EnableSubdivisionsIf", null)]
		public int subdivisions { get; set; }

		[JsonProperty("", "", null, "", false, true, "EnableSubdivisionsIf", null)]
		public bool subdivSound { get; set; } = true;

		[JsonProperty("", "", null, "", false, true, "EnableSkipshotIf", null)]
		public bool skipshot { get; set; }

		[JsonProperty("", "", null, "", false, true, "EnableHoldIf", null)]
		public bool hold { get; set; }

		[JsonProperty("", "", null, "", false, true, "EnableHoldIf", null)]
		public HoldCueType holdCue { get; set; }

		[JsonProperty("", "", null, "", false, true, "EnableSubdivTickOverrideIf", null)]
		public float subdivTickOverride { get; set; }

		[JsonProperty("", "", null, "", false, true, null, null)]
		[Sound(true, true, false, false, "GetBeatSounds", true, 16)]
		[Off]
		public SoundDataStruct? sound { get; set; } = new SoundDataStruct("Shaker");

		public float absoluteClapPos
		{
			get
			{
				if (_absoluteClapPos == 0f)
				{
					_absoluteClapPos = BarAndBeatToAbsoluteBeat(new BarAndBeat(base.bar, base.beat + clapPosition));
				}
				return _absoluteClapPos;
			}
		}

		public float actualDelay
		{
			get
			{
				if (freezeBurnMode != FreezeBurnMode.Freezeshot)
				{
					return 0f;
				}
				return delay;
			}
		}

		private float clapPosition => interval * (float)loops + boomOffset + tick + actualDelay;

		public static string[] GetBeatSounds()
		{
			return RDEditorConstants.BeatSounds.Select((SoundEffect s) => s.ToString()).ToArray();
		}

		public bool EnableFreezeBurnModeIf()
		{
			return freezeBurnMode != FreezeBurnMode.None;
		}

		public bool EnableIntervalIf()
		{
			if (interval > 0f)
			{
				if (loops <= 0 && freezeBurnMode == FreezeBurnMode.None && !skipshot)
				{
					return hold;
				}
				return true;
			}
			return false;
		}

		public bool EnableDelayIf()
		{
			return freezeBurnMode == FreezeBurnMode.Freezeshot;
		}

		public bool EnableLoopIf()
		{
			return loops > 0;
		}

		public bool EnableSubdivisionsIf()
		{
			if (pulseType != OneshotPulseType.Square)
			{
				return pulseType == OneshotPulseType.Triangle;
			}
			return true;
		}

		public bool EnableSubdivTickOverrideIf()
		{
			return subdivTickOverride > 0f;
		}

		public bool EnableSkipshotIf()
		{
			return skipshot;
		}

		public bool EnableHoldIf()
		{
			return hold;
		}

		public override IEnumerator Prepare()
		{
			Validate();
			float b = BarAndBeatToAbsoluteBeat(barAndBeat);
			bool flag = freezeBurnMode == FreezeBurnMode.Freezeshot;
			bool flag2 = freezeBurnMode == FreezeBurnMode.Burnshot;
			float freezeBurnOffset = ((flag || (hold && holdCue != HoldCueType.Late)) ? (interval - tick) : (flag2 ? interval : 0f));
			b = Mathf.Max(b - freezeBurnOffset, 0f);
			barAndBeat = Timeline.GetBarAndBeatWithColumn(b, LevelEvent_Base.level.cpbEvents);
			boomOffset = freezeBurnOffset;
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
			for (int i = 0; i < loops; i++)
			{
				BarAndBeat barAndBeatWithColumn = Timeline.GetBarAndBeatWithColumn(b + interval * (float)(i + 1), LevelEvent_Base.level.cpbEvents);
				LevelEvent_AddOneshotBeat item = new LevelEvent_AddOneshotBeat
				{
					prepared = true,
					barAndBeat = barAndBeatWithColumn,
					tick = tick,
					pulseType = pulseType,
					subdivisions = subdivisions,
					executionTime = LevelEventExecutionTime.OnPrebar,
					row = base.row,
					delay = delay,
					freezeBurnMode = freezeBurnMode,
					interval = interval,
					sortOrderOffset = sortOrderOffset,
					subdivSound = subdivSound,
					boomOffset = freezeBurnOffset,
					hold = hold
				};
				LevelEvent_Base.level.dynamicallyAddedEvents.Add(item);
			}
			prepared = true;
			yield return null;
		}

		public void Validate()
		{
			bool num = freezeBurnMode == FreezeBurnMode.Freezeshot;
			_ = freezeBurnMode;
			loops = Mathf.Max(0, loops);
			if (num)
			{
				if (delay <= 0f)
				{
					delay = 0.5f;
				}
			}
			else
			{
				delay = 0f;
			}
			tick = Mathf.Max(0f, tick);
			if (freezeBurnMode != FreezeBurnMode.None || loops > 0 || skipshot || hold)
			{
				interval = Mathf.Max(0f, interval);
			}
			else
			{
				interval = 2f * tick;
			}
			if (pulseType == OneshotPulseType.Square)
			{
				subdivisions = 1;
			}
			else if (pulseType == OneshotPulseType.Triangle)
			{
				subdivisions = Mathf.Clamp(subdivisions, 1, 10);
			}
			else
			{
				subdivisions = 0;
			}
			if (subdivisions <= 1 || freezeBurnMode != FreezeBurnMode.Burnshot)
			{
				subdivTickOverride = 0f;
			}
		}

		public override string Encode()
		{
			Validate();
			return base.Encode();
		}

		public override void Decode(Dictionary<string, object> dict)
		{
			if (dict.ContainsKey("squareSound"))
			{
				dict["subdivSound"] = dict["squareSound"];
			}
			base.Decode(dict);
			if (freezeBurnMode == FreezeBurnMode.Burnshot && delay > 0f)
			{
				tick -= delay;
				delay = 0f;
			}
			else if (freezeBurnMode == FreezeBurnMode.None && delay > 0f)
			{
				freezeBurnMode = FreezeBurnMode.Freezeshot;
				interval -= delay;
				base.beat += interval - tick;
			}
			if (RDLevelData.current.settings.version < 59 && freezeBurnMode == FreezeBurnMode.None && loops == 0 && skipshot && interval == 2f)
			{
				interval = tick * 2f;
			}
			Validate();
		}

		public override void RunPrebar()
		{
			if (LevelEvent_Base.level.data.rows[base.row].muteBeats)
			{
				return;
			}
			Row targetRow = base.game.rows[base.row];
			Row[] rows;
			if (skipshot)
			{
				rows = base.game.rows;
				foreach (Row row in rows)
				{
					if (LevelEvent_Base.level.RowGetsBeat(base.row, row.id, tag))
					{
						PlaySkipshotCue(row);
					}
				}
			}
			if (pulseType != OneshotPulseType.Square && pulseType != OneshotPulseType.Triangle)
			{
				return;
			}
			rows = base.game.rows;
			foreach (Row row2 in rows)
			{
				if (LevelEvent_Base.level.RowGetsBeat(base.row, row2.id, tag) && row2.rowCountingVoiceEnabled)
				{
					float num = GetCountOffset();
					float num2 = base.beat - 1f - num;
					if (num2 < 0f && num2 >= -1f)
					{
						num2 = 0f;
					}
					scrExecuteOnCertainBeat.Add(() =>
					{
						row2.ent.targetCharacterRowEnt.subdivisionSpotlightController.DoSubdiv(base.bar, base.beat, subdivisions);
					}, num2);
					PlayCountingSound(num, row2);
				}
			}
			float GetCountOffset()
			{
				float num3 = ((freezeBurnMode != FreezeBurnMode.Burnshot) ? tick : ((subdivTickOverride > 0f) ? subdivTickOverride : (interval / 2f)));
				float num4 = ((subdivisions == 1) ? 0f : (targetRow.rowCountingVoiceSubdivOffset * num3));
				if (freezeBurnMode == FreezeBurnMode.Freezeshot)
				{
					num4 -= interval - tick;
				}
				else if (freezeBurnMode == FreezeBurnMode.Burnshot)
				{
					num4 -= interval;
				}
				return num4;
			}
			void PlayCountingSound(float countOffset, Row row3)
			{
				SoundData soundData = row3.countingSounds[subdivisions - 1];
				float pan = ((row3.rowSoundPan == 0f && soundData.panPercentage != 0f) ? RDUtils.OverridePanFor2P(row3, soundData.panPercentage) : row3.rowSoundPanAdjustedFor2P);
				prebarAudioSrcs.RDAdd(base.conductor.PlayBeatInCrotchets(soundData.filename, base.beat - 1f - countOffset, soundData.volumePercentage, RDUtils.GetMixerGroup("RDGSVoice"), soundData.pitchPercentage, pan));
			}
			void PlaySkipshotCue(Row row3)
			{
				float num3 = ((subdivisions > 1) ? (tick * (1f - 1f / (float)subdivisions)) : 0f);
				RDGameSoundData rDGameSoundData = RDGameSounds.Get(GameSoundType.Skipshot);
				float pan = ((row3.rowSoundPan != 0f || rDGameSoundData.pan == 0f) ? row3.rowSoundPanAdjustedFor2P : RDUtils.OverridePanFor2P(row3, rDGameSoundData.pan));
				prebarAudioSrcs.Add(base.conductor.PlayBeatInCrotchets(rDGameSoundData.filename, base.beat - 1f + clapPosition + num3, row3.rowSoundVolMultiplier * rDGameSoundData.volume, RDUtils.GetMixerGroup("SpecialOneshotCues"), rDGameSoundData.maxPitch, pan));
			}
		}

		public override void Run()
		{
			RunOnBeat(() =>
			{
				Row[] rows = base.game.rows;
				for (int i = 0; i < rows.Length; i++)
				{
					if (LevelEvent_Base.level.RowGetsBeat(base.row, i, tag) && !LevelEvent_Base.level.data.rows[i].muted)
					{
						RowEntity rowEnt = base.game.rows[i].ent;
						string text = soundData?.filename ?? string.Empty;
						if (pulseType == OneshotPulseType.Square && subdivSound)
						{
							text = "sndSquareshot";
						}
						bool flag = pulseType == OneshotPulseType.Triangle;
						bool flag2 = freezeBurnMode == FreezeBurnMode.Burnshot;
						float num = 0f;
						if (holdCue == HoldCueType.Early || (holdCue == HoldCueType.Auto && base.conductor.DurationBeatsToTime(base.beat - 1f, tick) <= 0.666f))
						{
							num = (flag2 ? interval : (interval - tick));
						}
						if (flag)
						{
							float num2 = base.beat - 1f;
							float num3 = tick;
							if (flag2)
							{
								num3 = ((!(subdivTickOverride > 0f)) ? (interval / 2f) : subdivTickOverride);
							}
							float num4 = num3 / (float)subdivisions;
							num += num4 * (float)(subdivisions - 1);
							float visualFreezeBeatOverride = num2 + tick;
							List<BeatOneshot> list = new List<BeatOneshot>();
							List<BeatOneshot> list2 = new List<BeatOneshot>();
							for (int j = 0; j < subdivisions; j++)
							{
								bool flag3 = j == subdivisions - 1;
								float holdDuration = ((hold & flag3) ? (interval - (tick + delay + num4 * (float)j)) : 0f);
								float num5 = num2 + num4 * (float)j;
								string customHitSound = (subdivSound ? $"sndTriangleshot{j + 1}" : text);
								(BeatOneshot, BeatOneshot) tuple = LevelEvent_Base.level.AddBeatOneshot(i, num5, tick, pulseType, customHitSound, actualDelay, boomOffset, visualFreezeBeatOverride, subdivisions, j, flag2, holdDuration, num);
								if (tuple.Item1 != null)
								{
									list.Add(tuple.Item1);
									tuple.Item1.subdivBuddies = list;
									if (tuple.Item2 != null)
									{
										list2.Add(tuple.Item2);
										tuple.Item2.subdivBuddies = list2;
									}
									int subdivIndex = j;
									scrExecuteOnCertainBeat.Add(() =>
									{
										rowEnt.targetCharacterRowEnt.subdivisionSpotlightController.AdvanceSubdiv(base.bar, base.beat, subdivIndex);
									}, num5 + actualDelay + tick + boomOffset);
								}
							}
						}
						else
						{
							float holdDuration2 = (hold ? (interval - (tick + delay)) : 0f);
							LevelEvent_Base.level.AddBeatOneshot(i, base.beat - 1f, tick, pulseType, text, actualDelay, boomOffset, 0f, 1, 0, flag2, holdDuration2, num);
							if (pulseType == OneshotPulseType.Square)
							{
								scrExecuteOnCertainBeat.Add(() =>
								{
									rowEnt.targetCharacterRowEnt.subdivisionSpotlightController.AdvanceSubdiv(base.bar, base.beat, 0);
								}, actualDelay + tick + boomOffset);
							}
						}
						if (skipshot)
						{
							float subdivOffset = (flag ? (tick * (1f - 1f / (float)subdivisions)) : 0f);
							scrExecuteOnCertainBeat.Add(() =>
							{
								float duration = base.conductor.DurationBeatsToTime(base.beat, interval - subdivOffset);
								rowEnt.oneshotRowController.PlaySkipshotAnimation(duration);
							}, base.beat - 1f + clapPosition + subdivOffset);
						}
					}
				}
			});
		}

		public void CapXPosForPositiveFreezeBurnCueTime()
		{
			if (freezeBurnMode != FreezeBurnMode.None)
			{
				float num = ((freezeBurnMode == FreezeBurnMode.Freezeshot) ? (interval - tick) : interval);
				Timeline timeline = base.editor.timeline;
				float posXFromBarAndBeat = timeline.GetPosXFromBarAndBeat(barAndBeat);
				barAndBeat = timeline.GetBarAndBeatWithPosX(posXFromBarAndBeat, null, num * (float)timeline.cellWidth);
			}
		}

		public override string CustomControlName()
		{
			return "AddOneshotBeat";
		}
	}
}
