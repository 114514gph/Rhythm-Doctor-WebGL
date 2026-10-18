namespace OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Coupled.Chapter8
{
	public class Page1_0 : IStaticCodeBook
	{
		public int Dimensions { get; } = 4;

		public byte[] LengthList { get; } = new byte[81]
		{
			1, 5, 5, 0, 5, 5, 0, 5, 5, 5,
			7, 7, 0, 9, 8, 0, 9, 8, 6, 7,
			7, 0, 8, 9, 0, 8, 9, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 5, 9, 8, 0,
			8, 8, 0, 8, 8, 5, 8, 9, 0, 8,
			8, 0, 8, 8, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 5, 9, 8, 0, 8, 8, 0,
			8, 8, 5, 8, 9, 0, 8, 8, 0, 8,
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
