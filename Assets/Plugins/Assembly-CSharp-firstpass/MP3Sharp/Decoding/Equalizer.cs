using System;

namespace MP3Sharp.Decoding
{
	internal class Equalizer
	{
		internal abstract class EQFunction
		{
			public virtual float getBand(int band)
			{
				return 0f;
			}
		}

		private const int BANDS = 32;

		public const float BAND_NOT_PRESENT = float.NegativeInfinity;

		public static readonly Equalizer PASS_THRU_EQ = new Equalizer();

		private float[] settings;

		public float[] FromFloatArray
		{
			set
			{
				reset();
				int num = ((value.Length > 32) ? 32 : value.Length);
				for (int i = 0; i < num; i++)
				{
					settings[i] = limit(value[i]);
				}
			}
		}

		public virtual Equalizer FromEqualizer
		{
			set
			{
				if (value != this)
				{
					FromFloatArray = value.settings;
				}
			}
		}

		public EQFunction FromEQFunction
		{
			set
			{
				reset();
				int num = 32;
				for (int i = 0; i < num; i++)
				{
					settings[i] = limit(value.getBand(i));
				}
			}
		}

		public virtual int BandCount => settings.Length;

		internal virtual float[] BandFactors
		{
			get
			{
				float[] array = new float[32];
				int i = 0;
				for (int num = 32; i < num; i++)
				{
					array[i] = getBandFactor(settings[i]);
				}
				return array;
			}
		}

		public Equalizer()
		{
			InitBlock();
		}

		public Equalizer(float[] settings)
		{
			InitBlock();
			FromFloatArray = settings;
		}

		public Equalizer(EQFunction eq)
		{
			InitBlock();
			FromEQFunction = eq;
		}

		private void InitBlock()
		{
			settings = new float[32];
		}

		public void reset()
		{
			for (int i = 0; i < 32; i++)
			{
				settings[i] = 0f;
			}
		}

		public float setBand(int band, float neweq)
		{
			float result = 0f;
			if (band >= 0 && band < 32)
			{
				result = settings[band];
				settings[band] = limit(neweq);
			}
			return result;
		}

		public float getBand(int band)
		{
			float result = 0f;
			if (band >= 0 && band < 32)
			{
				result = settings[band];
			}
			return result;
		}

		private float limit(float eq)
		{
			if (eq == float.NegativeInfinity)
			{
				return eq;
			}
			if (eq > 1f)
			{
				return 1f;
			}
			if (eq < -1f)
			{
				return -1f;
			}
			return eq;
		}

		internal float getBandFactor(float eq)
		{
			if (eq == float.NegativeInfinity)
			{
				return 0f;
			}
			return (float)Math.Pow(2.0, eq);
		}
	}
}
