using System.Collections;
using RDLevelEditor;

public class Level_Lofi : LevelBase
{
	private RDHoodieBoyWard hoodieBoyWard;

	private readonly string[] lightPatterns = new string[30]
	{
		"1010101010111010", "1010101010111010", "1010101010111010", "1010111010111010", "1010101010111010", "1010111010111010", "1010101010111010", "1010111010111010", "1010101010111010", "1010111010110000",
		"1010101010111010", "1010111010111010", "1010101010111010", "1010111010111010", "1010101010111010", "1010111010111010", "1010101010111010", "1010111010111010", "0000000000000000", "0000000000000000",
		"0000000000000000", "0000000000001101", "1010101010111010", "1010111010111010", "1010101010111010", "1010111010111010", "1010101010111010", "1010111010111010", "1010101010111010", "1010111010111010"
	};

	public Level_Lofi(RDLevelData data)
		: base(data)
	{
	}

	public override IEnumerator LoadBigAssets()
	{
		hoodieBoyWard = LoadLevelAsset("RDHoodieBoyWard/RDHoodieBoyWard").GetComponent<RDHoodieBoyWard>();
		hoodieBoyWard.room = base.room2;
		hoodieBoyWard.transform.LocalMoveX(6000f);
		base.room2.SetRowContainerY(-30f);
		SetHandsPlayersCanUse(1, rightHand: false, leftHand: false);
		SetHandsCPUCanUse(2, rightHand: false, leftHand: false);
		SetCPUHand(1, rightHand: true);
		yield return null;
	}

	public override void preactions()
	{
	}

	public override void actions()
	{
		int num = base.conductor.barNumber;
		if (lightPatterns.Length <= num)
		{
			return;
		}
		string text = lightPatterns[num - 1];
		for (int i = 0; i < text.Length; i++)
		{
			bool lightShouldBeOn = text[i] == '1';
			int x = i;
			scrExecuteOnCertainBeat.Add(() =>
			{
				if (lightShouldBeOn)
				{
					TurnOnLight(x);
				}
				else
				{
					TurnOffLights();
				}
			}, (i % 2 == 0) ? ((float)i / 4f) : ((float)i / 4f + 0.25f - 0.15f));
		}
	}

	private void TurnOnLight(int index)
	{
		hoodieBoyWard.TurnOnLight(index);
	}

	private void TurnOffLights()
	{
		hoodieBoyWard.ToggleAllLights(on: false);
	}

	public void EnterIanHand(float timeMult)
	{
		SetHandToIan(1, rightHand: true);
		base.game.handController1.UpdateHandsPositions(HandUpdateType.RightHand, timeMult);
		base.game.handController1.transform.localPosition = base.game.handController1.transform.localPosition.WithY(9f);
	}
}
