namespace OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Uncoupled.Chapter8
{
	public class Page6_0 : IStaticCodeBook
	{
		public int Dimensions { get; } = 2;

		public byte[] LengthList { get; } = new byte[169]
		{
			2, 4, 4, 6, 6, 7, 7, 8, 8, 9,
			9, 10, 10, 4, 6, 5, 7, 7, 8, 8,
			8, 8, 9, 9, 10, 10, 4, 6, 6, 7,
			7, 8, 8, 8, 8, 9, 9, 10, 10, 6,
			7, 7, 7, 8, 8, 8, 8, 9, 9, 10,
			10, 10, 6, 7, 7, 8, 8, 8, 8, 9,
			8, 10, 9, 11, 10, 7, 8, 8, 8, 8,
			8, 9, 9, 9, 10, 10, 11, 11, 7, 8,
			8, 8, 8, 9, 8, 9, 9, 10, 10, 11,
			11, 8, 8, 8, 9, 9, 9, 9, 9, 10,
			10, 10, 11, 11, 8, 8, 8, 9, 9, 9,
			9, 10, 9, 10, 10, 11, 11, 9, 9, 9,
			9, 10, 10, 10, 10, 10, 10, 11, 11, 12,
			9, 9, 9, 10, 9, 10, 10, 10, 10, 11,
			10, 12, 11, 10, 10, 10, 10, 10, 11, 11,
			11, 11, 11, 12, 12, 12, 10, 10, 10, 10,
			11, 11, 11, 11, 11, 12, 11, 12, 12
		};

		public CodeBookMapType MapType { get; } = CodeBookMapType.Implicit;

		public int QuantMin { get; } = -526516224;

		public int QuantDelta { get; } = 1616117760;

		public int Quant { get; } = 4;

		public int QuantSequenceP { get; }

		public int[] QuantList { get; } = new int[13]
		{
			6, 5, 7, 4, 8, 3, 9, 2, 10, 1,
			11, 0, 12
		};
	}
}
