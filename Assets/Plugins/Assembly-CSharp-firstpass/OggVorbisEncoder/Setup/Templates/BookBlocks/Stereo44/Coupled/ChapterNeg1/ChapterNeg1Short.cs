namespace OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.ChapterNeg1
{
	public class ChapterNeg1Short : IStaticCodeBook
	{
		public int Dimensions { get; } = 2;

		public byte[] LengthList { get; } = new byte[81]
		{
			10, 9, 12, 15, 12, 13, 16, 14, 16, 7,
			1, 5, 14, 7, 10, 13, 16, 16, 9, 4,
			6, 16, 8, 11, 16, 16, 16, 14, 4, 7,
			16, 9, 12, 14, 16, 16, 10, 5, 7, 14,
			9, 12, 14, 15, 15, 13, 8, 9, 14, 10,
			12, 13, 14, 15, 13, 9, 9, 7, 6, 8,
			11, 12, 12, 14, 8, 8, 5, 4, 5, 8,
			11, 12, 16, 10, 10, 6, 5, 6, 8, 9,
			10
		};

		public CodeBookMapType MapType { get; }

		public int QuantMin { get; }

		public int QuantDelta { get; }

		public int Quant { get; }

		public int QuantSequenceP { get; }

		public int[] QuantList { get; }
	}
}
