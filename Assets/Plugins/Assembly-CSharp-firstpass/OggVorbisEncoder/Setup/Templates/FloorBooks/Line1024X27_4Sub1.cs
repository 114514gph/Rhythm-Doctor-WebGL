namespace OggVorbisEncoder.Setup.Templates.FloorBooks
{
	public class Line1024X27_4Sub1 : IStaticCodeBook
	{
		public int Dimensions { get; } = 1;

		public byte[] LengthList { get; } = new byte[18]
		{
			0, 4, 5, 4, 5, 4, 5, 4, 5, 3,
			5, 3, 5, 3, 5, 4, 5, 4
		};

		public CodeBookMapType MapType { get; }

		public int QuantMin { get; }

		public int QuantDelta { get; }

		public int Quant { get; }

		public int QuantSequenceP { get; }

		public int[] QuantList { get; }
	}
}
