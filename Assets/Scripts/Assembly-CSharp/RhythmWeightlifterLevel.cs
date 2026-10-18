using System;
using UnityEngine;

namespace RhythmWeightlifter
{
	[Serializable]
	public class Level
	{
		public static int lastConsistencyP1;

		public static int lastConsistencyP2;

		public static int lastSpeed;

		public int index;

		public int player1Weight;

		public int player2Weight;

		public GameObject container;

		public LevelCharacterName leftCharacter;

		public LevelCharacterName rightCharacter;

		public float bpm = 220f;

		[NonSerialized]
		public bool unlocked;

		[NonSerialized]
		public int consistency;

		[NonSerialized]
		public int speed;

		[NonSerialized]
		public int score;

		public static int lastConsistency => lastConsistencyP1 + lastConsistencyP2;

		public static int lastScore => lastConsistency + lastSpeed;

		public Level(int player1Weight, int player2Weight, LevelCharacterName leftCharacter, LevelCharacterName rightCharacter, float bpm = 220f)
		{
			this.player1Weight = player1Weight;
			this.player2Weight = player2Weight;
			this.leftCharacter = leftCharacter;
			this.rightCharacter = rightCharacter;
			this.bpm = bpm;
		}

		public void Load()
		{
			consistency = Persistence.GetRhythmWeightlifterConsistency(index);
			speed = Persistence.GetRhythmWeightlifterSpeed(index);
			score = Persistence.GetRhythmWeightlifterScore(index);
		}

		public void Save()
		{
			consistency = Mathf.Max(lastConsistency, consistency);
			Persistence.SetRhythmWeightlifterConsistency(index, consistency);
			speed = Mathf.Max(lastSpeed, speed);
			Persistence.SetRhythmWeightlifterSpeed(index, speed);
			score = Mathf.Max(lastScore, score);
			Persistence.SetRhythmWeightlifterScore(index, score);
		}

		public static string GetRank(int score)
		{
			int num = score / 2;
			if (num < 80)
			{
				if (num < 40)
				{
					if (num < 20)
					{
						if (num == 0)
						{
							return "-";
						}
						return "F";
					}
					return "D";
				}
				if (num < 60)
				{
					return "C";
				}
				return "B";
			}
			if (num < 100)
			{
				return "A";
			}
			return "S";
		}
	}
}
