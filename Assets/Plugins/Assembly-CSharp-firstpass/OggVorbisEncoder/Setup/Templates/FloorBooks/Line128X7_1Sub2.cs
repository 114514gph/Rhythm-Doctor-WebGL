namespace OggVorbisEncoder.Setup.Templates.FloorBooks
{
	public class Line128X7_1Sub2 : IStaticCodeBook
	{
		public int Dimensions { get; } = 1;

		public byte[] LengthList { get; } = new byte[25]
		{
			0, 0, 0, 0, 0, 0, 0, 0, 0, 3,
			4, 3, 6, 3, 6, 3, 6, 3, 7, 3,
			8, 4, 9, 4, 9
		};

		public CodeBookMapType MapType { get; }

		public int QuantMin { get; }

		public int QuantDelta { get; }

		public int Quant { get; }

		public int QuantSequenceP { get; }

		public int[] QuantList { get; }
	}
}
