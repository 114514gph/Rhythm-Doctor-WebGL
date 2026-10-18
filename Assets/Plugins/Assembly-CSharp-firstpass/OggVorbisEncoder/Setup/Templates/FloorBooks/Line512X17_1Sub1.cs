namespace OggVorbisEncoder.Setup.Templates.FloorBooks
{
	public class Line512X17_1Sub1 : IStaticCodeBook
	{
		public int Dimensions { get; } = 1;

		public byte[] LengthList { get; } = new byte[128]
		{
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 4, 3, 5, 3, 5, 4, 5, 4,
			5, 4, 5, 5, 5, 5, 6, 5, 6, 5,
			7, 5, 8, 6, 8, 6, 8, 6, 8, 6,
			8, 7, 9, 7, 9, 7, 11, 9, 11, 11,
			12, 11, 14, 12, 14, 16, 14, 16, 13, 16,
			14, 16, 12, 15, 13, 16, 14, 16, 13, 14,
			12, 15, 13, 15, 13, 13, 13, 15, 12, 14,
			14, 15, 13, 15, 12, 15, 15, 15, 15, 15,
			15, 15, 15, 15, 15, 15, 15, 15, 15, 15,
			15, 15, 15, 15, 15, 15, 15, 15
		};

		public CodeBookMapType MapType { get; }

		public int QuantMin { get; }

		public int QuantDelta { get; }

		public int Quant { get; }

		public int QuantSequenceP { get; }

		public int[] QuantList { get; }
	}
}
