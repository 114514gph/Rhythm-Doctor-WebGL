namespace OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo16.Uncoupled.Chapter2
{
	public class Page3_0 : IStaticCodeBook
	{
		public int Dimensions { get; } = 2;

		public byte[] LengthList { get; } = new byte[81]
		{
			2, 4, 4, 6, 6, 7, 7, 9, 9, 4,
			5, 5, 6, 6, 8, 7, 9, 9, 4, 5,
			5, 6, 6, 7, 8, 9, 9, 6, 6, 6,
			7, 7, 8, 8, 10, 10, 6, 6, 6, 7,
			7, 8, 8, 10, 10, 7, 8, 7, 8, 8,
			9, 9, 11, 10, 7, 7, 8, 8, 8, 9,
			9, 10, 11, 9, 9, 9, 10, 10, 11, 10,
			11, 11, 9, 9, 9, 10, 10, 10, 11, 11,
			11
		};

		public CodeBookMapType MapType { get; } = CodeBookMapType.Implicit;

		public int QuantMin { get; } = -531628032;

		public int QuantDelta { get; } = 1611661312;

		public int Quant { get; } = 4;

		public int QuantSequenceP { get; }

		public int[] QuantList { get; } = new int[9] { 4, 3, 5, 2, 6, 1, 7, 0, 8 };
	}
}
