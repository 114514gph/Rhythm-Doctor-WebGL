public class Level_djtest : LevelBase
{
	public override void preactions()
	{
		int num = base.conductor.barNumber;
		if (num != 1)
		{
			_ = 2;
			return;
		}
		base.conductor.SetBPM(65f);
		base.conductor.PlaySong("sndMurmurs", LevelBase.musicGroup);
		Background.Create("MenuBackground", 1f, 1f);
		base.game.MakeRow(RowType.Hold, 1, 1f, Character.Politician);
		base.game.MakeRow(RowType.Hold, 2, 1f, Character.Farmer, RDPlayer.P1, RDPlayer.P2);
		base.game.OnBeatHold(1, 1, 0f, 1f);
		base.game.OnBeatHold(2, 2, 0f, 2f);
	}

	public override void actions()
	{
	}
}
