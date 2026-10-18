namespace OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Uncoupled.Chapter1
{
	public class Page7_0 : IStaticCodeBook
	{
		public int Dimensions { get; } = 2;

		public byte[] LengthList { get; } = new byte[49]
		{
			1, 3, 2, 9, 9, 7, 9, 9, 9, 9,
			9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
			9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
			9, 9, 9, 9, 8, 8, 8, 8, 8, 8,
			8, 8, 8, 8, 8, 8, 8, 8, 8
		};

		public CodeBookMapType MapType { get; } = CodeBookMapType.Implicit;

		public int QuantMin { get; } = -518017024;

		public int QuantDelta { get; } = 1626677248;

		public int Quant { get; } = 3;

		public int QuantSequenceP { get; }

		public int[] QuantList { get; } = new int[7] { 3, 2, 4, 1, 5, 0, 6 };
	}
}
