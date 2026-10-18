using System.IO;
using MP3Sharp.Support;

namespace MP3Sharp.IO
{
	internal class WaveFile : RiffFile
	{
		internal sealed class WaveFormatChunkData
		{
			private WaveFile m_EnclosingInstance;

			public int NumAvgBytesPerSec;

			public short NumBitsPerSample;

			public short NumBlockAlign;

			public short NumChannels;

			public int NumSamplesPerSec;

			public short FormatTag;

			public WaveFile EnclosingInstance => m_EnclosingInstance;

			public WaveFormatChunkData(WaveFile enclosingInstance)
			{
				InitBlock(enclosingInstance);
				FormatTag = 1;
				Config(44100, 16, 1);
			}

			private void InitBlock(WaveFile enclosingInstance)
			{
				m_EnclosingInstance = enclosingInstance;
			}

			public void Config(int newSamplingRate, short newBitsPerSample, short newNumChannels)
			{
				NumSamplesPerSec = newSamplingRate;
				NumChannels = newNumChannels;
				NumBitsPerSample = newBitsPerSample;
				NumAvgBytesPerSec = NumChannels * NumSamplesPerSec * NumBitsPerSample / 8;
				NumBlockAlign = (short)(NumChannels * NumBitsPerSample / 8);
			}
		}

		internal class WaveFormatChunk
		{
			public WaveFormatChunkData Data;

			private WaveFile m_EnclosingInstance;

			public RiffChunkHeader Header;

			public WaveFile EnclosingInstance => m_EnclosingInstance;

			public WaveFormatChunk(WaveFile enclosingInstance)
			{
				InitBlock(enclosingInstance);
				Header = new RiffChunkHeader(enclosingInstance);
				Data = new WaveFormatChunkData(enclosingInstance);
				Header.CkId = RiffFile.FourCC("fmt ");
				Header.CkSize = 16;
			}

			private void InitBlock(WaveFile enclosingInstance)
			{
				m_EnclosingInstance = enclosingInstance;
			}

			public virtual int VerifyValidity()
			{
				if (Header.CkId != RiffFile.FourCC("fmt ") || (Data.NumChannels != 1 && Data.NumChannels != 2) || Data.NumAvgBytesPerSec != Data.NumChannels * Data.NumSamplesPerSec * Data.NumBitsPerSample / 8 || Data.NumBlockAlign != Data.NumChannels * Data.NumBitsPerSample / 8)
				{
					return 0;
				}
				return 1;
			}
		}

		internal class WaveFileSample
		{
			public short[] Chan;

			private WaveFile m_EnclosingInstance;

			public WaveFile EnclosingInstance => m_EnclosingInstance;

			public WaveFileSample(WaveFile enclosingInstance)
			{
				InitBlock(enclosingInstance);
				Chan = new short[2];
			}

			private void InitBlock(WaveFile enclosingInstance)
			{
				m_EnclosingInstance = enclosingInstance;
			}
		}

		public const int MAX_WAVE_CHANNELS = 2;

		private readonly int m_NumSamples;

		private readonly RiffChunkHeader m_PcmData;

		private readonly WaveFormatChunk m_WaveFormat;

		private bool m_JustWriteLengthBytes;

		private long m_PcmDataOffset;

		public WaveFile()
		{
			m_PcmData = new RiffChunkHeader(this);
			m_WaveFormat = new WaveFormatChunk(this);
			m_PcmData.CkId = RiffFile.FourCC("data");
			m_PcmData.CkSize = 0;
			m_NumSamples = 0;
		}

		public virtual int OpenForWrite(string filename, Stream stream, int samplingRate, short bitsPerSample, short numChannels)
		{
			if ((bitsPerSample != 8 && bitsPerSample != 16) || numChannels < 1 || numChannels > 2)
			{
				return 4;
			}
			m_WaveFormat.Data.Config(samplingRate, bitsPerSample, numChannels);
			int num = 0;
			if (stream != null)
			{
				Open(stream, 1);
			}
			else
			{
				Open(filename, 1);
			}
			if (num == 0)
			{
				sbyte[] data = new sbyte[4]
				{
					(sbyte)SupportClass.Identity(87L),
					(sbyte)SupportClass.Identity(65L),
					(sbyte)SupportClass.Identity(86L),
					(sbyte)SupportClass.Identity(69L)
				};
				num = Write(data, 4);
				if (num == 0)
				{
					num = Write(m_WaveFormat.Header, 8);
					num = Write(m_WaveFormat.Data.FormatTag, 2);
					num = Write(m_WaveFormat.Data.NumChannels, 2);
					num = Write(m_WaveFormat.Data.NumSamplesPerSec, 4);
					num = Write(m_WaveFormat.Data.NumAvgBytesPerSec, 4);
					num = Write(m_WaveFormat.Data.NumBlockAlign, 2);
					num = Write(m_WaveFormat.Data.NumBitsPerSample, 2);
					if (num == 0)
					{
						m_PcmDataOffset = CurrentFilePosition();
						num = Write(m_PcmData, 8);
					}
				}
			}
			return num;
		}

		public virtual int WriteData(short[] data, int numData)
		{
			int num = numData * 2;
			m_PcmData.CkSize += num;
			return Write(data, num);
		}

		public override int Close()
		{
			int num = 0;
			if (Fmode == 1)
			{
				num = Backpatch(m_PcmDataOffset, m_PcmData, 8);
			}
			if (!m_JustWriteLengthBytes && num == 0)
			{
				num = base.Close();
			}
			return num;
		}

		public int Close(bool justWriteLengthBytes)
		{
			m_JustWriteLengthBytes = justWriteLengthBytes;
			int result = Close();
			m_JustWriteLengthBytes = false;
			return result;
		}

		public virtual int SamplingRate()
		{
			return m_WaveFormat.Data.NumSamplesPerSec;
		}

		public virtual short BitsPerSample()
		{
			return m_WaveFormat.Data.NumBitsPerSample;
		}

		public virtual short NumChannels()
		{
			return m_WaveFormat.Data.NumChannels;
		}

		public virtual int NumSamples()
		{
			return m_NumSamples;
		}

		public virtual int OpenForWrite(string filename, WaveFile otherWave)
		{
			return OpenForWrite(filename, null, otherWave.SamplingRate(), otherWave.BitsPerSample(), otherWave.NumChannels());
		}
	}
}
