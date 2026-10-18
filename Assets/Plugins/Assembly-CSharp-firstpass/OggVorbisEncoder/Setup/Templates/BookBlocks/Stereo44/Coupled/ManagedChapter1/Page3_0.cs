namespace OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.ManagedChapter1
{
	public class Page3_0 : IStaticCodeBook
	{
		public int Dimensions { get; } = 2;

		public byte[] LengthList { get; } = new byte[81]
		{
			1, 3, 3, 7, 7, 0, 0, 0, 0, 0,
			5, 5, 6, 6, 0, 0, 0, 0, 0, 5,
			5, 7, 7, 0, 0, 0, 0, 0, 7, 7,
			7, 7, 0, 0, 0, 0, 0, 0, 0, 7,
			7, 0, 0, 0, 0, 0, 0, 0, 8, 9,
			0, 0, 0, 0, 0, 0, 0, 8, 8, 0,
			0, 0, 0, 0, 0, 0, 10, 10, 0, 0,
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
