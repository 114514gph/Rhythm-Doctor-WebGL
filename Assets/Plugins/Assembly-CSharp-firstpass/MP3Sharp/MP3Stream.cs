using System;
using System.IO;
using MP3Sharp.Decoding;

namespace MP3Sharp
{
	public class MP3Stream : Stream
	{
		private readonly Bitstream m_BitStream;

		private readonly Decoder m_Decoder = new Decoder(Decoder.DefaultParams);

		private readonly Buffer16BitStereo m_Buffer;

		private readonly Stream m_SourceStream;

		private readonly int m_BackStreamByteCountRep;

		private short m_ChannelCountRep = -1;

		protected SoundFormat FormatRep;

		private int m_FrequencyRep = -1;

		public bool IsEOF { get; protected set; }

		public int ChunkSize => m_BackStreamByteCountRep;

		public override bool CanRead => m_SourceStream.CanRead;

		public override bool CanSeek => m_SourceStream.CanSeek;

		public override bool CanWrite => m_SourceStream.CanWrite;

		public override long Length => m_SourceStream.Length;

		public override long Position
		{
			get
			{
				return m_SourceStream.Position;
			}
			set
			{
				m_SourceStream.Position = value;
			}
		}

		public int Frequency => m_FrequencyRep;

		public short ChannelCount => m_ChannelCountRep;

		public SoundFormat Format => FormatRep;

		public MP3Stream(string fileName)
			: this(new FileStream(fileName, FileMode.Open))
		{
		}

		public MP3Stream(string fileName, int chunkSize)
			: this(new FileStream(fileName, FileMode.Open), chunkSize)
		{
		}

		public MP3Stream(Stream sourceStream)
			: this(sourceStream, 4096)
		{
		}

		public MP3Stream(Stream sourceStream, int chunkSize)
		{
			IsEOF = false;
			FormatRep = SoundFormat.Pcm16BitStereo;
			m_SourceStream = sourceStream;
			m_BitStream = new Bitstream(new PushbackStream(m_SourceStream, chunkSize));
			m_Buffer = new Buffer16BitStereo();
			m_Decoder.OutputBuffer = m_Buffer;
			if (!ReadFrame())
			{
				IsEOF = true;
			}
		}

		public override void Flush()
		{
			m_SourceStream.Flush();
		}

		public override long Seek(long pos, SeekOrigin origin)
		{
			return m_SourceStream.Seek(pos, origin);
		}

		public override void SetLength(long len)
		{
			throw new InvalidOperationException();
		}

		public override void Write(byte[] buf, int ofs, int count)
		{
			throw new InvalidOperationException();
		}

		public int DecodeFrames(int frameCount)
		{
			int num = 0;
			bool flag = true;
			while ((num < frameCount) & flag)
			{
				flag = ReadFrame();
				if (flag)
				{
					num++;
				}
			}
			return num;
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (IsEOF)
			{
				return 0;
			}
			int num = 0;
			do
			{
				if (m_Buffer.BytesLeft <= 0 && !ReadFrame())
				{
					IsEOF = true;
					break;
				}
				num += m_Buffer.Read(buffer, offset + num, count - num);
			}
			while (num < count);
			return num;
		}

		public override void Close()
		{
			m_BitStream.close();
		}

		private bool ReadFrame()
		{
			Header header = m_BitStream.readFrame();
			if (header == null)
			{
				return false;
			}
			try
			{
				if (header.mode() == 3)
				{
					m_ChannelCountRep = 1;
				}
				else
				{
					m_ChannelCountRep = 2;
				}
				m_FrequencyRep = header.frequency();
				if (m_Decoder.DecodeFrame(header, m_BitStream) != m_Buffer)
				{
					throw new ApplicationException("Output buffers are different.");
				}
			}
			finally
			{
				m_BitStream.CloseFrame();
			}
			return true;
		}
	}
}
