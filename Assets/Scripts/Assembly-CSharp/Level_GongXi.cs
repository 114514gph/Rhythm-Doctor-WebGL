using System.Collections;
using RDLevelEditor;
using UnityEngine;

public class Level_GongXi : LevelBase
{
	public Level_GongXi(RDLevelData data)
		: base(data)
	{
	}

	public override void Init()
	{
		songsUsed = new string[1] { "gongxirag2" };
	}

	public override IEnumerator LoadBigAssets()
	{
		foreach (LevelEvent_Base levelEvent in levelEvents)
		{
			if (levelEvent is LevelEvent_MakeRow)
			{
				levelEvent.executionTime = LevelEventExecutionTime.OnBar;
				levelEvent.bar = 1;
			}
		}
		yield return null;
	}

	public override void preactions()
	{
		switch (base.conductor.barNumber)
		{
		case 1:
			RDClass.Vfx.GetRoomOverlay(0).SetColor(Color.black);
			break;
		case 2:
			RDClass.Vfx.GetRoomOverlay(0).SetColor(Color.clear);
			break;
		}
	}

	public override void actions()
	{
		switch (base.conductor.barNumber)
		{
		default:
			_ = 44;
			break;
		case 1:
			base.room0.showTextAboveBeatboxes = true;
			base.room1.showTextAboveBeatboxes = true;
			break;
		case 23:
			break;
		}
	}
}
