namespace MP3Sharp.Decoding
{
	internal class SampleBuffer : ABuffer
	{
		private readonly short[] buffer;

		private readonly int[] bufferp;

		private readonly int channels;

		private readonly int frequency;

		public virtual int ChannelCount => channels;

		public virtual int SampleFrequency => frequency;

		public virtual short[] Buffer => buffer;

		public virtual int BufferLength => bufferp[0];

		public SampleBuffer(int sample_frequency, int number_of_channels)
		{
			buffer = new short[2304];
			bufferp = new int[2];
			channels = number_of_channels;
			frequency = sample_frequency;
			for (int i = 0; i < number_of_channels; i++)
			{
				bufferp[i] = (short)i;
			}
		}

		public override void Append(int channel, short valueRenamed)
		{
			buffer[bufferp[channel]] = valueRenamed;
			bufferp[channel] += channels;
		}

		public override void AppendSamples(int channel, float[] f)
		{
			int num = bufferp[channel];
			int num2 = 0;
			while (num2 < 32)
			{
				float num3 = f[num2++];
				num3 = ((num3 > 32767f) ? 32767f : ((num3 < -32767f) ? (-32767f) : num3));
				short num4 = (short)num3;
				buffer[num] = num4;
				num += channels;
			}
			bufferp[channel] = num;
		}

		public override void WriteBuffer(int val)
		{
		}

		public override void Close()
		{
		}

		public override void ClearBuffer()
		{
			for (int i = 0; i < channels; i++)
			{
				bufferp[i] = (short)i;
			}
		}

		public override void SetStopFlag()
		{
		}
	}
}
