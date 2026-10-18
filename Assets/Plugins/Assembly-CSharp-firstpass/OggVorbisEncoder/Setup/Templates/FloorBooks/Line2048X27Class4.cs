namespace OggVorbisEncoder.Setup.Templates.FloorBooks
{
	public class Line2048X27Class4 : IStaticCodeBook
	{
		public int Dimensions { get; } = 1;

		public byte[] LengthList { get; } = new byte[64]
		{
			2, 4, 7, 13, 4, 5, 7, 15, 8, 7,
			10, 16, 16, 14, 16, 16, 2, 4, 7, 16,
			3, 4, 7, 14, 8, 8, 10, 16, 16, 16,
			15, 16, 6, 8, 11, 16, 7, 7, 9, 16,
			11, 9, 13, 16, 16, 16, 15, 16, 16, 16,
			16, 16, 14, 16, 16, 16, 16, 16, 16, 16,
			16, 16, 16, 16
		};

		public CodeBookMapType MapType { get; }

		public int QuantMin { get; }

		public int QuantDelta { get; }

		public int Quant { get; }

		public int QuantSequenceP { get; }

		public int[] QuantList { get; }
	}
}
