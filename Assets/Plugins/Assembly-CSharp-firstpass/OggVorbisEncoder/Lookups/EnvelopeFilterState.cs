using System;

namespace OggVorbisEncoder.Lookups
{
	public class EnvelopeFilterState
	{
		private const int EnvelopePre = 16;

		private const int EnvelopeAmp = 17;

		private readonly float[] _ampBuffer = new float[17];

		private readonly float[] _nearBuffer = new float[15];

		private int _ampPointer;

		private float _nearDcAcc;

		private float _nearDcPartialAcc;

		private int _nearPointer;

		public float SpreadNearDc(float input)
		{
			if (_nearPointer == 0)
			{
				_nearDcAcc = _nearDcPartialAcc + input;
				_nearDcPartialAcc = input;
			}
			else
			{
				_nearDcAcc += input;
				_nearDcPartialAcc += input;
			}
			_nearDcAcc -= _nearBuffer[_nearPointer];
			_nearBuffer[_nearPointer] = input;
			float x = _nearDcAcc * (float)(1.0 / (double)(_nearBuffer.Length + 1));
			_nearPointer++;
			if (_nearPointer >= _nearBuffer.Length)
			{
				_nearPointer = 0;
			}
			return (float)((double)x.ToDecibel() * 0.5 - 15.0);
		}

		public Delta ConvertAmplitudeToDelta(float amplitude, int stretch)
		{
			float num = -99999f;
			float num2 = 99999f;
			int ampPointer = _ampPointer;
			if (--ampPointer < 0)
			{
				ampPointer += 17;
			}
			float num3 = Math.Max(amplitude, _ampBuffer[ampPointer]);
			float num4 = Math.Min(amplitude, _ampBuffer[ampPointer]);
			for (int i = 0; i < stretch; i++)
			{
				if (--ampPointer < 0)
				{
					ampPointer += 17;
				}
				num = Math.Max(num, _ampBuffer[ampPointer]);
				num2 = Math.Min(num2, _ampBuffer[ampPointer]);
			}
			_ampBuffer[_ampPointer] = amplitude;
			_ampPointer++;
			if (_ampPointer >= _ampBuffer.Length)
			{
				_ampPointer = 0;
			}
			return new Delta(num4 - num2, num3 - num);
		}
	}
}
