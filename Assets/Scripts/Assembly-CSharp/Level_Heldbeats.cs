using RDLevelEditor;

public class Level_Heldbeats : LevelBase
{
	public Level_Heldbeats(RDLevelData data)
		: base(data)
	{
	}

	public override void Init()
	{
	}

	public override void preactions()
	{
	}

	public override void actions()
	{
	}

	public void MoveSnake()
	{
		base.room0.train.GetComponent<RDTrain>().dayContainer.transform.Find("Train_Rattlesnake").transform.LocalMoveX(376f);
	}
}
