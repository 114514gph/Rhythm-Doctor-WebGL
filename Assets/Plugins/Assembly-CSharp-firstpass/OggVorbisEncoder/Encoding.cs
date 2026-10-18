using System;

namespace OggVorbisEncoder
{
	public static class Encoding
	{
		private const int Man = 21;

		private const int ExpBias = 768;

		public static int Log(int value)
		{
			int num = 0;
			while (value != 0)
			{
				value >>= 1;
				num++;
			}
			return num;
		}

		public static float UnpackFloat(int value)
		{
			double num = value & 0x1FFFFF;
			long num2 = value & 0x80000000u;
			int num3 = (int)((long)((ulong)value & 0x7FE00000uL) >> 21);
			if (num2 != 0L)
			{
				num = 0.0 - num;
			}
			return (float)(num * Math.Pow(2.0, num3 - 20 - 768));
		}

		public static uint[] MakeWords(byte[] l, int sparsecount)
		{
			int num = 0;
			int num2 = l.Length;
			uint[] array = new uint[33];
			uint[] array2 = new uint[(sparsecount != 0) ? sparsecount : num2];
			for (int i = 0; i < num2; i++)
			{
				int num3 = l[i];
				if (num3 > 0)
				{
					uint num4 = array[num3];
					if (num3 < 32 && num4 >> num3 != 0)
					{
						return null;
					}
					array2[num++] = num4;
					for (int num5 = num3; num5 > 0; num5--)
					{
						if ((array[num5] & 1) != 0)
						{
							if (num5 == 1)
							{
								array[1]++;
							}
							else
							{
								array[num5] = array[num5 - 1] << 1;
							}
							break;
						}
						array[num5]++;
					}
					for (int j = num3 + 1; j < 33 && array[j] >> 1 == num4; j++)
					{
						num4 = array[j];
						array[j] = array[j - 1] << 1;
					}
				}
				else if (sparsecount == 0)
				{
					num++;
				}
			}
			if (num != 1 || array[2] != 2)
			{
				for (int k = 1; k < 33; k++)
				{
					if ((array[k] & (4294967295uL >> 32 - k)) != 0L)
					{
						return null;
					}
				}
			}
			num = 0;
			for (int m = 0; m < num2; m++)
			{
				uint num6 = 0u;
				for (int n = 0; n < l[m]; n++)
				{
					num6 <<= 1;
					num6 |= (array2[num] >> n) & 1;
				}
				if (sparsecount != 0)
				{
					if (l[m] != 0)
					{
						array2[num++] = num6;
					}
				}
				else
				{
					array2[num++] = num6;
				}
			}
			return array2;
		}
	}
}
