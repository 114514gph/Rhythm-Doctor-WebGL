using RDLevelEditor;

public class Level_SparkLine : LevelBase
{
	public Level_SparkLine(RDLevelData data)
		: base(data)
	{
	}

	public override void Init()
	{
		cutsceneBarsDesc = new (int, int)[1] { (0, 48) };
	}

	public void BalloonsSetStutterPoint()
	{
		base.room1.sparklineBalloons.SetStutterPoint();
	}

	public void BalloonsStutter()
	{
		base.room1.sparklineBalloons.Stutter();
	}

	public void BalloonsResume()
	{
		base.room1.sparklineBalloons.Resume();
	}
}
