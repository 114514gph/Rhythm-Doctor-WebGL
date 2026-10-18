using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RDLevelEditor
{
	[LevelEventInfo(LevelEventExecutionTime.OnBar, -1, RoomsUsage.NotUsed, true, true, true, false, -10, false)]
	public class LevelEvent_SetGameSound : LevelEvent_Base
	{
		private SoundData[] soundsData;

		[JsonProperty("", "", null, "", false, true, null, null)]
		[Dropdown(new string[]
		{
			"SmallMistake", "BigMistake", "Hand1PopSound", "Hand2PopSound", "HeartExplosion", "HeartExplosion2", "HeartExplosion3", "Skipshot", "ClapSoundHold", "PulseSoundHold",
			"ClapSoundHoldP2", "PulseSoundHoldP2", "FreezeshotSound", "BurnshotSound", "HoldshotSound"
		}, false, true)]
		public GameSoundType soundType { get; set; } = GameSoundType.SmallMistake;

		[JsonProperty("", "", null, "", false, true, null, null)]
		[SetGameSound]
		public SoundDataStruct[] sounds { get; set; } = new SoundDataStruct[1]
		{
			new SoundDataStruct("")
		};

		private Dictionary<GameSoundType, GameSoundType[]> gameSoundGroups => RDEditorConstants.gameSoundGroups;

		public override void Init()
		{
			base.Init();
			soundsData = new SoundData[1]
			{
				new SoundData("")
			};
		}

		public override void Decode(Dictionary<string, object> dict)
		{
			if (dict.ContainsKey("soundSubtypes"))
			{
				soundsData = RDEditorUtils.DecodeSoundArray(dict["soundSubtypes"]);
			}
			else
			{
				IEnumerable<GameSoundType> source = gameSoundGroups.Keys.Where((GameSoundType k) => gameSoundGroups[k].Contains(soundType));
				if (source.Any())
				{
					GameSoundType key = source.First();
					GameSoundType[] array = gameSoundGroups[key];
					soundsData = new SoundData[array.Length];
					int num = Array.IndexOf(array, soundType);
					for (int num2 = 0; num2 < soundsData.Length; num2++)
					{
						SoundData soundData = new SoundData("");
						if (num2 == num)
						{
							soundData.Decode(dict);
						}
						else
						{
							soundData.used = false;
						}
						soundsData[num2] = soundData;
					}
					soundType = key;
				}
				else
				{
					soundsData = new SoundData[1]
					{
						new SoundData("")
					};
					soundsData[0].Decode(dict);
				}
			}
			sounds = new SoundDataStruct[soundsData.Length];
			for (int num3 = 0; num3 < soundsData.Length; num3++)
			{
				SoundData soundData2 = soundsData[num3];
				sounds[num3] = new SoundDataStruct(soundData2.filename, soundData2.volume, soundData2.pitch, soundData2.pan, soundData2.offset, soundData2.used);
			}
			base.Decode(dict);
			GameSoundType gameSoundType = soundType;
			if ((uint)(gameSoundType - 4) <= 4u)
			{
				executionTime = LevelEventExecutionTime.OnBar;
				sortOrderOffset = 0;
			}
			else
			{
				executionTime = LevelEventExecutionTime.OnPrebar;
				sortOrderOffset = -10;
			}
		}

		public override IEnumerator Prepare()
		{
			soundsData = new SoundData[sounds.Length];
			for (int i = 0; i < soundsData.Length; i++)
			{
				soundsData[i] = sounds[i].ToSoundData();
				soundsData[i].groupSubtype = ((sounds.Length > 1) ? gameSoundGroups[soundType][i] : GameSoundType.ClapSoundP1Classic);
				yield return soundsData[i].Prepare();
			}
			prepared = true;
		}

		public override void Run()
		{
			SoundData[] array = soundsData;
			foreach (SoundData s in array)
			{
				if (!s.used)
				{
					continue;
				}
				GameSoundType sType = ((s.groupSubtype != GameSoundType.ClapSoundP1Classic) ? s.groupSubtype : soundType);
				if (string.IsNullOrEmpty(s.filename))
				{
					if (sType == GameSoundType.Hand1PopSound || sType == GameSoundType.Hand2PopSound)
					{
						s.filename = base.game.GetPlayerHandPopSound(sType == GameSoundType.Hand1PopSound);
					}
					else
					{
						s.filename = RDGameSounds.defaultSounds[sType].filename;
					}
				}
				if (executionTime == LevelEventExecutionTime.OnBar)
				{
					RunOnBeat(() =>
					{
						RDGameSounds.Set(sType, s.conductorFilename, s.volumePercentage, s.pitchPercentage, s.panPercentage);
					});
				}
				else
				{
					RDGameSounds.Set(sType, s.conductorFilename, s.volumePercentage, s.pitchPercentage, s.panPercentage);
				}
			}
		}

		public override string GetTooltipText()
		{
			return RDString.GetEnumValue(soundType);
		}

		public override void CopyFromInternal(LevelEvent_Base levelEvent)
		{
			base.CopyFromInternal(levelEvent);
			LevelEvent_SetGameSound levelEvent_SetGameSound = (LevelEvent_SetGameSound)levelEvent;
			soundType = levelEvent_SetGameSound.soundType;
			soundsData = new SoundData[levelEvent_SetGameSound.soundsData.Length];
			for (int i = 0; i < soundsData.Length; i++)
			{
				soundsData[i] = new SoundData("");
				soundsData[i].CopyFrom(levelEvent_SetGameSound.soundsData[i]);
			}
		}
	}
}
