namespace OggVorbisEncoder.Setup.Templates.FloorBooks
{
	public class Line256X7_0Sub2 : IStaticCodeBook
	{
		public int Dimensions { get; } = 1;

		public byte[] LengthList { get; } = new byte[25]
		{
			0, 0, 0, 0, 0, 0, 0, 0, 0, 3,
			4, 3, 4, 3, 5, 3, 6, 3, 6, 4,
			6, 4, 7, 5, 7
		};

		public CodeBookMapType MapType { get; }

		public int QuantMin { get; }

		public int QuantDelta { get; }

		public int Quant { get; }

		public int QuantSequenceP { get; }

		public int[] QuantList { get; }
	}
}
