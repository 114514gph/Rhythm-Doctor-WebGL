using System;
using System.Linq;
using OggVorbisEncoder.Setup;

namespace OggVorbisEncoder.Lookups
{
	public class EnvelopeLookup
	{
		public const int EnvelopePost = 2;

		private const int SearchStep = 64;

		private const int WindowLength = 128;

		private const int EnvelopeWindow = 4;

		private const int EnvelopeMinStretch = 2;

		private const int EnvelopeMaxStretch = 12;

		private readonly EnvelopeBand[] _bands;

		private readonly EnvelopeFilterState[] _filters;

		private readonly MdctLookup _mdctLookup;

		private readonly float[] _mdctWindow;

		private readonly float _minEnergy;

		private readonly PsyGlobal _psyGlobal;

		private int _current;

		private int _currentMark;

		private int _cursor;

		private int[] _mark;

		private int _stretch;

		public EnvelopeLookup(PsyGlobal psyGlobal, VorbisInfo info)
		{
			_psyGlobal = psyGlobal;
			CodecSetup codecSetup = info.CodecSetup;
			_minEnergy = codecSetup.PsyGlobalParam.PreEchoMinEnergy;
			_cursor = codecSetup.BlockSizes[1] / 2;
			_mdctWindow = new float[128];
			_mdctLookup = new MdctLookup(128);
			for (int i = 0; i < _mdctWindow.Length; i++)
			{
				_mdctWindow[i] = (float)Math.Sin((double)i / 127.0 * Math.PI);
				_mdctWindow[i] *= _mdctWindow[i];
			}
			_bands = new EnvelopeBand[7];
			_bands[0] = new EnvelopeBand(2, 4);
			_bands[1] = new EnvelopeBand(4, 5);
			_bands[2] = new EnvelopeBand(6, 6);
			_bands[3] = new EnvelopeBand(9, 8);
			_bands[4] = new EnvelopeBand(13, 8);
			_bands[5] = new EnvelopeBand(17, 8);
			_bands[6] = new EnvelopeBand(22, 8);
			_filters = (from s in Enumerable.Range(0, 7 * info.Channels)
				select new EnvelopeFilterState()).ToArray();
			_mark = new int[128];
		}

		public void Shift(int shift)
		{
			int num = _current / 64 + 2;
			int num2 = shift / 64;
			Array.Copy(_mark, num2, _mark, 0, num - num2);
			_current -= shift;
			if (_currentMark >= 0)
			{
				_currentMark -= shift;
			}
			_cursor -= shift;
		}

		public bool Mark(int beginWindow, int endWindow)
		{
			if (_currentMark >= beginWindow && _currentMark < endWindow)
			{
				return true;
			}
			int num = beginWindow / 64;
			int num2 = endWindow / 64;
			for (int i = num; i < num2; i++)
			{
				if (_mark[i] != 0)
				{
					return true;
				}
			}
			return false;
		}

		public int Search(float[][] pcm, int pcmCurrent, int centerWindow, int testWindow)
		{
			int num = _current / 64;
			int num2 = pcmCurrent / 64 - 4;
			if (num < 0)
			{
				num = 0;
			}
			int num3 = num2 + 4 + 2;
			if (num3 > _mark.Length)
			{
				_mark = new int[num3];
			}
			for (int i = num; i < num2; i++)
			{
				int num4 = 0;
				_stretch++;
				if (_stretch > 24)
				{
					_stretch = 24;
				}
				for (int j = 0; j < pcm.Length; j++)
				{
					num4 |= AmpPcm(pcm[j], 64 * i, j * 7);
				}
				_mark[i + 2] = 0;
				if ((num4 & 1) != 0)
				{
					_mark[i] = 1;
					_mark[i + 1] = 1;
				}
				if ((num4 & 2) != 0)
				{
					_mark[i] = 1;
					if (i > 0)
					{
						_mark[i - 1] = 1;
					}
				}
				if ((num4 & 4) != 0)
				{
					_stretch = -1;
				}
			}
			_current = num2 * 64;
			for (int k = _cursor; k < _current - 64; k += 64)
			{
				if (k >= testWindow)
				{
					return 1;
				}
				_cursor = k;
				if (_mark[k / 64] != 0 && k > centerWindow)
				{
					_currentMark = k;
					if (k < testWindow)
					{
						return 0;
					}
					return 1;
				}
			}
			return -1;
		}

		private int AmpPcm(float[] pcm, int pcmOffset, int filterOffset)
		{
			int num = 0;
			float[] array = new float[128];
			float num2 = _psyGlobal.StretchPenalty - (float)(_stretch / 2 - 2);
			if (num2 < 0f)
			{
				num2 = 0f;
			}
			if (num2 > _psyGlobal.StretchPenalty)
			{
				num2 = _psyGlobal.StretchPenalty;
			}
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = pcm[pcmOffset + i] * _mdctWindow[i];
			}
			_mdctLookup.Forward(array, array);
			float input = (float)((double)(array[0] * array[0]) + 0.7 * (double)array[1] * (double)array[1] + 0.2 * (double)array[2] * (double)array[2]);
			float num3 = _filters[filterOffset].SpreadNearDc(input);
			for (int j = 0; j < 64; j += 2)
			{
				float x = array[j] * array[j] + array[j + 1] * array[j + 1];
				x = x.ToDecibel() * 0.5f;
				if (x < num3)
				{
					x = num3;
				}
				if (x < _minEnergy)
				{
					x = _minEnergy;
				}
				array[j >> 1] = x;
				num3 -= 8f;
			}
			for (int k = 0; k < _bands.Length; k++)
			{
				float num4 = 0f;
				for (int l = 0; l < _bands[k].Window.Length; l++)
				{
					num4 += array[l + _bands[k].Begin] * _bands[k].Window[l];
				}
				num4 *= _bands[k].Total;
				int stretch = Math.Max(2, _stretch / 2);
				Delta delta = _filters[filterOffset + k].ConvertAmplitudeToDelta(num4, stretch);
				if (delta.Max > _psyGlobal.PreEchoThreshold[k] + num2)
				{
					num |= 1;
					num |= 4;
				}
				if (delta.Min < _psyGlobal.PostEchoThreshold[k] - num2)
				{
					num |= 2;
				}
			}
			return num;
		}
	}
}
