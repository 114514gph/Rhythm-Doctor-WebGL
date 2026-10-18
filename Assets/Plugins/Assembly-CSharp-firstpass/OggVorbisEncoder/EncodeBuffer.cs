using System;
using OggVorbisEncoder.Setup;

namespace OggVorbisEncoder
{
	public class EncodeBuffer
	{
		private const int BufferIncrement = 256;

		private static readonly uint[] Mask = new uint[33]
		{
			0u, 1u, 3u, 7u, 15u, 31u, 63u, 127u, 255u, 511u,
			1023u, 2047u, 4095u, 8191u, 16383u, 32767u, 65535u, 131071u, 262143u, 524287u,
			1048575u, 2097151u, 4194303u, 8388607u, 16777215u, 33554431u, 67108863u, 134217727u, 268435455u, 536870911u,
			1073741823u, 2147483647u, 4294967295u
		};

		private byte[] _buffer;

		private int _endBit;

		private int _endByte;

		private int Bytes => _endByte + (_endBit + 7) / 8;

		public EncodeBuffer()
			: this(256)
		{
		}

		public EncodeBuffer(int initialBufferSize)
		{
			_buffer = new byte[initialBufferSize];
		}

		public void WriteBook(CodeBook book, int a)
		{
			if (a >= 0 && a < book.Entries)
			{
				Write(book.CodeList[a], book.StaticBook.LengthList[a]);
			}
		}

		public void WriteString(string str)
		{
			foreach (char value in str)
			{
				Write(value, 8);
			}
		}

		public void Write(uint value, int bits)
		{
			if (bits < 0 || bits > 32)
			{
				throw new ArgumentException("bits must be between 0 and 32");
			}
			if (_endByte >= _buffer.Length - 4)
			{
				if (_buffer.Length > 2147483391)
				{
					throw new InvalidOperationException("Maximum buffer size exceeded");
				}
				Array.Resize(ref _buffer, _buffer.Length + 256);
			}
			value &= Mask[bits];
			bits += _endBit;
			_buffer[_endByte] = (byte)(_buffer[_endByte] | (value << _endBit));
			if (bits >= 8)
			{
				_buffer[_endByte + 1] = (byte)(value >> 8 - _endBit);
				if (bits >= 16)
				{
					_buffer[_endByte + 2] = (byte)(value >> 16 - _endBit);
					if (bits >= 24)
					{
						_buffer[_endByte + 3] = (byte)(value >> 24 - _endBit);
						if (bits >= 32)
						{
							if (_endBit != 0)
							{
								_buffer[_endByte + 4] = (byte)(value >> 32 - _endBit);
							}
							else
							{
								_buffer[_endByte + 4] = 0;
							}
						}
					}
				}
			}
			_endByte += bits / 8;
			_endBit = bits & 7;
		}

		public byte[] GetBytes()
		{
			Array.Resize(ref _buffer, Bytes);
			return _buffer;
		}
	}
}
