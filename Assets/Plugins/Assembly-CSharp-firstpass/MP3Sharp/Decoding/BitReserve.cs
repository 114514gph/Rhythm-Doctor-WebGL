namespace MP3Sharp.Decoding
{
	internal sealed class BitReserve
	{
		private const int BUFSIZE = 32768;

		private static readonly int BUFSIZE_MASK = 32767;

		private int[] buf;

		private int offset;

		private int totbit;

		private int buf_byte_idx;

		internal BitReserve()
		{
			InitBlock();
			offset = 0;
			totbit = 0;
			buf_byte_idx = 0;
		}

		private void InitBlock()
		{
			buf = new int[32768];
		}

		public int hsstell()
		{
			return totbit;
		}

		public int ReadBits(int N)
		{
			totbit += N;
			int num = 0;
			int num2 = buf_byte_idx;
			if (num2 + N < 32768)
			{
				while (N-- > 0)
				{
					num <<= 1;
					num |= ((buf[num2++] != 0) ? 1 : 0);
				}
			}
			else
			{
				while (N-- > 0)
				{
					num <<= 1;
					num |= ((buf[num2] != 0) ? 1 : 0);
					num2 = (num2 + 1) & BUFSIZE_MASK;
				}
			}
			buf_byte_idx = num2;
			return num;
		}

		public int ReadOneBit()
		{
			totbit++;
			int result = buf[buf_byte_idx];
			buf_byte_idx = (buf_byte_idx + 1) & BUFSIZE_MASK;
			return result;
		}

		public void hputbuf(int val)
		{
			int num = offset;
			buf[num++] = val & 0x80;
			buf[num++] = val & 0x40;
			buf[num++] = val & 0x20;
			buf[num++] = val & 0x10;
			buf[num++] = val & 8;
			buf[num++] = val & 4;
			buf[num++] = val & 2;
			buf[num++] = val & 1;
			if (num == 32768)
			{
				offset = 0;
			}
			else
			{
				offset = num;
			}
		}

		public void RewindStreamBits(int bitCount)
		{
			totbit -= bitCount;
			buf_byte_idx -= bitCount;
			if (buf_byte_idx < 0)
			{
				buf_byte_idx += 32768;
			}
		}

		public void RewindStreamBytes(int byteCount)
		{
			int num = byteCount << 3;
			totbit -= num;
			buf_byte_idx -= num;
			if (buf_byte_idx < 0)
			{
				buf_byte_idx += 32768;
			}
		}
	}
}
