namespace OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Uncoupled.ChapterNeg1
{
	public class Page2_0 : IStaticCodeBook
	{
		public int Dimensions { get; } = 4;

		public byte[] LengthList { get; } = new byte[81]
		{
			2, 4, 4, 5, 6, 6, 5, 6, 6, 5,
			7, 7, 7, 8, 8, 6, 7, 9, 5, 7,
			7, 6, 8, 7, 7, 9, 8, 4, 7, 7,
			7, 9, 8, 7, 8, 8, 7, 9, 8, 8,
			8, 10, 9, 10, 10, 6, 8, 8, 7, 10,
			8, 9, 10, 10, 5, 7, 7, 7, 8, 8,
			7, 8, 9, 6, 8, 8, 9, 10, 10, 7,
			8, 10, 6, 8, 9, 9, 10, 10, 8, 10,
			8
		};

		public CodeBookMapType MapType { get; } = CodeBookMapType.Implicit;

		public int QuantMin { get; } = -535822336;

		public int QuantDelta { get; } = 1611661312;

		public int Quant { get; } = 2;

		public int QuantSequenceP { get; }

		public int[] QuantList { get; } = new int[3] { 1, 0, 2 };
	}
}
