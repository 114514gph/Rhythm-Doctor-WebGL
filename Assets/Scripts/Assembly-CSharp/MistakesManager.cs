using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MistakesManager
{
	public const float InvalidOffsetType = -1f;

	public int earlyOffsetsSumP1;

	public int earlyOffsetsSumP2;

	public int lateOffsetsSumP1;

	public int lateOffsetsSumP2;

	public List<float> mistakeOffsetsListP1 = new List<float>();

	public List<float> mistakeOffsetsListP2 = new List<float>();

	private float mistakesCountP1;

	private float mistakesCountP2;

	public int consecutiveMistakesP1;

	public int consecutiveMistakesP2;

	public int totalOffsetsSumP1 => earlyOffsetsSumP1 + lateOffsetsSumP1;

	public int totalOffsetsSumP2 => earlyOffsetsSumP2 + lateOffsetsSumP2;

	public int earlyOffsetsSum => earlyOffsetsSumP1 + earlyOffsetsSumP2;

	public int lateOffsetsSum => lateOffsetsSumP1 + lateOffsetsSumP2;

	public int totalOffsetsSum => earlyOffsetsSum + lateOffsetsSum;

	public float mistakes
	{
		get
		{
			return mistakesCountP1 + mistakesCountP2;
		}
		[Obsolete("Use scnGame.OnMistakeOrHeal() instead.", true)]
		set
		{
		}
	}

	public float mistakesP1
	{
		get
		{
			return mistakesCountP1;
		}
		[Obsolete("Use scnGame.OnMistakeOrHeal() instead.", true)]
		set
		{
		}
	}

	public float mistakesP2
	{
		get
		{
			return mistakesCountP2;
		}
		[Obsolete("Use scnGame.OnMistakeOrHeal() instead.", true)]
		set
		{
		}
	}

	public static OffsetType mistakeOffsetType
	{
		get
		{
			float mistakeOffset = Persistence.GetMistakeOffset();
			if (mistakeOffset == -1f)
			{
				return OffsetType.Null;
			}
			if (mistakeOffset > -0.4f)
			{
				return OffsetType.VeryEarly;
			}
			if (mistakeOffset > -0.2f)
			{
				return OffsetType.SlightlyEarly;
			}
			if (mistakeOffset > 0f)
			{
				return OffsetType.SlightlyLate;
			}
			if (mistakeOffset > 0.2f)
			{
				return OffsetType.VeryLate;
			}
			return OffsetType.Null;
		}
	}

	public void ClearMistakes(RDPlayer player = RDPlayer.BOTH)
	{
		switch (player)
		{
		case RDPlayer.BOTH:
			mistakesCountP1 = 0f;
			mistakeOffsetsListP1 = new List<float>();
			mistakesCountP2 = 0f;
			mistakeOffsetsListP2 = new List<float>();
			break;
		case RDPlayer.P1:
			mistakesCountP1 = 0f;
			mistakeOffsetsListP1 = new List<float>();
			break;
		case RDPlayer.P2:
			mistakesCountP2 = 0f;
			mistakeOffsetsListP2 = new List<float>();
			break;
		}
		consecutiveMistakesP1 = 0;
		consecutiveMistakesP2 = 0;
	}

	public void AddMistake(RDPlayer player, float offset, float weight)
	{
		if (weight < 0f)
		{
			return;
		}
		switch (player)
		{
		case RDPlayer.CPU:
			return;
		case RDPlayer.BOTH:
			if (GC.twoPlayerMode)
			{
				mistakeOffsetsListP1.Add(offset);
				mistakesCountP1 += weight / 2f;
				mistakeOffsetsListP2.Add(offset);
				mistakesCountP2 += weight / 2f;
				return;
			}
			break;
		}
		if (player == RDPlayer.P1 || (player == RDPlayer.BOTH && !GC.twoPlayerMode))
		{
			mistakeOffsetsListP1.Add(offset);
			mistakesCountP1 += weight;
		}
		else if (player == RDPlayer.P2)
		{
			mistakeOffsetsListP2.Add(offset);
			mistakesCountP2 += weight;
		}
	}

	public void RemoveMistake(RDPlayer player, float weight)
	{
		if (!(weight > 0f))
		{
			if (player == RDPlayer.BOTH && GC.twoPlayerMode)
			{
				mistakesCountP1 = Mathf.Max(mistakesCountP1 + weight / 2f, 0f);
				mistakesCountP2 = Mathf.Max(mistakesCountP2 + weight / 2f, 0f);
			}
			else if (player == RDPlayer.P1 || (player == RDPlayer.BOTH && !GC.twoPlayerMode))
			{
				mistakesCountP1 = Mathf.Max(mistakesCountP1 + weight, 0f);
			}
			else if (player == RDPlayer.P2)
			{
				mistakesCountP2 = Mathf.Max(mistakesCountP2 + weight, 0f);
			}
		}
	}

	public float GetAverage(RDPlayer player = RDPlayer.BOTH)
	{
		if (mistakeOffsetsListP1.Count > 0 || mistakeOffsetsListP2.Count > 0)
		{
			return player switch
			{
				RDPlayer.P1 => mistakeOffsetsListP1.Average(), 
				RDPlayer.P2 => mistakeOffsetsListP2.Average(), 
				RDPlayer.BOTH => mistakeOffsetsListP1.Concat(mistakeOffsetsListP2).Average(), 
				_ => 0f, 
			};
		}
		return 0f;
	}

	public void SaveMistakesData()
	{
		if (mistakes > 0f)
		{
			Persistence.SetMistakeOffset(GetAverage());
		}
		Persistence.SetMistakeCount(mistakes);
	}

	public static OffsetType GetOffsetTypeFromMistakeOffset(float mistakeOffset, RDPlayer player = RDPlayer.P1)
	{
		float hitMargin = scnGame.GetHitMargin(player);
		if (mistakeOffset >= 0.4f)
		{
			return OffsetType.Missed;
		}
		if (mistakeOffset > 0.2f)
		{
			return OffsetType.VeryLate;
		}
		if (mistakeOffset > hitMargin)
		{
			return OffsetType.SlightlyLate;
		}
		if (mistakeOffset > 0f - hitMargin)
		{
			return OffsetType.Perfect;
		}
		if (mistakeOffset > -0.2f)
		{
			return OffsetType.SlightlyEarly;
		}
		if (mistakeOffset > -0.4f)
		{
			return OffsetType.VeryEarly;
		}
		return OffsetType.Null;
	}

	public void AddAbsoluteMistake(RDPlayer player, int frameOffset)
	{
		int num = Math.Abs(frameOffset);
		switch (player)
		{
		case RDPlayer.P1:
			if (frameOffset > 0)
			{
				lateOffsetsSumP1 += num;
			}
			else
			{
				earlyOffsetsSumP1 += num;
			}
			break;
		case RDPlayer.P2:
			if (frameOffset > 0)
			{
				lateOffsetsSumP2 += num;
			}
			else
			{
				earlyOffsetsSumP2 += num;
			}
			break;
		}
	}

	public void UpdateConsecutiveMistakes(RDPlayer player, OffsetType ot)
	{
		if (player != RDPlayer.P1 && player != RDPlayer.P2)
		{
			return;
		}
		bool flag = ot != OffsetType.Perfect;
		if (player == RDPlayer.P1)
		{
			if (flag)
			{
				consecutiveMistakesP1++;
			}
			else
			{
				consecutiveMistakesP1 -= 2;
			}
			consecutiveMistakesP1 = Math.Clamp(consecutiveMistakesP1, 0, 8);
		}
		else
		{
			if (flag)
			{
				consecutiveMistakesP2++;
			}
			else
			{
				consecutiveMistakesP2 -= 2;
			}
			consecutiveMistakesP2 = Math.Clamp(consecutiveMistakesP2, 0, 8);
		}
	}

	public MistakesManager Clone()
	{
		return new MistakesManager
		{
			earlyOffsetsSumP1 = earlyOffsetsSumP1,
			earlyOffsetsSumP2 = earlyOffsetsSumP2,
			lateOffsetsSumP1 = lateOffsetsSumP1,
			lateOffsetsSumP2 = lateOffsetsSumP2,
			mistakeOffsetsListP1 = new List<float>(mistakeOffsetsListP1),
			mistakeOffsetsListP2 = new List<float>(mistakeOffsetsListP2),
			mistakesCountP1 = mistakesCountP1,
			mistakesCountP2 = mistakesCountP2
		};
	}
}
