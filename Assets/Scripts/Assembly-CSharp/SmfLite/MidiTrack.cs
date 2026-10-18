using System.Collections.Generic;

namespace SmfLite
{
	public class MidiTrack
	{
		public struct DeltaEventPair
		{
			public int delta;

			public MidiEvent midiEvent;

			public DeltaEventPair(int delta, MidiEvent midiEvent)
			{
				this.delta = delta;
				this.midiEvent = midiEvent;
			}

			public override string ToString()
			{
				string[] obj = new string[5]
				{
					"(",
					delta.ToString(),
					":",
					null,
					null
				};
				MidiEvent midiEvent = this.midiEvent;
				obj[3] = midiEvent.ToString();
				obj[4] = ") \n";
				return string.Concat(obj);
			}
		}

		private List<DeltaEventPair> sequence;

		public string name { get; set; }

		public MidiTrack()
		{
			sequence = new List<DeltaEventPair>();
		}

		public List<DeltaEventPair>.Enumerator GetEnumerator()
		{
			return sequence.GetEnumerator();
		}

		public List<DeltaEventPair> GetList()
		{
			return sequence;
		}

		public DeltaEventPair GetAtIndex(int index)
		{
			return sequence[index];
		}

		public override string ToString()
		{
			string text = name + ": ";
			foreach (DeltaEventPair item in sequence)
			{
				string text2 = text;
				DeltaEventPair deltaEventPair = item;
				text = text2 + deltaEventPair.ToString();
			}
			return text;
		}

		public void AddEvent(int delta, MidiEvent midiEvent)
		{
			sequence.Add(new DeltaEventPair(delta, midiEvent));
		}
	}
}
