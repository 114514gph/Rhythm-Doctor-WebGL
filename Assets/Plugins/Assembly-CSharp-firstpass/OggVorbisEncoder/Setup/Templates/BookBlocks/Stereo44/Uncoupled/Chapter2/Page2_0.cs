namespace OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Uncoupled.Chapter2
{
	public class Page2_0 : IStaticCodeBook
	{
		public int Dimensions { get; } = 4;

		public byte[] LengthList { get; } = new byte[81]
		{
			2, 5, 5, 5, 6, 6, 5, 6, 6, 5,
			6, 6, 7, 8, 8, 6, 8, 8, 5, 6,
			6, 6, 8, 7, 7, 8, 8, 5, 6, 6,
			7, 8, 8, 6, 8, 8, 6, 8, 8, 8,
			9, 10, 8, 10, 10, 6, 8, 8, 7, 10,
			8, 8, 10, 10, 5, 6, 6, 6, 8, 8,
			7, 8, 8, 6, 8, 8, 8, 10, 10, 8,
			8, 10, 6, 8, 8, 8, 10, 10, 8, 10,
			9
		};

		public CodeBookMapType MapType { get; } = CodeBookMapType.Implicit;

		public int QuantMin { get; } = -535822336;

		public int QuantDelta { get; } = 1611661312;

		public int Quant { get; } = 2;

		public int QuantSequenceP { get; }

		public int[] QuantList { get; } = new int[3] { 1, 0, 2 };
	}
}
