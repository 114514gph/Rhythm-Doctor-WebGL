namespace OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Uncoupled.Chapter6
{
	public class Page8_1 : IStaticCodeBook
	{
		public int Dimensions { get; } = 2;

		public byte[] LengthList { get; } = new byte[121]
		{
			3, 5, 5, 6, 6, 7, 7, 7, 7, 7,
			7, 5, 6, 5, 7, 7, 7, 7, 8, 7,
			8, 8, 5, 5, 6, 6, 7, 7, 7, 7,
			7, 8, 8, 6, 7, 7, 7, 7, 8, 7,
			8, 8, 8, 8, 6, 6, 7, 7, 7, 7,
			8, 8, 8, 8, 8, 7, 7, 7, 8, 8,
			8, 8, 8, 8, 8, 8, 7, 7, 7, 7,
			8, 8, 8, 8, 8, 8, 8, 7, 7, 7,
			8, 8, 8, 8, 8, 8, 8, 8, 7, 8,
			8, 8, 8, 8, 8, 8, 8, 8, 8, 7,
			8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
			7, 8, 8, 8, 8, 8, 8, 8, 8, 8,
			8
		};

		public CodeBookMapType MapType { get; } = CodeBookMapType.Implicit;

		public int QuantMin { get; } = -531365888;

		public int QuantDelta { get; } = 1611661312;

		public int Quant { get; } = 4;

		public int QuantSequenceP { get; }

		public int[] QuantList { get; } = new int[11]
		{
			5, 4, 6, 3, 7, 2, 8, 1, 9, 0,
			10
		};
	}
}
