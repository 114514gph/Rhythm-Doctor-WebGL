using System;
using OggVorbisEncoder.Setup;

namespace OggVorbisEncoder.Lookups
{
	public class PsyGlobalLookup
	{
		private const int NegativeInfinite = -9999;

		private readonly PsyGlobal _psyGlobal;

		private float _ampMax;

		public float AmpMax
		{
			get
			{
				return _ampMax;
			}
			private set
			{
				_ampMax = Math.Max(-9999f, value);
			}
		}

		public PsyGlobalLookup(PsyGlobal global)
		{
			_psyGlobal = global;
			AmpMax = -9999f;
		}

		public void DecayAmpMax(int n, int sampleRate)
		{
			float num = (float)n / (float)sampleRate;
			AmpMax += num * _psyGlobal.AmpMaxAttPerSec;
		}
	}
}
