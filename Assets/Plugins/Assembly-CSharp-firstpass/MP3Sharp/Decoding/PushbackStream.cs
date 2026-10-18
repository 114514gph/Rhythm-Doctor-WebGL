using System;
using System.IO;

namespace MP3Sharp.Decoding
{
	internal class PushbackStream
	{
		private readonly int m_BackBufferSize;

		private readonly CircularByteBuffer m_CircularByteBuffer;

		private readonly Stream m_Stream;

		private readonly byte[] m_TemporaryBuffer;

		private int m_NumForwardBytesInBuffer;

		public PushbackStream(Stream s, int backBufferSize)
		{
			m_Stream = s;
			m_BackBufferSize = backBufferSize;
			m_TemporaryBuffer = new byte[m_BackBufferSize];
			m_CircularByteBuffer = new CircularByteBuffer(m_BackBufferSize);
		}

		public int Read(sbyte[] toRead, int offset, int length)
		{
			int num = 0;
			bool flag = true;
			while ((num < length) & flag)
			{
				if (m_NumForwardBytesInBuffer > 0)
				{
					m_NumForwardBytesInBuffer--;
					toRead[offset + num] = (sbyte)m_CircularByteBuffer[m_NumForwardBytesInBuffer];
					num++;
					continue;
				}
				int num2 = length - num;
				int num3 = m_Stream.Read(m_TemporaryBuffer, 0, num2);
				flag = num3 >= num2;
				for (int i = 0; i < num3; i++)
				{
					m_CircularByteBuffer.Push(m_TemporaryBuffer[i]);
					toRead[offset + num + i] = (sbyte)m_TemporaryBuffer[i];
				}
				num += num3;
			}
			return num;
		}

		public void UnRead(int length)
		{
			m_NumForwardBytesInBuffer += length;
			if (m_NumForwardBytesInBuffer > m_BackBufferSize)
			{
				throw new Exception("The backstream cannot unread the requested number of bytes.");
			}
		}

		public void Close()
		{
			m_Stream.Close();
		}
	}
}
