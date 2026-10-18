namespace OggVorbisEncoder
{
	public class OggPacket
	{
		public byte[] PacketData { get; }

		public bool EndOfStream { get; }

		public int GranulePosition { get; }

		public int PacketNumber { get; }

		public OggPacket(byte[] packetData, bool endOfStream, int granulePosition, int packetNumber)
		{
			PacketData = packetData;
			EndOfStream = endOfStream;
			GranulePosition = granulePosition;
			PacketNumber = packetNumber;
		}
	}
}
