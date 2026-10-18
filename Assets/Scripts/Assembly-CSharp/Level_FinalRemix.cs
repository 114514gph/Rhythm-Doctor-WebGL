public class Level_FinalRemix : LevelBase
{
	private Background[] arrMatrix;

	public override void Init()
	{
		songsUsed = new string[1] { "sndLeanOn" };
		heartExplodeType = HeartExplodeType.Instant;
		multiroom = true;
	}

	public override void preactions()
	{
		if (base.conductor.barNumber == 1)
		{
			base.conductor.SetBPM(200f);
			PlayMusic(songsUsed[0]);
			base.game.MakeRow(RowType.Classic, 1, 1f, Character.Samurai, RDPlayer.P1, RDPlayer.BOTH, 2, isShadow: false, 2);
			base.game.MakeRow(RowType.Classic, 2, 1f, Character.Boy, RDPlayer.P1, RDPlayer.BOTH, 2, isShadow: false, 1);
			base.game.MakeRow(RowType.Classic, 3, 1f, Character.Girl, RDPlayer.P1, RDPlayer.BOTH, 2);
			base.game.MakeRow(RowType.Classic, 4, 1f, Character.Samurai, RDPlayer.P1, RDPlayer.BOTH, 2, isShadow: false, 3);
			base.game.MakeRow(RowType.Classic, 5, 1f, Character.SamuraiBoss, RDPlayer.P1, RDPlayer.BOTH, 2, isShadow: false, 4);
			Background.Create("IntimateRain", 100000f, 100000f, isForeground: false, 1).SetAlpha(0.2f);
			Background.Create("Sakura/5", 0f, 0f, isForeground: false, 3);
			Background.Create("Sakura/5");
			arrMatrix = new Background[3];
			arrMatrix[0] = Background.Create("Matrix/0", 0f, 1f, isForeground: false, 2);
			arrMatrix[1] = Background.Create("Matrix/1", 0f, 2f, isForeground: false, 2);
			arrMatrix[2] = Background.Create("Matrix/2", 0f, 3f, isForeground: false, 2);
			RDClass.Vfx.BgColor(RDPresetColors.Blue, 1);
			RDClass.Vfx.BgColor(RDPresetColors.Chocolate, 2);
			RDClass.Vfx.BgColor(RDPresetColors.CornflowerBlue, 3);
			RDClass.Vfx.BgColor(RDPresetColors.Fuchsia, 4);
			RDClass.Vfx.BgColor(RDPresetColors.FireBrick, 5);
			RDClass.Vfx.BgColor(RDPresetColors.Indigo, 6);
			RDClass.Vfx.BgColor(RDPresetColors.Salmon, 7);
		}
	}

	public override void actions()
	{
		switch (base.conductor.barNumber)
		{
		case 1:
			RDClass.Vfx.ShowRooms("1,2,3,4");
			break;
		case 32:
			base.rankscreen.AdvanceGameover();
			break;
		case 33:
			base.rankscreen.AdvanceGameover();
			break;
		}
	}
}
