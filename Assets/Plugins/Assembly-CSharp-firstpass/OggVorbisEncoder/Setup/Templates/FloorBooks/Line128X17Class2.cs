namespace OggVorbisEncoder.Setup.Templates.FloorBooks
{
	public class Line128X17Class2 : IStaticCodeBook
	{
		public int Dimensions { get; } = 1;

		public byte[] LengthList { get; } = new byte[64]
		{
			1, 4, 10, 19, 3, 8, 13, 19, 7, 12,
			19, 19, 19, 19, 19, 19, 2, 6, 11, 19,
			8, 13, 19, 19, 9, 11, 19, 19, 19, 19,
			19, 19, 6, 7, 13, 19, 9, 13, 19, 19,
			10, 13, 18, 18, 18, 18, 18, 18, 18, 18,
			18, 18, 18, 18, 18, 18, 18, 18, 18, 18,
			18, 18, 18, 18
		};

		public CodeBookMapType MapType { get; }

		public int QuantMin { get; }

		public int QuantDelta { get; }

		public int Quant { get; }

		public int QuantSequenceP { get; }

		public int[] QuantList { get; }
	}
}
