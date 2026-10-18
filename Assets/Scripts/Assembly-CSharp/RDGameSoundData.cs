using System;
using UnityEngine;

[Serializable]
public struct RDGameSoundData
{
	public GameSoundType type;

	public string filename;

	[Range(0f, 3f)]
	public float volume;

	public float minPitch;

	public float maxPitch;

	public float pan;

	public RDGameSoundData(GameSoundType type, string sound, float volume)
	{
		this.type = type;
		filename = sound;
		this.volume = volume;
		minPitch = (maxPitch = 1f);
		pan = 0f;
	}
}
