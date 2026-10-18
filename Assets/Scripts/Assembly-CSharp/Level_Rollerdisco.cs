using System.Collections;
using RDLevelEditor;
using UnityEngine;

public class Level_Rollerdisco : LevelBase
{
	private RDTrain train;

	private RDTrain train2;

	public Level_Rollerdisco(RDLevelData data)
		: base(data)
	{
	}

	public override IEnumerator LoadBigAssets()
	{
		train = LoadLevelAsset<RDTrain>();
		train.transform.LocalMoveX(3000f);
		train.transform.LocalMoveY(130f);
		base.room1.SetRowContainerY(-30f);
		train.SetNightMode(night: true);
		train2 = LoadLevelAsset<RDTrain>();
		train2.transform.LocalMoveX(0f);
		train2.transform.LocalMoveY(130f);
		base.room0.SetRowContainerY(-30f);
		train2.tracks.gameObject.SetActive(value: false);
		foreach (Transform wheel in train2.wheels)
		{
			wheel.gameObject.SetActive(value: false);
		}
		train2.SetNightMode(night: true);
		foreach (Transform item in train2.nightContainer.transform)
		{
			if (item.GetComponent<tk2dSprite>() == null || item.GetComponent<tk2dSprite>() != train2.nightSky)
			{
				item.gameObject.SetActive(value: false);
			}
		}
		yield return null;
	}

	public void FlipTrain(bool room1, bool flipped)
	{
		if (room1)
		{
			train.xReversed = flipped;
		}
		else
		{
			train2.xReversed = flipped;
		}
	}
}
