using System;
using System.IO;
using MP3Sharp.Decoding;

namespace MP3Sharp.IO
{
	internal class WaveFileBuffer : ABuffer
	{
		private readonly short[] m_Buffer;

		private readonly short[] m_Bufferp;

		private readonly int m_Channels;

		private readonly WaveFile m_OutWave;

		public WaveFileBuffer(int numberOfChannels, int freq, string fileName)
		{
			if (fileName == null)
			{
				throw new NullReferenceException("FileName");
			}
			m_Buffer = new short[2304];
			m_Bufferp = new short[2];
			m_Channels = numberOfChannels;
			for (int i = 0; i < numberOfChannels; i++)
			{
				m_Bufferp[i] = (short)i;
			}
			m_OutWave = new WaveFile();
			m_OutWave.OpenForWrite(fileName, null, freq, 16, (short)m_Channels);
		}

		public WaveFileBuffer(int numberOfChannels, int freq, Stream stream)
		{
			m_Buffer = new short[2304];
			m_Bufferp = new short[2];
			m_Channels = numberOfChannels;
			for (int i = 0; i < numberOfChannels; i++)
			{
				m_Bufferp[i] = (short)i;
			}
			m_OutWave = new WaveFile();
		}

		public override void Append(int channel, short valueRenamed)
		{
			m_Buffer[m_Bufferp[channel]] = valueRenamed;
			m_Bufferp[channel] = (short)(m_Bufferp[channel] + m_Channels);
		}

		public override void WriteBuffer(int val)
		{
			for (int i = 0; i < m_Channels; i++)
			{
				m_Bufferp[i] = (short)i;
			}
		}

		public void close(bool justWriteLengthBytes)
		{
			m_OutWave.Close(justWriteLengthBytes);
		}

		public override void Close()
		{
			m_OutWave.Close();
		}

		public override void ClearBuffer()
		{
		}

		public override void SetStopFlag()
		{
		}
	}
}
