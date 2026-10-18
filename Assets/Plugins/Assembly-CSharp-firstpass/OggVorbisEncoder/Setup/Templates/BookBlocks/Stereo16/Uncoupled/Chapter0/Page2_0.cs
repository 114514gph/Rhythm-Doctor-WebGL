namespace OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo16.Uncoupled.Chapter0
{
	public class Page2_0 : IStaticCodeBook
	{
		public int Dimensions { get; } = 4;

		public byte[] LengthList { get; } = new byte[81]
		{
			2, 4, 4, 5, 6, 6, 5, 6, 6, 5,
			7, 7, 7, 8, 9, 7, 8, 9, 5, 7,
			7, 7, 9, 8, 7, 9, 7, 4, 7, 7,
			7, 9, 9, 7, 8, 8, 6, 9, 8, 7,
			8, 11, 9, 11, 10, 6, 8, 9, 8, 11,
			8, 9, 10, 11, 4, 7, 7, 7, 8, 8,
			7, 9, 9, 6, 9, 8, 9, 11, 10, 8,
			8, 11, 6, 8, 9, 9, 10, 11, 8, 11,
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
