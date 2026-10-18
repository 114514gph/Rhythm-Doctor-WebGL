namespace OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter0
{
	public class Chapter0Short : IStaticCodeBook
	{
		public int Dimensions { get; } = 2;

		public byte[] LengthList { get; } = new byte[81]
		{
			9, 8, 12, 11, 12, 13, 14, 14, 16, 6,
			1, 5, 6, 6, 9, 12, 14, 17, 9, 4,
			5, 9, 7, 9, 13, 15, 16, 8, 5, 8,
			6, 8, 10, 13, 17, 17, 9, 6, 7, 7,
			8, 9, 13, 15, 17, 11, 8, 9, 9, 9,
			10, 12, 16, 16, 13, 7, 8, 7, 7, 9,
			12, 14, 15, 13, 6, 7, 5, 5, 7, 10,
			13, 13, 14, 7, 8, 5, 6, 7, 9, 10,
			12
		};

		public CodeBookMapType MapType { get; }

		public int QuantMin { get; }

		public int QuantDelta { get; }

		public int Quant { get; }

		public int QuantSequenceP { get; }

		public int[] QuantList { get; }
	}
}
