using System.Collections.Generic;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnBar, -1, RoomsUsage.NotUsed, true, true, true, false, 0, false)]
	public class LevelEvent_NarrateRowInfo : LevelEvent_Base
	{
		[JsonProperty("", "", null, "", false, true, null, null)]
		public NarrateInfoType infoType { get; set; }

		[JsonProperty("", "", null, "", false, true, null, null)]
		[Checkbox]
		public bool soundOnly { get; set; }

		[JsonProperty("", "", null, "", false, true, "EnableNarrateSkipBeatsIf", null)]
		[ToggleGroup(false, null, null, null, true, 14)]
		public NarrateSkipBeats narrateSkipBeats { get; set; }

		[JsonProperty("", "", null, "", false, true, "EnableSkipsUnstableIf", null)]
		[Checkbox]
		public bool skipsUnstable { get; set; }

		[JsonProperty("", "", null, "", false, true, "EnableCustomPatternIf", null)]
		[BeatModifiers(true, false, false, false)]
		public string customPattern { get; set; } = "------";

		[JsonProperty("", "", null, "", false, true, null, null)]
		[ToggleGroup(true, null, null, new string[] { "AutoDetect", "P1", "P2" }, true, 14)]
		public RDPlayer customPlayer { get; set; } = RDPlayer.AutoDetect;

		[JsonProperty("", "", null, "", false, true, "EnableLengthIf", null)]
		[IntInfo(1, 7)]
		[Off]
		public int? customRowLength { get; set; } = 7;

		public override bool hasTaggedSpecialMethod => true;

		public bool EnableNarrateSkipBeatsIf()
		{
			return infoType != NarrateInfoType.Disconnect;
		}

		public bool EnableSkipsUnstableIf()
		{
			if (EnableNarrateSkipBeatsIf())
			{
				return narrateSkipBeats != NarrateSkipBeats.Off;
			}
			return false;
		}

		public bool EnableCustomPatternIf()
		{
			if (EnableNarrateSkipBeatsIf())
			{
				return narrateSkipBeats == NarrateSkipBeats.Custom;
			}
			return false;
		}

		public bool EnableLengthIf()
		{
			return RDBase.isAdvanced;
		}

		public override void TaggedActionVariant()
		{
			if (!base.conductor.isScrubbing && Narration.IsEnabled)
			{
				Row row = LevelEvent_Base.level.rows[base.row];
				scrConductor.PlayImmediately(GetSoundName(), 1f, RDUtils.GetMixerGroup("CuesoundsParent"), 1f, row.rowSoundPanAdjustedFor2P);
				NarrateNow();
			}
		}

		public override void RunPrebar()
		{
			if (!base.conductor.isScrubbing && Narration.IsEnabled)
			{
				Row rowToNarrate = LevelEvent_Base.level.rows[base.row];
				RunOnBeat(() =>
				{
					base.conductor.PlayBeatInCrotchets(GetSoundName(), base.beat - 1f, 1f, RDUtils.GetMixerGroup("CuesoundsParent"), 1f, (customPlayer == RDPlayer.AutoDetect) ? rowToNarrate.rowSoundPanAdjustedFor2P : RDUtils.OverridePanFor2P(customPlayer, rowToNarrate.rowSoundPan));
				}, forcePrebar: true);
			}
		}

		public override void Run()
		{
			if (!base.conductor.isScrubbing && Narration.IsEnabled)
			{
				RunOnBeat(() =>
				{
					NarrateNow();
				});
			}
		}

		private void NarrateNow()
		{
			Narration.NarrateRowInfo(LevelEvent_Base.level.rows[base.row], infoType, soundOnly, narrateSkipBeats != NarrateSkipBeats.Off, (narrateSkipBeats == NarrateSkipBeats.Custom) ? customPattern : null, skipsUnstable, customPlayer, customRowLength);
		}

		private string GetSoundName()
		{
			string result = "sndError";
			switch (infoType)
			{
			case NarrateInfoType.Connect:
				result = "sndPatientConnect";
				break;
			case NarrateInfoType.Update:
				result = "sndPatientUpdate";
				break;
			case NarrateInfoType.Disconnect:
				result = "sndPatientDisconnect";
				break;
			case NarrateInfoType.Online:
				result = "sndOttoActivate";
				break;
			case NarrateInfoType.Offline:
				result = "sndOttoDeactivate";
				break;
			}
			return result;
		}

		public override string GetTooltipText()
		{
			if (base.editor.rowsData == null || base.editor.rowsData.Count == 0 || base.row >= base.editor.rowsData.Count)
			{
				return string.Empty;
			}
			string rowString = base.editor.rowsData[base.row].GetRowString();
			List<string> list = new List<string>
			{
				rowString,
				RDString.GetEnumValue(infoType)
			};
			if (soundOnly)
			{
				list.Add("(" + RDString.Get("editor.NarrateRowInfo.soundOnly") + ")");
			}
			if (customPlayer != RDPlayer.AutoDetect)
			{
				list.Add(RDString.Get("enum.RDPlayer." + customPlayer) ?? "");
			}
			return string.Join('\n', list);
		}
	}
}
