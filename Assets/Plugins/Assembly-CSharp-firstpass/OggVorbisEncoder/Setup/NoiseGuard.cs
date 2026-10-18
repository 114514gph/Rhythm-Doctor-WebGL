namespace OggVorbisEncoder.Setup
{
	public class NoiseGuard
	{
		public int Low { get; }

		public int High { get; }

		public int Fixed { get; }

		public NoiseGuard(int low, int high, int fix)
		{
			Low = low;
			High = high;
			Fixed = fix;
		}
	}
}
