namespace OggVorbisEncoder.Setup.Templates.FloorBooks
{
	public class Line2048X27Class1 : IStaticCodeBook
	{
		public int Dimensions { get; } = 1;

		public byte[] LengthList { get; } = new byte[16]
		{
			2, 6, 8, 9, 7, 11, 13, 13, 1, 3,
			5, 5, 6, 6, 12, 10
		};

		public CodeBookMapType MapType { get; }

		public int QuantMin { get; }

		public int QuantDelta { get; }

		public int Quant { get; }

		public int QuantSequenceP { get; }

		public int[] QuantList { get; }
	}
}
