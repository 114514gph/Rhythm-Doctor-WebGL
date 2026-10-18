public class BPMChange
{
	public int bar;

	public float beat;

	public float bpm;

	public float time;

	public BPMChange(float beat, float bpm)
	{
		this.beat = beat;
		this.bpm = bpm;
	}

	public BPMChange(int bar, float beat, float bpm, float time)
	{
		this.bar = bar;
		this.beat = beat;
		this.bpm = bpm;
		this.time = time;
	}
}
