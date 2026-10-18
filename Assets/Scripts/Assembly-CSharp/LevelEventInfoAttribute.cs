using System;
using RDLevelEditor;

[AttributeUsage(AttributeTargets.Class)]
public class LevelEventInfoAttribute : Attribute
{
	public readonly LevelEventExecutionTime executionTime;

	public readonly int sortOffset;

	public readonly bool usesBar;

	public readonly bool usesBeat;

	public readonly bool usesType;

	public readonly int defaultRow;

	public readonly bool usesTargetId;

	public readonly RoomsUsage roomsUsage;

	public readonly bool constantConditionalsOnly;

	public LevelEventInfoAttribute(LevelEventExecutionTime executionTime = LevelEventExecutionTime.OnBar, int sortOffset = 0, RoomsUsage roomsUsage = RoomsUsage.NotUsed, bool usesBar = true, bool usesBeat = true, bool usesType = true, bool usesTargetId = false, int defaultRow = -10, bool constantConditionalsOnly = false)
	{
		this.executionTime = executionTime;
		this.sortOffset = sortOffset;
		this.roomsUsage = roomsUsage;
		this.usesBar = usesBar;
		this.usesBeat = usesBeat;
		this.usesType = usesType;
		this.usesTargetId = usesTargetId;
		this.defaultRow = defaultRow;
		this.constantConditionalsOnly = constantConditionalsOnly;
	}
}
