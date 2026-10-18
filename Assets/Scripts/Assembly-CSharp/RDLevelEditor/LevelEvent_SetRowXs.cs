using System.Collections.Generic;
using UnityEngine;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnBar, -1, RoomsUsage.NotUsed, true, true, true, false, 0, true)]
	public class LevelEvent_SetRowXs : LevelEvent_Base
	{
		[JsonProperty("", "", null, "", false, true, null, null)]
		[BeatModifiers(true, true, true, true)]
		public string pattern { get; set; } = "------";

		[JsonProperty("", "", null, "", false, true, null, null)]
		[DontShow]
		public int syncoBeat { get; set; } = -1;

		[JsonProperty("", "", null, "", false, true, "EnableIfSynco", null)]
		[Slider("", null, 8f, false, false)]
		[FloatInfo(0f, 1f)]
		public float syncoSwing { get; set; }

		[JsonProperty("", "", null, "", false, true, "EnableIfSynco", null)]
		[Slider("%", null, 0f, true, false)]
		[IntInfo(0, 200)]
		public int syncoVolume { get; set; } = 70;

		[JsonProperty("", "", null, "", false, true, "EnableIfSynco", null)]
		[Slider("%", null, 0f, true, false)]
		[IntInfo(0, 200)]
		public int syncoPitch { get; set; } = 100;

		[JsonProperty("", "", null, "", false, true, "EnableIfSynco", null)]
		[ToggleGroup(false, null, null, null, false, 14)]
		public SyncoStyle syncoStyle { get; set; }

		[JsonProperty("", "", null, "", false, true, "EnableIfSyncoChirp", null)]
		[Checkbox]
		public bool syncoPlayModifierSound { get; set; } = true;

		[JsonProperty("", "", null, "", false, true, "EnableIfSyncoChirp", null)]
		[Checkbox]
		public bool syncoPlayModifierOffSound { get; set; } = true;

		public bool EnableIfSynco()
		{
			return syncoBeat >= 0;
		}

		public bool EnableIfSyncoChirp()
		{
			if (EnableIfSynco())
			{
				return syncoStyle == SyncoStyle.Chirp;
			}
			return false;
		}

		public bool EnableIfPlayModifierSound()
		{
			return syncoPlayModifierSound;
		}

		public override void Decode(Dictionary<string, object> dict)
		{
			if (RDLevelData.current.settings.version <= 57)
			{
				dict["syncoVolume"] = 0;
				dict["syncoPlayModifierSound"] = false;
				dict["syncoPlayModifierOffSound"] = false;
			}
			else if (RDLevelData.current.settings.version <= 63)
			{
				if (dict.ContainsKey("syncoPlayModifierSound"))
				{
					dict["syncoPlayModifierOffSound"] = dict["syncoPlayModifierSound"];
				}
				else
				{
					dict["syncoPlayModifierOffSound"] = false;
				}
			}
			base.Decode(dict);
		}

		public void Validate()
		{
			syncoSwing = Mathf.Clamp(syncoSwing, 0f, 1f);
		}

		public override void RunPrebar()
		{
			if (tag == null)
			{
				for (int i = 0; i < base.game.rows.Length; i++)
				{
					if (LevelEvent_Base.level.RowGetsBeat(base.row, i, tag))
					{
						Row row = base.game.rows[i];
						if (syncoBeat >= 0)
						{
							row.syncoVolume = (float)syncoVolume / 100f;
							row.syncoPitch = (float)syncoPitch / 100f;
						}
						SetModifiers(row);
					}
				}
			}
			if (syncoStyle != SyncoStyle.Chirp)
			{
				return;
			}
			RunOnBeat(() =>
			{
				Row row2 = base.game.rows[base.row];
				if (syncoPlayModifierSound && syncoBeat >= 0)
				{
					row2.syncoShouldPlayModifierOffSound = syncoPlayModifierOffSound;
					_PlaySound(row2, "sndSyncoModifierOn");
				}
				if (syncoBeat == -1 && row2.syncoShouldPlayModifierOffSound)
				{
					_PlaySound(row2, "sndSyncoModifierOff");
					row2.syncoShouldPlayModifierOffSound = false;
				}
			}, forcePrebar: true);
			void _PlaySound(Row prop, string sound)
			{
				float time = base.conductor.BeatToTime(base.beat - 1f);
				base.conductor.PlayBeat(sound, time, prop.syncoVolume * prop.rowSoundVolMultiplier, RDUtils.GetMixerGroup("CuesoundsParent"), prop.syncoPitch * prop.rowSoundMinPitch, prop.syncoPitch * prop.rowSoundMaxPitch, prop.rowSoundPanAdjustedFor2P);
			}
		}

		public override void Run()
		{
			for (int i = 0; i < base.game.rows.Length; i++)
			{
				if (!LevelEvent_Base.level.RowGetsBeat(base.row, i, tag))
				{
					continue;
				}
				Row prop = base.game.rows[i];
				bool[] patternArr = Row.ShowBeatsFromString(pattern);
				RunOnBeat(() =>
				{
					prop.SetSyncopationAesthetic(syncoBeat);
					prop.SetBeatSkipsAesthetic(patternArr);
					for (int j = 0; j < prop.beatboxes.Length; j++)
					{
						prop.beatboxes[j].pulseMode = pattern[j];
					}
					if (tag != null)
					{
						SetModifiers(prop);
					}
				});
			}
		}

		private void SetModifiers(Row prop)
		{
			prop.SetSyncopationCueStyle(syncoStyle);
			prop.SetSyncopation(syncoBeat, syncoSwing);
			prop.SetBeatSkips(Row.ShowBeatsFromString(pattern));
		}

		public override string CustomControlName()
		{
			return "SetRowXs";
		}
	}
}
