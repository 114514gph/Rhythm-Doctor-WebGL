namespace OggVorbisEncoder.Setup.Templates.FloorBooks
{
	public class Line512X17_3Sub2 : IStaticCodeBook
	{
		public int Dimensions { get; } = 1;

		public byte[] LengthList { get; } = new byte[50]
		{
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 2, 3,
			3, 4, 3, 5, 4, 6, 4, 6, 5, 7,
			6, 7, 6, 8, 6, 8, 7, 9, 8, 10,
			8, 12, 9, 13, 10, 15, 10, 15, 11, 14
		};

		public CodeBookMapType MapType { get; }

		public int QuantMin { get; }

		public int QuantDelta { get; }

		public int Quant { get; }

		public int QuantSequenceP { get; }

		public int[] QuantList { get; }
	}
}
