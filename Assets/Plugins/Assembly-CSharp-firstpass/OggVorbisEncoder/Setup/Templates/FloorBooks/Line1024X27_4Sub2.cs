namespace OggVorbisEncoder.Setup.Templates.FloorBooks
{
	public class Line1024X27_4Sub2 : IStaticCodeBook
	{
		public int Dimensions { get; } = 1;

		public byte[] LengthList { get; } = new byte[50]
		{
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 4, 2,
			4, 2, 5, 3, 5, 4, 6, 6, 6, 7,
			7, 8, 7, 8, 7, 8, 7, 9, 8, 9,
			8, 9, 8, 10, 8, 11, 9, 12, 9, 12
		};

		public CodeBookMapType MapType { get; }

		public int QuantMin { get; }

		public int QuantDelta { get; }

		public int Quant { get; }

		public int QuantSequenceP { get; }

		public int[] QuantList { get; }
	}
}
