using System.Collections.Generic;

namespace SmfLite
{
	public class MidiTrackSequencer
	{
		private List<MidiTrack.DeltaEventPair>.Enumerator enumerator;

		private bool playing;

		private float pulsePerSecond;

		private float pulseToNext;

		private float pulseCounter;

		public bool Playing => playing;

		public MidiTrackSequencer(MidiTrack track, int ppqn, float bpm)
		{
			pulsePerSecond = bpm / 60f * (float)ppqn;
			enumerator = track.GetEnumerator();
		}

		public List<MidiEvent> Start(float startTime = 0f)
		{
			if (enumerator.MoveNext())
			{
				pulseToNext = enumerator.Current.delta;
				playing = true;
				return Advance(startTime);
			}
			playing = false;
			return null;
		}

		public List<MidiEvent> Advance(float deltaTime)
		{
			if (!playing)
			{
				return null;
			}
			pulseCounter += pulsePerSecond * deltaTime;
			if (pulseCounter < pulseToNext)
			{
				return null;
			}
			List<MidiEvent> list = new List<MidiEvent>();
			while (pulseCounter >= pulseToNext)
			{
				list.Add(enumerator.Current.midiEvent);
				if (!enumerator.MoveNext())
				{
					playing = false;
					break;
				}
				pulseCounter -= pulseToNext;
				pulseToNext = enumerator.Current.delta;
			}
			return list;
		}
	}
}
