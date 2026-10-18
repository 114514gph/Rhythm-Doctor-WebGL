namespace OggVorbisEncoder.Setup.Templates.FloorBooks
{
	public class Line256X4LowSub2 : IStaticCodeBook
	{
		public int Dimensions { get; } = 1;

		public byte[] LengthList { get; } = new byte[25]
		{
			0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
			3, 3, 3, 4, 3, 4, 4, 4, 4, 4,
			5, 5, 5, 6, 6
		};

		public CodeBookMapType MapType { get; }

		public int QuantMin { get; }

		public int QuantDelta { get; }

		public int Quant { get; }

		public int QuantSequenceP { get; }

		public int[] QuantList { get; }
	}
}
