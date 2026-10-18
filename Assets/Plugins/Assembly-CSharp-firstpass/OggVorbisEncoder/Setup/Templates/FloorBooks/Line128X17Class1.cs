namespace OggVorbisEncoder.Setup.Templates.FloorBooks
{
	public class Line128X17Class1 : IStaticCodeBook
	{
		public int Dimensions { get; } = 1;

		public byte[] LengthList { get; } = new byte[8] { 1, 3, 4, 7, 2, 5, 6, 7 };

		public CodeBookMapType MapType { get; }

		public int QuantMin { get; }

		public int QuantDelta { get; }

		public int Quant { get; }

		public int QuantSequenceP { get; }

		public int[] QuantList { get; }
	}
}
