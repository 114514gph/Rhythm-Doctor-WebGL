namespace OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Uncoupled.Chapter2
{
	public class Page7_2 : IStaticCodeBook
	{
		public int Dimensions { get; } = 2;

		public byte[] LengthList { get; } = new byte[169]
		{
			2, 5, 5, 6, 6, 7, 7, 8, 7, 8,
			8, 8, 8, 5, 6, 6, 7, 7, 8, 8,
			8, 8, 8, 8, 8, 8, 5, 6, 6, 7,
			7, 8, 7, 8, 8, 8, 8, 8, 8, 6,
			7, 7, 7, 8, 8, 8, 8, 8, 9, 9,
			9, 9, 6, 7, 7, 8, 7, 8, 8, 9,
			9, 9, 9, 9, 9, 7, 8, 8, 8, 8,
			8, 8, 9, 9, 9, 9, 9, 9, 7, 8,
			8, 8, 8, 8, 8, 9, 9, 9, 9, 9,
			9, 8, 8, 8, 8, 9, 9, 9, 9, 9,
			9, 9, 9, 9, 8, 8, 8, 9, 9, 9,
			9, 9, 9, 9, 9, 9, 9, 8, 8, 9,
			9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
			8, 9, 9, 9, 9, 9, 9, 9, 9, 9,
			9, 9, 9, 8, 9, 9, 9, 9, 9, 9,
			9, 9, 9, 9, 9, 9, 8, 9, 9, 9,
			9, 9, 9, 9, 9, 9, 9, 9, 9
		};

		public CodeBookMapType MapType { get; } = CodeBookMapType.Implicit;

		public int QuantMin { get; } = -531103744;

		public int QuantDelta { get; } = 1611661312;

		public int Quant { get; } = 4;

		public int QuantSequenceP { get; }

		public int[] QuantList { get; } = new int[13]
		{
			6, 5, 7, 4, 8, 3, 9, 2, 10, 1,
			11, 0, 12
		};
	}
}
