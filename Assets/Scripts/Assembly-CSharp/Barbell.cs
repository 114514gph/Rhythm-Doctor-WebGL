using System;
using UnityEngine;

namespace RhythmWeightlifter
{
	[Serializable]
	public class Barbell
	{
		private const float unbalancedWarningThreshold = 0.5f;

		private const float unbalancedFailThreshold = 1.25f;

		public Transform maxLimit;

		public Transform minLimit;

		public LineRenderer weightRod;

		public Weight p1Weight;

		public Weight p2Weight;

		public bool p1AltSound;

		public bool p2AltSound;

		public float[] collarOuterXPos;

		public float progress;

		public float progressPrevious;

		private static scnRhythmWeightlifter gameInstance => scnRhythmWeightlifter.gameInstance;

		public float p1Y
		{
			get
			{
				return p1Weight.y;
			}
			set
			{
				p1Weight.y = value;
			}
		}

		public float p2Y
		{
			get
			{
				return p2Weight.y;
			}
			set
			{
				p2Weight.y = value;
			}
		}

		public int additiveWeight => p1Weight.CurrentWeight + p2Weight.CurrentWeight;

		public bool completed
		{
			get
			{
				if (p1Y >= maxY)
				{
					return p2Y >= maxY;
				}
				return false;
			}
		}

		public float maxY => maxLimit.localPosition.y;

		public float minY => minLimit.localPosition.y;

		public float yDistance => maxY - minY;

		public void Setup()
		{
			p1Weight.Setup(this, p1: true);
			p2Weight.Setup(this, p1: false);
		}

		public void Reset()
		{
			p1Weight.Reset(minY);
			p2Weight.Reset(minY);
			p1AltSound = false;
			p2AltSound = false;
			progress = 0f;
		}

		public void Update(bool levelWin)
		{
			p1Weight.Update(levelWin);
			p2Weight.Update(levelWin);
			float num = Mathf.Min(p1Weight.CurrentWeight, p2Weight.CurrentWeight);
			float num2 = Mathf.Max(1f, num / 2f);
			if (p1Weight.Inconsistency || p2Weight.Inconsistency)
			{
				num2 = 1f;
			}
			float num3 = 50f / 255f * yDistance;
			float num4 = Mathf.Abs(p1Weight.y - p2Weight.y);
			gameInstance.unbalancedWarning = num4 > 0.5f * num3 / num2;
			if (gameInstance.failure == Failure.None && num4 > 1.25f * num3 / num2)
			{
				gameInstance.failure = ((p1Weight.Inconsistency || p2Weight.Inconsistency) ? Failure.Consistency : Failure.Speed);
				progress = 0f;
				string speech = RDString.Get("narration.rhythmWeightlifter.topple" + ((p1Weight.y > p2Weight.y) ? "Left" : "Right")) + Narration.ShortPause + RDString.Get("rhythmWeightlifter." + ((gameInstance.failure == Failure.Consistency) ? "tooInconsistent" : "tooSlow"));
				Narration.Stop();
				Narration.Say(speech, NarrationCategory.Notification, false);
			}
			float num5 = Mathf.InverseLerp(minY, maxY, Mathf.Min(p1Y, p2Y));
			if (num5 <= 0.25f)
			{
				progress = 0f;
			}
			else
			{
				progress = Mathf.Max(progress, num5);
			}
			if ((double)progressPrevious < 0.5 && progress >= 0.5f)
			{
				Narration.Say(RDString.Get("narration.rhythmWeightlifter.halfway"), NarrationCategory.Notification, false);
			}
			progressPrevious = progress;
		}

		public void Lift(bool p1, float levelWeight)
		{
			Weight obj = (p1 ? p1Weight : p2Weight);
			RWSound rWSound;
			if (p1)
			{
				rWSound = (p1AltSound ? gameInstance.flexHighAlt : gameInstance.flexHigh);
				if (gameInstance.DogMode)
				{
					rWSound = (p1AltSound ? gameInstance.dogHighAlt : gameInstance.dogHigh);
				}
				p1AltSound = !p1AltSound;
			}
			else
			{
				rWSound = (p2AltSound ? gameInstance.flexLowAlt : gameInstance.flexLow);
				if (gameInstance.DogMode)
				{
					rWSound = (p2AltSound ? gameInstance.dogLowAlt : gameInstance.dogLow);
				}
				p2AltSound = !p2AltSound;
			}
			rWSound.Play();
			obj.Lift();
		}
	}
}
