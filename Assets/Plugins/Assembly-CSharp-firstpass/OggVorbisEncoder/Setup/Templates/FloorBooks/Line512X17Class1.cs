namespace OggVorbisEncoder.Setup.Templates.FloorBooks
{
	public class Line512X17Class1 : IStaticCodeBook
	{
		public int Dimensions { get; } = 1;

		public byte[] LengthList { get; } = new byte[8] { 1, 2, 3, 6, 5, 4, 7, 7 };

		public CodeBookMapType MapType { get; }

		public int QuantMin { get; }

		public int QuantDelta { get; }

		public int Quant { get; }

		public int QuantSequenceP { get; }

		public int[] QuantList { get; }
	}
}
