using System.IO;
using MP3Sharp.Support;

namespace MP3Sharp.IO
{
	internal class RiffFile
	{
		internal class RiffChunkHeader
		{
			public int CkId;

			public int CkSize;

			private RiffFile m_EnclosingInstance;

			public RiffFile EnclosingInstance => m_EnclosingInstance;

			public RiffChunkHeader(RiffFile enclosingInstance)
			{
				InitBlock(enclosingInstance);
			}

			private void InitBlock(RiffFile enclosingInstance)
			{
				m_EnclosingInstance = enclosingInstance;
			}
		}

		protected const int DDC_SUCCESS = 0;

		protected const int DDC_FAILURE = 1;

		protected const int DDC_OUT_OF_MEMORY = 2;

		protected const int DDC_FILE_ERROR = 3;

		protected const int DDC_INVALID_CALL = 4;

		protected const int DDC_USER_ABORT = 5;

		protected const int DDC_INVALID_FILE = 6;

		protected const int RFM_UNKNOWN = 0;

		protected const int RFM_WRITE = 1;

		protected const int RFM_READ = 2;

		private readonly RiffChunkHeader m_RiffHeader;

		protected int Fmode;

		private Stream m_File;

		public RiffFile()
		{
			m_File = null;
			Fmode = 0;
			m_RiffHeader = new RiffChunkHeader(this);
			m_RiffHeader.CkId = FourCC("RIFF");
			m_RiffHeader.CkSize = 0;
		}

		public virtual int CurrentFileMode()
		{
			return Fmode;
		}

		public virtual int Open(string filename, int newMode)
		{
			int num = 0;
			if (Fmode != 0)
			{
				num = Close();
			}
			if (num == 0)
			{
				switch (newMode)
				{
				case 1:
					try
					{
						m_File = RandomAccessFileStream.CreateRandomAccessFile(filename, "rw");
						try
						{
							sbyte[] array = new sbyte[8]
							{
								(sbyte)(SupportClass.URShift(m_RiffHeader.CkId, 24) & 0xFF),
								(sbyte)(SupportClass.URShift(m_RiffHeader.CkId, 16) & 0xFF),
								(sbyte)(SupportClass.URShift(m_RiffHeader.CkId, 8) & 0xFF),
								(sbyte)(m_RiffHeader.CkId & 0xFF),
								0,
								0,
								0,
								0
							};
							sbyte b = (sbyte)(SupportClass.URShift(m_RiffHeader.CkSize, 24) & 0xFF);
							sbyte b2 = (sbyte)(SupportClass.URShift(m_RiffHeader.CkSize, 16) & 0xFF);
							sbyte b3 = (sbyte)(SupportClass.URShift(m_RiffHeader.CkSize, 8) & 0xFF);
							sbyte b4 = (sbyte)(m_RiffHeader.CkSize & 0xFF);
							array[4] = b4;
							array[5] = b3;
							array[6] = b2;
							array[7] = b;
							m_File.Write(SupportClass.ToByteArray(array), 0, 8);
							Fmode = 1;
						}
						catch
						{
							m_File.Close();
							Fmode = 0;
						}
					}
					catch
					{
						Fmode = 0;
						num = 3;
					}
					break;
				case 2:
					try
					{
						m_File = RandomAccessFileStream.CreateRandomAccessFile(filename, "r");
						try
						{
							sbyte[] target = new sbyte[8];
							SupportClass.ReadInput(m_File, ref target, 0, 8);
							Fmode = 2;
							m_RiffHeader.CkId = ((target[0] << 24) & (int)SupportClass.Identity(4278190080L)) | ((target[1] << 16) & 0xFF0000) | ((target[2] << 8) & 0xFF00) | (target[3] & 0xFF);
							m_RiffHeader.CkSize = ((target[4] << 24) & (int)SupportClass.Identity(4278190080L)) | ((target[5] << 16) & 0xFF0000) | ((target[6] << 8) & 0xFF00) | (target[7] & 0xFF);
						}
						catch
						{
							m_File.Close();
							Fmode = 0;
						}
					}
					catch
					{
						Fmode = 0;
						num = 3;
					}
					break;
				default:
					num = 4;
					break;
				}
			}
			return num;
		}

		public virtual int Open(Stream stream, int newMode)
		{
			int num = 0;
			if (Fmode != 0)
			{
				num = Close();
			}
			if (num == 0)
			{
				switch (newMode)
				{
				case 1:
					try
					{
						m_File = stream;
						try
						{
							sbyte[] array = new sbyte[8]
							{
								(sbyte)(SupportClass.URShift(m_RiffHeader.CkId, 24) & 0xFF),
								(sbyte)(SupportClass.URShift(m_RiffHeader.CkId, 16) & 0xFF),
								(sbyte)(SupportClass.URShift(m_RiffHeader.CkId, 8) & 0xFF),
								(sbyte)(m_RiffHeader.CkId & 0xFF),
								0,
								0,
								0,
								0
							};
							sbyte b = (sbyte)(SupportClass.URShift(m_RiffHeader.CkSize, 24) & 0xFF);
							sbyte b2 = (sbyte)(SupportClass.URShift(m_RiffHeader.CkSize, 16) & 0xFF);
							sbyte b3 = (sbyte)(SupportClass.URShift(m_RiffHeader.CkSize, 8) & 0xFF);
							sbyte b4 = (sbyte)(m_RiffHeader.CkSize & 0xFF);
							array[4] = b4;
							array[5] = b3;
							array[6] = b2;
							array[7] = b;
							m_File.Write(SupportClass.ToByteArray(array), 0, 8);
							Fmode = 1;
						}
						catch
						{
							m_File.Close();
							Fmode = 0;
						}
					}
					catch
					{
						Fmode = 0;
						num = 3;
					}
					break;
				case 2:
					try
					{
						m_File = stream;
						try
						{
							sbyte[] target = new sbyte[8];
							SupportClass.ReadInput(m_File, ref target, 0, 8);
							Fmode = 2;
							m_RiffHeader.CkId = ((target[0] << 24) & (int)SupportClass.Identity(4278190080L)) | ((target[1] << 16) & 0xFF0000) | ((target[2] << 8) & 0xFF00) | (target[3] & 0xFF);
							m_RiffHeader.CkSize = ((target[4] << 24) & (int)SupportClass.Identity(4278190080L)) | ((target[5] << 16) & 0xFF0000) | ((target[6] << 8) & 0xFF00) | (target[7] & 0xFF);
						}
						catch
						{
							m_File.Close();
							Fmode = 0;
						}
					}
					catch
					{
						Fmode = 0;
						num = 3;
					}
					break;
				default:
					num = 4;
					break;
				}
			}
			return num;
		}

		public virtual int Write(sbyte[] data, int numBytes)
		{
			if (Fmode != 1)
			{
				return 4;
			}
			try
			{
				m_File.Write(SupportClass.ToByteArray(data), 0, numBytes);
				Fmode = 1;
			}
			catch
			{
				return 3;
			}
			m_RiffHeader.CkSize += numBytes;
			return 0;
		}

		public virtual int Write(short[] data, int numBytes)
		{
			sbyte[] array = new sbyte[numBytes];
			int num = 0;
			for (int i = 0; i < numBytes; i += 2)
			{
				array[i] = (sbyte)(data[num] & 0xFF);
				array[i + 1] = (sbyte)(SupportClass.URShift(data[num++], 8) & 0xFF);
			}
			if (Fmode != 1)
			{
				return 4;
			}
			try
			{
				m_File.Write(SupportClass.ToByteArray(array), 0, numBytes);
				Fmode = 1;
			}
			catch
			{
				return 3;
			}
			m_RiffHeader.CkSize += numBytes;
			return 0;
		}

		public virtual int Write(RiffChunkHeader riffHeader, int numBytes)
		{
			sbyte[] array = new sbyte[8]
			{
				(sbyte)(SupportClass.URShift(riffHeader.CkId, 24) & 0xFF),
				(sbyte)(SupportClass.URShift(riffHeader.CkId, 16) & 0xFF),
				(sbyte)(SupportClass.URShift(riffHeader.CkId, 8) & 0xFF),
				(sbyte)(riffHeader.CkId & 0xFF),
				0,
				0,
				0,
				0
			};
			sbyte b = (sbyte)(SupportClass.URShift(riffHeader.CkSize, 24) & 0xFF);
			sbyte b2 = (sbyte)(SupportClass.URShift(riffHeader.CkSize, 16) & 0xFF);
			sbyte b3 = (sbyte)(SupportClass.URShift(riffHeader.CkSize, 8) & 0xFF);
			sbyte b4 = (sbyte)(riffHeader.CkSize & 0xFF);
			array[4] = b4;
			array[5] = b3;
			array[6] = b2;
			array[7] = b;
			if (Fmode != 1)
			{
				return 4;
			}
			try
			{
				m_File.Write(SupportClass.ToByteArray(array), 0, numBytes);
				Fmode = 1;
			}
			catch
			{
				return 3;
			}
			m_RiffHeader.CkSize += numBytes;
			return 0;
		}

		public virtual int Write(short data, int numBytes)
		{
			short value = data;
			if (Fmode != 1)
			{
				return 4;
			}
			try
			{
				new BinaryWriter(m_File).Write(value);
				Fmode = 1;
			}
			catch
			{
				return 3;
			}
			m_RiffHeader.CkSize += numBytes;
			return 0;
		}

		public virtual int Write(int data, int numBytes)
		{
			if (Fmode != 1)
			{
				return 4;
			}
			try
			{
				new BinaryWriter(m_File).Write(data);
				Fmode = 1;
			}
			catch
			{
				return 3;
			}
			m_RiffHeader.CkSize += numBytes;
			return 0;
		}

		public virtual int Read(sbyte[] data, int numBytes)
		{
			int result = 0;
			try
			{
				SupportClass.ReadInput(m_File, ref data, 0, numBytes);
			}
			catch
			{
				result = 3;
			}
			return result;
		}

		public virtual int Expect(string data, int numBytes)
		{
			int num = 0;
			try
			{
				while (numBytes-- != 0)
				{
					if ((sbyte)m_File.ReadByte() != data[num++])
					{
						return 3;
					}
				}
			}
			catch
			{
				return 3;
			}
			return 0;
		}

		public virtual int Close()
		{
			int result = 0;
			switch (Fmode)
			{
			case 1:
				try
				{
					m_File.Seek(0L, SeekOrigin.Begin);
					try
					{
						sbyte[] array = new sbyte[8];
						array[0] = (sbyte)(SupportClass.URShift(m_RiffHeader.CkId, 24) & 0xFF);
						array[1] = (sbyte)(SupportClass.URShift(m_RiffHeader.CkId, 16) & 0xFF);
						array[2] = (sbyte)(SupportClass.URShift(m_RiffHeader.CkId, 8) & 0xFF);
						array[3] = (sbyte)(m_RiffHeader.CkId & 0xFF);
						array[7] = (sbyte)(SupportClass.URShift(m_RiffHeader.CkSize, 24) & 0xFF);
						array[6] = (sbyte)(SupportClass.URShift(m_RiffHeader.CkSize, 16) & 0xFF);
						array[5] = (sbyte)(SupportClass.URShift(m_RiffHeader.CkSize, 8) & 0xFF);
						array[4] = (sbyte)(m_RiffHeader.CkSize & 0xFF);
						m_File.Write(SupportClass.ToByteArray(array), 0, 8);
						m_File.Close();
					}
					catch
					{
						result = 3;
					}
				}
				catch
				{
					result = 3;
				}
				break;
			case 2:
				try
				{
					m_File.Close();
				}
				catch
				{
					result = 3;
				}
				break;
			}
			m_File = null;
			Fmode = 0;
			return result;
		}

		public virtual long CurrentFilePosition()
		{
			try
			{
				return m_File.Position;
			}
			catch
			{
				return -1L;
			}
		}

		public virtual int Backpatch(long fileOffset, RiffChunkHeader data, int numBytes)
		{
			if (m_File == null)
			{
				return 4;
			}
			try
			{
				m_File.Seek(fileOffset, SeekOrigin.Begin);
			}
			catch
			{
				return 3;
			}
			return Write(data, numBytes);
		}

		public virtual int Backpatch(long fileOffset, sbyte[] data, int numBytes)
		{
			if (m_File == null)
			{
				return 4;
			}
			try
			{
				m_File.Seek(fileOffset, SeekOrigin.Begin);
			}
			catch
			{
				return 3;
			}
			return Write(data, numBytes);
		}

		protected internal virtual int Seek(long offset)
		{
			try
			{
				m_File.Seek(offset, SeekOrigin.Begin);
				return 0;
			}
			catch
			{
				return 3;
			}
		}

		private string DDCRET_String(int retcode)
		{
			return retcode switch
			{
				0 => "DDC_SUCCESS", 
				1 => "DDC_FAILURE", 
				2 => "DDC_OUT_OF_MEMORY", 
				3 => "DDC_FILE_ERROR", 
				4 => "DDC_INVALID_CALL", 
				5 => "DDC_USER_ABORT", 
				6 => "DDC_INVALID_FILE", 
				_ => "Unknown Error", 
			};
		}

		public static int FourCC(string chunkName)
		{
			sbyte[] destinationArray = new sbyte[4] { 32, 32, 32, 32 };
			SupportClass.GetSBytesFromString(chunkName, 0, 4, ref destinationArray, 0);
			return ((destinationArray[0] << 24) & (int)SupportClass.Identity(4278190080L)) | ((destinationArray[1] << 16) & 0xFF0000) | ((destinationArray[2] << 8) & 0xFF00) | (destinationArray[3] & 0xFF);
		}
	}
}
