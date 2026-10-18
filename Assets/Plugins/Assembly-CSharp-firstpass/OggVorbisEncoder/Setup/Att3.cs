namespace OggVorbisEncoder.Setup
{
	public class Att3
	{
		public int[] Att { get; }

		public float Boost { get; }

		public float Decay { get; }

		public Att3(int[] att, float boost, float decay)
		{
			Att = att;
			Boost = boost;
			Decay = decay;
		}
	}
}
