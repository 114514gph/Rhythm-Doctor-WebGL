using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnPrebar, 0, RoomsUsage.NotUsed, true, true, true, false, 0, false)]
	public class LevelEvent_AddFreeTimeBeat : LevelEvent_Base
	{
		private const int LastPulse = 6;

		private const int DestroyPulse = -99;

		private const int BombBeat = -1;

		private float[] pulseBeats;

		private float[] pulseReleaseBeats;

		private int[] pulseIndexes;

		[JsonProperty("", "", null, "", false, true, null, null)]
		[PulsePicker]
		[IntInfo(-1, 6)]
		public int pulse { get; set; }

		[JsonProperty("", "", null, "", false, true, "EnableIfNotBomb", null)]
		public float hold { get; set; }

		[JsonProperty("", "", null, "", false, true, "EnableIfBomb", null)]
		[Description("editor.AddFreeTimeBeat.bombDescription", 32, 16, TextAnchor.MiddleCenter)]
		public bool description => false;

		public bool EnableIfBomb()
		{
			return pulse == -1;
		}

		public bool EnableIfNotBomb()
		{
			return pulse >= 0;
		}

		public float GetHoldLength()
		{
			if (!EnableIfBomb())
			{
				return hold;
			}
			return 0f;
		}

		public override IEnumerator Prepare()
		{
			yield return null;
			List<LevelEvent_Base> levelEvents = LevelEvent_Base.level.data.levelEvents;
			int num = pulse;
			List<LevelEvent_PulseFreeTimeBeat> list = new List<LevelEvent_PulseFreeTimeBeat>();
			List<int> list2 = new List<int> { num + 1 };
			if (num != 6 && num != -1)
			{
				foreach (LevelEvent_Base item3 in levelEvents)
				{
					if (item3.row != base.row || item3.sortOrder <= base.sortOrder || !(item3 is LevelEvent_PulseFreeTimeBeat))
					{
						continue;
					}
					LevelEvent_PulseFreeTimeBeat levelEvent_PulseFreeTimeBeat = item3 as LevelEvent_PulseFreeTimeBeat;
					list.Add(levelEvent_PulseFreeTimeBeat);
					if (levelEvent_PulseFreeTimeBeat.action == PulseAction.Remove)
					{
						list2.Add(-99);
					}
					else
					{
						bool flag = false;
						if (levelEvent_PulseFreeTimeBeat.action == PulseAction.Increment)
						{
							num++;
						}
						else if (levelEvent_PulseFreeTimeBeat.action == PulseAction.Decrement)
						{
							num--;
						}
						else
						{
							num = levelEvent_PulseFreeTimeBeat.customPulse;
							flag = num == -1;
						}
						if (!flag)
						{
							num = Math.Min(Math.Max(num, 0), 6);
						}
						list2.Add(num + 1);
					}
					if (num == 6 || num == -99 || num == -1)
					{
						break;
					}
				}
			}
			foreach (LevelEvent_Base item4 in levelEvents)
			{
				if (item4 is LevelEvent_SetBeatsPerMinute)
				{
					_ = ((LevelEvent_SetBeatsPerMinute)item4).beatsPerMinute;
				}
				if (item4 is LevelEvent_PlaySong)
				{
					_ = ((LevelEvent_PlaySong)item4).beatsPerMinute;
				}
				if (item4 == this)
				{
					break;
				}
			}
			int num2 = 8;
			foreach (LevelEvent_Base item5 in levelEvents)
			{
				if (item5 is LevelEvent_SetCrotchetsPerBar)
				{
					num2 = ((LevelEvent_SetCrotchetsPerBar)item5).crotchetsPerBar;
				}
				if (item5 == this)
				{
					break;
				}
			}
			List<float> list3 = new List<float>();
			List<float> list4 = new List<float>();
			list3.Add(base.beat - 1f);
			list4.Add(base.beat + GetHoldLength() - 1f);
			foreach (LevelEvent_PulseFreeTimeBeat item6 in list)
			{
				float item = (float)((item6.bar - base.bar) * num2) + (item6.beat - 1f);
				list3.Add(item);
				float num3 = ((item6.action == PulseAction.Remove) ? 0f : item6.hold);
				float item2 = (float)((item6.bar - base.bar) * num2) + (item6.beat + num3 - 1f);
				list4.Add(item2);
			}
			pulseBeats = list3.ToArray();
			pulseReleaseBeats = list4.ToArray();
			pulseIndexes = list2.ToArray();
			prepared = true;
		}

		public override void Run()
		{
			List<float> list = new List<float>();
			for (int i = 0; i < pulseBeats.Length; i++)
			{
				list.Add(base.conductor.BeatToTime(pulseBeats[i]));
			}
			float[] arrTimesFSAB = list.ToArray();
			List<float> list2 = new List<float>();
			for (int j = 0; j < pulseBeats.Length; j++)
			{
				list2.Add(base.conductor.BeatToTime(pulseReleaseBeats[j]));
			}
			float[] arrReleaseTimesFSAB = list2.ToArray();
			for (int k = 0; k < base.game.rows.Length; k++)
			{
				if ((k == base.row || (base.game.rows[k].isShadow && base.game.rows[k].copyBeatsWhenShadow && base.game.rows[k].shadowOrHostID == base.row)) && !LevelEvent_Base.level.data.rows[k].muted)
				{
					BeatClassic beatClassic = base.game.AddBeatFreeFlex(k, pulseIndexes, arrTimesFSAB, arrReleaseTimesFSAB);
					if (beatClassic == null)
					{
						break;
					}
					beatClassic.freeTimeEvent = this;
				}
			}
		}

		public override string CustomControlName()
		{
			return "FreeTimeBeat";
		}
	}
}
