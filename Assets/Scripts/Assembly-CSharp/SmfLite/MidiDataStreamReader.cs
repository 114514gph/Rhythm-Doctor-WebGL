namespace SmfLite
{
	internal class MidiDataStreamReader
	{
		private byte[] data;

		private int offset;

		public int Offset => offset;

		public MidiDataStreamReader(byte[] data)
		{
			this.data = data;
		}

		public void Advance(int length)
		{
			offset += length;
		}

		public byte PeekByte()
		{
			return data[offset];
		}

		public byte ReadByte()
		{
			return data[offset++];
		}

		public char[] ReadChars(int length)
		{
			char[] array = new char[length];
			for (int i = 0; i < length; i++)
			{
				array[i] = (char)ReadByte();
			}
			return array;
		}

		public int ReadBEInt32()
		{
			int num = ReadByte();
			int num2 = ReadByte();
			int num3 = ReadByte();
			return ReadByte() + (num3 << 8) + (num2 << 16) + (num << 24);
		}

		public int ReadBEInt16()
		{
			int num = ReadByte();
			return ReadByte() + (num << 8);
		}

		public int ReadMultiByteValue()
		{
			int num = 0;
			while (true)
			{
				int num2 = ReadByte();
				num += num2 & 0x7F;
				if (num2 < 128)
				{
					break;
				}
				num <<= 7;
			}
			return num;
		}
	}
}
