using System.Collections.Generic;
using System.Linq;

public class scrBeatLoopOneshot : scrBeatLoop
{
	public class LoopOneshotChange
	{
		public float beat;

		public float tick;

		public bool kill;

		public float customBoomInterval;
	}

	public bool autoSetWeights = true;

	public float customBoomInterval;

	public bool kill;

	public bool visualSpeedChange;

	public List<LoopOneshotChange> loopChanges;

	public bool spilloverDelay;

	public float beatOfFirstBoomOfBar;

	public float boomInterval
	{
		get
		{
			if (customBoomInterval != 0f)
			{
				return customBoomInterval;
			}
			return tick * 2f;
		}
	}

	public void Setup(int _i, int _rowID, float _startbeat, float _tick, float _customBoomInterval = 0f, bool spilloverDelay = false)
	{
		i = _i;
		rowID = _rowID;
		startbeat = _startbeat;
		tick = _tick;
		customBoomInterval = _customBoomInterval;
		loopChanges = new List<LoopOneshotChange>();
		kill = true;
		visualSpeedChange = true;
		this.spilloverDelay = spilloverDelay;
		AddLoopChange(_startbeat, _tick, _customBoomInterval);
		base.wrldGame.beatLoops.Add(this);
	}

	public void AddLoopChange(float beat, float tick, float customBoomInterval = 0f, bool kill = false)
	{
		LoopOneshotChange loopOneshotChange = new LoopOneshotChange();
		loopOneshotChange.beat = beat;
		loopOneshotChange.tick = tick;
		loopOneshotChange.customBoomInterval = customBoomInterval;
		loopOneshotChange.kill = kill;
		loopChanges.Add(loopOneshotChange);
	}

	public override void Cycle()
	{
		if (!base.wrldGame.beatAlarm[i])
		{
			return;
		}
		float num = beatOfFirstBoomOfBar;
		float num2 = beatOfFirstBoomOfBar;
		if (!spilloverDelay)
		{
			num = (num2 = 0f);
		}
		while (true)
		{
			if (loopChanges.Count == 0)
			{
				if (kill)
				{
					break;
				}
				while (num < (float)base.conductor.crotchetsPerBar)
				{
					BeatOneshot beatOneshot = base.wrldGame.AddBeatOneshot(rowID, num, tick);
					if (autoSetWeights)
					{
						beatOneshot.SetWeight(tick);
					}
					num2 = num;
					num += boomInterval;
					if (base.wrldGame.currentLevel.pauseOn7thBeat && num >= 6f && num2 < 6f)
					{
						num++;
					}
				}
				break;
			}
			LoopOneshotChange loopOneshotChange = loopChanges.Aggregate((LoopOneshotChange i1, LoopOneshotChange i2) => (!(i1.beat < i2.beat)) ? i2 : i1);
			if (loopOneshotChange.beat >= (float)base.conductor.crotchetsPerBar)
			{
				foreach (LoopOneshotChange loopChange in loopChanges)
				{
					loopChange.beat -= base.conductor.crotchetsPerBar;
				}
				if (kill)
				{
					break;
				}
				while (num < (float)base.conductor.crotchetsPerBar)
				{
					base.wrldGame.AddBeatOneshot(rowID, num, tick);
					num2 = num;
					num += boomInterval;
					if (base.wrldGame.currentLevel.pauseOn7thBeat && num >= 6f && num2 < 6f)
					{
						num++;
					}
				}
				break;
			}
			while (num < loopOneshotChange.beat && num < (float)base.conductor.crotchetsPerBar)
			{
				if (kill)
				{
					num = loopOneshotChange.beat;
					continue;
				}
				base.wrldGame.AddBeatOneshot(rowID, num, tick);
				num2 = num;
				num += boomInterval;
				if (base.wrldGame.currentLevel.pauseOn7thBeat && num >= 6f && num2 < 6f)
				{
					num++;
				}
			}
			tick = loopOneshotChange.tick;
			customBoomInterval = loopOneshotChange.customBoomInterval;
			kill = loopOneshotChange.kill;
			num = loopOneshotChange.beat;
			loopChanges.Remove(loopOneshotChange);
			if (visualSpeedChange)
			{
				base.wrldGame.SetVisualSpeed(TickToVisualSpeed(loopOneshotChange.tick), loopOneshotChange.beat);
			}
		}
		beatOfFirstBoomOfBar = num - (float)base.conductor.crotchetsPerBar;
	}

	private float TickToVisualSpeed(float tick)
	{
		return 1f / tick;
	}
}
