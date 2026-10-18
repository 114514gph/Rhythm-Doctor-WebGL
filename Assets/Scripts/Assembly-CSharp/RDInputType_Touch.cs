using UnityEngine;

public class RDInputType_Touch : RDInputType
{
	public static int virtualButtonLeftFrame_WentDown = -1;

	public static int virtualButtonLeftFrame_WentUp = int.MaxValue;

	public static int virtualButtonUpFrame_WentDown = -1;

	public static int virtualButtonUpFrame_WentUp = int.MaxValue;

	public static int virtualButtonRightFrame_WentDown = -1;

	public static int virtualButtonRightFrame_WentUp = int.MaxValue;

	public static int virtualButtonDownFrame_WentDown = -1;

	public static int virtualButtonDownFrame_WentUp = int.MaxValue;

	public static int virtualButtonCancelFrame_WentDown = -1;

	public static int virtualButtonCancelFrame_WentUp = int.MaxValue;

	public static int virtualButtonFaceLeftFrame_WentDown = -1;

	public static int virtualButtonFaceLeftFrame_WentUp = int.MaxValue;

	public static int virtualButtonFaceUpFrame_WentDown = -1;

	public static int virtualButtonFaceUpFrame_WentUp = int.MaxValue;

	public static int virtualButtonSelectFrame_WentDown = -1;

	public static int virtualButtonSelectFrame_WentUp = int.MaxValue;

	public bool blocked
	{
		get
		{
			if (scnBase.instance is scnGame && scnGame.instance != null)
			{
				if (!RDInput.touchBlocked)
				{
					return scnGame.instance.paused;
				}
				return true;
			}
			return RDInput.touchBlocked;
		}
	}

	public RDInputType_Touch(int schemeIndex)
		: base(schemeIndex)
	{
	}

	public override bool Main(RDButtonState state)
	{
		return false;
	}

	public bool GetTouchDown(int playerIndex)
	{
		if (!blocked)
		{
			return GetTouchForPhase(playerIndex, TouchPhase.Began);
		}
		return false;
	}

	private bool GetTouch(int playerIndex)
	{
		if (!blocked)
		{
			return IsTouchPressed(playerIndex);
		}
		return false;
	}

	private bool GetTouchUp(int playerIndex)
	{
		if (!blocked)
		{
			return GetTouchForPhase(playerIndex, TouchPhase.Ended);
		}
		return false;
	}

	private bool GetTouchForPhase(int playerIndex, TouchPhase phase)
	{
		if (!blocked)
		{
			if (scnBase.instance is scnGame && scnGame.instance != null && GC.twoPlayerMode)
			{
				Touch[] touches = Input.touches;
				for (int i = 0; i < touches.Length; i++)
				{
					Touch touch = touches[i];
					int num = ((playerIndex == 0) ? scnGame.instance.leftFinger : scnGame.instance.rightFinger);
					if (touch.fingerId == num)
					{
						return touch.phase == phase;
					}
				}
			}
			else if (Input.touchCount > 0)
			{
				return Input.GetTouch(0).phase == phase;
			}
		}
		return false;
	}

	private bool IsTouchPressed(int playerIndex)
	{
		if (scnBase.instance is scnGame && scnGame.instance != null)
		{
			scnGame instance = scnGame.instance;
			return ((playerIndex == 0) ? instance.leftFinger : instance.rightFinger) != -1;
		}
		return Input.touchCount > 0;
	}

	public static void ResetAllVirtualButtons()
	{
		virtualButtonLeftFrame_WentDown = -1;
		virtualButtonLeftFrame_WentUp = int.MaxValue;
		virtualButtonUpFrame_WentDown = -1;
		virtualButtonUpFrame_WentUp = int.MaxValue;
		virtualButtonRightFrame_WentDown = -1;
		virtualButtonRightFrame_WentUp = int.MaxValue;
		virtualButtonDownFrame_WentDown = -1;
		virtualButtonDownFrame_WentUp = int.MaxValue;
		virtualButtonCancelFrame_WentDown = -1;
		virtualButtonCancelFrame_WentUp = int.MaxValue;
		virtualButtonFaceLeftFrame_WentDown = -1;
		virtualButtonFaceLeftFrame_WentUp = int.MaxValue;
		virtualButtonFaceUpFrame_WentDown = -1;
		virtualButtonFaceUpFrame_WentUp = int.MaxValue;
		virtualButtonSelectFrame_WentDown = -1;
		virtualButtonSelectFrame_WentUp = int.MaxValue;
	}

	private bool VirtualTouchCheck(RDButtonState state, int wentDownFrame, ref int wentUpFrame)
	{
		switch (state)
		{
		case RDButtonState.WentDown:
			if (Time.frameCount == wentDownFrame)
			{
				if (wentUpFrame < wentDownFrame)
				{
					wentUpFrame = int.MaxValue;
				}
				return true;
			}
			return false;
		case RDButtonState.WentUp:
			return Time.frameCount == wentUpFrame;
		case RDButtonState.IsDown:
			if (wentDownFrame > -1 && Time.frameCount >= wentDownFrame)
			{
				return Time.frameCount < wentUpFrame;
			}
			return false;
		default:
			return false;
		}
	}

	public override bool Left(RDButtonState state)
	{
		return VirtualTouchCheck(state, virtualButtonLeftFrame_WentDown, ref virtualButtonLeftFrame_WentUp);
	}

	public override bool Right(RDButtonState state)
	{
		return VirtualTouchCheck(state, virtualButtonRightFrame_WentDown, ref virtualButtonRightFrame_WentUp);
	}

	public override bool Up(RDButtonState state)
	{
		return VirtualTouchCheck(state, virtualButtonUpFrame_WentDown, ref virtualButtonUpFrame_WentUp);
	}

	public override bool Down(RDButtonState state)
	{
		return VirtualTouchCheck(state, virtualButtonDownFrame_WentDown, ref virtualButtonDownFrame_WentUp);
	}

	public override bool Skip()
	{
		return false;
	}

	public override bool Restart()
	{
		return false;
	}

	public override bool Cancel(RDButtonState state)
	{
		return VirtualTouchCheck(state, virtualButtonCancelFrame_WentDown, ref virtualButtonCancelFrame_WentUp);
	}

	public override bool Quit()
	{
		return false;
	}

	public override bool FinerControl(RDButtonState state)
	{
		return false;
	}

	public override bool FaceLeft(RDButtonState state)
	{
		return VirtualTouchCheck(state, virtualButtonFaceLeftFrame_WentDown, ref virtualButtonFaceLeftFrame_WentUp);
	}

	public override bool FaceUp(RDButtonState state)
	{
		return VirtualTouchCheck(state, virtualButtonFaceUpFrame_WentDown, ref virtualButtonFaceUpFrame_WentUp);
	}

	public override bool Select(RDButtonState state)
	{
		return VirtualTouchCheck(state, virtualButtonSelectFrame_WentDown, ref virtualButtonSelectFrame_WentUp);
	}
}
