namespace OggVorbisEncoder.Setup.Templates.FloorBooks
{
	public class Line2048X27Class2 : IStaticCodeBook
	{
		public int Dimensions { get; } = 1;

		public byte[] LengthList { get; } = new byte[8] { 1, 2, 3, 6, 4, 7, 5, 7 };

		public CodeBookMapType MapType { get; }

		public int QuantMin { get; }

		public int QuantDelta { get; }

		public int Quant { get; }

		public int QuantSequenceP { get; }

		public int[] QuantList { get; }
	}
}
