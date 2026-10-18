namespace OggVorbisEncoder.Setup.Templates.FloorBooks
{
	public class Line128X17_2Sub1 : IStaticCodeBook
	{
		public int Dimensions { get; } = 1;

		public byte[] LengthList { get; } = new byte[18]
		{
			0, 4, 5, 4, 6, 4, 8, 3, 9, 3,
			9, 2, 9, 3, 8, 4, 9, 4
		};

		public CodeBookMapType MapType { get; }

		public int QuantMin { get; }

		public int QuantDelta { get; }

		public int Quant { get; }

		public int QuantSequenceP { get; }

		public int[] QuantList { get; }
	}
}
