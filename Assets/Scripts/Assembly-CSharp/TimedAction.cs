using System;
using System.Collections.Generic;
using RDLevelEditor;

public class TimedAction
{
	public Action action;

	public List<ConditionalID> conditionals;

	public float timeOfEnd;

	public LevelEvent_Base levelEvent;

	public TimedAction(Action action, List<ConditionalID> conditionals, float timeOfEnd, LevelEvent_Base levelEvent)
	{
		this.action = action;
		this.conditionals = conditionals;
		this.timeOfEnd = timeOfEnd;
		this.levelEvent = levelEvent;
	}
}
