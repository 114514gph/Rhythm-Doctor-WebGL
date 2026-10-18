namespace OggVorbisEncoder.Setup.Templates.FloorBooks
{
	public class Line128X11Class1 : IStaticCodeBook
	{
		public int Dimensions { get; } = 1;

		public byte[] LengthList { get; } = new byte[8] { 1, 6, 3, 7, 2, 4, 5, 7 };

		public CodeBookMapType MapType { get; }

		public int QuantMin { get; }

		public int QuantDelta { get; }

		public int Quant { get; }

		public int QuantSequenceP { get; }

		public int[] QuantList { get; }
	}
}
