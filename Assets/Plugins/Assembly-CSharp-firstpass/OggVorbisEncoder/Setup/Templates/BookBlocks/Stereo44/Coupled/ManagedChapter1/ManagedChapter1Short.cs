namespace OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.ManagedChapter1
{
	public class ManagedChapter1Short : IStaticCodeBook
	{
		public int Dimensions { get; } = 2;

		public byte[] LengthList { get; } = new byte[81]
		{
			4, 7, 13, 14, 14, 15, 16, 18, 18, 4,
			2, 5, 8, 7, 9, 12, 15, 15, 10, 4,
			5, 10, 6, 8, 11, 15, 17, 12, 5, 7,
			5, 6, 8, 11, 14, 17, 11, 5, 6, 6,
			5, 6, 9, 13, 17, 12, 6, 7, 6, 5,
			6, 8, 12, 14, 14, 7, 8, 6, 6, 7,
			9, 11, 14, 14, 8, 9, 6, 5, 6, 9,
			11, 13, 16, 10, 10, 7, 6, 7, 8, 10,
			11
		};

		public CodeBookMapType MapType { get; }

		public int QuantMin { get; }

		public int QuantDelta { get; }

		public int Quant { get; }

		public int QuantSequenceP { get; }

		public int[] QuantList { get; }
	}
}
