namespace OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo16.Coupled.Chapter0
{
	public class Page8_1 : IStaticCodeBook
	{
		public int Dimensions { get; } = 2;

		public byte[] LengthList { get; } = new byte[25]
		{
			1, 4, 3, 5, 5, 7, 7, 7, 6, 6,
			7, 7, 7, 5, 5, 7, 7, 7, 6, 6,
			7, 7, 7, 6, 6
		};

		public CodeBookMapType MapType { get; } = CodeBookMapType.Implicit;

		public int QuantMin { get; } = -533725184;

		public int QuantDelta { get; } = 1611661312;

		public int Quant { get; } = 3;

		public int QuantSequenceP { get; }

		public int[] QuantList { get; } = new int[5] { 2, 1, 3, 0, 4 };
	}
}
