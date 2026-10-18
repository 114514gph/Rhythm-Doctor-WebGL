using System;
using System.Collections.Generic;
using OggVorbisEncoder.Setup;

namespace OggVorbisEncoder.Lookups
{
	public class ResidueLookup
	{
		private readonly CodeBook[][] _partitionBooks;

		private readonly CodeBook _phraseBook;

		private readonly ResidueEntry _residue;

		private readonly int _stages;

		public ResidueLookup(ResidueEntry residue, CodeBook[] fullBooks)
		{
			_residue = residue;
			_phraseBook = fullBooks[residue.GroupBook];
			int num = 0;
			int num2 = 0;
			_partitionBooks = new CodeBook[residue.Partitions][];
			for (int i = 0; i < _partitionBooks.Length; i++)
			{
				int num3 = Encoding.Log(residue.SecondStages[i]);
				if (num3 == 0)
				{
					continue;
				}
				if (num3 > num2)
				{
					num2 = num3;
				}
				_partitionBooks[i] = new CodeBook[num3];
				for (int j = 0; j < num3; j++)
				{
					if ((residue.SecondStages[i] & (1 << j)) != 0)
					{
						_partitionBooks[i][j] = fullBooks[residue.BookList[num++]];
					}
				}
			}
			_stages = num2;
		}

		public int Forward(EncodeBuffer buffer, int pcmend, int[][] couples, bool[] nonzero, int channels, int[][] partword)
		{
			return _residue.ResidueType switch
			{
				ResidueType.One => Res1Forward(buffer, pcmend, couples, nonzero, channels, partword), 
				ResidueType.Two => Res2Forward(buffer, pcmend, couples, nonzero, channels, partword), 
				_ => throw new NotImplementedException(), 
			};
		}

		private int Res1Forward(EncodeBuffer buffer, int pcmend, int[][] couples, bool[] nonzero, int channels, int[][] partword)
		{
			int num = 0;
			for (int i = 0; i < channels; i++)
			{
				if (nonzero[i])
				{
					couples[num++] = couples[i];
				}
			}
			if (num <= 0)
			{
				return 0;
			}
			return InnerForward(buffer, couples, num, partword);
		}

		private int Res2Forward(EncodeBuffer buffer, int pcmend, int[][] couples, bool[] nonzero, int channels, int[][] partword)
		{
			int num = pcmend / 2;
			bool flag = false;
			int[] array = new int[channels * num];
			for (int i = 0; i < channels; i++)
			{
				int[] array2 = couples[i];
				flag = flag || nonzero[i];
				int num2 = 0;
				int num3 = i;
				while (num2 < num)
				{
					array[num3] = array2[num2];
					num2++;
					num3 += channels;
				}
			}
			if (!flag)
			{
				return 0;
			}
			return InnerForward(buffer, new int[1][] { array }, 1, partword);
		}

		private int InnerForward(EncodeBuffer buffer, int[][] work, int channels, int[][] partword)
		{
			int num = (_residue.End - _residue.Begin) / _residue.Grouping;
			for (int i = 0; i < _stages; i++)
			{
				int num2 = 0;
				while (num2 < num)
				{
					if (i == 0)
					{
						for (int j = 0; j < channels; j++)
						{
							int num3 = partword[j][num2];
							for (int k = 1; k < _phraseBook.Dimensions; k++)
							{
								num3 *= _residue.Partitions;
								if (num2 + k < num)
								{
									num3 += partword[j][num2 + k];
								}
							}
							if (num3 < _phraseBook.Entries)
							{
								buffer.WriteBook(_phraseBook, num3);
							}
						}
					}
					int num4 = 0;
					while (num4 < _phraseBook.Dimensions && num2 < num)
					{
						int offset = num2 * _residue.Grouping + _residue.Begin;
						for (int l = 0; l < channels; l++)
						{
							if ((_residue.SecondStages[partword[l][num2]] & (1 << i)) != 0)
							{
								CodeBook codeBook = _partitionBooks[partword[l][num2]][i];
								if (codeBook != null)
								{
									EncodePart(buffer, work[l], offset, _residue.Grouping, codeBook);
								}
							}
						}
						num4++;
						num2++;
					}
				}
			}
			return 0;
		}

		private void EncodePart(EncodeBuffer buffer, int[] vec, int offset, int n, CodeBook book)
		{
			int num = n / book.Dimensions;
			for (int i = 0; i < num; i++)
			{
				int a = LocalBookBestError(book, vec, offset + i * book.Dimensions);
				buffer.WriteBook(book, a);
			}
		}

		private static int LocalBookBestError(CodeBook book, int[] vec, int offset)
		{
			int num = book.QuantValues >> 1;
			int num2 = 0;
			int[] array = new int[8];
			if (book.Delta != 1)
			{
				int i = 0;
				int num3 = book.Dimensions;
				for (; i < book.Dimensions; i++)
				{
					int num4 = (vec[offset + --num3] - book.MinVal + (book.Delta >> 1)) / book.Delta;
					int num5 = ((num4 < num) ? ((num - num4 << 1) - 1) : (num4 - num << 1));
					num2 = num2 * book.QuantValues + ((num5 >= 0) ? ((num5 >= book.QuantValues) ? (book.QuantValues - 1) : num5) : 0);
					array[num3] = num4 * book.Delta + book.MinVal;
				}
			}
			else
			{
				int i = 0;
				int num3 = book.Dimensions;
				for (; i < book.Dimensions; i++)
				{
					int num6 = vec[offset + --num3] - book.MinVal;
					int num7 = ((num6 < num) ? ((num - num6 << 1) - 1) : (num6 - num << 1));
					num2 = num2 * book.QuantValues + ((num7 >= 0) ? ((num7 >= book.QuantValues) ? (book.QuantValues - 1) : num7) : 0);
					array[num3] = num6 * book.Delta + book.MinVal;
				}
			}
			if (book.StaticBook.LengthList[num2] <= 0)
			{
				int num8 = -1;
				int[] array2 = new int[8];
				int num9 = book.MinVal + book.Delta * (book.QuantValues - 1);
				for (int i = 0; i < book.Entries; i++)
				{
					if (book.StaticBook.LengthList[i] > 0)
					{
						int num10 = 0;
						for (int j = 0; j < book.Dimensions; j++)
						{
							int num11 = array2[j] - vec[offset + j];
							num10 += num11 * num11;
						}
						if (num8 == -1 || num10 < num8)
						{
							for (int k = 0; k < array2.Length; k++)
							{
								array[k] = array2[k];
							}
							num8 = num10;
							num2 = i;
						}
					}
					int num12 = 0;
					while (array2[num12] >= num9)
					{
						array2[num12++] = 0;
					}
					if (array2[num12] >= 0)
					{
						array2[num12] += book.Delta;
					}
					array2[num12] = -array2[num12];
				}
			}
			if (num2 > -1)
			{
				for (int i = 0; i < book.Dimensions; i++)
				{
					vec[offset++] -= array[i];
				}
			}
			return num2;
		}

		public int[][] Class(int[][] couples, IList<bool> nonzero, int channels)
		{
			for (int i = 0; i < channels; i++)
			{
				if (nonzero[i])
				{
					switch (_residue.ResidueType)
					{
					case ResidueType.One:
						return ResOneClass(couples, channels);
					case ResidueType.Two:
						return ResTwoClass(couples, channels);
					}
				}
			}
			return null;
		}

		private int[][] ResOneClass(int[][] couples, int channels)
		{
			int num = (_residue.End - _residue.Begin) / _residue.Grouping;
			int[][] array = new int[channels][];
			for (int i = 0; i < channels; i++)
			{
				array[i] = new int[num];
			}
			int num2 = 0;
			int num3 = 0;
			for (int j = 0; j < num; j++)
			{
				int num4 = j * _residue.Grouping + _residue.Begin;
				for (num2 = 0; num2 < channels; num2++)
				{
					int num5 = 0;
					int num6 = 0;
					for (num3 = 0; num3 < _residue.Grouping; num3++)
					{
						if (Math.Abs(couples[num2][num4 + num3]) > num5)
						{
							num5 = Math.Abs(couples[num2][num4 + num3]);
						}
						num6 += Math.Abs(couples[num2][num4 + num3]);
					}
					num6 = (int)((float)num6 * (100f / (float)_residue.Grouping));
					for (num3 = 0; num3 < _residue.Partitions - 1 && (num5 > _residue.ClassMetric1[num3] || (_residue.ClassMetric2[num3] >= 0 && num6 >= _residue.ClassMetric2[num3])); num3++)
					{
					}
					array[num2][j] = num3;
				}
			}
			return array;
		}

		private int[][] ResTwoClass(int[][] couples, int channels)
		{
			int num = (_residue.End - _residue.Begin) / _residue.Grouping;
			int[][] array = new int[1][] { new int[num] };
			int i = 0;
			int num2 = _residue.Begin / channels;
			for (; i < num; i++)
			{
				int num3 = 0;
				int num4 = 0;
				for (int j = 0; j < _residue.Grouping; j += channels)
				{
					int num5 = Math.Abs(couples[0][num2]);
					if (num5 > num3)
					{
						num3 = num5;
					}
					for (int k = 1; k < channels; k++)
					{
						num5 = Math.Abs(couples[k][num2]);
						if (num5 > num4)
						{
							num4 = num5;
						}
					}
					num2++;
				}
				int l;
				for (l = 0; l < _residue.Partitions - 1 && (num3 > _residue.ClassMetric1[l] || num4 > _residue.ClassMetric2[l]); l++)
				{
				}
				array[0][i] = l;
			}
			return array;
		}
	}
}
