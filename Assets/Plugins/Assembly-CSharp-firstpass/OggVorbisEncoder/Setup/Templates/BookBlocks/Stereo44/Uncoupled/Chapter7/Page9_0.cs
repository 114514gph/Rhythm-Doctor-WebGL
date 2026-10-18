namespace OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Uncoupled.Chapter7
{
	public class Page9_0 : IStaticCodeBook
	{
		public int Dimensions { get; } = 2;

		public byte[] LengthList { get; } = new byte[121]
		{
			1, 3, 3, 10, 10, 10, 10, 10, 10, 10,
			10, 4, 10, 10, 10, 10, 10, 10, 10, 10,
			10, 10, 4, 10, 10, 10, 10, 10, 10, 10,
			10, 10, 10, 10, 10, 10, 10, 10, 10, 10,
			10, 10, 10, 10, 10, 10, 10, 10, 10, 10,
			10, 10, 10, 10, 10, 10, 10, 10, 10, 10,
			10, 10, 10, 10, 10, 10, 10, 10, 10, 10,
			10, 10, 10, 10, 10, 10, 10, 10, 10, 10,
			10, 10, 10, 10, 10, 10, 10, 10, 10, 10,
			10, 10, 10, 10, 10, 10, 10, 10, 10, 10,
			10, 10, 10, 10, 10, 10, 10, 10, 10, 9,
			9, 9, 9, 9, 9, 9, 9, 9, 9, 9,
			9
		};

		public CodeBookMapType MapType { get; } = CodeBookMapType.Implicit;

		public int QuantMin { get; } = -512171520;

		public int QuantDelta { get; } = 1630791680;

		public int Quant { get; } = 4;

		public int QuantSequenceP { get; }

		public int[] QuantList { get; } = new int[11]
		{
			5, 4, 6, 3, 7, 2, 8, 1, 9, 0,
			10
		};
	}
}
