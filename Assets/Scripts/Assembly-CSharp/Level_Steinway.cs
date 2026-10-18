using RDLevelEditor;

public class Level_Steinway : LevelBase
{
	public Level_Steinway(RDLevelData data)
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
		if (base.conductor.barNumber == 2)
		{
			if (GC.twoPlayerMode)
			{
				bool flag = base.game.GetP1HandUpdateType() == HandUpdateType.LeftHand;
				SetHandToIan(0, flag);
				SetHandsCPUCanUse(0, flag, !flag);
				SetHandsPlayersCanUse(0, !flag, flag);
				SetCPUHand(0, flag);
				bool flag2 = base.game.GetP2HandUpdateType() == HandUpdateType.LeftHand;
				SetHandToIan(1, flag2);
				SetHandsCPUCanUse(1, flag2, !flag2);
				SetHandsPlayersCanUse(1, !flag2, flag2);
				SetCPUHand(1, flag2);
			}
			else
			{
				SetHandToIan(0, rightHand: false);
				SetHandsCPUCanUse(0, rightHand: false, leftHand: true);
				SetHandsPlayersCanUse(0, rightHand: true, leftHand: false);
				SetCPUHand(0, rightHand: false);
				SetHandToIan(1, rightHand: false);
				SetHandsCPUCanUse(1, rightHand: false, leftHand: true);
				SetHandsPlayersCanUse(1, rightHand: true, leftHand: false);
				SetCPUHand(1, rightHand: false);
			}
		}
	}

	public void ToggleIanInRoom(int room, bool enable, bool replace)
	{
		bool flag = room == 0;
		bool flag2 = false;
		HandUpdateType handType = HandUpdateType.BothHands;
		if (replace)
		{
			flag2 = ((!flag) ? (base.game.GetP2HandUpdateType() == HandUpdateType.LeftHand) : (base.game.GetP1HandUpdateType() == HandUpdateType.LeftHand));
			handType = ((!enable) ? ((!flag2) ? HandUpdateType.RightHand : HandUpdateType.LeftHand) : (flag2 ? HandUpdateType.RightHand : HandUpdateType.LeftHand));
		}
		base.game.GetHandControllerFromInt(room).UpdateHandsPositions(handType);
	}

	public void ToggleBirdsVolume(bool on, bool solo)
	{
		float num = (on ? 0.9f : 0f);
		for (int i = 0; i < 4; i++)
		{
			base.rows[i].rowSoundVolMultiplier = ((solo && i != 0) ? 0f : num);
			base.rows[i].rowSoundPan = (GC.twoPlayerMode ? GC.PanP1 : (0.25f * (float)i));
		}
	}

	public void ToggleMrsStevensonVolume(bool on)
	{
		float rowSoundVolMultiplier = (on ? 1f : 0f);
		float rowSoundPan = (GC.twoPlayerMode ? GC.PanP2 : (-0.5f));
		base.rows[4].rowSoundVolMultiplier = rowSoundVolMultiplier;
		base.rows[4].rowSoundPan = rowSoundPan;
	}
}
