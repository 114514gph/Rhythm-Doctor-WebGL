using System.Collections.Generic;
using UnityEngine;

public class RDGameSounds : MonoBehaviour
{
	private RDGameSoundData[] defaults;

	public RDGameSoundData[] sounds;

	public static Dictionary<GameSoundType, RDGameSoundData> defaultSounds;

	private static RDGameSounds _instance;

	public static RDGameSounds data
	{
		get
		{
			if (_instance == null)
			{
				Init();
			}
			return _instance;
		}
	}

	public static RDGameSoundData Get(GameSoundType type)
	{
		RDGameSoundData[] array = data.sounds;
		for (int i = 0; i < array.Length; i++)
		{
			RDGameSoundData result = array[i];
			if (result.type == type)
			{
				return result;
			}
		}
		Debug.LogError("sound with type " + type.ToString() + "not found");
		return new RDGameSoundData(type, "", 0f);
	}

	public static void Set(GameSoundType type, string name, float volume = -1000f, float pitch = -1000f, float pan = -1000f)
	{
		int num = 0;
		RDGameSoundData[] array = data.sounds;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].type == type)
			{
				data.sounds[num].filename = name;
				if (volume != -1000f)
				{
					data.sounds[num].volume = volume;
				}
				if (pitch != -1000f)
				{
					data.sounds[num].minPitch = pitch;
					data.sounds[num].maxPitch = pitch;
				}
				if (pan != -1000f)
				{
					data.sounds[num].pan = pan;
				}
				return;
			}
			num++;
		}
		Debug.LogError("RDGameSounds - Sound type not found: " + type);
	}

	public static void SetVolume(GameSoundType type, float volume)
	{
		int num = 0;
		RDGameSoundData[] array = data.sounds;
		for (int i = 0; i < array.Length && array[i].type != type; i++)
		{
			num++;
		}
		data.sounds[num].volume = volume;
	}

	public static void SetMinPitch(GameSoundType type, float minPitch)
	{
		int num = 0;
		RDGameSoundData[] array = data.sounds;
		for (int i = 0; i < array.Length && array[i].type != type; i++)
		{
			num++;
		}
		data.sounds[num].minPitch = minPitch;
	}

	public static void SetMaxPitch(GameSoundType type, float maxPitch)
	{
		int num = 0;
		RDGameSoundData[] array = data.sounds;
		for (int i = 0; i < array.Length && array[i].type != type; i++)
		{
			num++;
		}
		data.sounds[num].maxPitch = maxPitch;
	}

	public static void SetPan(GameSoundType type, float pan)
	{
		int num = 0;
		RDGameSoundData[] array = data.sounds;
		for (int i = 0; i < array.Length && array[i].type != type; i++)
		{
			num++;
		}
		data.sounds[num].pan = pan;
	}

	public static void LoadDefaults()
	{
		if (_instance == null)
		{
			Init();
			return;
		}
		_instance.sounds = new RDGameSoundData[_instance.defaults.Length];
		_instance.defaults.CopyTo(_instance.sounds, 0);
	}

	public static void Init()
	{
		_instance = Object.Instantiate(Resources.Load<GameObject>("RDGameSounds")).GetComponent<RDGameSounds>();
		_instance.name = "RDGameSounds";
		_instance.defaults = new RDGameSoundData[_instance.sounds.Length];
		_instance.sounds.CopyTo(_instance.defaults, 0);
		defaultSounds = new Dictionary<GameSoundType, RDGameSoundData>();
		RDGameSoundData[] array = _instance.defaults;
		for (int i = 0; i < array.Length; i++)
		{
			RDGameSoundData value = array[i];
			defaultSounds.Add(value.type, value);
		}
	}
}
