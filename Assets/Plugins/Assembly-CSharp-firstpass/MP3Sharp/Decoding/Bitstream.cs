using System;
using System.IO;
using MP3Sharp.Support;

namespace MP3Sharp.Decoding
{
	internal sealed class Bitstream
	{
		private const int BUFFER_INT_SIZE = 433;

		internal static sbyte INITIAL_SYNC = 0;

		internal static sbyte STRICT_SYNC = 1;

		private readonly int[] bitmask = new int[18]
		{
			0, 1, 3, 7, 15, 31, 63, 127, 255, 511,
			1023, 2047, 4095, 8191, 16383, 32767, 65535, 131071
		};

		private readonly PushbackStream m_SourceStream;

		private int m_BitIndex;

		private Crc16[] m_CRC;

		private sbyte[] m_FrameBytes;

		private int[] m_FrameBuffer;

		private int m_FrameSize;

		private Header m_Header;

		private bool single_ch_mode;

		private sbyte[] m_SyncBuffer;

		private int m_SyncWord;

		private int m_WordPointer;

		internal Bitstream(PushbackStream stream)
		{
			InitBlock();
			if (stream == null)
			{
				throw new NullReferenceException("in stream is null");
			}
			m_SourceStream = stream;
			CloseFrame();
		}

		private void InitBlock()
		{
			m_CRC = new Crc16[1];
			m_SyncBuffer = new sbyte[4];
			m_FrameBytes = new sbyte[1732];
			m_FrameBuffer = new int[433];
			m_Header = new Header();
		}

		public void close()
		{
			try
			{
				m_SourceStream.Close();
			}
			catch (IOException throwable)
			{
				throw newBitstreamException(BitstreamErrors.STREAM_ERROR, throwable);
			}
		}

		internal Header readFrame()
		{
			Header result = null;
			try
			{
				result = readNextFrame();
			}
			catch (BitstreamException ex)
			{
				if (ex.ErrorCode != BitstreamErrors.STREAM_EOF)
				{
					throw newBitstreamException(ex.ErrorCode, ex);
				}
			}
			return result;
		}

		private Header readNextFrame()
		{
			if (m_FrameSize == -1)
			{
				m_Header.read_header(this, m_CRC);
			}
			return m_Header;
		}

		public void unreadFrame()
		{
			if (m_WordPointer == -1 && m_BitIndex == -1 && m_FrameSize > 0)
			{
				try
				{
					m_SourceStream.UnRead(m_FrameSize);
				}
				catch
				{
					throw newBitstreamException(BitstreamErrors.STREAM_ERROR);
				}
			}
		}

		public void CloseFrame()
		{
			m_FrameSize = -1;
			m_WordPointer = -1;
			m_BitIndex = -1;
		}

		public bool IsSyncCurrentPosition(int syncmode)
		{
			int num = readBytes(m_SyncBuffer, 0, 4);
			int headerstring = ((m_SyncBuffer[0] << 24) & (int)SupportClass.Identity(4278190080L)) | ((m_SyncBuffer[1] << 16) & 0xFF0000) | ((m_SyncBuffer[2] << 8) & 0xFF00) | (m_SyncBuffer[3] & 0xFF);
			try
			{
				m_SourceStream.UnRead(num);
			}
			catch
			{
			}
			bool result = false;
			switch (num)
			{
			case 0:
				result = true;
				break;
			case 4:
				result = isSyncMark(headerstring, syncmode, m_SyncWord);
				break;
			}
			return result;
		}

		public int readBits(int n)
		{
			return GetBitsFromBuffer(n);
		}

		public int readCheckedBits(int n)
		{
			return GetBitsFromBuffer(n);
		}

		internal BitstreamException newBitstreamException(int errorcode)
		{
			return new BitstreamException(errorcode, null);
		}

		internal BitstreamException newBitstreamException(int errorcode, Exception throwable)
		{
			return new BitstreamException(errorcode, throwable);
		}

		internal int syncHeader(sbyte syncmode)
		{
			if (readBytes(m_SyncBuffer, 0, 3) != 3)
			{
				throw newBitstreamException(BitstreamErrors.STREAM_EOF, null);
			}
			int num = ((m_SyncBuffer[0] << 16) & 0xFF0000) | ((m_SyncBuffer[1] << 8) & 0xFF00) | (m_SyncBuffer[2] & 0xFF);
			do
			{
				num <<= 8;
				if (readBytes(m_SyncBuffer, 3, 1) != 1)
				{
					throw newBitstreamException(BitstreamErrors.STREAM_EOF, null);
				}
				num |= m_SyncBuffer[3] & 0xFF;
			}
			while (!isSyncMark(num, syncmode, m_SyncWord));
			return num;
		}

		public bool isSyncMark(int headerstring, int syncmode, int word)
		{
			bool flag = false;
			flag = ((syncmode != INITIAL_SYNC) ? ((headerstring & 0xFFE00000u) == 4292870144u && (headerstring & 0xC0) == 192 == single_ch_mode) : ((headerstring & 0xFFE00000u) == 4292870144u));
			if (flag)
			{
				flag = (SupportClass.URShift(headerstring, 10) & 3) != 3;
			}
			if (flag)
			{
				flag = (SupportClass.URShift(headerstring, 17) & 3) != 0;
			}
			if (flag)
			{
				flag = (SupportClass.URShift(headerstring, 19) & 3) != 1;
				if (!flag)
				{
					Console.WriteLine("INVALID VERSION DETECTED");
				}
			}
			return flag;
		}

		internal void read_frame_data(int bytesize)
		{
			readFully(m_FrameBytes, 0, bytesize);
			m_FrameSize = bytesize;
			m_WordPointer = -1;
			m_BitIndex = -1;
		}

		internal void ParseFrame()
		{
			int num = 0;
			sbyte[] frameBytes = m_FrameBytes;
			int frameSize = m_FrameSize;
			for (int i = 0; i < frameSize; i += 4)
			{
				sbyte b = 0;
				sbyte b2 = 0;
				sbyte b3 = 0;
				sbyte b4 = 0;
				b = frameBytes[i];
				if (i + 1 < frameSize)
				{
					b2 = frameBytes[i + 1];
				}
				if (i + 2 < frameSize)
				{
					b3 = frameBytes[i + 2];
				}
				if (i + 3 < frameSize)
				{
					b4 = frameBytes[i + 3];
				}
				m_FrameBuffer[num++] = ((b << 24) & (int)SupportClass.Identity(4278190080L)) | ((b2 << 16) & 0xFF0000) | ((b3 << 8) & 0xFF00) | (b4 & 0xFF);
			}
			m_WordPointer = 0;
			m_BitIndex = 0;
		}

		public int GetBitsFromBuffer(int countBits)
		{
			int num = m_BitIndex + countBits;
			if (m_WordPointer < 0)
			{
				m_WordPointer = 0;
			}
			if (num <= 32)
			{
				int result = SupportClass.URShift(m_FrameBuffer[m_WordPointer], 32 - num) & bitmask[countBits];
				if ((m_BitIndex += countBits) == 32)
				{
					m_BitIndex = 0;
					m_WordPointer++;
				}
				return result;
			}
			int num2 = m_FrameBuffer[m_WordPointer] & 0xFFFF;
			m_WordPointer++;
			int number = m_FrameBuffer[m_WordPointer] & (int)SupportClass.Identity(4294901760L);
			int result2 = SupportClass.URShift(((num2 << 16) & (int)SupportClass.Identity(4294901760L)) | (SupportClass.URShift(number, 16) & 0xFFFF), 48 - num) & bitmask[countBits];
			m_BitIndex = num - 32;
			return result2;
		}

		internal void SetSyncWord(int syncword0)
		{
			m_SyncWord = syncword0 & -193;
			single_ch_mode = (syncword0 & 0xC0) == 192;
		}

		private void readFully(sbyte[] b, int offs, int len)
		{
			try
			{
				while (len > 0)
				{
					int num = m_SourceStream.Read(b, offs, len);
					if (num == -1 || num == 0)
					{
						while (len-- > 0)
						{
							b[offs++] = 0;
						}
						break;
					}
					offs += num;
					len -= num;
				}
			}
			catch (IOException throwable)
			{
				throw newBitstreamException(BitstreamErrors.STREAM_ERROR, throwable);
			}
		}

		private int readBytes(sbyte[] b, int offs, int len)
		{
			int num = 0;
			try
			{
				while (len > 0)
				{
					int num2 = m_SourceStream.Read(b, offs, len);
					if (num2 != -1 && num2 != 0)
					{
						num += num2;
						offs += num2;
						len -= num2;
						continue;
					}
					break;
				}
			}
			catch (IOException throwable)
			{
				throw newBitstreamException(BitstreamErrors.STREAM_ERROR, throwable);
			}
			return num;
		}
	}
}
