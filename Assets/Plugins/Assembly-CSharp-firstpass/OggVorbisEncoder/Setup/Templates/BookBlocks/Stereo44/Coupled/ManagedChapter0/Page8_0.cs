namespace OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.ManagedChapter0
{
	public class Page8_0 : IStaticCodeBook
	{
		public int Dimensions { get; } = 2;

		public byte[] LengthList { get; } = new byte[81]
		{
			1, 3, 3, 11, 11, 11, 11, 11, 11, 3,
			7, 6, 11, 11, 11, 11, 11, 11, 4, 8,
			7, 11, 11, 11, 11, 11, 11, 11, 11, 11,
			11, 11, 11, 11, 11, 11, 11, 11, 11, 11,
			11, 11, 11, 11, 11, 11, 11, 11, 11, 12,
			12, 12, 12, 12, 12, 12, 12, 12, 12, 12,
			12, 12, 12, 12, 12, 12, 12, 12, 12, 12,
			12, 12, 12, 12, 12, 12, 12, 12, 12, 12,
			12
		};

		public CodeBookMapType MapType { get; } = CodeBookMapType.Implicit;

		public int QuantMin { get; } = -516186112;

		public int QuantDelta { get; } = 1627103232;

		public int Quant { get; } = 4;

		public int QuantSequenceP { get; }

		public int[] QuantList { get; } = new int[9] { 4, 3, 5, 2, 6, 1, 7, 0, 8 };
	}
}
