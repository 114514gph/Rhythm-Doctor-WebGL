using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnPrebar, -9, RoomsUsage.NotUsed, true, false, true, false, -10, false)]
	public class LevelEvent_PlaySong : LevelEvent_Base
	{
		private SoundData songSoundData;

		public bool forceOGGConvert;

		public string lastSong;

		[JsonProperty("", "", null, "", false, true, null, null)]
		[Sound(true, false, true, true, null, true, 16)]
		public SoundDataStruct song { get; set; } = new SoundDataStruct("sndOrientalTechno");

		[JsonProperty("bpm", "", null, "", false, true, null, null)]
		[BPMCalculator]
		[FloatInfo(1f, float.PositiveInfinity)]
		public float beatsPerMinute { get; set; } = 100f;

		[JsonProperty("", "", null, "", false, true, null, null)]
		[DontShow]
		public bool loop { get; set; }

		public override IEnumerator Prepare()
		{
			songSoundData = song.ToSoundData();
			yield return songSoundData.Prepare();
			prepared = true;
		}

		public override void Decode(Dictionary<string, object> dict)
		{
			SoundDataStruct.MigrateLegacySound(dict, "song");
			base.Decode(dict);
			int num = song.volume;
			if (RDLevelData.current.settings.version < 5)
			{
				Debug.Log("increasing volume value because version is < r5.");
				num += 30;
			}
			if (RDLevelData.current.settings.version < 13)
			{
				Debug.Log("increasing volume again because version is < r9a.");
				num += 10;
			}
			if (RDLevelData.current.settings.version < 42 && song.IsExternalClip())
			{
				Debug.Log("adjusting external file volumes because PlaySong volume formula changed");
				num = Mathf.RoundToInt((float)(num - 40) / 0.88f);
			}
			song = song.WithNewVolume(num);
		}

		public override void Run()
		{
			RunOnBeat(() =>
			{
				if (songSoundData.externalClip && LevelEvent_Base.level.preparedAudioClips[songSoundData.conductorFilename] == null)
				{
					Debug.LogWarning("Key for prepared AudioClip " + songSoundData.conductorFilename + " exists, but value is null");
				}
				base.conductor.SetBPM(beatsPerMinute);
				base.conductor.previousSong = base.conductor.currentSong;
				base.conductor.currentSong = base.conductor.PlaySong(songSoundData.conductorFilename, RDUtils.GetMixerGroup("LevelMusic"), songSoundData.pitchPercentage, songSoundData.panPercentage, loop, songSoundData.volumePercentage);
			});
		}

		public bool RequiresConversion()
		{
			if (!song.filename.IsNullOrEmpty() && song.IsExternalClip())
			{
				return Path.GetExtension(song.filename).ToLowerInvariant() != ".ogg";
			}
			return false;
		}

		public override string GetTooltipText()
		{
			return $"{song.filename}\n{beatsPerMinute} BPM\n{song.GetTooltip()}";
		}
	}
}
