using System;
using System.Collections.Generic;

namespace OggVorbisEncoder.Lookups
{
	public class DrftLookup
	{
		private readonly IList<int> _splitCache;

		private readonly IList<float> _trigCache;

		public int N { get; }

		public DrftLookup(int n)
		{
			N = n;
			_trigCache = new float[3 * n];
			_splitCache = new int[32];
			fdrffti(n);
		}

		private void fdrffti(int n)
		{
			if (n != 1)
			{
				drfti1(n);
			}
		}

		private void drfti1(int n)
		{
			int[] array = new int[4] { 4, 2, 3, 5 };
			int num = 0;
			int num2 = -1;
			int num3 = n;
			int num4 = 0;
			int num6;
			do
			{
				num2++;
				num = ((num2 >= 4) ? (num + 2) : array[num2]);
				do
				{
					int num5 = num3 / num;
					num6 = num3 - num * num5;
					if (num6 != 0)
					{
						break;
					}
					num4++;
					_splitCache[num4 + 1] = num;
					num3 = num5;
					if (num == 2 && num4 != 1)
					{
						for (int i = 1; i < num4; i++)
						{
							int num7 = num4 - i + 1;
							_splitCache[num7 + 1] = _splitCache[num7];
						}
						_splitCache[2] = 2;
					}
				}
				while (num3 != 1);
			}
			while (num6 != 1);
			_splitCache[0] = n;
			_splitCache[1] = num4;
			float num8 = MathF.PI * 2f / (float)n;
			int num9 = n;
			int num10 = num4 - 1;
			int num11 = 1;
			if (num10 == 0)
			{
				return;
			}
			for (int j = 0; j < num10; j++)
			{
				int num12 = _splitCache[j + 2];
				int num13 = 0;
				int num14 = num11 * num12;
				int num15 = n / num14;
				int num16 = num12 - 1;
				for (num2 = 0; num2 < num16; num2++)
				{
					num13 += num11;
					int i = num9;
					float num17 = (float)num13 * num8;
					float num18 = 0f;
					for (int k = 2; k < num15; k += 2)
					{
						num18++;
						float num19 = num18 * num17;
						_trigCache[i++] = (float)Math.Cos(num19);
						_trigCache[i++] = (float)Math.Sin(num19);
					}
					num9 += num15;
				}
				num11 = num14;
			}
		}

		public void Forward(IList<float> data)
		{
			if (N == 1)
			{
				return;
			}
			int num = _splitCache[1];
			int num2 = 1;
			int num3 = N;
			int num4 = N;
			for (int i = 0; i < num; i++)
			{
				int num5 = num - i;
				int num6 = _splitCache[num5 + 1];
				int num7 = num3 / num6;
				int num8 = N / num3;
				int idl = num8 * num7;
				num4 -= (num6 - 1) * num8;
				num2 = 1 - num2;
				switch (num6)
				{
				default:
					if (num8 == 1)
					{
						num2 = 1 - num2;
					}
					if (num2 != 0)
					{
						dradfg(num8, num6, num7, idl, _trigCache, _trigCache, _trigCache, data, data, N + num4 - 1);
						num2 = 0;
					}
					else
					{
						dradfg(num8, num6, num7, idl, data, data, data, _trigCache, _trigCache, N + num4 - 1);
						num2 = 1;
					}
					break;
				case 2:
					if (num2 != 0)
					{
						dradf2(num8, num7, _trigCache, data, N + num4 - 1);
					}
					else
					{
						dradf2(num8, num7, data, _trigCache, N + num4 - 1);
					}
					break;
				case 4:
				{
					int num9 = num4 + num8;
					int num10 = num9 + num8;
					if (num2 != 0)
					{
						dradf4(num8, num7, _trigCache, data, N + num4 - 1, N + num9 - 1, N + num10 - 1);
					}
					else
					{
						dradf4(num8, num7, data, _trigCache, N + num4 - 1, N + num9 - 1, N + num10 - 1);
					}
					break;
				}
				}
				num3 = num7;
			}
			if (num2 != 1)
			{
				for (int j = 0; j < N; j++)
				{
					data[j] = _trigCache[j];
				}
			}
		}

		private void dradf4(int ido, int l1, IList<float> cc, IList<float> ch, int wa1, int wa2, int wa3)
		{
			int num = l1 * ido;
			int num2 = num;
			int num3 = num2 << 1;
			int num4 = num2 + (num2 << 1);
			int num5 = 0;
			int num8;
			for (int i = 0; i < l1; i++)
			{
				float num6 = cc[num2] + cc[num4];
				float num7 = cc[num5] + cc[num3];
				ch[num8 = num5 << 2] = num6 + num7;
				ch[(ido << 2) + num8 - 1] = num7 - num6;
				ch[(num8 += ido << 1) - 1] = cc[num5] - cc[num3];
				ch[num8] = cc[num4] - cc[num2];
				num2 += ido;
				num4 += ido;
				num5 += ido;
				num3 += ido;
			}
			if (ido < 2)
			{
				return;
			}
			int num9;
			if (ido > 2)
			{
				num2 = 0;
				for (int i = 0; i < l1; i++)
				{
					num4 = num2;
					num3 = num2 << 2;
					num8 = (num9 = ido << 1) + num3;
					for (int j = 2; j < ido; j += 2)
					{
						num5 = (num4 += 2);
						num3 += 2;
						num8 -= 2;
						num5 += num;
						float num10 = _trigCache[wa1 + j - 2] * cc[num5 - 1] + _trigCache[wa1 + j - 1] * cc[num5];
						float num11 = _trigCache[wa1 + j - 2] * cc[num5] - _trigCache[wa1 + j - 1] * cc[num5 - 1];
						num5 += num;
						float num12 = _trigCache[wa2 + j - 2] * cc[num5 - 1] + _trigCache[wa2 + j - 1] * cc[num5];
						float num13 = _trigCache[wa2 + j - 2] * cc[num5] - _trigCache[wa2 + j - 1] * cc[num5 - 1];
						num5 += num;
						float num14 = _trigCache[wa3 + j - 2] * cc[num5 - 1] + _trigCache[wa3 + j - 1] * cc[num5];
						float num15 = _trigCache[wa3 + j - 2] * cc[num5] - _trigCache[wa3 + j - 1] * cc[num5 - 1];
						float num6 = num10 + num14;
						float num16 = num14 - num10;
						float num17 = num11 + num15;
						float num18 = num11 - num15;
						float num19 = cc[num4] + num13;
						float num20 = cc[num4] - num13;
						float num7 = cc[num4 - 1] + num12;
						float num21 = cc[num4 - 1] - num12;
						ch[num3 - 1] = num6 + num7;
						ch[num3] = num17 + num19;
						ch[num8 - 1] = num21 - num18;
						ch[num8] = num16 - num20;
						ch[num3 + num9 - 1] = num18 + num21;
						ch[num3 + num9] = num16 + num20;
						ch[num8 + num9 - 1] = num7 - num6;
						ch[num8 + num9] = num17 - num19;
					}
					num2 += ido;
				}
				if ((ido & 1) != 0)
				{
					return;
				}
			}
			num4 = (num2 = num + ido - 1) + (num << 1);
			num5 = ido << 2;
			num3 = ido;
			num8 = ido << 1;
			num9 = ido;
			for (int i = 0; i < l1; i++)
			{
				float num17 = -0.70710677f * (cc[num2] + cc[num4]);
				float num6 = 0.70710677f * (cc[num2] - cc[num4]);
				ch[num3 - 1] = num6 + cc[num9 - 1];
				ch[num3 + num8 - 1] = cc[num9 - 1] - num6;
				ch[num3] = num17 - cc[num2 + num];
				ch[num3 + num8] = num17 + cc[num2 + num];
				num2 += ido;
				num4 += ido;
				num3 += num5;
				num9 += ido;
			}
		}

		private void dradf2(int ido, int l1, IList<float> cc, IList<float> ch, int wa1)
		{
			int num = 0;
			int num2 = l1 * ido;
			int num3 = num2;
			int num4 = ido << 1;
			for (int i = 0; i < l1; i++)
			{
				ch[num << 1] = cc[num] + cc[num2];
				ch[(num << 1) + num4 - 1] = cc[num] - cc[num2];
				num += ido;
				num2 += ido;
			}
			if (ido < 2)
			{
				return;
			}
			if (ido > 2)
			{
				num = 0;
				num2 = num3;
				for (int j = 0; j < l1; j++)
				{
					num4 = num2;
					int num5 = (num << 1) + (ido << 1);
					int num6 = num;
					int num7 = num + num;
					for (int k = 2; k < ido; k += 2)
					{
						num4 += 2;
						num5 -= 2;
						num6 += 2;
						num7 += 2;
						float num8 = _trigCache[wa1 + k - 2] * cc[num4 - 1] + _trigCache[wa1 + k - 1] * cc[num4];
						float num9 = _trigCache[wa1 + k - 2] * cc[num4] - _trigCache[wa1 + k - 1] * cc[num4 - 1];
						ch[num7] = cc[num6] + num9;
						ch[num5] = num9 - cc[num6];
						ch[num7 - 1] = cc[num6 - 1] + num8;
						ch[num5 - 1] = cc[num6 - 1] - num8;
					}
					num += ido;
					num2 += ido;
				}
				if (ido % 2 == 1)
				{
					return;
				}
			}
			num4 = (num2 = (num = ido) - 1);
			num2 += num3;
			for (int m = 0; m < l1; m++)
			{
				ch[num] = 0f - cc[num2];
				ch[num - 1] = cc[num4];
				num += ido << 1;
				num2 += ido;
				num4 += ido;
			}
		}

		private void dradfg(int ido, int ip, int l1, int idl1, IList<float> cc, IList<float> c1, IList<float> c2, IList<float> ch, IList<float> ch2, int wa)
		{
			float num = MathF.PI * 2f / (float)ip;
			float num2 = (float)Math.Cos(num);
			float num3 = (float)Math.Sin(num);
			int num4 = ip + 1 >> 1;
			int num5 = ido - 1 >> 1;
			int num6 = l1 * ido;
			int num7 = ip * ido;
			int num9;
			int num8;
			int num12;
			int num14;
			int num15;
			if (ido != 1)
			{
				for (int i = 0; i < idl1; i++)
				{
					ch2[i] = c2[i];
				}
				num8 = 0;
				for (int j = 1; j < ip; j++)
				{
					num8 += num6;
					num9 = num8;
					for (int k = 0; k < l1; k++)
					{
						ch[num9] = c1[num9];
						num9 += ido;
					}
				}
				int num10 = -ido;
				num8 = 0;
				if (num5 > l1)
				{
					for (int j = 1; j < ip; j++)
					{
						num8 += num6;
						num10 += ido;
						num9 = -ido + num8;
						for (int k = 0; k < l1; k++)
						{
							int num11 = num10 - 1;
							num9 += ido;
							num12 = num9;
							for (int m = 2; m < ido; m += 2)
							{
								num11 += 2;
								num12 += 2;
								ch[num12 - 1] = _trigCache[wa + num11 - 1] * c1[num12 - 1] + _trigCache[wa + num11] * c1[num12];
								ch[num12] = _trigCache[wa + num11 - 1] * c1[num12] - _trigCache[wa + num11] * c1[num12 - 1];
							}
						}
					}
				}
				else
				{
					for (int j = 1; j < ip; j++)
					{
						num10 += ido;
						int num13 = num10 - 1;
						num8 += num6;
						num9 = num8;
						for (int m = 2; m < ido; m += 2)
						{
							num13 += 2;
							num9 += 2;
							num12 = num9;
							for (int k = 0; k < l1; k++)
							{
								ch[num12 - 1] = _trigCache[wa + num13 - 1] * c1[num12 - 1] + _trigCache[wa + num13] * c1[num12];
								ch[num12] = _trigCache[wa + num13 - 1] * c1[num12] - _trigCache[wa + num13] * c1[num12 - 1];
								num12 += ido;
							}
						}
					}
				}
				num8 = 0;
				num9 = ip * num6;
				if (num5 < l1)
				{
					for (int j = 1; j < num4; j++)
					{
						num8 += num6;
						num9 -= num6;
						num12 = num8;
						num14 = num9;
						for (int m = 2; m < ido; m += 2)
						{
							num12 += 2;
							num14 += 2;
							num15 = num12 - ido;
							int num16 = num14 - ido;
							for (int k = 0; k < l1; k++)
							{
								num15 += ido;
								num16 += ido;
								c1[num15 - 1] = ch[num15 - 1] + ch[num16 - 1];
								c1[num16 - 1] = ch[num15] - ch[num16];
								c1[num15] = ch[num15] + ch[num16];
								c1[num16] = ch[num16 - 1] - ch[num15 - 1];
							}
						}
					}
				}
				else
				{
					for (int j = 1; j < num4; j++)
					{
						num8 += num6;
						num9 -= num6;
						num12 = num8;
						num14 = num9;
						for (int k = 0; k < l1; k++)
						{
							num15 = num12;
							int num16 = num14;
							for (int m = 2; m < ido; m += 2)
							{
								num15 += 2;
								num16 += 2;
								c1[num15 - 1] = ch[num15 - 1] + ch[num16 - 1];
								c1[num16 - 1] = ch[num15] - ch[num16];
								c1[num15] = ch[num15] + ch[num16];
								c1[num16] = ch[num16 - 1] - ch[num15 - 1];
							}
							num12 += ido;
							num14 += ido;
						}
					}
				}
			}
			for (int i = 0; i < idl1; i++)
			{
				c2[i] = ch2[i];
			}
			num8 = 0;
			num9 = ip * idl1;
			for (int j = 1; j < num4; j++)
			{
				num8 += num6;
				num9 -= num6;
				num12 = num8 - ido;
				num14 = num9 - ido;
				for (int k = 0; k < l1; k++)
				{
					num12 += ido;
					num14 += ido;
					c1[num12] = ch[num12] + ch[num14];
					c1[num14] = ch[num14] - ch[num12];
				}
			}
			float num17 = 1f;
			float num18 = 0f;
			num8 = 0;
			num9 = ip * idl1;
			num12 = (ip - 1) * idl1;
			for (int n = 1; n < num4; n++)
			{
				num8 += idl1;
				num9 -= idl1;
				float num19 = num2 * num17 - num3 * num18;
				num18 = num2 * num18 + num3 * num17;
				num17 = num19;
				num14 = num8;
				num15 = num9;
				int num16 = num12;
				int num20 = idl1;
				for (int i = 0; i < idl1; i++)
				{
					ch2[num14++] = c2[i] + num17 * c2[num20++];
					ch2[num15++] = num18 * c2[num16++];
				}
				float num21 = num17;
				float num22 = num18;
				float num23 = num17;
				float num24 = num18;
				num14 = idl1;
				num15 = (ip - 1) * idl1;
				for (int j = 2; j < num4; j++)
				{
					num14 += idl1;
					num15 -= idl1;
					float num25 = num21 * num23 - num22 * num24;
					num24 = num21 * num24 + num22 * num23;
					num23 = num25;
					num16 = num8;
					num20 = num9;
					int num26 = num14;
					int num27 = num15;
					for (int i = 0; i < idl1; i++)
					{
						ch2[num16++] += num23 * c2[num26++];
						ch2[num20++] += num24 * c2[num27++];
					}
				}
			}
			num8 = 0;
			for (int j = 1; j < num4; j++)
			{
				num8 += idl1;
				num9 = num8;
				for (int i = 0; i < idl1; i++)
				{
					ch2[i] += c2[num9++];
				}
			}
			if (ido < l1)
			{
				for (int m = 0; m < ido; m++)
				{
					num8 = m;
					num9 = m;
					for (int k = 0; k < l1; k++)
					{
						cc[num9] = ch[num8];
						num8 += ido;
						num9 += num7;
					}
				}
			}
			else
			{
				num8 = 0;
				num9 = 0;
				for (int k = 0; k < l1; k++)
				{
					num12 = num8;
					num14 = num9;
					for (int m = 0; m < ido; m++)
					{
						cc[num14++] = ch[num12++];
					}
					num8 += ido;
					num9 += num7;
				}
			}
			num8 = 0;
			num9 = ido << 1;
			num12 = 0;
			num14 = ip * num6;
			for (int j = 1; j < num4; j++)
			{
				num8 += num9;
				num12 += num6;
				num14 -= num6;
				num15 = num8;
				int num16 = num12;
				int num20 = num14;
				for (int k = 0; k < l1; k++)
				{
					cc[num15 - 1] = ch[num16];
					cc[num15] = ch[num20];
					num15 += num7;
					num16 += ido;
					num20 += ido;
				}
			}
			if (ido == 1)
			{
				return;
			}
			if (num5 >= l1)
			{
				num8 = -ido;
				num12 = 0;
				num14 = 0;
				num15 = ip * num6;
				for (int j = 1; j < num4; j++)
				{
					num8 += num9;
					num12 += num9;
					num14 += num6;
					num15 -= num6;
					int num16 = num8;
					int num20 = num12;
					int num26 = num14;
					int num27 = num15;
					for (int k = 0; k < l1; k++)
					{
						for (int m = 2; m < ido; m += 2)
						{
							int num28 = ido - m;
							cc[m + num20 - 1] = ch[m + num26 - 1] + ch[m + num27 - 1];
							cc[num28 + num16 - 1] = ch[m + num26 - 1] - ch[m + num27 - 1];
							cc[m + num20] = ch[m + num26] + ch[m + num27];
							cc[num28 + num16] = ch[m + num27] - ch[m + num26];
						}
						num16 += num7;
						num20 += num7;
						num26 += ido;
						num27 += ido;
					}
				}
				return;
			}
			num8 = -ido;
			num12 = 0;
			num14 = 0;
			num15 = ip * num6;
			for (int j = 1; j < num4; j++)
			{
				num8 += num9;
				num12 += num9;
				num14 += num6;
				num15 -= num6;
				for (int m = 2; m < ido; m += 2)
				{
					int num16 = ido + num8 - m;
					int num20 = m + num12;
					int num26 = m + num14;
					int num27 = m + num15;
					for (int k = 0; k < l1; k++)
					{
						cc[num20 - 1] = ch[num26 - 1] + ch[num27 - 1];
						cc[num16 - 1] = ch[num26 - 1] - ch[num27 - 1];
						cc[num20] = ch[num26] + ch[num27];
						cc[num16] = ch[num27] - ch[num26];
						num16 += num7;
						num20 += num7;
						num26 += ido;
						num27 += ido;
					}
				}
			}
		}
	}
}
