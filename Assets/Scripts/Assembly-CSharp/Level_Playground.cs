using System.Collections;

public class Level_Playground : LevelBase
{
	private Background[] arrMatrix;

	public override void Init()
	{
		songsUsed = new string[1] { "sndOrientalTechno" };
		heartExplodeType = HeartExplodeType.Instant;
		base.isOneRowButNotBoss = true;
	}

	public override IEnumerator LoadBigAssets()
	{
		yield return null;
	}

	public override void preactions()
	{
		RDClass.Vfx.debugText = "Bar: " + base.conductor.barNumber;
		if (base.conductor.barNumber == 1)
		{
			base.conductor.SetBPM(200f);
			PlayMusic(songsUsed[0]);
			arrMatrix = new Background[5];
			arrMatrix[0] = Background.Create("Matrix/0", 0f, 1f).SetBackgroundOrder(5).SetScale(0.75f);
			arrMatrix[1] = Background.Create("Matrix/1", 0f, 2f).SetBackgroundOrder(6).SetScale(1.5f);
			arrMatrix[2] = Background.Create("Matrix/2", 0f, 3f).SetBackgroundOrder(7).SetScale(2.25f);
		}
	}

	public override void actions()
	{
		int num = base.conductor.barNumber;
		if (num != 1)
		{
			_ = 2;
		}
	}
}
