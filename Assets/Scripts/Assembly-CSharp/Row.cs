using System;
using RDLevelEditor;
using UnityEngine;

public class Row
{
	public int room;

	public RowType rowType;

	public bool rowChangeWarning;

	public SoundData[] pulseSounds = new SoundData[7];

	public float tick;

	public bool[] showAesthetic;

	public float showdelay;

	public int syncoBeat = -1;

	public float syncoSwing;

	public float syncoVolume = 1f;

	public float syncoPitch = 1f;

	public SyncoStyle syncoStyle;

	public bool syncoShouldPlayModifierOffSound;

	public bool isFlashing;

	public bool faceDirection = true;

	public Character cpuCharacter = Character.Otto;

	[NonSerialized]
	public Character cpuCharacterToChangeInto = Character.None;

	[NonSerialized]
	public int transferTarget = -1;

	[NonSerialized]
	public int rowLengthForNarration = 7;

	public bool rowCountingVoiceEnabled;

	public SoundData[] countingSounds = new SoundData[7];

	public float rowCountingVoiceSubdivOffset = 0.5f;

	public float rowExplosionPitch = 1f;

	public string rowSoundHitOverride = "";

	public float rowSoundHitOverrideMultiplier = 1f;

	public bool muteRowSounds;

	public RowEntity ent;

	public SpriteReflection reflection;

	public bool dead;

	public int lastMissedFrame;

	public static int[] lastHitFrame = new int[2];

	public float mistakeWeight = 1f;

	public RDPlayer singleplayer;

	public RDPlayer multiplayer;

	public bool isShadow;

	public bool isShadowRealtime;

	public bool copyBeatsWhenShadow = true;

	public bool normalizeMistakeWeightWhenShadow = true;

	public bool copyPaintEffectsWhenShadowOrHost = true;

	public bool reactToHostsBeatsWhenShadow;

	public int shadowOrHostID = -1;

	public int shadowOrHostIDRealtime = -1;

	public RDPlayer multiplayerToChangeInto;

	public RDPlayer singleplayerToChangeInto;

	public int id { get; private set; }

	public bool[] show { get; private set; }

	public string rowSound
	{
		get
		{
			return pulseSounds[0].filename;
		}
		set
		{
			SetPulseSounds(new SoundData(value));
		}
	}

	public float rowSoundVolMultiplier
	{
		get
		{
			if (muteRowSounds)
			{
				return 0f;
			}
			return pulseSounds[0].volumePercentage;
		}
		set
		{
			SoundData[] array = pulseSounds;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].volumePercentage = value;
			}
		}
	}

	public float rowSoundMinPitch
	{
		get
		{
			return pulseSounds[0].minPitchPercentage;
		}
		set
		{
			SoundData[] array = pulseSounds;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].minPitchPercentage = value;
			}
		}
	}

	public float rowSoundMaxPitch
	{
		get
		{
			return pulseSounds[0].maxPitchPercentage;
		}
		set
		{
			SoundData[] array = pulseSounds;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].maxPitchPercentage = value;
			}
		}
	}

	public float rowSoundPan
	{
		get
		{
			return pulseSounds[0].panPercentage;
		}
		set
		{
			SoundData[] array = pulseSounds;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].panPercentage = value;
			}
		}
	}

	public float rowSoundPanAdjustedFor2P => RDUtils.OverridePanFor2P(this, rowSoundPan);

	public bool cpuControlled => GetCurrentPlayer() == RDPlayer.CPU;

	public string showBeats
	{
		set
		{
			show = ShowBeatsFromString(value);
		}
	}

	public scrBeatbox[] beatboxes
	{
		get
		{
			if (rowType != RowType.Classic || ent == null)
			{
				return null;
			}
			return ent.classicRowController.beatboxes;
		}
	}

	public scrPlayerbox playerBox
	{
		get
		{
			if (ent != null)
			{
				return ent.playerBox;
			}
			return null;
		}
	}

	public bool hasBeatsWithinMargin
	{
		get
		{
			foreach (Beat beat in scnGame.instance.beats)
			{
				if (beat != null && beat.rowID == id && playerBox.HitCheckBeat(0.4f, beat))
				{
					return true;
				}
			}
			return false;
		}
	}

	public void SetPulseSounds(SoundData soundData)
	{
		Array.Fill(pulseSounds, soundData);
	}

	public void SetCountingSounds(CountingVoiceSource voiceSource, float volume = 1f)
	{
		for (int i = 0; i < countingSounds.Length; i++)
		{
			string filename = GC.GetCountingSoundFilename(voiceSource, useInternalFilename: true) + (i + 1);
			countingSounds[i] = new SoundData
			{
				filename = filename,
				volume = (int)(volume * 100f)
			};
		}
	}

	public RDPlayer GetCurrentPlayer()
	{
		if (!GC.twoPlayerMode)
		{
			return singleplayer;
		}
		return multiplayer;
	}

	public RDPlayer GetPlayerWeAreGoingToChangeInto()
	{
		if (!GC.twoPlayerMode)
		{
			return singleplayerToChangeInto;
		}
		return multiplayerToChangeInto;
	}

	public Row(int index)
	{
		show = new bool[6];
		cpuCharacter = Character.Otto;
		cpuCharacterToChangeInto = Character.None;
		for (int i = 0; i < 6; i++)
		{
			show[i] = true;
		}
		SetPulseSounds(new SoundData
		{
			filename = "sndKick"
		});
		id = index;
	}

	public void ToggleReflections(bool enable)
	{
		if (reflection == null)
		{
			if (enable)
			{
				SpriteReflection orAddComponent = new GameObject(ent.character.ToString() + "Reflection").GetOrAddComponent<SpriteReflection>();
				orAddComponent.SetSprite(ent.character.customAnimation, isCharacter: true, null, id);
				reflection = orAddComponent;
				reflection.transform.SetParent(ent.character.transform.parent);
			}
		}
		else
		{
			reflection.gameObject.SetActive(enable);
		}
	}

	public static bool[] ShowBeatsFromString(string s)
	{
		bool[] array = new bool[6];
		for (int i = 0; i < 6; i++)
		{
			array[i] = s[i] != 'x';
		}
		return array;
	}

	public static string StringFromShowBeats(bool[] showBeats)
	{
		string text = "";
		foreach (bool flag in showBeats)
		{
			text += (flag ? "-" : "x");
		}
		return text;
	}

	public void SetBeatSkipsLegacy(string showBeatsString, float beatNumber = 0f)
	{
		show = ShowBeatsFromString(showBeatsString);
		bool[] visualShow = show.Clone() as bool[];
		scrExecuteOnCertainBeat.Add(() =>
		{
			SetBeatSkipsAesthetic(visualShow);
		}, beatNumber);
	}

	public void SetBeatSkips(bool[] showBeats)
	{
		show = showBeats;
	}

	public void SetBeatSkipsAesthetic(bool[] showBeats)
	{
		if (!(ent == null))
		{
			scrBeatbox[] array = ent.classicRowController.beatboxes;
			foreach (scrBeatbox scrBeatbox2 in array)
			{
				bool beatSkip = showBeats[scrBeatbox2.index];
				scrBeatbox2.SetBeatSkip(beatSkip);
			}
		}
	}

	public void RefreshRowPlayersScrub()
	{
		isFlashing = false;
		singleplayer = singleplayerToChangeInto;
		multiplayer = multiplayerToChangeInto;
		if (cpuCharacterToChangeInto != Character.None)
		{
			cpuCharacter = cpuCharacterToChangeInto;
		}
		if (ent != null)
		{
			ent.TintIfCPU(isEntrance: false);
			ent.UpdatePlayerSprites();
		}
	}

	public void RefreshBeatSkipAesthetic()
	{
		SetBeatSkipsAesthetic(show);
	}

	public void SetSyncopation(int sBeat, float sSwing)
	{
		syncoBeat = sBeat;
		syncoSwing = sSwing;
	}

	public void SetSyncopationCueStyle(SyncoStyle sStyle)
	{
		syncoStyle = sStyle;
	}

	public void SetSyncopationAesthetic(int sBeat)
	{
		if (!(ent == null))
		{
			scrBeatbox[] array = ent.classicRowController.beatboxes;
			foreach (scrBeatbox obj in array)
			{
				obj.SetSynco(obj.index == sBeat);
			}
		}
	}

	public void RefreshSyncopationAesthetic()
	{
		SetSyncopationAesthetic(syncoBeat);
	}

	public bool HasBeatsAutohitByHeldbeat(Beat heldBeat)
	{
		foreach (Beat beat in scnGame.instance.beats)
		{
			if (beat != null && beat.rowID == id && !(beat.inputTime + 0.009999999776482582 < heldBeat.inputTime) && !(beat.inputTime - 0.009999999776482582 > heldBeat.releaseTime))
			{
				return true;
			}
		}
		return false;
	}
}
