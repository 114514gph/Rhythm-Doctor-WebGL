namespace OggVorbisEncoder.Setup.Templates.FloorBooks
{
	public class Line512X17_1Sub0 : IStaticCodeBook
	{
		public int Dimensions { get; } = 1;

		public byte[] LengthList { get; } = new byte[32]
		{
			2, 4, 5, 4, 5, 4, 5, 4, 5, 5,
			5, 5, 5, 5, 6, 5, 6, 5, 6, 6,
			7, 6, 7, 6, 8, 7, 8, 7, 8, 7,
			8, 7
		};

		public CodeBookMapType MapType { get; }

		public int QuantMin { get; }

		public int QuantDelta { get; }

		public int Quant { get; }

		public int QuantSequenceP { get; }

		public int[] QuantList { get; }
	}
}
