using RDLevelEditor;

public class Level_VividStasis : LevelBase
{
	public Level_VividStasis(RDLevelData data)
		: base(data)
	{
	}

	public override void Init()
	{
		cutsceneBarsDesc = new (int, int)[1] { (0, 24) };
	}

	public void VRankHacky()
	{
		base.room1.camera.GetComponent<CameraFilterPack_Distortion_Heat>().enabled = true;
		CameraFilterPack_Pixel_Pixelisation component = base.room1.camera.GetComponent<CameraFilterPack_Pixel_Pixelisation>();
		component.enabled = true;
		component._Pixelisation = 2f;
		component._SizeX = 1f;
		component._SizeY = 1f;
		base.game.rankscreen.rank.gameObject.SetActive(value: false);
		base.game.rankscreen.vividStasisRank.gameObject.SetActive(value: true);
		base.game.rankscreen.header.color = "09CFFF".HexToColor();
		base.game.rankscreen.description.color = "FE10B0".HexToColor();
	}
}
