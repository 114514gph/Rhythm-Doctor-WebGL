using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnPrebar, -1, RoomsUsage.NotUsed, true, true, true, false, 0, true)]
	public class LevelEvent_SetBeatSound : LevelEvent_Base
	{
		private SoundData soundData;

		[JsonProperty("", "", null, "", false, true, null, null)]
		[Sound(true, true, false, false, "GetBeatSounds", true, 16)]
		public SoundDataStruct sound { get; set; } = new SoundDataStruct("Shaker");

		[JsonProperty("", "", null, "", false, true, null, null)]
		[Button("CopyRowBeatSound", "editor.SetBeatSound.copyRowBeatSound")]
		public bool copyRowButton => false;

		public static string[] GetBeatSounds()
		{
			return RDEditorConstants.BeatSounds.Select((SoundEffect s) => s.ToString()).ToArray();
		}

		public void CopyRowBeatSound()
		{
			LevelEvent_MakeRow levelEvent_MakeRow = base.editor.rowsData[base.row];
			sound = SoundDataStruct.FromSoundData(levelEvent_MakeRow.pulseSound);
		}

		public override void Decode(Dictionary<string, object> dict)
		{
			SoundDataStruct.MigrateLegacySound(dict, "sound");
			base.Decode(dict);
		}

		public override IEnumerator Prepare()
		{
			soundData = sound.ToSoundData();
			yield return soundData.Prepare();
			if (soundData.externalClip || soundData.filename != "None")
			{
				soundData.filename = soundData.conductorFilename;
			}
			prepared = true;
		}

		public override void Run()
		{
			base.game.rows[base.row].SetPulseSounds(soundData);
		}

		public void PreviewSound()
		{
			sound.ToSoundData().PlayImmediately(MixerPath.BeatsoundsRows[base.row]);
		}

		public override string GetTooltipText()
		{
			if (base.editor.rowsData == null || base.editor.rowsData.Count == 0 || base.row >= base.editor.rowsData.Count)
			{
				return string.Empty;
			}
			string rowString = base.editor.rowsData[base.row].GetRowString();
			List<string> values = new List<string>
			{
				rowString,
				sound.LocalizedName(),
				sound.GetTooltip()
			};
			return string.Join('\n', values);
		}
	}
}
