namespace OggVorbisEncoder.Setup.Templates.FloorBooks
{
	public class Line128X11_1Sub0 : IStaticCodeBook
	{
		public int Dimensions { get; } = 1;

		public byte[] LengthList { get; } = new byte[32]
		{
			2, 5, 5, 5, 5, 5, 5, 4, 5, 5,
			5, 5, 5, 5, 5, 5, 6, 5, 6, 5,
			6, 5, 7, 6, 7, 6, 7, 6, 8, 6,
			8, 6
		};

		public CodeBookMapType MapType { get; }

		public int QuantMin { get; }

		public int QuantDelta { get; }

		public int Quant { get; }

		public int QuantSequenceP { get; }

		public int[] QuantList { get; }
	}
}
