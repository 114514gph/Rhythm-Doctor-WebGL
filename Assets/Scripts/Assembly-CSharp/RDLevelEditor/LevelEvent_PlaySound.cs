using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnPrebar, -9, RoomsUsage.NotUsed, true, true, true, false, -10, false)]
	public class LevelEvent_PlaySound : LevelEvent_Base
	{
		private SoundData soundData;

		[JsonProperty("", "", null, "", false, true, null, null)]
		[Sound(true, true, false, false, "GetSounds", true, 16)]
		public SoundDataStruct sound { get; set; } = new SoundDataStruct("Shaker");

		[JsonProperty("", "", null, "", false, true, null, null)]
		[Dropdown(new string[] { "CueSound", "MusicSound", "BeatSound", "HitSound", "OtherSound" }, false, true)]
		public CustomSoundType customSoundType { get; set; } = CustomSoundType.CueSound;

		public override bool hasTaggedSpecialMethod => true;

		private float levelSpeed
		{
			get
			{
				if (customSoundType != CustomSoundType.MusicSound)
				{
					return 1f;
				}
				return RDTime.speed;
			}
		}

		public static string[] GetSounds()
		{
			return (from x in new SoundEffect[5][]
				{
					RDEditorConstants.BeatSounds,
					RDEditorConstants.ClapSoundsP1,
					RDEditorConstants.ClapSoundsP2,
					RDEditorConstants.ClapSoundsCPU,
					RDEditorConstants.PlaySounds
				}.SelectMany((SoundEffect[] x) => x)
				select x.ToString()).ToArray();
		}

		public bool EnableIsCustomIf()
		{
			return sound.filename.HasAudioFileExtension();
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
			prepared = true;
		}

		public override void TaggedActionVariant()
		{
			scrConductor.PlayImmediately(soundData.conductorFilename, soundData.volumePercentage, RDUtils.GetMixerGroup(GetMixerPath()), soundData.pitchPercentage * levelSpeed, soundData.panPercentage);
		}

		public override void Run()
		{
			RunOnBeat(() =>
			{
				float time = base.conductor.BeatToTime(base.beat - 1f);
				base.conductor.PlayBeat(soundData.conductorFilename, time, soundData.volumePercentage, RDUtils.GetMixerGroup(GetMixerPath()), soundData.pitchPercentage * levelSpeed, soundData.pitchPercentage * levelSpeed, soundData.panPercentage);
			});
		}

		public string GetMixerPath()
		{
			return customSoundType switch
			{
				CustomSoundType.MusicSound => "CustomMusicSound", 
				CustomSoundType.BeatSound => "CustomBeatSound", 
				CustomSoundType.HitSound => "CustomHitSound", 
				CustomSoundType.OtherSound => "CustomOtherSound", 
				CustomSoundType.CueSound => "CustomCueSound", 
				_ => "CustomCueSound", 
			};
		}

		public void PreviewSound()
		{
			sound.ToSoundData().PlayImmediately(GetMixerPath());
		}

		public override string GetTooltipText()
		{
			return sound.LocalizedName() + "\n" + sound.GetTooltip();
		}
	}
}
