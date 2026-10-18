using System.Collections;
using RDLevelEditor;

public class Level_Classy : LevelBase
{
	public Level_Classy(RDLevelData data)
		: base(data)
	{
	}

	public override IEnumerator LoadBigAssets()
	{
		keepHandsInFront = true;
		yield return null;
	}
}
