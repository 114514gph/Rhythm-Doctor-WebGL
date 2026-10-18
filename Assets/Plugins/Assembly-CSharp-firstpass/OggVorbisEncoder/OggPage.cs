namespace OggVorbisEncoder
{
	public class OggPage
	{
		public byte[] Header { get; }

		public byte[] Body { get; }

		public OggPage(byte[] header, byte[] body)
		{
			Header = header;
			Body = body;
		}
	}
}
