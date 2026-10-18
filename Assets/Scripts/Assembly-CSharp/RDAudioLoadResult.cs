using UnityEngine;

public struct RDAudioLoadResult
{
	public RDAudioLoadType type;

	public AudioClip clip;

	public RDAudioLoadResult(RDAudioLoadType type, AudioClip clip)
	{
		this.type = type;
		this.clip = clip;
	}

	public new string ToString()
	{
		string text = ((clip == null) ? "(null)" : clip.name);
		return "type: " + type.ToString() + " clip: " + text;
	}
}
