using System;
using System.Collections.Generic;
using System.Linq;
using OggVorbisEncoder.Setup;

namespace OggVorbisEncoder.Lookups
{
	public class FloorLookup
	{
		private struct FitAccumulation
		{
			public int x0;

			public int x1;

			public int xa;

			public int ya;

			public int x2a;

			public int xya;

			public int an;

			public int xb;

			public int yb;

			public int x2b;

			public int xyb;

			public int bn;
		}

		private const int Posit = 63;

		private readonly Floor _floor;

		private readonly int[] _forwardIndex = new int[65];

		private readonly int[] _highNeighbor = new int[63];

		private readonly int[] _lowNeighbor = new int[63];

		private readonly int _n;

		private readonly int _posts;

		private readonly int _quantQ;

		private readonly int[] _reverseIndex = new int[65];

		private readonly int[] _sortedIndex = new int[65];

		public FloorLookup(Floor floor)
		{
			_floor = floor;
			_n = floor.PostList[1];
			int num = 0;
			int[] partitionClass = floor.PartitionClass;
			foreach (int num2 in partitionClass)
			{
				num += floor.ClassDimensions[num2];
			}
			num = (_posts = num + 2);
			var array = (from o in floor.PostList.Select((int v, int index) => new
				{
					Value = v,
					Index = index
				})
				orderby o.Value
				select o).ToArray();
			for (int num3 = 0; num3 < num; num3++)
			{
				_forwardIndex[num3] = array[num3].Index;
			}
			for (int num4 = 0; num4 < num; num4++)
			{
				_reverseIndex[_forwardIndex[num4]] = num4;
			}
			for (int num5 = 0; num5 < num; num5++)
			{
				_sortedIndex[num5] = floor.PostList[_forwardIndex[num5]];
			}
			switch (floor.Mult)
			{
			case 1:
				_quantQ = 256;
				break;
			case 2:
				_quantQ = 128;
				break;
			case 3:
				_quantQ = 86;
				break;
			case 4:
				_quantQ = 64;
				break;
			}
			for (int num6 = 0; num6 < num - 2; num6++)
			{
				int num7 = 0;
				int num8 = 1;
				int num9 = 0;
				int num10 = _n;
				int num11 = floor.PostList[num6 + 2];
				for (int num12 = 0; num12 < num6 + 2; num12++)
				{
					int num13 = floor.PostList[num12];
					if (num13 > num9 && num13 < num11)
					{
						num7 = num12;
						num9 = num13;
					}
					if (num13 < num10 && num13 > num11)
					{
						num8 = num12;
						num10 = num13;
					}
				}
				_lowNeighbor[num6] = num7;
				_highNeighbor[num6] = num8;
			}
		}

		public int[] Fit(IList<float> logmdct, IList<float> logmask)
		{
			int n = _n;
			int num = 0;
			FitAccumulation[] array = new FitAccumulation[64];
			int[] array2 = new int[65];
			int[] array3 = new int[65];
			int[] array4 = new int[65];
			int[] array5 = new int[65];
			int[] array6 = new int[65];
			for (int i = 0; i < _posts; i++)
			{
				array2[i] = -200;
			}
			for (int j = 0; j < _posts; j++)
			{
				array3[j] = -200;
			}
			for (int k = 0; k < _posts; k++)
			{
				array4[k] = 0;
			}
			for (int l = 0; l < _posts; l++)
			{
				array5[l] = 1;
			}
			for (int m = 0; m < _posts; m++)
			{
				array6[m] = -1;
			}
			if (_posts == 0)
			{
				num = AccumulateFit(logmask, logmdct, 0, n, ref array[0], n);
			}
			else
			{
				for (int num2 = 0; num2 < _posts - 1; num2++)
				{
					num += AccumulateFit(logmask, logmdct, _sortedIndex[num2], _sortedIndex[num2 + 1], ref array[num2], n);
				}
			}
			if (num != 0)
			{
				FitLine(array, 0, _posts - 1, out var y, out var y2);
				array2[0] = y;
				array3[0] = y;
				array3[1] = y2;
				array2[1] = y2;
				for (int num3 = 2; num3 < _posts; num3++)
				{
					int num4 = _reverseIndex[num3];
					int num5 = array4[num4];
					int num6 = array5[num4];
					if (array6[num5] == num6)
					{
						continue;
					}
					int num7 = _reverseIndex[num5];
					int num8 = _reverseIndex[num6];
					array6[num5] = num6;
					int x = _floor.PostList[num5];
					int x2 = _floor.PostList[num6];
					int num9 = PostY(array2, array3, num5);
					int num10 = PostY(array2, array3, num6);
					if (num9 == -1 || num10 == -1)
					{
						throw new InvalidOperationException("An error occurred during minimization");
					}
					if (InspectError(x, x2, num9, num10, logmask, logmdct))
					{
						int num11 = FitLine(array, num7, num4 - num7, out var y3, out var y4);
						int num12 = FitLine(array, num4, num8 - num4, out var y5, out var y6);
						if (num11 != 0)
						{
							y3 = num9;
							y4 = y5;
						}
						if (num12 != 0)
						{
							y5 = y4;
							y6 = num10;
						}
						if (num11 != 0 && num12 != 0)
						{
							array2[num3] = -200;
							array3[num3] = -200;
							continue;
						}
						array3[num5] = y3;
						if (num5 == 0)
						{
							array2[num5] = y3;
						}
						array2[num3] = y4;
						array3[num3] = y5;
						array2[num6] = y6;
						if (num6 == 1)
						{
							array3[num6] = y6;
						}
						if (y4 >= 0 || y5 >= 0)
						{
							int num13 = num4 - 1;
							while (num13 >= 0 && array5[num13] == num6)
							{
								array5[num13] = num3;
								num13--;
							}
							for (int num14 = num4 + 1; num14 < _posts && array4[num14] == num5; num14++)
							{
								array4[num14] = num3;
							}
						}
					}
					else
					{
						array2[num3] = -200;
						array3[num3] = -200;
					}
				}
				int[] array7 = new int[_posts];
				array7[0] = PostY(array2, array3, 0);
				array7[1] = PostY(array2, array3, 1);
				for (int num15 = 2; num15 < _posts; num15++)
				{
					int num16 = _lowNeighbor[num15 - 2];
					int num17 = _highNeighbor[num15 - 2];
					int x3 = _floor.PostList[num16];
					int x4 = _floor.PostList[num17];
					y = array7[num16];
					y2 = array7[num17];
					int num18 = RenderPoint(x3, x4, y, y2, _floor.PostList[num15]);
					int num19 = PostY(array2, array3, num15);
					if (num19 >= 0 && num18 != num19)
					{
						array7[num15] = num19;
					}
					else
					{
						array7[num15] = num18 | 0x8000;
					}
				}
				return array7;
			}
			return null;
		}

		private bool InspectError(int x0, int x1, int y0, int y1, IList<float> mask, IList<float> mdct)
		{
			int num = y1 - y0;
			int num2 = x1 - x0;
			int num3 = Math.Abs(num);
			int num4 = num / num2;
			int num5 = ((num < 0) ? (num4 - 1) : (num4 + 1));
			int num6 = x0;
			int num7 = y0;
			int num8 = 0;
			int num9 = DecibelQuant(mask[num6]);
			int num10 = 0;
			num3 -= Math.Abs(num4 * num2);
			int num11 = num7 - num9;
			num11 *= num11;
			num10++;
			if (mdct[num6] + _floor.TwoFitAtten >= mask[num6])
			{
				if ((float)num7 + _floor.MaxOver < (float)num9)
				{
					return true;
				}
				if ((float)num7 - _floor.MaxUnder > (float)num9)
				{
					return true;
				}
			}
			while (++num6 < x1)
			{
				num8 += num3;
				if (num8 >= num2)
				{
					num8 -= num2;
					num7 += num5;
				}
				else
				{
					num7 += num4;
				}
				num9 = DecibelQuant(mask[num6]);
				num11 += (num7 - num9) * (num7 - num9);
				num10++;
				if (mdct[num6] + _floor.TwoFitAtten >= mask[num6] && num9 != 0)
				{
					if ((float)num7 + _floor.MaxOver < (float)num9)
					{
						return true;
					}
					if ((float)num7 - _floor.MaxUnder > (float)num9)
					{
						return true;
					}
				}
			}
			if (_floor.MaxOver * _floor.MaxOver / (float)num10 > _floor.MaxError)
			{
				return false;
			}
			if (_floor.MaxUnder * _floor.MaxUnder / (float)num10 > _floor.MaxError)
			{
				return false;
			}
			if ((float)(num11 / num10) > _floor.MaxError)
			{
				return true;
			}
			return false;
		}

		private int FitLine(IList<FitAccumulation> acc, int offset, int fits, out int y0, out int y1)
		{
			y0 = -200;
			y1 = -200;
			double num = 0.0;
			double num2 = 0.0;
			double num3 = 0.0;
			double num4 = 0.0;
			double num5 = 0.0;
			int x = acc[offset].x0;
			int x2 = acc[offset + fits - 1].x1;
			for (int i = 0; i < fits; i++)
			{
				double num6 = (double)((float)(acc[offset + i].bn + acc[offset + i].an) * _floor.TwoFitWeight / (float)(acc[offset + i].an + 1)) + 1.0;
				num += (double)acc[offset + i].xb + (double)acc[offset + i].xa * num6;
				num2 += (double)acc[offset + i].yb + (double)acc[offset + i].ya * num6;
				num3 += (double)acc[offset + i].x2b + (double)acc[offset + i].x2a * num6;
				num4 += (double)acc[offset + i].xyb + (double)acc[offset + i].xya * num6;
				num5 += (double)acc[offset + i].bn + (double)acc[offset + i].an * num6;
			}
			if (y0 >= 0)
			{
				num += (double)x;
				num2 += (double)y0;
				num3 += (double)(x * x);
				num4 += (double)(y0 * x);
				num5++;
			}
			if (y1 >= 0)
			{
				num += (double)x2;
				num2 += (double)y1;
				num3 += (double)(x2 * x2);
				num4 += (double)(y1 * x2);
				num5++;
			}
			double num7 = num5 * num3 - num * num;
			if (num7 > 0.0)
			{
				double num8 = (num2 * num3 - num4 * num) / num7;
				double num9 = (num5 * num4 - num * num2) / num7;
				y0 = (int)Math.Round(num8 + num9 * (double)x);
				y1 = (int)Math.Round(num8 + num9 * (double)x2);
				if (y0 > 1023)
				{
					y0 = 1023;
				}
				if (y1 > 1023)
				{
					y1 = 1023;
				}
				if (y0 < 0)
				{
					y0 = 0;
				}
				if (y1 < 0)
				{
					y1 = 0;
				}
				return 0;
			}
			y0 = 0;
			y1 = 0;
			return 1;
		}

		private static int RenderPoint(int x0, int x1, int y0, int y1, int x)
		{
			y0 &= 0x7FFF;
			y1 &= 0x7FFF;
			int num = y1 - y0;
			int num2 = x1 - x0;
			int num3 = Math.Abs(num) * (x - x0) / num2;
			if (num < 0)
			{
				return y0 - num3;
			}
			return y0 + num3;
		}

		private static int PostY(int[] a, int[] b, int pos)
		{
			if (a[pos] < 0)
			{
				return b[pos];
			}
			if (b[pos] < 0)
			{
				return a[pos];
			}
			return a[pos] + b[pos] >> 1;
		}

		private int AccumulateFit(IList<float> flr, IList<float> mdct, int x0, int x1, ref FitAccumulation fits, int n)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			int num7 = 0;
			int num8 = 0;
			int num9 = 0;
			int num10 = 0;
			fits.x0 = x0;
			fits.x1 = x1;
			if (x1 >= n)
			{
				x1 = n - 1;
			}
			for (int i = x0; i <= x1; i++)
			{
				int num11 = DecibelQuant(flr[i]);
				if (num11 != 0)
				{
					if (mdct[i] + _floor.TwoFitAtten >= flr[i])
					{
						num += i;
						num2 += num11;
						num3 += i * i;
						num4 += i * num11;
						num5++;
					}
					else
					{
						num6 += i;
						num7 += num11;
						num8 += i * i;
						num9 += i * num11;
						num10++;
					}
				}
			}
			fits.xa = num;
			fits.ya = num2;
			fits.x2a = num3;
			fits.xya = num4;
			fits.an = num5;
			fits.xb = num6;
			fits.yb = num7;
			fits.x2b = num8;
			fits.xyb = num9;
			fits.bn = num10;
			return num5;
		}

		private static int DecibelQuant(float x)
		{
			int num = (int)(x * 7.3142858f + 1023.5f);
			if (num > 1023)
			{
				return 1023;
			}
			if (num < 0)
			{
				return 0;
			}
			return num;
		}

		public bool Encode(EncodeBuffer buffer, IList<IStaticCodeBook> staticBooks, IList<CodeBook> books, int[] post, int[] ilogmask, int pcmEnd, int n)
		{
			int[] array = new int[65];
			if (post != null)
			{
				for (int i = 0; i < _posts; i++)
				{
					int num = post[i] & 0x7FFF;
					switch (_floor.Mult)
					{
					case 1:
						num >>= 2;
						break;
					case 2:
						num >>= 3;
						break;
					case 3:
						num /= 12;
						break;
					case 4:
						num >>= 4;
						break;
					}
					post[i] = num | (post[i] & 0x8000);
				}
				array[0] = post[0];
				array[1] = post[1];
				for (int j = 2; j < _posts; j++)
				{
					int num2 = _lowNeighbor[j - 2];
					int num3 = _highNeighbor[j - 2];
					int x = _floor.PostList[num2];
					int x2 = _floor.PostList[num3];
					int y = post[num2];
					int y2 = post[num3];
					int num4 = RenderPoint(x, x2, y, y2, _floor.PostList[j]);
					if ((post[j] & 0x8000) != 0 || num4 == post[j])
					{
						post[j] = num4 | 0x8000;
						array[j] = 0;
						continue;
					}
					int num5 = ((_quantQ - num4 < num4) ? (_quantQ - num4) : num4);
					int num6 = post[j] - num4;
					num6 = ((num6 < 0) ? ((num6 >= -num5) ? (-1 - (num6 << 1)) : (num5 - num6 - 1)) : ((num6 < num5) ? (num6 << 1) : (num6 + num5)));
					array[j] = num6;
					post[num2] &= 32767;
					post[num3] &= 32767;
				}
				buffer.Write(1u, 1);
				int bits = Encoding.Log(_quantQ - 1);
				buffer.Write((uint)array[0], bits);
				buffer.Write((uint)array[1], bits);
				int k = 0;
				int num7 = 2;
				for (; k < _floor.PartitionClass.Length; k++)
				{
					int num8 = _floor.PartitionClass[k];
					int num9 = _floor.ClassDimensions[num8];
					int num10 = _floor.ClassSubs[num8];
					int num11 = 1 << num10;
					int num12 = 0;
					int num13 = 0;
					int[] array2 = new int[8];
					if (num10 != 0)
					{
						int[] array3 = new int[8];
						for (int l = 0; l < num11; l++)
						{
							if (_floor.ClassSubBook[num8][l] < 0)
							{
								array3[l] = 1;
							}
							else
							{
								array3[l] = staticBooks[_floor.ClassSubBook[num8][l]].LengthList.Length;
							}
						}
						for (int l = 0; l < num9; l++)
						{
							for (int m = 0; m < num11; m++)
							{
								if (array[num7 + l] < array3[m])
								{
									array2[l] = m;
									break;
								}
							}
							num12 |= array2[l] << num13;
							num13 += num10;
						}
						buffer.WriteBook(books[_floor.ClassBook[num8]], num12);
					}
					for (int l = 0; l < num9; l++)
					{
						int num14 = _floor.ClassSubBook[num8][array2[l]];
						if (num14 >= 0 && array[num7 + l] < books[num14].Entries)
						{
							buffer.WriteBook(books[num14], array[num7 + l]);
						}
					}
					num7 += num9;
				}
				int num15 = 0;
				int x3 = 0;
				int num16 = post[0] * _floor.Mult;
				for (int num17 = 1; num17 < _posts; num17++)
				{
					int num18 = _forwardIndex[num17];
					int num19 = post[num18] & 0x7FFF;
					if (num19 == post[num18])
					{
						num19 *= _floor.Mult;
						num15 = _floor.PostList[num18];
						RenderLine0(n, x3, num15, num16, num19, ilogmask);
						x3 = num15;
						num16 = num19;
					}
				}
				for (int num20 = num15; num20 < pcmEnd / 2; num20++)
				{
					ilogmask[num20] = num16;
				}
				return true;
			}
			buffer.Write(0u, 1);
			Array.Clear(ilogmask, 0, pcmEnd / 2);
			return false;
		}

		private void RenderLine0(int n, int x0, int x1, int y0, int y1, int[] d)
		{
			int num = y1 - y0;
			int num2 = x1 - x0;
			int num3 = Math.Abs(num);
			int num4 = num / num2;
			int num5 = ((num < 0) ? (num4 - 1) : (num4 + 1));
			int num6 = x0;
			int num7 = y0;
			int num8 = 0;
			num3 -= Math.Abs(num4 * num2);
			if (n > x1)
			{
				n = x1;
			}
			if (num6 < n)
			{
				d[num6] = num7;
			}
			while (++num6 < n)
			{
				num8 += num3;
				if (num8 >= num2)
				{
					num8 -= num2;
					num7 += num5;
				}
				else
				{
					num7 += num4;
				}
				d[num6] = num7;
			}
		}
	}
}
