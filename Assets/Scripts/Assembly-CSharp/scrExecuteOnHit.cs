using System;

public class scrExecuteOnHit : RDExecuteOn
{
	public delegate void OnHitActionWithRow(int rowID);

	public delegate void OnHitActionWithBeat(Beat beat);

	public OnHitActionWithRow hitActionWithRow;

	public OnHitActionWithBeat hitActionWithBeat;

	public HitType hitType;

	public bool isActive;

	public bool persistent;

	public bool autodelete;

	public int rowID;

	public bool overwritable;

	public void Setup(HitType _hitType, bool _persistent, bool _autodelete, int _rowID, bool overwrite = true)
	{
		isActive = true;
		hitType = _hitType;
		persistent = _persistent;
		autodelete = _autodelete;
		rowID = _rowID;
		if (overwrite)
		{
			overwritable = true;
			{
				foreach (RDExecuteOn execute in base.executes)
				{
					if (execute is scrExecuteOnHit)
					{
						scrExecuteOnHit scrExecuteOnHit2 = (scrExecuteOnHit)execute;
						if (scrExecuteOnHit2 != this && scrExecuteOnHit2.hitType == hitType && scrExecuteOnHit2.rowID == rowID && scrExecuteOnHit2.overwritable)
						{
							scrExecuteOnHit2.Remove();
						}
					}
				}
				return;
			}
		}
		overwritable = false;
	}

	public scrExecuteOnHit MakeUnoverridable()
	{
		overwritable = false;
		return this;
	}

	public void Execute(Beat beat)
	{
		if (!removed && enabled)
		{
			if (hitActionWithRow != null)
			{
				hitActionWithRow(beat.rowID);
			}
			else if (hitActionWithBeat != null)
			{
				hitActionWithBeat(beat);
			}
			else
			{
				action();
			}
		}
	}

	public static scrExecuteOnHit Add(Action hitAction, HitType hitType, bool persistent, bool autoDelete = false, int rowID = -1, bool overwrite = true)
	{
		scrExecuteOnHit obj = new scrExecuteOnHit();
		obj.action = hitAction;
		obj.Setup(hitType, persistent, autoDelete, rowID, overwrite);
		return obj;
	}

	public static scrExecuteOnHit AddWithRow(OnHitActionWithRow hitAction, HitType hitType, bool persistent, bool autoDelete = false, int rowID = -1, bool overwrite = true)
	{
		scrExecuteOnHit obj = new scrExecuteOnHit();
		obj.hitActionWithRow = hitAction;
		obj.Setup(hitType, persistent, autoDelete, rowID, overwrite);
		obj.hitActionWithRow = hitAction;
		return obj;
	}

	public static scrExecuteOnHit AddWithBeat(OnHitActionWithBeat hitAction, HitType hitType, bool persistent, bool autoDelete = false, int rowID = -1, bool overwrite = true)
	{
		scrExecuteOnHit obj = new scrExecuteOnHit();
		obj.hitActionWithBeat = hitAction;
		obj.Setup(hitType, persistent, autoDelete, rowID, overwrite);
		return obj;
	}

	public static void Clear(bool clearUnoverwritables = true)
	{
		scrConductor.instance.executes.RemoveAll((RDExecuteOn e) => e is scrExecuteOnHit && (clearUnoverwritables || (e as scrExecuteOnHit).overwritable));
	}
}
