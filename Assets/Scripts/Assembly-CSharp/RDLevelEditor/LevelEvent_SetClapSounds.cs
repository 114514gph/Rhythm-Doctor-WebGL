using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnPrebar, -1, RoomsUsage.NotUsed, true, true, true, false, -10, false)]
	public class LevelEvent_SetClapSounds : LevelEvent_Base
	{
		[MaybeNull]
		private SoundData p1SoundData;

		[MaybeNull]
		private SoundData p2SoundData;

		[MaybeNull]
		private SoundData cpuSoundData;

		public static readonly SoundDataStruct p1SoundDefault = new SoundDataStruct(SoundEffect.ClapHit.ToString());

		public static readonly SoundDataStruct p2SoundDefault = new SoundDataStruct(SoundEffect.ClapHitP2.ToString());

		public static readonly SoundDataStruct cpuSoundDefault = new SoundDataStruct(SoundEffect.ClapHitCPU.ToString());

		[JsonProperty("", "", null, "", false, true, null, null)]
		[ToggleGroup(false, null, null, new string[] { "Classic", "Oneshot" }, true, 14)]
		public RowType rowType { get; set; }

		[JsonProperty("", "", null, "", false, true, null, null)]
		[Sound(true, true, false, false, "GetClapSoundsP1", true, 16)]
		public SoundDataStruct? p1Sound { get; set; } = p1SoundDefault;

		[JsonProperty("", "", null, "", false, true, null, null)]
		[Sound(true, true, false, false, "GetClapSoundsP2", true, 16)]
		[Off]
		public SoundDataStruct? p2Sound { get; set; } = p2SoundDefault;

		[JsonProperty("", "", null, "", false, true, null, null)]
		[Sound(true, true, false, false, "GetClapSoundsCPU", true, 16)]
		[Off]
		public SoundDataStruct? cpuSound { get; set; } = cpuSoundDefault;

		public static string[] GetClapSoundsP1()
		{
			return (from x in new SoundEffect[3][]
				{
					RDEditorConstants.ClapSoundsP1,
					RDEditorConstants.ClapSoundsP2,
					RDEditorConstants.ClapSoundsCPU
				}.SelectMany((SoundEffect[] x) => x)
				select x.ToString()).ToArray();
		}

		public static string[] GetClapSoundsP2()
		{
			return (from x in new SoundEffect[3][]
				{
					RDEditorConstants.ClapSoundsP2,
					RDEditorConstants.ClapSoundsP1,
					RDEditorConstants.ClapSoundsCPU
				}.SelectMany((SoundEffect[] x) => x)
				select x.ToString()).ToArray();
		}

		public static string[] GetClapSoundsCPU()
		{
			return (from x in new SoundEffect[3][]
				{
					RDEditorConstants.ClapSoundsCPU,
					RDEditorConstants.ClapSoundsP1,
					RDEditorConstants.ClapSoundsP2
				}.SelectMany((SoundEffect[] x) => x)
				select x.ToString()).ToArray();
		}

		public override void Init()
		{
			base.Init();
		}

		public override IEnumerator Prepare()
		{
			p1SoundData = p1Sound?.ToSoundData();
			p2SoundData = p2Sound?.ToSoundData();
			cpuSoundData = cpuSound?.ToSoundData();
			yield return p1SoundData?.Prepare();
			yield return p2SoundData?.Prepare();
			yield return cpuSoundData?.Prepare();
			prepared = true;
		}

		public override void Decode(Dictionary<string, object> dict)
		{
			SoundDataStruct.MigrateLegacySound(dict, "p1Sound", "p1");
			SoundDataStruct.MigrateLegacySound(dict, "p2Sound", "p2");
			SoundDataStruct.MigrateLegacySound(dict, "cpuSound", "cpu");
			if (dict.ContainsKey("p1Sound") && dict["p1Sound"] == null)
			{
				dict.Remove("p1Sound");
			}
			if (dict.ContainsKey("p2Sound") && dict["p2Sound"] == null)
			{
				dict.Remove("p2Sound");
			}
			if (dict.ContainsKey("cpuSound") && dict["cpuSound"] == null)
			{
				dict.Remove("cpuSound");
			}
			base.Decode(dict);
			if (RDLevelData.current.settings.version <= 49)
			{
				cpuSound = p2Sound;
			}
			if (!(bool)dict.GetValueOrDefault("p1Used", true))
			{
				p1Sound = null;
			}
			if (!(bool)dict.GetValueOrDefault("p2Used", true))
			{
				p2Sound = null;
			}
			if (!(bool)dict.GetValueOrDefault("cpuUsed", true))
			{
				cpuSound = null;
			}
		}

		public override void Run()
		{
			SetPlayerHitSounds(rowType, p1SoundData, p2SoundData, cpuSoundData);
		}

		public void SetPlayerHitSounds(RowType rowType, SoundData p1Sound, SoundData p2Sound, SoundData cpuSound)
		{
			if (p1Sound != null)
			{
				string text = p1Sound.conductorFilename;
				if (text == p1SoundDefault.filename)
				{
					text = "snd" + p1SoundDefault.filename;
				}
				RDGameSounds.Set((rowType != RowType.Classic) ? GameSoundType.ClapSoundP1Oneshot : GameSoundType.ClapSoundP1Classic, text, p1Sound.volumePercentage, p1Sound.pitchPercentage, p1Sound.panPercentage);
			}
			if (p2Sound != null)
			{
				string text2 = p2Sound.conductorFilename;
				if (text2 == p2SoundDefault.filename)
				{
					text2 = "snd" + p2SoundDefault.filename;
				}
				RDGameSounds.Set((rowType == RowType.Classic) ? GameSoundType.ClapSoundP2Classic : GameSoundType.ClapSoundP2Oneshot, text2, p2Sound.volumePercentage, p2Sound.pitchPercentage, p2Sound.panPercentage);
			}
			if (cpuSound != null)
			{
				string text3 = cpuSound.conductorFilename;
				if (text3 == cpuSoundDefault.filename)
				{
					text3 = "snd" + cpuSoundDefault.filename;
				}
				RDGameSounds.Set((rowType == RowType.Classic) ? GameSoundType.ClapSoundCPUClassic : GameSoundType.ClapSoundCPUOneshot, text3, cpuSound.volumePercentage, cpuSound.pitchPercentage, cpuSound.panPercentage);
			}
		}

		public override string GetTooltipText()
		{
			List<string> list = new List<string> { RDString.GetEnumValue(rowType) };
			SoundDataStruct? soundDataStruct = p1Sound;
			if (soundDataStruct.HasValue)
			{
				SoundDataStruct valueOrDefault = soundDataStruct.GetValueOrDefault();
				list.Add(string.Format("{0} {1} ({2}%)", RDString.Get("editor.SetClapSounds.p1Sound"), valueOrDefault.LocalizedName(), valueOrDefault.volume));
			}
			soundDataStruct = p2Sound;
			if (soundDataStruct.HasValue)
			{
				SoundDataStruct valueOrDefault2 = soundDataStruct.GetValueOrDefault();
				list.Add(string.Format("{0} {1} ({2}%)", RDString.Get("editor.SetClapSounds.p2Sound"), valueOrDefault2.LocalizedName(), valueOrDefault2.volume));
			}
			soundDataStruct = cpuSound;
			if (soundDataStruct.HasValue)
			{
				SoundDataStruct valueOrDefault3 = soundDataStruct.GetValueOrDefault();
				list.Add(string.Format("{0} {1} ({2}%)", RDString.Get("editor.SetClapSounds.cpuSound"), valueOrDefault3.LocalizedName(), valueOrDefault3.volume));
			}
			return string.Join('\n', list);
		}
	}
}
