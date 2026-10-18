namespace OggVorbisEncoder.Setup.Templates.FloorBooks
{
	public class Line512X17Class3 : IStaticCodeBook
	{
		public int Dimensions { get; } = 1;

		public byte[] LengthList { get; } = new byte[64]
		{
			2, 4, 6, 17, 4, 5, 7, 17, 8, 7,
			10, 17, 17, 17, 17, 17, 3, 4, 6, 15,
			3, 3, 6, 15, 7, 6, 9, 17, 17, 17,
			17, 17, 6, 8, 10, 17, 6, 6, 8, 16,
			9, 8, 10, 17, 17, 15, 16, 17, 17, 17,
			17, 17, 12, 15, 15, 16, 12, 15, 15, 16,
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
