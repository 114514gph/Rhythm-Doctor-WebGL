public class scrBeatLoop
{
	public RowType rowType;

	public int i;

	public int rowID;

	public float startbeat;

	public float tick;

	public int cyclecount;

	public int cyclefreq;

	public SwingBeatType beattype;

	public float customswing;

	public float holdTimeInCrotchets;

	public scnGame wrldGame => scnGame.instance;

	public scrConductor conductor => scrConductor.instance;

	public void SetupClassic(int _i, int _rowID, float _startbeat, float _tick, int _cyclefreq = 1, SwingBeatType beattype = SwingBeatType.Normal, float customswing = 0f)
	{
		cyclecount = -1;
		wrldGame.AddBeat(_rowID, _startbeat, _tick, beattype, customswing);
		i = _i;
		rowID = _rowID;
		startbeat = _startbeat;
		tick = _tick;
		cyclefreq = _cyclefreq;
		this.beattype = beattype;
		this.customswing = customswing;
		rowType = RowType.Classic;
		wrldGame.beatLoops.Add(this);
	}

	public void SetupOneshot(int _i, int _rowID, float _startbeat, float _tick, int _cyclefreq)
	{
		cyclecount = -1;
		wrldGame.AddBeatOneshot(_rowID, _startbeat, _tick);
		i = _i;
		rowID = _rowID;
		startbeat = _startbeat;
		tick = _tick;
		cyclefreq = _cyclefreq;
		rowType = RowType.Oneshot;
		wrldGame.beatLoops.Add(this);
	}

	public void SetupHold(int _i, int _rowID, float _startbeat, float _tick, int _cyclefreq = 1, float _holdTimeInCrotchets = 1f)
	{
		cyclecount = -1;
		holdTimeInCrotchets = _holdTimeInCrotchets;
		wrldGame.AddBeatHold(_rowID, _startbeat, _tick, holdTimeInCrotchets);
		i = _i;
		rowID = _rowID;
		startbeat = _startbeat;
		tick = _tick;
		cyclefreq = _cyclefreq;
		rowType = RowType.Hold;
		wrldGame.beatLoops.Add(this);
	}

	public virtual void Cycle()
	{
		if (wrldGame.rows[rowID].ent == null)
		{
			return;
		}
		float rowMisses = wrldGame.rows[rowID].ent.rowMisses;
		float missesToCrackHeart = wrldGame.currentLevel.missesToCrackHeart;
		if (GC.twoPlayerMode && wrldGame.currentLevel.shouldMakeHealthBar && rowMisses >= missesToCrackHeart)
		{
			return;
		}
		cyclecount++;
		if (wrldGame.beatAlarm[i] && cyclecount % cyclefreq == 0 && cyclecount != 0)
		{
			if (rowType == RowType.Classic)
			{
				wrldGame.AddBeat(rowID, startbeat, tick, beattype, customswing);
			}
			if (rowType == RowType.Hold)
			{
				wrldGame.AddBeatHold(rowID, startbeat, tick, holdTimeInCrotchets);
			}
			if (rowType == RowType.Oneshot)
			{
				wrldGame.AddBeatOneshot(rowID, startbeat, tick);
			}
		}
	}
}
