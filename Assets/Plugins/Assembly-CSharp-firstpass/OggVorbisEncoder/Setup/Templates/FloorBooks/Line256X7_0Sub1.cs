namespace OggVorbisEncoder.Setup.Templates.FloorBooks
{
	public class Line256X7_0Sub1 : IStaticCodeBook
	{
		public int Dimensions { get; } = 1;

		public byte[] LengthList { get; } = new byte[9] { 0, 2, 3, 3, 3, 3, 4, 3, 4 };

		public CodeBookMapType MapType { get; }

		public int QuantMin { get; }

		public int QuantDelta { get; }

		public int Quant { get; }

		public int QuantSequenceP { get; }

		public int[] QuantList { get; }
	}
}
