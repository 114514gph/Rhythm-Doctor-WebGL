namespace OggVorbisEncoder.Setup
{
	public interface IStaticCodeBook
	{
		int Dimensions { get; }

		byte[] LengthList { get; }

		CodeBookMapType MapType { get; }

		int QuantMin { get; }

		int QuantDelta { get; }

		int Quant { get; }

		int QuantSequenceP { get; }

		int[] QuantList { get; }
	}
}
