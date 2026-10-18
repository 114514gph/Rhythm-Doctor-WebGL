namespace OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter7
{
	public class Page5_0 : IStaticCodeBook
	{
		public int Dimensions { get; } = 4;

		public byte[] LengthList { get; } = new byte[81]
		{
			1, 4, 4, 5, 7, 7, 6, 7, 7, 4,
			6, 7, 10, 10, 10, 10, 10, 9, 4, 6,
			6, 10, 10, 10, 10, 9, 10, 5, 10, 10,
			9, 11, 12, 10, 11, 12, 7, 10, 10, 11,
			12, 12, 12, 12, 12, 7, 10, 10, 11, 12,
			12, 12, 12, 12, 6, 10, 10, 10, 12, 12,
			11, 12, 12, 7, 10, 10, 12, 12, 12, 12,
			11, 12, 7, 10, 10, 11, 12, 12, 12, 12,
			12
		};

		public CodeBookMapType MapType { get; } = CodeBookMapType.Implicit;

		public int QuantMin { get; } = -529137664;

		public int QuantDelta { get; } = 1618345984;

		public int Quant { get; } = 2;

		public int QuantSequenceP { get; }

		public int[] QuantList { get; } = new int[3] { 1, 0, 2 };
	}
}
