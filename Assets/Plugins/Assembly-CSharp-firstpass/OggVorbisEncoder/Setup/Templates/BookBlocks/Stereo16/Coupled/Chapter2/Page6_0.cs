namespace OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo16.Coupled.Chapter2
{
	public class Page6_0 : IStaticCodeBook
	{
		public int Dimensions { get; } = 2;

		public byte[] LengthList { get; } = new byte[169]
		{
			1, 4, 4, 6, 6, 8, 7, 8, 8, 9,
			9, 10, 10, 5, 5, 5, 7, 7, 9, 9,
			9, 9, 11, 11, 12, 12, 6, 5, 5, 7,
			7, 9, 9, 10, 9, 11, 11, 12, 12, 0,
			7, 7, 7, 7, 9, 9, 10, 10, 11, 11,
			12, 12, 0, 7, 7, 7, 7, 9, 9, 10,
			10, 11, 11, 12, 12, 0, 11, 11, 8, 8,
			10, 10, 11, 11, 12, 12, 13, 13, 0, 12,
			12, 9, 9, 10, 10, 11, 11, 12, 12, 13,
			13, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			0, 0, 0, 0, 0, 0, 0, 0, 0
		};

		public CodeBookMapType MapType { get; } = CodeBookMapType.Implicit;

		public int QuantMin { get; } = -526516224;

		public int QuantDelta { get; } = 1616117760;

		public int Quant { get; } = 4;

		public int QuantSequenceP { get; }

		public int[] QuantList { get; } = new int[13]
		{
			6, 5, 7, 4, 8, 3, 9, 2, 10, 1,
			11, 0, 12
		};
	}
}
