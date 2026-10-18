using System;
using System.Collections;
using System.Collections.Generic;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnPrebar, -1, RoomsUsage.NotUsed, true, true, true, false, 0, true)]
	public class LevelEvent_SetCountingSound : LevelEvent_Base
	{
		private SoundData[] soundsData;

		[JsonProperty("", "", null, "", false, true, null, null)]
		[ToggleGroup(false, null, null, null, false, 14)]
		public bool enabled { get; set; } = true;

		[JsonProperty("", "", null, "", false, true, "IfEnabled", null)]
		[Dropdown(null, false, true)]
		public CountingVoiceSource voiceSource { get; set; } = CountingVoiceSource.JyiCount;

		[JsonProperty("", "", null, "", false, true, "IfEnabled", null)]
		[Slider("%", null, 0f, true, false)]
		[IntInfo(0, 200)]
		public int volume { get; set; } = 100;

		[JsonProperty("", "", null, "", false, true, "EnableSubdivOffsetIf", null)]
		[Slider("", null, 8f, true, false)]
		public float subdivOffset { get; set; } = 0.5f;

		[JsonProperty("", "", null, "", false, true, "EnableSoundsIf", null)]
		[SetCountingSound]
		public SoundDataStruct[] sounds { get; set; } = Array.Empty<SoundDataStruct>();

		public bool IfEnabled()
		{
			return enabled;
		}

		public bool EnableSubdivOffsetIf()
		{
			if (IfEnabled() && base.editor.rowsData.Count > 0)
			{
				return base.editor.rowsData[base.row].rowType == RowType.Oneshot;
			}
			return false;
		}

		public bool EnableSoundsIf()
		{
			if (IfEnabled())
			{
				return voiceSource == CountingVoiceSource.Custom;
			}
			return false;
		}

		public override IEnumerator Prepare()
		{
			if (voiceSource != CountingVoiceSource.Custom)
			{
				prepared = true;
				yield break;
			}
			soundsData = new SoundData[sounds.Length];
			for (int i = 0; i < sounds.Length; i++)
			{
				SoundDataStruct soundDataStruct = sounds[i];
				SoundData soundData = soundDataStruct.WithNewVolume(soundDataStruct.volume * volume / 100).Validated().ToSoundData();
				yield return soundData.Prepare();
				if (soundData.externalClip || soundData.filename != "None")
				{
					soundData.filename = soundData.conductorFilename;
				}
				soundsData[i] = soundData;
			}
			prepared = true;
		}

		public override void Run()
		{
			Row row = LevelEvent_Base.level.rows[base.row];
			row.rowCountingVoiceEnabled = enabled;
			ValidateVoiceSource(row.rowType == RowType.Oneshot);
			if (voiceSource == CountingVoiceSource.Custom)
			{
				row.countingSounds = soundsData;
			}
			else
			{
				Array.Resize(ref row.countingSounds, (row.rowType == RowType.Oneshot) ? 10 : 7);
				row.SetCountingSounds(voiceSource, (float)volume / 100f);
			}
			row.rowCountingVoiceSubdivOffset = subdivOffset;
		}

		public void ValidateVoiceSource(bool isOneshot)
		{
			if ((voiceSource == CountingVoiceSource.JyiCount) & isOneshot)
			{
				voiceSource = CountingVoiceSource.JyiCountEnglish;
			}
		}

		public override string GetTooltipText()
		{
			if (base.editor.rowsData == null || base.editor.rowsData.Count == 0 || base.row >= base.editor.rowsData.Count)
			{
				return string.Empty;
			}
			LevelEvent_MakeRow levelEvent_MakeRow = base.editor.rowsData[base.row];
			string rowString = levelEvent_MakeRow.GetRowString();
			ValidateVoiceSource(levelEvent_MakeRow.rowType == RowType.Oneshot);
			List<string> list = new List<string> { rowString };
			if (enabled)
			{
				list.Add(RDString.GetEnumValue(voiceSource));
			}
			else
			{
				list.Add(RDString.Get("editor.SetCountingSound.off"));
			}
			list.Add(volume + "%");
			return string.Join('\n', list);
		}
	}
}
