namespace OggVorbisEncoder.Setup.Templates.FloorBooks
{
	public class Line1024X27Class1 : IStaticCodeBook
	{
		public int Dimensions { get; } = 1;

		public byte[] LengthList { get; } = new byte[16]
		{
			2, 10, 8, 14, 7, 12, 11, 14, 1, 5,
			3, 7, 4, 9, 7, 13
		};

		public CodeBookMapType MapType { get; }

		public int QuantMin { get; }

		public int QuantDelta { get; }

		public int Quant { get; }

		public int QuantSequenceP { get; }

		public int[] QuantList { get; }
	}
}
