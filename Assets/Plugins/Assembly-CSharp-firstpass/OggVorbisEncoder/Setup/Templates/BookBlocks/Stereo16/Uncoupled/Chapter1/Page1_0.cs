namespace OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo16.Uncoupled.Chapter1
{
	public class Page1_0 : IStaticCodeBook
	{
		public int Dimensions { get; } = 4;

		public byte[] LengthList { get; } = new byte[81]
		{
			1, 4, 4, 5, 7, 7, 5, 7, 7, 5,
			8, 7, 7, 10, 10, 7, 9, 10, 5, 7,
			8, 7, 10, 9, 7, 10, 10, 5, 8, 8,
			8, 10, 10, 8, 10, 10, 7, 10, 10, 10,
			11, 12, 10, 12, 13, 7, 10, 10, 9, 13,
			11, 10, 12, 13, 5, 8, 8, 8, 10, 10,
			8, 10, 10, 7, 10, 10, 10, 12, 12, 9,
			11, 12, 7, 10, 11, 10, 12, 12, 10, 13,
			11
		};

		public CodeBookMapType MapType { get; } = CodeBookMapType.Implicit;

		public int QuantMin { get; } = -535822336;

		public int QuantDelta { get; } = 1611661312;

		public int Quant { get; } = 2;

		public int QuantSequenceP { get; }

		public int[] QuantList { get; } = new int[3] { 1, 0, 2 };
	}
}
