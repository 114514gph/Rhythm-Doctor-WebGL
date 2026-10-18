namespace OggVorbisEncoder.Setup.Templates.FloorBooks
{
	public class Line256X7_0Sub3 : IStaticCodeBook
	{
		public int Dimensions { get; } = 1;

		public byte[] LengthList { get; } = new byte[64]
		{
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 3, 5, 2, 5, 3,
			5, 3, 6, 3, 6, 4, 7, 6, 7, 8,
			7, 9, 8, 9, 9, 9, 10, 9, 11, 13,
			11, 13, 10, 10, 13, 13, 13, 13, 13, 13,
			12, 12, 12, 12
		};

		public CodeBookMapType MapType { get; }

		public int QuantMin { get; }

		public int QuantDelta { get; }

		public int Quant { get; }

		public int QuantSequenceP { get; }

		public int[] QuantList { get; }
	}
}
