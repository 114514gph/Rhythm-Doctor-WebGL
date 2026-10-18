using System;

[Serializable]
public class SongOffset
{
	public string name;

	public float offset;

	public float volume;

	public int numberOfBeatsFromSongStart;

	public int prebarBeats;

	public string loopDestination;

	public string folder;

	public bool dummy;

	public FizzdOffsetAccuracy accuracy;

	public bool isInternal;

	public string fullPath => folder + "/" + name;
}
