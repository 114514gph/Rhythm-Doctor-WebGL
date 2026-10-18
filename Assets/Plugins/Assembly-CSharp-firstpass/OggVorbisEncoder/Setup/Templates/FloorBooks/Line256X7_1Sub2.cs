namespace OggVorbisEncoder.Setup.Templates.FloorBooks
{
	public class Line256X7_1Sub2 : IStaticCodeBook
	{
		public int Dimensions { get; } = 1;

		public byte[] LengthList { get; } = new byte[25]
		{
			0, 0, 0, 0, 0, 0, 0, 0, 0, 2,
			3, 3, 4, 3, 4, 4, 5, 4, 6, 5,
			6, 7, 6, 8, 8
		};

		public CodeBookMapType MapType { get; }

		public int QuantMin { get; }

		public int QuantDelta { get; }

		public int Quant { get; }

		public int QuantSequenceP { get; }

		public int[] QuantList { get; }
	}
}
