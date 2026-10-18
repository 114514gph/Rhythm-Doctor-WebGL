namespace OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter3
{
	public class Page8_1 : IStaticCodeBook
	{
		public int Dimensions { get; } = 2;

		public byte[] LengthList { get; } = new byte[25]
		{
			2, 4, 4, 5, 5, 6, 5, 5, 5, 5,
			6, 4, 5, 5, 5, 6, 5, 5, 5, 5,
			6, 6, 6, 5, 5
		};

		public CodeBookMapType MapType { get; } = CodeBookMapType.Implicit;

		public int QuantMin { get; } = -533725184;

		public int QuantDelta { get; } = 1611661312;

		public int Quant { get; } = 3;

		public int QuantSequenceP { get; }

		public int[] QuantList { get; } = new int[5] { 2, 1, 3, 0, 4 };
	}
}
