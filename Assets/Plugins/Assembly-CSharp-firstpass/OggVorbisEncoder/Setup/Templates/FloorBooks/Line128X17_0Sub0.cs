namespace OggVorbisEncoder.Setup.Templates.FloorBooks
{
	public class Line128X17_0Sub0 : IStaticCodeBook
	{
		public int Dimensions { get; } = 1;

		public byte[] LengthList { get; } = new byte[128]
		{
			5, 5, 6, 5, 6, 5, 6, 5, 6, 5,
			6, 5, 6, 5, 6, 5, 7, 5, 7, 5,
			7, 5, 7, 5, 7, 5, 7, 5, 8, 5,
			8, 5, 8, 5, 8, 5, 8, 6, 8, 6,
			8, 6, 9, 6, 9, 6, 9, 6, 9, 6,
			9, 7, 9, 7, 9, 7, 9, 7, 10, 7,
			10, 8, 10, 8, 10, 8, 10, 8, 10, 8,
			11, 8, 11, 8, 11, 8, 11, 8, 11, 9,
			12, 9, 12, 9, 12, 9, 12, 9, 12, 10,
			12, 10, 13, 11, 13, 11, 14, 12, 14, 13,
			15, 14, 16, 14, 17, 15, 18, 16, 20, 20,
			20, 20, 20, 20, 20, 20, 20, 20, 20, 20,
			20, 20, 20, 20, 20, 20, 20, 20
		};

		public CodeBookMapType MapType { get; }

		public int QuantMin { get; }

		public int QuantDelta { get; }

		public int Quant { get; }

		public int QuantSequenceP { get; }

		public int[] QuantList { get; }
	}
}
