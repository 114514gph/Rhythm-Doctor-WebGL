using UnityEngine;

namespace RDLevelEditor
{
	public struct BarAndBeat
	{
		private int _bar;

		private float _beat;

		public int bar
		{
			get
			{
				if (_bar < 1)
				{
					Debug.LogWarning($"Beat number is: {_bar}, returning 1");
					return 1;
				}
				return _bar;
			}
			set
			{
				if (value < 1)
				{
					Debug.LogWarning($"Bar number is: {value}, setting value to 1");
					value = 1;
				}
				_bar = value;
			}
		}

		public float beat
		{
			get
			{
				if (_beat < 1f)
				{
					Debug.LogWarning($"Beat number is: {_beat}, returning 1");
					return 1f;
				}
				return _beat;
			}
			set
			{
				if (value < 1f)
				{
					Debug.LogWarning($"Beat number is: {value}, setting value to 1");
					value = 1f;
				}
				_beat = value;
			}
		}

		public BarAndBeat(int barNumber, float beatNumber)
		{
			if (barNumber < 1)
			{
				Debug.LogWarning("Bar number is: " + barNumber + ", setting value to 1");
				barNumber = 1;
			}
			if (beatNumber < 1f)
			{
				Debug.LogWarning("Beat number is: " + beatNumber + ", setting value to 1");
				beatNumber = 1f;
			}
			_bar = barNumber;
			_beat = beatNumber;
		}

		public override string ToString()
		{
			return $"bar: {bar}, beat: {beat}";
		}
	}
}
