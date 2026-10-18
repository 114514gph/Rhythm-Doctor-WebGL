using System;
using System.Collections.Generic;

public abstract class RDExecuteOn : RDClass
{
	public bool enabled = true;

	public bool removed;

	public Action action;

	public string callingMethod = "";

	public List<RDExecuteOn> executes => scrConductor.instance.executes;

	public virtual void OnBeat()
	{
	}

	public virtual void Update()
	{
	}

	public virtual void LateUpdate()
	{
	}

	public RDExecuteOn()
	{
		executes.Add(this);
	}

	public void Remove()
	{
		removed = true;
	}

	public static List<RDExecuteOn> FindAll<T>()
	{
		return scrConductor.instance.executes.FindAll((RDExecuteOn e) => e is T);
	}

	public static void ClearAll<T>()
	{
		scrConductor.instance.executes.RemoveAll((RDExecuteOn e) => e is T);
	}

	public override string ToString()
	{
		return GetType().ToString() + " called by " + callingMethod;
	}
}
