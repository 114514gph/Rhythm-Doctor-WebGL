using System;
using System.Collections.Generic;
using OggVorbisEncoder.Lookups;
using OggVorbisEncoder.Setup;

namespace OggVorbisEncoder
{
	public class ProcessingState
	{
		private const float DecibelOffset = 0.345f;

		private readonly LookupCollection _lookups;

		private readonly float[][] _pcm;

		private readonly VorbisInfo _vorbisInfo;

		private readonly int[] _window;

		private int _centerWindow;

		private int _currentWindow;

		private int _eofFlag;

		private int _granulePosition;

		private int _lastWindow;

		private int _nextWindow;

		private int _pcmCurrent;

		private bool _preExtrapolated;

		private int _sequence = 3;

		private ProcessingState(VorbisInfo vorbisInfo, LookupCollection lookups, float[][] pcm, int[] window, int centerWindow)
		{
			_vorbisInfo = vorbisInfo;
			_lookups = lookups;
			_pcm = pcm;
			_window = window;
			_centerWindow = centerWindow;
		}

		public void WriteData(float[][] data, int length, int read_offset = 0)
		{
			if (length > 0)
			{
				EnsureBufferSize(length);
				for (int i = 0; i < data.Length; i++)
				{
					Array.Copy(data[i], read_offset, _pcm[i], _pcmCurrent, length);
				}
				CodecSetup codecSetup = _vorbisInfo.CodecSetup;
				_pcmCurrent += length;
				if (!_preExtrapolated && _pcmCurrent - _centerWindow > codecSetup.BlockSizes[1])
				{
					PreExtrapolate();
				}
			}
		}

		public void WriteEndOfStream()
		{
			CodecSetup codecSetup = _vorbisInfo.CodecSetup;
			float[] array = new float[32];
			if (!_preExtrapolated)
			{
				PreExtrapolate();
			}
			EnsureBufferSize(codecSetup.BlockSizes[1] * 3);
			_eofFlag = _pcmCurrent;
			_pcmCurrent += codecSetup.BlockSizes[1] * 3;
			for (int i = 0; i < _pcm.Length; i++)
			{
				if (_eofFlag > 64)
				{
					int num = _eofFlag;
					if (num > codecSetup.BlockSizes[1])
					{
						num = codecSetup.BlockSizes[1];
					}
					PopulateLpcFromPcm(array, _pcm[i], _eofFlag - num, num, 32);
					UpdatePcmFromLpcPredict(array, _pcm[i], _eofFlag, 32, _pcmCurrent - _eofFlag);
				}
				else
				{
					Array.Clear(_pcm[i], _eofFlag, _pcmCurrent - _eofFlag);
				}
			}
		}

		private static void UpdatePcmFromLpcPredict(float[] lpcCoeff, IList<float> data, int offset, int m, int n)
		{
			float[] array = new float[m + n];
			for (int i = 0; i < m; i++)
			{
				array[i] = data[offset - m + i];
			}
			for (int j = 0; j < n; j++)
			{
				int num = j;
				int num2 = m;
				float num3 = 0f;
				for (int k = 0; k < m; k++)
				{
					num3 -= array[num++] * lpcCoeff[--num2];
				}
				data[offset + j] = (array[num] = num3);
			}
		}

		private static void PopulateLpcFromPcm(IList<float> lpci, IList<float> data, int offset, int n, int m)
		{
			double[] array = new double[m + 1];
			double[] array2 = new double[m];
			int num = m + 1;
			while (num-- > 0)
			{
				double num2 = 0.0;
				for (int i = num; i < n; i++)
				{
					num2 += (double)data[offset + i] * (double)data[offset + i - num];
				}
				array[num] = num2;
			}
			double num3 = array[0] * 1.0000000001;
			double num4 = 1E-09 * array[0] + 1E-10;
			for (int j = 0; j < m; j++)
			{
				double num5 = 0.0 - array[j + 1];
				if (num3 < num4)
				{
					Array.Clear(array2, j, m - j);
					break;
				}
				for (num = 0; num < j; num++)
				{
					num5 -= array2[num] * array[j - num];
				}
				num5 = (array2[j] = num5 / num3);
				for (num = 0; num < j / 2; num++)
				{
					double num6 = array2[num];
					array2[num] += num5 * array2[j - 1 - num];
					array2[j - 1 - num] += num5 * num6;
				}
				if ((j & 1) != 0)
				{
					array2[num] += array2[num] * num5;
				}
				num3 *= 1.0 - num5 * num5;
			}
			double num7 = 0.99;
			double num8 = num7;
			for (num = 0; num < m; num++)
			{
				array2[num] *= num8;
				num8 *= num7;
			}
			for (num = 0; num < m; num++)
			{
				lpci[num] = (float)array2[num];
			}
		}

		private void PreExtrapolate()
		{
			_preExtrapolated = true;
			if (_pcmCurrent - _centerWindow <= 32)
			{
				return;
			}
			float[] array = new float[16];
			float[] array2 = new float[_pcmCurrent];
			for (int i = 0; i < _pcm.Length; i++)
			{
				for (int j = 0; j < _pcmCurrent; j++)
				{
					array2[j] = _pcm[i][_pcmCurrent - j - 1];
				}
				PopulateLpcFromPcm(array, array2, 0, _pcmCurrent - _centerWindow, 16);
				UpdatePcmFromLpcPredict(array, array2, _pcmCurrent - _centerWindow, 16, _centerWindow);
				for (int k = 0; k < _pcmCurrent; k++)
				{
					_pcm[i][_pcmCurrent - k - 1] = array2[k];
				}
			}
		}

		public void EnsureBufferSize(int needed)
		{
			int num = _pcm[0].Length;
			if (_pcmCurrent + needed >= num)
			{
				num = _pcmCurrent + needed * 2;
				for (int i = 0; i < _pcm.Length; i++)
				{
					float[] array = _pcm[i];
					Array.Resize(ref array, num);
					_pcm[i] = array;
				}
			}
		}

		public bool PacketOut(out OggPacket packet)
		{
			packet = null;
			if (!_preExtrapolated)
			{
				return false;
			}
			if (_eofFlag == -1)
			{
				return false;
			}
			CodecSetup codecSetup = _vorbisInfo.CodecSetup;
			int testWindow = _centerWindow + codecSetup.BlockSizes[_currentWindow] / 4 + codecSetup.BlockSizes[1] / 2 + codecSetup.BlockSizes[0] / 4;
			int num = _lookups.EnvelopeLookup.Search(_pcm, _pcmCurrent, _centerWindow, testWindow);
			if (num == -1)
			{
				if (_eofFlag == 0)
				{
					return false;
				}
				_nextWindow = 0;
			}
			else
			{
				_nextWindow = ((codecSetup.BlockSizes[0] != codecSetup.BlockSizes[1]) ? num : 0);
			}
			int num2 = _centerWindow + codecSetup.BlockSizes[_currentWindow] / 4 + codecSetup.BlockSizes[_nextWindow] / 4;
			int num3 = num2 + codecSetup.BlockSizes[_nextWindow] / 2;
			if (_pcmCurrent < num3)
			{
				return false;
			}
			int n = codecSetup.BlockSizes[_currentWindow] / 2;
			_lookups.PsyGlobalLookup.DecayAmpMax(n, _vorbisInfo.SampleRate);
			int num4 = codecSetup.BlockSizes[_currentWindow];
			float[][] array = new float[_pcm.Length][];
			int sourceIndex = _centerWindow - codecSetup.BlockSizes[_currentWindow] / 2;
			for (int i = 0; i < _pcm.Length; i++)
			{
				array[i] = new float[num4];
				Array.Copy(_pcm[i], sourceIndex, array[i], 0, array[i].Length);
			}
			bool flag = false;
			if (_eofFlag != 0 && _centerWindow >= _eofFlag)
			{
				_eofFlag = -1;
				flag = true;
			}
			byte[] packetData = PerformAnalysis(array, num4);
			packet = new OggPacket(packetData, flag, _granulePosition, _sequence++);
			if (!flag)
			{
				AdvanceStorageVectors(num2);
			}
			return true;
		}

		private void AdvanceStorageVectors(int centerNext)
		{
			int num = _vorbisInfo.CodecSetup.BlockSizes[1] / 2;
			int num2 = centerNext - num;
			if (num2 <= 0)
			{
				return;
			}
			_lookups.EnvelopeLookup.Shift(num2);
			_pcmCurrent -= num2;
			for (int i = 0; i < _pcm.Length; i++)
			{
				Array.Copy(_pcm[i], num2, _pcm[i], 0, _pcmCurrent);
			}
			_lastWindow = _currentWindow;
			_currentWindow = _nextWindow;
			_centerWindow = num;
			if (_eofFlag != 0)
			{
				_eofFlag -= num2;
				if (_eofFlag <= 0)
				{
					_eofFlag = -1;
				}
				if (_centerWindow >= _eofFlag)
				{
					_granulePosition += num2 - (_centerWindow - _eofFlag);
				}
				else
				{
					_granulePosition += num2;
				}
			}
			else
			{
				_granulePosition += num2;
			}
		}

		private bool MarkEnvelope()
		{
			EnvelopeLookup envelopeLookup = _lookups.EnvelopeLookup;
			CodecSetup codecSetup = _vorbisInfo.CodecSetup;
			int num = _centerWindow - codecSetup.BlockSizes[_currentWindow] / 4;
			int num2 = _centerWindow + codecSetup.BlockSizes[_currentWindow] / 4;
			if (_currentWindow != 0)
			{
				num -= codecSetup.BlockSizes[_lastWindow] / 4;
				num2 += codecSetup.BlockSizes[_nextWindow] / 4;
			}
			else
			{
				num -= codecSetup.BlockSizes[0] / 4;
				num2 += codecSetup.BlockSizes[0] / 4;
			}
			return envelopeLookup.Mark(num, num2);
		}

		public static ProcessingState Create(VorbisInfo info)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			CodecSetup codecSetup = info.CodecSetup;
			int num = codecSetup.BlockSizes[1];
			float[][] array = new float[info.Channels][];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new float[num];
			}
			int[] window = new int[2]
			{
				Encoding.Log(codecSetup.BlockSizes[0]) - 7,
				Encoding.Log(codecSetup.BlockSizes[1]) - 7
			};
			int centerWindow = codecSetup.BlockSizes[1] / 2;
			LookupCollection lookups = LookupCollection.Create(info);
			return new ProcessingState(info, lookups, array, window, centerWindow);
		}

		private byte[] PerformAnalysis(float[][] pcm, int pcmEnd)
		{
			int num = pcm.Length;
			float[][] gmdct = new float[num][];
			int[][] work = new int[num][];
			int[][][] floorPosts = new int[num][][];
			float[] localAmpMax = new float[num];
			int num2 = 0;
			if (_currentWindow != 0)
			{
				if (_lastWindow != 0 && _nextWindow != 0)
				{
					num2 = 1;
				}
			}
			else if (!MarkEnvelope())
			{
				num2 = 1;
			}
			Mapping mapping = _vorbisInfo.CodecSetup.MapParams[_currentWindow];
			PsyLookup psyLookup = _lookups.PsyLookup[num2 + ((_currentWindow != 0) ? 2 : 0)];
			EncodeBuffer encodeBuffer = new EncodeBuffer();
			TransformPcm(pcm, pcmEnd, work, gmdct, localAmpMax);
			ApplyMasks(pcm, pcmEnd, mapping, floorPosts, gmdct, psyLookup, localAmpMax);
			Encode(pcm, pcmEnd, encodeBuffer, mapping, work, floorPosts, psyLookup, gmdct);
			return encodeBuffer.GetBytes();
		}

		private void Encode(float[][] pcm, int pcmEnd, EncodeBuffer buffer, Mapping mapping, int[][] work, int[][][] floorPosts, PsyLookup psyLookup, float[][] gmdct)
		{
			CodecSetup codecSetup = _vorbisInfo.CodecSetup;
			int num = pcm.Length;
			bool[] array = new bool[num];
			int[][] array2 = new int[num][];
			bool[] array3 = new bool[num];
			buffer.Write(0u, 1);
			int bits = Encoding.Log(codecSetup.ModeParams.Count - 1);
			buffer.Write((uint)_currentWindow, bits);
			if (_currentWindow != 0)
			{
				buffer.Write((uint)_lastWindow, 1);
				buffer.Write((uint)_nextWindow, 1);
			}
			for (int i = 0; i < num; i++)
			{
				int num2 = mapping.ChannelMuxList[i];
				array[i] = _lookups.FloorLookup[mapping.FloorSubMap[num2]].Encode(buffer, codecSetup.BookParams, codecSetup.FullBooks, floorPosts[i][7], work[i], pcmEnd, codecSetup.BlockSizes[_currentWindow] / 2);
			}
			psyLookup.CoupleQuantizeNormalize(7, codecSetup.PsyGlobalParam, mapping, gmdct, work, array, codecSetup.PsyGlobalParam.SlidingLowPass[_currentWindow][7], num);
			for (int j = 0; j < mapping.SubMaps; j++)
			{
				int num3 = 0;
				int num4 = mapping.ResidueSubMap[j];
				for (int k = 0; k < num; k++)
				{
					if (mapping.ChannelMuxList[k] == j)
					{
						array3[num3] = array[k];
						array2[num3++] = work[k];
					}
				}
				ResidueLookup residueLookup = _lookups.ResidueLookup[num4];
				int[][] partword = residueLookup.Class(array2, array3, num3);
				num3 = 0;
				for (int l = 0; l < num; l++)
				{
					if (mapping.ChannelMuxList[l] == j)
					{
						array2[num3++] = work[l];
					}
				}
				residueLookup.Forward(buffer, pcmEnd, array2, array3, num3, partword);
			}
		}

		private void TransformPcm(float[][] inputPcm, int pcmEnd, IList<int[]> work, IList<float[]> gmdct, IList<float> localAmpMax)
		{
			for (int i = 0; i < inputPcm.Length; i++)
			{
				work[i] = new int[pcmEnd / 2];
				gmdct[i] = new float[pcmEnd / 2];
				float num = (4f / (float)pcmEnd).ToDecibel() + 0.345f;
				float[] array = inputPcm[i];
				ApplyWindow(array, _lastWindow, _currentWindow, _nextWindow);
				_lookups.TransformLookup[_currentWindow].Forward(array, gmdct[i]);
				_lookups.FftLookup[_currentWindow].Forward(array);
				array[0] = num + array[0].ToDecibel() + 0.345f;
				localAmpMax[i] = inputPcm[i][0];
				for (int j = 1; j < pcmEnd - 1; j += 2)
				{
					float x = array[j] * array[j] + array[j + 1] * array[j + 1];
					int num2 = j + 1 >> 1;
					x = (array[num2] = num + 0.5f * x.ToDecibel() + 0.345f);
					if (x > localAmpMax[i])
					{
						localAmpMax[i] = x;
					}
				}
				if (localAmpMax[i] > 0f)
				{
					localAmpMax[i] = 0f;
				}
			}
		}

		private void ApplyWindow(IList<float> pcm, int lastWindow, int window, int nextWindow)
		{
			lastWindow = ((window != 0) ? lastWindow : 0);
			nextWindow = ((window != 0) ? nextWindow : 0);
			float[] array = Block.Windows[_window[lastWindow]];
			float[] array2 = Block.Windows[_window[nextWindow]];
			int[] blockSizes = _vorbisInfo.CodecSetup.BlockSizes;
			int num = blockSizes[window];
			int num2 = blockSizes[lastWindow];
			int num3 = blockSizes[nextWindow];
			int num4 = num / 4 - num2 / 4;
			int num5 = num4 + num2 / 2;
			int num6 = num / 2 + num / 4 - num3 / 4;
			int num7 = num6 + num3 / 2;
			int i;
			for (i = 0; i < num4; i++)
			{
				pcm[i] = 0f;
			}
			int num8 = 0;
			while (i < num5)
			{
				pcm[i] *= array[num8];
				i++;
				num8++;
			}
			i = num6;
			num8 = num3 / 2 - 1;
			while (i < num7)
			{
				pcm[i] *= array2[num8];
				i++;
				num8--;
			}
			for (; i < num; i++)
			{
				pcm[i] = 0f;
			}
		}

		private void ApplyMasks(float[][] inputPcm, int pcmEnd, Mapping mapping, IList<int[][]> floorPosts, float[][] gmdct, PsyLookup psyLookup, float[] localAmpMax)
		{
			float[] array = new float[pcmEnd / 2];
			float[] array2 = new float[pcmEnd / 2];
			for (int i = 0; i < inputPcm.Length; i++)
			{
				int num = mapping.ChannelMuxList[i];
				float[] array3 = inputPcm[i];
				OffsetArray<float> offsetArray = new OffsetArray<float>(array3, pcmEnd / 2);
				floorPosts[i] = new int[15][];
				for (int j = 0; j < pcmEnd / 2; j++)
				{
					offsetArray[j] = gmdct[i][j].ToDecibel() + 0.345f;
				}
				psyLookup.NoiseMask(offsetArray, array);
				int num2 = (int)_lookups.PsyGlobalLookup.AmpMax;
				foreach (float num3 in localAmpMax)
				{
					if (num3 > (float)num2)
					{
						num2 = (int)num3;
					}
				}
				psyLookup.ToneMask(array3, array2, num2, localAmpMax[i]);
				psyLookup.OffsetAndMix(array, array2, 1, array3, gmdct[i], offsetArray);
				FloorLookup floorLookup = _lookups.FloorLookup[mapping.FloorSubMap[num]];
				floorPosts[i][7] = floorLookup.Fit(offsetArray, array3);
			}
		}
	}
}
