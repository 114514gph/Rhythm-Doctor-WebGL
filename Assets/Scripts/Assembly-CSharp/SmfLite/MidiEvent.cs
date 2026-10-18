namespace SmfLite
{
	public struct MidiEvent
	{
		public byte status;

		public byte data1;

		public byte data2;

		public MidiEvent(byte status, byte data1, byte data2)
		{
			this.status = status;
			this.data1 = data1;
			this.data2 = data2;
		}

		public override string ToString()
		{
			return "[" + status.ToString("X") + "," + data1 + "," + data2 + "]";
		}
	}
}
