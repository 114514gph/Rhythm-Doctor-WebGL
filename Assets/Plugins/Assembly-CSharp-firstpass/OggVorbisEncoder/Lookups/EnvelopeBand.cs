using System;

namespace OggVorbisEncoder.Lookups
{
	public class EnvelopeBand
	{
		public int Begin { get; }

		public float[] Window { get; }

		public float Total { get; }

		public EnvelopeBand(int begin, int windowLength)
		{
			Begin = begin;
			Window = new float[windowLength];
			for (int i = 0; i < Window.Length; i++)
			{
				Window[i] = (float)Math.Sin(((double)i + 0.5) / (double)Window.Length * Math.PI);
				Total += Window[i];
			}
			Total = (float)(1.0 / (double)Total);
		}
	}
}
