using System;
using System.Collections.Generic;

namespace OggVorbisEncoder.Lookups
{
	public class MdctLookup
	{
		private const float Pi3Eighths = 0.38268343f;

		private const float Pi2Eighths = 0.70710677f;

		private const float Pi1Eighth = 0.9238795f;

		private readonly int[] _bitReverse;

		private readonly int _log2N;

		private readonly int _n;

		private readonly float _scale;

		private readonly float[] _trig;

		public MdctLookup(int n)
		{
			_n = n;
			int num = n >> 1;
			_log2N = (int)Math.Round(Math.Log(n) / Math.Log(2.0));
			_trig = new float[n + n / 4];
			_bitReverse = new int[n / 4];
			for (int i = 0; i < n / 4; i++)
			{
				_trig[i * 2] = (float)Math.Cos(Math.PI / (double)n * (double)(4 * i));
				_trig[i * 2 + 1] = (float)(0.0 - Math.Sin(Math.PI / (double)n * (double)(4 * i)));
				_trig[num + i * 2] = (float)Math.Cos(Math.PI / (double)(2 * n) * (double)(2 * i + 1));
				_trig[num + i * 2 + 1] = (float)Math.Sin(Math.PI / (double)(2 * n) * (double)(2 * i + 1));
			}
			for (int j = 0; j < n / 8; j++)
			{
				_trig[n + j * 2] = (float)(Math.Cos(Math.PI / (double)n * (double)(4 * j + 2)) * 0.5);
				_trig[n + j * 2 + 1] = (float)((0.0 - Math.Sin(Math.PI / (double)n * (double)(4 * j + 2))) * 0.5);
			}
			int num2 = (1 << _log2N - 1) - 1;
			int num3 = 1 << _log2N - 2;
			for (int k = 0; k < n / 8; k++)
			{
				int num4 = 0;
				for (int l = 0; num3 >> l != 0; l++)
				{
					if (((num3 >> l) & k) != 0)
					{
						num4 |= 1 << l;
					}
				}
				_bitReverse[k * 2] = (~num4 & num2) - 1;
				_bitReverse[k * 2 + 1] = num4;
			}
			_scale = 4f / (float)n;
		}

		public void Forward(IList<float> input, float[] output)
		{
			int n = _n;
			int num = n >> 1;
			int num2 = n >> 2;
			int num3 = n >> 3;
			float[] array = new float[n];
			OffsetArray<float> offsetArray = new OffsetArray<float>(array, num);
			int num4 = num + num2;
			int num5 = num4 + 1;
			int num6 = num;
			int i;
			for (i = 0; i < num3; i += 2)
			{
				num4 -= 4;
				num6 -= 2;
				float num7 = input[num4 + 2] + input[num5];
				float num8 = input[num4] + input[num5 + 2];
				offsetArray[i] = num8 * _trig[num6 + 1] + num7 * _trig[num6];
				offsetArray[i + 1] = num8 * _trig[num6] - num7 * _trig[num6 + 1];
				num5 += 4;
			}
			num5 = 1;
			for (; i < num - num3; i += 2)
			{
				num6 -= 2;
				num4 -= 4;
				float num9 = input[num4 + 2] - input[num5];
				float num10 = input[num4] - input[num5 + 2];
				offsetArray[i] = num10 * _trig[num6 + 1] + num9 * _trig[num6];
				offsetArray[i + 1] = num10 * _trig[num6] - num9 * _trig[num6 + 1];
				num5 += 4;
			}
			num4 = n;
			for (; i < num; i += 2)
			{
				num6 -= 2;
				num4 -= 4;
				float num11 = 0f - input[num4 + 2] - input[num5];
				float num12 = 0f - input[num4] - input[num5 + 2];
				offsetArray[i] = num12 * _trig[num6 + 1] + num11 * _trig[num6];
				offsetArray[i + 1] = num12 * _trig[num6] - num11 * _trig[num6 + 1];
				num5 += 4;
			}
			Butterflies(offsetArray, num);
			ReverseBits(array);
			num6 = num;
			num4 = num;
			int num13 = 0;
			for (i = 0; i < num2; i++)
			{
				num4--;
				output[i] = (array[num13] * _trig[num6] + array[num13 + 1] * _trig[num6 + 1]) * _scale;
				output[num4] = (array[num13] * _trig[num6 + 1] - array[num13 + 1] * _trig[num6]) * _scale;
				num13 += 2;
				num6 += 2;
			}
		}

		private void Butterflies(IList<float> data, int points)
		{
			int num = _log2N - 5;
			if (--num > 0)
			{
				ButterflyFirst(data, points);
			}
			int num2 = 1;
			while (--num > 0)
			{
				for (int i = 0; i < 1 << num2; i++)
				{
					ButterflyGeneric(data, (points >> num2) * i, points >> num2, 4 << num2);
				}
				num2++;
			}
			for (int j = 0; j < points; j += 32)
			{
				Butterfly32(data, j);
			}
		}

		private static void Butterfly32(IList<float> data, int offset)
		{
			float value = data[offset + 30] - data[offset + 14];
			float value2 = data[offset + 31] - data[offset + 15];
			data[offset + 30] += data[offset + 14];
			data[offset + 31] += data[offset + 15];
			data[offset + 14] = value;
			data[offset + 15] = value2;
			value = data[offset + 28] - data[offset + 12];
			value2 = data[offset + 29] - data[offset + 13];
			data[offset + 28] += data[offset + 12];
			data[offset + 29] += data[offset + 13];
			data[offset + 12] = value * 0.9238795f - value2 * 0.38268343f;
			data[offset + 13] = value * 0.38268343f + value2 * 0.9238795f;
			value = data[offset + 26] - data[offset + 10];
			value2 = data[offset + 27] - data[offset + 11];
			data[offset + 26] += data[offset + 10];
			data[offset + 27] += data[offset + 11];
			data[offset + 10] = (value - value2) * 0.70710677f;
			data[offset + 11] = (value + value2) * 0.70710677f;
			value = data[offset + 24] - data[offset + 8];
			value2 = data[offset + 25] - data[offset + 9];
			data[offset + 24] += data[offset + 8];
			data[offset + 25] += data[offset + 9];
			data[offset + 8] = value * 0.38268343f - value2 * 0.9238795f;
			data[offset + 9] = value2 * 0.38268343f + value * 0.9238795f;
			value = data[offset + 22] - data[offset + 6];
			value2 = data[offset + 7] - data[offset + 23];
			data[offset + 22] += data[offset + 6];
			data[offset + 23] += data[offset + 7];
			data[offset + 6] = value2;
			data[offset + 7] = value;
			value = data[offset + 4] - data[offset + 20];
			value2 = data[offset + 5] - data[offset + 21];
			data[offset + 20] += data[offset + 4];
			data[offset + 21] += data[offset + 5];
			data[offset + 4] = value2 * 0.9238795f + value * 0.38268343f;
			data[offset + 5] = value2 * 0.38268343f - value * 0.9238795f;
			value = data[offset + 2] - data[offset + 18];
			value2 = data[offset + 3] - data[offset + 19];
			data[offset + 18] += data[offset + 2];
			data[offset + 19] += data[offset + 3];
			data[offset + 2] = (value2 + value) * 0.70710677f;
			data[offset + 3] = (value2 - value) * 0.70710677f;
			value = data[offset] - data[offset + 16];
			value2 = data[offset + 1] - data[offset + 17];
			data[offset + 16] += data[offset];
			data[offset + 17] += data[offset + 1];
			data[offset] = value2 * 0.38268343f + value * 0.9238795f;
			data[offset + 1] = value2 * 0.9238795f - value * 0.38268343f;
			Butterfly16(data, offset);
			Butterfly16(data, offset + 16);
		}

		private static void Butterfly16(IList<float> data, int offset)
		{
			float num = data[offset + 1] - data[offset + 9];
			float num2 = data[offset] - data[offset + 8];
			data[offset + 8] += data[offset];
			data[offset + 9] += data[offset + 1];
			data[offset] = (num + num2) * 0.70710677f;
			data[offset + 1] = (num - num2) * 0.70710677f;
			num = data[offset + 3] - data[offset + 11];
			num2 = data[offset + 10] - data[offset + 2];
			data[offset + 10] += data[offset + 2];
			data[offset + 11] += data[offset + 3];
			data[offset + 2] = num;
			data[offset + 3] = num2;
			num = data[offset + 12] - data[offset + 4];
			num2 = data[offset + 13] - data[offset + 5];
			data[offset + 12] += data[offset + 4];
			data[offset + 13] += data[offset + 5];
			data[offset + 4] = (num - num2) * 0.70710677f;
			data[offset + 5] = (num + num2) * 0.70710677f;
			num = data[offset + 14] - data[offset + 6];
			num2 = data[offset + 15] - data[offset + 7];
			data[offset + 14] += data[offset + 6];
			data[offset + 15] += data[offset + 7];
			data[offset + 6] = num;
			data[offset + 7] = num2;
			Butterfly8(data, offset);
			Butterfly8(data, offset + 8);
		}

		private static void Butterfly8(IList<float> data, int offset)
		{
			float num = data[offset + 6] + data[offset + 2];
			float num2 = data[offset + 6] - data[offset + 2];
			float num3 = data[offset + 4] + data[offset];
			float num4 = data[offset + 4] - data[offset];
			data[offset + 6] = num + num3;
			data[offset + 4] = num - num3;
			num = data[offset + 5] - data[offset + 1];
			num3 = data[offset + 7] - data[offset + 3];
			data[offset] = num2 + num;
			data[offset + 2] = num2 - num;
			num = data[offset + 5] + data[offset + 1];
			num2 = data[offset + 7] + data[offset + 3];
			data[offset + 3] = num3 + num4;
			data[offset + 1] = num3 - num4;
			data[offset + 7] = num2 + num;
			data[offset + 5] = num2 - num;
		}

		private void ButterflyGeneric(IList<float> data, int offset, int points, int trigIncrement)
		{
			int num = 0;
			int num2 = offset + points - 8;
			int num3 = offset + (points >> 1) - 8;
			do
			{
				float num4 = data[num2 + 6] - data[num3 + 6];
				float num5 = data[num2 + 7] - data[num3 + 7];
				data[num2 + 6] += data[num3 + 6];
				data[num2 + 7] += data[num3 + 7];
				data[num3 + 6] = num5 * _trig[num + 1] + num4 * _trig[num];
				data[num3 + 7] = num5 * _trig[num] - num4 * _trig[num + 1];
				num += trigIncrement;
				num4 = data[num2 + 4] - data[num3 + 4];
				num5 = data[num2 + 5] - data[num3 + 5];
				data[num2 + 4] += data[num3 + 4];
				data[num2 + 5] += data[num3 + 5];
				data[num3 + 4] = num5 * _trig[num + 1] + num4 * _trig[num];
				data[num3 + 5] = num5 * _trig[num] - num4 * _trig[num + 1];
				num += trigIncrement;
				num4 = data[num2 + 2] - data[num3 + 2];
				num5 = data[num2 + 3] - data[num3 + 3];
				data[num2 + 2] += data[num3 + 2];
				data[num2 + 3] += data[num3 + 3];
				data[num3 + 2] = num5 * _trig[num + 1] + num4 * _trig[num];
				data[num3 + 3] = num5 * _trig[num] - num4 * _trig[num + 1];
				num += trigIncrement;
				num4 = data[num2] - data[num3];
				num5 = data[num2 + 1] - data[num3 + 1];
				data[num2] += data[num3];
				data[num2 + 1] += data[num3 + 1];
				data[num3] = num5 * _trig[num + 1] + num4 * _trig[num];
				data[num3 + 1] = num5 * _trig[num] - num4 * _trig[num + 1];
				num += trigIncrement;
				num2 -= 8;
				num3 -= 8;
			}
			while (num3 >= offset);
		}

		private void ButterflyFirst(IList<float> data, int points)
		{
			int num = points - 8;
			int num2 = (points >> 1) - 8;
			int num3 = 0;
			do
			{
				float num4 = data[num + 6] - data[num2 + 6];
				float num5 = data[num + 7] - data[num2 + 7];
				data[num + 6] += data[num2 + 6];
				data[num + 7] += data[num2 + 7];
				data[num2 + 6] = num5 * _trig[num3 + 1] + num4 * _trig[num3];
				data[num2 + 7] = num5 * _trig[num3] - num4 * _trig[num3 + 1];
				num4 = data[num + 4] - data[num2 + 4];
				num5 = data[num + 5] - data[num2 + 5];
				data[num + 4] += data[num2 + 4];
				data[num + 5] += data[num2 + 5];
				data[num2 + 4] = num5 * _trig[num3 + 5] + num4 * _trig[num3 + 4];
				data[num2 + 5] = num5 * _trig[num3 + 4] - num4 * _trig[num3 + 5];
				num4 = data[num + 2] - data[num2 + 2];
				num5 = data[num + 3] - data[num2 + 3];
				data[num + 2] += data[num2 + 2];
				data[num + 3] += data[num2 + 3];
				data[num2 + 2] = num5 * _trig[num3 + 9] + num4 * _trig[num3 + 8];
				data[num2 + 3] = num5 * _trig[num3 + 8] - num4 * _trig[num3 + 9];
				num4 = data[num] - data[num2];
				num5 = data[num + 1] - data[num2 + 1];
				data[num] += data[num2];
				data[num + 1] += data[num2 + 1];
				data[num2] = num5 * _trig[num3 + 13] + num4 * _trig[num3 + 12];
				data[num2 + 1] = num5 * _trig[num3 + 12] - num4 * _trig[num3 + 13];
				num -= 8;
				num2 -= 8;
				num3 += 16;
			}
			while (num2 >= 0);
		}

		private void ReverseBits(IList<float> data)
		{
			int n = _n;
			int num = 0;
			int num2 = 0;
			int num3 = num2 + (n >> 1);
			int num4 = num3;
			int num5 = n;
			do
			{
				int num6 = num4 + _bitReverse[num];
				int num7 = num4 + _bitReverse[num + 1];
				float num8 = data[num6 + 1] - data[num7 + 1];
				float num9 = data[num6] + data[num7];
				float num10 = num9 * _trig[num5] + num8 * _trig[num5 + 1];
				float num11 = num9 * _trig[num5 + 1] - num8 * _trig[num5];
				num3 -= 4;
				num8 = 0.5f * (data[num6 + 1] + data[num7 + 1]);
				num9 = 0.5f * (data[num6] - data[num7]);
				data[num2] = num8 + num10;
				data[num3 + 2] = num8 - num10;
				data[num2 + 1] = num9 + num11;
				data[num3 + 3] = num11 - num9;
				num6 = num4 + _bitReverse[num + 2];
				num7 = num4 + _bitReverse[num + 3];
				num8 = data[num6 + 1] - data[num7 + 1];
				num9 = data[num6] + data[num7];
				num10 = num9 * _trig[num5 + 2] + num8 * _trig[num5 + 3];
				num11 = num9 * _trig[num5 + 3] - num8 * _trig[num5 + 2];
				num8 = 0.5f * (data[num6 + 1] + data[num7 + 1]);
				num9 = 0.5f * (data[num6] - data[num7]);
				data[num2 + 2] = num8 + num10;
				data[num3] = num8 - num10;
				data[num2 + 3] = num9 + num11;
				data[num3 + 1] = num11 - num9;
				num5 += 4;
				num += 4;
				num2 += 4;
			}
			while (num2 < num3);
		}
	}
}
