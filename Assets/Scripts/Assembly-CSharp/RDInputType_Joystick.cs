#if !UNITY_WEBGL
using Rewired;
using UnityEngine;

public class RDInputType_Joystick : RDInputType
{
	private Player player;

	protected override Vector2 Axis => new Vector2(LeftAxis + RightAxis, DownAxis + UpAxis);

	public float LeftAxis => 0f - player.GetAxis(58);

	public float RightAxis => player.GetAxis(59);

	public float UpAxis => player.GetAxis(56);

	public float DownAxis => 0f - player.GetAxis(57);

	public RDInputType_Joystick(int schemeIndex)
		: base(schemeIndex)
	{
		player = ReInput.players.GetPlayer(schemeIndex);
	}

	public override bool Main(RDButtonState state)
	{
		return GetActionState(state, base.gameplay ? 54 : 53);
	}

	public override bool Cancel(RDButtonState state)
	{
		return GetActionState(state, base.gameplayCancel ? 60 : 52);
	}

	public override bool Skip()
	{
		return false;
	}

	public override bool Restart()
	{
		return false;
	}

	public override bool Quit()
	{
		return false;
	}

	public override bool Left(RDButtonState state)
	{
		return GetActionState(state, 58);
	}

	public override bool Right(RDButtonState state)
	{
		return GetActionState(state, 59);
	}

	public override bool Up(RDButtonState state)
	{
		return GetActionState(state, 56);
	}

	public override bool Down(RDButtonState state)
	{
		return GetActionState(state, 57);
	}

	public override bool FinerControl(RDButtonState state)
	{
		return GetActionState(state, 55);
	}

	public override bool FaceLeft(RDButtonState state)
	{
		return GetActionState(state, 61);
	}

	public override bool FaceUp(RDButtonState state)
	{
		return GetActionState(state, 62);
	}

	public override bool Select(RDButtonState state)
	{
		return false;
	}

	private bool GetActionState(RDButtonState state, int actionId)
	{
		return state switch
		{
			RDButtonState.WentDown => player.GetButtonDown(actionId), 
			RDButtonState.WentUp => player.GetButtonUp(actionId), 
			RDButtonState.IsUp => !player.GetButton(actionId), 
			RDButtonState.IsDown => player.GetButton(actionId), 
			_ => false, 
		};
	}
}

#endif
