namespace OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.ManagedChapterNeg1
{
	public class Page3_0 : IStaticCodeBook
	{
		public int Dimensions { get; } = 2;

		public byte[] LengthList { get; } = new byte[81]
		{
			1, 3, 4, 7, 7, 0, 0, 0, 0, 0,
			4, 4, 7, 7, 0, 0, 0, 0, 0, 4,
			5, 7, 7, 0, 0, 0, 0, 0, 6, 7,
			8, 8, 0, 0, 0, 0, 0, 0, 0, 8,
			8, 0, 0, 0, 0, 0, 0, 0, 9, 9,
			0, 0, 0, 0, 0, 0, 0, 10, 9, 0,
			0, 0, 0, 0, 0, 0, 11, 11, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0
		};

		public CodeBookMapType MapType { get; } = CodeBookMapType.Implicit;

		public int QuantMin { get; } = -531628032;

		public int QuantDelta { get; } = 1611661312;

		public int Quant { get; } = 4;

		public int QuantSequenceP { get; }

		public int[] QuantList { get; } = new int[9] { 4, 3, 5, 2, 6, 1, 7, 0, 8 };
	}
}
