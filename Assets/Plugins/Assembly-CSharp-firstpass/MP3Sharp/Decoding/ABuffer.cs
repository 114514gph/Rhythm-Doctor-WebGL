namespace MP3Sharp.Decoding
{
	internal abstract class ABuffer
	{
		public const int OBUFFERSIZE = 2304;

		public const int MAXCHANNELS = 2;

		public abstract void Append(int channel, short valueRenamed);

		public virtual void AppendSamples(int channel, float[] f)
		{
			for (int i = 0; i < 32; i++)
			{
				Append(channel, Clip(f[i]));
			}
		}

		private static short Clip(float sample)
		{
			if (!(sample > 32767f))
			{
				if (!(sample < -32768f))
				{
					return (short)sample;
				}
				return short.MinValue;
			}
			return short.MaxValue;
		}

		public abstract void WriteBuffer(int val);

		public abstract void Close();

		public abstract void ClearBuffer();

		public abstract void SetStopFlag();
	}
}
