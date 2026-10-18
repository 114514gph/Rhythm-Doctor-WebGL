using System;
using System.Collections.Generic;

namespace SmfLite
{
	public static class MidiFileLoader
	{
		public static MidiFileContainer Load(byte[] data)
		{
			List<MidiTrack> list = new List<MidiTrack>();
			MidiDataStreamReader midiDataStreamReader = new MidiDataStreamReader(data);
			if (new string(midiDataStreamReader.ReadChars(4)) != "MThd")
			{
				throw new FormatException("Can't find header chunk.");
			}
			if (midiDataStreamReader.ReadBEInt32() != 6)
			{
				throw new FormatException("Length of header chunk must be 6.");
			}
			midiDataStreamReader.Advance(2);
			int num = midiDataStreamReader.ReadBEInt16();
			int num2 = midiDataStreamReader.ReadBEInt16();
			if ((num2 & 0x8000) != 0)
			{
				throw new FormatException("SMPTE time code is not supported.");
			}
			for (int i = 0; i < num; i++)
			{
				list.Add(ReadTrack(midiDataStreamReader));
			}
			return new MidiFileContainer(num2, list);
		}

		private static MidiTrack ReadTrack(MidiDataStreamReader reader)
		{
			MidiTrack midiTrack = new MidiTrack();
			if (new string(reader.ReadChars(4)) != "MTrk")
			{
				throw new FormatException("Can't find track chunk.");
			}
			int num = reader.ReadBEInt32();
			num += reader.Offset;
			byte b = 0;
			while (reader.Offset < num)
			{
				int delta = reader.ReadMultiByteValue();
				if ((reader.PeekByte() & 0x80) != 0)
				{
					b = reader.ReadByte();
				}
				switch (b)
				{
				case byte.MaxValue:
					if (reader.PeekByte() == 3)
					{
						reader.Advance(1);
						int length = reader.ReadByte();
						midiTrack.name = new string(reader.ReadChars(length));
					}
					else
					{
						reader.Advance(1);
						reader.Advance(reader.ReadMultiByteValue());
					}
					break;
				case 240:
					while (reader.ReadByte() != 247)
					{
					}
					break;
				default:
				{
					byte data = reader.ReadByte();
					byte data2 = (byte)(((b & 0xE0) != 192) ? reader.ReadByte() : 0);
					midiTrack.AddEvent(delta, new MidiEvent(b, data, data2));
					break;
				}
				}
			}
			return midiTrack;
		}
	}
}
