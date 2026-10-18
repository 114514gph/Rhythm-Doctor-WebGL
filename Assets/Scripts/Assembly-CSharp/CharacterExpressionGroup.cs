using System;

[Serializable]
public class CharacterExpressionGroup
{
	public string expressionLocked = "barely";

	public string expressionNotPassed = "barely";

	public string expressionPassed = "neutral";

	public string expressionPerfect = "happy";

	public CharacterExpressionGroup()
	{
	}

	public CharacterExpressionGroup(string expressionLocked, string expressionNotPassed, string expressionPassed, string expressionPerfect)
	{
		this.expressionLocked = expressionLocked;
		this.expressionNotPassed = expressionNotPassed;
		this.expressionPassed = expressionPassed;
		this.expressionPerfect = expressionPerfect;
	}

	public string GetExpression(LevelState levelState, Rank rank)
	{
		if (!rank.perfected)
		{
			return levelState switch
			{
				LevelState.NotPassed => expressionNotPassed, 
				LevelState.Passed => expressionPassed, 
				_ => expressionLocked, 
			};
		}
		return expressionPerfect;
	}
}
