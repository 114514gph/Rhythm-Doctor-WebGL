using System;
using MP3Sharp.Decoding;

namespace MP3Sharp
{
	internal class Buffer16BitStereo : ABuffer
	{
		private static readonly int CHANNELS = 2;

		private readonly byte[] m_Buffer = new byte[4608];

		private readonly int[] m_Bufferp = new int[2];

		private int m_End;

		private int m_Offset;

		public int BytesLeft => m_End - m_Offset;

		public Buffer16BitStereo()
		{
			ClearBuffer();
		}

		public int Read(byte[] bufferOut, int offset, int count)
		{
			if (bufferOut == null)
			{
				throw new ArgumentNullException("bufferOut");
			}
			if (count + offset > bufferOut.Length)
			{
				throw new ArgumentException("The sum of offset and count is larger than the buffer length");
			}
			int bytesLeft = BytesLeft;
			int num;
			if (count > bytesLeft)
			{
				num = bytesLeft;
			}
			else
			{
				int num2 = count % (2 * CHANNELS);
				num = count - num2;
			}
			Array.Copy(m_Buffer, m_Offset, bufferOut, offset, num);
			m_Offset += num;
			return num;
		}

		public override void Append(int channel, short sampleValue)
		{
			m_Buffer[m_Bufferp[channel]] = (byte)(sampleValue & 0xFF);
			m_Buffer[m_Bufferp[channel] + 1] = (byte)(sampleValue >> 8);
			m_Bufferp[channel] += CHANNELS * 2;
		}

		public override void AppendSamples(int channel, float[] samples)
		{
			if (samples == null)
			{
				throw new ArgumentNullException("samples");
			}
			if (samples.Length < 32)
			{
				throw new ArgumentException("samples must have 32 values");
			}
			int num = m_Bufferp[channel];
			for (int i = 0; i < 32; i++)
			{
				float num2 = samples[i];
				if (num2 > 32767f)
				{
					num2 = 32767f;
				}
				else if (num2 < -32767f)
				{
					num2 = -32767f;
				}
				int num3 = (int)num2;
				m_Buffer[num] = (byte)(num3 & 0xFF);
				m_Buffer[num + 1] = (byte)(num3 >> 8);
				num += CHANNELS * 2;
			}
			m_Bufferp[channel] = num;
		}

		public sealed override void ClearBuffer()
		{
			m_Offset = 0;
			m_End = 0;
			for (int i = 0; i < CHANNELS; i++)
			{
				m_Bufferp[i] = i * 2;
			}
		}

		public override void SetStopFlag()
		{
		}

		public override void WriteBuffer(int val)
		{
			m_Offset = 0;
			m_End = m_Bufferp[0];
		}

		public override void Close()
		{
		}
	}
}
